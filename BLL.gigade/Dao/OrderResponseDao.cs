using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class OrderResponseDao : IOrderResponseIDao
    {
        
        private IDBAccess _access;
        public OrderResponseDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
        }
        public int insert(Model.OrderResponse o)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sb1 = new StringBuilder();
            string sqlid = "SELECT * from serial where serial_id ='35';";
            try
            {
                o.response_id = uint.Parse(_access.getDataTable(sqlid).Rows[0]["serial_value"].ToString())+1;
                sb1.AppendFormat("UPDATE serial SET serial_value='{0}' where serial_id ='35';", o.response_id);
                _access.execCommand(sb1.ToString());
                sb.AppendFormat(@"INSERT INTO order_response (response_id,question_id,response_type,user_id,response_content,response_ipfrom,response_createdate) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}');", o.response_id, o.question_id,o.response_type, o.user_id, o.response_content, o.response_ipfrom, o.response_createdate);
                return  _access.execCommand(sb.ToString());
            }
            catch(Exception ex)
            {
                throw new Exception("OrderResponseDao-->insert-->" + ex.Message + sb, ex); 
            }
        }
    }
}
