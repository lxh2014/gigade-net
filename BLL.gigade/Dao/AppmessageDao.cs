using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao
{
    public class AppmessageDao : IAppmessageImplDao
    {
        private IDBAccess _access;
        private string connStr;
        public AppmessageDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }

        public List<AppmessageQuery> GetAppmessageList(AppmessageQuery appmsg, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            CommonFunction.GetPHPTime();
            sql.AppendFormat("SELECT message_id,`type`,title,content,FROM_UNIXTIME(messagedate) as messagedate_time,`group`,linkurl,display_type,FROM_UNIXTIME(msg_start) as msg_start_time,FROM_UNIXTIME(msg_end) as msg_end_time,fit_os,appellation,need_login FROM appmessage where 1=1 ");
            sqlcount.AppendFormat("SELECT message_id FROM appmessage where 1=1 ");
            if (appmsg.msg_start != 0)
            {
                sqlwhere.AppendFormat(" and msg_start >= {0}", appmsg.msg_start);
            }
            if (appmsg.msg_end != 0)
            {
                sqlwhere.AppendFormat(" and msg_end <= {0} ", appmsg.msg_end);
            }
            totalCount = 0;
            try
            {
                sqlcount.AppendFormat(sqlwhere.ToString());
                sql.AppendFormat(sqlwhere.ToString());
                if (appmsg.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sqlcount.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }
                    
                    sql.AppendFormat(" limit {0},{1}", appmsg.Start, appmsg.Limit);
                }
                return _access.getDataTableForObj<AppmessageQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AppmessageDao.GetAppmessageList-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public List<Appmessage> GetParaList(string sql)
        {
            try
            {
                return _access.getDataTableForObj<Appmessage>(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("AppmessageDao.GetParaList-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public int AppMessageInsert(Appmessage appmsg)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(@"insert into appmessage (`type`,title,content,messagedate,linkurl,display_type,msg_end,msg_start,fit_os,appellation)");
                sql.AppendFormat(@" values('{0}','{1}','{2}','{3}','{4}','{5}'", appmsg.type, appmsg.title, appmsg.content, appmsg.messagedate, appmsg.linkurl, appmsg.display_type);
                sql.AppendFormat(@",'{0}','{1}','{2}','{3}');", appmsg.msg_end, appmsg.msg_start, appmsg.fit_os, appmsg.appellation);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AppmessageDao.AppMessageInsert-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
