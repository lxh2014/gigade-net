/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromoAllMgr.cs
* 摘 要：
* 點數抵用與資料庫交互方法  
* 当前版本：v1.1
* 作 者：hongfei0416j    
* 完成日期：2014/6/20 
* 修改歷史:
*         v1.1修改日期：2014/8/15 
*         v1.1修改人員：zhejiang0304j 
*         v1.1修改内容：在抛出異常的時候將Sql語句抛出，合并代碼，添加注釋 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class PromoAllMgr : IPromoAllImplMgr
    {
        private IPromoAllImplDao _paDao;
        public PromoAllMgr(string connectionstring)
        {
            _paDao = new PromoAllDao(connectionstring);
        }

        #region 根據條件獲取促銷列表+List<PromoAll> GetList(PromoAllQuery query)
        /// <summary>
        /// 根據條件獲取促銷列表
        /// </summary>
        /// <param name="query">查詢條件model</param>
        /// <returns>促銷信息列表</returns>
        public List<PromoAll> GetList(PromoAllQuery query)
        {
            try
            {
                return _paDao.GetList(query);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoAllMgr-->GetList-->" + ex.Message, ex);
            }
        } 
        #endregion

        #region 保存promo_all表的數據+int Save(Model.PromoAll model)
        /// <summary>
        /// 保存promo_all表的數據
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Save(Model.PromoAll model)
        {
            try
            {
                return _paDao.Save(model);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoAllMgr-->Save-->" + ex.Message, ex);
            }
        } 
        #endregion

        #region 刪除promo_all表的數據+int Delete(Model.PromoAll model)
        /// <summary>
        /// 刪除promo_all表的數據
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Delete(Model.PromoAll model)
        {
            try
            {
                return _paDao.Delete(model);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoAllMgr-->Delete-->" + ex.Message, ex);
            }
        } 
        #endregion
    }
}
