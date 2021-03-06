﻿/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：OrderDetailMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/22 17:02:50 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;
using System.Data;
using System.Collections;
using BLL.gigade.Common;

namespace BLL.gigade.Mgr
{
    public class OrderDetailMgr : IOrderDetailImplMgr
    {
        private IOrderDetailImplDao _orderDetailDao;
        private IOrderSlaveImplDao _orderSlaveDao;
        private IOrderMasterImplDao _orderMaterDao;
        private ISerialImplDao _serial;
        private MySqlDao _mySqlDao;
        private IProductCategoryImplDao _productCategoryDao;
        private OrderMasterStatusDao _orderMaterStatusDao;
        private IParametersrcImplDao _parametersrcDao;
       // private ISiteImplDao _siteDao;
        //private OrderDetailDao _orderDetailDao;
        private IChannelImplDao _channelDao;
        public OrderDetailMgr(string connectionStr)
        {
            _orderDetailDao = new OrderDetailDao(connectionStr);
            _orderSlaveDao = new OrderSlaveDao(connectionStr);
            _orderMaterDao = new OrderMasterDao(connectionStr);
            _orderMaterStatusDao = new OrderMasterStatusDao(connectionStr);
            _serial = new SerialDao(connectionStr);
            _mySqlDao = new MySqlDao(connectionStr);
            _productCategoryDao = new ProductCategoryDao(connectionStr);
            _parametersrcDao = new ParametersrcDao(connectionStr);
            //_siteDao = new SiteDao(connectionStr); 
            _channelDao = new ChannelDao(connectionStr);
        }

        #region 開發用
        public string Save(BLL.gigade.Model.OrderDetail orderDetail)
        {
            return _orderDetailDao.Save(orderDetail);
        }
        public int Delete(int detailId)
        {

            try
            {
                return _orderDetailDao.Delete(detailId);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailMgr-->Delete-->" + ex.Message, ex);
            }
        }

        public List<OrderDetail> QueryReturnDetail(uint return_id)
        {
            try
            {
                return _orderDetailDao.QueryReturnDetail(return_id);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailMgr-->QueryReturnDetail-->" + ex.Message, ex);
            }
        }

        public List<Model.Query.OrderDetailQuery> GetOrderDetailList(Model.Query.OrderDetailQuery query, out int totalCount)
        {
            try
            {
                return _orderDetailDao.GetOrderDetailList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailMgr-->GetOrderDetailList-->" + ex.Message, ex);
            }
        }
        public List<BLL.gigade.Model.Query.OrderDetailQuery> SubPoenaDetail(BLL.gigade.Model.Query.OrderDetailQuery query)
        {
            try
            {
                return _orderDetailDao.SubPoenaDetail(query);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailMgr-->SubPoenaDetail-->" + ex.Message, ex);
            }
        }
        public bool no_delivery(BLL.gigade.Model.Query.OrderDetailQuery query)
        {
            try
            {
                return _orderDetailDao.no_delivery(query);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailMgr-->no_delivery-->" + ex.Message, ex);
            }
        }
        public int split_detail(BLL.gigade.Model.Query.OrderDetailQuery query)
        {
            try
            {
                return _orderDetailDao.split_detail(query);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailMgr-->split_detail-->" + ex.Message, ex);
            }
        }
        public List<Model.Query.OrderDetailQuery> VendorPickPrint(Model.Query.OrderDetailQuery query, out int totalCount, string sqlappend = null) 
        {
            try
            {
                return _orderDetailDao.VendorPickPrint(query, out totalCount, sqlappend);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailMgr-->VendorPickPrint-->" + ex.Message, ex);
            }
        }
        public List<Model.Query.OrderDetailQuery> AllOrderDeliver(Model.Query.OrderDetailQuery query, out int totalCount, string sqlappend = null)
        {
            try
            {
                return _orderDetailDao.AllOrderDeliver(query, out totalCount, sqlappend);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailMgr-->AllOrderDeliver-->" + ex.Message, ex);
            }
        }
        public List<Model.Query.OrderDetailQuery> GetOrderDetailToSum(uint vendor_id, long time) 
        {
            try
            {
                return _orderDetailDao.GetOrderDetailToSum(vendor_id, time);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailMgr-->GetOrderDetailToSum-->" + ex.Message, ex);
            }
        }

        public System.Data.DataTable GetLeaveproductList(Model.Query.OrderDetailQuery query, out int totalCount, string sqlappend = null)
        {
            try
            {
                return _orderDetailDao.GetLeaveproductList(query,out totalCount,sqlappend);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailMgr-->GetLeaveproductList-->" + ex.Message, ex);
            }
        }

