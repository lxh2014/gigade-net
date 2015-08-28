/*
* 文件名稱 :ChannelMgr.cs
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
using BLL.gigade.Model;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;

namespace BLL.gigade.Mgr
{
    public class ChannelMgr : IChannelImplMgr
    {
        private IChannelImplDao _chDao;

        public ChannelMgr(string connectionString)
        {
            _chDao = new ChannelDao(connectionString);
        }

        #region IChannelImplMgr 成员

        public string Query(int status = 0)
        {
            try
            {
                List<Channel> chResult = _chDao.Query(status);
                StringBuilder stb = new StringBuilder();

                stb.Append("{");
                stb.Append(string.Format("success:true,items:["));
                foreach (Channel ch in chResult)
                {
                    stb.Append("{");
                    stb.Append(string.Format("\"channel_id\":\"{0}\",\"channel_name_full\":\"{1}\",\"channel_name_simple\":\"{2}\",\"channel_status\":\"{3}\",\"channel_status_name\":\"{4}\",\"channel_type\":\"{5}\",\"receipt_to\":\"{6}\"", ch.channel_id, ch.channel_name_full, ch.channel_name_simple, ch.channel_status, ch.channel_status_name, ch.channel_type, ch.receipt_to));
                    stb.Append("}");
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelMgr-->Query(int status = 0)-->" + ex.Message, ex);
            }
        }

        public List<Channel> QueryList(int status = 0)
        {
            try
            {
                return _chDao.Query(status);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelMgr.QueryList-->" + ex.Message, ex);
            }
          
        }

        public string QueryCooperationSite(int status = 0)
        {
            try
            {
                List<Channel> chResult = _chDao.QueryCooperationSite(status);
                StringBuilder stb = new StringBuilder();

                stb.Append("{");
                stb.Append(string.Format("success:true,items:["));
                foreach (Channel ch in chResult)
                {
                    stb.Append("{");
                    stb.Append(string.Format("\"channel_id\":\"{0}\",\"channel_name_full\":\"{1}\",\"channel_name_simple\":\"{2}\",\"channel_status\":\"{3}\",\"channel_status_name\":\"{4}\",\"channel_type\":\"{5}\"", ch.channel_id, ch.channel_name_full, ch.channel_name_simple, ch.channel_status, ch.channel_status_name, ch.channel_type));
                    stb.Append("}");
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelMgr-->QueryCooperationSite-->" + ex.Message, ex);
            }
        }

        public string Query(string strSel)
        {
            try
            {
                List<Channel> chResult = _chDao.Query(strSel);
                StringBuilder stb = new StringBuilder();

                stb.Append("{");
                stb.Append(string.Format("success:true, items:["));
                foreach (Channel ch in chResult)
                {
                    stb.Append("{");
                    string str_createdate = ch.contract_createdate == DateTime.MinValue ? string.Empty : ch.contract_createdate.ToString("yyyy/MM/dd");
                    string str_contractStart = ch.contract_start == DateTime.MinValue ? string.Empty : ch.contract_start.ToString("yyyy/MM/dd");
                    string str_contractEnd = ch.contract_end == DateTime.MinValue ? string.Empty : ch.contract_end.ToString("yyyy/MM/dd");
                    stb.Append(string.Format("\"channel_id\":\"{0}\",\"channel_status\":\"{1}\",\"channel_name_full\":\"{2}\",\"channel_name_simple\":\"{3}\",\"channel_invoice\":\"{4}\",\"channel_email\":\"{5}\",\"company_phone\":\"{6}\",\"company_fax\":\"{7}\",\"company_zip\":\"{8}\",\"company_address\":\"{9}\",\"invoice_title\":\"{10}\",\"invoice_zip\":\"{11}\",\"invoice_address\":\"{12}\",\"contract_createdate\":\"{13}\",\"contract_start\":\"{14}\",\"contract_end\":\"{15}\",\"annaul_fee\":\"{16}\",\"renew_fee\":\"{17}\",\"channel_type\":\"{18}\",\"user_email\":\"{19}\",\"companyCity\":\"{20}\",\"invoiceCity\":\"{21}\",\"channel_status_name\":\"{22}\",\"model_in\":\"{23}\",\"notify_sms\":\"{24}\",\"receipt_to\":\"{25}\",\"erp_id\":\"{26}\"", ch.channel_id, ch.channel_status, ch.channel_name_full, ch.channel_name_simple, ch.channel_invoice, ch.channel_email, ch.company_phone, ch.company_fax, ch.company_zip, ch.company_address, ch.invoice_title, ch.invoice_zip, ch.invoice_address, str_createdate, str_contractStart, str_contractEnd, ch.annaul_fee, ch.renew_fee, ch.channel_type, ch.user_email, ch.companyCity, ch.invoiceCity, ch.channel_status_name, ch.model_in, ch.notify_sms, ch.receipt_to,ch.erp_id));
                    stb.Append("}");//add by xiangwang0413w 2014/06/26 增加 ERP客戶代號
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelMgr-->Query(string strSel)-->" + ex.Message, ex);
            }
        }


        public string DataPager(string strSel, int startPage, int endPage)
        {
            try
            {
                int totalPage = 0;
                List<Channel> chResult = _chDao.DataPager(strSel, startPage, endPage, ref totalPage);
                StringBuilder stb = new StringBuilder();

                stb.Append("{");
                stb.Append(string.Format("success:true,total:" + totalPage + ", items:["));
                foreach (Channel ch in chResult)
                {
                    stb.Append("{");
                    string str_createdate = ch.contract_createdate == DateTime.MinValue ? string.Empty : ch.contract_createdate.ToString("yyyy/MM/dd");
                    string str_contractStart = ch.contract_start == DateTime.MinValue ? string.Empty : ch.contract_start.ToString("yyyy/MM/dd");
                    string str_contractEnd = ch.contract_end == DateTime.MinValue ? string.Empty : ch.contract_end.ToString("yyyy/MM/dd");
                    stb.Append(string.Format("\"channel_id\":\"{0}\",\"channel_status\":\"{1}\",\"channel_name_full\":\"{2}\",\"channel_name_simple\":\"{3}\",\"channel_invoice\":\"{4}\",\"channel_email\":\"{5}\",\"company_phone\":\"{6}\",\"company_fax\":\"{7}\",\"company_zip\":\"{8}\",\"company_address\":\"{9}\",\"invoice_title\":\"{10}\",\"invoice_zip\":\"{11}\",\"invoice_address\":\"{12}\",\"contract_createdate\":\"{13}\",\"contract_start\":\"{14}\",\"contract_end\":\"{15}\",\"annaul_fee\":\"{16}\",\"renew_fee\":\"{17}\",\"channel_type\":\"{18}\",\"user_email\":\"{19}\",\"companyCity\":\"{20}\",\"invoiceCity\":\"{21}\",\"channel_status_name\":\"{22}\",\"model_in\":\"{23}\",\"notify_sms\":\"{24}\",\"receipt_to\":\"{25}\",\"erp_id\":\"{26}\"", ch.channel_id, ch.channel_status, ch.channel_name_full, ch.channel_name_simple, ch.channel_invoice, ch.channel_email, ch.company_phone, ch.company_fax, ch.company_zip, ch.company_address, ch.invoice_title, ch.invoice_zip, ch.invoice_address, str_createdate, str_contractStart, str_contractEnd, ch.annaul_fee, ch.renew_fee, ch.channel_type, ch.user_email, ch.companyCity, ch.invoiceCity, ch.channel_status_name, ch.model_in, ch.notify_sms, ch.receipt_to,ch.erp_id));
                    stb.Append("}");
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelMgr-->DataPager-->" + ex.Message, ex);
            }
        }

        public List<Channel> QueryC(string strSel)
        {
            try
            {
                return _chDao.Query(strSel);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelMgr-->QueryC-->" + ex.Message, ex);
            }
        }

        public int Save(Channel ch)
        {

            try
            {
                return _chDao.Save(ch);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelMgr-->SingleCompareSave-->" + ex.Message, ex);
            }
        }

        public int Edit(Channel ch)
        {

            try
            {
                return _chDao.Edit(ch);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelMgr-->Edit-->" + ex.Message, ex);
            }
        }

        public int Delete(Channel ch)
        {
            try
            {
                return _chDao.Delete(ch);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelMgr-->Delete-->" + ex.Message, ex);
            }

        }

        public System.Data.DataTable QueryUser(string strUserID)
        {

            try
            {
                return _chDao.QueryUser(strUserID);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelMgr-->QueryUser-->" + ex.Message, ex);
            }
        }

        public int SaveOther(Channel ch)
        {

            try
            {
                return _chDao.SaveOther(ch);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelMgr-->SaveOther-->" + ex.Message, ex);
            }
        }

        public int GetUserIdByChannelId(int channelId)
        {

            try
            {
                return _chDao.GetUserIdByChannelId(channelId);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelMgr-->GetUserIdByChannelId-->" + ex.Message, ex);
            }
        }

        public string QueryOther(int channelid)
        {
            try
            {
                List<Channel> chResult = _chDao.QueryOther(channelid);
                StringBuilder stb = new StringBuilder();

                stb.Append("{");
                stb.Append(string.Format("success:true,items:["));
                foreach (Channel ch in chResult)
                {
                    stb.Append("{");
                    stb.Append(string.Format("\"deal_method\":\"{0}\",\"deal_percent\":\"{1}\",\"deal_fee\":\"{2}\",\"creditcard_1_percent\":\"{3}\",\"creditcard_3_percent\":\"{4}\",\"shopping_car_percent\":\"{5}\",\"commission_percent\":\"{6}\",\"cost_by_percent\":\"{7}\",\"cost_low_percent\":\"{8}\",\"cost_normal_percent\":\"{9}\",\"invoice_checkout_day\":\"{10}\",\"invoice_apply_start\":\"{11}\",\"invoice_apply_end\":\"{12}\",\"checkout_note\":\"{13}\",\"receipt_to\":\"{14}\",\"channel_manager\":\"{15}\",\"channel_note\":\"{16}\"", ch.deal_method, ch.deal_percent, ch.deal_fee, ch.creditcard_1_percent, ch.creditcard_3_percent, ch.shopping_car_percent, ch.commission_percent, ch.cost_by_percent, ch.cost_low_percent, ch.cost_normal_percent, ch.invoice_checkout_day, ch.invoice_apply_start, ch.invoice_apply_end, ch.checkout_note, ch.receipt_to, ch.channel_manager, ch.channel_note));
                    stb.Append("}");
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelMgr-->QueryOther-->" + ex.Message, ex);
            }
        }


        #endregion
   
    }
}
