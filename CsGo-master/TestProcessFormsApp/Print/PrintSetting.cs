
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using CCWin.SkinControl;

namespace JLRScan
{
    public class PrintSetting
    {
        [DisplayName("打印序列号"), XmlIgnore]
        public string PrintSerialNum
        {
            get; /*{ return Form1.barTenderPrint.SerialNum; }*/
            set;
            //{
            //    if (value != null && Form1.barTenderPrint.SerialNum.Length == value.Length)
            //        Form1.barTenderPrint.SerialNum = value;
            //}
        }
        [DisplayName("位置偏移X(mm)"), Description("当标签模板为btw时，X为左边距")]
        public double OffsetX { set; get; }
        [DisplayName("位置偏移Y(mm)"), Description("当标签模板为btw时，Y为上边距")]
        public double OffsetY { set; get; }

        [DisplayName("标签宽度(mm)"),Browsable(false)]
        public double PaperWidth { set; get; }
        [DisplayName("标签高度(mm)"), Browsable(false)]
        public double PaperHeight { set; get; }
        [DisplayName("打印深度")]
        public int PrintDarkness { set; get; }
        [DisplayName("禁用转义字符"), Browsable(false)]
        public bool DisableUnescapeFlag { get; set; }
        [DisplayName("默认字体"), Browsable(false)]
        public string DefaultFont { get; set; }
        [DisplayName("条码设定"), Browsable(false)]
        public DataMatrixInfo[] DataMatrix { get; set; }
        [DisplayName("字符串设定"), Browsable(false)]
        public PrintStringInfo[] Strings { get; set; }
        public PrintImgInfo[] imgs { get; set; }
        [Browsable(false)]
        public string 序列号递增模式 { get; set; }
        [Browsable(false)]
        public string 二维码生成模式 { get; set; }

        public override string ToString()
        {
            //if (Settings.Default.打印配置.打印启用)
            //    if (打印模板文件.IsNullOrEmpty())
            //        return "未配置打印模板文件";
            //    else
            //        return 打印模板文件;
            //else
            //    return "打印未启用";
            return "";
        }

        internal void ApplyNewSettings(PrintSetting ns)
        {
            if (ns == null)
                return;
            PaperWidth = ns.PaperWidth;
            PaperHeight = ns.PaperHeight;
            PrintDarkness = ns.PrintDarkness;
            DisableUnescapeFlag = ns.DisableUnescapeFlag;
            DefaultFont = ns.DefaultFont;
            DataMatrix = ns.DataMatrix;
            Strings = ns.Strings;
            imgs = ns.imgs;
            DefaultFont = ns.DefaultFont;
            序列号递增模式 = ns.序列号递增模式;
            二维码生成模式 = ns.二维码生成模式;
        }
        public string[] GetSubStrings()
        {
            List<string> strs = new List<string>();
            for (int i = 0; i < DataMatrix?.Length; i++)
            {
                for (int j = 0; j < DataMatrix[i].strings?.Length; j++)
                {
                    if (DataMatrix[i].strings[j].SubName.IsNullOrEmpty() == false)
                    {
                        strs.Add(DataMatrix[i].strings[j].SubName);
                        strs.Add(DataMatrix[i].strings[j].Content);
                    }
                }
            }
            for (int i = 0; i < Strings?.Length; i++)
            {
                for (int j = 0; j < Strings[i].strings?.Length; j++)
                {
                    if (Strings[i].strings[j].SubName.IsNullOrEmpty() == false)
                    {
                        strs.Add(Strings[i].strings[j].SubName);
                        strs.Add(Strings[i].strings[j].Content);
                    }
                }
            }
            return strs.ToArray();
        }

