using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;

namespace BLL.gigade.Mgr
{
    public class InvoiceMasterRecordMgr : IInvoiceMasterRecordImplMgr
    {
        private IInvoiceMasterRecordImplDao _imrDao;
        private OrderMasterDao orderMasterDao;
        private OrderDetailDao orderDetailDao;
        public InvoiceMasterRecordMgr(string connectionstring)
        {
            _imrDao = new InvoiceMasterRecordDao(connectionstring);
        }
        /// <summary>
        /// 開立發票列表
        /// </summary>
        /// <param name="imrq"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable GetInvoiceList(InvoiceMasterRecordQuery imrq, out int totalCount)
        {
            try
            {
                return _imrDao.GetInvoiceList(imrq, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("InvoiceMasterRecordMgr-->GetInvoiceList" + ex.Message, ex);
            }
        }

        public DataTable ExportTXT(InvoiceMasterRecordQuery imrq)
        {
            try
            {
                return _imrDao.ExportTXT(imrq);
            }
            catch (Exception ex)
            {
                throw new Exception("InvoiceMasterRecordMgr-->ExportTXT" + ex.Message, ex);
            }
        }
        public DataTable ExportExlF(InvoiceMasterRecordQuery imrq)
        {
            try
            {
                return _imrDao.ExportExlF(imrq);
            }
            catch (Exception ex)
            {
                throw new Exception("InvoiceMasterRecordMgr-->ExportExlF" + ex.Message, ex);
            }
        }
        public DataTable ExportExlS(string order_id)
        {
            try
            {
                return _imrDao.ExportExlS(order_id);
            }
            catch (Exception ex)
            {
                throw new Exception("InvoiceMasterRecordMgr-->ExportExlS" + ex.Message, ex);
            }
        }
        public DataTable InvoicePrint(uint invoice_id)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 修改發票信息
        /// </summary>
        /// <param name="imr"></param>
        /// <returns></returns>
        public int Update(InvoiceMasterRecord imr)
        {
            try
            {
                return _imrDao.Update(imr);
            }
            catch (Exception ex)
            {
                throw new Exception("InvoiceMasterRecordMgr-->Update" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 獲取地址下拉列表
        /// </summary>
        /// <param name="zip"></param>
        /// <returns></returns>
        public List<Zip> GetZipAddress(Zip zip)
        {
            try
            {
                return _imrDao.GetZipAddress(zip);
            }
            catch (Exception ex)
            {
                throw new Exception("InvoiceMasterRecordMgr-->GetZipAddress" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 獲得參數表的數據
        /// </summary>
        /// <param name="parameterType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public DataTable GetParametersrc(string parameterType, string key)
        {
            try
            {
                return _imrDao.GetParametersrc(parameterType, key);
            }
            catch (Exception ex)
            {
                throw new Exception("InvoiceMasterRecordMgr-->GetParametersrc" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 更新定單狀態以便開立發票
        /// </summary>
        /// <param name="order_id">定單編號</param>
        /// <param name="freight_normal">常溫</param>
        /// <param name="freight_low">低溫</param>
        /// <param name="status">操作狀態</param>
        /// <returns>是否操作成功</returns>
        public bool ModifyOrderInvoice(int order_id, int freight_normal, int freight_low, string status)
        {
            //todo:對應Invoice.php頁面中第23行實現modify_order_invoice方法
            bool flag = false;
            OrderMasterQuery query = new OrderMasterQuery();
            OrderMasterQuery model = new OrderMasterQuery();
            query.Order_Id = Convert.ToUInt32(order_id);

            if (status == "modify")
            {
                //設定條件 invoice_master_record.invoice_number='is not null'
                query.status_description = "modify";
            }
            model = orderMasterDao.GetOrderMasterInvoice(query);
            if (model != null)
            {
                if (model.Deduct_Card_Bonus > 0)
                {
                    OrderDetailQuery queryOrderDetail = new OrderDetailQuery
                    {
                        Order_Id = Convert.ToUInt32(order_id)
                    };
                    DataTable dtAmount = orderDetailDao.GetBuyAmountAndTaxType(queryOrderDetail);
                    if (dtAmount!=null && dtAmount.Rows.Count>0)
                    {
                        int nTax = 0;
                        int nFreeTax = 0;
                        for (int i = 0; i < dtAmount.Rows.Count; i++)
                        {
                            if (dtAmount.Rows[i]["tax_type"]=="1")
                            {
                                //nTax=round
                            }
                            else if (dtAmount.Rows[i]["tax_type"]=="3")
                            {

                            }
                        }
                    }
                }
            }

            return flag;
        }
    }
}
