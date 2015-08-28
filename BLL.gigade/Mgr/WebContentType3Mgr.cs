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
    public class WebContentType3Mgr : IWebContentType3ImplMgr
    {
        private IWebContentType3ImplDao _wctdao;
        public WebContentType3Mgr(string connectionString)
        {
            _wctdao = new WebContentType3Dao(connectionString);
        }



        public int Update(Model.WebContentType3 m)
        {
            try
            {
                return _wctdao.Update(m);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType3Mgr.Update(-->" + ex.Message, ex);
            }
        }

        public List<Model.Query.WebContentType3Query> QueryAll(Model.Query.WebContentType3Query query, out int totalCount)
        {
            try
            {
                return _wctdao.QueryAll(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType3Mgr.QueryAll-->" + ex.Message, ex);
            }
        }

        public int Add(Model.WebContentType3 model)
        {
            try
            {
                return _wctdao.Insert(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType3Mgr.Add-->" + ex.Message, ex);
            }
        }

        public Model.WebContentType3 GetModel(Model.WebContentType3 model)
        {
            try
            {
                return _wctdao.GetModel(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType3Mgr.GetModel-->" + ex.Message, ex);
            }
        }

        public int delete(Model.WebContentType3 model)
        {
            try
            {
                return _wctdao.delete(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType3Mgr.delete-->" + ex.Message, ex);
            }
        }

        public int GetDefault(WebContentType3 model)
        {
            try
            {
                return _wctdao.GetDefault(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType3Mgr.GetDefault-->" + ex.Message, ex);
            }
        }
    }
}
