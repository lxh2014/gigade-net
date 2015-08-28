/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromotionsDeductRateMgr.cs
* 摘 要：
* 點數抵用與資料庫交互方法  
* 当前版本：v1.1
* 作 者：dongya0410j    
* 完成日期：2014/6/20 
* 修改歷史:
*         v1.1修改日期：2014/8/15 
*         v1.1修改人員：zhejiang0304j 
*         v1.1修改内容：合并代碼，添加注釋 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class PromotionsDeductRateMgr : IPromotionsDeductRateImplMgr
    {
        private IPromotionsDeductRateImplDao _DRateDao;
        public PromotionsDeductRateMgr(string connectionString)
        {
            _DRateDao = new PromotionsDeductRateDao(connectionString);
        }
        #region 保存點數抵用新增或者修改的數據+int Save(Model.PromotionsDeductRate promoDRate)
        /// <summary>
        /// 保存點數抵用新增或者修改的數據
        /// </summary>
        /// <param name="promoDRate">點數抵用的Model</param>
        /// <returns></returns>
        public int Save(Model.PromotionsDeductRate promoDRate)
        {
            try
            {
                return _DRateDao.Save(promoDRate);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsDeductRateMgr-->Save-->" + ex.Message, ex);
            }
        } 
        #endregion

        #region 更新點數抵用的數據+int Update(Model.PromotionsDeductRate promoDRate)
        /// <summary>
        /// 更新點數抵用的數據
        /// </summary>
        /// <param name="promoDRate">點數抵用的Model</param>
        /// <returns></returns>
        public int Update(Model.PromotionsDeductRate promoDRate)
        {
            try
            {
                return _DRateDao.Update(promoDRate);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsDeductRateMgr-->Update-->" + ex.Message, ex);
            }
            
        } 
        #endregion

        #region 刪除點數抵用數據+int Delete(Model.PromotionsDeductRate pdr)
        /// <summary>
        /// 刪除點數抵用數據
        /// </summary>
        /// <param name="pdr">點數抵用數據的Model</param>
        /// <returns></returns>
        public int Delete(Model.PromotionsDeductRate pdr)
        {
            try
            {
                return _DRateDao.Delete(pdr);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsDeductRateMgr-->Delete-->" + ex.Message, ex);
            }
        } 
        #endregion

        #region 查詢點數抵用的數據+List<Model.Query.PromotionsDeductRateQuery> Query(Model.Query.PromotionsDeductRateQuery store, out int totalCount)
        /// <summary>
        /// 查詢點數抵用的數據
        /// </summary>
        /// <param name="store">點數抵用的Model</param>
        /// <param name="totalCount">查出數據總數</param>
        /// <returns>list</returns>
        public List<Model.Query.PromotionsDeductRateQuery> Query(Model.Query.PromotionsDeductRateQuery store, out int totalCount)
        {
            try
            {
                return _DRateDao.QueryAll(store, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("PromotionsDeductRateMgr-->Query-->" + ex.Message, ex);
            }
            
        } 
        #endregion

        #region 獲取一條數據根據根據id+Model.PromotionsDeductRate GetMOdel(int id)
        /// <summary>
        /// 獲取一條數據根據根據id
        /// </summary>
        /// <param name="id">編號</param>
        /// <returns></returns>
        public Model.PromotionsDeductRate GetMOdel(int id)
        {
            try
            {
                return _DRateDao.GetModel(id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsDeductRateMgr-->GetModel-->" + ex.Message, ex);
            }
        } 
        #endregion

        #region 更改活動使用狀態+int UpdateActive(Model.PromotionsDeductRate store)
        /// <summary>
        /// 更改活動使用狀態
        /// </summary>
        /// <param name="store">點數抵用的Model</param>
        /// <returns></returns>
        public int UpdateActive(Model.PromotionsDeductRate store)
        {
            try
            {
                return _DRateDao.UpdateActive(store);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsDeductRateMgr-->UpdateActive-->" + ex.Message, ex);
            }
        } 
        #endregion
    }
}
