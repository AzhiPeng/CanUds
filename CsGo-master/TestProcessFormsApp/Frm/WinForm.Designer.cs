namespace TestProcessFormsApp
{
    partial class WinForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WinForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.开始ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.停止ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tESTToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.清空提示信息ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打印调试ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.上电ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.下电ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.重载测试项ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.测试项设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.本地设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.数据保存路径设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cAN报文配置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.下载测试项配置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.下载CAN节点配置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.上传测试项配置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.上传CAN节点配置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.监控界面ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.d元件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cAN监控ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.复测授权ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.DataGridView_TestFrm = new System.Windows.Forms.DataGridView();
            this.测试项 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.测试数据 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.当前结果 = new System.Windows.Forms.DataGridViewImageColumn();
            this.userControlCanMessage1 = new TestProcessFormsApp.Frm.UserControlCanMessage();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.skinLabel6 = new CCWin.SkinControl.SkinLabel();
            this.panel_SaveResult = new System.Windows.Forms.Panel();
            this.splitContainer8 = new System.Windows.Forms.SplitContainer();
            this.产品选择 = new CCWin.SkinControl.SkinLabel();
            this.型号列表 = new System.Windows.Forms.ComboBox();
            this.lb_vbat = new CCWin.SkinControl.SkinLabel();
            this.skinLabel4 = new CCWin.SkinControl.SkinLabel();
            this.TestResult_state = new System.Windows.Forms.Panel();
            this.skinLabel3 = new CCWin.SkinControl.SkinLabel();
            this.splitContainer7 = new System.Windows.Forms.SplitContainer();
            this.测试次数 = new CCWin.SkinControl.SkinLabel();
            this.skinLabel2 = new CCWin.SkinControl.SkinLabel();
            this.Lb_DvTime = new CCWin.SkinControl.SkinLabel();
            this.skinLabel1 = new CCWin.SkinControl.SkinLabel();
            this.dataGridView_GZState = new System.Windows.Forms.DataGridView();
            this.splitContainer5 = new System.Windows.Forms.SplitContainer();
            this.btn_OnceTestClose = new CCWin.SkinControl.SkinButton();
            this.btn_ClearDataGridView = new CCWin.SkinControl.SkinButton();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.btn_DVStart = new CCWin.SkinControl.SkinButton();
            this.btn_DvStop = new CCWin.SkinControl.SkinButton();
            this.CAEA008通讯 = new CCWin.SkinControl.SkinLabel();
            this.Communication_state = new System.Windows.Forms.Panel();
            this.tb_Msg = new CCWin.SkinControl.SkinTextBox();
            this.Refalshtimer = new System.Windows.Forms.Timer(this.components);
            this.Timer_Serial = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView_TestFrm)).BeginInit();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer8)).BeginInit();
            this.splitContainer8.Panel1.SuspendLayout();
            this.splitContainer8.Panel2.SuspendLayout();
            this.splitContainer8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer7)).BeginInit();
            this.splitContainer7.Panel1.SuspendLayout();
            this.splitContainer7.Panel2.SuspendLayout();
            this.splitContainer7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_GZState)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).BeginInit();
            this.splitContainer5.Panel1.SuspendLayout();
            this.splitContainer5.Panel2.SuspendLayout();
            this.splitContainer5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.开始ToolStripMenuItem,
            this.停止ToolStripMenuItem,
            this.testToolStripMenuItem,
            this.设置ToolStripMenuItem,
            this.监控界面ToolStripMenuItem,
            this.复测授权ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(8, 39);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1264, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 开始ToolStripMenuItem
            // 
            this.开始ToolStripMenuItem.Name = "开始ToolStripMenuItem";
            this.开始ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.开始ToolStripMenuItem.Text = "开始";
            this.开始ToolStripMenuItem.Click += new System.EventHandler(this.开始ToolStripMenuItem_Click);
            // 
            // 停止ToolStripMenuItem
            // 
            this.停止ToolStripMenuItem.Name = "停止ToolStripMenuItem";
            this.停止ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.停止ToolStripMenuItem.Text = "停止";
            this.停止ToolStripMenuItem.Click += new System.EventHandler(this.停止ToolStripMenuItem_Click_1);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tESTToolStripMenuItem1,
            this.清空提示信息ToolStripMenuItem,
            this.打印调试ToolStripMenuItem,
            this.上电ToolStripMenuItem,
            this.下电ToolStripMenuItem,
            this.重载测试项ToolStripMenuItem});
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.testToolStripMenuItem.Text = "调试";
            this.testToolStripMenuItem.Click += new System.EventHandler(this.testToolStripMenuItem_Click);
            // 
            // tESTToolStripMenuItem1
            // 
            this.tESTToolStripMenuItem1.Name = "tESTToolStripMenuItem1";
            this.tESTToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.tESTToolStripMenuItem1.Text = "TEST";
            this.tESTToolStripMenuItem1.Click += new System.EventHandler(this.tESTToolStripMenuItem1_Click);
            // 
            // 清空提示信息ToolStripMenuItem
            // 
            this.清空提示信息ToolStripMenuItem.Name = "清空提示信息ToolStripMenuItem";
            this.清空提示信息ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.清空提示信息ToolStripMenuItem.Text = "清空提示信息";
            this.清空提示信息ToolStripMenuItem.Click += new System.EventHandler(this.清空提示信息ToolStripMenuItem_Click);
            // 
            // 打印调试ToolStripMenuItem
            // 
            this.打印调试ToolStripMenuItem.Name = "打印调试ToolStripMenuItem";
            this.打印调试ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.打印调试ToolStripMenuItem.Text = "打印调试";
            this.打印调试ToolStripMenuItem.Click += new System.EventHandler(this.打印调试ToolStripMenuItem_Click);
            // 
            // 上电ToolStripMenuItem
            // 
            this.上电ToolStripMenuItem.Name = "上电ToolStripMenuItem";
            this.上电ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.上电ToolStripMenuItem.Text = "上电";
            this.上电ToolStripMenuItem.Click += new System.EventHandler(this.上电ToolStripMenuItem_Click);
            // 
            // 下电ToolStripMenuItem
            // 
            this.下电ToolStripMenuItem.Name = "下电ToolStripMenuItem";
            this.下电ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.下电ToolStripMenuItem.Text = "下电";
            this.下电ToolStripMenuItem.Click += new System.EventHandler(this.下电ToolStripMenuItem_Click);
            // 
            // 重载测试项ToolStripMenuItem
            // 
            this.重载测试项ToolStripMenuItem.Name = "重载测试项ToolStripMenuItem";
            this.重载测试项ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.重载测试项ToolStripMenuItem.Text = "重载测试项";
            this.重载测试项ToolStripMenuItem.Click += new System.EventHandler(this.重载测试项ToolStripMenuItem_Click);
            // 
            // 设置ToolStripMenuItem
            // 
            this.设置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.测试项设置ToolStripMenuItem,
            this.本地设置ToolStripMenuItem,
            this.数据保存路径设置ToolStripMenuItem,
            this.cAN报文配置ToolStripMenuItem,
            this.下载测试项配置ToolStripMenuItem,
            this.下载CAN节点配置ToolStripMenuItem,
            this.上传测试项配置ToolStripMenuItem,
            this.上传CAN节点配置ToolStripMenuItem});
            this.设置ToolStripMenuItem.Name = "设置ToolStripMenuItem";
            this.设置ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.设置ToolStripMenuItem.Text = "设置";
            // 
            // 测试项设置ToolStripMenuItem
            // 
            this.测试项设置ToolStripMenuItem.Name = "测试项设置ToolStripMenuItem";
            this.测试项设置ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.测试项设置ToolStripMenuItem.Text = "测试项设置";
            this.测试项设置ToolStripMenuItem.Click += new System.EventHandler(this.测试项设置ToolStripMenuItem_Click);
            // 
            // 本地设置ToolStripMenuItem
            // 
            this.本地设置ToolStripMenuItem.Name = "本地设置ToolStripMenuItem";
            this.本地设置ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.本地设置ToolStripMenuItem.Text = "本地设置";
            this.本地设置ToolStripMenuItem.Click += new System.EventHandler(this.本地设置ToolStripMenuItem_Click);
            // 
            // 数据保存路径设置ToolStripMenuItem
            // 
            this.数据保存路径设置ToolStripMenuItem.Name = "数据保存路径设置ToolStripMenuItem";
            this.数据保存路径设置ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.数据保存路径设置ToolStripMenuItem.Text = "数据保存路径设置";
            this.数据保存路径设置ToolStripMenuItem.Click += new System.EventHandler(this.数据保存路径设置ToolStripMenuItem_Click);
            // 
            // cAN报文配置ToolStripMenuItem
            // 
            this.cAN报文配置ToolStripMenuItem.Name = "cAN报文配置ToolStripMenuItem";
            this.cAN报文配置ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.cAN报文配置ToolStripMenuItem.Text = "CAN报文配置";
            this.cAN报文配置ToolStripMenuItem.Click += new System.EventHandler(this.cAN报文配置ToolStripMenuItem_Click);
            // 
            // 下载测试项配置ToolStripMenuItem
            // 
            this.下载测试项配置ToolStripMenuItem.Name = "下载测试项配置ToolStripMenuItem";
            this.下载测试项配置ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.下载测试项配置ToolStripMenuItem.Text = "下载测试项配置";
            this.下载测试项配置ToolStripMenuItem.Click += new System.EventHandler(this.下载测试项配置ToolStripMenuItem_Click);
            // 
            // 下载CAN节点配置ToolStripMenuItem
            // 
            this.下载CAN节点配置ToolStripMenuItem.Name = "下载CAN节点配置ToolStripMenuItem";
            this.下载CAN节点配置ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.下载CAN节点配置ToolStripMenuItem.Text = "下载标签模板";
            this.下载CAN节点配置ToolStripMenuItem.Click += new System.EventHandler(this.下载CAN节点配置ToolStripMenuItem_Click);
            // 
            // 上传测试项配置ToolStripMenuItem
            // 
            this.上传测试项配置ToolStripMenuItem.Name = "上传测试项配置ToolStripMenuItem";
            this.上传测试项配置ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.上传测试项配置ToolStripMenuItem.Text = "上传测试项配置";
            this.上传测试项配置ToolStripMenuItem.Click += new System.EventHandler(this.上传测试项配置ToolStripMenuItem_Click);
            // 
            // 上传CAN节点配置ToolStripMenuItem
            // 
            this.上传CAN节点配置ToolStripMenuItem.Name = "上传CAN节点配置ToolStripMenuItem";
            this.上传CAN节点配置ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.上传CAN节点配置ToolStripMenuItem.Text = "上传标签模板";
            this.上传CAN节点配置ToolStripMenuItem.Click += new System.EventHandler(this.上传CAN节点配置ToolStripMenuItem_Click);
            // 
            // 监控界面ToolStripMenuItem
            // 
            this.监控界面ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.d元件ToolStripMenuItem,
            this.cAN监控ToolStripMenuItem});
            this.监控界面ToolStripMenuItem.Name = "监控界面ToolStripMenuItem";
            this.监控界面ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.监控界面ToolStripMenuItem.Text = "监控界面";
            // 
            // d元件ToolStripMenuItem
            // 
            this.d元件ToolStripMenuItem.Name = "d元件ToolStripMenuItem";
            this.d元件ToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.d元件ToolStripMenuItem.Text = "D元件";
            this.d元件ToolStripMenuItem.Click += new System.EventHandler(this.d元件ToolStripMenuItem_Click);
            // 
            // cAN监控ToolStripMenuItem
            // 
            this.cAN监控ToolStripMenuItem.Name = "cAN监控ToolStripMenuItem";
            this.cAN监控ToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.cAN监控ToolStripMenuItem.Text = "CAN监控";
            this.cAN监控ToolStripMenuItem.Click += new System.EventHandler(this.cAN监控ToolStripMenuItem_Click);
            // 
            // 复测授权ToolStripMenuItem
            // 
            this.复测授权ToolStripMenuItem.Name = "复测授权ToolStripMenuItem";
            this.复测授权ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.复测授权ToolStripMenuItem.Text = "复测授权";
            this.复测授权ToolStripMenuItem.Visible = false;
            this.复测授权ToolStripMenuItem.Click += new System.EventHandler(this.复测授权ToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(8, 64);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer3);
            this.splitContainer1.Panel1.Controls.Add(this.statusStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1264, 679);
            this.splitContainer1.SplitterDistance = 810;
            this.splitContainer1.TabIndex = 1;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.DataGridView_TestFrm);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.userControlCanMessage1);
            this.splitContainer3.Size = new System.Drawing.Size(810, 657);
            this.splitContainer3.SplitterDistance = 360;
            this.splitContainer3.TabIndex = 869;
            // 
            // DataGridView_TestFrm
            // 
            this.DataGridView_TestFrm.AllowUserToAddRows = false;
            this.DataGridView_TestFrm.AllowUserToDeleteRows = false;
            this.DataGridView_TestFrm.AllowUserToResizeRows = false;
            this.DataGridView_TestFrm.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.DataGridView_TestFrm.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DataGridView_TestFrm.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.测试项,
            this.测试数据,
            this.当前结果});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DataGridView_TestFrm.DefaultCellStyle = dataGridViewCellStyle2;
            this.DataGridView_TestFrm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataGridView_TestFrm.Location = new System.Drawing.Point(0, 0);
            this.DataGridView_TestFrm.Name = "DataGridView_TestFrm";
            this.DataGridView_TestFrm.ReadOnly = true;
            this.DataGridView_TestFrm.RowHeadersWidth = 60;
            this.DataGridView_TestFrm.RowTemplate.Height = 23;
            this.DataGridView_TestFrm.Size = new System.Drawing.Size(810, 360);
            this.DataGridView_TestFrm.TabIndex = 867;
            this.DataGridView_TestFrm.VirtualMode = true;
            this.DataGridView_TestFrm.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_TestFrm_CellDoubleClick);
            this.DataGridView_TestFrm.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.DataGridView_TestFrm_CellValueNeeded_1);
            this.DataGridView_TestFrm.RowStateChanged += new System.Windows.Forms.DataGridViewRowStateChangedEventHandler(this.DataGridView_TestFrm_RowStateChanged);
            // 
            // 测试项
            // 
            this.测试项.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.测试项.FillWeight = 20F;
            this.测试项.HeaderText = "测试项";
            this.测试项.Name = "测试项";
            this.测试项.ReadOnly = true;
            this.测试项.Width = 66;
            // 
            // 测试数据
            // 
            this.测试数据.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.测试数据.FillWeight = 5.42635F;
            this.测试数据.HeaderText = "测试数据";
            this.测试数据.Name = "测试数据";
            this.测试数据.ReadOnly = true;
            // 
            // 当前结果
            // 
            this.当前结果.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.当前结果.FillWeight = 10F;
            this.当前结果.HeaderText = "当前结果";
            this.当前结果.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.当前结果.Name = "当前结果";
            this.当前结果.ReadOnly = true;
            this.当前结果.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.当前结果.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.当前结果.Width = 78;
            // 
            // userControlCanMessage1
            // 
            this.userControlCanMessage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userControlCanMessage1.Location = new System.Drawing.Point(0, 0);
            this.userControlCanMessage1.Name = "userControlCanMessage1";
            this.userControlCanMessage1.Size = new System.Drawing.Size(810, 293);
            this.userControlCanMessage1.TabIndex = 0;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 657);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(810, 22);
            this.statusStrip1.TabIndex = 868;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BackColor = System.Drawing.Color.White;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(56, 17);
            this.toolStripStatusLabel1.Text = "编译时间";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.skinLabel6);
            this.splitContainer2.Panel1.Controls.Add(this.panel_SaveResult);
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer8);
            this.splitContainer2.Panel1.Controls.Add(this.TestResult_state);
            this.splitContainer2.Panel1.Controls.Add(this.skinLabel3);
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer7);
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer5);
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer4);
            this.splitContainer2.Panel1.Controls.Add(this.CAEA008通讯);
            this.splitContainer2.Panel1.Controls.Add(this.Communication_state);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tb_Msg);
            this.splitContainer2.Size = new System.Drawing.Size(450, 679);
            this.splitContainer2.SplitterDistance = 410;
            this.splitContainer2.TabIndex = 0;
            // 
            // skinLabel6
            // 
            this.skinLabel6.AutoSize = true;
            this.skinLabel6.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel6.BorderColor = System.Drawing.Color.White;
            this.skinLabel6.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinLabel6.ForeColor = System.Drawing.Color.White;
            this.skinLabel6.Location = new System.Drawing.Point(316, 6);
            this.skinLabel6.Name = "skinLabel6";
            this.skinLabel6.Size = new System.Drawing.Size(89, 19);
            this.skinLabel6.TabIndex = 881;
            this.skinLabel6.Text = "上传结果";
            // 
            // panel_SaveResult
            // 
            this.panel_SaveResult.BackgroundImage = global::TestProcessFormsApp.Properties.Resources.就绪;
            this.panel_SaveResult.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel_SaveResult.Location = new System.Drawing.Point(411, 3);
            this.panel_SaveResult.Name = "panel_SaveResult";
            this.panel_SaveResult.Size = new System.Drawing.Size(28, 29);
            this.panel_SaveResult.TabIndex = 871;
            // 
            // splitContainer8
            // 
            this.splitContainer8.Location = new System.Drawing.Point(4, 38);
            this.splitContainer8.Name = "splitContainer8";
            // 
            // splitContainer8.Panel1
            // 
            this.splitContainer8.Panel1.Controls.Add(this.产品选择);
            this.splitContainer8.Panel1.Controls.Add(this.型号列表);
            // 
            // splitContainer8.Panel2
            // 
            this.splitContainer8.Panel2.Controls.Add(this.lb_vbat);
            this.splitContainer8.Panel2.Controls.Add(this.skinLabel4);
            this.splitContainer8.Size = new System.Drawing.Size(446, 68);
            this.splitContainer8.SplitterDistance = 278;
            this.splitContainer8.TabIndex = 880;
            // 
            // 产品选择
            // 
            this.产品选择.AutoSize = true;
            this.产品选择.BackColor = System.Drawing.Color.Transparent;
            this.产品选择.BorderColor = System.Drawing.Color.White;
            this.产品选择.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.产品选择.ForeColor = System.Drawing.Color.White;
            this.产品选择.Location = new System.Drawing.Point(-4, 9);
            this.产品选择.Name = "产品选择";
            this.产品选择.Size = new System.Drawing.Size(291, 19);
            this.产品选择.TabIndex = 881;
            this.产品选择.Text = "产品型号(切换型号请重启程序)";
            // 
            // 型号列表
            // 
            this.型号列表.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.型号列表.FormattingEnabled = true;
            this.型号列表.Items.AddRange(new object[] {
            "0Y4G40AAEWWA",
            "0Y4G40BAEWWA",
            "0Y4G40CAEWWA",
            "0Y4G40DAEWWA",
            "0Y4Z40AAEWWA",
            "0Y4Z40CAEWWA",
            "0Y4Z40DAEWWA"});
            this.型号列表.Location = new System.Drawing.Point(0, 28);
            this.型号列表.Name = "型号列表";
            this.型号列表.Size = new System.Drawing.Size(275, 37);
            this.型号列表.TabIndex = 1189;
            // 
            // lb_vbat
            // 
            this.lb_vbat.AutoSize = true;
            this.lb_vbat.BackColor = System.Drawing.Color.Transparent;
            this.lb_vbat.BorderColor = System.Drawing.Color.White;
            this.lb_vbat.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_vbat.ForeColor = System.Drawing.Color.White;
            this.lb_vbat.Location = new System.Drawing.Point(16, 40);
            this.lb_vbat.Name = "lb_vbat";
            this.lb_vbat.Size = new System.Drawing.Size(53, 19);
            this.lb_vbat.TabIndex = 881;
            this.lb_vbat.Text = "vbat";
            this.lb_vbat.Visible = false;
            // 
            // skinLabel4
            // 
            this.skinLabel4.AutoSize = true;
            this.skinLabel4.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel4.BorderColor = System.Drawing.Color.White;
            this.skinLabel4.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinLabel4.ForeColor = System.Drawing.Color.White;
            this.skinLabel4.Location = new System.Drawing.Point(4, 9);
            this.skinLabel4.Name = "skinLabel4";
            this.skinLabel4.Size = new System.Drawing.Size(109, 19);
            this.skinLabel4.TabIndex = 880;
            this.skinLabel4.Text = "产品电压：";
            this.skinLabel4.Visible = false;
            // 
            // TestResult_state
            // 
            this.TestResult_state.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("TestResult_state.BackgroundImage")));
            this.TestResult_state.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.TestResult_state.Location = new System.Drawing.Point(287, 0);
            this.TestResult_state.Name = "TestResult_state";
            this.TestResult_state.Size = new System.Drawing.Size(28, 29);
            this.TestResult_state.TabIndex = 870;
            // 
            // skinLabel3
            // 
            this.skinLabel3.AutoSize = true;
            this.skinLabel3.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel3.BorderColor = System.Drawing.Color.White;
            this.skinLabel3.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinLabel3.ForeColor = System.Drawing.Color.White;
            this.skinLabel3.Location = new System.Drawing.Point(162, 3);
            this.skinLabel3.Name = "skinLabel3";
            this.skinLabel3.Size = new System.Drawing.Size(129, 19);
            this.skinLabel3.TabIndex = 879;
            this.skinLabel3.Text = "本轮测试结果";
            // 
            // splitContainer7
            // 
            this.splitContainer7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitContainer7.Location = new System.Drawing.Point(0, 175);
            this.splitContainer7.Name = "splitContainer7";
            // 
            // splitContainer7.Panel1
            // 
            this.splitContainer7.Panel1.Controls.Add(this.测试次数);
            this.splitContainer7.Panel1.Controls.Add(this.skinLabel2);
            this.splitContainer7.Panel1.Controls.Add(this.Lb_DvTime);
            this.splitContainer7.Panel1.Controls.Add(this.skinLabel1);
            // 
            // splitContainer7.Panel2
            // 
            this.splitContainer7.Panel2.Controls.Add(this.dataGridView_GZState);
            this.splitContainer7.Size = new System.Drawing.Size(450, 139);
            this.splitContainer7.SplitterDistance = 143;
            this.splitContainer7.TabIndex = 878;
            // 
            // 测试次数
            // 
            this.测试次数.AutoSize = true;
            this.测试次数.BackColor = System.Drawing.Color.Transparent;
            this.测试次数.BorderColor = System.Drawing.Color.White;
            this.测试次数.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.测试次数.ForeColor = System.Drawing.Color.White;
            this.测试次数.Location = new System.Drawing.Point(5, 108);
            this.测试次数.Name = "测试次数";
            this.测试次数.Size = new System.Drawing.Size(119, 19);
            this.测试次数.TabIndex = 882;
            this.测试次数.Text = "testnumber";
            // 
            // skinLabel2
            // 
            this.skinLabel2.AutoSize = true;
            this.skinLabel2.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel2.BorderColor = System.Drawing.Color.White;
            this.skinLabel2.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinLabel2.ForeColor = System.Drawing.Color.White;
            this.skinLabel2.Location = new System.Drawing.Point(3, 85);
            this.skinLabel2.Name = "skinLabel2";
            this.skinLabel2.Size = new System.Drawing.Size(89, 19);
            this.skinLabel2.TabIndex = 882;
            this.skinLabel2.Text = "测试次数";
            // 
            // Lb_DvTime
            // 
            this.Lb_DvTime.AutoSize = true;
            this.Lb_DvTime.BackColor = System.Drawing.Color.Transparent;
            this.Lb_DvTime.BorderColor = System.Drawing.Color.White;
            this.Lb_DvTime.Dock = System.Windows.Forms.DockStyle.Left;
            this.Lb_DvTime.Font = new System.Drawing.Font("宋体", 16.75F, System.Drawing.FontStyle.Bold);
            this.Lb_DvTime.ForeColor = System.Drawing.Color.White;
            this.Lb_DvTime.Location = new System.Drawing.Point(0, 19);
            this.Lb_DvTime.Name = "Lb_DvTime";
            this.Lb_DvTime.Size = new System.Drawing.Size(140, 23);
            this.Lb_DvTime.TabIndex = 879;
            this.Lb_DvTime.Text = "DvDateTime";
            // 
            // skinLabel1
            // 
            this.skinLabel1.AutoSize = true;
            this.skinLabel1.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel1.BorderColor = System.Drawing.Color.White;
            this.skinLabel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.skinLabel1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinLabel1.ForeColor = System.Drawing.Color.White;
            this.skinLabel1.Location = new System.Drawing.Point(0, 0);
            this.skinLabel1.Name = "skinLabel1";
            this.skinLabel1.Size = new System.Drawing.Size(89, 19);
            this.skinLabel1.TabIndex = 878;
            this.skinLabel1.Text = "运行时间";
            // 
            // dataGridView_GZState
            // 
            this.dataGridView_GZState.AllowUserToAddRows = false;
            this.dataGridView_GZState.AllowUserToDeleteRows = false;
            this.dataGridView_GZState.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_GZState.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_GZState.Name = "dataGridView_GZState";
            this.dataGridView_GZState.ReadOnly = true;
            this.dataGridView_GZState.RowTemplate.Height = 23;
            this.dataGridView_GZState.Size = new System.Drawing.Size(303, 139);
            this.dataGridView_GZState.TabIndex = 0;
            this.dataGridView_GZState.Visible = false;
            // 
            // splitContainer5
            // 
            this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitContainer5.Location = new System.Drawing.Point(0, 314);
            this.splitContainer5.Name = "splitContainer5";
            // 
            // splitContainer5.Panel1
            // 
            this.splitContainer5.Panel1.Controls.Add(this.btn_OnceTestClose);
            // 
            // splitContainer5.Panel2
            // 
            this.splitContainer5.Panel2.Controls.Add(this.btn_ClearDataGridView);
            this.splitContainer5.Size = new System.Drawing.Size(450, 46);
            this.splitContainer5.SplitterDistance = 221;
            this.splitContainer5.TabIndex = 876;
            // 
            // btn_OnceTestClose
            // 
            this.btn_OnceTestClose.BackColor = System.Drawing.Color.Transparent;
            this.btn_OnceTestClose.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_OnceTestClose.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_OnceTestClose.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btn_OnceTestClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_OnceTestClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_OnceTestClose.DownBack = null;
            this.btn_OnceTestClose.DownBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_OnceTestClose.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_OnceTestClose.ForeColor = System.Drawing.Color.White;
            this.btn_OnceTestClose.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_OnceTestClose.InheritColor = true;
            this.btn_OnceTestClose.InnerBorderColor = System.Drawing.Color.White;
            this.btn_OnceTestClose.IsDrawGlass = false;
            this.btn_OnceTestClose.Location = new System.Drawing.Point(0, 0);
            this.btn_OnceTestClose.MouseBack = null;
            this.btn_OnceTestClose.MouseBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_OnceTestClose.Name = "btn_OnceTestClose";
            this.btn_OnceTestClose.NormlBack = null;
            this.btn_OnceTestClose.Radius = 20;
            this.btn_OnceTestClose.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btn_OnceTestClose.Size = new System.Drawing.Size(221, 46);
            this.btn_OnceTestClose.TabIndex = 869;
            this.btn_OnceTestClose.Text = "关闭手动测试";
            this.btn_OnceTestClose.UseVisualStyleBackColor = false;
            this.btn_OnceTestClose.Click += new System.EventHandler(this.btn_OnceTestClose_Click);
            // 
            // btn_ClearDataGridView
            // 
            this.btn_ClearDataGridView.BackColor = System.Drawing.Color.Transparent;
            this.btn_ClearDataGridView.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_ClearDataGridView.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_ClearDataGridView.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btn_ClearDataGridView.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_ClearDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_ClearDataGridView.DownBack = null;
            this.btn_ClearDataGridView.DownBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_ClearDataGridView.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_ClearDataGridView.ForeColor = System.Drawing.Color.White;
            this.btn_ClearDataGridView.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_ClearDataGridView.InheritColor = true;
            this.btn_ClearDataGridView.InnerBorderColor = System.Drawing.Color.White;
            this.btn_ClearDataGridView.IsDrawGlass = false;
            this.btn_ClearDataGridView.Location = new System.Drawing.Point(0, 0);
            this.btn_ClearDataGridView.MouseBack = null;
            this.btn_ClearDataGridView.MouseBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_ClearDataGridView.Name = "btn_ClearDataGridView";
            this.btn_ClearDataGridView.NormlBack = null;
            this.btn_ClearDataGridView.Radius = 20;
            this.btn_ClearDataGridView.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btn_ClearDataGridView.Size = new System.Drawing.Size(225, 46);
            this.btn_ClearDataGridView.TabIndex = 873;
            this.btn_ClearDataGridView.Text = "测试项/测试结果清空";
            this.btn_ClearDataGridView.UseVisualStyleBackColor = false;
            this.btn_ClearDataGridView.Click += new System.EventHandler(this.btn_ClearDataGridView_Click);
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitContainer4.Location = new System.Drawing.Point(0, 360);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.btn_DVStart);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.btn_DvStop);
            this.splitContainer4.Size = new System.Drawing.Size(450, 50);
            this.splitContainer4.SplitterDistance = 220;
            this.splitContainer4.TabIndex = 875;
            // 
            // btn_DVStart
            // 
            this.btn_DVStart.BackColor = System.Drawing.Color.Transparent;
            this.btn_DVStart.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_DVStart.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_DVStart.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btn_DVStart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_DVStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_DVStart.DownBack = null;
            this.btn_DVStart.DownBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_DVStart.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_DVStart.ForeColor = System.Drawing.Color.White;
            this.btn_DVStart.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_DVStart.InheritColor = true;
            this.btn_DVStart.InnerBorderColor = System.Drawing.Color.White;
            this.btn_DVStart.IsDrawGlass = false;
            this.btn_DVStart.Location = new System.Drawing.Point(0, 0);
            this.btn_DVStart.MouseBack = null;
            this.btn_DVStart.MouseBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_DVStart.Name = "btn_DVStart";
            this.btn_DVStart.NormlBack = null;
            this.btn_DVStart.Radius = 20;
            this.btn_DVStart.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btn_DVStart.Size = new System.Drawing.Size(220, 50);
            this.btn_DVStart.TabIndex = 871;
            this.btn_DVStart.Text = "开启测试";
            this.btn_DVStart.UseVisualStyleBackColor = false;
            this.btn_DVStart.Click += new System.EventHandler(this.btn_DVStart_Click);
            // 
            // btn_DvStop
            // 
            this.btn_DvStop.BackColor = System.Drawing.Color.Transparent;
            this.btn_DvStop.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_DvStop.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_DvStop.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btn_DvStop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_DvStop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_DvStop.DownBack = null;
            this.btn_DvStop.DownBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_DvStop.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_DvStop.ForeColor = System.Drawing.Color.White;
            this.btn_DvStop.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_DvStop.InheritColor = true;
            this.btn_DvStop.InnerBorderColor = System.Drawing.Color.White;
            this.btn_DvStop.IsDrawGlass = false;
            this.btn_DvStop.Location = new System.Drawing.Point(0, 0);
            this.btn_DvStop.MouseBack = null;
            this.btn_DvStop.MouseBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_DvStop.Name = "btn_DvStop";
            this.btn_DvStop.NormlBack = null;
            this.btn_DvStop.Radius = 20;
            this.btn_DvStop.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btn_DvStop.Size = new System.Drawing.Size(226, 50);
            this.btn_DvStop.TabIndex = 870;
            this.btn_DvStop.Text = "关闭测试";
            this.btn_DvStop.UseVisualStyleBackColor = false;
            this.btn_DvStop.Click += new System.EventHandler(this.btn_DvStop_Click);
            // 
            // CAEA008通讯
            // 
            this.CAEA008通讯.AutoSize = true;
            this.CAEA008通讯.BackColor = System.Drawing.Color.Transparent;
            this.CAEA008通讯.BorderColor = System.Drawing.Color.White;
            this.CAEA008通讯.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CAEA008通讯.ForeColor = System.Drawing.Color.White;
            this.CAEA008通讯.Location = new System.Drawing.Point(2, 3);
            this.CAEA008通讯.Name = "CAEA008通讯";
            this.CAEA008通讯.Size = new System.Drawing.Size(126, 19);
            this.CAEA008通讯.TabIndex = 128;
            this.CAEA008通讯.Text = "CAEA008通讯";
            // 
            // Communication_state
            // 
            this.Communication_state.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Communication_state.BackgroundImage")));
            this.Communication_state.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Communication_state.Location = new System.Drawing.Point(128, 0);
            this.Communication_state.Name = "Communication_state";
            this.Communication_state.Size = new System.Drawing.Size(28, 29);
            this.Communication_state.TabIndex = 869;
            // 
            // tb_Msg
            // 
            this.tb_Msg.AutoScroll = true;
            this.tb_Msg.BackColor = System.Drawing.Color.Transparent;
            this.tb_Msg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_Msg.DownBack = null;
            this.tb_Msg.Icon = null;
            this.tb_Msg.IconIsButton = false;
            this.tb_Msg.IconMouseState = CCWin.SkinClass.ControlState.Normal;
            this.tb_Msg.IsPasswordChat = '\0';
            this.tb_Msg.IsSystemPasswordChar = false;
            this.tb_Msg.Lines = new string[0];
            this.tb_Msg.Location = new System.Drawing.Point(0, 0);
            this.tb_Msg.Margin = new System.Windows.Forms.Padding(0);
            this.tb_Msg.MaxLength = 32767;
            this.tb_Msg.MinimumSize = new System.Drawing.Size(28, 28);
            this.tb_Msg.MouseBack = null;
            this.tb_Msg.MouseState = CCWin.SkinClass.ControlState.Normal;
            this.tb_Msg.Multiline = true;
            this.tb_Msg.Name = "tb_Msg";
            this.tb_Msg.NormlBack = null;
            this.tb_Msg.Padding = new System.Windows.Forms.Padding(5);
            this.tb_Msg.ReadOnly = false;
            this.tb_Msg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_Msg.Size = new System.Drawing.Size(450, 265);
            // 
            // 
            // 
            this.tb_Msg.SkinTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tb_Msg.SkinTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_Msg.SkinTxt.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.tb_Msg.SkinTxt.Location = new System.Drawing.Point(5, 5);
            this.tb_Msg.SkinTxt.Multiline = true;
            this.tb_Msg.SkinTxt.Name = "BaseText";
            this.tb_Msg.SkinTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_Msg.SkinTxt.Size = new System.Drawing.Size(440, 255);
            this.tb_Msg.SkinTxt.TabIndex = 0;
            this.tb_Msg.SkinTxt.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.tb_Msg.SkinTxt.WaterText = "提示信息";
            this.tb_Msg.TabIndex = 2;
            this.tb_Msg.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.tb_Msg.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.tb_Msg.WaterText = "提示信息";
            this.tb_Msg.WordWrap = true;
            // 
            // Refalshtimer
            // 
            this.Refalshtimer.Interval = 200;
            this.Refalshtimer.Tick += new System.EventHandler(this.Refalshtimer_Tick);
            // 
            // Timer_Serial
            // 
            this.Timer_Serial.Tick += new System.EventHandler(this.Timer_Serial_Tick);
            // 
            // WinForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 751);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "WinForm";
            this.Text = "通用发样校验上位机测试程序";
            this.Load += new System.EventHandler(this.WinForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView_TestFrm)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer8.Panel1.ResumeLayout(false);
            this.splitContainer8.Panel1.PerformLayout();
            this.splitContainer8.Panel2.ResumeLayout(false);
            this.splitContainer8.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer8)).EndInit();
            this.splitContainer8.ResumeLayout(false);
            this.splitContainer7.Panel1.ResumeLayout(false);
            this.splitContainer7.Panel1.PerformLayout();
            this.splitContainer7.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer7)).EndInit();
            this.splitContainer7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_GZState)).EndInit();
            this.splitContainer5.Panel1.ResumeLayout(false);
            this.splitContainer5.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).EndInit();
            this.splitContainer5.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 开始ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 停止ToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView DataGridView_TestFrm;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private CCWin.SkinControl.SkinTextBox tb_Msg;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ToolStripMenuItem 设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 测试项设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 本地设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tESTToolStripMenuItem1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private CCWin.SkinControl.SkinLabel CAEA008通讯;
        private System.Windows.Forms.Panel Communication_state;
        private System.Windows.Forms.Timer Refalshtimer;
        private CCWin.SkinControl.SkinButton btn_OnceTestClose;
        private CCWin.SkinControl.SkinButton btn_DVStart;
        private CCWin.SkinControl.SkinButton btn_DvStop;
        private System.Windows.Forms.ToolStripMenuItem 监控界面ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem d元件ToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer5;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private CCWin.SkinControl.SkinButton btn_ClearDataGridView;
        private CCWin.SkinControl.SkinLabel Lb_DvTime;
        private CCWin.SkinControl.SkinLabel skinLabel1;
        private System.Windows.Forms.SplitContainer splitContainer7;
        private System.Windows.Forms.ToolStripMenuItem 清空提示信息ToolStripMenuItem;
        private System.Windows.Forms.Panel TestResult_state;
        private CCWin.SkinControl.SkinLabel skinLabel3;
        private System.Windows.Forms.SplitContainer splitContainer8;
        private CCWin.SkinControl.SkinLabel lb_vbat;
        private CCWin.SkinControl.SkinLabel skinLabel4;
        private System.Windows.Forms.ToolStripMenuItem 数据保存路径设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cAN监控ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cAN报文配置ToolStripMenuItem;
        private System.Windows.Forms.Panel panel_SaveResult;
        private System.Windows.Forms.DataGridViewTextBoxColumn 测试项;
        private System.Windows.Forms.DataGridViewTextBoxColumn 测试数据;
        private System.Windows.Forms.DataGridViewImageColumn 当前结果;
        private CCWin.SkinControl.SkinLabel 产品选择;
        private System.Windows.Forms.ComboBox 型号列表;
        private System.Windows.Forms.ToolStripMenuItem 打印调试ToolStripMenuItem;
        private CCWin.SkinControl.SkinLabel skinLabel6;
        private System.Windows.Forms.ToolStripMenuItem 上电ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 下电ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 重载测试项ToolStripMenuItem;
        private System.Windows.Forms.DataGridView dataGridView_GZState;
        private CCWin.SkinControl.SkinLabel 测试次数;
        private CCWin.SkinControl.SkinLabel skinLabel2;
        private System.Windows.Forms.Timer Timer_Serial;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.ToolStripMenuItem 复测授权ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 下载测试项配置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 下载CAN节点配置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 上传测试项配置ToolStripMenuItem;
        private Frm.UserControlCanMessage userControlCanMessage1;
        private System.Windows.Forms.ToolStripMenuItem 上传CAN节点配置ToolStripMenuItem;
    }
}

