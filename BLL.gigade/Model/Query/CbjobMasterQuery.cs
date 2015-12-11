using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class CbjobMasterQuery : CbjobMaster
    {
        public string startDate { set; get; }
        public string endDate { set; get; }
        public string jobStart { set; get; }
        public string jobEnd { set; get; }
        public string row_id_IN { set; get; }
        public CbjobMasterQuery()
        {
            startDate = string.Empty;
            endDate = string.Empty;
            jobStart = string.Empty;
            jobEnd = string.Empty;
            row_id_IN = string.Empty;

        }
    }
}
