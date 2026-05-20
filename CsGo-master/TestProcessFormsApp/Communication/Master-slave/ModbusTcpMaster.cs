using JLRScan;
using JModbusClient;
using Modbus.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static TestProcessFormsApp.Communication.CAEA008TCPModbus;

namespace TestProcessFormsApp.Communication.Master_slave
{
    public class ModbusTcpMaster
    {
        private string ipAddress;
        private int _port;
        public bool ExitFlag = false;
        public System.Net.Sockets.TcpClient client;
        public ModbusIpMaster Tcpmaster2;
        ScanLog scanLog = new ScanLog();
        public Timer timer;
        public short Timedelay = 4000;//循环发送报文的时间间隔
        public short[] CurrentGroup = new short[2];
        public ushort LinStartAdree = 1;
        public int Lin1Value = 0;
        public int Lin2Value = 0;
        public Queue<string> WriteQueue = new Queue<string>();
        private CancellationTokenSource _cts;
        private Task _readTask;
        private IJModbusClient jModbusClient;
        private int LinStartIndex = 0;
        public ModbusTcpMaster(string _ipAddress,int port)
        {
            ipAddress = _ipAddress;
            _port = port;
            jModbusClient = JModbusClientFactory.CreateTcpClient(_ipAddress.ToString(), port);
        }
        public async Task ModbusTcp_Init(int index)
        {
            try
            {
                jModbusClient.SetSlaveAddress(1);
                await jModbusClient.ConnectAsync();
                LinStartIndex = 1 + (index - 2) * 2;
                StartAutoRead( 1 + (index - 2) * 2, 2);
                timer = new Timer(TimerCallback, null, 300, Timedelay); // 参数分别是：回调方法、状态对象、延迟时间、周期时间
            }
            catch (Exception ex)
            {
                scanLog.InsertLog(ipAddress + ex.Message);
            }
        }
        public void StartAutoRead( int startindex, int lenth)
        {
            if (_readTask != null && !_readTask.IsCompleted)
                return; // 已经在运行

            _cts = new CancellationTokenSource();
            _readTask = Task.Run(async () =>
            {
                while (!_cts.IsCancellationRequested)
                {
                    try
                    {
                        var value = await jModbusClient.ReadHoldingRegisterAsync(startindex, lenth, _cts.Token);
                        Lin1Value = value[0];
                        Lin2Value = value[1];
                    }
                    catch (OperationCanceledException)
                    {
                        //取消操作
                    }
                    catch (TimeoutException ex)
                    {
                        scanLog.InsertLog("SI03读操作超时" + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        scanLog.InsertLog(ex.Message);
                    }
                    await Task.Delay(2000, _cts.Token);
                }
            }, _cts.Token);
        }
        /// <summary>
        /// 停止定时读取任务
        /// </summary>
        public void StopAutoRead()
        {
            _cts?.Cancel();
            try { _readTask?.Wait(); } catch { }
            _readTask = null;
            _cts = null;
        }
        public void TimerCallback(object state)
        {
            // 这里执行定时任务
            try
            {
                CheckConnectionAndReconnect();
            
            }
            catch (Exception ex)
            {
                scanLog.InsertLog("主从机：" + ipAddress + ex.Message);
            }
        }
        public void CheckConnectionAndReconnect()
        {

           // if (!jModbusClient.Connected)
            {
                jModbusClient.ConnectAsync();
                WriteD();
            }
        }
        public void Connect()
        {
            client = new System.Net.Sockets.TcpClient();
            client.Connect(ipAddress, _port);
            Tcpmaster2 = ModbusIpMaster.CreateIp(client);
        }
        public void WriteD()
        {
            while (WriteQueue.Count > 0)//写入优先。
            {
                try
                {
                     var DWriteCom = WriteQueue.Dequeue();
                     ParseWrite(DWriteCom);
                }
                catch (Exception ex)
                {
                    scanLog.InsertLog(ex.Message + "12");
                    break;
                }
                Thread.Sleep(1);
            }
        }
        void ParseWrite(string st)
        {
            string[] parts = st.Split('-');
            if (parts.Length > 1)
            {
                jModbusClient.WriteSingleRegister(Convert.ToUInt16(parts[0]), Convert.ToUInt16(parts[1]));
            }
            
        }
        public void ClearLinD()
        {   
            jModbusClient.WriteSingleRegister(LinStartIndex, 0);
            jModbusClient.WriteSingleRegister(LinStartIndex + 1, 0);
        }
    }
}
