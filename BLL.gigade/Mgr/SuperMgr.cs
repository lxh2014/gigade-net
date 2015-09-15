using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class SuperMgr
    {
        private SuperDao _superDao;

        #region 類SuperMgr構造函數
        // 構造
        /// <summary>
        /// 實例化一個SuperMgr
        /// </summary>
        public SuperMgr(string connectionString)
        {
            _superDao = new SuperDao(connectionString);
        }
        #endregion

        #region Super Excel匯出方法
        /// <summary>
        /// 根據用戶輸入的查詢語句，匯出查詢結果
        /// </summary>
        /// <returns>Super Excel匯出</returns>
        public DataTable SuperExportExcel(SuperQuery query, out int totalCount)//GetPersonMsg查询方法，dao層有方法重載
        {
            try
            {
                DataTable dt = new DataTable();
                dt = _superDao.SuperExportExcel(query, out totalCount);

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("SuperMgr-->SuperExportExcel-->" + ex.Message);
            }
        }
        #endregion

    }
}
