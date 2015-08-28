using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    /// <summary>
    /// 品類管理目錄
    /// </summary>
    public class StatisticCm : PageBase
    {
        public int category_id { get; set; }
        public int category_father_id { get; set; }
        public int category_status { get; set; }
        public string category_code { get; set; }
        public string category_name { get; set; }
        public int category_sort { get; set; }
        public DateTime category_createdate { get; set; }
        public DateTime category_updatedate { get; set; }
        public string update_by { get; set; }

        public StatisticCm()
        {
            category_id = 0;
            category_father_id = 0;
            category_status = 1;
            category_code = string.Empty;
            category_name = string.Empty;
            category_sort = 0;
            category_createdate = DateTime.MinValue;
            category_updatedate = DateTime.MinValue;
            update_by = string.Empty;
        }
    }
}
