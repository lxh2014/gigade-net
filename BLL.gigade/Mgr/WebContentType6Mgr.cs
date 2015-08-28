using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class WebContentType6Mgr : IWebContentType6ImplMgr
    {
        private IWebContentType6ImplDao _wctdao;
        public WebContentType6Mgr(string connectionString)
        {
            _wctdao = new WebContentType6Dao(connectionString);
        }
        public int Insert(Model.WebContentType6 m)
        {
            try
            {
                return _wctdao.Insert(m);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType6Mgr.Insert-->" + ex.Message, ex);
            }
        }
        public int Update2(Model.WebContentType6 m)
        {
            try
            {
                return _wctdao.Update2(m);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType6Mgr.Update2-->" + ex.Message, ex);
            }
        }
        public int Update(Model.WebContentType6 m)
        {
            try
            {
                return _wctdao.Update(m);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType6Mgr.Update-->" + ex.Message, ex);
            }
        }
        public List<Model.Query.WebContentType6Query> QueryAll(Model.Query.WebContentType6Query query, out int totalCount)
        {
            try
            {
                return _wctdao.QueryAll(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType6Mgr.QueryAll-->" + ex.Message, ex);
            }
        }
        public Model.WebContentType6 GetModel(Model.WebContentType6 model)
        {
            try
            {
                return _wctdao.GetModel(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType6Mgr.GetModel-->" + ex.Message, ex);
            }
        }
        public int GetDefault(WebContentType6 model)
        {
            try
            {
                return _wctdao.GetDefault(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType6Mgr.GetDefault-->" + ex.Message, ex);
            }
        }
        public int delete(Model.WebContentType6 model)
        {
            try
            {
                return _wctdao.delete(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType6Mgr.delete-->" + ex.Message, ex);
            }
        }
    }
}
