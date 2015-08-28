using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
   public class EmsActualQuery : EmsActual
    {
        
       public string department_name { get; set; }
       public DateTime create_time { get; set; }
       public string user_username { get; set; }
       public int user_userid { get; set; }
       public int searchdate { get; set; }
       public DateTime date { get; set; }
       public DateTime predate { get; set; }
       public string EmsActual { get; set; }
       public int EmsValue { get; set; }
       public int insertType { get; set; }
       public string department_code_insert { get; set; }
       public EmsActualQuery()
       {
           department_name = string.Empty;
           create_time = DateTime.MinValue;
           user_username = string.Empty;
           user_userid =2;
           searchdate = 0;
           date = DateTime.MinValue;
           predate = DateTime.MinValue;
           EmsActual = string.Empty;//字段名
           EmsValue = 0;//值
           insertType = 2;
           department_code_insert = string.Empty;
       }
    }
}
