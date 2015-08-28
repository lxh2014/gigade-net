using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public class MailSend:PageBase
    {
       public int rowid { get; set; }
       public string mailfrom { get; set; }
       public string mailto { get; set; }
       public string subject { get; set; }
       public string mailbody { get; set; }
       public bool status { get; set; }
       public string kuser { get; set; }
       public DateTime kdate { get; set; }
       public DateTime senddate { get; set; }
       public string source { get; set; }
       public int weight { get; set; }
       public MailSend()
       {
           this.rowid = 0;
           this.mailfrom = string.Empty;
           this.mailto = string.Empty;
           this.subject = string.Empty;
           this.mailbody = string.Empty;
           this.status = false;
           this.kuser = string.Empty;
           this.kdate = DateTime.Now;
           this.senddate = DateTime.MinValue;
           this.source = string.Empty;
           this.weight = 0;

       }
    }
}
