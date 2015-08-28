#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：IProdVdReqImplDao.cs
* 摘 要：
* * 供應商上下架審核列表dao
* 当前版本：v1.1
* 作 者： mengjuan0826j
* 完成日期：
*/

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Common;

namespace BLL.gigade.Dao
{
    public class ProdVdReqDao : IProdVdReqImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;

        public ProdVdReqDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }

        public List<ProdVdReqQuery> QueryProdVdReqList(ProdVdReqQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                StringBuilder strCols = new StringBuilder("select pvr.rid,pvr.vendor_id,v.vendor_name_full,v.vendor_name_simple as vendor_name, ");
                strCols.Append("pvr.product_id,p.product_name,p.product_status,psta.parameterName as statusName,p.brand_id,vb.brand_name,");
                strCols.Append("  pvr.req_status,pvr.req_datatime,pvr.`explain`, ");
                strCols.Append("pvr.req_type, pvr.user_id,u.user_name,pvr.reply_note, ");
                strCols.AppendFormat(" pvr.reply_datetime ");
                StringBuilder strTbls = new StringBuilder(" from prod_vd_req pvr  ");
                strTbls.Append(" LEFT JOIN vendor v on v.vendor_id=pvr.vendor_id ");
                strTbls.Append(" LEFT JOIN product p on p.product_id=pvr.product_id ");
                strTbls.Append(" LEFT JOIN vendor_brand vb on vb.brand_id =p.brand_id and pvr.vendor_id=vb.vendor_id ");
                strTbls.Append("  LEFT JOIN (select * from  t_parametersrc  tp where   tp.parameterType  ='product_status') psta on psta.parameterCode= p.product_status ");
                strTbls.Append(" LEFT JOIN users u on u.user_id=pvr.user_id ");
                StringBuilder strCondi = new StringBuilder(" where 1=1 ");
                //if (!string.IsNullOrEmpty(query.vendor_name))
                //{
                //    strCondi.AppendFormat(" and ( v.vendor_name_simple LIKE '%{0}%'", query.vendor_name);
                //    strCondi.AppendFormat(" OR  v.vendor_name_simple LIKE '%{0}%' )", query.vendor_name);
                //}
                if (query.brand_id != 0)
                {
                    strCondi.AppendFormat(" and p.brand_id ={0} ", query.brand_id);
                }
                if (query.rid != 0)
                {
                    strCondi.AppendFormat(" and pvr.rid ={0} ", query.rid);
                }
                if (query.product_id != 0)
                {
                    strCondi.AppendFormat(" and pvr.product_id ={0}", query.product_id);
                }
                if (query.req_status != 0)
                {
                    strCondi.AppendFormat(" and pvr.req_status ={0}", query.req_status);
                }
                if (query.time_start != DateTime.MinValue)
                {
                    strCondi.AppendFormat(" and pvr.req_datatime >='{0}'", CommonFunction.DateTimeToString(query.time_start));
                }
                if (query.time_end != DateTime.MinValue)
                {
                    strCondi.AppendFormat(" and pvr.req_datatime <='{0}'", CommonFunction.DateTimeToString(query.time_end));
                }
                if (query.req_type != 0)
                {
                    strCondi.AppendFormat(" and pvr.req_type ={0}", query.req_type);
                }
                string strCount = "select count(*)  as totalCount " + strTbls.ToString() + strCondi.ToString();
                sql.Append(strCount + ";");
                System.Data.DataTable _dt = _dbAccess.getDataTable(strCount);
                totalCount = 0;
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                }

                strCondi.Append(" order by pvr.req_datatime desc ");
                if (query.IsPage)
                {
                    strCondi.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                string sqlstr = strCols.ToString() + strTbls.ToString() + strCondi.ToString();
                sql.Append(sqlstr + ";");
                return _dbAccess.getDataTableForObj<ProdVdReqQuery>(sqlstr);
            }
            catch (Exception ex)
            {
                throw new Exception("ProdVdReqDao.QueryProdVdReqList-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public int Update(ProdVdReq query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.Append("set sql_safe_updates=0;");
                sql.Append(" update prod_vd_req   set  ");
                sql.AppendFormat(" vendor_id='{0}'", query.vendor_id);
                sql.AppendFormat(" ,req_status='{0}' ", query.req_status);
                sql.AppendFormat(" ,req_datatime='{0} ' ", CommonFunction.DateTimeToString(query.req_datatime));
                sql.AppendFormat(" ,`explain`='{0}' ", query.explain);
                sql.AppendFormat(" ,req_type='{0}' ", query.req_type);
                sql.AppendFormat(" ,reply_note='{0}' ", query.reply_note);
                sql.AppendFormat(" ,reply_datetime='{0}' ", CommonFunction.DateTimeToString(query.reply_datetime));
                sql.AppendFormat(" ,user_id='{0}' ", query.user_id);
                sql.AppendFormat(" where product_id= '{0}'", query.product_id);
                sql.AppendFormat(" and rid= '{0}'", query.rid);

                sql.Append(";set sql_safe_updates=1;");
                return _dbAccess.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProdVdReqDao.Update-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public int Insert(ProdVdReq query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.Append(" Insert into prod_vd_req  (  ");
                sql.Append(" vendor_id,product_id,req_status,req_datatime,`explain`,");
                sql.Append("req_type,user_id,reply_datetime,reply_note ) values ( ");
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',",
                    query.vendor_id, query.product_id, query.req_status, CommonFunction.DateTimeToString(query.req_datatime), query.explain);
                sql.AppendFormat("'{0}','{1}','{2}','{3}' );",
                   query.req_type, query.user_id, CommonFunction.DateTimeToString(query.reply_datetime), query.reply_note);
                return _dbAccess.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProdVdReqDao.Insert-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
