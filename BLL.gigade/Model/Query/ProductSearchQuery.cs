using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ProductSearchQuery : Product
    {
       
        public string searchKey { get; set; }
        /// <summary>
        /// 是否為食安關鍵字標誌  1為是 0為否 初始為-1
        /// </summary>
        public string flag { get; set; }
        public int pid { get; set; }
        public int MaxMatches { get; set; }
        public ProductSearchQuery()
        {
            this.flag ="-1" ;
            this.searchKey = string.Empty;
            this.pid = -1;
            this.MaxMatches = 1000;
        }
    }
}
