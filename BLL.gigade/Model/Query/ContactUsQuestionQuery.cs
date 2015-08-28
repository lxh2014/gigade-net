using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ContactUsQuestionQuery : ContactUsQuestion
    {

        public int search_type { get; set; }
        public string searchcontent { get; set; }
        public Boolean isSecret { get; set; }//是否機敏
        public DateTime datestart { get; set; }
        public DateTime dateend { get; set; }

        public ContactUsQuestionQuery()
        {
            search_type = 0;
            searchcontent = string.Empty;
            isSecret = false;
            datestart = DateTime.MinValue;
            dateend = DateTime.MinValue;
        }
    }
}
