namespace TestProcessFormsApp.Frm
{
    partial class FrmUploadJsonToMes
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
            this.label2 = new System.Windows.Forms.Label();
            this.Cbb_FileName = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Close = new CCWin.SkinControl.SkinButton();
            this.btn_Upload = new CCWin.SkinControl.SkinButton();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(119, 124);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(234, 21);
            this.label2.TabIndex = 42;
            this.label2.Text = "下拉框中为已经存在的模板名称";
            // 
            // Cbb_FileName
            // 
            this.Cbb_FileName.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Cbb_FileName.FormattingEnabled = true;
            this.Cbb_FileName.Location = new System.Drawing.Point(91, 83);
            this.Cbb_FileName.Name = "Cbb_FileName";
            this.Cbb_FileName.Size = new System.Drawing.Size(348, 29);
            this.Cbb_FileName.TabIndex = 41;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(32, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 40;
            this.label1.Text = "模板名称";
            // 
            // btn_Close
            // 
            this.btn_Close.BackColor = System.Drawing.Color.Transparent;
            this.btn_Close.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_Close.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_Close.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btn_Close.DownBack = null;
            this.btn_Close.DownBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_Close.ForeColor = System.Drawing.Color.White;
            this.btn_Close.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_Close.InheritColor = true;
            this.btn_Close.InnerBorderColor = System.Drawing.Color.White;
            this.btn_Close.IsDrawGlass = false;
            this.btn_Close.Location = new System.Drawing.Point(578, 83);
            this.btn_Close.MouseBack = null;
            this.btn_Close.MouseBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.NormlBack = null;
            this.btn_Close.Radius = 20;
            this.btn_Close.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btn_Close.Size = new System.Drawing.Size(99, 28);
            this.btn_Close.TabIndex = 39;
            this.btn_Close.Text = "关闭";
            this.btn_Close.UseVisualStyleBackColor = false;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // btn_Upload
            // 
            this.btn_Upload.BackColor = System.Drawing.Color.Transparent;
            this.btn_Upload.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_Upload.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_Upload.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btn_Upload.DownBack = null;
            this.btn_Upload.DownBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_Upload.ForeColor = System.Drawing.Color.White;
            this.btn_Upload.GlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(56)))));
            this.btn_Upload.InheritColor = true;
            this.btn_Upload.InnerBorderColor = System.Drawing.Color.White;
            this.btn_Upload.IsDrawGlass = false;
            this.btn_Upload.Location = new System.Drawing.Point(460, 83);
            this.btn_Upload.MouseBack = null;
            this.btn_Upload.MouseBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(125)))), ((int)(((byte)(139)))));
            this.btn_Upload.Name = "btn_Upload";
            this.btn_Upload.NormlBack = null;
            this.btn_Upload.Radius = 20;
            this.btn_Upload.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btn_Upload.Size = new System.Drawing.Size(99, 28);
            this.btn_Upload.TabIndex = 38;
            this.btn_Upload.Text = "上传MES";
            this.btn_Upload.UseVisualStyleBackColor = false;
            this.btn_Upload.Click += new System.EventHandler(this.btn_Upload_Click);
            // 
            // FrmUploadJsonToMes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(717, 165);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Cbb_FileName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.btn_Upload);
            this.Name = "FrmUploadJsonToMes";
            this.Text = "FrmUploadJsonToMes";
            this.Load += new System.EventHandler(this.FrmUploadJsonToMes_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox Cbb_FileName;
        private System.Windows.Forms.Label label1;
        private CCWin.SkinControl.SkinButton btn_Close;
        private CCWin.SkinControl.SkinButton btn_Upload;
    }
}