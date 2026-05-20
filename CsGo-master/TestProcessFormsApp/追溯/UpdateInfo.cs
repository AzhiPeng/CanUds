using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace HMI
{
    [Serializable]
    public class UpdateInfo
    {
        public enum GW
        {
            工位115,
            工位120,
            工位125,
            工位130,
            工位135,
            工位140
        }

        public UpdateInfo() { }
        [Category("update"), Description("上传字段列表")]
        public List<ColumnInfo> InfoList { get; set; }
        //[Category("update"), Description("工位号")]
        //public GW CodeNo { get; set; }

    }
    [Serializable]
    public class ColumnInfo
    {
        public string CodeNo = "";
        public ColumnInfo() { }
        [Category("ColumnInfo"), Description("control")]
        public string ControlName { get; set; }
        [Category("ColumnInfo"), Description("控件提示")]
        public string Tips { get; set; }
        [Category("ColumnInfo"), Description("上传字段")]
        public string Column { set; get; }
        public bool UpdateFlag { get; set; }
        public override string ToString()
        {
            return Tips;
        }
    }

}
