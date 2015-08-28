using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
namespace BLL.gigade.Model.Query
{
    public class MailUserQuery : MailUser
    {
        public string row_id_in { set; get; }//批次刪除用戶信息
        public string create_user_name { set; get; }
        public string update_user_name { set; get; }
        public string nameemail { get; set; }
        public MailUserQuery()
    {
            row_id_in = string.Empty;
            create_user_name = string.Empty;
            update_user_name = string.Empty;
            nameemail = string.Empty;
        }
    }
    }
