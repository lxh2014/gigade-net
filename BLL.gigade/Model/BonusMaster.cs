using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class BonusMaster : PageBase
    {
        /// <summary>
        /// 該筆記錄流水編號
        /// </summary>
        public uint master_id { get; set; }
        /// <summary>
        /// 會員流水編號  users.user_id
        /// </summary>
        public uint user_id { get; set; }
        /// <summary>
        /// 該筆記錄添加類型  bonus_type.type_id
        /// </summary>
        public uint type_id { get; set; }
        /// <summary>
        /// 該筆記錄添加的總點數
        /// </summary>
        public uint master_total { get; set; }
        /// <summary>
        /// 剩餘可用點數
        /// </summary>
        public int master_balance { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string master_note { get; set; }
        /// <summary>
        /// 記錄者名稱
        /// </summary>
        public string master_writer { get; set; }
        /// <summary>
        /// 生效日期
        /// </summary>
        public uint master_start { get; set; }
        /// <summary>
        /// 過期日期
        /// </summary>
        public uint master_end { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        public uint master_createdate { get; set; }
        /// <summary>
        /// 最後修改日期
        /// </summary>
        public uint master_updatedate { get; set; }
        /// <summary>
        /// 操作者IP
        /// </summary>
        public string master_ipfrom { get; set; }
        public uint apprise { get; set; }
        /// <summary>
        /// 該筆記錄為購物金或抵用卷（1:購物金，2:抵用卷）
        /// </summary>
        public uint bonus_type { get; set; }

        public BonusMaster()
        {
            master_id = 0;
            user_id = 0;
            type_id = 0;
            master_total = 0;
            master_balance = 0;
            master_note = string.Empty;
            master_writer = string.Empty;
            master_start = 0;
            master_end = 0;
            master_createdate = 0;
            master_updatedate = 0;
            master_ipfrom = string.Empty;
            apprise = 0;
            bonus_type = 1;
        }
    }
}
