using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BASIC.AUTHEN.BUSSINESS
{
    /// <summary>
    /// Model dữ liệu đầu vào cho dịch vụ GetInvoicePdfUri
    /// </summary>
    public class InvoicePdfInputModel
    {
        /// <summary>
        /// Loại mẫu in 0: Bản thể, 1: Chuyển đổi
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// Id hóa đơn trên PMKT
        /// </summary>
        public string IdInvoicePMKT { get; set; }
    }

    public class ReviceDataFromCoreModel
    {
        public string Data { get; set; }
        public int TotalCount { get; set; }
        public int DataCount { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// Model dữ liệu đầu vào cho dịch vụ ChangeInvoice
    /// </summary>
    public class ChangeInvoiceModel
    {
        /// <summary>
        /// Thông tin hóa đơn
        /// </summary>
        public InvoiceModel DataInvoice { get; set; }
        /// <summary>
        /// Loại xử lý hóa đơn
        /// 1: Điều chỉnh tăng
        /// 2: Điều chỉnh giảm
        /// 3: Điều chỉnh thông tin
        /// 4: Thay thế hóa đơn
        /// 5: Hủy bỏ hóa đơn
        /// </summary>
        public int ChangeType { get; set; }
    }

    /// <summary>
    /// Model dữ liệu hóa đơn dạng base64 input
    /// </summary>
    public class InputModelBase64
    {
        public string DataBase64 { get; set; }
    }

    /// <summary>
    /// Model dữ liệu đầu vào cho dịch vụ CreatedInvoice
    /// </summary>
    public class InvoiceModel
    {
        /// <summary>
        /// Mã số thuế công ty phát hành hóa đơn
        /// </summary>
        public string CompanyTax { get; set; }
        /// <summary>
        /// Id loại hóa đơn
        /// </summary>
        public Guid? InvoiceCategoryId { get; set; }
        /// <summary>
        /// Id mẫu hóa đơn
        /// </summary>
        public Guid? InvoiceTemplateId { get; set; }
        /// <summary>
        /// Id công ty phát hành
        /// </summary>
        public Guid? CompanyId { get; set; }
        /// <summary>
        /// Số seri mẫu hóa đơn
        /// </summary>
        public string InvoiceSeries { get; set; }
        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string CustomerCode { get; set; }
        /// <summary>
        /// Id khách hàng
        /// </summary>
        public Guid? CustomerId { get; set; }
        /// <summary>
        /// Ngày phát hành hóa đơn
        /// </summary>
        public string PublishDate { get; set; }
        /// <summary>
        /// Hình thức thanh toán
        /// </summary>
        public string PaymentMethod { get; set; }
        /// <summary>
        /// Mã số mẫu hóa đơn
        /// </summary>
        public string TemplateCode { get; set; }
        /// <summary>
        /// Ngày tạo lập hóa đơn
        /// </summary>
        public string InvoiceIssuedDate { get; set; }
        /// <summary>
        /// Mã tiền tệ
        /// </summary>
        public string CurrencyCode { get; set; }
        /// <summary>
        /// Ghi chú thêm cho hóa đơn
        /// </summary>
        public string InvoiceNote { get; set; }
        /// <summary>
        /// Id hóa đơn gốc từ PMKT, đối với trường hợp Gọi dịch vụ ChangeInvoice  với tham số Type = 1,2,3,4
        /// </summary>
        public string OriginalInvoiceId { get; set; }
        /// <summary>
        /// Họ tên người mua hàng
        /// </summary>
        public string BuyerDisplayName { get; set; }
        /// <summary>
        /// Tên đơn vị mua hàng
        /// </summary>
        public string BuyerLegalName { get; set; }
        /// <summary>
        /// Mã số thuế đơn vị mua hàng
        /// </summary>
        public string BuyerTaxCode { get; set; }
        /// <summary>
        /// Địa chỉ chi tiết bên mua hàng
        /// </summary>
        public string BuyerAddressLine { get; set; }
        /// <summary>
        /// Số điện thoại bên mua hàng
        /// </summary>
        public string BuyerPhoneNumber { get; set; }
        /// <summary>
        /// Fax bên mua hàng
        /// </summary>
        public string BuyerFaxNumber { get; set; }
        /// <summary>
        /// Email bên mua hàng
        /// </summary>
        public string BuyerEmail { get; set; }
        /// <summary>
        /// Tên ngân hàng bên mua hàng
        /// </summary>
        public string BuyerBankName { get; set; }
        /// <summary>
        /// Tài khoàn ngân hàng bên mua hàng
        /// </summary>
        public string BuyerBankAccount { get; set; }
        /// <summary>
        /// Phần trăm thuế GTGT
        /// </summary>
        public decimal? VatPercentage { get; set; }
        /// <summary>
        /// Tổng giá trị hóa đơn
        /// </summary>
        public decimal? TotalAmount { get; set; }
        /// <summary>
        /// Tổng tiền thuế GTGT
        /// </summary>
        public decimal? TotalAmountVAT { get; set; }
        /// <summary>
        /// Tỷ giá ngoại tệ
        /// </summary>
        public decimal? ExchangeRate { get; set; }
        /// <summary>
        /// Số tiền quy đổi
        /// </summary>
        public decimal? AmountExchanged { get; set; }
        /// <summary>
        /// Danh sách mặt hàng theo hóa đơn
        /// </summary>
        public List<ProductInvoiceModel> ListProductInvoice { get; set; }
        /// <summary>
        /// Id hóa đơn hiện tại trên PMKT
        /// </summary>
        public string IdInvoicePMKT { get; set; }
        #region Thông tin trường đặc thù theo ngành nghề
        /// <summary>
        /// Sử dụng từ ngày (hóa đơn điện, nước..v..v)
        /// </summary>
        public string FromUseDate { get; set; }
        /// <summary>
        /// Sử dụng đến ngày (hóa đơn điện, nước...v.v..)
        /// </summary>
        public string ToUseDate { get; set; }
        /// <summary>
        /// Thuế môi trường
        /// </summary>
        public double? EnvironmentTax { get; set; }
        /// <summary>
        /// Phí môi trường
        /// </summary>
        public decimal? EnvironmentFee { get; set; }
        /// <summary>
        /// Số đầu kỳ
        /// </summary>
        public double? StartNumber { get; set; }
        /// <summary>
        /// Số cuối kỳ
        /// </summary>
        public double? EndNumber { get; set; }
        /// <summary>
        /// Số sử dụng trong kỳ
        /// </summary>
        public double? UseNumber { get; set; }
        #endregion
    }

    public class ProductInvoiceDataModel
    {
        public List<ProductInvoiceModel> Data { get; set; }
    }

    /// <summary>
    /// Model dữ liệu mặt hàng theo hóa đơn
    /// </summary>
    public class ProductInvoiceModel
    {
        /// <summary>
        /// Id mặt hàng
        /// </summary>
        public Guid? ProductId { get; set; }
        /// <summary>
        /// Mã mặt hàng
        /// </summary>
        public string ProductCode { get; set; }
        /// <summary>
        /// Thứ tự mặt hàng
        /// </summary>
        public int? LineNumber { get; set; }
        /// <summary>
        /// Tên mặt hàng
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// Đơn giá mặt hàng
        /// </summary>
        public decimal? UnitPrice { get; set; }
        /// <summary>
        /// Số lượng mặt hàng
        /// </summary>
        public decimal? Quantity { get; set; }
        /// <summary>
        /// Thành tiền mặt hàng
        /// </summary>
        public decimal? TotalAmount { get; set; }
        /// <summary>
        /// Thuế GTGT áp cho mặt hàng
        /// </summary>
        public decimal? VatPercentage { get; set; }
        /// <summary>
        /// Loại mặt hàng trên hóa đơn
        /// </summary>
        public int? ItemType { get; set; }
        /// <summary>
        /// Số lô hàng (hàng hóa dược phẩm)
        /// </summary>
        public string LotNumber { get; set; }
        /// <summary>
        /// Ngày hết hạn của mặt hàng
        /// </summary>
        public string ExpirationDate { get; set; }
        /// <summary>
        /// Tên đơn vị tính
        /// </summary>
        public string UnitName { get; set; }
    }
}
