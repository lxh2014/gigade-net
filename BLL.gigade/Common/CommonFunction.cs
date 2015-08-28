using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;
using BLL.gigade.Mgr;
using BLL.gigade.Model;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Attributes;
using System.Net.Mail;
using System.Net;

namespace BLL.gigade.Common
{
    public class CommonFunction
    {
        /// <summary>
        /// 獲取PHP时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetPHPTime()
        {
            DateTime _datetime = new DateTime(1970, 1, 1);
            return (DateTime.Now.Ticks - _datetime.Ticks) / 10000000 - 8 * 60 * 60;
        }

        /// <summary>
        /// 獲取PHP时间戳
        /// </summary>
        /// <param name="timeStr"></param>
        /// <returns></returns>
        public static long GetPHPTime(string timeStr)
        {
            try
            {
                DateTime _datetime = new DateTime(1970, 1, 1);
                DateTime _tTime = Convert.ToDateTime(timeStr);
                return (_tTime.Ticks - _datetime.Ticks) / 10000000 - 8 * 60 * 60;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 將PHP時間轉換成ASP.NET時間
        /// </summary>
        /// <param name="phpTime">PHP時間戳</param>
        /// <returns></returns>
        public static DateTime GetNetTime(long phpTime)
        {
            DateTime _datetime = new DateTime(1970, 1, 1);
            long ss = (phpTime + (8 * 60 * 60)) * 10000000 + _datetime.Ticks;
            DateTime dt = new DateTime(ss);
            return dt;
        }

        /// <summary>
        /// 將net時間轉化為字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DateTimeToString(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string GetEventId(string type, string id)
        {
            string sResult = type;
            if (id.Length < 6)
            {
                for (int i = 0; i < 6 - id.Length; i++)
                {
                    sResult += "0";
                }

            }
            sResult += id;
            return sResult;
        }

        /// <summary>
        /// 獲取客戶端IP
        /// </summary>
        /// <returns></returns>
        public static string GetClientIP()
        {
            string result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == result || result == string.Empty)
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_APPR"];
            }
            if (null == result || result == string.Empty)
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }
            return result;
        }

        /// <summary>
        /// 获取客户端IP
        /// </summary>
        /// <returns></returns>
        public static string GetClientIPNew()
        {
            if (HttpContext.Current == null) return "localhost";
            HttpRequest request = HttpContext.Current.Request;

            //获取客户端真实IP
            string clientIp = request.Headers["CDN-SRC-IP"];

            if (string.IsNullOrEmpty(clientIp))
            {
                if (!string.IsNullOrEmpty(request.ServerVariables["HTTP_X_FORWARDED_FOR"]))
                {
                    clientIp = request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(',')[0].ToString();
                }
            }

            if (string.IsNullOrEmpty(clientIp))
            {
                clientIp = request.ServerVariables["REMOTE_ADDR"];
            }

            if (string.IsNullOrEmpty(clientIp))
            {
                clientIp = request.UserHostAddress;
            }

            return clientIp;
        }


