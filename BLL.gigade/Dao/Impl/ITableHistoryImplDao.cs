/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ITableHistoryImplDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/22 14:05:12 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Dao.Impl
{
    interface ITableHistoryImplDao
    {
        int QueryExists(Model.TableHistory tableHistory);
        string Save(Model.TableHistory tableHistory);

        string Query_TB_PK(string tb_Name);
        List<Model.Column> Query_COL_Comment(string tb_Name, string table_schema);

        List<Model.TableHistory> Query(Model.TableHistory query);
        Model.TableHistory QueryLastModifyByProductId(Model.TableHistory query);

        //add by wwei0216w 2015/1/20
        /// <summary>
        /// 根據條件獲得批次號
        /// </summary>
        /// <param name="th">條件</param>
        List<TableHistoryCustom> QueryBatchno(TableHistory th, out int total);

        /// <summary>
        /// 查詢t_table_history中出現的表名稱
        /// </summary>
        /// <returns>包含表名稱的TableHistory集合</returns>
        //add by wwei0216w 2015/1/22
        List<TableHistory> QueryTableName();
    }
}
