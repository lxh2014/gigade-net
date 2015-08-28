using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class ScheduleParamerMgr
    {
        private ScheduleParamerDao _scheduleDao;

        public ScheduleParamerMgr(string connectionString)
        {
            _scheduleDao = new ScheduleParamerDao(connectionString);
        }


        public DataTable GetScheduleParamerList(string code)
        {
            try
            {
                return _scheduleDao.GetScheduleParamerList(code);
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleParamerMgr-->GetScheduleParamerList-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 獲取ScheduleParamer表的list
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <returns></returns>
        public List<ScheduleParamer> GetScheduleParameterList(ScheduleParamer query, out int totaoCount)
        {
            try
            {
                return _scheduleDao.GetScheduleParameterList(query, out totaoCount);
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleParamerMgr-->GetScheduleParameterList" + ex.Message, ex);
            }
        }
        /// <summary>
        /// ScheduleParamer表新增
        /// </summary>
        /// <param name="query">新增數據</param>
        /// <returns></returns>
        public int InsertScheduleParamer(ScheduleParamer query)
        {
            try
            {
                return _scheduleDao.InsertScheduleParamer(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleParamerMgr-->InsertScheduleParamer" + ex.Message, ex);
            }
        }
        /// <summary>
        /// ScheduleParamer表編輯
        /// </summary>
        /// <param name="query">編輯信息</param>
        /// <returns></returns>
        public int UpdateScheduleParamer(ScheduleParamer query)
        {

            try
            {
                return _scheduleDao.UpdateScheduleParamer(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleParamerMgr-->UpdateScheduleParamer" + ex.Message, ex);
            }
        }
        /// <summary>
        /// ScheduleParamer表更改狀態
        /// </summary>
        /// <param name="query">更改信息</param>
        /// <returns></returns>
        public int UpdateActive(ScheduleParamer query)
        {
            try
            {
                return _scheduleDao.UpdateActive(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleParamerMgr-->UpdateActive" + ex.Message, ex);
            }
        }
        /// <summary>
        /// ScheduleParamer表刪除數據
        /// </summary>
        /// <param name="query">刪除數據信息</param>
        /// <returns></returns>
        public int DelScheduleParamer(ScheduleParamer query)
        {
            try
            {
                return _scheduleDao.DelScheduleParamer(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleParamerMgr-->DelScheduleParamer" + ex.Message, ex);
            }
        }
    }
}
