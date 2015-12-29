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
                    conndSql.AppendFormat(" and dcl_create_datetime BETWEEN '{0}' and '{1}'", Query.time_start.ToString("yyyy-MM-dd HH:mm:ss"), Query.time_end.ToString("yyyy-MM-dd HH:mm:ss"));                  
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

        public DataTable GetDeliverChangeLogDataTable(DeliverChangeLogQuery Query)
        {
            StringBuilder sbSql = new StringBuilder();            
            Query.Replace4MySQL();
            try
            {
                //sbSql.AppendFormat("select dm.order_id,dcl.deliver_id,dcl_create_type, u.user_name as dcl_create_username,mu.user_username as dcl_create_musername ,
                //                            dcl.dcl_create_datetime,'' as ori_expect_arrive_date,dcl.expect_arrive_date,dcl.expect_arrive_period,dcl.dcl_note,dcl.dcl_ipfrom
                sbSql.Append(@"select dm.order_id,dcl.deliver_id,v.vendor_id,v.vendor_name_full,v.vendor_email,dm.type,dcl_create_type, u.user_name as dcl_create_username,mu.user_username as dcl_create_musername ,");
                sbSql.Append("dcl.dcl_create_datetime,dm.expect_arrive_date as expect_arrive_date_dcl,dm.expect_arrive_period as expect_arrive_period_dcl,dcl.dcl_note,dcl.dcl_ipfrom,");
                sbSql.Append("dm.deliver_org_days,dm.expect_arrive_date as expect_arrive_date_dm,dm.expect_arrive_period as expect_arrive_period_dm,'' as delivery_date ");
                sbSql.Append(" from delivery_change_log dcl ");
                sbSql.AppendFormat(" inner join(select MAX(dcl_id) as dcl_id from delivery_change_log  ");//where dcl_create_datetime between '{0}' and '{1}', Query.time_start.ToString("yyyy-MM-dd HH:mm:ss"), Query.time_end.ToString("yyyy-MM-dd HH:mm:ss")
                sbSql.Append(" group by deliver_id order by deliver_id,dcl_create_datetime desc  ) dc on dc.dcl_id=dcl.dcl_id ");
                sbSql.Append(" inner JOIN deliver_master dm on dm.deliver_id=dcl.deliver_id ");
                sbSql.Append("inner JOIN vendor v on v.vendor_id=dm.export_id ");
                sbSql.Append(" LEFT JOIN users u on u.user_id=dcl.dcl_create_user ");
                sbSql.Append(" LEFT JOIN manage_user mu on mu.user_id=dcl.dcl_create_muser where 1=1 and dm.delivery_date is null order by dcl.dcl_create_datetime desc ");
                //sbSql.AppendFormat(" and dcl.dcl_create_datetime between '{0}' and '{1}'", Query.time_start.ToString("yyyy-MM-dd HH:mm:ss"), Query.time_end.ToString("yyyy-MM-dd HH:mm:ss"));
                //sbSql.Append(" order by deliver_id,dcl_create_datetime desc");
                System.Data.DataTable dt=_access.getDataTable(sbSql.ToString());
                return dt;
            
            }
            catch(Exception ex)
            {
                throw new Exception("DeliverChangeLog-->GetDeliverChangeLogDataTable-->" + ex.Message + sbSql.ToString(), ex);
            }
        }

        public DataTable GetDataTable(DeliverChangeLogQuery Query)
        {
            StringBuilder sbSql = new StringBuilder();
            Query.Replace4MySQL();
            try
            {
                sbSql.AppendFormat(@"select  m.deliver_id,m.order_id,m.vendor_id,m.vendor_name_full,m.vendor_email,m.dcl_note,
                                         m.deliver_org_days,m.expect_arrive_date_dm,m.expect_arrive_period_dm ,m.dcl_create_datetime ,m.type,m.order_date_pay                                  
from(
select dcl.deliver_id, dm.order_id,dm.type,v.vendor_id,v.vendor_name_full,v.vendor_email,dcl.dcl_note,om.order_date_pay, 
        dm.deliver_org_days,dm.expect_arrive_date as expect_arrive_date_dm,dm.expect_arrive_period as expect_arrive_period_dm ,dcl.dcl_create_datetime from delivery_change_log dcl
LEFT JOIN deliver_master dm on dm.deliver_id=dcl.deliver_id LEFT JOIN order_master om on om.order_id=dm.order_id
inner JOIN vendor v on v.vendor_id=dm.export_id
      where 1=1 and dm.type=2 ");
                sbSql.AppendFormat(" and dcl.dcl_create_datetime between '{0}' and '{1}'", Query.time_start.ToString("yyyy-MM-dd HH:mm:ss"), Query.time_end.ToString("yyyy-MM-dd HH:mm:ss"));
                sbSql.Append(" order by deliver_id,dcl_create_datetime desc )m GROUP BY m.deliver_id");
                System.Data.DataTable dt = _access.getDataTable(sbSql.ToString());
                return dt;

            }
            catch (Exception ex)
            {
                throw new Exception("DeliverChangeLog-->GetDataTable-->" + ex.Message + sbSql.ToString(), ex);
            }
        }

        public DataTable GetExpectArriveDateByCreatetime(DeliverChangeLogQuery Query)
        {
            StringBuilder sbSql = new StringBuilder();
            Query.Replace4MySQL();
            try
            {
                sbSql.Append(@"select expect_arrive_date from delivery_change_log where 1=1");
                sbSql.AppendFormat(" and deliver_id = '{0}'", Query.deliver_id);
                sbSql.AppendFormat(" and dcl_create_datetime <= '{0}' ",Query.time_end.ToString("yyyy-MM-dd HH:mm:ss"));
                sbSql.Append(" order by dcl_create_datetime desc");
                System.Data.DataTable dt = _access.getDataTable(sbSql.ToString());
                return dt;

            }
            catch (Exception ex)
            {
                throw new Exception("DeliverChangeLog-->GetExpectArriveDateByCreatetime-->" + ex.Message + sbSql.ToString(), ex);
            }
        }
    }
}
