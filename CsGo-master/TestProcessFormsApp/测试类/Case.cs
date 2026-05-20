using CCWin.SkinClass;
using CCWin.Win32.Const;
using JLRScan;
using JLRScan.Log;
//using LoadPlateSerialPort.LoadBoxAT03;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Windows.Markup.Localizer;
using System.Xml.Linq;
//using TestProcessFormsApp.Boards;
using TestProcessFormsApp.Communication;
using TestProcessFormsApp.Communication.CAN;
using TestProcessFormsApp.Communication.Master_slave;
using TestProcessFormsApp.Log;
using TestProcessFormsApp.Public;
using JModbusClient;
using static TestProcessFormsApp.Communication.CAEA008TCPModbus;
using HMIPLC;
using Gpg.NET;
using KeyManageForGm.Model;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using KeyManageForGm.Dal;
using System.Globalization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using static org.apache.zookeeper.ZooDefs;
using CaeaGenKeyProxy;
using TestProcessFormsApp.Properties;
//using TouchSocket.Sockets;

namespace TestProcessFormsApp.测试类
{
    internal interface Case
    {
    }
    public enum 测试结果
    {
        未开始,
        测试中,
        合格,
        不合格
    }
    public enum 负载状态
    {
        关闭 = 0,
        开启 = 1,
    }
    public enum 测试方式
    {
        自动 = 0,
        手动 = 1,
    }
    public enum 操作
    {
        读 = 0,
        写 = 1
    }

    public class 测试项报文开关配置
    {
        public 操作 操作名称 { get; set; }
        public string 写入类型名称 { get; set; }
        public ushort 写入地址 { get; set; }
        public ushort 写入值 { get; set; }
    }
    public interface ICaseInterface
    {
        void Set结果(测试结果 value);
        void Set历史结果(测试结果 value);
        void Set历史结果数据(string value);
        测试结果 GetState();
        测试结果 GetHisToryState();
        测试结果 GetDVHisToryState();
        string GetDVHisToryDate();
        void DVResultInit();
        void DVDateInit();
        string GetData();
        string GetPinName();
        string GetCaseName();
        int 获取测试顺序();
        时序 获取测试时序();
        OpenorClose 获取Case1();
        OpenorClose 获取Case2();
        OpenorClose 获取Case3();
        OpenorClose 获取Case4();
        bool 获取测试时序状态1();
        bool 获取测试时序状态2();
        bool 获取测试时序状态3();
        bool 获取测试时序状态4();
        bool 获取测试时序状态5();
        bool 获取测试时序状态6();
        bool 获取测试时序状态7();
        bool 获取测试时序状态8();
        bool 获取测试时序状态9();
        bool 获取测试时序状态10();
        bool 获取测试时序状态11();
        bool 获取测试时序状态12();
        bool 获取测试时序瞬时状态1();
        bool 获取测试时序瞬时状态2();
        void Set测试时序状态1();
        void Set测试时序状态2();
        void Set测试时序状态3();
        void Set测试时序状态4();
        void Set测试时序状态5();
        void Set测试时序状态6();
        void Set测试时序状态7();
        void Set测试时序状态8();
        void Set测试时序状态9();
        void Set测试时序状态10();
        void Set测试时序状态11();
        void Set测试时序状态12();
        void Set测试时序瞬时状态1();
        void Set测试时序瞬时状态2();
        void Reset测试时序状态1();
        void Reset测试时序状态2();
        void Reset测试时序状态3();
        void Reset测试时序状态4();
        void Reset测试时序状态5();
        void Reset测试时序状态6();
        void Reset测试时序状态7();
        void Reset测试时序状态8();
        void Reset测试时序状态9();
        void Reset测试时序状态10();
        void Reset测试时序状态11();
        void Reset测试时序状态12();
        void Reset测试时序瞬时状态1();
        void Reset测试时序瞬时状态2();
        void ResetTestStep();
        void SetCaseName(string name);
        void Start(负载状态 OpenorClose, 测试方式 TestMethod);
        void SetCondition(object value1, object value2, object value3);
        void Set测试顺序(int value);
        void Set测试时序(时序 value);
        void SetCase(OpenorClose cs1, OpenorClose cs2, OpenorClose cs3, OpenorClose cs4);
        void Set测试项报文开关(string value);

        void Set循环发送报文开关(string value);

        void Set负载开启报文开关(string value);

        void Set负载关闭报文开关(string value);
        
        void Set停止报文开关(string value);
        void SetHL配置(string value1,string value2);
        void Set负载开启报文延时开关(string value1, string value2);
        //void Set测试数据来源(数据来源类型 value);
        void Set判断数据类型(数据类型 value);
        void Set测试结果对比规则(对比规则 value);
        void Set产品引脚(string value);
        void Set被测数据(string value);
        void SetBoard(object value);
        string GetCondition();
        void Init();
        void CapInit();
        void ResetInitStartFlag();
        void ResetInitStopFlag();
        void SetInitStartFlag();
        void SetInitStopFlag();
        ConcurrentQueue<string> GetCommunicataData();

        ConcurrentQueue<string> GetTestLog();
    }
    public class 测试项基类 : ICaseInterface 
    {
        public ScanLog Log = new ScanLog();
        public int DelayTime = 15000;
        public bool 时间段1标志 = false;//长时1
        public bool 时间段2标志 = false;
        public bool 时间段3标志 = false;
        public bool 时间段4标志 = false;//长时2
        public bool 时间段5标志 = false;
        public bool 时间段6标志 = false;
        public bool 时间段7标志 = false;
        public bool 时间段8标志 = false;
        public bool 时间段9标志 = false;
        public bool 时间段10标志 = false;
        public bool 时间段11标志 = false;
        public bool 时间段12标志 = false;
        public bool 时间段瞬时1标志 = false;
        public bool 时间段瞬时2标志 = false;
        public byte TestStep = 0;
        public static int ILD21Sum = 0;
        public static int ILD22Sum = 0;
        public static bool 测试停止 { set; get; } = true;
        public 测试结果 结果 { set; get; } = 测试结果.未开始;
        public 测试结果 历史结果 { set; get; } = 测试结果.未开始;
        public 测试结果 DV历史结果 { set; get; } = 测试结果.未开始;
        public string DV历史结果数据 { set; get; } = "";
        public string 测试数据 { get; set; }
        public object 结果判断数据1 { get; set; }
        public object 结果判断数据2 { get; set; }
        public object 结果判断数据3 { get; set; }
        public object 结果判断数据4 { get; set; }
        public object 结果判断数据5 { get; set; }
        public object 结果判断数据6 { get; set; }
        public object 结果判断数据7 { get; set; }
        public object 结果判断数据8 { get; set; }
        public object 结果判断数据9 { get; set; }
        public object 结果判断数据10 { get; set; }
        public object 结果判断数据11 { get; set; }
        public object 结果判断数据12 { get; set; }
        public string 测试项名 { get; set; }
        public int 测试顺序 { get; set; }
        public 时序 测试时序 { get; set; }
        public OpenorClose Case1 { get; set; }
        public OpenorClose Case2 { get; set; }
        public OpenorClose Case3 { get; set; }
        public OpenorClose Case4 { get; set; }
        public string 测试项报文开关 { get; set; }
        public string 循环发送报文开关 { get; set; }//
        public string 负载开启报文开关 { get; set; }//
        public string 负载关闭报文开关 { get; set; }//
        public string 停止报文开关 { get; set; }//
        public string 产品引脚 { get; set; }
        public string 被测数据 { get; set; }
        public string HL开启 { get; set; }
        public string HL关闭 { get; set; }
        public string 负载开启报文延时开关 { get; set; }
        public string 负载关闭报文延时开关 { get; set; }
        public bool 负载开启保存标志 { get; set; }//
        public bool 负载关闭保存标志 { get; set; }//
        /// <summary>
        /// 加载测试项时，从板子列表中获取的缓存
        /// </summary>
        public object GetBoardSelect { get; set; }
      //  public 数据来源类型 测试数据来源 { get; set; }
        public 数据类型 判断数据类型 { get; set; }
        public 对比规则 测试结果对比规则 { get; set; }
        public bool InitFlagStart = false;
        public bool InitFlagStop = false;
        public bool LastInitFlagStart = false;
        public bool LastInitFlagStop = false;
        public DateTime StartTime;
        public ConcurrentQueue<string> 报文记录 { get; set; } = new ConcurrentQueue<string>();
        public ConcurrentQueue<string> 测试记录 { get; set; } = new ConcurrentQueue<string>();
        public List<测试项报文开关配置> 测试项报文开关配置List = new List<测试项报文开关配置>();
        public List<线圈> 即将写入的M = new List<线圈>();
        public int 超时时间 { get; set; } = 30;
        public void ResetTestStep()
        {
            TestStep = 0;
        }
        public void Set负载开启报文延时开关(string value1, string value2)
        {
            负载开启报文延时开关 = value1;
            负载关闭报文延时开关 = value2;
        }
        public void  Set结果(测试结果 value)
        {
            结果 = value;
        }
        public void Set历史结果(测试结果 value)
        {
            DV历史结果 = value;
        }
        public void Set历史结果数据(string value)
        {
            DV历史结果数据 = value;
        }
        public bool 获取测试时序状态1()
        {
            return 时间段1标志;
        }
        public bool 获取测试时序状态2()
        {
            return 时间段2标志;
        }
        public bool 获取测试时序状态3()
        {
            return 时间段3标志;
        }
        public bool 获取测试时序状态4()
        {
            return 时间段4标志;
        }
        public bool 获取测试时序状态5()
        {
            return 时间段5标志;
        }
        public bool 获取测试时序状态6()
        {
            return 时间段6标志;
        }
        public bool 获取测试时序状态7()
        {
            return 时间段7标志;
        }
        public bool 获取测试时序状态8()
        {
            return 时间段8标志;
        }
        public bool 获取测试时序状态9()
        {
            return 时间段9标志;
        }
        public bool 获取测试时序状态10()
        {
            return 时间段10标志;
        }
        public bool 获取测试时序状态11()
        {
            return 时间段11标志;
        }
        public bool 获取测试时序状态12()
        {
            return 时间段12标志;
        }
        public bool 获取测试时序瞬时状态1()
        {
            return 时间段瞬时1标志;
        }
        public bool 获取测试时序瞬时状态2()
        {
            return 时间段瞬时2标志;
        }
        public  void Set测试时序状态1()
        {
            时间段1标志 =true;
        }
        public void Set测试时序状态2()
        {
            时间段2标志 = true;
        }
        public void Set测试时序状态3()
        {
            时间段3标志 = true;
        }
        public void Set测试时序状态4()
        {
            时间段4标志 = true;
        }
        public void Set测试时序状态5()
        {
            时间段5标志 = true;
        }
        public void Set测试时序状态6()
        {
            时间段6标志 = true;
        }
        public void Set测试时序状态7()
        {
            时间段7标志 = true;
        }
        public void Set测试时序状态8()
        {
            时间段8标志 = true;
        }
        public void Set测试时序状态9()
        {
            时间段9标志 = true;
        }
        public void Set测试时序状态10()
        {
            时间段10标志 = true;
        }
        public void Set测试时序状态11()
        {
            时间段11标志 = true;
        }
        public void Set测试时序状态12()
        {
            时间段12标志 = true;
        }
        public void Set测试时序瞬时状态1()
        {
            时间段瞬时1标志 = true;
        }
        public void Set测试时序瞬时状态2()
        {
            时间段瞬时2标志 = true;
        }
        public void Reset测试时序状态1()
        {
            时间段1标志 = false;
        }
        public void Reset测试时序状态2()
        {
            时间段2标志 = false;
        }
        public void Reset测试时序状态3()
        {
            时间段3标志 = false;
        }
        public void Reset测试时序状态4()
        {
            时间段4标志 = false;
        }
        public void Reset测试时序状态5()
        {
            时间段5标志 = false;
        }
        public void Reset测试时序状态6()
        {
            时间段6标志 = false;
        }
        public void Reset测试时序状态7()
        {
            时间段7标志 = false;
        }
        public void Reset测试时序状态8()
        {
            时间段8标志 = false;
        }
        public void Reset测试时序状态9()
        {
            时间段9标志 = false;
        }
        public void Reset测试时序状态10()
        {
            时间段10标志 = false;
        }
        public void Reset测试时序状态11()
        {
            时间段11标志 = false;
        }
        public void Reset测试时序状态12()
        {
            时间段12标志 = false;
        }
        public void Reset测试时序瞬时状态1()
        {
            时间段瞬时1标志 = false;
        }
        public void Reset测试时序瞬时状态2()
        {
            时间段瞬时2标志 = false;
        }
        public byte StepTime(DateTime time,int delaytime,byte step )
        {
            if((DateTime.Now - time).TotalMilliseconds > delaytime)
            {
                return ++step;
            }
            return 0;
        }
        public virtual void SetInitStopFlag()
        {
            InitFlagStop = true;
        }
        public virtual void ResetInitStopFlag()
        {
            InitFlagStop = false;
        }
        public virtual void SetInitStartFlag()
        {
            InitFlagStart = true;
        }
        public virtual void ResetInitStartFlag()
        {
            InitFlagStart = false;
        }
        public virtual string GetData()
        {
            return 测试数据;
        }
        public virtual string GetPinName()
        {
            return 产品引脚;
        }
        public virtual 测试结果 GetState()
        {
            return 结果;
        }
        public virtual 测试结果 GetHisToryState()
        {
            return 历史结果;
        }
        public virtual 测试结果 GetDVHisToryState()
        {
            return DV历史结果;
        }
        public virtual string GetDVHisToryDate()
        {
            return DV历史结果数据;
        }
        public virtual void DVResultInit()
        {
            DV历史结果 = 测试结果.未开始;
        }
        public virtual void DVDateInit()
        {
            DV历史结果数据 = "";
        }
        public virtual void Init()
        {
            结果 = 测试结果.未开始;
            历史结果 = 测试结果.未开始;
            测试数据 = "";
        }
        public virtual void CapInit()
        {

        }
        public virtual int 获取测试顺序()
        {
            return 测试顺序;
        }
        public virtual 时序 获取测试时序()
        {
            return 测试时序;
        }
        public virtual OpenorClose 获取Case1()
        {
            return Case1;
        }
        public virtual OpenorClose 获取Case2()
        {
            return Case2;
        }
        public virtual OpenorClose 获取Case3()
        {
            return Case3;
        }
        public virtual OpenorClose 获取Case4()
        {
            return Case4;
        }
        public virtual void SetCondition(object value1, object value2, object value3)
        {
            结果判断数据1 = value1;
            结果判断数据2 = value2;
            结果判断数据3 = value3;
        }
        public virtual void SetBoard(object value)
        {
            GetBoardSelect = value;
        }
        public virtual void Set测试顺序(int value)
        {
            测试顺序 = value;
        }
        public virtual void  Set测试时序(时序 value)
        {
            测试时序 = value;
        }
        public virtual void  SetCase(OpenorClose cs1, OpenorClose cs2, OpenorClose cs3, OpenorClose cs4)
        {
            Case1 = cs1; Case2 = cs2; Case3 = cs3; Case4 = cs4;
        }
        public virtual void Set测试项报文开关(string value)
        {
            测试项报文开关 = value;
        }
        public virtual void Set循环发送报文开关(string value)
        {
            循环发送报文开关 = value;
        }
        public virtual void Set负载开启报文开关(string value)
        {
            负载开启报文开关 = value;
        }
        public virtual void Set负载关闭报文开关(string value)
        {
            负载关闭报文开关 = value;
        }
        public virtual void Set停止报文开关(string value)
        {
            停止报文开关 = value;
        }
        public virtual void SetHL配置(string value1, string value2)
        {
            HL开启 = value1;
            HL关闭 = value2;
        }
        //public virtual void Set测试数据来源(数据来源类型 value)
        //{
        //    测试数据来源 = value;
        //}
        public virtual void Set判断数据类型(数据类型 value)
        {
            判断数据类型 = value;
        }
        public virtual void Set测试结果对比规则(对比规则 value)
        {
            测试结果对比规则 = value;
        }
        public virtual void Set产品引脚(string value)
        {
            产品引脚 = value;
        }
        public virtual void Set被测数据(string value)
        {
            被测数据 = value;
        }
        public virtual string GetCondition()
        { 
            return "";
          //  return 结果判断数据;
        }

