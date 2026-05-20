using KeyManageForGm.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KeyManageForGm.Dal
{

    internal class ProductInfoDal: BaseDao<MssqlDbConnent,ProductInfoModel>
    {
        public DataSet GetProductDetailInfoFrom批次号(string 批次号)
        { 
            using (var conn = new SqlConnection(MssqlDbConnent.ConnectionString))
            {
                try
                {
                    conn.Open();
                    const string sqlTest = "select e.* from [dbo].[产品信息] c left join [dbo].[ECUIDINFO] e on c.MTC=E.MTC  and c.用户图号=e.PN where [批次号] = @批次号 ";
                    SqlCommand cmd = new SqlCommand(sqlTest, conn);
                    cmd.Parameters.AddWithValue("@批次号", 批次号);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
            
        }
        public DataSet GetUnuploadProductDetailInfo()
        {
            using (var conn = new SqlConnection(MssqlDbConnent.ConnectionString))
            {
                try
                {
                    conn.Open();
                    const string sqlTest = "select e.*,c.批次号 from [dbo].[产品信息] c left join [dbo].[ECUIDINFO] e on c.MTC=E.MTC  and c.用户图号=e.PN  where [UploadState]=0 and e.MkValid=1 and e.UkValid=1 ";
                    SqlCommand cmd = new SqlCommand(sqlTest, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }

        }
    }
}
