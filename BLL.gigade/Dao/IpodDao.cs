using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
    public class IpodDao : IIpodImplDao
    {
         private IDBAccess _access;
        string strSql = string.Empty;
        public IpodDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        /// <summary>
        /// 採購單單身列表頁
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalcount"></param>
        /// <returns></returns>
        public List<IpodQuery> GetIpodList(IpodQuery query, out int totalcount)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sql.Append(@" select i.row_id,mu.user_username, para.parameterName,i.po_id,i.pod_id,i.plst_id as ParameterCode,i.bkord_allow,i.cde_dt_incr,i.cde_dt_var,i.cde_dt_shp,i.pwy_dte_ctl,i.qty_ord,i.qty_damaged,i.qty_claimed,i.promo_invs_flg,i.req_cost, ");
                sql.Append(" i.off_invoice,i.new_cost,i.freight_price,i.prod_id,i.create_user,i.create_dtim ");
                sqlCondi.Append(" from ipod i ");
               
                sqlCondi.Append(" left join (select parameterCode,parameterName from t_parametersrc where parameterType='plst_id') para on para.parameterCode=i.plst_id ");
                sqlCondi.Append("LEFT JOIN manage_user mu on mu.user_id=i.create_user "); 
                sqlCondi.Append(" where 1=1 ");
                if (!string.IsNullOrEmpty(query.po_id))
                {
                    sqlCondi.AppendFormat(" and i.po_id='{0}' ", query.po_id);
                }
                totalcount = 0;
                sqlCondi.Append(" order by i.row_id desc ");
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable("select count(i.po_id) as totalCount " + sqlCondi.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalcount = int.Parse(_dt.Rows[0]["totalCount"].ToString());
                    }
                    sqlCondi.AppendFormat(" limit {0},{1} ", query.Start, query.Limit);
                }

                sql.Append(sqlCondi.ToString());

                return _access.getDataTableForObj<IpodQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IpodDao.GetIpodList-->" + ex.Message + sql.ToString(), ex);
            }

        }

        public List<IpodQuery> GetIpodListExprot(IpodQuery query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sql.Append(@" select ipl.loc_id,p.product_id,p.product_name,dfsm.delivery_freight_set as product_freight_set,pi.item_stock,pi.erp_id,p.product_name,p.spec_title_1,p.spec_title_2,pi.spec_id_1,pi.spec_id_2,i.row_id,i.po_id,i.pod_id,i.plst_id,i.bkord_allow,i.cde_dt_incr,i.cde_dt_var,i.cde_dt_shp,i.pwy_dte_ctl,i.qty_ord,i.qty_damaged,i.qty_claimed,i.promo_invs_flg,i.req_cost, ");
                sql.Append(" i.off_invoice,i.new_cost,i.freight_price,i.prod_id,i.create_user,i.create_dtim,mu.user_username,i.change_user,i.change_dtim ");
                sqlCondi.Append(" from ipod i ");
                sqlCondi.Append(" left join product_item pi on pi.item_id=i.prod_id ");
                sqlCondi.Append(" inner join product p on pi.product_id=p.product_id ");
                sqlCondi.Append(" left join delivery_freight_set_mapping dfsm on dfsm.product_freight_set=p.product_freight_set ");
                sqlCondi.Append(" left join iplas ipl on ipl.item_id=i.prod_id ");
                sqlCondi.Append("LEFT JOIN manage_user mu on mu.user_id=i.change_user "); 
                sqlCondi.Append(" where 1=1 ");
                if (!string.IsNullOrEmpty(query.po_id))
                {
                    sqlCondi.AppendFormat(" and i.po_id='{0}' ", query.po_id);
                } 

                sqlCondi.Append(" order by ipl.loc_id desc; ");
                

                sql.Append(sqlCondi.ToString());

                return _access.getDataTableForObj<IpodQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IpodDao.GetIpodListExprot-->" + ex.Message + sql.ToString(), ex);
            }

        }
        /// <summary>
        /// 新增標頭
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int AddIpod(IpodQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                //,,bkord_allow,cde_dt_incr,cde_dt_var,cde_dt_shp,pwy_dte_ctl,qty_ord,qty_damaged,qty_claimed,promo_invs_flg,req_cost,off_invoice,new_cost,freight_price,prod_id,create_user,create_dtim,change_user,change_dtim
                sb.Append(" insert into ipod(po_id,pod_id,plst_id,bkord_allow,cde_dt_incr,cde_dt_var,cde_dt_shp,pwy_dte_ctl,qty_ord,qty_damaged,qty_claimed,promo_invs_flg,req_cost,off_invoice");
                sb.Append(" ,new_cost,freight_price,prod_id,create_user,change_user,create_dtim,change_dtim)  ");
                sb.AppendFormat(@" values('{0}','{1}','{2}','{3}','{4}','{5}'", query.po_id, query.pod_id, query.plst_id, query.bkord_allow, query.cde_dt_incr, query.cde_dt_var);
                sb.AppendFormat(@",'{0}','{1}','{2}','{3}','{4}','{5}'", query.cde_dt_shp, query.pwy_dte_ctl, query.qty_ord, query.qty_damaged, query.qty_claimed,query.promo_invs_flg);
                sb.AppendFormat(@",'{0}','{1}','{2}','{3}','{4}','{5}','{6}'", query.req_cost, query.off_invoice, query.new_cost, query.freight_price, query.prod_id, query.create_user, query.change_user);
                sb.AppendFormat(@",'{0}','{1}');",Common.CommonFunction.DateTimeToString(query.create_dtim),Common.CommonFunction.DateTimeToString(query.change_dtim));
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IpodDao.AddIpod-->" + ex.Message + sb.ToString(), ex);
            }
        }
        /// <summary>
        /// 編輯標頭
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int UpdateIpod(IpodQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("set sql_safe_updates=0;");
                sb.AppendFormat(" update ipod set po_id='{0}',pod_id='{1}',plst_id='{2}',bkord_allow='{3}',cde_dt_incr='{4}' ", query.po_id, query.pod_id, query.plst_id, query.bkord_allow, query.cde_dt_incr);
                sb.AppendFormat(" ,cde_dt_var='{0}',cde_dt_shp='{1}',pwy_dte_ctl='{2}',qty_ord='{3}',qty_damaged='{4}',qty_claimed='{5}' ", query.cde_dt_var, query.cde_dt_shp, query.pwy_dte_ctl, query.qty_ord, query.qty_damaged, query.qty_claimed);
                sb.AppendFormat(" ,promo_invs_flg='{0}',req_cost='{1}',off_invoice='{2}',new_cost='{3}',freight_price='{4}',prod_id='{5}' ", query.promo_invs_flg, query.req_cost, query.off_invoice, query.new_cost, query.freight_price, query.prod_id);
                sb.AppendFormat(" ,change_user='{0}',change_dtim='{1}' where row_id='{2}'; ", query.change_user, Common.CommonFunction.DateTimeToString(query.change_dtim), query.row_id);
                sb.AppendFormat("set sql_safe_updates=0;");
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IpodDao.UpdateIpod-->" + ex.Message + sb.ToString(), ex);
            }
        }
        /// <summary>
        /// 採購單驗收
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string UpdateIpodCheck(IpodQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                //sb.AppendFormat("set sql_safe_updates=0;");
                sb.AppendFormat(" update ipod set qty_damaged='{0}',qty_claimed='{1}',plst_id='{2}' ", query.qty_damaged, query.qty_claimed, query.plst_id);
                sb.AppendFormat(" ,change_user='{0}',change_dtim='{1}' where row_id='{2}'; ", query.change_user, Common.CommonFunction.DateTimeToString(query.change_dtim), query.row_id);
                //sb.AppendFormat("set sql_safe_updates=0;");
                //int result = _access.execCommand(sb.ToString());

                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("IpodDao.UpdateIpodCheck-->" + ex.Message + sb.ToString(), ex);
            }
        }

        public ProductItem GetStockHistorySql(IpodQuery query, out string Stock)
        {
            StringBuilder sqlStock = new StringBuilder();
            Stock = string.Empty;
            try
            {
                sqlStock.AppendFormat("update product_item pi,ipod set pi.item_stock='{0}' where pi.item_id=ipod.prod_id and ipod.row_id='{1}' ;", query.item_stock, query.row_id);
                //_access.execCommand(sqlStock.ToString());
                Stock = sqlStock.ToString();
                sqlStock.Clear();
                sqlStock.AppendFormat(@"SELECT pi.* FROM ipod i LEFT JOIN product_item pi ON pi.item_id = i.prod_id where i.row_id='{0}';", query.row_id);
                
                ProductItem productitem = new ProductItem();
                productitem = _access.getSinggleObj<ProductItem>(sqlStock.ToString());

                return productitem;
            }
            catch (Exception ex)
            {
                throw new Exception("IpodDao-->GetStockHistorySql-->" + ex.Message + sqlStock.ToString(), ex);
            }
        }
        public Product GetIgnoreHistorySql(IpodQuery query, out string Ignore)
        {
            StringBuilder sqlIgnore = new StringBuilder();
            Ignore = string.Empty;
            try
            {
                sqlIgnore.AppendFormat("update product p,product_item pi,ipod set p.ignore_stock=1 where pi.product_id=p.product_id and pi.item_id=ipod.prod_id and p.ignore_stock=0 and ipod.row_id='{0}'  ;", query.row_id);
                //_access.execCommand(sqlShortage.ToString());
                Ignore = sqlIgnore.ToString();
                sqlIgnore.Clear();
                sqlIgnore.AppendFormat(@"SELECT p.* FROM ipod i LEFT JOIN product_item pi ON pi.item_id = i.prod_id inner join product p on pi.product_id=p.product_id  where  p.ignore_stock=0 and i.row_id='{0}';", query.row_id);
                
                Product product = new Product();
                product = _access.getSinggleObj<Product>(sqlIgnore.ToString());

                return product;
            }
            catch (Exception ex)
            {
                throw new Exception("IpodDao-->GetIgnoreHistorySql-->" + ex.Message + sqlIgnore.ToString(), ex);
            }
        }


        public string GetHistorySql(IpodQuery query, out string Shortage)
        {
            StringBuilder sqlStock = new StringBuilder();
            StringBuilder sqlShortage = new StringBuilder();
            Shortage = string.Empty;
            try
            {
                sqlStock.AppendFormat("update product_item pi,ipod set pi.item_stock='{0}' where pi.item_id=ipod.prod_id and ipod.row_id='{1}'  ;", query.item_stock, query.row_id);

                sqlShortage.AppendFormat(@"SELECT p.product_id FROM ipod i LEFT JOIN product_item pi ON pi.item_id = i.prod_id inner join product p on pi.product_id=p.product_id and p.shortage=0 and i.row_id='{0}';", query.row_id);
                DataTable _dt = _access.getDataTable(sqlShortage.ToString());
                if (_dt.Rows.Count > 0)
                {
                    sqlShortage.Clear();
                    sqlShortage.AppendFormat("update product p set p.shortage='{0}' where p.product_id='{1}'  ;", 1, Convert.ToUInt32(_dt.Rows[0]["product_id"].ToString()));
                    Shortage = sqlShortage.ToString();
                    //if (_access.execCommand(sb.ToString()) > 0)
                    //{
                    //    //
                    //    sb.Clear();
                    //    sb.AppendFormat("INSERT into table_change_log(user_type,pk_id,change_table,change_field,field_ch_name,old_value,new_value,create_user,create_time) values('1','{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}');", Convert.ToUInt32(_dt.Rows[0]["product_id"].ToString()), "product", "shortage", "補貨中停止販售", '0', '1', query.change_user, Common.CommonFunction.DateTimeToString(DateTime.Now));
                    //    _access.execCommand(sb.ToString());
                    //}
                }
                return sqlStock.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCommentDao-->GetHistorySql-->" + ex.Message + sqlShortage.ToString(), ex);
            }
        }

        /// <summary>
        /// 採購單單身的序號-->通過採購單單號來算出序號
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int GetPodID(IpodQuery query)
        {
            int podid = 0;
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("select max(pod_id) as number from ipod where po_id='{0}'",query.po_id);
                if (query.row_id != 0)
                {
                    sb.AppendFormat(" and row_id <>'{0}';", query.row_id);
                }
                DataTable dt = _access.getDataTable(sb.ToString());
                if (dt.Rows.Count > 0 &&!string.IsNullOrEmpty(dt.Rows[0]["number"].ToString()))
                {
                    
                    podid = Convert.ToInt32(dt.Rows[0]["number"].ToString());
                }
                podid += 1;
                return podid;
            }
            catch (Exception ex)
            {
                throw new Exception("IpodDao.GetPodID-->" + ex.Message + sb.ToString(), ex);
            }
        }
        /// <summary>
        /// 刪除採購單單身的多個數據
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int DeletIpod(IpodQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(" delete from ipod where row_id in({0});", query.row_ids);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IpodDao.GetPodID-->" + ex.Message + sb.ToString(), ex);
            }
        }
        public bool GetIpodfreight(string po_id, int freight)
       {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            bool result=false;
            try
            {
                sql.Append(@" select dfsm.delivery_freight_set as product_freight_set");
                sqlCondi.Append(" from ipod i ");
                sqlCondi.Append(" left join product_item pi on pi.item_id=i.prod_id ");
                sqlCondi.Append(" inner join product p on pi.product_id=p.product_id ");
                sqlCondi.Append(" left join delivery_freight_set_mapping dfsm on dfsm.product_freight_set=p.product_freight_set ");
                sqlCondi.Append(" left join iplas ipl on ipl.item_id=i.prod_id ");
                sqlCondi.Append(" LEFT JOIN manage_user mu on mu.user_id=i.change_user ");
                sqlCondi.Append(" where 1=1 ");
                if (!string.IsNullOrEmpty(po_id))
                {
                    sqlCondi.AppendFormat(" and i.po_id='{0}' ", po_id);
                }
                sqlCondi.AppendFormat(" and dfsm.delivery_freight_set='{0}' ", freight);
               
                sql.Append(sqlCondi.ToString());
                DataTable _dt = _access.getDataTable(sql.ToString());
                if (_dt.Rows.Count > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
                return result;
            }
            catch (Exception ex)
            {

                throw new Exception("IpodDao-->GetIpodfreight-->" + ex.Message, ex);
            }
       }

    }
}
