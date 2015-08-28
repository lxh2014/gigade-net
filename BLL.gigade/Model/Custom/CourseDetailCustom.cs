using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class CourseDetailCustom:CourseDetail
    {
        public long P_NumberReality { get; set; }

        public CourseDetailCustom()
        {
            P_NumberReality = 0;
        }
    }
}
