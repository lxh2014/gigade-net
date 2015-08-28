using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class VoteMessageMgr
    {
         private VoteMessageDao _voteMsgDao;
        private MySqlDao _mysqlDao;
        public VoteMessageMgr(string connectionStr)
        {
            _voteMsgDao = new VoteMessageDao(connectionStr);
            _mysqlDao = new MySqlDao(connectionStr);
        }
        /// <summary>
        /// 回復消息列表頁
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<VoteMessageQuery> GetVoteMessageList(VoteMessageQuery query,out int totalCount)
        {
            try
            {
                return _voteMsgDao.GetVoteMessageList(query,out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("VoteMessageMgr-->AddVoteMessage-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddVoteMessage(VoteMessageQuery model)
        {
            try
            {
                return _voteMsgDao.AddVoteMessage(model);
            }
            catch (Exception ex)
            {
                throw new Exception("VoteMessageMgr-->AddVoteMessage-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 編輯
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateVoteMessage(VoteMessageQuery model)
        {
            try
            {
                return _voteMsgDao.UpdateVoteMessage(model);
            }
            catch (Exception ex)
            {
                throw new Exception("VoteMessageMgr-->UpdateVoteMessage-->" + ex.Message, ex);
            }

        }
        /// <summary>
        ///刪除列表頁
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int DelVoteMessage(VoteMessageQuery query)
        {
            try
            {
                return _voteMsgDao.DelVoteMessage(query);
            }
            catch (Exception ex)
            {
                throw new Exception("VoteMessageMgr-->DelVoteMessage-->" + ex.Message, ex);
            }

        }
        /// <summary>
        ///更改活動狀態
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int UpVoteMessageStatus(VoteMessageQuery query)
        {
            int result = 0;
            try
            {
                //if (query.message_status == 0)
                //{
                    result = _voteMsgDao.UpVoteMessageStatus(query);
                //}
                //else 
                //{
                    
                //}
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("VoteMessageMgr-->UpVoteMessageStatus-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 通過ID查詢留言信息
        /// </summary>
        /// <param name="message_id"></param>
        /// <returns></returns>
        public VoteMessageQuery GetVoteMessageQueryById(int message_id)
        {
            try
            {
                return _voteMsgDao. GetVoteMessageQueryById( message_id);
            }
            catch (Exception ex)
            {
                throw new Exception("VoteMessageMgr-->GetVoteMessageQueryById-->" + ex.Message, ex);
            }
        }
    }
}
