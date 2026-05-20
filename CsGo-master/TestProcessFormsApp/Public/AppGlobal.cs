using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JLRScan
{
    public class AppGlobal
    {
     //   public static Dictionary<string, PrintParm> PrintLog { set; get; } = new Dictionary<string, PrintParm>();
        public static FormStateEnum FrmState { set; get; } = FormStateEnum.Normal;

        public static bool 天线学习标志 { set; get; } = false;
        //public static List<TestResult> TestResults = new List<TestResult>();
        //public static TestResult CurrentTestResult = new TestResult();
        public static int workid { set; get; }
        public  enum FuncName
        {
            _MesGetSapOrderNub = 0,
            _MesFeedingScan,
            _MesPCBALock,
            _MesSkipCurrentBatch,
        }
  
        public static string ClearScanString { get; set; } = "清除日志消息";

        #region 扫描枪相关变量
        /// <summary>
        /// 串口输入结束符号
        /// </summary>
        public static byte ScanInputSuffix = 0x0D;
        /// <summary>
        /// 第二个结束符
        /// </summary>
        public static byte ScanInputSuffixTwo = 0x0A;
        /// <summary>
        /// 扫描枪输入类
        /// </summary>
        public class ScanInputClass
        {
            public string InputString;
            public DateTime Time;
        }
        public static string EncodingStr = "UTF-8";
        /// <summary>
        /// 扫描枪输入缓冲队列
        /// </summary>
        public static ConcurrentQueue<ScanInputClass> InputQueue = new ConcurrentQueue<ScanInputClass>();
        #endregion
        public static class UIntExtensions
        {
            public static string ToLittleEndianHex(uint value)
            {
                return $"0x{(value & 0xFF).ToString("X2")}{((value >> 8) & 0xFF).ToString("X2")}{((value >> 16) & 0xFF).ToString("X2")}{(value >> 24).ToString("X2")}";
            }
        }
    }
    public enum FormStateEnum
    {
        Normal = 0,
        error = 1,
    }
    public enum TestResult
    {
        Wait = 0,
        Testing = 1,
        Pass = 2,
        Fail = 3
    }
    public static class Myextension
    {
        internal static string FormatWith(this string format, params object[] parm1)
        {
            if (format == null)
                return null;
            else
                return string.Format(format, parm1);
        }
        internal static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
        internal static string ToHexString(this string str)
        {
            if (str.IsNullOrEmpty())
                return null;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                sb.Append(((ushort)str[i]).ToString("X")).Append('-');
            }
            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
    public class ClassConverter<T> : Frm.PropertySorter
        where T : new()
    {
        /// <summary>
        /// 返回该转换器是否可以使用指定的上下文将该对象转换为指定的类型
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
            //if (destinationType == typeof(T))
            //{
            //    return true;
            //}

            //return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// 返回该转换器是否可以使用指定的上下文将给定类型的对象转换为此转换器的类型
        /// </summary>
        /// <param name="context">提供有关组件的上下文，如其容器和属性描述符</param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
            //if (sourceType == typeof(object))
            //{
            //    return true;
            //}

            //return base.CanConvertFrom(context, sourceType);
        }
        /// <summary>
        /// 使用指定的上下文和区域性信息将给定的值对象转换为指定的类型
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture,
            object value, Type destinationType)
        {
            //if (destinationType == typeof(T) &&
            //    value is T str)
            //{
            //return value.ToString();
            //}
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {

            return base.GetProperties(context, value, attributes);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            //if (value is string s)
            //{
            //    T t = new T();
            //    t.SetValue(s);
            //    return t;
            //}
            return base.ConvertFrom(context, culture, value);
        }

    }
    //public class PrintParm
    //{
    //    public string 序列号 { get; set; }
    //    public DateTime OperationTime { get; set; }
    //}
    //public class TestResult
    //{
    //    public string 条码 { get; set; } = "";
    //    public string[] 零件 { get; set; }
    //    public ScanState 测试状态 { get; set; }
    //    public DateTime OperationTime { get; set; }
    //    public int id { get; set; }
    //}
}
