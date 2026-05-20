using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Data.Entity;
using static KeyManageForGm.Dal.MssqlDbConnent;
using JLRScan.Log;
using JLRScan;
using TestProcessFormsApp;

namespace KeyManageForGm.Dal
{
    internal class EcuidInfoDal
    {
        public ScanLog Log;
        internal List<Model.EcuidInfoModel> GetUnuseData(string productModel_GM)
        {
            string sqlText = $"select * from [dbo].[ECUIDINFO] where  [MKM4] is null and [MKM5] is null and [UKM4] is null and [UKM5] is null and [ProductModel_GM] ='{productModel_GM}'";
            try
            {
                using (var db = new MssqlDbConnent())
                {
                    return db.Database.SqlQuery<Model.EcuidInfoModel>(sqlText).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal List<Model.EcuidInfoModel> GetAvailableData(string productModel_GM)
        {
            string sqlText = $"select  * from [dbo].[ECUIDINFO] where  [STATE] = '0' and [ProductModel_GM] ='{productModel_GM}' and [ECUID] = '{WinForm.ECUID}'";
            try
            {
                using (var db = new MssqlDbConnent())
                {
                    return db.Database.SqlQuery<Model.EcuidInfoModel>(sqlText).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal List<Model.EcuidInfoModel> GetAvailableDataState2(string productModel_GM)
        {
            string sqlText = $"select  * from [dbo].[ECUIDINFO] where  [STATE] = '2' and [ProductModel_GM] ='{productModel_GM}' and [ECUID] = '{WinForm.ECUID}'";
            try
            {
                using (var db = new MssqlDbConnent())
                {
                    return db.Database.SqlQuery<Model.EcuidInfoModel>(sqlText).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal bool UpdateUpStateBatch(string ecuidInfos)
        {
            bool isSuccess = true;
            var now = DateTime.Now;

            string sqlText = $" Update [dbo].[ECUIDINFO] set State={2} , UploadTime=@UploadTIme where ecuid = @ecuid  ";
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();
                    SqlCommand cmd = new SqlCommand(sqlText, conn, trans);
                    //for(int i=0;i<40;i++)
                    
                    cmd.Parameters.Add(new SqlParameter("@Ecuid", ecuidInfos));
                    cmd.Parameters.Add(new SqlParameter("@UploadTIme", now));

                    isSuccess &= cmd.ExecuteNonQuery() > 0;
                    cmd.Parameters.Clear();
                    

                    if (isSuccess) trans.Commit(); else trans.Rollback();

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                    sw.Stop();
                    System.Diagnostics.Debug.WriteLine($"dddddd:{sw.ElapsedMilliseconds}");
                }
            }

            return isSuccess;

        }
        internal bool UpdateUploadStateBatch(IEnumerable<Model.EcuidInfoModel> ecuidInfos, bool isAuto = true)
        {
            bool isSuccess = true;
            var now = DateTime.Now;

            int uploadState = isAuto ? 11 : 21;
            string sqlText = $" Update [dbo].[ECUIDINFO] set UploadState={uploadState} , UploadTime=@UploadTIme where ecuid = @ecuid  ";
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();
                    SqlCommand cmd = new SqlCommand(sqlText, conn, trans);
                    //for(int i=0;i<40;i++)
                    foreach (var item in ecuidInfos)
                    {
                        cmd.Parameters.Add(new SqlParameter("@Ecuid", item.ECUID));
                        cmd.Parameters.Add(new SqlParameter("@UploadTime", now));

                        isSuccess &= cmd.ExecuteNonQuery() > 0;
                        cmd.Parameters.Clear();
                    }

                    if (isSuccess) trans.Commit(); else trans.Rollback();

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                    sw.Stop();
                    System.Diagnostics.Debug.WriteLine($"dddddd:{sw.ElapsedMilliseconds}");
                }
            }

            return isSuccess;

        }

        internal bool UpdateGmServerResultBatch(IEnumerable<Model.EcuidInfoModel> ecuidInfos)
        {
            bool isSuccess = true;
            var now = DateTime.Now;
            string sqlText = $" Update [dbo].[ECUIDINFO] set UploadState=@UploadState , [MkValid_GM]=@MkValid ,UkValid_GM=@UkValid ,[Upload_Result_Status]=@UploadResult where ecuid = @ecuid and UploadState%10 !=2  ";
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();
                    SqlCommand cmd = new SqlCommand(sqlText, conn, trans);

                    foreach (var item in ecuidInfos)
                    {
                        cmd.Parameters.Add(new SqlParameter("@Ecuid", item.ECUID));
                        cmd.Parameters.Add(new SqlParameter("@UploadTime", now));
                        cmd.Parameters.Add(new SqlParameter("@MkValid", item.MkValid));
                        cmd.Parameters.Add(new SqlParameter("@UkValid", item.UkValid));
                        cmd.Parameters.Add(new SqlParameter("@UploadState", item.UploadState));
                        cmd.Parameters.Add(new SqlParameter("@UploadResult", item.Upload_Result_Status));

                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            isSuccess &= false;
                            Log.InsertLog($"【UpdateGmServerResultBatch】【ecuid:{item.ECUID}】重新失败");
                        }
                        cmd.Parameters.Clear();
                    }
                    trans.Commit();
                    //if (isSuccess) trans.Commit(); else trans.Rollback();

                }
                catch (Exception ex)
                {
                   //Utils.Logger.Info($"【UpdateGmServerResultBatch】出错，{ex.Message}");
                    return false;
                }
                finally
                {
                    conn.Close();
                    sw.Stop();
                    System.Diagnostics.Debug.WriteLine($"dddddd:{sw.ElapsedMilliseconds}");
                }
            }

            return true;

        }

        internal bool InsertBatch(IEnumerable<Model.EcuidInfoModel> ecuidInfos)
        {
            bool isSuccess = true;

            const string sqlText = "IF NOT EXISTS(SELECT TOP 1 ECUID FROM  [dbo].[ECUIDINFO] WHERE ECUID=@ECUID) "
                + " INSERT INTO [dbo].[ECUIDINFO] ([ECUID],[UNLOCKKEY],[PN],[STATE],[CREATETIME],[MKM1],[MKM2],[MKM3],[UKM1],[UKM2],[UKM3],[RequstDate_GM],[ProductModel_GM])"
                                       + " VALUES(@ECUID,  'N/A'  ,   @PN,   0   , @CREATETIME, @MKM1, @MKM2, @MKM3, @UKM1, @UKM2, @UKM3,@RequstDate_GM,@ProductModel_GM) " +
                                       " else update [dbo].[ECUIDINFO] set [ECUID]=@ECUID where '1'='2';";
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();
                    SqlCommand cmd = new SqlCommand(sqlText, conn, trans);

                    foreach (var item in ecuidInfos)
                    {
                        cmd.Parameters.Add(new SqlParameter("@Ecuid", item.ECUID));
                        cmd.Parameters.Add(new SqlParameter("@CreateTime", item.CREATETIME));
                        cmd.Parameters.Add(new SqlParameter("@MKM1", item.MKM1));
                        cmd.Parameters.Add(new SqlParameter("@MKM2", item.MKM2));
                        cmd.Parameters.Add(new SqlParameter("@MKM3", item.MKM3));
                        cmd.Parameters.Add(new SqlParameter("@UKM1", item.UKM1));
                        cmd.Parameters.Add(new SqlParameter("@UKM2", item.UKM2));
                        cmd.Parameters.Add(new SqlParameter("@UKM3", item.UKM3));
                        cmd.Parameters.Add(new SqlParameter("@PN", item.PN ?? ""));
                        cmd.Parameters.Add(new SqlParameter("@RequstDate_GM", item.RequstDate_GM));
                        cmd.Parameters.Add(new SqlParameter("@ProductModel_GM", item.ProductModel_GM));
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            isSuccess &= false;
                            Log.InsertLog($"【InsertBatch】【ecuid:{item.ECUID}】插入失败");
                        }
                        cmd.Parameters.Clear();
                    }
                    if (isSuccess) trans.Commit(); else trans.Rollback();

                }
                catch (Exception ex)
                {
                    Log.InsertLog($"【InsertBatch】出错，{ex.Message}");
                    return false;
                    //throw ex;
                }
                finally
                {
                    conn.Close();
                    sw.Stop();
                    System.Diagnostics.Debug.WriteLine($"dddddd:{sw.ElapsedMilliseconds}");
                }
            }

