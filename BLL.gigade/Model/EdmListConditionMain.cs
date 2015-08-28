using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EdmListConditionMain:PageBase
    {
        public int elcm_id { get; set; }//主鍵
        public int elcm_creator_id { get; set; }//建立者ID
        public string elcm_name { get; set; }//條件名稱
        public DateTime elcm_created { get; set; }
        public DateTime elcm_modified { get; set; }
        public EdmListConditionMain()
        {
            elcm_id = 0;
            elcm_creator_id = 0;
            elcm_name = string.Empty;
            elcm_created = DateTime.MinValue;
            elcm_modified = DateTime.Now;
        } 
    }
}
