using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;


namespace BASIC.AUTHEN.DATA
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Lấy về tổng số
        /// </summary>
        int Count { get; }


        /// <summary>
        /// Lấy về tất cả đối tượng
        /// </summary>
        IQueryable<T> GetAll();

        /// <summary>
        /// Lấy về tất cả đối tượng
        /// </summary>
        IQueryable<T> GetAll(Expression<Func<T, string>> orderByProperty, bool isAscendingOrder);

        /// <summary>
        /// Lấy về đối tượng theo mã đối tượng
        /// </summary>
        /// <param name="id">mã đối tượng</param>        
        /// <remarks>
        /// Mã đối tượng kiểu unique
        /// </remarks>
        T GetById(System.Guid Id);

        /// <summary>
        /// Lấy về đối tượng theo mã đối tượng
        /// </summary>
        /// <param name="id">mã đối tượng</param>        
        /// <remarks>
        /// Mã đối tượng kiểu chuỗi
        /// </remarks>
        T GetById(string Id);

        /// <summary>
        /// Lấy về đối tượng đầu tiên theo tiêu chí tìm kiếm
        /// </summary>
        /// <param name="where">tiêu chí tìm kiếm</param>    
        T Get(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Lấy về các đối tượng theo tiêu chí tìm kiếm, thứ tự sắp xếp, các thuộc tính
        /// </summary>
        /// <param name= "filter"> </param>
        /// <param name= "orderBy"> </param>
        /// <param name= "includeProperties"> </param>
        /// <returns> </returns>
        IQueryable<T> GetMany(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int Count = 0, string includeProperties = "");

        /// <summary>
        /// Lấy về các đối tượng theo tiêu chí tìm kiếm
        /// </summary>
        /// <param name="where">tiêu chí tìm kiếm</param>
        /// <remarks>
        /// where: Bieu thuc linq
        /// </remarks>
        IQueryable<T> GetMany(Expression<Func<T, bool>> predicate);

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
        //IQueryable<T> GetPageMany<T, TResult>(IQueryable<T> query,
        //               Expression<Func<T, TResult>> orderByProperty = null, bool isAscendingOrder = true, int pageNum = 0, int pageSize = 20);

        IQueryable<T> GetPageMany(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, int pageNum = 0, int pageSize = 20);

        /// <summary>
        /// Thêm mới đối tượng
        /// </summary>
        /// <param name="entity">đối tượng được thêm mới</param>        
        /// <remarks>
        /// </remarks>
        T Add(T entity);

        /// <summary>
        /// Cập nhật đối tượng
        /// </summary>
        /// <param name="entity">đối tượng được cập nhật</param>        
        /// <remarks>
        /// </remarks>
        void Update(T entity);

        /// <summary>
        /// Xóa đối tượng
        /// </summary>
        /// <param name="entity">đối tượng bị xóa</param>        
        /// <remarks>
        /// </remarks>
        void Delete(T entity);

        /// <summary>
        /// Xóa đối tượng dựa theo các điều kiện
        /// </summary>
        /// <param name="where">điều kiện</param>        
        /// <remarks>
        /// </remarks>
        void Delete(Expression<Func<T, bool>> predicate);

    }
}
