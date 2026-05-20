using JModbusClient.Communicate;
using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JModbusClient
{
    public class JModbusTcpClient : JModbusBaseClient
    {
        protected MotorTcpClient _motorTcpClient;
        protected string _ip = "127.0.0.1";
        public string IP
        {
            get { return _ip; }
            set { _ip = value; }
        }
        protected int _port = 502;
        public override event ConnectedChangedHandler ConnectedChanged;
        public override event ReceiveActiveDataChangedHandler ReceiveActiveDataChanged;
        public override event ReceiveDataChangedHandler ReceiveDataChanged;
        public override event SendDataChangedHandler SendDataChanged;
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public override bool Connected
        {
            get
            {
                if (_motorTcpClient != null)
                {
                    return _motorTcpClient.Connected;
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
                if (_motorTcpClient != null)
                    _motorTcpClient.ConnectTimeout = _connectTimeout;
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
                if (_motorTcpClient != null)
                    _motorTcpClient.ReadTimeout = _readTimeout;
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
                if (_motorTcpClient != null)
                    _motorTcpClient.WriteTimeout = _writeTimeout;
            }
        }
        protected event MotorTcpClient.ReceiveDataChangedHandler ReceiveDataChangedEvent
        {
            add { _motorTcpClient.ReceiveDataChanged += value; }
            remove { _motorTcpClient.ReceiveDataChanged -= value; }
        }
        public override int BaudRate { get; set; }
        public override StopBits StopBits { get; set; }
        public override Parity Parity { get; set; }

        private bool _receivedhandler = false;

        private readonly object _receivelock = new object();
        public JModbusTcpClient(string ip, int port)
        {
            DebugRecord($"JModbus-TCP连接初始化 IP:{ip},PORT:{port}");
            _motorTcpClient = new MotorTcpClient(ip, port);
            this._ip = ip;
            this._port = port;
        }
        /// <summary>
        /// 主动模式接收数据处理事件
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void MotorReceiveDataHandler(object sender)
        {
            if (_receivedhandler)
                return;
            try
            {
                byte[] array = ReadMsgOnActiveMode(_readTimeout);
                if (array != null)
                {
                    int num = array.Length;
                    ReceiveActiveDataChanged?.Invoke(this);
                }
            }
            catch (Exception ex)
            {
                DebugRecord($"主动模式接收的数据异常{ex.Message}");
            }
            finally
            {
                //SetreceivedHandler(false);
            }
        }
        private byte[] ReadMsgOnActiveMode(int timeoutMs)
        {
            var receive = _motorTcpClient.Read(timeoutMs);
            //if(receive.Status == ReadStatus.Error)
            //    throw new ArgumentException(receive.Message);
            //if (receive.Status == ReadStatus.Timeout)
            //    throw new TimeoutException(receive.Message);
            //byte[] bytes1 = new byte[receive];
            //Array.Copy(bytes, 0, bytes1, 0, receive);
            //            DebugRecord("Read ModbusTCPActive-Data:  " + BitConverter.ToString(bytes1) + ", bufLen: " + receive);
            return receive.Data;
        }

        public override void Connect()
        {
            if (_motorTcpClient != null)
            {
                DebugRecord($"Open TCP-Socket,IP:{_ip},PORT:{_port}");
                _motorTcpClient.Connect(_ip, _port);
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
        public override async Task ConnectAsync(Action action = null)
        {
            DebugRecord($"Open TCP-Socket,IP:{_ip},PORT:{_port}");
            _motorTcpClient.ConnectTimeout = 2000;
            await _motorTcpClient.ConnectAsync(action);
        }
        public override void Disconnect()
        {
            if (_motorTcpClient != null)
            {
                DebugRecord($"Close TCP-Socket,IP:{_ip},PORT:{_port}");
                _motorTcpClient.Disconnect();
            }
            ConnectedChanged?.Invoke(this);
        }

        public override void Dispose()
        {
            _motorTcpClient?.Dispose();
        }
        protected override byte[] SendReceive(byte[] request, int targetReglen)
        {
            SendMsgOnMasterSlaveMode(request, 0, request.Length - 2);
            SendDataChanged?.Invoke(this);
            byte[] array = ReadMsgOnMasterSlaveMode(_readTimeout);
            return array;

        }
        protected override async Task<byte[]> SendReceiveAsync(byte[] request, int targetReglen, CancellationToken token = default)
        {

            await SendMsgOnMasterSlaveModeAsync(request, 0, request.Length - 2, token);
            SendDataChanged?.Invoke(this);
            byte[] array = await ReadMsgOnMasterSlaveModeAsync(_readTimeout, token);
            return array;

        }
        private byte[] ReadMsgOnMasterSlaveMode(int timeoutMs)
        {
            var receive = _motorTcpClient.Read(timeoutMs);
            if (receive.Status == ReadStatus.Error)
                throw new ArgumentException("JModbusTCP接收数据异常" + receive.Message);
            if (receive.Status == ReadStatus.Timeout)
                throw new ArgumentException("JModbusTCP接收数据超时" + receive.Message);
            DebugRecord("Read ModbusTCPSlave-Data:  " + BitConverter.ToString(receive.Data) + ", bufLen: " + receive.Data.Length);
            return receive.Data;
        }
        private async Task<byte[]> ReadMsgOnMasterSlaveModeAsync(int timeoutMs, CancellationToken token = default)
        {
            var receive = await _motorTcpClient.ReadAsync(timeoutMs, token);
            if (receive.Status == ReadStatus.Error)
                throw new ArgumentException("JModbusTCP接收数据异常" + receive.Message);
            if (receive.Status == ReadStatus.Timeout)
                throw new TimeoutException("JModbusTCP接收数据超时" + receive.Message);
            DebugRecord("Read ModbusTCPSlave-Data:  " + BitConverter.ToString(receive.Data) + ", bufLen: " + receive.Data.Length);
            return receive.Data;
        }
        private void SendMsgOnMasterSlaveMode(byte[] bytes, int offset, int bufLen)
        {
            _motorTcpClient.DiscardInBuffer();
            _motorTcpClient.Write(bytes, offset, bufLen);
            //确保缓冲区没有数据
            DebugRecord("Send ModbusTCP-Data:  " + BitConverter.ToString(bytes) + ", bufLen: " + bufLen);
        }
        private async Task SendMsgOnMasterSlaveModeAsync(byte[] bytes, int offset, int bufLen, CancellationToken token = default)
        {
            await _motorTcpClient.DiscardInBufferAsync();
            await _motorTcpClient.WriteAsync(bytes, offset, bufLen, token);
            //确保缓冲区没有数据
            DebugRecord("Send ModbusTCP-Data:  " + BitConverter.ToString(bytes) + ", bufLen: " + bufLen);
        }
    }
}
