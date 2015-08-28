using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class CourseProductTemp
    {
        public int Course_Product_Id { get; set; }
        public int Course_Id { get; set; }// '與Course關聯'
        public uint Product_Id { get; set; }// '與product關聯'
        public int Writer_Id { get; set; }

        public CourseProductTemp()
        {
            Course_Product_Id = 0;
            Course_Id = 0;
            Product_Id = 0;
            Writer_Id = 0;
        }
    }
}
