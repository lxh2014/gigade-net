#region 文件信息
/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：UserIODao.cs 
 * 摘   要： 
 *      會員資料探勘與資料庫交互方法
 * 当前版本：v1.2 
 * 作   者： mengjuan0826j
 * 完成日期：2014/6/20
 * 修改歷史：
 *      v1.1修改日期：2014/9/22
 *      v1.1修改人員：zhejiang0304j
 *      v1.1修改内容：在抛出異常的時候將Sql語句抛出，合并代碼，添加注釋
 */
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using System.Data;
using BLL.gigade.Model.Custom;
using System.Text.RegularExpressions;

namespace BLL.gigade.Dao
{
    public class UserIODao : IUserIOImplDao
    {
        private IDBAccess _accessMySql;
        private string connStr;

        public UserIODao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connStr = connectionString;
        }
        #region 根據條件查詢會員信息+DataTable GetExcelTable(string sqlCondition)
        /// <summary>
        /// 根據條件查詢會員信息
        /// </summary>
        /// <param name="sqlCondition">查詢條件</param>
        /// <returns></returns>
        public DataTable GetExcelTable(string sqlCondition)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("select user_id '編號',user_email '信箱',user_name '姓名', (case user_gender when '1' then '男' else '女'END)as '性別',");
                // string.Format("concat(user_birthday_year,'/',user_birthday_month,'/',user_birthday_day) as '生日',concat(" + "'" + ",user_mobile," + "'" + ") AS'行動電話',concat(" + "'" + ",user_phone," + "'" + ") AS '聯絡電話',user_zip'郵遞區號',");
                strSql.Append("concat(user_birthday_year,'/',user_birthday_month,'/',user_birthday_day) as '生日',");
                strSql.Append("  concat(" + '"' + "'" + '"' + ",user_mobile) AS'行動電話',");
                strSql.Append("   concat(" + '"' + "'" + '"' + ",user_phone) AS '聯絡電話',");
                strSql.Append("user_zip'郵遞區號',concat(middle,big,small,user_address)'地址',FROM_UNIXTIME(user_reg_date)'註冊日期' from users left join t_zip_code on users.user_zip=t_zip_code.zipcode");
                strSql.AppendFormat(" where user_id in ( {0} ) order by user_id;", sqlCondition);
                return _accessMySql.getDataTable(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UserIODao-->GetExcelTable-->" + ex.Message + strSql, ex);
            }
        }
        #endregion
    }
}
