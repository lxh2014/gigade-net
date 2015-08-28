using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class IinvdQuery:Iinvd
    {
        public string product_name { get; set; }
        public int movenum { get; set; }
        public string upc_id { get; set; }
        public int cde_dt_incr { get; set; }//保質天數
        public int cde_dt_var { get; set; } //允收天數
        public DateTime cde_dt_make { get; set; }//製造日期
        public string pwy_dte_ctl { get; set; }//有效期控管
        /// <summary>
        /// 料位編號
        /// </summary>
        public string loc_id { get; set; }
        public int sums { get; set; }
        public int loc_stor_cse_cap { get; set; }
        public int auto { get; set; }
        public int notimeortimeout { get; set; }
        public string startIloc { get; set; }
        public string endIloc { get; set; }
        public string lcat_id { get; set; }//主，副料位
        public string iloc_ista { get; set; }//料位锁
        public string parameterName{get;set;}
        public int cde_dt_shp { get; set; }//允出天數 
        public int serch_type { set; get; }//查詢條件
        public string serchcontent { get; set; }
        public string qity_name { get; set; }//鎖定原因
        public string Sort { get; set; } //排序選擇
        public string Firstsd { get; set; } //單雙選擇
        //為了匯出數據添加
        public int startDay { get; set; }
        public int endDay { get; set; }
        public int yugaoDay { get; set; }
        public int startcost { get; set; }
        public int endcost { get; set; }        
        public int startsum { get; set; }
        public int endsum { get; set; }
        public string prod_sz { get; set; }
        public string cb_jobid { get; set; }
        public string vender { get; set; }
        public uint vendor_id { get; set; }
        public string search_vendor { get; set; }
        public int prepaid { get; set; }
        public string user_name { get; set; }        
        public DateTime starttime { get; set; }//查詢區間日期
        public DateTime endtime { get; set; }//查詢區間日期
        public string remarks { get; set; }
      
        public IinvdQuery()
        {
            product_name = string.Empty;
            loc_id = string.Empty;
            movenum = 0;
            upc_id = string.Empty;
            cde_dt_incr = 999999;
            cde_dt_var = 999999;
            cde_dt_make = DateTime.Now;
            pwy_dte_ctl = "N";
            notimeortimeout = 0;
            startIloc = string.Empty;
            endIloc = string.Empty;
            lcat_id = string.Empty;
            iloc_ista = string.Empty;
            parameterName = string.Empty;
            cde_dt_shp = 999999;
            serchcontent = string.Empty;
            serch_type = 0;
            qity_name = string.Empty;
            startDay = 0;
            endDay = 0;
            yugaoDay = 0;
            startcost = 0;
            endcost = 0;
            prod_sz = string.Empty;
            prepaid = 0;
            user_name = string.Empty;
            vendor_id = 0;
            remarks = string.Empty;
        }
    }
}
