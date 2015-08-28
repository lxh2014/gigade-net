using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class IstockChange:PageBase
    {
        /// <summary>
        /// 帳卡編號
        /// </summary>
        public int sc_id { get; set; }
        /// <summary>
        /// 交易編號
        /// </summary>
        public string sc_trans_id { get; set; }
        /// <summary>
        /// 前置單號
        /// </summary>
        public string sc_cd_id { get; set; }
        /// <summary>
        /// 商品細項編號
        /// </summary>
        public uint item_id { get; set; }
        /// <summary>
        /// 交易類型
        /// </summary>
        public int sc_trans_type { get; set; }
        /// <summary>
        /// 原來數量
        /// </summary>
        public int sc_num_old { get; set; }
        /// <summary>
        /// 變化數量
        /// </summary>
        public int sc_num_chg { get; set; }
        /// <summary>
        /// 新數量
        /// </summary>
        public int sc_num_new { get; set; }
        /// <summary>
        /// 創建時間
        /// </summary>
        public DateTime sc_time { get; set; }
        /// <summary>
        ///創建者
        /// </summary>
        public int sc_user { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string sc_note { get; set; }
        /// <summary>
        /// 帳卡原因
        /// </summary>
        public int sc_istock_why{get;set;}
        public IstockChange()
        {
            sc_id = 0;
            sc_trans_id = string.Empty;
            sc_cd_id = string.Empty;
            sc_note = string.Empty;
            sc_num_chg = 0;
            sc_num_new = 0;
            sc_num_old = 0;
            sc_time = DateTime.MinValue;
            sc_trans_type = 0;
            sc_user = 0;
            item_id = 0;
            sc_istock_why = 0;
        }
    }
}
