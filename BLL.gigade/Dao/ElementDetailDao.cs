/*
* 文件名稱 :ElementDetailDao.cs
* 文件功能描述 :廣告詳情表數據操作 
* 版權宣告 :
* 開發人員 : shuangshuang0420j
* 版本資訊 : 1.0
* 日期 : 2014/10/14
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Common;
using BLL.gigade.Mgr;
using System.Data;

namespace BLL.gigade.Dao
{
    public class ElementDetailDao : IElementDetailImplDao
    {
        private IDBAccess _access;
        private string connStr;
        StringBuilder sql = new StringBuilder();

        private GroupAuthMapMgr _groupAuthMapMgr;

        public ElementDetailDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            _groupAuthMapMgr = new GroupAuthMapMgr(connectionstring);
            this.connStr = connectionstring;
        }
        public List<ElementDetailQuery> QueryAll(ElementDetailQuery query, out int totalCount)
        {
            try
            {
                query.Replace4MySQL();
                StringBuilder TempCol = new StringBuilder("SELECT p.product_id,ap.element_type,ap.packet_status,p.product_name,p.product_status,ap.packet_name, bd.element_id,ap.element_type,");//c.parametername as element_type_name,
                TempCol.Append(" bd.element_content,bd.packet_id,bd.element_name,bd.element_link_url,bd.element_link_mode,bd.element_remark,bd.element_sort,bd.element_status, bd.element_link_mode,");//d.parametername as element_linkmode,
                TempCol.Append("  bd.element_start,bd.element_end,bd.element_createdate,bd.element_updatedate,bd.category_id,bd.category_name ");
                StringBuilder tempCount = new StringBuilder("select count(bd.element_id) as totalCount  ");
                StringBuilder mainSql = new StringBuilder(" FROM element_detail bd ");
                mainSql.Append("  join area_packet ap on ap.packet_id=bd.packet_id");
                mainSql.Append(" LEFT JOIN product p ON p.product_id = bd.element_content   ");
                //   mainSql.Append(" left join (select parametername,parametercode from t_parametersrc where parametertype='element_type') c on ap.element_type=c.parametercode");
                //  mainSql.Append(" left join (select parametername,parametercode from t_parametersrc where parametertype='element_link_mode') d on bd.element_link_mode=d.parametercode");
                StringBuilder condi = new StringBuilder();
                if (query.packet_id != 0)
                {
                    condi.AppendFormat(" and ap.packet_id={0}  ", query.packet_id);
                }
                if (query.element_type != 0)
                {
                    condi.AppendFormat(" and ap.element_type={0}  ", query.element_type);
                }
                if (!string.IsNullOrEmpty(query.key))
                {

                    condi.AppendFormat(" and ( bd.element_name like N'%{0}%'  ", query.key);
                    condi.AppendFormat(" or  bd.element_remark like N'%{0}%'  ", query.key);
                    condi.AppendFormat(" or ap.packet_name like N'%{0}%'  ", query.key);
                    condi.AppendFormat("  or  bd.element_content like N'%{0}%') ", query.key);
                }
                if (!string.IsNullOrEmpty(query.searchcate))
                {
                    condi.AppendFormat(" and  bd.category_name like N'%{0}%'  ", query.searchcate);
                }
                if (query.product_status != 0 && query.element_type == 3)
                {
                    if (query.product_status == 5)
                    {
                        condi.Append(" and p.product_status=5  ");
                    }
                    else
                    {
                        condi.AppendFormat(" and p.product_status <> 5  ");
                    }

                }
                if (condi.Length > 0)
                {
                    mainSql.Append(" where ");
                    mainSql.Append(condi.ToString().TrimStart().Remove(0, 3));
                }
                mainSql.AppendFormat(" order by bd.element_id desc  ");
                totalCount = 0;
                if (query.IsPage)
                {
                    sql.Append(tempCount.ToString() + mainSql.ToString());
                    System.Data.DataTable _dt = _access.getDataTable(sql.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    mainSql.AppendFormat("  limit {0},{1}", query.Start, query.Limit);
                }
                IParametersrcImplDao _paradao = new ParametersrcDao(connStr);
                List<Parametersrc> parameterList = _paradao.QueryParametersrcByTypes("element_type", "element_link_mode");

                List<ElementDetailQuery> list = _access.getDataTableForObj<ElementDetailQuery>(TempCol.ToString() + mainSql.ToString());
                foreach (ElementDetailQuery q in list)
                {
                    var alist = parameterList.Find(m => m.ParameterType == "element_type" && m.ParameterCode == q.element_type.ToString());
                    var blist = parameterList.Find(m => m.ParameterType == "element_link_mode" && m.ParameterCode == q.element_link_mode.ToString());

                    if (alist != null)
                    {
                        q.element_type_name = alist.parameterName;
                    }
                    if (blist != null)
                    {
                        q.element_linkmode = blist.parameterName;
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("ElementDetailDao-->QueryAll-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public int Save(ElementDetail model)
        {
            model.Replace4MySQL();
            StringBuilder sqlStr = new StringBuilder("");
            try
            {
                model.Replace4MySQL();
                sqlStr.Append(@"insert into element_detail(`element_content`,`element_name`,`element_link_url`,`element_link_mode`,`element_sort`,`element_status`,`element_start`,`element_end`,`element_createdate`,`element_updatedate`,`create_userid`,`update_userid`,`element_remark`,`packet_id`,`category_id`,`category_name`) ");
                sqlStr.AppendFormat(" values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}',{13},'{14}','{15}') ",
                    model.element_content, model.element_name, model.element_link_url, model.element_link_mode, model.element_sort, model.element_status,
                    CommonFunction.DateTimeToString(model.element_start), CommonFunction.DateTimeToString(model.element_end), CommonFunction.DateTimeToString(model.element_createdate),
                    CommonFunction.DateTimeToString(model.element_updatedate), model.create_userid, model.update_userid, model.element_remark, model.packet_id, model.category_id, model.category_name);

                return _access.execCommand(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ElementDetailDao-->Save-->" + ex.Message + sqlStr.ToString(), ex);
            }
        }
        public int Update(ElementDetail model)
        {
            model.Replace4MySQL();
            StringBuilder sqlStr = new StringBuilder("");
            try
            {
                model.Replace4MySQL();
                sqlStr.AppendFormat(@"update element_detail set ");
                sqlStr.AppendFormat(" `element_content`='{0}',`element_name`='{1}',`element_link_url`='{2}',`element_link_mode`='{3}',", model.element_content, model.element_name, model.element_link_url, model.element_link_mode);
                sqlStr.AppendFormat(" `element_sort`='{0}',`element_start`='{1}',`element_end`='{2}',`element_updatedate`='{3}',`update_userid`='{4}',`element_remark`='{5}',`packet_id`={6} ,`category_id`='{7}',`category_name`='{8}' ",
                     model.element_sort, CommonFunction.DateTimeToString(model.element_start),
                     CommonFunction.DateTimeToString(model.element_end),
                     CommonFunction.DateTimeToString(model.element_updatedate), model.update_userid, model.element_remark, model.packet_id, model.category_id, model.category_name);
                sqlStr.AppendFormat(" where element_id='{0}'", model.element_id);
                return _access.execCommand(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ElementDetailDao-->Update-->" + ex.Message + sqlStr.ToString(), ex);
            }
        }
        public ElementDetail GetModel(ElementDetail query)
        {
            query.Replace4MySQL();
            StringBuilder sqlStr = new StringBuilder("");
            try
            {
                query.Replace4MySQL();
                sqlStr.Append(@"SELECT element_id,packet_id,category_id,category_name,element_content,element_name,");
                sqlStr.Append(@"element_link_url,element_link_mode,element_sort,element_status,element_start,element_end,");
                sqlStr.Append(@"element_createdate,element_updatedate,create_userid,update_userid,element_remark  ");
                sqlStr.AppendFormat(@"from element_detail where element_id={0};", query.element_id);
                return _access.getSinggleObj<ElementDetail>(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ElementDetailDao-->GetModel-->" + ex.Message + sqlStr.ToString(), ex);
            }

        }
        public int UpdateStatus(ElementDetailQuery model)
        {
            model.Replace4MySQL();
            try
            {
                sql.AppendFormat(@"UPDATE element_detail set element_status='{1}',element_updatedate='{2}' ,update_userid='{3}' where element_id='{0}' ",
                    model.element_id, model.element_status, CommonFunction.DateTimeToString(model.element_updatedate), model.update_userid);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ElementDetailDao-->UpdateStatus-->" + ex.Message + sql.ToString(), ex);
            }
        }

        #region 獲取element下拉框+ List<ElementDetail>QueryElementDetail()
        /// <summary>
        /// chaojie_zz 添加於2014/10/30
        /// </summary>
        /// <returns></returns>
        public List<ElementDetail> QueryElementDetail()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select element_id,element_name from element_detail where element_status=1 ");
                return _access.getDataTableForObj<ElementDetail>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ElementDetailDao-->QueryElementDetail-->" + ex.Message + sql.ToString(), ex);
            }

        }
        #endregion



        public List<ElementDetailQuery> QueryAllWares(ElementDetailQuery query, out int totalCount)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            StringBuilder sqlContent = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            try
            {

                sqlCount.AppendFormat(" select count(ed.element_id) as totalCount ");
                sqlContent.AppendFormat("select p.product_id,   c.parametername as element_type,ed.element_id,ed.element_name,ed.category_id,ed.category_name,ed.element_content,ed.element_link_url,d.parametername as element_link_mode,ed.element_sort,ed.element_status,ed.element_start,ed.element_end,ed.element_remark  ");
                sql.Append(" from element_detail ed ");
                sql.Append(" join area_packet ap on ap.packet_id=ed.packet_id ");
                sql.Append(" LEFT JOIN product p ON p.product_id = ed.element_content ");
                sql.Append(" left join (select parametername,parametercode from t_parametersrc where parametertype='element_type') c on ap.element_type=c.parametercode");
                sql.Append(" left join ( select parametername,parametercode from t_parametersrc where parametertype='element_link_mode') d on ed.element_link_mode=d.parametercode");
                sqlWhere.Append(" where ap.element_type=3 ");
                if (query.packet_id != 0)
                {
                    sqlWhere.AppendFormat(" and packet_id={0}", query.packet_id);
                }
                totalCount = 0;
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable(sqlCount.ToString() + sql.ToString() + sqlWhere.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sqlWhere.AppendFormat(" limit {0},{1} ;", query.Start, query.Limit);
                }
                return _access.getDataTableForObj<ElementDetailQuery>(sqlContent.ToString() + sql.ToString() + sqlWhere.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception("ElementDetailDao-->QueryAllWares-->" + sqlContent.ToString() + sql.ToString() + sqlWhere.ToString(), ex);
            }
        }

        public List<ElementDetailQuery> QueryPacketProd(ElementDetail model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select bd.element_id,bd.element_name,bd.packet_id,bd.element_content,ap.element_type  ");
                sql.Append(" FROM element_detail bd ");
                sql.Append("  join area_packet ap on ap.packet_id=bd.packet_id");
                sql.AppendFormat(" and bd.packet_id='{0}' ", model.packet_id);
                sql.AppendFormat(" and bd.element_content='{0}' ", model.element_content);
                return _access.getDataTableForObj<ElementDetailQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ElementDetailDao-->QueryPacketProd-->" + ex.Message + sql.ToString(), ex);
            }

        }
    }
}
