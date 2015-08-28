using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class Iupc : PageBase
    {
        public int row_id { set; get; }
        public string upc_id { get; set; }
        public uint item_id { get; set; }
        public string suppr_upc { get; set; }
        public DateTime lst_ship_dte { get; set; }
        public DateTime lst_rct_dte { get; set; }
        public DateTime create_dtim { get; set; }
        public int create_user { get; set; }
        public string upc_type_flg { get; set; }
        public Iupc()
        {
            row_id = 0;
            upc_id = string.Empty;
            item_id = 0;
            suppr_upc = string.Empty;
            lst_ship_dte = DateTime.Now;
            lst_rct_dte = DateTime.Now;
            create_dtim = DateTime.Now;
            create_user =0;
            upc_type_flg = string.Empty;

        }
    }
}
