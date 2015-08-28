using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class OrderReturnUserQuery : OrderReturnUser
    {
        public DateTime user_return_createdates { get; set; }
        public DateTime user_return_updatedates { get; set; }
        public string selecttype { get; set; }
        public string searchcon { get; set; }
        public string seldate { get; set; }
        public int timestart { get; set; }
        public int timeend { get; set; }
        public uint order_id { get; set; }
        public uint return_id { get; set; }

        //public uint slave_status { get; set; }
        public uint return_createdate { get; set; }
        public uint return_updatedate { get; set; }
        public string return_ipfrom { get; set; }
        public uint item_vendor_id { get; set; }
        public uint return_status { get; set; }
        public uint order_status { get; set; }
        public uint order_updatedate { get; set; }
        public string order_ipfrom { get; set; }
        public uint order_date_close { get; set; }
        public uint order_date_cancel { get; set; }
        public uint detail_status { get; set; }
        //public uint invoice_status { get; set; }
        //public string order_invoice { get; set; }


        public OrderReturnUserQuery()
        {
            user_return_createdates = DateTime.MinValue;
            user_return_updatedates = DateTime.MinValue;
            selecttype = string.Empty;
            searchcon = string.Empty;
            seldate = string.Empty;
            timestart = 0;
            timeend = 0;
            order_id = 0;
            return_id = 0;

            //slave_status = 0;
            //return_createdate = 0;
            //return_updatedate = 0;
            //return_ipfrom = string.Empty;
            //item_vendor_id = 0;
            //return_status = 0;
            //invoice_status = 0;
            //order_invoice = string.Empty;
        }
    }
}
