using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;

namespace BLL.gigade.Model
{
    [DBTableInfo("bonus_record")]
    public class BonusRecord : PageBase
    {
        /// <summary>
        /// 該筆記錄流水編號
        /// </summary>
        public uint record_id { get; set; }
        /// <summary>
        /// 購物金、抵用卷  流水編號 bonus_master.master_id
        /// </summary>
        public uint master_id { get; set; }
        /// <summary>
        /// 該筆記錄添加類型  bonus_type.type_id
        /// </summary>
        public uint type_id { get; set; }
        /// <summary>
        /// 訂單主檔編號  order_master.order_id
        /// </summary>
        public uint order_id { get; set; }
        /// <summary>
        /// 記錄使用點數
        /// </summary>
        public uint record_use { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string record_note { get; set; }
        /// <summary>
        /// 記錄者名稱
        /// </summary>
        public string record_writer { get; set; }
        /// <summary>
        /// 記錄時間
        /// </summary>
        public uint record_createdate { get; set; }
        /// <summary>
        /// 修改時間
        /// </summary>
        public uint record_updatedate { get; set; }
        /// <summary>
        /// 操作者IP
        /// </summary>
        public string record_ipfrom { get; set; }

        /// <summary>
        /// 會員流水編號  users.user_id  add by zhuoqin0830w 2015/08/25
        /// </summary>
        public uint user_id { get; set; }

        public BonusRecord()
        {
            record_id = 0;
            master_id = 0;
            type_id = 0;
            order_id = 0;
            record_use = 0;
            record_note = string.Empty;
            record_writer = string.Empty;
            record_createdate = 0;
            record_updatedate = 0;
            record_ipfrom = string.Empty;
            //add by zhuoqin0830w 2015/08/24
            user_id = 0;
        }
    }
}