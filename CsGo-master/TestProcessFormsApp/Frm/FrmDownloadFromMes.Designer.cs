namespace TestProcessFormsApp.Frm
{
    partial class FrmDownloadFromMes
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.Label_DownLoadName = new CCWin.SkinControl.SkinLabel();
            this.skinLabel2 = new CCWin.SkinControl.SkinLabel();
            this.btn_Find = new CCWin.SkinControl.SkinButton();
            this.btn_Flash = new CCWin.SkinControl.SkinButton();
            this.skinDataGridView1 = new CCWin.SkinControl.SkinDataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FindName = new CCWin.SkinControl.SkinTextBox();
            this.Btn_Download = new CCWin.SkinControl.SkinButton();
            this.Btn_Close = new CCWin.SkinControl.SkinButton();
            ((System.ComponentModel.ISupportInitialize)(this.skinDataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // Label_DownLoadName
            // 
            this.Label_DownLoadName.AutoSize = true;
            this.Label_DownLoadName.BackColor = System.Drawing.Color.Transparent;
            this.Label_DownLoadName.BorderColor = System.Drawing.Color.White;
            this.Label_DownLoadName.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Label_DownLoadName.ForeColor = System.Drawing.SystemColors.Control;
            this.Label_DownLoadName.Location = new System.Drawing.Point(162, 50);
            this.Label_DownLoadName.Name = "Label_DownLoadName";
            this.Label_DownLoadName.Size = new System.Drawing.Size(56, 21);
            this.Label_DownLoadName.TabIndex = 48;
            this.Label_DownLoadName.Text = "Name";
            // 
            // skinLabel2
            // 
            this.skinLabel2.AutoSize = true;
            this.skinLabel2.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel2.BorderColor = System.Drawing.Color.White;
            this.skinLabel2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinLabel2.ForeColor = System.Drawing.SystemColors.Control;
            this.skinLabel2.Location = new System.Drawing.Point(20, 50);
            this.skinLabel2.Name = "skinLabel2";
            this.skinLabel2.Size = new System.Drawing.Size(122, 21);
            this.skinLabel2.TabIndex = 47;
            this.skinLabel2.Text = "下载文件名称：";
            // 
            // btn_Find
            // 
            this.btn_Find.BackColor = System.Drawing.Color.Transparent;
            this.btn_Find.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_Find.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_Find.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btn_Find.DownBack = null;
            this.btn_Find.DownBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_Find.ForeColor = System.Drawing.Color.White;
            this.btn_Find.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_Find.InheritColor = true;
            this.btn_Find.InnerBorderColor = System.Drawing.Color.White;
            this.btn_Find.IsDrawGlass = false;
            this.btn_Find.Location = new System.Drawing.Point(230, 383);
            this.btn_Find.MouseBack = null;
            this.btn_Find.MouseBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_Find.Name = "btn_Find";
            this.btn_Find.NormlBack = null;
            this.btn_Find.Radius = 20;
            this.btn_Find.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btn_Find.Size = new System.Drawing.Size(97, 28);
            this.btn_Find.TabIndex = 46;
            this.btn_Find.Text = "查找";
            this.btn_Find.UseVisualStyleBackColor = false;
            this.btn_Find.Click += new System.EventHandler(this.btn_Find_Click);
            // 
            // btn_Flash
            // 
            this.btn_Flash.BackColor = System.Drawing.Color.Transparent;
            this.btn_Flash.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_Flash.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_Flash.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btn_Flash.DownBack = null;
            this.btn_Flash.DownBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_Flash.ForeColor = System.Drawing.Color.White;
            this.btn_Flash.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_Flash.InheritColor = true;
            this.btn_Flash.InnerBorderColor = System.Drawing.Color.White;
            this.btn_Flash.IsDrawGlass = false;
            this.btn_Flash.Location = new System.Drawing.Point(24, 383);
            this.btn_Flash.MouseBack = null;
            this.btn_Flash.MouseBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_Flash.Name = "btn_Flash";
            this.btn_Flash.NormlBack = null;
            this.btn_Flash.Radius = 20;
            this.btn_Flash.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btn_Flash.Size = new System.Drawing.Size(97, 28);
            this.btn_Flash.TabIndex = 45;
            this.btn_Flash.Text = "刷新";
            this.btn_Flash.UseVisualStyleBackColor = false;
            this.btn_Flash.Click += new System.EventHandler(this.btn_Flash_Click);
            // 
            // skinDataGridView1
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(246)))), ((int)(((byte)(253)))));
            this.skinDataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.skinDataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.skinDataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.skinDataGridView1.ColumnFont = null;
            this.skinDataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(246)))), ((int)(((byte)(239)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.skinDataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.skinDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.skinDataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5});
            this.skinDataGridView1.ColumnSelectForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(188)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.skinDataGridView1.DefaultCellStyle = dataGridViewCellStyle3;
            this.skinDataGridView1.EnableHeadersVisualStyles = false;
            this.skinDataGridView1.GridColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.skinDataGridView1.HeadFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinDataGridView1.HeadSelectForeColor = System.Drawing.SystemColors.HighlightText;
            this.skinDataGridView1.Location = new System.Drawing.Point(24, 86);
            this.skinDataGridView1.Name = "skinDataGridView1";
            this.skinDataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.skinDataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.skinDataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.skinDataGridView1.RowTemplate.Height = 23;
            this.skinDataGridView1.Size = new System.Drawing.Size(644, 261);
            this.skinDataGridView1.TabIndex = 44;
            this.skinDataGridView1.TitleBack = null;
            this.skinDataGridView1.TitleBackColorBegin = System.Drawing.Color.White;
            this.skinDataGridView1.TitleBackColorEnd = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(196)))), ((int)(((byte)(242)))));
            this.skinDataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.skinDataGridView1_CellClick);
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "fileName";
            this.Column1.HeaderText = "文件名";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "tag";
            this.Column2.HeaderText = "标签";
            this.Column2.Name = "Column2";
            // 
            // Column3
            // 
            this.Column3.DataPropertyName = "describe";
            this.Column3.HeaderText = "描述";
            this.Column3.Name = "Column3";
            // 
            // Column4
            // 
            this.Column4.DataPropertyName = "createTime";
            this.Column4.HeaderText = "创建时间";
            this.Column4.Name = "Column4";
            this.Column4.Width = 150;
            // 
            // Column5
            // 
            this.Column5.DataPropertyName = "updateTime";
            this.Column5.HeaderText = "最后修改时间";
            this.Column5.Name = "Column5";
            this.Column5.Width = 150;
            // 
            // FindName
            // 
            this.FindName.BackColor = System.Drawing.Color.Transparent;
            this.FindName.DownBack = null;
            this.FindName.Icon = null;
            this.FindName.IconIsButton = false;
            this.FindName.IconMouseState = CCWin.SkinClass.ControlState.Normal;
            this.FindName.IsPasswordChat = '\0';
            this.FindName.IsSystemPasswordChar = false;
            this.FindName.Lines = new string[0];
            this.FindName.Location = new System.Drawing.Point(341, 383);
            this.FindName.Margin = new System.Windows.Forms.Padding(0);
            this.FindName.MaxLength = 32767;
            this.FindName.MinimumSize = new System.Drawing.Size(28, 28);
            this.FindName.MouseBack = null;
            this.FindName.MouseState = CCWin.SkinClass.ControlState.Normal;
            this.FindName.Multiline = false;
            this.FindName.Name = "FindName";
            this.FindName.NormlBack = null;
            this.FindName.Padding = new System.Windows.Forms.Padding(5);
            this.FindName.ReadOnly = false;
            this.FindName.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.FindName.Size = new System.Drawing.Size(200, 28);
            // 
            // 
            // 
            this.FindName.SkinTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FindName.SkinTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FindName.SkinTxt.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.FindName.SkinTxt.Location = new System.Drawing.Point(5, 5);
            this.FindName.SkinTxt.Name = "BaseText";
            this.FindName.SkinTxt.Size = new System.Drawing.Size(190, 18);
            this.FindName.SkinTxt.TabIndex = 0;
            this.FindName.SkinTxt.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.FindName.SkinTxt.WaterText = "输入匹配字符";
            this.FindName.TabIndex = 43;
            this.FindName.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.FindName.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.FindName.WaterText = "输入匹配字符";
            this.FindName.WordWrap = true;
            // 
            // Btn_Download
            // 
            this.Btn_Download.BackColor = System.Drawing.Color.Transparent;
            this.Btn_Download.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.Btn_Download.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.Btn_Download.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.Btn_Download.DownBack = null;
            this.Btn_Download.DownBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.Btn_Download.ForeColor = System.Drawing.Color.White;
            this.Btn_Download.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.Btn_Download.InheritColor = true;
            this.Btn_Download.InnerBorderColor = System.Drawing.Color.White;
            this.Btn_Download.IsDrawGlass = false;
            this.Btn_Download.Location = new System.Drawing.Point(127, 383);
            this.Btn_Download.MouseBack = null;
            this.Btn_Download.MouseBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.Btn_Download.Name = "Btn_Download";
            this.Btn_Download.NormlBack = null;
            this.Btn_Download.Radius = 20;
            this.Btn_Download.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.Btn_Download.Size = new System.Drawing.Size(97, 28);
            this.Btn_Download.TabIndex = 42;
            this.Btn_Download.Text = "下载";
            this.Btn_Download.UseVisualStyleBackColor = false;
            this.Btn_Download.Click += new System.EventHandler(this.Btn_Download_Click);
            // 
            // Btn_Close
            // 
            this.Btn_Close.BackColor = System.Drawing.Color.Transparent;
            this.Btn_Close.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.Btn_Close.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.Btn_Close.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.Btn_Close.DownBack = null;
            this.Btn_Close.DownBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.Btn_Close.ForeColor = System.Drawing.Color.White;
            this.Btn_Close.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.Btn_Close.InheritColor = true;
            this.Btn_Close.InnerBorderColor = System.Drawing.Color.White;
            this.Btn_Close.IsDrawGlass = false;
            this.Btn_Close.Location = new System.Drawing.Point(571, 383);
            this.Btn_Close.MouseBack = null;
            this.Btn_Close.MouseBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.Btn_Close.Name = "Btn_Close";
            this.Btn_Close.NormlBack = null;
            this.Btn_Close.Radius = 20;
            this.Btn_Close.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.Btn_Close.Size = new System.Drawing.Size(97, 28);
            this.Btn_Close.TabIndex = 41;
            this.Btn_Close.Text = "关闭";
            this.Btn_Close.UseVisualStyleBackColor = false;
            this.Btn_Close.Click += new System.EventHandler(this.Btn_Close_Click);
            // 
            // FrmDownloadFromMes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(698, 429);
            this.Controls.Add(this.Label_DownLoadName);
            this.Controls.Add(this.skinLabel2);
            this.Controls.Add(this.btn_Find);
            this.Controls.Add(this.btn_Flash);
            this.Controls.Add(this.skinDataGridView1);
            this.Controls.Add(this.FindName);
            this.Controls.Add(this.Btn_Download);
            this.Controls.Add(this.Btn_Close);
            this.Name = "FrmDownloadFromMes";
            this.Text = "FrmDownloadFromMes";
            this.Load += new System.EventHandler(this.FrmDownloadFromMes_Load);
            ((System.ComponentModel.ISupportInitialize)(this.skinDataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CCWin.SkinControl.SkinLabel Label_DownLoadName;
        private CCWin.SkinControl.SkinLabel skinLabel2;
        private CCWin.SkinControl.SkinButton btn_Find;
        private CCWin.SkinControl.SkinButton btn_Flash;
        private CCWin.SkinControl.SkinDataGridView skinDataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private CCWin.SkinControl.SkinTextBox FindName;
        private CCWin.SkinControl.SkinButton Btn_Download;
        private CCWin.SkinControl.SkinButton Btn_Close;
    }
}