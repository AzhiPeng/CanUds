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
    public partial class FrmUploadMes : Skin_VS
    {
        public FrmUploadMes()
        {
            InitializeComponent();
        }

        private void Btn_UploadAll_Click(object sender, EventArgs e)
        {
            string path = System.Environment.CurrentDirectory;
            try
            {
                string[] jsonFiles = Directory.GetFiles(path + @"\" + @"产品测试项配置", "*.csv");

                foreach (string jsonFile in jsonFiles)
                {
                    //    string fileName = Path.GetFileName(jsonFile);
                    byte[] bs = System.IO.File.ReadAllBytes(jsonFile);
                    WebServer.UploadFile(Path.GetFileName(jsonFile), bs, "csv");
                }
                MessageBox.Show("上传成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Btn_Upload_Click(object sender, EventArgs e)
        {
            
            if (Cbb_Model.SelectedIndex == -1)
            {
                MessageBox.Show("请选择上传文件");
                return;
            }
            try
            {
                string path = System.Environment.CurrentDirectory;
                LogHelper.Info("上传配置：" + path + @"\" + @"产品测试项配置\" + Cbb_Model.Text);
                byte[] bs = System.IO.File.ReadAllBytes(path + @"\" + @"产品测试项配置\" + Cbb_Model.Text);
                WebServer.UploadFile(Cbb_Model.Text, bs, "csv");
                MessageBox.Show("上传成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Btn_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FrmUploadMes_Load(object sender, EventArgs e)
        {
            string path = System.Environment.CurrentDirectory;
            try
            {
                string[] jsonFiles = Directory.GetFiles(path + @"\" + @"产品测试项配置", "*.csv");

                foreach (string jsonFile in jsonFiles)
                {
                    string fileName = Path.GetFileName(jsonFile);
                    Cbb_Model.Items.Add(fileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
