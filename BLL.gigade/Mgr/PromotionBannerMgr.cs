using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BLL.gigade.Mgr
{
    public class PromotionBannerMgr
    {
        private PromotionBannerDao _promotionBannerDao;       
        private MySqlDao _mysqlDao;
        private VendorBrandDao _vendorBrandDao;
        private IDBAccess _accessMySql;
        public PromotionBannerMgr(string connectionStr)
        {            
            _promotionBannerDao = new PromotionBannerDao(connectionStr);           
            _mysqlDao = new MySqlDao(connectionStr);
            _vendorBrandDao = new VendorBrandDao(connectionStr);
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }
        public List<PromotionBannerQuery> GetPromotionBannerList(PromotionBannerQuery query, out int totalCount)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendFormat(@" ");
                if (query.brand_name != string.Empty)
                {
                    string sql_getID = _vendorBrandDao.GetBrand_idByBrand_name(query.brand_name);
                    DataTable dt = _accessMySql.getDataTable(sql_getID);
                    if (dt.Rows.Count > 0 && dt != null)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string id = dt.Rows[i][0].ToString();
                            if (i == 0)
                            {
                                strSql.AppendFormat(" AND ((brand_id LIKE N'{0},%' or brand_id LIKE N'%,{0},%'or brand_id LIKE N'%,{0}'  or brand_id ='{0}')", id);
                            }
                            else
                            {
                                strSql.AppendFormat(" OR (brand_id LIKE N'{0},%' or brand_id LIKE N'%,{0},%'or brand_id LIKE N'%,{0}'  or brand_id ='{0}')", id);
                            }
                        }
                        strSql.AppendFormat(")");
                    }
                    else {
                        strSql.AppendFormat(" AND ((brand_id LIKE N'{0},%' or brand_id LIKE N'%,{0},%'or brand_id LIKE N'%,{0}'  or brand_id ='{0}')", -1);
                    }
                }
                return _promotionBannerDao.GetPromotionBannerList(query, strSql.ToString(), out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerMgr-->GetPromotionBannerList-->" + ex.Message, ex);
            }
        }

        #region 更改促銷圖片信息
        public int UpdateImageInfo(PromotionBannerQuery query, out int brand_id)
        {
            brand_id = 0;
            try
            {
                if (query.multi == 0)//不允許一個品牌多個促銷圖的話要檢查，否則不檢查
                {
                    string[] bids = query.brand_id.Split(',');
                    query.date_start = query.pb_startdate;
                    query.date_end = query.pb_enddate;
                    AllowShowOrNot(query, bids, out brand_id);
                    if (brand_id != 0)
                    {
                        return -1;//品牌编号在該促銷圖片顯示期間已有其他促銷圖片 且已啟用
                    }
                }
                return _promotionBannerDao.UpdateImageInfo(query);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerMgr-->UpdateImageInfo-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 新增促銷圖片
        public int AddImageInfo(PromotionBannerQuery query,out int brand_id)
        {
            brand_id = 0;
            try
            {
                if (query.multi == 0)//不允許一個品牌多個促銷圖的話要檢查，否則不檢查
                {
                    string[] bids = query.brand_id.Split(',');
                    query.date_start = query.pb_startdate;
                    query.date_end = query.pb_enddate;
                    AllowShowOrNot(query, bids, out brand_id);
                    if (brand_id != 0)
                    {
                        return -1;//品牌编号在該促銷圖片顯示期間已有其他促銷圖片 且已啟用
                    }
                }
                return _promotionBannerDao.AddImageInfo(query);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerMgr-->UpdateStatus-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 更改促銷圖片狀態
        public int UpdateStatus(PromotionBannerQuery query, out int brand_id)
        {
            try
            {
                brand_id = 0;
                bool canModify = IsModifiable(query);
                if (canModify)
                {
                    if (query.multi == 0)//不允許一個品牌多個促銷圖的話要檢查，否則不檢查
                    {
                        if (query.pb_status == 1)
                        {
                            PromotionBannerQuery model = _promotionBannerDao.GetModelById(query.pb_id);
                            DataTable dt = new DataTable();
                            query.date_start = model.pb_startdate;
                            query.date_end = model.pb_enddate;
                            if (model != null && model.brand_id != string.Empty)
                            {
                                string[] bids = model.brand_id.Split(',');
                                AllowShowOrNot(query, bids, out brand_id);
                                if (brand_id != 0)
                                {
                                    return -2;
                                }
                            }
                        }
                    }
                    return _promotionBannerDao.UpdateStatus(query);
                }
                else
                {
                    return -1;//圖片已過期
                }

            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerMgr-->UpdateStatus-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 促銷圖片信息是否已過期
        public bool IsModifiable(PromotionBannerQuery query)
        {
            try
            {
                DateTime endTime = _promotionBannerDao.GetEndTime(query);
                if (DateTime.Now < endTime)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                throw new Exception("PromotionBannerMgr-->CanModify-->" + ex.Message, ex);
            }
        } 
        #endregion

        #region 刪除促銷圖片
        public int DeleteImage(PromotionBannerQuery query)
        {
            try
            {
                return _accessMySql.execCommand(_promotionBannerDao.DeleteImage(query));               
            }
            catch (Exception ex)
            {

                throw new Exception("PromotionBannerMgr-->DeleteImage-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 獲取促銷圖片下的品牌信息
        public List<VendorBrand> GetBrandList(PromotionBannerQuery query)
        {
            try
            {
                PromotionBannerQuery model = _promotionBannerDao.GetModelById(query.pb_id);
                if (model != null && model.brand_id != string.Empty)
                {
                    int id = 0;
                    List<VendorBrand> vb_store = _vendorBrandDao.GetBrandListByIds(model.brand_id, id);
                    return vb_store;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerMgr-->GetBrandList-->" + ex.Message, ex);
            }

        }
        #endregion

        #region 獲取單個對象
        public PromotionBannerQuery GetModelById(int id)
        {
            try
            {
                return _promotionBannerDao.GetModelById(id);
            }
            catch (Exception ex)
            {

                throw new Exception("PromotionBannerMgr-->GetModelById-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 檢查品牌編號是否已有促銷圖片且已啟用，有的話返回品牌編號
        public void AllowShowOrNot(PromotionBannerQuery query, string[] bids, out int brand_id)
        {
            brand_id = 0;
            try
            {
                if (query.pb_enddate != DateTime.MinValue && query.pb_enddate < DateTime.Now)
                {
                    return;
                }
                DataTable dt = new DataTable();
                for (int i = 0; i < bids.Length; i++)
                {
                    if (bids[i] != string.Empty)
                    {
                        query.singleBrand_id = Convert.ToInt32(bids[i]);
                        dt = _promotionBannerDao.GetUsingImage(query);
                        if (query.changeMode == 0)
                        {
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                brand_id = query.singleBrand_id;
                                return;//品牌编号在該促銷圖片顯示期間已有其他促銷圖片 且已啟用
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else {
                            if (dt != null && dt.Rows.Count >1)
                            {
                                brand_id = query.singleBrand_id;
                                return;//品牌编号在該促銷圖片顯示期間已有其他促銷圖片 且已啟用
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerMgr-->AllowShowOrNot-->" + ex.Message, ex);
            }
        } 
        #endregion

        #region 查詢品牌名稱
        public string GetBrandName(PromotionBannerQuery query)
        {
            try
            {
                VendorBrandQuery vb_query = new VendorBrandQuery();
                vb_query.Brand_Id = (uint)query.singleBrand_id;
                List<VendorBrand> exits = _vendorBrandDao.GetVendorBrand(vb_query);//查詢是否有這個品牌編號
                int brand_id = 0;
                string brand_name = string.Empty;
                if (exits.Count != 0)
                {
                    if (query.multi == 0)
                    {
                        string[] bids = query.singleBrand_id.ToString().Split(',');
                        AllowShowOrNot(query, bids, out brand_id);
                        if (brand_id != 0)
                        {
                            return "-2";//品牌编号在該促銷圖片顯示期間已有其他促銷圖片 且已啟用
                        }
                    }
                    List<VendorBrand> vb_store = _vendorBrandDao.GetBrandListByIds(string.Empty, query.singleBrand_id);
                    if (vb_store != null)
                    {
                        brand_name = vb_store[0].Brand_Name;
                    }
                    return brand_name;
                }
                else
                {
                    return "-1";
                }

            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerRelationMgr-->GetBrandName-->" + ex.Message, ex);
            }
        } 
        #endregion

        #region 是否允許一個品牌有多張促銷圖片
        public int AllowMultiOrNot(PromotionBannerQuery query,out int id)
        {
            int result = 0;
            id = 0;
            try
            {
                List<PromotionBannerQuery> store = _promotionBannerDao.ShowUsingImage();
                StringBuilder bids = new StringBuilder();
                DataTable dt = new DataTable();
                foreach (var item in store)
                {
                    bids.AppendFormat("{0},", item.brand_id);
                }
                string s = bids.ToString().Substring(0, bids.ToString().LastIndexOf(','));
                string[] brand_ids = s.Split(',');
                for (int i = 0; i < brand_ids.Length; i++)
                {
                    dt = _promotionBannerDao.SearchImages(brand_ids[i]);
                    if (dt.Rows.Count > 1)
                    {
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            if (!string.IsNullOrEmpty(dt.Rows[j]["pb_startdate"].ToString()))
                            {
                                query.date_start = Convert.ToDateTime(dt.Rows[j]["pb_startdate"]);
                            }
                            if (!string.IsNullOrEmpty(dt.Rows[j]["pb_enddate"].ToString()))
                            {
                                query.date_end = Convert.ToDateTime(dt.Rows[j]["pb_enddate"]);
                            }
                            string[] single_bid = brand_ids[i].ToString().Split(',');
                            AllowShowOrNot(query, single_bid, out id);
                            if (id != 0)
                            {
                                return result = 1; //品牌编号在該促銷圖片顯示期間已有其他促銷圖片 且已啟用
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerRelationMgr-->AllowMultiOrNot-->" + ex.Message, ex);
            }
        } 
        #endregion

    }
}
