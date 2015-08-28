using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class TrialProdCateMgr : ITrialProdCateImplMgr
    { 
        private ITrialProdCateImplDao _tpcDao;
        private MySqlDao _mysqlDao;
        private string connectionStr;
        public TrialProdCateMgr(string connectionstring)
        {
            _tpcDao = new TrialProdCateDao(connectionstring);
            this.connectionStr = connectionstring;
            _mysqlDao = new MySqlDao(connectionStr);
        }
        public List<TrialProdCateQuery> Query(TrialProdCateQuery query, out int totalCount)
        {
            try
            {
                return _tpcDao.Query(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("TrialProdCateMgr-->Query-->" + ex.Message, ex);
            }
        }



        public List<TrialProdCateQuery> UadateTrialProd()
        {
            try
            {
                return _tpcDao.UadateTrialProd();
            }
            catch (Exception ex)
            {
                throw new Exception("TrialProdCateMgr-->UadateTrialProd-->" + ex.Message, ex);
            }
        }





        public bool InsertTrialProd(List<TrialProdCateQuery> tpcQueryLi)
        {
            try
            {
                ArrayList arryList = new ArrayList();
                arryList.Add(DeleteTrialProd());
                foreach (var item in tpcQueryLi)
                {
                    arryList.Add(InsertTrialProd(item));
                }
                return _mysqlDao.ExcuteSqls(arryList);
            }
            catch (Exception ex)
            {
                throw new Exception("TrialProdCateMgr-->InsertTrialProd-->" + ex.Message, ex);
            }
        }




        public string InsertTrialProd(TrialProdCateQuery query)
        {
            try
            {
                return _tpcDao.InsertTrialProd(query);
            }
            catch (Exception ex)
            {
                throw new Exception("TrialProdCateMgr-->InsertTrialProd-->" + ex.Message, ex);
            }
        }

        public string DeleteTrialProd()
        {
            try
            {
                return _tpcDao.DeleteTrialProd();
            }
            catch (Exception ex)
            {
                throw new Exception("TrialProdCateMgr-->DeleteTrialProd-->" + ex.Message, ex);
            }
        }
    }
}
