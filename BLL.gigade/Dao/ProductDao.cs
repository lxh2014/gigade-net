/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司
* All rights reserved. 
*  
* 文件名称：ProductDao 
* 摘   要： 
*  
* 当前版本：1.0 
* 作   者：lhInc 
* 完成日期：2013/8/21 13:24:47 
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Data;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;
using System.Text.RegularExpressions;

namespace BLL.gigade.Dao
{
    public class ProductDao : IProductImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public ProductDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }
        public List<Product> Query(Product query)
        {
            try
            {
                StringBuilder strSql = new StringBuilder("select p.product_id,p.brand_id,product_vendor_code,product_name,prod_sz,product_price_list,product_spec,spec_title_1,spec_title_2,product_image,");
                strSql.Append("product_freight_set,product_buy_limit,product_status,product_hide,product_mode,product_sort,product_start,product_end,page_content_1,page_content_2,");
                strSql.Append("page_content_3,product_keywords,product_recommend,product_password,product_total_click,expect_time,product_image,product_createdate,product_updatedate,");
                strSql.Append("product_ipfrom,goods_area,goods_image1,goods_image2,city,bag_check_money,combination,bonus_percent,default_bonus_percent,bonus_percent_start,");
                strSql.Append("bonus_percent_end,tax_type,cate_id,fortune_quota,fortune_freight,product_media,ignore_stock,shortage,stock_alarm,price_type,user_id,show_listprice,expect_msg,");
                strSql.Append("detail_created,detail_createdate,detail_update,detail_updatedate,");//add by shuangshuang0420j 20150513 16:26 商品詳情文字更動時間
                strSql.Append("process_type,show_in_deliver,prepaid,product_type,prod_name,prod_sz,prod_classify,vb.vendor_id,cp.course_id,p.safe_stock_amount,p.deliver_days,p.min_purchase_amount,p.extra_days, mobile_image,sr.schedule_id,product_alt,s.`desc`,p.off_grade, purchase_in_advance,purchase_in_advance_start,purchase_in_advance_end,rpa.months,rpa.expend_day  from product p ");//edit by wwei0216 查詢的屬性多加一個mobile_image 2015/3/18
                strSql.Append(" left join vendor_brand vb ON p.brand_id = vb.brand_id ");
                strSql.Append(" left join course_product cp on p.product_id=cp.product_id");
                strSql.Append(" left join schedule_relation sr ON sr.relation_id = p.product_id  AND sr.relation_table = 'product'");
                strSql.Append(" LEFT JOIN schedule s ON s.schedule_id = sr.schedule_id");
                strSql.Append(" LEFT JOIN recommended_product_attribute rpa on rpa.product_id=p.product_id ");
                strSql.Append(" where 1=1 ");
                if (query.Product_Id != 0)
                {
                    strSql.AppendFormat(" and p.product_id={0}", query.Product_Id);
                }
                return _dbAccess.getDataTableForObj<Product>(strSql.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.Query-->" + ex.Message, ex);
            }
        }

        public List<Product> Query(uint[] productIds)
        {
            try
            {
                if (productIds.Length == 0)
                {
                    return new List<Product>();
                }
                string prodIds = "";
                foreach (uint prodId in productIds)
                {
                    prodIds += prodId + ",";
                }
                string strSql = string.Format("select * from product where product_id in({0});", prodIds.Remove(prodIds.Length - 1));
                return _dbAccess.getDataTableForObj<Product>(strSql);

            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.Query-->" + ex.Message, ex);
            }
        }

        public string Save(Product product)
        {
            product.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("insert into product( ");
            strSql.Append(" `product_id`,`brand_id`,`product_vendor_code`,");
            strSql.Append(" `product_name`,`product_price_list`,`product_spec`,`spec_title_1`,`spec_title_2`,");
            strSql.Append(" `product_freight_set`,`product_buy_limit`,`product_status`,`product_hide`,");
            strSql.Append(" `product_mode`,`product_sort`,`product_start`,`product_end`,`page_content_1`,`page_content_2`,");
            strSql.Append(" `page_content_3`,`product_keywords`,`product_recommend`,`product_password`,");
            strSql.Append(" `product_total_click`,`expect_time`,`product_image`,`product_createdate`,`product_updatedate`,");
            strSql.Append(" `product_ipfrom`,`goods_area`,`goods_image1`,`goods_image2`,`city`,");
            strSql.Append(" `bag_check_money`,`combination`,`bonus_percent`,`default_bonus_percent`,`bonus_percent_start`,`bonus_percent_end`,`tax_type`,`cate_id`,");
            strSql.Append(" `fortune_quota`,`fortune_freight`,`ignore_stock`,`shortage`,`stock_alarm`,`price_type`,`user_id`,`show_listprice`,`expect_msg`) values ({0},");
            strSql.AppendFormat(" '{0}','{1}',", product.Brand_Id, product.Product_Vendor_Code);
            strSql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}',", product.Product_Name, product.Product_Price_List, product.Product_Spec, product.Spec_Title_1, product.Spec_Title_2);
            strSql.AppendFormat(" '{0}','{1}','{2}','{3}',", product.Product_Freight_Set, product.Product_Buy_Limit, product.Product_Status, product.Product_Hide == false ? 0 : 1);
            strSql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}','{5}',", product.Product_Mode, product.Product_Sort, product.Product_Start, product.Product_End, product.Page_Content_1, product.Page_Content_2);
            strSql.AppendFormat(" '{0}','{1}','{2}','{3}',", product.Page_Content_3, product.Product_Keywords, product.Product_Recommend, product.Product_Password);
            strSql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}',", product.Product_Total_Click, product.Expect_Time, product.Product_Image, product.Product_Createdate, product.Product_Updatedate);
            strSql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}',", product.Product_Ipfrom, product.Goods_Area, product.Goods_Image1, product.Goods_Image2, product.City);
            strSql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}',", product.Bag_Check_Money, product.Combination, product.Bonus_Percent, product.Default_Bonus_Percent, product.Bonus_Percent_Start);
            strSql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}',", product.Bonus_Percent_End, product.Tax_Type, product.Cate_Id, product.Fortune_Quota, product.Fortune_Freight);
            strSql.AppendFormat(" '{0}','{1}','{2}','{3}',", product.Ignore_Stock, product.Shortage, product.stock_alarm, product.Price_type);
            strSql.AppendFormat(" '{0}','{1}','{2}');select @@identity;", product.user_id, product.show_listprice, product.expect_msg);
            return strSql.ToString();
        }

        public string Update(Model.Product product)
        {
            try
            {
                product.Replace4MySQL();
                StringBuilder strSql = new StringBuilder("SET sql_safe_updates = 0;update product set ");
                strSql.AppendFormat(" brand_id={0},product_vendor_code='{1}',product_name='{2}',product_price_list={3}", product.Brand_Id, product.Product_Vendor_Code, product.Product_Name, product.Product_Price_List);
                strSql.AppendFormat(",product_spec={0},spec_title_1='{1}',spec_title_2='{2}',product_freight_set={3}", product.Product_Spec, product.Spec_Title_1, product.Spec_Title_2, product.Product_Freight_Set);
                strSql.AppendFormat(",product_buy_limit={0},product_status={1},product_hide={2},product_mode={3}", product.Product_Buy_Limit, product.Product_Status, product.Product_Hide, product.Product_Mode);
                strSql.AppendFormat(",product_sort={0},product_start={1},product_end={2},page_content_1='{3}'", product.Product_Sort, product.Product_Start, product.Product_End, product.Page_Content_1);
                strSql.AppendFormat(",page_content_2='{0}',page_content_3='{1}',product_keywords='{2}',product_media='{3}'", product.Page_Content_2, product.Page_Content_3, product.Product_Keywords, product.product_media);
                strSql.AppendFormat(",product_recommend={0},product_password='{1}',product_total_click={2}", product.Product_Recommend, product.Product_Password, product.Product_Total_Click);
                strSql.AppendFormat(",expect_time={0},product_image='{1}',product_createdate={2},product_updatedate={3},mobile_image='{4}'", product.Expect_Time, product.Product_Image, product.Product_Createdate, Common.CommonFunction.GetPHPTime(), product.Mobile_Image);//product.Product_Updatedate
                strSql.AppendFormat(",product_ipfrom='{0}',goods_area={1},goods_image1='{2}',goods_image2='{3}'", product.Product_Ipfrom, product.Goods_Area, product.Goods_Image1, product.Goods_Image2);
                strSql.AppendFormat(",city='{0}',bag_check_money={1},combination={2},bonus_percent={3}", product.City, product.Bag_Check_Money, product.Combination, product.Bonus_Percent);
                strSql.AppendFormat(",default_bonus_percent={0},bonus_percent_start={1},bonus_percent_end={2},tax_type={3}", product.Default_Bonus_Percent, product.Bonus_Percent_Start, product.Bonus_Percent_End, product.Tax_Type);
                strSql.AppendFormat(",ignore_stock={0},shortage={1},stock_alarm={2}", product.Ignore_Stock, product.Shortage, product.stock_alarm);
                strSql.AppendFormat(",cate_id='{0}',fortune_quota={1},fortune_freight={2},shortage={3},price_type={4}", product.Cate_Id, product.Fortune_Quota, product.Fortune_Freight, product.Shortage, product.Price_type);
                strSql.AppendFormat(",show_listprice={0},expect_msg='{1}',process_type={2},show_in_deliver={3},prepaid={4},product_type={5},prod_name='{6}', prod_sz='{7}',prod_classify={8} ",
                    product.show_listprice, product.expect_msg, product.Process_Type, product.Show_In_Deliver, product.Prepaid, product.Product_Type, product.Prod_Name, product.Prod_Sz, product.Prod_Classify);//新增Process_Type ，Show_In_Deliver，Prepaid，Product_Type四個欄位 edit by xiangwang0413w 2014/09/26
                strSql.AppendFormat(",deliver_days={0},min_purchase_amount={1},safe_stock_amount={2},extra_days={3},product_alt='{4}',purchase_in_advance={5},purchase_in_advance_start = {6},purchase_in_advance_end={7}", product.Deliver_Days, product.Min_Purchase_Amount, product.Safe_Stock_Amount, product.Extra_Days, product.Product_alt, product.purchase_in_advance, product.purchase_in_advance_start, product.purchase_in_advance_end);// add by zhuoqin0830w 新增5個修改欄位  2015/03/17
                strSql.AppendFormat(" where product_id={0};SET sql_safe_updates = 1;", product.Product_Id);
                ///add by wwei0216w 2015/7/30 添加預購3欄位
                //strSql.Append(pmDao.UpdateProductName(product.Prod_Sz,product.Product_Id.ToString()));
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.Update-->" + ex.Message, ex);
            }
        }

        public string UpdateSort(Model.Product product)
        {
            try
            {
                product.Replace4MySQL();
                StringBuilder strSql = new StringBuilder("update product set ");
                strSql.AppendFormat(" product_sort={0} where product_id={1} ", product.Product_Sort, product.Product_Id);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.UpdateSort-->" + ex.Message, ex);
            }
        }

        public uint QueryMaxSort(uint brandId)
        {
            try
            {
                string strSql = string.Format("select max(product_sort) as product_sort from product where brand_id={0}", brandId);
                return _dbAccess.getSinggleObj<Product>(strSql).Product_Sort;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.UpdateSort-->" + ex.Message, ex);
            }
        }

        public int TempMove2Pro(string product, string courseProduct, string proItem, string courDetItem, string selPro, string priceMaster, string itemPrice, ArrayList sqls)
        {
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            SerialDao serialDao = new SerialDao("");
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                #region Product
                mySqlCmd.CommandText = serialDao.Update(17);//17 商品流水號
                int productId = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                mySqlCmd.CommandText = string.Format(product, productId);
                int i = mySqlCmd.ExecuteNonQuery();
                #endregion

                #region CoruseProduct
                if (!string.IsNullOrEmpty(courseProduct))
                {
                    mySqlCmd.CommandText = string.Format(courseProduct, productId);
                    mySqlCmd.ExecuteNonQuery();
                }

                #endregion



                #region 單一商品取出的是product_item_temp 組合商品取出的是price_master_temp
                mySqlCmd.CommandText = selPro;
                MySqlDataReader reader = mySqlCmd.ExecuteReader(System.Data.CommandBehavior.SingleResult);
                System.Data.DataTable _dt = new System.Data.DataTable();
                _dt.Load(reader);
                //List<ProductItemTemp> temps = _dbAccess.getObjByTable<ProductItemTemp>(_dt);
                if (_dt == null)
                {
                    mySqlCmd.Transaction.Rollback();
                    return -1;
                }
                #endregion
                if (!string.IsNullOrEmpty(proItem))//有product_item 為單一商品
                {
                    #region PriceMaster
                    mySqlCmd.CommandText = string.Format(priceMaster, productId);
                    int masterId = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                    #endregion

                    #region ProductItem
                    foreach (System.Data.DataRow item in _dt.Rows)
                    {
                        mySqlCmd.CommandText = serialDao.Update(19);//19 商品價格流水號
                        int itemId = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                        mySqlCmd.CommandText = string.Format(proItem, itemId, productId, item["item_id"]);
                        mySqlCmd.ExecuteNonQuery();

                        #region CourseDetailItem
                        //edit by xiangwang013w 2015/03/10
                        if (!string.IsNullOrEmpty(courDetItem))//有courDetItem則商品為課程
                        {
                            mySqlCmd.CommandText = string.Format(courDetItem, itemId, item["item_id"]);
                            mySqlCmd.ExecuteNonQuery();
                        }
                        #endregion

                        #region ItemPrice
                        //mySqlCmd.CommandText = string.Format(itemPrice, masterId, productId);
                        mySqlCmd.CommandText = string.Format(itemPrice, masterId, itemId, item["item_id"]);
                        mySqlCmd.ExecuteNonQuery();
                        #endregion

                    }
                    #endregion



                }
                else
                {
                    #region PriceMaster ItemPrice
                    int parentMaster = 0;
                    foreach (System.Data.DataRow item in _dt.Rows)
                    {
                        mySqlCmd.CommandText = string.Format(priceMaster, productId, item["price_master_id"]);
                        int masterId = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                        if (item["product_id"].ToString() == item["child_id"].ToString())
                        {
                            parentMaster = masterId;
                        }
                        mySqlCmd.CommandText = string.Format(itemPrice, masterId, item["price_master_id"]);
                        mySqlCmd.ExecuteNonQuery();
                    }
                    if (parentMaster != 0)
                    {
                        mySqlCmd.CommandText = string.Format("set sql_safe_updates = 0;update price_master set child_id={0} where price_master_id={1}; set sql_safe_updates = 1;", productId, parentMaster);
                        mySqlCmd.ExecuteNonQuery();
                    }
                    #endregion
                }
                #region spec  picture  notice  tag  category
                foreach (var item in sqls)
                {
                    mySqlCmd.CommandText = string.Format(item.ToString(), productId);
                    mySqlCmd.ExecuteNonQuery();
                }
                #endregion
                mySqlCmd.Transaction.Commit();
                return productId;
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("ProductDao.TempMove2Pro-->" + ex.Message, ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
        }

        public bool ProductMigration(string product, ArrayList priceMasters, ArrayList items, ArrayList itemPrices, ArrayList sqls, ArrayList specs)
        {
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            SerialDao serialDao = new SerialDao("");
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;

                #region Product

                mySqlCmd.CommandText = serialDao.Update(17);//17 商品流水號
                int productId = Convert.ToInt32(mySqlCmd.ExecuteScalar());

                mySqlCmd.CommandText = product.Replace("{0}", productId.ToString()); //string.Format(product, productId);
                mySqlCmd.ExecuteNonQuery();
                #endregion

                List<string> priceMasterIds = new List<string>();
                #region PriceMaster

                if (priceMasters != null)
                {
                    for (int i = 0; i < priceMasters.Count; i++)
                    {
                        mySqlCmd.CommandText = priceMasters[i].ToString().Replace("{0}", productId.ToString()); //string.Format(priceMasters[i].ToString(), productId);
                        priceMasterIds.Add(mySqlCmd.ExecuteScalar().ToString());
                    }
                }
                //組合商品
                if (itemPrices == null || itemPrices.Count == 0)
                {
                    mySqlCmd.CommandText = string.Format("set sql_safe_updates = 0;update price_master set child_id={0} where product_id={0} and child_id=0; set sql_safe_updates = 1;", productId);
                    mySqlCmd.ExecuteNonQuery();
                }
                #endregion

                #region ProductItem

                if (items != null)
                {
                    for (int j = 0; j < items.Count; j++)
                    {
                        mySqlCmd.CommandText = serialDao.Update(19);//19 商品價格流水號
                        int itemId = Convert.ToInt32(mySqlCmd.ExecuteScalar());

                        mySqlCmd.CommandText = string.Format(items[j].ToString(), itemId, productId);
                        mySqlCmd.ExecuteNonQuery();

                        if (itemPrices != null)
                        {
                            for (int k = 0; k < itemPrices.Count; k++)
                            {
                                ArrayList price = itemPrices[k] as ArrayList;
                                mySqlCmd.CommandText = string.Format(price[j].ToString(), priceMasterIds[k], itemId);
                                mySqlCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                #endregion

                #region ProductSpec
                int specId = 0;
                if (specs != null)
                {
                    foreach (string str in specs)
                    {
                        mySqlCmd.CommandText = serialDao.Update(18);//規格編號
                        specId = Convert.ToInt32(mySqlCmd.ExecuteScalar());

                        mySqlCmd.CommandText = str.Replace("{0}", specId.ToString()).Replace("{1}", productId.ToString()); //string.Format(str, specId, productId);
                        mySqlCmd.ExecuteNonQuery();
                    }
                }
                #endregion

                #region Others

                if (sqls != null)
                {
                    foreach (var item in sqls)
                    {
                        mySqlCmd.CommandText = item.ToString().Replace("{0}", productId.ToString()); //string.Format(item.ToString(), productId);
                        mySqlCmd.ExecuteNonQuery();
                    }
                }
                #endregion

                mySqlCmd.Transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("ProductDao.ProductMigration-->" + ex.Message, ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
        }

        //價格審核列表查詢
        public List<Model.Custom.QueryandVerifyCustom> QueryandVerify(Model.Query.QueryVerifyCondition qcCon, ref int total)
        {
            try
            {
                qcCon.Replace4MySQL();
                StringBuilder condition = new StringBuilder();
                StringBuilder group = new StringBuilder();
                StringBuilder colName = new StringBuilder("select a.product_image,a.product_id,b.brand_name,e.product_name,a.prod_sz,a.combination AS combination_id,");
                colName.Append("a.product_status AS product_status_id,f.site_name,h.user_email,e.price_status AS price_status_id,");
                colName.Append("e.price,e.event_price,e.event_start,e.cost,e.event_cost,e.event_end,i.apply_time,k.user_username as apply_user,i.apply_user as user_id,e.price_master_id,e.apply_id");  //edit by wangwei0216w 功能: 2014/8/8在sql語句中添加了e.cost,e.event_cost 2014/8/8
                colName.Append(",e.site_id,e.user_level as level,e.user_id as user");
                //condition.Append(" from price_master e");
                condition.Append(" from price_master_ts e"); //價格審核列表不再查詢price_master正式表，而是查詢price_master_ts表 edit by xiangwang0413w 2014/07/17 
                condition.Append(" left join product a on a.product_id=e.product_id");
                condition.Append(" left join vendor_brand b on a.brand_id=b.brand_id ");
                //condition.Append(" left join (select parametername,parametercode from t_parametersrc where parametertype='combo_type') c on a.combination=c.parametercode");
                //condition.Append(" left join (select parametername,parametercode from t_parametersrc where parametertype='product_status') d on a.product_status=d.parametercode");
                condition.Append(" left join site f on e.site_id=f.site_id");
                //condition.Append(" left join (select parametername,parametercode from t_parametersrc where parametertype='userlevel') g on e.user_level=g.parametercode");
                condition.Append(" left join users h on e.user_id = h.user_id ");
                condition.Append(" right join price_update_apply i on e.apply_id = i.apply_id ");
                condition.Append(" right join manage_user k on k.user_id = i.apply_user ");
                //condition.Append(" left join (select parametername,parametercode from t_parametersrc where parametertype='price_status') j on e.price_status=j.parametercode ");
                condition.Append(" where e.price_status=2 and (e.child_id=e.product_id or e.child_id=0) ");
                if (qcCon.brand_id != 0)
                {
                    condition.AppendFormat(" and a.brand_id={0}", qcCon.brand_id);
                }
                if (qcCon.site_id != 0)
                {
                    condition.AppendFormat(" and e.site_id={0}", qcCon.site_id);
                }
                if (qcCon.user_level != 0)
                {
                    condition.AppendFormat(" and e.user_level={0}", qcCon.user_level);
                }
                if (qcCon.combination != 0)
                {
                    condition.AppendFormat(" and a.combination={0}", qcCon.combination);
                }
                if (qcCon.product_status != -1)
                {
                    condition.AppendFormat(" and a.product_status={0}", qcCon.product_status);
                }
                if (!(qcCon.time_start == "" && qcCon.time_end == "") && !string.IsNullOrEmpty(qcCon.date_type))
                {
                    switch (qcCon.date_type)
                    {
                        case "apply_time": CheckCondition(qcCon, "i", condition); break;     //申請日期
                        case "product_start": CheckCondition(qcCon, "a", condition); break; //上架日期
                        case "product_end": CheckCondition(qcCon, "a", condition); break;    //下架日期
                        default:
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(qcCon.name_number))
                {
                    //condition.AppendFormat(" and (a.product_name like '%{0}%' or a.product_id='{0}')", qcCon.name_number);
                    condition.AppendFormat(new Regex("^[0-9,]+$").IsMatch(qcCon.name_number) ? " and (a.product_id in ({0}) or a.product_name like '%{0}%') " : " and a.product_name like '%{0}%'", qcCon.name_number);
                }
                condition.Append(" and a.product_id is not null");
                group.Append(" group by a.product_image,a.product_id,b.brand_name,a.product_name, combination,");
                group.Append("product_status,f.site_name,h.user_email,price_status,");
                group.Append("e.price,e.event_price,e.event_start,e.event_end,i.apply_time,i.apply_user,e.price_master_id");
                total = int.Parse(_dbAccess.getDataTable("select count(a.product_id) " + condition.ToString()).Rows[0][0].ToString());
                group.Append(" order by a.product_id desc ");
                group.AppendFormat(" limit {0},{1}", qcCon.Start, qcCon.Limit);

                var a = colName.Append(condition).Append(group).ToString();

                IParametersrcImplDao _parameterDao = new ParametersrcDao(connStr);
                List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("combo_type", "product_status", "userlevel", "price_status");
                List<Model.Custom.QueryandVerifyCustom> list = _dbAccess.getDataTableForObj<Model.Custom.QueryandVerifyCustom>(a);
                foreach (QueryandVerifyCustom q in list)
                {
                    var alist = parameterList.Find(m => m.ParameterType == "Combo_Type" && m.ParameterCode == q.combination_id.ToString());
                    var blist = parameterList.Find(m => m.ParameterType == "product_status" && m.ParameterCode == q.product_status_id.ToString());
                    var clist = parameterList.Find(m => m.ParameterType == "UserLevel" && m.ParameterCode == q.level.ToString());
                    var dlist = parameterList.Find(m => m.ParameterType == "price_status" && m.ParameterCode == q.price_status_id.ToString());
                    if (alist != null)
                    {
                        q.combination = alist.parameterName;
                    }
                    if (blist != null)
                    {
                        q.product_status = blist.parameterName;
                    }
                    if (clist != null)
                    {
                        q.user_level = clist.parameterName;
                    }
                    if (dlist != null)
                    {
                        q.price_status = dlist.parameterName;
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.QueryandVerify-->" + ex.Message, ex);
            }
        }

        #region 代/待審核列表查詢
        /// <summary>
        /// 代/待審核列表查詢
        /// </summary>
        /// <param name="qcCon"></param>
        /// <returns></returns>
        public List<Model.Custom.QueryandVerifyCustom> verifyWaitQuery(Model.Query.QueryVerifyCondition qcCon, out int totalCount)
        {
            try
            {
                qcCon.Replace4MySQL();
                StringBuilder stbCols = new StringBuilder("select distinct a.product_image,a.product_id,b.brand_name,a.product_name,a.prod_sz,a.user_id,u.user_username as user_name,a.combination AS combination_id,");
                stbCols.Append("a.product_price_list,s.prev_status AS prev_status_id,a.product_status AS product_status_id,a.product_freight_set AS product_freight_set_id,a.product_mode AS product_mode_id,a.tax_type,s.apply_time,s.online_mode,a.product_createdate,a.product_start,a.product_end");

                StringBuilder stbTabs = new StringBuilder(" from product a");
                stbTabs.Append(" left join vendor_brand b on a.brand_id=b.brand_id ");
                stbTabs.Append(" left join manage_user u on a.user_id = u.user_id");
                stbTabs.Append(" left join product_status_apply s on a.product_id = s.product_id");
                //stbTabs.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='combo_type') c on a.combination=c.parametercode");
                //stbTabs.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='product_status') p on s.prev_status = p.parametercode");
                //stbTabs.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='product_status') d on a.product_status = d.parametercode");
                //stbTabs.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='product_freight') e on a.product_freight_set = e.parametercode");
                //stbTabs.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='product_mode') f on a.product_mode = f.parametercode");add by wwei0216w 2015/5/18

                StringBuilder stbCon = new StringBuilder(" where 1=1");
                if (qcCon.brand_id != 0)
                {
                    stbCon.AppendFormat(" and a.brand_id={0}", qcCon.brand_id);
                }
                if (!string.IsNullOrEmpty(qcCon.cate_id))
                {
                    stbCon.AppendFormat(" and a.cate_id in  ('{0}')", GetStrbyCate_id(qcCon.cate_id));
                }
                if (qcCon.category_id != 0)
                {
                    stbTabs.Append(" left join product_category_set g on a.product_id = g.product_id");
                    stbCon.AppendFormat(" and g.category_id = {0}", qcCon.category_id);
                }
                if (qcCon.combination != 0)
                {
                    stbCon.AppendFormat(" and a.combination= {0}", qcCon.combination);
                }
                if (qcCon.product_status != -1)
                {
                    stbCon.AppendFormat(" and a.product_status = {0}", qcCon.product_status);
                    //當商品狀態為申請審核時,申請表中必須要有對應記錄
                    if (qcCon.product_status == 1)
                    {
                        stbCon.AppendFormat(" and s.apply_id <> ''");
                    }
                }
                if (qcCon.prev_status != -1)
                {
                    stbCon.AppendFormat(" and s.prev_status= {0}", qcCon.prev_status);
                }
                if (!string.IsNullOrEmpty(qcCon.name_number))
                {
                    //stbCon.AppendFormat(" and (a.product_name like '%{0}%' or a.product_id='{0}')", qcCon.name_number);
                    stbCon.AppendFormat(new Regex("^[0-9,]+$").IsMatch(qcCon.name_number) ? " and (a.product_id in ({0}) or a.product_name like '%{0}%') " : " and a.product_name like '%{0}%'", qcCon.name_number);
                }

                if (!string.IsNullOrEmpty(qcCon.date_type))
                {
                    switch (qcCon.date_type)
                    {
                        case "product_createdate": CheckCondition(qcCon, "a", stbCon); break; //建立日期
                        case "apply_time": CheckCondition(qcCon, "s", stbCon); break;    //申請日期
                        case "product_start": CheckCondition(qcCon, "a", stbCon); break; //上架日期
                        case "product_end": CheckCondition(qcCon, "a", stbCon); break;   //下架日期
                        default:
                            break;
                    }
                }

                totalCount = 0;
                System.Data.DataTable _dt = _dbAccess.getDataTable("select count(a.product_id) as totalCount" + stbTabs.ToString() + stbCon.ToString());
                if (_dt != null)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                }


                stbCon.Append(" order by product_id desc");

                if (qcCon.IsPage)
                {
                    stbCon.AppendFormat("  limit {0},{1}", qcCon.Start, qcCon.Limit);
                }

                IParametersrcImplDao _parameterDao = new ParametersrcDao(connStr);
                List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("Combo_Type", "product_status", "product_freight", "product_mode");
                List<Model.Custom.QueryandVerifyCustom> list = _dbAccess.getDataTableForObj<Model.Custom.QueryandVerifyCustom>(stbCols.ToString() + stbTabs.ToString() + stbCon.ToString());
                foreach (QueryandVerifyCustom q in list)
                {
                    var alist = parameterList.Find(m => m.ParameterType == "Combo_Type" && m.ParameterCode == q.combination_id.ToString());
                    var blist = parameterList.Find(m => m.ParameterType == "product_status" && m.ParameterCode == q.prev_status_id.ToString());
                    var clist = parameterList.Find(m => m.ParameterType == "product_status" && m.ParameterCode == q.product_status_id.ToString());
                    var dlist = parameterList.Find(m => m.ParameterType == "product_freight" && m.ParameterCode == q.product_freight_set_id.ToString());
                    var elist = parameterList.Find(m => m.ParameterType == "product_mode" && m.ParameterCode == q.product_mode_id.ToString());
                    if (alist != null)
                    {
                        q.combination = alist.parameterName;
                    }
                    if (blist != null)
                    {
                        q.prev_status = blist.parameterName;
                    }
                    if (clist != null)
                    {
                        q.product_status = clist.parameterName;
                    }
                    if (dlist != null)
                    {
                        q.product_freight_set = dlist.parameterName;
                    }
                    if (elist != null)
                    {
                        q.product_mode = elist.parameterName;
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.verifyWaitQuery-->" + ex.Message, ex);
            }
        }
        #endregion

        #region  判斷日期區間
        public void CheckCondition(Model.Query.QueryVerifyCondition qcCon, string table, StringBuilder stb)
        {
            if (qcCon.time_end != "")
            {
                stb.AppendFormat(" and {0}.{1}<='{2}'", table, qcCon.date_type, qcCon.time_end);
            }
            if (qcCon.time_start != "")
            {
                stb.AppendFormat(" and {0}.{1}>='{2}'", table, qcCon.date_type, qcCon.time_start);
            }
        }
        #endregion

        //根據商品列表的查詢條件查詢出商品信息
        public List<Model.Custom.PriceMasterCustom> Query(Model.Query.QueryVerifyCondition query)
        {
            try
            {
                query.Replace4MySQL();
                StringBuilder strCols = new StringBuilder("select a.product_id,concat('【',b.brand_name,'】',a.product_name) as product_name ,a.prod_sz,a.combination, ");
                strCols.Append(" c.price,c.cost,c.site_id,c.user_level,c.user_id ");

                StringBuilder strTbls = new StringBuilder(" from product a ");
                strTbls.Append(" left join vendor_brand b on a.brand_id=b.brand_id ");
                strTbls.Append(" inner join price_master c on a.product_id=c.product_id ");

                StringBuilder strCondition = new StringBuilder(" where 1=1 ");
                if (query.brand_id != 0)
                {
                    strCondition.AppendFormat(" and a.brand_id={0}", query.brand_id);
                }
                //add by zhuoqin0830w  2015/07/22  失格商品篩選
                if (query.off_grade != 0)
                {
                    strCondition.AppendFormat(" and a.off_grade={0} ", query.off_grade);
                }
                if (!string.IsNullOrEmpty(query.cate_id))
                {
                    strCondition.AppendFormat(" and a.cate_id='{0}'", query.cate_id);
                }
                //add by guodong1130w 2015/09/17 預購商品
                if (query.purchase_in_advance != 0)
                {
                        strCondition.AppendFormat(" and a.purchase_in_advance={0}  ", query.purchase_in_advance);
                }
                if (query.category_id != 0)
                {
                    //edit by hjiajun1211w 2014/08/08 父商品查詢
                    IProductCategoryImplDao pcDao = new ProductCategoryDao(connStr);
                    List<Model.ProductCategory> category = pcDao.QueryAll(new ProductCategory());
                    string str = string.Empty;
                    GetAllCategory_id(category, query.category_id, ref str);
                    strTbls.AppendFormat(" inner join (select distinct product_id from product_category_set where category_id in({0})) j on a.product_id=j.product_id ", str);
                }
                if (query.combination != 0)
                {
                    strCondition.AppendFormat(" and a.combination={0}", query.combination);
                }
                if (query.product_status != -1)
                {
                    strCondition.AppendFormat(" and a.product_status={0}", query.product_status);
                }
                if (query.freight != 0)
                {
                    strCondition.AppendFormat(" and a.product_freight_set={0}", query.freight);
                }
                if (query.mode != 0)
                {
                    strCondition.AppendFormat(" and a.product_mode={0}", query.mode);
                }
                if (query.tax_type != 0)
                {
                    strCondition.AppendFormat(" and a.tax_type={0}", query.tax_type);
                }
                if (!string.IsNullOrEmpty(query.date_type))
                {
                    CheckCondition(query, "a", strCondition);
                }

                if (!string.IsNullOrEmpty(query.name_number))
                {
                    strCondition.AppendFormat(new Regex("^[0-9,]+$").IsMatch(query.name_number) ? " and (a.product_id in ({0}) or a.product_name like '%{0}%') " : " and a.product_name like '%{0}%'", query.name_number);
                }

                strCondition.AppendFormat(" and c.site_id={0} and c.user_level={1} and c.user_id={2} and a.combination in (1,2,3,4) ", query.site_id, query.user_level, query.user_id);
                strCondition.Append(" order by a.product_id desc ");
                return _dbAccess.getDataTableForObj<Model.Custom.PriceMasterCustom>(strCols.ToString() + strTbls.ToString() + strCondition.ToString()); ;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.Query(Model.Query.QueryVerifyCondition query)-->" + ex.Message, ex);
            }
        }
        public List<Model.Custom.QueryandVerifyCustom> QueryByProSite(Model.Query.QueryVerifyCondition query, out int totalCount)
        {
            try
            {
                var regex = new Regex("^[0-9,]+$");
                query.Replace4MySQL();
                StringBuilder strCols = new StringBuilder("select  a.product_id,b.brand_name,a.product_image,a.prod_sz,a.combination AS combination_id,a.product_spec AS product_spec_id,");
                strCols.Append("a.product_price_list,a.sale_status AS sale_status_id,v.vendor_name_full,v.vendor_name_simple,v.erp_id,a.product_status as product_status_id,a.user_id, a.create_channel,a.prepaid,a.bag_check_money,a.off_grade ");//添加 失格欄位 a.off_grade  add by zhuoqin0830w  2015/06/30
                //add by wangwei 2014/9/29 添加a.create_channel字段
                strCols.Append(",a.purchase_in_advance_start,a.purchase_in_advance_end,a.expect_time ");//添加預購商品開始時間 ,結束時間 guodong1130w 2015/9/16
                StringBuilder strTbls = new StringBuilder("from product a left join vendor_brand b on a.brand_id=b.brand_id ");
                //strTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='combo_type') c on a.combination=c.parametercode ");
                //strTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='product_spec') d on a.product_spec=d.parametercode ");
                //strTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='product_status') e on a.product_status=e.parametercode ");
                //strTbls.Append(" LEFT JOIN (SELECT parametercode,parametername FROM t_parametersrc WHERE parametertype='sale_status') sa ON a.sale_status = sa.parametercode "); //add by wwei0216w 2015/02/05
                //add by wwei 0216w 2015/5/18 
                strTbls.Append(" LEFT JOIN vendor v ON v.vendor_id = b.vendor_id "); //add by wwei0216w 2015/02/06
                StringBuilder strCondition = new StringBuilder("where 1=1 ");
                if (query.brand_id != 0)
                {
                    strCondition.AppendFormat(" and a.brand_id={0}", query.brand_id);
                }

                //庫存分類 edit by xiangwang0413w 2014/11/24 
                if (query.StockStatus != 0)// 1.庫存為0還可販售 2.補貨中停止販售 3.庫存數<1
                {
                    switch (query.StockStatus)
                    {
                        case 1://1.庫存為0還可販售
                            strCondition.Append(" and a.combination=1 and  a.ignore_stock=1 ");
                            break;
                        case 2:
                            strCondition.Append(" and a.shortage=1");//2.補貨中停止販售
                            break;
                        case 3:
                            strCondition.Append(" and a.combination=1 and a.product_id in (select distinct product_id from product_item pi where item_stock <1 and pi.product_id=a.product_id)");//3.庫存數<1
                            break;
                        default:
                            throw new Exception("unaccepted StockStatus");
                    }
                }

                if (!string.IsNullOrEmpty(query.cate_id))
                {
                    strCondition.AppendFormat(" and a.cate_id in ('{0}')", GetStrbyCate_id(query.cate_id));
                }
                if (query.category_id != 0)
                {//edit by hjiajun1211w 2014/08/08 父商品查詢
                    IProductCategoryImplDao pcDao = new ProductCategoryDao(connStr);
                    List<Model.ProductCategory> category = pcDao.QueryAll(new ProductCategory());
                    string str = string.Empty;
                    GetAllCategory_id(category, query.category_id, ref str);
                    strTbls.AppendFormat(" inner join (select distinct product_id from product_category_set where category_id in({0})) j on a.product_id=j.product_id ", str);
                }
                if (query.combination != 0)
                {
                    strCondition.AppendFormat(" and a.combination={0}", query.combination);
                }
                if (query.product_status != -1)
                {
                    strCondition.AppendFormat(" and a.product_status={0}", query.product_status);
                }
                if (query.product_type != -1)
                {
                    strCondition.AppendFormat(" and a.product_type={0}", query.product_type);
                }
                if (query.freight != 0)
                {
                    strCondition.AppendFormat(" and a.product_freight_set={0}", query.freight);
                }
                if (query.mode != 0)
                {
                    strCondition.AppendFormat(" and a.product_mode={0}", query.mode);
                }
                if (query.tax_type != 0)
                {
                    strCondition.AppendFormat(" and a.tax_type={0}", query.tax_type);
                }
                //add by zhuoqin0830w  2015/03/11  已買斷商品的篩選功能
                if (query.Prepaid != -1)
                {
                    strCondition.AppendFormat(" and a.prepaid={0}", query.Prepaid);
                }
                //add by zhuoqin0830w  2015/06/30  失格商品篩選
                if (query.off_grade != 0)
                {
                    strCondition.AppendFormat(" and a.off_grade={0} ", query.off_grade);
                }
                //add by guodong1130w 2015/09/17 預購商品
                if (query.purchase_in_advance != 0)
                {
                    strCondition.AppendFormat(" and a.purchase_in_advance={0}  ", query.purchase_in_advance);
                }
                if (!string.IsNullOrEmpty(query.date_type))
                {
                    CheckCondition(query, "a", strCondition);
                }
                if (query.price_check) //價格進階搜尋條件
                {
                    strCols.Append(",f.product_name,f.price_master_id,f.user_id as master_user_id,g.site_name,f.site_id,f.user_level as level,f.price_status AS price_status_id,f.price,f.cost,f.event_price,f.event_cost,f.event_start,f.event_end,a.prod_classify  ");//add by wwei0216w 添加f.cost:成本 //添加館別欄位  eidt  by zhuoqin0830w 2015/03/05

                    strTbls.Append("left join price_master f on a.product_id=f.product_id and(f.product_id=f.child_id or f.child_id=0) ");
                    strTbls.Append("left join site g on f.site_id=g.site_id ");
                    //strTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='userlevel') h on f.user_level=h.parametercode ");
                    //strTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='price_status') i on f.price_status=i.parametercode ");
                    strCondition.Append(" and f.site_id is not null ");

                    if (!query.IsPage) //edit by wangwei0216w 當價格條件被選中,並且不分頁,證明為.商品價格匯出 拼接以下條件
                    {
                        //strTbls.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='product_freight') k on a.product_freight_set=k.parametercode ");
                        //strTbls.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='product_mode') l on a.product_mode=l.parametercode ");

                        strCols.Append(" ,a.product_freight_set AS product_freight_set_id,a.product_mode AS product_mode_id,a.tax_type,a.product_start,a.product_end ");

                        strCondition.Append(" and a.product_id > 10000");//匯出時不匯出大於10000的商品
                    }

                    if (!string.IsNullOrEmpty(query.name_number))
                    {
                        strCondition.AppendFormat(regex.IsMatch(query.name_number) ? " and (a.product_id in ({0}) or f.product_name like '%{0}%' or a.product_id in(select distinct product_id from product_item where item_id in({0})))" : " and f.product_name like '%{0}%'", query.name_number);//add by zhuoqin0830w   2015/03/30 添加商品細項編號
                    }
                    if (query.site_id != 0)
                    {
                        strCondition.AppendFormat(" and f.site_id={0}", query.site_id);
                    }
                    if (query.user_level != 0)
                    {
                        strCondition.AppendFormat(" and f.user_level={0}", query.user_level);
                    }
                    if (query.price_status != 0)
                    {
                        strCondition.AppendFormat(" and f.price_status={0}", query.price_status);
                    }
                }
                else
                {
                    strCols.Append(",a.product_name,a.price_type AS price_type_id,a.product_freight_set AS product_freight_set_id,a.product_mode AS product_mode_id,a.tax_type,a.product_sort,a.product_createdate,a.product_start,a.product_end,a.prod_classify ");

                    //strTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='product_freight') f on a.product_freight_set=f.parametercode ");
                    //strTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='product_mode') g on a.product_mode=g.parametercode ");
                    //strTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='price_type') h on a.price_type=h.parametercode ");
                    if (!string.IsNullOrEmpty(query.name_number))
                    {
                        strCondition.AppendFormat(regex.IsMatch(query.name_number) ? " and (a.product_id in ({0}) or a.product_name like '%{0}%' or a.product_id in (select distinct product_id from product_item where item_id in({0})))" : " and a.product_name like '%{0}%'", query.name_number); //add by zhuoqin0830w   2015/03/30 添加商品細項編號
                    }

                    if (!query.IsPage)//匯出時不匯出大於10000的商品
                    {
                        strCondition.Append(" and a.product_id > 10000");
                    }
                }
                //添加 按鈕選擇值的查詢條件  edit by zhuoqin0830w  2015/02/10
                if (query.priceCondition == 2)
                {
                    strTbls.Append("left join price_master l on a.product_id=l.product_id and l.site_id= 1 and l.user_level=1 and l.user_id=0 and (l.product_id=l.child_id or l.child_id=0) ");
                    strCondition.AppendFormat(" and ((a.combination not in(0,1) and a.price_type <>2) or (a.combination = 1 and l.same_price =1))");
                }
                string strCount = "select count(a.product_id) as totalCount " + strTbls.ToString() + strCondition.ToString();
                System.Data.DataTable _dt = _dbAccess.getDataTable(strCount);
                totalCount = 0;
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                }

                if (query.price_check == false)
                {
                    /*
                     *以下3行 add by wwei0216w 2015/8/13
                     *用於查詢商品所對應的子商品item_id 
                     */
                    strTbls.Append(" LEFT JOIN product_item pi ON a.product_id = pi.product_id ");
                    strCols.Append(" ,GROUP_CONCAT(pi.item_id) AS itemIds ");
                    strCondition.Append(" GROUP BY a.product_id ");
                }
                strCondition.Append(" order by a.product_id desc ");
                if (query.IsPage)
                {
                    strCondition.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }

                IParametersrcImplDao _parameterDao = new ParametersrcDao(connStr);
                List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("Combo_Type", "product_spec", "product_status", "sale_status", "UserLevel", "price_status", "product_freight", "product_mode", "Price_Type");
                List<Model.Custom.QueryandVerifyCustom> list = _dbAccess.getDataTableForObj<Model.Custom.QueryandVerifyCustom>(strCols.ToString() + strTbls.ToString() + strCondition.ToString());
                foreach (QueryandVerifyCustom q in list)
                {
                    var alist = parameterList.Find(m => m.ParameterType == "Combo_Type" && m.ParameterCode == q.combination_id.ToString());
                    var blist = parameterList.Find(m => m.ParameterType == "product_spec" && m.ParameterCode == q.product_spec_id.ToString());
                    var clist = parameterList.Find(m => m.ParameterType == "product_status" && m.ParameterCode == q.product_status_id.ToString());
                    var dlist = parameterList.Find(m => m.ParameterType == "sale_status" && m.ParameterCode == q.sale_status_id.ToString());
                    var elist = parameterList.Find(m => m.ParameterType == "UserLevel" && m.ParameterCode == q.level.ToString());
                    var flist = parameterList.Find(m => m.ParameterType == "price_status" && m.ParameterCode == q.price_status_id.ToString());
                    var glist = parameterList.Find(m => m.ParameterType == "product_freight" && m.ParameterCode == q.product_freight_set_id.ToString());
                    var hlist = parameterList.Find(m => m.ParameterType == "product_mode" && m.ParameterCode == q.product_mode_id.ToString());
                    var jlist = parameterList.Find(m => m.ParameterType == "Price_Type" && m.ParameterCode == q.price_type_id.ToString());
                    if (alist != null)
                    {
                        q.combination = alist.parameterName;
                    }
                    if (blist != null)
                    {
                        q.product_spec = blist.parameterName;
                    }
                    if (clist != null)
                    {
                        q.product_status = clist.parameterName;
                    }
                    if (dlist != null)
                    {
                        q.sale_name = dlist.parameterName;
                    }
                    if (elist != null)
                    {
                        q.user_level = elist.parameterName;
                    }
                    if (flist != null)
                    {
                        q.price_status = flist.parameterName;
                    }
                    if (glist != null)
                    {
                        q.product_freight_set = glist.parameterName;
                    }
                    if (hlist != null)
                    {
                        q.product_mode = hlist.parameterName;
                    }
                    if (jlist != null)
                    {
                        q.price_type = jlist.parameterName;
                    }
                }


                return list;

            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.QueryByProSite-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 預購商品導出  guodong1130w 2015/09/17 添加
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<Model.Custom.QueryandVerifyCustom> QueryForPurchase_in_advance(Model.Query.QueryVerifyCondition query, out int totalCount)
        {
            try
            {
                var regex = new Regex("^[0-9,]+$");
                query.Replace4MySQL();
                StringBuilder strCols = new StringBuilder("select  a.product_id,b.brand_name,v.vendor_name_full,a.combination AS combination_id");
                strCols.Append(",a.purchase_in_advance_start,a.purchase_in_advance_end,a.expect_time,pi.item_stock,sum(odetail.buy_num) as bnum,sdule.schedule_name  ");
                StringBuilder strTbls = new StringBuilder("from product a left join vendor_brand b on a.brand_id=b.brand_id ");
                strTbls.Append(" LEFT JOIN vendor v ON v.vendor_id = b.vendor_id ");
                //連接查出未出貨數量
                strTbls.Append(" LEFT JOIN product_item pi ON pi.product_id = a.product_id ");
                strTbls.Append(" LEFT JOIN order_detail odetail ON odetail.item_id = pi.item_id ");
                strTbls.Append(" LEFT JOIN order_slave oslave ON oslave.slave_id = odetail.slave_id ");
                strTbls.Append(" LEFT JOIN order_master omaster ON omaster.order_id = oslave.order_id  and order_status = 2");
                strTbls.Append(" LEFT JOIN deliver_master dmaster ON dmaster.order_id = omaster.order_id AND dmaster.delivery_status in (0,1,2,3) ");
                //關聯查出排成名稱
                strTbls.Append(" LEFT JOIN schedule_relation srelation on srelation.relation_id=a.product_id and srelation.relation_table='product' ");
                strTbls.Append(" LEFT JOIN schedule sdule on sdule.schedule_id=srelation.schedule_id ");
                StringBuilder strCondition = new StringBuilder(" WHERE 1=1 ");
                if (query.brand_id != 0)
                {
                    strCondition.AppendFormat(" and a.brand_id={0}", query.brand_id);
                }

                //庫存分類 edit by xiangwang0413w 2014/11/24 
                if (query.StockStatus != 0)// 1.庫存為0還可販售 2.補貨中停止販售 3.庫存數<1
                {
                    switch (query.StockStatus)
                    {
                        case 1://1.庫存為0還可販售
                            strCondition.Append(" and a.combination=1 and  a.ignore_stock=1 ");
                            break;
                        case 2:
                            strCondition.Append(" and a.shortage=1");//2.補貨中停止販售
                            break;
                        case 3:
                            strCondition.Append(" and a.combination=1 and a.product_id in (select distinct product_id from product_item pi where item_stock <1 and pi.product_id=a.product_id)");//3.庫存數<1
                            break;
                        default:
                            throw new Exception("unaccepted StockStatus");
                    }
                }

                if (!string.IsNullOrEmpty(query.cate_id))
                {
                    strCondition.AppendFormat(" and a.cate_id in ('{0}')", GetStrbyCate_id(query.cate_id));
                }
                if (query.category_id != 0)
                {//父商品查詢
                    IProductCategoryImplDao pcDao = new ProductCategoryDao(connStr);
                    List<Model.ProductCategory> category = pcDao.QueryAll(new ProductCategory());
                    string str = string.Empty;
                    GetAllCategory_id(category, query.category_id, ref str);
                    strTbls.AppendFormat(" inner join (select distinct product_id from product_category_set where category_id in({0})) j on a.product_id=j.product_id ", str);
                }
                if (query.combination != 0)
                {
                    strCondition.AppendFormat(" and a.combination={0}", query.combination);
                }
                if (query.product_status != -1)
                {
                    strCondition.AppendFormat(" and a.product_status={0}", query.product_status);
                }
                if (query.product_type != -1)
                {
                    strCondition.AppendFormat(" and a.product_type={0}", query.product_type);
                }
                if (query.freight != 0)
                {
                    strCondition.AppendFormat(" and a.product_freight_set={0}", query.freight);
                }
                if (query.mode != 0)
                {
                    strCondition.AppendFormat(" and a.product_mode={0}", query.mode);
                }
                if (query.tax_type != 0)
                {
                    strCondition.AppendFormat(" and a.tax_type={0}", query.tax_type);
                }
                //  已買斷商品的篩選功能
                if (query.Prepaid != -1)
                {
                    strCondition.AppendFormat(" and a.prepaid={0}", query.Prepaid);
                }
                //  失格商品篩選
                if (query.off_grade != 0)
                {
                    strCondition.AppendFormat(" and a.off_grade={0} ", query.off_grade);
                }
                //預購商品
                if (query.purchase_in_advance != 0)
                {
                    strCondition.AppendFormat(" and a.purchase_in_advance={0}  ", query.purchase_in_advance);
                }
                if (!string.IsNullOrEmpty(query.date_type))
                {
                    CheckCondition(query, "a", strCondition);
                }
                strCols.Append(",a.product_name ");


                if (!string.IsNullOrEmpty(query.name_number))
                {
                    strCondition.AppendFormat(regex.IsMatch(query.name_number) ? " and (a.product_id in ({0}) or a.product_name like '%{0}%' or a.product_id in (select distinct product_id from product_item where item_id in({0})))" : " and a.product_name like '%{0}%'", query.name_number); //add by zhuoqin0830w   2015/03/30 添加商品細項編號
                }

                if (!query.IsPage)//匯出時不匯出大於10000的商品
                {
                    strCondition.Append(" and a.product_id > 10000");
                }

                //添加 按鈕選擇值的查詢條件  
                if (query.priceCondition == 2)
                {
                    strTbls.Append("left join price_master l on a.product_id=l.product_id and l.site_id= 1 and l.user_level=1 and l.user_id=0 and (l.product_id=l.child_id or l.child_id=0) ");
                    strCondition.AppendFormat(" and ((a.combination not in(0,1) and a.price_type <>2) or (a.combination = 1 and l.same_price =1))");
                }
                string strCount = "select count(a.product_id) as totalCount " + strTbls.ToString() + strCondition.ToString();
                System.Data.DataTable _dt = _dbAccess.getDataTable(strCount);
                totalCount = 0;
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                }

                strCols.Append(" ,GROUP_CONCAT(pi.item_id) AS itemIds ");
                strCondition.Append(" GROUP BY a.product_id ");
                strCondition.Append(" order by a.product_id desc ");
                if (query.IsPage)
                {
                    strCondition.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }

                IParametersrcImplDao _parameterDao = new ParametersrcDao(connStr);
                List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("Combo_Type", "product_spec", "product_status", "sale_status", "UserLevel", "price_status", "product_freight", "product_mode", "Price_Type");
                List<Model.Custom.QueryandVerifyCustom> list = _dbAccess.getDataTableForObj<Model.Custom.QueryandVerifyCustom>(strCols.ToString() + strTbls.ToString() + strCondition.ToString());
                foreach (QueryandVerifyCustom q in list)
                {
                    var alist = parameterList.Find(m => m.ParameterType == "Combo_Type" && m.ParameterCode == q.combination_id.ToString());
                    if (alist != null)
                    {
                        q.combination = alist.parameterName;
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.QueryForPurchase_in_advance-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// add by jiajun 2014/08/22
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<Model.Custom.QueryandVerifyCustom> QueryForStation(Model.Query.QueryVerifyCondition query, out int totalCount)
        {
            try
            {
                var regex = new Regex("^[0-9,]+$");
                query.Replace4MySQL();
                StringBuilder strCols = new StringBuilder("select  a.product_id,b.brand_name,a.product_image,a.product_name,a.prod_sz,a.product_spec AS product_spec_id,");
                strCols.Append("a.product_price_list,a.product_status as product_status_id,a.combination as combination_id,a.user_id ");

                StringBuilder strTbls = new StringBuilder("from product a left join vendor_brand b on a.brand_id=b.brand_id ");
                //strTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='combo_type') c on a.combination=c.parametercode ");
                //strTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='product_spec') d on a.product_spec=d.parametercode ");
                //strTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='product_status') e on a.product_status=e.parametercode ");

                StringBuilder strCondition = new StringBuilder("where 1=1 ");
                if (query.brand_id != 0)
                {
                    strCondition.AppendFormat(" and a.brand_id={0}", query.brand_id);
                }
                if (!string.IsNullOrEmpty(query.cate_id))
                {

                    strCondition.AppendFormat(" and a.cate_id in ('{0}')", GetStrbyCate_id(query.cate_id));
                }
                if (query.category_id != 0)
                {//edit by hjiajun1211w 2014/08/08 父商品查詢
                    IProductCategoryImplDao pcDao = new ProductCategoryDao(connStr);
                    List<Model.ProductCategory> category = pcDao.QueryAll(new ProductCategory());
                    string str = string.Empty;
                    GetAllCategory_id(category, query.category_id, ref str);
                    strTbls.AppendFormat(" inner join (select distinct product_id from product_category_set where category_id in({0})) j on a.product_id=j.product_id ", str);
                }
                if (query.combination != 0)
                {
                    strCondition.AppendFormat(" and a.combination={0}", query.combination);
                }
                if (query.product_status != -1)
                {
                    strCondition.AppendFormat(" and a.product_status={0}", query.product_status);
                }
                if (query.freight != 0)
                {
                    strCondition.AppendFormat(" and a.product_freight_set={0}", query.freight);
                }
                if (query.mode != 0)
                {
                    strCondition.AppendFormat(" and a.product_mode={0}", query.mode);
                }
                if (query.tax_type != 0)
                {
                    strCondition.AppendFormat(" and a.tax_type={0}", query.tax_type);
                }

                //add by zhuoqin0830w  2015/03/11  已買斷商品的篩選功能
                if (query.Prepaid != -1)
                {
                    strCondition.AppendFormat(" and a.prepaid={0}", query.Prepaid);
                }

                if (!string.IsNullOrEmpty(query.date_type))
                {
                    CheckCondition(query, "a", strCondition);
                }

                strCols.Append(",a.price_type AS price_type_id,a.product_freight_set AS product_freight_set_id,a.product_mode as product_mode_id,a.tax_type,a.product_sort,a.product_createdate,a.product_start,a.product_end ");

                //strTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='product_freight') f on a.product_freight_set=f.parametercode ");
                //strTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='product_mode') g on a.product_mode=g.parametercode ");
                //strTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='price_type') h on a.price_type=h.parametercode ");

                //edit  2014/08/26
                if (query.priceCondition == 2)
                {
                    strTbls.Append("left join price_master l on a.product_id=l.product_id and l.site_id= 1 and l.user_level=1 and l.user_id=0 and (l.product_id=l.child_id or l.child_id=0) ");
                    strCondition.AppendFormat(" and ((a.combination not in(0,1) and a.price_type <>2) or (a.combination = 1 and l.same_price =1))");
                }
                if (!string.IsNullOrEmpty(query.name_number))
                {
                    strCondition.AppendFormat(regex.IsMatch(query.name_number) ? " and (a.product_id in ({0}) or a.product_name like '%{0}%')" : " and a.product_name like '%{0}%'", query.name_number);
                }

                string strCount = "select count(a.product_id) as totalCount " + strTbls.ToString() + strCondition.ToString();
                System.Data.DataTable _dt = _dbAccess.getDataTable(strCount);
                totalCount = 0;
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                }

                strCondition.Append(" order by a.product_id desc ");
                if (query.IsPage)
                {
                    strCondition.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }

                IParametersrcImplDao _parameterDao = new ParametersrcDao(connStr);
                List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("Combo_Type", "product_spec", "product_status", "product_freight", "product_mode", "Price_Type");
                List<Model.Custom.QueryandVerifyCustom> list = _dbAccess.getDataTableForObj<Model.Custom.QueryandVerifyCustom>(strCols.ToString() + strTbls.ToString() + strCondition.ToString());
                foreach (QueryandVerifyCustom q in list)
                {
                    var alist = parameterList.Find(m => m.ParameterType == "Combo_Type" && m.ParameterCode == q.combination_id.ToString());
                    var blist = parameterList.Find(m => m.ParameterType == "product_spec" && m.ParameterCode == q.product_spec_id.ToString());
                    var clist = parameterList.Find(m => m.ParameterType == "product_status" && m.ParameterCode == q.product_status_id.ToString());
                    var dlist = parameterList.Find(m => m.ParameterType == "product_freight" && m.ParameterCode == q.product_freight_set_id.ToString());
                    var elist = parameterList.Find(m => m.ParameterType == "product_mode" && m.ParameterCode == q.product_mode_id.ToString());
                    var flist = parameterList.Find(m => m.ParameterType == "Price_Type" && m.ParameterCode == q.price_type_id.ToString());
                    if (alist != null)
                    {
                        q.combination = alist.parameterName;
                    }
                    if (blist != null)
                    {
                        q.product_spec = blist.parameterName;
                    }
                    if (clist != null)
                    {
                        q.product_status = clist.parameterName;
                    }
                    if (dlist != null)
                    {
                        q.product_freight_set = dlist.parameterName;
                    }
                    if (elist != null)
                    {
                        q.product_mode = elist.parameterName;
                    }
                    if (flist != null)
                    {
                        q.price_type = flist.parameterName;
                    }
                }

                return list;

            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.QueryByProSite-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 遞歸查詢子ID //edit by hjiajun1211w 2014/08/08 父商品查詢
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="rowid"></param>
        public void GetAllCategory_id(List<Model.ProductCategory> category, uint rowid, ref string id)
        {
            List<Model.ProductCategory> query = category.FindAll(p => p.category_father_id == rowid).ToList();
            if (query.Count != 0)
            {
                foreach (var que in query)
                {
                    id += "," + que.category_id.ToString();
                    GetAllCategory_id(category, que.category_id, ref id);
                }
            }
            else if (id.IndexOf(rowid.ToString()) < 0)
            {
                id += "," + rowid.ToString();
            }
            if (id.Substring(0, 1) == ",")
            {
                id = id.Remove(0, 1);
            }
        }

        /// <summary>
        /// 根據cate_id獲取相應的sql語句字段
        /// </summary>
        /// add by wangwei0216w 2014/10/9
        /// <returns>sql語句</returns>
        public string GetStrbyCate_id(string cate_id)
        {
            IParametersrcImplDao _parametersrcDao = new ParametersrcDao(connStr);
            string str = "";
            List<Parametersrc> paras = _parametersrcDao.Query(new Parametersrc() { ParameterType = "product_cate" });
            var subParas = paras.FindAll(p => p.TopValue == cate_id);
            if (subParas.Count == 0)
            {
                str += cate_id + "','";
            }
            else
            {
                subParas.ForEach(p => str += p.ParameterCode + "','");
            }
            return str.Remove(str.Length - 3, 3);
        }

        public List<Model.Custom.QueryandVerifyCustom> QueryByProSite(string priceMasterIds)
        {
            try
            {
                string sqlStr = string.Format(@"select f.price_master_id,a.product_id,b.brand_name,a.product_image,a.product_name,a.prod_sz,a.combination AS combination_id,
                a.product_spec AS product_spec_id,a.product_price_list,a.product_status AS product_status_id,a.product_status as product_status_id,a.price_type as price_type_id,
                a.combination as combination_id,a.user_id ,f.user_id as master_user_id,g.site_name,f.site_id,f.user_level as level,
                f.price_status AS price_status_id,f.price,f.cost,f.event_price,f.event_cost,f.event_start,f.event_end,a.prepaid from product a left join vendor_brand b on a.brand_id=b.brand_id 
                left join price_master f on a.product_id=f.product_id and(f.product_id=f.child_id or f.child_id=0) 
                left join site g on f.site_id=g.site_id 
                where f.price_master_id in({0})  and f.site_id is not null  order by a.product_id desc", priceMasterIds);

                IParametersrcImplDao _parameterDao = new ParametersrcDao(connStr);
                List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("Combo_Type", "product_spec", "product_status", "UserLevel", "price_status", "Price_Type");
                List<Model.Custom.QueryandVerifyCustom> list = _dbAccess.getDataTableForObj<Model.Custom.QueryandVerifyCustom>(sqlStr);
                foreach (QueryandVerifyCustom q in list)
                {
                    var alist = parameterList.Find(m => m.ParameterType == "Combo_Type" && m.ParameterCode == q.combination_id.ToString());
                    var blist = parameterList.Find(m => m.ParameterType == "product_spec" && m.ParameterCode == q.product_spec_id.ToString());
                    var clist = parameterList.Find(m => m.ParameterType == "product_status" && m.ParameterCode == q.product_status_id.ToString());
                    var dlist = parameterList.Find(m => m.ParameterType == "UserLevel" && m.ParameterCode == q.level.ToString());
                    var elist = parameterList.Find(m => m.ParameterType == "price_status" && m.ParameterCode == q.price_status_id.ToString());
                    var flist = parameterList.Find(m => m.ParameterType == "Price_Type" && m.ParameterCode == q.price_type_id.ToString());
                    if (alist != null)
                    {
                        q.combination = alist.parameterName;
                    }
                    if (blist != null)
                    {
                        q.product_spec = blist.parameterName;
                    }
                    if (clist != null)
                    {
                        q.product_status = clist.parameterName;
                    }
                    if (dlist != null)
                    {
                        q.user_level = dlist.parameterName;
                    }
                    if (elist != null)
                    {
                        q.price_status = elist.parameterName;
                    }
                    if (flist != null)
                    {
                        q.price_type = flist.parameterName;
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.QueryByProSite-->" + ex.Message, ex);
            }

        }

        //查詢商品詳細信息
        public Model.Custom.ProductDetailsCustom ProductDetail(Model.Product query)
        {
            try
            {
                StringBuilder stb = new StringBuilder("select  a.combination as product_type,b.brand_name,a.product_name,a.prod_sz,a.product_sort,a.product_vendor_code,a.product_start,a.product_end,a.expect_time,");
                stb.Append("a.product_freight_set AS product_freight_set_id,a.product_mode AS product_mode_id,a.tax_type,a.combination AS combination_id,page_content_1,page_content_2,page_content_3,product_buy_limit,");
                stb.Append(@"product_keywords,a.fortune_quota,fortune_freight,a.product_status AS product_status_id,a.price_type,a.expect_msg,a.create_channel,a.process_type AS process_type_id, a.product_type AS product_kind_id ,a.sale_status,a.show_listprice,a.show_in_deliver,a.prepaid,k.course_name,j.course_id,a.safe_stock_amount,a.deliver_days,a.min_purchase_amount,a.extra_days,purchase_in_advance,purchase_in_advance_start,purchase_in_advance_end 
  from product a");// edit by zhuoqin0830w 增加5個欄位  ,p.safe_stock_amount,p.deliver_days,p.min_purchase_amount,p.extra_days
                stb.Append(" left join vendor_brand b on a.brand_id=b.brand_id ");
                //stb.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='product_freight') c on a.product_freight_set=c.parametercode");
                //stb.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='product_mode') d on a.product_mode=d.parametercode");
                //stb.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='process_type') g on a.process_type=g.parametercode");
                //stb.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='product_type') h on a.product_type=h.parametercode");
                //stb.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='sale_status') i on a.sale_status=i.parametercode");
                //stb.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='product_status') f on a.product_status = f.parametercode");
                //stb.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='combo_type') e on a.combination=e.parametercode ");
                stb.Append("left join course_product j on a.product_id=j.product_id left join course k on j.course_id=k.course_id ");
                stb.Append(" where 1=1 ");
                if (query.Product_Id != 0)
                {
                    stb.AppendFormat(" and a.product_id={0}", query.Product_Id);
                }

                IParametersrcImplDao _parameterDao = new ParametersrcDao(connStr);
                List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("product_freight", "product_mode", "process_type", "product_type", "sale_status", "product_status", "Combo_Type");
                Model.Custom.ProductDetailsCustom list = _dbAccess.getSinggleObj<Model.Custom.ProductDetailsCustom>(stb.ToString());

                var alist = parameterList.Find(m => m.ParameterType == "product_freight" && m.ParameterCode == list.product_freight_set_id.ToString());
                var blist = parameterList.Find(m => m.ParameterType == "product_mode" && m.ParameterCode == list.product_mode_id.ToString());
                var clist = parameterList.Find(m => m.ParameterType == "process_type" && m.ParameterCode == list.process_type_id.ToString());
                var dlist = parameterList.Find(m => m.ParameterType == "product_type" && m.ParameterCode == list.product_kind_id.ToString());
                var elist = parameterList.Find(m => m.ParameterType == "sale_status" && m.ParameterCode == list.sale_status.ToString());
                var flist = parameterList.Find(m => m.ParameterType == "product_status" && m.ParameterCode == list.product_status_id.ToString());
                var glist = parameterList.Find(m => m.ParameterType == "Combo_Type" && m.ParameterCode == list.combination_id.ToString());

                if (alist != null)
                {
                    list.product_freight_set = alist.parameterName;
                }
                if (blist != null)
                {
                    list.product_mode = blist.parameterName;
                }
                if (clist != null)
                {
                    list.process_type = clist.parameterName;
                }
                if (dlist != null)
                {
                    list.product_kind = dlist.parameterName;
                }
                if (elist != null)
                {
                    list.sale_name = elist.parameterName;
                }
                if (flist != null)
                {
                    list.product_status = flist.parameterName;
                }
                if (glist != null)
                {
                    list.combination = glist.parameterName;
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.ProductDetail-->" + ex.Message, ex);
            }
        }

        public string Delete(uint product_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder("delete from product where product_id=" + product_id);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.Delete-->" + ex.Message, ex);
            }
        }

        public Model.Custom.OrderComboAddCustom OrderQuery(Product query, uint user_level, uint user_id, uint site_id)
        {
            StringBuilder stb = new StringBuilder("select distinct a.price_type, b.product_id,b.product_name,b.cost,b.price as product_cost,c.buy_limit,case c.g_must_buy ");
            stb.Append("when 0 then count(c.g_must_buy) else c.g_must_buy end  as g_must_buy, ");
            stb.Append("case c.g_must_buy when 0 then 2 else 3 end as child ");
            stb.Append("from product a ");
            stb.AppendFormat(" inner join price_master b on a.product_id=b.child_id and b.user_level = {0} and b.user_id = {1} and b.site_id = {2}", user_level, user_id, site_id);   //edit by xinglu0624w b.product_id --> b.child_id
            stb.Append(" inner join product_combo c on a.product_id = c.parent_id");
            stb.AppendFormat(" where a.product_id = {0}", query.Product_Id);

            return _dbAccess.getSinggleObj<Model.Custom.OrderComboAddCustom>(stb.ToString());
        }
        public int QueryClassId(int pid)
        {
            StringBuilder stb = new StringBuilder("SELECT DISTINCT p.product_id,p.brand_id,vbs.class_id from product p   ");
            stb.Append(" LEFT JOIN  vendor_brand_set vbs  on vbs.brand_id=p.brand_id ");
            stb.AppendFormat(" where 1=1 and p.product_id='{0}'", pid);
            stb.Append(" LIMIT 1");
            DataTable dt = _dbAccess.getDataTable(stb.ToString());
            if (dt.Rows[0][2].ToString() == "")
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(dt.Rows[0][2].ToString());
            }

        }

        /// <summary>
        /// 根據商品ID獲得商品信息
        /// productID:商品ID
        /// add by wangwei0216w 2014/8/25
        /// </summary>
        public List<Model.Custom.QueryandVerifyCustom> GetProductInfoByID(string productIds)
        {
            StringBuilder sb = new StringBuilder(@"select a.product_image ,a.product_id ,b.brand_name,a.product_name ,a.prod_sz,a.combination AS combination_id,a.combination as combination_id,
             a.price_type as price_type_id,a.product_status AS product_status_id,f.price ,f.cost ,f.event_price,f.event_cost,a.prepaid  from product a 
            left join vendor_brand b on a.brand_id=b.brand_id 
            left join price_master f on f.product_id=a.product_id and f.site_id=1 and f.user_level=1 and f.user_id=0 and (f.product_id=f.child_id or child_id=0)");
            sb.AppendFormat(" where a.product_id in({0})", productIds);
            List<Model.Custom.QueryandVerifyCustom> list1 = _dbAccess.getDataTableForObj<Model.Custom.QueryandVerifyCustom>(sb.ToString());
            IParametersrcImplDao _parameterDao = new ParametersrcDao(connStr);
            List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("Combo_Type", "Price_Type", "product_status");
            foreach (QueryandVerifyCustom q in list1)
            {
                var alist = parameterList.Find(m => m.ParameterType == "Combo_Type" && m.ParameterCode == q.combination_id.ToString());
                var blist = parameterList.Find(m => m.ParameterType == "Price_Type" && m.ParameterCode == q.price_type_id.ToString());
                var clist = parameterList.Find(m => m.ParameterType == "product_status" && m.ParameterCode == q.product_status_id.ToString());
                if (alist != null)
                {
                    q.combination = alist.parameterName;
                }
                if (blist != null)
                {
                    q.price_type = blist.parameterName;
                }
                if (clist != null)
                {
                    q.product_status = clist.parameterName;
                }
            }
            return list1;
        }

        /// <summary>
        /// 類別商品修改中獲取所有商品信息
        /// add by shuangshuang0420j  20141020 10:00
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<Model.Custom.ProductDetailsCustom> GetAllProList(Model.Query.ProductQuery query, out int totalCount)
        {
            StringBuilder stb = new StringBuilder();
            StringBuilder stbCondi = new StringBuilder();
            StringBuilder stbWhere = new StringBuilder();
            query.Replace4MySQL();
            try
            {
                stb.Append(@"select DISTINCT(a.product_id), a.brand_id,b.brand_name,a.product_name,a.product_image,");
                stb.Append("a.product_freight_set AS product_freight_set_id,a.product_mode AS product_mode_id,a.combination AS combination_id,");
                stb.Append("a.product_keywords,a.product_status AS product_status_id, a.process_type AS process_type_id,a.combination as product_type,sale_status ");
                stbCondi.Append(" from (select product_id,brand_id,product_name,product_freight_set,product_mode,process_type,product_type,product_keywords,sale_status,product_status,combination,product_image from  product where product_id >= 10000 and product_status not in(0,20) ) a");//0:新建立商品 20：供應商新建商品
                stbCondi.Append(" left join vendor_brand b on a.brand_id=b.brand_id ");
                //stbCondi.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='product_freight') c on a.product_freight_set=c.parametercode");
                //stbCondi.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='product_mode') d on a.product_mode=d.parametercode");
                //stbCondi.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='process_type') g on a.process_type=g.parametercode");
                //stbCondi.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='product_type') h on a.product_type=h.parametercode");
                //stbCondi.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='sale_status') i on a.sale_status=i.parametercode");
                //stbCondi.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='product_status') f on a.product_status = f.parametercode");
                if (query.isjoincate)
                {
                    stb.Append(",pcs.category_id,pc.category_name ");//在product_category_set中加入sort欄位控制該類別下商品在前台的排序  add by shuangshuang0420j 20141105 15:00
                    stbCondi.Append(" right join product_category_set pcs on pcs.product_id=a.product_id ");
                    stbCondi.Append(" left join product_category pc on pc.category_id=pcs.category_id");//查詢類別中商品時查詢該商品的類別
                }
                else
                {
                    if (query.category_id != 0)//查詢所有商品時按類別查詢
                    {
                        stb.Append(",pcs.category_id,pc.category_name ");
                        stbCondi.Append(" left join product_category_set pcs on pcs.product_id=a.product_id ");
                        stbCondi.Append(" left join product_category pc on pc.category_id=pcs.category_id");
                    }
                }

                //  stbCondi.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='combo_type') e on a.combination=e.parametercode ");
                // stbWhere.AppendFormat(" where  a.product_status not in(0,20) ");//0:新建立商品 20：供應商新建商品
                if (query.Product_Status != 0 && query.Product_Status != 20)
                {
                    stbWhere.AppendFormat(" and a.product_status ='{0}' ", query.Product_Status);
                }
                if (query.category_id != 0)
                {
                    stbWhere.AppendFormat(" and pcs.category_id='{0}'", query.category_id);
                }
                else if (!string.IsNullOrEmpty(query.categoryArry))
                {
                    stbWhere.AppendFormat("and pcs.category_id in ({0})", query.categoryArry);
                }

                if (query.Product_Id != 0)
                {
                    stbWhere.AppendFormat(" and a.product_id = '{0}'", query.Product_Id);
                }
                if (!string.IsNullOrEmpty(query.Product_Id_In))
                {
                    stbWhere.AppendFormat(" and a.product_id in ({0}) ", query.Product_Id_In);
                }
                if (!string.IsNullOrEmpty(query.Product_Name))
                {
                    stbWhere.AppendFormat(" and a.product_name like N'%{0}%'", query.Product_Name);
                }

                if (!string.IsNullOrEmpty(query.siteStr))
                {
                    stbCondi.Append(" left join price_master pm on a.product_id=pm.product_id and(pm.product_id=pm.child_id or pm.child_id=0) ");
                    stbCondi.Append(" left join site s on pm.site_id=s.site_id ");
                    stbWhere.Append(" and s.site_id is not null ");
                    stbWhere.AppendFormat(" and s.site_id in ({0})", query.siteStr);
                }
                if (query.Brand_Id != 0)
                {
                    stbWhere.AppendFormat(" and a.brand_id ='{0}'", query.Brand_Id);
                }
                else if (!string.IsNullOrEmpty(query.brandArry))
                {
                    stbWhere.AppendFormat(" and a.brand_id in ({0})", query.brandArry);
                }
                if (stbWhere.Length != 0)
                {
                    stbCondi.Append(" WHERE ");
                    stbCondi.Append(stbWhere.ToString().TrimStart().Remove(0, 3));
                }
                string strCount = "select count(a.product_id) as totalCount " + stbCondi.ToString();
                System.Data.DataTable _dt = _dbAccess.getDataTable(strCount);
                totalCount = 0;
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                }
                stbCondi.Append(" order by a.product_id desc ");
                if (query.IsPage)
                {
                    stbCondi.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                stb.Append(stbCondi.ToString());

                // return _dbAccess.getDataTableForObj<Model.Custom.ProductDetailsCustom>(stb.ToString());

                IParametersrcImplDao _parameterDao = new ParametersrcDao(connStr);
                List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("product_freight", "product_mode", "process_type", "product_type", "sale_status", "product_status", "Combo_Type");
                List<Model.Custom.ProductDetailsCustom> listStore = _dbAccess.getDataTableForObj<Model.Custom.ProductDetailsCustom>(stb.ToString());
                foreach (ProductDetailsCustom list in listStore)
                {
                    var alist = parameterList.Find(m => m.ParameterType == "product_freight" && m.ParameterCode == list.product_freight_set_id.ToString());
                    var blist = parameterList.Find(m => m.ParameterType == "product_mode" && m.ParameterCode == list.product_mode_id.ToString());
                    var clist = parameterList.Find(m => m.ParameterType == "process_type" && m.ParameterCode == list.process_type_id.ToString());
                    var dlist = parameterList.Find(m => m.ParameterType == "product_type" && m.ParameterCode == list.product_type.ToString());
                    var elist = parameterList.Find(m => m.ParameterType == "sale_status" && m.ParameterCode == list.sale_status.ToString());
                    var flist = parameterList.Find(m => m.ParameterType == "product_status" && m.ParameterCode == list.product_status_id.ToString());
                    var glist = parameterList.Find(m => m.ParameterType == "Combo_Type" && m.ParameterCode == list.combination_id.ToString());

                    if (alist != null)
                    {
                        list.product_freight_set = alist.parameterName;
                    }
                    if (blist != null)
                    {
                        list.product_mode = blist.parameterName;
                    }
                    if (clist != null)
                    {
                        list.process_type = clist.parameterName;
                    }
                    if (dlist != null)
                    {
                        list.product_kind = dlist.parameterName;
                    }
                    if (elist != null)
                    {
                        list.sale_name = elist.parameterName;
                    }
                    if (flist != null)
                    {
                        list.product_status = flist.parameterName;
                    }

                    if (glist != null)
                    {
                        list.combination = glist.parameterName;
                    }
                }
                return listStore;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.GetAllProList-->" + ex.Message, ex);
            }
        }

        #region 供應商後臺相關
        #region 獲取供應商商品
        /// <summary>
        /// 獲取供應商商品
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <param name="totalCount">數據總條數</param>
        /// <returns>與供應商相關的商品列表</returns>
        public List<VenderProductListCustom> GetVendorProduct(VenderProductList query, out int totalCount)
        {
            try
            {
                query.Replace4MySQL();
                StringBuilder sbProCols = new StringBuilder("select a.product_id,a.create_channel as create_channel,b.brand_name,a.product_image,a.product_name,a.prod_sz,c.parametername as combination,d.parametername as product_spec,j.parametername as product_freight_set,a.product_price_list,e.parametername as product_status,a.product_status as product_status_id,k.parametername as product_mode,a.combination as combination_id,f.price_master_id,f.user_id as master_user_id,g.site_name,f.site_id,h.parametername as user_level,f.user_level as level,i.parametername as price_status,f.price,f.event_price,f.cost,f.event_cost,f.event_start,f.event_end ,a.user_id,'0' as temp_status ");
                StringBuilder sbProTempCols = new StringBuilder("select a.product_id,a.create_channel as create_channel,b.brand_name,a.product_image,a.product_name,a.prod_sz,c.parametername as combination,d.parametername as product_spec,j.parametername as product_freight_set,a.product_price_list,e.parametername as product_status,a.product_status as product_status_id,k.parametername as product_mode,a.combination as combination_id,f.price_master_id,f.user_id as master_user_id,g.site_name,f.site_id,h.parametername as user_level,f.user_level as level,i.parametername as price_status,f.price,f.event_price,f.cost,f.event_cost,f.event_start,f.event_end ,a.writer_id as user_id,a.temp_status ");

                StringBuilder sbProTbls = new StringBuilder("from product a ");
                StringBuilder sbProTempTbls = new StringBuilder("from product_temp a ");
                StringBuilder sbTbls = new StringBuilder();

                sbTbls.Append("left join vendor_brand b on a.brand_id=b.brand_id ");
                sbTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='combo_type') c on a.combination=c.parametercode ");          //商品類型
                sbTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='product_spec') d on a.product_spec=d.parametercode ");       //規格
                sbTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='product_status') e on a.product_status=e.parametercode ");   //商品狀態
                sbTbls.Append("left join site g on f.site_id=g.site_id ");
                sbTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='userlevel') h on f.user_level=h.parametercode ");//
                sbTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='price_status') i on f.price_status=i.parametercode ");//活動價，成本，期間
                sbTbls.Append("left join (select parametercode,parametername from t_parametersrc where parametertype='product_freight') j on a.product_freight_set=j.parametercode ");  //運送方式
                sbTbls.Append("left join (select parametercode,parametername from t_parametersrc where parameterType='product_mode') k on a.product_mode=k.parametercode ");            //出貨方式

                sbProTbls.Append("left join price_master f on a.product_id=f.product_id and (f.product_id=f.child_id or f.child_id=0) ");                             //                       //正式表商品價格
                sbProTempTbls.Append("left join price_master_temp f on a.product_id=f.product_id and (f.product_id=f.child_id or f.child_id=0) ");       //                         //臨時表商品價格

                //where 1=1 and f.site_id is not null and f.site_id=1 and f.user_level=1 and f.user_id=0 and a.brand_id in (select brand_id from vendor_brand where vendor_id='2') 
                //where 1=1 and f.site_id is not null and f.site_id=1 and f.user_level=1 and f.user_id=0 and a.temp_status=12 and a.create_channel=2) 
                StringBuilder sbCondition = new StringBuilder("where 1=1 and f.site_id=1 and f.user_level=1 and f.user_id=0 ");
                StringBuilder sbProCondition = new StringBuilder("");
                StringBuilder sbProTempCondition = new StringBuilder(" and temp_status=12 ");

                StringBuilder sbOrderBy = new StringBuilder(" order by a.product_createdate desc ");
                StringBuilder sbLimit = new StringBuilder("");

                if (query.brand_id != 0)    //品牌
                {
                    sbCondition.AppendFormat(" and a.brand_id={0}", query.brand_id);
                }
                if (query.combination != 0)
                {
                    sbCondition.AppendFormat(" and a.combination={0}", query.combination);
                }
                if (query.product_status != -1) //商品狀態
                {
                    sbCondition.AppendFormat(" and a.product_status={0}", query.product_status);
                }
                if (query.freight != 0) //運送方式
                {
                    sbCondition.AppendFormat(" and a.product_freight_set={0}", query.freight);
                }
                if (query.mode != 0)    //出貨方式
                {
                    sbCondition.AppendFormat(" and a.product_mode={0}", query.mode);
                }
                if (!string.IsNullOrEmpty(query.date_type))
                {
                    CheckCondition(query, "a", sbCondition);
                }
                if (query.price_type == 2)
                {
                    sbCondition.AppendFormat(" and a.price_type <>2 and a.combination = 1 and l.same_price <> 0 ");
                }
                if (!string.IsNullOrEmpty(query.name_number))
                {
                    sbCondition.AppendFormat(" and (a.product_name like '%{0}%' or a.product_id='{0}')", query.name_number);
                }
                if (!string.IsNullOrEmpty(query.vendor_id.ToString()))
                {
                    sbCondition.AppendFormat(" and a.brand_id in (select brand_id from vendor_brand where vendor_id='{0}') ", query.vendor_id);
                }
                //string strCount = "select count(b.product_id) as totalCount from((" + sbProCols + sbProTbls + sbTbls + sbCondition + sbProCondition + ") union (" + sbProTempCols + sbProTempTbls + sbTbls + sbCondition + sbProTempCondition + ")) as b";
                string strCount = "(" + sbProCols + sbProTbls + sbTbls + sbCondition + sbProCondition + ") union (" + sbProTempCols + sbProTempTbls + sbTbls + sbCondition + sbProTempCondition + ")";
                System.Data.DataTable _dt = _dbAccess.getDataTable(strCount);
                totalCount = 0;

                if (_dt != null && _dt.Rows.Count > 0)
                {
                    //totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    totalCount = _dt.Rows.Count;
                }
                //sbCondition.Append(" order by a.product_id desc ");
                if (query.IsPage)
                {
                    sbLimit.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                string sql = "select * from((" + sbProTempCols + sbProTempTbls + sbTbls + sbCondition + sbProTempCondition + sbOrderBy + ") union (" + sbProCols + sbProTbls + sbTbls + sbCondition + sbProCondition + sbOrderBy + ")) pp order by pp.temp_status desc  " + sbLimit;

                return _dbAccess.getDataTableForObj<VenderProductListCustom>(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.GetVendorProduct-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 獲取庫存資料信息
        /// </summary>
        /// <param name="query">判斷條件</param>
        /// <param name="totalCount">總共記錄</param>
        /// <returns>符合條件的集合</returns>
        /// Add by wangwei0216w 2014/10/20
        public List<Model.Custom.ProductItemCustom> GetStockInfo(Model.Query.QueryVerifyCondition query)
        {
            try
            {
                //add by zhuoqin0830w  2015/02/25  添加匯出庫存資料中規格一，二的狀態顯示和備註欄位
                StringBuilder strCols = new StringBuilder("SELECT  b.brand_name,p.product_id,item.item_id,item.erp_id,p.product_name,p.prod_sz,s1.spec_name as Spec_Name_1,s2.spec_name as Spec_Name_2,item.item_stock,p.prepaid,item.item_alarm,p.product_status AS Product_Status_Id,item.export_flag,s1.spec_status as Spec_Status1,s2.spec_status as Spec_Status2,item.remark,p.product_freight_set AS Freight_Set_Id,p.product_mode AS Product_Mode_Id");
                //add by wangwei 2014/9/29 添加a.create_channel字段
                StringBuilder strTbls = new StringBuilder(" FROM product p ");
                strTbls.AppendFormat(" INNER JOIN product_item item ON item.product_id = p.product_id ");
                strTbls.AppendFormat(" LEFT JOIN vendor_brand b ON p.brand_id=b.brand_id ");
                //strTbls.Append("LEFT JOIN (SELECT parametercode,parametername FROM t_parametersrc WHERE parametertype='product_status') e ON p.product_status=e.parametercode ");
                //strTbls.Append("LEFT JOIN (SELECT parametercode,parametername FROM t_parametersrc WHERE parametertype='product_freight') f ON p.product_freight_set=f.parametercode ");
                //strTbls.Append("LEFT JOIN (SELECT parametercode,parametername FROM t_parametersrc WHERE parametertype='product_mode') g ON p.product_mode=g.parametercode ");
                strTbls.Append("LEFT JOIN product_spec s1 on item.spec_id_1 = s1.spec_id ");
                strTbls.Append("LEFT JOIN product_spec s2 on item.spec_id_2 = s2.spec_id ");
                StringBuilder strCondition = new StringBuilder(" WHERE 1=1 AND p.product_id > 10000 AND p.combination = 1 ");

                if (query.StockStatus != 0)// 1.庫存為0還可販售 2.補貨中停止販售 3.庫存數<1
                {
                    switch (query.StockStatus)
                    {
                        case 1://1.庫存為0還可販售
                            strCondition.Append(" and p.combination=1 and  p.ignore_stock=1 ");
                            break;
                        case 2:
                            strCondition.Append(" and p.shortage=1");//2.補貨中停止販售
                            break;
                        case 3:
                            strCondition.Append(" and p.combination=1 and p.product_id in (select distinct product_id from product_item pi where item_stock <1 and pi.product_id=a.product_id)");//3.庫存數<1
                            break;
                        default:
                            throw new Exception("unaccepted StockStatus");
                    }
                }

                if (query.brand_id != 0)
                {
                    strCondition.AppendFormat(" AND p.brand_id={0}", query.brand_id);
                }
                if (!string.IsNullOrEmpty(query.cate_id))
                {
                    strCondition.AppendFormat(" AND p.cate_id IN ('{0}')", GetStrbyCate_id(query.cate_id));
                }
                if (query.category_id != 0)
                {
                    IProductCategoryImplDao pcDao = new ProductCategoryDao(connStr);
                    List<Model.ProductCategory> category = pcDao.QueryAll(new ProductCategory());
                    string str = string.Empty;
                    GetAllCategory_id(category, query.category_id, ref str);
                    strTbls.AppendFormat(" INNER JOIN (SELECT DISTINCT product_id FROM product_category_set WHERE category_id IN({0})) j ON p.product_id=j.product_id ", str);
                }
                if (query.product_status != -1)
                {
                    strCondition.AppendFormat(" AND p.product_status={0}", query.product_status);
                }
                if (query.freight != 0)
                {
                    strCondition.AppendFormat(" AND p.product_freight_set={0}", query.freight);
                }
                if (query.mode != 0)
                {
                    strCondition.AppendFormat(" AND p.product_mode={0}", query.mode);
                }
                if (query.tax_type != 0)
                {
                    strCondition.AppendFormat(" AND p.tax_type={0}", query.tax_type);
                }
                //add by zhuoqin0830w  2015/06/30  失格商品篩選
                if (query.off_grade != 0)
                {
                    strCondition.AppendFormat(" AND p.off_grade={0} ", query.off_grade);
                }
                if (!string.IsNullOrEmpty(query.date_type))
                {
                    CheckCondition(query, "p", strCondition);
                }

                if (!string.IsNullOrEmpty(query.name_number))
                {
                    strCondition.AppendFormat(new Regex("^[0-9,]+$").IsMatch(query.name_number) ? " and (p.product_id in ({0}) or p.product_name like '%{0}%' or p.product_id in (select distinct product_id from product_item where item_id in({0})))" : " and p.product_name like '%{0}%'", query.name_number);
                }

                IParametersrcImplDao _parameterDao = new ParametersrcDao(connStr);
                List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("product_status", "product_freight", "product_mode");
                List<Model.Custom.ProductItemCustom> list = _dbAccess.getDataTableForObj<Model.Custom.ProductItemCustom>(strCols.ToString() + strTbls.ToString() + strCondition.ToString());
                foreach (ProductItemCustom q in list)
                {
                    var alist = parameterList.Find(m => m.ParameterType == "product_status" && m.ParameterCode == q.Product_Status_Id.ToString());
                    var blist = parameterList.Find(m => m.ParameterType == "product_freight" && m.ParameterCode == q.Freight_Set_Id.ToString());
                    var clist = parameterList.Find(m => m.ParameterType == "product_mode" && m.ParameterCode == q.Product_Mode_Id.ToString());
                    if (alist != null)
                    {
                        q.Product_Status = alist.parameterName;
                    }
                    if (blist != null)
                    {
                        q.Freight_Set = blist.parameterName;
                    }
                    if (clist != null)
                    {
                        q.Product_Mode = clist.parameterName;
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.GetStoreInfo-->" + ex.Message, ex);
            }

        }


        #endregion
        #region 判斷是否有活動期間的限制
        /// <summary>
        /// 判斷是否有活動期間的限制
        /// </summary>
        /// <param name="qcCon">查詢條件</param>
        /// <param name="table">數據庫表</param>
        /// <param name="stb">sql語句拼接</param>
        public void CheckCondition(Model.Query.VenderProductList qcCon, string table, StringBuilder stb)
        {
            if (qcCon.time_end != "")
            {
                stb.AppendFormat(" and {0}.{1}<='{2}'", table, qcCon.date_type, qcCon.time_end);
            }
            if (qcCon.time_start != "")
            {
                stb.AppendFormat(" and {0}.{1}>='{2}'", table, qcCon.date_type, qcCon.time_start);
            }
        }
        #endregion
        #region 查詢該商品的信息 by 20140904 jialei
        public Model.Custom.ProductDetailsCustom VendorProductDetail(ProductTemp query)
        {
            try
            {
                StringBuilder stb = new StringBuilder("select  a.combination as product_type,b.brand_name,a.product_name,a.prod_sz,a.product_sort,a.product_vendor_code,a.product_start,a.product_end,a.expect_time,");
                stb.Append("c.parametername as product_freight_set,d.parametername as product_mode,a.tax_type,e.parametername as combination,page_content_1,page_content_2,page_content_3,product_buy_limit,");
                stb.Append("product_keywords,a.fortune_quota,fortune_freight,f.parametername as product_status,a.price_type,a.expect_msg from product_temp a");
                stb.Append(" left join vendor_brand b on a.brand_id=b.brand_id ");
                stb.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='product_freight') c on a.product_freight_set=c.parametercode");
                stb.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='product_mode') d on a.product_mode=d.parametercode");
                stb.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='product_status') f on a.product_status = f.parametercode");
                stb.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='combo_type') e on a.combination=e.parametercode where 1=1");
                if (!string.IsNullOrEmpty(query.Product_Id))
                {
                    stb.AppendFormat(" and a.product_id='{0}'", query.Product_Id);
                }
                return _dbAccess.getSinggleObj<Model.Custom.ProductDetailsCustom>(stb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.VendorProductDetail-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 刪除數據用到Query方法（臨時表）
        public List<Product> VendorQuery(ProductTemp query)
        {
            try
            {//product_id no user_id, bonus_ercent_end
                StringBuilder strSql = new StringBuilder("select product_id ,brand_id,product_vendor_code,product_name,prod_sz,product_price_list,product_spec,spec_title_1,spec_title_2,product_image,");
                strSql.Append("product_freight_set,product_buy_limit,product_status,product_hide,product_mode,product_sort,product_start,product_end,page_content_1,page_content_2,");
                strSql.Append("page_content_3,product_keywords,product_recommend,product_password,product_total_click,expect_time,product_image,product_createdate,product_updatedate,");
                strSql.Append("product_ipfrom,goods_area,goods_image1,goods_image2,city,bag_check_money,combination,bonus_percent,default_bonus_percent,bonus_percent_start,");
                strSql.Append("tax_type,cate_id,fortune_quota,fortune_freight,product_media,ignore_stock,shortage,stock_alarm,price_type,show_listprice,expect_msg ");
                strSql.Append(" from product_temp where 1=1 ");
                if (!string.IsNullOrEmpty(query.Product_Id))
                {
                    strSql.AppendFormat(" and product_id='{0}'", query.Product_Id);
                }
                return _dbAccess.getDataTableForObj<Product>(strSql.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.VendorQuery-->" + ex.Message, ex);
            }
        }
        #endregion
        #region Delete Product_temp Data
        public string Vendor_Delete(string product_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendFormat("delete from product_temp where product_id='{0}'", product_id);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.Vendor_Delete-->" + ex.Message, ex);
            }
        }
        #endregion

        #endregion

        public int Yesornoexist(int i, int j)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendFormat(@"SELECT id,product_id,category_id FROM product_category_set WHERE product_id='{0}' and category_id='{1}' ", i, j);
                return _dbAccess.getDataTable(strSql.ToString()).Rows.Count;//是否存在該行數據
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.Yesornoexist-->" + ex.Message, ex);
            }
        }

        public int Updateproductcategoryset(string condition)
        {
            int result = 0;
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
                mySqlCmd.CommandText = condition.ToString();
                result = mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("CbjobDetailDao.Updateproductcategoryset-->" + ex.Message + condition.ToString(), ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
            return result;
        }

        public DataTable Updownerrormessage(ProductCategory pc)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"SELECT 
                                       SUBSTRING(nvd.cde_dt,1,10) as cde_dt , 
                                        nvd.plas_loc_id, 
                                        upc.upc_id, 
                                        loc.lcat_id, 
                                        pext.cde_dt_incr, 
                                        pext.cde_dt_shp, 
                                        pext.cde_dt_var, 
                                        nvd.item_id, 
                                        nvd.prod_qty, 
                                        p.product_name, 
                                        concat(IFNULL(ps1.spec_name,''),IFNULL(ps2.spec_name,'')) as prod_sz
                                        FROM iinvd nvd 
                                        LEFT JOIN iloc loc on loc.loc_id=nvd.plas_loc_id 
                                        LEFT JOIN product_ext pext on pext.item_id=nvd.item_id 
                                        LEFT JOIN product_item pi on pi.item_id=nvd.item_id 
                                        LEFT JOIN product_spec ps1 ON pi.spec_id_1 = ps1.spec_id 
                                        LEFT JOIN product_spec ps2 ON pi.spec_id_2 = ps2.spec_id 
                                        LEFT JOIN product p on p.product_id= pi.product_id 
                                        LEFT JOIN (SELECT * from iupc GROUP BY item_id) upc on upc.item_id=nvd.item_id where pext.pwy_dte_ctl='Y' ");
            try
            {
                return _dbAccess.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdDao.Updownerrormessage-->" + ex.Message + sb.ToString(), ex);
            }
        }

        public int Yesornoexistproduct(int i)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendFormat(@"SELECT product_id FROM product WHERE product_id='{0}'", i);
                return _dbAccess.getDataTable(strSql.ToString()).Rows.Count;//是否存在該行數據
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.Yesornoexist-->" + ex.Message, ex);
            }
        }

        public int Yesornoexistproductcategory(int i)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendFormat(@"SELECT category_id FROM product_category WHERE category_id ='{0}'", i);
                return _dbAccess.getDataTable(strSql.ToString()).Rows.Count;//是否存在該行數據
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.Yesornoexist-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 由productID返回符合的prodName
        /// </summary>
        /// <param name="prodID"></param>
        /// <param name="classID"></param>
        /// <param name="brandID"></param>
        /// <returns></returns>
        public DataTable GetNameForID(int prodID, int classID, int brandID)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append(@"SELECT p.product_id,p.product_name,p.combination FROM (select brand_id,class_id from vendor_brand_set where 1=1 ");
                if (classID != 0)
                {
                    strSql.AppendFormat(@" AND class_id='{0}'", classID);
                }
                if (brandID != 0)
                {
                    strSql.AppendFormat(@" AND brand_id='{0}'", brandID);
                }

                strSql.Append(@") vbs left join product p ON p.brand_id=vbs.brand_id where 1=1 ");
                if (prodID != 0)
                {
                    strSql.AppendFormat(" and p.product_id = '{0}'", prodID);
                }
                return _dbAccess.getDataTable(strSql.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.GetNameForID-->" + ex.Message + strSql.ToString(), ex);
            }
        }

        public DataTable dt()
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            sql.Append("SELECT keywords FROM gigade_keywords;");
            DataTable d = _dbAccess.getDataTable(sql.ToString());
            sb.Append(@"SELECT
product_name as '商品名稱', 
page_content_1 as '商品描述1',
page_content_2 as '商品描述2',
page_content_3 as '商品描述3',
product_keywords as '商品關鍵字' 
from product where  ");
            for (int i = 0; i < d.Rows.Count; i++)
            {
                if (0 == i)
                {
                    sb.Append("  product_name like '%" + d.Rows[i][0] + "%' ");
                }
                else
                {
                    sb.Append(" or product_name like '%" + d.Rows[i][0] + "%' ");
                }

            }
            for (int i = 0; i < d.Rows.Count; i++)
            {
                sb.Append(" or page_content_1 like '%" + d.Rows[i][0] + "%' ");
            }
            for (int i = 0; i < d.Rows.Count; i++)
            {
                sb.Append(" or page_content_2 like '%" + d.Rows[i][0] + "%' ");
            }
            for (int i = 0; i < d.Rows.Count; i++)
            {
                sb.Append(" or page_content_3 like '%" + d.Rows[i][0] + "%' ");
            }
            for (int i = 0; i < d.Rows.Count; i++)
            {
                sb.Append(" or product_keywords like '%" + d.Rows[i][0] + "%' ");
            }


            DataTable dt = _dbAccess.getDataTable(sb.ToString());

            dt.Columns.Add("所含關鍵字", typeof(String));

            //dtHZ.Columns.Add("商品分類", typeof(String));
            // pe.inner_pack_len,pe.inner_pack_wid,pe.inner_pack_hgt
            foreach (DataRow item in dt.Rows)
            {

                StringBuilder sbGoodWords = new StringBuilder();

                for (int i = 0; i < d.Rows.Count; i++)
                {
                    string keywords = d.Rows[i][0].ToString();
                    if (item["product_name"].ToString().Contains(keywords))
                    {
                        sbGoodWords.Append("商品名稱中含有" + keywords);
                    }

                }
                for (int i = 0; i < d.Rows.Count; i++)
                {
                    string keywords = d.Rows[i][0].ToString();
                    if (item["page_content_1"].ToString().Contains(keywords))
                    {
                        sbGoodWords.Append("商品描述1中含有" + keywords);
                    }

                }
                for (int i = 0; i < d.Rows.Count; i++)
                {
                    string keywords = d.Rows[i][0].ToString();
                    if (item["page_content_2"].ToString().Contains(keywords))
                    {
                        sbGoodWords.Append("商品描述2中含有" + keywords);
                    }
                }
                for (int i = 0; i < d.Rows.Count; i++)
                {
                    string keywords = d.Rows[i][0].ToString();
                    if (item["page_content_3"].ToString().Contains(keywords))
                    {
                        sbGoodWords.Append("商品描述3中含有" + keywords);
                    }

                }
                for (int i = 0; i < d.Rows.Count; i++)
                {
                    string keywords = d.Rows[i][0].ToString();
                    if (item["product_keywords"].ToString().Contains(keywords))
                    {
                        sbGoodWords.Append("商品關鍵字中含有" + keywords);
                    }

                }
                //dtHZ.Rows.Add(dr);
                item["所含關鍵字"] = sbGoodWords.ToString();
            }

            return dt;
        }

        //add by wwei0216w 2015/1/27
        /// <summary>
        /// 獲得Product.SaleStatus狀態以及相關的列
        /// </summary>
        /// <param name="p">查詢條件</param>
        /// <returns>List<ProductCustom>集合</returns>
        public string UpdateSaleStatusBatch(Int64 nowTime)
        {
            string sale_status_no = "S" + nowTime + DateTime.Now.Second.ToString(); //創建批次號
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("SET sql_safe_updates = 0;");
                sb.AppendFormat("DELETE FROM product_statuslog WHERE (TO_DAYS(NOW())-TO_DAYS(FROM_UNIXTIME(kdate))) > 30;"); //刪除創建時間大於30天的數據
                sb.AppendFormat(@"INSERT INTO product_statuslog(`sale_status_no`,`product_id`,`old_sale_status`,`kdate`) 
	SELECT '{0}' AS sale_status_no,product_id,sale_status AS old_sale_status , now() AS kdate FROM product p; ", sale_status_no);//備份之前數據資料

                sb.Append(@"UPDATE product p 
                              INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
                              INNER JOIN vendor v ON v.vendor_id=vb.vendor_id
                                  SET sale_status = 0
                            WHERE v.vendor_status <> 2 AND sale_status = 24; "); //將供應商未下架但販售狀態等於24的商品設置為0

                sb.Append(@"UPDATE product p    
                              INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
                              INNER JOIN vendor v ON v.vendor_id=vb.vendor_id
                                  SET sale_status = 24
                              WHERE v.vendor_status = 2 AND sale_status <> 24;");//將供應商下架的商品販售狀態設置為24

                sb.Append(@"UPDATE product p    
                              INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
                                  SET sale_status = 0
                            WHERE vb.brand_status <> 2 AND sale_status = 25;");//將品牌未下架但販售狀態等於25的商品設置為0

                sb.Append(@"UPDATE product p    
                              INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
                                  SET sale_status = 25
                            WHERE vb.brand_status = 2 AND sale_status NOT IN(24,25);");//將品牌下架但販售狀態等於0的商品設置為25

                sb.Append(@"UPDATE product p 
                               SET sale_status=0
                            WHERE p.product_status = 5 AND sale_status = 21;");//將商品上架但販售狀態等於21的商品設置為0

                sb.Append(@"UPDATE product p 
                               SET sale_status = 21
                            WHERE p.product_status <> 5 AND sale_status NOT IN (24,25,21);");//將商品下架的商品販售狀態設置為21

                sb.AppendFormat(@"UPDATE product p
                               SET sale_status = 0
                            WHERE (p.product_start <= {0} AND p.product_end > {0}) AND sale_status = 22;", nowTime);//將商品滿足上架時間但販售狀態等於22的商品設置為0

                sb.AppendFormat(@"UPDATE product p
                               SET sale_status = 22
                            WHERE (p.product_start > {0} OR p.product_end <= {0}) AND sale_status NOT IN (24,25,21,22);", nowTime);//將未滿足時間的商品販售狀態設置為22

                sb.Append(@"UPDATE product p
	                            SET sale_status = 0
                            WHERE sale_status = 23 AND product_id IN (
                                SELECT DISTINCT pm.product_id FROM price_master pm
	                                INNER JOIN item_price ip ON pm.price_master_id = ip.price_master_id
                                WHERE pm.price_status = 1  AND pm.site_id = 1 AND(
                                  (pm.same_price = 1 AND pm.price <> 0 AND (pm.event_start > UNIX_TIMESTAMP(NOW()) OR pm.event_end < UNIX_TIMESTAMP(NOW()) OR (pm.event_start=0 AND pm.event_end=0)))
                                OR(pm.same_price = 1 AND pm.price <> 0 AND pm.event_start <= UNIX_TIMESTAMP(NOW()) AND pm.event_end >= UNIX_TIMESTAMP(NOW()) AND pm.event_price <>0)
                                OR(pm.same_price <> 1 AND ip.item_money <>0 AND (pm.event_start > UNIX_TIMESTAMP(NOW()) OR pm.event_end < UNIX_TIMESTAMP(NOW()) OR (pm.event_start=0 AND pm.event_end=0)))
                                OR(pm.same_price <> 1 AND ip.item_money <>0 AND pm.event_start <= UNIX_TIMESTAMP(NOW()) AND pm.event_end >= UNIX_TIMESTAMP(NOW()) AND ip.event_money <> 0)));");//將滿足價格檔或者價格>0的商品的販售狀態設置為0

                sb.Append(@"UPDATE product p
		                        SET sale_status = 23
	                        WHERE sale_status NOT IN (24,25,21,22,23) AND product_id IN(
		                        SELECT DISTINCT pm.product_id FROM price_master pm
			                        INNER JOIN item_price ip ON pm.price_master_id = ip.price_master_id
		                        WHERE pm.site_id = 1 AND (
			                        (pm.same_price = 1 AND pm.price = 0 ) 
			                        OR(pm.same_price = 1 AND pm.price <> 0 AND pm.event_start <= UNIX_TIMESTAMP(NOW()) AND pm.event_end >= UNIX_TIMESTAMP(NOW()) AND pm.event_price =0)
			                        OR(pm.same_price <> 1 AND ip.item_money = 0 )
			                        OR(pm.same_price <> 1 AND ip.item_money <>0 AND pm.event_start <= UNIX_TIMESTAMP(NOW()) AND pm.event_end >= UNIX_TIMESTAMP(NOW()) AND ip.event_money = 0))
                                        UNION
			                    SELECT pm2.product_id FROM price_master pm2 WHERE pm2.price_status != 1 AND pm2.site_id = 1);");//將未滿足價格檔的商品販售狀態設置為23

                sb.Append(@"UPDATE product p 
                                SET sale_status = 0
                            WHERE p.shortage <> 1 AND sale_status = 11;");//將未停賣的商品的販售狀態設置為0

                sb.Append(@"UPDATE product p 
                                SET sale_status = 11
                            WHERE p.shortage = 1 AND sale_status not IN (24,25,21,22,23,11);");//將停賣的商品販售狀態設置為11

                sb.Append(@"UPDATE product p
                                LEFT JOIN product_item pi ON p.product_id = pi.product_id
                                    SET sale_status = 0
                            WHERE (p.ignore_stock > 0 OR pi.item_stock > 0) AND sale_status = 12;");//將庫存正常但販售狀態=12的商品設置為0

                sb.Append(@"UPDATE product p
                                LEFT JOIN product_item pi ON p.product_id = pi.product_id
                                    SET sale_status = 12
                            WHERE (p.ignore_stock = 0 AND pi.item_stock <= 0) AND sale_status not IN (24,25,21,22,23,11,12);");//將庫存不足但販售狀態=12的商品設置為0

                sb.Append(@"UPDATE product 
                            INNER JOIN (
                                SELECT parent_id FROM product_combo WHERE parent_id NOT IN
                                    (SELECT DISTINCT pc.parent_id  FROM product_combo pc
                                         INNER JOIN product p ON pc.child_id = p.product_id
                                     WHERE p.sale_status = 11))t ON product_id=t.parent_id
                                         SET sale_status = 0
                                     WHERE sale_status = 13;");//將子商品未停賣但狀態等於13的組合商品設置為0

                sb.Append(@"UPDATE product 
                            INNER JOIN (
                                SELECT DISTINCT pc.parent_id  FROM product_combo pc
                                    INNER JOIN product p ON pc.child_id = p.product_id
                                WHERE p.sale_status=11)t ON product_id=t.parent_id
                                    SET sale_status = 13
                            WHERE sale_status not IN (24,25,21,22,23,11,12,13);");//將子商品停賣的組合商品設置為13

                sb.Append(@"UPDATE product 
                                INNER JOIN (
                            SELECT parent_id FROM product_combo WHERE parent_id NOT IN
                            (SELECT DISTINCT pc.parent_id  FROM product_combo pc
                                INNER JOIN product p ON pc.child_id = p.product_id
                            WHERE p.sale_status = 12))t ON product_id=t.parent_id
                                SET sale_status = 0 
                            WHERE sale_status = 14;");//將子商品庫存不足但狀態等於14的組合商品設置為0

                sb.Append(@"UPDATE product 
                                INNER JOIN (
                                    SELECT DISTINCT pc.parent_id  FROM product_combo pc
                                    INNER JOIN product p ON pc.child_id = p.product_id
                                        WHERE p.sale_status=12)t ON product_id=t.parent_id
                                SET sale_status = 14 
                            WHERE sale_status not IN (24,25,21,22,23,11,12,13,14);");////將子商品庫存不足的組合商品設置為13
                sb.Append("UPDATE product SET sale_status=0 WHERE sale_status = -1;SET sql_safe_updates = 1");

                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao-->QueryProductSaleStatus" + ex.Message, ex);
            }
        }

        public string UpdateSaleStatusByCondition(Int64 nowTime, Product p)
        {
            StringBuilder sb = new StringBuilder();
            if (p.Vendor_Id == 0 && p.Brand_Id == 0) return "";
            try
            {
                StringBuilder sb2 = new StringBuilder();
                if (p.Vendor_Id != 0 && p.Brand_Id != 0)
                {
                    sb2.AppendFormat(" AND v.vendor_id = {0} AND vb.brand_id = {1};", p.Vendor_Id, p.Brand_Id);
                }
                else if (p.Vendor_Id != 0)
                {
                    sb2.AppendFormat(" AND v.vendor_id = {0};", p.Vendor_Id);
                }
                else
                {
                    sb2.AppendFormat(" AND vb.brand_id = {0};", p.Brand_Id);
                }

                sb.Append("SET sql_safe_updates = 0;");

                sb.Append(@"UPDATE product p 
                              INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
                              INNER JOIN vendor v ON v.vendor_id=vb.vendor_id
                                  SET sale_status = 0
                            WHERE v.vendor_status <> 2 AND sale_status = 24"); //將供應商未下架但販售狀態等於24的商品設置為0

                sb.Append(sb2);

                sb.AppendFormat(@"UPDATE product p    
                              INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
                              INNER JOIN vendor v ON v.vendor_id=vb.vendor_id
                                  SET sale_status = 24
                              WHERE v.vendor_status = 2 AND sale_status <> 24");//將供應商下架的商品販售狀態設置為24

                sb.Append(sb2);

                sb.AppendFormat(@"UPDATE product p    
                              INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
                              INNER JOIN vendor v ON v.vendor_id=vb.vendor_id
                                  SET sale_status = 0
                            WHERE vb.brand_status <> 2 AND sale_status = 25");//將品牌未下架但販售狀態等於25的商品設置為0
                sb.Append(sb2);

                sb.AppendFormat(@"UPDATE product p    
                              INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
                              INNER JOIN vendor v ON v.vendor_id=vb.vendor_id
                                  SET sale_status = 25
                            WHERE vb.brand_status = 2 AND sale_status NOT IN(24,25)");//將品牌下架但販售狀態等於0的商品設置為25
                sb.Append(sb2);

                sb.AppendFormat(@"UPDATE product p 
                              INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
                              INNER JOIN vendor v ON v.vendor_id=vb.vendor_id
                                SET sale_status=0
                            WHERE p.product_status = 5 AND sale_status = 21");//將商品上架但販售狀態等於21的商品設置為0

                sb.Append(sb2);

                sb.AppendFormat(@"UPDATE product p 
                                INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
                                INNER JOIN vendor v ON v.vendor_id=vb.vendor_id
                               SET sale_status = 21
                            WHERE p.product_status <> 5 AND sale_status NOT IN (24,25,21)");//將商品下架的商品販售狀態設置為21

                sb.Append(sb2);

                sb.AppendFormat(@"UPDATE product p
                                INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
                                INNER JOIN vendor v ON v.vendor_id=vb.vendor_id
                               SET sale_status = 0
                            WHERE (p.product_start <= {0} AND p.product_end > {0}) AND sale_status = 22", nowTime);//將商品滿足上架時間但販售狀態等於22的商品設置為0


                sb.Append(sb2);

                sb.AppendFormat(@"UPDATE product p
                                INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
                                INNER JOIN vendor v ON v.vendor_id=vb.vendor_id
                               SET sale_status = 22
                            WHERE (p.product_start > {0} OR p.product_end <= {0}) AND sale_status NOT IN (24,25,21,22)", nowTime);//將未滿足時間的商品販售狀態設置為22

                sb.Append(sb2);

                sb.AppendFormat(@"UPDATE product p
                                    INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
                                    INNER JOIN vendor v ON v.vendor_id=vb.vendor_id
	                            SET sale_status = 0
                            WHERE sale_status = 23 AND product_id IN (
                                SELECT DISTINCT pm.product_id FROM price_master pm
	                                INNER JOIN item_price ip ON pm.price_master_id = ip.price_master_id
                                WHERE pm.price_status = 1  AND pm.site_id = 1 AND(
                                  (pm.same_price = 1 AND pm.price <> 0 AND (pm.event_start > UNIX_TIMESTAMP(NOW()) OR pm.event_end < UNIX_TIMESTAMP(NOW()) OR (pm.event_start=0 AND pm.event_end=0)))
                                OR(pm.same_price = 1 AND pm.price <> 0 AND pm.event_start <= UNIX_TIMESTAMP(NOW()) AND pm.event_end >= UNIX_TIMESTAMP(NOW()) AND pm.event_price <>0)
                                OR(pm.same_price <> 1 AND ip.item_money <>0 AND (pm.event_start > UNIX_TIMESTAMP(NOW()) OR pm.event_end < UNIX_TIMESTAMP(NOW()) OR (pm.event_start=0 AND pm.event_end=0)))
                                OR(pm.same_price <> 1 AND ip.item_money <>0 AND pm.event_start <= UNIX_TIMESTAMP(NOW()) AND pm.event_end >= UNIX_TIMESTAMP(NOW()) AND ip.event_money <> 0)))");//將滿足價格檔或者價格>0的商品的販售狀態設置為0
                sb.Append(sb2);
                sb.AppendFormat(@"UPDATE product p
                                      INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
                                      INNER JOIN vendor v ON v.vendor_id=vb.vendor_id
		                              SET sale_status = 23
	                              WHERE sale_status NOT IN (24,25,21,22,23) AND product_id IN(
		                              SELECT DISTINCT pm.product_id FROM price_master pm
			                              INNER JOIN item_price ip ON pm.price_master_id = ip.price_master_id
		                              WHERE pm.site_id = 1 AND (
			                        (pm.same_price = 1 AND pm.price = 0 ) 
			                        OR(pm.same_price = 1 AND pm.price <> 0 AND pm.event_start <= UNIX_TIMESTAMP(NOW()) AND pm.event_end >= UNIX_TIMESTAMP(NOW()) AND pm.event_price =0)
			                        OR(pm.same_price <> 1 AND ip.item_money = 0 )
			                        OR(pm.same_price <> 1 AND ip.item_money <>0 AND pm.event_start <= UNIX_TIMESTAMP(NOW()) AND pm.event_end >= UNIX_TIMESTAMP(NOW()) AND ip.event_money = 0))
                                        UNION
			                        SELECT pm2.product_id FROM price_master pm2 WHERE pm2.price_status != 1 AND pm2.site_id = 1)");//將未滿足價格檔的商品販售狀態設置為23

                sb.Append(sb2);

                sb.AppendFormat(@"UPDATE product p 
                                INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
                                INNER JOIN vendor v ON v.vendor_id=vb.vendor_id
                                    SET sale_status = 0
                            WHERE p.shortage <> 1 AND sale_status = 11");//將未停賣的商品的販售狀態設置為0

                sb.Append(sb2);

                sb.AppendFormat(@"UPDATE product p 
                                INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
                                INNER JOIN vendor v ON v.vendor_id=vb.vendor_id
                                SET sale_status = 11
                            WHERE p.shortage = 1 AND sale_status not IN (24,25,21,22,23,11)");//將停賣的商品販售狀態設置為11

                sb.Append(sb2);

                sb.AppendFormat(@"UPDATE product p
                                    INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
                                    INNER JOIN vendor v ON v.vendor_id=vb.vendor_id
                                LEFT JOIN product_item pi ON p.product_id = pi.product_id
                                    SET sale_status = 0
                            WHERE (p.ignore_stock > 0 AND pi.item_stock > 0) AND sale_status = 12");//將庫存正常但販售狀態=12的商品設置為0

                sb.Append(sb2);

                sb.AppendFormat(@"UPDATE product p
                                INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
                                INNER JOIN vendor v ON v.vendor_id=vb.vendor_id
                                LEFT JOIN product_item pi ON p.product_id = pi.product_id
                                    SET sale_status = 12
                            WHERE (p.ignore_stock = 0 AND pi.item_stock <= 0) AND sale_status not IN (24,25,21,22,23,11,12)");//將庫存不足但販售狀態=12的商品設置為0

                sb.Append(sb2);

                sb.AppendFormat(@"UPDATE product p
                            INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
                            INNER JOIN vendor v ON v.vendor_id=vb.vendor_id
                            INNER JOIN (
                                SELECT parent_id FROM product_combo WHERE parent_id NOT IN
                                    (SELECT DISTINCT pc.parent_id  FROM product_combo pc
                                         INNER JOIN product p ON pc.child_id = p.product_id
                                     WHERE p.sale_status = 11))t ON product_id=t.parent_id
                                         SET sale_status = 0
                                     WHERE sale_status = 13");//將子商品未停賣但狀態等於13的組合商品設置為0

                sb.Append(sb2);

                sb.AppendFormat(@"UPDATE product p
                            INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
                            INNER JOIN vendor v ON v.vendor_id=vb.vendor_id
                            INNER JOIN (
                                SELECT DISTINCT pc.parent_id  FROM product_combo pc
                                    INNER JOIN product p ON pc.child_id = p.product_id
                                WHERE p.sale_status=11)t ON product_id=t.parent_id
                                    SET sale_status = 13
                            WHERE sale_status not IN (24,25,21,22,23,11,12,13)");//將子商品停賣的組合商品設置為13

                sb.Append(sb2);

                sb.AppendFormat(@"UPDATE product p
                            INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
                            INNER JOIN vendor v ON v.vendor_id=vb.vendor_id
                                INNER JOIN (
                            SELECT parent_id FROM product_combo WHERE parent_id NOT IN
                            (SELECT DISTINCT pc.parent_id  FROM product_combo pc
                                INNER JOIN product p ON pc.child_id = p.product_id
                            WHERE p.sale_status = 12))t ON product_id=t.parent_id
                                SET sale_status = 0 
                            WHERE sale_status = 14");//將子商品庫存不足但狀態等於14的組合商品設置為0

                sb.Append(sb2);

                sb.AppendFormat(@"UPDATE product p
                                INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
                                INNER JOIN vendor v ON v.vendor_id=vb.vendor_id
                                INNER JOIN (
                                    SELECT DISTINCT pc.parent_id  FROM product_combo pc
                                    INNER JOIN product p ON pc.child_id = p.product_id
                                        WHERE p.sale_status=12)t ON product_id=t.parent_id
                                SET sale_status = 14 
                            WHERE sale_status not IN (24,25,21,22,23,11,12,13,14)");////將子商品庫存不足的組合商品設置為13

                sb.Append(sb2);
                sb.AppendFormat(@"UPDATE product p
                                        INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
                                        INNER JOIN vendor v ON v.vendor_id=vb.vendor_id
                                            SET sale_status=0
                                    WHERE sale_status = -1");
                sb.Append(sb2);

                sb.Append(" SET sql_safe_updates = 1");

                return sb.ToString();


            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao-->UpdateSaleStatusByCondition" + ex.Message, ex);
            }
        }

        //add by wwei0216w 2015/1/28
        /// <summary>
        /// 更新product表中的某一個具體列的值
        /// </summary>
        /// <returns>sql語句</returns>
        public string UpdateColumn(Product p, string columnName)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SET sql_safe_updates = 0; UPDATE product SET {0} = {1} WHERE product_id = {2}; SET sql_safe_updates = 1;", columnName, p.Sale_Status, p.Product_Id);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao-->UpdateSaleStatus" + ex.Message, ex);
            }
        }

        //add by wwei0216w 2015/1/30
        /// <summary>
        /// 獲取單一商品的子商品
        /// </summary>
        /// <returns></returns>
        public ProductCustom QueryProductInfo(uint productId, out List<Product> prods, out List<ProductItem> prodItems, out PriceMaster gigaPM)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SELECT p.product_id,p.product_status,p.product_start,p.product_end,p.ignore_stock,p.shortage,p.price_type,
                                         p.product_type,p.sale_status,pm.price_status,pm.price,v.vendor_status,vb.brand_status,
                                         pm.same_price,pm.event_start,pm.event_end,pm.event_price FROM product p 
	                                       INNER JOIN vendor_brand vb ON vb.brand_id=p.brand_id
	                                       INNER JOIN vendor v ON v.vendor_id=vb.vendor_id
	                                       INNER JOIN price_master pm ON pm.product_id=p.product_id AND pm.site_id = 1 AND pm.user_level = 1 AND pm.user_id = 0
                                       WHERE p.product_id={0};

                                       SELECT p.sale_status FROM product p 
                                       INNER JOIN product_combo c  on p.product_id = c.child_id 
                                       WHERE c.parent_id={0};

                                   SELECT item_stock FROM product_item WHERE product_id={0} AND item_stock <= 0;

                                    SELECT pm.price,pm.same_price,pm.event_start,pm.event_end,pm.event_price FROM  price_master pm 
                                       WHERE pm.product_id={0}  AND pm.site_id = 1 AND pm.user_level = 1 AND pm.user_id = 0;", productId);

                DataSet ds = _dbAccess.getDataSet(sb.ToString());
                ProductCustom prodc = _dbAccess.getObjByTable<ProductCustom>(ds.Tables[0]).FirstOrDefault();
                prods = _dbAccess.getObjByTable<Product>(ds.Tables[1]);
                prodItems = _dbAccess.getObjByTable<ProductItem>(ds.Tables[2]);
                gigaPM = _dbAccess.getObjByTable<PriceMaster>(ds.Tables[3]).FirstOrDefault();
                return prodc;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao-->QuerySingleItem" + ex.Message, ex);
            }
        }

        public DataTable GetVendorProductList(ProductQuery query, out int totalCount)
        {
            query.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            StringBuilder sbadi = new StringBuilder();

            sb.Append(@"SELECT  vdMsg.vendor_id,pt.product_id,pt.brand_id,pt.product_name,pt.product_status,pt.shortage,vdMsg.vendor_name_simple,vdMsg.brand_name,tpPm.parameterName as nProductStatus");
            sbadi.AppendFormat(@" FROM product pt RIGHT JOIN 
(SELECT vd.vendor_name_full,vd.vendor_name_simple,vb.brand_name,vb.brand_name_simple,vd.vendor_id,vb.brand_id FROM vendor vd 
LEFT JOIN vendor_brand vb on vd.vendor_id =vb.vendor_id where vd.vendor_id='{0}') as vdMsg on pt.brand_id=vdMsg.brand_id 
LEFT JOIN (SELECT parameterCode,parameterName FROM t_parametersrc WHERE parameterType='product_status') as tpPm on pt.product_status=tpPm.parameterCode where pt.product_mode in(1,3) and pt.product_id>10000 ", query.Vendor_Id);
            if (!string.IsNullOrEmpty(query.searchcontent.Trim()))
            {
                sbadi.AppendFormat("and pt.product_id in ({0}) ", query.searchcontent.Trim());
            }
            if (query.this_product_state != -1)
            {
                sbadi.AppendFormat("and pt.product_status = '{0}' ", query.this_product_state);
            }
            sbadi.Append(" ORDER BY pt.product_id ASC ");
            try
            {
                totalCount = 0;
                if (query.IsPage)
                {
                    System.Data.DataTable _dt = _dbAccess.getDataTable("SELECT count(1) as totalcounts " + sbadi.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalcounts"]);
                    }

                    sbadi.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                sb.Append(sbadi.ToString());
                return _dbAccess.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao-->GetVendorProductList-->" + ex.Message + sb.ToString(), ex);
            }
        }

        public DataTable GetVendorProductSpec(ProductQuery query, out int totalCount)
        {
            query.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            StringBuilder sbcount = new StringBuilder();

            sb.AppendFormat(@"SELECT pii.product_id,pii.item_id,ps.spec_name as specone,pst.spec_name as spectwo,pii.item_stock,ps.spec_status as spec_status_one,pst.spec_status as spec_status_two,ps.spec_id as spec_id_one,pst.spec_id as spec_id_two FROM product_item pii 
LEFT JOIN product_spec ps on ps.spec_id=pii.spec_id_1 
LEFT JOIN product_spec pst on pst.spec_id=pii.spec_id_2 
WHERE pii.product_id='{0}' ORDER BY pii.item_id ASC ", query.Product_Id);
            sbcount.AppendFormat(@"SELECT count(1) as totalcounts  FROM product_item pii 
LEFT JOIN product_spec ps on ps.spec_id=pii.spec_id_1 
LEFT JOIN product_spec pst on pst.spec_id=pii.spec_id_2 
WHERE pii.product_id='{0}' ", query.Product_Id);
            try
            {
                totalCount = 0;
                if (query.IsPage)
                {
                    System.Data.DataTable _dt = _dbAccess.getDataTable(sbcount.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalcounts"]);
                    }

                    sb.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                return _dbAccess.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao-->GetVendorProductSpec-->" + ex.Message + sb.ToString() + sbcount.ToString(), ex);
            }
        }

        public string UpdateStock(int newstock, int oldstock, int item_id, int vendor)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SET sql_safe_updates = 0; UPDATE product_item SET item_stock = '{0}' WHERE item_id = '{1}';", newstock, item_id);
                sb.AppendFormat("INSERT into table_change_log(user_type,pk_id,change_table,change_field,old_value,new_value,create_user,create_time)values('1','{0}','{1}','{2}','{3}','{4}','{5}','{6}');SET sql_safe_updates = 1;", item_id, "product_item", "item_stock", oldstock, newstock, vendor, Common.CommonFunction.DateTimeToString(DateTime.Now));
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao-->UpdateStock" + ex.Message, ex);
            }
        }

        public Product QueryClassify(uint product_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(" select product_id,product_name,prod_classify from product ");
                sql.AppendFormat(" where product_id ={0}", product_id);
                return _dbAccess.getSinggleObj<Product>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao-->QueryClassify" + ex.Message + sql.ToString(), ex);
            }
        }

        public int GetDefaultArriveDays(Product prod)
        {
            try
            {
                int defaultArriveDays = 0;
                string strSql = string.Format(@"select p.product_mode,v.self_send_days,v.stuff_ware_days,v.dispatch_days from product p
            inner join vendor_brand vb  on p.brand_id=vb.brand_id 
            inner join  vendor v on vb.vendor_id=v.vendor_id  
            where p.product_id='{0}';", prod.Product_Id);

                DataTable dt = _dbAccess.getDataTable(strSql);
                if (dt.Rows.Count > 0)
                {
                    switch (Convert.ToInt32(dt.Rows[0]["product_mode"]))
                    {
                        case 1:
                            defaultArriveDays = Convert.ToInt32(dt.Rows[0]["self_send_days"]);
                            break;
                        case 2:
                            defaultArriveDays = Convert.ToInt32(dt.Rows[0]["stuff_ware_days"]);
                            break;
                        case 3:
                            defaultArriveDays = Convert.ToInt32(dt.Rows[0]["dispatch_days"]);
                            break;
                    }
                }
                return defaultArriveDays;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao-->GetDefaultArriveDays" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 根據productid獲取商品信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public List<Product> GetProductName(Product p)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("select * from product where product_id='{0}';", p.Product_Id);
                return _dbAccess.getDataTableForObj<Product>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao-->GetProductName" + ex.Message + sb.ToString(), ex);
            }
        }


        public string UpdateStatus(int spec_id, int spce_status)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@" UPDATE product_spec SET spec_status = '{0}' WHERE spec_id = '{1}';", spce_status, spec_id);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao-->UpdateStatus" + ex.Message, ex);
            }
        }
        #region 商品詳情說明
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="p"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable GetProductList(ProductQuery p, out int totalCount)
        {
            p.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlfield = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            totalCount = 0;
            try
            {
                sqlfield.AppendLine(@"SELECT v.vendor_id,v.vendor_name_full, p.product_id,p.product_name,p.prod_classify,REPLACE(product_detail_text,'<br/>','\n') as product_detail_text, ");
                sqlfield.AppendFormat(@" p.product_status,u.user_username as create_username ,p.detail_createdate,uu.user_username as update_username ,p.detail_updatedate,vb.brand_name ");
                sqlwhere.AppendLine(@" FROM product p ");
                sqlwhere.AppendLine(@" LEFT JOIN vendor_brand vb ON p.brand_id=vb.brand_id ");
                sqlwhere.AppendLine(@" LEFT JOIN vendor v ON vb.vendor_id=v.vendor_id ");
                sqlwhere.AppendLine(@" LEFT JOIN manage_user u ON u.user_id=p.detail_created ");
                sqlwhere.AppendLine(@" LEFT JOIN manage_user uu ON uu.user_id=p.detail_update ");
                //  sqlwhere.AppendFormat(@" LEFT JOIN ( SELECT parameterCode,parameterName FROM t_parametersrc WHERE parameterType='product_status') pa ON p.product_status=pa.parameterCode ");
                sqlwhere.AppendLine(@" WHERE  p.product_id > 10000 ");
                if (!string.IsNullOrEmpty(p.pids))
                {
                    sqlwhere.AppendFormat(@" AND  p.product_id IN ({0}) ", p.pids);
                }
                if (!string.IsNullOrEmpty(p.Product_Name))
                {
                    string[] pname = p.Product_Name.Split(',');
                    sqlwhere.AppendFormat(@" AND ( ");
                    for (int i = 0; i < pname.Length; i++)
                    {
                        if (i == 0)
                        {
                            sqlwhere.AppendFormat(@"  p.product_name LIKE '%{0}%' ", pname[i]);
                        }
                        else
                        {
                            sqlwhere.AppendFormat(@" OR p.product_name LIKE '%{0}%' ", pname[i]);
                        }

                    }
                    sqlwhere.AppendFormat(@" )");
                }
                if (p.Vendor_Id != 0)
                {
                    sqlwhere.AppendFormat(@" AND v.vendor_id='{0}' ", p.Vendor_Id);
                }
                if (p.Brand_Id != 0)
                {
                    sqlwhere.AppendFormat(@" AND p.brand_id='{0}' ", p.Brand_Id);
                }
                if (p.Product_Status != 999)
                {
                    sqlwhere.AppendFormat(@" AND p.product_status='{0}' ", p.Product_Status);
                }
                //狀態 未編輯
                if (p.product_detail_text == "1")
                {
                    sqlwhere.AppendFormat(@" AND product_detail_text IS  NULL ");
                }
                else if (p.product_detail_text == "2")//已編輯
                {
                    sqlwhere.AppendFormat(@" AND product_detail_text IS NOT NULL ");
                }
                if (!string.IsNullOrEmpty(p.create_username))
                {
                    sqlwhere.AppendFormat(@" AND u.user_username like N'%{0}%'", p.create_username.Trim());
                }
                if (p.time_start != DateTime.MinValue)
                {
                    sqlwhere.AppendFormat(@" AND p.detail_createdate >= '{0}'", Common.CommonFunction.DateTimeToString(p.time_start));
                }
                if (p.time_end != DateTime.MinValue)
                {
                    sqlwhere.AppendFormat(@" AND p.detail_createdate <= '{0}'", Common.CommonFunction.DateTimeToString(p.time_end));
                }
                if (p.Combination != 0)
                {
                    sqlwhere.AppendFormat(@" AND p.combination = '{0}'", p.Combination);
                }
                if (p.vendor_status != 0)
                {
                    sqlwhere.AppendFormat(@" AND v.vendor_status = '{0}'", p.vendor_status);
                }
                if (p.brand_status != 0)
                {
                    sqlwhere.AppendFormat(@" AND vb.brand_status = '{0}'", p.brand_status);
                }
                sql.AppendFormat(sqlfield.ToString() + sqlwhere.ToString());
                if (p.IsPage)
                {
                    sql.AppendFormat(@" LIMIT {0},{1}; ", p.Start, p.Limit);
                    DataTable dt = _dbAccess.getDataTable("SELECT p.product_id " + sqlwhere.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = dt.Rows.Count;
                    }
                }
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao-->GetProductList" + ex.Message + sql.ToString(), ex);
            }

        }
        /// <summary>
        /// 編輯商品詳情文字
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public int UpdateProductDeatail(Product p)
        {
            StringBuilder sql = new StringBuilder();
            p.Replace4MySQL();
            sql.AppendFormat(@"UPDATE product SET product_detail_text='{0}' ,detail_created='{1}',detail_createdate='{2}',detail_update='{3}',detail_updatedate='{4}'  ",
                p.product_detail_text, p.detail_created, Common.CommonFunction.DateTimeToString(p.detail_createdate), p.detail_update, Common.CommonFunction.DateTimeToString(p.detail_updatedate));

            sql.AppendFormat(@"  WHERE product_id='{0}'; ", p.Product_Id);
            try
            {
                return _dbAccess.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao-->UpdateProductDeatail" + ex.Message + sql.ToString(), ex);
            }
        }
        /// <summary>
        /// 供應商下拉列表
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public DataTable GetVendor(Vendor v)
        {
            v.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT vendor_id,vendor_name_simple,vendor_name_full FROM vendor WHERE 1=1");
                if (v.assist != 0)
                {
                    sql.AppendFormat(@" AND assist = '{0}' ", v.assist);
                }
                sql.AppendFormat(@";");
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao-->UpdateProductDeatail" + ex.Message + sql.ToString(), ex);
            }
        }

        public DataTable GetProductDetialText(Product p)
        {
            StringBuilder sql = new StringBuilder();
            DataTable _dt = new DataTable();
            try
            {
                sql.AppendFormat(@"SELECT  product_detail_text FROM product WHERE product_id={0}", p.Product_Id);
                _dt = _dbAccess.getDataTable(sql.ToString());

            }
            catch (Exception ex)
            {

                throw new Exception("ProductDao-->GetProductDetialText" + ex.Message + sql.ToString(), ex);
            }
            return _dt;
        }


        public List<Product> GetProductByVendor(int vendor_id)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SELECT p.product_id FROM product p
                                     INNER JOIN vendor_brand vb ON vb.brand_id = p.brand_id
                                     INNER JOIN vendor v ON v.vendor_id = vb.vendor_id
                                  WHERE p.product_id > 10000 AND v.vendor_id = {0} AND p.combination = 1", vendor_id);
                return _dbAccess.getDataTableForObj<Product>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao-->GetProductByVendor" + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// 根據商品編號更新商品補貨中停止販售狀態
        /// </summary>
        /// <param name="product_id">商品編號</param>
        /// <param name="shortage">販售狀態</param>
        /// <returns>更新結果</returns>
        public int UpdateShortage(int product_id, int shortage)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat("update product set shortage={0},product_updatedate={1} where product_id={2}", shortage, Common.CommonFunction.GetPHPTime(), product_id);
            try
            {
                return _dbAccess.execCommand(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.UpdateShortage-->" + ex.Message + sbSql.ToString(), ex);
            }
        }

        /// <summary>
        /// 設置商品失格(一旦設置,無法還原)
        /// </summary>
        /// <param name="product_id">需要設置商品失格的id</param>
        /// <returns>成功 OR 失敗</returns>
        public bool UpdateOff_Grade(uint product_id, int off_grade)
        {
            StringBuilder sb = new StringBuilder();
            //StringBuilder sb2 = new StringBuilder();
            ArrayList list = new ArrayList();
            try
            {
                ///將單一商品,且狀態等於下架的商品設置為失格  //去掉 AND combination = 1 條件 edit by zhuoqin0830w  2015/07/06
                sb.AppendFormat(@"SET SQL_SAFE_UPDATES = 0; 
                                      UPDATE `product` SET `off_grade` = {0} WHERE product_id IN({1}) AND (product_status = 6 OR product_status = 99);
                                  SET SQL_SAFE_UPDATES = 1;", off_grade, product_id);
                ///設置與該商品相關的組合商品的off_grade
                //                sb2.AppendFormat(@"SET SQL_SAFE_UPDATES = 0;
                //                                   UPDATE `product` p
                //	                                   INNER JOIN product_combo pc ON pc.parent_id = p.product_id AND pc.child_id IN ({0}) 
                //                                           SET p.`off_grade` = -1;
                //                                   SET SQL_SAFE_UPDATES = 0;", product_id);
                ///將語句進行批處理
                //list.Add(sb.ToString());
                //list.Add(sb2.ToString());
                //MySqlDao sql = new MySqlDao(connStr);
                ///返回執行結果
                return _dbAccess.execCommand(sb.ToString()) >= 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Product-->UpdateOff_Grade" + ex.Message, ex);
            }
        }


        public int GetProductType(Product query)
        {
            StringBuilder sbSql = new StringBuilder();
            try
            {
                sbSql.AppendFormat("SELECT combination FROM product WHERE product_id='{0}';", query.Product_Id);
                DataTable _dt = _dbAccess.getDataTable(sbSql.ToString());
                if (_dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(_dt.Rows[0][0]);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao.GetProductType-->" + ex.Message + sbSql.ToString(), ex);
            }
        }
    }
}

