using CCWin;
using CCWin.SkinClass;

using HMI;
using HMIPLC;
using JLRScan;
using JLRScan.Frm;
using JLRScan.Log;
using JModbusClient;

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
//using TestProcessFormsApp.Boards;
using TestProcessFormsApp.Communication;
using TestProcessFormsApp.Communication.CAN;
using TestProcessFormsApp.Communication.Master_slave;
using TestProcessFormsApp.Frm;
using TestProcessFormsApp.Log;
using TestProcessFormsApp.Properties;
using TestProcessFormsApp.Public;
using TestProcessFormsApp.测试类;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
//using TouchSocket.Sockets;
//using static LoadPlateSerialPort.LoadBoxAT03.AT03TCPCommunicate;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Gpg.NET;
using KeyManageForGm.Model;

namespace TestProcessFormsApp
{
    public partial class WinForm : Skin_VS
    {
        ScanLog log = new ScanLog();
        private ToolStripMenuItem 手动补传追溯数据ToolStripMenuItem;
        private ToolStripMenuItem 手动补打标签ToolStripMenuItem;
        private ToolStripMenuItem 按关键字补打一条ToolStripMenuItem;
        private FrmPendingPrintJobs pendingPrintJobsForm;
        private static readonly object pendingPrintLock = new object();
        private static string PendingPrintFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PendingLabelPrints.json");

        class PendingPrintJob
        {
            public string Id { get; set; } = Guid.NewGuid().ToString("N");
            public DateTime CreateTime { get; set; } = DateTime.Now;
            public DateTime LastTryTime { get; set; } = DateTime.MinValue;
            public int RetryCount { get; set; } = 0;
            public string LastError { get; set; } = "";
            public string Trigger { get; set; } = "";
            public string ProductModel { get; set; } = "";
            public string TraceCode { get; set; } = "";
            public string BarCode { get; set; } = "";
            public string SerialNum { get; set; } = "";
            public string CheckNo { get; set; } = "";
            public string TeamNo { get; set; } = "";
            public string Ecuid { get; set; } = "";
        }
      //  public static List<Board> Boards = new List<Board>();
        public static List<string[]> CsVLog = new List<string[]>();
        public static BindingList<ParseTestItems> ParseTestItemsSetting = new BindingList<ParseTestItems>();
      //  public static List<Tuple<PinRegisterType, byte, List<int>>> RegisterPins = new List<Tuple<PinRegisterType, byte, List<int>>>();
        public ScanLog ScanMessage { get; set; } = new ScanLog();
        public static System.Drawing.Image Pass = Properties.Resources.合格;
        public static System.Drawing.Image Fail = Properties.Resources.不合格;
        public static System.Drawing.Image Unknown = Properties.Resources.就绪;
        public static System.Drawing.Image Test = Properties.Resources.正在测试;
        public static int SendBagSum = 0;
        public static int RevcBagSum = 0;
        public static List<CANUDPClient> CANUDPClients = new List<CANUDPClient>();
       // public static Dictionary<string, CurrentCollectionModbus> CurrentCollections = new Dictionary<string, CurrentCollectionModbus>();
        public static Dictionary<string, ModbusTcpMaster> Caea008Groups = new Dictionary<string, ModbusTcpMaster>();
     //   public static Dictionary<string, CurrentCollectionTcp> CurrentCollectionTcps = new Dictionary<string, CurrentCollectionTcp>();
        public static ConcurrentQueue<string> HLQueue = new ConcurrentQueue<string>();
        public static ConcurrentQueue<string> PWMQueue = new ConcurrentQueue<string>();
        public static string CANID = "";
        public static TupleRequstInfoModel _tupleTask = default;
        public static bool CanInitFlag = false;
        public static bool CanCycleInitFlag = false;
        public static List<EcuidInfoModel> ecuidInfoModels;
        public static string CANSendID = "";
        public static string CANRevID = "";
        public static CANSelectType CANType;
        //    TcpTouchSever socketClient = new TcpTouchSever();
        public WinForm()
        {  
            InitializeComponent();

            DoubleBufferDataGridView.DoubleBufferedDataGirdView(DataGridView_TestFrm, true);
          
        }
        public static string sqlstr;
        static void DBConn()
        {
            //string DBName = LocalSetting.localSetting.DBName;
            //string ServerName = LocalSetting.localSetting.ServerName;
            //string User = "sa";// LocalSetting.localSetting.User;
            //string password = "1";//LocalSetting.localSetting.password;
            //sqlstr = @"Password=" + password + ";Persist Security Info=True;User ID=" + User + ";Initial Catalog=" + DBName + ";Data Source=" + ServerName;
        }

