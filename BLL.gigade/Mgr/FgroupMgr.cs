/*
* 文件名稱 :FgroupMgr.cs
* 文件功能描述 :群組
* 版權宣告 :
* 開發人員 : 天娥璇子
* 版本資訊 : 1.0
* 日期 : 2013/08/02
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using System.Data;

namespace BLL.gigade.Mgr
{
    public class FgroupMgr : IFgroupImplMgr
    {
        private IFgroupImplDao _fgDao;
        private ISiteConfigImplMgr siteConfigMgr;

        public FgroupMgr(string connectionString)
        {
            _fgDao = new FgroupMySqlDao(connectionString);
        }

        #region IFgroupImplMgr 成员

        public string QueryAll()
        {
            
            try
            {
                List<Fgroup> fgResult = _fgDao.QueryAll();
                StringBuilder stb = new StringBuilder();

                stb.Append("{");
                stb.Append(string.Format("success:true,items:["));
                foreach (Fgroup fg in fgResult)
                {
                    stb.Append("{");
                    stb.Append(string.Format("\"rowid\":\"{0}\",\"groupName\":\"{1}\",\"groupCode\":\"{2}\",\"callid\":\"{3}\",\"remark\":\"{4}\"", fg.rowid, fg.groupName, fg.groupCode, fg.callid, fg.remark));
                    stb.Append("}");
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                throw new Exception("FgroupMgr-->QueryAll-->" + ex.Message, ex);
            }
        }

        public bool QueryStockPrerogative(string callid,string xmlPath)
        {
           siteConfigMgr = new SiteConfigMgr(xmlPath);
           string groupCode = siteConfigMgr.GetConfigByName("STOCK_PREROGATIVE").Value;
           return _fgDao.Query(callid, groupCode).Count() > 0;
        }

        public int Save(Fgroup fg)
        {
            
            try
            {
                return _fgDao.Save(fg);
            }
            catch (Exception ex)
            {
                throw new Exception("FgroupMgr-->Save-->" + ex.Message, ex);
            }
        }

        public int Delete(Fgroup fg)
        {
            try
            {
                return _fgDao.Delete(fg);
            }
            catch (Exception ex)
            {
                throw new Exception("FgroupMgr-->Delete-->" + ex.Message, ex);
            }
            
        }

        public string QueryCallid()
        {
            try
            {
                List<ManageUser> muResult = _fgDao.QueryCallid();
                StringBuilder stb = new StringBuilder();

                stb.Append("{");
                stb.Append(string.Format("success:true,items:["));
                foreach (ManageUser mu in muResult)
                {
                    stb.Append("{");
                    stb.Append(string.Format("\"name\":\"{0}\",\"callid\":\"{1}\"", mu.name, mu.callid));
                    stb.Append("}");
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                throw new Exception("FgroupMgr-->QueryCallid-->" + ex.Message, ex);
            }
        }

        #endregion

        public DataTable GetFgroupList()
        {
            try
            {
                return _fgDao.GetFgroupList();
            }
            catch (Exception ex)
            {
                throw new Exception("FgroupMgr-->ExportGroupLimit-->" + ex.Message, ex);
            }
        }

        public DataTable GetUsersByGroupId(int groupid)
        {
            try
            {
                return _fgDao.GetUsersByGroupId(groupid);
            }
            catch (Exception ex)
            {
                throw new Exception("FgroupMgr-->GetUsersByGroupId-->" + ex.Message, ex);
            }
        }
        public DataTable GetAuthorityByGroupId(int groupid)
        {
            try
            {
                return _fgDao.GetAuthorityByGroupId(groupid);
            }
            catch (Exception ex)
            {
                throw new Exception("FgroupMgr-->GetAuthorityByGroupId-->" + ex.Message, ex);
            }
        }


        public Fgroup GetSingle(Fgroup model)
        {
            try
            {
                return _fgDao.GetSingle(model);
            }
            catch (Exception ex)
            {
                throw new Exception("FgroupMgr-->GetSingle-->" + ex.Message, ex);
            }
        }
    }
}
