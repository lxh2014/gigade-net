using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class Ticket:PageBase
    {
        public int ticket_id { get; set; }
        public int type { get; set; }
        public int freight_set { get; set; }//運送方式
        public int export_id { get; set; }
        public int import_id { get; set; }
        public int delivery_store { get; set; }
        public int warehouse_status { get; set; }
        public int ticket_status { get; set; }
        public int seized_status { get; set; }
        public int ship_status { get; set; }
        public int Freight_status { get; set; }
        public int verifier { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public int work_status { get; set; }
        public Ticket()
        {
            ticket_id = 0;
            type = 0;
            freight_set = 0;
            export_id = 0;
            import_id = 0;
            delivery_store = 1;
            warehouse_status = 0;
            ticket_status = 0;
            seized_status = 0;
            ship_status = 0;
            Freight_status = 0;
            verifier = 0;
            created = DateTime.MinValue;
            modified = DateTime.MinValue;
            work_status = 0;
        }
    }
}
