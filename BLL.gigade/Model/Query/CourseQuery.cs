using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class CourseQuery : Course
    {
        public int Vendor_Id { get; set; }
        public string Vendor_Name_Simple { get; set; }
        public string Spec_Name_1 { get; set; }
        public string Spec_Name_2 { get; set; }
        public int Sales_Number { get; set; }
        public int Used_Number { get; set; }
        public CourseQuery()
        {
            Vendor_Id = 0;  
            Vendor_Name_Simple = string.Empty;
            Spec_Name_1 = string.Empty;
            Spec_Name_2 = string.Empty;
            Sales_Number = 0;
            Used_Number = 0;
        }
    }
}
