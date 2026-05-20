using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;

namespace KeyManageForGm.Dal
{
    /// <summary>
    /// 基于BseEntity的基本DAO
    /// </summary>
    /// <typeparam name="Context">数据库</typeparam>
    /// <typeparam name="TModel">数据表</typeparam>
    public class BaseDao<Context, TModel> 
        where Context : DbContext, new()
        where TModel : Model.BaseModel, new()
    {
        //public IProcedureDao procedure;

        public virtual int Insert(TModel model)
        {
            using (Context db = new Context())
            {
                db.Set<TModel>().Add(model);
                var rtn = db.SaveChanges();
                //procedure.P_Sync_Excess_All(db);
                return rtn ;
            }
        }
        public virtual int InsertOnNotExists(TModel model, Expression<Func<TModel, bool>> filterExpression)
        {
            using (Context db = new Context())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    //判断是否有重复数据
                    var list = db.Set<TModel>().Where(filterExpression);
                    if (list.Count(d => 1 == 1) == 0)
                    {
                        //插入数据
                        db.Set<TModel>().Add(model);
                        var rtn = db.SaveChanges();
                        dbContextTransaction.Commit();
                        return rtn;
                    }
                    else
                    {
                        
                        return -1;
                    }
                }
            }
        }

        public virtual bool Update(TModel model)
        {
            using (Context db = new Context())
            {
                if (db.Entry<TModel>(model).State == EntityState.Detached)
                {
                    db.Set<TModel>().Attach(model);
                    db.Entry<TModel>(model).State = EntityState.Modified;
                }
                var rtn = db.SaveChanges();
                //procedure.P_Sync_Excess_All(db);
                return rtn > 0;
            }
        }

        //public virtual int Update(Expression<Func<TModel, bool>> filterExpression, Expression<Func<TModel, TModel>> updateExpression)
        //{
        //    using (Context db = new Context())
        //    {
        //        var rtn = db.Set<TModel>().Where(filterExpression).Update(updateExpression);
        //        //procedure.P_Sync_Excess_All(db);
        //        return rtn;
        //    }
        //}

        public virtual bool Delete(int id)
        {
            using (Context db = new Context())
            {
                TModel model = db.Set<TModel>().Where(d=> d.Id==id ).Single();
                db.Set<TModel>().Remove(model);
                return db.SaveChanges() > 0;
            }
        }

        //public virtual int Delete(Expression<Func<TModel, bool>> filterExpression)
        //{
        //    using (Context db = new Context())
        //    {
        //        return db.Set<TModel>().Where(filterExpression).Delete();
        //    }
        //}

        public virtual IEnumerable<TModel> GetList(Expression<Func<TModel, bool>> filterExpression)
        {
            using (Context db = new Context())
            {
                IQueryable<TModel> data = db.Set<TModel>().Where(filterExpression);
                return data.ToList();
            }
        }

        public virtual int Count(Expression<Func<TModel, bool>> filterExpression)
        {
            using (Context db = new Context())
            {
                int data = db.Set<TModel>().Count(filterExpression);
                return data;
            }
        }
        public virtual IEnumerable<TModel> GetList<TKey>(Expression<Func<TModel, bool>> filterExpression, int page, int recordsPrePage)
        {
            using (Context db = new Context())
            {
                var skip = (page) * recordsPrePage;
                return db.Set<TModel>().Where(filterExpression).Skip(skip).Take(recordsPrePage).ToList();
            }
        }
        public virtual IEnumerable<TModel> GetList(Expression<Func<TModel, bool>> filterExpression, int page, int recordsPrePage)
        {
            using (Context db = new Context())
            {
                var skip = page * recordsPrePage;
                return db.Set<TModel>().Where(filterExpression).OrderByDescending(d=>d.Id).Skip(skip).Take(recordsPrePage).ToList();
            }
        }
        public virtual IEnumerable<TModel> GetList<TKey>(Expression<Func<TModel, bool>> filterExpression, Expression<Func<TModel, TKey>> orderBy, int page, int recordsPrePage,bool isDesc=false)
        {
            using (Context db = new Context())
            {
                var skip = (page) * recordsPrePage;
                if (isDesc)
                {
                    return db.Set<TModel>().Where(filterExpression).OrderByDescending(orderBy).Skip(skip).Take(recordsPrePage).ToList();
                }
                else
                {
                    return db.Set<TModel>().Where(filterExpression).OrderBy(orderBy).Skip(skip).Take(recordsPrePage).ToList();
                }
            }
        }

        public virtual TModel GetSingle(Expression<Func<TModel, bool>> filterExpression)
        {
            using (Context db = new Context())
            {
                return db.Set<TModel>().Where(filterExpression).SingleOrDefault();
            }
        }

        public int Execute(string sql, params object[] paraments)
        {
            using (Context db = new Context())
            {
                var rtn = db.Database.ExecuteSqlCommand(sql, paraments);
                //procedure.P_Sync_Excess_All(db);
                return rtn;
            }
        }

        public IEnumerable<TModel> GetList(string sql, params object[] paraments)
        {
            using (Context context = new Context())
            {
                return context.Database.SqlQuery<TModel>(sql, paraments).ToList();
            }
        }

        public TModel GetSingle(string sql, params object[] paraments)
        {
            using (Context context = new Context())
            {
                return context.Database.SqlQuery<TModel>(sql, paraments).SingleOrDefault();
            }
        }

        public IEnumerable<T> GetList<T>(string sql, params object[] paraments)
        {
            using (Context context = new Context())
            {
                return context.Database.SqlQuery<T>(sql, paraments).ToList();
            }
        }

        public T GetSingle<T>(string sql, params object[] paraments)
        {
            using (Context context = new Context())
            {
                return context.Database.SqlQuery<T>(sql, paraments).SingleOrDefault();
            }
        }

        public IEnumerable<TModel> GetList<TKey>(Expression<Func<TModel, bool>> filterExpression, Expression<Func<TModel, TKey>> orderBy,bool isDesc=false)
        {
            using (Context db = new Context())
            {
                if (isDesc)
                {
                    return db.Set<TModel>().Where(filterExpression).OrderByDescending(orderBy).ToList();
                }
                else
                {
                    return db.Set<TModel>().Where(filterExpression).OrderBy(orderBy).ToList();
                }
            }
        }
    }
}
