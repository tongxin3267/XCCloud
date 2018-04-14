using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.DAL.Base
{
    public partial class BaseDAL<T> where T : class, new()
    {

        protected string dbContextName;
        private DbContext dbContext;
        public BaseDAL()
        {
            dbContext = DbContextFactory.CreateByModelNamespace(typeof(T).Namespace);
        }

        public BaseDAL(string containerName)
        {
            this.dbContextName = containerName;
            dbContext = DbContextFactory.CreateByContainerName(containerName);
        }

        //用于监测Context中的Entity是否存在，如果存在，将其Detach，防止出现问题。
        private Boolean RemoveHoldingEntityInContext(T entity)
        {
            var objContext = ((IObjectContextAdapter)dbContext).ObjectContext;
            var objSet = objContext.CreateObjectSet<T>();
            var entityKey = objContext.CreateEntityKey(objSet.EntitySet.Name, entity);

            Object foundEntity;
            var exists = objContext.TryGetObjectByKey(entityKey, out foundEntity);

            if (exists)
            {
                objContext.Detach(foundEntity);
            }

            return (exists);
        }

        public void AddModel(T t)
        {
            dbContext.Set<T>().Add(t);
        }

        public void UpdateModel(T t)
        {
            RemoveHoldingEntityInContext(t);
            dbContext.Set<T>().Attach(t);
            dbContext.Entry<T>(t).State = EntityState.Modified;
        }

        public void DeleteModel(T t)
        {
            RemoveHoldingEntityInContext(t);
            dbContext.Set<T>().Attach(t);
            dbContext.Entry<T>(t).State = EntityState.Deleted;
        }

        public bool Add(T t)
        {
            dbContext.Set<T>().Add(t);
            return dbContext.SaveChanges() > 0;
        }

        public bool Update(T t)
        {
            RemoveHoldingEntityInContext(t);
            dbContext.Set<T>().Attach(t);
            dbContext.Entry<T>(t).State = EntityState.Modified;
            bool result = dbContext.SaveChanges() > 0;
            return result;
        }

        public bool Delete(T t)
        {
            RemoveHoldingEntityInContext(t);
            dbContext.Set<T>().Attach(t);
            dbContext.Entry<T>(t).State = EntityState.Deleted;
            return dbContext.SaveChanges() > 0;
        }

        public int GetCount(Expression<Func<T, bool>> whereLambda)
        {
            return dbContext.Set<T>().Count<T>(whereLambda);
        }

        public bool Any(Expression<Func<T, bool>> whereLambda)
        {
            return dbContext.Set<T>().Any<T>(whereLambda);
        }

        public IQueryable<T> GetModels(Expression<Func<T, bool>> whereLambda)
        {
            return dbContext.Set<T>().AsNoTracking().Where(whereLambda);
        }

        public IQueryable<T> GetModels()
        {
            return dbContext.Set<T>().AsNoTracking().AsQueryable<T>();
        }

        public IQueryable<T> GetModelsByPage<type>(int pageSize, int pageIndex, bool isAsc,
            Expression<Func<T, type>> OrderByLambda, Expression<Func<T, bool>> WhereLambda, out int recordCount)
        {
            recordCount = dbContext.Set<T>().AsNoTracking().Where(WhereLambda).Count<T>(); 
            //是否升序
            if (isAsc)
            {
                return dbContext.Set<T>().AsNoTracking().Where(WhereLambda).OrderBy(OrderByLambda).Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                return dbContext.Set<T>().Where(WhereLambda).OrderByDescending(OrderByLambda).Skip(pageIndex * pageSize).Take(pageSize);
            }
        }

        public bool SaveChanges()
        {
            return dbContext.SaveChanges() > 0;
        }

        public int ExecuteSqlCommand(string sql,params object[] parameters)
        {
            return dbContext.Database.ExecuteSqlCommand(sql, parameters);
        }

        public IQueryable<T> SqlQuery(string sql, params object[] parameters)
        {
            return dbContext.Database.SqlQuery<T>(sql, parameters).AsQueryable<T>();
        }

        public IQueryable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return dbContext.Database.SqlQuery<TElement>(sql, parameters).AsQueryable<TElement>();
        }
    }
}
