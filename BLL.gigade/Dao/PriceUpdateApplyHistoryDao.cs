using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;

namespace BLL.gigade.Dao
{
    public class PriceUpdateApplyHistoryDao : IPriceUpdateApplyHistory
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public PriceUpdateApplyHistoryDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }

        #region IPriceUpdateApplyHistoryDao 成员

        public string Save(PriceUpdateApplyHistory pH)
        {
            pH.Replace4MySQL();
            StringBuilder stb = new StringBuilder("insert into price_update_apply_history");
            stb.Append("(`apply_id`,`user_id`,`create_time`,`price_status`,`type`,`remark`) values");
            stb.AppendFormat("({0},{1},now(),{2},{3},'{4}')", pH.apply_id, pH.user_id, pH.price_status, pH.type, pH.remark);
            return stb.ToString();
        }


        #endregion
    }
}
