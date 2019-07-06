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
using System.Data;
using System.Data.SqlClient;

namespace BASIC.AUTHEN.BUSSINESS
{
    public class DbCommonHandler
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string invoice_api_url = ConfigurationManager.AppSettings["INVOICE_API_URL"];
        /// <summary>
        /// Lấy danh sách hình thức thanh toán
        /// </summary>
        /// <returns></returns>
        public Response<List<PaymentMethodModel>> GetPaymentMethod()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    List<PaymentMethodModel> dataReturn = new List<PaymentMethodModel>();
                    dataReturn = (from dt in unitOfWork.GetRepository<Catalog_Item>().GetAll()
                                  join term in unitOfWork.GetRepository<Taxonomy_Term>().GetAll()
                                  on dt.MappedTermId equals term.TermId
                                  join cm in unitOfWork.GetRepository<Catalog_Master>().GetAll()
                                  on term.VocabularyId equals cm.MappedVocabularyId
                                  where cm.Code == CONSTANT_CATALOG.HINH_THUC_THANH_TOAN
                                  orderby dt.Name
                                  select new PaymentMethodModel()
                                  {
                                      PaymentMethodId = dt.CatalogItemId,
                                      Name = dt.Name,
                                      Code = dt.Code
                                  }).ToList();
                    return new Response<List<PaymentMethodModel>>(1, "Success", dataReturn);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message
);
                return new Response<List<PaymentMethodModel>>(-1, "Lỗi: " + ex.Message, null);
            }
        }

        /// <summary>
        /// Lấy danh sách khách hàng
        /// </summary>
        /// <returns></returns>
        public Response<List<CustomerModel>> GetCustomer()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    List<CustomerModel> dataReturn = new List<CustomerModel>();
                    dataReturn = (from dt in unitOfWork.GetRepository<Customer>().GetMany(x => x.IsUsed == true)
                                  orderby dt.Name
                                  select new CustomerModel()
                                  {
                                      Address = dt.Address,
                                      BankAccountName = dt.BankAccountName,
                                      BankName = dt.BankName,
                                      BankNumber = dt.BankNumber,
                                      Code = dt.CustomerCode,
                                      ContactPerson = dt.ContactPerson,
                                      CustomerId = dt.CustomerId,
                                      Email = dt.Email,
                                      Fax = dt.Fax,
                                      Name = dt.Name,
                                      Phone = dt.Phone,
                                      TaxNumber = dt.TaxNumber
                                  }).ToList();
                    return new Response<List<CustomerModel>>(1, "Success", dataReturn);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<List<CustomerModel>>(-1, "Lỗi: " + ex.Message, null);
            }
        }

        /// <summary>
        /// Lấy danh sách đơn vị tính
        /// </summary>
        /// <returns></returns>
        public Response<List<UnitModel>> GetDataUnit()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    List<UnitModel> dataReturn = new List<UnitModel>();
                    dataReturn = (from dt in unitOfWork.GetRepository<Catalog_Item>().GetAll()
                                  join term in unitOfWork.GetRepository<Taxonomy_Term>().GetAll()
                                  on dt.MappedTermId equals term.TermId
                                  join cm in unitOfWork.GetRepository<Catalog_Master>().GetAll()
                                  on term.VocabularyId equals cm.MappedVocabularyId
                                  where cm.Code == CONSTANT_CATALOG.DON_VI_TINH
                                  orderby dt.Name
                                  select new UnitModel()
                                  {
                                      UnitId = dt.CatalogItemId,
                                      Name = dt.Name,
                                      Code = dt.Code
                                  }).ToList();
                    return new Response<List<UnitModel>>(1, "Success", dataReturn);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<List<UnitModel>>(-1, "Lỗi: " + ex.Message, null);
            }
        }

        /// <summary>
        /// Lấy danh sách loại mặt hàng
        /// </summary>
        /// <returns></returns>
        public Response<List<TypeOfProductModel>> GetDataTypeOfProduct()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    List<TypeOfProductModel> dataReturn = new List<TypeOfProductModel>();
                    dataReturn = (from dt in unitOfWork.GetRepository<Catalog_Item>().GetAll()
                                  join term in unitOfWork.GetRepository<Taxonomy_Term>().GetAll()
                                  on dt.MappedTermId equals term.TermId
                                  join cm in unitOfWork.GetRepository<Catalog_Master>().GetAll()
                                  on term.VocabularyId equals cm.MappedVocabularyId
                                  where cm.Code == CONSTANT_CATALOG.DON_VI_TINH
                                  orderby dt.Name
                                  select new TypeOfProductModel()
                                  {
                                      TypeOfProductId = dt.CatalogItemId,
                                      Name = dt.Name,
                                      Code = dt.Code
                                  }).ToList();
                    return new Response<List<TypeOfProductModel>>(1, "Success", dataReturn);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<List<TypeOfProductModel>>(-1, "Lỗi: " + ex.Message, null);
            }
        }

        /// <summary>
        /// Lấy danh sách mặt hàng
        /// </summary>
        /// <returns></returns>
        public Response<List<ProductModel>> GetDataProducts()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    List<ProductModel> dataReturn = new List<ProductModel>();
                    dataReturn = (from dt in unitOfWork.GetRepository<Product>().GetAll()
                                  orderby dt.Name
                                  select new ProductModel()
                                  {
                                      ProductCode = dt.Code,
                                      ProductId = dt.ProductId,
                                      ProductName = dt.Name,
                                      TypeOfProduct = (dt.TypeOfProduct != null) ? dt.TypeOfProduct.Value : Guid.Empty,
                                      Unit = (dt.Unit != null) ? dt.Unit.Value : Guid.Empty,
                                      VAT = (dt.VATPercent != null) ? Convert.ToInt32(dt.VATPercent.Value) : 0
                                  }).ToList();
                    return new Response<List<ProductModel>>(1, "Success", dataReturn);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<List<ProductModel>>(-1, "Lỗi: " + ex.Message, null);
            }
        }

        /// <summary>
        /// Lấy danh sách dải hóa đơn đã được đăng ký phát hành
        /// </summary>
        /// <returns></returns>
        public Response<List<InvoiceTemplatePublishModel>> GetInvoicePublishTemplate()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    List<InvoiceTemplatePublishModel> dataReturn = new List<InvoiceTemplatePublishModel>();
                    dataReturn = (from dt in unitOfWork.GetRepository<Publish_Invoice_Template>().GetAll()
                                  join temp in unitOfWork.GetRepository<InvoiceTemplate>().GetAll()
                                  on dt.InvoiceTemplateId equals temp.InvoiceTemplateId
                                  orderby dt.InvoiceTemplateId
                                  select new InvoiceTemplatePublishModel()
                                  {

                                      CompanyId = dt.CompanyId.Value,
                                      InvoiceTemplateId = dt.InvoiceTemplateId.Value,
                                      InvoiceCategoryId = dt.InvoiceCategoryId.Value,
                                      InvoiceSeries = dt.InvoiceSeries,
                                      InvoiceTemplateCode = temp.TemplateName
                                  }).ToList();
                    return new Response<List<InvoiceTemplatePublishModel>>(1, "Success", dataReturn);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<List<InvoiceTemplatePublishModel>>(-1, "Lỗi: " + ex.Message, null);
            }
        }

        /// <summary>
        /// Bulk insert
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dataTable"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        public void BulkInsertTable(string tableName, DataTable dataTable, SqlConnection connection, SqlTransaction transaction)
        {
            using (var sqlBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction)) // Lock the table
            {
                sqlBulkCopy.DestinationTableName = tableName;
                sqlBulkCopy.WriteToServer(dataTable);
            }
        }

        /// <summary>
        /// Get table schema
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public DataTable GetSchemaInfo(string tableName, SqlConnection connection, SqlTransaction transaction)
        {
            DataTable dataTable = new DataTable();
            using (SqlCommand selectSchemaCommand = connection.CreateCommand())
            {
                selectSchemaCommand.CommandText = string.Format("set fmtonly on; select * from {0}", tableName);
                selectSchemaCommand.Transaction = transaction;

                using (var adapter = new SqlDataAdapter(selectSchemaCommand))
                {
                    adapter.FillSchema(dataTable, SchemaType.Source);
                }
            }
            return dataTable;
        }
    }
}
