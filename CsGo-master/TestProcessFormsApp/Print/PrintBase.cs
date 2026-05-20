using CCWin.SkinControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using TestProcessFormsApp.Public;
using ZXing;
using ZXing.Datamatrix.Encoder;

namespace JLRScan
{
    public abstract class PrintBase
    {
        internal static string DefaultFont = "Arial";
        protected Dictionary<string, string> Substrings = new Dictionary<string, string>();
        internal static Dictionary<char, ushort> codeDFXKDic = new Dictionary<char, ushort>();
        internal static Dictionary<int, char> codeDFXKDicRev = new Dictionary<int, char>();
        internal static Dictionary<string, PrintSetting> PrintModels = new Dictionary<string, PrintSetting>();
        internal static Dictionary<string, Image> ImgModels = new Dictionary<string, Image>();
        internal DateTime loadTimeMem = DateTime.MinValue;
        internal string loadNameMem;
        internal bool printTestFlag = false;
        internal static bool applyAutoString = true;
        // 2026-05-20: 最近一次打印是否成功，供上层做离线补打判定。
        public bool LastPrintSuccess { get; protected set; } = false;
        static PrintBase()
        {
            try
            {
                for (ushort i = 1; i < 10; i++)
                    codeDFXKDic.Add((char)('0' + i), i);
                for (ushort i = 10; i <= 17; i++)
                    codeDFXKDic.Add((char)('A' + i - 10), i);
                for (ushort i = 18; i <= 22; i++)
                    codeDFXKDic.Add((char)('J' + i - 18), i);
                codeDFXKDic.Add('P', 23);
                codeDFXKDic.Add('R', 24);
                codeDFXKDic.Add('S', 25);
                codeDFXKDic.Add('T', 26);
                for (ushort i = 27; i <= 31; i++)
                    codeDFXKDic.Add((char)('V' + i - 27), i);
                foreach (var itm in codeDFXKDic)
                    codeDFXKDicRev.Add(itm.Value, itm.Key);

                //barApplication = new BarTender.Application();
                //barFormat = barApplication.Formats.Open(Environment.CurrentDirectory + "\\PrintB.btw");
                //barFormat.IdenticalCopiesOfLabel = 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal Bitmap GetFrxImg()
        {
            var pg = report.Pages[0] as FastReport.ReportPage;
            if (printTestFlag)
                pg.Watermark.Enabled = true;
            else
                pg.Watermark.Enabled = false;

            report.Prepare();

            var imageExport = new FastReport.Export.Image.ImageExport
            {
                ImageFormat = FastReport.Export.Image.ImageExportFormat.Bmp,
                Resolution = 600
            };
            var memoryStream = new System.IO.MemoryStream();
            imageExport.Export(report, memoryStream);
            return Image.FromStream(memoryStream) as Bitmap;
        }
        internal static string GetDFXKDate(DateTime tm)
        {
            try
            {
                if (tm < new DateTime(2000, 1, 1))
                    throw new Exception("日期超限");
                StringBuilder sb = new StringBuilder();
                sb.Append(codeDFXKDicRev[tm.Year % 100 % 32]).Append(codeDFXKDicRev[tm.Month]).Append(codeDFXKDicRev[tm.Day]);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static PrintBase Create(int flag)
        {
            switch (flag)
            {
                default:
                    return new BarTenderPrint();
            }
        }
        internal abstract void Print(string[] extras);
        internal virtual void PrintTest()
        {
            LoadBtw(null);
            printTestFlag = true;
            foreach (var v in Substrings)
            {
                if (!v.Key.IsNullOrEmpty())// && v.Key != "序列号")
                {
                    loadTimeMem = DateTime.MinValue;
                    StringBuilder sb = new StringBuilder();
                    var s2 = GetSubString(v.Key);
                    if (!s2.IsNullOrEmpty())
                    {
                        testID1 = 0;
                        testID2 = 0;
                        for (int i = 0; i < s2.Length; i++)
                            sb.Append(getNextTestValue(s2[i]));
                        SetSubString(v.Key, sb.ToString());
                    }
                }
            }
            Print(null);
            printTestFlag = false;
        }
       ScanLog scanLog = new ScanLog();

        // 2026-05-19: 统一包装USB缓冲区读取，避免Winpplb.dll位数/依赖异常直接中断流程。
        protected int GetUsbBufferLenSafe()
        {
            try
            {
                return B_GetUSBBufferLen();
            }
            catch (BadImageFormatException ex)
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string exeArch = Environment.Is64BitProcess ? "x64" : "x86";
                string p1 = Path.Combine(baseDir, "Winpplb.dll");
                string p2 = Path.Combine(baseDir, "WINPSK.dll");
                string p3 = Path.Combine(baseDir, "WinPort.dll");
                string msg =
                    $"打印DLL加载失败(BadImageFormat): 进程={exeArch}, BaseDir={baseDir}, " +
                    $"Winpplb={GetPeArchText(p1)}, WINPSK={GetPeArchText(p2)}, WinPort={GetPeArchText(p3)}, Err={ex.Message}";
                scanLog.InsertLog(msg);
                return -1;
            }
            catch (DllNotFoundException ex)
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string msg = $"打印DLL缺失: BaseDir={baseDir}, Err={ex.Message}";
                scanLog.InsertLog(msg);
                return -1;
            }
        }

        private static string GetPeArchText(string path)
        {
            try
            {
                if (!File.Exists(path))
                    return $"{path}(missing)";

                byte[] b = File.ReadAllBytes(path);
                int pe = BitConverter.ToInt32(b, 0x3C);
                ushort machine = BitConverter.ToUInt16(b, pe + 4);
                string arch = machine == 0x014c ? "x86" : machine == 0x8664 ? "x64" : "0x" + machine.ToString("X");
                return $"{Path.GetFileName(path)}[{arch}]";
            }
            catch (Exception ex)
            {
                return $"{Path.GetFileName(path)}[read-fail:{ex.Message}]";
            }
        }

        internal bool printAgroxImg(Bitmap bmp)
        {
            var PrinterDPI = 300;
            byte[] pbuf = new byte[128];
            System.Text.Encoding encAscII = System.Text.Encoding.ASCII;
            System.Text.Encoding encUnicode = System.Text.Encoding.Unicode;
            int nLen, ret;
            byte[] buf1, buf2;
            int len1 = 128, len2 = 128;
            buf1 = new byte[len1];
            buf2 = new byte[len2];
            B_EnumUSB(pbuf);
            B_GetUSBDeviceInfo(1, buf1, out len1, buf2, out len2);
            nLen = GetUsbBufferLenSafe();
            if (nLen <= 0)
            {
                scanLog.InsertLog("ERR9348：未找到打印机A");
                return false;
            }
            ret = B_CreateUSBPort(1);
            if (ret != 0)
            {
                B_ClosePrn();
                scanLog.InsertLog("ERR9956：打印机A连接失败" + ret);
                return false;
            }
            string argxh = encAscII.GetString(buf1, 0, len1);
            B_Set_DebugDialog(1);//1:Enable
            B_Set_ProcessDlg(1);//1:Enable
            B_Select_Option(6);//6 -> thermal transfer, Peel, disable Cutter.
                               //B_Set_Darkness(PrintSettings.PrintDarkness);
                               //    B_Set_Direction('B');//B T
            if (PrintSettings.PrintDarkness <= 0)
                PrintSettings.PrintDarkness = 8;
            else if (PrintSettings.PrintDarkness > 15)
                PrintSettings.PrintDarkness = 15;
            B_Set_Darkness(PrintSettings.PrintDarkness);
            double rate = PrinterDPI / 25.4;
            int offsety = (int)(PrintSettings.OffsetY * rate + 20);
            if (offsety < 0)
                offsety = 0;
            //B_Set_Originpoint((int)(614 - (PrintSettings.PaperWidth / 2 - PrintSettings.OffsetX) * rate), offsety);
            //B_Set_Labgap((int)(PrintSettings.PaperHeight * rate + 20), 3);

            int hPixel = (int)(PrintSettings.PaperHeight * rate + 10);
            int wPixel = (int)(PrintSettings.PaperWidth * rate);

            IntPtr drawhdl;

            drawhdl = bmp.GetHbitmap();
            //     B_Get_Graphic_ColorBMP_HBitmap(0, 0, wPixel, hPixel, 0, "bp1", drawhdl);
            if (argxh.IndexOf("3140L") > 0)
            {
                B_Set_Direction('B');//B T
                B_Set_Originpoint((int)(614 - (PrintSettings.PaperWidth / 2 - PrintSettings.OffsetX) * rate), offsety);
                B_Set_Labgap((int)(PrintSettings.PaperHeight * rate + 20), 3);
                B_Get_Graphic_ColorBMP_HBitmap(0, 0, wPixel, hPixel, 0, "bp1", drawhdl);
            }else if(argxh.IndexOf("3140EX")>0)
            {
                    B_Set_Direction('T');//B T
                    string pbuf1 = "\x1B" + "KI9" + (offsety >= 0 ? ("+" + offsety.ToString("f1")) : offsety.ToString("f1")) + "\x0D\x0A";
                    B_WriteData(0, pbuf1, 10);
                    B_Set_Labwidth(((int)PrintSettings.PaperWidth + (int)PrintSettings.OffsetX) * 12);
                    B_Set_LabelForSmartPrint((int)(PrintSettings.PaperHeight * 10), 31);
                    B_WriteData(0, "\x1B" + "KI80" + "\x0D\x0A", 7);//KI80反射试                                            // B_Set_Labgap(15 * 12, 2 * 12);
                    B_Get_Graphic_ColorBMP_HBitmap(0, 0, wPixel, hPixel, 2, "bp1", drawhdl);
            }
            else
            {
                B_Set_Direction('T');//B T
                B_Set_Originpoint((int)(614 - (PrintSettings.PaperWidth / 2 - PrintSettings.OffsetX) * rate), offsety);
                B_Set_LabelForSmartPrint((int)(PrintSettings.PaperHeight * 10), 10);
                B_Get_Graphic_ColorBMP_HBitmap(0, 0, wPixel, hPixel, 2, "bp1", drawhdl);
            }
            DeleteObject(drawhdl);

            ret = B_Print_Out(1);

            if (ret != 0)
            {
                B_ClosePrn();
                scanLog.InsertLog("ERR30854：打印失败" + ret);
                return false;
            }

            B_ClosePrn();
            IncSerial();
            return true;
        }
        void IncSerial()
        {
            //string ser = GetSubString("序列号");
            //if (ser.IsNullOrEmpty() == false)
            //{
            //    switch (PrintSettings.序列号递增模式)
            //    {
            //        case "HEX":
            //            if (int.TryParse(ser, System.Globalization.NumberStyles.HexNumber, null, out int seri))
            //            {
            //                var ns = (seri + 1).ToString("X").PadLeft(ser.Length, '0');
            //                if (ns.Length > ser.Length)
            //                    SerialNum = "1".PadLeft(ser.Length, '0');
            //                else
            //                    SerialNum = ns;
            //                return;
            //            }
            //            break;
            //        case "34":
            //            break;
            //        case "DFXK":
            //            break;
            //        default:
            //            if (int.TryParse(ser, out seri))
            //            {
            //                var ns = (seri + 1).ToString().PadLeft(ser.Length, '0');
            //                if (ns.Length > ser.Length)
            //                    SerialNum = "1".PadLeft(ser.Length, '0');
            //                else
            //                    SerialNum = ns;
            //                return;
            //            }
            //            break;
            //    }
            //    Program.MsgBox("序列号解析错误：" + ser);
            //}
        }
        internal bool printPostekImg(Bitmap bmp)
        {
            int ret;
            var PrinterDPI = 600;
            //ShowImg.Show(bmp);

            ret = PrintLab.OpenUSBPort(255);//打开打印机端口
            if (ret != 0)
            {
                scanLog.InsertLog("ERR9348：打印机P连接失败： " + ret);
                return false;
            }
            PrintLab.PTK_ClearBuffer();           //清空缓冲区
            PrintLab.PTK_SetPrintSpeed(4);        //设置打印速度
            PrintLab.PTK_SetDirection('B');

            double rate = PrinterDPI / 25.4;
            int offsety = (int)(PrintSettings.OffsetY * rate + 20);
            if (offsety < 0)
                offsety = 0;

            uint hPixel = (uint)(PrintSettings.PaperHeight * rate + 10);
            uint wPixel = (uint)(PrintSettings.PaperWidth * rate);
            PrintLab.PTK_SetLabelHeight(hPixel, 10, 0, false); //设置标签的高度和定位间隙\黑线\穿孔的高度
            PrintLab.PTK_SetLabelWidth(wPixel);      //设置标签的宽度
            PrintLab.PTK_SetCoordinateOrigin((uint)((105.6 / 2 - PrintSettings.PaperWidth / 2 + PrintSettings.OffsetX) * rate), (uint)offsety);

            Convert1bpp(null, bmp);
            //bmp.Save("print.bmp");
            var data = GetRawData(bmp);

            ret = PrintLab.PTK_DrawBinGraphics(0, 0, (uint)(data.Length / bmp.Height), (uint)bmp.Height, data);

            if (ret != 0)
            {
                PrintLab.CloseUSBPort();//关闭打印机端口
                scanLog.InsertLog("Err PTK_DrawBinGraphics: " + ret);
                return false;
            }
            //ret = PrintLab.PTK_DrawPcxGraphics(0, 0, "print");
            //if (ret != 0)
            //    Program.ErrHdl("Err PTK_DrawPcxGraphics: " + ret);


            ret = PrintLab.PTK_PrintLabel(1, 1);

            if (ret != 0)
            {
                PrintLab.CloseUSBPort();//关闭打印机端口
                scanLog.InsertLog("Err PTK_PrintLabel: " + ret);
                return false;
            }
            PrintLab.CloseUSBPort();//关闭打印机端口
            IncSerial();
            return true;
        }
        internal static byte[] GetRawData(Bitmap bmp)
        {
            if (bmp == null || bmp.Width == 0)
                return null;
            var data = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
            byte[] re = new byte[Math.Abs(data.Stride) * data.Height];
            Marshal.Copy(data.Scan0, re, 0, re.Length);
            bmp.UnlockBits(data);
            for (int i = 0; i < bmp.Height; i++)
            {
                int val = bmp.Size.Width / 8;
                int rem = bmp.Size.Width % 8;
                if (rem > 0)
                {
                    for (int j = rem; j < 8; j++)
                    {
                        re[i * data.Stride + val] |= (byte)(0x80 >> j);
                    }
                    val++;
                }
                for (int j = val; j < data.Stride; j++)
                    re[i * data.Stride + j] = 0xFF;
            }
            return re;
        }
        internal void PrintSDK(string[] extras)
        {
            if (Substrings.Count == 0)
                LoadBtw(null);
            SetSubStrings(extras);
            while (true)
            {
                var nLen = GetUsbBufferLenSafe();
                if (nLen > 0)
                {
                    if (printAgrox() == false)
                    {
                        if (System.Windows.Forms.MessageBox.Show("打印失败，是否重试？", null, System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                            return;
                        else
                            continue;
                    }
                    return;
                }
                else
                {
                    if (printPostek() == false)
                    {
                        if (System.Windows.Forms.MessageBox.Show("打印失败，是否重试？", null, System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                            return;
                        else
                            continue;
                    }
                    return;
                }
            }
            //Program.ErrHdl("无法打印：未找到打印机");
        }
        bool printAgrox()
        {
            var PrinterDPI = 300;
            //if (Setting.Default.test)
            //    showsubstrings();
            //var re2 = System.Text.RegularExpressions.Regex.Unescape(re);
            int nLen, ret;
            nLen = GetUsbBufferLenSafe();
            if (nLen <= 0)
            {
                scanLog.InsertLog("ERR9348：未找到打印机A");
                return false;
            }
            ret = B_CreateUSBPort(1);
            if (ret != 0)
            {
                B_ClosePrn();
                scanLog.InsertLog("ERR9956：打印机A连接失败" + ret);
                return false;
            }
          ;
            if (PrintSettings.PrintDarkness <= 0)
                PrintSettings.PrintDarkness = 8;
            else if (PrintSettings.PrintDarkness > 15)
                PrintSettings.PrintDarkness = 15;

            B_Set_DebugDialog(1);//1:Enable
            B_Set_ProcessDlg(1);//1:Enable
            B_Select_Option(6);//6 -> thermal transfer, Peel, disable Cutter.
            B_Set_Darkness(PrintSettings.PrintDarkness);
            B_Set_Direction('B');//B T

            double rate = PrinterDPI / 25.4;
            int offsety = (int)(PrintSettings.OffsetY * rate + 20);
            if (offsety < 0)
                offsety = 0;
            B_Set_Originpoint((int)(614 - (PrintSettings.PaperWidth / 2 - PrintSettings.OffsetX) * rate), offsety);
            B_Set_Labgap((int)(PrintSettings.PaperHeight * rate + 20), 3);

            int hPixel = (int)(PrintSettings.PaperHeight * rate + 10);
            int wPixel = (int)(PrintSettings.PaperWidth * rate);

            bool zxingflag = false;
            if (PrintSettings.二维码生成模式 == "ZXING")
                zxingflag = true;
            if (!zxingflag)
                for (int i = 0; i < PrintSettings.DataMatrix?.Length; i++)
                {
                    var code = GetBarCodeSubStrings(PrintSettings, Substrings, i);
                    if (string.IsNullOrEmpty(code)  == false)
                    {
                        int h = (int)Math.Round(PrintSettings.DataMatrix[i].MatrixHeight * rate);
                        if (h <= 0 || h > 40)
                            h = 5;
                        if (PrintSettings.DataMatrix[i].type == "QR")
                            B_Bar2d_QR((int)(PrintSettings.DataMatrix[i].x * rate), (int)(PrintSettings.DataMatrix[i].y * rate)
                                , 2, h, 'L', 'A', 0, 0, 0, code);
                        else
                            B_Bar2d_DataMatrix((int)(PrintSettings.DataMatrix[i].x * rate), (int)(PrintSettings.DataMatrix[i].y * rate), 0, 0, h, 0, code);
                    }
                }

            var bmp = GetPrintImg(PrintSettings, PrinterDPI, Substrings, printTestFlag, zxingflag);

            IntPtr drawhdl;

            drawhdl = bmp.GetHbitmap();
            B_Get_Graphic_ColorBMP_HBitmap(0, 0, wPixel, hPixel, 0, "bp1", drawhdl);
            DeleteObject(drawhdl);

            //ori: '0' is 0 '1' is 90 '2' is 180 '3' is 270

            //B_Draw_Box(0, 10, 4, 260, 130);
            //if (PrintSettings.PrintCopies > 1 && PrintSettings.PrintCopies < 5)
            //    ret = B_Print_Out(PrintSettings.PrintCopies);// copy 2.
            //else
            ret = B_Print_Out(1);

            if (ret != 0)
            {
                B_ClosePrn();
                scanLog.InsertLog("ERR30854：打印失败" + ret);
                return false;
            }

            B_ClosePrn();
            IncSerial();
            return true;
        }
        bool printPostek()
        {
            int ret;
            var PrinterDPI = 600;

            ret = PrintLab.OpenUSBPort(255);//打开打印机端口
            if (ret != 0)
            {
                scanLog.InsertLog("ERR9348：打印机P连接失败： " + ret);
                return false;
            }
            PrintLab.PTK_ClearBuffer();           //清空缓冲区
            PrintLab.PTK_SetPrintSpeed(4);        //设置打印速度
            if (PrintSettings.PrintDarkness <= 0)
                PrintSettings.PrintDarkness = 10;
            else if (PrintSettings.PrintDarkness > 20)
                PrintSettings.PrintDarkness = 20;
            PrintLab.PTK_SetDarkness((uint)PrintSettings.PrintDarkness);         //设置打印黑度
            PrintLab.PTK_SetDirection('B');
            //var re = PrintLab.PTK_SetPrinterState('N');
            //PrintLab.PTK_EnableBackFeed(140);

            double rate = PrinterDPI / 25.4;
            int offsety = (int)(PrintSettings.OffsetY * rate + 20);
            if (offsety < 0)
                offsety = 0;
            //B_Set_Originpoint((int)(614 - (PaperWidth / 2 + offsetX) * rate), offsety);
            //B_Set_Labgap((int)(PaperHeight * rate + 20), 3);

            uint hPixel = (uint)(PrintSettings.PaperHeight * rate + 10);
            uint wPixel = (uint)(PrintSettings.PaperWidth * rate);
            PrintLab.PTK_SetLabelHeight(hPixel, 10, 0, false); //设置标签的高度和定位间隙\黑线\穿孔的高度
            PrintLab.PTK_SetLabelWidth(wPixel);      //设置标签的宽度
            PrintLab.PTK_SetCoordinateOrigin((uint)((105.6 / 2 - PrintSettings.PaperWidth / 2 + PrintSettings.OffsetX) * rate), (uint)offsety);

            //PrintLab.PTK_DrawRectangle(0, 0, 5, 500, 200);
            bool zxingflag = false;
            if (PrintSettings.二维码生成模式 == "ZXING")
                zxingflag = true;
            if (!zxingflag)
                for (int i = 0; i < PrintSettings.DataMatrix?.Length; i++)
                {
                    var code = GetBarCodeSubStrings(PrintSettings, Substrings, i);
                    if (code.IsNullOrEmpty() == false)
                    {
                        uint h = (uint)Math.Round(PrintSettings.DataMatrix[i].MatrixHeight * rate);
                        if (h <= 0 || h > 40)
                            h = 5;
                        if (PrintSettings.DataMatrix[i].type == "QR")
                            PrintLab.PTK_DrawBar2D_QR((uint)(PrintSettings.DataMatrix[i].x * rate), (uint)(PrintSettings.DataMatrix[i].y * rate), 0, 0, 0, h, 1, 0, 8, code);
                        else
                            PrintLab.PTK_DrawBar2D_DATAMATRIX((uint)(PrintSettings.DataMatrix[i].x * rate), (uint)(PrintSettings.DataMatrix[i].y * rate), 0, 0, 0, h, code);
                    }
                }

            var bmp = GetPrintImg(PrintSettings, PrinterDPI, Substrings, printTestFlag, zxingflag);

            Convert1bpp(null, bmp);
            //bmp.Save("print.bmp");
            var data = GetRawData(bmp);

            ret = PrintLab.PTK_DrawBinGraphics(0, 0, (uint)(data.Length / bmp.Height), (uint)bmp.Height, data);
            if (ret != 0)
            {
                PrintLab.CloseUSBPort();//关闭打印机端口
                scanLog.InsertLog("Err PTK_DrawBinGraphics: " + ret);
                return false;
            }

            ret = PrintLab.PTK_PrintLabel(1, 1);

            if (ret != 0)
            {
                PrintLab.CloseUSBPort();//关闭打印机端口
                scanLog.InsertLog("Err PTK_PrintLabel: " + ret);
                return false;
            }
            PrintLab.CloseUSBPort();//关闭打印机端口
            IncSerial();
            return true;
        }
        internal int testID1, testID2;
        string t1 = "test";
        string t2 = "打印测试";
        internal char getNextTestValue(char v)
        {
            var re = '?';
            if (v < '\xFF')
            {
                if (testID1 >= t1.Length)
                    testID1 = 0;
                re = t1[testID1];
                testID1++;
            }
            else
            {
                if (testID2 >= t2.Length)
                    testID2 = 0;
                re = t2[testID2];
                testID2++;
            }
            return re;
        }
        internal static Bitmap GetPrintImg(PrintSetting ps, int PrinterDPI, Dictionary<string, string> ss, bool printTestFlag, bool dispMatrix = false)
        {
            double rate = PrinterDPI / 25.4;
            int hPixel = (int)(ps.PaperHeight * rate + 10);
            int wPixel = (int)(ps.PaperWidth * rate);

            Bitmap bmp = new Bitmap(wPixel, hPixel, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            bmp.SetResolution(PrinterDPI, PrinterDPI);
            Graphics gr = Graphics.FromImage(bmp);
            gr.Clear(Color.White);
            gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (dispMatrix) for (int i = 0; i < ps.DataMatrix?.Length; i++)
                {
                    var ds = GetBarCodeSubStrings(ps, ss, i);
                    var writer = new BarcodeWriter
                    {
                        Format = ps.DataMatrix[i].type == "QR" ? BarcodeFormat.QR_CODE : BarcodeFormat.DATA_MATRIX
                    };

                    writer.Options.Hints.Add(EncodeHintType.DATA_MATRIX_SHAPE, SymbolShapeHint.FORCE_SQUARE);
                    writer.Options.Height = 1;
                    writer.Options.Width = 1;
                    writer.Options.Margin = 0;
                    var cc = writer.Encode(ds);
                    var h = (int)Math.Round(ps.DataMatrix[i].MatrixHeight * rate);
                    writer.Options.Height = cc.Height * h;
                    writer.Options.Width = cc.Width * h;
                    var bmpqr = writer.Write(ds);
                    gr.DrawImage(bmpqr, (int)(ps.DataMatrix[i].x * rate), (int)(ps.DataMatrix[i].y * rate), bmpqr.Width, bmpqr.Height);
                    //gr.DrawRectangle(new Pen(Color.Black, 3), (int)(ps.DataMatrix[i].x * rate), (int)(ps.DataMatrix[i].y * rate), bmpqr.Width, bmpqr.Height);
                }

            for (int i = 0; i < ps.imgs?.Length; i++)
            {
                if (ps.imgs[i].pic != null)
                {
                    var w = (float)(ps.imgs[i].w * rate);
                    var h = (float)(ps.imgs[i].h * rate);
                    if (ImgModels.TryGetValue(ps.imgs[i].pic, out Image bp))
                        gr.DrawImage(bp, (float)(ps.imgs[i].x * rate), (float)(ps.imgs[i].y * rate), w, h);
                    else
                    {
                      //  ImgModels.Add
                        var fd = LocalSetting.SystemDIR;
                        if (System.IO.File.Exists(fd + ps.imgs[i].pic))
                            bp = Image.FromFile(fd + ps.imgs[i].pic);
                        else if (System.IO.File.Exists(fd + ps.imgs[i].pic + ".bmp"))
                            bp = Image.FromFile(fd + ps.imgs[i].pic + ".bmp");
                        else if (System.IO.File.Exists(fd + ps.imgs[i].pic + ".jpg"))
                            bp = Image.FromFile(fd + ps.imgs[i].pic + ".jpg");
                        else if (System.IO.File.Exists(fd + ps.imgs[i].pic + ".png"))
                            bp = Image.FromFile(fd + ps.imgs[i].pic + ".png");
                        else
                            throw new Exception("无法找到图片：" + ps.imgs[i].pic);
                        ImgModels.Add(ps.imgs[i].pic, bp);
                        gr.DrawImage(bp, (float)(ps.imgs[i].x * rate), (float)(ps.imgs[i].y * rate), w, h);
                    }
                }
                else
                {
                    var w = (float)(ps.imgs[i].w * rate);
                    var h = (float)(ps.imgs[i].h * rate);
                    var x = (int)(ps.imgs[i].x * rate);
                    var y = (int)(ps.imgs[i].y * rate);
                    if (h < 4)
                        h = 4;
                    if (w < 4)
                        w = 4;
                    gr.FillRectangle(Brushes.Black, x, y, w, h);
                }
            }

            //var img = Bitmap.FromFile("tt.bmp");
            //gr.DrawImage(img, 0, 0, img.Width / 2, img.Height/2);

            string defaultfont = DefaultFont;
            if (string.IsNullOrEmpty(ps.DefaultFont) == false)
                defaultfont = ps.DefaultFont;
            for (int i = 0; i < ps.Strings?.Length; i++)
            {
                StringBuilder sb = new StringBuilder();
                double szMem = 0;
                double szMax = 0;
                double xMem = ps.Strings[i].x * rate;
                var scalx = ps.Strings[i].scaleX;
                if (scalx > 1)
                {
                    var sz = MeasureWidth(PrinterDPI, ps.Strings[i], gr, ss, ps.DisableUnescapeFlag) / rate;
                    if (sz <= scalx)
                        scalx = 0;
                    else
                        scalx /= sz;
                }
                for (int j = 0; j < ps.Strings[i].strings?.Length; j++)
                {
                    if (szMem != ps.Strings[i].strings[j].Size)
                    {
                        if (sb.Length > 0)
                        {
                            if (szMax == 0)
                            {
                                var max = ps.Strings[i].strings.Max();
                                szMax = max.Size;
                            }
                            if (szMem == 0)
                                szMem = ps.Strings[i].size;
                            var sz = DrawString(sb.ToString(), xMem, ps.Strings[i].y * rate, PrinterDPI, szMem, ps.Strings[i].FontWeight, ps.Strings[i].Font, scalx, gr, szMax, ps.Strings[i].rotate);
                            xMem += sz.Width;
                            sb.Clear();
                        }
                        szMem = ps.Strings[i].strings[j].Size;
                    }
                    string substr;
                    if (ps.Strings[i].strings[j].SubName.IsNullOrEmpty() == false
                        && ss.TryGetValue(ps.Strings[i].strings[j].SubName, out substr))
                    {
                        sb.Append(substr);
                    }
                    else if (ps.DisableUnescapeFlag)
                        sb.Append(ps.Strings[i].strings[j].Content);
                    else
                        sb.Append(System.Text.RegularExpressions.Regex.Unescape(ps.Strings[i].strings[j].Content));
                }

                if (sb.Length > 0)
                {
                    if (szMem == 0)
                        szMem = ps.Strings[i].size;
                    DrawString(sb.ToString(), xMem, ps.Strings[i].y * rate, PrinterDPI, szMem, ps.Strings[i].FontWeight, ps.Strings[i].Font, scalx, gr, szMax, ps.Strings[i].rotate);
                }
            }
            if (printTestFlag)
            {
                gr.ResetTransform();
                var p = new Pen(Color.Black, 3);
                gr.DrawRectangle(p, 1, 1, wPixel - 2, hPixel - 2);
                if (true)
                {
                    gr.DrawLine(p, 1, 1, wPixel - 1, hPixel - 1);
                    gr.DrawLine(p, wPixel - 1, 1, 1, hPixel - 1);
                    var ft = new Font(defaultfont, 12 * PrinterDPI / 72, FontStyle.Bold, GraphicsUnit.Pixel);
                    var str = "打印测试";
                    var sz = gr.MeasureString(str, ft);
                    gr.DrawString(str, ft, Brushes.Black, new PointF(wPixel / 2 - sz.Width / 2, hPixel / 2 - sz.Height / 2));
                }
            }
            //ShowImg.Show(bmp);
            return bmp;
        }
        static double MeasureWidth(int PrinterDPI, PrintStringInfo pi, Graphics gr, Dictionary<string, string> ss, bool DisableUnescapeFlag)
        {
            double fontsize = pi.size;
            int fontWeight = pi.FontWeight;
            string font = pi.Font;
            int sz = 6 * PrinterDPI / 72;
            if (fontsize > 0)
                sz = (int)(fontsize * PrinterDPI / 72);
            Font ft;
            FontStyle fontstyle;
            if (fontWeight > 4 || fontWeight == 0)
                fontstyle = FontStyle.Bold;
            else
                fontstyle = FontStyle.Regular;
            if (font.IsNullOrEmpty() == false)
                font = DefaultFont;

            double wid = 0;
            gr.ResetTransform();
            for (int i = 0; i < pi.strings?.Length; i++)
            {
                if (pi.strings[i].Size > 0)
                    ft = new Font(font, sz, fontstyle, GraphicsUnit.Pixel);
                else
                    ft = new Font(font, sz, fontstyle, GraphicsUnit.Pixel);
                string substr;
                string s;
                if (pi.strings[i].SubName.IsNullOrEmpty() == false
                    && ss.TryGetValue(pi.strings[i].SubName, out substr))
                {
                    s = substr;
                }
                else if (DisableUnescapeFlag)
                    s = pi.strings[i].Content;
                else
                    s = System.Text.RegularExpressions.Regex.Unescape(pi.strings[i].Content);
                var r = gr.MeasureString(s, ft);
                var r2 = gr.MeasureString(" ", ft);
                wid += r.Width - r2.Width;
            }

            return wid;
        }
        static SizeF DrawString(string s, double x, double y, int PrinterDPI, double fontsize, int fontWeight, string font, double scX, Graphics gr, double szMax, int rotate)
        {
            int sz = 6 * PrinterDPI / 72;
            if (fontsize > 0)
                sz = (int)(fontsize * PrinterDPI / 72);

            Font ft;
            FontStyle fontstyle;
            if (fontWeight > 4 || fontWeight == 0)
                fontstyle = FontStyle.Bold;
            else
                fontstyle = FontStyle.Regular;
            if (font.IsNullOrEmpty() == false)
                font = DefaultFont;
            ft = new Font(font, sz, fontstyle, GraphicsUnit.Pixel);

            gr.ResetTransform();
            float scaleX = 1;
            if (scX > 0 && scX != 1)
            {
                scaleX = (float)scX;
                gr.ScaleTransform(scaleX, 1);
            }
            float yfix = 0;
            int szmx = 6 * PrinterDPI / 72;
            if (szmx > 0)
                szmx = (int)(szMax * PrinterDPI / 72);
            var r = gr.MeasureString(s, ft);
            var r2 = gr.MeasureString(" ", ft);
            if (szmx > sz)
            {
                var ft2 = new Font(font, szmx, fontstyle, GraphicsUnit.Pixel);
                var r3 = gr.MeasureString(" ", ft2);
                r2 = gr.MeasureString(" ", ft2);
                yfix = (r3.Height - r.Height) * 0.7f;
            }

            if (rotate > 0)
            {
                gr.TranslateTransform((float)(-x), -(float)(y + yfix), System.Drawing.Drawing2D.MatrixOrder.Append);
                gr.RotateTransform(90, System.Drawing.Drawing2D.MatrixOrder.Append);
                gr.TranslateTransform((float)(x), (float)(y + yfix), System.Drawing.Drawing2D.MatrixOrder.Append);
                gr.DrawString(s, ft, Brushes.Black, new PointF((float)(x / scaleX), (float)(y + yfix)));
            }
            else
                gr.DrawString(s, ft, Brushes.Black, new PointF((float)(x / scaleX), (float)(y + yfix)));
            //gr.ResetTransform();
            //gr.DrawRectangle(Pens.Black, (float)(x), (float)(y + yfix), (float)(r.Width * scX), r.Height);

            //var l = new System.Windows.Forms.Label();

            return new SizeF((float)((r.Width - r2.Width) * scaleX), r.Height);
            //return gr.MeasureString(s, ft);
        }
        internal abstract void LoadBtw(string path);
        internal abstract void SetCopies(int copies);
        internal abstract string SerialNum { get; set; }
        internal virtual bool SubStringsContains(string key)
        {
            return Substrings.ContainsKey(key);
        }
       // internal abstract string PNNum { get; set; }
        internal virtual bool SetSubStrings(string[] extras) { return true; }
        internal virtual string[] GetSubStrings() { return null; }
        internal virtual string GetSubString(string subName)
        {
            if (Substrings != null && Substrings.TryGetValue(subName, out string val))
                return val;
            return null;
        }
        internal static void SetSubString(string subName, string val, Dictionary<string, string> s)
        {
            if (s != null && subName != null)
            {
                if (s.ContainsKey(subName))
                    s[subName] = val;
                else
                    s.Add(subName, val);
            }
        }
        internal virtual void SetSubString(string subName, string val)
        {
            SetSubString(subName, val, Substrings);
        }
        public virtual void ShowDebugForm()
        {

        }

        internal virtual void showsubstrings()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in Substrings)
            {
                sb.Append(item.Key).Append(": ").AppendLine(item.Value);
            }
            scanLog.InsertLog(sb.ToString());
        }
        internal virtual void SetDate(DateTime t)
        {
            List<string> lst = new List<string>();
            if (Substrings.ContainsKey("年"))
            {
                lst.Add("年");
                lst.Add(t.ToString("yyyy"));
            }
            if (Substrings.ContainsKey("月"))
            {
                lst.Add("月");
                lst.Add(t.ToString("MM"));
            }
            if (Substrings.ContainsKey("日"))
            {
                lst.Add("日");
                lst.Add(t.ToString("dd"));
            }
            if (Substrings.ContainsKey("DayOfYear"))
            {
                lst.Add("DayOfYear");
                t.DayOfYear.ToString("000");
            }
            if (Substrings.ContainsKey("年2"))
            {
                lst.Add("年2");
                lst.Add(t.ToString("yy"));
            }
        }
        internal static string GetYear(DateTime tm)
        {
            try
            {
                if (tm < new DateTime(2023, 1, 1))
                    throw new Exception("日期超限");
                StringBuilder sb = new StringBuilder();
                char y = (char)('P' + (tm.Year - 2023));
                if (tm.Year >= 2025)
                    y++;
                sb.Append(y).Append(tm.Month.ToString("00")).Append(tm.Day.ToString("00"));
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal static string GetDateByd(DateTime tm)
        {
            try
            {
                if (tm < new DateTime(2023, 1, 1))
                    throw new Exception("日期超限");
                StringBuilder sb = new StringBuilder();
                char y = (char)('P' + (tm.Year - 2023));
                if (tm.Year >= 2025)
                    y++;
                sb.Append(y).Append(codeDFXKDicRev[tm.Month]).Append(tm.Day.ToString("00"));
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal void SetDateString(string sd, string sv)
        {
            try
            {
                if (sd == "日期")
                {
                    DateTime t;
                    if (DateTime.TryParseExact(sv, "yyyyMMdd", new System.Globalization.CultureInfo("zh-CN"), System.Globalization.DateTimeStyles.None, out t))
                        SetDate(t);
                    else
                        throw new Exception("日期解析错误：" + sd);
                }
                else if (sd == "日期6")
                {
                    DateTime t;
                    if (DateTime.TryParseExact(sv, "yyMMdd", new System.Globalization.CultureInfo("zh-CN"), System.Globalization.DateTimeStyles.None, out t))
                        SetDate(t);
                    else
                    {
                        throw new Exception("日期解析错误：" + sv);
                    }
                }
                else if (sd == "日期5")
                {
                    DateTime t;
                    int val;
                    if (int.TryParse(sv, out val))
                    {
                        t = new DateTime(2000 + val / 1000, 1, 1);
                        t = t.AddDays(val % 1000 - 1);
                        SetDate(t);
                    }
                    else
                    {
                        throw new Exception("日期解析错误：" + sv);
                    }
                }
                else if (sd == "日期DFXK")
                {
                    DateTime t;
                    ushort y, m, d;
                    if (sv == null || sv.Length != 3 || codeDFXKDic.TryGetValue(sv[0], out y) == false || codeDFXKDic.TryGetValue(sv[1], out m) == false || codeDFXKDic.TryGetValue(sv[2], out d) == false)
                    {
                        throw new Exception("日期解析错误：" + sv);
                    }
                    t = new DateTime(2000 + y, m, d);
                    SetDate(t);
                }
                else if (sd == "日期BQ")
                {
                    if (sv == null || sv.Length != 5)
                    {
                        throw new Exception("日期解析错误：" + sv);
                    }
                    SetSubString(sd, sv);
                    //  SetDate(t);
                }
                else if (sd == "日期WJY")
                {
                    SetSubString(sd, sv);
                }
                string sn = "序列号";
                //if (!Settings.Default.打印模板中序列号共享名称.IsNullOrEmpty())
                //    sn = Settings.Default.打印模板中序列号共享名称;
                if (sd == sn /*&& Settings.Default.Check34GQ34 == false*/)
                {
                    //if (int.TryParse(sv, out val))
                    //{
                    //    if (val <= 0)
                    //    {
                    //        throw new Exception("序列号错误：" + sd);
                    //    }
                    //}
                    //else
                    //{
                    //    throw new Exception("序列号解析错误：" + sd);
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal FastReport.Report report;
        //internal Bitmap DrawImg(Bitmap pic, int dpi)
        //{
        //    double rate = dpi / 25.4;

        //    uint hPixel = (uint)(PrintSettings.PaperHeight * rate + 20);
        //    uint wPixel = (uint)(PrintSettings.PaperWidth * rate);
        //    Bitmap bmp = new Bitmap((int)wPixel, (int)hPixel, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        //    bmp.SetResolution(dpi, dpi);
        //    Graphics gr = Graphics.FromImage(bmp);
        //    gr.Clear(Color.White);
        //    gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        //    gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //    gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        //    gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        //    if (pic != null)
        //        gr.DrawImage(pic, 0, 0);

        //    string defaultfont = DefaultFont;
        //    bool reportFontError = true;
        //    if (string.IsNullOrEmpty(PrintSettings.DefaultFont) == false)
        //        defaultfont = PrintSettings.DefaultFont;
        //    for (int i = 0; i < PrintSettings.Strings?.Length; i++)
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        for (int j = 0; j < PrintSettings.Strings[i].strings?.Length; j++)
        //        {
        //            string substr;
        //            if (PrintSettings.Strings[i].strings[j].SubName.IsNullOrEmpty() == false
        //                && Substrings.TryGetValue(PrintSettings.Strings[i].strings[j].SubName, out substr))
        //            {
        //                sb.Append(substr);
        //            }
        //            else if (PrintSettings.UnescapeFlag)
        //                sb.Append(System.Text.RegularExpressions.Regex.Unescape(PrintSettings.Strings[i].strings[j].Content));
        //            else
        //                sb.Append(PrintSettings.Strings[i].strings[j].Content);
        //        }

        //        if (sb.Length > 0)
        //        {
        //            int sz = 6 * dpi / 72;
        //            if (PrintSettings.Strings[i].size > 0)
        //                sz = (int)(PrintSettings.Strings[i].size * dpi / 72);

        //            float szf = 6;
        //            if (PrintSettings.Strings[i].size > 0)
        //                szf = PrintSettings.Strings[i].size;
        //            Font ft;
        //            FontStyle fontstyle;
        //            if (PrintSettings.Strings[i].FontWeight > 4)
        //                fontstyle = FontStyle.Bold;
        //            else
        //                fontstyle = FontStyle.Regular;
        //            if (PrintSettings.Strings[i].Font.IsNullOrEmpty() == false)
        //                ft = new Font(PrintSettings.Strings[i].Font, sz, fontstyle, GraphicsUnit.Pixel);
        //            else
        //                ft = new Font(defaultfont, sz, fontstyle, GraphicsUnit.Pixel);
        //            if (reportFontError)
        //            {
        //                if (ft.Name != defaultfont)
        //                {
        //                    reportFontError = false;
        //                    HMI.Program.MsgBox("未找到字体：" + defaultfont);
        //                }
        //            }
        //            gr.ResetTransform();
        //            float scaleX = 1;
        //            if (PrintSettings.Strings[i].scaleX > 0 && PrintSettings.Strings[i].scaleX != 1)
        //            {
        //                scaleX = PrintSettings.Strings[i].scaleX;
        //                gr.ScaleTransform(scaleX, 1);
        //            }
        //            gr.DrawString(sb.ToString(), ft, Brushes.Black, new PointF((float)(PrintSettings.Strings[i].x * rate / scaleX), (float)(PrintSettings.Strings[i].y * rate)));
        //        }
        //    }

        //    return bmp;
        //}

        public PrintSetting PrintSettings
        {
            get
            {
                return LocalSetting.localSetting.打印配置;
            }
            set
            {
                if (value != null)
                    LocalSetting.localSetting.打印配置 = value;
            }
        }
        public virtual void Dispose()
        {

        }

        internal static void Convert1bpp(Graphics gr, Bitmap imgSource)
        {
            if (gr == null)
                gr = Graphics.FromImage(imgSource);
            gr.ResetTransform();
            var greyMatrix = new float[][]
            {
                new float[] { 0.299f, 0.299f, 0.299f, 0, 0},
                new float[] { 0.587f, 0.587f, 0.587f, 0, 0},
                new float[] { 0.114f, 0.114f, 0.114f, 0, 0},
                new float[] { 0, 0, 0, 1, 0},
                new float[] { 0, 0, 0, 0, 1}
            };
            var ia = new System.Drawing.Imaging.ImageAttributes();
            ia.SetColorMatrix(new System.Drawing.Imaging.ColorMatrix(greyMatrix));
            ia.SetThreshold(0.8f);
            var rc = new Rectangle(0, 0, imgSource.Width, imgSource.Height);
            gr.DrawImage(imgSource, rc, 0, 0, imgSource.Width, imgSource.Height, GraphicsUnit.Pixel, ia);
        }
        internal virtual string GetBarCode()
        {
            return null;
        }
        internal static string GetBarCodeSubStrings(PrintSetting ps, Dictionary<string, string> ss, int id = 0)
        {
            if (ps.DataMatrix?.Length > id)
            {
                StringBuilder sb = new StringBuilder();
                for (int j = 0; j < ps.DataMatrix[id].strings?.Length; j++)
                {
                    if (ps.DataMatrix[id].strings[j].SubName.IsNullOrEmpty() == false
                        && ss.TryGetValue(ps.DataMatrix[id].strings[j].SubName, out string substr))
                    {
                        sb.Append(substr);
                    }
                    else if (ps.DisableUnescapeFlag)
                        sb.Append(ps.DataMatrix[id].strings[j].Content);
                    else
                        sb.Append(System.Text.RegularExpressions.Regex.Unescape(ps.DataMatrix[id].strings[j].Content));
                }
                if (sb.Length > 0)
                    return sb.ToString();
            }
            return null;
        }
        public class PrintLab
        {
            [DllImport("WINPSK.dll")]
            public static extern int OpenPort(string printname);
            [DllImport("WINPSK.dll")]
            public static extern int OpenUSBPort(int port);
            [DllImport("WINPSK.dll")]
            public static extern int PTK_SetPrintSpeed(uint px);
            [DllImport("WINPSK.dll")]
            public static extern int PTK_SetDarkness(uint id);
            [DllImport("WINPSK.dll")]
            public static extern int PTK_SetDirection(char direct);
            [DllImport("WINPSK.dll")]
            public static extern int PTK_SetCoordinateOrigin(uint px, uint py);
            [DllImport("WINPSK.dll")]
            public static extern int ClosePort();
            [DllImport("WINPSK.dll")]
            public static extern int CloseUSBPort();
            [DllImport("WINPSK.dll")]
            public static extern int PTK_SetPrinterState(char state);
            [DllImport("WINPSK.dll")]
            public static extern int PTK_PrintLabel(uint number, uint cpnumber);
            [DllImport("WINPSK.dll")]
            public static extern int PTK_DrawTextTrueTypeW
                                                (int x, int y, int FHeight,
                                                int FWidth, string FType,
                                                int Fspin, int FWeight,
                                                bool FItalic, bool FUnline,
                                                bool FStrikeOut,
                                                string id_name,
                                                string data);
            [DllImport("WINPSK.dll")]
            public static extern int PTK_DrawBarcode(uint px,
                                            uint py,
                                            uint pdirec,
                                            string pCode,
                                            uint pHorizontal,
                                            uint pVertical,
                                            uint pbright,
                                            char ptext,
                                            string pstr);
            [DllImport("WINPSK.dll")]
            public static extern int PTK_SetLabelHeight(uint lheight, uint gapH, int gapOffset, bool bFlag);
            [DllImport("WINPSK.dll")]
            public static extern int PTK_SetLabelWidth(uint lwidth);
            [DllImport("WINPSK.dll")]
            public static extern int PTK_ClearBuffer();
            [DllImport("WINPSK.dll")]
            public static extern int PTK_EnableBackFeed(uint distance);
            [DllImport("WINPSK.dll")]
            public static extern int PTK_DrawRectangle(uint px, uint py, uint thickness, uint pEx, uint pEy);
            [DllImport("WINPSK.dll")]
            public static extern int PTK_DrawLineOr(uint px, uint py, uint pLength, uint pH);
            [DllImport("WINPSK.dll")]
            public static extern int PTK_DrawBar2D_DATAMATRIX(uint x, uint y, uint w, uint v, uint o, uint m, string pstr);
            [DllImport("WINPSK.dll")]
            public static extern int PTK_DrawBar2D_QR(uint x, uint y, uint w, uint v, uint o, uint r, uint m, uint g, uint s, string pstr);
            [DllImport("WINPSK.dll")]
            public static extern int PTK_DrawBar2D_Pdf417(uint x, uint y, uint w, uint v, uint s, uint c, uint px, uint py, uint r, uint l, uint t, uint o, string pstr);
            [DllImport("WINPSK.dll")]
            public static extern int PTK_PcxGraphicsDel(string pid);
            [DllImport("WINPSK.dll")]
            public static extern int PTK_DrawBinGraphics(uint px, uint py, uint pbyte, uint pH, byte[] Gdata);
            [DllImport("WINPSK.dll")]
            public static extern int PTK_BmpGraphicsDownload(string pid, string filename, int iDire);
            [DllImport("WINPSK.dll")]
            public static extern int PTK_PcxGraphicsDownload(string pcxname, string pcxpath);
            [DllImport("WINPSK.dll")]
            public static extern int PTK_PrintPCX(uint px, uint py, string pcxpath);
            [DllImport("WINPSK.dll")]
            public static extern int PTK_DrawPcxGraphics(uint px, uint py, string gname);
            [DllImport("WINPSK.dll")]
            public static extern int PTK_DrawText(uint px, uint py, uint pdirec, uint pFont, uint pHorizontal, uint pVertical, char ptext, string pstr);
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern int DeleteDC(IntPtr hdc);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern int BitBlt(IntPtr hdcDst, int xDst, int yDst, int w, int h, IntPtr hdcSrc, int xSrc, int ySrc, int rop);
        static int SRCCOPY = 0x00CC0020;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO bmi, uint Usage, out IntPtr bits, IntPtr hSection, uint dwOffset);
        static uint BI_RGB = 0;
        static uint DIB_RGB_COLORS = 0;
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct BITMAPINFO
        {
            public uint biSize;
            public int biWidth, biHeight;
            public short biPlanes, biBitCount;
            public uint biCompression, biSizeImage;
            public int biXPelsPerMeter, biYPelsPerMeter;
            public uint biClrUsed, biClrImportant;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 256)]
            public uint[] cols;
        }
        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);
        [DllImport("Winpplb.dll")]
        private static extern int B_Bar2d_Maxi(int x, int y, int cl, int cc, int pc, string data);
        [DllImport("Winpplb.dll")]
        private static extern int B_Bar2d_PDF417(int x, int y, int w, int v, int s, int c, int px,
            int py, int r, int l, int t, int o, string data);
        [DllImport("Winpplb.dll")]
        private static extern int B_Bar2d_PDF417_N(int x, int y, int w, int h, string pParameter, string data);
        [DllImport("Winpplb.dll")]
        private static extern int B_Bar2d_DataMatrix(int x, int y, int r, int l, int h, int v, string data);
        [DllImport("Winpplb.dll")]
        private static extern void B_ClosePrn();
        [DllImport("Winpplb.dll")]
        private static extern int B_CreatePrn(int selection, string filename);
        [DllImport("Winpplb.dll")]
        private static extern int B_Del_Form(string formname);
        [DllImport("Winpplb.dll")]
        private static extern int B_Del_Pcx(string pcxname);
        [DllImport("Winpplb.dll")]
        private static extern int B_Draw_Box(int x, int y, int thickness, int hor_dots,
            int ver_dots);
        [DllImport("Winpplb.dll")]
        private static extern int B_Draw_Line(char mode, int x, int y, int hor_dots, int ver_dots);
        [DllImport("Winpplb.dll")]
        private static extern int B_Error_Reporting(char option);
        [DllImport("Winpplb.dll")]
        private static extern IntPtr B_Get_DLL_Version(int nShowMessage);
        [DllImport("Winpplb.dll")]
        private static extern int B_Get_DLL_VersionA(int nShowMessage);
        [DllImport("Winpplb.dll")]
        private static extern int B_Get_Graphic_ColorBMP(int x, int y, string filename);
        [DllImport("Winpplb.dll")]
        private static extern int B_Get_Graphic_ColorBMPEx(int x, int y, int nWidth, int nHeight,
            int rotate, string id_name, string filename);
        [DllImport("Winpplb.dll")]
        private static extern int B_Get_Graphic_ColorBMP_HBitmap(int x, int y, int nWidth, int nHeight,
           int rotate, string id_name, IntPtr hbm);
        [DllImport("Winpplb.dll")]
        private static extern int B_Get_Pcx(int x, int y, string filename);
        [DllImport("Winpplb.dll")]
        private static extern int B_Initial_Setting(int Type, string Source);
        [DllImport("Winpplb.dll")]
        private static extern int B_WriteData(int IsImmediate, byte[] pbuf, int length);
        [DllImport("Winpplb.dll")]
        private static extern int B_WriteData(int IsImmediate, string pbuf, int length);
        [DllImport("Winpplb.dll")]
        private static extern int B_ReadData(byte[] pbuf, int length, int dwTimeoutms);
        [DllImport("Winpplb.dll")]
        private static extern int B_Load_Pcx(int x, int y, string pcxname);
        [DllImport("Winpplb.dll")]
        private static extern int B_Open_ChineseFont(string path);
        [DllImport("Winpplb.dll")]
        private static extern int B_Print_Form(int labset, int copies, string form_out, string var);
        [DllImport("Winpplb.dll")]
        private static extern int B_Print_MCopy(int labset, int copies);
        [DllImport("Winpplb.dll")]
        private static extern int B_Print_Out(int labset);
        [DllImport("Winpplb.dll")]
        private static extern int B_Prn_Barcode(int x, int y, int ori, string type, int narrow,
            int width, int height, char human, string data);
        [DllImport("Winpplb.dll")]
        private static extern void B_Prn_Configuration();
        [DllImport("Winpplb.dll")]
        private static extern int B_Prn_Text(int x, int y, int ori, int font, int hor_factor,
            int ver_factor, char mode, string data);
        [DllImport("Winpplb.dll")]
        private static extern int B_Prn_Text_Chinese(int x, int y, int fonttype, string id_name,
            string data);
        [DllImport("Winpplb.dll")]
        private static extern int B_Prn_Text_TrueType(int x, int y, int FSize, string FType,
            int Fspin, int FWeight, int FItalic, int FUnline, int FStrikeOut, string id_name,
            string data);
        [DllImport("Winpplb.dll")]
        private static extern int B_Prn_Text_TrueType_W(int x, int y, int FHeight, int FWidth,
            string FType, int Fspin, int FWeight, int FItalic, int FUnline, int FStrikeOut,
            string id_name, string data);
        [DllImport("Winpplb.dll")]
        private static extern int B_Select_Option(int option);
        [DllImport("Winpplb.dll")]
        private static extern int B_Select_Option2(int option, int p);
        [DllImport("Winpplb.dll")]
        private static extern int B_Select_Symbol(int num_bit, int symbol, int country);
        [DllImport("Winpplb.dll")]
        private static extern int B_Select_Symbol2(int num_bit, string csymbol, int country);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_Backfeed(char option);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_Backfeed_Offset(int offset);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_CutPeel_Offset(int offset);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_BMPSave(int nSave, string strBMPFName);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_Darkness(int darkness);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_DebugDialog(int nEnable);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_Direction(char direction);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_Form(string formfile);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_Labgap(int lablength, int gaplength);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_Labwidth(int labwidth);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_Originpoint(int hor, int ver);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_Prncomport(int baud, char parity, int data, int stop);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_Prncomport_PC(int nBaudRate, int nByteSize, int nParity,
            int nStopBits, int nDsr, int nCts, int nXonXoff);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_Speed(int speed);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_ProcessDlg(int nShow);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_ErrorDlg(int nShow);
        [DllImport("Winpplb.dll")]
        internal static extern int B_GetUSBBufferLen();
        [DllImport("Winpplb.dll")]
        private static extern int B_EnumUSB(byte[] buf);
        [DllImport("Winpplb.dll")]
        private static extern int B_CreateUSBPort(int nPort);
        [DllImport("Winpplb.dll")]
        private static extern int B_ResetPrinter();
        [DllImport("Winpplb.dll")]
        private static extern int B_GetPrinterResponse(byte[] buf, int nMax);
        [DllImport("Winpplb.dll")]
        private static extern int B_TFeedMode(int nMode);
        [DllImport("Winpplb.dll")]
        private static extern int B_TFeedTest();
        [DllImport("Winpplb.dll")]
        private static extern int B_CreatePort(int nPortType, int nPort, string filename);
        [DllImport("Winpplb.dll")]
        private static extern int B_Execute_Form(string form_out, string var);
        [DllImport("Winpplb.dll")]
        private static extern int B_Bar2d_QR(int x, int y, int model, int scl, char error,
            char dinput, int c, int d, int p, string data);
        [DllImport("Winpplb.dll")]
        private static extern int B_GetNetPrinterBufferLen();
        [DllImport("Winpplb.dll")]
        private static extern int B_EnumNetPrinter(byte[] buf);
        [DllImport("Winpplb.dll")]
        private static extern int B_CreateNetPort(int nPort);
        [DllImport("Winpplb.dll")]
        internal static extern int B_Set_LabelForSmartPrint(int lablength, int gaplength);
        [DllImport("Winpplb.dll")]
        private static extern int B_Prn_Text_TrueType_Uni(int x, int y, int FSize, string FType,
            int Fspin, int FWeight, int FItalic, int FUnline, int FStrikeOut, string id_name,
            byte[] data, int format);
        [DllImport("Winpplb.dll")]
        private static extern int B_Prn_Text_TrueType_UniB(int x, int y, int FSize, string FType,
            int Fspin, int FWeight, int FItalic, int FUnline, int FStrikeOut, string id_name,
            byte[] data, int format);
        [DllImport("Winpplb.dll")]
        private static extern int B_GetUSBDeviceInfo(int nPort, byte[] pDeviceName,
            out int pDeviceNameLen, byte[] pDevicePath, out int pDevicePathLen);
        [DllImport("Winpplb.dll")]
        private static extern int B_Set_EncryptionKey(string encryptionKey);
        [DllImport("Winpplb.dll")]
        private static extern int B_Check_EncryptionKey(string decodeKey, string encryptionKey,
            int dwTimeoutms);
    }
}
