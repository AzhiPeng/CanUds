using CCWin;
using Otp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace TestProcessFormsApp.Frm
{
    public partial class FrmLogin : Skin_VS
    {
        public FrmLogin()
        {
            InitializeComponent();
        }
        const string key = "kjdsfjioasmnoger89439JK45320O342KLFDGASW342309OBFDOBnjdsaf";
        public static int TestNum = 0;
        private void btn_授权_Click(object sender, EventArgs e)
        {
            Totp otp = new Totp(Encoding.UTF8.GetBytes(key), 60 * 60 * 24);
            string temp = otp.ComputeTotp();
            if (textBox1.Text != temp && textBox1.Text.ToUpper() != temp)
            {
                MessageBox.Show("密码错误！");
                return;
            }
            else
            {
                TestNum++;
                this.Close();
            }
                
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
