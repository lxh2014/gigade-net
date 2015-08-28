using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using DBAccess;
using BLL.gigade.Model.Custom;
namespace BLL.gigade.Dao
{
    public class ProductComboTempDao : BLL.gigade.Dao.Impl.IProductComboTempImplDao
    {
        private IDBAccess _access;
        public ProductComboTempDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public int Save(ProductComboTemp saveTemp)
        {
            StringBuilder stb = new StringBuilder("insert into product_combo_temp (`writer_id`,`parent_id`,`child_id`,`s_must_buy`,`g_must_buy`,`pile_id`,`buy_limit`)");
            stb.AppendFormat(" values({0},{1},'{2}',{3},{4},{5},{6})", saveTemp.Writer_Id, saveTemp.Parent_Id, saveTemp.Child_Id, saveTemp.S_Must_Buy, saveTemp.G_Must_Buy, saveTemp.Pile_Id, saveTemp.Buy_Limit);
            return _access.execCommand(stb.ToString());
        }

        public int Delete(ProductComboTemp delTemp)
        {
            StringBuilder stb = new StringBuilder("set sql_safe_updates = 0;delete from product_combo_temp where 1=1 ");
            if (delTemp.Writer_Id != 0)
            {
                stb.AppendFormat("and writer_id = {0}", delTemp.Writer_Id);
            }

            stb.AppendFormat(" and parent_id='{0}';set sql_safe_updates = 1;", uint.Parse(delTemp.Parent_Id) );
            return _access.execCommand(stb.ToString());
        }

        public List<ProductComboCustom> combQuery(ProductComboCustom query)
        {
            StringBuilder sql = new StringBuilder("select c.child_id,p.product_name,p.prod_sz,p.brand_id,c.s_must_buy,c.g_must_buy,c.pile_id,c.buy_limit from product p inner join product_combo_temp c  on p.product_id = c.child_id where 1=1");

            if (query.Writer_Id != 0)
            {
                sql.AppendFormat(" and c.writer_id = {0}", query.Writer_Id);
            }
            if (query.Pile_Id != 0)
            {
                sql.AppendFormat(" and c.pile_id = {0}", query.Pile_Id);
            }
            sql.AppendFormat(" and parent_id='{0}'", query.Parent_Id );
            //sql.AppendFormat(" and parent_id={0}", query.Parent_Id); // edit 2014/9/25
            return _access.getDataTableForObj<ProductComboCustom>(sql.ToString());
        }

        public List<ProductComboCustom> comboPriceQuery(ProductComboCustom query)
        {
            StringBuilder sql = new StringBuilder("select c.child_id,p.product_name,p.prod_sz,c.s_must_buy,c.g_must_buy,c.pile_id,c.buy_limit,m.price_master_id,m.price as item_money,m.event_price as event_money,m.event_cost,m.cost as item_cost ");
            sql.Append(" from product p ");
            sql.Append(" inner join product_combo_temp c  on p.product_id = c.child_id ");
            sql.AppendFormat(" left join price_master_temp m on c.child_id = m.child_id and m.writer_id= {0} and m.product_id='{1}'", query.Writer_Id, query.Parent_Id );
            sql.Append(" where 1=1");
            if (query.Writer_Id != 0)
            {
                sql.AppendFormat(" and c.writer_id = {0}", query.Writer_Id);
            }
            if (query.Pile_Id != 0)
            {
                sql.AppendFormat(" and c.pile_id = {0}", query.Pile_Id);
            }
            sql.AppendFormat(" and parent_id='{0}'", query.Parent_Id );
            return _access.getDataTableForObj<ProductComboCustom>(sql.ToString());
        }

        public List<ProductComboCustom> priceComboQuery(ProductComboCustom query)
        {
            StringBuilder sql = new StringBuilder("select m.child_id,c.pile_id from price_master_temp m");
            sql.Append(" inner join product_combo_temp c on m.child_id = c.child_id and m.product_id = c.parent_id and m.writer_id = c.writer_id");
            sql.AppendFormat(" where m.writer_id = {0} and m.combo_type = 2 and m.product_id = {1}", query.Writer_Id, query.Parent_Id);
            return _access.getDataTableForObj<ProductComboCustom>(sql.ToString());
        }


