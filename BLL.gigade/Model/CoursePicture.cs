using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class CoursePicture
    {
        public uint id { get; set; }
        public int course_id { get; set; }
        public string picture_name { get; set; }
        public string picture_type { get; set; }
        public int picture_status { get; set; }
        public int picture_sort { get; set; }

        public CoursePicture() 
        {
            id = 0;
            course_id = 0;
            picture_name = string.Empty;
            picture_type = string.Empty;
            picture_status = 0;
            picture_sort = 0;
        }
    }
}
