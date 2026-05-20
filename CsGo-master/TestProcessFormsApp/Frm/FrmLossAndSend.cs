using CCWin;
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
    public partial class FrmLossAndSend : Skin_VS
    {
        public FrmLossAndSend()
        {
            InitializeComponent();
        }

        private void btn_chanel_Click(object sender, EventArgs e)
        {
            try
            {
                //WinForm.上轮发包数 = Convert.ToInt32(Text_发包数.Text);
                //WinForm.上轮天线1丢包数 = Convert.ToInt32(Text_丢包数1.Text);
                //WinForm.上轮天线2丢包数 = Convert.ToInt32(Text_丢包数2.Text);
                //WinForm.上轮天线3丢包数 = Convert.ToInt32(Text_丢包数3.Text);
                //WinForm.上轮天线4丢包数 = Convert.ToInt32(Text_丢包数4.Text);
                //WinForm.上轮天线5丢包数 = Convert.ToInt32(Text_丢包数5.Text);
                //WinForm.上轮天线6丢包数 = Convert.ToInt32(Text_丢包数6.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
         
        }
    }
}
