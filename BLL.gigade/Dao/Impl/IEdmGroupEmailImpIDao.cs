using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
     public interface IEdmGroupEmailImpIDao
    {
        List<EdmGroupEmailQuery> GetEdmGroupEmailList(EdmGroupEmailQuery query,out int totalCount);//電子報群組名單列表
        EdmGroupQuery Load(EdmGroupQuery query);//群組信息獲取
        int DeleteEdmGroupEmail(EdmGroupEmailQuery query);//電子報群組名單刪除
        List<EdmEmail> getList(EdmEmail query);//查詢郵件信息
        int UpdateEdmEmail(EdmEmail query);//更新edm_email表
        List<EdmGroupEmailQuery> Check(EdmGroupEmailQuery query);//新增/編輯時判斷EGE表中是否存在
        int insertEGEInfo(EdmGroupEmailQuery query);//新增EGE表數據
        int UpdateEGE(EdmGroupEmailQuery query);//ege表更新
        Serial execSql(string sql);//執行一串sql語句
        int insertEdmEmail(EdmEmail query);//新增edm_email表數據
        DataTable getCount(int group_id);//獲取群組的訂閱總人數
        int updateEdmGroupCount(EdmGroupQuery query);//更新群組的訂閱總人數
        string UpdateEdmEmailStr(EdmEmailQuery query);
        List<EdmGroupEmailQuery> GetModel(EdmGroupEmail query);
        DataTable GetGroupID(string email);
        int UpdateEGEname(EdmGroupEmailQuery query);
    }
}
