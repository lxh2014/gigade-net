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
    public class WebContentType5Mgr : IWebContentType5ImplMgr
    {
        private IWebContentType5ImplDao _wctdao;
        public WebContentType5Mgr(string connectionString)
        {
            _wctdao = new WebContentType5Dao(connectionString);
        }

        public int Insert(Model.WebContentType5 m)
        {
            try
            {
                return _wctdao.Insert(m);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType5Mgr.Insert-->" + ex.Message, ex);
            }
        }

        public int Update(Model.WebContentType5 m)
        {
            try
            {
                return _wctdao.Update(m);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType5Mgr.Update-->" + ex.Message, ex);
            }
        }

        public List<Model.Query.WebContentType5Query> QueryAll(Model.Query.WebContentType5Query query, out int totalCount)
        {
            try
            {
                return _wctdao.QueryAll(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType5Mgr.QueryAll-->" + ex.Message, ex);
            }
        }

        public Model.WebContentType5 GetModel(Model.WebContentType5 model)
        {
            try
            {
                return _wctdao.GetModel(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType5Mgr.GetModel-->" + ex.Message, ex);
            }
        }

        public int delete(Model.WebContentType5 model)
        {
            try
            {
                return _wctdao.delete(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType5Mgr.delete-->" + ex.Message, ex);
            }
        }

        public int GetDefault(WebContentType5 model)
        {
            try
            {
                return _wctdao.GetDefault(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType5Mgr.GetDefault-->" + ex.Message, ex);
            }
        }
    }
}
