using JModbusClient.Log;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace JModbusClient.Communicate
{
    public class MotorTcpClient : IDisposable
    {
        public delegate void ReceiveDataChangedHandler(object sender);
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private string _ip = "127.0.0.1";
        private int _port = 502;
        private int _connectTimeout = 3000;
        private int _writeTimeout = 1000;
        private int _readTimeout = 1000;
        private bool _connected = false;
        private Task _receiveTask;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private SafeBusDataBuffer<byte[]> _safeDataBuffer = new SafeBusDataBuffer<byte[]>();
        public event ReceiveDataChangedHandler ReceiveDataChanged;
        private readonly object _writeLock = new object();
        private readonly SemaphoreSlim _writeSemaphore = new SemaphoreSlim(1, 1);
        public int DataBufferCount => _safeDataBuffer.BufferCount;
        public bool DataAvailable => _safeDataBuffer.BufferCount > 0;

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }
        public string IP
        {
            get { return _ip; }
            set { _ip = value; }
        }
        public int ConnectTimeout
        {
            get { return _connectTimeout; }
            set { _connectTimeout = value; }
        }
        public bool Connected
        {
            get
            {
                return _tcpClient?.Client?.Connected == true && _connected;
            }
        }

        public int WriteTimeout
        {
            get
            { return _writeTimeout; }
            set
            { _writeTimeout = value; }
        }

        public int ReadTimeout
        {
            get
            {
                return _readTimeout;
            }
            set
            {
                _readTimeout = value;
            }
        }
        public MotorTcpClient()
        {

        }
        public MotorTcpClient(string ip, int port) : this()
        {
            this._ip = ip;
            this._port = port;
        }

        public void Connect()
        {
            if (IsConnected())
                return;
            _tcpClient = new TcpClient();
            _tcpClient.ReceiveBufferSize = 8192;
            IAsyncResult asyncResult = _tcpClient.BeginConnect(_ip, _port, null, null);
            try
            {
                if (!asyncResult.AsyncWaitHandle.WaitOne(_connectTimeout))
                {
                    _tcpClient.Close();
                    throw new TimeoutException("connection timed out");
                }
                _tcpClient.EndConnect(asyncResult);
            }
            finally
            {
                if (asyncResult != null && asyncResult.AsyncWaitHandle != null)
                    asyncResult.AsyncWaitHandle.Close();
            }
            _stream = _tcpClient.GetStream();
            _stream.ReadTimeout = _readTimeout;
            _stream.WriteTimeout = _writeTimeout;
            _connected = true;
            ThreadPool.QueueUserWorkItem(_ => PollingFallback(_cts.Token));
        }
        public async Task ConnectAsync(Action action = null)
        {
            if (IsConnected())
                return;
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            _tcpClient = new TcpClient();
            var timeoutTask = Task.Delay(_connectTimeout);
            var connectTask =  _tcpClient.ConnectAsync(_ip, _port);
            var completedTask = await Task.WhenAny(connectTask, timeoutTask);
            if (completedTask == timeoutTask)
            {
                _tcpClient.Dispose();
                throw new ConnectionException($"{_ip}:{_port} TCPConnection timed out.");
            }
            if (!_tcpClient.Connected) // 二次验证
                throw new ConnectionException("Socket not connected.");
            _stream = _tcpClient.GetStream();
            _stream.ReadTimeout = _readTimeout;
            _stream.WriteTimeout = _writeTimeout;
            _connected = true;
            action?.Invoke();
            _receiveTask = AsyncReceiveLoop(_cts.Token);
        }

        private bool IsConnected()
        {
            if (_tcpClient?.Client == null)
                return false;
            // 检查可读状态(检测连接关闭)
            // 检查错误状态
            try
            {
                bool isconn = !(_tcpClient.Client.Poll(0, SelectMode.SelectRead) && _tcpClient.Client.Available == 0);
                return isconn;
            }
            catch
            {
                return false;
            }
        }

        public void Connect(string ip, int port)
        {
            this._ip = ip;
            this._port = port;
            Connect();
        }

        public void Disconnect()
        {
            _cts?.Cancel();
            if (_tcpClient != null)
            {
                _stream.Close();
                _tcpClient.Close();
            }
            _connected = false;
        }
        internal ReadResult<byte[]> Read(int timeoutMs = 1000)
        {
            return _safeDataBuffer.Read(timeoutMs);
        }
        internal async virtual Task<ReadResult<byte[]>> ReadAsync(int timeoutMs = 1000, CancellationToken cancellationToken = default)
        {
            return await _safeDataBuffer.ReadAsync(timeoutMs, cancellationToken);
        }
        public void Write(byte[] bytes, int offset, int bufLen)
        {
            if (bytes == null || bufLen <= 0 || _stream == null)
                throw new ArgumentException("Invalid Write buffer");
            lock (_writeLock)
            {
                if (!_connected)
                {
                    throw new InvalidOperationException("Tcp socket Connection is not active");
                }
                try
                {
                    _stream.Write(bytes, offset, bufLen);
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
            if (bytes == null || bufLen <= 0 || _stream == null)
                throw new ArgumentException("Invalid Write buffer");

            await _writeSemaphore.WaitAsync(cancellationToken);
            try
            {
                if (!_connected)
                {
                    throw new InvalidOperationException("Tcp socket Connection is not active");
                }
                await _stream.WriteAsync(bytes, offset, bufLen, cancellationToken);
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
        public void DiscardInBuffer()
        {
            _safeDataBuffer.Clear();
        }
        public async Task DiscardInBufferAsync()
        {
            await _safeDataBuffer.ClearAsync();
        }
        private async Task AsyncReceiveLoop(CancellationToken ct)
        {
            byte[] buffer = new byte[4096];
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, ct);
                    if (bytesRead != 0)
                    {
                        byte[] validData = new byte[bytesRead];
                        Array.Copy(buffer, 0, validData, 0, bytesRead);
                        _safeDataBuffer.TryWrite(validData);
                        ReceiveDataChanged?.Invoke(this);
                    }
                }
            }
            catch (OperationCanceledException) { /* 正常退出 */ }
            catch (IOException ex) when (ex.InnerException is SocketException se)
            {
                DebugRecord($"网络错误: {se.SocketErrorCode}");
                Disconnect(); // 强制断开
            }
            catch (Exception ex)
            {
                DebugRecord($"系统错误: {ex.GetType().Name} - {ex.Message}");
                Disconnect();
            }
        }

        private void PollingFallback(CancellationToken ct)
        {
            byte[] buffer = new byte[4096];
            try
            {

                while (!ct.IsCancellationRequested)
                {
                    int delay = _safeDataBuffer?.BufferCount > 0 ? 1 : 10;
                    if (_stream.DataAvailable)
                    {
                        int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead != 0)
                        {
                            byte[] validData = new byte[bytesRead];
                            Array.Copy(buffer, 0, validData, 0, bytesRead);
                            _safeDataBuffer.TryWrite(validData);
                            ReceiveDataChanged?.Invoke(this);
                        }
                    }
                    Thread.Sleep(delay);
                }
            }
            catch (OperationCanceledException) { /* 正常退出 */ }
            catch (IOException ex) when (ex.InnerException is SocketException se)
            {
                DebugRecord($"网络错误: {se.SocketErrorCode}");
                Disconnect(); // 强制断开
            }
            catch (Exception ex)
            {
                DebugRecord($"系统错误: {ex.GetType().Name} - {ex.Message}");
                Disconnect();
            }
        }

        private void DebugRecord(string msg)
        {
            StoreLog.Instance.Store(msg);
        }

        public void Dispose()
        {
            _cts?.Cancel();
            if (_tcpClient != null)
            {
                if (_stream != null)
                {
                    _stream.Close();
                }
                _tcpClient.Close();
            }
            _tcpClient?.Dispose();
            _safeDataBuffer?.Dispose();
        }
    }
}
