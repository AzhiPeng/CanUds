using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using JLRScan;
using JModbusClient;

using Modbus.Device;
using Modbus.Extensions.Enron;
//using TestProcessFormsApp.Boards;
using TestProcessFormsApp.测试类;
//using TouchSocket.Sockets;

namespace TestProcessFormsApp.Communication
{
    public class 寄存器
    {
        public ushort 寄存器地址 { get; set; }
        public ushort 值 { get; set; }
        public 寄存器(ushort value1, ushort value2)
        {
            寄存器地址 = value1;
            值 = value2;
        }
    }


    public class 线圈
    {
        public ushort 线圈地址 { get; set; }
        public 状态 值 { get; set; }
        public 线圈(ushort value1, 状态 value2)
        {
            线圈地址 = value1;
            值 = value2;
        }

    }
    public enum 状态
    {
        开启 = 1,
        关闭 = 0
    }
    public  class CAEA008TCPModbus
    {

        ScanLog scanLog = new ScanLog();
        #region 单例
        static readonly Lazy<CAEA008TCPModbus> instance = new Lazy<CAEA008TCPModbus>(() => new CAEA008TCPModbus());
        /// <summary>
        /// 单例
        /// </summary>
        public static CAEA008TCPModbus Instance => instance.Value;
        #endregion
        public  System.Net.Sockets.TcpClient client;//= new TcpClient("127.0.0.1", 502);
      //  public  static ModbusIpMaster Tcpmaster;//= ModbusIpMaster.CreateIp(client);
        public  ConcurrentDictionary<ushort, ushort> DList = new ConcurrentDictionary<ushort, ushort>();
        public  ConcurrentDictionary<ushort, bool>   MList = new ConcurrentDictionary<ushort, bool>();
        internal List<ushort> KeyList = new List<ushort>();
        internal List<List<ushort>> KeySortList = new List<List<ushort>>();
        //public  ConcurrentQueue<寄存器> DWritestack = new ConcurrentQueue<寄存器>();
        //public  ConcurrentQueue<线圈> MWritestack = new ConcurrentQueue<线圈>();
        public 寄存器管理器 _寄存器管理器 = new 寄存器管理器();
        public 线圈管理器 _线圈管理器 = new 线圈管理器();
        public  ConcurrentDictionary<string, ushort> 循环发送队列 = new ConcurrentDictionary<string, ushort>();
        public  ConcurrentDictionary<string, ushort> 常开发送队列 = new ConcurrentDictionary<string, ushort>();
        public  ConcurrentDictionary<string, ushort> 长时1负载开启 = new ConcurrentDictionary<string, ushort>();
        public  ConcurrentDictionary<string, ushort> 长时1负载关闭 = new ConcurrentDictionary<string, ushort>();
        public  ConcurrentDictionary<string, ushort> 长时2负载开启 = new ConcurrentDictionary<string, ushort>();
        public  ConcurrentDictionary<string, ushort> 长时2负载关闭 = new ConcurrentDictionary<string, ushort>();
        private string _ipAddress;
        private int _port;
        public bool ExitFlag = false;
        public  bool CAEA008TX = false;
        public  Timer timer;
        public short Timedelay = 4000;//循环发送报文的时间间隔
        private static IJModbusClient Tcpmaster;
        public class 寄存器管理器
        {
            public ConcurrentQueue<寄存器> DWritestack = new ConcurrentQueue<寄存器>();
            private ConcurrentDictionary<ushort, ushort> 寄存器字典 = new ConcurrentDictionary<ushort, ushort>();

            public void 插入寄存器(ushort 寄存器地址, ushort 值)
            {
                if (!寄存器字典.ContainsKey(寄存器地址))
                {
                    DWritestack.Enqueue(new 寄存器(寄存器地址, 值));
                    寄存器字典.TryAdd(寄存器地址, 值);
                }
                else
                {
           //         Console.WriteLine($"寄存器地址 {寄存器地址} 已存在，值为 {寄存器字典[寄存器地址]}，无法插入重复地址。");
                }
            }
            public void 清空寄存器(ushort 起始地址, int[] 清空缓存)
            {
                try
                {
                    Tcpmaster.WriteMultipleRegisters(起始地址, 清空缓存);
                }
                catch(Exception ex)
                {

                }
                
            }
            public void 移除寄存器(ushort 寄存器地址)
            {
                if (寄存器字典.TryRemove(寄存器地址, out var _))
                {
                    // 从队列中移除所有匹配的寄存器地址
                    while (DWritestack.TryPeek(out var reg) && reg.寄存器地址 == 寄存器地址)
                    {
                        DWritestack.TryDequeue(out var tr);
                    }
                 //   Console.WriteLine($"寄存器地址 {寄存器地址} 已被移除。");
                }
                else
                {
                //    Console.WriteLine($"寄存器地址 {寄存器地址} 不存在，无法移除。");
                }
            }
        }
        public class 线圈管理器
        {
            public ConcurrentQueue<线圈> MWritestack = new ConcurrentQueue<线圈>();
            private ConcurrentDictionary<ushort, ushort> 线圈字典 = new ConcurrentDictionary<ushort, ushort>();

