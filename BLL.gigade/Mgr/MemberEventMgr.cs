using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Common;
namespace BLL.gigade.Mgr
{
    public class MemberEventMgr
    {
        private MemberEventDao _MemberEventDao;
        public MemberEventMgr(string connectionString)
        {
            _MemberEventDao = new MemberEventDao(connectionString);
        }

        /// <summary>
        /// 會員活動列表查詢
        /// </summary>
        /// <param name="store"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<MemberEventQuery> Query(MemberEventQuery store, out int totalCount)
        {
            try
            {
                return _MemberEventDao.Query(store, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("MemberEventMgr-->Query-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 會員活動新增編輯
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int MemberEventSave(MemberEventQuery query)
        {
            try
            {
                return _MemberEventDao.MemberEventSave(query);
            }
            catch (Exception ex)
            {

                throw new Exception("MemberEventMgr-->MemberEventSave-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 更新狀態
        /// </summary>
        /// <param name="meq"></param>
        /// <returns></returns>
        public int UpdateState(MemberEventQuery meq)
        {
            try
            {
                return _MemberEventDao.UpdateState(meq);
            }
            catch (Exception ex)
            {
                throw new Exception("MemberEventMgr-->UpdateState-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 會員等級列表
        /// </summary>
        /// <param name="ml"></param>
        /// <returns></returns>
        public DataTable GetMemberLevelList(MemberLevel ml)
        {
            try
            {
                return _MemberEventDao.GetMemberLevelList(ml);
            }
            catch (Exception ex)
            {
                throw new Exception("MemberEventMgr-->GetMemberLevelList-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 會員級別保存
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int EventGroupSave(MemberEventQuery query)
        {
            try
            {
                return _MemberEventDao.MemberEventSave(query);
            }
            catch (Exception ex)
            {

                throw new Exception("MemberEventMgr-->EventGroupSave-->" + ex.Message, ex);
            }
        }
        ///// <summary>
        ///// 群組名稱
        ///// </summary>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //public string GetMlName(MemberEventQuery query)
        //{
        //    try
        //    {
        //        return _MemberEventDao.GetMlName(query);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw new Exception("MemberEventMgr-->GetMlName-->" + ex.Message, ex);
        //    }
        //}

        public bool IsRepeat(MemberEventQuery query)
        {
            string json = string.Empty;
            bool result = true;
            DataTable _dt=new DataTable ();
            try
            {
                if (query.rowID != 0)
                {
                    _dt = _MemberEventDao.UpdateRepeat(query);
                 
                }
                else
                {
                    _dt = _MemberEventDao.InsertRepeat(query);

                }
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    result = false;//數據重複
                }
                else
                {
                    result = true;//不重複
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("MemberEventMgr-->IsRepeat-->" + ex.Message, ex);
            }
        }

        public string IsGetEventID(string event_id)
        {
            string json = string.Empty;
            try
            {
                DataTable _dt = _MemberEventDao.IsGetEventID(event_id);
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    json = "{success:true,time1:'"+CommonFunction.DateTimeToString(Convert.ToDateTime(_dt.Rows[0][0]))+"',time2:'"+CommonFunction.DateTimeToString( Convert.ToDateTime(_dt.Rows[0][1]))+"'}";
                }
                else
                {
                    json = "{success:false}";
                }
                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("MemberEventMgr-->IsGetEventID-->" + ex.Message, ex);
            }
        }
    }
}
