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
    public class WebContentType7Mgr : IWebContentType7ImplMgr
    {
        private IWebContentType7ImplDao _wctdao;
        public WebContentType7Mgr(string connectionString)
        {
            _wctdao = new WebContentType7Dao(connectionString);
        }

        public int Insert(Model.WebContentType7 model)
        {
            try
            {
                return _wctdao.Insert(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType7Mgr.Insert-->" + ex.Message, ex);
            }
        }

        //public int Update2(Model.WebContentType7 model)
        //{
        //    return _wctdao.Update2(model);
        //}

        public int Update(Model.WebContentType7 model)
        {
            try
            {
                return _wctdao.Update(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType7Mgr.Update-->" + ex.Message, ex);
            }
        }

        public List<Model.Query.WebContentType7Query> QueryAll(Model.Query.WebContentType7Query query, out int totalCount)
        {
            try
            {
                return _wctdao.QueryAll(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType7Mgr.QueryAll-->" + ex.Message, ex);
            }
        }

        public Model.WebContentType7 GetModel(Model.WebContentType7 model)
        {
            try
            {
                return _wctdao.GetModel(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType7Mgr.GetModel-->" + ex.Message, ex);
            }
        }

        public int GetDefault(WebContentType7 model)
        {
            try
            {
                return _wctdao.GetDefault(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType7Mgr.GetDefault-->" + ex.Message, ex);
            }
        }

        public int delete(Model.WebContentType7 model)
        {
            try
            {
                return _wctdao.delete(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType7Mgr.delete-->" + ex.Message, ex);
            }
        }
    }
}
