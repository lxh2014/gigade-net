using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ScheduleItem
    {
        public uint id { get; set; }
        public int schedule_Id { get; set; }
        public string item_name { get; set; }
        public int type { get; set; }
        public int key1 { get; set; }
        public int key2 { get; set; }
        public int key3 { get; set; }
        public string value1 { get; set; }
        public string value2 { get; set; }
        public string value3 { get; set; }

        public ScheduleItem() 
        {
            id = 0;
            schedule_Id = 0;
            item_name = string.Empty;
            type = 0;
            key1 = 0;
            key2 = 0;
            key3 = 0;
            value1 = string.Empty;
            value2 = string.Empty;
            value3 = string.Empty;
        }
    }
}
