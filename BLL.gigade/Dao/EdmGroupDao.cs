using BLL.gigade.Common;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class EdmGroupDao
    {
        private IDBAccess _access;
        private string connStr;
        public EdmGroupDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }
        public List<EdmGroup> GetEdmGroupList(EdmGroup query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            totalCount = 0;
            try
            {
                query.Replace4MySQL();
                sqlCount.Append("select count(eg.group_id)  as totalCount ");
                sql.Append(" select eg.group_id,eg.group_name,eg.group_total_email,eg.group_createdate,eg.group_updatedate,count(ec.content_id) as total_content ");
                sqlFrom.Append("from edm_group  eg ");
                sqlFrom.Append("  left join edm_content ec on	eg.group_id = ec.group_id ");
                sqlWhere.Append(" where 1=1  ");
                if (query.selectType == "0"&&!string.IsNullOrEmpty(query.search_con))
                {
                    sqlWhere.AppendFormat(" and  eg.group_id like '%{0}%'",query.search_con.ToString());
                }
                if (query.selectType == "1" && !string.IsNullOrEmpty(query.search_con))
                {
                    sqlWhere.AppendFormat(" and  eg.group_name like '%{0}%'", query.search_con.ToString());
                }
                if (query.dateCondition != -1)
                {
                    if (query.dateCondition == 0)
                    {
                        sqlWhere.AppendFormat("AND group_createdate BETWEEN {0} AND {1}", query.start, query.end);
                    }
                    if (query.dateCondition == 1)
                    {
                        sqlWhere.AppendFormat("AND group_updatedate BETWEEN {0} AND {1}", query.start, query.end);
                    }
                }               
                sqlWhere.AppendFormat("	group by eg.group_id, eg.group_name, eg.group_total_email, eg.group_createdate, eg.group_updatedate order by eg.group_id asc ");
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable(sqlCount.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }
                    sqlWhere.AppendFormat(" limit {0},{1};", query.Start, query.Limit);
                }
                return _access.getDataTableForObj<EdmGroup>(sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupDao-->GetEdmGroupList-->" + ex.Message + sql.ToString() + sqlWhere.ToString(), ex);
            }
        }

        public int UpEdmGroup(EdmGroup query)
        {
            StringBuilder sql = new StringBuilder();

            try
            {
                query.Replace4MySQL();
                sql.AppendFormat("set sql_safe_updates=0;update edm_group set group_name ='{0}',group_updatedate='{1}' where group_id='{2}';set sql_safe_updates=1;", query.group_name, CommonFunction.GetPHPTime(), query.group_id);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupDao-->UpEdmGroup-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public string InsertEdmGroup(EdmGroup query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("insert into edm_group (group_id,group_name,group_createdate,group_updatedate)values('{0}','{1}','{2}','{2}');", query.group_id, query.group_name, CommonFunction.GetPHPTime());
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupDao-->InsertEdmGroup-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public bool execSql(ArrayList arrayList)
        {
            try
            {
                MySqlDao myDao = new MySqlDao(connStr);
                return myDao.ExcuteSqls(arrayList);
            }
            catch (Exception ex)
            {
                throw new Exception(" EmsDao-->execSql--> " + arrayList + ex.Message, ex);
            }
        }

        public string DeleteEdmGroup(EdmGroup query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates=0;delete from edm_group where group_id='{0}';set sql_safe_updates=1;", query.group_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupDao-->DeleteEdmGroup-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string DeleteEdmGroupMail(EdmGroup query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates=0;delete from edm_group_email where group_id='{0}';set sql_safe_updates=1;", query.group_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupDao-->DeleteEdmGroup-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public DataTable Export(EdmGroup query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select	ee.email_address,ege.email_status,ege.email_name FROM	edm_email  ee, edm_group_email  ege where ege.group_id ='{0}' and	ege.email_id = ee.email_id order by ee.email_id asc;", query.group_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupDao-->DeleteEdmGroup-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public string UpdateEGE(EdmGroupEmailQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("UPDATE edm_group_email SET email_name='{0}',email_status='{1}',email_updatedate='{2}' WHERE group_id='{3}' AND email_id='{4}'", query.email_name, query.email_status, query.email_updatedate, query.group_id, query.email_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupDao-->UpdateEGE" + ex.Message + sql.ToString(), ex);
            }
        }

        public string insertEGEInfo(EdmGroupEmailQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("INSERT INTO edm_group_email (group_id,email_id,email_name,email_status,email_createdate,email_updatedate) VALUES('{0}','{1}','{2}','{3}','{4}','{5}')", query.group_id, query.email_id, query.email_name, query.email_status, query.email_createdate, query.email_updatedate);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupDao-->insertEGEInfo" + ex.Message + sql.ToString(), ex);
            }
        }

        public string insertEdmEmail(EdmEmailQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("INSERT INTO edm_email (email_id,email_name,email_address,email_check,email_createdate,email_updatedate) VALUES('{0}','{1}','{2}','{3}','{4}','{5}')", query.email_id, query.email_name, query.email_address, query.email_check, query.email_createdate, query.email_updatedate);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupEmailDao-->insertEdmEmail" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
