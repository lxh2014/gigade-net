#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromotionsAccumulateBonusMgr.cs
* 摘 要：
* 點數抵用與資料庫交互方法  
* 当前版本：v1.1
* 作 者：hongfei0416j    
* 完成日期：2014/6/20 
* 修改歷史:
*         v1.1修改日期：2014/8/15 
*         v1.1修改人員：hongfei0416j 
*         v1.1修改内容：合并代碼，添加注釋 
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

namespace BLL.gigade.Mgr
{
    public class PromotionsAccumulateBonusMgr : IPromotionsAccumulateBonusImplMgr
    {
        private IPromotionsAccumulateBonusImplDao _proAccBonusDao;
        private string conn;
        public PromotionsAccumulateBonusMgr(string connectionString)
        {
            _proAccBonusDao = new PromotionsAccumulateBonusDao(connectionString);
            conn = connectionString;
        }
         
        public int Save(Model.PromotionsAccumulateBonus promoAccumulateBonus)
        {
            try
            {
                return _proAccBonusDao.Save(promoAccumulateBonus);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAccumulateBonusMgr-->Save-->" + ex.Message, ex);
            }
        }

        public int Update(Model.PromotionsAccumulateBonus promoAccumulateBonus)
        {
            try
            {
                return _proAccBonusDao.Update(promoAccumulateBonus);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAccumulateBonusMgr-->Update-->" + ex.Message, ex);
            }
        }

        public int Delete(int Id)
        {
            try
            {
                return _proAccBonusDao.Delete(Id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAccumulateBonusMgr-->Delete-->" + ex.Message, ex);
            }
        }

        public List<Model.Query.PromotionsAccumulateBonusQuery> Query(Model.Query.PromotionsAccumulateBonusQuery store, out int totalCount)
        {
            try
            {
                List<Model.Query.PromotionsAccumulateBonusQuery> stores = _proAccBonusDao.Query(store, out totalCount);
                if (stores.Count > 0)
                {
                    ParametersrcDao _pDa = new ParametersrcDao(conn);
                    List<Model.Parametersrc> alist = _pDa.QueryParametersrcByTypes("payment");
                    foreach (var item in stores)
                    {
                        string[] strids = item.payment_code.Split(',');

                        for (int i = 0; i < strids.Length; i++)
                        {
                            var clist = alist.Find(m => m.ParameterCode == strids[i].ToString());

                            if (clist != null)
                            {
                                item.payment_name += clist.parameterName + ",";
                            }
                        }
                        item.payment_name = item.payment_name.TrimEnd(',');
                    }
                }
                return stores;
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAccumulateBonusMgr-->Query-->" + ex.Message, ex);
            }
        }

        public Model.PromotionsAccumulateBonus GetModel(int id)
        {
            try
            {
                return _proAccBonusDao.GetModel(id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAccumulateBonusMgr-->GetModel-->" + ex.Message, ex);
            }
        }
    }
}
