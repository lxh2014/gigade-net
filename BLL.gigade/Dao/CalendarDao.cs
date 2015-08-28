using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class CalendarDao:ICalendarImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        private string connStr;
        public CalendarDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connStr = connectionString;
        }

        public List<Calendar> Query()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("SELECT id,calendarId AS CalendarId,title AS Title,startDate AS StartDateStr,endDate AS EndDateStr,notes AS Notes FROM event;");
                return _access.getDataTableForObj<Calendar>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CalendarDao-->Query" + ex.Message,ex) ;
            }
        }

        public int Save(Calendar c)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"INSERT INTO event(`calendarId`,`title`,`startDate`,`endDate`,`notes`)VALUES({0},'{1}','{2}','{3}','{4}');", c.CalendarId, c.Title, c.StartDateStr, c.EndDateStr, c.Notes);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CalendarDao-->Save" + ex.Message, ex);
            }       
        }

        public int Update(Calendar c)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"UPDATE `event` SET calendarId = {0},title ='{1}', startDate = '{2}', endDate = '{3}',notes = '{4}' WHERE id = {5};", c.CalendarId, c.Title, c.StartDateStr, c.EndDateStr, c.Notes, c.id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CalendarDao-->Update" + ex.Message, ex);
            }
        }

        public int Delete(Calendar c)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"DELETE FROM `event` WHERE id = {0} order by id desc", c.id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CalendarDao---->Delete" + ex.Message, ex);
            }
        }


        //add by wwei0216w 2015/5/22
        public List<Calendar> GetCalendarInfo(Calendar c)
        {
            ///根據條件查詢行事歷控件信息,可在if后添加條件 edit by wwei0216w 2015/5/22
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@"SELECT id,calendarId AS CalendarId,title AS Title,startDate AS StartDateStr,endDate AS EndDateStr,notes AS Notes
                                FROM event 
                            WHERE 1=1");
                if (c.EndDateStr != string.Empty)
                {
                    sb.AppendFormat(" AND calendarId = 2 AND '{0}' < endDate", c.EndDateStr);
                }
                else
                {
                    return new List<Calendar>();
                }
                return _access.getDataTableForObj<Calendar>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("CalendarDao-->GetCalendarInfo" + ex.Message,ex) ;
            }
        }
    }
}
