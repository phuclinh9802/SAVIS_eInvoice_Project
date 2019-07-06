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
using System.Data.SqlClient;
using System.Data;

namespace BASIC.AUTHEN.BUSSINESS
{
    public class DbSynchronizedHandler
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string invoice_api_url = ConfigurationManager.AppSettings["INVOICE_API_URL"];

        /// <summary>
        /// Đồng bộ dữ liệu mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Response<List<ProductSyncOutputModel>> SynchronizedProduct(ProductInputDataModel data)
        {
            try
            {
                List<ProductSyncOutputModel> dataReturn = new List<ProductSyncOutputModel>();
                List<ProductInputModel> dataProduct = data.Data;
                List<ProductInputModel> dataRealProduct = new List<ProductInputModel>();
                if (dataProduct == null || dataProduct.Count == 0)
                {
                    return new Response<List<ProductSyncOutputModel>>(CONSTANT_STATUS_API.THAT_BAI, "Dữ liệu mặt hàng trống", null);
                }
                if (string.IsNullOrWhiteSpace(data.CompanyTax))
                {
                    return new Response<List<ProductSyncOutputModel>>(CONSTANT_STATUS_API.THAT_BAI, "Mã số thuế công ty trống", null);
                }
                using (var unitOfWork = new UnitOfWork())
                {
                    var company = unitOfWork.GetRepository<Company>().GetMany(x => x.TaxNumber == data.CompanyTax).FirstOrDefault();
                    if (company == null)
                    {
                        return new Response<List<ProductSyncOutputModel>>(CONSTANT_STATUS_API.THAT_BAI, "Không tìm thấy thông tin công ty", null);
                    }
                    var dataProductCheck = unitOfWork.GetRepository<Product>().GetAll();
                    foreach (var item in dataProduct)
                    {
                        bool flag = true;
                        //Kiểm tra mã mặt hàng
                        if (string.IsNullOrWhiteSpace(item.ProductCode))
                        {
                            flag = false;
                            dataReturn.Add(new ProductSyncOutputModel() { ProductCode = "Trống mã mặt hàng", Status = 0 });
                        }
                        //Kiểm tra thông tin xem mặt hàng này có trong CSDL của HĐĐT chưa
                        var pCheck = dataProductCheck.FirstOrDefault(x => x.Code == item.ProductCode);
                        if (pCheck != null)
                        {
                            flag = false;
                            dataReturn.Add(new ProductSyncOutputModel() { ProductCode = item.ProductCode+"-Mã mặt hàng này đã có trong CSDL", Status = 0 });
                        }
                        //Kiểm tra tên mặt hàng
                        if (string.IsNullOrWhiteSpace(item.ProductName))
                        {
                            flag = false;
                            dataReturn.Add(new ProductSyncOutputModel() { ProductCode = item.ProductCode+"-Tên mặt hàng trống", Status = 0 });
                        }
                        //Kiểm tra tên đơn vị tính
                        //if (string.IsNullOrWhiteSpace(item.UnitName))
                        //{
                        //    flag = false;
                        //    dataReturn.Add(new ProductSyncOutputModel() { ProductCode = item.ProductCode + "-Đơn vị tính trống", Status = 0 });
                        //}

                        if (flag)//Những mặt hàng nào thỏa mãn điều kiện mới add vào list đưa vào DB
                        {
                            dataRealProduct.Add(item);
                            dataReturn.Add(new ProductSyncOutputModel() { ProductCode = item.ProductCode + "-Sync Success", Status = 1 });
                        }
                    }
                    InsertDBProduct(dataRealProduct, company.CompanyId);
                    return new Response<List<ProductSyncOutputModel>>(1, "Success", dataReturn);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<List<ProductSyncOutputModel>>(CONSTANT_STATUS_API.THAT_BAI, "Lỗi: " + ex.Message, null);
            }
        }

        /// <summary>
        /// Bulk insert to DB
        /// </summary>
        /// <param name="dataProduct"></param>
        /// <param name="companyId"></param>
        public void InsertDBProduct(List<ProductInputModel> dataProduct, Guid companyId)
        {
            try
            {
                DbCommonHandler CommonFunction = new DbCommonHandler();
                #region Clone bieu_giatri đối với các biểu in có nội dung out
                string cnn = ConfigurationManager.ConnectionStrings["CustomConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(cnn))
                {
                    conn.Open();
                    SqlTransaction transaction = null;
                    DataTable tbProducts = null;
                    string tableName = "Products";
                    try
                    {
                        transaction = conn.BeginTransaction();
                        tbProducts = CommonFunction.GetSchemaInfo(tableName, conn, transaction);
                        foreach (var iCell in dataProduct)
                        {
                            DataRow r = tbProducts.NewRow();
                            r[0] = Guid.NewGuid();
                            r[1] = companyId;//CompanyId
                            r[2] = DBNull.Value;//TypeOfProduct
                            r[3] = DBNull.Value;//Unit Id đơn vị tính, tạm thời không dùng
                            if (iCell.Price != null)//Price
                                r[4] = iCell.Price;
                            else
                                r[4] = DBNull.Value;
                            r[5] = DBNull.Value;//BarCode
                            r[6] = iCell.ProductCode;//Code
                            r[7] = iCell.ProductName;//Name
                            r[8] = DBNull.Value;//ShortName
                            r[9] = true;//Status mặc định đang hoạt động
                            r[10] = DBNull.Value;//NodeId
                            if (!string.IsNullOrWhiteSpace(iCell.Description))//Description
                                r[11] = iCell.Description;
                            else
                                r[11] = DBNull.Value;
                            r[12] = DBNull.Value;//Tax
                            r[13] = DBNull.Value;//[Order]
                            r[14] = DBNull.Value;//PriceBeforeVAT
                            r[15] = DBNull.Value;//DiscountPercent
                            r[16] = DBNull.Value;//AmountBeforeVAT
                            r[17] = iCell.VAT;//VATPercent
                            r[18] = DBNull.Value;//PriceAfterVAT
                            r[19] = DBNull.Value;//TotalAmountAfterVAT
                            r[20] = DBNull.Value;//CreatedByUserId
                            r[21] = DBNull.Value;//CreatedOnDate
                            r[22] = DBNull.Value;//LastModifiedByUserId
                            r[23] = DBNull.Value;//LastModifiedOnDate
                            r[24] = companyId;//ApplicationId
                            r[25] = iCell.UnitName;//UnitName
                            tbProducts.Rows.Add(r);
                        }
                        CommonFunction.BulkInsertTable(tableName, tbProducts, conn, transaction);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                    finally
                    {
                        if (tbProducts != null) { tbProducts.Dispose(); }
                        if (transaction != null) { transaction.Dispose(); }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Cập nhật thông tin mặt hàng
        /// </summary>
        /// <param name="editModel"></param>
        /// <returns></returns>
        public Response<ProductSyncOutputModel> UpdateProductInfo(ProductInputModel editModel)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    ProductSyncOutputModel rt = new ProductSyncOutputModel();
                    rt.ProductCode = editModel.ProductCode;
                    rt.Status = 0;
                    if (editModel == null)
                    {
                        return new Response<ProductSyncOutputModel>(0, "Lỗi: Dữ liệu đầu vào trống", rt);
                    }
                    if (string.IsNullOrWhiteSpace(editModel.CompanyTax))
                    {
                        return new Response<ProductSyncOutputModel>(0, "Lỗi: Mã số thuế công ty trống", rt);
                    }
                    var company = unitOfWork.GetRepository<Company>().GetMany(x => x.TaxNumber == editModel.CompanyTax).FirstOrDefault();
                    if (company == null)
                    {
                        return new Response<ProductSyncOutputModel>(0, "Lỗi: Không tìm thấy thông tin công ty với mã số thuế: " + editModel.CompanyTax, rt);
                    }
                    if (string.IsNullOrWhiteSpace(editModel.ProductCode))
                    {
                        return new Response<ProductSyncOutputModel>(0, "Lỗi: Mã mặt hàng trống", rt);
                    }
                    var productEntity = unitOfWork.GetRepository<Product>().GetMany(x => x.Code == editModel.ProductCode && x.CompanyId == company.CompanyId).FirstOrDefault();
                    if (productEntity == null)
                    {
                        return new Response<ProductSyncOutputModel>(0, "Lỗi: Không tìm thấy thông tin mặt hàng mã: " + editModel.ProductCode, rt);
                    }
                    productEntity.UnitName = editModel.UnitName;
                    productEntity.Price = editModel.Price;
                    productEntity.Code = editModel.ProductCode;
                    productEntity.Name = editModel.ProductName;
                    productEntity.ShortName = editModel.ProductName;
                    productEntity.Description = editModel.Description;
                    productEntity.VATPercent = Convert.ToDouble(editModel.VAT);
                    productEntity.LastModifiedOnDate = DateTime.Now;

                    unitOfWork.GetRepository<Product>().Update(productEntity);

                    var result = unitOfWork.Save();
                    if (result >= 1)
                    {
                        rt.Status = 1;
                        return new Response<ProductSyncOutputModel>(1, MessageResponseConstants.MESSAGE_UPDATE_SUCCESS, rt);
                    }
                    else
                    {
                        return new Response<ProductSyncOutputModel>(0, MessageResponseConstants.MESSAGE_UPDATE_FAILED_DB, rt);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<ProductSyncOutputModel>(0, MessageResponseConstants.MESSAGE_EXCEPTION + ex.Message, null);
            }
        }

        /// <summary>
        /// Đồng bộ dữ liệu khách hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Response<List<CustomerSyncOutputModel>> SynchronizedCustomer(CustomerInputDataModel data)
        {
            try
            {
                List<CustomerSyncOutputModel> dataReturn = new List<CustomerSyncOutputModel>();
                List<CustomerInputModel> dataCustomer = data.Data;
                List<CustomerInputModel> dataRealCustomer = new List<CustomerInputModel>();
                if (dataCustomer == null || dataCustomer.Count == 0)
                {
                    return new Response<List<CustomerSyncOutputModel>>(CONSTANT_STATUS_API.THAT_BAI, "Dữ liệu khách hàng trống", null);
                }
                if (string.IsNullOrWhiteSpace(data.CompanyTax))
                {
                    return new Response<List<CustomerSyncOutputModel>>(CONSTANT_STATUS_API.THAT_BAI, "Mã số thuế công ty trống", null);
                }
                using (var unitOfWork = new UnitOfWork())
                {
                    var company = unitOfWork.GetRepository<Company>().GetMany(x => x.TaxNumber == data.CompanyTax).FirstOrDefault();
                    if (company == null)
                    {
                        return new Response<List<CustomerSyncOutputModel>>(CONSTANT_STATUS_API.THAT_BAI, "Không tìm thấy thông tin công ty", null);
                    }
                    var dataProductCheck = unitOfWork.GetRepository<Customer>().GetAll();
                    foreach (var item in dataCustomer)
                    {
                        bool flag = true;
                        //Kiểm tra mã khách hàng
                        if (string.IsNullOrWhiteSpace(item.CustomerCode))
                        {
                            flag = false;
                            dataReturn.Add(new CustomerSyncOutputModel() { CustomerCode = "Trống mã khách hàng", Status = 0 });
                        }
                        //Kiểm tra thông tin xem khách hàng này có trong CSDL của HĐĐT chưa
                        var pCheck = dataProductCheck.FirstOrDefault(x => x.CustomerCode == item.CustomerCode);
                        if (pCheck != null)
                        {
                            flag = false;
                            dataReturn.Add(new CustomerSyncOutputModel() { CustomerCode = item.CustomerCode+"-Mã khách hàng này đã tồn tại trong CSDL", Status = 0 });
                        }
                        //Kiểm tra tên khách hàng
                        if (string.IsNullOrWhiteSpace(item.CustomerName))
                        {
                            flag = false;
                            dataReturn.Add(new CustomerSyncOutputModel() { CustomerCode = item.CustomerCode+"-Tên khách hàng trống", Status = 0 });
                        }
                        //Kiểm tra loại khách hàng là đơn vị kế toán thì bắt buộc phải có mã số thuế
                        if (item.CustomerType == 1 && string.IsNullOrWhiteSpace(item.CustomerTax))
                        {
                            flag = false;
                            dataReturn.Add(new CustomerSyncOutputModel() { CustomerCode = item.CustomerCode+"-Mã số thuế trống", Status = 0 });
                        }
                        if (flag)//Những mặt hàng nào thỏa mãn điều kiện mới add vào list đưa vào DB
                        {
                            dataRealCustomer.Add(item);
                            dataReturn.Add(new CustomerSyncOutputModel() { CustomerCode = item.CustomerCode + "-Sync Success", Status = 1 });
                        }
                    }
                    InsertDBCustomer(dataRealCustomer, company.CompanyId);
                    return new Response<List<CustomerSyncOutputModel>>(1, "Success", dataReturn);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<List<CustomerSyncOutputModel>>(CONSTANT_STATUS_API.THAT_BAI, "Lỗi: " + ex.Message, null);
            }
        }

        /// <summary>
        /// Bulk insert dữ liệu khách hàng vào DB
        /// </summary>
        /// <param name="dataCustomer"></param>
        /// <param name="companyId"></param>
        public void InsertDBCustomer(List<CustomerInputModel> dataCustomer, Guid companyId)
        {
            try
            {
                DbCommonHandler CommonFunction = new DbCommonHandler();
                string cnn = ConfigurationManager.ConnectionStrings["CustomConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(cnn))
                {
                    conn.Open();
                    SqlTransaction transaction = null;
                    DataTable tbCustomers = null;
                    string tableName = "Customer";
                    try
                    {
                        transaction = conn.BeginTransaction();
                        tbCustomers = CommonFunction.GetSchemaInfo(tableName, conn, transaction);
                        foreach (var iCell in dataCustomer)
                        {
                            DataRow r = tbCustomers.NewRow();
                            r[0] = Guid.NewGuid();//CustomerId
                            r[1] = iCell.CustomerName;//Name
                            if (!string.IsNullOrWhiteSpace(iCell.CustomerBankAccount))
                            {
                                r[2] = iCell.CustomerBankAccount;//BankAccountName
                                r[4] = iCell.CustomerBankAccount;//BankNumber
                            }
                            else
                            {
                                r[2] = DBNull.Value;//BankAccountName
                                r[4] = DBNull.Value;//BankNumber
                            }
                            if (!string.IsNullOrWhiteSpace(iCell.CustomerBankName))
                                r[3] = iCell.CustomerBankName;//BankName
                            else
                                r[3] = DBNull.Value;//BankName
                            if (!string.IsNullOrWhiteSpace(iCell.CustomerBuyer))
                            {
                                r[5] = iCell.CustomerBuyer;//ContactPerson
                            }
                            else {
                                r[5] = DBNull.Value;//ContactPerson
                            }
                            r[6] = DBNull.Value;//Description
                            if (!string.IsNullOrWhiteSpace(iCell.CustomerAddress))
                            {
                                r[7] = iCell.CustomerAddress;//Address
                            }
                            else
                            {
                                r[7] = DBNull.Value;//Address
                            }
                            if (!string.IsNullOrWhiteSpace(iCell.CustomerEmail))
                            {
                                r[8] = iCell.CustomerEmail;//Email
                            }
                            else
                            {
                                r[8] = DBNull.Value;//Email
                            }
                            if (!string.IsNullOrWhiteSpace(iCell.CustomerPhone))
                            {
                                r[9] = iCell.CustomerPhone;//Phone
                            }
                            else
                            {
                                r[9] = DBNull.Value;//Phone
                            }
                            r[10] = DBNull.Value;//Fax

                            if (!string.IsNullOrWhiteSpace(iCell.CustomerBuyer))//RepresentPerson
                            {
                                r[11] = iCell.CustomerBuyer;//RepresentPerson
                            }
                            else
                            {
                                r[11] = DBNull.Value;
                            }
                            if (!string.IsNullOrWhiteSpace(iCell.CustomerTax))
                            {
                                r[12] = iCell.CustomerTax;//TaxNumber
                            }
                            else
                            {
                                r[12] = DBNull.Value;//TaxNumber
                            }
                            r[13] = DBNull.Value;//AccountId
                            r[14] = companyId;//CompanyId
                            r[15] = DBNull.Value;//SignatureImage
                            r[16] = DBNull.Value;//SerialCert
                            r[17] = DBNull.Value;//DeliverMethod
                            r[18] = DBNull.Value;//CC
                            r[19] = DBNull.Value;//CreatedByUserId
                            r[20] = DBNull.Value;//CreatedOnDate
                            r[21] = DBNull.Value;//LastModifiedByUserId
                            r[22] = DBNull.Value;//LastModifiedOnDate
                            r[23] = true;//Approved
                            r[24] = true;//IsUsed
                            r[25] = 1;//InvoiceStatus
                            r[26] = "";//Password
                            r[27] = "";//UserName
                            r[28] = iCell.CustomerCode;//CustomerCode                            
                            r[29] = companyId;//ApplicationId
                            r[30] = iCell.CustomerType;//CustomerType
                            r[31] = iCell.CustomerIdentity;//IdentityNumber
                            tbCustomers.Rows.Add(r);
                        }
                        CommonFunction.BulkInsertTable(tableName, tbCustomers, conn, transaction);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                    finally
                    {
                        if (tbCustomers != null) { tbCustomers.Dispose(); }
                        if (transaction != null) { transaction.Dispose(); }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Sửa thông tin khách hàng
        /// </summary>
        /// <param name="editModel"></param>
        /// <returns></returns>
        public Response<CustomerSyncOutputModel> UpdateCustomerInfo(CustomerInputModel editModel)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    CustomerSyncOutputModel rt = new CustomerSyncOutputModel();
                    rt.CustomerCode = editModel.CustomerCode;
                    rt.Status = 0;
                    if (editModel == null)
                    {
                        return new Response<CustomerSyncOutputModel>(0, "Lỗi: Dữ liệu đầu vào trống", rt);
                    }
                    if (string.IsNullOrWhiteSpace(editModel.CustomerCode))
                    {
                        return new Response<CustomerSyncOutputModel>(0, "Lỗi: Mã khách hàng trống", rt);
                    }
                    if (string.IsNullOrWhiteSpace(editModel.CompanyTax))
                    {
                        return new Response<CustomerSyncOutputModel>(0, "Lỗi: Mã số thuế công ty trống", rt);
                    }
                    var company = unitOfWork.GetRepository<Company>().GetMany(x => x.TaxNumber == editModel.CompanyTax).FirstOrDefault();
                    if (company == null)
                    {
                        return new Response<CustomerSyncOutputModel>(0, "Lỗi: Không tìm thấy thông tin công ty với mã số thuế: " + editModel.CompanyTax, rt);
                    }
                    var customerEntity = unitOfWork.GetRepository<Customer>().GetMany(x => x.CustomerCode == editModel.CustomerCode && x.CompanyId == company.CompanyId).FirstOrDefault();
                    if (customerEntity == null)
                    {
                        return new Response<CustomerSyncOutputModel>(0, "Lỗi: Không tìm thấy thông tin khách hàng mã: " + editModel.CustomerCode, rt);
                    }

                    customerEntity.CustomerCode = editModel.CustomerCode;
                    customerEntity.Name = editModel.CustomerName;
                    customerEntity.BankAccountName = editModel.CustomerBankAccount;
                    customerEntity.BankName = editModel.CustomerBankName;
                    customerEntity.BankNumber = editModel.CustomerBankAccount;
                    customerEntity.ContactPerson = editModel.CustomerBuyer;
                    customerEntity.Address = editModel.CustomerAddress;
                    customerEntity.Email = editModel.CustomerEmail;
                    customerEntity.Phone = editModel.CustomerPhone;
                    customerEntity.RepresentPerson = editModel.CustomerBuyer;
                    customerEntity.TaxNumber = editModel.CustomerTax;
                    customerEntity.IdentityNumber = editModel.CustomerIdentity;
                    customerEntity.LastModifiedOnDate = DateTime.Now;
                    customerEntity.CustomerType = editModel.CustomerType;

                    unitOfWork.GetRepository<Customer>().Update(customerEntity);

                    var result = unitOfWork.Save();
                    if (result >= 1)
                    {
                        rt.Status = 1;
                        return new Response<CustomerSyncOutputModel>(1, MessageResponseConstants.MESSAGE_UPDATE_SUCCESS, rt);
                    }
                    else
                    {
                        return new Response<CustomerSyncOutputModel>(0, MessageResponseConstants.MESSAGE_UPDATE_FAILED_DB, rt);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<CustomerSyncOutputModel>(0, MessageResponseConstants.MESSAGE_EXCEPTION + ex.Message, null);
            }
        }
    }
}
