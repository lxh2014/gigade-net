using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class ProdDeliverySetImport
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        public string FreightTypeName { get; set; }//添加 物流配送模式  TranSport   add by zhuoqin0830w  2015/04/24
        /// <summary>
        /// 1為成功,2為已存在,3為不符合
        /// </summary>
        public int Status { get; set; }
        public string Msg
        {
            get
            {
                switch (this.Status)
                {
                    case 1:
                        return "保存成功";
                    case 2: 
                        return "已存在";
                    case 3:
                        return "不符合條件";
                    case 4:
                        return "該商品不存在";
                    default:
                        return "失敗";
                }
            }
        }

        public ProdDeliverySetImport()
        {
            ProductId = "0";
            ProductName = string.Empty;
            BrandName = string.Empty;
            FreightTypeName = string.Empty;//添加 物流配送模式  TranSport   add by zhuoqin0830w  2015/04/24
            Status = 0;
        }

    }
}
