namespace JLRScan.Frm
{
    partial class FrmNewText
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
            this.label1 = new System.Windows.Forms.Label();
            this.Tb_ModelName = new CCWin.SkinControl.SkinTextBox();
            this.btn_Save = new CCWin.SkinControl.SkinButton();
            this.skinButton1 = new CCWin.SkinControl.SkinButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(81, 84);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 18;
            this.label1.Text = "板子名称";
            // 
            // Tb_ModelName
            // 
            this.Tb_ModelName.BackColor = System.Drawing.Color.Transparent;
            this.Tb_ModelName.DownBack = null;
            this.Tb_ModelName.Icon = null;
            this.Tb_ModelName.IconIsButton = false;
            this.Tb_ModelName.IconMouseState = CCWin.SkinClass.ControlState.Normal;
            this.Tb_ModelName.IsPasswordChat = '\0';
            this.Tb_ModelName.IsSystemPasswordChar = false;
            this.Tb_ModelName.Lines = new string[0];
            this.Tb_ModelName.Location = new System.Drawing.Point(156, 76);
            this.Tb_ModelName.Margin = new System.Windows.Forms.Padding(0);
            this.Tb_ModelName.MaxLength = 32767;
            this.Tb_ModelName.MinimumSize = new System.Drawing.Size(28, 28);
            this.Tb_ModelName.MouseBack = null;
            this.Tb_ModelName.MouseState = CCWin.SkinClass.ControlState.Normal;
            this.Tb_ModelName.Multiline = false;
            this.Tb_ModelName.Name = "Tb_ModelName";
            this.Tb_ModelName.NormlBack = null;
            this.Tb_ModelName.Padding = new System.Windows.Forms.Padding(5);
            this.Tb_ModelName.ReadOnly = false;
            this.Tb_ModelName.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.Tb_ModelName.Size = new System.Drawing.Size(242, 28);
            // 
            // 
            // 
            this.Tb_ModelName.SkinTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Tb_ModelName.SkinTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tb_ModelName.SkinTxt.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.Tb_ModelName.SkinTxt.Location = new System.Drawing.Point(5, 5);
            this.Tb_ModelName.SkinTxt.Name = "BaseText";
            this.Tb_ModelName.SkinTxt.Size = new System.Drawing.Size(232, 18);
            this.Tb_ModelName.SkinTxt.TabIndex = 0;
            this.Tb_ModelName.SkinTxt.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.Tb_ModelName.SkinTxt.WaterText = "型号名称";
            this.Tb_ModelName.TabIndex = 17;
            this.Tb_ModelName.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.Tb_ModelName.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.Tb_ModelName.WaterText = "型号名称";
            this.Tb_ModelName.WordWrap = true;
            // 
            // btn_Save
            // 
            this.btn_Save.BackColor = System.Drawing.Color.Transparent;
            this.btn_Save.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_Save.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_Save.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btn_Save.DownBack = null;
            this.btn_Save.DownBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_Save.ForeColor = System.Drawing.Color.White;
            this.btn_Save.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_Save.InheritColor = true;
            this.btn_Save.InnerBorderColor = System.Drawing.Color.White;
            this.btn_Save.IsDrawGlass = false;
            this.btn_Save.Location = new System.Drawing.Point(156, 122);
            this.btn_Save.MouseBack = null;
            this.btn_Save.MouseBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.NormlBack = null;
            this.btn_Save.Radius = 20;
            this.btn_Save.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btn_Save.Size = new System.Drawing.Size(97, 28);
            this.btn_Save.TabIndex = 19;
            this.btn_Save.Text = "确定";
            this.btn_Save.UseVisualStyleBackColor = false;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // skinButton1
            // 
            this.skinButton1.BackColor = System.Drawing.Color.Transparent;
            this.skinButton1.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.skinButton1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.skinButton1.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinButton1.DownBack = null;
            this.skinButton1.DownBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.skinButton1.ForeColor = System.Drawing.Color.White;
            this.skinButton1.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.skinButton1.InheritColor = true;
            this.skinButton1.InnerBorderColor = System.Drawing.Color.White;
            this.skinButton1.IsDrawGlass = false;
            this.skinButton1.Location = new System.Drawing.Point(290, 122);
            this.skinButton1.MouseBack = null;
            this.skinButton1.MouseBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.skinButton1.Name = "skinButton1";
            this.skinButton1.NormlBack = null;
            this.skinButton1.Radius = 20;
            this.skinButton1.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.skinButton1.Size = new System.Drawing.Size(97, 28);
            this.skinButton1.TabIndex = 20;
            this.skinButton1.Text = "关闭";
            this.skinButton1.UseVisualStyleBackColor = false;
            this.skinButton1.Click += new System.EventHandler(this.skinButton1_Click);
            // 
            // FrmNewText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 181);
            this.Controls.Add(this.skinButton1);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Tb_ModelName);
            this.Name = "FrmNewText";
            this.Text = "FrmNewText";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private CCWin.SkinControl.SkinTextBox Tb_ModelName;
        private CCWin.SkinControl.SkinButton btn_Save;
        private CCWin.SkinControl.SkinButton skinButton1;
    }
}