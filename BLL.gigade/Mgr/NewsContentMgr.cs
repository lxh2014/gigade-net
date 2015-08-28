using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Mgr
{
    public class NewsContentMgr : INewsContentImplMgr
    {
        private INewsContentImplDao _INewsContentDao;

        public NewsContentMgr(string connectionString)
        {
            _INewsContentDao = new NewsContentDao(connectionString);
        }

        public List<Model.Query.NewsContentQuery> GetNewsList(Model.Query.NewsContentQuery store, out int totalCount)
        {
            try
            {
                return _INewsContentDao.GetNewsList(store, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("NewsContentMgr-->GetNewsList" + ex.Message, ex);
            }
        }

        public string GetNewsContent()
        {
            try
            {

                List<Model.Query.NewsContentQuery> store = _INewsContentDao.GetNewContent();
                StringBuilder stb = new StringBuilder();

                stb.Append("{");
                stb.Append(string.Format("success:true,data:["));

                foreach (NewsContentQuery para in store)
                {
                    stb.Append("{");
                    stb.Append(string.Format("\"info_id\":\"{0}\",\"info_name\":\"{1}\"", para.news_id, para.news_title));
                    stb.Append("}");
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                throw new Exception("NewsContentMgr-->GetNewsContent" + ex.Message, ex);
            }
        }


        public int NewsContentSave(NewsContentQuery store)
        {
            try
            {
                return _INewsContentDao.NewsContentSave(store);
            }
            catch (Exception ex)
            {
                throw new Exception("NewsContentMgr-->NewsContentSave" + ex.Message, ex);
            }
        }


        public NewsContentQuery OldQuery(uint newsId)
        {
            try
            {
                return _INewsContentDao.OldQuery(newsId);
            }
            catch (Exception ex)
            {
                throw new Exception("NewsContentMgr-->OldQuery" + ex.Message, ex);
            }

        }


        public List<NewsLogQuery> GetNewsLogList(NewsLogQuery store, out int totalCount)
        {
            try
            {
                return _INewsContentDao.GetNewsLogList(store, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("NewsContentMgr-->GetNewsLogList" + ex.Message, ex);
            }
        }
    }
}
