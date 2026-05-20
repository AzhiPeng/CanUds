using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JModbusClient
{
    public interface IJModbusSI03Client : IJModbusClient
    {
        int ActiveReceiveInterval { get; set; }
        bool IsActive { get;}
    }
}
