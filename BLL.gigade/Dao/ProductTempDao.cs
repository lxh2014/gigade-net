#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：ProductTempDao.cs
* 摘 要：
* producttemp的表操作
* 当前版本：v1.0
* 作 者： 
* 完成日期：
* 修改歷史：
*         v1.1修改日期：2014/8/18 15:35
*         v1.1修改人員：shuangshuang0420j
*         v1.1修改内容：添加供應商商品審核列表查詢+ List<Model.Custom.QueryandVerifyCustom> verifyWaitQuery(Model.Query.QueryVerifyCondition qcCon, out int totalCount)
*                     判斷日期區間 + void CheckCondition(Model.Query.QueryVerifyCondition qcCon, string table, StringBuilder stb)
 *                     獲得臨時表中供應商新建的數據+ProductTemp GetProTempByVendor(ProductTemp proTemp)
 *                     修改數據+string Update(ProductTemp proTemp)
 *                     
*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;
using System.Collections;
using MySql.Data.MySqlClient;

namespace BLL.gigade.Dao
{
    class ProductTempDao : IProductTempImplDao
    {
        private IDBAccess _dbAccess;
        private string strConn;
        private RecommendedProductAttributeDao reProductDao;
        public ProductTempDao(string connectStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectStr);

            this.strConn = connectStr;
            reProductDao = new RecommendedProductAttributeDao(connectStr);
        }

