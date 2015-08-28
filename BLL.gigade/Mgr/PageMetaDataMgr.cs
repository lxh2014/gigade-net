using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using DBAccess;

namespace BLL.gigade.Mgr
{

    public class PageMetaDataMgr
    {
        private PageMetaDataDao _pageMetaDataDao;
        public PageMetaDataMgr(string connectionStr)
        {
            _pageMetaDataDao = new PageMetaDataDao(connectionStr);
        }

        public List<PageMetaData> GetPageMetaDataList(PageMetaData query, ref int totalCount)
        {
            try
            {
                return _pageMetaDataDao.GetPageMetaDataList(query, ref totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("PageMetaDataMgr-->GetPageMetaDataList-->" + ex.Message, ex);

            }
        }
        public int UpdatePageMeta(PageMetaData query)
        {
            try
            {
                return _pageMetaDataDao.UpdatePageMeta(query);
            }
            catch (Exception ex)
            {
                throw new Exception("PageMetaDataMgr-->UpdatePageMeta-->" + ex.Message, ex);

            }
        }
        public int InsertPageMeta(PageMetaData query)
        {
            try
            {
                return _pageMetaDataDao.InsertPageMeta(query);
            }
            catch (Exception ex)
            {
                throw new Exception("PageMetaDataMgr-->InsertPageMeta-->" + ex.Message, ex);

            }
        }
        public int DeletePageMeta(string rowIds)
        {
            try
            {
                rowIds = rowIds.TrimEnd(',');
                return _pageMetaDataDao.DeletePageMeta(rowIds);
            }
            catch (Exception ex)
            {
                throw new Exception("PageMetaDataMgr-->DeletePageMeta-->" + ex.Message, ex);

            }
        }
    }
}
