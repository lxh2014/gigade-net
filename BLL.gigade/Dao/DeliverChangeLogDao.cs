/*
* 文件名稱 :DeliverMasterDao.cs
* 文件功能描述 :營管--出貨查詢等操作
* 版權宣告 :
* 開發人員 : chaojie1124j
* 版本資訊 : 1.0
* 日期 : 2014/10/14
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using MySql.Data.MySqlClient;
 
namespace BLL.gigade.Dao
{
    public class DeliverChangeLogDao : IDeliverChangeLogImplDao 
    {
        private IDBAccess _access;
        private string connStr;
        public DeliverChangeLogDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }

        public int insertDeliverChangeLog(DeliverChangeLog dCL)
        {
            StringBuilder sbSql = new StringBuilder();
            dCL.Replace4MySQL();
            try
            {           
                sbSql.AppendFormat(@"insert into delivery_change_log (deliver_id,dcl_create_user,dcl_create_datetime,dcl_create_muser,dcl_create_type,
                                 dcl_note,dcl_ipfrom,expect_arrive_date,expect_arrive_period) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", dCL.deliver_id,
                                     dCL.dcl_create_user, dCL.dcl_create_datetime, dCL.dcl_create_muser, dCL.dcl_create_type,
                                     dCL.dcl_note, dCL.dcl_ipfrom, dCL.expect_arrive_date.ToString("yyyy-MM-dd"), dCL.expect_arrive_period);
                return _access.execCommand(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverChangeLogDao-->insertDeliverChangeLog-->" + ex.Message + sbSql.ToString(), ex);
            }          
        }       
    }
}
