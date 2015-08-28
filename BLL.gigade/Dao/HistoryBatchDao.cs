/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：HistoryBatchDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2014/2/6 9:40:05 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
    public class HistoryBatchDao : IHistoryBatchImplDao
    {
        private IDBAccess _dbAccess;
        private string strcon = "";
        public HistoryBatchDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.strcon = connectionStr;
        }

        #region IHistoryBatchImplDao 成员

        public string Save(Model.HistoryBatch historyBatch)
        {
            ///edit by wwei0216w 2015/8/14 將原先的保存user_email改為保存 user_id
            ///以下為根據user_email查詢相應的user_id
            uint user_id = 0;
            IManageUserImplDao _manageDao = new ManageUserDao(strcon);
            ManageUser mu = _manageDao.GetUserIdByEmail(historyBatch.kuser).FirstOrDefault();
            if(mu!=null)
            {
                user_id = mu.user_id;
            }
            StringBuilder strSql = new StringBuilder("insert into t_history_batch(`batchno`,`kuser`,`kdate`)values");
            strSql.AppendFormat("('{0}',{1},now())", historyBatch.batchno, user_id);
            return strSql.ToString();
        }

        public bool BatchExists(Model.HistoryBatch historyBatch)
        {
            try
            {
                StringBuilder strSql = new StringBuilder("select count(batchno) as count from t_history_batch where");
                strSql.AppendFormat(" batchno='{0}'", historyBatch.batchno);
                System.Data.DataTable _dt = _dbAccess.getDataTable(strSql.ToString());
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(_dt.Rows[0]["count"]) > 0;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("HistoryBatchDao.BatchExists-->" + ex.Message, ex);
            }
        }

        public List<Model.HistoryBatch> QueryToday(int itemType)
        {
            try
            {
                string strSql = "";
                if (itemType != 1)
                {
                    //edit by hufeng0813w 014/06/13 reason 需要根據product_id排序
                    strSql = "select batchno,kuser,kdate,substring(substring(batchno,instr(batchno,'_')+1,length(batchno)),instr(substring(batchno,instr(batchno,'_')+1,length(batchno)),'_')+1,length(substring(batchno,instr(batchno,'_')+1,length(batchno)))) as product_rowid from t_history_batch where to_days(kdate)=to_days(now())";
                }
                else
                {
                    //單一商品對照信息
                    strSql = "select distinct thb.batchno,thb.kuser,thb.kdate,substring(substring(batchno,instr(batchno,'_')+1,length(batchno)),instr(substring(batchno,instr(batchno,'_')+1,length(batchno)),'_')+1,length(substring(batchno,instr(batchno,'_')+1,length(batchno)))) as product_rowid,c.channel_name_full,pim.channel_detail_id,pim.product_name from t_history_batch thb";
                    strSql += "	left join product p on p.product_id=substring(substring(batchno,instr(batchno,'_')+1,length(batchno)),instr(substring(batchno,instr(batchno,'_')+1,length(batchno)),'_')+1,length(substring(batchno,instr(batchno,'_')+1,length(batchno))))";
                    strSql += "	left join product_item pi on pi.product_id=substring(substring(batchno,instr(batchno,'_')+1,length(batchno)),instr(substring(batchno,instr(batchno,'_')+1,length(batchno)),'_')+1,length(substring(batchno,instr(batchno,'_')+1,length(batchno))))";
                    strSql += "	left join product_item_map pim on (pim.item_id=pi.item_id or instr(pim.group_item_id,pi.item_id))";
                    strSql += "	left join channel c on c.channel_id=pim.channel_id";
                    strSql += "  where to_days(kdate)=to_days(now()) and (instr(pim.item_id,pi.item_id) or instr(pim.group_item_id,pi.item_id))";
                    strSql += " union all ";
                    //組合商品的對照信息
                    strSql += " select distinct thb.batchno,thb.kuser,thb.kdate,substring(substring(batchno,instr(batchno,'_')+1,length(batchno)),instr(substring(batchno,instr(batchno,'_')+1,length(batchno)),'_')+1,length(substring(batchno,instr(batchno,'_')+1,length(batchno)))) as product_rowid,c.channel_name_full,pim.channel_detail_id,pim.product_name from t_history_batch thb";
                    strSql += " left join product_combo pc on pc.parent_id=substring(substring(batchno,instr(batchno,'_')+1,length(batchno)),instr(substring(batchno,instr(batchno,'_')+1,length(batchno)),'_')+1,length(substring(batchno,instr(batchno,'_')+1,length(batchno))))";
                    strSql += " left join product_item pi on pi.product_id in(pc.child_id)";
                    strSql += " left join product_item_map pim on pim.product_id= substring(substring(batchno,instr(batchno,'_')+1,length(batchno)),instr(substring(batchno,instr(batchno,'_')+1,length(batchno)),'_')+1,length(substring(batchno,instr(batchno,'_')+1,length(batchno))))";
                    strSql += " left join channel c on c.channel_id=pim.channel_id";
                    strSql += " where to_days(kdate)=to_days(now()) and instr(pim.group_item_id,pi.item_id); ";
                }

                return _dbAccess.getDataTableForObj<Model.HistoryBatch>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("HistoryBatchDao.QueryToday-->" + ex.Message, ex);
            }
        }

        public Model.HistoryBatch Query(Model.HistoryBatch historyBatch)
        {
            try
            {
                StringBuilder strSql = new StringBuilder("select batchno,kuser,kdate from t_history_batch where ");
                strSql.AppendFormat(" batchno='{0}'", historyBatch.batchno);
                return _dbAccess.getSinggleObj<Model.HistoryBatch>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("HistoryBatchDao.Query-->" + ex.Message, ex);
            }
        }

        #endregion
    }
}
