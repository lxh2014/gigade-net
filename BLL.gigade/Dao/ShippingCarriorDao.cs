using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model.Custom;


namespace BLL.gigade.Dao
{
    public class ShippingCarriorDao : IShippingCarriorImplDao
    {
        private IDBAccess _dbAccess;
        private string strConn;
        public ShippingCarriorDao(string connectStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectStr);
            this.strConn = connectStr;
        }

        /// <summary>
        /// 新增插入信息
        /// </summary>
        /// <param name="sc">一個Shipping_carrior對象</param>
        /// <returns>受影響的行數</returns>
        public int InsertShippingCarrior(ShippingCarrior sc)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO shipping_carrior( ");
            sb.Append(" `delivery_store_id`,`freight_big_area`,`freight_type`,`delivery_freight_set`,");
            sb.Append(" `active`,`charge_type`,`shipping_fee`,`return_fee`,`size_limitation`,");
            sb.Append(" `length`,`width`,`height`,`weight`,`pod`,`note`");
            sb.Append(") values (");
            sb.AppendFormat(" '{0}','{1}','{2}','{3}',", sc.Delivery_store_id, sc.Freight_big_area, sc.Freight_type, sc.Delivery_freight_set);
            sb.AppendFormat(" '{0}','{1}','{2}','{3}','{4}',", sc.Active, sc.Charge_type, sc.Shipping_fee, sc.Return_fee, sc.Size_limitation);
            sb.AppendFormat(" '{0}','{1}','{2}','{3}','{4}','{5}')", sc.Length, sc.Width, sc.Height, sc.Weight, sc.Pod, sc.Note);
            return _dbAccess.execCommand(sb.ToString());
        }

