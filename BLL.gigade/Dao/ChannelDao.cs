/*
* 文件名稱 :ChannelDao.cs
* 文件功能描述 :外站資訊表
* 版權宣告 :
* 開發人員 : 天娥璇子
* 版本資訊 : 1.0
* 日期 : 2013/08/19
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using System.Data;
using System.Data.SqlClient;

namespace BLL.gigade.Dao
{
    class ChannelDao : IChannelImplDao
    {
        private IDBAccess _accessMySql;
        string strSql = string.Empty;

        public ChannelDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        #region IChannelImplDao 成员

        public List<Channel> Query(int status = 0)
        {
            try
            {
                if (status == 0)
                {
                    strSql = string.Format(@"select channel_id,channel_type, channel_name_full, channel_name_simple, channel_status, case channel_status when '1' then '啟用' when '2' then '停用' end as channel_status_name,receipt_to from channel");
                    return _accessMySql.getDataTableForObj<Channel>(strSql);
                }
                else
                {
                    strSql = string.Format("select channel_id,channel_type, channel_name_full, channel_name_simple, channel_status, case channel_status when '1' then '啟用' when '2' then '停用' end as channel_status_name,receipt_to from channel where channel_status = {0}", status);
                    return _accessMySql.getDataTableForObj<Channel>(strSql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelDao-->Query-->" + ex.Message, ex);
            }
        }

        public List<Channel> QueryCooperationSite(int status = 0)
        {
            try
            {
                if (status == 0)
                {
                    strSql = string.Format("select channel_id,channel_type, channel_name_full, channel_name_simple, channel_status, case channel_status when '1' then '啟用' when '2' then '停用' end as channel_status_name from channel where channel_type <> 2");
                    return _accessMySql.getDataTableForObj<Channel>(strSql);
                }
                else
                {
                    strSql = string.Format("select channel_id,channel_type, channel_name_full, channel_name_simple, channel_status, case channel_status when '1' then '啟用' when '2' then '停用' end as channel_status_name from channel where channel_type <> 2 and channel_status = {0}", status);
                    return _accessMySql.getDataTableForObj<Channel>(strSql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelDao-->QueryCooperationSite-->" + ex.Message, ex);
            }
        }

        public List<Channel> Query(string strSel)//add by xiangwang0413w 2014/06/26 增加 ERP客戶代號
        {
            try
            {
                strSql = string.Format(@"select channel_id,channel_status,
                case channel_status when '1' then '啟用' when '2' then '停用' end as channel_status_name, 
                channel_name_full,channel_name_simple,channel_invoice,channel_email,company_phone,
                company_fax,company_zip,company_address,invoice_title,invoice_zip,invoice_address,contract_createdate,
                contract_start,contract_end, annaul_fee,renew_fee,receipt_to,channel_type,user_email,model_in,notify_sms,
                (select middlecode from t_zip_code where zipcode=company_zip) as companycity,
                (select middlecode from t_zip_code where zipcode=invoice_zip) as invoicecity,erp_id
                from channel
                left join users on channel.user_id = users.user_id where 1=1 {0}", strSel);
                return _accessMySql.getDataTableForObj<Channel>(strSql);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelDao-->Query-->" + ex.Message, ex);
            }
        }

        public List<Channel> DataPager(string strSel, int startPage, int endPage, ref int totalPage)
        {//add by xiangwang0413w 2014/06/26 增加 ERP客戶代號(erp_id)
            try
            {
                strSql = string.Format(@"select channel_id,channel_status,
                case channel_status when '1' then '啟用' when '2' then '停用' end as channel_status_name, 
                channel_name_full,channel_name_simple,channel_invoice,channel_email,company_phone,
                company_fax,company_zip,company_address,invoice_title,invoice_zip,invoice_address,contract_createdate,
                contract_start,contract_end, annaul_fee,renew_fee,receipt_to,channel_type,user_email,model_in,notify_sms,erp_id,
                (select middlecode from t_zip_code where zipcode=company_zip) as companycity,
                (select middlecode from t_zip_code where zipcode=invoice_zip) as invoicecity
                from channel
                left join users on channel.user_id = users.user_id where 1=1 {0} limit {1},{2}", strSel, startPage, endPage);
                string sql = string.Format(@"select count(channel_id) from channel left join users on channel.user_id = users.user_id where 1=1 {0}", strSel);
                totalPage = int.Parse(_accessMySql.getDataTable(sql).Rows[0][0].ToString());
                return _accessMySql.getDataTableForObj<Channel>(strSql);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelDao-->DataPager-->" + ex.Message, ex);
            }
            
        }

        public List<Channel> QueryOther(int channelid)
        {
            try
            {
                strSql = string.Format(@"select deal_method,deal_percent,deal_fee,creditcard_1_percent,creditcard_3_percent, shopping_car_percent,commission_percent,cost_by_percent,
                cost_low_percent,cost_normal_percent,invoice_checkout_day,invoice_apply_start,invoice_apply_end,checkout_note, receipt_to,channel_manager,channel_note
                from channel where channel_id = {0}", channelid);
                return _accessMySql.getDataTableForObj<Channel>(strSql);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelDao-->QueryOther-->" + ex.Message, ex);
            }
           
        }

        public int Save(Channel ch)
        {
            ch.Replace4MySQL();
            StringBuilder sbCloum = new StringBuilder();
            StringBuilder sbValue = new StringBuilder();
            try
            {
                if (ch.contract_createdate != DateTime.MinValue)
                {
                    sbCloum.Append("contract_createdate,");
                    sbValue.Append("'" + ch.contract_createdate.ToString("yyyy/MM/dd") + "',");
                }
                if (ch.contract_start != DateTime.MinValue)
                {
                    sbCloum.Append("contract_start,");
                    sbValue.Append("'" + ch.contract_start.ToString("yyyy/MM/dd") + "',");
                }
                if (ch.contract_end != DateTime.MinValue)
                {
                    sbCloum.Append("contract_end,");
                    sbValue.Append("'" + ch.contract_end.ToString("yyyy/MM/dd") + "',");
                }
                //add by xiangwang0413w 2014/06/26 增加 ERP客戶代號(erp_id)
                strSql = string.Format(@"insert into channel(channel_status,channel_name_full,channel_name_simple,channel_invoice,channel_email,company_phone,
                company_fax,company_zip,company_address,invoice_title,invoice_zip,invoice_address,{0} annaul_fee,renew_fee,channel_type,user_id,model_in,notify_sms,erp_id) 
                values ('{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8},'{9}','{10}','{11}','{12}',{13} {14},{15},'{16}','{17}','{18}','{19}','{20}');select @@identity",
                    sbCloum.ToString(),
                    ch.channel_status, ch.channel_name_full, ch.channel_name_simple, ch.channel_invoice, ch.channel_email, ch.company_phone, ch.company_fax,
                    ch.company_zip == 0 ? "null" : ch.company_zip.ToString(),
                    ch.company_address, ch.invoice_title, ch.invoice_zip, ch.invoice_address,
                    sbValue.ToString(),
                    ch.annaul_fee == 0 ? "null" : ch.annaul_fee.ToString(),
                    ch.renew_fee == 0 ? "null" : ch.renew_fee.ToString(),
                    ch.channel_type, ch.user_id, ch.model_in, ch.notify_sms, ch.erp_id);

                return Int32.Parse(_accessMySql.getDataTable(strSql).Rows[0][0].ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelDao-->Save-->" + ex.Message, ex);
            }
          
        }

        public int Edit(Channel ch)
        {
            ch.Replace4MySQL();
            try
            {
                StringBuilder sql = new StringBuilder("update channel set ");
                sql.Append(string.Format("channel_status='{0}',channel_name_full='{1}',channel_name_simple='{2}',channel_invoice='{3}',channel_email='{4}',company_phone='{5}',company_fax='{6}',company_zip='{7}',company_address='{8}',invoice_title='{9}',invoice_zip='{10}',invoice_address='{11}',annaul_fee='{12}',renew_fee='{13}',channel_type='{14}',user_id='{15}',model_in='{16}',notify_sms='{17}',erp_id='{18}' ", ch.channel_status, ch.channel_name_full, ch.channel_name_simple, ch.channel_invoice, ch.channel_email, ch.company_phone, ch.company_fax, ch.company_zip, ch.company_address, ch.invoice_title, ch.invoice_zip, ch.invoice_address, ch.annaul_fee, ch.renew_fee, ch.channel_type, ch.user_id, ch.model_in, ch.notify_sms, ch.erp_id));
                if (ch.contract_createdate != DateTime.MinValue)
                {
                    sql.Append(string.Format(",contract_createdate='{0}'", ch.contract_createdate.ToString("yyyy/MM/dd")));
                }
                if (ch.contract_start != DateTime.MinValue)
                {
                    sql.Append(string.Format(",contract_start='{0}'", ch.contract_start.ToString("yyyy/MM/dd")));
                }
                if (ch.contract_end != DateTime.MinValue)
                {
                    sql.Append(string.Format(",contract_end='{0}'", ch.contract_end.ToString("yyyy/MM/dd")));
                }
                sql.Append(string.Format(" WHERE channel_id = '{0}';", ch.channel_id));

                sql.Append(string.Format(" UPDATE users SET user_name = '{0}' WHERE user_id='{1}'", ch.channel_name_full, ch.user_id));
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelDao-->Edit-->" + ex.Message, ex);
            }
            
        }

        public int Delete(Channel ch)
        {
            try
            {
                strSql = string.Format("delete channel where channel_id = '{0}'", ch.channel_id);
                return _accessMySql.execCommand(strSql);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelDao-->Delete-->" + ex.Message, ex);
            }
            
        }

        public DataTable QueryUser(string strUserID)
        {
            try
            {
                strSql = string.Format("select user_id,channel_id from channel where user_id = '{0}'", strUserID);
                return _accessMySql.getDataTable(strSql);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelDao-->QueryUser-->" + ex.Message, ex);
            }
            
        }

        public int SaveOther(Channel ch)
        {
            ch.Replace4MySQL();
            try
            {
                strSql = string.Format(@"update channel set deal_method={0},deal_percent={1},deal_fee={2},creditcard_1_percent={3},creditcard_3_percent={4},
                shopping_car_percent={5},commission_percent={6},cost_by_percent={7},cost_low_percent={8},cost_normal_percent={9},invoice_checkout_day={10},
                invoice_apply_start={11},invoice_apply_end={12},checkout_note='{13}',receipt_to={14},channel_manager='{15}',channel_note='{16}' where channel_id='{17}'",
               ch.deal_method == 0 ? "null" : ch.deal_method.ToString(),
               ch.deal_percent == 0 ? "null" : ch.deal_percent.ToString(),
               ch.deal_fee == 0 ? "null" : ch.deal_fee.ToString(),
               ch.creditcard_1_percent == 0 ? "null" : ch.creditcard_1_percent.ToString(),
               ch.creditcard_3_percent == 0 ? "null" : ch.creditcard_3_percent.ToString(),
               ch.shopping_car_percent == 0 ? "null" : ch.shopping_car_percent.ToString(),
               ch.commission_percent == 0 ? "null" : ch.commission_percent.ToString(),
               ch.cost_by_percent == 0 ? "null" : ch.cost_by_percent.ToString(),
               ch.cost_low_percent == 0 ? "null" : ch.cost_low_percent.ToString(),
               ch.cost_normal_percent == 0 ? "null" : ch.cost_normal_percent.ToString(),
               ch.invoice_checkout_day == 0 ? "null" : ch.invoice_checkout_day.ToString(),
               ch.invoice_apply_start == 0 ? "null" : ch.invoice_apply_start.ToString(),
               ch.invoice_apply_end == 0 ? "null" : ch.invoice_apply_end.ToString(),
               ch.checkout_note,
               ch.receipt_to == 0 ? "null" : ch.receipt_to.ToString(),
               ch.channel_manager, ch.channel_note, ch.channel_id);

                return _accessMySql.execCommand(strSql);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelDao-->SaveOther-->" + ex.Message, ex);
            }
           
        }


        public int GetUserIdByChannelId(int channelId)
        {
            try
            {
                strSql = "select user_id from channel where channel_id=" + channelId;
                DataTable _dt = _accessMySql.getDataTable(strSql);
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(_dt.Rows[0]["user_id"]);
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelDao-->GetUserIdByChannelId-->" + ex.Message, ex);
            }
           
        }
        #endregion
    }
}
