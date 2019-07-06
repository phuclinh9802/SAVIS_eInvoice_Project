using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BASIC.AUTHEN.DATA;
using System.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using log4net;
using System.IO;

namespace BASIC.AUTHEN.BUSSINESS
{
    public class DbInvoiceHandler
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string invoice_api_url = ConfigurationManager.AppSettings["INVOICE_API_URL"];

        /// <summary>
        /// Đẩy hóa đơn từ PMKT vào HĐĐT
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Response<DataOutputModel> PushInvoice(string jsonData, string appId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    InvoiceModel data = JsonConvert.DeserializeObject<InvoiceModel>(jsonData);
                    var db = unitOfWork.DataContext;
                    DataOutputModel dataReturn = new DataOutputModel();
                    dataReturn.IdInvoicePMKT = data.IdInvoicePMKT;
                    dataReturn.Status = CONSTANT_STATUS_API.THAT_BAI;

                    #region Id hóa đơn bên PMKT
                    if (string.IsNullOrWhiteSpace(data.IdInvoicePMKT))
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Id hóa đơn từ PMKT trống", dataReturn);
                    }
                    var dataConnectInvoice = unitOfWork.GetRepository<ConnectWithInvoice>().GetMany(x => x.OtherId == data.IdInvoicePMKT).FirstOrDefault();
                    if (dataConnectInvoice != null)//Nếu có rồi thì update thông tin
                    {
                        return UpdateInvoice(data, appId);
                    }
                    else//Chưa có thì insert dữ liệu
                    {
                        return CreatedInvoice(data, appId);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                return new Response<DataOutputModel>(0, "Lỗi: " + ex.ToString(), null);
            }
        }

        /// <summary>
        /// Tạo mới hóa đơn
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Response<DataOutputModel> CreatedInvoice(InvoiceModel data, string appId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var db = unitOfWork.DataContext;
                    DataOutputModel dataReturn = new DataOutputModel();
                    dataReturn.IdInvoicePMKT = data.IdInvoicePMKT;
                    dataReturn.Status = CONSTANT_STATUS_API.THAT_BAI;

                    #region Kiểm tra dữ liệu đầu vào  
                    #region Id hóa đơn bên PMKT
                    if (string.IsNullOrWhiteSpace(data.IdInvoicePMKT))
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Id hóa đơn từ PMKT trống", dataReturn);
                    }
                    //Check xem hóa đơn này đã đẩy sang chưa, nếu đẩy sang rồi thì phải dùng API update 
                    var dataConnectInvoice = unitOfWork.GetRepository<ConnectWithInvoice>().GetMany(x => x.OtherId == data.IdInvoicePMKT).FirstOrDefault();
                    if (dataConnectInvoice != null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Hóa đơn này đã được đẩy sang. Hãy sử dụng API updateinvoice để cập nhật thông tin hóa đơn", dataReturn);
                    }
                    #endregion
                    #region Công ty phát hành
                    if (string.IsNullOrWhiteSpace(data.CompanyTax))
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: CompanyTax trống", dataReturn);
                    }
                    var checkCompany = unitOfWork.GetRepository<Company>().GetMany(x => x.TaxNumber == data.CompanyTax).FirstOrDefault();
                    if (checkCompany == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Không tìm thấy dữ liệu công ty với mã số thuế =" + data.CompanyTax, dataReturn);
                    }
                    #endregion
                    #region Mẫu hóa đơn
                    if (string.IsNullOrWhiteSpace(data.TemplateCode))
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: TemplateCode trống", dataReturn);
                    }
                    var checkTemp = unitOfWork.GetRepository<InvoiceTemplate>().GetMany(x => x.TemplateName == data.TemplateCode).FirstOrDefault();
                    if (checkTemp == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Không tìm thấy dữ liệu mẫu hóa đơn với mã số =" + data.TemplateCode, dataReturn);
                    }
                    #endregion
                    #region Seri mẫu
                    if (string.IsNullOrWhiteSpace(data.InvoiceSeries))
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: InvoiceSeries trống", dataReturn);
                    }
                    var checkTempPublish = unitOfWork.GetRepository<Publish_Invoice_Template>().GetMany(x => x.InvoiceTemplateId == checkTemp.InvoiceTemplateId
                    && x.InvoiceSeries == data.InvoiceSeries && x.CompanyId == checkCompany.CompanyId && x.Status == 1).FirstOrDefault();
                    if (checkTempPublish == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Không tìm thấy dữ liệu dải hóa đơn", dataReturn);
                    }
                    #endregion
                    #region Ngày tạo hóa đơn
                    if (string.IsNullOrWhiteSpace(data.InvoiceIssuedDate))
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Ngày hóa đơn(InvoiceIssuedDate) trống.", dataReturn);
                    }
                    #endregion
                    data.InvoiceCategoryId = checkTempPublish.InvoiceCategoryId.Value;
                    data.InvoiceTemplateId = checkTempPublish.InvoiceTemplateId.Value;
                    data.CompanyId = checkTempPublish.CompanyId.Value;
                    data.InvoiceSeries = checkTempPublish.InvoiceSeries;
                    #region Loại hóa đơn
                    string categoryName = "";
                    if (data.InvoiceCategoryId == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: InvoiceCategoryId trống", dataReturn);
                    }
                    var checkCategory = unitOfWork.GetRepository<InvoiceCategory>().GetMany(x => x.InvoiceCategoryId == data.InvoiceCategoryId).FirstOrDefault();
                    if (checkCategory == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Không tìm thấy dữ liệu với InvoiceCategoryId =" + data.InvoiceCategoryId, dataReturn);
                    }
                    categoryName = checkCategory.Name;
                    #endregion


                    #region Khách hàng
                    if (!string.IsNullOrWhiteSpace(data.CustomerCode))
                    {
                        var checkCustomer = unitOfWork.GetRepository<Customer>().GetMany(x => x.CustomerCode == data.CustomerCode).FirstOrDefault();
                        if (checkCustomer == null)
                        {
                            return new Response<DataOutputModel>(0, "Lỗi: Không tìm thấy dữ liệu khách hàng với CustomerCode =" + data.CustomerCode, dataReturn);
                        }
                        #region Set lại thông tin khách hàng theo khách hàng tìm được trong DB
                        data.CustomerId = checkCustomer.CustomerId;
                        data.BuyerAddressLine = checkCustomer.Address;
                        data.BuyerBankAccount = checkCustomer.BankAccountName;
                        data.BuyerBankName = checkCustomer.BankName;
                        data.BuyerDisplayName = checkCustomer.RepresentPerson;
                        data.BuyerEmail = checkCustomer.Email;
                        data.BuyerFaxNumber = checkCustomer.Fax;
                        data.BuyerLegalName = checkCustomer.Name;
                        data.BuyerPhoneNumber = checkCustomer.Phone;
                        data.BuyerTaxCode = checkCustomer.TaxNumber;
                        #endregion
                    }
                    #endregion
                    #region Hình thức thanh toán                                      
                    if (string.IsNullOrWhiteSpace(data.PaymentMethod))
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: PaymentMethod trống", dataReturn);
                    }
                    var dataPayment = new DbCommonHandler().GetPaymentMethod().Data;
                    var checkPayment = dataPayment.FirstOrDefault(x => x.Name == data.PaymentMethod);
                    if (checkPayment == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Không tìm thấy dữ liệu với PaymentMethod =" + data.PaymentMethod, dataReturn);
                    }
                    #endregion
                    #region Thông tin mặt hàng
                    if (data.ListProductInvoice == null || data.ListProductInvoice.Count == 0)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Dữ liệu mặt hàng theo hóa đơn trống", dataReturn);
                    }
                    #endregion
                    #endregion

                    #region Chuyển đổi dữ liệu đầu vào này sang dữ liệu đầu vào của API Internal
                    CreateInvoiceModel dataInputInternal = new CreateInvoiceModel();
                    List<ProductInvoiceInternalModel> listProductInvoice = new List<ProductInvoiceInternalModel>();

                    #region Convert thông tin mặt hàng
                    foreach (var p in data.ListProductInvoice)
                    {
                        ProductInvoiceInternalModel ip = new ProductInvoiceInternalModel();
                        if (!string.IsNullOrWhiteSpace(p.ExpirationDate))
                        {
                            ip.ExpirationDate = Utils.TryToConvertDateTimeFromString(p.ExpirationDate);
                        }
                        ip.ItemName = p.ItemName;
                        ip.ItemTotalAmountWithoutVat = (p.UnitPrice * p.Quantity);
                        if (p.TotalAmount != null)
                            ip.ItemTotalAmountWithoutVat = p.TotalAmount;
                        ip.ItemTotalAmountWithVat = (ip.ItemTotalAmountWithoutVat * (p.VatPercentage / 100)) + ip.ItemTotalAmountWithoutVat;
                        ip.LineNumber = p.LineNumber;
                        ip.LotNumber = p.LotNumber;
                        if (p.ItemType == InvoiceConstants.ProductInvoiceType.HANG_HOA_BINH_THUONG)
                            ip.Promotion = false;
                        else
                            ip.Promotion = true;
                        ip.ItemType = p.ItemType;
                        ip.Quantity = p.Quantity;
                        ip.UnitName = p.UnitName;
                        ip.UnitPrice = p.UnitPrice;
                        ip.VatAmount = (ip.ItemTotalAmountWithoutVat * (p.VatPercentage / 100));
                        ip.VatPercentage = p.VatPercentage;
                        ip.ItemCode = p.ProductCode;
                        listProductInvoice.Add(ip);
                    }
                    #endregion

                    #region Thông tin hóa đơn chi tiết
                    dataInputInternal.ListProductInvoice = listProductInvoice;
                    dataInputInternal.AdjustmentType = InvoiceConstants.InvoiceAdjustmentType.HOA_DON_THONG_THUONG;
                    dataInputInternal.BuyerAddressLine = data.BuyerAddressLine;
                    dataInputInternal.BuyerBankAccount = data.BuyerBankAccount;
                    dataInputInternal.BuyerBankName = data.BuyerBankName;
                    dataInputInternal.BuyerCityName = "";
                    dataInputInternal.BuyerCountryCode = "";
                    dataInputInternal.BuyerDisplayName = data.BuyerDisplayName;
                    dataInputInternal.BuyerDistrictName = "";
                    dataInputInternal.BuyerEmail = data.BuyerEmail;
                    dataInputInternal.BuyerFaxNumber = data.BuyerFaxNumber;
                    dataInputInternal.BuyerLegalName = data.BuyerLegalName;
                    dataInputInternal.BuyerPhoneNumber = data.BuyerPhoneNumber;
                    dataInputInternal.BuyerTaxCode = data.BuyerTaxCode;
                    dataInputInternal.CompanyId = data.CompanyId;
                    dataInputInternal.CreatedByUserId = new Guid(CONSTANT_APP.ADMIN_ID);
                    dataInputInternal.CreatedOnDate = DateTime.Now;
                    dataInputInternal.CurrencyCode = data.CurrencyCode;
                    dataInputInternal.CustomerId = data.CustomerId;
                    dataInputInternal.InvoiceCategoryId = data.InvoiceCategoryId;
                    dataInputInternal.InvoiceIssuedDate = Utils.TryToConvertDateTimeFromString(data.InvoiceIssuedDate);
                    dataInputInternal.InvoiceName = categoryName;
                    dataInputInternal.InvoiceNote = data.InvoiceNote;
                    dataInputInternal.InvoiceSeries = data.InvoiceSeries;
                    dataInputInternal.InvoiceStatus = InvoiceConstants.InvoiceStatus.HOA_DON_MOI_TAO_LAP;
                    dataInputInternal.InvoiceTemplateId = data.InvoiceTemplateId;
                    dataInputInternal.LastModifiedByUserId = new Guid(CONSTANT_APP.ADMIN_ID);
                    dataInputInternal.LastModifiedOnDate = DateTime.Now;
                    dataInputInternal.PaymentMethod = checkPayment.PaymentMethodId;
                    dataInputInternal.SellerAddressLine = checkCompany.Address;
                    dataInputInternal.SellerBankAccount = checkCompany.BankAccountName;
                    dataInputInternal.SellerBankName = checkCompany.BankName;
                    dataInputInternal.SellerContactPersonName = checkCompany.ContactPerson;
                    dataInputInternal.SellerEmail = checkCompany.Email;
                    dataInputInternal.SellerFaxNumber = checkCompany.Fax;
                    dataInputInternal.SellerLegalName = checkCompany.Name;
                    dataInputInternal.SellerPhoneNumber = checkCompany.PhoneNumber;
                    dataInputInternal.SellerTaxCode = checkCompany.TaxNumber;
                    dataInputInternal.SumOfTotalLineAmountWithoutVat = listProductInvoice.Sum(x => x.ItemTotalAmountWithoutVat);
                    dataInputInternal.TemplateCode = data.TemplateCode;
                    dataInputInternal.TotalAmountWithoutVat = listProductInvoice.Sum(x => x.ItemTotalAmountWithoutVat);
                    if (data.TotalAmount != null)//Cho phép sửa số tiền tổng hóa đơn
                        dataInputInternal.TotalAmountWithoutVat = data.TotalAmount;
                    dataInputInternal.TotalAmountWithVat = listProductInvoice.Sum(x => x.ItemTotalAmountWithVat);
                    dataInputInternal.TotalVatamount = dataInputInternal.TotalAmountWithVat - dataInputInternal.TotalAmountWithoutVat;
                    if (data.TotalAmountVAT != null)//Cho phép sửa sô tiền thuế
                        dataInputInternal.TotalVatamount = data.TotalAmountVAT;
                    dataInputInternal.VatPercentage = data.VatPercentage;
                    dataInputInternal.ExchangeRate = data.ExchangeRate;
                    dataInputInternal.AmountExchanged = data.AmountExchanged;
                    #region Thông tin trường đặc thù theo ngành nghề
                    dataInputInternal.FromUseDate = Utils.TryToConvertDateTimeFromString(data.FromUseDate);
                    dataInputInternal.ToUseDate = Utils.TryToConvertDateTimeFromString(data.ToUseDate);
                    dataInputInternal.EnvironmentTax = data.EnvironmentTax;
                    dataInputInternal.EnvironmentFee = data.EnvironmentFee;
                    dataInputInternal.StartNumber = data.StartNumber;
                    dataInputInternal.EndNumber = data.StartNumber;
                    dataInputInternal.UseNumber = data.UseNumber;
                    #endregion
                    #endregion
                    #endregion

                    #region Lưu thông tin hóa đơn
                    var result = Create(dataInputInternal);
                    if (result.Status == 1)
                    {
                        #region Lưu thông tin ràng buộc giữa hóa đơn PMKT và HĐĐT
                        var dataInvoiceFromHDDT = result.Data;
                        ConnectWithInvoice itemConnectInvoice = new ConnectWithInvoice();
                        itemConnectInvoice.AdjustmentType = dataInvoiceFromHDDT.AdjustmentType.Value;
                        itemConnectInvoice.ConnectApplicationId = new Guid(appId);
                        itemConnectInvoice.ConnectWithInvoiceId = Guid.NewGuid();
                        itemConnectInvoice.CreatedOnDate = DateTime.Now;
                        itemConnectInvoice.InvoiceId = dataInvoiceFromHDDT.InvoiceId;
                        itemConnectInvoice.OtherId = data.IdInvoicePMKT;
                        itemConnectInvoice.Status = CONSTANT_STATUS_GIVE.CHUA_GUI;
                        itemConnectInvoice.StatusInvoice = dataInvoiceFromHDDT.InvoiceStatus.Value;
                        unitOfWork.GetRepository<ConnectWithInvoice>().Add(itemConnectInvoice);

                        ConnectHistory itemHis = new ConnectHistory();
                        itemHis.ConnectApplicationId = new Guid(appId);
                        itemHis.ConnectHistoryId = Guid.NewGuid();
                        itemHis.Content = "Lập mới hóa đơn " + dataInvoiceFromHDDT.InvoiceCode;
                        itemHis.CreatedOnDate = DateTime.Now;
                        itemHis.InvoiceId = dataInvoiceFromHDDT.InvoiceId;
                        itemHis.Status = true;
                        unitOfWork.GetRepository<ConnectHistory>().Add(itemHis);
                        #endregion
                        unitOfWork.Save();
                        dataReturn.Status = CONSTANT_STATUS_API.THANH_CONG;
                        return new Response<DataOutputModel>(1, "Success", dataReturn);
                    }
                    else
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Không lưu được dữ liệu hóa đơn vào hệ thống HĐĐT", dataReturn);
                    }
                    #endregion
                }
            }
            catch (NullReferenceException ex)
            {
                DataOutputModel dataReturn = new DataOutputModel();
                if (data != null)
                {
                    dataReturn.IdInvoicePMKT = data.IdInvoicePMKT;
                }
                dataReturn.Status = CONSTANT_STATUS_API.THAT_BAI;
                logger.Debug(ex.Message);
                return new Response<DataOutputModel>(0, "Lỗi: Sai cấu trúc dữ liệu đầu vào", dataReturn);
            }
            catch (Exception ex)
            {
                DataOutputModel dataReturn = new DataOutputModel();
                dataReturn.IdInvoicePMKT = data.IdInvoicePMKT;
                dataReturn.Status = CONSTANT_STATUS_API.THAT_BAI;
                logger.Debug(ex.Message);
                return new Response<DataOutputModel>(0, "Lỗi: " + ex.Message, dataReturn);
            }
        }

        /// <summary>
        /// Cập nhật hóa đơn
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Response<DataOutputModel> UpdateInvoice(InvoiceModel data, string appId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var db = unitOfWork.DataContext;
                    DataOutputModel dataReturn = new DataOutputModel();
                    dataReturn.IdInvoicePMKT = data.IdInvoicePMKT;
                    dataReturn.Status = CONSTANT_STATUS_API.THAT_BAI;

                    #region Kiểm tra dữ liệu đầu vào  
                    #region Id hóa đơn bên PMKT
                    if (string.IsNullOrWhiteSpace(data.IdInvoicePMKT))
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: IdInvoicePMKT trống", dataReturn);
                    }
                    var checkInvoiceInDB = unitOfWork.GetRepository<ConnectWithInvoice>().GetMany(x => x.OtherId == data.IdInvoicePMKT && x.ConnectApplicationId == new Guid(appId)).FirstOrDefault();
                    if (checkInvoiceInDB == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: hóa đơn không thuộc ứng dụng", dataReturn);
                    }
                    #endregion
                    #region Kiểm tra hóa đơn đã phát hành chưa
                    var invoiceCheck = unitOfWork.GetRepository<Invoice>().GetMany(x => x.InvoiceId == checkInvoiceInDB.InvoiceId).FirstOrDefault();
                    if (invoiceCheck == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Không tìm thấy thông tin hóa đơn trong cơ sở dữ liệu", dataReturn);
                    }
                    if (!string.IsNullOrWhiteSpace(invoiceCheck.InvoiceNumber) && invoiceCheck.PublishDate != null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Hóa đơn đã phát hành không được sửa thông tin", dataReturn);
                    }
                    #endregion
                    #region Công ty phát hành
                    if (string.IsNullOrWhiteSpace(data.CompanyTax))
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: CompanyTax trống", dataReturn);
                    }
                    var checkCompany = unitOfWork.GetRepository<Company>().GetMany(x => x.TaxNumber == data.CompanyTax).FirstOrDefault();
                    if (checkCompany == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Không tìm thấy dữ liệu công ty với mã số thuế =" + data.CompanyTax, dataReturn);
                    }
                    #endregion
                    #region Mẫu hóa đơn
                    if (string.IsNullOrWhiteSpace(data.TemplateCode))
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: TemplateCode trống", dataReturn);
                    }
                    var checkTemp = unitOfWork.GetRepository<InvoiceTemplate>().GetMany(x => x.TemplateName == data.TemplateCode).FirstOrDefault();
                    if (checkTemp == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Không tìm thấy dữ liệu mẫu hóa đơn với mã số =" + data.TemplateCode, dataReturn);
                    }
                    #endregion
                    #region Seri mẫu
                    if (string.IsNullOrWhiteSpace(data.InvoiceSeries))
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: InvoiceSeries trống", dataReturn);
                    }
                    var checkTempPublish = unitOfWork.GetRepository<Publish_Invoice_Template>().GetMany(x => x.InvoiceTemplateId == checkTemp.InvoiceTemplateId
                    && x.InvoiceSeries == data.InvoiceSeries && x.CompanyId == checkCompany.CompanyId && x.Status == 1).FirstOrDefault();
                    if (checkTempPublish == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Không tìm thấy dữ liệu dải hóa đơn", dataReturn);
                    }
                    #endregion
                    #region Ngày tạo hóa đơn
                    if (string.IsNullOrWhiteSpace(data.InvoiceIssuedDate))
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Ngày hóa đơn(InvoiceIssuedDate) trống.", dataReturn);
                    }
                    #endregion
                    data.InvoiceCategoryId = checkTempPublish.InvoiceCategoryId.Value;
                    data.InvoiceTemplateId = checkTempPublish.InvoiceTemplateId.Value;
                    data.CompanyId = checkTempPublish.CompanyId.Value;
                    data.InvoiceSeries = checkTempPublish.InvoiceSeries;
                    #region Loại hóa đơn
                    string categoryName = "";
                    if (data.InvoiceCategoryId == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: InvoiceCategoryId trống", dataReturn);
                    }
                    var checkCategory = unitOfWork.GetRepository<InvoiceCategory>().GetMany(x => x.InvoiceCategoryId == data.InvoiceCategoryId).FirstOrDefault();
                    if (checkCategory == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Không tìm thấy dữ liệu với InvoiceCategoryId =" + data.InvoiceCategoryId, dataReturn);
                    }
                    categoryName = checkCategory.Name;
                    #endregion


                    #region Khách hàng
                    if (!string.IsNullOrWhiteSpace(data.CustomerCode))
                    {
                        var checkCustomer = unitOfWork.GetRepository<Customer>().GetMany(x => x.CustomerCode == data.CustomerCode).FirstOrDefault();
                        if (checkCustomer == null)
                        {
                            return new Response<DataOutputModel>(0, "Lỗi: Không tìm thấy dữ liệu khách hàng với CustomerCode =" + data.CustomerCode, dataReturn);
                        }
                        #region Set lại thông tin khách hàng theo khách hàng tìm được trong DB
                        data.CustomerId = checkCustomer.CustomerId;
                        data.BuyerAddressLine = checkCustomer.Address;
                        data.BuyerBankAccount = checkCustomer.BankAccountName;
                        data.BuyerBankName = checkCustomer.BankName;
                        data.BuyerDisplayName = checkCustomer.RepresentPerson;
                        data.BuyerEmail = checkCustomer.Email;
                        data.BuyerFaxNumber = checkCustomer.Fax;
                        data.BuyerLegalName = checkCustomer.Name;
                        data.BuyerPhoneNumber = checkCustomer.Phone;
                        data.BuyerTaxCode = checkCustomer.TaxNumber;
                        #endregion
                    }
                    #endregion
                    #region Hình thức thanh toán                                      
                    if (string.IsNullOrWhiteSpace(data.PaymentMethod))
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: PaymentMethod trống", dataReturn);
                    }
                    var dataPayment = new DbCommonHandler().GetPaymentMethod().Data;
                    var checkPayment = dataPayment.FirstOrDefault(x => x.Name == data.PaymentMethod);
                    if (checkPayment == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Không tìm thấy dữ liệu với PaymentMethod =" + data.PaymentMethod, dataReturn);
                    }
                    #endregion
                    #region Thông tin mặt hàng
                    if (data.ListProductInvoice == null || data.ListProductInvoice.Count == 0)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Dữ liệu mặt hàng theo hóa đơn trống", dataReturn);
                    }
                    #endregion
                    #endregion

                    #region Chuyển đổi dữ liệu đầu vào này sang dữ liệu đầu vào của API Internal
                    CreateInvoiceModel dataInputInternal = new CreateInvoiceModel();
                    List<ProductInvoiceInternalModel> listProductInvoice = new List<ProductInvoiceInternalModel>();

                    #region Convert thông tin mặt hàng
                    foreach (var p in data.ListProductInvoice)
                    {
                        ProductInvoiceInternalModel ip = new ProductInvoiceInternalModel();
                        if (!string.IsNullOrWhiteSpace(p.ExpirationDate))
                        {
                            ip.ExpirationDate = Utils.TryToConvertDateTimeFromString(p.ExpirationDate);
                        }
                        ip.ItemName = p.ItemName;
                        ip.ItemTotalAmountWithoutVat = (p.UnitPrice * p.Quantity);
                        if (p.TotalAmount != null)
                            ip.ItemTotalAmountWithoutVat = p.TotalAmount;
                        ip.ItemTotalAmountWithVat = (ip.ItemTotalAmountWithoutVat * (p.VatPercentage / 100)) + ip.ItemTotalAmountWithoutVat;
                        ip.LineNumber = p.LineNumber;
                        ip.LotNumber = p.LotNumber;
                        if (p.ItemType == InvoiceConstants.ProductInvoiceType.HANG_HOA_BINH_THUONG)
                            ip.Promotion = false;
                        else
                            ip.Promotion = true;
                        ip.ItemType = p.ItemType;
                        ip.Quantity = p.Quantity;
                        ip.UnitName = p.UnitName;
                        ip.UnitPrice = p.UnitPrice;
                        ip.VatAmount = (ip.ItemTotalAmountWithoutVat * (p.VatPercentage / 100));
                        ip.VatPercentage = p.VatPercentage;
                        ip.ItemCode = p.ProductCode;
                        listProductInvoice.Add(ip);
                    }
                    #endregion

                    #region Thông tin hóa đơn chi tiết
                    dataInputInternal.ListProductInvoice = listProductInvoice;
                    dataInputInternal.AdjustmentType = InvoiceConstants.InvoiceAdjustmentType.HOA_DON_THONG_THUONG;
                    dataInputInternal.BuyerAddressLine = data.BuyerAddressLine;
                    dataInputInternal.BuyerBankAccount = data.BuyerBankAccount;
                    dataInputInternal.BuyerBankName = data.BuyerBankName;
                    dataInputInternal.BuyerCityName = "";
                    dataInputInternal.BuyerCountryCode = "";
                    dataInputInternal.BuyerDisplayName = data.BuyerDisplayName;
                    dataInputInternal.BuyerDistrictName = "";
                    dataInputInternal.BuyerEmail = data.BuyerEmail;
                    dataInputInternal.BuyerFaxNumber = data.BuyerFaxNumber;
                    dataInputInternal.BuyerLegalName = data.BuyerLegalName;
                    dataInputInternal.BuyerPhoneNumber = data.BuyerPhoneNumber;
                    dataInputInternal.BuyerTaxCode = data.BuyerTaxCode;
                    dataInputInternal.CompanyId = data.CompanyId;
                    dataInputInternal.CreatedByUserId = new Guid(CONSTANT_APP.ADMIN_ID);
                    dataInputInternal.CreatedOnDate = DateTime.Now;
                    dataInputInternal.CurrencyCode = data.CurrencyCode;
                    dataInputInternal.CustomerId = data.CustomerId;
                    dataInputInternal.InvoiceCategoryId = data.InvoiceCategoryId;
                    dataInputInternal.InvoiceIssuedDate = Utils.TryToConvertDateTimeFromString(data.InvoiceIssuedDate);
                    dataInputInternal.InvoiceName = categoryName;
                    dataInputInternal.InvoiceNote = data.InvoiceNote;
                    dataInputInternal.InvoiceSeries = data.InvoiceSeries;
                    dataInputInternal.InvoiceStatus = InvoiceConstants.InvoiceStatus.HOA_DON_MOI_TAO_LAP;
                    dataInputInternal.InvoiceTemplateId = data.InvoiceTemplateId;
                    dataInputInternal.LastModifiedByUserId = new Guid(CONSTANT_APP.ADMIN_ID);
                    dataInputInternal.LastModifiedOnDate = DateTime.Now;
                    dataInputInternal.PaymentMethod = checkPayment.PaymentMethodId;
                    dataInputInternal.SellerAddressLine = checkCompany.Address;
                    dataInputInternal.SellerBankAccount = checkCompany.BankAccountName;
                    dataInputInternal.SellerBankName = checkCompany.BankName;
                    dataInputInternal.SellerContactPersonName = checkCompany.ContactPerson;
                    dataInputInternal.SellerEmail = checkCompany.Email;
                    dataInputInternal.SellerFaxNumber = checkCompany.Fax;
                    dataInputInternal.SellerLegalName = checkCompany.Name;
                    dataInputInternal.SellerPhoneNumber = checkCompany.PhoneNumber;
                    dataInputInternal.SellerTaxCode = checkCompany.TaxNumber;
                    dataInputInternal.SumOfTotalLineAmountWithoutVat = listProductInvoice.Sum(x => x.ItemTotalAmountWithoutVat);
                    dataInputInternal.TemplateCode = data.TemplateCode;
                    dataInputInternal.TotalAmountWithoutVat = listProductInvoice.Sum(x => x.ItemTotalAmountWithoutVat);
                    if (data.TotalAmount != null)//Cho phép sửa số tiền tổng hóa đơn
                        dataInputInternal.TotalAmountWithoutVat = data.TotalAmount;
                    dataInputInternal.TotalAmountWithVat = listProductInvoice.Sum(x => x.ItemTotalAmountWithVat);
                    dataInputInternal.TotalVatamount = dataInputInternal.TotalAmountWithVat - dataInputInternal.TotalAmountWithoutVat;
                    if (data.TotalAmountVAT != null)//Cho phép sửa sô tiền thuế
                        dataInputInternal.TotalVatamount = data.TotalAmountVAT;
                    dataInputInternal.VatPercentage = data.VatPercentage;
                    dataInputInternal.ExchangeRate = data.ExchangeRate;
                    dataInputInternal.AmountExchanged = data.AmountExchanged;
                    #region Thông tin trường đặc thù theo ngành nghề
                    dataInputInternal.FromUseDate = Utils.TryToConvertDateTimeFromString(data.FromUseDate);
                    dataInputInternal.ToUseDate = Utils.TryToConvertDateTimeFromString(data.ToUseDate);
                    dataInputInternal.EnvironmentTax = data.EnvironmentTax;
                    dataInputInternal.EnvironmentFee = data.EnvironmentFee;
                    dataInputInternal.StartNumber = data.StartNumber;
                    dataInputInternal.EndNumber = data.StartNumber;
                    dataInputInternal.UseNumber = data.UseNumber;
                    #endregion
                    #endregion
                    #endregion

                    #region Lưu thông tin hóa đơn
                    #region Xóa thông tin hóa đơn cũ
                    unitOfWork.GetRepository<Invoice>().Delete(invoiceCheck);
                    var lstProductInvoice = unitOfWork.GetRepository<Product_Invoice>().GetMany(x => x.InvoiceId == invoiceCheck.InvoiceId).ToList();
                    if (lstProductInvoice != null && lstProductInvoice.Count > 0)
                    {
                        foreach (var piItem in lstProductInvoice)
                        {
                            unitOfWork.GetRepository<Product_Invoice>().Delete(piItem);
                        }
                    }
                    unitOfWork.GetRepository<ConnectWithInvoice>().Delete(checkInvoiceInDB);
                    var result = Create(dataInputInternal);
                    if (result.Status == 1)
                    {
                        #region Lưu thông tin ràng buộc giữa hóa đơn PMKT và HĐĐT
                        var dataInvoiceFromHDDT = result.Data;
                        ConnectWithInvoice itemConnectInvoice = new ConnectWithInvoice();
                        itemConnectInvoice.AdjustmentType = dataInvoiceFromHDDT.AdjustmentType.Value;
                        itemConnectInvoice.ConnectApplicationId = new Guid(appId);
                        itemConnectInvoice.ConnectWithInvoiceId = Guid.NewGuid();
                        itemConnectInvoice.CreatedOnDate = DateTime.Now;
                        itemConnectInvoice.InvoiceId = dataInvoiceFromHDDT.InvoiceId;
                        itemConnectInvoice.OtherId = data.IdInvoicePMKT;
                        itemConnectInvoice.Status = CONSTANT_STATUS_GIVE.CHUA_GUI;
                        itemConnectInvoice.StatusInvoice = dataInvoiceFromHDDT.InvoiceStatus.Value;
                        unitOfWork.GetRepository<ConnectWithInvoice>().Add(itemConnectInvoice);

                        ConnectHistory itemHis = new ConnectHistory();
                        itemHis.ConnectApplicationId = new Guid(appId);
                        itemHis.ConnectHistoryId = Guid.NewGuid();
                        itemHis.Content = "Cập nhật thông tin hóa đơn " + dataInvoiceFromHDDT.InvoiceCode;
                        itemHis.CreatedOnDate = DateTime.Now;
                        itemHis.InvoiceId = dataInvoiceFromHDDT.InvoiceId;
                        itemHis.Status = true;
                        unitOfWork.GetRepository<ConnectHistory>().Add(itemHis);
                        #endregion
                        unitOfWork.Save();
                        dataReturn.Status = CONSTANT_STATUS_API.THANH_CONG;
                        return new Response<DataOutputModel>(1, "Success", dataReturn);
                    }
                    else
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: (1)Không lưu được dữ liệu hóa đơn vào hệ thống HĐĐT", dataReturn);
                    }
                    #endregion
                    #endregion
                }
            }
            catch (NullReferenceException ex)
            {
                DataOutputModel dataReturn = new DataOutputModel();
                if (data != null)
                {
                    dataReturn.IdInvoicePMKT = data.IdInvoicePMKT;
                }
                dataReturn.Status = CONSTANT_STATUS_API.THAT_BAI;
                logger.Debug(ex.Message);
                return new Response<DataOutputModel>(0, "Lỗi: Sai cấu trúc dữ liệu đầu vào", dataReturn);
            }
            catch (Exception ex)
            {
                DataOutputModel dataReturn = new DataOutputModel();
                dataReturn.IdInvoicePMKT = data.IdInvoicePMKT;
                dataReturn.Status = CONSTANT_STATUS_API.THAT_BAI;
                logger.Debug(ex.Message);
                return new Response<DataOutputModel>(0, "Lỗi: " + ex.Message, dataReturn);
            }
        }

        /// <summary>
        /// Xóa hóa đơn chưa phát hành
        /// </summary>
        /// <param name="dataInvoice"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Response<List<DataOutputModel>> DeleteInvoice(List<string> dataInvoice, string appId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var db = unitOfWork.DataContext;

                    #region Kiểm tra dữ liệu đầu vào    
                    if (dataInvoice == null)
                    {
                        return new Response<List<DataOutputModel>>(0, "Lỗi: Sai cấu trúc dữ liệu đầu vào", null);
                    }

                    if (dataInvoice == null || dataInvoice.Count == 0)
                    {
                        return new Response<List<DataOutputModel>>(0, "Lỗi: Không có dữ liệu hóa đơn cần xóa", null);
                    }
                    List<DataOutputModel> dataReturn = new List<DataOutputModel>();
                    foreach (var i in dataInvoice)
                    {
                        DataOutputModel item = new DataOutputModel();
                        item.IdInvoicePMKT = i;
                        item.Status = CONSTANT_STATUS_API.THAT_BAI;
                        dataReturn.Add(item);
                    }
                    #endregion

                    #region Lấy dữ liệu hóa đơn đã phát hành để kiểm tra dữ liệu đầu vào
                    dataReturn = (from invoice in unitOfWork.GetRepository<Invoice>().GetAll()
                                      join invoiceConnect in unitOfWork.GetRepository<ConnectWithInvoice>().GetAll()
                                      on invoice.InvoiceId equals invoiceConnect.InvoiceId
                                      where invoiceConnect.ConnectApplicationId == new Guid(appId)
                                      && dataInvoice.Contains(invoiceConnect.OtherId)
                                      select new DataOutputModel()
                                      {
                                          InvoiceId = invoice.InvoiceId,
                                          IdInvoicePMKT = invoiceConnect.OtherId,
                                          InvoiceNumber = (invoice.InvoiceNumber != null && invoice.InvoiceNumber != InvoiceConstants.FIRST_INVOICE_NUMBER) ? invoice.InvoiceNumber : "",
                                          InvoiceStatus = invoice.InvoiceStatus,
                                          InvoiceType = invoice.AdjustmentType,
                                          Status = (invoice.InvoiceNumber != null && invoice.InvoiceNumber != InvoiceConstants.FIRST_INVOICE_NUMBER) ? CONSTANT_STATUS_API.THAT_BAI : CONSTANT_STATUS_API.THANH_CONG                                         
                                      }).ToList();
                    #endregion

                    #region Xóa thông tin hóa đơn   
                    var dataDelete = (from invoice in unitOfWork.GetRepository<Invoice>().GetAll()
                                      join invoiceConnect in unitOfWork.GetRepository<ConnectWithInvoice>().GetAll()
                                      on invoice.InvoiceId equals invoiceConnect.InvoiceId
                                      where invoiceConnect.ConnectApplicationId == new Guid(appId)
                                      && dataInvoice.Contains(invoiceConnect.OtherId)
                                      select invoice).ToList();
                    var dataDeleteConnect = (from invoice in unitOfWork.GetRepository<Invoice>().GetAll()
                                             join invoiceConnect in unitOfWork.GetRepository<ConnectWithInvoice>().GetAll()
                                             on invoice.InvoiceId equals invoiceConnect.InvoiceId
                                             where invoiceConnect.ConnectApplicationId == new Guid(appId)
                                             && dataInvoice.Contains(invoiceConnect.OtherId)
                                             select invoiceConnect).ToList();
                    foreach (var iReturn in dataReturn)
                    {
                        if (iReturn.Status == CONSTANT_STATUS_API.THANH_CONG)
                        {
                            var invoice = dataDelete.FirstOrDefault(x => x.InvoiceId == iReturn.InvoiceId);
                            unitOfWork.GetRepository<Invoice>().Delete(invoice);
                            var itemConnect = dataDeleteConnect.FirstOrDefault(x => x.InvoiceId == invoice.InvoiceId);
                            if (itemConnect != null)
                            {
                                unitOfWork.GetRepository<ConnectWithInvoice>().Delete(itemConnect);
                            }
                            var lstProductInvoice = unitOfWork.GetRepository<Product_Invoice>().GetMany(x => x.InvoiceId == invoice.InvoiceId).ToList();
                            if (lstProductInvoice != null && lstProductInvoice.Count > 0)
                            {
                                foreach (var piItem in lstProductInvoice)
                                {
                                    unitOfWork.GetRepository<Product_Invoice>().Delete(piItem);
                                }
                            }
                            #region Lưu lịch sử
                            ConnectHistory itemHis = new ConnectHistory();
                            itemHis.ConnectApplicationId = new Guid(appId);
                            itemHis.ConnectHistoryId = Guid.NewGuid();
                            itemHis.Content = "Xóa hóa đơn " + invoice.InvoiceCode;
                            itemHis.CreatedOnDate = DateTime.Now;
                            itemHis.InvoiceId = invoice.InvoiceId;
                            itemHis.Status = true;
                            unitOfWork.GetRepository<ConnectHistory>().Add(itemHis);
                            #endregion
                        }
                    }
                    unitOfWork.Save();
                    #endregion
                    return new Response<List<DataOutputModel>>(1, "Success", dataReturn);
                }
            }
            catch (NullReferenceException ex)
            {
                logger.Debug(ex.Message);
                return new Response<List<DataOutputModel>>(0, "Lỗi: Sai cấu trúc dữ liệu đầu vào", null);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<List<DataOutputModel>>(0, "Lỗi: " + ex.Message, null);
            }
        }

        /// <summary>
        /// Hủy hóa đơn đã phát hành
        /// </summary>
        /// <param name="dataInvoice"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Response<List<DataOutputModel>> CancelInvoice(List<string> dataInvoice, string appId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var db = unitOfWork.DataContext;

                    #region Kiểm tra dữ liệu đầu vào    
                    if (dataInvoice == null)
                    {
                        return new Response<List<DataOutputModel>>(0, "Lỗi: Sai cấu trúc dữ liệu đầu vào", null);
                    }

                    if (dataInvoice == null || dataInvoice.Count == 0)
                    {
                        return new Response<List<DataOutputModel>>(0, "Lỗi: Không có dữ liệu hóa đơn cần xóa", null);
                    }
                    List<DataOutputModel> dataReturn = new List<DataOutputModel>();
                    foreach (var i in dataInvoice)
                    {
                        DataOutputModel item = new DataOutputModel();
                        item.IdInvoicePMKT = i;
                        item.Status = CONSTANT_STATUS_API.THAT_BAI;
                        dataReturn.Add(item);
                    }
                    #endregion

                    #region Lấy dữ liệu hóa đơn để kiểm tra dữ liệu đầu vào
                    dataReturn = (from invoice in unitOfWork.GetRepository<Invoice>().GetAll()
                                  join invoiceConnect in unitOfWork.GetRepository<ConnectWithInvoice>().GetAll()
                                  on invoice.InvoiceId equals invoiceConnect.InvoiceId
                                  where invoiceConnect.ConnectApplicationId == new Guid(appId)
                                  && dataInvoice.Contains(invoiceConnect.OtherId)
                                  select new DataOutputModel()
                                  {
                                      InvoiceId = invoice.InvoiceId,
                                      IdInvoicePMKT = invoiceConnect.OtherId,
                                      InvoiceNumber = (invoice.InvoiceNumber != null && invoice.InvoiceNumber != InvoiceConstants.FIRST_INVOICE_NUMBER) ? invoice.InvoiceNumber : "",
                                      InvoiceStatus = invoice.InvoiceStatus,
                                      InvoiceType = invoice.AdjustmentType,
                                      Status = (invoice.InvoiceNumber != null && invoice.InvoiceNumber != InvoiceConstants.FIRST_INVOICE_NUMBER) ? CONSTANT_STATUS_API.THANH_CONG : CONSTANT_STATUS_API.THAT_BAI
                                  }).ToList();
                    #endregion

                    #region Hủy thông tin hóa đơn   
                    var dataDelete = (from invoice in unitOfWork.GetRepository<Invoice>().GetAll()
                                      join invoiceConnect in unitOfWork.GetRepository<ConnectWithInvoice>().GetAll()
                                      on invoice.InvoiceId equals invoiceConnect.InvoiceId
                                      where invoiceConnect.ConnectApplicationId == new Guid(appId)
                                      && dataInvoice.Contains(invoiceConnect.OtherId)
                                      select invoice).ToList();
                    foreach (var iReturn in dataReturn)
                    {
                        if (iReturn.Status == CONSTANT_STATUS_API.THANH_CONG)
                        {
                            var invoice = dataDelete.FirstOrDefault(x => x.InvoiceId == iReturn.InvoiceId);
                            var rsCancel = Cancel(iReturn.InvoiceId, new Guid(CONSTANT_APP.ADMIN_ID));
                            if (rsCancel.Status != 1)
                            {
                                iReturn.Status = CONSTANT_STATUS_API.THAT_BAI;
                            }
                            #region Lưu lịch sử
                            ConnectHistory itemHis = new ConnectHistory();
                            itemHis.ConnectApplicationId = new Guid(appId);
                            itemHis.ConnectHistoryId = Guid.NewGuid();
                            itemHis.Content = "Hủy hóa đơn " + invoice.InvoiceCode;
                            itemHis.CreatedOnDate = DateTime.Now;
                            itemHis.InvoiceId = invoice.InvoiceId;
                            itemHis.Status = true;
                            unitOfWork.GetRepository<ConnectHistory>().Add(itemHis);
                            #endregion
                        }
                    }
                    unitOfWork.Save();
                    #endregion
                    return new Response<List<DataOutputModel>>(1, "Success", dataReturn);
                }
            }
            catch (NullReferenceException ex)
            {
                logger.Debug(ex.Message);
                return new Response<List<DataOutputModel>>(0, "Lỗi: Sai cấu trúc dữ liệu đầu vào", null);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<List<DataOutputModel>>(0, "Lỗi: " + ex.Message, null);
            }
        }

        /// <summary>
        /// Cập nhật dữ liệu phát hành hóa đơn về PMKT
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Response<List<DataOutputStatusPublishModel>> GetInvoiceNumber(string appId, List<string> value)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var dataInvoiceForCheck = (from invoice in unitOfWork.GetRepository<Invoice>().GetAll()
                                               join invoiceConnect in unitOfWork.GetRepository<ConnectWithInvoice>().GetAll()
                                               on invoice.InvoiceId equals invoiceConnect.InvoiceId
                                               where invoiceConnect.ConnectApplicationId == new Guid(appId)
                                               //&& invoice.InvoiceNumber != null && invoice.PublishDate != null
                                               && value.Contains(invoiceConnect.OtherId)
                                               select new DataOutputStatusPublishModel()
                                               {
                                                   IdInvoicePMKT = invoiceConnect.OtherId,
                                                   PublishDate = invoice.PublishDate,
                                                   InvoiceNumber = (invoice.PublishDate != null) ? invoice.InvoiceNumber : null,
                                                   InvoiceType = invoice.AdjustmentType,
                                                   InvoiceStatus = invoice.InvoiceStatus,
                                                   VoucherDate = invoice.InvoiceIssuedDate
                                               }).ToList();
                    var dataInvoiceForUpdate = (from invoice in unitOfWork.GetRepository<Invoice>().GetAll()
                                                join invoiceConnect in unitOfWork.GetRepository<ConnectWithInvoice>().GetAll()
                                                on invoice.InvoiceId equals invoiceConnect.InvoiceId
                                                where invoiceConnect.ConnectApplicationId == new Guid(appId)
                                                && invoice.InvoiceNumber != null && invoice.PublishDate != null
                                                && value.Contains(invoiceConnect.OtherId)
                                                select invoiceConnect).ToList();
                    foreach (var item in dataInvoiceForUpdate)
                    {
                        item.Status = CONSTANT_STATUS_GIVE.DA_GUI;
                        unitOfWork.GetRepository<ConnectWithInvoice>().Update(item);
                        #region Lưu lịch sử
                        ConnectHistory itemHis = new ConnectHistory();
                        itemHis.ConnectApplicationId = new Guid(appId);
                        itemHis.ConnectHistoryId = Guid.NewGuid();
                        itemHis.Content = "PM HĐĐT đã gửi thông tin phát hành hóa đơn " + item.InvoiceId + " về PMKT";
                        itemHis.CreatedOnDate = DateTime.Now;
                        itemHis.InvoiceId = item.InvoiceId;
                        itemHis.Status = true;
                        unitOfWork.GetRepository<ConnectHistory>().Add(itemHis);
                        #endregion
                    }
                    unitOfWork.Save();
                    return new Response<List<DataOutputStatusPublishModel>>(1, "Success", dataInvoiceForCheck);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<List<DataOutputStatusPublishModel>>(0, "Lỗi: " + ex.Message, null);
            }
        }

        /// <summary>
        /// Cập nhật trạng thái PMKT đã nhận hóa đơn
        /// </summary>
        /// <param name="dataInvoice"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Response<bool> UpdateGiveStatus(List<string> dataInvoice, string appId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var db = unitOfWork.DataContext;

                    #region Kiểm tra dữ liệu đầu vào    
                    if (dataInvoice == null)
                    {
                        return new Response<bool>(0, "Lỗi: Không có dữ liệu", false);
                    }

                    if (dataInvoice == null || dataInvoice.Count == 0)
                    {
                        return new Response<bool>(0, "Lỗi: Không có dữ liệu hóa đơn cần cập nhật", false);
                    }
                    #endregion

                    #region Cập nhật thông tin trạng thái hóa đơn   
                    var dataDeleteConnect = (from invoice in unitOfWork.GetRepository<Invoice>().GetAll()
                                             join invoiceConnect in unitOfWork.GetRepository<ConnectWithInvoice>().GetAll()
                                             on invoice.InvoiceId equals invoiceConnect.InvoiceId
                                             where invoiceConnect.ConnectApplicationId == new Guid(appId)
                                             && dataInvoice.Contains(invoiceConnect.OtherId)
                                             select invoiceConnect).ToList();
                    foreach (var invoice in dataDeleteConnect)
                    {
                        invoice.Status = CONSTANT_STATUS_GIVE.DA_NHAN;
                        unitOfWork.GetRepository<ConnectWithInvoice>().Update(invoice);
                        #region Lưu lịch sử
                        ConnectHistory itemHis = new ConnectHistory();
                        itemHis.ConnectApplicationId = new Guid(appId);
                        itemHis.ConnectHistoryId = Guid.NewGuid();
                        itemHis.Content = "PMKT đã nhận hóa đơn " + invoice.OtherId + " sau khi được phát hành từ HĐĐT";
                        itemHis.CreatedOnDate = DateTime.Now;
                        itemHis.InvoiceId = invoice.InvoiceId;
                        itemHis.Status = true;
                        unitOfWork.GetRepository<ConnectHistory>().Add(itemHis);
                        #endregion
                    }
                    unitOfWork.Save();
                    #endregion
                    return new Response<bool>(1, "Success", true);
                }
            }
            catch (NullReferenceException ex)
            {
                logger.Debug(ex.Message);
                return new Response<bool>(0, "Lỗi: Sai cấu trúc dữ liệu đầu vào", false);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<bool>(0, "Lỗi: " + ex.Message, false);
            }
        }

        /// <summary>
        /// Xử lý hóa đơn
        /// </summary>
        /// <param name="dataInput"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Response<DataOutputModel> ChangeInvoice(ChangeInvoiceModel dataInput, string appId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var db = unitOfWork.DataContext;
                    InvoiceModel data = dataInput.DataInvoice;
                    DataOutputModel dataReturn = new DataOutputModel();
                    dataReturn.IdInvoicePMKT = data.IdInvoicePMKT;
                    dataReturn.Status = CONSTANT_STATUS_API.THAT_BAI;

                    #region Kiểm tra dữ liệu đầu vào    
                    #region Dữ liệu hóa đơn
                    if (data == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Dữ liệu hóa đơn đầu vào trống", dataReturn);
                    }
                    #endregion
                    #region Dữ liệu loại xử lý
                    if (dataInput.ChangeType < 1 || dataInput.ChangeType > 5)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Dữ liệu loại xử lý không hợp lệ ( 1<=ChangeType<=5 )", dataReturn);
                    }
                    #endregion
                    #region Dữ liệu hóa đơn gốc đối với loại xử lý là 1: Điều chỉnh tăng, 2: Điều chỉnh giảm, 3: Điều chỉnh thông tin, 4: Thay thế hóa đơn
                    Invoice dataInvoiceOriginal = null;
                    if (dataInput.ChangeType != 5 && string.IsNullOrWhiteSpace(data.OriginalInvoiceId))
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Bắt buộc nhập dữ liệu hóa đơn gốc OriginalInvoiceId đối với các hóa đơn 1: Điều chỉnh tăng, 2: Điều chỉnh giảm, 3: Điều chỉnh thông tin, 4: Thay thế hóa đơn", dataReturn);
                    }
                    if (dataInput.ChangeType != 5)
                    {
                        var invoiceOriginalConnect = unitOfWork.GetRepository<ConnectWithInvoice>().GetMany(x => x.OtherId == data.OriginalInvoiceId).FirstOrDefault();
                        if (invoiceOriginalConnect == null)
                        {
                            return new Response<DataOutputModel>(0, "Lỗi: Không tìm thấy dữ liệu hóa đơn gốc", dataReturn);
                        }
                        dataInvoiceOriginal = unitOfWork.GetRepository<Invoice>().GetMany(x => x.InvoiceId == invoiceOriginalConnect.InvoiceId).FirstOrDefault();
                    }
                    #endregion
                    #region Id hóa đơn bên PMKT
                    if (string.IsNullOrWhiteSpace(data.IdInvoicePMKT))
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: IdInvoicePMKT trống", dataReturn);
                    }
                    var checkInvoiceInDB = unitOfWork.GetRepository<ConnectWithInvoice>().GetMany(x => x.OtherId == data.IdInvoicePMKT && x.ConnectApplicationId == new Guid(appId)).FirstOrDefault();
                    if (checkInvoiceInDB == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: hóa đơn không thuộc ứng dụng", dataReturn);
                    }
                    #endregion
                    #region Kiểm tra hóa đơn đã phát hành chưa
                    var invoiceCheck = unitOfWork.GetRepository<Invoice>().GetMany(x => x.InvoiceId == checkInvoiceInDB.InvoiceId).FirstOrDefault();
                    if (invoiceCheck == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Không tìm thấy thông tin hóa đơn trong cơ sở dữ liệu", dataReturn);
                    }
                    //Đối với trường hợp 1: Điều chỉnh tăng, 2: Điều chỉnh giảm, 3: Điều chỉnh thông tin, 4: Thay thế hóa đơn thì hóa đôn gốc bắt buộc phải đã phát hành rồi
                    if (string.IsNullOrWhiteSpace(invoiceCheck.InvoiceNumber) || invoiceCheck.PublishDate == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Hóa đơn chưa phát hành không thế xử lý", dataReturn);
                    }
                    #endregion
                    #region Công ty phát hành
                    if (!string.IsNullOrWhiteSpace(data.CompanyTax))
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: CompanyTax trống", dataReturn);
                    }
                    var checkCompany = unitOfWork.GetRepository<Company>().GetMany(x => x.TaxNumber == data.CompanyTax).FirstOrDefault();
                    if (checkCompany == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Không tìm thấy dữ liệu công ty với mã số thuế =" + data.CompanyTax, dataReturn);
                    }
                    #endregion
                    #region Mẫu hóa đơn
                    if (string.IsNullOrWhiteSpace(data.TemplateCode))
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: TemplateCode trống", dataReturn);
                    }
                    var checkTemp = unitOfWork.GetRepository<InvoiceTemplate>().GetMany(x => x.TemplateName == data.TemplateCode).FirstOrDefault();
                    if (checkTemp == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Không tìm thấy dữ liệu mẫu hóa đơn với mã số =" + data.TemplateCode, dataReturn);
                    }
                    #endregion
                    #region Seri mẫu
                    if (string.IsNullOrWhiteSpace(data.InvoiceSeries))
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: InvoiceSeries trống", dataReturn);
                    }
                    var checkTempPublish = unitOfWork.GetRepository<Publish_Invoice_Template>().GetMany(x => x.InvoiceTemplateId == checkTemp.InvoiceTemplateId
                    && x.InvoiceSeries == data.InvoiceSeries && x.CompanyId == checkCompany.CompanyId && x.Status == 1).FirstOrDefault();
                    if (checkTempPublish == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Không tìm thấy dữ liệu dải hóa đơn", dataReturn);
                    }
                    #endregion
                    data.InvoiceCategoryId = checkTempPublish.InvoiceCategoryId.Value;
                    data.InvoiceTemplateId = checkTempPublish.InvoiceTemplateId.Value;
                    data.CompanyId = checkTempPublish.CompanyId.Value;
                    data.InvoiceSeries = checkTempPublish.InvoiceSeries;
                    #region Loại hóa đơn
                    string categoryName = "";
                    if (data.InvoiceCategoryId == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: InvoiceCategoryId trống", dataReturn);
                    }
                    var checkCategory = unitOfWork.GetRepository<InvoiceCategory>().GetMany(x => x.InvoiceCategoryId == data.InvoiceCategoryId).FirstOrDefault();
                    if (checkCategory == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Không tìm thấy dữ liệu với InvoiceCategoryId =" + data.InvoiceCategoryId, dataReturn);
                    }
                    categoryName = checkCategory.Name;
                    #endregion
                    #region Khách hàng
                    if (!string.IsNullOrWhiteSpace(data.CustomerCode))
                    {
                        var checkCustomer = unitOfWork.GetRepository<Customer>().GetMany(x => x.CustomerCode == data.CustomerCode).FirstOrDefault();
                        if (checkCustomer == null)
                        {
                            return new Response<DataOutputModel>(0, "Lỗi: Không tìm thấy dữ liệu khách hàng với CustomerCode =" + data.CustomerCode, dataReturn);
                        }
                        #region Set lại thông tin khách hàng theo khách hàng tìm được trong DB
                        data.CustomerId = checkCustomer.CustomerId;
                        data.BuyerAddressLine = checkCustomer.Address;
                        data.BuyerBankAccount = checkCustomer.BankAccountName;
                        data.BuyerBankName = checkCustomer.BankName;
                        data.BuyerDisplayName = checkCustomer.RepresentPerson;
                        data.BuyerEmail = checkCustomer.Email;
                        data.BuyerFaxNumber = checkCustomer.Fax;
                        data.BuyerLegalName = checkCustomer.Name;
                        data.BuyerPhoneNumber = checkCustomer.Phone;
                        data.BuyerTaxCode = checkCustomer.TaxNumber;
                        #endregion
                    }
                    #endregion
                    #region Hình thức thanh toán
                    if (string.IsNullOrWhiteSpace(data.PaymentMethod))
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: PaymentMethod trống", dataReturn);
                    }
                    var dataPayment = new DbCommonHandler().GetPaymentMethod().Data;
                    var checkPayment = dataPayment.FirstOrDefault(x => x.Name == data.PaymentMethod);
                    if (checkPayment == null)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Không tìm thấy dữ liệu với PaymentMethod =" + data.PaymentMethod, dataReturn);
                    }
                    #endregion
                    #region Thông tin mặt hàng
                    if (data.ListProductInvoice == null || data.ListProductInvoice.Count == 0)
                    {
                        return new Response<DataOutputModel>(0, "Lỗi: Dữ liệu mặt hàng theo hóa đơn trống", dataReturn);
                    }
                    #endregion
                    #endregion

                    #region Chuyển đổi dữ liệu đầu vào này sang dữ liệu đầu vào của API Internal
                    CreateInvoiceModel dataInputInternal = new CreateInvoiceModel();
                    List<ProductInvoiceInternalModel> listProductInvoice = new List<ProductInvoiceInternalModel>();

                    #region Convert thông tin mặt hàng
                    foreach (var p in data.ListProductInvoice)
                    {
                        ProductInvoiceInternalModel ip = new ProductInvoiceInternalModel();
                        if (!string.IsNullOrWhiteSpace(p.ExpirationDate))
                        {
                            ip.ExpirationDate = Utils.TryToConvertDateTimeFromString(p.ExpirationDate);
                        }
                        ip.ItemName = p.ItemName;
                        ip.ItemTotalAmountWithoutVat = (p.UnitPrice * p.Quantity);
                        if (p.TotalAmount != null)
                            ip.ItemTotalAmountWithoutVat = p.TotalAmount;
                        ip.ItemTotalAmountWithVat = (ip.ItemTotalAmountWithoutVat * (p.VatPercentage / 100)) + ip.ItemTotalAmountWithoutVat;
                        ip.LineNumber = p.LineNumber;
                        ip.LotNumber = p.LotNumber;
                        if (p.ItemType == InvoiceConstants.ProductInvoiceType.HANG_HOA_BINH_THUONG)
                            ip.Promotion = false;
                        else
                            ip.Promotion = true;
                        ip.ItemType = p.ItemType;
                        ip.Quantity = p.Quantity;
                        ip.UnitName = p.UnitName;
                        ip.UnitPrice = p.UnitPrice;
                        ip.VatAmount = (ip.ItemTotalAmountWithoutVat * (p.VatPercentage / 100));
                        ip.VatPercentage = p.VatPercentage;
                        ip.ItemCode = p.ProductCode;
                        listProductInvoice.Add(ip);
                    }
                    #endregion

                    #region Thông tin hóa đơn chi tiết
                    dataInputInternal.ListProductInvoice = listProductInvoice;
                    dataInputInternal.AdjustmentType = InvoiceConstants.InvoiceAdjustmentType.HOA_DON_THONG_THUONG;
                    dataInputInternal.BuyerAddressLine = data.BuyerAddressLine;
                    dataInputInternal.BuyerBankAccount = data.BuyerBankAccount;
                    dataInputInternal.BuyerBankName = data.BuyerBankName;
                    dataInputInternal.BuyerCityName = "";
                    dataInputInternal.BuyerCountryCode = "";
                    dataInputInternal.BuyerDisplayName = data.BuyerDisplayName;
                    dataInputInternal.BuyerDistrictName = "";
                    dataInputInternal.BuyerEmail = data.BuyerEmail;
                    dataInputInternal.BuyerFaxNumber = data.BuyerFaxNumber;
                    dataInputInternal.BuyerLegalName = data.BuyerLegalName;
                    dataInputInternal.BuyerPhoneNumber = data.BuyerPhoneNumber;
                    dataInputInternal.BuyerTaxCode = data.BuyerTaxCode;
                    dataInputInternal.CompanyId = data.CompanyId;
                    dataInputInternal.CreatedByUserId = new Guid(CONSTANT_APP.ADMIN_ID);
                    dataInputInternal.CreatedOnDate = DateTime.Now;
                    dataInputInternal.CurrencyCode = data.CurrencyCode;
                    dataInputInternal.CustomerId = data.CustomerId;
                    dataInputInternal.InvoiceCategoryId = data.InvoiceCategoryId;
                    dataInputInternal.InvoiceIssuedDate = Utils.TryToConvertDateTimeFromString(data.InvoiceIssuedDate);
                    dataInputInternal.InvoiceName = categoryName;
                    dataInputInternal.InvoiceNote = data.InvoiceNote;
                    dataInputInternal.InvoiceSeries = data.InvoiceSeries;
                    dataInputInternal.InvoiceStatus = InvoiceConstants.InvoiceStatus.HOA_DON_MOI_TAO_LAP;
                    dataInputInternal.InvoiceTemplateId = data.InvoiceTemplateId;
                    dataInputInternal.LastModifiedByUserId = new Guid(CONSTANT_APP.ADMIN_ID);
                    dataInputInternal.LastModifiedOnDate = DateTime.Now;
                    dataInputInternal.PaymentMethod = checkPayment.PaymentMethodId;
                    dataInputInternal.SellerAddressLine = checkCompany.Address;
                    dataInputInternal.SellerBankAccount = checkCompany.BankAccountName;
                    dataInputInternal.SellerBankName = checkCompany.BankName;
                    dataInputInternal.SellerContactPersonName = checkCompany.ContactPerson;
                    dataInputInternal.SellerEmail = checkCompany.Email;
                    dataInputInternal.SellerFaxNumber = checkCompany.Fax;
                    dataInputInternal.SellerLegalName = checkCompany.Name;
                    dataInputInternal.SellerPhoneNumber = checkCompany.PhoneNumber;
                    dataInputInternal.SellerTaxCode = checkCompany.TaxNumber;
                    dataInputInternal.SumOfTotalLineAmountWithoutVat = listProductInvoice.Sum(x => x.ItemTotalAmountWithoutVat);
                    dataInputInternal.TemplateCode = data.TemplateCode;
                    dataInputInternal.TotalAmountWithoutVat = listProductInvoice.Sum(x => x.ItemTotalAmountWithoutVat);
                    if (data.TotalAmount != null)//Cho phép sửa số tiền tổng hóa đơn
                        dataInputInternal.TotalAmountWithoutVat = data.TotalAmount;
                    dataInputInternal.TotalAmountWithVat = listProductInvoice.Sum(x => x.ItemTotalAmountWithVat);
                    dataInputInternal.TotalVatamount = dataInputInternal.TotalAmountWithVat - dataInputInternal.TotalAmountWithoutVat;
                    if (data.TotalAmountVAT != null)//Cho phép sửa sô tiền thuế
                        dataInputInternal.TotalVatamount = data.TotalAmountVAT;
                    dataInputInternal.VatPercentage = data.VatPercentage;
                    dataInputInternal.ExchangeRate = data.ExchangeRate;
                    dataInputInternal.AmountExchanged = data.AmountExchanged;
                    #region Thông tin trường đặc thù theo ngành nghề
                    dataInputInternal.FromUseDate = Utils.TryToConvertDateTimeFromString(data.FromUseDate);
                    dataInputInternal.ToUseDate = Utils.TryToConvertDateTimeFromString(data.ToUseDate);
                    dataInputInternal.EnvironmentTax = data.EnvironmentTax;
                    dataInputInternal.EnvironmentFee = data.EnvironmentFee;
                    dataInputInternal.StartNumber = data.StartNumber;
                    dataInputInternal.EndNumber = data.StartNumber;
                    dataInputInternal.UseNumber = data.UseNumber;
                    #endregion
                    if (dataInvoiceOriginal != null)
                    {
                        dataInputInternal.OriginalInvoiceId = dataInvoiceOriginal.InvoiceId;
                    }
                    #endregion
                    #endregion

                    Response<BasicInvoiceModel> rsDataExcute = null;
                    if (dataInput.ChangeType == 1)//Điều chỉnh tăng
                    {
                        rsDataExcute = AdjustmentIncrease(dataInputInternal);
                    }
                    else if (dataInput.ChangeType == 2)//Điều chỉnh giảm
                    {
                        rsDataExcute = AdjustmentDecrease(dataInputInternal);
                    }
                    else if (dataInput.ChangeType == 3)//Điều chỉnh thông tin
                    {
                        rsDataExcute = AdjustmentInfo(dataInputInternal);
                    }
                    else if (dataInput.ChangeType == 4)//Thay thế hóa đơn
                    {
                        rsDataExcute = Replace(dataInputInternal);
                    }
                    else if (dataInput.ChangeType == 5)//Hủy bỏ hóa đơn
                    {
                        var invoiceIn = unitOfWork.GetRepository<ConnectWithInvoice>().GetMany(x => x.OtherId == dataInput.DataInvoice.IdInvoicePMKT).FirstOrDefault();
                        if (invoiceIn != null)
                            rsDataExcute = Cancel(invoiceIn.InvoiceId, new Guid(CONSTANT_APP.ADMIN_ID));
                    }
                    if (rsDataExcute.Status == 1)
                    {
                        #region Lưu thông tin ràng buộc giữa hóa đơn PMKT và HĐĐT
                        var dataInvoiceFromHDDT = rsDataExcute.Data;
                        ConnectHistory itemHis = new ConnectHistory();
                        itemHis.ConnectApplicationId = new Guid(appId);
                        itemHis.ConnectHistoryId = Guid.NewGuid();
                        if (dataInput.ChangeType == 1)//Điều chỉnh tăng
                        {
                            itemHis.Content = "Điều chỉnh tăng hóa đơn " + dataInvoiceFromHDDT.InvoiceCode;
                        }
                        else if (dataInput.ChangeType == 2)//Điều chỉnh giảm
                        {
                            itemHis.Content = "Điều chỉnh giảm hóa đơn " + dataInvoiceFromHDDT.InvoiceCode;
                        }
                        else if (dataInput.ChangeType == 3)//Điều chỉnh thông tin
                        {
                            itemHis.Content = "Điều chỉnh thông tin hóa đơn " + dataInvoiceFromHDDT.InvoiceCode;
                        }
                        else if (dataInput.ChangeType == 4)//Thay thế hóa đơn
                        {
                            itemHis.Content = "Thay thế hóa đơn " + dataInvoiceFromHDDT.InvoiceCode;
                        }
                        else if (dataInput.ChangeType == 5)//Hủy hóa đơn
                        {
                            itemHis.Content = "Hủy hóa đơn " + dataInvoiceFromHDDT.InvoiceCode;
                        }
                        itemHis.CreatedOnDate = DateTime.Now;
                        itemHis.InvoiceId = dataInvoiceFromHDDT.InvoiceId;
                        itemHis.Status = true;
                        unitOfWork.GetRepository<ConnectHistory>().Add(itemHis);
                        #endregion
                        unitOfWork.Save();
                        dataReturn.Status = CONSTANT_STATUS_API.THANH_CONG;
                        return new Response<DataOutputModel>(1, "Success", dataReturn);
                    }
                    else
                    {
                        dataReturn.Status = CONSTANT_STATUS_API.THAT_BAI;
                        return new Response<DataOutputModel>(0, "Lỗi không lưu được dữ liệu vào hệ thống HĐĐT", dataReturn);
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                logger.Debug(ex.Message);
                return new Response<DataOutputModel>(0, "Lỗi: Sai cấu trúc dữ liệu đầu vào", null);
            }
            catch (Exception ex)
            {
                DataOutputModel dataReturn = new DataOutputModel();
                dataReturn.IdInvoicePMKT = dataInput.DataInvoice.IdInvoicePMKT;
                dataReturn.Status = CONSTANT_STATUS_API.THAT_BAI;
                logger.Debug(ex.Message);
                return new Response<DataOutputModel>(0, "Lỗi: " + ex.Message, dataReturn);
            }
        }

        /// <summary>
        /// Tạo mới hóa đơn
        /// </summary>
        /// <param name="createModel"></param>
        /// <returns></returns>
        public Response<BasicInvoiceModel> Create(CreateInvoiceModel createModel)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var invoiceRepo = unitOfWork.GetRepository<Invoice>();
                    var productinvoiceRepo = unitOfWork.GetRepository<Product_Invoice>();

                    #region Validation data before process

                    if (createModel == null)
                    {
                        return new Response<BasicInvoiceModel>(0, "Dữ liệu trống: createModel = null", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.KindOfService) && (createModel.KindOfService.Length > 64))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu KindOfService > 64", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.ProcessInvoiceNote) && (createModel.ProcessInvoiceNote.Length > 256))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu ProcessInvoiceNote > 256", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.CertifiedId) && (createModel.CertifiedId.Length > 32))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu CertifiedId > 32", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.CertifiedData) && (createModel.CertifiedData.Length > 128))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu CertifiedData > 128", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.InvoiceType) && (createModel.InvoiceType.Length > 8))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu InvoiceType > 8", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.TemplateCode) && (createModel.TemplateCode.Length > 16))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu TemplateCode > 16", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.InvoiceSeries) && (createModel.InvoiceSeries.Length > 8))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu InvoiceSeries > 8", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.InvoiceNumber) && (createModel.InvoiceNumber.Length > 8))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu InvoiceNumber > 8", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.ContractNumber) && (createModel.ContractNumber.Length > 64))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu ContractNumber > 64", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.CurrencyCode) && (createModel.CurrencyCode.Length > 4))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu CurrencyCode > 4", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.InvoiceNote) && (createModel.InvoiceNote.Length > 512))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu InvoiceNote > 512", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.AdditionalReferenceDesc) && (createModel.AdditionalReferenceDesc.Length > 256))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu AdditionalReferenceDesc > 256", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.SellerLegalName) && (createModel.SellerLegalName.Length > 64))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu SellerLegalName > 256", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.SellerTaxCode) && (createModel.SellerTaxCode.Length > 64))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu SellerTaxCode > 16", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.SellerAddressLine) && (createModel.SellerAddressLine.Length > 512))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu SellerAddressLine > 512", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.SellerPostalCode) && (createModel.SellerPostalCode.Length > 16))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu SellerPostalCode > 16", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.SellerDistrictName) && (createModel.SellerDistrictName.Length > 64))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu SellerDistrictName > 64", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.SellerCityName) && (createModel.SellerCityName.Length > 64))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu SellerCityName > 64", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.SellerCountryCode) && (createModel.SellerCountryCode.Length > 4))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu SellerCountryCode > 4", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.SellerPhoneNumber) && (createModel.SellerPhoneNumber.Length > 32))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu SellerPhoneNumber > 32", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.SellerFaxNumber) && (createModel.SellerFaxNumber.Length > 32))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu SellerFaxNumber > 32", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.SellerEmail) && (createModel.SellerEmail.Length > 64))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu SellerEmail > 64", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.SellerBankName) && (createModel.SellerBankName.Length > 128))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu SellerBankName > 128", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.SellerBankAccount) && (createModel.SellerBankAccount.Length > 32))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu SellerBankAccount > 32", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.SellerContactPersonName) && (createModel.SellerContactPersonName.Length > 128))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu SellerContactPersonName > 128", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.SellerSignedPersonName) && (createModel.SellerSignedPersonName.Length > 128))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu SellerSignedPersonName > 128", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.SellerSubmittedPersonName) && (createModel.SellerSubmittedPersonName.Length > 128))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu SellerSubmittedPersonName > 128", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.BuyerDisplayName) && (createModel.BuyerDisplayName.Length > 256))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu BuyerDisplayName > 256", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.BuyerLegalName) && (createModel.BuyerLegalName.Length > 256))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu BuyerLegalName > 256", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.BuyerTaxCode) && (createModel.BuyerTaxCode.Length > 16))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu BuyerTaxCode > 16", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.BuyerAddressLine) && (createModel.BuyerAddressLine.Length > 512))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu BuyerAddressLine > 512", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.BuyerPostalCode) && (createModel.BuyerPostalCode.Length > 16))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu BuyerPostalCode > 16", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.BuyerDistrictName) && (createModel.BuyerDistrictName.Length > 64))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu BuyerDistrictName > 64", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.BuyerCityName) && (createModel.BuyerCityName.Length > 64))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu BuyerCityName > 64", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.BuyerCountryCode) && (createModel.BuyerCountryCode.Length > 4))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu BuyerCountryCode > 4", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.BuyerPhoneNumber) && (createModel.BuyerPhoneNumber.Length > 32))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu BuyerPhoneNumber > 32", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.BuyerFaxNumber) && (createModel.BuyerFaxNumber.Length > 32))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu BuyerFaxNumber > 32", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.BuyerEmail) && (createModel.BuyerEmail.Length > 64))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu BuyerEmail > 64", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.BuyerBankName) && (createModel.BuyerBankName.Length > 128))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu BuyerBankName > 128", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.BuyerBankAccount) && (createModel.BuyerBankAccount.Length > 32))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu BuyerBankAccount > 32", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.ProductCodes) && (createModel.ProductCodes.Length > 512))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu ProductCodes > 512", null);
                    }

                    if (!string.IsNullOrEmpty(createModel.PortalLink) && (createModel.PortalLink.Length > 256))
                    {
                        return new Response<BasicInvoiceModel>(0, "Độ dài trường dữ liệu PortalLink > 256", null);
                    }

                    #endregion

                    #region Check require object before process

                    if (string.IsNullOrWhiteSpace(createModel.InvoiceSeries))
                    {
                        return new Response<BasicInvoiceModel>(0, "Ký hiệu hóa đơn trống", null);
                    }

                    if (!createModel.InvoiceIssuedDate.HasValue)
                    {
                        return new Response<BasicInvoiceModel>(0, "Ngày tạo hóa đơn trống", null);
                    }

                    if (!createModel.PaymentMethod.HasValue)
                    {
                        return new Response<BasicInvoiceModel>(0, "Hình thức thanh toán trống", null);
                    }

                    if ((createModel.ListProductInvoice == null) || (createModel.ListProductInvoice.Count() == 0))
                    {
                        return new Response<BasicInvoiceModel>(0, "Không tồn tại thông tin hàng hóa trong hóa đơn", null);
                    }

                    #endregion

                    #region Set value to model

                    Invoice invoiceEntity = new Invoice();
                    invoiceEntity.InvoiceId = Guid.NewGuid();
                    invoiceEntity.InvoiceCategoryId = createModel.InvoiceCategoryId;
                    invoiceEntity.InvoiceTemplateId = createModel.InvoiceTemplateId;
                    invoiceEntity.CompanyId = createModel.CompanyId;
                    invoiceEntity.CustomerId = createModel.CustomerId;
                    invoiceEntity.Signature = createModel.Signature;
                    invoiceEntity.Converted = createModel.Converted;
                    invoiceEntity.KindOfService = createModel.KindOfService;
                    invoiceEntity.PaymentStatus = createModel.PaymentStatus;
                    invoiceEntity.ArisingDate = createModel.ArisingDate;
                    invoiceEntity.ProcessInvoiceNote = createModel.ProcessInvoiceNote;
                    invoiceEntity.GrossValue = createModel.GrossValue;
                    invoiceEntity.VatAmount0 = createModel.VatAmount0;
                    invoiceEntity.GrossValue0 = createModel.GrossValue0;
                    invoiceEntity.VatAmount5 = createModel.VatAmount5;
                    invoiceEntity.GrossValue5 = createModel.GrossValue5;
                    invoiceEntity.VatAmount10 = createModel.VatAmount10;
                    invoiceEntity.GrossValue10 = createModel.GrossValue10;
                    invoiceEntity.Certified = createModel.Certified;
                    invoiceEntity.CertifiedId = createModel.CertifiedId;
                    invoiceEntity.CertifiedData = createModel.CertifiedData;
                    invoiceEntity.CreationDate = createModel.CreationDate;
                    invoiceEntity.CreationDate = createModel.CreationDate;
                    invoiceEntity.PublishDate = createModel.PublishDate;
                    invoiceEntity.CreationBy = createModel.CreationBy;
                    invoiceEntity.PublishBy = createModel.PublishBy;
                    invoiceEntity.PaymentMethod = createModel.PaymentMethod;
                    invoiceEntity.SellerAppRecordId = createModel.SellerAppRecordId;
                    invoiceEntity.InvoiceAppRecordId = createModel.InvoiceAppRecordId;
                    invoiceEntity.InvoiceType = createModel.InvoiceType;
                    invoiceEntity.TemplateCode = createModel.TemplateCode;
                    invoiceEntity.InvoiceSeries = createModel.InvoiceSeries;
                    invoiceEntity.InvoiceNumber = createModel.InvoiceNumber;
                    invoiceEntity.InvoiceName = createModel.InvoiceName;
                    invoiceEntity.InvoiceIssuedDate = createModel.InvoiceIssuedDate != null ? new DateTime(createModel.InvoiceIssuedDate.Value.Year, createModel.InvoiceIssuedDate.Value.Month, createModel.InvoiceIssuedDate.Value.Day) : DateTime.Today;
                    invoiceEntity.SignedDate = createModel.SignedDate;
                    invoiceEntity.SubmittedDate = createModel.SubmittedDate;
                    invoiceEntity.ContractNumber = createModel.ContractNumber;
                    invoiceEntity.ContractDate = createModel.ContractDate;
                    invoiceEntity.CurrencyCode = createModel.CurrencyCode;
                    invoiceEntity.ExchangeRate = createModel.ExchangeRate;
                    invoiceEntity.InvoiceNote = createModel.InvoiceNote;
                    invoiceEntity.OriginalInvoiceId = createModel.OriginalInvoiceId;
                    invoiceEntity.AdditionalReferenceDesc = createModel.AdditionalReferenceDesc;
                    invoiceEntity.AdditionalReferenceDate = createModel.AdditionalReferenceDate;
                    invoiceEntity.SellerLegalName = createModel.SellerLegalName;
                    invoiceEntity.SellerTaxCode = createModel.SellerTaxCode;
                    invoiceEntity.SellerAddressLine = createModel.SellerAddressLine;
                    invoiceEntity.SellerPostalCode = createModel.SellerPostalCode;
                    invoiceEntity.SellerDistrictName = createModel.SellerDistrictName;
                    invoiceEntity.SellerCityName = createModel.SellerCityName;
                    invoiceEntity.SellerCountryCode = createModel.SellerCountryCode;
                    invoiceEntity.SellerPhoneNumber = createModel.SellerPhoneNumber;
                    invoiceEntity.SellerFaxNumber = createModel.SellerFaxNumber;
                    invoiceEntity.SellerEmail = createModel.SellerEmail;
                    invoiceEntity.SellerBankName = createModel.SellerBankName;
                    invoiceEntity.SellerBankAccount = createModel.SellerBankAccount;
                    invoiceEntity.SellerContactPersonName = createModel.SellerContactPersonName;
                    invoiceEntity.SellerSignedPersonName = createModel.SellerSignedPersonName;
                    invoiceEntity.SellerSubmittedPersonName = createModel.SellerSubmittedPersonName;
                    invoiceEntity.BuyerDisplayName = createModel.BuyerDisplayName;
                    invoiceEntity.BuyerLegalName = createModel.BuyerLegalName;
                    invoiceEntity.BuyerTaxCode = createModel.BuyerTaxCode;
                    invoiceEntity.BuyerAddressLine = createModel.BuyerAddressLine;
                    invoiceEntity.BuyerPostalCode = createModel.BuyerPostalCode;
                    invoiceEntity.BuyerDistrictName = createModel.BuyerDistrictName;
                    invoiceEntity.BuyerCityName = createModel.BuyerCityName;
                    invoiceEntity.BuyerCountryCode = createModel.BuyerCountryCode;
                    invoiceEntity.BuyerPhoneNumber = createModel.BuyerPhoneNumber;
                    invoiceEntity.BuyerFaxNumber = createModel.BuyerFaxNumber;
                    invoiceEntity.BuyerEmail = createModel.BuyerEmail;
                    invoiceEntity.BuyerBankName = createModel.BuyerBankName;
                    invoiceEntity.BuyerBankAccount = createModel.BuyerBankAccount;
                    invoiceEntity.SumOfTotalLineAmountWithoutVAT = createModel.SumOfTotalLineAmountWithoutVat;
                    invoiceEntity.TotalAmountWithoutVAT = createModel.TotalAmountWithoutVat;
                    invoiceEntity.TotalVATAmount = createModel.TotalVatamount;
                    invoiceEntity.TotalAmountWithVAT = createModel.TotalAmountWithVat;
                    invoiceEntity.TotalAmountWithVATFrn = createModel.TotalAmountWithVatfrn;
                    invoiceEntity.TotalAmountWithVATInWords = createModel.TotalAmountWithVatinWords;
                    invoiceEntity.IsTotalAmountPos = createModel.IsTotalAmountPos;
                    invoiceEntity.IsTotalVATAmountPos = createModel.IsTotalVatamountPos;
                    invoiceEntity.IsTotalAmtWithoutVatPos = createModel.IsTotalAmtWithoutVatPos;
                    invoiceEntity.DiscountAmount = createModel.DiscountAmount;
                    invoiceEntity.IsDiscountAmtPos = createModel.IsDiscountAmtPos;
                    invoiceEntity.ProductCodes = createModel.ProductCodes;
                    invoiceEntity.PortalLink = createModel.PortalLink;
                    invoiceEntity.VatPercentage = createModel.VatPercentage;
                    invoiceEntity.AdjustmentType = InvoiceConstants.InvoiceAdjustmentType.HOA_DON_THONG_THUONG;
                    invoiceEntity.InvoiceStatus = InvoiceConstants.InvoiceStatus.HOA_DON_MOI_TAO_LAP;
                    invoiceEntity.TrangThaiXemTraCuuHoaDon = false;
                    invoiceEntity.CreatedByUserId = createModel.CreatedByUserId;
                    invoiceEntity.CreatedOnDate = createModel.CreatedOnDate;
                    invoiceEntity.ConvertedStatus = false;
                    invoiceEntity.InvoiceCode = InvoiceConstants.PREFIX_INVOICE_CODE + (createModel.CreatedOnDate.HasValue ? createModel.CreatedOnDate.Value.ToString(DateTimeFormatConstants.YYMMDDHHMMSSFFFF) : DateTime.Now.ToString(DateTimeFormatConstants.YYMMDDHHMMSSFFFF));
                    invoiceEntity.IsOtherApp = true;//Hóa đơn từ hệ thống khác đồng bộ sang
                    #region Thông tin trường đặc thù theo ngành nghề
                    /// <summary>
                    /// Sử dụng từ ngày (hóa đơn điện, nước..v..v)
                    /// </summary>
                    invoiceEntity.FromUseDate = createModel.FromUseDate;
                    /// <summary>
                    /// Sử dụng đến ngày (hóa đơn điện, nước...v.v..)
                    /// </summary>
                    invoiceEntity.ToUseDate = createModel.ToUseDate;
                    /// <summary>
                    /// Thuế môi trường
                    /// </summary>
                    invoiceEntity.EnvironmentTax = createModel.EnvironmentTax;
                    /// <summary>
                    /// Phí môi trường
                    /// </summary>
                    invoiceEntity.EnvironmentFee = createModel.EnvironmentFee;
                    /// <summary>
                    /// Số đầu kỳ
                    /// </summary>
                    invoiceEntity.StartNumber = createModel.StartNumber;
                    /// <summary>
                    /// Số cuối kỳ
                    /// </summary>
                    invoiceEntity.EndNumber = createModel.StartNumber;
                    /// <summary>
                    /// Số sử dụng trong kỳ
                    /// </summary>
                    invoiceEntity.UseNumber = createModel.UseNumber;
                    #endregion

                    #endregion

                    #region Get total amount with VAT in words

                    try
                    {
                        invoiceEntity.TotalAmountWithVATInWords = Utils.DocTienBangChu((long)invoiceEntity.TotalAmountWithVAT, " đồng");
                    }
                    catch (Exception ex)
                    {
                        logger.Debug(ex.Message);
                        invoiceEntity.TotalAmountWithVATInWords = "Không đồng";
                    }

                    #endregion                    

                    invoiceRepo.Add(invoiceEntity);

                    #region Add Product invoice to database

                    if (createModel.ListProductInvoice != null && createModel.ListProductInvoice.Count > 0)
                    {
                        foreach (var itemPI in createModel.ListProductInvoice)
                        {
                            Product_Invoice pi = new Product_Invoice();
                            pi.ProductInvoiceId = Guid.NewGuid();
                            pi.InvoiceId = invoiceEntity.InvoiceId;
                            pi.LineNumber = itemPI.LineNumber;
                            pi.ItemCode = itemPI.ItemCode;
                            pi.ItemName = itemPI.ItemName;
                            pi.UnitCode = itemPI.UnitCode;
                            pi.UnitName = itemPI.UnitName;
                            pi.UnitPrice = itemPI.UnitPrice;
                            pi.Quantity = itemPI.Quantity;
                            pi.ItemTotalAmountWithoutVat = itemPI.ItemTotalAmountWithoutVat;
                            pi.VatPercentage = itemPI.VatPercentage;
                            pi.VatAmount = itemPI.VatAmount;
                            pi.Promotion = itemPI.Promotion;
                            pi.LotNumber = itemPI.LotNumber;
                            pi.ItemTotalAmountWithVat = itemPI.ItemTotalAmountWithVat;
                            pi.ExpirationDate = itemPI.ExpirationDate;
                            pi.AdjustmentVatAmount = itemPI.AdjustmentVatAmount;
                            pi.IsIncreaseItem = itemPI.IsIncreaseItem;
                            pi.ItemType = itemPI.ItemType;
                            productinvoiceRepo.Add(pi);
                        }
                    }

                    #endregion

                    var result = unitOfWork.Save();
                    if (result >= 1)
                    {
                        return new Response<BasicInvoiceModel>(1, MessageResponseConstants.MESSAGE_ADD_SUCCESS, InvoiceConvert.entityToModel(invoiceEntity));
                    }
                    else
                    {
                        return new Response<BasicInvoiceModel>(-1, MessageResponseConstants.MESSAGE_ADD_FAILED_DB, null);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<BasicInvoiceModel>(-1, MessageResponseConstants.MESSAGE_EXCEPTION + ex.Message, null);
            }
        }

        /// <summary>
        /// Thay thế hóa đơn
        /// </summary>
        /// <param name="createModel"></param>
        /// <returns></returns>
        public Response<BasicInvoiceModel> Replace(CreateInvoiceModel createModel)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var invoiceRepo = unitOfWork.GetRepository<Invoice>();
                    var productinvoiceRepo = unitOfWork.GetRepository<Product_Invoice>();

                    #region Validation data before process

                    if (createModel == null)
                    {
                        return new Response<BasicInvoiceModel>(0, "Dữ liệu trống: model = null", null);
                    }
                    #endregion

                    #region Check require object before process

                    if (string.IsNullOrWhiteSpace(createModel.InvoiceSeries))
                    {
                        return new Response<BasicInvoiceModel>(0, "Ký hiệu hóa đơn trống", null);
                    }

                    if (!createModel.InvoiceIssuedDate.HasValue)
                    {
                        return new Response<BasicInvoiceModel>(0, "Ngày tạo hóa đơn trống", null);
                    }

                    if (!createModel.PaymentMethod.HasValue)
                    {
                        return new Response<BasicInvoiceModel>(0, "Hình thức thanh toán trống", null);
                    }

                    if ((createModel.ListProductInvoice == null) || (createModel.ListProductInvoice.Count() == 0))
                    {
                        return new Response<BasicInvoiceModel>(0, "Không tồn tại thông tin hàng hóa trong hóa đơn", null);
                    }

                    #endregion

                    if (createModel.OriginalInvoiceId.HasValue)
                    {
                        var orgInvoiceEntity = invoiceRepo.GetById(createModel.OriginalInvoiceId.Value);
                        if (orgInvoiceEntity == null)
                        {
                            return new Response<BasicInvoiceModel>(0, MessageResponseConstants.InvoiceMessage.MESSAGE_ORIGINAL_INVOICE_NOT_FOUND, null);
                        }

                        if (!orgInvoiceEntity.InvoiceStatus.Equals(InvoiceConstants.InvoiceStatus.HOA_DON_DA_PHAT_HANH))
                        {
                            return new Response<BasicInvoiceModel>(0, MessageResponseConstants.InvoiceMessage.MESSAGE_INVOICE_REPLACE_FAILED, null);
                        }

                        if (createModel.IsPublishInvoice.HasValue && createModel.IsPublishInvoice.Value)
                        {
                            orgInvoiceEntity.InvoiceStatus = InvoiceConstants.InvoiceStatus.HOA_DON_BI_THAY_THE;
                            orgInvoiceEntity.LastModifiedByUserId = createModel.CreatedByUserId;
                            orgInvoiceEntity.LastModifiedOnDate = DateTime.Now;
                            invoiceRepo.Update(orgInvoiceEntity);
                        }
                    }

                    Invoice invoiceEntity = new Invoice();
                    invoiceEntity.InvoiceId = Guid.NewGuid();
                    invoiceEntity.InvoiceCategoryId = createModel.InvoiceCategoryId;
                    invoiceEntity.InvoiceTemplateId = createModel.InvoiceTemplateId;
                    invoiceEntity.CompanyId = createModel.CompanyId;
                    invoiceEntity.CustomerId = createModel.CustomerId;
                    invoiceEntity.Signature = createModel.Signature;
                    invoiceEntity.Converted = createModel.Converted;
                    invoiceEntity.KindOfService = createModel.KindOfService;
                    invoiceEntity.PaymentStatus = createModel.PaymentStatus;
                    invoiceEntity.ArisingDate = createModel.ArisingDate;
                    invoiceEntity.ProcessInvoiceNote = createModel.ProcessInvoiceNote;
                    invoiceEntity.GrossValue = createModel.GrossValue;
                    invoiceEntity.VatAmount0 = createModel.VatAmount0;
                    invoiceEntity.GrossValue0 = createModel.GrossValue0;
                    invoiceEntity.VatAmount5 = createModel.VatAmount5;
                    invoiceEntity.GrossValue5 = createModel.GrossValue5;
                    invoiceEntity.VatAmount10 = createModel.VatAmount10;
                    invoiceEntity.GrossValue10 = createModel.GrossValue10;
                    invoiceEntity.Certified = createModel.Certified;
                    invoiceEntity.CertifiedId = createModel.CertifiedId;
                    invoiceEntity.CertifiedData = createModel.CertifiedData;
                    invoiceEntity.CreationDate = createModel.CreationDate;
                    invoiceEntity.CreationDate = createModel.CreationDate;
                    invoiceEntity.PublishDate = null;
                    invoiceEntity.CreationBy = createModel.CreationBy;
                    invoiceEntity.PublishBy = createModel.PublishBy;
                    invoiceEntity.PaymentMethod = createModel.PaymentMethod;
                    invoiceEntity.SellerAppRecordId = createModel.SellerAppRecordId;
                    invoiceEntity.InvoiceAppRecordId = createModel.InvoiceAppRecordId;
                    invoiceEntity.InvoiceType = createModel.InvoiceType;
                    invoiceEntity.TemplateCode = createModel.TemplateCode;
                    invoiceEntity.InvoiceSeries = createModel.InvoiceSeries;
                    invoiceEntity.InvoiceName = createModel.InvoiceName;
                    invoiceEntity.InvoiceIssuedDate = createModel.InvoiceIssuedDate != null ? new DateTime(createModel.InvoiceIssuedDate.Value.Year, createModel.InvoiceIssuedDate.Value.Month, createModel.InvoiceIssuedDate.Value.Day) : DateTime.Today;
                    invoiceEntity.SignedDate = null;
                    invoiceEntity.SubmittedDate = createModel.SubmittedDate;
                    invoiceEntity.ContractNumber = createModel.ContractNumber;
                    invoiceEntity.ContractDate = createModel.ContractDate;
                    invoiceEntity.CurrencyCode = createModel.CurrencyCode;
                    invoiceEntity.ExchangeRate = createModel.ExchangeRate;
                    invoiceEntity.InvoiceNote = createModel.InvoiceNote;
                    invoiceEntity.OriginalInvoiceId = createModel.OriginalInvoiceId;
                    invoiceEntity.AdditionalReferenceDesc = createModel.AdditionalReferenceDesc;
                    invoiceEntity.AdditionalReferenceDate = createModel.AdditionalReferenceDate;
                    invoiceEntity.SellerLegalName = createModel.SellerLegalName;
                    invoiceEntity.SellerTaxCode = createModel.SellerTaxCode;
                    invoiceEntity.SellerAddressLine = createModel.SellerAddressLine;
                    invoiceEntity.SellerPostalCode = createModel.SellerPostalCode;
                    invoiceEntity.SellerDistrictName = createModel.SellerDistrictName;
                    invoiceEntity.SellerCityName = createModel.SellerCityName;
                    invoiceEntity.SellerCountryCode = createModel.SellerCountryCode;
                    invoiceEntity.SellerPhoneNumber = createModel.SellerPhoneNumber;
                    invoiceEntity.SellerFaxNumber = createModel.SellerFaxNumber;
                    invoiceEntity.SellerEmail = createModel.SellerEmail;
                    invoiceEntity.SellerBankName = createModel.SellerBankName;
                    invoiceEntity.SellerBankAccount = createModel.SellerBankAccount;
                    invoiceEntity.SellerContactPersonName = createModel.SellerContactPersonName;
                    invoiceEntity.SellerSignedPersonName = createModel.SellerSignedPersonName;
                    invoiceEntity.SellerSubmittedPersonName = createModel.SellerSubmittedPersonName;
                    invoiceEntity.BuyerDisplayName = createModel.BuyerDisplayName;
                    invoiceEntity.BuyerLegalName = createModel.BuyerLegalName;
                    invoiceEntity.BuyerTaxCode = createModel.BuyerTaxCode;
                    invoiceEntity.BuyerAddressLine = createModel.BuyerAddressLine;
                    invoiceEntity.BuyerPostalCode = createModel.BuyerPostalCode;
                    invoiceEntity.BuyerDistrictName = createModel.BuyerDistrictName;
                    invoiceEntity.BuyerCityName = createModel.BuyerCityName;
                    invoiceEntity.BuyerCountryCode = createModel.BuyerCountryCode;
                    invoiceEntity.BuyerPhoneNumber = createModel.BuyerPhoneNumber;
                    invoiceEntity.BuyerFaxNumber = createModel.BuyerFaxNumber;
                    invoiceEntity.BuyerEmail = createModel.BuyerEmail;
                    invoiceEntity.BuyerBankName = createModel.BuyerBankName;
                    invoiceEntity.BuyerBankAccount = createModel.BuyerBankAccount;
                    invoiceEntity.SumOfTotalLineAmountWithoutVAT = createModel.SumOfTotalLineAmountWithoutVat;
                    invoiceEntity.TotalAmountWithoutVAT = createModel.TotalAmountWithoutVat;
                    invoiceEntity.TotalVATAmount = createModel.TotalVatamount;
                    invoiceEntity.TotalAmountWithVAT = createModel.TotalAmountWithVat;
                    invoiceEntity.TotalAmountWithVATFrn = createModel.TotalAmountWithVatfrn;
                    invoiceEntity.TotalAmountWithVATInWords = createModel.TotalAmountWithVatinWords;
                    invoiceEntity.IsTotalAmountPos = createModel.IsTotalAmountPos;
                    invoiceEntity.IsTotalVATAmountPos = createModel.IsTotalVatamountPos;
                    invoiceEntity.IsTotalAmtWithoutVatPos = createModel.IsTotalAmtWithoutVatPos;
                    invoiceEntity.DiscountAmount = createModel.DiscountAmount;
                    invoiceEntity.IsDiscountAmtPos = createModel.IsDiscountAmtPos;
                    invoiceEntity.ProductCodes = createModel.ProductCodes;
                    invoiceEntity.PortalLink = createModel.PortalLink;
                    invoiceEntity.VatPercentage = createModel.VatPercentage;
                    invoiceEntity.AdjustmentType = InvoiceConstants.InvoiceAdjustmentType.HOA_DON_THAY_THE;
                    invoiceEntity.ConvertedStatus = false;
                    invoiceEntity.TrangThaiXemTraCuuHoaDon = false;
                    invoiceEntity.InvoiceCode = InvoiceConstants.PREFIX_INVOICE_CODE + (createModel.CreatedOnDate.HasValue ? createModel.CreatedOnDate.Value.ToString(DateTimeFormatConstants.YYMMDDHHMMSSFFFF) : DateTime.Now.ToString(DateTimeFormatConstants.YYMMDDHHMMSSFFFF));
                    #region Thông tin trường đặc thù theo ngành nghề
                    /// <summary>
                    /// Sử dụng từ ngày (hóa đơn điện, nước..v..v)
                    /// </summary>
                    invoiceEntity.FromUseDate = createModel.FromUseDate;
                    /// <summary>
                    /// Sử dụng đến ngày (hóa đơn điện, nước...v.v..)
                    /// </summary>
                    invoiceEntity.ToUseDate = createModel.ToUseDate;
                    /// <summary>
                    /// Thuế môi trường
                    /// </summary>
                    invoiceEntity.EnvironmentTax = createModel.EnvironmentTax;
                    /// <summary>
                    /// Phí môi trường
                    /// </summary>
                    invoiceEntity.EnvironmentFee = createModel.EnvironmentFee;
                    /// <summary>
                    /// Số đầu kỳ
                    /// </summary>
                    invoiceEntity.StartNumber = createModel.StartNumber;
                    /// <summary>
                    /// Số cuối kỳ
                    /// </summary>
                    invoiceEntity.EndNumber = createModel.StartNumber;
                    /// <summary>
                    /// Số sử dụng trong kỳ
                    /// </summary>
                    invoiceEntity.UseNumber = createModel.UseNumber;
                    /// <summary>
                    /// Tiền dịch vụ thoát nước
                    /// </summary>
                    //invoiceEntity.DrainageServicePrice = createModel.DrainageServicePrice;
                    /// <summary>
                    /// Tiền thuế GTGT của dịch vụ thoát nước
                    /// </summary>
                    //invoiceEntity.DrainageServiceTaxMoney = createModel.DrainageServiceTaxMoney;
                    #endregion
                    if (createModel.IsPublishInvoice.HasValue && createModel.IsPublishInvoice.Value)
                    {
                        var item = invoiceRepo.GetMany(x => x.InvoiceTemplateId == invoiceEntity.InvoiceTemplateId && x.InvoiceSeries == invoiceEntity.InvoiceSeries).OrderByDescending(x => x.InvoiceNumber).FirstOrDefault();

                        var invoiceNumber = InvoiceConstants.FIRST_INVOICE_NUMBER;

                        if (item != null && !string.IsNullOrWhiteSpace(item.InvoiceNumber))
                        {
                            long numInvoiceNumber = 0;
                            try
                            {
                                numInvoiceNumber = Convert.ToInt64(item.InvoiceNumber);
                            }
                            catch (Exception ex)
                            {
                                logger.Debug(ex);
                            }
                            invoiceNumber = (++numInvoiceNumber).ToString().PadLeft(7, '0');
                        }
                        else
                        {
                            invoiceNumber = "1".PadLeft(7, '0');
                        }

                        //invoiceEntity.InvoiceStatus = InvoiceConstants.InvoiceStatus.HOA_DON_DA_PHAT_HANH;
                        if (invoiceEntity.InvoiceNumber.Equals(InvoiceConstants.FIRST_INVOICE_NUMBER))
                        {
                            invoiceEntity.InvoiceNumber = invoiceNumber;
                        }
                        invoiceEntity.PublishBy = createModel.CreatedByUserId;
                    }
                    else
                    {
                        invoiceEntity.InvoiceStatus = InvoiceConstants.InvoiceStatus.HOA_DON_MOI_TAO_LAP;
                        invoiceEntity.InvoiceNumber = InvoiceConstants.FIRST_INVOICE_NUMBER;
                    }

                    invoiceEntity.CreatedByUserId = createModel.CreatedByUserId;
                    invoiceEntity.CreatedOnDate = createModel.CreatedOnDate;
                    try
                    {
                        invoiceEntity.TotalAmountWithVATInWords = Utils.DocTienBangChu((long)invoiceEntity.TotalAmountWithVAT, " đồng");
                    }
                    catch (Exception ex)
                    {
                        logger.Debug(ex);
                        invoiceEntity.TotalAmountWithVATInWords = "Không đồng";
                    }
                    invoiceRepo.Add(invoiceEntity);

                    if (createModel.ListProductInvoice != null && createModel.ListProductInvoice.Count > 0)
                    {
                        foreach (var itemPI in createModel.ListProductInvoice)
                        {
                            Product_Invoice pi = new Product_Invoice();
                            pi.ProductInvoiceId = Guid.NewGuid();
                            pi.InvoiceId = invoiceEntity.InvoiceId;
                            pi.LineNumber = itemPI.LineNumber;
                            pi.ItemCode = itemPI.ItemCode;
                            pi.ItemName = itemPI.ItemName;
                            pi.UnitCode = itemPI.UnitCode;
                            pi.UnitName = itemPI.UnitName;
                            pi.UnitPrice = itemPI.UnitPrice;
                            pi.Quantity = itemPI.Quantity;
                            pi.ItemTotalAmountWithoutVat = itemPI.ItemTotalAmountWithoutVat;
                            pi.VatPercentage = itemPI.VatPercentage;
                            pi.VatAmount = itemPI.VatAmount;
                            pi.Promotion = itemPI.Promotion;
                            pi.LotNumber = itemPI.LotNumber;
                            pi.ItemTotalAmountWithVat = itemPI.ItemTotalAmountWithVat;
                            pi.ExpirationDate = itemPI.ExpirationDate;
                            pi.AdjustmentVatAmount = itemPI.AdjustmentVatAmount;
                            pi.IsIncreaseItem = itemPI.IsIncreaseItem;
                            pi.ItemType = itemPI.ItemType;
                            productinvoiceRepo.Add(pi);
                        }
                    }

                    var result = unitOfWork.Save();
                    if (result >= 1)
                    {
                        var invoiceModel = InvoiceConvert.entityToModel(invoiceEntity);
                        invoiceModel.IsPublishInvoice = createModel.IsPublishInvoice;
                        return new Response<BasicInvoiceModel>(1, MessageResponseConstants.InvoiceMessage.MESSAGE_REPLACE_SUCCESS, invoiceModel);
                    }
                    else
                    {
                        return new Response<BasicInvoiceModel>(-1, MessageResponseConstants.InvoiceMessage.MESSAGE_REPLACE_FAILED_DB, null);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                return new Response<BasicInvoiceModel>(-1, MessageResponseConstants.MESSAGE_EXCEPTION + ex.Message, null);
            }
        }

        /// <summary>
        /// Điều chỉnh tăng hóa đơn
        /// </summary>
        /// <param name="createModel"></param>
        /// <returns></returns>
        public Response<BasicInvoiceModel> AdjustmentIncrease(CreateInvoiceModel createModel)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var invoiceRepo = unitOfWork.GetRepository<Invoice>();
                    var productinvoiceRepo = unitOfWork.GetRepository<Product_Invoice>();

                    #region Validation data before process

                    if (createModel == null)
                    {
                        return new Response<BasicInvoiceModel>(0, "Dữ liệu trống: model = null", null);
                    }
                    #endregion

                    #region Check require object before process

                    if (string.IsNullOrWhiteSpace(createModel.InvoiceSeries))
                    {
                        return new Response<BasicInvoiceModel>(0, "Ký hiệu hóa đơn trống", null);
                    }

                    if (!createModel.InvoiceIssuedDate.HasValue)
                    {
                        return new Response<BasicInvoiceModel>(0, "Ngày tạo hóa đơn trống", null);
                    }

                    if (!createModel.PaymentMethod.HasValue)
                    {
                        return new Response<BasicInvoiceModel>(0, "Hình thức thanh toán trống", null);
                    }

                    if ((createModel.ListProductInvoice == null) || (createModel.ListProductInvoice.Count() == 0))
                    {
                        return new Response<BasicInvoiceModel>(0, "Không tồn tại thông tin hàng hóa trong hóa đơn", null);
                    }

                    #endregion

                    if (createModel.OriginalInvoiceId.HasValue)
                    {
                        var orgInvoiceEntity = invoiceRepo.GetById(createModel.OriginalInvoiceId.Value);
                        if (orgInvoiceEntity == null)
                        {
                            return new Response<BasicInvoiceModel>(0, MessageResponseConstants.InvoiceMessage.MESSAGE_ORIGINAL_INVOICE_NOT_FOUND, null);
                        }
                        if (orgInvoiceEntity.InvoiceStatus.Equals(InvoiceConstants.InvoiceStatus.HOA_DON_MOI_TAO_LAP) || orgInvoiceEntity.InvoiceStatus.Equals(InvoiceConstants.InvoiceStatus.HOA_DON_BI_XOA_BO) || orgInvoiceEntity.InvoiceStatus.Equals(InvoiceConstants.InvoiceStatus.HOA_DON_BI_THAY_THE))
                        {
                            return new Response<BasicInvoiceModel>(0, MessageResponseConstants.InvoiceMessage.MESSAGE_INVOICE_ADJUSTMENT_FAILED, null);
                        }

                        if (createModel.IsPublishInvoice.HasValue && createModel.IsPublishInvoice.Value)
                        {
                            orgInvoiceEntity.InvoiceStatus = InvoiceConstants.InvoiceStatus.HOA_DON_BI_DIEU_CHINH;
                            orgInvoiceEntity.LastModifiedByUserId = createModel.CreatedByUserId;
                            orgInvoiceEntity.LastModifiedOnDate = DateTime.Now;
                            invoiceRepo.Update(orgInvoiceEntity);
                        }
                    }

                    Invoice invoiceEntity = new Invoice();
                    invoiceEntity.InvoiceId = Guid.NewGuid();
                    invoiceEntity.InvoiceCategoryId = createModel.InvoiceCategoryId;
                    invoiceEntity.InvoiceTemplateId = createModel.InvoiceTemplateId;
                    invoiceEntity.CompanyId = createModel.CompanyId;
                    invoiceEntity.CustomerId = createModel.CustomerId;
                    invoiceEntity.Signature = createModel.Signature;
                    invoiceEntity.Converted = createModel.Converted;
                    invoiceEntity.KindOfService = createModel.KindOfService;
                    invoiceEntity.PaymentStatus = createModel.PaymentStatus;
                    invoiceEntity.ArisingDate = createModel.ArisingDate;
                    invoiceEntity.ProcessInvoiceNote = createModel.ProcessInvoiceNote;
                    invoiceEntity.GrossValue = createModel.GrossValue;
                    invoiceEntity.VatAmount0 = createModel.VatAmount0;
                    invoiceEntity.GrossValue0 = createModel.GrossValue0;
                    invoiceEntity.VatAmount5 = createModel.VatAmount5;
                    invoiceEntity.GrossValue5 = createModel.GrossValue5;
                    invoiceEntity.VatAmount10 = createModel.VatAmount10;
                    invoiceEntity.GrossValue10 = createModel.GrossValue10;
                    invoiceEntity.Certified = createModel.Certified;
                    invoiceEntity.CertifiedId = createModel.CertifiedId;
                    invoiceEntity.CertifiedData = createModel.CertifiedData;
                    invoiceEntity.CreationDate = createModel.CreationDate;
                    invoiceEntity.CreationDate = createModel.CreationDate;
                    invoiceEntity.PublishDate = null;
                    invoiceEntity.CreationBy = createModel.CreationBy;
                    invoiceEntity.PublishBy = createModel.PublishBy;
                    invoiceEntity.PaymentMethod = createModel.PaymentMethod;
                    invoiceEntity.SellerAppRecordId = createModel.SellerAppRecordId;
                    invoiceEntity.InvoiceAppRecordId = createModel.InvoiceAppRecordId;
                    invoiceEntity.InvoiceType = createModel.InvoiceType;
                    invoiceEntity.TemplateCode = createModel.TemplateCode;
                    invoiceEntity.InvoiceSeries = createModel.InvoiceSeries;
                    invoiceEntity.InvoiceName = createModel.InvoiceName;
                    invoiceEntity.InvoiceIssuedDate = createModel.InvoiceIssuedDate != null ? new DateTime(createModel.InvoiceIssuedDate.Value.Year, createModel.InvoiceIssuedDate.Value.Month, createModel.InvoiceIssuedDate.Value.Day) : DateTime.Today;
                    invoiceEntity.SignedDate = null;
                    invoiceEntity.SubmittedDate = createModel.SubmittedDate;
                    invoiceEntity.ContractNumber = createModel.ContractNumber;
                    invoiceEntity.ContractDate = createModel.ContractDate;
                    invoiceEntity.CurrencyCode = createModel.CurrencyCode;
                    invoiceEntity.ExchangeRate = createModel.ExchangeRate;
                    invoiceEntity.InvoiceNote = createModel.InvoiceNote;
                    invoiceEntity.OriginalInvoiceId = createModel.OriginalInvoiceId;
                    invoiceEntity.AdditionalReferenceDesc = createModel.AdditionalReferenceDesc;
                    invoiceEntity.AdditionalReferenceDate = createModel.AdditionalReferenceDate;
                    invoiceEntity.SellerLegalName = createModel.SellerLegalName;
                    invoiceEntity.SellerTaxCode = createModel.SellerTaxCode;
                    invoiceEntity.SellerAddressLine = createModel.SellerAddressLine;
                    invoiceEntity.SellerPostalCode = createModel.SellerPostalCode;
                    invoiceEntity.SellerDistrictName = createModel.SellerDistrictName;
                    invoiceEntity.SellerCityName = createModel.SellerCityName;
                    invoiceEntity.SellerCountryCode = createModel.SellerCountryCode;
                    invoiceEntity.SellerPhoneNumber = createModel.SellerPhoneNumber;
                    invoiceEntity.SellerFaxNumber = createModel.SellerFaxNumber;
                    invoiceEntity.SellerEmail = createModel.SellerEmail;
                    invoiceEntity.SellerBankName = createModel.SellerBankName;
                    invoiceEntity.SellerBankAccount = createModel.SellerBankAccount;
                    invoiceEntity.SellerContactPersonName = createModel.SellerContactPersonName;
                    invoiceEntity.SellerSignedPersonName = createModel.SellerSignedPersonName;
                    invoiceEntity.SellerSubmittedPersonName = createModel.SellerSubmittedPersonName;
                    invoiceEntity.BuyerDisplayName = createModel.BuyerDisplayName;
                    invoiceEntity.BuyerLegalName = createModel.BuyerLegalName;
                    invoiceEntity.BuyerTaxCode = createModel.BuyerTaxCode;
                    invoiceEntity.BuyerAddressLine = createModel.BuyerAddressLine;
                    invoiceEntity.BuyerPostalCode = createModel.BuyerPostalCode;
                    invoiceEntity.BuyerDistrictName = createModel.BuyerDistrictName;
                    invoiceEntity.BuyerCityName = createModel.BuyerCityName;
                    invoiceEntity.BuyerCountryCode = createModel.BuyerCountryCode;
                    invoiceEntity.BuyerPhoneNumber = createModel.BuyerPhoneNumber;
                    invoiceEntity.BuyerFaxNumber = createModel.BuyerFaxNumber;
                    invoiceEntity.BuyerEmail = createModel.BuyerEmail;
                    invoiceEntity.BuyerBankName = createModel.BuyerBankName;
                    invoiceEntity.BuyerBankAccount = createModel.BuyerBankAccount;
                    invoiceEntity.SumOfTotalLineAmountWithoutVAT = createModel.SumOfTotalLineAmountWithoutVat;
                    invoiceEntity.TotalAmountWithoutVAT = createModel.TotalAmountWithoutVat;
                    invoiceEntity.TotalVATAmount = createModel.TotalVatamount;
                    invoiceEntity.TotalAmountWithVAT = createModel.TotalAmountWithVat;
                    invoiceEntity.TotalAmountWithVATFrn = createModel.TotalAmountWithVatfrn;
                    invoiceEntity.TotalAmountWithVATInWords = createModel.TotalAmountWithVatinWords;
                    invoiceEntity.IsTotalAmountPos = createModel.IsTotalAmountPos;
                    invoiceEntity.IsTotalVATAmountPos = createModel.IsTotalVatamountPos;
                    invoiceEntity.IsTotalAmtWithoutVatPos = createModel.IsTotalAmtWithoutVatPos;
                    invoiceEntity.DiscountAmount = createModel.DiscountAmount;
                    invoiceEntity.IsDiscountAmtPos = createModel.IsDiscountAmtPos;
                    invoiceEntity.ProductCodes = createModel.ProductCodes;
                    invoiceEntity.PortalLink = createModel.PortalLink;
                    invoiceEntity.VatPercentage = createModel.VatPercentage;
                    invoiceEntity.AdjustmentType = InvoiceConstants.InvoiceAdjustmentType.HOA_DON_DIEU_CHINH_TANG;
                    invoiceEntity.ConvertedStatus = false;
                    invoiceEntity.TrangThaiXemTraCuuHoaDon = false;
                    invoiceEntity.InvoiceCode = InvoiceConstants.PREFIX_INVOICE_CODE + (createModel.CreatedOnDate.HasValue ? createModel.CreatedOnDate.Value.ToString(DateTimeFormatConstants.YYMMDDHHMMSSFFFF) : DateTime.Now.ToString(DateTimeFormatConstants.YYMMDDHHMMSSFFFF));
                    #region Thông tin trường đặc thù theo ngành nghề
                    /// <summary>
                    /// Sử dụng từ ngày (hóa đơn điện, nước..v..v)
                    /// </summary>
                    invoiceEntity.FromUseDate = createModel.FromUseDate;
                    /// <summary>
                    /// Sử dụng đến ngày (hóa đơn điện, nước...v.v..)
                    /// </summary>
                    invoiceEntity.ToUseDate = createModel.ToUseDate;
                    /// <summary>
                    /// Thuế môi trường
                    /// </summary>
                    invoiceEntity.EnvironmentTax = createModel.EnvironmentTax;
                    /// <summary>
                    /// Phí môi trường
                    /// </summary>
                    invoiceEntity.EnvironmentFee = createModel.EnvironmentFee;
                    /// <summary>
                    /// Số đầu kỳ
                    /// </summary>
                    invoiceEntity.StartNumber = createModel.StartNumber;
                    /// <summary>
                    /// Số cuối kỳ
                    /// </summary>
                    invoiceEntity.EndNumber = createModel.StartNumber;
                    /// <summary>
                    /// Số sử dụng trong kỳ
                    /// </summary>
                    invoiceEntity.UseNumber = createModel.UseNumber;
                    /// <summary>
                    /// Tiền dịch vụ thoát nước
                    /// </summary>
                    //invoiceEntity.DrainageServicePrice = createModel.DrainageServicePrice;
                    /// <summary>
                    /// Tiền thuế GTGT của dịch vụ thoát nước
                    /// </summary>
                    //invoiceEntity.DrainageServiceTaxMoney = createModel.DrainageServiceTaxMoney;
                    #endregion
                    if (createModel.IsPublishInvoice.HasValue && createModel.IsPublishInvoice.Value)
                    {
                        var item = invoiceRepo.GetMany(x => x.InvoiceTemplateId == invoiceEntity.InvoiceTemplateId && x.InvoiceSeries == invoiceEntity.InvoiceSeries).OrderByDescending(x => x.InvoiceNumber).FirstOrDefault();

                        var invoiceNumber = InvoiceConstants.FIRST_INVOICE_NUMBER;

                        if (item != null && !string.IsNullOrWhiteSpace(item.InvoiceNumber))
                        {
                            long numInvoiceNumber = 0;
                            try
                            {
                                numInvoiceNumber = Convert.ToInt64(item.InvoiceNumber);
                            }
                            catch (Exception ex)
                            {
                                logger.Debug(ex);
                            }
                            invoiceNumber = (++numInvoiceNumber).ToString().PadLeft(7, '0');
                        }
                        else
                        {
                            invoiceNumber = "1".PadLeft(7, '0');
                        }

                        //invoiceEntity.InvoiceStatus = InvoiceConstants.InvoiceStatus.HOA_DON_DA_PHAT_HANH;
                        if (invoiceEntity.InvoiceNumber.Equals(InvoiceConstants.FIRST_INVOICE_NUMBER))
                        {
                            invoiceEntity.InvoiceNumber = invoiceNumber;
                        }
                        invoiceEntity.PublishBy = createModel.CreatedByUserId;
                    }
                    else
                    {
                        invoiceEntity.InvoiceStatus = InvoiceConstants.InvoiceStatus.HOA_DON_MOI_TAO_LAP;
                        invoiceEntity.InvoiceNumber = InvoiceConstants.FIRST_INVOICE_NUMBER;
                    }
                    // invoiceEntity.InvoiceStatus = (createModel.InvoiceStatus != null) ? createModel.InvoiceStatus : InvoiceConstants.InvoiceStatus.HOA_DON_MOI_TAO_LAP;
                    invoiceEntity.CreatedByUserId = createModel.CreatedByUserId;
                    invoiceEntity.CreatedOnDate = createModel.CreatedOnDate;
                    try
                    {
                        invoiceEntity.TotalAmountWithVATInWords = Utils.DocTienBangChu((long)invoiceEntity.TotalAmountWithVAT, " đồng");
                    }
                    catch (Exception ex)
                    {
                        logger.Debug(ex);
                        invoiceEntity.TotalAmountWithVATInWords = "Không đồng";
                    }
                    invoiceRepo.Add(invoiceEntity);

                    if (createModel.ListProductInvoice != null && createModel.ListProductInvoice.Count > 0)
                    {
                        foreach (var itemPI in createModel.ListProductInvoice)
                        {
                            Product_Invoice pi = new Product_Invoice();
                            pi.ProductInvoiceId = Guid.NewGuid();
                            pi.InvoiceId = invoiceEntity.InvoiceId;
                            pi.LineNumber = itemPI.LineNumber;
                            pi.ItemCode = itemPI.ItemCode;
                            pi.ItemName = itemPI.ItemName;
                            pi.UnitCode = itemPI.UnitCode;
                            pi.UnitName = itemPI.UnitName;
                            pi.UnitPrice = itemPI.UnitPrice;
                            pi.Quantity = itemPI.Quantity;
                            pi.ItemTotalAmountWithoutVat = itemPI.ItemTotalAmountWithoutVat;
                            pi.VatPercentage = itemPI.VatPercentage;
                            pi.VatAmount = itemPI.VatAmount;
                            pi.Promotion = itemPI.Promotion;
                            pi.LotNumber = itemPI.LotNumber;
                            pi.ItemTotalAmountWithVat = itemPI.ItemTotalAmountWithVat;
                            pi.ExpirationDate = itemPI.ExpirationDate;
                            pi.AdjustmentVatAmount = itemPI.AdjustmentVatAmount;
                            pi.IsIncreaseItem = itemPI.IsIncreaseItem;
                            pi.ItemType = itemPI.ItemType;
                            productinvoiceRepo.Add(pi);
                        }
                    }

                    var result = unitOfWork.Save();
                    if (result >= 1)
                    {
                        var invoiceModel = InvoiceConvert.entityToModel(invoiceEntity);
                        invoiceModel.IsPublishInvoice = createModel.IsPublishInvoice;
                        return new Response<BasicInvoiceModel>(1, MessageResponseConstants.InvoiceMessage.MESSAGE_ADJUSTMENT_SUCCESS, invoiceModel);
                    }
                    else
                    {
                        return new Response<BasicInvoiceModel>(-1, MessageResponseConstants.InvoiceMessage.MESSAGE_ADJUSTMENT_FAILED_DB, null);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                return new Response<BasicInvoiceModel>(-1, MessageResponseConstants.MESSAGE_EXCEPTION + ex.Message, null);
            }
        }

        /// <summary>
        /// Điều chỉnh giảm hóa đơn
        /// </summary>
        /// <param name="createModel"></param>
        /// <returns></returns>
        public Response<BasicInvoiceModel> AdjustmentDecrease(CreateInvoiceModel createModel)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var invoiceRepo = unitOfWork.GetRepository<Invoice>();
                    var productinvoiceRepo = unitOfWork.GetRepository<Product_Invoice>();

                    #region Validation data before process

                    if (createModel == null)
                    {
                        return new Response<BasicInvoiceModel>(0, "Dữ liệu trống: model = null", null);
                    }
                    #endregion

                    #region Check require object before process

                    if (string.IsNullOrWhiteSpace(createModel.InvoiceSeries))
                    {
                        return new Response<BasicInvoiceModel>(0, "Ký hiệu hóa đơn trống", null);
                    }

                    if (!createModel.InvoiceIssuedDate.HasValue)
                    {
                        return new Response<BasicInvoiceModel>(0, "Ngày tạo hóa đơn trống", null);
                    }

                    if (!createModel.PaymentMethod.HasValue)
                    {
                        return new Response<BasicInvoiceModel>(0, "Hình thức thanh toán trống", null);
                    }

                    if ((createModel.ListProductInvoice == null) || (createModel.ListProductInvoice.Count() == 0))
                    {
                        return new Response<BasicInvoiceModel>(0, "Không tồn tại thông tin hàng hóa trong hóa đơn", null);
                    }

                    #endregion

                    if (createModel.OriginalInvoiceId.HasValue)
                    {
                        var orgInvoiceEntity = invoiceRepo.GetById(createModel.OriginalInvoiceId.Value);
                        if (orgInvoiceEntity == null)
                        {
                            return new Response<BasicInvoiceModel>(0, MessageResponseConstants.InvoiceMessage.MESSAGE_ORIGINAL_INVOICE_NOT_FOUND, null);
                        }
                        if (orgInvoiceEntity.InvoiceStatus.Equals(InvoiceConstants.InvoiceStatus.HOA_DON_MOI_TAO_LAP) || orgInvoiceEntity.InvoiceStatus.Equals(InvoiceConstants.InvoiceStatus.HOA_DON_BI_XOA_BO) || orgInvoiceEntity.InvoiceStatus.Equals(InvoiceConstants.InvoiceStatus.HOA_DON_BI_THAY_THE))
                        {
                            return new Response<BasicInvoiceModel>(0, MessageResponseConstants.InvoiceMessage.MESSAGE_INVOICE_ADJUSTMENT_FAILED, null);
                        }

                        if (createModel.IsPublishInvoice.HasValue && createModel.IsPublishInvoice.Value)
                        {
                            orgInvoiceEntity.InvoiceStatus = InvoiceConstants.InvoiceStatus.HOA_DON_BI_DIEU_CHINH;
                            orgInvoiceEntity.LastModifiedByUserId = createModel.CreatedByUserId;
                            orgInvoiceEntity.LastModifiedOnDate = DateTime.Now;
                            invoiceRepo.Update(orgInvoiceEntity);
                        }
                    }

                    Invoice invoiceEntity = new Invoice();
                    invoiceEntity.InvoiceId = Guid.NewGuid();
                    invoiceEntity.InvoiceCategoryId = createModel.InvoiceCategoryId;
                    invoiceEntity.InvoiceTemplateId = createModel.InvoiceTemplateId;
                    invoiceEntity.CompanyId = createModel.CompanyId;
                    invoiceEntity.CustomerId = createModel.CustomerId;
                    invoiceEntity.Signature = createModel.Signature;
                    invoiceEntity.Converted = createModel.Converted;
                    invoiceEntity.KindOfService = createModel.KindOfService;
                    invoiceEntity.PaymentStatus = createModel.PaymentStatus;
                    invoiceEntity.ArisingDate = createModel.ArisingDate;
                    invoiceEntity.ProcessInvoiceNote = createModel.ProcessInvoiceNote;
                    invoiceEntity.GrossValue = createModel.GrossValue;
                    invoiceEntity.VatAmount0 = createModel.VatAmount0;
                    invoiceEntity.GrossValue0 = createModel.GrossValue0;
                    invoiceEntity.VatAmount5 = createModel.VatAmount5;
                    invoiceEntity.GrossValue5 = createModel.GrossValue5;
                    invoiceEntity.VatAmount10 = createModel.VatAmount10;
                    invoiceEntity.GrossValue10 = createModel.GrossValue10;
                    invoiceEntity.Certified = createModel.Certified;
                    invoiceEntity.CertifiedId = createModel.CertifiedId;
                    invoiceEntity.CertifiedData = createModel.CertifiedData;
                    invoiceEntity.CreationDate = createModel.CreationDate;
                    invoiceEntity.CreationDate = createModel.CreationDate;
                    invoiceEntity.PublishDate = null;
                    invoiceEntity.CreationBy = createModel.CreationBy;
                    invoiceEntity.PublishBy = createModel.PublishBy;
                    invoiceEntity.PaymentMethod = createModel.PaymentMethod;
                    invoiceEntity.SellerAppRecordId = createModel.SellerAppRecordId;
                    invoiceEntity.InvoiceAppRecordId = createModel.InvoiceAppRecordId;
                    invoiceEntity.InvoiceType = createModel.InvoiceType;
                    invoiceEntity.TemplateCode = createModel.TemplateCode;
                    invoiceEntity.InvoiceSeries = createModel.InvoiceSeries;
                    invoiceEntity.InvoiceName = createModel.InvoiceName;
                    invoiceEntity.InvoiceIssuedDate = createModel.InvoiceIssuedDate != null ? new DateTime(createModel.InvoiceIssuedDate.Value.Year, createModel.InvoiceIssuedDate.Value.Month, createModel.InvoiceIssuedDate.Value.Day) : DateTime.Today;
                    invoiceEntity.SignedDate = null;
                    invoiceEntity.SubmittedDate = createModel.SubmittedDate;
                    invoiceEntity.ContractNumber = createModel.ContractNumber;
                    invoiceEntity.ContractDate = createModel.ContractDate;
                    invoiceEntity.CurrencyCode = createModel.CurrencyCode;
                    invoiceEntity.ExchangeRate = createModel.ExchangeRate;
                    invoiceEntity.InvoiceNote = createModel.InvoiceNote;
                    invoiceEntity.OriginalInvoiceId = createModel.OriginalInvoiceId;
                    invoiceEntity.AdditionalReferenceDesc = createModel.AdditionalReferenceDesc;
                    invoiceEntity.AdditionalReferenceDate = createModel.AdditionalReferenceDate;
                    invoiceEntity.SellerLegalName = createModel.SellerLegalName;
                    invoiceEntity.SellerTaxCode = createModel.SellerTaxCode;
                    invoiceEntity.SellerAddressLine = createModel.SellerAddressLine;
                    invoiceEntity.SellerPostalCode = createModel.SellerPostalCode;
                    invoiceEntity.SellerDistrictName = createModel.SellerDistrictName;
                    invoiceEntity.SellerCityName = createModel.SellerCityName;
                    invoiceEntity.SellerCountryCode = createModel.SellerCountryCode;
                    invoiceEntity.SellerPhoneNumber = createModel.SellerPhoneNumber;
                    invoiceEntity.SellerFaxNumber = createModel.SellerFaxNumber;
                    invoiceEntity.SellerEmail = createModel.SellerEmail;
                    invoiceEntity.SellerBankName = createModel.SellerBankName;
                    invoiceEntity.SellerBankAccount = createModel.SellerBankAccount;
                    invoiceEntity.SellerContactPersonName = createModel.SellerContactPersonName;
                    invoiceEntity.SellerSignedPersonName = createModel.SellerSignedPersonName;
                    invoiceEntity.SellerSubmittedPersonName = createModel.SellerSubmittedPersonName;
                    invoiceEntity.BuyerDisplayName = createModel.BuyerDisplayName;
                    invoiceEntity.BuyerLegalName = createModel.BuyerLegalName;
                    invoiceEntity.BuyerTaxCode = createModel.BuyerTaxCode;
                    invoiceEntity.BuyerAddressLine = createModel.BuyerAddressLine;
                    invoiceEntity.BuyerPostalCode = createModel.BuyerPostalCode;
                    invoiceEntity.BuyerDistrictName = createModel.BuyerDistrictName;
                    invoiceEntity.BuyerCityName = createModel.BuyerCityName;
                    invoiceEntity.BuyerCountryCode = createModel.BuyerCountryCode;
                    invoiceEntity.BuyerPhoneNumber = createModel.BuyerPhoneNumber;
                    invoiceEntity.BuyerFaxNumber = createModel.BuyerFaxNumber;
                    invoiceEntity.BuyerEmail = createModel.BuyerEmail;
                    invoiceEntity.BuyerBankName = createModel.BuyerBankName;
                    invoiceEntity.BuyerBankAccount = createModel.BuyerBankAccount;
                    invoiceEntity.SumOfTotalLineAmountWithoutVAT = createModel.SumOfTotalLineAmountWithoutVat;
                    invoiceEntity.TotalAmountWithoutVAT = createModel.TotalAmountWithoutVat;
                    invoiceEntity.TotalVATAmount = createModel.TotalVatamount;
                    invoiceEntity.TotalAmountWithVAT = createModel.TotalAmountWithVat;
                    invoiceEntity.TotalAmountWithVATFrn = createModel.TotalAmountWithVatfrn;
                    invoiceEntity.TotalAmountWithVATInWords = createModel.TotalAmountWithVatinWords;
                    invoiceEntity.IsTotalAmountPos = createModel.IsTotalAmountPos;
                    invoiceEntity.IsTotalVATAmountPos = createModel.IsTotalVatamountPos;
                    invoiceEntity.IsTotalAmtWithoutVatPos = createModel.IsTotalAmtWithoutVatPos;
                    invoiceEntity.DiscountAmount = createModel.DiscountAmount;
                    invoiceEntity.IsDiscountAmtPos = createModel.IsDiscountAmtPos;
                    invoiceEntity.ProductCodes = createModel.ProductCodes;
                    invoiceEntity.PortalLink = createModel.PortalLink;
                    invoiceEntity.VatPercentage = createModel.VatPercentage;
                    invoiceEntity.AdjustmentType = InvoiceConstants.InvoiceAdjustmentType.HOA_DON_DIEU_CHINH_GIAM;
                    invoiceEntity.ConvertedStatus = false;
                    invoiceEntity.TrangThaiXemTraCuuHoaDon = false;
                    invoiceEntity.InvoiceCode = InvoiceConstants.PREFIX_INVOICE_CODE + (createModel.CreatedOnDate.HasValue ? createModel.CreatedOnDate.Value.ToString(DateTimeFormatConstants.YYMMDDHHMMSSFFFF) : DateTime.Now.ToString(DateTimeFormatConstants.YYMMDDHHMMSSFFFF));
                    #region Thông tin trường đặc thù theo ngành nghề
                    /// <summary>
                    /// Sử dụng từ ngày (hóa đơn điện, nước..v..v)
                    /// </summary>
                    invoiceEntity.FromUseDate = createModel.FromUseDate;
                    /// <summary>
                    /// Sử dụng đến ngày (hóa đơn điện, nước...v.v..)
                    /// </summary>
                    invoiceEntity.ToUseDate = createModel.ToUseDate;
                    /// <summary>
                    /// Thuế môi trường
                    /// </summary>
                    invoiceEntity.EnvironmentTax = createModel.EnvironmentTax;
                    /// <summary>
                    /// Phí môi trường
                    /// </summary>
                    invoiceEntity.EnvironmentFee = createModel.EnvironmentFee;
                    /// <summary>
                    /// Số đầu kỳ
                    /// </summary>
                    invoiceEntity.StartNumber = createModel.StartNumber;
                    /// <summary>
                    /// Số cuối kỳ
                    /// </summary>
                    invoiceEntity.EndNumber = createModel.StartNumber;
                    /// <summary>
                    /// Số sử dụng trong kỳ
                    /// </summary>
                    invoiceEntity.UseNumber = createModel.UseNumber;
                    /// <summary>
                    /// Tiền dịch vụ thoát nước
                    /// </summary>
                    //invoiceEntity.DrainageServicePrice = createModel.DrainageServicePrice;
                    /// <summary>
                    /// Tiền thuế GTGT của dịch vụ thoát nước
                    /// </summary>
                    //invoiceEntity.DrainageServiceTaxMoney = createModel.DrainageServiceTaxMoney;
                    #endregion
                    if (createModel.IsPublishInvoice.HasValue && createModel.IsPublishInvoice.Value)
                    {
                        var item = invoiceRepo.GetMany(x => x.InvoiceTemplateId == invoiceEntity.InvoiceTemplateId && x.InvoiceSeries == invoiceEntity.InvoiceSeries).OrderByDescending(x => x.InvoiceNumber).FirstOrDefault();

                        var invoiceNumber = InvoiceConstants.FIRST_INVOICE_NUMBER;

                        if (item != null && !string.IsNullOrWhiteSpace(item.InvoiceNumber))
                        {
                            long numInvoiceNumber = 0;
                            try
                            {
                                numInvoiceNumber = Convert.ToInt64(item.InvoiceNumber);
                            }
                            catch (Exception ex)
                            {
                                logger.Debug(ex);
                            }
                            invoiceNumber = (++numInvoiceNumber).ToString().PadLeft(7, '0');
                        }
                        else
                        {
                            invoiceNumber = "1".PadLeft(7, '0');
                        }

                        // invoiceEntity.InvoiceStatus = InvoiceConstants.InvoiceStatus.HOA_DON_DA_PHAT_HANH;
                        if (invoiceEntity.InvoiceNumber.Equals(InvoiceConstants.FIRST_INVOICE_NUMBER))
                        {
                            invoiceEntity.InvoiceNumber = invoiceNumber;
                        }
                        invoiceEntity.PublishBy = createModel.CreatedByUserId;
                    }
                    else
                    {
                        invoiceEntity.InvoiceStatus = InvoiceConstants.InvoiceStatus.HOA_DON_MOI_TAO_LAP;
                        invoiceEntity.InvoiceNumber = InvoiceConstants.FIRST_INVOICE_NUMBER;
                    }
                    // invoiceEntity.InvoiceStatus = (createModel.InvoiceStatus != null) ? createModel.InvoiceStatus : InvoiceConstants.InvoiceStatus.HOA_DON_MOI_TAO_LAP;
                    invoiceEntity.CreatedByUserId = createModel.CreatedByUserId;
                    invoiceEntity.CreatedOnDate = createModel.CreatedOnDate;
                    try
                    {
                        invoiceEntity.TotalAmountWithVATInWords = Utils.DocTienBangChu((long)invoiceEntity.TotalAmountWithVAT, " đồng");
                    }
                    catch (Exception ex)
                    {
                        logger.Debug(ex);
                        invoiceEntity.TotalAmountWithVATInWords = "Không đồng";
                    }
                    invoiceRepo.Add(invoiceEntity);

                    if (createModel.ListProductInvoice != null && createModel.ListProductInvoice.Count > 0)
                    {
                        foreach (var itemPI in createModel.ListProductInvoice)
                        {
                            Product_Invoice pi = new Product_Invoice();
                            pi.ProductInvoiceId = Guid.NewGuid();
                            pi.InvoiceId = invoiceEntity.InvoiceId;
                            pi.LineNumber = itemPI.LineNumber;
                            pi.ItemCode = itemPI.ItemCode;
                            pi.ItemName = itemPI.ItemName;
                            pi.UnitCode = itemPI.UnitCode;
                            pi.UnitName = itemPI.UnitName;
                            pi.UnitPrice = itemPI.UnitPrice;
                            pi.Quantity = itemPI.Quantity;
                            pi.ItemTotalAmountWithoutVat = itemPI.ItemTotalAmountWithoutVat;
                            pi.VatPercentage = itemPI.VatPercentage;
                            pi.VatAmount = itemPI.VatAmount;
                            pi.Promotion = itemPI.Promotion;
                            pi.LotNumber = itemPI.LotNumber;
                            pi.ItemTotalAmountWithVat = itemPI.ItemTotalAmountWithVat;
                            pi.ExpirationDate = itemPI.ExpirationDate;
                            pi.AdjustmentVatAmount = itemPI.AdjustmentVatAmount;
                            pi.IsIncreaseItem = itemPI.IsIncreaseItem;
                            pi.ItemType = itemPI.ItemType;
                            productinvoiceRepo.Add(pi);
                        }
                    }

                    var result = unitOfWork.Save();
                    if (result >= 1)
                    {
                        var invoiceModel = InvoiceConvert.entityToModel(invoiceEntity);
                        invoiceModel.IsPublishInvoice = createModel.IsPublishInvoice;
                        return new Response<BasicInvoiceModel>(1, MessageResponseConstants.InvoiceMessage.MESSAGE_ADJUSTMENT_SUCCESS, invoiceModel);
                    }
                    else
                    {
                        return new Response<BasicInvoiceModel>(-1, MessageResponseConstants.InvoiceMessage.MESSAGE_ADJUSTMENT_FAILED_DB, null);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                return new Response<BasicInvoiceModel>(-1, MessageResponseConstants.MESSAGE_EXCEPTION + ex.Message, null);
            }
        }

        /// <summary>
        /// Điều chỉnh thông tin hóa đơn
        /// </summary>
        /// <param name="createModel"></param>
        /// <returns></returns>
        public Response<BasicInvoiceModel> AdjustmentInfo(CreateInvoiceModel createModel)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var invoiceRepo = unitOfWork.GetRepository<Invoice>();
                    var productinvoiceRepo = unitOfWork.GetRepository<Product_Invoice>();

                    #region Validation data before process

                    if (createModel == null)
                    {
                        return new Response<BasicInvoiceModel>(0, "Dữ liệu trống: model = null", null);
                    }
                    #endregion

                    #region Check require object before process

                    if (string.IsNullOrWhiteSpace(createModel.InvoiceSeries))
                    {
                        return new Response<BasicInvoiceModel>(0, "Ký hiệu hóa đơn trống", null);
                    }

                    if (!createModel.InvoiceIssuedDate.HasValue)
                    {
                        return new Response<BasicInvoiceModel>(0, "Ngày tạo hóa đơn trống", null);
                    }

                    if (!createModel.PaymentMethod.HasValue)
                    {
                        return new Response<BasicInvoiceModel>(0, "Hình thức thanh toán trống", null);
                    }

                    if ((createModel.ListProductInvoice == null) || (createModel.ListProductInvoice.Count() == 0))
                    {
                        return new Response<BasicInvoiceModel>(0, "Không tồn tại thông tin hàng hóa trong hóa đơn", null);
                    }

                    #endregion

                    if (createModel.OriginalInvoiceId.HasValue)
                    {
                        var orgInvoiceEntity = invoiceRepo.GetById(createModel.OriginalInvoiceId.Value);
                        if (orgInvoiceEntity == null)
                        {
                            return new Response<BasicInvoiceModel>(0, MessageResponseConstants.InvoiceMessage.MESSAGE_ORIGINAL_INVOICE_NOT_FOUND, null);
                        }
                        if (orgInvoiceEntity.InvoiceStatus.Equals(InvoiceConstants.InvoiceStatus.HOA_DON_MOI_TAO_LAP) || orgInvoiceEntity.InvoiceStatus.Equals(InvoiceConstants.InvoiceStatus.HOA_DON_BI_XOA_BO) || orgInvoiceEntity.InvoiceStatus.Equals(InvoiceConstants.InvoiceStatus.HOA_DON_BI_THAY_THE))
                        {
                            return new Response<BasicInvoiceModel>(0, MessageResponseConstants.InvoiceMessage.MESSAGE_INVOICE_ADJUSTMENT_FAILED, null);
                        }

                        if (createModel.IsPublishInvoice.HasValue && createModel.IsPublishInvoice.Value)
                        {
                            orgInvoiceEntity.InvoiceStatus = InvoiceConstants.InvoiceStatus.HOA_DON_BI_DIEU_CHINH;
                            orgInvoiceEntity.LastModifiedByUserId = createModel.CreatedByUserId;
                            orgInvoiceEntity.LastModifiedOnDate = DateTime.Now;
                            invoiceRepo.Update(orgInvoiceEntity);
                        }
                    }

                    Invoice invoiceEntity = new Invoice();
                    invoiceEntity.InvoiceId = Guid.NewGuid();
                    invoiceEntity.InvoiceCategoryId = createModel.InvoiceCategoryId;
                    invoiceEntity.InvoiceTemplateId = createModel.InvoiceTemplateId;
                    invoiceEntity.CompanyId = createModel.CompanyId;
                    invoiceEntity.CustomerId = createModel.CustomerId;
                    invoiceEntity.Signature = createModel.Signature;
                    invoiceEntity.Converted = createModel.Converted;
                    invoiceEntity.KindOfService = createModel.KindOfService;
                    invoiceEntity.PaymentStatus = createModel.PaymentStatus;
                    invoiceEntity.ArisingDate = createModel.ArisingDate;
                    invoiceEntity.ProcessInvoiceNote = createModel.ProcessInvoiceNote;
                    invoiceEntity.GrossValue = createModel.GrossValue;
                    invoiceEntity.VatAmount0 = createModel.VatAmount0;
                    invoiceEntity.GrossValue0 = createModel.GrossValue0;
                    invoiceEntity.VatAmount5 = createModel.VatAmount5;
                    invoiceEntity.GrossValue5 = createModel.GrossValue5;
                    invoiceEntity.VatAmount10 = createModel.VatAmount10;
                    invoiceEntity.GrossValue10 = createModel.GrossValue10;
                    invoiceEntity.Certified = createModel.Certified;
                    invoiceEntity.CertifiedId = createModel.CertifiedId;
                    invoiceEntity.CertifiedData = createModel.CertifiedData;
                    invoiceEntity.CreationDate = createModel.CreationDate;
                    invoiceEntity.CreationDate = createModel.CreationDate;
                    invoiceEntity.PublishDate = null;
                    invoiceEntity.CreationBy = createModel.CreationBy;
                    invoiceEntity.PublishBy = createModel.PublishBy;
                    invoiceEntity.PaymentMethod = createModel.PaymentMethod;
                    invoiceEntity.SellerAppRecordId = createModel.SellerAppRecordId;
                    invoiceEntity.InvoiceAppRecordId = createModel.InvoiceAppRecordId;
                    invoiceEntity.InvoiceType = createModel.InvoiceType;
                    invoiceEntity.TemplateCode = createModel.TemplateCode;
                    invoiceEntity.InvoiceSeries = createModel.InvoiceSeries;
                    invoiceEntity.InvoiceName = createModel.InvoiceName;
                    invoiceEntity.InvoiceIssuedDate = createModel.InvoiceIssuedDate != null ? new DateTime(createModel.InvoiceIssuedDate.Value.Year, createModel.InvoiceIssuedDate.Value.Month, createModel.InvoiceIssuedDate.Value.Day) : DateTime.Today;
                    invoiceEntity.SignedDate = null;
                    invoiceEntity.SubmittedDate = createModel.SubmittedDate;
                    invoiceEntity.ContractNumber = createModel.ContractNumber;
                    invoiceEntity.ContractDate = createModel.ContractDate;
                    invoiceEntity.CurrencyCode = createModel.CurrencyCode;
                    invoiceEntity.ExchangeRate = createModel.ExchangeRate;
                    invoiceEntity.InvoiceNote = createModel.InvoiceNote;
                    invoiceEntity.OriginalInvoiceId = createModel.OriginalInvoiceId;
                    invoiceEntity.AdditionalReferenceDesc = createModel.AdditionalReferenceDesc;
                    invoiceEntity.AdditionalReferenceDate = createModel.AdditionalReferenceDate;
                    invoiceEntity.SellerLegalName = createModel.SellerLegalName;
                    invoiceEntity.SellerTaxCode = createModel.SellerTaxCode;
                    invoiceEntity.SellerAddressLine = createModel.SellerAddressLine;
                    invoiceEntity.SellerPostalCode = createModel.SellerPostalCode;
                    invoiceEntity.SellerDistrictName = createModel.SellerDistrictName;
                    invoiceEntity.SellerCityName = createModel.SellerCityName;
                    invoiceEntity.SellerCountryCode = createModel.SellerCountryCode;
                    invoiceEntity.SellerPhoneNumber = createModel.SellerPhoneNumber;
                    invoiceEntity.SellerFaxNumber = createModel.SellerFaxNumber;
                    invoiceEntity.SellerEmail = createModel.SellerEmail;
                    invoiceEntity.SellerBankName = createModel.SellerBankName;
                    invoiceEntity.SellerBankAccount = createModel.SellerBankAccount;
                    invoiceEntity.SellerContactPersonName = createModel.SellerContactPersonName;
                    invoiceEntity.SellerSignedPersonName = createModel.SellerSignedPersonName;
                    invoiceEntity.SellerSubmittedPersonName = createModel.SellerSubmittedPersonName;
                    invoiceEntity.BuyerDisplayName = createModel.BuyerDisplayName;
                    invoiceEntity.BuyerLegalName = createModel.BuyerLegalName;
                    invoiceEntity.BuyerTaxCode = createModel.BuyerTaxCode;
                    invoiceEntity.BuyerAddressLine = createModel.BuyerAddressLine;
                    invoiceEntity.BuyerPostalCode = createModel.BuyerPostalCode;
                    invoiceEntity.BuyerDistrictName = createModel.BuyerDistrictName;
                    invoiceEntity.BuyerCityName = createModel.BuyerCityName;
                    invoiceEntity.BuyerCountryCode = createModel.BuyerCountryCode;
                    invoiceEntity.BuyerPhoneNumber = createModel.BuyerPhoneNumber;
                    invoiceEntity.BuyerFaxNumber = createModel.BuyerFaxNumber;
                    invoiceEntity.BuyerEmail = createModel.BuyerEmail;
                    invoiceEntity.BuyerBankName = createModel.BuyerBankName;
                    invoiceEntity.BuyerBankAccount = createModel.BuyerBankAccount;
                    invoiceEntity.SumOfTotalLineAmountWithoutVAT = createModel.SumOfTotalLineAmountWithoutVat;
                    invoiceEntity.TotalAmountWithoutVAT = createModel.TotalAmountWithoutVat;
                    invoiceEntity.TotalVATAmount = createModel.TotalVatamount;
                    invoiceEntity.TotalAmountWithVAT = createModel.TotalAmountWithVat;
                    invoiceEntity.TotalAmountWithVATFrn = createModel.TotalAmountWithVatfrn;
                    invoiceEntity.TotalAmountWithVATInWords = createModel.TotalAmountWithVatinWords;
                    invoiceEntity.IsTotalAmountPos = createModel.IsTotalAmountPos;
                    invoiceEntity.IsTotalVATAmountPos = createModel.IsTotalVatamountPos;
                    invoiceEntity.IsTotalAmtWithoutVatPos = createModel.IsTotalAmtWithoutVatPos;
                    invoiceEntity.DiscountAmount = createModel.DiscountAmount;
                    invoiceEntity.IsDiscountAmtPos = createModel.IsDiscountAmtPos;
                    invoiceEntity.ProductCodes = createModel.ProductCodes;
                    invoiceEntity.PortalLink = createModel.PortalLink;
                    invoiceEntity.VatPercentage = createModel.VatPercentage;
                    invoiceEntity.AdjustmentType = InvoiceConstants.InvoiceAdjustmentType.HOA_DON_DIEU_CHINH_THONG_TIN;
                    invoiceEntity.ConvertedStatus = false;
                    invoiceEntity.TrangThaiXemTraCuuHoaDon = false;
                    invoiceEntity.InvoiceCode = InvoiceConstants.PREFIX_INVOICE_CODE + (createModel.CreatedOnDate.HasValue ? createModel.CreatedOnDate.Value.ToString(DateTimeFormatConstants.YYMMDDHHMMSSFFFF) : DateTime.Now.ToString(DateTimeFormatConstants.YYMMDDHHMMSSFFFF));
                    #region Thông tin trường đặc thù theo ngành nghề
                    /// <summary>
                    /// Sử dụng từ ngày (hóa đơn điện, nước..v..v)
                    /// </summary>
                    invoiceEntity.FromUseDate = createModel.FromUseDate;
                    /// <summary>
                    /// Sử dụng đến ngày (hóa đơn điện, nước...v.v..)
                    /// </summary>
                    invoiceEntity.ToUseDate = createModel.ToUseDate;
                    /// <summary>
                    /// Thuế môi trường
                    /// </summary>
                    invoiceEntity.EnvironmentTax = createModel.EnvironmentTax;
                    /// <summary>
                    /// Phí môi trường
                    /// </summary>
                    invoiceEntity.EnvironmentFee = createModel.EnvironmentFee;
                    /// <summary>
                    /// Số đầu kỳ
                    /// </summary>
                    invoiceEntity.StartNumber = createModel.StartNumber;
                    /// <summary>
                    /// Số cuối kỳ
                    /// </summary>
                    invoiceEntity.EndNumber = createModel.StartNumber;
                    /// <summary>
                    /// Số sử dụng trong kỳ
                    /// </summary>
                    invoiceEntity.UseNumber = createModel.UseNumber;
                    /// <summary>
                    /// Tiền dịch vụ thoát nước
                    /// </summary>
                    //invoiceEntity.DrainageServicePrice = createModel.DrainageServicePrice;
                    /// <summary>
                    /// Tiền thuế GTGT của dịch vụ thoát nước
                    /// </summary>
                    //invoiceEntity.DrainageServiceTaxMoney = createModel.DrainageServiceTaxMoney;
                    #endregion
                    if (createModel.IsPublishInvoice.HasValue && createModel.IsPublishInvoice.Value)
                    {
                        var item = invoiceRepo.GetMany(x => x.InvoiceTemplateId == invoiceEntity.InvoiceTemplateId && x.InvoiceSeries == invoiceEntity.InvoiceSeries).OrderByDescending(x => x.InvoiceNumber).FirstOrDefault();

                        var invoiceNumber = InvoiceConstants.FIRST_INVOICE_NUMBER;

                        if (item != null && !string.IsNullOrWhiteSpace(item.InvoiceNumber))
                        {
                            long numInvoiceNumber = 0;
                            try
                            {
                                numInvoiceNumber = Convert.ToInt64(item.InvoiceNumber);
                            }
                            catch (Exception ex)
                            {
                                logger.Debug(ex);
                            }
                            invoiceNumber = (++numInvoiceNumber).ToString().PadLeft(7, '0');
                        }
                        else
                        {
                            invoiceNumber = "1".PadLeft(7, '0');
                        }

                        // invoiceEntity.InvoiceStatus = InvoiceConstants.InvoiceStatus.HOA_DON_DA_PHAT_HANH;
                        if (invoiceEntity.InvoiceNumber.Equals(InvoiceConstants.FIRST_INVOICE_NUMBER))
                        {
                            invoiceEntity.InvoiceNumber = invoiceNumber;
                        }
                        invoiceEntity.PublishBy = createModel.CreatedByUserId;
                    }
                    else
                    {
                        invoiceEntity.InvoiceStatus = InvoiceConstants.InvoiceStatus.HOA_DON_MOI_TAO_LAP;
                        invoiceEntity.InvoiceNumber = InvoiceConstants.FIRST_INVOICE_NUMBER;
                    }
                    // invoiceEntity.InvoiceStatus = (createModel.InvoiceStatus != null) ? createModel.InvoiceStatus : InvoiceConstants.InvoiceStatus.HOA_DON_MOI_TAO_LAP;
                    invoiceEntity.CreatedByUserId = createModel.CreatedByUserId;
                    invoiceEntity.CreatedOnDate = createModel.CreatedOnDate;
                    try
                    {
                        invoiceEntity.TotalAmountWithVATInWords = Utils.DocTienBangChu((long)invoiceEntity.TotalAmountWithVAT, " đồng");
                    }
                    catch (Exception ex)
                    {
                        logger.Debug(ex);
                        invoiceEntity.TotalAmountWithVATInWords = "Không đồng";
                    }
                    invoiceRepo.Add(invoiceEntity);

                    if (createModel.ListProductInvoice != null && createModel.ListProductInvoice.Count > 0)
                    {
                        foreach (var itemPI in createModel.ListProductInvoice)
                        {
                            Product_Invoice pi = new Product_Invoice();
                            pi.ProductInvoiceId = Guid.NewGuid();
                            pi.InvoiceId = invoiceEntity.InvoiceId;
                            pi.LineNumber = itemPI.LineNumber;
                            pi.ItemCode = itemPI.ItemCode;
                            pi.ItemName = itemPI.ItemName;
                            pi.UnitCode = itemPI.UnitCode;
                            pi.UnitName = itemPI.UnitName;
                            pi.UnitPrice = itemPI.UnitPrice;
                            pi.Quantity = itemPI.Quantity;
                            pi.ItemTotalAmountWithoutVat = itemPI.ItemTotalAmountWithoutVat;
                            pi.VatPercentage = itemPI.VatPercentage;
                            pi.VatAmount = itemPI.VatAmount;
                            pi.Promotion = itemPI.Promotion;
                            pi.LotNumber = itemPI.LotNumber;
                            pi.ItemTotalAmountWithVat = itemPI.ItemTotalAmountWithVat;
                            pi.ExpirationDate = itemPI.ExpirationDate;
                            pi.AdjustmentVatAmount = itemPI.AdjustmentVatAmount;
                            pi.IsIncreaseItem = itemPI.IsIncreaseItem;
                            pi.ItemType = itemPI.ItemType;
                            productinvoiceRepo.Add(pi);
                        }
                    }

                    var result = unitOfWork.Save();
                    if (result >= 1)
                    {
                        var invoiceModel = InvoiceConvert.entityToModel(invoiceEntity);
                        invoiceModel.IsPublishInvoice = createModel.IsPublishInvoice;
                        return new Response<BasicInvoiceModel>(1, MessageResponseConstants.InvoiceMessage.MESSAGE_ADJUSTMENT_SUCCESS, invoiceModel);
                    }
                    else
                    {
                        return new Response<BasicInvoiceModel>(-1, MessageResponseConstants.InvoiceMessage.MESSAGE_ADJUSTMENT_FAILED_DB, null);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                return new Response<BasicInvoiceModel>(-1, MessageResponseConstants.MESSAGE_EXCEPTION + ex.Message, null);
            }
        }

        /// <summary>
        /// Hủy hóa đơn
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Response<BasicInvoiceModel> Cancel(Guid invoiceId, Guid userId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var invoiceRepo = unitOfWork.GetRepository<Invoice>();
                    //check unique code

                    var invoiceEntity = invoiceRepo.GetById(invoiceId);
                    if (invoiceEntity != null)
                    {
                        if (invoiceEntity.InvoiceStatus.Equals(InvoiceConstants.InvoiceStatus.HOA_DON_DA_PHAT_HANH))
                        {
                            invoiceEntity.InvoiceStatus = InvoiceConstants.InvoiceStatus.HOA_DON_BI_XOA_BO;
                            invoiceEntity.LastModifiedByUserId = userId;
                            invoiceEntity.LastModifiedOnDate = DateTime.Now;
                            invoiceRepo.Update(invoiceEntity);
                        }
                        else
                        {
                            return new Response<BasicInvoiceModel>(0, MessageResponseConstants.InvoiceMessage.MESSAGE_INVOICE_CANCEL_FAILED, InvoiceConvert.entityToModel(invoiceEntity));
                        }
                    }
                    else
                    {
                        return new Response<BasicInvoiceModel>(0, MessageResponseConstants.InvoiceMessage.MESSAGE_ORIGINAL_INVOICE_NOT_FOUND, InvoiceConvert.entityToModel(invoiceEntity));
                    }

                    var result = unitOfWork.Save();
                    if (result >= 1)
                    {
                        return new Response<BasicInvoiceModel>(1, MessageResponseConstants.InvoiceMessage.MESSAGE_CANCEL_SUCCESS, InvoiceConvert.entityToModel(invoiceEntity));
                    }
                    else
                    {
                        return new Response<BasicInvoiceModel>(-1, MessageResponseConstants.InvoiceMessage.MESSAGE_CANCEL_FAILED_DB, null);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                return new Response<BasicInvoiceModel>(-1, MessageResponseConstants.MESSAGE_EXCEPTION + ex.Message, null);
            }
        }

        public class InvoiceConvert
        {
            public static BasicInvoiceModel entityToModel(Invoice entity)
            {
                BasicInvoiceModel basicInvoiceModel = new BasicInvoiceModel();
                basicInvoiceModel.InvoiceId = entity.InvoiceId;
                basicInvoiceModel.InvoiceCategoryId = entity.InvoiceCategoryId;
                basicInvoiceModel.InvoiceTemplateId = entity.InvoiceTemplateId;
                basicInvoiceModel.CompanyId = entity.CompanyId;
                basicInvoiceModel.CustomerId = entity.CustomerId;
                basicInvoiceModel.InvoiceCode = entity.InvoiceCode;
                basicInvoiceModel.Data = entity.Data;
                basicInvoiceModel.Signature = entity.Signature;
                basicInvoiceModel.Converted = entity.Converted;
                basicInvoiceModel.KindOfService = entity.KindOfService;
                basicInvoiceModel.ArisingDate = entity.ArisingDate;
                basicInvoiceModel.ProcessInvoiceNote = entity.ProcessInvoiceNote;
                basicInvoiceModel.GrossValue = entity.GrossValue;
                basicInvoiceModel.VatAmount0 = entity.VatAmount0;
                basicInvoiceModel.GrossValue0 = entity.GrossValue0;
                basicInvoiceModel.VatAmount5 = entity.VatAmount5;
                basicInvoiceModel.GrossValue5 = entity.GrossValue5;
                basicInvoiceModel.VatAmount10 = entity.VatAmount10;
                basicInvoiceModel.GrossValue10 = entity.GrossValue10;
                basicInvoiceModel.Certified = entity.Certified;
                basicInvoiceModel.CertifiedId = entity.CertifiedId;
                basicInvoiceModel.CertifiedData = entity.CertifiedData;
                basicInvoiceModel.CreationDate = entity.CreationDate;
                basicInvoiceModel.PublishDate = entity.PublishDate;
                basicInvoiceModel.CreationBy = entity.CreationBy;
                basicInvoiceModel.PublishBy = entity.PublishBy;
                basicInvoiceModel.PaymentMethod = entity.PaymentMethod;
                basicInvoiceModel.SellerAppRecordId = entity.SellerAppRecordId;
                basicInvoiceModel.InvoiceAppRecordId = entity.InvoiceAppRecordId;
                basicInvoiceModel.InvoiceType = entity.InvoiceType;
                basicInvoiceModel.TemplateCode = entity.TemplateCode;
                basicInvoiceModel.InvoiceSeries = entity.InvoiceSeries;
                basicInvoiceModel.InvoiceNumber = entity.InvoiceNumber;
                basicInvoiceModel.InvoiceName = entity.InvoiceName;
                basicInvoiceModel.InvoiceIssuedDate = entity.InvoiceIssuedDate;
                basicInvoiceModel.SignedDate = entity.SignedDate;
                basicInvoiceModel.SubmittedDate = entity.SubmittedDate;
                basicInvoiceModel.ContractNumber = entity.ContractNumber;
                basicInvoiceModel.ContractDate = entity.ContractDate;
                basicInvoiceModel.CurrencyCode = entity.CurrencyCode;
                basicInvoiceModel.ExchangeRate = entity.ExchangeRate;
                basicInvoiceModel.InvoiceNote = entity.InvoiceNote;
                basicInvoiceModel.AdjustmentType = entity.AdjustmentType;
                basicInvoiceModel.OriginalInvoiceId = entity.OriginalInvoiceId;
                basicInvoiceModel.AdditionalReferenceDesc = entity.AdditionalReferenceDesc;
                basicInvoiceModel.AdditionalReferenceDate = entity.AdditionalReferenceDate;
                basicInvoiceModel.SellerLegalName = entity.SellerLegalName;
                basicInvoiceModel.SellerTaxCode = entity.SellerTaxCode;
                basicInvoiceModel.SellerAddressLine = entity.SellerAddressLine;
                basicInvoiceModel.SellerPostalCode = entity.SellerPostalCode;
                basicInvoiceModel.SellerDistrictName = entity.SellerDistrictName;
                basicInvoiceModel.SellerCityName = entity.SellerCityName;
                basicInvoiceModel.SellerCountryCode = entity.SellerCountryCode;
                basicInvoiceModel.SellerPhoneNumber = entity.SellerPhoneNumber;
                basicInvoiceModel.SellerFaxNumber = entity.SellerFaxNumber;
                basicInvoiceModel.SellerEmail = entity.SellerEmail;
                basicInvoiceModel.SellerBankName = entity.SellerBankName;
                basicInvoiceModel.SellerBankAccount = entity.SellerBankAccount;
                basicInvoiceModel.SellerContactPersonName = entity.SellerContactPersonName;
                basicInvoiceModel.SellerSignedPersonName = entity.SellerSignedPersonName;
                basicInvoiceModel.SellerSubmittedPersonName = entity.SellerSubmittedPersonName;
                basicInvoiceModel.BuyerDisplayName = entity.BuyerDisplayName;
                basicInvoiceModel.BuyerLegalName = entity.BuyerLegalName;
                basicInvoiceModel.BuyerTaxCode = entity.BuyerTaxCode;
                basicInvoiceModel.BuyerAddressLine = entity.BuyerAddressLine;
                basicInvoiceModel.BuyerPostalCode = entity.BuyerPostalCode;
                basicInvoiceModel.BuyerDistrictName = entity.BuyerDistrictName;
                basicInvoiceModel.BuyerCityName = entity.BuyerCityName;
                basicInvoiceModel.BuyerCountryCode = entity.BuyerCountryCode;
                basicInvoiceModel.BuyerPhoneNumber = entity.BuyerPhoneNumber;
                basicInvoiceModel.BuyerFaxNumber = entity.BuyerFaxNumber;
                basicInvoiceModel.BuyerEmail = entity.BuyerEmail;
                basicInvoiceModel.BuyerBankName = entity.BuyerBankName;
                basicInvoiceModel.BuyerBankAccount = entity.BuyerBankAccount;
                basicInvoiceModel.SumOfTotalLineAmountWithoutVat = entity.SumOfTotalLineAmountWithoutVAT;
                basicInvoiceModel.TotalAmountWithoutVat = entity.TotalAmountWithoutVAT;
                basicInvoiceModel.TotalVatamount = entity.TotalVATAmount;
                basicInvoiceModel.TotalAmountWithVat = entity.TotalAmountWithVAT;
                basicInvoiceModel.TotalAmountWithVatfrn = entity.TotalAmountWithVATFrn;
                basicInvoiceModel.TotalAmountWithVatinWords = entity.TotalAmountWithVATInWords;
                basicInvoiceModel.IsTotalAmountPos = entity.IsTotalAmountPos;
                basicInvoiceModel.IsTotalVatamountPos = entity.IsTotalVATAmountPos;
                basicInvoiceModel.IsTotalAmtWithoutVatPos = entity.IsTotalAmtWithoutVatPos;
                basicInvoiceModel.DiscountAmount = entity.DiscountAmount;
                basicInvoiceModel.IsDiscountAmtPos = entity.IsDiscountAmtPos;
                basicInvoiceModel.ProductCodes = entity.ProductCodes;
                basicInvoiceModel.PortalLink = entity.PortalLink;
                basicInvoiceModel.InvoiceStatus = entity.InvoiceStatus;
                basicInvoiceModel.TrangThaiXemTraCuuHoaDon = entity.TrangThaiXemTraCuuHoaDon;
                basicInvoiceModel.VatPercentage = entity.VatPercentage;
                basicInvoiceModel.CreatedByUserId = entity.CreatedByUserId;
                basicInvoiceModel.CreatedOnDate = entity.CreatedOnDate;
                basicInvoiceModel.LastModifiedByUserId = entity.LastModifiedByUserId;
                basicInvoiceModel.LastModifiedOnDate = entity.LastModifiedOnDate;

                return basicInvoiceModel;
            }

            public static List<BasicInvoiceModel> listEntityToListModel(List<Invoice> listEntity)
            {
                List<BasicInvoiceModel> listBasicInvoiceModel = new List<BasicInvoiceModel>();
                for (var i = 0; i < listEntity.Count; i++)
                {
                    var model = entityToModel(listEntity[i]);
                    listBasicInvoiceModel.Add(model);
                }
                return listBasicInvoiceModel;
            }
        }
    }
}
