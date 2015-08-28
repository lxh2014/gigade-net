using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class CourseDetailItemTemp : CourseDetailItem
    {
        public int Writer_Id { get; set; }

        public CourseDetailItemTemp()
        {
            Writer_Id = 0;
        }
    }
}
