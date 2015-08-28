using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class Iloc : PageBase
    {
        public int row_id { get; set; }
        public int dc_id { get; set; }
        public int whse_id { get; set; }
        public string loc_id { get; set; }
        public string llts_id { get; set; }
        public string bkfill_loc { get; set; }
        public string ldim_id { get; set; }
        public string ldes_id { get; set; }
        public int x_coord { get; set; }
        public int y_coord { get; set; }
        public int z_coord { get; set; }
        public int bkfill_x_coord { get; set; }
        public int bkfill_y_coord { get; set; }
        public int bkfill_z_coord { get; set; }
        public string lsta_id { get; set; }
        public int sel_stk_pos { get; set; }
        public string sel_seq_loc { get; set; }
        public int sel_pos_hgt { get; set; }
        public int rsv_stk_pos { get; set; }
        public int rsv_pos_hgt { get; set; }
        public int stk_pos_dep { get; set; }
        public int stk_lmt { get; set; }
        public int stk_pos_wid { get; set; }
        public int lev { get; set; }
        public string lhnd_id { get; set; }
        public string ldsp_id { get; set; }
        public int create_user { get; set; }
        public DateTime create_dtim { get; set; }
        public string comingle_allow { get; set; }
        public int change_user { get; set; }
        public DateTime change_dtim { get; set; }
        public string lcat_id { get; set; }
        public int space_remain { get; set; }
        public int max_loc_wgt { get; set; }
        public int loc_status { get; set; }
        public string hash_loc_id { get; set; }
        public Iloc()
        {
            row_id = 0;
            dc_id = 0;
            whse_id = 0;
            loc_id = string.Empty;
            llts_id = string.Empty;
            bkfill_loc = string.Empty;
            ldim_id = string.Empty;
            ldes_id = string.Empty;
            x_coord = 0;
            y_coord = 0;
            z_coord = 0;
            bkfill_x_coord = 0;
            bkfill_y_coord = 0;
            bkfill_z_coord = 0;
            lsta_id = "F";
            sel_stk_pos = 0;
            sel_seq_loc = string.Empty;
            sel_pos_hgt = 0;
            rsv_stk_pos = 0;
            rsv_pos_hgt = 0;
            stk_pos_dep = 0;
            stk_lmt = 0;
            stk_pos_wid = 0;
            lev = 0;
            lhnd_id = string.Empty;
            ldsp_id = string.Empty;
            create_user = 0;
            create_dtim = DateTime.MaxValue;
            comingle_allow = string.Empty;
            change_user = 0;
            change_dtim = DateTime.MaxValue;
            lcat_id = string.Empty;
            space_remain = 0;
            max_loc_wgt = 0;
            loc_status = 0;
            hash_loc_id = string.Empty;
        }
    }
}
