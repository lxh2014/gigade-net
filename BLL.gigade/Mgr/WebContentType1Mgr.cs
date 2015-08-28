using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using System.Data;

namespace BLL.gigade.Mgr
{
    public class WebContentType1Mgr : IWebContentType1ImplMgr
    {

        private IWebContentType1ImplDao _wctdao;

        public WebContentType1Mgr(string connectionString)
        {
            _wctdao = new WebContentType1Dao(connectionString);
        }

        public int Add(Model.WebContentType1 model)
        {
            try
            {
                return _wctdao.Insert(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType1Mgr.Insert-->" + ex.Message, ex);
            }
        }

        public int Update(Model.WebContentType1 model)
        {
            try
            {
                return _wctdao.Update(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType1Mgr.Update-->" + ex.Message, ex);
            }
        }

        public List<Model.Query.WebContentType1Query> QueryAll(Model.Query.WebContentType1Query query, out int totalCount)
        {
            try
            {
                return _wctdao.QueryAll(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType1Mgr.QueryAll-->" + ex.Message, ex);
            }
        }


        public Model.WebContentType1 GetModel(Model.WebContentType1 model)
        {
            try
            {
                return _wctdao.GetModel(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType1Mgr.GetModel-->" + ex.Message, ex);
            }
        }


        public int delete(Model.WebContentType1 model)
        {
            try
            {
                return _wctdao.delete(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType1Mgr.delete-->" + ex.Message, ex);
            }
        }
        public int GetDefault(WebContentType1 model)
        {
            try
            {
                return _wctdao.GetDefault(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType1Mgr.GetDefault-->" + ex.Message, ex);
            }

        }


    }
}
