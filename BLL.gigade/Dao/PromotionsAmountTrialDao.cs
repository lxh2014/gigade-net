//#region 文件信息
///* 
//* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
//* All rights reserved. 
//* 
//* 文件名称：PromotionsAmountTrialDao.cs
//* 摘 要：促銷試用
//* 
//* 当前版本：v1.1
//* 作 者： shuangshuang0420j
//* 完成日期：2014/11/18 
//* 修改歷史：
//*         
//*/

//#endregion
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
    public class PromotionsAmountTrialDao : IPromotionsAmountTrialImplDao
    {
        private IDBAccess _access;
        private string connStr;

        public PromotionsAmountTrialDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;

        }

        #region 獲取列表+List<Model.Query.PromotionsAmountTrialQuery> Query(Model.Query.PromotionsAmountTrialQuery query, out int totalCount)
        public List<Model.Query.PromotionsAmountTrialQuery> Query(Model.Query.PromotionsAmountTrialQuery query, out int totalCount)
        {
            StringBuilder condition = new StringBuilder();
            StringBuilder TempCol = new StringBuilder();
            try
            {
                query.Replace4MySQL();


                TempCol.Append("SELECT DISTINCT( pat.id),pat.event_id,pat.event_type,pat.show_number,pat.apply_sum, ");
                TempCol.Append(" pat.active ,pat.product_id,pat.product_name,pat.brand_id,pat.name,");
                TempCol.Append("pat.event_desc,pat.group_id,vug.group_name,pat.event_img_small,pat.market_price,");
                TempCol.Append("pat.condition_id,uc.condition_name,pat.category_id,pc.category_name,");
                TempCol.Append("pat.paper_id,p.paperName paper_name,apply_limit,");
                TempCol.Append("pat.count_by,pat.num_limit,pat.gift_mundane,pat.`repeat`,sale_productid,");

                TempCol.Append("pat.freight_type,pat.brand_id,vb.brand_name,");//pf.parameterName 'freight',
                TempCol.Append("pat.product_img,pat.event_img,pat.url,");//et.parameterName 'eventtype',
                TempCol.Append("pat.start_date,pat.end_date,pat.site,pat.muser,mu.user_username ");//dv.parameterName 'device_name',

                condition.Append(" FROM promotions_amount_trial pat ");
                condition.Append(" LEFT JOIN vip_user_group vug on vug.group_id=pat.group_id ");//鏈接會員信息
                condition.Append(" LEFT JOIN user_condition uc on uc.condition_id=pat.condition_id ");//鏈接會員條件信息
                condition.Append(" LEFT JOIN vendor_brand  vb on vb.brand_id=pat.brand_id  ");//鏈接品牌信息 
                condition.Append(" LEFT JOIN product_category  pc on pc.category_id=pat.category_id  ");//鏈接類別信息
                condition.Append(" LEFT JOIN paper  p on p.paperID=pat.paper_id  ");//問卷調查
                condition.Append(" LEFT JOIN manage_user mu ON pat.muser=mu.user_id ");
                // condition.Append(" LEFT JOIN shop_class  sc on sc.class_id=pat.class_id  ");//鏈接品牌信息 
               // condition.Append(" LEFT JOIN (select * from  t_parametersrc  where parameterType='product_freight') pf on pf.parameterCode=pat.freight_type ");//運送方式
               // condition.Append(" LEFT JOIN (select * from  t_parametersrc  where parameterType='event_type' ) et on et.parameterCode=pat.event_type ");//活動類別
               //condition.Append(" LEFT JOIN (select * from  t_parametersrc  where parameterType='device') dv on dv.parameterCode=pat.device ");//設備
                condition.Append(" WHERE pat.`status`=1");

                if (query.expired == 0)//是未過期
                {
                    condition.AppendFormat(" and pat.end_date >= '{0}'", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else if (query.expired == 1)
                {
                    condition.AppendFormat(" and pat.end_date < '{0}'", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                if (!string.IsNullOrEmpty(query.key))
                {
                    condition.AppendFormat(" and (pat.name like'%{0}%' or pat.event_id like '%{0}%' ) ", query.key);
                }

                condition.AppendFormat(" order by pat.id desc ");
                totalCount = 0;
                if (query.IsPage)
                {

                    System.Data.DataTable _dt = _access.getDataTable("select count(pat.id) as totalCount " + condition.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    condition.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                return _access.getDataTableForObj<PromotionsAmountTrialQuery>(TempCol.ToString() + condition.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountTrialDao-->Query-->" + ex.Message + TempCol.ToString() + condition.ToString(), ex);
            }

        }

        #endregion


        #region 根據id獲取Trial的對象+Model.Query.PromotionsAmountTrialQuery Select(int id)
        public Model.Query.PromotionsAmountTrialQuery Select(int id)
        {

            StringBuilder TempCol = new StringBuilder();
            StringBuilder condition = new StringBuilder();
            try
            {

                TempCol.Append(" select name,site,group_id,condition_id,");
                TempCol.Append("paper_id,apply_limit,");
                TempCol.Append("count_by,num_limit,gift_mundane,`repeat`,sale_productid,");
                TempCol.Append("brand_id,category_id,product_id,product_name,market_price,");
                TempCol.Append("product_img,show_number,apply_sum,device,");
                TempCol.Append("event_type,event_id,event_desc,freight_type,");
                TempCol.Append("url,event_img_small, event_img,active,start_date,");
                TempCol.AppendFormat("end_date,created,kuser ,modified ,muser,`status`"); ;
                condition.AppendFormat(" from  promotions_amount_trial  where id='{0}'", id);


                return _access.getSinggleObj<PromotionsAmountTrialQuery>(TempCol.ToString() + condition.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountTrialDao-->Select-->" + ex.Message + TempCol.ToString() + condition.ToString(), ex);
            }
        }
        #endregion

        #region 新增+int Save(Model.Query.PromotionsAmountTrialQuery query)
        public int Save(Model.Query.PromotionsAmountTrialQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("insert into promotions_amount_trial (name,site,group_id,condition_id,");
                sql.Append("paper_id,apply_limit,");
                sql.Append("count_by,num_limit,gift_mundane,`repeat`,sale_productid,");
                //sql.Append("class_id,brand_id,category_id,product_id,product_name,market_price,");
                sql.Append("brand_id,category_id,product_id,product_name,market_price,");
                sql.Append("product_img,show_number,apply_sum,device,");
                sql.Append("event_type,event_id,event_desc,freight_type,");
                sql.Append("url,event_img_small, event_img,active,start_date,");

                sql.AppendFormat("end_date,created,kuser ,modified ,muser,`status`) values('{0}','{1}','{2}','{3}',", query.name, query.site,  query.group_id, query.condition_id);
                sql.AppendFormat("'{0}','{1}',", query.paper_id, query.apply_limit);//添加審核過程
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',", query.count_by, query.num_limit, query.gift_mundane, query.repeat ? '1' : '0', query.sale_productid);//添加次數限制
                //sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}',", query.class_id, query.brand_id, query.category_id, query.product_id, query.product_name, query.market_price);
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',", query.brand_id, query.category_id, query.product_id, query.product_name, query.market_price);
                sql.AppendFormat("'{0}','{1}','{2}','{3}',", query.product_img, query.show_number, query.apply_sum, query.device);
                sql.AppendFormat("'{0}','{1}','{2}','{3}',", query.event_type, query.event_id, query.event_desc, query.freight_type);
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',", query.url, query.event_img_small, query.event_img, query.active, CommonFunction.DateTimeToString(query.start_date));
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}');select @@identity;", CommonFunction.DateTimeToString(query.end_date), CommonFunction.DateTimeToString(query.created), query.kuser, CommonFunction.DateTimeToString(query.modified), query.muser, query.status);
                DataTable dt = _access.getDataTable(sql.ToString());
                return int.Parse(dt.Rows[0][0].ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountTrialDao-->Save-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 更新 +int Update(Model.Query.PromotionsAmountTrialQuery query)
        public int Update(Model.Query.PromotionsAmountTrialQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update promotions_amount_trial set name='{0}',site='{1}',group_id='{2}',condition_id='{3}',", query.name, query.site, query.group_id, query.condition_id);
                sql.AppendFormat("paper_id='{0}',apply_limit='{1}',", query.paper_id, query.apply_limit);
                sql.AppendFormat("count_by='{0}',num_limit='{1}',gift_mundane='{2}',`repeat`='{3}',sale_productid='{4}',", query.count_by, query.num_limit, query.gift_mundane, query.repeat ? '1' : '0', query.sale_productid);

                //   sql.AppendFormat("class_id='{0}',brand_id='{1}',category_id='{2}',product_id='{3}',product_name='{4}',market_price='{5}',", query.class_id, query.brand_id, query.category_id, query.product_id, query.product_name, query.market_price);
                sql.AppendFormat("brand_id='{0}',category_id='{1}',product_id='{2}',product_name='{3}',market_price='{4}',", query.brand_id, query.category_id, query.product_id, query.product_name, query.market_price);
                sql.AppendFormat("product_img='{0}',show_number='{1}',apply_sum='{2}',device='{3}',", query.product_img, query.show_number, query.apply_sum, query.device);
                sql.AppendFormat("event_type='{0}',event_id='{1}',event_desc='{2}',freight_type='{3}',", query.event_type, query.event_id, query.event_desc, query.freight_type );
                sql.AppendFormat("url='{0}',event_img_small='{1}', event_img='{2}',active='{3}',start_date='{4}',", query.url, query.event_img_small, query.event_img, query.active, CommonFunction.DateTimeToString(query.start_date));
                sql.AppendFormat("end_date='{0}', modified='{1}' ,muser='{2}',`status`='{3}' where id='{4}'", CommonFunction.DateTimeToString(query.end_date), CommonFunction.DateTimeToString(query.modified), query.muser, query.status, query.id);

                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountTrialDao-->Update-->" + ex.Message + sql.ToString(), ex);
            }
        }

        #endregion

        public int Delete(int id, string event_id)
        {
            throw new NotImplementedException();
        }


        #region 改變數據的狀態 + int UpdateActive(Model.Query.PromotionsAmountTrialQuery model)
        public int UpdateActive(Model.Query.PromotionsAmountTrialQuery model)
        {

            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update promotions_amount_trial set active='{0}',modified='{1}',muser='{2}' where id='{3}'", model.active,CommonFunction.DateTimeToString(model.modified),model.muser,model.id);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountTrialDao-->UpdateActive-->" + ex.Message + sql.ToString(), ex);
            }

        }
        #endregion

        #region 根據id獲取model 對象+PromotionsAmountTrial GetModel(int id)
        /// <summary>
        /// 根據id獲取model 對象
        /// </summary>
        /// <param name="id"></param>
        /// <returns>PromotionsAmountTrial對象</returns>
        public PromotionsAmountTrial GetModel(int id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select id,event_type,name,event_id,product_img,event_img_small,event_img ");
                sql.Append(" from  promotions_amount_trial");
                sql.AppendFormat("  where 1=1 and id={0};", id);
                return _access.getSinggleObj<PromotionsAmountTrial>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountTrialDao-->GetModel-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
    }
}
