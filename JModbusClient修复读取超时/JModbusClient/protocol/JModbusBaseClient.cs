using JModbusClient.Log;
using System;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace JModbusClient
{
    public abstract class JModbusBaseClient : IJModbusClient
    {
        public enum ModbusRegisterType
        {
            Coil,
            DiscreteInput,
            HoldingRegister,
            InputRegister
        }
        public abstract bool Connected { get; }
        public abstract int ConnectTimeout { get; set; }
        public abstract int ReadTimeout { get; set; }
        public abstract int WriteTimeout { get; set; }
        public abstract int BaudRate { get; set; }
        public abstract StopBits StopBits { get; set; }
        public abstract Parity Parity { get; set; }
        public int DataActivebufCount { get { return _dataCollection.Count; } }
        public bool DataAvailable { get { return _dataCollection.Count > 0; } }
        public bool ActiveModeOn { get; set; }

        protected byte _unitIdentifier = 1;

        protected int _numberOfRetries = 3;

        protected bool debug = false;

        protected int _connectTimeout = 3000;

        protected int _writeTimeout = 2000;

        protected int _readTimeout = 2000;

        protected int _countRetries = 0;
        protected StopBits _stopBits = StopBits.One;
        protected Parity _parity = Parity.Even;
        protected int _baudRate = 9600;
        protected uint transactionIdentifierInternal = 0u;
        protected byte[] transactionIdentifier = new byte[2];
        protected byte[] protocolIdentifier = new byte[2];
        protected byte[] quantities = new byte[2];
        protected byte[] Address = new byte[2];
        protected byte[] crc = new byte[2];
        protected byte[] length = new byte[2];
        protected byte functionCode;

        private BlockingCollection<int[]> _dataCollection = new BlockingCollection<int[]>();
        private int _safeCount;
        private DateTime _lastModbusOpTime;
        private readonly object _slavelocker = new object();

        public int NumberOfRetries
        {
            get
            {
                return _numberOfRetries;
            }
            set
            {
                _numberOfRetries = value;
            }
        }
        public byte UnitIdentifier
        {
            get
            {
                return _unitIdentifier;
            }
            set
            {
                _unitIdentifier = value;
            }
        }
        public string Logfilename
        {
            get { return StoreLog.Instance.Filename; }
            set
            {
                StoreLog.Instance.Filename = value;
                if (StoreLog.Instance.Filename != null)
                    debug = true;
                else
                    debug = false;
            }
        }
        public abstract event ConnectedChangedHandler ConnectedChanged;
        public abstract event ReceiveActiveDataChangedHandler ReceiveActiveDataChanged;
        public abstract event ReceiveDataChangedHandler ReceiveDataChanged;
        public abstract event SendDataChangedHandler SendDataChanged;
        public abstract void Connect();
        public virtual async Task ConnectAsync(Action action = null)
        {
            await Task.FromResult(0);
        }
        public abstract void Disconnect();
        protected abstract byte[] SendReceive(byte[] request, int targetReglen);

        protected abstract Task<byte[]> SendReceiveAsync(byte[] request, int targetReglen, CancellationToken token = default);
        public void DiscardInBuffer()
        {
            _dataCollection = new BlockingCollection<int[]>();
        }
        public void SetSlaveAddress(byte slaveAddress)
        {
            _unitIdentifier = slaveAddress;
        }
        protected bool TryAddCollection(int[] ints)
        {
            if (ActiveModeOn)
                return _dataCollection.TryAdd(ints);
            return false;
        }
        public virtual int[] Read(int timeoutMs = 1000)
        {           
            if (_dataCollection.TryTake(out var data, timeoutMs))
            {               
                return data;
            }
            return null;
        }
        public virtual bool[] ReadCoil(int startingAddress, int quantity, int countRetries = 0)
        {
            VerifyConnection();
            DebugRecord("(Read Coil from Master device), StartingAddress: " + startingAddress + ", Quantity: " + quantity);
            ValidateAddressAndQuantity(startingAddress, quantity);
            functionCode = 1;
            var request = CreateReadRequest(startingAddress, quantity);
            var response = SendReceive(request, 5 + 2 * quantity);
            if (!ValidateResponse(response, functionCode, countRetries))
                return ReadCoil(startingAddress, quantity, countRetries++);
            return ParseRegisterValues(response, quantity, true);
        }

        public virtual async Task<bool[]> ReadCoilAsync(int startingAddress, int quantity, CancellationToken token = default,int countRetries = 0)
        {
            VerifyConnection();
            DebugRecord("(Read Coil from Master device), StartingAddress: " + startingAddress + ", Quantity: " + quantity);
            ValidateAddressAndQuantity(startingAddress, quantity);
            functionCode = 1;
            var request = CreateReadRequest(startingAddress, quantity);
            var response = await SendReceiveAsync(request, 5 + 2 * quantity, token);
            if (!ValidateResponse(response, functionCode, countRetries))
                return await ReadCoilAsync(startingAddress, quantity, token, countRetries++);
            return ParseRegisterValues(response, quantity, true);
        }
        public virtual bool[] ReadDiscreteInputs(int startingAddress, int quantity, int countRetries = 0)
        {
            VerifyConnection();
            DebugRecord("(Read DiscreteInputs from Master device), StartingAddress: " + startingAddress + ", Quantity: " + quantity);
            ValidateAddressAndQuantity(startingAddress, quantity);
            functionCode = 2;
            var request = CreateReadRequest(startingAddress, quantity);
            var response = SendReceive(request, 5 + 2 * quantity);
            if (!ValidateResponse(response, functionCode, countRetries))
                return ReadDiscreteInputs(startingAddress, quantity, countRetries++);
            return ParseRegisterValues(response, quantity, true);
        }

        public virtual async Task<bool[]> ReadDiscreteInputsAsync(int startingAddress, int quantity, CancellationToken token = default, int countRetries = 0)
        {
            VerifyConnection();
            DebugRecord("(Read DiscreteInputs from Master device), StartingAddress: " + startingAddress + ", Quantity: " + quantity);
            ValidateAddressAndQuantity(startingAddress, quantity);
            functionCode = 2;
            var request = CreateReadRequest(startingAddress, quantity);
            var response = await SendReceiveAsync(request, 5 + 2 * quantity, token);
            if (!ValidateResponse(response, functionCode, countRetries))
                return await ReadDiscreteInputsAsync(startingAddress, quantity, token, countRetries++);
            return ParseRegisterValues(response, quantity, true);
        }
        public virtual int[] ReadHoldingRegister(int startingAddress, int quantity, int countRetries = 0)
        {
            VerifyConnection();
            DebugRecord("(Read Holding Registers from Master device), StartingAddress: " + startingAddress + ", Quantity: " + quantity);
            ValidateAddressAndQuantity(startingAddress, quantity);
            functionCode = 3;
            var request = CreateReadRequest(startingAddress, quantity);
            var response = SendReceive(request, 5 + 2 * quantity);
            if (!ValidateResponse(response, functionCode, countRetries))
                return ReadHoldingRegister(startingAddress, quantity, countRetries++);
            return ParseRegisterValues(response, quantity);
        }

        public virtual async Task<int[]> ReadHoldingRegisterAsync(int startingAddress, int quantity, CancellationToken token = default, int countRetries = 0)
        {
            VerifyConnection();
            DebugRecord("(Read Holding Registers from Master device), StartingAddress: " + startingAddress + ", Quantity: " + quantity);
            ValidateAddressAndQuantity(startingAddress, quantity);
            functionCode = 3;
            var request = CreateReadRequest(startingAddress, quantity);
            var response = await SendReceiveAsync(request, 5 + 2 * quantity, token);
            if (!ValidateResponse(response, functionCode, countRetries))
                return await ReadHoldingRegisterAsync(startingAddress, quantity, token, countRetries++);
            return ParseRegisterValues(response, quantity);
        }
        public virtual int[] ReadInputRegisters(int startingAddress, int quantity, int countRetries = 0)
        {
            VerifyConnection();
            DebugRecord("(Read Input Registers from Master device), StartingAddress: " + startingAddress + ", Quantity: " + quantity);
            ValidateAddressAndQuantity(startingAddress, quantity);
            functionCode = 4;
            var request = CreateReadRequest(startingAddress, quantity);
            var response = SendReceive(request, 5 + 2 * quantity);
            if (!ValidateResponse(response, functionCode, countRetries))
                return ReadInputRegisters(startingAddress, quantity, countRetries++);
            return ParseRegisterValues(response, quantity);
        }
        public virtual async Task<int[]> ReadInputRegistersAsync(int startingAddress, int quantity, CancellationToken token = default, int countRetries = 0)
        {
            VerifyConnection();
            DebugRecord("(Read Input Registers from Master device), StartingAddress: " + startingAddress + ", Quantity: " + quantity);
            ValidateAddressAndQuantity(startingAddress, quantity);
            functionCode = 4;
            var request = CreateReadRequest(startingAddress, quantity);
            var response = await SendReceiveAsync(request, 5 + 2 * quantity, token);
            if (!ValidateResponse(response, functionCode, countRetries))
                return await ReadInputRegistersAsync(startingAddress, quantity, token, countRetries++);
            return ParseRegisterValues(response, quantity);
        }
        public virtual void WriteSingleRegister(int startingAddress, int value, int countRetries = 0)
        {
            VerifyConnection();
            DebugRecord("(Write Single Registers from Master device), StartingAddress: " + startingAddress + ", value: " + value);
            ValidateAddressAndValue(startingAddress, ModbusRegisterType.HoldingRegister, value);
            functionCode = 6;
            var request = CreateWriteRequest(startingAddress, value);
            var response = SendReceive(request, 8);
            if (!ValidateResponse(response, functionCode, countRetries))
                WriteSingleRegister(startingAddress, value, countRetries++);
        }

        public virtual async Task WriteSingleRegisterAsync(int startingAddress, int value, CancellationToken token = default, int countRetries = 0)
        {
            VerifyConnection();
            DebugRecord("(Write Single Registers from Master device), StartingAddress: " + startingAddress + ", value: " + value);
            ValidateAddressAndValue(startingAddress, ModbusRegisterType.HoldingRegister, value);
            functionCode = 6;
            var request = CreateWriteRequest(startingAddress, value);
            var response = await SendReceiveAsync(request, 8, token);
            if (!ValidateResponse(response, functionCode, countRetries))
                await WriteSingleRegisterAsync(startingAddress, value, token, countRetries++);
        }
        public virtual void WriteMultipleRegisters(int startingAddress, int[] values, int countRetries = 0)
        {
            VerifyConnection();
            DebugRecord("(Write Single Registers from Master device), StartingAddress: " + startingAddress + ", value: " + string.Join(", ", Array.ConvertAll(values, x => x.ToString())));
            ValidateAddressAndValue(startingAddress, ModbusRegisterType.HoldingRegister, values);
            functionCode = 16;
            var request = CreateWriteRequest(startingAddress, values);
            var response = SendReceive(request, 8);
            if (!ValidateResponse(response, functionCode, countRetries))
                WriteMultipleRegisters(startingAddress, values, countRetries++);
        }
        public virtual async Task WriteMultipleRegistersAsync(int startingAddress, int[] values, CancellationToken token = default, int countRetries = 0)
        {
            VerifyConnection();
            DebugRecord("(Write Single Registers from Master device), StartingAddress: " + startingAddress + ", value: " + string.Join(", ", Array.ConvertAll(values, x => x.ToString())));
            ValidateAddressAndValue(startingAddress, ModbusRegisterType.HoldingRegister, values);
            functionCode = 16;
            var request = CreateWriteRequest(startingAddress, values);
            var response = await SendReceiveAsync(request, 8, token);
            if (!ValidateResponse(response, functionCode, countRetries))
                await WriteMultipleRegistersAsync(startingAddress, values, token, countRetries++);
        }
        public virtual void WriteSingleCoil(int startingAddress, bool value, int countRetries = 0)
        {
            VerifyConnection();
            DebugRecord("(Write Single Registers from Master device), StartingAddress: " + startingAddress + ", value: " + value);
            ValidateAddressAndValue(startingAddress, ModbusRegisterType.Coil, value);
            functionCode = 5;
            var request = CreateWriteRequest(startingAddress, value);
            var response = SendReceive(request, 8);
            if (!ValidateResponse(response, functionCode, countRetries))
                WriteSingleCoil(startingAddress, value, countRetries++);
        }
        public virtual async Task WriteSingleCoilAsync(int startingAddress, bool value, CancellationToken token = default, int countRetries = 0)
        {
            VerifyConnection();
            DebugRecord("(Write Single Registers from Master device), StartingAddress: " + startingAddress + ", value: " + value);
            ValidateAddressAndValue(startingAddress, ModbusRegisterType.Coil, value);
            functionCode = 5;
            var request = CreateWriteRequest(startingAddress, value);
            var response = await SendReceiveAsync(request, 8, token);
            if (!ValidateResponse(response, functionCode, countRetries))
                await WriteSingleCoilAsync(startingAddress, value, token, countRetries++);
        }
        public virtual void WriteMultipleCoil(int startingAddress, bool[] values, int countRetries = 0)
        {
            VerifyConnection();
            DebugRecord("(Write Single Registers from Master device), StartingAddress: " + startingAddress + ", value: " + string.Join(", ", Array.ConvertAll(values, x => x.ToString())));
            ValidateAddressAndValue(startingAddress, ModbusRegisterType.Coil, values);
            functionCode = 15;
            var request = CreateWriteRequest(startingAddress, values);
            var response = SendReceive(request, 8);
            if (!ValidateResponse(response, functionCode, countRetries))
                WriteMultipleCoil(startingAddress, values, countRetries++);
        }
        public virtual async Task WriteMultipleCoilAsync(int startingAddress, bool[] values, CancellationToken token = default, int countRetries = 0)
        {
            VerifyConnection();
            DebugRecord("(Write Single Registers from Master device), StartingAddress: " + startingAddress + ", value: " + string.Join(", ", Array.ConvertAll(values, x => x.ToString())));
            ValidateAddressAndValue(startingAddress, ModbusRegisterType.Coil, values);
            functionCode = 15;
            var request = CreateWriteRequest(startingAddress, values);
            var response = await SendReceiveAsync(request, 8, token);
            if (!ValidateResponse(response, functionCode, countRetries))
                await WriteMultipleCoilAsync(startingAddress, values, token, countRetries++);
        }
        protected virtual byte[] CreateWriteRequest(int startingAddress, bool[] values)
        {
            byte datalen = (byte)((unchecked(values.Length % 8) != 0) ? (unchecked(values.Length / 8) + 1) : unchecked(values.Length / 8));
            transactionIdentifierInternal++;
            transactionIdentifier = BitConverter.GetBytes(transactionIdentifierInternal);
            protocolIdentifier = BitConverter.GetBytes(0);
            length = BitConverter.GetBytes(7 + datalen);
            Address = BitConverter.GetBytes(startingAddress);
            byte[] bytes = BitConverter.GetBytes(values.Length);
            byte[] array = new byte[15 + datalen];
            byte dataval = 0;
            array[0] = transactionIdentifier[1];
            array[1] = transactionIdentifier[0];
            array[2] = protocolIdentifier[1];
            array[3] = protocolIdentifier[0];
            array[4] = length[1];
            array[5] = length[0];
            array[6] = _unitIdentifier;
            array[7] = functionCode;
            array[8] = Address[1];
            array[9] = Address[0];
            array[10] = bytes[1];
            array[11] = bytes[0];
            array[12] = datalen;
            for (int j = 0; j < values.Length; j++)
            {
                byte b3;
                unchecked
                {
                    if (j % 8 == 0)
                    {
                        dataval = 0;
                    }

                    b3 = (byte)(values[j] ? 1 : 0);
                }

                dataval = (byte)((b3 << unchecked(j % 8)) | dataval);
                array[13 + unchecked(j / 8)] = dataval;
            }
            crc = BitConverter.GetBytes(CalcCRC.calculateCRC(array, (ushort)(array.Length - 8), 6));
            array[12] = crc[0];
            array[13] = crc[1];
            return array;
        }
        protected virtual byte[] CreateWriteRequest(int startingAddress, bool value)
        {
            transactionIdentifierInternal++;
            transactionIdentifier = BitConverter.GetBytes(transactionIdentifierInternal);
            protocolIdentifier = BitConverter.GetBytes(0);
            length = BitConverter.GetBytes(4 + 2);
            Address = BitConverter.GetBytes(startingAddress);
            byte[] bytes = ((!value) ? BitConverter.GetBytes(0) : BitConverter.GetBytes(65280));
            byte[] array = new byte[14];
            array[0] = transactionIdentifier[1];
            array[1] = transactionIdentifier[0];
            array[2] = protocolIdentifier[1];
            array[3] = protocolIdentifier[0];
            array[4] = length[1];
            array[5] = length[0];
            array[6] = _unitIdentifier;
            array[7] = functionCode;
            array[8] = Address[1];
            array[9] = Address[0];
            array[10] = bytes[1];
            array[11] = bytes[0];
            crc = BitConverter.GetBytes(CalcCRC.calculateCRC(array, (ushort)(4 + 2), 6));
            array[12] = crc[0];
            array[13] = crc[1];
            return array;
        }
        protected virtual byte[] CreateWriteRequest(int startingAddress, params int[] values)
        {
            int tmpLen = values.Length * 2;
            byte b = (byte)(values.Length * 2);
            transactionIdentifierInternal++;
            transactionIdentifier = BitConverter.GetBytes(transactionIdentifierInternal);
            protocolIdentifier = BitConverter.GetBytes(0);
            if (functionCode > 10)
            {
                tmpLen += 3;
            }
            length = BitConverter.GetBytes(4 + tmpLen);
            Address = BitConverter.GetBytes(startingAddress);
            byte[] array = new byte[12 + tmpLen];
            array[0] = transactionIdentifier[1];
            array[1] = transactionIdentifier[0];
            array[2] = protocolIdentifier[1];
            array[3] = protocolIdentifier[0];
            array[4] = length[1];
            array[5] = length[0];
            array[6] = _unitIdentifier;
            array[7] = functionCode;
            array[8] = Address[1];
            array[9] = Address[0];
            int tmpindex = 10;
            if (functionCode > 10)
            {
                byte[] bytes = BitConverter.GetBytes(values.Length);
                array[tmpindex++] = bytes[1];
                array[tmpindex++] = bytes[0];
                array[tmpindex++] = b;
            }
            for (int j = 0; j < values.Length; j++)
            {
                byte[] bytes2 = BitConverter.GetBytes(values[j]);
                array[tmpindex++] = bytes2[1];
                array[tmpindex++] = bytes2[0];
            }
            crc = BitConverter.GetBytes(CalcCRC.calculateCRC(array, (ushort)(4 + tmpLen), 6));
            array[tmpindex++] = crc[0];
            array[tmpindex++] = crc[1];
            return array;
        }

        protected virtual byte[] CreateReadRequest(int startingAddress, int quantity)
        {
            transactionIdentifierInternal++;
            transactionIdentifier = BitConverter.GetBytes(transactionIdentifierInternal);
            protocolIdentifier = BitConverter.GetBytes(0);
            length = BitConverter.GetBytes(6);
            byte[] Address = BitConverter.GetBytes(startingAddress);
            byte[] quantities = BitConverter.GetBytes(quantity);
            byte[] array = new byte[14]
            {
                 transactionIdentifier[1],
                 transactionIdentifier[0],
                 protocolIdentifier[1],
                 protocolIdentifier[0],
                 length[1],
                 length[0],
                 _unitIdentifier,
                 functionCode,
                 Address[1],
                 Address[0],
                 quantities[1],
                 quantities[0],
                 crc[0],
                 crc[1]
            };
            crc = BitConverter.GetBytes(CalcCRC.calculateCRC(array, 6, 6));
            array[12] = crc[0];
            array[13] = crc[1];
            return array;
        }
        protected virtual void VerifyConnection()
        {
            if (!Connected)
            {
                DebugRecord("JModbus-TCP-Socket未连接");
                    throw new Exception("JModbus-TCP-Socket未连接");
            }
        }

        protected void DebugRecord(string msg)
        {
            if (debug)
            {
                StoreLog.Instance.Store(msg);
            }
        }
        private void ValidateAddressAndQuantity(int startingAddress, int quantity)
        {
            if (startingAddress < 0 || startingAddress > 65535)
                throw new ArgumentException("Starting address must be 0-65535");

            if (quantity < 1 || quantity > 2000)
                throw new ArgumentException("Quantity must be 1-2000");
        }
        private void ValidateAddressAndValue<T>(int startingAddress, ModbusRegisterType registerType, params T[] values)
        {
            if (startingAddress < 0 || startingAddress > 65535)
                throw new ArgumentException("Starting address must be 0-65535");

            switch (registerType)
            {
                case ModbusRegisterType.Coil:
                case ModbusRegisterType.DiscreteInput:
                    break;
                case ModbusRegisterType.HoldingRegister:
                case ModbusRegisterType.InputRegister:
                    // 16位寄存器
                    if (typeof(T) != typeof(int))
                        throw new ArgumentException($"Type {typeof(T)} is invalid for {registerType}. Must be int.");
                    if (typeof(T) == typeof(int))
                    {
                        foreach (var value in values)
                        {
                            int intValue = Convert.ToInt32(value);
                            if (intValue < 0 || intValue > 65535)
                                throw new ArgumentException($"Value {value} is invalid for {registerType}. Must be 0-65535");
                        }
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(registerType), registerType, null);
            }
            // 数量验证（Modbus协议限制单次操作最多125个寄存器）
            if (values.Length > 125)
                throw new ArgumentException("Modbus protocol limits maximum registers to 125 per operation");
        }

        protected virtual int[] ParseRegisterValues(byte[] response, int quantity)
        {
            int[] array2 = new int[quantity];
            for (int k = 0; k < quantity; k++)
            {
                byte b3 = response[9 + k * 2];
                byte b4 = response[9 + k * 2 + 1];
                response[9 + k * 2] = b4;
                response[9 + k * 2 + 1] = b3;
                array2[k] = BitConverter.ToInt16(response, 9 + k * 2);
            }
            return array2;
        }
        protected virtual bool[] ParseRegisterValues(byte[] response, int quantity, bool isbool)
        {
            bool[] array2 = new bool[quantity];
            for (int i = 0; i < quantity; i++)
            {
                int num2 = response[9 + unchecked(i / 8)];
                unchecked
                {
                    int num3 = Convert.ToInt32(Math.Pow(2.0, i % 8));
                    array2[i] = Convert.ToBoolean((num2 & num3) / num3);
                }
            }
            return array2;
        }
        protected virtual bool ValidateResponse(byte[] response, int fcode, int countRetries = 0)
        {
            if (response == null || response.Length < 8)
            {
                DebugRecord("RespondformatNotValid Throwed");
                if (NumberOfRetries <= countRetries)
                {
                    throw new ModbusException("Respond format Not Valid");
                }
                return false;
            }
            int errormsg = 128 + fcode;
            if ((response[7] == errormsg) & (response[8] == 1))
            {
                DebugRecord("FunctionCodeNotSupportedException Throwed");
                throw new FunctionCodeNotSupportedException("Function code not supported by master");
            }

            if ((response[7] == errormsg) & (response[8] == 2))
            {
                DebugRecord("StartingAddressInvalidException Throwed");
                throw new StartingAddressInvalidException("Starting address invalid or starting address + quantity invalid");
            }

            if ((response[7] == errormsg) & (response[8] == 3))
            {
                DebugRecord("QuantityInvalidException Throwed");
                throw new QuantityInvalidException("quantity invalid");
            }

            if ((response[7] == errormsg) & (response[8] == 4))
            {
                DebugRecord("ModbusException Throwed");
                throw new ModbusException("error reading");
            }
            return true;
        }

        public int[] ReadsafeRegister(int slaveAddress, int startingAddress, int quantity)
        {
            EnterslaveMode(slaveAddress, 0);
            try
            {
                return ReadHoldingRegister(startingAddress, quantity);
            }
            catch (Exception ex)
            {
                DebugRecord("ExceptionReadHold:" + ex.Message);
                throw;
            }
            finally
            {
                ExitslaveMode(slaveAddress, 1);
            }
        }

        private void ExitslaveMode(int startingAddress, int value)
        {
            int localCount = 0;
            lock (_slavelocker)
            {
                localCount = _safeCount--;
                if (localCount <= 0)
                {
                    _safeCount = 0;
                }
            }
            if (localCount <= 0)
            {
                Task.Delay(20).Wait();
                try
                {
                    WriteSingleRegister(startingAddress, value, NumberOfRetries);
                }
                catch (Exception ex)
                {
                    DebugRecord("ExceptionExitslave:" + ex.Message);
                    throw;
                }
            }
        }
        private void EnterslaveMode(int startingAddress, int value)
        {
            lock (_slavelocker)
            {
                var elapsedMs = (DateTime.Now - _lastModbusOpTime).TotalMilliseconds;
                if (elapsedMs < 20) // 3.5字符时间，通常至少3.5ms，20ms更安全
                {
                    Task.Delay(20 - (int)elapsedMs).Wait();
                }
                if (_safeCount++ > 0)
                {
                    _lastModbusOpTime = DateTime.Now;
                    return;
                }
                try
                {
                    //防止递归调用死锁
                    WriteSingleRegister(startingAddress, value, NumberOfRetries);                 
                    Task.Delay(20).Wait();
                    _lastModbusOpTime = DateTime.Now;
                }
                catch (Exception ex)
                {
                    _safeCount--;
                    DebugRecord("ExceptionExterslave:" + ex.Message);
                    throw;
                }
            }
        }
        public virtual void Dispose()
        {

        }
        ~JModbusBaseClient()
        {
            Dispose();
        }
    }
}
