using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Mgr.Schedules;
namespace BLL.gigade.Dao
{
    public class LogisticsTcatSodDao 
    { 
        private IDBAccess _accessMySql;       
        public LogisticsTcatSodDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public DataTable GetRepeatInfo(LogisticsTcatSodQuery query)
        {
            query.Replace4MySQL();
            StringBuilder selectSql = new StringBuilder("");
            selectSql.AppendFormat(@"select order_id  from logistics_tcat_sod where delivery_number='{0}' and order_id='{1}' 
                                            and delivery_status_time='{2}' and status_id='{3}'", query.delivery_number,query.order_id, query.delivery_status_time.ToString("yyyy-MM-dd HH:mm:ss"), query.status_id);
            return _accessMySql.getDataTable(selectSql.ToString());
        }

        public int InsertReceiveFromTcatFTP(LogisticsTcatSod model)
        {
            model.Replace4MySQL();
            StringBuilder InsertSql = new StringBuilder("");
            InsertSql.AppendFormat(@" INSERT INTO logistics_tcat_sod (delivery_number,order_id,station_name,delivery_status_time,customer_id,status_id,
                                   status_note,specification,create_date) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}'); ",
                                     model.delivery_number, model.order_id, model.station_name, model.delivery_status_time, model.customer_id, 
                                     model.status_id,model.status_note,model.specification ,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            return _accessMySql.execCommand(InsertSql.ToString());
        }
        public DeliveryInfo GetLogisticsTcatSod(string delivery_code)
        {
            DeliveryInfo deliver = new DeliveryInfo();

            string sql = string.Format("SELECT  status_id,delivery_status_time  FROM logistics_tcat_sod WHERE delivery_number={0};", delivery_code);
            DataTable table = _accessMySql.getDataTable(sql);
            if (table.Rows.Count > 0)
            {
                DateTime date;
                if (DateTime.TryParse(table.Rows[0]["delivery_status_time"].ToString(), out date))
                {
                    deliver.CreateTime = date;
                }
                if (table.Rows[0]["status_id"].ToString() == "00003")
                {
                    deliver.Status = "順利送達";
                }
            }
            return deliver;
        }  
    }
}
