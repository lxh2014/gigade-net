using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ProductItemMapQuery : PageBase
    {
        public conditionNo condition { get; set; }
        public string content { get; set; }
        public int ChannelId { get; set; }

        public ProductItemMapQuery()
        {
            condition = 0;
            content = string.Empty;
            ChannelId = 0;
        }

        public enum conditionNo
        {
            product_id = 1,
            item_id = 2,
            user_id = 3,
            channel_name_full = 4,
            product_name = 5,
            channel_detail_id = 6,
            channelid_detailid = 7
        }
    }

}
