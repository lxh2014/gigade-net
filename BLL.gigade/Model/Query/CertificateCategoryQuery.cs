using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class  CertificateCategoryQuery : CertificateCategory
    {
        public string k_user_tostring { get; set; }
        public string certificate_category_childname { get; set; }//證書-小類名稱
        public string certificate_category_childcode { get; set; }//證書-小類code
        public string rowIDs { get; set; }//刪除時實現多條刪除,小類id集合
        public string frowIDs { get; set; }//刪除時實現多條刪除,大類id集合
        public string searchcon { get; set; }//列表頁查詢條件
        public int frowID { get; set; }//證書大類id
        public CertificateCategoryQuery()
        {
            k_user_tostring = string.Empty;
            certificate_category_childname = string.Empty;
            certificate_category_childcode = string.Empty;
            rowIDs = string.Empty;
            searchcon = string.Empty;
            frowID = 0;
            frowIDs = string.Empty;
        }
    }
}
