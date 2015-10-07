/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：VendorBrandMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/21 16:48:48 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Dao;
using System.Data;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class VendorBrandMgr : IVendorBrandImplMgr
    {
        private IVendorBrandImplDao _vendorBrandDao;
        public VendorBrandMgr(string connectionStr)
        {
            _vendorBrandDao = new VendorBrandDao(connectionStr);
        }

        public VendorBrand GetProductBrand(VendorBrand query)
        {
            try
            {
                return _vendorBrandDao.GetProductBrand(query);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandMgr-->GetProductBrand-->" + ex.Message, ex);
            }

        }

        public List<VendorBrand> GetProductBrandList(VendorBrand brand)
        {
            try
            {
                return _vendorBrandDao.GetProductBrandList(brand);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandMgr-->GetProductBrandList-->" + ex.Message, ex);
            }
        }

        public string QueryBrand(VendorBrand brand, int hideOffGrade = 0)
        {
            try
            {
                StringBuilder stb = new StringBuilder();
                List<VendorBrand> results = _vendorBrandDao.GetProductBrandList(brand, hideOffGrade);
                stb.Append("{");
                stb.Append("success:true,item:[");
                foreach (VendorBrand item in results)
                {
                    stb.Append("{");
                    stb.AppendFormat("\"brand_id\":\"{0}\",\"brand_name\":\"{1}\"", item.Brand_Id, item.Brand_Name);
                    stb.Append("}");
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {

                throw new Exception("VendorBrandMgr-->QueryBrand-->" + ex.Message, ex);
            }

        }
        public string QueryClassBrand(VendorBrand brand, uint cid, int hideOffGrade = 0)
        {
            try
            {
                StringBuilder stb = new StringBuilder();
                List<VendorBrand> results = _vendorBrandDao.GetClassBrandList(brand, cid, hideOffGrade);
                stb.Append("{");
                stb.Append("success:true,item:[");
                stb.Append("{");
                stb.AppendFormat("\"brand_id\":\"{0}\",\"brand_name\":\"{1}\"", 0, "不分");
                stb.Append("}");
                foreach (VendorBrand item in results)
                {
                    stb.Append("{");
                    stb.AppendFormat("\"brand_id\":\"{0}\",\"brand_name\":\"{1}\"", item.Brand_Id, item.Brand_Name);
                    stb.Append("}");
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {

                throw new Exception("VendorBrandMgr-->QueryClassBrand-->" + ex.Message, ex);
            }
        }
        public DataTable GetBandList(string sqlconcat)
        {
            try
            {
                return _vendorBrandDao.GetBandList(sqlconcat);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandMgr-->GetBandList-->" + ex.Message, ex);
            }
        }
        public List<VendorBrandQuery> GetBandList(VendorBrandQuery query)
        {
            try
            {
                return _vendorBrandDao.GetBandList(query);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandMgr-->GetBandList-->" + ex.Message, ex);
            }
        }
        public DataTable GetVendorBrandStory(VendorBrandQuery query, out int totalCount)
        {
            try
            {
                return _vendorBrandDao.GetVendorBrandStory(query, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("VendorBrandMgr-->GetVendorBrandStory-->" + ex.Message, ex);
            }
        }
        public int AddVendorBrandStory(VendorBrandQuery query)
        {
            try
            {
                return _vendorBrandDao.AddVendorBrandStory(query);
            }
            catch (Exception ex)
            {

                throw new Exception("VendorBrandMgr-->AddVendorBrandStory-->" + ex.Message, ex);
            }
        }

        public int GetClassify(VendorBrandQuery query)
        {
            try
            {
                return _vendorBrandDao.GetClassify(query);
            }
            catch (Exception ex)
            {

                throw new Exception("VendorBrandMgr-->GetClassify-->" + ex.Message, ex);
            }
        }
        public List<VendorBrand> GetVendorBrand(VendorBrandQuery query)
        {
            try
            {
                return _vendorBrandDao.GetVendorBrand(query);
            }
            catch (Exception ex)
            {

                throw new Exception("VendorBrandMgr-->GetVendorBrand-->" + ex.Message, ex);
            }
        }

        public int DelPromoPic(int brand_id,string type)
        {
            try
            {
                return _vendorBrandDao.DelPromoPic(brand_id,type);
            }
            catch (Exception ex)
            {

                throw new Exception("VendorBrandMgr-->DelPromoPic-->" + ex.Message, ex);
            }
        }
    }
}
