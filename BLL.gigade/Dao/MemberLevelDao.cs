using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Model.Query;
using DBAccess;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
   public class MemberLevelDao
    {
       private IDBAccess _access;
       public MemberLevelDao(string connectionString)
       {
           _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
       }

       public List<MemberLevelQuery> MemberLevelList(MemberLevelQuery query, out int totalCount )
       {
           StringBuilder sql = new StringBuilder();
           StringBuilder sqlFrom = new StringBuilder();
           StringBuilder sqlWhere = new StringBuilder();
           query.Replace4MySQL();
           totalCount = 0;
           try
           {
               sql.Append("select ml.rowID,ml.ml_code,ml.ml_name,ml.ml_seq,ml.ml_minimal_amount,ml_max_amount,ml.ml_month_seniority,ml.ml_last_purchase,ml.ml_minpurchase_times,ml.ml_birthday_voucher,ml.ml_shipping_voucher,ml.ml_status,ml.k_date,ml.k_user,ml.m_date,ml.m_user,mu1.user_username 'create_user',mu2.user_username 'update_user' ");
               sqlFrom.Append(" from member_level ml ");
               sqlFrom.Append(" LEFT JOIN manage_user mu1 on mu1.user_id=ml.k_user   ");
               sqlFrom.Append(" LEFT JOIN manage_user mu2 on mu2.user_id=ml.m_user  ");
               sqlWhere.Append(" where 1=1 ");
               if (!string.IsNullOrEmpty(query.code_name))
               {
                   sqlWhere.AppendFormat(" and (ml.ml_code='{0}' or ml.ml_name='{0}' )   ",query.code_name);
               }
               if (query.ml_status != -1)
               {
                   sqlWhere.AppendFormat(" and ml.ml_status='{0}' ",query.ml_status);
               }
               if (query.IsPage)
               {
                   DataTable dt = _access.getDataTable("select count(ml.rowID) as totalCount " + sqlFrom.ToString() + sqlWhere.ToString());
                   if (dt != null && dt.Rows.Count > 0)
                   {
                       totalCount = Convert.ToInt32(dt.Rows[0]["totalCount"]);
                   }
               }
               sqlWhere.AppendFormat("order by ml.ml_seq  limit {0},{1} ", query.Start, query.Limit);
               sql.Append(sqlFrom.ToString() + sqlWhere.ToString());
               return _access.getDataTableForObj<MemberLevelQuery>(sql.ToString());
           }
           catch (Exception ex)
           {
               throw new Exception("MemberLevelDao-->MemberLevelList-->" + ex.Message + sql.ToString(), ex);
           }
       }

       public int UpdateActive(MemberLevelQuery query)
       {
           StringBuilder sql = new StringBuilder();
           try
           {
               sql.AppendFormat("update member_level set ml_status='{0}',m_date='{1}',m_user='{2}' where rowID='{3}';", query.ml_status, CommonFunction.DateTimeToString(query.m_date), query.m_user, query.rowID);
               return _access.execCommand(sql.ToString());
           }
           catch (Exception ex)
           {
               throw new Exception("MemberLevelDao-->UpdateActive-->" + ex.Message + sql.ToString(), ex);
           }
       }

       public int SaveMemberLevel(MemberLevelQuery query)
       {
           StringBuilder sql = new StringBuilder();
           query.Replace4MySQL();
           try
           {
               if (query.rowID == 0)//新增
               {
                   sql.Append("insert into member_level(ml_code,ml_name,ml_seq,ml_minimal_amount,ml_max_amount,");
                   sql.Append("ml_month_seniority,ml_last_purchase,ml_minpurchase_times,ml_birthday_voucher,ml_shipping_voucher,");
                   sql.Append("k_date,k_user,m_date,m_user,ml_status)");
                   sql.AppendFormat("values('{0}','{1}','{2}','{3}','{4}',", query.ml_code, query.ml_name, query.ml_seq, query.ml_minimal_amount,query.ml_max_amount);
                   sql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}',", query.ml_month_seniority, query.ml_last_purchase, query.ml_minpurchase_times, query.ml_birthday_voucher,query.ml_shipping_voucher);
                   sql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}');",CommonFunction.DateTimeToString(query.k_date),query.k_user,CommonFunction.DateTimeToString(query.m_date),query.m_user,query.ml_status);
               }
               else//編輯
               {
                   sql.AppendFormat("update member_level set ml_code='{0}',ml_name='{1}',ml_seq='{2}',ml_minimal_amount='{3}',ml_max_amount='{4}',", query.ml_code, query.ml_name, query.ml_seq, query.ml_minimal_amount,query.ml_max_amount);
                   sql.AppendFormat(" ml_month_seniority='{0}',ml_last_purchase='{1}',ml_minpurchase_times='{2}',ml_birthday_voucher='{3}',ml_shipping_voucher='{4}', ", query.ml_month_seniority, query.ml_last_purchase, query.ml_minpurchase_times, query.ml_birthday_voucher,query.ml_shipping_voucher);
                   sql.AppendFormat(" m_date='{0}',m_user='{1}' where rowID={2};", CommonFunction.DateTimeToString(query.m_date), query.m_user, query.rowID);
               }
               return _access.execCommand(sql.ToString());
           }
           catch (Exception ex)
           {
               throw new Exception("MemberLevelDao-->SaveMemberLevel-->" + ex.Message + sql.ToString(), ex);
           }
       }

       public int MaxMLSeq()
       {
           StringBuilder sql = new StringBuilder();
           try
           {
               sql.Append("select max(ml_seq) as maxSeq from member_level ;");
               DataTable _dt = _access.getDataTable(sql.ToString());
               if (_dt.Rows[0]["maxSeq"].ToString() != "")
               {
                   return Convert.ToInt32(_dt.Rows[0]["maxSeq"]) + 1;
               }
               else
               {
                   return 1;
               }
           }
           catch (Exception ex)
           {
               throw new Exception("MemberLevelDao-->MaxMLSeq-->" + ex.Message + sql.ToString(), ex);
           }
       }

       public bool DistinctCode(MemberLevelQuery query)
       {
           StringBuilder sql = new StringBuilder();
           try
           {
               query.Replace4MySQL();
               sql.AppendFormat("select ml_code from member_level where ml_code='{0}'; ", query.ml_code);
               DataTable _dt = _access.getDataTable(sql.ToString());
               if (_dt.Rows.Count > 0)
               {
                   return true;
               }
               else
               {
                   return false;
               }
           }
           catch (Exception ex)
           {
               throw new Exception("MemberLevelDao-->DistinctCode-->" + ex.Message + sql.ToString(), ex);
           }
       }

       public bool DistinctSeq(MemberLevelQuery query)
       {

           StringBuilder sql = new StringBuilder();
           try
           {
               query.Replace4MySQL();
               sql.AppendFormat("select ml_seq from member_level where ml_seq='{0}'; ", query.ml_seq);
               DataTable _dt = _access.getDataTable(sql.ToString());
               if (_dt.Rows.Count > 0)
               {
                   return true;
               }
               else
               {
                   return false;
               }
           }
           catch (Exception ex)
           {
               throw new Exception("MemberLevelDao-->DistinctSeq-->" + ex.Message + sql.ToString(), ex);
           }
       }

       public DataTable GetLevel()
       {
           StringBuilder sql = new StringBuilder();
           try
           {
               sql.AppendFormat("select ml_code,ml_name from member_level where ml_status=1;");
               DataTable _dt = _access.getDataTable(sql.ToString());
               return _dt;
           }
           catch (Exception ex)
           {
               throw new Exception("MemberLevelDao-->DistinctSeq-->" + ex.Message + sql.ToString(), ex);
           }
       }
    }
}
