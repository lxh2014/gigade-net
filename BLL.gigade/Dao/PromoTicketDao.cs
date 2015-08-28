#region 文件信息
/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：PromoTicketDao.cs
 * 摘   要： 
 *      
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
using System.Data;
using BLL.gigade.Common;
using BLL.gigade.Model;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Dao
{
    public class PromoTicketDao : IPromoTicketImplDao
    {
        private IDBAccess _access;
        private string connStr;
        public PromoTicketDao(string connectionstring)
        { 
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
        }
        #region 刪除+int Delete(int rid)
        public int Delete(int rid)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" update promo_ticket set status=0 where rid='{0}';", rid);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoTicketDao-->Delete-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 保存 +int Save(PromoTicket model)
        public int Save(PromoTicket model)
        {
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(" insert into promo_ticket( ticket_name,event_id,event_type,active_now,valid_interval,");
                sb.AppendFormat(" use_start,use_end,kuser,kdate,muser,mdate,status)");
                sb.AppendFormat(" values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}');select @@identity;",
                 model.ticket_name, model.event_id, model.event_type, model.active_now, model.valid_interval, CommonFunction.DateTimeToString(model.use_start), CommonFunction.DateTimeToString(model.use_end), model.kuser, CommonFunction.DateTimeToString(model.kdate), model.muser, CommonFunction.DateTimeToString(model.mdate), model.status);
                return Int32.Parse(_access.getDataTable(sb.ToString()).Rows[0][0].ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoTicketDao-->Save-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 更新 +int Update(PromoTicket model)
        public int Update(PromoTicket model)
        {
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(" update promo_ticket set");
                sb.AppendFormat(" ticket_name='{0}',event_id='{1}',event_type='{2}',active_now='{3}',valid_interval='{4}',", model.ticket_name, model.event_id, model.event_type, model.active_now, model.valid_interval);
                sb.AppendFormat(" use_start='{0}',use_end='{1}',kuser='{2}',kdate='{3}',muser,mdate='{4}',status='{5}' where rid='{6}' ;", CommonFunction.DateTimeToString(model.use_start), CommonFunction.DateTimeToString(model.use_end), model.kuser, CommonFunction.DateTimeToString(model.kdate), model.muser, CommonFunction.DateTimeToString(model.mdate), model.status, model.rid);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoTicketDao-->Update-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region  Query+PromoTicket Query(int rid)
        public PromoTicket Query(int rid)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select ticket_name,event_id,event_type,active_now,valid_interval,");
                sql.AppendFormat(" use_start,use_end,kuser,kdate,muser,mdate,status from promo_ticket where rid='{0}';", rid);
                return _access.getSinggleObj<PromoTicket>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoTicketDao-->Query-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 刪除sql語句 +string DeleteSql(int id)
        public string DeleteSql(int id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("set sql_safe_updates = 0;");
                sql.AppendFormat(" update promo_ticket set status=0 where rid='{0}';", id);
                sql.Append("set sql_safe_updates = 1;");
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromoTicketDao-->DeleteSql-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 保存的sql語句 +string SaveSql(PromoTicket model)
        public string SaveSql(PromoTicket model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" insert into promo_ticket( ticket_name,event_id,event_type,active_now,valid_interval,");
                sql.AppendFormat(" use_start,use_end,kuser,kdate,muser,mdate,status)");
                sql.AppendFormat(" values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}');select @@identity;",
                 model.ticket_name, model.event_id, model.event_type, model.active_now, model.valid_interval, CommonFunction.DateTimeToString(model.use_start), CommonFunction.DateTimeToString(model.use_end), model.kuser, CommonFunction.DateTimeToString(model.kdate), model.muser, CommonFunction.DateTimeToString(model.mdate), model.status);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromoTicketDao-->SaveSql-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 更新sql語句 +string UpdateSql(PromoTicket model)
        public string UpdateSql(PromoTicket model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" update promo_ticket set");
                sql.AppendFormat(" ticket_name='{0}',event_id='{1}',event_type='{2}',active_now='{3}',valid_interval='{4}',", model.ticket_name, model.event_id, model.event_type, model.active_now, model.valid_interval);
                sql.AppendFormat(" use_start='{0}',use_end='{1}',kuser='{2}',kdate='{3}',muser='{4}',mdate='{5}',status='{6}' where rid='{7}' ;", CommonFunction.DateTimeToString(model.use_start), CommonFunction.DateTimeToString(model.use_end), model.kuser, CommonFunction.DateTimeToString(model.kdate), model.muser, CommonFunction.DateTimeToString(model.mdate), model.status, model.rid);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromoTicketDao-->UpdateSql-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region QuerySql語句+string QuerySql(int rid)
        public string QuerySql(int rid)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select * from promo_ticket where rid='{0}';", rid);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromoTicketDao-->QuerySql-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 刪除sql語句+string DelY(string event_id)
        public string DelY(string event_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" delete from promo_ticket where event_id='{0}' and status=1 ;", event_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromoTicketDao-->DelY-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
    }
}
