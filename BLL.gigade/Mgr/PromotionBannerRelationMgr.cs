using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class PromotionBannerRelationMgr
    {

        private PromotionBannerRelationDao _promotionBannerRelationDao;
        private IDBAccess _accessMySql;
        public PromotionBannerRelationMgr(string connectionStr)
        {
            _promotionBannerRelationDao = new PromotionBannerRelationDao(connectionStr);
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }
        public List<PromotionBannerRelationQuery> GetRelationList(PromotionBannerRelationQuery query)
        {
            try
            {
                return _promotionBannerRelationDao.GetRelationList(query);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerRelationMgr-->GetRelationList-->" + ex.Message, ex);
            }
        }
        public int DeleteBrand(PromotionBannerRelationQuery query)
        {
            try
            {
                string sql = _promotionBannerRelationDao.DeleteBrand(query);
                return _accessMySql.execCommand(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerRelationMgr-->DeleteBrand-->" + ex.Message, ex);
            }
        }    
    }
}
