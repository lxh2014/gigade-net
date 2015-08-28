using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class Aseld:PageBase
    {
        public int seld_id { get; set; }
        public int dc_id { get; set; }
        public int whse_id { get; set; }
        public int ord_id { get; set; }
        public int sgmt_id { get; set; }
        public int ordd_id { get; set; }
        public string cust_id { get; set; }
        public uint item_id { get; set; }
        public int prdd_id { get; set; }
        public string assg_id { get; set; }
        public string sety_id { get; set; }
        public int unit_ship_cse { get; set; }
        public int prod_cub { get; set; }
        public int prod_wgt { get; set; }
        public int prod_qty { get; set; }
        public string sel_loc { get; set; }
        public int ckpt_id { get; set; }
        public Int64 curr_pal_no { get; set; }
        public int cse_lbl_lmt { get; set; }
        public string wust_id { get; set; }
        public int lic_plt_id { get; set; }
        public string description { get; set; }
        public string prod_sz { get; set; }
        public string hzd_ind { get; set; }
        public string cust_name { get; set; }
        public string order_type_id { get; set; }
        public string stg_dcpt_id { get; set; }
        public string stg_dcpd_id { get; set; }
        public Int64 invc_id { get; set; }
        public string route_id { get; set; }
        public int stop_id { get; set; }
        public int batch_id { get; set; }
        public int batch_seq { get; set; }
        public DateTime start_dtim { get; set; }
        public DateTime complete_dtim { get; set; }
        public DateTime change_dtim { get; set; }
        public int change_user { get; set; }
        public DateTime create_dtim { get; set; }
        public int create_user { get; set; }
        public int ord_msg_id { get; set; }
        public string door_dcpd_id { get; set; }
        public string door_dcpt_id { get; set; }
        public string catch_wgt_cntl { get; set; }
        public string lot_no { get; set; }
        public string commodity_type { get; set; }
        public string sect_id { get; set; }
        public string ucn { get; set; }
        public string hzd_class { get; set; }
        public string pkde_id { get; set; }
        public DateTime ord_rqst_del_dt { get; set; }
        public DateTime ord_rqst_del_tim { get; set; }
        public string spmd_id { get; set; }
        public string flow_dcpt_id { get; set; }
        public string flow_dcpd_id { get; set; }
        public string flow_assg_flg { get; set; }
        public string sel_seq_loc { get; set; }
        public int out_qty { get; set; }
        public string eqpt_class_id { get; set; }
        public int sel_x_coord { get; set; }
        public int sel_y_coord { get; set; }
        public int sel_z_coord { get; set; }
        public string upc_id { get; set; }
        public int ft_id { get; set; }
        public int ftd_id { get; set; }
        public int act_pick_qty { get; set; }
        public int ord_qty { get; set; }
        public string family_group { get; set; }
        public int deliver_id { get; set; }
        public string deliver_code { get; set; }

        public Aseld()
        {
            dc_id = 1;
            whse_id = 1;
            ord_id = 0;
            sgmt_id = 1;
            ordd_id = 0;
            cust_id = string.Empty;
            item_id = 0;
            prdd_id = 1;
            assg_id = string.Empty;
            sety_id = string.Empty;
            unit_ship_cse = 0;
            prod_cub = 0;
            prod_wgt = 0;
            prod_qty = 0;
            sel_loc = string.Empty;
            ckpt_id = 0;
            curr_pal_no = 0;
            cse_lbl_lmt = 0;
            wust_id = string.Empty;
            lic_plt_id = 0;
            description = string.Empty;
            prod_sz = string.Empty;
            hzd_ind = string.Empty;
            cust_name = string.Empty;
            order_type_id = string.Empty;
            stg_dcpt_id = string.Empty;
            stg_dcpd_id = string.Empty;
            invc_id = 0;
            route_id = string.Empty;
            stop_id = 0;
            batch_id = 0;
            batch_seq = 0;
            start_dtim = DateTime.MinValue;
            complete_dtim = DateTime.MinValue;
            change_dtim = DateTime.Now;
            change_user = 0;
            create_dtim = DateTime.Now;
            create_user = 0;
            ord_msg_id = 0;
            door_dcpd_id = string.Empty;
            door_dcpt_id = string.Empty;
            catch_wgt_cntl = string.Empty;
            lot_no = string.Empty;
            commodity_type = string.Empty;
            sect_id = string.Empty;
            ucn = string.Empty;
            hzd_class = string.Empty;
            pkde_id = string.Empty;
            ord_rqst_del_dt = DateTime.MinValue;
            ord_rqst_del_tim = DateTime.MinValue;
            spmd_id = string.Empty;
            flow_dcpt_id = string.Empty;
            flow_dcpd_id = string.Empty;
            flow_assg_flg = string.Empty;
            sel_seq_loc = string.Empty;
            out_qty = 0;
            eqpt_class_id = string.Empty;
            sel_x_coord = 0;
            sel_y_coord = 0;
            sel_z_coord = 0;
            upc_id = string.Empty;
            ft_id = 0;
            ftd_id = 0;
            act_pick_qty = 0;
            ord_qty = 0;
            family_group = string.Empty;
            deliver_id = 0;
            deliver_code = string.Empty;
        }

    }
}
