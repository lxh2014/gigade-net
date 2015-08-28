/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：OrderSlaveMasterMgr 
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
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
using System.Collections;
using BLL.gigade.Model;


namespace BLL.gigade.Mgr
{
    public class OrderSlaveMasterMgr : IOrderSlaveMasterImplMgr
    {
        private IOrderSlaveMasterImplDao _orderSlaveMasterDao;
        private MySqlDao _mysqlDao;
        private ISerialImplDao _serialDao;
        private IParametersrcImplDao _paraDao;
        public OrderSlaveMasterMgr(string connectionStr)
        {
            _orderSlaveMasterDao = new OrderSlaveMasterDao(connectionStr);
            _serialDao = new SerialDao(connectionStr);
            _mysqlDao = new MySqlDao(connectionStr);
            _paraDao = new ParametersrcDao(connectionStr);
        }
        public List<OrderSlaveMasterQuery> GetBatchList(OrderSlaveMasterQuery store, out int totalCount)
        {
            try
            {
                return _orderSlaveMasterDao.GetBatchList(store, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderSlaveMasterMgr-->GetBatchList-->" + ex.Message, ex);
            }
        }
        public List<OrderSlaveMasterQuery> GetSlaveByMasterId(OrderSlaveMasterQuery store)
        {
            try
            {
                return _orderSlaveMasterDao.GetSlaveByMasterId(store);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderSlaveMasterMgr-->GetSlaveByMasterId-->" + ex.Message, ex);
            }
        }
        public List<OrderSlaveMasterQuery> GetDetailBySlave(string slaves)
        {
            try
            {
                return _orderSlaveMasterDao.GetDetailBySlave(slaves);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderSlaveMasterMgr-->GetDetailBySlave-->" + ex.Message, ex);
            }
        }
        ////到貨確認功能棄用
//        public bool BatchSendProd(string slaveMasters, string userName, string userIP)
//        {
//            try
//            {
//                ArrayList arryList = new ArrayList();
//                string orderIDs = string.Empty;
//                string deliverIDs = string.Empty;
//                //查出訂單狀態
//                //  Parametersrc para = _paraDao.Query(new Parametersrc { parameterName = "ORDER_STATUS_DISPATCH_IN", Used = 1 }).FirstOrDefault();//已進倉
//                //修改出貨單的on_check為已檢查
//                arryList.Add(string.Format(@"update order_slave_master set on_check=1 where slave_master_id in ({0});", slaveMasters));
//                //查出出貨單狀態為進倉中的數據
//                List<OrderSlaveMasterQuery> store = _orderSlaveMasterDao.GetOrderByMasterIDs(slaveMasters);

//                foreach (var item in store)
//                {
//                    //修改付款單狀態為已進倉
//                    arryList.Add(string.Format(@"update order_detail set detail_status=7 where slave_id='{0}' and detail_status=6;", item.slave_id));
//                    arryList.Add(string.Format(@"update order_slave set slave_status=7,slave_updatedate='{0}',slave_ipfrom='{1}' where slave_id='{2}';"
//                        , Common.CommonFunction.GetPHPTime(), userIP, item.slave_id));
//                    //獲取訂單狀態流水號
//                    arryList.Add(_serialDao.Update(31));
//                    Serial serialQuery = _serialDao.GetSerialById(31);
//                    //記錄付款單狀態變更
//                    arryList.Add(string.Format(@"insert into order_slave_status
//                     ('serial_id','slave_id','order_status','status_description',
//                      'status_ipfrom','status_createdate')values(
//                       '{0}','{1}','{2}','{3}','{4}','{5}');",
//                      serialQuery.Serial_Value, item.slave_id, 7, "Writer:user(" + userName + ")已進倉",
//                     userIP, Common.CommonFunction.GetPHPTime()));
//                    orderIDs += item.order_id + ",";

//                }
//                if (!string.IsNullOrEmpty(orderIDs))
//                {
//                    orderIDs = orderIDs.Substring(0, orderIDs.Length - 1);
//                    List<OrderSlaveMasterQuery> query = _orderSlaveMasterDao.GetDeliverByOrderIDs(orderIDs);
//                    foreach (var item in query)
//                    {
//                        deliverIDs += item.deliver_id + ",";
//                    }
//                    if (!string.IsNullOrEmpty(deliverIDs))
//                    {
//                        deliverIDs.Substring(0, deliverIDs.Length - 1);
//                        //delivery_status: 0 => '待出貨', 1 => '可出貨',  2 => '出貨中',3 => '已出貨', 4 => '已到貨',
//                        // 5 => '未到貨',  6 => '取消出貨',7 => '待取貨'
//                        arryList.Add(string.Format(@" update deliver_master set delivery_status = 1 where deliver_id in ({0}) and delivery_status in (0,4); ", deliverIDs));

//                    }
//                }

//                return _mysqlDao.ExcuteSqls(arryList);
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("OrderSlaveMasterMgr-->BatchSendProd-->" + ex.Message, ex);
//            }
//        }
    }
}
