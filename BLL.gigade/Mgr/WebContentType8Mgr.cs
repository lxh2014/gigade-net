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
    public class WebContentType8Mgr : IWebContentType8ImplMgr
    {
        private IWebContentType8ImplDao _wctdao;
        public WebContentType8Mgr(string connectionString)
        {
            _wctdao = new WebContentType8Dao(connectionString);
        }
        public int Insert(Model.WebContentType8 m)
        {
            try
            {
                return _wctdao.Insert(m);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType8Mgr.Insert(-->" + ex.Message, ex);
            }
        }
        public int Update(Model.WebContentType8 m)
        {
            try
            {
                return _wctdao.Update(m);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType8Mgr.Update-->" + ex.Message, ex);
            }
        }
        public List<Model.Query.WebContentType8Query> QueryAll(Model.Query.WebContentType8Query query, out int totalCount)
        {
            try
            {
                return _wctdao.QueryAll(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType8Mgr.QueryAll-->" + ex.Message, ex);
            }
        }
        public Model.WebContentType8 GetModel(Model.WebContentType8 model)
        {
            try
            {
                return _wctdao.GetModel(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType8Mgr.GetModel-->" + ex.Message, ex);
            }
        }
        public int GetDefault(WebContentType8 model)
        {
            try
            {
                return _wctdao.GetDefault(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType8Mgr.GetDefault-->" + ex.Message, ex);
            }

        }
        public int delete(Model.WebContentType8 model)
        {
            try
            {
                return _wctdao.delete(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType8Mgr.GetDefault-->" + ex.Message, ex);
            }
        }
    }
}
