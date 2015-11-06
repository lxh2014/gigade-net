using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using BLL.gigade.Model.Custom;
using BLL.gigade.Common;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;


namespace BLL.gigade.Mgr
{
    public class DeliverMasterMgr : IDeliverMasterImplMgr
    {
        private IDBAccess _access;
        string xmlPath = System.Configuration.ConfigurationManager.AppSettings["SiteConfig"];//郵件服務器的設置
        private IDeliverMasterImplDao _ideliver;
        IScheduleRelationImplMgr _srMgr;
        string connection = string.Empty;
        public DeliverMasterMgr(string connectionString)
        {
            connection = connectionString;
            _ideliver = new DeliverMasterDao(connectionString);
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);

        }
        public List<DeliverMasterQuery> GetdeliverList(DeliverMasterQuery deliver, out int totalCount)
        {
            try
            {
                return _ideliver.GetdeliverList(deliver, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterMgr-->GetdeliverList-->" + ex.Message, ex);
            }
        }
        public List<DeliverMasterQuery> DeliverVerifyList(DeliverMaster deliver, out int totalCount)
        {
            try
            {
                return _ideliver.DeliverVerifyList(deliver, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterMgr-->DeliverVerifyList-->" + ex.Message, ex);
            }
        }
        public List<DeliverMasterQuery> JudgeOrdid(DeliverMaster dm)
        {
            try
            {
                return _ideliver.JudgeOrdid(dm);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterMgr-->JudgeOrdid-->" + ex.Message, ex);
            }
        }


        public DataTable GetMessageByDeliveryCode(DeliverMasterQuery dmQuery)
        {
            try
            {
                return _ideliver.GetMessageByDeliveryCode(dmQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterMgr-->GetMessageByDeliveryCode-->" + ex.Message, ex);
            }
        }


        public int Updatedeliveryfreightcost(StringBuilder str)
        {
            try
            {
                return _ideliver.Updatedeliveryfreightcost(str);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterMgr-->Updatedeliveryfreightcost-->" + ex.Message, ex);
            }
        }
        public List<DeliverMasterQuery> GetdeliverListCSV(DeliverMasterQuery deliver)
        {
            try
            {
                return _ideliver.GetdeliverListCSV(deliver);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterMgr-->GetdeliverListCSV-->" + ex.Message, ex);
            }
        }


        public DataTable GetReportManagementList(DeliverMasterQuery deliver, out int totalCount)
        {
            try
            {
                DataTable _newDt = new DataTable();
                DataTable _dt = _ideliver.GetReportManagementList(deliver, out totalCount);
                _newDt.Columns.Add("delivery_code", typeof(String));
                _newDt.Columns.Add("deliver_id", typeof(String));
                _newDt.Columns.Add("order_id", typeof(String));
                _newDt.Columns.Add("order_date", typeof(String));
                _newDt.Columns.Add("order_status", typeof(String));
                _newDt.Columns.Add("order_payment", typeof(String));
                _newDt.Columns.Add("delivery_store", typeof(String));
                _newDt.Columns.Add("logisticsTypes", typeof(String));
                _newDt.Columns.Add("delivery_status", typeof(String));
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    DataRow _newDtRow = _newDt.NewRow();
                    _newDtRow = _newDt.NewRow();
                    _newDtRow[0] = _dt.Rows[i]["delivery_code"];
                    _newDtRow[1] = _dt.Rows[i]["deliver_id"];
                    _newDtRow[2] = _dt.Rows[i]["order_id"];
                    _newDtRow[3] = _dt.Rows[i]["order_date"];
                    DataTable _dtOrderStatus = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='order_status' and parameterCode='{0}'", _dt.Rows[i]["order_status"]));//order_status
                    if (_dtOrderStatus.Rows.Count > 0)
                    {
                        string name = _dtOrderStatus.Rows[0]["remark"].ToString();
                        _newDtRow[4] = name;
                    }
                    else
                    {
                        _newDtRow[4] = "";
                    }
                    DataTable _dtOrderPayment = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='payment' and parameterCode='{0}'", _dt.Rows[i]["order_payment"]));//order_payment
                    if (_dtOrderPayment.Rows.Count > 0)
                    {
                        _newDtRow[5] = _dtOrderPayment.Rows[0]["parameterName"];
                    }
                    else
                    {
                        _newDtRow[5] = "";
                    }
                    DataTable _dtDeliveryStore = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='Deliver_Store' and parameterCode='{0}'", _dt.Rows[i]["delivery_store"]));//delivery_store
                    if (_dtDeliveryStore.Rows.Count > 0)
                    {
                        _newDtRow[6] = _dtDeliveryStore.Rows[0]["parameterName"];
                    }
                    else
                    {
                        _newDtRow[6] = "";
                    }
                    DataTable _dtLogisticsType = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='logistics_type' and parameterCode='{0}'", _dt.Rows[i]["logisticsTypes"]));//logisticsType
                    if (_dtLogisticsType.Rows.Count > 0)
                    {
                        _newDtRow[7] = _dtLogisticsType.Rows[0]["parameterName"];
                    }
                    else
                    {
                        _newDtRow[7] = "";
                    }
                    DataTable _dtDeliveryStatus = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='delivery_status' and parameterCode='{0}'", _dt.Rows[i]["delivery_status"]));
                    if (_dtDeliveryStatus.Rows.Count > 0)
                    {
                        _newDtRow[8] = _dtDeliveryStatus.Rows[0]["parameterName"];
                    }
                    else
                    {
                        _newDtRow[8] = "";
                    }
                    _newDt.Rows.Add(_newDtRow);
                }
                //return _ideliver.GetReportManagementList(deliver, out totalCount);

                return _newDt;
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterMgr-->GetReportManagementList-->" + ex.Message, ex);
            }
        }


        public DataTable ReportManagementExcelList(DeliverMasterQuery deliver)
        {
            try
            {
                DataTable _newDt = new DataTable();
                DataTable _dt = _ideliver.ReportManagementExcelList(deliver);
                _newDt.Columns.Add("delivery_code", typeof(String));
                _newDt.Columns.Add("deliver_id", typeof(String));
                _newDt.Columns.Add("order_id", typeof(String));
                _newDt.Columns.Add("order_date", typeof(String));
                _newDt.Columns.Add("order_status", typeof(String));
                _newDt.Columns.Add("order_payment", typeof(String));
                _newDt.Columns.Add("delivery_store", typeof(String));
                _newDt.Columns.Add("logisticsTypes", typeof(String));
                _newDt.Columns.Add("delivery_status", typeof(String));
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                   
                    DataRow _newDtRow = _newDt.NewRow();
                    _newDtRow = _newDt.NewRow();
                    _newDtRow[0] = _dt.Rows[i]["delivery_code"];
                    _newDtRow[1] = _dt.Rows[i]["deliver_id"];
                    _newDtRow[2] = _dt.Rows[i]["order_id"];
                    _newDtRow[3] = _dt.Rows[i]["order_date"];
                    DataTable _dtOrderStatus = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='order_status' and parameterCode='{0}'", _dt.Rows[i]["order_status"]));//order_status
                    if (_dtOrderStatus.Rows.Count > 0)
                    {
                        string name = _dtOrderStatus.Rows[0]["remark"].ToString();
                        _newDtRow[4] = name;
                    }
                    else
                    {
                        _newDtRow[4] = "";
                    }
                    DataTable _dtOrderPayment = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='payment' and parameterCode='{0}'", _dt.Rows[i]["order_payment"]));//order_payment
                    if (_dtOrderPayment.Rows.Count > 0)
                    {
                        _newDtRow[5] = _dtOrderPayment.Rows[0]["parameterName"];
                    }
                    else
                    {
                        _newDtRow[5] = "";
                    }
                    DataTable _dtDeliveryStore = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='Deliver_Store' and parameterCode='{0}'", _dt.Rows[i]["delivery_store"]));//delivery_store
                    if (_dtDeliveryStore.Rows.Count > 0)
                    {
                        _newDtRow[6] = _dtDeliveryStore.Rows[0]["parameterName"];
                    }
                    else
                    {
                        _newDtRow[6] = "";
                    }
                    DataTable _dtLogisticsType = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='logistics_type' and parameterCode='{0}'", _dt.Rows[i]["logisticsTypes"]));//logisticsType
                    if (_dtLogisticsType.Rows.Count > 0)
                    {
                        _newDtRow[7] = _dtLogisticsType.Rows[0]["parameterName"];
                    }
                    else
                    {
                        _newDtRow[7] = "";
                    }
                    DataTable _dtDeliveryStatus = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='delivery_status' and parameterCode='{0}'", _dt.Rows[i]["delivery_status"]));
                    if (_dtDeliveryStatus.Rows.Count > 0)
                    {
                        _newDtRow[8] = _dtDeliveryStatus.Rows[0]["parameterName"];
                    }
                    else
                    {
                        _newDtRow[8] = "";
                    }
                    _newDt.Rows.Add(_newDtRow);
                }
                //return _ideliver.GetReportManagementList(deliver, out totalCount);

                return _newDt;
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterMgr-->ReportManagementExcelList-->" + ex.Message, ex);
            }
        }

        public List<DeliverMasterQuery> GetTicketDetailList(DeliverMasterQuery query, out int totalCount)
        {
            try
            {
                return _ideliver.GetTicketDetailList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterMgr-->GetTicketDetailList-->" + ex.Message, ex);
            }
        }
        public DataTable GetDelayDeliverList(DeliverMasterQuery query, out int totalCount)
        {
            try
            {
                return _ideliver.GetDelayDeliverList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterMgr-->GetDelayDeliverList-->" + ex.Message, ex);
            }
        }

        public DataTable GetDeliveryMsgList(DeliverMasterQuery deliver, out int totalCount)
        {
            try
            {

                DataTable _newDt = new DataTable();
                if (deliver.serch_msg == 1)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(deliver.serch_where.Trim()))
                        {
                            deliver.serch_where = string.Empty;
                        }
                        else
                        {
                            int i = Convert.ToInt32(deliver.serch_where);
                        }
                    }
                    catch (Exception ex)
                    {
                        deliver.serch_where = "0";
                    }
                }
                else if (deliver.serch_msg == 2)
                {
                    if (deliver.serch_where.Length > 0)
                    {
                        DataTable _myDt = _ideliver.GetVnndorId(deliver.serch_where);
                        if (_myDt.Rows.Count > 0)
                        {
                            deliver.serch_where = string.Empty;
                            for (int i = 0; i < _myDt.Rows.Count; i++)
                            {
                                deliver.serch_where = deliver.serch_where + _myDt.Rows[i]["vendor_id"] + ",";
                            }
                            int length = deliver.serch_where.Length;
                            deliver.serch_where = deliver.serch_where.Substring(0, length - 1);
                        }
                        else
                        {
                            deliver.serch_where = "0";
                        }
                    }
                    else
                    {
                        deliver.serch_where = string.Empty;
                    }
                }
                else
                {
                    deliver.serch_where = string.Empty;
                }

