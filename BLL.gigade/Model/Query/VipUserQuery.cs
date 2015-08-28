using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class VipUserQuery:VipUser
    {
        public string screatedate { set; get; }
        public DateTime reg_date { set; get; }
        public string serchtype { set; get; }
        public string content { set; get; }
        public string birthday { set; get; }
        public string mytype { set; get; }
        public int search_state { set; get; } //黑名單管理 add by jiaohe
        public DateTime start { set; get; } //黑名單管理 add by jiaohe
        public DateTime end { set; get; } //黑名單管理 add by jiaohe
        public string createUsername { set; get; } //黑名單管理 add by jiaohe
        public string updateUsername { set; get; } //黑名單管理 add by jiaohe
        
        public VipUserQuery()
        {
            serchtype = string.Empty;
            content = string.Empty;
            screatedate =string.Empty;
            reg_date = DateTime.MinValue;
            birthday = string.Empty;
            mytype = string.Empty;
            search_state = 2;
            start = DateTime.MinValue;
            end = DateTime.MinValue;
            createUsername = string.Empty;
            updateUsername = string.Empty;
        }
    }
}
