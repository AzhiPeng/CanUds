using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Management;
using CCWin.SkinControl;
using TestProcessFormsApp.Public;
using TestProcessFormsApp;
using TestProcessFormsApp.Properties;


namespace JLRScan
{
    public class BarTenderPrint : PrintBase
    {
        static BarTender.Application barApplication;
        internal static BarTender.Format barFormat;
        internal static DateTime lastWriteTime;
        ScanLog scanLog = new ScanLog();
        internal override void Print(string[] extras)
        {
            LastPrintSuccess = false;
            if (LocalSetting.localSetting.打印方法 == TestProcessFormsApp.Public.打印方式.frx)
            {
                if (Substrings.Count == 0)
                    LoadBtw(null);
                if (report != null)
                    LastPrintSuccess = PrintFrx(extras);
                //else
                //    PrintSDK(extras);
                return;
            }
            if (barFormat != null)
            {
                try
                {
                    if (extras != null)
                        for (int i = 0; i < extras.Length / 2; i++)
                        {
                            if (string.IsNullOrEmpty(extras[i * 2]) == false)
                            {
                                string sval = null;
                                if (Substrings.TryGetValue(extras[i * 2], out sval))
                                {
                                    if (string.IsNullOrEmpty( sval))
                                    {
                                        sval = barFormat.GetNamedSubStringValue(extras[i * 2]);
                                        Substrings[extras[i * 2]] = sval;
                                    }
                                    string str = extras[i * 2 + 1];
                                    if (str.Length != sval.Length || str.IndexOf('\x0') >= 0)
                                        throw new Exception("打印内容与模版不匹配：" + extras[i * 2] +"\r\n模板："+sval+"\r\n打印内容："+ str + "\r\n");//FormatWith(sval, str.ToHexString(), str, extras[i * 2]));
                                    barFormat.SetNamedSubStringValue(extras[i * 2], str);
                                }
                                try
                                {
                                  
                                    if (extras[i * 2] == "序列号")
                                    {
                                        int val;
                                        if (int.TryParse(extras[i * 2 + 1], out val))
                                        {
                                            if (val <= 0)
                                            {
                                                throw new Exception("序列号错误：" + extras[i * 2 + 1]);
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("序列号解析错误：" + extras[i * 2 + 1]);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                            }
                        }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                var ptname = barFormat.Printer;
                if (ptname.IndexOf("FAX") > 0 || ptname.IndexOf("PDF") > 0)
                {
                    scanLog.InsertLog($"打印机选择错误：{ptname}\r\n请检查默认打印机设定和标签模板中的打印机设定");
                    LastPrintSuccess = false;
                    return;
                }
                //Program.MsgBox(ptname);
                var cmd = $"SELECT * FROM Win32_Printer where Name = '{ptname}'";
                var sch = new System.Management.ManagementObjectSearcher(cmd);
                var prts = sch.Get();
                var enu = prts.GetEnumerator();
                if (!enu.MoveNext())
                {
                    scanLog.InsertLog($"无法找到打印机{ptname}");
                    LastPrintSuccess = false;
                    return;
                }
                else
                {
                    if ((bool)enu.Current.Properties["WorkOffline"].Value)
                    {
                        scanLog.InsertLog($"打印机 {ptname} 脱机，请检查打印机连接");
                        LastPrintSuccess = false;
                        return;
                    }
                    //else
                    //Program.MsgBox(enu.Current.Properties["WorkOffline"].Value.ToString());
                }
                barFormat.PageSetup.MarginTop = (float)PrintSettings.OffsetY;
                barFormat.PageSetup.MarginLeft = (float)PrintSettings.OffsetX;
                barFormat.PrintOut(true);
                LastPrintSuccess = true;
                if (printTestFlag == false)
                {
                    string snum = SerialNum;
                    if (string.IsNullOrEmpty(snum) == false)
                    {
                        LocalSetting.localSetting.PrintSerialNumMem = snum;
                       // Setting.SaveDefault();
                    }
                }
            }
            else
            {
                scanLog.InsertLog("未加载打印样式文件");
                LastPrintSuccess = false;
            }
        }

        internal override void LoadBtw(string path)
        {
            try
            {
                string PrevSerial = null;
                if (LocalSetting.localSetting.打印方法 == TestProcessFormsApp.Public.打印方式.frx)
                {
                    try
                    {
                        LoadFrx(path,false);
                    }
                    catch (Exception ex)
                    {
                        scanLog.InsertLog(ex.Message);
                        if (report != null)
                        {
                            report.Dispose();
                            report = null;
                        }
                        //LoadSTDModel(path);
                    }
                    return;
                }
                if (barApplication == null)
                {
                    barApplication = new BarTender.Application();
                    reStartFlag = false;
                }
                else if (reStartFlag)
                {
                    barApplication.Quit(BarTender.BtSaveOptions.btDoNotSaveChanges);
                    barApplication = new BarTender.Application();
                    barFormat = null;
                    reStartFlag = false;
                }
                else
                    PrevSerial = SerialNum;
                if (path == null)
                {
                    //if (Setting.Default.型号配置?.Length > Setting.Default.型号ID)
                    //    path = Setting.Default.型号配置[Setting.Default.型号ID].打印模板文件;
                    if (String.IsNullOrEmpty(path))
                        path = WinForm.产品型号;
                }

                string path2 = $"{LocalSetting.SystemDIR}\\FrxBtw\\{path}";
               
                if (File.Exists(path2) == false)
                {
                    path2 = $"{LocalSetting.SystemDIR}\\FrxBtw\\{path}.btw";
                    if (File.Exists(path2) == false)
                        path2 = $"{LocalSetting.SystemDIR}\\FrxBtw\\{path}.BTW";
                    //if (File.Exists(path2) == false)
                    //{
                    //    object obj = File.OpenWrite(path); 
                    //  //  if (Zip.PrintDic.TryGetValue(path, out object obj))
                    //    {
                    //        if (obj is byte[] buf)
                    //        {
                    //            path2 = $"{LoadingSetting.SystemDIR}\\FrxBtw\\{path}.btw";
                    //            if (LocalSetting.localSetting.OffsetX == xoffsetMem && LocalSetting.localSetting.OffsetY == yoffsetMem
                    //        && loadTimeMem != DateTime.MinValue && loadTimeMem == lastWriteTime && loadNameMem == path2)
                    //                return;
                    //            else
                    //            {
                                    
                    //                File.WriteAllBytes(path2, buf);
                    //                File.SetLastWriteTime(path2,lastWriteTime);
                    //                loadTimeMem = DateTime.MinValue;
                    //            }
                    //        }
                    //        else if (obj is MemoryStream ms)
                    //        {//TODO
                    //            throw new NotImplementedException();
                    //        }
                    //    }
                    //}
                }

                if (File.Exists(path2))
                {
                    var fl = new FileInfo(path2);
                    //if (LocalSetting.localSetting.OffsetX == xoffsetMem && LocalSetting.localSetting.OffsetY == yoffsetMem
                    //    && fl.LastWriteTime == loadTimeMem && loadNameMem == path2)
                    //    return;
                   /* else*/ if (barFormat != null)
                    {
                        barFormat.Close(BarTender.BtSaveOptions.btDoNotSaveChanges);
                        barFormat = null;
                    }

                    barFormat = barApplication.Formats.Open(Path.GetFullPath(path2));
                    barFormat.NumberSerializedLabels = 1;
                    //if (Setting.Default.打印配置.打印张数 <= 1)
                        barFormat.IdenticalCopiesOfLabel = 1; // set copies
                    //else
                    //    barFormat.IdenticalCopiesOfLabel = Setting.Default.打印配置.打印张数; // set copies

                    Substrings.Clear();
                    var enu = barFormat.NamedSubStrings.GetAll(":", ",");
                    var spi = enu.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < spi.Length; i++)
                    {
                        int index = spi[i].IndexOf(':');
                        if (index > 0)
                            Substrings.Add(spi[i].Substring(0, index), null);
                    }

                    SetOffset(LocalSetting.localSetting.OffsetX, LocalSetting.localSetting.OffsetY);
                    //if ( string.IsNullOrEmpty(SerialNum))
                    //    scanLog.InsertLog(("标签模板中不存在序列号，需检查设定"));
                   
                    LocalSetting.localSetting.PrintSerialNumMemTime = DateTime.Now;
                    loadTimeMem = fl.LastWriteTime;
                    loadNameMem = path2;
                }
                else
                {
                    if (barFormat != null)
                    {
                        barFormat.Close(BarTender.BtSaveOptions.btDoNotSaveChanges);
                        barFormat = null;
                    }
                    throw new Exception($"样式文件 \"{path}\" 不存在");
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("没有注册类") > 0)
                    throw new Exception("Bartender 注册版本错误，要求为Enterprise Automation Edition");
                else
                    throw ex;
            }
        }//
        public void SetZSM()
        {
            var sn = "追溯码";
            if (!string.IsNullOrEmpty(WinForm.追溯码))
                SetSubString(sn, WinForm.追溯码);
        }
        bool PrintFrx(string[] extras)
        {
            SetSubStrings(extras);
            var sn = "追溯码";
            if(!string.IsNullOrEmpty(WinForm.追溯码))
            SetSubString(sn, WinForm.追溯码);
            var labelImage = GetFrxImg();
            while (true)
            {
                var nLen = GetUsbBufferLenSafe();
                if (nLen > 0)
                {
                    if (printAgroxImg(labelImage) == false)
                    {
                        if (System.Windows.Forms.MessageBox.Show("打印失败，是否重试？", null, System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                            return false;
                        else
                            continue;
                    }
                    return true;
                }
                else
                {
                    if (printPostekImg(labelImage) == false)
                    {
                        if (System.Windows.Forms.MessageBox.Show("打印失败，是否重试？", null, System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                            return false;
                        else
                            continue;
                    }
                    return true;
                }
            }
        }
        void LoadFrx(string path,bool flag = false)
        {

            if (string.IsNullOrEmpty(path))
                path =WinForm.产品型号;

            var temp = $"{LocalSetting.SystemDIR}\\FrxBtw\\{path}.frx";

            LoadFrx(temp);


        }

        void LoadFrx(string  sm)
        {
            var PrevSerial = SerialNum;
            var Prevbz = LocalSetting.localSetting.班组号;
            var Prevjch = LocalSetting.localSetting.检测号;

          //  sm.Position = 0;
            report = new FastReport.Report();
            report.Load(sm);
            report.Prepare();
            var reportPage = report.Pages[0] as FastReport.ReportPage;
            PrintSettings.PaperWidth = reportPage.PaperWidth;
            PrintSettings.PaperHeight = reportPage.PaperHeight;
            var pas = report.Parameters;
            Substrings.Clear();
            for (int i = 0; i < pas.Count; i++)
            {
                var v = pas[i].Value as string;
                var k = pas[i].Name;
                //    pas[i].Expression = "";
                Substrings.Add(k, v);
            }

           // if (string.IsNullOrEmpty(SerialNum) )
           //     scanLog.InsertLog(("标签模板中不存在序列号，需检查设定"));
           // if ((DateTime.Now - LocalSetting.localSetting.PrintSerialNumMemTime).TotalHours < 4
           //|| LocalSetting.localSetting.PrintSerialNumMemTime.Day == DateTime.Now.Day)
           // {
           //     if (string.IsNullOrEmpty(PrevSerial))
           //         PrevSerial = LocalSetting.localSetting.PrintSerialNumMem;
           // }
           // else if (string.IsNullOrEmpty(SerialNum) == false)
           // {
           //     StringBuilder sb = new StringBuilder(SerialNum);
           //     for (int i = 0; i < sb.Length - 1; i++)
           //         sb[i] = '0';
           //     if (LocalSetting.localSetting.PrintSerialNumMem != null && SerialNum.Length == LocalSetting.localSetting.PrintSerialNumMem.Length)
           //     {
           //         if (LocalSetting.localSetting.PrintSerialNumMem.Length == 6)
           //         {
           //             sb[0] = LocalSetting.localSetting.PrintSerialNumMem[0];
           //             sb[1] = LocalSetting.localSetting.PrintSerialNumMem[1];
           //         }
           //     }
           //     sb[sb.Length - 1] = '1';
           //     PrevSerial = sb.ToString();
           // }
           // if (string.IsNullOrEmpty(PrevSerial) == false && SerialNum != null)
           // {
           //     if (PrevSerial.Length == SerialNum.Length)
           //         SerialNum = PrevSerial;
           //     else if (PrevSerial.Length > SerialNum.Length)
           //         SerialNum = PrevSerial.Substring(0, SerialNum.Length);
           //     else
           //     {
           //         SerialNum = PrevSerial.PadLeft(SerialNum.Length, '0');
           //     }
           // }
            if (Prevbz.IsNullOrEmpty() == false && Prevbz.Length == LocalSetting.localSetting.班组号?.Length)
                LocalSetting.localSetting.班组号 = Prevbz;
            if (Prevjch.IsNullOrEmpty() == false && Prevjch.Length == LocalSetting.localSetting.检测号?.Length)
                LocalSetting.localSetting.检测号 = Prevjch;

        }
     
        //internal override string PNNum
        //{
        //    //get
        //    //{
        //    //    try
        //    //    {
        //    //           // return GetSubString("零件号");
        //    //    }
        //    //    catch
        //    //    {
        //    //        return null;
        //    //    }
        //    //}
        //    //set
        //    //{
        //    //  //  ApplyPn(value, Substrings);
        //    //}
        //}
        internal override void SetCopies(int copies)
        {
            if (copies > 0 && barFormat != null)
                barFormat.IdenticalCopiesOfLabel = copies; // set copies
        }
        internal override string SerialNum
        {
            get
            {
                try
                {
                    if (LocalSetting.localSetting.打印方法 == TestProcessFormsApp.Public.打印方式.btw)
                        return barFormat?.GetNamedSubStringValue("序列号");
                    else
                        return GetSubString("序列号");
                    //var sn = "序列号";
                    //return GetSubString(sn);
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    if (LocalSetting.localSetting.打印方法 == TestProcessFormsApp.Public.打印方式.btw)
                    {
                        barFormat.SetNamedSubStringValue("序列号", value);
                    }
                    else
                    {
                        var sn = "序列号";
                        SetSubString(sn, value);
                    }
                }
                catch
                {
                }
            }
        }
        public  string 检测号
        {
            get
            {
                try
                {
                    if (LocalSetting.localSetting.打印方法 == TestProcessFormsApp.Public.打印方式.btw)
                        return barFormat?.GetNamedSubStringValue("检测号");
                    else
                        return base.GetSubString("检测号");
                    //var sn = "序列号";
                    //return GetSubString(sn);
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    if (LocalSetting.localSetting.打印方法 == TestProcessFormsApp.Public.打印方式.btw)
                    {
                        barFormat.SetNamedSubStringValue("检测号", value);
                    }
                    else
                    {
                        var sn = "检测号";
                        SetSubString(sn, value);
                    }
                }
                catch
                {
                }
            }
        }
        public string 班组号
        {
            get
            {
                try
                {
                    if (LocalSetting.localSetting.打印方法 == TestProcessFormsApp.Public.打印方式.btw)
                        return barFormat?.GetNamedSubStringValue("班组号");
                    else
                        return base.GetSubString("班组号");
                    //var sn = "序列号";
                    //return GetSubString(sn);
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    if (LocalSetting.localSetting.打印方法 == TestProcessFormsApp.Public.打印方式.btw)
                    {
                        barFormat.SetNamedSubStringValue("班组号", value);
                    }
                    else
                    {
                        var sn = "班组号";
                       SetSubString(sn, value);
                    }
                }
                catch
                {
                }
            }
        }
        internal override string GetSubString(string subName)
        {
            if (LocalSetting.localSetting.打印方法 == TestProcessFormsApp.Public.打印方式.btw)
            {
                if (barFormat != null)
                    try
                    { return barFormat.GetNamedSubStringValue(subName); }
                    catch { }
            }
            else
            {
                if (report != null)
                {
                    try
                    { return report.Parameters.FindByName(subName)?.Value as string; }
                    catch { return null; }
                }
                else if (Substrings != null && Substrings.TryGetValue(subName, out string val))
                    return val;
            }
            return null;
        }
        internal override void SetSubString(string subName, string val)
        {
            if (barFormat != null)
                try
                {
                    barFormat.SetNamedSubStringValue(subName, val);
                }
                catch { }
            else
            {
                base.SetSubString(subName, val);
                if (report != null)
                {
                    report.Parameters.FindByName(subName).Expression = "";
                    report.Parameters.FindByName(subName).Value = val;
                }
            }
             
        }

        internal override void SetDate(DateTime t)
        {
            if (Substrings.ContainsKey("年"))
                barFormat.SetNamedSubStringValue("年", t.ToString("yyyy"));
            if (Substrings.ContainsKey("月"))
                barFormat.SetNamedSubStringValue("月", t.ToString("MM"));
            if (Substrings.ContainsKey("日"))
                barFormat.SetNamedSubStringValue("日", t.ToString("dd"));
            if (Substrings.ContainsKey("DayOfYear"))
                barFormat.SetNamedSubStringValue("DayOfYear", t.DayOfYear.ToString("000"));
            if (Substrings.ContainsKey("年2"))
                barFormat.SetNamedSubStringValue("年2", t.ToString("yy"));
            if (Substrings.ContainsKey("年1"))
            {
                var yy = t.Year % 100;
                while (yy > 30)
                    yy -= 30;
                if (codeDFXKDicRev.TryGetValue(yy, out char y))
                    barFormat.SetNamedSubStringValue("年1", y.ToString());
            }
        }
        public override void ShowDebugForm()
        {
            LoadBtw(null);
            //reStartFlag = true;
            //loadTimeMem = DateTime.MinValue;
            //barApplication.Visible = true;

        }
        public override void Dispose()
        {
            try
            {
                if (barApplication != null)
                    barApplication.Quit(BarTender.BtSaveOptions.btDoNotSaveChanges);
            }
            catch { }
        }
        bool reStartFlag = false;
        double xoffsetMem = 0, yoffsetMem;
        void SetOffset(double x, double y)
        {
            //Program.MsgBox(barFormat.Objects.Count.ToString());
            if (barFormat == null)
                return;
            try
            {
                xoffsetMem = -9999;
                yoffsetMem = -9999;
                var len = barFormat.Objects.Count;
                //Program.MsgBox($"{itm.X}, {itm.Y}, {itm.Value}");
                for (int i = 1; i <= len; i++)
                {
                    var itm = barFormat.Objects.Item(i);
                    itm.X = itm.X + x;
                    itm.Y = itm.Y + y;
                }
                xoffsetMem = x;
                yoffsetMem = y;
            }
            catch (Exception ex)
            {
                loadTimeMem = DateTime.MinValue;
                throw ex;
            }
        }
   
        internal static void ApplyPn(string value, Dictionary<string, string> s)
        {
            //if (Setting.Default.打印模板中零件号共享名称.IsNullOrEmpty())
            //    Setting.Default.打印模板中零件号共享名称 = "零件号";
            //int len2 = 0;
            //string s2 = LoadingSetting.Default.零件号;
            //if (s2.IsNullOrEmpty() == false && s.TryGetValue(s2, out string val))
            //    len2 = val.Length;
            //if (value.Length > len2)
            //{
            //    if (LoadingSetting.Default.打印方法 == 打印方式.btw)
            //    {
            //        if (barFormat != null)
            //        {
            //            try { barFormat.SetNamedSubStringValue(s2, value.Substring(value.Length - len2, len2)); }
            //            catch { }
            //            try { barFormat.SetNamedSubStringValue(LoadingSetting.Default.零件号, value.Substring(0, value.Length - len2)); } catch { }
            //        }
            //    }
            //    else
            //    {
            //        SetSubString(s2, value.Substring(value.Length - len2, len2), s);
            //        SetSubString(LoadingSetting.Default.零件号, value.Substring(0, value.Length - len2), s);
            //    }
            //}
        }
        internal override string GetBarCode()
        {
            if (barFormat == null)
            {
                if (LocalSetting.localSetting.打印方法 == TestProcessFormsApp.Public.打印方式.frx)
                {
                    if (report != null)
                    {
                        report.Prepare();
                        var reportPage = report.Pages[0] as FastReport.ReportPage;
                        foreach (var item in reportPage.AllObjects)
                        {
                            if (item is FastReport.Barcode.BarcodeObject cd)
                            {
                                var tp = cd.GetType();
                                var fi = tp.GetField("barcode", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                                var ob = fi.GetValue(cd) as FastReport.Barcode.BarcodeBase;
                                var tp2 = ob.GetType();
                                var fi2 = tp2.GetField("text", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                                var txt = fi2.GetValue(ob) as string;
                                return txt;
                            }
                        }
                    }
                    else
                        return GetBarCodeSubStrings(PrintSettings, Substrings);
                }
                else
                    return null;
            }
               
            var len = barFormat.Objects.Count;
            for (int i = 1; i <= len; i++)
            {
                var itm = barFormat.Objects.Item(i);
                if (itm.Type == BarTender.BtObjectType.btObjectBarcode)
                    return itm.Value;
            }
            return null;
        }
    }
}
