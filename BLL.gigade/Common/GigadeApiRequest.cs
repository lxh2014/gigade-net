using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using System.Configuration;
using BLL.gigade.Mgr;

namespace BLL.gigade.Common
{
    public class GigadeApiRequest
    {
        
       // public event System.Action ServerError;

        static GigadeApiClientSettings Settings;
        public GigadeApiRequest()
        {
            string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//XML的設置
            string path = HttpContext.Current.Server.MapPath(xmlPath);
            SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
            string apiServer = _siteConfigMgr.GetConfigByName("APIServer").Value;
            Settings = new GigadeApiClientSettings(apiServer, Language.ZhTw, "8d7d4d7fc27111e483cffcaa14733140", "8d801aa0c27111e483cffcaa147331408d840b64c27111e483cffcaa14733140");
            //GigadeApiRequest request = new GigadeApiRequest(Settings);
        }
        public GigadeApiRequest(string apiServer)
        {

            Settings = new GigadeApiClientSettings(apiServer, Language.ZhTw, "8d7d4d7fc27111e483cffcaa14733140", "8d801aa0c27111e483cffcaa147331408d840b64c27111e483cffcaa14733140");
            //GigadeApiRequest request = new GigadeApiRequest(Settings);
        }
        public GigadeApiRequest(GigadeApiClientSettings settings)
        {
            if (Settings == null)
                Settings = settings;
        }
        public ApiResult<TResult> Request<TResult>(string url)
        {
            return InnerRequest<TResult>(url, "");
        }
        public ApiResult<TResult> Request<T, TResult>(string url, T t)
        {
            string content = JsonConvert.SerializeObject(t);
            return InnerRequest<TResult>(url, content);
        }

