/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductItemTempDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/28 11:30:09 
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
namespace BLL.gigade.Dao
{
    public class ProductItemTempDao : IProductItemTempImplDao
    {
        private IDBAccess _dbAccess;
        public ProductItemTempDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        #region IProductItemTempImplDao 成员


        public int Save(ProductItemTemp saveTemp)
        {
            saveTemp.Replace4MySQL();
            StringBuilder stb = new StringBuilder("insert into product_item_temp (`writer_id`,`spec_id_1`,`spec_id_2`,`product_id`,`item_cost`,`item_money`,`event_product_start`,`event_product_end`,`event_item_cost`,`event_item_money`,`item_stock`,`item_alarm`,`item_status`,`item_code`,`barcode`)");
            stb.AppendFormat(" values ({0},{1},{2},'{3}',{4},{5},{6},{7},{8},{9},{10},{11},{12},'{13}','{14}')", saveTemp.Writer_Id, saveTemp.Spec_Id_1, saveTemp.Spec_Id_2, saveTemp.Product_Id, saveTemp.Item_Cost, saveTemp.Item_Money, saveTemp.Event_Product_Start, saveTemp.Event_Product_End, saveTemp.Event_Item_Cost, saveTemp.Event_Item_Money, saveTemp.Item_Stock, saveTemp.Item_Alarm, saveTemp.Item_Status, saveTemp.Item_Code, saveTemp.Barcode);

            return _dbAccess.execCommand(stb.ToString());
        }

        public int Save(List<ProductItemTemp> saveTemps)
        {
            StringBuilder stb = new StringBuilder(@"insert into product_item_temp (`writer_id`,`spec_id_1`,`spec_id_2`,`product_id`,`item_cost`,`item_money`,`event_product_start`,`event_product_end`,
            `event_item_cost`,`event_item_money`,`item_stock`,`item_alarm`,`item_status`,`item_code`,`barcode`)  values ");
            foreach (var saveTemp in saveTemps)
            {
                saveTemp.Replace4MySQL();
                stb.AppendFormat(" ({0},{1},{2},'{3}',{4},{5},{6},{7},{8},{9},{10},{11},{12},'{13}','{14}'),",
                    saveTemp.Writer_Id, saveTemp.Spec_Id_1, saveTemp.Spec_Id_2, saveTemp.Product_Id, saveTemp.Item_Cost, saveTemp.Item_Money, saveTemp.Event_Product_Start, saveTemp.Event_Product_End,
                    saveTemp.Event_Item_Cost, saveTemp.Event_Item_Money, saveTemp.Item_Stock, saveTemp.Item_Alarm, saveTemp.Item_Status, saveTemp.Item_Code, saveTemp.Barcode);
            }
            return _dbAccess.execCommand(stb.Remove(stb.Length-1,1).ToString());
        }

        public int Delete(ProductItemTemp delTemp)
        {
            StringBuilder sql = new StringBuilder("set sql_safe_updates = 0;delete from product_item_temp where 1=1");
            if (delTemp.Writer_Id != 0)
            {
                sql.AppendFormat(" and writer_id = {0}", delTemp.Writer_Id);
            }
            if (delTemp.Spec_Id_1 != 0)
            {
                sql.AppendFormat(" and spec_id_1 = {0}", delTemp.Spec_Id_1);
            }
            if (delTemp.Spec_Id_2 != 0)
            {
                sql.AppendFormat(" and spec_id_2 = {0}", delTemp.Spec_Id_2);
            }
            sql.AppendFormat(" and product_id='{0}';set sql_safe_updates = 1;", delTemp.Product_Id);
            return _dbAccess.execCommand(sql.ToString());
        }

