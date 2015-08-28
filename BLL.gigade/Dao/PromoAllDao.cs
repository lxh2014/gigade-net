/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromoAllDao.cs
* 摘 要：
* 點數抵用與資料庫交互方法  
* 当前版本：v1.2
* 作 者：hongfei0416j    
* 完成日期：2014/6/20 
* 修改歷史:
*         v1.1修改日期：2014/8/15 
*         v1.1修改人員：zhejiang0304j 
*         v1.1修改内容：在抛出異常的時候將Sql語句抛出，合并代碼，添加注釋 
*         v1.2修改日期：2014/09/15
*         v1.2修改人員：hongfei0416j
*         v1.2修改内容：設置更新的時候的安全模式     
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Common;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Dao
{
    public class PromoAllDao : IPromoAllImplDao
    {
        private IDBAccess _access;
        private string connStr;
        public PromoAllDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
        }
        #region 根據查詢條件獲取促銷活動列表+List<PromoAll> GetList(PromoAllQuery query)
        /// <summary>
        /// 根據查詢條件獲取促銷活動列表
        /// </summary>
        /// <param name="query">條件model</param>
        /// <returns>促銷活動列表</returns>
        public List<PromoAll> GetList(PromoAllQuery query)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("select rid,event_id,event_type,brand_id,class_id,category_id,product_id,start as startTime,end,kuser,kdate,muser,mdate,status ");
            sbSql.Append(" from promo_all where 1=1 ");
            if (query.status != 0)
            {
                sbSql.AppendFormat(" and status={0} ", query.status);
            }
            if (query.brand_id != 0)
            {
                sbSql.AppendFormat(" and brand_id={0} ", query.brand_id);
            }
            if (query.category_id != 0)
            {
                sbSql.AppendFormat(" and category_id={0} ", query.category_id);
            }
            try
            {
                return _access.getDataTableForObj<PromoAll>(sbSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("PromoAllDao-->GetList-->" + ex.Message + sbSql.ToString(), ex);
            }
            
        } 
        #endregion

        #region 保存新增promo_all表的數據+int Save(PromoAll model)
        /// <summary>
        /// 保存新增promo_all表的數據
        /// </summary>
        /// <param name="model">PromoAll的Model</param>
        /// <returns></returns>
        public int Save(PromoAll model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            sql.Append(@" insert into promo_all (");
            sql.Append(" rid, event_id,event_type,brand_id,class_id,category_id,product_id,start,end,kuser,kdate,muser,mdate,status) ");
            sql.AppendFormat(" values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}');", model.rid, model.event_id, model.event_type, model.brand_id, model.class_id, model.category_id, model.product_id, CommonFunction.DateTimeToString(model.startTime), Common.CommonFunction.DateTimeToString(model.end), model.kuser, Common.CommonFunction.DateTimeToString(model.kdate), model.muser, Common.CommonFunction.DateTimeToString(model.mdate), model.status);
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("PromoAllDao-->Save-->" + ex.Message + sql.ToString(), ex);
            }

        } 
        #endregion

        #region 刪除promo_all表的數據+int Delete(PromoAll model)
        /// <summary>
        /// 刪除promo_all表的數據
        /// </summary>
        /// <param name="model">promo_all的Model</param>
        /// <returns></returns>
        public int Delete(PromoAll model)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("set sql_safe_updates = 0;");
            sql.AppendFormat("update promo_all set status=1 where event_id='{0}'", model.event_id);
            sql.Append("set sql_safe_updates = 1;");
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoAllDao-->Delete-->" + ex.Message + sql.ToString(), ex);
            }
            
        } 
        #endregion

        #region 事物所用返回相應的sql語句

        #region 保存更新promo_all的數據的sql語句+string UpdatePromAll(PromoAll model)
        /// <summary>
        /// 保存更新promo_all的數據的sql語句
        /// </summary>
        /// <param name="model"></param>
        /// <returns>保存更新promo_all的數據的sql語句</returns>
        public string UpdatePromAll(PromoAll model)
        {
            model.Replace4MySQL();

            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("set sql_safe_updates = 0;");
                sql.Append(@"update  promo_all  set ");
                sql.AppendFormat(" event_type='{0}',brand_id='{1}',class_id='{2}', category_id='{3}',product_id='{4}',",
                    model.event_type, model.brand_id, model.class_id, model.category_id, model.product_id);
                sql.AppendFormat(" start='{0}',end='{1}',muser='{2}',mdate='{3}',status='{4}'",
                    CommonFunction.DateTimeToString(model.startTime), Common.CommonFunction.DateTimeToString(model.end),
                    model.muser,
                    Common.CommonFunction.DateTimeToString(model.mdate), model.status);
                sql.AppendFormat("where event_id='{0}';", model.event_id);
                sql.Append("set sql_safe_updates = 1;");
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromoAllDao-->UpdatePromAll-->" + ex.Message + sql.ToString(), ex);
            }
        } 
        #endregion
        #region 軟刪除promo_all數據的sql語句+string DeletePromAll(string event_id)
        /// <summary>
        /// 軟刪除promo_all數據的sql語句
        /// </summary>
        /// <param name="event_id">活動id</param>
        /// <returns>軟刪除promo_all數據的sql語句</returns>
        public string DeletePromAll(string event_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("set sql_safe_updates = 0;");
                sql.AppendFormat("update promo_all set status=0 where event_id='{0}'; ", event_id);
                sql.Append("set sql_safe_updates = 1;");
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromoAllDao-->DeletePromAll-->" + ex.Message + sql.ToString(), ex);
            }

        } 
        #endregion
        #region 硬刪除promo_all數據的sql語句+string DelPromAll(string event_id)
        /// <summary>
        ///硬刪除promo_all數據的sql語句
        /// </summary>
        /// <param name="event_id">活動id</param>
        /// <returns>硬刪除promo_all數據的sql語句</returns>
        public string DelPromAll(string event_id)
        {
            StringBuilder sql = new StringBuilder(); 
            sql.Append("set sql_safe_updates = 0;");
            sql.AppendFormat("delete from promo_all where event_id='{0}'; ", event_id);
            sql.Append("set sql_safe_updates = 1;");
            return sql.ToString();  
        } 
        #endregion
        #region 保存新增promo_all表數據的sql語句+string SavePromAll(PromoAll model)
        /// <summary>
        /// 保存新增promo_all表數據的sql語句
        /// </summary>
        /// <param name="model"></param>
        /// <returns>保存新增promo_all表數據的sql語句</returns>
        public string SavePromAll(PromoAll model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(@" insert into promo_all (");
                sql.Append("  event_id,event_type,brand_id,class_id,category_id,product_id,start,end,kuser,kdate,muser,mdate,status) ");
                sql.AppendFormat(" values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}');", model.event_id, model.event_type, model.brand_id, model.class_id, model.category_id, model.product_id, CommonFunction.DateTimeToString(model.startTime), Common.CommonFunction.DateTimeToString(model.end), model.kuser, Common.CommonFunction.DateTimeToString(model.kdate), model.muser, Common.CommonFunction.DateTimeToString(model.mdate), model.status);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromoAllDao-->SavePromAll-->" + ex.Message + sql.ToString(), ex);
            }
        } 
        #endregion
        #endregion
    }
}
