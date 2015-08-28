using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;
namespace BLL.gigade.Mgr.Impl
{
    public interface IProductConsultImplMgr
    {

        List<ProductConsultQuery> Query(ProductConsultQuery store, out int totalCount);
        int UpdateActive(ProductConsultQuery model);
        int SaveProductConsultAnswer(ProductConsultQuery query);
        ProductConsultQuery GetProductInfo(ProductConsultQuery query);
        int UpdateAnswerStatus(ProductConsultQuery model);
        int UpdateConsultType(ProductConsultQuery model);
        DataTable QueryMailGroup(ProductConsultQuery query);
    }
}
