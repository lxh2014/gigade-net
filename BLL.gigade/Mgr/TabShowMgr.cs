using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;
namespace BLL.gigade.Mgr
{
    public class TabShowMgr : ITabShowImplMgr
    {
        private ITabShowImplDao _tabshowdao;
        public TabShowMgr(string connectionString)
        {
            _tabshowdao = new TabShowDao(connectionString);
        }
        #region 狀態列表

        public List<Model.Query.OrderMasterStatusQuery> GetStatus(Model.Query.OrderMasterStatusQuery store, out int totalCount)
        {
            try
            {
                return _tabshowdao.GetStatus(store, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("TabShowMgr-->GetStatus-->" + ex.Message, ex);
            }
        }

        #endregion

        #region 出貨單


        public List<Model.Query.OrderDeliverQuery> GetDeliver(Model.Query.OrderDeliverQuery store, out int totalCount)
        {
            try
            {
                return _tabshowdao.GetDeliver(store, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("TabShowMgr-->GetDeliver-->" + ex.Message, ex);
            }
        }

        #endregion

        #region 物流出貨狀態
        public List<LogisticsDetailQuery> GetLogistics(LogisticsDetailQuery store, out int totalCount)
        {
            try
            {
                return _tabshowdao.GetLogistics(store, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("TabShowMgr-->GetLogistics-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 退貨單
        #region 退貨單-上
        public List<OrderReturnContentQuery> GetOrderReturnContentQueryUp(OrderReturnContentQuery store, out int totalCount)
        {
            try
            {
                return _tabshowdao.GetOrderReturnContentQueryUp(store, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("TabShowMgr-->GetOrderReturnContentQueryUp-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 退貨單-下
        public List<OrderReturnMasterQuery> GetReturnMasterDown(OrderReturnMasterQuery store, out int totalCount) 
        {
            try
            {
                return _tabshowdao.GetReturnMasterDown(store, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("TabShowMgr-->GetReturnMasterDown-->" + ex.Message, ex);
            }
        }
        #endregion
        #endregion

        #region 支付寶
        public List<OrderPaymentAlipay> GetAlipayList(OrderPaymentAlipay store, out int totalCount)
        {
            try
            {
                return _tabshowdao.GetAlipayList(store, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("TabShowMgr-->GetAlipayList-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 銀聯
        public List<OrderPaymentUnionPay> GetUnionPayList(OrderPaymentUnionPay store, out int totalCount)
        {
            try
            {
                return _tabshowdao.GetUnionPayList(store, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("TabShowMgr-->GetUnionPayList-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 新出貨單


        public List<Model.Query.DeliverMasterQuery> GetNewDeliver(Model.Query.DeliverMasterQuery store, out int totalCount)
        {
            try
            {
                return _tabshowdao.GetNewDeliver(store, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("TabShowMgr-->GetNewDeliver-->" + ex.Message, ex);
            }
        }

        #endregion

        #region 購物金扣除記錄
        public List<UsersDeductBonus> GetUserDeductBonus(UsersDeductBonus store, out int totalCount)
        {
            try
            {
                return _tabshowdao.GetUserDeductBonus(store, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("TabShowMgr-->GetUserDeductBonus-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 取消單


        public List<Model.Query.OrderCancelMasterQuery> GetCancel(Model.Query.OrderCancelMasterQuery store, out int totalCount)
        {
            try
            {
                return _tabshowdao.GetCancel(store, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("TabShowMgr-->GetCancel-->" + ex.Message, ex);
            }
        }

        #endregion

        #region 退貨單


        public List<Model.Query.OrderReturnMasterQuery> GetReturn(Model.Query.OrderReturnMasterQuery store, out int totalCount)
        {
            try
            {
                return _tabshowdao.GetReturn (store, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("TabShowMgr-->GetReturn-->" + ex.Message, ex);
            }
        }

        #endregion

        #region 退款單


        public List<Model.Query.OrderMoneyReturnQuery> GetMoney(Model.Query.OrderMoneyReturnQuery store, out int totalCount)
        {
            try
            {
                return _tabshowdao.GetMoney (store, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("TabShowMgr-->GetMoney-->" + ex.Message, ex);
            }
        }

        #endregion

        #region 發票記錄
        public List<InvoiceMasterRecordQuery> GetInvoiceMasterRecord(InvoiceMasterRecordQuery store, out int totalCount)
        {
            try
            {
                return _tabshowdao.GetInvoiceMasterRecord(store, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("TabShowMgr-->GetInvoiceMasterRecord-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 問題與回覆


        public List<Model.Query.OrderQuestionQuery> GetQuestion(Model.Query.OrderQuestionQuery store, out int totalCount)
        {
            try
            {
                return _tabshowdao.GetQuestion (store, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("TabShowMgr-->GetQuestion-->" + ex.Message, ex);
            }
        }

        #endregion

        #region 中國信託
        public List<OrderPaymentCt> GetOrderPaymentCtList(OrderPaymentCt store, out int totalCount)
        {
            try
            {
                 return _tabshowdao.GetOrderPaymentCtList(store, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("TabShowMgr-->GetOrderPaymentCtList-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 取消單問題


        public List<OrderCancelMsgQuery> GetCancelMsg(OrderCancelMsgQuery store, out int totalCount)
        {
            try
            {
                return _tabshowdao.GetCancelMsg (store, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("TabShowMgr-->GetCancelMsg-->" + ex.Message, ex);
            }
        }

        #endregion

        #region 華南匯款資料
        public List<OrderPaymentHncbQuery> QueryOrderHncb(OrderPaymentHncbQuery store, out int totalCount)
        {
            try
            {
                return _tabshowdao.QueryOrderHncb(store, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("TabShowMgr-->QueryOrderHncb-->" + ex.Message, ex);
            }
        
        }
        #endregion

        #region 聯合信用卡


        public List<OrderPaymentNcccQuery> GetNCCC(OrderPaymentNcccQuery store, out int totalCount)
        {
            try
            {
                return _tabshowdao.GetNCCC (store, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("TabShowMgr-->GetNCCC-->" + ex.Message, ex);
            }
        }

        #endregion

        #region Hitrust-網際威信
        public List<OrderPaymentHitrustQuery> GetOderHitrust(OrderPaymentHitrustQuery store, out int totalCount)
        {
            try
            {
                return _tabshowdao.GetOderHitrust(store, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("TabShowMgr-->GetOderHitrust-->" + ex.Message, ex);
            }
        }
        public DataTable GetOderHitrustDT(int order_id)
        {
            try
            {
                return _tabshowdao.GetOderHitrustDT(order_id);
            }
            catch (Exception ex)
            {

                throw new Exception("TabShowMgr-->GetOderHitrustDT-->" + ex.Message, ex);
            }
        }
        #endregion
    }
}
