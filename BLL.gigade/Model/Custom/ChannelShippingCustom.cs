using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
namespace BLL.gigade.Model.Custom
{
    public class ChannelShippingCustom:ChannelShipping
    {
        public string parameterCode { get; set; }
        public string parameterName { get; set; }
        public int sort { get; set; }
        public ChannelShippingCustom()
        {
            parameterCode = string.Empty;
            parameterName = string.Empty;
            sort = 0;
        }
    }
}
