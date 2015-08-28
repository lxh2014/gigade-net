#region 文件信息
/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：PromotionsAccumulateRateDao.cs 
 * 摘   要： 
 *      點數累積
 * 当前版本：v1.1 
 * 作   者： dongya0410j
 * 完成日期：2014/6/20
 * 修改歷史：
 *      v1.1修改日期：2014/8/15
 *      v1.1修改人員：dongya0410j
 *      v1.1修改内容：代碼合併
 */

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model.Query;
using BLL.gigade.Common;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
   public class PromotionsAccumulateRateDao : IPromotionsAccumulateRateImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        public PromotionsAccumulateRateDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        #region 保存
        public int Save(Model.PromotionsAccumulateRate store)
        {
              StringBuilder sb = new StringBuilder();
            try
            {
                store.Replace4MySQL();
                sb.Append("insert into promotions_accumulate_rate(");
                sb.Append("name,group_id,amount,bonus_type,point,dollar,start,end,");
                sb.Append("created,modified,condition_id,payment_type_rid,active,kuser,muser)");
                sb.Append(" values");
                sb.AppendFormat("('{0}','{1}','{2}',", store.name, store.group_id, store.amount);
                sb.AppendFormat("'{0}','{1}','{2}',", store.bonus_type, store.point, store.dollar);
                sb.AppendFormat("'{0}','{1}','{2}',", CommonFunction.DateTimeToString(store.start), CommonFunction.DateTimeToString(store.end), CommonFunction.DateTimeToString(store.created));
                sb.AppendFormat("'{0}','{1}','{2}',", CommonFunction.DateTimeToString(store.modified), store.condition_id, store.payment_type_rid);
                sb.AppendFormat("{0},{1},{2})", store.active,store.kuser,store.muser);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAccumulateRateDao-->Save-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 點數累積 編輯 +int Update(Model.PromotionsAccumulateRate store)
        public int Update(Model.PromotionsAccumulateRate store)
        {
            StringBuilder sb = new StringBuilder();
            store.Replace4MySQL();
            try
            {
                sb.AppendFormat("update promotions_accumulate_rate set name='{1}',group_id='{2}',bonus_type='{3}',point='{4}',dollar='{5}',start='{6}',end='{7}',amount='{8}',modified='{9}',condition_id='{10}',payment_type_rid='{11}',active={12},muser={13} where id={0}", store.id, store.name, store.group_id, store.bonus_type, store.point, store.dollar, CommonFunction.DateTimeToString(store.start), CommonFunction.DateTimeToString(store.end), store.amount, CommonFunction.DateTimeToString(store.modified), store.condition_id, store.payment_type_rid,store.active,store.muser);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAccumulateRateDao-->Update-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 點數累積 刪除 +int Delete(Model.PromotionsAccumulateRate rodId)
        public int Delete(Model.PromotionsAccumulateRate rodId)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("update promotions_accumulate_rate set status ={0},modified='{2}',muser={3} where id={1}", 0, rodId.id,CommonFunction.DateTimeToString(rodId.modified),rodId.muser);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAccumulateRateDao-->Delete-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 點數累積 列表頁 +List<Model.Query.PromotionsAccumulateRateQuery> Query(Model.Query.PromotionsAccumulateRateQuery query, ref int totalCount)
        public List<Model.Query.PromotionsAccumulateRateQuery> Query(Model.Query.PromotionsAccumulateRateQuery query, ref int totalCount)
        {
           
            query.Replace4MySQL();
            StringBuilder sb = new StringBuilder("SELECT PAR.payment_type_rid,PAR.name,PAR.id,PAR.start as newstart ,PAR.active,PAR.amount,PAR.point,PAR.dollar,concat(PAR.dollar,'/',PAR.point) as PointDollars,PAR.end,VUG.group_name,PAR.bonus_type,PAR.condition_id,PAR.muser,mu.user_username ");
            StringBuilder ssbb = new StringBuilder("select count(PAR.id) as totalcounts");
            StringBuilder sbsb = new StringBuilder();
            sbsb.Append(@" FROM promotions_accumulate_rate AS PAR 
                         left join vip_user_group as VUG on PAR.group_id=VUG.group_id  LEFT JOIN manage_user mu ON PAR.muser=mu.user_id  
                         where status = 1");
            try
            {
                if (query.expired == 1)//是未過期
                {
                    sbsb.AppendFormat(" and end >= '{0}' order by PAR.id desc", CommonFunction.DateTimeToString(DateTime.Now));

                }
                else if (query.expired == 0)
                {
                    sbsb.AppendFormat(" and end < '{0}' order by PAR.id desc", CommonFunction.DateTimeToString(DateTime.Now));
                }
                totalCount = 0;
                if (query.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(ssbb.ToString() + sbsb.ToString());

                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalcounts"]);
                    }

                    sbsb.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                return _access.getDataTableForObj<PromotionsAccumulateRateQuery>(sb.ToString() + sbsb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAccumulateRateDao-->Query-->" + ex.Message + sb.ToString() + sbsb.ToString(), ex);
            }
        }
        #endregion

        #region 點數累積 獲取某行數據 +Model.PromotionsAccumulateRate GetModel(int id)
        public Model.PromotionsAccumulateRate GetModel(int id)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("select id,name,group_id,active from promotions_accumulate_rate where id={0}", id);
                return _access.getSinggleObj<PromotionsAccumulateRate>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAccumulateRateDao-->GetModel-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 點數累積 改變活動狀態 +int UpdateActive(PromotionsAccumulateRate store)
        public int UpdateActive(PromotionsAccumulateRate store)
        {
            StringBuilder sb = new StringBuilder();
            store.Replace4MySQL();
            try
            {
                sb.AppendFormat("update promotions_accumulate_rate set active ={0},modified='{1}',muser={2} where id={3}", store.active, CommonFunction.DateTimeToString(store.modified),store.muser, store.id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAccumulateRateDao-->UpdateActive-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
    }
}
