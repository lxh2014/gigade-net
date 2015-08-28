using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
  public  interface IMailGroupImplMgr
    {
      List<MailGroupQuery> MailGroupList(MailGroupQuery query,out int totalCount);
      int SaveMailGroup(MailGroupQuery query);
      bool DeleteMailGroup(List<MailGroupQuery> list);
      int UpMailGroupStatus(MailGroupQuery query);
      string QueryUserById(MailGroupMapQuery query);
      int DeleteMailMap(int group_id);
      bool SaveMailMap(List<MailGroupMapQuery> list);
      int GetStatus(int user_id);

      /// <summary>
      /// 查詢用戶信息
      /// </summary>
      /// <param name="query">MailGroupMapQuery 對象</param>
      /// <returns>List<MailGroupMapQuery>集合</returns>
      List<MailGroupMapQuery> QueryUserInfo(MailGroupMapQuery query);
    }
}