                DataTable _dt = _ideliver.GetDeliveryMsgList(deliver, out totalCount);
                _newDt.Columns.Add("delivery_code", typeof(String));
                _newDt.Columns.Add("deliver_id", typeof(String));
                _newDt.Columns.Add("order_id", typeof(String));
                _newDt.Columns.Add("order_date", typeof(String));
                _newDt.Columns.Add("order_status", typeof(String));
                _newDt.Columns.Add("order_payment", typeof(String));
                _newDt.Columns.Add("delivery_store", typeof(String));
                _newDt.Columns.Add("logisticsTypes", typeof(String));
                _newDt.Columns.Add("delivery_status", typeof(String));
                _newDt.Columns.Add("vendor_name_simple", typeof(String));
                _newDt.Columns.Add("freight_set", typeof(String));
                _newDt.Columns.Add("delivery_freight_cost", typeof(String));
                _newDt.Columns.Add("delivery_date", typeof(String));
                _newDt.Columns.Add("arrival_date", typeof(String));
                _newDt.Columns.Add("estimated_delivery_date", typeof(String));//14
                _newDt.Columns.Add("estimated_arrival_date", typeof(String));
                _newDt.Columns.Add("estimated_arrival_period", typeof(String));
                _newDt.Columns.Add("delivery_name", typeof(String));
                _newDt.Columns.Add("product_name", typeof(String));
                _newDt.Columns.Add("buy_num", typeof(String));
                _newDt.Columns.Add("overdue_day", typeof(String));
                _newDt.Columns.Add("item_id", typeof(String));
                _newDt.Columns.Add("product_mode", typeof(String));
                _newDt.Columns.Add("slave_status", typeof(String));
                _newDt.Columns.Add("detail_status", typeof(String));
                _newDt.Columns.Add("dvendor_name_simple", typeof(String));
                _newDt.Columns.Add("deliver_master_date", typeof(String));
                 _newDt.Columns.Add("note_order", typeof(String));
                _newDt.Columns.Add("note_admin", typeof(String));
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    int product_id=Convert.ToInt32(_dt.Rows[i]["product_id"]);
                    uint detail_id = Convert.ToUInt32(_dt.Rows[i]["detail_id"]);
                    int item_id = Convert.ToInt32(_dt.Rows[i]["item_id"]);
                    string created = _dt.Rows[i]["created"].ToString();
                    DataRow _newDtRow = _newDt.NewRow();
                    _newDtRow = _newDt.NewRow();
                    _newDtRow[0] = _dt.Rows[i]["delivery_code"];
                    _newDtRow[1] = _dt.Rows[i]["deliver_id"];
                    _newDtRow[2] = _dt.Rows[i]["order_id"];
                    _newDtRow[3] = _dt.Rows[i]["order_date"];
                    DataTable _dtOrderStatus = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='order_status' and parameterCode='{0}'", _dt.Rows[i]["order_status"]));//order_status
                    if (_dtOrderStatus.Rows.Count > 0)
                    {
                        string name = _dtOrderStatus.Rows[0]["remark"].ToString();
                        _newDtRow[4] = name;
                    }
                    else
                    {
                        _newDtRow[4] = "";
                    }
                    DataTable _dtOrderPayment = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='payment' and parameterCode='{0}'", _dt.Rows[i]["order_payment"]));//order_payment
                    if (_dtOrderPayment.Rows.Count > 0)
                    {
                        _newDtRow[5] = _dtOrderPayment.Rows[0]["parameterName"];
                    }
                    else
                    {
                        _newDtRow[5] = "";
                    }
                    DataTable _dtDeliveryStore = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='Deliver_Store' and parameterCode='{0}'", _dt.Rows[i]["delivery_store"]));//delivery_store
                    if (_dtDeliveryStore.Rows.Count > 0)
                    {
                        _newDtRow[6] = _dtDeliveryStore.Rows[0]["parameterName"];
                    }
                    else
                    {
                        _newDtRow[6] = "";
                    }
                    DataTable _dtLogisticsType = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='logistics_type' and parameterCode='{0}'", _dt.Rows[i]["logisticsTypes"]));//logisticsType
                    if (_dtLogisticsType.Rows.Count > 0)
                    {
                        _newDtRow[7] = _dtLogisticsType.Rows[0]["parameterName"];
                    }
                    else
                    {
                        _newDtRow[7] = "";
                    }
                    DataTable _dtDeliveryStatus = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='delivery_status' and parameterCode='{0}'", _dt.Rows[i]["delivery_status"]));
                    if (_dtDeliveryStatus.Rows.Count > 0)
                    {
                        _newDtRow[8] = _dtDeliveryStatus.Rows[0]["parameterName"];
                    }
                    else
                    {
                        _newDtRow[8] = "";
                    }
                    _newDtRow[9] = _dt.Rows[i]["vendor_name_simple"];
                    DataTable _dtProductFreight = _access.getDataTable(string.Format(@"SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='product_freight' and parameterCode='{0}';", _dt.Rows[i]["freight_set"]));
                    if (_dtProductFreight.Rows.Count > 0)
                    {
                        _newDtRow[10] = _dtProductFreight.Rows[0]["parameterName"];
                    }
                    else
                    {
                        _newDtRow[10] = "";
                    }
                    _newDtRow[11] = _dt.Rows[i]["delivery_freight_cost"];
                    _newDtRow[12] = _dt.Rows[i]["delivery_date"]; 
                    _newDtRow[13] = _dt.Rows[i]["arrival_date"]; 
                    _newDtRow[14] = _dt.Rows[i]["estimated_delivery_date"];//預計出貨時間 
                  
