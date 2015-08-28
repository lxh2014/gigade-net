using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class InspectionReportQuery : InspectionReport
    {
        public string create_user { get; set; }
        public string update_user { get; set; }
        public uint brand_id { get; set; }
        public string brand_name { get; set; }
        public string product_name { get; set;}
        public string name_code { get; set; }
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public int last_day { get; set; }
        public int brand_status { get; set; }
        public string certificate_type1_name { get; set; }
        public string certificate_type2_name { get; set; }
        public string certificate_filename_string { get; set; }
        public int search_date { get; set; }
        public string code1 { get;set;}
           public string code2 { get;set;}
           public int sort { get; set; }
           public int old_sort { get; set; }
        
        public InspectionReportQuery()
        {
            create_user = string.Empty;
            update_user = string.Empty;
            brand_id = 0;
            brand_name = string.Empty;
            product_name = string.Empty;
            name_code = string.Empty;
            start_time = DateTime.MinValue;
            end_time = DateTime.MinValue;
            last_day = 0;
            brand_status = 0;
            certificate_type1_name = string.Empty;
            certificate_type2_name = string.Empty;
            certificate_filename_string = string.Empty;
            search_date = 0;
            sort = 0;
            old_sort = 0;
        }
    }
}
