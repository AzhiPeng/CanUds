using CCWin;
using HMIPLC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestProcessFormsApp.Frm
{
    public partial class FrmDownloadJsonFromMes :Skin_VS
    {
        public FrmDownloadJsonFromMes()
        {
            InitializeComponent();
        }
        GetFileListData[] info;
        public static GetFileListData[] infolist;
        internal string mode;
        private void FrmDownloadJsonFromMes_Load(object sender, EventArgs e)
        {
            info = WebServer.GetFIleList();
            var lst = new List<GetFileListData>();
            for (int i = 0; i < info.Length; i++)
            {
                if (info[i].remark == "FrxBtw")
                    lst.Add(info[i]);
            }
            info = lst.ToArray();
            Array.Sort(info);
            skinDataGridView1.DataSource = info;
            infolist = info;
        }

        private void btn_Flash_Click(object sender, EventArgs e)
        {
            try
            {
                skinDataGridView1.DataSource = infolist;
                FindName.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Btn_Download_Click(object sender, EventArgs e)
        {
            if (Label_DownLoadName.Text == "Name")
            {
                MessageBox.Show("请选择下载文件");
                return;
            }
            var itm = infolist.FirstOrDefault(data => data.fileName == Label_DownLoadName.Text);
            if (itm != null)
            {
                if (mode != null)
                    MessageBox.Show(itm.remark);
                else
                {
                    var re = WebServer.DownLoadFile(itm.id);
                    if (mode != null && re.Length < 500)
                    {
                        MessageBox.Show(Encoding.ASCII.GetString(re.GetBuffer(), 0, (int)re.Length));
                    }
                    else
                    {
                        string path = System.Environment.CurrentDirectory;
                        SaveFileDialog sf = new SaveFileDialog();
                        sf.InitialDirectory = path + @"\" + @"FrxBtw";
                        sf.FileName = itm.fileName;
                        if (sf.ShowDialog() == DialogResult.OK)
                        {
                            System.IO.File.WriteAllBytes(sf.FileName, re.ToArray());

                            MessageBox.Show("保存成功：" + sf.FileName);
                            //Close();
                        }
                    }
                }
            }
        }

        private void btn_Find_Click(object sender, EventArgs e)
        {
            try
            {
                skinDataGridView1.DataSource = infolist.Where(p => p.fileName.Contains(FindName.Text)).ToList();
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

        private void skinDataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (skinDataGridView1.SelectedCells.Count > 0)
            {
                int rowIndex = skinDataGridView1.SelectedCells[0].RowIndex;
                Label_DownLoadName.Text = skinDataGridView1.Rows[rowIndex].Cells["Column1"].Value.ToString();
            }
        }
    }
}
