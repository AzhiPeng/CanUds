using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO.Ports;
using System.IO.Pipes;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO.MemoryMappedFiles;
using System.Diagnostics;

namespace Go
{
    internal class TypeReflection
    {
        public static readonly Assembly _systemAss = Assembly.LoadFrom(RuntimeEnvironment.GetRuntimeDirectory() + "System.dll");
        public static readonly Assembly _mscorlibAss = Assembly.LoadFrom(RuntimeEnvironment.GetRuntimeDirectory() + "mscorlib.dll");

        public static MethodInfo GetMethod(Type type, string name, Type[] parmsType = null)
        {
            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            for (int i = 0; i < methods.Length; i++)
            {
                if (methods[i].Name == name)
                {
                    if (null == parmsType)
                    {
                        return methods[i];
                    }
                    else
                    {
                        ParameterInfo[] parameters = methods[i].GetParameters();
                        if (parameters.Length == parmsType.Length)
                        {
                            int j = 0;
                            for (; j < parmsType.Length && parameters[j].ParameterType == parmsType[j]; j++) { }
                            if (j == parmsType.Length)
                            {
                                return methods[i];
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static FieldInfo GetField(Type type, string name)
        {
            FieldInfo[] members = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].Name == name)
                {
                    return members[i];
                }
            }
            return null;
        }
    }

    public struct socket_result
    {
        public bool success;
        public int size;
        public System.Exception ec;

        public socket_result(bool success = false, int size = 0, System.Exception ec = null)
        {
            this.success = success;
            this.size = size;
            this.ec = ec;
        }

        public string message
        {
            get
            {
                return null != ec ? ec.Message : null;
            }
        }

        static public implicit operator bool(socket_result src)
        {
            return src.success;
        }

        static public implicit operator int(socket_result src)
        {
            return src.size;
        }
    }

    struct WSABuffer
    {
        public int Length;
        public IntPtr Pointer;
    }

    struct socket_same_handler
    {
        public WSABuffer buffer;
        public object pinnedObj;
        public Action<socket_result> callback;
        public AsyncCallback handler;
    }

    struct socket_handler
    {
        public int currTotal;
        public ArraySegment<byte> buff;
        public Action<socket_result> callback;
        public Action<socket_result> handler;
    }

    struct socket_ptr_handler
    {
        public int currTotal;
        public IntPtr pointer;
        public int size;
        public object pinnedObj;
        public Action<socket_result> handler;
        public Action<socket_result> callback;
    }

    struct accept_handler
    {
        public socket_tcp socket;
        public AsyncCallback handler;
        public Action<socket_result> callback;
    }

    struct accept_lost_handler
    {
        public socket_tcp socket;
        public Action<socket_result> handler;
    }

    public abstract class socket
    {
        [DllImport("kernel32.dll")]
        static extern int WriteFile(SafeHandle handle, IntPtr bytes, int numBytesToWrite, out int numBytesWritten, IntPtr mustBeZero);
        [DllImport("kernel32.dll")]
        static extern int ReadFile(SafeHandle handle, IntPtr bytes, int numBytesToRead, out int numBytesRead, IntPtr mustBeZero);
        [DllImport("kernel32.dll")]
        static extern int CancelIoEx(SafeHandle handle, IntPtr mustBeZero);

        static readonly Type _memoryAccessorType = TypeReflection._mscorlibAss.GetType("System.IO.UnmanagedMemoryAccessor");
        static readonly Type _memoryStreamType = TypeReflection._mscorlibAss.GetType("System.IO.UnmanagedMemoryStream");
        static readonly Type _safeHandleType = TypeReflection._mscorlibAss.GetType("System.Runtime.InteropServices.SafeHandle");
        static readonly FieldInfo _memoryAccessorSafeBuffer = TypeReflection.GetField(_memoryAccessorType, "_buffer");
        static readonly FieldInfo _memoryStreamSafeBuffer = TypeReflection.GetField(_memoryStreamType, "_buffer");
        static readonly FieldInfo _safeBufferHandle = TypeReflection.GetField(_safeHandleType, "handle");

        static public tuple<bool, int> sync_write(SafeHandle handle, IntPtr pointer, long offset, int size)
        {
            int num = 0;
            return new tuple<bool, int>(1 == WriteFile(handle, new IntPtr(pointer.ToInt64() + offset), size, out num, IntPtr.Zero), num);
        }

        static public tuple<bool, int> sync_read(SafeHandle handle, IntPtr pointer, long offset, int size)
        {
            int num = 0;
            return new tuple<bool, int>(1 == ReadFile(handle, new IntPtr(pointer.ToInt64() + offset), size, out num, IntPtr.Zero), num);
        }

        static public tuple<bool, int> sync_write(SafeHandle handle, byte[] bytes, int offset, int size)
        {
            return sync_write(handle, Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0), offset, size);
        }

        static public tuple<bool, int> sync_read(SafeHandle handle, byte[] bytes, int offset, int size)
        {
            return sync_read(handle, Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0), offset, size);
        }

        static public tuple<bool, int> sync_write(SafeHandle handle, byte[] bytes)
        {
            return sync_write(handle, bytes, 0, bytes.Length);
        }

        static public tuple<bool, int> sync_read(SafeHandle handle, byte[] bytes)
        {
            return sync_read(handle, bytes, 0, bytes.Length);
        }

        static public bool cancel_io(SafeHandle handle)
        {
            return 1 == CancelIoEx(handle, IntPtr.Zero);
        }

        static public IntPtr get_mmview_ptr(MemoryMappedViewAccessor mmv)
        {
            return (IntPtr)_safeBufferHandle.GetValue(_memoryAccessorSafeBuffer.GetValue(mmv));
        }

        static public IntPtr get_mmview_ptr(MemoryMappedViewStream mmv)
        {
            return (IntPtr)_safeBufferHandle.GetValue(_memoryStreamSafeBuffer.GetValue(mmv));
        }

        socket_handler _readHandler;
        socket_handler _writeHandler;

        protected socket()
        {
            _readHandler.handler = delegate (socket_result tempRes)
            {
                if (tempRes.success)
                {
                    _readHandler.currTotal += tempRes.size;
                    _readHandler.buff = new ArraySegment<byte>(_readHandler.buff.Array, _readHandler.buff.Offset + tempRes.size, _readHandler.buff.Count - tempRes.size);
                    if (0 == _readHandler.buff.Count)
                    {
                        Action<socket_result> callback = _readHandler.callback;
                        _readHandler.buff = default(ArraySegment<byte>);
                        _readHandler.callback = null;
                        functional.catch_invoke(callback, new socket_result(true, _readHandler.currTotal));
                    }
                    else
                    {
                        async_read_same(_readHandler.buff, _readHandler.handler);
                    }
                }
                else
                {
                    Action<socket_result> callback = _readHandler.callback;
                    _readHandler.buff = default(ArraySegment<byte>);
                    _readHandler.callback = null;
                    functional.catch_invoke(callback, new socket_result(false, _readHandler.currTotal, tempRes.ec));
                }
            };
            _writeHandler.handler = delegate (socket_result tempRes)
            {
                if (tempRes.success)
                {
                    _writeHandler.currTotal += tempRes.size;
                    _writeHandler.buff = new ArraySegment<byte>(_writeHandler.buff.Array, _writeHandler.buff.Offset + tempRes.size, _writeHandler.buff.Count - tempRes.size);
                    if (0 == _writeHandler.buff.Count)
                    {
                        Action<socket_result> callback = _writeHandler.callback;
                        _writeHandler.buff = default(ArraySegment<byte>);
                        _writeHandler.callback = null;
                        functional.catch_invoke(callback, new socket_result(true, _writeHandler.currTotal));
                    }
                    else
                    {
                        async_write_same(_writeHandler.buff, _writeHandler.handler);
                    }
                }
                else
                {
                    Action<socket_result> callback = _writeHandler.callback;
                    _writeHandler.buff = default(ArraySegment<byte>);
                    _writeHandler.callback = null;
                    functional.catch_invoke(callback, new socket_result(false, _writeHandler.currTotal, tempRes.ec));
                }
            };
        }

        public abstract void async_read_same(ArraySegment<byte> buff, Action<socket_result> callback);
        public abstract void async_write_same(ArraySegment<byte> buff, Action<socket_result> callback);
        public abstract void close();

        public void async_read_same(byte[] buff, Action<socket_result> callback)
        {
            async_read_same(new ArraySegment<byte>(buff), callback);
        }

        public void async_write_same(byte[] buff, Action<socket_result> callback)
        {
            async_write_same(new ArraySegment<byte>(buff), callback);
        }

        void _async_reads(int currTotal, int currIndex, IList<ArraySegment<byte>> buff, Action<socket_result> callback)
        {
            if (buff.Count == currIndex)
            {
                functional.catch_invoke(callback, new socket_result(true, currTotal));
            }
            else
            {
                async_read(buff[currIndex], delegate (socket_result tempRes)
                {
                    if (tempRes.success)
                    {
                        _async_reads(currTotal + tempRes.size, currIndex + 1, buff, callback);
                    }
                    else
                    {
                        functional.catch_invoke(callback, new socket_result(false, currTotal, tempRes.ec));
                    }
                });
            }
        }

        void _async_reads(int currTotal, int currIndex, IList<byte[]> buff, Action<socket_result> callback)
        {
            if (buff.Count == currIndex)
            {
                functional.catch_invoke(callback, new socket_result(true, currTotal));
            }
            else
            {
                async_read(buff[currIndex], delegate (socket_result tempRes)
                {
                    if (tempRes.success)
                    {
                        _async_reads(currTotal + tempRes.size, currIndex + 1, buff, callback);
                    }
                    else
                    {
                        functional.catch_invoke(callback, new socket_result(false, currTotal, tempRes.ec));
                    }
                });
            }
        }

        public void async_read(ArraySegment<byte> buff, Action<socket_result> callback)
        {
            _readHandler.currTotal = 0;
            _readHandler.buff = buff;
            _readHandler.callback = callback;
            async_read_same(_readHandler.buff, _readHandler.handler);
        }

        public void async_read(byte[] buff, Action<socket_result> callback)
        {
            async_read(new ArraySegment<byte>(buff), callback);
        }

        void _async_writes(int currTotal, int currIndex, IList<ArraySegment<byte>> buff, Action<socket_result> callback)
        {
            if (buff.Count == currIndex)
            {
                functional.catch_invoke(callback, new socket_result(true, currTotal));
            }
            else
            {
                async_write(buff[currIndex], delegate (socket_result tempRes)
                {
                    if (tempRes.success)
                    {
                        _async_writes(currTotal + tempRes.size, currIndex + 1, buff, callback);
                    }
                    else
                    {
                        functional.catch_invoke(callback, new socket_result(false, currTotal, tempRes.ec));
                    }
                });
            }
        }

        void _async_writes(int currTotal, int currIndex, IList<byte[]> buff, Action<socket_result> callback)
        {
            if (buff.Count == currIndex)
            {
                functional.catch_invoke(callback, new socket_result(true, currTotal));
            }
            else
            {
                async_write(buff[currIndex], delegate (socket_result tempRes)
                {
                    if (tempRes.success)
                    {
                        _async_writes(currTotal + tempRes.size, currIndex + 1, buff, callback);
                    }
                    else
                    {
                        functional.catch_invoke(callback, new socket_result(false, currTotal, tempRes.ec));
                    }
                });
            }
        }

        public void async_write(ArraySegment<byte> buff, Action<socket_result> callback)
        {
            _writeHandler.currTotal = 0;
            _writeHandler.buff = buff;
            _writeHandler.callback = callback;
            async_write_same(_writeHandler.buff, _writeHandler.handler);
        }

        public void async_write(byte[] buff, Action<socket_result> callback)
        {
            async_write(new ArraySegment<byte>(buff), callback);
        }

        public ValueTask<socket_result> read_same(ArraySegment<byte> buff)
        {
            return generator.async_call((Action<socket_result> callback) => async_read_same(buff, callback));
        }

        public ValueTask<socket_result> read_same(byte[] buff)
        {
            return generator.async_call((Action<socket_result> callback) => async_read_same(buff, callback));
        }

        public ValueTask<socket_result> read(ArraySegment<byte> buff)
        {
            return generator.async_call((Action<socket_result> callback) => async_read(buff, callback));
        }

        public ValueTask<socket_result> read(byte[] buff)
        {
            return generator.async_call((Action<socket_result> callback) => async_read(buff, callback));
        }

        public ValueTask<socket_result> reads(IList<byte[]> buff)
        {
            return generator.async_call((Action<socket_result> callback) => _async_reads(0, 0, buff, callback));
        }

        public ValueTask<socket_result> reads(IList<ArraySegment<byte>> buff)
        {
            return generator.async_call((Action<socket_result> callback) => _async_reads(0, 0, buff, callback));
        }

        public ValueTask<socket_result> reads(params byte[][] buff)
        {
            return generator.async_call((Action<socket_result> callback) => _async_reads(0, 0, buff, callback));
        }

        public ValueTask<socket_result> reads(params ArraySegment<byte>[] buff)
        {
            return generator.async_call((Action<socket_result> callback) => _async_reads(0, 0, buff, callback));
        }

        public ValueTask<socket_result> write_same(ArraySegment<byte> buff)
        {
            return generator.async_call((Action<socket_result> callback) => async_write_same(buff, callback));
        }

        public ValueTask<socket_result> write_same(byte[] buff)
        {
            return generator.async_call((Action<socket_result> callback) => async_write_same(buff, callback));
        }

        public ValueTask<socket_result> write(ArraySegment<byte> buff)
        {
            return generator.async_call((Action<socket_result> callback) => async_write(buff, callback));
        }

        public ValueTask<socket_result> write(byte[] buff)
        {
            return generator.async_call((Action<socket_result> callback) => async_write(buff, callback));
        }

        public ValueTask<socket_result> writes(IList<byte[]> buff)
        {
            return generator.async_call((Action<socket_result> callback) => _async_writes(0, 0, buff, callback));
        }

        public ValueTask<socket_result> writes(IList<ArraySegment<byte>> buff)
        {
            return generator.async_call((Action<socket_result> callback) => _async_writes(0, 0, buff, callback));
        }

        public ValueTask<socket_result> writes(params byte[][] buff)
        {
            return generator.async_call((Action<socket_result> callback) => _async_writes(0, 0, buff, callback));
        }

        public ValueTask<socket_result> writes(params ArraySegment<byte>[] buff)
        {
            return generator.async_call((Action<socket_result> callback) => _async_writes(0, 0, buff, callback));
        }

        public Task unsafe_read_same(async_result_wrap<socket_result> res, ArraySegment<byte> buff)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => async_read_same(buff, callback));
        }