        public List<MakePriceCustom> differentSpecQuery(ProductComboCustom query)
        {
            StringBuilder stb = new StringBuilder("select a.product_id as child_id,a.item_id,b.product_name,b.prod_sz,c.spec_name as spec_1,d.spec_name as spec_2,e.s_must_buy,e.g_must_buy,  ");
            stb.Append("e.parent_id,e.pile_id,e.buy_limit,g.item_money,g.item_cost,g.event_money,g.event_cost,g.item_price_id,f.price_master_id ");
            stb.Append(" from product_item a ");
            stb.Append("left join product b on a.product_id=b.product_id ");
            stb.Append("left join product_spec c on a.spec_id_1=c.spec_id ");
            stb.Append("left join product_spec d on a.spec_id_2=d.spec_id ");
            stb.AppendFormat("left join (select pile_id,s_must_buy,g_must_buy,child_id,parent_id,writer_id,buy_limit from product_combo_temp where writer_id={0} and parent_id={1}) e on a.product_id=e.child_id ", query.Writer_Id, query.Parent_Id);
            stb.AppendFormat(" left join price_master_temp f on  f.combo_type=2 and f.writer_id={0} and f.child_id<>'0' and a.product_id=f.child_id and f.product_id={1} ", query.Writer_Id, query.Parent_Id );
            stb.AppendFormat(" left join item_price_temp g on g.price_master_id=f.price_master_id and a.item_id=g.item_id ");
            stb.AppendFormat("where a.product_id in (select child_id from product_combo_temp where writer_id={0} and parent_id='{1}')", query.Writer_Id, query.Parent_Id);
            stb.AppendFormat(" and e.parent_id='{0}'", query.Parent_Id );
            return _access.getDataTableForObj<MakePriceCustom>(stb.ToString());
        }

        public List<ProductComboTemp> groupNumQuery(ProductComboTemp query)
        {
            StringBuilder strSql = new StringBuilder("select pile_id,g_must_buy,buy_limit from product_combo_temp ");
            strSql.AppendFormat("where writer_id = {0}", query.Writer_Id);
            strSql.AppendFormat(" and parent_id='{0}'", query.Parent_Id);
            strSql.Append(" group by pile_id,g_must_buy,buy_limit");
            return _access.getDataTableForObj<ProductComboTemp>(strSql.ToString());
        }

        public string TempMoveCombo(ProductComboTemp query)
        {//edit jialei 20140917 加判斷writerId
            StringBuilder stb = new StringBuilder("insert into product_combo(`parent_id`,`child_id`,`s_must_buy`,`g_must_buy`,`pile_id`,`buy_limit`) ");
            stb.Append("select {0} as parent_id,child_id,s_must_buy,g_must_buy,pile_id,buy_limit from product_combo_temp where 1=1 ");
            if (query.Writer_Id != 0)
            {
                stb.AppendFormat(" and writer_id = {0}", query.Writer_Id);
            }
            stb.AppendFormat(" and parent_id='{0}';", query.Parent_Id);
            return stb.ToString();
        }

        public string TempDelete(ProductComboTemp query)
        {//edit jialei 20140917 加判斷writerId
            StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0; delete from product_combo_temp where 1=1");
            if (query.Writer_Id != 0)
            {
                strSql.AppendFormat(" and writer_id = {0}", query.Writer_Id);
            }
            //strSql.AppendFormat(" and parent_id='{0}';set sql_safe_updates= 1;", query.Parent_Id); 
            strSql.AppendFormat(" and parent_id='{0}';set sql_safe_updates= 1;", query.Parent_Id);//edit by wangwei0216w 改回原先代碼
            return strSql.ToString();
        }

        public string SaveFromCombo(ProductComboTemp proComboTemp)
        {
            StringBuilder strSql = new StringBuilder("insert into product_combo_temp(`writer_id`,`parent_id`,`child_id`");
            strSql.Append(",`s_must_buy`,`g_must_buy`,`pile_id`,`buy_limit`) select ");
            strSql.AppendFormat("{0} as writer_id,parent_id,child_id,s_must_buy,g_must_buy,pile_id,buy_limit from product_combo where parent_id='{1}'", proComboTemp.Writer_Id, proComboTemp.Parent_Id);
            return strSql.ToString();
        }


        #region 供應商商品處理

        #region 刪除供應商新建的商品的臨時數據Sql語句 + string TempDeleteByVendor(ProductComboTemp query)
        public string TempDeleteByVendor(ProductComboTemp query)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("set sql_safe_updates = 0; delete from product_combo_temp where 1=1");
                if (query.Writer_Id != 0)
                {
                    strSql.AppendFormat(" and writer_id = {0}", query.Writer_Id);
                }
                if (!string.IsNullOrEmpty(query.Parent_Id))
                {
                    strSql.AppendFormat(" and parent_id='{0}'", query.Parent_Id);
                }
                strSql.Append(" ;set sql_safe_updates= 1;");
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempDao-->TempDeleteByVendor-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion


