using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;

namespace BLL.gigade.Mgr
{
    public class OrderMoneyReturnMgr : IOrderMoneyReturnImplMgr
    {

            private IOrderMoneyReturnImplDao _IOrderMoneyReturnDao;

            public OrderMoneyReturnMgr(string connectionString)
            {
                _IOrderMoneyReturnDao = new OrderMoneyReturnDao(connectionString);
            }
        public List<Model.Query.OrderMoneyReturnQuery> OrderMoneyReturnList(Model.Query.OrderMoneyReturnQuery query, out int totalCount)
        {
            try
            {
                return _IOrderMoneyReturnDao.OrderMoneyReturnList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMoneyReturnMgr-->OrderMoneyReturnList-->"+ex.Message,ex);
            }
        }


        public DataTable ExportATM(Model.Query.OrderMoneyReturnQuery query)
        {
            try
            {
                return _IOrderMoneyReturnDao.ExportATM(query);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMoneyReturnMgr-->ExportATM-->" + ex.Message, ex);
            }
        }


        public DataTable ExportCARD(Model.Query.OrderMoneyReturnQuery query)
        {
            try
            {
                return _IOrderMoneyReturnDao.ExportCARD(query);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMoneyReturnMgr-->ExportCARD-->" + ex.Message, ex);
            }
        }


        public int SaveOMReturn(Model.Query.OrderMoneyReturnQuery query)
        {
            try
            {
                return _IOrderMoneyReturnDao.SaveOMReturn(query);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMoneyReturnMgr-->SaveOMReturn-->" + ex.Message, ex);
            }
        }


        public string SaveCSNote(Model.Query.OrderMoneyReturnQuery query)
        {
            string json = string.Empty;
            try
            {
                if (_IOrderMoneyReturnDao.SaveCSNote(query) > 0)
                {
                    json = "{success:true}";
                }
                else
                {
                    json = "{success:false}";
                }
                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMoneyReturnMgr-->SaveCSNote-->" + ex.Message, ex);
            }
            //throw new NotImplementedException();
        }
    }
}
