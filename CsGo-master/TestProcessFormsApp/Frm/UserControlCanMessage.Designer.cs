namespace TestProcessFormsApp.Frm
{
    partial class UserControlCanMessage
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
            this.lvMessages = new System.Windows.Forms.ListView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btn_clear = new CCWin.SkinControl.SkinButton();
            this.btn_start = new CCWin.SkinControl.SkinButton();
            this.btn_close = new CCWin.SkinControl.SkinButton();
            this.btn_Stop = new CCWin.SkinControl.SkinButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvMessages
            // 
            this.lvMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvMessages.HideSelection = false;
            this.lvMessages.Location = new System.Drawing.Point(0, 0);
            this.lvMessages.Name = "lvMessages";
            this.lvMessages.Size = new System.Drawing.Size(1188, 427);
            this.lvMessages.TabIndex = 1;
            this.lvMessages.UseCompatibleStateImageBehavior = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lvMessages);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btn_clear);
            this.splitContainer1.Panel2.Controls.Add(this.btn_start);
            this.splitContainer1.Panel2.Controls.Add(this.btn_close);
            this.splitContainer1.Panel2.Controls.Add(this.btn_Stop);
            this.splitContainer1.Size = new System.Drawing.Size(1188, 477);
            this.splitContainer1.SplitterDistance = 427;
            this.splitContainer1.TabIndex = 2;
            // 
            // btn_clear
            // 
            this.btn_clear.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btn_clear.BackColor = System.Drawing.Color.Transparent;
            this.btn_clear.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_clear.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_clear.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btn_clear.DownBack = null;
            this.btn_clear.DownBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_clear.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_clear.ForeColor = System.Drawing.Color.Transparent;
            this.btn_clear.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_clear.InheritColor = true;
            this.btn_clear.InnerBorderColor = System.Drawing.Color.White;
            this.btn_clear.IsDrawGlass = false;
            this.btn_clear.Location = new System.Drawing.Point(296, 8);
            this.btn_clear.MouseBack = null;
            this.btn_clear.MouseBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.NormlBack = null;
            this.btn_clear.Radius = 20;
            this.btn_clear.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btn_clear.Size = new System.Drawing.Size(89, 38);
            this.btn_clear.TabIndex = 40;
            this.btn_clear.Text = "清空";
            this.btn_clear.UseVisualStyleBackColor = false;
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // btn_start
            // 
            this.btn_start.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btn_start.BackColor = System.Drawing.Color.Transparent;
            this.btn_start.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_start.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_start.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btn_start.DownBack = null;
            this.btn_start.DownBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_start.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_start.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_start.ForeColor = System.Drawing.Color.Transparent;
            this.btn_start.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_start.InheritColor = true;
            this.btn_start.InnerBorderColor = System.Drawing.Color.White;
            this.btn_start.IsDrawGlass = false;
            this.btn_start.Location = new System.Drawing.Point(11, 8);
            this.btn_start.MouseBack = null;
            this.btn_start.MouseBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_start.Name = "btn_start";
            this.btn_start.NormlBack = null;
            this.btn_start.Radius = 20;
            this.btn_start.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btn_start.Size = new System.Drawing.Size(89, 38);
            this.btn_start.TabIndex = 39;
            this.btn_start.Text = "开始";
            this.btn_start.UseVisualStyleBackColor = false;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // btn_close
            // 
            this.btn_close.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btn_close.BackColor = System.Drawing.Color.Transparent;
            this.btn_close.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_close.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_close.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btn_close.DownBack = null;
            this.btn_close.DownBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_close.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_close.ForeColor = System.Drawing.Color.Transparent;
            this.btn_close.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_close.InheritColor = true;
            this.btn_close.InnerBorderColor = System.Drawing.Color.White;
            this.btn_close.IsDrawGlass = false;
            this.btn_close.Location = new System.Drawing.Point(201, 8);
            this.btn_close.MouseBack = null;
            this.btn_close.MouseBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_close.Name = "btn_close";
            this.btn_close.NormlBack = null;
            this.btn_close.Radius = 20;
            this.btn_close.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btn_close.Size = new System.Drawing.Size(89, 38);
            this.btn_close.TabIndex = 38;
            this.btn_close.Text = "关闭";
            this.btn_close.UseVisualStyleBackColor = false;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // btn_Stop
            // 
            this.btn_Stop.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btn_Stop.BackColor = System.Drawing.Color.Transparent;
            this.btn_Stop.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_Stop.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_Stop.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btn_Stop.DownBack = null;
            this.btn_Stop.DownBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_Stop.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Stop.ForeColor = System.Drawing.Color.Transparent;
            this.btn_Stop.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_Stop.InheritColor = true;
            this.btn_Stop.InnerBorderColor = System.Drawing.Color.White;
            this.btn_Stop.IsDrawGlass = false;
            this.btn_Stop.Location = new System.Drawing.Point(106, 8);
            this.btn_Stop.MouseBack = null;
            this.btn_Stop.MouseBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_Stop.Name = "btn_Stop";
            this.btn_Stop.NormlBack = null;
            this.btn_Stop.Radius = 20;
            this.btn_Stop.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btn_Stop.Size = new System.Drawing.Size(89, 38);
            this.btn_Stop.TabIndex = 37;
            this.btn_Stop.Text = "暂停";
            this.btn_Stop.UseVisualStyleBackColor = false;
            this.btn_Stop.Click += new System.EventHandler(this.btn_Stop_Click);
            // 
            // UserControlCanMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "UserControlCanMessage";
            this.Size = new System.Drawing.Size(1188, 477);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvMessages;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private CCWin.SkinControl.SkinButton btn_Stop;
        private CCWin.SkinControl.SkinButton btn_close;
        private CCWin.SkinControl.SkinButton btn_start;
        private CCWin.SkinControl.SkinButton btn_clear;

    }
}
