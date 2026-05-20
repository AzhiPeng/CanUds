using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyManageForGm.Model
{
    [Table("产品型号")]
    public  class ProductModelInfoModel:BaseModel
    {
        [Column("ord")]
        public override int Id { get; set; }
        public string 产品型号 { get; set; }
        public string 产品名称 { get; set; }
        public string 产品编码 { get; set; }
        public string 用户图号 { get; set; }
        //public string CAEA软件版本号 { get; set; }
        //public string CAEA硬件版本号 { get; set; }
        public int? 任务余量 { get; set; }
        public string PCBA码 { get; set; }
        public string 条码0 { get; set; }
        public string 条码1 { get; set; }
        public string 条码2 { get; set; }
        public string 条码3 { get; set; }
        public string 条码4 { get; set; }
        public string 条码5 { get; set; }
        public string 条码6 { get; set; }
        public string 条码7 { get; set; }
        public string 条码8 { get; set; }
        public string 条码9 { get; set; }
        public string 备注 { get; set; }
        public string 标识 { get; set; }
        public string 上传标识 { get; set; }
        public string 线体 { get; set; }

        //public string 客户软件版本号 { get; set; }
        //public string 客户硬件版本号 { get; set; }
        //public string CVPPS { get; set; }
        //public string DUNS { get; set; }
        //public string End_Model_PN { get; set; }
        //public string Base_Model_PN { get; set; }
        //public string Software_ModuleID1 { get; set; }
        //public string Software_ModuleID2 { get; set; }
        //public string Software_ModuleID3 { get; set; }
        //public string Software_ModuleDataID { get; set; }
        //public string Boot_SoftwareID { get; set; }
        //public string Application_SoftwareID { get; set; }
        //public string Application_DataID { get; set; }
        //public string ECU_Name { get; set; }
        //public string End_Model_PN_Alpha_Code { get; set; }
        //public string ECU_Diag_Address { get; set; }
        //public string End_Model { get; set; }
        //public string Base_Model { get; set; }

        public string ProductModel_GM { get; set; }

        [Column("FileName_Prefix")]
        public string FileNamePrefix { get; set; }

        [Column("Service_Name")]
        public string ServiceName { get; set; }

    }
}
