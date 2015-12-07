#region 文件信息
/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：PromoPairDao.cs
 * 摘   要： 
 *      
 * 当前版本：v1.2 
 * 作   者： dongya0410j
 * 完成日期：2014/6/20
 * 修改歷史：
 *      v1.1修改日期：2014/8/15
 *      v1.1修改人員：dongya0410j
 *      v1.1修改内容：代碼合併
 *      v1.2修改日期：2014/09/15
 *      v1.2修改人員：hongfei0416j
 *      v1.2修改内容：設置更新的時候的安全模式
 */
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
using BLL.gigade.Common;
using System.Data;
using MySql.Data.MySqlClient;

namespace BLL.gigade.Dao
{

    public class PromoPairDao : IPromoPairImplDao
    {
        private IDBAccess _access;
        private string connStr;
        ProductCategoryDao _proCateDao = null;
        ProductCategorySetDao _prodCategSet = null;
        ProdPromoDao _prodPromoDao = null;
        PromoAllDao _promoAllDao = null;
        SerialDao _serialDao = null;

        public PromoPairDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
            _proCateDao = new ProductCategoryDao(connectionstring);
            _prodCategSet = new ProductCategorySetDao(connectionstring);
            _prodPromoDao = new ProdPromoDao(connectionstring);
            _promoAllDao = new PromoAllDao(connectionstring);
            _serialDao = new SerialDao(connectionstring);

        }
        #region 紅配綠 查詢數據 +List<PromoPairQuery> QueryAll(PromoPairQuery query, out int totalCount)
        /// <summary>
        /// 查詢數據 
        /// </summary>
        /// <param name="query">查詢參數</param>
        /// <param name="totalCount">查詢出來的數量</param>
        /// <param name="eventtype"></param>
        /// <returns>返回列表所需數據</returns>
        public List<PromoPairQuery> QueryAll(PromoPairQuery query, out int totalCount)
        {//促銷商品類別和銀行沒有加!
            StringBuilder str = new StringBuilder();
            StringBuilder strall = new StringBuilder();
            try
            {//TP.parameterName,TP1.parameterName as 'PN1',ET.parameterName as PTname,
                strall.AppendFormat(@"select DISTINCT PP.id,PP.event_name,PP.active, PP.event_desc,PC.banner_image,PC.category_link_url,
VUG.group_name,CONCAT(PP.event_type ,right(CONCAT('00000000',PP.id),6)) as 'condition_name' ,PP.event_type ,
PT.payment_name,PP.start as starts,PP.end,PP.deliver_type,PP.device,PP.website,PP.cate_red,PP.vendor_coverage,
PP.cate_green,PP.category_id,PP.price,PP.discount,PP.condition_id,PP.muser,mu.user_username from promo_pair as PP ");
                str.AppendFormat(" left join vip_user_group as VUG on PP.group_id=VUG.group_id ");
                str.AppendFormat(" left join product_category as PC on PP.category_id = PC.category_id ");
                //str.AppendFormat(" left join t_parametersrc as TP on PP.deliver_type = TP.parameterCode AND TP.parameterType='product_freight'");
               // str.AppendFormat(" left join t_parametersrc as TP1 on PP.device = TP1.parameterCode AND TP1.parameterType='device'");
               // str.AppendFormat(" left join (select * from  t_parametersrc  where parameterType='event_type' ) ET ON PP.event_type = ET.parameterCode");
                str.AppendFormat(" left join payment_type as PT on PP.payment_code = PT.payment_code");
                str.AppendFormat(" LEFT JOIN manage_user mu ON PP.muser=mu.user_id ");
                str.AppendFormat(" where PP.`status`=1");

                totalCount = 0;
                if (query.expired == 1)//是未過期
                {
                    str.AppendFormat(" and `end` >= '{0}'", CommonFunction.DateTimeToString(DateTime.Now));
                }
                else if (query.expired == 0)
                {
                    str.AppendFormat(" and `end` <= '{0}'", CommonFunction.DateTimeToString(DateTime.Now));
                }
                totalCount = 0;
                str.Append(" order by PP.id DESC");
                if (query.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable("select count(PP.id) as totalcounts from promo_pair as PP " + str.ToString());

                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalcounts"]);
                    }
                    str.AppendFormat(" limit {0},{1} ", query.Start, query.Limit);
                }
                return _access.getDataTableForObj<PromoPairQuery>(strall.ToString() + str.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoPairDao-->QueryAll-->" + ex.Message + strall.ToString() + str.ToString(), ex);
            }
        }
        #endregion
        #region 紅配綠 查詢數據+PromoPairQuery Select(int id)
        public PromoPairQuery Select(int id)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(" select id,event_name,event_desc,event_type,condition_id,group_id,start as starts,end,created,modified,active,deliver_type,device,payment_code,cate_red,cate_green,kuser,muser,website,category_id,price,discount,status from promo_pair where 1=1");
                if (id != 0)
                {
                    sb.AppendFormat(" and id='{0}'", id);
                }
                return _access.getSinggleObj<PromoPairQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoPairDao-->Select-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 紅配綠  保存第一步 +SaveOne
        public int SaveOne(PromoPair model)
        {
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
                StringBuilder sb = new StringBuilder();
                string father_id = _access.getDataTable("SELECT parameterProperty from t_parametersrc where parameterCode='P1';").Rows[0][0].ToString();
                ProductCategory pmodel = new ProductCategory();
                pmodel.category_father_id = Convert.ToUInt32(father_id);
                pmodel.category_name = model.event_name;
                pmodel.category_display = 1;
                pmodel.category_show_mode = 0;
                pmodel.category_createdate = (uint)BLL.gigade.Common.CommonFunction.GetPHPTime();
                pmodel.category_display = 0;
                mySqlCmd.CommandText = _proCateDao.SaveCategory(pmodel);
                //mySqlCmd.CommandText = string.Format("INSERT INTO product_category(category_father_id,category_name,category_display,category_show_mode,category_createdate) values('{0}','{1}','{2}','{3}','{4}'); select @@identity ;", father_id, model.event_name, "1", "0", CommonFunction.GetPHPTime(model.created.ToString()));
                model.category_id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                //修改表serial
                Serial serial = new Serial();
                serial.Serial_id = 12;
                serial.Serial_Value = Convert.ToUInt32(model.category_id);
                mySqlCmd.CommandText = _serialDao.UpdateAutoIncreament(serial);
                id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                #region insert red+green
                pmodel.category_father_id = Convert.ToUInt32(model.category_id);
                pmodel.category_name = "紅";
                mySqlCmd.CommandText = _proCateDao.SaveCategory(pmodel);
                model.cate_red = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                //修改表serial
                Serial serialred = new Serial();
                serialred.Serial_id = 12;
                serialred.Serial_Value = Convert.ToUInt32(model.cate_red);
                mySqlCmd.CommandText = _serialDao.UpdateAutoIncreament(serialred);
                id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                //mySqlCmd.CommandText = string.Format("INSERT INTO product_category(category_father_id,category_name,category_display,category_show_mode,category_createdate,status) values('{0}','{1}','{2}','{3}','{4}',1); select @@identity ;", model.category_id, "紅", "1", "0", CommonFunction.GetPHPTime(model.created.ToString()));
                pmodel.category_name = "綠";
                mySqlCmd.CommandText = _proCateDao.SaveCategory(pmodel);
                model.cate_green = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                //修改表serial
                Serial serialgreen = new Serial();
                serialgreen.Serial_id = 12;
                serialgreen.Serial_Value = Convert.ToUInt32(model.cate_green);
                mySqlCmd.CommandText = _serialDao.UpdateAutoIncreament(serialgreen);
                id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                //mySqlCmd.CommandText = string.Format("INSERT INTO product_category(category_father_id,category_name,category_display,category_show_mode,category_createdate,status) values('{0}','{1}','{2}','{3}','{4}',1); select @@identity ;", model.category_id, "綠", "1", "0", CommonFunction.GetPHPTime(model.created.ToString()));
                #endregion
                sb.AppendFormat("INSERT INTO promo_pair(event_name,event_desc,event_type,created,kuser,category_id,active,cate_red,cate_green,vendor_coverage) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'); select @@identity  ;", model.event_name, model.event_desc, model.event_type, CommonFunction.DateTimeToString(model.created), model.kuser, model.category_id, "0", model.cate_red, model.cate_green,model.vendor_coverage);
                mySqlCmd.CommandText = sb.ToString();
                id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PromoPairDao-->SaveOne-->" + ex.Message, ex);
            }
            finally
            {
                mySqlConn.Close();
            }
            return id;
        }
        #endregion
        #region 紅配綠 保存第二步+SaveTwo
        public int SaveTwo(PromoPair model, PromoPairQuery PPquery)
        {
            int id = 0;
            StringBuilder sb = new StringBuilder();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                    mySqlConn.Open();
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                sb.AppendFormat(@"UPDATE promo_pair SET condition_id='{1}',group_id='{2}',`start`='{3}',`end`='{4}',modified='{5}',deliver_type='{6}',device='{7}',muser='{8}',website='{9}',price='{10}',discount='{11}',vendor_coverage='{12}',status=1 where id={0} ; ", model.id, model.condition_id, model.group_id, CommonFunction.DateTimeToString(model.starts), CommonFunction.DateTimeToString(model.end), CommonFunction.DateTimeToString(model.modified), model.deliver_type, model.device, model.muser, model.website, model.price, model.discount,model.vendor_coverage);
                #region 操作ProductCategory
                ProductCategoryDao _categoryDao = new ProductCategoryDao(connStr);
                ProductCategory pcmodel = _categoryDao.GetModelById(Convert.ToUInt32(model.category_id));
                pcmodel.category_id = Convert.ToUInt32(model.category_id);
                pcmodel.banner_image = PPquery.banner_image;
                pcmodel.category_link_url = PPquery.category_link_url;
                pcmodel.category_display = Convert.ToUInt32(model.status);
                pcmodel.category_ipfrom = PPquery.category_ipfrom;
                sb.Append(_proCateDao.UpdateProdCate(pcmodel));
                #endregion
                #region 操作PromoAll
                PromoAll pamodel = new PromoAll();
                pamodel.event_id = PPquery.event_id;
                pamodel.event_type = model.event_type;
                pamodel.category_id = Convert.ToInt32(model.category_id);
                pamodel.startTime = model.starts;
                pamodel.end = model.end;
                pamodel.status = model.status;
                pamodel.kuser = model.muser;
                pamodel.kdate = DateTime.Now;
                pamodel.muser = model.muser;
                pamodel.mdate = pamodel.kdate;
                sb.Append(_promoAllDao.SavePromAll(pamodel));
                #endregion
                //查詢出商品信息后插入prod_promo表
                //mySqlCmd.CommandText = _prodPromoDao.DeleteProdProm(PPquery.event_id);
                //mySqlCmd.ExecuteScalar();
                #region MyRegion
                
               
                //if (model.active)
                //{
                //    DataTable dt = _access.getDataTable("SELECT cate_red,cate_green FROM promo_pair WHERE id=" + model.id + "");
                //    DataTable dt_red = _access.getDataTable("SELECT product_id FROM product_category_set WHERE category_id=" + dt.Rows[0]["cate_red"] + " ;");
                //    DataTable dt_green = _access.getDataTable("SELECT product_id FROM product_category_set WHERE category_id=" + dt.Rows[0]["cate_green"] + "   ;");
                //    if (dt_red.Rows.Count > 0)
                //    {
                //        for (int i = 0; i < dt_red.Rows.Count; i++)
                //        {
                //            ProdPromo ppmodel = new ProdPromo();
                //            ppmodel.product_id = Convert.ToInt32(dt_red.Rows[i][0].ToString());
                //            ppmodel.event_id = PPquery.event_id;
                //            ppmodel.event_type = model.event_type;
                //            //ppmodel.event_desc = model.event_desc;
                //            ppmodel.start = model.starts;
                //            ppmodel.end = model.end;
                //            ppmodel.page_url = PPquery.category_link_url;
                //            if (model.group_id == 0 && model.condition_id == 0)
                //                ppmodel.user_specified = 0;
                //            else
                //                ppmodel.user_specified = 1;
                //            ppmodel.kuser = model.kuser;
                //            ppmodel.kdate = model.created;
                //            ppmodel.muser = ppmodel.kuser;
                //            ppmodel.mdate = ppmodel.kdate;
                //            ppmodel.status = model.status;
                //            sb.Append(_prodPromoDao.SaveProdProm(ppmodel));
                //        }
                //    }
                //    if (dt_green.Rows.Count > 0)
                //    {
                //        for (int i = 0; i < dt_green.Rows.Count; i++)
                //        {
                //            ProdPromo ppmodel = new ProdPromo();
                //            ppmodel.product_id = Convert.ToInt32(dt_green.Rows[i][0].ToString());
                //            ppmodel.event_id = PPquery.event_id;
                //            ppmodel.event_type = model.event_type;
                //            //ppmodel.event_desc = model.event_desc;
                //            ppmodel.start = model.starts;
                //            ppmodel.end = model.end;
                //            ppmodel.page_url = PPquery.category_link_url;
                //            if (model.group_id == 0 && model.condition_id == 0)
                //                ppmodel.user_specified = 0;
                //            else
                //                ppmodel.user_specified = 1;
                //            ppmodel.kuser = model.kuser;
                //            ppmodel.kdate = model.created;
                //            ppmodel.muser = ppmodel.kuser;
                //            ppmodel.mdate = ppmodel.kdate;
                //            ppmodel.status = model.status;
                //            sb.Append(_prodPromoDao.SaveProdProm(ppmodel));
                //        }
                //    }
                //}
                #endregion
                mySqlCmd.CommandText = sb.ToString();
                id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PromoPairDao-->SaveTwo-->" + ex.Message, ex);
            }
            finally
            {
                mySqlConn.Close();
            }
            return id;
        }
        #endregion
        #region 紅配綠  修改+ Update
        public int Update(PromoPair model, PromoPairQuery PPquery)
        {
            int id = 0;
            StringBuilder sb = new StringBuilder();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                    mySqlConn.Open();
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                sb.AppendFormat(@"update promo_pair set condition_id='{1}',group_id='{2}',`start`='{3}',`end`='{4}',modified='{5}',deliver_type='{6}',device='{7}',muser='{8}',website='{9}',event_name='{10}',event_desc='{11}',price='{12}',discount='{13}',vendor_coverage='{14}',active=0 where id={0} ; ", model.id, model.condition_id, model.group_id, CommonFunction.DateTimeToString(model.starts), CommonFunction.DateTimeToString(model.end), CommonFunction.DateTimeToString(model.modified), model.deliver_type, model.device, model.muser, model.website, model.event_name, model.event_desc, model.price, model.discount,model.vendor_coverage);
                #region 操作修改ProductCategory
                ProductCategoryDao _categoryDao = new ProductCategoryDao(connStr);
                ProductCategory pcmodel = _categoryDao.GetModelById(Convert.ToUInt32(model.category_id));
                pcmodel.category_id = Convert.ToUInt32(model.category_id);
                pcmodel.banner_image = PPquery.banner_image;
                pcmodel.category_link_url = PPquery.category_link_url;
                pcmodel.category_name = model.event_name;
                pcmodel.category_display = Convert.ToUInt32(model.status);
                sb.Append(_proCateDao.UpdateProdCate(pcmodel));
                #endregion
                #region 刪除原來的商品信息再新增
                mySqlCmd.CommandText = _prodPromoDao.DeleteProdProm(PPquery.event_id);
                mySqlCmd.ExecuteScalar();
                ////查詢出商品信息后插入prod_promo表
                //if (model.active)
                //{
                //    DataTable dt = _access.getDataTable("SELECT cate_red,cate_green FROM promo_pair WHERE id=" + model.id + "");
                //    DataTable dt_red = _access.getDataTable("SELECT product_id FROM product_category_set WHERE category_id=" + dt.Rows[0]["cate_red"] + "  ;");
                //    DataTable dt_green = _access.getDataTable("SELECT product_id FROM product_category_set WHERE category_id=" + dt.Rows[0]["cate_green"] + "   ;");
                //    if (dt_red.Rows.Count > 0)
                //    {
                //        for (int i = 0; i < dt_red.Rows.Count; i++)
                //        {
                //            ProdPromo ppmodel = new ProdPromo();
                //            ppmodel.product_id = Convert.ToInt32(dt_red.Rows[i][0].ToString());
                //            ppmodel.event_id = PPquery.event_id;
                //            ppmodel.event_type = model.event_type;
                //            ppmodel.event_desc = model.event_desc;
                //            ppmodel.start = model.starts;
                //            ppmodel.end = model.end;
                //            ppmodel.page_url = PPquery.category_link_url;
                //            if (model.group_id == 0 && model.condition_id == 0)
                //                ppmodel.user_specified = 0;
                //            else
                //                ppmodel.user_specified = 1;
                //            ppmodel.kuser = model.muser;
                //            ppmodel.kdate = model.modified;
                //            ppmodel.muser = model.muser;
                //            ppmodel.mdate = model.modified;
                //            ppmodel.status = model.status;
                //            sb.Append(_prodPromoDao.SaveProdProm(ppmodel));
                //        }
                //    }
                //    if (dt_green.Rows.Count > 0)
                //    {
                //        for (int i = 0; i < dt_green.Rows.Count; i++)
                //        {
                //            ProdPromo ppmodel = new ProdPromo();
                //            ppmodel.product_id = Convert.ToInt32(dt_green.Rows[i][0].ToString());
                //            ppmodel.event_id = PPquery.event_id;
                //            ppmodel.event_type = model.event_type;
                //            ppmodel.event_desc = model.event_desc;
                //            ppmodel.start = model.starts;
                //            ppmodel.end = model.end;
                //            ppmodel.page_url = PPquery.category_link_url;
                //            if (model.group_id == 0 && model.condition_id == 0)
                //                ppmodel.user_specified = 0;
                //            else
                //                ppmodel.user_specified = 1;
                //            ppmodel.kuser = model.muser;
                //            ppmodel.kdate = model.modified;
                //            ppmodel.muser = model.muser;
                //            ppmodel.mdate = model.modified;
                //            ppmodel.status = model.status;
                //            sb.Append(_prodPromoDao.SaveProdProm(ppmodel));
                //        }
                //    }
                //}
                #endregion
                
                //if (PPquery.event_id != "")
                //{
                //    sb.Append(_promoAllDao.DelPromAll(PPquery.event_id));
                //}
                mySqlCmd.CommandText = sb.ToString();
                id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PromoPairDao-->Update-->" + ex.Message, ex);
            }
            finally
            {
                mySqlConn.Close();
            }
            return id;
        }
        #endregion
        #region 紅配綠 修改status +DeletePromoPair
        public string DeletePromoPair(int id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update promo_pair set status=0 where id={0} ;", id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromoPairDao-->DeletePromoPair-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 紅配綠 刪除 +Delete
        public int Delete(Model.PromoPair query)
        {
            int i = 0;
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            string sql = String.Empty;
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
                PromoPair pp = new PromoPair();
                if (query.id != 0)
                {
                    pp = GetModelById(query.id);
                    sqlSW += DeletePromoPair(pp.id);
                    if (pp.category_id != 0)
                    {
                        sqlSW += _proCateDao.Delete(Convert.ToUInt32(pp.category_id));
                        sqlSW += _proCateDao.Delete(Convert.ToUInt32(pp.cate_red));
                        sqlSW += _proCateDao.Delete(Convert.ToUInt32(pp.cate_green));
                        //sqlSW += _prodCategSet.DelProdCateSet(Convert.ToUInt32(pp.cate_red));
                        //sqlSW += _prodCategSet.DelProdCateSet(Convert.ToUInt32(pp.cate_green));
                    }
                }
                mySqlCmd.CommandText = sqlSW.ToString();
                sql = mySqlCmd.CommandText;
                i += mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PromoPair-->Delete-->" + ex.Message + sql, ex);
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
        #region 紅配綠 查詢promo_pair +GetPPModel
        public PromoPair GetPPModel(int id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select id,event_name,event_desc,event_type,condition_id,group_id,created,modified,deliver_type,device,muser,website,price,discount from  promo_pair");
                sql.AppendFormat(" where 1=1 and id={0}", id);
                return _access.getSinggleObj<PromoPair>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoPairDao-->GetPPModel-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        #region 紅配綠 查詢GetPPModel +Select
        public DataTable Select(PromoPair model)
        {//查詢出選擇id的所有主表信息
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@" select id , event_name , event_desc , event_type , condition_id , group_id , start as starts , end 
            , created , modified , active , deliver_type , device , payment_code , cate_red , cate_green , kuser , muser 
            , website, category_id , price , discount , status from promo_pair where 1=1");
                if (model.id != 0)
                {
                    sb.AppendFormat(" and id='{0}'", model.id);
                }
                return _access.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoPairDao-->Select-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 紅配綠 返回promo_pair裡面的category_id+SelCategoryID
        public DataTable SelCategoryID(PromoPair model)
        {//查詢出該id的category_id
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("select category_id from promo_pair where 1=1", model.id);
                if (model.id != 0)
                {
                    sb.AppendFormat(" and id='{0}' ;", model.id);
                }
                return _access.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoPairDao-->SelCategoryID-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 紅配綠 查詢promo_pair裡面的紅和綠是否有商品+CategoryID
        public string CategoryID(PromoPair model)
        {//查詢紅和綠是否有選擇商品（促銷商品維護）
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            DataTable dt, dt_red, dt_green;
            try
            {
                dt = _access.getDataTable(String.Format("select cate_red,cate_green from promo_pair where id={0} ", model.id));
                if (dt.Rows.Count > 0)
                {
                    dt_red = _access.getDataTable(String.Format("SELECT product_id FROM product_category_set WHERE category_id={0} ", dt.Rows[0][0].ToString()));
                    if (dt_red.Rows.Count > 0)
                    {
                        dt_green = _access.getDataTable(String.Format("SELECT product_id FROM product_category_set WHERE category_id={0} ", dt.Rows[0][1].ToString()));
                        if (dt_green.Rows.Count > 0)
                            return "true";
                        else
                            return "green";
                    }
                    else
                        return "red";
                }
                else
                    return "false";
            }
            catch (Exception ex)
            {
                throw new Exception("PromoPairDao-->CategoryID-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 紅配綠 查詢PromoPair裡面某一條數據 +GetModelById
        public PromoPair GetModelById(int id)
        {
            StringBuilder sql = new StringBuilder("select id,category_id,cate_red,cate_green,event_name,event_desc,event_type,condition_id,group_id,created,modified,deliver_type,device,muser,website,price,discount from  promo_pair ");
            try
            {
                sql.AppendFormat(" where 1=1 and id={0} ", id);
                return _access.getSinggleObj<PromoPair>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoPairDao-->GetModelById-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        #region 紅配綠 把product_category_set 裡面的商品添加到ProdPromo +UpdateActive
        public int UpdateActive(PromoPairQuery store)
        {//是否啟用
            int id = 0;
            StringBuilder sb = new StringBuilder();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                    mySqlConn.Open();
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                sb.Append(_prodPromoDao.DeleteProdProm(store.event_id));
                if (store.active)
                {
                    //查詢出商品信息后插入prod_promo表
                    DataTable dt = _access.getDataTable("SELECT cate_red,cate_green FROM promo_pair WHERE id=" + store.id + "");
                    DataTable dt_red = _access.getDataTable("SELECT product_id FROM product_category_set WHERE category_id=" + dt.Rows[0]["cate_red"] + " ;");
                    DataTable dt_green = _access.getDataTable("SELECT product_id FROM product_category_set WHERE category_id=" + dt.Rows[0]["cate_green"] + "   ;");
                    if (dt_red.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt_red.Rows.Count; i++)
                        {
                            ProdPromo ppmodel = new ProdPromo();
                            ppmodel.product_id = Convert.ToInt32(dt_red.Rows[i][0].ToString());
                            ppmodel.event_id = store.event_id;
                            ppmodel.event_type = store.event_type;
                            ppmodel.event_desc = store.event_desc;
                            ppmodel.start = store.starts;
                            ppmodel.end = store.end;
                            ppmodel.page_url = store.category_link_url;
                            if (store.group_id == 0 && store.condition_id == 0)
                                ppmodel.user_specified = 0;
                            else
                                ppmodel.user_specified = 1;
                            ppmodel.kuser = store.muser;
                            ppmodel.kdate = store.modified;
                            ppmodel.muser = store.muser;
                            ppmodel.mdate = store.modified;
                            ppmodel.status = store.status;
                            sb.Append(_prodPromoDao.SaveProdProm(ppmodel));
                        }
                    }
                    if (dt_green.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt_green.Rows.Count; i++)
                        {
                            ProdPromo ppmodel = new ProdPromo();
                            ppmodel.product_id = Convert.ToInt32(dt_green.Rows[i][0].ToString());
                            ppmodel.event_id = store.event_id;
                            ppmodel.event_type = store.event_type;
                            ppmodel.event_desc = store.event_desc;
                            ppmodel.start = store.starts;
                            ppmodel.end = store.end;
                            ppmodel.page_url = store.category_link_url;
                            if (store.group_id == 0 && store.condition_id == 0)
                                ppmodel.user_specified = 0;
                            else
                                ppmodel.user_specified = 1;
                            ppmodel.kuser = store.muser;
                            ppmodel.kdate = store.modified;
                            ppmodel.muser = store.muser;
                            ppmodel.mdate = store.modified;
                            ppmodel.status = store.status;
                            sb.Append(_prodPromoDao.SaveProdProm(ppmodel));
                        }
                    }
                }
                sb.AppendFormat("update promo_pair set active ={0},modified='{1}',muser='{2}' where id={3}", store.active,CommonFunction.DateTimeToString( store.modified),store.muser,store.id);
                mySqlCmd.CommandText = sb.ToString();
                id = Convert.ToInt32(mySqlCmd.ExecuteNonQuery());
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PromoPairDao-->UpdateActive-->" + ex.Message+sb.ToString(), ex);
            }
            finally
            {
                mySqlConn.Close();
            }
            return id;

        }
        #endregion
    }
}
