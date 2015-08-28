using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class OrderMasterShopCom
    {
        private IDBAccess _access;
        private string connStr;
        public OrderMasterShopCom(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }

        public List<MarketOrderQuery> GetMarketOrderExcel(MarketOrderQuery q)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@"SELECT om.order_createdate,om.order_id,delivery_name,oms.rid,od.product_name,od.buy_num, od.parent_num,od.single_money AS 'price','' as 'Sale Amount',od.item_id,od.deduct_happygo_money,od.deduct_welfare,od.deduct_bonus,od.item_mode
from order_master_shop_com oms LEFT JOIN order_master om ON oms.order_id=om.order_id
LEFT JOIN order_slave os ON om.order_id = os.order_id
LEFT JOIN order_detail od ON os.slave_id = od.slave_id
WHERE  item_mode in (0,1) ");
                DateTime smaill = DateTime.Parse("1990-01-01");
                if (!string.IsNullOrEmpty(q.starttime.ToString()) && q.starttime > smaill)
                {
                    sb.AppendFormat(" and om.order_createdate > '{0}' ",CommonFunction.GetPHPTime(q.starttime.ToString()));
                }
                if (!string.IsNullOrEmpty(q.endtime.ToString()) && q.endtime > smaill && q.starttime < q.endtime)
                {
                    sb.AppendFormat(" and om.order_createdate < '{0}' ", CommonFunction.GetPHPTime(q.endtime.ToString()));                
                }
                return _access.getDataTableForObj<MarketOrderQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" OrderMasterShopCom-->GetMarketOrderExcel-->" + ex.Message + "sql:" + sb.ToString(), ex);

            }
        }

    }
}
