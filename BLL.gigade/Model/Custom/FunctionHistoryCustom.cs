using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class FunctionHistoryCustom:FunctionHistory
    {
        public string FunctionName { get; set; }
        public string User_Name { get; set; }
        public FunctionHistoryCustom()
        {
            FunctionName = string.Empty;
            User_Name = string.Empty;
        }
    }
}
