using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;
//changjian 于 2014/11/04 建立
namespace BLL.gigade.Mgr
{
    public class IplasMgr:IIplasImplMgr
    {
        private IIplasImplDao _IplasDao;
        public IplasMgr(string connectionString)
        {
            _IplasDao = new IplasDao(connectionString);
        }
        #region 查詢列表頁面
        public List<IplasQuery> GetIplas(IplasQuery m, out int totalCount)
        {
            try
            {
                return _IplasDao.GetIplas(m, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("IplasMgr-->GetIplas-->" + ex.Message, ex);
            }
        }
        #endregion
        #region  新增
        public int InsertIplas(Iplas m)
        {
            try
            {
                return _IplasDao.InsertIplasList(m);
            }
            catch (Exception ex)
            {
                throw new Exception("IplasMgr-->InsertIplas-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 編輯
        public int UpIplas(Iplas m)
        {
            try
            {
                return _IplasDao.UpIplas(m);
            }
            catch (Exception ex)
            {
                throw new Exception("IplasMgr-->UpIplas-->" + ex.Message, ex);
            }
        }
        #endregion


        #region 判斷商品是否存在


        public string IsTrue(Iplas m)
        {
            try
            {
                return _IplasDao.IsTrue(m);
            }
            catch (Exception ex)
            {
                throw new Exception("IplasMgr-->IsTrue-->" + ex.Message, ex);
            }
        }

        #endregion

        #region 判斷主料位是否重複
        public List<Iplas> GetIplasCount(Iplas m)
        {
            try
            {
                return _IplasDao.GetIplasCount(m);
            }
            catch (Exception ex)
            {
                throw new Exception("IplasMgr-->GetIplasCount-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 判斷主料位是否存在


        public int GetLocCount(Iloc loc)
        {
            try
            {
                return _IplasDao.GetLocCount(loc);
            }
            catch (Exception ex)
            {
                throw new Exception("IplasMgr-->GetLocCount-->" + ex.Message, ex);
            }
        }

        #endregion

        #region 通過條碼查詢商品編號


        public DataTable Getprodbyupc(string prodid)
        {
            try
            {
                return _IplasDao.Getprodbyupc(prodid);
            }
            catch (Exception ex)
            {
                throw new Exception("IplasMgr-->Getprodbyupc-->" + ex.Message, ex);
            }
        }

        #endregion

        public List<IplasQuery> Export(IplasQuery m)
        {
            try
            {
                return _IplasDao.Export(m);
            }
            catch (Exception ex)
            {
                throw new Exception("IplasMgr-->Export-->" + ex.Message, ex);
            }
        }

        //public List<ProductQueryForExcel> NoIlocReportList(ProductQuery query, out int totalCount) 
        //{
        //    try
        //    {
        //        return _IplasDao.NoIlocReportList(query,out totalCount);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("IplasMgr-->NOIlocReportList-->" + ex.Message, ex);
        //    }
        //}

       public List<Vendor> VendorQueryAll(Vendor query, string AddSql = null)
        {
            try
            {
                return _IplasDao.VendorQueryAll(query, AddSql);
            }
            catch (Exception ex)
            {
                throw new Exception("IplasMgr-->GetIlocReportList-->" + ex.Message, ex);
            }
        }
        public DataTable GetIlocReportList(ProductQuery query, out int totalCount)
        {
            try
            {
                return _IplasDao.GetIlocReportList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("IplasMgr-->GetIlocReportList-->" + ex.Message, ex);
            }
        }
        #region 查詢主料位是有商品佔據
        public int GetIinvdItemId(IinvdQuery vd)
        {
            try
            {
                return _IplasDao.GetIinvdItemId(vd);
            }
            catch (Exception ex)
            {

                throw new Exception("IplasMgr-->GetIinvdItemId-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 刪除沒有商品佔據的iplas 數據
        public int DeleteIplasById(IplasQuery plas)
        {
            try
            {
                return _IplasDao.DeleteIplasById(plas);
            }
            catch (Exception ex)
            {

                throw new Exception("IplasMgr-->DeleteIplasById-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 判斷商品是否重複
        public int GetIplasid(IplasQuery plas)
        {
            try
            {
                return _IplasDao.GetIplasid(plas);
            }
            catch (Exception ex)
            {

                throw new Exception("IplasMgr-->GetIplasid-->" + ex.Message, ex);
            }
        }
        #endregion



        public DataTable NoIlocReportList(ProductQuery query)
        {
            try
            {
                return _IplasDao.NoIlocReportList(query);
            }
            catch (Exception ex)
            {
                throw new Exception("IplasMgr-->NOIlocReportList-->" + ex.Message, ex);
            }
        }


        public DataTable ExportMessage(IplasQuery m)
        {
            try
            {
                return _IplasDao.ExportMessage(m);
            }
            catch (Exception ex)
            {
                throw new Exception("IplasMgr-->ExportMessage-->" + ex.Message, ex);
            }
        }

        public List<IplasQuery> GetIplasExportList(IplasQuery iplas)
        {
            try
            {
                return _IplasDao.GetIplasExportList(iplas);
            }
            catch (Exception ex)
            {
                throw new Exception("IplasMgr-->GetIplasExportList-->" + ex.Message, ex);
            }
        }


        public int YesOrNoExist(int item_id)
        {
            try
            {
                return _IplasDao.YesOrNoExist(item_id);
            }
            catch (Exception ex)
            {
                throw new Exception("IplasMgr-->YesOrNoExist-->" + ex.Message, ex);
            }
        }



        public int YesOrNoLocIdExsit(string loc_id)
        {
            try
            {
                return _IplasDao.YesOrNoLocIdExsit(loc_id);
            }
            catch (Exception ex)
            {
                throw new Exception("IplasMgr-->YesOrNoLocIdExsit-->" + ex.Message, ex);
            }
        }
        public int YesOrNoLocIdExsit(int item_id, string loc_id)
        {
            try
            {
                return _IplasDao.YesOrNoLocIdExsit(item_id,loc_id);
            }
            catch (Exception ex)
            {
                throw new Exception("IplasMgr-->YesOrNoLocIdExsit-->" + ex.Message, ex);
            }
        }

        public int ExcelImportIplas(string condition)
        {
            try
            {
                return _IplasDao.ExcelImportIplas(condition);
            }
            catch (Exception ex)
            {
                throw new Exception("IplasMgr-->ExcelImportIplas-->" + ex.Message, ex);
            }
        }
        public Iplas getplas(Iplas query)
        {
            try
            {
                return _IplasDao.getplas(query);
            }
            catch (Exception ex)
            {
                throw new Exception("IplasMgr-->getplas-->" + ex.Message, ex);
            }
        }
    }
}
