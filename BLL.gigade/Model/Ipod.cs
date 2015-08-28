using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public  class Ipod :PageBase
    {
        public int row_id { set; get; }
        public string po_id { set; get; }
        public int pod_id { set; get; }
        public string plst_id { set; get; }
        public string bkord_allow { set; get; }
        public int cde_dt_incr { set; get; }
        public int cde_dt_var { set; get; }
        public int cde_dt_shp { set; get; }
        public string pwy_dte_ctl { set; get; }
        public int qty_ord { set; get; }
        public int qty_damaged { set; get; }
        public int qty_claimed { set; get; }
        public string promo_invs_flg { set; get; }
        public string prod_id { set; get; }
        public int create_user { set; get; }
        public DateTime create_dtim { set; get; }
        public int change_user { set; get; }
        public DateTime change_dtim { set; get; }
        public double req_cost { set; get; }
        public double off_invoice { set; get; }
        public double new_cost { set; get; }
        public int freight_price { set; get; }
        public Ipod()
        {
            row_id = 0;
            po_id = string.Empty;
            pod_id = 0;
            plst_id = string.Empty;
            bkord_allow = string.Empty;
            cde_dt_incr =0;
            cde_dt_var = 0;
            cde_dt_shp = 0;
            pwy_dte_ctl = string.Empty;
            qty_ord = 0;
            qty_damaged = 0;
            qty_claimed = 0;
            promo_invs_flg = string.Empty;
            prod_id = string.Empty;
            create_user = 0;
            create_dtim = DateTime.MinValue;
            change_user = 0;
            change_dtim = DateTime.MinValue;
            req_cost = 0;
            off_invoice = 0;
            new_cost = 0;
            freight_price = 0;
        }
    }
}
