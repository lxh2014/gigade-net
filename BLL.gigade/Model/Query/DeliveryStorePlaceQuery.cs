using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class DeliveryStorePlaceQuery : DeliveryStorePlace
    {
        public string big { get; set; }//區域名
        public string parameterName { get; set; }//物流公司名
        public string create_username { get; set; }
        public string modify_username { get; set; }
        public string dsp_ids { get; set; }

        public DeliveryStorePlaceQuery()
        {
            big = string.Empty;
            parameterName = string.Empty;
            create_username = string.Empty;
            modify_username = string.Empty;
            dsp_ids = string.Empty;
        }
    }
}
