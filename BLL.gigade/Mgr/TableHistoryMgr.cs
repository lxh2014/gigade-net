/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：TableHistoryMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/22 16:05:39 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model.Attributes;
using BLL.gigade.Model;
using System.Collections;
using MySql.Data.MySqlClient;
using System.Data;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Mgr
{
    public class TableHistoryMgr : ITableHistoryImplMgr
    {
        private TableHistoryDao _tableHistoryDao;
        private ITableHistoryItemImplMgr tableHistoryItemMgr;
        private IHistoryBatchImplMgr historyBatchMgr;
        private string connStr;
        private MySqlConnection mycon = new MySqlConnection();//用來查找庫的名稱
        public DBTableInfo tableInfo;
        public TableHistoryMgr(string connectionStr)
        {
            mycon.ConnectionString = connectionStr;
            _tableHistoryDao = new TableHistoryDao(connectionStr);
            tableHistoryItemMgr = new TableHistoryItemMgr(connectionStr);
            historyBatchMgr = new HistoryBatchMgr(connectionStr);
            this.connStr = connectionStr;
        }


        #region ITableHistoryImplMgr 成员

        public int QueryExists(TableHistory tableHistory)
        {
            try
            {
                return _tableHistoryDao.QueryExists(tableHistory);
            }
            catch (Exception ex)
            {

                throw new Exception("TableHistoryMgr.QueryExists-->" + ex.Message, ex);
            }

        }

        public string Save(TableHistory tableHistory)
        {
            try
            {
                return _tableHistoryDao.Save(tableHistory);
            }
            catch (Exception ex)
            {

                throw new Exception("TableHistoryMgr.SingleCompareSave-->" + ex.Message, ex);
            }

        }

        public string Query_TB_PK(string tb_Name)
        {
            try
            {
                return _tableHistoryDao.Query_TB_PK(tb_Name);
            }
            catch (Exception ex)
            {

                throw new Exception("TableHistoryMgr.Query_TB_PK-->" + ex.Message, ex);
            }

        }

        public List<Model.Column> Query_COL_Comment(string tb_Name, string table_schema)
        {
            try
            {
                return _tableHistoryDao.Query_COL_Comment(tb_Name, table_schema);
            }
            catch (Exception ex)
            {
                throw new Exception("TableHistoryMgr.Query_COL_Comment-->" + ex.Message, ex);
            }

        }
        public List<Model.Column> Query_COL_Comment(string tb_Name)
        {
            try
            {
                return _tableHistoryDao.Query_COL_Comment(tb_Name, mycon.Database);
            }
            catch (Exception ex)
            {
                throw new Exception("TableHistoryMgr.Query_COL_Comment-->" + ex.Message, ex);
            }

        }

        public List<Model.TableHistory> Query(Model.TableHistory query)
        {
            return _tableHistoryDao.Query(query);
        }

        public Model.TableHistory QueryLastModifyByProductId(Model.TableHistory query)
        {
            return _tableHistoryDao.QueryLastModifyByProductId(query);
        }


        public bool SaveHistory<T>(T current, HistoryBatch batch, ArrayList excuteSqls, List<string> cols = null, List<Column> columns = null, string pkName = null)
        {
            try
            {
                if (current == null || batch == null)
                {
                    return false;
                }

                //Type curInfo = current.GetType();
                DBTableInfo tableInfo = Common.CommonFunction.GetDBInfo<T>();
                if (tableInfo == null)
                {
                    throw new Exception("TableHistoryMgr.SaveHistory-->Null DBTableInfo Exception!");
                }
                TableHistory history = new TableHistory { table_name = tableInfo.DBName, batchno = batch.batchno };
                if (pkName == null)
                {
                    pkName = Query_TB_PK(history.table_name);
                }
                history.pk_name = pkName;
                history.functionid = batch.functionid;
                if (columns == null)
                {
                    columns = Query_COL_Comment(history.table_name, mycon.Database);
                }

                ArrayList histories = new ArrayList();
                ArrayList historyItems = new ArrayList();
                ArrayList otherSql = new ArrayList();
                //取得表內需記錄的字段

                if (cols == null)
                {
                    IParametersrcImplMgr parametersrcMgr = new ParameterMgr(connStr);
                    var paras = parametersrcMgr.QueryUsed(new Parametersrc { ParameterType = "ColumnHistory", ParameterCode = history.table_name, Used = 1 });
                    cols = (from p in paras
                            select p.parameterName.Trim().ToLower()).ToList();
                }

                Dictionary<string, string> curProp = new Dictionary<string, string>();
                Dictionary<string, string> oldProp = new Dictionary<string, string>();//curProp的值會改變,需要一個Dictionary保存所有信息
                if (cols != null && cols.Count != 0)
                {
                    string selCols = string.Empty; cols.ForEach(m => selCols += m.ToString() + ",");
                    selCols = selCols.Remove(selCols.Length - 1, 1);

                    //取得屬性集合,篩選需歷史記錄的屬性名
                    curProp = GetPropertiesInfo<T>(current);
                    oldProp = curProp;
                    history.pk_value = curProp.Where(m => m.Key == history.pk_name.ToLower()).FirstOrDefault().Value;//取出主鍵值
                    if (string.IsNullOrEmpty(history.pk_name))
                    {
                        return false;
                    }
                    curProp = curProp.Where(m => cols.Exists(n => n.ToString() == m.Key)).ToDictionary(m => m.Key, m => m.Value);//從新數據中取出需要記錄的欄位

                    string itemsSql = string.Empty;
                    int tableHistoryId = -1;
                    List<TableHistoryItem> newItems = null;
                    //目前未執行修改，可以查詢出修改前數據
                    System.Data.DataTable _dt = _tableHistoryDao.ExcuteSql(string.Format("select {0} from {1} where {2}='{3}'", selCols, history.table_name, history.pk_name, history.pk_value));
                    tableHistoryId = QueryExists(history);//是否存在歷史記錄
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        Dictionary<string, string> oldProperites = OldDataInfo(_dt, cols);//修改前屬性集合,只包括需要記錄的欄位

                        newItems = CompareUpdate(oldProperites, curProp);//對比取得修改記錄
                        if (newItems != null && newItems.Count != 0)
                        {
                            if (tableHistoryId != -1)
                            {
                                //存在歷史記錄則 修改原記錄狀態為0
                                newItems.ForEach(m => m.tablehistoryid = tableHistoryId);
                                //newItems.ForEach(m => otherSql.Add(tableHistoryItemMgr.UpdateType(m)));//修改語句加至隊列
                                otherSql.Add(tableHistoryItemMgr.UpdateType(newItems, tableHistoryId));
                            }
                            else
                            {
                                //不存在則先添加舊記錄(狀態為0)
                                List<TableHistoryItem> oldHistoryItems = GetHistoryItems(oldProperites, 0);
                                oldHistoryItems.FindAll(m => !newItems.Exists(n => n.col_name == m.col_name)).ForEach(m => m.type = 1);

                                histories.Add(Save(history));//將保存tablehistory語句添加至隊列

                                //oldHistoryItems.ForEach(m => m.col_chsname = Query_COL_Comment(history.table_name, m.col_name, mycon.Database));
                                //oldHistoryItems.ForEach(m => itemsSql += tableHistoryItemMgr.Save(m));
                                foreach (var item in oldHistoryItems)
                                {
                                    Column col = columns.FirstOrDefault(o => o.Column_Name == item.col_name);
                                    //item.col_chsname = col == null ? Resource.CoreMessage.GetResource("NO_DESCRIPTION") : col.ColumnComment;
                                    item.col_chsname = col == null ? "No Description" : col.Column_Comment;
                                    itemsSql += tableHistoryItemMgr.Save(item);//插入對比后的記錄
                                }
                                historyItems.Add(itemsSql);//保存historyitem語句添加至隊列
                            }
                            histories.Add(Save(history));//將保存tablehistory語句添加至隊列


                            itemsSql = string.Empty;
                            //newItems.ForEach(m => m.col_chsname = columns.First(o => o.Column_Name == m.col_name).ColumnComment);
                            //newItems.ForEach(m => m.col_chsname = columns.FindAll(o => o.Column_Name == m.col_name).FirstOrDefault().ColumnComment);//Query_COL_Comment(history.table_name, m.col_name, mycon.Database));
                            //newItems.ForEach(m => itemsSql += tableHistoryItemMgr.Save(m));//插入對比后的記錄
                            foreach (var item in newItems)
                            {
                                Column col = columns.FirstOrDefault(o => o.Column_Name == item.col_name);
                                //item.col_chsname = col == null ? Resource.CoreMessage.GetResource("NO_DESCRIPTION") : col.ColumnComment;
                                item.col_chsname = col == null ? "No Description" : col.Column_Comment;
                                itemsSql += tableHistoryItemMgr.Save(item);//插入對比后的記錄
                            }
                            historyItems.Add(itemsSql);//保存historyitem語句添加至隊列
                        }
                        else
                        {
                            if (tableHistoryId == -1)
                            {
                                histories.Add(Save(history));//將保存tablehistory語句添加至隊列

                                List<TableHistoryItem> curHistoryItems = GetHistoryItems(curProp, 1);//存在歷史記錄時 插入對比后的記錄
                                itemsSql = string.Empty;
                                //curHistoryItems.ForEach(m => m.col_chsname =columns.FindAll(o => o.Column_Name == m.col_name).FirstOrDefault().ColumnComment);// Query_COL_Comment(history.table_name, m.col_name, mycon.Database));
                                //curHistoryItems.ForEach(m => itemsSql += tableHistoryItemMgr.Save(m));
                                foreach (var item in curHistoryItems)
                                {
                                    Column col = columns.FirstOrDefault(o => o.Column_Name == item.col_name);
                                    //item.col_chsname = col == null ? Resource.CoreMessage.GetResource("NO_DESCRIPTION") : col.ColumnComment;
                                    item.col_chsname = col == null ? "No Description" : col.Column_Comment;
                                    itemsSql += tableHistoryItemMgr.Save(item);//插入對比后的記錄
                                }
                                historyItems.Add(itemsSql);//保存historyitem語句添加至隊列
                            }
                        }
                    }
                    else
                    {
                        histories.Add(Save(history));//將保存tablehistory語句添加至隊列

                        List<TableHistoryItem> curHistoryItems = GetHistoryItems(curProp, 1);//新數據直接寫入
                        itemsSql = string.Empty;
                        //curHistoryItems.ForEach(m => m.col_chsname = columns.FindAll(o => o.Column_Name == m.col_name).FirstOrDefault().ColumnComment);//Query_COL_Comment(history.table_name, m.col_name, mycon.Database));
                        //curHistoryItems.ForEach(m => itemsSql += tableHistoryItemMgr.Save(m));
                        foreach (var item in curHistoryItems)
                        {
                            Column col = columns.FirstOrDefault(o => o.Column_Name == item.col_name);
                            //item.col_chsname = col == null ? Resource.CoreMessage.GetResource("NO_DESCRIPTION") : col.ColumnComment;
                            item.col_chsname = col == null ? "No Description" : col.Column_Comment;
                            itemsSql += tableHistoryItemMgr.Save(item);//插入對比后的記錄
                        }
                        historyItems.Add(itemsSql);//保存historyitem語句添加至隊列
                    }

                }
                if (histories.Count > 0 && !historyBatchMgr.BatchExists(batch)) //該批次是否已存在，不存在則新增
                {
                    otherSql.Add(historyBatchMgr.Save(batch));
                }
                if (excuteSqls != null)
                {
                    otherSql.AddRange(excuteSqls);//其他語句加至隊列
                }

                bool flag = _tableHistoryDao.SaveHistory(histories, historyItems, otherSql);
                if(flag)
                {
                    //設置sale_status
                    SetSaleStatus(batch, tableInfo.DBName, oldProp);
                }
                return flag;
            }
            catch (Exception ex)
            {
                throw new Exception("TableHistoryMgr.SaveHistory(T current, HistoryBatch batch, ArrayList excuteSqls)-->" + ex.Message, ex);
            }
        }

        public bool SaveHistory<T>(List<T> current, HistoryBatch batch, ArrayList excuteSqls)
        {
            try
            {
                ArrayList histories = new ArrayList();
                ArrayList historyItems = new ArrayList();
                ArrayList otherSql = new ArrayList();

                if (current != null && current.Count > 0)
                {
                    Type first = current.FirstOrDefault().GetType();
                    DBTableInfo tableInfo = (DBTableInfo)Attribute.GetCustomAttribute(first, typeof(DBTableInfo));
                    if (tableInfo == null)
                    {
                        return false;
                    }
                    TableHistory history = new TableHistory { table_name = tableInfo.DBName, pk_name = tableInfo.PK_Col, batchno = batch.batchno, functionid = batch.functionid };
                    Dictionary<string, string> properties = new Dictionary<string, string>();
                    //取得表內需記錄的字段
                    IParametersrcImplMgr parametersrcMgr = new ParameterMgr(connStr);
                    List<string> cols = (from p in parametersrcMgr.QueryUsed(new Parametersrc { ParameterType = "ColumnHistory", ParameterCode = history.table_name, Used = 1 })
                                         select p.parameterName.Trim().ToLower()).ToList();
                    if (cols != null && cols.Count != 0)
                    {
                        foreach (var o in (IEnumerable<object>)current)
                        {
                            Type curInfo = o.GetType();
                            if (string.IsNullOrEmpty(history.pk_value))
                            {
                                System.Reflection.PropertyInfo pkProperty = curInfo.GetProperties().Where(m => m.Name.ToLower() == history.pk_name).FirstOrDefault();
                                if (pkProperty != null)
                                {
                                    history.pk_value = pkProperty.GetValue(o, null).ToString();
                                }
                            }
                            foreach (System.Reflection.PropertyInfo item in curInfo.GetProperties().Where(m => cols.Contains(m.Name.ToLower())))
                            {
                                string col = item.Name.ToLower();
                                if (properties.Keys.Contains(col))
                                {
                                    properties[col] += "," + item.GetValue(o, null).ToString();
                                }
                                else
                                {
                                    properties.Add(col, item.GetValue(o, null).ToString());
                                }
                            }
                        }
                        List<Model.TableHistoryItem> newHistoryItems = GetHistoryItems(properties, 1);
                        int tableHistoryId = QueryExists(history);//是否存在歷史記錄
                        string itemsSql = string.Empty;
                        if (tableHistoryId != -1)
                        {
                            List<Model.TableHistoryItem> oldHistory = tableHistoryItemMgr.Query(new TableHistoryItem { tablehistoryid = tableHistoryId });
                            foreach (var item in oldHistory)
                            {
                                if (newHistoryItems.Exists(m => m.col_name == item.col_name && m.col_value != item.col_value))
                                {
                                    otherSql.Add(tableHistoryItemMgr.UpdateType(item));//舊記錄修改狀態
                                    //舊的col_value 添加至新記錄的old_value
                                    newHistoryItems.FindAll(m => m.col_name == item.col_name && m.col_value != item.col_value).ForEach(m => m.old_value = item.col_value);
                                }
                                newHistoryItems.RemoveAll(m => m.col_name == item.col_name && m.col_value == item.col_value);
                            }
                            if (newHistoryItems.Count > 0)
                            {
                                histories.Add(Save(history));

                                //newHistoryItems.ForEach(m => m.col_chsname = Query_COL_Comment(history.table_name, m.col_name));
                                newHistoryItems.ForEach(m => itemsSql += tableHistoryItemMgr.Save(m));
                                historyItems.Add(itemsSql);
                            }
                        }
                        else
                        {
                            histories.Add(Save(history));

                            //newHistoryItems.ForEach(m => m.col_chsname = Query_COL_Comment(history.table_name, m.col_name));
                            newHistoryItems.ForEach(m => itemsSql += tableHistoryItemMgr.Save(m));
                            historyItems.Add(itemsSql);
                        }
                    }
                }
                if (histories.Count > 0 && !historyBatchMgr.BatchExists(batch)) //該批次是否已存在，不存在則新增
                {
                    otherSql.Add(historyBatchMgr.Save(batch));
                }
                if (excuteSqls != null)
                {
                    otherSql.AddRange(excuteSqls);//其他語句加至隊列
                }
                return _tableHistoryDao.SaveHistory(histories, historyItems, otherSql);
            }
            catch (Exception ex)
            {
                throw new Exception("TableHistoryMgr.SaveHistory(List<T> current, HistoryBatch batch, ArrayList excuteSqls)-->" + ex.Message, ex);
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_dt"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        public Dictionary<string, string> OldDataInfo(System.Data.DataTable _dt, List<string> cols)
        {
            try
            {
                Dictionary<string, string> oldProperties = new Dictionary<string, string>();
                foreach (string str in cols)
                {
                    oldProperties.Add(str, Common.CommonFunction.StringTransfer(_dt.Rows[0][str].ToString()));
                }
                return oldProperties;
            }
            catch (Exception ex)
            {

                throw new Exception("TableHistoryMgr.OldDataInfo-->" + ex.Message, ex);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetPropertiesInfo<T>(T model)
        {
            try
            {
                Dictionary<string, string> properties = new Dictionary<string, string>();
                Type entityType = model.GetType();
                foreach (System.Reflection.PropertyInfo item in entityType.GetProperties())
                {
                    properties.Add(item.Name.ToLower(), item.GetValue(model, null).ToString());
                }
                return properties;
            }
            catch (Exception ex)
            {
                throw new Exception("TableHistoryMgr.GetPropertiesInfo-->" + ex.Message, ex);
            }

        }

        /// <summary>
        /// 對比生成修改的資訊集合
        /// </summary>
        /// <param name="oldProp">舊屬性集合</param>
        /// <param name="newProp">新屬性集合</param>
        /// <returns></returns>
        private List<TableHistoryItem> CompareUpdate(Dictionary<string, string> oldProp, Dictionary<string, string> newProp)
        {
            try
            {
                List<TableHistoryItem> historyItems = new List<TableHistoryItem>();
                foreach (var item in newProp)
                {
                    string oldValue = oldProp.Where(m => m.Key == item.Key).FirstOrDefault().Value;
                    string newValue = item.Value;
                    if (oldValue.ToLower().Trim() != newValue.ToLower().Trim())
                    {
                        historyItems.Add(new Model.TableHistoryItem { col_name = item.Key, col_value = newValue, old_value = oldValue, type = 1 });
                    }
                }
                return historyItems;
            }
            catch (Exception ex)
            {

                throw new Exception("TableHistoryMgr.CompareUpdate-->" + ex.Message, ex);
            }

        }

        /// <summary>
        /// 生成記錄集合
        /// </summary>
        /// <param name="properties">屬性集合</param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        private List<Model.TableHistoryItem> GetHistoryItems(Dictionary<string, string> properties, int typeId)
        {
            try
            {
                List<TableHistoryItem> historyItems = new List<TableHistoryItem>();
                foreach (var item in properties)
                {
                    historyItems.Add(new Model.TableHistoryItem { col_name = item.Key, col_value = item.Value, type = typeId });
                }
                return historyItems;
            }
            catch (Exception ex)
            {
                throw new Exception("TableHistoryMgr.GetHistoryItems-->" + ex.Message, ex);
            }

        }

        //add by wwei0216w 2015/1/22
        /// <summary>
        /// 根據條件獲得歷史記錄信息
        /// </summary>
        /// <param name="condition">條件</param>
        /// <returns>符合條件的ListTableHistory集合</returns>
        public List<TableHistoryCustom> GetHistoryByCondition(TableHistory condition,out int total)
        {
            try
            {
                List<TableHistoryCustom> list = _tableHistoryDao.QueryBatchno(condition, out total);//獲得批次號
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("TableHistoryMgr.GetHistoryByCondition-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查詢t_table_history中出現的表名稱
        /// </summary>
        /// <returns>包含表名稱的TableHistory集合</returns>
        //add by wwei0216w 2015/1/22
        public List<TableHistory> QueryTableName()
        {
            try
            {
                return _tableHistoryDao.QueryTableName();
            }
            catch (Exception ex)
            {
                throw new Exception("TableHistoryMgr.GetHistoryByCondition-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 保存product表中販售狀態歷史記錄
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="current">Product對象</param>
        /// <param name="batch">批次</param>
        /// <param name="excuteSqls">存儲要執行的sql語句集合</param>
        /// <param name="cols">記錄的列名</param>
        /// <param name="columns"></param>
        /// <returns></returns>
        private bool SaveSaleStatusHistory<T>(Product current, HistoryBatch batch, ArrayList excuteSqls)
        {
            try 
            {
                if (current == null || batch == null)
                {
                    return false;
                }

                TableHistory history = new TableHistory { table_name = "product", batchno = batch.batchno };
                history.pk_name = "product_id";
                history.functionid = batch.functionid;
     
                    List<Column> columns = Query_COL_Comment(history.table_name, mycon.Database);
  

                ArrayList histories = new ArrayList();
                ArrayList historyItems = new ArrayList();
                ArrayList otherSql = new ArrayList();
                //取得表內需記錄的字段
                Dictionary<string, string> curProp = GetPropertiesInfo<Product>(current);

                List<string> cols = new List<string>();
                cols.Add("sale_status"); //整個方法值記錄sale_status的狀態


                if (cols != null && cols.Count != 0)
                {
                    string selCols = cols[0];

                    history.pk_value = current.Product_Id.ToString();
                    if (string.IsNullOrEmpty(history.pk_name))
                    {
                        return false;
                    }
                    curProp = curProp.Where(m => cols.Exists(n => n.ToString() == m.Key)).ToDictionary(m => m.Key, m => m.Value);//從新數據中取出需要記錄的欄位
                    string itemsSql = string.Empty;
                    int tableHistoryId = -1;
                    List<TableHistoryItem> newItems = null;
                    //目前未執行修改，可以查詢出修改前數據
                    System.Data.DataTable _dt = _tableHistoryDao.ExcuteSql(string.Format("select sale_status from product where product_id ={0}", history.pk_value));
                    tableHistoryId = QueryExists(history);//是否存在歷史記錄
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        Dictionary<string, string> oldProperites = OldDataInfo(_dt, cols);//修改前屬性集合,只包括需要記錄的欄位

                        newItems = CompareUpdate(oldProperites, curProp);//對比取得修改記錄
                        if (newItems != null && newItems.Count != 0)
                        {
                            if (tableHistoryId != -1)
                            {
                                //存在歷史記錄則 修改原記錄狀態為0
                                newItems.ForEach(m => m.tablehistoryid = tableHistoryId);
                                otherSql.Add(tableHistoryItemMgr.UpdateType(newItems, tableHistoryId));
                            }
                            else
                            {
                                //不存在則先添加舊記錄(狀態為0)
                                List<TableHistoryItem> oldHistoryItems = GetHistoryItems(oldProperites, 0);
                                oldHistoryItems.FindAll(m => !newItems.Exists(n => n.col_name == m.col_name)).ForEach(m => m.type = 1);

                                histories.Add(Save(history));//將保存tablehistory語句添加至隊列

                                foreach (var item in oldHistoryItems)
                                {
                                    Column col = columns.FirstOrDefault(o => o.Column_Name == item.col_name);
                                    item.col_chsname = col == null ? "No Description" : col.Column_Comment;
                                    itemsSql += tableHistoryItemMgr.Save(item);//插入對比后的記錄
                                }
                                historyItems.Add(itemsSql);//保存historyitem語句添加至隊列
                            }
                            histories.Add(Save(history));//將保存tablehistory語句添加至隊列


                            itemsSql = string.Empty;
                            foreach (var item in newItems)
                            {
                                Column col = columns.FirstOrDefault(o => o.Column_Name == item.col_name);
                                item.col_chsname = col == null ? "No Description" : col.Column_Comment;
                                itemsSql += tableHistoryItemMgr.Save(item);//插入對比后的記錄
                            }
                            historyItems.Add(itemsSql);//保存historyitem語句添加至隊列
                        }
                        else
                        {
                            if (tableHistoryId == -1)
                            {
                                histories.Add(Save(history));//將保存tablehistory語句添加至隊列

                                List<TableHistoryItem> curHistoryItems = GetHistoryItems(curProp, 1);//存在歷史記錄時 插入對比后的記錄
                                itemsSql = string.Empty;
                                foreach (var item in curHistoryItems)
                                {
                                    Column col = columns.FirstOrDefault(o => o.Column_Name == item.col_name);
                                    item.col_chsname = col == null ? "No Description" : col.Column_Comment;
                                    itemsSql += tableHistoryItemMgr.Save(item);//插入對比后的記錄
                                }
                                historyItems.Add(itemsSql);//保存historyitem語句添加至隊列
                            }
                        }
                    }
                    else
                    {
                        histories.Add(Save(history));//將保存tablehistory語句添加至隊列

                        List<TableHistoryItem> curHistoryItems = GetHistoryItems(curProp, 1);//新數據直接寫入
                        itemsSql = string.Empty;
                        foreach (var item in curHistoryItems)
                        {
                            Column col = columns.FirstOrDefault(o => o.Column_Name == item.col_name);
                            item.col_chsname = col == null ? "No Description" : col.Column_Comment;
                            itemsSql += tableHistoryItemMgr.Save(item);//插入對比后的記錄
                        }
                        historyItems.Add(itemsSql);//保存historyitem語句添加至隊列
                    }

                }
                if (histories.Count > 0 && !historyBatchMgr.BatchExists(batch)) //該批次是否已存在，不存在則新增
                {
                    otherSql.Add(historyBatchMgr.Save(batch));
                }
                if (excuteSqls != null)
                {
                    otherSql.AddRange(excuteSqls);//其他語句加至隊列
                }
                return _tableHistoryDao.SaveHistory(histories, historyItems, otherSql);
            }
            catch (Exception ex)
            {
                throw new Exception("TableHistoryMgr.SaveSaleStatusHistory(T current, HistoryBatch batch, ArrayList excuteSqls)-->" + ex.Message, ex);
            }
        }


        /// <summary>
        /// 根據傳遞的表名稱獲得productId
        /// </summary>
        /// <param name="dictionary">需要搜索的Dictionary</param>
        /// <param name="tableName">表名</param>
        /// <returns>productId</returns>
        private uint GetProductId(Dictionary<string, string> dictionary, string tableName)
        {
            uint productId = 0;
            switch (tableName)
            {
                case "product":
                    productId = Convert.ToUInt32(dictionary.Where(m => m.Key == "product_id").FirstOrDefault().Value);
                    break;
                case "price_master":
                    productId = Convert.ToUInt32(dictionary.Where(m => m.Key == "product_id").FirstOrDefault().Value);
                    break;
                //case "item_price":
                //    IItemPriceImplMgr _ipMgr =new ItemPriceMgr(connStr);
                //    int item_price_id = Convert.ToInt32((dictionary.Where(m => m.Key == "item_price_id").FirstOrDefault().Value));
                //    productId =Convert.ToUInt32(_ipMgr.GetProductId(item_price_id));
                //    break;
                default:
                    return 0;
            }
            return productId;
        }

        /// <summary>
        /// 保存販售狀態的歷史記錄
        /// </summary>
        /// <param name="batch">批次</param>
        /// <param name="tableName">表名</param>
        /// <param name="dictionary">用於查找主鍵值的dictionary集合</param>
        /// <returns>true or false</returns>
        public bool SetSaleStatus(HistoryBatch batch, string tableName, Dictionary<string, string> dictionary)
        {
            IProductImplMgr _productMgr = new ProductMgr(connStr);
            ArrayList listSql = new ArrayList();
            uint productId = GetProductId(dictionary,tableName);
            Product prod = _productMgr.UpdateSaleStatus(productId);
            if (prod == null) return true;
            listSql.Add(_productMgr.UpdateColumn(prod, "sale_status"));
            return SaveSaleStatusHistory<Product>(prod, batch, listSql);
        }
    }
}
