using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class CourseDetailItemCustom:CourseDetailItem
    {
        public string Course_Detail_Name { get; set; }
        public string Spec_Name1 { get; set; }
        public string Spec_Name2 { get; set; }

        public CourseDetailItemCustom()
        {
            Course_Detail_Name = string.Empty;
            Spec_Name1 = string.Empty;
            Spec_Name2 = string.Empty;
        }
    }
}
