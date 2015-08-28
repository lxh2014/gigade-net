using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;

namespace BLL.gigade.Mgr
{
    public class ItemPriceTsMgr : IItemPriceTsImplMgr
    {   private IItemPriceTsImplDao _itemPriceTsDao;
        public ItemPriceTsMgr(string connectionStr)
        {
            _itemPriceTsDao = new ItemPriceTsDao(connectionStr);
        }

        public string UpdateTs(Model.ItemPrice itemPrice)
        {
            return _itemPriceTsDao.UpdateTs(itemPrice);
        }

        public string DeleteTs(Model.ItemPrice itemPrice)
        {
            return _itemPriceTsDao.DeleteTs(itemPrice);
        }
    }
}
