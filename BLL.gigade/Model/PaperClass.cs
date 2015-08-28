using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class PaperClass : PageBase
    {
        public int id { get; set; }
        public int paperID { get; set; }
        public int classID { get; set; }
        public string className { get; set; }
        public string classType { get; set; }
        public int projectNum { get; set; }
        public string classContent { get; set; }
        public int orderNum { get; set; }
        public int isMust { get; set; }
        public int status { get; set; }

        public string paperName { get; set; }
        public PaperClass()
        {
            id = 0;
            paperID = 0;
            classID = 0;
            className = string.Empty;
            classType = string.Empty;
            projectNum = 0;
            classContent = string.Empty;
            orderNum = 0;
            isMust = 0;
            status = 0;
            paperName = string.Empty;
        }
    }
}
