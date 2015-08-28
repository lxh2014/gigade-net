using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class TrialRecordQuery : TrialRecord
    {

        public int user_gender { get; set; }
        public string name { get; set; }
        public string event_type { get; set; }
        public string eventId { get; set; }
        public int paper_id { get; set; }

        public TrialRecordQuery()
        {
            user_gender = 0;
            name = string.Empty;
            event_type = string.Empty;
            eventId = string.Empty;
            paper_id = 0;
        }
    }
}
