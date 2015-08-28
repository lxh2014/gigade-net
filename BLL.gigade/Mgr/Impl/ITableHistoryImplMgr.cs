/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ITableHistoryImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/22 15:55:52 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Mgr.Impl
{
    public interface ITableHistoryImplMgr
    { 
        int QueryExists(Model.TableHistory tableHistory);
        string Save(Model.TableHistory tableHistory);

        string Query_TB_PK(string tb_Name);
        List<Model.Column> Query_COL_Comment(string tb_Name, string table_schema);
        List<Model.Column> Query_COL_Comment(string tb_Name);

        List<Model.TableHistory> Query(Model.TableHistory query);
        Model.TableHistory QueryLastModifyByProductId(Model.TableHistory query);

        bool SaveHistory<T>(T current, Model.HistoryBatch batch, ArrayList excuteSqls,List<string> cols = null,List<Model.Column> columns=null,string pkName=null);
        bool SaveHistory<T>(List<T> current, Model.HistoryBatch batch, ArrayList excuteSqls);

        /// <summary>
        /// 根據條件獲得歷史記錄批次號
        /// </summary>
        /// <param name="condition">條件</param>
        /// <returns>符合條件的ListTableHistory集合</returns>
        List<TableHistoryCustom> GetHistoryByCondition(TableHistory condition, out int total);
                                 
        /// <summary>
        /// 查詢t_table_history中出現的表名稱
        /// </summary>
        /// <returns>包含表名稱的TableHistory集合</returns>
        //add by wwei0216w 2015/1/22
        List<TableHistory> QueryTableName();
    }
}
