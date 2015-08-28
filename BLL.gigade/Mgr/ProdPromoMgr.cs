using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class ProdPromoMgr : IProdPromoImplMgr
    {
        private IProdPromoImplDao _ppDao;
        public ProdPromoMgr(string connectionstring)
        {
            _ppDao = new ProdPromoDao(connectionstring);
        }
        public List<Model.ProdPromo> Select(Model.ProdPromo model, out int totalCount)
        {
            try
            {
                return _ppDao.Select(model, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("ProdPromoMgr-->Query-->" + ex.Message, ex);
            }
        }

        public int Save(Model.ProdPromo model)
        {
            try
            {
                return _ppDao.Save(model);
            }
            catch (Exception ex)
            {

                throw new Exception("ProdPromoMgr-->Save-->" + ex.Message, ex);
            }

        }

        public int Update(Model.ProdPromo model)
        {
            try
            {
                return _ppDao.Update(model);
            }
            catch (Exception ex)
            {

                throw new Exception("ProdPromoMgr-->Update-->" + ex.Message, ex);
            }

        }

       
    }
}
