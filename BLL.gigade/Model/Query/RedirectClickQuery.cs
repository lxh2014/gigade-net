using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class RedirectClickQuery : RedirectClick
    {
        public int startdate { get; set; }
        public int enddate { get; set; }
        public string redirectstr { get; set; }
        public  uint redirect_id_groupID { get; set; }
        public Int64 sum_click { set; get; }//chaojie1124j添加于2015/02/04  08:57 為了統計點擊量
        public RedirectClickQuery()
        {
            startdate = 0;
            enddate = 0;
            redirectstr = string.Empty;
            redirect_id_groupID = 0;
            sum_click = 0;
        }
    }
}
