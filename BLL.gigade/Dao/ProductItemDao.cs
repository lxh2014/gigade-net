/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductItemDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/21 13:25:00 
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
using BLL.gigade.Model.Custom;
using BLL.gigade.Common;

namespace BLL.gigade.Dao
{
    public class ProductItemDao : IProductItemImplDao
    {
        private IDBAccess _access;
        private IDBAccess _dbAccess;
        private string connStr;
        public ProductItemDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            _access = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }

        public List<ProductItem> Query(ProductItem query)
        {
            try
            {
                query.Replace4MySQL();
                StringBuilder strSql = new StringBuilder("select i.item_id,barcode,i.product_id,spec_id_1,spec_id_2,s1.spec_name as Spec_Name_1,s2.spec_name as Spec_Name_2,");
                //strSql.Append("i.item_cost,i.item_money,i.event_product_start,i.event_product_end,i.event_item_cost,i.event_item_money,");
                strSql.Append("i.item_cost,i.item_money,i.event_product_start,i.event_product_end,i.event_item_cost,i.event_item_money,");
                //add by zhuoqin0830w 2015/02/05 增加備註  // add by zhuoqin0830w 2014/03/20 增加運達天數
                strSql.Append("item_stock,item_alarm,item_status,item_code,erp_id,remark,arrive_days from product_item i left join product_spec s1 on i.spec_id_1 = s1.spec_id left join product_spec s2 on i.spec_id_2 = s2.spec_id  where 1=1");//edit by xiangwang0413w 2014/06/18 (增加ERP廠商編號erp_id)
                if (query.Item_Id != 0)
                {
                    strSql.AppendFormat(" and i.item_id={0}", query.Item_Id);
                }
                if (query.Product_Id != 0)
                {
                    strSql.AppendFormat(" and i.product_id={0}", query.Product_Id);
                }
                if (query.Spec_Id_1 != 0)
                {
                    strSql.Append(" and spec_id_1=" + query.Spec_Id_1 + "");
                }
                if (query.Spec_Id_2 != 0)
                {
                    strSql.Append(" and spec_id_2=" + query.Spec_Id_2 + "");
                }
                return _dbAccess.getDataTableForObj<ProductItem>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemDao.Query-->" + ex.Message, ex);
            }
        }

        //查詢吉甲地價格細項採用item_price裡面的價格 add by xiangwang0413w 2014/08/12
        public List<ProductItem> QueryPrice(ProductItem query)
        {
            try
            {
                query.Replace4MySQL();
                StringBuilder strSql = new StringBuilder("select  i.item_id,barcode,i.product_id,spec_id_1,spec_id_2,s1.spec_name as Spec_Name_1,s2.spec_name as Spec_Name_2,");
                //細項價格採用item_price裡面的價格
                strSql.Append(" ip.item_money,ip.item_cost,ip.event_money as event_item_money,ip.event_cost as event_item_cost,i.event_product_start,i.event_product_end,");
                strSql.Append("item_stock,item_alarm,item_status,item_code,erp_id from product_item i left join product_spec s1 on i.spec_id_1 = s1.spec_id left join product_spec s2 on i.spec_id_2 = s2.spec_id left join item_price ip on i.item_id=ip.item_id ");
                strSql.Append(" inner join price_master pm on ip.price_master_id=pm.price_master_id and pm.site_id=1 and pm.user_level=1 and user_id=0 where 1=1");
                if (query.Product_Id != 0)
                {
                    strSql.AppendFormat(" and i.product_id={0}", query.Product_Id);
                }
                //添加 根據Item_id 查詢數據 的條件  add by zhuoqin0830w  2015/07/10
                if (query.Item_Id != 0)
                {
                    strSql.AppendFormat(" and i.item_id={0}", query.Item_Id);
                }
                return _dbAccess.getDataTableForObj<ProductItem>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemDao.Query-->" + ex.Message, ex);
            }
        }

