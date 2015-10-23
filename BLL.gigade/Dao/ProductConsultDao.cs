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
    public class ProductConsultDao : IProductConsultImplDao
    {
        private IDBAccess _access;
        public ProductConsultDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        #region 商品咨詢列表查詢+List<ProductConsultQuery> Query(ProductConsultQuery store, out int totalCount)
        /// <summary>
        /// 商品咨詢列表查詢
        /// </summary>
        /// <param name="store"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<ProductConsultQuery> Query(ProductConsultQuery store, out int totalCount)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder strCondition = new StringBuilder();
            StringBuilder strSelect = new StringBuilder();
            StringBuilder strTemp = new StringBuilder();
            try
            {
                store.Replace4MySQL();
                totalCount = 0;
                strSelect.AppendFormat("select pc.consult_id,pc.product_id,p.product_name,mu.user_username as manage_name,pc.user_id,uc.user_email,uc.user_name,pc.consult_info,tp.parameterName,pc.consult_type,pc.consult_answer,pc.is_sendEmail,pc.create_date,pc.answer_date,pc.answer_user,pc.status,pc.consult_url,pc.product_url,pc.item_id,pc.answer_status,pc.delay_reason,p.prod_classify  ");
                strCondition.AppendFormat(" from product_consult pc left join product p on p.product_id=pc.product_id left join users uc on uc.user_id=pc.user_id ");
                strCondition.AppendFormat(" left join  (SELECT parameterName,parameterCode,parameterType from t_parametersrc where parameterType='consultType' ) tp on pc.consult_type=tp.parameterCode ");
                strCondition.AppendFormat(" left join manage_user mu on mu.user_id=pc.answer_user  ");
                strCondition.AppendFormat(" where 1=1 ");
                if (store.answer_status != 0)
                {
                    strCondition.AppendFormat(" and answer_status={0}  ", store.answer_status);
                }
                if (store.consult_id != 0)
                {
                    strCondition.AppendFormat(" and pc.consult_id ={0} ", store.consult_id);
                }
                if (store.huifu ==1)
                {
                    strCondition.AppendFormat(" and status={0}  ", 1);
                }
                if (store.huifu == 2)
                {
                    strCondition.AppendFormat(" and status={0}  ", 0);
                }
                if (store.product_name != "")
                {
                    strCondition.AppendFormat(" and p.product_name like '%{0}%'", store.product_name);
                }
              
                if (store.product_id != 0)
                {
                    strCondition.AppendFormat(" and p.product_id ={0} ", store.product_id);
                }
                if (store.consultType != "")
                {
                    strCondition.AppendFormat(" and pc.consult_type in ({0}) ", store.consultType);
                }
                if (store.user_email != "")
                {
                    strCondition.AppendFormat(" and  uc.user_email like '%{0}%' ", store.user_email);
                }
                if (store.user_name != "")
                {
                    strCondition.AppendFormat(" and  uc.user_name like '%{0}%' ", store.user_name);
                }
                if (store.productIds != "")
                {
                    strCondition.AppendFormat(" and p.prod_classify ={0} ", store.productIds);
                }
                if (store.beginTime != DateTime.MinValue)
                {
                    strCondition.AppendFormat(" and pc.create_date>='{0}' ", store.beginTime.ToString("yyyy-MM-dd 00:00:00"));
                }
                if (store.endTime != DateTime.MinValue)
                {
                    strCondition.AppendFormat(" and pc.create_date<='{0}' ", store.endTime.ToString("yyyy-MM-dd 23:59:59"));
                }
                StringBuilder strTotalCount = new StringBuilder(" select count(pc.consult_id) as totalCount ");
                if (store.IsPage)
                {
                    strSql.AppendFormat(strTotalCount.ToString());
                    strSql.AppendFormat(strCondition.ToString());
                    System.Data.DataTable _dt = _access.getDataTable(strSql.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }

                    strSql.AppendFormat(";");
                    strCondition.AppendFormat(" order by pc.consult_id desc ");
                }
                strTemp.AppendFormat(strSelect.ToString());
                strTemp.AppendFormat(strCondition.ToString());
                strTemp.AppendFormat(" limit {0},{1}", store.Start, store.Limit);
                strTemp.AppendFormat(";");
                strSql.AppendFormat(strTemp.ToString());
                return _access.getDataTableForObj<ProductConsultQuery>(strTemp.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("ProductConsultDao-->Query-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        #region  查詢商品信息+ProductConsultQuery GetProductInfo(ProductConsultQuery query)
        /// <summary>
        /// 查詢商品信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ProductConsultQuery GetProductInfo(ProductConsultQuery query)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                if (query.item_id == 0)
                {
                    strSql.AppendFormat("SELECT vb.brand_name,p.product_name,p.product_image,pm.price ,pm.event_price ,pm.event_start,pm.event_end FROM price_master pm LEFT JOIN product p ON pm.product_id=p.product_id LEFT JOIN vendor_brand vb on vb.brand_id=p.brand_id  WHERE pm.product_id ='{0}' AND pm.site_id = 1 AND (pm.child_id = pm.product_id or pm.child_id = 0)AND pm.user_level = 1", query.product_id);
                }
                else
                {
                    strSql.AppendFormat("SELECT vb.brand_name,p.product_name,ps1.spec_name as spec_name1,ps2.spec_name as spec_name2,ps1.spec_image as product_image1,ps2.spec_image as product_image2,p.product_image,ip.item_money,ip.event_money,pm.event_start,pm.event_end  FROM product_item pit LEFT JOIN product p ON pit.product_id = p.product_id LEFT JOIN product_spec ps1 on ps1.spec_id=pit.spec_id_1 LEFT JOIN product_spec ps2 on ps2.spec_id=pit.spec_id_2 LEFT JOIN price_master pm ON p.product_id = pm.product_id LEFT JOIN item_price ip ON ip.price_master_id = pm.price_master_id LEFT JOIN vendor_brand vb on vb.brand_id=p.brand_id WHERE 1=1 AND pm.price_status = 1 AND pit.item_id = {0} AND pm.site_id = 1 AND pm.user_level = 1 AND ip.price_master_id = pm.price_master_id AND ip.item_id = {0} and p.product_id={1}", query.item_id, query.product_id);
                }
                return _access.getSinggleObj<ProductConsultQuery>(strSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("ProductConsultDao-->GetProductInfo-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        #region 回覆咨詢+int SaveProductConsultAnswer(ProductConsultQuery query)
        /// <summary>
        /// 回覆咨詢
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int SaveProductConsultAnswer(ProductConsultQuery query)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat("set sql_safe_updates = 0; update product_consult set answer_date='{0}',consult_answer='{1}',answer_user='{2}',answer_status='{3}',status='{4}',is_sendEmail='{5}' where consult_id='{6}'; set sql_safe_updates = 1;", query.answer_date.ToString("yyyy-MM-dd HH:mm:ss"), query.consult_answer, query.answer_user, query.answer_status, query.status,query.is_sendEmail, query.consult_id);
                return _access.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("ProductConsultDao-->SaveProductConsultAnswer-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        #region 更改狀態+int UpdateActive(ProductConsultQuery model)
        /// <summary>
        /// 更改狀態
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateActive(ProductConsultQuery model)
        {
            string strSql = string.Empty;
            try
            {
                strSql = string.Format("set sql_safe_updates = 0; update product_consult set status='{0}' where consult_id='{1}'; set sql_safe_updates = 1;", model.status, model.consult_id);
                return _access.execCommand(strSql);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductConsultDao-->UpdateActive-->" + ex.Message + strSql, ex);
            }
        }
        #endregion

        #region 更改回覆狀態+int UpdateAnswerStatus(ProductConsultQuery model)
        /// <summary>
        /// 更改回覆狀態
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateAnswerStatus(ProductConsultQuery model)
        {
            string strSql = string.Empty;
            try
            {
                strSql = string.Format("set sql_safe_updates = 0; update product_consult set answer_status='{0}',answer_user='{1}',delay_reason='{2}' where consult_id='{3}'; set sql_safe_updates = 1;", model.answer_status, model.answer_user, model.delay_reason, model.consult_id);
                return _access.execCommand(strSql);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductConsultDao-->UpdateAnswerStatus-->" + ex.Message + strSql, ex);
            }
        }
        #endregion

        #region 更改諮詢類型+int UpdateConsultType(ProductConsultQuery model)
        /// <summary>
        /// 更改諮詢類型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateConsultType(ProductConsultQuery model)
        {
            string strSql = string.Empty;
            try
            {
                strSql = string.Format("set sql_safe_updates = 0; update product_consult set consult_type='{0}' where consult_id='{1}'; set sql_safe_updates = 1;", model.consult_type, model.consult_id);
                return _access.execCommand(strSql);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductConsultDao-->UpdateConsultType-->" + ex.Message + strSql, ex);
            }
        }
        #endregion
        #region 查詢郵件群組的編號+DataTable QueryMailGroup(ProductConsultQuery query)
        /// <summary>
        /// 查詢郵件群組的編號
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable QueryMailGroup(ProductConsultQuery query)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat("select row_id from mail_group where group_code='{0}'", query.group_code);
                return _access.getDataTable(strSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("ProductConsultDao-->QueryMailGroup-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

    }
}
