using CCWin;
using CCWin.SkinControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JLRScan.Frm
{
    public partial class FrmNewText : Skin_VS
    {
        public FrmNewText()
        {
            InitializeComponent();
        }
        public string saveName { get;set; }
        private void btn_Save_Click(object sender, EventArgs e)
        {
            if( !Tb_ModelName.Text.IsNullOrEmpty() )
            {
                saveName = Tb_ModelName.Text.ToUpper();
                Close();
            }
          
        }

        private void skinButton1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
