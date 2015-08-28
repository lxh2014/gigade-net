using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class NewPromoQuestionQuery : NewPromoQuestionnaire
    {

        public int IsModified { set; get; }//新增或者編輯
        public string row_id_in { set; get; }//用於批量刪除
        public Int64 present_num { set; get; }//有效贈品數量
        public int searchtype { set; get; }//群組名稱
        public string s_promo_image { get; set; }//圖片的絕對路徑
        public string group_name { get; set; }
        public NewPromoQuestionQuery()
        {
            IsModified = 0;
            row_id_in = string.Empty;
            present_num = 0;
            searchtype = 1;
            s_promo_image = string.Empty;
            group_name = string.Empty;
        }
    }
}
