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
            
            //sbHtml.Append("<th ");
            //sbHtml.Append(" >");
            //sbHtml.Append("行號");
            //sbHtml.Append("</th>");

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

                //sbHtml.Append("<td ");
                //if (i % 2 == 0)
                //{
                //    sbHtml.Append(aligns + color);
                //}
                //else
                //{
                //    sbHtml.Append(aligns);
                //}
                
                //sbHtml.Append(" >");
                //sbHtml.Append(i+1);
                //sbHtml.Append("</td>");
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
                    else if (item.parameterCode.Equals("EmailPassWord"))
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
                #endregion

                //獲取期望到貨日調整記錄(邮件内容)
                #region 獲取期望到貨日調整記錄表格
                DeliverChangeLogQuery aclQuery = new DeliverChangeLogQuery();

                aclQuery.time_start = DateTime.Now.AddHours(-Convert.ToDouble(hourNum));
                aclQuery.time_end = DateTime.Now;
                System.Data.DataTable dclTable = _IDeliverChangeLogDao.GetDeliverChangeLogDataTable(aclQuery);

                System.Data.DataTable dmTable = _IDeliverChangeLogDao.GetDataTable(aclQuery);


                DataTable _dt = new DataTable();
                DataRow dr;
                _dt.Columns.Add("訂單編號", typeof(string));
                _dt.Columns.Add("出貨單號", typeof(string));
                _dt.Columns.Add("供應商名稱", typeof(string));
                _dt.Columns.Add("出貨方式", typeof(string));
                _dt.Columns.Add("異動人", typeof(string));
                _dt.Columns.Add("異動類型", typeof(string));//
                _dt.Columns.Add("異動時間", typeof(string));
                //_dt.Columns.Add("原期望到貨日", typeof(string));
                _dt.Columns.Add("期望到貨日", typeof(string));
                _dt.Columns.Add("期望到貨時段", typeof(string));
                _dt.Columns.Add("備註", typeof(string));
                _dt.Columns.Add("來源IP", typeof(string));


                //自行出貨的供應商
                DataTable deliverDt = new DataTable();
                DataRow deliverDr;
                deliverDt.Columns.Add("訂單編號", typeof(string));
                deliverDt.Columns.Add("出貨單號", typeof(string));
                deliverDt.Columns.Add("付款完成時間", typeof(string));
                deliverDt.Columns.Add("供應商編號", typeof(string));
                deliverDt.Columns.Add("供應商郵箱", typeof(string));
                deliverDt.Columns.Add("供應商名稱", typeof(string));     
                deliverDt.Columns.Add("期望到貨日", typeof(string));
                deliverDt.Columns.Add("期望到貨時段", typeof(string));
                deliverDt.Columns.Add("預計到貨日", typeof(string));
                deliverDt.Columns.Add("備註", typeof(string));
               


                if (dclTable.Rows.Count > 0)
                {
                    #region 循環賦值

                    for (int i = 0; i < dmTable.Rows.Count; i++)
                    {
                        #region 自行出貨的供應商的table賦值
                        deliverDr = deliverDt.NewRow();
                        deliverDr["訂單編號"] = dmTable.Rows[i]["order_id"].ToString();
                        deliverDr["出貨單號"] = dmTable.Rows[i]["deliver_id"].ToString();
                        if (Convert.ToInt32(dmTable.Rows[i]["order_date_pay"]) == 0)
                        {
                            deliverDr["付款完成時間"] = "";
                        }
                        else
                        {
                            deliverDr["付款完成時間"] = CommonFunction.GetNetTime(Convert.ToInt32(dmTable.Rows[i]["order_date_pay"])).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        deliverDr["供應商編號"] = dmTable.Rows[i]["vendor_id"].ToString();
                        deliverDr["供應商郵箱"] = dmTable.Rows[i]["vendor_email"].ToString();
                        deliverDr["供應商名稱"] = dmTable.Rows[i]["vendor_name_full"].ToString();

                        if (Convert.ToDateTime(dmTable.Rows[i]["expect_arrive_date_dm"]).ToString("yyyy-MM-dd") == "0001-01-01")
                        {
                            deliverDr["期望到貨日"] = "";
                        }
                        else
                        {
                            deliverDr["期望到貨日"] = Convert.ToDateTime(dmTable.Rows[i]["expect_arrive_date_dm"]).ToString("yyyy-MM-dd");
                        }
                        //期望到貨時段
                        StringBuilder expectArrivePeriodSb = new StringBuilder();
                        if (dmTable.Rows[i]["expect_arrive_period_dm"].ToString() == "1")
                        {
                            expectArrivePeriodSb.Append("12:00以前");
                        }
                        if (dmTable.Rows[i]["expect_arrive_period_dm"].ToString() == "2")
                        {
                            expectArrivePeriodSb.Append("12:00-17:00");
                        }
                        if (dmTable.Rows[i]["expect_arrive_period_dm"].ToString() == "3")
                        {
                            expectArrivePeriodSb.Append("17:00-20:00");
                        }
                        if (dmTable.Rows[i]["expect_arrive_period_dm"].ToString() == "0")
                        {
                            expectArrivePeriodSb.Append("不限制");
                        }
                        deliverDr["期望到貨時段"] = expectArrivePeriodSb.ToString();
                        if (Convert.ToInt32(dmTable.Rows[i]["deliver_org_days"]) == 0)
                        {
                            deliverDr["預計到貨日"] = "";
                        }
                        else
                        {
                            deliverDr["預計到貨日"] = Convert.ToDateTime(CommonFunction.GetNetTime(Convert.ToInt32(dmTable.Rows[i]["deliver_org_days"]))).ToString("yyyy-MM-dd");
                        }
                        deliverDr["備註"] = dmTable.Rows[i]["dcl_note"].ToString();
                        deliverDt.Rows.Add(deliverDr);                       
                        #endregion
                    }

                    for (int i = 0; i < dclTable.Rows.Count; i++)
                    {                       
                        dr = _dt.NewRow();
                        StringBuilder sb = new StringBuilder();
                        dr["訂單編號"] = dclTable.Rows[i]["order_id"].ToString();
                        dr["出貨單號"] = dclTable.Rows[i]["deliver_id"].ToString();
                        dr["供應商名稱"] = dclTable.Rows[i]["vendor_name_full"].ToString();
                        if (dclTable.Rows[i]["type"].ToString() == "1")
                        {
                            dr["出貨方式"] = "統倉出貨";
                        }
                        else if (dclTable.Rows[i]["type"].ToString() == "2")
                        {
                            dr["出貨方式"] = "供應商自行出貨";
                        }
                        else
                        //if (dclTable.Rows[i]["type"].ToString() == "101")
                        {
                            dr["出貨方式"] = "其他";
                        }

                        if (dclTable.Rows[i]["dcl_create_type"].ToString() == "1")
                        {
                            dr["異動人"] = dclTable.Rows[i]["dcl_create_username"].ToString();
                            dr["異動類型"] = "前台";
                        }
                        else if (dclTable.Rows[i]["dcl_create_type"].ToString() == "2")
                        {
                            dr["異動人"] = dclTable.Rows[i]["dcl_create_musername"].ToString();
                            dr["異動類型"] = "後台";
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

                        if (Convert.ToDateTime(dclTable.Rows[i]["expect_arrive_date_dcl"]).ToString("yyyy-MM-dd") == "0001-01-01")
                        {
                            dr["期望到貨日"] = "";
                        }
                        else
                        {
                            dr["期望到貨日"] = Convert.ToDateTime(dclTable.Rows[i]["expect_arrive_date_dcl"]).ToString("yyyy-MM-dd");
                        }
                        if (dclTable.Rows[i]["expect_arrive_period_dcl"].ToString() == "1")
                        {
                            sb.Append("12:00以前");
                        }
                        if (dclTable.Rows[i]["expect_arrive_period_dcl"].ToString() == "2")
                        {
                            sb.Append("12:00-17:00");
                        }
                        if (dclTable.Rows[i]["expect_arrive_period_dcl"].ToString() == "3")
                        {
                            sb.Append("17:00-20:00");
                        }
                        if (dclTable.Rows[i]["expect_arrive_period_dcl"].ToString() == "0")
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
                    
                    #region 出貨方式為“供應商自行出貨”的出貨單整理后，發郵件給對應的供應商

                    Dictionary<string, string> vendorDictionary = new Dictionary<string, string>();
                    for (int i = 0; i < deliverDt.Rows.Count; i++)
                    {
                        if (!vendorDictionary.ContainsKey(deliverDt.Rows[i]["出貨單號"].ToString()))
                        {
                            vendorDictionary.Add(deliverDt.Rows[i]["出貨單號"].ToString(), deliverDt.Rows[i]["供應商編號"].ToString());
                        }
                    }

                    List<string> SendEmailVendorIdList = new List<string>();
                    
                    foreach (KeyValuePair<string, string> kvp in vendorDictionary)
                    {
                        DataTable deliverDt_1 = deliverDt.Clone();

                        string MailToAddress_1 = string.Empty;
                        string vendor_name_full = string.Empty;
                        if (!SendEmailVendorIdList.Contains(kvp.Value))
                        {
                            for (int i = 0; i < deliverDt.Rows.Count; i++)
                            {
                                DataRow deliverDr_1 = deliverDt_1.NewRow();
                                if (kvp.Value == deliverDt.Rows[i]["供應商編號"].ToString())
                                {
                                    MailToAddress_1 = deliverDt.Rows[i]["供應商郵箱"].ToString();
                                    vendor_name_full = deliverDt.Rows[i]["供應商名稱"].ToString();

                                    deliverDr_1["訂單編號"] = deliverDt.Rows[i]["訂單編號"].ToString();
                                    deliverDr_1["出貨單號"] = deliverDt.Rows[i]["出貨單號"].ToString();
                                    deliverDr_1["付款完成時間"] = deliverDt.Rows[i]["付款完成時間"].ToString();
                                    //deliverDr_1["供應商編號"] = deliverDt.Rows[i]["供應商編號"].ToString();
                                    //deliverDr_1["供應商郵箱"] = deliverDt.Rows[i]["供應商郵箱"].ToString();
                                    //deliverDr_1["供應商名稱"] = deliverDt.Rows[i]["供應商名稱"].ToString();
                                    deliverDr_1["期望到貨日"] = deliverDt.Rows[i]["期望到貨日"].ToString();
                                    deliverDr_1["期望到貨時段"] = deliverDt.Rows[i]["期望到貨時段"].ToString();
                                    deliverDr_1["預計到貨日"] = deliverDt.Rows[i]["預計到貨日"].ToString();
                                    deliverDr_1["備註"] = deliverDt.Rows[i]["備註"].ToString();

                                    deliverDt_1.Rows.Add(deliverDr_1);
                                }
                            }
                            SendEmailVendorIdList.Add(kvp.Value);
                            BLL.gigade.Common.MailModel mailModel_1 = new Common.MailModel();
                            mailModel_1.MysqlConnectionString = mySqlConnectionString;
                            mailModel_1.MailFromAddress = mailModel.MailFromAddress;
                            mailModel_1.MailHost = mailModel.MailHost;
                            mailModel_1.MailPort = mailModel.MailPort;
                            mailModel_1.MailFromUser = mailModel.MailFromUser;
                            mailModel_1.MailFormPwd = mailModel.MailFormPwd;


                            //string MailBody_1 = "<br/><font size=\"4\">" + "<font color=\"#00BB00\" >" + vendor_name_full + "</font>" + " 您好，在前 " + "<font color=\"#FF0000\" >" + Convert.ToDouble(hourNum)
                            //                      + "</font>" + " 個小時之內，貴公司自行出貨的商品出貨單期望到貨日調整記錄如下：</font><br/><p/>" + GetHtmlByDataTable(deliverDt_1);
                            deliverDt_1.Columns.Remove("供應商編號");
                            deliverDt_1.Columns.Remove("供應商郵箱");
                            deliverDt_1.Columns.Remove("供應商名稱");
                            string MailBody_1 = "<p>吉甲地市集【期望到貨日改變】通知信</p><p><font color=\"#00BB00\" >" + vendor_name_full + "</font> 您好：</p>" +
                                "<p>以下訂單已改變出貨單期望到貨日，訂單資訊如下。</p>" +                                    
                                //"<p>============================================================</p>"+
                                            GetHtmlByDataTable(deliverDt_1) +
                                //"<p>============================================================</p>"+
              "<p>訂單的相關資訊，請至<a href='http://vendor.gigade100.com' style='color:#3399ff;text-decoration: none;'>【後台管理】</a>中查詢。</p>"+
              "<p>※本信由系統寄出，請勿直接回覆！</p>"+
              "有任何問題與建議，歡迎聯絡我們<a href='http://www.gigade100.com/contact_service.php' target='_blank'> <img src='http://www.gigade100.com/images/send_mail.jpg'></a>" +
              "<p>吉甲地市集<a href='http://www.gigade100.com/'>http://www.gigade100.com/</a></p>";

                            string MailTitle_1 = MailTitle;
                            //MailToAddress_1 = "zhaozhi0623j@gimg.tw";
                            BLL.gigade.Common.MailHelper mailHelper = new MailHelper(mailModel_1);
                            //public Boolean SendMailAction(string MailToAddress, string MailTitle, string MailBody)
                            try
                            {
                                mailHelper.SendMailAction(MailToAddress_1, MailTitle_1, MailBody_1 + " ");//給單個供應商發送郵件
                            }
                            catch (Exception)
                            {
                                continue;                               
                            }
                        }                     
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

                BLL.gigade.Common.MailHelper mail = new Common.MailHelper(mailModel);                
                return mail.SendToGroup(GroupCode, MailTitle, MailBody + " ", false, true); 
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
