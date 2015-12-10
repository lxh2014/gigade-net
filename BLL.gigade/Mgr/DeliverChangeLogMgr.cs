/*
* 文件名稱 :DeliverChangeLogDao.cs
* 文件功能描述 :出貨管理--出貨單期望到貨日
* 版權宣告 :
* 開發人員 : zhaozhi0623j
* 版本資訊 : 1.0
* 日期 : 2015-11-12
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Common;
using System.Configuration;
using BLL.gigade.Model.APIModels;

namespace BLL.gigade.Mgr
{
    public class DeliverChangeLogMgr : IDeliverChangeLogImplMgr
    {
        private IDeliverChangeLogImplDao _IDeliverChangeLogDao;
        
        private string mySqlConnectionString;
        public DeliverChangeLogMgr(string connectionString)
        {
            _IDeliverChangeLogDao = new DeliverChangeLogDao(connectionString);
            mySqlConnectionString = connectionString;
        }
        
        #region 期望到貨日調整記錄
        public int insertDeliverChangeLog(DeliverChangeLog dCL)
        {
            try
            {
                return _IDeliverChangeLogDao.insertDeliverChangeLog(dCL);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverChangeLogMgr-->insertDeliverChangeLog-->" + ex.Message, ex);
            }
        }
        public List<DeliverChangeLogQuery> GetDeliverChangeLogList(DeliverChangeLogQuery Query, out int totalCount)
        {
            try
            {
                return _IDeliverChangeLogDao.GetDeliverChangeLogList(Query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverChangeLogMgr-->GetDeliverChangeLogList-->" + ex.Message, ex);
            }
        } 
        #endregion

        #region 期望到貨日調整記錄通知排程 add by zhaozhi0623j 20151118PM
        //將DataTable轉化為Html 
        public string GetHtmlByDataTable(DataTable _dtmyMonth)
        {
            System.Text.StringBuilder sbHtml = new System.Text.StringBuilder();
            //sbHtml.Append("<table  cellpadding=3 cellspacing=1  border=2 style=\"border:white solid #ccc; \">");
            sbHtml.Append("<table cellpadding=2 cellspacing=2  style='border: black solid;border-width:2 0 2 0'>");//style='border-collapse: collapse '
            sbHtml.Append("<tr style=\"text-align: center; COLOR: black; BACKGROUND-COLOR: #c0e0f0; font-weight: bold\">");//B3D4FF
            
            string aligns = "align=\"left\"";
            string color = "style=\"background-color:#ffeedd;\"";//單數行的樣式f0f0f0 dcb5ff  e0e0e0
            
            sbHtml.Append("<th ");
            sbHtml.Append(" >");
            sbHtml.Append("行號");
            sbHtml.Append("</th>");

            //插入列頭
            for (int i = 0; i < _dtmyMonth.Columns.Count; i++)
            {
                sbHtml.Append("<th ");
                sbHtml.Append(" >");
                sbHtml.Append(_dtmyMonth.Columns[i].ColumnName);
                sbHtml.Append("</th>");
            }
            sbHtml.Append("</tr>");
            //插入數據，單數行設置背景色
            for (int i = 0; i < _dtmyMonth.Rows.Count; i++)//行
            {
                sbHtml.Append("<tr>");

                sbHtml.Append("<td ");
                if (i % 2 == 0)
                {
                    sbHtml.Append(aligns + color);
                }
                else
                {
                    sbHtml.Append(aligns);
                }
                
                sbHtml.Append(" >");
                sbHtml.Append(i+1);
                sbHtml.Append("</td>");
                for (int j = 0; j < _dtmyMonth.Columns.Count; j++)
                {
                    sbHtml.Append("<td ");               
                    if (i % 2 == 0)
                    {
                        sbHtml.Append(aligns + color);
                    }
                    else
                    {
                        sbHtml.Append(aligns);
                    }
                    sbHtml.Append(" >");
                    sbHtml.Append(_dtmyMonth.Rows[i][j]);
                    sbHtml.Append("</td>");
                }
                sbHtml.Append("</tr>");
            }
            sbHtml.Append("</table>");
            return sbHtml.ToString();

        }
        public bool Start(string schedule_code)
        {
                      
            
            BLL.gigade.Common.MailModel mailModel = new Common.MailModel ();
            mailModel.MysqlConnectionString = mySqlConnectionString;

            string GroupCode = string.Empty;
            string MailTitle = string.Empty;
            string MailBody = string.Empty;
            string hourNum = string.Empty;
            //bool IsSeparate = false;
            //bool IsDisplyName = true;
           
            try
            {
               

                //獲取該排程參數
                List<ScheduleConfigQuery> store_config = new List<ScheduleConfigQuery>();
                ScheduleConfigQuery query_config = new ScheduleConfigQuery();
                query_config.schedule_code = schedule_code;
                ScheduleServiceMgr _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);
                store_config = _secheduleServiceMgr.GetScheduleConfig(query_config);
                #region mailhelp賦值
                foreach (ScheduleConfigQuery item in store_config)
                {
                    
                    if (item.parameterCode.Equals("MailFromAddress"))
                    {
                        mailModel.MailFromAddress = item.value;
                    }
                    else if (item.parameterCode.Equals("MailHost"))
                    {
                        mailModel.MailHost = item.value;
                    }
                    else if (item.parameterCode.Equals("MailPort"))
                    {
                        mailModel.MailPort = item.value;
                    }
                    else if (item.parameterCode.Equals("MailFromUser"))
                    {
                        mailModel.MailFromUser = item.value;
                    }
                    else if (item.parameterCode.Equals("MailFormPwd"))
                    {
                        mailModel.MailFormPwd = item.value;
                    }
                    else if (item.parameterCode.Equals("GroupCode"))
                    {
                        GroupCode = item.value;
                    }
                    else if (item.parameterCode.Equals("MailTitle"))
                    {
                        MailTitle = item.value;
                    }
                    else if (item.parameterCode.Equals("HourNum"))
                    {
                        if (item.value.Trim() == "0")
                        {
                            hourNum = "1";
                        }
                        else
                        {
                            hourNum = item.value;
                        }
                    }                 
                }
                if (hourNum.Trim() == string.Empty)
                {
                    hourNum = "1";
                }

                //獲取期望到貨日調整記錄(邮件内容)
                #region 獲取期望到貨日調整記錄表格
                DeliverChangeLogQuery aclQuery = new DeliverChangeLogQuery();

                aclQuery.time_start = DateTime.Now.AddHours(-Convert.ToDouble(hourNum));
                aclQuery.time_end = DateTime.Now;
                System.Data.DataTable dclTable = _IDeliverChangeLogDao.GetDeliverChangeLogDataTable(aclQuery);

                DataTable _dt = new DataTable();
                DataRow dr;
                _dt.Columns.Add("訂單編號", typeof(string));
                _dt.Columns.Add("出貨單號", typeof(string));
                _dt.Columns.Add("異動人", typeof(string));
                _dt.Columns.Add("異動時間", typeof(string));
                //_dt.Columns.Add("原期望到貨日", typeof(string));
                _dt.Columns.Add("期望到貨日", typeof(string));
                _dt.Columns.Add("期望到貨時段", typeof(string));
                _dt.Columns.Add("備註", typeof(string));
                _dt.Columns.Add("來源IP", typeof(string));

                if (dclTable.Rows.Count > 0)
                {
                    #region 循環賦值
                    for (int i = 0; i < dclTable.Rows.Count; i++)
                    {
                        dr = _dt.NewRow();
                        StringBuilder sb = new StringBuilder();
                        dr["訂單編號"] = dclTable.Rows[i]["order_id"].ToString();
                        dr["出貨單號"] = dclTable.Rows[i]["deliver_id"].ToString();
                        if (dclTable.Rows[i]["dcl_create_type"].ToString() == "1")
                        {
                            dr["異動人"] = dclTable.Rows[i]["dcl_create_username"].ToString();
                        }
                        else
                        {
                            dr["異動人"] = dclTable.Rows[i]["dcl_create_musername"].ToString();
                        }
                        dr["異動時間"] = Convert.ToDateTime(dclTable.Rows[i]["dcl_create_datetime"]).ToString("yyyy-MM-dd HH:mm:ss");

                        #region 獲取該出貨單的上一個小時之前的期望到貨日
                        ////aclQuery.time_start = dcl_create_datetime_start.AddHours(-1);
                        //aclQuery.time_end = dcl_create_datetime_end.AddHours(-1);
                        //aclQuery.deliver_id = Convert.ToInt32(dclTable.Rows[i]["deliver_id"]);
                        //System.Data.DataTable Table = _IDeliverChangeLogDao.GetExpectArriveDateByCreatetime(aclQuery);
                        //if (Table.Rows.Count > 0)
                        //{
                        //    if (Convert.ToDateTime(Table.Rows[0]["expect_arrive_date"]).ToString("yyyy-MM-dd") == "0001-01-01")
                        //    {
                        //        dr["原期望到貨日"] = "";
                        //    }
                        //    else
                        //    {
                        //        dr["原期望到貨日"] = Convert.ToDateTime(Table.Rows[0]["expect_arrive_date"]).ToString("yyyy-MM-dd");  
                        //    }                                                   
                        //}
                        //else
                        //{
                        //    dr["原期望到貨日"] = "";
                        //}
                        #endregion

                        if (Convert.ToDateTime(dclTable.Rows[i]["expect_arrive_date"]).ToString("yyyy-MM-dd") == "0001-01-01")
                        {
                            dr["期望到貨日"] = "";
                        }
                        else
                        {
                            dr["期望到貨日"] = Convert.ToDateTime(dclTable.Rows[i]["expect_arrive_date"]).ToString("yyyy-MM-dd");
                        }
                        if (dclTable.Rows[i]["expect_arrive_period"].ToString() == "1")
                        {
                            sb.Append("12:00以前");
                        }
                        if (dclTable.Rows[i]["expect_arrive_period"].ToString() == "2")
                        {
                            sb.Append("12:00-17:00");
                        }
                        if (dclTable.Rows[i]["expect_arrive_period"].ToString() == "3")
                        {
                            sb.Append("17:00-20:00");
                        }
                        if (dclTable.Rows[i]["expect_arrive_period"].ToString() == "0")
                        {
                            sb.Append("不限制");
                        }
                        dr["期望到貨時段"] = sb.ToString();
                        dr["備註"] = dclTable.Rows[i]["dcl_note"].ToString();
                        dr["來源IP"] = dclTable.Rows[i]["dcl_ipfrom"].ToString();
                        _dt.Rows.Add(dr);
                        sb.Clear();
                    }
                    #endregion
                }

                #endregion


                if (_dt.Rows.Count == 0)
                {
                    MailBody = "<br/><p><font size=\"4\">   出貨單期望日在前 <font color=\"#FF0000\" >" + Convert.ToDouble(hourNum) + "</font> 個小時之內沒有調整記錄!</font><p/>";
                }
                else
                {
                    MailBody = "<br/><font size=\"4\">出貨單期望到貨日在前 " + "<font color=\"#FF0000\" >" + Convert.ToDouble(hourNum) + "</font>" + " 個小時之內的調整記錄如下：</font><br/><p/>" + GetHtmlByDataTable(_dt);
                }

                #endregion

                //DateTime endTime = DateTime.Now;
                //TimeSpan ts = endTime - startTime;
                //double Second = ts.TotalSeconds;
                BLL.gigade.Common.MailHelper mail = new Common.MailHelper(mailModel);
                //public Boolean SendToGroup(int GroupID, string MailTitle, string MailBody, 
                //                           Boolean IsSeparate = false, Boolean IsDisplyName = false)
                //return mail.SendToGroup(GroupCode, MailTitle, MailBody + "<br/>郵件發送共耗時" + Second + "秒", true, true);  
                return mail.SendToGroup(GroupCode, MailTitle, MailBody, true, true); 
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverChangeLogMgr-->Start-->" + ex.Message, ex);
            }
        } 
        #endregion

        #region 是否能夠修改期望到貨日 +bool isCanModifyExpertArriveDate(string apiServer,long deliver_id) 
        /// <summary>
        /// 是否能夠修改期望到貨日
        /// </summary>
        /// <param name="apiServer">apisever地址</param>
        /// <param name="deliver_id">要修改的deliver_id</param>
        /// <returns></returns>
        public bool isCanModifyExpertArriveDate(string apiServer, long deliver_id)
        {


            try
            {
                GigadeApiRequest request = new GigadeApiRequest(apiServer);
                var result = request.Request<DeliverIdViewModel, object>("api/admin/Logistics/CanModifyExpertArriveDate", new DeliverIdViewModel() { deliver_id = deliver_id });
                //var result = request.Request<DeliverIdViewModel, object>("api/Logistics/ModifyExpertArriveDate", new DeliverIdViewModel() { deliver_id = deliver_id });//api/Logistics/ModifyExpertArriveDate
                if (Convert.ToBoolean(result.result))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception("DeliverChangeLogMgr-->isCanModifyExpertArriveDate-->" + ex.Message, ex);

            }

        } 
        #endregion

        #region 修改期望到貨日 +bool ModifyExpertArriveDate(string apiServer, ModifyExpertArriveDateViewModel expertArriveDateViewModel)
        /// <summary>
        /// 修改期望到貨日
        /// </summary>
        /// <param name="apiServer"></param>
        /// <param name="expertArriveDateViewModel"></param>
        /// <returns></returns>
        public bool ModifyExpertArriveDate(string apiServer, ModifyExpertArriveDateViewModel expertArriveDateViewModel)
        {


            try
            {
                GigadeApiRequest request = new GigadeApiRequest(apiServer);
                var result = request.Request<ModifyExpertArriveDateViewModel, object>("api/admin/Logistics/ModifyExpertArriveDate", expertArriveDateViewModel);
                if (Convert.ToBoolean(result.result))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception("DeliverChangeLogMgr-->isCanModifyExpertArriveDate-->" + ex.Message, ex);

            }

        }
    } 
        #endregion
}
