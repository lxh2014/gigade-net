/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromotionsBonusSerialHistoryMgr.cs
* 摘 要：
* 點數抵用與資料庫交互方法  
* 当前版本：v1.1
* 作 者：dongya0410j    
* 完成日期：2014/6/20 
* 修改歷史:
*         v1.1修改日期：2014/8/15 
*         v1.1修改人員：hongfei0416j 
*         v1.1修改内容：合并代碼，添加注釋 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class PromotionsBonusSerialHistoryMgr : IPromotionsBonusSerialHistoryImplMgr
    {
        private IPromotionsBonusSerialHistoryImplDao _bonusDao;
        public PromotionsBonusSerialHistoryMgr(string connectionString)
        {
            _bonusDao = new PromotionsBonusSerialHistoryDao(connectionString);
        }

        public List<PromotionsBonusSerialHistoryQuery> QueryById(int id)
        {
            try
            {
                return _bonusDao.QueryById(id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsBonusSerialHistoryMgr-->QueryById-->" + ex.Message, ex);
            }
        }
        public List<Model.Query.PromotionsBonusSerialHistoryQuery> QueryById(PromotionsBonusSerialHistoryQuery query, out int TotalCount)
        {
            try
            {
                return _bonusDao.QueryById(query,out TotalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsBonusSerialHistoryMgr-->QueryById-->" + ex.Message, ex);
            }
        }
    }
}
