/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromotionsBonusSerialHistoryDao.cs
* 摘 要：
* 序號兌換
* 当前版本：v1.1
* 作 者：dongya0410j    
* 完成日期：2014/6/20 
* 修改歷史:
*         v1.1修改日期：2014/8/15 
*         v1.1修改人員：dongya0410j
*         v1.1修改内容：在抛出異常的時候將Sql語句抛出，合并代碼，添加注釋 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao
{
    public class PromotionsBonusSerialHistoryDao : IPromotionsBonusSerialHistoryImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        public PromotionsBonusSerialHistoryDao(string connectionString)
        {
            // TODO: Complete member initialization  this
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        #region QueryById +List<Model.Query.PromotionsBonusSerialHistoryQuery> QueryById(int id)
        public List<Model.Query.PromotionsBonusSerialHistoryQuery> QueryById(int id)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@" SELECT id,serial,promotion_id,created,pseh.user_id,user_email from promotions_bonus_serial_history  pseh ");
                sb.AppendFormat(@" LEFT JOIN users u on pseh.user_id=u.user_id where `promotion_id` = '{0}'", id);
                return _access.getDataTableForObj<PromotionsBonusSerialHistoryQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsBonusSerialHistoryDao-->QueryById-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
    }
}
