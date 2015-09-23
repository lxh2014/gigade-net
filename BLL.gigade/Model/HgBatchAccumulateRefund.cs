using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public class HgBatchAccumulateRefund
    {

       public uint order_id { get; set; }
       public string head { get; set; }
       public string card_no { get; set; }
       public string card_checksum { get; set; }
       public string enc_idno { get; set; }
       public string checksum { get; set; }
       public int transaction_time { get; set; }
       public string merchant_pos { get; set; }
       public string terminal_pos { get; set; }
       public int refund_point { get; set; }
       public string category_id { get; set; }
       public string order_note { get; set; }
       public string wallet { get; set; }
       public int batch_import_time { get; set; }
       public string batch_error_code { get; set; }
       public int batch_status { get; set; }
       public int created_time { get; set; }
       public int modified_time { get; set; }
       public int billing_checked { get; set; }
       public HgBatchAccumulateRefund()
       {
           order_id = 0;
           head = string.Empty;
           card_no = string.Empty;
           card_checksum = string.Empty;
           enc_idno = string.Empty;
           checksum = string.Empty;
           transaction_time = 0;
           merchant_pos = string.Empty;
           terminal_pos = string.Empty;
           refund_point = 0;
           category_id = string.Empty;
           order_note = string.Empty;
           batch_import_time =0;
           batch_error_code = string.Empty;
           batch_status =0;
           created_time = 0;
           modified_time = 0;
           billing_checked = 0;
       }
    }
}
