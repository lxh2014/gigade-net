using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ConfigQuery : Config
    {
        public string chaek { get; set; }
        public string name { get; set; }
        public string email { get; set; }


        public ConfigQuery()
        {
            chaek = string.Empty;
            name = string.Empty;
            email = string.Empty;
        }
    }
}