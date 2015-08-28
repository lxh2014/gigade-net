/* 
* 文件名稱 :OrderUserReduceDao.cs 
* 文件功能描述 :促銷減免查詢數據操作 
* 版權宣告 : 
* 開發人員 : shiwei0620j 
* 版本資訊 : 1.0 
* 日期 : 2014/10/17
* 修改人員 : 
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao
{
 public   class OrderUserReduceDao : IOrderUserReduceImplDao
    {

        public IDBAccess _accessMySql;
        public string connStr;
        public OrderUserReduceDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connStr = connectionString;
        }
        #region 促銷減免查詢列表頁 +List<PromotionsAmountReduceMemberQuery> GetOrderUserReduce()
        public List<PromotionsAmountReduceMemberQuery> GetOrderUserReduce(PromotionsAmountReduceMemberQuery store, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            sql.AppendFormat(" SELECT parm.id,parm.user_id,parm.group_id,parm.reduce_id,parm.order_id,parm.order_type,vug.group_name,u.user_email,u.user_password,u.user_gender, u.user_birthday_year ,u.user_birthday_month,u.user_birthday_day,u.user_zip,u.user_address, u.user_mobile,u.user_phone,u.user_reg_date,u.user_type,u.send_sms_ad,u.adm_note,u.user_province,u.user_city, ");
            sql.AppendFormat(" CASE u.user_type  when '1' THEN '網路會員' else'電話會員' END as mytype,concat(u.user_birthday_year,'/',u.user_birthday_month,'/',u.user_birthday_day) as birthday,parm.order_status,parm.created,par.`name`,par.type,par.quantity,u.user_name    ");
            sqlFrom.AppendFormat(" FROM promotions_amount_reduce_member parm   ");
            sqlFrom.AppendFormat(" INNER JOIN promotions_amount_reduce par ON par.id=parm.reduce_id  ");
            sqlFrom.AppendFormat(" INNER JOIN users u ON u.user_id=parm.user_id  ");
            sqlFrom.AppendFormat(" INNER JOIN vip_user_group vug ON vug.group_id=parm.group_id ");
            sqlWhere.AppendFormat(" WHERE 1=1 AND order_status > 0 ");
            #region 運送類別
            sqlWhere.AppendFormat("  and par.type={0} ", store.type);
            #endregion
            #region 查詢條件
            if (!string.IsNullOrEmpty(store.select_type))
                {
            if (store.select_type == "1")
            {
                sqlWhere.AppendFormat(" and  parm.order_id='{0}'", store.search_con);
            }
            if (store.select_type == "2")
            {
                sqlWhere.AppendFormat(" and  u.user_name like  '%{0}%'", store.search_con);
            }
            if (store.select_type == "3")
            {
                sqlWhere.AppendFormat(" and  parm.user_id={0} ", store.search_con);
            }
                }
            #endregion
            #region 會員群組
            if (store.group_id != 0)
            {
                sqlWhere.AppendFormat(" and parm.group_id={0}", store.group_id);
            }
            #endregion
            #region 減免活動
            if (store.reduce_id != 0)
                {
                    sqlWhere.AppendFormat(" and  parm.reduce_id='{0}'", store.reduce_id);
                }
            #endregion
            #region 日期
                if (store.search_date != 0)
                {
                    sqlWhere.AppendFormat(" and parm.created>='{0}' and parm.created<='{1}' ",store.start_time.ToString("yyyy-MM-dd 00:00:00"), store.end_time.ToString("yyyy-MM-dd 23:59:59"));
                }
                #endregion
      
            sqlCount.AppendFormat("SELECT	count(DISTINCT parm.id) AS totalCount ");
            totalCount = 0;
            if (store.IsPage)
            {
                DataTable _dt = _accessMySql.getDataTable(sqlCount.ToString()+sqlFrom.ToString()+sqlWhere.ToString());
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                }
                
            }
            sqlWhere.AppendFormat(" limit {0},{1};", store.Start, store.Limit);
            try
            {
                return _accessMySql.getDataTableForObj<PromotionsAmountReduceMemberQuery>(sql.ToString()+sqlFrom.ToString()+sqlWhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderUserReduceDao-->GetOrderUserReduce-->" + ex.Message + sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString(), ex);
            }
        }
        #endregion
        #region 查詢條件之減免活動 +List<PromotionsAmountReduceMemberQuery> GetReduceStore()
        public List<PromotionsAmountReduceMemberQuery> GetReduceStore()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("select id,name from promotions_amount_reduce where active =1;");
            try
            {
                return _accessMySql.getDataTableForObj<PromotionsAmountReduceMemberQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderUserReduceDao-->GetReduceStore-->"+ex.Message+sql.ToString(),ex);
            }
        }
        #endregion
        #region 查詢條件之群組會員 +List<VipUserGroup> GetVipUserGroupStore()
        public List<VipUserGroup> GetVipUserGroupStore()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(" select group_id,group_name from vip_user_group; ");
            try
            {
                return _accessMySql.getDataTableForObj<VipUserGroup>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderUserReduceDao-->GetVipUserGroupStore-->"+ex.Message+sql.ToString(),ex);
            }
        }
        #endregion
    }
}
