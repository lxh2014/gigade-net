/*
* 文件名稱 :ChannelContactDao.cs
* 文件功能描述 :外站聯絡人
* 版權宣告 :
* 開發人員 : 天娥璇子
* 版本資訊 : 1.0
* 日期 : 2013/08/19
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using System.Data;
using System.Data.SqlClient;

namespace BLL.gigade.Dao
{
    class ChannelContactDao :IChannelContactImplDao
    {
        private IDBAccess _accessMySql;
        string strSql = string.Empty;

        public ChannelContactDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        #region IChannelContactImplDao 成员

        public List<ChannelContact> Query(string strChannelId)
        {

            strSql = string.Format("select rid,contact_type,contact_name,contact_phone1,contact_phone2,contact_mobile,contact_email from channel_contact where channel_id ='{0}'", strChannelId);
            return _accessMySql.getDataTableForObj<ChannelContact>(strSql);
        }

        public int Save(ChannelContact chc)
        {
            chc.Replace4MySQL();
            strSql = string.Format(@"insert into channel_contact(contact_type,contact_name,contact_phone1,contact_phone2,contact_mobile,contact_email,channel_id) 
                values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", chc.contact_type, chc.contact_name, chc.contact_phone1, chc.contact_phone2, chc.contact_mobile, 
                chc.contact_email, chc.channel_id);
            return _accessMySql.execCommand(strSql);
        }

        public int Edit(ChannelContact chc)
        {
            chc.Replace4MySQL();
            strSql = string.Format(@"update channel_contact set contact_type='{0}',contact_name='{1}',contact_phone1='{2}',contact_phone2='{3}',contact_mobile='{4}',contact_email='{5}'
                where rid='{6}'", chc.contact_type, chc.contact_name, chc.contact_phone1, chc.contact_phone2, chc.contact_mobile,
                chc.contact_email, chc.rid);
            return _accessMySql.execCommand(strSql);
        }

        public int Delete(int rid)
        {
            strSql = string.Format(@"delete from channel_contact where rid='{0}'", rid);
            return _accessMySql.execCommand(strSql);
        }

        #endregion
    }
}
