using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using System.Data;
namespace BLL.gigade.Dao.Impl
{
    public interface IProductConsultImplDao
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
