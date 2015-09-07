using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;


namespace BLL.gigade.Mgr
{
    public class EdmGroupEmailMgr:IEdmGroupEmailImpIMgr
    {
        private IEdmGroupEmailImpIDao _IEdmGroupEmailMgr;
        private ISerialImplDao _serialDao;
        private EdmEmailDao _edmemailDao;

        public EdmGroupEmailMgr(string connectionString)
        {
            _IEdmGroupEmailMgr = new EdmGroupEmailDao(connectionString);
            _serialDao = new SerialDao(connectionString);
            _edmemailDao = new EdmEmailDao(connectionString);
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<Model.Query.EdmGroupEmailQuery> GetEdmGroupEmailList(Model.Query.EdmGroupEmailQuery query, out int totalCount)
        {
            try
            {
                return _IEdmGroupEmailMgr.GetEdmGroupEmailList(query,out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupEmailMgr-->GetEdmGroupEmailList" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 頁面加載
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Model.Query.EdmGroupQuery Load(Model.Query.EdmGroupQuery query)
        {
            try
            {
                return _IEdmGroupEmailMgr.Load(query);
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupEmailMgr-->Load" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 電子報群組成員刪除
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int DeleteEdmGroupEmail(Model.Query.EdmGroupEmailQuery query)
        {
            try
            {
                return _IEdmGroupEmailMgr.DeleteEdmGroupEmail(query);
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupEmailMgr-->DeleteEdmGroupEmail" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 新增編輯處理
        /// </summary>
        /// <param name="EGEquery"></param>
        /// <param name="EEquery"></param>
        /// <returns></returns>
        public string EdmGroupEmailEdit(Model.Query.EdmGroupEmailQuery query)
        {
            query.Replace4MySQL();
            string json = string.Empty;
            EdmEmail oldQuery = new EdmEmail();
            EdmEmail emailQuery = new EdmEmail();
            EdmGroupEmailQuery groupemailQuery = new EdmGroupEmailQuery();
            List<EdmEmail> store = new List<EdmEmail>();
            List<EdmGroupEmailQuery> listEGE=new List<EdmGroupEmailQuery>();
            Serial serial = new Serial();
            uint email_id=query.email_id;
            try
            {
                emailQuery.email_name = query.email_name;                
                if (string.IsNullOrEmpty(query.email_name))
                {//如果郵件名稱沒有填,則默認郵件地址@符號以前的內容
                    string[] strs = query.email_address.Split('@');
                    query.email_name = strs[0].ToString();
                    emailQuery.email_name = query.email_name;
                }
                oldQuery.email_address = query.email_address;
                emailQuery.email_address = query.email_address;              
                #region 判斷郵件地址在edm_email表中是否存在
                store = _IEdmGroupEmailMgr.getList(oldQuery);//判斷該郵件是否存在
                if (store != null && store.Count > 0)//存在,獲取email_id
                {
                    emailQuery.email_id = store[0].email_id;
                    emailQuery.email_updatedate = Convert.ToInt32(Common.CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                    if (query.email_name != store[0].email_name)//檔修改的郵件名稱與已存在的不一致就更新成一致
                    {
                        if (_IEdmGroupEmailMgr.UpdateEdmEmail(emailQuery) <= 0)
                        {
                            json = "{success:true,msg:1,type:0}";//msg=1代表更新郵件時出錯,type=0代表更新郵件名稱時錯誤
                            return json;
                        }
                        groupemailQuery.email_name = query.email_name;
                        groupemailQuery.email_updatedate = Convert.ToUInt32(Common.CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                        groupemailQuery.email_id = emailQuery.email_id;
                        DataTable ids = _IEdmGroupEmailMgr.GetGroupID(emailQuery.email_address);
                        for (int i = 0; i < ids.Rows.Count; i++)
                        {
                            groupemailQuery.group_id = Convert.ToUInt32(ids.Rows[i][0]);
                            int result = _IEdmGroupEmailMgr.UpdateEGEname(groupemailQuery);
                            if (result > 0)
                            {
                                continue;
                            }
                            else
                            {
                                json = "{success:true,msg:1,type:0}";
                                return json;
                            }
                        }                       
                    }
                }
                else
                { //不存在,在edm_email表新增
                    //uint largestID=0;
                    //string empty = string.Empty;
                    //uint id = 0;
                   // _edmemailDao.GetData(empty, out largestID, out id, out empty);
                    string sql = _serialDao.Update(51);//51代表edm_email表
                    serial = _IEdmGroupEmailMgr.execSql(sql);
                    emailQuery.email_id = Convert.ToUInt32(serial.Serial_Value);//largestID + 1;
                    emailQuery.email_createdate = Convert.ToInt32(Common.CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                    emailQuery.email_updatedate = Convert.ToInt32(Common.CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                    emailQuery.email_check = 0;
                    _IEdmGroupEmailMgr.insertEdmEmail(emailQuery);
                }
                #endregion
                query.email_id = emailQuery.email_id;
                listEGE = _IEdmGroupEmailMgr.Check(query);
                #region 編輯
                if (email_id > 0 || (listEGE != null && listEGE.Count > 0))//編輯
                {
                    if (listEGE == null || listEGE.Count <= 0)//檔填寫的郵件地址對應的email_id在edm_group_email表中沒有對應的數據時,新增一條數據
                    {
                        query.email_createdate = Convert.ToUInt32(Common.CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                        query.email_updatedate = Convert.ToUInt32(Common.CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                        _IEdmGroupEmailMgr.insertEGEInfo(query);
                    }
                    query.email_updatedate = Convert.ToUInt32(Common.CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                    if (email_id != 0 && email_id != query.email_id)//檔編輯時修改的郵件地址不是原來的郵件地址時,把原來的edm_group_email表中的數據刪除
                    {
                        EdmGroupEmailQuery egequery = new EdmGroupEmailQuery();
                        egequery.group_id = query.group_id;
                        egequery.email_ids = email_id.ToString();
                        _IEdmGroupEmailMgr.DeleteEdmGroupEmail(egequery);
                    }
                    int res = _IEdmGroupEmailMgr.UpdateEGE(query);//更新edm_group_email表數據
                    if (res > 0)
                    {
                        json = "{success:true,msg:0,type:1}";//0表示成功,type=1代表編輯
                    }
                    else
                    {
                        json = "{success:true,msg:3,type:1}";//0表示更新失敗,type=1代表編輯
                        return json;
                    }

                }
                #endregion
                #region 新增
                else
                { //新增
                    query.email_createdate = Convert.ToUInt32(Common.CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                    query.email_updatedate = Convert.ToUInt32(Common.CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                    int res = _IEdmGroupEmailMgr.insertEGEInfo(query);//新增edm_group_email表數據
                    if (res > 0)
                    {
                        json = "{success:true,msg:0,type:2}";//0表示成功,type=2代表新增操作
                    }
                    else
                    {
                        json = "{success:true,msg:2,type:2}";//msg=2表示新增時出錯了,type=2代表新增操作
                        return json;
                    }
                }
                #endregion
                #region 更新edm_group表的群組人數
                int num = UpdateCount(Convert.ToInt32(query.group_id));
                if (num <= 0)
                {
                    json = "{success:false}";
                }
                #endregion
                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupEmailMgr-->EdmGroupEmailEdit" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 更新edm_group表的群組人數
        /// </summary>
        /// <param name="group_id"></param>
        /// <returns></returns>
        public int UpdateCount(int group_id)
        {
            int num = 0;
            try
            {
                //獲取群組訂閱總人數
                DataTable dt = new DataTable();
                EdmGroupQuery egquery = new EdmGroupQuery();
                dt = _IEdmGroupEmailMgr.getCount(group_id);
                if (dt != null && dt.Rows.Count > 0)
                {
                    egquery.group_total_email = Convert.ToUInt32(dt.Rows[0][0]);
                    egquery.group_id = Convert.ToUInt32(group_id);
                    num = _IEdmGroupEmailMgr.updateEdmGroupCount(egquery);
                    return num;
                }
                else
                {
                    return num;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupEmailMgr-->UpdateCount" + ex.Message, ex);
            }
          
        }


        public List<EdmGroupEmailQuery> GetModel(EdmGroupEmail query)
        {
            try
            {
                return _IEdmGroupEmailMgr.GetModel(query);
            }
            catch (Exception ex)
            {
                throw new Exception("EdmGroupEmailMgr-->GetModel" + ex.Message, ex);
            }
        }
    }
}
