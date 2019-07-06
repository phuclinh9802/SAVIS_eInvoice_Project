using BASIC.AUTHEN.DATA;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace BASIC.AUTHEN.API
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    Guid appId = Guid.Empty;
                    try
                    {
                        #region Kiểm tra thông tin AppId
                        IEnumerable<string> headerValues = actionContext.Request.Headers.GetValues("AppId");
                        appId = new Guid(headerValues.FirstOrDefault());
                        var checkAppInDb = unitOfWork.GetRepository<ConnectApplication>().GetMany(x => x.ConnectApplicationId == appId);
                        //AppId chưa được đăng ký trong CSDL
                        if (checkAppInDb == null)
                            HandleNotfoundAppInDB(actionContext);
                        #endregion

                        #region Kiểm tra thông tin Basic Authentication
                        var authHeader = actionContext.Request.Headers.Authorization;
                        if (authHeader != null)
                        {
                            var authenticationToken = actionContext.Request.Headers.Authorization.Parameter;
                            var decodedAuthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationToken));
                            var usernamePasswordArray = decodedAuthenticationToken.Split(':');
                            var userName = usernamePasswordArray[0];
                            var password = usernamePasswordArray[1];

                            bool isValid = false;

                            var db = unitOfWork.DataContext;
                            var checkAuthen = unitOfWork.GetRepository<ConnectApplication>().GetMany(x => x.UserName == userName
                            && x.Password == password && x.IsActive == true);

                            if (checkAuthen.FirstOrDefault() != null)
                                isValid = true;

                            if (isValid)
                            {
                                if (checkAuthen.FirstOrDefault().ConnectApplicationId == appId)
                                {
                                    var principal = new GenericPrincipal(new GenericIdentity(userName), null);
                                    Thread.CurrentPrincipal = principal;
                                }
                                else
                                {
                                    //AppId và thông tin xác thực không cùng 1 bản ghi trong cơ sở dữ liệu
                                    HandleAppNotMappAuthen(actionContext);
                                }
                            }
                            else
                            {
                                //Không tìm thấy thông tin xác thực trong cơ sở dữ liệu
                                HandleUnathorizedInDB(actionContext);
                            }
                        }
                        else
                        {
                            //Không tìm thấy thông tin xác thực trên Header
                            HandleUnathorized(actionContext);
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        logger.Debug(ex.Message);
                        //Không có thông tin AppId trên header
                        HandleNotfoundApp(actionContext);
                    }                    
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                //Lỗi ngoại lệ
                HandleException(actionContext, ex.Message);
            }
        }

        private static void HandleUnathorized(HttpActionContext actionContext)
        {
            actionContext.Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = new StringContent("Không thể xác thực ứng dụng (Không tìm thấy thông tin Authorization trên Header")
            };
        }

        private static void HandleUnathorizedInDB(HttpActionContext actionContext)
        {
            actionContext.Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = new StringContent("Không tìm thấy thông tin xác thực trong cơ sở dữ liệu")
            };
        }

        private static void HandleNotfoundApp(HttpActionContext actionContext)
        {
            actionContext.Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("Không tìm thấy thông tin AppId trên header")
            };
        }

        private static void HandleNotfoundAppInDB(HttpActionContext actionContext)
        {
            actionContext.Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("Không tìm thấy thông tin AppId trong cơ sở dữ liệu")
            };
        }

        private static void HandleAppNotMappAuthen(HttpActionContext actionContext)
        {
            actionContext.Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("Không thể xác thực thông tin với ứng dụng này (App not map Authorization)")
            };
        }

        private static void HandleException(HttpActionContext actionContext, string mess)
        {
            actionContext.Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.ExpectationFailed,
                Content = new StringContent(mess)
            };
        }
    }
}