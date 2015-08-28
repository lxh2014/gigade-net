/* 
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

namespace BLL.gigade.Mgr
{
    public class OrderDetailMgr : IOrderDetailImplMgr
    {
        private IOrderDetailImplDao _orderDetailDao;
        //private OrderDetailDao _orderDetailDao;
        public OrderDetailMgr(string connectionStr)
        {
            _orderDetailDao = new OrderDetailDao(connectionStr);
        }

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
    }
}
