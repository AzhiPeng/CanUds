using JModbusClient.protocol;
using System.IO.Ports;

namespace JModbusClient
{
    public class JModbusClientFactory
    {
        public static IJModbusClient CreateTcpClient(string host, int port)
        {
            return new JModbusTcpClient(host, port);
        }
        public static IJModbusSI03Client CreateSI03Client(string host, int port,bool isactive=false)
        {
            return new JModbusSI03Client(host, port, isactive);
        }
        public static IJModbusClient CreateRtuClient(string portName, int baudRate = 9600,
            Parity parity = Parity.Even, StopBits stopBits = StopBits.One)
        {
            return new JModbusRtuClient(portName, baudRate, parity, stopBits);
        }
    }
}