        //  public static ProgressBar toolStripProgressBar1 = new ProgressBar();
        private List<string> 型号列表list = new List<string>();
        void Load型号列表()
        {
            try
            {
                型号列表list.Clear();
                型号列表.Items.Clear();
                型号列表list = Directory.GetFiles($"{LocalSetting.SystemDIR}\\产品测试项配置", "*.csv")
                .Select(Path.GetFileNameWithoutExtension)
                .ToList();
                 
                foreach(var xh in 型号列表list)
                {
                    型号列表.Items.Add(xh);
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void 开始ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (型号列表.SelectedIndex == -1)
                {
                    MessageBox.Show("请选择型号！");
                    return;
                }
                型号列表.Enabled = false; 
                
                ParseTestLoad($"{LocalSetting.SystemDIR}\\产品测试项配置\\" + 型号列表.Text);//加载对应型号测试项
                toolStripProgressBar1.Visible = true;
                CaseConfig.CreateParseItem();
                DataGridView_TestFrm.RowCount = CaseConfig.caseList.Count;
              //  BoardConfig_Load();
                LocalSettingLoad();
                // 2026-05-11: 按当前选中型号加载CAN节点配置，避免型号写死导致报文/信号不匹配。
                CANUDPClientsLoad($"{LocalSetting.SystemDIR}\\产品报文配置\\0Y4G40AAEWWA");
             //   MatchingParseTestAndBoard();
                //    DBConn();
                StartButtonEnable();
                InitExitFlag = false;
                InitStep = 0;
             //   this.Text = 型号列表.Text +" "+ LocalSetting.localSetting.CurrentGw +"工位";
                ThreadPool.QueueUserWorkItem(InitThread);
             //   ThreadPool.QueueUserWorkItem(GongZhuangThread);
             //   StartSerial();
                Timer_Serial.Start();
                // 1. 初始化键列表和先前状态
                // 配置DataGridView
                //stateKeys = GZState.Keys.ToList();
                //dataGridView_GZState.VirtualMode = true;
                //dataGridView_GZState.RowCount = stateKeys.Count;
                //dataGridView_GZState.ColumnCount = 2;
                //dataGridView_GZState.Columns[0].Name = "状态";
                //dataGridView_GZState.Columns[1].Name = "值";
                //dataGridView_GZState.CellValueNeeded += dataGridView_GZState_CellValueNeeded;
            }
            catch (Exception ex)
            {
                log.InsertLog(ex.Message + "16");
                throw (ex);
            }

        }
        private void 停止ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            型号列表.Enabled = true;
            //开始ToolStripMenuItem.Enabled = true;
            //停止ToolStripMenuItem.Enabled = false;
            //StopButtonEnable();
        }
        const ushort Start1 = 1010;
        const ushort Start2 = 1030;
        const ushort UnLock = 1050;
        const ushort WuLiao = 1070;
        const ushort GaiBan = 1090;
        const ushort TanZhen = 1110;
        bool StartFlag = false;
        bool LastStartFlag = false;
        public Dictionary<string, ushort> GZD = new Dictionary<string, ushort>()
        {
            {"启动1", Start1},
            {"启动2", Start2},
            {"解锁",  UnLock},
            {"物料",  WuLiao},
            {"盖板",  GaiBan},
        };
        public Dictionary<string, ushort> GZState = new Dictionary<string, ushort>()
        {
            {"启动1", 9999},
            {"启动2", 9999},
            {"解锁",  9999},
            {"物料",  9999},
            {"盖板",  9999},
        };
        private BindingList<DictionaryEntry> bindingList;
        void GongZhuangThread(object o)
        {
            while (true)
            {
                Thread.Sleep(10);
                if (!CAEA008TCPModbus.Instance.DList.ContainsKey(Start1) || !CAEA008TCPModbus.Instance.DList.ContainsKey(Start2) || !CAEA008TCPModbus.Instance.DList.ContainsKey(UnLock)
                    || !CAEA008TCPModbus.Instance.DList.ContainsKey(WuLiao) || !CAEA008TCPModbus.Instance.DList.ContainsKey(GaiBan))
                {
                    continue;
                }
                if (CAEA008TCPModbus.Instance.DList[Start1] < 500 && CAEA008TCPModbus.Instance.DList[Start2] < 500 && CAEA008TCPModbus.Instance.DList[UnLock] > 1000 &&
                   CAEA008TCPModbus.Instance.DList[WuLiao] < 500 && CAEA008TCPModbus.Instance.DList[GaiBan] < 500)
                {
                    StartFlag = true;
                }
                else
                {
                    StartFlag = false;
                }
                if (StartFlag == true && LastStartFlag == false && 本轮测试结果 != 测试结果.测试中 && 本轮测试结果 != 测试结果.不合格)
                {
                    CANUDPClient.StopSendfalg = false;
                    CAEA008TCPModbus.Instance._寄存器管理器.插入寄存器(32, 1);
                    CAEA008TCPModbus.Instance._线圈管理器.插入线圈(303, 0);
                    CAEA008TCPModbus.Instance._线圈管理器.插入线圈(301, 0);

                    TaskMainWorker.TestStep = 1;
                    Thread.Sleep(3000);
                    ThreadPool.QueueUserWorkItem(StartTest);
                }
                if (CAEA008TCPModbus.Instance.DList[UnLock] < 500 && 本轮测试结果 == 测试结果.不合格)
                {
        
                    Thread.Sleep(3000);
                    if (CAEA008TCPModbus.Instance.DList[32] == 1)
                        CAEA008TCPModbus.Instance._寄存器管理器.插入寄存器(32, 0);
                    本轮测试结果 = 测试结果.未开始;
                    _IsActie = 0;
                    TaskMainWorker.Instance.DVTestStartFlag = false;
                }
                LastStartFlag = StartFlag;
                //foreach(var temp in GZD)
                //{
                //    {
                //        Thread.Sleep(1);
                //        GZState[temp.Key] = CAEA008TCPModbus.Instance.DList[temp.Value];
                //        UpdateState(temp.Key, CAEA008TCPModbus.Instance.DList[temp.Value]);
                //    }
                //}
            }

        }
        void StartButtonEnable()
        {
            Communication_state.BackgroundImage = Red;
            开始ToolStripMenuItem.Enabled = false;
            停止ToolStripMenuItem.Enabled = true;
            //     CAEA008TCPModbus.Instance.ExitFlag = false;
            Refalshtimer.Enabled = true;
        }
        void StopButtonEnable()
        {
            停止ToolStripMenuItem.Enabled = false;
            Refalshtimer.Enabled = false;
            CAEA008TCPModbus.Instance.ExitFlag = true;
        }
        private void DataGridView_TestFrm_CellValueNeeded_1(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex >= CaseConfig.caseList.Count || e.RowIndex < 0)
                return;
            ICaseInterface temp = CaseConfig.caseList[e.RowIndex];
            switch (e.ColumnIndex)
            {
                case 0:
                    e.Value = temp.GetCaseName();
                    break;
                case 1:
                    e.Value = temp.GetData();
                    break;
                case 2:
                    switch (temp.GetState())
                    {
                        case 测试结果.未开始:
                            e.Value = Unknown;
                            break;
                        case 测试结果.测试中:
                            e.Value = Test;
                            break;
                        case 测试结果.合格:
                            e.Value = Pass;
                            break;
                        case 测试结果.不合格:
                            e.Value = Fail;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
        void CANUDPClientsLoad(string var)
        {
            try
            {
                
                string json = File.ReadAllText( $"{var}CanNode.json");
                var loadedClients = JsonConvert.DeserializeObject<BindingList<CANUDPClient>>(json);

                CANUDPClients.Clear();
                foreach (var client in loadedClients)
                    CANUDPClients.Add(client);
                foreach (var client in CANUDPClients)//遍历Message
                {
                    client.周期发送List.Clear();
                    foreach (var mes in client.Messages)
                    {
                        if (mes.CycleTime != 0)
                            client.周期发送List.Add(mes);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载失败: {ex.Message}");
            }
        }

        ScanLog scanLog = new ScanLog();
        /// <summary>
        ///  通讯板初始化
        /// </summary>
        async Task CAEA008GroupInit()
        {
            Caea008Groups.Clear();
            string ip = "192.168.1.";//21-22 
            for (int i = 3; i < 10; i++)
            {
                ModbusTcpMaster modbusTcpMaster = new ModbusTcpMaster(ip + i.ToString(), 502);
                await modbusTcpMaster.ModbusTcp_Init(i);
                Caea008Groups.Add(ip + i.ToString(), modbusTcpMaster);
            }
        }

        public ModbusTcpMaster FindmodbusTcpMasterByName(string targetName)
        {
            return WinForm.Caea008Groups.FirstOrDefault(c => c.Key == targetName).Value;
        }
    
        void StartCanUdp()
        {
            try
            {
                foreach (var client in CANUDPClients)//遍历Message
                {
                    client.Init(client.ip, client.port, client.Listingport);
                    client.StopFlag = false;
                    ThreadPool.QueueUserWorkItem(client.RunningThread);
                    ThreadPool.QueueUserWorkItem(client.SendThread);
                    //   ThreadPool.QueueUserWorkItem(client.CheckChangesThread);
                    //client.CanInitSend(client.CAN初始化设置);
                    //Thread.Sleep(1000);
                    //client.CanIDInitSend(client.CAN初始化设置);
                }
            }
            catch (Exception ex)
            {

            }
        }
        void StopCanUdp()
        {
            foreach (var client in CANUDPClients)//
                client.StopFlag = true;
        }
 

        private delegate void delProgressBar(ToolStripProgressBar progBar, int val);
        private void progressBar1Update(ToolStripProgressBar proBar, int val)
        {
            if (this.InvokeRequired)
            {
                delProgressBar delProgress = new delProgressBar(progressBar1Update);
                this.Invoke(delProgress, new object[] { proBar, val });
            }
            else
            {
                proBar.Value = Convert.ToInt32(val);
            }
        }
        /// <summary>
        /// 刷新信息提示
        /// </summary>
        private void RefreshMessage()
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                while (ScanMessage.GetLogIfExists(out string s))
                {
                    if (s.Equals(AppGlobal.ClearScanString))
                    {
                        tb_Msg.ResetText();
                    }
                    else
                    {
                        // builder.Append(s + Environment.NewLine);
                        tb_Msg.Text = s + Environment.NewLine + tb_Msg.Text;
                    }
                    if (tb_Msg.Text.Length > 50000)
                        tb_Msg.ResetText();
                }
                //  tb_Msg.Text += builder.ToString();//gundong
                //   tb_Msg.AutoScroll = true;
            }
            catch { }
        }

       

        private void 测试项设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmParseTest frmSetting = new FrmParseTest();
            frmSetting.Show();
        }

        private void WinForm_Load(object sender, EventArgs e)
        {
            Load型号列表();
            LocalSettingLoad();
            开始ToolStripMenuItem.Enabled = true;
            停止ToolStripMenuItem.Enabled = false;
            InitTraceRetryMenu();
            InitPrintRetryMenu();
            var v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            var tm = System.IO.File.GetLastWriteTime(GetType().Assembly.Location);
            try
            {
                GpgNet.Initialise();
            }
            catch (Exception ex)
            {
                log.InsertLog($"请确认解密软件已经正确安装,才可以使用自动下载功能。");
            }
            //   Program.MsgBox($"最后修改时间：{tm:yyyy-MM-dd HH:mm}\r\n版本：{v}");
            toolStripStatusLabel1.Text = "最后修改时间：{" + tm + " 运行版本：" + v + " }";

            this.WindowState = FormWindowState.Maximized;
        }

        // 2026-05-20: 调试菜单增加“补传追溯数据”入口，人工触发离线缓存补传。
        void InitTraceRetryMenu()
        {
            if (手动补传追溯数据ToolStripMenuItem != null)
                return;

            手动补传追溯数据ToolStripMenuItem = new ToolStripMenuItem();
            手动补传追溯数据ToolStripMenuItem.Name = "手动补传追溯数据ToolStripMenuItem";
            手动补传追溯数据ToolStripMenuItem.Click += 手动补传追溯数据ToolStripMenuItem_Click;
            testToolStripMenuItem.DropDownItems.Add(手动补传追溯数据ToolStripMenuItem);
            UpdateTraceRetryMenuText();
        }

        void UpdateTraceRetryMenuText()
        {
            if (手动补传追溯数据ToolStripMenuItem == null)
                return;

            int pending = WebServer.GetPendingUploadCount();
            手动补传追溯数据ToolStripMenuItem.Text = $"补传追溯数据(待补传:{pending})";
        }

        private void 手动补传追溯数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string msg = WebServer.ManualRetryPendingUploads();
            UpdateTraceRetryMenuText();
            MessageBox.Show(msg, "追溯补传", MessageBoxButtons.OK, MessageBoxIcon.Information);
            log.InsertLog(msg);
        }

        // 2026-05-20: 调试菜单增加“补打标签”入口，人工触发离线缓存补打。
        void InitPrintRetryMenu()
        {
            if (手动补打标签ToolStripMenuItem != null)
                return;

            手动补打标签ToolStripMenuItem = new ToolStripMenuItem();
            手动补打标签ToolStripMenuItem.Name = "手动补打标签ToolStripMenuItem";
            手动补打标签ToolStripMenuItem.Click += 手动补打标签ToolStripMenuItem_Click;
            testToolStripMenuItem.DropDownItems.Add(手动补打标签ToolStripMenuItem);

            按关键字补打一条ToolStripMenuItem = new ToolStripMenuItem();
            按关键字补打一条ToolStripMenuItem.Name = "按关键字补打一条ToolStripMenuItem";
            按关键字补打一条ToolStripMenuItem.Text = "按关键字补打一条";
            按关键字补打一条ToolStripMenuItem.Click += 按关键字补打一条ToolStripMenuItem_Click;
            testToolStripMenuItem.DropDownItems.Add(按关键字补打一条ToolStripMenuItem);
            UpdatePrintRetryMenuText();
        }