        public Task unsafe_read_same(async_result_wrap<socket_result> res, byte[] buff)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => async_read_same(buff, callback));
        }

        public Task unsafe_read(async_result_wrap<socket_result> res, ArraySegment<byte> buff)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => async_read(buff, callback));
        }

        public Task unsafe_read(async_result_wrap<socket_result> res, byte[] buff)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => async_read(buff, callback));
        }

        public Task unsafe_reads(async_result_wrap<socket_result> res, IList<byte[]> buff)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => _async_reads(0, 0, buff, callback));
        }

        public Task unsafe_reads(async_result_wrap<socket_result> res, IList<ArraySegment<byte>> buff)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => _async_reads(0, 0, buff, callback));
        }

        public Task unsafe_reads(async_result_wrap<socket_result> res, params byte[][] buff)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => _async_reads(0, 0, buff, callback));
        }

        public Task unsafe_reads(async_result_wrap<socket_result> res, params ArraySegment<byte>[] buff)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => _async_reads(0, 0, buff, callback));
        }

        public Task unsafe_write_same(async_result_wrap<socket_result> res, ArraySegment<byte> buff)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => async_write_same(buff, callback));
        }

        public Task unsafe_write_same(async_result_wrap<socket_result> res, byte[] buff)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => async_write_same(buff, callback));
        }

        public Task unsafe_write(async_result_wrap<socket_result> res, ArraySegment<byte> buff)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => async_write(buff, callback));
        }

        public Task unsafe_write(async_result_wrap<socket_result> res, byte[] buff)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => async_write(buff, callback));
        }

        public Task unsafe_writes(async_result_wrap<socket_result> res, IList<byte[]> buff)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => _async_writes(0, 0, buff, callback));
        }

        public Task unsafe_writes(async_result_wrap<socket_result> res, IList<ArraySegment<byte>> buff)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => _async_writes(0, 0, buff, callback));
        }

        public Task unsafe_writes(async_result_wrap<socket_result> res, params byte[][] buff)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => _async_writes(0, 0, buff, callback));
        }

        public Task unsafe_writes(async_result_wrap<socket_result> res, params ArraySegment<byte>[] buff)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => _async_writes(0, 0, buff, callback));
        }
    }

    public class socket_tcp : socket
    {
        class SocketAsyncIo
        {
            [DllImport("Ws2_32.dll", CharSet = CharSet.None, ExactSpelling = false, SetLastError = true)]
            static extern SocketError WSASend(IntPtr socketHandle, ref WSABuffer buffer, int bufferCount, out int bytesTransferred, SocketFlags socketFlags, SafeHandle overlapped, IntPtr completionRoutine);
            [DllImport("Ws2_32.dll", CharSet = CharSet.None, ExactSpelling = false, SetLastError = true)]
            static extern SocketError WSARecv(IntPtr socketHandle, ref WSABuffer buffer, int bufferCount, out int bytesTransferred, ref SocketFlags socketFlags, SafeHandle overlapped, IntPtr completionRoutine);
            [DllImport("Kernel32.dll", CharSet = CharSet.None, ExactSpelling = false, SetLastError = true)]
            static extern int SetFilePointerEx(SafeHandle fileHandle, long liDistanceToMove, out long lpNewFilePointer, int dwMoveMethod);
            [DllImport("Kernel32.dll", CharSet = CharSet.None, ExactSpelling = false, SetLastError = true)]
            static extern int GetFileSizeEx(SafeHandle fileHandle, out long lpFileSize);
            [DllImport("Mswsock.dll", CharSet = CharSet.None, ExactSpelling = false, SetLastError = true)]
            static extern int TransmitFile(IntPtr socketHandle, SafeHandle fileHandle, int nNumberOfBytesToWrite, int nNumberOfBytesPerSend, SafeHandle overlapped, IntPtr mustBeZero, int dwFlags);

            static readonly Type _overlappedType = TypeReflection._systemAss.GetType("System.Net.Sockets.OverlappedAsyncResult");
            static readonly Type _cacheSetType = TypeReflection._systemAss.GetType("System.Net.Sockets.Socket+CacheSet");
            static readonly Type _callbackClosureType = TypeReflection._systemAss.GetType("System.Net.CallbackClosure");
            static readonly Type _callbackClosureRefType = TypeReflection._systemAss.GetType("System.Net.CallbackClosure&");
            static readonly Type _overlappedCacheRefType = TypeReflection._systemAss.GetType("System.Net.Sockets.OverlappedCache&");
            static readonly Type _WSABufferType = TypeReflection._systemAss.GetType("System.Net.WSABuffer");
            static readonly MethodInfo _overlappedStartPostingAsyncOp = TypeReflection.GetMethod(_overlappedType, "StartPostingAsyncOp", new Type[] { typeof(bool) });
            static readonly MethodInfo _overlappedFinishPostingAsyncOp = TypeReflection.GetMethod(_overlappedType, "FinishPostingAsyncOp", new Type[] { _callbackClosureRefType });
            static readonly MethodInfo _overlappedSetupCache = TypeReflection.GetMethod(_overlappedType, "SetupCache", new Type[] { _overlappedCacheRefType });
            static readonly MethodInfo _overlappedExtractCache = TypeReflection.GetMethod(_overlappedType, "ExtractCache", new Type[] { _overlappedCacheRefType });
            static readonly MethodInfo _overlappedSetUnmanagedStructures = TypeReflection.GetMethod(_overlappedType, "SetUnmanagedStructures", new Type[] { typeof(object) });
            static readonly MethodInfo _overlappedOverlappedHandle = TypeReflection.GetMethod(_overlappedType, "get_OverlappedHandle");
            static readonly MethodInfo _overlappedCheckAsyncCallOverlappedResult = TypeReflection.GetMethod(_overlappedType, "CheckAsyncCallOverlappedResult", new Type[] { typeof(SocketError) });
            static readonly MethodInfo _overlappedInvokeCallback = TypeReflection.GetMethod(_overlappedType, "InvokeCallback", new Type[] { typeof(object) });
            static readonly MethodInfo _socketCaches = TypeReflection.GetMethod(typeof(Socket), "get_Caches");
            static readonly MethodInfo _sockektUpdateStatusAfterSocketError = TypeReflection.GetMethod(typeof(Socket), "UpdateStatusAfterSocketError", new Type[] { typeof(SocketError) });
            static readonly FieldInfo _overlappedmSingleBuffer = TypeReflection.GetField(_overlappedType, "m_SingleBuffer");
            static readonly FieldInfo _WSABufferLength = TypeReflection.GetField(_WSABufferType, "Length");
            static readonly FieldInfo _WSABufferPointer = TypeReflection.GetField(_WSABufferType, "Pointer");
            static readonly FieldInfo _cacheSendClosureCache = TypeReflection.GetField(_cacheSetType, "SendClosureCache");
            static readonly FieldInfo _cacheSendOverlappedCache = TypeReflection.GetField(_cacheSetType, "SendOverlappedCache");
            static readonly FieldInfo _cacheReceiveClosureCache = TypeReflection.GetField(_cacheSetType, "ReceiveClosureCache");
            static readonly FieldInfo _cacheReceiveOverlappedCache = TypeReflection.GetField(_cacheSetType, "ReceiveOverlappedCache");

            static readonly string _overlappedTypeName = _overlappedType.FullName;
            static private IAsyncResult MakeOverlappedAsyncResult(Socket sck, AsyncCallback callback)
            {
                return (IAsyncResult)TypeReflection._systemAss.CreateInstance(_overlappedTypeName, true,
                    BindingFlags.Instance | BindingFlags.NonPublic, null,
                    new object[] { sck, null, callback }, null, null);
            }

            static readonly object[] startPostingAsyncOpParam = new object[] { false };
            static private void StartPostingAsyncOp(IAsyncResult overlapped)
            {
                _overlappedStartPostingAsyncOp.Invoke(overlapped, startPostingAsyncOpParam);
            }

            static private void FinishPostingAsyncSendOp(IAsyncResult overlapped, object cacheSet)
            {
                object oldClosure = _cacheSendClosureCache.GetValue(cacheSet);
                object[] closure = new object[] { oldClosure };
                _overlappedFinishPostingAsyncOp.Invoke(overlapped, closure);
                if (oldClosure != closure[0])
                {
                    _cacheSendClosureCache.SetValue(cacheSet, closure[0]);
                }
            }

            static private void FinishPostingAsyncRecvOp(IAsyncResult overlapped, object cacheSet)
            {
                object oldClosure = _cacheReceiveClosureCache.GetValue(cacheSet);
                object[] closure = new object[] { oldClosure };
                _overlappedFinishPostingAsyncOp.Invoke(overlapped, closure);
                if (oldClosure != closure[0])
                {
                    _cacheReceiveClosureCache.SetValue(cacheSet, closure[0]);
                }
            }

            static private void SendSetupCache(IAsyncResult overlapped, object cacheSet, object pinnedObj)
            {
                object oldCache = _cacheSendOverlappedCache.GetValue(cacheSet);
                object[] overlappedCache = new object[] { oldCache };
                _overlappedSetupCache.Invoke(overlapped, overlappedCache);
                if (oldCache != overlappedCache[0])
                {
                    _cacheSendOverlappedCache.SetValue(cacheSet, overlappedCache[0]);
                }
                _overlappedSetUnmanagedStructures.Invoke(overlapped, new object[] { pinnedObj });
            }

            static private void RecvSetupCache(IAsyncResult overlapped, object cacheSet, object pinnedObj)
            {
                object oldCache = _cacheReceiveOverlappedCache.GetValue(cacheSet);
                object[] overlappedCache = new object[] { oldCache };
                _overlappedSetupCache.Invoke(overlapped, overlappedCache);
                if (oldCache != overlappedCache[0])
                {
                    _cacheReceiveOverlappedCache.SetValue(cacheSet, overlappedCache[0]);
                }
                _overlappedSetUnmanagedStructures.Invoke(overlapped, new object[] { pinnedObj });
            }

            static public SocketError Send(Socket socket, ref WSABuffer buffer, SocketFlags socketFlags, AsyncCallback callback, object pinnedObj)
            {
                IAsyncResult overlapped = MakeOverlappedAsyncResult(socket, callback);
                StartPostingAsyncOp(overlapped);
                object cacheSet = _socketCaches.Invoke(socket, null);
                SendSetupCache(overlapped, cacheSet, pinnedObj);
                int num = 0;
                SafeHandle overlappedHandle = (SafeHandle)_overlappedOverlappedHandle.Invoke(overlapped, null);
                SocketError lastWin32Error = WSASend(socket.Handle, ref buffer, 1, out num, socketFlags, overlappedHandle, IntPtr.Zero);
                if (SocketError.Success != lastWin32Error)
                {
                    lastWin32Error = (SocketError)Marshal.GetLastWin32Error();
                }
                lastWin32Error = (SocketError)_overlappedCheckAsyncCallOverlappedResult.Invoke(overlapped, new object[] { lastWin32Error });
                if (SocketError.Success != lastWin32Error)
                {
                    object oldCache = _cacheSendOverlappedCache.GetValue(cacheSet);
                    object[] overlappedCache = new object[] { oldCache };
                    _overlappedExtractCache.Invoke(overlapped, overlappedCache);
                    if (oldCache != overlappedCache[0])
                    {
                        _cacheSendOverlappedCache.SetValue(cacheSet, overlappedCache[0]);
                    }
                    _sockektUpdateStatusAfterSocketError.Invoke(socket, new object[] { lastWin32Error });
                }
                if (SocketError.Success == lastWin32Error || SocketError.IOPending == lastWin32Error)
                {
                    FinishPostingAsyncSendOp(overlapped, cacheSet);
                    return SocketError.Success;
                }
                return lastWin32Error;
            }

            static public SocketError Recv(Socket socket, ref WSABuffer buffer, SocketFlags socketFlags, AsyncCallback callback, object pinnedObj)
            {
                IAsyncResult overlapped = MakeOverlappedAsyncResult(socket, callback);
                StartPostingAsyncOp(overlapped);
                object cacheSet = _socketCaches.Invoke(socket, null);
                RecvSetupCache(overlapped, cacheSet, pinnedObj);
                int num = 0;
                SafeHandle overlappedHandle = (SafeHandle)_overlappedOverlappedHandle.Invoke(overlapped, null);
                SocketError lastWin32Error = WSARecv(socket.Handle, ref buffer, 1, out num, ref socketFlags, overlappedHandle, IntPtr.Zero);
                if (SocketError.Success != lastWin32Error)
                {
                    lastWin32Error = (SocketError)Marshal.GetLastWin32Error();
                }
                lastWin32Error = (SocketError)_overlappedCheckAsyncCallOverlappedResult.Invoke(overlapped, new object[] { lastWin32Error });
                if (SocketError.Success != lastWin32Error)
                {
                    object oldCache = _cacheReceiveOverlappedCache.GetValue(cacheSet);
                    object[] overlappedCache = new object[] { oldCache };
                    _overlappedExtractCache.Invoke(overlapped, overlappedCache);
                    if (oldCache != overlappedCache[0])
                    {
                        _cacheReceiveOverlappedCache.SetValue(cacheSet, overlappedCache[0]);
                    }
                    _sockektUpdateStatusAfterSocketError.Invoke(socket, new object[] { lastWin32Error });
                    _overlappedInvokeCallback.Invoke(overlapped, new object[] { new SocketException((int)lastWin32Error) });
                }
                if (SocketError.Success == lastWin32Error || SocketError.IOPending == lastWin32Error)
                {
                    FinishPostingAsyncRecvOp(overlapped, cacheSet);
                    return SocketError.Success;
                }
                return lastWin32Error;
            }

            static public SocketError SendFile(Socket socket, SafeHandle fileHandle, long offset, int size, AsyncCallback callback, object pinnedObj)
            {
                if (offset >= 0)
                {
                    long newOffset = 0;
                    if (0 == SetFilePointerEx(fileHandle, offset, out newOffset, 0) || offset != newOffset)
                    {
                        return SocketError.SocketError;
                    }
                }
                IAsyncResult overlapped = MakeOverlappedAsyncResult(socket, callback);
                StartPostingAsyncOp(overlapped);
                object cacheSet = _socketCaches.Invoke(socket, null);
                SendSetupCache(overlapped, cacheSet, pinnedObj);
                SafeHandle overlappedHandle = (SafeHandle)_overlappedOverlappedHandle.Invoke(overlapped, null);
                SocketError lastWin32Error = SocketError.Success;
                if (0 == TransmitFile(socket.Handle, fileHandle, size, 0, overlappedHandle, IntPtr.Zero, 0x20 | 0x04))
                {
                    lastWin32Error = (SocketError)Marshal.GetLastWin32Error();
                }
                lastWin32Error = (SocketError)_overlappedCheckAsyncCallOverlappedResult.Invoke(overlapped, new object[] { lastWin32Error });
                if (SocketError.Success == lastWin32Error)
                {
                    FinishPostingAsyncSendOp(overlapped, cacheSet);
                    return SocketError.Success;
                }
                object oldCache = _cacheSendOverlappedCache.GetValue(cacheSet);
                object[] overlappedCache = new object[] { oldCache };
                _overlappedExtractCache.Invoke(overlapped, overlappedCache);
                if (oldCache != overlappedCache[0])
                {
                    _cacheSendOverlappedCache.SetValue(cacheSet, overlappedCache[0]);
                }
                _sockektUpdateStatusAfterSocketError.Invoke(socket, new object[] { lastWin32Error });
                return lastWin32Error;
            }
        }

        bool _closed;
        Socket _socket;
        EndPoint _localPoint;
        socket_same_handler _readSameHandler;
        socket_same_handler _writeSameHandler;
        socket_ptr_handler _readPtrHandler;
        socket_ptr_handler _writePtrHandler;

        public socket_tcp()
        {
            _closed = false;
            _socket = null;
            _readSameHandler.handler = delegate (IAsyncResult ar)
            {
                object pinnedObj = _readSameHandler.pinnedObj;
                Action<socket_result> callback = _readSameHandler.callback;
                _readSameHandler.buffer = default;
                _readSameHandler.pinnedObj = null;
                _readSameHandler.callback = null;
                try
                {
                    int size = _socket.EndReceive(ar);
                    functional.catch_invoke(callback, new socket_result(true, size));
                }
                catch (System.Exception ec)
                {
                    functional.catch_invoke(callback, new socket_result(false, 0, ec));
                }
            };
            _writeSameHandler.handler = delegate (IAsyncResult ar)
            {
                object pinnedObj = _writeSameHandler.pinnedObj;
                Action<socket_result> callback = _writeSameHandler.callback;
                _writeSameHandler.buffer = default;
                _writeSameHandler.pinnedObj = null;
                _writeSameHandler.callback = null;
                try
                {
                    int size = _socket.EndSend(ar);
                    functional.catch_invoke(callback, new socket_result(true, size));
                }
                catch (System.Exception ec)
                {
                    functional.catch_invoke(callback, new socket_result(false, 0, ec));
                }
            };
            _readPtrHandler.handler = delegate (socket_result tempRes)
            {
                if (tempRes.success)
                {
                    _readPtrHandler.currTotal += tempRes.size;
                    _readPtrHandler.pointer += tempRes.size;
                    _readPtrHandler.size -= tempRes.size;
                    if (0 == _readPtrHandler.size)
                    {
                        object pinnedObj = _readPtrHandler.pinnedObj;
                        Action<socket_result> callback = _readPtrHandler.callback;
                        _readPtrHandler.pinnedObj = null;
                        _readPtrHandler.callback = null;
                        functional.catch_invoke(callback, new socket_result(true, _readPtrHandler.currTotal));
                    }
                    else
                    {
                        async_read_same(_readPtrHandler.pointer, 0, _readPtrHandler.size, _readPtrHandler.handler);
                    }
                }
                else
                {
                    object pinnedObj = _readPtrHandler.pinnedObj;
                    Action<socket_result> callback = _readPtrHandler.callback;
                    _readPtrHandler.pinnedObj = null;
                    _readPtrHandler.callback = null;
                    functional.catch_invoke(callback, new socket_result(false, _readPtrHandler.currTotal, tempRes.ec));
                }
            };
            _writePtrHandler.handler = delegate (socket_result tempRes)
            {
                if (tempRes.success)
                {
                    _writePtrHandler.currTotal += tempRes.size;
                    _writePtrHandler.pointer += tempRes.size;
                    _writePtrHandler.size -= tempRes.size;
                    if (0 == _writePtrHandler.size)
                    {
                        object pinnedObj = _writePtrHandler.pinnedObj;
                        Action<socket_result> callback = _writePtrHandler.callback;
                        _writePtrHandler.pinnedObj = null;
                        _writePtrHandler.callback = null;
                        functional.catch_invoke(callback, new socket_result(true, _writePtrHandler.currTotal));
                    }
                    else
                    {
                        async_write_same(_writePtrHandler.pointer, 0, _writePtrHandler.size, _writePtrHandler.handler);
                    }
                }
                else
                {
                    object pinnedObj = _writePtrHandler.pinnedObj;
                    Action<socket_result> callback = _writePtrHandler.callback;
                    _writePtrHandler.pinnedObj = null;
                    _writePtrHandler.callback = null;
                    functional.catch_invoke(callback, new socket_result(false, _writePtrHandler.currTotal, tempRes.ec));
                }
            };
        }

        public Socket socket
        {
            get
            {
                return _socket;
            }
        }

        public override void close()
        {
            try
            {
                Socket socket = null;
                lock (this)
                {
                    if (!_closed)
                    {
                        _closed = true;
                        socket = _socket;
                    }
                }
                socket?.Close();
            }
            catch (System.Exception) { }
        }

        public void bind(string ip)
        {
            _localPoint = null == ip ? null : new IPEndPoint(IPAddress.Parse(ip), 0);
        }

        public void async_connect(string ip, int port, Action<socket_result> callback)
        {
            try
            {
                bool isEmpty = false;
                lock (this) isEmpty = !_closed && null == _socket;
                if (!isEmpty)
                {
                    functional.catch_invoke(callback, new socket_result(false));
                    return;
                }
                Socket newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                if (null != _localPoint)
                {
                    newSocket.Bind(_localPoint);
                }
                newSocket.BeginConnect(IPAddress.Parse(ip), port, delegate (IAsyncResult ar)
                {
                    try
                    {
                        newSocket.EndConnect(ar);
                        newSocket.NoDelay = true;
                        bool state = false;
                        lock (this)
                        {
                            if (!_closed && null == _socket)
                            {
                                _socket = newSocket;
                                state = true;
                            }
                        }
                        if (!state)
                        {
                            newSocket.Close();
                        }
                        functional.catch_invoke(callback, new socket_result(state));
                    }
                    catch (System.Exception ec)
                    {
                        functional.catch_invoke(callback, new socket_result(false, 0, ec));
                    }
                }, null);
            }
            catch (System.Exception ec)
            {
                close();
                functional.catch_invoke(callback, new socket_result(false, 0, ec));
            }
        }

        public void async_disconnect(bool reuseSocket, Action<socket_result> callback)
        {
            try
            {
                _socket.BeginDisconnect(reuseSocket, delegate (IAsyncResult ar)
                {
                    try
                    {
                        _socket.EndDisconnect(ar);
                        functional.catch_invoke(callback, new socket_result(true));
                    }
                    catch (System.Exception ec)
                    {
                        functional.catch_invoke(callback, new socket_result(false, 0, ec));
                    }
                }, null);
            }
            catch (System.Exception ec)
            {
                close();
                functional.catch_invoke(callback, new socket_result(false, 0, ec));
            }
        }

        public override void async_read_same(ArraySegment<byte> buff, Action<socket_result> callback)
        {
            try
            {
                Debug.Assert(null == _readSameHandler.callback, "重入的 read 操作!");
                _readSameHandler.callback = callback;
                _socket.BeginReceive(buff.Array, buff.Offset, buff.Count, 0, _readSameHandler.handler, null);
            }
            catch (System.Exception ec)
            {
                close();
                _readSameHandler.callback = null;
                functional.catch_invoke(callback, new socket_result(false, 0, ec));
            }
        }

        public override void async_write_same(ArraySegment<byte> buff, Action<socket_result> callback)
        {
            try
            {
                Debug.Assert(null == _writeSameHandler.callback, "重入的 write 操作!");
                _writeSameHandler.callback = callback;
                _socket.BeginSend(buff.Array, buff.Offset, buff.Count, 0, _writeSameHandler.handler, null);
            }
            catch (System.Exception ec)
            {
                close();
                _writeSameHandler.callback = null;
                functional.catch_invoke(callback, new socket_result(false, 0, ec));
            }
        }

        public void async_send_file(SafeHandle fileHandle, long offset, int size, Action<socket_result> callback, object pinnedObj = null)
        {
            try
            {
                Debug.Assert(null == _writeSameHandler.callback, "重入的 write 操作!");
                _writeSameHandler.pinnedObj = pinnedObj;
                _writeSameHandler.callback = callback;
                SocketError lastWin32Error = SocketAsyncIo.SendFile(_socket, fileHandle, offset, size, _writeSameHandler.handler, _writeSameHandler.pinnedObj);
                if (SocketError.Success != lastWin32Error)
                {
                    close();
                    _writeSameHandler.pinnedObj = null;
                    _writeSameHandler.callback = null;
                    functional.catch_invoke(callback, new socket_result(false, (int)lastWin32Error));
                }
            }
            catch (System.Exception ec)
            {
                close();
                _writeSameHandler.pinnedObj = null;
                _writeSameHandler.callback = null;
                functional.catch_invoke(callback, new socket_result(false, 0, ec));
            }
        }

        public void async_read_same(IntPtr pointer, long offset, int size, Action<socket_result> callback, object pinnedObj = null)
        {
            try
            {
                Debug.Assert(null == _readSameHandler.callback, "重入的 read 操作!");
                _readSameHandler.buffer.Pointer = new IntPtr(pointer.ToInt64() + offset);
                _readSameHandler.buffer.Length = size;
                _readSameHandler.pinnedObj = pinnedObj;
                _readSameHandler.callback = callback;
                SocketError lastWin32Error = SocketAsyncIo.Recv(_socket, ref _readSameHandler.buffer, 0, _readSameHandler.handler, _readSameHandler.pinnedObj);
                if (SocketError.Success != lastWin32Error)
                {
                    close();
                    _readSameHandler.buffer = default;
                    _readSameHandler.pinnedObj = null;
                    _readSameHandler.callback = null;
                    functional.catch_invoke(callback, new socket_result(false, (int)lastWin32Error));
                }
            }
            catch (System.Exception ec)
            {
                close();
                _readSameHandler.buffer = default;
                _readSameHandler.pinnedObj = null;
                _readSameHandler.callback = null;
                functional.catch_invoke(callback, new socket_result(false, 0, ec));
            }
        }

        public void async_write_same(IntPtr pointer, long offset, int size, Action<socket_result> callback, object pinnedObj = null)
        {
            try
            {
                Debug.Assert(null == _writeSameHandler.callback, "重入的 write 操作!");
                _writeSameHandler.buffer.Pointer = new IntPtr(pointer.ToInt64() + offset);
                _writeSameHandler.buffer.Length = size;
                _writeSameHandler.pinnedObj = pinnedObj;
                _writeSameHandler.callback = callback;
                SocketError lastWin32Error = SocketAsyncIo.Send(_socket, ref _writeSameHandler.buffer, 0, _writeSameHandler.handler, _writeSameHandler.pinnedObj);
                if (SocketError.Success != lastWin32Error)
                {
                    close();
                    _writeSameHandler.buffer = default;
                    _writeSameHandler.pinnedObj = null;
                    _writeSameHandler.callback = null;
                    functional.catch_invoke(callback, new socket_result(false, (int)lastWin32Error));
                }
            }
            catch (System.Exception ec)
            {
                close();
                _writeSameHandler.buffer = default;
                _writeSameHandler.pinnedObj = null;
                _writeSameHandler.callback = null;
                functional.catch_invoke(callback, new socket_result(false, 0, ec));
            }
        }

        public void async_read(IntPtr pointer, long offset, int size, Action<socket_result> callback, object pinnedObj = null)
        {
            _readPtrHandler.currTotal = 0;
            _readPtrHandler.pointer = new IntPtr(pointer.ToInt64() + offset);
            _readPtrHandler.size = size;
            _readPtrHandler.pinnedObj = pinnedObj;
            _readPtrHandler.callback = callback;
            async_read_same(_readPtrHandler.pointer, 0, _readPtrHandler.size, _readPtrHandler.handler);
        }

        public void async_write(IntPtr pointer, long offset, int size, Action<socket_result> callback, object pinnedObj = null)
        {
            _writePtrHandler.currTotal = 0;
            _writePtrHandler.pointer = new IntPtr(pointer.ToInt64() + offset);
            _writePtrHandler.size = size;
            _writePtrHandler.pinnedObj = pinnedObj;
            _writePtrHandler.callback = callback;
            async_write_same(_writePtrHandler.pointer, 0, _writePtrHandler.size, _writePtrHandler.handler);
        }

        public ValueTask<socket_result> read_same(IntPtr pointer, long offset, int size, object pinnedObj = null)
        {
            return generator.async_call((Action<socket_result> callback) => async_read_same(pointer, offset, size, callback, pinnedObj));
        }

        public ValueTask<socket_result> write_same(IntPtr pointer, long offset, int size, object pinnedObj = null)
        {
            return generator.async_call((Action<socket_result> callback) => async_write_same(pointer, offset, size, callback, pinnedObj));
        }

        public ValueTask<socket_result> read(IntPtr pointer, long offset, int size, object pinnedObj = null)
        {
            return generator.async_call((Action<socket_result> callback) => async_read(pointer, offset, size, callback, pinnedObj));
        }

        public ValueTask<socket_result> write(IntPtr pointer, long offset, int size, object pinnedObj = null)
        {
            return generator.async_call((Action<socket_result> callback) => async_write(pointer, offset, size, callback, pinnedObj));
        }

        public ValueTask<socket_result> send_file(SafeHandle fileHandle, long offset = 0, int size = 0, object pinnedObj = null)
        {
            return generator.async_call((Action<socket_result> callback) => async_send_file(fileHandle, offset, size, callback, pinnedObj));
        }

        public ValueTask<socket_result> send_file(System.IO.FileStream file)
        {
            return generator.async_call((Action<socket_result> callback) => async_send_file(file.SafeFileHandle, -1, 0, callback, file));
        }

        public ValueTask<socket_result> connect(string ip, int port)
        {
            return generator.async_call((Action<socket_result> callback) => async_connect(ip, port, callback));
        }

        public ValueTask<socket_result> disconnect(bool reuseSocket)
        {
            return generator.async_call((Action<socket_result> callback) => async_disconnect(reuseSocket, callback));
        }

        public Task unsafe_read_same(async_result_wrap<socket_result> res, IntPtr pointer, long offset, int size, object pinnedObj = null)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => async_read_same(pointer, offset, size, callback, pinnedObj));
        }

        public Task unsafe_write_same(async_result_wrap<socket_result> res, IntPtr pointer, long offset, int size, object pinnedObj = null)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => async_write_same(pointer, offset, size, callback, pinnedObj));
        }

        public Task unsafe_read(async_result_wrap<socket_result> res, IntPtr pointer, long offset, int size, object pinnedObj = null)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => async_read(pointer, offset, size, callback, pinnedObj));
        }

        public Task unsafe_write(async_result_wrap<socket_result> res, IntPtr pointer, long offset, int size, object pinnedObj = null)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => async_write(pointer, offset, size, callback, pinnedObj));
        }

        public Task unsafe_send_file(async_result_wrap<socket_result> res, SafeHandle fileHandle, long offset = 0, int size = 0, object pinnedObj = null)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => async_send_file(fileHandle, offset, size, callback, pinnedObj));
        }

        public Task unsafe_send_file(async_result_wrap<socket_result> res, System.IO.FileStream file)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => async_send_file(file.SafeFileHandle, -1, 0, callback, file));
        }

        public Task unsafe_connect(async_result_wrap<socket_result> res, string ip, int port)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => async_connect(ip, port, callback));
        }

        public Task unsafe_disconnect(async_result_wrap<socket_result> res, bool reuseSocket)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => async_disconnect(reuseSocket, callback));
        }

        public class acceptor
        {
            Socket _socket;
            accept_handler _acceptHandler;
            accept_lost_handler _lostHandler;

            public acceptor()
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _acceptHandler.socket = null;
                _acceptHandler.callback = null;
                _acceptHandler.handler = delegate (IAsyncResult ar)
                {
                    socket_tcp socket = _acceptHandler.socket;
                    Action<socket_result> callback = _acceptHandler.callback;
                    _acceptHandler.socket = null;
                    _acceptHandler.callback = null;
                    try
                    {
                        Socket newSocket = _socket.EndAccept(ar);
                        newSocket.NoDelay = true;
                        bool state = false;
                        lock (socket)
                        {
                            if (!socket._closed && null == socket._socket)
                            {
                                socket._socket = newSocket;
                                state = true;
                            }
                        }
                        if (!state)
                        {
                            newSocket.Close();
                        }
                        functional.catch_invoke(callback, new socket_result(state));
                    }
                    catch (System.Exception ec)
                    {
                        functional.catch_invoke(callback, new socket_result(false, 0, ec));
                    }
                };
                _lostHandler.handler = delegate (socket_result res)
                {
                    _lostHandler.socket?.close();
                    _lostHandler.socket = null;
                };
            }

            public Socket socket
            {
                get
                {
                    return _socket;
                }
            }

            public bool resue
            {
                set
                {
                    try
                    {
                        _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, value);
                    }
                    catch (System.Exception) { }
                }
            }

            public bool bind(string ip, int port, int backlog = 1)
            {
                try
                {
                    _socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
                    _socket.Listen(backlog);
                    return true;
                }
                catch (System.Exception) { }
                return false;
            }

            public void close()
            {
                try
                {
                    _socket.Close();
                }
                catch (System.Exception) { }
            }

            public void async_accept(socket_tcp socket, Action<socket_result> callback)
            {
                try
                {
                    bool isEmpty;
                    lock (socket) isEmpty = !socket._closed && null == socket._socket;
                    if (isEmpty)
                    {
                        _acceptHandler.socket = socket;
                        _acceptHandler.callback = callback;
                        _socket.BeginAccept(_acceptHandler.handler, null);
                    }
                    else
                    {
                        functional.catch_invoke(callback, new socket_result(false));
                    }
                }
                catch (System.Exception ec)
                {
                    close();
                    functional.catch_invoke(callback, new socket_result(false, 0, ec));
                }
            }

            public ValueTask<socket_result> accept(socket_tcp sck)
            {
                _lostHandler.socket = sck;
                return generator.async_call((Action<socket_result> callback) => async_accept(sck, callback), _lostHandler.handler);
            }

            public Task unsafe_accept(async_result_wrap<socket_result> res, socket_tcp sck)
            {
                return generator.unsafe_async_call(res, (Action<socket_result> callback) => async_accept(sck, callback));
            }
        }
    }

    struct pipe_same_handler
    {
        public ArraySegment<byte> buff;
        public object pinnedObj;
        public Action<socket_result> callback;
        public AsyncCallback handler;
    }

