#region 文件信息
/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：PromoDiscountDao.cs
 * 摘   要： 
 *      
 * 当前版本：v1.2 
 * 作   者： dongya0410j
 * 完成日期：2014/6/20
 * 修改歷史：
 *      v1.1修改日期：2014/8/15
 *      v1.1修改人員：dongya0410j
 *      v1.1修改内容：代碼合併
 *      v1.2修改日期：2014/09/15
 *      v1.2修改人員：hongfei0416j
 *      v1.2修改内容：設置更新的時候的安全模式
 */
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Common;
using System.Data;

namespace BLL.gigade.Dao
{
    class PromoDiscountDao : IPromoDiscountImplDao
    {
        private IDBAccess _access;
        public PromoDiscountDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);

        }

        #region 獲取promo_discount +List<PromoDiscount> GetPromoDiscount(PromoDiscount model)
        public List<PromoDiscount> GetPromoDiscount(PromoDiscount model)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                List<PromoDiscount> list = new List<PromoDiscount>();
                sb.Append(@"select rid,event_id,quantity,discount,special_price from promo_discount  where 1=1 ");
                sb.AppendFormat(" and status=1 and event_id='{0}' order by quantity;", model.event_id);
                return _access.getDataTableForObj<PromoDiscount>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoDiscountDao-->GetPromoDiscount-->" + ex.Message + sb.ToString(), ex);
            }

        } 
        #endregion

        #region 根據id刪除promo_discount +int DeleteByRid(int rid)
        /// <summary>
        /// 根據id刪除promo_discount 
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        public int DeleteByRid(int rid)
        {
            string sql = "update promo_discount set status=0 where rid= '" + rid + "';";
            try
            {
                return _access.execCommand(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoDiscountDao-->DeleteByRid-->" + ex.Message + sql.ToString(), ex);
            }
        } 
        #endregion

        #region 將status狀態修改爲0，即軟刪除 +int DeleteByEventid(PromoDiscount model)
        /// <summary>
        /// 將status狀態修改爲0，即軟刪除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int DeleteByEventid(PromoDiscount model)
        {
            string sql = "update promo_discount set status=0 where event_id= '" + model.event_id + "';";
            try
            {
                return _access.execCommand(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoDiscountDao-->DeleteByEventid-->" + ex.Message + sql.ToString(), ex);
            }
        } 
        #endregion

        #region 保存數據 int Save(PromoDiscount model)
        /// <summary>
        /// 保存數據
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Save(PromoDiscount model)
        {
            model.Replace4MySQL();
            StringBuilder sbSql = new StringBuilder();
            try
            {

                sbSql.Append(@" insert into promo_discount (");
                sbSql.Append(" event_id, quantity,discount,special_price,kuser,kdate,muser,mdate,status) ");
                sbSql.AppendFormat(" values('{0}',{1},{2},{3},'{4}','{5}','{6}','{7}','{8}');", model.event_id, model.quantity, model.discount, model.special_price, model.kuser, Common.CommonFunction.DateTimeToString(model.kdate), model.muser, Common.CommonFunction.DateTimeToString(model.mdate), model.status);
                return _access.execCommand(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoDiscountDao-->Save-->" + ex.Message + sbSql.ToString(), ex);
            }
        } 
        #endregion

        #region 刪除數據 +int Update(PromoDiscount model)
        /// <summary>
        /// 刪除數據 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(PromoDiscount model)
        {
            model.Replace4MySQL();
            StringBuilder sbSql = new StringBuilder();
            try
            {

                sbSql.Append(@" update promo_discount set ");
                sbSql.AppendFormat(" event_id='{0}', quantity={1},discount={2},special_price={3},kuser='{4}',kdate='{5}',muser='{6}',mdate='{7}' where rid={8};", model.event_id, model.quantity, model.discount, model.special_price, model.kuser, Common.CommonFunction.DateTimeToString(model.kdate), model.muser, Common.CommonFunction.DateTimeToString(model.mdate), model.rid);
                return _access.execCommand(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoDiscountDao-->Update-->" + ex.Message + sbSql.ToString(), ex);
            }

        } 
        #endregion

        #region GetLimitByEventId +DataTable GetLimitByEventId(string event_id, int rid)
        /// <summary>
        /// GetLimitByEventId 
        /// </summary>
        /// <param name="event_id"></param>
        /// <param name="rid"></param>
        /// <returns></returns>
        public DataTable GetLimitByEventId(string event_id, int rid)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" SELECT MIN(quantity) 'minQ', MAX(quantity) 'maxQ',MIN(discount) 'minD',MAX(discount) 'maxD',MIN(special_price) 'minS',MAX(special_price) 'maxS' from promo_discount where 1=1 and status=1");
                sql.AppendFormat(" and event_id='{0}'", event_id);
                if (rid != 0)
                {
                    sql.AppendFormat(" and rid <> '{0}';", rid);
                }
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoDiscountDao-->GetLimitByEventId-->" + ex.Message + sql.ToString(), ex);
            }

        } 
        #endregion

        #region DeleteStr +string DeleteStr(string event_id)
        /// <summary>
        /// 刪除 
        /// </summary>
        /// <param name="event_id"></param>
        /// <returns></returns>
        public string DeleteStr(string event_id)
        {
            string sql = "update promo_discount set status=0 where event_id= '" + event_id + "';";
            return sql.ToString();
        } 
        #endregion
        
    }
}
