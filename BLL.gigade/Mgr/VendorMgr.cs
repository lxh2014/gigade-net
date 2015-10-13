/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：VendorMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2014/1/14 13:48:38 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using System.Data;
using BLL.gigade.Model.Query;
using System.Collections;
using MySql.Data.MySqlClient;
namespace BLL.gigade.Mgr
{
    public class VendorMgr : IVendorImplMgr
    {
        private IVendorImplDao _vendorDao;
        private string connStr;
        private MySqlDao _mysqlDao;
        public VendorMgr(string connectionStr)
        {
            _vendorDao = new VendorDao(connectionStr);
            _mysqlDao = new MySqlDao(connectionStr);
            connStr = connectionStr;
        }

        public List<VendorQuery> Query(VendorQuery query, ref int totalCount)
        {
            try
            {
                return _vendorDao.Query(query, ref totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorMgr-->Query-->" + ex.Message, ex);
            }
        }
        public DataTable GetVendorDetail(string sqlwhere)
        {
            try
            {
                return _vendorDao.GetVendorDetail(sqlwhere);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorMgr-->GetVendorDetail-->" + ex.Message, ex);
            }
        }

        public Vendor GetSingle(Vendor query)
        {
            try
            {
                return _vendorDao.GetSingle(query);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorMgr-->GetSingle-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 根據供應商編號查詢最新登陸的記錄login_id
        /// </summary>
        /// <param name="vendorid">供應商編號</param>
        /// <returns>最新登陸的記錄login_id</returns>
        public string GetLoginId(int vendorid)
        {
            try
            {
                return _vendorDao.GetLoginId(vendorid);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorMgr-->GetLoginId-->" + ex.Message, ex);
            }
        }

        public int UnGrade(string vendorId, string active, List<TableChangeLog> list)
        {
            try
            {
                #region 處理table_change_log 記錄供應商資料異動
                ArrayList _list = new ArrayList();
                //if (!string.IsNullOrEmpty(update_log))
                //{
                //   
                //    TableChangeLogDao _logDao = new TableChangeLogDao(connStr);
                //    string[] arr_col = update_log.Split(':');
                //    TableChangeLog tcl = new TableChangeLog();
                //    tcl.change_table = "vendor";
                //    tcl.change_field = arr_col[0].ToString();
                //    tcl.field_ch_name = arr_col[3].ToString();
                //    tcl.create_time = DateTime.Now;
                //    tcl.create_user = Convert.ToInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                //    tcl.pk_id = Convert.ToInt32(vendorId);
                //    tcl.old_value = arr_col[1].ToString();
                //    tcl.new_value = arr_col[2].ToString();
                //    tcl.user_type = 2;
                //    _list.Add(_logDao.insert(tcl));
                //    _mysqlDao.ExcuteSqls(_list);
                //}
                if (list != null)
                {
                    foreach (TableChangeLog t in list)
                    {
                        TableChangeLogDao _logDao = new TableChangeLogDao(connStr);
                        t.change_table = "vendor";
                        _list.Add(_logDao.insert(t));
                    }
                }
                #endregion
                return _vendorDao.UnGrade(vendorId, active);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorMgr-->UnGrade-->" + ex.Message, ex);
            }
        }
        public List<Vendor> VendorQueryAll(Vendor query)
        {
            try
            {
                return _vendorDao.VendorQueryAll(query);
            }
            catch (Exception ex)
            {

                throw new Exception(" VendorMgr-->VendorQueryAll-->" + ex.Message, ex);
            }

        }
        public List<Vendor> VendorQueryList(Vendor query)
        {
            try
            {
                return _vendorDao.VendorQueryList(query);
            }
            catch (Exception ex)
            {

                throw new Exception(" VendorMgr-->VendorQueryList-->" + ex.Message, ex);
            }
        }

        public int Add(Model.Query.VendorQuery model)
        {
            try
            {
                return _vendorDao.Add(model);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorMgr-->Add-->" + ex.Message, ex);
            }
        }

        public int Update(VendorQuery model, List<TableChangeLog> list)
        {
            UserHistoryDao _userhistoryDao = new UserHistoryDao(connStr);
            SerialDao _serialDao = new SerialDao(connStr);
            model.Replace4MySQL();
            int i = 0;

            try
            {
                ArrayList _list = new ArrayList();

                model.content = _vendorDao.ReturnHistoryCon(model).ToString();

                #region 處理vendor表
                _list.Add(_vendorDao.UpdateVendor(model));

                #endregion
                #region 處理userhistory表
                _list.Add(_userhistoryDao.Save(model));

                #endregion

                #region 處理table_change_log 記錄供應商資料異動
                //if (!string.IsNullOrEmpty(update_log))
                //{
                //    update_log = update_log.TrimEnd('#');//去掉最後一個#
                //    string[] arr_log = update_log.Split('#');//分離每條記錄
                //    foreach (string item in arr_log)
                //    {
                //        TableChangeLogDao _logDao = new TableChangeLogDao(connStr);
                //        string[] arr_col = item.Split(':');
                //        TableChangeLog tcl = new TableChangeLog();
                //        tcl.change_table = "vendor";
                //        tcl.change_field = arr_col[0].ToString();
                //        tcl.field_ch_name = arr_col[3].ToString();
                //        tcl.create_time = model.created;
                //        tcl.create_user = (int)model.kuser_id;
                //        tcl.pk_id = (int)model.vendor_id;
                //        tcl.old_value = arr_col[1].ToString();
                //        tcl.new_value = arr_col[2].ToString();
                //        tcl.user_type = model.user_type;
                //        _list.Add(_logDao.insert(tcl));
                //    }
                //}
                if (list != null)
                {
                    foreach (TableChangeLog t in list)
                    {
                        TableChangeLogDao _logDao = new TableChangeLogDao(connStr);
                        t.change_table = "vendor";
                        t.create_time = model.created;
                        t.create_user = (int)model.kuser_id;
                        t.pk_id = (int)model.vendor_id;
                        t.user_type = model.user_type;
                        _list.Add(_logDao.insert(t));
                    }
                }
                #endregion

                if (_mysqlDao.ExcuteSqls(_list))
                {
                    i = 1;
                }
                return i;
            }
            catch (Exception ex)
            {

                throw new Exception("VendorDao-->Update-->" + ex.Message, ex);
            }
        }

        public string QueryContanct(Vendor query)
        {
            try
            {
                Vendor ven = _vendorDao.GetSingle(query);

                StringBuilder stb = new StringBuilder();
                stb.Append("{");
                stb.Append(string.Format("success:true,items:["));
                stb.Append("{");
                stb.Append(string.Format("\"contact_type\":\"{0}\",\"contact_name\":\"{1}\",\"contact_phone1\":\"{2}\",\"contact_phone2\":\"{3}\",\"contact_mobile\":\"{4}\",\"contact_email\":\"{5}\"", ven.contact_type_1, ven.contact_name_1, ven.contact_phone_1_1, ven.contact_phone_2_1, ven.contact_mobile_1, ven.contact_email_1));
                stb.Append("}");
                stb.Append(string.Format("\"contact_type\":\"{0}\",\"contact_name\":\"{1}\",\"contact_phone1\":\"{2}\",\"contact_phone2\":\"{3}\",\"contact_mobile\":\"{4}\",\"contact_email\":\"{5}\"", ven.contact_type_2, ven.contact_name_2, ven.contact_phone_1_2, ven.contact_phone_2_2, ven.contact_mobile_2, ven.contact_email_2));
                stb.Append("}");
                stb.Append(string.Format("\"contact_type\":\"{0}\",\"contact_name\":\"{1}\",\"contact_phone1\":\"{2}\",\"contact_phone2\":\"{3}\",\"contact_mobile\":\"{4}\",\"contact_email\":\"{5}\"", ven.contact_type_3, ven.contact_name_3, ven.contact_phone_1_3, ven.contact_phone_2_3, ven.contact_mobile_3, ven.contact_email_3));
                stb.Append("}");
                stb.Append(string.Format("\"contact_type\":\"{0}\",\"contact_name\":\"{1}\",\"contact_phone1\":\"{2}\",\"contact_phone2\":\"{3}\",\"contact_mobile\":\"{4}\",\"contact_email\":\"{5}\"", ven.contact_type_4, ven.contact_name_4, ven.contact_phone_1_4, ven.contact_phone_2_4, ven.contact_mobile_4, ven.contact_email_4));
                stb.Append("}");
                stb.Append(string.Format("\"contact_type\":\"{0}\",\"contact_name\":\"{1}\",\"contact_phone1\":\"{2}\",\"contact_phone2\":\"{3}\",\"contact_mobile\":\"{4}\",\"contact_email\":\"{5}\"", ven.contact_type_5, ven.contact_name_5, ven.contact_phone_1_5, ven.contact_phone_2_5, ven.contact_mobile_5, ven.contact_email_5));
                stb.Append("}");


                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                throw new Exception(" VendorMgr-->QueryContanct-->" + ex.Message, ex);
            }
        }

        public int IsExitEmail(string email)
        {
            try
            {
                return _vendorDao.IsExitEmail(email);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorMgr-->IsExitEmail-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 供應商登錄查詢
        /// </summary>
        /// <param name="vendor_email"></param>
        /// <returns></returns>
        public Vendor Login(Vendor query)
        {
            try
            {
                return _vendorDao.GetSingle(query);
            }
            catch (Exception ex)
            {

                throw; throw new Exception(" VendorMgr-->Login-->" + ex.Message, ex);
            }
        }

        public int Add_Login_Attempts(int vendor_id)
        {
            try
            {
                return _vendorDao.Add_Login_Attempts(vendor_id);
            }
            catch (Exception ex)
            {

                throw; throw new Exception(" VendorMgr-->Add_Login_Attempts-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 更改供應商賬號狀態
        /// </summary>
        /// <param name="vendor_id"></param>
        /// <param name="status"></param>
        public void Modify_Vendor_Status(int vendor_id, int status)
        {
            try
            {
                _vendorDao.Modify_Vendor_Status(vendor_id, status);
            }
            catch (Exception ex)
            {

                throw; throw new Exception(" VendorMgr-->Add_Login_Attempts-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 異動/修改使用者的確認碼,用來做變更密碼判斷用,並非更改使用者的登入密碼
        /// </summary>
        /// <param name="vendor_id">使用者編號</param>
        /// <param name="vendor_confirm_code">使用者確認碼</param>
        public void Modify_Vendor_Confirm_Code(int vendor_id, string vendor_confirm_code)
        {
            try
            {
                _vendorDao.Modify_Vendor_Confirm_Code(vendor_id, vendor_confirm_code);
            }
            catch (Exception ex)
            {

                throw; throw new Exception(" VendorMgr-->Modify_Vendor_Confirm_Code-->" + ex.Message, ex);
            }
        }

        public int EditPass(string vendorId, string newPass)
        {
            try
            {
                return _vendorDao.EditPass(vendorId, newPass);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorMgr-->EditPass-->" + ex.Message, ex);
            }
        }
        public List<ManageUser> GetVendorPM()
        {
            try
            {
                return _vendorDao.GetVendorPM();
            }
            catch (Exception ex)
            {
                throw new Exception("VendorMgr-->GetVendorPM-->" + ex.Message, ex);
            }
        }

        public int GetOffGradeCount(string vendorId)
        {
            try
            {
                return _vendorDao.GetOffGradeCount(vendorId);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorMgr-->GetOffGradeCount-->" + ex.Message, ex);
            }
        }

        public List<Vendor> GetArrayDaysInfo(uint brand_id)
        {
            try
            {
                return _vendorDao.GetArrayDaysInfo(brand_id);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorMgr-->GetArrayDaysInfo" + ex.Message, ex);
            }
        }

        #region 供應商銀行信息
        public List<VendorBank> QueryBank(VendorBank query, ref int totalCount)
        {
            try
            {
                return _vendorDao.QueryBank(query, ref totalCount);
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Number.ToString() + ":VendorMgr-->QueryBank-->" + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorMgr-->QueryBank" + ex.Message, ex);
            }
        }

        public string ImportVendorBank(DataTable dt)
        {
            try
            {
                string result = "";
                uint user_id = Convert.ToUInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id);
                ArrayList _list = new ArrayList();
                foreach (DataRow dr in dt.Rows)
                {
                    VendorBank model = new VendorBank();
                    model.bank_code = dr[0].ToString();
                    model.bank_name = dr[1].ToString();
                    model.kuser = user_id;
                    model.kdate = DateTime.Now;
                    model.muser = model.kuser;
                    model.mdate = model.kdate;
                    model.status = 1;//默認啟用
                    List<VendorBank> stores = _vendorDao.GetVendorBank(model);
                    if (stores.Count == 0)
                    {
                        _list.Add(_vendorDao.SaveBank(model));
                    }
                    else
                    {
                        result += model.bank_code + ',';
                    }
                }
                _mysqlDao.ExcuteSqlsThrowException(_list);
                return result.TrimEnd(',');
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Number.ToString() + ":VendorMgr-->ImportVendorBank-->" + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorMgr-->ImportVendorBank" + ex.Message, ex);
            }
        }
        public int SaveVendorBank(VendorBank model)
        {
            try
            {
                List<VendorBank> _list = _vendorDao.GetVendorBank(model);
                if (model.id == 0)//新增
                {
                    //判定該代碼或名稱是否存在
                    if (_list.Count > 0)
                    {
                        return -1;//提示該代碼或名稱已存在,不可重複保存！
                    }
                    else
                    {
                        model.kuser = model.muser;
                        model.kdate = model.mdate;
                        ArrayList arr = new ArrayList();
                        arr.Add(_vendorDao.SaveBank(model));
                        if (_mysqlDao.ExcuteSqls(arr))
                        {
                            return 1;//新增成功！
                        }
                        else
                        {
                            return 0;
                        }
                    }

                }
                else//編輯
                {
                    if (_list.Count > 1)
                    {
                        return -1;//提示該代碼或名稱已存在,不可重複保存！
                    }
                    else
                    {
                        return _vendorDao.UpdateBank(model);
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Number.ToString() + ":VendorMgr-->SaveVendorBank-->" + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorMgr-->SaveVendorBank" + ex.Message, ex);
            }
        }


        public int UpdateActiveBank(VendorBank model)
        {
            try
            {
                return _vendorDao.UpdateActiveBank(model);
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Number.ToString() + ":VendorMgr-->UpdateActiveBank-->" + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorMgr-->UpdateActiveBank" + ex.Message, ex);
            }
        }

        public List<VendorBank> GetVendorBank(VendorBank model)
        {
            try
            {
                return _vendorDao.GetVendorBank(model);
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Number.ToString() + ":VendorMgr-->GetVendorBank-->" + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorMgr-->GetVendorBank" + ex.Message, ex);
            }
        }
        #endregion
    }
}
