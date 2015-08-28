using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ItemIpoCreateLogQuery : ItemIpoCreateLog
    {
        public string item_id_in { set; get; }
        public ItemIpoCreateLogQuery()
        {
            item_id_in = string.Empty;
        }
    }
}
