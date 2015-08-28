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
    public class WebContentType2Mgr : IWebContentType2ImplMgr
    {
        private IWebContentType2ImplDao _wctdao;
        public WebContentType2Mgr(string connectionString)
        {
            _wctdao = new WebContentType2Dao(connectionString);
        }



        public int Update(Model.WebContentType2 model)
        {
            try
            {
                return _wctdao.Update(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType2Mgr.Update-->" + ex.Message, ex);
            }
        }

        public List<Model.Query.WebContentType2Query> QueryAll(Model.Query.WebContentType2Query query, out int totalCount)
        {
            try
            {
                return _wctdao.QueryAll(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType2Mgr.QueryAll-->" + ex.Message, ex);
            }
        }

        public int Add(Model.WebContentType2 model)
        {
            try
            {
                return _wctdao.Insert(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType2Mgr.Insert-->" + ex.Message, ex);
            }
        }

        public Model.WebContentType2 GetModel(Model.WebContentType2 model)
        {
            try
            {
                return _wctdao.GetModel(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType2Mgr.GetModel-->" + ex.Message, ex);
            }
        }

        public int GetDefault(WebContentType2 model)
        {
            try
            {
                return _wctdao.GetDefault(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType2Mgr.GetDefault-->" + ex.Message, ex);
            }
        }

        public int delete(Model.WebContentType2 model)
        {
            try
            {
                return _wctdao.delete(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType2Mgr.delete-->" + ex.Message, ex);
            }
        }
    }
}