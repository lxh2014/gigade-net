using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class DesignRequestDao
    {
        private IDBAccess _accessMySql;
        private string connStr;
        public DesignRequestDao(string connectionstring)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
        }
        #region 獲取design_request表的list
        public List<DesignRequestQuery> GetList(DesignRequestQuery query,out int totalCount)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbwhere = new StringBuilder();
            StringBuilder sbcount = new StringBuilder();
            totalCount = 0;
            try
            {
                sb.Append("SELECT dr.dr_id,dr_type,dr.dr_content_text,dr.dr_description,dr.dr_document_path,dr.dr_resource_path");
                sb.Append(",mu.user_username AS dr_requester_id_name,mu1.user_username AS dr_assign_to_name,tp1.parameterName as dr_type_tostring,tp3.parameterName AS dr_status_tostring,dr.dr_created,dr.dr_modified,dr.product_id,p.product_name,dr.dr_status,dr_expected,dr_assign_to ");
                sbwhere.Append(" FROM design_request dr ");
                sbwhere.Append("  LEFT JOIN manage_user mu  ON mu.user_id=dr.dr_requester_id  ");
                sbwhere.Append(" LEFT JOIN manage_user mu1 ON mu1.user_id=dr.dr_assign_to  ");
                sbwhere.Append(" LEFT JOIN product p ON p.product_id=dr.product_id  ");
                sbwhere.Append(" LEFT JOIN (SELECT tp.parameterName,tp.parameterCode FROM t_parametersrc tp WHERE tp.parameterType='job_type') tp1 ON tp1.parameterCode=dr.dr_type  ");
                sbwhere.Append(" LEFT JOIN (SELECT tp2.parameterName,tp2.parameterCode FROM t_parametersrc tp2 WHERE tp2.parameterType='job_status') tp3 ON tp3.parameterCode=dr.dr_status  ");
                sbcount.Append(" select count(dr.dr_id) as totalCount ");
                sbwhere.Append(" where 1=1 ");
                if (!IsManager(query) && !IsEmail(query) && !IsManagerNumber(query))
                {//不是主管,不是郵件群組人員,不是設計部人員就限制查看的項目
                    sbwhere.AppendFormat(" and (dr.dr_requester_id='{0}' or dr.dr_assign_to ='{0}') ", query.login_id);
                }
                if (!string.IsNullOrEmpty(query.dr_requester_id_name))
                {
                    sbwhere.AppendFormat(" AND mu.user_username LIKE '%{0}%'  ", query.dr_requester_id_name);
                }
                if (query.dr_type!=0)
                {
                    sbwhere.AppendFormat(" and dr.dr_type='{0}' ", query.dr_type);
                }
                if (query.dr_assign_to != 0)
                {
                    sbwhere.AppendFormat(" and dr.dr_assign_to='{0}' ", query.dr_assign_to);
                }
                if (query.dr_status != 0)
                {
                    sbwhere.AppendFormat(" and dr.dr_status='{0}' ", query.dr_status);
                }
                else
                {//如果沒有選擇狀態默認為非已結案的全部顯示
                    sbwhere.AppendFormat(" and dr.dr_status<>'6' ", query.dr_status);
                }
                if (query.date_type != 0)
                {
                    if (!string.IsNullOrEmpty(query.start_time))
                    {
                        sbwhere.AppendFormat(" and dr.dr_created>='{0}' ", query.start_time);
                    }
                    if (!string.IsNullOrEmpty(query.end_time))
                    {
                        sbwhere.AppendFormat(" and dr.dr_created<='{0}' ", query.end_time);
                    }
                }
                if (query.IsPage)
                {
                    sbcount.Append(sbwhere.ToString());
                    DataTable dt = _accessMySql.getDataTable(sbcount.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0]["totalCount"]);
                    }
                }
                sbwhere.AppendFormat(" order by dr.dr_id desc limit {0},{1} ",query.Start,query.Limit);
                sb.Append(sbwhere.ToString());
                return _accessMySql.getDataTableForObj<DesignRequestQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestDao.GetList-->" + ex.Message + sb.ToString()+sbwhere.ToString(), ex);
            }
        }
        #endregion
        #region 新增
        public int InsertDesignRequest(DesignRequestQuery query)
        {
            StringBuilder sb = new StringBuilder();
            query.Replace4MySQL();
            try
            {
                sb.AppendFormat(@"INSERT INTO design_request (dr_requester_id,dr_type,dr_assign_to,dr_content_text,dr_description,dr_resource_path,dr_document_path,dr_status,dr_created,product_id,dr_expected) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',NOW(),'{8}',DATE_ADD(NOW(),INTERVAL '{9}' DAY));", query.dr_requester_id, query.dr_type, query.dr_assign_to, query.dr_content_text, query.dr_description, query.dr_resource_path, query.dr_document_path, query.dr_status, query.product_id, query.day);
                return _accessMySql.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestDao.InsertDesignRequest-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 編輯
        public int UpdateDesignRequest(DesignRequestQuery query)
        {
            StringBuilder sb = new StringBuilder();
            query.Replace4MySQL();
            try
            {
                sb.AppendFormat(@"UPDATE design_request SET dr_content_text='{0}',dr_description='{1}',dr_resource_path='{2}',dr_document_path='{3}',dr_type='{4}',product_id='{5}',dr_status='{6}' ", query.dr_content_text, query.dr_description, query.dr_resource_path, query.dr_document_path, query.dr_type, query.product_id,query.dr_status);
                if (query.day > 0)
                {
                    sb.AppendFormat(@",dr_expected=DATE_ADD(NOW(),INTERVAL '{0}' DAY) ", query.day);
                }
                sb.AppendFormat(@"WHERE dr_id='{0}';", query.dr_id);
                return _accessMySql.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestDao.UpdateDesignRequest-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 刪除
        public int DelDesignRequest(DesignRequestQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"DELETE FROM design_request WHERE dr_id IN ({0});",query.dr_ids);
                return _accessMySql.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestDao.DelDesignRequest-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 根據id查詢DesignRequest信息
        public DesignRequestQuery GetSingleDesignRequest(DesignRequestQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SELECT dr_type,dr_assign_to FROM design_request WHERE dr_id='{0}'", query.dr_id);
                return _accessMySql.getSinggleObj<DesignRequestQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestDao.GetSingleDesignRequest-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 獲取敏感詞彙
        public List<DisableKeywords> GetKeyWordsList()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SELECT dk_id,dk_string FROM disable_keywords where dk_active='0' ORDER BY dk_id DESC");
                return _accessMySql.getDataTableForObj<DisableKeywords>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestDao.GetKeyWordsList-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 獲取設計人員
        public DataTable GetDesign(ManageUserQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@"SELECT m.user_username,m.user_id from t_fgroup tf
LEFT JOIN t_groupcaller tg ON tf.rowid=tg.groupId
LEFT JOIN manage_user m ON tg.callid=m.user_email
LEFT JOIN t_parametersrc t ON  t.parameterCode=tf.rowid
WHERE  t.parameterType='design';");
                return _accessMySql.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestDao.GetDesign-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 根據product_id獲取name
        public DataTable GetPorductNameByProductId(int product_id)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SELECT product_name,product_detail_text FROM product WHERE product_id='{0}';", product_id);
                DataTable _dt=_accessMySql.getDataTable(sb.ToString());
                if (_dt.Rows.Count > 0)
                {
                    return _dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestDao.GetPorductNameByProductId-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 更新product表中的product_detail_text
        public int UpdateProductDetailText(DesignRequestQuery drt)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                DataTable _dt = GetPorductNameByProductId(Convert.ToInt32(drt.product_id));
                sb.AppendFormat(@"update product set product_detail_text='{0}'", drt.dr_content_text);
                if (string.IsNullOrEmpty(_dt.Rows[0]["product_detail_text"].ToString()))
                {//無內容的變更時間和建立人
                    sb.AppendFormat(",detail_createdate='{0}',detail_created='{1}' ", Common.CommonFunction.DateTimeToString(DateTime.Now), drt.dr_requester_id);
                }
                else 
                {
                    sb.AppendFormat(",detail_updatedate='{0}',detail_update='{1}' ", Common.CommonFunction.DateTimeToString(DateTime.Now), drt.dr_requester_id);
                }
                sb.AppendFormat(" where product_id='{0}';", drt.product_id);
                return  _accessMySql.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestDao.UpdateProductDetailText-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 狀態變更
        public int UpdStatus(DesignRequestQuery drt)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                 if (drt.dr_status > 1)
                 {
                     sb.AppendFormat(@"update design_request set dr_status='{0}' ", drt.dr_status);
                     if (drt.dr_assign_to > 0)
                     {
                         sb.AppendFormat(" ,dr_assign_to='{0}' ", drt.dr_assign_to);
                     }
                     if (drt.day > 0)
                     {
                         sb.AppendFormat(" ,dr_expected=DATE_ADD(NOW(),INTERVAL '{0}' DAY) ", drt.day);
                     }
                     if (drt.dr_id > 0)
                     {
                         sb.AppendFormat("  where dr_id in ({0})  ", drt.dr_id);
                     }
                     else
                     {
                         sb.AppendFormat("  where dr_id in ({0}) ", drt.dr_ids);
                     
                     }
                     return _accessMySql.execCommand(sb.ToString());
                 }
                 else
                 {
                     return 0;
                 }
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestDao.UpdStatus-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 是否是設計部主管
        public bool IsManager(DesignRequestQuery drt)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("SELECT parameterCode FROM t_parametersrc WHERE parameterType='design_manager';");
                DataTable _dt = _accessMySql.getDataTable(sb.ToString());
                if (_dt.Rows.Count > 0 && Convert.ToInt32(_dt.Rows[0][0]) == drt.login_id)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestDao.IsManager-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 是否是設計部人員
        public bool IsManagerNumber(DesignRequestQuery drt)
        {
            bool bo=false;
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@"SELECT m.user_id from t_fgroup tf
LEFT JOIN t_groupcaller tg ON tf.rowid=tg.groupId
LEFT JOIN manage_user m ON tg.callid=m.user_email
LEFT JOIN t_parametersrc t ON  t.parameterCode=tf.rowid
WHERE  t.parameterType='design';");
                DataTable _dt = _accessMySql.getDataTable(sb.ToString());
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    if (Convert.ToInt32(_dt.Rows[i][0]) == drt.login_id)
                    {
                        bo= true;
                    }
                }
                return bo;
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestDao.IsManager-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 預期天數
        public DataTable GetExpected(DesignRequestQuery drt)
        {
            DataTable _dt = new DataTable();
            StringBuilder sb = new StringBuilder();
            try
            {
                if (drt.dr_type > 0)
                {
                    sb.AppendFormat("SELECT remark FROM t_parametersrc WHERE parameterType='job_day' and parameterCode='{0}' ", drt.dr_type);
                    _dt = _accessMySql.getDataTable(sb.ToString());
                }
                return _dt;               
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestDao.GetExpected-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        //是否是檢查關鍵人人員
        public bool IsEmail(DesignRequestQuery drt)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@"SELECT user_id from manage_user where user_email IN (SELECT user_mail from mail_user where row_id IN (SELECT user_id from mail_group_map where group_id IN (SELECT row_id from mail_group WHERE group_code='Job')));");
                DataTable _dt = _accessMySql.getDataTable(sb.ToString());
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    if (Convert.ToInt32(_dt.Rows[i][0]) == drt.login_id)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestDao.IsEmail-->" + ex.Message + sb.ToString(), ex);
            }
        }
        public bool IsSelf(DesignRequestQuery drt)
        {
            bool bo = false;
            StringBuilder sb = new StringBuilder();
            try
            {//是否是自己申請的派工系統
                sb.AppendFormat("SELECT dr_requester_id from design_request WHERE dr_id in ({0}) ;", drt.dr_ids);
                DataTable _dt = _accessMySql.getDataTable(sb.ToString());
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    if (Convert.ToInt32(_dt.Rows[i][0]) == drt.login_id)
                    {
                        bo = true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return bo;                
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestDao.IsSelf-->" + ex.Message + sb.ToString(), ex);
            }
        }
        public bool IsDesSelf(DesignRequestQuery drt)
        {
            bool bo = false;
            StringBuilder sb = new StringBuilder();
            try
            {//是否是自己申請的派工系統
                sb.AppendFormat("SELECT dr_assign_to from design_request WHERE dr_id in ('{0}') ;", drt.dr_id);
                DataTable _dt = _accessMySql.getDataTable(sb.ToString());
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    if (Convert.ToInt32(_dt.Rows[i][0]) == drt.login_id)
                    {
                        bo = true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return bo;
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestDao.IsDesSelf-->" + ex.Message + sb.ToString(), ex);
            }
        }
        public int GetmailId(int id)
        {//根據指派人id找出mail_id
             StringBuilder sb = new StringBuilder();
            DataTable dt =new DataTable();
             try
             {
                 sb.AppendFormat(@"select row_id from mail_user  where user_mail in (SELECT user_email from manage_user where user_id='{0}');", id);
                 dt = _accessMySql.getDataTable(sb.ToString());
                 if (dt.Rows.Count > 0)
                 {
                     return int.Parse(dt.Rows[0][0].ToString());
                 }
                 else
                 {
                     return 0;
                 }
             }
             catch (Exception ex)
             {
                 throw new Exception("DesignRequestDao.GetmailId-->" + ex.Message + sb.ToString(), ex);
             }
        }
    }
}
