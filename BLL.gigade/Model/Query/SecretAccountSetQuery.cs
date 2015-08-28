using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Model.Query
{ 
    public class SecretAccountSetQuery : SecretAccountSet
    {
        public string user_username { get; set; }
        public string user_email { get; set; }
        public int type { get; set; }
        public string content { get; set; }
        public SecretAccountSetQuery()
        {
            user_username = string.Empty;
            user_email = string.Empty;
            type = 0;
            content = string.Empty;
        }
    }
}
