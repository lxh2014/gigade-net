using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class OrderModifyModel
    {
        public string ip_from;
        public string record_note;
        /// <summary>
        /// 訂單編號
        /// </summary>
        public int order_id;
        /// <summary>
        /// 紅利折抵點數
        /// </summary>
        public int deduct_card_bonus;
        /// <summary>
        /// 是否開立發票
        /// </summary>
        public bool isBilling_checked;
        /// <summary>
        /// 是否贈送HG點數
        /// </summary>
        public bool isHGBonus;
        /// <summary>
        /// 是否折抵金額
        /// </summary>
        public bool isCash_record_bonus;
        /// <summary>
        /// 訂單狀態
        /// </summary>
        public int order_status;
        /// <summary>
        /// 
        /// </summary>
        public int slave_status;
        /// <summary>
        /// 
        /// </summary>
        public int detail_status;
        /// <summary>
        /// 
        /// </summary>
        public int order_date_pay;
        /// <summary>
        /// 
        /// </summary>
        public int order_date_cancel;
        /// <summary>
        /// 
        /// </summary>
        public int money_cancel;
        /// <summary>
        /// 
        /// </summary>
        public int export_flag;

        public int user_id;
        public int type_id;
        public int bonus_num;
        public string use_note;
        public int use_writer;
        public string record_writer;
    }
}
