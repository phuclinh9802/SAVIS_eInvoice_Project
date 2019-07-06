using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BASIC.AUTHEN.BUSSINESS
{
    /// <summary>
    /// Model dữ liệu hình thức thanh toán
    /// </summary>
    public class PaymentMethodModel
    {
        /// <summary>
        /// Id hình thức thanh toán
        /// </summary>
        public Guid PaymentMethodId { get; set; }
        /// <summary>
        /// Tên hình thức thanh toán
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Mã hình thức thanh toán
        /// </summary>
        public string Code { get; set; }
    }

    /// <summary>
    /// Model dữ liệu khách hàng
    /// </summary>
    public class CustomerModel
    {
        /// <summary>
        /// Id khách hàng
        /// </summary>
        public Guid CustomerId { get; set; }
        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Tên tài khoản ngân hàng
        /// </summary>
        public string BankAccountName { get; set; }
        /// <summary>
        /// Tên ngân hàng
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// Số tài khoản
        /// </summary>
        public string BankNumber { get; set; }
        /// <summary>
        /// Người đại diện
        /// </summary>
        public string ContactPerson { get; set; }
        /// <summary>
        /// Dịa chỉ
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Điện thoại
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Fax
        /// </summary>
        public string Fax { get; set; }
        /// <summary>
        /// Mã số thuế
        /// </summary>
        public string TaxNumber { get; set; }

    }

    /// <summary>
    /// Model dữ liệu đơn vị tính
    /// </summary>
    public class UnitModel
    {
        /// <summary>
        /// Id đơn vị tính
        /// </summary>
        public Guid UnitId { get; set; }
        /// <summary>
        /// Tên đơn vị tính
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Mã đơn vị tính
        /// </summary>
        public string Code { get; set; }
    }

    /// <summary>
    /// Model dữ liệu loại mặt hàng
    /// </summary>
    public class TypeOfProductModel
    {
        /// <summary>
        /// Id loại mặt hàng
        /// </summary>
        public Guid TypeOfProductId { get; set; }
        /// <summary>
        /// Tên loại mặt hàng
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Mã loại mặt hàng
        /// </summary>
        public string Code { get; set; }
    }

    /// <summary>
    /// Model dữ liệu mặt hàng
    /// </summary>
    public class ProductModel
    {
        /// <summary>
        /// Id mặt hàng
        /// </summary>
        public Guid ProductId { get; set; }
        /// <summary>
        /// Id loaij mặt hàng
        /// </summary>
        public Guid TypeOfProduct { get; set; }
        /// <summary>
        /// Id loại đơn vị tính
        /// </summary>
        public Guid Unit { get; set; }
        /// <summary>
        /// Phần trăm thuế
        /// </summary>
        public int VAT { get; set; }
        /// <summary>
        /// Tên mặt hàng
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// Mã mặt hàng
        /// </summary>
        public string ProductCode { get; set; }
    }

    /// <summary>
    /// Model dữ liệu dải hóa đơn mẫu đã đăng ký với thuế
    /// </summary>
    public class InvoiceTemplatePublishModel
    {
        /// <summary>
        /// Id đơn vị phát hành hóa đơn
        /// </summary>
        public Guid CompanyId { get; set; }
        /// <summary>
        /// Id loại hóa đơn
        /// </summary>
        public Guid InvoiceCategoryId { get; set; }
        /// <summary>
        /// Id mẫu hóa đơn
        /// </summary>
        public Guid InvoiceTemplateId { get; set; }
        /// <summary>
        /// Số seri mẫu hóa đơn
        /// </summary>
        public string InvoiceSeries { get; set; }
        /// <summary>
        /// Mã số mẫu hóa đơn
        /// </summary>
        public string InvoiceTemplateCode { get; set; }
    }

    /// <summary>
    /// Model dữ liệu trả về cho dịch vụ UpdateSatusInvoice
    /// </summary>
    public class DataOutputStatusPublishModel
    {
        /// <summary>
        /// Id hóa đơn PMKT
        /// </summary>
        public string IdInvoicePMKT { get; set; }
        /// <summary>
        /// Số hóa đơn
        /// </summary>
        public string InvoiceNumber { get; set; }
        /// <summary>
        /// Ngày hóa đơn
        /// </summary>
        public DateTime? VoucherDate { get; set; }
        /// <summary>
        /// Ngày phát hành
        /// </summary>
        public DateTime? PublishDate { get; set; }
        /// <summary>
        /// Kiểu hóa đơn 
        /// 1. Hóa đơn thông thường
        /// 2. Hóa đơn thay thế
        /// 3. Điều chỉnh tăng
        /// 4. Điều chỉnh giảm
        /// 5. Điều chỉnh thông tin
        /// </summary>
        public int? InvoiceType { get; set; }
        /// <summary>
        /// Trạng thái hóa đơn
        /// 1. Mới tạo lập
        /// 2. Đã phát hành
        /// 3. Bị thay thế
        /// 4. Bị điều chỉnh
        /// 5. Bị hủy bỏ 
        /// </summary>
        public int? InvoiceStatus { get; set; }
    }

    /// <summary>
    /// Model dữ liệu trả về cho dịch vụ CreatedInvoice, UpdateInvoice, DeleteInvoice
    /// </summary>
    public class DataOutputModel
    {
        /// <summary>
        /// Id hóa đơn trên HĐĐT
        /// </summary>
        public Guid InvoiceId { get; set; }
        /// <summary>
        /// Id hóa đơn trên PMKT
        /// </summary>
        public string IdInvoicePMKT { get; set; }
        /// <summary>
        /// Số hóa đơn
        /// </summary>
        public string InvoiceNumber { get; set; }
        /// <summary>
        /// Kiểu hóa đơn 
        /// 1. Hóa đơn thông thường
        /// 2. Hóa đơn thay thế
        /// 3. Điều chỉnh tăng
        /// 4. Điều chỉnh giảm
        /// 5. Điều chỉnh thông tin
        /// </summary>
        public int? InvoiceType { get; set; }
        /// <summary>
        /// Trạng thái hóa đơn
        /// 1. Mới tạo lập
        /// 2. Đã phát hành
        /// 3. Bị thay thế
        /// 4. Bị điều chỉnh
        /// 5. Bị hủy bỏ 
        /// </summary>
        public int? InvoiceStatus { get; set; }
        /// <summary>
        /// Trạng thái 1: Thành công, 0: Thất bại
        /// </summary>
        public int Status { get; set; }
    }
}
