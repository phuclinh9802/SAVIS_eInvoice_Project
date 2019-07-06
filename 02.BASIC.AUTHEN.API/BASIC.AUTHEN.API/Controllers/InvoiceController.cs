using BASIC.AUTHEN.API;
using BASIC.AUTHEN.BUSSINESS;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Configuration;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using BASIC.AUTHEN.DATA;
using System.Text;

namespace ERP.API.Controllers
{
    /// <summary>
    /// 01. INVOICE API || Các API xử lý dữ liệu hóa đơn
    /// </summary>
    [BasicAuthentication]
    public class InvoiceController : ApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// PushInvoice || Đẩy hóa đơn vào hệ thống HĐĐT
        /// </summary>
        ///<param name="dataBase64"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/pushinvoice")]
        [EnableCors(origins: "*", headers: "*", methods: "POST")]
        public Response<DataOutputModel> PushInvoice([FromBody] string dataBase64)
        {
            try
            {
                IEnumerable<string> headerValues = Request.Headers.GetValues("AppId");
                string appId = headerValues.FirstOrDefault();
                DbInvoiceHandler handler = new DbInvoiceHandler();
                if (string.IsNullOrWhiteSpace(dataBase64))
                    return new Response<DataOutputModel>(0, "Lỗi: Dữ liệu đầu vào trống", null);
                string jsonData = Utils.Base64Decode(dataBase64);
                var catalog = handler.PushInvoice(jsonData, appId);
                return catalog;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<DataOutputModel>(0, ex.Message, null);
            }
        }

        #region Hiện tại không cần dùng
        /*
        /// <summary>
        /// CreatedInvoice || Lưu hóa đơn vào hệ thống HĐĐT
        /// </summary>
        ///<param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/createdinvoice")]
        [EnableCors(origins: "*", headers: "*", methods: "POST")]
        public Response<DataOutputModel> CreatedInvoice([FromBody] InvoiceModel data)
        {
            try
            {
                IEnumerable<string> headerValues = Request.Headers.GetValues("AppId");
                string appId = headerValues.FirstOrDefault();
                DbInvoiceHandler handler = new DbInvoiceHandler();
                var catalog = handler.CreatedInvoice(data, appId);
                return catalog;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<DataOutputModel>(0, ex.Message, null);
            }            
        }

        /// <summary>
        /// UpdateInvoice || Chỉnh sửa thông tin hóa đơn chưa phát hành vào hệ thống HĐĐT
        /// </summary>
        ///<param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/updateinvoice")]
        [EnableCors(origins: "*", headers: "*", methods: "POST")]
        public Response<DataOutputModel> UpdateInvoice([FromBody] InvoiceModel data)
        {
            try
            {
                IEnumerable<string> headerValues = Request.Headers.GetValues("AppId");
                string appId = headerValues.FirstOrDefault();
                DbInvoiceHandler handler = new DbInvoiceHandler();
                var catalog = handler.UpdateInvoice(data, appId);
                return catalog;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<DataOutputModel>(0, ex.Message, null);
            }
        }
        */
        #endregion

