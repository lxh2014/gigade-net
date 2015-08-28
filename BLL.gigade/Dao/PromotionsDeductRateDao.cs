/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromotionsDeductRateDao.cs
* 摘 要：
* 點數抵用與資料庫交互方法  
* 当前版本：v1.1
* 作 者：dongya0410j    
* 完成日期：2014/6/20 
* 修改歷史:
*         v1.1修改日期：2014/8/15 
*         v1.1修改人員：zhejiang0304j 
*         v1.1修改内容：在抛出異常的時候將Sql語句抛出，合并代碼，添加注釋 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
using BLL.gigade.Common;

namespace BLL.gigade.Dao
{
   public class PromotionsDeductRateDao : IPromotionsDeductRateImplDao
    {
        private IDBAccess _access;
        public PromotionsDeductRateDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        #region 查詢點數抵用列表頁的數據+List<Model.Query.PromotionsDeductRateQuery> QueryAll(Model.Query.PromotionsDeductRateQuery query, out int total)
        /// <summary>
        /// 查詢點數抵用列表頁的數據
        /// </summary>
        /// <param name="query">點數抵用的Model</param>
        /// <param name="total">數據的總數</param>
        /// <returns></returns>
        public List<Model.Query.PromotionsDeductRateQuery> QueryAll(Model.Query.PromotionsDeductRateQuery query, out int total)
        {
            query.Replace4MySQL();
            string sql = "SELECT `PDR`.`id`,`PDR`.`active`, `PDR`.`name`,`PDR`.`condition_id`,`PDR`.`group_id`, `PDR`.`amount`, `PDR`.`bonus_type`,CONCAT( `PDR`.`dollar`,'/',`PDR`.`point`) as points,`PDR`.`rate`,PDR.dollar,PDR.point, `PDR`.`start` as startdate, `PDR`.`end` , `PDR`.`created`, `PDR`.`modified`, `PDR`.`status`,VUG.group_name,PDR.muser,mu.user_username ";
            string sqlfrom = "FROM `promotions_deduct_rate` AS `PDR` left join vip_user_group as VUG on PDR.group_id=VUG.group_id LEFT JOIN manage_user mu ON PDR.muser=mu.user_id WHERE `status` = 1 ";
            if (query.expired == 0)
            {
                sqlfrom += " AND `end` <'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }
            else
            {
                sqlfrom += " AND `end` >'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }
            total = 0;
            string sqlfrom2 = sqlfrom + " order by PDR.id desc " + " limit " + query.Start + "," + query.Limit;
            System.Data.DataTable _dt = _access.getDataTable("select count(PDR.id) as total " + sqlfrom);
            if (_dt != null)
            {
                total = Convert.ToInt32(_dt.Rows[0]["total"]);
            }
            try
            {
                return _access.getDataTableForObj<PromotionsDeductRateQuery>(sql + sqlfrom2);
            }
            catch (Exception ex)
            {

                throw new Exception("PromotionsDeductRateDao-->QueryAll-->" + ex.Message + sql + sqlfrom2, ex);
            }
           
        } 
        #endregion

        #region 保存點數抵用新增數據+int Save(Model.PromotionsDeductRate rate)
        /// <summary>
        /// 保存點數抵用新增數據
        /// </summary>
        /// <param name="rate">點數抵用Model</param>
        /// <returns></returns>
        public int Save(Model.PromotionsDeductRate rate)
        {
            rate.Replace4MySQL();
            string strSql = string.Format("insert into `promotions_deduct_rate`(`name`, `group_id`, `amount`, `bonus_type`,`dollar`,`point`,`rate`, `start`, `end` ,`created`,`modified`,`condition_id`,active,status,kuser,muser) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}',{12},'{13}',{14},{15})", rate.name, rate.group_id, rate.amount, rate.bonus_type, rate.dollar, rate.point, rate.rate, CommonFunction.DateTimeToString(rate.start), CommonFunction.DateTimeToString(rate.end), CommonFunction.DateTimeToString(DateTime.Now), CommonFunction.DateTimeToString(DateTime.Now), rate.condition_id, rate.active, rate.status,rate.kuser,rate.muser);
            try
            {
                return _access.execCommand(strSql);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsDeductRateDao-->Save-->" + ex.Message + strSql, ex);
            }
        } 
        #endregion

        #region 保存點數抵用更新數據+int Update(Model.PromotionsDeductRate rate)
        /// <summary>
        /// 保存點數抵用更新數據
        /// </summary>
        /// <param name="rate">點數抵用Model</param>
        /// <returns></returns>
        public int Update(Model.PromotionsDeductRate rate)
        {
            rate.Replace4MySQL();
            string strSql = string.Format("update `promotions_deduct_rate` set `name`='{1}',`group_id`='{2}', `amount`='{3}', `bonus_type`='{4}',`dollar`='{5}',`point`='{6}',`rate`='{7}', `start`='{8}', `end`='{9}' , `modified`='{10}',condition_id='{11}',`active`={12},muser={13} where id={0} ", rate.id, rate.name, rate.group_id, rate.amount, rate.bonus_type, rate.dollar, rate.point, rate.rate, CommonFunction.DateTimeToString(rate.start), CommonFunction.DateTimeToString(rate.end), CommonFunction.DateTimeToString(DateTime.Now), rate.condition_id,rate.active,rate.muser);
            try
            {
                return _access.execCommand(strSql);
            }
            catch (Exception ex)
            {

                throw new Exception("PromotionsDeductRateDao-->Update-->" + ex.Message + strSql, ex);
            }
            
        } 
        #endregion

        #region 刪除點數抵用數據+int Delete(Model.PromotionsDeductRate rate)
        /// <summary>
        /// 刪除點數抵用數據
        /// </summary>
        /// <param name="rate">點數抵用的Model</param>
        /// <returns></returns>
        public int Delete(Model.PromotionsDeductRate rate)
        {
            string strSql = "update `promotions_deduct_rate` set status=0,muser="+rate.muser+" where id=" + rate.id;
            try
            {
                return _access.execCommand(strSql);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsDeductRateDao-->Delete-->" + ex.Message + strSql, ex);
            }
            
        } 
        #endregion

        #region 獲取一條數據根據編號+Model.PromotionsDeductRate GetModel(int id)
        /// <summary>
        /// 獲取一條數據根據編號
        /// </summary>
        /// <param name="id">編號</param>
        /// <returns></returns>
        public Model.PromotionsDeductRate GetModel(int id)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("select `PDR`.`id`,`PDR`.`active`, `PDR`.`name`,`PDR`.`group_id`,`PDR`.`bonus_type` from `promotions_deduct_rate` AS `PDR` where id='{0}'", id);
            try
            {
                return _access.getSinggleObj<PromotionsDeductRate>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsDeductRateDao-->GetModel-->" + ex.Message + sb.ToString(), ex);
            }
        } 
        #endregion

        #region 更改活動狀態+int UpdateActive(PromotionsDeductRate store)
        /// <summary>
        /// 更改活動狀態
        /// </summary>
        /// <param name="store">點數抵用的Model</param>
        /// <returns></returns>
        public int UpdateActive(PromotionsDeductRate store)
        {
            StringBuilder sb = new StringBuilder();
            store.Replace4MySQL();
            sb.AppendFormat("update `promotions_deduct_rate` AS `PDR` set `PDR`.`active` ={0},PDR.modified='{1}',PDR.muser={2} where `PDR`.`id`={3}", store.active,CommonFunction.DateTimeToString(store.modified),store.muser, store.id);
            try
            {
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsDeductRateDao-->UpdateActive-->" + ex.Message + sb.ToString(), ex);
            }
        }
    } 
        #endregion
}
