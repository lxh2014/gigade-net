using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class PayEasyMgr:IPayEasyImplMgr
    {
        IPayEasyImplDao _payEasyDao;
        public PayEasyMgr(string connstring)
        {
            _payEasyDao = new PayEasyDao(connstring);
        }
        public List<PayEasyQuery> Query(PayEasyQuery query)
        {
            //todo:
            List<PayEasyQuery> list = new List<PayEasyQuery>();
            return list;
        }


        public System.Data.DataTable QueryExcel(PayEasyQuery query)
        {
            return _payEasyDao.QueryExcel(query);
        }
    }
}