        public List<ProductItemTemp> Query(ProductItemTemp proItemTemp)
        {
            //edit by xiangwang0413w 2014/06/18 (增加ERP廠商編號 erp_id)
            StringBuilder strSql = new StringBuilder("select a.item_id,b.spec_id as spec_id_1,b.spec_name as spec_name_1,c.spec_id as spec_id_2,c.spec_name as spec_name_2,item_cost,item_money,event_product_start");
            strSql.Append(",event_product_end,event_item_cost,event_item_money,item_code,item_stock,item_alarm,item_status,barcode,erp_id,remark,arrive_days from product_item_temp as a ");//edit by zhuoqin0830w 2015/02/05 增加備註  // add by zhuoqin0830w 2014/03/20 增加運達天數
            strSql.Append("left join product_spec_temp as b on a.spec_id_1=b.spec_id left join product_spec_temp as c on a.spec_id_2=c.spec_id");
            strSql.AppendFormat(" where a.writer_id={0}", proItemTemp.Writer_Id);
            strSql.AppendFormat(" and a.product_id='{0}'", proItemTemp.Product_Id);
            return _dbAccess.getDataTableForObj<Model.ProductItemTemp>(strSql.ToString());
        }

        public string QuerySql(ProductItemTemp proItemTemp)
        {
            StringBuilder strSql = new StringBuilder("select writer_id,item_id,spec_id_1,spec_id_2,item_cost,item_money,event_product_start,event_product_end,event_item_cost,");
            strSql.AppendFormat("event_item_money,item_stock,item_alarm,item_status,item_code,erp_id,barcode,remark,arrive_days from product_item_temp where writer_id={0}", proItemTemp.Writer_Id);
            strSql.AppendFormat(" and product_id='{0}'", proItemTemp.Product_Id);//edit by xiangwang0413w 2014/06/18 (增加ERP廠商編號 erp_id)  edit by zhuoqin0830w 2015/02/05 增加備註  // add by zhuoqin0830w 2014/03/20 增加運達天數
            return strSql.ToString();
        }

        public string UpdateCostMoney(Model.ProductItemTemp productItemTemp)
        {
            productItemTemp.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0;update product_item_temp set ");
            strSql.AppendFormat("item_cost={0},item_money={1},event_item_cost={2}", productItemTemp.Item_Cost, productItemTemp.Item_Money, productItemTemp.Event_Item_Cost);
            strSql.AppendFormat(",event_item_money={0},event_product_start={1},event_product_end={2}", productItemTemp.Event_Item_Money, productItemTemp.Event_Product_Start, productItemTemp.Event_Product_End);
            strSql.AppendFormat(",item_code='{0}' where item_id={1}", productItemTemp.Item_Code, productItemTemp.Item_Id);
            if (productItemTemp.Writer_Id != 0)
            {
                strSql.AppendFormat(" and writer_id={0}", productItemTemp.Writer_Id);
            }
            if (productItemTemp.Spec_Id_1 != 0)
            {
                strSql.AppendFormat(" and spec_id_1={0}", productItemTemp.Spec_Id_1);
            }
            if (productItemTemp.Spec_Id_2 != 0)
            {
                strSql.AppendFormat(" and spec_id_2={0}", productItemTemp.Spec_Id_2);
            }
            strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates = 1;", productItemTemp.Product_Id);
            return strSql.ToString();
        }

        public string UpdateStockAlarm(Model.ProductItemTemp productItemTemp)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0;update product_item_temp set ");
            strSql.AppendFormat("item_stock={0},item_alarm={1},barcode='{2}',remark='{3}',arrive_days={4}", productItemTemp.Item_Stock, productItemTemp.Item_Alarm, productItemTemp.Barcode, productItemTemp.Remark, productItemTemp.Arrive_Days);//add by zhuoqin0830w 2014/03/20 增加運達天數
            strSql.AppendFormat(" where item_id={0}", productItemTemp.Item_Id);
            if (productItemTemp.Writer_Id != 0)
            {
                strSql.AppendFormat(" and writer_id={0}", productItemTemp.Writer_Id);
            }
            if (productItemTemp.Spec_Id_1 != 0)
            {
                strSql.AppendFormat(" and spec_id_1={0}", productItemTemp.Spec_Id_1);
            }
            if (productItemTemp.Spec_Id_2 != 0)
            {
                strSql.AppendFormat(" and spec_id_2={0}", productItemTemp.Spec_Id_2);
            }
            strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates = 1;", productItemTemp.Product_Id);
            return strSql.ToString();
        }

