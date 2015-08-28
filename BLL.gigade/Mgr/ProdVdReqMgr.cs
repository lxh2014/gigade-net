#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：ProdVdReqMgr.cs
* 摘 要：
* 供應商上下架審核列表Mgr
* 当前版本：v1.1
* 作 者： mengjuan0826j
* 完成日期：
*/

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Dao.Impl;

namespace BLL.gigade.Mgr
{
    public class ProdVdReqMgr : IProdVdReqImplMgr
    {

        private IProdVdReqImplDao _prodVbReqdao;
        public ProdVdReqMgr(string connectionStr)
        {
            _prodVbReqdao = new ProdVdReqDao(connectionStr);
        }
        public List<ProdVdReqQuery> QueryProdVdReqList(ProdVdReqQuery query, out int totalCount)
        {
            try
            {
                return _prodVbReqdao.QueryProdVdReqList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("ProdVdReqMgr.QueryProdVdReqList-->" + ex.Message, ex);
            }
        }
        public int Update(ProdVdReq query)
        {
            try
            {
                return _prodVbReqdao.Update(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProdVdReqMgr.Update-->" + ex.Message, ex);
            }
        }
        public int Insert(ProdVdReq query)
        {
            try
            {
                return _prodVbReqdao.Insert(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProdVdReqMgr.Insert-->" + ex.Message, ex);
            }
        }
    }
}
