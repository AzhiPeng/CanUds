using JLRScan;
using Modbus.Data;
using Modbus.Device;
using Modbus.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace TestProcessFormsApp.Communication
{
    public class ModbusTcpSalve
    {
        #region 单例
        static readonly Lazy<ModbusTcpSalve> instance = new Lazy<ModbusTcpSalve>(() => new ModbusTcpSalve());
        /// <summary>
        /// 单例
        /// </summary>
        public static ModbusTcpSalve Instance => instance.Value;
        #endregion

        public enum ModbusFunctionCodes
        {
            _ReadCoils = 0x01,
            _ReadDiscreteInputs = 0x02,
            _ReadHoldingRegisters = 0x03,
            _ReadInputRegisters = 0x04,
            _WriteSingleCoil = 0x05,
            _WriteSingleRegister = 0x06,
            _WriteMultipleCoils = 0x0F,
            _WriteMultipleRegisters = 0x10,
        }
        /// <summary>
        /// 服务器提供的数据区
        /// </summary>
        public static DataStore Data = new DataStore();

        /// <summary>
        /// Modbus服务器
        /// </summary>
        public static ModbusSlave slave;

        public static ScanLog scanLog { get; set; } = new ScanLog();
        public static void CreateTcp(IPAddress ip, int port)
        {
            try
            {
                TcpListener listener = new TcpListener(ip, port);
                listener.Start();
                slave = ModbusTcpSlave.CreateTcp(1, listener);
                slave.DataStore = DataStoreFactory.CreateDefaultDataStore();
                scanLog.InsertLog("创建服务器成功！" + "线程ID：" + Thread.CurrentThread.ManagedThreadId);
                Data.HoldingRegisters.Add(0);
                Data.HoldingRegisters.Add(0);
                Data.HoldingRegisters.Add(0);
                Data.HoldingRegisters.Add(0);
                Data.HoldingRegisters.Add(0);
                Data.HoldingRegisters.Add(0);
                Data.HoldingRegisters.Add(0);
                //订阅接收到报文请求事件，可以打印接收到的报文
                slave.ModbusSlaveRequestReceived += _modbusSlave_ModbusSlaveRequestReceived;
                slave.Listen();
            }
            catch (Exception ex)
            {
                scanLog.InsertLog("创建服务器失败！");
                return;
            }
        }
        public static void StopTcp()
        {
            try
            {
                slave.Dispose();
            }
            catch (Exception ex)
            {

            }

        }
        public static void DataClear(int index)
        {
            Data.HoldingRegisters[index] = 0;
            Data.CoilDiscretes[index] = false;
        }
        /// <summary>
        /// 接收事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void _modbusSlave_ModbusSlaveRequestReceived(object sender, ModbusSlaveRequestEventArgs e)
        {
            string str = "";
            foreach (var item in e.Message.MessageFrame)
            {
                str += item.ToString("x2").PadLeft(2, '0').ToUpper();
            }
            //   scanLog.InsertLog("服务器收到报文:  " + str );
            Data = ((ModbusSlave)sender).DataStore;
            // 根据请求类型和地址判断哪个元件发生了变化
            if (e.Message.FunctionCode == (byte)ModbusFunctionCodes._WriteSingleRegister)
            {
                WriteSingleRegisterRequestResponse request = (WriteSingleRegisterRequestResponse)e.Message;
                ushort address = request.StartAddress;
                ushort value = request.Data[0];
                scanLog.InsertLog($"寄存器{address}的值已更新为{value}");
            }
            else if (e.Message.FunctionCode == (byte)ModbusFunctionCodes._WriteMultipleRegisters)
            {
                WriteMultipleRegistersRequest request = (WriteMultipleRegistersRequest)e.Message;
                ushort startAddress = request.StartAddress;
                ushort[] values = request.Data.ToArray();

                for (int i = 0; i < values.Length; i++)
                {
                    ushort address = (ushort)(startAddress + i);
                    ushort value = values[i];
                    scanLog.InsertLog($"寄存器{address}的值已更新为{value}");
                }
            }
            else if (e.Message.FunctionCode == (byte)ModbusFunctionCodes._WriteSingleCoil)
            {
                WriteSingleCoilRequestResponse request = (WriteSingleCoilRequestResponse)e.Message;
                ushort address = request.StartAddress;
                ushort value = request.Data[0];
                // 在DataStore中记录变化
                //Data.HoldingRegisters.add(address, value);
                //    Data.HoldingRegisters.Insert(address, value);
                scanLog.InsertLog($"线圈{address}的值已更新为{value}");
            }
            // 处理其他请求类型...
        }
    }
}
