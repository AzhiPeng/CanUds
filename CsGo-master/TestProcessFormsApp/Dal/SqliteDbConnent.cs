using KeyManageForGm.Model;
using System.Data.Entity;


namespace KeyManageForGm.Dal
{

    public class SqliteDbConnent : BaseDbContext
    {
        public static string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["sqlite"].ConnectionString;

        public SqliteDbConnent():base("sqlite")
        {
            
        }

        public DbSet<TupleRequstInfoModel> TupleRequestDatas { get; set; }


    }
}
