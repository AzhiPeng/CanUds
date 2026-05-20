using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyManageForGm.Model
{
    [Table("产品信息")]
    public class ProductInfoModel : BaseModel
    //public class 产品信息 : BaseModel
    {
        [Column("ord")]
        public override int Id { get; set; }

        public string 追溯码 { get; set; }

        public string 用户图号 { get; set; }

        public string MTC { get; set; }

        public string PCBA码 { get; set; }

        [Column("shell code")]
        public string shell_code { get; set; }

        public string ZJ01标识 { get; set; }

        public string 批次号 { get; set; }

        public string 工位号 { get; set; }

        public string 操作人 { get; set; }

        public DateTime? 操作时间 { get; set; }

        public DateTime? 老化箱出口时间 { get; set; }

        public string 包装标识 { get; set; }

        public string 一次性 { get; set; }

        public string 有效标识 { get; set; }

        public string 上传标识 { get; set; }

        public string fgfx标识 { get; set; }

        public string PCBA标识 { get; set; }

        public string ECUID { get; set; }

        public string pcba { get; set; }

        public string 编程写DID返工标识 { get; set; }
    }
}
