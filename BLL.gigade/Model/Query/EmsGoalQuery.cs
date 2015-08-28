using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public    class EmsGoalQuery : EmsGoal
    {
        public string department_name { get; set; }
        public DateTime create_time { get; set; }
        public string user_username { get; set; }
        public int user_userid { get; set; }
        public int searchdate { get; set; }
        public DateTime date { get; set; }
        public EmsGoalQuery()
        {
            department_name = string.Empty;
            create_time = DateTime.MinValue;
            user_username = string.Empty;
            user_userid = 2;
            searchdate = 0;
            date = DateTime.MinValue;

        }
    }
}
