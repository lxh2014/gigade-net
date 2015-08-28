using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class OrderReturnMasterQuery : OrderReturnMaster
    {
        public string vendor_name { get; set; }
        public DateTime createdate { get; set; }
        public DateTime updatedate { get; set; }
        public string sqlCon { get; set; }
        public string sqlTable { get; set; }
        public string bank_name { get; set; }
        public string bank_branch { get; set; }
        public string bank_account { get; set; }
        public string account_name { get; set; }
        public string HgReturnUrl { get; set; }
        public string HgMerchandID { get; set; }
        public string HgTeminalID { get; set; }
        public string HgCategoty { get; set; }
        public string HgWallet { get; set; }
        #region changjian0408j 于 2014/10/30添加
        public UInt64 subtotal { get; set; }
        public uint detail_id { get; set; }
        public uint slave_id { get; set; }
        public uint item_id { get; set; }
        public uint product_freight_set { get; set; }
        public string product_name { get; set; }
        public string product_spec_name { get; set; }
        public string detail_note { get; set; }
        public uint single_money { get; set; }
        public uint buy_num { get; set; }
        public uint deduct_bonus { get; set; }
        public string return_status_string { get; set; }
        public string product_freight_set_string { get; set; }
        public string vendor_name_simple { get; set; }
        public string company_phone { get; set; }
        public uint company_zip { get; set; }
        public string company_address { get; set; }
        #endregion
        public string detailId { get; set; }

        public int ven_type { get; set; }
        public string content { get; set; }
        public int date_type { get; set; }
        public string time_start { get; set; }
        public string time_end { get; set; }
        public string user_id { get; set; }
        public string user_username { get; set; }

        public string orc_remark { get; set; }
        public string orc_service_remark { get; set; }


        public OrderReturnMasterQuery()
        {
            vendor_name = string.Empty;
            createdate = DateTime.MinValue;
            updatedate = DateTime.MinValue;
            sqlCon = string.Empty;
            sqlTable = string.Empty;
            bank_name = string.Empty;
            bank_branch = string.Empty;
            bank_account = string.Empty;
            account_name = string.Empty;
            HgReturnUrl = string.Empty;
            HgMerchandID = string.Empty;
            HgTeminalID = string.Empty;
            HgCategoty = string.Empty;
            HgWallet = string.Empty;
            detailId = string.Empty;
            ven_type = 0;
            content = string.Empty;
            date_type = 0;
            time_start = string.Empty;
            time_end = string.Empty;
            user_id = string.Empty;
            user_username = string.Empty;
            orc_remark = string.Empty;
            orc_service_remark = string.Empty;
        }
    }
}
