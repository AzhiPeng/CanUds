using CCWin;
using Newtonsoft.Json;
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
using TestProcessFormsApp.Public;

namespace JLRScan.Frm
{
    public partial class FrmLocalSetting : Skin_VS
    {
        public FrmLocalSetting()
        {
            InitializeComponent();
        }

        private void FrmLocalSetting_Load(object sender, EventArgs e)
        {
            LocalSettingPropertyGrid.SelectedObject = LocalSetting.localSetting;
        }

        private void btn_Save_Click(object sender, EventArgs e)
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

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
