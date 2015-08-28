using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Common;
using System.Data;


namespace BLL.gigade.Dao
{
    public class ProdPromoDao : IProdPromoImplDao
    {
        private IDBAccess _access;
        private string connStr;
        public ProdPromoDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
        }
        public List<ProdPromo> Select(ProdPromo query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("select product_id, event_id,event_type,event_desc,start,end,page_url,user_specified,kuser,kdate,muser,mdate,status from prod_promo  ");
            sql.AppendFormat("where status = 1 and 1=1;");
            totalCount = 0;
            return _access.getDataTableForObj<ProdPromo>(sql.ToString());
        }
        public DataTable Query(ProdPromo query)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("select product_id, event_id,event_type,event_desc,start,end,page_url,user_specified,kuser,kdate,muser,mdate,status from prod_promo  ");
            sql.AppendFormat("where status = 1 and event_id='{0}' and product_id='{1}';", query.event_id, query.product_id);
            return _access.getDataTable(sql.ToString());
        }
        public int Save(ProdPromo model)
        {
            model.Replace4MySQL();

            StringBuilder sbSql = new StringBuilder();
            sbSql.Append(@" insert into prod_promo (");
            sbSql.Append(" product_id, event_id,event_type,event_desc,start,end,page_url,user_specified,kuser,kdate,muser,mdate,status) ");
            sbSql.AppendFormat(" values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}');", model.product_id, model.event_id, model.event_type, model.event_desc, CommonFunction.DateTimeToString(model.start), Common.CommonFunction.DateTimeToString(model.end), model.page_url, model.user_specified, model.kuser, Common.CommonFunction.DateTimeToString(model.kdate), model.muser, Common.CommonFunction.DateTimeToString(model.mdate), model.status);
            return _access.execCommand(sbSql.ToString());

        }

        public int Update(ProdPromo model)
        {
            model.Replace4MySQL();

            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("set sql_safe_updates = 0; ");
            sbSql.Append("update prod_promo set ");
            sbSql.AppendFormat(" product_id='{0}', event_type='{1}',event_desc='{2}',start='{3}',end='{4}', ", model.product_id, model.event_type, model.event_desc, CommonFunction.DateTimeToString(model.start), Common.CommonFunction.DateTimeToString(model.end));
            sbSql.AppendFormat(" page_url='{0}',user_specified='{1}',muser='{2}',mdate='{3}',status='{4}'", model.page_url, model.user_specified, model.muser, Common.CommonFunction.DateTimeToString(model.mdate), model.status);
            sbSql.AppendFormat(" where event_id='{0}';", model.event_id);
            sbSql.Append("set sql_safe_updates = 1;");
            return _access.execCommand(sbSql.ToString());
        }

      
        /// <summary>
        /// 新增促銷活動時使用
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string SaveProdProm(ProdPromo model)
        {
            model.Replace4MySQL();

            StringBuilder sbSql = new StringBuilder();
            sbSql.Append(@" insert into prod_promo (");
            sbSql.Append(" product_id, event_id,event_type,event_desc,start,end,page_url,user_specified,kuser,kdate,muser,mdate,status) ");
            sbSql.AppendFormat(" values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}');", model.product_id, model.event_id, model.event_type, model.event_desc, CommonFunction.DateTimeToString(model.start), Common.CommonFunction.DateTimeToString(model.end), model.page_url, model.user_specified, model.kuser, Common.CommonFunction.DateTimeToString(model.kdate), model.muser, Common.CommonFunction.DateTimeToString(model.mdate), model.status);
            return sbSql.ToString();
        }

        public string DeleteProdProm(string event_id)
        {
            StringBuilder sbSql = new StringBuilder();

            sbSql.AppendFormat("set sql_safe_updates = 0;delete from prod_promo where event_id='{0}';set sql_safe_updates = 1; ", event_id);

            return sbSql.ToString();
        }
        public string DeleteProdPromByPID(ProdPromo model)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("set sql_safe_updates = 0;delete from prod_promo where event_id='{0}' and status=1 and product_id='{1}' ;set sql_safe_updates = 1;", model.event_id, model.product_id);
            return sb.ToString();
        }
        public string DelProdProm(string event_id)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("set sql_safe_updates = 0;update prod_promo set status=0 where event_id='{0}' and status=1 ;set sql_safe_updates = 1;", event_id);
            return sb.ToString();
        }

    }
}
