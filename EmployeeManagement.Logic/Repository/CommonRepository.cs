using EmployeeManagement.Logic;
using EmployeeManagement.Model;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;

namespace EmployeeManagement.Logic
{
    //public class CommonRepository : ICommonRepository
    public class CommonRepository<T> : IDisposable where T : class
    {
        public CommonRepository(DbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException("Null DbContext");
            DbContext = dbContext;
            DbSet = DbContext.Set<T>();
        }
        protected DbContext DbContext { get; set; }
        protected DbSet<T> DbSet { get; set; }

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (DbContext != null)
                {
                    DbContext.Dispose();
                }
            }
        }
        #endregion

        public List<T> GetAll()
        {
            return DbSet.ToList();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Where(predicate).AsEnumerable();
        }

        public bool IsExist(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Any(predicate);
        }

        public virtual T GetById(int id)
        {
            return DbSet.Find(id);
        }

        public virtual int Count
        {
            get
            {
                return DbSet.Count();
            }
        }

        public virtual void Add(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                DbSet.Add(entity);
            }
            DbContext.SaveChanges();
        }

        //we check if the entity state is attached (DbContext knows about it) or not, if attached that means we can track the modifications, if not attached we have to attach it to our DbContext so we can track the new modification. Finally, we will change its status to modified to alert SaveChanges that we have changed entity
        public virtual void Update(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            dbEntityEntry.State = EntityState.Modified;
            DbContext.SaveChanges();
        }

        public virtual void Delete(IEnumerable<T> result)
        {
            DbSet.RemoveRange(result);
            DbContext.SaveChanges();
        }

        public virtual void Delete(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Deleted)
            {
                dbEntityEntry.State = EntityState.Deleted;
            }
            else
            {
                DbSet.Attach(entity);
                DbSet.Remove(entity);
            }
            DbContext.SaveChanges();
        }
        public virtual void Delete(int id)
        {
            var entity = GetById(id);
            if (entity == null) return; // not found; assume already deleted.
            Delete(entity);
            DbContext.SaveChanges();
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return DbSet.FirstOrDefault(predicate);
        }
        public IEnumerable<TResult> SelectMultipleResult<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector)
        {
            return DbSet.Where(predicate).Select(selector).AsEnumerable();
        }
        public IEnumerable<TResult> SelectMultipleResult<TResult>(Expression<Func<T, TResult>> selector)
        {
            return DbSet.Select(selector).AsEnumerable();
        }
        

        public void setModel(StringBuilder searchQ, CommonModel<T> model, List<string> colName, string sortFieldName, int page = 1, int pageSize = 10)
        {
            model.Filters = model.Filters == null ? new List<GridFilter>() : model.Filters;

            searchQ.Append(" WHERE ");
            searchQ.Append(CommonFunction.GenerateFilterCondition(model.Filters));
            
            if (!string.IsNullOrWhiteSpace(model.fieldName) && !string.IsNullOrWhiteSpace(model.sortOrder))
            {
                searchQ.Append(" ORDER BY " + model.fieldName + " " + model.sortOrder);
            }
            else
            {
                searchQ.Append(" ORDER BY " + sortFieldName);
            }

            foreach (var item in colName)
            {
                if (model.Filters != null)
                {
                    var obj = model.Filters.FirstOrDefault(d => d.ColumnName == item);
                    if (obj == null)
                    {
                        obj = new GridFilter { ColumnName = item };
                        model.Filters.Add(obj);
                    }
                }
            }

            model.Filters = model.Filters.OrderBy(d => colName.IndexOf(d.ColumnName)).ToList();
            int totalCount = 0;
            int pIndex = model.page.HasValue ? model.page.Value - 1 : 0;
            var list = GetDynamicList(searchQ.ToString(), ref totalCount, pIndex, pageSize).ToList();

            var pagedList = new StaticPagedList<dynamic>(list, page, pageSize, totalCount).GetMetaData();
            model.dynamicList = list;
            model.dynamicListMetaData = pagedList;

        }

        public void setModel(string searchQ, CommonModel<T> model, List<string> colName, int page = 1, int pageSize = 10)
        {
            model.Filters = model.Filters == null ? new List<GridFilter>() : model.Filters;
            foreach (var item in colName) //(var item in col)
            {
                if (model.Filters != null)
                {
                    var obj = model.Filters.FirstOrDefault(d => d.ColumnName == item);
                    if (obj == null)
                    {
                        obj = new GridFilter { ColumnName = item };
                        model.Filters.Add(obj);
                    }
                }
            }

            model.Filters = model.Filters.OrderBy(d => colName.IndexOf(d.ColumnName)).ToList();
            int totalCount = 0;
            int pIndex = model.page.HasValue ? model.page.Value - 1 : 0;
            var list = GetDynamicList(searchQ.ToString(), ref totalCount, pIndex, pageSize).ToList();

            var pagedList = new StaticPagedList<dynamic>(list, page, pageSize, totalCount).GetMetaData();
            model.dynamicList = list;
            model.dynamicListMetaData = pagedList;
        }

        public virtual List<dynamic> GetDynamicList(string query, ref int totalCount, int pageNo = 0, int? pageSize = null)
        {
            using (DataContext context = new DataContext())
            {
                query += "  OFFSET " + (pageNo * pageSize) + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY";

                using (var connection = new SqlConnection(context.Database.Connection.ConnectionString))//,
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    command.CommandType = System.Data.CommandType.Text;
                    connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        //totalCount = dt.Rows.Count;
                        //return dt.DynamicMapToList<dynamic>(Convert.ToInt32(pageNo), pageSize, true);
                        totalCount = (dt == null || dt.Rows.Count == 0 || dt.Rows[0]["rwcnt"] == null) ? 0 : Convert.ToInt32(dt.Rows[0]["rwcnt"].ToString());
                        dt.Columns.Remove("rwcnt");
                        return dt.DynamicMapToList<dynamic>(0, pageSize, true);//pageno=0 because always get pagesize wise data not get all data
                    }
                }
            }
        }

        public virtual DataTable GetDataTable(string query)
        {
            using (DataContext context = new DataContext())
            {
                using (var connection = new SqlConnection(context.Database.Connection.ConnectionString))//,
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    command.CommandType = System.Data.CommandType.Text;
                    connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

    }
}