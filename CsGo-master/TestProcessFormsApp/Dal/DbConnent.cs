using KeyManageForGm.Model;
using System.Data.Entity;


namespace KeyManageForGm.Dal
{

    public abstract class BaseDbContext : DbContext
    {
        //public string ConnectionString { get; set; } //= System.Configuration.ConfigurationManager.ConnectionStrings["Default"].ConnectionString;

        public BaseDbContext(string ConnectionName) :base(ConnectionName)
        {
            Database.SetInitializer<BaseDbContext>(null);
        }
    }
}
