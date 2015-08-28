using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class ViewCheckAlarmMgr : IViewCheckAlarmImplMgr
    {
        private string connectionStr;
        private IViewCheckAlarmImplDao _viewAlarmDao;
        public ViewCheckAlarmMgr(string connectionStr)
        {
            _viewAlarmDao = new ViewCheckAlarmDao(connectionStr);
            this.connectionStr = connectionStr;
        }
        public string QueryStockAlarm()
        {
            System.Data.DataTable alarmDt = _viewAlarmDao.QueryStockAlarm();
            StringBuilder mailBody = new StringBuilder("");
            if (alarmDt.Rows.Count != 0)
            {
                mailBody = new StringBuilder("<table style=\"border:solid 1px black; border-collapse:collapse\">");
                mailBody.Append("<th style=\"width:500px;border:solid 1px gray\">品牌名稱</th><th style=\"width:500px;border:solid 1px gray\">商品名稱</th><th style=\"border:solid 1px gray\">出貨方式</th><th style=\"border:solid 1px gray\">是否買斷</th><th style=\"border:solid 1px gray\">商品編號</th><th style=\"border:solid 1px gray\">商品細項編號</th><th style=\"border:solid 1px gray\">規格</th><th style=\"border:solid 1px gray\">庫存</th><th style=\"border:solid 1px gray\">安全存量</th>");
                foreach (System.Data.DataRow item in alarmDt.Rows)
                {
                    mailBody.AppendFormat("<tr><td style=\"border:solid 1px gray\">{0}</td>", item["brand_name"]);//品牌 add by hufeng0813w 2014/07/07
                    mailBody.AppendFormat("<td style=\"width:500px;border:solid 1px gray\"><a href=\"www.baidu.com\">{0}</a></td>", item["product_name"]);
                    mailBody.AppendFormat("<td style=\"border:solid 1px gray\">{0}</td>", item["product_mode"]);//出貨方式(寄倉,自出,調度)     add by xiangwang0413w 2015/01/13
                    mailBody.AppendFormat("<td style=\"border:solid 1px gray\"><input type=\"checkbox\" {0}  disabled=\"disabled\" /></td>", item["prepaid"].ToString() == "1" ? "checked" : "");//是否買斷(買斷商品打勾)    add by xiangwang0413w 2015/01/13
                    mailBody.AppendFormat("<td style=\"border:solid 1px gray\">{0}</td>", item["product_id"]);
                    mailBody.AppendFormat("<td style=\"border:solid 1px gray\">{0}</td>", item["item_id"]);
                    string spec_name = "";
                    if (item["spec1_name"].ToString() != "" && item["spec2_name"].ToString() != "")
                    {
                        spec_name = item["spec1_name"].ToString() + "+" + item["spec2_name"].ToString();
                    }
                    else if (item["spec1_name"].ToString() != "")
                    {
                        spec_name = item["spec1_name"].ToString();
                    }
                    else if (item["spec2_name"].ToString() != "")
                    {
                        spec_name = item["spec2_name"].ToString();
                    }
                    else
                    {
                        spec_name = "無規格";
                    }

                    mailBody.AppendFormat("<td style=\"border:solid 1px gray\">{0}</td>", spec_name);
                    mailBody.AppendFormat("<td style=\"border:solid 1px gray\">{0}</td>", item["item_stock"]);
                    mailBody.AppendFormat("<td style=\"border:solid 1px gray\">{0}</td></tr>", item["item_alarm"]);//安全存量       add by xiangwang0413w 2015/01/13
                }
                mailBody.Append("</table>");
            }

            return mailBody.ToString();
        }
    }
}
