/* 
* 文件名稱 :ISitePageImplMgr.cs 
* 文件功能描述 :頁面表數據操作 
* 版權宣告 : 
* 開發人員 : shiwei0620j 
* 版本資訊 : 1.0 
* 日期 : 2014/10/14 
* 修改人員 : 
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface ISitePageImplMgr
    {
        List<SitePageQuery> GetSitePageList(SitePageQuery store, out int totalCount);
        List<SitePage> GetPage(SitePage bp);
        int UpdateStatus(SitePageQuery query);
        int Save(SitePageQuery model);
        int Update(SitePageQuery model);
    }
}
