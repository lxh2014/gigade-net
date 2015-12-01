using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class IialgMgr : IIialgImplMgr
    {
         private IIialgImplDao _iagDao;
         private IManageUserImplDao _maguserDao;
         public IialgMgr(string connectionString)
        {
            _iagDao = new IialgDao(connectionString);
            _maguserDao = new ManageUserDao(connectionString);
        }
        public int insertiialg(Model.Query.IialgQuery iagQuery)
        {
            try
            {
                return _iagDao.insertiialg(iagQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("IialgMgr-->insertiialg-->" + ex.Message, ex);
            }
        }

        public List<IialgQuery> GetIialgList(IialgQuery q, out int totalCount)
        {
            try
            {
                return _iagDao.GetIialgList(q, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("IialgMgr-->GetIialgList-->" + ex.Message, ex);
            }
        }
        public int HuiruInsertiialg(System.Data.DataRow[] dr, out int iialgtotal, out int iinvdtotal)
        {
            try
            {
                return _iagDao.HuiruInsertiialg(dr, out iialgtotal,out iinvdtotal);
            }
            catch (Exception ex)
            {
                throw new Exception("IialgMgr-->HuiruInsertiialg-->" + ex.Message, ex);
            }
        }
        public List<ManageUser> GetkutiaoUser() //by zhaozhi0623j add 庫存調整人員store
        {
            try
            {             
                return _maguserDao.GetkutiaoUser();
            }
            catch (Exception ex)
            {
                throw new Exception("IialgMgr-->GetkutiaoUser-->" + ex.Message, ex);
            }
        }

        public List<IialgQuery> GetExportIialgList(IialgQuery q)
        {
            try
            {
                return _iagDao.GetExportIialgList(q);
            }
            catch (Exception ex)
            {
                throw new Exception("IialgMgr-->GetExportIialgList-->" + ex.Message, ex);
            }
        }
        public int addIialgIstock(IialgQuery q)
        {
            try
            {
                return _iagDao.addIialgIstock(q);
            }
            catch (Exception ex)
            {
                throw new Exception("IialgMgr-->addIialgIstock-->" + ex.Message, ex);
            }
        }
        public int addIialgIstock_AutoMarket(IialgQuery q)
        {
            try
            {
                return _iagDao.addIialgIstock_AutoMarket(q);
            }
            catch (Exception ex)
            {
                throw new Exception("IialgMgr-->addIialgIstock_AutoMarket-->" + ex.Message, ex);
            }
        }
    }
}
