using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using System.Collections;
using BLL.gigade.Common;

namespace BLL.gigade.Mgr
{
    public class TicketMasterMgr : ITicketMasterImplMgr
    {
        private ITicketMasterImplDao _tmDao;
        private string connStr;
        public TicketMasterMgr(string connectionString)
        {
            _tmDao = new TicketMasterDao(connectionString);
            connStr = connectionString;
        }
        public DataTable GetTicketMasterList(TicketMasterQuery tm, out int totalCount)
        {
            try
            {
                DataTable _dt = _tmDao.GetTicketMasterList(tm, out totalCount);
                _dt.Columns.Add("s_order_createdate", typeof(string));
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    if (tm.isSecret == "true")
                    {
                        _dt.Rows[i]["order_name"] = _dt.Rows[i]["order_name"].ToString().Substring(0, 1) + "**";
                    }
                    string time = _dt.Rows[i]["order_createdate"].ToString();
                    DateTime longTime = CommonFunction.GetNetTime(long.Parse(time));
                    _dt.Rows[i]["s_order_createdate"] = CommonFunction.DateTimeToString(longTime);
                }
              //  return _tmDao.GetTicketMasterList(tm, out totalCount);
              return _dt;
            }
            catch (Exception ex)
            {
                throw new Exception("TicketMasterMgr-->GetTicketMasterList" + ex.Message, ex);
            }
        }
        public string Update(TicketMaster tm)
        {
            string json = string.Empty;
            try
            {
                if (_tmDao.Update(tm) > 0)
                {
                    json = "{success:true}";//返回json數據
                }
                else
                {
                    json = "{success:false}";//返回json數據
                }
                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("TicketMasterMgr-->Update" + ex.Message, ex);
            }
        }
          
        public DataTable GetCourseCountList(CourseQuery query, out int totalCount)
        {
            try
            {

                DataTable course_dt = _tmDao.GetCourseCountList(query, out totalCount);
                course_dt.Columns.Add("ticket_detail_id");
                course_dt.Columns.Add("spec_name_1");
                course_dt.Columns.Add("spec_name_2");
                course_dt.Columns.Add("vendor_name_simple");
                course_dt.Columns.Add("sales_number");
                course_dt.Columns.Add("used_number");
                if (course_dt != null)
                {
                    VendorBrandDao _vbDao = new VendorBrandDao(connStr);
                    ProductSpecDao _psDao = new ProductSpecDao(connStr);
                    CourseTicketDao _ctDao = new CourseTicketDao(connStr);
                    foreach (DataRow item in course_dt.Rows)
                    {
                        //獲取供應商名稱
                        item["vendor_name_simple"] = _vbDao.GetBandList(string.Format(" and vb.brand_id='{0}'", item["brand_id"])).Rows[0]["vendor_name_simple"];
                        item["spec_name_1"] = _psDao.query(Convert.ToInt32(item["spec_id_1"])).spec_name;
                        item["spec_name_2"] = _psDao.query(Convert.ToInt32(item["spec_id_2"])).spec_name;
                        DataTable s_dt = _ctDao.GetCount(Convert.ToInt32(item["item_id"]));
                        if (s_dt.Rows.Count > 0)
                        {
                            item["sales_number"] = s_dt.Rows[0]["number"];
                            item["ticket_detail_id"] = s_dt.Rows[0]["ticket_detail_id"];
                        }
                        DataTable u_dt = _ctDao.GetCount(Convert.ToInt32(item["item_id"]), 1);
                        if (u_dt.Rows.Count > 0)
                        {
                            item["used_number"] = u_dt.Rows[0]["number"];
                        }

                    }
                }

                return course_dt;
            }
            catch (Exception ex)
            {
                throw new Exception("TicketMasterMgr-->GetCourseCountList-->" + ex.Message, ex);
            }

        }


        public bool CancelOrder(List<TicketMasterQuery> list)
        {
            ArrayList arrList = new ArrayList();
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    arrList.Add(_tmDao.CancelOrderTM(list[i]));
                    arrList.Add(_tmDao.CancelOrderTD(list[i]));
                }
              return     _tmDao.ExecSql(arrList);
            }
            catch(Exception ex)
            {
                throw new Exception("MailGroupMgr-->CancelOrder-->" + arrList.ToString() + ex.Message, ex);
            }
        }

        public string ExecCancelOrder(List<TicketMasterQuery> list)
        {
            string json = string.Empty;
            try
            {
                if (CancelOrder(list))
                {
                    json = "{success:true}";
                }
                else
                {
                    json = "{failure:true}";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("MailGroupMgr-->ExecCancelOrder-->" + list.ToString() + ex.Message, ex);
            }
            return json;
        }
    }
}
