using MEM.DAL.Interface;
using MEM.Domain.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using MEM.Infrastructure.Extensions;

namespace MEM.DAL
{
    public class Repository : IRepository
    {

        DbContext Context;

        public Repository()
        {
            Context = new EFDbContext();
        }

        public void Initialize()
        {
            Database.SetInitializer<EFDbContext>(null);
        }


        public Repository(EFDbContext context)
        {
            Context = context;
        }


        //public void CommitChanges()
        //{
        //    Context.SaveChanges();
        //}


        public T Single<T>(Expression<Func<T, bool>> expression) where T : ModelBase
        {
            return All<T>().Where(a => a.IsActive == true).FirstOrDefault(expression);
           
        }

        public T ForceSingleFromDB<T>(Expression<Func<T, bool>> expression) where T : ModelBase
        {
            using (EFDbContext cntxt = new EFDbContext())
            {
                return cntxt.Set<T>().Where(a => a.IsActive == true).AsQueryable().FirstOrDefault(expression);
            }
        }


        public IQueryable<T> All<T>() where T : ModelBase
        {
            return Context.Set<T>().Where(a => a.IsActive == true).AsQueryable();
        }


        public virtual IQueryable<T> Filter<T>(string filterExpression, string sortExpression, string sortDirection, int pageIndex, int pageSize, int pagesCount) where T : ModelBase
        {
            if (!String.IsNullOrWhiteSpace(filterExpression))
                return Context.Set<T>().Where(filterExpression).Where(a => a.IsActive == true).OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pageSize);
            else
                return Context.Set<T>().Where(a => a.IsActive == true).OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pagesCount * pageSize);
        }

        public virtual IQueryable<T> Filter<T>(string filterExpression, Expression<Func<T, bool>> predicate, string sortExpression, string sortDirection, int pageIndex, int pageSize, int pagesCount) where T : ModelBase
        {
            if (!String.IsNullOrWhiteSpace(filterExpression))
                return Context.Set<T>().Where(a => a.IsActive == true).Where(filterExpression).Where(predicate).OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pageSize);
            else
                return Context.Set<T>().Where(a => a.IsActive == true).Where(predicate).OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pagesCount * pageSize);
        }

        public virtual IQueryable<T> Filter<T>(string filterExpression) where T : ModelBase
        {
            if (!String.IsNullOrWhiteSpace(filterExpression))
                return Context.Set<T>().Where(filterExpression).Where(a => a.IsActive == true);
            else
                return Context.Set<T>().Where(a => a.IsActive == true);
        }

        public virtual IQueryable<T> Filter<T>(Expression<Func<T, bool>> predicate) where T : ModelBase
        {
            return predicate != null ? Context.Set<T>().Where(predicate).Where(a => a.IsActive == true).AsQueryable() : Context.Set<T>().AsQueryable();
        }

        public virtual IQueryable<T> Filter<T>(Expression<Func<T, bool>> filter, out int total, int index = 0, int size = 50) where T : ModelBase
        {
            int skipCount = index * size;
            var _resetSet = filter != null ? Context.Set<T>().Where(filter).Where(a => a.IsActive == true).AsQueryable() : Context.Set<T>().Where(a => a.IsActive == true).AsQueryable();
            _resetSet = skipCount == 0 ? _resetSet.Take(size) : _resetSet.Skip(skipCount).Take(size);
            total = _resetSet.Count();

            return _resetSet.AsQueryable();
        }

        public virtual int GetCounts<T>(string filterExpression) where T : ModelBase
        {
            if (!String.IsNullOrWhiteSpace(filterExpression))
                return Context.Set<T>().AsQueryable().Where(filterExpression).Where(a => a.IsActive == true).Count();
            else
                return Context.Set<T>().AsQueryable().Where(a => a.IsActive == true).Count();
        }

        public virtual int GetCounts<T>(Expression<Func<T, bool>> filter) where T : ModelBase
        {
            return Context.Set<T>().AsQueryable().Where(filter).Where(a => a.IsActive == true).Count();
        }

        public virtual T Create<T>(T TObject, bool saveChanges = true) where T : ModelBase
        {
            TObject.CreatedDate = DateTime.Now;
            var newEntry = Context.Set<T>().Add(TObject);
            if (saveChanges)
                Context.SaveChanges();
            return TObject;
        }


        public virtual int Delete<T>(T TObject, bool saveChanges = true) where T : ModelBase
        {
            //Context.Set<T>().Remove(TObject);
            TObject.IsActive = false;
            this.Update(TObject, saveChanges);
            //if (saveChanges)
                //Context.SaveChanges();
            return TObject.Id;
        }


        public virtual int Update<T>(T TObject, bool saveChanges = true) where T : ModelBase
        {
            try
            {
                //var entry = Context.Entry(TObject);

                //Context.Set<T>().Attach(TObject);

                //entry.State = EntityState.Modified;

                


                if (TObject == null)
                {
                    throw new ArgumentException("Cannot add a null entity.");
                }

                var entry = Context.Entry<T>(TObject);

                if (entry.State == System.Data.Entity.EntityState.Detached)
                {
                    var set = Context.Set<T>();
                    T attachedEntity = set.Local.SingleOrDefault(e => e.Id == TObject.Id);  // You need to have access to key

                    if (attachedEntity != null)
                    {
                        var attachedEntry = Context.Entry(attachedEntity);
                        attachedEntry.CurrentValues.SetValues(TObject);
                    }
                    else
                    {
                        entry.State = System.Data.Entity.EntityState.Modified; // This should attach entity
                    }
                }

                if (saveChanges)
                    Context.SaveChanges();

                return TObject.Id;

            }


            catch (OptimisticConcurrencyException ex)
            {
                throw ex;
            }

        }


        public virtual int Delete<T>(Expression<Func<T, bool>> predicate, bool saveChanges = true) where T : ModelBase
        {
            var objects = Filter<T>(predicate);

            foreach (var obj in objects)
            {
                //Context.Set<T>().Remove(obj);
                obj.IsActive = false;
                this.Update(obj, false);
            }

            if (saveChanges)
                return Context.SaveChanges();
            return 0;

        }


        public
        bool Contains<T>(Expression<Func<T, bool>> predicate) where T : ModelBase
        {
            return Context.Set<T>().Where(a => a.IsActive == true).Count(predicate) > 0;
        }


        public virtual T Find<T>(params object[] keys) where T : ModelBase
        {
            T t = (T)Context.Set<T>().Find(keys);
            return t.IsActive ? t : null;
        }


        public virtual T Find<T>(Expression<Func<T, bool>> predicate) where T : ModelBase
        {
            return Context.Set<T>().Where(a => a.IsActive == true).FirstOrDefault(predicate);
        }


        public virtual IEnumerable<T> ExecuteProcedure<T>(String name, params SqlParameter[] param) where T : ModelBase
        {
            param = this.RemoveNullValues(param);
            string completeName = this.GetSPString(name, param);

            return Context.Database.SqlQuery<T>(completeName, param);
        }

        private SqlParameter[] RemoveNullValues(params SqlParameter[] param)
        {
            List<SqlParameter> newParam = new List<SqlParameter>();
            foreach (var p in param)
            {
                if (p.Value != null)
                {
                    newParam.Add(p);
                }
            }

            return newParam.ToArray();
        }

        private string GetSPString(String name, params SqlParameter[] param)
        {
            int i = 0;
            foreach (var p in param)
            {
                if (p.Value != null)
                {
                    name += (i == 0 ? " " : ", ") + p.ParameterName;
                    i++;
                }
            }
            return name;
        }


        public virtual void SaveChanges()
        {
            Context.SaveChanges();
        }




        public void Dispose()
        {
            if (Context != null)

                Context.Dispose();
        }

    }
}