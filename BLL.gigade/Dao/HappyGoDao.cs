using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class HappyGoDao : IHappyGoImplDao
    {
        private IDBAccess _accessMySql;
        private string connString;
        public HappyGoDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connString = connectionString;

        }
        public List<HgDeduct> GetHGDeductList(uint order_id)
        {

            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"select id,token,message,date,time from hg_deduct where order_id={0};", order_id);
                return _accessMySql.getDataTableForObj<HgDeduct>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("HappyGoDao.GetHGDeductList-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public List<HgAccumulate> GetHGAccumulateList(uint order_id)
        {

            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"select id,set_point,point_amount,`status` ,note from hg_accumulate where order_id={0} ;", order_id);
                return _accessMySql.getDataTableForObj<HgAccumulate>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("HappyGoDao.GetHGAccumulateList-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public List<HgAccumulateRefund> GetHGAccumulateRefundList(uint order_id)
        {

            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"select id,refund_point,`status` ,note from hg_accumulate_refund where order_id={0} ;", order_id);
                return _accessMySql.getDataTableForObj<HgAccumulateRefund>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("HappyGoDao.GetHGAccumulateRefundList-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public List<HgDeductRefund> GetHgDeductRefundList(uint order_id)
        {

            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"select id,refund_point,`status` ,note  from hg_deduct_refund where order_id={0} ;", order_id);
                return _accessMySql.getDataTableForObj<HgDeductRefund>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("HappyGoDao.GetHgDeductRefundList-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public List<HgDeductReverse> GetHgDeductReverseList(uint order_id)
        {

            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"select id,message  from hg_deduct_reverse where order_id={0};", order_id);
                return _accessMySql.getDataTableForObj<HgDeductReverse>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("HappyGoDao.GetHgDeductReverseList-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
