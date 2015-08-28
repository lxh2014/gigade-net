using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class CourseDetailItem
    {
        public int Course_Detail_Item_Id { get; set; }
        public int Course_Detail_Id { get; set; }//與Course_Detail關聯
        public uint Item_Id { get; set; }//與product_item關聯
        public int People_Count { get; set; }//人數
        public long Ticket_Count { get; set; }//實際賣出的票數

        public CourseDetailItem()
        {
            Course_Detail_Item_Id = 0;
            Course_Detail_Id = 0;
            Item_Id = 0;
            Ticket_Count = 0;
        }
    }
}
