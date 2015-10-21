using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Mgr.Impl
{
    public interface IProductCommentImplMgr
    {
        DataTable Query(ProductCommentQuery store, out int totalCount);
        int UpdateActive(ProductCommentQuery model);
        int ProductCommentSave(ProductCommentQuery query);
        int ProductCommentSatisfySave(ProductCommentQuery query);
        ProductCommentQuery GetUsetInfo(Model.Query.ProductCommentQuery store);
        DataTable QueryTableName();
        DataTable GetChangeLogList(ProductCommentQuery query, out int totalCount);
        BLL.gigade.Model.Custom.TableChangeLogCustom GetChangeLogDetailList(int pk_id, string create_time);
        DataTable ProductCommentLogExport(ProductCommentQuery query);
    }
}
