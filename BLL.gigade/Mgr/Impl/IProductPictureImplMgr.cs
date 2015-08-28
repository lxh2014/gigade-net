using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProductPictureImplMgr
    {
        bool Save(List<ProductPicture> PicList, int type = 1);
        List<ProductPicture> Query(int product_id, int type = 1);
        string Delete(int product_id, int type = 1);
        string SaveFromOtherPro(ProductPicture productPicture);
    }
}
