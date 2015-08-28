/* 
*zhengzhou
 * 文件名称：SinopacDetail 
* 創建者：chaojie1124j
 * 完成日期：2014/12/1 05:06 PM
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public  class SinopacDetail
    {
       public int sinopac_detail_id { set; get; }
       public int order_id { set; get; }
       public string sinopac_id { set; get; }
       public int pay_amount { set; get; }
       public int entday { set; get; }
       public int txday { set; get; }
       public string  trmseq { set; get; }
       public int txtno { set; get; }
       public int error { set; get; }
       public int sinopac_createdate { set; get; }
       public SinopacDetail()
       {
           sinopac_detail_id = 0;
           order_id = 0;
           sinopac_id = string.Empty;
           pay_amount=0;
           entday = 0;
           txday = 0;
           trmseq = string.Empty;
           txtno = 0;
           error = 0;
           sinopac_createdate = 0;
       }
    }
}
