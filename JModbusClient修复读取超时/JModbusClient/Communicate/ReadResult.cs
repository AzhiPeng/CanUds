using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JModbusClient
{
    internal enum ReadStatus
    {
        Success,
        Timeout,
        Null,
        Error
    }
    internal class ReadResult<T>
    {
        public ReadStatus Status { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public ReadResult(ReadStatus success, T data, string message = null)
        {
            Status = success;
            Data = data;
            Message = message;
        }
    }
}
