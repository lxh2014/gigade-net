/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductSiteDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 10:04:00 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Dao.Impl;
using MySql.Data.MySqlClient;
using BLL.gigade.Model;
using System.Text.RegularExpressions;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Dao
{
    public class PriceMasterDao : IPriceMasterImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public PriceMasterDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }
        #region IPriceMasterImplDao 成员

        public List<Model.Custom.PriceMasterCustom> Query(Model.PriceMaster priceMaster)
        {
            priceMaster.Replace4MySQL();
            try
            {
                StringBuilder strSql = new StringBuilder("select price_master_id,accumulated_bonus,product_id,site.site_id,site.site_name,price_master.user_level,price_master.user_level as user_level_name,price_master.user_id,product_name,bonus_percent,default_bonus_percent,");
                strSql.Append("same_price,accumulated_bonus,event_start,event_end,price_status,price,event_price,cost,event_cost,child_id,price_master.user_id,user_email,price_master.price_status,price_master.price_status as status,bonus_percent_start,bonus_percent_end,max_price,max_event_price,valid_start,valid_end from price_master ");
                strSql.Append(" left join site on site.site_id=price_master.site_id left join users on users.user_id=price_master.user_id where 1=1 ");
                //strSql.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='userlevel') t on t.parametercode=price_master.user_level");
                //strSql.Append(" left join (select parametercode,parametername from t_parametersrc where parametertype='price_status') s on s.parametercode=price_master.price_status where 1=1");
                if (priceMaster.product_id != 0)
                {
                    strSql.AppendFormat(" and product_id={0}", priceMaster.product_id);
                }
                if (priceMaster.user_id != 0)
                {
                    strSql.AppendFormat(" and price_master.user_id={0}", priceMaster.user_id);
                }
                if (priceMaster.user_level != 0)
                {
                    strSql.AppendFormat(" and price_master.user_level={0}", priceMaster.user_level);
                }
                if (priceMaster.site_id != 0)
                {
                    strSql.AppendFormat(" and site.site_id={0}", priceMaster.site_id);
                }
                if (priceMaster.price_master_id != 0)
                {
                    strSql.AppendFormat(" and price_master_id={0}", priceMaster.price_master_id);
                }
                else
                {
                    strSql.AppendFormat(" and child_id={0}", priceMaster.child_id);
                }
                //edit by zhuoqin0830w  2015/05/18
                IParametersrcImplDao _parameterDao = new ParametersrcDao(connStr);
                List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("userlevel", "price_status");
                List<PriceMasterCustom> list = _dbAccess.getDataTableForObj<PriceMasterCustom>(strSql.ToString());
                foreach (PriceMasterCustom q in list)
                {
                    var alist = parameterList.Find(m => m.ParameterType == "UserLevel" && m.ParameterCode == q.user_level.ToString());
                    var blist = parameterList.Find(m => m.ParameterType == "price_status" && m.ParameterCode == q.price_status.ToString());
                    if (alist != null)
                    {
                        q.user_level_name = alist.parameterName;
                    }
                    if (blist != null)
                    {
                        q.status = blist.parameterName;
                    }
                }

                return list;

                //return _dbAccess.getDataTableForObj<Model.Custom.PriceMasterCustom>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.Query-->" + ex.Message, ex);
            }
        }

        //add by wwei 0216w 2014/12/12
        //添加根據price_master_id進行查詢的方法
        public List<Model.PriceMaster> Query(uint[] priceMasterIds)
        {
            try
            {
                string master_Ids = "";
                foreach (uint price_master_id in priceMasterIds)
                {
                    master_Ids += price_master_id.ToString() + ",";
                }
                master_Ids = master_Ids.Length > 0 ? master_Ids.Remove(master_Ids.Length - 1, 1) : "0";
                StringBuilder strSql = new StringBuilder();
                strSql.AppendFormat(@"SELECT pm.price_master_id,pm.product_id,pm.site_id,pm.user_level,pm.user_id,pm.product_name,pm.accumulated_bonus,pm.bonus_percent,pm.default_bonus_percent,
pm.bonus_percent_start,pm.bonus_percent_end,pm.same_price,pm.event_start,pm.event_end,pm.price_status,pm.price,pm.event_price,pm.child_id,pm.apply_id,pm.cost,
pm.event_cost,pm.max_price,pm.max_event_price,pm.valid_start,pm.valid_end 
FROM price_master pm WHERE pm.price_master_id IN({0})", master_Ids);
                return _dbAccess.getDataTableForObj<Model.PriceMaster>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.Query-->" + ex.Message, ex);
            }
        }

        public Model.PriceMaster QueryPriceMaster(Model.PriceMaster pM)
        {
            pM.Replace4MySQL();
            try
            {
                StringBuilder stb = new StringBuilder("select price_master_id,product_id,site_id,user_level,user_id,");
                stb.Append("product_name,bonus_percent,default_bonus_percent,same_price,accumulated_bonus,event_start,event_end,");
                stb.Append("price_status,price,event_price,cost,event_cost,child_id,apply_id,bonus_percent_start,bonus_percent_end,max_price,max_event_price,valid_start,valid_end ");
                stb.Append(" from price_master where 1=1");
                if (pM.price_master_id != 0)
                {
                    stb.AppendFormat(" and price_master_id={0}", pM.price_master_id);
                }
                else
                {
                    stb.AppendFormat(" and child_id={0}", pM.child_id);
                }
                if (pM.product_id != 0)
                {
                    stb.AppendFormat(" and product_id={0}", pM.product_id);
                }
                return _dbAccess.getSinggleObj<Model.PriceMaster>(stb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.QueryPriceMaster-->" + ex.Message, ex);
            }
        }

        public List<Model.PriceMaster> PriceMasterQuery(Model.PriceMaster query)
        {
            query.Replace4MySQL();
            try
            {
                StringBuilder stb = new StringBuilder("select price_master_id,product_id,site_id,user_level,user_id,");
                stb.Append("product_name,bonus_percent,default_bonus_percent,same_price,event_start,event_end,");
                stb.Append("price_status,price,event_price,cost,event_cost,child_id,apply_id,bonus_percent_start,bonus_percent_end,max_price,max_event_price,valid_start,valid_end ");
                stb.Append(" from price_master where 1=1");
                if (query.price_master_id != 0)
                {
                    stb.AppendFormat(" and price_master_id={0}", query.price_master_id);
                }
                else
                {
                    stb.AppendFormat(" and child_id={0}", query.child_id);
                }
                if (query.product_id != 0)
                {
                    stb.AppendFormat(" and product_id={0}", query.product_id);
                }
                if (query.site_id != 0)
                {
                    stb.AppendFormat(" and site_id = {0}", query.site_id);
                }
                if (query.price_status != 0)
                {
                    stb.AppendFormat(" and price_status = {0}", query.price_status);
                }
                return _dbAccess.getDataTableForObj<Model.PriceMaster>(stb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.PriceMasterQuery(Model.PriceMaster query)-->" + ex.Message, ex);
            }

        }

        /// <summary>
        /// 查詢自各定價
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<PriceMaster> QuerySelf(PriceMaster query)
        {
            string sqlStr = string.Format(@"select * from price_master pm1 inner join price_master pm2 
on pm1.product_id =pm2.product_id and pm1.site_id=pm2.site_id and pm1.user_level=pm2.user_level and pm1.user_id=pm2.user_id
where pm2.price_master_id={0}", query.price_master_id);
            return _dbAccess.getDataTableForObj<Model.PriceMaster>(sqlStr);
        }

        //add by xiangwang0413w 2014/07/02
        public List<Model.Custom.PriceMasterCustom> QuerySitePriceOption(uint proId, uint channelId)
        {
            try
            {
                //left join t_parametersrc c on a.user_level=c.parameterCode and parameterType='UserLevel'
                string strSql = string.Format(@"select distinct a.price_master_id,concat(!isnull(e.price_master_id) ,b.site_name) as site_name,a.user_level,
                    a.user_level as user_level_name,a.user_id,d.user_email from price_master a 
                    left join users d on a.user_id=d.user_id left join site b on a.site_id=b.site_id
                    left join product_item_map e on a.price_master_id=e.price_master_id and e.channel_id={0}
                    where a.product_id={1} order by e.price_master_id desc ;", channelId, proId);

                //edit by zhuoqin0830w  2015/05/18
                IParametersrcImplDao _parameterDao = new ParametersrcDao(connStr);
                List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("UserLevel");
                List<PriceMasterCustom> list = _dbAccess.getDataTableForObj<Model.Custom.PriceMasterCustom>(strSql);
                foreach (PriceMasterCustom q in list)
                {
                    var alist = parameterList.Find(m => m.ParameterType == "UserLevel" && m.ParameterCode == q.user_level.ToString());
                    if (alist != null)
                    {
                        q.user_level_name = alist.parameterName;
                    }
                }

                return list;

                //return _dbAccess.getDataTableForObj<Model.Custom.PriceMasterCustom>(strSql);
            }
            catch (Exception ex)
            {
                throw new Exception("QuerySitePriceOption(uint proId)-->" + ex.Message, ex);
            }
        }

        //add by hufeng0813w 2014/06/12
        public List<Model.PriceMaster> PriceMasterQueryByid(Model.PriceMaster query)
        {
            query.Replace4MySQL();
            try
            {
                StringBuilder stb = new StringBuilder("select price_master_id,product_id,site_id,user_level,user_id,");
                stb.Append("product_name,bonus_percent,default_bonus_percent,same_price,event_start,event_end,");
                stb.Append("price_status,price,event_price,cost,event_cost,child_id,apply_id,bonus_percent_start,bonus_percent_end,max_price,max_event_price,valid_start,valid_end ");
                stb.Append(" from price_master where 1=1");
                if (query.product_id != 0)
                {
                    stb.AppendFormat(" and product_id={0}", query.product_id);
                }
                if (query.site_id != 0)
                {
                    stb.AppendFormat(" and site_id = {0}", query.site_id);
                }
                if (query.user_level != 0)
                {
                    //stb.AppendFormat(" and user_level = {0}", query.price_status);
                    stb.AppendFormat(" and user_level = {0}", query.user_level);
                }
                if (query.user_id != 0)
                {
                    stb.AppendFormat(" and user_id={0}", query.user_id);
                }
                return _dbAccess.getDataTableForObj<Model.PriceMaster>(stb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.PriceMasterQuery(Model.PriceMaster query)-->" + ex.Message, ex);
            }

        }

        //不要在此方法上修改對查詢條件進行默認值判斷
        public Model.PriceMaster QueryPMaster(Model.PriceMaster pM)
        {
            pM.Replace4MySQL();
            try
            {
                StringBuilder stb = new StringBuilder("select price_master_id from price_master where 1=1 ");
                stb.AppendFormat(" and price_master_id<>{0} and user_id={1} and user_level={2} and site_id={3} and product_id={4}", pM.price_master_id, pM.user_id, pM.user_level, pM.site_id, pM.product_id);
                stb.AppendFormat(" and child_id={0}", pM.child_id);
                return _dbAccess.getSinggleObj<Model.PriceMaster>(stb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.QueryPMaster-->" + ex.Message, ex);
            }
        }

        public string Save(Model.PriceMaster priceMaster)
        {
            priceMaster.Replace4MySQL();
            try
            {
                StringBuilder strSql = new StringBuilder("insert into price_master(`product_id`,`site_id`,`user_level`,`user_id`,`product_name`,`accumulated_bonus`,");
                strSql.Append("`bonus_percent`,`default_bonus_percent`,`same_price`,`event_start`,`event_end`,`price_status`,`price`,`event_price`,`child_id`,`apply_id`,`cost`,`event_cost`,`bonus_percent_start`,`bonus_percent_end`,`max_price`,`max_event_price`,`valid_start`,`valid_end`)");
                strSql.AppendFormat("values({0},{1},{2},{3}", priceMaster.product_id, priceMaster.site_id, priceMaster.user_level, priceMaster.user_id);
                strSql.AppendFormat(",'{0}',{1},{2},{3}", priceMaster.product_name, priceMaster.accumulated_bonus, priceMaster.bonus_percent, priceMaster.default_bonus_percent);
                strSql.AppendFormat(",{0},{1},{2},{3},{4}", priceMaster.same_price, priceMaster.event_start, priceMaster.event_end, priceMaster.price_status, priceMaster.price);
                strSql.AppendFormat(",{0},{1},{2},{3},{4}", priceMaster.event_price, priceMaster.child_id, priceMaster.apply_id, priceMaster.cost, priceMaster.event_cost);
                strSql.AppendFormat(",{0},{1},{2},{3},{4},{5});select @@identity;", priceMaster.bonus_percent_start, priceMaster.bonus_percent_end, priceMaster.max_price, priceMaster.max_event_price, priceMaster.valid_start, priceMaster.valid_end);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.Save(Model.PriceMaster priceMaster)-->" + ex.Message, ex);
            }
        }

        public string SaveNoProId(Model.PriceMaster priceMaster)
        {
            priceMaster.Replace4MySQL();
            try
            {
                StringBuilder strSql = new StringBuilder("insert into price_master(`product_id`,`site_id`,`user_level`,`user_id`,`product_name`,`accumulated_bonus`,");
                strSql.Append("`bonus_percent`,`default_bonus_percent`,`same_price`,`event_start`,`event_end`,`price_status`,`price`,`event_price`,`child_id`,`apply_id`,`cost`,`event_cost`,`bonus_percent_start`,`bonus_percent_end`,`max_price`,`max_event_price`,`valid_start`,`valid_end`)values({0},");
                strSql.AppendFormat("{0},{1},{2}", priceMaster.site_id, priceMaster.user_level, priceMaster.user_id);
                strSql.AppendFormat(",'{0}',{1},{2},{3}", priceMaster.product_name, priceMaster.accumulated_bonus, priceMaster.bonus_percent, priceMaster.default_bonus_percent);
                strSql.AppendFormat(",{0},{1},{2},{3},{4}", priceMaster.same_price, priceMaster.event_start, priceMaster.event_end, priceMaster.price_status, priceMaster.price);
                strSql.AppendFormat(",{0},{1},{2},{3},{4}", priceMaster.event_price, priceMaster.child_id, priceMaster.apply_id, priceMaster.cost, priceMaster.event_cost);
                strSql.AppendFormat(",{0},{1},{2},{3},{4},{5});select @@identity;", priceMaster.bonus_percent_start, priceMaster.bonus_percent_end, priceMaster.max_price, priceMaster.max_event_price, priceMaster.valid_start, priceMaster.valid_end);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.SaveNoProId(Model.PriceMaster priceMaster)-->" + ex.Message, ex);
            }
        }

        public string Update(Model.PriceMaster pM)
        {
            pM.Replace4MySQL();
            try
            {
                StringBuilder strSql = new StringBuilder("update price_master set ");
                strSql.AppendFormat(" product_id={0},site_id={1},user_level={2},user_id={3},", pM.product_id, pM.site_id, pM.user_level, pM.user_id);
                strSql.AppendFormat("product_name='{0}',bonus_percent={1},default_bonus_percent={2},", pM.product_name, pM.bonus_percent, pM.default_bonus_percent);
                strSql.AppendFormat("same_price={0},accumulated_bonus={1},price_status={2},event_start={3},", pM.same_price, pM.accumulated_bonus, pM.price_status, pM.event_start);
                strSql.AppendFormat("event_end={0},price = {1},event_price = {2},cost={3},event_cost={4}", pM.event_end, pM.price, pM.event_price, pM.cost, pM.event_cost);
                strSql.AppendFormat(",bonus_percent_start={0},bonus_percent_end={1}", pM.bonus_percent_start, pM.bonus_percent_end);
                strSql.AppendFormat(",child_id={0},apply_id={1},max_price={2},max_event_price={3},valid_start={4},valid_end={5} ", pM.child_id, pM.apply_id, pM.max_price, pM.max_event_price, pM.valid_start, pM.valid_end);
                strSql.AppendFormat(" where price_master_id={0};", pM.price_master_id);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.Update-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 根據條件只修改product_name
        /// edit by wwei0216w 2014/12/18
        /// </summary>
        /// <returns></returns>
        public string UpdateName(PriceMaster pm)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("UPDATE price_master SET product_name = '{0}' WHERE price_master_id = {1};", pm.product_name, pm.price_master_id);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.UpdateName-->" + ex.Message, ex);
            }
        }

        public string SelectChild(PriceMaster priceMaster)
        {
            try
            {
                StringBuilder strSql = new StringBuilder("select price_master_id,product_id,child_id from price_master where ");
                strSql.AppendFormat("product_id = {0} and site_id=1 and user_level=1 and user_id=0 ", priceMaster.product_id);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.SelectChild-->" + ex.Message, ex);
            }
        }

        public string DeleteByProductId(int product_Id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder("set sql_safe_updates=0;delete from price_master");
                strSql.AppendFormat(" where price_master.product_id={0};", product_Id);
                strSql.Append("set sql_safe_updates=1;");
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.DeleteByProductId-->" + ex.Message, ex);
            }
        }

        public int Save(string priceMasterSql, System.Collections.ArrayList itemPriceSqls, System.Collections.ArrayList otherSqls)
        {
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

                mySqlCmd.CommandText = priceMasterSql;
                int rowId = Convert.ToInt32(mySqlCmd.ExecuteScalar());

                if (itemPriceSqls != null)
                {
                    foreach (var item in itemPriceSqls)
                    {
                        mySqlCmd.CommandText = string.Format(item.ToString(), rowId);
                        mySqlCmd.ExecuteNonQuery();
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
                return rowId;
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PriceMasterDao.Save(string priceMasterSql, System.Collections.ArrayList itemPriceSqls, System.Collections.ArrayList otherSqls)-->" + ex.Message, ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
        }

        //查詢組合商品中單一商品信息
        public List<Model.Custom.SingleProductPrice> SingleProductPriceQuery(uint product_id, int pile_id)
        {
            try
            {
                StringBuilder stb = new StringBuilder("select a.product_id,b.product_name,b.prod_sz, min(a.item_money) as price,c.s_must_buy,c.g_must_buy from product_item a");
                stb.Append(" left join product b on a.product_id=b.product_id");
                stb.AppendFormat(" left join product_combo c on a.product_id = c.child_id and parent_id={0}", product_id);
                stb.AppendFormat(" where a.product_id in (select child_id from product_combo where parent_id={0} and pile_id={1}) group by a.product_id", product_id, pile_id);
                return _dbAccess.getDataTableForObj<Model.Custom.SingleProductPrice>(stb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.SingleProductPriceQuery-->" + ex.Message, ex);
            }
        }
        //查詢各自定價時組合商品中單一商品信息  add by zhuoqin0830w   2015/07/09
        public List<Model.Custom.SelfSingleProductPrice> SingleProductPriceQuery(uint product_id)
        {
            try
            {
                StringBuilder stb = new StringBuilder("SELECT a.product_id, p.product_name,p.prod_sz,MIN(m.price) AS price,s_must_buy,g_must_buy FROM product_item a LEFT JOIN product p ON a.product_id=p.product_id INNER JOIN product_combo c ON p.product_id = c.child_id AND ");
                stb.AppendFormat(@"c.parent_id = {0} LEFT JOIN price_master m ON c.child_id = m.child_id AND c.parent_id = m.product_id WHERE a.product_id IN (SELECT child_id FROM product_combo WHERE parent_id = {0}) GROUP BY a.product_id", product_id);
                return _dbAccess.getDataTableForObj<Model.Custom.SelfSingleProductPrice>(stb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.SingleProductPriceQuery-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询User_Id 为零的价格信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<PriceMaster> QueryByUserId(PriceMaster query)
        {
            query.Replace4MySQL();
            try
            {
                StringBuilder stb = new StringBuilder("select price_master_id,product_id,site_id,user_level,user_id,");
                stb.Append("product_name,bonus_percent,default_bonus_percent,same_price,event_start,event_end,");
                stb.Append("price_status,price,event_price,cost,event_cost,child_id,apply_id");
                stb.Append(" from price_master where 1=1");
                if (query.user_id != 0)
                {
                    stb.AppendFormat(" and user_id = {0}", query.user_id);
                }
                else
                {
                    stb.Append(" and user_id = 0");
                }

                if (query.product_id != 0)
                {
                    stb.AppendFormat(" and product_id = {0}", query.product_id);
                }
                if (query.site_id != 0)
                {
                    stb.AppendFormat(" and site_id = {0}", query.site_id);
                }
                return _dbAccess.getDataTableForObj<PriceMaster>(stb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.QueryByUserId(PriceMaster query)-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 根據商品id、站點id、會員等級、用戶id查詢price_master_id 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public uint QueryPriceMasterId(PriceMaster query)
        {
            query.Replace4MySQL();
            string strSql = string.Format("select price_master_id from price_master where product_id={0} and site_id={1} and user_level={2} and user_id={3} ", query.product_id, query.site_id, query.user_level, query.user_id);
            if (_dbAccess.getSinggleObj<PriceMaster>(strSql) != null)  //edit by wangwei0216w 2014/08/06 (增加對_dbAccess.getSinggleObj<PriceMaster>(strSql)是否為空的判斷,以解決匯入失敗的情況)
            {
                uint price_master_id = _dbAccess.getSinggleObj<PriceMaster>(strSql).price_master_id;
                return price_master_id;
            }
            else
            {
                return 0;
            }

        }
        #region 查詢價格臨時表裡面的數據
        public string VendorSelectChild(PriceMasterTemp priceMaster)
        {   //20140905 複製供應商
            try
            {
                StringBuilder strSql = new StringBuilder("select price_master_id,product_id,child_id");
                uint productid = 0;
                if (uint.TryParse(priceMaster.product_id, out productid))
                {
                    strSql.AppendFormat("   from price_master where product_id = {0} ", productid);
                }
                else
                {
                    strSql.AppendFormat("   from price_master_temp where product_id = '{0}' ", priceMaster.product_id);
                }
                strSql.AppendFormat(" and site_id=1 and user_level=1 and user_id=0 ");
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.VendorSelectChild-->" + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// 獲取要新增站臺商品的子項
        /// </summary>
        /// <param name="pmc">PriceMaster對象</param>
        /// <returns>獲取要新增站臺商品的子項的集合(ItempPrice集合)</returns>
        public List<ItemPrice> AddSingleProduct(Model.Custom.PriceMasterCustom pmc)
        {
            StringBuilder sb2 = new StringBuilder(@"select ip.item_price_id,ip.item_id,ip.item_money,ip.item_cost,ip.event_money,ip.event_cost from item_price ip where price_master_id in 
(select pm.price_master_id from price_master pm where 1=1");
            sb2.AppendFormat(" and product_id={0} and site_id={1} and user_level={2}", pmc.product_id, 1, pmc.user_level);
            if (pmc.combination == 1)
            {
                sb2.AppendFormat(" and  child_id = 0)");
            }
            else
            {
                sb2.AppendFormat(" and  child_id = product_id)");
            }
            return _dbAccess.getDataTableForObj<ItemPrice>(sb2.ToString());
        }



        /// <summary>
        /// 保存單一商品的各項子商品價格
        /// </summary>
        /// <returns></returns>
        public string SaveSingleItemPrice(Model.ItemPrice ip)
        {
            ip.Replace4MySQL();
            try
            {
                StringBuilder strSql = new StringBuilder("insert into item_price(`item_id`,`price_master_id`,`item_money`,`item_cost`,`event_money`,`event_cost`)");
                strSql.AppendFormat("values({0},{1},{2},{3},{4},{5}", ip.item_id, ip.price_master_id, ip.item_money, ip.item_cost, ip.event_money, ip.event_cost);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("SaveSingleItemPrice(Model.ItemPrice ip)-->" + ex.Message, ex);
            }
        }



        /// <summary>
        /// 保存單一商品
        /// </summary>
        /// <returns></returns>
        public int SaveSingleProduct(Model.PriceMaster pm)
        {
            pm.Replace4MySQL();
            try
            {
                StringBuilder strSql = new StringBuilder("insert into price_master(`product_id`,`site_id`,`user_level`,`user_id`,`product_name`,`accumulated_bonus`,");
                strSql.Append("`bonus_percent`,`default_bonus_percent`,`same_price`,`event_start`,`event_end`,`price_status`,`price`,`event_price`,`child_id`,`apply_id`,`cost`,`event_cost`,`bonus_percent_start`,`bonus_percent_end`,`max_price`,`max_event_price`,`valid_start`,`valid_end`)");
                strSql.AppendFormat("values({0},{1},{2},{3}", pm.product_id, pm.site_id, pm.user_level, pm.user_id);
                strSql.AppendFormat(",'{0}',{1},{2},{3}", pm.product_name, pm.accumulated_bonus, pm.bonus_percent, pm.default_bonus_percent);
                strSql.AppendFormat(",{0},{1},{2},{3},{4}", pm.same_price, pm.event_start, pm.event_end, pm.price_status, pm.price);
                strSql.AppendFormat(",{0},{1},{2},{3},{4}", pm.event_price, pm.child_id, pm.apply_id, pm.cost, pm.event_cost);
                strSql.AppendFormat(",{0},{1},{2},{3},{4},{5});select @@identity;", pm.bonus_percent_start, pm.bonus_percent_end, pm.max_price, pm.max_event_price, pm.valid_start, pm.valid_end);
                int result = _dbAccess.execCommand(strSql.ToString());
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.SaveSingleProduct(Model.PriceMaster priceMaster)-->" + ex.Message, ex);
            }
        }



        #endregion

        /// <summary>
        /// 根據ID獲得priceMaster信息
        /// add by wangwei0216w
        /// </summary>
        /// <returns>符合條件的信息集合</returns>
        public List<PriceMaster> GetPriceMasterInfo(string productIds, int siteId)
        {
            try
            {
                StringBuilder sb = new StringBuilder(@"select pm.price_master_id,pm.product_id,pm.site_id,pm.user_level,pm.product_name,pm.accumulated_bonus,pm.bonus_percent,pm.default_bonus_percent,
                    pm.bonus_percent_start,
                    pm.bonus_percent_end,
                    pm.same_price,
                    pm.event_start,
                    pm.event_end,
                    pm.price_status,
                    pm.price,
                    pm.event_price,
                    pm.child_id,
                    pm.apply_id,
                    pm.cost,
                    pm.event_cost,
                    pm.max_price,
                    pm.max_event_price,
                    pm.valid_start,
                    pm.valid_end
                    from price_master pm
                    where");
                // sb.AppendFormat(" site_id=1 and (pm.child_id=0 or pm.child_id=product_id) and user_id=0 and product_id in ({1})", siteId,productIds);
                sb.AppendFormat(" site_id=1 and  user_id=0 and product_id in ({0})", productIds);
                List<PriceMaster> list = _dbAccess.getDataTableForObj<PriceMaster>(sb.ToString());
                return list;
            }

            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.GetPriceMasterInfo(Model.PriceMaster priceMaster)-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 根據ID獲得priceMaster信息(各自定價情況)
        /// add by wangwei0216w 2014/9/19
        /// </summary>
        /// <returns>符合條件的信息集合</returns>
        public List<PriceMaster> GetPriceMasterInfoByID2(string productID)
        {
            try
            {
                StringBuilder sb = new StringBuilder(@"select  
                    pm.price_master_id,
                    pm.product_id,
                    pm.site_id,
                    pm.user_level,
                    pm.user_id,
                    pm.product_name,
                    pm.accumulated_bonus,
                    pm.bonus_percent,
                    pm.default_bonus_percent,
                    pm.bonus_percent_start,
                    pm.bonus_percent_end,
                    pm.same_price,
                    pm.event_start,
                    pm.event_end,
                    pm.price_status,
                    pm.price,
                    pm.event_price,
                    pm.child_id,
                    pm.apply_id,
                    pm.cost,
                    pm.event_cost
                     from 
                    price_master pm");
                sb.AppendFormat(" where product_id={0}", productID);
                List<PriceMaster> list = _dbAccess.getDataTableForObj<PriceMaster>(sb.ToString());
                return list;
            }

            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.GetPriceMasterInfo(Model.PriceMaster priceMaster)-->" + ex.Message, ex);
            }
        }

        public string UpdateProductName(string product_sz, string product_id)
        {
            string obj = string.Empty;
            try
            {

                List<PriceMaster> query = GetPriceMasterInfoByID2(product_id);
                foreach (PriceMaster pm in query)
                {
                    string[] tmp = Regex.Split(PriceMaster.Product_Name_Op(pm.product_name), "`LM`");
                    tmp[2] = product_sz;
                    string result = tmp[0] + "`LM`" + tmp[1] + "`LM`" + tmp[2] + "`LM`" + tmp[3];
                    pm.product_name = PriceMaster.Product_Name_FM(result);
                    obj += Update(pm);
                }
                return obj;
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.GetPriceMasterInfo(Model.PriceMaster UpdateProductName)-->" + ex.Message, ex);
            }
        }

        public List<PriceMasterCustom> GetExcelItemIdInfo(string price_master_ids)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                //添加規格 詳細信息  add by zhuoqin0830w  2015/09/17 s1.spec_name as spec_title_1, s2.spec_name as spec_title_2  INNER JOIN product_item pt ON ip.item_id = pt.item_id LEFT JOIN product_spec s1 on pt.spec_id_1 = s1.spec_id LEFT JOIN product_spec s2 on pt.spec_id_2 = s2.spec_id
                sb.Append(@"SELECT s.site_name,p.product_id,p.product_name,ip.item_id,pm.product_name as item_name,p.product_status,pm.price_status,ip.item_money,ip.item_cost,ip.event_money,");
                sb.Append(@"ip.event_cost AS item_event_cost,s1.spec_name as spec_title_1, s2.spec_name as spec_title_2 FROM price_master pm INNER JOIN product p ON p.product_id = pm.product_id ");
                sb.Append(@"LEFT JOIN item_price ip ON ip.price_master_id = pm.price_master_id INNER JOIN product_item pt ON ip.item_id = pt.item_id LEFT JOIN product_spec s1 on pt.spec_id_1 = s1.spec_id ");
                sb.Append(@"LEFT JOIN product_spec s2 on pt.spec_id_2 = s2.spec_id INNER JOIN site s ON s.site_id = pm.site_id ");
                sb.AppendFormat(@"WHERE pm.price_master_id IN ({0}) ORDER BY pm.product_id ASC,ip.item_id ASC;", price_master_ids);
                return _dbAccess.getDataTableForObj<PriceMasterCustom>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.GetExcelItemIdInfo-->" + ex.Message, ex);
            }
        }

    }
}
