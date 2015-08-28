#region 文件信息
/* 
* Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromotionsAmountFareDao.cs      
* 摘 要：                                                                               
* 滿額滿件免運
* 当前版本：v1.1                                                                 
* 作 者： shuangshuang0420j                                           
* 完成日期：2014/6/20 
* 修改歷史：                                                                     
*         v1.1修改日期：2014/8/15 
*         v1.1修改人員：shuangshuang0420j     
*         v1.1修改内容：規範代碼結構，完善異常拋出，添加注釋
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using System.Data;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr  
{
    public class PromotionsAmountFareMgr : IPromotionsAmountFareImplMgr
    {

        private IPromotionsAmountFareImplDao _promoFareDao;
        private string connStr;
        public PromotionsAmountFareMgr(string connectionstring)
        {
            _promoFareDao = new PromotionsAmountFareDao(connectionstring);
            connStr = connectionstring;
        }

        #region 獲取滿額滿件免運的信息+List<PromotionsAmountFareQuery> Query(PromotionsAmountFareQuery query, out int totalCount)
        /// <summary>
        /// 獲取滿額滿件免運的信息
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<PromotionsAmountFareQuery> Query(PromotionsAmountFareQuery query, out int totalCount)
        {
            try
            {
                List<PromotionsAmountFareQuery> _list = _promoFareDao.Query(query, out totalCount);
                if (_list.Count > 0)
                {
                    ParametersrcDao _parameterDao = new ParametersrcDao(connStr);
                    List<BLL.gigade.Model.Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("product_freight", "event_type", "device", "payment");
                    foreach (PromotionsAmountFareQuery item in _list)
                    {
                        var alist = parameterList.Find(m => m.ParameterType == "product_freight" && m.ParameterCode == item.type.ToString());
                        var blist = parameterList.Find(m => m.ParameterType == "event_type" && m.ParameterCode == item.event_type.ToString());
                        var clist = parameterList.Find(m => m.ParameterType == "device" && m.ParameterCode == item.device.ToString());
                        if (alist != null)
                        {
                            item.typeName = alist.parameterName;
                        }
                        if (blist != null)
                        {
                            item.event_type_name = blist.parameterName;
                        }
                        if (clist != null)
                        {
                            item.deviceName = clist.parameterName;
                        }
                        if (!string.IsNullOrEmpty(item.payment_code))
                        {
                            string[] arryPayment = item.payment_code.Split(',');//將payment_code轉化為payment_name
                            for (int i = 0; i < arryPayment.Length; i++)
                            {
                                if (arryPayment[i] != "0")
                                {
                                    var dlist = parameterList.Find(m => m.ParameterType == "payment" && m.ParameterCode == arryPayment[i].ToString());
                                    if (dlist != null)
                                    {
                                        item.payment_name += dlist.parameterName + ",";
                                    }
                                }

                            }
                            item.payment_name = item.payment_name.TrimEnd(',');
                        }
                    }
                }
                return _list;
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountFareMgr-->Query-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 保存滿額滿件免運的信息+int Save(PromotionsAmountFareQuery model)
        /// <summary>
        /// 保存滿額滿件免運的信息
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public int Save(PromotionsAmountFareQuery model)
        {
            try
            {
                return _promoFareDao.Save(model);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountFareMgr-->Save-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 查詢滿額滿件免運的信息 +DataTable Select(Model.PromotionsAmountFare model)
        public DataTable Select(Model.PromotionsAmountFare model)
        {
            try
            {
                return _promoFareDao.Select(model);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountFareMgr-->Select-->" + ex.Message, ex);
            }

        }
        #endregion

        #region 查詢滿額滿件免運的信息+ PromotionsAmountFareQuery Select(int id)
        public PromotionsAmountFareQuery Select(int id)
        {
            try
            {
                return _promoFareDao.Select(id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountFareMgr-->Select-->" + ex.Message, ex);
            }

        }
        #endregion
        #region 第二步保存滿額滿件免運的信息+ int ReSave(PromotionsAmountFareQuery model)

        public int ReSave(PromotionsAmountFareQuery model)
        {
            try
            {
                return _promoFareDao.ReSave(model);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountFareMgr-->ReSave-->" + ex.Message, ex);
            }

        }
        #endregion
        #region 刪除滿額滿件免運的信息+ int Delete(int id)

        public int Delete(int id)
        {
            try
            {
                return _promoFareDao.Delete(id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountFareMgr-->Delete-->" + ex.Message, ex);
            }

        }
        #endregion
        #region 修改滿額滿件免運的信息+  int Update(PromotionsAmountFareQuery model, string oldEventId)

        public int Update(PromotionsAmountFareQuery model, string oldEventId)
        {
            try
            {
                return _promoFareDao.Update(model, oldEventId);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountFareMgr-->Update-->" + ex.Message, ex);
            }

        }
        #endregion
        #region 修改滿額滿件免運的狀態信息+ int UpdateActive(PromotionsAmountFareQuery model)

        public int UpdateActive(PromotionsAmountFareQuery model)
        {
            try
            {
                return _promoFareDao.UpdateActive(model);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountFareMgr-->UpdateActive-->" + ex.Message, ex);
            }

        }
        #endregion
    }
}