#if !NETCORE
    public class socket_serial : socket
    {
        SerialPort _socket;
        pipe_same_handler _readSameHandler;
        pipe_same_handler _writeSameHandler;

        public socket_serial(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            _socket = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            _readSameHandler.pinnedObj = null;
            _readSameHandler.callback = null;
            _readSameHandler.handler = delegate (IAsyncResult ar)
            {
                Action<socket_result> callback = _readSameHandler.callback;
                _readSameHandler.callback = null;
                try
                {
                    int size = _socket.BaseStream.EndRead(ar);
                    functional.catch_invoke(callback, new socket_result(true, size));
                }
                catch (System.Exception ec)
                {
                    functional.catch_invoke(callback, new socket_result(false, 0, ec));
                }
            };
            _writeSameHandler.pinnedObj = null;
            _writeSameHandler.callback = null;
            _writeSameHandler.handler = delegate (IAsyncResult ar)
            {
                ArraySegment<byte> buff = _writeSameHandler.buff;
                Action<socket_result> callback = _writeSameHandler.callback;
                _writeSameHandler.buff = default(ArraySegment<byte>);
                _writeSameHandler.callback = null;
                try
                {
                    _socket.BaseStream.EndWrite(ar);
                    functional.catch_invoke(callback, new socket_result(true, buff.Count));
                }
                catch (System.Exception ec)
                {
                    functional.catch_invoke(callback, new socket_result(false, 0, ec));
                }
            };
        }

        public SerialPort socket
        {
            get
            {
                return _socket;
            }
        }

        public bool open()
        {
            try
            {
                _socket.Open();
                return true;
            }
            catch (System.Exception) { }
            return false;
        }

        public override void close()
        {
            try
            {
                _socket.Close();
            }
            catch (System.Exception) { }
        }

        public override void async_read_same(ArraySegment<byte> buff, Action<socket_result> callback)
        {
            try
            {
                Debug.Assert(null == _readSameHandler.callback, "重入的 read 操作!");
                _readSameHandler.callback = callback;
                _socket.BaseStream.BeginRead(buff.Array, buff.Offset, buff.Count, _readSameHandler.handler, null);
            }
            catch (System.Exception ec)
            {
                close();
                _readSameHandler.callback = null;
                functional.catch_invoke(callback, new socket_result(false, 0, ec));
            }
        }

        public override void async_write_same(ArraySegment<byte> buff, Action<socket_result> callback)
        {
            try
            {
                Debug.Assert(null == _writeSameHandler.callback, "重入的 write 操作!");
                _writeSameHandler.buff = buff;
                _writeSameHandler.callback = callback;
                _socket.BaseStream.BeginWrite(buff.Array, buff.Offset, buff.Count, _writeSameHandler.handler, null);
            }
            catch (System.Exception ec)
            {
                close();
                _writeSameHandler.buff = default(ArraySegment<byte>);
                _writeSameHandler.callback = null;
                functional.catch_invoke(callback, new socket_result(false, 0, ec));
            }
        }

        public void clear_in_buffer()
        {
            try
            {
                _socket.DiscardInBuffer();
            }
            catch (System.Exception) { }
        }

        public void clear_out_buffer()
        {
            try
            {
                _socket.DiscardOutBuffer();
            }
            catch (System.Exception) { }
        }
    }
