/***
 * 訂單管理=>訂單內容=>購物金扣除記錄
 * chaojie1124j_zz添加於 2014/10/30  09:16 AM
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public  class UsersDeductBonus:PageBase
    {
        public uint id { set; get; }//編號
        public uint deduct_bonus { set; get; }//扣除購物金
        public uint user_id { set; get; }
        public uint order_id { set; get; }
        public uint status { set; get; }//狀態0，未扣除
        public uint createdate { set; get; }

        public UsersDeductBonus()
        {
            id = 0;
            deduct_bonus = 0;
            status = 0;
        }
    }
}
