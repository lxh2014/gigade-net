/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromotionsBonusDao.cs
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
using System.Data;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
using BLL.gigade.Common;

namespace BLL.gigade.Dao
{
   
   public class PromotionsBonusDao : IPromotionsBonusImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        public PromotionsBonusDao(string connectionString) 
        {

            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        #region 序號兌換 保存 +int Save(Model.PromotionsBonus promoBonus)
        public int Save(Model.PromotionsBonus promoBonus)
        {
            promoBonus.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"insert into promotions_bonus(name,group_id,type,amount,new_user,multiple,start,end,created,modified,days,`repeat`,`condition_id`,active,kuser,muser) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}',{13},{14},{15})",
                    promoBonus.name, promoBonus.group_id, promoBonus.type, promoBonus.amount, promoBonus.new_user == true ? "1" : "0", promoBonus.multiple == true ? "1" : "0", CommonFunction.DateTimeToString(promoBonus.start),
                    CommonFunction.DateTimeToString(promoBonus.end), CommonFunction.DateTimeToString(promoBonus.created), CommonFunction.DateTimeToString(promoBonus.modified), promoBonus.days, promoBonus.repeat == true ? "1" : "0", promoBonus.condition_id, promoBonus.active,promoBonus.kuser,promoBonus.muser);

                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsBonusDao-->Save-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 序號兌換 編輯 +int Update(Model.PromotionsBonus promoBonus)
        public int Update(Model.PromotionsBonus promoBonus)
        {
            StringBuilder sb = new StringBuilder();
            promoBonus.Replace4MySQL();
            try
            {
                sb.AppendFormat("update promotions_bonus  set name='{1}',group_id='{2}',type='{3}',amount='{4}',new_user='{5}',`repeat`='{6}',multiple='{7}',start='{8}',end='{9}',modified='{10}',condition_id='{11}',active={12},days={13},muser={14} where id={0}", promoBonus.id, promoBonus.name, promoBonus.group_id, promoBonus.type, promoBonus.amount, promoBonus.new_user == true ? "1" : "0", promoBonus.repeat == true ? "1" : "0", promoBonus.multiple == true ? "1" : "0", CommonFunction.DateTimeToString(promoBonus.start), CommonFunction.DateTimeToString(promoBonus.end), CommonFunction.DateTimeToString(promoBonus.modified), promoBonus.condition_id, promoBonus.active, promoBonus.days,promoBonus.muser);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsBonusDao-->Update-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 序號兌換 刪除 +int Delete(Model.PromotionsBonus pId)
        public int Delete(Model.PromotionsBonus pId)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("update promotions_bonus set status={0},muser={2},modified='{3}' where id={1}", 0, pId.id, pId.muser,CommonFunction.DateTimeToString(pId.modified));
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsBonusDao-->Delete-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 序號兌換 列表 +List<Model.Query.PromotionsBonusQuery> Query(Model.Query.PromotionsBonusQuery store, out int totalCount)
        public List<Model.Query.PromotionsBonusQuery> Query(Model.Query.PromotionsBonusQuery store, out int totalCount)
        {
            string str = string.Empty;
            store.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@"SELECT PB.id, PB.name, PB.group_id, PB.type, PB.amount,PB.active, PB.days, PB.new_user, PB.repeat, PB.multiple, PB.start as startbegin,PB.condition_id, PB.end,VUG.group_name,PB.type,PB.muser,mu.user_username FROM promotions_bonus AS PB  left join vip_user_group as VUG on PB.group_id=VUG.group_id LEFT JOIN manage_user mu ON PB.muser=mu.user_id WHERE `status` = '1'");
                StringBuilder ssbb = new StringBuilder("");
                if (store.expired == 1)
                {
                    ssbb.AppendFormat(" and `end` >= '{0}' order by PB.id desc ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    ssbb.AppendFormat(" and `end` < '{0}' order by PB.id desc ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                totalCount = 0;
                if (store.IsPage)
                {
                    DataTable dt = _access.getDataTable(@"SELECT count(1) as totalcounts FROM promotions_bonus AS PB left join vip_user_group as VUG on PB.group_id=VUG.group_id WHERE `status` = '1' " + ssbb.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0]["totalcounts"]);
                    }
                    ssbb.AppendFormat(" limit {0},{1}", store.Start, store.Limit);
                }
                 str = sb.ToString() + ssbb.ToString();
                return _access.getDataTableForObj<PromotionsBonusQuery>(str);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsBonusDao-->Query-->" + ex.Message + str, ex);
            }
        }
        #endregion

        #region 序號兌換 獲取某行數據 +Model.PromotionsBonus GetModel(int id)
        public Model.PromotionsBonus GetModel(int id)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"select id,name,group_id,type,amount,days,new_user,`repeat`,`multiple`,status from promotions_bonus where id={0}", id);
                return _access.getSinggleObj<PromotionsBonus>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsBonusDao-->GetModel-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 序號兌換 改變狀態 +int UpdateActive(PromotionsBonus store)
        public int UpdateActive(PromotionsBonus store)
        {
                
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("update promotions_bonus set active ={0},muser={2},modified='{3}' where id={1}", store.active, store.id,store.muser,CommonFunction.DateTimeToString(store.modified));
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsBonusDao-->UpdateActive-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
    } 

}
