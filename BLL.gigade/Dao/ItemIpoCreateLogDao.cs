using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class ItemIpoCreateLogDao : IItemIpoCreateLogImplDao
    {
        private IDBAccess _accessMySql;

        public ItemIpoCreateLogDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        public string getAddsql(ItemIpoCreateLogQuery query)
        {  
            StringBuilder sb = new StringBuilder();
            try
            {
              
                if (!string.IsNullOrEmpty(query.item_id_in))
                {
                    string[] item_id = query.item_id_in.Split(',');
                    for (int i = 0; i < item_id.Length;i++ )
                    {
                        sb.Append(" insert into item_ipo_create_log (item_id,create_time,create_user,log_status)value( ");
                        sb.AppendFormat("'{0}','{1}','{2}','{3}');", item_id[i], Common.CommonFunction.DateTimeToString(query.create_datetime), query.create_user, query.log_status); 
                    }
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ItemIpoCreateLogDao.AddItemIpoCreate-->" + ex.Message + sb.ToString(), ex);
            }
        }
        public int AddItemIpoCreate(ItemIpoCreateLogQuery query)
        {
            StringBuilder sql = new StringBuilder();

            try
            {
                sql.Append(getAddsql(query));
                if (!string.IsNullOrEmpty(sql.ToString()))
                {
                    return _accessMySql.execCommand(sql.ToString());
                }
                else 
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ItemIpoCreateLogDao.AddItemIpoCreate-->" + ex.Message + sql.ToString(), ex);
            }

        }
      
    }
}
