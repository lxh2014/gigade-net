using BLL.gigade.Dao;
using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace BLL.gigade.Mgr
{
    public class BonusTypeMgr
    {
        BonusTypeDao dao;
        public BonusTypeMgr(string connectionstring)
        {
            dao = new BonusTypeDao(connectionstring);
        }

        public DataTable  GetBonusTypeList()
        {
            try
            {
                return dao.GetBonusTypeList();
            }
            catch (Exception ex)
            {
                throw new Exception("BonusTypeMgr-->GetBonusTypeList-->" + ex.Message, ex);
            }
        }
    }
}
