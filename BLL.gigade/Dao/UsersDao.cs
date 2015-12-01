using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao
{
    class UsersDao : IUsersImplDao
    {
        private IDBAccess _accessMySql;
        string strSql = string.Empty;
        private string connStr;
        UserHistoryDao _userhistoryDao = new UserHistoryDao("");
        SmsDao _smsdao = new SmsDao("");
        SerialDao _serialDao = new SerialDao("");
        public UsersDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connStr = connectionString;
        }

        #region IUsersImplDao 成员

        public DataTable Query(string strUserEmail)
        {
            strSql = string.Format("select user_id from users where user_email = '{0}'", strUserEmail);

            return _accessMySql.getDataTable(strSql);
        }

        public List<Users> Query(Users query)
        {
            try
            {
                StringBuilder sql = new StringBuilder("select user_id,user_email,user_name,user_password,user_mobile,user_phone,user_zip,user_address,user_status,");
                sql.Append("user_reg_date,user_updatedate,user_birthday_year,user_birthday_month,user_birthday_day,send_sms_ad from users where 1=1");
                if (query.user_id != 0)
                {
                    sql.AppendFormat(" and user_id={0}", query.user_id);
                }
                return _accessMySql.getDataTableForObj<Users>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UsersDao.Query(Users query)-->" + ex.Message, ex);
            }
        }

        public int SelSaveID(Users u)
        {
            u.Replace4MySQL();
            strSql = string.Format(@"insert into users (user_id,user_email,user_name,user_password,user_mobile,user_phone,user_zip,user_address,user_status,user_reg_date,user_updatedate,
            user_birthday_year,user_birthday_month,user_birthday_day,user_mobile_bak) 
            values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}')",
                u.user_id, u.user_email, u.user_name, u.user_password, u.user_mobile, u.user_phone, u.user_zip, u.user_address, u.user_status, u.user_reg_date, u.user_updatedate,
            u.user_birthday_year, u.user_birthday_month, u.user_birthday_day,u.user_mobile);

            return _accessMySql.execCommand(strSql);
        }

        public string Save(UserQuery uquery)
        {

            uquery.Replace4MySQL();


            StringBuilder sql = new StringBuilder();
            sql.Append("insert into users (user_id,user_email,user_new_email,user_password,user_newpasswd,user_name,user_mobile,user_zip,user_address,user_type, user_birthday_year,user_birthday_month,user_birthday_day,send_sms_ad,adm_note,user_status,user_source,user_login_attempts,user_actkey,user_reg_date,user_updatedate,user_mobile_bak)");
            sql.AppendFormat(" values({0},'{1}','{2}','{3}','{4}','{5}','{6}',{7},'{8}',{9},{10},{11},", uquery.user_id, uquery.user_email, uquery.user_new_email, uquery.user_password, uquery.user_newpasswd, uquery.user_name, uquery.user_mobile, uquery.user_zip, uquery.user_address, uquery.user_type,
                uquery.user_birthday_year, uquery.user_birthday_month);
            sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}');", uquery.user_birthday_day, uquery.send_sms_ad == false ? 0 : 1, uquery.adm_note, uquery.user_status, uquery.user_source, uquery.user_login_attempts, uquery.user_actkey, uquery.user_reg_date, uquery.user_updatedate,uquery.user_mobile);
            return sql.ToString();
        }

        #region 獲得會員購買記錄排行 
        //create by zhaozhi0623j 20151016 18:01
        #region 會員消費記錄排行排序GetViplistUserId(Model.Query.UserVipListQuery uvlq)
        /// <summary>
        /// 會員消費記錄排行排序 查詢滿足條件的id，並根據消費總額sum_amount排序
        /// </summary>
        public string GetViplistUserId(Model.Query.UserVipListQuery uvlq)
        {
            StringBuilder userId = new StringBuilder();
            string userIdString = string.Empty;
            string queryUserID = string.Empty;
            string timelimit = string.Empty;
            //添加搜索條件
            
            try
            {
                if (uvlq.create_dateOne != 0)
                {
                    timelimit += string.Format(" and om.order_createdate >='{0}' ", uvlq.create_dateOne);
                }
                if (uvlq.create_dateTwo != 0)
                {
                    timelimit += string.Format(" and om.order_createdate <='{0}' ", uvlq.create_dateTwo);
                }
                if (uvlq.user_id != 0)
                {
                    queryUserID = Convert.ToString(uvlq.user_id);
                    return queryUserID;
                }
                StringBuilder sbuser = new StringBuilder();
                DataTable sbuserDT = new DataTable();
                sbuser.Append(" select u.user_id ");               
                sbuser.Append(" from order_master om inner join users u on u.user_id = om.user_id ");
                sbuser.Append(" where om.order_status = 99 ");
                sbuser.Append(timelimit);
                sbuser.Append("  group by om.user_id ");

                if (uvlq.IsPage)
                {                                       
                    sbuser.AppendFormat(" limit {0},{1}", uvlq.Start, uvlq.Limit);//分頁
                }

                sbuserDT = _accessMySql.getDataTable(sbuser.ToString());

                //如果查到的table的數據條數為零，返回空字符串
                if (sbuserDT.Rows.Count == 0)
                {
                    return userIdString;
                }  
                //獲取查詢到的所有user_id
                for (int i = 0; i < sbuserDT.Rows.Count; i++)
                {
                    userId.AppendFormat("{0},", Convert.ToInt32(sbuserDT.Rows[i]["user_id"]));
                }               
                userIdString = userId.ToString().Substring(0, userId.Length - 1);
               
                return userIdString;
                #region 會員消費記錄排序，得到前25條數據的user_id
                //                //得到常溫商品總額集合sbNPro
                //                StringBuilder sbNPro = new StringBuilder("");
                //                //List<Model.Query.UserVipListQuery> sbNProList = new List<UserVipListQuery>();
                //                DataTable sbNProDT = new DataTable();
                //                sbNPro.Append(" select om.user_id,sum(single_money * buy_num) as normal_product ,sum(od.deduct_bonus) as normal_deduct_bonus");
                //                sbNPro.Append(" from order_master om  ");
                //                sbNPro.Append(" left join order_slave os on os.order_id = om.order_id");
                //                sbNPro.Append(" left join order_detail od on od.slave_id = os.slave_id");
                //                sbNPro.AppendFormat(@" where od.product_freight_set in (1,3) and od.detail_status = 4  and om.order_status = 99 
                //                                    and od.item_mode in (0,2) and om.user_id in ({0}) ", userIdString);
                //                sbNPro.Append(timelimit);
                //                sbNPro.Append("  group by om.user_id ");
                //                //sbNProList = _accessMySql.getDataTableForObj<Model.Query.UserVipListQuery>(sbNPro.ToString());
                //                sbNProDT = _accessMySql.getDataTable(sbNPro.ToString());

                //                //得到低溫商品總額集合sbLPro
                //                StringBuilder sbLPro = new StringBuilder("");
                //                //List<Model.Query.UserVipListQuery> sbLProList = new List<UserVipListQuery>();
                //                DataTable sbLProDT = new DataTable();
                //                sbLPro.Append(" select om.user_id,sum(single_money * buy_num) as low_product,sum(od.deduct_bonus) as low_deduct_bonus ");
                //                sbLPro.Append(" from order_master om ");
                //                sbLPro.Append(" left join order_slave os on os.order_id = om.order_id");
                //                sbLPro.Append(" left join order_detail od on od.slave_id = os.slave_id");
                //                sbLPro.AppendFormat(@" where od.product_freight_set in (2,4,5,6) and od.detail_status = 4  and om.order_status = 99 
                //                                    and od.item_mode in (0,2) and om.user_id in ({0}) ", userIdString);
                //                sbLPro.Append(timelimit);
                //                sbLPro.Append(" group by om.user_id ");
                //                //sbLProList = _accessMySql.getDataTableForObj<Model.Query.UserVipListQuery>(sbLPro.ToString());
                //                sbLProDT = _accessMySql.getDataTable(sbLPro.ToString());


                //                //foreach (var sbuserItem in sbuserList)
                //                //{
                //                //    foreach (var sbNProItem in sbNProList)
                //                //    {
                //                //        if (sbuserItem.user_id == sbNProItem.user_id)
                //                //        {
                //                //            if(String.IsNullOrEmpty(sbuserItem.sum_amount.ToString()))
                //                //            {
                //                //             sbuserItem.sum_amount = 0;
                //                //            }
                //                //             if(String.IsNullOrEmpty(sbuserItem.sum_bonus.ToString()))
                //                //            {
                //                //             sbuserItem.sum_bonus = 0;
                //                //            }
                //                //            sbuserItem.normal_product = sbNProItem.normal_product;
                //                //            sbuserItem.normal_deduct_bonus = sbNProItem.normal_deduct_bonus;
                //                //            sbuserItem.sum_amount + =  sbNProItem.normal_product;
                //                //            sbuserItem.sum_bonus + =  sbNProItem.normal_deduct_bonus;
                //                //        }
                //                //    }
                //                //    foreach (var sbLProItem in sbLProList)
                //                //    {
                //                //        if (sbuserItem.user_id == sbLProItem.user_id)
                //                //        {
                //                //            if(String.IsNullOrEmpty(sbuserItem.sum_amount.ToString()))
                //                //            {
                //                //             sbuserItem.sum_amount = 0;
                //                //            }
                //                //             if(String.IsNullOrEmpty(sbuserItem.sum_bonus.ToString()))
                //                //            {
                //                //             sbuserItem.sum_bonus = 0;
                //                //            }
                //                //            sbuserItem.low_product = sbLProItem.low_product;
                //                //            sbuserItem.low_deduct_bonus = sbLProItem.low_deduct_bonus;
                //                //            sbuserItem.sum_amount + =  sbLProItem.low_product;
                //                //            sbuserItem.sum_bonus + =  sbLProItem.low_deduct_bonus;
                //                //        }
                //                //    }                
                //                //}

                //                for (int i = 0; i < sbuserDT.Rows.Count; i++)
                //                {
                //                    for (int j = 0; j < sbNProDT.Rows.Count; j++)
                //                    {
                //                        if (Convert.ToDecimal(sbuserDT.Rows[i]["user_id"]) == Convert.ToDecimal(sbNProDT.Rows[j]["user_id"]))
                //                        {
                //                            sbuserDT.Rows[i]["normal_product"] = sbNProDT.Rows[j]["normal_product"];
                //                            sbuserDT.Rows[i]["sum_amount"] = Convert.ToDecimal(sbuserDT.Rows[i]["sum_amount"]) + Convert.ToDecimal(sbNProDT.Rows[j]["normal_product"]);
                //                        }
                //                    }
                //                    for (int k = 0; k < sbLProDT.Rows.Count; k++)
                //                    {
                //                        if (Convert.ToDecimal(sbuserDT.Rows[i]["user_id"]) == Convert.ToDecimal(sbLProDT.Rows[k]["user_id"]))
                //                        {
                //                            sbuserDT.Rows[i]["low_product"] = sbLProDT.Rows[k]["low_product"];
                //                            sbuserDT.Rows[i]["sum_amount"] = Convert.ToDecimal(sbuserDT.Rows[i]["sum_amount"]) + Convert.ToDecimal(sbLProDT.Rows[k]["low_product"]);
                //                        }
                //                    }
                //                }
                //                //DataTable排序
                //                #region 獲得DataTable，按照會員消費總額排序（降序）
                //                DataRow row = sbuserDT.NewRow();
                //                DataTable _dt = new DataTable();
                //                DataTable paixunDT = new DataTable();
                //                int id = 0;
                //                _dt = sbuserDT;
                //                object sumAmountObj = null;
                //                object userIdObj = null;

                //                paixunDT.Columns.Add("user_id", typeof(int));
                //                paixunDT.Columns.Add("sum_amount", typeof(decimal));

                //                for (int i = 1; i < _dt.Rows.Count; i++)
                //                {
                //                    for (int j = 0; j < _dt.Rows.Count - i; j++)
                //                    {
                //                        if (Convert.ToDecimal(_dt.Rows[j]["sum_amount"]) >= Convert.ToDecimal(_dt.Rows[j + 1]["sum_amount"]))
                //                        {

                //                            sumAmountObj = _dt.Rows[j + 1]["sum_amount"];
                //                            userIdObj = _dt.Rows[j + 1]["user_id"];
                //                            _dt.Rows[j + 1]["sum_amount"] = _dt.Rows[j]["sum_amount"];
                //                            _dt.Rows[j + 1]["user_id"] = _dt.Rows[j]["user_id"];
                //                            _dt.Rows[j]["sum_amount"] = sumAmountObj;
                //                            _dt.Rows[j]["user_id"] = userIdObj;

                //                        }
                //                    }
                //                    id = Convert.ToInt32(_dt.Rows[_dt.Rows.Count - i]["user_id"]);
                //                    for (int k = 0; k < sbuserDT.Rows.Count; k++)
                //                    {
                //                        if (Convert.ToInt32(sbuserDT.Rows[k]["user_id"]) == id)
                //                        {
                //                            row = sbuserDT.Rows[k];
                //                        }
                //                    }

                //                    DataRow dr = paixunDT.NewRow();
                //                    dr[0] = row["user_id"];
                //                    dr[1] = row["sum_amount"];
                //                    paixunDT.Rows.Add(dr);
                //                }
                //                //paixunDT.Rows.Add(sbuserDT.Rows[0]) ;   
                //                #endregion

                //                StringBuilder userId25 = new StringBuilder();
                //                string userIdString25 = string.Empty;
                //                for (int m = uvlq.Start; m <= uvlq.Limit; m++)
                //                {
                //                    userId25.AppendFormat("{0},", Convert.ToInt32(paixunDT.Rows[m]["user_id"]));
                //                }
                //                userIdString25 = userId25.ToString();
                //                userIdString25 = userIdString25.Substring(0, userIdString25.Length - 1);
                //                return userIdString25; 
                #endregion
            
            }
            catch (Exception ex)
            {
                throw new Exception("UsersDao-->GetViplistUserId-->" + ex.Message , ex);
            }
        }

        #endregion        
        //create by shuangshuang0420j 20140923 17:44
        public List<Model.Query.UserVipListQuery> GetVipList(Model.Query.UserVipListQuery uvlq,string userIdString, ref int totalCount)       
        {
            
            StringBuilder sql = new StringBuilder();
            try
            {
                uvlq.Replace4MySQL();
                string timelimit = string.Empty;
                //添加搜索條件
                if (uvlq.create_dateOne != 0)
                {
                    timelimit += string.Format(" and om.order_createdate >='{0}' ", uvlq.create_dateOne);
                }
                if (uvlq.create_dateTwo != 0)
                {
                    timelimit += string.Format(" and om.order_createdate <='{0}' ", uvlq.create_dateTwo);
                }
                StringBuilder sbuser = new StringBuilder();
                sbuser.Append(" select  ");
                sbuser.Append(" u.user_id,u.user_status,u.last_time,count(om.order_id) as cou,u.user_name ,u.ml_code, ");
                sbuser.Append(" u.user_phone,u.user_mobile,u.user_gender,u.user_birthday_year,u.user_birthday_month,");
                sbuser.Append(" u.user_birthday_day,u.user_type,u.adm_note,u.send_sms_ad,u.paper_invoice, u.user_password,u.user_reg_date,");                
                
                sbuser.Append(" sum(om.order_freight_low) as freight_low ,sum(om.order_freight_normal) as freight_normal, ");
                sbuser.Append(" max( om.order_createdate ) as order_createdate,");
                sbuser.Append(" round(sum(om.deduct_happygo_convert * om.deduct_happygo)) as happygo ");
                sbuser.Append(" from order_master om inner join users u on u.user_id = om.user_id ");
                sbuser.Append(" where om.order_status = 99 ");
                sbuser.AppendFormat(" and om.user_id in ({0}) ", userIdString);
                sbuser.Append(timelimit);
                sbuser.Append("  group by om.user_id order by om.user_id ");
                
                        

                //order_status=99表示訂單已歸檔
                //得到常溫商品總額集合sbNPro
                StringBuilder sbNPro = new StringBuilder("");
                sbNPro.Append(@" select om.user_id,sum(od.single_money * (case  when od.item_mode=0  then od.buy_num 
                when od.item_mode=2 then od.parent_num end)) as normal_product ,sum(od.deduct_bonus) as normal_deduct_bonus");                        
                sbNPro.Append(" from order_master om  ");
                sbNPro.Append(" left join order_slave os on os.order_id = om.order_id");
                sbNPro.Append(" left join order_detail od on od.slave_id = os.slave_id");
                sbNPro.AppendFormat(@" where od.product_freight_set in (1,3) and od.detail_status = 4  and om.order_status = 99 
                                    and od.item_mode in (0,2) and om.user_id in ({0}) ", userIdString);
                sbNPro.Append(timelimit);
                sbNPro.Append("  group by om.user_id order by om.user_id desc");

                //得到低溫商品總額集合sbLPro
                StringBuilder sbLPro = new StringBuilder("");
                sbLPro.Append(@" select om.user_id,sum(od.single_money * (case  when od.item_mode=0  then od.buy_num 
                when od.item_mode=2 then od.parent_num end)) as low_product,sum(od.deduct_bonus) as low_deduct_bonus ");
                sbLPro.Append(" from order_master om ");
                sbLPro.Append(" left join order_slave os on os.order_id = om.order_id");
                sbLPro.Append(" left join order_detail od on od.slave_id = os.slave_id");
                sbLPro.AppendFormat(@" where od.product_freight_set in (2,4,5,6) and od.detail_status = 4  and om.order_status = 99 
                                    and od.item_mode in (0,2) and om.user_id in ({0}) ", userIdString);
                sbLPro.Append(timelimit);
                sbLPro.Append(" group by om.user_id order by om.user_id desc");

                //得到ct集合sbCT
                StringBuilder sbCT = new StringBuilder("");
                sbCT.Append(" select om.user_id, sum(opc.offsetamt) as ct ");
                sbCT.Append(" from order_master om inner join order_payment_ct opc on om.order_id = opc.lidm ");
                sbCT.AppendFormat(@" where   om.order_status = 99 and om.user_id in ({0}) ", userIdString);
                sbCT.Append(timelimit);
                sbCT.Append("  group by om.user_id order by om.user_id desc");

                //得到ht集合sbHT
                StringBuilder sbHT = new StringBuilder("");
                sbHT.Append(" select om.user_id, sum(oph.redem_discount_amount) as ht");
                sbHT.Append(" from order_master om inner join order_payment_hitrust oph on om.order_id = oph.order_id");
                sbHT.AppendFormat(@" where  om.order_status = 99 and om.user_id in ({0}) ", userIdString);
                sbHT.Append(timelimit);
                sbHT.Append("  group by om.user_id order by om.user_id desc");


                #region 手動插入數據
                ////                foreach ( var sbuserItem in sbuserList)
                ////                {
                ////                    foreach (var sbNProItem in sbNProList)
                ////                    {
                ////                        if (sbuserItem.user_id == sbNProItem.user_id)
                ////                        {
                ////                            sbuserItem.normal_product = sbNProItem.normal_product;
                ////                            sbuserItem.normal_deduct_bonus = sbNProItem.normal_deduct_bonus;
                ////                            //sbuserItem.sum_amount + = sbNProItem.normal_product;
                ////                            //sbuserItem.sum_bonus + = sbNProItem.normal_deduct_bonus;
                ////                        }                  
                ////                    }
                ////                    foreach (var sbLProItem in sbLProList)
                ////                    {
                ////                        if (sbuserItem.user_id == sbLProItem.user_id)
                ////                        {
                ////                            sbuserItem.low_product = sbLProItem.low_product;
                ////                            sbuserItem.low_deduct_bonus = sbLProItem.low_deduct_bonus;
                ////                            //sbuserItem.sum_amount + = sbLProItem.low_product;
                ////                            //sbuserItem.sum_bonus + = sbLProItem.low_deduct_bonus;
                ////                        }
                ////                    }
                ////                    foreach (var sbCTItem in sbCTList)
                ////                    {
                ////                        if (sbuserItem.user_id == sbCTItem.user_id)
                ////                        {
                ////                            sbuserItem.ct = sbCTItem.ct;
                ////                        }
                ////                    }
                ////                    foreach (var sbHTItem in sbHTList)
                ////                    {
                ////                        if (sbuserItem.user_id == sbHTItem.user_id)
                ////                        {
                ////                            sbuserItem.ht = sbHTItem.ht;
                ////                        }
                ////                    }                   
                ////                } 
                #endregion

                sql.Append(@" select b.*,IFNULL(n.normal_product,0) as normal_product,IFNULL(l.low_product,0) as low_product,IFNULL(c.ct,0) as ct ,IFNULL(h.ht,0) as ht,
                                           IFNULL(n.normal_deduct_bonus,0)+IFNULL(l.low_deduct_bonus,0) as sum_bonus,
                                           IFNULL(n.normal_product,0)+IFNULL(l.low_product,0)  as sum_amount  ");
                sql.AppendFormat(" from  ( {0} ) b ", sbuser);
                sql.AppendFormat(" left join ( {0} )  n on n.user_id = b.user_id", sbNPro);
                sql.AppendFormat(" left join ( {0} )  l on l.user_id = b.user_id", sbLPro);
                sql.AppendFormat(" left join ( {0} )  c on c.user_id = b.user_id", sbCT);
                sql.AppendFormat(" left join  ( {0} )  h on h.user_id = b.user_id", sbHT);
                sql.AppendFormat(" where 1=1   ");//order by sum_amount DESC
                //得到數據總條數
                totalCount = 0;
                if (uvlq.IsPage)
                {
                    string sqlForCount = @"select count(s.user_id) as totalCount from (select u.user_id from order_master om inner join users u 
                                              on u.user_id = om.user_id  where om.order_status = 99 " + timelimit + "  group by om.user_id) s ";                     
                    System.Data.DataTable _dt = _accessMySql.getDataTable(sqlForCount);
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    //sql.AppendFormat(" limit {0},{1}", uvlq.Start, uvlq.Limit);//分頁
                }
                DataTable vipListDT = _accessMySql.getDataTable(sql.ToString());        
                List<Model.Query.UserVipListQuery> store = new List<UserVipListQuery>();

                for (var i = 0; i < vipListDT.Rows.Count; i++)
                {
                    Model.Query.UserVipListQuery query = new UserVipListQuery();

                    query.user_id = Convert.ToUInt32(vipListDT.Rows[i]["user_id"]);
                    query.user_status = Convert.ToUInt32(vipListDT.Rows[i]["user_status"]);
                    query.cou = Convert.ToInt64(vipListDT.Rows[i]["cou"]);
                    query.user_name = Convert.ToString(vipListDT.Rows[i]["user_name"]);
                    query.ml_code = Convert.ToString(vipListDT.Rows[i]["ml_code"]);
                    query.user_phone = Convert.ToString(vipListDT.Rows[i]["user_phone"]);
                    query.user_mobile = Convert.ToString(vipListDT.Rows[i]["user_mobile"]);
                    query.user_gender = Convert.ToUInt32(vipListDT.Rows[i]["user_gender"]);
                    query.user_birthday_year = Convert.ToUInt32(vipListDT.Rows[i]["user_birthday_year"]);
                    query.user_birthday_month = Convert.ToUInt32(vipListDT.Rows[i]["user_birthday_month"]);
                    query.user_birthday_day = Convert.ToUInt32(vipListDT.Rows[i]["user_birthday_day"]);
                    query.user_type = Convert.ToInt32(vipListDT.Rows[i]["user_type"]);
                    query.adm_note = Convert.ToString(vipListDT.Rows[i]["adm_note"]);
                    query.send_sms_ad = Convert.ToBoolean(vipListDT.Rows[i]["send_sms_ad"]);
                    query.paper_invoice = Convert.ToBoolean(vipListDT.Rows[i]["paper_invoice"]);
                    query.user_password = Convert.ToString(vipListDT.Rows[i]["user_password"]);
                    query.user_reg_date = Convert.ToUInt32(vipListDT.Rows[i]["user_reg_date"]);
                    query.last_time = Convert.ToUInt32(vipListDT.Rows[i]["last_time"]);
                    query.freight_low = Convert.ToDecimal(vipListDT.Rows[i]["freight_low"]);
                    query.freight_normal = Convert.ToDecimal(vipListDT.Rows[i]["freight_normal"]);
                    query.order_createdate = Convert.ToUInt32(vipListDT.Rows[i]["order_createdate"]);
                    query.happygo = Convert.ToDouble(vipListDT.Rows[i]["happygo"]);
                    query.normal_product = Convert.ToDecimal(vipListDT.Rows[i]["normal_product"]);
                    query.low_product = Convert.ToDecimal(vipListDT.Rows[i]["low_product"]);
                    query.ct = Convert.ToDecimal(vipListDT.Rows[i]["ct"]);
                    query.ht = Convert.ToDecimal(vipListDT.Rows[i]["ht"]);
                    query.sum_bonus = Convert.ToDecimal(vipListDT.Rows[i]["sum_bonus"]);
                    query.sum_amount = Convert.ToDecimal(vipListDT.Rows[i]["sum_amount"]);
                    store.Add(query);
                }
                return store;
                                              
            }
            catch (Exception ex)
            {
                throw new Exception("UsersDao-->GetVipList-->" + ex.Message + sql, ex);
            }
        }
        #endregion
        #region 獲取normal，low，ct，ht
        //public UserVipListQuery GetNormalProd(UserVipListQuery uvlq)
        //{
        //    StringBuilder sbNPro = new StringBuilder();
        //    try
        //    {
        //        ////order_status=99表示訂單已歸檔
        //        //得到常溫商品總額
        //        sbNPro.Append(" select om.user_id,sum(single_money * buy_num) as normal_product ");
        //        sbNPro.Append(" from order_master om  ");
        //        sbNPro.Append(" left join order_slave os on os.order_id = om.order_id");
        //        sbNPro.Append(" left join order_detail od on od.slave_id = os.slave_id");
        //        sbNPro.Append(" where od.product_freight_set in (1,3) and od.detail_status = 4  and om.order_status = 99 ");
        //        //添加搜索條件
        //        if (uvlq.create_dateOne != 0)
        //        {
        //            sbNPro.AppendFormat(" and om.order_createdate >='{0}' ", uvlq.create_dateOne);
        //        }
        //        if (uvlq.create_dateTwo != 0)
        //        {
        //            sbNPro.AppendFormat(" and om.order_createdate <='{0}' ", uvlq.create_dateTwo);
        //        }
        //        if (uvlq.user_id != 0)
        //        {
        //            sbNPro.AppendFormat(" and om.user_id='{0}'", uvlq.user_id);
        //        }
        //        sbNPro.Append("  group by om.user_id order by sum(om.order_amount) desc ");
        //        return _accessMySql.getSinggleObj<UserVipListQuery>(sbNPro.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("UsersDao-->GetNormalProd-->" + ex.Message + sbNPro.ToString(), ex);
        //    }
        //}
        //public UserVipListQuery GetLowProd(UserVipListQuery uvlq)
        //{
        //    StringBuilder sbLPro = new StringBuilder("");
        //    try
        //    {

        //        sbLPro.Append(" select om.user_id,sum(single_money * buy_num) as low_product ");
        //        sbLPro.Append(" from order_master om ");
        //        sbLPro.Append(" left join order_slave os on os.order_id = om.order_id");
        //        sbLPro.Append(" left join order_detail od on od.slave_id = os.slave_id");
        //        sbLPro.Append(" where od.product_freight_set in (2,4,5,6) and od.detail_status = 4  and om.order_status = 99 ");
        //        if (uvlq.create_dateOne != 0)
        //        {
        //            sbLPro.AppendFormat(" and om.order_createdate >='{0}' ", uvlq.create_dateOne);
        //        }
        //        if (uvlq.create_dateTwo != 0)
        //        {
        //            sbLPro.AppendFormat(" and om.order_createdate <='{0}' ", uvlq.create_dateTwo);
        //        }
        //        if (uvlq.user_id != 0)
        //        {
        //            sbLPro.AppendFormat(" and om.user_id='{0}'", uvlq.user_id);
        //        }
        //        return _accessMySql.getSinggleObj<UserVipListQuery>(sbLPro.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("UsersDao-->GetNormalProd-->" + ex.Message + sbLPro, ex);
        //    }
        //}
        //public UserVipListQuery GetProdCT(UserVipListQuery uvlq)
        //{
        //    StringBuilder sbCT = new StringBuilder();
        //    try
        //    {
        //        sbCT.Append(" select om.user_id, sum(opc.offsetamt) as ct ");
        //        sbCT.Append(" from order_master om inner join order_payment_ct opc on om.order_id = opc.lidm ");
        //        sbCT.Append(" where   om.order_status = 99 ");
        //        if (uvlq.create_dateOne != 0)
        //        {
        //            sbCT.AppendFormat(" and om.order_createdate >='{0}' ", uvlq.create_dateOne);
        //        }
        //        if (uvlq.create_dateTwo != 0)
        //        {
        //            sbCT.AppendFormat(" and om.order_createdate <='{0}' ", uvlq.create_dateTwo);
        //        }
        //        if (uvlq.user_id != 0)
        //        {
        //            sbCT.AppendFormat(" and om.user_id='{0}'", uvlq.user_id);

        //        }
        //        return _accessMySql.getSinggleObj<UserVipListQuery>(sbCT.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("UsersDao-->GetNormalProd-->" + ex.Message + sbCT, ex);
        //    }
        //}
        //public UserVipListQuery GetProdHT(UserVipListQuery uvlq)
        //{
        //    StringBuilder sbHT = new StringBuilder();
        //    try
        //    {
        //        sbHT.Append(" select om.user_id, sum(oph.redem_discount_amount) as ht");
        //        sbHT.Append(" from order_master om inner join order_payment_hitrust oph on om.order_id = oph.order_id");

        //        sbHT.Append(" where  om.order_status = 99 ");
        //        if (uvlq.create_dateOne != 0)
        //        {
        //            sbHT.AppendFormat(" and om.order_createdate >='{0}' ", uvlq.create_dateOne);
        //        }
        //        if (uvlq.create_dateTwo != 0)
        //        {
        //            sbHT.AppendFormat(" and om.order_createdate <='{0}' ", uvlq.create_dateTwo);
        //        }
        //        if (uvlq.user_id != 0)
        //        {
        //            sbHT.AppendFormat(" and om.user_id='{0}'", uvlq.user_id);

        //        }
        //        return _accessMySql.getSinggleObj<UserVipListQuery>(sbHT.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("UsersDao-->GetNormalProd-->" + ex.Message + sbHT, ex);
        //    }
        //}
        #endregion

        #region 判斷是否是vip用戶 +DataTable IsVipUserId()
        public DataTable IsVipUserId(uint user_id)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(" select * from vip_user ");
                sb.Append(" where  group_id in (SELECT group_id from  vip_user_group where group_name like '%VIP%') ");
                if (user_id != 0)
                {
                    sb.AppendFormat(" and user_id='{0}'", user_id);
                }
                return _accessMySql.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UsersDao-->IsVipUserId-->" + ex.Message + sb.ToString(), ex);
            }

        }

        #endregion

        #endregion


        public DataTable QueryByUserMobile(string userMobile)
        {
            strSql = string.Format("select user_id from users where user_mobile = '{0}';", userMobile);
            return _accessMySql.getDataTable(strSql);
        }

        public int SaveUserPhone(Model.Query.UserQuery uQuery)
        {
            _userhistoryDao = new UserHistoryDao(connStr);
            _smsdao = new SmsDao(connStr);
            _serialDao = new SerialDao(connStr);
            Serial serial = new Serial();
            uQuery.Replace4MySQL();
            int i = 0;
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();

                mySqlCmd.CommandType = System.Data.CommandType.Text;

                #region 處理serial表數據


                mySqlCmd.CommandText = _serialDao.Update(22);//22電話會員
                serial = _serialDao.GetSerialById(22);
                uQuery.user_id = Convert.ToUInt32(serial.Serial_Value) + 1;

                #endregion

                #region 處理user 和user_history數據

                mySqlCmd.CommandText = Save(uQuery);
                mySqlCmd.CommandText += _userhistoryDao.Save(uQuery);

                #endregion

                #region 處理sms表
                Sms smsModel = new Sms();
                smsModel.type = 9;
                smsModel.mobile = uQuery.user_mobile;
                smsModel.subject = "電話會員";
                smsModel.content = "感謝您選擇成為吉甲地市集電話會員，未來我們將透過簡訊提供商品優惠。日後若有電話訂購服務之需求，可撥打專線(02)2783-4995，謝謝。";
                smsModel.send = uQuery.send_sms_ad == true ? 1 : 0;
                smsModel.created = uQuery.created;
                smsModel.modified = uQuery.created;
                smsModel.estimated_send_time = uQuery.created;
                mySqlCmd.CommandText += _smsdao.SaveSms(smsModel);

                #endregion

                i += mySqlCmd.ExecuteNonQuery();
                //全部执行成功以后，对serial表的serial_value的值進行變更
                if (i == 3)
                {
                    serial.Serial_Value = serial.Serial_Value + 1;
                    int j = _serialDao.Update(serial);
                }
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("UsersDao-->Save-->" + ex.Message + mySqlCmd.CommandText.ToString(), ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
            return i;
        }

        /// <summary>
        /// TEXT提示框獲得的信息
        /// </summary>
        /// <param name="condition">搜索條件</param>
        /// <returns>符合要求的集合</returns>
        /// add by wangwei0216w 2014/10/27 
        public List<Users> GetUserInfoByTest(string condition)
        {
            try
            {
                StringBuilder sql = new StringBuilder("SELECT user_name,user_email,user_id,user_mobile,user_phone,user_address,user_gender FROM users ");// add by wwei0216w 添加查詢user_gender 性別列
                sql.AppendFormat(" WHERE user_id = '{0}'", condition);//edit by zhuoqin0830w  修改為使用userid查詢  2015/11/16
                return _accessMySql.getDataTableForObj<Users>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UsersDao.GetUserInfoByTest(Users query)-->" + ex.Message, ex);
            }
        }

        public string UpdateFirstTime(uint user_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"UPDATE   users SET	first_time = 0 WHERE user_id = {0}", user_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("UsersDao.UpdateFirstTime -->" + ex.Message + sql.ToString(), ex);
            }
        }


        public List<UserQuery> GetBonusList(UserQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            query.Replace4MySQL();
            try
            {
                sql.Append("select su.s_id, u.user_id,u.user_email,u.user_name,u.user_mobile,u.user_status");
                sql.Append(" ,u.user_reg_date,u.user_gender,user_company_id");
                sqlCondi.Append(" from users u");
                sqlCondi.Append(" left join sales_user su on u.user_id=su.user_id where 1=1");
                if (!string.IsNullOrEmpty(query.user_email))
                {
                    sqlCondi.AppendFormat(" and u.user_email like '%{0}%'", query.user_email);
                }
                if (!string.IsNullOrEmpty(query.user_name))
                {
                    sqlCondi.AppendFormat(" and u.user_name like '%{0}%'", query.user_name);
                }
                if (!string.IsNullOrEmpty(query.user_mobile))
                {
                    sqlCondi.AppendFormat(" and u.user_mobile like '%{0}%'", query.user_mobile);
                }
                if (query.is_select_status == 1)
                {
                    sqlCondi.AppendFormat(" and u.user_status={0}", query.user_status);
                }
                if (query.date_start != 0)
                {
                    sqlCondi.AppendFormat(" and u.user_reg_date>={0}", query.date_start);
                }
                if (query.date_end != 0)
                {
                    sqlCondi.AppendFormat(" and u.user_reg_date<={0}", query.date_end);
                }
                sqlCondi.Append(" order by u.user_id desc");
                totalCount = 0;
                if (query.IsPage)
                {
                    DataTable _dt = _accessMySql.getDataTable("select count(u.user_id) as totalCount " + sqlCondi.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = int.Parse(_dt.Rows[0]["totalCount"].ToString());
                    }
                    sqlCondi.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }

                sql.Append(sqlCondi.ToString());
                return _accessMySql.getDataTableForObj<UserQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UsersDao.GetBonusList-->" + ex.Message + sql.ToString(), ex);
            }
        }


