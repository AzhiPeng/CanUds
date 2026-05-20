using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyManageForGm.Model
{
    [Table("生产任务")]
    public class ProductionTaskModel:BaseModel
    {
        [Column("ord")]
        public override int Id { get; set; }
        public string 批次号 { get; set; }
        public string 客户订单 { get; set; }
        public string 客户名称 { get; set; }
        public string 产品型号 { get; set; }
        public string 产品名称 { get; set; }
        public string 产品编码 { get; set; }
        public string 用户图号 { get; set; }
        public int 任务余量 { get; set; }
        public string 线体 { get; set; }
        public int 任务量 { get; set; }
        public int 计划时间 { get; set; }
        public string 班次编号 { get; set; }
        public string 班次名称 { get; set; }
        public string 班次代号 { get; set; }
        public DateTime? 操作时间 { get; set; }
        public DateTime? 开始时间 { get; set; }
        public DateTime? 结束时间 { get; set; }
        public string 备注 { get; set; }
        public string 标识 { get; set; }
        public string 上传标识 { get; set; }

    }
}
