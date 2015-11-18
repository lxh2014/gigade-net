using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Dao
{
    public class RecommendedExcleDao : IRecommendedExcleImplDao
    {
        private IDBAccess _access;
        private string connStr;
        string strSql = string.Empty;
        public RecommendedExcleDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }

        //獲得會員信息
        public DataTable GetVipUserInfo(RecommendedOutPra rop)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT distinct(us.user_id)  as '會員編號',CASE us.user_gender WHEN 0 THEN '女' ELSE '男' END as '性別', case vu.v_id WHEN null then '否' ELSE '是' end as 'VIP',
(Year(CURDATE())-us.user_birthday_year)as '年齡',FROM_UNIXTIME(us.user_reg_date)as '註冊時間',
us.buy_amount as '購買總金額',us.buy_times as '購買次數',uos.buy_counts as '購買次數',uos.order_product_subtotals as '購買總金額'
,om.deduct_bonuss as '購物金使用',uos.normal_product_subtotals as '常溫商品總額',uos.low_product_subtotals as '低溫商品總額'
FROM users us LEFT JOIN (SELECT user_id,sum(buy_count)as buy_counts,SUM(order_product_subtotal) as order_product_subtotals
,sum(normal_product_subtotal)as normal_product_subtotals,sum(low_product_subtotal)as low_product_subtotals 
FROM user_orders_subtotal GROUP BY user_id) as uos on us.user_id=uos.user_id 
left join vip_user vu on us.user_id=vu.user_id 
LEFT JOIN (SELECT user_id,sum(deduct_bonus) as deduct_bonuss FROM  order_master GROUP BY user_id) as om on us.user_id = om.user_id  ");
                sqlwhere.Append(" where 1=1 ");
                //如果沒給類型就是導出全部時間的
                if (!string.IsNullOrEmpty(rop.outType))
                {
                    switch (rop.outType)
                    {
                        case "年":
                            sqlwhere.Append("  and year(FROM_UNIXTIME(us.user_reg_date))=" + rop.outTime);
                            break;
                        case "月":
                            sqlwhere.Append("  and year(FROM_UNIXTIME(us.user_reg_date))=" + rop.nowYear);
                            sqlwhere.Append("  and month(FROM_UNIXTIME(us.user_reg_date))=" + rop.outTime);
                            break;
                        case "日":
                            sqlwhere.Append("  and year(FROM_UNIXTIME(us.user_reg_date))=" + rop.nowYear);
                            sqlwhere.Append("  and month(FROM_UNIXTIME(us.user_reg_date))=" + rop.nowMonth);
                            sqlwhere.Append("  and day(FROM_UNIXTIME(us.user_reg_date))=" + rop.outTime);
                            break;
                    }
                }
                sql.Append(sqlwhere);
                DataTable dt = _access.getDataTable(sql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedExcleDao-->GetVipUserInfo" + ex.Message + sql.ToString(), ex);
            }
        }
        //獲得商品信息
        public DataTable GetProductInfo(RecommendedOutPra rop)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            try
            {
                sql.AppendFormat(@"select pro.product_id as '商品編號',pro.brand_id as '品牌編號' ,
pro.product_type as '類別編號',pro.product_name as '名稱',FROM_UNIXTIME(pro.product_start) as '上架時間',
FROM_UNIXTIME(pro.product_end) as  '下架時間',pro.page_content_1 as '描述',
pro.product_image as '商品圖片',pro.mobile_image as '手機圖片',rpa.months AS '推薦月份設定',
pro.page_content_2 as '商品規格',case rpa.months WHEN null then '否' else '是' end as '是否推薦商品',
rpa.expend_day as '預計消耗時間'
from product as pro left join recommended_product_attribute as rpa on pro.product_id=rpa.product_id   ");
                sqlwhere.Append(" where 1=1 ");
                sqlwhere.Append(" and pro.product_id > 10000 and pro.product_status=5");
                //如果沒給類型就是導出全部時間的
                if (!string.IsNullOrEmpty(rop.outType))
                {
                    switch (rop.outType)
                    {
                        case "年":
                            sqlwhere.Append("  and year(FROM_UNIXTIME(pro.product_createdate))=" + rop.outTime);
                            break;
                        case "月":
                            sqlwhere.Append("  and year(FROM_UNIXTIME(pro.product_createdate))=" + rop.nowYear);
                            sqlwhere.Append("  and month(FROM_UNIXTIME(pro.product_createdate))=" + rop.outTime);
                            break;
                        case "日":
                            sqlwhere.Append("  and year(FROM_UNIXTIME(pro.product_createdate))=" + rop.nowYear);
                            sqlwhere.Append("  and month(FROM_UNIXTIME(pro.product_createdate))=" + rop.nowMonth);
                            sqlwhere.Append("  and day(FROM_UNIXTIME(pro.product_createdate))=" + rop.outTime);
                            break;
                    }
                }
                sql.Append(sqlwhere);
                DataTable dt = _access.getDataTable(sql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedExcleDao-->GetProductInfo" + ex.Message + sql.ToString(), ex);
            }
        }
        //獲得訂單信息
        public DataTable GetOrderInfo(RecommendedOutPra rop)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT om.order_id as '訂單編號',us.user_id as '會員編號',order_amount as '訂單金額',
                                                    FROM_UNIXTIME(order_createdate) as '訂單創建時間' 
                                                    FROM order_master om 
                                                    LEFT JOIN users us on om.user_id=us.user_id   ");
                sqlwhere.Append(" where 1=1 ");
                //如果沒給類型就是導出全部時間的
                if (!string.IsNullOrEmpty(rop.outType))
                {
                    switch (rop.outType)
                    {
                        case "年":
                            sqlwhere.Append("  and year(FROM_UNIXTIME(om.order_createdate))=" + rop.outTime);
                            break;
                        case "月":
                            sqlwhere.Append("  and year(FROM_UNIXTIME(om.order_createdate))=" + rop.nowYear);
                            sqlwhere.Append("  and month(FROM_UNIXTIME(om.order_createdate))=" + rop.outTime);
                            break;
                        case "日":
                            sqlwhere.Append("  and year(FROM_UNIXTIME(om.order_createdate))=" + rop.nowYear);
                            sqlwhere.Append("  and month(FROM_UNIXTIME(om.order_createdate))=" + rop.nowMonth);
                            sqlwhere.Append("  and day(FROM_UNIXTIME(om.order_createdate))=" + rop.outTime);
                            break;
                    }
                }
                sql.Append(sqlwhere);
                DataTable dt = _access.getDataTable(sql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedExcleDao-->GetOrderInfo" + ex.Message + sql.ToString(), ex);
            }
        }
        //獲得訂單內容信息
        public DataTable GetOrderDetailInfo(RecommendedOutPra rop)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT om.order_id as '訂單編號',pi.product_id as '商品編號',od.buy_num as '數量',