        /// <summary>
        /// 刪除滿足條件的信息
        /// </summary>
        /// <param name="sc">刪除條件</param>
        /// <returns>受影響的行數</returns>
        public int DeleteShippingCarrior(string rids)
        {
            if (!string.IsNullOrEmpty(rids))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("DELETE FROM shipping_carrior WHERE rid in ({0})", rids);
                return _dbAccess.execCommand(sb.ToString());
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 查詢符合條件的ShippintCarrior集合
        /// </summary>
        /// <param name="sc">查詢的條件</param>
        /// <returns>符合條件的集合</returns>
        public List<ShippingCarriorCustom> QueryAll(ShippingCarrior sc, out int totalCount)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"SELECT sc.rid,sc.delivery_store_id,sc.freight_big_area,sc.freight_type,sc.delivery_freight_set,sc.active,sc.charge_type,sc.shipping_fee,sc.return_fee,
                            sc.size_limitation,sc.length,sc.width,sc.height,sc.weight,sc.pod,sc.note,ds.store_name FROM shipping_carrior sc ");
            sb.Append(@"
  LEFT JOIN delivery_store ds ON ds.delivery_store_id = sc.delivery_store_id
");
            sb.AppendFormat(" WHERE 1=1 ");
            if (sc.Rid != 0)
            {
                sb.AppendFormat(" AND sc.Rid ={0}", sc.Rid);
            }
            if (sc.Delivery_store_id != 0)
            {
                sb.AppendFormat(" AND sc.delivery_store_id ={0}", sc.Delivery_store_id);
            }
            if (sc.Freight_big_area != 0)
            {
                sb.AppendFormat(" AND sc.freight_big_area ={0}", sc.Freight_big_area);
            }
            if (sc.Freight_type != 0)
            {
                sb.AppendFormat(" AND sc.freight_type ={0}", sc.Freight_type);
            }
            if (sc.Delivery_freight_set != 0)
            {
                sb.AppendFormat(" AND sc.delivery_freight_set", sc.Delivery_freight_set);
            }
            if (sc.Active != 0)
            {
                sb.AppendFormat(" AND sc.active", sc.Active);
            }

            totalCount = 0;

            System.Data.DataTable _dt = _dbAccess.getDataTable("SELECT COUNT(Rid) AS totalCount from shipping_carrior ");
            if (_dt != null)
            {
                totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
            }

            IParametersrcImplDao _parameterDao = new ParametersrcDao(strConn);
            List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("freight_big_area", "freight_type");
            List<ShippingCarriorCustom> list = _dbAccess.getDataTableForObj<ShippingCarriorCustom>(sb.ToString());
            foreach (ShippingCarriorCustom q in list)
            {
                var alist = parameterList.Find(m => m.ParameterType == "freight_big_area" && m.ParameterCode == q.Freight_big_area.ToString());
                var blist = parameterList.Find(m => m.ParameterType == "freight_type" && m.ParameterCode == q.Freight_type.ToString());
                if (alist != null)
                {
                    q.Area_name = alist.parameterName;
                }
                if (blist != null)
                {
                    q.Freight_type_Name = blist.parameterName;
                }
            }

            return _dbAccess.getDataTableForObj<ShippingCarriorCustom>(sb.ToString());
        }

        /// <summary>
        /// 更新Shiiping_carrior表
        /// </summary>
        /// <param name="sc">需要更新的數據</param>
        /// <returns>受影響的行數</returns>
        public int UpdateShippingCarrior(ShippingCarrior sc)
        {
            StringBuilder sb = new StringBuilder("UPDATE shipping_carrior SET ");

            sb.AppendFormat(" delivery_freight_set = {0}, delivery_store_id = {1}, freight_big_area = {2}, freight_type = {3}, active={4}, charge_type = {5}, shipping_fee = {6}, return_fee = {7}, size_limitation = {8}, length = {9}, width = {10}, height = {11}, weight = {12}, pod = {13}, note = '{14}' WHERE rid = {15} ", sc.Delivery_freight_set, sc.Delivery_store_id, sc.Freight_big_area, sc.Freight_type, sc.Active, sc.Charge_type, sc.Shipping_fee, sc.Return_fee, sc.Size_limitation, sc.Length, sc.Width, sc.Height, sc.Weight, sc.Pod, sc.Note, sc.Rid);
            try
            {
                return _dbAccess.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ShippingCarriorDao-->UpdateShippingCarrior-->" + ex.Message + "sql:" + sb.ToString(), ex);
            }
        }


        #region 獲取編輯頁面的combobox的數據 + System.Data.DataTable GetLogisticsName(Parametersrc pt)
        /// <summary>
        /// 獲取編輯頁面的combobox的數據
        /// </summary>
        /// <param name="pt">搜索條件</param>
        /// <param name="name">搜索條件</param>
        /// <returns></returns>
        public System.Data.DataTable GetLogisticsName(Parametersrc pt, string name)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SELECT tp.parameterCode,tp.parameterName  FROM t_parametersrc tp WHERE tp.parameterType='{0}' ", pt.ParameterType);
                if (!string.IsNullOrEmpty(name))
                {
                    sb.AppendFormat(" and tp.parameterCode like '{0}" + "%" + "'", name);
                }
                return _dbAccess.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ShippingCarriorDao-->GetLogisticsName-->" + ex.Message + "sql:" + sb.ToString(), ex);
            }
        }
        #endregion

        #region 物流信息新增保存 + LogisticsSave(ShippingCarrior sc)
        public int LogisticsSave(ShippingCarrior sc)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"INSERT INTO shipping_carrior (delivery_store_id,freight_big_area,freight_type,delivery_freight_set,active,charge_type,shipping_fee,");
                sb.AppendFormat("return_fee,size_limitation,length,width,height,weight,pod,note) VALUES (");
                sb.AppendFormat("'{0}','{1}','{2}','{3}'", sc.Delivery_store_id, sc.Freight_big_area, sc.Freight_type, sc.Delivery_freight_set);
                sb.AppendFormat(",'{0}','{1}','{2}','{3}'", sc.Active, sc.Charge_type, sc.Shipping_fee, sc.Return_fee);
                sb.AppendFormat(",'{0}','{1}',{2},{3},{4}", sc.Size_limitation, sc.Length, sc.Width, sc.Height, sc.Weight);
                sb.AppendFormat(",'{0}','{1}'", sc.Pod, sc.Note);
                sb.AppendFormat(");");
                return _dbAccess.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ShippingCarriorDao-->LogisticsSave-->" + ex.Message + "sql:" + sb.ToString(), ex);
            }

        }
        #endregion


        #region 物流信息編輯保存 + LogisticsUpdate(ShippingCarrior sc)
        public int LogisticsUpdate(ShippingCarrior sc)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("set sql_safe_updates=0;");
                sql.Append(" update shipping_carrior   set  ");
                sql.AppendFormat("freight_big_area='{0}',", sc.Freight_big_area);
                sql.AppendFormat("freight_type='{0} ' ,", sc.Freight_type);
                sql.AppendFormat(" `delivery_freight_set`='{0}' ,", sc.Delivery_freight_set);
                sql.AppendFormat(" charge_type='{0}', ", sc.Charge_type);
                sql.AppendFormat(" shipping_fee='{0}', ", sc.Shipping_fee);
                sql.AppendFormat(" return_fee='{0}' ,", sc.Return_fee);
                sql.AppendFormat(" size_limitation= '{0}',", sc.Size_limitation);
                sql.AppendFormat(" length= {0},", sc.Length);
                sql.AppendFormat(" width= {0},", sc.Width);
                sql.AppendFormat(" height= {0},", sc.Height);
                sql.AppendFormat(" weight= {0},", sc.Weight);
                sql.AppendFormat("note= '{0}',", sc.Note);
                sql.AppendFormat("pod= '{0}'", sc.Pod);
                sql.AppendFormat(" where rid= '{0}'", sc.Rid);
                sql.Append(";set sql_safe_updates=1;");
                return _dbAccess.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ShippingCarriorDao-->LogisticsUpdate-->" + ex.Message + "sql:" + sql.ToString(), ex);
            }

        }
        #endregion

        #region 獲取ShippingCarrior表的list + GetShippingCarriorList(Model.Query.ShippingCarriorQuery sc, out int totalCount)
        public System.Data.DataTable GetShippingCarriorList(Model.Query.ShippingCarriorQuery sc, out int totalCount)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbW = new StringBuilder();
            try
            {
                sc.Replace4MySQL();//fba.parameterName AS freight_big_area_name,ft.parameterName AS freight_type_name,DSI.parameterName AS delivery_store_name,
                sb.Append(@"SELECT sc.rid,sc.delivery_store_id, sc.freight_big_area,sc.freight_type,");
                sb.Append(@"sc.delivery_freight_set,sc.active,sc.charge_type,sc.shipping_fee,");
                sb.Append(@"sc.return_fee,sc.size_limitation,sc.width AS wid,sc.length AS len,sc.weight AS wei,sc.height AS hei,sc.pod,sc.note ");
                sbW.Append(@" FROM shipping_carrior sc ");
                //sbW.Append(@"  LEFT JOIN (SELECT tpc.parameterName,tpc.parameterCode FROM t_parametersrc tpc WHERE tpc.parameterType='freight_big_area') fba ON fba.parameterCode=sc.freight_big_area ");
                //sbW.Append(@" LEFT JOIN (SELECT tpc1.parameterName,tpc1.parameterCode FROM t_parametersrc tpc1 WHERE tpc1.parameterType='freight_type') ft ON ft.parameterCode=sc.freight_type");
                //sbW.Append(@" LEFT JOIN (SELECT tpc2.parameterName,tpc2.parameterCode FROM t_parametersrc tpc2 WHERE tpc2.parameterType='Deliver_Store'");
                //sbW.Append(" ) DSI ON DSI.parameterCode=sc.delivery_store_id ");
                sbW.Append(" WHERE 1=1 ");
                if (sc.Delivery_store_id > 0)
                {
                    sbW.AppendFormat(" AND sc.delivery_store_id='{0}'", sc.Delivery_store_id);
                }
                totalCount = 0;
                if (sc.IsPage)
                {
                    System.Data.DataTable _dt = _dbAccess.getDataTable(" SELECT COUNT(sc.rid) as totalCount " + sbW.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sbW.AppendFormat(" order by sc.rid desc  limit {0},{1}", sc.Start, sc.Limit);
                }
                sb.Append(sbW.ToString());
                return _dbAccess.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ShippingCarriorDao-->GetShippingCarriorList-->" + ex.Message + "sql:" + sb.ToString(), ex);
            }

        }
        #endregion

        #region 更改物流啟用狀態 + LogisticsUpdateActive(ShippingCarrior sc)
        public int LogisticsUpdateActive(ShippingCarrior sc)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("set sql_safe_updates=0;");
                sb.AppendFormat("update shipping_carrior set active='{0}'", sc.Active);
                sb.AppendFormat("where rid='{0}'", sc.Rid);
                sb.Append(";set sql_safe_updates=1;");
                return _dbAccess.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ShippingCarriorDao-->LogisticsUpdateActive-->" + ex.Message + "sql:" + sb.ToString(), ex);
            }

        }
        #endregion

        #region 物流信息新增檢測是否存在+ LogisticsAddCheck(ShippingCarrior sc)
        public int LogisticsAddCheck(ShippingCarrior sc)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            try
            {
                sb.AppendFormat("select count(rid) as countNum from shipping_carrior where delivery_store_id='{0}'", sc.Delivery_store_id);
                sb.AppendFormat(" and delivery_freight_set='{0}'", sc.Delivery_freight_set);
                System.Data.DataTable _dt = _dbAccess.getDataTable(sb.ToString());
                if (_dt.Rows.Count > 0)
                {
                    i = Convert.ToInt32(_dt.Rows[0]["countNum"].ToString());
                }
                return i;
            }
            catch (Exception ex)
            {
                throw new Exception("ShippingCarriorDao-->LogisticsAddCheck-->" + ex.Message + "sql:" + sb.ToString(), ex);
            }

        }
        #endregion
    }
}
