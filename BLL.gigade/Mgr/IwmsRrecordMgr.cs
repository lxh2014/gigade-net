using BLL.gigade.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
namespace BLL.gigade.Mgr
{
    public class IwmsRrecordMgr:IIwmsRrecordMgr
    {
        IwmsRrecordDao dao;
        public IwmsRrecordMgr(string connectionstring)
        {
            dao = new IwmsRrecordDao(connectionstring);
        }
        public List<Model.Query.IwmsRrecordQuery> GetIwmsRrecordList(Model.Query.IwmsRrecordQuery query, out int totalCount)
        {
            try
            {
                return dao.GetIwmsRrecordList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("IwmsRrecordMgr-->GetIwmsRrecordList-->" + ex.Message, ex);
            }

        }


        public List<Model.ManageUser> GetUserslist(string code)
        {
            try
            {
                return dao.GetUserslist(code);
            }
            catch (Exception ex)
            {
                throw new Exception("IwmsRrecordMgr-->GetUserslist-->" + ex.Message, ex);
            }
        }
    }
}
