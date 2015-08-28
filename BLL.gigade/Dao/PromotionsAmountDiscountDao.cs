#region 文件信息
/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：PromotionsAmountDiscountDao.cs 
 * 摘   要： 
 *      幾件幾元、幾件幾折與資料庫交互方法
 * 当前版本：v1.2 
 * 作   者： shuangshuang0420j
 * 完成日期：2014/6/20
 * 修改歷史：
 *      v1.1修改日期：2014/8/15
 *      v1.1修改人員：hongfei0416j
 *      v1.1修改内容：在抛出異常的時候將Sql語句抛出，合并代碼，添加注釋
 *      v1.2修改日期：2014/08/26
 *      v1.2修改人員：hongfei0416j
 *      v1.2修改内容：主要更改網prod_promo表中插入數據的時候的處理
 */
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using System.Data;
using BLL.gigade.Common;
using BLL.gigade.Model;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;
using System.Collections;
 
namespace BLL.gigade.Dao
{
    public class PromotionsAmountDiscountDao : IPromotionsAmountDiscountImplDao
    {
        private IDBAccess _access;
        private string connStr;
        ProductCategoryDao _proCateDao = null;
        ProductCategorySetDao _prodCategSet = null;
        ProdPromoDao _prodpromoDao = null;
        PromoAllDao _proAllDao = null;
        UserConditionDao _usconDao = null;
        PromoDiscountDao _promoDisDao = null;
        private MySqlDao _mySql;

        #region 有參構造函數
        /// <summary>
        /// 有參構造函數
        /// </summary>
        /// <param name="connectionstring">數據庫連接字符串</param>
        public PromotionsAmountDiscountDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
            _prodpromoDao = new ProdPromoDao(connectionstring);
            _proCateDao = new ProductCategoryDao(connectionstring);
            _prodCategSet = new ProductCategorySetDao(connectionstring);
            _proAllDao = new PromoAllDao(connectionstring);
            _usconDao = new UserConditionDao(connectionstring);
            _promoDisDao = new PromoDiscountDao(connectionstring);
            _mySql = new MySqlDao(connectionstring);
        }
        #endregion

