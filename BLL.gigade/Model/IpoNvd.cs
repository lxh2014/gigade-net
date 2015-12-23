using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class IpoNvd:PageBase
    {
        public int row_id { get; set; }
        public string work_id {get;set;}
        public string ipo_id {get;set;}
        public uint item_id {get;set;}
        public int ipo_qty {get;set;}
        public int out_qty {get;set;}
        public int com_qty {get;set;}
        public DateTime cde_dt {get;set;}
        public DateTime made_date {get;set;}
        public string work_status {get;set;}
        public int create_user {get;set;}
        public DateTime create_datetime {get;set;}
        public int modify_user {get;set;}
        public DateTime modify_datetime {get;set;}

        public IpoNvd()
        {
            row_id = 0;
            work_id = string.Empty;
            ipo_id = string.Empty;
            item_id = 0;
            ipo_qty = 0;
            out_qty = 0;
            com_qty = 0;
            cde_dt = DateTime.MinValue;
            made_date = DateTime.MinValue;
            work_status = string.Empty;
            create_user = 0;
            create_datetime = DateTime.MinValue;
            modify_user = 0;
            modify_datetime = DateTime.MinValue;
        }
    }
}
