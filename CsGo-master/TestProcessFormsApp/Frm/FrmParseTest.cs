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
using System.Windows.Shapes;
using System.Xml.XPath;
using TestProcessFormsApp.Public;
//using TestProcessFormsApp.Boards;
using TestProcessFormsApp.测试类;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TestProcessFormsApp.Frm
{
    public partial class FrmParseTest : Skin_VS
    {
        private System.Windows.Forms.ComboBox comboBox = null;
        private System.Windows.Forms.ComboBox comboBox2 = null;
        private System.Windows.Forms.ComboBox comboBox3 = null;
        private System.Windows.Forms.ComboBox comboBox4 = null;
        private System.Windows.Forms.ComboBox comboBox5 = null;
        public FrmParseTest()
        {
           
            InitializeComponent();
            InitComboBox();
            this.dataGridView1.DataSource = WinForm.ParseTestItemsSetting;
        //    this.dataGridView1.DataSource = CsvHelper.parseTestItems;
            //dataGridView1.Columns["判断数据类型"].DefaultCellStyle = typeof(数据类型);
            //dataGridView1.Refresh();
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void InitComboBox()
        {
            comboBox = new System.Windows.Forms.ComboBox();
            this.comboBox.Items.Add("_String");
            this.comboBox.Items.Add("_int");
            this.comboBox.Items.Add("_float");
            this.comboBox.Leave += new EventHandler(ComboBox_Leave);
            this.comboBox.SelectedIndexChanged += new EventHandler(ComboBox_TextChanged);
            this.comboBox.Visible = false;
            this.comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            this.dataGridView1.Controls.Add(this.comboBox);

            //comboBox2 = new System.Windows.Forms.ComboBox();
            //this.comboBox2.Items.Add("CAEA008");
            //this.comboBox2.Items.Add("CAEAAT03");
            //this.comboBox2.Leave += new EventHandler(ComboBox2_Leave);
            //this.comboBox2.SelectedIndexChanged += new EventHandler(ComboBox2_TextChanged);
            //this.comboBox2.Visible = false;
            //this.comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;

            //this.dataGridView1.Controls.Add(this.comboBox2);


            comboBox3 = new System.Windows.Forms.ComboBox();
            this.comboBox3.Items.Add("全部");
            this.comboBox3.Items.Add("范围");
            this.comboBox3.Items.Add("等于");
            this.comboBox3.Items.Add("不等于");
            this.comboBox3.Items.Add("大于");
            this.comboBox3.Items.Add("大于或等于");
            this.comboBox3.Items.Add("小于");
            this.comboBox3.Items.Add("小于或等于");
            this.comboBox3.Leave += new EventHandler(ComboBox3_Leave);
            this.comboBox3.SelectedIndexChanged += new EventHandler(ComboBox3_TextChanged);
            this.comboBox3.Visible = false;
            this.comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;

            this.dataGridView1.Controls.Add(this.comboBox3);

            comboBox4 = new System.Windows.Forms.ComboBox();
            this.comboBox4.Items.Add("常开");
            this.comboBox4.Items.Add("Case");
            
            this.comboBox4.Leave += new EventHandler(ComboBox4_Leave);
            this.comboBox4.SelectedIndexChanged += new EventHandler(ComboBox4_TextChanged);
            this.comboBox4.Visible = false;
            this.comboBox4.DropDownStyle = ComboBoxStyle.DropDownList;

            this.dataGridView1.Controls.Add(this.comboBox4);
            comboBox5 = new System.Windows.Forms.ComboBox();
            this.comboBox5.Items.Add("OFF");
            this.comboBox5.Items.Add("ON");
            this.comboBox5.Items.Add("正转");
            this.comboBox5.Items.Add("反转");
            this.comboBox5.Items.Add("空");
            this.comboBox5.Leave += new EventHandler(ComboBox5_Leave);
            this.comboBox5.SelectedIndexChanged += new EventHandler(ComboBox5_TextChanged);
            this.comboBox5.Visible = false;
            this.comboBox5.DropDownStyle = ComboBoxStyle.DropDownList;

            this.dataGridView1.Controls.Add(this.comboBox5);
        }
        private void ComboBox_TextChanged(object sender, EventArgs e)
        {
            this.dataGridView1.CurrentCell.Value = ((System.Windows.Forms.ComboBox)sender).Text;
            this.comboBox.Visible = false;
        }

        private void ComboBox_Leave(object sender, EventArgs e)
        {
            this.comboBox.Visible = false;
        }
        //private void ComboBox2_TextChanged(object sender, EventArgs e)
        //{
        //    this.dataGridView1.CurrentCell.Value = ((System.Windows.Forms.ComboBox)sender).Text;
        //    this.comboBox2.Visible = false;
        //}

        //private void ComboBox2_Leave(object sender, EventArgs e)
        //{
        //    this.comboBox2.Visible = false;
        //}
        private void ComboBox3_TextChanged(object sender, EventArgs e)
        {
            this.dataGridView1.CurrentCell.Value = ((System.Windows.Forms.ComboBox)sender).Text;

            this.comboBox3.Visible = false;
        }

        private void ComboBox3_Leave(object sender, EventArgs e)
        {
            this.comboBox3.Visible = false;
        }
        private void ComboBox4_TextChanged(object sender, EventArgs e)
        {
            this.dataGridView1.CurrentCell.Value = ((System.Windows.Forms.ComboBox)sender).Text;

            this.comboBox4.Visible = false;
        }

        private void ComboBox4_Leave(object sender, EventArgs e)
        {
            this.comboBox4.Visible = false;
        }
        private void ComboBox5_TextChanged(object sender, EventArgs e)
        {
            this.dataGridView1.CurrentCell.Value = ((System.Windows.Forms.ComboBox)sender).Text;

            this.comboBox5.Visible = false;
        }

        private void ComboBox5_Leave(object sender, EventArgs e)
        {
            this.comboBox5.Visible = false;
        }
        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.dataGridView1.SelectedCells[0].OwningColumn.Name == "判断数据类型")
                {
                    System.Drawing.Rectangle rectangle = dataGridView1.GetCellDisplayRectangle(dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex, false);
                    string value = dataGridView1.CurrentCell.Value.ToString();
                    this.comboBox.Text = value;
                    this.comboBox.Left = rectangle.Left;
                    this.comboBox.Top = rectangle.Top;
                    this.comboBox.Width = rectangle.Width;
                    this.comboBox.Height = rectangle.Height;
                    this.comboBox.Visible = true;
                }
                //else if (this.dataGridView1.SelectedCells[0].OwningColumn.Name == "测试数据来源")
                //{
                //    Rectangle rectangle = dataGridView1.GetCellDisplayRectangle(dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex, false);
                //    string value = dataGridView1.CurrentCell.Value.ToString();
                //    this.comboBox2.Text = value;
                //    this.comboBox2.Left = rectangle.Left;
                //    this.comboBox2.Top = rectangle.Top;
                //    this.comboBox2.Width = rectangle.Width;
                //    this.comboBox2.Height = rectangle.Height;
                //    this.comboBox2.Visible = true;
                //}
                else if (this.dataGridView1.SelectedCells[0].OwningColumn.Name == "测试结果对比规则")
                {
                    System.Drawing.Rectangle rectangle = dataGridView1.GetCellDisplayRectangle(dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex, false);
                    string value = dataGridView1.CurrentCell.Value.ToString();
                    this.comboBox3.Text = value;
                    this.comboBox3.Left = rectangle.Left;
                    this.comboBox3.Top = rectangle.Top;
                    this.comboBox3.Width = rectangle.Width;
                    this.comboBox3.Height = rectangle.Height;
                    this.comboBox3.Visible = true;
                }
                //else if (this.dataGridView1.SelectedCells[0].OwningColumn.Name == "测试时序")
                //{
                //    Rectangle rectangle = dataGridView1.GetCellDisplayRectangle(dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex, false);
                //    string value = dataGridView1.CurrentCell.Value.ToString();
                //    this.comboBox4.Text = value;
                //    this.comboBox4.Left = rectangle.Left;
                //    this.comboBox4.Top = rectangle.Top;
                //    this.comboBox4.Width = rectangle.Width;
                //    this.comboBox4.Height = rectangle.Height;
                //    this.comboBox4.Visible = true;
                //}
                //else if (this.dataGridView1.SelectedCells[0].OwningColumn.Name == "Case1" ||
                //    this.dataGridView1.SelectedCells[0].OwningColumn.Name == "Case2" ||
                //    this.dataGridView1.SelectedCells[0].OwningColumn.Name == "Case3" ||
                //    this.dataGridView1.SelectedCells[0].OwningColumn.Name == "Case4" )
                //{
                //    Rectangle rectangle = dataGridView1.GetCellDisplayRectangle(dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex, false);
                //    string value = dataGridView1.CurrentCell.Value.ToString();
                //    this.comboBox5.Text = value;
                //    this.comboBox5.Left = rectangle.Left;
                //    this.comboBox5.Top = rectangle.Top;
                //    this.comboBox5.Width = rectangle.Width;
                //    this.comboBox5.Height = rectangle.Height;
                //    this.comboBox5.Visible = true;
                //}
                else
                {
                    this.comboBox.Visible = false;
                    //this.comboBox2.Visible = false;
                    this.comboBox3.Visible = false;
                    this.comboBox4.Visible = false;
                    this.comboBox5.Visible = false;
                }
            }
            catch (Exception ex)
            {
             //   return;
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            CaseConfig.测试项排序();
            try
            {
                CsvHelper.ExportToCsv(WinForm.ParseTestItemsSetting, $"{LocalSetting.SystemDIR}\\产品测试项配置\\{WinForm.产品型号}.csv");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void Btn_Delete_Click(object sender, EventArgs e)
        {
            // 检查是否有选中的行
            if (selectedRowIndex >= 0)
            {
                DialogResult result = MessageBox.Show("确定要删除这行数据吗？", "删除确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    BindingList<ParseTestItems> bindingList = (BindingList<ParseTestItems>)dataGridView1.DataSource;

                    // 检查BindingList是否包含选中的行索引
                    if (bindingList.Count > selectedRowIndex)
                    {
                        // 从BindingList中删除选中行的数据
                        bindingList.RemoveAt(selectedRowIndex);

                        // 重置选中的行索引
                        selectedRowIndex = -1;
                    }
                }
                    // 获取BindingList数据源
            }
            else
            {
                MessageBox.Show("请先选择要删除的数据行。");
            }
        }
        // 定义一个变量来存储选中的行索引
        private int selectedRowIndex = -1;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // 确保用户点击的是单元格，而不是行头或列头
            if (e.RowIndex >= 0)
            {
                // 记录选中的行索引
                selectedRowIndex = e.RowIndex;
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            ParseTestItems temp = new ParseTestItems() { 测试项名称 = "1", 测试结果对比规则 = 对比规则.范围/*, 测试时序 = 时序.常开*/,
                判断数据类型 = 数据类型._int, 判断数据1 = "1", 判断数据2 = 1, 判断数据3 = "1", 测试方法 = "Can读取" };
            WinForm.ParseTestItemsSetting.Add(temp);
        }

        private void UpMove_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell.RowIndex >= 1)
            {
                ParseTestItems temp = new ParseTestItems();
                temp = WinForm.ParseTestItemsSetting[dataGridView1.CurrentCell.RowIndex];
                WinForm.ParseTestItemsSetting[dataGridView1.CurrentCell.RowIndex] = WinForm.ParseTestItemsSetting[dataGridView1.CurrentCell.RowIndex - 1];
                WinForm.ParseTestItemsSetting[dataGridView1.CurrentCell.RowIndex - 1] = temp;
                dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex - 1].Cells[1];
            }
        }

        private void DownMove_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell.RowIndex < WinForm.ParseTestItemsSetting.Count - 1)
            {
                ParseTestItems temp = new ParseTestItems();
                temp = WinForm.ParseTestItemsSetting[dataGridView1.CurrentCell.RowIndex];
                WinForm.ParseTestItemsSetting[dataGridView1.CurrentCell.RowIndex] = WinForm.ParseTestItemsSetting[dataGridView1.CurrentCell.RowIndex + 1];
                WinForm.ParseTestItemsSetting[dataGridView1.CurrentCell.RowIndex + 1] = temp;
                int index = dataGridView1.CurrentCell.RowIndex;
                dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex + 1].Cells[1];
            }
        }

        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            e.Row.HeaderCell.Value = string.Format("{0}", e.Row.Index + 1);
        }
    }
}
