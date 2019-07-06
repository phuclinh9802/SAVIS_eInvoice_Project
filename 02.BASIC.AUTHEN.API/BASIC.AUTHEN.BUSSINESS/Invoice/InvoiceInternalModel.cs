using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BASIC.AUTHEN.BUSSINESS
{
    public class CreateInvoiceModel
    {
        public Guid? InvoiceCategoryId { get; set; }
        public Guid? InvoiceTemplateId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? CustomerId { get; set; }
        public string InvoiceCode { get; set; }
        public string Data { get; set; }
        public string Signature { get; set; }
        public int? Converted { get; set; }
        public string KindOfService { get; set; }
        public int? PaymentStatus { get; set; }
        public DateTime? ArisingDate { get; set; }
        public string ProcessInvoiceNote { get; set; }
        public decimal? GrossValue { get; set; }
        public decimal? VatAmount0 { get; set; }
        public decimal? GrossValue0 { get; set; }
        public decimal? VatAmount5 { get; set; }
        public decimal? GrossValue5 { get; set; }
        public decimal? VatAmount10 { get; set; }
        public decimal? GrossValue10 { get; set; }
        public bool? Certified { get; set; }
        public string CertifiedId { get; set; }
        public string CertifiedData { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public Guid? CreationBy { get; set; }
        public Guid? PublishBy { get; set; }
        public Guid? PaymentMethod { get; set; }
        public string SellerAppRecordId { get; set; }
        public string InvoiceAppRecordId { get; set; }
        public string InvoiceType { get; set; }
        public string TemplateCode { get; set; }
        public string InvoiceSeries { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceName { get; set; }
        public DateTime? InvoiceIssuedDate { get; set; }
        public DateTime? SignedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string ContractNumber { get; set; }
        public DateTime? ContractDate { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? AmountExchanged { get; set; }
        public string InvoiceNote { get; set; }
        public int? AdjustmentType { get; set; }
        public Guid? OriginalInvoiceId { get; set; }
        public string AdditionalReferenceDesc { get; set; }
        public DateTime? AdditionalReferenceDate { get; set; }
        public string SellerLegalName { get; set; }
        public string SellerTaxCode { get; set; }
        public string SellerAddressLine { get; set; }
        public string SellerPostalCode { get; set; }
        public string SellerDistrictName { get; set; }
        public string SellerCityName { get; set; }
        public string SellerCountryCode { get; set; }
        public string SellerPhoneNumber { get; set; }
        public string SellerFaxNumber { get; set; }
        public string SellerEmail { get; set; }
        public string SellerBankName { get; set; }
        public string SellerBankAccount { get; set; }
        public string SellerContactPersonName { get; set; }
        public string SellerSignedPersonName { get; set; }
        public string SellerSubmittedPersonName { get; set; }
        public string BuyerDisplayName { get; set; }
        public string BuyerLegalName { get; set; }
        public string BuyerTaxCode { get; set; }
        public string BuyerAddressLine { get; set; }
        public string BuyerPostalCode { get; set; }
        public string BuyerDistrictName { get; set; }
        public string BuyerCityName { get; set; }
        public string BuyerCountryCode { get; set; }
        public string BuyerPhoneNumber { get; set; }
        public string BuyerFaxNumber { get; set; }
        public string BuyerEmail { get; set; }
        public string BuyerBankName { get; set; }
        public string BuyerBankAccount { get; set; }
        public decimal? SumOfTotalLineAmountWithoutVat { get; set; }
        public decimal? TotalAmountWithoutVat { get; set; }
        public decimal? TotalVatamount { get; set; }
        public decimal? TotalAmountWithVat { get; set; }
        public decimal? TotalAmountWithVatfrn { get; set; }
        public string TotalAmountWithVatinWords { get; set; }
        public bool? IsTotalAmountPos { get; set; }
        public bool? IsTotalVatamountPos { get; set; }
        public bool? IsTotalAmtWithoutVatPos { get; set; }
        public decimal? DiscountAmount { get; set; }
        public bool? IsDiscountAmtPos { get; set; }
        public string ProductCodes { get; set; }
        public string PortalLink { get; set; }
        public int? InvoiceStatus { get; set; }
        public bool? TrangThaiXemTraCuuHoaDon { get; set; }
        public decimal? VatPercentage { get; set; }
        public bool? ConvertedStatus { get; set; }
        public DateTime? ConvertedDate { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public bool? IsPublishInvoice { get; set; }
        public string HtmlContent { get; set; }
        public string PdfUrl { get; set; }
        #region Thông tin trường đặc thù theo ngành nghề
        /// <summary>
        /// Sử dụng từ ngày (hóa đơn điện, nước..v..v)
        /// </summary>
        public DateTime? FromUseDate { get; set; }
        /// <summary>
        /// Sử dụng đến ngày (hóa đơn điện, nước...v.v..)
        /// </summary>
        public DateTime? ToUseDate { get; set; }
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
        public List<ProductInvoiceInternalModel> ListProductInvoice { get; set; }
    }

    public class ProductInvoiceInternalModel
    {
        public Guid? ProductInvoiceId { get; set; }
        public Guid? InvoiceId { get; set; }
        public int? LineNumber { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ItemTotalAmountWithoutVat { get; set; }
        public decimal? VatPercentage { get; set; }
        public decimal? VatAmount { get; set; }
        public bool? Promotion { get; set; }
        public decimal? AdjustmentVatAmount { get; set; }
        public bool? IsIncreaseItem { get; set; }
        public decimal? ItemTotalAmountWithVat { get; set; }
        public string LotNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? ItemType { get; set; }
    }

    public class BasicInvoiceModel
    {
        public InvoiceSigningResult InvoiceSigningResult { get; set; }
        public Guid InvoiceId { get; set; }
        public Guid? InvoiceCategoryId { get; set; }
        public Guid? InvoiceTemplateId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? CustomerId { get; set; }
        public string InvoiceCode { get; set; }
        public string InvoiceCodeOther { get; set; }
        public string Data { get; set; }
        public string Signature { get; set; }
        public int? Converted { get; set; }
        public string KindOfService { get; set; }
        public int? PaymentStatus { get; set; }
        public DateTime? ArisingDate { get; set; }
        public string ProcessInvoiceNote { get; set; }
        public decimal? GrossValue { get; set; }
        public decimal? VatAmount0 { get; set; }
        public decimal? GrossValue0 { get; set; }
        public decimal? VatAmount5 { get; set; }
        public decimal? GrossValue5 { get; set; }
        public decimal? VatAmount10 { get; set; }
        public decimal? GrossValue10 { get; set; }
        public bool? Certified { get; set; }
        public string CertifiedId { get; set; }
        public string CertifiedData { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public Guid? CreationBy { get; set; }
        public Guid? PublishBy { get; set; }
        public Guid? PaymentMethod { get; set; }
        public string SellerAppRecordId { get; set; }
        public string InvoiceAppRecordId { get; set; }
        public string InvoiceType { get; set; }
        public string TemplateCode { get; set; }
        public string TemplateName { get; set; }
        public string InvoiceSeries { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceName { get; set; }
        public DateTime? InvoiceIssuedDate { get; set; }
        public DateTime? SignedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string ContractNumber { get; set; }
        public DateTime? ContractDate { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? AmountExchanged { get; set; }
        public string InvoiceNote { get; set; }
        public int? AdjustmentType { get; set; }
        public Guid? OriginalInvoiceId { get; set; }
        public string AdditionalReferenceDesc { get; set; }
        public DateTime? AdditionalReferenceDate { get; set; }
        public string SellerLegalName { get; set; }
        public string SellerTaxCode { get; set; }
        public string SellerAddressLine { get; set; }
        public string SellerPostalCode { get; set; }
        public string SellerDistrictName { get; set; }
        public string SellerCityName { get; set; }
        public string SellerCountryCode { get; set; }
        public string SellerPhoneNumber { get; set; }
        public string SellerFaxNumber { get; set; }
        public string SellerEmail { get; set; }
        public string SellerBankName { get; set; }
        public string SellerBankAccount { get; set; }
        public string SellerContactPersonName { get; set; }
        public string SellerSignedPersonName { get; set; }
        public string SellerSubmittedPersonName { get; set; }
        public string BuyerDisplayName { get; set; }
        public string BuyerLegalName { get; set; }
        public string BuyerTaxCode { get; set; }
        public string BuyerAddressLine { get; set; }
        public string BuyerPostalCode { get; set; }
        public string BuyerDistrictName { get; set; }
        public string BuyerCityName { get; set; }
        public string BuyerCountryCode { get; set; }
        public string BuyerPhoneNumber { get; set; }
        public string BuyerFaxNumber { get; set; }
        public string BuyerEmail { get; set; }
        public string BuyerBankName { get; set; }
        public string BuyerBankAccount { get; set; }
        public decimal? SumOfTotalLineAmountWithoutVat { get; set; }
        public decimal? TotalAmountWithoutVat { get; set; }
        public decimal? TotalVatamount { get; set; }
        public decimal? TotalAmountWithVat { get; set; }
        public decimal? TotalAmountWithVatfrn { get; set; }
        public string TotalAmountWithVatinWords { get; set; }
        public bool? IsTotalAmountPos { get; set; }
        public bool? IsTotalVatamountPos { get; set; }
        public bool? IsTotalAmtWithoutVatPos { get; set; }
        public decimal? DiscountAmount { get; set; }
        public bool? IsDiscountAmtPos { get; set; }
        public string ProductCodes { get; set; }
        public string PortalLink { get; set; }
        public int? InvoiceStatus { get; set; }
        public bool? TrangThaiXemTraCuuHoaDon { get; set; }
        public decimal? VatPercentage { get; set; }
        public bool? ConvertedStatus { get; set; }
        public DateTime? ConvertedDate { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public bool? IsPublishInvoice { get; set; }
        public BasicInvoiceTemplateModel InvoiceTemplate { get; set; }
        // Added info field
        public string PublisherName { get; set; }
        public string PaymentMethodName { get; set; }
        public List<ProductInvoiceInternalModel> ListProductInvoice { get; set; }
        public OriginalInvoiceModel OriginalInvoice { get; set; }
        /// <summary>
        /// Trạng thái đánh dấu có phải hóa đơn cách dải không (True: Là hóa đơn cách dải; False: Không là hóa đơn cách dải)
        /// </summary>
        public bool? IsSkipped { get; set; }
        /// <summary>
        /// Id cách dải hóa đơn (có dữ liệu khi hóa đơn này là hóa đơn cách dải)
        /// </summary>
        public Guid? SkippedInvoiceId { get; set; }
        /// <summary>
        /// Trạng thái đóng mở control date Ngày tạo hóa đơn cho client
        /// </summary>
        public bool? InvoiceIssuedDateOpened { get; set; }
        /// <summary>
        /// Trạng thái đóng mở control date Ngày phát hành hóa đơn cho client
        /// </summary>
        public bool? InvoicePublishDateOpened { get; set; }
        /// <summary>
        /// Check điều kiện cho phép phát hành (Chỉ cho phép phát hành khi Ngày hóa đơn nhỏ hơn hoặc bằng Ngày phát hành)
        /// </summary>
        public bool? HasPublish { get; set; }
        /// <summary>
        /// Đường dẫn tải về file biên bản thay thế hóa đơn
        /// </summary>
        public string DownloadUrl { get; set; }
        /// <summary>
        /// Đường dẫn file biên bản thay thế hóa đơn lưu DB
        /// </summary>
        public string FileReportUrl { get; set; }
        /// <summary>
        /// Tên file biên bản thay thế hóa đơn lưu DB
        /// </summary>
        public string FileReportName { get; set; }
        /// <summary>
        /// Kiểm tra xem công ty đã ký biên bản chưa True: Đã ký; False: Chưa ký
        /// </summary>
        public bool? IsCompanySignReport { get; set; }
        /// <summary>
        /// Kiểm tra xem khách hàng đã ký biên bản chưa Trua: Đã ký; False: Chưa ký
        /// </summary>
        public bool? IsCustomerSignReport { get; set; }
        /// <summary>
        /// Đường dẫn tải về file biên bản điều chỉnh hóa đơn
        /// </summary>
        public string ChangeDownloadUrl { get; set; }
        /// <summary>
        /// Đường dẫn file biên bản điều chỉnh hóa đơn
        /// </summary>
        public string ChangeReportUrl { get; set; }
        /// <summary>
        /// Tên file biên bản điều chỉnh
        /// </summary>
        public string ChangeReportFileName { get; set; }
        /// <summary>
        /// Kiểm tra xem công ty đã ký biên bản điều chỉnh chưa True: Đã ký; False: Chưa ký
        /// </summary>
        public bool? IsCompanySignReportChange { get; set; }
        /// <summary>
        /// Kiểm tra xem khách hàng đã ký biên bản điều chỉnh chưa Trua: Đã ký; False: Chưa ký
        /// </summary>
        public bool? IsCustomerSignReportChange { get; set; }
        /// <summary>
        /// Đường dẫn tải về file biên bản hủy hóa đơn
        /// </summary>
        public string DeleteDownloadUrl { get; set; }
        /// <summary>
        /// Đường dẫn file biên bản hủy hóa đơn
        /// </summary>
        public string DeleteReportUrl { get; set; }
        /// <summary>
        /// Tên file biên bản hủy
        /// </summary>
        public string DeleteReportFileName { get; set; }
        /// <summary>
        /// Kiểm tra xem công ty đã ký biên bản hủy chưa True: Đã ký; False: Chưa ký
        /// </summary>
        public bool? IsCompanySignReportDelete { get; set; }
        /// <summary>
        /// Kiểm tra xem khách hàng đã ký biên bản delete chưa Trua: Đã ký; False: Chưa ký
        /// </summary>
        public bool? IsCustomerSignReportDelete { get; set; }
    }

    public class BasicInvoiceTemplateModel
    {
        public Guid InvoiceTemplateId { get; set; }
        public Guid? InvoiceCategoryId { get; set; }
        public string TemplateName { get; set; }
        public string XmlFile { get; set; }
        public string XsltFile { get; set; }
        public string SchemaFile { get; set; }
        public string ServiceType { get; set; }
        public string InvoiceType { get; set; }
        public string InvoiceView { get; set; }
        public string Igenerator { get; set; }
        public string Iviewer { get; set; }
        public string CssData { get; set; }
        public string CssLogo { get; set; }
        public string CssBackground { get; set; }
        public string ImagePath { get; set; }
        public bool? IsPublic { get; set; }
        public bool? IsCertify { get; set; }
        public string TemplateCode { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public BasicInvoiceCategoryModel InvoiceCategory { get; set; }
    }

    public class BasicInvoiceCategoryModel
    {
        public Guid InvoiceCategoryId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsPublic { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
    }

    public class InvoiceSigningResult
    {
        public Guid InvoiceId { get; set; }
        public bool Result { get; set; }
        public string Message { get; set; }
    }

    public class OriginalInvoiceModel
    {
        public Guid InvoiceId { get; set; }
        public Guid? InvoiceCategoryId { get; set; }
        public Guid? InvoiceTemplateId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? CustomerId { get; set; }
        public string InvoiceCode { get; set; }
        public string Data { get; set; }
        public string Signature { get; set; }
        public int? Converted { get; set; }
        public string KindOfService { get; set; }
        public int? PaymentStatus { get; set; }
        public DateTime? ArisingDate { get; set; }
        public string ProcessInvoiceNote { get; set; }
        public decimal? GrossValue { get; set; }
        public decimal? VatAmount0 { get; set; }
        public decimal? GrossValue0 { get; set; }
        public decimal? VatAmount5 { get; set; }
        public decimal? GrossValue5 { get; set; }
        public decimal? VatAmount10 { get; set; }
        public decimal? GrossValue10 { get; set; }
        public bool? Certified { get; set; }
        public string CertifiedId { get; set; }
        public string CertifiedData { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public Guid? CreationBy { get; set; }
        public Guid? PublishBy { get; set; }
        public Guid? PaymentMethod { get; set; }
        public string SellerAppRecordId { get; set; }
        public string InvoiceAppRecordId { get; set; }
        public string InvoiceType { get; set; }
        public string TemplateCode { get; set; }
        public string InvoiceSeries { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceName { get; set; }
        public DateTime? InvoiceIssuedDate { get; set; }
        public DateTime? SignedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string ContractNumber { get; set; }
        public DateTime? ContractDate { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string InvoiceNote { get; set; }
        public int? AdjustmentType { get; set; }
        public Guid? OriginalInvoiceId { get; set; }
        public string AdditionalReferenceDesc { get; set; }
        public DateTime? AdditionalReferenceDate { get; set; }
        public string SellerLegalName { get; set; }
        public string SellerTaxCode { get; set; }
        public string SellerAddressLine { get; set; }
        public string SellerPostalCode { get; set; }
        public string SellerDistrictName { get; set; }
        public string SellerCityName { get; set; }
        public string SellerCountryCode { get; set; }
        public string SellerPhoneNumber { get; set; }
        public string SellerFaxNumber { get; set; }
        public string SellerEmail { get; set; }
        public string SellerBankName { get; set; }
        public string SellerBankAccount { get; set; }
        public string SellerContactPersonName { get; set; }
        public string SellerSignedPersonName { get; set; }
        public string SellerSubmittedPersonName { get; set; }
        public string BuyerDisplayName { get; set; }
        public string BuyerLegalName { get; set; }
        public string BuyerTaxCode { get; set; }
        public string BuyerAddressLine { get; set; }
        public string BuyerPostalCode { get; set; }
        public string BuyerDistrictName { get; set; }
        public string BuyerCityName { get; set; }
        public string BuyerCountryCode { get; set; }
        public string BuyerPhoneNumber { get; set; }
        public string BuyerFaxNumber { get; set; }
        public string BuyerEmail { get; set; }
        public string BuyerBankName { get; set; }
        public string BuyerBankAccount { get; set; }
        public decimal? SumOfTotalLineAmountWithoutVat { get; set; }
        public decimal? TotalAmountWithoutVat { get; set; }
        public decimal? TotalVatamount { get; set; }
        public decimal? TotalAmountWithVat { get; set; }
        public decimal? TotalAmountWithVatfrn { get; set; }
        public string TotalAmountWithVatinWords { get; set; }
        public bool? IsTotalAmountPos { get; set; }
        public bool? IsTotalVatamountPos { get; set; }
        public bool? IsTotalAmtWithoutVatPos { get; set; }
        public decimal? DiscountAmount { get; set; }
        public bool? IsDiscountAmtPos { get; set; }
        public string ProductCodes { get; set; }
        public string PortalLink { get; set; }
        public int? InvoiceStatus { get; set; }
        public bool? TrangThaiXemTraCuuHoaDon { get; set; }
        public decimal? VatPercentage { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }

        // Added info field
        public string PublisherName { get; set; }
        public string PaymentMethodName { get; set; }
        public List<ProductInvoiceInternalModel> ListProductInvoice { get; set; }
    }

    public class EditInvoiceModel
    {
        public Guid InvoiceId { get; set; }
        public Guid? InvoiceCategoryId { get; set; }
        public Guid? InvoiceTemplateId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? CustomerId { get; set; }
        public string InvoiceCode { get; set; }
        public string Data { get; set; }
        public string Signature { get; set; }
        public int? Converted { get; set; }
        public string KindOfService { get; set; }
        public int? PaymentStatus { get; set; }
        public DateTime? ArisingDate { get; set; }
        public string ProcessInvoiceNote { get; set; }
        public decimal? GrossValue { get; set; }
        public decimal? VatAmount0 { get; set; }
        public decimal? GrossValue0 { get; set; }
        public decimal? VatAmount5 { get; set; }
        public decimal? GrossValue5 { get; set; }
        public decimal? VatAmount10 { get; set; }
        public decimal? GrossValue10 { get; set; }
        public bool? Certified { get; set; }
        public string CertifiedId { get; set; }
        public string CertifiedData { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public Guid? CreationBy { get; set; }
        public Guid? PublishBy { get; set; }
        public Guid? PaymentMethod { get; set; }
        public string SellerAppRecordId { get; set; }
        public string InvoiceAppRecordId { get; set; }
        public string InvoiceType { get; set; }
        public string TemplateCode { get; set; }
        public string InvoiceSeries { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceName { get; set; }
        public DateTime? InvoiceIssuedDate { get; set; }
        public DateTime? SignedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string ContractNumber { get; set; }
        public DateTime? ContractDate { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? AmountExchanged { get; set; }
        public string InvoiceNote { get; set; }
        public int? AdjustmentType { get; set; }
        public Guid? OriginalInvoiceId { get; set; }
        public string AdditionalReferenceDesc { get; set; }
        public DateTime? AdditionalReferenceDate { get; set; }
        public string SellerLegalName { get; set; }
        public string SellerTaxCode { get; set; }
        public string SellerAddressLine { get; set; }
        public string SellerPostalCode { get; set; }
        public string SellerDistrictName { get; set; }
        public string SellerCityName { get; set; }
        public string SellerCountryCode { get; set; }
        public string SellerPhoneNumber { get; set; }
        public string SellerFaxNumber { get; set; }
        public string SellerEmail { get; set; }
        public string SellerBankName { get; set; }
        public string SellerBankAccount { get; set; }
        public string SellerContactPersonName { get; set; }
        public string SellerSignedPersonName { get; set; }
        public string SellerSubmittedPersonName { get; set; }
        public string BuyerDisplayName { get; set; }
        public string BuyerLegalName { get; set; }
        public string BuyerTaxCode { get; set; }
        public string BuyerAddressLine { get; set; }
        public string BuyerPostalCode { get; set; }
        public string BuyerDistrictName { get; set; }
        public string BuyerCityName { get; set; }
        public string BuyerCountryCode { get; set; }
        public string BuyerPhoneNumber { get; set; }
        public string BuyerFaxNumber { get; set; }
        public string BuyerEmail { get; set; }
        public string BuyerBankName { get; set; }
        public string BuyerBankAccount { get; set; }
        public decimal? SumOfTotalLineAmountWithoutVat { get; set; }
        public decimal? TotalAmountWithoutVat { get; set; }
        public decimal? TotalVatamount { get; set; }
        public decimal? TotalAmountWithVat { get; set; }
        public decimal? TotalAmountWithVatfrn { get; set; }
        public string TotalAmountWithVatinWords { get; set; }
        public bool? IsTotalAmountPos { get; set; }
        public bool? IsTotalVatamountPos { get; set; }
        public bool? IsTotalAmtWithoutVatPos { get; set; }
        public decimal? DiscountAmount { get; set; }
        public bool? IsDiscountAmtPos { get; set; }
        public string ProductCodes { get; set; }
        public string PortalLink { get; set; }
        public int? InvoiceStatus { get; set; }
        public bool? TrangThaiXemTraCuuHoaDon { get; set; }
        public decimal? VatPercentage { get; set; }
        public bool? ConvertedStatus { get; set; }
        public DateTime? ConvertedDate { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        #region Thông tin trường đặc thù theo ngành nghề
        /// <summary>
        /// Sử dụng từ ngày (hóa đơn điện, nước..v..v)
        /// </summary>
        public DateTime? FromUseDate { get; set; }
        /// <summary>
        /// Sử dụng đến ngày (hóa đơn điện, nước...v.v..)
        /// </summary>
        public DateTime? ToUseDate { get; set; }
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

        public List<ProductInvoiceInternalModel> ListProductInvoice { get; set; }
    }
}
