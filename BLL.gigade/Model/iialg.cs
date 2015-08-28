using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class iialg:PageBase
    {
        public int row_id { get; set; }
        public string loc_id { get; set; }
        public uint item_id { get; set; }
        public string iarc_id { get; set; }
        public int qty_o { get; set; }
        public int adj_qty { get; set; }//轉移量 可正可負
        public DateTime create_dtim { get; set; }
        public int create_user { get; set; }
        public string doc_no { get; set; }
        public string po_id { get; set; }
        public DateTime made_dt { get; set; }
        public DateTime cde_dt { get; set; }
        public string remarks { get; set; }
        public int type { get; set; }
        public DateTime c_made_dt { get; set; }
        public DateTime c_cde_dt { get; set; }

        public iialg()
        {
            row_id = 0;
            loc_id = string.Empty;
            item_id = 0;
            iarc_id = string.Empty;
            qty_o = 0;
            adj_qty = 0;
            create_dtim = DateTime.MinValue;
            create_user = 0;
            doc_no = string.Empty;
            po_id = string.Empty;
            made_dt = DateTime.MinValue;
            cde_dt = DateTime.MinValue;
            remarks = string.Empty;
            c_made_dt = DateTime.MinValue;
            c_cde_dt = DateTime.MinValue;
        }
    }
}