        #region 執行刪除供應商新建的商品的臨時數據+ string DeleteByVendor(ProductComboTemp delTemp)
        public int DeleteByVendor(ProductComboTemp delTemp)
        {

            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("set sql_safe_updates = 0; delete from product_combo_temp ");
                strSql.AppendFormat("where writer_id = {0}", delTemp.Writer_Id);
                if (!string.IsNullOrEmpty(delTemp.Parent_Id))
                {
                    strSql.AppendFormat(" and parent_id='{0}' ", delTemp.Parent_Id);
                }

                strSql.Append("; set sql_safe_updates= 1;");
                return _access.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempDao-->DeleteByVendor-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        #region 查詢組合商品組合信息 +List<ProductComboCustomVendor> combQueryByVendor(ProductComboCustomVendor query)
        public List<ProductComboCustomVendor> combQueryByVendor(ProductComboCustomVendor query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                //子商品為正式單一商品時
                StringBuilder productSql = new StringBuilder();
                productSql.Append("select c.child_id ,p.product_name,p.prod_sz,p.brand_id,c.s_must_buy, ");
                productSql.Append("c.g_must_buy,c.pile_id,c.buy_limit from product p ");
                productSql.Append(" inner join product_combo_temp c  on p.product_id = c.child_id where 1=1");

                if (query.Writer_Id != 0)
                {
                    productSql.AppendFormat(" and c.writer_id = {0}", query.Writer_Id);
                }
                if (query.Pile_Id != 0)
                {
                    productSql.AppendFormat(" and c.pile_id = {0}", query.Pile_Id);
                }
                if (!string.IsNullOrEmpty(query.Parent_Id))
                {
                    productSql.AppendFormat(" and parent_id='{0}' ", query.Parent_Id);
                }

                //子商品為供應商新建立的已經完成的單一商品時
                StringBuilder productTempSql = new StringBuilder();
                productTempSql.Append("select ct.child_id  ,pt.product_name,pt.prod_sz,pt.brand_id,ct.s_must_buy, ");
                productTempSql.Append("ct.g_must_buy,ct.pile_id,ct.buy_limit from product_temp pt  ");
                productTempSql.Append("  inner join product_combo_temp ct  on pt.product_id = ct.child_id  where 1=1 ");

                if (query.Writer_Id != 0)
                {
                    productTempSql.AppendFormat(" and ct.writer_id = {0}", query.Writer_Id);
                }
                if (query.Pile_Id != 0)
                {
                    productTempSql.AppendFormat(" and ct.pile_id = {0}", query.Pile_Id);
                }
                if (query.create_channel != 0)
                {
                    productTempSql.AppendFormat(" and pt.create_channel = {0}", query.create_channel);
                }
                if (query.temp_status != 0)
                {
                    productTempSql.AppendFormat(" and pt.temp_status = {0}", query.temp_status);
                }
                if (!string.IsNullOrEmpty(query.Parent_Id))
                {
                    productTempSql.AppendFormat(" and parent_id='{0}' ", query.Parent_Id);
                }

                sql.Append("(" + productSql + ")" + " UNION " + "(" + productTempSql + ")");

                return _access.getDataTableForObj<ProductComboCustomVendor>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempDao-->combQueryByVendor-->" + ex.Message + sql.ToString(), ex);
            }
        }

        #endregion


