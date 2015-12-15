using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using System.Data;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Dao
{
    public class LogisticsTcatEodDao 
    { 
        private IDBAccess _accessMySql;       
        public LogisticsTcatEodDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public DataTable GetOrderInfoForTcat ()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"select 
lte.delivery_type,lte.delivery_number,lte.order_id,'' as 'ftp_userID',lte.freight_set,lte.delivery_distance,lte.package_size,
lte.cash_collect_service,lte.cash_collect_amount,'N' as '是否到付','01'as '是否付現',
CASE when delivery_type='1' then '*' else  dm.delivery_name end ,
CASE when delivery_type='1' then '*' else  dm.delivery_mobile end ,
CASE when delivery_type='1' then '*' else  dm.delivery_phone end ,
lte.receiver_zip,lte.receiver_address,'' as '寄件人姓名','' as '寄件人電話','' as '寄件人手機','' as '寄件人郵遞區號','' as '寄件人地址',lte.delivery_date,
'' as '預定取件時段',lte.estimate_arrival,om.user_id,lte.package_name,'N' as '易碎物品' ,'N' as '精密儀器' ,lte.delivery_note,''  as 'SD路線代碼' 
from logistics_tcat_eod lte left join deliver_master dm on dm.order_id=lte.order_id and dm.delivery_code=lte.delivery_number 
                            LEFT JOIN order_master om on om.order_id=lte.order_id 
where lte.upload_time is null ");
            return _accessMySql.getDataTable(sql.ToString());
        }

        public int UpdateUploadTime()
        {
            StringBuilder UpdateSql = new StringBuilder();
            UpdateSql.AppendFormat(@"set sql_safe_updates = 0; update logistics_tcat_eod set upload_time='{0}' where upload_time is null; set sql_safe_updates = 1;",
                                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            return _accessMySql.execCommand(UpdateSql.ToString());
        }
    }
}