                    //_newDtRow[14] = ExpectTime("product", product_id);
                    //_newDtRow[15] = _dt.Rows[i]["estimated_arrival_date"];//預計到貨時間 
                   // _newDtRow[15] = ArriveTime(item_id, created);
                    _newDtRow[15] = _dt.Rows[i]["delivery_date_str"];//預計到貨時間deliver_org_days delivery_date_str
                    _newDtRow[16] = _dt.Rows[i]["estimated_arrival_period"]; 
                    _newDtRow[17] = _dt.Rows[i]["delivery_name"];
                    _newDtRow[18] = _dt.Rows[i]["product_name"];
                    _newDtRow[19] = _dt.Rows[i]["buy_num"];
                    #region 計算距離壓單日：(出貨時間-付款單成立日期+1)-4，出貨時間=空值，以當日計算
                    _newDtRow[20] = _dt.Rows[i]["overdue_day"];
                    #endregion
                    _newDtRow[21] = _dt.Rows[i]["item_id"];
                    if (_dt.Rows[i]["product_mode"].ToString() == "1")
                    {
                        _newDtRow[22] = "自出";
                    }
                    else if (_dt.Rows[i]["product_mode"].ToString() == "2")
                    {
                        _newDtRow[22] = "寄倉";
                    }
                    else if (_dt.Rows[i]["product_mode"].ToString() == "3")
                    {
                        _newDtRow[22] = "調度";
                    }
                    else
                    {
                        _newDtRow[22] = "";
                    }
                    DataTable _dtSlaveStatus = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='order_status' and parameterCode='{0}'", _dt.Rows[i]["slave_status"]));//order_status
                    if (_dtSlaveStatus.Rows.Count > 0)
                    {
                        string name = _dtSlaveStatus.Rows[0]["remark"].ToString();
                        _newDtRow[23] = name;
                    }
                    else
                    {
                        _newDtRow[23] = "";
                    }
                    DataTable _dtDetailStatus = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='order_status' and parameterCode='{0}'", _dt.Rows[i]["detail_status"]));//order_status
                    if (_dtDetailStatus.Rows.Count > 0)
                    {
                        string name = _dtDetailStatus.Rows[0]["remark"].ToString();
                        _newDtRow[24] = name;
                    }
                    else
                    {
                        _newDtRow[24] = "";
                    }
                    _newDtRow[25] = _dt.Rows[i]["dvendor_name_simple"];
                    _newDtRow[26] = _dt.Rows[i]["deliver_master_date"];

