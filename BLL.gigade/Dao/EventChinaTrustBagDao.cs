using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao
{
   public  class EventChinaTrustBagDao
    {
       private IDBAccess _access;
       public EventChinaTrustBagDao(string connectionString)
       {
           _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
       }

       public List<EventChinaTrustBagQuery> EventChinaTrustBagList(EventChinaTrustBagQuery query, out int totalCount)
       {
           StringBuilder sql = new StringBuilder();
           StringBuilder sqlcount = new StringBuilder();
           StringBuilder sqlFrom = new StringBuilder();
           StringBuilder sqlWhere = new StringBuilder();
           totalCount = 0;
           try
           {
               sql.Append("select ecb.bag_id,ecb.bag_name,ecb.bag_desc,  ecb.bag_banner, ecb.bag_start_time,ecb.bag_end_time, ecb.bag_active, ecb.bag_create_user, ecb.bag_update_user, ecb.bag_create_time,ecb.bag_update_time,ecb.bag_show_start_time,  ecb.bag_show_end_time, ecb.event_id, ecb.product_number, mu1.user_username 'create_user' ,mu2.user_username 'update_user'");
               sqlFrom.Append("from event_chinatrust_bag ecb ");
               sqlFrom.Append(" LEFT JOIN manage_user mu1 on mu1.user_id=ecb.bag_create_user ");
               sqlFrom.Append(" LEFT JOIN manage_user mu2 on mu2.user_id=ecb.bag_update_user ");
               sqlWhere.Append(" where 1=1 ");
               sqlcount.Append("select count(ecb.bag_id) as totalCount ");
               if (!string.IsNullOrEmpty(query.event_id))
               {
                   sqlWhere.AppendFormat("and  ecb.event_id='{0}'  ", query.event_id);
               }
               if (query.bag_active != -1)
               {
                   sqlWhere.AppendFormat(" and  ecb.bag_active = '{0}' ", query.bag_active);
               }
               if (!string.IsNullOrEmpty(query.bag_name))
               {
                   sqlWhere.AppendFormat(" and  ecb.bag_name like N'%{0}%' ", query.bag_name);
               }
               if (query.date !=0)
               {
                   if (!string.IsNullOrEmpty(query.start_time))
                   {
                       if (query.end_time != "")
                       {
                           switch (query.date)
                           { 
                               case 1:
                                   sqlWhere.AppendFormat("  and  ecb.bag_start_time between'{0}' and '{1}'  ", CommonFunction.DateTimeToString(Convert.ToDateTime(query.start_time)),CommonFunction.DateTimeToString(Convert.ToDateTime(query.end_time)));
                                   break;
                               case 2:
                                   sqlWhere.AppendFormat("  and  ecb.bag_end_time between'{0}' and '{1}'  ", CommonFunction.DateTimeToString(Convert.ToDateTime(query.start_time)), CommonFunction.DateTimeToString(Convert.ToDateTime(query.end_time)));
                                   break;
                               case 3:
                                   sqlWhere.AppendFormat("  and  ecb.bag_show_start_time between'{0}' and '{1}'  ", CommonFunction.DateTimeToString(Convert.ToDateTime(query.start_time)), CommonFunction.DateTimeToString(Convert.ToDateTime(query.end_time)));
                                   break;
                               case 4:
                                   sqlWhere.AppendFormat("  and  ecb.bag_show_end_time between'{0}' and '{1}'  ", CommonFunction.DateTimeToString(Convert.ToDateTime(query.start_time)), CommonFunction.DateTimeToString(Convert.ToDateTime(query.end_time)));
                                   break;
                           }
                        
                       }
                   }
                  
               }           
               if (query.IsPage)
               {
                   sqlcount.Append(sqlFrom.ToString()).Append(sqlWhere.ToString());
                   DataTable dt = _access.getDataTable(sqlcount.ToString());
                   if (dt != null && dt.Rows.Count > 0)
                   {
                       totalCount = Convert.ToInt32(dt.Rows[0]["totalCount"]);
                   }
               }
               sqlWhere.AppendFormat("order by ecb.bag_id desc limit {0},{1} ", query.Start, query.Limit);
               sql.Append(sqlFrom.ToString()+sqlWhere.ToString());
               return _access.getDataTableForObj<EventChinaTrustBagQuery>(sql.ToString());
           }
           catch (Exception ex)
           {
               throw new Exception("EventChinaTrustBagDao-->EventChinaTrustBagList-->" + ex.Message + sql.ToString() + sqlcount.ToString(), ex);
           }
       }

       public int EventChinaTrustBagSave(EventChinaTrustBagQuery query)
       {
           StringBuilder sql = new StringBuilder();
           query.Replace4MySQL();
           try
           {
               if (query.bag_id == 0)//新增
               {
                   sql.Append("insert into event_chinatrust_bag (");
                   sql.Append("bag_name,bag_desc,bag_banner,bag_start_time,bag_end_time,");
                   sql.Append("bag_active,bag_create_user,bag_update_user,bag_create_time,bag_update_time,");
                   sql.Append("bag_show_start_time,bag_show_end_time,event_id,product_number) ");
                   sql.AppendFormat(" values('{0}','{1}','{2}','{3}','{4}',", query.bag_name, query.bag_desc, query.bag_banner, CommonFunction.DateTimeToString(query.bag_start_time), CommonFunction.DateTimeToString(query.bag_end_time));
                   sql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}',", query.bag_active, query.bag_create_user, query.bag_update_user, CommonFunction.DateTimeToString(query.bag_create_time), CommonFunction.DateTimeToString(query.bag_update_time));
                   sql.AppendFormat(" '{0}','{1}','{2}','{3}'); ", CommonFunction.DateTimeToString(query.bag_show_start_time), CommonFunction.DateTimeToString(query.bag_show_end_time), query.event_id, query.product_number);
               }
               else//編輯
               {
                   sql.AppendFormat("update event_chinatrust_bag set bag_name='{0}',bag_desc='{1}',bag_banner='{2}',bag_start_time='{3}',bag_end_time='{4}', event_id='{5}',",query.bag_name,query.bag_desc,query.bag_banner,CommonFunction.DateTimeToString(query.bag_start_time),CommonFunction.DateTimeToString(query.bag_end_time),query.event_id);
                   sql.AppendFormat("bag_update_user='{0}',bag_update_time='{1}', bag_show_start_time='{2}',bag_show_end_time='{3}',product_number='{4}' where bag_id='{5}';", query.bag_update_user, CommonFunction.DateTimeToString(query.bag_update_time), CommonFunction.DateTimeToString(query.bag_show_start_time), CommonFunction.DateTimeToString(query.bag_show_end_time), query.product_number, query.bag_id);
               }
               return _access.execCommand(sql.ToString());
           }
           catch (Exception ex)
           {
               throw new Exception("EventChinaTrustBagDao-->EventChinaTrustBagList-->"+ex.Message+sql.ToString(),ex);
           }
       }

       public EventChinaTrustBagQuery GetSinggleData(EventChinaTrustBagQuery query)
       {
           StringBuilder sql = new StringBuilder();
           StringBuilder sqlFrom = new StringBuilder();
           StringBuilder sqlWhere = new StringBuilder();
           try
           {
               sql.Append("select ecb.bag_id,ecb.bag_name,ecb.bag_desc,  ecb.bag_banner, ecb.bag_start_time,ecb.bag_end_time, ecb.bag_active, ecb.bag_create_user, ecb.bag_update_user, ecb.bag_create_time,ecb.bag_update_time,ecb.bag_show_start_time,  ecb.bag_show_end_time, ecb.event_id, ecb.product_number, mu1.user_username 'create_user' ,mu2.user_username 'update_user'");
               sqlFrom.Append("from event_chinatrust_bag ecb LEFT JOIN manage_user mu1 on mu1.user_id=ecb.bag_create_user LEFT JOIN manage_user mu2 on mu2.user_id=ecb.bag_update_user ");
               sqlWhere.AppendFormat(" where 1=1 and  ecb.bag_id='{0}'", query.bag_id);
               return _access.getSinggleObj<EventChinaTrustBagQuery>(sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
           }
           catch (Exception ex)
           {
               throw new Exception("EventChinaTrustBagDao-->GetSinggleData-->" + ex.Message + sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString(), ex);
           }
       }

       public int EventChinaTrustBagStatus(EventChinaTrustBagQuery query)
       {
           StringBuilder sql = new StringBuilder();
           try
           {
               sql.AppendFormat("update event_chinatrust_bag set bag_active='{0}',bag_update_user='{1}',bag_update_time='{2}' where bag_id='{3}';",query.bag_active,query.bag_update_user,CommonFunction.DateTimeToString(query.bag_update_time),query.bag_id);
               return _access.execCommand(sql.ToString());
           }
           catch (Exception ex)
           {
               throw new Exception("EventChinaTrustBagDao-->EventChinaTrustBagStatus-->" + ex.Message + sql.ToString(),ex);
           }
       }

       public List<EventChinaTrustBag> GetBag(EventChinaTrustBagQuery q)
       {
           StringBuilder str = new StringBuilder();
           try
           {
               //str.AppendFormat("SELECT article_id,CONCAT(article_id,'-',article_title) as article_title FROM vote_article  where article_status='1' order by article_id DESC; ");
               str.AppendFormat("SELECT bag_id, CONCAT(bag_id,'-',bag_name) as bag_name FROM event_chinatrust_bag  order by bag_id DESC; ");
               return _access.getDataTableForObj<EventChinaTrustBag>(str.ToString());
           }
           catch (Exception ex)
           {
               throw new Exception(" VoteArticleDao-->GetBag-->" + ex.Message + "sql:" + str.ToString(), ex);
           }
       }
    }
}
