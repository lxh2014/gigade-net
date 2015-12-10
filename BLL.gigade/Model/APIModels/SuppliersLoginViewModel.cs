using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GigadeApi.Framework.ViewModels.SuppliersAccount
{
    /// <summary>
    /// 登入參數
    /// </summary>
    [Serializable]
    public class SuppliersLoginViewModel
    {
        public SuppliersLoginViewModel()
        {
            //deviceInfo = new DeviceInfoViewModel();
        }
        /// <summary>
        ///   會員賬號
        /// </summary>
        public string user_email { get; set; }

        /// <summary>
        /// 會員密碼
        /// </summary>
        public string user_password { get; set; }
        /// <summary>
        /// 前臺token
        /// </summary>
        public string user_halfToken { get; set; }
        /// <summary>
        /// 登入IP地址
        /// </summary>
        public string login_ipfrom { get; set; }
        /// <summary>
        /// 登入時間
        /// </summary>
        public long login_createdate { get; set; }
    }
}