        public int UpdateItemStock(ProductItemTemp proItemTemp)
        {
            try
            {
                string strSql = string.Format("set sql_safe_updates=0;update product_item_temp set item_stock={0} where product_id='{1}' and writer_id={2};set sql_safe_updates=1;",
                    proItemTemp.Item_Stock, proItemTemp.Product_Id, proItemTemp.Writer_Id);
                return _dbAccess.execCommand(strSql);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempDao.UpdateItemStock-->" + ex.Message, ex);
            }
        }

        //將臨時表數據導入正式表
        public string MoveProductItem(ProductItemTemp proItemTemp)
        {
            //StringBuilder strSql = new StringBuilder("insert into product_item(item_id,product_id,spec_id_1,spec_id_2,item_cost,item_money,event_product_start,");
            //strSql.Append("event_product_end,event_item_cost,event_item_money,item_stock,item_alarm,item_status,item_code,barcode)select {0} as item_id,{1} as product_id,");
            //strSql.Append("spec_id_1,spec_id_2,item_cost,item_money,event_product_start,event_product_end,event_item_cost,event_item_money,");
            //strSql.Append("item_stock,item_alarm,item_status,item_code,");
            //strSql.AppendFormat("barcode from product_item_temp where writer_id={0}", proItemTemp.Writer_Id);
            //strSql.AppendFormat(" and product_id ='{0}'", proItemTemp.Product_Id );
            //strSql.Append(" and item_id={2}");
            // edit by wangwei0216w 2014/9/23
            StringBuilder strSql = new StringBuilder("insert into product_item(item_id,product_id,spec_id_1,spec_id_2,event_product_start,");
            strSql.Append("event_product_end,item_stock,item_alarm,item_status,item_code,barcode,remark,arrive_days)select {0} as item_id,{1} as product_id,");
            strSql.Append("spec_id_1,spec_id_2,event_product_start,event_product_end,");
            strSql.Append("item_stock,item_alarm,item_status,item_code,");
            strSql.AppendFormat("barcode,remark,arrive_days from product_item_temp where writer_id={0}", proItemTemp.Writer_Id);// add by zhuoqin0830w 2014/03/20 增加運達天數
            strSql.AppendFormat(" and product_id ='{0}'", proItemTemp.Product_Id);
            strSql.Append(" and item_id={2}");
            return strSql.ToString();
        }

