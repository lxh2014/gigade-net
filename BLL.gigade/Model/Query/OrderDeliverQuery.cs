using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class OrderDeliverQuery : OrderDeliver
    {
        public uint order_id { get; set; }
        public string vendor_name_simple { get; set; }
        public int deliverstart { get; set; }
        public int deliverend { get; set; }
        public DateTime delivertime { get; set; }
        public DateTime delivercre { get; set; }
        public DateTime deliverup { get; set; }
        public string serchs { get; set; }
        public string serchcontent { get; set; }
        public int seldate { get; set; }
        public int selven { get; set; }
        public string search { get; set; }
        public string deliver_name { get; set; }
        #region changjian0408j 于2014/10/29 添加
        public uint vendor_id { get; set; }
        public string vendor_name_full { get; set; }

        #endregion

        public string Deliver_Store_Url { set; get; }
        public OrderDeliverQuery()
        {
            order_id = 0;
            vendor_name_simple = string.Empty;
            deliverstart = 0;
            deliverend = 0;
            delivertime = DateTime.MinValue;
            deliverup = DateTime.MinValue;
            serchs = string.Empty;
            serchcontent = string.Empty;
            seldate = 0;
            selven = 0;
            search = string.Empty;
            deliver_name = string.Empty;
            Deliver_Store_Url = string.Empty;
        }
    }
}
