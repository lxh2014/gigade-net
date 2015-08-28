using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class VoteEventMgr
    {
        private VoteEventDao veDao;
        public VoteEventMgr(string connectionstring)
        {
            veDao = new VoteEventDao(connectionstring);
        }
        /// <summary>
        /// 列表頁
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalcount"></param>
        /// <returns></returns>
        public List<VoteEventQuery> GetVoteEventList(VoteEventQuery query, out int totalcount)
        {
            try
            {
                return veDao.GetVoteEventList(query, out totalcount);
            }
            catch (Exception ex)
            {

                throw new Exception("VoteEventMgr-->GetVoteEventList-->" + ex.Message, ex);
            }

        }
        /// <summary>
        /// 活動名稱下拉列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<VoteEventQuery> GetVoteEventDownList(VoteEventQuery query)
        {
            try
            {
                return veDao.GetVoteEventDownList(query);
            }
            catch (Exception ex)
            {

                throw new Exception("VoteEventMgr-->GetVoteEventDownList-->" + ex.Message, ex);
            }

        }
        public int Save(VoteEvent ve)
        {
            try
            {
                return veDao.Save(ve);
            }
            catch (Exception ex)
            {
                throw new Exception("VoteEventMgr-->Save-->" + ex.Message, ex);
            }
        }

        public int Update(VoteEvent ve)
        {
            try
            {
                return veDao.Update(ve);
            }
            catch (Exception ex)
            {
                throw new Exception("VoteEventMgr.Update-->" + ex.Message, ex);
            }
        }
        public int UpdateState(VoteEvent ve)
        {
            try
            {
                return veDao.UpdateState(ve);
            }
            catch (Exception ex)
            {
                throw new Exception("VoteEventMgr-->UpdateState-->" + ex.Message, ex);
            }
        }
        public int SelectByEventName(VoteEvent m)
        {
            try
            {
                return veDao.SelectByEventName(m);
            }
            catch (Exception ex)
            {
                throw new Exception("VoteEventMgr-->SelectByEventName-->" + ex.Message, ex);
            }
        }
    }
}
