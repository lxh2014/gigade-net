/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：OrderSlaveMasterDao 
 * 摘   要： 
 *  批次出貨單列表查詢
 * 当前版本：1.0 
 * 作   者：shuangshuang0420j
 * 完成日期：2015/1/12 10:34:17 
 * 修改：
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using System.Data;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao
{
    public class OrderSlaveMasterDao : IOrderSlaveMasterImplDao
    {
        private IDBAccess _dbAccess;
        public OrderSlaveMasterDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }
        public List<OrderSlaveMasterQuery> GetBatchList(OrderSlaveMasterQuery store, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();

            totalCount = 0;
            try
            {//
                sql.Append(@"SELECT	osm.slave_master_id,osm.code_num,osm.paper,osm.order_freight_normal,osm.order_freight_low, ");
                sql.Append(@" osm.normal_subtotal,osm.hypothermia_subtotal,osm.deliver_store,osm.deliver_code,FROM_UNIXTIME(osm.deliver_time) as deliver_date, ");
                sql.Append(@" osm.deliver_note,FROM_UNIXTIME(osm.createdate) as create_date,osm.creator,osm.on_check,v.vendor_name_simple,t_type.parameterName as deliver_name");
                sqlCondi.Append(@"  FROM order_slave_master osm ");
                sqlCondi.Append(@" INNER JOIN vendor  v on osm.creator=v.vendor_id ");
                sqlCondi.Append(@"  LEFT JOIN (SELECT parameterName,parameterCode from  t_parametersrc  where parameterType='Deliver_Store') as t_type on t_type.parameterCode=osm.deliver_store ");
                sqlCondi.Append(@" WHERE 1=1 ");
                if (!string.IsNullOrEmpty(store.code_num))
                {
                    sqlCondi.AppendFormat(" AND osm.code_num like '%{0}%'", store.code_num);
                }
                if (!string.IsNullOrEmpty(store.vendor_name_simple))
                {
                    sqlCondi.AppendFormat(" AND v.vendor_name_simple like '%{0}%'", store.vendor_name_simple);
                }
                if (store.is_check != -1)
                {
                    sqlCondi.AppendFormat(" AND osm.on_check ='{0}'", store.on_check);
                }
                if (store.date_type == 1)
                {
                    sqlCondi.AppendFormat(" AND osm.createdate >='{0}' AND osm.createdate<='{1}'", store.date_start, store.date_end);
                }
                else if (store.date_type == 2)
                {
                    sqlCondi.AppendFormat(" AND osm.deliver_time >='{0}' AND osm.deliver_time<='{1}'", store.date_start, store.date_end);
                }
                sqlCondi.Append(@" ORDER BY  osm.code_num DESC");
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _dbAccess.getDataTable(string.Format(" select count(1) as totalCount from ( SELECT osm.slave_master_id {0} ) t", sqlCondi.ToString()));
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = int.Parse(_dt.Rows[0]["totalCount"].ToString());
                    }
                    sqlCondi.AppendFormat(" limit {0},{1}", store.Start, store.Limit);
                }
                sql.Append(sqlCondi.ToString());
                return _dbAccess.getDataTableForObj<OrderSlaveMasterQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderSlaveMasterDao-->GetBatchList-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public List<OrderSlaveMasterQuery> GetSlaveByMasterId(OrderSlaveMasterQuery store)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();


            try
            {//
                sql.Append(@"SELECT	osd.slave_id , om.order_id   ");
                sqlCondi.Append(@"  FROM order_slave_detail osd,order_slave_master osm,order_master om,order_slave os ");
                sqlCondi.Append(@" WHERE osd.slave_master_id = osm.slave_master_id  AND om.order_id = os.order_id AND osd.slave_id = os.slave_id   ");
                if (store.slave_master_id != 0)
                {
                    sqlCondi.AppendFormat(" AND osd.slave_master_id ='{0}'", store.slave_master_id);
                }

                sql.Append(sqlCondi.ToString());
                return _dbAccess.getDataTableForObj<OrderSlaveMasterQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderSlaveMasterDao-->GetSlaveByMasterId-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public List<OrderSlaveMasterQuery> GetDetailBySlave(string slaves)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sql.Append(@"SELECT	od.slave_id,om.order_id,od.item_id,od.product_name,od.product_spec_name,od.buy_num  ");
                sqlCondi.Append(@"  FROM  order_detail od  INNER JOIN order_slave os ON os.slave_id=od.slave_id");
                sqlCondi.Append(@" LEFT JOIN order_master om ON om.order_id=os.order_id");
                sqlCondi.Append(@" WHERE  od.combined_mode<1 and od.item_mode <>1 ");//組合商品 0:一般 1:組合 2:子商品;//0:單一商品, 1:父商品, 2:子商品

                if (!string.IsNullOrEmpty(slaves))
                {
                    sqlCondi.AppendFormat(" AND os.slave_id in ({0})", slaves);
                }
                sqlCondi.Append(" ORDER BY om.order_id ASC");
                sql.Append(sqlCondi.ToString());
                return _dbAccess.getDataTableForObj<OrderSlaveMasterQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderSlaveMasterDao-->GetDetailBySlave-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public List<OrderSlaveMasterQuery> GetOrderByMasterIDs(string slaves)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();


            try
            {
                sql.Append(@"select osd.slave_id, os.order_id   ");
                sqlCondi.Append(@" from order_slave_detail osd,order_slave_master osm,order_slave os");
                sqlCondi.Append(@" where osm.slave_master_id=osd.slave_master_id and os.slave_id = osd.slave_id");
                sqlCondi.Append(@" and os.slave_status = 6  ");//slave_status = 6 進倉中

                if (!string.IsNullOrEmpty(slaves))
                {
                    sqlCondi.AppendFormat(" and osm.slave_master_id in ({0}) ", slaves);
                }

                sql.Append(sqlCondi.ToString());
                return _dbAccess.getDataTableForObj<OrderSlaveMasterQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderSlaveMasterDao-->GetOrderByMasterIDs-->" + ex.Message + sql.ToString(), ex);
            }
        }
        //到貨確認功能棄用
        //public List<OrderSlaveMasterQuery> GetDeliverByOrderIDs(string orderIDs)
        //{
        //    StringBuilder sql = new StringBuilder();
        //    StringBuilder sqlCondi = new StringBuilder();
        //    try
        //    {
        //        sql.Append(@"select dm.deliver_id, detail_status   ");
        //        sqlCondi.Append(@" from order_master om");
        //        sqlCondi.Append(@" left join deliver_master dm on dm.order_id=om.order_id ");
        //        sqlCondi.Append(@" left join deliver_detail dd on dd.deliver_id=dm.deliver_id");
        //        sqlCondi.Append(@" left join order_detail od on od.detail_id=dd.detail_id");
        //        sqlCondi.AppendFormat(@" where om.order_id in ({0}) and detail_status in (2,6,7)", orderIDs);//7:已進倉 6：進倉中 2：待出貨
        //        //  sqlCondi.Append(@" and dm.delivery_type=1");//該字段已不存在
        //        sqlCondi.Append(@" group by om.order_id");
        //        sqlCondi.Append(@" having detail_status=7 and count(distinct detail_status)=1");
        //        sql.Append(sqlCondi.ToString());
        //        return _dbAccess.getDataTableForObj<OrderSlaveMasterQuery>(sql.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("OrderSlaveMasterDao-->GetDeliverByOrderIDs-->" + ex.Message + sql.ToString(), ex);
        //    }
        //}

    }
}
