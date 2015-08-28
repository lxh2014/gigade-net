using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ItemIpoCreateLog:PageBase
    {
        public int row_id { set; get; }
        public int item_id { set; get; }
        public DateTime create_datetime { set; get; }
        public int create_user { set; get; }
        public int log_status { set; get; }
        public ItemIpoCreateLog()
        {
            row_id = 0;
            item_id = 0;
            create_datetime = DateTime.MinValue;
            create_user = 0;
            log_status = 1;
        
        }
    }
}
