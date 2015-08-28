using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class ProductComboCustom : BLL.gigade.Model.ProductCombo
    {
        public string Product_Name { get; set; }
        public uint brand_id { get; set; }

        public int Writer_Id { get; set; }
        public int item_money { get; set; }
        public int event_money { get; set; }
        public string spec_1 { get; set; }
        public string spec_2 { get; set; }
        public uint item_id { get; set; }
        public uint item_price_id { get; set; }
        public uint price_master_id { get; set; }

        public string prod_sz { get; set; }//add by wwei0216w 2014/11/19
        public int user_id { get; set; }
        public int user_level { get; set; }
        public int site_id { get; set; }

        public int item_cost { get; set; }
        public int event_cost { get; set; }

        public int price_type { get; set; }

        public int sale_status { get; set; }//add by wwei0216w 2015/1/28 商品販售狀態
        
        private string child_id;
        public new string Child_Id {
            get
            {
                if (child_id == "0")
                {
                    return base.Child_Id.ToString();
                }
                else
                {
                    return child_id;
                }
            }
            set
            {
                child_id = value;
            } }//add 2014/09/24

        public ProductComboCustom()
        { 
            Product_Name = string.Empty;
            brand_id = 0;
            Writer_Id = 0;
            item_money = 0;
            event_money = 0;
            spec_1 = string.Empty;
            spec_2 = string.Empty;
            item_id = 0;
            item_price_id = 0;
            price_master_id = 0;
            user_id = 0;
            user_level = 0;
            site_id = 0;
            item_cost = 0;
            event_cost = 0;
            Child_Id = "0";  // add by wangwei0216w 2014/9/24
            prod_sz = "";//add by wwei0216w 2014/11/19
            sale_status = 0;// add by wwei0216w 2015/1/28
        }
    }
}
