using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class VoteDetailMgr
    {
        private VoteDetailDao _voteDetailDao;
        public VoteDetailMgr(string connectionStr)
        {
            _voteDetailDao = new VoteDetailDao(connectionStr);
        }
        #region 新增
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">VoteDetail對象</param>
        /// <returns>新增后的標識</returns>
        public int Add(VoteDetail model)
        {
            try
            {
               return _voteDetailDao.Add(model);
            }
            catch (Exception ex)
            {
                throw new Exception("VoteDetailMgr-->Add-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">VoteDetail對象</param>
        /// <returns>更新結果</returns>
        public int Update(VoteDetail model)
        {
            try
            {
                return _voteDetailDao.Update(model);
            }
            catch (Exception ex)
            {
                throw new Exception("VoteDetailMgr-->Update-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 查詢
        #region 通過id獲取VoteDetail對象
        /// <summary>
        /// 通過id獲取VoteDetail對象
        /// </summary>
        /// <param name="promo_id">編號</param>
        /// <returns>VoteDetail對象</returns>
        public VoteDetail Get(int promo_id)
        {
            try
            {
                return _voteDetailDao.Get(promo_id);
            }
            catch (Exception ex)
            {
                throw new Exception("VoteDetail-->Get-->"+ex.Message, ex);
            }
        }
        #endregion

        #region 通過查詢條件獲取VoteDetail列表
        /// <summary>
        /// 通過查詢條件獲取VoteDetail列表
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <param name="totalCount">數據總條數</param>
        /// <returns>VoteDetail列表</returns>
        public List<VoteDetailQuery> GetList(VoteDetailQuery query, out int totalCount)
        {
            try
            {
                return _voteDetailDao.GetList(query,out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("VoteDetailMgr-->GetList-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 匯出Excel
        public DataTable GetDtVoteDetail(VoteDetailQuery query, out int totalCount)
        {
            try
            {
                return _voteDetailDao.GetDtVoteDetail(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("VoteDetailMgr-->GetDtVoteDetail-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 更改狀態
        public int UpdateVoteDetaiStatus(VoteDetailQuery query)
        {
            try
            {
                return _voteDetailDao.UpdateVoteDetaiStatus(query);
            }
            catch (Exception ex)
            {
                throw new Exception("VoteDetailMgr-->UpdateVoteDetaiStatus-->" + ex.Message, ex);
            }
        }

        #endregion
        #endregion
    }
}
