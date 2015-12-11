/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：OrderMasterMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/22 16:06:53 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using System.Collections;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;
using BLL.gigade.Common;

namespace BLL.gigade.Mgr
{
    public class OrderMasterMgr : IOrderMasterImplMgr
    {
        private IOrderMasterImplDao _orderMasterDao;
        //private OrderMasterDao orderMasterDao;
        private BonusMasterDao bonusMasterDao;
        private BonusMasterMgr _bonusMasterMgr;
        private HappyGoMgr _happyGoMgr;
        //Se_serialDao
        private MySqlDao _mysqlDao;
        private SerialDao _serialDao;
        private OrderReturnStatusDao _orsDao;
        private IParametersrcImplDao _parametersrcDao;
        private string conn;
        public OrderMasterMgr(string connectionStr)
        {
            _orderMasterDao = new OrderMasterDao(connectionStr);
            //orderMasterDao = new OrderMasterDao(connectionStr);
            bonusMasterDao = new BonusMasterDao(connectionStr);
            _bonusMasterMgr = new BonusMasterMgr(connectionStr);
            _happyGoMgr = new HappyGoMgr(connectionStr);
            _serialDao = new SerialDao(connectionStr);
            _mysqlDao = new MySqlDao(connectionStr);
            _orsDao = new OrderReturnStatusDao(connectionStr);
            _parametersrcDao = new ParametersrcDao(connectionStr);
            conn = connectionStr;
        }

        public string Save(BLL.gigade.Model.OrderMaster orderMaster, OrderMasterPattern op = null)
        {
            // edit by xiangwang0413w 20140827
            //不管是訂單匯入還是訂單輸入，order_master.export_flag = 1;order_master.order_invoice = 0

            //判斷前臺輸入的是報廢單還是其他，如果是報廢單則 order_master.export_flag = 4 edit by zhuoqin0830w  2015/03/12
            if (op != null && op.Pattern == 20)
            {
                orderMaster.Export_Flag = 4;
            }
            else
            {
                orderMaster.Export_Flag = 1;
            }
            orderMaster.Order_Invoice = "0";
            return _orderMasterDao.Save(orderMaster);
        }

        public int Delete(int orderId)
        {
            return _orderMasterDao.Delete(orderId);
        }

        public bool SaveOrder(string orderMaster, string orderMasterPattern, string orderPayment, ArrayList orderSlaves, ArrayList orderDetails, ArrayList otherSqls, string bonusMaster, string bonusRecord)
        {
            return _orderMasterDao.SaveOrder(orderMaster, orderMasterPattern, orderPayment, orderSlaves, orderDetails, otherSqls, bonusMaster, bonusRecord);
        }

        public OrderMaster GetPaymentById(uint order_id)
        {
            try
            {
                return _orderMasterDao.GetPaymentById(order_id);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr.GetPaymentById-->" + ex.Message, ex);
            }
        }

        public List<OrderMasterQuery> getOrderSearch(OrderMasterQuery query, string sqladdstr, out int totalCount, string addSerch)
        {
            try
            {
                return _orderMasterDao.getOrderSearch(query, sqladdstr, out totalCount, addSerch);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->getOrderSearch-->" + ex.Message, ex);
            }
        }

        public List<OrderMasterQuery> Export(OrderMasterQuery query, string sqladdstr, out int totalCount)
        {
            return _orderMasterDao.Export(query, sqladdstr, out totalCount);
        }

        public OrderShowMasterQuery GetData(uint orderId)
        {
            try
            {
                return _orderMasterDao.GetData(orderId);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->GetData-->" + ex.Message, ex);
            }
        }

        public string VerifyData(uint orderId)
        {
            try
            {
                return _orderMasterDao.VerifyData(orderId);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->VerifyData-->" + ex.Message, ex);
            }
        }

        public int SaveNoteOrder(OrderShowMasterQuery store)
        {
            try
            {
                return _orderMasterDao.SaveNoteOrder(store);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->VerifyData-->" + ex.Message, ex);
            }
        }

        public int SaveNoteAdmin(OrderShowMasterQuery store)
        {
            try
            {
                return _orderMasterDao.SaveNoteAdmin(store);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->SaveNoteAdmin-->" + ex.Message, ex);
            }
        }

        public int SaveStatus(OrderShowMasterQuery store)
        {
            try
            {
                return _orderMasterDao.SaveStatus(store);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->SaveStatus-->" + ex.Message, ex);
            }
        }