        public List<Model.Query.OrderDetailQuery> OrderVendorProducesList(Model.Query.OrderDetailQuery query, out int totalCount)
        {
            try
            {
                return _orderDetailDao.OrderVendorProducesList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailMgr-->OrderVendorProducesList" + ex.Message, ex);
            }
        }

        public System.Data.DataTable ProduceGroupCsv(Model.Query.OrderDetailQuery query)
        {
            try
            {
                return _orderDetailDao.ProduceGroupCsv(query);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailMgr-->ProduceGroupCsv-->" + ex.Message, ex);
            }
        }

        public System.Data.DataTable ProduceGroupExcel(Model.Query.OrderDetailQuery query)
        {
            try
            {
                return _orderDetailDao.ProduceGroupExcel(query);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailMgr-->ProduceGroupExcel-->" + ex.Message, ex);
            }
        }
        public List<OrderDetailQuery> DeliveryInformation(OrderDetailQuery query, string str)
        {
            try
            {
                return _orderDetailDao.DeliveryInformation(query, str);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailMgr-->DeliveryInformation" + ex.Message, ex);
            }
        }

        /// add by wwei0216w 2015/5/20 獲得運送達天數
        public List<OrderDetailCustom> GetArriveDay(uint detail_id)
        {
            try
            {
                List<OrderDetailCustom> list = _orderDetailDao.GetArriveDay(detail_id);
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailMgr-->GetArriveDay" + ex.Message,ex);
            }
        }

        /// <summary>
        /// 查詢購買金額與應稅類型
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <returns>購買金額與應稅類型列表</returns>
        public DataTable GetBuyAmountAndTaxType(OrderDetailQuery query)
        {
            try
            {
                return _orderDetailDao.GetBuyAmountAndTaxType(query);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailMgr-->GetBuyAmountAndTaxType-->" + ex.Message, ex);
            }
        }
        public List<OrderDetailQuery> GetOrderDetailList(OrderDetailQuery query)
        {
            try
            {
                return _orderDetailDao.GetOrderDetailList(query);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailMgr-->GetOrderDetailList-->" + ex.Message, ex);
            }        
        }

        public List<Vendor> GetVendor(Vendor query)
        {
            try
            {
                return _orderDetailDao.GetVendor(query);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailMgr-->GetVendor-->" + ex.Message, ex);
            }      
        }
#endregion

        public string OrderWaitClick(OrderMasterStatusQuery query)
        {
            string json = "";
            Serial s = new Serial();//流水號
            ArrayList sql = new ArrayList();
            OrderMaster om = new OrderMaster();
            OrderSlave os = new OrderSlave();
            OrderMasterStatusQuery oms = new OrderMasterStatusQuery();
            
            List<OrderSlaveQuery> vendor = new List<OrderSlaveQuery>();
            try
            {
                vendor = _orderSlaveDao.GetVendor(query.order_id);
                if (vendor.Count > 0)
                {
                    uint a = 1;
                    om.Order_Id = query.order_id;
                    om.Order_Status = query.order_status;
                    om.Order_Ipfrom = query.status_ipfrom;
                    //獲取變更order_master.status和付款money的sql
                    sql.Add(_orderMaterDao.UpdateOrderMasterStatus(om));

                    s = _serial.GetSerialById(29);//獲取訂單主檔狀態流水號 
                    sql.Add(_serial.Update(29));//變更流水號+1   
                    query.serial_id = s.Serial_Value+a;
                    //往訂單記錄表插入一條等待付款的狀態數據
                    sql.Add(_orderMaterStatusDao.Insert(query));
                    //
                    foreach (var item in vendor)
                    {
                        os.Slave_Id = item.Slave_Id;
                        os.Slave_Ipfrom = query.status_ipfrom;
                        sql.Add(_orderSlaveDao.UpdOrderSlaveStatus(os));
                        //往order_slave_status表插入數據
                        s = _serial.GetSerialById(31);//獲取slaver狀態表流水號 
                        sql.Add(_serial.Update(31));//變更流水號+1 
                        oms.serial_id = s.Serial_Value + a;
                        oms.slave_id = item.Slave_Id;
                        oms.order_status = query.order_status;
                        oms.status_description = query.status_description;
                        oms.status_ipfrom = query.status_ipfrom;
                        sql.Add(_orderMaterStatusDao.InsertSlave(oms));
                        a++;
                    }
                    if (_mySqlDao.ExcuteSqlsThrowException(sql))
                    {
                        json = "{success:true}";
                    }
                    else
                    {
                        json = "{success:true,msg:2}";//執行sql報錯
                    }   
                }
                else
                {
                    json = "{success:flase,msg:3}";//slave沒有數據
                }
                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailMgr-->OrderWaitClick-->sql:" + sql + ",Message:" + ex.Message, ex);
            }
        }

