using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class CourseDetail
    {
        public int Course_Detail_Id { get; set; }
        public string Course_Detail_Name { get; set; }
        public int Course_Id { get; set; }//與Course
        public string Address { get; set; }//上課地點
        public DateTime Start_Date { get; set; }//單節課程開始時間
        public DateTime End_Date { get; set; }//單節課程結束時間
        public int P_Number { get; set; }//人數

        public CourseDetail()
        {
            Course_Detail_Id = 0;
            Course_Detail_Name = string.Empty;
            Course_Id = 0;
            Address = string.Empty;
            Start_Date = DateTime.MinValue;
            End_Date = DateTime.MinValue;
            P_Number = 0;
        }
    }
}
