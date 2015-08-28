/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromotionsBonusMgr.cs
* 摘 要：
* 序號兌換
* 当前版本：v1.1
* 作 者：dongya0410j    
* 完成日期：2014/6/20 
* 修改歷史:
*         v1.1修改日期：2014/8/15 
*         v1.1修改人員：dongya0410j
*         v1.1修改内容：在抛出異常的時候將Sql語句抛出，合并代碼，添加注釋 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using System.Data;

namespace BLL.gigade.Mgr
{
   public class PromotionsBonusMgr : IPromotionsBonusImplMgr
    {
        private IPromotionsBonusImplDao _bonusDao;
        public PromotionsBonusMgr(string connectionString)
        {
            _bonusDao = new PromotionsBonusDao(connectionString);
        }

        #region 序號兌換 保存 +int Save(Model.PromotionsBonus promoBonus)
        public int Save(Model.PromotionsBonus promoBonus)
        {
            try
            {
                return _bonusDao.Save(promoBonus);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsBonusMgr-->Save-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 序號兌換 編輯 +int Update(Model.PromotionsBonus promoBonus)
        public int Update(Model.PromotionsBonus promoBonus)
        {
            try
            {
                return _bonusDao.Update(promoBonus);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsBonusMgr-->Update-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 序號兌換 刪除 +int Delete(Model.PromotionsBonus pId)
        public int Delete(Model.PromotionsBonus pId)
        {
            try
            {
                return _bonusDao.Delete(pId);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsBonusMgr-->Delete-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 序號兌換 列表 +List<Model.Query.PromotionsBonusQuery> Query(Model.Query.PromotionsBonusQuery store, out int totalCount)
        public List<Model.Query.PromotionsBonusQuery> Query(Model.Query.PromotionsBonusQuery store, out int totalCount)
        {
            try
            {
                return _bonusDao.Query(store, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsBonusMgr-->Query-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 序號兌換 獲取某行數據 +Model.PromotionsBonus GetModel(int id)
        public Model.PromotionsBonus GetModel(int id)
        {
            try
            {
                return _bonusDao.GetModel(id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsBonusMgr-->GetModel-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 序號兌換 改變狀態 +int UpdateActive(Model.PromotionsBonus store)
        public int UpdateActive(Model.PromotionsBonus store)
        {
            try
            {
                return _bonusDao.UpdateActive(store);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsBonusMgr-->UpdateActive-->" + ex.Message, ex);
            }
        }
        #endregion
    }
}
