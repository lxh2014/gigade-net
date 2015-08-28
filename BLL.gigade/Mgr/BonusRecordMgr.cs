/* 
 * 武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：BonusRecordMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作    者：zhuoqin0830w 
 * 完成日期：2015/08/25
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class BonusRecordMgr : IBonusRecordImplMgr
    {
        private IBonusRecordImplDao _bonusRecordDao;
        public BonusRecordMgr(string connectionStr)
        {
            _bonusRecordDao = new BonusRecordDao(connectionStr);
        }

        #region 得到 bonus_record 裱中 最大的 主鍵  add by zhuoqin0830w 2015/08/25
        /// <summary>
        /// 得到 bonus_record 裱中 最大的 主鍵
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public uint GetRecordID()
        {
            try
            {
                return _bonusRecordDao.GetRecordID();
            }
            catch (Exception ex)
            {
                throw new Exception("BonusRecordMgr-->GetRecordID()-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 向 bonus_record 裱中添加 數據  add by zhuoqin0830w 2015/08/25
        /// <summary>
        /// 向 bonus_record 裱中添加 數據
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public string InsertBonusRecord(BonusRecord br)
        {
            try
            {
                return _bonusRecordDao.InsertBonusRecord(br);
            }
            catch (Exception ex)
            {
                throw new Exception("BonusRecordMgr-->InsertBonusRecord(BonusRecord br)-->" + ex.Message, ex);
            }
        }
        #endregion
    }
}
