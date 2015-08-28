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
    public class AnnounceMgr : IAnnounceImplMgr
    {
        private IAnnounceImplDao _IAnnounceDao;

        public AnnounceMgr(string connectionString)
        {
            _IAnnounceDao = new AnnounceDao(connectionString);
        }

        public string GetAnnounce()
        {
            try
            {
                List<AnnounceQuery> store = _IAnnounceDao.GetAnnounce();
                StringBuilder stb = new StringBuilder();

                stb.Append("{");
                stb.Append(string.Format("success:true,data:["));

                foreach (AnnounceQuery para in store)
                {
                    stb.Append("{");
                    stb.Append(string.Format("\"info_id\":\"{0}\",\"info_name\":\"{1}\"", para.announce_id, para.title));
                    stb.Append("}");
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                throw new Exception("AnnounceMgr-->GetAnnounce-->" + ex.Message, ex);
            }
        }
        public AnnounceQuery GetAnnounce(AnnounceQuery query)
        {
            try
            {
                return _IAnnounceDao.GetAnnounce(query);
            }
            catch (Exception ex)
            {

                throw new Exception("AnnounceMgr-->GetAnnounce-->" + ex.Message, ex);
            }
        }

        public int AnnounceSave(AnnounceQuery store)
        {
            try
            {
                return _IAnnounceDao.AnnounceSave(store);
            }
            catch (Exception ex)
            {

                throw new Exception("AnnounceMgr-->AnnounceSave-->" + ex.Message, ex);
            }

        }
        public List<AnnounceQuery> GetAnnounceList(AnnounceQuery store, out int totalCount)
        {
            try
            {
                return _IAnnounceDao.GetAnnounceList(store, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("AnnounceMgr-->GetAnnounceList-->" + ex.Message, ex);
            }
        }
    }
}