            return isSuccess;

        }
        public Model.EcuidInfoModel GetInfo(string ecuid) => new MssqlDbConnent().EcuidInfo.FirstOrDefault(x => x.ECUID == ecuid);


        //internal List<Model.EcuidInfoModel> GetUploadDataForTimeOut()
        //{
        //    return GetUploadData("select * from [dbo].[ECUIDINFO] where [MkValid]=1 and [UkValid]=1 and [MKM4] is not null and [MKM5] is not null and [UKM4] is not null and [UKM5] is not null and [UploadState]=11");
        //}
        //internal List<Model.EcuidInfoModel> GetUploadDataForError()
        //{
        //    return GetUploadData("select * from [dbo].[ECUIDINFO] where [MkValid]=1 and [UkValid]=1 and [MKM4] is not null and [MKM5] is not null and [UKM4] is not null and [UKM5] is not null and [UploadState]=13 and [UploadTime]<@UploadTime");
        //}

        //internal List<Model.EcuidInfoModel> GetUploadData()
        //{
        //    return GetUploadData("select * from [dbo].[ECUIDINFO] where [MkValid]=1 and [UkValid]=1 and [MKM4] is not null and [MKM5] is not null and [UKM4] is not null and [UKM5] is not null and [UploadState]=0");
        //}

