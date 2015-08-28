using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Mgr.Impl
{
    public interface IWebContentType1ImplMgr
    {
        /// <summary>
        /// 根據條件查詢影音廣告列表
        /// </summary>
        /// <param name="query">查詢條件model</param>
        /// <returns>影音廣告列表</returns>
        List<WebContentType1Query> QueryAll(WebContentType1Query query, out int totalCount);

        /// <summary>
        /// 新增影音廣告
        /// </summary>
        /// <param name="model">影音廣告model</param>
        /// <returns>新增數據的編號</returns>
        int Add(WebContentType1 model);

        /// <summary>
        /// 修改影音廣告
        /// </summary>
        /// <param name="model">影音廣告model</param>
        /// <returns>更新成功與否</returns>
        int Update(WebContentType1 model);


        WebContentType1 GetModel(WebContentType1 model);

        int delete(WebContentType1 model);

        int GetDefault(WebContentType1 model);
    }
}
