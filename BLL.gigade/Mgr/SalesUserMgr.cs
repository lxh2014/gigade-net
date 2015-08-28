using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class SalesUserMgr : ISalesUserImplMgr
    {
        private ISalesUserImplDao _salesIpldao;
        public SalesUserMgr(string connectionString)
        {
            _salesIpldao = new SalesUserDao(connectionString);
        }

        public List<Model.Query.SalesUserQuery> Query(Model.Query.SalesUserQuery store, out int totalCount)
        {
            throw new NotImplementedException();
        }

        public int SaveUserList(Model.Query.SalesUserQuery usr)
        {
            return _salesIpldao.SaveUserList(usr);
        }

        public Model.Custom.Users getModel(int id)
        {
            throw new NotImplementedException();
        }


        public int Selectbigsid()
        {
            return _salesIpldao.Selectbigsid();
        }



        public int updatesaleuser(Model.Query.SalesUserQuery usr)
        {
            return _salesIpldao.updatesaleuser(usr);
        }


        public Model.SalesUser MySalesUser(int id)
        {
            return _salesIpldao.MySalesUser(id);
        }
    }
}
