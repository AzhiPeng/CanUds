using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace JModbusClient
{
    internal class SafeBusDataBuffer<T>:IDisposable
    {
        private const int DEFAULT_BUFFER_SIZE = 1000;
        private readonly ConcurrentQueue<T> _dataQueue;
        private readonly SemaphoreSlim _writeLock;
        private readonly SemaphoreSlim _dataAvailableSemaphore;
        private readonly int _maxBufferSize;
        private bool _disposed;

        public int BufferCount => _dataQueue.Count;
        public bool DataAvailable => !_dataQueue.IsEmpty;

        public SafeBusDataBuffer(int maxBufferSize = DEFAULT_BUFFER_SIZE)
        {
            _maxBufferSize = maxBufferSize;
            _dataQueue = new ConcurrentQueue<T>();
            _writeLock = new SemaphoreSlim(1, 1);
            _dataAvailableSemaphore = new SemaphoreSlim(0);
        }
        public async Task<bool> WriteAsync(T data, int timeoutMs = 1000, CancellationToken cancellation = default)
        {
            if (data == null) return false;

            try
            {
                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellation))
                {
                    cts.CancelAfter(TimeSpan.FromMilliseconds(timeoutMs));

                    if (!await _writeLock.WaitAsync(timeoutMs, cts.Token).ConfigureAwait(false))
                    {
                        return false;
                    }
                }
                try
                {
                    while (_dataQueue.Count >= _maxBufferSize)
                    {
                        _dataQueue.TryDequeue(out _);
                    }

                    _dataQueue.Enqueue(data);
                    _dataAvailableSemaphore.Release();
                    return true;
                }
                finally
                {
                    _writeLock.Release();
                }
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch
            {
                throw;
            }
        }
        public bool TryWrite(T data, int timeoutMs = 1000)
        {
            return WriteAsync(data, timeoutMs).GetAwaiter().GetResult();
        }

        public ReadResult<T> Read(int timeoutMs = 1000)
        {
            return ReadAsync(timeoutMs).GetAwaiter().GetResult();
        }      
        public async Task<ReadResult<T>> ReadAsync(int timeoutMs = 1000, CancellationToken cancellation = default)
        {
            try
            {
                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellation))
                {
                    cts.CancelAfter(TimeSpan.FromMilliseconds(timeoutMs));

                    if (!await _dataAvailableSemaphore.WaitAsync(timeoutMs, cts.Token).ConfigureAwait(false))
                    {
                        return new ReadResult<T>(ReadStatus.Timeout, default(T), $"读取操作超时 {timeoutMs}ms");
                    }
                    T result;
                    if (_dataQueue.TryDequeue(out result))
                    {
                        return new ReadResult<T>(ReadStatus.Success, result);
                    }
                    return new ReadResult<T>(ReadStatus.Null, default(T));
                }
            }
            catch (OperationCanceledException ex)
            {
                return new ReadResult<T>(ReadStatus.Timeout, default(T), "读取操作超时" + ex.Message);
            }
            catch (Exception ex)
            {
                return new ReadResult<T>(ReadStatus.Error, default(T),ex.Message);
            }
        }
        public async Task ClearAsync()
        {
            await _writeLock.WaitAsync().ConfigureAwait(false);
            try
            {
                while (_dataQueue.TryDequeue(out _)) { }

                // 清空信号量
                while (_dataAvailableSemaphore.CurrentCount > 0)
                {
                    _dataAvailableSemaphore.Wait(0);
                }
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public void Clear()
        {
            ClearAsync().GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _writeLock.Dispose();
                    _dataAvailableSemaphore.Dispose();
                    while (_dataQueue.TryDequeue(out _)) { }
                }
                _disposed = true;
            }
        }
    }
}
