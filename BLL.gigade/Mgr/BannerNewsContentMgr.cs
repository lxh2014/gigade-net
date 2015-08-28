using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class BannerNewsContentMgr
    {
        private BannerNewsContentDao _bannerNewsContent;
        public BannerNewsContentMgr(string connectionString)
        {
            _bannerNewsContent = new BannerNewsContentDao(connectionString);
        }
        public DataTable GetBannerNewsContentList(BannerNewsContent bnc, out int totalCount)
        {
            try
            {
                return _bannerNewsContent.GetBannerNewsContentList(bnc, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("BannerNewsContentMgr-->GetBannerNewsContentList" + ex.Message, ex);
            }
        }
        public int SaveBannerNewsContent(BannerNewsContentQuery q)
        {

            try
            {
                return _bannerNewsContent.SaveBannerNewsContent(q);
            }
            catch (Exception ex)
            {
                throw new Exception("BannerNewsContentMgr-->SaveBannerNewsContent" + ex.Message, ex);
            }
        }
        public int UpdateBannerNewsContent(BannerNewsContentQuery q)
        {
            try
            {
                return _bannerNewsContent.UpdateBannerNewsContent(q);
            }
            catch (Exception ex)
            {
                throw new Exception("BannerNewsContentMgr-->UpdateBannerNewsContent" + ex.Message, ex);
            }
        }
    }
}
