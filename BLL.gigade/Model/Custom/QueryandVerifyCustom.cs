using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Model;
namespace BLL.gigade.Model.Custom
{
    public class QueryandVerifyCustom
    {

        //public string _imgsrc { get; set; }
        //public int _productid { get; set; }
        //public string _brandname { get; set; }
        //public string _productname { get; set; }
        //public string _combination { get; set; }
        //public int _combination_id { get; set; }
        //public string _pricetype { get; set; }
        //public int _pricetype_id { get; set; }
        //public string _productstate { get; set; }
        //public uint _price { get; set; }
        //public uint _cost { get; set; }
        //public uint _activeprice { get; set; }
        //public uint _activecost { get; set; }

        public string product_image { get; set; }
        public uint product_id { get; set; }
        public string brand_name { get; set; }
        public int prepaid { get; set; } //add by wangwei0216w 2014/10/24
        public string isPrepaid { get { return prepaid == 0 ? "否" : "是"; } }
        /// <summary>
        /// 商品名稱
        /// </summary>
        public string product_name { get; set; }
        /// <summary>
        /// 規格欄位
        /// </summary>
        public string prod_sz { get; set; }
        /// <summary>
        /// 商品類型
        /// </summary>
        public string combination { get; set; }
        /// <summary>
        /// 價格類型
        /// </summary>
        public string price_type { get; set; }//add by hufeng0813w 2014/06/18
        public int price_type_id { get; set; }
        /// <summary>
        /// 商品類型
        /// </summary>
        public uint combination_id { get; set; }
        public string product_spec { get; set; }
        public int product_spec_id { get; set; }
        public string product_status { get; set; }
        public uint product_status_id { get; set; }
        public uint product_sort { get; set; }
        public uint product_createdate { get; set; }
        public string site_name { get; set; }
        public uint site_id { get; set; }//站臺ID
        public uint master_user_id { get; set; }//add by hufeng0813w 2014/05/22 price_master表中的User_id
        public string user_level { get; set; }//會員等級
        public uint level { get; set; }//會員等級ID
        public string user_email { get; set; }
        public uint user { get; set; }
        public string price_status { get; set; }
        public int price_status_id { get; set; }
        public int price { get; set; }
        public int cost { get; set; }//成本 add by xiangwang0413w 2014/07/30
        public int event_price { get; set; }
        public int event_cost { get; set; }// 活動成本 add by xiangwang0413w 2014/07/30
        public uint event_start { get; set; }
        public uint event_end { get; set; }
        public int Prod_Classify { get; set; }//添加館別欄位  eidt  by zhuoqin0830w 2015/03/05
        //add by zhuoqin0830w  2015/03/10  商品資料匯出是館別信息
        public string prod_classify { get { return Prod_Classify == 0 ? "" : (Prod_Classify == 10 ? "食品館" : "用品館"); } }
        public uint bag_check_money { get; set; }
        //add by zhuoqin0830w  2015/06/30  失格商品篩選
        public int off_grade { get; set; }

