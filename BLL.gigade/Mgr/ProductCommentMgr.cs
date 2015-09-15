using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;
namespace BLL.gigade.Mgr
{
    public class ProductCommentMgr : IProductCommentImplMgr
    {
        private IProductCommentImplDao _proCommentDao;
        private string connStr;
        public ProductCommentMgr(string connectionString)
        {
            _proCommentDao = new ProductCommentDao(connectionString);
            this.connStr = connectionString;
        }
        public DataTable Query(Model.Query.ProductCommentQuery store, out int totalCount)
        {
            try
            {
                return _proCommentDao.Query(store, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCommentMgr-->Query-->" + ex.Message, ex);
            }
        }
        public int UpdateActive(ProductCommentQuery model)
        {
            try
            {
                return _proCommentDao.UpdateActive(model);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductCommentMgr-->UpdateActive-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 評價回覆
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int ProductCommentSave(ProductCommentQuery query)
        {
            try
            {
                return _proCommentDao.ProductCommentSave(query);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductCommentMgr-->ProductCommentSave-->" + ex.Message, ex);
            }
        }


        public ProductCommentQuery GetUsetInfo(ProductCommentQuery store)
        {
            try
            {
                return _proCommentDao.GetUsetInfo(store);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCommentMgr-->GetUsetInfo-->" + ex.Message, ex);
            }
        }

        //add by xiaohui1027j 2015/7/24
        public DataTable QueryTableName()
        {
            try
            {
                return _proCommentDao.QueryTableName();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCommentMgr.GetChangeLogList-->" + ex.Message, ex);
            }
        }
        public DataTable GetChangeLogList(ProductCommentQuery query, out int totalCount)
        {
            try
            {
                DataTable _dt = _proCommentDao.GetChangeLogList(query, out totalCount);
                _dt.Columns.Add("user_name");
                if (_dt.Rows.Count > 0)
                {
                    foreach (DataRow item in _dt.Rows)
                    {
                        Dao.ManageUserDao _muDao = new Dao.ManageUserDao(connStr);
                        if (!string.IsNullOrEmpty(item["create_user"].ToString()) && item["create_user"].ToString() != "0")
                        {
                            item["user_name"] = _muDao.GetManageUser(new Model.ManageUser { user_id = Convert.ToUInt32(item["create_user"].ToString()) }).FirstOrDefault().user_username;
                        }
                    }
                }
                return _dt;
                //return _proCommentDao.GetChangeLogList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCommentMgr.GetChangeLogList-->" + ex.Message, ex);
            }
        }
        public Model.Custom.TableChangeLogCustom GetChangeLogDetailList(int pk_id, string create_time)
        {
            try
            {
                DataTable _dt = _proCommentDao.GetChangeLogDetailList( pk_id, create_time);
                Model.Custom.TableChangeLogCustom _model = new Model.Custom.TableChangeLogCustom();
                if (_dt.Rows.Count > 0)
                {
                    _model.pk_id = Convert.ToInt32(_dt.Rows[0]["pk_id"].ToString());
                    _model.change_table = _dt.Rows[0]["change_table"].ToString();
                    List<Model.TableChangeLog> _list = new List<Model.TableChangeLog>();
                    foreach (DataRow item in _dt.Rows)
                    {
                        Model.TableChangeLog _log = new Model.TableChangeLog();
                        if (item["change_table"].ToString() == "comment_detail")
                        {
                            if (item["change_field"].ToString() == "answer_is_show")
                            {
                                //item["old_value"]=item["old_value"].ToString() == "0" ? "否" : "是";                      
                                if (item["old_value"].ToString() == "0")
                                {
                                    item["old_value"] = "否";
                                }
                                if (item["old_value"].ToString() == "1")
                                {
                                    item["old_value"] = "是";
                                }
                                if (item["new_value"].ToString() == "0")
                                {
                                    item["new_value"] = "否";
                                }
                                if (item["new_value"].ToString() == "1")
                                {
                                    item["new_value"] = "是";
                                }
                            }
                            if (item["change_field"].ToString() == "send_mail")
                            {
                                //item["old_value"]=item["old_value"].ToString() == "0" ? "否" : "是";                      
                                if (item["old_value"].ToString() == "0")
                                {
                                    item["old_value"] = "發送成功";
                                }
                                if (item["old_value"].ToString() == "1")
                                {
                                    item["old_value"] = "否";
                                }
                                if (item["old_value"].ToString() == "2")
                                {
                                    item["old_value"] = "發送失敗";
                                }
                                if (item["new_value"].ToString() == "0")
                                {
                                    item["new_value"] = "發送成功";
                                }
                                if (item["new_value"].ToString() == "1")
                                {
                                    item["new_value"] = "否";
                                }
                                if (item["new_value"].ToString() == "2")
                                {
                                    item["new_value"] = "發送失敗";
                                }
                            }
                        }
                        _log.change_field = item["change_field"].ToString();
                        _log.old_value = item["old_value"].ToString();
                        _log.new_value = item["new_value"].ToString();
                        _log.field_ch_name = item["field_ch_name"].ToString(); 
                        _list.Add(_log);
                    }
                   _model.tclModel = _list;
                }
                
                return _model;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCommentMgr.GetChangeLogDetailList-->" + ex.Message, ex);
            }
        }
        public DataTable ProductCommentLogExport(ProductCommentQuery query)
        {
            DataTable result = new DataTable();

            result.Columns.Add("評價編號", typeof(String));
            result.Columns.Add("評價內容", typeof(String));
            result.Columns.Add("修改人", typeof(String));
            result.Columns.Add("修改時間", typeof(String));
            result.Columns.Add("變動欄位", typeof(String));
            result.Columns.Add("欄位中文名", typeof(String));
            result.Columns.Add("修改前值", typeof(String));
            result.Columns.Add("修改后值", typeof(String));
           // string[] colname = { "", "建立人", "建立時間", "變動欄位", "欄位中文名", "修改前值", "修改后值" };

            int totalCount = 0;
            query.IsPage = false;
            DataTable _dt = GetChangeLogList(query, out totalCount);

            if (_dt.Rows.Count > 0)
            {
                foreach (DataRow item in _dt.Rows)
                {
                    DataRow dr = result.NewRow();
                    dr[0] = item["comment_id"].ToString();
                    dr[1] = item["comment_info"].ToString();
                    dr[1] = dr[1].ToString().Replace(',', '，').Replace("\r", "").Replace("\n", "");
                    dr[2] = item["user_name"].ToString();
                    dr[3] = item["create_time"].ToString();
                    
                    int pk_id = Convert.ToInt32(item["pk_id"]);
                    string create_time = Common.CommonFunction.DateTimeToString(Convert.ToDateTime(item["create_time"]));

                    Model.Custom.TableChangeLogCustom tclc = GetChangeLogDetailList(pk_id, create_time);
                    dr[4] = tclc.tclModel[0].change_field.ToString();
                    dr[5] = tclc.tclModel[0].field_ch_name.ToString();
                    dr[6] = tclc.tclModel[0].old_value.ToString();
                    dr[7] = tclc.tclModel[0].new_value.ToString();
                    dr[6] = dr[6].ToString().Replace(',', '，').Replace("\r", "").Replace("\n", "");
                    dr[7] = dr[7].ToString().Replace(',', '，').Replace("\r", "").Replace("\n", "");
                    result.Rows.Add(dr);
                    for (int i = 1; i < tclc.tclModel.Count; i++)
                    {
                        DataRow dr2 = result.NewRow();
                        dr2[4] = tclc.tclModel[i].change_field.ToString();
                        dr2[5] = tclc.tclModel[i].field_ch_name.ToString();
                        dr2[6] = tclc.tclModel[i].old_value.ToString();
                        dr2[7] = tclc.tclModel[i].new_value.ToString();
                        dr2[6] = dr2[6].ToString().Replace(',', '，').Replace("\r", "").Replace("\n", "");
                        dr2[7] = dr2[7].ToString().Replace(',', '，').Replace("\r", "").Replace("\n", "");
                        result.Rows.Add(dr2);
                    }
                }
            }
            return result;
        }

    }
}
