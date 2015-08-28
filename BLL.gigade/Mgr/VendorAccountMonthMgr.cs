using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Common;
using BLL.gigade.Model.Custom;
namespace BLL.gigade.Mgr
{
    public class VendorAccountMonthMgr : IVendorAccountMonthImplMgr
    {
        private IVendorAccountMonthImplDao _Ivendor;
        public VendorAccountMonthMgr(string connectionString)
        {
            _Ivendor = new VendorAccountMonthDao(connectionString);
        }
        /// <summary>
        /// 供應商業績報表
        /// </summary>
        /// <param name="store"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<VendorAccountMonthQuery> GetVendorAccountMonthList(VendorAccountMonthQuery store, out int totalCount)
        {
            try
            {
                return _Ivendor.GetVendorAccountMonthList(store, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("VendorAccountMonthMgr-->GetVendorAccountMonthList-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查詢供應商業績明細
        /// </summary>
        /// <param name="store"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<VendorAccountDetailQuery> GetVendorAccountMonthDetailList(VendorAccountDetailQuery store, out int totalCount)
        {
            try
            {
                return _Ivendor.GetVendorAccountMonthDetailList(store, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("VendorAccountMonthMgr-->GetVendorAccountMonthDetailList-->" + ex.Message, ex);
            }

        }
        /// <summary>
        /// 業績明細查詢
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<VendorAccountCustom> VendorAccountDetailExport(VendorAccountDetailQuery query)
        {
            try
            {
                return _Ivendor.VendorAccountDetailExport(query);
            }
            catch (Exception ex)
            {

                throw new Exception("VendorAccountMonthMgr-->VendorAccountDetailExport-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查詢供應商信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public VendorQuery GetVendorInfoByCon(VendorQuery query)
        {
            try
            {
                return _Ivendor.GetVendorInfoByCon(query);
            }
            catch (Exception ex)
            {

                throw new Exception("VendorAccountMonthMgr-->GetVendorInfoByCon-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查詢供應商總賬
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable GetVendorAccountMonthZongZhang(VendorAccountMonthQuery query)
        {
            try
            {
                return _Ivendor.GetVendorAccountMonthZongZhang(query);
            }
            catch (Exception ex)
            {

                throw new Exception("VendorAccountMonthMgr-->GetVendorAccountMonthZongZhang-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查詢調度倉運費計算
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable GetFreightMoney(VendorAccountDetailQuery query, out int tempFreightDelivery_Normal, out int tempFreightDelivery_Low)
        {
            try
            {
                return _Ivendor.GetFreightMoney(query, out tempFreightDelivery_Normal, out tempFreightDelivery_Low);
            }
            catch (Exception ex)
            {

                throw new Exception("VendorAccountMonthMgr-->GetFreightMoney-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 批次出貨單明細
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable BatchOrderDetail(VendorAccountDetailQuery query)
        {
            try
            {
                return _Ivendor.BatchOrderDetail(query);
            }
            catch (Exception ex)
            {

                throw new Exception("VendorAccountMonthMgr-->BatchOrderDetail-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查詢訂單筆數
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable VendorAccountCountExport(VendorAccountMonthQuery query)
        {
            try
            {
                return _Ivendor.VendorAccountCountExport(query);
            }
            catch (Exception ex)
            {

                throw new Exception("VendorAccountMonthMgr-->VendorAccountCountExport-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 供應商總表部分信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable VendorAccountInfoExport(VendorAccountMonthQuery query)
        {
            try
            {
                return _Ivendor.VendorAccountInfoExport(query);
            }
            catch (Exception ex)
            {

                throw new Exception("VendorAccountMonthMgr-->VendorAccountCountExport-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查詢供應商
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable GetVendorAccountMonthInfo(VendorAccountMonthQuery query)
        {
            try
            {
                return _Ivendor.GetVendorAccountMonthInfo(query);
            }
            catch (Exception ex)
            {

                throw new Exception("VendorAccountMonthMgr-->GetVendorAccountMonthInfo-->" + ex.Message, ex);
            }
        }
         /// <summary>
        /// 查詢應稅、免稅金額計算
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable GetTaxMoney(VendorAccountDetailQuery query)
        {
            try
            {
                return _Ivendor.GetTaxMoney(query);
            }
            catch (Exception ex)
            {

                throw new Exception("VendorAccountMonthMgr-->GetTaxMoney-->" + ex.Message, ex);
            }
        }
    }
}
