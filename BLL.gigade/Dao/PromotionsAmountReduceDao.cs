#region 文件信息
/* 
* Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromotionsAmountReduceDao.cs      
* 摘 要：                                                                               
* 滿額滿件免運中促銷減免
* 当前版本：v1.1                                                                 
* 作 者： shuangshuang0420j                                           
* 完成日期：2014/6/20 
* 修改歷史：                                                                     
*         v1.1修改日期：2014/8/16 
*         v1.1修改人員：shuangshuang0420j     
*         v1.1修改内容：規範代碼結構，完善異常拋出，添加注釋
*/

#endregion


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using System.Data;
using BLL.gigade.Common;
using BLL.gigade.Model;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Dao
{
    public class PromotionsAmountReduceDao : IPromotionsAmountReduceImplDao
    {
        private IDBAccess _access;
        private string connStr;
        public PromotionsAmountReduceDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
        }
        #region 新增語句+string Save(PromotionsAmountReduce model)
        public string Save(PromotionsAmountReduce model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(" insert into promotions_amount_reduce(id,name,delivery_store,group_id,type,amount,quantity,start,end,created,active,status)");
                sql.AppendFormat(" values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}');",
                    model.id, model.name, model.delivery_store, model.group_id, model.type, model.amount,
                    model.quantity, CommonFunction.DateTimeToString(model.start), CommonFunction.DateTimeToString(model.end),
                    CommonFunction.DateTimeToString(model.created), model.active, model.status);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountReduceDao-->Save-->" + ex.Message + sql.ToString(), ex);
            }
        }

        #endregion

        #region 修改語句+string Update(PromotionsAmountReduce model)
        public string Update(PromotionsAmountReduce model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(" update promotions_amount_reduce set ");
                sql.AppendFormat(" name='{0}',delivery_store='{1}',group_id='{2}',type='{3}',amount='{4}',quantity='{5}',",
                    model.name, model.delivery_store, model.group_id, model.type, model.amount, model.quantity);
                sql.AppendFormat(" start='{0}',end='{1}',updatetime='{2}',active='{3}',status='{4}'", CommonFunction.DateTimeToString(model.start),
                    CommonFunction.DateTimeToString(model.end),
                    CommonFunction.DateTimeToString(model.updatetime), model.active, model.status);
                sql.AppendFormat(" where id='{0}';", model.id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountReduceDao-->Update-->" + ex.Message + sql.ToString(), ex);
            }
        }

        #endregion

        #region 刪除語句+string Delete(int id)
        public string Delete(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("update promotions_amount_reduce set status=0 where id='{0}';", id);
            return sql.ToString();

        }
        #endregion

        #region 修改狀態語句+string UpdateActive(int active, int id)
        public string UpdateActive(int active, int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("update promotions_amount_reduce set active='{0}' where id='{1}';", active, id);
            return sql.ToString();
        }
        #endregion
    }
}
