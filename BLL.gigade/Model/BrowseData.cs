/*
* 文件名稱 :Browse.cs
* 文件功能描述 :商品點擊信息表
* 版權宣告 :
* 開發人員 : yunlong0726h
* 版本資訊 : 1.0
* 日期 : 2015/02/03
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    /// <summary>
    /// 商品點擊信息
    /// </summary>
    public class BrowseData:PageBase
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        public int user_id { get; set; }
        public int product_id { get; set; }
        public int type { get; set; }
        public int count { get; set; }

        public BrowseData()
        {
            id = 0;
            user_id = 0;
            product_id = 0;
            type = 0;
            count = 0;
        }
    }
}
