#region 文件信息
/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：PromotionsAccumulateRateMgr.cs 
 * 摘   要： 
 *      點數累積
 * 当前版本：v1.1 
 * 作   者： dongya0410j
 * 完成日期：2014/6/20
 * 修改歷史：
 *      v1.1修改日期：2014/8/15
 *      v1.1修改人員：dongya0410j
 *      v1.1修改内容：代碼合併
 */

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
   public class PromotionsAccumulateRateMgr : IPromotionsAccumulateRateImplMgr
    {
        private IPromotionsAccumulateRateImplDao _deliveryDao;
        public PromotionsAccumulateRateMgr(string connectionString)
        {
            _deliveryDao = new PromotionsAccumulateRateDao(connectionString);
        }
        #region 點數累積 保存 +int Save(Model.PromotionsAccumulateRate store)
        public int Save(Model.PromotionsAccumulateRate store)
        {
            try
            {
                return _deliveryDao.Save(store);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAccumulateRateMgr-->Save-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 點數累積 編輯 +int Update(Model.PromotionsAccumulateRate store)
        public int Update(Model.PromotionsAccumulateRate store)
        {
            try
            {
                return _deliveryDao.Update(store);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAccumulateRateMgr-->Update-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 點數累積 刪除 +int Delete(Model.PromotionsAccumulateRate Query)
        public int Delete(Model.PromotionsAccumulateRate Query)
        {
            try
            {
                return _deliveryDao.Delete(Query);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAccumulateRateMgr-->Delete-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 點數累積 獲取列表信息 +List<Model.Query.PromotionsAccumulateRateQuery> AllMessage(Model.Query.PromotionsAccumulateRateQuery query, ref int tatal)
        public List<Model.Query.PromotionsAccumulateRateQuery> AllMessage(Model.Query.PromotionsAccumulateRateQuery query, ref int tatal)
        {

            try
            {
                return _deliveryDao.Query(query, ref tatal);
            }
            catch (Exception ex)
            {

                throw new Exception("PromotionsAccumulateRateMgr-->AllMessage-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 點數累積 獲取某行數據 +Model.PromotionsAccumulateRate GetModel(int id)
        public Model.PromotionsAccumulateRate GetModel(int id)
        {
            try
            {
                return _deliveryDao.GetModel(id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAccumulateRateMgr-->GetModel-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 點數累積 更新活動狀態 +int UpdateActive(Model.PromotionsAccumulateRate store)
        public int UpdateActive(Model.PromotionsAccumulateRate store)
        {
            try
            {
                return _deliveryDao.UpdateActive(store);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAccumulateRateMgr-->UpdateActive-->" + ex.Message, ex);
            }
        }
        #endregion
    }
}
