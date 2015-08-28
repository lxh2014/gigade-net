using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model.Custom;
using BLL.gigade.Model;
namespace BLL.gigade.Mgr
{
    public class ProductComboMgr : BLL.gigade.Mgr.Impl.IProductComboImplMgr
    {
        private IProductComboImplDao _combDao;
        public ProductComboMgr(string connectionString)
        {
            _combDao = new ProductComboDao(connectionString);
        }
        public List<ProductComboCustom> combQuery(ProductComboCustom query)
        {
            try
            {
                return _combDao.combQuery(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboMgr-->combQuery-->" + ex.Message, ex);
            }
            
        }
        public List<ProductComboCustom> combNoPriceQuery(ProductComboCustom query)
        {
            try
            {
                return _combDao.combNoPriceQuery(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboMgr-->combQuery-->" + ex.Message, ex);
            }
        }


        public List<ProductCombo> groupNumQuery(ProductCombo query)
        {
            
            try
            {
                return _combDao.groupNumQuery(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboMgr-->groupNumQuery-->" + ex.Message, ex);
            }
        }

        public List<ProductComboCustom> getChildren(ProductComboCustom query)
        {
            try
            {
                return _combDao.getChildren(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboMgr-->getChildren-->" + ex.Message, ex);
            }
        }

        public string Save(ProductCombo combo)
        {
            return _combDao.Save(combo);
        }

        public List<ProductComboCustom> sameSpecQuery(ProductComboCustom query)
        {
            try
            {
                return _combDao.sameSpecQuery(query);
            }
            catch (Exception ex)
            {
                
                throw new Exception("ProductComboMgr-->sameSpecQuery-->" + ex.Message, ex);
            }
        }

        public List<MakePriceCustom> differentSpecQuery(ProductComboCustom query)
        {
            try
            {
                return _combDao.differentSpecQuery(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboMgr-->differentSpecQuery-->" + ex.Message, ex);
            }
        }

        public List<MakePriceCustom> differentNoPriceSpecQuery(ProductComboCustom query)
        {
            try
            {
                return _combDao.differentNoPriceSpecQuery(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboMgr-->differentNoPriceSpecQuery-->" + ex.Message, ex);
            }
        }

        public string Delete(int parent_Id)
        {
            try
            {
                return _combDao.Delete(parent_Id);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboMgr-->Delete-->" + ex.Message, ex);
            }
        }

        //add by wwei0216w 2015/7/6 
        public string GetParentList(int child_Id)
        {
            try
            {
                List<ProductCombo> list = _combDao.GetParentList(child_Id);
                if (list.Count == 0) return "";
                string parent_list = string.Empty;
                foreach (var item in list)
                {
                    parent_list += item.Parent_Id.ToString() + ",";
                }
                parent_list = parent_list.Remove(parent_list.ToString().Length - 1, 1);
                return parent_list;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboMgr-->Delete-->" + ex.Message, ex);
            }
        }

    }
}
