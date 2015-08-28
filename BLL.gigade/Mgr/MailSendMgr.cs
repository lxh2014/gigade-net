using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
   public  class MailSendMgr
    {
        string connectionstring = string.Empty;
        MailSendDao dao;
        public MailSendMgr(string _connectionstring)
        {
            this.connectionstring = _connectionstring;
            dao = new MailSendDao(connectionstring);
        }
       #region 查分頁數據
        public List<MailSendQuery> GetData(MailSendQuery query, out int totalCount)
       {
           try
           {
               return dao.GetData(query, out totalCount);
           }
           catch (Exception ex)
           {
               throw new Exception("MailSendMgr->GetData:"+ex.Message);
           }
       }
       #endregion
    }
}
