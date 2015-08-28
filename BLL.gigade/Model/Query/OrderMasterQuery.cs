using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class OrderMasterQuery : OrderMaster
    {
        /// <summary>
        /// 搜索關鍵字
        /// </summary>
        public string KeyWords { get; set; }
        /// <summary>
        /// 訂購時間
        /// </summary>
        public DateTime ordercreatedate { get; set; }
        public int channel_id { get; set; }
        public string channel_name_full { get; set; }

        public int billing_check { get; set; } //在現金,外站,貨到付款對賬搜索條件用到
        /*zz_chaojie 2014/09/11添加三個字段*/
        public string redirect_name { set; get; }
        public string redirect_url { set; get; }
        public string order_pay_message { set; get; }
        public int receivable { get; set; }
        public int receipted { get; set; }
        public string offsetamt { get; set; }
        public string group_name { get; set; }
        public string utm_id { get; set; }
        public string utm_source { get; set; }
        public string hncb_id { get; set; }
        //這三個字段是為了在類表頁編輯時能夠進入用戶修改信息修改
        public string birthday { get; set; }
        public string mytype { get; set; }
        public DateTime suser_reg_date { get; set; }
        public int product_mode { get; set; }
        public int count { get; set; }
        public int dcount { get; set; }
        public int export_id { get; set; }
        public int jdtype { get; set; }//是否顯示調度
        public uint deliver_id { get; set; }
        public int deliver_type { get; set; }//運送方式
        public string deliver_store_name { get; set; }
        public string deliver_code { get; set; }
        public DateTime NoteOrderModifyTime { get; set; }
        public int serial_id { get; set; }
        public string status_description { get; set; }
        public string status_ipfrom { get; set; }
        public DateTime StatusCreateDate { get; set; }
        public DateTime OrderDatePay { get; set; }
        public DateTime MoneyCollectDate { get; set; }
        public DateTime OrderCreateDate { get; set; }
        public DateTime OrderDateClose { get; set; }
        public string Hg_Nt { get; set; }
        public uint manager_id { get; set; }
        public string manager_name { get; set; }
        public string order_name { get; set; }// 收件人
        public string delivery_name { get; set; }// 寄件人
        public DateTime order_date_pay_startTime { get; set; }// 付款日期起始時間
        public DateTime order_date_pay_endTime { get; set; }// 付款日期結束時間
        public uint OrderId { get; set; }// 訂單編號
        public string vendor_name_simple { get; set; }// 供應商
        public int freight_set { get; set; }// 運送方式
        public int type { get; set; }// 出貨類別,1:統倉出貨,2:供應商自行出貨,3:供應商調度出貨,4:退貨,5:退貨瑕疵,6:瑕疵(目前數據中只有1和2兩種)
        public string product_name { get; set; }// 商品名稱
        public string product_spec_name { get; set; }// 商品活動
        public uint buy_num { get; set; }// 購買數量
        public uint delivery_status { get; set; }// 出貨單狀態
        public uint detail_status { get; set; }// 商品狀態
        public string note_order { get; set; } //備註
        public int combined_mode { get; set; }
        public int item_mode { get; set; }
        public int Item_Id { get; set; }
        public int delay { get; set; }
        public int dateType { get; set; }
        public int group_id { get; set; }
        public string selecttype { get; set; } //查詢條件
        public string searchcon { get; set; } //查詢內容
        public DateTime datestart { get; set; }
        public DateTime dateend { get; set; }
        public int pay_status { get; set; }
        public int invoice { get; set; }
        public string payment { get; set; }//
        public string orderStatus { get; set; }//
        public string export_flag_str { set; get; }
        //public string amount { set; get; }
        public int show_type { set; get; }

    
        public int delivery { get; set; }
        public string username { get; set; }
        public OrderMasterQuery()
        {
            KeyWords = string.Empty;
            channel_id = 0;
            channel_name_full = string.Empty;
            billing_check = -1;
            redirect_name = string.Empty;
            redirect_url = string.Empty;
            order_pay_message = string.Empty;
            receivable = 0;
            receipted = 0;
            offsetamt = string.Empty;
            group_name = string.Empty;
            utm_id = string.Empty;
            utm_source = string.Empty;
            hncb_id = string.Empty;
            birthday = string.Empty;
            mytype = string.Empty;
            suser_reg_date = DateTime.MinValue;
            product_mode = 0;
            count = 0;
            dcount = 0;
            export_id = 0;
            jdtype = 0;
            deliver_type = 0;
            deliver_id = 0;
            deliver_store_name = string.Empty;
            deliver_code = string.Empty;
            NoteOrderModifyTime = DateTime.MinValue;
            serial_id = 0;
            status_ipfrom = string.Empty;
            StatusCreateDate = DateTime.MinValue;
            OrderDatePay = DateTime.MinValue;
            MoneyCollectDate = DateTime.MinValue;
            OrderCreateDate = DateTime.MinValue;
            OrderDateClose = DateTime.MinValue;
            Hg_Nt = string.Empty;
            manager_id = 0;
            manager_name = string.Empty;
            combined_mode = 0;
            item_mode = 0;
            Item_Id = 0;
            delay = 0;
            dateType = 0;
            group_id = 0;
            selecttype = string.Empty;
            searchcon = string.Empty;
            datestart = DateTime.MinValue;
            dateend = DateTime.MinValue;
            export_flag_str = string.Empty;
            show_type = 0;
            delivery = 0;
            username = string.Empty;
        }
    }
}