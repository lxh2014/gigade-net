using BLL.gigade.Common;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace BLL.gigade.Mgr
{
    public class TicketDetailMgr : ITicketDetailImplMgr
    {
        private ITicketDetailImplDao _ITicketDetailDao;
        private IDBAccess _access;
        private string connStr;
        public TicketDetailMgr(string connectionString)
        {
            _ITicketDetailDao = new TicketDetailDao(connectionString);
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString; 
        }
        public DataTable GetTicketDetailTable(TicketDetailQuery query, out int totalCount)
        {
            try
            {
                return _ITicketDetailDao.GetTicketDetailTable(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("TicketDetailMgr-->GetTicketDetailTable" + ex.Message, ex);
            }
        }
        public int UpdateTicketStatus(string RowId)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(_ITicketDetailDao.UpdateTicketStatus(RowId));
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TicketDetailMgr-->UpdateTicketStatus" + ex.Message + sb.ToString(), ex);
            }

        }
        public bool UpLoadFile(HttpPostedFileBase httpPostedFile, string serverPath, string fileName, string extensions, int maxSize, int minSize, ref string ErrorMsg, string ftpUser, string ftpPasswd)
        {
            try
            {
                return _ITicketDetailDao.UpLoadFile(httpPostedFile, serverPath, fileName, extensions, maxSize, minSize, ref ErrorMsg, ftpUser, ftpPasswd);
            }
            catch (Exception ex)
            {
                throw new Exception("TicketDetailMgr-->UpLoadFile" + ex.Message, ex);
            }
        }


        /// <summary>
        /// 供應商後臺檢視使用序號功能 列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable GetTicketDetailAllCodeTable(TicketDetailQuery query, out int totalCount)
        {
            try
            {
                return _ITicketDetailDao.GetTicketDetailAllCodeTable(query, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("TicketDetailMgr-->GetTicketDetailAllCodeTable" + ex.Message, ex);
            }
        }
    }
}
