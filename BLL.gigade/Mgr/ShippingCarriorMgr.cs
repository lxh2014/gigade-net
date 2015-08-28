using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Mgr
{
    public class ShippingCarriorMgr : IShippingCarriorImplMgr
    {
        private IShippingCarriorImplDao _shippingCarriorDao;
        private string connStr;
        public ShippingCarriorMgr(string connectionStr)
        {
            _shippingCarriorDao = new ShippingCarriorDao(connectionStr);
            connStr = connectionStr;
        }

        public List<ShippingCarriorCustom> QueryAll(ShippingCarrior sc, out int totalCount)
        {
            return _shippingCarriorDao.QueryAll(sc, out totalCount);
        }

        public int Save(ShippingCarrior sc)
        {
            return _shippingCarriorDao.InsertShippingCarrior(sc);
        }

        public int Update(ShippingCarrior sc)
        {
            return _shippingCarriorDao.UpdateShippingCarrior(sc);
        }

        public int Delete(string rids)
        {
            return _shippingCarriorDao.DeleteShippingCarrior(rids);
        }

        #region 獲取ShippingCarrior表的list + System.Data.DataTable GetShippingCarriorList(ShippingCarrior sc, out int totalCount)
        /// <summary>
        /// 獲取ShippingCarrior表的list
        /// </summary>
        /// <param name="sc">查詢條件</param>
        /// <param name="totalCount">頁數</param>
        /// <returns></returns>
        public System.Data.DataTable GetShippingCarriorList(Model.Query.ShippingCarriorQuery sc, out int totalCount)
        {
            try
            {
                DataTable _list = _shippingCarriorDao.GetShippingCarriorList(sc, out totalCount);

                if (_list.Rows.Count > 0)
                {
                    _list.Columns.Add("freight_big_area_name");
                    _list.Columns.Add("freight_type_name");
                    _list.Columns.Add("delivery_store_name");
                    ParametersrcDao _parameterDao = new Dao.ParametersrcDao(connStr);
                    List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("freight_big_area", "freight_type", "Deliver_Store");
                    foreach (DataRow dr in _list.Rows)
                    {
                        //求參數
                        var alist = parameterList.Find(m => m.ParameterType == "freight_big_area" && m.ParameterCode == dr["freight_big_area"].ToString());
                        var blist = parameterList.Find(m => m.ParameterType == "freight_type" && m.ParameterCode == dr["freight_type"].ToString());
                        var clist = parameterList.Find(m => m.ParameterType == "Deliver_Store" && m.ParameterCode == dr["delivery_store_id"].ToString());
                        if (alist != null)
                        {
                            dr["freight_big_area_name"] = alist.parameterName;
                        }
                        if (blist != null)
                        {
                            dr["freight_type_name"] = blist.parameterName;
                        }
                        if (clist != null)
                        {
                            dr["delivery_store_name"] = clist.parameterName;
                        }
                    }
                }
                return _list;
            }
            catch (Exception ex)
            {
                throw new Exception("ShippingCarriorMgr-->GetShippingCarriorList-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 獲取編輯頁面的combobox數據 +  System.Data.DataTable GetLogisticsName(Parametersrc pt)
        /// <summary>
        /// 獲取編輯頁面的combobox數據
        /// 
        /// </summary>
        /// <param name="pt">搜索條件</param>
        /// <param name="name">搜索條件(二級聯動)</param>
        /// <returns></returns>
        public System.Data.DataTable GetLogisticsName(Parametersrc pt, string name)
        {
            try
            {
                return _shippingCarriorDao.GetLogisticsName(pt, name);
            }
            catch (Exception ex)
            {
                throw new Exception("ShippingCarriorMgr-->GetLogisticsName-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 物流信息新增保存 + LogisticsSave(ShippingCarrior sc)
        public int LogisticsSave(ShippingCarrior sc)
        {
            try
            {
                return _shippingCarriorDao.LogisticsSave(sc);
            }
            catch (Exception ex)
            {
                throw new Exception("ShippingCarriorMgr-->LogisticsSave-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 物流信息修改保存 + LogisticsUpdate(ShippingCarrior sc)
        public int LogisticsUpdate(ShippingCarrior sc)
        {
            try
            {
                return _shippingCarriorDao.LogisticsUpdate(sc);
            }
            catch (Exception ex)
            {
                throw new Exception("ShippingCarriorMgr-->LogisticsUpdate-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 更新物流啟用狀態 + LogisticsUpdateActive(ShippingCarrior sc)
        public int LogisticsUpdateActive(ShippingCarrior sc)
        {
            try
            {
                return _shippingCarriorDao.LogisticsUpdateActive(sc);
            }
            catch (Exception ex)
            {
                throw new Exception("ShippingCarriorMgr-->LogisticsUpdateActive-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 物流信息新增檢測是否存在 + LogisticsAddCheck(ShippingCarrior sc)
        public int LogisticsAddCheck(ShippingCarrior sc)
        {
            try
            {
                return _shippingCarriorDao.LogisticsAddCheck(sc);
            }
            catch (Exception ex)
            {
                throw new Exception("ShippingCarriorMgr-->LogisticsAddCheck-->" + ex.Message, ex);
            }
        }
        #endregion
    }
}
