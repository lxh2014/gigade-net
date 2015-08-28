#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromotionsAmountGiftDao.cs
* 摘 要：
* 滿額滿件送禮dao
* 当前版本：v1.1
* 作 者： mengjuan0826j
* 完成日期：2014/6/20 
* 修改歷史：
*         v1.1修改日期：2014/8/15 
*         v1.1修改人員：mengjuan0826j 
*         v1.1修改内容：在抛出異常的時候將Sql語句抛出，合并代碼，添加注釋 
*/

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model.Query;
using MySql.Data.MySqlClient;
using BLL.gigade.Model;
using BLL.gigade.Common;
using System.Data;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Dao
{
    public class PromotionsAmountGiftDao : IPromotionsAmountGiftImplDao
    {
        private IDBAccess _access;
        private string connStr;
        ProductCategoryDao _proCateDao = null;
        ProductCategorySetDao _prodCategSetDao = null;
        ProdPromoDao _prodpromoDao = null; 
        PromoAllDao _proAllDao = null;
        UserConditionDao _usconDao = null;
        PromoDiscountDao _promoDisDao = null;
        SerialDao _serialDao = null;
        PromoTicketDao _ptDao = null;
        ProductDao _prodDao = null;
        PromotionsMaintainDao _promoMainDao = null;
        VendorBrandDao _vendorBrandDao = null;
        #region 有參構造函數
        /// <summary>
        /// 有參構造函數
        /// </summary>
        /// <param name="connectionstring">數據庫連接字符串</param>
        public PromotionsAmountGiftDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
            _proCateDao = new ProductCategoryDao(connectionstring);
            _prodCategSetDao = new ProductCategorySetDao(connectionstring);
            _prodpromoDao = new ProdPromoDao(connectionstring);
            _proAllDao = new PromoAllDao(connectionstring);
            _usconDao = new UserConditionDao(connectionstring);
            _promoDisDao = new PromoDiscountDao(connectionstring);
            _serialDao = new SerialDao(connectionstring);
            _ptDao = new PromoTicketDao(connectionstring);
            _prodDao = new ProductDao(connectionstring);
            _promoMainDao = new PromotionsMaintainDao(connectionstring);
            _vendorBrandDao = new VendorBrandDao(connectionstring);
        }
        #endregion

        #region  獲取列表頁 +List<Model.Query.PromotionsAmountGiftQuery> Query(Model.Query.PromotionsAmountGiftQuery query, out int totalCount, string type)
        /// <summary>
        /// 獲取滿額滿件送禮列表頁
        /// </summary>
        /// <param name="query">PromotionsAmountGiftQuery query對象 </param>
        /// <param name="totalCount">輸出總行數</param>
        /// <returns>List<Model.Query.PromotionsAmountGiftQuery>對象</returns>
        public List<Model.Query.PromotionsAmountGiftQuery> Query(Model.Query.PromotionsAmountGiftQuery query, out int totalCount, string type)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                StringBuilder condition = new StringBuilder();
                StringBuilder TempCol = new StringBuilder();

                TempCol.Append("select DISTINCT( pag.id),pag.dollar,pag.point,pag.gift_mundane,pag.event_type,event_id,bonus_state, ");
                TempCol.Append("  case pag.active when true then 1 when false then 0 end as newactive,pag.product_id,pag.brand_id,pag.`name`,");
                TempCol.Append("  pag.event_desc,pag.url_by,pc.banner_image,pc.category_link_url,pag.group_id,vug.group_name,pag.gift_id,");
                TempCol.Append(" pag.condition_id,pag.num_limit,pag.count_by,pag.active_now,pag.category_id,");
                TempCol.Append(" pag.`repeat` ,pag.amount,pag.quantity,pag.class_id,pag.brand_id,");
                TempCol.Append(" pag.event_type,pag.deduct_welfare,");//pm.parameterName  'payment',
                TempCol.Append("pag.start 'startdate',pag.end 'enddate',pag.use_start,pag.use_end,pag.site,");
                //TempCol.Append(" pag.gift_type ,case pag.gift_type when 1 then '商品' WHEN 2 then '機會' when 3  then '購物金' when 4 then '抵用券' end as 'gift_type_name',pag.vendor_coverage,pag.gift_product_number,pag.payment_code,pag.delivery_category,pag.freight_price,");
                TempCol.Append(" pag.gift_type ,pag.vendor_coverage,pag.gift_product_number,pag.payment_code,pag.delivery_category,pag.freight_price,pag.muser,mu.user_username,");

                TempCol.Append(" pag.dividend ");//紅利類型1.點2:點+金3:比率固定4:非固定 add by shuangshuang0420j 2014.11.10 11:43:23 
                StringBuilder tempCount = new StringBuilder("select count(pag.id) as totalCount  ");
                condition.Append(" from promotions_amount_gift pag ");
                condition.Append(" LEFT JOIN vip_user_group vug on vug.group_id=pag.group_id ");
                condition.Append(" LEFT JOIN product_category  pc on pc.category_id=pag.category_id ");
                condition.Append(" LEFT JOIN manage_user mu ON pag.muser=mu.user_id");
                condition.AppendFormat(" where pag.status=1 and event_type in ({0}) ", type);

                if (query.expired == 0)//是未過期
                {
                    condition.AppendFormat(" and pag.end >= '{0}'", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else if (query.expired == 1)
                {
                    condition.AppendFormat(" and pag.end < '{0}'", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                if (!string.IsNullOrEmpty(query.selcon))
                {
                    condition.AppendFormat(" and (pag.name like N'%{0}%' or pag.event_id like N'%{0}%' ) ", query.selcon);
                }
                if (query.start != DateTime.MinValue)
                {
                    condition.AppendFormat(" and pag.start >= '{0}' ", CommonFunction.DateTimeToString(query.start));
                }
                if (query.end != DateTime.MinValue)
                {
                    condition.AppendFormat(" and pag.start <= '{0}'  ", CommonFunction.DateTimeToString(query.end));
                }
                totalCount = 0;
                if (query.IsPage)
                {
                    sql.Append(tempCount.ToString() + condition.ToString());
                    System.Data.DataTable _dt = _access.getDataTable(sql.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    condition.AppendFormat(" order by pag.id desc ");
                    condition.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                condition.AppendFormat(";");
                sql.Append(";" + TempCol.ToString() + condition.ToString() + ";");
                IParametersrcImplDao _parameterDao = new ParametersrcDao(connStr);
                List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("product_freight", "payment", "device");
                List<PromotionsAmountGiftQuery> list = _access.getDataTableForObj<PromotionsAmountGiftQuery>(TempCol.ToString() + condition.ToString());
                foreach (PromotionsAmountGiftQuery q in list)
                {
                    var alist = parameterList.Find(m => m.ParameterType == "product_freight" && m.ParameterCode == q.type.ToString());
                    var clist = parameterList.Find(m => m.ParameterType == "payment" && m.ParameterCode == q.payment_code.ToString());
                    var dlist = parameterList.Find(m => m.ParameterType == "device" && m.ParameterCode == q.device.ToString());
                    if (alist != null)
                    {
                        q.freight = alist.parameterName;
                    }
                    if (clist != null)
                    {
                        q.payment = clist.parameterName;
                    }
                    if (dlist != null)
                    {
                        q.devicename = dlist.parameterName;
                    }
                    //CONCAT(pag.event_type ,right(CONCAT('00000000',pag.id),6)) as event_ids,
                    q.event_ids = Common.CommonFunction.GetEventId(q.event_type, q.id.ToString());
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountGiftDao-->Query-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 新增數據 第一步保存
        #region 事物新增主表數據和product_category表數據 + int Save(PromotionsAmountGiftQuery query)
        /// <summary>
        /// 第一步保存(事物) 新增主表數據和product_category表數據
        /// </summary>
        /// <param name="query">PromotionsAmountGiftQuery query對象</param>
        /// <returns></returns>
        public int Save(PromotionsAmountGiftQuery query)
        {
            _proCateDao = new ProductCategoryDao(connStr);
            query.Replace4MySQL();
            int id = 0;
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
                //#region 保存第一步到product_category 獲取prodduct_amount_fare的category_id
                ProductCategory pmodel = new ProductCategory();
                pmodel.category_name = query.name;
                pmodel.category_createdate = (uint)BLL.gigade.Common.CommonFunction.GetPHPTime();
                pmodel.category_father_id = query.category_father_id;
                pmodel.category_ipfrom = query.category_ipfrom;
                pmodel.category_display = Convert.ToUInt32(query.status);

                mySqlCmd.CommandText = _proCateDao.SaveCategory(pmodel);
                sbExSql.Append(mySqlCmd.CommandText);
                query.category_id = Convert.ToUInt32(mySqlCmd.ExecuteScalar());

                //修改表serial
                Serial serial = new Serial();
                serial.Serial_id = 12;
                serial.Serial_Value = Convert.ToUInt32(query.category_id);
                mySqlCmd.CommandText = _serialDao.UpdateAutoIncreament(serial);
                sbExSql.Append(mySqlCmd.CommandText);
                id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                //#endregion

                PromotionsAmountGift pgmodel = new PromotionsAmountGift();
                pgmodel.name = query.name;
                pgmodel.event_desc = query.event_desc;
                pgmodel.vendor_coverage = query.vendor_coverage;
                pgmodel.event_type = query.event_type;
                pgmodel.amount = query.amount;
                pgmodel.quantity = query.quantity;
                pgmodel.kuser = query.kuser;
                pgmodel.created = query.created;
                pgmodel.status = query.status;
                pgmodel.category_id = query.category_id;
                mySqlCmd.CommandText = SavePromoGift(pgmodel);
                sbExSql.Append(mySqlCmd.CommandText);
                id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PromotionsAmountGiftDao-->Save-->" + ex.Message + sbExSql.ToString(), ex);
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

        #region 新增主表數據 +string SavePromoGift(PromotionsAmountGift query)
        /// <summary>
        /// 想promotions_amount_discount添加第一步數據
        /// </summary>
        /// <param name="query">PromotionsAmountGift query對象</param>
        /// <returns>添加主表數據的sql語句</returns>
        public string SavePromoGift(PromotionsAmountGift query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("insert into  promotions_amount_gift (");
                sql.AppendFormat("name,group_id,class_id,brand_id,category_id,product_id,type,amount,quantity,`repeat` ,gift_id,");
                sql.AppendFormat("deduct_welfare,bonus_type,mailer_id,start,end,created,modified,active,event_desc,event_type,condition_id,device, ");
                sql.AppendFormat(" payment_code,gift_type,ticket_id,ticket_name,count_by,number,num_limit,active_now,valid_interval, ");
                sql.AppendFormat("use_start,use_end ,kuser,muser,url_by,url,banner_file,status,site,vendor_coverage,gift_product_number) values ");
                sql.AppendFormat("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}',", query.name, query.group_id, query.class_id, query.brand_id,
                 query.category_id, query.product_id, query.type, query.amount, query.quantity, query.repeat ? '1' : '0', query.gift_id);
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},'{8}','{9}','{10}','{11}',", query.deduct_welfare, query.bonus_type, query.mailer_id, CommonFunction.DateTimeToString(query.start),
                 CommonFunction.DateTimeToString(query.end), CommonFunction.DateTimeToString(query.created), CommonFunction.DateTimeToString(query.modified), query.active, query.event_desc, query.event_type, query.condition_id, query.device);
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',", query.payment_code, query.gift_type, query.ticket_id, query.ticket_name, query.count_by, query.number,
                 query.num_limit, query.active_now, query.valid_interval);
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}');select @@identity;", CommonFunction.DateTimeToString(query.use_start), CommonFunction.DateTimeToString(query.use_end), query.kuser, query.muser, query.url_by, query.url, query.banner_file, query.status, query.site, query.vendor_coverage, query.gift_product_number);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountGiftDao-->SavePromoGift-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        #endregion

        #region 根據id獲取query對象 + PromotionsAmountGiftQuery Select(int id)
        /// <summary>
        /// 根據id獲取query對象
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>PromotionsAmountGiftQuery對象</returns>
        public PromotionsAmountGiftQuery Select(int id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select pag.id,pag.name,pag.category_id,pc.banner_image,pc.category_link_url,pag.group_id,pag.class_id,");
                sql.Append(" pag.brand_id,pag.product_id,pag.type,pag.amount,pag.quantity,pag.`repeat`,pag.gift_id,pag.ticket_id,");
                sql.Append(" pag.ticket_name,pag.deduct_welfare,pag.bonus_type,pag.mailer_id,pag.start 'startdate',pag.end 'enddate',pag.created,");
                sql.Append(" pag.modified,pag.active,pag.event_desc,pag.event_type,pag.condition_id,pag.device,pag.payment_code,pag.gift_type,pag.ticket_name,");
                sql.Append(" pag.count_by,pag.number,pag.num_limit,pag.valid_interval,pag.active_now,pag.use_start,pag.use_end,pag.kuser,pag.muser,pag.url_by,pag.site,pag.status,pag.vendor_coverage,pag.gift_product_number");
                sql.Append(" from promotions_amount_gift pag");
                sql.Append(" LEFT JOIN product_category  pc on pc.category_id=pag.category_id ");
                sql.Append(" where ");

                sql.AppendFormat(" pag.id='{0}';", id);
                return _access.getSinggleObj<PromotionsAmountGiftQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountGiftDao-->Select-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 更新數據
        #region 更新數據  事物更新 +int Update(PromotionsAmountGiftQuery model, string oldeventid)
        /// <summary>
        /// 第二步保存和編輯數據  更新主表和product_category數據 處理product_category_set prod_promo prom_all prom_ticket
        /// </summary>
        /// <param name="model">query對象</param>
        /// <param name="oldeventid">原來的event_type</param>
        /// <returns>執行結果</returns>
        public int Update(PromotionsAmountGiftQuery model, string oldeventid)
        {
            //實例dao對象
            _ptDao = new PromoTicketDao(connStr);
            _proCateDao = new ProductCategoryDao(connStr);
            _prodCategSetDao = new ProductCategorySetDao(connStr);
            _prodpromoDao = new ProdPromoDao(connStr);

            int i = 0;
            //聲明鏈接數據庫
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            StringBuilder sbExSql = new StringBuilder();
            try
            {
                model.Replace4MySQL();
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                //開啟事物
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                StringBuilder Sql = new StringBuilder();


                mySqlCmd.CommandText = _prodpromoDao.DeleteProdProm(oldeventid);
                sbExSql.Append(mySqlCmd.CommandText);
                mySqlCmd.ExecuteNonQuery();//刪除ProdPromo

                mySqlCmd.CommandText = _proAllDao.DelPromAll(oldeventid);
                sbExSql.Append(mySqlCmd.CommandText);
                mySqlCmd.ExecuteNonQuery();//刪除PromAll


                //Sql.AppendFormat(_prodpromoDao.DeleteProdProm(model.event_id));
                //處理表product_category_set 和 prod_promo
                if (model.quanguan == 1)
                {
                    //根據category_id刪除product_category_set表數據
                    Sql.Clear();
                    Sql.AppendFormat(_prodCategSetDao.DeleteProdCateSet(model.category_id));
                    ////添加product_category_set數據
                    ProductCategorySet pcs = new ProductCategorySet();
                    pcs.Category_Id = model.category_id;
                    pcs.Product_Id = 999999;
                    pcs.Brand_Id = 0;
                    Sql.AppendFormat(_prodCategSetDao.SaveProdCategorySet(pcs));
                    mySqlCmd.CommandText = Sql.ToString();
                    sbExSql.Append(mySqlCmd.CommandText);
                    mySqlCmd.ExecuteNonQuery();//添加全館商品到PromAll
                    Sql.Clear();

                }
                else//非全館
                {
                    //刪除全館商品
                    ProductCategorySet qgSet = new ProductCategorySet();
                    qgSet.Category_Id = model.category_id;
                    qgSet.Product_Id = 999999;//全館商品刪除 id=999999
                    //根據category_id刪除product_category_set表數據
                    mySqlCmd.CommandText = _prodCategSetDao.DelProdCateSetByCPID(qgSet);
                    sbExSql.Append(mySqlCmd.CommandText);
                    mySqlCmd.ExecuteNonQuery();//刪除全館別商品999999

                    if (model.url_by == 1)//專區時 add by shuangshuang0420j 20150309 11：25 區分專區和非專區
                    {
                        //  Sql.Append(_prodpromoDao.DeleteProdProm(oldeventid));//修改前先刪除已有的數據
                        //選擇brand_id 的時候處理prod_promo  和product_category_set
                        if (model.brand_id != 0)//當品牌不為空時講該品牌下的所有商品加入set表
                        {

                            Sql.Append(_prodCategSetDao.DeleteProdCateSet(model.category_id));
                            mySqlCmd.CommandText = Sql.ToString();
                            sbExSql.Append(mySqlCmd.CommandText);
                            mySqlCmd.ExecuteNonQuery();
                            Sql.Clear();
                            QueryVerifyCondition query = new QueryVerifyCondition();
                            query.brand_id = Convert.ToUInt32(model.brand_id);
                            query.site_ids = model.site.ToString();
                            PromotionsMaintainDao _promoMainDao = new PromotionsMaintainDao(connStr);
                            int totalCount = 0;
                            List<QueryandVerifyCustom> qvcList = _promoMainDao.QueryByProSite(query, out totalCount, 0);

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
                                Sql.Append(_prodCategSetDao.SaveProdCategorySet(pcsModel));
                            }
                            if (!string.IsNullOrEmpty(Sql.ToString()))
                            {
                                mySqlCmd.CommandText = Sql.ToString();
                                sbExSql.Append(mySqlCmd.CommandText);
                                mySqlCmd.ExecuteNonQuery();
                                Sql.Clear();
                            }
                        }
                    }
                    else
                    { //非專區時
                        mySqlCmd.CommandText = _prodCategSetDao.DeleteProdCateSet(model.category_id);
                        sbExSql.Append(mySqlCmd.CommandText);
                        mySqlCmd.ExecuteNonQuery();
                        if (model.product_id != 0)
                        {
                            ProductCategorySet pcsModel = new ProductCategorySet();
                            pcsModel.Product_Id = Convert.ToUInt32(model.product_id);
                            pcsModel.Brand_Id = _prodDao.Query(new Product { Product_Id = pcsModel.Product_Id }).FirstOrDefault().Brand_Id;
                            pcsModel.Category_Id = model.category_id;
                            //Sql.Append(_prodCategSetDao.SaveProdCategorySet(pcsModel));
                            mySqlCmd.CommandText = _prodCategSetDao.SaveProdCategorySet(pcsModel);
                            sbExSql.Append(mySqlCmd.CommandText);
                            mySqlCmd.ExecuteNonQuery();//新增商品關係
                        }
                        else if (model.brand_id != 0)
                        {
                            QueryVerifyCondition query = new QueryVerifyCondition();
                            query.brand_id = Convert.ToUInt32(model.brand_id);
                            query.site_ids = model.site;
                            query.combination = 1;
                            int totalCount = 0;
                            List<QueryandVerifyCustom> qvcList = _promoMainDao.QueryByProSite(query, out totalCount, 0);
                            foreach (QueryandVerifyCustom qvcItem in qvcList)
                            {
                                ProductCategorySet pcsModel = new ProductCategorySet();
                                pcsModel.Product_Id = qvcItem.product_id;
                                pcsModel.Brand_Id = Convert.ToUInt32(model.brand_id);
                                pcsModel.Category_Id = model.category_id;
                                // arryList.Add(_prodCategSet.SaveProdCategorySet(pcsModel));
                                mySqlCmd.CommandText = _prodCategSetDao.SaveProdCategorySet(pcsModel);
                                sbExSql.Append(mySqlCmd.CommandText);
                                mySqlCmd.ExecuteNonQuery();//新增商品關係
                            }
                        }
                        else if (model.class_id != 0)
                        {
                            List<VendorBrand> brandIDs = _vendorBrandDao.GetClassBrandList(new VendorBrand { }, (uint)model.class_id);
                            foreach (VendorBrand item in brandIDs)
                            {
                                QueryVerifyCondition query = new QueryVerifyCondition();
                                query.brand_id = item.Brand_Id;
                                query.site_ids = model.site;
                                query.combination = 1;
                                int totalCount = 0;
                                List<QueryandVerifyCustom> qvcList = _promoMainDao.QueryByProSite(query, out totalCount, 0);
                                foreach (QueryandVerifyCustom qvcItem in qvcList)
                                {
                                    ProductCategorySet pcsModel = new ProductCategorySet();
                                    pcsModel.Product_Id = qvcItem.product_id;
                                    pcsModel.Brand_Id = Convert.ToUInt32(model.brand_id);
                                    pcsModel.Category_Id = model.category_id;
                                    //arryList.Add(_prodCategSet.SaveProdCategorySet(pcsModel));
                                    mySqlCmd.CommandText = _prodCategSetDao.SaveProdCategorySet(pcsModel);
                                    sbExSql.Append(mySqlCmd.CommandText);
                                    mySqlCmd.ExecuteNonQuery();//新增商品關係
                                }
                            }
                        }
                    }
                }
                //新增表promo_all數據
                //  Sql.AppendFormat(_proAllDao.DelPromAll(model.event_id));
                Sql.Clear();
                PromoAll pamodel = new PromoAll();
                pamodel.event_id = model.event_id;
                pamodel.product_id = model.product_id;
                pamodel.class_id = model.class_id;
                pamodel.brand_id = model.brand_id;
                pamodel.event_type = model.event_type;
                pamodel.category_id = Convert.ToInt32(model.category_id);
                pamodel.startTime = model.startdate;
                pamodel.end = model.enddate;
                pamodel.status = model.status;
                pamodel.kuser = model.kuser;
                pamodel.kdate = model.created;
                pamodel.muser = model.muser;
                pamodel.mdate = model.modified;
                Sql.AppendFormat(_proAllDao.SavePromAll(pamodel));
                //更新product_category表數據
                ProductCategory pcmodel = _proCateDao.GetModelById(Convert.ToUInt32(model.category_id));
                pcmodel.category_id = Convert.ToUInt32(model.category_id);
                pcmodel.banner_image = model.banner_image;
                pcmodel.category_link_url = model.category_link_url;
                pcmodel.category_display = Convert.ToUInt32(model.status);
                Sql.AppendFormat(_proCateDao.UpdateProdCate(pcmodel));

                //判斷送禮類型是否是機會,若是的話 修改編輯表promo_ticket
                if (model.gift_type == 2)
                { //新增數據到promo_ticket                   
                    if (model.ticket_id == 0)
                    {
                        PromoTicket pt = new PromoTicket();
                        pt.ticket_name = model.name;//機會name
                        pt.event_id = model.event_id;
                        pt.event_type = model.event_type;
                        pt.active_now = model.active_now;
                        pt.use_start = model.use_start;
                        pt.use_end = model.use_end;
                        pt.kuser = model.kuser;
                        pt.kdate = model.created;
                        pt.muser = model.muser;
                        pt.mdate = model.modified;
                        pt.valid_interval = model.valid_interval;
                        pt.status = model.status;
                        model.ticket_id = _ptDao.Save(pt);
                        model.ticket_name = pt.ticket_name;
                    }
                    //更新表數據
                    else
                    {
                        PromoTicket pt = _ptDao.Query(model.ticket_id);

                        pt.kuser = model.kuser;
                        pt.kdate = model.created;
                        pt.muser = model.muser;
                        pt.mdate = model.modified;

                        pt.ticket_name = model.ticket_name;//機會name
                        pt.event_id = model.event_id;
                        pt.event_type = model.event_type;
                        pt.active_now = model.active_now;
                        pt.use_start = model.use_start;
                        pt.use_end = model.use_end;
                        pt.valid_interval = model.valid_interval;
                        pt.status = model.status;

                        Sql.AppendFormat(_ptDao.UpdateSql(pt));
                    }
                }
                else if (model.ticket_id != 0)
                {
                    Sql.AppendFormat(_ptDao.DeleteSql(model.ticket_id));
                }


                if (model.gift_type == 3 || model.gift_type == 4)//當贈送購物金或抵用券時產生gift_id
                {
                    PromotionsAmountGiftCustom promGiftCustom = new PromotionsAmountGiftCustom();

                    promGiftCustom.product_id = 5669;
                    promGiftCustom.spec_type = 1;
                    promGiftCustom.spec_name = model.name;

                    promGiftCustom.Item_Alarm = 5;
                    promGiftCustom.Item_Stock = 9999;



                    mySqlCmd.CommandText = SaveProSpec(promGiftCustom);
                    uint specId = Convert.ToUInt32(mySqlCmd.ExecuteScalar());

                    promGiftCustom.Spec_Id_1 = specId;

                    mySqlCmd.CommandText = SaveProItem(promGiftCustom);

                    model.gift_id = Convert.ToInt32(mySqlCmd.ExecuteScalar());


                }

                //更新主表數據
                Sql.AppendFormat(UpdatePromoGift(model));
                mySqlCmd.CommandText = Sql.ToString();
                sbExSql.Append(mySqlCmd.CommandText);
                i += mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {//事物回滾
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PromotionsAmountGiftDao-->Update-->" + ex.Message + sbExSql.ToString(), ex);
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

        #region 更新主表數據 返回sql語句 + string UpdatePromoGift(PromotionsAmountGiftQuery model)
        /// <summary>
        /// 更新主表數據
        /// </summary>
        /// <param name="model">PromotionsAmountGiftQuery對象</param>
        /// <returns>更新主表數據的sql語句</returns>
        public string UpdatePromoGift(PromotionsAmountGiftQuery model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update  promotions_amount_gift  set ");
                sql.AppendFormat("name='{0}',group_id='{1}',class_id='{2}',brand_id='{3}',category_id='{4}',", model.name, model.group_id, model.class_id, model.brand_id, model.category_id);
                sql.AppendFormat("product_id='{0}',  type='{1}',  amount='{2}' ,  quantity='{3}' , `repeat` ='{4}' ,  gift_id='{5}' , ", model.product_id, model.type, model.amount, model.quantity, model.repeat ? '1' : '0', model.gift_id);
                sql.AppendFormat("deduct_welfare='{0}' , bonus_type='{1}' , mailer_id='{2}' , start='{3}' , end='{4}' , created='{5}' , ", model.deduct_welfare, model.bonus_type, model.mailer_id, CommonFunction.DateTimeToString(model.startdate), CommonFunction.DateTimeToString(model.enddate), CommonFunction.DateTimeToString(model.created));
                sql.AppendFormat(" modified='{0}' , active='{1}' , event_desc='{2}' , event_type='{3}' , condition_id='{4}' , device='{5}' , ", CommonFunction.DateTimeToString(model.modified), model.active, model.event_desc, model.event_type, model.condition_id, model.device);
                sql.AppendFormat(" payment_code='{0}' , gift_type='{1}' , ticket_id='{2}' , ticket_name='{3}' , count_by='{4}' , number='{5}' , num_limit='{6}' , active_now='{7}' , valid_interval='{8}' ,  ", model.payment_code, model.gift_type, model.ticket_id, model.ticket_name, model.count_by, model.number, model.num_limit, model.active_now, model.valid_interval);
                sql.AppendFormat("use_start='{0}' , use_end ='{1}',  muser='{2}'  , site='{3}', url_by='{4}' , status='{5}',vendor_coverage='{6}',gift_product_number='{7}'", CommonFunction.DateTimeToString(model.use_start), CommonFunction.DateTimeToString(model.use_end), model.muser, model.site, model.url_by, model.status, model.vendor_coverage, model.gift_product_number);
                sql.AppendFormat("  where id='{0}';", model.id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountGiftDao-->UpdatePromoGift-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        #endregion

        #region 刪除列表頁數據 以及與其有關的任何數據
        #region 刪除列表頁數據 +int Delete(int id, string event_id)
        /// <summary>
        /// 刪除列表頁數據
        /// </summary>
        /// <param name="id"></param>
        /// <param name="event_id">event_type+id</param>
        /// <returns>執行結果</returns>
        public int Delete(int id, string event_id)
        {
            int i = 0;
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            string sqlSW = "";
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

                PromotionsAmountGiftQuery query = new PromotionsAmountGiftQuery();
                if (id != 0)
                {
                    //更新主表數據 設置status為0
                    query = Select(id);

                    //更新表promo_ticket 軟刪除
                    //sqlSW += _promAmountReduceDao.Delete(querys.id);
                    if (query.ticket_id != 0)
                    {
                        sqlSW += _ptDao.DeleteSql(query.ticket_id);
                    }
                    //軟刪除表user_conditon
                    if (query.condition_id != 0)
                    {
                        sqlSW += _usconDao.DeleteUserCon(query.condition_id);
                    }

                    //軟刪除product_cate 和 product_category_set 
                    if (query.category_id != 0)
                    {
                        sqlSW += _proCateDao.Delete(query.category_id);

                        sqlSW += _prodCategSetDao.DelProdCateSet(query.category_id);
                    }
                    //軟刪除promo_all 和 prod_promo
                    if (!string.IsNullOrEmpty(event_id))
                    {
                        sqlSW += _prodpromoDao.DelProdProm(event_id);
                        sqlSW += _proAllDao.DeletePromAll(event_id);
                    }
                    //軟刪除主表
                    sqlSW += DeletePromoGift(query.id);
                }
                mySqlCmd.CommandText = sqlSW.ToString();
                sql.Append(mySqlCmd.CommandText);
                i += mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PromotionsAmountGiftDao-->Delete-->" + ex.Message + sql.ToString(), ex);
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

        #region 軟刪除主表數據  + string DeletePromoGift(int id)
        /// <summary>
        /// 軟刪除數據 根據id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string DeletePromoGift(int id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("set sql_safe_updates = 0;");
                sql.AppendFormat(" update promotions_amount_gift set status=0 where id={0} ;", id);
                sql.Append("set sql_safe_updates = 1;");
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountGiftDao-->DeletePromoGift-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        #endregion

        #region 根據model獲取datatable對象 +DataTable SelectDt(PromotionsAmountGift model)
        /// <summary>
        /// 根據model獲取datatable對象
        /// </summary>
        /// <param name="model">PromotionsAmountGift model對象</param>
        /// <returns>datatable類型數據</returns>
        public DataTable SelectDt(PromotionsAmountGift model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select pag.id,pag.name,pag.category_id,pag.group_id,pag.class_id,");
                sql.Append(" pag.brand_id,pag.product_id,pag.type,pag.amount,pag.quantity,pag.`repeat`,pag.gift_id,pag.ticket_id,");
                sql.Append(" pag.ticket_name,pag.deduct_welfare,pag.bonus_type,pag.mailer_id,pag.start 'startdate',pag.end 'enddate',pag.created,");
                sql.Append(" pag.modified,pag.active,pag.event_desc,pag.event_type,pag.condition_id,pag.device,pag.payment_code,pag.gift_type,pag.ticket_name,");
                sql.Append(" pag.count_by,pag.number,pag.num_limit,pag.valid_interval,pag.active_now,pag.use_start,pag.use_end,pag.kuser,pag.muser,pag.url_by,pag.site,pag.status,pag.vendor_coverage,pag.gift_product_number");
                sql.Append(" from promotions_amount_gift pag where 1=1 ");
                sql.AppendFormat(" and id='{0}';", model.id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountGiftDao-->SelectDt-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 更改列表頁數據啟用狀態 +int UpdateActive(PromotionsAmountGiftQuery model)
        /// <summary>
        /// 根據gift對象 更新啟用狀態
        /// </summary>
        /// <param name="model"> PromotionsAmountGiftQuery 對象</param>
        /// <returns>執行結果</returns>
        public int UpdateActive(PromotionsAmountGiftQuery model)
        {
            int i = 0;
            //聲明鏈接數據庫
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
                //開啟事物
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                StringBuilder Sql = new StringBuilder();

                Sql.Append(_prodpromoDao.DeleteProdProm(model.event_id));//先刪除在新增當active=0時照樣刪除只有active=1時才新增
                if (model.active == 1)
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
                        ppmodel.start = model.startdate;
                        ppmodel.end = model.enddate;
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
                        Sql.Append(_prodpromoDao.SaveProdProm(ppmodel));
                    }
                }

                //PromotionsAmountGiftDao proAmoGiftDao = new PromotionsAmountGiftDao(connStr);
                Sql.Append(UpdatePromoGift(model));
                mySqlCmd.CommandText = Sql.ToString();
                sql.Append(mySqlCmd.CommandText);
                i += mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PromotionsAmountGiftDao-->UpdateActive-->" + ex.Message + sql.ToString(), ex);
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

        #region 根據id獲取model 對象+PromotionsAmountGift GetModel(int id)
        /// <summary>
        /// 根據id獲取model 對象
        /// </summary>
        /// <param name="id"></param>
        /// <returns>PromotionsAmountGift對象</returns>
        public PromotionsAmountGift GetModel(int id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select id,event_type,name,event_id,category_id ");
                sql.Append(" from  promotions_amount_gift");
                sql.AppendFormat("  where id={0};", id);
                return _access.getSinggleObj<PromotionsAmountGift>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountGiftDao-->GetModel-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        public string SaveProSpec(PromotionsAmountGiftCustom prog)
        {
            prog.Replace4MySQL();

            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append(@" update serial set serial_value= serial_value+1 where serial_id=18;");
                sbSql.Append(@" insert into product_spec (");
                sbSql.Append(" spec_id, product_id,spec_type,spec_name) ");
                sbSql.AppendFormat(" values ((select serial_value from serial where serial_id=18),{0},{1},'{2}');select serial_value from serial where serial_id=18;", prog.product_id, prog.spec_type, prog.spec_name);
                //sbSql.Append(@" insert into product_spec (");
                //sbSql.Append(" spec_id, product_id,spec_type,spec_name) ");
                //sbSql.AppendFormat(" values ((select max(ps.spec_id) from product_spec ps)+1,{0},{1},'{2}');select max(spec_id) from product_spec;", prog.product_id, prog.spec_type, prog.spec_name);

                return sbSql.ToString();

            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountGiftDao-->SaveProSpec-->" + ex.Message, ex);
            }

        }

        public string SaveProItem(PromotionsAmountGiftCustom prog)
        {
            prog.Replace4MySQL();

            try
            {
                StringBuilder piSql = new StringBuilder();
                piSql.Append(@"update serial set serial_value= serial_value+1 where serial_id=19;");//更改serial表中的product_item 主鍵
                piSql.Append(@"insert into product_item (`item_id`,`product_id`,`spec_id_1`,`Item_Stock`,`Item_Alarm`)values(");
                piSql.AppendFormat("(select serial_value from serial where serial_id=19),{0},{1},{2},{3});select serial_value from serial where serial_id=19;", prog.product_id, prog.Spec_Id_1, prog.Item_Stock, prog.Item_Alarm);
                //piSql.Append(@"insert into product_item (`item_id`,`product_id`,`spec_id_1`,`Item_Stock`,`Item_Alarm`)values(");
                //piSql.AppendFormat("(select max(pi.item_id) from product_item pi)+1,{0},{1},{2},{3});select max(item_id) from product_item;", prog.product_id, prog.Spec_Id_1, prog.Item_Stock, prog.Item_Alarm);

                return piSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountGiftDao-->SaveProItem-->" + ex.Message, ex);
            }

        }

        //新添加
        #region 新增數據 第一步試吃/紅利抵用保存
        #region 事物新增主表數據和product_category表數據 + int Save(PromotionsAmountGiftQuery query)
        public int TryEatAndDiscountSave(PromotionsAmountGiftQuery query)
        {
            _proCateDao = new ProductCategoryDao(connStr);
            query.Replace4MySQL();
            int id = 0;
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
                //#region 保存第一步到product_category 獲取prodduct_amount_fare的category_id
                ProductCategory pmodel = new ProductCategory();
                pmodel.category_name = query.name;
                pmodel.category_createdate = (uint)BLL.gigade.Common.CommonFunction.GetPHPTime();
                pmodel.category_father_id = query.category_father_id;
                pmodel.category_ipfrom = query.category_ipfrom;
                pmodel.category_display = Convert.ToUInt32(query.status);

                mySqlCmd.CommandText = _proCateDao.SaveCategory(pmodel);
                sbExSql.Append(mySqlCmd.CommandText);
                query.category_id = Convert.ToUInt32(mySqlCmd.ExecuteScalar());

                //修改表serial
                Serial serial = new Serial();
                serial.Serial_id = 12;
                serial.Serial_Value = Convert.ToUInt32(query.category_id);
                mySqlCmd.CommandText = _serialDao.UpdateAutoIncreament(serial);
                sbExSql.Append(mySqlCmd.CommandText);
                id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                //#endregion

                PromotionsAmountGift pgmodel = new PromotionsAmountGift();
                pgmodel.name = query.name;
                pgmodel.event_desc = query.event_desc;
                pgmodel.vendor_coverage = query.vendor_coverage;
                pgmodel.event_type = query.event_type;
                pgmodel.freight_price = query.freight_price;
                pgmodel.kuser = query.kuser;
                pgmodel.created = query.created;
                pgmodel.status = query.status;
                pgmodel.category_id = query.category_id;
                pgmodel.dividend = query.dividend;
                mySqlCmd.CommandText = TryEatAndDiscountSavePromoGift(pgmodel);
                sbExSql.Append(mySqlCmd.CommandText);
                id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PromotionsAmountGiftDao-->Save-->" + ex.Message + sbExSql.ToString(), ex);
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


        #region 新增主表數據 +string TryEatAndDiscountSavePromoGift(PromotionsAmountGift query)
        public string TryEatAndDiscountSavePromoGift(PromotionsAmountGift query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("insert into  promotions_amount_gift (");
                sql.AppendFormat("name,group_id,class_id,brand_id,category_id,product_id,type,freight_price,amount,`repeat` ,gift_id,quantity,point,dollar,");
                sql.AppendFormat("deduct_welfare,bonus_type,mailer_id,start,end,created,modified,active,event_desc,event_type,condition_id,device,gift_mundane, ");
                sql.AppendFormat(" payment_code,gift_type,ticket_id,ticket_name,count_by,number,num_limit,active_now,valid_interval, bonus_state, ");
                sql.AppendFormat("use_start,use_end ,kuser,muser,url_by,url,banner_file,status,site,vendor_coverage,gift_product_number,dividend) values ");
                sql.AppendFormat("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}',", query.name, query.group_id, query.class_id, query.brand_id,
                 query.category_id, query.product_id, query.type, query.freight_price, query.amount, query.repeat ? '1' : '0', query.gift_id, query.quantity, query.point, query.dollar);
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},'{8}','{9}','{10}','{11}','{12}',", query.deduct_welfare, query.bonus_type, query.mailer_id, CommonFunction.DateTimeToString(query.start),
                 CommonFunction.DateTimeToString(query.end), CommonFunction.DateTimeToString(query.created), CommonFunction.DateTimeToString(query.modified), query.active, query.event_desc, query.event_type, query.condition_id, query.device, query.gift_mundane);
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',", query.payment_code, query.gift_type, query.ticket_id, query.ticket_name, query.count_by, query.number,
                 query.num_limit, query.active_now, query.valid_interval, query.bonus_state);
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}');select @@identity;", CommonFunction.DateTimeToString(query.use_start), CommonFunction.DateTimeToString(query.use_end), query.kuser, query.muser, query.url_by, query.url, query.banner_file, query.status, query.site, query.vendor_coverage, query.gift_product_number, query.dividend);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountGiftDao-->SavePromoGift-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #endregion

        #region 更新數據  事物更新 +int Update(PromotionsAmountGiftQuery model, string oldeventid)
        #region  事物更新 +int Update(PromotionsAmountGiftQuery model, string oldeventid)
        public int TryEatAndDiscountUpdate(PromotionsAmountGiftQuery model, string oldeventid)
        {
            //實例dao對象
            _ptDao = new PromoTicketDao(connStr);
            _proCateDao = new ProductCategoryDao(connStr);
            _prodCategSetDao = new ProductCategorySetDao(connStr);
            _prodpromoDao = new ProdPromoDao(connStr);

            int i = 0;
            //聲明鏈接數據庫
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
                //開啟事物
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                StringBuilder Sql = new StringBuilder();


                mySqlCmd.CommandText = _prodpromoDao.DeleteProdProm(oldeventid);
                sbExSql.Append(mySqlCmd.CommandText);
                mySqlCmd.ExecuteNonQuery();//刪除ProdPromo

                mySqlCmd.CommandText = _proAllDao.DelPromAll(oldeventid);
                sbExSql.Append(mySqlCmd.CommandText);
                mySqlCmd.ExecuteNonQuery();//刪除PromAll


                //Sql.AppendFormat(_prodpromoDao.DeleteProdProm(model.event_id));
                //處理表product_category_set 和 prod_promo
                if (model.quanguan == 1)
                {
                    //根據category_id刪除product_category_set表數據
                    Sql.Clear();
                    Sql.AppendFormat(_prodCategSetDao.DeleteProdCateSet(model.category_id));
                    ////添加product_category_set數據
                    ProductCategorySet pcs = new ProductCategorySet();
                    pcs.Category_Id = model.category_id;
                    pcs.Product_Id = 999999;
                    pcs.Brand_Id = 0;
                    Sql.AppendFormat(_prodCategSetDao.SaveProdCategorySet(pcs));
                    mySqlCmd.CommandText = Sql.ToString();
                    sbExSql.Append(mySqlCmd.CommandText);
                    mySqlCmd.ExecuteNonQuery();//添加全館商品到PromAll


                }
                else//非全館
                {
                    //刪除全館商品
                    ProductCategorySet qgSet = new ProductCategorySet();
                    qgSet.Category_Id = model.category_id;
                    qgSet.Product_Id = 999999;//全館商品刪除 id=999999
                    //根據category_id刪除product_category_set表數據
                    mySqlCmd.CommandText = _prodCategSetDao.DelProdCateSetByCPID(qgSet);
                    sbExSql.Append(mySqlCmd.CommandText);
                    mySqlCmd.ExecuteNonQuery();//刪除全館別商品999999


                    //  Sql.Append(_prodpromoDao.DeleteProdProm(oldeventid));//修改前先刪除已有的數據
                    //選擇brand_id 的時候處理prod_promo  和product_category_set
                    if (model.brand_id != 0)//當品牌不為空時講該品牌下的所有商品加入set表
                    {

                        Sql.Append(_prodCategSetDao.DeleteProdCateSet(model.category_id));
                        mySqlCmd.CommandText = Sql.ToString();
                        sbExSql.Append(mySqlCmd.CommandText);
                        mySqlCmd.ExecuteNonQuery();
                        Sql.Clear();
                        QueryVerifyCondition query = new QueryVerifyCondition();
                        query.brand_id = Convert.ToUInt32(model.brand_id);
                        query.site_ids = model.site.ToString();
                        PromotionsMaintainDao _promoMainDao = new PromotionsMaintainDao(connStr);
                        int totalCount = 0;
                        List<QueryandVerifyCustom> qvcList = _promoMainDao.QueryByProSite(query, out totalCount, 0);
                        Sql.Clear();
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
                            Sql.Append(_prodCategSetDao.SaveProdCategorySet(pcsModel));
                        }
                        if (!string.IsNullOrEmpty(Sql.ToString()))
                        {
                            mySqlCmd.CommandText = Sql.ToString();
                            sbExSql.Append(mySqlCmd.CommandText);
                            mySqlCmd.ExecuteNonQuery();
                        }
                    }
                }
                //新增表promo_all數據
                //  Sql.AppendFormat(_proAllDao.DelPromAll(model.event_id));

                Sql.Clear();
                PromoAll pamodel = new PromoAll();
                pamodel.event_id = model.event_id;
                pamodel.product_id = model.product_id;
                pamodel.class_id = model.class_id;
                pamodel.brand_id = model.brand_id;
                pamodel.event_type = model.event_type;
                pamodel.category_id = Convert.ToInt32(model.category_id);
                pamodel.startTime = model.startdate;
                pamodel.end = model.enddate;
                pamodel.status = model.status;
                pamodel.kuser = model.kuser;
                pamodel.kdate = model.created;
                pamodel.muser = model.muser;
                pamodel.mdate = model.modified;
                Sql.AppendFormat(_proAllDao.SavePromAll(pamodel));
                //更新product_category表數據
                ProductCategory pcmodel = _proCateDao.GetModelById(Convert.ToUInt32(model.category_id));
                pcmodel.category_id = Convert.ToUInt32(model.category_id);
                pcmodel.banner_image = model.banner_image;
                pcmodel.category_link_url = model.category_link_url;
                pcmodel.category_display = Convert.ToUInt32(model.status);
                Sql.AppendFormat(_proCateDao.UpdateProdCate(pcmodel));

                //判斷送禮類型是否是機會,若是的話 修改編輯表promo_ticket
                if (model.gift_type == 2)
                { //新增數據到promo_ticket                   
                    if (model.ticket_id == 0)
                    {
                        PromoTicket pt = new PromoTicket();
                        pt.ticket_name = model.name;//機會name
                        pt.event_id = model.event_id;
                        pt.event_type = model.event_type;
                        pt.active_now = model.active_now;
                        pt.use_start = model.use_start;
                        pt.use_end = model.use_end;
                        pt.kuser = model.kuser;
                        pt.kdate = model.created;
                        pt.muser = model.muser;
                        pt.mdate = model.modified;
                        pt.valid_interval = model.valid_interval;
                        pt.status = model.status;
                        model.ticket_id = _ptDao.Save(pt);
                        model.ticket_name = pt.ticket_name;
                    }
                    //更新表數據
                    else
                    {
                        PromoTicket pt = _ptDao.Query(model.ticket_id);

                        pt.kuser = model.kuser;
                        pt.kdate = model.created;
                        pt.muser = model.muser;
                        pt.mdate = model.modified;

                        pt.ticket_name = model.ticket_name;//機會name
                        pt.event_id = model.event_id;
                        pt.event_type = model.event_type;
                        pt.active_now = model.active_now;
                        pt.use_start = model.use_start;
                        pt.use_end = model.use_end;
                        pt.valid_interval = model.valid_interval;
                        pt.status = model.status;

                        Sql.AppendFormat(_ptDao.UpdateSql(pt));
                    }
                }
                else if (model.ticket_id != 0)
                {
                    Sql.AppendFormat(_ptDao.DeleteSql(model.ticket_id));
                }

                //更新主表數據
                Sql.AppendFormat(TryEatAndDiscountUpdatePromoGift(model));
                mySqlCmd.CommandText = Sql.ToString();
                sbExSql.Append(mySqlCmd.CommandText);
                i += mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {//事物回滾
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PromotionsAmountGiftDao-->Update-->" + ex.Message + sbExSql.ToString(), ex);
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

        #region 更新主表數據 返回sql語句 + string UpdatePromoGift(PromotionsAmountGiftQuery model)
        /// <summary>
        /// 更新主表數據
        /// </summary>
        /// <param name="model">PromotionsAmountGiftQuery對象</param>
        /// <returns>更新主表數據的sql語句</returns>
        public string TryEatAndDiscountUpdatePromoGift(PromotionsAmountGiftQuery model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update  promotions_amount_gift  set ");
                sql.AppendFormat("name='{0}',group_id='{1}',class_id='{2}',brand_id='{3}',category_id='{4}',delivery_category='{5}',freight_price='{6}',gift_mundane='{7}',point='{8}',dollar='{9}', ", model.name, model.group_id, model.class_id, model.brand_id, model.category_id, model.delivery_category, model.freight_price, model.gift_mundane, model.point, model.dollar);
                sql.AppendFormat("product_id='{0}',  type='{1}',  amount='{2}' ,  quantity='{3}' , `repeat` ='{4}' ,  gift_id='{5}' ,bonus_state='{6}', ", model.product_id, model.type, model.amount, model.quantity, model.repeat ? '1' : '0', model.gift_id, model.bonus_state);
                sql.AppendFormat("deduct_welfare='{0}' , bonus_type='{1}' , mailer_id='{2}' , start='{3}' , end='{4}' , created='{5}' , ", model.deduct_welfare, model.bonus_type, model.mailer_id, CommonFunction.DateTimeToString(model.startdate), CommonFunction.DateTimeToString(model.enddate), CommonFunction.DateTimeToString(model.created));
                sql.AppendFormat(" modified='{0}' , active='{1}' , event_desc='{2}' , event_type='{3}' , condition_id='{4}' , device='{5}' , ", CommonFunction.DateTimeToString(model.modified), model.active, model.event_desc, model.event_type, model.condition_id, model.device);
                sql.AppendFormat(" payment_code='{0}' , gift_type='{1}' , ticket_id='{2}' , ticket_name='{3}' , count_by='{4}' , number='{5}' , num_limit='{6}' , active_now='{7}' , valid_interval='{8}' ,  ", model.payment_code, model.gift_type, model.ticket_id, model.ticket_name, model.count_by, model.number, model.num_limit, model.active_now, model.valid_interval);
                sql.AppendFormat("use_start='{0}' , use_end ='{1}',  muser='{2}'  , site='{3}', url_by='{4}' , status='{5}',vendor_coverage='{6}',gift_product_number='{7}',event_id='{8}',dividend='{9}'", CommonFunction.DateTimeToString(model.use_start), CommonFunction.DateTimeToString(model.use_end), model.muser, model.site, model.url_by, model.status, model.vendor_coverage, model.gift_product_number, model.event_id, model.dividend);
                sql.AppendFormat("  where id='{0}';", model.id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountGiftDao-->UpdatePromoGift-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        #endregion
    }
}