        void UpdatePrintRetryMenuText()
        {
            if (手动补打标签ToolStripMenuItem == null)
                return;

            int pending = GetPendingPrintCount();
            手动补打标签ToolStripMenuItem.Text = $"补打标签(待补打:{pending})";
        }

        private void 手动补打标签ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenPendingPrintJobsWindow();
        }

        private void 按关键字补打一条ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string keyword = PromptSinglePrintKeyword();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                MessageBox.Show("已取消补打。", "标签补打", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string msg = ManualRetrySinglePendingPrintJob(keyword);
            UpdatePrintRetryMenuText();
            MessageBox.Show(msg, "标签补打", MessageBoxButtons.OK, MessageBoxIcon.Information);
            log.InsertLog(msg);
        }

        // 2026-05-20: 新增“待补打管理窗口”，方便员工离线补打查看与重试。
        void OpenPendingPrintJobsWindow()
        {
            if (pendingPrintJobsForm == null || pendingPrintJobsForm.IsDisposed)
            {
                pendingPrintJobsForm = new FrmPendingPrintJobs();
                pendingPrintJobsForm.FormClosed += (s, e) => { pendingPrintJobsForm = null; };
            }
            if (!pendingPrintJobsForm.Visible)
                pendingPrintJobsForm.Show(this);
            else
                pendingPrintJobsForm.Activate();

            pendingPrintJobsForm.BringToFront();
        }