        public List<ProductItem> Query(Model.Custom.PriceMasterCustom query)
        {
            string strSql = string.Format(@"select a.product_id ,a.item_id,s1.spec_name as spec_name_1 ,s2.spec_name as spec_name_2 from product_item a
            left join product_spec as s1 on a.spec_id_1=s1.spec_id left join product_spec as s2 on a.spec_id_2=s2.spec_id
            where a.product_id={0}", query.product_id);
            return _dbAccess.getDataTableForObj<ProductItem>(strSql);
        }

        public string UpdateStock(BLL.gigade.Model.ProductItem item)
        {
            StringBuilder strSql = new StringBuilder("update product_item set ");
            strSql.AppendFormat("item_stock=item_stock-{0} where item_id={1}", item.Item_Stock, item.Item_Id);
            return strSql.ToString();
        }
        public string UpdateItemStock(uint Item_Id, uint Item_Stock)
        {
            StringBuilder strSql = new StringBuilder("update product_item set ");
            strSql.AppendFormat("item_stock=item_stock+{0} where item_id={1};", Item_Stock, Item_Id);
            return strSql.ToString();
        }
        //更新product_item表export_flag字段 edit by xiangwang0413w 2014/06/30
        public void UpdateExportFlag(BLL.gigade.Model.ProductItem item)
        {
            StringBuilder strSql = new StringBuilder("update product_item set ");
            strSql.AppendFormat("export_flag={0} where product_id={1}", item.Export_flag, item.Product_Id);
            _dbAccess.execCommand(strSql.ToString());
        }

        //單一商品修改時將修改後的數據更新至正式表product_item
        public string Update(Model.ProductItem item)
        {
            item.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("update product_item set ");
            //strSql.AppendFormat(" product_id={0},spec_id_1={1},spec_id_2={2},item_cost={3}", item.Product_Id, item.Spec_Id_1, item.Spec_Id_2, item.Item_Cost);
            //strSql.AppendFormat(",item_money={0},event_product_start={1},event_product_end={2}", item.Item_Money, item.Event_Product_Start, item.Event_Product_End); ;
            //strSql.AppendFormat(",event_item_cost={0},event_item_money={1},item_stock={2},item_alarm={3}", item.Event_Item_Cost, item.Event_Item_Money, item.Item_Stock, item.Item_Alarm);
            //strSql.AppendFormat(",item_status={0},item_code='{1}',barcode='{2}' where item_id={3}", item.Item_Status, item.Item_Code, item.Barcode, item.Item_Id);

            //edit by xiangwang0413w 2014/10/17 不再更新product_item裡的價格  
            strSql.AppendFormat(" product_id={0},spec_id_1={1},spec_id_2={2}", item.Product_Id, item.Spec_Id_1, item.Spec_Id_2);
            strSql.AppendFormat(",event_product_start={0},event_product_end={1}", item.Event_Product_Start, item.Event_Product_End); ;
            strSql.AppendFormat(",item_stock={0},item_alarm={1}", item.Item_Stock, item.Item_Alarm);
            strSql.AppendFormat(",item_status={0},item_code='{1}',barcode='{2}',remark='{3}',arrive_days={4} where item_id={5}", item.Item_Status, item.Item_Code, item.Barcode, item.Remark, item.Arrive_Days, item.Item_Id);//add by zhuoqin0830w 2014/03/20 增加運達天數
            return strSql.ToString();
        }

        public string SaveSql(ProductItem item)
        {
            item.Replace4MySQL();
            StringBuilder stb = new StringBuilder("insert into product_item(`item_id`,`spec_id_1`,`spec_id_2`,`item_cost`,`item_money`,");
            stb.Append("`event_product_start`,`event_product_end`,`event_item_cost`,`event_item_money`,`item_stock`,`item_alarm`,`item_status`,`item_code`,`barcode`,`product_id`)values({0},");
            stb.AppendFormat("{0},{1},{2}", item.Spec_Id_1, item.Spec_Id_2, item.Item_Cost);
            stb.AppendFormat(",{0},{1},{2},{3}", item.Item_Money, item.Event_Product_Start, item.Event_Product_End, item.Event_Item_Cost);
            stb.AppendFormat(",{0},{1},{2},{3}", item.Event_Item_Money, item.Item_Stock, item.Item_Alarm, item.Item_Status);
            stb.AppendFormat(",'{0}','{1}'", item.Item_Code, item.Barcode);
            stb.Append(",{1});select @@identity;");
            return stb.ToString();
        }

        public int Save(ProductItem saveModel)
        {
            try
            {
                //StringBuilder stb = new StringBuilder("insert into product_item (`product_id`,`spec_id_1`,`spec_id_2`,`item_cost`,`item_money`,`event_product_start`,`event_product_end`,`event_item_cost`,`event_item_money`,`item_stock`,`item_alarm`,`item_status`,`item_code`,`barcode`,`item_id`)");
                //stb.AppendFormat(" values ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},'{12}','{13}',{14})", saveModel.Product_Id, saveModel.Spec_Id_1, saveModel.Spec_Id_2, saveModel.Item_Cost, saveModel.Item_Money, saveModel.Event_Product_Start, saveModel.Event_Product_End, saveModel.Event_Item_Cost, saveModel.Event_Item_Money, saveModel.Item_Stock, saveModel.Item_Alarm, saveModel.Item_Status, saveModel.Item_Code, saveModel.Barcode, saveModel.Item_Id);
                //edit by xiangwang0413w 2014/10/17 不再更新product_item裡的價格
                StringBuilder stb = new StringBuilder("insert into product_item (`product_id`,`spec_id_1`,`spec_id_2`,`event_product_start`,`event_product_end`,`item_stock`,`item_alarm`,`item_status`,`item_code`,`barcode`,`item_id`)");
                stb.AppendFormat(" values ({0},{1},{2},{3},{4},{5},{6},{7},'{8}','{9}',{10})", saveModel.Product_Id, saveModel.Spec_Id_1, saveModel.Spec_Id_2, saveModel.Event_Product_Start, saveModel.Event_Product_End, saveModel.Item_Stock, saveModel.Item_Alarm, saveModel.Item_Status, saveModel.Item_Code, saveModel.Barcode, saveModel.Item_Id);

                return _dbAccess.execCommand(stb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemDao.Save-->" + ex.Message, ex);
            }
        }

        public void UpdateErpId(string pro_id)
        {
            string erp_id = string.Empty;
            string cate_id = string.Empty;
            string topvalue = string.Empty;
            string tax_type = string.Empty;
            string item_id = string.Empty;
            string spec_type = string.Empty;
            //int spec_id = 0;

            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("select cate_id,tax_type,product_spec from product where product_id = '{0}' ;", pro_id);
            stb.AppendFormat("select item_id,spec_id_1,spec_id_2,erp_id from product_item where product_id = '{0}'", pro_id);
            DataSet ds = _dbAccess.getDataSet(stb.ToString());
            cate_id = ds.Tables[0].Rows[0]["cate_id"].ToString();
            tax_type = ds.Tables[0].Rows[0]["tax_type"].ToString() == "1" ? "1" : "0";
            spec_type = ds.Tables[0].Rows[0]["product_spec"].ToString();
            stb = new StringBuilder();
            foreach (DataRow dr in ds.Tables[1].Rows)
            {
                item_id = dr["item_id"].ToString();
                if (dr["erp_id"].ToString() != "")
                    continue;
                //Author:castle
                //Date:2014/06/30:
                //descript:将规格拿掉
                //當無規格時，插入000，否則依Item自動增長
                //if (spec_type != "0")
                //{
                //    spec_id++;
                //    erp_id = "3" + cate_id + item_id + spec_id.ToString().PadLeft(3, '0') + tax_type;
                //} else
                //{
                //    erp_id = "3" + cate_id + item_id + "000" + tax_type;
                //}
                erp_id = "3" + cate_id + item_id + tax_type;
                stb.AppendFormat("update product_item set erp_id = '{0}' where item_id = '{1}';", erp_id, item_id);
            }

            if (stb.ToString().Length != 0)
                _dbAccess.execCommand(stb.ToString());
        }

        public string Delete(ProductItem delModel)
        {
            return string.Format("delete from product_item where product_id = {0}", delModel.Product_Id);
        }

        public List<Model.Custom.StockDataCustom> QueryItemStock(int product_id, int pile_id)
        {
            try
            {
                //增加一個shortage  edit by hufeng0813w 2014/05/22
                StringBuilder stb = new StringBuilder("select a.product_id,a.item_id,b.product_name,b.prod_sz,c.spec_name as spec_name1,d.spec_name as spec_name2,a.item_stock,b.ignore_stock,b.shortage as shortstatus from product_item a ");
                stb.Append(" left join product b on a.product_id=b.product_id");
                stb.Append(" left join product_spec c on a.spec_id_1=c.spec_id");
                stb.Append(" left join product_spec d on a.spec_id_2=d.spec_id");
                stb.AppendFormat(" where a.product_id in (select child_id from product_combo where parent_id={0} and pile_id={1})", product_id, pile_id);
                return _dbAccess.getDataTableForObj<Model.Custom.StockDataCustom>(stb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemDao.QueryItemStock-->" + ex.Message, ex);
            }
        }

        public string UpdateCopySpecId(ProductItem proItem)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0;update product_item set spec_id_1={0} where product_id = {1}");
            strSql.AppendFormat("  and spec_id_1={0};", proItem.Spec_Id_1);
            strSql.Append(" update product_item set spec_id_2={0} where product_id = {1}");
            strSql.AppendFormat(" and spec_id_2={0}", proItem.Spec_Id_2);
            strSql.Append(";set sql_safe_updates = 1; ");
            return strSql.ToString();
        }

        /// <summary>
        /// 獲得新增加的商品ID
        /// </summary>
        /// <returns>符合條件的新增加商品ID</returns>
        public List<ProductItem> GetProductNewItem_ID(int product_id)
        {
            try
            {
                StringBuilder sb = new StringBuilder(@"select
                 item_id  
                 from 
                 product_item 
                 where item_id 
                not in (select item_id from item_price)");
                sb.AppendFormat(" and product_id={0}", product_id);
                return _dbAccess.getDataTableForObj<ProductItem>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemDao.GetProductNewItem_ID-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 根據Id獲取productitem的信息
        /// </summary>
        /// <param name="productId">商品Id</param>
        /// <returns>符合條件的集合</returns>
        /// add by wangwei0216w 2014/9/22
        public List<ProductItem> GetProductItemByID(int productId)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(@"SELECT pi.item_id,pi.product_id,pi.spec_id_1,pi.spec_id_2,pi.spec_id_2,pi.item_cost,pi.item_money,
	    pi.event_product_start,pi.event_product_end,pi.event_item_cost,pi.event_item_money
                                  FROM product_item pi 
                                  WHERE pi.product_id={0}",productId);
                return _dbAccess.getDataTableForObj<ProductItem>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemDao.GetProductItemByID-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查詢商品庫存
        /// </summary>
        /// <param name="product_id">商品編號</param>
        /// <param name="pile_id"></param>
        /// <returns></returns>
        public List<Model.Custom.StockDataCustom> VendorQueryItemStock(string product_id, int pile_id)
        {
            StringBuilder stb = new StringBuilder();
            StringBuilder strTempSql = new StringBuilder();
            StringBuilder strSql = new StringBuilder();
            string str = string.Empty;
            try
            {
                //增加一個shortage 
                stb = stb.Append("select a.product_id as vendor_product_id ,a.item_id as item_id,b.product_name as product_name,b.prod_sz,c.spec_name as spec_name1,d.spec_name as spec_name2,a.item_stock as item_stock,b.ignore_stock as ignore_stock,b.shortage as shortstatus from product_item a ");
                stb.Append(" left join product b on a.product_id=b.product_id");
                stb.Append(" left join product_spec c on a.spec_id_1=c.spec_id");
                stb.Append(" left join product_spec d on a.spec_id_2=d.spec_id");
                stb.AppendFormat(" where a.product_id in (select child_id from product_combo where parent_id='{0}' and pile_id={1})", product_id, pile_id);
                // stb.Append(" GROUP BY a.product_id");
                //查臨時表中臨時子商品數據
                strTempSql = strTempSql.Append("select a.product_id as vendor_product_id ,a.item_id as item_id,bt.product_name as product_name,ct.spec_name as spec_name1, dt.spec_name as spec_name2,a.item_stock  as item_stock,bt.ignore_stock as ignore_stock,bt.shortage as shortstatus from product_item_temp a");
                strTempSql.Append("   left join product_temp bt on a.product_id=bt.product_id ");
                strTempSql.Append(" left join product_spec_temp ct on a.spec_id_1=ct.spec_id ");
                strTempSql.Append(" left join product_spec_temp dt on a.spec_id_2=dt.spec_id");
                strTempSql.AppendFormat(" where a.product_id in (select child_id from product_combo_temp where parent_id='{0}' and pile_id={1})", product_id, pile_id);
                // strTempSql.Append(" GROUP BY a.product_id");

                //查臨時表中正式子商品數據
                strSql = strSql.Append("select a.product_id as vendor_product_id ,a.item_id as item_id,b.product_name as product_name,b.prod_sz,c.spec_name as spec_name1,d.spec_name as spec_name2,a.item_stock as item_stock,b.ignore_stock as ignore_stock,b.shortage as shortstatus from product_item a ");
                strSql.Append(" left join product b on a.product_id=b.product_id");
                strSql.Append(" left join product_spec c on a.spec_id_1=c.spec_id");
                strSql.Append(" left join product_spec d on a.spec_id_2=d.spec_id");
                strSql.AppendFormat(" where a.product_id in (select child_id from product_combo_temp where parent_id='{0}' and pile_id={1})", product_id, pile_id);
                // strSql.Append(" GROUP BY a.product_id");
                str = " select * from ((" + stb.ToString() + ") union all(" + strTempSql.ToString() + ")" + " union all(" + strSql.ToString() + ")) n ";
                str += " where n.vendor_product_id is not NULL ";
                return _dbAccess.getDataTableForObj<Model.Custom.StockDataCustom>(str.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemDao.VendorQueryItemStock-->" + str.ToString() + ex.Message, ex);
            }
        }

        /// <summary>
        /// 獲取商品建議採購量信息
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <returns>商品建議採購列表</returns>
        ///
        public DataTable GetSuggestPurchaseInfo(ProductItemQuery query, out int TotalCount)
        {
            DataTable _dt = new DataTable();
            StringBuilder sbSqlColumn = new StringBuilder();
            StringBuilder sbSqlTable = new StringBuilder();
            StringBuilder sbSqlCondition = new StringBuilder();
            StringBuilder sb = new StringBuilder();

            TotalCount = 0;
            string sumdate = DateTime.Now.AddDays(-query.sumDays).ToString("yyyy-MM-dd 00:00:00"); ;
            /*清理掉記錄表中已經補充貨物的記錄*/
            sb.Append("set sql_safe_updates = 0; delete from item_ipo_create_log where item_id in(select pi.item_id from (SELECT od.item_id,sum(od.buy_num*od.parent_num) as sum_total from order_master om LEFT JOIN order_slave os USING(order_id)LEFT JOIN order_detail od USING(slave_id)  ");
            sb.AppendFormat(" where FROM_UNIXTIME( om.order_createdate)>='{0}'  GROUP BY od.item_id) sum_biao ", sumdate);
            sb.Append(" left join  product_item pi on sum_biao.item_id=pi.item_id left join product p on p.product_id=pi.product_id  LEFT JOIN vendor_brand vb on vb.brand_id=p.brand_id LEFT JOIN vendor v on v.vendor_id=vb.vendor_id ");
            sb.AppendFormat(" where pi.item_stock-(v.procurement_days* sum_biao.sum_total/'{0}'*'{1}')>pi.item_alarm); set sql_safe_updates = 1; ", query.sumDays, query.periodDays);
            _dbAccess.execCommand(sb.ToString());

            sbSqlColumn.Append("select v.vendor_id,p.product_id,p.product_name,(select max(create_time) from item_ipo_create_log where item_id=pi.item_id ) as create_datetime, p.sale_status ,'' as sale_name,p.product_mode ,'' as product_mode_name, p.prepaid,pi.erp_id,pi.item_id,pi.item_stock, pi.item_alarm,p.safe_stock_amount,  ");
            sbSqlColumn.Append("pm.price as item_money,pm.cost as item_cost,sum_biao.sum_total,  p.min_purchase_amount,v.vendor_name_simple, v.procurement_days,p.product_status, ");
            sbSqlColumn.Append("  pi.product_id,pi.spec_id_1 , '' as spec_title_1,pi.spec_id_2,'' as spec_title_2 ,''as NoticeGoods ");
            sbSqlTable.Append(" from (SELECT  od.item_id,sum( case item_mode when 0 then od.buy_num when  2 then od.buy_num*od.parent_num end ) as sum_total from order_master om INNER JOIN order_slave os USING(order_id)INNER JOIN order_detail od USING(slave_id)  ");
            sbSqlTable.AppendFormat(" where FROM_UNIXTIME( om.order_createdate)>='{0}' and od.item_mode in (0,2) GROUP BY od.item_id) sum_biao ", sumdate);
            sbSqlTable.Append(" INNER join  product_item pi on sum_biao.item_id=pi.item_id");
            sbSqlTable.Append(" INNER join product p on p.product_id=pi.product_id ");
            sbSqlTable.Append(" INNER JOIN vendor_brand vb on vb.brand_id=p.brand_id");
            sbSqlTable.Append(" INNER JOIN vendor v on v.vendor_id=vb.vendor_id ");
            sbSqlTable.Append(" LEFT JOIN item_ipo_create_log iicl on iicl.item_id=pi.item_id ");
            sbSqlTable.Append(" INNER join price_master pm on pm.product_id=p.product_id and pm.site_id=1 ");
            sbSqlCondition.Append(" WHERE  p.product_status =5  ");  //現在只要上架的
            sbSqlCondition.Append(" and ((p.prepaid=1) or (p.prepaid=0 and p.product_mode=2)) ");  //如果商品為買斷商品（product.prepaid=1）,則全部顯示.如果商品為非買斷商品（product.prepaid=0）,則只要寄倉的
            if (query.prepaid != -1)
            {
                sbSqlCondition.AppendFormat(" and p.prepaid='{0}' ", query.prepaid);
            }
            if (!string.IsNullOrEmpty(query.category_ID_IN))
            {
                sbSqlCondition.AppendFormat(" and p.product_id NOT in(select product_id from  product_category_set where category_id in({0}))", query.category_ID_IN);
            }
            if (!string.IsNullOrEmpty(query.vendor_name))
            {
                sbSqlCondition.AppendFormat(" and (v.vendor_name_full like '%{0}%' or v.vendor_name_simple like '%{0}%') ", query.vendor_name);
            }
            switch (query.Is_pod)
            {
                case 0:
                    //不管下單採購還是未下單採購
                    break;
                case 1://下單採購的，時間不為空
                    sbSqlCondition.Append(" and iicl.create_time IS NOT NULL ");
                    break;
                case 2:
                    //未下單採購的時間為空
                    sbSqlCondition.Append(" and iicl.create_time IS NULL ");
                    break;
                default:
                    break;
            }

            switch (query.stockScope)
            {
                case 0://所有庫存，不加條件的
                    //當(庫存數量-安全存量)<(供應商的進貨天數*近3個月的平均每周銷售數量(最小值為1))時,就需要採購
                    break;
                case 1:
                    sbSqlCondition.Append(" and pi.item_stock<=0 ");//庫存數量在0或者0以下
                    break;
                //case 2:
                //    sbSqlCondition.Append(" and (pi.item_stock<=5 and pi.item_stock>0) ");//庫存數量在0到5之間
                //    break;
                //case 3:
                //    sbSqlCondition.Append(" and (pi.item_stock<=10 and pi.item_stock>5) ");//庫存數量在5到10之間
                //    break;
                case 2:
                    //sbSqlCondition.Append(" and (pi.item_stock<pi.item_alarm) ");//庫存數量小於安全存量
                    //當前庫存量-供應商的採購天數*平均銷售數量(最小值為1))<=安全存量時,就需要採購
                    sbSqlCondition.AppendFormat(" and  pi.item_stock-(v.procurement_days* sum_biao.sum_total/'{0}'*'{1}')<=pi.item_alarm ", query.sumDays, query.periodDays);
                    break;
                default:
                    break;
            }
            try
            {
                sbSqlCondition.Append(" order by sum_biao.item_id desc ");
                if (query.IsPage)
                {
                    DataTable dt = _dbAccess.getDataTable("select count(pi.item_id) as totalCount " + sbSqlTable.ToString() + sbSqlCondition.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        TotalCount = Convert.ToInt32(dt.Rows[0]["totalCount"]);
                    }

                    sbSqlCondition.AppendFormat(" limit {0},{1} ", query.Start, query.Limit);
                }
                Dictionary<int, int>NoticeGoods= GetNoticeGoods(query);
                IParametersrcImplDao _parameterDao = new ParametersrcDao(connStr);
                IProductSpecImplDao _specDao = new ProductSpecDao(connStr);
                List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("product_mode", "sale_status");
                DataTable dtResult = _dbAccess.getDataTable(sbSqlColumn.ToString() + sbSqlTable.ToString() + sbSqlCondition.ToString());
                foreach (DataRow dr in dtResult.Rows)
                {
                    dr["NoticeGoods"] = 0;
                    if (NoticeGoods.Keys.Contains(Convert.ToInt32(dr["item_id"])))
                    {
                        dr["NoticeGoods"] = NoticeGoods[Convert.ToInt32(dr["item_id"])];
                    }
                    var alist = parameterList.Find(m => m.ParameterType == "product_mode" && m.ParameterCode == dr["product_mode"].ToString());
                    var dlist = parameterList.Find(m => m.ParameterType == "sale_status" && m.ParameterCode == dr["sale_status"].ToString());

                    if (alist != null)
                    {
                        dr["product_mode_name"] = alist.parameterName;
                    }
                    if (dlist != null)
                    {
                        dr["sale_name"] = dlist.parameterName;
                    }
                    ProductSpec spec1 = _specDao.query(Convert.ToInt32(dr["spec_id_1"].ToString()));
                    ProductSpec spec2 = _specDao.query(Convert.ToInt32(dr["spec_id_2"].ToString()));
                    if (spec1 != null)
                    {
                        dr["spec_title_1"] = spec1.spec_name;
                    }
                    if (spec2 != null)
                    {
                        dr["spec_title_2"] = spec2.spec_name;
                    }
                }
                return dtResult;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemDao-->GetSuggestPurchaseInfo-->" + ex.Message + sbSqlColumn.Append(sbSqlTable).Append(sbSqlCondition).ToString(), ex);
            }
        }

        ///查詢和運達天數相關的信息
        public ProductItemCustom GetProductArriveDay(ProductItem pi, string type)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@"SELECT pi.arrive_days,p.deliver_days,item_stock,pi.product_id
                                FROM product_item pi
                                    INNER JOIN product p ON p.product_id =  pi.product_id 
                            WHERE 1 = 1 ");
                if (type == "item")
                {
                    sb.AppendFormat("AND pi.item_id = {0}", pi.Item_Id);
                }
                else if (type == "product")
                {
                    sb.AppendFormat("AND p.product_id = {0}", pi.Product_Id);
                }
                return _dbAccess.getSinggleObj<ProductItemCustom>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemDao-->GetProductArriveDay" + ex.Message, ex);
            }
        }
        //查詢稅額類型 根據item_id  bymengjuan0826J 2015/7/2
        public Product GetTaxByItem(uint item_id)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@"SELECT p.product_id,p.tax_type
                                FROM product_item pi
                                    INNER JOIN product p ON p.product_id =  pi.product_id ");

                sb.AppendFormat("WHERE pi.item_id = {0}", item_id);
                return _dbAccess.getSinggleObj<Product>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemDao-->GetTaxByItem" + ex.Message, ex);
            }
        }

        public string GetItemInfoByProductIds(string productIds)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("SELECT GROUP_CONCAT(item_id) AS item_ids FROM product_item WHERE product_id IN ('{0}')",productIds);
                return _dbAccess.getDataTable(sb.ToString()).Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemDao-->GetItemInfoByProductIds" + ex.Message, ex);
            }
        }
        /***
         * chaojie1124j 2015/08/26添加GetNoticeGoods方法，實現商品建議採購量的補貨通知人數。
         *補貨通知人數 
         */
        public Dictionary<int, int> GetNoticeGoods(ProductItemQuery query)
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<int, int> NoticeGoods = new Dictionary<int, int>();
            string startTime = DateTime.Now.AddDays(-query.sumDays).ToString("yyyy-MM-dd 00:00:00");
            try
            {
                sb.AppendFormat(" select count(user_id) as Count,item_id from  arrival_notice  where create_time>='{0}' group by item_id ;", CommonFunction.GetPHPTime(startTime));
                DataTable _dtGoods = _dbAccess.getDataTable(sb.ToString());
                foreach (DataRow item in _dtGoods.Rows)
                {
                    NoticeGoods.Add(Convert.ToInt32(item["item_id"]), Convert.ToInt32(item["Count"]));
                }
                return NoticeGoods;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemDao-->GetNoticeGoods" + ex.Message, ex);
            }
        }

