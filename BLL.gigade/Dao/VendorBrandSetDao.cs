using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;
using MySql.Data.MySqlClient;

namespace BLL.gigade.Dao
{
    public class VendorBrandSetDao : IVendorBrandSetImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;

        public VendorBrandSetDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            connStr = connectionStr;
        }
        public List<VendorBrandSet> Query(VendorBrandSet vbs)
        {
            StringBuilder sbSql = new StringBuilder();
            List<VendorBrandSet> list = new List<VendorBrandSet>();
            try
            {
                sbSql.Append("select id,brand_id,class_id from vendor_brand_set where 1=1 ");
                if (vbs.class_id != 0)
                {
                    sbSql.AppendFormat(" and class_id='{0}'", vbs.class_id);
                }
                list = _dbAccess.getDataTableForObj<VendorBrandSet>(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandSetDao-->Query-->" + ex.Message + "sql:" + sbSql.ToString(), ex);
            }
            return list;
        }
        public List<VendorBrandSet> GetClassId(VendorBrandSet vbs)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat("select id,brand_id,class_id from vendor_brand_set where brand_id=" + vbs.brand_id + " limit 1");
            return _dbAccess.getDataTableForObj<VendorBrandSet>(sbSql.ToString());
        }
        #region 品牌列表查詢
        public List<VendorBrandSetQuery> GetVendorBrandList(VendorBrandSetQuery store, out int totalCount)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder search = new StringBuilder();
            List<VendorBrandSetQuery> list = new List<VendorBrandSetQuery>();
            StringBuilder strCount = new StringBuilder();
            store.Replace4MySQL();
            try
            {
                strSql.AppendFormat(@"select vb.brand_id,vb.vendor_id,vb.brand_name,vb.brand_sort,vb.brand_status,vb.image_name, ");
                strSql.AppendFormat(" vb.image_status,vb.image_link_mode,vb.image_link_url,vb.media_report_link_url,vb.brand_msg, ");
                strSql.AppendFormat(" vb.brand_msg_start_time,brand_msg_end_time,vb.cucumber_brand,vb.promotion_banner_image,vb.resume_image, ");
                strSql.AppendFormat(" vb.promotion_banner_image_link,vb.resume_image_link, v.vendor_name_full, v.vendor_name_simple, v.vendor_invoice A  ,v.vendor_status as vendorstatus,vb.short_description");
                //strSql.AppendFormat(" ebh.event_money,ebh.event_statr_time,ebh.event_end_time,vb.`event`,ebh.createdate ");
                //strSql.AppendFormat("  from vendor_brand vb left join vendor v on vb.vendor_id=v.vendor_id ");
                //strSql.AppendFormat(" LEFT JOIN (SELECT b.* from event_brand_history b INNER JOIN (SELECT brand_id ,MAX(createdate) as 'time' FROM event_brand_history GROUP BY brand_id) a on b.brand_id=a.brand_id ");
                //strSql.AppendFormat(" AND createdate = a.time) ebh ON vb.brand_id = ebh.brand_id where 1=1 ");

                strSql.AppendFormat(" FROM vendor_brand vb, vendor v WHERE vb.vendor_id = v.vendor_id ");

                strCount = strCount.Append("select count(vb.brand_id) as search_total from vendor_brand vb left join vendor v on vb.vendor_id=v.vendor_id where 1=1");
                if (store.SearchType == 1)
                {
                    search.AppendFormat(" and vendor_name_simple like '%{0}%'", store.SearchCondition);
                    strSql.AppendFormat(search.ToString());
                    strCount.AppendFormat(search.ToString());
                }
                if (store.SearchType == 2)
                {

                    search.AppendFormat(" and v.vendor_invoice like '%{0}%'", store.SearchCondition);
                    strSql.AppendFormat(search.ToString());
                    strCount.AppendFormat(search.ToString());
                }
                if (store.SearchType == 3)
                {
                    search.AppendFormat(" and brand_name like '%{0}%'", store.SearchCondition);
                    strSql.AppendFormat(search.ToString());
                    strCount.AppendFormat(search.ToString());
                }
                if (store.Brand_Id != 0)
                {
                    strSql.AppendFormat(" and vb.brand_id='{0}'", store.Brand_Id);
                    strCount.AppendFormat(" and vb.brand_id='{0}'", store.Brand_Id);
                }
                strSql.AppendFormat(" order by vb.brand_sort desc,brand_id desc ");
                totalCount = 0;
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _dbAccess.getDataTable(strCount.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["search_total"]);
                    }
                    strSql.AppendFormat(" limit {0},{1}", store.Start, store.Limit);
                }
                list = _dbAccess.getDataTableForObj<VendorBrandSetQuery>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandSetDao-->GetVendorBrandList-->" + ex.Message + "sql:" + strSql.ToString(), ex);
            }
            return list;
        }
        #endregion
        public DataTable GetShop(string id)/*返回品牌*/
        {
            //GROUP_CONCAT()
            string strSql = string.Format("SELECT class_name,shop_class.class_id,brand_id from shop_class,vendor_brand_set where vendor_brand_set.class_id=shop_class.class_id and brand_id in ({0})", id);
            DataTable dt = _dbAccess.getDataTable(strSql);
            return dt;
        }
        public string GetShopName(uint id)
        {
            string str = string.Empty;
            StringBuilder strid = new StringBuilder();
            StringBuilder strb = new StringBuilder();
            VendorBrandSetQuery vbs = new VendorBrandSetQuery();
            try
            {
                string strSql = string.Format("SELECT class_name,class_id from shop_class where class_id IN (SELECT class_id FROM vendor_brand_set where brand_id ={0})", id);
                DataTable dt = _dbAccess.getDataTable(strSql);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strb.AppendFormat(dt.Rows[i][0].ToString());
                        strb.AppendFormat("|");
                        strid.AppendFormat(dt.Rows[i][1].ToString());
                        strid.AppendFormat(",");
                    }
                    str += strb.ToString().Remove(strb.ToString().LastIndexOf('|')) + "$";
                    str += strid.ToString().Remove(strid.ToString().LastIndexOf(','));
                }
                else
                {
                    str = "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandSetDao-->GetShopName-->" + ex.Message + "sql:" + str.ToString(), ex);
            }
            return str;
        }
        #region 品牌新增int Save(VendorBrandSetQuery model)
        /// <summary>
        /// 品牌新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Save(VendorBrandSetQuery model)
        {
            model.Replace4MySQL();
            int id = 0;
            StringBuilder sbSql = new StringBuilder();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                    mySqlConn.Open();
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                sbSql.Append(@"set sql_safe_updates = 0;update serial set serial_value= serial_value+1 where serial_id=14;set sql_safe_updates = 1;");
                sbSql.AppendFormat("INSERT INTO vendor_brand (brand_id,vendor_id,brand_name,brand_sort,brand_status,image_status,image_link_mode,image_link_url,promotion_banner_image_link,resume_image_link,media_report_link_url,brand_msg,brand_msg_start_time,brand_msg_end_time,brand_createdate,brand_updatedate,brand_ipfrom,cucumber_brand,`event`,image_name,resume_Image,promotion_banner_image,short_description) Values((select serial_value from serial where serial_id=14),'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}'); ", model.vendor_id, model.Brand_Name, model.Brand_Sort, model.Brand_Status, model.Image_Status, model.Image_Link_Mode, model.Image_Link_Url, model.Promotion_Banner_Image_Link, model.Resume_Image_Link, model.Media_Report_Link_Url, model.Brand_Msg, model.Brand_Msg_Start_Time, model.Brand_Msg_End_Time, model.Brand_Createdate, model.Brand_Createdate, model.Brand_Ipfrom, model.Cucumber_Brand, model.Event, model.Image_Name, model.Resume_Image, model.Promotion_Banner_Image, model.short_description);
                string str = model.classIds.ToString();
                string[] classid = str.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in classid)
                {
                    uint class_id = Convert.ToUInt32(item);
                    sbSql.AppendFormat(@"Insert into vendor_brand_set (brand_id,class_id) Values ((select serial_value from serial where serial_id=14),'{0}');", class_id);
                }
                mySqlCmd.CommandText = sbSql.ToString();
                mySqlCmd.ExecuteScalar();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("VendorBrandSetDao-->Save-->" + ex.Message + " sql:" + sbSql.ToString(), ex);
            }
            finally
            {
                mySqlConn.Close();
            }
            return id;
        }
        #endregion

        #region 品牌修改
        public int Update(VendorBrandSetQuery model)
        {
            model.Replace4MySQL();
            int id = 0;
            StringBuilder sb = new StringBuilder();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                    mySqlConn.Open();
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                sb.AppendFormat(@"UPDATE vendor_brand SET vendor_id='{0}',brand_name='{1}',brand_sort='{2}',brand_status='{3}',image_status='{4}',image_link_mode='{5}',image_link_url='{6}',promotion_banner_image_link='{7}',resume_image_link='{8}',media_report_link_url='{9}',brand_msg='{10}',brand_msg_start_time='{11}',brand_msg_end_time='{12}',brand_createdate='{13}',brand_updatedate='{14}',brand_ipfrom='{15}',cucumber_brand='{16}',image_name='{17}',Resume_Image='{18}',Promotion_Banner_Image='{19}',short_description='{20}' where brand_id='{21}' ;", model.vendor_id, model.Brand_Name, model.Brand_Sort, model.Brand_Status, model.Image_Status, model.Image_Link_Mode, model.Image_Link_Url, model.Promotion_Banner_Image_Link, model.Resume_Image_Link, model.Media_Report_Link_Url, model.Brand_Msg, model.Brand_Msg_Start_Time, model.Brand_Msg_End_Time, model.Brand_Createdate, model.Brand_Createdate, model.Brand_Ipfrom, model.Cucumber_Brand, model.Image_Name, model.Resume_Image, model.Promotion_Banner_Image, model.short_description, model.Brand_Id);
                sb.AppendFormat("delete from vendor_brand_set where brand_id='{0}';", model.Brand_Id);
                string str = model.classIds.ToString();
                string[] classid = str.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in classid)
                {
                    id = Convert.ToInt32(item);
                    sb.AppendFormat(@"insert into vendor_brand_set(class_id,brand_id) values('{0}','{1}'); ", id, model.Brand_Id);
                }
                mySqlCmd.CommandText = sb.ToString();
                mySqlCmd.ExecuteScalar();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("VendorBrandSetDao-->Update-->" + ex.Message + " sql:" + sb.ToString(), ex);
            }
            finally
            {
                mySqlConn.Close();
            }
            return id;
        }
        #endregion

        #region 從serial表查詢venddor_brand 表的brand_id+int GetBrandId()
        /// <summary>
        /// 從serial表查詢venddor_brand 表的brand_id
        /// </summary>
        /// <returns></returns>
        public int GetBrandId()
        {
            int id = 0;
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat("SELECT serial_value from serial where serial_id='14'; ");
                DataTable dt = _dbAccess.getDataTable(strSql.ToString());
                id = Int32.Parse(dt.Rows[0][0].ToString()) + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandSetDao-->GetBrandId()-->" + ex.Message + " sql:" + strSql.ToString(), ex);
            }
            return id;
        }
        #endregion
        public VendorBrandSetQuery GetModelById(int id)
        {
            StringBuilder sql = new StringBuilder();
            VendorBrandSetQuery m = new VendorBrandSetQuery();
            try
            {
                sql.Append("SELECT brand_id,vendor_id,brand_name,brand_sort,brand_status,image_name,image_status,image_link_mode,image_link_url,media_report_link_url,brand_msg,brand_msg_start_time,brand_msg_end_time,brand_createdate,brand_updatedate,brand_ipfrom,cucumber_brand,`event`,promotion_banner_image,resume_image,promotion_banner_image_link,resume_image_link,short_description ");
                sql.Append(" FROM vendor_brand ");
                sql.AppendFormat("  where 1=1 and brand_id={0}", id);
                m = _dbAccess.getSinggleObj<VendorBrandSetQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandSetDao-->GetModelById-->" + ex.Message + " sql:" + sql.ToString(), ex);
            }
            return m;
        }




        #region 查找出圖片 +VendorBrandSetQuery GetSingleImage(string name)
        /// <summary>
        /// 得到圖片
        /// </summary>
        /// <param name="name">圖片名稱</param>
        /// <returns></returns>
        public VendorBrandSetQuery GetSingleImage(string name)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("SELECT vendor_brand_story.image_sort,vendor_brand_story.image_state,vendor_brand_story.image_createdate FROM vendor_brand_story WHERE image_filename='{0}'; ", name);

            try
            {
                return _dbAccess.getSinggleObj<VendorBrandSetQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandSetDao-->GetSingleImage-->" + ex.Message + sql.ToString());
            }

        }
        #endregion


        #region 刪除圖片 + int DeleteImage(string imageName)
        /// <summary>
        /// 刪除圖片
        /// </summary>
        /// <param name="imageName">圖片名稱</param>
        /// <returns></returns>
        public int DeleteImage(string imageName)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("set sql_safe_updates=0;DELETE from vendor_brand_story WHERE image_filename='{0}';set sql_safe_updates=1;", imageName);
            try
            {
                return _dbAccess.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandSetDao-->DeleteImage-->sql:" + ex.Message + sql.ToString());
            }

        }

        #endregion

        #region 修改圖片狀態和排序 +int UpdateImage(VendorBrandSetQuery query)
        /// <summary>
        ///更新圖片
        /// </summary>
        /// <param name="query">傳值</param>
        /// <returns></returns>
        public int UpdateImage(VendorBrandSetQuery query)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("set sql_safe_updates=0;UPDATE vendor_brand_story SET image_sort='{0}' ,image_state='{1}' WHERE image_filename='{2}';set sql_safe_updates=1; ", query.image_sort, query.image_state, query.image_filename);
            try
            {
                return _dbAccess.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandSetDao-->GetSingleImage-->" + ex.Message + sql.ToString());
            }
        }
        #endregion

        #region 新增圖片 + SaveBrandStory(VendorBrandSetQuery query)
        /// <summary>
        /// 新增圖片
        /// </summary>
        /// <param name="query"></param>
        public void SaveBrandStory(VendorBrandSetQuery query)
        {
            StringBuilder strSql = new StringBuilder();
            //   string[] filenames = query.image_filename.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
            //   foreach (string item in filenames)
            //   {
            strSql.AppendFormat("INSERT INTO vendor_brand_story(brand_id,image_filename,image_sort,image_state,image_createdate) VALUES('{0}','{1}','{2}','{3}','{4}');", query.Brand_Id, query.image_filename, query.image_sort, query.image_state, query.image_createdate);
            //   }
            try
            {
                _dbAccess.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandSetDao-->SaveBrandStory-->" + ex.Message + strSql.ToString());
            }
        }
        #endregion

        #region 顯示圖片信息 +GetImageInfo(VendorBrandSetQuery store)
        /// <summary>
        /// 顯示圖片信息
        /// </summary>
        /// <param name="store">傳值</param>
        /// <returns></returns>
        public List<VendorBrandSetQuery> GetImageInfo(VendorBrandSetQuery store)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("SELECT vendor_brand_story.brand_id, vendor_brand_story.image_filename,vendor_brand_story.image_sort,vendor_brand_story.image_state,vendor_brand_story.image_createdate FROM vendor_brand_story where  brand_id='{0}' ORDER BY image_sort DESC,image_createdate DESC;", store.Brand_Id);
            try
            {
                return _dbAccess.getDataTableForObj<VendorBrandSetQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandSetDao-->GetImageInfo-->sql:" + ex.Message + sql.ToString());
            }

        }
        #endregion

        public bool GetSortIsRepeat(VendorBrandSetQuery query)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("SELECT count(image_sort) as count FROM vendor_brand_story where image_sort='{0}' and brand_id='{1}' ", query.image_sort, query.Brand_Id);
            try
            {
                DataTable _dt = _dbAccess.getDataTable(sql.ToString());
                int result = Convert.ToInt32(_dt.Rows[0]["count"]);
                if (result > 0)
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
                throw new Exception("VendorBrandSetDao-->GetSortIsRepeat-->sql:" + ex.Message + sql.ToString());
            }
        }
    }
}
