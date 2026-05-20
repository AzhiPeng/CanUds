/******************************************************    
创建人:   朱旭
创建时间: 2017年7月5日
---------------------------------
修改人:   
修改时间: 
修改模块：      
---------------------------------
功能描述: 数据库连接基本类,继承后使用
******************************************************/

using System.Data.Entity;

namespace KeyManageForGm.Dal
{
    /// <summary>
    /// 创建人:   朱旭
    /// 创建时间: 2017年7月5日
    /// 功能描述: 数据库连接基本类,继承后使用
    /// </summary>
    public abstract class BaseDbContext : DbContext
    {
       public static string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Default"].ConnectionString;

        /// <summary>
        /// 使用Oracle时一般为用户名。
        /// </summary>
        public abstract string DefaultSchema { get; } 

        /// <summary>
        /// 创建模型
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            ///设置空间名
            if (!string.IsNullOrWhiteSpace(DefaultSchema))
                modelBuilder.HasDefaultSchema(DefaultSchema);

            base.OnModelCreating(modelBuilder);
        }

        public BaseDbContext(string connectionString):base(connectionString)
        {

        }
    }
}
