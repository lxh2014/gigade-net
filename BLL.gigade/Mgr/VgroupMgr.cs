/*
* 文件名稱 :VgroupMgr.cs
* 文件功能描述 :群組
* 版權宣告 :
* 開發人員 : 天娥璇子
* 版本資訊 : 1.0
* 日期 : 2013/08/02
* 修改人員 :shiwei0620j
* 版本資訊 :    
* 日期 : 2014/08/18
* 修改備註 : 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class VgroupMgr : IVgroupImplMgr
    {
        private IVgroupImplDao _vgDao;
        public VgroupMgr(string connectionString)
        {
            _vgDao = new VgroupMySqlDao(connectionString);
        }
        public string QueryAll()
        {
            try
            {
                List<Vgroup> vgResult = _vgDao.QueryAll();
                StringBuilder stb = new StringBuilder();
                stb.Append("{");
                stb.Append(string.Format("success:true,items:["));
                foreach (Vgroup vg in vgResult)
                {
                    stb.Append("{");
                    stb.Append(string.Format("\"rowid\":\"{0}\",\"groupName\":\"{1}\",\"groupCode\":\"{2}\",\"callid\":\"{3}\",\"remark\":\"{4}\"", vg.rowid, vg.groupName, vg.groupCode, vg.callid, vg.remark));
                    stb.Append("}");
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                throw new Exception("VgroupMgr-->QueryAll-->" + ex.Message, ex);
            }
        }

        public string QueryCallid()
        {
            try
            {
                List<ManageUser> muResult = _vgDao.QueryCallid();
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
                throw new Exception("VgroupMgr-->QueryCallid-->" + ex.Message, ex);
            }
        }

        public int Save(Model.Vgroup vg)
        {
            try
            {
                return _vgDao.Save(vg);
            }
            catch (Exception ex)
            {
                throw new Exception("VgroupMgr-->Save-->" + ex.Message, ex);
            }
        }

        public int Delete(Model.Vgroup vg)
        {
            try
            {
                return _vgDao.Delete(vg);
            }
            catch (Exception ex)
            {
                throw new Exception("VgroupMgr-->Delete-->" + ex.Message, ex);
            }
        }
    }
}
