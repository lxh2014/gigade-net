using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    [Serializable]
    public class Caller:PageBase
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int user_id { get; set; }
        /// <summary>
        /// 郵箱
        /// </summary>
        public string user_email { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string user_username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string user_password { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public int user_status { get; set; }
        /// <summary>
        /// 登錄錯誤次數
        /// </summary>
        public int user_login_attempts { get; set; }
        /// <summary>
        /// 確認碼
        /// </summary>
        public string user_confirm_code { get; set; }

        public Caller()
        {
            user_id = 0;
            user_email = string.Empty;
            user_username = string.Empty;
            user_password = string.Empty;
            user_status = 0;
            user_login_attempts = 0;
            user_confirm_code = string.Empty;

        }

    }
}
