using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class CourseTicketCustom:CourseTicket
    {
        public string User_Name { get; set; }

        public CourseTicketCustom()
        {
            User_Name = string.Empty;
        }
    }
}