        /// <summary>
        /// DeleteInvoice || Xóa hóa đơn chưa phát hành hoặc hủy hóa đơn đã phát hành
        /// </summary>
        ///<param name="dataBase64"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/deleteinvoice/{type}")]
        [EnableCors(origins: "*", headers: "*", methods: "POST")]
        public Response<List<DataOutputModel>> DeleteInvoice([FromBody] string dataBase64, int type)
        {
            try
            {
                IEnumerable<string> headerValues = Request.Headers.GetValues("AppId");
                string appId = headerValues.FirstOrDefault();
                DbInvoiceHandler handler = new DbInvoiceHandler();
                if (string.IsNullOrWhiteSpace(dataBase64))
                    return new Response<List<DataOutputModel>>(0, "Lỗi: Dữ liệu đầu vào trống", null);
                string jsonData = Utils.Base64Decode(dataBase64);
                List<string> value = JsonConvert.DeserializeObject<List<string>>(jsonData);
                if (type == 0)//Xóa hóa đơn chưa phát hành
                {
                    var catalog = handler.DeleteInvoice(value, appId);
                    return catalog;
                }
                else if (type == 1)//Hủy hóa đơn đã phát hành
                {
                    var catalog = handler.CancelInvoice(value, appId);
                    return catalog;
                }
                else
                {
                    return new Response<List<DataOutputModel>>(0, "Lỗi: Không xác định loại xử lý", null);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<List<DataOutputModel>>(0, ex.Message, null);
            }
        }

        /// <summary>
        /// GetInvoiceNumber || Lấy thông tin số hóa đơn từ hệ thống HĐĐT
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/getinvoicenumber")]
        [EnableCors(origins: "*", headers: "*", methods: "POST")]
        public Response<List<DataOutputStatusPublishModel>> GetInvoiceNumber([FromBody] string dataBase64)
        {
            try
            {
                IEnumerable<string> headerValues = Request.Headers.GetValues("AppId");
                string appId = headerValues.FirstOrDefault();
                DbInvoiceHandler handler = new DbInvoiceHandler();
                if (string.IsNullOrWhiteSpace(dataBase64))
                    return new Response<List<DataOutputStatusPublishModel>>(0, "Lỗi: Dữ liệu đầu vào trống", null);
                string jsonData = Utils.Base64Decode(dataBase64);
                List<string> value = JsonConvert.DeserializeObject<List<string>>(jsonData);
                var catalog = handler.GetInvoiceNumber(appId, value);
                return catalog;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<List<DataOutputStatusPublishModel>>(0, ex.Message, null);
            }
        }

        #region Tạm thời không dùng
        /*
        /// <summary>
        /// UpdateGiveStatus || Cập nhật trạng thái PMKT đã nhận được hóa đơn phát hành khi gọi dịch vụ GetInvoiceNumber
        /// </summary>
        ///<param name="dataBase64"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/updategivestatus")]
        [EnableCors(origins: "*", headers: "*", methods: "POST")]
        public Response<bool> UpdateGiveStatus([FromBody] string dataBase64)
        {
            try
            {
                IEnumerable<string> headerValues = Request.Headers.GetValues("AppId");
                string appId = headerValues.FirstOrDefault();
                DbInvoiceHandler handler = new DbInvoiceHandler();
                if (string.IsNullOrWhiteSpace(dataBase64))
                    return new Response<bool>(0, "Lỗi: Dữ liệu đầu vào trống", false);
                string jsonData = Utils.Base64Decode(dataBase64);
                List<string> value = JsonConvert.DeserializeObject<List<string>>(jsonData);
                var catalog = handler.UpdateGiveStatus(value, appId);
                return catalog;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<bool>(0, ex.Message, false);
            }
        }
        */
        #endregion

        /// <summary>
        /// ChangeInvoice || Xử lý hóa đơn: Điều chỉnh tăng, điều chỉnh giảm, điều chỉnh thông tin, thay thế hóa đơn, hủy bỏ hóa đơn
        /// </summary>
        ///<param name="dataBase64"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/changeinvoice")]
        [EnableCors(origins: "*", headers: "*", methods: "POST")]
        public Response<DataOutputModel> ChangeInvoice([FromBody] string dataBase64)
        {
            try
            {
                IEnumerable<string> headerValues = Request.Headers.GetValues("AppId");
                string appId = headerValues.FirstOrDefault();
                DbInvoiceHandler handler = new DbInvoiceHandler();
                if (string.IsNullOrWhiteSpace(dataBase64))
                    return new Response<DataOutputModel>(0, "Lỗi: Dữ liệu đầu vào trống", null);
                string jsonData = Utils.Base64Decode(dataBase64);
                ChangeInvoiceModel dataInput = JsonConvert.DeserializeObject<ChangeInvoiceModel>(jsonData);
                var catalog = handler.ChangeInvoice(dataInput, appId);
                return catalog;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<DataOutputModel>(0, ex.Message, null);
            }
        }

        /// <summary>
        /// GetInvoicePdfUri || Lấy file PDF cho hóa đơn
        /// </summary>
        /// <param name="dataBase64"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/getinvoicepdfuri")]
        [EnableCors(origins: "*", headers: "*", methods: "POST")]
        public async Task<Response<string>> GetInvoicePdfUri([FromBody] string dataBase64)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    IEnumerable<string> headerValues = Request.Headers.GetValues("AppId");
                    string appId = headerValues.FirstOrDefault();
                    DbInvoiceHandler handler = new DbInvoiceHandler();
                    if (string.IsNullOrWhiteSpace(dataBase64))
                        return new Response<string>(0, "Lỗi: Dữ liệu đầu vào trống", null);
                    string jsonData = Utils.Base64Decode(dataBase64);
                    InvoicePdfInputModel value = JsonConvert.DeserializeObject<InvoicePdfInputModel>(jsonData);
                    if (string.IsNullOrWhiteSpace(value.IdInvoicePMKT))
                        return new Response<string>(0, "Lỗi IdInvoicePMKT trống", null);
                    List<Guid> dataPost = new List<Guid>();
                    List<string> listId = value.IdInvoicePMKT.Split(';').ToList();
                    foreach (var idOther in listId)
                    {
                        if (!string.IsNullOrWhiteSpace(idOther))
                        {
                            var invoiceConnect = unitOfWork.GetRepository<ConnectWithInvoice>().GetMany(x => x.OtherId == idOther && x.ConnectApplicationId == new Guid(appId)).FirstOrDefault();
                            if (invoiceConnect == null)
                                return new Response<string>(0, "Lỗi không tìm thấy thông tin hóa đơn mã: " + idOther, null);
                            dataPost.Add(invoiceConnect.InvoiceId);
                        }
                    }
                    using (var client = new HttpClient())
                    {
                        string _serviceValidate = ConfigurationManager.AppSettings["INVOICE_API_URL"];
                        string uri = _serviceValidate + "api/invoice/uri/pdf/" + value.Type;
                        client.BaseAddress = new Uri(uri);
                        var jsonString = JsonConvert.SerializeObject(dataPost);
                        var content = new StringContent(jsonString, Encoding.UTF8);
                        content.Headers.ContentType.MediaType = "application/json";
                        content.Headers.ContentType.CharSet = "UTF-8";
                        var response = client.PostAsync(uri, content).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            var messageContent = response.Content.ReadAsStringAsync().Result;
                            var statusCode = response.StatusCode;
                            var result = JsonConvert.DeserializeObject<ReviceDataFromCoreModel>(messageContent);
                            if (result.Status == 1)
                                return new Response<string>(1, "Success", result.Data);
                            else
                                return new Response<string>(0, result.Message, "");
                        }
                        else
                        {
                            return new Response<string> (-1, "Không kết nối được API Core", null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return new Response<string>(0, ex.Message, null);
            }
        }
    }
}