        public List<ProductItemQuery> GetProductItemByID(ProductItemQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("SELECT p.product_id,p.product_name as Remark,CONCAT(p.spec_title_1,' ',ps1.spec_name) as Spec_Name_1,CONCAT(p.spec_title_2,' ',ps2.spec_name) as Spec_Name_2 ");
                sb.Append(" from product_item pi left join product p on p.product_id =pi.product_id left join product_spec ps1 on ps1.spec_id=pi.spec_id_1 left join product_spec ps2 on ps2.spec_id=pi.spec_id_2");
                sb.AppendFormat(" where 1=1 ");
                if (query.Item_Id != 0)
                {
                    sb.AppendFormat(" and pi.item_id ='{0}'", query.Item_Id);
                }
                return _dbAccess.getDataTableForObj<ProductItemQuery>(sb.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemDao-->GetProductItemByID" + ex.Message, ex);
            }
        }

        public List<ProductItemQuery> GetInventoryQueryList(ProductItemQuery query, out int totalCount)// by yachao1120j 2015-9-10 商品库存查询
        {
            StringBuilder str = new StringBuilder();
            StringBuilder strcont = new StringBuilder();
            totalCount = 0;

            try
            {
                str.AppendFormat("SELECT p.product_id,p.product_name,pi.item_id,CONCAT(p.spec_title_1,' ',ps1.spec_name) as Spec_Name_1 ");
                str.AppendFormat(" ,CONCAT(p.spec_title_2,' ',ps2.spec_name) as Spec_Name_2 ,v.vendor_id,v.vendor_name_full, vb.brand_id ");
                str.Append(" ,vb.brand_name,tp1.parameterName as product_status_string,p.product_status,tp2.parameterName as sale_status_string,pi.item_stock,p.ignore_stock  ");
                strcont.AppendFormat("  from product_item pi ");
                strcont.AppendFormat(" left JOIN product p on p.product_id=pi.product_id ");
                strcont.AppendFormat(" INNER JOIN vendor_brand vb on vb.brand_id=p.brand_id ");
                strcont.AppendFormat(" INNER JOIN vendor v on v.vendor_id=vb.vendor_id ");
                strcont.AppendFormat(" left JOIN product_spec ps1 on ps1.spec_id=pi.spec_id_1 ");
                strcont.AppendFormat(" left JOIN product_spec ps2 on  ps2.spec_id=pi.spec_id_2 ");
                //strcont.AppendFormat(" inner JOIN t_parametersrc tp on tp.parameterCode=p.product_status and  tp.parameterType='product_status' ");
                strcont.AppendFormat(" inner JOIN (SELECT parameterName,parameterCode from t_parametersrc where parameterType='product_status')  tp1 on tp1.parameterCode=p.product_status  ");
                strcont.AppendFormat(" INNER JOIN (SELECT parameterName,parameterCode from t_parametersrc where parameterType='sale_status' )  tp2 on tp2.parameterCode=p.sale_status ");
                strcont.AppendFormat(" where 1=1 ");
                if (!string.IsNullOrEmpty(query.product_id_OR_product_name))//商品名稱或者商品編號或商品細項編號
                {
                    int ID = 0;
                    if (int.TryParse(query.product_id_OR_product_name, out ID))
                    {
                        if (query.product_id_OR_product_name.Length == 6)
                        {
                            strcont.AppendFormat("and pi.item_id='{0}'", query.product_id_OR_product_name);
                        }
                        else
                        {
                            strcont.AppendFormat("and p.product_id='{0}'",query.product_id_OR_product_name);
                        }
                        //strcont.AppendFormat(" and ( p.product_id = '{0}' or pi.item_id = '{1}') ", query.product_id_OR_product_name, query.product_id_OR_product_name);
                    }
                    else
                    {
                        strcont.AppendFormat(" and p.product_name LIKE '%{0}%'", query.product_id_OR_product_name);
                    }
                }
                if (!string.IsNullOrEmpty(query.vendor_name_full_OR_vendor_id))//供應商名稱或者供應商編號
                {
                    int ID = 0;
                    if (int.TryParse(query.vendor_name_full_OR_vendor_id, out ID))
                    {
                        strcont.AppendFormat(" and  v.vendor_id = '{0}' ", query.vendor_name_full_OR_vendor_id);
                    }
                    else
                    {
                        strcont.AppendFormat(" and v.vendor_name_full LIKE '%{0}%'", query.vendor_name_full_OR_vendor_id);
                    }
                }

                if (!string.IsNullOrEmpty(query.brand_id_OR_brand_name))//品牌名稱或者品牌編號
                {
                    int ID = 0;
                    if (int.TryParse(query.brand_id_OR_brand_name, out ID))
                    {
                        strcont.AppendFormat(" and vb.brand_id = '{0}'", query.brand_id_OR_brand_name);
                    }
                    else
                    {
                        strcont.AppendFormat(" and vb.brand_name LIKE '%{0}%'", query.brand_id_OR_brand_name);
                    }
                }
                if (query.product_status != 10)//商品狀態  10 代表全部
                {
                    strcont.AppendFormat("and p.product_status = '{0}'", query.product_status);
                }
                if (query.sale_status != 100)//  商品販售狀態  100 代表全部
                {
                    strcont.AppendFormat("and p.sale_status = '{0}'", query.sale_status);
                }
                if (query.item_stock_start <= query.item_stock_end)//库存数量开始--库存数量结束   
                {
                    strcont.AppendFormat("  and pi.item_stock >='{0}' and pi.item_stock <='{1}'  ", query.item_stock_start, query.item_stock_end);
                }

                strcont.AppendFormat("and p.ignore_stock = '{0}'", query.ignore_stock);//庫存為0時是否還能販售
                str.Append(strcont);

                if (query.IsPage)
                {
                    StringBuilder strpage = new StringBuilder();//  
                    StringBuilder strcontpage = new StringBuilder();
                    strpage.AppendFormat(" SELECT count(pi.item_id) as totalCount  ");
                    strpage.Append(strcont);
                    string sql = strpage.ToString();
                    DataTable _dt = _access.getDataTable(sql);
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0][0]);
                        str.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                    }
                }
                return _access.getDataTableForObj<ProductItemQuery>(str.ToString());// 獲取查詢記錄

            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemDao-->GetInventoryQueryList-->" + ex.Message);
            }

        }
    }
}