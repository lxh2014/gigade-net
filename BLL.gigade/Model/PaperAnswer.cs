using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class PaperAnswer:PageBase
    {
        public int answerID { get; set; }
        public int paperID { get; set; }
        public int userid { get; set; }
        public string userMail { get; set; }
        public int order_id { get; set; }
        public int classID { get; set; }
        public string answerContent { get; set; }
        public string classType { get; set; }
        public DateTime answerDate { get; set; }
        //帶出一些信息
        public string paperName { get; set; }
        public string className { get; set; }
        public string classContent { get; set; }
        public PaperAnswer()
    {
            answerID = 0;
            paperID = 0;
            classID = 0;
            userid = 0;
            userMail = string.Empty;
            order_id = 0;
            classType = string.Empty;
            answerContent = string.Empty;
            answerDate = DateTime.Now;
            paperName = string.Empty;
            className = string.Empty;
            classContent = string.Empty;
        }
    }
}
