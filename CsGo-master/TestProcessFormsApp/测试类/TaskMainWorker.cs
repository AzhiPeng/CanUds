using CCWin.SkinControl;
using CCWin.Win32.Const;

using JLRScan;
using JLRScan.Log;
//using //LoadPlateSerialPort.LoadBoxAT03;
//using RegenerativeTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using TestProcessFormsApp.Communication;
using TestProcessFormsApp.Communication.CAN;
using TestProcessFormsApp.Communication.Master_slave;
using TestProcessFormsApp.Log;
using TestProcessFormsApp.Public;

namespace TestProcessFormsApp.测试类
{
    public class TaskMainWorker
    {
        #region 单例
        static readonly Lazy<TaskMainWorker> instance = new Lazy<TaskMainWorker>(() => new TaskMainWorker());
        /// <summary>
        /// 单例
        /// </summary>
        public static TaskMainWorker Instance => instance.Value;
        #endregion
        public  bool DVTestStartFlag = false;
        public bool TempTestStartFlag = false;
        public bool DVTestEndFlag = false;
        public int 测试轮数 = 0;
        public static double DvDateTime;
        public 时间分段 GetNowDvTime = 时间分段._0_75;
        public static string 当前瞬时工况测试项 { set; get; }
        public ScanLog log = new ScanLog();
        public enum 时间分段
        {
            _0_75 = 0,
            _75_150 = 1,
            _150_225 = 2,
            _225_300 = 3,
            _300_375 = 4,
            _375_450 = 5,
            _450_525 = 6,
            _525_600 = 7,
        }
        static void Log(string msg)
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} {msg}");
        }
        static async Task MainWorker()
        {
           // await 监控任务();
        }

        //   static double[] timeSpan = { 0, 7.5, 15, 22.5, 30, 37.5, 45, 52.5, 60 };
        const double k = 1.0;
        static double[] timeSpan = { 0,100,120, 220, 240,340,360,460,480 };
        public  static double[] WoKerNum = { 1, 1, 1, 1, 1, 1, 1, 1 };
        public static byte TestStep = 1;
        public static int TestMax = 0;

        public static double DiffSeconds(DateTime startTime, DateTime endTime)
        {
            TimeSpan span = endTime - startTime; // 直接相减更简洁
            return span.TotalSeconds; // 总秒数（含小数）
        }
        public void StartSingleTask(object o)
        {
            ScanLog log = new ScanLog();
            bool 常开标志 = false;
            int FailSum = 0;
            try
            {
                DateTime StartTime = DateTime.Now;
                double Seconds = 0;
                int PassSum = 0;
                int NowPass = 0;
                int PassTestSum = 0;
                int ReadyTestSum = 0;
                int index = 0;
                int lastWarnNoStep = int.MinValue;
                while (Instance.DVTestStartFlag)
                {
                    PassSum = 0;
                    NowPass = 0;
                    PassTestSum = 0;
                    ReadyTestSum = 0;
                    index = 0;
                    foreach (var caseInterface in CaseConfig.caseList)
                    {
                        if (caseInterface.获取测试顺序() == TestStep)
                            PassSum++;
                        if (caseInterface.GetState() == 测试结果.合格 || (LocalSetting.localSetting.DianJianState == true && caseInterface.GetState() == 测试结果.不合格))
                            ReadyTestSum++;
                        if(caseInterface.GetState() == 测试结果.测试中)
                        {
                            if(TestMax < index)
                                TestMax = index;
                        }
                        index++;
                    }

                    // 2026-05-11: 若当前步号无任何测试项，打印一次日志并跳转到下一个可执行步号。
                    if (PassSum == 0)
                    {
                        var orderedSteps = CaseConfig.caseList
                            .Select(c => c.获取测试顺序())
                            .Distinct()
                            .OrderBy(s => s)
                            .ToList();

                        int nextStep = orderedSteps.FirstOrDefault(s => s > TestStep);
                        if (nextStep == 0 && orderedSteps.Count > 0)
                            nextStep = orderedSteps[0];

                        if (lastWarnNoStep != TestStep)
                        {
                            log.InsertLog($"[TaskWorker] 当前Step={TestStep}未命中测试项，已知Step={string.Join(",", orderedSteps)}，下一步={nextStep}");
                            lastWarnNoStep = TestStep;
                        }

                        if (nextStep >= 0 && nextStep != TestStep)
                            TestStep = (byte)nextStep;

                        Thread.Sleep(30);
                        continue;
                    }

                    foreach (var caseInterface in CaseConfig.caseList)
                    {
                        try
                        {
                            Seconds = DiffSeconds(StartTime, DateTime.Now);
                            DvDateTime = Seconds;
                            if (caseInterface.获取测试顺序() == TestStep)
                            {
                                caseInterface.Start(负载状态.开启, 测试方式.自动);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.InsertLog(caseInterface.GetCaseName()+ ex.Message + "19");
                            Console.WriteLine(caseInterface.GetCaseName() + ex.Message + "19");
                        }
                      
                    }
           
                    foreach (var caseInterface in CaseConfig.caseList)
                    {
                        if (caseInterface.获取测试顺序() == TestStep)
                        {
                            if (caseInterface.GetState() == 测试结果.合格 || (LocalSetting.localSetting.DianJianState == true && caseInterface.GetState() == 测试结果.不合格))
                                NowPass++;
                        }
                        if (caseInterface.GetState() == 测试结果.不合格 && LocalSetting.localSetting.DianJianState == false)
                        {
                            WinForm.本次为复测 = false;
                            WinForm._int扫码结果 = 0;
                            WinForm._IsActie = 0;
                            WinForm.本轮测试结果 = 测试结果.不合格;
                            DVTestStartFlag = false;
                            WinForm.Save(3);
                            LocalSetting.localSetting.TestNumb++;
                            WinForm.SaveLocalSetting();
                            WinForm._ScanString = "";
                            break;
                        }
                        if (caseInterface.GetState() == 测试结果.合格)
                        {
                            //WinForm._int扫码结果 = 0;
                            PassTestSum++;
                            if (WinForm.循环测试标志 == true)
                                DVTestEndFlag = true;
                        }

                    }
                    
                    if (NowPass == PassSum)//当前测试项都合格 跳下一步骤
                    {
                        TestStep++;
                    }
                    if(PassTestSum == CaseConfig.caseList.Count)
                    {
                        WinForm.本次为复测 = false;
                        WinForm._IsActie = 0;
                        WinForm.本轮测试结果 = 测试结果.合格;
                        DVTestStartFlag = false;
                        Thread.Sleep(3000);
                        WinForm.Save(2);
                        WinForm.TryPrintLabelWithOfflineCache("测试合格自动打印");
                        LocalSetting.localSetting.TestNumb++;
                        WinForm.SaveLocalSetting();
                    }
                    if (LocalSetting.localSetting.DianJianState == true && ReadyTestSum == CaseConfig.caseList.Count)
                    {
                        DVTestStartFlag = false;
                        LocalSetting.localSetting.TestNumb++;
                        WinForm.SaveLocalSetting();
                        WinForm._ScanString = "";
                        break;
                    }
                    Thread.Sleep(30);

                }              
                }
                catch (Exception ex)
                {
                    log.InsertLog(ex.Message + "20");
                }
        }
            
        
        //double staticTime = 0;
        //double staticinitTime = 0;
        //double[] timeSpanSleep = 
        //{ 
        //     0,  100, 120, 125, 
        //     225, 245, 250,
        //     350, 370, 375, 
        //     475, 495, 500
        //};
        double[] timeSpanSleep =
       {
             0,  100, 150, 160,
             260, 310, 320,
             420, 470, 480,
             580, 630, 640,645
        };
        public static bool InitFlag1 = false;
        public static bool InitFlag2 = false;
        public static bool InitFlag3 = false;
        public static bool InitFlag4 = false;
        public static bool InitFlag5 = false;
        public static bool InitFlag6 = false;
        public static bool InitFlag7 = false;
        public static bool InitFlag8 = false;
        public void ResetInit()
        {
            InitFlag1 = false;
            InitFlag2 = false;
            InitFlag3 = false;
            InitFlag4 = false;
            InitFlag5 = false;
            InitFlag6 = false;
            InitFlag7 = false;
            InitFlag8 = false;
            foreach (var caseInterface in CaseConfig.caseList)
            {
                caseInterface.Reset测试时序状态1();
                caseInterface.Reset测试时序状态2();
                caseInterface.Reset测试时序状态3();
                caseInterface.Reset测试时序状态4();
                caseInterface.Reset测试时序状态5();
                caseInterface.Reset测试时序状态6();
                caseInterface.Reset测试时序状态7();
                caseInterface.Reset测试时序状态8();
                caseInterface.Reset测试时序状态9();
                caseInterface.Reset测试时序状态10();
                caseInterface.Reset测试时序状态11();
                caseInterface.Reset测试时序状态12();
            }
        }
        static byte StaticStep = 0;
        static byte FianllyFlag = 0;
        DateTime TempStartTime = DateTime.Now;
        
        /// <summary>
        /// 测试工况带睡眠模式
        /// </summary>
        /// <param name="o"></param>
        public void StartSingleTaskWithSleep()
        {
            //ScanLog log = new ScanLog();
            ////bool 常开标志 = false;
            //int FailSum = 0;
            //try
            //{
            //    double Seconds = 0;
            //    foreach (var caseInterface in CaseConfig.caseList)
            //    {
            //        try
            //        {
            //            if(TestStep == caseInterface.获取测试顺序())
            //            {
            //                caseInterface.Start(负载状态.开启, 测试方式.自动);
            //            }
            //            Seconds = DiffSeconds(TempStartTime, DateTime.Now);
            //            DvDateTime = Seconds;
                      
            //            }
                       
            //        }
            //        catch (Exception ex)
            //        {
            //            log.InsertLog(ex.Message + "21");
            //        }

            //    }
            //    try
            //    {
            //        if (Seconds >= timeSpanSleep[1] && Seconds < timeSpanSleep[2])//100-120
            //        {
            //            {
            //                if (InitFlag1 == false)
            //                {
            //                    InitFlag1 = true;
            //                    TestStaticStep = 0;
            //                    RegenerativeCommunicate.Instance.SetVoltage(12.0f);
            //                }
            //                StaticCurrentTest();
            //            }
            //        }
            //        if (Seconds >= timeSpanSleep[2] && Seconds < timeSpanSleep[3])//120-125  只要执行一次。这个还没做好0522
            //        {
            //            if (InitFlag2 == false)
            //            {
            //                InitFlag2 = true;
            //                CAEA008TCPModbus.Instance._线圈管理器.插入线圈(301, 0);
            //                CAEA008TCPModbus.Instance._线圈管理器.插入线圈(300, 0);
            //                WinForm.ReConnectFlag = true;
            //                AT03TCPCommunicate.Instance.PinWriteHL("HL-37-0");
            //                AT03TCPCommunicate.Instance.PinWriteHL("HL-40-0");
            //                AT03TCPCommunicate.Instance.PinWriteHL("HL-68-0");
            //                AT03TCPCommunicate.Instance.PinWriteHL("HL-69-0");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-3-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-4-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-5-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-6-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-7-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-8-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-9-200-5000");
            //                WinForm.ReConnectFlag = false;
            //                RegenerativeCommunicate.Instance.SetVoltage(14.0f);
            //                CANUDPClient.StopSendfalg = false;
            //            }
            //        }
            //        if (Seconds >= timeSpanSleep[4] && Seconds < timeSpanSleep[5])//225-245
            //        {
            //            if (InitFlag3 == false)
            //            {
            //                TestStaticStep = 0;
            //                InitFlag3 = true;
            //                RegenerativeCommunicate.Instance.SetVoltage(12.0f);
            //            }
            //            StaticCurrentTest();
            //        }
            //        else if (Seconds >= timeSpanSleep[5] && Seconds < timeSpanSleep[6])//245-250  
            //        {
            //            if (InitFlag4 == false)
            //            {
            //                InitFlag4 = true;
            //                CAEA008TCPModbus.Instance._线圈管理器.插入线圈(301, 0);
            //                CAEA008TCPModbus.Instance._线圈管理器.插入线圈(300, 0);
            //                WinForm.ReConnectFlag = true;
            //                AT03TCPCommunicate.Instance.PinWriteHL("HL-37-0");
            //                AT03TCPCommunicate.Instance.PinWriteHL("HL-40-0");
            //                AT03TCPCommunicate.Instance.PinWriteHL("HL-68-0");
            //                AT03TCPCommunicate.Instance.PinWriteHL("HL-69-0");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-3-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-4-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-5-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-6-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-7-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-8-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-9-200-5000");
            //                WinForm.ReConnectFlag = false;
            //                RegenerativeCommunicate.Instance.SetVoltage(14.0f);
            //                CANUDPClient.StopSendfalg = false;
            //            }
            //        }
            //        if (Seconds >= timeSpanSleep[7] && Seconds < timeSpanSleep[8])//350-370
            //        {
            //            if (InitFlag5 == false)
            //            {
            //                TestStaticStep = 0;
            //                InitFlag5 = true;
            //                RegenerativeCommunicate.Instance.SetVoltage(12.0f);
            //            }
            //            StaticCurrentTest();
            //        }
            //        else if (Seconds >= timeSpanSleep[8] && Seconds < timeSpanSleep[9])//370-375  
            //        {
            //            if (InitFlag6 == false)
            //            {
            //                InitFlag6 = true;
            //                CAEA008TCPModbus.Instance._线圈管理器.插入线圈(301, 0);
            //                CAEA008TCPModbus.Instance._线圈管理器.插入线圈(300, 0);
            //                WinForm.ReConnectFlag = true;
            //                AT03TCPCommunicate.Instance.PinWriteHL("HL-37-0");
            //                AT03TCPCommunicate.Instance.PinWriteHL("HL-40-0");
            //                AT03TCPCommunicate.Instance.PinWriteHL("HL-68-0");
            //                AT03TCPCommunicate.Instance.PinWriteHL("HL-69-0");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-3-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-4-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-5-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-6-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-7-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-8-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-9-200-5000");
            //                WinForm.ReConnectFlag = false;
            //                RegenerativeCommunicate.Instance.SetVoltage(14.0f);
            //                CANUDPClient.StopSendfalg = false;
            //            }
            //        }
            //        if (Seconds >= timeSpanSleep[10] && Seconds < timeSpanSleep[11])//475-495
            //        {
            //            if (InitFlag7 == false)
            //            {
            //                TestStaticStep = 0;
            //                InitFlag7 = true;
            //                RegenerativeCommunicate.Instance.SetVoltage(12.0f);
            //            }
            //            StaticCurrentTest();
            //        }
            //        else if (Seconds >= timeSpanSleep[11] && Seconds < timeSpanSleep[12])//495-500  
            //        {
            //            if (InitFlag8 == false)
            //            {
            //                InitFlag8 = true;
            //                CAEA008TCPModbus.Instance._线圈管理器.插入线圈(301, 0);
            //                CAEA008TCPModbus.Instance._线圈管理器.插入线圈(300, 0);
            //                WinForm.ReConnectFlag = true;
            //                AT03TCPCommunicate.Instance.PinWriteHL("HL-37-0");
            //                AT03TCPCommunicate.Instance.PinWriteHL("HL-40-0");
            //                AT03TCPCommunicate.Instance.PinWriteHL("HL-68-0");
            //                AT03TCPCommunicate.Instance.PinWriteHL("HL-69-0");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-3-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-4-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-5-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-6-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-7-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-8-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-9-200-5000");
            //                WinForm.ReConnectFlag = false;
            //                RegenerativeCommunicate.Instance.SetVoltage(14.0f);
            //                CANUDPClient.StopSendfalg = false;
            //                FianllyFlag = 0;
            //            }
            //        }
            //        if (Seconds > timeSpanSleep[12] && Seconds < timeSpanSleep[13])
            //        {
            //            if(FianllyFlag == 0)
            //            {
            //                FianllyFlag = 1;
            //                foreach (var temp in CaseConfig.caseList)
            //                {
            //                    if (temp.GetState() == 测试结果.未开始)
            //                    {
            //                        temp.Set结果(测试结果.不合格);
            //                        temp.Set历史结果(测试结果.不合格);
            //                        temp.Set历史结果数据(temp.GetData());
            //                        log.InsertLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + temp.GetCaseName() + "." + temp.GetData() + "," + temp.GetState());
            //                        LogHelperError.Error(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + temp.GetCaseName() + "." + temp.GetData() + "," + temp.GetState());
            //                    }
            //                    if (temp.GetState() == 测试结果.不合格)
            //                        FailSum++;
            //                }
            //                if (FailSum > 0)
            //                {
            //                    WinForm.本轮测试结果 = 测试结果.不合格;
            //                }
            //                else
            //                {
            //                    WinForm.本轮测试结果 = 测试结果.合格;
            //                }
            //                测试轮数++;
            //              //  DVTestStartFlag = false;
            //                string filePath = Path.Combine(LocalSetting.localSetting.FileNamePath, DateTime.Now.ToString("yyMMddHHmmss") + ".csv");
            //                LogTestDatacsv.WriteData(filePath, WinForm.CsVLog);

            //                WinForm.CsVLog.Clear();
            //                List<string> Namelist = new List<string>();
            //                Namelist.Add("时间戳");
            //                foreach (var temp in CaseConfig.caseList)
            //                {
            //                    Namelist.Add(temp.GetCaseName());
            //                    temp.Init();
            //                    temp.DVResultInit();
            //                }
            //                Namelist.Add("工作电流");
            //                Namelist.Add("静态电流");
            //                string[] Namearray = Namelist.ToArray();
            //                WinForm.CsVLog.Add(Namearray);
            //                ResetInit();
            //                WinForm.ReConnectFlag = true;
            //                AT03TCPCommunicate.Instance.PinWriteHL("HL-37-0");
            //                AT03TCPCommunicate.Instance.PinWriteHL("HL-40-0");
            //                AT03TCPCommunicate.Instance.PinWriteHL("HL-68-0");
            //                AT03TCPCommunicate.Instance.PinWriteHL("HL-69-0");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-3-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-4-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-5-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-6-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-7-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-8-200-5000");
            //                AT03TCPCommunicate.Instance.PinWritePWM("PWM-9-200-5000");
            //                WinForm.ReConnectFlag = false;
            //            }
                      
                        
            //            //if (WinForm.循环测试标志 == true)
            //            //    DVTestEndFlag = true;
            //        }
            //        if ( Seconds > timeSpanSleep[13])
            //        {
            //            PowerONStep = 1;
            //         //   TempStartTime = DateTime.Now;
            //        }
            //    } catch (Exception ex) {

            //        }
            //        Thread.Sleep(30);

                
            //}
            //catch (Exception ex)
            //{
            //    log.InsertLog(ex.Message + "22");
            //}
        }
        void SettimeSpanSleep(double SleepTime)
        {
            //timeSpanSleep[2] = SleepTime + 100;
            //timeSpanSleep[3] = timeSpanSleep[2] + 100;
            //timeSpanSleep[4] = timeSpanSleep[3] + SleepTime;
            //timeSpanSleep[5] = timeSpanSleep[4] + 100;
            //timeSpanSleep[6] = timeSpanSleep[5] + SleepTime;
            //timeSpanSleep[7] = timeSpanSleep[6] + 100;
            //timeSpanSleep[8] = timeSpanSleep[7] + SleepTime;
        }
      
    
        public CANUDPClient FindClientByName(string targetName)
        {
            return WinForm.CANUDPClients.FirstOrDefault(c => c.Name == targetName);
        }
        public byte StepTime(DateTime time, int delaytime, byte step)
        {
            if ((DateTime.Now - time).TotalMilliseconds > delaytime)
            {
                return ++step;
            }
            return 0;
        }
    }
}