        public void Save(string ProfilePath)
        {
            XmlSerializer ser = new XmlSerializer(typeof(PrintSetting));
            {
                TextWriter writer = new StreamWriter(ProfilePath, false);
                ser.Serialize(writer, this);
                writer.Close();
            }
        }
        public MemoryStream GetBuf()
        {
            var mem = new MemoryStream(4096);
            XmlSerializer ser = new XmlSerializer(typeof(PrintSetting));
            {
                TextWriter writer = new StreamWriter(mem);
                ser.Serialize(writer, this);
            }
            return mem;
        }
        public static PrintSetting Load(byte[] buf)
        {
            if (buf == null || buf.Length == 0)
                return null;
            MemoryStream ms = new MemoryStream(buf);
            XmlSerializer ser = new XmlSerializer(typeof(PrintSetting));
            XmlTextReader reader = new XmlTextReader(ms);
            PrintSetting obj = (PrintSetting)(ser.Deserialize(reader));
            reader.Close();
            if (obj != null)
                return obj;
            return null;
        }
        public static PrintSetting Load(string ProfilePath)
        {
            if (File.Exists(ProfilePath))
            {
                FileStream fs = new FileStream(ProfilePath, FileMode.Open);
                if (fs != null)
                {
                    XmlSerializer ser = new XmlSerializer(typeof(PrintSetting));
                    try
                    {
                        XmlTextReader reader = new XmlTextReader(fs);
                        PrintSetting obj = (PrintSetting)(ser.Deserialize(reader));
                        reader.Close();
                        if (obj != null)
                            return obj;
                    }
                    catch (Exception ex)
                    {
                        fs.Close();
                        throw ex;
                    }
                }
            }
            return null;
        }


    }
    public struct PrintImgInfo
    {
        public PrintImgInfo(double x, double y, double w, double h, string pic)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
            this.pic = pic;
        }
        public double x { get; set; }
        public double y { get; set; }
        public double w { get; set; }
        public double h { get; set; }
        public string pic;
    }
    public struct PrintStringInfo
    {
        public PrintStringInfo(double x, double y, StringContentInfo[] strings)
        {
            this.x = x;
            this.y = y;
            this.strings = strings;
            size = 0;
            scaleX = 0;
            Font = null;
            FontWeight = 0;
            rotate = 0;
        }
        public PrintStringInfo(double x, double y, StringContentInfo[] strings, byte FontWeight, double size = 0, double scaleX = 0, int rotate = 0)
        {
            this.x = x;
            this.y = y;
            this.strings = strings;
            this.size = size;
            this.scaleX = scaleX;
            Font = null;
            this.FontWeight = FontWeight;
            this.rotate = rotate;
        }
        public double x { get; set; }
        public double y { get; set; }
        [DisplayName("字号(6)")]
        public double size { get; set; }
        [DisplayName("水平拉伸")]
        public double scaleX { get; set; }
        [DisplayName("字体(默认Arial)")]
        public string Font { get; set; }
        [DisplayName("加粗(1-7,默认4)")]
        public byte FontWeight { get; set; }
        [DisplayName("旋转")]
        public int rotate { get; set; }
        [DisplayName("内容")]
        public StringContentInfo[] strings { get; set; }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < strings?.Length; i++)
            {
                sb.Append(strings[i].Content);
            }
            return sb.ToString();
        }
    }
    public struct StringContentInfo : IComparable<StringContentInfo>
    {
        public StringContentInfo(string content)
        {
            this.content = content;
            SubName = null;
            Size = 0;
        }
        public StringContentInfo(string content, string subname, double size = 0)
        {
            this.content = content;
            SubName = subname;
            Size = size;
        }
        internal static Dictionary<string, string> SubStringDic
        {
            get
            {
                //if (Cognex.VisionPro.JobManager.barTenderPrint != null)
                //    return Cognex.VisionPro.JobManager.barTenderPrint.Substrings;
                return null;
            }
        }
        internal string content;
        public string Content
        {
            get
            {
                string val;
                if (SubName.IsNullOrEmpty() == false && SubStringDic != null && SubStringDic.TryGetValue(SubName, out val))
                    return val;
                return content;
            }
            set
            {
                if (SubName.IsNullOrEmpty() == false && SubStringDic != null)
                    if (SubStringDic.ContainsKey(SubName))
                        SubStringDic[SubName] = value;
                    else
                        SubStringDic.Add(SubName, value);
                content = value;
            }
        }
        public string SubName { get; set; }
        public double Size { get; set; }
        public override string ToString()
        {
            return Content;
        }

        public int CompareTo(StringContentInfo other)
        {
            double sz1 = Size <= 0 ? 6 : Size;
            double sz2 = other.Size <= 0 ? 6 : other.Size;
            if (sz1 > sz2)
                return 1;
            else if (sz1 == sz2)
                return 0;
            else
                return -1;
        }
    }
    public struct DataMatrixInfo
    {
        public double x { get; set; }
        public double y { get; set; }
        [DisplayName("二维码类型")]
        public string type { get; set; }
        [DisplayName("点阵大小(毫米)")]
        public double MatrixHeight { get; set; }
        [DisplayName("内容")]
        public StringContentInfo[] strings { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < strings?.Length; i++)
            {
                sb.Append(strings[i].Content);
            }
            return sb.ToString();
        }
    }
}
