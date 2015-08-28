﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Dao.Impl
{
    interface IProductStatusHistoryImplDao
    {
        string Save(BLL.gigade.Model.ProductStatusHistory save);
        string SaveNoProductId(BLL.gigade.Model.ProductStatusHistory save);
        string Delete(BLL.gigade.Model.ProductStatusHistory history);
        /// <summary>
        /// 商品狀態更動歷程
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<ProductStatusHistoryCustom> HistoryQuery(ProductStatusHistoryCustom query);
        int UpdateColumn(BLL.gigade.Model.ProductStatusHistory save);
    }
}
