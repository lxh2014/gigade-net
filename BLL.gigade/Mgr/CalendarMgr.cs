using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class CalendarMgr:ICalendarImplMgr
    {
        ICalendarImplDao clDao;
        public CalendarMgr(string connectionString)
        {
            clDao = new CalendarDao(connectionString);
        }

        public List<Calendar> Query()
        {
            try
            {
                return clDao.Query();
            }
            catch (Exception ex)
            {
                throw new Exception("CalendarMgr-->Query" + ex.Message,ex);
            }
        }

        public bool Save(Calendar c)
        {
            try
            {
                return clDao.Save(c) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("CalendarMgr-->Save" + ex.Message, ex);
            }
        }

        public bool Update(Calendar c)
        {
            try
            {
                return clDao.Update(c) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("CalendarMgr-->Update" + ex.Message, ex);
            }
        }

        public bool Delete(Calendar c)
        {
            try
            {
                return clDao.Delete(c) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("CalendarMgr-->Delete" + ex.Message, ex);
            }
        }

        public List<Calendar> GetCalendarInfo(Calendar c)
        {
            try
            {
                return clDao.GetCalendarInfo(c);
            }
            catch (Exception ex)
            {
                throw new Exception("CalendarMgr-->GetCalendarInfo" + ex.Message,ex);
            }
        }
    }
}
