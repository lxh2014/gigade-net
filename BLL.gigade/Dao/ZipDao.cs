using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using System.Data;
using System.Data.SqlClient;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao
{
    class ZipDao : IZipImplDao
    {
        private IDBAccess _accessMySql;
        string strSql = string.Empty;

        public ZipDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        #region IZipImplDao 成员
        public List<Zip> QueryBig(string strTopValue)
        {
            if (string.IsNullOrEmpty(strTopValue))
            {
                strSql = "select distinct big, bigcode from t_zip_code";
            }
            else
            {
                strSql = "select distinct big, bigcode from t_zip_code where bigcode = '" + strTopValue + "'";
            }
            return _accessMySql.getDataTableForObj<Zip>(strSql);
        }

        public List<Zip> QueryMiddle(string strTopValue)
        {
            if (string.IsNullOrEmpty(strTopValue))
            {
                strSql = "select distinct middle, middlecode from t_zip_code";
            }
            else
            {
                strSql = "select distinct middle, middlecode from t_zip_code where bigcode = '" + strTopValue + "'";
            }
            return _accessMySql.getDataTableForObj<Zip>(strSql);
        }

        public List<Zip> QuerySmall(string strTopValue, string topText)
        {
            if (string.IsNullOrEmpty(strTopValue))
            {
                strSql = "select zipcode, small from t_zip_code";
            }
            else
            {
                strSql = "select zipcode, small from t_zip_code where middlecode = '" + strTopValue + "' and middle = '" + topText + "'";
            }
            return _accessMySql.getDataTableForObj<Zip>(strSql);
        }

        #region 根據zipcode獲取city和zip區域
        //edit mengjuan0826j 2014/10/21
        public Zip QueryCityAndZip(string zipcode)
        {
            try
            {
                strSql = "select big,bigcode,middle,middlecode, zipcode, small from t_zip_code where zipcode = '" + zipcode + "'";
                return _accessMySql.getSinggleObj<Zip>(strSql);
            }

            catch (Exception ex)
            {
                throw new Exception("ZipDao-->QueryCityAndZip-->" + ex.Message + strSql.ToString(), ex);
            }

        }
        #endregion
        /// <summary>
        /// 查詢所有，匯成DataTable好篩選
        /// </summary>
        /// <param name="zip">通過實體追加查詢條件,方便複用</param>
        /// <param name="appendSql">追加的sql語句</param>
        /// <returns></returns>
        public DataTable ZipTable(Zip zip,String appendSql=null)
        {
            StringBuilder sqlClomn = new StringBuilder();
            StringBuilder sqlCondition = new StringBuilder();
            StringBuilder sqlTable = new StringBuilder();
            try
            {
                sqlClomn .AppendLine(@"select big,bigcode,middle,middlecode, zipcode, small  ");
                sqlTable.Append(" from t_zip_code");
                sqlCondition.Append(" where 1=1 ");
                return _accessMySql.getDataTable(sqlClomn.ToString() + sqlTable.ToString() + sqlCondition.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ZipDao-->ZipTable-->" + ex.Message + strSql.ToString(), ex);
            }
        }

        public DataTable GetZip()
        {

            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat("SELECT CONCAT(zipcode,' ',middle,' ',small) AS zipname,zipcode from t_zip_code;");
                return _accessMySql.getDataTable(strSql.ToString());
            }
            catch(Exception ex)
            {
                throw new Exception("ZipDao-->GetZip-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        public string Getaddress(int zipcode)
        {
            StringBuilder strSql = new StringBuilder();
            DataTable dt=new DataTable();
            try
            {
                strSql.AppendFormat("SELECT CONCAT(zipcode,' ',middle,' ',small) AS zipname from t_zip_code where zipcode='{0}';", zipcode);
                dt=_accessMySql.getDataTable(strSql.ToString());
                if(dt.Rows.Count>0)
                {
                    return dt.Rows[0][0].ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ZipDao-->Getaddress-->" + ex.Message + strSql.ToString(), ex);
            }
        }

        public List<Zip> GetZipList()
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat("SELECT big,bigcode FROM t_zip_code GROUP BY bigcode; ");
                return _accessMySql.getDataTableForObj<Zip>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ZipDao-->GetZipList-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        /// <summary>
        /// chaojie1124j add 搬移2015/09/29實現供應商細項的公司地址和發票地址
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<Zip> GetZipList(Zip query)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat("select big,middle,middlecode,zipcode,small from t_zip_code where 1=1");
                if (!string.IsNullOrEmpty(query.bigcode))
                {
                    strSql.AppendFormat(" and bigcode='{0}' ", query.bigcode);
                }
                if (!string.IsNullOrEmpty(query.middlecode))
                {
                    strSql.AppendFormat(" and middlecode='{0}' ", query.middlecode);
                }
                if (!string.IsNullOrEmpty(query.zipcode))
                {
                    strSql.AppendFormat(" and zipcode='{0}' ", query.zipcode);
                }
                return _accessMySql.getDataTableForObj<Zip>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ZipDao-->GetZipList-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion
    }
}
