#region 文件信息
/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：PromoAdditionalPriceMgr.cs
 * 摘   要： 
 *      不同品加不同價等
 * 当前版本：v1.1 
 * 作   者： jialei0706j dongya0410j
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
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{ 
    public class PromoAdditionalPriceMgr : IPromoAdditionalPriceMgr
    {
        private IPromoAdditionalPriceDao _PAProceDao;
        public PromoAdditionalPriceMgr(string connectionString)
        {
            _PAProceDao = new PromoAdditionalPriceDao(connectionString);
        }
        #region 不同商品固定價 同品加固定價 不同品加不同價格 列表頁 +List<Model.Query.PromoAdditionalPriceQuery> Query(Model.Query.PromoAdditionalPriceQuery store, out int totalCount)
        public List<Model.Query.PromoAdditionalPriceQuery> Query(Model.Query.PromoAdditionalPriceQuery store, out int totalCount)
        {
            try
            {
                return _PAProceDao.QueryAll(store, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoAdditionalPriceMgr-->Query-->" + ex.Message, ex);
            }
        }

        #endregion

        #region 不同商品固定價 同品加固定價 不同品加不同價格 插入第一步 +int InsertFirst(Model.PromoAdditionalPrice m)
        public int InsertFirst(Model.PromoAdditionalPrice m)
        {
            try
            {
                return _PAProceDao.InsertFirst(m);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoAdditionalPriceMgr-->InsertFirst-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 不同商品固定價 同品加固定價 不同品加不同價格 改變狀態 +int ChangeActive(Model.Query.PromoAdditionalPriceQuery m)
        public int ChangeActive(Model.Query.PromoAdditionalPriceQuery m)
        {
            try
            {
                return _PAProceDao.ChangeActive(m);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoAdditionalPriceMgr-->ChangeActive-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 不同商品固定價 同品加固定價 不同品加不同價格 刪除 +int Delete(int i, string str)
        public int Delete(int i, string str)
        {
            try
            {
                return _PAProceDao.Delete(i, str);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoAdditionalPriceMgr-->Delete-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 不同商品固定價 同品加固定價 不同品加不同價格 getmodel +Model.PromoAdditionalPrice GetModel(int id)
        public Model.PromoAdditionalPrice GetModel(int id)
        {
            try
            {
                return _PAProceDao.GetModel(id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoAdditionalPriceMgr-->GetModel-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 不同商品固定價 同品加固定價 不同品加不同價格 +Model.Query.PromoAdditionalPriceQuery Select(int id)
        public Model.Query.PromoAdditionalPriceQuery Select(int id)
        {
            try
            {
                return _PAProceDao.Select(id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoAdditionalPriceMgr-->Select-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 不同商品固定價 同品加固定價 不同品加不同價格 插入第二部 +int InsertSecond(Model.PromoAdditionalPrice m, Model.Query.PromoAdditionalPriceQuery mq)
        public int InsertSecond(Model.PromoAdditionalPrice m, Model.Query.PromoAdditionalPriceQuery mq)
        {
            try
            {
                return _PAProceDao.InsertSecond(m, mq);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoAdditionalPriceMgr-->InsertSecond-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 不同商品固定價 同品加固定價 不同品加不同價格 +string CategoryID(Model.PromoAdditionalPrice m)
        public string CategoryID(Model.PromoAdditionalPrice m)
        {
            try
            {
                return _PAProceDao.CategoryID(m);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoAdditionalPriceMgr-->CategoryID-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 不同商品固定價 同品加固定價 不同品加不同價格 編輯 +int Update(Model.PromoAdditionalPrice m, Model.Query.PromoAdditionalPriceQuery mq)
        public int Update(Model.PromoAdditionalPrice m, Model.Query.PromoAdditionalPriceQuery mq)
        {
            try
            {
                return _PAProceDao.Update(m, mq);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoAdditionalPriceMgr-->Update-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 把低於商品加購價的促銷商品刪除掉
        public int DeletLessThen(PromoAdditionalPriceQuery m, int types)
        {
            try
            {
                return _PAProceDao.DeletLessThen(m, types);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoAdditionalPriceMgr-->DeletLessThen-->" + ex.Message, ex);
            }
        }
        #endregion

    }
}
