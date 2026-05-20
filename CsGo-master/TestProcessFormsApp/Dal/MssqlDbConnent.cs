using KeyManageForGm.Model;
using System.Data.Entity;


namespace KeyManageForGm.Dal
{

    public class MssqlDbConnent : BaseDbContext
    {
        public static string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Default"].ConnectionString;

        public MssqlDbConnent():base("Default")
        {
            Database.SetInitializer<MssqlDbConnent>(null);
        }

        public DbSet<ProductionTaskModel> 生产任务 { get; set; }

        public DbSet<ProductInfoModel> 产品信息 { get; set; }

        public DbSet<ProductModelInfoModel> 产品型号 { get; set; }
        public DbSet<EcuidInfoModel> EcuidInfo { get; set; }


    }
}