        string PromptSinglePrintKeyword()
        {
            using (var form = new Form())
            {
                form.Text = "输入补打关键字";
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.ClientSize = new Size(460, 130);
                form.MinimizeBox = false;
                form.MaximizeBox = false;
                form.ShowInTaskbar = false;

                var lb = new Label
                {
                    AutoSize = true,
                    Left = 12,
                    Top = 14,
                    Text = "请输入条码/追溯码/序列号/ECUID关键字："
                };
                var tb = new System.Windows.Forms.TextBox
                {
                    Left = 12,
                    Top = 38,
                    Width = 430
                };
                var ok = new System.Windows.Forms.Button
                {
                    Text = "确定",
                    Left = 286,
                    Top = 76,
                    Width = 75,
                    DialogResult = DialogResult.OK
                };
                var cancel = new System.Windows.Forms.Button
                {
                    Text = "取消",
                    Left = 367,
                    Top = 76,
                    Width = 75,
                    DialogResult = DialogResult.Cancel
                };

                form.Controls.Add(lb);
                form.Controls.Add(tb);
                form.Controls.Add(ok);
                form.Controls.Add(cancel);
                form.AcceptButton = ok;
                form.CancelButton = cancel;

                var dr = form.ShowDialog(this);
                return dr == DialogResult.OK ? tb.Text?.Trim() : string.Empty;
            }
        }
        void  LocalSettingLoad()
        {
            ScanLog scanLog = new ScanLog();
            string path = System.Environment.CurrentDirectory;
            try
            {
                if (System.IO.File.Exists(path + "\\LocalSetting.json"))
                {
                    var json = System.IO.File.ReadAllText(path + "\\LocalSetting.json", Encoding.UTF8);
                    var b = Newtonsoft.Json.JsonConvert.DeserializeObject<LocalSetting>(json);
                    if (b != null)
                        LocalSetting.localSetting = b;
                }
            }
            catch (Exception ex)
            {
                scanLog.InsertLog(ex.Message);
                LogHelper.Warn(ex.Message);
            }
        }
        public static void SaveLocalSetting()
        {
            var jsonSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            try
            {
                var json = JsonConvert.SerializeObject(LocalSetting.localSetting, Formatting.None, jsonSetting);
                string path = System.Environment.CurrentDirectory;
                File.WriteAllText(path + "\\LocalSetting.json", json, Encoding.UTF8);
            }
            catch (Exception ex) { }
        }
        void ParseTestLoad(string var)
        {
            try
            {
                //string path = System.Windows.Forms.Application.StartupPath;
                //var json = File.ReadAllText(path + @"\\" + @"ParseTestItemsConfig.json", Encoding.UTF8);
                //var b = Newtonsoft.Json.JsonConvert.DeserializeObject<BindingList<ParseTestItems>>(json);
                //if (b != null)
                //{
                //    ParseTestItemsSetting = b;
                //}
                ParseTestItemsSetting = CsvHelper.ImportFromCsv(var + ".csv");
                CaseConfig.测试项排序();
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// 将D元件放到读列表
        /// </summary>
        void ADD_D_Read_List()
        {
            CAEA008TCPModbus.Instance.KeyList.Clear();
            int k;
            foreach (var item in ParseTestItemsSetting)
            {
                string[] strings = item.被测数据.ToString().Split(',');
                if (strings.Length > 5)
                    k = 0;
                foreach (var st in strings)
                {
                    if (st.Contains("D") && !st.Contains("ADC") && !st.Contains("MCU"))
                    {
                        Match match = Regex.Match(st, @"\d+");
                        if (match.Success)
                        {
                            CAEA008TCPModbus.Instance.KeyList.Add(Convert.ToUInt16(match.Value));
                            CAEA008TCPModbus.Instance.DList.AddOrUpdate(Convert.ToUInt16(match.Value), 0, (existingKey, existingValue) => 0);
                        }
                    }
                }
            }
            


            CAEA008TCPModbus.Instance.KeySortList = CAEA008TCPModbus.Instance.SortList(CAEA008TCPModbus.Instance.KeyList);
        }
        /// <summary>
        /// 将配置的循环发送，开启负载 关闭负载，加入队列
        /// </summary>
        void AddWriteList()
        {
            CAEA008TCPModbus.Instance.循环发送队列.Clear();
            CAEA008TCPModbus.Instance.常开发送队列.Clear();
            CAEA008TCPModbus.Instance.长时1负载关闭.Clear();
            CAEA008TCPModbus.Instance.长时1负载开启.Clear();
            CAEA008TCPModbus.Instance.长时2负载关闭.Clear();
            CAEA008TCPModbus.Instance.长时2负载开启.Clear();
            foreach (var item in ParseTestItemsSetting)
            {
                //if (!String.IsNullOrEmpty(item.循环发送报文开关))
                //{
                //    var 循环发送报文开关s = ParseWrite(item.循环发送报文开关);
                //    foreach (var st in 循环发送报文开关s)
                //    {
                //        CAEA008TCPModbus.Instance.循环发送队列.AddOrUpdate(st.Item1, st.Item2, (existingKey, existingValue) => st.Item2);
                //    }
                //}
                //if (item.测试时序 == 时序.常开 && !String.IsNullOrEmpty(item.负载开启报文开关))
                //{
                //    var 负载开启报文开关s = ParseWrite(item.负载开启报文开关);
                //    foreach (var st in 负载开启报文开关s)
                //    {
                //        CAEA008TCPModbus.Instance.常开发送队列.AddOrUpdate(st.Item1, st.Item2, (existingKey, existingValue) => st.Item2);
                //    }
                //}

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

     
    

        private void 本地设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmLocalSetting frmLocalSetting = new FrmLocalSetting();
            frmLocalSetting.ShowDialog();
        }
       // public static Board ioTest = new Board();
        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ParmsInputDialogue pm = new ParmsInputDialogue("io测试", ioTest);
            //pm.ShowDialog();
            //    method.Com_SerialPort = 
            //  method.aT03Rs485.Com = LocalSetting.localSetting._485port;

        }
     
    //    public Harnessdefinition harnessdefinition = new Harnessdefinition();
        ushort tzstate = 0;
        private void tESTToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // harnessdefinition.AT03Def.AT03Definition = 
            //AT03Rs485Communicate.Instance.Com = LocalSetting.localSetting._485port;
            //AT03Rs485Communicate.Instance.Open();
            // string st =    AT03Rs485Communicate.Instance.Read_Software(55);
            // float temp =Convert.ToSingle( (0x3F66 << 16) + 26214);
            //byte[] vs = new byte[4] { 0x00, 0x00, 0x73, 0x3F };
            //float temp = BitConverter.ToSingle(vs, 0);
            //Console.WriteLine(temp.ToString("F6"));
            //AT03TCPCommunicate.IP = LocalSetting.localSetting.MyIp;// "192.168.1.218";
            //ThreadPool.QueueUserWorkItem(AT03TCPCommunicate.Instance.GetFuncPorts,new object());
            //        Task<TestResult> task = Task.Run(() => AT03TCPCommunicate.Instance.GetFuncPorts(new object()));

            // 等待任务完成并获取返回值
            //try
            //{
            //    TestResult result = task.Result; // 这里可能会抛出异常，因为它是阻塞的
            ////    Console.WriteLine("Result: " + result);
            //}
            //catch (AggregateException ex)
            //{
            //    // 处理任务执行中的异常
            //    Console.WriteLine("An error occurred: " + ex.InnerException.Message);
            //}
            //    AT03Rs485Communicate.Instance.Open();
            //       method.ScanSerial();
            //foreach (var temp in CaseConfig.caseList)
            //    temp.Init();

            //  CsvHelper.ExportToCsv(ParseTestItemsSetting, "Temp.csv");
            //// 加载DBC
            ////var parser = new DbcParser();
            ////parser.Parse(File.ReadAllText("CAN01_CAN02.dbc"));
            ////    var parser = new DbcParser();
            ////     var db = parser.Parse(File.ReadAllText("CAN01_CAN02.dbc"));
            ////FrmCanNode frmCanNode = new FrmCanNode();
            ////frmCanNode.ShowDialog();
            //CsvHelper.parseTestItems =  CsvHelper.ImportFromCsv("Temp.csv");
            //FrmParseTest frmSetting = new FrmParseTest();
            //frmSetting.Show();
            //  StartCanUdp();
            //   CreatCurrentCollentModbus();
            //   CreatCurrentCollectionTcp();
            //     ShiYanModbusTCP.Instance.ModbusTcp_Init(LocalSetting.localSetting.SeverIP, LocalSetting.localSetting.Severport);
            //   RegenerativeCommunicate.Instance.Connect(LocalSetting.localSetting.PowerCom, LocalSetting.localSetting.PowerbaudRate);
            //CAEA008TCPModbus.Instance._寄存器管理器.插入寄存器(20, 1628);
            //  tzstate = 0;
            //if (手动测试负载选择 == 0)
            //    CAEA008TCPModbus.Instance._寄存器管理器.插入寄存器(32, 0);
            //else
            //    CAEA008TCPModbus.Instance._寄存器管理器.插入寄存器(32, 1);
            // CAEA008TCPModbus.Instance._寄存器管理器.插入寄存器(30, 1);

            //CAEA008TCPModbus.Instance._线圈管理器.插入线圈(303, 0);
            //CAEA008TCPModbus.Instance._线圈管理器.插入线圈(301, 0);
            //CAEA008TCPModbus.Instance._寄存器管理器.插入寄存器(36, 1);
            //foreach (var temp in Caea008Groups)
            //{
            //    temp.Value.ClearLinD();
            //}
            //    AT03TCPCommunicate.Instance.PinWriteHL("HL-107-0");
            //   _ = CAEA008GroupInit();
            //Communication.CAN.Message message = new Communication.CAN.Message()
            //{
            //    Id = "0x14DA40F1",
            //    Name = "2F C0 50 03",
            //    WriteOrRead = WriteOrRead.Write,
            //    FrameType = FrameType.ExtendedFrame,
            //    SendType = SendType.Diagnosis,
            //    Transmitter = "2F C0 50 03 10 10",
            //};
            //FindClientByName("MCU1").EnSendQueue(message);
            //CAEA008TCPModbus.Instance._线圈管理器.插入线圈(303,0);
            //   CAEA008TCPModbus.Instance._线圈管理器.插入线圈(303, 1);
            //  AT03TCPCommunicate.Instance.PinWriteHL("HL-108-0-0");
            //    WinForm.Save(2);
            var bytes = new byte[] { 0x00, 0x00, 0x7F, 0x07, 0x80, 0x00, 0x24, 0x00, 0x04, 0x19, 0x49, 0x94, 0x21, 0x01, 0x14, 0xAC, 0x96, 0x9C, 0xDC, 0xE1, 0xFC, 0xA1, 0x18, 0x5B, 0x91, 0x3E, 0xDB, 0x03, 0x0E, 0x05, 0xF5,0x55 };
            uint seedLeeth = 32;
            string error = string.Empty;
            var result = SeedKeyCaculate.GMAndPatac32ByteCaculate("",0x68,"01",false, ref seedLeeth, bytes,ref error);

        }
        public static BarTenderPrint barTenderPrint = new BarTenderPrint();
        public static void PrintLoadAndPrint()
        {
            ScanLog scanLog = new ScanLog();
            WinForm.barTenderPrint.LoadBtw(null);
            WinForm.barTenderPrint.SerialNum = LocalSetting.localSetting.PrintSerialNum;
            if (WinForm.barTenderPrint.检测号 != null)
                WinForm.barTenderPrint.检测号 = LocalSetting.localSetting.检测号;
            if (WinForm.barTenderPrint.班组号 != null)
                WinForm.barTenderPrint.班组号 = LocalSetting.localSetting.班组号;
            var sn = WinForm.barTenderPrint.GetBarCode();
            WinForm.TryPrintLabelWithOfflineCache("手动打印");
            scanLog.InsertLog("保存的二维码为:" + sn);
            LogHelper.Info("保存的二维码为:" + sn);
        }
        public static string 追溯码;
        public static string 零件号;
        public static string ECUID;

        static List<PendingPrintJob> LoadPendingPrintJobsNoThrow()
        {
            try
            {
                if (!File.Exists(PendingPrintFilePath))
                    return new List<PendingPrintJob>();

                string json = File.ReadAllText(PendingPrintFilePath, Encoding.UTF8);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<PendingPrintJob>();

                return JsonConvert.DeserializeObject<List<PendingPrintJob>>(json) ?? new List<PendingPrintJob>();
            }
            catch
            {
                return new List<PendingPrintJob>();
            }
        }

        static void SavePendingPrintJobsNoThrow(List<PendingPrintJob> list)
        {
            try
            {
                string json = JsonConvert.SerializeObject(list ?? new List<PendingPrintJob>(), Formatting.Indented);
                File.WriteAllText(PendingPrintFilePath, json, Encoding.UTF8);
            }
            catch
            {
            }
        }

        static PendingPrintJob BuildCurrentPendingPrintJob(string trigger)
        {
            var job = new PendingPrintJob
            {
                Trigger = trigger ?? "",
                ProductModel = 产品型号 ?? "",
                TraceCode = 追溯码 ?? "",
                CheckNo = LocalSetting.localSetting?.检测号 ?? "",
                TeamNo = LocalSetting.localSetting?.班组号 ?? "",
                Ecuid = ecuidInfoModels != null && ecuidInfoModels.Count > 0 && ecuidInfoModels[0] != null ? ecuidInfoModels[0].ECUID : "",
            };
            try { job.BarCode = barTenderPrint.GetBarCode() ?? ""; } catch { }
            try { job.SerialNum = barTenderPrint.SerialNum ?? ""; } catch { }
            return job;
        }

        static bool TryPrintJobNow(PendingPrintJob job, out string error)
        {
            error = string.Empty;
            if (job == null)
            {
                error = "打印任务为空。";
                return false;
            }

            try
            {
                barTenderPrint.LoadBtw(string.IsNullOrWhiteSpace(job.ProductModel) ? null : job.ProductModel);
                if (!string.IsNullOrWhiteSpace(job.SerialNum))
                    barTenderPrint.SerialNum = job.SerialNum;
                if (!string.IsNullOrWhiteSpace(job.CheckNo))
                    barTenderPrint.检测号 = job.CheckNo;
                if (!string.IsNullOrWhiteSpace(job.TeamNo))
                    barTenderPrint.班组号 = job.TeamNo;
                if (!string.IsNullOrWhiteSpace(job.TraceCode))
                {
                    追溯码 = job.TraceCode;
                    barTenderPrint.SetSubString("追溯码", job.TraceCode);
                }

                barTenderPrint.Print(null);
                if (barTenderPrint.LastPrintSuccess)
                    return true;

                error = "打印机未连接或打印失败（已记录待补打）。";
                return false;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        static void EnqueuePendingPrintNoThrow(PendingPrintJob job, string error)
        {
            if (job == null)
                return;

            job.LastError = error ?? "";
            lock (pendingPrintLock)
            {
                var list = LoadPendingPrintJobsNoThrow();
                list.Add(job);
                SavePendingPrintJobsNoThrow(list);
            }
        }

        public static int GetPendingPrintCount()
        {
            lock (pendingPrintLock)
            {
                return LoadPendingPrintJobsNoThrow().Count;
            }
        }

        public static bool TryPrintLabelWithOfflineCache(string trigger)
        {
            ScanLog scanLog = new ScanLog();
            var job = BuildCurrentPendingPrintJob(trigger);
            if (TryPrintJobNow(job, out string error))
            {
                scanLog.InsertLog("标签打印成功。");
                return true;
            }

            EnqueuePendingPrintNoThrow(job, error);
            scanLog.InsertLog($"标签打印失败，已缓存待补打。原因: {error}，当前待补打数量: {GetPendingPrintCount()}");
            return false;
        }

        public static string ManualRetryPendingPrintJobs()
        {
            List<PendingPrintJob> snapshot;
            lock (pendingPrintLock)
            {
                snapshot = LoadPendingPrintJobsNoThrow();
                SavePendingPrintJobsNoThrow(new List<PendingPrintJob>());
            }

            if (snapshot.Count == 0)
                return "没有待补打标签。";

            int success = 0;
            int failed = 0;
            var remain = new List<PendingPrintJob>();
            foreach (var job in snapshot)
            {
                job.LastTryTime = DateTime.Now;
                if (TryPrintJobNow(job, out string error))
                {
                    success++;
                }
                else
                {
                    failed++;
                    job.RetryCount++;
                    job.LastError = error ?? "";
                    remain.Add(job);
                }
            }

            if (remain.Count > 0)
            {
                lock (pendingPrintLock)
                {
                    var latest = LoadPendingPrintJobsNoThrow();
                    latest.AddRange(remain);
                    SavePendingPrintJobsNoThrow(latest);
                }
            }

            return $"补打完成：成功 {success} 条，失败 {failed} 条，剩余待补打 {GetPendingPrintCount()} 条。";
        }

        static bool ContainsIgnoreCase(string source, string keyword)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(keyword))
                return false;
            return source.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public class PendingPrintJobView
        {
            public string Id { get; set; }
            public string 创建时间 { get; set; }
            public string 来源 { get; set; }
            public string 产品型号 { get; set; }
            public string 条码 { get; set; }
            public string 追溯码 { get; set; }
            public string 序列号 { get; set; }
            public string ECUID { get; set; }
            public int 重试次数 { get; set; }
            public string 最后错误 { get; set; }
            public string 最后尝试时间 { get; set; }
        }

        public static List<PendingPrintJobView> GetPendingPrintJobsSnapshot()
        {
            lock (pendingPrintLock)
            {
                return LoadPendingPrintJobsNoThrow()
                    .OrderByDescending(x => x.CreateTime)
                    .Select(x => new PendingPrintJobView
                    {
                        Id = x.Id,
                        创建时间 = x.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        来源 = x.Trigger ?? "",
                        产品型号 = x.ProductModel ?? "",
                        条码 = x.BarCode ?? "",
                        追溯码 = x.TraceCode ?? "",
                        序列号 = x.SerialNum ?? "",
                        ECUID = x.Ecuid ?? "",
                        重试次数 = x.RetryCount,
                        最后错误 = x.LastError ?? "",
                        最后尝试时间 = x.LastTryTime == DateTime.MinValue ? "" : x.LastTryTime.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                    .ToList();
            }
        }

        public static string ManualRetryPendingPrintJobById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return "任务ID为空。";

            PendingPrintJob target = null;
            lock (pendingPrintLock)
            {
                var list = LoadPendingPrintJobsNoThrow();
                int idx = list.FindIndex(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));
                if (idx < 0)
                    return $"未找到待补打任务: {id}";

                target = list[idx];
                list.RemoveAt(idx);
                SavePendingPrintJobsNoThrow(list);
            }

            target.LastTryTime = DateTime.Now;
            if (TryPrintJobNow(target, out string error))
                return $"补打成功：{id}，剩余待补打 {GetPendingPrintCount()} 条。";

            target.RetryCount++;
            target.LastError = error ?? "";
            lock (pendingPrintLock)
            {
                var latest = LoadPendingPrintJobsNoThrow();
                latest.Add(target);
                SavePendingPrintJobsNoThrow(latest);
            }
            return $"补打失败：{id}，原因：{error}，剩余待补打 {GetPendingPrintCount()} 条。";
        }

        public static string ManualRetrySinglePendingPrintJob(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return "关键字为空，未执行补打。";

            PendingPrintJob target = null;
            lock (pendingPrintLock)
            {
                var list = LoadPendingPrintJobsNoThrow();
                int idx = list.FindIndex(j =>
                    ContainsIgnoreCase(j.BarCode, keyword) ||
                    ContainsIgnoreCase(j.TraceCode, keyword) ||
                    ContainsIgnoreCase(j.SerialNum, keyword) ||
                    ContainsIgnoreCase(j.Ecuid, keyword));

                if (idx < 0)
                    return $"未找到匹配关键字[{keyword}]的待补打任务。";

                target = list[idx];
                list.RemoveAt(idx);
                SavePendingPrintJobsNoThrow(list);
            }

            target.LastTryTime = DateTime.Now;
            if (TryPrintJobNow(target, out string error))
            {
                return $"补打一条成功：关键字[{keyword}]，剩余待补打 {GetPendingPrintCount()} 条。";
            }

            target.RetryCount++;
            target.LastError = error ?? "";
            lock (pendingPrintLock)
            {
                var latest = LoadPendingPrintJobsNoThrow();
                latest.Add(target);
                SavePendingPrintJobsNoThrow(latest);
            }
            return $"补打一条失败：关键字[{keyword}]，原因：{error}，剩余待补打 {GetPendingPrintCount()} 条。";
        }

        public static void Save(int result)
        {
            ScanLog scanLog = new ScanLog();
            foreach (var client in CANUDPClients)//遍历Message
            {
                client.周期发送List.Clear();
            }
            追溯结果 = 1;
            barTenderPrint.SetZSM();
            var st = 关联追溯.整理需要上传的内容();
            if (!string.IsNullOrEmpty(零件号) && !string.IsNullOrEmpty(追溯码))
            {
                string re = WebServer.SaveTestResult(new string[5] { WinForm.barTenderPrint.GetBarCode(), st, WinForm.ecuidInfoModels[0].ECUID, 零件号, result.ToString() });
                scanLog.InsertLog($"追溯上传结果: {re}");
            }
            else
            {
                scanLog.InsertLog("追溯上传跳过: 零件号或追溯码为空。");
            }
        }
        public static int SelectGW(string MTC, string ServerModul)
        {
            int result = 0;
            DBConn();
            try
            {
                using (SqlConnection sqlcnt = new SqlConnection(WinForm.sqlstr))
                {
                    using (SqlCommand cmand = sqlcnt.CreateCommand())
                    {
                        cmand.CommandText = " SELECT *  FROM 产品信息" +
                                                " where 有效标识 = 0 AND MTC = '" + MTC + "' and 用户图号='" + ServerModul + "'";
                        sqlcnt.Open();
                        SqlDataReader r = cmand.ExecuteReader();
                        if (r.Read())
                        {
                            string gw = "";
                            if (产品型号 == "0Y4G40AAEWWA" || 产品型号 == "0Y4G40DAEWWA" || 产品型号 == "0Y4Z40AAEWWA" /*|| 产品型号 == "0Y4Z40DAEWWA"*/)
                            {
                                gw = "145";
                            }
                            else
                                gw = LocalSetting.localSetting.FrontGw;

                            if (string.IsNullOrEmpty(r[gw + "标识"].ToString()))
                            {
                                result = 3;
                            }
                            else
                            {
                                result = Convert.ToInt16(r[gw + "标识"].ToString());
                            }
                        }
                        else
                        {
                            MessageBox.Show("产品信息无此数据！");
                            result = 1;
                            return result;
                        }
                        if (result == 2)
                        {

                        }
                        else
                        {
                            //         label46.Text = "前工位未合格！";  
                        }
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                return result;
            }
        }
   
        public CANUDPClient FindClientByName(string targetName)
        {
            return WinForm.CANUDPClients.FirstOrDefault(c => c.Name == targetName);
        }
  
      
        float GetStaticCurrent()
        {
            byte by1, by2, by3, by4;
            if (CAEA008TCPModbus.Instance.DList.ContainsKey(600) && CAEA008TCPModbus.Instance.DList.ContainsKey(601))
            {
                by4 = (byte)(CAEA008TCPModbus.Instance.DList[601] >> 8);
                by3 = (byte)(CAEA008TCPModbus.Instance.DList[601] & 0xFF);
                by2 = (byte)(CAEA008TCPModbus.Instance.DList[600] >> 8);
                by1 = (byte)(CAEA008TCPModbus.Instance.DList[600] & 0xFF);
                byte[] vs = new byte[4] { by1, by2, by3, by4 };
                float temp = BitConverter.ToSingle(vs, 0);
                return temp;
            }
            return 0;
        }
        public static System.Drawing.Image Red = Properties.Resources.red;
        public static System.Drawing.Image Green = Properties.Resources.green;
        public int 手动测试负载选择 = 1;

        public static bool 循环测试标志 = false;
        public static 测试结果 本轮测试结果 = 测试结果.未开始;
        TimeSpan timeSpan;
        int minutes = 0;
        int seconds = 0;
        public static DateTime 上轮结束时间;
        public static bool 上轮结束标志 = false;
        public static byte VbatFlag = 0;//1=9V 2=14V 3=16V
        public static double WorkCurrent = 0;
        public static double StaticCurrent = 0;
        public static string 产品型号;
        public static string SeverName;
        public static int 追溯结果;
        public static int _int扫码结果;
        public static string _ScanString = "";
        private void Refalshtimer_Tick(object sender, EventArgs e)
        {
            Refalshtimer.Stop();
          //  ScanTextBox.Text =  _ScanString;
            Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
       
            if ((本轮测试结果 == 测试结果.测试中)&& DataGridView_TestFrm.CurrentCell != DataGridView_TestFrm.Rows[TaskMainWorker.TestMax].Cells[0])
                DataGridView_TestFrm.CurrentCell = DataGridView_TestFrm.Rows[TaskMainWorker.TestMax].Cells[0];
            //   DataGridView_TestFrm.Refresh();
            //  DataGridView_TestFrm.sel
            产品型号 = 型号列表.Text;
            if (CAEA008TCPModbus.Instance.CAEA008TX == true)
                Communication_state.BackgroundImage = Green;
            else
                Communication_state.BackgroundImage = Red;
            if (本轮测试结果 == 测试结果.合格)
                TestResult_state.BackgroundImage = Pass;
            if (本轮测试结果 == 测试结果.不合格)
                TestResult_state.BackgroundImage = Fail;
            if (本轮测试结果 == 测试结果.测试中)
                TestResult_state.BackgroundImage = Test;
            if (本轮测试结果 == 测试结果.未开始)
                TestResult_state.BackgroundImage = Unknown;
            if (追溯结果 == 2)
                panel_SaveResult.BackgroundImage = Pass;
            if (追溯结果 == 3)
                panel_SaveResult.BackgroundImage = Fail;
            if (追溯结果 == 1)
                panel_SaveResult.BackgroundImage = Test;
            if (追溯结果 == 0)
                panel_SaveResult.BackgroundImage = Unknown;
            //if (_int扫码结果 == 2)
            //    扫码结果.BackgroundImage = Pass;
            //if (_int扫码结果 == 3)
            //    扫码结果.BackgroundImage = Fail;
            //if (_int扫码结果 == 1)
            //    扫码结果.BackgroundImage = Test;
            //if (_int扫码结果 == 0)
            //    扫码结果.BackgroundImage = Unknown;
            RefreshMessage();
            //TaskMainWorker.TestStep
            //   if (本轮测试结果 == 测试结果.测试中 && DataGridView_TestFrm.CurrentCell != DataGridView_TestFrm.Rows[mainFrom.测试步骤].Cells[0])
            //       DataGridView_TestFrm.CurrentCell = DataGridView_TestFrm.Rows[mainFrom.测试步骤].Cells[0];
            DataGridView_TestFrm.Refresh();
         
            if (Time显示Flag == true)
            {
                Lb_DvTime.Text = TaskMainWorker.DvDateTime.ToString("0.00") + "秒";
                skinLabel1.Visible = true;
            }
            else
            {
                skinLabel1.Visible = false;
                Lb_DvTime.Text = "";
            }

            if (!String.IsNullOrEmpty(LocalSetting.localSetting.VBAT_Signal) && !String.IsNullOrEmpty(LocalSetting.localSetting.VBAT_UdpName) &&
              FindClientByName(LocalSetting.localSetting.VBAT_UdpName).SignalDictionary.ContainsKey(LocalSetting.localSetting.VBAT_Signal))
            {
                double Vbat = FindClientByName(LocalSetting.localSetting.VBAT_UdpName).SignalDictionary[LocalSetting.localSetting.VBAT_Signal];
                lb_vbat.Text = Vbat.ToString();
            }

           // StaticCurrent = GetStaticCurrent();
            //   sklb_TestItem.Text = LocalSetting.localSetting.产品型号;

            if (TaskMainWorker.Instance.DVTestEndFlag == true)
            {
                TaskMainWorker.Instance.DVTestEndFlag = false;
                上轮结束时间 = DateTime.Now;
                上轮结束标志 = true;
            }
            测试次数.Text = LocalSetting.localSetting.TestNumb.ToString();
            UpdateTraceRetryMenuText();
            UpdatePrintRetryMenuText();
            //if (上轮结束标志 == true && (DateTime.Now - 上轮结束时间).TotalMilliseconds > 10000)
            //{
            //    上轮结束标志 = false;
            //    ThreadPool.QueueUserWorkItem(StartTest);
            //}
            
            stopwatch.Stop();
            Refalshtimer.Start();
        }

        private void DataGridView_TestFrm_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        }
        private void DataGridView_TestFrm_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // 检查是否为左键双击
            OnceTaskFlag = false;
            Thread.Sleep(1000);
            //if (e.Button == MouseButtons.Left && e.Clicks == 2)
            //{
            //    // 左键双击事件处理逻辑
            //    // MessageBox.Show("左键双击了单元格: " + e.RowIndex + "," + e.ColumnIndex);
            try
            {
                //   ThreadPool.QueueUserWorkItem(new WaitCallback(OneTest), 负载状态.开启, DataGridView_TestFrm.CurrentRow.Index);
                bool flag = ThreadPool.QueueUserWorkItem(delegate
                   {
                       OneTest((负载状态)手动测试负载选择, DataGridView_TestFrm.CurrentRow.Index);
                   });
                int k = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //}
            //// 检查是否为右键双击
            //else if (e.Button == MouseButtons.Right && e.Clicks == 2)
            //{
            //    // 右键双击事件处理逻辑
            //    // MessageBox.Show("右键双击了单元格: " + e.RowIndex + "," + e.ColumnIndex);
            //    try
            //    {
            //        ThreadPool.QueueUserWorkItem(new WaitCallback(OneTest), 负载状态.关闭);
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }
            //}
        }
        public bool OnceTaskFlag = false;
        private void OneTest(object OpenorClose, int index)
        {
            OnceTaskFlag = true;
            CaseConfig.caseList[index].DVResultInit();
            CaseConfig.caseList[index].DVDateInit();
            CaseConfig.caseList[index].Init();
            Thread.Sleep(500);
            while (OnceTaskFlag)
            {
                Thread.Sleep(100);
                CaseConfig.caseList[index].Start((负载状态)OpenorClose, 测试方式.手动);
            }
        }

        private void btn_OnceTestClose_Click(object sender, EventArgs e)
        {
            OnceTaskFlag = false;
        }
        public bool DV_btn = false;
        public bool Temp_btn = false;
        public static int  HLIDH01 = 117;//104
        private void btn_DVStart_Click(object sender, EventArgs e)
        {
            //if (ScanFlag != true)
            //{
            //    MessageBox.Show("未扫码无法开始测试！");
            //    return;
            //}
            ScanFlag = false;
            TaskMainWorker.Instance.测试轮数 = 0;
           // AppGlobal.天线学习标志 = false;
            //         WinForm.InitStaticExitFlag = false;
            //CAEA008TCPModbus.Instance._线圈管理器.插入线圈(303, 0);
            //CAEA008TCPModbus.Instance._线圈管理器.插入线圈(301, 0);

            SendBagSum = 0;
            RevcBagSum = 0;
            WinForm.barTenderPrint.LoadBtw(null);

            TaskMainWorker.TestStep = 1;
            ThreadPool.QueueUserWorkItem(StartTest);
        }
        public static int _IsActie = 0;// 1 run   0stop
        void StartTest(object o)
        { 
            if(_IsActie == 1)
            {
                return;
            }
            //CAEA008TCPModbus.Instance._寄存器管理器.插入寄存器(30, 0);
            //CAEA008TCPModbus.Instance._寄存器管理器.插入寄存器(31, 0);
            //CAEA008TCPModbus.Instance._寄存器管理器.清空寄存器(1, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,0,0,0,0,0 });
           
            //var 循环发送报文开关s = ParseWriteD("(192.168.1.3&3-0)(192.168.1.3&4-0)(192.168.1.4&5-0)(192.168.1.4&6-0)(192.168.1.5&7-0)(192.168.1.5&8-0)(192.168.1.6&9-0)(192.168.1.6&10-0)(192.168.1.7&11-0)(192.168.1.7&12-0)(192.168.1.8&13-0)(192.168.1.8&14-0)(192.168.1.9&15-0)(192.168.1.9&16-0)");
            //foreach (var st in 循环发送报文开关s)
            //{
            //    FindmodbusTcpMasterByName(st.IP).WriteQueue.Enqueue(st.Address.ToString() + "-" + st.Value.ToString());
            //}
            foreach(var temp in Caea008Groups)
            {
                temp.Value.ClearLinD();
            }
            _IsActie = 1;
            CsVLog.Clear();
            WinForm.追溯结果 = 0;
            List<string> Namelist = new List<string>();
            Namelist.Add("时间戳");
            foreach (var temp in CaseConfig.caseList)
            {
                Namelist.Add(temp.GetCaseName());
                temp.Init();
                temp.DVResultInit();
                temp.Reset测试时序状态1();//复位时序状态
                temp.Reset测试时序状态2();
                temp.Reset测试时序状态3();
                temp.Reset测试时序状态4();
                temp.Reset测试时序状态5();
                temp.Reset测试时序状态6();
                temp.Reset测试时序状态7();
                temp.Reset测试时序状态8();
                temp.Reset测试时序状态9();
                temp.Reset测试时序状态10();
                temp.Reset测试时序状态11();
                temp.Reset测试时序状态12();
                //       temp.DVDateInit();
            }
            //Namelist.Add("工作电流");
            //Namelist.Add("静态电流");
            string[] Namearray = Namelist.ToArray();
            CsVLog.Add(Namearray);
           // AddWriteList();

            InitTransmitterDictionary();
            Thread.Sleep(2000);

            // 2026-05-11: 以当前Case列表中的最小测试顺序作为起始步号，避免步号不一致导致前置项被跳过。
            if (CaseConfig.caseList.Count > 0)
            {
                int minStep = CaseConfig.caseList.Min(c => c.获取测试顺序());
                TaskMainWorker.TestStep = (byte)Math.Max(0, minStep);
            }
            else
            {
                TaskMainWorker.TestStep = 1;
            }

            // 2026-05-11: 输出一次调度快照，便于定位“界面顺序正常但执行未命中”的问题。
            try
            {
                //var stepMap = string.Join(" | ", CaseConfig.caseList
                //    .Select((c, i) => $"{i + 1}:{c.GetCaseName()}(Step={c.获取测试顺序()})"));
                //log.InsertLog($"[TaskInit] StartStep={TaskMainWorker.TestStep}, CaseCount={CaseConfig.caseList.Count}");
                //log.InsertLog($"[TaskInit] {stepMap}");
            }
            catch
            {
                // 忽略日志异常，不影响测试主流程
            }

            TaskMainWorker.Instance.DVTestStartFlag = true;
            TaskMainWorker.TestMax = 0;
            ThreadPool.QueueUserWorkItem(TaskMainWorker.Instance.StartSingleTask);
       //     ThreadPool.QueueUserWorkItem(SaveDataLogThread);
            WinForm.本轮测试结果 = 测试结果.测试中;
        }
        public void SaveDataLogThread(object o)
        {
            ScanLog log = new ScanLog();
            while (TaskMainWorker.Instance.DVTestStartFlag)
            {
                List<string> Testlist = new List<string>();
                Testlist.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                foreach (var temp in CaseConfig.caseList)
                {
                    Testlist.Add(temp.GetData() + " " + temp.GetState());
                }
                //Testlist.Add(WorkCurrent.ToString("0.00") + "mA");
                //Testlist.Add(StaticCurrent.ToString("0.00") + "mA");
                string[] TestDataarray = Testlist.ToArray();
                CsVLog.Add(TestDataarray);
                Thread.Sleep(1200);
            }

        }
        public static string savepath = "";
        void Dvstop()
        {
            WinForm._IsActie = 0;
            TaskMainWorker.Instance.DVTestStartFlag = false;
            TaskMainWorker.Instance.TempTestStartFlag = false;
            WinForm._IsActie = 0;
            string filePath = System.IO.Path.Combine(LocalSetting.localSetting.FileNamePath, DateTime.Now.ToString("yyMMddHHmmss") + ".csv");
            savepath = filePath;
            LogTestDatacsv.WriteData(filePath, WinForm.CsVLog);
        }
        private void btn_DvStop_Click(object sender, EventArgs e)
        {
            Dvstop();
        }
        private void DataGridView_TestFrm_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            e.Row.HeaderCell.Value = string.Format("{0}", e.Row.Index + 1);
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

       
        private void 配置D读列表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ADD_D_Read_List();
        }


        private void d元件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDRead frmDRead = new FrmDRead(CAEA008TCPModbus.Instance.DList);
            //foreach (var temp in CAEA008TCPModbus.Instance.DList)
            //{
            //    frmDRead.UpdateBindingList(temp.Key, temp.Value);
            //}
            frmDRead.Show();
        }