        /// <summary>
        /// 預購商品開始時間 add by guodong1130w 2015/09/16 
        /// </summary>
        public uint purchase_in_advance_start { get; set; }
        public string purchase_in_advance_start_time
        {
            get
            {
                DateTime dt = CommonFunction.GetNetTime(purchase_in_advance_start);
                if (dt.Year == 1970)
                    return "";
                else
                    return dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// 預購商品結束時間 add by guodong1130w 2015/09/16 
        /// </summary>
        public uint purchase_in_advance_end { get; set; }
        public string purchase_in_advance_end_time
        {
            get
            {
                DateTime dt = CommonFunction.GetNetTime(purchase_in_advance_end);
                if (dt.Year == 1970)
                    return "";
                else
                    return dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// 預購商品出貨時間 add by guodong1130w 2015/09/16 
        /// </summary>
        public uint expect_time { get; set; }
        public string expect_time_time
        {
            get
            {
                DateTime dt = CommonFunction.GetNetTime(expect_time);
                if (dt.Year == 1970)
                    return "";
                else
                    return dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        //庫存數 add by guodong1130w 2015/09/16 
        public int Item_Stock { get; set; }
        //未出貨數量 add by guodong1130w 2015/09/16 
        public int bnum { get; set; }
        //排成設定 add by guodong1130w 2015/09/16 
        public string  schedule_name { get; set; }
        /// <summary>
        /// 活動時間
        /// </summary>
        public string event_time {
            get {
                DateTime start = CommonFunction.GetNetTime(event_start);
                DateTime end = CommonFunction.GetNetTime(event_end);
                string startTime = start.ToString("yyyy-MM-dd HH:mm:ss"); ;
                string endTime = end.ToString("yyyy-MM-dd HH:mm:ss");
                if (start.Year == 1970) startTime = "";
                if (end.Year == 1970) endTime = "";
                return startTime + " ~ " + endTime;
            }
        }
        public DateTime apply_time { get; set; }
        public uint apply_id { get; set; }
        public string apply_user { get; set; }
        public uint online_mode { get; set; }
        public uint price_master_id { get; set; }
        public uint brand_id { get; set; }
        public int create_channel { get; set; }
        /// <summary>
        /// 建立人
        /// </summary>
        public string user_name { get; set; }
        public uint user_id { get; set; }
        /// <summary>
        /// 建議售價
        /// </summary>
        public uint product_price_list { get; set; }
        /// <summary>
        /// 申請類型
        /// </summary>
        public string prev_status { get; set; }

        public int prev_status_id { get; set; }
        /// <summary>
        /// 運送方式
        /// </summary>
        public string product_freight_set { get; set; }

        public uint product_freight_set_id { get; set; }
        /// <summary>
        /// 出貨方式
        /// </summary>
        public string product_mode { get; set; }
        public int product_mode_id { get; set; }
        /// <summary>
        /// 營業稅
        /// </summary>
        public int tax_type { get; set; }
        /// <summary>
        /// 營業稅
        /// </summary>
        public string tax_type_name
        {
            get
            {
                return tax_type == 1 ? "應稅" : "免稅";
            }
        }
        /// <summary>
        /// 上架時間 
        /// </summary>
        public uint product_start { get; set; }
        public string product_start_time {
            get 
            {
                DateTime dt=CommonFunction.GetNetTime(product_start);
                if (dt.Year == 1970)
                    return "";
                else
                    return dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        /// <summary>
        /// 下架時間
        /// </summary>
        public uint product_end { get; set; }
        public string product_end_time
        {
            get
            {
                DateTime dt = CommonFunction.GetNetTime(product_end);
                if (dt.Year == 1970)
                    return "";
                else
                    return dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }


        public string CanSel
        {
            get
            {
                return user_id.ToString() ==(System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString() ? "0" : "1";
            }
        }
        private string cando = null;//edit by xiangwang 2014/09/11
        public string CanDo
        {
            get
            {
                if (cando == null)
                {
                    cando = user_id.ToString() != (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString() ? "0" : "1";
                }
                return cando ;
            }
            set{
                cando = value;
            }
        }


        public string CanReplace
        {
            get
            {
                return (product_status == "新建立商品" || product_status == "下架") ? "0" : "1";
            }
        }

        public string sale_name { get; set; }
        public string vendor_name_full { get; set; } //供應商全程
        public string vendor_name_simple { get; set; }//供應商簡稱
        public string erp_id { get; set; }
        public int sale_status { get; set; }//add by wwei0216w 2015/2/5 販售狀態

        public int sale_status_id { get; set; }

        public string itemIds { get; set; }/// add by wwei0216w2015/8/12
        public string remark { get; set; }//添加商品上下架備註欄位 add by mingwei0727w 2015/09/25
        public QueryandVerifyCustom()
        {
             //_imgsrc =string.Empty;
             //_productid= 0;
             //_brandname =string.Empty;
             //_productname =string.Empty;
             //_combination =string.Empty;
             //_combination_id = 0;
             //_pricetype = string.Empty;
             //_pricetype_id = 0;
             //_productstate = string.Empty;
             //_price = 0;
             //_cost = 0;
             //_activeprice = 0;
             //_activecost = 0;

            product_image = string.Empty;
            product_id = 0;
            brand_name = string.Empty;
            brand_id = 0;
            product_name = string.Empty;
            combination = string.Empty;
            price_type = string.Empty;//add by hufeng0813w 2014/06/18
            price_type_id = 0;
            product_spec = string.Empty;
            product_spec_id = 0;
            product_status = string.Empty;
            product_status_id = 0;
            tax_type = 0;
            product_sort = 0;
            product_createdate = 0;
            product_start = 0;
            product_end = 0;
            site_name = string.Empty;
            site_id = 0;
            master_user_id = 0;
            user_level = string.Empty;
            level = 0;
            user_email = string.Empty;
            user = 0;
            price_status = string.Empty;
            price_status_id = 0;
            price = 0;
            cost = 0;
            event_price = 0;
            event_cost = 0;
            event_start = 0;
            event_end = 0;
            apply_time = DateTime.MinValue;
            apply_id = 0;
            apply_user = string.Empty;
            online_mode = 0;
            user_name = string.Empty;
            user_id = 0;
            product_price_list = 0;
            prev_status = string.Empty;
            prev_status_id = 0;
            product_freight_set = string.Empty;
            product_freight_set_id = 0;
            product_mode = string.Empty;
            product_mode_id = 0;
            price_master_id = 0;
            create_channel = 1; //add by wangwei0216w 2014/9/29 添加chreate_channel字段 並且使其默認為 1(既管理者添加的商品)
            prepaid = 0;
            sale_name = string.Empty;
            sale_status = 0;
            sale_status_id = 0;
            erp_id = string.Empty;
            vendor_name_simple = string.Empty;
            Prod_Classify = 0;//添加館別欄位  eidt  by zhuoqin0830w 2015/03/05
            remark = string.Empty;//添加商品上下架備註欄位 add by mingwei0727w 2015/09/25
            bag_check_money = 0; //寄倉費
            combination_id = 0;
            off_grade = 0;
            itemIds = "Empty data!";
            Item_Stock = 0;
             bnum =0;
             schedule_name = string.Empty;
        }
    }
}
