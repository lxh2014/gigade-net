using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface IPalletMoveImplDao
    {
        List<IinvdQuery> GetPalletList(IinvdQuery Iinvd, string invposcat = null);
        string UpPallet(IinvdQuery Iinvd, string newinvd, string num, int userId);
        int UpPalletTime(IinvdQuery Iinvd);
        DataTable GetProdInfo(string pid);
        int updatemadedate(IinvdQuery Iinvd);
        List<ProductExt> selectproductexttime(string item_id);
        int selectcount(IinvdQuery invd);//判斷是否有同一料位 同一時間的商品
        DataTable selectrow_id(IinvdQuery invd);//判斷是否有同一料位 同一時間的商品 則獲取該row_id
        int UpdateordeleteIinvd(IinvdQuery invd, IinvdQuery newinvd);//刪除並且更改iinvd
        DataTable GetProductMsgByLocId(string loc_id);
    }
}
