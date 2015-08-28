using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProductPictureTempImplMgr
    {
        bool Save(List<ProductPictureTemp> PicList, ProductPictureTemp prPictureTemp, int type=1);
        List<ProductPictureTemp> Query(ProductPictureTemp proPictureTemp, int type=1);
        string Delete(ProductPictureTemp proPictureTemp, int type);
        string MoveToProductPicture(ProductPictureTemp proPictureTemp, int type = 1);
        string SaveFromProPicture(ProductPictureTemp proPictureTemp,int type=1);

        string DeleteByVendor(ProductPictureTemp proPictureTemp);
        List<ProductPictureTemp> VendorQuery(ProductPictureTemp proPictureTemp);
        string VendorSaveFromProPicture(ProductPictureTemp proPictureTemp, string old_product_Id);
    }
}
