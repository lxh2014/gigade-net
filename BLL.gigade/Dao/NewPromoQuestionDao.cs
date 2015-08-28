using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
/**
 * chaojie1124j添加于2015/2/13活動模塊=> 問卷主表
 */
namespace BLL.gigade.Dao
{
    public class NewPromoQuestionDao : INewPromoQuestionImplDao
    {
        private IDBAccess _access;
        public NewPromoQuestionDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public DataTable GetPromoQuestionList(NewPromoQuestionQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();
            StringBuilder sqlCondition = new StringBuilder();
            try
            {
                sql.Append(@" select  row_id,event_name, q.present_event_id,event_desc,q.event_id,q.group_id,link_url,promo_image,
promo_image as s_promo_image,device,count_by,count,active_now, new_user,new_user_date,
start,end,active,kuser,muser,created,modified,vug.group_name as group_name ");

                sqlCondition.Append(" from new_promo_questionnaire q ");
                sqlCondition.Append("LEFT JOIN vip_user_group vug on q.group_id=vug.group_id ");
                sqlCondition.Append(" where q.event_id!='' ");
                sqlcount.Append("select count(row_id) as totalCount ");
                if (query.event_id != "")
                {
                    sqlCondition.AppendFormat(" and q.event_id='{0}' ", query.event_id);
                }
                else
                {
                    if (query.searchtype == 0)//已過期
                    {
                        sqlCondition.AppendFormat(" and end <'{0}' ", query.end.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        sqlCondition.AppendFormat(" and end >'{0}' ", query.end.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }
                
                totalCount = 0;
                if (query.IsPage)
                {
                    sqlcount.Append(sqlCondition.ToString());
                    DataTable _dt = _access.getDataTable(sqlcount.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = int.Parse(_dt.Rows[0]["totalCount"].ToString());
                    }
                    sqlCondition.Append(" order by row_id desc ");
                    sqlCondition.AppendFormat(" limit {0},{1} ", query.Start, query.Limit);
                }

                sql.Append(sqlCondition.ToString());

                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoQuestionDao.GetPromoQuestionList-->" + ex.Message + sql.ToString() + sqlcount.ToString(), ex);
            }

        }
        public List<NewPromoQuestionQuery> GetPromoQuestionList(NewPromoQuestionQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sql.Append(@" select  row_id,event_name, present_event_id,event_desc,event_id,group_id,link_url,promo_image,device,count_by,count,active_now,new_user,new_user_date,start,end,active,kuser,muser,created,modified ");

                sqlCondi.Append(" from new_promo_questionnaire  ");
                sqlCondi.Append(" where event_id!='' ");
                if (!string.IsNullOrEmpty(query.event_id))
                {
                    sqlCondi.AppendFormat(" and event_id='{0}'", query.event_id);
                }
                if (!string.IsNullOrEmpty(query.row_id_in))
                {
                    sqlCondi.AppendFormat(" and  row_id in({0}) ", query.row_id_in);
                }
                if (query.row_id != 0)
                {
                    sqlCondi.AppendFormat(" and  row_id ='{0}' ", query.row_id);
                }
                sqlCondi.Append(" order by row_id desc ");


                sql.Append(sqlCondi.ToString());

                return _access.getDataTableForObj<NewPromoQuestionQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoQuestionDao.GetPromoQuestionList-->" + ex.Message + sql.ToString(), ex);
            }

        }
        public string InsertNewPromoQuestion(NewPromoQuestionQuery store)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(@"insert into new_promo_questionnaire (row_id,present_event_id, event_name,event_desc,event_id,group_id,link_url,promo_image,device,count_by,count,active_now,new_user,new_user_date,start,end,active,kuser,muser,created,modified )");
                sql.AppendFormat(@" values('{0}','{1}','{2}','{3}','{4}','{5}'", store.row_id, store.present_event_id, store.event_name, store.event_desc, store.event_id, store.group_id);
                sql.AppendFormat(@",'{0}','{1}','{2}','{3}'", store.link_url, store.promo_image, store.device, store.count_by);
                sql.AppendFormat(@",'{0}','{1}','{2}','{3}'", store.count, store.active_now, store.new_user, Common.CommonFunction.DateTimeToString(store.new_user_date));
                sql.AppendFormat(@",'{0}','{1}','{2}','{3}'", Common.CommonFunction.DateTimeToString(store.start), Common.CommonFunction.DateTimeToString(store.end), store.active, store.kuser);
                sql.AppendFormat(@",'{0}','{1}','{2}');", store.muser, Common.CommonFunction.DateTimeToString(store.created), Common.CommonFunction.DateTimeToString(store.modified));
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoQuestionDao.InsertNewPromoQuestion-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public string UpdateNewPromoQuestion(NewPromoQuestionQuery store)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" update   new_promo_questionnaire set ");
                if (store.IsModified != 0)//新增
                {
                    sql.AppendFormat(@"  event_name='{0}',event_desc='{1}',start='{2}',end='{3}',", store.event_name, store.event_desc, Common.CommonFunction.DateTimeToString(store.start), Common.CommonFunction.DateTimeToString(store.end));
                }
                sql.AppendFormat(@" present_event_id='{0}',group_id='{1}'", store.present_event_id, store.group_id);
                sql.AppendFormat(@",link_url='{0}',promo_image='{1}',device='{2}',count_by='{3}'", store.link_url, store.promo_image, store.device, store.count_by);
                sql.AppendFormat(@",count='{0}',active_now='{1}',new_user='{2}',new_user_date='{3}'", store.count, store.active_now, store.new_user, Common.CommonFunction.DateTimeToString(store.new_user_date));
                sql.AppendFormat(@",active='{0}'", store.active);
                sql.AppendFormat(@",muser='{0}',modified='{1}' where row_id='{2}'", store.muser, Common.CommonFunction.DateTimeToString(store.modified), store.row_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoQuestionDao.UpdateNewPromoQuestion-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public string UpdateActive(NewPromoQuestionQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates=0;update new_promo_questionnaire set active='{0}' where row_id='{1}';set sql_safe_updates=1;", query.active, query.row_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoQuestionDao.UpdateActive-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public int DeleteNewPromoQuestion(string row_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"delete from  new_promo_questionnaire  where row_id in ( {0} ) ;", row_id);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoQuestionDao.DeleteNewPromoQuestion-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public string GetMaxRowId()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select max(row_id) as row_id from new_promo_questionnaire;");
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoQuestionDao.GetMaxRowId-->" + ex.Message + sql.ToString(), ex);
            }
        }


    }
}
