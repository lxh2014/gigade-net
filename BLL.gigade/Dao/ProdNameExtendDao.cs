using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using BLL.gigade.Common;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using MySql.Data.MySqlClient;

namespace BLL.gigade.Dao
{
    public class ProdNameExtendDao : IProdNameExtendImplDao
    {
        private IDBAccess _access;
        private string connStr;
        public ProdNameExtendDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
        }

        /// <summary>
        /// 查詢語句
        /// </summary>
        /// <param name="productId">商品id</param>
        /// <returns>符合條件的集合</returns>
        public List<ProdNameExtendCustom> Query(ProdNameExtendCustom pec,string ids)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(@"SELECT DISTINCT pe.rid, pm.product_id,pm.price_master_id,pm.product_name,pm.site_id,st.site_name,pm.user_level as level_name ,pe.product_prefix,pe.product_suffix,");
                sb.Append(@"pm.user_level,pm.user_id,pe.event_start,pe.event_end,pe.kuser,pe.kdate,pe.flag,pu.`type`,pe.apply_id FROM price_master pm ");
                sb.Append(" LEFT JOIN site st on st.site_id = pm.site_id ");
                sb.Append(" LEFT JOIN product_extend pe ON pm.price_master_id = pe.price_master_id ");
                //sb.Append(" LEFT JOIN (select parametername,parametercode from t_parametersrc where parametertype='userlevel') g on pm.user_level=g.parametercode");
                sb.Append(" LEFT JOIN price_update_apply_history pu ON pu.apply_id = pe.apply_id AND (pu.`type`=2 OR pu.`type`=3)");
                sb.Append(" WHERE 1=1 ");
                if (ids != "")
                {
                    sb.AppendFormat(" AND pm.product_id in ({0})  AND (pm.child_id = 0 OR pm.child_id =pm.product_id)", ids);
                }
                if (pec.Site_Id != 0)
                {
                    sb.AppendFormat(" AND pm.site_id ={0} ", pec.Site_Id);
                }

                //edit by zhuoqin0830w  2015/05/18
                IParametersrcImplDao _parameterDao = new ParametersrcDao(connStr);
                List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("UserLevel");
                List<ProdNameExtendCustom> list = _access.getDataTableForObj<ProdNameExtendCustom>(sb.ToString());
                foreach (ProdNameExtendCustom q in list)
                {
                    var alist = parameterList.Find(m => m.ParameterType == "UserLevel" && m.ParameterCode == q.User_Level.ToString());
                    if (alist != null)
                    {
                        q.Level_Name = alist.parameterName;
                    }
                }

