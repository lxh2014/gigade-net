using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class IpoDao : IIpoImplDao
    {
         private IDBAccess _access;
        string strSql = string.Empty;
        public IpoDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        /// <summary>
        /// 採購單,列表頁
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalcount"></param>
        /// <returns></returns>
        public List<IpoQuery> GetIpoList(IpoQuery query, out int totalcount)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sql.Append(@" select  i.row_id,mu.user_username,i.po_id,i.vend_id,i.buyer,i.sched_rcpt_dt,i.po_type,i.po_type_desc,i.cancel_dt,i.msg1,i.msg2,i.msg3,i.create_user,i.create_dtim,i.`status`  ");
                sqlCondi.Append(" from ipo i ");
                //sqlCondi.Append(" left join vendor v on v.vendor_id=i.vend_id ");
                sqlCondi.Append(" left join manage_user mu on mu.user_id=i.create_user ");
                sqlCondi.Append(" where 1=1 and i.status=1 ");
                if (!string.IsNullOrEmpty(query.po_id))
                {
                    sqlCondi.AppendFormat(" and i.po_id ='{0}' ", query.po_id);
                }
                if (!string.IsNullOrEmpty(query.po_type))
                {
                    sqlCondi.AppendFormat(" and i.po_type ='{0}' ", query.po_type);
                }
                totalcount = 0;
                sqlCondi.Append(" order by i.row_id desc ");
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable("select count(po_id) as totalCount " + sqlCondi.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalcount = int.Parse(_dt.Rows[0]["totalCount"].ToString());
                    }
                    sqlCondi.AppendFormat(" limit {0},{1} ", query.Start, query.Limit);
                }

                sql.Append(sqlCondi.ToString());

                return _access.getDataTableForObj<IpoQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IpoDao.GetIpoList-->" + ex.Message + sql.ToString(), ex);
            }

        }
        /// <summary>
        /// 新增標頭
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int AddIpo(IpoQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(" insert into ipo(po_id,vend_id,buyer,sched_rcpt_dt,po_type,po_type_desc,cancel_dt,msg1,msg2,msg3,create_user,create_dtim,change_user,change_dtim,`status`)  ");
                sb.AppendFormat(@" values('{0}','{1}','{2}','{3}','{4}','{5}'", query.po_id, query.vend_id, query.buyer, Common.CommonFunction.DateTimeToString(query.sched_rcpt_dt), query.po_type, query.po_type_desc);
                sb.AppendFormat(@",'{0}','{1}','{2}','{3}','{4}','{5}'",  Common.CommonFunction.DateTimeToString(query.cancel_dt), query.msg1, query.msg2, query.msg3, query.create_user, Common.CommonFunction.DateTimeToString(query.create_dtim));
                sb.AppendFormat(@",'{0}','{1}','{2}');", query.change_user, Common.CommonFunction.DateTimeToString(query.change_dtim), query.status);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IpoDao.AddIpo-->" + ex.Message + sb.ToString(), ex);
            }
        }
        /// <summary>
        /// 編輯標頭
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int UpdateIpo(IpoQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("set sql_safe_updates=0;");
                sb.AppendFormat(" update ipo set po_id='{0}',vend_id='{1}',buyer='{2}',sched_rcpt_dt='{3}',po_type='{4}' ", query.po_id, query.vend_id, query.buyer, Common.CommonFunction.DateTimeToString(query.sched_rcpt_dt), query.po_type);
                sb.AppendFormat(" ,po_type_desc='{0}',cancel_dt='{1}',msg1='{2}',msg2='{3}',msg3='{4}',change_user='{5}',change_dtim='{6}' where row_id='{7}';", query.po_type_desc, Common.CommonFunction.DateTimeToString(query.cancel_dt), query.msg1, query.msg2, query.msg3, query.change_user,Common.CommonFunction.DateTimeToString(query.change_dtim),query.row_id);
                sb.AppendFormat("set sql_safe_updates=1;");
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IpoDao.UpdateIpo-->" + ex.Message + sb.ToString(), ex);
            }
        }
        /// <summary>
        /// 刪除採購單單頭的多個數據
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int DeletIpo(IpoQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(" update ipo set status=0,cancel_dt='{0}' where row_id in({1});", Common.CommonFunction.DateTimeToString(DateTime.Now), query.row_ids);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IpodDao.GetPodID-->" + ex.Message + sb.ToString(), ex);
            }
        }

        public int SelectIpoCountByIpo(string ipo)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(" SELECT count(po_id) number FROM ipo WHERE po_id='{0}' and status=1 ;",ipo);
               
                DataTable dt = _access.getDataTable(sb.ToString());
                if (dt.Rows.Count > 0 && !string.IsNullOrEmpty(dt.Rows[0]["number"].ToString()))
                {

                   return  Convert.ToInt32(dt.Rows[0]["number"].ToString());
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception("IpoDao.SelectIpoCountByIpo-->" + ex.Message + sb.ToString(), ex);
            }
        }
    }
}
