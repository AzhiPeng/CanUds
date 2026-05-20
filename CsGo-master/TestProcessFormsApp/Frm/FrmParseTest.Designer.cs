namespace TestProcessFormsApp.Frm
{
    partial class FrmParseTest
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.DownMove = new CCWin.SkinControl.SkinPanel();
            this.UpMove = new CCWin.SkinControl.SkinPanel();
            this.AddButton = new CCWin.SkinControl.SkinButton();
            this.btn_close = new CCWin.SkinControl.SkinButton();
            this.btn_Save = new CCWin.SkinControl.SkinButton();
            this.Btn_Delete = new CCWin.SkinControl.SkinButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(8, 39);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.DownMove);
            this.splitContainer1.Panel2.Controls.Add(this.UpMove);
            this.splitContainer1.Panel2.Controls.Add(this.AddButton);
            this.splitContainer1.Panel2.Controls.Add(this.btn_close);
            this.splitContainer1.Panel2.Controls.Add(this.btn_Save);
            this.splitContainer1.Panel2.Controls.Add(this.Btn_Delete);
            this.splitContainer1.Size = new System.Drawing.Size(1454, 490);
            this.splitContainer1.SplitterDistance = 412;
            this.splitContainer1.TabIndex = 0;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 60;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(1454, 412);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CurrentCellChanged += new System.EventHandler(this.dataGridView1_CurrentCellChanged);
            this.dataGridView1.RowStateChanged += new System.Windows.Forms.DataGridViewRowStateChangedEventHandler(this.dataGridView1_RowStateChanged);
            // 
            // DownMove
            // 
            this.DownMove.BackColor = System.Drawing.Color.Transparent;
            this.DownMove.BackgroundImage = global::TestProcessFormsApp.Properties.Resources.Arrow_Down;
            this.DownMove.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.DownMove.Cursor = System.Windows.Forms.Cursors.Hand;
            this.DownMove.DownBack = null;
            this.DownMove.Location = new System.Drawing.Point(73, 5);
            this.DownMove.MouseBack = null;
            this.DownMove.Name = "DownMove";
            this.DownMove.NormlBack = null;
            this.DownMove.Size = new System.Drawing.Size(46, 46);
            this.DownMove.TabIndex = 29;
            this.DownMove.Click += new System.EventHandler(this.DownMove_Click);
            // 
            // UpMove
            // 
            this.UpMove.BackColor = System.Drawing.Color.Transparent;
            this.UpMove.BackgroundImage = global::TestProcessFormsApp.Properties.Resources.Arrow_Up;
            this.UpMove.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.UpMove.Cursor = System.Windows.Forms.Cursors.Hand;
            this.UpMove.DownBack = null;
            this.UpMove.Location = new System.Drawing.Point(12, 5);
            this.UpMove.MouseBack = null;
            this.UpMove.Name = "UpMove";
            this.UpMove.NormlBack = null;
            this.UpMove.Size = new System.Drawing.Size(46, 46);
            this.UpMove.TabIndex = 28;
            this.UpMove.Click += new System.EventHandler(this.UpMove_Click);
            // 
            // AddButton
            // 
            this.AddButton.BackColor = System.Drawing.Color.Transparent;
            this.AddButton.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.AddButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.AddButton.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.AddButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.AddButton.DownBack = null;
            this.AddButton.DownBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.AddButton.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.AddButton.ForeColor = System.Drawing.Color.White;
            this.AddButton.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.AddButton.InheritColor = true;
            this.AddButton.InnerBorderColor = System.Drawing.Color.White;
            this.AddButton.IsDrawGlass = false;
            this.AddButton.Location = new System.Drawing.Point(397, 5);
            this.AddButton.MouseBack = null;
            this.AddButton.MouseBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.AddButton.Name = "AddButton";
            this.AddButton.NormlBack = null;
            this.AddButton.Radius = 20;
            this.AddButton.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.AddButton.Size = new System.Drawing.Size(92, 46);
            this.AddButton.TabIndex = 27;
            this.AddButton.Text = "新增";
            this.AddButton.UseVisualStyleBackColor = false;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // btn_close
            // 
            this.btn_close.BackColor = System.Drawing.Color.Transparent;
            this.btn_close.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_close.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_close.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btn_close.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_close.DownBack = null;
            this.btn_close.DownBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_close.Font = new System.Drawing.Font("宋体", 10.5F);
            this.btn_close.ForeColor = System.Drawing.Color.White;
            this.btn_close.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_close.InheritColor = true;
            this.btn_close.InnerBorderColor = System.Drawing.Color.White;
            this.btn_close.IsDrawGlass = false;
            this.btn_close.Location = new System.Drawing.Point(806, 5);
            this.btn_close.MouseBack = null;
            this.btn_close.MouseBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_close.Name = "btn_close";
            this.btn_close.NormlBack = null;
            this.btn_close.Radius = 20;
            this.btn_close.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btn_close.Size = new System.Drawing.Size(102, 46);
            this.btn_close.TabIndex = 26;
            this.btn_close.Text = "关闭";
            this.btn_close.UseVisualStyleBackColor = false;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.BackColor = System.Drawing.Color.Transparent;
            this.btn_Save.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_Save.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_Save.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btn_Save.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Save.DownBack = null;
            this.btn_Save.DownBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_Save.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Save.ForeColor = System.Drawing.Color.White;
            this.btn_Save.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_Save.InheritColor = true;
            this.btn_Save.InnerBorderColor = System.Drawing.Color.White;
            this.btn_Save.IsDrawGlass = false;
            this.btn_Save.Location = new System.Drawing.Point(207, 5);
            this.btn_Save.MouseBack = null;
            this.btn_Save.MouseBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.NormlBack = null;
            this.btn_Save.Radius = 20;
            this.btn_Save.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btn_Save.Size = new System.Drawing.Size(92, 46);
            this.btn_Save.TabIndex = 22;
            this.btn_Save.Text = "保存";
            this.btn_Save.UseVisualStyleBackColor = false;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // Btn_Delete
            // 
            this.Btn_Delete.BackColor = System.Drawing.Color.Transparent;
            this.Btn_Delete.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.Btn_Delete.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.Btn_Delete.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.Btn_Delete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Btn_Delete.DownBack = null;
            this.Btn_Delete.DownBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.Btn_Delete.Font = new System.Drawing.Font("宋体", 10.5F);
            this.Btn_Delete.ForeColor = System.Drawing.Color.White;
            this.Btn_Delete.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.Btn_Delete.InheritColor = true;
            this.Btn_Delete.InnerBorderColor = System.Drawing.Color.White;
            this.Btn_Delete.IsDrawGlass = false;
            this.Btn_Delete.Location = new System.Drawing.Point(589, 5);
            this.Btn_Delete.MouseBack = null;
            this.Btn_Delete.MouseBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.Btn_Delete.Name = "Btn_Delete";
            this.Btn_Delete.NormlBack = null;
            this.Btn_Delete.Radius = 20;
            this.Btn_Delete.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.Btn_Delete.Size = new System.Drawing.Size(102, 46);
            this.Btn_Delete.TabIndex = 24;
            this.Btn_Delete.Text = "删除";
            this.Btn_Delete.UseVisualStyleBackColor = false;
            this.Btn_Delete.Click += new System.EventHandler(this.Btn_Delete_Click);
            // 
            // FrmParseTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1470, 537);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FrmParseTest";
            this.Text = "FrmParseTest";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private CCWin.SkinControl.SkinButton btn_close;
        private CCWin.SkinControl.SkinButton btn_Save;
        private CCWin.SkinControl.SkinButton Btn_Delete;
        private CCWin.SkinControl.SkinButton AddButton;
        private CCWin.SkinControl.SkinPanel DownMove;
        private CCWin.SkinControl.SkinPanel UpMove;
    }
}