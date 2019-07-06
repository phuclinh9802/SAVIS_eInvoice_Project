using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data;
using System.Linq.Expressions;
//using System.Linq.Dynamic;

namespace BASIC.AUTHEN.DATA
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private EFEntities _dataContext;
        private readonly IDbSet<T> _dbset;
        public Repository(IDatabaseFactory databaseFactory)
        {
            DatabaseFactory = databaseFactory;
            _dbset = DataContext.Set<T>();
        }

        public Repository(EFEntities dataContext)
        {
            _dataContext = dataContext;
            _dbset = _dataContext.Set<T>();
        }
        protected IDatabaseFactory DatabaseFactory
        {
            get;
            private set;
        }

        protected EFEntities DataContext
        {
            get { return _dataContext ?? (_dataContext = DatabaseFactory.GetDbContext()); }
        }

        /// <summary>
        /// Lấy về tổng số
        /// </summary>
        public virtual int Count
        {
            get { return _dbset.Count(); }
        }

        /// <summary>
        /// Lấy về tất cả đối tượng
        /// </summary>
        public virtual IQueryable<T> GetAll()
        {
            return _dbset.AsQueryable();
        }

        /// <summary>
        /// Lấy về tất cả đối tượng
        /// </summary>
        public virtual IQueryable<T> GetAll(Expression<Func<T, string>> orderByProperty, bool isAscendingOrder)
        {
            var resetSet = _dbset.AsQueryable();

            resetSet = isAscendingOrder ? resetSet.OrderBy(orderByProperty) : resetSet.OrderByDescending(orderByProperty);

            //Skip the required rows for the current page and take the next records of pagesize count
            return resetSet;
        }

        /// <summary>
        /// Lấy về đối tượng theo mã đối tượng
        /// </summary>
        /// <param name="id">mã đối tượng</param>        
        /// <remarks>
        /// Mã đối tượng kiểu unique
        /// </remarks>
        public virtual T GetById(System.Guid id)
        {
            return _dbset.Find(id);
        }

        /// <summary>
        /// Lấy về đối tượng theo mã đối tượng
        /// </summary>
        /// <param name="id">mã đối tượng</param>        
        /// <remarks>
        /// Mã đối tượng kiểu chuỗi
        /// </remarks>
        public virtual T GetById(string id)
        {
            return _dbset.Find(id);
        }

        /// <summary>
        /// Lấy về đối tượng đầu tiên theo tiêu chí tìm kiếm
        /// </summary>
        /// <param name="where">tiêu chí tìm kiếm</param>    
        public T Get(Expression<Func<T, bool>> predicate)
        {
            return _dbset.Where(predicate).FirstOrDefault<T>();
        }

        /// <summary>
        /// Lấy về các đối tượng theo tiêu chí tìm kiếm, thứ tự sắp xếp, các thuộc tính
        /// </summary>
        /// <param name= "filter"> </param>
        /// <param name= "orderBy"> </param>
        /// <param name= "includeProperties"> </param>
        /// <returns> </returns>
        public virtual IQueryable<T> GetMany(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int Count = 0, string includeProperties = "")
        {
            var query = filter != null ? _dbset.Where(filter) : _dbset;
            if (!String.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (Count > 0)
            {
                query = query.Take(Count);
            }
            return query;
        }

        /// <summary>
        /// Lấy về các đối tượng theo tiêu chí tìm kiếm
        /// </summary>
        /// <param name="where">tiêu chí tìm kiếm</param>  
        /// <remarks>
        /// where: bieu thuc linq
        /// </remarks>
        public virtual IQueryable<T> GetMany(Expression<Func<T, bool>> predicate)
        {
            return _dbset.Where(predicate);
        }

        /// <summary>
        /// Pages the specified query.
        /// </summary>
        /// <typeparam name="T">Generic Type Object</typeparam>
        /// <param name="filter">The Object query where paging needs to be applied.</param>
        /// <param name="orderByProperty">The order by property.</param>
        /// <param name="isAscendingOrder">if set to <c>true</c> [is ascending order].</param>
        /// <param name="rowsCount">The total rows count.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        //public virtual IQueryable<T> GetPageMany<T, TResult>(IQueryable<T> query, 
        //        Expression<Func<T, TResult>> orderByProperty = null, bool isAscendingOrder = true, int pageNum = 0,  int pageSize = 20)
        //{
        //    if (pageSize <= 0) pageSize = 20;

        //    //Total result count
        //    //rowsCount = query.Count();

        //    //If page number should be > 0 else set to first page
        //    //if (rowsCount <= pageSize || pageNum <= 0) pageNum = 1;

        //    //Calculate nunber of rows to skip on pagesize
        //    int excludedRows = (pageNum - 1) * (pageSize - 1);

        //    if (excludedRows <= 0) excludedRows = 0;
        //    if (orderByProperty !=null)
        //    {
        //        query = isAscendingOrder ? query.OrderBy(orderByProperty) : query.OrderByDescending(orderByProperty);
        //    }           

        //    //Skip the required rows for the current page and take the next records of pagesize count
        //    return query.Skip(excludedRows).Take(pageSize);
        //}

        public virtual IQueryable<T> GetPageMany(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, int pageNum = 0, int pageSize = 20)
        {
            if (pageSize <= 0) pageSize = 20;
            var query = filter != null ? _dbset.Where(filter) : _dbset;
            ////Total result count
            //rowsCount = resetSet.Count();

            ////If page number should be > 0 else set to first page
            //if (rowsCount <= pageSize || pageNum <= 0) pageNum = 1;

            //Calculate nunber of rows to skip on pagesize
            int excludedRows = (pageNum - 1) * (pageSize - 1);
            if (excludedRows <= 0) excludedRows = 0;

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            //Skip the required rows for the current page and take the next records of pagesize count
            return query.Skip(excludedRows).Take(pageSize);
        }

        /// <summary>
        /// Thêm mới đối tượng
        /// </summary>
        /// <param name="entity">đối tượng được thêm mới</param>        
        /// <remarks>
        /// </remarks>
        public virtual T Add(T entity)
        {
            return _dbset.Add(entity);
        }

        /// <summary>
        /// Cập nhật đối tượng
        /// </summary>
        /// <param name="entity">đối tượng được cập nhật</param>        
        /// <remarks>
        /// </remarks>
        public virtual void Update(T entity)
        {
            _dbset.Attach(entity);
            _dataContext.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// Xóa đối tượng
        /// </summary>
        /// <param name="entity">đối tượng bị xóa</param>        
        /// <remarks>
        /// </remarks>
        public virtual void Delete(T entity)
        {
            _dbset.Remove(entity);
        }

        /// <summary>
        /// Xóa đối tượng dựa theo các điều kiện
        /// </summary>
        /// <param name="where">điều kiện</param>        
        /// <remarks>
        /// </remarks>
        public virtual void Delete(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> objects = _dbset.Where<T>(predicate);
            foreach (T obj in objects)
                _dbset.Remove(obj);
        }
    }
}
