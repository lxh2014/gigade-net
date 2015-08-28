using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;

namespace BLL.gigade.Model
{
    [DBTableInfo("config")]
    public class Config : PageBase
    {

        public int id { get; set; }
        public string config_name { get; set; }
        public string config_value { get; set; }
        public string config_content { get; set; }

        public Config()
        {
            id = 0;
            config_name = string.Empty;
            config_value = string.Empty;
            config_content = string.Empty;
        }
    }
}