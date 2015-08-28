#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：IProdVdReqImplMgr.cs
* 摘 要：
* 供應商上下架審核列表Mgr接口
* 当前版本：v1.1
* 作 者： mengjuan0826j
* 完成日期：
*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public interface IProdVdReqImplMgr
    {

        List<ProdVdReqQuery> QueryProdVdReqList(ProdVdReqQuery query, out int totalCount);
        int Update(ProdVdReq query);
        int Insert(ProdVdReq query);
    }
}
