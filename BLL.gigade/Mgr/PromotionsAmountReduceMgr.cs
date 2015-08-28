#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromotionsBonusSerialMgr.cs
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
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class PromotionsAmountReduceMgr : IPromotionsAmountReduceImplMgr
    {
        IPromotionsAmountReduceImplDao _promoAmountReduceDao;
        public PromotionsAmountReduceMgr(string connectionStr)
        {
            _promoAmountReduceDao = new PromotionsAmountReduceDao(connectionStr);

        }
        public string Save(PromotionsAmountReduce model)
        {
            try
            {
                return _promoAmountReduceDao.Save(model);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountReduceMgr-->Save-->" + ex.Message, ex);
            }
        }

        public string Update(PromotionsAmountReduce model)
        {
            try
            {
                return _promoAmountReduceDao.Update(model);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountReduceMgr-->Update-->" + ex.Message, ex);
            }
        }

        public string Delete(int id)
        {
            try
            {
                return _promoAmountReduceDao.Delete(id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountReduceMgr-->Delete-->" + ex.Message, ex);
            }
        }
    }
}