            public void 插入线圈(ushort 线圈地址, ushort 值)
            {
                if (!线圈字典.ContainsKey(线圈地址))
                {
                    MWritestack.Enqueue(new 线圈(线圈地址,(状态)值));
                    线圈字典.TryAdd(线圈地址, (ushort)值);
                }
                else
                {
                    //         Console.WriteLine($"寄存器地址 {寄存器地址} 已存在，值为 {寄存器字典[寄存器地址]}，无法插入重复地址。");
                }
            }

            public void 移除线圈(ushort 线圈地址)
            {
                if (线圈字典.TryRemove(线圈地址, out var _))
                {
                    // 从队列中移除所有匹配的寄存器地址
                    while (MWritestack.TryPeek(out var reg) && reg.线圈地址 == 线圈地址)
                    {
                        MWritestack.TryDequeue(out var tr);
                    }
                //    Console.WriteLine($"寄存器地址 {线圈地址} 已被移除。");
                }
                else
                {
                 //   Console.WriteLine($"寄存器地址 {线圈地址} 不存在，无法移除。");
                }
            }
        }
        public void CAEA008TCPipSet(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
        }
        public  string ModbusTcp_Init(/*ModbusIpMaster PLCmasterTemp, System.Net.Sockets.TcpClient clientTemp*/)
        {
            try
            {
                if (client != null)
                {
                    client.Close();
                    if (Tcpmaster != null)
                        Tcpmaster.Dispose();
                }
                IPAddress ipAddress = IPAddress.Parse(_ipAddress);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, _port);
                Instance.client = new System.Net.Sockets.TcpClient();
                Instance.client.ReceiveTimeout = 1000;
                Instance.client.SendTimeout = 1000;

                Tcpmaster = JModbusClientFactory.CreateTcpClient(_ipAddress.ToString(), _port);
                Tcpmaster.SetSlaveAddress(1);
                Tcpmaster.ConnectAsync();
                // client.Connect(remoteEP);

                //    Tcpmaster = ModbusIpMaster.CreateIp(client);
                CAEA008TX = true;

            //    timer = new Timer(TimerCallback, null, 500, Timedelay); // 参数分别是：回调方法、状态对象、延迟时间、周期时间
                return "True";

            }
            catch (Exception ex)
            {
                CAEA008TX = false;
                scanLog.InsertLog(ex.Message + "11");
                return ex.Message;
            }
        }
        public void AddDList(ushort StartAdree,ushort Lenth) 
        {
            for (ushort i = StartAdree; i < Lenth; i++)
            {
                DList.AddOrUpdate( i, 0, (existingKey, existingValue) => 0);
                KeyList.Add(i);
            }
        }
        public void ModbusTcpThread(object state)
        {
            while (!ExitFlag)
            {
                WriteDList();
                WriteMList();
                UpdataDList();
                Thread.Sleep(100);
                CheckConnectionAndReconnect();
            }
        }
        private static void TimerCallback(object state)
        {
            // 这里执行定时任务
          //   Instance.Circulate();

        //    Console.WriteLine("Timer tick at: " + DateTime.Now);
        }

