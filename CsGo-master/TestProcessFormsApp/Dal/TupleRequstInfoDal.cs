using KeyManageForGm.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.Entity;

namespace KeyManageForGm.Dal
{
    internal class TupleRequstInfoDal//:BaseDao<SqliteDbConnent, TupleRequstInfoModel>
    {
        public TupleRequstInfoModel GetUncompleteInfo()
        {
            using (SqliteDbConnent db = new SqliteDbConnent())
            {
                return db.TupleRequestDatas.FirstOrDefault(
                     x => x.Stage == StageType.GetRequstId ||
                          x.Stage == StageType.Downloading ||
                          x.Stage == StageType.Downloaded);
            }
        }
        public bool InsertBatch(TupleRequstInfoModel model)
        {
            const string sqlStr = "INSERT INTO tb_tuple_requests ( id,request_id, product_name, gm_product_name,is_bind_pn, requst_num, file_name, status, stage,requst_time, download_time, expired_time, export_time)" +
                                                " VALUES( @id,@request_id, @product_name, @gm_product_name, @is_bind_pn, @requst_num, @file_name, @status,@stage, @requst_time, @download_time, @expired_time, @export_time)  ";

            using (SQLiteConnection conn = new SQLiteConnection(SqliteDbConnent.ConnectionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = sqlStr;
                    cmd.Parameters.Add(new SQLiteParameter("@id", model.Id));
                    cmd.Parameters.Add(new SQLiteParameter("@request_id", model.RequetId));
                    cmd.Parameters.Add(new SQLiteParameter("@gm_product_name", model.GmProductName));
                    cmd.Parameters.Add(new SQLiteParameter("@is_bind_pn", model.IsBindPn));
                    cmd.Parameters.Add(new SQLiteParameter("@product_name", model.ProductName));
                    cmd.Parameters.Add(new SQLiteParameter("@requst_num", model.RequstNum));
                    cmd.Parameters.Add(new SQLiteParameter("@file_name", model.FileName));
                    cmd.Parameters.Add(new SQLiteParameter("@status", model.Status));
                    cmd.Parameters.Add(new SQLiteParameter("@requst_time", model.RequstTime));
                    cmd.Parameters.Add(new SQLiteParameter("@stage", model.Stage));
                    cmd.Parameters.Add(new SQLiteParameter("@download_time", model.DownloadTime));
                    cmd.Parameters.Add(new SQLiteParameter("@expired_time", model.ExpiredTime));
                    cmd.Parameters.Add(new SQLiteParameter("@export_time", model.ExpiredTime));

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            return false;

        }
        public bool Update(TupleRequstInfoModel model)
        {
            using (SqliteDbConnent db = new SqliteDbConnent())
            {
               //var info= db.TupleRequestDatas.FirstOrDefault(x => x.Id == model.Id);
                if (db.Entry(model).State == EntityState.Detached)
                {
                    db.TupleRequestDatas.Attach(model);
                    db.Entry(model).State = EntityState.Modified;
                }
                var rtn = db.SaveChanges();
                return rtn > 0;
            }
        }

        public bool UpdateStatus(string id, string statusText)
        {
            string sqlStr = " UPDATE tb_tuple_requests  set  status = @status  " +
                        " WHERE id = @id ";

            using (SQLiteConnection conn = new SQLiteConnection(SqliteDbConnent.ConnectionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlStr;
                    cmd.Parameters.Add(new SQLiteParameter("@id", id));
                    cmd.Parameters.Add(new SQLiteParameter("@status", statusText));

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            return false;
        }

        public bool UpdateStatus(string id, string statusText,string fileName)
        {
            string sqlStr = " UPDATE tb_tuple_requests  set  status = @status , file_name=@fileName , download_time=@datetime " +
                        " WHERE id = @id ";

            using (SQLiteConnection conn = new SQLiteConnection(SqliteDbConnent.ConnectionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlStr;
                    cmd.Parameters.Add(new SQLiteParameter("@id", id));
                    cmd.Parameters.Add(new SQLiteParameter("@status", statusText));
                    cmd.Parameters.Add(new SQLiteParameter("@fileName", fileName));
                    cmd.Parameters.Add(new SQLiteParameter("@datetime", DateTime.Now));

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            return false;
        }

        public bool UpdateStatus(string id, string statusText, StageType type,string result="")
        {
            string typeStr = "";
            switch (type)
            {
                case StageType.Import:
                    typeStr = "  , export_time=@datetime ";
                    break;
                case StageType.Expired:
                    typeStr = " , expired_time=@datetime ";
                    break;
                case StageType.Downloaded:
                    typeStr = " , download_time=@datetime ";
                    break;
                //default:
                //    return false;
            }
            string sqlStr = " UPDATE tb_tuple_requests  set " +
                " status = @status  " + 
                typeStr + 
                " ,stage=" + (int)type + "  " +
               (string.IsNullOrWhiteSpace(result)?" ": " , result = @result ") +
                " WHERE id = @id";


            using (SQLiteConnection conn = new SQLiteConnection(SqliteDbConnent.ConnectionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlStr;
                    cmd.Parameters.Add(new SQLiteParameter("@id", id));
                    cmd.Parameters.Add(new SQLiteParameter("@status", statusText));
                    cmd.Parameters.Add(new SQLiteParameter("@datetime", DateTime.Now));
                    cmd.Parameters.Add(new SQLiteParameter("@result", result));

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            return false;
        }

        public bool UpdateDoloadStatus(string id, string statusText, string fileName)
        {
            string sqlStr = " UPDATE tb_tuple_requests  set status = @status ,download_time= @datetime,file_name=@file_name,stage=" + (int)StageType.Downloaded + " " +
                " WHERE id = @id ";

            using (SQLiteConnection conn = new SQLiteConnection(SqliteDbConnent.ConnectionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlStr;
                    cmd.Parameters.Add(new SQLiteParameter("@id", id));
                    cmd.Parameters.Add(new SQLiteParameter("@status", statusText));
                    cmd.Parameters.Add(new SQLiteParameter("@datetime", DateTime.Now));
                    cmd.Parameters.Add(new SQLiteParameter("@file_name", fileName));

                    return cmd.ExecuteNonQuery() > 0;
                }
            }

            return false;
        }


    }
}
