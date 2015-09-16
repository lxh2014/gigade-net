using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Mgr;
using BLL.gigade.Dao;
using System.Collections;

namespace BLL.gigade.Mgr
{
    public class SiteStatisticsMgr
    {
        private SiteStatisticsDao ssDao;
        public SiteStatisticsMgr(string connectionString)
        {
            ssDao = new SiteStatisticsDao(connectionString);
        }
        public DataTable GetSiteStatisticsList(SiteStatistics query, out int totalCount)
        {
            try
            {
                return ssDao.GetSiteStatisticsList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("SiteStatisticsDao-->GetSiteStatisticsList" + ex.Message, ex);
            }
        }
        public int Insert(SiteStatistics ss)
        {
            try
            {
                return ssDao.Insert(ss);
            }
            catch (Exception ex)
            {
                throw new Exception("SiteStatisticsDao-->Insert" + ex.Message, ex);
            }
        }
        public int Update(SiteStatistics ss)
        {
            try
            {
                return ssDao.Update(ss);
            }
            catch (Exception ex)
            {
                throw new Exception("SiteStatisticsDao-->Update" + ex.Message, ex);
            }
        }
        public int Delete(SiteStatistics ss)
        {
            try
            {
                return ssDao.Delete(ss);
            }
            catch (Exception ex)
            {
                throw new Exception("SiteStatisticsDao-->Delete" + ex.Message, ex);
            }
        }
        public string ImportExcelToDt(DataTable _dt)
        {
            string json = "{success:'true'}";
            SiteStatistics query = new SiteStatistics();
            query.ss_create_time = DateTime.Now;
            query.ss_create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
            try
            {
                int total = 0;
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    if (_dt.Rows[i][0].ToString() != "" && _dt.Rows[i][1].ToString() != "" && _dt.Rows[i][2].ToString() != "")
                    {
                        query.ss_date = Convert.ToDateTime(_dt.Rows[i][0]);
                        query.ss_show_num = Convert.ToInt32(_dt.Rows[i][1]);
                        query.ss_click_num = Convert.ToInt32(_dt.Rows[i][2]);
                        query.ss_click_through = Convert.ToSingle(_dt.Rows[i][3]);
                        query.ss_cost = Convert.ToSingle(_dt.Rows[i][4]);
                        query.ss_newuser_number = Convert.ToInt32(_dt.Rows[i][5]);
                        query.ss_converted_newuser = Convert.ToInt32(_dt.Rows[i][6]);
                        query.ss_sum_order_amount = Convert.ToInt32(_dt.Rows[i][7]);
                        query.ss_code = _dt.Rows[i][8].ToString().ToUpper();
                        DataTable dt=GetSiteStatisticsList(query, out total);
                        if (dt.Rows.Count == 0)
                        {
                            ssDao.Insert(query);
                        }
                        else
                        {
                            query.ss_id = Convert.ToInt32(dt.Rows[0][0]);
                            ssDao.Update(query);
                            query.ss_id = 0;
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception("SiteStatisticsDao-->ImportExcelToDt" + ex.Message, ex);
            }
            return json;
        }
    }
}
