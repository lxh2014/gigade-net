/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 文件名称：PromotionsAmountDiscountDao.cs
* 摘 要：購物金活動管理與資料庫交互類
* 当前版本：v1.2
* 作 者： yunlong0726h
* 完成日期：2014/6/20
* 修改歷史：
*         v1.2修改日期：2014/8/15 
*         v1.2修改人員：yunlong0726h
*         v1.2修改内容：在抛出異常的時候將Sql語句抛出，合并代碼，添加注釋 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl; 
using DBAccess;
using System.Data;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
using BLL.gigade.Common;

namespace BLL.gigade.Dao
{
    /// <summary>
    /// 購物金活動管理數據庫交互類
    /// </summary>
    public class PromotionsAccumulateBonusDao : IPromotionsAccumulateBonusImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;

        #region 有參構造函數
        /// <summary>
        /// 有參構造函數
        /// </summary>
        /// <param name="connectionString">數據庫連接字符串</param>
        public PromotionsAccumulateBonusDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        #endregion

        #region 新增 +int Save(Model.PromotionsAccumulateBonus promoAccumulateBonus)
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="promoAccumulateBonus">購物金活動對象</param>
        /// <returns>數據庫受影響行數</returns>
        public int Save(Model.PromotionsAccumulateBonus promoAccumulateBonus)
        {
            StringBuilder sb = new StringBuilder();
            promoAccumulateBonus.Replace4MySQL();
            sb.AppendFormat(@"insert into promotions_accumulate_bonus(name,group_id,start,end,bonus_rate,extra_point,bonus_expire_day,new_user,`repeat`,present_time,created,modified,active,muser,event_desc,event_type,condition_id,device,payment_code,kuser,new_user_date,status) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}')",
                promoAccumulateBonus.name, promoAccumulateBonus.group_id, CommonFunction.DateTimeToString(promoAccumulateBonus.startTime), CommonFunction.DateTimeToString(promoAccumulateBonus.end), promoAccumulateBonus.bonus_rate, promoAccumulateBonus.extra_point,
                promoAccumulateBonus.bonus_expire_day, promoAccumulateBonus.new_user == true ? 1 : 0, promoAccumulateBonus.repeat, promoAccumulateBonus.present_time, CommonFunction.DateTimeToString(promoAccumulateBonus.created), CommonFunction.DateTimeToString(promoAccumulateBonus.modified),
                promoAccumulateBonus.active == true ? 1 : 0, promoAccumulateBonus.muser, promoAccumulateBonus.event_desc, promoAccumulateBonus.event_type, promoAccumulateBonus.condition_id, promoAccumulateBonus.device, promoAccumulateBonus.payment_code, promoAccumulateBonus.kuser,
                CommonFunction.DateTimeToString(promoAccumulateBonus.new_user_date), promoAccumulateBonus.status);
            try
            {
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAccumulateBonusDao-->Save-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 修改 +int Update(Model.PromotionsAccumulateBonus promoAccumulateBonus)
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="promoAccumulateBonus">購物金活動對象</param>
        /// <returns>數據庫受影響行數</returns>
        public int Update(Model.PromotionsAccumulateBonus promoAccumulateBonus)
        {
            StringBuilder sb = new StringBuilder();
            promoAccumulateBonus.Replace4MySQL();
            sb.AppendFormat("update promotions_accumulate_bonus set `name`='{1}',`group_id`='{2}',`start`='{3}',`end`='{4}',`bonus_rate`='{5}',`extra_point`='{6}',`bonus_expire_day`='{7}',`new_user`='{8}',`repeat`='{9}',`present_time`='{10}',`created`='{11}',`modified`='{12}',`active`='{13}',`muser`='{14}',`event_desc`='{15}',`event_type`='{16}',`condition_id`='{17}',`device`='{18}',`payment_code`='{19}',`kuser`='{20}',`new_user_date`='{21}',`status`='{22}' where `id`={0}",
                promoAccumulateBonus.id, promoAccumulateBonus.name, promoAccumulateBonus.group_id, CommonFunction.DateTimeToString(promoAccumulateBonus.startTime), CommonFunction.DateTimeToString(promoAccumulateBonus.end), promoAccumulateBonus.bonus_rate, promoAccumulateBonus.extra_point, promoAccumulateBonus.bonus_expire_day, promoAccumulateBonus.new_user == true ? 1 : 0, promoAccumulateBonus.repeat, promoAccumulateBonus.present_time, CommonFunction.DateTimeToString(promoAccumulateBonus.created),
                CommonFunction.DateTimeToString(promoAccumulateBonus.modified), promoAccumulateBonus.active == true ? 1 : 0, promoAccumulateBonus.muser, promoAccumulateBonus.event_desc, promoAccumulateBonus.event_type, promoAccumulateBonus.condition_id, promoAccumulateBonus.device, promoAccumulateBonus.payment_code, promoAccumulateBonus.kuser, CommonFunction.DateTimeToString(promoAccumulateBonus.new_user_date), promoAccumulateBonus.status);
            try
            {
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAccumulateBonusDao-->Update-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 根據編號刪除購物金活動 +int Delete(int Id)
        /// <summary>
        /// 根據編號刪除購物金活動
        /// </summary>
        /// <param name="Id">購物金活動編號</param>
        /// <returns>數據庫受影響的行數</returns>
        public int Delete(int Id)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("update promotions_accumulate_bonus set status={0} where id={1}", 0, Id.ToString());
            try
            {
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAccumulateBonusDao-->Delete-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 根據條件查詢購物金活動列表 +List<Model.Query.PromotionsAccumulateBonusQuery> Query(Model.Query.PromotionsAccumulateBonusQuery query, out int totalCount)
        /// <summary>
        /// 根據條件查詢購物金活動列表
        /// </summary>
        /// <param name="query">購物金活動查詢條件對象</param>
        /// <param name="totalCount">返回數據總條數</param>
        /// <returns>購物金活動列表</returns>
        public List<Model.Query.PromotionsAccumulateBonusQuery> Query(Model.Query.PromotionsAccumulateBonusQuery query, out int totalCount)
        {
            query.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            StringBuilder sbw = new StringBuilder("");
            try
            {
                sb.Append(@"SELECT PB.id, PB.name, PB.group_id, PB.start as startTime, PB.end, PB.bonus_rate, PB.extra_point, PB.bonus_expire_day, PB.new_user, PB.repeat, PB.present_time, PB.created, PB.modified, PB.active, PB.muser, PB.event_desc, PB.event_type, PB.condition_id, PB.device, PB.payment_code, PB.kuser, PB.new_user_date,VUG.group_name,mu.user_username");
                sbw.Append(" FROM promotions_accumulate_bonus AS PB  left join vip_user_group as VUG on PB.group_id=VUG.group_id LEFT JOIN manage_user mu ON PB.muser=mu.user_id  WHERE status = '1'");

                if (query.expired == 1)
                {
                    sbw.AppendFormat(" and end >= '{0}'", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    sbw.AppendFormat(" and end <= '{0}'", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                totalCount = 0;
                if (query.IsPage)
                {
                    DataTable dt = _access.getDataTable(@"SELECT count(PB.id) as totalCount " + sbw.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0]["totalCount"]);
                    }
                    sbw.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                sb.Append(sbw.ToString());

                return _access.getDataTableForObj<PromotionsAccumulateBonusQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAccumulateBonusDao-->Query-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 根據編號獲取購物金活動 +Model.PromotionsAccumulateBonus GetModel(int id)
        /// <summary>
        /// 根據編號獲取購物金活動
        /// </summary>
        /// <param name="id">購物金活動編號</param>
        /// <returns>購物金活動對象</returns>
        public Model.PromotionsAccumulateBonus GetModel(int id)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"SELECT `id`,`name`,`group_id`,`start` as `startTime`,`end`,`bonus_rate`,`extra_point`,`bonus_expire_day`,`new_user`,`repeat`,`present_time`,`created`,`modified`,`active`,`muser`,`event_desc`,`event_type`,`condition_id`,`device`,`payment_code`,`kuser`,`new_user_date`,`status` FROM `promotions_accumulate_bonus` where id={0}", id);
            try
            {
                return _access.getSinggleObj<PromotionsAccumulateBonus>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAccumulateBonusDao-->GetModel-->sql:" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
    }
}
