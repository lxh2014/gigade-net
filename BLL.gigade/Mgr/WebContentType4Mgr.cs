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
    public class WebContentType4Mgr : IWebContentType4ImplMgr
    {
        private IWebContentType4ImplDao _wctdao;
        public WebContentType4Mgr(string connectionString)
        {
            _wctdao = new WebContentType4Dao(connectionString);
        }

        public int Insert(Model.WebContentType4 m)
        {
            try
            {
                return _wctdao.Insert(m);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType4Mgr.Insert-->" + ex.Message, ex);
            }
        }

        public int Update(Model.WebContentType4 m)
        {
            try
            {
                return _wctdao.Update(m);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType4Mgr.Update-->" + ex.Message, ex);
            }
        }

        public List<Model.Query.WebContentType4Query> QueryAll(Model.Query.WebContentType4Query query, out int totalCount)
        {
            try
            {
                return _wctdao.QueryAll(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType4Mgr.QueryAll-->" + ex.Message, ex);
            }
        }

        public Model.WebContentType4 GetModel(Model.WebContentType4 model)
        {
            try
            {
                return _wctdao.GetModel(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType4Mgr.GetModel-->" + ex.Message, ex);
            }
        }

        public int GetDefault(WebContentType4 model)
        {
            try
            {
                return _wctdao.GetDefault(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType4Mgr.GetDefault-->" + ex.Message, ex);
            }
        }

        public int delete(Model.WebContentType4 model)
        {
            try
            {
                return _wctdao.delete(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType4Mgr.delete-->" + ex.Message, ex);
            }
        }
    }
}