        public virtual void Start(负载状态 OpenorClose, 测试方式 TestMethod)
        {
            结果 = 测试结果.合格;
        }
        public virtual string GetCaseName()
        {
            return 测试项名;
        }
        public virtual void SetCaseName(string name)
        {
            测试项名 = name;
        }
        public virtual ConcurrentQueue<string> GetCommunicataData()
        {
            return 报文记录;
        }
        public virtual ConcurrentQueue<string> GetTestLog()
        {
            return 测试记录;
        }
        public 负载状态 负载开启标志 = 负载状态.关闭;
        public void OpenorCloseMethod(负载状态 bl)
        {
            if (负载开启标志 != bl)
            {
                String[] Mstring;
                bool PwmFlag = false;
                if (string.IsNullOrEmpty(测试项报文开关))
                {
                    Mstring = null;
                }
                else
                    Mstring = 测试项报文开关.Split(',');//前面开启 后面关闭
                Type myClassType = typeof(测试项基类);
                // 获取MyIntField字段的FieldInfo对象
                FieldInfo intFieldInfo = myClassType.GetField("GetBoardSelect");
                // 获取字段类型
                Type intFieldType = intFieldInfo.FieldType;
                //if (intFieldType == typeof(PWM))
                //{
                //    PwmFlag = true;
                //}

                //switch (bl)
                //{
                //    case 负载状态.开启:
                //        if (Mstring != null)
                //        {
                //            ushort MKey = ushort.Parse(Mstring[0].Remove('M'));
                //            CAEA008TCPModbus.Instance._线圈管理器.插入线圈(MKey,1);
                //        }
                //        if (PwmFlag)
                //        {
                //            AT03TCPCommunicate.WriteTcpstack.Enqueue(new AT03TCPCommunicate.WriteTcpParameter() { _WriteFunction = AT03TCPCommunicate.WriteFunction.WPwm, pinNum = ((PWM)GetBoardSelect).产品引脚, GetBoard = GetBoardSelect });
                //        }
                //        break;
                //    case 负载状态.关闭:
                //        if (Mstring != null)
                //        {
                //            ushort MKey = ushort.Parse(Mstring[1].Remove('M'));
                //         //   CAEA008TCPModbus.Instance._线圈管理器.MWritestack.Enqueue(new 线圈(MKey, 状态.关闭));
                //            CAEA008TCPModbus.Instance._线圈管理器.插入线圈(MKey, 0);
                //        }
                //        if (PwmFlag)
                //        {
                //            AT03TCPCommunicate.WriteTcpstack.Enqueue(new AT03TCPCommunicate.WriteTcpParameter() { _WriteFunction = AT03TCPCommunicate.WriteFunction.WPwm, pinNum = ((PWM)GetBoardSelect).产品引脚, GetBoard = GetBoardSelect });
                //        }
                //        break;
                //}
                负载开启标志 = bl;
            }
        }
        public void ParseWriteOrRead(string input)
        {
            //   string input = "(R-D300)(W-M100)(W-D200-30)";
            string pattern = @"\((W|R)-(M|D)(\d*)-(\d*)?\)";

            // 使用正则表达式匹配所有操作
            MatchCollection matches = Regex.Matches(input, pattern);

            foreach (Match match in matches)
            {
                // 提取操作类型（写入W或读取R）
                string operation = match.Groups[1].Value;

                // 提取设备类型（M或D）
                string deviceType = match.Groups[2].Value;

                // 提取设备编号
                string deviceId = match.Groups[3].Value;

                // 提取写入值（如果有）
                string writeValue = match.Groups[4].Success ? match.Groups[4].Value.Trim('-') : null;

                // 根据操作类型和设备类型输出相应的操作
                if (operation == "W")
                {
                    if (writeValue != null)
                    {
                        测试项报文开关配置List.Add(new 测试项报文开关配置 { 操作名称 = 操作.写, 写入类型名称 = deviceType, 写入地址 = Convert.ToUInt16(deviceId), 写入值 = Convert.ToUInt16(writeValue) });
                    }
                    else
                    {
                        测试项报文开关配置List.Add(new 测试项报文开关配置 { 操作名称 = 操作.写, 写入类型名称 = deviceType, 写入地址 = Convert.ToUInt16(deviceId) });
                    }
                }
                else if (operation == "R")
                {
                    测试项报文开关配置List.Add(new 测试项报文开关配置 { 操作名称 = 操作.读, 写入类型名称 = deviceType, 写入地址 = Convert.ToUInt16(deviceId) });
                }
            }
        }
        public List<Tuple<string, ushort>> ParseWrite(string input)
        {
            //   string input = "(R-D300)(W-M100)(W-D200-30)";
            string pattern = @"\((W|R)-(M|D)(\d*)-(\d*)?\)";
            List<Tuple<string, ushort>> tuples = new List<Tuple<string, ushort>>();
            // 使用正则表达式匹配所有操作
            MatchCollection matches = Regex.Matches(input, pattern);

            foreach (Match match in matches)
            {
                // 提取操作类型（写入W或读取R）
                string operation = match.Groups[1].Value;

                // 提取设备类型（M或D）
                string deviceType = match.Groups[2].Value;

                // 提取设备编号
                string deviceId = match.Groups[3].Value;

                // 提取写入值（如果有）
                string writeValue = match.Groups[4].Success ? match.Groups[4].Value.Trim('-') : null;

                // 根据操作类型和设备类型输出相应的操作
                if (operation == "W")
                {
                    if (writeValue != null)
                    {
                        tuples.Add(new Tuple<string, ushort>(deviceType + deviceId, Convert.ToUInt16(writeValue)));
                    }
                }
            }
            return tuples;
        }
        public List<(string IP, int Address, int Value)> ParseWriteD(string input)
        {
          //  var matches = Regex.Matches(input, @"$(?<ip>[^&]+)&(?<addr>\d+)-(?<value>\d+)$");
            var results = new List<(string IP, int Address, int Value)>();

            //foreach (Match match in matches)
            //{
            //    string ip = match.Groups["ip"].Value;
            //    int address = int.Parse(match.Groups["addr"].Value);
            //    int value = int.Parse(match.Groups["value"].Value);
            //    results.Add((ip, address, value));
            //}
            // 分割每个条目
            string[] entries = input.Split(new[] { ")(" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string entry in entries)
            {
                // 清理括号
                string cleanEntry = entry.Trim('(', ')');

                // 分割IP和后面的部分
                string[] ipAndRest = cleanEntry.Split('&');
                string ip = ipAndRest[0];

                // 分割地址和值
                string[] addressAndValue = ipAndRest[1].Split('-');
                int address = addressAndValue[0].ToInt32();
                int value = addressAndValue[1].ToInt32();
                results.Add((ip, address, value));
              //  Console.WriteLine($"IP: {ip}, 地址: {address}, 值: {value}");
            }
            return results;
        }
        public List<List<string>> ParseWriteCan(string input)
        {
            var matches = Regex.Matches(input, @"\{([^}]+)\}");
            List<string> objectStrings = new List<string>();
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    // 移除首尾的 {}，只保留内部内容
                    objectStrings.Add(match.Groups[1].Value);
                }
            }
            List<List<string>> allResults = new List<List<string>>();
            foreach (var obj in objectStrings)
            {
                string[] parts = obj.Split(';');
                List<string> result = new List<string>();
                foreach (string part in parts)
                {
                    if (!part.Contains(":"))
                    {
                        // 处理字符串部分（如 "UDP服务器" 或 "发送节点名称"）
                        string value = part.Trim().Trim('"', '“', '”'); // 移除引号
                        result.Add(value);
                    }
                    else
                    {
                        // 处理键值对部分（如 Start:8）
                        string[] kv = part.Split(new[] { ':' }, 2);
                        result.Add(kv[1].Trim());
                    }
                }

                allResults.Add(result);
            }
            return allResults;
        }
        public enum HighLowSideType { High, Low, Unknown }
        public class DeviceInfo
        {
            public HighLowSideType HighLowSide { get; set; }
            public int AdcPin { get; set; }
            public string Ip { get; set; }
            public int? Channel { get; set; }
            public List<(string UdpName, string SignalName)> Signals { get; } = new List<(string, string)>();
        }
        public DeviceInfo Parse(string input)
        {
            var info = new DeviceInfo();
            string[] parts = input.Split(',');

            // 解析 High/Low 边 (新需求)
            int startIndex = 0;
            if (parts.Length > 0 && (parts[0] == "H" || parts[0] == "L"))
            {
                info.HighLowSide = parts[0] == "H" ? HighLowSideType.High : HighLowSideType.Low;
                startIndex = 1; // 剩余部分从索引 1 开始
            }
            else
            {
                info.HighLowSide = HighLowSideType.Unknown; // 旧格式兼容
            }

            // 剩余需要解析的部分
            var remainingParts = parts.Skip(startIndex).ToArray();

            // 解析 ADC 引脚
            if (remainingParts.Length < 1 || !Regex.IsMatch(remainingParts[0], @"^ADC-\d+$"))
                throw new FormatException("Invalid ADC format");
            info.AdcPin = int.Parse(remainingParts[0].Substring(4));

            // 解析 IP & 通道号（可选）
            if (remainingParts.Length > 1)
            {
                if (TryParseIpAndChannel(remainingParts[1], out var ip, out var channel))
                {
                    info.Ip = ip;
                    info.Channel = channel;
                    ParseSignals(remainingParts.Skip(2).ToArray(), info.Signals);
                }
                else
                {
                    ParseSignals(remainingParts.Skip(1).ToArray(), info.Signals);
                }
            }

            return info;
        }
        private static bool TryParseIpAndChannel(string str, out string ip, out int channel)
        {
            ip = null;
            channel = 0;
            var parts = str.Split('&');

            if (parts.Length != 2) return false;
            if (!IPAddress.TryParse(parts[0], out _)) return false;
            if (!int.TryParse(parts[1], out channel)) return false;

            ip = parts[0];
            return true;
        }

        private static void ParseSignals(string[] signalParts, List<(string, string)> signals)
        {
            foreach (var part in signalParts)
            {
                var pair = part.Split('&');
                if (pair.Length != 2) continue;
                signals.Add((pair[0], pair[1]));
            }
        }
        public CANUDPClient FindClientByName(string targetName)
        {
            return WinForm.CANUDPClients[0];//.FirstOrDefault(c => c.Name == targetName);
        }
        public Communication.CAN.Message FindMessageByName(string targetName,string MessageName)
        {
            return (WinForm.CANUDPClients.FirstOrDefault(c => c.Name == targetName)).Messages.FirstOrDefault(d => d.Name == MessageName);
        }
        public ModbusTcpMaster FindmodbusTcpMasterByName(string targetName)
        {
            return WinForm.Caea008Groups.FirstOrDefault(c => c.Key == targetName).Value;
        }
    