//        public List<Model.Query.UserVipListQuery> ExportVipListCsv(UserVipListQuery query,string userIdString)
//        {
//            StringBuilder sql = new StringBuilder();
//            try
//            {
//                query.Replace4MySQL();
//                string timelimit = string.Empty;
//                //添加搜索條件
//                if (query.create_dateOne != 0)
//                {
//                    timelimit += string.Format(" and om.order_createdate >='{0}' ", query.create_dateOne);
//                }
//                if (query.create_dateTwo != 0)
//                {
//                    timelimit += string.Format(" and om.order_createdate <='{0}' ", query.create_dateTwo);
//                }
//                //count(*) as cou,
//                StringBuilder sbuser = new StringBuilder();
//                sbuser.Append(" select  ");
//                sbuser.Append(" u.user_id,count(om.order_id) as cou,u.user_status,u.user_name ,u.ml_code, ");
//                sbuser.Append(" u.user_phone,u.user_mobile,u.user_gender,u.user_birthday_year,u.user_birthday_month,");
//                sbuser.Append(" u.last_time,");
//                sbuser.Append(" u.user_birthday_day,u.user_type,u.adm_note,u.send_sms_ad,u.paper_invoice, u.user_password,u.user_reg_date,");
//                sbuser.Append(" sum(om.order_freight_low) as freight_low ,sum(om.order_freight_normal) as freight_normal, ");
//                sbuser.Append(" max( om.order_createdate ) as order_createdate,");
//                sbuser.Append(" round(sum(om.deduct_happygo_convert * om.deduct_happygo)) as happygo ");
//                sbuser.Append(" from order_master om inner join users u on u.user_id = om.user_id ");
//                sbuser.Append(" where om.order_status = 99 ");
//                sbuser.AppendFormat(" and om.user_id in ({0}) ", userIdString);
//                sbuser.Append(timelimit);
//                sbuser.Append("  group by om.user_id ");

                
//                ////order_status=99表示訂單已歸檔
//                //得到常溫商品總額
//                StringBuilder sbNPro = new StringBuilder("");
//                sbNPro.Append(@" select om.user_id,sum(single_money * (case  when od.item_mode=0  then od.buy_num 
//                              when od.item_mode=2 then od.parent_num end)) as normal_product ,sum(od.deduct_bonus) as normal_deduct_bonus");
//                sbNPro.Append(" from order_master om  ");
//                sbNPro.Append(" left join order_slave os on os.order_id = om.order_id");
//                sbNPro.Append(" left join order_detail od on od.slave_id = os.slave_id");
//                sbNPro.AppendFormat(@" where od.product_freight_set in (1,3) and od.detail_status = 4  and om.order_status = 99 
//                                    and od.item_mode in (0,2) and om.user_id in ({0}) ", userIdString);
//                sbNPro.Append(timelimit);
//                sbNPro.Append("  group by om.user_id ");              


