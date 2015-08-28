using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Dao
{
    public class DeliveryStoreDao:IDeliveryStoreImplDao
    {
        private IDBAccess _access;
        private string sqlStr = "";
        public DeliveryStoreDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public int Save(DeliveryStore store)
        {
            store.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("insert into delivery_store(`delivery_store_id`,`big`,`bigcode`,`middle`,`middlecode`,`small`,`smallcode`,`store_id`,`store_name`,");
            strSql.AppendFormat("`address`,`phone`,`status`)values({0},'{1}','{2}','{3}','{4}',",store.delivery_store_id,store.big,store.bigcode,store.middle,store.middlecode);
            strSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}',{6})", store.small, store.smallcode, store.store_id, store.store_name, store.address, store.phone, store.status);
            return _access.execCommand(strSql.ToString());
        }
        public int Update(DeliveryStore store)
        {
            store.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("update delivery_store set ");
            strSql.AppendFormat("delivery_store_id={0},big='{1}',bigcode='{2}',middle='{3}',middlecode='{4}',", store.delivery_store_id, store.big, store.bigcode, store.middle, store.middlecode);
            strSql.AppendFormat("small='{0}',smallcode='{1}',store_id='{2}',store_name='{3}',address='{4}',", store.small, store.smallcode,store.store_id, store.store_name, store.address);
            strSql.AppendFormat("phone='{0}',status={1} where rowid={2}", store.phone, store.status, store.rowid);
            return _access.execCommand(strSql.ToString());
        }
        public int Delete(int rodId)
        {
            StringBuilder strSql = new StringBuilder("delete from delivery_store where rowid=" + rodId);
            return _access.execCommand(strSql.ToString());
        }

        /// <summary>
        /// 查詢超商店家
        /// </summary>
        /// <param name="store">Store Model</param>
        /// <param name="status">店家狀態</param>
        /// <returns>Store Model List</returns>
        public List<DeliveryStoreQuery> Query(DeliveryStore store, out int totalCount)
        {
            store.Replace4MySQL();
            StringBuilder tempStr = new StringBuilder("select delivery_store.rowid,delivery_store_id,a.parametername as delivery_store_name,big,bigcode,middle,middlecode,");
            tempStr.Append("small,smallcode,store_id,store_name,address,phone,status from delivery_store left join ");
            tempStr.Append(" (select parametercode,parametername from t_parametersrc where parametertype='deliver_store') a on ");
            tempStr.Append("delivery_store.delivery_store_id=a.parametercode where 1=1");
            if (store.delivery_store_id != 0)
            {
                tempStr.AppendFormat(" and delivery_store_id = {0}", store.delivery_store_id); ;
            }
            if (store.status != 0)
            {
                tempStr.AppendFormat(" and status = {0}", store.status);
            }
            totalCount = 0;
            if (store.IsPage)
            {
                System.Data.DataTable _dt = _access.getDataTable(tempStr.ToString());
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    totalCount = _dt.Rows.Count;
                }
                tempStr.AppendFormat(" limit {0},{1}", store.Start, store.Limit);
            }
            return _access.getDataTableForObj<DeliveryStoreQuery>(tempStr.ToString());
        }
    }
}