        private void 主控板ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void btn_init_Click(object sender, EventArgs e)
        {
            InitExitFlag = false;
            InitStep = 0;
            ThreadPool.QueueUserWorkItem(InitThread);

        }
        bool TCpInitExitFlag = false;
        bool InitExitFlag = false;
        byte InitStep = 0;
        bool InitFlag = false;
        private void InitThread(object o)
        {
            while (!InitExitFlag)
            {
                switch (InitStep)
                {
                    case 0://添加D读列表
                        CAEA008TCPModbus.Instance.ExitFlag = true;
                        ADD_D_Read_List();
                        InitStep++;
                        break;
                    case 1:
                        CAEA008TCPModbus.Instance.CAEA008TCPipSet(LocalSetting.localSetting.CAEA008IP, LocalSetting.localSetting.port);
                        if (CAEA008TCPModbus.Instance.ModbusTcp_Init() == "True")
                            InitStep++;
                        else
                        {
                            //InitExitFlag = true;
                            InitStep = 20;
                            log.InsertLog("初始化失败!,下位机通讯异常 尝试重连");
                            Thread.Sleep(2000);
                        }
                        break;
                    case 20:
                        CAEA008TCPModbus.Instance.CAEA008TCPipSet(LocalSetting.localSetting.CAEA008IP, LocalSetting.localSetting.port);
                        if (CAEA008TCPModbus.Instance.ModbusTcp_Init() == "True")
                            InitStep = 2;
                        else
                        {
                            InitExitFlag = true;
                            InitStep = 99;
                            log.InsertLog("重连失败！ 初始化失败!,下位机通讯异常");
                        }
                        break;
                    case 2:
                     //   AddWriteList();//加入循环列表
                        CAEA008TCPModbus.Instance.ExitFlag = false;
                    ThreadPool.QueueUserWorkItem(CAEA008TCPModbus.Instance.ModbusTcpThread);
                        //CAEA008TCPModbus.Instance.timer.
                        InitStep++;
                        break;
                    case 3:
                        foreach (var temp in CaseConfig.caseList)
                        {
                            temp.Init();
                            temp.CapInit();
                        }
                        InitStep++;
                        break;
                    case 4:
                        StartCanUdp();
                        log.InsertLog("CAN UDP初始化完成!");
                        InitStep++;
                        break;
                  
                    default:
                        InitStep = 0;
                        InitFlag = true;
                        log.InsertLog("初始化完成!");
                        InitExitFlag = true;
                        break;
                }
            }
        }
        public static bool ReConnectFlag = false;
        public static bool Staticing = false;
        public DateTime ReConnectTime = DateTime.Now;
   
