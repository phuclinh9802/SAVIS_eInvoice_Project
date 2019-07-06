using BASIC.AUTHEN.API;
using BASIC.AUTHEN.BUSSINESS;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ERP.API.Controllers
{
    /// <summary>
    /// 01. Synchronized API || Các API đồng bộ dữ liệu
    /// </summary>
    [BasicAuthentication]
    public class SynchronizedController : ApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// SynchronizedProduct || Đồng bộ dữ liệu mặt hàng từ PMKT sang HĐĐT
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/syncproduct")]
        [EnableCors(origins: "*", headers: "*", methods: "POST")]
        public Response<List<ProductSyncOutputModel>> SynchronizedProduct([FromBody] string dataBase64)
        {
            try
            {
                DbSynchronizedHandler handler = new DbSynchronizedHandler();
                if (string.IsNullOrWhiteSpace(dataBase64))
                    return new Response<List<ProductSyncOutputModel>>(0, "Lỗi: Dữ liệu đầu vào trống", null);
                string jsonData = Utils.Base64Decode(dataBase64);
                ProductInputDataModel value = JsonConvert.DeserializeObject<ProductInputDataModel>(jsonData);
                var catalog = handler.SynchronizedProduct(value);
                return catalog;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<List<ProductSyncOutputModel>>(0, ex.Message, null);
            }
        }

        /// <summary>
        /// SynchronizedCustomer || Đồng bộ dữ liệu khách hàng từ PMKT sang HĐĐT
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/synccustomer")]
        [EnableCors(origins: "*", headers: "*", methods: "POST")]
        public Response<List<CustomerSyncOutputModel>> SynchronizedCustomer([FromBody]string dataBase64)
        {
            try
            {
                DbSynchronizedHandler handler = new DbSynchronizedHandler();
                if (string.IsNullOrWhiteSpace(dataBase64))
                    return new Response<List<CustomerSyncOutputModel>>(0, "Lỗi: Dữ liệu đầu vào trống", null);
                string jsonData = Utils.Base64Decode(dataBase64);
                CustomerInputDataModel value = JsonConvert.DeserializeObject<CustomerInputDataModel>(jsonData);
                var catalog = handler.SynchronizedCustomer(value);
                return catalog;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<List<CustomerSyncOutputModel>>(0, ex.Message, null);
            }
        }

        /// <summary>
        /// UpdateCustomerInfo || Cập nhật dữ liệu khách hàng chỉnh sửa thông tin từ PMKT sang HĐĐT
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/updatecustomer")]
        [EnableCors(origins: "*", headers: "*", methods: "POST")]
        public Response<CustomerSyncOutputModel> UpdateCustomerInfo([FromBody]string dataBase64)
        {
            try
            {
                DbSynchronizedHandler handler = new DbSynchronizedHandler();
                if (string.IsNullOrWhiteSpace(dataBase64))
                    return new Response<CustomerSyncOutputModel>(0, "Lỗi: Dữ liệu đầu vào trống", null);
                string jsonData = Utils.Base64Decode(dataBase64);
                CustomerInputModel value = JsonConvert.DeserializeObject<CustomerInputModel>(jsonData);
                var catalog = handler.UpdateCustomerInfo(value);
                return catalog;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<CustomerSyncOutputModel>(0, ex.Message, null);
            }
        }

        /// <summary>
        /// UpdateProductInfo || Cập nhật dữ liệu mặt hàng chỉnh sửa thông tin từ PMKT sang HĐĐT
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/updateproduct")]
        [EnableCors(origins: "*", headers: "*", methods: "POST")]
        public Response<ProductSyncOutputModel> UpdateProductInfo([FromBody]string dataBase64)
        {
            try
            {
                DbSynchronizedHandler handler = new DbSynchronizedHandler();
                if (string.IsNullOrWhiteSpace(dataBase64))
                    return new Response<ProductSyncOutputModel>(0, "Lỗi: Dữ liệu đầu vào trống", null);
                string jsonData = Utils.Base64Decode(dataBase64);
                ProductInputModel value = JsonConvert.DeserializeObject<ProductInputModel>(jsonData);
                var catalog = handler.UpdateProductInfo(value);
                return catalog;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<ProductSyncOutputModel>(0, ex.Message, null);
            }
        }
    }
}