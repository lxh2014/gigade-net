using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public class OrderPaymentNccc:PageBase
    {
       public uint nccc_id { get; set; }
       public uint order_id { get; set; }
       public string merchantid { get; set; }
       public string terminalid { get; set; }
       public string orderid { get; set; }
       public string pan { get; set; }
       public string bankname { get; set; }
       public string transcode { get; set; }
       public string transmode { get; set; }
       public string transdate { get; set; }
       public string transtime { get; set; }
       public string transamt { get; set; }
       public string approvecode { get; set; }
       public string responsecode { get; set; }
       public string responsemsg { get; set; }
       public string installtype { get; set; }
       public string install { get; set; }
       public string firstamt { get; set; }
       public string eachamt { get; set; }
       public string fee { get; set; }
       public string redeemtype { get; set; }
       public string redeemused { get; set; }
       public string redeembalance { get; set; }
       public string creditamt { get; set; }
       public string riskmark { get; set; }
       public string foreign1 { get; set; }
       public string secure_status { get; set; }
       public uint nccc_createdate { get; set; }
       public string nccc_ipfrom { get; set; }
       public string post_data { get; set; }
       public OrderPaymentNccc()
       {
           nccc_createdate = 0;
           nccc_ipfrom = string.Empty;
       }
    }
}
