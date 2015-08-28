using BLL.gigade.Common;
using BLL.gigade.Model;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class GroupCommitteContactDao
    {
        private IDBAccess _accessMySql;
        public GroupCommitteContactDao(string connectionStr)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }
        public string GetGroupCommitteContact(GroupCommitteContact query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT gcc_id,group_id,gcc_chairman,gcc_phone,gcc_mail FROM group_committe_contact WHERE group_id={0}", query.group_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(" GroupCommitteContactDao -->GetGroupCommitteContact " + ex.Message, ex);
            }
        }
        public string SaveGCC(GroupCommitteContact query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"INSERT INTO group_committe_contact (group_id,gcc_chairman,gcc_phone,gcc_mail,k_user,k_date,m_user,m_date) VALUES({0},'{1}','{2}','{3}',{4},'{5}',{6},'{7}')", query.group_id, query.gcc_chairman, query.gcc_phone, query.gcc_mail, query.k_user, CommonFunction.DateTimeToString(query.k_date), query.m_user, CommonFunction.DateTimeToString(query.m_date));
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(" GroupCommitteContactDao -->SaveGCC " + ex.Message, ex);
            }
        }
        public string UpdateGCC(GroupCommitteContact query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"UPDATE group_committe_contact SET gcc_chairman='{0}',gcc_phone={1},gcc_mail='{2}',m_user='{3}',m_date='{4}' WHERE gcc_id={5}", query.gcc_chairman, query.gcc_phone, query.gcc_mail, query.m_user, CommonFunction.DateTimeToString(query.m_date), query.gcc_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(" GroupCommitteContactDao -->SaveGCC " + ex.Message, ex);
            }
        }
        public string DeleteGCC(GroupCommitteContact query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"DELETE FROM group_committe_contact where gcc_id={0}", query.gcc_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(" GroupCommitteContactDao -->SaveGCC " + ex.Message, ex);
            }
        }
    }
}
