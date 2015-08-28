using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class SecretAccountSet : PageBase
    {
        public int id { get; set; }
        public uint user_id { get; set; }
        public string secret_pwd { get; set; }
        public DateTime createdate { get; set; }
        public DateTime updatedate { get; set; }
        public int status { get; set; }
        public int pwd_status { get; set; }
        public int user_login_attempts { get; set; }
        public string ipfrom { get; set; }
        public int secret_limit { get; set; }
        public int secret_count { get; set; }

        public SecretAccountSet()
        {
            id = 0;
            user_id = 0;
            secret_pwd = string.Empty;
            createdate = DateTime.MinValue;
            updatedate = DateTime.MinValue;
            status = -1;
            pwd_status = 0;
            ipfrom = string.Empty;
            user_login_attempts = -1;
            secret_limit = -1;
            secret_count = -1;
        }

    }
}
