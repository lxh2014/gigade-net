using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class GroupAuthMapMgr:IGroupAuthMapImplMgr
    {
        private IGroupAuthMapImplDao _groupAuthMapImplDao;
        public GroupAuthMapMgr(string connectionStr)
        {
            _groupAuthMapImplDao = new GroupAuthMapDao(connectionStr);
        }
        public string Query(GroupAuthMapQuery query)
        {
            DataTable dt = _groupAuthMapImplDao.Query(query);
            StringBuilder sbResult = new StringBuilder();
            string column = String.Empty;
           
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (0 == i)
                {
                    sbResult.AppendFormat(" and ({0}.{1}='{2}' ", dt.Rows[i]["table_alias_name"].ToString(), dt.Rows[i]["column_name"].ToString(), dt.Rows[i]["value"].ToString());
                }
                else
                {
                    if (column == dt.Rows[i]["column_name"].ToString())
                    {
                        sbResult.AppendFormat(" or {0}.{1}='{2}' ", dt.Rows[i]["table_alias_name"].ToString(), dt.Rows[i]["column_name"].ToString(), dt.Rows[i]["value"].ToString());
                    }
                    else
                    {
                        sbResult.Append(")");
                        sbResult.AppendFormat(" and( {0}.{1}='{2}' ", dt.Rows[i]["table_alias_name"].ToString(), dt.Rows[i]["column_name"].ToString(), dt.Rows[i]["value"].ToString());
                    }
                }

                column = dt.Rows[i]["column_name"].ToString();
                if (i == dt.Rows.Count - 1)
                {
                    sbResult.Append(")");
                }
            }
            return sbResult.ToString();
           
        }
        public List<GroupAuthMapQuery> QueryAll(GroupAuthMapQuery m, out int totalCount)
        {
            try
            {
                return _groupAuthMapImplDao.QueryAll(m, out totalCount);
            }
            catch (Exception ex)
            {
               throw new Exception("GroupAuthMapMgr-->QueryAll-->" + ex.Message, ex);
            }
        }
        public int AddGroupAuthMapQuery(GroupAuthMapQuery query)
        {
            try
            {
                return _groupAuthMapImplDao.AddGroupAuthMapQuery(query);
            }
            catch (Exception ex)
            {
                throw new Exception("GroupAuthMapMgr-->AddGroupAuthMapQuery-->" + ex.Message, ex);
            }
        }
        public int UpGroupAuthMapQuery(GroupAuthMapQuery query) 
        {
            try
            {
                return _groupAuthMapImplDao.UpGroupAuthMapQuery(query);
            }
            catch (Exception ex)
            {
                throw new Exception("GroupAuthMapMgr-->UpGroupAuthMapQuery-->" + ex.Message, ex);
            }
        }
        public int UpStatus(int content_id, int status) 
        {
            try
            {
                return _groupAuthMapImplDao.UpStatus(content_id, status);
            }
            catch (Exception ex)
            {
                throw new Exception("GroupAuthMapMgr-->UpStatus-->" + ex.Message, ex);
            }
        }
    }
}
