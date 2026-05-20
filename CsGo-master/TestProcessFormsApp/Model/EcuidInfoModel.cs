using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyManageForGm.Model
{
    [Table("ECUIDINFO")]
    public class EcuidInfoModel
    {
        [Key]
        public string ECUID { get; set; }
        public string UNLOCKKEY { get; set; }
        public string PN { get; set; }
        public int? STATE { get; set; }
        public DateTime CREATETIME { get; set; }
        public DateTime? UPDATETIME { get; set; }
        public string MTC { get; set; }
        public string SESSION_KEY { get; set; }
        public string SendKey { get; set; }
        public string MKM1 { get; set; }
        public string MKM2 { get; set; }
        public string MKM3 { get; set; }
        public string MKM4 { get; set; }
        public string MKM5 { get; set; }
        public string UKM1 { get; set; }
        public string UKM2 { get; set; }
        public string UKM3 { get; set; }
        public string UKM4 { get; set; }
        public string UKM5 { get; set; }

        public byte? MkValid { get; set; }

        public byte? UkValid { get; set; }
        public UploadStateType? UploadState { get; set; }
        public DateTime? UploadTime { get; set; }
        public DateTime? RequstDate_GM { get; set; }
        public string ProductModel_GM { get; set; }

        [Column("Upload_Result_Status")]
        public string Upload_Result_Status { get; set; }
        public enum UploadStateType : byte
        {
            未上传 = 0,
            验证成功 = 2,
            验证失败 = 3,
            操作失败 = 9,
            自动上传 = 10,
            正在自动上传 = 11,
            自动上传_验证成功 = 12,
            自动上传_验证失败 = 13,
            自动上传_上传失败 = 19,
            手动上传 = 20,
            手动上传_已导出 = 21,
            手动回传结果_验证成功 = 22,
            手动回传结果_验证失败 = 23,
        }
    }
}