        public string DeleteSql(ProductItemTemp proItemTemp)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0;delete from product_item_temp ");
            strSql.AppendFormat("where writer_id={0}", proItemTemp.Writer_Id);
            strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates = 1;", proItemTemp.Product_Id);
            return strSql.ToString();
        }


        public string SaveFromProItem(ProductItemTemp proItemTemp)
        {
            //StringBuilder strSql = new StringBuilder("insert into product_item_temp(writer_id,product_id,spec_id_1,spec_id_2,item_cost,item_money,event_product_start,event_product_end,event_item_cost,");
            //strSql.Append("event_item_money,item_stock,item_alarm,item_status,item_code,barcode)");
            //strSql.AppendFormat("select {0} as writer_id,product_id,spec_id_1,spec_id_2,item_cost,item_money,event_product_start,event_product_end,event_item_cost,", proItemTemp.Writer_Id);
            //strSql.AppendFormat("event_item_money,item_stock,item_alarm,item_status,item_code,barcode from product_item where product_id={0}", proItemTemp.Product_Id);
            //return strSql.ToString();
            //edit by xiangwang0413w 2014/09/25 product_item裡的價格不在使用,改用item_price裡的價格
            StringBuilder strSql = new StringBuilder("insert into product_item_temp(writer_id,product_id,spec_id_1,spec_id_2,event_product_start,event_product_end,");
            strSql.Append("item_money,item_cost,event_item_money,event_item_cost,");
            strSql.Append("item_alarm,item_status,item_code,barcode,remark)");//edit by zhuoqin0830w  2015/04/01  去掉 arrive_days 欄位
            strSql.AppendFormat("select {0} as writer_id,pi.product_id,spec_id_1,spec_id_2,event_product_start,event_product_end,", proItemTemp.Writer_Id);
            strSql.Append("ip.item_money,ip.item_cost,ip.event_money as event_item_money,ip.event_cost as event_item_cost,");//item_price
            strSql.Append("item_alarm,item_status,item_code,barcode,remark from product_item pi "); //edit by zhuoqin0830w  2015/04/01  去掉 arrive_days 欄位
            strSql.Append(" left join item_price ip on pi.item_id=ip.item_id ");
            strSql.Append(" inner join price_master pm on ip.price_master_id=pm.price_master_id and (pm.site_id=1 and pm.user_level=1 and pm.user_id=0) ");
            strSql.AppendFormat(" where pi.product_id='{0}'", proItemTemp.Product_Id);
            return strSql.ToString();
        }

        public string UpdateCopySpecId(ProductItemTemp proItemTemp)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0;update product_item_temp set spec_id_1={0}");
            strSql.AppendFormat(" where writer_id={0} and spec_id_1={1}", proItemTemp.Writer_Id, proItemTemp.Spec_Id_1);
            strSql.AppendFormat(" and product_id='{0}';", proItemTemp.Product_Id);
            strSql.Append("update product_item_temp set spec_id_2={0}");
            strSql.AppendFormat(" where writer_id={0} and spec_id_2={1}", proItemTemp.Writer_Id, proItemTemp.Spec_Id_2);
            strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates = 1;", proItemTemp.Product_Id);
            return strSql.ToString();
        }
        #region 供應商修改
        public List<Model.ProductItemTemp> QueryByVendor(ProductItemTemp proItemTemp)
        {
            //edit by shuangshuang0420j 2014/09/01 (用於獲得供應商新增商品的價格)
            //edit by jialei 20140912 (用於商品管理供應商申請審核)
            StringBuilder strSql = new StringBuilder("");
            try
            {
                strSql.Append(" select a.item_id,a.product_id,b.spec_id as spec_id_1,b.spec_name as spec_name_1,c.spec_id as spec_id_2,c.spec_name as spec_name_2,item_cost,item_money,event_product_start");
                strSql.Append(",event_product_end,event_item_cost,event_item_money,item_code,item_stock,item_alarm,item_status,barcode,erp_id from product_item_temp as a ");
                strSql.Append("left join product_spec_temp as b on a.spec_id_1=b.spec_id left join product_spec_temp as c on a.spec_id_2=c.spec_id where 1=1 ");
                if (proItemTemp.Writer_Id != 0)
                {
                    strSql.AppendFormat(" and a.writer_id={0}", proItemTemp.Writer_Id);
                }
                if (!string.IsNullOrEmpty(proItemTemp.Product_Id))
                {
                    strSql.AppendFormat(" and a.product_id='{0}';", proItemTemp.Product_Id);
                }
                return _dbAccess.getDataTableForObj<Model.ProductItemTemp>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempDao-->QueryByVendor-->" + ex.Message, ex);
            }

        }

        public int SaveByVendor(ProductItemTemp saveTemp)
        {
            saveTemp.Replace4MySQL();
            StringBuilder stb = new StringBuilder("insert into product_item_temp (`writer_id`,`spec_id_1`,`spec_id_2`,`product_id`,`item_cost`,`item_money`,`event_product_start`,`event_product_end`,`event_item_cost`,`event_item_money`,`item_stock`,`item_alarm`,`item_status`,`item_code`,`barcode`)");
            stb.AppendFormat(" values ({0},{1},{2},'{3}',{4},{5},{6},{7},{8},{9},{10},{11},{12},'{13}','{14}')", saveTemp.Writer_Id, saveTemp.Spec_Id_1, saveTemp.Spec_Id_2, saveTemp.Product_Id, saveTemp.Item_Cost, saveTemp.Item_Money, saveTemp.Event_Product_Start, saveTemp.Event_Product_End, saveTemp.Event_Item_Cost, saveTemp.Event_Item_Money, saveTemp.Item_Stock, saveTemp.Item_Alarm, saveTemp.Item_Status, saveTemp.Item_Code, saveTemp.Barcode);

            return _dbAccess.execCommand(stb.ToString());
        }

        //單一商品修改時將修改後的數據更新至正式表product_item
        public string UpdateByVendor(Model.ProductItemTemp item)
        {
            item.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("update product_item_temp set ");
            strSql.AppendFormat(" product_id='{0}',spec_id_1={1},spec_id_2={2},item_cost={3}", item.Product_Id, item.Spec_Id_1, item.Spec_Id_2, item.Item_Cost);
            strSql.AppendFormat(",item_money={0},event_product_start={1},event_product_end={2}", item.Item_Money, item.Event_Product_Start, item.Event_Product_End); ;
            strSql.AppendFormat(",event_item_cost={0},event_item_money={1},item_stock={2},item_alarm={3}", item.Event_Item_Cost, item.Event_Item_Money, item.Item_Stock, item.Item_Alarm);
            strSql.AppendFormat(",item_status={0},item_code='{1}',barcode='{2}' where item_id={3}", item.Item_Status, item.Item_Code, item.Barcode, item.Item_Id);
            return strSql.ToString();
        }


        public int DeleteByVendor(ProductItemTemp delTemp)
        {
            string sql = DeleteVendorSql(delTemp);
            return _dbAccess.execCommand(sql.ToString());
        }
        public string DeleteVendorSql(ProductItemTemp proItemTemp)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0;delete from product_item_temp where 1=1 ");

            if (proItemTemp.Writer_Id != 0)
            {
                strSql.AppendFormat(" and writer_id = {0}", proItemTemp.Writer_Id);
            }
            if (proItemTemp.Spec_Id_1 != 0)
            {
                strSql.AppendFormat(" and spec_id_1 = {0}", proItemTemp.Spec_Id_1);
            }
            if (proItemTemp.Spec_Id_2 != 0)
            {
                strSql.AppendFormat(" and spec_id_2 = {0}", proItemTemp.Spec_Id_2);
            }
            if (!string.IsNullOrEmpty(proItemTemp.Product_Id))
            {
                strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates = 1;", proItemTemp.Product_Id);
            }

            return strSql.ToString();
        }


        public string UpdateStockAlarmByVendor(Model.ProductItemTemp productItemTemp)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0;update product_item_temp set ");
            strSql.AppendFormat("item_stock={0},item_alarm={1},barcode='{2}'", productItemTemp.Item_Stock, productItemTemp.Item_Alarm, productItemTemp.Barcode);
            strSql.AppendFormat(" where item_id={0}", productItemTemp.Item_Id);
            if (productItemTemp.Writer_Id != 0)
            {
                strSql.AppendFormat(" and writer_id={0}", productItemTemp.Writer_Id);
            }
            if (productItemTemp.Spec_Id_1 != 0)
            {
                strSql.AppendFormat(" and spec_id_1={0}", productItemTemp.Spec_Id_1);
            }
            if (productItemTemp.Spec_Id_2 != 0)
            {
                strSql.AppendFormat(" and spec_id_2={0}", productItemTemp.Spec_Id_2);
            }
            strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates = 1;", productItemTemp.Product_Id);
            return strSql.ToString();
        }
        public string VendorSaveFromProItem(ProductItemTemp proItemTemp, string old_product_id)
        {//20140905 供應商複製
            StringBuilder strSql = new StringBuilder("insert into product_item_temp(writer_id,product_id,spec_id_1,spec_id_2,item_cost,item_money,event_product_start,event_product_end,event_item_cost,");
            strSql.Append("event_item_money,item_stock,item_alarm,item_status,item_code,barcode)");
            strSql.AppendFormat("select {0} as writer_id,'{1}' as product_id,spec_id_1,spec_id_2,item_cost,item_money,event_product_start,event_product_end,event_item_cost,", proItemTemp.Writer_Id, proItemTemp.Product_Id);
            strSql.AppendFormat("event_item_money,item_stock,item_alarm,item_status,item_code,barcode");
            uint productid = 0;
            if (uint.TryParse(old_product_id, out productid))
            {
                strSql.AppendFormat(" from product_item where product_id={0}", productid);
            }
            else
            {
                strSql.AppendFormat(" from product_item_temp where product_id='{0}'", old_product_id);
            }
            return strSql.ToString();
        }

        public string VendorUpdateCopySpecId(ProductItemTemp proItemTemp)
        {//20140905 供應商複製
            StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0;update product_item_temp set spec_id_1={0} ");
            strSql.AppendFormat("where writer_id={0} and spec_id_1={1} ", proItemTemp.Writer_Id, proItemTemp.Spec_Id_1);
            strSql.AppendFormat(" and product_id='{0}';", proItemTemp.Product_Id);
            strSql.Append(" update product_item_temp set spec_id_2='{0}' ");
            strSql.AppendFormat(" where writer_id={0} and spec_id_2={1}", proItemTemp.Writer_Id, proItemTemp.Spec_Id_2);
            strSql.AppendFormat(" and product_id='{0}';", proItemTemp.Product_Id);
            strSql.AppendFormat(" set sql_safe_updates = 1;");
            return strSql.ToString();
        }
        public string VendorQuerySql(ProductItemTemp proItemTemp)
        {//add by jialei 20140916
            StringBuilder strSql = new StringBuilder("select writer_id,item_id,spec_id_1,spec_id_2,item_cost,item_money,event_product_start,event_product_end,event_item_cost,");
            strSql.Append("event_item_money,item_stock,item_alarm,item_status,item_code,erp_id,barcode from product_item_temp where 1=1");
            if (proItemTemp.Writer_Id != 0)
            {
                strSql.AppendFormat(" and writer_id={0}", proItemTemp.Writer_Id);

            }
            strSql.AppendFormat(" and product_id='{0}';", proItemTemp.Product_Id);
            return strSql.ToString();
        }
        public string VendorMoveProductItem(ProductItemTemp proItemTemp)
        {
            //StringBuilder strSql = new StringBuilder("insert into product_item(item_id,product_id,spec_id_1,spec_id_2,item_cost,item_money,event_product_start,");
            //strSql.Append("event_product_end,event_item_cost,event_item_money,item_stock,item_alarm,item_status,item_code,barcode)select {0} as item_id,{1} as product_id,");
            //strSql.Append("spec_id_1,spec_id_2,item_cost,item_money,event_product_start,event_product_end,event_item_cost,event_item_money,");
            //strSql.Append("item_stock,item_alarm,item_status,item_code,barcode from product_item_temp where 1=1");
            //if (proItemTemp.Writer_Id != 0)
            //{
            //    strSql.AppendFormat(" and writer_id={0}", proItemTemp.Writer_Id);
            //}
            //strSql.AppendFormat(" and product_id ='{0}'", proItemTemp.Product_Id );
            //strSql.Append(" and item_id={2};");
            //return strSql.ToString();

            // edit by mengjuan0826j 2014/10/16
            StringBuilder strSql = new StringBuilder("insert into product_item(item_id,product_id,spec_id_1,spec_id_2,event_product_start,");
            strSql.Append("event_product_end,item_stock,item_alarm,item_status,item_code,barcode)select {0} as item_id,{1} as product_id,");
            strSql.Append("spec_id_1,spec_id_2,event_product_start,event_product_end,");
            strSql.Append("item_stock,item_alarm,item_status,item_code,");
            strSql.AppendFormat("barcode from product_item_temp  where 1=1");
            if (proItemTemp.Writer_Id != 0)
            {
                strSql.AppendFormat(" and writer_id={0}", proItemTemp.Writer_Id);
            }
            strSql.AppendFormat(" and product_id ='{0}'", proItemTemp.Product_Id);
            strSql.Append(" and item_id={2};");
            return strSql.ToString();
        }
        #endregion
        #endregion
    }
}
