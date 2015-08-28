using BLL.gigade.Dao.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using DBAccess;
using BLL.gigade.Common;
using System.Data;

namespace BLL.gigade.Dao
{
    public class TrialPictureDao : ITrialPictureImplDao
    {
          private IDBAccess  _access;
        private string connStr;
        public TrialPictureDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;

        }



        public List<TrialPictureQuery> QueryPic(TrialPictureQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("SELECT tp.share_id,tp.image_filename,tp.image_sort,tp.image_state,tp.image_createdate from trial_picture tp where tp.share_id={0};", query.share_id);
                return _access.getDataTableForObj<TrialPictureQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TrialRecordDao-->QueryPic-->" + sql.ToString() + ex.Message, ex);
            }
        }




        public string DeletePic(TrialPictureQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates=0;delete from trial_picture where share_id={0};set sql_safe_updates = 1; ", query.share_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("TrialPictureDao.DeletePic-->"+sql.ToString()+ex.Message,ex);
            }
        }

        public string SavePic(TrialPictureQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("insert into trial_picture (share_id,image_filename,image_sort,image_state,image_createdate) VALUES({0},'{1}',{2},{3},{4});", query.share_id, query.image_filename, query.image_sort, query.image_state, CommonFunction.GetPHPTime());
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("TrialPictureDao.SavePic-->"+sql.ToString()+ex.Message,ex);
            }
            throw new NotImplementedException();
        }


        public int QueryMaxId()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select  MAX(record_id) as record_id from trial_record;");
                DataTable _dt = _access.getDataTable(sql.ToString());
                return Convert.ToInt32(_dt.Rows[0]["record_id"]);
            }

            catch (Exception ex)
            {
                throw new Exception("TrialPictureDao.QueryMaxId-->" + sql.ToString() + ex.Message, ex);
            }
        }


        public string VerifyEmail(string email)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select user_name,user_gender from users where user_email='{0}';", email);
                DataTable _dt = _access.getDataTable(sql.ToString());
                if (_dt.Rows.Count>0)
                {
                    return _dt.Rows[0]["user_gender"].ToString()+";"+_dt.Rows[0]["user_name"];
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("TrialPictureDao.VerifyEmail-->" + sql.ToString() + ex.Message, ex);
            }
        }


        public int DeleteAllPic(TrialPictureQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates=0;delete from trial_picture where share_id={0};set sql_safe_updates = 1; ", query.share_id);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TrialPictureDao.DeleteAllPic-->" + sql.ToString() + ex.Message, ex);
            }
        }
    }
}