        public List<OrderDetailQuery> GetCategorySummaryList(OrderDetailQuery query, out int totalCount, out int SumAmount)
        {
            try
            {
                SumAmount = 0;
                totalCount = 0;
                ProductCategory pc_query = new ProductCategory();
                List<OrderDetailQuery> list = new List<OrderDetailQuery>();
                pc_query.category_id = query.category_id;
                pc_query.Start = query.Start;
                pc_query.Limit = query.Limit;
                DataTable dt = _productCategoryDao.GetCagegoryIdsByIdAndFatherId(pc_query);
                if (dt != null && dt.Rows.Count > 0)
                {
                    totalCount = dt.Rows.Count;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(dt.Rows[i]["category_id"].ToString()))
                        {
                            query.category_id = Convert.ToUInt32(dt.Rows[i]["category_id"]);
                            DataTable model = _orderDetailDao.GetCategorySummary(query);
                            OrderDetailQuery listRow = new OrderDetailQuery();
                            if (model != null && model.Rows.Count > 0)
                            {
                                listRow.category_name = dt.Rows[i]["category_name"].ToString();
                                listRow.category_id = query.category_id;
                                if (!string.IsNullOrEmpty(model.Rows[0]["amount"].ToString()))
                                {
                                    SumAmount += Convert.ToInt32(model.Rows[0]["amount"]);
                                    listRow.amount = Convert.ToInt32(model.Rows[0]["amount"]);
                                }
                                list.Add(listRow);
                            }
                        }
                    }
                }
                if (query.Start + query.Limit > list.Count)
                {
                    query.Limit = list.Count - query.Start;
                    if (query.Limit < 0)
                    {
                        query.Start = 0;
                        if (list.Count > 25)
                        {
                            query.Limit = 25;
                        }
                        else
                        {
                            query.Limit = list.Count;
                        }
                    }
                }
                list = list.GetRange(query.Start, query.Limit);
                return list;              
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->GetCategorySummaryList-->" + ex.Message, ex);
            }
        }

        public DataTable GetAmountDetial(OrderDetailQuery query,out int totalCount)
        {
            totalCount = 0;
            DataTable dt = new DataTable();
            try
            {
                dt = _orderDetailDao.GetAmountDetial(query, out totalCount);
                if (dt != null && dt.Rows.Count > 0)
                {                   
                    List<Parametersrc> parameterList = _parametersrcDao.SearchParameters("payment", "order_status", "product_mode");
                    foreach (DataRow dr in dt.Rows)
                    {
                        var alist = parameterList.Find(m => m.ParameterType == "payment" && m.ParameterCode == dr["order_payment"].ToString());
                        var blist = parameterList.Find(m => m.ParameterType == "order_status" && m.ParameterCode == dr["slave_status"].ToString());
                       
                        if (alist != null)
                        {
                            dr["payment_name"] = alist.parameterName;
                        }
                        if (blist != null)
                        {
                            dr["slave_status_name"] = blist.remark;
                        }
                        int channel = dr["channel"].ToString() == "" ? 0 : Convert.ToInt32(dr["channel"].ToString());
                        Channel clist = new Channel();
                        if (channel == 0)
                        {
                            dr["channel_name_simple"] = "";
                        }
                        else
                        {
                            clist = _channelDao.getSingleObj(channel);
                        }
                        if (clist != null)
                        {
                            dr["channel_name_simple"] = clist.channel_name_simple;
                        }
                        if (dr["order_createdate"] != null)
                        {
                            dr["order_createdate_format"] = CommonFunction.DateTimeToString(CommonFunction.GetNetTime(Convert.ToInt32(dr["order_createdate"].ToString())));
                        }
                        if (dr["deduct_bonus"] != null)
                        {
                            dr["deducts"] = Convert.ToInt32(dr["deduct_bonus"].ToString());
                        }
                        if (dr["money"] != null)
                        {
                            dr["amount"] = Convert.ToInt32(dr["money"].ToString());
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
               throw new Exception("OrderMasterMgr-->GetAmountDetial-->" + ex.Message, ex);
            }
        }
   
    }
}