#endif

    public abstract class socket_pipe : socket
    {
        protected PipeStream _socket;
        pipe_same_handler _readSameHandler;
        pipe_same_handler _writeSameHandler;
        socket_ptr_handler _readPtrHandler;
        socket_ptr_handler _writePtrHandler;

        protected socket_pipe()
        {
            _readSameHandler.pinnedObj = null;
            _readSameHandler.callback = null;
            _readSameHandler.handler = delegate (IAsyncResult ar)
            {
                Action<socket_result> callback = _readSameHandler.callback;
                _readSameHandler.callback = null;
                try
                {
                    int size = _socket.EndRead(ar);
                    functional.catch_invoke(callback, new socket_result(true, size));
                }
                catch (System.Exception ec)
                {
                    functional.catch_invoke(callback, new socket_result(false, 0, ec));
                }
            };
            _writeSameHandler.pinnedObj = null;
            _writeSameHandler.callback = null;
            _writeSameHandler.handler = delegate (IAsyncResult ar)
            {
                ArraySegment<byte> buff = _writeSameHandler.buff;
                Action<socket_result> callback = _writeSameHandler.callback;
                _writeSameHandler.buff = default(ArraySegment<byte>);
                _writeSameHandler.callback = null;
                try
                {
                    _socket.EndWrite(ar);
                    functional.catch_invoke(callback, new socket_result(true, buff.Count));
                }
                catch (System.Exception ec)
                {
                    functional.catch_invoke(callback, new socket_result(false, 0, ec));
                }
            };
            _readPtrHandler.pinnedObj = null;
            _readPtrHandler.callback = null;
            _readPtrHandler.handler = delegate (socket_result tempRes)
            {
                if (tempRes.success)
                {
                    _readPtrHandler.currTotal += tempRes.size;
                    _readPtrHandler.pointer += tempRes.size;
                    _readPtrHandler.size -= tempRes.size;
                    if (0 == _readPtrHandler.size)
                    {
                        object pinnedObj = _readPtrHandler.pinnedObj;
                        Action<socket_result> callback = _readPtrHandler.callback;
                        _readPtrHandler.pinnedObj = null;
                        _readPtrHandler.callback = null;
                        functional.catch_invoke(callback, new socket_result(true, _readPtrHandler.currTotal));
                    }
                    else
                    {
                        async_read_same(_readPtrHandler.pointer, 0, _readPtrHandler.size, _readPtrHandler.handler);
                    }
                }
                else
                {
                    object pinnedObj = _readPtrHandler.pinnedObj;
                    Action<socket_result> callback = _readPtrHandler.callback;
                    _readPtrHandler.pinnedObj = null;
                    _readPtrHandler.callback = null;
                    functional.catch_invoke(callback, new socket_result(false, _readPtrHandler.currTotal, tempRes.ec));
                }
            };
            _writePtrHandler.pinnedObj = null;
            _writePtrHandler.callback = null;
            _writePtrHandler.handler = delegate (socket_result tempRes)
            {
                if (tempRes.success)
                {
                    _writePtrHandler.currTotal += tempRes.size;
                    _writePtrHandler.pointer += tempRes.size;
                    _writePtrHandler.size -= tempRes.size;
                    if (0 == _writePtrHandler.size)
                    {
                        object pinnedObj = _writePtrHandler.pinnedObj;
                        Action<socket_result> callback = _writePtrHandler.callback;
                        _writePtrHandler.pinnedObj = null;
                        _writePtrHandler.callback = null;
                        functional.catch_invoke(callback, new socket_result(true, _writePtrHandler.currTotal));
                    }
                    else
                    {
                        async_write_same(_writePtrHandler.pointer, 0, _writePtrHandler.size, _writePtrHandler.handler);
                    }
                }
                else
                {
                    object pinnedObj = _writePtrHandler.pinnedObj;
                    Action<socket_result> callback = _writePtrHandler.callback;
                    _writePtrHandler.pinnedObj = null;
                    _writePtrHandler.callback = null;
                    functional.catch_invoke(callback, new socket_result(false, _writePtrHandler.currTotal, tempRes.ec));
                }
            };
        }

        public override void async_read_same(ArraySegment<byte> buff, Action<socket_result> callback)
        {
            try
            {
                Debug.Assert(null == _readSameHandler.callback, "重入的 read 操作!");
                _readSameHandler.callback = callback;
                _socket.BeginRead(buff.Array, buff.Offset, buff.Count, _readSameHandler.handler, null);
            }
            catch (System.Exception ec)
            {
                close();
                _readSameHandler.callback = null;
                functional.catch_invoke(callback, new socket_result(false, 0, ec));
            }
        }

        public override void async_write_same(ArraySegment<byte> buff, Action<socket_result> callback)
        {
            try
            {
                Debug.Assert(null == _writeSameHandler.callback, "重入的 write 操作!");
                _writeSameHandler.buff = buff;
                _writeSameHandler.callback = callback;
                _socket.BeginWrite(buff.Array, buff.Offset, buff.Count, _writeSameHandler.handler, null);
            }
            catch (System.Exception ec)
            {
                close();
                _writeSameHandler.buff = default(ArraySegment<byte>);
                _writeSameHandler.callback = null;
                functional.catch_invoke(callback, new socket_result(false, 0, ec));
            }
        }

        public void async_read_same(IntPtr pointer, long offset, int size, Action<socket_result> callback, object pinnedObj = null)
        {
            new Task(delegate ()
            {
                if (_socket.IsConnected)
                {
                    try
                    {
                        object holdPin = pinnedObj;
                        tuple<bool, int> res = sync_read(_socket.SafePipeHandle, pointer, offset, size);
                        functional.catch_invoke(callback, new socket_result(res.value1, res.value2));
                        return;
                    }
                    catch (System.Exception) { }
                }
                functional.catch_invoke(callback, new socket_result(false, 0));
            }).Start();
        }

        public void async_write_same(IntPtr pointer, long offset, int size, Action<socket_result> callback, object pinnedObj = null)
        {
            new Task(delegate ()
            {
                if (_socket.IsConnected)
                {
                    try
                    {
                        object holdPin = pinnedObj;
                        tuple<bool, int> res = sync_write(_socket.SafePipeHandle, pointer, offset, size);
                        functional.catch_invoke(callback, new socket_result(res.value1, res.value2));
                        return;
                    }
                    catch (System.Exception) { }
                }
                functional.catch_invoke(callback, new socket_result(false, 0));
            }).Start();
        }

        public void async_read(IntPtr pointer, long offset, int size, Action<socket_result> callback, object pinnedObj = null)
        {
            _readPtrHandler.currTotal = 0;
            _readPtrHandler.pointer = new IntPtr(pointer.ToInt64() + offset);
            _readPtrHandler.size = size;
            _readPtrHandler.pinnedObj = pinnedObj;
            _readPtrHandler.callback = callback;
            async_read_same(_readPtrHandler.pointer, 0, _readPtrHandler.size, _readPtrHandler.handler);
        }

        public void async_write(IntPtr pointer, long offset, int size, Action<socket_result> callback, object pinnedObj = null)
        {
            _writePtrHandler.currTotal = 0;
            _writePtrHandler.pointer = new IntPtr(pointer.ToInt64() + offset);
            _writePtrHandler.size = size;
            _writePtrHandler.pinnedObj = pinnedObj;
            _writePtrHandler.callback = callback;
            async_write_same(_writePtrHandler.pointer, 0, _writePtrHandler.size, _writePtrHandler.handler);
        }

        public ValueTask<socket_result> read_same(IntPtr pointer, long offset, int size, object pinnedObj = null)
        {
            return generator.async_call((Action<socket_result> callback) => async_read_same(pointer, offset, size, callback, pinnedObj));
        }

        public ValueTask<socket_result> write_same(IntPtr pointer, long offset, int size, object pinnedObj = null)
        {
            return generator.async_call((Action<socket_result> callback) => async_write_same(pointer, offset, size, callback, pinnedObj));
        }

        public ValueTask<socket_result> read(IntPtr pointer, long offset, int size, object pinnedObj = null)
        {
            return generator.async_call((Action<socket_result> callback) => async_read(pointer, offset, size, callback, pinnedObj));
        }

        public ValueTask<socket_result> write(IntPtr pointer, long offset, int size, object pinnedObj = null)
        {
            return generator.async_call((Action<socket_result> callback) => async_write(pointer, offset, size, callback, pinnedObj));
        }

        public Task unsafe_read_same(async_result_wrap<socket_result> res, IntPtr pointer, long offset, int size, object pinnedObj = null)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => async_read_same(pointer, offset, size, callback, pinnedObj));
        }

        public Task unsafe_write_same(async_result_wrap<socket_result> res, IntPtr pointer, long offset, int size, object pinnedObj = null)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => async_write_same(pointer, offset, size, callback, pinnedObj));
        }

        public Task unsafe_read(async_result_wrap<socket_result> res, IntPtr pointer, long offset, int size, object pinnedObj = null)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => async_read(pointer, offset, size, callback, pinnedObj));
        }

        public Task unsafe_write(async_result_wrap<socket_result> res, IntPtr pointer, long offset, int size, object pinnedObj = null)
        {
            return generator.unsafe_async_call(res, (Action<socket_result> callback) => async_write(pointer, offset, size, callback, pinnedObj));
        }
    }

    public class socket_pipe_server : socket_pipe
    {
        readonly string _pipeName;

        public socket_pipe_server(string pipeName, int inBufferSize = 4 * 1024, int outBufferSize = 4 * 1024)
        {
            _pipeName = pipeName;
            _socket = new NamedPipeServerStream("CsGo_" + pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, inBufferSize, outBufferSize);
        }

        public NamedPipeServerStream socket
        {
            get
            {
                return (NamedPipeServerStream)_socket;
            }
        }

        public override void close()
        {
            if (_socket.IsConnected)
            {
                try
                {
                    ((NamedPipeServerStream)_socket).Disconnect();
                }
                catch (System.Exception) { }
            }
            try
            {
                _socket.Close();
            }
            catch (System.Exception) { }
        }

        public async Task<bool> wait_connection(int ms = -1)
        {
            bool overtime = false;
            async_timer waitTimeout = null;
            try
            {
                if (ms >= 0)
                {
                    waitTimeout = new async_timer(generator.self_strand());
                    waitTimeout.timeout(ms, delegate ()
                    {
                        overtime = true;
                        try
                        {
                            NamedPipeClientStream timedPipe = new NamedPipeClientStream(".", "CsGo_" + _pipeName);
                            timedPipe.Connect(0);
                            timedPipe.Close();
                        }
                        catch (System.Exception) { }
                    });
                }
                await generator.send_task(((NamedPipeServerStream)_socket).WaitForConnection);
                if (overtime)
                {
                    close();
                }
                waitTimeout?.cancel();
            }
            catch (System.Exception)
            {
                waitTimeout?.advance();
                close();
                throw;
            }
            return !overtime;
        }
    }

    public class socket_pipe_client : socket_pipe
    {
        public socket_pipe_client(string pipeName, string serverName = ".")
        {
            _socket = new NamedPipeClientStream(serverName, "CsGo_" + pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
        }

        public NamedPipeClientStream socket
        {
            get
            {
                return (NamedPipeClientStream)_socket;
            }
        }

        public override void close()
        {
            if (_socket.IsConnected)
            {
                try
                {
                    cancel_io(_socket.SafePipeHandle);
                }
                catch (System.Exception) { }
            }
            try
            {
                _socket.Close();
            }
            catch (System.Exception) { }
        }

        public bool try_connect()
        {
            try
            {
                ((NamedPipeClientStream)_socket).Connect(0);
                return true;
            }
            catch (System.Exception) { }
            return false;
        }

        public async Task<bool> connect(int ms = -1)
        {
            if (0 == ms)
            {
                return try_connect();
            }
            long beginTick = system_tick.get_tick_us();
            while (!try_connect())
            {
                await generator.sleep(1);
                if (ms >= 0 && system_tick.get_tick_us() - beginTick >= (long)ms * 1000)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
