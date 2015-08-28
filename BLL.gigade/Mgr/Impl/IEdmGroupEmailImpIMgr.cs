using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IEdmGroupEmailImpIMgr
    {
        List<EdmGroupEmailQuery> GetEdmGroupEmailList(EdmGroupEmailQuery query,out int totalCount);//電子報群組名單列表
        EdmGroupQuery Load(EdmGroupQuery query);//群組信息獲取
        int DeleteEdmGroupEmail(EdmGroupEmailQuery query);//電子報群組名單刪除
        string EdmGroupEmailEdit(EdmGroupEmailQuery query);//電子報群組新增/編輯處理
        int UpdateCount(int group_id);//更新edm_group中的群組訂閱總人數
        List<EdmGroupEmailQuery> GetModel(EdmGroupEmail query);   
    }
}
