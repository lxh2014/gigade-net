
/*
* 文件名稱 :ElementDetailMgr.cs
* 文件功能描述 :廣告詳情表數據操作 
* 版權宣告 :
* 開發人員 : shuangshuang0420j
* 版本資訊 : 1.0
* 日期 : 2014/10/14
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class ElementDetailMgr : IElementDetailImplMgr
    {
        private IElementDetailImplDao _detaildao;
        public ElementDetailMgr(string connectionString)
        {
            _detaildao = new ElementDetailDao(connectionString);
        }

        public List<Model.Query.ElementDetailQuery> QueryAll(Model.Query.ElementDetailQuery query, out int totalCount)
        {
            try
            {
                return _detaildao.QueryAll(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("ElementDetailMgr.QueryAll-->" + ex.Message, ex);
            }
        }
        public int Save(Model.ElementDetail query)
        {
            try
            {
             return   _detaildao.Save(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ElementDetailMgr.Save-->" + ex.Message, ex);
            }
        }
        public int Update(Model.ElementDetail query)
        {
            try
            {
             return   _detaildao.Update(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ElementDetailMgr.Update-->" + ex.Message, ex);
            }
        }
        public Model.ElementDetail GetModel(Model.ElementDetail query)
        {
            try
            {
             return  _detaildao.GetModel(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ElementDetailMgr.GetModel-->" + ex.Message, ex);
            }
        }
        public int UpdateStatus(Model.Query.ElementDetailQuery query)
        {
            try
            {
                return _detaildao.UpdateStatus(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ElementDetailMgr.UpdateStatus-->" + ex.Message, ex);
            }
        }
        public List<ElementDetail> QueryElementDetail()
        {
            try
            {
                return _detaildao.QueryElementDetail();
            }
            catch (Exception ex)
            {
                throw new Exception("ElementDetailMgr.QueryElementDetail-->" + ex.Message, ex);
            }
        }
        public List<ElementDetailQuery> QueryAllWares(ElementDetailQuery query, out int totalCount)
        {
            try
            {
                return _detaildao.QueryAllWares(query,out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("ElementDetailMgr-->QueryAllWares-->" + ex.Message, ex);
            }
        }


        public List<ElementDetailQuery> QueryPacketProd(ElementDetail model)
        {
            try
            {
                return _detaildao.QueryPacketProd(model);
            }
            catch (Exception ex)
            {
                throw new Exception("ElementDetailMgr-->QueryPacketProd-->" + ex.Message, ex);
            };
        }
    }
}
