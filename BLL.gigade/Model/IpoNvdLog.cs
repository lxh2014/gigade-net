using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class IpoNvdLog:PageBase
    {
        public int row_id { get; set; }
        public string work_id {get;set;}
        public string ipo_id {get;set;}
        public int add_qty {get;set;}
        public DateTime cde_date {get;set;}
        public uint item_id {get;set;}
        public DateTime made_date {get;set;}
        public int create_user {get;set;}
        public DateTime create_datetime {get;set;}

        public IpoNvdLog()
        {
            row_id = 0;
            work_id = string.Empty;
            ipo_id = string.Empty;
            item_id = 0;
            add_qty = 0;
            cde_date = DateTime.MinValue;
            made_date = DateTime.MinValue;
            create_user = 0;
            create_datetime = DateTime.MinValue;
        }
    }
}
