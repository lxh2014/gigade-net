/*
* 文件名稱 :ChannelContactMgr.cs
* 文件功能描述 :外站聯絡人
* 版權宣告 :
* 開發人員 : 天娥璇子
* 版本資訊 : 1.0
* 日期 : 2013/08/19
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

namespace BLL.gigade.Mgr
{
    public class ChannelContactMgr:IChannelContactImplMgr
    {
        private IChannelContactImplDao _chcDao;

        public ChannelContactMgr(string connectionString)
        {
            _chcDao = new ChannelContactDao(connectionString);
        }

        #region IChannelContactImplMgr 成员

        public string Query(string strChannelId)
        {
            try
            {
                List<ChannelContact> chcResult = _chcDao.Query(strChannelId);
                StringBuilder stb = new StringBuilder();

                stb.Append("{");
                stb.Append(string.Format("success:true,items:["));
                foreach (ChannelContact chc in chcResult)
                {
                    stb.Append("{");
                    stb.Append(string.Format("\"rid\":\"{0}\",\"contact_type\":\"{1}\",\"contact_name\":\"{2}\",\"contact_phone1\":\"{3}\",\"contact_phone2\":\"{4}\",\"contact_mobile\":\"{5}\",\"contact_email\":\"{6}\"", chc.rid, chc.contact_type, chc.contact_name, chc.contact_phone1, chc.contact_phone2, chc.contact_mobile, chc.contact_email));
                    stb.Append("}");
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelContactMgr-->Query-->" + ex.Message, ex);
            }
        }

        public int Save(ChannelContact chc)
        {
            try
            {
                return _chcDao.Save(chc);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelContactMgr-->SingleCompareSave-->" + ex.Message, ex);
            }
        }

        public int Edit(ChannelContact chc)
        {
            try
            {
                return _chcDao.Edit(chc);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelContactMgr-->Edit-->" + ex.Message, ex);
            }
        }

        public int Delete(int rid)
        {
            try
            {
                return _chcDao.Delete(rid);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelContactMgr-->Edit-->" + ex.Message, ex);
            }
        }

        #endregion
    }
}
