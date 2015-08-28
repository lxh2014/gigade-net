using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class TableChangeLogMgr
    {
        private Dao.TableChangeLogDao _tclDao;
        private string connStr;
        public TableChangeLogMgr(string connectionString)
        {
            _tclDao = new Dao.TableChangeLogDao(connectionString);
            this.connStr = connectionString;
        } 

        public DataTable GetVendorChangeLog(TableChangeLogQuery query, out int totalCount)
        {
            try
            {
                DataTable _dt = _tclDao.GetVendorChangeLog(query, out totalCount);
                _dt.Columns.Add("kuser_name");
                _dt.Columns.Add("muser_name");
                if (_dt.Rows.Count > 0)
                {
                    foreach (DataRow item in _dt.Rows)
                    {
                        Dao.ManageUserDao _muDao = new Dao.ManageUserDao(connStr);
                        if (!string.IsNullOrEmpty(item["kuser"].ToString()) && item["kuser"].ToString() != "0")
                        {
                            item["kuser_name"] = _muDao.GetManageUser(new Model.ManageUser { user_id = Convert.ToUInt32(item["kuser"].ToString()) }).FirstOrDefault().user_username;
                        }
                        if (!string.IsNullOrEmpty(item["muser"].ToString()) && item["muser"].ToString() != "0")
                        {
                            item["muser_name"] = _muDao.GetManageUser(new Model.ManageUser { user_id = Convert.ToUInt32(item["muser"].ToString()) }).FirstOrDefault().user_username;
                        }
                    }
                }
                return _dt;
            }
            catch (Exception ex)
            {
                throw new Exception("TableChangeLogMgr-->GetVendorChangeLog-->" + ex.Message, ex);
            }

        }

        public Model.Custom.TableChangeLogCustom GetVendorChangeDetail(TableChangeLogQuery query)
        {
            try
            {
                DataTable _dt = _tclDao.GetVendorChangeDetail(query);
                Model.Custom.TableChangeLogCustom _model = new Model.Custom.TableChangeLogCustom();
                if (_dt.Rows.Count > 0)
                {
                    Dao.ParametersrcDao _pDao = new Dao.ParametersrcDao(connStr);
                    List<Model.Parametersrc> _pModel = _pDao.Query(new Model.Parametersrc { ParameterType = "ColumnChange", ParameterCode = "vendor" });
                    List<Model.Parametersrc> _slist = _pDao.QueryParametersrcByTypes("vendor_type");
                    if (_pModel.Count > 0)
                    {
                        _model.vendor_id = Convert.ToInt32(_dt.Rows[0]["vendor_id"].ToString());
                        _model.vendor_name_full = _dt.Rows[0]["vendor_name_full"].ToString();
                        List<Model.TableChangeLog> _list = new List<Model.TableChangeLog>();
                        foreach (DataRow dr in _dt.Rows)
                        {
                            Model.TableChangeLog _log = new Model.TableChangeLog();
                            _log.change_field = dr["change_field"].ToString();
                            if (_log.change_field == "vendor_status")
                            {
                                string[] status_str = { "", "啟用", "停用", "失格" };

                                _log.old_value = status_str[Convert.ToInt32(dr["old_value"].ToString())].ToString();
                                _log.new_value = status_str[Convert.ToInt32(dr["new_value"].ToString())].ToString();


                            }
                            else if (_log.change_field == "product_manage")
                            {

                                ManageUserMgr mu = new ManageUserMgr(connStr);
                                List<ManageUser> o_mlist = mu.GetManageUser(new ManageUser { user_id = Convert.ToUInt32(dr["old_value"].ToString()) });
                                List<ManageUser> n_mlist = mu.GetManageUser(new ManageUser { user_id = Convert.ToUInt32(dr["new_value"].ToString()) });
                                if (o_mlist.Count > 0)
                                {
                                    _log.old_value = o_mlist[0].user_username;
                                }
                                if (n_mlist.Count > 0)
                                {
                                    _log.new_value = n_mlist[0].user_username;
                                }
                            }
                            else if (_log.change_field.IndexOf("contact_type") > -1)
                            {
                                string[] type_str = { "", "負責人", "業務窗口", "圖/文窗口", "出貨負責窗口", "帳務連絡窗口", "客服窗口", "刪除" };

                                _log.old_value = type_str[Convert.ToInt32(dr["old_value"].ToString())].ToString();
                                if (dr["new_value"].ToString() != "")
                                {
                                    _log.new_value = type_str[Convert.ToInt32(dr["new_value"].ToString())].ToString();
                                }
                                else
                                {
                                    _log.new_value = type_str[0].ToString();
                                }
                            }
                            else if (_log.change_field == "vendor_type")//供應商類型
                            {
                                if (_slist.Count > 0)
                                {
                                    var a_o = dr["old_value"].ToString().Split(',');
                                    var a_n = dr["new_value"].ToString().Split(',');
                                    string l_o = ""; string l_n = "";
                                    for (int i = 0; i < a_o.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(a_o[i].ToString()))
                                        {
                                            l_o = l_o + _slist.Find(m => m.ParameterCode == a_o[i].ToString()).parameterName + ",";
                                        }
                                    }
                                    for (int i = 0; i < a_n.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(a_n[i].ToString()))
                                        {
                                            l_n = l_n + _slist.Find(m => m.ParameterCode == a_n[i].ToString()).parameterName + ",";

                                        }
                                    }
                                    _log.old_value = l_o.TrimEnd(',');
                                    _log.new_value = l_n.TrimEnd(',');
                                }
                            }
                            else if (_log.change_field == "company_address" || _log.change_field == "invoice_address")//公司地址或發票地址
                            {
                                string o_zip = dr["old_value"].ToString().Split('&')[0];
                                string o_adress = dr["old_value"].ToString().Substring(dr["old_value"].ToString().IndexOf('&') + 1);
                                string n_zip = dr["new_value"].ToString().Split('&')[0];
                                string n_adress = dr["new_value"].ToString().Substring(dr["new_value"].ToString().IndexOf('&') + 1);
                                ZipMgr zip = new ZipMgr(connStr);
                                _log.old_value = zip.Getaddress(Convert.ToInt32(o_zip)) + o_adress;
                                _log.new_value = zip.Getaddress(Convert.ToInt32(n_zip)) + n_adress;
                            }
                            else
                            {
                                _log.old_value = dr["old_value"].ToString();
                                _log.new_value = dr["new_value"].ToString();
                            }
                            if (!string.IsNullOrEmpty(dr["field_ch_name"].ToString()))
                            {
                                _log.field_ch_name = dr["field_ch_name"].ToString();
                            }
                            else
                            {
                                _log.field_ch_name = _pModel.Find(m => m.parameterName == _log.change_field).remark;
                            }

                            _list.Add(_log);

                        }
                        _model.tclModel = _list;
                    }
                }
                return _model;
            }
            catch (Exception ex)
            {
                throw new Exception("TableChangeLogMgr-->GetVendorChangeDetail-->" + ex.Message, ex);
            }
        }

        public DataTable VendorLogExport(TableChangeLogQuery query)
        {
            DataTable result = new DataTable();

            result.Columns.Add("供應商編號", typeof(String));
            result.Columns.Add("供應商編碼", typeof(String));
            result.Columns.Add("供應商名稱", typeof(String));
            result.Columns.Add("建立人", typeof(String));
            result.Columns.Add("建立時間", typeof(String));
            result.Columns.Add("修改人", typeof(String));
            result.Columns.Add("修改時間", typeof(String));
            result.Columns.Add("變動欄位", typeof(String));
            result.Columns.Add("欄位中文名", typeof(String));
            result.Columns.Add("修改前值", typeof(String));
            result.Columns.Add("修改后值", typeof(String));


            int totalCount = 0;
            query.IsPage = false;
            DataTable _dt = GetVendorChangeLog(query, out totalCount);

            if (_dt.Rows.Count > 0)
            {
                foreach (DataRow item in _dt.Rows)
                {
                    DataRow dr = result.NewRow();
                    dr[0] = item["vendor_id"].ToString();
                    dr[1] = item["vendor_code"].ToString();
                    dr[2] = item["vendor_name_full"].ToString();
                    dr[3] = item["kuser_name"].ToString();
                    dr[4] = item["kdate"].ToString();
                    dr[5] = item["muser_name"].ToString();
                    dr[6] = item["mdate"].ToString();


                    TableChangeLogQuery log = new TableChangeLogQuery();
                    log.change_table = "vendor";
                    log.pk_id = Convert.ToInt32(item["vendor_id"].ToString());
                    log.create_user = Convert.ToInt32(item["muser"].ToString());
                    log.create_time = Convert.ToDateTime(item["mdate"].ToString());
                    Model.Custom.TableChangeLogCustom tclc = GetVendorChangeDetail(log);

                    dr[7] = tclc.tclModel[0].change_field.ToString();
                    dr[8] = tclc.tclModel[0].field_ch_name.ToString();
                    dr[9] = tclc.tclModel[0].old_value.ToString();
                    dr[10] = tclc.tclModel[0].new_value.ToString();
                    result.Rows.Add(dr);
                    for (int i = 1; i < tclc.tclModel.Count; i++)
                    {
                        DataRow dr2 = result.NewRow();
                        dr2[7] = tclc.tclModel[i].change_field.ToString();
                        dr2[8] = tclc.tclModel[i].field_ch_name.ToString();
                        dr2[9] = tclc.tclModel[i].old_value.ToString();
                        dr2[10] = tclc.tclModel[i].new_value.ToString();
                        result.Rows.Add(dr2);
                    }
                }
            }
            return result;
        }



    }
}
