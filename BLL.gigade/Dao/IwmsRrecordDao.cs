using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class IwmsRrecordDao
    {
        IDBAccess _accessMySql;
        string connStr = string.Empty;
        public IwmsRrecordDao(string connectionstring)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
        }
        public List<IwmsRrecordQuery> GetIwmsRrecordList(IwmsRrecordQuery query, out int totalCount)
        {
            query.Replace4MySQL();
            StringBuilder sbAll = new StringBuilder();
            StringBuilder sbJoin = new StringBuilder();
            StringBuilder sbWhr = new StringBuilder();
            StringBuilder sbPage = new StringBuilder();
            sbAll.Append("SELECT ir.order_id,od.item_id,ir.act_pick_qty,ir.made_dt,ir.cde_dt,ir.create_date,ir.cde_dt_incr,ir.cde_dt_shp,mu.user_username,od.product_name,od.product_spec_name,pii.product_id FROM iwms_record ir  ");
            sbJoin.Append("LEFT JOIN manage_user mu ON ir.create_user_id=mu.user_id ");
            sbJoin.Append("LEFT JOIN order_detail od ON ir.detail_id=od.detail_id LEFT JOIN product_item pii on pii.item_id =od.item_id  where 1=1 ");
            if (query.item_id.ToString().Trim().Length == 6)
            {
                sbWhr.AppendFormat("and od.item_id ='{0}' ", query.item_id);
            }
            else if(query.item_id.ToString().Length==5)
            {
                sbWhr.AppendFormat(" and pii.product_id = '{0}' ",query.item_id);
            }
            if (query.starttime != DateTime.MinValue && query.endtime != DateTime.MinValue)
            {
                if (query.datetype != "0")
                {
                    sbWhr.AppendFormat("and ir.{0} BETWEEN '{1}' and '{2}' ", query.datetype, Common.CommonFunction.DateTimeToString(query.starttime), Common.CommonFunction.DateTimeToString(query.endtime));
                }
            }
            if (query.product_name != string.Empty)
            {
                sbWhr.AppendFormat("and od.product_name like '%{0}%'", query.product_name);
            }
            if (query.user_username != string.Empty)
            {
                sbWhr.AppendFormat(" and mu.user_username like '%{0}%' ", query.user_username);
            }
            sbPage.AppendFormat("LIMIT {0},{1};", query.Start, query.Limit);
            try
            {
                DataTable dt = _accessMySql.getDataTable("select count(ir.order_id) from iwms_record ir " + sbJoin.ToString() + sbWhr.ToString());
                totalCount = int.Parse(dt.Rows[0][0].ToString());
                return _accessMySql.getDataTableForObj<IwmsRrecordQuery>(sbAll.ToString() + sbJoin.ToString() + sbWhr.ToString() + sbPage.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IwmsRrecordDao-->GetIwmsRrecordList-->" + ex.Message + sbAll.ToString() + sbJoin.ToString() + sbWhr.ToString() + sbPage.ToString(), ex);
            }
        }


        public List<ManageUser> GetUserslist(string code)
        {
           
            StringBuilder sbWhr = new StringBuilder();

            sbWhr.AppendFormat(@"SELECT mur.user_id,mur.user_username FROM t_fgroup tf LEFT JOIN t_groupcaller tfpl on tf.rowid=tfpl.groupId 
LEFT JOIN manage_user mur on mur.user_email=tfpl.callid WHERE tf.groupCode='{0}';",code);
            try
            {
                return _accessMySql.getDataTableForObj<ManageUser>(sbWhr.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IwmsRrecordDao-->GetUserslist-->" + ex.Message + sbWhr.ToString(), ex);
            }
        }
    }
}
