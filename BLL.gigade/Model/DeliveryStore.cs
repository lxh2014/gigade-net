using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    /// <summary>
    /// 超商店家
    /// </summary>
    public class DeliveryStore:PageBase
    {
        public int rowid { get; set; }
        public int delivery_store_id { get; set; }
        public string big { get; set; }
        public string bigcode { get; set; }
        public string middle { get; set; }
        public string middlecode { get; set; }
        public string small { get; set; }
        public string smallcode { get; set; }
        public string store_id { get; set; }
        public string store_name { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public int status { get; set; }

        public DeliveryStore()
        {
            rowid = 0;
            delivery_store_id = 0;
            big = string.Empty;
            bigcode = string.Empty;
            middle = string.Empty;
            middlecode = string.Empty;
            small = string.Empty;
            smallcode = string.Empty;
            store_id = string.Empty;
            store_name = string.Empty;
            address = string.Empty;
            phone = string.Empty;
            status = 1;
        }
    }
}
