/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：VendorBrandDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/21 16:40:18 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;
using System.Data;
using BLL.gigade.Model.Query;
using System.Text.RegularExpressions;

namespace BLL.gigade.Dao
{
    public class VendorBrandDao : IVendorBrandImplDao
    {
        private IDBAccess _dbAccess;
        public VendorBrandDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        public VendorBrand GetProductBrand(VendorBrand query)
        {
            query.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("select brand_id,vendor_id,brand_name,brand_sort,brand_status,image_name,image_status,image_link_mode,image_link_url,media_report_link_url,");
            strSql.Append("brand_msg,brand_msg_start_time,brand_msg_end_time,brand_createdate,brand_updatedate,brand_ipfrom,cucumber_brand,event,promotion_banner_image,resume_image,");
            strSql.Append("promotion_banner_image_link,resume_image_link from vendor_brand where 1=1 ");
            if (query.Vendor_Id != 0)
            {
                strSql.AppendFormat(" and vendor_id={0}", query.Vendor_Id);
            }
            if (query.Brand_Id != 0)
            {
                strSql.AppendFormat(" and brand_id={0}", query.Brand_Id);
            }
            if (query.Brand_Name != "")
            {
                strSql.AppendFormat(" and brand_name='{0}'", query.Brand_Name);
            }
            return _dbAccess.getSinggleObj<VendorBrand>(strSql.ToString());
        }