        public void Write负载开启与关闭(ConcurrentDictionary<string, ushort> keyValuePairs)
        {
            foreach (var kv in keyValuePairs)
            {
                if (kv.Key.Contains("D"))
                {
                    Match match = Regex.Match(kv.Key, @"\d+");
                    if (match.Success)
                    {
                        _寄存器管理器.插入寄存器(Convert.ToUInt16(match.Value), kv.Value);
                     //   DWritestack.Enqueue(new 寄存器(Convert.ToUInt16(match.Value), kv.Value));
                    }
                }
            }
            foreach (var kv in keyValuePairs)
            {
                if (kv.Key.Contains("M"))
                {
                    Match match = Regex.Match(kv.Key, @"\d+");
                    if (match.Success)
                    {
                        _线圈管理器.插入线圈(Convert.ToUInt16(match.Value), kv.Value);
                      //  MWritestack.Enqueue(new 线圈(Convert.ToUInt16(match.Value), (状态)(kv.Value)));
                    }
                }
            }
        }
        /// <summary>
        /// 获取需要写入M的数量
        /// </summary>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        public List<线圈> ReadMNum(ConcurrentDictionary<string, ushort> keyValuePairs)
        {
            List<线圈> list = new List<线圈>();
            foreach (var kv in keyValuePairs)
            {
                if (kv.Key.Contains("M"))
                {
                    Match match = Regex.Match(kv.Key, @"\d+");
                    if (match.Success)
                    {
                        list.Add(new 线圈(Convert.ToUInt16(match.Value),(状态) kv.Value));
                    }
                }
            }
            return list;
        }
        public void WriteDList()
        {
            while (_寄存器管理器.DWritestack.Count > 0)//写入优先。
            {
                try
                {
                    if (_寄存器管理器.DWritestack.TryPeek(out var DWriteCom))
                    {

                        Tcpmaster.WriteSingleRegister((int)DWriteCom.寄存器地址, (int)DWriteCom.值);
                        _寄存器管理器.移除寄存器(DWriteCom.寄存器地址);
                    }
                    CAEA008TX = true;
                }
                catch (Exception ex)
                {
                    CAEA008TX = false;
                    scanLog.InsertLog(ex.Message + "12");
                    break;
                    Thread.Sleep(1000);
                }
                 Thread.Sleep(1);
            }
        }
        public void WriteMList()
        {
            while (_线圈管理器.MWritestack.Count > 0&& _寄存器管理器.DWritestack.Count == 0)//写入优先。
            {
                try
                {
                    if (_线圈管理器.MWritestack.TryPeek(out var MWriteCom))
                    {
                        bool bl = Convert.ToBoolean(MWriteCom.值);
                        if (_寄存器管理器.DWritestack.Count != 0)
                            break;
                        Tcpmaster.WriteSingleCoil((int)(MWriteCom.线圈地址 + 2000), bl);
                        Console.WriteLine(_ipAddress + MWriteCom.线圈地址+"-"+ bl.ToString());
                        _线圈管理器.移除线圈(MWriteCom.线圈地址);
                    }
                    CAEA008TX = true;
                }
                catch (Exception ex)
                {
                    CAEA008TX = false;
                    scanLog.InsertLog(ex.Message + "13");
                    if(ex.Message != "索引超出了数组界限")
                    break;
                    Thread.Sleep(1000);

                }
                Thread.Sleep(200);

            }
        }
        public void UpdataDList()
        {
            try
            {
                
                foreach (var ls in KeySortList)
                {
                    try
                    {
                        Thread.Sleep(10);
                        //        Console.WriteLine("R d:" + ls[0] + " Count:" + ls.Count);
                        var rdd = Tcpmaster.ReadHoldingRegister(ls[0], ls.Count);

                        for (ushort i = 0; i < rdd.Length; i++)
                        {
                            DList.AddOrUpdate((ushort)(ls[0] + i), 0, (existingKey, existingValue) => (ushort)rdd[i]);
                        }
                    }
                    catch(Exception ex) 
                    {
                        scanLog.InsertLog(ex.Message + "19");
                    }
                          
                }

            }
            catch (Exception ex)
            {
                CAEA008TX = false;
                scanLog.InsertLog(ex.Message + "14");
                Thread.Sleep(1000);
            }
           
        }

        public void CheckConnectionAndReconnect()
        {
            
            if (!Tcpmaster.Connected)
            {
                scanLog.InsertLog("Connection lost. Reconnecting...");
                Connect(); // 尝试重新连接
                ModbusTcp_Init();
                Thread.Sleep(3000);
            }
        }
        public void Connect()
        {
            Tcpmaster.ConnectAsync();
            //Tcpmaster = ModbusIpMaster.CreateIp(client);
        }
         public  List<List<ushort>> SortList(List<ushort> list)
        {
            
            // 首先对列表进行排序
            list.Sort();

            // 存储连续数据段的列表
            List<List<ushort>> consecutiveLists = new List<List<ushort>>();
            List<ushort> currentList = new List<ushort>();

            for (int i = 0; i < list.Count; i++)
            {
                if (i == 0 || list[i] == currentList[currentList.Count - 1] + 1)
                {
                    // 如果是列表的第一个元素，或者当前元素是前一个元素的连续值，则添加到当前列表
                    currentList.Add(list[i]);
                }
                else
                {
                    // 如果当前元素不是连续的，则将当前列表添加到结果列表中，并开始一个新的列表
                    consecutiveLists.Add(currentList);
                    currentList = new List<ushort> { list[i] };
                }
            }

            // 添加最后一个列表到结果列表中
            if (currentList.Count > 0)
            {
                consecutiveLists.Add(currentList);
            }
            return consecutiveLists;
        }

    }
}
