/*
* 文件名稱 :DeliverChangeLogDao.cs
* 文件功能描述 :出貨管理--出貨單期望到貨日
* 版權宣告 :
* 開發人員 : zhaozhi0623j
* 版本資訊 : 1.0
* 日期 : 2015-11-12
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using MySql.Data.MySqlClient;
 
namespace BLL.gigade.Dao
{
    public class DeliverChangeLogDao : IDeliverChangeLogImplDao 
    {
        private IDBAccess _access;
        private string connStr;
        public DeliverChangeLogDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }

        public int insertDeliverChangeLog(DeliverChangeLog dCL)
        {
            StringBuilder sbSql = new StringBuilder();
            dCL.Replace4MySQL();
            try
            {           
                sbSql.AppendFormat(@"insert into delivery_change_log (deliver_id,dcl_create_user,dcl_create_datetime,dcl_create_muser,dcl_create_type,
                                 dcl_note,dcl_ipfrom,expect_arrive_date,expect_arrive_period) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", dCL.deliver_id,
                                     dCL.dcl_create_user,BLL.gigade.Common.CommonFunction.DateTimeToString( dCL.dcl_create_datetime), dCL.dcl_create_muser, dCL.dcl_create_type,
                                     dCL.dcl_note, dCL.dcl_ipfrom, dCL.expect_arrive_date.ToString("yyyy-MM-dd"), dCL.expect_arrive_period);
                return _access.execCommand(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverChangeLogDao-->insertDeliverChangeLog-->" + ex.Message + sbSql.ToString(), ex);
            }          
        }

        public List<DeliverChangeLogQuery> GetDeliverChangeLogList(DeliverChangeLogQuery Query, out int totalCount)
        {
            StringBuilder sbSql = new StringBuilder();
            StringBuilder conndSql = new StringBuilder();
            Query.Replace4MySQL();
            
            try
            {                      
                sbSql.AppendFormat(@"select deliver_id,dcl_create_datetime,dcl_create_type,dcl_note,dcl_ipfrom, expect_arrive_date,expect_arrive_period,
                                              u.user_name as dcl_create_username,mu.user_username as dcl_create_musername                                        
                                     from delivery_change_log dcl LEFT JOIN users u on u.user_id=dcl.dcl_create_user 
                                     LEFT JOIN manage_user mu on mu.user_id=dcl.dcl_create_muser where 1=1 ");
                if (Query.deliver_id != 0)
                {
                    conndSql.AppendFormat(" and deliver_id='{0}' ", Query.deliver_id);
                }              
                if (Query.dcl_create_type != 0)
                {
                    conndSql.AppendFormat(" and dcl_create_type='{0}' ", Query.dcl_create_type);
                }


                if (Query.dcl_user_name != string.Empty)
                {
                    if (Query.dcl_create_type == 0)
                    {
                        conndSql.AppendFormat(" and (u.user_name like '%{0}%' or mu.user_username like '%{0}%') ", Query.dcl_user_name);
                    }
                    if (Query.dcl_create_type == 1)
                    {
                        conndSql.AppendFormat(" and u.user_name like '%{0}%' ", Query.dcl_user_name);
                    }
                    if (Query.dcl_create_type == 2)
                    {
                        conndSql.AppendFormat(" and mu.user_username like '%{0}%' ", Query.dcl_user_name);
                    }                   
                }
                if (Query.dcl_user_email != string.Empty)
                {
                    if (Query.dcl_create_type == 0)
                    {
                        conndSql.AppendFormat(" and (u.user_email like '%{0}%' or mu.user_email like '%{0}%') ", Query.dcl_user_email);
                    }
                    if (Query.dcl_create_type == 1)
                    {
                        conndSql.AppendFormat(" and u.user_email like '%{0}%' ", Query.dcl_user_email);
                    }
                    if (Query.dcl_create_type == 2)
                    {
                        conndSql.AppendFormat(" and mu.user_email like '%{0}%' ", Query.dcl_user_email);
                    }                 
                }


                if (Query.time_start != DateTime.MinValue && Query.time_end != DateTime.MinValue)
                {
                    conndSql.AppendFormat(" and dcl_create_datetime BETWEEN '{0}' and '{1}'", Query.time_start.ToString("yyyy-MM-dd 00:00:00"), Query.time_end.ToString("yyyy-MM-dd 23:59:59"));                  
                }

                conndSql.AppendFormat(" order by dcl_create_datetime desc ");
                sbSql.AppendFormat(conndSql.ToString());

                totalCount = 0;
                if (Query.IsPage)
                {
                    DataTable _dt = new DataTable();
                    _dt = _access.getDataTable(@"select count(deliver_id) as totalCount from delivery_change_log dcl LEFT JOIN users u on u.user_id=dcl.dcl_create_user 
                                              LEFT JOIN manage_user mu on mu.user_id=dcl.dcl_create_muser where 1=1 " + conndSql.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sbSql.AppendFormat(" limit {0},{1}",Query.Start,Query.Limit);

                }
                return _access.getDataTableForObj<DeliverChangeLogQuery>(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverChangeLogDao-->GetDeliverChangeLogList-->" + ex.Message + sbSql.ToString(), ex);
            }
        }
    }
}
