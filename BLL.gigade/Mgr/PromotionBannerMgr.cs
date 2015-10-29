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
        private PromotionBannerRelationDao _promotionBannerRelationDao;
        private MySqlDao _mysqlDao;
        private VendorBrandDao _vendorBrandDao;
        private IDBAccess _accessMySql;
        public PromotionBannerMgr(string connectionStr)
        {
            _promotionBannerDao = new PromotionBannerDao(connectionStr);
            _promotionBannerRelationDao = new PromotionBannerRelationDao(connectionStr);
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
                    string id = string.Empty;
                    if (dt.Rows.Count > 0 && dt != null)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            id += dt.Rows[i][0].ToString() + ",";

                        }
                        id = id.Substring(0, id.LastIndexOf(','));
                        strSql.AppendFormat(" AND pbr.brand_id in({0})", id);
                    }
                    else
                    {
                        strSql.AppendFormat(" AND pbr.brand_id in({0})", -1);
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
        public bool UpdateImageInfo(PromotionBannerQuery query, out int brand_id)
        {
            brand_id = 0;
            ArrayList arr = new ArrayList();
            string[] bids = null;
            try
            {
                if (query.brandIDS != string.Empty)
                {
                    bids = query.brandIDS.Split(',');
                }
                if (query.multi == 0)//不允許一個品牌多個促銷圖的話要檢查，否則不檢查
                {
                    if (bids != null)
                    {
                        query.date_start = query.pb_startdate;
                        query.date_end = query.pb_enddate;
                        AllowShowOrNot(query, bids, out brand_id);
                        if (brand_id != 0)
                        {
                            return false;//品牌编号在該促銷圖片顯示期間已有其他促銷圖片 且已啟用
                        }
                    }

                }
                PromotionBannerRelationQuery pbr_query = new PromotionBannerRelationQuery();
                pbr_query.pb_id = query.pb_id;
                arr.Add(_promotionBannerRelationDao.DeleteBrand(pbr_query));
                if (bids != null)
                {
                    for (int i = 0; i < bids.Length; i++)
                    {
                        if (bids[i] != string.Empty)
                        {
                            pbr_query.brand_id = Convert.ToInt32(bids[i]);
                            arr.Add(_promotionBannerRelationDao.AddBrand(pbr_query));
                        }
                    }
                }

                arr.Add(_promotionBannerDao.UpdateImageInfo(query));
                return _mysqlDao.ExcuteSqls(arr);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerMgr-->UpdateImageInfo-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 新增促銷圖片
        public bool AddImageInfo(PromotionBannerQuery query, out int brand_id)
        {
            brand_id = 0;
            ArrayList arr = new ArrayList();
            string[] bids = null;
            try
            {
                if (query.brandIDS != string.Empty)
                {
                    bids = query.brandIDS.Split(',');
                }
                if (query.multi == 0)//不允許一個品牌多個促銷圖的話要檢查，否則不檢查
                {
                    if (bids != null)
                    {
                        query.date_start = query.pb_startdate;
                        query.date_end = query.pb_enddate;
                        AllowShowOrNot(query, bids, out brand_id);
                        if (brand_id != 0)
                        {
                            return false;//品牌编号在該促銷圖片顯示期間已有其他促銷圖片 且已啟用
                        }
                    }
                }
                int result = _promotionBannerDao.AddImageInfo(query);
                if (result > 0)
                {
                    PromotionBannerQuery model = _promotionBannerDao.GetNewPb_id();
                    PromotionBannerRelationQuery pbr_query = new PromotionBannerRelationQuery();
                    pbr_query.pb_id = model.pb_id;
                    if (bids != null)
                    {
                        for (int i = 0; i < bids.Length; i++)
                        {
                            if (bids[i] != string.Empty)
                            {
                                pbr_query.brand_id = Convert.ToInt32(bids[i]);
                                arr.Add(_promotionBannerRelationDao.AddBrand(pbr_query));
                            }
                        }
                    }
                    return _mysqlDao.ExcuteSqls(arr);
                }
                else
                {
                    return false;
                }
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
                            if (model != null && model.brandIDS != string.Empty)
                            {
                                string[] bids = model.brandIDS.Split(',');
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
        public bool DeleteImage(PromotionBannerQuery query)
        {
            try
            {
                ArrayList arr = new ArrayList();
                PromotionBannerRelationQuery pbr_query = new PromotionBannerRelationQuery();
                pbr_query.pb_id = query.pb_id;
                arr.Add(_promotionBannerRelationDao.DeleteBrand(pbr_query));
                arr.Add(_promotionBannerDao.DeleteImage(query));
                return _mysqlDao.ExcuteSqls(arr);
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
                if (model != null && model.brandIDS != string.Empty)
                {
                    int id = 0;
                    List<VendorBrand> vb_store = _vendorBrandDao.GetBrandListByIds(model.brandIDS, id);
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
                string pb_ids = string.Empty;
                DataTable dt = _promotionBannerDao.GetUsingImageInTimeArea(query);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        pb_ids += dt.Rows[i]["pb_id"] + ",";
                    }
                    pb_ids = pb_ids.Substring(0, pb_ids.LastIndexOf(","));
                    List<PromotionBannerRelationQuery> pbr_store = _promotionBannerRelationDao.GetBrandIds(pb_ids, string.Empty);
                    for (int a = 0; a < bids.Length; a++)
                    {
                        if (bids[a] != string.Empty)
                        {
                            query.singleBrand_id = Convert.ToInt32(bids[a]);
                        }
                        for (int b = 0; b < pbr_store.Count; b++)
                        {
                            if (query.singleBrand_id == pbr_store[b].brand_id)
                            {
                                brand_id = query.singleBrand_id;
                                return;
                                //品牌编号在該促銷圖片顯示期間已有其他促銷圖片 且已啟用
                            }
                        }
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
        public int AllowMultiOrNot(PromotionBannerQuery query, out int id)
        {
            int result = 0;
            id = 0;
            try
            {
                DataTable Using_Image = _promotionBannerDao.ShowUsingImage();
                StringBuilder bids = new StringBuilder();
                string pb_ids = string.Empty;
                for (int a = 0; a < Using_Image.Rows.Count; a++)
                {
                    if (!string.IsNullOrEmpty(Using_Image.Rows[a]["pb_startdate"].ToString()))
                    {
                        query.date_start = Convert.ToDateTime(Using_Image.Rows[a]["pb_startdate"].ToString());
                    }
                    if (!string.IsNullOrEmpty(Using_Image.Rows[a]["pb_enddate"].ToString()))
                    {
                        query.date_end = Convert.ToDateTime(Using_Image.Rows[a]["pb_enddate"].ToString());
                    }

                    for (int b = Using_Image.Rows.Count - 1; b > a; b--)
                    {
                        DateTime pb_startdate = Convert.ToDateTime(Using_Image.Rows[b]["pb_startdate"].ToString());
                        DateTime pb_enddate = Convert.ToDateTime(Using_Image.Rows[b]["pb_enddate"].ToString());
                        if (pb_startdate <= query.date_start && pb_enddate >= query.date_end ||
                            pb_startdate <= query.date_start && pb_enddate >= query.date_start ||
                            pb_startdate >= query.date_start && pb_enddate <= query.date_end ||
                            pb_startdate >= query.date_start && pb_startdate <= query.date_end && pb_enddate >= query.date_end)
                        {
                            if (pb_ids == string.Empty)
                            {
                                pb_ids = Using_Image.Rows[b]["pb_id"].ToString();
                            }
                            else
                            {
                                pb_ids += "," + Convert.ToInt32(Using_Image.Rows[b]["pb_id"].ToString());
                            }
                        }
                    }
                }
                List<PromotionBannerRelationQuery> pbr_store = _promotionBannerRelationDao.GetBrandIds(pb_ids, string.Empty);
                if (pbr_store.Count > 0)
                {
                    for (int i = 0; i < pbr_store.Count; i++)
                    {
                        List<PromotionBannerRelationQuery> pbr_store_two = _promotionBannerRelationDao.GetBrandIds(pb_ids, pbr_store[i].brand_id.ToString());
                        if (pbr_store_two.Count > 1)
                        {
                            query.singleBrand_id = pbr_store[i].brand_id;
                            PromotionBannerQuery model = _promotionBannerDao.GetModelById(pbr_store[i].pb_id);
                            query.date_start = model.pb_startdate;
                            query.date_end = model.pb_enddate;
                            query.pb_id = pbr_store[i].pb_id;
                            DataTable repet_dt = _promotionBannerDao.GetUsingImageInTimeArea(query);
                            if (repet_dt.Rows.Count > 0)
                            {
                                id = pbr_store[i].brand_id;
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
