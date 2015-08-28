using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using System.Data;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao
{
    public class MarketCategoryDao : IMarketCategoryImplDao
    {
        private IDBAccess _access;

        public MarketCategoryDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        public List<MarketCategoryQuery> GetMarketCategoryList(MarketCategory model, out int totalCount)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            sql.Append("select m.market_category_id,m.market_category_name,m.market_category_father_id,m.market_category_code");
            sql.Append(",m.market_category_sort,m.market_category_status,m.modified");
            sql.Append(",f.market_category_father_name,mu.user_username as muser_name ");//m.muser,
            sqlCondi.Append(" from market_category m");
            //獲取父級類別名稱
            sqlCondi.Append(" left join (select market_category_id, market_category_name as market_category_father_name from market_category) f on f.market_category_id=m.market_category_father_id");
            sqlCondi.Append("  left join manage_user mu on mu.user_id=m.muser ");
            //獲取特定父級下的類別

            sqlCondi.AppendFormat(" where m.market_category_father_id='{0}'", model.market_category_father_id);

            if (!string.IsNullOrEmpty(model.market_category_code))
            {
                sqlCondi.AppendFormat(" and m.market_category_code='{0}'", model.market_category_code);
            }

            if (!string.IsNullOrEmpty(model.market_category_name))
            {
                sqlCondi.AppendFormat(" and m.market_category_name like '%{0}%'", model.market_category_name);
            }
            sqlCondi.Append(" order by m.market_category_id desc");
            try
            {
                totalCount = 0;
                if (model.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable("select count(m.market_category_id) as totalCount" + sqlCondi.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"].ToString());
                    }
                    sqlCondi.AppendFormat(" limit {0},{1}", model.Start, model.Limit);
                }
                sql.Append(sqlCondi.ToString());
                return _access.getDataTableForObj<MarketCategoryQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("MarketCategoryDao.GetMarketCategoryList-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string InsertMarketCategory(MarketCategory model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("insert into market_category(market_category_name,market_category_father_id");
                sql.Append(",market_category_code,market_category_sort,market_category_status,kuser,muser,created,modified,attribute)");
                sql.AppendFormat(" values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}');",
                    model.market_category_name, model.market_category_father_id, model.market_category_code, model.market_category_sort
                    , model.market_category_status, model.kuser, model.muser, BLL.gigade.Common.CommonFunction.DateTimeToString(model.created), BLL.gigade.Common.CommonFunction.DateTimeToString(model.modified), model.attribute);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("MarketCategoryDao.InsertMarketCategory-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public List<MarketCategoryQuery> GetMarketCategoryList(MarketCategory model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            sql.Append("select m.market_category_id,m.market_category_name,m.market_category_father_id,m.market_category_code");
            sql.Append(",m.market_category_sort,m.market_category_status,m.muser,m.modified,f.market_category_father_name");

            sqlCondi.Append(" from market_category m ");
            sqlCondi.Append(" left join (select market_category_id, market_category_name as market_category_father_name from market_category) f on f.market_category_id=m.market_category_father_id");
            sqlCondi.Append(" where 1=1");
            if (!string.IsNullOrEmpty(model.market_category_code))
            {
                sqlCondi.AppendFormat(" and m.market_category_code='{0}'", model.market_category_code);
            }
            if (model.market_category_id != 0)
            {
                sqlCondi.AppendFormat(" and m.market_category_id='{0}'", model.market_category_id);
            }
            if (model.market_category_father_id != 0)
            {
                sqlCondi.AppendFormat(" and m.market_category_father_id='{0}'", model.market_category_father_id);
            }
            try
            {
                sql.Append(sqlCondi.ToString());
                return _access.getDataTableForObj<MarketCategoryQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("MarketCategoryDao.GetMarketCategoryList-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string UpdateMarketCategory(MarketCategory model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update market_category  set market_category_name='{0}',market_category_father_id='{1}'", model.market_category_name, model.market_category_father_id);
                sql.AppendFormat(",market_category_code='{0}',market_category_sort='{1}',market_category_status='{2}'", model.market_category_code, model.market_category_sort, model.market_category_status);
                sql.AppendFormat(",muser='{0}',modified='{1}',attribute='{2}' where market_category_id='{3}';", model.muser, BLL.gigade.Common.CommonFunction.DateTimeToString(model.modified), model.attribute, model.market_category_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("MarketCategoryDao.UpdateMarketCategory-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public string DeleteMarketCategory(int cid)
        {

            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("delete from market_category where market_category_id='{0}';", cid);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("MarketCategoryDao.DeleteMarketCategory-->" + ex.Message + sql.ToString(), ex);
            }
        }

    }
}
