using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class AppversionsDao : IAppversionsImplDao
    {
        private IDBAccess _access;
        private string connStr;
        string strSql = string.Empty;
        public AppversionsDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }
        /// <summary>
        /// 查詢分頁方法
        /// </summary>
        /// <param name="appsions"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<Model.AppversionsQuery> GetAppversionsList(Model.AppversionsQuery appsions, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            sql.AppendFormat("SELECT id,versions_id,versions_code,versions_name,versions_desc,drive,release_type,FROM_UNIXTIME(release_date) as releasedateQuery FROM appversions where 1=1 ");
            sqlcount.AppendFormat("SELECT id FROM appversions where 1=1 ");
            totalCount = 0;
            try
            {
                if (appsions.drive != -1)
                {
                    sqlwhere.AppendFormat(@" AND   drive={0} ", appsions.drive);
                }
                sqlcount.AppendFormat(sqlwhere.ToString());
                sql.AppendFormat(sqlwhere.ToString());
                sql.AppendFormat(" order by  release_date DESC ");
                if (appsions.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sqlcount.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }
                    sql.AppendFormat(" limit {0},{1}", appsions.Start, appsions.Limit);
                }
                return _access.getDataTableForObj<AppversionsQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AppversionsDao.GetAppversionsList-->" + ex.Message + sql.ToString(), ex);
            }
        }
        /// <summary>
        /// 通過ID刪除
        /// </summary>
        /// <param name="rowid"></param>
        /// <returns></returns>
        public int DeleteAppversionsById(string rowid)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"DELETE FROM appversions WHERE id in {0};", "(" + rowid + ")");
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AppversionsDao.DeleteAppversionsById-->" + ex.Message + sql.ToString(), ex);
            }
        }
        /// <summary>
        /// Appnotifypool表增加方法
        /// </summary>
        /// <param name="anpq"></param>
        /// <returns></returns>
        public int AddAppversions(Model.AppversionsQuery anpq)
        {
            try
            {
                anpq.Replace4MySQL();
                strSql = string.Format(@"insert into appversions(versions_id,versions_code,versions_name,versions_desc,drive,release_date,release_type) 
                                                    values({0},{1},'{2}','{3}',{4},{5},{6});select @@identity",
                        anpq.versions_id, anpq.versions_code,anpq.versions_name, anpq.versions_desc, anpq.drive, anpq.release_date,anpq.release_type
                );
                return _access.execCommand(strSql);
            }
            catch (Exception ex)
            {
                throw new Exception("AppversionsDao-->AddAppversions" + ex.Message + strSql.ToString(), ex);
            }

        }


        public int UpdateAppversionsActive(int id, int status)
        {
            StringBuilder str = new StringBuilder();
            try
            {

                str.AppendFormat(@"update appversions set release_type='{0}' where id='{1}';", status, id);
                return _access.execCommand(str.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AppversionsDao-->UpdateAppversionsActive" + ex.Message + str.ToString(), ex);
            }
        }


        public int EditAppversions(AppversionsQuery appsions)
        {
            StringBuilder str = new StringBuilder();
            try
            {
                appsions.Replace4MySQL();
                str.AppendFormat(@"update appversions set versions_id='{0}',versions_code='{1}',versions_name='{2}', versions_desc='{3}',drive='{4}',release_date='{5}',release_type='{6}' where id='{7}';"
                    ,appsions.versions_id,appsions.versions_code,appsions.versions_name,appsions.versions_desc,appsions.drive,appsions.release_date,appsions.release_type,appsions.id); 
                return _access.execCommand(str.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AppversionsDao-->EditAppversions" + ex.Message + str.ToString(), ex);
            }
        }
    }
}
