using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Dao
{
    //add by yafeng0715j 20150825PM
    public class DeliveryStorePlaceDao
    {
        IDBAccess dbaccess;
        public DeliveryStorePlaceDao(string connectionStr)
        {
            dbaccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        public List<DeliveryStorePlaceQuery> GetDeliveryStorePlaceList(DeliveryStorePlace model, out int total)
        {
            total = 0;
            StringBuilder sbAll = new StringBuilder("SELECT dsp.dsp_id,dsp.dsp_name,dsp.dsp_address,dsp.dsp_deliver_store,dsp.dsp_big_code,dsp.dsp_telephone,zc.big,tp.parameterName,dsp.dsp_status,dsp.dsp_note,");
            sbAll.Append("mu.user_username AS create_username,mu1.user_username AS modify_username,dsp.create_time,dsp.modify_time");
            StringBuilder sbJoin = new StringBuilder(" FROM delivery_store_place dsp LEFT JOIN manage_user mu ON mu.user_id=dsp.create_user");
            sbJoin.Append(" LEFT JOIN manage_user mu1 ON mu1.user_id=dsp.modify_user");
            sbJoin.Append(" LEFT JOIN (SELECT big,bigcode FROM t_zip_code GROUP BY bigcode) zc ON zc.bigcode=dsp.dsp_big_code");
            sbJoin.Append(" LEFT JOIN (SELECT s.parameterCode,s.parameterName FROM t_parametersrc s WHERE  s.parameterType='deliver_store') tp ON tp.parameterCode=dsp.dsp_deliver_store");
            StringBuilder sbWhr = new StringBuilder(" where 1=1 ");

            try
            {
                if (model.dsp_name != string.Empty)
                {
                    sbWhr.AppendFormat(" and dsp.dsp_name like '%{0}%'", model.dsp_name);
                }
                if (model.dsp_big_code !="0")
                {
                    sbWhr.AppendFormat(" and dsp.dsp_big_code = '{0}'", model.dsp_big_code);
                }
                if (model.dsp_deliver_store != "0")
                {
                    sbWhr.AppendFormat(" and dsp.dsp_deliver_store='{0}'", model.dsp_deliver_store);
                }
                if (model.IsPage)
                {
                    StringBuilder sbPage = new StringBuilder("select count(dsp.dsp_id)");
                    sbPage.Append(sbJoin).Append(sbWhr);
                    total = Convert.ToInt32(dbaccess.getDataTable(sbPage.ToString()).Rows[0][0]);
                    sbWhr.AppendFormat(" limit {0},{1}", model.Start, model.Limit);
                }
                return dbaccess.getDataTableForObj<DeliveryStorePlaceQuery>(sbAll.Append(sbJoin).Append(sbWhr).ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("DeliveryStorePlaceDao-->GetDeliveryStorePlaceList-->" + ex.Message + "sql:" + sbAll.Append(sbJoin).Append(sbWhr).ToString(), ex);
            }

        }

        public int InsertDeliveryStorePlace(DeliveryStorePlace model)
        {
            StringBuilder sbAll = new StringBuilder("insert into  delivery_store_place(dsp_name,dsp_address,dsp_telephone,dsp_big_code,dsp_deliver_store,dsp_status,dsp_note,create_user,create_time,modify_user,modify_time) values");
            sbAll.AppendFormat("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')", model.dsp_name, model.dsp_address, model.dsp_telephone, model.dsp_big_code, model.dsp_deliver_store, model.dsp_status, model.dsp_note, model.create_user, Common.CommonFunction.DateTimeToString(model.create_time), model.modify_user, Common.CommonFunction.DateTimeToString(model.modify_time));
            try
            {
                return dbaccess.execCommand(sbAll.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("DeliveryStorePlaceDao-->InsertDeliveryStorePlace-->" + ex.Message + "sql:" + sbAll.ToString(), ex);
            }
        }

        public int UpdateDeliveryStorePlace(DeliveryStorePlace model)
        {
            StringBuilder sbAll = new StringBuilder();
            sbAll.AppendFormat("update delivery_store_place set dsp_name='{0}',dsp_address='{1}',dsp_telephone='{2}',dsp_big_code='{3}',dsp_deliver_store='{4}',", model.dsp_name, model.dsp_address, model.dsp_telephone, model.dsp_big_code, model.dsp_deliver_store);
            sbAll.AppendFormat("dsp_note='{0}',modify_user='{1}',modify_time='{2}' where dsp_id={3} ",model.dsp_note,model.modify_user,Common.CommonFunction.DateTimeToString(model.modify_time),model.dsp_id);
            try
            {
                return dbaccess.execCommand(sbAll.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("DeliveryStorePlaceDao-->UpdateDeliveryStorePlace-->" + ex.Message + "sql:" + sbAll.ToString(), ex);
            }
        }

        public int DeleteDeliveryStorePlace(DeliveryStorePlaceQuery query)
        {
            StringBuilder sbAll = new StringBuilder();
            sbAll.AppendFormat("delete from delivery_store_place where dsp_id in ({0})",query.dsp_ids);
            try
            {
                return dbaccess.execCommand(sbAll.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("DeliveryStorePlaceDao-->DeleteDeliveryStorePlace-->" + ex.Message + "sql:" + sbAll.ToString(), ex);
            }
        }

        public int SelectDspName(DeliveryStorePlace model)
        {
            StringBuilder sbAll = new StringBuilder();
            sbAll.AppendFormat("select count(dsp_id) from delivery_store_place where dsp_name='{0}'", model.dsp_name);
            try
            {
                return int.Parse(dbaccess.getDataTable(sbAll.ToString()).Rows[0][0].ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("DeliveryStorePlaceDao-->SelectDspName-->" + ex.Message + "sql:" + sbAll.ToString(), ex);
            }
        }
    }
}