//                //得到低溫商品總額
//                StringBuilder sbLPro = new StringBuilder("");
//                sbLPro.Append(@" select om.user_id,sum(single_money * (case  when od.item_mode=0  then od.buy_num 
//                              when od.item_mode=2 then od.parent_num end)) as low_product,sum(od.deduct_bonus) as low_deduct_bonus ");
//                sbLPro.Append(" from order_master om ");
//                sbLPro.Append(" left join order_slave os on os.order_id = om.order_id");
//                sbLPro.Append(" left join order_detail od on od.slave_id = os.slave_id");
//                sbLPro.AppendFormat(@" where od.product_freight_set in (2,4,5,6) and od.detail_status = 4  and om.order_status = 99 
//                                    and od.item_mode in (0,2) and om.user_id in ({0}) ", userIdString);
//                sbLPro.Append(timelimit);
//                sbLPro.Append(" group by om.user_id ");

                
//                //得到ct
//                StringBuilder sbCT = new StringBuilder("");
//                sbCT.Append(" select om.user_id, sum(opc.offsetamt) as ct ");
//                sbCT.Append(" from order_master om inner join order_payment_ct opc on om.order_id = opc.lidm ");
//                sbCT.AppendFormat(@" where   om.order_status = 99 and om.user_id in ({0}) ", userIdString);
//                sbCT.Append(timelimit);
//                sbCT.Append("  group by om.user_id ");                


