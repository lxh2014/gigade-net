using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class AppcategoryDao : IAppcategoryImplDao
    {
        private IDBAccess _access;
        private string connStr;
        public AppcategoryDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }
        /// <summary>
        /// 查找列表
        /// </summary>
        /// <param name="appgory"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<Appcategory> GetAppcategoryList(Appcategory appgory, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            sql.AppendFormat("SELECT category_id,category,brand_id,brand_name,category1,category2,category3,product_id,Property FROM appcategory where 1=1 ");
            sqlcount.AppendFormat("SELECT category_id FROM appcategory where 1=1 ");
            if (!string.IsNullOrEmpty(appgory.category))
            {
                sqlwhere.AppendFormat(" and appcategory.category='{0}'", appgory.category);
            }
            if (!string.IsNullOrEmpty(appgory.category1))
            {
                sqlwhere.AppendFormat(" and appcategory.category1='{0}'", appgory.category1);
            }
            if (!string.IsNullOrEmpty(appgory.category2))
            {
                sqlwhere.AppendFormat(" and appcategory.category2='{0}'", appgory.category2);
            }
            if (!string.IsNullOrEmpty(appgory.category3))
            {
                sqlwhere.AppendFormat(" and appcategory.category3='{0}'", appgory.category3);
            }
            if (appgory.product_id != 0)
            {
                sqlwhere.AppendFormat(" and appcategory.product_id='{0}'", appgory.product_id);
            }
            totalCount = 0;
            try
            {
                sqlcount.AppendFormat(sqlwhere.ToString());
                sql.AppendFormat(sqlwhere.ToString());
                if (appgory.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sqlcount.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }
                    sql.AppendFormat(" limit {0},{1}", appgory.Start, appgory.Limit);
                }
                return _access.getDataTableForObj<Appcategory>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AppcategoryDao.GetAppcategoryList-->" + ex.Message + sql.ToString(), ex);
            }
        }
        /// <summary>
        /// 查找參數
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public List<Appcategory> GetParaList(string sql)
        {
            try
            {
                return _access.getDataTableForObj<Appcategory>(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("AppcategoryDao.GetParaList-->" + ex.Message + sql.ToString(), ex);
            }
        }
        /// <summary>
        /// 刪除數據
        /// </summary>
        /// <param name="appgory"></param>
        /// <returns></returns>
        public int AppcategoryDelete(Appcategory appgory)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("delete from appcategory where category_id ='{0}'", appgory.category_id);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AppcategoryDao-->AppcategoryDelete-->" + ex.Message + sql.ToString(), ex);
            }
        }

        /// <summary>
        /// 保存數據
        /// </summary>
        /// <param name="appgory"></param>
        /// <returns></returns>
        public int AppcategorySave(Appcategory appgory)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("insert into appcategory (category,brand_id,brand_name,category1,category2,category3,product_id,property)values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')",appgory.category,appgory.brand_id,appgory.brand_name,appgory.category1,appgory.category2,appgory.category3,appgory.product_id,appgory.property);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AppcategoryDao-->AppcategorySave-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
