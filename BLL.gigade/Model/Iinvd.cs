using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public class Iinvd:PageBase
    {
       public int row_id { get; set; }
       public decimal lic_plt_id { get; set; }
       public int dc_id { get; set; }
       public int whse_id { get; set; }
       public string po_id { get; set; }
       public int plas_id { get; set; }
       public int prod_qty { get; set; }
       public int rcpt_id { get; set; }
       public string lot_no { get; set; }
       public int hgt_used { get; set; }
       public int create_user { get; set; }
       public DateTime create_dtim { get; set; }
       public int change_user { get; set; }
       public DateTime change_dtim { get; set; }
       public DateTime cde_dt { get; set; }
       public string ista_id { get; set; }
       public DateTime receipt_dtim { get; set; }
       public int stor_ti { get; set; }
       public int stor_hi { get; set; }
       public string inv_pos_cat { get; set; }
       public int qity_id { get; set; }
       public string plas_loc_id { get; set; }
       public uint item_id { get; set; }
       public int plas_prdd_id { get; set; }
       public DateTime made_date { get; set;}
       public int st_qty { get; set; }

       public Iinvd ()
       {
           row_id = 0;
           lic_plt_id = 0;
           dc_id = 0;
           whse_id = 0;
           po_id = string.Empty;
           plas_id = 0;
           prod_qty = 0;
           rcpt_id = 0;
           lot_no = string.Empty;
           hgt_used = 0;
           create_user = 0;
           create_dtim = DateTime.MinValue;
           change_user = 0;
           change_dtim = DateTime.MinValue;
           cde_dt = DateTime.MinValue;
           ista_id = string.Empty;
           receipt_dtim = DateTime.MinValue;
           stor_ti = 0;
           stor_hi = 0;
           inv_pos_cat = string.Empty;
           qity_id = 0;
           plas_loc_id = string.Empty;
           item_id = 0;
           plas_prdd_id = 0;
           made_date = DateTime.MinValue;
           st_qty = 0;//復盤數量
       }
    }
}