        public List<VendorBrand> GetProductBrandList(VendorBrand brand, int hideOffGrade)
        {
            brand.Replace4MySQL();
            //StringBuilder strSql = new StringBuilder("select brand_id,brand_name,vendor_id from vendor_brand where 1=1");
            ///edit by wwei0216w 2015/6/30 要去掉失格供應商下的品牌  所以添加INNER JOIN vendor v ON v.vendor_id = vb.vendor_id的內聯
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@"SELECT vb.brand_id,vb.brand_name,vb.vendor_id 
                            FROM vendor_brand vb 
                                INNER JOIN vendor v ON v.vendor_id = vb.vendor_id");
                sb.Append(" WHERE 1=1 ");
                if (hideOffGrade == 1)
                {
                    sb.Append(" and v.vendor_status != 3 ");
                }

                if (brand.Brand_Status != 0)
                {
                    sb.AppendFormat(" AND vb.brand_status = {0}", brand.Brand_Status);
                }
                if (brand.Vendor_Id != 0)
                {
                    sb.AppendFormat(" AND vb.vendor_id={0}", brand.Vendor_Id);
                }
                return _dbAccess.getDataTableForObj<VendorBrand>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandDao-->GetProductBrandList" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 帶條件的品牌搜索
        /// </summary>
        /// <param name="brand">品牌搜索條件</param>
        /// <param name="cid">根據館別搜索品牌</param>
        /// <param name="hideOffGrade">失格供應商下的品牌是否顯示</param>
        /// <returns></returns>
        public List<VendorBrand> GetClassBrandList(VendorBrand brand, uint cid, int hideOffGrade = 0)
        {
            brand.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("select vb.brand_id,vb.brand_name from vendor_brand vb  ");
            if (cid != 0)
            {
                strSql.AppendFormat(" inner join vendor_brand_set vs on vb.brand_id=vs.brand_id  and vs.class_id='{0}'", cid);

            }
            if (hideOffGrade == 1)
            {
                strSql.AppendFormat(" inner join vendor v on v.vendor_id =vb.vendor_id and v.vendor_status <> 3");
            }
            strSql.Append(" where 1=1 ");
            if (brand.Brand_Status != 0)
            {
                strSql.AppendFormat(" and vb.brand_status = {0}", brand.Brand_Status);
            }

            return _dbAccess.getDataTableForObj<VendorBrand>(strSql.ToString());
        }
        #region 返回一个卖场列表+DataTable GetBandList(string sqlconcat)
        /// <summary>
        /// 返回一個賣場列表,
        /// </summary>chaojie_zz 添加于2014/10/17 04:54 pm
        /// <param name="sqlconcat">查询全部列表或者通过brand_id查询</param>
        /// <returns>DataTable表，用于筛选数据</returns>
        public DataTable GetBandList(string sqlconcat)
        {
            StringBuilder strSql = new StringBuilder(@"SELECT vb.brand_id,vb.brand_name,v.vendor_id,v.vendor_name_simple ");
            strSql.Append("  FROM vendor_brand vb,vendor v ");
            strSql.Append("  WHERE vb.vendor_id=v.vendor_id AND vb.brand_status=1 ");
            if (sqlconcat != null)
            {
                strSql.Append(sqlconcat);
            }
            strSql.Append("   ORDER BY vb.brand_id ASC");
            try
            {
                DataTable dt = _dbAccess.getDataTable(strSql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandDao-->GetBandList-->" + ex.Message + "sql:" + strSql.ToString(), ex);
            }

        }
        #endregion
        #region MyRegion品牌營業額統計，需要

        public List<VendorBrandQuery> GetBandList(VendorBrandQuery query)
        {
            StringBuilder strSql = new StringBuilder(@"SELECT vb.brand_id,vb.brand_name,v.vendor_id,v.vendor_name_simple ");
            try
            {
            strSql.Append("  FROM vendor_brand vb,vendor v ");
            strSql.Append("  WHERE vb.vendor_id=v.vendor_id AND vb.brand_status=1 ");
            
            strSql.Append("   ORDER BY vb.brand_id ASC");
            
                return _dbAccess.getDataTableForObj<VendorBrandQuery>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandDao-->GetBandList-->" + ex.Message + "sql:" + strSql.ToString(), ex);
            }

        }
        #endregion
        #region 返回品牌故事文字列表+ DataTable GetVendorBrandStory(VendorBrand query, out int totalCount)
        /// <summary>
        /// 獲取品牌故事文字列表
        /// </summary>
        /// <param name="query">VendorBrand表對象</param>
        /// <param name="totalCount">查詢記錄總條數</param>
        /// <returns></returns>
        public DataTable GetVendorBrandStory(VendorBrandQuery query, out int totalCount)
        {
            query.Replace4MySQL();
            StringBuilder conditionsql = new StringBuilder();
            StringBuilder strSql = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            StringBuilder count = new StringBuilder();
            VendorBrand list = new VendorBrand();
            DataTable store = new DataTable();
            totalCount = 0;
            try
            {
                if (query.isExport)
                {
                    conditionsql.AppendFormat(@"SELECT v.vendor_id,v.vendor_name_full, brand_id,brand_name,mu.user_username story_createname,story_createdate,mu1.user_username story_updatename,story_updatedate ");
                    conditionsql.AppendFormat(@"  FROM vendor_brand vb  ");
                }
                else
                {
                    conditionsql.AppendFormat(@"SELECT brand_id,brand_name,brand_status,brand_story_text,story_created,mu.user_username story_createname,story_createdate, ");
                    conditionsql.AppendFormat(@"  story_update,mu1.user_username story_updatename,story_updatedate FROM vendor_brand vb  ");
                }
                strSql.AppendFormat(@" LEFT JOIN manage_user mu on mu.user_id=vb.story_created ");
                strSql.AppendFormat(@" LEFT JOIN manage_user mu1 on mu1.user_id=vb.story_update ");
                strSql.AppendFormat(@" LEFT JOIN vendor v on vb.vendor_id=v.vendor_id  ");

                count.AppendFormat(@" SELECT count(brand_id) as totalCount from vendor_brand vb ");
                if (query.Vendor_Id != 0)
                {
                    sqlWhere.AppendFormat(" AND vb.vendor_id={0} ", query.Vendor_Id);
                }
                if (query.Brand_Id != 0)
                {
                    sqlWhere.AppendFormat(" AND vb.brand_id={0} ", query.Brand_Id);
                }
                if (query.date_start != DateTime.MinValue)
                {
                    sqlWhere.AppendFormat(" and vb.story_createdate >= '{0}'", Common.CommonFunction.DateTimeToString(query.date_start));

                }
                if (query.date_end != DateTime.MinValue)
                {
                    sqlWhere.AppendFormat(" and  vb.story_createdate <= '{0}'", Common.CommonFunction.DateTimeToString(query.date_end));
                }

                if (!string.IsNullOrEmpty(query.searchContent))
                {
                    //判斷“品牌編號/品牌名稱”輸入欄是否輸入的是數字
                    //如果是數字就按品牌編號和品牌名稱中帶輸入內容的條件進行搜索，不是數字就按品牌名稱帶搜索內容的條件來搜索
                    int result = 0;
                    //支持空格，中英文逗號隔開
                    string content = Regex.Replace(query.searchContent.Trim(), "(\\s+)|(，)|(\\,)", ",");
                    string[] contents = content.Split(',');
                    bool isSucess = int.TryParse(contents[0], out result);
                    if (isSucess)
                    {
                        sqlWhere.AppendFormat(" AND vb.brand_id IN ({0})  ", content);
                    }
                    else
                    {
                        sqlWhere.AppendFormat(@" AND ( ");
                        for (int i = 0; i < contents.Length; i++)
                        {
                            if (i == 0)
                            {
                                sqlWhere.AppendFormat(@"  vb.brand_name LIKE N'%{0}%' ", contents[i]);
                            }
                            else
                            {
                                sqlWhere.AppendFormat(@" OR vb.brand_name LIKE N'%{0}%'  ", contents[i]);
                            }

                        }
                        sqlWhere.AppendFormat(@" )");
                    }

                }
                if (!string.IsNullOrEmpty(query.story_createname))
                {
                    sqlWhere.AppendFormat(" AND mu.user_username like N'%{0}%'  ", query.story_createname);
                }
                //Brand_Story_Text是未編輯狀態
                if (query.Brand_Story_Text == "1")
                {
                    sqlWhere.AppendFormat(" AND vb.brand_story_text IS NULL");
                }
                //Brand_Story_Text是已編輯狀態
                else if (query.Brand_Story_Text == "2")
                {
                    sqlWhere.AppendFormat(" AND vb.brand_story_text IS NOT NULL");
                }
                if (query.vendorState != 0)
                {
                    sqlWhere.AppendFormat(" AND v.vendor_status={0}", query.vendorState);
                }
                if (query.Brand_Status != 0)
                {
                    sqlWhere.AppendFormat(" AND vb.brand_status={0}", query.Brand_Status);
                }
                if (sqlWhere.Length != 0)
                {
                    strSql.Append(" WHERE ");
                    strSql.Append(sqlWhere.ToString().TrimStart().Remove(0, 3));
                }
                if (query.IsPage)
                {
                    DataTable dt = _dbAccess.getDataTable(count.ToString() + strSql.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0]["totalCount"]);
                    }
                    strSql.AppendFormat(" order by brand_id limit {0},{1} ", query.Start, query.Limit);
                }
                else
                {
                    strSql.AppendFormat(" order by brand_id ");
                }

                store = _dbAccess.getDataTable(conditionsql.ToString() + strSql.ToString());
                return store;
            }
            catch (Exception ex)
            {

                throw new Exception("VendorBrandDao-->GetVendorBrandStory-->" + ex.Message + "sql:" + conditionsql.ToString() + strSql.ToString(), ex);
            }
        }
        #endregion

