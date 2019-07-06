using BASIC.AUTHEN.API;
using BASIC.AUTHEN.BUSSINESS;
using log4net;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ERP.API.Controllers
{
    /// <summary>
    /// 01. COMMON API || Các API lấy dữ liệu dùng chung
    /// </summary>
    [BasicAuthentication]
    public class CommonController : ApiController
    {
        #region Không dùng đến thời điểm hiện tại
        /*Các API hiện tại không dùng
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// GetPaymentMethod || Lấy dữ liệu hình thức thanh toán
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getpaymentmethod")]
        [EnableCors(origins: "*", headers: "*", methods: "GET")]
        public Response<List<PaymentMethodModel>> GetPaymentMethod()
        {
            try
            {
                DbCommonHandler handler = new DbCommonHandler();
                var catalog = handler.GetPaymentMethod();
                return catalog;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<List<PaymentMethodModel>>(0, ex.Message, null);
            }            
        }
                
        /// <summary>
        /// GetCustomer || Lấy dữ liệu khách hàng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getcustomer")]
        [EnableCors(origins: "*", headers: "*", methods: "GET")]
        public Response<List<CustomerModel>> GetCustomer()
        {
            try
            {
                DbCommonHandler handler = new DbCommonHandler();
                var catalog = handler.GetCustomer();
                return catalog;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<List<CustomerModel>>(0, ex.Message, null);
            }
        }

        /// <summary>
        /// GetDataUnit || Lấy dữ liệu danh sách đơn vị tính
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getdataunit")]
        [EnableCors(origins: "*", headers: "*", methods: "GET")]
        public Response<List<UnitModel>> GetDataUnit()
        {
            try
            {
                DbCommonHandler handler = new DbCommonHandler();
                var catalog = handler.GetDataUnit();
                return catalog;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<List<UnitModel>>(0, ex.Message, null);
            }
        }

        /// <summary>
        /// GetDataTypeOfProduct || Lấy dữ liệu danh sách loại mặt hàng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getdatatypeofproduct")]
        [EnableCors(origins: "*", headers: "*", methods: "GET")]
        public Response<List<TypeOfProductModel>> GetDataTypeOfProduct()
        {
            try
            {
                DbCommonHandler handler = new DbCommonHandler();
                var catalog = handler.GetDataTypeOfProduct();
                return catalog;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<List<TypeOfProductModel>>(0, ex.Message, null);
            }
        }

        /// <summary>
        /// GetDataProducts || Lấy dữ liệu danh sách mặt hàng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getdataproduct")]
        [EnableCors(origins: "*", headers: "*", methods: "GET")]
        public Response<List<ProductModel>> GetDataProducts()
        {
            try
            {
                DbCommonHandler handler = new DbCommonHandler();
                var catalog = handler.GetDataProducts();
                return catalog;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<List<ProductModel>>(0, ex.Message, null);
            }
        }

        /// <summary>
        /// GetInvoicePublishTemplate || Lấy dữ liệu danh sách dải hóa đơn đã được đăng ký để phát hành
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getinvoicepublishtemplate")]
        [EnableCors(origins: "*", headers: "*", methods: "GET")]
        public Response<List<InvoiceTemplatePublishModel>> GetInvoicePublishTemplate()
        {
            try
            {
                DbCommonHandler handler = new DbCommonHandler();
                var catalog = handler.GetInvoicePublishTemplate();
                return catalog;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<List<InvoiceTemplatePublishModel>>(0, ex.Message, null);
            }
        }
        */
        #endregion
    }
}