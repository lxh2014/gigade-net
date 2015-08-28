using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;

namespace BLL.gigade.Mgr
{
    public class PromoShareConditionMgr
    {
        private PromoShareConditionDao _pscDao;
        private MySqlDao _mysqlDao;
        public PromoShareConditionMgr(string connectionStr)
        {
            _pscDao = new PromoShareConditionDao(connectionStr);
            _mysqlDao = new MySqlDao(connectionStr);
        }
        #region 新增
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">PromoShareCondition對象</param>
        /// <returns>新增后的標識</returns>
        public int Add(PromoShareCondition model)
        {
            try
            {
               return _pscDao.Add(model);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareConditionMgr-->Add-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">PromoShareCondition對象</param>
        /// <returns>新增數據的Sql語句</returns>
        public bool AddSql(List<PromoShareCondition>list)
        {
            try
            {
                ArrayList arrlist = new ArrayList();
                for (int i = 0; i < list.Count; i++)
                {
                    arrlist.Add(_pscDao.AddSql(list[i]));
                }
                return _mysqlDao.ExcuteSqls(arrlist);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareConditionMgr-->AddSql-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">PromoShareCondition對象</param>
        /// <returns>更新結果</returns>
        public int Update(PromoShareCondition model)
        {
            try
            {
                return _pscDao.Update(model);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareConditionMgr-->Update-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 查詢
        #region 通過id獲取PromoShareCondition對象
        /// <summary>
        /// 通過id獲取PromoShareCondition對象
        /// </summary>
        /// <param name="promo_id">編號</param>
        /// <returns>PromoShareCondition對象</returns>
        public PromoShareCondition Get(int promo_id)
        {
            try
            {
                return _pscDao.Get(promo_id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareCondition-->Get-->"+ex.Message, ex);
            }
        }
        #endregion

        #region 通過查詢條件獲取PromoShareCondition列表
        /// <summary>
        /// 通過查詢條件獲取PromoShareCondition列表
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <param name="totalCount">數據總條數</param>
        /// <returns>PromoShareCondition列表</returns>
        public List<PromoShareConditionQuery> GetList(PromoShareConditionQuery query)
        {
            try
            {
                return _pscDao.GetList(query);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareConditionMgr-->GetList-->" + ex.Message, ex);
            }
        }
        #endregion

        public DataTable Get(string[] condition, PromoShareCondition query)
        {
            try
            {
                return _pscDao.Get(condition, query);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareConditionMgr-->Get-->" + ex.Message, ex);
            }
        }

        public int GetPromoShareConditionCount(PromoShareCondition query)
        {
            try
            {
                return _pscDao.GetPromoShareConditionCount(query);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareConditionMgr-->GetPromoShareConditionCount-->" + ex.Message, ex);
            }
        }
        #endregion

        public int Delete(int promo_id)
        {
            try
            {
             return   _pscDao.Delete(promo_id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareConditionMgr-->Delete-->" + ex.Message, ex);
            }
        }
    }
}