        #region 查詢數據 +List<PromotionAmountDiscountQuery> Query(PromotionAmountDiscountQuery query, out int totalCount, string eventtype)
        /// <summary>
        /// 查詢數據 
        /// </summary>
        /// <param name="query">查詢參數</param>
        /// <param name="totalCount">查詢出來的數量</param>
        /// <param name="eventtype"></param>
        /// <returns>返回列表所需數據</returns>
        public List<PromotionAmountDiscountQuery> Query(PromotionAmountDiscountQuery query, out int totalCount, string eventtype)
        {
            query.Replace4MySQL();
            StringBuilder condition = new StringBuilder();
            StringBuilder TempCol = new StringBuilder();
            try
            {
                TempCol.Append(@"select CONCAT(pad.event_type ,right(CONCAT('00000000',pad.id),6)) as 'event_id',pad.url_by, pad.id,
                                pad.name,pad.active*1 as 'active', pad.event_desc ,pad.class_id,pad.brand_id,pad.amount,pad.quantity,pad.discount,pad.product_id,
                                pc.category_id,pc.banner_image,pc.category_link_url 'category_link_url',pad.group_id,tp.parameterName 'devicename',
                                pad.condition_id,uc.condition_name,pad.vendor_coverage,pad.site as 'siteId',pad.start 'startdate',pad.end 'enddate',pad.muser,mu.user_username ");
                condition.Append("  from  promotions_amount_discount  pad LEFT JOIN manage_user mu ON pad.muser=mu.user_id ");
                condition.Append(" left join product_category  pc on pc.category_id=pad.category_id ");
                condition.Append(" left join user_condition  uc on uc.condition_id =pad.condition_id ");
                condition.Append(" left join (select parameterCode,parameterName from t_parametersrc where parameterType='device'  ) tp on tp.parameterCode=pad.device ");

                condition.AppendFormat(" where pad.status=1 ");
                if (query.url_by != -1)
                {
                    condition.AppendFormat(" and pad.url_by = '{0}' ", query.url_by);
                }

                if (query.id > 0)
                {
                    condition.AppendFormat(" and pad.id = '{0}' ", query.id);
                }
                if (eventtype != "")
                {
                    if (eventtype == "M1")
                    {
                        condition.AppendFormat(" and (pad.event_type='{0}'  OR pad.event_type='') ", eventtype);
                    }
                    else
                    {
                        condition.AppendFormat(" and pad.event_type='{0}' ", eventtype);
                    }
                }

                if (query.expired == 0)//是未過期
                {
                    condition.AppendFormat(" and pad.end >= '{0}' ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else if (query.expired == 1)
                {
                    condition.AppendFormat(" and pad.end < '{0}' ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                condition.AppendFormat(" ORDER BY pad.id desc ");
                totalCount = 0;
                if (query.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable("select count(pad.id) as totalCount " + condition.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    condition.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                TempCol.Append(condition.ToString());

                return _access.getDataTableForObj<PromotionAmountDiscountQuery>(TempCol.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountDiscountDao-->Query-->" + ex.Message + TempCol.ToString(), ex);
            }
        }
        #endregion

        #region 保存數據 +DataTable Save(Model.PromotionsAmountDiscount query)
        /// <summary>
        /// 保存數據 
        /// </summary>
        /// <param name="query">參數</param>
        /// <returns>返回獲取的第一條數據</returns>
        public DataTable Save(Model.PromotionsAmountDiscount query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("insert into  promotions_amount_discount (");
            sql.AppendFormat("name,group_id,class_id,brand_id,category_id,product_id,type,amount,quantity,discount,vendor_coverage,start,end,");
            sql.AppendFormat("event_desc,event_type,condition_id,device,payment_code,kuser,created,muser,modified,site,active,status,url_by) values ");
            sql.AppendFormat("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}',{23},'{24}','{25}');select @@identity;", query.name, query.group_id, query.class_id, query.brand_id, query.category_id, query.product_id, query.type, query.amount, query.quantity, query.discount, query.vendor_coverage, CommonFunction.DateTimeToString(query.start), CommonFunction.DateTimeToString(query.end), query.event_desc, query.event_type, query.condition_id, query.device, query.payment_code, query.kuser, CommonFunction.DateTimeToString(query.created), query.muser, CommonFunction.DateTimeToString(query.modified), query.site, query.active, query.status, query.url_by);
            int indentityId = Int32.Parse(_access.getDataTable(sql.ToString()).Rows[0][0].ToString());
            string SqlDt = "select * from  promotions_amount_discount  where id=(" + indentityId + ")";
            try
            {
                return _access.getDataTable(SqlDt);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountDiscountDao-->Save-->" + ex.Message + SqlDt, ex);
            }
        }
        #endregion

        #region 刪除數據 +int Delete(Model.PromotionsAmountDiscount query, string event_id)
        /// <summary>
        /// 刪除數據
        /// </summary>
        /// <param name="query">參數</param>
        /// <param name="event_id">項目編號</param>
        /// <returns>返回刪除成功的數量</returns>
        public int Delete(Model.PromotionsAmountDiscount query, string event_id)
        {
            int i = 0;
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                PromotionsAmountDiscount model = new PromotionsAmountDiscount();
                model = GetModelById(int.Parse(query.id.ToString()));
                string sqlDel = "";
                sqlDel += _proCateDao.Delete(model.category_id);
                sqlDel += _prodCategSet.DelProdCateSet(model.category_id);
                sqlDel += _prodpromoDao.DelProdProm(event_id);
                sqlDel += _proAllDao.DeletePromAll(event_id);
                sqlDel += _usconDao.DeleteUserCon(query.condition_id);
                sqlDel += _promoDisDao.DeleteStr(event_id);
                mySqlCmd.CommandText = sqlDel + string.Format(" update promotions_amount_discount set status=0 where id={0} ;", query.id);
                i += mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PromotionsAmountDiscountDao-->Delete-->" + ex.Message + mySqlCmd.CommandText, ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
            return i;
        }
        #endregion

        #region 根據id獲取PromotionsAmountDiscountCustom詳細内容 +PromotionsAmountDiscountCustom GetModelById(int id)
        /// <summary>
        /// 根據id獲取PromotionsAmountDiscountCustom詳細内容 
        /// </summary>
        /// <param name="id">表中id</param>
        /// <returns>返回獲取的Model</returns>
        public PromotionsAmountDiscountCustom GetModelById(int id)
        {
            StringBuilder sql = new StringBuilder("select id,name,group_id,class_id,brand_id,category_id,product_id,type,amount,quantity,discount,vendor_coverage,start as 'date_state',end as 'date_end',");
            sql.Append(" event_desc,event_type,condition_id,device,payment_code,kuser,created,muser,modified,site,active,status,url_by  from  promotions_amount_discount ");
            sql.AppendFormat("  where 1=1 and id={0};", id);
            try
            {
                return _access.getSinggleObj<PromotionsAmountDiscountCustom>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountDiscountDao-->GetModelById-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 更新PromoAmountDis數據  +string UpdatePromoAmountDis(PromotionsAmountDiscountCustom model)
        /// <summary>
        /// 更新PromoAmountDis數據 
        /// </summary>
        /// <param name="model">參數模型</param>
        /// <returns>返回sql語句</returns>
        public string UpdatePromoAmountDis(PromotionsAmountDiscountCustom model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update  promotions_amount_discount  set ");
                sql.AppendFormat("name='{0}',group_id={1},class_id={2},brand_id={3},category_id={4}", model.name, model.group_id, model.class_id, model.brand_id, model.category_id);
                sql.AppendFormat(",product_id={0},type={1},amount={2},quantity={3},discount={4},vendor_coverage={5}", model.product_id, model.type, model.amount, model.quantity, model.discount, model.vendor_coverage);
                sql.AppendFormat(",start='{0}',end='{1}',modified='{2}',active={3}", CommonFunction.DateTimeToString(model.start), CommonFunction.DateTimeToString(model.end), CommonFunction.DateTimeToString(model.modified), model.active);
                sql.AppendFormat(",event_desc='{0}',event_type='{1}',condition_id={2},device={3},payment_code={4}", model.event_desc, model.event_type, model.condition_id, model.device, model.payment_code);
                sql.AppendFormat(",muser='{0}',site='{1}',status='{2}',url_by='{3}'", model.muser, model.site, model.status, model.url_by);
                sql.AppendFormat("  where id={0};", model.id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountDiscountDao-->UpdatePromoAmountDis-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 更新Active的時候進行更新的數據 +string UpdatePromoAmountDisActive(PromotionsAmountDiscountCustom model)
        /// <summary>
        /// 更新Active的時候進行更新的數據
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string UpdatePromoAmountDisActive(PromotionsAmountDiscountCustom model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update  promotions_amount_discount  set active={0},modified='{1}',muser='{2}'", model.active, CommonFunction.DateTimeToString(model.modified), model.muser);

                sql.AppendFormat("  where id={0};", model.id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountDiscountDao-->UpdatePromoAmountDisActive-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 滿額滿件折扣  shiwei0620j 20150514
        public List<PromotionsAmountDiscountCustom> GetList(PromotionsAmountDiscountCustom query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            totalCount = 0;
            try
            {
                sql.Append(" select  pad.id,pad.`name`,pad.group_id,vug.group_name, pad.discount,pad.amount,pad.quantity,pad.site,pad.start,pad.end,pad.kuser,mu1.user_username 'create_user', pad.muser,mu2.user_username 'update_user',pad.active  ");
                sqlFrom.Append(" from promotions_amount_discount pad  ");
                sqlFrom.Append(" LEFT JOIN vip_user_group vug on vug.group_id=pad.group_id LEFT JOIN manage_user mu1 on mu1.user_id=pad.kuser LEFT JOIN manage_user mu2 on mu2.user_id=pad.muser  ");
                sqlWhere.Append(" where 1=1   ");
                if (query.searchStore == 1)
                {
                    sqlWhere.AppendFormat(" and pad.end>'{0}' ", CommonFunction.DateTimeToString(DateTime.Now));
                }
                else
                {
                    sqlWhere.AppendFormat(" and pad.end<'{0}' ", CommonFunction.DateTimeToString(DateTime.Now));
                }
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable(" select  count(pad.id)  as totalCount " + sqlFrom.ToString() + sqlWhere.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sqlWhere.AppendFormat(" order by pad.id desc limit {0},{1};", query.Start, query.Limit);
                }
                sql.Append(sqlFrom.ToString() + sqlWhere.ToString());
                return _access.getDataTableForObj<PromotionsAmountDiscountCustom>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionAmountDiscountsDao-->GetList-->+" + sql.ToString() + ex.Message, ex);
            }
        }

        public int Save(PromotionsAmountDiscountCustom query)
        {
            StringBuilder sql = new StringBuilder();
            query.Replace4MySQL();
            try
            {

                if (query.id == 0)//新增
                {
                    sql.AppendFormat("insert into  promotions_amount_discount (");
                    sql.AppendFormat("name,group_id,class_id,brand_id,category_id,product_id,type,amount,quantity,discount,vendor_coverage,start,end,");
                    sql.AppendFormat("event_desc,event_type,condition_id,device,payment_code,kuser,created,muser,modified,site,active,status,url_by) values ");
                    sql.AppendFormat("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}',{23},'{24}','{25}');", query.name, query.group_id, query.class_id, query.brand_id, query.category_id, query.product_id, query.type, query.amount, query.quantity, query.discount, query.vendor_coverage, CommonFunction.DateTimeToString(query.start), CommonFunction.DateTimeToString(query.end), query.event_desc, query.event_type, query.condition_id, query.device, query.payment_code, query.kuser, CommonFunction.DateTimeToString(query.created), query.muser, CommonFunction.DateTimeToString(query.modified), query.site, query.active, query.status, query.url_by);
                }
                else//編輯
                {
                    sql.AppendFormat("update promotions_amount_discount set name='{0}',group_id='{1}', discount='{2}',start='{3}',end='{4}',modified='{5}', muser='{6}' ,amount='{7}',quantity='{8}',site='{9}' where id='{10}';", query.name, query.group_id, query.discount, CommonFunction.DateTimeToString(query.start), CommonFunction.DateTimeToString(query.end), CommonFunction.DateTimeToString(query.modified), query.muser, query.amount,query.quantity,query.site,query.id);
                }
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionAmountDiscountsDao-->Save-->+" + sql.ToString() + ex.Message, ex);
            }
        }

        public bool Delete(List<PromotionsAmountDiscountCustom> list)
        {
            ArrayList arrList = new ArrayList();
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    arrList.Add(string.Format("delete from promotions_amount_discount where id='{0}';", list[i].id));
                }
                return _mySql.ExcuteSqls(arrList);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionAmountDiscountsDao-->Delete-->+" + arrList.ToString() + ex.Message, ex);
            }

        }
        #endregion
    }
}