        public float ObjToFloat(object o)
        {
            return (float)Convert.ToSingle(o);
        }
        public void FailDispose()
        {
            if (历史结果 == 测试结果.未开始)
            {
                DV历史结果数据 = 测试数据;
                DV历史结果 = 测试结果.不合格;
                历史结果 = 测试结果.不合格;
                Log.InsertLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + 测试项名 + "." + 测试数据 + "," + 结果);
                LogHelperError.Error(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + 测试项名 + "." + 测试数据 + "," + 结果);
                WinForm.本轮测试结果 = 测试结果.不合格;
            }
        }
        public void TimeInit()
        {
            时间段1标志 = false;//长时1
            时间段2标志 = false;
            时间段3标志 = false;
            时间段4标志 = false;//长时2
            时间段5标志 = false;
            时间段6标志 = false;
            时间段瞬时1标志 = false;
            时间段瞬时2标志 = false;
            负载开启保存标志 = false;
            负载关闭保存标志 = false;
            即将写入的M.Clear();
         }
        public void PassSave(string st)
        {
            LogHelper.Info(st);
        }
    }
    public class 电压读取 : 测试项基类
    {
        public ushort index = 0;
      //  public byte TestStep = 0;
        Rule rule = new Rule();
        public DateTime FailTime;
        public string UdpName;
        public string SignalName;
        public int FailSum = 0;
        public bool TwoTestFlag = false;
        public override void Init()
        {
            结果 = 测试结果.未开始;
            历史结果 = 测试结果.未开始;
            测试数据 = "";
            TestStep = 0;
            LastInitFlagStart = false;
            InitFlagStart = false;
            LastInitFlagStop = false;
            InitFlagStop  = false;
            TimeInit();
            //   ParseWriteOrRead(测试项报文开关);
            if (被测数据.Contains("ADC-"))
            {
                string[] parts = 被测数据.Split('-');
                if (parts.Length > 1)
                {
                    index = Convert.ToUInt16(parts[1]);
                }
            }
            else if (被测数据.Contains("D")&& !被测数据.Contains("MCU"))
            {
                Match match = Regex.Match(被测数据, @"\d+");
                if (match.Success)
                {
                    index = Convert.ToUInt16(match.Value);
                }
            }
            else if (被测数据.Contains("CAP-"))
            {
                string[] parts = 被测数据.Split('-');
                if (parts.Length > 1)
                {
                    index = Convert.ToUInt16(parts[1]);
                }
            }
            else if(被测数据.Contains("MCU"))//"MCU1&IAN01_AD"
            {
                string[] parts = 被测数据.Split(new[] { '&' }, 2);
                UdpName = parts[0];
                SignalName = parts.Length > 1 ? parts[1] : "";
            }
            else if (被测数据.Contains("192"))// ip
            {
                string[] parts = 被测数据.Split(new[] { '&' }, 2);
                UdpName = parts[0];
                SignalName = parts.Length > 1 ? parts[1] : "";
            }
            FailSum = 0;
        }
        public override void Start(负载状态 OpenorClose, 测试方式 TestMethod )
        {
            DateTime intotime = DateTime.Now;
            int 测试数据Temp = 0;
            try
            {
                if(TestStep <= 2)
                {
                    if (被测数据.Contains("D") && !被测数据.Contains("MCU"))
                    {
                        测试数据Temp = CAEA008TCPModbus.Instance.DList[index];
                        测试数据 = OpenorClose + 测试数据Temp.ToString();
                    }
                    else if (被测数据.Contains("MCU"))
                    {
                        测试数据Temp = (int)FindClientByName(UdpName).SignalDictionary[SignalName];//截去小数部分
                        测试数据 = OpenorClose + 测试数据Temp.ToString() + "mV";
                    }
                    else if (被测数据.Contains("192"))
                    {
                        if (SignalName == "Lin1")
                            测试数据Temp = (int)FindmodbusTcpMasterByName(UdpName).Lin1Value;//截去小数部分
                        else if (SignalName == "Lin2")
                            测试数据Temp = (int)FindmodbusTcpMasterByName(UdpName).Lin2Value;//截去小数部分
                        测试数据 = OpenorClose + 测试数据Temp.ToString();
                    }
                }
                
              //  string st = 测试数据;
            }
            catch (Exception e) 
            {

            }

          
            switch (TestStep)
            {
                case 0:
                    结果 = 测试结果.测试中;

                    //if (InitFlagStart == false && !string.IsNullOrEmpty(负载开启报文开关) && OpenorClose == 负载状态.开启)
                    //{
                    //  //  { "UDP服务器";“发送节点名称”; Start: 8; Lenth: 8; value: 100}
                    //    var 负载开启报文开关s = ParseWriteCan(负载开启报文开关);
                    //    for (int i = 0; i < 负载开启报文开关s.Count; i++)
                    //    {
                    //        var result = 负载开启报文开关s[i];
                    //        FindClientByName(result[0]).ModifyBits(result[1],Convert.ToUInt32(result[2]),Convert.ToByte(result[3]), Convert.ToInt32(result[4]));
                    //    }
                    //    for (int i = 0; i < 负载开启报文开关s.Count; i++)
                    //    {
                    //        var result = 负载开启报文开关s[i];
                    //        FindClientByName(result[0]).UpSendflag(result[1]);
                    //    }
                      
                    //    InitFlagStart = true;
                    //    LastInitFlagStart = InitFlagStart;
                    //}
                    //if (InitFlagStop == false && !string.IsNullOrEmpty(负载关闭报文开关) && OpenorClose == 负载状态.关闭)
                    //{
                    //    var 负载关闭报文开关s = ParseWriteCan(负载关闭报文开关);
                    //    for (int i = 0; i < 负载关闭报文开关s.Count; i++)
                    //    {
                    //        var result = 负载关闭报文开关s[i];
                    //        FindClientByName(result[0]).ModifyBits(result[1], Convert.ToUInt32(result[2]), Convert.ToByte(result[3]), Convert.ToInt32(result[4]));
                    //    }
                    //    for (int i = 0; i < 负载关闭报文开关s.Count; i++)
                    //    {
                    //        var result = 负载关闭报文开关s[i];
                    //        FindClientByName(result[0]).UpSendflag(result[1]);
                    //    }
                    //    InitFlagStop = true;
                    //    LastInitFlagStop = InitFlagStop;
                    //}
                    Log.InsertLog(测试项名 + OpenorClose + "开始测试...");
                    StartTime = DateTime.Now;
                    FailTime = DateTime.Now;
                    TestStep++;
                    break;
                case 1:
                    byte StepTemp = StepTime(StartTime, 500, TestStep);
                    if (StepTemp != 0)
                    {
                        TestStep = StepTemp;
                        StartTime = DateTime.Now;
                    }
                    break;
                case 2:
                    int errorvalue = Convert.ToInt32(结果判断数据5);
                    if (OpenorClose == 负载状态.开启)
                    {
                        
       
                        if (rule.IsMatch(测试结果对比规则, 测试数据Temp, Convert.ToInt32(结果判断数据1) - errorvalue, Convert.ToInt32(结果判断数据2) + errorvalue))
                        {
                            if (负载开启保存标志 == false)
                            {
                                PassSave(DateTime.Now.ToString(OpenorClose+"yyyy-MM-dd HH:mm:ss.fff") + "," + 测试项名 + "." + 测试数据 + "," + 结果);
                                负载开启保存标志 = true;
                            }
                            int value = 测试数据Temp;
                            if (测试结果对比规则 == 对比规则.范围 )
                            {
                                if (测试数据Temp < Convert.ToInt32(结果判断数据1))
                                {

                                    if (测试数据Temp + errorvalue > Convert.ToInt32(结果判断数据2))
                                    {
                                        value = Convert.ToInt32(结果判断数据2);
                                    }
                                    else
                                    {
                                        value = 测试数据Temp + errorvalue;
                                    }
                                }
                                else if (测试数据Temp > Convert.ToInt32(结果判断数据2))
                                {
                                    if (测试数据Temp - errorvalue < Convert.ToInt32(结果判断数据1))
                                    {
                                        value = Convert.ToInt32(结果判断数据1);
                                    }
                                    else
                                    {
                                        value = 测试数据Temp - errorvalue;
                                    }
                                }
                                else
                                    value = 测试数据Temp;
                            }
                            else
                                value = 测试数据Temp;
                           if (被测数据.Contains("D") && !被测数据.Contains("MCU"))
                            {
                                测试数据Temp = CAEA008TCPModbus.Instance.DList[index];
                                测试数据 = OpenorClose + value.ToString();
                            }
                            else if (被测数据.Contains("MCU"))
                            {
                                测试数据Temp = (int)FindClientByName(UdpName).SignalDictionary[SignalName];//截去小数部分
                                测试数据 = OpenorClose + value.ToString() + "mV";
                            }
                            else if (被测数据.Contains("192"))
                            {
                                if (SignalName == "Lin1")
                                    测试数据Temp = (int)FindmodbusTcpMasterByName(UdpName).Lin1Value;//截去小数部分
                                else if (SignalName == "Lin2")
                                    测试数据Temp = (int)FindmodbusTcpMasterByName(UdpName).Lin2Value;//截去小数部分
                                测试数据 = OpenorClose + value.ToString();
                            }
                            TestStep++;
                            结果 = 测试结果.合格;
                            
                            FailTime = DateTime.Now;
                            break;
                        }
                        if ((DateTime.Now - FailTime).TotalMilliseconds > 5000)
                        {
                            if (FailSum < 5)
                            {
                                InitFlagStart = false;
                                FailSum++;
                                TestStep = 0;
                                break;
                            }
                            结果 = 测试结果.不合格;
                            FailDispose();
                        }

                    }
                    else
                    {
                        if (rule.IsMatch(测试结果对比规则, 测试数据Temp, Convert.ToInt32(结果判断数据3) - errorvalue, Convert.ToInt32(结果判断数据4) + errorvalue))
                        {
                            if (负载关闭保存标志 == false)
                            {
                                PassSave(DateTime.Now.ToString(OpenorClose + "yyyy-MM-dd HH:mm:ss.fff") + "," + 测试项名 + "." + 测试数据 + "," + 结果);
                                负载关闭保存标志 = true;
                            }
                            TestStep++;
                            结果 = 测试结果.合格;
                             if (被测数据.Contains("D") && !被测数据.Contains("MCU"))
                            {
                                测试数据Temp = CAEA008TCPModbus.Instance.DList[index];
                                测试数据 = OpenorClose + 测试数据Temp.ToString();
                            }
                            else if (被测数据.Contains("MCU"))
                            {
                                测试数据Temp = (int)FindClientByName(UdpName).SignalDictionary[SignalName];//截去小数部分
                                测试数据 = OpenorClose + 测试数据Temp.ToString() + "mV";
                            }
                            else if (被测数据.Contains("192"))
                            {
                                if (SignalName == "Lin1")
                                    测试数据Temp = (int)FindmodbusTcpMasterByName(UdpName).Lin1Value;//截去小数部分
                                else if (SignalName == "Lin2")
                                    测试数据Temp = (int)FindmodbusTcpMasterByName(UdpName).Lin2Value;//截去小数部分
                                测试数据 = OpenorClose + 测试数据Temp.ToString();
                            }
                            FailTime = DateTime.Now;
                        }
                        if ((DateTime.Now - FailTime).TotalMilliseconds > 5000)
                        {
                            if (FailSum < 5)
                            {
                                InitFlagStop = false;
                                FailSum++;
                                TestStep = 0;
                                break;
                            }
                            结果 = 测试结果.不合格;
                            FailDispose();
                        }
                    }

                    break;
            }
            LastInitFlagStart = InitFlagStart;
            LastInitFlagStop = InitFlagStop;
            
            double endtime = (DateTime.Now - intotime).TotalMilliseconds;
         //   Console.WriteLine(测试项名 + "结束时间" + endtime);
        }
     
    }

    public class CANID读取 : 测试项基类
    {
        public ushort index = 0;
        //  public byte TestStep = 0;
        Rule rule = new Rule();
        public DateTime FailTime;
        public string UdpName;
        public string CANIDName;
        public int FailSum = 0;
        public override void Init()
        {
            结果 = 测试结果.未开始;
            历史结果 = 测试结果.未开始;
            测试数据 = "";
            TestStep = 0;
            LastInitFlagStart = false;
            InitFlagStart = false;
            LastInitFlagStop = false;
            InitFlagStop = false;
            TimeInit();
            //   ParseWriteOrRead(测试项报文开关);
            if (被测数据.Contains("MCU"))//"MCU1&IAN01_AD"
            {
                string[] parts = 被测数据.Split(new[] { '&' }, 2);
                UdpName = parts[0];
                CANIDName = parts.Length > 1 ? parts[1] : "";
            }
            FailSum = 0;
        }
        public override void Start(负载状态 OpenorClose, 测试方式 TestMethod)
        {
            DateTime intotime = DateTime.Now;
            int 测试数据Temp = 0;
            switch (TestStep)
            {
                case 0:
                    结果 = 测试结果.测试中;

                    StartTime = DateTime.Now;
                    TestStep++;
                    break;
                case 1:
                    byte StepTemp = StepTime(StartTime, 500, TestStep);
                    if (StepTemp != 0)
                    {
                        TestStep = StepTemp;
                        StartTime = DateTime.Now;
                    }
                    break;
                case 2:
                    try
                    {
                        if (被测数据.Contains("MCU"))
                        {
                            if(FindClientByName(UdpName)!=null && CANUDPClient.CanIdReadTime.ContainsKey(CANIDName))
                            测试数据Temp = Convert.ToInt32((DateTime.Now - CANUDPClient.CanIdReadTime[CANIDName]).TotalMilliseconds);//截去小数部分
                            else
                            {
                                测试数据 = OpenorClose + CANIDName /*+ " :" + 测试数据Temp.ToString() + "ms"*/;
                                结果 = 测试结果.不合格;
                                break;
                            }
                            测试数据 = OpenorClose + CANIDName /*+ " :" + 测试数据Temp.ToString() + "ms"*/;
                        }
                    }
                    catch (Exception ex)
                    {
                        测试数据 = OpenorClose + CANIDName + ex.Message;
                        结果 = 测试结果.不合格;
                        FailDispose();
                    }
                   
                  //  if (OpenorClose == 负载状态.开启)
                    {

                        if (rule.IsMatch(测试结果对比规则, 测试数据Temp, Convert.ToInt32(结果判断数据1), Convert.ToInt32(结果判断数据2)))
                        {
                            if (负载开启保存标志 == false)
                            {
                                PassSave(DateTime.Now.ToString(OpenorClose + "yyyy-MM-dd HH:mm:ss.fff") + "," + 测试项名 + "." + 测试数据 + "," + 结果);
                                负载开启保存标志 = true;
                            }
                            结果 = 测试结果.合格;
                            FailTime = DateTime.Now;
                        }
                        if ((DateTime.Now - FailTime).TotalMilliseconds > 5000)
                        {
                            if (FailSum < 2)
                            {
                                FailSum++;
                                TestStep = 0;
                                break;
                            }
                            结果 = 测试结果.不合格;
                            FailDispose();
                        }

                    }


                    break;
            }
            LastInitFlagStart = InitFlagStart;
            LastInitFlagStop = InitFlagStop;

            double endtime = (DateTime.Now - intotime).TotalMilliseconds;
            //   Console.WriteLine(测试项名 + "结束时间" + endtime);
        }

    }
    public class Can读取  : 测试项基类
    {
        public ushort index = 0;
        //  public byte TestStep = 0;
        Rule rule = new Rule();
        public DateTime FailTime;
        public string UdpName;
        public string SignalName;
        public int FailSum = 0;
        public CANSelectType CanType;
        public uint CanId;
        public byte[] SendUds;

        public override void Init()
        {
            结果 = 测试结果.未开始;
            历史结果 = 测试结果.未开始;
            测试数据 = "";
            TestStep = 0;
            LastInitFlagStart = false;
            InitFlagStart = false;
            LastInitFlagStop = false;
            InitFlagStop = false;
            TimeInit();
            //被测数据 = "22 F1 90" 
            //(结果判断数据1) == 显示格式"
            //(结果判断数据2) == 对比值

            // 3. 解析 byte[]

            SendUds = 被测数据
                .Split(' ')
                .Select(s => Convert.ToByte(s, 16))
                .ToArray();

            //   ParseWriteOrRead(测试项报文开关);
            //if (被测数据.Contains("MCU"))//"MCU1&IAN01_AD"
            //{
            //    string[] parts = 被测数据.Split(new[] { '&' }, 2);
            //    UdpName = parts[0];
            //    SignalName = parts.Length > 1 ? parts[1] : "";
            //}
            FailSum = 0;
        }
        public override void Start(负载状态 OpenorClose, 测试方式 TestMethod)
        {
            DateTime intotime = DateTime.Now;
            string 测试数据Temp = "";

            switch (TestStep)
            {
                case 0:
                    byte[] resp;
                    bool ok = FindClientByName(UdpName).TrySendUdsAndWait(
                        frameType: FrameType.ExtendedFrame,
                        requestCanId: Convert.ToUInt32(WinForm.CANSendID.Replace("0x", "").Trim(), 16),              // 你的诊断请求ID
                        sendType: SendType.Diagnosis,
                        canSelectType: WinForm.CANType,
                        requestPayload: SendUds,//new byte[] { 0x22, 0xF1, 0x90 }, // 读DID F190
                        expectedPositiveSid: (byte)(SendUds[0] + 0x40),// 0x62,             // 0x22正响应是0x62
                        expectedPositiveSubFunction: null,     // 0x22没有subfunction，传null
                        responsePayload: out resp,
                        negativeResponseCode: out byte nrc,
                        error: out string err,
                        p2TimeoutMs: 500,
                        p2StarTimeoutMs: 5000,
                        expectedResponseCanId: Convert.ToUInt32(WinForm.CANRevID.Replace("0x", "").Trim(), 16)// 0x14DAF140      // 不确定可传null
                    );

                    if (!ok)
                    {
                       Log.InsertLog($" {err}");
                        结果 = 测试结果.不合格;
                    }
                    else
                    {
                        string date = "";
                        // 响应格式: 62 DID_H DID_L DATA...
                        byte[] didData = resp.Length >= 3 ? resp.Skip(3).ToArray() : Array.Empty<byte>();
                      
                        if(Convert.ToString(结果判断数据1) == "ASCII")
                        {
                            date = Encoding.ASCII.GetString(didData);
                            测试数据 = Encoding.ASCII.GetString(didData) + " " + (BitConverter.ToString(didData));
                            if((Convert.ToString(结果判断数据2) == "追溯码"))
                            {
                                测试数据 = Encoding.ASCII.GetString(didData);
                            }
                        }
                          
                        if(Convert.ToString(结果判断数据1) == "HEX")
                        {
                            byte[] bytes = didData.ToArray();
                            string result = BitConverter.ToString(bytes).Replace("-", "");
                            date = result;
                            测试数据 = result + " " + (BitConverter.ToString(didData));
                            if ((Convert.ToString(结果判断数据2) == "ECUID"))
                            {
                                测试数据 = result;
                            }
                        }
                        if (Convert.ToString(结果判断数据1) == "DEC")
                        {
                            byte[] bytes = didData.ToArray();
                            Array.Reverse(bytes);
                            uint result = BitConverter.ToUInt32(bytes, 0);
                            date = result.ToString();
                            测试数据 = result.ToString() + " " + (BitConverter.ToString(didData));
                        }
                        if (!(Convert.ToString(结果判断数据2) is null) && (Convert.ToString(结果判断数据2) == "追溯码"))
                        {
                            WinForm.追溯码 = 测试数据;
                            结果 = 测试结果.合格;
                        }
                        if (!(Convert.ToString(结果判断数据2) is null) &&  Convert.ToString(结果判断数据2) == "ECUID")
                        {
                            结果 = 测试结果.合格;
                        }
                        if (!(Convert.ToString(结果判断数据3) is null) && Convert.ToString(结果判断数据3) == "零件号")
                            WinForm.零件号 = 测试数据;
                        if (!(Convert.ToString(结果判断数据2) is null) && Convert.ToString(结果判断数据2) != "追溯码" && Convert.ToString(结果判断数据2) != "ECUID")
                        {
                            if(Convert.ToString(结果判断数据2)  == date)
                            {
                                结果 = 测试结果.合格;
                            }else
                            {
                                结果 = 测试结果.不合格;
                            }
                        }
                    }
                    TestStep++;
                    break;
                case 1:
          
                    break;
            }
            LastInitFlagStart = InitFlagStart;
            LastInitFlagStop = InitFlagStop;

            double endtime = (DateTime.Now - intotime).TotalMilliseconds;
        }

    }

    // 2026-05-20: 可配置UDS 2F写入测试项。
    // 配置约定:
    // 1) 被测数据: "FD 03 03"（3字节: DID_H DID_L ControlParameter）
    // 2) 判断数据1: "FE"（写入值XX，十六进制1字节）
    // 3) 判断数据2: 可选，默认"A5"（附加字节）
    // 4) 判断数据3: 可选，默认"00"（附加字节）
    // 最终请求示例: 2F FD 03 03 A5 FE 00
    public class UDS_2F可配置写入 : 测试项基类
    {
        private ushort did;
        private byte controlParameter;
        private byte writeValueXX;
        private byte fixedByteA5 = 0xA5;
        private byte fixedByteTail = 0x00;

        public override void Init()
        {
            结果 = 测试结果.未开始;
            历史结果 = 测试结果.未开始;
            测试数据 = "";
            TestStep = 0;
            LastInitFlagStart = false;
            InitFlagStart = false;
            LastInitFlagStop = false;
            InitFlagStop = false;
            TimeInit();

            try
            {
                byte[] didAndCtrl = ParseHexBytes(Convert.ToString(被测数据));
                if (didAndCtrl.Length < 3)
                    throw new Exception("被测数据格式错误，至少需要3字节，例如: FD 03 03");

                did = (ushort)((didAndCtrl[0] << 8) | didAndCtrl[1]);
                controlParameter = didAndCtrl[2];
                writeValueXX = ParseHexByte(Convert.ToString(结果判断数据1), "判断数据1(XX)");

                string cfgA5 = Convert.ToString(结果判断数据2);
                string cfgTail = Convert.ToString(结果判断数据3);
                if (!string.IsNullOrWhiteSpace(cfgA5))
                    fixedByteA5 = ParseHexByte(cfgA5, "判断数据2(A5)");
                if (!string.IsNullOrWhiteSpace(cfgTail))
                    fixedByteTail = ParseHexByte(cfgTail, "判断数据3(Tail)");
            }
            catch (Exception ex)
            {
                测试数据 = ex.Message;
                结果 = 测试结果.不合格;
                FailDispose();
                TestStep = 99;
            }
        }

        public override void Start(负载状态 OpenorClose, 测试方式 TestMethod)
        {
            DateTime intotime = DateTime.Now;
            try
            {
                switch (TestStep)
                {
                    case 0:
                        {
                            结果 = 测试结果.测试中;
                            var can = FindClientByName("");
                            uint reqId = Convert.ToUInt32(WinForm.CANSendID.Replace("0x", "").Trim(), 16);
                            uint respId = Convert.ToUInt32(WinForm.CANRevID.Replace("0x", "").Trim(), 16);

                            // 请求体: 2F DID_H DID_L ControlParameter A5 XX Tail
                            byte[] controlOptionRecord = new byte[] { controlParameter, fixedByteA5, writeValueXX, fixedByteTail };
                            bool ok = can.TryUds2F(
                                FrameType.ExtendedFrame,
                                reqId,
                                WinForm.CANType,
                                did,
                                controlOptionRecord,
                                out var resp,
                                out var nrc,
                                out var err,
                                expectedResponseCanId: respId,
                                p2TimeoutMs: 1000,
                                p2StarTimeoutMs: 5000,
                                useSingleFramePci: true);

                            if (!ok)
                            {
                                测试数据 = $"2F写入失败 DID=0x{did:X4}, CP=0x{controlParameter:X2}, XX=0x{writeValueXX:X2}, Err={err}";
                                结果 = 测试结果.不合格;
                                FailDispose();
                                TestStep = 99;
                                break;
                            }

                            // 2026-05-20: 额外校验响应第4字节应回显ControlParameter（示例: 6F FD 03 03 ...）。
                            if (!TryCheck2FResponseControlParameter(resp, controlParameter, out string cpErr))
                            {
                                测试数据 = cpErr;
                                结果 = 测试结果.不合格;
                                FailDispose();
                                TestStep = 99;
                                break;
                            }

                            测试数据 = $"2F写入成功 DID=0x{did:X4}, CP=0x{controlParameter:X2}, XX=0x{writeValueXX:X2}";
                            结果 = 测试结果.合格;
                            TestStep = 1;
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                测试数据 = ex.Message;
                结果 = 测试结果.不合格;
                FailDispose();
                TestStep = 99;
            }

            LastInitFlagStart = InitFlagStart;
            LastInitFlagStop = InitFlagStop;
            double endtime = (DateTime.Now - intotime).TotalMilliseconds;
        }

        private static bool TryCheck2FResponseControlParameter(byte[] resp, byte expectedCp, out string err)
        {
            err = string.Empty;
            if (resp == null || resp.Length < 4)
            {
                err = "2F响应长度不足。";
                return false;
            }

            int offset = DetectUdsPayloadOffset(resp);
            if (offset < 0 || resp.Length <= offset + 3)
            {
                err = "2F响应格式异常。";
                return false;
            }

            if (resp[offset] != 0x6F)
            {
                err = $"2F响应SID异常: 0x{resp[offset]:X2}";
                return false;
            }

            byte recvCp = resp[offset + 3];
            if (recvCp != expectedCp)
            {
                err = $"2F响应CP回显不匹配: recv=0x{recvCp:X2}, expect=0x{expectedCp:X2}";
                return false;
            }

            return true;
        }

        private static int DetectUdsPayloadOffset(byte[] payload)
        {
            if (payload == null || payload.Length == 0)
                return -1;

            // 兼容 "00 xx + UDS"
            if (payload.Length >= 3 && payload[0] == 0x00)
                return 2;

            // 兼容 单帧PCI(0x0N)
            if (payload.Length >= 2 && ((payload[0] >> 4) & 0x0F) == 0x0)
                return 1;

            return 0;
        }

        private static byte[] ParseHexBytes(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Array.Empty<byte>();

            string normalized = text.Replace(",", " ")
                                    .Replace("，", " ")
                                    .Replace("-", " ")
                                    .Trim();
            string[] parts = normalized.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var list = new List<byte>();
            foreach (string part in parts)
            {
                list.Add(ParseHexByte(part, "被测数据"));
            }
            return list.ToArray();
        }

        private static byte ParseHexByte(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception($"{fieldName}为空，无法解析十六进制字节。");

            string s = value.Trim();
            if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                s = s.Substring(2);

            if (s.Length == 0 || s.Length > 2)
                throw new Exception($"{fieldName}格式错误: {value}");

            if (!byte.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte b))
                throw new Exception($"{fieldName}不是有效十六进制字节: {value}");

            return b;
        }
    }


    public class UDS_0D解锁 : 测试项基类
    {
        public ushort index = 0;
        //  public byte TestStep = 0;
        Rule rule = new Rule();
        public DateTime FailTime;
        public string UdpName;
        public string SignalName;
        public int FailSum = 0;
        public CANSelectType CanType;
        public uint CanId;
        public byte[] SendUds;

        public override void Init()
        {
            结果 = 测试结果.未开始;
            历史结果 = 测试结果.未开始;
            测试数据 = "";
            TestStep = 0;
            LastInitFlagStart = false;
            InitFlagStart = false;
            LastInitFlagStop = false;
            InitFlagStop = false;
            TimeInit();
            //被测数据 = "0x14DA40F1" 
            //(结果判断数据1) == "CAN"or"CANFD"
          
     
            FailSum = 0;
        }
        public override void Start(负载状态 OpenorClose, 测试方式 TestMethod)
        {
            DateTime intotime = DateTime.Now;
            string 测试数据Temp = "";

            switch (TestStep)
            {
                case 0:
                    var unlocker = new SecurityUnlocker();
                    bool ok = unlocker.TryUnlock(
                        can: FindClientByName(UdpName),
                        requestCanId: Convert.ToUInt32(WinForm.CANSendID.Replace("0x", "").Trim(), 16),
                        securityLevel: 0x0D, // 请求seed通常用奇数等级
                        error: out string err,
                        frameType: FrameType.ExtendedFrame,
                        canSelectType:WinForm.CANType,
                        expectedResponseCanId: Convert.ToUInt32(WinForm.CANRevID.Replace("0x", "").Trim(), 16), // 可空
                        p2TimeoutMs: 500,
                        p2StarTimeoutMs: 5000
                    );


                    if (!ok)
                    {
                        Log.InsertLog($" {err}");
                        结果 = 测试结果.不合格;
                    }
                    else
                    {
                        // 响应格式: 62 DID_H DID_L DATA...
                      // byte[] didData = resp.Length >= 3 ? resp.Skip(3).ToArray() : Array.Empty<byte>();
                        测试数据 = "解锁成功";
                        结果 = 测试结果.合格;
                    }
                    TestStep++;
                    break;
                case 1:
                
                    break;
            }
            LastInitFlagStart = InitFlagStart;
            LastInitFlagStop = InitFlagStop;

            double endtime = (DateTime.Now - intotime).TotalMilliseconds;
        }

    }
    public class 追溯码读取 : 测试项基类
    {
        public ushort index = 0;
        //  public byte TestStep = 0;
        Rule rule = new Rule();
        public DateTime FailTime;
        public string UdpName;
        public string SignalName;
        public int FailSum = 0;
        public override void Init()
        {
            结果 = 测试结果.未开始;
            历史结果 = 测试结果.未开始;
            测试数据 = "";
            TestStep = 0;
            LastInitFlagStart = false;
            InitFlagStart = false;
            LastInitFlagStop = false;
            InitFlagStop = false;
            TimeInit();
            //   ParseWriteOrRead(测试项报文开关);
            if (被测数据.Contains("MCU"))//"MCU1&IAN01_AD"
            {
                string[] parts = 被测数据.Split(new[] { '&' }, 2);
                UdpName = parts[0];
                SignalName = parts.Length > 1 ? parts[1] : "";
            }
            FailSum = 0;
        }
        public override void Start(负载状态 OpenorClose, 测试方式 TestMethod)
        {
            DateTime intotime = DateTime.Now;
            string 测试数据Temp = "";
            var cANUDP = FindMessageByName(UdpName, SignalName);
            Communication.CAN.Message message = new Communication.CAN.Message()
            {
                Id = cANUDP.Id,
                Name = cANUDP.Name,
                WriteOrRead = cANUDP.WriteOrRead,
                FrameType = cANUDP.FrameType,
                SendType = cANUDP.SendType,
                Transmitter = cANUDP.Transmitter,
                Signals = cANUDP.Signals,
                ReadType = cANUDP.ReadType,
            };
            FindClientByName(UdpName).EnSendQueue(message);
            if (被测数据.Contains("MCU"))
            {
                if (Convert.ToString(结果判断数据2) == "ASCII")
                {
                    if (FindClientByName(UdpName).SignalReadBytes.ContainsKey(SignalName))
                        测试数据Temp = Encoding.ASCII.GetString(FindClientByName(UdpName).SignalReadBytes[SignalName]);//截去小数部分
                }
                else if (Convert.ToString(结果判断数据2) == "DEC")
                {
                    if (FindClientByName(UdpName).SignalReadBytes.ContainsKey(SignalName))
                    {

                        var bys = FindClientByName(UdpName).SignalReadBytes[SignalName];
                        byte[] bytes = bys.ToArray();
                        Array.Reverse(bytes);
                        uint result = BitConverter.ToUInt32(bytes, 0);
                        测试数据Temp = result.ToString();
                    }
                }
               // 测试数据Temp = "252423.3-20356";
                测试数据 = 测试数据Temp;
            }
            switch (TestStep)
            {
                case 0:
                   
                    TestStep++;
                    break;
                case 1:
                    //   if (OpenorClose == 负载状态.开启)
                    {
                        结果 = 测试结果.测试中;

                        if (!测试数据Temp.IsNullOrEmpty()) /*|| Convert.ToString(结果判断数据1) == "不校验"*/
                        {
                            if (负载开启保存标志 == false)
                            {
                                PassSave(DateTime.Now.ToString(OpenorClose + "yyyy-MM-dd HH:mm:ss.fff") + "," + 测试项名 + "." + 测试数据 + "," + 结果);
                                负载开启保存标志 = true;
                            }
                            结果 = 测试结果.合格;
                            FailTime = DateTime.Now;
                            WinForm.追溯码 = 测试数据;
                        }
                        if ((DateTime.Now - FailTime).TotalMilliseconds > 9000)
                        {
                            if (FailSum < 2)
                            {
                                FailTime = DateTime.Now;
                                FailSum++;
                                TestStep = 0;
                                break;
                            }
                            结果 = 测试结果.不合格;
                            FailDispose();
                        }

                    }
                    break;
                default:
                    break;
            }
            LastInitFlagStart = InitFlagStart;
            LastInitFlagStop = InitFlagStop;

            double endtime = (DateTime.Now - intotime).TotalMilliseconds;
        }

    }
    public class 零件号读取 : 测试项基类
    {
        public ushort index = 0;
        //  public byte TestStep = 0;
        Rule rule = new Rule();
        public DateTime FailTime;
        public string UdpName;
        public string SignalName;
        public int FailSum = 0;
        public override void Init()
        {
            结果 = 测试结果.未开始;
            历史结果 = 测试结果.未开始;
            测试数据 = "";
            TestStep = 0;
            LastInitFlagStart = false;
            InitFlagStart = false;
            LastInitFlagStop = false;
            InitFlagStop = false;
            TimeInit();
            //   ParseWriteOrRead(测试项报文开关);
            if (被测数据.Contains("MCU"))//"MCU1&IAN01_AD"
            {
                string[] parts = 被测数据.Split(new[] { '&' }, 2);
                UdpName = parts[0];
                SignalName = parts.Length > 1 ? parts[1] : "";
            }
            FailSum = 0;
        }
        public override void Start(负载状态 OpenorClose, 测试方式 TestMethod)
        {
            DateTime intotime = DateTime.Now;
            string 测试数据Temp = "";
            if (被测数据.Contains("MCU"))
            {
                if (Convert.ToString(结果判断数据2) == "ASCII")
                {
                    if (FindClientByName(UdpName).SignalReadBytes.ContainsKey(SignalName))
                        测试数据Temp = Encoding.ASCII.GetString(FindClientByName(UdpName).SignalReadBytes[SignalName]);//截去小数部分
                }
                else if (Convert.ToString(结果判断数据2) == "DEC")
                {
                    if (FindClientByName(UdpName).SignalReadBytes.ContainsKey(SignalName))
                    {
                        var bys = FindClientByName(UdpName).SignalReadBytes[SignalName];
                        byte[] bytes = bys.ToArray();
                        Array.Reverse(bytes);
                        uint result = BitConverter.ToUInt32(bytes, 0);
                        测试数据Temp = result.ToString();
                    }
                }
                测试数据 = 测试数据Temp;
            }
            switch (TestStep)
            {
                case 0:
                    var cANUDP = FindMessageByName(UdpName, SignalName);
                    Communication.CAN.Message message = new Communication.CAN.Message()
                    {
                        Id = cANUDP.Id,
                        Name = cANUDP.Name,
                        WriteOrRead = cANUDP.WriteOrRead,
                        FrameType = cANUDP.FrameType,
                        SendType = cANUDP.SendType,
                        Transmitter = cANUDP.Transmitter,
                        Signals = cANUDP.Signals,
                        ReadType = cANUDP.ReadType,
                    };
                    FindClientByName(UdpName).EnSendQueue(message);
                    TestStep++;
                    break;
                case 1:
                    //   if (OpenorClose == 负载状态.开启)
                    {
                        结果 = 测试结果.测试中;

                        if (测试数据Temp.Trim() == Convert.ToString(结果判断数据1) || Convert.ToString(结果判断数据1) == "不校验")
                        {
                            if (负载开启保存标志 == false)
                            {
                                PassSave(DateTime.Now.ToString(OpenorClose + "yyyy-MM-dd HH:mm:ss.fff") + "," + 测试项名 + "." + 测试数据 + "," + 结果);
                                负载开启保存标志 = true;
                            }
                            结果 = 测试结果.合格;
                            FailTime = DateTime.Now;
                            WinForm.零件号 = 测试数据;
                        }
                        if ((DateTime.Now - FailTime).TotalMilliseconds > 3000)
                        {
                            if (FailSum < 2)
                            {
                                FailTime = DateTime.Now;
                                FailSum++;
                                TestStep = 0;
                                break;
                            }
                            结果 = 测试结果.不合格;
                            FailDispose();
                        }

                    }
                    break;
            }
            LastInitFlagStart = InitFlagStart;
            LastInitFlagStop = InitFlagStop;

            double endtime = (DateTime.Now - intotime).TotalMilliseconds;
        }

    }

    /// <summary>
    /// 判断同一ECUID是否写入多个追溯码
    /// </summary>
    public class ECUID判断 : 测试项基类
    {
        public ushort index = 0;
        //  public byte TestStep = 0;
        Rule rule = new Rule();
        public DateTime FailTime;
        public string UdpName;
        public string SignalName;
        public int FailSum = 0;
        public override void Init()
        {
            结果 = 测试结果.未开始;
            历史结果 = 测试结果.未开始;
            测试数据 = "";
            TestStep = 0;
            LastInitFlagStart = false;
            InitFlagStart = false;
            LastInitFlagStop = false;
            InitFlagStop = false;
            TimeInit();
            //   ParseWriteOrRead(测试项报文开关);
            if (被测数据.Contains("MCU"))//"MCU1&IAN01_AD"
            {
                string[] parts = 被测数据.Split(new[] { '&' }, 2);
                UdpName = parts[0];
                SignalName = parts.Length > 1 ? parts[1] : "";
            }
            FailSum = 0;
        }
        public override void Start(负载状态 OpenorClose, 测试方式 TestMethod)
        {
            DateTime intotime = DateTime.Now;
            string 测试数据Temp = "";
            if (被测数据.Contains("MCU"))
            {
                if (Convert.ToString(结果判断数据2) == "ASCII")
                {
                    if (FindClientByName(UdpName).SignalReadBytes.ContainsKey(SignalName))
                        测试数据Temp = Encoding.ASCII.GetString(FindClientByName(UdpName).SignalReadBytes[SignalName]);//截去小数部分
                }
                else if (Convert.ToString(结果判断数据2) == "DEC")
                {
                    if (FindClientByName(UdpName).SignalReadBytes.ContainsKey(SignalName))
                    {

                        var bys = FindClientByName(UdpName).SignalReadBytes[SignalName];
                        byte[] bytes = bys.ToArray();
                        Array.Reverse(bytes);
                        uint result = BitConverter.ToUInt32(bytes, 0);
                        测试数据Temp = result.ToString();
                    }
                }

                测试数据 = "123456789123456789";// 测试数据Temp;
                测试数据Temp = 测试数据;
            }
            switch (TestStep)
            {
                case 0:
                    var cANUDP = FindMessageByName(UdpName, SignalName);
                    Communication.CAN.Message message = new Communication.CAN.Message()
                    {
                        Id = cANUDP.Id,
                        Name = cANUDP.Name,
                        WriteOrRead = cANUDP.WriteOrRead,
                        FrameType = cANUDP.FrameType,
                        SendType = cANUDP.SendType,
                        Transmitter = cANUDP.Transmitter,
                        Signals = cANUDP.Signals,
                        ReadType = cANUDP.ReadType,
                    };
                    FindClientByName(UdpName).EnSendQueue(message);
                    WinForm.ECUID = 测试数据Temp;
                    TestStep++;
                    break;
                case 1:
                    //   if (OpenorClose == 负载状态.开启)
                    {
                        结果 = 测试结果.测试中;
                        var st = WebServer.GetTestResultFromEcuid(测试数据Temp);
                        if(st.Length == 0)
                        {
                            Log.InsertLog("Ecuid查询成功,不存在重复写入");
                            结果 = 测试结果.合格;
                        }
                        else if(WinForm.本次为复测 == true)
                        {
                            Log.InsertLog("本次为复测，跳过是否多次写入ecuid");
                            WinForm.本次为复测 = false;
                            结果 = 测试结果.合格;
                        }
                        else
                        {
                            foreach(var temp in st)
                            {
                                if(temp.mtc != WinForm.追溯码)
                                {
                                    Log.InsertLog("存在重复写入ECUID MTC：" + WinForm.追溯码);
                                   // 测试数据 = "存在重复写入ECUID MTC：" + WinForm.追溯码;
                                    结果 = 测试结果.不合格;
                                    FailDispose();
                                }
                            }
                        }
                        
                        TestStep++;
                    }
                    break;
                default:
                    break;
            }
            LastInitFlagStart = InitFlagStart;
            LastInitFlagStop = InitFlagStop;

            double endtime = (DateTime.Now - intotime).TotalMilliseconds;
        }

    }
    public class 扫码对比 : 测试项基类
    {
        public ushort index = 0;
        //  public byte TestStep = 0;
        Rule rule = new Rule();
        public DateTime FailTime;
        public string UdpName;
        public string SignalName;
        public int FailSum = 0;
        public string ScanString = "";
        public override void Init()
        {
            结果 = 测试结果.未开始;
            历史结果 = 测试结果.未开始;
            测试数据 = "";
            TestStep = 0;
            LastInitFlagStart = false;
            InitFlagStart = false;
            LastInitFlagStop = false;
            InitFlagStop = false;
            TimeInit();

            ScanString = "";
            FailSum = 0;
        }
        public override void Start(负载状态 OpenorClose, 测试方式 TestMethod)
        {
            DateTime intotime = DateTime.Now;
           

            测试数据 = "条码:" + ScanString +" 追溯码:" + WinForm.追溯码;
            
            switch (TestStep)
            {
                case 0:
                    if (Convert.ToString(结果判断数据1).Contains("-"))//"MCU1&IAN01_AD"
                    {
                        string[] parts = Convert.ToString(结果判断数据1).Split(new[] { '-' }, 2);
                        ScanString += WinForm._ScanString.Substring(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]));
                    }
                    if (Convert.ToString(结果判断数据2).Contains("-"))//"MCU1&IAN01_AD"
                    {
                        string[] parts = Convert.ToString(结果判断数据2).Split(new[] { '-' }, 2);
                        ScanString += WinForm._ScanString.Substring(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]));
                    }
                    if (Convert.ToString(结果判断数据3).Contains("-"))//"MCU1&IAN01_AD"
                    {
                        string[] parts = Convert.ToString(结果判断数据3).Split(new[] { '-' }, 2);
                        ScanString += WinForm._ScanString.Substring(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]));
                    }
                    TestStep++;
                    break;
                case 1:
                    //   if (OpenorClose == 负载状态.开启)
                    {
                        结果 = 测试结果.测试中;

                        if (WinForm.追溯码.Contains(ScanString))
                        {
                            结果 = 测试结果.合格;
                            FailTime = DateTime.Now;
                        //    WinForm.追溯码 = 测试数据;
                        }
                        else
                        {
                            结果 = 测试结果.不合格;
                            FailDispose();
                        }
                        TestStep++;
                    }
                    break;
                    default:
                    break;
            }
            LastInitFlagStart = InitFlagStart;
            LastInitFlagStop = InitFlagStop;

            double endtime = (DateTime.Now - intotime).TotalMilliseconds;
        }

    }
    public class 前工位判断  : 测试项基类
    {
        public ushort index = 0;
        //  public byte TestStep = 0;
        Rule rule = new Rule();
        public DateTime FailTime;
        public string UdpName;
        public string SignalName;
        public int FailSum = 0;
        public override void Init()
        {
            结果 = 测试结果.未开始;
            历史结果 = 测试结果.未开始;
            测试数据 = "";
            TestStep = 0;
            LastInitFlagStart = false;
            InitFlagStart = false;
            LastInitFlagStop = false;
            InitFlagStop = false;
            TimeInit();

            FailSum = 0;
        }
        public override void Start(负载状态 OpenorClose, 测试方式 TestMethod)
        {
            DateTime intotime = DateTime.Now;
            string 测试数据Temp = "";

            switch (TestStep)
            {
                case 0:
                    结果 = 测试结果.测试中;

                    //   if (OpenorClose == 负载状态.开启)
                    {
                        string 追溯码 = "";
                        string 零件号 = "";
                        foreach (var n in CaseConfig.caseList)
                        {
                            if (n.GetCaseName() == "MCU1 End model PartNumber")
                            {
                                零件号 = n.GetData();
                            }
                            if (n.GetCaseName() == "MCU1 追溯码批次号")
                            {
                                追溯码 = n.GetData();
                            }
                        }
                         int result = WinForm.SelectGW(追溯码, 零件号);
                        if (result == 2)
                        {
                            TestStep++;
                            结果 = 测试结果.合格;
                            FailTime = DateTime.Now;
                        }
                        else
                        {
                            TestStep++;
                            结果 = 测试结果.不合格;
                            FailDispose();
                        }

                    }
                    break;
            }
        }

    }
    public class 写MEC校验 : 测试项基类
    {
        public ushort index = 0;
        //  public byte TestStep = 0;
        Rule rule = new Rule();
        public DateTime FailTime;
        public string UdpName;
        public string SignalName;
        public int FailSum = 0;
        public override void Init()
        {
            结果 = 测试结果.未开始;
            历史结果 = 测试结果.未开始;
            测试数据 = "";
            TestStep = 0;
            LastInitFlagStart = false;
            InitFlagStart = false;
            LastInitFlagStop = false;
            InitFlagStop = false;
            TimeInit();
            //   ParseWriteOrRead(测试项报文开关);
            if (被测数据.Contains("MCU"))//"MCU1&IAN01_AD"
            {
                string[] parts = 被测数据.Split(new[] { '&' }, 2);
                UdpName = parts[0];
                SignalName = parts.Length > 1 ? parts[1] : "";
            }
            FailSum = 0;
        }
        public override void Start(负载状态 OpenorClose, 测试方式 TestMethod)
        {
            DateTime intotime = DateTime.Now;
            string 测试数据Temp = "";
            if (被测数据.Contains("MCU"))
            {
                if (Convert.ToString(结果判断数据2) == "ASCII")
                {
                    if (FindClientByName(UdpName).SignalReadBytes.ContainsKey(SignalName))
                        测试数据Temp = Encoding.ASCII.GetString(FindClientByName(UdpName).SignalReadBytes[SignalName]);//截去小数部分
                }
                else if (Convert.ToString(结果判断数据2) == "DEC")
                {
                    if (FindClientByName(UdpName).SignalReadBytes.ContainsKey(SignalName))
                    {

                        var bys = FindClientByName(UdpName).SignalReadBytes[SignalName];
                        byte[] bytes = bys.ToArray();
                        Array.Reverse(bytes);
                        uint result = BitConverter.ToUInt32(bytes, 0);
                        测试数据Temp = result.ToString();
                    }
                }

                测试数据 = 测试数据Temp;
            }
            switch (TestStep)
            {
                case 0:
                    var cANUDP = FindMessageByName(UdpName, SignalName);
                    Communication.CAN.Message message = new Communication.CAN.Message()
                    {
                        Id = cANUDP.Id,
                        Name = cANUDP.Name,
                        WriteOrRead = cANUDP.WriteOrRead,
                        FrameType = cANUDP.FrameType,
                        SendType = cANUDP.SendType,
                        Transmitter = cANUDP.Transmitter,
                    };
                    FindClientByName(UdpName).EnSendQueue(message);
                    TestStep++;
                    break;
                case 1:
                    //   if (OpenorClose == 负载状态.开启)
                    {
                        结果 = 测试结果.测试中;

                        if (测试数据Temp.Trim() == Convert.ToString(结果判断数据1) || Convert.ToString(结果判断数据1) == "不校验")
                        {
                            if (负载开启保存标志 == false)
                            {
                                PassSave(DateTime.Now.ToString(OpenorClose + "yyyy-MM-dd HH:mm:ss.fff") + "," + 测试项名 + "." + 测试数据 + "," + 结果);
                                负载开启保存标志 = true;
                            }
                            结果 = 测试结果.合格;
                            FailTime = DateTime.Now;
                        }
                        if ((DateTime.Now - FailTime).TotalMilliseconds > 3000)
                        {
                            if (FailSum < 2)
                            {
                                FailTime = DateTime.Now;
                                FailSum++;
                                TestStep = 0;
                                break;
                            }
                            结果 = 测试结果.不合格;
                            FailDispose();
                        }

                    }
                    break;
            }
            LastInitFlagStart = InitFlagStart;
            LastInitFlagStop = InitFlagStop;

            double endtime = (DateTime.Now - intotime).TotalMilliseconds;
        }

    }
    public class 下载ECUID以及密钥 : 测试项基类
    {
        public ushort index = 0;
        //  public byte TestStep = 0;
        Rule rule = new Rule();
        public DateTime FailTime;
        public string UdpName;
        public string SignalName;
        public int FailSum = 0;
        private string SeverName = string.Empty;
        private GmApi.IIecsApi api;
        private TupleRequstInfoModel tupleInfo;
        public static int DownLoadResult = 0;
        public static int requstIdResult = 0;
        public override void Init()
        {
            结果 = 测试结果.未开始;
            历史结果 = 测试结果.未开始;
            测试数据 = "";
            TestStep = 0;
            LastInitFlagStart = false;
            InitFlagStart = false;
            LastInitFlagStop = false;
            InitFlagStop = false;
            TimeInit();
            //   ParseWriteOrRead(测试项报文开关);
            if (被测数据.Contains("MCU"))//"MCU1&IAN01_AD"
            {
                string[] parts = 被测数据.Split(new[] { '&' }, 2);
                UdpName = parts[0];
                SignalName = parts.Length > 1 ? parts[1] : "";
            }
            FailSum = 0;
            DownLoadResult = 0;
            requstIdResult = 0;
            SeverName = Convert.ToString(结果判断数据1);
            WinForm._tupleTask = default;
            WinForm.SeverName = SeverName;
        }
        public override async void Start(负载状态 OpenorClose, 测试方式 TestMethod)
        {
            DateTime intotime = DateTime.Now;
            string 测试数据Temp = "";
           
            switch (TestStep)
            {
                case 0:
                    // 判断是否安装解密软件
                    结果 = 测试结果.测试中;
                    if (string.IsNullOrEmpty(GpgNet.Version))
                    {
                        Log.InsertLog($"操作失败。请确认解密软件已经正确安装。");
                        结果 = 测试结果.不合格;
                        FailDispose();
                    }
                    if (
                        WinForm._tupleTask != default &&
                        (
                            WinForm._tupleTask.Stage != StageType.Import &&
                            WinForm._tupleTask.Stage != StageType.Expired &&
                            WinForm._tupleTask.Stage != StageType.ServerFailed
                        ))
                    {
                        Log.InsertLog($"操作失败。自动下载正在运行。");
                        结果 = 测试结果.不合格;
                        FailDispose();
                        TestStep = 99;
                    }
                    TestStep++;
                    break;
                case 1:
                    // 判断是否配置GM服务器
                    if (string.IsNullOrWhiteSpace(SeverName))
                    {
                        MessageBox.Show("该产品未配置服务器。", "操作失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Log.InsertLog($"操作失败。该产品未配置服务器。");
                        结果 = 测试结果.不合格;
                        FailDispose();
                        TestStep = 99;
                    }
                    TestStep++;

                    break;
                case 2:

                    测试数据 = "开始请求id";
                   // var res = new TupleRequstInfoDal().InsertBatch(tupleInfo);
                    new Task(() => requstId()).Start();
                    TestStep++;
                   
                    break;
                case 3:
                    测试数据 = "等待请求id结果";
                    //  DownloadTulpe(api, tupleInfo);
                    if (requstIdResult == 2)
                    {
                        测试数据 = "id请求结果成功";
                        TestStep++;
                    }
                    else if (requstIdResult == 3)
                    {
                        测试数据 = "id请求结果失败";
                        结果 = 测试结果.不合格;
                        TestStep = 99;
                        FailDispose();
                    }
                    break;
                case 4:
                    测试数据 = "开始下载ecuid";
                    // var res = new TupleRequstInfoDal().InsertBatch(tupleInfo);
                    new Task(() => DownloadTulpe(api, tupleInfo)).Start();
                    TestStep++;

                    break;
                case 5:
                    测试数据 = "等待ecuid下载结果";
                    //  DownloadTulpe(api, tupleInfo);
                    if (DownLoadResult == 2)
                    {
                        测试数据 = "ecuid下载结果成功";
                        结果 = 测试结果.合格;
                        TestStep++;
                    }
                    else if(DownLoadResult == 3) 
                    {
                        测试数据 = "ecuid下载结果失败";
                        结果 = 测试结果.不合格;
                        TestStep = 99;
                        FailDispose();
                    }
                    
                    break;
                default:
                    break;
            }
            LastInitFlagStart = InitFlagStart;
            LastInitFlagStop = InitFlagStop;

            double endtime = (DateTime.Now - intotime).TotalMilliseconds;
        }
        private string GetGmErrorTypeText(string errorType)
        {
            switch (errorType)
            {
                case null:
                case "":
                    return "未知";

                case "PRODUCT_INFO_NOT_FOUND":
                    return "没有找到应该产品，请检查产品信息配置。";

                default:
                    return errorType;
            }

        }
        private async void requstId()
        {
            api = GmApi.IecsApi.GetIecsApi(SeverName);
            //    int requstNum = (int)numericUpDown_DownloadNum.Value;
            GmApi.Models.DownloadTuple.DownloadTupleResponseModel requstIdResponse = default;
            try
            {
                requstIdResponse = await api.GetDownloadTupleRequset(Convert.ToString(结果判断数据2), 1);
                //var requstIdResponse = new GmApi.Models.DownloadTuple.DownloadTupleResponseModel() { RequestId = "E11AA4D86376BA55E0534512800AE4C1", Status = null };
            }
            catch (Exception ex)
            {
                Log.InsertLog($"请求GM服务器出错，请检查网络是不否可用。error:{ex.Message}");
                requstIdResult = 3;
                //TestStep = 99;
                return;
            }

            if (string.IsNullOrEmpty(requstIdResponse.RequestId))
            {
                Log.InsertLog($"请求失败，原因：{GetGmErrorTypeText(requstIdResponse.ErrorType)}。");
                requstIdResult = 3;
                //TestStep = 99;
                return;
            }
            tupleInfo = new TupleRequstInfoModel()
            {
                Id = requstIdResponse.RequestId,
                RequetId = requstIdResponse.RequestId,
                RequstTime = DateTime.Now,
                Status = "GetRequstId",
                Stage = StageType.GetRequstId,
                //    ProductName = productModelInfo.产品型号,
                GmProductName = SeverName,
                //   IsBindPn = checkBox_PnCode.Checked ? 0 : 1,
                RequstNum = 1
            };
            WinForm._tupleTask = tupleInfo;
            requstIdResult = 2;
            Log.InsertLog($"]【Id:{requstIdResponse.RequestId}】请求GM服务器生成【1组】ECU数据成功，请不要关闭程序，等待数据生成完成。");
        }
        private async void DownloadTulpe(GmApi.IIecsApi api, TupleRequstInfoModel tupleInfo)
        {
         //   var dao = new TupleRequstInfoDal();
            string tuplesDataPath = "tuples";

            while (true)//tupleInfo.Stage == StageType.GetRequstId)
            {
                switch (tupleInfo.Stage)
                {
                    case StageType.GetRequstId:
                        GmApi.Models.DownloadTuple.DownloadTupleResponseModel requstIdResponse = default;
                        try
                        {
                            requstIdResponse = await api.GetDownloadTupleFile(tupleInfo.RequetId);
                        }
                        catch (Exception ex)
                        {
                            Log.InsertLog($"【Id:{tupleInfo.RequetId}】请求GM服务器出错，请检查网络是不否可用，将在30秒后重试。Error：{ex.Message}");
                            Thread.Sleep(30000);
                            continue;
                        }
                        switch (requstIdResponse.Status.ToUpper())
                        {
                            case "COMPLETED":
                                // GM数据已生成，开始下载数据
                                tupleInfo.Status = requstIdResponse.Status;
                                tupleInfo.Stage = StageType.Downloading;
                               // Utils.Logger.Debug(Newtonsoft.Json.JsonConvert.SerializeObject(requstIdResponse));
                                //dao.UpdateStatus(requstIdResponse.RequestId, requstIdResponse.Status, StageType.Downloading);
                                Log.InsertLog($"【Id:{tupleInfo.RequetId}】数据已生成。开始保存文件");
                                var fileResponse = requstIdResponse;
                                // 下载数据
                                //var fileResponse = await api.GetDownloadTupleFile(requstIdResponse.RequestId);
                                //var fileResponse = await api.GetDownloadTupleFile("E11AA4D86378BA55E0534512800AE4C1");

                                // 生成文件
                                byte[] downloadBytes = Convert.FromBase64String(fileResponse.DownloadFile);
                                if (!Directory.Exists(tuplesDataPath)) Directory.CreateDirectory(tuplesDataPath);
                                FileInfo file = new FileInfo(Path.Combine(tuplesDataPath, fileResponse.FileName));
                                //File.WriteAllBytes(Path.Combine(tuplesDataPath, fileResponse.FileName), downloadBytes);

                                if (file.Exists)
                                {
                                    string fileName = file.Name.Substring(0, file.Name.IndexOf('.'));
                                    fileName += DateTime.Now.ToString("-yyMMddHHmmss");
                                    fileName += file.Name.Substring(file.Name.IndexOf('.'));
                                    file = new FileInfo(Path.Combine(tuplesDataPath, fileName));
                                }
                                try
                                {
                                    using (var fs = file.Create())
                                    {
                                        Log.InsertLog($"【Id:{tupleInfo.RequetId}】正在保存文件。");

                                        try
                                        {
                                            fs.Write(downloadBytes, 0, downloadBytes.Length);
                                            fs.Close();
                                            file.Attributes = FileAttributes.ReadOnly;
                                        }
                                        catch (IOException ex)
                                        {
                                            Log.InsertLog($"【Id:{tupleInfo.RequetId}】保存文件失败，请检查磁盘空间，将在30秒后重试。Error:{ex.Message}");
                                            Thread.Sleep(30000);
                                            continue;
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.InsertLog($"【Id:{tupleInfo.RequetId}】保存文件失败，将在30秒后重试。Error:{ex.Message}");
                                            Thread.Sleep(30000);
                                            continue;
                                        }
                                    }
                                }
                                catch (UnauthorizedAccessException ex)
                                {
                                    Log.InsertLog($"【Id:{tupleInfo.RequetId}】文件已存在，将在30秒后重试。Error:{ex.Message}");
                                    Thread.Sleep(30000);
                                    continue;
                                }
                                catch (Exception ex)
                                {
                                    Log.InsertLog($"【Id:{tupleInfo.RequetId}】保存文件失败，将在30秒后重试。Error:{ex.Message}");
                                    Thread.Sleep(30000);
                                    continue;
                                }

                                tupleInfo.Status = fileResponse.Status;
                                tupleInfo.Stage = StageType.Downloaded;
                                tupleInfo.FileName = fileResponse.FileName;
                              //  dao.UpdateStatus(requstIdResponse.RequestId, requstIdResponse.Status, tupleInfo.FileName);
                                break;
                            case "EXPIRED":
                            case "DELETED":
                                tupleInfo.Status = requstIdResponse.Status;
                                tupleInfo.Stage = StageType.Expired;
                              //  dao.UpdateStatus(tupleInfo.RequetId, tupleInfo.Status, StageType.Expired);

                                Log.InsertLog($"【Id:{tupleInfo.RequetId}】操作失败。请求已超时，任务结束。");
                                DownLoadResult = 3;
                                return;
                            case "FAILED":
                                tupleInfo.Status = requstIdResponse.Status;
                                tupleInfo.Stage = StageType.ServerFailed;
                              //  dao.UpdateStatus(tupleInfo.RequetId, tupleInfo.Status, StageType.ServerFailed);

                                Log.InsertLog($"【Id:{tupleInfo.RequetId}】操作失败。服务器返回失败，任务结束。");
                                DownLoadResult = 3;
                                return;
                            case "PROCESSING":
                                Thread.Sleep(1000);
                                continue;
                            case "WAITING":
                                Thread.Sleep(10000);
                                continue;
                            default:
                                Debug.WriteLine(tupleInfo.Status);
                                //dao.UpdateStatus(tupleInfo.RequetId, tupleInfo.Status);
                                continue;
                        }
                        break;
                    case StageType.Downloaded:

                        //FileInfo file = new FileInfo(Path.Combine(tuplesDataPath, tupleInfo.FileName));

                        Log.InsertLog($"【Id:{tupleInfo.RequetId}】开始解析文件，并导入生产数据库。");
                        // 文件解密
                        if (!File.Exists(Path.Combine(tuplesDataPath, tupleInfo.FileName)))
                        {
                            Log.InsertLog($"【Id:{tupleInfo.RequetId}】文件不存在，请确认数据是否已导入。");
                            tupleInfo.Stage = StageType.ImportFailed;
                           // dao.UpdateStatus(tupleInfo.RequetId, tupleInfo.Status,StageType.LostFile, "file missing.");
                            DownLoadResult = 3;
                            return;
                        }
                        var inputBuffer = MemoryGpgBuffer.CreateFromFile(Path.Combine(tuplesDataPath, tupleInfo.FileName));
                        var gpg = GpgContext.CreateContext();
                        var bb = gpg.Decrypt(inputBuffer);
                        byte[] decryptBytes = Kyle.Lib.TypeConverter.ByteConverter.ReadStream(bb);

                        // 判断文件大小
                        if (decryptBytes.Length != 144 *   WinForm._tupleTask.RequstNum)
                        {
                            Log.InsertLog($"【Id:{tupleInfo.RequetId}】文件文件大小异常，1个ECUID应为144，实际为{decryptBytes.Length}。");
                            tupleInfo.Stage = StageType.ImportFailed;
                         //   dao.UpdateStatus(tupleInfo.RequetId, tupleInfo.Status, StageType.ImportFailed, "File size exception");
                            DownLoadResult = 3;
                            return;
                        }
                        // 导入数据

                        var isSuccess = await ImportDataToProductServer(decryptBytes, new FileInfo(Path.Combine(tuplesDataPath, tupleInfo.FileName)),WinForm.产品型号 );
                      //  dao.UpdateStatus(tupleInfo.RequetId, tupleInfo.Status, isSuccess ? StageType.Import : StageType.ImportFailed);
                        if (isSuccess)
                        {
                            tupleInfo.Stage = StageType.Import;
                            Log.InsertLog($"【Id:{tupleInfo.RequetId}】下载完成。");
                            DownLoadResult = 2;
                            return;
                        }
                        else
                        {
                            tupleInfo.Stage = StageType.ImportFailed;
                            Log.InsertLog($"【Id:{tupleInfo.RequetId}】下载失败，尝试手动导入。文件名：{tupleInfo.FileName}");
                            DownLoadResult = 3;
                            return;
                        }

                        break;
                }
            }
        }
        /// <summary>
        /// 数据导入生产服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dataBytes"></param>
        /// <param name="gmDeviceType"></param>
        /// <param name="file"></param>
        private async Task<bool> ImportDataToProductServer(byte[] dataBytes, FileInfo file,string 产品型号)
        {

            // 文件标记为只读
            file.Attributes = FileAttributes.ReadOnly;

            // 数据列表
            List<EcuidInfoModel> ecuidInfos = new List<EcuidInfoModel>();
            //bool isSuccess = false;
            DateTime RequstTime;
            var str_datetime = Regex.Match(file.Name, @"20\d{2}-\d{2}-\d{2}-\d{2}-\d{2}-\d{2}").Value;
            DateTime.TryParseExact(str_datetime, "yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out RequstTime);

            //标准化数据
            for (long i = 0; i < dataBytes.LongLength; i += 144)
            {
                var info = new EcuidInfoModel();
                info.ECUID = BytesToHex(dataBytes, i + 0, 16);
                info.MKM1 = BytesToHex(dataBytes, i + 16, 16);
                info.MKM2 = BytesToHex(dataBytes, i + 32, 32);
                info.MKM3 = BytesToHex(dataBytes, i + 64, 16);
                info.UKM1 = BytesToHex(dataBytes, i + 80, 16);
                info.UKM2 = BytesToHex(dataBytes, i + 96, 32);
                info.UKM3 = BytesToHex(dataBytes, i + 128, 16);
                info.CREATETIME = DateTime.Now;
              //  info.PN = productModelInfo.用户图号;
                info.STATE = 0;
                info.RequstDate_GM = RequstTime;
                info.ProductModel_GM = 产品型号;
                WinForm.ECUID = info.ECUID;
               ecuidInfos.Add(info);
            }
           
            //插入数据库
            try
            {
                if (new EcuidInfoDal().InsertBatch(ecuidInfos))
                {
                    var oldDir = file.DirectoryName;//Path.GetDirectoryName(fileDialog.FileName);
                    var bakDir = new DirectoryInfo(Path.Combine(oldDir, "used"));


                    if (!bakDir.Exists)
                        bakDir.Create();
                    //if (string.IsNullOrWhiteSpace(productModelInfo.用户图号))
                    //{
                    //    InvokeWriteLog($"【{(sender as Button).Text}】成功到导入【{productModelInfo.ProductModel_GM}】【{ecuidInfos.Count}条】不绑定图号数据。");
                    //}
                    //else
                    {
                        Log.InsertLog($"成功到导入【{ecuidInfos.Count}条】数据。");
                    }
                    try
                    {
                        File.Move(Path.Combine(oldDir, file.Name), Path.Combine(bakDir.FullName, file.Name + "." + DateTime.Now.ToString("yyyyMMddHHmmss")));
                    }
                    catch (Exception ex)
                    {
                        Log.InsertLog($"文件移动失败。Error:{ex.Message}");
                       // Utils.Logger.Debug(ex.ToString());
                    }
                    return true;
                }
                else
                {
                    Log.InsertLog($"操作失败，存在重复数据。");
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.InsertLog($"{ex.Message}");
                return false;
            }
        }
        public static string BytesToHex(byte[] bytes, long startIndex, int count)
        {
            StringBuilder sb = new StringBuilder();
            for (long i = startIndex; i < startIndex + count; i++)
            {
                sb.Append(bytes[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }

    public class 获取以及写入ECUID : 测试项基类
    {
        public ushort index = 0;
        //  public byte TestStep = 0;
        Rule rule = new Rule();
        public DateTime FailTime;
        public string UdpName;
        public string SignalName;
        public int FailSum = 0;
        private string SeverName = string.Empty;
        private GmApi.IIecsApi api;
        private TupleRequstInfoModel tupleInfo;
        public static int DownLoadResult = 0;
        
        public override void Init()
        {
            结果 = 测试结果.未开始;
            历史结果 = 测试结果.未开始;
            测试数据 = "";
            TestStep = 0;
            LastInitFlagStart = false;
            InitFlagStart = false;
            LastInitFlagStop = false;
            InitFlagStop = false;
            TimeInit();
            FailSum = 0;
            DownLoadResult = 0;
        }
        public override async void Start(负载状态 OpenorClose, 测试方式 TestMethod)
        {
            DateTime intotime = DateTime.Now;
            string 测试数据Temp = "";

            switch (TestStep)
            {
                case 0:
                    结果 = 测试结果.测试中;
                    //获取ECUID数据库
                    try
                    {
                       WinForm.ecuidInfoModels = new EcuidInfoDal().GetAvailableData(WinForm.产品型号);
                    }
                    catch (Exception ex)
                    {
                        Log.InsertLog($"{ex.Message}");
                        结果 = 测试结果.不合格;
                        FailDispose();
                        TestStep = 99;
                    }
                    TestStep++;
                    break;
                case 1:
                    // 开始写入ECUID 
                    if (!string.IsNullOrWhiteSpace(WinForm.ecuidInfoModels[0].ECUID))
                    {
                        // 1. 转 byte[]
                        byte[] data = HexStringToBytes(WinForm.ecuidInfoModels[0].ECUID);

                        // 2. 在前面加 0x03 和 0xA5
                        byte[] EcuidDatas = new byte[] { 0x03, 0xA5 }
                            .Concat(data)
                            .ToArray();
                        bool ok = FindClientByName("").TryUds2F(
                        FrameType.ExtendedFrame,
                            Convert.ToUInt32(WinForm.CANSendID.Replace("0x", "").Trim(), 16),
                            WinForm.CANType,
                            did: 0xFD02,
                            controlOptionRecord: EcuidDatas, // 例如ShortTermAdjustment
                            responsePayload: out var resp,
                            negativeResponseCode: out var nrc,
                            error: out var err,
                            expectedResponseCanId: null,
                            p2TimeoutMs: 500,
                            p2StarTimeoutMs: 5000,
                            useSingleFramePci: true
                        );
                        if(!ok)
                        {
                            Log.InsertLog($"ECUID写入失败:{err}");
                            结果 = 测试结果.不合格;
                            FailDispose();
                            TestStep = 99;
                        }
                        else
                        {
                            测试数据 = "写入ECUID:" + WinForm.ecuidInfoModels[0].ECUID;

                            Log.InsertLog($"更新{WinForm.ecuidInfoModels[0].ECUID} STATE标识为2");
                            if(new EcuidInfoDal().UpdateUpStateBatch(WinForm.ecuidInfoModels[0].ECUID)==true )
                            {
                                TestStep++;
                                结果 = 测试结果.合格;
                            }
                            else
                            {
                                Log.InsertLog($"ECUID写入标识更新失败:{err}");
                                结果 = 测试结果.不合格;
                                FailDispose();
                                TestStep = 99;
                            }
                           
                        }
                    }
                    else
                    {
                        Log.InsertLog($"ECUID为空");
                        结果 = 测试结果.不合格;
                        FailDispose();
                        TestStep = 99;
                    }
                    break;
                default:
                    break;
            }
            LastInitFlagStart = InitFlagStart;
            LastInitFlagStop = InitFlagStop;

            double endtime = (DateTime.Now - intotime).TotalMilliseconds;
        }
        public static byte[] HexStringToBytes(string hex)
        {
            hex = hex.Replace(" ", "")
                     .Replace("-", "")
                     .Replace("0x", "");

            if (hex.Length % 2 != 0)
                throw new ArgumentException("十六进制字符串长度必须是偶数");

            byte[] bytes = new byte[hex.Length / 2];

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }

            return bytes;
        }
    }
    public class CAN初始化 : 测试项基类
    {
        public ushort index = 0;
        //  public byte TestStep = 0;
        Rule rule = new Rule();
        public DateTime FailTime;
        public string UdpName;
        public string SignalName;
        public int FailSum = 0;
        ushort Arbitration;
        ushort DataSegment;
        string CANRevID;
        byte RevLenth = 0;
        public override void Init()
        {
            结果 = 测试结果.未开始;
            历史结果 = 测试结果.未开始;
            测试数据 = "";
            TestStep = 0;
            LastInitFlagStart = false;
            InitFlagStart = false;
            LastInitFlagStop = false;
            InitFlagStop = false;
            TimeInit();
            //ParseWriteOrRead(测试项报文开关);
            //结果判断数据1 ：仲裁段波特率&数据段波特率
            //结果判断数据2 ：发送诊断报文ID&接收诊断报文ID
            //结果判断数据3 : CANfd or can
            var parts = Convert.ToString(结果判断数据1).Split('&');
            Arbitration = Convert.ToUInt16(parts[0]);
            DataSegment = Convert.ToUInt16(parts[1]);
            var partsID = Convert.ToString(结果判断数据2).Split('&');
            WinForm.CANSendID = partsID[0];
            WinForm.CANRevID = partsID[1];
            var partsCan = Convert.ToString(结果判断数据3).Split('&');
            if (partsCan[0] == "CAN")
                WinForm.CANType = CANSelectType.CAN;
            else
                WinForm.CANType = CANSelectType.CANFD;
            RevLenth = Convert.ToByte(partsCan[1]);

               FailSum = 0;
        }
        public override void Start(负载状态 OpenorClose, 测试方式 TestMethod)
        {
            DateTime intotime = DateTime.Now;
            string 测试数据Temp = "";
          
            switch (TestStep)
            {
                case 0:
                    if(WinForm.CanInitFlag == true)
                    {
                        结果 = 测试结果.合格;
                        TestStep = 3;
                        break;
                    }
                    FindClientByName("").CanInitSend(Arbitration, DataSegment, RevLenth);
                    TestStep++;
                    break;
                case 1:
                    byte StepTemp = StepTime(StartTime, 1000, TestStep);
                    if (StepTemp != 0)
                    {
                        TestStep = StepTemp;
                        StartTime = DateTime.Now;
                    }
                    break;

                case 2:
                    //   if (OpenorClose == 负载状态.开启)
                    {
                        FindClientByName("").CanIDInitSend(WinForm.CANRevID);
                        WinForm.CanInitFlag = true;
                        结果 = 测试结果.合格;
                        TestStep++;

                    }
                    break;
            }
            LastInitFlagStart = InitFlagStart;
            LastInitFlagStop = InitFlagStop;

            double endtime = (DateTime.Now - intotime).TotalMilliseconds;
        }

    }
    public class WriteMASTER_UnlockKey : 测试项基类
    {
        public ushort index = 0;
        //  public byte TestStep = 0;
        Rule rule = new Rule();
        public DateTime FailTime;
        public string UdpName;
        public string SignalName;
        public int FailSum = 0;
        private string SeverName = string.Empty;
        private GmApi.IIecsApi api;
        private TupleRequstInfoModel tupleInfo;
        public static int DownLoadResult = 0;
        byte[] MasterResetM1 = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x11 };//lenth 16
        byte[] MasterResetM2 = { 0x88, 0x9B, 0x71, 0x64, 0x28, 0xBF, 0x0F, 0xD9, 0x9A, 0xBA, 0x27, 0xFC, 0x1F, 0xB1, 0xDE, 0x0D,
                                 0x95, 0x90, 0x81, 0x6E, 0xA1, 0xD9, 0x44, 0xE3, 0xC5, 0xEB, 0x6C, 0x93, 0x18, 0x28, 0x9B, 0xF4  };
        byte[] MasterResetM3 = { 0x7C, 0xC8, 0xFE, 0xB2, 0x57, 0xDA, 0x9C, 0xD5, 0x09, 0xD1, 0x4B, 0xAB, 0xE4, 0xA1, 0xFB, 0x61 };

        byte[] UnlockResetM1 = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x44 };//lenth 16
        byte[] UnlockResetM2 = { 0x88, 0x9B, 0x71, 0x64, 0x28, 0xBF, 0x0F, 0xD9, 0x9A, 0xBA, 0x27, 0xFC, 0x1F, 0xB1, 0xDE, 0x0D,
                                 0x95, 0x90, 0x81, 0x6E, 0xA1, 0xD9, 0x44, 0xE3, 0xC5, 0xEB, 0x6C, 0x93, 0x18, 0x28, 0x9B, 0xF4  };
        byte[] UnlockResetM3 = { 0xD5, 0x74, 0x33, 0xEB, 0xB0, 0xFF, 0xE6, 0xA8, 0xA6, 0xDD, 0x27, 0x26, 0x43, 0x3D, 0xCB, 0x5A};
        private const byte MasterSlot = 0xFF;
        private const byte UnlockSlot = 0x01;
        private const int KeyBytesM1 = 16;
        private const int KeyBytesM2 = 32;
        private const int KeyBytesM3 = 16;
        private const int RespBytesM4 = 32;
        private const int RespBytesM5 = 16;

        public override void Init()
        {
            结果 = 测试结果.未开始;
            历史结果 = 测试结果.未开始;
            测试数据 = "";
            TestStep = 0;
            LastInitFlagStart = false;
            InitFlagStart = false;
            LastInitFlagStop = false;
            InitFlagStop = false;
            TimeInit();
            FailSum = 0;
            DownLoadResult = 0;
        }
        public override async void Start(负载状态 OpenorClose, 测试方式 TestMethod)
        {
            DateTime intotime = DateTime.Now;
            string 测试数据Temp = "";

            switch (TestStep)
            {
                case 0:
                    结果 = 测试结果.测试中;
                    // 2026-04-27: 先切到运行中，避免定时调用重入重复执行。
                    TestStep = 1;
                    string err = "";
                    string summary = "";
                    WinForm.ecuidInfoModels = new EcuidInfoDal().GetAvailableDataState2(WinForm.产品型号);
                    bool executeOk = await Task.Run(() => ExecuteWriteFlow(out  summary, out  err));
                    if (!executeOk)
                    {
                        测试数据 = err;
                        Log.InsertLog($"写MASTER Unlock Key失败: {err}");
                        结果 = 测试结果.不合格;
                        FailDispose();
                        TestStep = 99;
                    }
                    else
                    {
                        测试数据 = summary;
                        结果 = 测试结果.合格;
                        TestStep = 2;
                    }
                    break;
                case 1:
                    // 2026-04-27: 异步流程进行中，等待case0里的后台任务回填结果。
                    结果 = 测试结果.测试中;
                    break;
                default:
                    break;
            }
            LastInitFlagStart = InitFlagStart;
            LastInitFlagStop = InitFlagStop;

            double endtime = (DateTime.Now - intotime).TotalMilliseconds;
        }

        private bool ExecuteWriteFlow(out string summary, out string error)
        {
            summary = string.Empty;
            error = string.Empty;

            var can = FindClientByName("");
            if (can == null)
            {
                error = "未找到CAN客户端。";
                return false;
            }

            if (!TryParseCanId(WinForm.CANSendID, out uint requestCanId) ||
                !TryParseCanId(WinForm.CANRevID, out uint responseCanId))
            {
                error = $"CAN ID配置异常: SendID={WinForm.CANSendID}, RevID={WinForm.CANRevID}";
                return false;
            }

            if (WinForm.ecuidInfoModels == null || WinForm.ecuidInfoModels.Count == 0 || WinForm.ecuidInfoModels[0] == null)
            {
                error = "未找到可用ECUID数据，请先执行下载/获取流程。";
                return false;
            }

            var info = WinForm.ecuidInfoModels[0];
            if (string.IsNullOrWhiteSpace(info.ECUID))
            {
                error = "当前ECUID为空，无法执行密钥写入。";
                return false;
            }

            if (!TryParseFixedLenHex(info.MKM1, KeyBytesM1, "MKM1", out byte[] mkm1, out error) ||
                !TryParseFixedLenHex(info.MKM2, KeyBytesM2, "MKM2", out byte[] mkm2, out error) ||
                !TryParseFixedLenHex(info.MKM3, KeyBytesM3, "MKM3", out byte[] mkm3, out error) ||
                !TryParseFixedLenHex(info.UKM1, KeyBytesM1, "UKM1", out byte[] ukm1, out error) ||
                !TryParseFixedLenHex(info.UKM2, KeyBytesM2, "UKM2", out byte[] ukm2, out error) ||
                !TryParseFixedLenHex(info.UKM3, KeyBytesM3, "UKM3", out byte[] ukm3, out error))
            {
                return false;
            }

            // 2026-04-27: 按新流程，10/21...29由下位机自动处理，上位机只发送完整UDS服务负载。
            //var keepAliveCts = new CancellationTokenSource();
            //var keepAliveTask = Task.Run(() => TesterPresentLoop(can, requestCanId, keepAliveCts.Token));
            try
            {
                //Log.InsertLog("进入扩展会话");
                //// 1) 进入扩展会话
                //if (!TrySendSingleFrameUds(
                //    can,
                //    requestCanId,
                //    responseCanId,
                //    new byte[] { 0x10, 0x03 },
                //    expectedPositiveSid: 0x50,
                //    expectedPositiveSubFunc: 0x03,
                //    out error))
                //{
                //    error = "进入扩展模式失败: " + error;
                //    return false;
                //}
                Log.InsertLog("关闭应用报文");
                // 2) 关闭应用报文
                if (!TrySendSingleFrameUds(
                    can,
                    requestCanId,
                    responseCanId,
                    new byte[] {  0x28, 0x03, 0x01 },
                    expectedPositiveSid: 0x68,
                    expectedPositiveSubFunc: 0x03,
                    out error))
                {
                    error = "关闭应用报文失败: " + error;
                    return false;
                }
                Log.InsertLog("MASTER写0");
                // 3) MASTER写0（slot=0xFF）
                if (!TryWrite31Key(
                    can, requestCanId, responseCanId,
                    slot: MasterSlot,
                    m1: MasterResetM1, m2: MasterResetM2, m3: MasterResetM3,
                    out string resetM4, out string resetM5, out error))
                {
                    error = "MASTER写0失败: " + error;
                    return false;
                }
                Log.InsertLog("UNLOCK写0");
                // 4) UNLOCK写0（slot=0x01）
                if (!TryWrite31Key(
                    can, requestCanId, responseCanId,
                    slot: UnlockSlot,
                    m1: UnlockResetM1, m2: UnlockResetM2, m3: UnlockResetM3,
                    out string resetUkM4, out string resetUkM5, out error))
                {
                    error = "UNLOCK写0失败: " + error;
                    return false;
                }

                // 5) MASTER写客户值（slot=0xFF）
                if (!TryWrite31Key(
                    can, requestCanId, responseCanId,
                    slot: MasterSlot,
                    m1: mkm1, m2: mkm2, m3: mkm3,
                    out string mkm4, out string mkm5, out error))
                {
                    error = "MASTER写客户值失败: " + error;
                    return false;
                }

                // 2026-04-27: 用户明确要求步骤5与步骤6间隔2秒。
                Thread.Sleep(2000);

                // 6) UNLOCK写客户值（slot=0x01）
                if (!TryWrite31Key(
                    can, requestCanId, responseCanId,
                    slot: UnlockSlot,
                    m1: ukm1, m2: ukm2, m3: ukm3,
                    out string ukm4, out string ukm5, out error))
                {
                    error = "UNLOCK写客户值失败: " + error;
                    return false;
                }

                // 2026-04-27: 写回内存与数据库，为后续上传流程提供M4/M5与有效标志。
                info.MKM4 = mkm4;
                info.MKM5 = mkm5;
                info.UKM4 = ukm4;
                info.UKM5 = ukm5;
                info.MkValid = 1;
                info.UkValid = 1;
                info.UPDATETIME = DateTime.Now;
                info.Upload_Result_Status = "MASTER/UNLOCK写入成功";
                EcuRecord ecuRecord = new EcuRecord();
                ecuRecord.serialNo = WinForm.追溯码;
                ecuRecord.setMkm1(info.MKM1);
                ecuRecord.setMkm2(info.MKM2);
                ecuRecord.setMkm3(info.MKM3);
                ecuRecord.setMkm4(info.MKM4);
                ecuRecord.setMkm5(info.MKM5);
                ecuRecord.setUkm1(info.UKM1);
                ecuRecord.setUkm2(info.UKM2);
                ecuRecord.setUkm3(info.UKM3);
                ecuRecord.setUkm4(info.UKM4);
                ecuRecord.setUkm5(info.UKM5);
                ecuRecord.setEcuid(KeyVerificationCLI.EncodeBase64(info.ECUID));
                List<VerificationResult> verfi = KeyVerificationCLI.IecsLocalVerfication(new List<EcuRecord>() { ecuRecord });
                if (verfi != null && verfi.Count == 1)
                {
                    var mkvalid = verfi[0].getMkValid();
                    var ukvalid = verfi[0].getUkValid();
                  //  return new Tuple<bool, bool, bool, bool>(mtcvalid, ecuidvalid, mkvalid, ukvalid);
                }
                else
                {
                    error = "密钥校验失败。";
                    return false;
                    //   return new Tuple<bool, bool, bool, bool>(ecuidvalid, ecuidvalid, false, false);
                }
                bool dbOk = new EcuidInfoDal().UpdateWriteKeyResult(
                    ecuid: info.ECUID,
                    mkm4: mkm4,
                    mkm5: mkm5,
                    ukm4: ukm4,
                    ukm5: ukm5,
                    mkValid: 1,
                    ukValid: 1,
                    uploadResult: info.Upload_Result_Status);
                if (!dbOk)
                {
                    error = "密钥结果回写数据库失败。";
                    return false;
                }

                summary =
                    $"ECUID:{info.ECUID}, MASTER(M4/M5)={mkm4}/{mkm5}, UNLOCK(M4/M5)={ukm4}/{ukm5}";
                Log.InsertLog($"WriteMASTER_UnlockKey完成: {summary}");
                return true;
            }
            finally
            {
                //keepAliveCts.Cancel();
                //try
                //{
                //    keepAliveTask.Wait(1500);
                //}
                //catch
                //{
                //    // ignore
                //}
                //keepAliveCts.Dispose();
            }
        }

        private void TesterPresentLoop(CANUDPClient can, uint requestCanId, CancellationToken token)
        {
            byte[] testerPresent = { 0x02, 0x3E, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00 };
            while (!token.IsCancellationRequested)
            {
                try
                {
                    can.SendCANMessage(
                        FrameType.ExtendedFrame,
                        requestCanId,
                        SendType.Diagnosis,
                        WinForm.CANType,
                        testerPresent);
                }
                catch
                {
                    // 保活失败不主动中断主流程，由主流程中的关键写入步骤判定最终结果。
                }

                if (token.WaitHandle.WaitOne(1000))
                {
                    break;
                }
            }
        }

        private bool TrySendSingleFrameUds(
            CANUDPClient can,
            uint requestCanId,
            uint responseCanId,
            byte[] payload,
            byte expectedPositiveSid,
            byte? expectedPositiveSubFunc,
            out string error)
        {
            return can.TrySendUdsAndWait(
                frameType: FrameType.ExtendedFrame,
                requestCanId: requestCanId,
                sendType: SendType.Diagnosis,
                canSelectType: WinForm.CANType,
                requestPayload: payload,
                expectedPositiveSid: expectedPositiveSid,   
                expectedPositiveSubFunction: expectedPositiveSubFunc,
                responsePayload: out byte[] _,
                negativeResponseCode: out byte _,
                error: out error,
                p2TimeoutMs: 1000,
                p2StarTimeoutMs: 8000,
                expectedResponseCanId: responseCanId);
        }

        private bool TryWrite31Key(
            CANUDPClient can,
            uint requestCanId,
            uint responseCanId,
            byte slot,
            byte[] m1,
            byte[] m2,
            byte[] m3,
            out string m4Hex,
            out string m5Hex,
            out string error)
        {
            m4Hex = string.Empty;
            m5Hex = string.Empty;
            error = string.Empty;

            if (m1 == null || m1.Length != KeyBytesM1 ||
                m2 == null || m2.Length != KeyBytesM2 ||
                m3 == null || m3.Length != KeyBytesM3)
            {
                error = "M1/M2/M3长度异常，要求16/32/16字节。";
                return false;
            }

            byte[] requestPayload = new byte[5 + KeyBytesM1 + KeyBytesM2 + KeyBytesM3];
            int idx = 0;
            requestPayload[idx++] = 0x31;
            requestPayload[idx++] = 0x01;
            requestPayload[idx++] = 0x02;
            requestPayload[idx++] = 0x72;
            requestPayload[idx++] = slot;
            Buffer.BlockCopy(m1, 0, requestPayload, idx, KeyBytesM1);
            idx += KeyBytesM1;
            Buffer.BlockCopy(m2, 0, requestPayload, idx, KeyBytesM2);
            idx += KeyBytesM2;
            Buffer.BlockCopy(m3, 0, requestPayload, idx, KeyBytesM3);

            if (!can.TrySendUdsAndWait(
                frameType: FrameType.ExtendedFrame,
                requestCanId: requestCanId,
                sendType: SendType.Diagnosis,
                canSelectType: WinForm.CANType,
                requestPayload: requestPayload,
                expectedPositiveSid: 0x71,
                expectedPositiveSubFunction: 0x01,
                responsePayload: out byte[] responsePayload,
                negativeResponseCode: out byte _,
                error: out error,
                p2TimeoutMs: 1000,
                p2StarTimeoutMs: 15000,
                expectedResponseCanId: responseCanId))
            {
                return false;
            }

            if (!TryParse31WriteResponse(responsePayload, slot, out byte[] m4, out byte[] m5, out error))
            {
                return false;
            }

            m4Hex = BytesToHexNoSplit(m4);
            m5Hex = BytesToHexNoSplit(m5);
            return true;
        }

        private bool TryParse31WriteResponse(
            byte[] responsePayload,
            byte slot,
            out byte[] m4,
            out byte[] m5,
            out string error)
        {
            m4 = null;
            m5 = null;
            error = string.Empty;

            if (responsePayload == null || responsePayload.Length == 0)
            {
                error = "31服务响应为空。";
                return false;
            }

            // 2026-04-27: 兼容“纯UDS”与前缀场景，通过匹配固定头定位有效负载。
            int start = FindSequence(
                responsePayload,
                new byte[] { 0x71, 0x01, 0x02, 0x72});
            if (start < 0)
            {
                error = $"31服务响应格式异常，未找到头: 71 01 02 72 ";
                return false;
            }
            start += 5;

            int remain = responsePayload.Length - start;
            if (remain < RespBytesM4 + RespBytesM5)
            {
                error = $"31服务响应长度不足，收到{remain}字节，期望至少{RespBytesM4 + RespBytesM5}字节。";
                return false;
            }

            m4 = new byte[RespBytesM4];
            m5 = new byte[RespBytesM5];
            Buffer.BlockCopy(responsePayload, start, m4, 0, RespBytesM4);
            Buffer.BlockCopy(responsePayload, start + RespBytesM4, m5, 0, RespBytesM5);
            return true;
        }

        private static int FindSequence(byte[] source, byte[] target)
        {
            if (source == null || target == null || source.Length == 0 || target.Length == 0 || source.Length < target.Length)
            {
                return -1;
            }

            for (int i = 0; i <= source.Length - target.Length; i++)
            {
                bool ok = true;
                for (int j = 0; j < target.Length; j++)
                {
                    if (source[i + j] != target[j])
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                {
                    return i;
                }
            }
            return -1;
        }

        private static bool TryParseCanId(string canIdText, out uint canId)
        {
            canId = 0;
            if (string.IsNullOrWhiteSpace(canIdText))
            {
                return false;
            }

            string hex = canIdText.Trim();
            if (hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                hex = hex.Substring(2);
            }
            return uint.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out canId);
        }

        private static bool TryParseFixedLenHex(string raw, int expectBytes, string fieldName, out byte[] bytes, out string error)
        {
            bytes = null;
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(raw))
            {
                error = $"{fieldName}为空。";
                return false;
            }

            string normalized = Regex.Replace(raw, @"[^0-9a-fA-F]", string.Empty);
            if (normalized.Length != expectBytes * 2)
            {
                error = $"{fieldName}长度异常，期望{expectBytes}字节，实际{normalized.Length / 2}字节。";
                return false;
            }

            try
            {
                bytes = new byte[expectBytes];
                for (int i = 0; i < expectBytes; i++)
                {
                    bytes[i] = Convert.ToByte(normalized.Substring(i * 2, 2), 16);
                }
                return true;
            }
            catch (Exception ex)
            {
                error = $"{fieldName}解析失败: {ex.Message}";
                return false;
            }
        }

        private static string BytesToHexNoSplit(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return string.Empty;
            }
            return BitConverter.ToString(data).Replace("-", "");
        }

    }
    public class 周期发送 : 测试项基类
    {
        public ushort index = 0;
        //  public byte TestStep = 0;
        Rule rule = new Rule();
        public DateTime FailTime;
        public string UdpName;
        public string SignalName;
        public int FailSum = 0;
        ushort Arbitration;
        ushort DataSegment;
        string CANSendID;
        FrameType frameType;
        int cycletime = 0;
        public override void Init()
        {
            结果 = 测试结果.未开始;
            历史结果 = 测试结果.未开始;
            测试数据 = "";
            TestStep = 0;
            LastInitFlagStart = false;
            InitFlagStart = false;
            LastInitFlagStop = false;
            InitFlagStop = false;
            TimeInit();
            //ParseWriteOrRead(测试项报文开关);
            //结果判断数据1 ：发送诊断报文ID & 标准帧还是拓展帧& 发送间隔
            //结果判断数据2 ：发送报文 
            //结果判断数据3 : CANfd or can

            var parts = Convert.ToString(结果判断数据1).Split('&');
            CANSendID = Convert.ToString(parts[0]);
            if (Convert.ToString(parts[1]) == "StandardFrame")
                frameType = FrameType.StandardFrame;
            else
                frameType = FrameType.ExtendedFrame;
            cycletime = Convert.ToInt32(parts[2]);
            //var partsID = Convert.ToString(结果判断数据2).Split('&');
            //WinForm.CANSendID = partsID[0];
            //WinForm.CANRevID = partsID[1];
            //if (Convert.ToString(结果判断数据3) == "CAN")
            //    WinForm.CANType = CANSelectType.CAN;
            //else
            //    WinForm.CANType = CANSelectType.CANFD;


            FailSum = 0;
        }
        public override void Start(负载状态 OpenorClose, 测试方式 TestMethod)
        {
            DateTime intotime = DateTime.Now;
            string 测试数据Temp = "";

            switch (TestStep)
            {
                case 0:
                    //if (WinForm.CanCycleInitFlag == true)
                    //{
                    //    结果 = 测试结果.合格;
                    //    TestStep = 3;
                    //    break;
                    //}
                    // SendCANMessage(Mes.FrameType, id, Mes.SendType, CAN初始化设置.CAN_Select, StringToBytes(Mes.Transmitter));
                    Communication.CAN.Message message = new Communication.CAN.Message()
                    {
                        Id = CANSendID,
                        WriteOrRead =  WriteOrRead.Read,
                        FrameType = frameType,
                        SendType =  SendType.Apply,
                        Transmitter = Convert.ToString(结果判断数据2),
                        ReadType =  ReadType.Signals,
                        CycleTime = cycletime
                    };
                    FindClientByName("").周期发送List.Add(message);
                    TestStep++;
                    break;
                case 1:
                    byte StepTemp = StepTime(StartTime, 1000, TestStep);
                    if (StepTemp != 0)
                    {
                        TestStep = StepTemp;
                        StartTime = DateTime.Now;
                    }
                    break;

                case 2:
                    //   if (OpenorClose == 负载状态.开启)
                    {
                  //      FindClientByName("").CanIDInitSend(WinForm.CANRevID);
                        WinForm.CanCycleInitFlag = true;
                        结果 = 测试结果.合格;
                        TestStep++;

                    }
                    break;
            }
            LastInitFlagStart = InitFlagStart;
            LastInitFlagStop = InitFlagStop;

            double endtime = (DateTime.Now - intotime).TotalMilliseconds;
        }

    }
    public class 上传GM : 测试项基类
    {
        public ushort index = 0;
        //  public byte TestStep = 0;
        Rule rule = new Rule();
        public DateTime FailTime;
        public string UdpName;
        public string SignalName;
        public int FailSum = 0;
        ushort Arbitration;
        ushort DataSegment;
        string CANSendID;
        FrameType frameType;
        int cycletime = 0;
        DataSet data2;
        private int uploadState = 0;
        public override void Init()
        {
            结果 = 测试结果.未开始;
            历史结果 = 测试结果.未开始;
            测试数据 = "";
            TestStep = 0;
            LastInitFlagStart = false;
            InitFlagStart = false;
            LastInitFlagStop = false;
            InitFlagStop = false;
            TimeInit();
            //ParseWriteOrRead(测试项报文开关);

            data2 = new DataSet();


            FailSum = 0;
        }
        public override void Start(负载状态 OpenorClose, 测试方式 TestMethod)
        {
            DateTime intotime = DateTime.Now;
            string 测试数据Temp = "";

            switch (TestStep)
            {
                case 0:
                    data2 = new DataSet("ecuRecordList");
                    var dt = new DataTable("ecuRecord");
                    data2.Tables.Add(dt);
                    dt.Columns.Add(new DataColumn("ecuid"));
                    dt.Columns.Add(new DataColumn("serialNo"));
                    dt.Columns.Add(new DataColumn("mkm1"));
                    dt.Columns.Add(new DataColumn("mkm2"));
                    dt.Columns.Add(new DataColumn("mkm3"));
                    dt.Columns.Add(new DataColumn("mkm4"));
                    dt.Columns.Add(new DataColumn("mkm5"));
                    dt.Columns.Add(new DataColumn("ukm1"));
                    dt.Columns.Add(new DataColumn("ukm2"));
                    dt.Columns.Add(new DataColumn("ukm3"));
                    dt.Columns.Add(new DataColumn("ukm4"));
                    dt.Columns.Add(new DataColumn("ukm5"));
                   // foreach (DataRow item in data)
                    {
                        var row = dt.NewRow();
                        row["ecuid"] = Hex2ToBase64(WinForm.ecuidInfoModels[0].ECUID);
                        row["serialNo"] = WinForm.追溯码;//;
                        row["mkm1"] = Hex2ToBase64(WinForm.ecuidInfoModels[0].MKM1);
                        row["mkm2"] = Hex2ToBase64(WinForm.ecuidInfoModels[0].MKM2);
                        row["mkm3"] = Hex2ToBase64(WinForm.ecuidInfoModels[0].MKM3);
                        row["mkm4"] = Hex2ToBase64(WinForm.ecuidInfoModels[0].MKM4);
                        row["mkm5"] = Hex2ToBase64(WinForm.ecuidInfoModels[0].MKM5);
                        row["ukm1"] = Hex2ToBase64(WinForm.ecuidInfoModels[0].UKM1);
                        row["ukm2"] = Hex2ToBase64(WinForm.ecuidInfoModels[0].UKM2);
                        row["ukm3"] = Hex2ToBase64(WinForm.ecuidInfoModels[0].UKM3);
                        row["ukm4"] = Hex2ToBase64(WinForm.ecuidInfoModels[0].UKM4);
                        row["ukm5"] = Hex2ToBase64(WinForm.ecuidInfoModels[0].UKM5);

                        dt.Rows.Add(row);
                       // ecuidList.Add(new Model.EcuidInfoModel() { ECUID = item["ECUID"].ToString() });
                    }

                    //生成文件-{WinForm.追溯码}
                    string fileName = $"{WinForm.产品型号}-{DateTime.Now.ToString("yyyyMMdd-HHmmss")}.xml";
                    string xml = string.Empty;
                    try
                    {
                        DirectoryInfo dir = new DirectoryInfo("xmlbak");
                        if (!dir.Exists) dir.Create();
                        //生成xml文件
                        data2.WriteXml(Path.Combine(dir.FullName, fileName));
                        xml = File.ReadAllText(Path.Combine(dir.FullName, fileName));
                        //更新数据状态
                        new EcuidInfoDal().UpdateUploadStateBatch(WinForm.ecuidInfoModels, true);

                    }
                    catch (Exception ex)
                    {
                        uploadState = 3;
                        TestStep = 1;
                        Log.InsertLog($"生成文件失败:{ex.Message}");
                        break;
                    }
                   
                    Task task = new Task(() =>
                    {
                        //Model.ProductionTaskModel productionTask = _productionTask;
                        try
                        {
                            uploadState = 1;
                            GmApi.IIecsApi api = GmApi.IecsApi.GetIecsApi(WinForm.SeverName);
                            var res = default(GmApi.Models.Verification.BaseResponseModel);


                            res = api.CreateVerificationRequset();
                            if (res.errorResponse != null)
                            {
                                uploadState = 3;
                                Log.InsertLog($"申请Id失败。error:" + res.errorResponse);
                                return;
                            }
                            string requestId = res.requestId;
                            System.Diagnostics.Debug.WriteLine(requestId);

                            res = api.UploadVerificationFile(requestId, xml, fileName);
                            if (res.errorResponse != null)
                            {
                                Log.InsertLog($"上传XML失败。error:" + res.errorResponse);
                                return;
                            }
                            for (int i = 0; ; i++)
                            {
                                System.Threading.Thread.Sleep(2000);
                                res = api.GetVerificationRequset(requestId);
                                switch ((res as GmApi.Models.Verification.GetInfoResponseModel).status)
                                {
                                    case "FAILED":
                                        uploadState = 3;
                                        Log.InsertLog($"获取GM服务器处理出错。error:" + res.errorResponse);
                                        return;
                                    case "success":
                                        //保存返回的文件
                                        DirectoryInfo dir = new DirectoryInfo("xmlbak");
                                        if (!dir.Exists) dir.Create();

                                        string resFileName = Path.Combine(dir.FullName, (res as GmApi.Models.Verification.GetInfoResponseModel).fileName);
                                        if (File.Exists(resFileName))
                                            resFileName = resFileName.Remove(resFileName.Length - 4, 4) + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xml";
                                        File.WriteAllText(resFileName, (res as GmApi.Models.Verification.GetInfoResponseModel).fileContent);
                                        uploadState = 2;
                                        //处理返回的数据
                                         ProcessVerifyResultData( resFileName, true);
                                        return;
                                    default:
                                        if (i > 60)
                                        {
                                            Log.InsertLog($"获取结果。error:{(res as GmApi.Models.Verification.GetInfoResponseModel).status}, {res.errorResponse}");
                                            uploadState = 3;
                                            return;
                                        }
                                        break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //Utils.Logger.Debug(ex.ToString());
                            uploadState = 3;
                            Log.InsertLog($"上传XML失败。error:" + ex.Message);
                        }
                    });
                    //_autoUploadTasks.Add(new KeyValuePair<DateTime, Task>(DateTime.Now, task));
                    task.Start();
                    TestStep++;
                    break;
                case 1:
                    if(uploadState == 2)
                    {
                        结果 = 测试结果.合格;
                        TestStep++;
                    }
                    if (uploadState == 3)
                    {
                        结果 = 测试结果.不合格;
                        TestStep++;
                    }
                    break;

                case 2:

                    break;
            }
            LastInitFlagStart = InitFlagStart;
            LastInitFlagStop = InitFlagStop;

            double endtime = (DateTime.Now - intotime).TotalMilliseconds;
        }
        public static string Hex2ToBase64(string hex)
        {
            var str = hex.Replace(" ", "");
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < str.Length; i += 2)
            {
                bytes.Add(Convert.ToByte(str.Substring(i, 2), 16));
            }

            return Convert.ToBase64String(bytes.ToArray());
        }
        /// <summary>
        ///  处理回传状态文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="xmlFilePath"></param>
        /// <param name="isAuto"></param>
        private void ProcessVerifyResultData( string xmlFilePath, bool isAuto)
        {

            DataSet ds = new DataSet();
            try
            {
                ds.ReadXml(xmlFilePath);
            }
            catch (Exception ex)
            {
                Log.InsertLog($"打开XML文件失败，{ex.Message}。");
                return;
            }

            if (ds?.Tables["ValidationRecord"]?.TableName.ToUpper() != "VALIDATIONRECORD")
            {
                Log.InsertLog($"请打开正确的XML数据。");
                return;
            }
            List<EcuidInfoModel> ecuidInfos = new List<EcuidInfoModel>();
            // 文件标记为只读
            File.SetAttributes(xmlFilePath, FileAttributes.ReadOnly);

            int duplicationCount = 0;
            foreach (DataRow item in ds.Tables["ValidationRecord"]?.Rows)
            {
                EcuidInfoModel ecuidInfo = new EcuidInfoModel();
                ecuidInfo.ECUID = Base64ToHex(item["ecuid"].ToString());
                try
                {
                    ecuidInfo.MkValid = (byte)(item["MkValid"]?.ToString().ToUpper() == "TRUE" ? 1 : 0);
                }
                catch { ecuidInfo.MkValid = 255; }
                try
                {
                    ecuidInfo.UkValid = (byte)(item["UkValid"]?.ToString().ToUpper() == "TRUE" ? 1 : 0);
                }
                catch { ecuidInfo.UkValid = 255; }
                ecuidInfo.Upload_Result_Status = item["status"]?.ToString() ?? "";
                if ((ecuidInfo.MkValid == 1 && ecuidInfo.UkValid == 1) || ecuidInfo.Upload_Result_Status == "valid")
                {
                    ecuidInfo.UploadState = EcuidInfoModel.UploadStateType.验证成功 + (byte)(isAuto ? EcuidInfoModel.UploadStateType.自动上传 : EcuidInfoModel.UploadStateType.手动上传);
                }
                else if (ecuidInfo.Upload_Result_Status == "ECUID_VERIFICATION_LIMIT_REACHED")
                {
                    ecuidInfo.UkValid = 2;
                    ecuidInfo.MkValid = 2;
                    duplicationCount++;
                    ecuidInfo.UploadState = EcuidInfoModel.UploadStateType.验证成功 + (byte)(isAuto ? EcuidInfoModel.UploadStateType.自动上传 : EcuidInfoModel.UploadStateType.手动上传);
                }
                else
                {
                    ecuidInfo.UploadState = EcuidInfoModel.UploadStateType.验证失败 + (byte)(isAuto ? EcuidInfoModel.UploadStateType.自动上传 : EcuidInfoModel.UploadStateType.手动上传);
                }
                ecuidInfos.Add(ecuidInfo);
            }
            if (new EcuidInfoDal().UpdateGmServerResultBatch(ecuidInfos))
            {
                Log.InsertLog($"操作成功，其中验证成功【{ds.Tables["validationRecords"].Rows[0]["ValidRecords"]}条】，无效【{ds.Tables["validationRecords"].Rows[0]["InvalidRecords"]}条】，到达验证上限【{duplicationCount}条】，Unchecked【{Convert.ToInt32(ds.Tables["validationRecords"].Rows[0]["UncheckedRecords"]) - duplicationCount}条】");
            }
            else
            {
                Log.InsertLog($"操作失败，存在错误数据。");
            }
        }
        public static string Base64ToHex(string base64)
        {
            try
            {
                return BytesToHex(Convert.FromBase64String(base64));
            }
            catch
            {
              //  Utils.Logger.Debug($"【Base64ToHex】无效的Base64:{base64}");
                return "";
            }
        }
        public static string BytesToHex(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in bytes)
            {
                sb.Append(item.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
