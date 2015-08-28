using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Model.Custom
{
    public class ProdNameExtendCustom : ProdNameExtend
    {
        public uint Product_Id { get; set; }
        public string Site_Name { get; set; }
        public uint Site_Id { get; set; }
        public uint User_Level { get; set; }
        public uint User_Id { get; set; }
        public string Product_Name { get; set; }//商品後綴 add by wwei0216w 2014/12/12
        public int Type { get; set; } //add  by  wwei 2014/12/17 用來記錄審核表的核可記錄
        public int Key_Id { get; set; } //add by wwei 2014/12/30 用來區別該數據是否有做修改的id
        //public int UpdateState { get { return Flag; } set { Flag = value; } }
        

        public string Level_Name { get; set; }

        public ProdNameExtendCustom()
        {
            Product_Id = 0;
            Type = 0;
            Site_Name = string.Empty;
            Level_Name = string.Empty;
            Product_Name = string.Empty;
            Key_Id = 0;
        }
    }
}
