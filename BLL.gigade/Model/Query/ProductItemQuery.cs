using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ProductItemQuery : ProductItem
    {/*chaojie1124j 添加三個字段：isAllStock,sumDays,periodDays用來實現商品建議採購量功能*/
        public int stockScope { get; set; }/*庫存範圍--*/
        public int sumDays { get; set; }/*總天數*/
        public int periodDays { get; set; }/*查詢天數*/
        //public int ChannelId { get; set; }
        public int prepaid { set; get; }/*是否買斷*/
        public int Is_pod { set; get; }/*是否已下單採購*/
        public string vendor_name { set; get; }/*供應商名稱*/
        public string category_ID_IN { set; get; }
        public ProductItemQuery()
        {
            stockScope = 0;
            sumDays = 0;
            periodDays = 0;
            prepaid = -1;
            Is_pod = 0;
            vendor_name = string.Empty;
            category_ID_IN = string.Empty;
        }
    }
}