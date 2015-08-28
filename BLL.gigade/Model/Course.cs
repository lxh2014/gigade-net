using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class Course:PageBase
    {
        public int Course_Id { get; set; }
        public string Course_Name { get; set; } //課程名稱
        public string Tel { get; set; }//聯繫電話
        public int Send_Msg { get; set; }//是否發送簡訊(0:是 1:否)
        public string Msg { get; set; }//簡訊內容
        public int Send_Mail { get; set; }//是否發送Mail
        public string Mail_Content { get; set; }//Mail內容
        public DateTime Start_Date { get; set; }//整個課程的開始時間
        public DateTime End_Date { get; set; }//整個課程的結束時間
        public DateTime Create_Time { get; set; }//創建時間
        public int Source { get; set; }//課程來源(0:自辦 1:合作商)
        public int Ticket_Type { get; set; }//票券類型(0:虛擬 1:實體)

        public Course()
        {
            Course_Id = 0;
            Course_Name = string.Empty;
            Tel = string.Empty;
            Send_Msg = -1;
            Msg = string.Empty;
            Send_Mail = -1;
            Mail_Content = string.Empty;
            Start_Date = DateTime.MinValue;
            End_Date = DateTime.MinValue;
            Create_Time = DateTime.MinValue;
            Source = -1;
            Ticket_Type = -1;
        }
    }
}
