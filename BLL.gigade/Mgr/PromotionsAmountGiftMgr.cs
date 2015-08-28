#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromotionsAmountGiftMgr.cs
* 摘 要：
* 滿額滿件送禮mgr
* 当前版本：v1.1
* 作 者： mengjuan0826j
* 完成日期：2014/6/20 
* 修改歷史：
*         v1.1修改日期：2014/8/15 
*         v1.1修改人員：mengjuan0826j 
*         v1.1修改内容：在抛出異常的時候將Sql語句抛出，合并代碼，添加注釋 
 *        v1.2修改日期：2015/3/16 
*             修改人員：shuangshuang0420j
*             修改内容：完善否專區時寫入字段
 *                      贈送購物金或抵用券時自動生成gift_id
*/

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class PromotionsAmountGiftMgr : IPromotionsAmountGiftImplMgr
    {
        private IPromotionsAmountGiftImplDao _pagDao;
        public PromotionsAmountGiftMgr(string connectionstring)
        {
            _pagDao = new PromotionsAmountGiftDao(connectionstring);
        }
        public List<Model.Query.PromotionsAmountGiftQuery> Query(Model.Query.PromotionsAmountGiftQuery query, out int totalCount, string type)
        {
            try
            {
                return _pagDao.Query(query, out totalCount, type);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountGiftMgr-->Query-->" + ex.Message, ex);
            }
        }

        public int Save(PromotionsAmountGiftQuery query)
        {
            try
            {
                return _pagDao.Save(query);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountGiftMgr-->Save-->" + ex.Message, ex);
            }
        }

        public int Update(PromotionsAmountGiftQuery query, string oldeventid)
        {
            try
            {
                return _pagDao.Update(query, oldeventid);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountGiftMgr-->Update-->" + ex.Message, ex);
            }
        }

        public int Delete(int id, string event_id)
        {
            try
            {
                return _pagDao.Delete(id, event_id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountGiftMgr-->Delete-->" + ex.Message, ex);
            }
        }


        public Model.PromotionsAmountGift GetModel(int id)
        {
            try
            {
                return _pagDao.GetModel(id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountGiftMgr-->GetModel-->" + ex.Message, ex);
            }
        }


        public DataTable SelectDt(PromotionsAmountGift model)
        {
            try
            {
                return _pagDao.SelectDt(model);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountGiftMgr-->SelectDt-->" + ex.Message, ex);
            }
        }

        public PromotionsAmountGiftQuery Select(int id)
        {
            try
            {
                return _pagDao.Select(id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountGiftMgr-->Select-->" + ex.Message, ex);
            }
        }


        public int UpdateActive(PromotionsAmountGiftQuery model)
        {
            try
            {
                return _pagDao.UpdateActive(model);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountGiftMgr-->UpdateActive-->" + ex.Message, ex);
            }
        }


        public int TryEatAndDiscountSave(PromotionsAmountGiftQuery query)
        {
            try
            {
                return _pagDao.TryEatAndDiscountSave(query);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountGiftMgr-->TryEatAndDiscountSave-->" + ex.Message, ex);
            }
        }

        public int TryEatAndDiscountUpdate(PromotionsAmountGiftQuery query, string oldeventid)
        {
            try
            {
                return _pagDao.TryEatAndDiscountUpdate(query, oldeventid);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountGiftMgr-->TryEatAndDiscountUpdate-->" + ex.Message, ex);
            }
        }

    }
}
