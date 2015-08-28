using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*
* 文件名稱 :ShippingVoucherDao.cs
* 文件功能描述 :
* 版權宣告 :
* 開發人員 : yafeng0715j
* 版本資訊 : 1.0
* 日期 : 2015/08/03
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
	*/

namespace BLL.gigade.Dao
{
    public class ShippingVoucherDao
    {
        private IDBAccess accessMySql;
        private string connStr;
        public ShippingVoucherDao(string connectionString)
        {
            accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }
        public List<ShippingVoucherQuery> GetList(ShippingVoucherQuery query,out int total)
        {
            total = 0;
            StringBuilder sqlStr = new StringBuilder();
            StringBuilder sqlWhr = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlLimit = new StringBuilder();
            try
            {
                sqlStr.Append("SELECT  sv.sv_id,sv.order_id,sv.user_id,u.user_name,ml.ml_name,sv.sv_year,sv.sv_month,sv.sv_created,sv.sv_modified,");
                sqlStr.Append("CASE sv_state WHEN 0 THEN '未使用' WHEN 1 THEN '已使用' WHEN 2 THEN '未使用已到期' END AS state_name");
                sqlStr.Append(" FROM shipping_voucher sv LEFT JOIN users u ON sv.user_id=u.user_id");
                sqlStr.Append(" LEFT JOIN member_level ml ON u.ml_code=ml.ml_code");
                sqlStr.Append(" WHERE 1=1 ");
                if(query.user_id!=0 || query.order_id!=0)
                {
                    sqlWhr.AppendFormat(" and (sv.user_id={0} or sv.order_id={1})",query.user_id,query.order_id);
                }
                if(query.user_name!=string.Empty)
                {
                    sqlWhr.AppendFormat(" and u.user_name like '%{0}%'",query.user_name);
                }
                if(query.created_start!=DateTime.MinValue &&query.created_end!=DateTime.MinValue)
                {
                    sqlWhr.AppendFormat(" and sv.sv_created between '{0}' and '{1}'",Common.CommonFunction.DateTimeToString(query.created_start),Common.CommonFunction.DateTimeToString(query.created_end));
                }
                if(query.sv_state!=3)
                {
                    sqlWhr.AppendFormat(" and sv.sv_state={0}", query.sv_state);
                }
                if(query.IsPage)
                {
                    sqlLimit.AppendFormat(" limit {0},{1}",query.Start,query.Limit);
                }
                sqlCount.Append("select count(sv.sv_id) FROM shipping_voucher sv LEFT JOIN users u ON sv.user_id=u.user_id LEFT JOIN member_level ml ON u.ml_code=ml.ml_code WHERE 1=1");
                total = int.Parse(accessMySql.getDataTable(sqlCount.ToString() + sqlWhr.ToString()).Rows[0][0].ToString());
                return accessMySql.getDataTableForObj<ShippingVoucherQuery>(sqlStr.ToString()+sqlWhr.ToString()+sqlLimit.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ShippingVoucherDao-->GetList-->" + ex.Message + sqlStr.ToString(), ex);
            }
        }
    }
}
