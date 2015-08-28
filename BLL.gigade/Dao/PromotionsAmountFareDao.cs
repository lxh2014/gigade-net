

#region 文件信息
/* 
* Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromotionsAmountFareDao.cs      
* 摘 要：                                                                               
* 滿額滿件免運列表顯示和後台方法處理
* 当前版本：v1.1                                                                 
* 作 者： shuangshuang0420j                                           
* 完成日期：2014/6/20 
* 修改歷史：                                                                     
*         v1.1修改日期：2014/8/15  
*         v1.1修改人員：hongfei0416j     
*         v1.1修改内容：規範代碼結構，完善異常拋出，添加注釋
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
namespace BLL.gigade.Dao
{
    public class PromotionsAmountFareDao : IPromotionsAmountFareImplDao
    {
        private IDBAccess _access;
        private string connStr;
        ProductCategoryDao _cateDao = null;
        ProductCategorySetDao _prodCateSetDao = null;
        ProdPromoDao _prodPromoDao = null;
        PromoAllDao _promoAllDao = null;
        PromotionsAmountReduceDao _promAmountReduceDao = null;
        UserConditionDao _usconDao = null;
        SerialDao _serialDao = null;

        #region 有參構造函數
        /// <summary>
        /// 有參構造函數
        /// </summary>
        /// <param name="connectionstring">數據庫連接字符串</param>
        public PromotionsAmountFareDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
            _cateDao = new ProductCategoryDao(connectionstring);
            _prodCateSetDao = new ProductCategorySetDao(connectionstring);
            _prodPromoDao = new ProdPromoDao(connectionstring);
            _promoAllDao = new PromoAllDao(connectionstring);
            _promAmountReduceDao = new PromotionsAmountReduceDao(connectionstring);
            _usconDao = new UserConditionDao(connectionstring);
            _serialDao = new SerialDao(connectionstring);
        }
        #endregion

        #region 獲取列表信息+List<PromotionsAmountFareQuery> Query(PromotionsAmountFareQuery query, out int totalCount)
        /// <summary>
        /// 獲取列表信息
        /// </summary>
        /// <param name="query">查詢條件對象</param>
        /// <param name="totalCount">返回的數據總條數</param>
        /// <returns>數據列表</returns>
        public List<PromotionsAmountFareQuery> Query(PromotionsAmountFareQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.Append(" select pf.id,pf.category_id,pf.condition_id, CONCAT(pf.event_type ,right(CONCAT('00000000',pf.id),6)) as 'event_id',pf.name,pf.event_desc,pf.delivery_store,vg.group_name,");
                sql.Append(" pf.brand_id,vb.brand_name,pf.class_id,pf.product_id,pf.amount,pf.quantity,pf.fare_percent,pf.off_times,pf.site, pf.url_by,pc.banner_image,pc.category_link_url,");
                sql.Append(" uc.condition_name,pf.type ,");
                sql.Append(" pf.event_type ,pf.payment_code,pf.active,");
                sql.Append("  pf.device,pf.start start_time,pf.end end_time,pf.vendor_coverage,pf.muser,mu.user_username");
                StringBuilder tbSQL = new StringBuilder(" from promotions_amount_fare pf");
                tbSQL.Append(" left join product_category pc on pc.category_id=pf.category_id");
                tbSQL.Append(" left join vip_user_group vg on vg.group_id=pf.group_id");
                tbSQL.Append(" left join vendor_brand vb on vb.brand_id=pf.brand_id ");
                tbSQL.Append(" left join user_condition  uc on uc.condition_id =pf.condition_id ");
                tbSQL.Append(" LEFT JOIN manage_user mu ON pf.muser=mu.user_id ");
                tbSQL.Append(" where pf.status=1  ");
                if (query.expired == 0)//是未過期
                {
                    tbSQL.AppendFormat(" and pf.end >= '{0}'", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else if (query.expired == 1)
                {
                    tbSQL.AppendFormat(" and pf.end < '{0}'", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                tbSQL.Append("   order by pf.id desc");
                totalCount = 0;
                if (query.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(" select count(pf.id) as totalCount " + tbSQL.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    tbSQL.AppendFormat(" limit {0},{1};", query.Start, query.Limit);
                }

                sql.Append(tbSQL.ToString());

                return _access.getDataTableForObj<PromotionsAmountFareQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountFareDao-->Query-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 根據編號獲取數據+DataTable Select(PromotionsAmountFare model)
        /// <summary>
        /// 根據編號獲取數據
        /// </summary>
        /// <param name="model">滿額滿件免運對象</param>
        /// <returns>數據列表</returns>
        public DataTable Select(PromotionsAmountFare model)
        {
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            sb.Append(@" select id, name,display,delivery_store,group_id,class_id,brand_id,category_id,product_id,type,amount,quantity,start,end,
           created,active,event_desc,event_type,condition_id,device,kuser,fare_percent,payment_code,off_times,url_by,
            site,modified,muser,status,vendor_coverage from promotions_amount_fare where 1=1");
            if (model.id != 0)
            {
                sb.AppendFormat(" and id='{0}';", model.id);
            }
            try
            {
                return _access.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountFareDao-->Select-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 根據編號獲取數據+PromotionsAmountFareQuery Select(int id)
        /// <summary>
        /// 根據編號獲取滿額滿件免運對象
        /// </summary>
        /// <param name="id">滿額滿件免運編號</param>
        /// <returns>滿額滿件免運對象</returns>
        public PromotionsAmountFareQuery Select(int id)
        {

            StringBuilder sql = new StringBuilder();
            sql.Append(" select pf.id,pf.name, CONCAT(pf.event_type ,right(CONCAT('00000000',pf.id),6)) as 'event_id',");
            sql.Append(" pf.brand_id,pf.event_type,pf.product_id,pf.category_id,pf.amount,pf.quantity,");
            sql.Append(" pf.condition_id,pf.device,pf.event_desc,pf.delivery_store,pf.event_type,pf.group_id,pf.class_id,");
            sql.Append(" pf.display,pf.type,pf.created,pf.modified,pf.muser,");
            sql.Append("  pf.start as start_time ,pf.end as end_time,pf.kuser,pf.payment_code,pf.off_times,pf.fare_percent,pf.site, pf.url_by,pc.banner_image,pc.category_link_url ");
            sql.Append(" from promotions_amount_fare pf");
            sql.Append(" left join product_category pc on pc.category_id=pf.category_id");
            sql.Append(" where 1=1  ");
            if (id != 0)
            {
                sql.AppendFormat(" and pf.id='{0}';", id);
            }
            try
            {
                return _access.getSinggleObj<PromotionsAmountFareQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountFareDao-->Select-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 第一步保存+int Save(PromotionsAmountFareQuery model)
        /// <summary>
        /// 頁面的第一次保存,向product_category和promotionsAmountFare中新增基本數據并分別獲取category_id和id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Save(PromotionsAmountFareQuery model)
        {
            model.Replace4MySQL();
            int id = 0;
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
                //#region 保存第一步到product_category 獲取prodduct_amount_fare的category_id
                ProductCategory pmodel = new ProductCategory();
                pmodel.category_name = model.name;
                pmodel.category_createdate = (uint)BLL.gigade.Common.CommonFunction.GetPHPTime();
                pmodel.category_father_id = model.category_father_id;
                pmodel.category_ipfrom = model.category_ipfrom;
                pmodel.category_display = Convert.ToUInt32(model.status);
                mySqlCmd.CommandText = _cateDao.SaveCategory(pmodel);
                model.category_id = Convert.ToUInt32(mySqlCmd.ExecuteScalar());
                //修改表serial
                Serial serial = new Serial();
                serial.Serial_id = 12;
                serial.Serial_Value = Convert.ToUInt32(model.category_id);
                mySqlCmd.CommandText = _serialDao.UpdateAutoIncreament(serial);
                id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                //#endregion
                PromotionsAmountFare pfmodel = new PromotionsAmountFare();
                pfmodel.name = model.name;
                pfmodel.event_desc = model.event_desc;
                pfmodel.vendor_coverage = model.vendor_coverage;
                pfmodel.event_type = model.event_type;
                pfmodel.kuser = model.kuser;
                pfmodel.created = model.created;
                pfmodel.active = model.active;
                pfmodel.category_id = model.category_id;
                pfmodel.status = model.status;
                pfmodel.payment_code = model.payment_code;
                mySqlCmd.CommandText = SavePromoFare(pfmodel);
                id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PromotionsAmountFareDao-->Save-->" + ex.Message, ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
            return id;
        }
        #endregion
        #region 第二步保存+ int ReSave(PromotionsAmountFareQuery model)
        /// <summary>
        /// 最終保存，獲取第一步保存時產生的category_id和id，
        /// 并重新獲取頁面內的所有元素讓后根據id修改product_category和promotionsamountfare表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int ReSave(PromotionsAmountFareQuery model)
        {
            int i = 0;
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            StringBuilder sbExSql = new StringBuilder();
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                StringBuilder sqlf = new StringBuilder();//存儲sql語句，當其長度大於2000時，使用事物統一執行從而提高運行速度


                mySqlCmd.CommandText = _prodPromoDao.DeleteProdProm(model.event_id);
                sbExSql.Append(mySqlCmd.CommandText);
                mySqlCmd.ExecuteNonQuery();//刪除ProdPromo

                mySqlCmd.CommandText = _promoAllDao.DelPromAll(model.event_id);
                sbExSql.Append(_promoAllDao.DelPromAll(mySqlCmd.CommandText));
                mySqlCmd.ExecuteNonQuery();//刪除PromAll


                if (model.allClass == 0)//非全館時
                {
                    //刪除全館商品
                    ProductCategorySet qgSet = new ProductCategorySet();
                    qgSet.Category_Id = model.category_id;
                    qgSet.Product_Id = 999999;//全館商品刪除 id=999999
                    //根據category_id刪除product_category_set表數據
                    mySqlCmd.CommandText = _prodCateSetDao.DelProdCateSetByCPID(qgSet);
                    sbExSql.Append(mySqlCmd.CommandText);
                    mySqlCmd.ExecuteNonQuery();//刪除全館別商品999999

                    if (model.brand_id != 0)//當品牌不為空時講該品牌下的所有商品加入set表
                    {
                        sqlf.Append(_prodCateSetDao.DeleteProdCateSet(model.category_id));


                        QueryVerifyCondition query = new QueryVerifyCondition();//根據條件從product表中獲取符合條件的商品并添加到set和prodprom表中
                        query.brand_id = Convert.ToUInt32(model.brand_id);//品牌
                        query.site_ids = model.site;
                        query.combination = 1;
                        PromotionsMaintainDao _promoMainDao = new PromotionsMaintainDao(connStr);
                        query.IsPage = false;
                        int totalCount = 0;
                        List<QueryandVerifyCustom> qvcList = _promoMainDao.GetProList(query, out totalCount);
                        List<uint> categorysetProduct = new List<uint>();

                        foreach (QueryandVerifyCustom qvcItem in qvcList)//將查到的數據依次添加到set和prodprom表中，其他欄位同傳過來的數據統一
                        {
                            if (categorysetProduct.Contains(qvcItem.product_id))
                            {
                                continue;
                            }
                            categorysetProduct.Add(qvcItem.product_id);
                            ProductCategorySet pcsModel = new ProductCategorySet();
                            pcsModel.Product_Id = qvcItem.product_id;
                            pcsModel.Brand_Id = Convert.ToUInt32(model.brand_id);
                            pcsModel.Category_Id = model.category_id;
                            sqlf.Append(_prodCateSetDao.SaveProdCategorySet(pcsModel));
                        }
                    }
                    if (!string.IsNullOrEmpty(sqlf.ToString()))
                    {
                        mySqlCmd.CommandText = sqlf.ToString();
                        sbExSql.Append(mySqlCmd.CommandText);
                        mySqlCmd.ExecuteNonQuery();
                        sqlf.Clear();
                    }
                }
                else//全館時商品product_id=999999;brand_id=0
                {
                    sqlf.Clear();
                    sqlf.AppendFormat(_prodCateSetDao.DeleteProdCateSet(model.category_id));

                    ProductCategorySet pcs = new ProductCategorySet();
                    pcs.Category_Id = model.category_id;
                    pcs.Product_Id = 999999;
                    pcs.Brand_Id = 0;
                    sqlf.Append(_prodCateSetDao.SaveProdCategorySet(pcs));
                    mySqlCmd.CommandText = sqlf.ToString();
                    sbExSql.Append(_promoAllDao.DeletePromAll(mySqlCmd.CommandText));
                    mySqlCmd.ExecuteNonQuery();

                }
                sqlf.Clear();
                PromoAll pamodel = new PromoAll();
                pamodel.event_id = model.event_id;
                pamodel.event_type = model.event_type;
                pamodel.category_id = Convert.ToInt32(model.category_id);
                pamodel.startTime = model.start;
                pamodel.end = model.end;
                pamodel.kuser = model.kuser;
                pamodel.kdate = DateTime.Now;
                pamodel.muser = model.muser;
                pamodel.mdate = model.modified;
                pamodel.status = model.status;
                pamodel.product_id = model.product_id;
                pamodel.class_id = model.class_id;
                pamodel.brand_id = model.brand_id;
                sqlf.AppendFormat(_promoAllDao.SavePromAll(pamodel));
                ProductCategoryDao _categoryDao = new ProductCategoryDao(connStr);
                ProductCategory pcmodel = _categoryDao.GetModelById(Convert.ToUInt32(model.category_id));
                pcmodel.category_id = Convert.ToUInt32(model.category_id);
                pcmodel.banner_image = model.banner_image;
                pcmodel.category_link_url = model.category_link_url;
                pcmodel.category_name = model.name;
                pcmodel.category_display = Convert.ToUInt32(model.status);
                pcmodel.category_father_id = model.category_father_id;
                sqlf.AppendFormat(_cateDao.UpdateProdCate(pcmodel));
                PromotionsAmountReduce parModel = new PromotionsAmountReduce();
                parModel.id = model.id;
                parModel.name = model.name;
                parModel.delivery_store = model.delivery_store;
                parModel.group_id = model.group_id;
                parModel.type = model.type;
                parModel.amount = model.amount;
                parModel.quantity = model.quantity;
                parModel.start = model.start;
                parModel.end = model.end;
                parModel.created = model.created;
                parModel.active = model.active ? 1 : 0;
                parModel.status = model.status;
                parModel.created = model.created;
                parModel.updatetime = model.modified;
                sqlf.AppendFormat(_promAmountReduceDao.Save(parModel));
                sqlf.Append(UpdatePromoFare(model));
                mySqlCmd.CommandText = sqlf.ToString();
                i += mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PromotionsAmountFareDao-->ReSave-->" + ex.Message, ex);
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

        #region 修改信息+int Update(PromotionsAmountFareQuery model, string oldEventId)
        public int Update(PromotionsAmountFareQuery model, string oldEventId)
        {
            int i = 0;
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            StringBuilder sbExSql = new StringBuilder();
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                StringBuilder sqlf = new StringBuilder();

                mySqlCmd.CommandText = _prodPromoDao.DeleteProdProm(model.event_id);
                sbExSql.Append(mySqlCmd.CommandText);
                mySqlCmd.ExecuteNonQuery();//刪除ProdPromo

                mySqlCmd.CommandText = _promoAllDao.DelPromAll(model.event_id);
                sbExSql.Append(_promoAllDao.DelPromAll(mySqlCmd.CommandText));
                mySqlCmd.ExecuteNonQuery();//刪除PromAll

                if (model.allClass == 0)
                {
                    //刪除全館商品
                    ProductCategorySet qgSet = new ProductCategorySet();
                    qgSet.Category_Id = model.category_id;
                    qgSet.Product_Id = 999999;//全館商品刪除 id=999999
                    //根據category_id刪除product_category_set表數據
                    mySqlCmd.CommandText = _prodCateSetDao.DelProdCateSetByCPID(qgSet);
                    sbExSql.Append(mySqlCmd.CommandText);
                    mySqlCmd.ExecuteNonQuery();//刪除全館別商品999999

                    if (model.brand_id != 0)//當品牌不為空時講該品牌下的所有商品加入set表
                    {
                        sqlf.Append(_prodCateSetDao.DeleteProdCateSet(model.category_id));

                        mySqlCmd.CommandText = sqlf.ToString();
                        sbExSql.Append(mySqlCmd.CommandText);
                        mySqlCmd.ExecuteNonQuery();
                        sqlf.Clear();

                        QueryVerifyCondition query = new QueryVerifyCondition();//根據條件從product表中獲取符合條件的商品并添加到set和prodprom表中
                        query.brand_id = Convert.ToUInt32(model.brand_id);//品牌
                        query.site_ids = model.site;
                        query.combination = 1;
                        PromotionsMaintainDao _promoMainDao = new PromotionsMaintainDao(connStr);
                        query.IsPage = false;
                        int totalCount = 0;
                        List<QueryandVerifyCustom> qvcList = _promoMainDao.GetProList(query, out totalCount);

                        List<uint> categorysetProduct = new List<uint>();
                        foreach (QueryandVerifyCustom qvcItem in qvcList)
                        {
                            if (categorysetProduct.Contains(qvcItem.product_id))
                            {
                                continue;
                            }
                            categorysetProduct.Add(qvcItem.product_id);
                            ProductCategorySet pcsModel = new ProductCategorySet();
                            pcsModel.Product_Id = qvcItem.product_id;
                            pcsModel.Brand_Id = Convert.ToUInt32(model.brand_id);
                            pcsModel.Category_Id = model.category_id;
                            //刪除已有的 新增異動的
                            sqlf.Append(_prodCateSetDao.SaveProdCategorySet(pcsModel));

                        }
                    }
                    if (!string.IsNullOrEmpty(sqlf.ToString()))
                    {
                        mySqlCmd.CommandText = sqlf.ToString();
                        sbExSql.Append(mySqlCmd.CommandText);
                        mySqlCmd.ExecuteNonQuery();
                        sqlf.Clear();
                    }

                }
                else//全館時
                {
                    sqlf.Clear();
                    sqlf.AppendFormat(_prodCateSetDao.DeleteProdCateSet(model.category_id));


                    ProductCategorySet pcs = new ProductCategorySet();
                    pcs.Category_Id = model.category_id;
                    pcs.Product_Id = 999999;
                    pcs.Brand_Id = 0;
                    sqlf.Append(_prodCateSetDao.SaveProdCategorySet(pcs));
                    mySqlCmd.CommandText = sqlf.ToString();
                    sbExSql.Append(_promoAllDao.DeletePromAll(mySqlCmd.CommandText));
                    mySqlCmd.ExecuteNonQuery();

                }
                sqlf.Clear();
                PromoAll pamodel = new PromoAll();
                pamodel.event_id = model.event_id;
                pamodel.event_type = model.event_type;
                pamodel.category_id = Convert.ToInt32(model.category_id);
                pamodel.startTime = model.start;
                pamodel.end = model.end;
                pamodel.status = model.status;
                pamodel.kuser = model.kuser;
                pamodel.kdate = model.created;
                pamodel.muser = model.muser;
                pamodel.mdate = model.modified;
                pamodel.product_id = model.product_id;
                pamodel.class_id = model.class_id;
                pamodel.brand_id = model.brand_id;
                sqlf.AppendFormat(_promoAllDao.SavePromAll(pamodel));
                ProductCategoryDao _categoryDao = new ProductCategoryDao(connStr);
                ProductCategory pcmodel = _categoryDao.GetModelById(Convert.ToUInt32(model.category_id));
                pcmodel.category_id = Convert.ToUInt32(model.category_id);
                pcmodel.category_name = model.name;
                pcmodel.banner_image = model.banner_image;
                pcmodel.category_link_url = model.category_link_url;
                pcmodel.category_father_id = model.category_father_id;
                pcmodel.category_updatedate = (uint)BLL.gigade.Common.CommonFunction.GetPHPTime(model.modified.ToString());
                pcmodel.category_display = Convert.ToUInt32(model.status);
                sqlf.AppendFormat(_cateDao.UpdateProdCate(pcmodel));
                PromotionsAmountReduce parModel = new PromotionsAmountReduce();
                parModel.id = model.id;
                parModel.name = model.name;
                parModel.delivery_store = model.delivery_store;
                parModel.group_id = model.group_id;
                parModel.type = model.type;
                parModel.amount = model.amount;
                parModel.quantity = model.quantity;
                parModel.start = model.start;
                parModel.end = model.end;
                parModel.updatetime = model.modified;
                parModel.created = model.created;
                parModel.active = model.active ? 1 : 0;
                parModel.status = model.status;
                sqlf.AppendFormat(_promAmountReduceDao.Update(parModel));
                mySqlCmd.CommandText = sqlf + UpdatePromoFare(model);
                i += mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PromotionsAmountFareDao-->Update-->sql:" + sbExSql.ToString() + ex.Message, ex);
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

        #region 刪除信息+int Delete(int id)
        public int Delete(int id)
        {
            int i = 0;
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            string sqlSW = "";
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                PromotionsAmountFareQuery query = new PromotionsAmountFareQuery();
                if (id != 0)
                {
                    query = Select(id);
                    sqlSW += DeletePromoFare(query.id);
                    sqlSW += _promAmountReduceDao.Delete(query.id);
                    if (query.category_id != 0)
                    {
                        sqlSW += _cateDao.Delete(query.category_id);
                        sqlSW += _prodCateSetDao.DelProdCateSet(query.category_id);
                        sqlSW += _usconDao.DeleteUserCon(query.condition_id);
                    }
                    if (!string.IsNullOrEmpty(query.event_id))
                    {
                        sqlSW += _prodPromoDao.DelProdProm(query.event_id);
                        sqlSW += _promoAllDao.DeletePromAll(query.event_id);
                    }
                }
                mySqlCmd.CommandText = sqlSW.ToString();
                i += mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PromotionsAmountFareDao-->Delete-->" + ex.Message + sqlSW.ToString(), ex);
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

        #region 刪除promotionsamountfare的sql語句，用於事物的調用+string DeletePromoFare(int id)
        /// <summary>
        /// 刪除promotionsamountfare的sql語句，用於事物的調用
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string DeletePromoFare(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("update promotions_amount_fare set status=0 where id={0} ;", id);
            return sql.ToString();
        }
        #endregion
        #region 新增promotionsamountfare的sql語句，用於事物的調用+string SavePromoFare(PromotionsAmountFare model)
        /// <summary>
        /// 新增promotionsamountfare的sql語句，用於事物的調用
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string SavePromoFare(PromotionsAmountFare model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("insert into  promotions_amount_fare (");
                sql.AppendFormat("name,display,delivery_store,group_id,class_id,brand_id,category_id,product_id,type,amount,quantity,start,end,");
                sql.AppendFormat("created,active,event_desc,event_type,condition_id,device,kuser,fare_percent,payment_code,off_times,url_by,site,modified,muser,status,vendor_coverage) values ");
                sql.AppendFormat("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}',{14},'{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}');select @@identity;",
                 model.name, model.display, model.delivery_store, model.group_id, model.class_id, model.brand_id,
                 model.category_id, model.product_id, model.type, model.amount, model.quantity, CommonFunction.DateTimeToString(model.start),
                 CommonFunction.DateTimeToString(model.end),
                 CommonFunction.DateTimeToString(model.created),
                 model.active ? 1 : 0, model.event_desc, model.event_type,
                 model.condition_id, model.device, model.kuser,
                 model.fare_percent, model.payment_code, model.off_times,
                 model.url_by, model.site, CommonFunction.DateTimeToString(model.modified), model.muser, model.status, model.vendor_coverage);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountFareDao-->SavePromoFare-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 修改promotionsamountfare的sql語句，用於事物的調用+string UpdatePromoFare(PromotionsAmountFareQuery model)
        /// <summary>
        /// 修改promotionsamountfare的sql語句，用於事物的調用
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string UpdatePromoFare(PromotionsAmountFareQuery model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update  promotions_amount_fare  set ");
                sql.AppendFormat("name='{0}',display='{1}',delivery_store='{2}',group_id='{3}',class_id='{4}'", model.name, model.display, model.delivery_store, model.group_id, model.class_id);
                sql.AppendFormat(",brand_id='{0}',category_id='{1}',product_id='{2}',type='{3}',amount='{4}',quantity='{5}'", model.brand_id, model.category_id, model.product_id, model.type, model.amount, model.quantity);
                sql.AppendFormat(",start='{0}',end='{1}',active={2},event_desc='{3}'", CommonFunction.DateTimeToString(model.start), CommonFunction.DateTimeToString(model.end), model.active ? 1 : 0, model.event_desc);
                sql.AppendFormat(",event_type='{0}',condition_id='{1}',device='{2}',fare_percent='{3}',payment_code='{4}'", model.event_type, model.condition_id, model.device, model.fare_percent, model.payment_code);
                sql.AppendFormat(",off_times='{0}',url_by='{1}',modified='{2}',muser='{3}',site='{4}',status='{5}',vendor_coverage='{6}'", model.off_times, model.url_by, CommonFunction.DateTimeToString(model.modified), model.muser, model.site, model.status, model.vendor_coverage);
                sql.AppendFormat("  where id='{0}';", model.id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountFareDao-->UpdatePromoFare-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 修改狀態+int UpdateActive(PromotionsAmountFareQuery model)
        /// <summary>
        /// 修改狀態
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateActive(PromotionsAmountFareQuery model)
        {
            int i = 0;
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            StringBuilder sql = new StringBuilder();
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;

                sql.Append(_prodPromoDao.DeleteProdProm(model.event_id));//先刪除在新增當active=0時照樣刪除只有active=1時才新增
                if (model.active)
                {
                    ProductCategorySet querySet = new ProductCategorySet();
                    querySet.Category_Id = Convert.ToUInt32(model.category_id);
                    ProductCategorySetDao _setDao = new ProductCategorySetDao(connStr);
                    List<ProductCategorySet> lmodelSet = _setDao.Query(querySet);
                    foreach (ProductCategorySet item in lmodelSet)
                    {
                        ProdPromo ppmodel = new ProdPromo();
                        ppmodel.product_id = Convert.ToInt32(item.Product_Id);
                        ppmodel.event_id = model.event_id;
                        ppmodel.event_type = model.event_type;
                        ppmodel.event_desc = model.event_desc;
                        ppmodel.start = model.start;
                        ppmodel.end = model.end;
                        ppmodel.page_url = model.category_link_url;
                        if (model.group_id == 0 && model.condition_id == 0)
                        {
                            ppmodel.user_specified = 0;
                        }
                        else
                        {
                            ppmodel.user_specified = 1;
                        }
                        ppmodel.kuser = model.muser;
                        ppmodel.kdate = model.modified;
                        ppmodel.muser = model.muser;
                        ppmodel.mdate = model.modified;
                        ppmodel.status = model.status;
                        sql.Append(_prodPromoDao.SaveProdProm(ppmodel));
                    }
                }
                if (model.id != 0)
                {
                    sql.Append(UpdatePromoFare(model));
                    PromotionsAmountReduce parModel = new PromotionsAmountReduce();
                    parModel.id = model.id;
                    parModel.name = model.name;
                    parModel.delivery_store = model.delivery_store;
                    parModel.group_id = model.group_id;
                    parModel.type = model.type;
                    parModel.amount = model.amount;
                    parModel.quantity = model.quantity;
                    parModel.start = model.start;
                    parModel.end = model.end;
                    parModel.created = model.created;
                    parModel.updatetime = model.modified;
                    parModel.active = model.active ? 1 : 0;
                    parModel.status = model.status;
                    sql.Append(_promAmountReduceDao.Update(parModel));
                }
                mySqlCmd.CommandText = sql.ToString();
                i += mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PromotionsAmountFareDao-->UpdateActive-->" + ex.Message + sql.ToString(), ex);
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
    }
}