        public byte StepTime(DateTime time, int delaytime, byte step)
        {
            if ((DateTime.Now - time).TotalMilliseconds > delaytime)
            {
                return ++step;
            }
            return 0;
        }
        private void btn_ClearDataGridView_Click(object sender, EventArgs e)
        {
            foreach (var temp in CaseConfig.caseList)
            {
                temp.DVResultInit();
                temp.DVDateInit();
                temp.Init();
            }
        }

        private void 清空提示信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            log.InsertLog(AppGlobal.ClearScanString);
        }

        private void hLToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void tcpLiveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        public static bool Time显示Flag = true;
        private void 显示运行时间ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Time显示Flag = true;
        }

        private void 隐藏运行时间ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Time显示Flag = false;
        }


        public static bool InitCurrentExitFlag = true;
        byte InitCurrentStep = 0;

        public DateTime InitStartTime;
        
        public void InitTransmitterDictionary()
        {
            try
            {
                foreach (var client in CANUDPClients)
                {
                    client.SignalDictionary.Clear();
                    client.TransmitterDictionary.Clear();
                    client.ReadMesDictionary.Clear();
                    client.SignalReadBytes.Clear();
                    string name = "";

                    foreach (var mes in client.Messages)
                    {
                        if (mes.WriteOrRead == WriteOrRead.Read)
                        {
                            Console.WriteLine(mes.Name);
                            client.ReadMesDictionary.Add(mes.Name, mes);
                            foreach (var signal in mes.Signals)
                            {
                                Console.WriteLine(mes.Name + " " + signal.Name);
                                client.SignalDictionary.Add(signal.Name, 0);
                            }
                        }
                        if (mes.WriteOrRead == WriteOrRead.Write)
                        {
                            Console.WriteLine(mes.Name);
                            client.TransmitterDictionary.Add(new CANUDPClient.UpdateMessage() { Name = mes.Name, Sendflag = false, UpdateTime = DateTime.Now }, client.StringToBytes(mes.Transmitter));
                        }
                    }
                }



            }
            catch (Exception ex)
            {

            }

        }
        public byte[] StringToBytes(string input)
        {
            string[] hexParts = input.Split(' ');
            byte[] result = new byte[hexParts.Length];

            for (int i = 0; i < hexParts.Length; i++)
            {
                result[i] = Convert.ToByte(hexParts[i], 16);
            }
            return result;
        }

        private void 数据保存路径设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogTestDatacsv.SelectPath();
        }

        private void cAN监控ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmCanMessage frmSetting = new FrmCanMessage();
            frmSetting.Show();
        }

        private void cAN报文配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmCanNode frmSetting = new FrmCanNode();
            frmSetting.Show();
        }

        private void 钥匙功能ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void btn_closeStatic_Click(object sender, EventArgs e)
        {
            Staticing = false;
        }
        async void StartWithSleepTest()
        {
            //if (LocalSetting.localSetting.MasterOrslave == 主从关系.主)
            //{
            //    ModbusTcpSalve.Data.HoldingRegisters[6 + 1] = 0;
            //    await ExecuteAfterDelayAsync(() =>
            //    {
            //        Console.WriteLine("This message appears after 3 seconds!");
            //        ModbusTcpSalve.Data.HoldingRegisters[6 + 1] = 1;
            //    });
            //}

            //TaskMainWorker.Instance.测试轮数 = 0;
            //AppGlobal.天线学习标志 = false;
            ////         WinForm.InitStaticExitFlag = false;
            //CAEA008TCPModbus.Instance._线圈管理器.插入线圈(303, 0);
            //CAEA008TCPModbus.Instance._线圈管理器.插入线圈(301, 0);
            //AT03TCPCommunicate.Instance.PinWriteHL("HL-37-0");
            //AT03TCPCommunicate.Instance.PinWriteHL("HL-40-0");
            //AT03TCPCommunicate.Instance.PinWriteHL("HL-68-0");
            //AT03TCPCommunicate.Instance.PinWriteHL("HL-69-0");
            //AT03TCPCommunicate.Instance.PinWritePWM("PWM-3-200-5000");
            //AT03TCPCommunicate.Instance.PinWritePWM("PWM-4-200-5000");
            //AT03TCPCommunicate.Instance.PinWritePWM("PWM-5-200-5000");
            //AT03TCPCommunicate.Instance.PinWritePWM("PWM-6-200-5000");
            //AT03TCPCommunicate.Instance.PinWritePWM("PWM-7-200-5000");
            //AT03TCPCommunicate.Instance.PinWritePWM("PWM-8-200-5000");
            //AT03TCPCommunicate.Instance.PinWritePWM("PWM-9-200-5000");//防止静态电流测完 没有复位
            //TaskMainWorker.Instance.TempTestStartFlag = true;
            //ThreadPool.QueueUserWorkItem(StartTest);
        }
        private void btn_WithSleepTest_Click(object sender, EventArgs e)
        {
            //   StartWithSleepTest();
        }

       

        private async void 从机暂停ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (LocalSetting.localSetting.MasterOrslave == 主从关系.主)
            //{
            //    ModbusTcpSalve.Data.HoldingRegisters[5 + 1] = 0;
            //    // 调用异步方法
            //    await ExecuteAfterDelayAsync(() =>
            //    {
            //        Console.WriteLine("This message appears after 3 seconds!");
            //        ModbusTcpSalve.Data.HoldingRegisters[5 + 1] = 1;
            //    });
            //}
        }
        // 异步方法：等待指定时间后执行操作
        public static async Task ExecuteAfterDelayAsync(Action action, int delayMilliseconds = 1000)
        {
            try
            {
                // 非阻塞等待3秒
                await Task.Delay(delayMilliseconds);
                // 执行传入的操作
                action?.Invoke();
            }
            catch (TaskCanceledException)
            {
                // 处理任务被取消的情况
                Console.WriteLine("Delay was cancelled");
            }
            catch (Exception ex)
            {
                // 处理其他异常
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void 打印调试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WinForm.barTenderPrint.LoadBtw(null);
         //   Save(2);
            WinForm.TryPrintLabelWithOfflineCache("打印调试");

        }

        private void 上电ToolStripMenuItem_Click(object sender, EventArgs e)
        {
         //   AT03TCPCommunicate.Instance.PinWriteHL("HL-108-0-0");//
        }

        private void 下电ToolStripMenuItem_Click(object sender, EventArgs e)
        {
        //    AT03TCPCommunicate.Instance.PinWriteHL("HL-108-0");//
        }

        private void 重载测试项ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 2026-05-11: 测试项CSV应从“产品测试项配置”目录加载。
            ParseTestLoad($"{LocalSetting.SystemDIR}\\产品测试项配置\\" + 型号列表.Text);//加载对应型号测试项
            CaseConfig.CreateParseItem();
        }

        private void 型号列表_TextChanged(object sender, EventArgs e)
        {
            // 2026-05-11: 测试项CSV应从“产品测试项配置”目录加载。
            ParseTestLoad($"{LocalSetting.SystemDIR}\\产品测试项配置\\" + 型号列表.Text);//加载对应型号测试项
            CaseConfig.CreateParseItem();
            DataGridView_TestFrm.RowCount = CaseConfig.caseList.Count;
            // 2026-05-11: 型号切换时同步切换CAN节点配置，避免写死型号。
            CANUDPClientsLoad($"{LocalSetting.SystemDIR}\\产品报文配置\\{型号列表.Text}");
        }
        // 刷新控制变量
        private int refreshThrottleInterval = 100; // 40ms刷新间隔 (25fps)
        private DateTime lastRefreshTime = DateTime.MinValue;
        private bool refreshPending;
        private readonly object refreshLock = new object();


        private Dictionary<string, ushort> previousState = new Dictionary<string, ushort>();
        // 用于保持顺序的键列表
        private  List<string> stateKeys;

        // 用于记录需要刷新的行（如果知道哪一行变化了，可以只刷新那一行）
        private HashSet<int> dirtyRows = new HashSet<int>();
        public void UpdateState(string key, ushort value)
        {
            // 更新字典值
            if (GZState.ContainsKey(key))
            {
                GZState[key] = value;

                // 找到该键对应的行索引
                int rowIndex = stateKeys.IndexOf(key);
                if (rowIndex >= 0)
                {
                    // 标记该行为需要刷新
                    dirtyRows.Add(rowIndex);

                    // 请求刷新该行（立即刷新，也可以选择在定时器中批量刷新）
                    dataGridView_GZState.InvalidateRow(rowIndex);
                }
            }
        }

        private void dataGridView_GZState_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= stateKeys.Count) return;

            string key = stateKeys[e.RowIndex];

            if (e.ColumnIndex == 0)
            {
                e.Value = key; // 第一列显示键
            }
            else if (e.ColumnIndex == 1)
            {
                // 第二列显示值
                if (GZState.TryGetValue(key, out ushort value))
                {
                    e.Value = value;
                }
                else
                {
                    e.Value = "N/A";
                }
            }
        }
        SerialPort Serial = null;
        private void StartSerial()
        {
            try
            {
                Serial = new SerialPort(LocalSetting.localSetting.扫码枪端口, LocalSetting.localSetting.扫码枪波特率);
                Serial.Open();
                Timer_Serial.Start();
                //   ScanMessage.InsertLog()
            }
            catch (Exception exp)
            {
                MessageBox.Show($"扫码枪串口操作失败：" + exp.Message);
                LogHelper.Warn($"扫码枪串口操作失败：" + exp.Message);
                Timer_Serial.Stop();

            }
        }
        public bool ScanFlag { get; set; } = false;
        byte[] ProductBuffer = new byte[1024];
        int ProductBufferlen = 0;
        public static byte ScanInputSuffix = 0x0D;
        /// <summary>
        /// 第二个结束符
        /// </summary>
        public static byte ScanInputSuffixTwo = 0x0A;
        public static string EncodingStr = "UTF-8";
        private void Timer_Serial_Tick(object sender, EventArgs e)
        {
            try
            {
                
                if (Serial != null && Serial.IsOpen)
                {
                    int count = Serial.BytesToRead;
                    byte[] tempBuffer = new byte[count];
                    if (count > 0)
                    {
                        Serial.Read(tempBuffer, 0, count);
                        LogHelper.Info("扫码获取内容:" + tempBuffer + "长度" + count);
                        if (ProductBufferlen > 500)
                        {
                            ProductBufferlen = 0;
                        }
                        for (int i = 0; i < count; i++)
                        {
                            ProductBuffer[ProductBufferlen++] = tempBuffer[i];
                        }
                        int len = -1;
                        if (ProductBuffer[ProductBufferlen - 1] == ScanInputSuffix)
                        {
                            len = ProductBufferlen - 1;
                        }
                        else if (ProductBuffer[ProductBufferlen - 2] == ScanInputSuffix && ProductBuffer[ProductBufferlen - 1] == ScanInputSuffixTwo)
                        {
                            len = ProductBufferlen - 2;
                        }
                        if (len > 0)
                        {
                            LogHelper.Info("Len" + len);
                            ProductBufferlen = 0;
                            Encoding encoding = Encoding.GetEncoding(EncodingStr);
                            string str = encoding.GetString(ProductBuffer, 0, len);
                            if(!检测是否复测(str))
                            {
                                _int扫码结果 = 3;
                                MessageBox.Show("该条码已测试，无法再次测试！");
                                ScanFlag = false;
                            }
                            else
                            {
                                _int扫码结果 = 2;
                                ScanFlag = true;
                            }
                            // ThreadTestStep.ScanBuffer.Enqueue(str);

                            //if (Settings.Default.不合格是否复测)
                            //{
                            //    var temp = GetTestResultList(str);
                            //    int failFlag = 0;
                            //    foreach (var test in temp)\3
                            //    {
                            //        if (test.data.resultCode != 2 && test.data.operationInfo.工位 == Settings.Default.追溯工位号)
                            //        {
                            //            failFlag++;
                            //        }
                            //    }
                            //    if (failFlag != 0)
                            //    {
                            //        MessageBox.Show("产品已在该工位测试不合格");
                            //    }
                            //}


                            _ScanString = str;
                          
                          
                            //FindName.Text = str;
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); LogHelper.Info(ex.Message); }
        }

        private void 停止ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            StopButtonEnable();
            Timer_Serial.Stop();
            InitExitFlag = true;
            foreach (var client in CANUDPClients)//遍历Message
            {
                client.StopFlag = true;
            }
        }

        public static bool 本次为复测 = false;
         bool 检测是否复测(string sn)
        {
            var st = WebServer.GetTestResult(sn);
            if(st.Length >= 1 && FrmLogin.TestNum == 0)
            {
              
                return false;
            }
             else if(FrmLogin.TestNum >= 1)
            {
                FrmLogin.TestNum--;
                本次为复测 = true;
                return true;
            }
            else { return true; }
        }

        private void 复测授权ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmLogin frmLogin = new FrmLogin();
            frmLogin.Show();
        }

        private void 下载测试项配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDownloadFromMes frmDownloadFromMes = new FrmDownloadFromMes();
            frmDownloadFromMes.Show();
        }

        private void 下载CAN节点配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDownloadJsonFromMes frmDownloadJsonFromMes = new FrmDownloadJsonFromMes();
            frmDownloadJsonFromMes.Show();
        }

        private void 上传测试项配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmUploadMes frmUploadMes = new FrmUploadMes();
            frmUploadMes.Show();
        }

        private void 上传CAN节点配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmUploadJsonToMes frmUploadJsonToMes = new FrmUploadJsonToMes();
            frmUploadJsonToMes.Show();
        }
    }
}
