using JLRScan.Frm;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;


namespace JLRScan
{
    [Serializable]
    public class MesSetting
    {
        //[DisplayName("型号名称"), PropertyOrder(1)]
        //public string 型号名称 { get; set; }
        //[DisplayName("零件号"), PropertyOrder(2)]
        //public string 零件号 { get; set; }

    
        [TypeConverter(typeof(ClassConverter<DID_Numb>)) ,DisplayName("序列号位置"), PropertyOrder(10),Browsable(false)]
        public DID_Numb 序列号位置 { get; set; } = new DID_Numb();
        [DisplayName("序列号"), PropertyOrder(11)]
        internal string PrintSerialNumMem { get; set; }
        [DisplayName("打印方法"), PropertyOrder(12)]
        public 打印方式 打印方法 { get; set; }
        [TypeConverter(typeof(ClassConverter<PrintSetting>)) ,DisplayName("打印配置"), PropertyOrder(13)]
        public PrintSetting 打印配置 { get; set; } = new PrintSetting();
    }
    // [Serializable, TypeConverter(typeof(ExpandableObjectConverter))/*, TypeConverter(typeof(ClassConverter<DID_Numb>))*/]// TypeConverter(typeof(ExpandableObjectConverter))]
 
    public class DID_Numb
    {
       [ DisplayName("起始位置")]
        public int 起始位置 { get; set; }
        [DisplayName("长度")]
        public int 长度 { get; set; }
    }
    public enum 打印方式
    {
        frx = 0,
        btw = 1,
    }
  

}
