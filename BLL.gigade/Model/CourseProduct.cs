using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class CourseProduct
    {
        public int Course_Product_Id { get; set; }
        public int Course_Id { get; set; }// '與Course關聯'
        public uint Product_Id { get; set; }// '與product關聯'

        public CourseProduct()
        {
            Course_Product_Id = 0;
            Course_Id = 0;
            Product_Id = 0;
        }
    }
}