        #region 添加品牌故事文字+ int AddVendorBrandStory()

        public int AddVendorBrandStory(VendorBrandQuery query)
        {
            query.Replace4MySQL();
            StringBuilder strSql = new StringBuilder();
            DataTable store = new DataTable();

            try
            {
                query.Brand_Story_Text = query.Brand_Story_Text.Replace("\n", "<br/>");
                strSql.AppendFormat(@"UPDATE vendor_brand SET brand_story_text ='{0}',story_created='{1}',story_createdate='{2}',", query.Brand_Story_Text, query.Story_Created, Common.CommonFunction.DateTimeToString(query.Story_Createdate));
                strSql.AppendFormat(" story_update='{0}', story_updatedate='{1}'  WHERE brand_id={2} ", query.Story_Update, Common.CommonFunction.DateTimeToString(query.Story_Updatedate), query.Brand_Id);
                return _dbAccess.execCommand(strSql.ToString());

            }
            catch (Exception ex)
            {

                throw new Exception("VendorBrandDao-->AddVendorBrandStory-->" + ex.Message + "sql:" + strSql.ToString(), ex);
            }
        }
        #endregion

        public int GetClassify(VendorBrandQuery query)
        {
            query.Replace4MySQL();
            StringBuilder strSql = new StringBuilder();
            DataTable store = new DataTable();
            int classify = 0;

            try
            {
                strSql.AppendFormat(@"SELECT DISTINCT prod_classify from product WHERE brand_id={0}  ", query.Brand_Id);

                store = _dbAccess.getDataTable(strSql.ToString());
                foreach (DataRow row in store.Rows)
                {
                    if (Convert.ToInt32(row[0]) == 10 || Convert.ToInt32(row[0]) == 20)
                    {
                        classify = Convert.ToInt32(row[0]);
                        break;
                    }
                    else
                    {
                        classify = Convert.ToInt32(row[0]);
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception("VendorBrandDao-->GetClassify-->" + ex.Message + "sql:" + strSql.ToString(), ex);
            }
            return classify;
        }
        public List<VendorBrand> GetVendorBrand(VendorBrandQuery query)
        {
            query.Replace4MySQL();
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat(@"SELECT brand_id,brand_name,brand_status,brand_story_text,story_created,story_createdate,story_update,story_updatedate FROM vendor_brand ");
                if (query.Brand_Id != 0)
                {
                    strSql.AppendFormat(" where brand_id='{0}'", query.Brand_Id);
                }
                return _dbAccess.getDataTableForObj<VendorBrand>(strSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("VendorBrandDao-->GetVendorBrand-->" + ex.Message + "sql:" + strSql.ToString(), ex);
            }
        }
        /// <summary>
        /// 刪除供應商品牌的促銷圖片
        /// </summary>
        /// <param name="brand_id"></param>
        /// <returns></returns>
        public int DelPromoPic(int brand_id,string type)
        {

            StringBuilder strSql = new StringBuilder();
            try
            {

                strSql.AppendFormat("set sql_safe_updates = 0; update vendor_brand set {1}='' where brand_id='{0}';set sql_safe_updates = 1;", brand_id,type);

                return _dbAccess.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("VendorBrandDao-->DelPromoPic-->" + ex.Message + "sql:" + strSql.ToString(), ex);
            }
        }

        public string GetBrand_idByBrand_name(VendorBrandQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.AppendFormat(@"SELECT brand_id,brand_name from vendor_brand WHERE brand_name LIKE N'%{0}%'",query);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandDao-->GetBrand_idByBrand_name-->" + ex.Message + "sql:" + sql.ToString(), ex);
            }
        }

        public List<VendorBrand> GetBrandListByIds(string ids,uint id)
        {           
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat(@"SELECT brand_id,brand_name,brand_status,brand_story_text,story_created,story_createdate,story_update,story_updatedate FROM vendor_brand ");
                if (ids != string.Empty)
                {
                    strSql.AppendFormat(" where brand_id in ({0})", ids);
                }
                if (id != 0)
                {
                    strSql.AppendFormat(" where brand_id = {0}", id);
                }
                return _dbAccess.getDataTableForObj<VendorBrand>(strSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("VendorBrandDao-->GetBrandList-->" + ex.Message + "sql:" + strSql.ToString(), ex);
            }
        }
    }
}
