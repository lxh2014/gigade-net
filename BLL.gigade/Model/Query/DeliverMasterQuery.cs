using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//chaojie_zz添加字段order_createtime,order_status,warehouse_status,ShipmentName，overdue_day,priority,time_start,time_end;
namespace BLL.gigade.Model.Query
{
    public class DeliverMasterQuery:DeliverMaster
    {
        public uint vendor_id { get; set; }
        public string vendor_name_simple { get; set; }
        public string vendor_name_full { get; set; }
        public string types { get; set; }
        public string states { get; set; }
        public string stores { get; set; }
        //出貨管理-出貨查詢列表頁面
        public DateTime order_createtime { get; set; }//訂單成立日期
        public uint order_status { set; get; }//訂單狀態
        public int warehouse_status { set; get; }//出貨單狀態
        public string ShipmentName { set; get; }//物流業者名稱
        public int overdue_day { set; get; }//逾期天數
        public int priority { set; get; }//出貨篩選
        public DateTime time_start{set;get;}//開始時間
        public DateTime time_end{set;get;}//結束時間
        public int logisticsType { set; get; }//物流状态号码
        public string LogisticsStatus { set; get; }//物流状态

        //--出貨管理-匯出CSV
        public DateTime money_pay_date { set; get; }//付款日期
        public DateTime order_pay_date { set; get; }//可出貨日期
        //出貨管理-出貨確認需加信息\\2014/12/10
        public uint item_id { get; set; }
        public string product_name { get; set; }
        public uint buy_num { get; set; }
        public string status { get; set; }
        
        //出貨查詢中的檢視頁面 zhangyu
        public string note_order { get; set; }
        public uint holiday_deliver { get; set; }

        //出貨確認
        public string delivery_store_name { get; set; }     //快遞商名稱

        //外站出貨檔匯出列表
        public uint od_order_id { get; set; }//order_master
        public int retrieve_mode { get; set; }//order_master
        public int channel { get; set; }//order_master
        public string channel_order_id { get; set; }//order_master
        public string sub_order_id { get; set; } //order_detail
        public int dd_status { get; set; }//deliver_master
        public string sqlwhere { get; set; }
        public string sretrieve_mode { get; set; }//取貨模式
        public string schannel { get; set; }//賣場名稱
        public string sdelivery_store { get; set; }//物流商家
        public string sexport_id { get; set; }//供應商
        public int payment { get; set; }//支付方式
        public DateTime order_time_begin { get; set; }//訂單創建時間
        public DateTime order_time_end { get; set; }
        public int deliver_store { get; set; }
        public string sorder_id { get; set; }
        public string sdeliver_id { get; set; }
        public int i_order_status { get; set; }
        public int i_slave_status { get; set; }
        public int i_detail_status { get; set; }
        public int ideliver_status { get; set; }
        public int product_mode { get; set; }
        public int serch_msg { get; set; }
        public string serch_where { get; set; }
        public int t_days { get; set; }
        public int time_type { get; set; }

        public string estimated_arrival_period_str { get; set; }
        public string delivery_date_str { get; set; }
        public int order_day { set; get; }//add by chaojie1124j 2015/11/3 05:21PM,距離預計到貨日，實現訂單細項查詢中，預計到貨日-今天<=order_day的未到貨的出貨單show出來
        public DeliverMasterQuery()
        {
            vendor_id = 0;
            vendor_name_full = string.Empty;
            vendor_name_simple = string.Empty;
            order_status = 0;
            warehouse_status =0;
            order_createtime = DateTime.Now;
            item_id = 0;
            product_name = string.Empty;
            buy_num = 0;
            status = string.Empty;
            ShipmentName = string.Empty;
            note_order = string.Empty;
            holiday_deliver = 0;
            overdue_day = 0;
            priority = 0;
            time_start = DateTime.MinValue;
            time_end = DateTime.MinValue;
            delivery_store_name = string.Empty;
            retrieve_mode = -1;
            channel = 0;
            channel_order_id = string.Empty;
            sub_order_id = string.Empty;
            dd_status = -1;
            od_order_id = 0;
            sqlwhere = string.Empty;
            sretrieve_mode = string.Empty;
            schannel = string.Empty;
            sexport_id = string.Empty;
            sdelivery_store = string.Empty;
            money_pay_date = DateTime.MinValue;
            order_pay_date = DateTime.MinValue;
            logisticsType = 0;
            LogisticsStatus = string.Empty;
            payment = 0;
            order_time_begin = DateTime.MinValue;
            order_time_end = DateTime.MinValue;
            deliver_store = 0;
            sdeliver_id = string.Empty;
            sorder_id = string.Empty;
            i_order_status = -1;
            i_slave_status=-1;
            i_detail_status = -1;
            ideliver_status = -1;
            product_mode = 0;
            serch_msg = 0;
            serch_where = string.Empty;
            t_days = 0;
            time_type = 0;
            estimated_arrival_period_str = string.Empty;
            delivery_date_str = string.Empty;
        }
    }
}