        private ApiResult<TResult> InnerRequest<TResult>(string url, string content)
        {
            HttpContent body = null;
            if (string.IsNullOrWhiteSpace(content))
                body = new StringContent("");
            else
                body = new StringContent(content);

            string targetUrl = string.Format("{0}/{1}", Settings.BaseAddress, url);
            body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
           // var t1 = DateTime.UtcNow.ToUnixTimeStamp();
            var t1 =Common.CommonFunction.GetPHPTime(DateTime.Now.ToString());
            var result = ApiResult<TResult>.Get();
            using (var _httpClient = new HttpClient())
            {
                _httpClient.Timeout = Settings.Timeout;
                _httpClient.DefaultRequestHeaders.Add("Accept-Language", Settings.AppectLanguage);
                _httpClient.DefaultRequestHeaders.Add("gigade-appkey", string.Format("{0}:{1}", Settings.AppKey, t1.ToString()));
                _httpClient.DefaultRequestHeaders.Add("gigade-sign", new HashEncrypt().SHA256Encrypt(string.Format("{0}:{1}:{2}", Settings.AppSecret, t1.ToString(), content)));
                _httpClient.DefaultRequestHeaders.Add("device", string.Format("{0}:{1}:{2}:{3}:{4}", Settings.Brand, Settings.Model, Settings.OS, Settings.OsVersion, Settings.ClientVersion));
                _httpClient.DefaultRequestHeaders.Add("client_timezone", Settings.ClientTimezone);
                if (HttpContext.Current != null)
                {
                    var token = HttpContext.Current.Session["AccessToken"];
                    if (token != null)
                    {
                        if (!string.IsNullOrWhiteSpace(token.ToString()))
                        {
                            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.ToString());
                        }
                    }
                }
                var response = _httpClient.PostAsync(targetUrl, body).Result;
                var responseJson = response.Content.ReadAsStringAsync().Result;
                result = JsonToModel<TResult>(responseJson);
            }
            if (!result.success)
            {
                switch (result.errorCode)
                {
                    case ErrorCode.AccessDenied:
                        //if (AccessDenied != null)
                        //    AccessDenied(result.error);
                        break;
                    case ErrorCode.BadClock:
                    case ErrorCode.BadHeader:
                    case ErrorCode.RequestTooOffen:
                        //if (SecurityError != null)
                        //    SecurityError(result.error);
                        break;
                    case ErrorCode.ServerError:
                        //if (ServerError != null)
                        //    ServerError();
                        break;
                }
            }
            return result;
        }
        protected virtual ApiResult<T> JsonToModel<T>(string json)
        {
            return JsonConvert.DeserializeObject<ApiResult<T>>(json);
        }
    }
    #region GigadeApiClientSettings
    public class GigadeApiClientSettings
    {
        public GigadeApiClientSettings(string baseAddress, Language acceptLanguage, string appkey, string appsecret, string clientVersion = "")
        {

            if (string.IsNullOrEmpty(baseAddress))
            {
                BaseAddress = "http://192.168.71.11/";
            }
            else
            {
                BaseAddress = baseAddress;
            }
            
            if (acceptLanguage == Language.EnUs)
                AppectLanguage = "en-us";
            else
            {
                AppectLanguage = "zh-tw";
            }
            AppKey = appkey;
            AppSecret = appsecret;

            Brand = "Microsoft";
            Model = "Asp.net";
            OS = Environment.OSVersion.Platform.ToString();
            OsVersion = Environment.OSVersion.Version.ToString();
            Timeout = TimeSpan.FromMinutes(1);
            ClientVersion = clientVersion;
        }
        public string AppKey { get; private set; }
        public string AppSecret { get; private set; }
        public string AppectLanguage { get; private set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string OS { get; set; }
        public string OsVersion { get; set; }
        public TimeSpan Timeout { get; set; }
        public string BaseAddress { get; private set; }
        public string ClientVersion { get; set; }
        public string ClientTimezone { get; set; }
    }
    public enum ApiServer
    {
        TwDevTest,
        TwOnline,
        WhDevTest,
        Debug
    }
    public enum Language
    {
        EnUs,
        ZhTw
    } 
    #endregion

    

    public class ApiResult<T>
    //where T : class
    {
        /// <summary>
        /// 錯誤代碼
        /// </summary>
        public ErrorCode errorCode { get; set; }
        /// <summary>
        /// 請求及執行是否成功
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// Success为false时，该属性报告遇到的错误
        /// </summary>
        public string error { get; set; }
        /// <summary>
        /// 该属性包含返回的结果
        /// </summary>
        public T result { get; set; }
        public static ApiResult<T> Get()
        {
            return new ApiResult<T>() { success = true };
        }
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public enum ErrorCode
    {
        #region System Error
        /// <summary>
        /// 無
        /// </summary>
        None,
        /// <summary>
        /// 內部服務器錯誤
        /// </summary>
        ServerError,
        /// <summary>
        /// 存取被拒
        /// </summary>
        AccessDenied,
        /// <summary>
        /// 客戶端時間和api不同步(差別>120秒)
        /// </summary>
        BadClock,
        /// <summary>
        /// 請求過於頻繁
        /// </summary>
        RequestTooOffen,
        /// <summary>
        /// 模型驗證錯誤(傳遞給api的參數存在錯誤)
        /// </summary>
        ModelValidError,
        #endregion

        /// <summary>
        /// 用戶不存在
        /// </summary>
        UserNotFound,
        /// <summary>
        /// 收藏記錄不存在
        /// </summary>
        FavoriteHistoryNotFound,
        /// <summary>
        /// 瀏覽記錄不存在
        /// </summary>
        BrowsingHistoryNotFound,
        /// <summary>
        /// 郵箱已被使用
        /// </summary>
        EmailBeenUsed,
        /// <summary>
        /// 登入信息無效
        /// </summary>
        LoginInfoIllegal,
        /// <summary>
        /// 商品無效
        /// </summary>
        ProductIllegal,
        /// <summary>
        /// http header 錯誤
        /// </summary>
        BadHeader,
        /// <summary>
        /// 訂單未找到
        /// </summary>
        OrderNotFound,
        /// <summary>
        /// 取消訂單時,該訂單狀態不允許執行取消操作
        /// </summary>
        CancelWithWrongStatus,
        /// <summary>
        /// Facebook id未匹配到用戶
        /// </summary>
        FacebookMisMatch,
        /// <summary>
        /// 贈品兌換失敗
        /// </summary>
        ExchangeGiftFailed,
        /// <summary>
        /// 提交答案失敗
        /// </summary>
        AnswerFailed,
        /// <summary>
        /// fb_id被使用
        /// </summary>
        FbidBeenUsed,
        /// <summary>
        /// 操作用戶收貨地址時出錯
        /// </summary>
        DeliveryAddressUpdateError,
        /// <summary>
        /// 品牌未找到
        /// </summary>
        BrandNotFound,
        /// <summary>
        /// 商品未找到
        /// </summary>
        ProductNotFound
    }

    /// <summary>
    /// 登入返回結果
    /// </summary>
    
    public class UserToken
    {
        /// <summary>
        /// 令牌值
        /// </summary>
        public string user_token { get; set; }
        /// <summary>
        /// 過期時間
        /// </summary>
        public long expired { get; set; }
    }

}
