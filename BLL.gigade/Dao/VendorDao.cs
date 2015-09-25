/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：VendorDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2014/1/14 13:48:16 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model.Query;
using MySql.Data.MySqlClient;
using BLL.gigade.Common;
using System.Data;


namespace BLL.gigade.Dao
{
    public class VendorDao : IVendorImplDao
    {
        private IDBAccess _dbAccess;
        private Common.MySqlHelper mysqlHelp;
        UserHistoryDao _userhistoryDao = new UserHistoryDao("");
        SmsDao _smsdao = new SmsDao("");
        SerialDao _serialDao = new SerialDao("");
        private string connStr;

        public VendorDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            connStr = connectionStr;
            mysqlHelp = new Common.MySqlHelper(connectionStr);
            _serialDao = new SerialDao(connectionStr);
        }

        #region 獲取供應商列表+List<VendorQuery> Query(VendorQuery query, ref int totalCount)
        public List<VendorQuery> Query(VendorQuery query, ref int totalCount)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder("");
            StringBuilder sqlCon = new StringBuilder("");
            try
            {
                sql.Append(@" select  zipI.bigcode as i_bigcode, zipI.middlecode as i_midcode, zipI.middle as i_middle,zipI.zipcode as i_zipcode, CONCAT(zipI.zipcode ,'/',zipI.small) as 'i_zip',CONCAT(CONCAT(zipC.middle,zipC.small),vendor.company_address) as vendor_company_address, ");
                sql.Append(" zipC.bigcode as c_bigcode, zipC.middlecode as c_midcode,zipC.middle as c_middle,zipC.zipcode as c_zipcode,CONCAT(zipC.zipcode ,'/',zipC.small) as 'c_zip' ,");
                sql.Append(@" us.user_username as manage_name, vendor.*,");
                sql.Append(" FROM_UNIXTIME(vendor.agreement_createdate) as agr_date,FROM_UNIXTIME(vendor.agreement_start) as agr_start,FROM_UNIXTIME(vendor.agreement_end) as agr_end");//將php時間轉化為net時間
                sql.Append(" from vendor ");
                sql.Append(" left join t_zip_code zipC on vendor.company_zip=zipC.zipcode");
                sql.Append(" left join manage_user us on us.user_id=vendor.product_manage");
                sql.Append(" left join t_zip_code zipI on vendor.invoice_zip=zipI.zipcode ");
                sql.Append(" where 1=1 ");
                if (!string.IsNullOrEmpty(query.searchEmail))
                {
                    sqlCon.AppendFormat(" and vendor_email like '%{0}%'", query.searchEmail);
                }
                if (!string.IsNullOrEmpty(query.searchName))
                {
                    sqlCon.AppendFormat(" and vendor_name_simple like '%{0}%'", query.searchName);
                }
                if (!string.IsNullOrEmpty(query.vendor_name_full))
                {
                    sqlCon.AppendFormat(" and vendor_name_full like '%{0}%'", query.vendor_name_full);
                }
                if (!string.IsNullOrEmpty(query.searchInvoice))
                {
                    sqlCon.AppendFormat(" and vendor.vendor_invoice like '%{0}%'", query.searchInvoice);
                }
                if (!string.IsNullOrEmpty(query.erp_id))
                {
                    sqlCon.AppendFormat(" and vendor.erp_id like '%{0}%'", query.erp_id);
                }
                if (!string.IsNullOrEmpty(query.vendor_code))
                {
                    sqlCon.AppendFormat(" and vendor.vendor_code like '%{0}%'", query.vendor_code);
                }
                if (query.vendor_id != 0)
                {
                    sqlCon.AppendFormat(" and vendor.vendor_id = '{0}'", query.vendor_id);
                }
                if (query.create_dateOne != 0)
                {
                    sqlCon.AppendFormat(" and agreement_createdate >= '{0}'", query.create_dateOne);
                }
                if (query.create_dateTwo != 0)
                {
                    sqlCon.AppendFormat(" and agreement_createdate <= '{0}'", query.create_dateTwo);
                }
                if (query.searchStatus != -1)
                {
                    sqlCon.AppendFormat(" and dispatch='{0}'", query.searchStatus);
                }
                if (!string.IsNullOrEmpty(query.vendor_type))
                {
                    sqlCon.Append(" and (");
                    string[] checks = query.vendor_type.Split(',');
                    int num = 0;
                    for (int i = 0; i < checks.Length; i++)
                    {
                        if (num == 0)
                        {
                            sqlCon.AppendFormat(" vendor_type like '%{0}%'", checks[i]);
                            num++;
                        }
                        else
                        {
                            sqlCon.AppendFormat(" or vendor_type like '%{0}%'", checks[i]);
                            num++;
                        }
                    }
                    sqlCon.Append(" ) ");
                }
                sqlCon.Append(" order by vendor_id desc ");

                //得到數據總條數
                totalCount = 0;
                System.Data.DataTable _dt = _dbAccess.getDataTable(" select count(vendor_id) as totalCount from vendor where 1=1 " + sqlCon.ToString());
                if (_dt != null)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                }
                sqlCon.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                return _dbAccess.getDataTableForObj<VendorQuery>(sql.ToString() + sqlCon.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" VendorDao-->Query-->" + ex.Message + sql.ToString() + sqlCon.ToString(), ex);
            }
        }
        /// <summary>
        /// 獲取供應商詳細信息
        /// </summary>
        /// <param name="sqlwhere">查詢條件</param>
        /// <returns>供應商信息列表</returns>
        public DataTable GetVendorDetail(string sqlwhere)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"SELECT vendor_id ,vendor_code,case vendor_status when 1 then '啟用' when 2 then '停用' when 3 then '失格' end,vendor_name_full,vendor_name_simple,vendor_invoice,");
            sql.AppendFormat(@"company_phone,company_fax,vendor_email,company_person,company_zip,");
            sql.AppendFormat(@"company_address,invoice_zip,invoice_address,");
            sql.AppendFormat(@"cost_percent,creditcard_1_percent,creditcard_3_percent,FROM_UNIXTIME(agreement_createdate,'%Y/%m/%d'),");
            sql.AppendFormat(@"CONCAT_WS('~',FROM_UNIXTIME(agreement_start,'%Y/%m/%d'),FROM_UNIXTIME(agreement_end,'%Y/%m/%d')) as agreement_duration,");
            sql.AppendFormat(@"case checkout_type when 1 then '月結' when 2 then '半月結' ELSE '其它' end as checkout_type,bank_code,bank_name");
            sql.AppendFormat(@",bank_number,bank_account,freight_normal_money,freight_normal_limit,");
            sql.AppendFormat(@"freight_return_normal_money,freight_low_money,freight_low_limit,");
            sql.AppendFormat(@"freight_return_low_money,product_manage,user_username,");
            sql.AppendFormat(@"contact_name_1,contact_phone_1_1,contact_mobile_1,contact_email_1,");
            sql.AppendFormat(@"contact_type_2,contact_name_2,contact_phone_1_2,contact_mobile_2,contact_email_2,");
            sql.AppendFormat(@"contact_type_3,contact_name_3,contact_phone_1_3,contact_mobile_3,contact_email_3,");
            sql.AppendFormat(@"contact_type_4,contact_name_4,contact_phone_1_4,contact_mobile_4,contact_email_4,");
            sql.AppendFormat(@"contact_type_5,contact_name_5,contact_phone_1_5,contact_mobile_5,contact_email_5,");
            sql.AppendFormat(@"gigade_bunus_threshold,CONCAT(gigade_bunus_percent,'%') as gigade_bunus_percent ,procurement_days,self_send_days,stuff_ware_days,dispatch_days,CASE self_send_days WHEN 0 THEN '自出' ELSE '' end as vendor_mode,vendor_note ");
            sql.AppendFormat(@" from vendor LEFT JOIN manage_user on product_manage=user_id where 1=1  ");
            sql.AppendFormat(sqlwhere);
            sql.AppendFormat(@"  order by vendor_id");
            return _dbAccess.getDataTable(sql.ToString());
        }
        #endregion



        /// <summary>
        /// 獲取一條供應商信息
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <returns>供應商信息對象</returns>
        public Vendor GetSingle(Vendor query)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                strSql.Append(" select vendor_id,vendor_code,vendor_status,vendor_email,vendor_password,vendor_name_full,vendor_name_simple,vendor_invoice,");
                strSql.Append("company_phone,company_fax,company_person,company_zip,company_address,invoice_zip,invoice_address,contact_type_1,contact_name_1,contact_phone_1_1,");
                strSql.Append("contact_phone_2_1,contact_mobile_1,contact_email_1,contact_type_2,contact_name_2,contact_phone_1_2,contact_phone_2_2,contact_mobile_2,contact_email_2,");
                strSql.Append("contact_type_3,contact_name_3,contact_phone_1_3,contact_phone_2_3,contact_mobile_3,contact_email_3,contact_type_4,contact_name_4,contact_phone_1_4,");
                strSql.Append("contact_phone_2_4,contact_mobile_4,contact_email_4,contact_type_5,contact_name_5,contact_phone_1_5,contact_phone_2_5,contact_mobile_5,contact_email_5,");
                strSql.Append("cost_percent,creditcard_1_percent,creditcard_3_percent,sales_limit,bonus_percent,agreement_createdate,agreement_start,agreement_end,checkout_type,");
                strSql.Append("checkout_other,bank_code,bank_name,bank_number,bank_account,freight_low_limit,freight_low_money,freight_normal_limit,freight_normal_money,erp_id,");
                strSql.Append("freight_return_low_money,freight_return_normal_money,vendor_note,vendor_confirm_code,vendor_login_attempts,assist,dispatch,product_mode,");//新增字段kuser、kdate保存供應商建立信息 add by shuangshuang0420j 20150624 10:15
                strSql.Append("product_manage,gigade_bunus_percent,gigade_bunus_threshold,procurement_days,self_send_days,stuff_ware_days,dispatch_days,vendor_type,kuser,kdate from vendor where 1=1");//新增字段procurement_days,self_send_days,stuff_ware_days,dispatch_days add by shuangshuang0420j 20150323 10:15
                if (query.vendor_id != 0)
                {
                    strSql.AppendFormat(" and vendor_id={0}", query.vendor_id);
                }
                if (!string.IsNullOrEmpty(query.vendor_email))
                {
                    strSql.AppendFormat(" and vendor_email='{0}'", query.vendor_email);
                }
                if (!string.IsNullOrEmpty(query.erp_id))
                {
                    strSql.AppendFormat(" and erp_id='{0}'", query.erp_id);
                }

                return _dbAccess.getSinggleObj<Vendor>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" vendorDao-->GetSingle-->" + ex.Message + strSql.ToString(), ex);
            }
        }

        #region 根據供應商編號查詢最新登陸的記錄login_id+string GetLoginId(int vendorid)
        /// <summary>
        /// 根據供應商編號查詢最新登陸的記錄login_id
        /// </summary>
        /// <param name="vendorid">供應商編號</param>
        /// <returns>最新登陸的記錄login_id</returns>
        public string GetLoginId(int vendorid)
        {
            string strSql = string.Format("select login_id login_id from vendor_login where vendor_id={0} order by login_createdate desc limit 1", vendorid);
            DataTable Db = _dbAccess.getDataTable(strSql);
            if (Db.Rows.Count > 0)
            {
                return Db.Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region 獲取所有供應商（用作下拉列表框）+List<Vendor> VendorQueryAll(Vendor query)
        /// <summary>
        /// 獲取所有未失格的供應商（用作下拉列表框）
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<Vendor> VendorQueryAll(Vendor query)
        {
            query.Replace4MySQL();
            StringBuilder sbSql = new StringBuilder();
            try
            {
                sbSql.Append(@" SELECT vendor_id,vendor_code,vendor_status,vendor_email,vendor_password,vendor_name_full,vendor_name_simple,vendor_invoice, 
                company_phone,company_fax,company_person,company_zip,company_address,invoice_zip,invoice_address,contact_type_1,contact_name_1,contact_phone_1_1,
                contact_phone_2_1,contact_mobile_1,contact_email_1,contact_type_2,contact_name_2,contact_phone_1_2,contact_phone_2_2,contact_mobile_2,contact_email_2,
                contact_type_3,contact_name_3,contact_phone_1_3,contact_phone_2_3,contact_mobile_3,contact_email_3,contact_type_4,contact_name_4,contact_phone_1_4,
                contact_phone_2_4,contact_mobile_4,contact_email_4,contact_type_5,contact_name_5,contact_phone_1_5,contact_phone_2_5,contact_mobile_5,contact_email_5,
                cost_percent,creditcard_1_percent,creditcard_3_percent,sales_limit,bonus_percent,agreement_createdate,agreement_start,agreement_end,checkout_type,
                checkout_other,bank_code,bank_name,bank_number,bank_account,freight_low_limit,freight_low_money,freight_normal_limit,freight_normal_money,
                freight_return_low_money,freight_return_normal_money,vendor_note,vendor_confirm_code,vendor_login_attempts,assist,dispatch,product_mode,
                product_manage,gigade_bunus_percent,gigade_bunus_threshold,procurement_days,self_send_days,stuff_ware_days,dispatch_days,vendor_type,kuser,kdate FROM vendor WHERE vendor_status!=3 ");
                if (query.assist != 0)
                {
                    sbSql.AppendFormat(" AND assist = 1 ");
                }

                sbSql.Append(" ORDER BY vendor_id ASC");
                return _dbAccess.getDataTableForObj<Vendor>(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" vendorDao-->VendorQueryAll-->" + ex.Message + sbSql.ToString(), ex);
            }
        }
        #endregion
        //chaojie天界與2015-07-09上面的用作供應商下拉列表，數據太多 可能導致延遲或數據加載不出
        public List<Vendor> VendorQueryList(Vendor query)
        {
            query.Replace4MySQL();
            StringBuilder sbSql = new StringBuilder();
            try
            {
                sbSql.Append(@" SELECT vendor_id,vendor_name_full,vendor_name_simple FROM vendor WHERE vendor_status!=3 ");
                if (query.assist != 0)
                {
                    sbSql.AppendFormat(" AND assist = 1 ");
                }

                sbSql.Append(" ORDER BY vendor_id ASC");
                return _dbAccess.getDataTableForObj<Vendor>(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" vendorDao-->VendorQueryList-->" + ex.Message + sbSql.ToString(), ex);
            }
        }
        //public int Update(VendorQuery model, string update_log)
        //{
        //    _userhistoryDao = new UserHistoryDao(connStr);
        //    _serialDao = new SerialDao(connStr);

        //    model.Replace4MySQL();
        //    int i = 0;
        //    MySqlCommand mySqlCmd = new MySqlCommand();
        //    MySqlConnection mySqlConn = new MySqlConnection(connStr);
        //    try
        //    {
        //        if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
        //        {
        //            mySqlConn.Open();
        //        }
        //        mySqlCmd.Connection = mySqlConn;
        //        mySqlCmd.Transaction = mySqlConn.BeginTransaction();
        //        mySqlCmd.CommandType = System.Data.CommandType.Text;

        //        model.content = ReturnHistoryCon(model).ToString();

        //        #region 處理vendor表
        //        mySqlCmd.CommandText = UpdateVendor(model);
        //        i += mySqlCmd.ExecuteNonQuery();
        //        #endregion
        //        #region 處理userhistory表
        //        mySqlCmd.CommandText += _userhistoryDao.Save(model);
        //        i += mySqlCmd.ExecuteNonQuery();
        //        #endregion

        //        #region 處理table_change_log 記錄供應商資料異動
        //        if (!string.IsNullOrEmpty(update_log))
        //        {
        //            update_log = update_log.TrimEnd('#');//去掉最後一個#
        //            string[] arr_log = update_log.Split('#');//分離每條記錄
        //            foreach (string item in arr_log)
        //            {

        //            }
        //        }
        //        #endregion

        //        mySqlCmd.Transaction.Commit();
        //    }
        //    catch (Exception ex)
        //    {
        //        mySqlCmd.Transaction.Rollback();
        //        throw new Exception("VendorDao-->Update-->" + ex.Message, ex);
        //    }
        //    finally
        //    {
        //        if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
        //        {
        //            mySqlConn.Close();
        //        }
        //    }
        //    return i;
        //}

        public int Add(VendorQuery model)
        {
            model.Replace4MySQL();
            int i = 0;
            StringBuilder sql = new StringBuilder();
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

                #region 獲取vendor_id
                mySqlCmd.CommandText = _serialDao.Update(10);
                sql.Append(mySqlCmd.CommandText);
                model.vendor_id = Convert.ToUInt32(mySqlCmd.ExecuteScalar());
                #endregion

                #region 獲取vendor_code
                mySqlCmd.CommandText = "select max(vendor_code)as vendor_code from vendor;";
                sql.Append(mySqlCmd.CommandText);
                string tempCode = mySqlCmd.ExecuteScalar().ToString();
                int nowYear = Convert.ToInt32(DateTime.Today.Year.ToString().Substring(2));
                int nowDays = DateTime.Today.DayOfYear;
                int nCode_4 = 1;
                if (!string.IsNullOrEmpty(tempCode))
                {
                    int tCode_2 = Convert.ToInt32(tempCode.Substring(1, 2));
                    int tCode_3 = Convert.ToInt32(tempCode.Substring(3, 3));
                    int tCode_4 = Convert.ToInt32(tempCode.Substring(6));
                    if (tCode_2 == nowYear && tCode_3 == nowDays)
                    {
                        nCode_4 = tCode_4 + 1;
                    }
                }
                string tYear = StrPad_Left(nowYear.ToString(), 2);
                string tDays = StrPad_Left(nowDays.ToString(), 3);

                model.vendor_code = "V" + tYear + tDays + StrPad_Left(nCode_4.ToString(), 4);
                #endregion

                model.content = ReturnHistoryCon(model).ToString();

                #region 獲取erp_id
                if (!string.IsNullOrEmpty(model.prod_cate) && !string.IsNullOrEmpty(model.buy_cate)
                   && !string.IsNullOrEmpty(model.tax_type)
                   && model.buy_cate.StartsWith(model.prod_cate))
                {
                    //獲取serial_id;
                    Serial serModel = _serialDao.GetSerialById(77);
                    if (serModel == null)
                    {
                        serModel = new Serial();
                        serModel.Serial_id = 77;
                        serModel.Serial_Value = 500;//默認從500開始
                        mySqlCmd.CommandText = _serialDao.InsertStr(serModel);
                        sql.Append(mySqlCmd.CommandText);
                        i += mySqlCmd.ExecuteNonQuery();
                    }
                    mySqlCmd.CommandText = _serialDao.Update(77);//77代表erp_id的serial
                    sql.Append(mySqlCmd.CommandText);
                    model.serial = mySqlCmd.ExecuteScalar().ToString();
                    if (!string.IsNullOrEmpty(model.serial))
                    {
                        model.erp_id = model.buy_cate + CommonFunction.Supply(model.serial, "0", 5) + model.tax_type;

                        if (IsExitErpID(model.erp_id) > 0)
                        {
                            i = -1;
                            return i;
                        }
                    }
                }
                #endregion

                #region 處理vendor表
                mySqlCmd.CommandText = InsertVendor(model);
                sql.Append(mySqlCmd.CommandText);
                i += mySqlCmd.ExecuteNonQuery();
                #endregion
               
                #region 處理userhistory表
                mySqlCmd.CommandText = _userhistoryDao.Save(model);
                sql.Append(mySqlCmd.CommandText);
                i += mySqlCmd.ExecuteNonQuery();
                #endregion


                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("VendorDao-->Add-->" + ex.Message + sql.ToString(), ex);
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

        public string ReturnHistoryCon(VendorQuery model)
        {
            model.Replace4MySQL();
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat(" vendor_status:'{0}',vendor_email:'{1}',vendor_password:'{2}',vendor_name_full:'{3}',vendor_name_simple:'{4}',vendor_invoice:'{5}', ", model.vendor_status, model.vendor_email, model.vendor_password, model.vendor_name_full, model.vendor_name_simple, model.vendor_invoice);
                strSql.AppendFormat("company_phone:'{0}',company_fax:'{1}',company_person:'{2}',company_zip:'{3}',company_address:'{4}',invoice_zip:'{5}',invoice_address:'{6}',contact_type_1:'{7}',contact_name_1:'{8}',contact_phone_1_1:'{9}',", model.company_phone, model.company_fax, model.company_person, model.company_zip, model.company_address, model.invoice_zip, model.invoice_address, model.contact_type_1, model.contact_name_1, model.contact_phone_1_1);
                strSql.AppendFormat("contact_phone_2_1:'{0}',contact_mobile_1:'{1}',contact_email_1:'{2}',contact_type_2:'{3}',contact_name_2:'{4}',contact_phone_1_2:'{5}',contact_phone_2_2:'{6}',contact_mobile_2:'{7}',contact_email_2:'{8}',", model.contact_phone_2_1, model.contact_mobile_1, model.contact_email_1, model.contact_type_2, model.contact_name_2, model.contact_phone_1_2, model.contact_phone_2_2, model.contact_mobile_2, model.contact_email_2);
                strSql.AppendFormat("contact_type_3:'{0}',contact_name_3:'{1}',contact_phone_1_3:'{2}',contact_phone_2_3:'{3}',contact_mobile_3:'{4}',contact_email_3:'{5}',contact_type_4:'{6}',contact_name_4:'{7}',contact_phone_1_4:'{8}',", model.contact_type_3, model.contact_name_3, model.contact_phone_1_3, model.contact_phone_2_3, model.contact_mobile_3, model.contact_email_3, model.contact_type_4, model.contact_name_4, model.contact_phone_1_4);
                strSql.AppendFormat("contact_phone_2_4:'{0}',contact_mobile_4:'{1}',contact_email_4:'{2}',contact_type_5:'{3}',contact_name_5:'{4}',contact_phone_1_5:'{5}',contact_phone_2_5:'{6}',contact_mobile_5:'{7}',contact_email_5:'{8}',", model.contact_phone_2_4, model.contact_mobile_4, model.contact_email_4, model.contact_type_5, model.contact_name_5, model.contact_phone_1_5, model.contact_phone_2_5, model.contact_mobile_5, model.contact_email_5);
                strSql.AppendFormat("cost_percent:'{0}',creditcard_1_percent:'{1}',creditcard_3_percent:'{2}',sales_limit:'{3}',bonus_percent:'{4}',agreement_createdate:'{5}',agreement_start:'{6}',agreement_end:'{7}',checkout_type:'{8}',", model.cost_percent, model.creditcard_1_percent, model.creditcard_3_percent, model.sales_limit, model.bonus_percent, model.agreement_createdate, model.agreement_start, model.agreement_end, model.checkout_type);
                strSql.AppendFormat("checkout_other:'{0}',bank_code:'{1}',bank_name:'{2}',bank_number:'{3}',bank_account:'{4}',freight_low_limit:'{5}',freight_low_money:'{6}',freight_normal_limit:'{7}',freight_normal_money:'{8}',",
                     model.checkout_other, model.bank_code, model.bank_name, model.bank_number, model.bank_account, model.freight_low_limit, model.freight_low_money, model.freight_normal_limit, model.freight_normal_money);
                strSql.AppendFormat("freight_return_low_money:'{0}',freight_return_normal_money:'{1}',vendor_note:'{2}',vendor_confirm_code:'{3}',vendor_login_attempts:'{4}',assist:'{5}',dispatch:'{6}',product_mode:'{7}',", model.freight_return_low_money, model.freight_return_normal_money, model.vendor_note, model.vendor_confirm_code, model.vendor_login_attempts, model.assist, model.dispatch, model.product_mode);
                strSql.AppendFormat("product_manage:'{0}',gigade_bunus_percent:'{1}',gigade_bunus_threshold :'{2}'", model.product_manage, model.gigade_bunus_percent, model.gigade_bunus_threshold);
                strSql.AppendFormat(",procurement_days:'{0}',self_send_days:'{1}',stuff_ware_days:'{2}',dispatch_days:'{3}'", model.procurement_days, model.self_send_days, model.stuff_ware_days, model.dispatch_days);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(" VendorDao-->ReturnHistoryCon-->" + ex.Message + strSql.ToString(), ex);
            }
        }

        public string UpdateVendor(Vendor model)
        {
            model.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("");
            try
            {
                strSql.AppendFormat("update  vendor set vendor_code='{0}',vendor_status='{1}',vendor_email='{2}',vendor_name_full='{3}',vendor_name_simple='{4}',vendor_invoice='{5}', ",
                    model.vendor_code, model.vendor_status, model.vendor_email, model.vendor_name_full, model.vendor_name_simple, model.vendor_invoice);
                strSql.AppendFormat("company_phone='{0}',company_fax='{1}',company_person='{2}',company_zip='{3}',company_address='{4}',invoice_zip='{5}',invoice_address='{6}',contact_type_1='{7}',contact_name_1='{8}',contact_phone_1_1='{9}',",
                     model.company_phone, model.company_fax, model.company_person, model.company_zip, model.company_address, model.invoice_zip, model.invoice_address, model.contact_type_1, model.contact_name_1, model.contact_phone_1_1);
                strSql.AppendFormat("contact_phone_2_1='{0}',contact_mobile_1='{1}',contact_email_1='{2}',contact_type_2='{3}',contact_name_2='{4}',contact_phone_1_2='{5}',contact_phone_2_2='{6}',contact_mobile_2='{7}',contact_email_2='{8}',",
                     model.contact_phone_2_1, model.contact_mobile_1, model.contact_email_1, model.contact_type_2, model.contact_name_2, model.contact_phone_1_2, model.contact_phone_2_2, model.contact_mobile_2, model.contact_email_2);
                strSql.AppendFormat("contact_type_3='{0}',contact_name_3='{1}',contact_phone_1_3='{2}',contact_phone_2_3='{3}',contact_mobile_3='{4}',contact_email_3='{5}',contact_type_4='{6}',contact_name_4='{7}',contact_phone_1_4='{8}',",
                    model.contact_type_3, model.contact_name_3, model.contact_phone_1_3, model.contact_phone_2_3, model.contact_mobile_3, model.contact_email_3, model.contact_type_4, model.contact_name_4, model.contact_phone_1_4);
                strSql.AppendFormat("contact_phone_2_4='{0}',contact_mobile_4='{1}',contact_email_4='{2}',contact_type_5='{3}',contact_name_5='{4}',contact_phone_1_5='{5}',contact_phone_2_5='{6}',contact_mobile_5='{7}',contact_email_5='{8}',",
                    model.contact_phone_2_4, model.contact_mobile_4, model.contact_email_4, model.contact_type_5, model.contact_name_5, model.contact_phone_1_5, model.contact_phone_2_5, model.contact_mobile_5, model.contact_email_5);
                strSql.AppendFormat("cost_percent='{0}',creditcard_1_percent='{1}',creditcard_3_percent='{2}',sales_limit='{3}',bonus_percent='{4}',agreement_createdate='{5}',agreement_start='{6}',agreement_end='{7}',checkout_type='{8}',",
                     model.cost_percent, model.creditcard_1_percent, model.creditcard_3_percent, model.sales_limit, model.bonus_percent, model.agreement_createdate, model.agreement_start, model.agreement_end, model.checkout_type);
                strSql.AppendFormat("checkout_other='{0}',bank_code='{1}',bank_name='{2}',bank_number='{3}',bank_account='{4}',freight_low_limit='{5}',freight_low_money='{6}',freight_normal_limit='{7}',freight_normal_money='{8}',",
                    model.checkout_other, model.bank_code, model.bank_name, model.bank_number, model.bank_account, model.freight_low_limit, model.freight_low_money, model.freight_normal_limit, model.freight_normal_money);
                strSql.AppendFormat("freight_return_low_money='{0}',freight_return_normal_money='{1}',vendor_note='{2}',vendor_confirm_code='{3}',vendor_login_attempts='{4}',assist='{5}',dispatch='{6}',product_mode='{7}',",
                    model.freight_return_low_money, model.freight_return_normal_money, model.vendor_note, model.vendor_confirm_code, model.vendor_login_attempts, model.assist, model.dispatch, model.product_mode);
                strSql.AppendFormat("procurement_days='{0}',self_send_days='{1}',stuff_ware_days='{2}',dispatch_days='{3}',", model.procurement_days, model.self_send_days, model.stuff_ware_days, model.dispatch_days);
                strSql.AppendFormat("vendor_type='{0}',", model.vendor_type);//供應商類型
                strSql.AppendFormat("product_manage='{0}',gigade_bunus_percent='{1}',gigade_bunus_threshold='{2}',erp_id='{3}',export_flag='{4}' where vendor_id='{5}';",
                    model.product_manage, model.gigade_bunus_percent, model.gigade_bunus_threshold, model.erp_id, model.export_flag, model.vendor_id);

                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("VendorDao-->UpdateVendor-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        public string InsertVendor(Vendor model)
        {
            model.Replace4MySQL();
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("insert into vendor( vendor_id,vendor_code,vendor_status,vendor_email,vendor_password,vendor_name_full,vendor_name_simple,vendor_invoice, ");
                strSql.Append("company_phone,company_fax,company_person,company_zip,company_address,invoice_zip,invoice_address,contact_type_1,contact_name_1,contact_phone_1_1,");
                strSql.Append("contact_phone_2_1,contact_mobile_1,contact_email_1,contact_type_2,contact_name_2,contact_phone_1_2,contact_phone_2_2,contact_mobile_2,contact_email_2,");
                strSql.Append("contact_type_3,contact_name_3,contact_phone_1_3,contact_phone_2_3,contact_mobile_3,contact_email_3,contact_type_4,contact_name_4,contact_phone_1_4,");
                strSql.Append("contact_phone_2_4,contact_mobile_4,contact_email_4,contact_type_5,contact_name_5,contact_phone_1_5,contact_phone_2_5,contact_mobile_5,contact_email_5,");
                strSql.Append("cost_percent,creditcard_1_percent,creditcard_3_percent,sales_limit,bonus_percent,agreement_createdate,agreement_start,agreement_end,checkout_type,");
                strSql.Append("checkout_other,bank_code,bank_name,bank_number,bank_account,freight_low_limit,freight_low_money,freight_normal_limit,freight_normal_money,");
                strSql.Append("freight_return_low_money,freight_return_normal_money,vendor_note,vendor_confirm_code,vendor_login_attempts,assist,dispatch,product_mode,");
                strSql.Append("product_manage,erp_id,gigade_bunus_percent,gigade_bunus_threshold, procurement_days,self_send_days,stuff_ware_days,dispatch_days,export_flag,vendor_type,kuser,kdate)");//vendor_type供應商類型
                strSql.AppendFormat(" values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',",
                    model.vendor_id, model.vendor_code, model.vendor_status, model.vendor_email, model.vendor_password, model.vendor_name_full, model.vendor_name_simple, model.vendor_invoice);
                strSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',",
                    model.company_phone, model.company_fax, model.company_person, model.company_zip, model.company_address, model.invoice_zip, model.invoice_address, model.contact_type_1, model.contact_name_1, model.contact_phone_1_1);
                strSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',",
                    model.contact_phone_2_1, model.contact_mobile_1, model.contact_email_1, model.contact_type_2, model.contact_name_2, model.contact_phone_1_2, model.contact_phone_2_2, model.contact_mobile_2, model.contact_email_2);
                strSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',",
                    model.contact_type_3, model.contact_name_3, model.contact_phone_1_3, model.contact_phone_2_3, model.contact_mobile_3, model.contact_email_3, model.contact_type_4, model.contact_name_4, model.contact_phone_1_4);
                strSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',",
                    model.contact_phone_2_4, model.contact_mobile_4, model.contact_email_4, model.contact_type_5, model.contact_name_5, model.contact_phone_1_5, model.contact_phone_2_5, model.contact_mobile_5, model.contact_email_5);
                strSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',",
                     model.cost_percent, model.creditcard_1_percent, model.creditcard_3_percent, model.sales_limit, model.bonus_percent, model.agreement_createdate, model.agreement_start, model.agreement_end, model.checkout_type);
                strSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',",
                    model.checkout_other, model.bank_code, model.bank_name, model.bank_number, model.bank_account, model.freight_low_limit, model.freight_low_money, model.freight_normal_limit, model.freight_normal_money);
                strSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',",
                    model.freight_return_low_money, model.freight_return_normal_money, model.vendor_note, model.vendor_confirm_code, model.vendor_login_attempts, model.assist, model.dispatch, model.product_mode);
                strSql.AppendFormat("'{0}','{1}','{2}','{3}',", model.product_manage, model.erp_id, model.gigade_bunus_percent, model.gigade_bunus_threshold);
                strSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}');", model.procurement_days, model.self_send_days, model.stuff_ware_days, model.dispatch_days, model.export_flag, model.vendor_type, model.kuser, CommonFunction.DateTimeToString(model.kdate));
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(" vendorDao-->InsertVendor-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        /// <summary>
        /// 補充字符至指定長度
        /// </summary>
        /// <param name="input"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public string StrPad_Left(string input, int len)
        {
            string result = string.Empty;
            int sLen = len - input.Length;
            if (sLen > 0)
            {
                for (int i = 0; i < sLen; i++)
                {
                    result += "0";
                }
            }
            result += input;
            return result;
        }

        public int IsExitEmail(string email)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                int total = 0;
                strSql.AppendFormat(" select count(vendor_id) as total from vendor where  vendor_email ='{0}'", email);
                DataTable _dt = _dbAccess.getDataTable(strSql.ToString());
                if (_dt.Rows.Count > 0)
                {
                    total = Convert.ToInt32(_dt.Rows[0]["total"].ToString());
                }
                return total;
            }
            catch (Exception ex)
            {
                throw new Exception(" vendorDao-->IsExitEmail-->" + ex.Message + strSql.ToString(), ex);
            }
        }

        public int IsExitErpID(string erp_id)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                int total = 0;
                strSql.AppendFormat(" select count(vendor_id) as total from vendor where  erp_id ='{0}'", erp_id);
                DataTable _dt = _dbAccess.getDataTable(strSql.ToString());
                if (_dt.Rows.Count > 0)
                {
                    total = Convert.ToInt32(_dt.Rows[0]["total"].ToString());
                }
                return total;
            }
            catch (Exception ex)
            {
                throw new Exception(" vendorDao-->IsExitEmail-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #region 供應商登錄查詢+Vendor Login(Vendor query)
        /// <summary>
        /// 供應商登錄查詢
        /// </summary>
        /// <param name="vendor_email"></param>
        /// <returns></returns>
        //public Vendor Login(Vendor query)
        //{
        //    string strSql = string.Empty;
        //    try
        //    {
        //        strSql = string.Format(" select * from vendor where vendor_email='{0}'", query.vendor_email);
        //        return mysqlHelp.getSinggleObj<VendorQuery>(strSql);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(" vendorDao-->InsertVendor-->" + ex.Message + strSql.ToString(), ex);
        //    }
        //}
        #endregion

        #region 更新錯誤次數+int Add_Login_Attempts(int vendor_id)
        /// <summary>
        /// 更新錯誤次數
        /// </summary>
        /// <param name="vendor_id"></param>
        /// <returns></returns>
        public int Add_Login_Attempts(int vendor_id)
        {
            string strSql = string.Empty;
            try
            {
                strSql = string.Format("set sql_safe_updates = 0;update vendor set vendor_login_attempts = vendor_login_attempts + 1 where vendor_id = '{0}';set sql_safe_updates = 1;", vendor_id);
                return _dbAccess.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" vendorDao-->Add_Login_Attempts-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        #region 更改供應商賬號狀態+void Modify_Vendor_Status(int vendor_id, int status)
        /// <summary>
        /// 更改供應商賬號狀態
        /// </summary>
        /// <param name="vendor_id"></param>
        /// <param name="status"></param>
        public void Modify_Vendor_Status(int vendor_id, int status)
        {
            string strSql = string.Empty;
            try
            {
                strSql = string.Format("set sql_safe_updates = 0; update vendor set vendor_status = '{0}',vendor_confirm_code = '' where vendor_id = '{1}';set sql_safe_updates = 1;", status, vendor_id);
                _dbAccess.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" vendorDao-->Modify_Vendor_Status-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        #region 異動/修改使用者的確認碼,用來做變更密碼判斷用,並非更改使用者的登入密碼+void Modify_User_Confirm_Code(int vendor_id, string vendor_confirm_code)
        /// <summary>
        /// 異動/修改使用者的確認碼,用來做變更密碼判斷用,並非更改使用者的登入密碼
        /// </summary>
        /// <param name="vendor_id">使用者編號</param>
        /// <param name="vendor_confirm_code">使用者確認碼</param>
        public void Modify_Vendor_Confirm_Code(int vendor_id, string vendor_confirm_code)
        {
            string strSql = string.Empty;
            try
            {
                strSql = string.Format("update vendor set vendor_confirm_code = '{0}' where vendor_id={1}", vendor_confirm_code, vendor_id);
                _dbAccess.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" vendorDao-->Modify_Vendor_Confirm_Code-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        public int EditPass(string vendorId, string newPass)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                if (!string.IsNullOrEmpty(vendorId))
                {
                    sql.AppendFormat("update vendor set vendor_password='{0}' where vendor_id='{1}' ;", newPass, vendorId);
                }
                return _dbAccess.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" vendorDao-->EditPass-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public List<ManageUser> GetVendorPM()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("SELECT product_manage 'user_id' ,user_username, user_email FROM vendor ");
                sql.Append("INNER JOIN manage_user ON user_id = product_manage");
                sql.Append(" GROUP BY product_manage ");

                return _dbAccess.getDataTableForObj<ManageUser>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" vendorDao-->GetVendorPM-->" + ex.Message + sql.ToString(), ex);
            }
        }

        /// <summary>
        /// 根據供應商獲取其下未失格的和狀態不是下架和下架不販售狀態的商品的個數
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        public int GetOffGradeCount(string vendorId)
        {
            StringBuilder sql = new StringBuilder();
            try
            {//6:下架 99：下架不販售
                sql.AppendFormat(@"SELECT count(p.product_id) as cou from vendor_brand vb 
inner join product p on vb.vendor_id='{0}' and p.product_id>=10000 and combination<>0 and p.brand_id=vb.brand_id
where p.off_grade<> 1 or p.product_status not in(6,99);", vendorId);

                DataTable _dt = _dbAccess.getDataTable(sql.ToString());
                int cou = 0;
                if (_dt.Rows.Count > 0)
                {
                    cou = Convert.ToInt32(_dt.Rows[0]["cou"]);
                }
                return cou;
            }

            catch (Exception ex)
            {
                throw new Exception(" vendorDao-->GetOffGradeCount-->" + ex.Message + sql.ToString(), ex);
            }


        }

        public int UnGrade(string vendorId, string active)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates = 0;update vendor set vendor_status ='{0}' where vendor_id = '{1}';set sql_safe_updates = 1;", active, vendorId);
                return _dbAccess.execCommand(sql.ToString());
            }

            catch (Exception ex)
            {
                throw new Exception(" vendorDao-->UnGrade-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public List<Vendor> GetArrayDaysInfo(uint brand_id)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SELECT v.self_send_days,v.stuff_ware_days,v.dispatch_days 
                                      FROM vendor v
                                  INNER JOIN vendor_brand vb ON vb.vendor_id = v.vendor_id
                                  WHERE vb.brand_id = {0}", brand_id);
                return _dbAccess.getDataTableForObj<Vendor>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("vendorDao.GetArrayDaysInfo" + ex.Message, ex);
            }
        }

    }
}