#region 文件信息
/* 
* Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：OrderExpectDeliverDao.cs      
* 摘 要：                                                                               
* 預購單
* 当前版本：v1.1                                                                 
* 作 者： shuangshuang0420j                                           
* 完成日期：2014/10/21
* 修改歷史：                                                                     
*         
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using System.Data;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
using BLL.gigade.Common;

namespace BLL.gigade.Dao
{
    public class OrderExpectDeliverDao : IOrderExpectDeliverImplDao
    {
        private IDBAccess _accessMySql;
        public OrderExpectDeliverDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        #region 預購單列表
        public List<OrderExpectDeliverQuery> GetOrderExpectList(OrderExpectDeliverQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sql.Append(@" SELECT oed.expect_id,oed.order_id,oed.slave_id,oed.detail_id,oed.`status`,oed.store,oed.`code`,FROM_UNIXTIME(oed.time) as stime,oed.note,FROM_UNIXTIME(oed.createdate) as date_one,FROM_UNIXTIME(oed.updatedate) as date_two,om.note_order,od.product_name,od.detail_status,c.remark as d_status_name ");
                sqlCondi.Append(" FROM order_expect_deliver oed ,order_master om,order_detail od left join (select parametercode,parametername,remark from t_parametersrc where parametertype='order_status') c on od.detail_status=c.parametercode ");
                sqlCondi.Append(" WHERE  oed.order_id=om.order_id AND oed.detail_id =od.detail_id ");

                if (query.date_one != DateTime.MinValue)
                {
                    sqlCondi.AppendFormat(" and oed.createdate >='{0}'", CommonFunction.GetPHPTime(query.date_one.ToString("yyyy-MM-dd HH:mm:ss")));

                }
                if (query.date_two != DateTime.MinValue)
                {
                    sqlCondi.AppendFormat(" and oed.createdate <='{0}'", CommonFunction.GetPHPTime(query.date_two.ToString("yyyy-MM-dd HH:mm:ss")));
                }

                if (query.query_status != -1)//-1表所有狀態，0表示未出貨
                {
                    sqlCondi.AppendFormat(" and oed.`status`='{0}'", query.query_status);
                }
                totalCount = 0;
                if (query.IsPage)
                {
                    System.Data.DataTable _dt = _accessMySql.getDataTable("select count(*) as totalCount " + sqlCondi.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sqlCondi.AppendFormat(" limit {0},{1}", query.Start, query.Limit);

                }
                sql.Append(sqlCondi.ToString());
                return _accessMySql.getDataTableForObj<OrderExpectDeliverQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderExpectDeliverDao.GetOrderExpectList-->" + ex.Message + sql.ToString(), ex);
            }

        }
        #endregion
        #region 預購單出貨+int OrderExpectModify(OrderExpectDeliverQuery store)

        public int OrderExpectModify(OrderExpectDeliverQuery store)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" UPDATE order_expect_deliver SET `status`='{0}',store='{1}',code='{2}',time='{3}',updatedate='{4}',note='{5}' WHERE expect_id='{6}';", store.status, store.store, store.code, store.time, store.updatedate, store.note, store.expect_id);
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderExpectDeliverDao.OrderExpectModify-->" + ex.Message + sql.ToString(), ex);
            }
        }

        #endregion

        #region 出貨單匯出+List<OrderExpectDeliverQuery> GetModel(OrderExpectDeliverQuery store)
        public List<OrderExpectDeliverQuery> GetModel(OrderExpectDeliverQuery store)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {

                if (store.date_one != DateTime.MinValue)
                {
                    sqlCondi.AppendFormat(" and oed.createdate >='{0}'", CommonFunction.GetPHPTime(store.date_one.ToString("yyyy-MM-dd HH:mm:ss")));

                }
                if (store.date_two != DateTime.MinValue)
                {
                    sqlCondi.AppendFormat(" and oed.createdate <='{0}'", CommonFunction.GetPHPTime(store.date_two.ToString("yyyy-MM-dd HH:mm:ss")));
                }

                if (store.query_status != -1)//-1表所有狀態，0表示未出貨
                {
                    sqlCondi.AppendFormat(" and oed.`status`='{0}'", store.query_status);
                }
                sql.Append(@" SELECT oed.expect_id,CONCAT_WS(' ',om.delivery_zip,tzc.middle,tzc.small,om.delivery_address) as zip,om.order_address,oed.order_id,oed.slave_id,oed.detail_id, ");
                sql.Append(" oed.`status`,oed.`code`,oed.time,oed.note,oed.store, ");
                sql.Append(" oed.createdate,oed.updatedate,od.detail_status,om.note_order,od.product_name, ");
                sql.Append(" od.item_id,om.order_name,om.delivery_name,om.delivery_mobile,om.delivery_zip,");
                sql.Append(" om.delivery_address,od.buy_num,od.single_money,od.deduct_bonus,(od.single_money*od.buy_num-od.deduct_bonus) as sum ");
                sql.AppendFormat(" FROM order_expect_deliver oed ,order_master om,order_detail od, t_zip_code tzc ");
                sql.AppendFormat(" WHERE 1=1 AND oed.order_id=om.order_id AND oed.detail_id =od.detail_id and tzc.zipcode= om.delivery_zip ");
                return _accessMySql.getDataTableForObj<OrderExpectDeliverQuery>(sql.ToString() + sqlCondi.ToString());
            }
            catch(Exception ex)
            {
             throw new Exception("OrderExpectDeliverDao-->GetModel"+ex.Message+sql.ToString(),ex);
            }
        }

        #endregion


    }
}