                    _newDtRow[27] = _dt.Rows[i]["note_order"];
                    _newDtRow[28] = _dt.Rows[i]["note_admin"];
                    
                    _newDt.Rows.Add(_newDtRow);
                }
                    return _newDt;
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterMgr-->GetReportManagementList-->" + ex.Message, ex);
            }
        }


        public DataTable GetDeliveryMsgExcelList(DeliverMasterQuery deliver)
        {
            try
            {

                DataTable _newDt = new DataTable();
                if (deliver.serch_msg == 1)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(deliver.serch_where.Trim()))
                        {
                            deliver.serch_where = string.Empty;
                        }
                        else
                        {
                            int i = Convert.ToInt32(deliver.serch_where);
                        }
                    }
                    catch (Exception ex)
                    {
                        deliver.serch_where = "0";
                    }
                }
                else if (deliver.serch_msg == 2)
                {
                    if (deliver.serch_where.Length > 0)
                    {
                        DataTable _myDt = _ideliver.GetVnndorId(deliver.serch_where);
                        if (_myDt.Rows.Count > 0)
                        {
                            deliver.serch_where = string.Empty;
                            for (int i = 0; i < _myDt.Rows.Count; i++)
                            {
                                deliver.serch_where = deliver.serch_where + _myDt.Rows[i]["vendor_id"] + ",";
                            }
                            int length = deliver.serch_where.Length;
                            deliver.serch_where = deliver.serch_where.Substring(0, length - 1);
                        }
                        else
                        {
                            deliver.serch_where = "0";
                        }
                    }
                    else
                    {
                        deliver.serch_where = string.Empty;
                    }
                }
                else
                {
                    deliver.serch_where = string.Empty;
                }

                DataTable _dt = _ideliver.GetDeliveryMsgExcelList(deliver);
                _newDt.Columns.Add("delivery_code", typeof(String));
                _newDt.Columns.Add("deliver_id", typeof(String));
                _newDt.Columns.Add("order_id", typeof(String));
                _newDt.Columns.Add("order_date", typeof(String));
                _newDt.Columns.Add("order_status", typeof(String));
                _newDt.Columns.Add("order_payment", typeof(String));
                _newDt.Columns.Add("delivery_store", typeof(String));
                _newDt.Columns.Add("logisticsTypes", typeof(String));
                _newDt.Columns.Add("delivery_status", typeof(String));
                _newDt.Columns.Add("vendor_name_simple", typeof(String));
                _newDt.Columns.Add("freight_set", typeof(String));
                _newDt.Columns.Add("delivery_freight_cost", typeof(String));
                _newDt.Columns.Add("delivery_date", typeof(String));
                _newDt.Columns.Add("arrival_date", typeof(String));
                _newDt.Columns.Add("estimated_delivery_date", typeof(String));
                _newDt.Columns.Add("estimated_arrival_date", typeof(String));
                _newDt.Columns.Add("estimated_arrival_period", typeof(String));
                _newDt.Columns.Add("delivery_name", typeof(String));
                _newDt.Columns.Add("product_name", typeof(String));
                _newDt.Columns.Add("buy_num", typeof(String));
                _newDt.Columns.Add("overdue_day", typeof(String));
                _newDt.Columns.Add("item_id", typeof(String));
                _newDt.Columns.Add("product_mode", typeof(String));
                _newDt.Columns.Add("slave_status", typeof(String));
                _newDt.Columns.Add("detail_status", typeof(String));
                _newDt.Columns.Add("dvendor_name_simple",typeof(String));
                _newDt.Columns.Add("deliver_master_date", typeof(String));
                _newDt.Columns.Add("note_order", typeof(String));
                _newDt.Columns.Add("note_admin", typeof(String));
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    int product_id = Convert.ToInt32(_dt.Rows[i]["product_id"]);
                    uint detail_id = Convert.ToUInt32(_dt.Rows[i]["detail_id"]);
                    int item_id = Convert.ToInt32(_dt.Rows[i]["item_id"]);
                    string created = _dt.Rows[i]["created"].ToString();
                    DataRow _newDtRow = _newDt.NewRow();
                    _newDtRow = _newDt.NewRow();
                    _newDtRow[0] = _dt.Rows[i]["delivery_code"];
                    _newDtRow[1] = _dt.Rows[i]["deliver_id"];
                    _newDtRow[2] = _dt.Rows[i]["order_id"];
                    _newDtRow[3] = _dt.Rows[i]["order_date"];
                    DataTable _dtOrderStatus = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='order_status' and parameterCode='{0}'", _dt.Rows[i]["order_status"]));//order_status
                    if (_dtOrderStatus.Rows.Count > 0)
                    {
                        string name = _dtOrderStatus.Rows[0]["remark"].ToString();
                        _newDtRow[4] = name;
                    }
                    else
                    {
                        _newDtRow[4] = "";
                    }
                    DataTable _dtOrderPayment = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='payment' and parameterCode='{0}'", _dt.Rows[i]["order_payment"]));//order_payment
                    if (_dtOrderPayment.Rows.Count > 0)
                    {
                        _newDtRow[5] = _dtOrderPayment.Rows[0]["parameterName"];
                    }
                    else
                    {
                        _newDtRow[5] = "";
                    }
                    DataTable _dtDeliveryStore = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='Deliver_Store' and parameterCode='{0}'", _dt.Rows[i]["delivery_store"]));//delivery_store
                    if (_dtDeliveryStore.Rows.Count > 0)
                    {
                        _newDtRow[6] = _dtDeliveryStore.Rows[0]["parameterName"];
                    }
                    else
                    {
                        _newDtRow[6] = "";
                    }
                    DataTable _dtLogisticsType = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='logistics_type' and parameterCode='{0}'", _dt.Rows[i]["logisticsTypes"]));//logisticsType
                    if (_dtLogisticsType.Rows.Count > 0)
                    {
                        _newDtRow[7] = _dtLogisticsType.Rows[0]["parameterName"];
                    }
                    else
                    {
                        _newDtRow[7] = "";
                    }
                    DataTable _dtDeliveryStatus = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='delivery_status' and parameterCode='{0}'", _dt.Rows[i]["delivery_status"]));
                    if (_dtDeliveryStatus.Rows.Count > 0)
                    {
                        _newDtRow[8] = _dtDeliveryStatus.Rows[0]["parameterName"];
                    }
                    else
                    {
                        _newDtRow[8] = "";
                    }
                    _newDtRow[9] = _dt.Rows[i]["vendor_name_simple"];
                    DataTable _dtProductFreight = _access.getDataTable(string.Format(@"SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='product_freight' and parameterCode='{0}';", _dt.Rows[i]["freight_set"]));
                    if (_dtProductFreight.Rows.Count > 0)
                    {
                        _newDtRow[10] = _dtProductFreight.Rows[0]["parameterName"];
                    }
                    else
                    {
                        _newDtRow[10] = "";
                    }
                    _newDtRow[11] = _dt.Rows[i]["delivery_freight_cost"];
                    _newDtRow[12] = _dt.Rows[i]["delivery_date"];
                    _newDtRow[13] = _dt.Rows[i]["arrival_date"];
                    //_newDtRow[14] = _dt.Rows[i]["estimated_delivery_date"];//預計出貨時間 
                    //_newDtRow[14] = ExpectTime("product", product_id);
                    //_newDtRow[15] = _dt.Rows[i]["estimated_arrival_date"];//預計到貨時間 
                    //_newDtRow[15] = ArriveTime(item_id, created);
                    _newDtRow[14] = _dt.Rows[i]["estimated_delivery_date"];//預計出貨時間 
                    _newDtRow[15] = _dt.Rows[i]["delivery_date_str"];

                    _newDtRow[16] = _dt.Rows[i]["estimated_arrival_period"];
                    _newDtRow[17] = _dt.Rows[i]["delivery_name"];
                    _newDtRow[18] = _dt.Rows[i]["product_name"];
                    _newDtRow[19] = _dt.Rows[i]["buy_num"];
                    #region 計算逾期天數：(出貨時間-付款單成立日期+1)-4，出貨時間=空值，以當日計算。
                    _newDtRow[20] = _dt.Rows[i]["overdue_day"];
                    #endregion
                    _newDtRow[21] = _dt.Rows[i]["item_id"];
                    if (_dt.Rows[i]["product_mode"].ToString() == "1")
                    {
                        _newDtRow[22] = "自出";
                    }
                    else if (_dt.Rows[i]["product_mode"].ToString() == "2")
                    {
                        _newDtRow[22] = "寄倉";
                    }
                    else if (_dt.Rows[i]["product_mode"].ToString() == "3")
                    {
                        _newDtRow[22] = "調度";
                    }
                    else
                    {
                        _newDtRow[22] = "";
                    }
                    DataTable _dtSlaveStatus = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='order_status' and parameterCode='{0}'", _dt.Rows[i]["slave_status"]));//order_status
                    if (_dtSlaveStatus.Rows.Count > 0)
                    {
                        string name = _dtSlaveStatus.Rows[0]["remark"].ToString();
                        _newDtRow[23] = name;
                    }
                    else
                    {
                        _newDtRow[23] = "";
                    }
                    DataTable _dtDetailStatus = _access.getDataTable(string.Format("SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='order_status' and parameterCode='{0}'", _dt.Rows[i]["detail_status"]));//order_status
                    if (_dtDetailStatus.Rows.Count > 0)
                    {
                        string name = _dtDetailStatus.Rows[0]["remark"].ToString();
                        _newDtRow[24] = name;
                    }
                    else
                    {
                        _newDtRow[24] = "";
                    }
                    _newDtRow[25] = _dt.Rows[i]["dvendor_name_simple"];
                    _newDtRow[26] = _dt.Rows[i]["deliver_master_date"];
                    _newDtRow[27] = _dt.Rows[i]["note_order"];
                    _newDtRow[28] = _dt.Rows[i]["note_admin"];
                    _newDt.Rows.Add(_newDtRow);
                }
                    return _newDt;
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterMgr-->GetDeliveryMsgExcelList-->" + ex.Message, ex);
            }

        }
        #region 根據產品編號獲取到預計出貨時間
        //public string ExpectTime(string t, int v = 0)
        //{
        //    DateTime date = DateTime.MinValue;
        //    try
        //    {
        //        _srMgr = new ScheduleRelationMgr(connection);
        //        date = _srMgr.GetRecentlyTime(v, t);
        //        if (date == DateTime.MinValue)
        //        {
        //            return "";
        //        }
        //        else
        //        {
        //            return date.ToString("yyyy/MM/dd");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("DeliverMasterMgr-->ExpectTime-->" + ex.Message, ex);
        //    }
        //}
        public string ExpectTime(string t, int v = 0)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath(xmlPath);
            SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
            SiteConfig NETDoMain_Name = _siteConfigMgr.GetConfigByName("NETDoMain_Name");

            string Url = "http://"+NETDoMain_Name.Value+"/Open/ExpectTime?t="+t+"&v="+v;
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(Url);
                httpRequest.Timeout = 2000;
                httpRequest.Method = "GET";
                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                StreamReader sr = new StreamReader(httpResponse.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                string result = sr.ReadToEnd();
                //JsonConvert.DeserializeObject(result);
                JObject o = JObject.Parse(result);
                result = o["data"].ToString();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterMgr-->ExpectTime-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 獲取到預計到貨日期
        public string ArriveTime(int itemId, string created)
        {
            created= CommonFunction.DateTimeToString(Convert.ToDateTime(created.ToString()));
            string path = System.Web.HttpContext.Current.Server.MapPath(xmlPath);
            SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
            SiteConfig NETDoMain_Name = _siteConfigMgr.GetConfigByName("NETDoMain_Name");

            string Url = "http://" + NETDoMain_Name.Value + "/Open/ArriveTime?itemId=" + itemId+"&dateTime="+created;
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(Url);
                httpRequest.Timeout = 2000;
                httpRequest.Method = "GET";
                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                StreamReader sr = new StreamReader(httpResponse.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                string result = sr.ReadToEnd();
                //JsonConvert.DeserializeObject(result);
                JObject o = JObject.Parse(result);
                result = o["data"].ToString();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterMgr-->ArriveTime-->" + ex.Message, ex);
            }
        }
        //public string ArriveTime(uint detailId)
        //{
        //    //add by wwei0216w 2015/5/25
        //    int days = 0;
        //    DateTime date;
        //    try
        //    {
        //        _srMgr = new ScheduleRelationMgr(connection);
        //        ProductItemCustom pi = new ProductItemCustom();
        //        IProductItemImplMgr _productItemMgr = new ProductItemMgr(connection);
        //        IOrderDetailImplMgr _orderDetailMgr = new OrderDetailMgr(connection);
        //        OrderDetailCustom od = _orderDetailMgr.GetArriveDay(detailId).FirstOrDefault();///獲得訂單中關於天數的信息(供應商出貨天數,運達天數....)
        //        if (od.item_mode == 0) //如果是單一商品(既item_mode=0,parent_id = 0)則根據item_id來獲取計算運達天數的信息
        //        {
        //            pi = _productItemMgr.GetProductArriveDay(new ProductItem { Item_Id = od.Item_Id }, "item");
        //        }
        //        else if (od.item_mode != 0)//如果是組合商品(既item_mode !=0,有parent_id的值)則更具parent_id來計算運達天數
        //        {
        //            //組合商品需根據出貨單確定最晚一件到達的時間為運達天數,暫且擱置
        //            return "";
        //        }
        //        else
        //        {
        //            return "";
        //        }
        //        days = pi.Arrive_Days + pi.Deliver_Days;///計算運達天數
        //        date = _srMgr.GetRecentlyTime(Convert.ToInt32(pi.Product_Id), "product");///調用GetRecentlyTime()方法 獲得最近出貨時間

        //        //edit by wwei0216w 2015/6/2 添加item_stock > 0 的判斷,如果 item_stock > 0 則不適用排程的時間,啟用當前時間作為基礎時間
        //        if (date == DateTime.MinValue || date < DateTime.Now || date == null)///如果得到的最近出貨天數  為最小時間   或者   小于當前時間
        //        {
        //            date = DateTime.Now;
        //        }

        //        DateTime isAddDay = CommonFunction.GetNetTime(od.order_date_pay);///轉換時間
        //        date = restDay(date, isAddDay, days);///計算具體到貨日期
        //        if (date == DateTime.MinValue)
        //        {
        //            return "";
        //        }
        //        else
        //        {
        //            return date.ToString("yyyy/MM/dd");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("DeliverMasterMgr-->ArriveTime-->" + ex.Message, ex);
        //    }
        //}
        #endregion 

        ///返回貨物運達日期
        private DateTime restDay(DateTime dt, DateTime isAddDay, int days)
        {
            try
            {
                long num_date = CommonFunction.GetPHPTime(dt.ToString());
                days = isAddDay.Hour > 15 ? days + 1 : days;  //判斷時間是否大於15點,大於時,運達天數加1
                ICalendarImplMgr _cdMgr = new CalendarMgr(connection);
                List<Calendar> calendar_list = _cdMgr.GetCalendarInfo(new Calendar { EndDateStr = num_date.ToString() }); ///獲取行事歷控件中休息時間的集合
                return VerifyTime(dt, days, calendar_list);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterMgr-->restDay-->" + ex.Message, ex);
            }
        }
        public DateTime VerifyTime(DateTime dt, int days, List<Calendar> calendar_list)
        {
            /// 遞歸調用計算運達天數 add by wwei0216w 2015/5/26
            try
            {
                long num_date = CommonFunction.GetPHPTime(dt.ToString());
                List<Calendar> result_list = new List<Calendar>();
                result_list = calendar_list.FindAll(m => (num_date >= Convert.ToInt64(m.StartDateStr)) && (num_date <= Convert.ToInt64(m.EndDateStr)));
                if (days != 0 && result_list.Count > 0)
                {
                    dt = dt.AddDays(1);
                    dt = VerifyTime(dt, days, calendar_list);
                    return dt;
                }
                else if (days != 0 && result_list.Count == 0)
                {
                    dt = dt.AddDays(1);
                    days--;
                    dt = VerifyTime(dt, days, calendar_list);
                    return dt;
                }
                else if (days == 0 && (result_list.Count > 0))
                {
                    dt = dt.AddDays(1);
                    dt = VerifyTime(dt, days, calendar_list);
                    return dt;
                }
                else
                {
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterMgr-->VerifyTime-->" + ex.Message, ex);
            }
        }
        public DataTable GetVnndorId(string name)
        {
            throw new NotImplementedException();
        }

        public int GetDeliverMasterCount(DeliverMasterQuery query)
        {
            try
            {
                return _ideliver.GetDeliverMasterCount(query);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterMgr-->GetDeliverMasterCount-->" + ex.Message, ex);
            }
        }
        public DataTable GetDeliverMaster(string hourNum)
        {
            try
            {
                return _ideliver.GetDeliverMaster(hourNum);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverMasterMgr-->GetDeliverMaster-->" + ex.Message, ex);
            }
        }

    }
}
