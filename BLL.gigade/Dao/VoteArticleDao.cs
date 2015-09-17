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
    public class VoteArticleDao
    {
        private IDBAccess _access;
        private string connStr;
        string strSql = string.Empty;
        public VoteArticleDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
              this.connStr = connectionString;
        }
        public List<VoteArticleQuery> GetAll(VoteArticleQuery query, out int totalCount)
        {
            StringBuilder str = new StringBuilder();
            StringBuilder strall = new StringBuilder();
            StringBuilder strcounts = new StringBuilder();
            try
            {
                strcounts.AppendFormat("select count(va.article_id) as totalcounts  FROM vote_article va ");
                strall.AppendFormat("SELECT article_id,va.event_id,va.product_id,va.prod_link,p.product_name,va.user_id,m.user_name as name,article_content,va.vote_count,va.article_sort,va.article_start_time,va.article_end_time,va.article_show_start_time,va.article_show_end_time ");//
                strall.AppendFormat(",(select count(vd.article_id)from vote_detail vd where va.article_id=vd.article_id and vd.vote_status=1) as reception_count,article_status ");
                strall.AppendFormat(",article_title,article_banner,va.create_user,va.create_time,va.update_time,va.update_user,ve.event_name FROM vote_article va ");
                str.AppendFormat(" LEFT JOIN vote_event ve ON va.event_id=ve.event_id ");
                str.AppendFormat(" LEFT JOIN product p ON va.product_id=p.product_id ");
                str.AppendFormat(" LEFT JOIN users m ON va.user_id=m.user_id ");
                str.AppendFormat(" where 1=1 ");
                totalCount = 0;
                if (query.article_id > 0)
                {
                    str.AppendFormat(" and va.article_id='{0}' ", query.article_id);
                }
                if (query.event_id>0)//活動編號
                {
                    str.AppendFormat(" and va.event_id='{0}' ",query.event_id);
                }
                if (!string .IsNullOrEmpty(query.article_title))//文章標題
                {
                    str.AppendFormat(" and va.article_title like N'%{0}%' ", query.article_title);
                }
                if (query.date != 0)
                {
                    if (!string.IsNullOrEmpty(query.time_start))
                    {
                        if (!string.IsNullOrEmpty(query.time_end))
                        {
                            switch (query.date)
                            {
                                case 1:
                                    str.AppendFormat(" and va.create_time between'{0}' and '{1}' ", query.time_start, query.time_end);
                                    break;
                                case 2:
                                    str.AppendFormat(" and va.article_start_time between'{0}' and '{1}' ", query.time_start, query.time_end);
                                    break;
                                case 3:
                                    str.AppendFormat(" and va.article_end_time between'{0}' and '{1}' ", query.time_start, query.time_end);
                                    break;
                                case 4:
                                    str.AppendFormat(" and va.article_show_start_time between'{0}' and '{1}' ", query.time_start, query.time_end);
                                    break;
                                case 5:
                                    str.AppendFormat(" and va.article_show_end_time between'{0}' and '{1}' ", query.time_start, query.time_end);
                                    break;
                            }
                        }
                    }
                }

                totalCount = 0;
                if (query.IsPage)
                {
                    strcounts.Append(str.ToString());
                    System.Data.DataTable _dt = _access.getDataTable(strcounts.ToString());

                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalcounts"]);
                    }
                    str.AppendFormat("order by va.article_sort DESC limit {0},{1} ", query.Start, query.Limit);
                }
                else {
                    str.Append(" order by va.article_sort DESC  ");
                }
                strall.Append(str.ToString());
                return _access.getDataTableForObj<VoteArticleQuery>(strall.ToString() );
            }
            catch (Exception ex)
            {
                throw new Exception(" VoteArticleDao-->GetAll-->" + ex.Message + "sql:" + strall.ToString() + strcounts.ToString(), ex);
            }
        }

        public int Save(VoteArticleQuery m)
        {
            StringBuilder str = new StringBuilder();
            m.Replace4MySQL();
            try
            {
                m.Replace4MySQL();
                str.AppendFormat("insert into vote_article (event_id,user_id,article_content,article_status,article_title,article_banner,create_user,create_time,update_time,update_user,product_id,vote_count,article_sort,prod_link,article_start_time,article_end_time,article_show_start_time,article_show_end_time) Value('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')", m.event_id, m.user_id, m.article_content, m.article_status, m.article_title, m.article_banner, m.create_user, CommonFunction.DateTimeToString(m.create_time), CommonFunction.DateTimeToString(m.update_time), m.update_user, m.product_id, m.vote_count, m.article_sort, m.prod_link, CommonFunction.DateTimeToString(m.article_start_time),CommonFunction.DateTimeToString(m.article_end_time),  CommonFunction.DateTimeToString(m.article_show_start_time),CommonFunction.DateTimeToString(m.article_show_end_time));
                return _access.execCommand(str.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" VoteArticleDao-->Save-->" + ex.Message + "sql:"+ str.ToString(), ex);
            }
        }

        public int Update(VoteArticleQuery m)
        {
            StringBuilder str = new StringBuilder();
            m.Replace4MySQL();
            try
            {
                str.AppendFormat("update vote_article SET user_id='{0}',article_content='{1}',article_title='{2}',product_id='{3}',prod_link='{4}'  ", m.user_id, m.article_content, m.article_title, m.product_id,m.prod_link);
                str.AppendFormat(",article_banner='{0}',update_time='{1}',update_user='{2}'", m.article_banner, CommonFunction.DateTimeToString(m.update_time), m.update_user);
                str.AppendFormat(",event_id='{0}',vote_count='{1}',article_sort='{2}', article_start_time='{3}', article_end_time='{4}',article_show_start_time='{5}',article_show_end_time='{6}'   WHERE article_id='{7}';", m.event_id, m.vote_count, m.article_sort, CommonFunction.DateTimeToString(m.article_start_time), CommonFunction.DateTimeToString(m.article_end_time), CommonFunction.DateTimeToString(m.article_show_start_time), CommonFunction.DateTimeToString(m.article_show_end_time), m.article_id);
                return _access.execCommand(str.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" VoteArticleDao-->Update-->" + ex.Message + "sql:" + str.ToString(), ex);
            }
        }

        public int UpdateStatus(VoteArticleQuery m)
        {//變更狀態
            StringBuilder str = new StringBuilder();
            try
            {
                m.Replace4MySQL();
                if (m.article_id > 0)
                {
                    str.Append("set sql_safe_updates=0;");
                    str.AppendFormat("Update vote_article SET article_status='{0}',update_time='{1}',update_user='{2}' where article_id='{3}';", m.article_status, CommonFunction.DateTimeToString(m.update_time), m.update_user, m.article_id);
                    str.Append("set sql_safe_updates=1;");
                    //添加修改message表所有狀態
                    return _access.execCommand(str.ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(" VoteArticleDao-->UpdateStatus-->" + ex.Message + "sql:" + str.ToString(), ex);
            }
        }
        public List<VoteArticleQuery> GetArticle()
        {
            StringBuilder str = new StringBuilder();
            try
            {
                str.AppendFormat("SELECT article_id,CONCAT(article_id,'-',article_title) as article_title FROM vote_article  where article_status='1' order by article_id DESC; ");
                return _access.getDataTableForObj<VoteArticleQuery>(str.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" VoteArticleDao-->GetArticle-->" + ex.Message + "sql:" + str.ToString(), ex);
            }

        }

        public int SelectByArticleName(VoteArticleQuery m)
        {
            StringBuilder str = new StringBuilder();
            try
            {
                 str.AppendFormat("SELECT article_id,article_title FROM vote_article  where  article_title ='{0}' and article_id<>'{1}'",m.article_title,m.article_id);
                 return _access.getDataTable(str.ToString()).Rows.Count;
            }
            catch (Exception ex )
            {
                throw new Exception(" VoteArticleDao-->SelectByArticleName-->" + ex.Message + "sql:" + str.ToString(), ex);
            }
        }

        public int SelMaxSort(VoteArticleQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select  max(article_sort) as maxsort from vote_article where event_id='{0}';", query.event_id);
                DataTable _dt = _access.getDataTable(sql.ToString());
                if (_dt.Rows[0]["maxsort"].ToString()!="")
                {
                    return Convert.ToInt32(_dt.Rows[0]["maxsort"]) + 1;
                }
                else
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(" VoteArticleDao-->SelMaxSort-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}