        #region 保存組合商品規格 +int SaveByVendor(ProductComboTemp saveTemp)
        public int SaveByVendor(ProductComboTemp saveTemp)
        {
            StringBuilder stb = new StringBuilder();
            try
            {
                stb.Append("insert into product_combo_temp (`writer_id`,`parent_id`,`child_id`,`s_must_buy`,`g_must_buy`,`pile_id`,`buy_limit`)");
                stb.AppendFormat(" values({0},'{1}','{2}',{3},{4},{5},{6})", saveTemp.Writer_Id, saveTemp.Parent_Id, saveTemp.Child_Id, saveTemp.S_Must_Buy, saveTemp.G_Must_Buy, saveTemp.Pile_Id, saveTemp.Buy_Limit);
                return _access.execCommand(stb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempDao-->SaveByVendor-->" + ex.Message + stb.ToString(), ex);
            }
        }
        #endregion


        #region 獲取群組信息 + List<ProductComboTemp> groupNumQueryByVendor(ProductComboTemp query)
        /// <summary>
        /// 獲取群組信息
        /// </summary>
        /// <param name="query">ProductComboTemp model對象</param>
        /// <returns> List<ProductComboTemp> </returns>
        public List<ProductComboTemp> groupNumQueryByVendor(ProductComboTemp query)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("select pile_id,g_must_buy,buy_limit from product_combo_temp where 1=1");
                if (query.Writer_Id != 0)
                {
                    strSql.AppendFormat(" and writer_id = {0}", query.Writer_Id);
                }
                if (!string.IsNullOrEmpty(query.Parent_Id))
                {
                    strSql.AppendFormat(" and parent_id='{0}' ", query.Parent_Id);
                }
                strSql.Append(" group by pile_id,g_must_buy,buy_limit");
                return _access.getDataTableForObj<ProductComboTemp>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempDao-->groupNumQueryByVendor-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        public string VendorSaveFromCombo(ProductComboTemp proComboTemp, string old_id)
        {//20140905 複製供應商
            StringBuilder strSql = new StringBuilder("insert into product_combo_temp");
            strSql.AppendFormat("(`writer_id`,`parent_id`,`child_id`,`s_must_buy`,`g_must_buy`,`pile_id`,`buy_limit`) select ");
            strSql.AppendFormat("{0} as writer_id,'{1}' as parent_id,child_id,s_must_buy,g_must_buy,pile_id,buy_limit ", proComboTemp.Writer_Id, proComboTemp.Parent_Id);
            uint productid = 0;
            if (uint.TryParse(old_id, out productid))
            {
                strSql.AppendFormat(" from product_combo where parent_id={0};", productid);
            }
            else
            {
                strSql.AppendFormat(" from product_combo_temp where parent_id='{0}';", old_id);
            }
            return strSql.ToString();
        }

        #region 獲取組合商品的子商品信息
        public List<ProductTemp> QueryChildStatusVendor(ProductComboTemp query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                //子商品為正式單一商品時
                //add 判斷運費和運送方式

                StringBuilder productcom = new StringBuilder();
                productcom.AppendFormat("(SELECT product_id,product_name,prod_sz,brand_id,product_status,product_mode,product_freight_set from product_temp pt where product_id='{0}') UNION ", query.Parent_Id);
                StringBuilder productSql = new StringBuilder();
                productSql.Append("select c.child_id as product_id ,p.product_name,p.prod_sz,p.brand_id,p.product_status,p.product_mode,p.product_freight_set ");
                productSql.Append(" from product p ");
                productSql.Append(" inner join product_combo_temp c  on p.product_id = c.child_id where 1=1");
                if (query.Writer_Id != 0)
                {
                    productSql.AppendFormat(" and c.writer_id = {0}", query.Writer_Id);
                }

                if (!string.IsNullOrEmpty(query.Parent_Id))
                {
                    productSql.AppendFormat(" and parent_id='{0}' ", query.Parent_Id);
                }
                //子商品為供應商新建立的已經完成的單一商品時
                StringBuilder productTempSql = new StringBuilder();
                productTempSql.Append("select ct.child_id as product_id ,pt.product_name,pt.prod_sz,pt.brand_id,pt.product_status,pt.product_mode,pt.product_freight_set ");
                productTempSql.Append(" from product_temp pt  ");
                productTempSql.Append("  inner join product_combo_temp ct  on pt.product_id = ct.child_id  where 1=1 ");
                if (query.Writer_Id != 0)
                {
                    productTempSql.AppendFormat(" and ct.writer_id = {0}", query.Writer_Id);
                }
                if (!string.IsNullOrEmpty(query.Parent_Id))
                {
                    productTempSql.AppendFormat(" and parent_id='{0}' ", query.Parent_Id);
                }
                sql.Append(productcom + "(" + productSql + ")" + " UNION " + "(" + productTempSql + ")");
                return _access.getDataTableForObj<ProductTemp>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempDao-->QueryChildStatusVendor-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 供應商商品審核時將商品臨時表中的組合商品數據添加到琥正式表中
        /// <summary>
        /// 供應商商品審核時將商品臨時表中的組合商品數據添加到琥正式表中
        /// </summary>
        /// <param name="proComboTemp">組合商品對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_TempMoveCombo(ProductComboTemp proComboTemp)
        {
            StringBuilder stb = new StringBuilder("insert into product_combo(`parent_id`,`child_id`,`s_must_buy`,`g_must_buy`,`pile_id`,`buy_limit`) ");
            stb.Append("select {0} as parent_id,child_id,s_must_buy,g_must_buy,pile_id,buy_limit from product_combo_temp where 1=1 ");
            stb.AppendFormat(" and parent_id='{0}';", proComboTemp.Parent_Id);
            return stb.ToString();
        } 
        #endregion
        #endregion
    }
}
