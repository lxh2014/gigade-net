using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
    public class ArrivalNoticeDao : IArrivalNoticeImplDao
    {
        private IDBAccess _access;

        public ArrivalNoticeDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public List<ArrivalNoticeQuery> ArrivalNoticeList(ArrivalNoticeQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            totalCount = 0;
            try
            {
                sqlCount.Append("select count(an.user_id) as totalCount ");
                sql.Append(" select an.id,an.user_id,u.user_email, u.user_name,an.item_id,pi.item_stock,an.product_id,p.product_name, an.`status`,an.create_time,an.coming_time   ");
                sqlFrom.Append("  from arrival_notice as an  ");
                sqlFrom.Append(" LEFT JOIN users u on an.user_id=u.user_id ");
                sqlFrom.Append("  LEFT JOIN product p on an.product_id=p.product_id  ");
                sqlFrom.Append("  left JOIN product_item pi on an.item_id=pi.item_id  ");
                sqlWhere.Append(" where 1=1   ");
                if (query.condition == 1 && query.searchCon != "")
                {
                    sqlWhere.AppendFormat("  and u.user_name like '%{0}%' ", query.searchCon);
                }
                else if (query.condition == 2 && query.searchCon != "")
                {
                    sqlWhere.AppendFormat("  and p.product_name like '%{0}%' ", query.searchCon);
                }
                if (query.status != -1)
                {
                    sqlWhere.AppendFormat("  and an.`status` ={0} ", query.status);
                }
                sqlWhere.AppendFormat("  and pi.item_stock >{0} ", query.item_stock);
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable(sqlCount.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sqlWhere.AppendFormat("limit {0},{1}", query.Start, query.Limit);
                }
                return _access.getDataTableForObj<ArrivalNoticeQuery>(sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ArrivalNoticeDao-->ArrivalNoticeList-->" + ex.Message + sql.ToString() + sqlWhere.ToString(), ex);
            }
        }


        public string IgnoreNotice(ArrivalNoticeQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("UPDATE  arrival_notice SET status='{0}' ,create_time='{1}' ", query.status, CommonFunction.GetPHPTime());
                if (query.status == 3 && query.coming_time != 0)
                {
                    sql.AppendFormat(",coming_time='{0}'  ", query.coming_time);
                }
                sql.AppendFormat(" where id='{0}';", query.id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ArrivalNoticeDao-->IgnoreNotice-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public List<ArrivalNoticeQuery> GetArrNoticeList(ArrivalNoticeQuery query, out int totalCount)
        {
            StringBuilder str = new StringBuilder();//  
            StringBuilder strcont = new StringBuilder();// 

            totalCount = 0;
            try
            {
                str.AppendFormat("SELECT an.id,p.product_id,p.product_name,an.item_id,CONCAT(p.spec_title_1,' ',ps1.spec_name) as spec_title_1,CONCAT(p.spec_title_2,' ',ps2.spec_name) as spec_title_2 ,v.vendor_id,v.vendor_name_full, COUNT(DISTINCT(an.id)) as 'ri_nums' from arrival_notice an ");
                strcont.AppendFormat(" INNER JOIN product p on p.product_id=an.product_id ");
                strcont.AppendFormat(" INNER JOIN product_item pi on pi.product_id=p.product_id ");
                strcont.AppendFormat(" INNER JOIN vendor_brand vb on vb.brand_id=p.brand_id ");
                strcont.AppendFormat(" INNER JOIN vendor v on v.vendor_id=vb.vendor_id ");
                strcont.AppendFormat(" left JOIN product_spec ps1 on ps1.spec_id=pi.spec_id_1 ");
                strcont.AppendFormat(" left JOIN product_spec ps2 on  ps2.spec_id=pi.spec_id_2 ");
                strcont.AppendFormat(" where 1=1 ");

                if (query.product_id != 0)//商品編號
                {
                    strcont.AppendFormat(" and an.product_id like  '{0}' ", query.product_id);
                }

                if (!string.IsNullOrEmpty(query.vendor_name_full_OR_vendor_id))//供應商名稱或者供應商編號
                {
                    strcont.AppendFormat(" and (v.vendor_name_full LIKE '%{0}%' or v.vendor_id like '{1}') ", query.vendor_name_full_OR_vendor_id, query.vendor_name_full_OR_vendor_id);
                }
                if (!string.IsNullOrEmpty(query.start_time) && !string.IsNullOrEmpty(query.end_time))//報名開始日期 報名結束時間 都不為空的條件下
                {
                    strcont.AppendFormat("  and an.create_time >='{0}' and an.create_time <='{1}'  ", CommonFunction.GetPHPTime(query.start_time), CommonFunction.GetPHPTime(query.end_time));
                }
                strcont.AppendFormat("GROUP BY an.item_id ");
                str.Append(strcont);

                if (query.IsPage)//分頁
                {
                    StringBuilder strpage = new StringBuilder();//  
                    StringBuilder strcontpage = new StringBuilder();
                    strpage.AppendFormat("SELECT count(biao.item_id) as totalCount FROM(select an.item_id from arrival_notice an  ");
                    strpage.Append(strcont);
                    strpage.AppendFormat(")as biao ");
                    DataTable _dt = _access.getDataTable(strpage.Append(strcontpage).ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                        str.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                    }

                }
                return _access.getDataTableForObj<ArrivalNoticeQuery>(str.ToString());  //獲取查詢記錄
            }
            catch (Exception ex)
            {
                throw new Exception("ArrivalNoticeDao-->GetArrNoticeList-->" + ex.Message);
            }
        }
        public List<ArrivalNoticeQuery> ShowArrByUserList(ArrivalNoticeQuery query, out int totalCount)
        {
            StringBuilder str = new StringBuilder();
            StringBuilder strcont = new StringBuilder();
            totalCount = 0;
            try
            {
                str.AppendFormat(" select an.id,an.item_id,u.user_id,u.user_name,mu.user_username,an.source_type,an.create_time,an.send_notice_time,an.status as user_status from arrival_notice an  ");
                strcont.AppendFormat(" LEFT JOIN  users u  on an.user_id=u.user_id ");
                strcont.AppendFormat(" LEFT  JOIN manage_user mu on mu.user_id=an.muser_id ");
                strcont.AppendFormat(" where 1=1 and an.item_id={0} ", query.item_id);
                if (!string.IsNullOrEmpty(query.start_time) && !string.IsNullOrEmpty(query.end_time))//報名開始日期 報名結束時間 都不為空的條件下
                {
                    strcont.AppendFormat("  and an.create_time >='{0}' and an.create_time <='{1}'  ", CommonFunction.GetPHPTime(query.start_time), CommonFunction.GetPHPTime(query.end_time));

                }
                str.Append(strcont);
                return _access.getDataTableForObj<ArrivalNoticeQuery>(str.ToString());  //獲取查詢記錄
            }
            catch (Exception ex)
            {
                throw new Exception("ArrivalNoticeDao-->ShowArrByUserList-->" + ex.Message);
            }
        }
        public int SaveArrivaleNotice(ArrivalNotice query)
        {
            StringBuilder sql = new StringBuilder();
            int result = 1;
            try
            {
                sql.AppendFormat(" select Count(user_id) as Total from arrival_notice where user_id='{0}' and item_id='{1}' and status=0;", query.user_id, query.item_id);
                DataTable _dtUser = _access.getDataTable(sql.ToString());
                sql.Clear();
                if (Convert.ToInt32(_dtUser.Rows[0]["Total"]) > 0)
                {
                    result = 98;//此人員已在未通知列表中
                }
                else
                {
                    sql.AppendFormat("INSERT INTO arrival_notice(user_id,item_id,product_id,status,create_time,source_type,muser_id,send_notice_time)value(");
                    sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}' ", query.user_id, query.item_id, query.product_id, query.status, query.create_time, query.source_type);
                    sql.AppendFormat(",'{0}','{1}'); ", query.muser_id, query.send_notice_time);
                    _access.execCommand(sql.ToString());
                    result = 99;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("ArrivalNoticeDao-->SaveArrivaleNotice-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public int UpArrivaleNoticeStatus(ArrivalNotice query)
        {

            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" select count(id) as Total from arrival_notice where item_id='{0}' and user_id='{1}' and status='0'; ", query.item_id, query.user_id);
                DataTable _td = _access.getDataTable(sql.ToString());
                sql.Clear();
                if (Convert.ToInt32(_td.Rows[0]["Total"]) > 0)
                {
                    sql.AppendFormat("  set sql_safe_updates = 0;UPDATE arrival_notice SET status='2' where item_id='{0}' and user_id='{1}';  set sql_safe_updates = 1; ", query.item_id, query.user_id);
                    return _access.execCommand(sql.ToString());
                }
                else
                {
                    return 100;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ArrivalNoticeDao-->UpArrivaleNoticeStatus-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public List<ArrivalNoticeQuery> GetInventoryQueryList(ArrivalNoticeQuery query, out int totalCount)// by yachao1120j 2015-9-10 商品库存查询
        {
            StringBuilder str = new StringBuilder();
            StringBuilder strcont = new StringBuilder();
            totalCount = 0;

            try
            {
                str.AppendFormat("SELECT an.id,p.product_id,p.product_name,an.item_id,CONCAT(p.spec_title_1,' ',ps1.spec_name) as spec_title_1,CONCAT(p.spec_title_2,' ',ps2.spec_name) as spec_title_2 ,v.vendor_id,v.vendor_name_full, vb.brand_id,vb.brand_name,p.product_status,pi.item_stock,p.ignore_stock from arrival_notice an ");
                strcont.AppendFormat(" INNER JOIN product p on p.product_id=an.product_id ");
                strcont.AppendFormat(" INNER JOIN product_item pi on pi.product_id=p.product_id ");
                strcont.AppendFormat(" INNER JOIN vendor_brand vb on vb.brand_id=p.brand_id ");
                strcont.AppendFormat(" INNER JOIN vendor v on v.vendor_id=vb.vendor_id ");
                strcont.AppendFormat(" left JOIN product_spec ps1 on ps1.spec_id=pi.spec_id_1 ");
                strcont.AppendFormat(" left JOIN product_spec ps2 on  ps2.spec_id=pi.spec_id_2 ");
                strcont.AppendFormat(" where 1=1 ");


                if (!string.IsNullOrEmpty(query.product_id_OR_product_name))//商品名稱或者商品編號或商品細項編號
                {
                    strcont.AppendFormat(" and (p.product_name LIKE '%{0}%' or p.product_id like '{1}' or pi.item_id like '{2}') ", query.product_id_OR_product_name, query.product_id_OR_product_name,query.product_id_OR_product_name);
                }

                if (!string.IsNullOrEmpty(query.vendor_name_full_OR_vendor_id))//供應商名稱或者供應商編號
                {
                    strcont.AppendFormat(" and (v.vendor_name_full LIKE '%{0}%' or v.vendor_id like '{1}') ", query.vendor_name_full_OR_vendor_id, query.vendor_name_full_OR_vendor_id);
                }

                if (!string.IsNullOrEmpty(query.brand_id_OR_brand_name))//品牌名稱或者品牌編號
                {
                    int ID = 0;
                    if (int.TryParse(query.brand_id_OR_brand_name, out ID))
                    {
                        strcont.AppendFormat(" and vb.brand_id = '{0}'", query.brand_id_OR_brand_name);
                    }
                    else
                    {
                        strcont.AppendFormat(" and vb.brand_name LIKE '%{0}%'", query.brand_id_OR_brand_name);
                    }
                }
                if (query.product_status != 10)//商品狀態
                {
                    strcont.AppendFormat("and p.product_status like '{0}'", query.product_status);
                }

                if (query.item_stock_start <= query.item_stock_end)//库存数量开始--库存数量结束   
                {
                    strcont.AppendFormat("  and pi.item_stock >='{0}' and pi.item_stock <='{1}'  ", query.item_stock_start, query.item_stock_end);
                }

                if (1 == 1)//補貨中停止販售
                {
                    strcont.AppendFormat("and p.ignore_stock = '{0}'", query.ignore_stock);
                }
                strcont.AppendFormat("GROUP BY an.item_id ");
                str.Append(strcont);

                if (query.IsPage)
                {
                    StringBuilder strpage = new StringBuilder();//  
                    StringBuilder strcontpage = new StringBuilder();
                    strpage.AppendFormat("SELECT count(biao.item_id) as totalCount FROM(select an.item_id from arrival_notice an  ");
                    strpage.Append(strcont);
                    strpage.AppendFormat(")as biao ");
                    string sql = strpage.ToString();
                    DataTable _dt = _access.getDataTable(sql);
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0][0]);
                        str.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                    }
                }
                return _access.getDataTableForObj<ArrivalNoticeQuery>(str.ToString());// 獲取查詢記錄

            }
            catch (Exception ex)
            {
                throw new Exception("ArrivalNoticeDao-->GetInventoryQueryList-->" + ex.Message);
            }

        }
    }
}
