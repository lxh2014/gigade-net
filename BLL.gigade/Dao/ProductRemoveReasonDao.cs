using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class ProductRemoveReasonDao : IProductRemoveReasonImplDao
    {
        private IDBAccess _access;
        private string connStr;
        public ProductRemoveReasonDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }

        #region 獲取到上架商品庫存<=0,並且排除掉庫存為0可販賣機暫停售賣的商品
        public DataTable GetStockLessThanZero()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"SELECT pi.item_id,pt.product_name,pt.shortage,pt.product_status,pt.ignore_stock,pi.item_stock FROM product pt 
INNER JOIN product_item pi on pi.product_id=pt.product_id 
WHERE pt.product_status=5 and pt.shortage = 0 and pt.ignore_stock =0 and pi.item_stock<=0; "); //獲取到上架商品庫存<=0,並且排除掉庫存為0可販賣機暫停售賣的商品sale_status = 0表示正常贩卖ignore_stock =0库存为0不可贩卖
            try
            {
                return _access.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductRemoveReasonDao.GetStockLessThanZero-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 插入到库存记录临时表中
        public string InsertProductRemoveReason(ProductRemoveReason prr)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"insert into product_remove_reason(item_id,product_num,create_name,create_time)values('{0}','{1}','{2}','{3}');",prr.item_id,prr.product_num,prr.create_name,prr.create_time); //獲取到上架商品庫存<=0,並且排除掉庫存為0可販賣機暫停售賣的商品
            try
            {
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductRemoveReasonDao.InsertProductRemoveReason-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 获取缺货天数等信息
        public DataTable GetStockMsg()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"SELECT prr.item_id,pt.outofstock_days_stopselling,pt.product_id,prr.create_time,FROM_UNIXTIME(prr.create_time) as new_time,pt.product_status,pt.shortage,pt.ignore_stock FROM product_remove_reason prr INNER JOIN product_item pi 
on prr.item_id =pi.item_id 
INNER JOIN product pt on pt.product_id =pi.product_id;"); //获取到信息,判断缺货天数
            try
            {
                return _access.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductRemoveReasonDao.GetStockMsg-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 删除临时表中的数据
        public string DeleteProductRemoveReason(ProductRemoveReason prr)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"set sql_safe_updates = 0;delete from  product_remove_reason where item_id='{0}';set sql_safe_updates = 1;", prr.item_id); //獲取到上架商品庫存<=0,並且排除掉庫存為0可販賣機暫停售賣的商品
            try
            {
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductRemoveReasonDao.InsertProductRemoveReason-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 更新商品状态,其中66为缺货系统下架
        public string UpdateProductStatus(Product pt)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"set sql_safe_updates = 0;update product set product_status='{0}' where product_id='{1}';set sql_safe_updates = 1;", pt.Product_Status,pt.Product_Id); //獲取到上架商品庫存<=0,並且排除掉庫存為0可販賣機暫停售賣的商品
            try
            {
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductRemoveReasonDao.InsertProductRemoveReason-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 商品状态记录表中做记录
        public string InsertIntoProductStatusHistory(ProductStatusHistory psh)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"insert into product_status_history (`product_id`,`user_id`,`create_time`,`type`,`product_status`,`remark`)");
            sb.AppendFormat(" values ({0},{1},{2},{3},{4},'{5}');",psh.product_id,2,Common.CommonFunction.DateTimeToString(DateTime.Now), psh.type,psh.product_status,psh.remark);
            try
            {
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductRemoveReasonDao.InsertIntoProductStatusHistory-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 获取到资料表中的缺货系统下架的信息
        public DataTable GetOutofStockMsg()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"SELECT pi.item_id,pt.product_id,pt.product_name,pt.sale_status,pt.product_status,pt.ignore_stock,pi.item_stock FROM product pt 
INNER JOIN product_item pi on pi.product_id=pt.product_id 
WHERE pt.product_status=7;");//7表示缺货系统下架
            try
            {
                return _access.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductRemoveReasonDao.GetOutofStockMsg-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 获取到临时表中的数据信息
        public DataTable GetProductRemoveReasonList()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"SELECT item_id,product_num,create_name FROM product_remove_reason ;");
            try
            {
                return _access.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductRemoveReasonDao.GetProductRemoveReasonList-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 执行事务
        public int ProductRemoveReasonTransact(string str)
        {
            int result = 0;
            try
            {
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
                    mySqlCmd.CommandText = str.ToString();
                    result = mySqlCmd.ExecuteNonQuery();
                    mySqlCmd.Transaction.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    mySqlCmd.Transaction.Rollback();
                    throw new Exception("ProductRemoveReasonDao.ProductRemoveReasonTransact-->" + ex.Message + str.ToString(), ex);
                }
                finally
                {
                    if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                    {
                        mySqlConn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ProductRemoveReasonDao.ProductRemoveReasonTransact-->" + ex.Message + str.ToString(), ex);
            }
        }
        #endregion

        #region 获取要删除的临时表中的数据
        public DataTable GetDeleteProductRemoveReasonList()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"SELECT prr.item_id,prr.product_num,prr.create_name,pii.item_stock,pt.shortage,pt.product_status,pt.ignore_stock FROM product_remove_reason prr
 INNER JOIN product_item pii on prr.item_id=pii.item_id 
 INNER JOIN product pt on pii.product_id =pt.product_id 
 WHERE pt.ignore_stock=1 or pt.shortage = 1 or pii.item_stock>0;");//item_stock>0表示库存大于0 sale_status=0表示正常销售 ignore_stock=1表示贩停继续销售
            try
            {
                return _access.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductRemoveReasonDao.GetDeleteProductRemoveReasonList-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

    }
}