        public UsersListQuery GetUserInfo(uint user_id)
        {
            try
            {
                return _orderMasterDao.GetUserInfo(user_id);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->GetUserInfo-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 根據條件獲取出貨列表
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <param name="totalCount">返回數據總條數</param>
        /// <returns>出貨列表信息</returns>
        public List<OrderMasterQuery> GetShipmentList(OrderMasterQuery query, out int totalCount)
        {
            try
            {
                return _orderMasterDao.GetShipmentList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->GetShipmentList-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查詢購買次數
        /// </summary>
        /// <param name="orderMasterQuery">查詢條件</param>
        /// <returns>購買次數</returns>
        public int GetBuyCount(OrderMasterQuery query)
        {
            try
            {
                return _orderMasterDao.GetBuyCount(query);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->GetBuyCount-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 匯入會計賬款實收時間
        /// </summary>
        /// <param name="dt">匯入入賬時間表</param>
        /// <returns></returns>
        public int OrderMasterImport(List<OrderAccountCollection> oacli)
        {
            try
            {
                ArrayList arryList = new ArrayList();
                foreach (var item in oacli)
                {
                    if (item.order_id != 0)
                    {
                        if (_orderMasterDao.IsNotOrderId(item.order_id))
                        {
                            DataTable DtTemp = IsExistOrderId(item.order_id);
                            if (DtTemp.Rows.Count > 0)
                            {
                                if (string.IsNullOrEmpty(DtTemp.Rows[0]["account_collection_time"].ToString()) ||
                                   string.IsNullOrEmpty(DtTemp.Rows[0]["return_collection_time"].ToString()))
                                {
                                    arryList.Add(_orderMasterDao.UpdateOac(DtTemp, item));
                                }
                            }
                            else
                            {
                                arryList.Add(_orderMasterDao.InsertOac(item));
                            }
                        }
                    }

                }
                if (arryList.Count != 0)
                {
                    if (_mysqlDao.ExcuteSqls(arryList))
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 99999;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->OrderMasterImport-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 會計入賬時間匯出
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable OrderMasterExport(OrderMasterQuery query)
        {
            try
            {
                return _orderMasterDao.OrderMasterExport(query);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->OrderMasterExport-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 異常訂單列表
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <returns></returns>
        public DataTable ArrorOrderList(OrderMasterQuery query, out int totalCount)
        {
            try
            {
                return _orderMasterDao.ArrorOrderList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->ArrorOrderList-->" + ex.Message, ex);
            }
        }

        public DataTable ExportArrorOrderExcel(OrderMasterQuery query)
        {
            try
            {
                DataTable _list = _orderMasterDao.ExportArrorOrderExcel(query);
                _list.Columns.Add("remark");
                _list.Columns.Add("mode_name");
                _list.Columns.Add("combined_mode_name");
                if (_list.Rows.Count > 0)
                {
                    ParametersrcDao _pDao = new ParametersrcDao(conn);
                    List<Parametersrc> parameterList = _pDao.QueryParametersrcByTypes("order_status");
                    foreach (DataRow item in _list.Rows)
                    {
                        var alist = parameterList.Find(m => m.ParameterCode == item["order_status"].ToString());
                        if (alist != null)
                        {
                            item["remark"] = alist.remark.ToString();
                        }
                        switch (item["item_mode"].ToString())
                        {
                            case "0":
                                item["mode_name"] = "單一商品";
                                break;
                            case "1":
                                item["mode_name"] = "父商品";
                                break;
                            case "2":
                                item["mode_name"] = "子商品";
                                break;
                            default:
                                item["mode_name"] = "";
                                break;
                        }
                        //0:一般 1:組合 2:子商品
                        switch (item["combined_mode"].ToString())
                        {
                            case "0":
                                item["combined_mode_name"] = "一般";
                                break;
                            case "1":
                                item["combined_mode_name"] = "組合";
                                break;
                            case "2":
                                item["combined_mode_name"] = "子商品";
                                break;
                            default:
                                item["combined_mode_name"] = "";
                                break;
                        }
                    }
                }
                return _list;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->ExportArrorOrderExcel-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 會計入賬匯出列表
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <returns></returns>
        public DataTable OrderMasterExportList(OrderMasterQuery query, out int totalCount)
        {
            try
            {
                return _orderMasterDao.OrderMasterExportList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->OrderMasterExportList-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 會計入賬匯出數據匯總
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <returns></returns>
        public DataTable OrderMasterHuiZong(OrderMasterQuery query)
        {
            try
            {
                return _orderMasterDao.OrderMasterHuiZong(query);
            }
            catch (Exception ex)
            {

                throw new Exception("OrderMasterMgr-->OrderMasterHuiZong-->" + ex.Message, ex);
            }
        }
        #region 現金,外站,貨到付款對賬

        #region 獲取泛用對賬列表 + List<OrderMasterQuery> GetOBCList(OrderMaster query, out int totalCount)
        public List<OrderMasterQuery> GetOBCList(OrderMasterQuery query, out int totalCount)
        {
            try
            {
                return _orderMasterDao.GetOBCList(query, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("OrderMasterMgr-->GetOBCList-->" + ex.Message, ex);
            }
        }
        #endregion

        #region  賣場store + DataTable GetChannelList(Channel query)
        public List<Channel> GetChannelList(Channel query)
        {
            try
            {
                return _orderMasterDao.GetChannelList(query);
            }
            catch (Exception ex)
            {

                throw new Exception("OrderMasterMgr-->GetChannelList-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 付款方式store + List<Parametersrc> GetPaymentList(OrderMasterQuery query)
        public List<Parametersrc> GetPaymentList(OrderMasterQuery query)
        {
            try
            {
                return _orderMasterDao.GetPaymentList(query);
            }
            catch (Exception ex)
            {

                throw new Exception("OrderMasterMgr-->GetPaymentList-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 修改確認對賬狀態 + int UpdateOrderBilling(OrderMaster query)
        public int UpdateOrderBilling(OrderMasterQuery query)
        {
            try
            {
                return _orderMasterDao.UpdateOrderBilling(query);
            }
            catch (Exception ex)
            {

                throw new Exception("OrderMasterMgr-->UpdateOrderBilling-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 泛用對賬匯出 + DataTable ReportOrderBillingExcel(OrderMasterQuery query)
        public DataTable ReportOrderBillingExcel(OrderMasterQuery query)
        {
            try
            {
                return _orderMasterDao.ReportOrderBillingExcel(query);
            }
            catch (Exception ex)
            {

                throw new Exception("OrderMasterMgr-->ReportOrderBillingExcel-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 泛用對賬匯入對賬檢測 + DataTable CheckedImport(OrderMasterQuery query)
        public DataTable CheckedImport(OrderMasterQuery query)
        {
            try
            {
                return _orderMasterDao.CheckedImport(query);
            }
            catch (Exception ex)
            {

                throw new Exception("OrderMasterMgr-->CheckedImport-->" + ex.Message, ex);
            }
        }
        #endregion

        #endregion

        #region 根據定單編號獲取定單信息
        /// <summary>
        /// 根據定單編號獲取定單信息
        /// </summary>
        /// <param name="order_id">定單編號</param>
        /// <returns>定單信息</returns>
        public OrderMaster GetOrderMasterByOrderId(int order_id)
        {
            try
            {
                return _orderMasterDao.GetOrderMasterByOrderId(order_id);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->GetOrderMasterByOrderId-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 更新定單狀態以開立發票
        /// <summary>
        /// 根據定單編號更新定單狀態以開立發票
        /// </summary>
        /// <param name="order_id">定單編號</param>
        /// <returns>執行結果</returns>
        public int UpdateOrderToOpenInvoice(int order_id)
        {
            try
            {
                return _orderMasterDao.UpdateOrderToOpenInvoice(order_id);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->UpdateOrderToOpenInvoice-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 根據條件查詢要開立發票的訂單信息
        /// <summary>
        /// 根據條件查詢要開立發票的訂單信息
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <returns></returns>
        public OrderMasterQuery GetOrderMasterInvoice(OrderMasterQuery query)
        {
            try
            {
                return _orderMasterDao.GetOrderMasterInvoice(query);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->GetOrderMasterInvoice-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 檢查是否可以進行轉單 +bool isCanModifyForDeliver(int order_id)
        /// <summary>
        /// 檢查是否可以進行轉單
        /// </summary>
        /// <param name="order_id"></param>
        /// <returns></returns>
        public bool isCanModifyForDeliver(int order_id)
        {

            OrderMaster omModel = _orderMasterDao.GetOrderMasterByOrderId4Change(order_id);
            if (null == omModel)
            {
                return false;
            }
            if (0 != omModel.Order_Status && 90 != omModel.Order_Status)
            {
                return false;
            }

            return true;

        }
        #endregion

        public bool ModifyOrderMsaterForDeliver(OrderModifyModel order)
        {
            try
            {
                OrderMaster omModel = _orderMasterDao.GetOrderMasterByOrderId4Change(order.order_id);
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append(@" om.order_status = 2,
os.slave_status=2,
od.detail_status=2,
om.order_date_cancel=0,
om.money_cancel=0,
om.export_flag=1,");

                sbSql.AppendFormat("om.order_date_pay ={0}", Common.CommonFunction.GetPHPTime());

                if (!isCanModifyForDeliver(order.order_id))
                {
                    return false;
                }
                //1、是否寫入對帳
                if (order.isBilling_checked)
                {
                    sbSql.Append(",om.billing_checked=1,");
                    sbSql.AppendFormat("om.money_collect_date={0}", Common.CommonFunction.GetPHPTime());
                }
                //2、紅利折抵
                if (order.deduct_card_bonus != 0)
                {
                    if (omModel.Order_Amount < order.deduct_card_bonus)
                    {
                        return false;
                    }
                    sbSql.AppendFormat(",om.deduct_card_bonus={0},", order.deduct_card_bonus);
                    sbSql.AppendFormat("om.order_amount={0}", omModel.Order_Amount - order.deduct_card_bonus);
                }
                //3、要不要扣除消費者抵用購物金
                if (order.isCash_record_bonus)
                {
                    order.user_id = Convert.ToInt32(omModel.user_id);
                    //order.bonus_num = 
                    order.bonus_num = Convert.ToInt32(omModel.Deduct_Bonus);

                    order.record_note = "強制轉單扣點";
                    order.record_writer = "server";
                    AddBonusRecord(order);//扣除

                }

                #region //4、要不要給hg點或購物金
                if (order.isHGBonus)
                {
                    #region 購物金
                    if (omModel.Accumulated_Bonus > 0)
                    {
                        //訂單取消後,回撥購物金的使用日期限制
                        int nExpire_Day = 90;
                        DateTime nMaster_Start = DateTime.Now.Date;
                        DateTime nMaster_End = DateTime.Now.AddDays(omModel.BonusExpireDay).Date.AddSeconds(-1);
                        // DateTime nMaster_End = DateTime.Now.AddDays(1).Date.AddSeconds(-1);

                        //OrderModifyModel orderModifyModel = new OrderModifyModel();
                        ////$amego_bonus->bonus_master_add($aOrder['user_id'], 30, $aOrder['accumulated_bonus'], $nMaster_Start, $nMaster_End, $aOrder['order_id'], '商品回饋購物金' , 1);
                        //orderModifyModel.user_id =Convert.ToInt32( omModel.User_Id);
                        //orderModifyModel.order_id = Convert.ToInt32(omModel.Order_Id);
                        //orderModifyModel.bonus_num = Convert.ToInt32(omModel.Deduct_Bonus);
                        //orderModifyModel.use_note ="強制轉單扣點";
                        //orderModifyModel.use_writer = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                        // 會員目前可用購物金
                        BonusMasterQuery query = new BonusMasterQuery();
                        query.user_id = omModel.User_Id;
                        query.type_id = 30;
                        query.master_total = Convert.ToUInt32(omModel.Accumulated_Bonus);
                        query.master_balance = omModel.Accumulated_Bonus;
                        query.smaster_start = nMaster_Start;
                        query.smaster_end = nMaster_End;
                        query.smaster_createtime = DateTime.Now;
                        query.smaster_updatedate = DateTime.Now;
                        query.master_writer = string.Format("Writer:{0}", (System.Web.HttpContext.Current.Session["caller"] as Caller).user_username);

                        query.bonus_type = 1;
                        query.master_note = "商品回饋購物金";
                        //Serial ser = _serialDao.GetSerialById(27);
                        //ser.Serial_Value = ser.Serial_Value+1;
                        //_serialDao.Update(ser);
                        //query.master_id = Convert.ToUInt32(ser.Serial_Value);
                        query.master_ipfrom = order.ip_from;
                        List<BonusMasterQuery> queryList = new List<BonusMasterQuery>();
                        queryList.Add(query);
                        _bonusMasterMgr.BonusMasterAdd(queryList);
                        //bonusMasterDao.InsertBonusMaster(query); 
                    }
                    #endregion
                    #region hg點
                    if (omModel.Accumulated_Happygo > 0)
                    {
                        if (_happyGoMgr.GetHGDeductList(omModel.Order_Id).Count > 0)
                        {
                            //有則進行點數累積，無則發信通知
                        }
                        else
                        {
                            MailHelper mailHelper = new MailHelper();
                            string MailTitle = "HG累點失敗";
                            string MailBody = string.Format("{0}無HG資料，無法累點。", omModel.Order_Id);
                            mailHelper.SendToGroup("BonusFailure", MailTitle, MailBody);
                        }

                    }
                    #endregion
                }
                #endregion
                //更新條件
                StringBuilder sql = new StringBuilder();
                sql.Append(@"update order_master om,order_slave os,order_detail od
set ");
                sql.Append(sbSql);
                sql.AppendFormat(@" where om.order_id={0}  AND om.order_id = os.order_id
 AND os.slave_id = od.slave_id ", order.order_id);
                try
                {
                    _orderMasterDao.UpdateOrderMaster(sql.ToString());
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("OrderMasterMgr-->ModifyOrderMsaterForDeliver-->" + ex.Message, ex);

                }
            }
            catch (Exception ex)
            {

                throw new Exception("OrderMasterMgr-->ModifyOrderMsaterForDeliver-->" + ex.Message, ex);
            }

        }

        public void AddBonusRecord(OrderModifyModel model)
        {
            // 會員目前可用購物金
            int userBonus = bonusMasterDao.GetUserBonus(model.user_id);
            if (model.bonus_num < 0)
            {
                model.bonus_num = model.bonus_num * -1;
            }
            if (model.bonus_num > userBonus)
            {
                return;
            }
            List<BonusMasterQuery> bonusQuery = bonusMasterDao.GetBonusMasterListByUser(model.user_id);
            foreach (BonusMasterQuery bonus in bonusQuery)
            {
                if (model.bonus_num > 0)
                {
                    BonusRecord bonusRecord = new BonusRecord();
                    int decuteBonusNum = bonus.master_balance > model.bonus_num ? model.bonus_num : bonus.master_balance;
                    Serial ser = _serialDao.GetSerialById(28);
                    ser.Serial_Value = ser.Serial_Value + 1;
                    _serialDao.Update(ser);
                    bonusRecord.record_id = Convert.ToUInt32(ser.Serial_Value);
                    bonusRecord.master_id = bonus.master_id;
                    bonusRecord.type_id = bonus.type_id;
                    bonusRecord.order_id = Convert.ToUInt32(model.order_id);
                    bonusRecord.record_use = Convert.ToUInt32(decuteBonusNum);
                    bonusRecord.record_note = model.record_note;
                    bonusRecord.record_writer = model.record_writer;
                    bonusRecord.record_ipfrom = model.ip_from;
                    bonusMasterDao.InsertIntoBonusRecord(bonusRecord);
                    bonus.master_balance = bonus.master_balance - decuteBonusNum;//減去扣除的購物金
                    bonusMasterDao.UpdateBonusMasterMasterBalance(bonus);
                    model.bonus_num -= decuteBonusNum;//更新
                }
                else
                {
                    return;
                }
            }
        }




        public bool IsNotOrderId(uint orderId)
        {
            try
            {
                return _orderMasterDao.IsNotOrderId(orderId);
            }
            catch (Exception ex)
            {

                throw new Exception("OrderMasterMgr-->IsNotOrderId-->" + ex.Message, ex);
            }
        }

        public DataTable IsExistOrderId(uint order_id)
        {
            try
            {
                return _orderMasterDao.IsExistOrderId(order_id);
            }
            catch (Exception ex)
            {

                throw new Exception("OrderMasterMgr-->IsExistOrderId-->" + ex.Message, ex);
            }
        }

        public DataTable GetOrderidAndName(int order_id)
        {
            try
            {
                return _orderMasterDao.GetOrderidAndName(order_id);
            }
            catch (Exception ex)
            {

                throw new Exception("OrderMasterMgr-->IsExistOrderId-->" + ex.Message, ex);
            }
        }
        public string UpFirstTime(OrderMasterQuery query)
        {
            try
            {
                DataTable _dt = _orderMasterDao.IsFirstTime(query);
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    return _orderMasterDao.UpFirstTime(query);
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->UpFirstTime-->" + ex.Message, ex);
            }
        }
        public string ChangePayMent(OrderMasterQuery query)
        {
            string json = string.Empty;
            ArrayList arrList = new ArrayList();
            OrderMasterStatusQuery omsQuery = new OrderMasterStatusQuery();
            OrderShowMasterQuery osmQuery = new OrderShowMasterQuery();
            osmQuery = _orderMasterDao.GetData(query.Order_Id);
            try
            {
                if (query.payment == "T_CAT")
                {
                    query.Order_Payment = 8;
                    query.Order_Date_Pay = (uint)CommonFunction.GetPHPTime();
                    query.status_description = query.username + "轉黑貓貨到付款";
                    query.Delivery_Store = (uint)query.delivery;
                    #region modify_order_status
                    arrList.Add(_orderMasterDao.ModifyOrderStatus(query));
                    omsQuery.serial_id = Convert.ToUInt64(_orderMasterDao.GetNextSerial(new Serial { Serial_id = 29 }).Rows[0][0]);
                    omsQuery.order_id = query.Order_Id;
                    omsQuery.status_description = query.status_description;
                    omsQuery.order_status = 2;
                    omsQuery.status_ipfrom = query.Order_Ipfrom;
                    query.user_id = osmQuery.user_id;
                    arrList.Add(_orderMasterDao.OMSRecord(omsQuery));
                    DataTable _dt = _orderMasterDao.IsFirstTime(query);
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        arrList.Add(_orderMasterDao.UpFirstTime(query));
                    }
                    #endregion
                    //加30運費
                    if (osmQuery.order_freight_normal != 0)
                    {
                        query.Order_Freight_Normal = osmQuery.order_freight_normal += 30;
                        query.t_cat_amount += 30;
                    }
                    if (osmQuery.order_freight_low != 0)
                    {
                        query.Order_Freight_Low = osmQuery.order_freight_low += 30;
                        query.t_cat_amount += 30;
                    }
                    if (query.t_cat_amount != 0)
                    {
                        query.Order_Amount = osmQuery.order_amount += query.t_cat_amount;
                    }
                }
                if (query.delivery == 12)
                {
                    if (osmQuery.order_status == 0)
                    {
                        query.Order_Amount = osmQuery.order_amount - (osmQuery.order_freight_normal + osmQuery.order_freight_low);
                        query.Order_Freight_Normal = 0;
                        query.Order_Freight_Low = 0;
                        query.Order_Payment = 9;
                        query.Order_Date_Pay = (uint)CommonFunction.GetPHPTime();
                        query.status_description = query.username + "轉自取，現金";
                        #region modify_order_status
                        arrList.Add(_orderMasterDao.ModifyOrderStatus(query));
                        omsQuery.serial_id = Convert.ToUInt64(_orderMasterDao.GetNextSerial(new Serial { Serial_id = 29 }).Rows[0][0]);
                        omsQuery.order_id = query.Order_Id;
                        omsQuery.status_description = query.status_description;
                        omsQuery.order_status = 2;
                        omsQuery.status_ipfrom = query.Order_Ipfrom;
                        arrList.Add(_orderMasterDao.OMSRecord(omsQuery));
                        query.user_id = osmQuery.user_id;
                        DataTable _dt = _orderMasterDao.IsFirstTime(query);
                        if (_dt != null && _dt.Rows.Count > 0)
                        {
                            arrList.Add(_orderMasterDao.UpFirstTime(query));
                        }
                        #endregion
                        int master_total = osmQuery.accumulated_bonus;
                        if (master_total > 0)
                        {
                            BonusMaster bm = new BonusMaster();
                            bm.master_start = (uint)CommonFunction.GetPHPTime();
                            bm.master_end = bm.master_start + (86400 * 90);
                            bm.master_note = query.Order_Id.ToString();
                            bm.master_writer = "Writer : 轉自取發放購物金";
                            bm.type_id = 30;
                            bm.master_total = (uint)master_total;
                            bm.master_balance = master_total;
                            bm.user_id = osmQuery.user_id;
                            bm.master_createdate = (uint)CommonFunction.GetPHPTime();
                            bm.master_updatedate = bm.master_createdate;
                            bm.master_ipfrom = query.Order_Ipfrom;
                            _orsDao.Bonus_Master_Add(bm);
                        }
                    }
                    else
                    {
                        DataTable _serDt = _orderMasterDao.GetNextSerial(new Serial { Serial_id = 29 });
                        omsQuery.serial_id = Convert.ToUInt64(_serDt.Rows[0][0]);
                        omsQuery.order_id = query.Order_Id;
                        omsQuery.status_description = query.username + "轉自取";
                        omsQuery.order_status = osmQuery.order_status;
                        omsQuery.status_ipfrom = query.Order_Ipfrom;
                        arrList.Add(_orderMasterDao.OMSRecord(omsQuery));
                    }
                    arrList.Add(_orderMasterDao.UpDeliveryMaster(query.Order_Id, query.Delivery_Store));
                }
                //更新order_master
                arrList.Add(_orderMasterDao.UpdateOrderMaster(query, osmQuery));
                if (_mysqlDao.ExcuteSqlsThrowException(arrList))
                {
                    json = "{success:true}";
                }
                else
                {
                    json = "{success:false}";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->ChangePayMent-->" + ex.Message, ex);
            }
            return json;
        }

        public ArrayList ModifyOrderStatus(OrderMasterQuery query, OrderMasterStatusQuery omsQuery)
        {
            ArrayList arrList = new ArrayList();
            try
            {
                arrList.Add(_orderMasterDao.ModifyOrderStatus(query));
                omsQuery.serial_id = Convert.ToUInt64(_orderMasterDao.GetNextSerial(new Serial { Serial_id = 29 }).Rows[0][0]);
                omsQuery.order_id = query.Order_Id;
                omsQuery.status_description = query.status_description;
                omsQuery.order_status = query.Order_Status;
                arrList.Add(_orderMasterDao.OMSRecord(omsQuery));
                return arrList;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->ModifyOrderStatus-->" + ex.Message, ex);
            }

        }

        public bool IsVendorDeliver(uint order_id)
        {
            //  string json = string.Empty;
            try
            {
                DataTable _dt = _orderMasterDao.IsVendorDeliver(order_id);
                if (Convert.ToInt32(_dt.Rows[0][0]) == 0)
                {
                    return false;
                }
                else//是自出，不能轉自取
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->IsVendorDeliver-->" + ex.Message, ex);
            }
        }

        public string ModifyDeliveryData(OrderMasterQuery om)
        {
            string json = string.Empty;
            ArrayList arrList = new ArrayList();
            OrderShowMasterQuery oms = new OrderShowMasterQuery();
            OrderShowMasterQuery newOms = new OrderShowMasterQuery();
            try
            {
                if (IsSendProduct(om))//true可以變更 false不可變更。
                {
                    json=   ChangeDeliverData(oms, newOms, om);
                }
                else
                {
                    json = "{success:true,msg:'1'}";
                }
                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->ModifyDeliveryData-->" + ex.Message, ex);
            }

        }

        public bool IsSendProduct(OrderMasterQuery om)
        {
            bool result = false;//已出貨，不能修改  為true時可以變更
            int i = 0;
            DataTable _dt = _orderMasterDao.CheckDeliveryStatus(om);
            if (_dt != null)//有數據，看看裏面的status是不是都是==0？執行變更：已出貨不能變更
            {
                foreach (DataRow item in _dt.Rows)
                {
                    if (Convert.ToInt32(item["delivery_status"]) == 0)
                    {
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (i != _dt.Rows.Count)//不全為0，有已出貨的不能變更
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            else
            {
                result = true;
            }

            return result;
        }

        public string ChangeDeliverData(OrderShowMasterQuery oms, OrderShowMasterQuery newOms, OrderMasterQuery om)
        {
            string json = string.Empty;
            ArrayList arrList = new ArrayList();
            Serial serial = new Serial();
            oms.status_description = "Writer:" + "(" + om.user_id + ")" + om.user_name + "," + "\r\n" + "異動收貨資訊";
            oms.status_ipfrom = om.Order_Ipfrom;
            oms.StatusCreateDate = DateTime.Now;
            oms.order_id = om.Order_Id;
            om.mdd = true;
            newOms = _orderMasterDao.GetData(om.Order_Id);
            string note_order = newOms.note_order;
            if (note_order == "")
            {
                om.note_order = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "更改收件人資訊";
            }
            else
            {
                om.note_order = note_order + '/' + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "更改收件人資訊";
            }
            oms.order_status = newOms.order_status;
            arrList.Add(_orderMasterDao.UpOrderMaster(om));//更新order_master
            arrList.Add(_orderMasterDao.UpdateDM(om));//更新deliver_master
            serial = _serialDao.GetSerialById(29);
            oms.serial_id = Convert.ToInt32(serial.Serial_Value) + 1;
            serial.Serial_Value = Convert.ToUInt64(oms.serial_id);
            arrList.Add(_serialDao.UpdateAutoIncreament(serial));//更新serial表
            arrList.Add(_orderMasterDao.InsertOrderMasterStatus(oms));//插入order_master_status表
            if (_mysqlDao.ExcuteSqlsThrowException(arrList))
            {
                json = "{success:true}";
                #region 發送郵件
                MailHelper mail = new MailHelper();
                string body = "付款單號 : " + om.Order_Id + " 已更改收貨人資訊，請重新檢視出貨單<br/>更改時間：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "<br/><br/>以上為系統通知信請勿直接回覆，謝謝！               ";
                mail.SendMailAction("zhaopeng0205j@gimg.tw", "收貨人資訊變更", body);
                #endregion
            }
            else
            {
                json = "{success:false}";
            }

            return json;
        }

        public int VerifySession(uint user_id)
        {
            try
            {
                DataTable _dt = _orderMasterDao.VerifySession(user_id);
                if (_dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(_dt.Rows[0][0]);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->VerifySession-->" + ex.Message, ex);
            }
        }

        public DataTable OrderSerchExport(OrderMasterQuery query)
        {
            DataTable _dt = new DataTable();
            try
            {
                _dt = _orderMasterDao.OrderSerchExport(query);
                return _dt;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->OrderSerchExport-->" + ex.Message, ex);
            }
        }

        public string   GetPara(string type,int order_status)
        {
            string data = string.Empty;
            try
            {
                DataTable _dt = _orderMasterDao.GetPara(type,order_status);
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    data = _dt.Rows[0][0].ToString();
                }
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->GetParaByOrderStatus-->" + ex.Message, ex);
            }
        }

        public string GetParaByPayment(int payment)
        {
            string data = string.Empty;
            try
            {
                DataTable _dt = _orderMasterDao.GetParaByPayment(payment);
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    data = _dt.Rows[0][0].ToString();
                }
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->GetParaByPayment-->" + ex.Message, ex);
            }
        }

        public DataTable GetOrderFreight(uint order_id)
        {
            try
            {
                DataTable _dt = _orderMasterDao.GetOrderFreight(order_id);
                return _dt;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterMgr-->GetOrderFreight-->" + ex.Message, ex);
            }
        }
        //類別訂單明細匯出
        public DataTable CagegoryDetialExportInfo(OrderDetailQuery query)
        {
            DataTable detial=new DataTable();
            DataTable all = new DataTable();
            try
            {
                DataTable dt_ids = _orderMasterDao.GetOrderDetialExportOrderid(query);
                foreach (DataRow dr1 in dt_ids.Rows)
                {
                    if (!string.IsNullOrEmpty(dr1[0].ToString()))
                    {
                        query.Order_Id = Convert.ToUInt32(dr1[0]);
                        detial = _orderMasterDao.CagegoryDetialExport(query);

                        if (detial != null && detial.Rows.Count > 0)
                        {
                            List<Parametersrc> parameterList = _parametersrcDao.SearchParameters("payment", "order_status", "product_mode", "Deliver_Store");
                            foreach (DataRow dr in detial.Rows)
                            {
                                var alist = parameterList.Find(m => m.ParameterType == "payment" && m.ParameterCode == dr["order_payment"].ToString());
                                var blist = parameterList.Find(m => m.ParameterType == "order_status" && m.ParameterCode == dr["order_status"].ToString());
                                var clist = parameterList.Find(m => m.ParameterType == "order_status" && m.ParameterCode == dr["slave_status"].ToString());
                                var dlist = parameterList.Find(m => m.ParameterType == "product_mode" && m.ParameterCode == dr["product_mode"].ToString());
                                var delist = parameterList.Find(m => m.ParameterType == "Deliver_Store" && m.ParameterCode == dr["delivery_store"].ToString());

                                if (alist != null)
                                {
                                    dr["payment_name"] = alist.parameterName;
                                }
                                if (blist != null)
                                {
                                    dr["order_status_name"] = blist.remark;
                                }
                                if (clist != null)
                                {
                                    dr["slave_status_name"] = clist.remark;
                                }
                                if (dlist != null)
                                {
                                    dr["product_mode_name"] = dlist.remark;
                                }
                                if (delist != null)
                                {
                                    dr["deliver_name"] = delist.parameterName;
                                }
                                if (dr["order_createdate"] != null)
                                {
                                    dr["order_createdate_format"] = CommonFunction.DateTimeToString(CommonFunction.GetNetTime(Convert.ToInt32(dr["order_createdate"].ToString())));
                                }
                                if (dr["slave_date_close"] != null)
                                {
                                    dr["slave_date_close_format"] = CommonFunction.DateTimeToString(CommonFunction.GetNetTime(Convert.ToInt32(dr["slave_date_close"].ToString())));
                                }
                                if (dr["single_money"] != null && dr["buy_num"] != null)
                                {
                                    dr["amount"] = Convert.ToInt32(dr["single_money"].ToString()) * Convert.ToInt32(dr["buy_num"]);
                                }
                                if (dr["single_cost"] != null && dr["buy_num"] != null)
                                {
                                    dr["cost_amount"] = Convert.ToInt32(dr["single_cost"].ToString()) * Convert.ToInt32(dr["buy_num"]);
                                }
                            }

                        }
                        else
                        {
                            continue;
                        }
                        if (all.Rows.Count == 0)
                        {
                            all = detial;
                        }
                        else
                        {
                            all.Merge(detial, true);
                        }
                    }
                }
                return all;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMgr-->CagegoryDetialExportInfo-->" + ex.Message, ex);
            }
        }
        //public DataTable CagegoryDetialExportInfo(OrderDetailQuery query)
        //{
        //    try
        //    {
        //        DataTable dt = _orderMasterDao.CagegoryDetialExport(query);
        //        if (dt != null && dt.Rows.Count > 0)
        //        {
        //            List<Parametersrc> parameterList = _parametersrcDao.SearchParameters("payment", "order_status", "product_mode", "Deliver_Store");
        //            foreach (DataRow dr in dt.Rows)
        //            {
        //                var alist = parameterList.Find(m => m.ParameterType == "payment" && m.ParameterCode == dr["order_payment"].ToString());
        //                var blist = parameterList.Find(m => m.ParameterType == "order_status" && m.ParameterCode == dr["order_status"].ToString());
        //                var clist = parameterList.Find(m => m.ParameterType == "order_status" && m.ParameterCode == dr["slave_status"].ToString());
        //                var dlist = parameterList.Find(m => m.ParameterType == "product_mode" && m.ParameterCode == dr["product_mode"].ToString());
        //                var delist = parameterList.Find(m => m.ParameterType == "Deliver_Store" && m.ParameterCode == dr["delivery_store"].ToString());

        //                if (alist != null)
        //                {
        //                    dr["payment_name"] = alist.parameterName;
        //                }
        //                if (blist != null)
        //                {
        //                    dr["order_status_name"] = blist.remark;
        //                }
        //                if (clist != null)
        //                {
        //                    dr["slave_status_name"] = clist.remark;
        //                }
        //                if (dlist != null)
        //                {
        //                    dr["product_mode_name"] = dlist.remark;
        //                }
        //                if (delist != null)
        //                {
        //                    dr["deliver_name"] = delist.parameterName;
        //                }
        //                if (dr["order_createdate"] != null)
        //                {
        //                    dr["order_createdate_format"] = CommonFunction.DateTimeToString(CommonFunction.GetNetTime(Convert.ToInt32(dr["order_createdate"].ToString())));
        //                }
        //                if (dr["slave_date_close"] != null)
        //                {
        //                    dr["slave_date_close_format"] = CommonFunction.DateTimeToString(CommonFunction.GetNetTime(Convert.ToInt32(dr["slave_date_close"].ToString())));
        //                }
        //                if (dr["single_money"] != null && dr["buy_num"] != null)
        //                {
        //                    dr["amount"] = Convert.ToInt32(dr["single_money"].ToString()) * Convert.ToInt32(dr["buy_num"]);
        //                }
        //                if (dr["single_cost"] != null && dr["buy_num"] != null)
        //                {
        //                    dr["cost_amount"] = Convert.ToInt32(dr["single_cost"].ToString()) * Convert.ToInt32(dr["buy_num"]);
        //                }
        //            }
        //        }
        //        return dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("OrderMgr-->CagegoryDetialExportInfo-->" + ex.Message, ex);
        //    }
        //}

        //訂單明細匯出
        public DataTable OrderDetialExportInfo(OrderDetailQuery query)
        {
            try
            {
                DataTable dt_ids = _orderMasterDao.GetOrderDetialExportOrderid(query);
                DataTable detial=new DataTable();
                DataTable all = new DataTable();
                foreach (DataRow dr in dt_ids.Rows)
                {
                    if (!string.IsNullOrEmpty(dr[0].ToString()))
                    {
                        int order_id = Convert.ToInt32(dr[0]);
                        detial = _orderMasterDao.OrderDetialExportInfo(order_id);
                        if (detial != null && detial.Rows.Count > 0)
                        {
                            List<Parametersrc> parameterList = _parametersrcDao.SearchParameters("payment", "order_status", "product_mode");
                            foreach (DataRow dr_t in detial.Rows)
                            {
                                var alist = parameterList.Find(m => m.ParameterType == "payment" && m.ParameterCode == dr_t["order_payment"].ToString());
                                var blist = parameterList.Find(m => m.ParameterType == "order_status" && m.ParameterCode == dr_t["order_status"].ToString());
                                var clist = parameterList.Find(m => m.ParameterType == "order_status" && m.ParameterCode == dr_t["slave_status"].ToString());
                                var dlist = parameterList.Find(m => m.ParameterType == "product_mode" && m.ParameterCode == dr_t["product_mode"].ToString());
                                if (alist != null)
                                {
                                    dr_t["payment_name"] = alist.parameterName;
                                }
                                if (blist != null)
                                {
                                    dr_t["order_status_name"] = blist.remark;
                                }
                                if (clist != null)
                                {
                                    dr_t["slave_status_name"] = clist.remark;
                                }
                                if (dlist != null)
                                {
                                    dr_t["product_mode_name"] = dlist.remark;
                                }
                                if (dr_t["order_createdate"] != null)
                                {
                                    dr_t["order_createdate_format"] = CommonFunction.DateTimeToString(CommonFunction.GetNetTime(Convert.ToInt32(dr_t["order_createdate"].ToString())));
                                }
                                if (dr_t["slave_date_close"] != null)
                                {
                                    dr_t["slave_date_close_format"] = CommonFunction.DateTimeToString(CommonFunction.GetNetTime(Convert.ToInt32(dr_t["slave_date_close"].ToString())));
                                }
                                if (dr_t["single_money"] != null && dr_t["buy_num"] != null)
                                {
                                    dr_t["amount"] = Convert.ToInt32(dr_t["single_money"].ToString()) * Convert.ToInt32(dr_t["buy_num"]);
                                }
                                if (dr_t["single_cost"] != null && dr_t["buy_num"] != null)
                                {
                                    dr_t["cost_amount"] = Convert.ToInt32(dr_t["single_cost"].ToString()) * Convert.ToInt32(dr_t["buy_num"]);
                                }
                                if (dr_t["parent_id"] != null && dr_t["product_id"] != null)
                                {
                                    if (dr_t["item_mode"].ToString() == "0")//單一商品編號是pi.product_id
                                    {
                                        dr_t["product_id"] = Convert.ToInt32(dr_t["product_id"].ToString());
                                    }
                                    else//組合商品編號是od.parent_id
                                    {
                                        dr_t["product_id"] = Convert.ToInt32(dr_t["parent_id"].ToString());
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                    if (all.Rows.Count == 0)
                    {
                        all = detial;
                    }
                    else
                    {
                        all.Merge(detial, true);
                    }
                }
                return all;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMgr-->OrderDetialExportInfo-->" + ex.Message, ex);
            }
        }

        public DataTable ExcelTitle()
        {
            try
            {
                DataTable dtNew = new DataTable();
                string newExcelName = string.Empty;
                dtNew.Columns.Add("category_id", typeof(int));
                dtNew.Columns.Add("user_name", typeof(String));
                dtNew.Columns.Add("order_createdate", typeof(String));
                dtNew.Columns.Add("order_id", typeof(int));
                dtNew.Columns.Add("order_payment", typeof(String));
                dtNew.Columns.Add("order_amount", typeof(int));
                dtNew.Columns.Add("order_status", typeof(String));
                dtNew.Columns.Add("item_mode", typeof(String));
                dtNew.Columns.Add("item_id", typeof(int));
                dtNew.Columns.Add("slave_status", typeof(String));
                dtNew.Columns.Add("vendor_name_simple", typeof(String));
                dtNew.Columns.Add("vendor_code", typeof(String));
                dtNew.Columns.Add("product_name", typeof(String));
                dtNew.Columns.Add("buy_num", typeof(int));
                dtNew.Columns.Add("single_money", typeof(int));
                dtNew.Columns.Add("deduct_bonus", typeof(int));
                dtNew.Columns.Add("deduct_welfare", typeof(int));
                dtNew.Columns.Add("single_money*buy_num", typeof(int));
                dtNew.Columns.Add("single_cost", typeof(int));
                dtNew.Columns.Add("bag_check_money", typeof(int));
                dtNew.Columns.Add("single_cost*od.buy_num", typeof(int));
                dtNew.Columns.Add("slave_date_close", typeof(String));
                dtNew.Columns.Add("pm", typeof(String));
                dtNew.Columns.Add("ID", typeof(String));
                dtNew.Columns.Add("group_name", typeof(String));
                dtNew.Columns.Add("product_mode", typeof(String));
                dtNew.Columns.Add("delivery_name", typeof(String));
                dtNew.Columns.Add("delivery_address", typeof(String)); 
                return dtNew;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMgr-->CagegoryDetialExportInfo-->" + ex.Message, ex);
            }
        }


        public DataTable GetInvoiceData(uint order_id)
        {
            try
            {
                return _orderMasterDao.GetInvoiceData(order_id);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMgr-->GetInvoiceData-->" + ex.Message, ex);
            }
        }

        public DataTable GetInvoice(uint order_id, uint pid)
        {
            try
            {
                return _orderMasterDao.GetInvoice(order_id, pid);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMgr-->GetInvoice-->" + ex.Message, ex);
            }
        }
    }
}
