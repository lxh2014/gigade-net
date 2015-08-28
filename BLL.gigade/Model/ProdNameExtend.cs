using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProdNameExtend:PageBase
    {
        public long Rid { get; set; } //主鍵
        public uint Price_Master_Id { get; set; }
        public uint Apply_id { get; set; }
        public string Product_Prefix { get; set; }//商品前綴
        public string Product_Suffix { get; set; }//商品後綴
        public uint Event_Start { get; set; }//活動開始時間
        public uint Event_End { get; set; }//活動結束時間
        public int Flag { get; set; }   // 0:可編輯 1:審核中 2:作用中 3:過期
        public string Kuser { get; set; }//創建人
        public DateTime Kdate { get; set; }//創建時間

        public ProdNameExtend()
        {
            Price_Master_Id = 0;
            Apply_id = 0;
            Product_Prefix = string.Empty;
            Product_Suffix = string.Empty;
            Event_Start = 0;
            Event_End = 0;
            Flag = 0;
            Kuser = string.Empty;
            Kdate = DateTime.MinValue; 
        }
    }
    //public class ProdNameExtendComparer : IEqualityComparer<ProdNameExtend>
    //{
    //    public bool Equals(ProdNameExtend x, ProdNameExtend y)
    //    {
    //        return x.Price_Master_Id == y.Price_Master_Id;
    //    }

    //    public int GetHashCode(ProdNameExtend obj)
    //    {
    //        return obj.GetHashCode();
    //    }
    //}

}
