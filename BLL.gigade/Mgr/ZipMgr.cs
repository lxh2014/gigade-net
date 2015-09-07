using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using System.Data;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class ZipMgr : IZipImplMgr
    {
        private IZipImplDao _zipDao;

        public ZipMgr(string connectionString)
        {
            _zipDao = new ZipDao(connectionString);
        }

        #region IZipImplMgr 成员

        public string QueryBig(string strTopValue)
        {
            StringBuilder stb = new StringBuilder();
            try
            {
                List<Zip> zipResult = _zipDao.QueryBig(strTopValue);
                stb.Append("{");
                stb.Append(string.Format("success:true,items:["));
                foreach (Zip z in zipResult)
                {
                    stb.Append("{");
                    stb.Append(string.Format("\"big\":\"{0}\",\"bigcode\":\"{1}\"", z.big, z.bigcode));
                    stb.Append("}");
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                throw new Exception("ZipMgr-->QueryBig-->" + ex.Message, ex);
            }
        }

        public string QueryMiddle(string strTopValue)
        {
            try
            {
                List<Zip> zipResult = _zipDao.QueryMiddle(strTopValue);
                StringBuilder stb = new StringBuilder();
                stb.Append("{");
                stb.Append(string.Format("success:true,items:["));
                foreach (Zip z in zipResult)
                {
                    stb.Append("{");
                    stb.Append(string.Format("\"middle\":\"{0}\",\"middlecode\":\"{1}\"", z.middle, z.middlecode));
                    stb.Append("}");
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                throw new Exception("ZipMgr-->QueryMiddle-->" + ex.Message, ex);
            }
        }

        public string QuerySmall(string strTopValue, string topText)
        {
            try
            {
                List<Zip> zipResult = _zipDao.QuerySmall(strTopValue, topText);
                StringBuilder stb = new StringBuilder();
                stb.Append("{");
                stb.Append(string.Format("success:true,items:["));
                foreach (Zip z in zipResult)
                {
                    stb.Append("{");
                    stb.Append(string.Format("\"small\":\"{0}\",\"zipcode\":\"{1}\"", z.zipcode + "/" + z.small, z.zipcode));
                    stb.Append("}");
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                throw new Exception("ZipMgr-->QuerySmall-->" + ex.Message, ex);
            }
        }
        public Zip QueryCityAndZip(string zipcode)
        {
            try
            {
                return _zipDao.QueryCityAndZip(zipcode);
            }
            catch (Exception ex)
            {
                throw new Exception("ZipMgr-->QueryCityAndZip-->" + ex.Message, ex);
            }
        }
        public DataTable ZipTable(Zip zip, String appendSql)
        {
            try
            {
                return _zipDao.ZipTable(zip, appendSql);
            }
            catch (Exception ex)
            {
                throw new Exception("ZipMgr-->ZipTable-->" + ex.Message, ex);
            }
        }

        public DataTable GetZip()
        {
            try
            {
                return _zipDao.GetZip();
            }
            catch (Exception ex)
            {
                throw new Exception("ZipMgr-->GetZip-->" + ex.Message, ex);
            }
        }
        public string Getaddress(int zipcode)
        {
            try
            {
                return _zipDao.Getaddress(zipcode);
            }
            catch (Exception ex)
            {
                throw new Exception("ZipMgr-->Getaddress-->" + ex.Message, ex);
            }
        }
        public List<Zip> GetZipList()
        {
            try
            {
                return _zipDao.GetZipList();
            }
            catch (Exception ex)
            {
                throw new Exception("ZipMgr-->GetZipList-->" + ex.Message, ex);
            }
        }
        #endregion
    }
}
