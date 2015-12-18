using BLL.gigade.Common;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using gigadeExcel.Comment;
using NPOI.OpenXml4Net.OPC.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Schedules
{
    public class ReadLogMgr
    {
        private ScheduleServiceMgr _secheduleServiceMgr;
        static IDBAccess _accessMySql;
        static string mySqlConnectionString = string.Empty;
        private string stockExcelsavePath = String.Empty;
        private string stockExcelsavePathSuccess = String.Empty;
        private string stockExcelsavePathFail = String.Empty;
        private string stockExcelsavePathIgnore = String.Empty;
        private ExcelHelperXhf excelHelper = null;
        public IProductItemImplMgr _productItemMgr;
        public ReadLogMgr(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            mySqlConnectionString = connectionString;
        }
        public bool Start(string schedule_code)
        {
            bool result = false;
            FileManagement _fileHelper = new FileManagement();
           
            try
            {
                if (string.IsNullOrEmpty(schedule_code))
                {
                    return result;
                }
                string GroupCode = string.Empty;
                string MailTitle = string.Empty;

                StringBuilder strbody = new StringBuilder();
                MailModel mailModel = new MailModel();
                mailModel.MysqlConnectionString = mySqlConnectionString;
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
                    else if (item.parameterCode.Equals("filepath"))
                    {
                        stockExcelsavePath = item.value.Trim().ToLower();;
                    }
                }
                stockExcelsavePathSuccess = Path.Combine(stockExcelsavePath, "success");
                stockExcelsavePathFail = Path.Combine(stockExcelsavePath, "fail");
                stockExcelsavePathIgnore = Path.Combine(stockExcelsavePath, "ignore");
                #endregion

                MailHelper mail = new MailHelper(mailModel);
                string[] files = _fileHelper.GetAllFiles(stockExcelsavePath, @"*.*");
                string consoleFile = String.Empty;
                string lastFile = files.Max<String>();
                if (files.Length < 1)
                {
                    mail.SendToGroup(GroupCode, MailTitle, "今日沒有文件倒入", false, true);//發送郵件給群組
                }
                #region 創建四個Datata來保存數據
                DataTable _dtSucess = new DataTable();//更新成功的數據
                _dtSucess.Columns.Add("商品編號", typeof(string));
                _dtSucess.Columns.Add("商品細項編號", typeof(string));
                _dtSucess.Columns.Add("商品ERP編號", typeof(string));
                _dtSucess.Columns.Add("商品名稱", typeof(string));
                _dtSucess.Columns.Add("規格", typeof(string));
                _dtSucess.Columns.Add("庫存", typeof(string));
                DataTable _dtFail = new DataTable();//更新失敗的數據
                _dtFail.Columns.Add("商品編號", typeof(string));
                _dtFail.Columns.Add("商品細項編號", typeof(string));
                _dtFail.Columns.Add("商品ERP編號", typeof(string));
                _dtFail.Columns.Add("商品名稱", typeof(string));
                _dtFail.Columns.Add("規格", typeof(string));
                _dtFail.Columns.Add("庫存", typeof(string));
                DataTable _dtIgnore = new DataTable();//跳過更新的數據
                _dtIgnore.Columns.Add("商品ERP編號", typeof(string));
                DataTable _dtErrorTable = new DataTable();//打不開Excel文件的數據
                _dtErrorTable.Columns.Add("路徑", typeof(string));
                _dtErrorTable.Columns.Add("描述", typeof(string));
                #endregion
                foreach (string file in files)
                {
                    if (lastFile == file)//只操作最新的文件
                    {
                      
                        int num = 0;
                        StringBuilder errorLog = GetStockMessageFromFile(file, ref _dtSucess, ref _dtFail, ref _dtIgnore, ref _dtIgnore);
                        if (String.IsNullOrEmpty(errorLog.ToString()))
                        {
                            string newFileName = file.Substring(file.LastIndexOf("\\"));
                            _fileHelper.MoveOneFile(file, stockExcelsavePathSuccess, newFileName);
                            MailTitle = "ERP庫存更新提醒";
                            mail.SendToGroup(GroupCode, MailTitle, "更新成功數量：" + num + "文件名称:" + newFileName, false, true);//發送郵件給群組
                        }
                        else
                        {
                            string newFileName = file.Substring(file.LastIndexOf("\\"));
                            string errorNewFileName = newFileName.Substring(0, newFileName.LastIndexOf(".")) + "_err.xls";
                            string errorlogNewFileName = stockExcelsavePathFail + newFileName.Substring(0, newFileName.LastIndexOf(".")) + "_err.txt";
                            _fileHelper.MoveOneFile(file, stockExcelsavePathFail, errorNewFileName);
                            SaveErrorLog(errorLog, errorlogNewFileName, newFileName);
                            MailTitle = "ERP庫存更新異常提醒";
                            StringBuilder sbMailBody = new StringBuilder();
                            if (_dtSucess.Rows.Count > 0)
                            {
                                sbMailBody.AppendLine("更新成功商品");
                                sbMailBody.AppendLine(GetHtmlByDataTable(_dtSucess));
                            }
                            if (_dtFail.Rows.Count > 0)
                            {
                                sbMailBody.AppendLine("更新失敗商品");
                                sbMailBody.AppendLine(GetHtmlByDataTable(_dtFail));
                            }
                            if (_dtIgnore.Rows.Count > 0)
                            {
                                sbMailBody.AppendLine("跳過更新商品");
                                sbMailBody.AppendLine(GetHtmlByDataTable(_dtIgnore));
                            }
                            if (_dtErrorTable.Rows.Count > 0)
                            {
                                sbMailBody.AppendLine("打不開Excel文件的數據");
                                sbMailBody.AppendLine(GetHtmlByDataTable(_dtErrorTable));
                            }
                            mail.SendToGroup(GroupCode, MailTitle, sbMailBody.ToString(), false, true);//發送郵件給群組

                        }
                            

                        
                    }
                    else//忽略過時文件
                    {
                        string newFileName = file.Substring(file.LastIndexOf("\\"));
                        _fileHelper.MoveOneFile(file, stockExcelsavePathIgnore, newFileName);
                    }

                }

               
               // StringBuilder sbMailBody = new StringBuilder();

                result = true;
            }
            catch (Exception ex)
            {
                throw new Exception("CheckOrderAmount-->Start-->" + ex.Message);
            }
            return result;
        }
        public StringBuilder GetStockMessageFromFile(string fileName, ref DataTable _dtSucess, ref DataTable _dtFail, ref DataTable _dtIgnore, ref DataTable _dtErrorTable)
        {
            StringBuilder sbErrContent = new StringBuilder();
            DataTable dt = null;
            try
            {
                dt = ExcelHelperXhf.ImportExcel2003toDt(fileName);
            }
            catch (Exception)
            {

                try
                {
                    dt = ExcelHelperXhf.ImportExcel2007toDt(fileName);
                }
                catch (Exception)
                {
                    DataRow rows = _dtErrorTable.NewRow();
                    rows[0] = fileName;
                    rows[1] = "這個格式的Excel格式未知，不能讀取";
                    _dtErrorTable.Rows.Add(rows);
                    sbErrContent.AppendLine(fileName + "這個格式的Excel格式未知，不能讀取");
                    //Console.WriteLine("這個格式的Excel格式未知，不能讀取");
                }
            }
            _productItemMgr = new ProductItemMgr(mySqlConnectionString);
            foreach (DataRow r in dt.Rows)
            {

                ProductItemQuery model = new ProductItemQuery();
                model.Erp_Id = r["品號"].ToString();
                model.item_stock = Convert.ToInt32(r["可用量"].ToString());
                //查詢atm數量
                int atmNumber = _productItemMgr.GetATMStock(model);/*ATM的數量*/
                int useNumber = Convert.ToInt32(model.item_stock) - atmNumber;
                //int olditem_stock = Convert.ToInt32(model.item_stock);
                model.item_stock = useNumber;
                try
                {
                    List<ProductItemQuery> ItemList = new List<ProductItemQuery>();
                    ItemList = _productItemMgr.GetProdItemByERp(model);
                    if (ItemList.Count > 0 && ItemList[0].product_id != 0)
                    {

                        if (atmNumber > 0)
                        {//扣除ATM的
                            #region MyRegion
                            if (model.item_stock == ItemList[0].item_stock)
                            {
                                DataRow rows = _dtSucess.NewRow();
                                rows[0] = ItemList[0].product_id;
                                rows[1] = ItemList[0].item_id;
                                rows[2] = model.Erp_Id;
                                rows[3] = ItemList[0].product_name;
                                rows[4] = ItemList[0].Spec_Name_1 + ItemList[0].Spec_Name_2;
                                rows[5] = string.Format(" {0} ", model.item_stock);
                                _dtSucess.Rows.Add(rows);
                                sbErrContent.AppendLine(string.Format("erp_id為{0}的商品庫存變為{1}", model.Erp_Id, model.item_stock));
                            }
                            else
                            {

                                if (_productItemMgr.UpdateStockAsErpId(model) > 0)
                                {
                                    DataRow rows = _dtSucess.NewRow();
                                    rows[0] = ItemList[0].product_id;
                                    rows[1] = ItemList[0].item_id;
                                    rows[2] = model.Erp_Id;
                                    rows[3] = ItemList[0].product_name;
                                    rows[4] = ItemList[0].Spec_Name_1 + ItemList[0].Spec_Name_2;
                                    rows[5] = string.Format(" {0}扣除ATM庫存為{1} ", model.item_stock + atmNumber, model.item_stock);
                                    _dtSucess.Rows.Add(rows);
                                    sbErrContent.AppendLine(string.Format("erp_id為{0}的商品扣除ATM庫存{1}", model.Erp_Id, atmNumber));
                                }
                                else
                                {
                                    DataRow rows = _dtFail.NewRow();
                                    rows[0] = ItemList[0].product_id;
                                    rows[1] = ItemList[0].item_id;
                                    rows[2] = model.Erp_Id;
                                    rows[3] = ItemList[0].product_name;
                                    rows[4] = ItemList[0].Spec_Name_1 + ItemList[0].Spec_Name_2;
                                    rows[5] = string.Format(" 由{0}更新為{1}失敗! ", ItemList[0].item_stock, model.item_stock);
                                    _dtFail.Rows.Add(rows);
                                    sbErrContent.AppendLine(string.Format("erp_id為{0}的更新失敗", model.Erp_Id));
                                }
                            }
                            #endregion
                        }
                        else 
                        {//不扣除ATM的，可能需要更新，可能不需要更新
                            #region MyRegion
                            
                           
                            if (ItemList[0].item_stock == model.item_stock)//不需要更新的
                            {
                                DataRow rows = _dtSucess.NewRow();
                                rows[0] = ItemList[0].product_id;
                                rows[1] = ItemList[0].item_id;
                                rows[2] = model.Erp_Id;
                                rows[3] = ItemList[0].product_name;
                                rows[4] = ItemList[0].Spec_Name_1 + ItemList[0].Spec_Name_2;
                                rows[5] = string.Format(" {0} ", model.item_stock);
                                _dtSucess.Rows.Add(rows);
                                sbErrContent.AppendLine(string.Format("erp_id為{0}的商品庫存變為{1}", model.Erp_Id, model.item_stock));
                            }
                            else //需要更新的
                            {
                                if (_productItemMgr.UpdateStockAsErpId(model) > 0)
                                {
                                    DataRow rows = _dtSucess.NewRow();
                                    rows[0] = ItemList[0].product_id;
                                    rows[1] = ItemList[0].item_id;
                                    rows[2] = model.Erp_Id;
                                    rows[3] = ItemList[0].product_name;
                                    rows[4] = ItemList[0].Spec_Name_1 + ItemList[0].Spec_Name_2;
                                    rows[5] = string.Format(" 由{0}更新為{1}成功! ", ItemList[0].item_stock, model.item_stock);
                                    _dtSucess.Rows.Add(rows);
                                    sbErrContent.AppendLine(string.Format("erp_id為{0}的商品庫存變為{1}", model.Erp_Id, model.item_stock));
                                }
                                else
                                {
                                    DataRow rows = _dtFail.NewRow();
                                    rows[0] = ItemList[0].product_id;
                                    rows[1] = ItemList[0].item_id;
                                    rows[2] = model.Erp_Id;
                                    rows[3] = ItemList[0].product_name;
                                    rows[4] = ItemList[0].Spec_Name_1 + ItemList[0].Spec_Name_2;
                                    rows[5] = string.Format(" 由{0}更新為{1}失敗! ", ItemList[0].item_stock, model.item_stock);
                                    _dtFail.Rows.Add(rows);
                                    sbErrContent.AppendLine(string.Format("erp_id為{0}的更新失敗", model.Erp_Id));
                                }
                            }
                            #endregion
                        }


                        //if (_productItemMgr.UpdateStockAsErpId(model) > 0)
                        //{//更新成功的
                        //    if (atmNumber > 0)
                        //    {//扣除ATM的
                        //        DataRow rows = _dtSucess.NewRow();
                        //        rows[0] = ItemList[0].product_id;
                        //        rows[1] = ItemList[0].item_id;
                        //        rows[2] = model.Erp_Id;
                        //        rows[3] = ItemList[0].product_name;
                        //        rows[4] = ItemList[0].Spec_Name_1 + ItemList[0].Spec_Name_2;
                        //        rows[5] = string.Format(" {0}扣除ATM庫存為{1} ", model.item_stock + atmNumber, model.item_stock); 
                        //        _dtSucess.Rows.Add(rows);
                        //        sbErrContent.AppendLine(string.Format("erp_id為{0}的商品扣除ATM庫存{1}", model.Erp_Id, atmNumber));
                        //    }
                        //    else 
                        //    {
                        //        DataRow rows = _dtSucess.NewRow();
                        //        rows[0] = ItemList[0].product_id;
                        //        rows[1] = ItemList[0].item_id;
                        //        rows[2] = model.Erp_Id;
                        //        rows[3] = ItemList[0].product_name;
                        //        rows[4] = ItemList[0].Spec_Name_1 + ItemList[0].Spec_Name_2;
                        //        rows[5] = string.Format(" 由{0}更新為{1}成功! ", ItemList[0].item_stock, model.item_stock); 
                        //        _dtSucess.Rows.Add(rows);
                        //        sbErrContent.AppendLine(string.Format("erp_id為{0}的商品庫存變為{1}", model.Erp_Id, model.item_stock));
                        //    }
                        //}
                        //else 
                        //{//更新失敗的
                        //    DataRow rows = _dtFail.NewRow();
                        //    rows[0] = ItemList[0].product_id;
                        //    rows[1] = ItemList[0].item_id;
                        //    rows[2] = model.Erp_Id;
                        //    rows[3] = ItemList[0].product_name;
                        //    rows[4] = ItemList[0].Spec_Name_1 + ItemList[0].Spec_Name_2;
                        //    rows[5] = string.Format(" 由{0}更新為{1}失敗! ", ItemList[0].item_stock, model.item_stock);
                        //    _dtFail.Rows.Add(rows);
                        //    sbErrContent.AppendLine(string.Format("erp_id為{0}的更新失敗", model.Erp_Id));
                        //}
                    }
                    else 
                    {
                        DataRow rows = _dtIgnore.NewRow();
                        rows[0] = model.Erp_Id;
                        _dtIgnore.Rows.Add(rows);
                        sbErrContent.AppendLine(string.Format("erp_id為{0}的跳過", model.Erp_Id));
                    }
                    
                }
                catch (Exception ex)
                {
                    throw new Exception("CheckOrderAmount-->GetStockMessageFromFile-->" + ex.Message);
                }
            }

            return sbErrContent;
        }



        #region 保存日誌 +void SaveErrorLog(StringBuilder sbContent, string fileName, string newFileName)
        /// <summary>
        /// 保存日誌
        /// </summary>
        /// <param name="sbContent"></param>
        /// <param name="fileName"></param>
        /// <param name="newFileName"></param>
        private void SaveErrorLog(StringBuilder sbContent, string fileName, string newFileName)
        {
            StreamWriter sw = new StreamWriter(fileName);
            sw.Write(sbContent.ToString());
            sw.Close();
            //發送郵件
            try
            {
                //SendMail mail = new SendMail();
                //mail.MailTitle = "ERP庫存更新異常提醒";
                //mail.MailBody = sbContent.ToString();
                //mail.SendAllMail();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region DataTable轉Html +string GetHtmlByDataTable(DataTable _dtmyMonth)
        /// <summary>
        /// DataTable轉Html
        /// </summary>
        /// <param name="_dtmyMonth"></param>
        /// <returns></returns>
        public string GetHtmlByDataTable(DataTable _dtmyMonth)
        {
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("<table  cellpadding=3 cellspacing=1  border=1 style=\"border-collapse: collapse\">");
            sbHtml.Append("<tr  style=\"text-align: center; COLOR: #0076C8; BACKGROUND-COLOR: #F4FAFF; font-weight: bold\">");
            string[] str = { "style=\"background-color:#dda29a;\"", "style=\"background-color:#d98722;\"", "style=\"background-color:#cfbd2d;\"", "style=\"background-color:#cbd12c;\"", "style=\"background-color:#91ca15;\"", "style=\"background-color:#6dc71e;\"", "style=\"background-color:#25b25c;\"", "style=\"background-color:#13a7a2;\"" };
            string aligns = "align=\"right\"";
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
                    sbHtml.Append(aligns);
                    sbHtml.Append(" >");
                    sbHtml.Append(_dtmyMonth.Rows[i][j]);
                    sbHtml.Append("</td>");
                }
                sbHtml.Append("</tr>");
            }
            sbHtml.Append("</table>");
            return sbHtml.ToString();

        }
        #endregion

    }
}
