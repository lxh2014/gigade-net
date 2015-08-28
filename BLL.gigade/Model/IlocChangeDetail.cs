using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class IlocChangeDetail:PageBase
    {
        public int icd_id { get; set; }
        public string icd_work_id { get; set; }
        public uint icd_item_id { get; set; }
        public string icd_old_loc_id { get; set; }
        public string icd_new_loc_id { get; set; }
        public DateTime icd_create_time { get; set; }
        public int icd_create_user { get; set; }
        public string icd_remark { set; get; }
        public string icd_msg { set; get; }
        public string icd_status { set; get; }
        public DateTime icd_modify_time { set; get; }
        public int icd_modify_user { set; get; }


        public IlocChangeDetail()
        {
            icd_id = 0;
            icd_work_id = string.Empty;
            icd_item_id = 0;
            icd_old_loc_id = string.Empty;
            icd_new_loc_id = string.Empty;
            icd_create_time = DateTime.MinValue;
            icd_create_user = 0;
            icd_remark = string.Empty;
            icd_msg = string.Empty;
            icd_status = string.Empty;
            icd_modify_time = DateTime.MinValue;
            icd_modify_user = 0;
        }
    }
}
