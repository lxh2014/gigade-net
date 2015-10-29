using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
using DBAccess;
using System.Data;
using BLL.gigade.Common;

namespace BLL.gigade.Dao
{
    public class PromotionBannerDao
    {
        private IDBAccess _accessMySql;
        private string connStr;
        public PromotionBannerDao(string connectionstring)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
        }

        #region 列表頁store
        public List<PromotionBannerQuery> GetPromotionBannerList(PromotionBannerQuery query, string strSql, out int totalCount)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            totalCount = 0;
            DataTable dt = new DataTable();
            string now = CommonFunction.DateTimeToString(DateTime.Now);
            try
            {
                sql.AppendFormat(@"SELECT DISTINCT pb.pb_id,pb_image,pb_image_link,pb_startdate,pb_enddate,pb_status,pb_kdate,(SELECT user_username FROM manage_user WHERE manage_user.user_id=pb.pb_kuser ) as createusername,pb_mdate,(SELECT user_username FROM manage_user WHERE manage_user.user_id=pb.pb_muser ) as updateusername FROM promotion_banner pb LEFT JOIN promotion_banner_relation pbr ON pb.pb_id=pbr.pb_id WHERE 1=1  ");
                if (query.dateCon != 0)
                {
                    if (query.date_start != DateTime.MinValue)
                    {
                        if (query.date_end != DateTime.MinValue)
                        {
                            switch (query.dateCon)
                            {
                                case 1:
                                    sql.AppendFormat(" AND pb_startdate BETWEEN '{0}' AND '{1}'", CommonFunction.DateTimeToString(query.date_start), CommonFunction.DateTimeToString(query.date_end));
                                    break;
                                case 2:
                                    sql.AppendFormat(" AND pb_enddate BETWEEN '{0}' AND '{1}'", CommonFunction.DateTimeToString(query.date_start), CommonFunction.DateTimeToString(query.date_end));
                                    break;
                                case 3:
                                    sql.AppendFormat(" AND pb_kdate BETWEEN '{0}' AND '{1}'", CommonFunction.DateTimeToString(query.date_start), CommonFunction.DateTimeToString(query.date_end));
                                    break;
                                case 4:
                                    sql.AppendFormat(" AND pb_mdate BETWEEN '{0}' AND '{1}'", CommonFunction.DateTimeToString(query.date_start), CommonFunction.DateTimeToString(query.date_end));
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                switch (query.pb_status)
                {
                    case 0:
                        sql.AppendFormat(" AND pb_status={0}", query.pb_status);
                        break;
                    case 1:
                        sql.AppendFormat(" AND pb_status={0}", query.pb_status);
                        break;
                    default:
                        sql.AppendFormat(" ");
                        break;
                }
                if (query.singleBrand_id != 0)
                {
                    sql.AppendFormat(" AND pbr.brand_id = '{0}' ", query.singleBrand_id);
                }
                if (query.brandIDS != string.Empty)
                {
                    sql.AppendFormat(" AND pbr.brand_id in '{0}' ", query.brandIDS);
                }
                if (query.brand_name != string.Empty)
                {
                    sql.AppendFormat(strSql);
                }
                if (query.showStatus != 0)
                {
                    switch (query.showStatus)
                    {
                        case 1://1是未過期
                            sql.AppendFormat(" AND (pb_enddate>='{0}')", now);
                            break;
                        case 2://2是已過期
                            sql.AppendFormat(" AND (pb_enddate<'{0}')", now);
                            break;
                        default:
                            sql.AppendFormat(" ");
                            break;
                    }
                }
                sql.AppendFormat(" ORDER BY pb_id DESC");
                if (query.IsPage)
                {
                    dt = _accessMySql.getDataTable(sql.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = dt.Rows.Count;
                    }
                    sql.AppendFormat(" limit {0},{1}", query.Start, query.Limit);

                }
                return _accessMySql.getDataTableForObj<PromotionBannerQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerDao-->GetPromotionBannerList-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 更改促銷圖片狀態
        public int UpdateStatus(PromotionBannerQuery query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            string now = CommonFunction.DateTimeToString(DateTime.Now);
            try
            {
                sql.AppendFormat(@"UPDATE promotion_banner SET pb_status={0},pb_mdate='{1}',pb_muser={2} WHERE pb_id={3}", query.pb_status, now, query.pb_muser, query.pb_id);
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerDao-->UpdateStatus-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 更改促銷圖片信息
        public string UpdateImageInfo(PromotionBannerQuery query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            string now = CommonFunction.DateTimeToString(DateTime.Now);
            try
            {
                sql.AppendFormat(@"UPDATE promotion_banner SET pb_image='{0}',pb_image_link='{1}',pb_startdate='{2}',pb_enddate='{3}',pb_mdate='{4}',pb_muser={5} WHERE pb_id={6};", query.pb_image, query.pb_image_link, CommonFunction.DateTimeToString(query.pb_startdate), CommonFunction.DateTimeToString(query.pb_enddate), now, query.pb_muser, query.pb_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerDao-->UpdateImageInfo-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 新增促銷圖片
        public int AddImageInfo(PromotionBannerQuery query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            string now = CommonFunction.DateTimeToString(DateTime.Now);
            try
            {
                sql.AppendFormat(@"INSERT into promotion_banner(pb_image,pb_image_link,pb_startdate,pb_enddate,pb_status,pb_kdate,pb_kuser,pb_mdate,pb_muser) VALUES('{0}','{1}','{2}','{3}',1,'{4}',{5},'{4}',{6})", query.pb_image, query.pb_image_link, CommonFunction.DateTimeToString(query.pb_startdate), CommonFunction.DateTimeToString(query.pb_enddate), now, query.pb_kuser, query.pb_muser);
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerDao-->AddImageInfo-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 獲得新增的促銷圖片的ID
        public PromotionBannerQuery GetNewPb_id()
        {          
            StringBuilder sql = new StringBuilder();
            string now = CommonFunction.DateTimeToString(DateTime.Now);
            try
            {
                sql.AppendFormat(@"SELECT max( pb_id)as pb_id from promotion_banner ORDER BY pb_id DESC");
                return _accessMySql.getSinggleObj<PromotionBannerQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerDao-->GetNewPb_id-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 刪除促銷圖片
        public string DeleteImage(PromotionBannerQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"DELETE FROM promotion_banner WHERE pb_id={0};", query.pb_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerDao-->DeleteImage-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 通過pb_id獲取單個促銷圖片的信息
        public PromotionBannerQuery GetModelById(int id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT pb_id,pb_image,pb_image_link,pb_startdate,pb_enddate,pb_status,pb_kdate,(SELECT user_username FROM manage_user WHERE manage_user.user_id=pb.pb_kuser ) as createusername,pb_mdate,(SELECT user_username FROM manage_user WHERE manage_user.user_id=pb.pb_muser ) as updateusername FROM promotion_banner pb WHERE pb_id={0}", id);
                return _accessMySql.getSinggleObj<PromotionBannerQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerDao-->GetModelById-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 獲取促銷圖片顯示結束時間
        public DateTime GetEndTime(PromotionBannerQuery query)
        {
            StringBuilder sql = new StringBuilder();
            query.Replace4MySQL();
            try
            {
                sql.AppendFormat(@"SELECT pb_enddate from promotion_banner WHERE pb_id={0}", query.pb_id);
                PromotionBannerQuery model = _accessMySql.getSinggleObj<PromotionBannerQuery>(sql.ToString());
                if (model != null)
                {
                    return model.pb_enddate;
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerDao-->GetEndTime-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 查詢某個品牌在它的促銷圖片顯示時間段內是否有已啟用的促銷圖片
        public DataTable GetUsingImageInTimeArea(PromotionBannerQuery query)
        {
            StringBuilder sql = new StringBuilder();
            query.Replace4MySQL();
            try
            {
                sql.AppendFormat(@"SELECT DISTINCT pb.pb_id,pb_status,pb_startdate,pb_enddate from promotion_banner pb INNER JOIN promotion_banner_relation pbr on pb.pb_id=pbr.pb_id WHERE pb_status=1 ");
                if (query.pb_id != 0)
                {
                    sql.AppendFormat(" AND pbr.pb_id!={0}", query.pb_id);
                }
                if (query.singleBrand_id != 0)
                {
                    sql.AppendFormat(" AND pbr.brand_id={0}", query.singleBrand_id);
                }
                if (CommonFunction.GetPHPTime(CommonFunction.DateTimeToString(query.date_start)) < CommonFunction.GetPHPTime(CommonFunction.DateTimeToString(DateTime.Now)))
                {
                    sql.AppendFormat("  AND((pb_enddate>='{0}' AND pb_enddate <= '{1}')", CommonFunction.DateTimeToString(DateTime.Now), CommonFunction.DateTimeToString(query.date_end));
                    sql.AppendFormat("  OR( pb_startdate>='{0}' AND pb_startdate <= '{1}' AND pb_enddate>='{0}' AND pb_enddate <= '{1}')", CommonFunction.DateTimeToString(DateTime.Now), CommonFunction.DateTimeToString(query.date_end));
                    sql.AppendFormat("  OR( pb_startdate>='{0}' AND pb_startdate <= '{1}' AND pb_enddate >= '{1}')", CommonFunction.DateTimeToString(DateTime.Now), CommonFunction.DateTimeToString(query.date_end));
                    sql.AppendFormat("  OR( pb_startdate<='{0}' AND pb_enddate >= '{1}'))", CommonFunction.DateTimeToString(DateTime.Now), CommonFunction.DateTimeToString(query.date_end));
                }
                else
                {
                    sql.AppendFormat("  AND((pb_startdate <= '{0}' AND pb_enddate>='{0}' AND pb_enddate <= '{1}' )", CommonFunction.DateTimeToString(query.date_start), CommonFunction.DateTimeToString(query.date_end));
                    sql.AppendFormat("  OR( pb_startdate>='{0}' AND pb_startdate <= '{1}' AND pb_enddate>='{0}' AND pb_enddate <= '{1}')", CommonFunction.DateTimeToString(query.date_start), CommonFunction.DateTimeToString(query.date_end));
                    sql.AppendFormat("  OR( pb_startdate>='{0}' AND pb_startdate <= '{1}' AND pb_enddate >= '{1}')", CommonFunction.DateTimeToString(query.date_start), CommonFunction.DateTimeToString(query.date_end));
                    sql.AppendFormat("  OR( pb_startdate<='{0}' AND pb_enddate >= '{1}'))", CommonFunction.DateTimeToString(query.date_start), CommonFunction.DateTimeToString(query.date_end));

                }
                sql.AppendFormat(" ORDER BY pb.pb_id");
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerDao-->GetUsingImage-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 進入頁面前要判斷當前狀態是否允許多圖

        #region 顯示所有啟用中未過期且有品牌使用的圖片
        public DataTable ShowUsingImage()
        {
            StringBuilder sql = new StringBuilder();
            string now = CommonFunction.DateTimeToString(DateTime.Now);
            try
            {
                sql.AppendFormat(@"SELECT DISTINCT pbr.pb_id,pb_startdate,pb_enddate from promotion_banner_relation pbr LEFT  JOIN promotion_banner pb ON pbr.pb_id=pb.pb_id  WHERE 1=1  AND pb_status=1 AND pb_enddate>'{0}' ", now);
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerDao-->ShowUsingImage-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion
        #region 查詢某品牌編號是否有多張未過期的啟用狀態下的圖片
        public DataTable SearchImages(string brand_id)
        {
            StringBuilder sql = new StringBuilder();
            string now = CommonFunction.DateTimeToString(DateTime.Now);
            try
            {
                sql.AppendFormat(@"select pb.pb_id,pb_startdate,pb_enddate from promotion_banner pb  LEFT JOIN  promotion_banner_relation  pbr ON pbr.pb_id=pb.pb_id where 1=1");
                if (brand_id != string.Empty)
                {
                    sql.AppendFormat(" AND pbr.brand_id='{0}'", brand_id);
                }
                sql.AppendFormat(" AND pb_status=1 AND pb_enddate>'{0}'", now);
                sql.AppendFormat(" ORDER BY pb_id");
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerDao-->SearchImages-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #endregion
    }
}
