using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IWebContentType3ImplMgr
    {
        /// <summary>
        /// 根據條件查詢影音廣告列表
        /// </summary>
        /// <param name="query">查詢條件model</param>
        /// <returns>影音廣告列表</returns>
        List<WebContentType3Query> QueryAll(WebContentType3Query query, out int totalCount);

        /// <summary>
        /// 新增影音廣告
        /// </summary>
        /// <param name="model">影音廣告model</param>
        /// <returns>新增數據的編號</returns>
        int Add(WebContentType3 model);

        /// <summary>
        /// 修改影音廣告
        /// </summary>
        /// <param name="model">影音廣告model</param>
        /// <returns>更新成功與否</returns>
        int Update(WebContentType3 model);
        WebContentType3 GetModel(WebContentType3 model);
        int delete(WebContentType3 model);
        int GetDefault(WebContentType3 model);
    }
}