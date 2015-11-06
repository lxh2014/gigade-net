using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using System.Data;
using MySql.Data.MySqlClient;


namespace BLL.gigade.Dao
{
    public class UserLifeDao
    {
        private IDBAccess _access;
        public UserLifeDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public List<Model.UserLife> GetUserLife(uint user_id)
        {
            StringBuilder sql = new StringBuilder();

            try
            {
                sql.Append(@"select row_id, user_id,info_type, info_code,info_name,remark,kdate,kuser");
                sql.AppendFormat(" from user_life where user_id='{0}';", user_id);
                return _access.getDataTableForObj<Model.UserLife>(sql.ToString());
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Number.ToString() + ":UserLifeDao-->GetUserLife" + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("UserLifeDao-->GetUserLife-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string SaveUserLife(BLL.gigade.Model.UserLife model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();

            sql.AppendFormat(@"insert into user_life (user_id,info_type, info_code,info_name,remark,kdate,kuser) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}');",
                model.user_id, model.info_type, model.info_code, model.info_name, model.remark, model.kdate, model.kuser);

            return sql.ToString();
        }
        public string DeleteUserLife(uint user_id)
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendFormat("delete from user_life where user_id='{0}' and info_type not in ('cancel_edm_time','disable_time','cancel_info_time');", user_id);

            return sql.ToString();
        }
        public string UpdateEdmTime(uint user_id, uint time, int modified)
        {
            StringBuilder sql = new StringBuilder();
            if (time == 0)
            {
                sql.AppendFormat("delete from user_life where user_id='{0}' and info_type='cancel_edm_time';", user_id);
            }
            else
            {
                Model.UserLife tModel = new Model.UserLife();
                tModel.user_id = user_id;
                tModel.info_type = "cancel_edm_time";
                tModel.info_code = time.ToString();
                tModel.info_name = "取消電子報時間";
                tModel.kdate = time;
                tModel.kuser = modified;
                sql.Append(SaveUserLife(tModel));

            }
            return sql.ToString();
        }
        public string UpdateDisableTime(uint user_id, uint time, int modified)
        {
            StringBuilder sql = new StringBuilder();
            if (time == 0)
            {
                sql.AppendFormat("delete from user_life where user_id='{0}' and info_type='disable_time';", user_id);
            }
            else
            {
                Model.UserLife tModel = new Model.UserLife();
                tModel.user_id = user_id;
                tModel.info_type = "disable_time";
                tModel.info_code = time.ToString();
                tModel.info_name = "禁用會員時間";
                tModel.kdate = time;
                tModel.kuser = modified;
                sql.Append(SaveUserLife(tModel));

            }
            return sql.ToString();
        }
        public string UpdateCancelTime(uint user_id, uint time, int modified)
        {
            StringBuilder sql = new StringBuilder();
            if (time == 0)
            {
                sql.AppendFormat("delete from user_life where user_id='{0}' and info_type='cancel_info_time';", user_id);
            }
            else
            {
                Model.UserLife tModel = new Model.UserLife();
                tModel.user_id = user_id;
                tModel.info_type = "cancel_info_time";
                tModel.info_code = time.ToString();
                tModel.info_name = "取消發送簡訊廣告時間";
                tModel.kdate = time;
                tModel.kuser = modified;
                sql.Append(SaveUserLife(tModel));

            }
            return sql.ToString();
        }
        public Model.UserLife GetSingle(uint user_id, string info_type)
        {
            StringBuilder sql = new StringBuilder();

            try
            {
                sql.Append(@"select row_id, user_id,info_type, info_code,info_name,remark,kdate,kuser");
                sql.AppendFormat(" from user_life where user_id='{0}' and info_type='{1}';", user_id, info_type);
                return _access.getSinggleObj<Model.UserLife>(sql.ToString());
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Number.ToString() + ":UserLifeDao-->GetSingle" + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("UserLifeDao-->GetSingle-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
