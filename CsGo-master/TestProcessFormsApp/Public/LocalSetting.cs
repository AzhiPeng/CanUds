using JLRScan.Frm;
using JLRScan;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TestProcessFormsApp.Communication.CAN;

namespace TestProcessFormsApp.Public
{
    public enum 主从关系
    {
        主 = 0,
        从 = 1
    }
    internal class LocalSetting
    {
        public static LocalSetting localSetting = new LocalSetting();
        [Browsable(false)]
        public static string SystemDIR { get; set; } = System.Windows.Forms.Application.StartupPath;
        //[DisplayName("485下位机端口号，例如COM1")]
        //public string _485port { get; set; }
        [DisplayName("下位机端口号，例如5900")]
        public int port { get; set; }
        [DisplayName("下位机ip")]
        public string CAEA008IP { get; set; }
        //[DisplayName("本机 IP")]
        //public string MyIp { get; set; }
        //[DisplayName("数据库名称")]
        //public string DBName { get; set; }
        //[DisplayName("数据库地址")]
        //public string ServerName { get; set; }
        [Browsable(false)]
        [DisplayName("VBAT_Signal")]
        public string VBAT_Signal { get; set; }
        [Browsable(false)]
        [DisplayName("VBAT_UdpName")]
        public string VBAT_UdpName { get; set; }
        [DisplayName("测试结果路径")]
        public string FileNamePath { get; set; }
        //[DisplayName("板卡ADC数量")]
        //public int AdcSum { get; set; }
        //[DisplayName("板卡OTP数量")]
        //public int OtpSum { get; set; }
        [DisplayName("当前工位")]
        public string CurrentGw { get; set; }
        [DisplayName("前工位")]
        public string FrontGw { get; set; }

        public string 检测号 { get; set; }
        public string 班组号 { get; set; }
        [Browsable(false)]
        public string 电脑编号 { get; set; }
        [Browsable(false)]
        public DateTime PrintSerialNumMemTime;
        [Browsable(false)]
        public string PrintSerialNumMem;// PrintSerialNumMem
        public string PrintSerialNum//序列号
        {
            get;
            set;
        }
    
        [TypeConverter(typeof(ClassConverter<DID_Numb>)), DisplayName("序列号位置"), PropertyOrder(10), Browsable(false)]
        public DID_Numb 序列号位置 { get; set; } = new DID_Numb();
   
        [DisplayName("打印方法")]
        public 打印方式 打印方法 { get; set; }
        [TypeConverter(typeof(ClassConverter<PrintSetting>)), DisplayName("打印配置"), PropertyOrder(13)]
        public PrintSetting 打印配置 { get; set; } = new PrintSetting();
        [DisplayName("位置偏移X(mm)"), Description("当标签模板为btw时，X为左边距")]
        public double OffsetX { set; get; }
        [DisplayName("位置偏移Y(mm)"), Description("当标签模板为btw时，Y为上边距")]
        public double OffsetY { set; get; }
        [DisplayName("点检模式")]
        public bool DianJianState { set; get; } = false;
        [DisplayName("测试次数")]
        public int TestNumb { set; get; }  = 0;
        [Browsable(false)]
        public string 扫码枪端口 { get; set; } = "COM1";
        [Browsable(false)]
        public int 扫码枪波特率 { get; set; } = 9600;
    }
    public enum 打印方式
    {
        frx = 0,
        btw = 1,
    }
}
