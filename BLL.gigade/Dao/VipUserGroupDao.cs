using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;
using BLL.gigade.Common;
using BLL.gigade.Model.Query;
using System.Data;
using System.Collections;

namespace BLL.gigade.Dao
{
    public class VipUserGroupDao : IVipUserGroupImplDao
    {

        private IDBAccess _accessMySql;
        private string connStr; 
        public VipUserGroupDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }

        public List<VipUserGroup> QueryAll(VipUserGroup query)
        {
            List<VipUserGroup> list = new List<VipUserGroup>();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select  group_id,group_name,domain,tax_id,image_name,gift_bonus,createdate,group_category,bonus_rate,bonus_expire_day,eng_name,check_iden from vip_user_group");
            return _accessMySql.getDataTableForObj<VipUserGroup>(sb.ToString());
        }
        #region 群組列表
        /// <summary>
        /// 獲取群組列表
        /// </summary>
        /// <param name="query">實體</param>
        /// <param name="totalCount">數據的總記錄數</param>
        /// <returns></returns>
        public List<VipUserGroupQuery> QueryAll(VipUserGroupQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();
            StringBuilder sqlstr = new StringBuilder();            
            try
            {
                sql.AppendLine(@"select group_id,group_name,domain,tax_id,gift_bonus,bonus_rate,bonus_expire_day,");
                sql.AppendLine(@"createdate,image_name,group_category,eng_name,check_iden,site_id ");
                sql.AppendLine(@"from vip_user_group where 1=1 ");
                sqlcount.AppendFormat(@"select count(group_id) as totalCount from vip_user_group where 1=1 ");
                //分頁
                //if (query.create_dateOne != 0)
                //{
                //    sqlstr.AppendFormat(" and createdate>={0}", query.create_dateOne);
                //}
                //if (query.create_dateTwo != 0)
                //{
                //    sqlstr.AppendFormat(" and createdate<={0}", query.create_dateTwo);
                //}
                if (query.group_id != 0)
                {
                    sqlstr.AppendFormat(" and group_id={0}", query.group_id);
                }
                if (!string.IsNullOrEmpty(query.group_name))
                {                  
                    sqlstr.AppendFormat(" and group_name like '%{0}%'", query.group_name);
                }
                totalCount = 0;
                if (query.IsPage)
                {
                    System.Data.DataTable _dt = _accessMySql.getDataTable(sqlcount.ToString() + sqlstr.ToString());

                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"].ToString());
                    }

                    sqlstr.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                return _accessMySql.getDataTableForObj<VipUserGroupQuery>(sql.ToString() + sqlstr.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao.QueryAll-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        #region 名單數
        /// <summary>
        /// 名單數
        /// </summary>
        /// <param name="query">根據group_id查詢</param>
        /// <returns></returns>
        public string GetVuserCount(VipUserGroup query)
        {
            string sql = string.Format("select count(v_id) as search_total from vip_user where 1=1 and group_id={0}", query.group_id);
            try
            {
                DataTable vuser = _accessMySql.getDataTable(sql);
                if (vuser.Rows.Count > 0)
                {
                    return vuser.Rows[0]["search_total"].ToString();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao.GetVuserCount-->" + ex.Message + sql, ex);
            }

        }
        #endregion
        #region  獲得一條群組數據
        /// <summary>
        /// 根據group_id獲得vip_user_group中的單條數據
        /// </summary>
        /// <param name="group_id"></param>
        /// <returns></returns>
        public VipUserGroup GetModelById(uint group_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"select group_id,group_name,domain,tax_id,image_name,gift_bonus,createdate,group_category,bonus_rate,bonus_expire_day,eng_name,check_iden,site_id from vip_user_group where 1=1 and group_id={0}", group_id);
                return _accessMySql.getSinggleObj<VipUserGroup>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao.GetModelById-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        #region 新增群組
        /// <summary>
        /// 向vip_user_group表中插入數據
        /// </summary>
        /// <param name="userGroup"></param>
        /// <returns></returns>
        public int Insert(VipUserGroup userGroup)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"insert into vip_user_group(group_id,group_name,tax_id,image_name,gift_bonus,group_category,eng_name,check_iden,createdate) ");
                sql.AppendFormat(@"values({0},'{1}','{2}',", userGroup.group_id, userGroup.group_name, userGroup.tax_id);
                sql.AppendFormat(@"'{0}',{1},{2},", userGroup.image_name, userGroup.gift_bonus, userGroup.group_category);
                sql.AppendFormat(@"'{0}',{1},{2} )", userGroup.eng_name, userGroup.check_iden, userGroup.createdate);
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao.Insert-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        #region 編輯群組
        /// <summary>
        /// 更新vip_user_group表中的數據
        /// </summary>
        /// <param name="userGroup"></param>
        /// <returns></returns>
        public int Update(VipUserGroup userGroup)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"update vip_user_group set group_name='{0}',tax_id='{1}',image_name='{2}',", userGroup.group_name, userGroup.tax_id, userGroup.image_name);
                sql.AppendFormat(@"gift_bonus={0},group_category={1},eng_name='{2}',", userGroup.gift_bonus, userGroup.group_category, userGroup.eng_name);
                sql.AppendFormat(@"check_iden={0} where 1=1 and group_id={1}", userGroup.check_iden, userGroup.group_id);
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao.Update-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        #region VIP群組名單列表
        /// <summary>
        ///根據group_id獲取群組的組員列表
        /// </summary>
        /// <param name="vu"></param>
        /// <param name="totalCount"></param>
        /// <param name="group_id"></param>
        /// <returns></returns>
        public List<VipUserQuery> GetVipUserList(VipUserQuery vu, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"select v.v_id,v.user_email as vuser_email,v.user_id ,v.status,v.group_id,emp_id, ");
                sql.AppendFormat(@"v.createdate,u.user_password,u.user_name,u.user_email,u.user_gender,");
                sql.AppendFormat(@"concat(u.user_birthday_year,'/',u.user_birthday_month,'/',u.user_birthday_day) as birthday,");
                sql.AppendFormat(@"u.user_mobile,u.user_phone,u.user_zip,u.user_address,u.user_reg_date,u.user_source,  ");
                sql.AppendFormat(@"case u.user_type WHEN  1 then '網絡會員'  when 2 then '電話會員' ELSE  '賣場(通路)代表會員' end as mytype ,u.send_sms_ad,u.paper_invoice,u.adm_note ");
                sql.AppendFormat(@" from vip_user v left join users u on v.user_id =u.user_id where 1=1 ");
                sql.AppendFormat(@" and v.group_id={0}", vu.group_id);
                totalCount = 0;
                try
                {
                    if (!string.IsNullOrEmpty(vu.serchtype))
                    {
                        if (vu.serchtype.ToString() == "0")
                        {
                            sql.Append(" ");
                        }
                        else if (vu.serchtype.ToString() == "1")
                        {
                            sql.AppendFormat(" and v.user_email like'%{0}%' ", vu.content.Trim());
                        }
                    }
                    else
                    {
                        sql.AppendFormat(" ");
                    }
                }
                catch
                {
                    sql.Append(" ");
                }

                if (vu.IsPage)
                {
                    System.Data.DataTable dt = _accessMySql.getDataTable(sql.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = dt.Rows.Count;
                    }
                    sql.AppendFormat(" limit {0},{1}", vu.Start, vu.Limit);

                }
                return _accessMySql.getDataTableForObj<VipUserQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao.GetVipUserList-->" + ex.Message + sql.ToString(), ex);
            }

        }
        #endregion
        #region 匯入郵箱
        #region 查詢user表中的數據
        /// <summary>
        /// 根據Email查詢users表,判斷郵箱是否有效
        /// </summary>
        /// <param name="sqlwhere"></param>
        /// <returns></returns>
        public DataTable GetUser(string sqlwhere)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"select user_id,user_email from users where 1=1 and user_email='{0}'", sqlwhere);
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao.GetUser-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        #region 判斷郵箱是否存在于此群組
        /// <summary>
        /// 通過group_id和user_email查詢vip_user表中是否存在這一條信息
        /// </summary>
        /// <param name="vu"></param>
        /// <returns></returns>
        public DataTable GetVipUser(VipUser vu)
        {
            string sql = string.Format("select * from vip_user where 1=1 and user_email='{0}' and group_id={1}", vu.user_email, vu.group_id);
            try
            {
                return _accessMySql.getDataTable(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao.GetVipUser-->" + ex.Message + sql, ex);
            }
        }
        #endregion
        #region 新增vip_user名單
        /// <summary>
        /// 向vip_user中插入數據
        /// </summary>
        /// <param name="vu"></param>
        /// <param name="sqlwhere"></param>
        /// <returns></returns>
        public int InsertVipUser(VipUser vu)
        {
            string sql = string.Format("insert into vip_user(user_email,group_id,user_id,createdate) values('{0}',{1},{2},{3}) ", vu.vuser_email, vu.group_id, vu.User_Id, vu.createdate);
            try
            {
                return _accessMySql.execCommand(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao.InsertVipUser-->" + ex.Message + sql, ex);
            }
        }
        #endregion
        #endregion
        #region 修改狀態
        /// <summary>
        /// 修改會員的狀態，啟用或者禁用
        /// </summary>
        /// <param name="state"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public int UpdateUserState(int state, int id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                if (id != 0)
                {
                    sql.AppendFormat("update vip_user set status ={0} where v_id={1};", state, id);
                }
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao.UpdateUserState-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        #region 匯入員工編號
        #region 查詢btob_emp表中信息
        public DataTable BtobEmp(string group_id)
        {

            string sql = string.Format("SELECT emp_id ,group_id FROM btob_emp WHERE group_id={0}", group_id);
            try
            {
                return _accessMySql.getDataTable(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao.BtobEmp-->" + ex.Message + sql, ex);
            }
        }
        #endregion
        #region 更新btob_emp表
        /// <summary>
        /// 對匯入數據進行比對,如果已經存在此數據,就進行更新
        /// </summary>
        /// <param name="group_id"></param>
        /// <param name="erp_id"></param>
        /// <param name="k">用於判斷採用哪個更新語句</param>
        /// <returns></returns>
        public int UpdateEmp(string group_id, string erp_id, int k)
        {
            string sql = string.Empty;
            //k
            try
            {
                if (k == 1)
                {
                    sql = string.Format("UPDATE btob_emp SET status=2 WHERE group_id={0}", group_id);
                }
                if (k == 2)
                {
                    sql = string.Format("UPDATE btob_emp SET status=1,update_date=UNIX_TIMESTAMP('{0}') WHERE emp_id='{1}' and group_id={2} ", CommonFunction.GetPHPTime(DateTime.Now.ToString()), erp_id, group_id);
                }
                return _accessMySql.execCommand(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao.UpdateEmp-->" + ex.Message + sql, ex);
            }

        }
        #endregion
        #region 向btob_emp新增數據
        public int InsertEmp(string group_id, string emp_id)
        {

            string sql = string.Format("insert into btob_emp (emp_id,group_id,status,create_date,update_date)");
            sql += string.Format(" values('{0}',{1},{2},UNIX_TIMESTAMP('{3}'),UNIX_TIMESTAMP('{4}'))", emp_id, group_id, 1, CommonFunction.GetPHPTime(DateTime.Now.ToString()), CommonFunction.GetPHPTime(DateTime.Now.ToString()));
            try
            {
                return _accessMySql.execCommand(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao.InsertEmp-->" + ex.Message + sql, ex);
            }
        }
        #endregion
        #endregion
        #region getvipuserGroup
        public List<VipUserGroup> GetVipUserByOrderId(uint user_id, uint group_id, uint status)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select vug.* from vip_user vu inner join vip_user_group vug on vug.group_id=vu.group_id and user_id={0}", user_id);

                if (group_id != 0)
                {
                    sql.AppendFormat(" AND vu.group_id={0} ", group_id);
                }
                if (status != 0)
                {
                    sql.AppendFormat(" AND vu.status={0} ", status);
                }

                return _accessMySql.getDataTableForObj<VipUserGroup>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao.GetVipUserByOrderId-->" + ex.Message + sql.ToString(), ex);
            }
        }

        #endregion

        #region 企業會員管理
        public bool execSql(ArrayList arrayList)
        {
            try
            {
                MySqlDao myDao = new MySqlDao(connStr);
                return myDao.ExcuteSqlsThrowException(arrayList);
            }
            catch (Exception ex)
            {
                throw new Exception(" EmsDao-->execSql--> " + arrayList + ex.Message, ex);
            }
        }
        #region 獲取郵遞store
        public List<ZipQuery> GetZipStore()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select zipcode, CONCAT(zipcode,middle,'  ',small) 'zipname' from t_zip_code;");
                return _accessMySql.getDataTableForObj<ZipQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao-->GetZipStore-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 第一步 新增
        public string InsertVipUserGroup(VipUserGroupQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.Append("insert into vip_user_group(group_id,group_name,domain,tax_id,group_code, ");
                sql.Append("group_capital,group_emp_number,group_emp_age,group_emp_gender,group_benefit_type, ");
                sql.Append("group_benefit_desc,group_subsidiary,group_hq_name,group_hq_code,group_committe_name,");
                sql.Append("group_committe_code,group_committe_promotion,");
                sql.Append("group_committe_desc, image_name,gift_bonus,createdate,group_category, ");
                sql.Append("bonus_rate,bonus_expire_day,eng_name,check_iden,site_id,");
                sql.Append("group_status,k_user,k_date,m_user,m_date) values( ");
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',",query.group_id,query.group_name,query.domain,query.tax_id,query.group_code);
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',",query.group_capital,query.group_emp_number,query.group_emp_age,query.group_emp_gender,query.group_benefit_type);
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',", query.group_benefit_desc, query.group_subsidiary, query.group_hq_name, query.group_hq_code, query.group_committe_name);
                sql.AppendFormat("'{0}','{1}',", query.group_committe_code, query.group_committe_promotion);
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',",query.group_committe_desc, query.image_name, query.gift_bonus, query.createdate, query.group_category);
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',", query.bonus_rate, query.bonus_expire_day, query.eng_name, query.check_iden, query.site_id);
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}');", query.group_status, query.k_user,CommonFunction.DateTimeToString(query.k_date), query.m_user,CommonFunction.DateTimeToString(query.m_date));
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao-->InsertVipUserGroup-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        #region 保存
        public string SaveVipUserGroup(VipUserGroupQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.AppendFormat("set sql_safe_updates=0;update vip_user_group set group_name='{0}',eng_name='{1}',tax_id='{2}',group_code='{3}',group_capital='{4}',check_iden='{5}',", query.group_name, query.eng_name, query.tax_id, query.group_code, query.group_capital,query.check_iden);
                sql.AppendFormat("group_emp_number='{0}',group_emp_age='{1}',group_emp_gender='{2}',group_benefit_type='{3}',group_benefit_desc='{4}',", query.group_emp_number, query.group_emp_age, query.group_emp_gender, query.group_benefit_type, query.group_benefit_desc);
                sql.AppendFormat("group_subsidiary='{0}',group_hq_name='{1}',group_hq_code='{2}',", query.group_subsidiary, query.group_hq_name, query.group_hq_code);
                sql.AppendFormat("group_committe_name='{0}',group_committe_code='{1}',",query.group_committe_name,query.group_committe_code);
                sql.AppendFormat("group_committe_promotion='{0}',group_committe_desc='{1}',m_user='{2}',m_date='{3}',file_name='{4}' where group_id='{5}';set sql_safe_updates=1;", query.group_committe_promotion, query.group_committe_desc, query.m_user, CommonFunction.DateTimeToString(DateTime.Now), query.file_name,query.group_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao-->SaveVipUserGroup-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        #region 第四個面板的grid
        public List<DeliveryAddress> GetComAddress(DeliveryAddress query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            try
            {
                sql.Append("select da.da_id,da.user_id,da.da_title,CONCAT(da.da_dist,tzc.middle,'  ',tzc.small) as 'da_dist',da.da_address ");
                sqlFrom.Append(" from delivery_address da left join t_zip_code tzc on da.da_dist=tzc.zipcode ");
                sqlWhere.Append(" where 1= 1 ");
                if (query.user_id != 0)
                {
                    sqlWhere.AppendFormat(" and da.user_id='{0}';", query.user_id);
                }
                sql.Append(sqlFrom.ToString()+sqlWhere.ToString());
                return _accessMySql.getDataTableForObj<DeliveryAddress>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao-->GetComAddress-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public int SaveComAddress(DeliveryAddress query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.Append("insert into delivery_address(user_id,da_title,da_name,da_gender,da_mobile_no,");
                sql.Append("da_tel_no,da_dist,da_address,da_default,da_created) values(");



                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',",query.user_id,query.da_title,query.da_name,query.da_gender,query.da_mobile_no);
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}');",query.da_tel_no,query.da_dist,query.da_address,query.da_default,CommonFunction.DateTimeToString( query.da_created));
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao-->SaveComAddress-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public int UPComAddress(DeliveryAddress query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.AppendFormat("update  delivery_address  set da_address='{0}' ,da_dist='{1}' ,da_title='{2}' where da_id='{3}';", query.da_address, query.da_dist,query.da_title, query.da_id);
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao-->SaveComAddress-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        #region 列表頁
        public List<VipUserGroupQuery> GetVipUserGList(VipUserGroupQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            query.Replace4MySQL();
            totalCount = 0;
            try
            {
                sql.Append(" select  vug.group_id,  vug.group_name,  vug.domain ,   vug.tax_id ,vug.group_code,vug.group_capital ,vug.group_emp_number ,vug.group_emp_age  ,vug.group_emp_gender ,vug.group_benefit_type,vug.group_benefit_desc  ,vug.group_subsidiary ,vug.group_hq_name ,vug.group_hq_code ,vug.group_committe_name,vug.group_committe_code ,vug.group_committe_promotion,vug.group_committe_desc ,vug.image_name ,vug.gift_bonus  ,vug.createdate ,vug.group_category,vug.bonus_rate  ,vug.bonus_expire_day ,vug.eng_name  ,vug.check_iden  ,vug.site_id,vug.group_status  ,vug.file_name,vug.k_user ,vug.k_date ,vug.m_user  ,vug.m_date,mu1.user_username as 'create_user',mu2.user_username as 'update_user' ");

                sqlFrom.Append("  from vip_user_group  vug  ");
                sqlFrom.Append(" LEFT JOIN manage_user mu1 on vug.k_user=mu1.user_id  ");
                sqlFrom.Append(" LEFT JOIN manage_user mu2 on vug.m_user=mu2.user_id  ");
                sqlWhere.Append(" where 1=1 ");
                if (query.group_id != 0)
                {
                    sqlWhere.AppendFormat(" and vug.group_id='{0}'  ", query.group_id);
                }
                sqlWhere.Append(" and vug.group_committe_promotion !=''  ");
                if (query.tax_name != "")
                {
                    sqlWhere.AppendFormat(" and (vug.group_name  like '%{0}%' or group_code like '%{0}%')  ",query.tax_name);
                }
                if (query.group_id != 0)
                {
                    sqlWhere.AppendFormat(" and vug.group_id='{0}'  ", query.group_id);
                }
                if (query.IsPage)
                {
                    sqlCount.Append(" select count(vug.group_id) as 'totalCount'  " + sqlFrom.ToString() + sqlWhere.ToString());
                    DataTable _dt = _accessMySql.getDataTable(sqlCount.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0][0]);
                    }
                }
                sqlWhere.AppendFormat(" order by vug.group_id desc limit {0},{1};  ", query.Start, query.Limit);
                sql.Append(sqlFrom.ToString() + sqlWhere.ToString());
                return _accessMySql.getDataTableForObj<VipUserGroupQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao-->GetVipUserGList-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion
        public int DeleteDeliveryAddress(DeliveryAddress query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("delete from delivery_address where da_id='{0}';", query.da_id);
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao-->DeleteDeliveryAddress-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public int UpVUGStatus(VipUserGroupQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates=0;update vip_user_group set group_status='{0}',m_user='{1}',m_date='{2}'  where group_id='{3}';set sql_safe_updates=1;", query.group_status,query.m_user,CommonFunction.DateTimeToString(query.m_date), query.group_id);
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao-->UpVUGStatus-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public DataTable IsGroupIdStatus(VipUserGroupQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select group_id from promotions_amount_discount where `end`>='{0}' and active='1' and group_id='{1}';",CommonFunction.DateTimeToString(DateTime.Now),query.group_id);
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao-->IsGroupIdStatus-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public string GetInsertBtobSql(BtobEmp query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.AppendFormat("insert into btob_emp (`emp_id`, `group_id`, `status`, `create_date`, `update_date`) values ('{0}','{1}','{2}','{3}','{4}');", query.emp_id, query.group_id, query.status, CommonFunction.GetPHPTime(), CommonFunction.GetPHPTime());
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao->GetInsertBtobSql->" + sql.ToString() + ex.Message, ex);
            }
        }

        public string GetUpdateBtobSql(BtobEmp query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.AppendFormat("update btob_emp set update_date='{0}' where id='{1}';", CommonFunction.GetPHPTime(),query.id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao->GetUpdateBtobSql->" + sql.ToString() + ex.Message, ex);
            }
        }

        public int GetUserId(string user_email)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                int result = 0;
                sql.AppendFormat("select user_id from users where user_email='{0}';", user_email);
                DataTable _dt = _accessMySql.getDataTable(sql.ToString());
                if (_dt.Rows.Count > 0)
                {
                    result = Convert.ToInt32(_dt.Rows[0]["user_id"]);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao->GetUserId->" + sql.ToString() + ex.Message, ex);
            }
        }

        //public int ExistEmail(VipUserGroupQuery query)
        //{
        //    StringBuilder sql = new StringBuilder();
        //    try
        //    {
        //        query.Replace4MySQL();
        //        sql.AppendFormat("select group_committe_mail from vip_user_group where group_committe_mail='{0}' and group_committe_chairman !=''  ;", query.group_committe_mail);
        //        DataTable _dt = _accessMySql.getDataTable(sql.ToString());
        //        if (_dt != null && _dt.Rows.Count > 0)
        //        {
        //            return 1;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("VipUserGroupDao->ExistEmail->" + sql.ToString() + ex.Message, ex);
        //    }
        //}

        public int VIPUserExistEmail(VipUser query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.AppendFormat("select user_email from vip_user where group_id='{0}' and user_email ='{1}';", query.group_id,query.user_email);
                DataTable _dt = _accessMySql.getDataTable(sql.ToString());
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao->VIPUserExistEmail->" + sql.ToString() + ex.Message, ex);
            }
        }

        public string GetZiPCode(string code)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" select t_code.zipcode from(select zipcode, CONCAT(zipcode,middle,'  ',small) 'zipname' from t_zip_code) t_code where t_code.zipname='{0}';", code);
                DataTable _dt = _accessMySql.getDataTable(sql.ToString());
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    return _dt.Rows[0][0].ToString();
                }
                else
                {
                    return "100";
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao->GetZiPCode->" + sql.ToString() + ex.Message, ex);
            }
        }

        public int GetIdFromBtob(BtobEmp query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select id from btob_emp where emp_id='{0}' and  group_id='{1}';",query.emp_id,query.group_id);
                DataTable _dt = _accessMySql.getDataTable(sql.ToString());
                if (_dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(_dt.Rows[0][0]);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao->GetIdFromBtob->" + sql.ToString() + ex.Message, ex);
            }
        }

        public bool IsEmpIdExist(BtobEmp query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select emp_id from btob_emp where emp_id='{0}';", query.emp_id);
                DataTable _dt = _accessMySql.getDataTable(sql.ToString());
                if (_dt.Rows.Count > 0)
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
                throw new Exception("VipUserGroupDao->IsEmpIdExist->" + sql.ToString() + ex.Message, ex);
            }
        }

        public int ExisGroupCode(VipUserGroupQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.AppendFormat("select group_code from vip_user_group where group_code='{0}'  and group_committe_promotion !='' ;", query.group_code);
                DataTable _dt = _accessMySql.getDataTable(sql.ToString());
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao->ExisTaxId->" + sql.ToString() + ex.Message, ex);
            }
        }

        public int ExisTaxId(VipUserGroupQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.AppendFormat("select tax_id from vip_user_group where tax_id='{0}'  and group_committe_promotion !=''  ", query.tax_id);
                if (query.group_id != 0)
                {
                    sql.AppendFormat(" and  group_id!='{0}';",query.group_id);
                }
              
                DataTable _dt = _accessMySql.getDataTable(sql.ToString());
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao->ExisTaxId->" + sql.ToString() + ex.Message, ex);
            }
        }
        
        
        #endregion



    }
}

