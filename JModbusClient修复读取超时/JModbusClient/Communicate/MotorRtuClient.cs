using JModbusClient.Log;
using System;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace JModbusClient.Communicate
{
    public class MotorRtuClient:IDisposable
    {
        public delegate void ReceiveDataChangedHandler(object sender);
        private SerialPort _serialport;
        private int _baudRate = 9600;
        private Parity _parity = Parity.Even;
        private StopBits _stopBits = StopBits.One;
        private bool _connected = false;
        private int _writeTimeout = 1000;
        private int _readTimeout = 1000;
        private int _countRetries = 0;
        private bool _dataReceived = false;
        private bool _receiveActive = false;
        private byte[] _readBuffer = new byte[256];
        private SafeBusDataBuffer<byte[]> _safeDataBuffer = new SafeBusDataBuffer<byte[]>();
        public event ReceiveDataChangedHandler ReceiveDataChanged;
        private readonly object _writeLock = new object();
        private readonly SemaphoreSlim _writeSemaphore = new SemaphoreSlim(1, 1);
        public bool Connected
        {
            get { return _connected; }
        }
        public int ReadTimeout
        {
            get { return _readTimeout; }
            set { _readTimeout = value; }
        }

        public int WriteTimeout
        {
            get { return _writeTimeout; }
            set { _writeTimeout = value; }
        }

        public int BaudRate
        {
            get
            {
                return _baudRate;
            }
            set
            {
                _baudRate = value;
            }
        }
        public Parity Parity
        {
            get
            {
                if (_serialport != null)
                {
                    return _parity;
                }

                return Parity.Even;
            }
            set
            {
                if (_serialport != null)
                {
                    _parity = value;
                }
            }
        }

        public StopBits StopBits
        {
            get
            {
                if (_serialport != null)
                {
                    return _stopBits;
                }

                return StopBits.One;
            }
            set
            {
                if (_serialport != null)
                {
                    _stopBits = value;
                }
            }
        }
        public void DiscardInBuffer()
        {
            _safeDataBuffer.Clear();
        }

        public MotorRtuClient(string serialName)
        {
            _serialport = new SerialPort();
            _serialport.PortName = serialName;
            _serialport.BaudRate = _baudRate;
            _serialport.Parity = _parity;
            _serialport.StopBits = _stopBits;
            _serialport.WriteTimeout = _writeTimeout;
            _serialport.ReadTimeout = _readTimeout;
            _serialport.DataReceived += DataReceivedHandler;
        }
        public string SerialPort
        {
            get
            {
                return _serialport.PortName;
            }
            set
            {
                if (value == null)
                {
                    _serialport = null;
                    return;
                }

                if (_serialport != null)
                {
                    _serialport.Close();
                }

                _serialport = new SerialPort();
                _serialport.PortName = value;
                _serialport.BaudRate = _baudRate;
                _serialport.Parity = _parity;
                _serialport.StopBits = _stopBits;
                _serialport.WriteTimeout = _writeTimeout;
                _serialport.ReadTimeout = _readTimeout;
                _serialport.DataReceived += DataReceivedHandler;
            }
        }

        public void Connect()
        {
            if (_serialport == null)
            {
                throw new Exception("Serial port not set");
            }
            if (_connected)
            {
                return;
            }
            try
            {
                _serialport.BaudRate = _baudRate;
                _serialport.Parity = _parity;
                _serialport.StopBits = _stopBits;
                _serialport.WriteTimeout = _writeTimeout;
                _serialport.ReadTimeout = _readTimeout;
                _serialport.Open();
                _connected = true;
            }
            catch (Exception ex)
            {
                DebugRecord("Error opening serial port: " + ex.Message);
                throw new Exception("Error opening serial port: " + ex.Message);
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            _receiveActive = true;
            SerialPort serialPort = (SerialPort)sender;
            int num = 0;
            try
            {
                num = serialPort.BytesToRead;
                if (num > 0)
                {
                    byte[] array = new byte[num];
                    serialPort.Read(array, 0, num);
                    _safeDataBuffer.TryWrite(array);
                }
            }
            catch (Exception ex)
            {
                DebugRecord($"DataReceived Error: {ex.Message}");
                throw;
            }         
            _receiveActive = false;
            ReceiveDataChanged?.Invoke(this);
        }

        public void Write(byte[] bytes, int offset, int bufLen)
        {
            if (bytes == null || bufLen <= 0 || _serialport == null)
                throw new ArgumentException("Invalid Write buffer");
            lock (_writeLock)
            {
                if (!_connected)
                {
                    throw new InvalidOperationException("Serial Connection is not active");
                }
                try
                {
                    _serialport.Write(bytes, offset, bufLen);
                }
                catch (Exception ex)
                {
                    DebugRecord($"系统错误: {ex.GetType().Name} - {ex.Message}");
                    Disconnect();
                    throw;
                }
            }
        }
        public async Task WriteAsync(byte[] bytes, int offset, int bufLen, CancellationToken cancellationToken = default)
        {
            if (bytes == null || bufLen <= 0 || _serialport == null)
                throw new ArgumentException("Invalid Write buffer");
            await _writeSemaphore.WaitAsync(cancellationToken);
            try
            {
                if (!_connected)
                {
                    throw new InvalidOperationException("Serial Connection is not active");
                }
                await _serialport.BaseStream.WriteAsync(bytes, offset, bufLen, cancellationToken);
                await _serialport.BaseStream.FlushAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                DebugRecord($"系统错误: {ex.GetType().Name} - {ex.Message}");
                Disconnect();
                throw;
            }
            finally
            {
                _writeSemaphore.Release();
            }
        }
        internal ReadResult<byte[]> Read(int timeoutMs = 1000)
        {
            return _safeDataBuffer.Read(timeoutMs);
        }
        internal async virtual Task<ReadResult<byte[]>> ReadAsync(int timeoutMs = 1000, CancellationToken cancellationToken = default)
        {
            return await _safeDataBuffer.ReadAsync(timeoutMs, cancellationToken);
        }
        private void DebugRecord(string msg)
        {
            StoreLog.Instance.Store(msg);
        }
        public void Disconnect()
        {
            if (_serialport != null)
            {
                if (_serialport.IsOpen & !_receiveActive)
                {
                    _serialport.Close();
                    _connected = false;
                }
            }
        }

        public void Dispose()
        {
            if (_serialport != null)
                _serialport.DataReceived -= DataReceivedHandler;
            _serialport?.Close();
            _serialport?.Dispose();
            _safeDataBuffer?.Dispose();
        }
    }
}
