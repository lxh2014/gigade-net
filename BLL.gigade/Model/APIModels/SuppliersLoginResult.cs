using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GigadeApi.Framework.ViewModels.SuppliersAccount
{
    /// <summary>
    /// 供應商信息
    /// </summary>
    [Serializable]
    public class VendorInfo
    {
        /// <summary>
        /// 供應商ID
        /// </summary>
        public long vendor_id { get; set; }
        /// <summary>
        /// 供應商編碼
        /// </summary>
        public string vendor_code { get; set; }
        /// <summary>
        /// 供應商狀態
        /// </summary>
        public byte vendor_status { get; set; }
        /// <summary>
        /// 供應商郵箱
        /// </summary>
        public string vendor_email { get; set; }
        /// <summary>
        /// 供應商名稱
        /// </summary>
        public string vendor_name_full { get; set; }
        /// <summary>
        /// 供應商簡稱
        /// </summary>
        public string vendor_name_simple { get; set; }
        /// <summary>
        /// 公司地址
        /// </summary>
        public string company_address { get; set; }
        /// <summary>
        /// 帳號上次登入時間
        /// </summary>
        public DateTime vendorLastLoginTime { get; set; }
    }
    /// <summary>
    /// 登入返回結果
    /// </summary>
    public class SuppliersLoginResult
    {
        public VendorInfo accountInfo { get; set; }
        /// <summary>
        /// gigade存取令牌
        /// </summary>
        public UserToken userToken { get; set; }
    }
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
