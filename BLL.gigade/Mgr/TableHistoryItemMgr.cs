/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：TableHistoryItemMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/22 16:06:00 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System.Data;

namespace BLL.gigade.Mgr
{
    public class TableHistoryItemMgr : ITableHistoryItemImplMgr
    {
        private ITableHistoryItemImplDao _tableHistoryItemDao;
        public TableHistoryItemMgr(string connectionStr)
        {
            _tableHistoryItemDao = new TableHistoryItemDao(connectionStr);
        }

        #region ITableHistoryItemImplMgr 成员

        public List<Model.TableHistoryItem> Query(Model.TableHistoryItem tableHistoryItem)
        {
            try
            {
                return _tableHistoryItemDao.Query(tableHistoryItem);
            }
            catch (Exception ex)
            {

                throw new Exception("TableHistoryItemMgr-->Query-->" + ex.Message, ex);
            }

        }

        public string Save(Model.TableHistoryItem tableHistoryItem)
        {
            try
            {
                return _tableHistoryItemDao.Save(tableHistoryItem);
            }
            catch (Exception ex)
            {

                throw new Exception("TableHistoryItemMgr-->SingleCompareSave-->" + ex.Message, ex);
            }

        }

        public string UpdateType(Model.TableHistoryItem tableHistoryItem)
        {
            try
            {
                return _tableHistoryItemDao.UpdateType(tableHistoryItem);
            }
            catch (Exception ex)
            {

                throw new Exception("TableHistoryItemMgr-->UpdateType-->" + ex.Message, ex);
            }

        }
        public string UpdateType(List<Model.TableHistoryItem> tableHistoryItems, int tablehistoryid)
        {
            try
            {
                return _tableHistoryItemDao.UpdateType(tableHistoryItems ,tablehistoryid);
            }
            catch (Exception ex)
            {

                throw new Exception("TableHistoryItemMgr-->UpdateType-->" + ex.Message, ex);
            }

        }

        public List<Model.TableHistoryItem> Query4Batch(Model.Query.TableHistoryItemQuery query)
        {
            return _tableHistoryItemDao.Query4Batch(query);
        }

        #region 查詢歷史記錄
        //add by wwei0216w 2015/1/22
        /// <summary>
        /// 根據批次號獲得該彼此相關記錄
        /// </summary>
        /// <param name="th">條件</param>
        /// <returns>IEnumerable<IGrouping<string,TableHistoryItemCustom>></returns>
        public List<TableHistoryItemCustom> GetHistoryInfoByConditon(TableHistory th)
        {
            try
            {
                List<TableHistoryItemCustom> result = new List<TableHistoryItemCustom>();
                //根據批次獲得符合條件的歷史操作記錄集合
                DataTable dt =  _tableHistoryItemDao.GetHistoryInfoByCondition(th);
                foreach (DataRow dr in dt.Rows) {
                    if (result.FindIndex(p=>p.table_name == dr["table_name"].ToString())>=0)
                        continue;
                    TableHistoryItemCustom thic = new TableHistoryItemCustom();
                    thic.batchno = dr["batchno"].ToString();
                    thic.functionname = dr["functionName"].ToString();
                    thic.pk_name = dr["PK_name"].ToString();
                    thic.pk_value = dr["PK_value"].ToString();
                    thic.table_name = dr["table_name"].ToString();
                    foreach (DataRow dr1 in dt.Rows) {
                        if(dr1["table_name"].ToString() != dr["table_name"].ToString())
                            continue;
                        TableHistoryItem thi = new TableHistoryItem();
                        thi.col_chsname = dr1["col_chsName"].ToString();
                        thi.col_name = dr1["col_name"].ToString();
                        thi.col_value = dr1["col_value"].ToString();
                        thi.old_value = dr1["old_value"].ToString();
                        thi.rowid = Int32.Parse(dr1["rowid"].ToString());
                        thi.tablehistoryid = Int32.Parse(dr1["rowid"].ToString());
                        thi.type = Int32.Parse(dr1["type"].ToString());

                        thic.historyItem.Add(thi);
                    }
                    result.Add(thic);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("TableHistoryItemMgr-->GetHistoryInfoByConditon" + ex.Message, ex);
            }
        }

        #endregion

        #endregion


    }
}
