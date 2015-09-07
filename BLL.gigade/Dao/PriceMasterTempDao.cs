using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model;
using MySql.Data.MySqlClient;
using BLL.gigade.Dao.Impl;
using System.Collections;

namespace BLL.gigade.Dao
{
    public class PriceMasterTempDao : IPriceMasterTempImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public PriceMasterTempDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }
        #region IPriceMasterImplDao 成员

        public Model.Custom.PriceMasterProductCustom Query(PriceMasterTemp priceMasterTemp)
        {
            priceMasterTemp.Replace4MySQL();
            StringBuilder stb = new StringBuilder();
            try
            {

                stb.Append("select a.price_type,a.product_price_list,b.product_name,b.default_bonus_percent,b.bonus_percent,a.bag_check_money,b.accumulated_bonus,b.bonus_percent_start,b.bonus_percent_end,");
                stb.Append(" b.price,b.event_price,b.cost,b.event_cost,b.event_start,b.event_end,b.same_price,b.valid_start,b.valid_end,a.show_listprice,a.product_mode from product_temp a right join ");
                stb.AppendFormat("price_master_temp b on a.writer_id=b.writer_id and a.combo_type=b.combo_type and a.product_id=b.product_id and a.combo_type={0} where a.writer_id={1}", priceMasterTemp.combo_type, priceMasterTemp.writer_Id);
                stb.AppendFormat(" and b.product_id='{0}' and b.child_id='{1}'", priceMasterTemp.product_id, priceMasterTemp.child_id);
                return _dbAccess.getSinggleObj<Model.Custom.PriceMasterProductCustom>(stb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempDao.Query-->" + ex.Message + stb.ToString(), ex);
            }
        }

        public string Save(PriceMasterTemp priceMaster)
        {
            priceMaster.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("");
            try
            {
                strSql.Append("insert into price_master_temp(`writer_id`,`product_id`,`site_id`,`user_level`,`user_id`,`product_name`,`bonus_percent`,`default_bonus_percent`,");
                strSql.Append("`same_price`,`event_start`,`event_end`,`price_status`,`price`,`event_price`,`child_id`,`combo_type`,`cost`,`event_cost`,`accumulated_bonus`,`bonus_percent_start`");
                strSql.AppendFormat(",`bonus_percent_end`,`max_price`,`max_event_price`,`valid_start`,`valid_end`)values({0},'{1}',{2},{3}", priceMaster.writer_Id, string.IsNullOrEmpty(priceMaster.product_id) ? priceMaster.product_id.ToString() : priceMaster.product_id, priceMaster.site_id, priceMaster.user_level);
                strSql.AppendFormat(",{0},'{1}',{2},{3},{4},", priceMaster.user_id, priceMaster.product_name, priceMaster.bonus_percent, priceMaster.default_bonus_percent, priceMaster.same_price);
                strSql.AppendFormat("{0},{1},{2},{3},", priceMaster.event_start, priceMaster.event_end, priceMaster.price_status, priceMaster.price);
                strSql.AppendFormat("{0},'{1}',{2},{3},{4},{5},", priceMaster.event_price, priceMaster.child_id, priceMaster.combo_type, priceMaster.cost, priceMaster.event_cost, priceMaster.accumulated_bonus);
                strSql.AppendFormat("{0},{1},{2},{3},{4},{5});select @@identity;", priceMaster.bonus_percent_start, priceMaster.bonus_percent_end, priceMaster.max_price, priceMaster.max_event_price, priceMaster.valid_start, priceMaster.valid_end);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempDao.Save-->" + ex.Message + strSql.ToString(), ex);
            }
        }

        public string Update(PriceMasterTemp priceMaster)
        {
            priceMaster.Replace4MySQL();
            try
            {
                StringBuilder strSql = new StringBuilder("set sql_safe_updates=0;update price_master_temp set ");
                strSql.AppendFormat(" writer_id={0},site_id={1},user_level={2},user_id={3},", priceMaster.writer_Id, priceMaster.site_id, priceMaster.user_level, priceMaster.user_id);
                strSql.AppendFormat("product_name='{0}',bonus_percent={1},default_bonus_percent={2},", priceMaster.product_name, priceMaster.bonus_percent, priceMaster.default_bonus_percent);
                strSql.AppendFormat("same_price={0},price_status={1},event_start={2},", priceMaster.same_price, priceMaster.price_status, priceMaster.event_start);
                strSql.AppendFormat("event_end={0},price={1},event_price={2},accumulated_bonus = {3}", priceMaster.event_end, priceMaster.price, priceMaster.event_price, priceMaster.accumulated_bonus);
                strSql.AppendFormat(",bonus_percent_start={0},bonus_percent_end={1}", priceMaster.bonus_percent_start, priceMaster.bonus_percent_end);
                strSql.AppendFormat(",cost={0},event_cost={1},child_id={2},max_price={3} ", priceMaster.cost, priceMaster.event_cost, priceMaster.child_id, priceMaster.max_price);
                strSql.AppendFormat(",max_event_price={0},valid_start={1},valid_end={2} ", priceMaster.max_event_price, priceMaster.valid_start, priceMaster.valid_end);
                strSql.AppendFormat(" where writer_id={0} and combo_type={1} and product_id='{2}' and child_id='{3}';", priceMaster.writer_Id, priceMaster.combo_type, priceMaster.product_id, priceMaster.child_id);
                strSql.AppendFormat("set sql_safe_updates = 1;");
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempDao.Update-->" + ex.Message, ex);
            }
        }


        public string Delete(PriceMasterTemp priceMasterTemp)
        {
            try
            {
                StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0;delete from price_master_temp");
                strSql.AppendFormat(" where writer_id={0} and combo_type={1}", priceMasterTemp.writer_Id, priceMasterTemp.combo_type);
                strSql.AppendFormat(" and product_id='{0}';set sql_safe_updates = 1;", priceMasterTemp.product_id);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempDao.Delete-->" + ex.Message, ex);
            }
        }

        public string ChildDelete(PriceMasterTemp priceMasterTemp)
        {
            try
            {
                StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0;delete from price_master_temp");
                strSql.AppendFormat(" where =writer_id{0} and combo_type={1}", priceMasterTemp.writer_Id, priceMasterTemp.combo_type);
                strSql.AppendFormat(" and product_id='{0}' and child_id <> '0';set sql_safe_updates = 1;", priceMasterTemp.product_id );
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempDao.Delete-->" + ex.Message, ex);
            }
        }


        public string Move2PriceMaster(PriceMasterTemp priceMasterTemp)
        {
            try
            {
                StringBuilder stb = new StringBuilder("insert into price_master(`product_id`,`site_id`,`user_level`,`user_id`,`product_name`,`accumulated_bonus`");
                stb.Append(",`bonus_percent`,`default_bonus_percent`,`same_price`,`event_start`,`event_end`,`price_status`,`price`,`event_price`,`child_id`,`cost`,`event_cost`,`bonus_percent_start`,`bonus_percent_end`,`max_price`,`max_event_price`,`valid_start`,`valid_end`) select {0} as product_id,");
                stb.Append("site_id,user_level,user_id,product_name,accumulated_bonus,bonus_percent,default_bonus_percent,same_price,event_start,event_end,price_status,price,event_price,child_id,cost,event_cost,bonus_percent_start,bonus_percent_end,max_price,max_event_price,valid_start,valid_end");
                stb.AppendFormat(" from price_master_temp where writer_id = {0} and combo_type={1}", priceMasterTemp.writer_Id, priceMasterTemp.combo_type);
                stb.AppendFormat(" and product_id='{0}';select @@identity;", priceMasterTemp.product_id);
                return stb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempDao.Move2PriceMaster-->" + ex.Message, ex);
            }
        }

        public string Move2PriceMasterByMasterId()
        {
            try
            {
                StringBuilder stb = new StringBuilder("insert into price_master(`product_id`,`site_id`,`user_level`,`user_id`,`product_name`,`accumulated_bonus`");
                stb.Append(",`bonus_percent`,`default_bonus_percent`,`same_price`,`event_start`,`event_end`,`price_status`,`price`,`event_price`,`child_id`,`cost`,`event_cost`");
                stb.Append(",`bonus_percent_start`,`bonus_percent_end`,`max_price`,`max_event_price`,`valid_start`,`valid_end`) select {0} as product_id,site_id,user_level");
                stb.Append(",user_id,product_name,accumulated_bonus,bonus_percent,default_bonus_percent,same_price,event_start,event_end,price_status,price");
                stb.Append(",event_price,child_id,cost,event_cost,bonus_percent_start,bonus_percent_end,max_price,max_event_price,valid_start,valid_end");
                stb.Append(" from price_master_temp where price_master_id = {1};select @@identity;");
                return stb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempDao.Move2PriceMasterByMasterId-->" + ex.Message, ex);
            }
        }

        public string SelectChild(PriceMasterTemp priceMasterTemp)
        {//edit jialei 20140917 加判斷writerId
            try
            {
                StringBuilder strSql = new StringBuilder("select price_master_id,product_id,child_id from price_master_temp where 1=1");
                if (priceMasterTemp.writer_Id != 0)
                {
                    strSql.AppendFormat(" and writer_id = {0}", priceMasterTemp.writer_Id);
                }
                strSql.AppendFormat(" and combo_type={0} ", priceMasterTemp.combo_type);
                strSql.AppendFormat("and product_id='{0}';", priceMasterTemp.product_id);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempDao.SelectChild-->" + ex.Message, ex);
            }
        }

        public List<PriceMasterTemp> queryChild(PriceMasterTemp query)
        {
            try
            {
                StringBuilder strSql = new StringBuilder("select price_master_id,product_id,child_id from price_master_temp where ");
                strSql.AppendFormat("writer_id = {0} and combo_type={1} ", query.writer_Id, query.combo_type);
                strSql.AppendFormat("and product_id='{0}'", query.product_id);
                return _dbAccess.getDataTableForObj<PriceMasterTemp>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempDao.queryChild-->" + ex.Message, ex);
            }
        }


        public string SaveFromPriceMaster(PriceMasterTemp priceMasterTemp)
        {
            priceMasterTemp.Replace4MySQL();
            try
            {
                StringBuilder strSql = new StringBuilder("insert into price_master_temp(`writer_id`,`product_id`,`site_id`,`user_level`,`user_id`,`product_name`,`accumulated_bonus`");
                strSql.Append(",`bonus_percent`,`default_bonus_percent`,`same_price`,`event_start`,`event_end`,`price_status`,`price`,`event_price`,`child_id`,`combo_type`,`cost`,`event_cost`,`bonus_percent_start`,`bonus_percent_end`,`max_price`,`max_event_price`,valid_start,valid_end) select ");
                strSql.AppendFormat("{0} as writer_id,product_id,site_id,user_level,user_id,'' AS product_name,accumulated_bonus", priceMasterTemp.writer_Id);
                strSql.AppendFormat(",bonus_percent,default_bonus_percent,same_price,event_start,event_end,{0} as price_status,price,event_price,child_id", priceMasterTemp.price_status);
                strSql.AppendFormat(",{0} as combo_type,cost,event_cost,bonus_percent_start,bonus_percent_end,max_price,max_event_price,valid_start,valid_end from price_master where product_id='{1}' and site_id=1 and user_level=1 and user_id=0", priceMasterTemp.combo_type, priceMasterTemp.product_id);
                return strSql.ToString();
                ///ediy by wwei0216w  2015/8/27
                /*
                 修改代碼 '' AS product_name
                 * 修改原因:
                 * 商品名稱添加前後綴后,複製商品時,價格名稱部份會連前後綴一起複製,導致商品沒有前後綴的添加記錄,為避免該問題
                 * 複製商品時,價格名稱部份將不會再進行複製
                 */
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempDao.SaveFromPriceMaster-->" + ex.Message, ex);
            }
        }

        public string SaveFromPriceMasterByMasterId(PriceMasterTemp priceMasterTemp)
        {
            try
            {
                StringBuilder stb = new StringBuilder("insert into price_master_temp(`writer_id`,`combo_type`,`product_id`,`site_id`,`user_level`,`user_id`,`product_name`,`accumulated_bonus`");
                stb.Append(",`bonus_percent`,`default_bonus_percent`,`same_price`,`event_start`,`event_end`,`price_status`,`price`,`event_price`,`child_id`,`cost`,`event_cost`,`bonus_percent_start`,`bonus_percent_end`,`max_price`,`max_event_price`) ");
                stb.AppendFormat("select {0} as writer_id,{1} as combo_type, product_id,", priceMasterTemp.writer_Id, priceMasterTemp.combo_type);
                stb.Append("site_id,user_level,user_id,'' AS product_name,accumulated_bonus,bonus_percent,default_bonus_percent,same_price,event_start,event_end,price_status,price,event_price,child_id,cost,event_cost,bonus_percent_start,bonus_percent_end,max_price,max_event_price");
                stb.Append(" from price_master where price_master_id = {0};select @@identity;");
                return stb.ToString();
                ///ediy by wwei0216w  2015/8/27
                /*
                 修改代碼 '' AS product_name
                 * 修改原因:
                 * 商品名稱添加前後綴后,複製商品時,價格名稱部份會連前後綴一起複製,導致商品沒有前後綴的添加記錄,為避免該問題
                 * 複製商品時,價格名稱部份將不會再進行複製
                 */
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempDao.SaveFromPriceMasterByMasterId-->" + ex.Message, ex);
            }
        }

        public bool Save(ArrayList priceMasters, ArrayList itemPriceSqls, ArrayList otherSqls)
        {
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            try
            {
                if (priceMasters == null)
                {
                    return false;
                }
                if (itemPriceSqls != null && itemPriceSqls.Count > 0 && priceMasters.Count != itemPriceSqls.Count)
                {
                    return false;
                }
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;

                foreach (var item in priceMasters)
                {
                    mySqlCmd.CommandText = item.ToString();
                    int rowId = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                    if (itemPriceSqls != null && itemPriceSqls.Count > 0)
                    {
                        ArrayList prices = (ArrayList)itemPriceSqls[priceMasters.IndexOf(item)];
                        if (prices != null)
                        {
                            foreach (var price in prices)
                            {
                                mySqlCmd.CommandText = string.Format(price.ToString(), rowId);
                                mySqlCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                if (otherSqls != null)
                {
                    foreach (var item in otherSqls)
                    {
                        mySqlCmd.CommandText = item.ToString();
                        mySqlCmd.ExecuteNonQuery();
                    }
                }
                mySqlCmd.Transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PriceMasterTempDao.Save(ArrayList priceMasters, ArrayList itemPriceSqls, ArrayList otherSqls)-->" + ex.Message, ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
        }

        #endregion
        #region 供應商商品數據處理


        #region 事物刪除數據 返回sql語句 +string DeleteByVendor(PriceMasterTemp priceMasterTemp)
        public string DeleteByVendor(PriceMasterTemp priceMasterTemp)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("set sql_safe_updates = 0;delete from price_master_temp ");

                strSql.AppendFormat(" where  writer_id={0} ", priceMasterTemp.writer_Id);

                strSql.AppendFormat(" and combo_type={0}", priceMasterTemp.combo_type);

                if (!string.IsNullOrEmpty(priceMasterTemp.product_id) && priceMasterTemp.product_id != "0")
                {
                    strSql.AppendFormat("  and product_id='{0}' ;", priceMasterTemp.product_id);
                }
                strSql.Append("set sql_safe_updates = 1;");
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempDao.DeleteByVendor-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        #region 根據PriceMasterTempmodel對象查詢數據 +PriceMasterProductCustomTemp QueryByVendor(PriceMasterTemp priceMasterTemp)
        public Model.Custom.PriceMasterProductCustomTemp QueryByVendor(PriceMasterTemp priceMasterTemp)
        {
            priceMasterTemp.Replace4MySQL();
            StringBuilder stb = new StringBuilder();
            try
            {

                stb.Append("select a.price_type,a.product_price_list,b.product_name,b.default_bonus_percent,b.bonus_percent,a.bag_check_money,b.accumulated_bonus,b.bonus_percent_start,b.bonus_percent_end,");
                stb.Append(" b.price,b.event_price,b.cost,b.event_cost,b.event_start,b.event_end,b.same_price,b.valid_start,b.valid_end,a.show_listprice,a.product_mode from product_temp a right join ");
                stb.AppendFormat("price_master_temp b on a.writer_id=b.writer_id and a.combo_type=b.combo_type and a.product_id=b.product_id and a.combo_type={0} where a.writer_id={1}", priceMasterTemp.combo_type, priceMasterTemp.writer_Id);

                if (!string.IsNullOrEmpty(priceMasterTemp.product_id) && priceMasterTemp.product_id != "0")
                {
                    stb.AppendFormat(" and b.product_id='{0}' ", priceMasterTemp.product_id);

                }
                if (!string.IsNullOrEmpty(priceMasterTemp.child_id) && priceMasterTemp.child_id != "0")
                {
                    stb.AppendFormat(" and b.child_id='{0}' ;", priceMasterTemp.child_id);

                }

                return _dbAccess.getSinggleObj<Model.Custom.PriceMasterProductCustomTemp>(stb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempDao.QueryByVendor-->" + ex.Message + stb.ToString(), ex);
            }
        }
        #endregion

        #region 供應商商品價格保存 返回sql語句 + string SaveByVendor(PriceMasterTemp priceMaster)
        public string SaveByVendor(PriceMasterTemp priceMaster)
        {
            priceMaster.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("");
            try
            {
                strSql.Append("insert into price_master_temp(`writer_id`,`product_id`,`site_id`,`user_level`,`user_id`,`product_name`,`bonus_percent`,`default_bonus_percent`,");
                strSql.Append("`same_price`,`event_start`,`event_end`,`price_status`,`price`,`event_price`,`child_id`,`combo_type`,`cost`,`event_cost`,`accumulated_bonus`,`bonus_percent_start`");
                strSql.AppendFormat(",`bonus_percent_end`,`max_price`,`max_event_price`,`valid_start`,`valid_end`)values({0},'{1}',{2},{3}", priceMaster.writer_Id, string.IsNullOrEmpty(priceMaster.product_id) ? priceMaster.product_id.ToString() : priceMaster.product_id, priceMaster.site_id, priceMaster.user_level);
                strSql.AppendFormat(",{0},'{1}',{2},{3},{4},", priceMaster.user_id, priceMaster.product_name, priceMaster.bonus_percent, priceMaster.default_bonus_percent, priceMaster.same_price);
                strSql.AppendFormat("{0},{1},{2},{3},", priceMaster.event_start, priceMaster.event_end, priceMaster.price_status, priceMaster.price);
                strSql.AppendFormat("{0},'{1}',{2},{3},{4},{5},", priceMaster.event_price, priceMaster.child_id, priceMaster.combo_type, priceMaster.cost, priceMaster.event_cost, priceMaster.accumulated_bonus);
                strSql.AppendFormat("{0},{1},{2},{3},{4},{5});select @@identity;", priceMaster.bonus_percent_start, priceMaster.bonus_percent_end, priceMaster.max_price, priceMaster.max_event_price, priceMaster.valid_start, priceMaster.valid_end);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempDao.SaveByVendor-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion
        #region 供應商商品價格編輯 返回sql語句 + string UpdateByVendor(PriceMasterTemp priceMaster)
        public string UpdateByVendor(PriceMasterTemp priceMaster)
        {
            priceMaster.Replace4MySQL();
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("set sql_safe_updates=0;update price_master_temp set ");
                strSql.AppendFormat(" writer_id={0},site_id={1},user_level={2},user_id={3},", priceMaster.writer_Id, priceMaster.site_id, priceMaster.user_level, priceMaster.user_id);
                strSql.AppendFormat("product_name='{0}',bonus_percent={1},default_bonus_percent={2},", priceMaster.product_name, priceMaster.bonus_percent, priceMaster.default_bonus_percent);
                strSql.AppendFormat("same_price={0},price_status={1},event_start={2},", priceMaster.same_price, priceMaster.price_status, priceMaster.event_start);
                strSql.AppendFormat("event_end={0},price={1},event_price={2},accumulated_bonus = {3}", priceMaster.event_end, priceMaster.price, priceMaster.event_price, priceMaster.accumulated_bonus);
                strSql.AppendFormat(",bonus_percent_start={0},bonus_percent_end={1}", priceMaster.bonus_percent_start, priceMaster.bonus_percent_end);
                strSql.AppendFormat(",cost={0},event_cost={1},child_id='{2}',max_price={3} ", priceMaster.cost, priceMaster.event_cost, priceMaster.child_id, priceMaster.max_price);
                strSql.AppendFormat(",max_event_price={0},valid_start={1},valid_end={2} ", priceMaster.max_event_price, priceMaster.valid_start, priceMaster.valid_end);
                strSql.AppendFormat(" where writer_id={0} and combo_type={1}  ", priceMaster.writer_Id, priceMaster.combo_type);
                if (!string.IsNullOrEmpty(priceMaster.child_id) && priceMaster.child_id != "0")
                {
                    strSql.AppendFormat(" and child_id ='{0}' ", priceMaster.child_id);
                }
                if (!string.IsNullOrEmpty(priceMaster.product_id) && priceMaster.product_id != "0")
                {
                    strSql.AppendFormat(" and product_id ='{0}' ", priceMaster.product_id);
                }
                if (priceMaster.price_master_id != 0)
                {
                    strSql.AppendFormat(" and price_master_id='{0}'", priceMaster.price_master_id);
                }
                strSql.AppendFormat(";set sql_safe_updates = 1;");
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempDao.UpdateByVendor-->" + ex.Message + strSql.ToString(), ex);
            }
        }

        #endregion
        #region 查詢組合商品中臨時表中單一商品信息 List<Model.Custom.SingleProductPriceTemp> SingleProductPriceQueryByVendor(string product_id, int pile_id)

        //查詢組合商品中單一商品信息
        public List<Model.Custom.SingleProductPriceTemp> SingleProductPriceQueryByVendor(string product_id, int pile_id)
        {
            StringBuilder stb = new StringBuilder();
            try
            {
                //子商品為正式數據
                StringBuilder proSql = new StringBuilder();
                proSql.Append(" select a.product_id ,b.product_name,b.prod_sz, min(a.item_money) as price, ");
                proSql.Append(" c.s_must_buy,c.g_must_buy ");
                proSql.Append(" from product_item a ");
                proSql.Append(" left join product b on a.product_id=b.product_id ");
                proSql.AppendFormat(" left join product_combo_temp c  on a.product_id = c.child_id and parent_id='{0}' ", product_id);
                proSql.AppendFormat(" where a.product_id in (select child_id from product_combo_temp where parent_id='{0}' and pile_id={1}) group by a.product_id ", product_id, pile_id);
                //子商品為臨時數據
                StringBuilder proTempSql = new StringBuilder();
                proTempSql.Append(" select a.product_id ,b.product_name, min(a.item_money) as price, ");
                proTempSql.Append(" c.s_must_buy,c.g_must_buy ");
                proTempSql.Append(" from product_item_temp a ");
                proTempSql.Append(" left join product_temp b on a.product_id=b.product_id ");
                proTempSql.AppendFormat(" left join product_combo_temp c  on a.product_id = c.child_id and parent_id='{0}' ", product_id);
                proTempSql.AppendFormat(" where a.product_id in (select child_id from product_combo_temp where parent_id='{0}' and pile_id={1}) group by a.product_id ", product_id, pile_id);

                stb.Append(" select product_id,product_name,price,s_must_buy,g_must_buy ");
                stb.AppendFormat(" from (({0}) UNION ({1})) singlePro", proSql, proTempSql);
                stb.Append(" where  singlePro.product_id is not NULL; ");

                return _dbAccess.getDataTableForObj<Model.Custom.SingleProductPriceTemp>(stb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.SingleProductPriceQueryByVendor-->" + ex.Message + stb.ToString(), ex);
            }
        }
        #endregion
        public string VendorSaveFromPriceMaster(PriceMasterTemp priceMasterTemp, string old_product_id)
        {//20140905 供應商複製
            priceMasterTemp.Replace4MySQL();
            try
            {
                StringBuilder strSql = new StringBuilder("insert into price_master_temp(`writer_id`,`product_id`,`site_id`,`user_level`,`user_id`,`product_name`,`accumulated_bonus`");
                strSql.Append(",`bonus_percent`,`default_bonus_percent`,`same_price`,`event_start`,`event_end`,`price_status`,`price`,`event_price`,`child_id`,`combo_type`,`cost`,`event_cost`,`bonus_percent_start`,`bonus_percent_end`,`max_price`,`max_event_price`,`valid_start`,`valid_end`) select ");
                strSql.AppendFormat("{0} as writer_id,'{1}' as product_id,site_id,user_level,user_id,product_name,accumulated_bonus", priceMasterTemp.writer_Id, priceMasterTemp.product_id);
                strSql.AppendFormat(",bonus_percent,default_bonus_percent,same_price,event_start,event_end,{0} as price_status,price,event_price, '{1}' as  child_id", priceMasterTemp.price_status, priceMasterTemp.product_id);
                strSql.AppendFormat(",{0} as combo_type,cost,event_cost,bonus_percent_start,bonus_percent_end,max_price,max_event_price,valid_start,valid_end ", priceMasterTemp.combo_type);
                uint productid = 0;
                if (uint.TryParse(old_product_id, out productid))
                {
                    strSql.AppendFormat(" from price_master where product_id={0} ", old_product_id);
                }
                else
                {
                    strSql.AppendFormat(" from price_master_temp where product_id='{0}' ", old_product_id);
                }
                strSql.Append(" and site_id=1 and user_level=1 and user_id=0 ;");
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempDao.VendorSaveFromPriceMaster-->" + ex.Message, ex);
            }
        }

        #region 獲取供應商修改商品時的站台價格List<Model.Custom.PriceMasterCustom> QueryProdSiteByVendor(PriceMasterTemp priceMasterTemp)
        public List<Model.Custom.PriceMasterCustom> QueryProdSiteByVendor(PriceMasterTemp priceMasterTemp)
        {
            priceMasterTemp.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("");
            try
            {
                strSql.Append("select price_master_id,accumulated_bonus,product_id as vendor_product_id,site.site_id,site.site_name,pmt.user_level,t.parametername as user_level_name,pmt.user_id,product_name,bonus_percent,default_bonus_percent,");
                strSql.Append(" same_price,accumulated_bonus,event_start,event_end,price_status,price,event_price,cost,event_cost,child_id ,pmt.user_id,user_email,s.parametername as status,bonus_percent_start,bonus_percent_end,max_price,max_event_price,valid_start,valid_end from price_master_temp pmt  ");
                strSql.Append(" left join site on site.site_id=pmt.site_id left join users on users.user_id=pmt.user_id ");
                strSql.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='userlevel') t on t.parametercode=pmt.user_level");
                strSql.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='price_status') s on s.parametercode=pmt.price_status where 1=1");
                if (!string.IsNullOrEmpty(priceMasterTemp.product_id) && priceMasterTemp.product_id != "0")
                {
                    strSql.AppendFormat(" and product_id='{0}'", priceMasterTemp.product_id);
                }
                if (priceMasterTemp.user_id != 0)
                {
                    strSql.AppendFormat(" and pmt.user_id={0}", priceMasterTemp.user_id);
                }
                if (priceMasterTemp.user_level != 0)
                {
                    strSql.AppendFormat(" and pmt.user_level={0}", priceMasterTemp.user_level);
                }
                if (priceMasterTemp.site_id != 0)
                {
                    strSql.AppendFormat(" and site.site_id={0}", priceMasterTemp.site_id);
                }
                if (priceMasterTemp.price_master_id != 0)
                {
                    strSql.AppendFormat(" and price_master_id={0}", priceMasterTemp.price_master_id);
                }
                if (!string.IsNullOrEmpty(priceMasterTemp.child_id) && priceMasterTemp.child_id != "0")
                {
                    strSql.AppendFormat(" and child_id='{0}'", priceMasterTemp.child_id);
                }

                return _dbAccess.getDataTableForObj<Model.Custom.PriceMasterCustom>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempDao.QueryProdSiteByVendor-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        //修改站台價格
        public string UpdateTs(Model.Custom.PriceMasterCustom pM)
        {
            pM.Replace4MySQL();
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat("update price_master_temp set product_id='{0}',site_id={1},user_level={2},user_id={3},", pM.vendor_product_id, pM.site_id, pM.user_level, pM.user_id);
                strSql.AppendFormat("product_name='{0}',bonus_percent={1},default_bonus_percent={2},", pM.product_name, pM.bonus_percent, pM.default_bonus_percent);
                strSql.AppendFormat("same_price={0},accumulated_bonus={1},price_status={2},event_start={3},", pM.same_price, pM.accumulated_bonus, pM.price_status, pM.event_start);
                strSql.AppendFormat("event_end={0},price = {1},event_price = {2},cost={3},event_cost={4}", pM.event_end, pM.price, pM.event_price, pM.cost, pM.event_cost);
                strSql.AppendFormat(",bonus_percent_start={0},bonus_percent_end={1}", pM.bonus_percent_start, pM.bonus_percent_end);
                strSql.AppendFormat(",child_id='{0}',max_price={1},max_event_price={2},valid_start={3},valid_end={4} ", pM.child_id, pM.max_price, pM.max_event_price, pM.valid_start, pM.valid_end);
                strSql.AppendFormat(" where price_master_id={0}", pM.price_master_id);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempDao.UpdateTs-->" + ex.Message + strSql.ToString(), ex);
            }
        }

        /// <summary>
        /// 管理員核可供應商建立的商品時將商品價格主檔信息由臨時表移動除
        /// </summary>
        /// <param name="priceMasterTemp">臨時表中的數據對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_Delete(PriceMasterTemp priceMasterTemp)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates = 0;delete from price_master_temp where 1=1");
            if (priceMasterTemp.writer_Id != 0)
            {
                strSql.AppendFormat(" and writer_id={0}", priceMasterTemp.writer_Id);
            }
            strSql.AppendFormat(" and product_id='{0}' and combo_type={1};set sql_safe_updates = 1;", priceMasterTemp.product_id, priceMasterTemp.combo_type);
            return strSql.ToString();
        }


        public string VendorMove2PriceMaster(PriceMasterTemp priceMasterTemp)
        {
            try
            {
                StringBuilder stb = new StringBuilder("insert into price_master(`product_id`,`site_id`,`user_level`,`user_id`,`product_name`,`accumulated_bonus`");
                stb.Append(",`bonus_percent`,`default_bonus_percent`,`same_price`,`event_start`,`event_end`,`price_status`,`price`,`event_price`,`cost`,`event_cost`,`bonus_percent_start`,`bonus_percent_end`,`max_price`,`max_event_price`,`valid_start`,`valid_end`,`child_id`) select {0} as product_id,");
                stb.Append("site_id,user_level,user_id,product_name,accumulated_bonus,bonus_percent,default_bonus_percent,same_price,event_start,event_end,price_status,price,event_price,cost,event_cost,bonus_percent_start,bonus_percent_end,max_price,max_event_price,valid_start,valid_end,");

                if (priceMasterTemp.combo_type == 2)
                {
                    stb.Append(" {0} as child_id");
                }
                else if (priceMasterTemp.combo_type == 1)
                {
                    stb.Append(" 0 as child_id");
                }


                stb.Append(" from price_master_temp where 1=1");

                if (priceMasterTemp.writer_Id != 0)
                {
                    stb.AppendFormat(" and writer_id = {0}", priceMasterTemp.writer_Id);
                }
                stb.AppendFormat(" and combo_type={0}", priceMasterTemp.combo_type);
                stb.AppendFormat(" and product_id='{0}';select @@identity;", priceMasterTemp.product_id);
                return stb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempDao.VendorMove2PriceMaster-->" + ex.Message, ex);
            }
        }
        #endregion
    }
}
