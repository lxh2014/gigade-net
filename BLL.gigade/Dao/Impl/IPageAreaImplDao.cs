using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface IPageAreaImplDao
    {
        List<PageAreaQuery> QueryAll(PageAreaQuery query, out int totalCount);
        //int Update2(Model.WebContentType7 model);
        int Update(PageArea model);
        PageArea GetModel(PageArea model);
        List<PageArea> GetArea();
        int AreaSave(PageArea ba);
        PageAreaQuery GetBannerByAreaId(int areaId);
        int UpPageAreaStatus(PageArea model);
    }
}
