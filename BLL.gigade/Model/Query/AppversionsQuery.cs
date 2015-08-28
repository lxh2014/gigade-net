using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class AppversionsQuery : Appversions
    {
        public DateTime releasedateQuery { get; set; }
        public string isAddOrEidt { get; set; }
        public AppversionsQuery()
        {
            releasedateQuery = DateTime.Now;
            isAddOrEidt = string.Empty;
        }
    }
}
