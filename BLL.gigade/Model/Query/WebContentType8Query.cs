﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
   public class WebContentType8Query:WebContentType8
    {
        public string site_name { get; set; }
        public string page_name { get; set; }
        public string area_name { get; set; }



        public WebContentType8Query()
        {
            site_name = string.Empty;
            page_name = string.Empty;
            area_name = string.Empty;
        }
    }
}
