using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductComment : PageBase
    {
       public int comment_id { get; set; }
       public int order_id { get; set; }
       public uint product_id { get; set; }
       public uint user_id { get; set; }
       public int is_show_name { get; set; }
       public DateTime create_time { get; set; }
       public ProductComment()
       {
           comment_id = 0;
           product_id = 0;
           user_id = 0;
           is_show_name = 0;
       }
    }
}
