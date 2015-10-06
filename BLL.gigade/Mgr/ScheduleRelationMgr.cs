using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Working;

namespace BLL.gigade.Mgr
{
    public class ScheduleRelationMgr : IScheduleRelationImplMgr
    {
        private Dao.Impl.IScheduleRelationImplDao _scheduleRelationDao;
        private string connectionString;
        public ScheduleRelationMgr(string connectionString)
        {
            this.connectionString = connectionString;
            _scheduleRelationDao = new Dao.ScheduleRelationDao(connectionString);
        }

        public bool Delete(string relateion, int relation_id)
        {
            return _scheduleRelationDao.Delete(relateion,relation_id)>0;
        }

        public List<ScheduleRelation> Query(int schedule_id)
        {
            return _scheduleRelationDao.Query(schedule_id);
        }

        public List<ScheduleRelation> Query(ScheduleRelation sr)
        {
            try 
	        {
                return _scheduleRelationDao.Query(sr);
	        }
	        catch (Exception ex)
	        {
                throw new Exception("ScheduleRelationMgr-->Query" + ex.Message,ex);
	        }
        }

        public bool Save(ScheduleRelation sr)
        {
            try
            {
                if (Query(sr).Count > 0)
                {
                    if (sr.schedule_id == 0)
                    {
                        return Delete(sr.relation_table, sr.relation_id);
                    }
                    else
                    {
                        return _scheduleRelationDao.Update(sr) > 0;
                    }
                }
                else 
                {
                    if (sr.schedule_id != 0)
                        return _scheduleRelationDao.Save(sr) > 0;
                    else
                        return true;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleRelationMgr-->Save" + ex.Message,ex);
            }
        }

        /// <summary>
        /// 獲取最近出貨時間
        /// </summary>
        public DateTime GetRecentlyTime(int relationId, string relationType)
        {
            try
            {
                var _scheduleRelation = _scheduleRelationDao.Query(new ScheduleRelation { relation_id = relationId, relation_table = relationType }).FirstOrDefault();
                //_scheduleRelation = new ScheduleRelation();
                if (_scheduleRelation == null) return DateTime.MinValue;
                //_scheduleRelation.schedule.type = 3;
                //_scheduleRelation.schedule.duration_start = Convert.ToDateTime("2015-09-01");
                //_scheduleRelation.schedule.duration_end = Convert.ToDateTime("2015-10-30");
                //_scheduleRelation.schedule.execute_days = "1,1,3";
                //_scheduleRelation.schedule.trigger_time = "0,49,50,119,120,167";
                Work work = new Work(_scheduleRelation.schedule);
                return work.CurrentExecuteDate();
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleMgr-->GetRecentlyTime" + ex.Message, ex);
            }
        }
    }
}
