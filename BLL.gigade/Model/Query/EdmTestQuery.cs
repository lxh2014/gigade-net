using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
     public class EdmTestQuery:EdmTest
    {
         public uint email_id { get; set; }
         public string email_address { get; set; }
         public string selectType { get; set; }   //查詢條件
         public string search_con { get; set; }   //查詢關鍵字
         public string dateCon { get; set; }   //選擇日期狀態
         public uint date_start { get; set; }   //開始時間
         public uint date_end { get; set; }   //結束時間

       
         public EdmTestQuery()
         {
             email_id = 0;
             email_address = string.Empty;
             selectType = string.Empty;
             search_con = string.Empty;
             dateCon = string.Empty;
             date_start = 0;
             date_end = 0;
         }

    }
}
