using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class PriceMasterTsMgr : IPriceMasterTsImplMgr
    {
        private IPriceMasterTsImplDao _priceMasterTsDao;
        string connectionString = string.Empty;
        public PriceMasterTsMgr(string connectionStr)
        {
            _priceMasterTsDao = new PriceMasterTsDao(connectionStr);
            this.connectionString = connectionStr;
        }
        public Model.PriceMaster QueryPriceMasterTs(Model.PriceMaster pM)
        {
            return _priceMasterTsDao.QueryPriceMasterTs(pM);
        }
        public List<Model.PriceMaster> QueryByApplyId(Model.PriceMaster pM)
        {
            return _priceMasterTsDao.QueryByApplyId(pM);
        }
        public string UpdateTs(Model.PriceMaster pM)
        {
            return _priceMasterTsDao.UpdateTs(pM);
        }
        //public string UpdateEventTs(Model.PriceMaster pM)
        //{
        //    return _priceMasterTsDao.UpdateEventTs(pM);
        //}

        public string DeleteTs(Model.PriceMaster pM)
        {
            return _priceMasterTsDao.DeleteTs(pM);
        }
    }
}