//                //得到ht集合sbHT
//                StringBuilder sbHT = new StringBuilder("");               
//                sbHT.Append(" select om.user_id, sum(oph.redem_discount_amount) as ht");
//                sbHT.Append(" from order_master om inner join order_payment_hitrust oph on om.order_id = oph.order_id");
//                sbHT.AppendFormat(@" where  om.order_status = 99 and om.user_id in ({0}) ", userIdString);
//                sbHT.Append(timelimit);
//                sbHT.Append("  group by om.user_id ");

                

//                //獲取全部信息
//                sql.Append(@" select b.*,n.normal_product,l.low_product,c.ct,h.ht,
//                                           IFNULL(n.normal_deduct_bonus,0)+IFNULL(l.low_deduct_bonus,0) as sum_bonus,
//                                           IFNULL(n.normal_product,0)+IFNULL(l.low_product,0)  as sum_amount  ");
//                sql.AppendFormat(" from  ( {0} ) b ", sbuser);
//                sql.AppendFormat(" left join ( {0} )  n on n.user_id = b.user_id", sbNPro);
//                sql.AppendFormat(" left join ( {0} )  l on l.user_id = b.user_id", sbLPro);
//                sql.AppendFormat(" left join ( {0} )  c on c.user_id = b.user_id", sbCT);
//                sql.AppendFormat(" left join  ( {0} )  h on h.user_id = b.user_id", sbHT);
//                sql.AppendFormat(" where 1=1   order by sum_amount DESC");
             
