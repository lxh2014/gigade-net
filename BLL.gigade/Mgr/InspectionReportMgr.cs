using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.Configuration;
using BLL.gigade.Common;

namespace BLL.gigade.Mgr
{
    public class InspectionReportMgr
    {
        public InspectionReportDao _inspectionReport;
        public string imgServerPath = ConfigurationManager.AppSettings["imgServerPath"];//顯示圖片路徑http://192.168.71.10:8765
        public string InspectionReportPath = ConfigurationManager.AppSettings["InspectionReportPath"];//圖片保存路徑
        public string defaultImg = "/Content/img/nopic_50.jpg";
        public string imgLocalPath = ConfigurationManager.AppSettings["imgLocalPath"];
        public InspectionReportMgr(string connectionString)
        {
            _inspectionReport = new InspectionReportDao(connectionString);
        }
        #region 證書類型列表
        public List<CertificateCategoryQuery> GetCertificateCategoryList(CertificateCategoryQuery query, out int totalCount)
        {
            try
            {
                return _inspectionReport.GetCertificateCategoryList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->GetCertificateCategoryList-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 更新證書類型狀態
        public int UpdateActive(CertificateCategoryQuery query)
        {
            try
            {
                return _inspectionReport.UpdateActive(query);
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->UpdateActive-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 刪除前判斷是否是該大類對應的唯一小類
        public int CheckOnly(CertificateCategoryQuery query)
        {
            List<CertificateCategoryQuery> list = new List<CertificateCategoryQuery>();
            try
            {
                list = _inspectionReport.CheckOnly(query);
                return list.Count;
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->CheckOnly-->" + ex.Message , ex);
            }
        }
        #endregion
        #region 證書類型刪除
        public int DeleteCertificateCategory(CertificateCategoryQuery query)
        {
            try
            {
                return _inspectionReport.DeleteCertificateCategory(query);
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->DeleteCertificateCategory-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 事務刪除檔大類對應的小類全部被刪除.同時也刪除大類
        public bool DeleteCCByTransaction(CertificateCategoryQuery query)
        {
            try
            {
                if (_inspectionReport.DeleteCCByTransaction(query) > 0)
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
                throw new Exception("InspectionReportMgr-->DeleteCCByTransaction-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 新增證書類型大類store
        public List<CertificateCategoryQuery> GetStore(CertificateCategoryQuery query)
        {
            try
            {
                return _inspectionReport.GetStore(query);
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->GetStore-->" + ex.Message, ex);
            }
 
        }
        #endregion
        #region 根據id搜索證書信息
        public List<CertificateCategoryQuery> GetCertificateCategoryInfo(CertificateCategoryQuery query)
        {
            try
            {
                return _inspectionReport.GetCertificateCategoryInfo(query);
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->GetCertificateCategoryInfo-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 判斷證書大類名稱是否重複
        public bool CheckCertificateCategoryName(CertificateCategoryQuery query)
        {
            try
            {
                List<CertificateCategoryQuery> list = new List<Model.Query.CertificateCategoryQuery>();
                list= _inspectionReport.CheckCertificateCategoryName(query);
                if (list!=null&&list.Count > 0)
                {
                    return true;
                }
                else {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->CheckCertificateCategoryName-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 新增證書-大類返回新增id
        public int GetNewCertificateCategoryId(CertificateCategoryQuery query)
        {
            try
            {
                DataTable dt = _inspectionReport.GetNewCertificateCategoryId(query);
                return Convert.ToInt32(dt.Rows[0][0]);
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->GetNewCertificateCategoryId-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 證書大類更新
        public int UpdateCertificateCategory(CertificateCategoryQuery query)
        {
            try
            {
                return _inspectionReport.UpdateCertificateCategory(query);
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->UpdateCertificateCategory-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 編輯保存
        public bool Update(CertificateCategoryQuery query)
        {
            try
            {
                int res = _inspectionReport.Update(query);
                if (res > 0)
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
                throw new Exception("InspectionReportMgr-->Update-->" + ex.Message, ex);
            }
        }

        #endregion
        #region 新增保存
        public int AddSave(CertificateCategoryQuery query)
        {
            try
            {
                return _inspectionReport.AddSave(query);
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->AddSave-->" + ex.Message, ex);
            }
        }
          #endregion
        #region 證書小類名稱檢查
        public bool CheckChildName(CertificateCategoryQuery query)
        {
            try
            {
                List<CertificateCategoryQuery> list = new List<Model.Query.CertificateCategoryQuery>();
                list=_inspectionReport.CheckChildName(query);
                if(list!=null&&list.Count>0)
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
                throw new Exception("InspectionReportMgr-->CheckChildName-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 非重複code驗證(大類小類通用,不包含二級聯動,只適用於新增/編輯)
        public bool CheckCode(CertificateCategoryQuery query)
        {
            try
            {
                List<CertificateCategoryQuery> list = new List<Model.Query.CertificateCategoryQuery>();
                list=_inspectionReport.CheckCode(query);
                if (list != null && list.Count > 0)
                {
                    return true;
                }
                else {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->CheckCode-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 根據商品id搜品牌id
        public uint GetBrandId(uint productid)
        {
            ProductQuery query = new ProductQuery();
            try
            {
                query = _inspectionReport.GetBrandId(productid);
                return query.Brand_Id;
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->GetBrandId-->" + ex.Message , ex);
            }
        }
        #endregion

        public List<InspectionReportQuery> InspectionReportList(InspectionReportQuery query, out int totalCount)
        {
            try
            {
                List<InspectionReportQuery> store = new List<Model.Query.InspectionReportQuery>();
                store = _inspectionReport.InspectionReportList(query, out totalCount);
                foreach (var item in store)
                {
                    query=new Model.Query.InspectionReportQuery ();
                    query.certificate_type1=item.certificate_type1;
                    query.certificate_type2=item.certificate_type2;
                    if (item.certificate_filename != "")
                    {
                        item.certificate_filename_string = imgServerPath + InspectionReportPath + item.brand_id+"/" +item.product_id+"/" + item.certificate_filename;
                    }
                    else
                    {
                        item.certificate_filename_string = defaultImg;
                    }
                }

                return store;
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->InspectionReportList-->" + ex.Message, ex);
            }
        }

        public DataTable Export(InspectionReportQuery query)
        {
            try
            {
                return _inspectionReport.Export(query);

            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->SaveInspectionRe-->" + ex.Message, ex);
            }
        }

        public string SaveInspectionRe(InspectionReportQuery query)
        {
            string json = string.Empty;
            ArrayList arrList = new ArrayList();
            try
            {
                if (query.rowID != 0)
                {
                    query.m_date = DateTime.Now;
                    arrList.Add(_inspectionReport.UpInspectionRe(query));
                }
                else
                {
                    query.k_date = DateTime.Now;
                    query.m_date = query.k_date;
                    arrList.Add(_inspectionReport.InsertInspectionRe(query));
                }
                if (_inspectionReport.ExecSql(arrList))
                {
                    json = "{success:'true',msg:'0'}";//保存成功
                }
                else
                {
                    json = "{success:'false',msg:'1'}";//保存失敗
                }
                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->SaveInspectionRe-->" + ex.Message, ex);
            }
        }

        public string DeleteInspectionRe(List<InspectionReportQuery> list)
        {
            ArrayList arrList = new ArrayList();
            string json = string.Empty;
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    arrList.Add(_inspectionReport.DeleteInspectionRe(list[i]));
                }
                if (_inspectionReport.ExecSql(arrList))
                {
                    json = "{success:'true'}";
                }
                else
                {
                    json = "{success:'false'}";
                }
                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->DeleteInspectionRe-->" + ex.Message, ex);
            }
        }

        public string GetType1Folder(InspectionReportQuery query)
        {
            string folder = string.Empty;
            try
            {
              folder=_inspectionReport.GetType1Folder(query).ToString()+"/";
              return folder;
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->GetTypeFolder-->" + ex.Message, ex);
            }
        }
        public string GetType2Folder(InspectionReportQuery query)
        {
            string folder = string.Empty;
            try
            {
                folder = _inspectionReport.GetType2Folder(query).ToString() + "/";
                return folder;
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->GetTypeFolder-->" + ex.Message, ex);
            }
        }
        public InspectionReportQuery oldQuery(InspectionReportQuery query)
        {
            try
            {
                return _inspectionReport.oldQuery(query);
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->oldQuery-->" + ex.Message, ex);
            }
        }
        public List<CertificateCategoryQuery> GetType1Store(CertificateCategoryQuery query)
        {
            try
            {
                return _inspectionReport.GetType1Store(query);
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->GetType1Store-->" + ex.Message, ex);
            }
        }
        public List<CertificateCategoryQuery> GetType2Store(CertificateCategoryQuery query)
        {
            try
            {
                return _inspectionReport.GetType2Store(query);
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->GetType2Store-->" + ex.Message, ex);
            }
        }
        public bool IsExist(InspectionReportQuery query)
        {
            string json = string.Empty;
            bool b = true;
            try
            {
                DataTable _dt = _inspectionReport.IsExist(query);
                if (_dt.Rows.Count > 0)
                {
                    b = true;
                }
                else
                {
                    b = false;
                }
                return b;
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->IsExist-->"+ex.Message,ex);
            }
        }
        public string GetBrandID(InspectionReportQuery query)
        {
            string json = string.Empty;
            DataTable _dt = new DataTable();
            try
            {
                _dt=_inspectionReport.GetBrandID(query);
                if (_dt.Rows.Count > 0)
                {
                    string brand_id = _dt.Rows[0][0].ToString();
                    string brand_name = _dt.Rows[0][1].ToString();
                    json = "{success:true,brand_id:'" + brand_id + "',brand_name:'" + brand_name + "'}";
                }
                else
                {
                    json = "{success:false}";
                }
                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->IsExist-->" + ex.Message, ex);
            }
        }
        public string GetType1(InspectionReportQuery query)
        {
            try
            {
                return _inspectionReport.GetType1(query);
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->GetType1-->"  + ex.Message, ex);
            }
        }
        public string GetType2(InspectionReportQuery query)
        {
            try
            {
                return _inspectionReport.GetType2(query);
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->GetType2-->" + ex.Message, ex);
            }
        }
        #region 檢查報告匯入操作

        #region 判斷商品編號是否存在
        public bool GetProductById(InspectionReportQuery query)
        {
            List<ProductQuery> list = new List<Model.Query.ProductQuery>();
            try
            {
                list = _inspectionReport.GetProductById(query);
                if (list!=null&&list.Count > 0)
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
                throw new Exception("InspectionReportMgr-->GetProductById-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 檢查大類code是否存在
        public List<CertificateCategoryQuery> CheckBigCode(CertificateCategoryQuery query)
        {
            string folder = string.Empty;
            try
            {
                return _inspectionReport.CheckCode(query);
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->CheckBigCode-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 二級聯動,判斷大類下邊的小類是否存在
        public List<CertificateCategoryQuery> GetLsit(CertificateCategoryQuery query)
        {
            string json = string.Empty;
            DataTable _dt = new DataTable();
            try
            {
                return _inspectionReport.GetLsit(query);
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->GetLsit-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 匯入時判斷是否已存在
        public bool CheckInspectionReport(InspectionReportQuery query)
        {
            List<InspectionReportQuery> list = new List<Model.Query.InspectionReportQuery>();
            try
            {
                list = _inspectionReport.CheckInspectionReport(query);
                if (list != null && list.Count > 0)
                {
                    return true;
                }
                else {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->CheckInspectionReport-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 插入數據
        public bool InsertInspectionReport(InspectionReportQuery query)
        {
            try
            {
                if (query.certificate_desc.Length >= 50)
                {
                  query.certificate_desc=query.certificate_desc.Substring(0,50);
                }
                if (_inspectionReport.InsertInspectionReport(query) > 0)
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
                throw new Exception("InspectionReportMgr-->InsertInspectionReport-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 如果存在更新
        public bool UpdateInspectionReport(InspectionReportQuery query)
        {
            try
            {
                if (_inspectionReport.UpdateInspectionReport(query) > 0)
                {
                    return true;
                }
                else {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->UpdateInspectionReport-->" + ex.Message, ex);
            }
        }
        #endregion

        #endregion
        public bool BeforeDelete( string rowIDs)
        {
            string json = string.Empty;
            bool b = false;
            CertificateCategoryQuery query = new CertificateCategoryQuery();
            try
            {
                if (rowIDs.IndexOf("|") != -1)
                {
                    foreach (string id in rowIDs.Split('|'))
                    {
                        if (!string.IsNullOrEmpty(id))
                        {
                            string[] data = id.Split(',');
                            query = new CertificateCategoryQuery();
                            query.rowID = Convert.ToInt32(data[0]);
                            query.frowID = Convert.ToInt32(data[1]);
                            DataTable _dt = _inspectionReport.BeforeDelete(query);
                            if (_dt.Rows.Count > 0)
                            {
                                b = false;
                                break;
                            }
                            else
                            {
                                json = "{success:true,msg:'1'}";
                                b = true;
                            }
                        }
                    }
                }
              return b;
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->BeforeDelete-->" + ex.Message, ex);
            }
        }
        public bool IsSortExist(InspectionReportQuery query)
        {
            try
            {
                if (_inspectionReport.IsSortExist(query) == 0)
                {
                    return true;//不重複
                }
                else
                {
                    return false;//重複
                }
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportMgr-->IsSortExist-->" + ex.Message, ex);
            }
        }
    }
}

