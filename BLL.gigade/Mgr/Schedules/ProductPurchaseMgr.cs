using BLL.gigade.Common;
using BLL.gigade.Dao;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Schedules
{
    public class ProductPurchaseMgr
    {
        private ScheduleServiceMgr _secheduleServiceMgr;
        static IDBAccess _accessMySql;
        static string mySqlConnectionString = string.Empty;
        public IProductItemImplMgr _productItemMgr;
        BLL.gigade.Mgr.ProductItemMgr productItemMgr = new BLL.gigade.Mgr.ProductItemMgr(mySqlConnectionString);
        IParametersrcImplMgr _paraMgr;
        public ProductPurchaseMgr(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            mySqlConnectionString = connectionString;
        }
        public bool Start(string schedule_code)
        {
            bool result = false;

            try
            {
                if (string.IsNullOrEmpty(schedule_code))
                {
                    return result;
                }
                productItemMgr = new ProductItemMgr(mySqlConnectionString);
                _paraMgr = new ParameterMgr(mySqlConnectionString);
                ProductItemQuery query = new ProductItemQuery();
                ArrivalNotice arriva = new ArrivalNotice();
                MailModel mailModel = new MailModel();
                mailModel.MysqlConnectionString = mySqlConnectionString;
                string GroupCode = string.Empty;
                string MailTitle = string.Empty;
                string MailBody = string.Empty;
                string NOSuggestCountMsg = "今天沒有要採購的商品";//沒有要採購的商品提示
                string sumDays = "60";//採購總天數
                string periodDays = "1";//採購週期天數
                ScheduleServiceMgr _secheduleServiceMgr;

                //獲取該排程參數
                List<ScheduleConfigQuery> store_config = new List<ScheduleConfigQuery>();
                ScheduleConfigQuery query_config = new ScheduleConfigQuery();
                query_config.schedule_code = schedule_code;
                _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);
                store_config = _secheduleServiceMgr.GetScheduleConfig(query_config);
                #region FTP參數賦值
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
                    else if (item.parameterCode.Equals("MailTitle"))
                    {
                        MailTitle = item.value;
                    }
                    else if (item.parameterCode.Equals("MailBody"))
                    {
                        MailBody = item.value;
                    }
                    else if (item.parameterCode.Equals("sumDays"))
                    {
                        sumDays = item.value;
                    }
                    else if (item.parameterCode.Equals("periodDays"))
                    {
                        periodDays = item.value;
                    }
                    else if (item.parameterCode.Equals("NOSuggestCountMsg"))
                    {
                        NOSuggestCountMsg = item.value;
                    }
                }
                #endregion
                Parametersrc p = new Parametersrc();
                List<Parametersrc> list = new List<Parametersrc>();
                p.ParameterType = "Food_Articles";
                list = _paraMgr.GetAllKindType(p.ParameterType);
                for (int i = 0; i < list.Count; i++)/*要禁用的食品錧和用品館的商品*/
                {
                    if (!string.IsNullOrEmpty(list[i].ParameterCode))
                    {
                        query.category_ID_IN += list[i].ParameterCode + ",";
                    }
                }
                query.sumDays = int.Parse(sumDays);
                query.periodDays = int.Parse(periodDays);
                query.category_ID_IN = query.category_ID_IN.TrimEnd(',');
                query.sale_status = 100;
                query.Is_pod = 0;
                query.stockScope = 2;
                query.prepaid = -1;
                query.IsPage = false;
                int totalCount = 0;

                DataTable dt = productItemMgr.GetSuggestPurchaseInfo(query, out totalCount);
                MailHelper mail = new MailHelper(mailModel);
                if (dt.Rows.Count > 0)
                {
                    DataTable dtExcel = new DataTable();
                    dtExcel.Columns.Add("行號", typeof(String));
                    dtExcel.Columns.Add("供應商編號", typeof(String));
                    dtExcel.Columns.Add("供應商名稱", typeof(String));//
                    dtExcel.Columns.Add("商品編號", typeof(String));
                    dtExcel.Columns.Add("商品細項編號", typeof(String));
                    dtExcel.Columns.Add("商品ERP編號", typeof(String));
                    dtExcel.Columns.Add("商品名稱", typeof(String));
                    //dtExcel.Columns.Add("商品狀態", typeof(String));
                    //dtExcel.Columns.Add("販售狀態", typeof(String));
                    dtExcel.Columns.Add("規格", typeof(String));
                    // dtExcel.Columns.Add("規格二", typeof(String));
                    dtExcel.Columns.Add("庫存量", typeof(String));
                    dtExcel.Columns.Add("後台庫存量", typeof(String));
                    dtExcel.Columns.Add("安全存量", typeof(String));
                    dtExcel.Columns.Add("建議採購量", typeof(String));
                    dtExcel.Columns.Add("補貨通知人數", typeof(String));
                    dtExcel.Columns.Add("成本", typeof(String));
                    dtExcel.Columns.Add("價格", typeof(String));
                    dtExcel.Columns.Add("上架時間", typeof(String));
                    dtExcel.Columns.Add("下架時間", typeof(String));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        #region
                        DataRow newRow = dtExcel.NewRow();
                        newRow[0] = i + 1;
                        newRow[1] = Convert.ToInt64(dt.Rows[i]["vendor_id"]);
                        newRow[2] = dt.Rows[i]["vendor_name_full"];
                        newRow[3] = dt.Rows[i]["product_id"];
                        newRow[4] = dt.Rows[i]["item_id"];
                        newRow[5] = dt.Rows[i]["erp_id"];
                        newRow[6] = dt.Rows[i]["product_name"];
                        //newRow[7] = dt.Rows[i]["product_status_string"];
                        //newRow[8] = dt.Rows[i]["sale_name"];
                        newRow[7] = dt.Rows[i]["spec_title_1"];

                        newRow[8] = dt.Rows[i]["item_stock"];
                        newRow[9] = dt.Rows[i]["iinvd_stock"];
                        newRow[10] = dt.Rows[i]["item_alarm"];
                        if (string.IsNullOrEmpty(dt.Rows[i]["sum_total"].ToString()))
                        {
                            newRow[11] = 0;
                        }
                        #region
                        else
                        {
                            double sum_total = 0;
                            int safe_stock_amount = 0;
                            int item_stock = 0;
                            int item_alarm = 0;
                            int procurement_days = 0;
                            if (double.TryParse(dt.Rows[i]["sum_total"].ToString(), out sum_total))
                            {
                                sum_total = Convert.ToDouble(dt.Rows[i]["sum_total"]);
                            }
                            if (int.TryParse(dt.Rows[i]["safe_stock_amount"].ToString(), out safe_stock_amount))
                            {
                                safe_stock_amount = Convert.ToInt32(dt.Rows[i]["safe_stock_amount"]);
                            }
                            if (int.TryParse(dt.Rows[i]["item_stock"].ToString(), out item_stock))
                            {
                                item_stock = Convert.ToInt32(dt.Rows[i]["item_stock"]);
                            }
                            if (int.TryParse(dt.Rows[i]["item_alarm"].ToString(), out item_alarm))
                            {
                                item_alarm = Convert.ToInt32(dt.Rows[i]["item_alarm"]);
                            }
                            if (int.TryParse(dt.Rows[i]["procurement_days"].ToString(), out procurement_days))
                            {
                                procurement_days = Convert.ToInt32(dt.Rows[i]["procurement_days"]);
                            }
                            if (item_stock - procurement_days * sum_total / query.sumDays * query.periodDays <= item_alarm)
                            {
                                //建議採購量:供應商的進貨天數*採購調整系數*近3個月的平均每周銷售數量(最小值為1)

                                double suggestPurchaseTemp = (procurement_days + safe_stock_amount) * (sum_total / query.sumDays) * query.periodDays + ((item_alarm - item_stock) > 0 ? (item_alarm - item_stock) : 0);

                                if (suggestPurchaseTemp <= int.Parse(dt.Rows[i]["min_purchase_amount"].ToString()))   //最小值為1
                                {
                                    newRow[11] = dt.Rows[i]["min_purchase_amount"];
                                }
                                else
                                {
                                    int suggestPurchase = Convert.ToInt32(suggestPurchaseTemp);
                                    if (suggestPurchase < suggestPurchaseTemp)
                                    {
                                        newRow[11] = Convert.ToInt32(suggestPurchaseTemp) + 1;
                                    }
                                    else
                                    {
                                        newRow[11] = Convert.ToInt32(suggestPurchaseTemp);
                                    }
                                }
                            }

                        }
                        #endregion
                        newRow[12] = dt.Rows[i]["NoticeGoods"];

                        newRow[13] = dt.Rows[i]["item_cost"];
                        newRow[14] = dt.Rows[i]["item_money"];
                        newRow[15] = string.IsNullOrEmpty(dt.Rows[i]["product_start"].ToString()) ? " " : DateTime.Parse(dt.Rows[i]["product_start"].ToString()).ToString("yyyy-MM-dd hh:mm:ss");
                        newRow[16] = string.IsNullOrEmpty(dt.Rows[i]["product_end"].ToString()) ? "" : DateTime.Parse(dt.Rows[i]["product_end"].ToString()).ToString("yyyy-MM-dd hh:mm:ss");
                        dtExcel.Rows.Add(newRow);
                        #endregion
                    }


                    string EmailContent = GetMail(dtExcel);
                    mail.SendToGroup(GroupCode, MailTitle, EmailContent, false, true);//發送郵件給群組
                }
                else
                {
                    mail.SendToGroup(GroupCode, MailTitle, NOSuggestCountMsg, false, true);//發送郵件給群組 
                }
                result = true;
            }
            catch (Exception ex)
            {
                throw new Exception("SendOrderInfoToBlackCatFTPMgr-->Start-->" + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 郵件內容-
        /// </summary>
        /// <param name="dtExcel"></param>
        /// <returns></returns>
        public static string GetMail(DataTable dtExcel)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("<table style='border:1px solid #ccc'><thead style='background-color:#B3D4FF'><tr >");
            sb.AppendFormat("<td>{0}</td>", dtExcel.Columns[0]);
            sb.AppendFormat("<td>{0}</td>", dtExcel.Columns[1]);
            sb.AppendFormat("<td>{0}</td>", dtExcel.Columns[2]);
            sb.AppendFormat("<td>{0}</td>", dtExcel.Columns[3]);
            sb.AppendFormat("<td>{0}</td>", dtExcel.Columns[4]);
            sb.AppendFormat("<td>{0}</td>", dtExcel.Columns[5]);
            sb.AppendFormat("<td>{0}</td>", dtExcel.Columns[6]);
            sb.AppendFormat("<td>{0}</td>", dtExcel.Columns[7]);
            sb.AppendFormat("<td>{0}</td>", dtExcel.Columns[8]);
            sb.AppendFormat("<td>{0}</td>", dtExcel.Columns[9]);
            sb.AppendFormat("<td>{0}</td>", dtExcel.Columns[10]);
            sb.AppendFormat("<td>{0}</td>", dtExcel.Columns[11]);
            sb.AppendFormat("<td>{0}</td>", dtExcel.Columns[12]);
            sb.AppendFormat("<td>{0}</td>", dtExcel.Columns[13]);
            sb.AppendFormat("<td>{0}</td>", dtExcel.Columns[14]);
            sb.AppendFormat("<td>{0}</td>", dtExcel.Columns[15]);
            sb.AppendFormat("<td>{0}</td>", dtExcel.Columns[16]);
            //sb.AppendFormat("<td>{0}</td>", dtExcel.Columns[17]);
            //sb.AppendFormat("<td>{0}</td>", dtExcel.Columns[18]);
            sb.AppendFormat("</tr></thead>");
            int i = 0;
            foreach (DataRow item in dtExcel.Rows)
            {
                if (i % 2 != 0)
                {
                    sb.Append("<tr>");
                }
                else
                {
                    sb.Append("<tr style='background-color:#E8E8EC'>");
                }
                sb.AppendFormat("<td>{0}</td>", item[0]);
                sb.AppendFormat("<td>{0}</td>", item[1]);
                sb.AppendFormat("<td>{0}</td>", item[2]);
                sb.AppendFormat("<td>{0}</td>", item[3]);
                sb.AppendFormat("<td>{0}</td>", item[4]);
                sb.AppendFormat("<td>{0}</td>", item[5]);
                sb.AppendFormat("<td>{0}</td>", item[6]);
                sb.AppendFormat("<td>{0}</td>", item[7]);
                sb.AppendFormat("<td>{0}</td>", item[8]);
                sb.AppendFormat("<td>{0}</td>", item[9]);
                sb.AppendFormat("<td>{0}</td>", item[10]);
                sb.AppendFormat("<td>{0}</td>", item[11]);
                sb.AppendFormat("<td>{0}</td>", item[12]);
                sb.AppendFormat("<td>{0}</td>", item[13]);
                sb.AppendFormat("<td>{0}</td>", item[14]);
                sb.AppendFormat("<td>{0}</td>", item[15]);
                sb.AppendFormat("<td>{0}</td>", item[16]);
                //sb.AppendFormat("<td>{0}</td>", item[17]);
                //sb.AppendFormat("<td>{0}</td>", item[18]);
                sb.Append("</tr>");
                i++;
            }
            sb.Append("</table>");
            return sb.ToString();

        }



    }
}
