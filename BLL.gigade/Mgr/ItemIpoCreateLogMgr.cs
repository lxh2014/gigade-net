using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class ItemIpoCreateLogMgr : IItemIpoCreateLogImplMgr
    {
        private IItemIpoCreateLogImplDao _ItemIpo;

         public ItemIpoCreateLogMgr(string connectionString)
        {
            _ItemIpo = new ItemIpoCreateLogDao(connectionString);
        }
         public int AddItemIpoCreate(ItemIpoCreateLogQuery query)
        {
            try
            {
                return _ItemIpo.AddItemIpoCreate(query);
            }
            catch (Exception ex)
            {

                throw new Exception("ItemIpoCreateLogMgr-->AddItemIpoCreate-->" + ex.Message, ex);
            }
        }
    }
}
