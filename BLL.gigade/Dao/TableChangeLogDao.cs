using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Dao
{
    public class TableChangeLogDao
    {

        private IDBAccess _accessMySql;
         
        public TableChangeLogDao(string connectionstring)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionstring);

        } 

        public string insert(TableChangeLog model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("INSERT into table_change_log(user_type,pk_id,change_table,change_field,old_value,new_value,create_user,create_time,field_ch_name)");
                sql.AppendFormat(" values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');", model.user_type, model.pk_id, model.change_table, model.change_field, model.old_value, model.new_value, model.create_user, Common.CommonFunction.DateTimeToString(model.create_time),model.field_ch_name);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("TableChangeLogDao-->insert-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public DataTable GetVendorChangeLog(TableChangeLogQuery query, out int totalCount)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlW = new StringBuilder();
            StringBuilder sqlW1 = new StringBuilder();
            try
            {
                if (!string.IsNullOrEmpty(query.key))
                {
                    switch (query.key_type)
                    {
                        case 1:
                            query.key = query.key.Trim().Replace(' ', ',').Replace('，', ',').ToString();
                            sqlW.AppendFormat(" and vendor_id in ('{0}')", query.key);
                            break;
                        case 2:
                            query.key = query.key.Trim().Replace(' ', ',').Replace('，', ',').ToString();
                            sqlW.AppendFormat(" and vendor_code in ('{0}')", query.key);
                            break;
                        case 3:
                            query.key = query.key.Trim().ToString();
                            sqlW.AppendFormat(" and vendor_name_full like N'%{0}%'", query.key);
                            break;
                        default:
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(query.change_table))
                {
                    sqlW1.AppendFormat(" and change_table='{0}'", query.change_table);
                }
                if (query.date_one != DateTime.MinValue && query.date_two != DateTime.MinValue)
                {
                    if (query.d_type == 1)
                    {
                        sqlW.AppendFormat(" and kdate between '{0}' and '{1}'", Common.CommonFunction.DateTimeToString(query.date_one), Common.CommonFunction.DateTimeToString(query.date_two));
                    }
                    else
                    {
                        sqlW1.AppendFormat(" and create_time between '{0}' and '{1}'", Common.CommonFunction.DateTimeToString(query.date_one), Common.CommonFunction.DateTimeToString(query.date_two));
                    }

                }

                sql.Append(" SELECT DISTINCT v.vendor_id,v.vendor_code,v.vendor_name_full,v.kuser,v.kdate,c.create_user as muser,c.create_time as mdate");
                sql.AppendFormat(" FROM (SELECT vendor_id,vendor_code,vendor_name_full,kuser,kdate from vendor where 1=1 {0}  )v", sqlW.ToString());
                sql.AppendFormat(" INNER JOIN(select change_table,pk_id,create_user,create_time from table_change_log  where 1=1  {0} )c on c.pk_id=v.vendor_id", sqlW1.ToString());

                sql.Append(" order by c.create_time desc");
                totalCount = 0;
                if (query.IsPage)
                {

                    DataTable _dt = _accessMySql.getDataTable(@" select count(vendor_id) as totalCount from (" + sql.ToString() + ") a;");
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                        sql.AppendFormat(" limit {0},{1};", query.Start, query.Limit);
                    }
                }
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TableChangeLogDao-->GetVendorChangeLog-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public DataTable GetVendorChangeDetail(TableChangeLogQuery query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlW = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(query.change_table))
                {
                    sqlW.AppendFormat(" and change_table='{0}'", query.change_table);
                }
                if (query.pk_id != 0)
                {
                    sqlW.AppendFormat(" and pk_id='{0}'", query.pk_id);
                }
                if (query.create_user != 0)
                {
                    sqlW.AppendFormat(" and create_user='{0}'", query.create_user);
                }
                if (query.create_time != DateTime.MinValue)
                {
                    sqlW.AppendFormat(" and create_time='{0}'", Common.CommonFunction.DateTimeToString(query.create_time));
                }
                sql.AppendFormat("select pk_id, change_field,field_ch_name,old_value,new_value,vendor_id,vendor_name_full from ( SELECT pk_id,change_field,field_ch_name,old_value,new_value from table_change_log where 1=1 {0} ) tcl ", sqlW.ToString());
                sql.Append(" inner join vendor v on v.vendor_id=tcl.pk_id");

                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TableChangeLogDao-->GetVendorChangeDetail-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
