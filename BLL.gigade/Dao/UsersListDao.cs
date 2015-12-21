/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：UsersListDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：dongya0410j 
 * 完成日期：2014/09/22 13:35:21 
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Common;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Dao
{
    public class UsersListDao : IUsersListImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        private string conn = string.Empty;
        public UsersListDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            conn = connectionString;
        }
        #region 會員信息查詢+List<Model.Query.UsersListQuery> Query(Model.Query.UsersListQuery store, out int totalCount)
        public List<Model.Query.UsersListQuery> Query(Model.Query.UsersListQuery store, out int totalCount)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                StringBuilder sql = new StringBuilder();
                StringBuilder sqlcount = new StringBuilder();
                StringBuilder sqlfrom = new StringBuilder();
                //  user_id,user_name,user_email,user_mobile,user_status,user_reg_date,user_company_id,user_source,source_trace,first_time,last_time,be4_last_time
                sql.Append(@"SELECT redirect_url,redirect_name, users.user_id,user_email,user_new_email,user_status,user_password,user_newpasswd,user_name,");
                sql.Append(@" user_gender,user_birthday_year,user_birthday_month,user_birthday_day,user_mobile,user_phone,user_zip,user_address,user_login_attempts,");
                sql.Append(@"user_actkey,user_reg_date,user_updatedate,user_old_password,user_company_id,user_source,user_fb_id,user_country,user_ref_user_id,user_province,");
                sql.Append(@"user_city,source_trace , CASE user_type  when '1' THEN '網路會員' else'電話會員' END as mytype ,send_sms_ad,adm_note ,");
                sql.Append(@"concat(user_birthday_year,'/',user_birthday_month,'/',user_birthday_day) as birthday,user_level,ml_code,first_time,last_time,be4_last_time,paper_invoice ");
                sqlcount.Append("select count(users.user_id) as totalcounts ");
                sqlfrom.Append(" FROM users left join redirect on users.source_trace = redirect.redirect_id  ");
                sqlfrom.Append(" WHERE 1=1 ");

                try
                {
                    if (store.serchstart != DateTime.MinValue)
                    {
                        sqlfrom.AppendFormat(" and user_reg_date >= '{0}'", CommonFunction.GetPHPTime(store.serchstart.ToString()));
                    }
                    if (store.serchend.ToShortDateString() != DateTime.MinValue.ToShortDateString())
                    {
                        sqlfrom.AppendFormat(" AND user_reg_date <= '{0}'", CommonFunction.GetPHPTime(store.serchend.ToString()));
                    }


                    if (!string.IsNullOrEmpty(store.serchtype))
                    {
                        if (store.serchtype.ToString() == "0")
                        {
                            sqlfrom.Append(" ");
                        }
                        else if (store.serchtype.ToString() == "1")
                        {
                            sqlfrom.AppendFormat(" and user_email like '%{0}%' ", store.content.Trim());
                        }
                        else if (store.serchtype.ToString() == "2")
                        {
                            sqlfrom.AppendFormat(" and user_name like '%{0}%' ", store.content.Trim());

                        }
                        else if (store.serchtype.ToString() == "3")
                        {
                            store.content = EncryptComputer.EncryptDecryptTextByApi(store.content.Trim());
                            sqlfrom.AppendFormat(" and user_mobile like '%{0}%' ", store.content.Trim());
                        }
                        else if (store.serchtype.ToString() == "4")
                        {
                            sqlfrom.AppendFormat(" and users.user_id like '%{0}%' ", store.content.Trim());
                        }
                        //else if (store.serchtype.ToString() == "5")
                        //{
                        //    sqlfrom.AppendFormat(" and user_phone like '%{0}%' ", store.content.Trim());
                        //}
                        else if (store.serchtype.ToString() == "6")
                        {
                            sqlfrom.AppendFormat(" and user_address like '%{0}%' ", store.content.Trim());
                        }

                        //Edit Start 
                        //Add by yuwei1015j 2015-12-02
                        else if (store.serchtype.ToString() == "7")
                        {
                            sqlfrom.AppendFormat(" and ml_code like '%{0}%' ", store.content.Trim());
                        }
                        //Edit End


                    }
                    else
                    {
                        sqlfrom.AppendFormat(" ");
                    }
                    if (store.user_id != 0)
                    {
                        sqlfrom.AppendFormat(" and users.user_id = '{0}' ", store.user_id);
                    }
                }
                catch
                {
                    sqlfrom.Append(" ");
                }
                try
                {
                    if (!string.IsNullOrEmpty(store.types.ToString()))
                    {
                        sqlfrom.AppendFormat(" and user_status='{0}' ", store.types);
                    }
                    else
                    {
                        sqlfrom.AppendFormat(" ");
                    }
                }
                catch
                {
                    sqlfrom.Append(" ");
                }


                try
                {
                    if (store.checks.ToString() == "true")
                    {
                        sqlfrom.AppendFormat(" and (user_type=2 or adm_note LIKE '%更改為網路會員') ");
                    }
                    else
                    {
                        sqlfrom.AppendFormat(" ");
                    }
                }
                catch
                {
                    sqlfrom.Append(" ");
                }

                sqlfrom.AppendFormat(" ORDER BY users.user_id DESC ");
                totalCount = 0;
                if (store.IsPage)
                {
                    sb.Append(sqlcount.ToString() + sqlfrom.ToString() + ";");
                    System.Data.DataTable _dt = _access.getDataTable(sqlcount.ToString() + sqlfrom.ToString());

                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalcounts"]);
                    }

                    sqlfrom.AppendFormat(" limit {0},{1}", store.Start, store.Limit);
                }
                sb.Append(sql.ToString() + sqlfrom.ToString());
                return _access.getDataTableForObj<UsersListQuery>(sql.ToString() + sqlfrom.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UsersListDao-->Query-->" + ex.Message + sb.ToString(), ex);
            }
        }

        #endregion

        #region 保存用戶列表+int SaveUserList(UsersListQuery usr)
        public string SaveUserList(UsersListQuery usr)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                usr.Replace4MySQL();
                sb.AppendFormat(@"update users  set user_name='{0}',user_password='{1}',user_gender='{2}',
                                                       user_birthday_year='{3}',user_birthday_month='{4}',user_birthday_day='{5}',user_zip='{6}',
                                                       user_address='{7}',user_phone='{8}',user_mobile='{9}',send_sms_ad='{10}',adm_note='{11}' ,paper_invoice='{12}',
                                                       user_mobile_bak='{13}'   where user_id='{14}';",
                                                          usr.user_name, usr.user_password, usr.user_gender,
                                                          usr.user_birthday_year, usr.user_birthday_month, usr.user_birthday_day, usr.user_zip,
                                                          usr.user_address, usr.user_phone, usr.user_mobile, Convert.ToInt32(usr.send_sms_ad), usr.adm_note, Convert.ToInt32(usr.paper_invoice),
                                                          usr.user_mobile, usr.user_id);

                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("UsersListDao-->SaveUserList-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 獲取到model+Model.Custom.Users getModel(int id)
        public Model.Custom.Users getModel(int id)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"select * from users where user_id={0}", id);
                return _access.getSinggleObj<BLL.gigade.Model.Custom.Users>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UsersListDao-->getModel-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 獲取購物金列表+List<BonusMasterQuery> bQuery(BonusMasterQuery store, out int totalCount)
        public List<BonusMasterQuery> bQuery(BonusMasterQuery query, out int totalCount)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                StringBuilder sql = new StringBuilder();
                StringBuilder sqlcount = new StringBuilder();
                StringBuilder sqlfrom = new StringBuilder();
                StringBuilder sqlStatus = new StringBuilder();
                sql.Append(@"SELECT	bm.master_id,bm.user_id,bm.master_total,bm.master_balance,bm.master_start,bm.master_end,bm.master_createdate,bm.bonus_type,bm.master_note,us.user_name,us.user_email,bt.type_admin_link,bt.type_description");
                sqlcount.Append("SELECT	count(bm.master_id) AS search_total ");
                sqlfrom.Append(" FROM bonus_master bm left join bonus_type bt on bm.type_id=bt.type_id left join users us on bm.user_id=us.user_id  where 1=1 ");
                if (query.master_id != 0)
                {
                    sqlfrom.AppendFormat(" and  bm.master_id='{0}' ", query.master_id);
                }
                if (query.user_id != 0)
                {
                    sqlfrom.AppendFormat(" and  bm.user_id='{0}' ", query.user_id);
                }
                if (query.user_email != string.Empty)
                {
                    sqlfrom.AppendFormat(" and (us.user_email like '%{0}%' or us.user_name like '%{0}%' escape '/')", query.user_email);
                }
                //if (query.user_name != string.Empty)
                //{
                //    sqlfrom.AppendFormat(" and u.user_name like '%{0}%'ESCAPE'/'", query.user_name);
                //}
                if (query.smaster_start != DateTime.MinValue && query.smaster_end != DateTime.MinValue)
                {
                    sqlfrom.AppendFormat(" and bm.master_createdate between {0} and {1}", CommonFunction.GetPHPTime(query.smaster_start.ToString()), CommonFunction.GetPHPTime(query.smaster_end.ToString()));
                }
                if (query.bonus_type != 0)
                {
                    sqlfrom.AppendFormat(" and bm.bonus_type={0}", query.bonus_type);
                }
                if (query.type_id != 0)
                {
                    sqlfrom.AppendFormat(" and bm.type_id={0}", query.type_id);
                }
                if (query.use || query.useing || query.used || query.useings || query.useds)
                {
                    sqlStatus.Append(" and (");
                    if (query.use)
                    {
                        sqlStatus.AppendFormat("bm.master_start>'{0}'", CommonFunction.GetPHPTime());
                    }
                    if (query.useing)
                    {
                        sqlStatus.AppendFormat(" or (bm.master_balance>=bm.master_total and '{0}' between bm.master_start and bm.master_end)", CommonFunction.GetPHPTime());
                    }
                    if (query.used)
                    {
                        sqlStatus.AppendFormat(" or (bm.master_end<'{0}' and bm.master_balance>0)", CommonFunction.GetPHPTime());
                    }
                    if (query.useings)
                    {//不包含未開始的，也不包含過期的
                        sqlStatus.AppendFormat(" or (0<bm.master_balance and bm.master_balance<bm.master_total  and '{0}' between bm.master_start and bm.master_end)", CommonFunction.GetPHPTime());
                    }
                    if (query.useds)
                    {
                        sqlStatus.Append(" or bm.master_balance=0");
                    }
                    sqlStatus.Append(")");
                    sqlStatus.Replace("( or", "(");
                }
                sqlfrom.Append(sqlStatus);
                sqlfrom.AppendFormat(" order by master_createdate DESC,master_balance DESC , master_end DESC, master_start DESC ");
                totalCount = 0;
                if (query.IsPage)
                {
                    sb.Append(sqlcount.ToString() + sqlfrom.ToString() + ";");
                    System.Data.DataTable _dt = _access.getDataTable(sqlcount.ToString() + sqlfrom.ToString());

                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["search_total"]);
                    }

                    sqlfrom.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                sb.Append(sql.ToString() + sqlfrom.ToString());
                return _access.getDataTableForObj<BonusMasterQuery>(sql.ToString() + sqlfrom.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UsersListDao-->bQuery-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 更新users表+int UpdateUser(Model.Custom.Users usr)
        public int UpdateUser(Model.Custom.Users usr)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                usr.Replace4MySQL();
                strSql = string.Format(@"update users  set user_actkey='{0}' where user_id='{1}'", usr.user_actkey, usr.user_id);
                sb.Append(strSql.ToString());
                return _access.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UsersListDao-->UpdateUser-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 更新user_master表+int updateuser_master(BonusMasterQuery store)
        public int updateuser_master(BonusMasterQuery store)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                store.Replace4MySQL();
                strSql = string.Format(@"update bonus_master set master_total='{0}',master_balance='{1}', master_start='{2}',master_end='{3}',master_note='{4}' where user_id='{5}' and master_id='{6}'", store.master_total, store.master_balance, store.master_start, store.master_end, store.master_note, store.user_id, store.master_id);
                sb.Append(strSql.ToString());
                return _access.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UsersListDao-->updateuser_master-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        public List<UsersListQuery> Export(UsersListQuery store)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            try
            {
                sql.Append(@"SELECT users_login.login_ipfrom as master_ipfrom,redirect_name,group_name,users.user_id,users.user_name
                           ,user_gender,user_reg_date,
                           user_company_id,user_source ,concat(user_birthday_year,'/',user_birthday_month,'/',user_birthday_day) as birthday,ml_code,
                           first_time,last_time,be4_last_time,bonus_master.master_total as master_total,bonus_master.master_balance as bonus_master");
                sqlfrom.Append(" FROM users left join redirect on users.source_trace = redirect.redirect_id  ");
                sqlfrom.Append(@" left join bonus_master on users.user_id =bonus_master.user_id ");
                sqlfrom.Append(@" LEFT JOIN users_login on users_login.user_id=bonus_master.user_id ");
                sqlfrom.Append(@" LEFT JOIN redirect_group on redirect_group.group_id=redirect.group_id  WHERE 1=1  ");

                if (store.serchstart != DateTime.MinValue)
                {
                    sqlfrom.AppendFormat(" and user_reg_date >= '{0}'", CommonFunction.GetPHPTime(store.serchstart.ToString()));
                }
                if (store.serchend.ToShortDateString() != DateTime.MinValue.ToShortDateString())
                {
                    sqlfrom.AppendFormat(" AND user_reg_date <= '{0}'", CommonFunction.GetPHPTime(store.serchend.ToString()));
                }
                if (!string.IsNullOrEmpty(store.serchtype))
                {
                    if (store.serchtype.ToString() == "0")
                    {
                        sqlfrom.Append(" ");
                    }
                    else if (store.serchtype.ToString() == "1")
                    {
                        sqlfrom.AppendFormat(" and user_email like '%{0}%' ", store.content.Trim());
                    }
                    else if (store.serchtype.ToString() == "2")
                    {
                        sqlfrom.AppendFormat(" and user_name like '%{0}%' ", store.content.Trim());

                    }
                    else if (store.serchtype.ToString() == "3")
                    {
                        store.content = EncryptComputer.EncryptDecryptTextByApi(store.content.Trim());
                        sqlfrom.AppendFormat(" and user_mobile like '%{0}%' ", store.content.Trim());
                    }
                    else if (store.serchtype.ToString() == "4")
                    {
                        sqlfrom.AppendFormat(" and users.user_id like '%{0}%' ", store.content.Trim());
                    }
                    else if (store.serchtype.ToString() == "5")
                    {
                        sqlfrom.AppendFormat(" and user_phone like '%{0}%' ", store.content.Trim());
                    }
                    else if (store.serchtype.ToString() == "6")
                    {
                        sqlfrom.AppendFormat(" and user_address like '%{0}%' ", store.content.Trim());
                    }
                    //Edit Start 
                    //Add by yuwei1015j 2015-12-02
                    else if (store.serchtype.ToString() == "7")
                    {
                        sqlfrom.AppendFormat(" and ml_code like '%{0}%' ", store.content.Trim());
                    }
                    //Edit End
                }
                if (!string.IsNullOrEmpty(store.types.ToString()))
                {
                    sqlfrom.AppendFormat(" and user_status='{0}' ", store.types);
                }

                if (store.checks.ToString() == "true")
                {
                    sqlfrom.AppendFormat(" and (user_type=2 or adm_note LIKE '%更改為網路會員') ");
                }

                sqlfrom.AppendFormat(" GROUP BY users.user_id  ORDER BY users.user_id DESC ");
                return _access.getDataTableForObj<UsersListQuery>(sql.ToString() + sqlfrom.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UsersListDao-->Query-->" + ex.Message + sql.ToString() + sqlfrom.ToString(), ex);
            }
        }

        public int UserCancel(UsersListQuery u)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sel = new StringBuilder();
            string email = DateTime.Now.ToString("yyyyMMdd") + "_" + u.user_email;
            try
            {
                if (u.user_id > 0)
                {
                    sb.Append("set sql_safe_updates = 0;");
                    sb.AppendFormat("Update users set user_status=2,user_email='{1}' where user_id='{0}';", u.user_id, email);
                    sel.AppendFormat("SELECT * from edm_email where email_address='{0}' ;", u.user_email);
                    if (_access.getDataTable(sel.ToString()).Rows.Count > 0)
                    {//修改會員電子報信息
                        sb.AppendFormat("Update edm_email set email_updatedate='{2}' , email_address='{1}' where email_address='{0}';", u.user_email, email, CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                    }
                    sel.Clear();
                    sel.AppendFormat("SELECT * from epaper_email where email_address='{0}'", u.user_email);
                    if (_access.getDataTable(sel.ToString()).Rows.Count > 0)
                    {//修改會員電子報信息
                        sb.AppendFormat("Update epaper_email set email_updatedate='{2}', email_address='{1}' where email_address='{0}';", u.user_email, email, CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                    }
                    sb.Append("set sql_safe_updates = 1; ");
                    //保存禁用會員的時間和操作人至會員生活表（user_life）edit by shuangshuang0420j 20150814 09:42
                    UserLifeDao _userLifeDao = new UserLifeDao(conn);
                    sb.Append(_userLifeDao.UpdateDisableTime(u.user_id, (uint)CommonFunction.GetPHPTime(), u.update_user));

                    return _access.execCommand(sb.ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("UsersListDao-->UserCancel-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #region ///汇出会员查询列表所需要的几个列
        ///chaojie_zz添加于2014/10/08 
        ///功能：后台=》会员管理=》会员查询=》汇出csv档案
        ///返回会员查询列表中的购物金使用和购物金发放。
        ///关联于MemberController汇出的CSV档案
        public DataTable GetBonusTotal(DateTime timestart, DateTime timeend, string user_id)/*购物金发放*/
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendFormat(@"select sum(master_total) as total_in,user_id  from bonus_master where user_id in(");
                if (timeend != DateTime.MinValue && timeend != DateTime.MinValue)
                {
                    strSql.AppendFormat(" {0} )  AND master_createdate >='{1}'AND master_createdate <='{2}' ", user_id, CommonFunction.GetPHPTime(timestart.ToString()), CommonFunction.GetPHPTime(timeend.ToString()));
                }
                else
                {
                    strSql.AppendFormat(" {0} )", user_id);
                }
                strSql.AppendFormat("  GROUP BY user_id");

                return _access.getDataTable(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UsersListDao-->GetBonusTotal-->" + ex.Message + strSql.ToString(), ex);
            }
        }

        public DataTable GetRecordTotal(DateTime timestart, DateTime timeend, string user_id)/*购物金使用*/
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendFormat(@"select sum(record_use) as total_out ,user_id from bonus_record INNER JOIN order_master using(order_id) where user_id in (");
                if (timeend != DateTime.MinValue && timeend != DateTime.MinValue)
                {
                    strSql.AppendFormat(" {0} )  AND record_createdate >='{1}'AND record_createdate <='{2}' ", user_id, CommonFunction.GetPHPTime(timestart.ToString()), CommonFunction.GetPHPTime(timeend.ToString()));
                }
                else
                {
                    strSql.AppendFormat(" {0} )", user_id);
                }
                strSql.AppendFormat("  GROUP BY user_id");

                return _access.getDataTable(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UsersListDao-->GetRecordTotal-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        public DataTable GetZipCode()/*查询所有地址*/
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendFormat(@"select *from t_zip_code");


                return _access.getDataTable(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UsersListDao-->GetZipCode-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion
    }
}