        public ProductTemp GetProTemp(ProductTemp proTemp)
        {
            try
            {
                StringBuilder strSql = new StringBuilder("select p.writer_id,p.brand_id,product_vendor_code,product_status,product_name,p.prod_name,p.prod_sz,product_price_list,product_spec,spec_title_1,spec_title_2,");
                strSql.Append("product_freight_set,product_buy_limit,product_hide,product_mode,product_sort,product_start,product_end,page_content_1,page_content_2,");
                strSql.Append("page_content_3,product_keywords,product_recommend,product_password,product_total_click,expect_time,product_image,product_createdate,product_updatedate,");
                strSql.Append("product_ipfrom,goods_area,goods_image1,goods_image2,city,bag_check_money,combination,bonus_percent,default_bonus_percent,bonus_percent_start,");
                strSql.Append("bonus_percent_end,tax_type,cate_id,fortune_quota,fortune_freight,product_media,ignore_stock,shortage,stock_alarm,show_listprice,expect_msg,");
                strSql.Append("show_in_deliver,prepaid,process_type,product_type,vb.vendor_id,p.prod_classify,cp.course_id,p.safe_stock_amount,p.deliver_days,p.min_purchase_amount,p.extra_days, p.mobile_image,p.product_alt,purchase_in_advance,purchase_in_advance_start,purchase_in_advance_end,rpat.expend_day,rpat.months from product_temp p ");  // edit by zhuoqin0830w 增加5個欄位  ,p.safe_stock_amount,p.deliver_days,p.min_purchase_amount,p.extra_days 

                //edit by wwei0216w 2015/3/18 查詢屬性多了一個mobile_image列
                strSql.Append("left join vendor_brand vb ON p.brand_id = vb.brand_id ");

                strSql.Append("left join course_product_temp cp on p.product_id=cp.product_id and p.writer_id=cp.writer_id ");
                //edit by dongya0410j 2015/8/24 商品推薦屬性
                strSql.Append(" left join recommended_product_attribute_temp rpat on p.product_id=rpat.product_id and p.writer_id=rpat.write_id and p.combo_type=rpat.combo_type ");
                strSql.AppendFormat("where p.writer_id={0} and p.combo_type={1}", proTemp.Writer_Id, proTemp.Combo_Type);
                strSql.AppendFormat(" and p.product_id='{0}'", proTemp.Product_Id);
                return _dbAccess.getSinggleObj<ProductTemp>(strSql.ToString()); //add by wangwei0216w 查詢時添加product_status字段 2014/9/30
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempDao.GetProTemp-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 保存基本資料
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public int baseInfoSave(ProductTemp p)
        {
            try
            {
                p.Replace4MySQL();
                StringBuilder strSql = new StringBuilder("insert into product_temp( ");
                strSql.Append(" `writer_id`,`product_id`,`brand_id`,`product_vendor_code`,");
                strSql.Append(" `product_name`,`product_price_list`,`product_spec`,`spec_title_1`,`spec_title_2`,");
                strSql.Append(" `product_freight_set`,`product_buy_limit`,`product_status`,`product_hide`,");
                strSql.Append(" `product_mode`,`product_sort`,`product_start`,`product_end`,`page_content_1`,`page_content_2`,");
                strSql.Append(" `page_content_3`,`product_keywords`,`product_recommend`,`product_password`,");
                strSql.Append(" `product_total_click`,`expect_time`,`product_image`,`product_createdate`,`product_updatedate`,");
                strSql.Append(" `product_ipfrom`,`goods_area`,`goods_image1`,`goods_image2`,`city`,");
                strSql.Append(" `bag_check_money`,`combination`,`bonus_percent`,`default_bonus_percent`,`bonus_percent_start`,`bonus_percent_end`,`tax_type`,`cate_id`,");
                strSql.Append(" `fortune_quota`,`fortune_freight`,`combo_type`,`product_media`,`ignore_stock`,`shortage`,`stock_alarm`,`price_type`,`show_listprice`,`expect_msg`,`create_channel`,");
                strSql.Append(" `show_in_deliver`,`prepaid`,`process_type`,`product_type`,`prod_name`,`prod_sz`,`prod_classify`,`deliver_days`,`min_purchase_amount`,`safe_stock_amount`,`extra_days`,`product_alt`,`purchase_in_advance`,`purchase_in_advance_start`,`purchase_in_advance_end`) values ("); //商品新增欄位  add by zhuoqin0830w  2015/03/17
                strSql.AppendFormat(" '{0}','{1}','{2}','{3}',", p.Writer_Id, p.Product_Id, p.Brand_Id, p.Product_Vendor_Code);
                strSql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}',", p.Product_Name, p.Product_Price_List, p.Product_Spec, p.Spec_Title_1, p.Spec_Title_2);
                strSql.AppendFormat(" '{0}','{1}','{2}','{3}',", p.Product_Freight_Set, p.Product_Buy_Limit, p.Product_Status, p.Product_Hide == false ? 0 : 1);
                strSql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}','{5}',", p.Product_Mode, p.Product_Sort, p.Product_Start, p.Product_End, p.Page_Content_1, p.Page_Content_2);
                strSql.AppendFormat(" '{0}','{1}','{2}','{3}',", p.Page_Content_3, p.Product_Keywords, p.Product_Recommend, p.Product_Password);
                strSql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}',", p.Product_Total_Click, p.Expect_Time, p.Product_Image, p.Product_Createdate, p.Product_Updatedate);
                strSql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}',", p.Product_Ipfrom, p.Goods_Area, p.Goods_Image1, p.Goods_Image2, p.City);
                strSql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}',", p.Bag_Check_Money, p.Combination, p.Bonus_Percent, p.Default_Bonus_Percent, p.Bonus_Percent_Start);
                strSql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}','{5}',", p.Bonus_Percent_End, p.Tax_Type, p.Cate_Id, p.Fortune_Quota, p.Fortune_Freight, p.Combo_Type);
                strSql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}',", p.product_media, p.Ignore_Stock, p.Shortage, p.stock_alarm, p.Price_type);
                strSql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',{9},", p.show_listprice, p.expect_msg, p.Create_Channel, p.Show_In_Deliver, p.Prepaid, p.Process_Type, p.Product_Type, p.Prod_Name, p.Prod_Sz, p.Prod_Classify);
                strSql.AppendFormat(" {0},{1},{2},{3},'{4}','{5}','{6}','{7}');", p.Deliver_Days, p.Min_Purchase_Amount, p.Safe_Stock_Amount, p.Extra_Days, p.Product_alt, p.purchase_in_advance, p.purchase_in_advance_start, p.purchase_in_advance_end);
     
                if (p.expend_day>=0&&p.months!="")//新增單一商品或者組合商品商品推薦臨時表
                {
                    strSql.AppendFormat("insert into recommended_product_attribute_temp(`product_id`,`write_id`,`time_start`,`time_end`,`expend_day`,`months`,`combo_type`)values('{0}','{1}','{2}','{3}','{4}','{5}','{6}');", p.Product_Id, p.Writer_Id, 0, 0, p.expend_day, p.months,p.Combo_Type);
                }
                return _dbAccess.execCommand(strSql.ToString());
                ///add by wwei0216w 2015/7/30 添加預購3欄位
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempDao.baseInfoSave-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 更新基本信息
        /// </summary>
        /// <param name="pTemp"></param>
        /// <returns></returns>
        public int baseInfoUpdate(ProductTemp pTemp)
        {
            try
            {
                pTemp.Replace4MySQL();
                StringBuilder stb = new StringBuilder("set sql_safe_updates=0;update product_temp set");
                stb.AppendFormat(" brand_id='{0}',product_name='{1}',product_sort='{2}',product_vendor_code='{3}',product_status={4},", pTemp.Brand_Id, pTemp.Product_Name, pTemp.Product_Sort, pTemp.Product_Vendor_Code, pTemp.Product_Status);
                stb.AppendFormat(" product_start='{0}',product_end='{1}',expect_time='{2}',product_freight_set='{3}',product_mode='{4}',tax_type='{5}' ", pTemp.Product_Start, pTemp.Product_End, pTemp.Expect_Time, pTemp.Product_Freight_Set, pTemp.Product_Mode, pTemp.Tax_Type);
                stb.AppendFormat(",expect_msg='{0}',bag_check_money={1} ", pTemp.expect_msg, pTemp.Bag_Check_Money);
                if (pTemp.Combination != 1)
                {
                    stb.AppendFormat(" ,combination = {0}", pTemp.Combination);
                }
                stb.AppendFormat(",show_in_deliver={0},prepaid={1},process_type={2},product_type={3},prod_name='{4}',prod_sz='{5}',", pTemp.Show_In_Deliver, pTemp.Prepaid, pTemp.Process_Type, pTemp.Product_Type, pTemp.Prod_Name, pTemp.Prod_Sz);
                stb.AppendFormat("deliver_days={0},min_purchase_amount={1},safe_stock_amount={2},extra_days={3},purchase_in_advance={4},purchase_in_advance_start = {5},purchase_in_advance_end={6} ", pTemp.Deliver_Days, pTemp.Min_Purchase_Amount, pTemp.Safe_Stock_Amount, pTemp.Extra_Days, pTemp.purchase_in_advance, pTemp.purchase_in_advance_start, pTemp.purchase_in_advance_end);  // add by zhuoqin0830w 新增5個修改欄位  2015/03/17
                stb.AppendFormat("  where writer_id = {0} and combo_type={1}", pTemp.Writer_Id, pTemp.Combo_Type);
                stb.AppendFormat(" and product_id='{0}';", pTemp.Product_Id);

                //如果存在更新或者刪除  如果是是否推薦點擊了否,則刪除
                if (pTemp.Combo_Type == 1 && pTemp.expend_day >= 0)//如果是單一商品
                {
                    if (reProductDao.ExsitInTemp(Convert.ToInt32(pTemp.Writer_Id), Convert.ToInt32(pTemp.Product_Id), pTemp.Combo_Type) > 0)
                    {
                        stb.AppendFormat("update recommended_product_attribute_temp set expend_day='{0}',months='{1}' where product_id='{2}'and write_id='{3}' and combo_type='{4}'; ", pTemp.expend_day, pTemp.months, pTemp.Product_Id, pTemp.Writer_Id, pTemp.Combo_Type);
                    }
                    else
                    {
                        stb.AppendFormat("insert into recommended_product_attribute_temp(`product_id`,`write_id`,`time_start`,`time_end`,`expend_day`,`months`,`combo_type`)values('{0}','{1}','{2}','{3}','{4}','{5}','{6}');", pTemp.Product_Id, pTemp.Writer_Id, 0, 0, pTemp.expend_day, pTemp.months, pTemp.Combo_Type);
                    }
                }
                else if (pTemp.Combo_Type == 1 && pTemp.expend_day == 0 && pTemp.months == "")//如果expend_day==0,並且months等於"",則說明選擇了否
                {
                    stb.AppendFormat("delete from recommended_product_attribute_temp where product_id='{0}'and write_id='{1}' and combo_type='{2}'; ", pTemp.Product_Id, pTemp.Writer_Id, pTemp.Combo_Type);
                }
                
                if (pTemp.Combo_Type == 2 && pTemp.expend_day >= 0)//如果是組合商品
                {
                    if (reProductDao.ExsitInTemp(Convert.ToInt32(pTemp.Writer_Id), Convert.ToInt32(pTemp.Product_Id), pTemp.Combo_Type) > 0)
                    {
                        stb.AppendFormat("update recommended_product_attribute_temp set expend_day='{0}',months='{1}' where product_id='{2}'and write_id='{3}' and combo_type='{4}'; ", pTemp.expend_day, pTemp.months, pTemp.Product_Id, pTemp.Writer_Id, pTemp.Combo_Type);
                    }
                    else
                    {
                        stb.AppendFormat("insert into recommended_product_attribute_temp(`product_id`,`write_id`,`time_start`,`time_end`,`expend_day`,`months`,`combo_type`)values('{0}','{1}','{2}','{3}','{4}','{5}','{6}');", pTemp.Product_Id, pTemp.Writer_Id, 0, 0, pTemp.expend_day, pTemp.months, pTemp.Combo_Type);
                    }
                }
                else if (pTemp.Combo_Type == 2 && pTemp.expend_day == 0 && pTemp.months == "")//如果expend_day==0,並且months等於"",則說明選擇了否
                {
                    stb.AppendFormat("delete from recommended_product_attribute_temp where product_id='{0}'and write_id='{1}' and combo_type='{2}'; ", pTemp.Product_Id, pTemp.Writer_Id,pTemp.Combo_Type);
                }

                stb.Append("set sql_safe_updates=1;");
                return _dbAccess.execCommand(stb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempDao.baseInfoUpdate-->" + ex.Message, ex);
            }
        }

        public int PriceBonusInfoSave(Model.ProductTemp proTemp)
        {
            try
            {
                StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0; update product_temp set ");
                strSql.AppendFormat(" product_price_list={0},bonus_percent={1},default_bonus_percent={2},", proTemp.Product_Price_List, proTemp.Bonus_Percent, proTemp.Default_Bonus_Percent);
                strSql.AppendFormat("bag_check_money={0},bonus_percent_start={1},price_type={2},", proTemp.Bag_Check_Money, proTemp.Bonus_Percent_Start, proTemp.Price_type);
                strSql.AppendFormat("bonus_percent_end={0},show_listprice={1} where writer_id={2} and combo_type={3}", proTemp.Bonus_Percent_End, proTemp.show_listprice, proTemp.Writer_Id, proTemp.Combo_Type);
                strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates=1;", proTemp.Product_Id);
                return _dbAccess.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempDao.PriceBonusInfoSave-->" + ex.Message, ex);
            }
        }

        public string DescriptionInfoSave(Model.ProductTemp proTemp)
        {
            proTemp.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0;update product_temp set ");
            strSql.AppendFormat("page_content_1='{0}',page_content_2='{1}',page_content_3='{2}'", proTemp.Page_Content_1, proTemp.Page_Content_2, proTemp.Page_Content_3);
            strSql.AppendFormat(",product_keywords='{0}',product_buy_limit={1} where writer_id={2} and combo_type={3}", proTemp.Product_Keywords, proTemp.Product_Buy_Limit, proTemp.Writer_Id, proTemp.Combo_Type);
            strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates=1;", proTemp.Product_Id);
            return strSql.ToString();
        }

        public int SpecInfoSave(Model.ProductTemp proTemp)
        {
            proTemp.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("set sql_safe_updates=0;update product_temp set ");
            strSql.AppendFormat(" product_spec={0},spec_title_1='{1}',spec_title_2='{2}'", proTemp.Product_Spec, proTemp.Spec_Title_1, proTemp.Spec_Title_2);
            strSql.AppendFormat(" where writer_id={0} and combo_type={1}", proTemp.Writer_Id, proTemp.Combo_Type);
            strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates=1;", proTemp.Product_Id);
            return _dbAccess.execCommand(strSql.ToString());
        }

        public int ProductTempUpdate(Model.ProductTemp proTemp, string page)
        {
            try
            {
                proTemp.Replace4MySQL();
                StringBuilder strSql = new StringBuilder("set sql_safe_updates=0;update product_temp set ");
                strSql.AppendFormat("writer_id={0} ", proTemp.Writer_Id);
                if (page == "stock")
                {
                    strSql.AppendFormat(" ,ignore_stock='{0}'", proTemp.Ignore_Stock);
                    strSql.AppendFormat(" ,shortage='{0}'", proTemp.Shortage);
                    strSql.AppendFormat(" ,stock_alarm='{0}'", proTemp.stock_alarm);
                }
                if (page == "pic")
                {
                    //if (proTemp.Product_Image != "")
                    //{
                    strSql.AppendFormat(" ,product_image='{0}'", proTemp.Product_Image);
                    //}
                    if (!string.IsNullOrEmpty(proTemp.product_media))
                    {
                        strSql.AppendFormat(" ,product_media='{0}'", proTemp.product_media);
                    }
                    //if(proTemp.Mobile_Image !="")//edit by wwei0216w 2015/3/18 更新手機說明圖的列
                    //{
                    strSql.AppendFormat(" ,mobile_image ='{0}'", proTemp.Mobile_Image);
                    //}
                    if (proTemp.Product_alt != "")// add by wwei0216w 2015/4/10 添加商品圖片說明
                    {
                        strSql.AppendFormat(" ,product_alt ='{0}'", proTemp.Product_alt);
                    }
                }
                strSql.AppendFormat(" where writer_id={0} and combo_type={1}", proTemp.Writer_Id, proTemp.Combo_Type);
                strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates=1;", proTemp.Product_Id);
                return _dbAccess.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempDao.ProductTempUpdate-->" + ex.Message, ex);
            }
        }

        public int CategoryInfoUpdate(Model.ProductTemp proTemp)
        {
            try
            {
                proTemp.Replace4MySQL();
                StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0;update product_temp set ");
                strSql.AppendFormat(" cate_id='{0}',prod_classify={1} ", proTemp.Cate_Id, proTemp.Prod_Classify);
                strSql.AppendFormat(" where writer_id={0} and combo_type={1} and product_id='{2}';set sql_safe_updates=1;", proTemp.Writer_Id, proTemp.Combo_Type, proTemp.Product_Id);
                return _dbAccess.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempDao.CategoryInfoUpdate-->" + ex.Message, ex);
            }
        }

        public int FortuneInfoSave(Model.ProductTemp proTemp)
        {
            try
            {
                StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0;update product_temp set ");
                strSql.AppendFormat(" fortune_quota={0},fortune_freight={1}", proTemp.Fortune_Quota, proTemp.Fortune_Freight);
                strSql.AppendFormat(" where writer_id={0} and combo_type={1}", proTemp.Writer_Id, proTemp.Combo_Type);
                strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates=1;", proTemp.Product_Id);
                return _dbAccess.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempDao.FortuneInfoSave-->" + ex.Message, ex);
            }
        }

        public string MoveProduct(Model.ProductTemp proTemp)
        {
            StringBuilder strSql = new StringBuilder("insert into product");
            strSql.Append("(product_id,brand_id,product_vendor_code,product_name,prod_name,prod_sz,product_price_list,product_spec,spec_title_1,spec_title_2,");
            strSql.Append("product_freight_set,product_buy_limit,product_status,product_hide,product_mode,product_sort,product_start,product_end,page_content_1,page_content_2,page_content_3,");
            strSql.Append("product_keywords,product_recommend,product_password,product_total_click,expect_time,product_image,product_createdate,");
            strSql.Append("product_updatedate,product_ipfrom,goods_area,goods_image1,goods_image2,city,bag_check_money,combination,bonus_percent,default_bonus_percent,bonus_percent_start,");
            strSql.Append("bonus_percent_end,tax_type,cate_id,fortune_quota,fortune_freight,product_media,ignore_stock,shortage,stock_alarm,price_type,user_id,show_listprice,expect_msg,");
            strSql.Append("create_channel,show_in_deliver,prepaid,process_type,product_type,prod_classify,safe_stock_amount, deliver_days,min_purchase_amount,extra_days,off_grade,mobile_image,product_alt,purchase_in_advance,purchase_in_advance_start,purchase_in_advance_end )");//add by wwei0216w 2015/4/9 臨時表導入正式表 中添加 product_alt

            strSql.Append(" select {0} as productId,brand_id,product_vendor_code,product_name,prod_name,prod_sz,product_price_list,product_spec,spec_title_1,spec_title_2,");
            strSql.Append("product_freight_set,product_buy_limit,product_status,product_hide,product_mode,product_sort,product_start,product_end,page_content_1,page_content_2,");
            strSql.AppendFormat("page_content_3,product_keywords,product_recommend,product_password,product_total_click,expect_time,product_image,{0} as product_createdate,", Common.CommonFunction.GetPHPTime());
            strSql.Append("product_updatedate,product_ipfrom,goods_area,goods_image1,goods_image2,city,bag_check_money,combination,bonus_percent,default_bonus_percent,bonus_percent_start,");
            strSql.Append("bonus_percent_end,tax_type,cate_id,fortune_quota,fortune_freight,product_media,ignore_stock,shortage,stock_alarm,price_type,writer_id,show_listprice,expect_msg,");
            //strSql.Append("create_channel,show_in_deliver,prepaid,process_type,product_type,prod_classify,safe_stock_amount, deliver_days,min_purchase_amount,extra_days,mobile_image,product_alt ");//add by wwei0216w 2015/4/9
            strSql.AppendFormat("create_channel,show_in_deliver,prepaid,process_type,product_type,prod_classify,safe_stock_amount, deliver_days,min_purchase_amount,extra_days,{0} AS off_grade,mobile_image,product_alt,purchase_in_advance,purchase_in_advance_start,purchase_in_advance_end ", proTemp.Combo_Type == 2 ? -1 : proTemp.off_grade);//edit by wwei0216w 2015/6/30 添加off_grade列的插入

            strSql.AppendFormat(" from product_temp where writer_id={0} and combo_type={1} and create_channel={2}", proTemp.Writer_Id, proTemp.Combo_Type, proTemp.Create_Channel);
            strSql.AppendFormat(" and product_id='{0}'", proTemp.Product_Id);
            return strSql.ToString();
        }

        public string Delete(Model.ProductTemp proTemp)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0;delete from product_temp where");
            strSql.AppendFormat(" writer_id={0} and combo_type={1}", proTemp.Writer_Id, proTemp.Combo_Type);
            strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates = 1;", proTemp.Product_Id);
            return strSql.ToString();
        }

        public string SaveFromPro(ProductTemp proTemp)
        {
            StringBuilder strSql = new StringBuilder("insert into product_temp");
            strSql.Append("(writer_id,product_id,brand_id,product_vendor_code,product_name,prod_name,prod_sz,product_price_list,product_spec,spec_title_1,spec_title_2,");
            strSql.Append("product_freight_set,product_buy_limit,product_status,product_hide,product_mode,product_sort,product_start,product_end,page_content_1,page_content_2,");
            strSql.Append("page_content_3,product_keywords,product_recommend,product_password,product_total_click,expect_time,product_image,product_createdate,product_updatedate,");
            strSql.Append("product_ipfrom,goods_area,goods_image1,goods_image2,city,bag_check_money,combination,bonus_percent,default_bonus_percent,bonus_percent_start,");
            strSql.Append("bonus_percent_end,tax_type,cate_id,fortune_quota,fortune_freight,product_media,combo_type,stock_alarm,price_type,show_listprice,expect_msg,product_type,process_type,sale_status,prepaid,show_in_deliver,create_channel,prod_classify,deliver_days,min_purchase_amount,safe_stock_amount,extra_days,mobile_image,product_alt,purchase_in_advance,purchase_in_advance_start,purchase_in_advance_end )");//edit 2014/09/24  //商品複製新增欄位  add by zhuoqin0830w  2015/03/17

            strSql.AppendFormat(" select {0} as writer_id,product_id,brand_id,product_vendor_code,product_name,prod_name,prod_sz,product_price_list,product_spec,spec_title_1,spec_title_2,", proTemp.Writer_Id);
            strSql.AppendFormat("product_freight_set,product_buy_limit,{0} as product_status,product_hide,product_mode,product_sort,product_start,product_end,page_content_1,page_content_2,", proTemp.Product_Status);
            strSql.Append("page_content_3,product_keywords,product_recommend,product_password,product_total_click,expect_time,product_image,product_createdate,product_updatedate,");
            strSql.Append("product_ipfrom,goods_area,goods_image1,goods_image2,city,bag_check_money,combination,bonus_percent,default_bonus_percent,bonus_percent_start,");
            strSql.AppendFormat("bonus_percent_end,tax_type,cate_id,fortune_quota,fortune_freight,product_media,{0} as combo_type,stock_alarm,price_type,show_listprice,expect_msg,product_type,process_type,sale_status,prepaid,show_in_deliver,{1} as create_channel,prod_classify,deliver_days,min_purchase_amount,safe_stock_amount,extra_days,mobile_image,product_alt,purchase_in_advance,purchase_in_advance_start,purchase_in_advance_end from product ", proTemp.Combo_Type, proTemp.Create_Channel);//商品複製新增欄位  add by zhuoqin0830w  2015/03/17
            strSql.AppendFormat("where product_id='{0}'", proTemp.Product_Id);
            return strSql.ToString();
        }

        public bool CopyProduct(ArrayList execSql, ArrayList specs, string selMaster, string moveMaster, string movePrice)
        {
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(strConn);
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                foreach (string str in execSql)
                {
                    mySqlCmd.CommandText = str;
                    mySqlCmd.ExecuteNonQuery();
                }

                SerialDao serialDao = new SerialDao("");
                foreach (string str in specs)
                {
                    mySqlCmd.CommandText = serialDao.Update(18);//規格編號
                    int specId = Convert.ToInt32(mySqlCmd.ExecuteScalar());

                    mySqlCmd.CommandText = string.Format(str, specId);
                    mySqlCmd.ExecuteNonQuery();
                }

                if (!string.IsNullOrEmpty(selMaster) && !string.IsNullOrEmpty(moveMaster) && !string.IsNullOrEmpty(movePrice))
                {
                    mySqlCmd.CommandText = selMaster;
                    MySqlDataReader reader = mySqlCmd.ExecuteReader(System.Data.CommandBehavior.SingleResult);
                    System.Data.DataTable _dt = new System.Data.DataTable();
                    _dt.Load(reader);
                    if (_dt == null)
                    {
                        mySqlCmd.Transaction.Rollback();
                        return false;
                    }
                    else
                    {
                        foreach (System.Data.DataRow item in _dt.Rows)
                        {
                            mySqlCmd.CommandText = string.Format(moveMaster, item["price_master_id"]);
                            int masterId = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                            mySqlCmd.CommandText = string.Format(movePrice, masterId, item["price_master_id"]);
                            mySqlCmd.ExecuteNonQuery();
                        }
                    }
                }

                mySqlCmd.Transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("ProductTempDao.CopyProduct-->" + ex.Message, ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
        }



        #region 供應商商品操作
        public bool VendorCopyProduct(ArrayList execSql, ArrayList specs, string selMaster, string moveMaster, string movePrice)
        {
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(strConn);
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                foreach (string str in execSql)
                {
                    mySqlCmd.CommandText = str;
                    mySqlCmd.ExecuteNonQuery();
                }
                SerialDao serialDao = new SerialDao("");
                foreach (string str in specs)
                {
                    mySqlCmd.CommandText = serialDao.Update(18);//規格編號
                    int specId = Convert.ToInt32(mySqlCmd.ExecuteScalar());

                    mySqlCmd.CommandText = string.Format(str, specId);
                    mySqlCmd.ExecuteNonQuery();
                }

                if (!string.IsNullOrEmpty(selMaster) && !string.IsNullOrEmpty(moveMaster) && !string.IsNullOrEmpty(movePrice))
                {
                    mySqlCmd.CommandText = selMaster;
                    MySqlDataReader reader = mySqlCmd.ExecuteReader(System.Data.CommandBehavior.SingleResult);
                    System.Data.DataTable _dt = new System.Data.DataTable();
                    _dt.Load(reader);
                    if (_dt == null)
                    {
                        mySqlCmd.Transaction.Rollback();
                        return false;
                    }
                    else
                    {
                        foreach (System.Data.DataRow item in _dt.Rows)
                        {
                            mySqlCmd.CommandText = string.Format(moveMaster, item["price_master_id"]);
                            int masterId = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                            mySqlCmd.CommandText = string.Format(movePrice, masterId, item["price_master_id"]);
                            mySqlCmd.ExecuteNonQuery();
                        }
                    }
                }
                mySqlCmd.Transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("ProductTempDao.VendorCopyProduct-->" + ex.Message, ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
        }

        #region 供應商商品待審核列表查詢 + List<Model.Custom.QueryandVerifyCustom> verifyWaitQuery(Model.Query.QueryVerifyCondition qcCon, out int totalCount)
        /// <summary>
        /// 供應商商品待審核列表查詢 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<BLL.gigade.Model.Custom.VenderProductListCustom> verifyWaitQuery(Model.Query.QueryVerifyCondition query, out int totalCount)
        {
            StringBuilder sqlStr = new StringBuilder("");
            try
            {
                query.Replace4MySQL();
                StringBuilder stbCols = new StringBuilder("select distinct a.product_image,a.product_id,combo_type,b.brand_name,a.product_name,a.writer_id as user_id,v.vendor_name_simple as user_name,a.combination as combination_id,c.parametername as combination,");
                stbCols.Append("a.product_price_list,p.parametername as prev_status ,d.parametername as product_status,e.parametername as product_freight_set,f.parametername as product_mode,a.tax_type,a.product_createdate,a.product_start,a.product_end");
                //StringBuilder stbCols = new StringBuilder("select distinct a.product_image,a.product_id,combo_type,b.brand_name,a.product_name,a.combination as combination_id,c.parametername as combination,");
                //stbCols.Append("a.product_price_list,p.parametername as prev_status ,d.parametername as product_status,e.parametername as product_freight_set,f.parametername as product_mode,a.tax_type,s.apply_time,s.online_mode,a.product_createdate,a.product_start,a.product_end");
                StringBuilder stbTabs = new StringBuilder(" from product_temp a");

                stbTabs.Append(" LEFT JOIN vendor v on v.vendor_id=a.writer_id ");
                stbTabs.Append(" left join vendor_brand b on a.brand_id=b.brand_id ");
                //stbTabs.Append(" left join product_status_apply s on a.product_id = s.product_id");
                stbTabs.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='combo_type') c on a.combination=c.parametercode");
                stbTabs.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='product_status') p on 20 = p.parametercode");
                stbTabs.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='product_status') d on a.product_status = d.parametercode");
                stbTabs.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='product_freight') e on a.product_freight_set = e.parametercode");
                stbTabs.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='product_mode') f on a.product_mode = f.parametercode");

                StringBuilder stbCon = new StringBuilder(" where 1=1");
                if (query.brand_id != 0)
                {
                    stbCon.AppendFormat(" and a.brand_id={0}", query.brand_id);
                }
                if (!string.IsNullOrEmpty(query.cate_id))
                {
                    stbCon.AppendFormat(" and a.cate_id = '{0}'", query.cate_id);
                }
                if (query.category_id != 0)
                {
                    stbTabs.Append(" left join product_category_set g on a.product_id = g.product_id");
                    stbCon.AppendFormat(" and g.category_id = {0}", query.category_id);
                }
                if (query.combination != 0)
                {
                    stbCon.AppendFormat(" and a.combination= {0}", query.combination);
                }
                //if (query.prev_status != -1)
                //{
                //    stbCon.AppendFormat(" and s.prev_status= {0}", query.prev_status);
                //}
                if (!string.IsNullOrEmpty(query.name_number))
                {
                    stbCon.AppendFormat(" and (a.product_name like '%{0}%' or a.product_id='{0}')", query.name_number);
                }
                if (query.product_status != -1)
                {
                    stbCon.AppendFormat(" and a.product_status={0} ", query.product_status);
                }
                if (!string.IsNullOrEmpty(query.date_type))
                {
                    switch (query.date_type)
                    {
                        case "product_createdate": CheckCondition(query, "a", stbCon); break; //建立日期
                        //case "apply_time": CheckCondition(query, "s", stbCon); break;    //申請日期
                        case "product_start": CheckCondition(query, "a", stbCon); break; //上架日期
                        case "product_end": CheckCondition(query, "a", stbCon); break;   //下架日期
                        default:
                            break;
                    }
                }
                //添加搜索條件搜索符合temp_tatus=12[供應商新建] and create_channel=2[供應商]的數據
                //stbCon.Append(" and a.temp_status=12 and a.create_channel=2 ");
                stbCon.Append(" and a.temp_status=12 ");
                totalCount = 0;
                System.Data.DataTable _dt = _dbAccess.getDataTable("select count(a.product_id) as totalCount" + stbTabs.ToString() + stbCon.ToString());
                if (_dt != null)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                }


                stbCon.Append(" order by product_id desc");

                if (query.IsPage)
                {
                    stbCon.AppendFormat("  limit {0},{1}", query.Start, query.Limit);
                }
                sqlStr.Append(stbCols.ToString());
                sqlStr.Append(stbTabs.ToString());
                sqlStr.Append(stbCon.ToString());
                return _dbAccess.getDataTableForObj<BLL.gigade.Model.Custom.VenderProductListCustom>(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempDao.verifyWaitQuery-->" + ex.Message + sqlStr.ToString(), ex);
            }
        }
        #endregion

        #region  判斷日期區間+void CheckCondition(Model.Query.QueryVerifyCondition qcCon, string table, StringBuilder stb)
        /// <summary>
        /// 判斷日期區間
        /// </summary>
        /// <param name="qcCon"></param>
        /// <param name="table"></param>
        /// <param name="stb"></param>
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

        #region 獲得臨時表中供應商新建的數據+ProductTemp GetProTempByVendor(ProductTemp proTemp)
        /// <summary>
        /// 獲得臨時表中供應商新建的數據
        /// </summary>
        /// <param name="proTemp"></param>
        /// <returns></returns>
        public List<ProductTemp> GetProTempByVendor(ProductTemp proTemp)
        {
            StringBuilder strSql = new StringBuilder("");
            try
            {
                strSql.Append(" select rid,writer_id,product_id,brand_id,product_vendor_code,product_name,prod_sz,product_price_list,product_spec,spec_title_1,spec_title_2,price_type, ");
                strSql.Append(" product_freight_set,product_buy_limit,product_status,product_hide,product_mode,product_sort,product_start,product_end,page_content_1,page_content_2, ");
                strSql.Append(" page_content_3,product_keywords,product_recommend,product_password,product_total_click,expect_time,product_image,product_createdate,product_updatedate, ");
                strSql.Append(" product_ipfrom,goods_area,goods_image1,goods_image2,city,bag_check_money,combination,bonus_percent,default_bonus_percent,bonus_percent_start, ");
                strSql.Append(" bonus_percent_end,tax_type,cate_id,fortune_quota,fortune_freight,product_media,ignore_stock,shortage,stock_alarm,show_listprice,combo_type,expect_msg,create_channel,temp_status from product_temp ");
                strSql.Append(" where 1=1  ");
                if (proTemp.Create_Channel != 0)
                {
                    strSql.AppendFormat(" and create_channel={0}", proTemp.Create_Channel);
                }
                if (!string.IsNullOrEmpty(proTemp.Product_Id) && proTemp.Product_Id != "0")
                {
                    strSql.AppendFormat(" and product_id='{0}'", proTemp.Product_Id);
                }
                else
                {
                    strSql.AppendFormat(" and temp_status={0}", proTemp.Temp_Status);
                }
                if (proTemp.Writer_Id != 0)
                {
                    strSql.AppendFormat(" and writer_id={0}", proTemp.Writer_Id);
                }
                if (proTemp.Combo_Type != 0)
                {
                    strSql.AppendFormat(" and combo_type={0}", proTemp.Combo_Type);
                }
                strSql.Append(";");
                return _dbAccess.getDataTableForObj<ProductTemp>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempDao.GetProTempByVendor-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        #region 修改數據+string Update(ProductTemp proTemp)
        /// <summary>
        /// 修改數據
        /// </summary>
        /// <param name="proTemp"></param>
        /// <returns></returns>
        public string Update(ProductTemp proTemp)
        {
            proTemp.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("");
            try
            {
                strSql.Append("set sql_safe_updates=0; ");
                strSql.Append(" update  product_temp set ");
                strSql.AppendFormat(" `writer_id`='{0}',`brand_id`='{1}',`product_vendor_code`='{2}',", proTemp.Writer_Id, proTemp.Brand_Id, proTemp.Product_Vendor_Code);
                strSql.AppendFormat(" `product_name`='{0}',`product_price_list`='{1}',`product_spec`='{2}',`spec_title_1`='{3}',`spec_title_2`='{4}',", proTemp.Product_Name, proTemp.Product_Price_List, proTemp.Product_Spec, proTemp.Spec_Title_1, proTemp.Spec_Title_2);
                strSql.AppendFormat(" `product_freight_set`='{0}',`product_buy_limit`='{1}',`product_status`='{2}',`product_hide`='{3}',", proTemp.Product_Freight_Set, proTemp.Product_Buy_Limit, proTemp.Product_Status, proTemp.Product_Hide == false ? 0 : 1);
                strSql.AppendFormat(" `product_mode`='{0}',`product_sort`='{1}',`product_start`='{2}',`product_end`='{3}',`page_content_1`='{4}',`page_content_2`='{5}',", proTemp.Product_Mode, proTemp.Product_Sort, proTemp.Product_Start, proTemp.Product_End, proTemp.Page_Content_1, proTemp.Page_Content_2);
                strSql.AppendFormat(" `page_content_3`='{0}',`product_keywords`='{1}',`product_recommend`='{2}',`product_password`='{3}',", proTemp.Page_Content_3, proTemp.Product_Keywords, proTemp.Product_Recommend, proTemp.Product_Password);
                strSql.AppendFormat(" `product_total_click`='{0}',`expect_time`='{1}',`product_image`='{2}',`product_createdate`='{3}',`product_updatedate`='{4}',", proTemp.Product_Total_Click, proTemp.Expect_Time, proTemp.Product_Image, proTemp.Product_Createdate, proTemp.Product_Updatedate);
                strSql.AppendFormat(" `product_ipfrom`='{0}',`goods_area`='{1}',`goods_image1`='{2}',`goods_image2`='{3}',`city`='{4}',", proTemp.Product_Ipfrom, proTemp.Goods_Area, proTemp.Goods_Image1, proTemp.Goods_Image2, proTemp.City);
                strSql.AppendFormat(" `bag_check_money`='{0}',`combination`='{1}',`bonus_percent`='{2}',`default_bonus_percent`='{3}',`bonus_percent_start`='{4}',`bonus_percent_end`='{5}',`tax_type`='{6}',`cate_id`='{7}',", proTemp.Bag_Check_Money, proTemp.Combination, proTemp.Bonus_Percent, proTemp.Default_Bonus_Percent, proTemp.Bonus_Percent_Start, proTemp.Bonus_Percent_End, proTemp.Tax_Type, proTemp.Cate_Id);
                strSql.AppendFormat(" `fortune_quota`='{0}',`fortune_freight`='{1}',`combo_type`='{2}',`product_media`='{3}',`ignore_stock`='{4}',`shortage`='{5}',`stock_alarm`='{6}',`price_type`='{7}',`show_listprice`='{8}',`expect_msg`='{9}' ", proTemp.Fortune_Quota, proTemp.Fortune_Freight, proTemp.Combo_Type, proTemp.product_media, proTemp.Ignore_Stock, proTemp.Shortage, proTemp.stock_alarm, proTemp.Price_type, proTemp.show_listprice, proTemp.expect_msg);
                strSql.AppendFormat(" where 1=1");
                if (proTemp.Create_Channel != 0)
                {
                    strSql.AppendFormat("  and create_channel='{0}'", proTemp.Create_Channel);
                }
                if (!string.IsNullOrEmpty(proTemp.Product_Id))
                {
                    strSql.AppendFormat(" and product_id='{0}'", proTemp.Product_Id);
                }
                strSql.Append(";");
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempDao.Update-->" + ex.Message + strSql.ToString(), ex);

            }
            return strSql.ToString();
        }
        #endregion

        #region 執行product_temp的整體修改
        public bool UpdateAchieve(Model.ProductTemp proTemp)
        {
            proTemp.Replace4MySQL();
            string sql = "";
            try
            {
                sql = Update(proTemp);
                int i = _dbAccess.execCommand(sql);
                if (i > 0)
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
                throw new Exception("ProductTempDao.UpdateAchieve-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 刪除供應商商品臨時表中的數據+string DeleteVendorTemp(Model.ProductTemp proTemp)
        /// <summary>
        /// 刪除供應商商品臨時表中的數據
        /// </summary>
        /// <param name="proTemp"></param>
        /// <returns></returns>
        public string DeleteVendorTemp(Model.ProductTemp proTemp)
        {
            StringBuilder strSql = new StringBuilder("");
            try
            {
                strSql.Append(" set sql_safe_updates = 0;delete from product_temp where 1=1 ");
                if (proTemp.Writer_Id != 0)
                {
                    strSql.AppendFormat(" and writer_id={0} ", proTemp.Writer_Id);
                }
                if (proTemp.Combo_Type != 0)
                {
                    strSql.AppendFormat("  and combo_type={0}", proTemp.Combo_Type);

                }
                if (!string.IsNullOrEmpty(proTemp.Product_Id))
                {
                    strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates = 1;", proTemp.Product_Id);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempDao.DeleteVendorTemp-->" + ex.Message + strSql.ToString(), ex);

            }
            return strSql.ToString();
        }
        #endregion

        #region 供應商商品基本信息新增+ int vendorBaseInfoSave(ProductTemp p)
        public string vendorBaseInfoSave(ProductTemp p)
        {
            StringBuilder strSql = new StringBuilder("");
            try
            {
                p.Replace4MySQL();
                strSql.Append("insert into product_temp( ");
                strSql.Append(" `writer_id`,`product_id`,`brand_id`,`product_vendor_code`,");
                strSql.Append(" `product_name`,`product_price_list`,`product_spec`,`spec_title_1`,`spec_title_2`,");
                strSql.Append(" `product_freight_set`,`product_buy_limit`,`product_status`,`product_hide`,");
                strSql.Append(" `product_mode`,`product_sort`,`product_start`,`product_end`,`page_content_1`,`page_content_2`,");
                strSql.Append(" `page_content_3`,`product_keywords`,`product_recommend`,`product_password`,");
                strSql.Append(" `product_total_click`,`expect_time`,`product_image`,`product_createdate`,`product_updatedate`,");
                strSql.Append(" `product_ipfrom`,`goods_area`,`goods_image1`,`goods_image2`,`city`,");
                strSql.Append(" `bag_check_money`,`combination`,`bonus_percent`,`default_bonus_percent`,`bonus_percent_start`,`bonus_percent_end`,`tax_type`,`cate_id`,");
                strSql.Append(" `fortune_quota`,`fortune_freight`,`combo_type`,`product_media`,`ignore_stock`,`shortage`,`stock_alarm`,`price_type`,`show_listprice`,`expect_msg`,`temp_status`,`create_channel`,`prod_classify`) values (");
                strSql.AppendFormat(" '{0}','{1}','{2}','{3}',", p.Writer_Id, p.Product_Id, p.Brand_Id, p.Product_Vendor_Code);
                strSql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}',", p.Product_Name, p.Product_Price_List, p.Product_Spec, p.Spec_Title_1, p.Spec_Title_2);
                strSql.AppendFormat(" '{0}','{1}','{2}','{3}',", p.Product_Freight_Set, p.Product_Buy_Limit, p.Product_Status, p.Product_Hide == false ? 0 : 1);
                strSql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}','{5}',", p.Product_Mode, p.Product_Sort, p.Product_Start, p.Product_End, p.Page_Content_1, p.Page_Content_2);
                strSql.AppendFormat(" '{0}','{1}','{2}','{3}',", p.Page_Content_3, p.Product_Keywords, p.Product_Recommend, p.Product_Password);
                strSql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}',", p.Product_Total_Click, p.Expect_Time, p.Product_Image, p.Product_Createdate, p.Product_Updatedate);
                strSql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}',", p.Product_Ipfrom, p.Goods_Area, p.Goods_Image1, p.Goods_Image2, p.City);
                strSql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}',", p.Bag_Check_Money, p.Combination, p.Bonus_Percent, p.Default_Bonus_Percent, p.Bonus_Percent_Start);
                strSql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}','{5}',", p.Bonus_Percent_End, p.Tax_Type, p.Cate_Id, p.Fortune_Quota, p.Fortune_Freight, p.Combo_Type);
                strSql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}',", p.product_media, p.Ignore_Stock, p.Shortage, p.stock_alarm, p.Price_type);
                strSql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}');select @@identity;", p.show_listprice, p.expect_msg, p.Temp_Status, p.Create_Channel, p.Prod_Classify);
                string rid = _dbAccess.getDataTable(strSql.ToString()).Rows[0][0].ToString();
                strSql.Clear();//product_id=“T”+rid
                string product_id = "T" + rid;
                strSql.AppendFormat(" update product_temp set product_id='{0}' where rid='{1}'", product_id, rid);

                if (_dbAccess.execCommand(strSql.ToString()) == 1)
                {
                    return product_id;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempDao.vendorBaseInfoSave-->" + ex.Message + strSql.ToString(), ex);
            }
        }

        #endregion

        #region 臨時表基本信息描述更新 +int vendorBaseInfoUpdate(ProductTemp pTemp)
        /// <summary>
        /// 複製商品時跟新商品數據+int vendorBaseInfoUpdate(ProductTemp pTemp)
        /// </summary>
        /// <param name="pTemp"></param>
        /// <returns></returns>
        public int vendorBaseInfoUpdate(ProductTemp pTemp)
        {
            StringBuilder stb = new StringBuilder("");
            try
            {
                pTemp.Replace4MySQL();
                stb.Append("set sql_safe_updates=0;update product_temp set");
                stb.AppendFormat(" brand_id='{0}',product_name='{1}',product_sort='{2}',product_vendor_code='{3}',product_status={4},", pTemp.Brand_Id, pTemp.Product_Name, pTemp.Product_Sort, pTemp.Product_Vendor_Code, pTemp.Product_Status);
                stb.AppendFormat(" product_start='{0}',product_end='{1}',expect_time='{2}',product_freight_set='{3}',product_mode='{4}',tax_type='{5}' ", pTemp.Product_Start, pTemp.Product_End, pTemp.Expect_Time, pTemp.Product_Freight_Set, pTemp.Product_Mode, pTemp.Tax_Type);
                stb.AppendFormat(",expect_msg='{0}',bag_check_money={1},temp_status='{2}' ", pTemp.expect_msg, pTemp.Bag_Check_Money, pTemp.Temp_Status);
                stb.AppendFormat(" ,product_updatedate={0}", pTemp.Product_Updatedate);
                if (pTemp.Combination != 1)
                {
                    stb.AppendFormat(" ,combination = {0}", pTemp.Combination);
                }
                stb.AppendFormat("  where writer_id = {0} and combo_type={1}", pTemp.Writer_Id, pTemp.Combo_Type);

                stb.AppendFormat(" and product_id='{0}' and create_channel={1};set sql_safe_updates=1;", pTemp.Product_Id, pTemp.Create_Channel);
                return _dbAccess.execCommand(stb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempDao.vendorBaseInfoUpdate-->" + ex.Message + stb.ToString(), ex);
            }
        }
        #endregion

        #region 修改product_temp價格 + int PriceBonusInfoSaveByVendor(Model.ProductTemp proTemp)
        public int PriceBonusInfoSaveByVendor(Model.ProductTemp proTemp)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("set sql_safe_updates = 0; update product_temp set ");
                strSql.AppendFormat(" product_price_list={0},bonus_percent={1},default_bonus_percent={2},", proTemp.Product_Price_List, proTemp.Bonus_Percent, proTemp.Default_Bonus_Percent);
                strSql.AppendFormat("bag_check_money={0},bonus_percent_start={1},price_type={2},", proTemp.Bag_Check_Money, proTemp.Bonus_Percent_Start, proTemp.Price_type);
                strSql.AppendFormat("bonus_percent_end={0},show_listprice={1} where writer_id={2} and combo_type={3}", proTemp.Bonus_Percent_End, proTemp.show_listprice, proTemp.Writer_Id, proTemp.Combo_Type);
                if (!string.IsNullOrEmpty(proTemp.Product_Id))
                {
                    strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates=1;", proTemp.Product_Id);
                }

                return _dbAccess.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempDao.PriceBonusInfoSaveByVendor-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        #region 更新臨時表信息  +int ProductTempUpdateByVendor(Model.ProductTemp proTemp, string page)
        public int ProductTempUpdateByVendor(Model.ProductTemp proTemp, string page)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                proTemp.Replace4MySQL();
                strSql.Append("set sql_safe_updates=0;update product_temp set ");
                strSql.AppendFormat("writer_id={0} ", proTemp.Writer_Id);

                if (page == "stock")
                {
                    strSql.AppendFormat(" ,ignore_stock='{0}'", proTemp.Ignore_Stock);
                    strSql.AppendFormat(" ,shortage='{0}'", proTemp.Shortage);
                    strSql.AppendFormat(" ,stock_alarm='{0}'", proTemp.stock_alarm);
                }
                if (page == "pic")
                {
                    if (proTemp.Product_Image != "")
                    {
                        strSql.AppendFormat(" ,product_image='{0}'", proTemp.Product_Image);
                    }
                    if (!string.IsNullOrEmpty(proTemp.product_media))
                    {
                        strSql.AppendFormat(" ,product_media='{0}'", proTemp.product_media);
                    }
                }
                strSql.AppendFormat(" where writer_id={0} and combo_type={1}", proTemp.Writer_Id, proTemp.Combo_Type);
                if (!string.IsNullOrEmpty(proTemp.Product_Id))
                {
                    strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates=1;", proTemp.Product_Id);
                }

                return _dbAccess.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempDao.ProductTempUpdateByVendor-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        #region 保存供應商描述 +string VendorDescriptionInfoSave(Model.ProductTemp proTemp)
        public string VendorDescriptionInfoSave(Model.ProductTemp proTemp)
        {
            proTemp.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0;update product_temp set ");
            strSql.AppendFormat("page_content_1='{0}',page_content_2='{1}',page_content_3='{2}'", proTemp.Page_Content_1, proTemp.Page_Content_2, proTemp.Page_Content_3);
            strSql.AppendFormat(",product_keywords='{0}',product_buy_limit={1} where 1=1 ", proTemp.Product_Keywords, proTemp.Product_Buy_Limit);
            if (proTemp.Writer_Id != 0)
            {
                strSql.AppendFormat(" and writer_id={0}", proTemp.Writer_Id);
            }
            if (proTemp.Combo_Type != 0)
            {
                strSql.AppendFormat("  and combo_type={0}", proTemp.Combo_Type);
            }
            if (!string.IsNullOrEmpty(proTemp.Product_Id))
            {
                strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates=1;", proTemp.Product_Id);
            }
            return strSql.ToString();
        }
        #endregion

        #region 單一規格保存
        public int vendorSpecInfoSave(Model.ProductTemp proTemp)
        {
            proTemp.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("");
            try
            {
                strSql.Append(" set sql_safe_updates=0;update product_temp set ");
                strSql.AppendFormat(" product_spec={0},spec_title_1='{1}',spec_title_2='{2}'", proTemp.Product_Spec, proTemp.Spec_Title_1, proTemp.Spec_Title_2);
                strSql.AppendFormat(" where writer_id={0} and combo_type={1}", proTemp.Writer_Id, proTemp.Combo_Type);
                strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates=1;", proTemp.Product_Id);
                return _dbAccess.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempDao.vendorSpecInfoSave-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        #region 根據model對象獲取整個model信息 +ProductTemp GetVendorProTemp(ProductTemp proTemp)
        public ProductTemp GetVendorProTemp(ProductTemp proTemp)
        {   //edit by jialei 20140912 用於商品管理供應商申請審核 20140916 add combo_type 
            try
            {
                StringBuilder strSql = new StringBuilder("select product_id,writer_id,brand_id,product_vendor_code,product_name,prod_sz,product_price_list,product_spec,spec_title_1,spec_title_2,");
                strSql.Append("product_freight_set,product_buy_limit,product_status,product_hide,product_mode,product_sort,product_start,product_end,page_content_1,page_content_2,");
                strSql.Append("page_content_3,product_keywords,product_recommend,product_password,product_total_click,expect_time,product_image,product_createdate,product_updatedate,");
                strSql.Append("product_ipfrom,goods_area,goods_image1,goods_image2,city,bag_check_money,combination,bonus_percent,default_bonus_percent,bonus_percent_start,");
                strSql.Append("bonus_percent_end,tax_type,cate_id,fortune_quota,fortune_freight,product_media,ignore_stock,shortage,stock_alarm,show_listprice,expect_msg,combo_type from product_temp ");
                strSql.Append(" where 1=1 ");
                //strSql.AppendFormat("where writer_id={0} and combo_type={1}", proTemp.Writer_Id, proTemp.Combo_Type);
                if (proTemp.Writer_Id != 0)
                {
                    strSql.AppendFormat(" and writer_id={0} ", proTemp.Writer_Id);
                }
                if (!string.IsNullOrEmpty(proTemp.Product_Id) && proTemp.Product_Id != "0")
                {
                    strSql.AppendFormat(" and product_id='{0}'", proTemp.Product_Id);
                }
                return _dbAccess.getSinggleObj<ProductTemp>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempDao.GetVendorProTemp-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 更新類別信息 +int CategoryInfoUpdate(Model.ProductTemp proTemp)
        public int CategoryInfoUpdateByVendor(Model.ProductTemp proTemp)
        {
            proTemp.Replace4MySQL();
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("set sql_safe_updates = 0;update product_temp set ");
                strSql.AppendFormat(" cate_id='{0}' where writer_id={1} and combo_type={2}", proTemp.Cate_Id, proTemp.Writer_Id, proTemp.Combo_Type);
                if (!string.IsNullOrEmpty(proTemp.Product_Id))
                {
                    strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates=1;", proTemp.Product_Id);
                }

                return _dbAccess.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempDao.CategoryInfoUpdateByVendor-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        #region 保存供應商新增商品到臨時表 更新庫存信息+ int SaveTempByVendor(ProductTemp proTemp)
        /// <summary>
        /// 保存供應商新增商品到臨時表
        /// </summary>
        /// <param name="proTemp"></param>
        /// <returns></returns>
        public int SaveTempByVendor(ProductTemp proTemp)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append(" set sql_safe_updates = 0; ");
                strSql.Append("  update product_temp set  ");
                strSql.AppendFormat("    ignore_stock='{0}',shortage='{1}',stock_alarm='{2}', temp_status=12  ", proTemp.Ignore_Stock, proTemp.Shortage, proTemp.stock_alarm);
                strSql.AppendFormat(" where product_id='{0}';", proTemp.Product_Id);
                strSql.Append(" set sql_safe_updates = 1;");

                return _dbAccess.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("ProductTempDao.SaveTempByVendor-->" + ex.Message + strSql.ToString(), ex);
            }

        }
        #endregion

        #region 儲存商品信息將temp_status更改為12+ bool SaveTempByVendor(ProductTemp proTemp)
        /// <summary>
        /// 儲存商品信息將temp_status更改為12
        /// </summary>
        /// <param name="writerId">操作者id</param>
        /// <param name="combo_type">商品類型</param>
        /// <param name="product_Id">商品id</param>
        /// <returns></returns>
        public bool SaveTemp(ProductTemp proTemp)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates=0;update product_temp set temp_status=12,product_status=20 where writer_id={0} and combo_type={1} and product_id='{2}';set sql_safe_updates=1;", proTemp.Writer_Id, proTemp.Combo_Type, proTemp.Product_Id);
                int result = _dbAccess.execCommand(sql.ToString());
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
                throw new Exception("ProductTempDao.[bool]SaveTemp-->" + ex.Message + sql.ToString(), ex);
            }

        }
        #endregion

        #region 複製臨時數據 + 修改新增的product_id
        public int VendorSaveFromPro(ProductTemp proTemp)
        {   //20140905 供應商複製
            StringBuilder strSql = new StringBuilder("insert into product_temp");
            int rid = 0;
            try
            {
                strSql.Append("(writer_id,brand_id,product_vendor_code,product_name,prod_sz,product_price_list,product_spec,spec_title_1,spec_title_2,");
                strSql.Append("product_freight_set,product_buy_limit,product_status,product_hide,product_mode,product_sort,product_start,product_end,page_content_1,page_content_2,");
                strSql.Append("page_content_3,product_keywords,product_recommend,product_password,product_total_click,expect_time,product_image,product_createdate,product_updatedate,");
                strSql.Append("product_ipfrom,goods_area,goods_image1,goods_image2,city,bag_check_money,combination,bonus_percent,default_bonus_percent,bonus_percent_start,");
                strSql.Append("bonus_percent_end,tax_type,cate_id,fortune_quota,fortune_freight,product_media,ignore_stock,shortage,combo_type,stock_alarm,price_type,show_listprice,expect_msg,temp_status,create_channel,prod_classify)");
                strSql.AppendFormat(" select {0} as writer_id,brand_id,product_vendor_code,product_name,prod_sz,product_price_list,product_spec,spec_title_1,spec_title_2,", proTemp.Writer_Id);
                strSql.AppendFormat("product_freight_set,product_buy_limit,{0} as product_status,product_hide,product_mode,product_sort,product_start,product_end,page_content_1,page_content_2,", proTemp.Product_Status);
                strSql.AppendFormat("page_content_3,product_keywords,product_recommend,product_password,product_total_click,expect_time,product_image,{0} as product_createdate,{1} as product_updatedate,", proTemp.Product_Createdate, proTemp.Product_Createdate);
                strSql.AppendFormat("'{0}' as product_ipfrom,goods_area,goods_image1,goods_image2,city,bag_check_money,combination,bonus_percent,default_bonus_percent,bonus_percent_start,", proTemp.Product_Ipfrom);
                strSql.AppendFormat("bonus_percent_end,tax_type,cate_id,fortune_quota,fortune_freight,product_media,ignore_stock,shortage,{0} as combo_type,stock_alarm,price_type,show_listprice,expect_msg,'13' as temp_status,'2'  as create_channel,prod_classify ", proTemp.Combo_Type);//13表複製中

                uint productid = 0;
                if (uint.TryParse(proTemp.Product_Id, out productid))
                {
                    strSql.AppendFormat(" from product  where product_id={0}; select @@identity;", productid);
                }
                else
                {
                    strSql.AppendFormat(" from product_temp  where product_id='{0}'; select @@identity;", proTemp.Product_Id);
                }
                rid = Int32.Parse(_dbAccess.getDataTable(strSql.ToString()).Rows[0][0].ToString());
                strSql.Clear();
                strSql.AppendFormat("Update product_temp set product_id='{0}' where rid='{1}' ", ("T" + rid).ToString(), rid);
                _dbAccess.execCommand(strSql.ToString());
                return rid;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempDao.VendorSaveFromPro-->" + ex.Message + "-->sql:" + strSql.ToString(), ex);
            }
        }
        #endregion

        #region 管理員核可供應商建立的商品時將商品信息由臨時表移動到正式表
        /// <summary>
        /// 管理員核可供應商建立的商品時將商品信息由臨時表移動到正式表
        /// </summary>
        /// <param name="proTemp">臨時表中的數據對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_MoveProduct(Model.ProductTemp proTemp)
        {
            StringBuilder strSql = new StringBuilder("insert into product");
            strSql.Append("(product_id,brand_id,product_vendor_code,product_name,prod_sz,product_price_list,product_spec,spec_title_1,spec_title_2,");
            strSql.Append("product_freight_set,product_buy_limit,product_status,product_hide,product_mode,product_sort,product_start,product_end,page_content_1,page_content_2,page_content_3,");
            strSql.Append("product_keywords,product_recommend,product_password,product_total_click,expect_time,product_image,product_createdate,");
            strSql.Append("product_updatedate,product_ipfrom,goods_area,goods_image1,goods_image2,city,bag_check_money,combination,bonus_percent,default_bonus_percent,bonus_percent_start,");
            strSql.Append("bonus_percent_end,tax_type,cate_id,fortune_quota,fortune_freight,product_media,ignore_stock,shortage,stock_alarm,price_type,user_id,show_listprice,expect_msg,create_channel,prod_classify )");
            strSql.Append(" select {0} as product_id,brand_id,product_vendor_code,product_name,prod_sz,product_price_list,product_spec,spec_title_1,spec_title_2,");
            strSql.Append("product_freight_set,product_buy_limit,'0' as product_status,product_hide,product_mode,product_sort,product_start,product_end,page_content_1,page_content_2,");
            strSql.AppendFormat("page_content_3,product_keywords,product_recommend,product_password,product_total_click,expect_time,product_image,{0} as product_createdate,", Common.CommonFunction.GetPHPTime());
            strSql.Append("product_updatedate,product_ipfrom,goods_area,goods_image1,goods_image2,city,bag_check_money,combination,bonus_percent,default_bonus_percent,bonus_percent_start,");
            strSql.Append("bonus_percent_end,tax_type,cate_id,fortune_quota,fortune_freight,product_media,ignore_stock,shortage,stock_alarm,price_type,writer_id,show_listprice,expect_msg,create_channel,prod_classify from product_temp where 1=1 ");
            if (proTemp.Writer_Id != 0)
            {
                strSql.AppendFormat(" and writer_id={0} ", proTemp.Writer_Id);
            }
            if (proTemp.Combo_Type != 0)
            {
                strSql.AppendFormat(" and combo_type={0} ", proTemp.Combo_Type);
            }
            strSql.AppendFormat("and create_channel={0} and product_id='{1}';", proTemp.Create_Channel, proTemp.Product_Id);
            return strSql.ToString();
        }

        /// <summary>
        /// 管理員核可供應商建立的商品時將商品信息由臨時表移除
        /// </summary>
        /// <param name="proTemp">臨時表中的數據對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_Delete(ProductTemp proTemp)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0;");
            strSql.AppendFormat("delete from product_temp where product_id='{0}';set sql_safe_updates = 1;", proTemp.Product_Id);
            return strSql.ToString();
        }

        /// <summary>
        /// 處理product_combo_temp 和 price_mater_temp表裡面的臨時id
        /// </summary>
        /// <param name="proTemp">商品臨時表對象</param>
        /// <returns>處理product_combo_temp表與price_master_temp表的sql語句</returns>
        public string VendorEditCM(ProductTemp proTemp)
        {//處理product_combo_temp 和 price_master_temp 表裡面的臨時id
            StringBuilder strSql = new StringBuilder();
            strSql.Append("set sql_safe_updates = 0;");
            strSql.Append("update product_combo_temp set child_id='{0}' where");
            strSql.AppendFormat(" child_id='{0}'; ", proTemp.Product_Id);
            strSql.Append("update price_master_temp  set product_id='{0}' where");
            strSql.AppendFormat(" product_id='{0}'; ", proTemp.Product_Id);
            strSql.Append("set sql_safe_updates = 1;");
            return strSql.ToString();
        }
        #endregion
        #endregion

        /// <summary>
        /// 商品取消送審
        /// </summary>
        /// <param name="productTemp"></param>
        /// <returns></returns>
        public bool CancelVerify(ProductTemp productTemp)
        {
            try
            {
                productTemp.Replace4MySQL();
                StringBuilder stb = new StringBuilder("set sql_safe_updates=0;update product_temp set product_status=20 where 1=1");
                stb.AppendFormat(" and product_id='{0}'", productTemp.Product_Id);
                _dbAccess.execCommand(stb.ToString());
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempDao.CancelVerify-->" + ex.Message, ex);
            }
        }

        public int GetDefaultArriveDays(ProductTemp temp)
        {
            try
            {
                int defaultArriveDays = 0;
                string strSql = string.Format(@"select p.product_mode,v.self_send_days,v.stuff_ware_days,v.dispatch_days from product_temp p
            inner join vendor_brand vb  on p.brand_id=vb.brand_id 
            inner join  vendor v on vb.vendor_id=v.vendor_id  
            where  p.combo_type=1 and p.writer_id={0} and p.product_id='{1}';", temp.Writer_Id, temp.Product_Id);

                System.Data.DataTable dt = _dbAccess.getDataTable(strSql);
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
                throw new Exception("ProductTempDao.GetDefaultArriveDays-->" + ex.Message, ex);
            }
        }
    }
}
