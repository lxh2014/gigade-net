using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
namespace BLL.gigade.Mgr
{
    public class EpaperContentMgr : IEpaperContentImplMgr
    {
        private IEpaperContentImplDao _IEpaperContentDao;

        public EpaperContentMgr(string connectionString)
        {
            _IEpaperContentDao = new EpaperContentDao(connectionString);
        }

        public string GetEpaperContent()
        {
            try
            {
                List<EpaperContentQuery> store = _IEpaperContentDao.GetEpaperContent();
                StringBuilder stb = new StringBuilder();

                stb.Append("{");
                stb.Append(string.Format("success:true,data:["));

                foreach (EpaperContentQuery para in store)
                {
                    stb.Append("{");
                    stb.Append(string.Format("\"info_id\":\"{0}\",\"info_name\":\"{1}\"", para.epaper_id, para.epaper_title));
                    stb.Append("}");
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {

                throw new Exception("EpaperContentMgr-->GetEpaperContent-->" + ex.Message, ex);
            }
        }
         /// <summary>
        /// 新增電子報時查詢最新四條活動頁面
        /// </summary>
        /// <returns></returns>
        public List<EpaperContent> GetEpaperContentLimit()
         {
             try
             {
                 return _IEpaperContentDao.GetEpaperContentLimit();
             }
             catch (Exception ex)
             {

                 throw new Exception("EpaperContentMgr-->GetEpaperContent-->"+ex.Message,ex);
             }
         }
        public EpaperContentQuery GetEpaperContentById(EpaperContentQuery query)
        {
            try
            {
                return _IEpaperContentDao.GetEpaperContentById(query);
            }
            catch (Exception ex)
            {
                
                throw new Exception("EpaperContentMgr-->GetEpaperContentById-->"+ex.Message,ex);
            }
        }
        public List<EpaperContentQuery> GetEpaperContentList(EpaperContentQuery query, out int totalCount)
        {
            try
            {
                return _IEpaperContentDao.GetEpaperContentList(query,out totalCount);
            }
            catch (Exception ex)
            {
                
                throw new Exception("EpaperContentMgr-->GetEpaperContentList-->"+ex.Message,ex);
            }
        }
        public List<EpaperLogQuery> GetEpaperLogList(EpaperLogQuery query, out int totalCount)
        {
            try
            {
                return _IEpaperContentDao.GetEpaperLogList(query, out totalCount);
            }
            catch (Exception ex)
            {
                
                throw new Exception("EpaperContentMgr-->GetEpaperLogList-->"+ex.Message,ex);
            }
        }


        public int SaveEpaperContent(EpaperContentQuery query)
        {
            try
            {
                return _IEpaperContentDao.SaveEpaperContent(query);
            }
            catch (Exception ex)
            {
                throw new Exception("EpaperContentMgr-->SaveEpaperContent-->" + ex.Message, ex);
            }
        }
    }
}
