using CCWin;
using HMIPLC;
using JLRScan.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestProcessFormsApp.Frm
{
    public partial class FrmUploadJsonToMes : Skin_VS
    {
        public FrmUploadJsonToMes()
        {
            InitializeComponent();
        }

        private void FrmUploadJsonToMes_Load(object sender, EventArgs e)
        {
            string path = System.Environment.CurrentDirectory;
            try
            {
                string[] searchPatterns = { "*.btw", "*.frx" };
                foreach (string searchPattern in searchPatterns)
                {
                    string[] files = Directory.GetFiles(path + @"\" + @"FrxBtw", searchPattern);
                    foreach (string jsonFile in files)
                    {
                        string fileName = Path.GetFileName(jsonFile);
                        Cbb_FileName.Items.Add(fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_Upload_Click(object sender, EventArgs e)
        {
            if (Cbb_FileName.SelectedIndex == -1)
            {
                MessageBox.Show("请选择上传文件");
                return;
            }
            try
            {
                string path = System.Environment.CurrentDirectory;
                LogHelper.Info("上传配置：" + path + @"\" + @"FrxBtw\" + Cbb_FileName.Text);
                byte[] bs = System.IO.File.ReadAllBytes(path + @"\" + @"FrxBtw\" + Cbb_FileName.Text);
                WebServer.UploadFile(Cbb_FileName.Text, bs, "FrxBtw");
                MessageBox.Show("上传成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
