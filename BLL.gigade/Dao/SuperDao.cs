using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class SuperDao
    {
        private IDBAccess _access;

        #region 類SuperDao構造函數
        // 構造
        /// <summary>
        /// 實例化一個SuperDao
        /// </summary>
        public SuperDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        #endregion

        #region Super Excel匯出方法
        /// <summary>
        /// 根據用戶輸入的查詢語句，匯出查詢結果
        /// </summary>
        /// <returns>Super Excel匯出</returns>
        public DataTable SuperExportExcel(SuperQuery query, out int totalCount)//  數據總數 ，mgr層也有此方法，
        {
            StringBuilder str = new StringBuilder();

            totalCount = 0;
            try
            {

                if (!string.IsNullOrEmpty(query.superSql))
                {
                    str.Append(query.superSql);
                }
                DataTable myDataTable = _access.getDataTable(str.ToString());
                totalCount = myDataTable.Rows.Count;

                return myDataTable;
            }
            catch (Exception ex)
            {
                throw new Exception("SuperDao-->SuperExportExcel-->" + ex.Message);

            }
        }
        #endregion

    }
}
