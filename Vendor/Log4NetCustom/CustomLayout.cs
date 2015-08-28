using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net.Util;
using System.IO;
using log4net.Layout.Pattern;
using System.Collections;
using log4net.Core;
namespace Vendor.Log4NetCustom
{
    public class CustomLayout : log4net.Layout.PatternLayout
    {
        public CustomLayout()
        {
            this.AddConverter("property", typeof(MyMessagePatternConverter));
        }
    }
}