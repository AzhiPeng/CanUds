using JModbusClient.Communicate;
using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace JModbusClient
{
    public class JModbusRtuClient : JModbusBaseClient
    {
        private MotorRtuClient _motorRtuClient;
        private bool _dataReceived;

        public override int BaudRate
        {
            get { return _baudRate; }
            set
            {
                _baudRate = value;
                _motorRtuClient.BaudRate = _baudRate;
            }
        }
        public override Parity Parity
        {
            get { return _parity; }
            set
            {
                _parity = value;
                _motorRtuClient.Parity = _parity;
            }
        }
        public override StopBits StopBits
        {
            get { return _stopBits; }
            set
            {
                _stopBits = value;
                _motorRtuClient.StopBits = _stopBits;
            }
        }
        public override bool Connected
        {
            get
            {
                if (_motorRtuClient != null)
                {
                    return _motorRtuClient.Connected;
                }
                return false;
            }
        }
        public override int ConnectTimeout
        {
            get { return _connectTimeout; }
            set
            {
                _connectTimeout = value;
            }
        }
        public override int ReadTimeout
        {
            get
            {
                return _readTimeout;
            }
            set
            {
                _readTimeout = value;
                if (_motorRtuClient != null)
                    _motorRtuClient.ReadTimeout = _readTimeout;
            }
        }
        public override int WriteTimeout
        {
            get
            {
                return _writeTimeout;
            }
            set
            {
                _writeTimeout = value;
                if (_motorRtuClient != null)
                    _motorRtuClient.WriteTimeout = _writeTimeout;
            }
        }

        public override event ConnectedChangedHandler ConnectedChanged;
        public override event ReceiveActiveDataChangedHandler ReceiveActiveDataChanged;
        public override event ReceiveDataChangedHandler ReceiveDataChanged;
        public override event SendDataChangedHandler SendDataChanged;

        public JModbusRtuClient(string serialName, int baudRate = 9600,
            Parity parity = Parity.Even, StopBits stopBits = StopBits.One)
        {
            _motorRtuClient = new MotorRtuClient(serialName);
            BaudRate = baudRate;
            _motorRtuClient.BaudRate = BaudRate;
            this.Parity = parity;
            _motorRtuClient.Parity = Parity;
            this.StopBits = stopBits;
            _motorRtuClient.StopBits = StopBits;
            _motorRtuClient.WriteTimeout = _writeTimeout;
            _motorRtuClient.ReadTimeout = _readTimeout;
            _motorRtuClient.ReceiveDataChanged += MotorReceiveDataHandler;
        }

        private void MotorReceiveDataHandler(object sender)
        {
            try
            {
                //TODO target 读取数量在主动模式下需要判断
                int bytesToRead = 5 + 2 * 12;
                byte[] array = ReadSerialMsgOnActiveMode(bytesToRead, _readTimeout);
                ValidateResponse(array, 3);
                int[] array2 = ParseModbusResponse(array);
                TryAddCollection(array2);
                ReceiveActiveDataChanged?.Invoke(this);
            }
            catch (Exception ex)
            {
                DebugRecord($"主动模式接收的数据异常{ex.Message}");
            }
        }

        private static int[] ParseModbusResponse(byte[] array)
        {
            int datalen = array[8];
            int[] array2 = new int[datalen];
            for (int i = 0; i < datalen; i++)
            {
                byte b2 = array[9 + i * 2];
                byte b3 = array[9 + i * 2 + 1];
                array[9 + i * 2] = b3;
                array[9 + i * 2 + 1] = b2;
                array2[i] = BitConverter.ToInt16(array, 9 + i * 2);
            }

            return array2;
        }

        private byte[] ReadSerialMsgOnActiveMode(int targetNum, int timeoutMs)
        {
            var receive = ReadSerialCheckOnModbusprotocol(targetNum, timeoutMs);
            DebugRecord("Read Serial-Data:  " + BitConverter.ToString(receive) + ", bufLen: " + receive.Length);
            return receive;
        }
        // 2. 同步方法包装
        private byte[] ReadSerialCheckOnModbusprotocol(int targetNums, int timeoutMs)
        {
            return ReadSerialCheckOnModbusprotocolAsync(
                targetNums,
                timeoutMs
            ).GetAwaiter().GetResult();
        }

        // 3. 异步方法包装
        private async Task<byte[]> ReadSerialCheckOnModbusprotocolAsync(int targetNums, int timeoutMs, CancellationToken token = default)
        {
            return await ReadSerialCheckOnModbusCoreprotocolAsync(targetNums,timeoutMs, token);
        }        
        private async Task<byte[]> ReadSerialCheckOnModbusCoreprotocolAsync(int targetNums, int timeoutMs, CancellationToken token = default)
        {
            //串口通信一次性可能收不全数据需要分包处理
            int modbuslen = 0;
            int num;
            byte[] readBuffer = new byte[256];
            DateTime now = DateTime.Now;
            do
            {
                var receive = await _motorRtuClient.ReadAsync(timeoutMs,cancellationToken: token).ConfigureAwait(false);
                if (receive.Status == ReadStatus.Error)
                    throw new ArgumentException("JModbusRtu接收异常" + receive.Message);
                else if (receive.Status == ReadStatus.Timeout)
                    throw new TimeoutException("JModbusRtu接收超时" + receive.Message);
                num = receive.Data.Length;
                if (num > 0)
                {
                    byte[] array = receive.Data;
                    Array.Copy(array, 0, readBuffer, modbuslen, (modbuslen + array.Length <= targetNums) ? array.Length : (targetNums - modbuslen));
                    modbuslen += array.Length;
                }
            } while (targetNums > modbuslen &&
                !(DetectValidModbusFrame(readBuffer, (modbuslen < readBuffer.Length) ? modbuslen : readBuffer.Length) | (targetNums <= modbuslen)) &&
                    (DateTime.Now - now).TotalMilliseconds < _readTimeout);
            var len = Math.Min(modbuslen, readBuffer.Length);
            byte[] receiveData = new byte[len];
            Array.Copy(readBuffer, 0, receiveData, 0, len);
            _dataReceived = true;
            return receiveData;
        }
        /// <summary>
        /// 验证modbus数据的完整性
        /// </summary>
        /// <param name="readBuffer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private bool DetectValidModbusFrame(byte[] readBuffer, int length)
        {
            if (length < 6)
            {
                return false;
            }

            if ((readBuffer[0] < 1) | (readBuffer[0] > 247))
            {
                return false;
            }

            byte[] array = new byte[2];
            checked
            {
                array = BitConverter.GetBytes(CalcCRC.calculateCRC(readBuffer, (ushort)(length - 2), 0));
                if ((array[0] != readBuffer[length - 2]) | (array[1] != readBuffer[length - 1]))
                {
                    return false;
                }

                return true;
            }
        }

        public override void Dispose()
        {
            _motorRtuClient.Dispose();
        }
        public override void Connect()
        {
            if (_motorRtuClient != null)
            {
                DebugRecord($"Open serial,RTUName:{_motorRtuClient.SerialPort}");
                _motorRtuClient.Connect();
            }
            try
            {
                ConnectedChanged?.Invoke(this);
            }
            catch (Exception ex)
            {
                DebugRecord($"Open TCP-Socket Exception:{ex.Message}");
            }
        }

        public override void Disconnect()
        {
            if (_motorRtuClient != null)
            {
                DebugRecord($"Close serial,RTUName:{_motorRtuClient.SerialPort}");
                _motorRtuClient?.Disconnect();
            }
            ConnectedChanged?.Invoke(this);
        }

        protected override byte[] SendReceive(byte[] request, int targetReglen)
        {
            byte gate = byte.MaxValue;
            SendSerialMsgOnMasterSlaveMode(request, 6, request.Length - 6);
            SendDataChanged?.Invoke(this);
            _dataReceived = false;
            int bytesToRead = targetReglen;
            var array = ReadSerialMsgOnMasterSlaveMode(bytesToRead, _readTimeout);
            ReceiveDataChanged?.Invoke(this);
            gate = array[6];
            if (gate != _unitIdentifier)
            {
                array = null;
            }
            return array;
        }
        protected override async Task<byte[]> SendReceiveAsync(byte[] request, int targetReglen,CancellationToken token=default)
        {
            byte gate = byte.MaxValue;
            await SendSerialMsgOnMasterSlaveModeAsync(request, 6, request.Length - 6, token);
            SendDataChanged?.Invoke(this);
            _dataReceived = false;
            int bytesToRead = targetReglen;
            var array = await ReadSerialMsgOnMasterSlaveModeAsync(bytesToRead, _readTimeout, token);
            ReceiveDataChanged?.Invoke(this);
            gate = array[6];
            if (gate != _unitIdentifier)
            {
                array = null;
            }
            return array;
        }
        protected override bool ValidateResponse(byte[] response, int fcode, int countRetries = 0)
        {
            if (!base.ValidateResponse(response, fcode, countRetries))
                return false;
            crc = BitConverter.GetBytes(CalcCRC.calculateCRC(response, (ushort)(response[8] + 3), 6));
            if (((crc[0] != response[response.Length - 2]) | (crc[1] != response[response.Length - 1])))
            {
                DebugRecord("CRCCheckFailedException Throwed");
                if (NumberOfRetries <= countRetries)
                {
                    throw new CRCCheckFailedException("Response CRC check failed");
                }
                return false;
            }

            if (!_dataReceived)
            {
                DebugRecord("TimeoutException Throwed");
                if (NumberOfRetries <= _countRetries)
                {
                    throw new TimeoutException("No Response from Modbus Slave");
                }
                return false;
            }
            return true;
        }
        private byte[] ReadSerialMsgOnMasterSlaveMode(int targetNums, int readTimeout)
        {
            var receive = ReadSerialCheckOnModbusprotocol(targetNums, readTimeout);
            _motorRtuClient.ReceiveDataChanged -= MotorReceiveDataHandler;
            _motorRtuClient.ReceiveDataChanged += MotorReceiveDataHandler;
            DebugRecord("Read ModbusTCP-Data:  " + BitConverter.ToString(receive) + ", bufLen: " + receive.Length);
            return receive;
        }
        private async Task<byte[]> ReadSerialMsgOnMasterSlaveModeAsync(int targetNums, int readTimeout, CancellationToken token = default)
        {
            var receive = await ReadSerialCheckOnModbusprotocolAsync(targetNums, readTimeout, token);
            _motorRtuClient.ReceiveDataChanged -= MotorReceiveDataHandler;
            _motorRtuClient.ReceiveDataChanged += MotorReceiveDataHandler;
            DebugRecord("Read ModbusTCP-Data:  " + BitConverter.ToString(receive) + ", bufLen: " + receive.Length);
            return receive;
        }
        private void SendSerialMsgOnMasterSlaveMode(byte[] bytes, int offset, int bufLen)
        {
            _motorRtuClient.Write(bytes, offset, bufLen);
            _motorRtuClient.ReceiveDataChanged -= MotorReceiveDataHandler;
            DebugRecord("Send ModbusTCP-Data:  " + BitConverter.ToString(bytes) + ", bufLen: " + bufLen);
        }
        private async Task SendSerialMsgOnMasterSlaveModeAsync(byte[] bytes, int offset, int bufLen,CancellationToken token = default)
        {
            await _motorRtuClient.WriteAsync(bytes, offset, bufLen, token);
            _motorRtuClient.ReceiveDataChanged -= MotorReceiveDataHandler;
            DebugRecord("Send ModbusTCP-Data:  " + BitConverter.ToString(bytes) + ", bufLen: " + bufLen);
        }
    }
}
