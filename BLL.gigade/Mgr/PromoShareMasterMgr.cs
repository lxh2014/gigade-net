using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class PromoShareMasterMgr
    {
        private PromoShareMasterDao _PshareDao;
        public PromoShareMasterMgr(string connectionStr)
        {
            _PshareDao = new PromoShareMasterDao(connectionStr);
        }
        #region 新增
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">PromoShareMaster對象</param>
        /// <returns>新增后的標識</returns>
        public int Add(PromoShareMaster model)
        {
            try
            {
               return _PshareDao.Add(model);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareMasterMgr-->Add-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">PromoShareMaster對象</param>
        /// <returns>更新結果</returns>
        public int Update(PromoShareMaster model)
        {
            try
            {
                return _PshareDao.Update(model);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareMasterMgr-->Update-->" + ex.Message, ex);
            }
        }
        #endregion

        public int UpdateActivePromoShareMaster(PromoShareMaster model)
        {
            try
            {
                return _PshareDao.UpdateActivePromoShareMaster(model);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareMasterDao-->UpdateActivePromoShareMaster-->" + ex.Message, ex);
            }
        }
        public int DeletePromoShareMessage(string str)
        {
            try
            {
                return _PshareDao.DeletePromoShareMessage(str);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareMasterDao-->DeletePromoShareMessage-->" + ex.Message, ex);
            }
        }
        


        #region 查詢
        #region 通過id獲取PromoShareMaster對象
        /// <summary>
        /// 通過id獲取PromoShareMaster對象
        /// </summary>
        /// <param name="promo_id">編號</param>
        /// <returns>PromoShareMaster對象</returns>
        public PromoShareMaster Get(int promo_id)
        {
            try
            {
                return _PshareDao.Get(promo_id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareMaster-->Get-->"+ex.Message, ex);
            }
        }
        #endregion

        #region 通過查詢條件獲取PromoShareMaster列表
        /// <summary>
        /// 通過查詢條件獲取PromoShareMaster列表
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <param name="totalCount">數據總條數</param>
        /// <returns>PromoShareMaster列表</returns>
        public DataTable GetList(PromoShareMasterQuery query, out int totalCount)
        {
            try
            {
                return _PshareDao.GetList(query,out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareMasterMgr-->GetList-->" + ex.Message, ex);
            }
        }
        #endregion
        #endregion

        public int PromoCon(PromoShareMaster query)
        {
            try
            {
                return _PshareDao.PromoCon(query);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareMasterMgr-->PromoCon-->"+ex.Message,ex);
            }
        }
    }
}
