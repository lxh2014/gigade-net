using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ErrorLog:PageBase
    {
        public int rowid { get; set; }
        public DateTime log_date { get; set; }
        public string Thread { get; set; }
        public string Level { get; set; }
        public string logger { get; set; }
        public string message { get; set; }
        public string method { get; set; }
        public ErrorLog()
        {
            rowid = 0;
            log_date = DateTime.MinValue;
            Thread = string.Empty;
            Level = string.Empty;
            logger = string.Empty;
            message = string.Empty;
            method = string.Empty;
        }
    }
}
