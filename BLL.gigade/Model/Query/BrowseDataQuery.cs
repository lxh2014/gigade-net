using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class BrowseDataQuery : BrowseData
    {
        /// <summary>
        /// 會員姓名
        /// </summary>
        public string user_name { get; set; }
        /// <summary>
        /// 商品名稱
        /// </summary>
        public string product_name { get; set; }
        /// <summary>
        /// 查詢內容
        /// </summary>
        public string SearchCondition { get; set; }

          public bool isSecret { get; set; }
        
          public int SearchType { get; set; }//搜索類型
        public BrowseDataQuery()
        {
            user_name = string.Empty;
            product_name = string.Empty;
            SearchCondition = string.Empty;
            isSecret = true;
            SearchType = 0;
        }
    }
}