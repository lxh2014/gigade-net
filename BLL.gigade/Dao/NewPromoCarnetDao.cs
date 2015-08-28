using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class NewPromoCarnetDao : INewPromoCarnetImplDao
    {
        private IDBAccess _access;

        public NewPromoCarnetDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        public DataTable NewPromoCarnetList(NewPromoCarnetQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            totalCount = 0;
            try
            {
                sql.Append("select npc.row_id,npc.event_name,npc.event_desc,npc.message_mode,npc.message_content,npc.group_id,");
                sql.Append("npc.link_url,npc.promo_image, npc.promo_image as s_promo_image,npc.event_id,npc.device,npc.count_by,npc.count,case when npc.active_now=0 then '否' else '是' end as active_now , case when npc.new_user=0 then '否' else '是' end as new_user,npc.new_user_date,");
                sql.Append("npc.`start`,npc.`end`,npc.active,npc.kuser,muser,npc.created,npc.modified");
                sql.Append(",vug.group_name as group_name,npc.present_event_id ");
                sqlWhere.Append(" from new_promo_carnet npc ");
                sqlWhere.Append("LEFT JOIN vip_user_group vug on npc.group_id=vug.group_id ");
                sqlWhere.Append(" where npc.event_id!='' ");
                sqlcount.Append(" select count(npc.row_id) as totalCount ");
                if (query.event_id != "")
                {
                    sqlWhere.AppendFormat(" and npc.event_id='{0}' ", query.event_id);
                }
                else
                {
                    if (query.condition == 0)//已過期
                    {
                        sqlWhere.AppendFormat(" and npc.end<'{0}'", CommonFunction.DateTimeToString(DateTime.Now));
                    }
                    else
                    {
                        sqlWhere.AppendFormat(" and npc.end>'{0}'", CommonFunction.DateTimeToString(DateTime.Now));

                    }
                }
                sqlWhere.Append(" order by npc.row_id desc ");
                if (query.IsPage)
                {
                    sqlcount.Append(sqlWhere.ToString());
                    DataTable _dt = _access.getDataTable(sqlcount.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = int.Parse(_dt.Rows[0]["totalCount"].ToString());
                    }
                    sqlWhere.AppendFormat("limit {0},{1};", query.Start, query.Limit);
                }
                sql.Append(sqlWhere.ToString());
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoCarnetDao-->NewPromoCarnetList-->" + ex.Message + sql.ToString() + sqlcount.ToString(), ex);
            }
        }
        public string InsertNewPromoCarnet(NewPromoCarnetQuery store)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(@"insert into new_promo_carnet (event_name,event_desc,event_id,group_id,row_id,present_event_id,
link_url,promo_image,device,count_by,
count,active_now,new_user,new_user_date,
start,end,active,kuser,
muser,created,modified,message_mode,message_content )");
                sql.AppendFormat(@" values('{0}','{1}','{2}','{3}','{4}','{5}'", store.event_name, store.event_desc, store.event_id, store.group_id, store.row_id, store.present_event_id);
                sql.AppendFormat(@",'{0}','{1}','{2}','{3}'", store.link_url, store.promo_image, store.device, store.count_by);
                sql.AppendFormat(@",'{0}','{1}','{2}','{3}'", store.count, store.active_now, store.new_user, Common.CommonFunction.DateTimeToString(store.new_user_date));
                sql.AppendFormat(@",'{0}','{1}','{2}','{3}'", Common.CommonFunction.DateTimeToString(store.start), Common.CommonFunction.DateTimeToString(store.end), store.active, store.kuser);
                sql.AppendFormat(@",'{0}','{1}','{2}','{3}','{4}');", store.muser, Common.CommonFunction.DateTimeToString(store.created), Common.CommonFunction.DateTimeToString(store.modified), store.message_mode, store.message_content);


                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoCarnetDao.InsertNewPromoCarnet-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string UpdateNewPromoCarnet(NewPromoCarnetQuery store)
        {
            StringBuilder sql = new StringBuilder();
            try
            {


                sql.AppendFormat(@"set sql_safe_updates=0;update   new_promo_carnet set event_name='{0}',event_desc='{1}',present_event_id='{2}',group_id='{3}'", store.event_name, store.event_desc, store.present_event_id, store.group_id);
                sql.AppendFormat(@",link_url='{0}',promo_image='{1}',device='{2}',count_by='{3}'", store.link_url, store.promo_image, store.device, store.count_by);
                sql.AppendFormat(@",count='{0}',active_now='{1}',new_user='{2}',new_user_date='{3}'", store.count, store.active_now, store.new_user, Common.CommonFunction.DateTimeToString(store.new_user_date));
                sql.AppendFormat(@",start='{0}',end='{1}'", Common.CommonFunction.DateTimeToString(store.start), Common.CommonFunction.DateTimeToString(store.end));
                sql.AppendFormat(@",muser='{0}',modified='{1}',message_mode='{2}',message_content='{3}' where row_id='{4}';set sql_safe_updates=1;", store.muser, Common.CommonFunction.DateTimeToString(store.modified), store.message_mode, store.message_content, store.row_id);

                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoCarnetDao.UpdateNewPromoCarnet-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string UpdateActive(NewPromoCarnetQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates=0;update new_promo_carnet set active='{0}' where row_id='{1}';set sql_safe_updates=1;", query.active, query.row_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoCarnetDao.UpdateActive-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public int DeleteNewPromoCarnet(string row_ids)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" delete from  new_promo_carnet  where row_id in ({0});", row_ids);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoCarnetDao.DeleteNewPromoCarnet-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string GetModel(NewPromoCarnetQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select npc.row_id,npc.present_event_id,npc.event_name,npc.event_desc,npc.message_mode,npc.message_content,npc.group_id,npc.link_url,npc.promo_image,npc.event_id,npc.device,npc.count_by,npc.count,npc.active_now,npc.new_user,npc.new_user_date,npc.`start`,npc.`end`,npc.active,npc.kuser,muser,npc.created,npc.modified from new_promo_carnet npc where npc.row_id={0};", query.row_id);
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoCarnetDao.GetModel-->" + ex.Message + sql.ToString(), ex);
            }
            return sql.ToString();
        }


        public int GetNewPromoCarnetMaxId()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"select max(row_id) from new_promo_carnet ");
                DataTable _dt = _access.getDataTable(sql.ToString());
                if (_dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(_access.getDataTable(sql.ToString()).Rows[0][0]) + 1;
                }
                else
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoCarnetDao.GetNewPromoCarnetMaxId-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
