using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class PromoShareConditionTypeMgr
    {
        private PromoShareConditionTypeDao _psctDao;
        public PromoShareConditionTypeMgr(string connectionStr)
        {
            _psctDao = new PromoShareConditionTypeDao(connectionStr);
        }
        #region 新增
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">PromoShareConditionType對象</param>
        /// <returns>新增后的標識</returns>
        public int Add(PromoShareConditionType model)
        {
            try
            {
               return _psctDao.Add(model);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareConditionTypeMgr-->Add-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">PromoShareConditionType對象</param>
        /// <returns>更新結果</returns>
        public int Update(PromoShareConditionType model)
        {
            try
            {
                return _psctDao.Update(model);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareConditionTypeMgr-->Update-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 查詢
        #region 通過id獲取PromoShareConditionType對象
        /// <summary>
        /// 通過id獲取PromoShareConditionType對象
        /// </summary>
        /// <param name="promo_id">編號</param>
        /// <returns>PromoShareConditionType對象</returns>
        public PromoShareConditionType Get(int promo_id)
        {
            try
            {
                return _psctDao.Get(promo_id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareConditionType-->Get-->"+ex.Message, ex);
            }
        }
        #endregion

        #region 通過查詢條件獲取PromoShareConditionType列表
        /// <summary>
        /// 通過查詢條件獲取PromoShareConditionType列表
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <param name="totalCount">數據總條數</param>
        /// <returns>PromoShareConditionType列表</returns>
        public List<PromoShareConditionTypeQuery> GetList(PromoShareConditionTypeQuery query, out int totalCount)
        {
            try
            {
                return _psctDao.GetList(query,out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareConditionTypeMgr-->GetList-->" + ex.Message, ex);
            }
        }
        #endregion
        #endregion
    }
}
