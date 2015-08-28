using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;
namespace BLL.gigade.Model
{
    [DBTableInfo("edm_email")]
    public class UserEdm : PageBase
    {
        public uint email_id { get; set; }
        public string email_name { get; set; }
        public string email_address { get; set; }
        public uint email_check { get; set; }
        public uint email_sent { get; set; }
        public uint email_user_unknown { get; set; }
        public uint email_click { get; set; }
        public uint email_createdate { get; set; }
        public uint email_updatedate { get; set; }
    }
}
