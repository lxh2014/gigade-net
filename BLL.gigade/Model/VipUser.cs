using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Model
{
   public class VipUser:Custom.Users
    {
       public uint v_id{set;get;}
       public string vuser_email { set; get; }
       public uint User_Id { set; get; }
       public uint status { set; get; }
       public uint group_id { set; get; }
       public string emp_id { set; get; }
       public uint createdate { set; get; }
       public uint source { set; get; }
       public uint create_id { set; get; }
       public uint update_id { set; get; }
       public uint updatedate { set; get; }
       public VipUser()
       {
           v_id = 0;
           vuser_email = string.Empty;
           User_Id = 0;
           status = 0;
           group_id = 0;
           emp_id = string.Empty;
           createdate = 0;
           source = 0;
           create_id = 0;
           update_id = 0;
           updatedate = 0;
       }

    }
}
