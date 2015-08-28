/*
* 文件名稱 :SystemKeyWordMgr.cs
* 文件功能描述 :處理系統關鍵字的邏輯
* 版權宣告 :鄭州分公司
* 開發人員 : 文博
* 版本資訊 : 1.0
* 日期 : 2015-7-31
* 修改人員 :無
* 版本資訊 : 1.0
* 日期 : 2015-7-31
* 修改備註 :無
*/
using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class SystemKeyWordMgr
    {
        private SystemKeyWordDao dao;
        private string connectionstring = string.Empty;
        public SystemKeyWordMgr(string connectionstring)
        {
            this.connectionstring = connectionstring;
        }
        #region 獲取系統關鍵字列表
        public List<SphinxKeywordQuery> GetSystemKeyWord(SphinxKeywordQuery query, out int totalCount)
        {
            try
            {
                dao = new SystemKeyWordDao(connectionstring);
                return dao.GetSystemKeyWord(query, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("SystemKeyWordMgr->GetSystemKeyWord():" + ex.Message);
            }

        }
        #endregion
        #region 新增/修改關鍵字列表  SaveSystemKeyWord(SphinxKeywordQuery query)
        public int SaveSystemKeyWord(SphinxKeywordQuery query)
        {
            ///返回的状态
            int state = 0;
            try
            {
                dao = new SystemKeyWordDao(connectionstring);
                //新增
                if (query.operateType == 1)
                {
                    query.kuser = query.user_name;
                    query.moduser = query.user_name;
                    query.kdate = DateTime.Now;
                    query.mddate = DateTime.Now;
                    state = dao.AddSystemKeyWord(query);
                }
                //編輯
                if (query.operateType == 2)
                {
                    query.moduser = query.user_name;
                    query.mddate = DateTime.Now;
                    state = dao.UpdateSystemKeyWord(query);
                }
                return state;
            }
            catch (Exception ex)
            {

                throw new Exception("SystemKeyWordMgr->SaveSystemKeyWord():" + ex.Message);
            }

        } 
        #endregion
        #region 刪除關鍵字 +DelSystemKeyWord(SphinxKeywordQuery query)
        public int DelSystemKeyWord(SphinxKeywordQuery query)
        {

            ///返回的状态
            int state = 0;
            try
            {
                dao = new SystemKeyWordDao(connectionstring);
                state = dao.DelSystemKeyWord(query);
                return state;
            }
            catch (Exception ex)
            {

                throw new Exception("SystemKeyWordMgr->DelSystemKeyWord():" + ex.Message);
            }
        } 
        #endregion
        #region 獲取匯出需要的信息 + GetKeyWordExportList(SphinxKeywordQuery query)
        public List<SphinxKeywordQuery> GetKeyWordExportList(SphinxKeywordQuery query)
        {
            try
            {
                dao = new SystemKeyWordDao(connectionstring);
                return dao.GetKeyWordExportList(query);
            }
            catch (Exception ex)
            {
                throw new Exception("SystemKeyWordMgr->GetKeyWordExportList():" + ex.Message);
            }
        } 
        #endregion
        #region 判斷關鍵字是否存在 + YesOrNokeyWordExsit(string key_word)
        public int CheckKeyWordExsit(string key_word)
        {
            try
            {
                dao = new SystemKeyWordDao(connectionstring);
                return dao.CheckKeyWordExsit(key_word);
            }
            catch (Exception ex)
            {
                throw new Exception("SystemKeyWordMgr-->YesOrNokeyWordExsit-->" + ex.Message, ex);
            }
        } 
        #endregion
    }
}
