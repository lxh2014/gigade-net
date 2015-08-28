
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model.Query;
using BLL.gigade.Common;
using BLL.gigade.Dao.Impl;
using MySql.Data.MySqlClient;
using BLL.gigade.Model;
using System.Data; 

namespace BLL.gigade.Dao
{
    public class PromoAdditionalPriceDao : IPromoAdditionalPriceDao
    {
        private IDBAccess _access;
        private string connStr;
        ProductCategoryDao _proCateDao = new ProductCategoryDao("");
        ProductCategorySetDao _prodCategSetDao = new ProductCategorySetDao("");
        ProdPromoDao _prodPromoDao = new ProdPromoDao("");
        PromoAllDao _promoAllDao = new PromoAllDao("");
        UserConditionDao _usconDao = new UserConditionDao("");
        PromoDiscountDao _promoDisDao = new PromoDiscountDao("");
        SerialDao _serialDao = new SerialDao("");

        public PromoAdditionalPriceDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
        }

        #region 不同商品固定價 同品加固定價 不同品加不同價格 列表頁 +List<PromoAdditionalPriceQuery> QueryAll(PromoAdditionalPriceQuery query, out int totalCount)
        public List<PromoAdditionalPriceQuery> QueryAll(PromoAdditionalPriceQuery query, out int totalCount)
        {//促銷商品類別和銀行沒有加!
            StringBuilder str = new StringBuilder();
            StringBuilder strall = new StringBuilder();
            StringBuilder strcounts = new StringBuilder();
            try
            {
                strcounts.AppendFormat("select count(PA.id) as totalcounts from promo_additional_price as PA ");
                //strall.AppendFormat("SELECT CONCAT(PA.event_type ,right(CONCAT('00000000',PA.id),6)) as 'event_id',PA.id,PA.deliver_type,PA.website,PA.device,event_name,event_desc,PC.banner_image,PC.category_link_url,VUG.group_name,fixed_price,buy_limit,TP.parameterName as deliver_name,TP1.parameterName as device_name,`start` as starts,`end`,active,PA.condition_id,PA.category_id,PA.discount,PA.left_category_id,PA.right_category_id,PA.url_by from promo_additional_price AS PA ");
                strall.AppendFormat("SELECT PA.event_type,PA.id,PA.deliver_type,PA.website,PA.device,event_name,event_desc,PC.banner_image,PC.category_link_url,VUG.group_name,fixed_price,buy_limit,`start` as starts,`end`,active,PA.condition_id,PA.category_id,PA.discount,PA.left_category_id,PA.right_category_id,PA.url_by,PA.muser,mu.user_username from promo_additional_price AS PA ");

                str.AppendFormat(" left join vip_user_group as VUG on PA.group_id=VUG.group_id ");
                str.AppendFormat(" LEFT JOIN product_category as PC ON PA.category_id=PC.category_id ");
                //str.AppendFormat(" LEFT JOIN t_parametersrc as TP on PA.deliver_type = TP.parameterCode AND TP.parameterType='product_freight'");
                //str.AppendFormat(" left join t_parametersrc as TP1 on PA.device = TP1.parameterCode AND TP1.parameterType='device'");
                //str.AppendFormat(" left join (select * from  t_parametersrc  where parameterType='event_type' ) ET ON PA.event_type = ET.parameterCode");
                str.Append(" LEFT JOIN manage_user mu ON PA.muser=mu.user_id");
                str.AppendFormat(" where PA.status=1 and PA.event_type= '{0}'", query.event_type);
                totalCount = 0;
                if (query.expired == 1)//是未過期
                {
                    str.AppendFormat(" and end >= '{0}'", CommonFunction.DateTimeToString(DateTime.Now));
                }
                else if (query.expired == 0)
                {
                    str.AppendFormat(" and end < '{0}'", CommonFunction.DateTimeToString(DateTime.Now));
                }
                totalCount = 0;
                if (query.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(strcounts.ToString() + str.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalcounts"]);
                    }
                    str.AppendFormat("order by PA.id DESC limit {0},{1} ", query.Start, query.Limit);
                }
                return _access.getDataTableForObj<PromoAdditionalPriceQuery>(strall.ToString() + str.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoAdditionalPriceDao-->QueryAll-->" + ex.Message + strall.ToString() + str.ToString(), ex);
            }
        }
        //public List<PromoAdditionalPriceQuery> QueryAll(PromoAdditionalPriceQuery query, out int totalCount)
        //{//促銷商品類別和銀行沒有加!
        //    StringBuilder str = new StringBuilder();
        //    StringBuilder strall = new StringBuilder();
        //    StringBuilder strcounts = new StringBuilder();
        //    try
        //    {
        //        strcounts.AppendFormat("select count(PA.id) as totalcounts from promo_additional_price as PA ");
        //        strall.AppendFormat("SELECT CONCAT(PA.event_type ,right(CONCAT('00000000',PA.id),6)) as 'event_id',PA.id,PA.deliver_type,PA.website,PA.device,event_name,event_desc,PC.banner_image,PC.category_link_url,VUG.group_name,fixed_price,buy_limit,TP.parameterName as deliver_name,TP1.parameterName as device_name,`start` as starts,`end`,active,PA.condition_id,PA.category_id,PA.discount,PA.left_category_id,PA.right_category_id,PA.url_by from promo_additional_price AS PA ");
        //        str.AppendFormat(" left join vip_user_group as VUG on PA.group_id=VUG.group_id ");
        //        str.AppendFormat(" LEFT JOIN product_category as PC ON PA.category_id=PC.category_id ");
        //        str.AppendFormat(" LEFT JOIN t_parametersrc as TP on PA.deliver_type = TP.parameterCode AND TP.parameterType='product_freight'");
        //        str.AppendFormat(" left join t_parametersrc as TP1 on PA.device = TP1.parameterCode AND TP1.parameterType='device'");
        //        str.AppendFormat(" left join (select * from  t_parametersrc  where parameterType='event_type' ) ET ON PA.event_type = ET.parameterCode");
        //        str.AppendFormat(" where PA.status=1 and PA.event_type= '{0}'", query.event_type);
        //        totalCount = 0;
        //        if (query.expired == 1)//是未過期
        //        {
        //            str.AppendFormat(" and end >= '{0}'", CommonFunction.DateTimeToString(DateTime.Now));
        //        }
        //        else if (query.expired == 0)
        //        {
        //            str.AppendFormat(" and end < '{0}'", CommonFunction.DateTimeToString(DateTime.Now));
        //        }
        //        totalCount = 0;
        //        if (query.IsPage)
        //        {
        //            System.Data.DataTable _dt = _access.getDataTable(strcounts.ToString() + str.ToString());
        //            if (_dt != null && _dt.Rows.Count > 0)
        //            {
        //                totalCount = Convert.ToInt32(_dt.Rows[0]["totalcounts"]);
        //            }
        //            str.AppendFormat("order by PA.id DESC limit {0},{1} ", query.Start, query.Limit);
        //        }
        //        return _access.getDataTableForObj<PromoAdditionalPriceQuery>(strall.ToString() + str.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("PromoAdditionalPriceDao-->QueryAll-->" + ex.Message + strall.ToString() + str.ToString(), ex);
        //    }
        //}
        #endregion

        #region 不同商品固定價 同品加固定價 不同品加不同價格 新增第一步 +int InsertFirst(PromoAdditionalPrice model)
        public int InsertFirst(PromoAdditionalPrice model)
        {
            model.Replace4MySQL();
            int id = 0;
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            string mysql = string.Empty;
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;

                string father_id = _access.getDataTable(string.Format("SELECT parameterProperty from t_parametersrc where parameterCode='{0}'", model.event_type)).Rows[0][0].ToString();
                //insert ProductCategory 獲取category_id
                ProductCategory pmodel = new ProductCategory();
                pmodel.category_father_id = Convert.ToUInt32(father_id);
                pmodel.category_name = model.event_name;
                pmodel.category_createdate = (uint)BLL.gigade.Common.CommonFunction.GetPHPTime();
                pmodel.category_display = 0;
                mySqlCmd.CommandText = _proCateDao.SaveCategory(pmodel);
                model.category_id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                mysql = mySqlCmd.CommandText;
                //修改表serial
                Serial serial = new Serial();
                serial.Serial_id = 12;
                serial.Serial_Value = Convert.ToUInt32(model.category_id);
                mySqlCmd.CommandText = _serialDao.UpdateAutoIncreament(serial);
                id = Convert.ToInt32(mySqlCmd.ExecuteScalar());

                if (model.event_type != "A1")
                {
                    pmodel.category_father_id = Convert.ToUInt32(model.category_id);
                    pmodel.category_name = "左邊";
                    mySqlCmd.CommandText = _proCateDao.SaveCategory(pmodel);
                    model.left_category_id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                    //修改表serial
                    Serial serialred = new Serial();
                    serialred.Serial_id = 12;
                    serialred.Serial_Value = Convert.ToUInt32(model.left_category_id);
                    mySqlCmd.CommandText = _serialDao.UpdateAutoIncreament(serialred);
                    id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                    //mySqlCmd.CommandText = string.Format("INSERT INTO product_category(category_father_id,category_name,category_display,category_show_mode,category_createdate,status) values('{0}','{1}','{2}','{3}','{4}',1); select @@identity ;", model.category_id, "紅", "1", "0", CommonFunction.GetPHPTime(model.created.ToString()));
                    pmodel.category_name = "右邊";
                    mySqlCmd.CommandText = _proCateDao.SaveCategory(pmodel);
                    model.right_category_id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                    //修改表serial
                    Serial serialgreen = new Serial();
                    serialgreen.Serial_id = 12;
                    serialgreen.Serial_Value = Convert.ToUInt32(model.right_category_id);
                    mySqlCmd.CommandText = _serialDao.UpdateAutoIncreament(serialgreen);
                    id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                    PromoAdditionalPrice papModel = new PromoAdditionalPrice();
                    papModel.event_name = model.event_name;
                    papModel.event_desc = model.event_desc;
                    papModel.event_type = model.event_type;
                    papModel.kuser = model.kuser;
                    papModel.created = Convert.ToDateTime(CommonFunction.DateTimeToString(model.created));
                    papModel.active = model.active;
                    papModel.category_id = model.category_id;
                    papModel.status = 0;
                    papModel.left_category_id = model.left_category_id;
                    papModel.right_category_id = model.right_category_id;
                    mySqlCmd.CommandText = SavePromoAdditionalPrice(papModel);
                    mysql = mysql + mySqlCmd.CommandText;
                    id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                    mySqlCmd.Transaction.Commit();
                }
                else
                {
                    //insert PromoAdditionalPrice 主表 狀態為0
                    PromoAdditionalPrice papModel = new PromoAdditionalPrice();
                    papModel.event_name = model.event_name;
                    papModel.event_desc = model.event_desc;
                    papModel.event_type = model.event_type;
                    papModel.kuser = model.kuser;
                    papModel.created = Convert.ToDateTime(CommonFunction.DateTimeToString(model.created));
                    papModel.active = model.active;
                    papModel.category_id = model.category_id;
                    papModel.status = 0;
                    mySqlCmd.CommandText = SavePromoAdditionalPrice(papModel);
                    mysql = mysql + mySqlCmd.CommandText;
                    id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                    mySqlCmd.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PromoAdditionalPriceDao-->InsertFirst-->" + ex.Message + mysql.ToString(), ex);
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
        #region  不同商品固定價 同品加固定價 不同品加不同價格 新增第二步 +InsertSecond
        public int InsertSecond(PromoAdditionalPrice m, PromoAdditionalPriceQuery mq)
        {
            int id = 0;
            StringBuilder sb = new StringBuilder();
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
                sb.AppendFormat(@"UPDATE promo_additional_price SET condition_id='{1}',group_id='{2}',`start`='{3}',`end`='{4}',modified='{5}',deliver_type='{6}',device='{7}',muser='{8}',status='{9}',active='{10}',fixed_price='{11}',buy_limit='{12}',website='{13}',url_by='{14}',kuser='{15}',created='{16}',discount='{17}' where id={0} ; ", m.id, m.condition_id, m.group_id, CommonFunction.DateTimeToString(m.starts), CommonFunction.DateTimeToString(m.end), CommonFunction.DateTimeToString(m.modified), m.deliver_type, m.device, m.muser, m.status, Convert.ToInt32(m.active), m.fixed_price, m.buy_limit, m.website, m.url_by, m.kuser, CommonFunction.DateTimeToString(m.created), m.discount);

                #region 操作ProductCategory
                ProductCategoryDao _categoryDao = new ProductCategoryDao(connStr);
                ProductCategory pcmodel = _categoryDao.GetModelById(Convert.ToUInt32(m.category_id));
                pcmodel.category_id = Convert.ToUInt32(m.category_id);
                pcmodel.banner_image = mq.banner_image;
                pcmodel.category_link_url = mq.category_link_url;
                pcmodel.category_display = Convert.ToUInt32(m.status);
                pcmodel.category_ipfrom = mq.category_ipfrom;
                sb.Append(_proCateDao.UpdateProdCate(pcmodel));
                #endregion
                #region 操作PromoAll
                PromoAll pamodel = new PromoAll();
                pamodel.event_id = mq.event_id;
                pamodel.event_type = m.event_type;
                pamodel.category_id = Convert.ToInt32(m.category_id);
                pamodel.startTime = m.starts;
                pamodel.end = m.end;
                pamodel.status = m.status;
                pamodel.kuser = m.muser;
                pamodel.kdate = DateTime.Now;
                pamodel.muser = m.muser;
                pamodel.mdate = pamodel.kdate;
                sb.Append(_promoAllDao.SavePromAll(pamodel));
                #endregion
                mySqlCmd.CommandText = sb.ToString();
                id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PromoAdditionalPriceDao-->InsertSecond-->" + ex.Message, ex);
            }
            finally
            {
                mySqlConn.Close();
            }
            return id;
        }
        #endregion

        #region  不同商品固定價 同品加固定價 不同品加不同價格 插入數據到promo_additional_price+SavePromoAdditionalPrice
        private string SavePromoAdditionalPrice(PromoAdditionalPrice papModel)
        {
            StringBuilder sbSql = new StringBuilder();
            try
            {
                sbSql.AppendFormat("insert into promo_additional_price (event_name,event_desc,event_type,kuser,created,active,category_id,left_category_id,right_category_id) values('{0}','{1}','{2}','{3}','{4}',{5},'{6}',{7},{8});select @@identity;", papModel.event_name, papModel.event_desc, papModel.event_type, papModel.kuser, CommonFunction.DateTimeToString(papModel.created), papModel.active, papModel.category_id, papModel.left_category_id, papModel.right_category_id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoAdditionalPriceDao-->SavePromoAdditionalPrice-->" + ex.Message, ex);
            }
            return sbSql.ToString();
        }
        #endregion

        #region 不同商品固定價 同品加固定價 不同品加不同價格 編輯 +int Update(PromoAdditionalPrice m, PromoAdditionalPriceQuery mq)
        public int Update(PromoAdditionalPrice m, PromoAdditionalPriceQuery mq)
        {
            int id = 0;
            StringBuilder sb = new StringBuilder();
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
                sb.Append(_prodPromoDao.DeleteProdProm(mq.event_id));
                sb.AppendFormat(@"UPDATE promo_additional_price SET condition_id='{1}',group_id='{2}',`start`='{3}',`end`='{4}',modified='{5}',deliver_type='{6}',device='{7}',muser='{8}',website='{9}',event_name='{10}',event_desc='{11}',fixed_price='{12}',buy_limit='{13}',url_by='{14}',discount='{15}',active={16} where id={0} ; ", m.id, m.condition_id, m.group_id, CommonFunction.DateTimeToString(m.starts), CommonFunction.DateTimeToString(m.end), CommonFunction.DateTimeToString(m.modified), m.deliver_type, m.device, m.muser, m.website, m.event_name, m.event_desc, m.fixed_price, m.buy_limit, m.url_by, m.discount, m.active);
                #region 操作修改ProductCategory
                ProductCategoryDao _categoryDao = new ProductCategoryDao(connStr);
                ProductCategory pcmodel = _categoryDao.GetModelById(Convert.ToUInt32(m.category_id));
                pcmodel.category_id = Convert.ToUInt32(m.category_id);
                pcmodel.banner_image = mq.banner_image;
                pcmodel.category_link_url = mq.category_link_url;
                pcmodel.category_name = m.event_name;
                pcmodel.category_display = Convert.ToUInt32(m.status);
                pcmodel.category_ipfrom = mq.category_ipfrom;
                pcmodel.category_updatedate = (uint)BLL.gigade.Common.CommonFunction.GetPHPTime();
                sb.Append(_proCateDao.UpdateProdCate(pcmodel));
                #endregion
                mySqlCmd.CommandText = sb.ToString();
                id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PromoAdditionalPriceDao-->Update-->" + ex.Message + mySqlCmd.CommandText.ToString(), ex);
            }
            finally
            {
                mySqlConn.Close();
            }
            return id;
        }
        #endregion

        #region 不同商品固定價 同品加固定價 不同品加不同價格 刪除 +int Delete(int i, string str)
        public int Delete(int i, string str)
        {
            int j = 0;
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
                PromoAdditionalPriceQuery query = new PromoAdditionalPriceQuery();
                if (i != 0)
                {//更新主表數據 設置status為0                    
                    query = Select(i);
                    if (query.condition_id != 0)
                    {//軟刪除表user_conditon
                        sqlSW += _usconDao.DeleteUserCon(query.condition_id);
                    }
                    if (query.category_id != 0)
                    {//軟刪除product_cate 和 product_category_set 
                        //sqlSW += _proCateDao.Delete(query.category_id);
                        //sqlSW += _prodCategSetDao.DelProdCateSet(query.category_id);
                    }
                    if (!string.IsNullOrEmpty(str))
                    {//軟刪除promo_all 和 prod_promo
                        sqlSW += _prodPromoDao.DelProdProm(str);
                        sqlSW += _promoAllDao.DeletePromAll(str);
                    }
                    //軟刪除主表
                    sqlSW += Deletepromoadditionalprice(query.id);
                }
                mySqlCmd.CommandText = sqlSW.ToString();
                j += mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PromoAdditionalPriceDao-->Delete-->" + ex.Message + mySqlCmd.CommandText, ex);
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

        #region 不同商品固定價 同品加固定價 不同品加不同價格 獲取刪除sql語句 +string Deletepromoadditionalprice(int id)
        public string Deletepromoadditionalprice(int id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update promo_additional_price set status=0 where id={0} ;", id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoAdditionalPriceDao-->Deletepromoadditionalprice-->" + ex.Message, ex);
            }
            return sql.ToString();
        }
        #endregion

        #region 不同商品固定價 同品加固定價 不同品加不同價格 根據id獲取數據 +PromoAdditionalPriceQuery Select(int id)
        public PromoAdditionalPriceQuery Select(int id)
        {
            StringBuilder str = new StringBuilder();
            try
            {
                //str.AppendFormat("SELECT CONCAT(PA.event_type ,right(CONCAT('00000000',PA.id),6)) as 'event_id',PC.banner_image,PC.category_link_url,VUG.group_name,TP.parameterName as deliver_name,TP1.parameterName as device_name,PA.id,PA.event_name,PA.event_desc,PA.event_type,PA.condition_id,PA.group_id,PA.class_id,PA.brand_id,PA.product_id,PA.created,PA.modified,PA.active,PA.deliver_type,PA.device,PA.payment_code,PA.fixed_price,PA.left_category_id,PA.right_category_id,PA.category_id,PA.buy_limit,PA.kuser,PA.muser,PA.website,PA.status,PA.url_by,`start` as starts,`end` from promo_additional_price AS PA ");
                str.AppendFormat("SELECT CONCAT(PA.event_type ,right(CONCAT('00000000',PA.id),6)) as 'event_id',PA.id,PA.event_name,PA.event_desc,PA.event_type,PA.condition_id,PA.group_id,PA.class_id,PA.brand_id,PA.product_id,PA.created,PA.modified,PA.active,PA.deliver_type,PA.device,PA.payment_code,PA.fixed_price,PA.left_category_id,PA.right_category_id,PA.category_id,PA.buy_limit,PA.kuser,PA.muser,PA.website,PA.status,PA.url_by,`start` as starts,`end` from promo_additional_price AS PA ");

                //str.AppendFormat(" left join vip_user_group as VUG on PA.group_id=VUG.group_id ");
                //str.AppendFormat(" LEFT JOIN product_category as PC ON PA.category_id=PC.category_id ");
                //str.AppendFormat(" LEFT JOIN t_parametersrc as TP on PA.deliver_type = TP.parameterCode AND TP.parameterType='product_freight'");
                //str.AppendFormat(" left join t_parametersrc as TP1 on PA.device = TP1.parameterCode AND TP1.parameterType='device'");
                // str.AppendFormat(" left join (select * from  t_parametersrc  where parameterType='event_type' ) ET ON PA.event_type = ET.parameterCode");
                str.AppendFormat(" where 1=1  ");
                if (id != 0)
                {
                    str.AppendFormat(" and PA.id='{0}';", id);
                }
                return _access.getSinggleObj<PromoAdditionalPriceQuery>(str.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoAdditionalPriceDao-->Select-->" + ex.Message + str.ToString(), ex);
            }
        }

        //public PromoAdditionalPriceQuery Select(int id)
        //{
        //    StringBuilder str = new StringBuilder();
        //    try
        //    {
        //        str.AppendFormat("SELECT CONCAT(PA.event_type ,right(CONCAT('00000000',PA.id),6)) as 'event_id',PC.banner_image,PC.category_link_url,VUG.group_name,TP.parameterName as deliver_name,TP1.parameterName as device_name,PA.id,PA.event_name,PA.event_desc,PA.event_type,PA.condition_id,PA.group_id,PA.class_id,PA.brand_id,PA.product_id,PA.created,PA.modified,PA.active,PA.deliver_type,PA.device,PA.payment_code,PA.fixed_price,PA.left_category_id,PA.right_category_id,PA.category_id,PA.buy_limit,PA.kuser,PA.muser,PA.website,PA.status,PA.url_by,`start` as starts,`end` from promo_additional_price AS PA ");
        //        str.AppendFormat(" left join vip_user_group as VUG on PA.group_id=VUG.group_id ");
        //        str.AppendFormat(" LEFT JOIN product_category as PC ON PA.category_id=PC.category_id ");
        //        str.AppendFormat(" LEFT JOIN t_parametersrc as TP on PA.deliver_type = TP.parameterCode AND TP.parameterType='product_freight'");
        //        str.AppendFormat(" left join t_parametersrc as TP1 on PA.device = TP1.parameterCode AND TP1.parameterType='device'");
        //        str.AppendFormat(" left join (select * from  t_parametersrc  where parameterType='event_type' ) ET ON PA.event_type = ET.parameterCode");
        //        str.AppendFormat(" where 1=1  ");
        //        if (id != 0)
        //        {
        //            str.AppendFormat(" and PA.id='{0}';", id);
        //        }
        //        return _access.getSinggleObj<PromoAdditionalPriceQuery>(str.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("PromoAdditionalPriceDao-->Select-->" + ex.Message + str.ToString(), ex);
        //    }
        //}
        #endregion

        #region 不同商品固定價 同品加固定價 不同品加不同價格 獲取promo_additional_price表某行數據 +PromoAdditionalPrice GetModel(int id)
        public PromoAdditionalPrice GetModel(int id)
        {
            StringBuilder sql = new StringBuilder("select id,event_type,category_id,group_id,condition_id,left_category_id,right_category_id");
            try
            {
                sql.Append(" from  promo_additional_price");
                sql.AppendFormat("  where 1=1 and id={0};", id);
                return _access.getSinggleObj<Model.PromoAdditionalPrice>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoAdditionalPriceDao-->GetModel-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 不同商品固定價 同品加固定價 不同品加不同價格 獲取促銷商品 +string CategoryID(PromoAdditionalPrice m)
        public string CategoryID(PromoAdditionalPrice m)
        {//判斷是否選擇了促銷商品
            m.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            DataTable dt, dt_cateOne, dt_cateTwo, dt_cate;
            try
            {
                dt = _access.getDataTable(String.Format("select category_id, left_category_id,right_category_id from promo_additional_price where id={0} ", m.id));

                if (m.event_type != "A1")
                {
                    if (dt.Rows.Count > 0)
                    {
                        dt_cateOne = _access.getDataTable(String.Format("SELECT product_id FROM product_category_set WHERE category_id={0} ", dt.Rows[0][1].ToString()));
                        if (dt_cateOne.Rows.Count > 0)
                        {
                            dt_cateTwo = _access.getDataTable(String.Format("SELECT product_id FROM product_category_set WHERE category_id={0} ", dt.Rows[0][2].ToString()));
                            if (dt_cateTwo.Rows.Count > 0)
                                return "true";
                            else
                                return "two";
                        }
                        else
                            return "one";
                    }
                    else
                        return "false";
                }
                else
                {
                    if (dt.Rows.Count > 0)
                    {
                        dt_cate = _access.getDataTable(String.Format("SELECT product_id FROM product_category_set WHERE category_id={0}  ", dt.Rows[0][0].ToString()));
                        if (dt_cate.Rows.Count > 0)
                        {
                            return "true";
                        }
                        else
                            return "false";
                    }
                    else
                        return "false";
                }


            }
            catch (Exception ex)
            {
                throw new Exception("PromoAdditionalPriceDao-->CategoryID-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 不同商品固定價 同品加固定價 不同品加不同價格 改變狀態 +int ChangeActive(Model.Query.PromoAdditionalPriceQuery m)
        public int ChangeActive(Model.Query.PromoAdditionalPriceQuery m)
        {//是否啟用
            int r = 0;
            StringBuilder sb = new StringBuilder();
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
                sb.Append(_prodPromoDao.DeleteProdProm(m.event_id));
                if (m.active)
                {
                    if (m.event_type != "A1")
                    {
                        // 保存左類別下面的商品
                        DataTable dt_L = _access.getDataTable("SELECT product_id FROM product_category_set WHERE category_id=" + m.left_category_id + "");
                        if (dt_L.Rows.Count > 0)
                        {
                            ProdPromo ppmodel = new ProdPromo();
                            for (int i = 0; i < dt_L.Rows.Count; i++)
                            {
                                ppmodel.product_id = Convert.ToInt32(dt_L.Rows[i]["product_id"].ToString());
                                ppmodel.event_id = m.event_id;
                                if (m.event_type == "A2")
                                {
                                    ppmodel.event_type = "A4";
                                }
                                else if (m.event_type == "A3")
                                {
                                    ppmodel.event_type = "A6";
                                }

                                ppmodel.event_desc = m.event_desc;
                                ppmodel.start = m.starts;
                                ppmodel.end = m.end;
                                ppmodel.page_url = m.category_link_url;
                                if (m.group_id == 0 && m.condition_id == 0)
                                    ppmodel.user_specified = 0;
                                else
                                    ppmodel.user_specified = 1;
                                ppmodel.kuser = m.muser;
                                ppmodel.kdate = m.modified;
                                ppmodel.muser = m.muser;
                                ppmodel.mdate = m.modified;
                                ppmodel.status = m.status;
                                sb.Append(_prodPromoDao.SaveProdProm(ppmodel));
                            }
                        }
                        //保存右類別下面的商品
                        DataTable dt_R = _access.getDataTable("SELECT product_id FROM product_category_set WHERE category_id=" + m.right_category_id + "");
                        if (dt_R.Rows.Count > 0)
                        {
                            ProdPromo ppmodel = new ProdPromo();
                            for (int i = 0; i < dt_R.Rows.Count; i++)
                            {
                                ppmodel.product_id = Convert.ToInt32(dt_R.Rows[i]["product_id"].ToString());
                                ppmodel.event_id = m.event_id;
                                if (m.event_type == "A2")
                                {
                                    ppmodel.event_type = "A5";
                                }
                                else if (m.event_type == "A3")
                                {
                                    ppmodel.event_type = "A7";
                                }

                                ppmodel.event_desc = m.event_desc;
                                ppmodel.start = m.starts;
                                ppmodel.end = m.end;
                                ppmodel.page_url = m.category_link_url;
                                if (m.group_id == 0 && m.condition_id == 0)
                                    ppmodel.user_specified = 0;
                                else
                                    ppmodel.user_specified = 1;
                                ppmodel.kuser = m.muser;
                                ppmodel.kdate = m.modified;
                                ppmodel.muser = m.muser;
                                ppmodel.mdate = m.modified;
                                ppmodel.status = m.status;
                                sb.Append(_prodPromoDao.SaveProdProm(ppmodel));
                            }
                        }
                    }
                    else
                    {
                        DataTable _dt = _access.getDataTable("SELECT product_id FROM product_category_set WHERE category_id=" + m.category_id + "");
                        if (_dt.Rows.Count > 0)
                        {
                            ProdPromo ppmodel = new ProdPromo();
                            for (int i = 0; i < _dt.Rows.Count; i++)
                            {
                                ppmodel.product_id = Convert.ToInt32(_dt.Rows[i]["product_id"].ToString());
                                ppmodel.event_id = m.event_id;
                                ppmodel.event_type = m.event_type;
                                ppmodel.event_desc = m.event_desc;
                                ppmodel.start = m.starts;
                                ppmodel.end = m.end;
                                ppmodel.page_url = m.category_link_url;
                                if (m.group_id == 0 && m.condition_id == 0)
                                    ppmodel.user_specified = 0;
                                else
                                    ppmodel.user_specified = 1;
                                ppmodel.kuser = m.muser;
                                ppmodel.kdate = m.modified;
                                ppmodel.muser = m.muser;
                                ppmodel.mdate = m.modified;
                                ppmodel.status = m.status;
                                sb.Append(_prodPromoDao.SaveProdProm(ppmodel));
                            }
                        }
                    }
                }
                sb.AppendFormat(@"UPDATE promo_additional_price SET condition_id='{1}',group_id='{2}',`start`='{3}',`end`='{4}',modified='{5}',deliver_type='{6}',device='{7}',muser='{8}',website='{9}',event_name='{10}',event_desc='{11}',fixed_price='{12}',buy_limit='{13}',url_by='{14}',active={15} where id={0} ; ", m.id, m.condition_id, m.group_id, CommonFunction.DateTimeToString(m.starts), CommonFunction.DateTimeToString(m.end), CommonFunction.DateTimeToString(m.modified), m.deliver_type, m.device, m.muser, m.website, m.event_name, m.event_desc, m.fixed_price, m.buy_limit, m.url_by, m.active);
                mySqlCmd.CommandText = sb.ToString();
                r += mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PromoAdditionalPriceDao-->ChangeActive-->" + ex.Message + sb.ToString(), ex);
            }
            finally
            {
                mySqlConn.Close();
            }
            return r;
        }
        #endregion

        #region 把低於商品加購價的促銷商品刪除掉
        public int DeletLessThen(PromoAdditionalPriceQuery m, int types)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(" select pm.product_id from product_category_set pcs ");
                if (types == 1)
                {
                    sb.AppendFormat(" inner join promo_additional_price pap on pcs.category_id=pap.category_id ");
                }
                else if (types == 2)
                {
                    sb.AppendFormat(" inner join promo_additional_price pap on pcs.category_id=pap.right_category_id ");
                }
                sb.AppendFormat(" inner join price_master pm on pm.product_id= pcs.product_id ");
                sb.AppendFormat(" where 1=1 and pap.id='{0}' and pm.price<'{1}' and pm.price_master_id in({2}); ", m.id, m.fixed_price, m.price_master_in);
                DataTable dt_R = _access.getDataTable(sb.ToString());
                sb.Clear();
                string product_id_in = "";
                if (dt_R.Rows.Count > 0)
                {
                    for (int i = 0; i < dt_R.Rows.Count; i++)
                    {
                        product_id_in += dt_R.Rows[i]["product_id"].ToString() + ",";
                    }
                    product_id_in = product_id_in.TrimEnd(',');
                    if (types == 1)
                    {
                        sb.AppendFormat("delete from product_category_set where category_id =(select category_id from promo_additional_price where id='{0}') ", m.id);
                    }
                    else if (types == 2)
                    {
                        sb.AppendFormat("delete from product_category_set where category_id =(select right_category_id from promo_additional_price where id='{0}') ", m.id);
                    }
                    sb.AppendFormat(" and product_id in ({0}) ", product_id_in);

                    _access.execCommand(sb.ToString());
                    return 1;
                }
                else
                {
                    return 0;
                }


            }
            catch (Exception ex)
            {
                throw new Exception("PromoAdditionalPriceDao-->DeletLessThen-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
    }
}