tp.parameterName FROM order_master om 
INNER JOIN order_slave os on om.order_id =os.order_id 
INNER JOIN order_detail od on os.slave_id=od.slave_id 
INNER JOIN product_item pi on od.item_id=pi.item_id 
INNER JOIN product pt on pi.product_id =pt.product_id 
INNER JOIN t_parametersrc tp on pt.product_freight_set =tp.parameterCode    ");
                sqlwhere.Append(" where 1=1 ");
                sqlwhere.Append(" and  tp.parameterType = 'product_freight'");
                //如果沒給類型就是導出全部時間的
                if (!string.IsNullOrEmpty(rop.outType))
                {
                    switch (rop.outType)
                    {
                        case "年":
                            sqlwhere.Append("  and year(FROM_UNIXTIME(om.order_createdate))=" + rop.outTime);
                            break;
                        case "月":
                            sqlwhere.Append("  and year(FROM_UNIXTIME(om.order_createdate))=" + rop.nowYear);
                            sqlwhere.Append("  and month(FROM_UNIXTIME(om.order_createdate))=" + rop.outTime);
                            break;
                        case "日":
                            sqlwhere.Append("  and year(FROM_UNIXTIME(om.order_createdate))=" + rop.nowYear);
                            sqlwhere.Append("  and month(FROM_UNIXTIME(om.order_createdate))=" + rop.nowMonth);
                            sqlwhere.Append("  and day(FROM_UNIXTIME(om.order_createdate))=" + rop.outTime);
                            break;
                    }
                }
                sql.Append(sqlwhere);
                DataTable dt = _access.getDataTable(sql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedExcleDao-->GetOrderDetailInfo" + ex.Message + sql.ToString(), ex);
            }
        }
        //獲得類別信息
        public DataTable GetCategoryInfo(RecommendedOutPra rop)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            try
            {
                sql.AppendFormat(@"select category_id as '類別編號',category_name as '類別名稱',category_father_id as '父類別編號' 
                                                from product_category ");
                sqlwhere.Append(" where 1=1 ");
                //如果沒給類型就是導出全部時間的
                if (!string.IsNullOrEmpty(rop.outType))
                {
                    switch (rop.outType)
                    {
                        case "年":
                            sqlwhere.Append("  and year(FROM_UNIXTIME(category_createdate))=" + rop.outTime);
                            break;
                        case "月":
                            sqlwhere.Append("  and year(FROM_UNIXTIME(category_createdate))=" + rop.nowYear);
                            sqlwhere.Append("  and month(FROM_UNIXTIME(category_createdate))=" + rop.outTime);
                            break;
                        case "日":
                            sqlwhere.Append("  and year(FROM_UNIXTIME(category_createdate))=" + rop.nowYear);
                            sqlwhere.Append("  and month(FROM_UNIXTIME(category_createdate))=" + rop.nowMonth);
                            sqlwhere.Append("  and day(FROM_UNIXTIME(category_createdate))=" + rop.outTime);
                            break;
                    }
                }
                sql.Append(sqlwhere);
                DataTable dt = _access.getDataTable(sql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedExcleDao-->GetCategoryInfo" + ex.Message + sql.ToString(), ex);
            }
        }
        //獲得品牌信息
        public DataTable GetBrandInfo(RecommendedOutPra rop)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            try
            {
                sql.AppendFormat(@"select case  p.prod_classify WHEN 10 then '食品館' WHEN 20 then '用品館' else '' end  as '館別編號',
p.brand_id as '品牌編號',p.product_name as '商品名稱',
vb.brand_name as '品牌名稱',pcb.category_id as '品牌類別編號'
from product as p left join product_category_brand as pcb
on p.brand_id=pcb.brand_id 
left join vendor_brand as vb
on p.brand_id=vb.brand_id  ");
                sqlwhere.Append(" where 1=1 ");
                sqlwhere.Append(" and  p.prod_classify<>0 group by p.brand_id ");
                //如果沒給類型就是導出全部時間的
                if (!string.IsNullOrEmpty(rop.outType))
                {
                    switch (rop.outType)
                    {
                        case "年":
                            sqlwhere.Append("  and year(FROM_UNIXTIME(p.product_createdate))=" + rop.outTime);
                            break;
                        case "月":
                            sqlwhere.Append("  and year(FROM_UNIXTIME(p.product_createdate))=" + rop.nowYear);
                            sqlwhere.Append("  and month(FROM_UNIXTIME(p.product_createdate))=" + rop.outTime);
                            break;
                        case "日":
                            sqlwhere.Append("  and year(FROM_UNIXTIME(p.product_createdate))=" + rop.nowYear);
                            sqlwhere.Append("  and month(FROM_UNIXTIME(p.product_createdate))=" + rop.nowMonth);
                            sqlwhere.Append("  and day(FROM_UNIXTIME(p.product_createdate))=" + rop.outTime);
                            break;
                    }
                }
                sql.Append(sqlwhere);
                DataTable dt = _access.getDataTable(sql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedExcleDao-->GetBrandInfo" + ex.Message + sql.ToString(), ex);
            }
        }
        //獲得訂單信息最小年份
        public DataTable GetOrderInfoByMinYear()
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            try
            {
                sql.AppendFormat(@" SELECT year(FROM_UNIXTIME(min(order_createdate))) as minyear ,
year(FROM_UNIXTIME(max(order_createdate)))  as maxyear from order_master  ");
                sqlwhere.Append(" where 1=1 ");
                sqlwhere.Append(" and order_createdate!=0 ");
                sql.Append(sqlwhere);
                DataTable dt = _access.getDataTable(sql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedExcleDao-->GetBrandInfo" + ex.Message + sql.ToString(), ex);
            }
        }


        public DataTable GetThisProductInfo(int start_product_id, int end_product_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
               
                if (end_product_id > start_product_id && start_product_id > 10000)
                {
                    sql.AppendFormat(@"SELECT pt.product_id,pcs.category_id,pii.item_id,pt.brand_id,FROM_UNIXTIME(pt.product_start)as crate_time,pt.product_status,pt.product_name,pt.product_alt,pt.page_content_1,
                                pt.product_image,price,event_price,pm.cost,pm.event_cost,FROM_UNIXTIME(pm.event_start)as event_starts,FROM_UNIXTIME(pm.event_end)as event_ends,pt.prod_classify
                                FROM product pt 
                                INNER JOIN product_item pii 
                                on pt.product_id=pii.product_id and pt.combination=1 and pt.product_id >= '{0}' and pt.product_id <= '{1}' 
                                INNER JOIN price_master pm 
                                on pm.product_id=pii.product_id and pm.product_id>='{0}' and pm.product_id <= '{1}'
                                right JOIN product_category_set pcs on pii.item_id=pcs.item_id 
                                where pii.product_id>='{0}'and pii.product_id <='{1}' ", start_product_id,end_product_id);
                }
                else
                {
                    sql.AppendFormat(@"SELECT pt.product_id,pii.item_id,pt.brand_id,FROM_UNIXTIME(pt.product_start)as crate_time,pt.product_status,pt.product_name,pt.product_alt,pt.page_content_1,
                                pt.product_image,price,event_price,pm.cost,pm.event_cost,FROM_UNIXTIME(pm.event_start)as event_starts,FROM_UNIXTIME(pm.event_end)as event_ends,pt.prod_classify
                                FROM product pt 
                                INNER JOIN product_item pii 
                                on pt.product_id=pii.product_id and pt.combination=1 and pt.product_id>=10001
                                INNER JOIN price_master pm 
                                on pm.product_id=pii.product_id and pm.product_id>=10001
                                where pii.product_id>=10001 ");
                }
                    sql.AppendFormat(" ORDER BY pii.product_id ;");
               
                DataTable dt = _access.getDataTable(sql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedExcleDao-->GetThisProductInfo" + ex.Message + sql.ToString(), ex);
            }
        }

        public List<CategoryItem> GetVendorCategoryMsg(CategoryItem pcb, List<CategoryItem> lscm)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT category_id, category_name, depth FROM product_category_brand 
WHERE category_father_id='{0}' GROUP BY category_id, category_name, depth;", pcb.Id);
                DataTable dtVendorMsg = _access.getDataTable(sql.ToString());
                if (dtVendorMsg.Rows.Count > 0)//再次調用自己
                {
                    for (int i = 0; i < dtVendorMsg.Rows.Count; i++)
                    {
                        CategoryItem cm = new CategoryItem();
                        cm.Id = dtVendorMsg.Rows[i]["category_id"].ToString();
                        cm.Name = dtVendorMsg.Rows[i]["category_name"].ToString();
                        cm.Depth = Convert.ToInt32(dtVendorMsg.Rows[i]["depth"]) - 1;
                        lscm.Add(cm);
                        GetVendorCategoryMsg(cm, lscm);
                    }
                }
                else
                {
                    DataTable _dttwo = GetVendorBrandMsg(pcb);
                    for (int j = 0; j < _dttwo.Rows.Count; j++)
                    {
                        CategoryItem cmtwo = new CategoryItem();
                        cmtwo.Id = String.Format("{0}-{1}", new Object[] { pcb.Id, Int32.Parse(_dttwo.Rows[j]["brand_id"].ToString()) });
                        cmtwo.Name = _dttwo.Rows[j]["brand_name"].ToString();
                        cmtwo.Depth = pcb.Depth + 1;
                        lscm.Add(cmtwo);
                    }
                }
                return lscm;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedExcleDao-->GetVendorCategoryMsg" + ex.Message + sql.ToString(), ex);
            }
        }
        public DataTable GetVendorBrandMsg(CategoryItem pcb)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT v.brand_id, v.brand_name FROM vendor_brand AS v 
                                   INNER JOIN product_category_brand AS p ON v.brand_id = p.brand_id 
                                   WHERE p.category_id = '{0}' AND p.banner_cate_id = 754;", pcb.Id);
                DataTable dt = _access.getDataTable(sql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedExcleDao-->GetVendorBrandMsg" + ex.Message + sql.ToString(), ex);
            }
        }


        public string GetCidMessage(int porduct_id, int brand_id)
        {
            StringBuilder sql = new StringBuilder();
            string str = string.Empty;
            try
            {
                sql.AppendFormat(@"SELECT pcs.product_id,pcs.category_id,pcb.brand_id FROM product_category_set pcs INNER JOIN product_category_brand pcb on pcs.category_id=pcb.category_id 
WHERE pcs.product_id='{0}' and pcb.banner_cate_id=754 and pcs.brand_id='{1}';", porduct_id, brand_id);
                DataTable _dt = _access.getDataTable(sql.ToString());
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    if (str.Length > 0)
                    {
                        str = str+","+_dt.Rows[i]["category_id"] + "-" + _dt.Rows[i]["brand_id"];
                    }
                    else
                    {
                        str = _dt.Rows[i]["category_id"] + "-" + _dt.Rows[i]["brand_id"];
                    }
                }
                return str;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedExcleDao-->GetCidMessage" + ex.Message + sql.ToString(), ex);
            }
        }


        public DataTable GetAllBrandByProductId()
        {
            
         StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT pcs.product_id,pcs.category_id,pcb.brand_id FROM product_category_set pcs 
INNER JOIN product_category_brand pcb on pcs.category_id=pcb.category_id 
WHERE pcb.banner_cate_id=754 ORDER BY pcs.product_id;");
                DataTable dt = _access.getDataTable(sql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedExcleDao-->GetAllBrandByProductId" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
