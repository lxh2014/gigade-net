using BLL.gigade.Common;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Schedules
{
    public class UpdateLoginAddressMgr
    {
        static IDBAccess _accessMySql;
        static string mySqlConnectionString = string.Empty;
        public UpdateLoginAddressMgr(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            mySqlConnectionString = connectionString;
        }

        public bool Start(string schedule_code)
        {
            DataTable table = UpdateAddress();
            if(table.Rows.Count>0)
            {
                string mailBody = GetHtmlByDataTable(table);
                MailModel mailModel = new MailModel();
                mailModel.MysqlConnectionString = mySqlConnectionString;
                string GroupCode = string.Empty;
                string MailTitle = string.Empty;
                //獲取該排程參數
                List<ScheduleConfigQuery> store_config = new List<ScheduleConfigQuery>();
                ScheduleConfigQuery query_config = new ScheduleConfigQuery();
                query_config.schedule_code = schedule_code;
                ScheduleServiceMgr _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);
                store_config = _secheduleServiceMgr.GetScheduleConfig(query_config);
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
                }

                MailHelper mail = new MailHelper(mailModel);
                return mail.SendToGroup(GroupCode, MailTitle, mailBody, false, true);
            }
            return true;          
        }

        public DataTable UpdateAddress()
        {
            DataTable othenCityUser = new DataTable();
            othenCityUser.Columns.Add("會員編號", typeof(string));
            othenCityUser.Columns.Add("會員賬號", typeof(string));
            othenCityUser.Columns.Add("IP", typeof(string));
            othenCityUser.Columns.Add("登錄地區", typeof(string));
            //othenCityUser.Columns.Add("登錄時間", typeof(string));

            #region 獲取登錄地區不為空的IP地址
            DataTable addressTable = new DataTable();
            string sqladdressTable = @"SELECT login_ipfrom,login_address,u.user_email,u.user_id,FROM_UNIXTIME(login_createdate) login_createdate FROM users_login ul 
INNER JOIN users u on u.user_id=ul.user_id WHERE login_address<>'' GROUP BY login_ipfrom;";
            addressTable = _accessMySql.getDataTable(sqladdressTable);
            #endregion       
            foreach (DataRow row in addressTable.Rows)
            {
                string loginid = "";
                string sqlloginidTable = string.Format("SELECT login_id FROM users_login WHERE login_address='' AND login_ipfrom='{0}'", row["login_ipfrom"]);
                DataTable loginidTable = _accessMySql.getDataTable(sqlloginidTable);
                foreach (DataRow row1 in loginidTable.Rows)
                {
                    loginid += row1["login_id"].ToString()+",";
                }
                loginid=loginid.TrimEnd(',');
                if(loginid!="")
                {
                    DataRow newRow = othenCityUser.NewRow();
                    newRow["會員編號"] = row["user_id"];
                    newRow["會員賬號"] = row["user_email"];
                    newRow["登錄地區"] = row["login_address"];
                    newRow["IP"] = row["login_ipfrom"];
                    //newRow["登錄時間"] = CommonFunction.DateTimeToString(Convert.ToDateTime(row["login_createdate"]));
                    othenCityUser.Rows.Add(newRow);
                    string sqlUpdateAddress =string.Format("UPDATE users_login set login_address='{0}' WHERE login_id IN({1});",row["login_address"],loginid);
                    _accessMySql.execCommand(sqlUpdateAddress);
                }
            }
            return othenCityUser;
        }

        public static string GetHtmlByDataTable(DataTable _dtmyMonth)
        {
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("<table  cellpadding=3 cellspacing=1  border=1 style=\"border-collapse: collapse\">");
            sbHtml.Append("<tr  style=\"text-align: center; COLOR: #0076C8; BACKGROUND-COLOR: #F4FAFF; font-weight: bold\">");
            string[] str = { "style=\"background-color:#dda29a;\"", "style=\"background-color:#d98722;\"", "style=\"background-color:#cfbd2d;\"", "style=\"background-color:#cbd12c;\"", "style=\"background-color:#91ca15;\"", "style=\"background-color:#6dc71e;\"", "style=\"background-color:#25b25c;\"", "style=\"background-color:#13a7a2;\"", "style=\"background-color:#13b7a2;\"" };
            string aligns = "align=\"right\"";
            string alignsleft = "align=\"left\"";
            for (int i = 0; i < _dtmyMonth.Columns.Count; i++)
            {
                sbHtml.Append("<th ");
                sbHtml.Append(str[i]);
                sbHtml.Append(" >");
                sbHtml.Append(_dtmyMonth.Columns[i].ColumnName);
                sbHtml.Append("</th>");
            }
            sbHtml.Append("</tr>");
            for (int i = 0; i < _dtmyMonth.Rows.Count; i++)//行
            {
                sbHtml.Append("<tr>");
                for (int j = 0; j < _dtmyMonth.Columns.Count; j++)
                {
                    sbHtml.Append("<td ");
                    if (j == 0)
                    {
                        sbHtml.Append(alignsleft);
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

    }
}
