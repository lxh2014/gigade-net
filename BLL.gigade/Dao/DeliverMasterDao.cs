/*
* 文件名稱 :DeliverMasterDao.cs
* 文件功能描述 :營管--出貨查詢等操作
* 版權宣告 :
* 開發人員 : chaojie1124j
* 版本資訊 : 1.0
* 日期 : 2014/10/14
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
using BLL.gigade.Common;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using MySql.Data.MySqlClient;
 /*chaojie1124j于2015/03/17 10:24添加GetTicketDetailList方法實現英冠的可出貨功能
  */
namespace BLL.gigade.Dao
{
    public class DeliverMasterDao : IDeliverMasterImplDao 
    {
        private IDBAccess _access;
        private string connStr;
        public DeliverMasterDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }
        #region 出貨查詢+List<DeliverMaster> GetdeliverList(DeliverMaster deliver, out int totalCount)
        /// <summary>
        /// 出貨管理>出貨查詢:列表頁面
        /// </summary>
        /// <param name="deliver">實體</param>
        /// <param name="totalCount">分頁</param>
        /// <returns>返回集合</returns>

        public List<DeliverMasterQuery> GetdeliverList(DeliverMasterQuery deliver, out int totalCount)
        {
            StringBuilder sqlClomn = new StringBuilder();
            StringBuilder sqlTable = new StringBuilder();
            StringBuilder sqlCondition = new StringBuilder();
            StringBuilder sqlSort = new StringBuilder();
            string addsql = getSqlForDliverList(deliver);
            try
            {
                totalCount = 0;
                sqlClomn.Append(@"select dm.type,dm.deliver_id,dm.delivery_status,dm.freight_set,dm.ticket_id,dm.export_id,dm.delivery_store ");
                sqlClomn.Append(@",dm.delivery_code,dm.delivery_freight_cost,dm.delivery_date,dm.arrival_date,dm.estimated_delivery_date ");
                sqlClomn.Append(@",dm.estimated_arrival_date,dm.estimated_arrival_period,dm.delivery_name,dm.order_id ,v.vendor_name_simple");
                sqlClomn.Append(@",t.warehouse_status,FROM_UNIXTIME(om.order_createdate) as order_createtime,om.order_status ");
                sqlClomn.Append(@", (select ld.logisticsType from logistics_detail ld where ld.deliver_id=dm.deliver_id order by rid desc limit 1)as logisticsType ");
                sqlTable.Append(@" from  deliver_master dm ");
                sqlTable.Append(@" left join vendor v on v.vendor_id=dm.export_id left join order_master om on om.order_id=dm.order_id left join ticket t on dm.ticket_id=t.ticket_id  ");
                sqlCondition.Append(@" where 1=1 ");
                if (!string.IsNullOrEmpty(addsql))//查詢條件
                {
                    sqlCondition.Append(addsql);
                }
                else
                {
                    sqlCondition.Append(" and dm.type=1");
                }
                if (deliver.IsPage)
                {
                    string sql = "select count(dm.deliver_id) as total_count " + sqlTable.ToString() + sqlCondition.ToString();
                    System.Data.DataTable _dt = _access.getDataTable(sql);
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = int.Parse(_dt.Rows[0]["total_count"].ToString());
                    }
                    sqlSort.Append(@" order by dm.deliver_id desc ");
                    sqlSort.AppendFormat(" limit {0},{1} ;", deliver.Start, deliver.Limit);
                }
                else
                {
                    sqlSort.Append(@" order by dm.deliver_id desc ");
                }
                string sqlstr = sqlClomn.ToString() + sqlTable.ToString() + sqlCondition.ToString() + sqlSort.ToString();
                return _access.getDataTableForObj<DeliverMasterQuery>(sqlstr);
            }
            catch (Exception ex)
            {
                throw new Exception(" DeliverMasterDao-->GetdeliverList-->" + ex.Message + sqlClomn.ToString() + sqlTable.ToString() + sqlCondition.ToString() + sqlSort.ToString(), ex);
            }
        }

        public List<DeliverMasterQuery> GetdeliverListCSV(DeliverMasterQuery deliver)
        {
            StringBuilder sqlClomn = new StringBuilder();
            StringBuilder sqlTable = new StringBuilder();
            StringBuilder sqlCondition = new StringBuilder();
            StringBuilder sqlSort = new StringBuilder();
            string addsql = getSqlForDliverList(deliver);
            try
            {
                sqlClomn.Append(@"select dm.type,dm.deliver_id,dm.delivery_status,dm.freight_set,dm.ticket_id,dm.export_id,dm.delivery_store ");
                sqlClomn.Append(@",dm.delivery_code,dm.delivery_freight_cost,dm.delivery_date,dm.arrival_date,dm.estimated_delivery_date ");
                sqlClomn.Append(@",dm.estimated_arrival_date,dm.estimated_arrival_period,dm.delivery_name,dm.order_id ,v.vendor_name_simple");
                sqlClomn.Append(@",FROM_UNIXTIME(om.order_createdate) as order_createtime,om.order_status ");
                sqlClomn.Append(@", FROM_UNIXTIME(om.money_collect_date) as money_pay_date, FROM_UNIXTIME(om.order_date_pay)as order_pay_date ");
                sqlTable.Append(@" from  deliver_master dm,order_master om,vendor v ");
                sqlCondition.Append(@" where 1=1 and	dm.order_id = om.order_id 	and	v.vendor_id= dm.export_id ");
                if (deliver.warehouse_status != -1) //調度狀態
                {
                    sqlTable.Append(@" ,ticket t ");
                    sqlCondition.Append(@" and  dm.ticket_id = t.ticket_id  ");
                }
                else
                {
                    sqlClomn.Append(@",(SELECT	warehouse_status FROM	ticket WHERE	1=1 	AND	ticket_id =dm.ticket_id) as warehouse_status ");
                }
                if (!string.IsNullOrEmpty(addsql))//查詢條件
                {
                    sqlCondition.Append(addsql);
                }
                else
                {
                    sqlCondition.Append(" and dm.type=1");
                }
                //if (deliver.IsPage)
                //{
                //    string sql = "select count(dm.deliver_id) as total_count " + sqlTable.ToString() + sqlCondition.ToString();
                //    System.Data.DataTable _dt = _access.getDataTable(sql);
                //    if (_dt != null && _dt.Rows.Count > 0)
                //    {
                //        totalCount = int.Parse(_dt.Rows[0]["total_count"].ToString());
                //    }
                //sqlSort.Append(@" order by dm.deliver_id desc ");
                //    sqlSort.AppendFormat(" limit {0},{1} ;", deliver.Start, deliver.Limit);
                //}
                //else
                //{
                sqlSort.Append(@" order by dm.deliver_id desc ");
                //}
                string sql = sqlClomn.ToString() + sqlTable.ToString() + sqlCondition.ToString() + sqlSort.ToString();
                return _access.getDataTableForObj<DeliverMasterQuery>(sql);
            }
            catch (Exception ex)
            {
                throw new Exception(" DeliverMasterDao-->GetdeliverList-->" + ex.Message + sqlClomn.ToString() + sqlTable.ToString() + sqlCondition.ToString() + sqlSort.ToString(), ex);
            }


        }

        /// <summary>
        /// 把查詢條件在這裡判斷並且返回查詢語句
        /// </summary>
        /// <param name="deliver"></param>
        /// <returns></returns>
        public string getSqlForDliverList(DeliverMasterQuery deliver)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 查詢條件
                if (!string.IsNullOrEmpty(deliver.types))//出貨類別
                {
                    sb.AppendFormat(" and dm.type='{0}' ", deliver.types);
                }
                if (!string.IsNullOrEmpty(deliver.status))//出貨狀態
                {
                    sb.AppendFormat(" and dm.delivery_status='{0}' ", deliver.status);
                }
                if (deliver.vendor_id != 0) //出貨方式
                {
                    sb.AppendFormat(" and v.vendor_id='{0}' ", deliver.vendor_id);
                }
                if (deliver.delivery_store != 0)//物流商
                {
                    sb.AppendFormat(" and dm.delivery_store='{0}' ", deliver.delivery_store);
                }
                if (deliver.warehouse_status != -1) //調度狀態
                {
                    sb.AppendFormat(" and t.warehouse_status='{0}' ", deliver.warehouse_status);
                }
                if (deliver.priority != -1)//出貨篩選
                {
                    sb.AppendFormat(" and om.priority='{0}' ", deliver.priority);
                }

                if (deliver.time_start != DateTime.MinValue)//出貨日期
                {
                    sb.AppendFormat(" and dm.delivery_date > '{0}' ", deliver.time_start.ToString("yyyy-MM-dd 23:59:59"));
                }
                if (deliver.time_end != DateTime.MinValue)//出貨日期
                {
                    sb.AppendFormat(" and dm.delivery_date < '{0}' ", deliver.time_end.ToString("yyyy-MM-dd 23:59:59"));
                }

                if (!string.IsNullOrEmpty(deliver.vendor_name_simple))//搜索
                {
                    sb.AppendFormat("  and  (dm.deliver_id='{0}' or dm.delivery_code='{1}' ", deliver.vendor_name_simple, deliver.vendor_name_simple, deliver.vendor_name_simple);
                    sb.AppendFormat("  or dm.order_id='{0}' or dm.delivery_name like'%{1}%' ", deliver.vendor_name_simple, deliver.vendor_name_simple);
                    sb.AppendFormat("  or dm.delivery_mobile='{0}' or v.vendor_name_simple like '%{0}%') ", deliver.vendor_name_simple, deliver.vendor_name_simple);
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(" DeliverMasterDao-->getSqlForDliverList-->" + ex.Message + sb.ToString(), ex);
            }

            return sb.ToString();
        }
        #endregion

        public List<DeliverMasterQuery> DeliverVerifyList(DeliverMaster dm, out int totalCount)
        {
            StringBuilder sbSql = new StringBuilder();
            try
            {
                totalCount = 0;
                //sbSql.Append(@"SELECT dm.deliver_id,dm.order_id,pi.item_id,od.product_name,od.buy_num,tp.remark AS 'status',CONCAT('(',tp2.parameterName,tp1.parameterName,')') as delivery_store_name  from deliver_master dm ");
                //sbSql.Append(@"LEFT JOIN deliver_detail dd ON dm.deliver_id = dd.deliver_id ");
                //sbSql.Append(@"LEFT JOIN order_detail od ON dd.detail_id = od.detail_id ");
                //sbSql.Append(@"LEFT JOIN product_item pi ON od.item_id = pi.item_id ");
                //sbSql.Append(@"LEFT JOIN product p ON pi.product_id = p.product_id ");
                //sbSql.Append(@"LEFT JOIN t_parametersrc tp ON od.detail_status = tp.parameterCode AND tp.parameterType='order_status' ");
                //sbSql.Append(@"LEFT JOIN t_parametersrc tp1 ON dm.delivery_store = tp1.parameterCode AND tp1.parameterType='Deliver_Store' ");
                //sbSql.Append(@"LEFT JOIN t_parametersrc tp2 ON od.product_freight_set = tp2.parameterCode AND tp2.parameterType='product_freight' ");
                //od.detail_status,dd.detail_id, od.parent_name,  od.product_spec_name,od.product_freight_set,od.item_vendor_id,od.product_mode,od.combined_mode,od.parent_id,od.item_mode,od.pack_id,od.parent_num,p.product_mode,
                // 轉型錯誤  dd.delivery_status,
                sbSql.Append(@"select  pi.item_id, od.product_name, od.buy_num,dd.deliver_id, 
dm.order_id,tp.remark AS 'status',CONCAT('(',tp2.parameterName,tp1.parameterName,')') as delivery_store_name 
from deliver_detail dd
left join order_detail od on dd.detail_id=od.detail_id
left join product_item  pi on od.item_id=pi.item_id
left join product p on p.product_id=pi.product_id
left join deliver_master dm ON dm.deliver_id = dd.deliver_id 
LEFT JOIN t_parametersrc tp ON od.detail_status = tp.parameterCode AND tp.parameterType='order_status' 
LEFT JOIN t_parametersrc tp1 ON dm.delivery_store = tp1.parameterCode AND tp1.parameterType='Deliver_Store' 
LEFT JOIN t_parametersrc tp2 ON od.product_freight_set = tp2.parameterCode AND tp2.parameterType='product_freight'
");
                sbSql.Append(@" where 1=1 ");
                if (dm.deliver_id != 0 && dm.order_id != 0)
                {
                    //sbSql.AppendFormat(@" AND dm.deliver_id='{0}' AND dm.order_id='{1}' ", dm.deliver_id, dm.order_id); 
                    sbSql.AppendFormat(@" AND dd.deliver_id='{0}' ", dm.deliver_id);
                }
                sbSql.Append(@" order by od.item_vendor_id asc ,od.item_id asc ");
                //if (dm.IsPage)
                //{
                //    System.Data.DataTable _dt = _access.getDataTable(sbSql.ToString());
                //    if ( _dt.Rows.Count > 0)
                //    {
                //        totalCount = _dt.Rows.Count;
                //    }
                //    sbSql.AppendFormat(" limit {0},{1}", dm.Start, dm.Limit);
                //}
                return _access.getDataTableForObj<DeliverMasterQuery>(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" DeliverMasterDao-->DeliverVerifyList-->" + ex.Message + sbSql.ToString(), ex);
            }
        }

        public List<DeliverMasterQuery> JudgeOrdid(DeliverMaster dm)
        {
            StringBuilder sbSql = new StringBuilder();
            try
            {
                sbSql.Append(@"SELECT dm.deliver_id,dm.order_id,dm.delivery_status,CONCAT('(',tp2.parameterName,tp1.parameterName,')') as delivery_store_name,dm.delivery_store from deliver_master dm ");
                sbSql.Append(@"LEFT JOIN deliver_detail dd ON dm.deliver_id = dd.deliver_id  ");
                sbSql.Append(@"LEFT JOIN order_detail od ON dd.detail_id=od.detail_id ");
                sbSql.Append(@"LEFT JOIN t_parametersrc tp1 ON dm.delivery_store = tp1.parameterCode AND tp1.parameterType='Deliver_Store' ");
                sbSql.Append(@"LEFT JOIN t_parametersrc tp2 ON od.product_freight_set = tp2.parameterCode AND tp2.parameterType='product_freight' ");
                sbSql.Append(@" where 1=1");


                if (dm.deliver_id != 0)
                {
                    sbSql.AppendFormat(@" AND dm.deliver_id='{0}' ", dm.deliver_id);
                }
                if (dm.order_id != 0)
                {
                    sbSql.AppendFormat(@" AND dm.order_id='{0}' ", dm.order_id);
                }
                sbSql.Append(" limit 1;");
                return _access.getDataTableForObj<DeliverMasterQuery>(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" DeliverMasterDao-->JudgeOrdid-->" + ex.Message + sbSql.ToString(), ex);
            }

        }

        public void Add(DeliverMaster dm)
        {
            StringBuilder sbSql = new StringBuilder();
            try
            {
                sbSql.AppendFormat(@"insert into deliver_master(deliver_id,detail_id,delivery_status) values()");
            }
            catch (Exception ex)
            {
                throw new Exception(" DeliverMasterDao-->Add-->" + ex.Message + sbSql.ToString(), ex);
            }
        }



        public DataTable GetMessageByDeliveryCode(DeliverMasterQuery dmQuery)
        {
            StringBuilder sbSql = new StringBuilder();
            try
            {
                sbSql.AppendFormat(@"SELECT dm.deliver_id, dm.delivery_status, dm.freight_set, dm.ticket_id, dm.export_id,
			            dm.delivery_store, dm.delivery_code, dm.delivery_date,tp.parameterName
			            , dm.estimated_arrival_period,
			            dm.delivery_name, dm.order_id, vd.vendor_name_simple,om.order_amount,om.order_status FROM deliver_master dm 
                        LEFT JOIN order_master om on dm.order_id=om.order_id 
                        LEFT JOIN vendor vd on dm.export_id=vd.vendor_id 
                        LEFT JOIN (SELECT parameterName,parameterCode FROM t_parametersrc WHERE parameterType='Deliver_Store' ) as tp on tp.parameterCode=dm.delivery_store 
                        WHERE dm.delivery_code='{0}'", dmQuery.delivery_code);

                return _access.getDataTable(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" DeliverMasterDao-->GetMessageByDeliveryCode-->" + ex.Message + sbSql.ToString(), ex);
            }
        }


        public int Updatedeliveryfreightcost(StringBuilder str)
        {
            int result = 0;
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                mySqlCmd.CommandText = str.ToString();
                result = mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("DeliverMasterDao-->Updatedeliveryfreightcost-->" + ex.Message + str.ToString(), ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
            return result;
        }

        public DataTable GetReportManagementList(DeliverMasterQuery deliver, out int totalCount)
        {
            DataTable dt = new DataTable();
            StringBuilder str = new StringBuilder();
            StringBuilder strCount = new StringBuilder();
            StringBuilder strcondition = new StringBuilder();
            try
            {
                strcondition.AppendFormat(" and om.order_createdate >='{0}' and om.order_createdate <='{1}' ", Common.CommonFunction.GetPHPTime(deliver.order_time_begin.ToString()), Common.CommonFunction.GetPHPTime(deliver.order_time_end.ToString()));
                if (!string.IsNullOrEmpty(deliver.sorder_id))
                {
                    strcondition.AppendFormat(" and om.order_id like '%{0}%' ", deliver.sorder_id);
                }
                if (!string.IsNullOrEmpty(deliver.sdeliver_id))
                {
                    strcondition.AppendFormat(" and dm.deliver_id like '%{0}%' ", deliver.sdeliver_id);
                }
                if (deliver.deliver_store != 0)
                {
                    strcondition.AppendFormat(" and dm.delivery_store='{0}' ", deliver.deliver_store);
                }
                if (deliver.i_order_status != -1)
                {
                    strcondition.AppendFormat(" and om.order_status='{0}' ", deliver.i_order_status);
                }
                if (deliver.payment != 0)
                {
                    strcondition.AppendFormat(" and om.order_payment='{0}' ", deliver.payment);
                }
                if (deliver.logisticsType != 0)
                {
                    strcondition.AppendFormat(" and ld.logisticsTypes='{0}' ", deliver.logisticsType);
                }
                if (deliver.ideliver_status!= -1)
                {
                    strcondition.AppendFormat(" and dm.delivery_status='{0}' ", deliver.ideliver_status );
                }
                str.AppendFormat(@"SELECT dm.delivery_code,dm.deliver_id,om.order_id,DATE(FROM_UNIXTIME(om.order_createdate)) as order_date,om.order_status,om.order_payment,dm.delivery_store,ld.logisticsTypes,dm.delivery_status 
                FROM deliver_master dm  
                LEFT JOIN order_master om on om.order_id=dm.order_id                                
                LEFT JOIN (SELECT max(logisticsType) as logisticsTypes, deliver_id fROM logistics_detail  GROUP BY deliver_id ) as ld 
on ld.deliver_id=dm.deliver_id  where 1=1  ");
                strCount.AppendFormat(@"SELECT count(dm.deliver_id) as 'count'  FROM deliver_master dm                                
                LEFT JOIN order_master om on om.order_id=dm.order_id 
                LEFT JOIN (SELECT max(logisticsType) as logisticsTypes, deliver_id fROM logistics_detail  GROUP BY deliver_id ) as ld on ld.deliver_id=dm.deliver_id
                where 1=1  ");
                str.AppendFormat(strcondition.ToString());
                strCount.Append(strcondition);
                totalCount = 0;
                if (deliver.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(strCount.ToString());

                    int.TryParse(_dt.Rows[0]["count"].ToString(), out totalCount);

                    str.AppendFormat(" limit {0},{1}", deliver.Start, deliver.Limit);
                }
                dt = _access.getDataTable(str.ToString());

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterDao.GetReportManagementList-->" + ex.Message + str.ToString() + strcondition.ToString(), ex);
            }
        }
        public DataTable ReportManagementExcelList(DeliverMasterQuery deliver)
        {
            StringBuilder strcondition = new StringBuilder();
            StringBuilder str = new StringBuilder();
            DataTable dt = new DataTable();
            DataTable dt_type = new DataTable();
            try
            {
                strcondition.AppendFormat(" and om.order_createdate >='{0}' and om.order_createdate <='{1}' ", Common.CommonFunction.GetPHPTime(deliver.order_time_begin.ToString()), Common.CommonFunction.GetPHPTime(deliver.order_time_end.ToString()));
                if (!string.IsNullOrEmpty(deliver.sorder_id))
                {
                    strcondition.AppendFormat(" and om.order_id like '%{0}%' ", deliver.sorder_id);
                }
                if (!string.IsNullOrEmpty(deliver.sdeliver_id))
                {
                    strcondition.AppendFormat(" and dm.deliver_id like '%{0}%' ", deliver.sdeliver_id);
                }
                if (deliver.deliver_store != 0)
                {
                    strcondition.AppendFormat(" and dm.delivery_store='{0}' ", deliver.deliver_store);
                }
                if (deliver.i_order_status != -1)
                {
                    strcondition.AppendFormat(" and om.order_status='{0}' ", deliver.i_order_status);
                }
                if (deliver.payment != 0)
                {
                    strcondition.AppendFormat(" and om.order_payment='{0}' ", deliver.payment);
                }
                if (deliver.logisticsType != 0)
                {
                    strcondition.AppendFormat(" and ld.logisticsTypes='{0}' ", deliver.logisticsType);
                }
                if (deliver.ideliver_status != -1)
                {
                    strcondition.AppendFormat(" and dm.delivery_status='{0}' ", deliver.ideliver_status);
                }
                str.AppendFormat(@"SELECT dm.delivery_code,dm.deliver_id,om.order_id,DATE(FROM_UNIXTIME(om.order_createdate)) as order_date,om.order_status,om.order_payment,dm.delivery_store,ld.logisticsTypes,dm.delivery_status 
                FROM deliver_master dm  
                LEFT JOIN order_master om on om.order_id=dm.order_id                                
                LEFT JOIN (SELECT max(logisticsType) as logisticsTypes, deliver_id fROM logistics_detail  GROUP BY deliver_id ) as ld 
on ld.deliver_id=dm.deliver_id  where 1=1 ");
                str.AppendFormat(strcondition.ToString());
                dt = _access.getDataTable(str.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterDao.ReportManagementExcelList-->" + ex.Message + str.ToString() + strcondition.ToString(), ex);
            }
        }
//        public DataTable GetReportManagementList(DeliverMasterQuery deliver, out int totalCount)
//        {
//            DataTable dt = new DataTable();
//            StringBuilder str = new StringBuilder();
//            StringBuilder strCount = new StringBuilder();
//            StringBuilder strcondition = new StringBuilder();
//            StringBuilder sql = new StringBuilder();
//            try
//            {
//                strcondition.AppendFormat(" and DATE(FROM_UNIXTIME(om.order_createdate)) >='{0}' and DATE(FROM_UNIXTIME(om.order_createdate)) <='{1}' ", Common.CommonFunction.DateTimeToString(deliver.order_time_begin), Common.CommonFunction.DateTimeToString(deliver.order_time_end));
//                if (!string.IsNullOrEmpty(deliver.sorder_id))
//                {
//                    strcondition.AppendFormat(" and om.order_id like '%{0}%' ", deliver.sorder_id);
//                }
//                if (!string.IsNullOrEmpty(deliver.sdeliver_id))
//                {
//                    strcondition.AppendFormat(" and dm.deliver_id like '%{0}%' ", deliver.sdeliver_id);
//                }
//                if (deliver.deliver_store != 0)
//                {
//                    strcondition.AppendFormat(" and dm.delivery_store='{0}' ", deliver.deliver_store);
//                }
//                if (deliver.i_order_status != -1)
//                {
//                    strcondition.AppendFormat(" and om.order_status='{0}' ", deliver.i_order_status);
//                }
//                if (deliver.payment != 0)
//                {
//                    strcondition.AppendFormat(" and om.order_payment='{0}' ", deliver.payment);
//                }

//                str.AppendFormat(@"SELECT * FROM (SELECT dm.delivery_code,dm.deliver_id,om.order_id,tp_payment.parameterName as order_payment,tp_deliver.parameterName as delivery_store,tp_order.remark as order_status,tp_logistics.parameterName as logisticsType,DATE(FROM_UNIXTIME(om.order_createdate)) as order_date, log_tail.logisticsType as type
//FROM logistics_detail log_tail right JOIN deliver_master dm  USING(deliver_id)
//LEFT JOIN order_master om on om.order_id=dm.order_id   
//LEFT JOIN (SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='order_status') as tp_order on tp_order.parameterCode=om.order_status
//LEFT JOIN (SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='payment') as tp_payment on tp_payment.parameterCode=om.order_payment
//LEFT JOIN (SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='Deliver_Store') as tp_deliver on tp_deliver.parameterCode=dm.delivery_store
//LEFT JOIN (SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='logistics_type') as tp_logistics on tp_logistics.parameterCode=log_tail.logisticsType
//                where 1=1  ");
//                strCount.AppendFormat(@"SELECT count(*) as 'count' FROM (SELECT  dm.deliver_id,log_tail.logisticsType as type  FROM logistics_detail log_tail right JOIN deliver_master dm  USING(deliver_id) LEFT JOIN order_master om on om.order_id=dm.order_id   
//                where 1=1  ");
//                strcondition.Append("GROUP BY dm.deliver_id ORDER BY log_tail.deliver_id ASC,log_tail.set_time DESC)  a");
//                str.AppendFormat(strcondition.ToString());
//                strCount.Append(strcondition);
//                totalCount = 0;
//                if (deliver.logisticsType != 0)
//                {
//                    strCount.AppendFormat(" where type={0} ", deliver.logisticsType);
//                    str.AppendFormat(" where type={0} ", deliver.logisticsType);
//                }
//                if (deliver.IsPage)
//                {
//                    System.Data.DataTable _dt = _access.getDataTable(strCount.ToString());

//                    int.TryParse(_dt.Rows[0]["count"].ToString(), out totalCount);

//                    str.AppendFormat(" limit {0},{1}", deliver.Start, deliver.Limit);
//                }
//                dt = _access.getDataTable(str.ToString());

//                return dt;
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("DeliverMasterDao.GetReportManagementList-->" + ex.Message + str.ToString() + strcondition.ToString(), ex);
//            }
//        }

//        public DataTable ReportManagementExcelList(DeliverMasterQuery deliver)
//        {
//            StringBuilder strcondition = new StringBuilder();
//            StringBuilder str = new StringBuilder();
//            DataTable dt = new DataTable();
//            DataTable dt_type = new DataTable();
//            try
//            {
//                strcondition.AppendFormat(" and DATE(FROM_UNIXTIME(om.order_createdate)) >='{0}' and DATE(FROM_UNIXTIME(om.order_createdate)) <='{1}' ", Common.CommonFunction.DateTimeToString(deliver.order_time_begin), Common.CommonFunction.DateTimeToString(deliver.order_time_end));
//                if (!string.IsNullOrEmpty(deliver.sorder_id))
//                {
//                    strcondition.AppendFormat(" and om.order_id like '%{0}%' ", deliver.sorder_id);
//                }
//                if (!string.IsNullOrEmpty(deliver.sdeliver_id))
//                {
//                    strcondition.AppendFormat(" and dm.deliver_id like '%{0}%' ", deliver.sdeliver_id);
//                }
//                if (deliver.deliver_store != 0)
//                {
//                    strcondition.AppendFormat(" and dm.delivery_store='{0}' ", deliver.deliver_store);
//                }
//                if (deliver.i_order_status != -1)
//                {
//                    strcondition.AppendFormat(" and om.order_status='{0}' ", deliver.i_order_status);
//                }
//                if (deliver.payment != 0)
//                {
//                    strcondition.AppendFormat(" and om.order_payment='{0}' ", deliver.payment);
//                }
//                str.AppendFormat(@"SELECT * FROM (SELECT dm.delivery_code,dm.deliver_id,om.order_id,tp_payment.parameterName as order_payment,tp_deliver.parameterName as delivery_store,tp_order.remark as order_status,tp_logistics.parameterName as logisticsType,DATE(FROM_UNIXTIME(om.order_createdate)) as order_date 
//FROM logistics_detail log_tail right JOIN deliver_master dm  USING(deliver_id)
//LEFT JOIN order_master om on om.order_id=dm.order_id   
//LEFT JOIN (SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='order_status') as tp_order on tp_order.parameterCode=om.order_status
//LEFT JOIN (SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='payment') as tp_payment on tp_payment.parameterCode=om.order_payment
//LEFT JOIN (SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='Deliver_Store') as tp_deliver on tp_deliver.parameterCode=dm.delivery_store
//LEFT JOIN (SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='logistics_type') as tp_logistics on tp_logistics.parameterCode=log_tail.logisticsType
//                where 1=1  ");
//                strcondition.Append("GROUP BY dm.deliver_id ORDER BY log_tail.deliver_id ASC,log_tail.set_time DESC ) a ");
//                if (deliver.logisticsType != 0)
//                {
//                    strcondition.AppendFormat(" where type={0} ", deliver.logisticsType);
//                }
//                str.AppendFormat(strcondition.ToString());
//                dt=_access.getDataTable(str.ToString());                              
//                return dt;
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("DeliverMasterDao.ReportManagementExcelList-->" + ex.Message + str.ToString() + strcondition.ToString(), ex);
//            }
//        }

        public DataTable Getlogistics(string type)
        {
            StringBuilder str = new StringBuilder();
            try
            {
                str.Append("SELECT deliver_id,logisticsType,tp.parameterName  FROM (sELECT max(logisticsType) as logisticsType, deliver_id fROM logistics_detail  GROUP BY deliver_id ) as ld LEFT JOIN (SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='logistics_type') as tp on tp.parameterCode=ld.logisticsType where 1=1 ");
                if (type != "0")
                {
                    str.AppendFormat(" AND ld.logisticsType='{0}' ", type);
                }
                return _access.getDataTable(str.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterDao-->Getlogistics-->" + ex.Message, ex);
            }
        }

        /// <summary>
        ///出貨管理->可出貨列表頁
        /// </summary>
        /// <param name="query">Model</param>
        /// <param name="totalCount">分頁</param>
        /// <returns></returns>
        public List<DeliverMasterQuery> GetTicketDetailList(DeliverMasterQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sql.Append(@" select dm.deliver_id,dm.order_id,dm.delivery_name,dm.estimated_delivery_date,dm.estimated_arrival_date,dm.estimated_arrival_period ,om.note_order ");
                sqlCondi.Append("  from deliver_master dm ");
                sqlCondi.Append(" left join order_master om on dm.order_id=om.order_id  ");
                sqlCondi.Append(" where 1=1 ");
                if (query.type != 0)
                {
                    sqlCondi.AppendFormat(" and  dm.type='{0}' ", query.type);
                }
                if (query.export_id != 0)
                {
                    sqlCondi.AppendFormat(" and dm.export_id='{0}' ", query.export_id);
                }
                if (query.freight_set != 0)
                {
                    sqlCondi.AppendFormat(" and dm.freight_set='{0}' ", query.freight_set);
                }
                if (query.delivery_status != 0)
                {
                    sqlCondi.AppendFormat(" and dm.delivery_status='{0}' ", query.delivery_status);
                }
                totalCount = 0;
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable("select count(dm.deliver_id) as totalCount " + sqlCondi.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = int.Parse(_dt.Rows[0]["totalCount"].ToString());
                    }

                    sqlCondi.AppendFormat(" limit {0},{1} ", query.Start, query.Limit);
                }

                sql.Append(sqlCondi.ToString());

                return _access.getDataTableForObj<DeliverMasterQuery>(sql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("DeliverMasterDao.GetTicketDetailList" + ex.Message + sql.ToString(), ex);
            }
        }
        /// <summary>
        /// 出貨管理->延遲出貨列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetDelayDeliverList(DeliverMasterQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sql.Append("select DATE(FROM_UNIXTIME(om.order_date_pay)) AS date_pay,count(deliver_id) AS count   ");
                sqlCondi.Append(" from deliver_master dm ");
                sqlCondi.Append(" left join order_master om on dm.order_id=om.order_id ");
                sqlCondi.Append(" where om.order_date_pay > 0 and dm.type in (1,2) and dm.delivery_status in(0, 1, 2, 5) group by date_pay order by date_pay asc ");//Php頁面admin/view/delivers/delay_delivers.ctp第151和152行，寫的固定查詢數據
              
                totalCount = 0;
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable(sql.ToString() + sqlCondi.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }
                    sqlCondi.AppendFormat(" limit {0},{1} ", query.Start, query.Limit);
                }
                sql.Append(sqlCondi.ToString());
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterDao-->GetDelayDeliverList-->" + ex.Message, ex);
            }
        }


        public DataTable GetDeliveryMsgList(DeliverMasterQuery deliver, out int totalCount)
        {
            DataTable dt = new DataTable();
            StringBuilder str = new StringBuilder();
            StringBuilder strCount = new StringBuilder();
            StringBuilder strcondition = new StringBuilder();
            try
            {
                if (deliver.time_type == 2)
                {
                    strcondition.AppendFormat(" and om.order_createdate >='{0}' and om.order_createdate <='{1}' ", Common.CommonFunction.GetPHPTime(deliver.order_time_begin.ToString()), Common.CommonFunction.GetPHPTime(deliver.order_time_end.ToString()));
                }
                if (!string.IsNullOrEmpty(deliver.sorder_id))
                {
                    strcondition.AppendFormat(" and om.order_id = '{0}' ", deliver.sorder_id);
                }
                if (!string.IsNullOrEmpty(deliver.sdeliver_id))
                {
                    strcondition.AppendFormat(" and dm.deliver_id = '{0}' ", deliver.sdeliver_id);
                }
                if (deliver.deliver_store != 0)
                {
                    strcondition.AppendFormat(" and dm.delivery_store='{0}' ", deliver.deliver_store);
                }
                if (deliver.i_order_status != -1)
                {
                    strcondition.AppendFormat(" and om.order_status='{0}' ", deliver.i_order_status);
                }
                if (deliver.order_day != 0)
                {
                    strcondition.AppendFormat(" and dm.deliver_org_days<='{0}' ", Common.CommonFunction.GetPHPTime(DateTime.Now.AddDays(deliver.order_day).ToString("yyyy-MM-dd 23:59:59")));//預計到貨日期且未到貨
                }
                if (deliver.i_slave_status != -1)
                {
                    strcondition.AppendFormat(" and os.slave_status='{0}' ", deliver.i_slave_status);
                }
                if (deliver.i_detail_status != -1)
                {
                    strcondition.AppendFormat(" and odt.detail_status='{0}' ", deliver.i_detail_status);
                }
                if (deliver.payment != 0)
                {
                    strcondition.AppendFormat(" and om.order_payment='{0}' ", deliver.payment);
                }
                if (deliver.logisticsType != 0)
                {
                    strcondition.AppendFormat(" and ld.logisticsTypes='{0}' ", deliver.logisticsType);
                }
                if (deliver.ideliver_status != -1)
                {
                    strcondition.AppendFormat(" and dm.delivery_status='{0}' ", deliver.ideliver_status);
                }
                if (deliver.product_mode != 0)
                {
                    strcondition.AppendFormat(" and pt.product_mode ='{0}' ",deliver.product_mode);
                }
                if (!string.IsNullOrEmpty(deliver.serch_where))
                {
                    strcondition.AppendFormat(" and vd.vendor_id in ({0})",deliver.serch_where);
                }
                strcondition.AppendFormat(" )as mytable ");
                if (!string.IsNullOrEmpty(deliver.t_days.ToString()) && deliver.t_days != -1)
                {
                    strcondition.AppendFormat(" where overdue_day>='{0}' ", deliver.t_days);
                }
                str.AppendFormat(@"SELECT delivery_code,deliver_id,created,delivery_date,order_id,order_date,order_status,order_payment,delivery_store,
logisticsTypes,delivery_status,dvendor_name_simple,vendor_name_simple,freight_set,delivery_freight_cost,delivery_date_str,
overdue_day,
arrival_date,estimated_delivery_date 
,estimated_arrival_date,estimated_arrival_period,delivery_name,product_name,note_order,note_admin,
buy_num,item_id,product_mode,deliver_master_date,slave_status,detail_status,product_id,detail_id 
FROM (
SELECT dm.delivery_code, case dm.deliver_org_days when 0 then '-' else FROM_UNIXTIME(dm.deliver_org_days) end as delivery_date_str,dm.deliver_id,dm.created,om.order_id,DATE(FROM_UNIXTIME(om.order_createdate)) as order_date,dd.detail_id,pt.product_id,om.order_status,om.order_payment,dm.delivery_store,
ld.logisticsTypes,dm.delivery_status,vds.vendor_name_simple as dvendor_name_simple,vd.vendor_name_simple,dm.freight_set,dm.delivery_freight_cost,
case ISNULL(dm.delivery_date) when TRUE then DATEDIFF(NOW(),DATE(FROM_UNIXTIME(om.order_date_pay)))  ELSE datediff(dm.delivery_date,DATE(FROM_UNIXTIME(om.order_date_pay))) end as overdue_day,
dm.arrival_date,dm.estimated_delivery_date 
,dm.estimated_arrival_date,dm.estimated_arrival_period,dm.delivery_name,odt.product_name,om.note_order,om.note_admin,
case odt.item_mode WHEN 2 THEN odt.parent_num * odt.buy_num ELSE odt.buy_num END as buy_num,pii.item_id,pt.product_mode,DATE(FROM_UNIXTIME(om.order_date_pay)) as deliver_master_date,dm.delivery_date,os.slave_status,odt.detail_status
FROM deliver_master dm  
INNER JOIN deliver_detail dd on dd.deliver_id = dm.deliver_id
LEFT JOIN (SELECT max(logisticsType) as logisticsTypes, deliver_id fROM logistics_detail  GROUP BY deliver_id ) as ld 
on ld.deliver_id=dm.deliver_id  
INNER JOIN order_detail odt on odt.detail_id=dd.detail_id 
INNER JOIN order_slave os on os.slave_id=odt.slave_id 
INNER JOIN order_master om on os.order_id=om.order_id               
INNER JOIN product_item pii on pii.item_id =odt.item_id   
INNER JOIN vendor vds on vds.vendor_id=dm.export_id  
INNER JOIN vendor vd on vd.vendor_id=odt.item_vendor_id  
INNER JOIN product pt on pii.product_id=pt.product_id where odt.item_mode !=1  ");
                strCount.AppendFormat(@"SELECT count(deliver_id) as count
FROM (
SELECT dm.deliver_id,
 case ISNULL(dm.delivery_date) when TRUE then DATEDIFF(NOW(),DATE(FROM_UNIXTIME(om.order_date_pay)))  ELSE datediff(dm.delivery_date,DATE(FROM_UNIXTIME(om.order_date_pay))) end as overdue_day
FROM deliver_master dm  
INNER JOIN deliver_detail dd on dd.deliver_id = dm.deliver_id
LEFT JOIN (SELECT max(logisticsType) as logisticsTypes, deliver_id fROM logistics_detail  GROUP BY deliver_id ) as ld 
on ld.deliver_id=dm.deliver_id  
INNER JOIN order_detail odt on odt.detail_id=dd.detail_id 
INNER JOIN order_slave os on os.slave_id=odt.slave_id 
INNER JOIN order_master om on os.order_id=om.order_id               
INNER JOIN product_item pii on pii.item_id =odt.item_id   
INNER JOIN vendor vds on vds.vendor_id=dm.export_id  
INNER JOIN vendor vd on vd.vendor_id=odt.item_vendor_id  
INNER JOIN product pt on pii.product_id=pt.product_id where odt.item_mode !=1  ");
                str.AppendFormat(strcondition.ToString()); 
                strCount.Append(strcondition); 
                totalCount = 0;
                if (deliver.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(strCount.ToString());
                    
                    int.TryParse(_dt.Rows[0]["count"].ToString(), out totalCount);

                    str.AppendFormat(" limit {0},{1}", deliver.Start, deliver.Limit);
                }
                dt = _access.getDataTable(str.ToString());

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterDao.GetDeliveryMsgList-->" + ex.Message + str.ToString() + strcondition.ToString(), ex);
            }
        }

        public DataTable GetDeliveryMsgExcelList(DeliverMasterQuery deliver)
        {
            StringBuilder strcondition = new StringBuilder();
            StringBuilder str = new StringBuilder();
            DataTable dt = new DataTable();
            DataTable dt_type = new DataTable();
            try
            {
                if (deliver.time_type == 2)
                {
                    strcondition.AppendFormat(" and om.order_createdate >='{0}' and om.order_createdate <='{1}' ", Common.CommonFunction.GetPHPTime(deliver.order_time_begin.ToString()), Common.CommonFunction.GetPHPTime(deliver.order_time_end.ToString()));
                }
              
                if (!string.IsNullOrEmpty(deliver.sorder_id))
                {
                    strcondition.AppendFormat(" and om.order_id = '{0}' ", deliver.sorder_id);
                }
                if (!string.IsNullOrEmpty(deliver.sdeliver_id))
                {
                    strcondition.AppendFormat(" and dm.deliver_id = '{0}' ", deliver.sdeliver_id);
                }
                if (deliver.deliver_store != 0)
                {
                    strcondition.AppendFormat(" and dm.delivery_store='{0}' ", deliver.deliver_store);
                }
                if (deliver.i_order_status != -1)
                {
                    strcondition.AppendFormat(" and om.order_status='{0}' ", deliver.i_order_status);
                }
                if (deliver.order_day != 0)
                {
                    strcondition.AppendFormat(" and dm.deliver_org_days<='{0}'  ", Common.CommonFunction.GetPHPTime(DateTime.Now.AddDays(deliver.order_day).ToString("yyyy-MM-dd 23:59:59")));//預計到貨日期且未到貨
                }
                if (deliver.i_slave_status != -1)
                {
                    strcondition.AppendFormat(" and os.slave_status='{0}' ", deliver.i_slave_status);
                }
                if (deliver.i_detail_status != -1)
                {
                    strcondition.AppendFormat(" and odt.detail_status='{0}' ", deliver.i_detail_status);
                }
                if (deliver.payment != 0)
                {
                    strcondition.AppendFormat(" and om.order_payment='{0}' ", deliver.payment);
                }
                if (deliver.logisticsType != 0)
                {
                    strcondition.AppendFormat(" and ld.logisticsTypes='{0}' ", deliver.logisticsType);
                }
                if (deliver.ideliver_status != -1)
                {
                    strcondition.AppendFormat(" and dm.delivery_status='{0}' ", deliver.ideliver_status);
                }
                if (deliver.product_mode != 0)
                {
                    strcondition.AppendFormat(" and pt.product_mode ='{0}' ", deliver.product_mode);
                }
                if (!string.IsNullOrEmpty(deliver.serch_where))
                {
                    strcondition.AppendFormat(" and vd.vendor_id in ({0})", deliver.serch_where);
                }
                strcondition.AppendFormat(" )as mytable ");
                if (!string.IsNullOrEmpty(deliver.t_days.ToString()) && deliver.t_days != -1)
                {
                    strcondition.AppendFormat(" where overdue_day>='{0}' ", deliver.t_days);
                }
                str.AppendFormat(@"SELECT delivery_code,deliver_id,delivery_date,created,order_id,order_date,order_status,order_payment,delivery_store,
logisticsTypes,delivery_status,dvendor_name_simple,vendor_name_simple,freight_set,delivery_freight_cost,delivery_date_str,
overdue_day,
arrival_date,estimated_delivery_date 
,estimated_arrival_date,estimated_arrival_period,delivery_name,product_name,note_order,note_admin,
buy_num,item_id,product_mode,deliver_master_date,slave_status,detail_status,product_id,detail_id 
FROM (
SELECT dm.delivery_code,case dm.deliver_org_days when 0 then '-' else FROM_UNIXTIME(dm.deliver_org_days) end as delivery_date_str,dm.deliver_id,dm.created,om.order_id,DATE(FROM_UNIXTIME(om.order_createdate)) as order_date,dd.detail_id ,pt.product_id,om.order_status,om.order_payment,dm.delivery_store,
ld.logisticsTypes,dm.delivery_status,vds.vendor_name_simple as dvendor_name_simple,vd.vendor_name_simple,dm.freight_set,dm.delivery_freight_cost,
case ISNULL(dm.delivery_date) when TRUE then DATEDIFF(NOW(),DATE(FROM_UNIXTIME(om.order_date_pay)))  ELSE datediff(dm.delivery_date,DATE(FROM_UNIXTIME(om.order_date_pay))) end as overdue_day,
dm.arrival_date,dm.estimated_delivery_date 
,dm.estimated_arrival_date,dm.estimated_arrival_period,dm.delivery_name,odt.product_name,om.note_order,om.note_admin,
case odt.item_mode WHEN 2 THEN odt.parent_num * odt.buy_num ELSE odt.buy_num END as buy_num,pii.item_id,pt.product_mode,DATE(FROM_UNIXTIME(om.order_date_pay)) as deliver_master_date,dm.delivery_date,os.slave_status,odt.detail_status
FROM deliver_master dm  
INNER JOIN deliver_detail dd on dd.deliver_id = dm.deliver_id
LEFT JOIN (SELECT max(logisticsType) as logisticsTypes, deliver_id fROM logistics_detail  GROUP BY deliver_id ) as ld 
on ld.deliver_id=dm.deliver_id  
INNER JOIN order_detail odt on odt.detail_id=dd.detail_id 
INNER JOIN order_slave os on os.slave_id=odt.slave_id 
INNER JOIN order_master om on os.order_id=om.order_id               
INNER JOIN product_item pii on pii.item_id =odt.item_id   
INNER JOIN vendor vds on vds.vendor_id=dm.export_id  
INNER JOIN vendor vd on vd.vendor_id=odt.item_vendor_id  
INNER JOIN product pt on pii.product_id=pt.product_id where odt.item_mode !=1 ");
                str.AppendFormat(strcondition.ToString());
                dt = _access.getDataTable(str.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterDao.GetDeliveryMsgExcelList-->" + ex.Message + str.ToString() + strcondition.ToString(), ex);
            }
        }

        public DataTable GetVnndorId(string name)
        {
            StringBuilder sbSql = new StringBuilder();
            try
            {
                sbSql.AppendFormat(@"SELECT vendor_id FROM vendor WHERE vendor_name_simple like '%{0}%';",name);
                return _access.getDataTable(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" DeliverMasterDao-->GetVnndorId-->" + ex.Message + sbSql.ToString(), ex);
            }
        }

        public int GetDeliverMasterCount(DeliverMasterQuery query)
        {
            StringBuilder sbStr = new StringBuilder("SELECT  count(dm.deliver_id) FROM deliver_master dm ");
            sbStr.AppendFormat("WHERE dm.delivery_status IN(2,3) AND dm.export_id={0} AND  dm.delivery_store={1} AND dm.delivery_date BETWEEN  '{2}' AND '{3}';", query.export_id, query.delivery_store, Common.CommonFunction.DateTimeToString(query.time_start), Common.CommonFunction.DateTimeToString(query.time_end));
            return int.Parse(_access.getDataTable(sbStr.ToString()).Rows[0][0].ToString());
        }
        /// <summary>
        /// 根據時間間隔查取相應的物流單號  add by yafeng0715j 20151019AM 
        /// </summary>
        /// <returns></returns>
        public DataTable GetDeliverMaster(string hourNum)
        {
            int hoursNum = int.Parse(hourNum);
            DateTime date = DateTime.Now.AddHours(hoursNum);
            string sql = string.Format(@"SELECT  dm.deliver_id,delivery_code from deliver_master dm WHERE not EXISTS (SELECT deliver_id from deliver_status ds where ds.deliver_id=dm.deliver_id and ds.state=99)and  dm.delivery_store=1 and dm.deliver_org_days <>0 and dm.delivery_code<>''and created between '{0}' and '{1}'", Common.CommonFunction.DateTimeToString(date), Common.CommonFunction.DateTimeToString(DateTime.Now));
            DataTable table = _access.getDataTable(sql);
            return table;
        }

        #region 出貨單期望到貨日
        ///add by zhaozhi0623j 20151110 pm
        /// <summary>
        /// 根據出貨單編號更新期望到貨日期、時段   
        /// </summary>
        /// <returns></returns>
        public int UpdateExpectArrive(DeliverMasterQuery Query)
        {
            StringBuilder sbSql = new StringBuilder();
            Query.Replace4MySQL();

            try
            {
                sbSql.AppendFormat(@"update deliver_master set expect_arrive_date='{0}',expect_arrive_period='{1}' where deliver_id='{2}'",
                                Query.expect_arrive_date.ToString("yyyy-MM-dd"), Query.expect_arrive_period, Query.deliver_id);     
                return _access.execCommand(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" DeliverMasterDao-->UpdateExpectArrive-->" + ex.Message + sbSql.ToString(), ex);
            }
        }
        /// <summary>
        /// 獲得出貨單期望到貨日list   
        /// </summary>
        /// <returns></returns>
        public List<DeliverMasterQuery> GetDeliverExpectArriveList(DeliverMasterQuery Query, out int totalCount)
        {
            StringBuilder finalSql = new StringBuilder();
            StringBuilder sbSql = new StringBuilder();
            StringBuilder fromSql = new StringBuilder();
            StringBuilder conSql = new StringBuilder();
            Query.Replace4MySQL();
            try
            {
                sbSql.Append(@"select dm.deliver_id,dm.order_id,om.user_id,tp.parameterName as delivery_status_str,dm.type,dm.freight_set,v.vendor_name_full,
                                            dm.estimated_delivery_date,dm.deliver_org_days,dm.estimated_arrival_period,
                                            dm.expect_arrive_date,dm.expect_arrive_period ");
                fromSql.Append(@"from deliver_master dm inner JOIN vendor v on v.vendor_id=dm.export_id inner JOIN order_master om on om.order_id=dm.order_id                                
                           LEFT JOIN (SELECT * from t_parametersrc  where parameterType ='delivery_status') tp on tp.parameterCode=dm.delivery_status 

                                    where 1=1 ");
                if (Query.type != 0)
                {
                    if (Query.type == 101)
                    {
                        conSql.AppendFormat(" and dm.type='{0}'", 101);
                    }
                    else
                    {
                        conSql.AppendFormat(" and dm.type='{0}'", Query.type);
                    }              
                }
                if (Query.freight_set != 0)
                {
                    conSql.AppendFormat(" and dm.freight_set='{0}'", Query.freight_set);
                }
                if (Query.delivery_status != 10000)
                {
                    conSql.AppendFormat(" and dm.delivery_status='{0}'", Query.delivery_status);
                }


                if (Query.deliver_id != 0)
                {
                    conSql.AppendFormat(" and dm.deliver_id='{0}'", Query.deliver_id);
                }
                if (Query.order_id != 0)
                {
                    conSql.AppendFormat(" and dm.order_id='{0}'", Query.order_id);
                }
                if (Query.time_start != DateTime.MinValue && Query.time_end != DateTime.MinValue)
                {
                    conSql.AppendFormat(" and dm.deliver_org_days between '{0}' and '{1}'",CommonFunction.GetPHPTime(Query.time_start.ToString("yyyy-MM-dd 00:00:00")), CommonFunction.GetPHPTime(Query.time_end.ToString("yyyy-MM-dd 23:59:59")));
                    
                }
                //if (Query.time_end != DateTime.MinValue)
                //{
                //    conSql.AppendFormat(" and dm.deliver_org_days <= '{0}'", Query.time_end.ToString("yyyy-MM-dd"));
                //    //BLL.gigade.Common.CommonFunction.DateTimeToString(Query.time_end)
                //}
                if (Query.vendor_id != 0)
                {
                    conSql.AppendFormat(" and dm.export_id='{0}'",Query.vendor_id);
                }
                if (!string.IsNullOrEmpty(Query.vendor_name_full))
                {
                    conSql.AppendFormat(" and v.vendor_name_full like '%{0}%'", Query.vendor_name_full);
                }
                finalSql.Append(sbSql.ToString() + fromSql.ToString() + conSql.ToString());

                totalCount = 0;
                if (Query.IsPage)
                {
                    DataTable _dt = _access.getDataTable(" select dm.deliver_id " + fromSql.ToString() + conSql.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }
                    finalSql.AppendFormat(" limit {0},{1} ", Query.Start, Query.Limit);
                }
                return _access.getDataTableForObj<DeliverMasterQuery>(finalSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" DeliverMasterDao-->GetDeliverExpectArriveList-->" + ex.Message + finalSql.ToString(), ex);
            }
        } 
        #endregion
    }
}
