using CCWin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using TestProcessFormsApp;
using TestProcessFormsApp.Public;
using TestProcessFormsApp.测试类;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace HMI
{
    class 关联追溯
    {
        int ProductType = 11;
        //上传相关
        public static string Operate;
        public static string conn;
        public static UpdateInfo update = null;
        public static UpdateInfo update_List = new UpdateInfo()
        {
            InfoList = new List<ColumnInfo>()
        };
        public static bool ScanFlag = false;
        public static string ProductID = "";
        public static string Mtc = "";
        public static string Modul = "";
        public static int UpdateResult = 0;
        public static int Result = 0;

       
        private static void ClearUpdate()
        {
            ScanFlag = false;
            ProductID = "";
            Mtc = "";
            Modul = "";
        }

        public static void AddUpdate(string productID, string mtc)
        {
            ProductID = productID;
            Mtc = mtc;
        }
 

        public static string UpdateToSql(string MTC, string Model, int Result, string CodeNo)
        {
            //if (update == null)
            //{
            //    ClearUpdate();
            //    return "";
            //}
            string sqltext = "";//string.Format("declare @RecordID varchar(50) " +
            //                          " declare @BaseModle varchar(50) " +
            //                          " declare @MTC varchar(50)" +
            //                          " declare @GWNO varchar(50)" +
            //                          " declare @UserName varchar(50)" +
            //                          " declare @Result int" +
            //                          //" set @BaseModle = (select x.用户图号 from dbo.[生产任务] x where  x.[标识] = 1)" +
            //                          " set @MTC = '{0}'" +
            //                          " set @GWNO = '{1}'" +
            //                          " set @Result = {2} " +
            //                          " set @UserName = '{3}' " +
            //                          " set @BaseModle = '{4}' " +
            //                          //" set @RecordID = '' ",
            //                          //" set @RecordID = (select a.[批次号] from dbo.[产品信息] a where a.[用户图号] = @BaseModle and a.[MTC] = @MTC and a.[有效标识] = 0)" +
            //                          //" if(LEN(@MTC) > 0) " +
            //                          //" if(1 = 1) " +
            //                          //" begin",
            //                   //" if not exists (select 1 from dbo.[检测数据] a where a.[用户图号] = @BaseModle and a.[MTC] = @MTC and a.[工位号] = @GWNO and a.[有效标识] = 0)" +
            //                   //" begin",
            //                   MTC,
            //                        CodeNo,
            //                        Result.ToString(),
            //                        CodeNo,
            //                        Model);
            //sqltext += " insert into dbo.[检测数据]([用户图号],[MTC],[工位号],[批次号],[操作人],[操作时间],[结果],[有效标识],[上传标识]";
            //for (int i = 0; i < update_List.InfoList.Count; i++)
            //{
            //    if (CodeNo != update_List.InfoList[i].CodeNo)
            //        continue;
            //    sqltext += ",[" + update_List.InfoList[i].Column + "]";
            //}
            //sqltext += " ) ";
            //sqltext += " values (@BaseModle,@MTC,@GWNO,@RecordID,@UserName,getdate(), @Result,0,1";

            //for (int i = 0; i < update_List.InfoList.Count; i++)
            //{
            //    string s = update_List.InfoList[i].CodeNo;
            //    if (CodeNo != update_List.InfoList[i].CodeNo)
            //        continue;
            //    sqltext += " ,'" + update_List.InfoList[i].Tips + "'";
            //}
            //sqltext += " )";
            //sqltext += " end";

            //sqltext += string.Format(" update dbo.[产品信息] set [{0}] = @Result,[包装标识] = (case when @Result = 2 then 0 else 3 end) where 用户图号 = @BaseModle and MTC = @MTC",
            //                CodeNo + "标识");
            sqltext += string.Format(" update dbo.[产品信息] set [{0}标识] ={1},[包装标识] = (case when {2} = 2 then 0 else 3 end) where 用户图号 ='{3}' and MTC ='{4}' and 有效标识 = '0'", CodeNo, Result, Result, Model, MTC);
            if (WinForm.产品型号 == "0Y4G40BAEWWA" || WinForm.产品型号 == "0Y4G40CAEWWA" || WinForm.产品型号 == "0Y4Z40CAEWWA" /*|| 产品型号 == "0Y4Z40DAEWWA"*/)//跳过天线工位
                sqltext += string.Format(" update dbo.[产品信息] set [{0}标识] ={1},[包装标识] = (case when {2} = 2 then 0 else 3 end) where 用户图号 ='{3}' and MTC ='{4}' and 有效标识 = '0'", "145", "2", "2", Model, MTC);
            sqltext += string.Format("INSERT INTO 检测数据 (用户图号,MTC, 工位号,操作人,操作时间,结果,产品名称,上传标识,测试路径,检测电脑编号) VALUES('{0}', '{1}', '{2}', '{3}', GETDATE(), '{4}', '{5}', '1', '{6}', '{7}')", Model, MTC, CodeNo, CodeNo, Result,WinForm.产品型号, WinForm.savepath, LocalSetting.localSetting.电脑编号);
            ClearUpdate();
            return sqltext;
        }
        /// <summary>
        /// MTC 图号 结果 工位
        /// </summary>
        /// <param name="MTC"></param>
        /// <param name="Model"></param>
        /// <param name="Result"></param>
        /// <param name="CodeNo"></param>
        /// <returns></returns>
        public static bool SaveData(string MTC, string Model, int Result, string CodeNo)
        {
            string sqltext = UpdateToSql(MTC, Model, Result, CodeNo);
            using (SqlConnection sqlcnt = new SqlConnection(WinForm.sqlstr))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    try
                    {
                       command.CommandType = CommandType.Text;
                        command.Connection = sqlcnt;
                        sqlcnt.Open();
                        command.CommandText = sqltext;
                        command.ExecuteNonQuery();
                        ClearUpdate();
                        WinForm.追溯结果 = 2;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        WinForm.追溯结果 = 3;
                        ClearUpdate();
                        return false;
                    }
                }
            }
        }
        public static string SaveDataString = string.Empty;
        public static string 整理需要上传的内容()
        {
            SaveDataString = string.Empty;
            try
            {
                foreach (var n in CaseConfig.caseList)
                {
                    ColumnInfo columnInfo = new ColumnInfo();
                    columnInfo.CodeNo = LocalSetting.localSetting.CurrentGw;
                    columnInfo.Column = n.GetCaseName();
                    columnInfo.Tips = n.GetData();
                    //ColumnInfo columnInfo1 = new ColumnInfo();
                    //columnInfo1.CodeNo = LocalSetting.localSetting.CurrentGw;
                    //columnInfo1.Column = columnInfo.Column + "结果";
                    //columnInfo1.Tips = n.GetState().ToString();
                    SaveDataString += $" [{n.GetCaseName()}：{n.GetData()}：{n.GetState()}：]";
                }
            }
            catch (Exception ex)
            {

            }
            return SaveDataString;
        }
    }
}
