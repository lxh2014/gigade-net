using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IConfigImplMgr
    {
        List<ConfigQuery> Query(string paramername, int parameternumber);
      
        uint QueryByEmail(string email);
        uint QueryByName(string name);
        List<Model.ManageUser> getUserPm(string nameStr);
        DataTable GetConfig(ConfigQuery query);//
        int InsertConfig(ConfigQuery query);
        int UpdateConfig(ConfigQuery query);
        DataTable ConfigCheck(ConfigQuery query);//檢查config_name是否存在
        DataTable GetConfigByLike(ConfigQuery query);//根據like查詢出config的值
        DataTable GetUser(ManageUserQuery query);//搜索優化名稱store
        List<ConfigQuery> CheckSingleConfig(ConfigQuery query);//新增時查看收件人名稱是否重複
        List<ConfigQuery> CheckUserName(ConfigQuery query);//編輯判斷用戶名是否存在
    }
}
