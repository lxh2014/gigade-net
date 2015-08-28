using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class LogInLogeQuery : PageBase
    {
        /// <summary>
        /// 用戶編號
        /// </summary>
        public uint user_id { get; set; }
        /// <summary>
        /// 用戶名稱
        /// </summary>
        public string user_username { get; set; }
        /// <summary>
        /// 登錄編號
        /// </summary>
        public uint login_id { get; set; }
        /// <summary>
        /// 登錄IP
        /// </summary>
        public string login_ipfrom { get; set; }
        /// <summary>
        /// 登錄日期
        /// </summary>
        public uint login_createdate { get; set; }

        /// <summary>
        /// 登錄時間（時間類型字符串）
        /// </summary>
        private string strlogindate;
        public string Strlogindate
        {
            get { return strlogindate; }
            set { strlogindate = BLL.gigade.Common.CommonFunction.GetNetTime(long.Parse(value)).ToString("yyyy-MM-dd hh:mm:ss"); }
        }

        public LogInLogeQuery()
        {
            user_id = 0;
            user_username = string.Empty;
            login_id = 0;
            login_ipfrom = string.Empty;
            login_createdate = 0;
            strlogindate = string.Empty;
        }
    }
}
