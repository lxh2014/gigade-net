using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
namespace BLL.gigade.Mgr
{
    public class ReturnMasterMgr : IReturnMasterImplMgr
    {
        private IReturnMasterImplDao _iordertempreturnlistdao;

        public ReturnMasterMgr(string connectionString)
        {
            _iordertempreturnlistdao = new ReturnMasterDao(connectionString);
        }

        #region send
        public DataTable GetOrderReturnCount(OrderReturnUserQuery store)
        {
            try
            {
                return _iordertempreturnlistdao.GetOrderReturnCount(store);
            }
            catch (Exception ex)
            {
                throw new Exception(" ReturnMasterMgr-->GetOrderReturnCount-->" + ex.Message, ex);
            }
        }
        public int InsertOrderReturnMaster(OrderReturnUserQuery store)
        {
            try
            {
                return _iordertempreturnlistdao.InsertOrderReturnMaster(store);
            }
            catch (Exception ex)
            {
                throw new Exception(" ReturnMasterMgr-->InsertOrderReturnMaster-->" + ex.Message, ex);
            }
           
        }
        public int InsertOrderReturnDetail(OrderReturnUserQuery store)
        {
            try
            {
                return _iordertempreturnlistdao.InsertOrderReturnDetail(store);
            }
            catch (Exception ex)
            {
                throw new Exception(" ReturnMasterMgr-->InsertOrderReturnDetail-->" + ex.Message, ex);
            }
           
           
        }
        public int InsertOrderMasterStatus(OrderMasterStatus OMS)
        {
            try
            {
                return _iordertempreturnlistdao.InsertOrderMasterStatus(OMS);
            }
            catch (Exception ex)
            {
                throw new Exception(" ReturnMasterMgr-->InsertOrderMasterStatus-->" + ex.Message, ex);
            }
            
        }
        public List<OrderReturnUserQuery> OrderMasterQuery(OrderReturnUserQuery store)
        {
            try
            {
                return _iordertempreturnlistdao.OrderMasterQuery(store);
            }
            catch (Exception ex)
            {
                throw new Exception(" ReturnMasterMgr-->OrderMasterQuery-->" + ex.Message, ex);
            }
          
        }
        public int UpdateOrderMaster(OrderReturnUserQuery store)
        {
            try
            {
                return _iordertempreturnlistdao.UpdateOrderMaster(store);
            }
            catch (Exception ex)
            {
                throw new Exception(" ReturnMasterMgr-->UpdateOrderMaster-->" + ex.Message, ex);
            }
           
        }
        public int UpdateOrderDetailStatus(OrderReturnUserQuery store)
        {
            try
            {
                return _iordertempreturnlistdao.UpdateOrderDetailStatus(store);
            }
            catch (Exception ex)
            {
                throw new Exception(" ReturnMasterMgr-->UpdateOrderDetailStatus-->" + ex.Message, ex);
            }
           
        }
        #endregion

        public DataTable SelOrderMaster(OrderReturnUserQuery store)
        {
            try
            {
                return _iordertempreturnlistdao.SelOrderMaster(store);
            }
            catch (Exception ex)
            {
                throw new Exception(" ReturnMasterMgr-->SelOrderMaster-->" + ex.Message, ex);
            }
           
        }
        public DataTable Seltime(OrderReturnUserQuery store)
        {
            try
            {
                return _iordertempreturnlistdao.Seltime(store);
            }
            catch (Exception ex)
            {
                throw new Exception(" ReturnMasterMgr-->Seltime-->" + ex.Message, ex);
            }
           
        }
        public DataTable SelCon(OrderReturnUserQuery store)
        {
            try
            {
                return _iordertempreturnlistdao.SelCon(store);
            }
            catch (Exception ex)
            {
                throw new Exception(" ReturnMasterMgr-->SelCon-->" + ex.Message, ex);
            }
           
        }
        public DataTable SelInvoiceid(int invoice)
        {
            try
            {
                return _iordertempreturnlistdao.SelInvoiceid(invoice);
            }
            catch (Exception ex)
            {
                throw new Exception(" ReturnMasterMgr-->SelInvoiceid-->" + ex.Message, ex);
            }
            
        }
        public int Delinvoice(int invoice)
        {
            try
            {
                return _iordertempreturnlistdao.Delinvoice(invoice);
            }
            catch (Exception ex)
            {
                throw new Exception(" ReturnMasterMgr-->Delinvoice-->" + ex.Message, ex);
            }
            
        }
        public int Updinvoice(OrderReturnUserQuery store)
        {
            try
            {
                return _iordertempreturnlistdao.Updinvoice(store);
            }
            catch (Exception ex)
            {
                throw new Exception(" ReturnMasterMgr-->Updinvoice-->" + ex.Message, ex);
            }
         
        }
        public List<OrderMaster> Selpay(OrderMaster store)
        {
            try
            {
                return _iordertempreturnlistdao.Selpay(store);
            }
            catch (Exception ex)
            {
                throw new Exception(" ReturnMasterMgr-->Selpay-->" + ex.Message, ex);
            }
            
        }
        public DataTable Seldetail(OrderReturnUserQuery store, string status)
        {
            try
            {
                return _iordertempreturnlistdao.Seldetail(store, status);
            }
            catch (Exception ex)
            {
                throw new Exception(" ReturnMasterMgr-->Seldetail-->" + ex.Message, ex);
            }
           
        }
        public int Insertdb(InvoiceMasterRecord Imr, InvoiceSliveInfo Isi)
        {
            try
            {
                return _iordertempreturnlistdao.Insertdb(Imr, Isi);
            }
            catch (Exception ex)
            {
                throw new Exception(" ReturnMasterMgr-->Insertdb-->" + ex.Message, ex);
            }
            
        }
        public int Updcount(InvoiceAllowanceRecord m)
        {
            try
            {
                return _iordertempreturnlistdao.Updcount(m);
            }
            catch (Exception ex)
            {
                throw new Exception(" ReturnMasterMgr-->Updcount-->" + ex.Message, ex);
            }
           
        }
        public DataTable Selmaster(int invoice_id)
        {
            try
            {
                return _iordertempreturnlistdao.Selmaster(invoice_id);
            }
            catch (Exception ex)
            {
                throw new Exception(" ReturnMasterMgr-->Selmaster-->" + ex.Message, ex);
            }
           
        }
        public DataTable Selslive(int invoice_id)
        {
            try
            {
                return _iordertempreturnlistdao.Selslive(invoice_id);
            }
            catch (Exception ex)
            {
                throw new Exception(" ReturnMasterMgr-->Selslive-->" + ex.Message, ex);
            }
           
        }
        public int UpdMaster(int invoice_id)
        {
            try
            {
                return _iordertempreturnlistdao.UpdMaster(invoice_id);
            }
            catch (Exception ex)
            {
                throw new Exception(" ReturnMasterMgr-->UpdMaster-->" + ex.Message, ex);
            }
        }
        #region 暫存退貨單棄用
        //public int UpdateTempStatus(OrderReturnUserQuery store)
        //{
        //    try
        //    {
        //        return _iordertempreturnlistdao.UpdateTempStatus(store);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(" ReturnMasterMgr-->UpdateTempStatus-->" + ex.Message, ex);
        //    }    
        //}
        //public List<OrderReturnUserQuery> GetOrderTempReturnList(OrderReturnUserQuery store, out int totalCount)
        //{
        //    try
        //    {
        //        return _iordertempreturnlistdao.GetOrderTempReturnList(store, out totalCount);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(" ReturnMasterMgr-->GetOrderTempReturnList-->" + ex.Message, ex);
        //    }
        //}
        #endregion
    }
}