                return list;
                //return _access.getDataTableForObj<ProdNameExtendCustom>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProdNameExtendDao.Query-->" + ex.Message, ex);
            }
        }

        public List<ProdNameExtend> QueryByFlag(int flag)
        {
          return _access.getDataTableForObj<ProdNameExtend>(string.Format("select * from product_extend where flag={0};", flag));
        }

        public int Update(List<ProdNameExtend> prodExts)
        {
            try
            {
                StringBuilder sqlStr = new StringBuilder();
                if (prodExts.Count == 0) return 0;
                foreach (var item in prodExts)
                {
                    sqlStr.AppendFormat("update product_extend set price_master_id={0},product_prefix='{1}',product_suffix='{2}',event_start={3},event_end={4},flag={5},apply_id={6} where rid ={7};",
                        item.Price_Master_Id, item.Product_Prefix, item.Product_Suffix, item.Event_Start, item.Event_End, item.Flag, item.Apply_id,item.Rid);
                }
                return _access.execCommand(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProdNameExtendDao.Update-->" + ex.Message, ex);
            }
        }


        /// <summary>
        /// 更新,保存ProdNameExtend表
        /// </summary>
        /// <param name="pn">需要保存的數據</param>
        /// <returns>保存結果</returns>
        public bool SaveByList(List<ProdNameExtendCustom> listpn)
        {
            try
            {
                if (listpn.Count == 0)
                    return true;
                string ids = "";
                StringBuilder strSql = new StringBuilder("INSERT INTO product_extend(`price_master_id`,`apply_id`,`product_prefix`,`product_suffix`,`event_start`,`event_end`,`kuser`,`kdate`) value");
            foreach (var item in listpn)
            {
                item.Replace4MySQL();
                    ids += item.Rid + ",";
                    strSql.AppendFormat("({0},{1},'{2}','{3}',{4},{5},'{6}',now()),", item.Price_Master_Id, item.Apply_id, item.Product_Prefix, item.Product_Suffix, item.Event_Start, item.Event_End, item.Kuser);
            }
                ids = ids.Length > 0 ? ids.Substring(0, ids.Length - 1) : "0";
                string deleteStr = string.Format("SET sql_safe_updates = 0; DELETE FROM product_extend WHERE  rid in ({0}); SET sql_safe_updates = 1; ", ids);
            strSql.Remove(strSql.Length - 1, 1);
            deleteStr = deleteStr + strSql.ToString();
                return _access.execCommand(deleteStr) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("ProdNameExtendDao.SaveByList-->" + ex.Message, ex);
            }

        }


        /// <summary>
        /// 獲得需要去掉前後綴的商品
        /// </summary>
        /// add by wwei0216w 2014/12/12
        /// <returns></returns>
        public List<ProdNameExtendCustom> GetPastProduct()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                //添加 pe.rid  以便於 後面 可以 根據 pe.rid 修改商品前後綴  edit by zhuoqin0830w  2015/04/29
                sb.AppendFormat(@"SELECT pe.rid,pm.product_name,pe.price_master_id,pe.product_prefix,pe.product_suffix,pe.event_start,pe.event_end,pe.apply_id 
                                      FROM price_master pm 
                                  INNER JOIN product_extend pe ON pm.price_master_id = pe.price_master_id 
                                  WHERE ((UNIX_TIMESTAMP(NOW()) - 86400) > pe.event_end) AND ((LOCATE(CONCAT('{0}',pe.product_prefix,'{1}'),pm.product_name)>0) 
                                        OR (LOCATE(CONCAT('{0}',pe.product_suffix,'{1}'),pm.product_name)>0));", PriceMaster.L_HKH, PriceMaster.R_HKH);
                return _access.getDataTableForObj<ProdNameExtendCustom>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProdNameExtendDao.GetPastProduct-->" + ex.Message, ex);
            }
        }

        public List<ProdNameExtend> QueryStart()
        {
            try
            {
                string strSql= string.Format(@"select pe.rid,pe.price_master_id,pe.product_prefix,pe.product_suffix,pe.event_start,pe.event_end,pe.flag,pe.apply_id from product_extend pe
                where pe.event_start<=UNIX_TIMESTAMP(now()) and apply_id=0;", PriceMaster.L_HKH, PriceMaster.R_HKH);
                return _access.getDataTableForObj<ProdNameExtend>(strSql);
            }
            catch (Exception ex)
            {
                throw new Exception("ProdNameExtendDao.QueryStart-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 刪除的Sql語句
        /// </summary>
        /// add by wwei0216w 2014/12/12 
        /// <returns></returns>
        public int DeleteExtendName(int days)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SET sql_safe_updates = 0;");
                sb.AppendFormat("DELETE FROM product_extend WHERE ((TO_DAYS(NOW())-TO_DAYS(FROM_UNIXTIME(event_end))) > {0});", days);
                sb.Append("SET sql_safe_updates = 1;");
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProdNameExtendDao.DeleteExtendName-->" + ex.Message, ex);
            }
        }

        //add by wwei0216w 2014/12/18
        /// <summary>
        /// 根據條件修改時間
        /// </summary>
        /// <returns>更新時間的sql語句</returns>
        public bool UpdateTime(List<ProdNameExtendCustom> pns)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (var pn in pns)
                {
                    sb.AppendFormat("UPDATE product_extend SET event_end = {0} WHERE rid = {1};", pn.Event_End, pn.Rid);
                }
                return _access.execCommand(sb.ToString()) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("ProdNameExtendDao.SaveByList-->" + ex.Message, ex);
            }
        }

        public List<ProdNameExtendCustom> GetPastProductById(uint product_id)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendFormat(@"SELECT pe.rid,pm.product_name,pe.price_master_id,pe.product_prefix,pe.product_suffix,pe.event_start,pe.event_end,pe.apply_id 
                                      FROM price_master pm 
                                          INNER JOIN product_extend pe ON pm.price_master_id = pe.price_master_id 
                                  WHERE pm.product_id = {0}",product_id);
                return _access.getDataTableForObj<ProdNameExtendCustom>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProdNameExtendDao.GetPastProductById-->" + ex.Message, ex);
            }
        }
    }
}
