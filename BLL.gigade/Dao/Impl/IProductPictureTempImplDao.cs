using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    public interface IProductPictureTempImplDao
    {
        string Save(ProductPictureTemp Pic, int type);
        List<ProductPictureTemp> Query(ProductPictureTemp proPictureTemp, int type);
        string Delete(ProductPictureTemp proPictureTemp, int type);
        string MoveToProductPicture(ProductPictureTemp proPictureTemp, int type = 1);
        string SaveFromProPicture(ProductPictureTemp proPictureTemp, int type = 1);

        string DeleteByVendor(ProductPictureTemp proPictureTemp);
        List<ProductPictureTemp> VendorQuery(ProductPictureTemp proPictureTemp);
        string VendorSaveFromProPicture(ProductPictureTemp proPictureTemp, string old_product_Id);

        #region 與供應商商品相關
        string Vendor_MoveToProductPicture(ProductPictureTemp proPictureTemp);
        string Vendor_Delete(ProductPictureTemp proPictureTemp); 
        #endregion
    }
}