        public static string GetIP4Address(string s)
        {
            string IP4Address = String.Empty;

            foreach (IPAddress IPA in Dns.GetHostAddresses(s))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }

            }

            if (IP4Address != String.Empty)
            {
                return IP4Address;
            }

            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            return IP4Address;
        }

        //public static string Hash(string strType, string strValue)
        //{
        //    return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(strValue, "SHA1");
        //}

        /// <summary>
        /// 產生亂數字串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Generate_Rand_String(int length)
        {
            char[] aChars = { '2', '3', '4', '5', '6', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y' };
            int nMax_Chars = aChars.Count() - 1;
            int nGet_Length = 8;

            if (length != 0)
            {
                nGet_Length = length;
            }

            string sRand_String = "";
            Random rand = new Random();
            for (int i = 0; i < nGet_Length; i++)
            {
                sRand_String += aChars[rand.Next(0, nMax_Chars)];
            }

            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sRand_String, "md5");

        }


        /// <summary>
        /// 設置清空Cookie
        /// </summary>
        /// <param name="CookieName">Cookie名稱</param>
        /// <param name="ItemName">Cookie項名稱</param>
        /// <param name="ItemValue">Cookie項值</param>
        /// <param name="IsRemember">是否記住</param>
        /// <param name="Time">Cookie有效天數</param>
        public static void Cookie_Set(string CookieName, string ItemName, string ItemValue, string IsRemember, int CookieExpireTime)
        {
            HttpCookie cookies = HttpContext.Current.Request.Cookies[CookieName];
            if (cookies == null)
            {
                if (IsRemember == "true")
                {
                    cookies = new HttpCookie(CookieName);
                    cookies.Values.Add(ItemName, ItemValue);
                    cookies.Expires = System.DateTime.Now.AddDays(CookieExpireTime);
                    HttpContext.Current.Response.Cookies.Add(cookies);
                }
            }
            else
            {
                if (IsRemember == "false")
                {
                    HttpContext.Current.Response.Cookies[CookieName].Expires = DateTime.Now;
                }
                else
                {
                    cookies.Values.Set(ItemName, ItemValue);
                    cookies.Expires = System.DateTime.Now.AddDays(CookieExpireTime);
                    HttpContext.Current.Response.Cookies.Add(cookies);
                }
            }
        }


        /// <summary>
        /// 獲取序列號
        /// </summary>
        /// <param name="serialId">序列ID</param>
        /// <param name="connectionStr">數據庫連接串</param>
        /// <returns>序列Value</returns>
        public string NextSerial(int serialId, string connectionStr)
        {
            try
            {
                ISerialImplMgr serialMgr = new SerialMgr(connectionStr);
                Serial serial = serialMgr.GetSerialById(serialId);
                if (serial != null)
                {
                    serial.Serial_Value = serial.Serial_Value + 1;
                    if (serialMgr.Update(serial) > 0)
                    {
                        return serial.Serial_Value.ToString();
                    }
                }

            }
            catch (Exception)
            {
            }
            return "";
        }


        /// <summary>
        /// 轉義字符串
        /// </summary>
        /// <param name="value">原始串</param>
        /// <returns></returns>
        public static string StringTransfer(string value)
        {
            return value.Replace("'", "''");
        }

        public static string jsStringTransfer(string value)
        {
            return value.Replace("'", "\'");
        }



        /// <summary>
        /// 排序Product_item_map中的item_id
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string Rank_ItemId(string pm)
        {
            string ItemId = pm;
            int ItemCount = ItemId.Split(',').Length;
            string[] NewItemId = new string[ItemCount];
            string ChangItemId;
            for (int i = 0; i < ItemCount; i++)
            {
                NewItemId[i] = ItemId.Split(',')[i];
            }
            for (int j = 0; j < ItemCount; j++)
            {
                for (int k = 0; k < ItemCount - j - 1; k++)
                {
                    if (int.Parse(NewItemId[k]) > int.Parse(NewItemId[k + 1]))
                    {
                        ChangItemId = NewItemId[k];
                        NewItemId[k] = NewItemId[k + 1];
                        NewItemId[k + 1] = ChangItemId;
                    }
                }
            }
            ItemId = "";
            for (int i = 0; i < NewItemId.Count(); i++)
            {
                ItemId += NewItemId[i].ToString();
                if (i != NewItemId.Count() - 1)
                {
                    ItemId += ",";

                }
            }
            return ItemId;
        }

        #region 不足位數字符串補位

        /// <summary>
        /// 不足位數字符串補位
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="str">補位字符</param>
        /// <param name="count">字符總長度</param>
        /// <returns></returns>
        public static string Supply(string source, string str, int count)
        {
            try
            {
                if (!string.IsNullOrEmpty(source) && !string.IsNullOrEmpty(str))
                {
                    while (source.Length < count)
                    {
                        source = str + source;
                    }
                }
                return source;
            }
            catch (Exception ex)
            {
                throw new Exception("CommonFunction.Supply-->" + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// 計算折扣
        /// </summary>
        /// <param name="price">原價</param>
        /// <param name="discount">折扣</param>
        /// <returns>打折後價格</returns>
        public static int ArithmeticalDiscount(int price, int discount)
        {
            return (int)Math.Round(price * discount * 0.01, 0, MidpointRounding.AwayFromZero);//AwayFromZero為中國式的四舍五入
        }

        /// <summary>
        /// 4舍5入
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int Math4Cut5Plus(double num)
        {
            if (num - (int)num >= 0.5)
            {
                return (int)num + 1;
            }
            else
            {
                return (int)num;
            }
        }

        public static string Getserials(int lenght, Random rd)
        {
            char[] i = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            //Random rd = new Random(w);
            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < lenght; j++)
            {
                int rownumber = rd.Next(1, i.Length);
                sb.Append(i[rownumber - 1]);
            }
            return sb.ToString();
        }


        #region 刪除本地上傳的圖片
        /// <summary>
        /// 刪除本地上傳的圖片
        /// </summary>

        public static void DeletePicFile(string imageName)
        {
            if (System.IO.File.Exists(imageName))
            {
                System.IO.File.Delete(imageName);
            }
        }
        #endregion

        /// <summary>
        /// 获取类的属性(DBTableInfo)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static DBTableInfo GetDBInfo<T>()
        {
            Type type = typeof(T);
            return (DBTableInfo)Attribute.GetCustomAttribute(type, typeof(DBTableInfo));
        }

        /// <summary>
        /// 發送郵件
        /// </summary>
        /// <param name="MailFromAddress">發送人地址</param>
        /// <param name="MailFormPwd">密碼</param>
        /// <param name="MailTitle">郵件標題</param>
        /// <param name="MailBody">郵件內容</param>
        /// <param name="MailTo">收件人地址</param>
        /// <returns>發送成功返回true發送失敗返回false</returns>
        public static Boolean SendMail(string MailFromAddress, string MailFormPwd, string MailTitle, string MailBody, string MailTo)
        {
            try
            {
                MailMessage objMail = new MailMessage();
                //發送人地址
                objMail.From = new MailAddress(MailFromAddress);
                //郵件標題
                objMail.Subject = MailTitle;
                //郵件標題編碼
                objMail.SubjectEncoding = System.Text.Encoding.UTF8;
                //邮件内容
                objMail.Body = MailBody;
                //郵件內容編碼
                objMail.BodyEncoding = System.Text.Encoding.UTF8;
                //收件人地址在這裡可以加多個
                objMail.To.Add(MailTo);
                //用SMTP發送郵件的方式
                SmtpClient client = new SmtpClient();
                //用戶名和密碼
                client.Credentials = new System.Net.NetworkCredential(MailFromAddress, MailFormPwd);
                //string StrMail = "163.com";
                //for (int i = 0; i < MailFromAddress.Length; i++)
                //{

                //    if (MailFromAddress.Substring(i, 1).ToString().Trim() == "@")
                //    {
                //        StrMail = MailFromAddress.Substring(i + 1, MailFromAddress.Length - i - 1);
                //    }
                //}
                //服務器名
                //client.Host = "smtp." + StrMail.ToString().Trim();
                client.Host = "192.168.26.1";
                //發送
                client.Send(objMail);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        #region C#发送邮件函数
        /// <summary>
        /// C#发送邮件函数
        /// </summary>
        /// <param name="from">发送者邮箱</param>
        /// <param name="fromer">发送人</param>
        /// <param name="sto">接受者邮箱</param>
        /// <param name="toer">收件人</param>
        /// <param name="Subject">主题</param>
        /// <param name="Body">内容</param>
        /// <param name="file">附件地址</param>
        /// <param name="SMTPHost">smtp服务器</param>
        /// <param name="SMTPuser">邮箱</param>
        /// <param name="SMTPpass">密码</param>
        /// <returns></returns>
        public static bool sendmail(string sfrom, string sfromer, string sto, string stoer, string sSubject, string sBody, string sfile, string sSMTPHost, int sSMTPPort, string sSMTPuser, string sSMTPpass)
        {
            ////设置from和to地址
            MailAddress from = new MailAddress(sfrom, sfromer);
            MailAddress to = new MailAddress(sto, stoer);

            ////创建一个MailMessage对象
            MailMessage oMail = new MailMessage(from, to);
            //// 添加附件
            if (sfile != "")
            {
                oMail.Attachments.Add(new Attachment(sfile));
            }
            ////邮件标题
            oMail.Subject = sSubject;
            ////邮件内容
            //oMail.Body = sBody;
            AlternateView htmlBody = AlternateView.CreateAlternateViewFromString(sBody, null, "text/html");
            oMail.AlternateViews.Add(htmlBody);
            ////邮件格式
            oMail.IsBodyHtml = true;
            ////邮件采用的编码
            oMail.BodyEncoding = System.Text.Encoding.GetEncoding("GB2312");
            ////设置邮件的优先级为高
            oMail.Priority = MailPriority.Normal;
            ////发送邮件
            SmtpClient client = new SmtpClient();
            ////client.UseDefaultCredentials = false;
            client.Host = sSMTPHost;
            client.Port = sSMTPPort;
            client.Credentials = new NetworkCredential(sSMTPuser, sSMTPpass);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            try
            {
                client.Send(oMail);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                ////释放资源
                oMail.Dispose();
            }
        }
        #endregion

        /// <summary>
        /// 根據zipcode查詢詳細的地址 zhangyu 2014/10/08
        /// </summary>
        /// <param name="zipcode"></param>
        /// <returns></returns>
        public static string ZipAddress(string zipcode)
        {
            string zipaddress = string.Empty;
            Dictionary<string, string> address = new Dictionary<string, string> {
    { "100",  "100 臺北市 中正區" },
	{ "103",  "103 臺北市 大同區" },
	{ "104",  "104 臺北市 中山區" },
	{ "105",  "105 臺北市 松山區" },
	{ "106",  "106 臺北市 大安區" },
    { "108",  "108 臺北市 萬華區" },
	{ "110",  "110 臺北市 信義區" },
	{ "111",  "111 臺北市 士林區" },
	{ "112",  "112 臺北市 北投區" },
	{ "114",  "114 臺北市 內湖區" },
	{ "115",  "115 臺北市 南港區" },
	{ "116",  "116 臺北市 文山區" },
	{ "200",  "200 基隆市 仁愛區" },
	{ "201",  "201 基隆市 信義區" },
	{ "202",  "202 基隆市 中正區" },
	{ "203",  "203 基隆市 中山區" },
	{ "204",  "204 基隆市 安樂區" },
	{ "205",  "205 基隆市 暖暖區" },
	{ "206",  "206 基隆市 七堵區" },
	{ "207",  "207 新北市 萬里區" },
	{ "208",  "208 新北市 金山區" },
	{ "220",  "220 新北市 板橋區" },
	{ "221",  "221 新北市 汐止區" },
	{ "222",  "222 新北市 深坑區" },
	{ "223",  "223 新北市 石碇區" },
	{ "224",  "224 新北市 瑞芳區" },
	{ "226",  "226 新北市 平溪區" },
	{ "227",  "227 新北市 雙溪區" },
	{ "228",  "228 新北市 貢寮區" },
	{ "231",  "231 新北市 新店區" },
	{ "232",  "232 新北市 坪林區" },
	{ "233",  "233 新北市 烏來區" },
	{ "234",  "234 新北市 永和區" },
	{ "235",  "235 新北市 中和區" },
	{ "236",  "236 新北市 土城區" },
	{ "237",  "237 新北市 三峽區" },
	{ "238",  "238 新北市 樹林區" },
	{ "239",  "239 新北市 鶯歌區" },
	{ "241",  "241 新北市 三重區" },
	{ "242",  "242 新北市 新莊區" },
	{ "243",  "243 新北市 泰山區" },
	{ "244",  "244 新北市 林口區" },
	{ "247",  "247 新北市 蘆洲區" },
	{ "248",  "248 新北市 五股區" },
	{ "249",  "249 新北市 八里區" },
	{ "251",  "251 新北市 淡水區" },
	{ "252",  "252 新北市 三芝區" },
	{ "253",  "253 新北市 石門區" },
	{ "260",  "260 宜蘭縣 宜蘭市" },
	{ "261",  "261 宜蘭縣 頭城鎮" },
	{ "262",  "262 宜蘭縣 礁溪鄉" },
	{ "263",  "263 宜蘭縣 壯圍鄉" },
	{ "264",  "264 宜蘭縣 員山鄉" },
	{ "265",  "265 宜蘭縣 羅東鎮" },
	{ "266",  "266 宜蘭縣 三星鄉" },
	{ "267",  "267 宜蘭縣 大同鄉" },
	{ "268",  "268 宜蘭縣 五結鄉" },
	{ "269",  "269 宜蘭縣 冬山鄉" },
	{ "270",  "270 宜蘭縣 蘇澳鎮" },
	{ "272",  "272 宜蘭縣 南澳鄉" },
	{ "300",  "300 新竹市" },
	{ "302",  "302 新竹縣 竹北市" },
	{ "303",  "303 新竹縣 湖口鄉" },
	{ "304",  "304 新竹縣 新豐鄉" },
	{ "305",  "305 新竹縣 新埔鎮" },
	{ "306",  "306 新竹縣 關西鎮" },
	{ "307",  "307 新竹縣 芎林鄉" },
	{ "308",  "308 新竹縣 寶山鄉" },
	{ "310",  "310 新竹縣 竹東鎮" },
	{ "311",  "311 新竹縣 五峰鄉" },
	{ "312",  "312 新竹縣 橫山鄉" },
	{ "313",  "313 新竹縣 尖石鄉" },
	{ "314",  "314 新竹縣 北埔鄉" },
	{ "315",  "315 新竹縣 峨眉鄉" },
	{ "320",  "320 桃園縣 中壢市" },
	{ "324",  "324 桃園縣 平鎮市" },
	{ "325",  "325 桃園縣 龍潭鄉" },
	{ "326",  "326 桃園縣 楊梅鎮" },
	{ "327",  "327 桃園縣 新屋鄉" },
	{ "328",  "328 桃園縣 觀音鄉" },
	{ "330",  "330 桃園縣 桃園市" },
	{ "333",  "333 桃園縣 龜山鄉" },
	{ "334",  "334 桃園縣 八德市" },
	{ "335",  "335 桃園縣 大溪鎮" },
	{ "336",  "336 桃園縣 復興鄉" },
	{ "337",  "337 桃園縣 大園鄉" },
	{ "338",  "338 桃園縣 蘆竹鄉" },
	{ "350",  "350 苗栗縣 竹南鎮" },
	{ "351",  "351 苗栗縣 頭份鎮" },
	{ "352",  "352 苗栗縣 三灣鄉" },
	{ "353",  "353 苗栗縣 南庄鄉" },
	{ "354",  "354 苗栗縣 獅潭鄉" },
	{ "356",  "356 苗栗縣 後龍鎮" },
	{ "357",  "357 苗栗縣 通霄鎮" },
	{ "358",  "358 苗栗縣 苑裡鎮" },
	{ "360",  "360 苗栗縣 苗栗市" },
	{ "361",  "361 苗栗縣 造橋鄉" },
	{ "362",  "362 苗栗縣 頭屋鄉" },
	{ "363",  "363 苗栗縣 公館鄉" },
	{ "364",  "364 苗栗縣 大湖鄉" },
	{ "365",  "365 苗栗縣 泰安鄉" },
	{ "366",  "366 苗栗縣 銅鑼鄉" },
	{ "367",  "367 苗栗縣 三義鄉" },
	{ "368",  "368 苗栗縣 西湖鄉" },
	{ "369",  "369 苗栗縣 卓蘭鎮" },
	{ "400",  "400 臺中市 中　區" },
	{ "401",  "401 臺中市 東　區" },
	{ "402",  "402 臺中市 南　區" },
	{ "403",  "403 臺中市 西　區" },
	{ "404",  "404 臺中市 北　區" },
	{ "406",  "406 臺中市 北屯區" },
	{ "407",  "407 臺中市 西屯區" },
	{ "408",  "408 臺中市 南屯區" },
	{ "411",  "411 臺中市 太平區" },
	{ "412",  "412 臺中市 大里區" },
	{ "413",  "413 臺中市 霧峰區" },
	{ "414",  "414 臺中市 烏日區" },
	{ "420",  "420 臺中市 豐原區" },
	{ "421",  "421 臺中市 后里區" },
	{ "422",  "422 臺中市 石岡區" },
	{ "423",  "423 臺中市 東勢區" },
	{ "424",  "424 臺中市 和平區" },
	{ "426",  "426 臺中市 新社區" },
	{ "427",  "427 臺中市 潭子區" },
	{ "428",  "428 臺中市 大雅區" },
	{ "429",  "429 臺中市 神岡區" },
	{ "432",  "432 臺中市 大肚區" },
	{ "433",  "433 臺中市 沙鹿區" },
	{ "434",  "434 臺中市 龍井區" },
	{ "435",  "435 臺中市 梧棲區" },
	{ "436",  "436 臺中市 清水區" },
	{ "437",  "437 臺中市 大甲區" },
	{ "438",  "438 臺中市 外埔區" },
	{ "439",  "439 臺中市 大安區" },
	{ "500",  "500 彰化縣 彰化市" },
	{ "502",  "502 彰化縣 芬園鄉" },
	{ "503",  "503 彰化縣 花壇鄉" },
	{ "504",  "504 彰化縣 秀水鄉" },
	{ "505",  "505 彰化縣 鹿港鎮" },
	{ "506",  "506 彰化縣 福興鄉" },
	{ "507",  "507 彰化縣 線西鄉" },
	{ "508",  "508 彰化縣 和美鎮" },
	{ "509",  "509 彰化縣 伸港鄉" },
	{ "510",  "510 彰化縣 員林鎮" },
	{ "511",  "511 彰化縣 社頭鄉" },
	{ "512",  "512 彰化縣 永靖鄉" },
	{ "513",  "513 彰化縣 埔心鄉" },
	{ "514",  "514 彰化縣 溪湖鎮" },
	{ "515",  "515 彰化縣 大村鄉" },
	{ "516",  "516 彰化縣 埔鹽鄉" },
	{ "520",  "520 彰化縣 田中鎮" },
	{ "521",  "521 彰化縣 北斗鎮" },
	{ "522",  "522 彰化縣 田尾鄉" },
	{ "523",  "523 彰化縣 埤頭鄉" },
	{ "524",  "524 彰化縣 溪州鄉" },
	{ "525",  "525 彰化縣 竹塘鄉" },
	{ "526",  "526 彰化縣 二林鎮" },
	{ "527",  "527 彰化縣 大城鄉" },
	{ "528",  "528 彰化縣 芳苑鄉" },
	{ "530",  "530 彰化縣 二水鄉" },
	{ "540",  "540 南投縣 南投市" },
	{ "541",  "541 南投縣 中寮鄉" },
	{ "542",  "542 南投縣 草屯鎮" },
	{ "544",  "544 南投縣 國姓鄉" },
	{ "545",  "545 南投縣 埔里鎮" },
	{ "546",  "546 南投縣 仁愛鄉" },
	{ "551",  "551 南投縣 名間鄉" },
	{ "552",  "552 南投縣 集集鎮" },
	{ "553",  "553 南投縣 水里鄉" },
	{ "555",  "555 南投縣 魚池鄉" },
	{ "556",  "556 南投縣 信義鄉" },
	{ "557",  "557 南投縣 竹山鎮" },
	{ "558",  "558 南投縣 鹿谷鄉" },
	{ "600",  "600 嘉義市" },
	{ "602",  "602 嘉義縣 番路鄉" },
	{ "603",  "603 嘉義縣 梅山鄉" },
	{ "604",  "604 嘉義縣 竹崎鄉" },
	{ "605",  "605 嘉義縣 阿里山" },
	{ "606",  "606 嘉義縣 中埔鄉" },
	{ "607",  "607 嘉義縣 大埔鄉" },
	{ "608",  "608 嘉義縣 水上鄉" },
	{ "611",  "611 嘉義縣 鹿草鄉" },
	{ "612",  "612 嘉義縣 太保市" },
	{ "613",  "613 嘉義縣 朴子市" },
	{ "614",  "614 嘉義縣 東石鄉" },
	{ "615",  "615 嘉義縣 六腳鄉" },
	{ "616",  "616 嘉義縣 新港鄉" },
	{ "621",  "621 嘉義縣 民雄鄉" },
	{ "622",  "622 嘉義縣 大林鎮" },
	{ "623",  "623 嘉義縣 溪口鄉" },
	{ "624",  "624 嘉義縣 義竹鄉" },
	{ "625",  "625 嘉義縣 布袋鎮" },
	{ "630",  "630 雲林縣 斗南鎮" },
	{ "631",  "631 雲林縣 大埤鄉" },
	{ "632",  "632 雲林縣 虎尾鎮" },
	{ "633",  "633 雲林縣 土庫鎮" },
	{ "634",  "634 雲林縣 褒忠鄉" },
	{ "635",  "635 雲林縣 東勢鄉" },
	{ "636",  "636 雲林縣 臺西鄉" },
	{ "637",  "637 雲林縣 崙背鄉" },
	{ "638",  "638 雲林縣 麥寮鄉" },
	{ "640",  "640 雲林縣 斗六市" },
	{ "643",  "643 雲林縣 林內鄉" },
	{ "646",  "646 雲林縣 古坑鄉" },
	{ "647",  "647 雲林縣 莿桐鄉" },
	{ "648",  "648 雲林縣 西螺鎮" },
	{ "649",  "649 雲林縣 二崙鄉" },
	{ "651",  "651 雲林縣 北港鎮" },
	{ "652",  "652 雲林縣 水林鄉" },
    { "653",  "653 雲林縣 口湖鄉" },
	{ "654",  "654 雲林縣 四湖鄉" },
	{ "655",  "655 雲林縣 元長鄉" },
	{ "700",  "700 臺南市 中西區" },
	{ "701",  "701 臺南市 東　區" },
	{ "702",  "702 臺南市 南　區" },
	{ "704",  "704 臺南市 北　區" },
	{ "708",  "708 臺南市 安平區" },
	{ "709",  "709 臺南市 安南區" },
	{ "710",  "710 臺南市 永康區" },
	{ "711",  "711 臺南市 歸仁區" },
	{ "712",  "712 臺南市 新化區" },
	{ "713",  "713 臺南市 左鎮區" },
	{ "714",  "714 臺南市 玉井區" },
	{ "715",  "715 臺南市 楠西區" },
	{ "716",  "716 臺南市 南化區" },
	{ "717",  "717 臺南市 仁德區" },
	{ "718",  "718 臺南市 關廟區" },
	{ "719",  "719 臺南市 龍崎區" },
	{ "720",  "720 臺南市 官田區" },
	{ "721",  "721 臺南市 麻豆區" },
	{ "722",  "722 臺南市 佳里區" },
	{ "723",  "723 臺南市 西港區" },
    { "724",  "724 臺南市 七股區" },
	{ "725",  "725 臺南市 將軍區" },
	{ "726",  "726 臺南市 學甲區" },
	{ "727",  "727 臺南市 北門區" },
	{ "730",  "730 臺南市 新營區" },
	{ "731",  "731 臺南市 後壁區" },
	{ "732",  "732 臺南市 白河區" },
	{ "733",  "733 臺南市 東山區" },
	{ "734",  "734 臺南市 六甲區" },
	{ "735",  "735 臺南市 下營區" },
	{ "736",  "736 臺南市 柳營區" },
	{ "737",  "737 臺南市 鹽水區" },
	{ "741",  "741 臺南市 善化區" },
	{ "742",  "742 臺南市 大內區" },
	{ "743",  "743 臺南市 山上區" },
	{ "744",  "744 臺南市 新市區" },
	{ "745",  "745 臺南市 安定區" },
	{ "800",  "800 高雄市 新興區" },
	{ "801",  "801 高雄市 前金區" },
	{ "802",  "802 高雄市 苓雅區" },
	{ "803",  "803 高雄市 鹽埕區" },
	{ "804",  "804 高雄市 鼓山區" },
	{ "805",  "805 高雄市 旗津區" },
	{ "806",  "806 高雄市 前鎮區" },
	{ "807",  "807 高雄市 三民區" },
	{ "811",  "811 高雄市 楠梓區" },
	{ "812",  "812 高雄市 小港區" },
	{ "813",  "813 高雄市 左營區" },
	{ "814",  "814 高雄市 仁武區" },
	{ "815",  "815 高雄市 大社區" },
	{ "820",  "820 高雄市 岡山區" },
	{ "821",  "821 高雄市 路竹區" },
	{ "822",  "822 高雄市 阿蓮區" },
	{ "823",  "823 高雄市 田寮區" },
	{ "824",  "824 高雄市 燕巢區" },
	{ "825",  "825 高雄市 橋頭區" },
	{ "826",  "826 高雄市 梓官區" },
	{ "827",  "827 高雄市 彌陀區" },
	{ "828",  "828 高雄市 永安區" },
	{ "829",  "829 高雄市 湖內區" },
	{ "830",  "830 高雄市 鳳山區" },
	{ "831",  "831 高雄市 大寮區" },
	{ "832",  "832 高雄市 林園區" },
	{ "833",  "833 高雄市 鳥松區" },
	{ "840",  "840 高雄市 大樹區" },
	{ "842",  "842 高雄市 旗山區" },
	{ "843",  "843 高雄市 美濃區" },
	{ "844",  "844 高雄市 六龜區" },
	{ "845",  "845 高雄市 內門區" },
	{ "846",  "846 高雄市 杉林區" },
	{ "847",  "847 高雄市 甲仙區" },
	{ "848",  "848 高雄市 桃源區" },
	{ "849",  "849 高雄市 那瑪夏區" },
	{ "851",  "851 高雄市 茂林區" },
	{ "852",  "852 高雄市 茄萣區" },
	{ "880",  "880 澎湖縣 馬公市" },
	{ "881",  "881 澎湖縣 西嶼鄉" },
	{ "882",  "882 澎湖縣 望安鄉" },
	{ "883",  "883 澎湖縣 七美鄉" },
	{ "884",  "884 澎湖縣 白沙鄉" },
	{ "885",  "885 澎湖縣 湖西鄉" },
	{ "900",  "900 屏東縣 屏東市" },
	{ "901",  "901 屏東縣 三地門" },
	{ "902",  "902 屏東縣 霧臺鄉" },
	{ "903",  "903 屏東縣 瑪家鄉" },
	{ "904",  "904 屏東縣 九如鄉" },
	{ "905",  "905 屏東縣 里港鄉" },
	{ "906",  "906 屏東縣 高樹鄉" },
	{ "907",  "907 屏東縣 鹽埔鄉" },
	{ "908",  "908 屏東縣 長治鄉" },
	{ "909",  "909 屏東縣 麟洛鄉" },
	{ "911",  "911 屏東縣 竹田鄉" },
	{ "912",  "912 屏東縣 內埔鄉" },
	{ "913",  "913 屏東縣 萬丹鄉" },
	{ "920",  "920 屏東縣 潮州鎮" },
	{ "921",  "921 屏東縣 泰武鄉" },
	{ "922",  "922 屏東縣 來義鄉" },
	{ "923",  "923 屏東縣 萬巒鄉" },
	{ "924",  "924 屏東縣 崁頂鄉" },
	{ "925",  "925 屏東縣 新埤鄉" },
	{ "926",  "926 屏東縣 南州鄉" },
	{ "927",  "927 屏東縣 林邊鄉" },
	{ "928",  "928 屏東縣 東港鎮" },
	{ "929",  "929 屏東縣 琉球鄉" },
	{ "931",  "931 屏東縣 佳冬鄉" },
	{ "932",  "932 屏東縣 新園鄉" },
	{ "940",  "940 屏東縣 枋寮鄉" },
	{ "941",  "941 屏東縣 枋山鄉" },
	{ "942",  "942 屏東縣 春日鄉" },
	{ "943",  "943 屏東縣 獅子鄉" },
	{ "944",  "944 屏東縣 車城鄉" },
	{ "945",  "945 屏東縣 牡丹鄉" },
	{ "946",  "946 屏東縣 恆春鎮" },
	{ "947",  "947 屏東縣 滿州鄉" },
	{ "950",  "950 臺東縣 臺東市" },
	{ "951",  "951 臺東縣 綠島鄉" },
	{ "952",  "952 臺東縣 蘭嶼鄉" },
	{ "953",  "953 臺東縣 延平鄉" },
	{ "954",  "954 臺東縣 卑南鄉" },
	{ "955",  "955 臺東縣 鹿野鄉" },
	{ "956",  "956 臺東縣 關山鎮" },
	{ "957",  "957 臺東縣 海端鄉" },
	{ "958",  "958 臺東縣 池上鄉" },
	{ "959",  "959 臺東縣 東河鄉" },
	{ "961",  "961 臺東縣 成功鎮" },
	{ "962",  "962 臺東縣 長濱鄉" },
	{ "963",  "963 臺東縣 太麻里" },
	{ "964",  "964 臺東縣 金峰鄉" },
	{ "965",  "965 臺東縣 大武鄉" },
	{ "966",  "966 臺東縣 達仁鄉" },
	{ "970",  "970 花蓮縣 花蓮市" },
	{ "971",  "971 花蓮縣 新城鄉" },
	{ "972",  "972 花蓮縣 秀林鄉" },
	{ "973",  "973 花蓮縣 吉安鄉" },
	{ "974",  "974 花蓮縣 壽豐鄉" },
	{ "975",  "975 花蓮縣 鳳林鎮" },
	{ "976",  "976 花蓮縣 光復鄉" },
	{ "977",  "977 花蓮縣 豐濱鄉" },
	{ "978",  "978 花蓮縣 瑞穗鄉" },
	{ "979",  "979 花蓮縣 萬榮鄉" },
	{ "981",  "981 花蓮縣 玉里鎮" },
	{ "982",  "982 花蓮縣 卓溪鄉" },
	{ "983",  "983 花蓮縣 富里鄉" },
	{ "890",  "890 金門縣 金沙鎮" },
	{ "891",  "891 金門縣 金湖鎮" },
	{ "892",  "892 金門縣 金寧鄉" },
	{ "893",  "893 金門縣 金城鎮" },
	{ "894",  "894 金門縣 烈嶼鄉" },
	{ "896",  "896 金門縣 烏坵鄉" },
	{ "209",  "209 連江縣 南竿鄉" },
	{ "210",  "210 連江縣 北竿鄉" },
	{ "211",  "211 連江縣 莒光鄉" },
	{ "212",  "212 連江縣 東引鄉" },
	{ "817",  "817 南海諸島 東沙" },
	{ "819",  "819 南海諸島 南沙" },
	{ "290",  "290 釣魚台列嶼" } };
            if (address.TryGetValue(zipcode.Trim(), out zipaddress))
            {
                return zipaddress;
            }
            else
            {
                return "";
            }

        }
        /// <summary>
        /// 用戶email刪除時候
        /// </summary>
        /// <param name="lenght"></param>
        /// <param name="rd"></param>
        /// <returns></returns>
        public static string Getdeleteemail(int lenght, Random rd)
        {
            char[] i = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < lenght; j++)
            {
                int rownumber = rd.Next(1, i.Length);
                sb.Append(i[rownumber - 1]);
            }
            sb.Append(GetPHPTime(DateTime.Now.ToString()));
            return sb.ToString();
        }
        /// <summary>
        /// 獲取店內碼
        /// </summary>
        /// <param name="lenght"></param>
        /// <param name="rd"></param>
        /// <returns></returns>
        public static string GetUpc(string item_id, string vender_id, string made_date)
        {
            string upc = "";
            if (!string.IsNullOrEmpty(item_id) && !string.IsNullOrEmpty(vender_id) && !string.IsNullOrEmpty(made_date))
            {
                upc = item_id.PadLeft(7, '0') + vender_id.PadLeft(4, '0') + made_date;
            }
            return upc;
        }

        /// <summary>
        /// 獲取mysql資料庫異常
        /// </summary>
        /// <param name="exNumber">mysql異常代碼</param>
        /// <returns></returns>
        public static string MySqlException(Exception ex)
        {
            string resultStr = string.Empty;

            if (ex != null)
            {
                int exNumber = 0;
                GetExNumber(ex, ref exNumber);
                if (exNumber != 0)
                {
                    switch (exNumber)
                    {
                        case 1011:
                            resultStr = "資料庫管理員名稱或密碼錯誤,請聯繫IT部門.";
                            break;
                        case 1045:
                            resultStr = "資料庫管理員名稱或密碼錯誤,請聯繫IT部門.";
                            break;
                        case 1049:
                            resultStr = "資料庫名稱錯誤,請聯繫IT部門.";
                            break;
                        case 1054:
                            resultStr = "資料表中字段異常,請聯繫IT部門.";
                            break;
                        case 1062:
                            resultStr = "資料表主鍵重複,請聯繫IT部門.";
                            break;
                        case 1064:
                            resultStr = "SQL語法錯誤,請聯繫IT部門.";
                            break;
                        case 1146:
                            resultStr = "資料表不存在,請聯繫IT部門.";
                            break;
                        case 1175:
                            resultStr = "資料庫阻止修改,請聯繫IT部門.";
                            break;
                        case 1451:
                            resultStr = "資料庫主外鍵更改異常,請聯繫IT部門.";
                            break;
                        default:
                            resultStr = "資料庫異常,請聯繫IT部門.";
                            break;
                    }
                }
            }
            return resultStr;
        }

        public static void GetExNumber(Exception ex, ref int exNumber)
        {
            int number = 0;

            if (ex != null)
            {
                int i = ex.Message.IndexOf(':');
                if (i != -1)
                {
                    string num = ex.Message.Substring(0, i);
                    if (int.TryParse(num, out number))
                    {
                        exNumber = number;
                    }
                    else
                    {
                        GetExNumber(ex.InnerException, ref exNumber);
                    }
                }
            }

        }
    }

}
