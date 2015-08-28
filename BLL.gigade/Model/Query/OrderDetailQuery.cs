#region 文件信息
/* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司
* All rights reserved. 
*  
* 文件名称：OrderDetailQuery.cs 
* 摘   要： 查詢付款單中所有訂單所用到的字段
*  
* 当前版本：1.0 
* 作   者：shuangshuang0420j 
* 完成日期：2014/10/28 15:14:47 
* 
*chaojie1124j 于2014/12/15 05:19 PM 添加字段Deliver_Id,Delivery_Name,Ticket_Id,order_id
 *chaojie1124j 于2014/12/24 09:15 PM 添加三個字段 Note_Order,Order_Name
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
namespace BLL.gigade.Model.Query
{
    public class OrderDetailQuery : OrderDetail
    {
        //order_detail內部元素
        //public uint Detail_Id { get; set; }
        //public uint Slave_Id { get; set; }
        //public uint Item_Id { get; set; }
        //public uint Item_Vendor_Id { get; set; }
        //public uint Product_Freight_Set { get; set; }
        //public uint Product_Mode { get; set; }
        //public string Product_Name { get; set; }
        //public string Product_Spec_Name { get; set; }
        //public uint Single_Cost { get; set; }
        //public uint Event_Cost { get; set; }
        //public uint Single_Price { get; set; }
        //public uint Single_Money { get; set; }
        //public uint Deduct_Bonus { get; set; }
        //public uint Deduct_Welfare { get; set; }
        //public int Deduct_Happygo { get; set; }//int
        //public int Deduct_Happygo_Money { get; set; }
        //public uint Deduct_Account { get; set; }
        //public string Deduct_Account_Note { get; set; }
        //public int Accumulated_Bonus { get; set; }//int
        //public int Accumulated_Happygo { get; set; }//int
        //public uint Buy_Num { get; set; }
        //public uint Detail_Status { get; set; }
        //public string Detail_Note { get; set; }
        //public string Item_Code { get; set; }
        //public uint Arrival_Status { get; set; }
        //public uint Delay_Till { get; set; }
        //public string Lastmile_Deliver_Serial { get; set; }
        //public uint Lastmile_Deliver_Datetime { get; set; }
        //public string Lastmile_Deliver_Agency { get; set; }
        //public uint Bag_Check_Money { get; set; }
        //public string Channel_Detail_Id { get; set; }
        //public int Combined_Mode { get; set; }//int
        //public uint item_mode { get; set; }
        //public int Parent_Id { get; set; }//int
        //public string Sub_Order_Id { get; set; }
        //public uint pack_id { get; set; }
        //public string parent_name { get; set; }
        //public uint parent_num { get; set; }
        //public uint price_master_id { get; set; }
        /// <summary>
        /// 已買斷的商品  0:否  1:是
        /// </summary>
        //public int Prepaid { get; set; }
        //public int site_id { get; set; }
        //public string event_id { get; set; }
        //public int is_gift { get; set; }//是否為贈品
        //end 內部元素

        public int isChildItem { get; set; }//訂單內容中用於是否用item_mode作為條件判斷的依據，無其他意義
        public uint Vendor_Id { get; set; }
        public uint SumNum { get; set; }
        public uint Order_Id { get; set; }
        public uint Slave_Status { get; set; }//訂單狀態
        public uint Slave_Date_Close { get; set; }//訂單歸檔日
        public string Brand_Name { get; set; }
        public int Tax_Type { get; set; }//營業稅
        //public int Sub_Total { get; set; }//小計
        public uint subtotal { get; set; }
        public uint Account_Status { get; set; }//廠商對賬

        public string Vendor_Name_Simple { get; set; }
        public string Slave_Status_Str { get; set; }
        public DateTime Clos_Date { get; set; }
        public string Detail_Status_Str { get; set; }
        public string Product_Freight_Set_Str { get; set; }
        public string Product_Mode_Name { get; set; }
        public uint Product_Id { get; set; }
        //chaojie1124j 添加四個字段,批次出貨編號=>傳票明細
        public uint Deliver_Id { set; get; }//出貨編號
        public string Delivery_Name { set; get; }//收貨人
        public int Ticket_Id { set; get; }//批次出貨編號

        public int order_num { set; get; }//deliver_master>order_id
        public string VendorMd5 { get; set; }
        //chaojie1124j 添加三個字段，供應商 供應商後台=>調度出貨中=>批次檢貨明細列印 2014/12/24
        public string Note_Order { set; get; }
        public string Order_Name { set; get; }
        //  public int slave_date_delivery { get; set; }
        //public int Search_Type { get; set; }//寄倉商品出貨列表
        //public string SearchComment { get; set; }
        //public int Serch_Time_Type { get; set; }
        //public int time_start { get; set; }
        //public int time_end { get; set; }
        public uint slave_date_delivery { get; set; }
        public string s_slave_date_delivery { get; set; }
        public int select_type { get; set; }
        public string select_con { get; set; }
        public int date { get; set; }
        public DateTime start_time { get; set; }

        public DateTime end_time { get; set; }
        public int radiostatus { get; set; }
        public int promodel { get; set; }
        public string slave_status_string { get; set; }
        public string product_mode_string { get; set; }
        public string spec_image { get; set; }
        public uint Brand_Id { set; get; }
        public string Brand_Id_In { get; set; }
        
        public uint cost { get; set; }

        public uint Order_Createdate { get; set; }
        public long time_start { set; get; }
        public long time_end { set; get; }
        public int product_manage { set; get; }
        public int channel { set; get; }
        public int order_payment { set; get; }
        public int Status { set; get; }
        //public string singlemoney { get; set; }
        //public string Total { get; set; }
        public string channel_name_simple { get; set; }
        
       
        public OrderDetailQuery()
        {            
            //Detail_Id = 0;
            //Slave_Id = 0;
            //Item_Id = 0;
            //Item_Vendor_Id = 0;
            //Product_Freight_Set = 0;
            //Product_Mode = 0;
            //Product_Name = string.Empty;
            //Product_Spec_Name = string.Empty;
            //Single_Cost = 0;
            //Event_Cost = 0;
            //Single_Price = 0;
            //Single_Money = 0;
            //Deduct_Bonus = 0;
            //Deduct_Welfare = 0;
            //Deduct_Happygo = 0;
            //Deduct_Happygo_Money = 0;
            //Deduct_Account = 0;
            //Deduct_Account_Note = string.Empty;
            //Accumulated_Bonus = 0;
            //Accumulated_Happygo = 0;
            //Buy_Num = 0;
            //Detail_Status = 0;
            //Detail_Note = string.Empty;
            //Item_Code = string.Empty;
            //Arrival_Status = 0;
            //Delay_Till = 0;
            //Lastmile_Deliver_Serial = string.Empty;
            //Lastmile_Deliver_Datetime = 0;
            //Lastmile_Deliver_Agency = string.Empty;
            //Bag_Check_Money = 0;
            //Channel_Detail_Id = string.Empty;
            //Combined_Mode = 0;
            //item_mode = 0;//0:單一商品, 1:父商品, 2:子商品
            //Parent_Id = 0;
            //Sub_Order_Id = string.Empty;
            //pack_id = 0;
            //parent_name = string.Empty;
            //parent_num = 0;
            //price_master_id = 0;
            //Prepaid = 0;

            SumNum = 0;
            Vendor_Id = 0;
            isChildItem = -1;//訂單內容中用於是否用item_mode作為條件判斷的依據，無其他意義。-1：不做判斷，0：非子商品即item_mode!=2,1是子商品，item_mode=2

            Order_Id = 0;
            Slave_Status = 0;
            Slave_Date_Close = 0;
            Brand_Name = string.Empty;
            Tax_Type = 0;
            //Sub_Total = 0;
            Account_Status = 0;
            Vendor_Name_Simple = string.Empty;
            Slave_Status_Str = string.Empty;
            Detail_Status_Str = string.Empty;
            Clos_Date = DateTime.MinValue;
            Product_Freight_Set_Str = string.Empty;
            Product_Mode_Name = string.Empty;
            Product_Id = 0;
            Deliver_Id = 0;
            Delivery_Name = string.Empty;
            Ticket_Id = 0;
            order_num = 0;
            VendorMd5 = string.Empty;
            // slave_date_delivery = 0;
            //Search_Type=0;
            //SearchComment=string.Empty;
            //Serch_Time_Type=0;
            //time_start=0;
            //time_end = 0;
            slave_date_delivery = 0;
            s_slave_date_delivery = string.Empty;
            select_type = 0;
            select_con = string.Empty;
            date = 0;
            start_time = DateTime.MinValue;
            end_time = DateTime.MinValue;
            radiostatus = 0;
            promodel = 0;
            slave_status_string = string.Empty;
            product_mode_string = string.Empty;
            spec_image = string.Empty;
            //subtotal = 0;
            cost = 0;
            Brand_Id = 0;
            Brand_Id_In = string.Empty;
            Order_Createdate=0;
            time_start = 0;
            time_end = 0;
            product_manage = 0;
            channel = 0;
            order_payment = 0;
            Status = 0;
         
        }
    }
}
