using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Temp;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
    public class ProductOrderTempDao:IProductOrderTempImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public ProductOrderTempDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }

        public List<ProductOrderTemp> QuerySingle(ProductOrderTemp pot)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                //LEFT JOIN (SELECT parameterCode,parameterName FROM t_parametersrc WHERE parameterType = 'Combo_Type') AS c ON c.parameterCode = p.combination
                sb.AppendFormat(@"SELECT vb.brand_name,product_name,p.product_id,pi.item_id,p.combination,product_name AS combination_name, o.buy_num AS c_num,
		                                FROM_UNIXTIME(p.product_start) AS product_start ,FROM_UNIXTIME(p.product_end) AS product_end,
	                                    CASE WHEN ignore_stock = 0 then '否' ELSE '是' END AS ignore_stock,
		                                CASE WHEN shortage = 0 then '否' ELSE '是' END AS shortage FROM product p
                                        INNER JOIN vendor_brand vb ON vb.brand_id = p.brand_id
                                        INNER JOIN product_item pi ON pi.product_id = p.product_id
                                        LEFT JOIN (
                                                SELECT pi.item_id,CAST(SUM(od.buy_num) AS SIGNED) AS buy_num FROM product_item pi
                                                	INNER JOIN order_detail od ON pi.item_id = od.item_id
                                                	INNER JOIN order_slave os ON os.slave_id = od.slave_id
                                                	INNER JOIN order_master om ON om.order_id = os.order_id
                                                WHERE od.detail_status= 4 AND order_createdate > UNIX_TIMESTAMP('{0}') AND od.item_mode = 0
                                                GROUP BY pi.item_id
                                            ) o ON o.item_id = pi.item_id
                            WHERE p.product_status = 5 AND p.product_id >10000 AND p.brand_id IN({1}) AND p.combination = 1", pot.create_time.ToString("yyyy-MM-dd HH:MM:ss"), pot.brand_id);

                //edit by zhuoqin0830w  2015/05/18
                IParametersrcImplDao _parameterDao = new ParametersrcDao(connStr);
                List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("Combo_Type");
                List<ProductOrderTemp> list = _dbAccess.getDataTableForObj<ProductOrderTemp>(sb.ToString());
                foreach (ProductOrderTemp q in list)
                {
                    var alist = parameterList.Find(m => m.ParameterType == "Combo_Type" && m.ParameterCode == q.combination.ToString());
                    if (alist != null)
                    {
                        q.combination_name = alist.parameterName;
                    }
                    else { q.combination_name = ""; }
                }
                return list;
                //return _dbAccess.getDataTableForObj<ProductOrderTemp>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductOrderTempDao-->QuerySingle" + ex.Message,ex); ;
            }
        }

        public List<ProductOrderTemp> QueryParent(ProductOrderTemp pot)
        { 
            StringBuilder sb = new StringBuilder();
            try
            {
                //LEFT JOIN (SELECT parameterCode,parameterName FROM t_parametersrc WHERE parameterType = 'Combo_Type') AS c ON c.parameterCode = p.combination
                sb.AppendFormat(@"SELECT vb.brand_name,product_name,p.product_id,pi.item_id,p.combination,product_name AS combination_name,o.buy_num AS c_num,
		                                FROM_UNIXTIME(p.product_start) AS product_start,FROM_UNIXTIME(p.product_end) AS product_end,
	                                    CASE WHEN ignore_stock = 0 then '否' ELSE '是' END AS ignore_stock,
		                                CASE WHEN shortage = 0 then '否' ELSE '是' END AS shortage FROM product p
                                        INNER JOIN vendor_brand vb ON vb.brand_id = p.brand_id
                                        LEFT JOIN product_item pi ON pi.product_id = p.product_id
                                        LEFT JOIN (
                                                SELECT pi.item_id,CAST(SUM(od.buy_num) AS SIGNED) AS buy_num FROM product_item pi
                                                	INNER JOIN order_detail od ON pi.item_id = od.item_id
                                                	INNER JOIN order_slave os ON os.slave_id = od.slave_id
                                                	INNER JOIN order_master om ON om.order_id = os.order_id
                                                WHERE od.detail_status= 4 AND order_createdate > UNIX_TIMESTAMP('2014-10-01 00:10:01') AND od.item_mode = 1
                                                GROUP BY pi.item_id
                                            ) o ON o.item_id = pi.item_id
                            WHERE p.product_status = 5 AND p.product_id >10000 AND p.brand_id IN({1}) AND p.combination <> 1", pot.create_time.ToString("yyyy-MM-dd HH:MM:ss"), pot.brand_id);

                //edit by zhuoqin0830w  2015/05/18
                IParametersrcImplDao _parameterDao = new ParametersrcDao(connStr);
                List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("Combo_Type");
                List<ProductOrderTemp> list = _dbAccess.getDataTableForObj<ProductOrderTemp>(sb.ToString());
                foreach (ProductOrderTemp q in list)
                {
                    var alist = parameterList.Find(m => m.ParameterType == "Combo_Type" && m.ParameterCode == q.combination.ToString());
                    if (alist != null)
                    {
                        q.combination_name = alist.parameterName;
                    }
                    else { q.combination_name = ""; }
                }
                return list;

                //return _dbAccess.getDataTableForObj<ProductOrderTemp>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductOrderTempDao-->QueryParent" + ex.Message, ex);
            }
        }

        public List<ProductOrderTemp> QueryChild(DateTime dt,string parentIds)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                //LEFT JOIN (SELECT parameterCode,parameterName FROM t_parametersrc WHERE parameterType = 'Combo_Type') AS c ON c.parameterCode = p.combination
                sb.AppendFormat(@"SELECT vb.brand_name,product_name,p.product_id,pi.item_id,p.combination,product_name AS combination_name,o.buy_num AS c_num,com.parent_id,
		                                FROM_UNIXTIME(p.product_start) AS product_start ,FROM_UNIXTIME(p.product_end) AS product_end,
	                                    CASE WHEN ignore_stock = 0 then '否' ELSE '是' END AS ignore_stock,
		                                CASE WHEN shortage = 0 then '否' ELSE '是' END AS shortage FROM product p
                                        INNER JOIN vendor_brand vb ON vb.brand_id = p.brand_id
                                        LEFT JOIN product_item pi ON pi.product_id = p.product_id
                                        LEFT JOIN (SELECT child_id, parent_id FROM product_combo WHERE parent_id IN ({0})) com ON com.child_id = p.product_id
                                        LEFT JOIN (
                                            SELECT pi.item_id,CAST(SUM(od.buy_num * parent_num) AS SIGNED) AS buy_num FROM product_item pi
	                                            INNER JOIN order_detail od ON pi.item_id = od.item_id
	                                            INNER JOIN order_slave os ON os.slave_id = od.slave_id
	                                            INNER JOIN order_master om ON om.order_id = os.order_id
                                            WHERE od.detail_status= 4 AND order_createdate > UNIX_TIMESTAMP('{1}') AND od.item_mode = 2
                                            GROUP BY pi.item_id) o ON o.item_id = pi.item_id
                                    WHERE p.combination = 1 AND p.product_id >10000 AND p.brand_id IN(29, 98,41, 6, 53, 90) ", parentIds, dt.ToString("yyyy-MM-dd HH:MM:ss"));
                //edit by zhuoqin0830w  2015/05/18
                IParametersrcImplDao _parameterDao = new ParametersrcDao(connStr);
                List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("Combo_Type");
                List<ProductOrderTemp> list = _dbAccess.getDataTableForObj<ProductOrderTemp>(sb.ToString());
                foreach (ProductOrderTemp q in list)
                {
                    var alist = parameterList.Find(m => m.ParameterType == "Combo_Type" && m.ParameterCode == q.combination.ToString());
                    if (alist != null)
                    {
                        q.combination_name = alist.parameterName;
                    }
                    else { q.combination_name = ""; }
                }
                return list;
                //return _dbAccess.getDataTableForObj<ProductOrderTemp>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductOrderTempDao-->QueryChild" + ex.Message, ex);
            }        
        }
    }
}