//                return _accessMySql.getDataTableForObj<UserVipListQuery>(sql.ToString());
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("UsersDao-->ExportVipListCsv-->" + ex.Message + sql, ex);
//            }
//        }

        public List<UserQuery> Query(Model.Custom.Users query)
        {
            try
            {
                StringBuilder sql = new StringBuilder("select user_id,user_email,user_name,user_mobile,user_phone,user_company_id,user_address,user_status,");
                sql.Append(" CONCAT(user_name ,'[',user_email,']')  as 'file_name'   from users where 1=1");
                sql.Append(" and user_status not in (0,2)");//0未啟用，2停用
                if (!string.IsNullOrEmpty(query.user_name))
                {
                    sql.AppendFormat(" and user_name like '%{0}%'", query.user_name);
                }

                return _accessMySql.getDataTableForObj<UserQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UsersDao.List<Model.Custom.Users> Query(Model.Custom.Users query)-->" + ex.Message, ex);
            }
        }
        public List<Users> GetUser(Users u)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("SELECT user_id,user_name,user_email,user_phone,user_mobile,user_address from users WHERE user_id='{0}';", u.user_id);
                return _accessMySql.getDataTableForObj<Users>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UsersDao.List<Users> GetUser(Users query)-->" + ex.Message, ex);
            }
        }

        public string GetUserIDbyEmail(string email)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"select user_id from users where user_email='{0}';", email);              
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("UsersDao.string GetUserIDbyEmail(string email)-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// add by chaojie1124j 2015/09/18通過群組編號和郵箱判斷此會員
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="group_id"></param>
        /// <returns></returns>
        public List<UserQuery> GetUserByEmail(string mail,uint group_id)
        {
            List<UserQuery> user = new List<UserQuery>();
            StringBuilder sql = new StringBuilder();
            try
            {
               
             
                sql.AppendFormat(@"select u.user_id,u.user_name from users u left join vip_user v on v.user_id =u.user_id where  u.user_email='{0}' ",mail);
                if (group_id != 0)
                {
                    sql.AppendFormat(" and v.group_id='{0}';", group_id);
                }
                return _accessMySql.getDataTableForObj<UserQuery>(sql.ToString());
              
            }
            catch (Exception ex)
            {
                throw new Exception("UsersDao->GetUserByEmail-->" + ex.Message+sql.ToString(), ex);
            }
        }
    }
}
