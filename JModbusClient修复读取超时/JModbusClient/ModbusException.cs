using System;
using System.Runtime.Serialization;

namespace JModbusClient
{
    public class ModbusException:Exception
    {
        public ModbusException()
        { 
        }

        public ModbusException(string message)
        : base(message)
        {
        }

        public ModbusException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ModbusException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    public class FunctionCodeNotSupportedException : ModbusException
    {
        public FunctionCodeNotSupportedException()
        {
        }

        public FunctionCodeNotSupportedException(string message)
            : base(message)
        {
        }

        public FunctionCodeNotSupportedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected FunctionCodeNotSupportedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    public class ConnectionException : ModbusException
    {
        public ConnectionException()
        {
        }

        public ConnectionException(string message)
            : base(message)
        {
        }

        public ConnectionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ConnectionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
    public class StartingAddressInvalidException : ModbusException
    {
        public StartingAddressInvalidException()
        {
        }

        public StartingAddressInvalidException(string message)
            : base(message)
        {
        }

        public StartingAddressInvalidException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected StartingAddressInvalidException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
    public class QuantityInvalidException : ModbusException
    {
        public QuantityInvalidException()
        {
        }

        public QuantityInvalidException(string message)
            : base(message)
        {
        }

        public QuantityInvalidException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected QuantityInvalidException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    public class CRCCheckFailedException : ModbusException
    {
        public CRCCheckFailedException()
        {
        }

        public CRCCheckFailedException(string message)
            : base(message)
        {
        }

        public CRCCheckFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CRCCheckFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
