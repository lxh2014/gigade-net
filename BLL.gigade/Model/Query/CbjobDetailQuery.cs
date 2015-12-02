using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class CbjobDetailQuery : CbjobDetail
    {
        public string searchcontent { get; set; }//輸入工作單號
        public string user_username { get; set; }
        public DateTime made_dt { get; set; }
        public DateTime cde_dt { get; set; }
        public string loc_id { get; set; }
        public int prod_qty { get; set; }
        public uint item_id { get; set; }
        public int st_qty { get; set; }//復盤數量
        public string sta_id { get; set; }
        public string product_name { get; set; }//產品名稱
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public uint spec_id_1 { get; set; }//規格
        public uint spec_id_2 { get; set; }
        public string spec_title_1 { get; set; }
        public string spec_title_2 { get; set; }
        public CbjobDetailQuery()
        {
            searchcontent = string.Empty;
            user_username = string.Empty;
            made_dt = DateTime.MinValue;
            cde_dt = DateTime.MinValue;
            loc_id = string.Empty;
            prod_qty = 0;
            item_id = 0;
            st_qty = 0;
            sta_id = "CNT";
            product_name = string.Empty;
            StartDate = DateTime.MinValue;
            EndDate = DateTime.MinValue;
            spec_id_1 = 0;
            spec_id_2=0;
            spec_title_1 = string.Empty;
            spec_title_2 = string.Empty;
        }
    }
}
