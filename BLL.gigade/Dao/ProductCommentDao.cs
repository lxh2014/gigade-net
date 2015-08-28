using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Common;
using System.Data;
namespace BLL.gigade.Dao
{
    public class ProductCommentDao : IProductCommentImplDao
    {
        private IDBAccess _access;
        public ProductCommentDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        #region 查詢評論根據查詢條件+List<Model.Query.ProductCommentQuery> Query(Model.Query.ProductCommentQuery store, out int totalCount)
        /// <summary>
        /// 查詢評論根據查詢條件
        /// </summary>
        /// <param name="store"></param>
        /// <param name="totalCount">總數</param>
        /// <returns></returns>
        public List<Model.Query.ProductCommentQuery> Query(Model.Query.ProductCommentQuery store, out int totalCount)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder strCondition = new StringBuilder();
            StringBuilder strSelect = new StringBuilder();
            StringBuilder strTemp = new StringBuilder();
            try
            {
                store.Replace4MySQL();
                totalCount = 0;
                strSelect.AppendFormat("select pc.comment_id as comment_id ,pc.product_id,p.product_name as product_name,vb.brand_name as brand_name,uc.user_id, uc.user_email as user_email,uc.user_name,pc.is_show_name ,replace(replace(cd.comment_info, char(10), ''), char(13), '')  as comment_info,cd.comment_detail_id as comment_detail_id,cd.comment_advice as comment_advice, cd.comment_answer as comment_answer,cd.status as status,cd.answer_is_show, FROM_UNIXTIME(cd.create_time) as create_time,cn.product_desc as product_desc,cn.seller_server as seller_server,cn.web_server as web_server,cn.logistics_deliver as logistics_deliver,cd.reply_time,mu.user_username 's_reply_user'");
                strCondition.AppendFormat(" from product_comment pc left join product p on p.product_id=pc.product_id left join comment_detail cd on cd.comment_id=pc.comment_id left join comment_num cn on cn.comment_id=pc.comment_id left join users uc on uc.user_id=pc.user_id left join manage_user mu on mu.user_id=cd.reply_user  ");
                strCondition.AppendFormat(" left join vendor_brand vb on p.brand_id=vb.brand_id");
                strCondition.AppendFormat(" where 1=1");
                StringBuilder strTotalCount = new StringBuilder(" select count(pc.comment_id) as totalCount ");
                if (store.product_name != "")
                {
                    strCondition.AppendFormat(" and p.product_name like '%{0}%'", store.product_name);
                }
                if (store.user_email != "")
                {
                    strCondition.AppendFormat(" and uc.user_email like '%{0}%'", store.user_email);
                }
                if (store.comment_type != -1)
                {
                    if (store.comment_type == 0)
                    {
                        //strCondition.AppendFormat(" and cn.product_desc >= 4");
                        strCondition.AppendFormat(" and cn.product_desc+cn.seller_server+cn.web_server+cn.logistics_deliver >= 15");
                        strCondition.AppendFormat(" and cn.product_desc+cn.seller_server+cn.web_server+cn.logistics_deliver <= 20");
                    }
                    else if (store.comment_type == 1)
                    {
                        //strCondition.AppendFormat(" and cn.product_desc >= 2 and cn.product_desc <= 3");
                        strCondition.AppendFormat(" and cn.product_desc+cn.seller_server+cn.web_server+cn.logistics_deliver >= 10");
                        strCondition.AppendFormat(" and cn.product_desc+cn.seller_server+cn.web_server+cn.logistics_deliver <= 14");
                    }
                    else if (store.comment_type == 2)
                    {
                        //strCondition.AppendFormat(" and cn.product_desc=1 ");
                        strCondition.AppendFormat(" and cn.product_desc+cn.seller_server+cn.web_server+cn.logistics_deliver >= 4");
                        strCondition.AppendFormat(" and cn.product_desc+cn.seller_server+cn.web_server+cn.logistics_deliver <= 9");
                    }
                }
                if (store.commentsel != 0)
                {
                    strCondition.AppendFormat(" and (cn.product_desc = {0} or cn.seller_server ={1} or cn.web_server={2} or cn.logistics_deliver ={3})", store.commentsel, store.commentsel, store.commentsel, store.commentsel);
                }
                if (store.beginTime != 0)
                {
                    strCondition.AppendFormat(" and cd.create_time>='{0}' ", store.beginTime);
                }
                if (store.endTime != 0)
                {
                    strCondition.AppendFormat(" and cd.create_time<='{0}' ", store.endTime);
                }
                if (store.brand_name != "")
                {
                    strCondition.AppendFormat(" and  vb.brand_name like '%{0}%' ", store.brand_name);
                }
                if (store.product_id != 0)
                {
                    strCondition.AppendFormat(" and  pc.product_id={0} ", store.product_id);
                }
                if (store.productIds != "")
                {
                    strCondition.AppendFormat(" and  p.prod_classify={0} ", store.productIds);
                }
                if (store.user_email != "")
                {
                    strCondition.AppendFormat(" and  uc.user_email like '%{0}%' ", store.user_email);
                }
                if (store.user_name != "")
                {
                    strCondition.AppendFormat(" and  uc.user_name like '%{0}%' ", store.user_name);
                }
                if (store.comment_id != 0)
                {
                    strCondition.AppendFormat(" and  pc.comment_id = '{0}' ", store.comment_id);
                }
                //comment_answer
                if (store.isReplay == 1)
                {
                    strCondition.AppendFormat(" and cd.comment_answer is null ", store.comment_answer);
                }
                else if (store.isReplay == 2)
                {
                    strCondition.AppendFormat(" and cd.comment_answer is not null ", store.comment_answer);
                }
                if (store.IsPage)
                {
                    strSql.AppendFormat(strTotalCount.ToString());
                    strSql.AppendFormat(strCondition.ToString());
                    System.Data.DataTable _dt = _access.getDataTable(strSql.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    strCondition.AppendFormat(" order by pc.comment_id desc ");
                    strCondition.AppendFormat(" limit {0},{1}", store.Start, store.Limit);
                    strCondition.AppendFormat(";");
                }
                strTemp.AppendFormat(strSelect.ToString());
                strTemp.AppendFormat(strCondition.ToString());
                strTemp.AppendFormat(";");
                strSql.AppendFormat(strTemp.ToString());
                return _access.getDataTableForObj<Model.Query.ProductCommentQuery>(strTemp.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("ProductCommentDao-->Query-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        #region 更改狀態+int UpdateActive(ProductCommentQuery model)
        /// <summary>
        /// 更改狀態
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateActive(ProductCommentQuery model)
        {
            string strSql = string.Empty;
            try
            {
                strSql = string.Format("set sql_safe_updates = 0; update comment_detail set status='{0}' where comment_id='{1}';update comment_num set status='{0}' where comment_id='{1}'; set sql_safe_updates = 1;", model.status, model.comment_id);
                return _access.execCommand(strSql);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductCommentDao-->UpdateActive-->" + ex.Message + strSql, ex);
            }
        }
        #endregion
        /// <summary>
        /// 評價回覆
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int ProductCommentSave(ProductCommentQuery query)
        {
            StringBuilder sql = new StringBuilder();
            query.Replace4MySQL();
            try
            {
                sql.Append("set sql_safe_updates = 0;");
                if (query.send_mail == 2)
                {
                    sql.AppendFormat(@"UPDATE table_change_log,
(SELECT max(row_id) as row_id from table_change_log where pk_id= '{0}'  and change_field = 'send_mail') row
 set new_value='2' WHERE table_change_log.row_id = row.row_id;", query.comment_detail_id);
                }
                else
                {
                    sql.AppendFormat("update comment_detail set comment_answer='{0}', answer_is_show='{1}',reply_time='{2}',reply_user='{3}'   where comment_id='{4}';", query.comment_answer, query.answer_is_show, CommonFunction.GetPHPTime(), query.reply_user, query.comment_id);
                    if(query.old_comment_answer!=query.comment_answer)
                    {
                        sql.AppendFormat("INSERT into table_change_log(user_type,pk_id,change_table,change_field,field_ch_name,old_value,new_value,create_user,create_time)values('1','{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}');", query.comment_detail_id, "comment_detail", "comment_answer", "回覆內容", query.old_comment_answer, query.comment_answer, query.reply_user, Common.CommonFunction.DateTimeToString(DateTime.Now));
                    }
                    if (query.old_answer_is_show!=query.answer_is_show.ToString()) //如果修改了是否顯示
                    {
                        sql.AppendFormat("INSERT into table_change_log(user_type,pk_id,change_table,change_field,field_ch_name,old_value,new_value,create_user,create_time)values('1','{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}');", query.comment_detail_id, "comment_detail", "answer_is_show", "回覆內容是否顯示", query.old_answer_is_show, query.answer_is_show.ToString(), query.reply_user, Common.CommonFunction.DateTimeToString(DateTime.Now));
                    }
                
                    if (query.send_mail==0) //如果發送郵件
                    {
                        sql.AppendFormat("INSERT into table_change_log(user_type,pk_id,change_table,change_field,field_ch_name,old_value,new_value,create_user,create_time)values('1','{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}');", query.comment_detail_id, "comment_detail", "send_mail", "是否發送郵件", "",query.send_mail.ToString(), query.reply_user, Common.CommonFunction.DateTimeToString(DateTime.Now));
                    }     
                }
                sql.AppendFormat("set sql_safe_updates = 1;");
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCommentDao-->ProductCommentSave-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #region  根據comment_id獲取用戶郵箱，姓名，購買商品，評價信息
        public ProductCommentQuery GetUsetInfo(Model.Query.ProductCommentQuery store)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(" select pc.comment_id as comment_id ,pc.product_id,p.product_name as product_name,vb.brand_name as brand_name,uc.user_id, uc.user_email as user_email,uc.user_name,cd.comment_info   ");
                sql.Append(" from product_comment pc left join product p on p.product_id=pc.product_id left join comment_detail cd on cd.comment_id=pc.comment_id left join comment_num cn on cn.comment_id=pc.comment_id left join users uc on uc.user_id=pc.user_id    ");
                sql.Append(" left join vendor_brand vb on p.brand_id=vb.brand_id ");
                sql.Append(" where 1=1    ");
                if (store.comment_id != 0)
                {
                    sql.AppendFormat(" and  pc.comment_id = '{0}'; ", store.comment_id);
                }
                return  _access.getSinggleObj<ProductCommentQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCommentDao-->GetUsetInfo-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 異動記錄

		public DataTable QueryTableName()
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("SELECT DISTINCT change_table as table_name from table_change_log ");
                return _access.getDataTable(strSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("ProductCommentDao-->QueryTableName-->" + ex.Message + strSql.ToString(), ex);
            }
        }

        public DataTable GetChangeLogList(ProductCommentQuery query, out int totalCount)
        {
            query.Replace4MySQL();
            StringBuilder strSql = new StringBuilder();
            StringBuilder SqlWhere = new StringBuilder();
            StringBuilder SqlCount = new StringBuilder();
            try
            {
                //SqlCount.AppendFormat("select count(tcl.pk_id) as totalCount ");
                strSql.AppendFormat(@"select  DISTINCT tcl.create_time,tcl.pk_id,tcl.create_user,comment_detail.comment_id,comment_detail.comment_info");
                SqlWhere.Append(" from table_change_log tcl LEFT JOIN comment_detail on tcl.pk_id = comment_detail.comment_detail_id WHERE 1=1 ");

                if (!string.IsNullOrEmpty(query.change_table))
                {
                    SqlWhere.AppendFormat(" and tcl.change_table='{0}'", query.change_table);
                }
                if (query.comment_id>0)
                {
                    SqlWhere.AppendFormat(" and comment_detail.comment_id ='{0}'", query.comment_id);
                }
                if (query.start_time != DateTime.MinValue && query.end_time != DateTime.MinValue)
                {
                    SqlWhere.AppendFormat(" and tcl.create_time between '{0}' and '{1}'", 
                      CommonFunction.DateTimeToString(query.start_time), CommonFunction.DateTimeToString(query.end_time));

                }
                SqlWhere.Append(" order by tcl.create_time desc");

                //strSql.AppendFormat("select row_id,pk_id,create_user,create_time from table_change_log  where 1=1 ");
                
                totalCount = 0;
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable(strSql.ToString() + SqlWhere.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                        SqlWhere.AppendFormat(" limit {0},{1};", query.Start, query.Limit);
                    }
                }
                return _access.getDataTable(strSql.ToString() + SqlWhere.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("ProductCommentDao-->GetChangeLogList-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        public DataTable GetChangeLogDetailList(int pk_id, string create_time)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {

                strSql.AppendFormat("select pk_id,change_table,change_field,field_ch_name,old_value,new_value from table_change_log  where pk_id={0} and create_time ='{1}' ", pk_id, create_time);
               return _access.getDataTable(strSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("ProductCommentDao-->GetChangeLogDetailList-->" + ex.Message + strSql.ToString(), ex);
            }
        }

	    #endregion

    }
}
