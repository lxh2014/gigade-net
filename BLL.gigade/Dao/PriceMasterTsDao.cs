using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class PriceMasterTsDao : IPriceMasterTsImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public PriceMasterTsDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }

        //查詢price_master_ts表 add by xiangwang0413w 2014/07/17
        public Model.PriceMaster QueryPriceMasterTs(Model.PriceMaster pM)
        {
            try
            {
                StringBuilder stb = new StringBuilder("select price_master_id,product_id,site_id,user_level,user_id,");
                stb.Append("product_name,bonus_percent,default_bonus_percent,same_price,accumulated_bonus,event_start,event_end,");
                stb.Append("price_status,price,event_price,cost,event_cost,child_id,apply_id,bonus_percent_start,bonus_percent_end,max_price,max_event_price,valid_start,valid_end ");
                stb.AppendFormat(" from price_master_ts where price_master_id={0};", pM.price_master_id);

                return _dbAccess.getSinggleObj<Model.PriceMaster>(stb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.QueryPriceMasterTs-->" + ex.Message, ex);
            }
        }

        public List<Model.PriceMaster> QueryByApplyId(Model.PriceMaster pM)
        {
            try
            {
                StringBuilder stb = new StringBuilder("select price_master_id,product_id,site_id,user_level,user_id,");
                stb.Append("product_name,bonus_percent,default_bonus_percent,same_price,accumulated_bonus,event_start,event_end,");
                stb.Append("price_status,price,event_price,cost,event_cost,child_id,apply_id,bonus_percent_start,bonus_percent_end,max_price,max_event_price,valid_start,valid_end ");
                stb.AppendFormat(" from price_master_ts where product_id ={0} and apply_id={1}", pM.product_id, pM.apply_id);
                return _dbAccess.getDataTableForObj<Model.PriceMaster>(stb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.QueryPriceMaster-->" + ex.Message, ex);
            }
        }
        //修改站台價格不再更新price_master表，而是更新price_master_ts表  add by xiangwang0413w 2014/07/16
        public string UpdateTs(Model.PriceMaster pM)
        {
            try
            {
                pM.Replace4MySQL();
                StringBuilder strSql = new StringBuilder();
                strSql.AppendFormat("set sql_safe_updates=0;delete from price_master_ts where price_master_id={0};set sql_safe_updates=1;", pM.price_master_id);

                strSql.Append(@"insert into price_master_ts(`price_master_id`,`product_id`,`site_id`,`user_level`,`user_id`,`product_name`,
                `accumulated_bonus`,`bonus_percent`,`default_bonus_percent`,`bonus_percent_start`,`bonus_percent_end`,
                `same_price`,`event_start`,`event_end`,`price_status`, `price`,`event_price`,
                `child_id`,`apply_id`,`cost`,`event_cost`,`max_price`,`max_event_price`,`valid_start`,`valid_end`)");
                strSql.AppendFormat(" select `price_master_id`,{0} as `product_id`,{1} as `site_id`,{2} as `user_level`,{3} as `user_id`,'{4}' as `product_name`,"
                    , pM.product_id, pM.site_id, pM.user_level, pM.user_id, pM.product_name);
                strSql.AppendFormat("{0} as `accumulated_bonus`,{1} as `bonus_percent`,{2} as `default_bonus_percent`,{3} as `bonus_percent_start`,{4} as `bonus_percent_end`,"
                    , pM.accumulated_bonus, pM.bonus_percent, pM.default_bonus_percent,pM.bonus_percent_start,pM.bonus_percent_end);
                strSql.AppendFormat("{0} as `same_price`,{1} as `event_start`,{2} as `event_end`,{3} as `price_status`, {4} as `price`,{5} as `event_price`,"
                    ,pM.same_price,pM.event_start,pM.event_end,pM.price_status,pM.price,pM.event_price);
                strSql.AppendFormat("{0} as `child_id`,{1} as `apply_id`,{2} as `cost`,{3} as `event_cost`,{4} as `max_price`,{5} as `max_event_price`,{6} as `valid_start`,{7} as `valid_end`" 
                    , pM.child_id, pM.apply_id, pM.cost, pM.event_cost, pM.max_price, pM.max_event_price, pM.valid_start,pM.valid_end);
                strSql.AppendFormat(" from price_master where price_master_id={0};", pM.price_master_id);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterDao.UpdateTs-->" + ex.Message, ex);
            }
        }

        //更新活動價(活動售價,活動成本,活動開始時間,活動結束時間)
//        public string UpdateEventTs(Model.PriceMaster pM)
//        {
//            try
//            {
//                StringBuilder strSql = new StringBuilder();
//                strSql.AppendFormat("set sql_safe_updates=0;delete from price_master_ts where price_master_id={0};set sql_safe_updates=1;", pM.price_master_id);
//                strSql.Append(@"insert into price_master_ts(`price_master_id`,`product_id`,`site_id`,`user_level`,`user_id`,`product_name`,
//                `accumulated_bonus`,`bonus_percent`,`default_bonus_percent`,`bonus_percent_start`,`bonus_percent_end`,
//                `same_price`, `price`, `child_id`,`cost`,`max_price`,`max_event_price`,`valid_start`,`valid_end`,
//                `event_price`,`event_cost`,`event_start`,`event_end`,`price_status`,`apply_id`) ");
//                strSql.Append(@" select `price_master_id`,`product_id`,`site_id`,`user_level`,`user_id`,`product_name`,
//                `accumulated_bonus`,`bonus_percent`,`default_bonus_percent`,`bonus_percent_start`,`bonus_percent_end`,
//                `same_price`, `price`, `child_id`,`cost`,`max_price`,`max_event_price`,`valid_start`,`valid_end`,");
//                strSql.AppendFormat("{0} as `event_price`,{1} as `event_cost`,{2} as `event_start`,{3} as `event_end`,", pM.event_price, pM.event_cost, pM.event_start, pM.event_end);
//                strSql.AppendFormat("{0} as `price_status`,{1} as `apply_id` from price_master where price_master_id={2};", pM.price_status, pM.apply_id, pM.price_master_id);
//                return strSql.ToString();
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("PriceMasterDao.UpdateEventTs-->" + ex.Message, ex);
//            }
//        }

        //根據pirce_master_id刪除price_master_ts
        public string DeleteTs(Model.PriceMaster pM)
        {
            //return string.Format("delete from price_master_ts where price_master_id={0};", pM.price_master_id);
            return string.Format("delete from price_master_ts where  price_master_id={0} and apply_id={1};",pM.price_master_id, pM.apply_id);
        }

    }

}
