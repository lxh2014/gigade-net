using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
   public class SphinxExcludeDao
    {
        private IDBAccess accessMySql;
        private string connStr;
        public SphinxExcludeDao(string connectionString)
        {
            accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }
        public int InsertModel(SphinxExcludeQuery model)
        {
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                sqlStr.AppendFormat("select count(product_id) from sphinx_exclude where product_id={0}", model.product_id);
                if(accessMySql.getDataTable(sqlStr.ToString()).Rows[0][0].ToString()!="0")
                {
                    return -1;
                }
                sqlStr.Clear();
                sqlStr.AppendFormat("select count(product_id) from product where product_id={0}",model.product_id);
                if (accessMySql.getDataTable(sqlStr.ToString()).Rows[0][0].ToString() == "0")
                {
                    return -2;
                }
                sqlStr.Clear();
                sqlStr.AppendFormat("insert sphinx_exclude(product_id,kdate,kuser) values({0},'{1}','{2}')", model.product_id, Common.CommonFunction.DateTimeToString(DateTime.Now),(System.Web.HttpContext.Current.Session["caller"] as Caller).user_id);
                return accessMySql.execCommand(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SphinxExcludeDao-->InsertModel-->" + ex.Message + sqlStr.ToString(), ex);
            }
        }
        public List<SphinxExcludeQuery> GetList(SphinxExcludeQuery model, out int total)
        {
            StringBuilder sqlStr = new StringBuilder();
            StringBuilder sqlWhr = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlPage = new StringBuilder();
            try
            {
                sqlStr.Append("SELECT se.product_id,se.kdate,mu.user_username,p.product_name FROM sphinx_exclude se");
                sqlStr.Append(" left join product p on se.product_id=p.product_id");
                sqlStr.Append(" left join manage_user mu on se.kuser=mu.user_id");
                sqlStr.Append(" WHERE 1=1 ");
                if (model.product_id != 0)
                {
                    sqlWhr.AppendFormat(" and se.product_id={0}", model.product_id);
                }
                if (model.created_start != DateTime.MinValue && model.created_end != DateTime.MinValue)
                {
                    sqlWhr.AppendFormat(" and se.kdate between '{0}' and '{1}'", Common.CommonFunction.DateTimeToString(model.created_start), Common.CommonFunction.DateTimeToString(model.created_end));
                }
                if(model.product_name!=string.Empty)
                {
                    sqlWhr.AppendFormat(" and p.product_name like '%{0}%'",model.product_name);
                }
                if (model.IsPage)
                {
                    sqlPage.AppendFormat(" limit {0},{1}", model.Start, model.Limit);
                }
                sqlCount.Append("SELECT count(se.product_id)FROM sphinx_exclude se left join product p on se.product_id=p.product_id ");
                total = int.Parse(accessMySql.getDataTable(sqlCount.ToString() + sqlWhr.ToString()).Rows[0][0].ToString());
                return accessMySql.getDataTableForObj<SphinxExcludeQuery>(sqlStr.ToString() + sqlWhr.ToString() + sqlPage.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SphinxExcludeDao-->GetList-->" + ex.Message + sqlStr.ToString(), ex);
            }
        }

        public int UpdateModel(SphinxExcludeQuery model)
        {
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                if(model.product_id!=model.product_id_old)
                {
                    sqlStr.AppendFormat("select count(product_id) from sphinx_exclude where product_id={0}", model.product_id);
                    if (accessMySql.getDataTable(sqlStr.ToString()).Rows[0][0].ToString() != "0")
                    {
                        return -1;
                    }
                    sqlStr.Clear();
                    sqlStr.AppendFormat("select count(product_id) from product where product_id={0}", model.product_id);
                    if (accessMySql.getDataTable(sqlStr.ToString()).Rows[0][0].ToString() == "0")
                    {
                        return -2;
                    }
                    sqlStr.Clear();
                }
                sqlStr.AppendFormat("update sphinx_exclude set product_id={0} where product_id={1}",model.product_id,model.product_id_old);
                return accessMySql.execCommand(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SphinxExcludeDao-->UpdateModel-->" + ex.Message + sqlStr.ToString(), ex);
            }
        }

        public int DeleteModel(SphinxExcludeQuery model)
        {
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                sqlStr.AppendFormat("delete from sphinx_exclude  where product_id in({0})", model.product_ids);
                return accessMySql.execCommand(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SphinxExcludeDao-->DeleteModel-->" + ex.Message + sqlStr.ToString(), ex);
            }
        }
    }
}
