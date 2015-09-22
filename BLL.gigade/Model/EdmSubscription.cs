using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public class EdmSubscription:PageBase
    {
        public int group_id { get; set; }//電子報群組代碼
        public int user_id { get; set; }//會員代碼

        public EdmSubscription()
        {
            group_id = 0;
            user_id = 0;
        }
    }
}
