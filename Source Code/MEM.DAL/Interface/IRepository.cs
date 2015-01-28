using MEM.Domain.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MEM.DAL.Interface
{
    public interface IRepository
    {
        void Initialize();

        T Single<T>(Expression<Func<T, bool>> expression) where T : ModelBase;
        IQueryable<T> All<T>() where T : ModelBase;
        IQueryable<T> Filter<T>(string filterExpression, string sortExpression, string sortDirection, int pageIndex, int pageSize, int pagesCount) where T : ModelBase;
        IQueryable<T> Filter<T>(string filterExpression, Expression<Func<T, bool>> predicate, string sortExpression, string sortDirection, int pageIndex, int pageSize, int pagesCount) where T : ModelBase;
        IQueryable<T> Filter<T>(string filterExpression) where T : ModelBase;
        IQueryable<T> Filter<T>(Expression<Func<T, bool>> predicate) where T : ModelBase;
        IQueryable<T> Filter<T>(Expression<Func<T, bool>> filter, out int total, int index = 0, int size = 50) where T : ModelBase;
        int GetCounts<T>(string filterExpression) where T : ModelBase;
        int GetCounts<T>(Expression<Func<T, bool>> filter) where T : ModelBase;
        T Create<T>(T TObject, bool saveChanges = true) where T : ModelBase;
        int Delete<T>(T TObject, bool saveChanges = true) where T : ModelBase;
        int Update<T>(T TObject, bool saveChanges = true) where T : ModelBase;
        int Delete<T>(Expression<Func<T, bool>> predicate, bool saveChanges = true) where T : ModelBase;
        bool Contains<T>(Expression<Func<T, bool>> predicate) where T : ModelBase;
        T Find<T>(params object[] keys) where T : ModelBase;
        T Find<T>(Expression<Func<T, bool>> predicate) where T : ModelBase;
        IEnumerable<T> ExecuteProcedure<T>(String name, params SqlParameter[] param) where T : ModelBase;
        T ForceSingleFromDB<T>(Expression<Func<T, bool>> expression) where T : ModelBase;

        void SaveChanges();
        void Dispose();



    }
}
