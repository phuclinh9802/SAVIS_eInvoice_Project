using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BASIC.AUTHEN.BUSSINESS
{
    /// <summary>
    /// Model input cho việc đồng bộ dữ liệu khách hàng
    /// </summary>
    public class CustomerInputDataModel
    {
        /// <summary>
        /// Danh sách khách hàng
        /// </summary>
        public List<CustomerInputModel> Data { get; set; }
        /// <summary>
        /// Mã số thuế công ty nhập mặt hàng
        /// </summary>
        public string CompanyTax { get; set; }
        /// <summary>
        /// Id công ty nhập mặt hàng, lấy dữ liệu từ mã số thuế
        /// </summary>
        public Guid CompanyId { get; set; }
    }
    /// <summary>
    /// Model dữ liệu đầu vào cho việc đồng bộ khách hàng
    /// </summary>
    public class CustomerInputModel
    {
        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string CustomerCode { get; set; }
        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// Mã số thuế
        /// </summary>
        public string CustomerTax { get; set; }
        /// <summary>
        /// Số CMTND/Hộ chiếu/Căn cước công dân
        /// </summary>
        public string CustomerIdentity { get; set; }
        /// <summary>
        /// Địa chỉ khách hàng
        /// </summary>
        public string CustomerAddress { get; set; }
        /// <summary>
        /// Email khách hàng
        /// </summary>
        public string CustomerEmail { get; set; }
        /// <summary>
        /// Tên người mua hàng (người đại diện đơn vị để mua hàng)
        /// </summary>
        public string CustomerBuyer { get; set; }
        /// <summary>
        /// Tài khoản ngân hàng của khách hàng
        /// </summary>
        public string CustomerBankAccount { get; set; }
        /// <summary>
        /// Tên ngân hàng của khách hàng
        /// </summary>
        public string CustomerBankName { get; set; }
        /// <summary>
        /// Số điện thoại khách hàng
        /// </summary>
        public string CustomerPhone { get; set; }
        /// <summary>
        /// Loại khách hàng 1: Là đơn vị kế toán (công ty, doanh nghiệp) 2: Không là đơn vị kế toán (Cá nhân)
        /// </summary>
        public int CustomerType { get; set; }
        /// <summary>
        /// Mã số thuế
        /// </summary>
        public string CompanyTax { get; set; }
    }
    /// <summary>
    /// Model output sau khi đồng bộ khách hàng
    /// </summary>
    public class CustomerSyncOutputModel
    {
        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string CustomerCode { get; set; }
        /// <summary>
        /// Trạng thái lưu trên HĐĐT
        /// </summary>
        public int Status { get; set; }
    }


    /// <summary>
    /// Model input cho việc đồng bộ dữ liệu mặt hàng
    /// </summary>
    public class ProductInputDataModel
    {
        /// <summary>
        /// Danh sách mặt hàng
        /// </summary>
        public List<ProductInputModel> Data { get; set; }
        /// <summary>
        /// Mã số thuế công ty nhập mặt hàng
        /// </summary>
        public string CompanyTax { get; set; }
        /// <summary>
        /// Id công ty nhập mặt hàng, lấy dữ liệu từ mã số thuế
        /// </summary>
        public Guid CompanyId { get; set; }
    }
    /// <summary>
    /// Model dữ liệu đầu vào cho việc đồng bộ mặt hàng
    /// </summary>
    public class ProductInputModel
    {
        /// <summary>
        /// Mã mặt hàng
        /// </summary>
        public string ProductCode { get; set; }
        /// <summary>
        /// Tên mặt hàng
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string UnitName { get; set; }
        /// <summary>
        /// Thuế suất 0: 0%; 5: 5%; 10: 10%; -1: Không thuế suất
        /// </summary>
        public decimal VAT { get; set; }
        /// <summary>
        /// Đơn giá mặt hàng
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// Mô tả
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Mã số thuế công ty
        /// </summary>
        public string CompanyTax { get; set; }
    }

    /// <summary>
    /// Model output sau khi đồng bộ mặt hàng
    /// </summary>
    public class ProductSyncOutputModel
    {
        /// <summary>
        /// Mã mặt hàng từ PMKT
        /// </summary>
        public string ProductCode { get; set; }
        /// <summary>
        /// Trạng thái lưu trên HĐĐT
        /// </summary>
        public int Status { get; set; }
    }
}
