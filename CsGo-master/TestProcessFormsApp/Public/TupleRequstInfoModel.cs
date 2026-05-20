using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyManageForGm.Model
{

    [Table("tb_tuple_requests")]
    public class TupleRequstInfoModel //: BaseModel
    {
        [Key]
        public string Id { get; set; }

        [Column("request_id")]
        public string RequetId { get; set; } = "";

        [Column("product_name")]
        public string ProductName { get; set; } = "";

        [Column("gm_product_name")]
        public string GmProductName { get; set; } = "";

        [Column("is_bind_pn")]
        public int IsBindPn { get; set; } = 0;

        [Column("requst_num")]
        public int RequstNum { get; set; } = 0;

        [Column("file_name")]
        public string FileName { get; set; } = "";

        [Column("stage")]
        public StageType Stage { get; set; } = 0;

        [Column("status")]
        public string Status { get; set; } = "";

        [Column("requst_time")]
        public DateTime RequstTime { get; set; } = DateTime.Now;

        [Column("download_time")]
        public DateTime? DownloadTime { get; set; } = default;

        [Column("expired_time")]
        public DateTime? ExpiredTime { get; set; } = default;

        [Column("export_time")]
        public DateTime? ExportTime { get; set; } = default;
    }
    public enum StageType : int
    {
        None = 0,
        GetRequstId = 10,
        Downloading = 20,
        Downloaded = 21,
        Import = 30,
        ImportFailed = 31,
        Expired = 32,
        LostFile = 33,
        ServerFailed = 34,
    }
}