        private List<Model.EcuidInfoModel> GetList(string sqlText)
        {
            using (var db = new DbContext(ConnectionString))
            {
                try
                {
                    return db.Database.SqlQuery<Model.EcuidInfoModel>(sqlText).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public int UpdateStateToUsed(string ecuid, string reason)
        {
            int result = 0;
            using (MssqlDbConnent db = new MssqlDbConnent())
            {
                var trans = db.Database.BeginTransaction();
                var info = db.EcuidInfo.FirstOrDefault(x => x.ECUID == ecuid);
                if (info == default)
                {
                    return -2;
                }
                else if (!string.IsNullOrEmpty(info.MKM4) ||
                    !string.IsNullOrEmpty(info.MKM5) ||
                    !string.IsNullOrEmpty(info.UKM4) ||
                    !string.IsNullOrEmpty(info.UKM5) ||
                    !string.IsNullOrEmpty(info.MTC) ||
                    info.UPDATETIME != null ||
                    info.UploadState != 0)
                {
                    trans.Rollback();
                    return -1;
                }
                else
                {

                    info.MTC = "1";
                    info.Upload_Result_Status = reason;
                    info.UploadState = Model.EcuidInfoModel.UploadStateType.手动回传结果_验证成功;
                    info.UPDATETIME = DateTime.Now;
                    info.UploadTime = DateTime.Now;
                    var rtn = db.SaveChanges();
                    trans.Commit();
                    //procedure.P_Sync_Excess_All(db);
                    return rtn;
                }
            }
        }

        internal bool UpdateWriteKeyResult(
            string ecuid,
            string mkm4,
            string mkm5,
            string ukm4,
            string ukm5,
            byte mkValid,
            byte ukValid,
            string uploadResult = "")
        {
            try
            {
                using (var db = new MssqlDbConnent())
                {
                    var info = db.EcuidInfo.FirstOrDefault(x => x.ECUID == ecuid);
                    if (info == null)
                    {
                        return false;
                    }

                    // 2026-04-27: 回写密钥结果，供后续上传与追溯流程使用。
                    if (!string.IsNullOrWhiteSpace(mkm4)) info.MKM4 = mkm4;
                    if (!string.IsNullOrWhiteSpace(mkm5)) info.MKM5 = mkm5;
                    if (!string.IsNullOrWhiteSpace(ukm4)) info.UKM4 = ukm4;
                    if (!string.IsNullOrWhiteSpace(ukm5)) info.UKM5 = ukm5;

                    info.MkValid = mkValid;
                    info.UkValid = ukValid;
                    info.UPDATETIME = DateTime.Now;
                    //if (!string.IsNullOrWhiteSpace(uploadResult))
                    //{
                    //    info.Upload_Result_Status = uploadResult;
                    //}

                    return db.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                Log?.InsertLog($"【UpdateWriteKeyResult】出错，{ex.Message}");
                return false;
            }
        }

        //private  DataSet GetUploadData(string sqlText)
        //{
        //    //const string sqlText = " select * from [dbo].[ECUIDINFO] where [MkValid]=1 and [UkValid]=1 and [MKM4] is not null and [MKM5] is not null and [UKM4] is not null and [UKM5] is not null";
        //    using (var conn = new SqlConnection(ConnectionString))
        //    {
        //        try
        //        {
        //            conn.Open();
        //            var cmd = new SqlCommand(sqlText,conn);
        //            SqlDataAdapter da = new SqlDataAdapter(cmd); 
        //            DataSet ds = new DataSet();
        //            da.Fill(ds);
        //            return ds;
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        finally
        //        {
        //            conn.Close();
        //        }
        //        return null;
        //    }
        //}

        //internal bool UpdateBatch(IEnumerable<Model.EcuidInfoModel> ecuidInfos)
        //{
        //    bool isSuccess = true;

        //    const string sqlText = "IF NOT EXISTS(SELECT TOP 1 ECUID FROM  [dbo].[ECUIDINFO] WHERE ECUID=@ECUID) "
        //        + " INSERT INTO [dbo].[ECUIDINFO] ([ECUID],[UNLOCKKEY],[PN],[STATE],[CREATETIME],[MKM1],[MKM2],[MKM3],[UKM1],[UKM2],[UKM3]) "
        //                               + " VALUES(@ECUID,  'N/A'  ,   @PN,   0   , @CREATETIME, @MKM1, @MKM2, @MKM3, @UKM1, @UKM2, @UKM3);";
        //    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //    sw.Start();

        //    using (var conn = new SqlConnection(ConnectionString))
        //    {
        //        try
        //        {
        //            ecuidInfos[0]
        //            conn.Open();
        //            SqlTransaction trans = conn.BeginTransaction();
        //            SqlCommand cmd = new SqlCommand(sqlText, conn, trans);
        //            for (int i = 0; i < 40; i++)
        //                foreach (var item in ecuidInfos)
        //                {
        //                    cmd.Parameters.Add(new SqlParameter("@Ecuid", item.ECUID));
        //                    cmd.Parameters.Add(new SqlParameter("@CreateTime", item.CREATETIME));
        //                    cmd.Parameters.Add(new SqlParameter("@MKM1", item.MKM1));
        //                    cmd.Parameters.Add(new SqlParameter("@MKM2", item.MKM2));
        //                    cmd.Parameters.Add(new SqlParameter("@MKM3", item.MKM3));
        //                    cmd.Parameters.Add(new SqlParameter("@UKM1", item.UKM1));
        //                    cmd.Parameters.Add(new SqlParameter("@UKM2", item.UKM2));
        //                    cmd.Parameters.Add(new SqlParameter("@UKM3", item.UKM3));
        //                    cmd.Parameters.Add(new SqlParameter("@PN", item.PN));
        //                    cmd.Parameters.Add(new SqlParameter("@UploadTime", DateTime.Now.AddHours(-0.5)));
        //                    if (cmd.ExecuteNonQuery() == 0)
        //                    {
        //                        isSuccess &= false;
        //                        Utils.Logger.Debug($"【UpdateBatch】【ecuid:{item.ECUID}】插入失败");
        //                    }
        //                    cmd.Parameters.Clear();
        //                }
        //            if (isSuccess) trans.Commit(); else trans.Rollback();

        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        finally
        //        {
        //            conn.Close();
        //            sw.Stop();
        //            System.Diagnostics.Debug.WriteLine($"dddddd:{sw.ElapsedMilliseconds}");
        //        }
        //    }

        //    return isSuccess;

        //}


    }

}


