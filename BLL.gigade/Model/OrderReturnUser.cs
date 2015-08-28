using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class OrderReturnUser : PageBase
    {
        public uint user_return_id { get; set; }
        public uint detail_id { get; set; }
        public uint return_reason { get; set; }
        public uint gift { get; set; }
        public uint temp_status { get; set; }
        public string user_note { get; set; }
        public uint return_zip { get; set; }
        public string return_address { get; set; }
        public string bank_name { get; set; }
        public string bank_branch { get; set; }
        public string bank_account { get; set; }
        public string account_name { get; set; }
        public uint user_return_createdate { get; set; }
        public uint user_return_updatedate { get; set; }
        public string user_return_ipfrom { get; set; }
        public OrderReturnUser()
        {
            user_return_id = 0;
            detail_id = 0;
            return_reason = 1;
            gift = 0;
            temp_status = 0;
            user_note = string.Empty;
            return_zip = 0;
            return_address = string.Empty;
            bank_name = string.Empty;
            bank_branch = string.Empty;
            bank_account = string.Empty;
            account_name = string.Empty;
            user_return_createdate = 0;
            user_return_updatedate = 0;
            user_return_ipfrom = string.Empty;

        }
    }
}
