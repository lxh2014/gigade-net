using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using BLL.gigade.Mgr;
using BLL.gigade.Common;
using BLL.gigade.Model;
using System.Data;
namespace BLL.gigade.Dao
{
    public class AreaPacketDao : IAreaPacketImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        string _connStr = string.Empty;
        public AreaPacketDao(string connectionString)
        {
            _connStr = connectionString;
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        #region 區域包列表頁查詢+List<AreaPacket> QueryAll(AreaPacket query, out int totalCount)
        /// <summary>
        /// 區域包列表頁查詢
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<AreaPacket> QueryAll(AreaPacket query, out int totalCount)
        {
            query.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
       
            try
            {
                StringBuilder sql = new StringBuilder();
                StringBuilder sqlcount = new StringBuilder();
                StringBuilder sqlfrom = new StringBuilder();
                StringBuilder sqlwhere = new StringBuilder();
                sql.Append(@"select packet_id,packet_name,show_number,packet_sort,element_type,packet_status,packet_desc,packet_createdate,packet_updatedate,create_userid,update_userid ");
                sqlcount.Append("select count(packet_id) as totalcounts ");
                sqlfrom.Append(" FROM area_packet ");
                if (query.packet_id != 0)
                {
                    sqlwhere.AppendFormat(" and packet_id={0}", query.packet_id);
                }

                if (!string.IsNullOrEmpty(query.packet_name) || !string.IsNullOrEmpty(query.packet_desc))
                {
                    sqlwhere.AppendFormat("  and (packet_name like N'%{0}%' or packet_desc like '%{1}%')", query.packet_name, query.packet_desc);
                }
                if (query.element_type != 0)
                {
                    sqlwhere.AppendFormat(" and element_type='{0}'", query.element_type);
                }

                if (sqlwhere.Length != 0)
                {
                    sqlfrom.Append(" WHERE ");
                    sqlfrom.Append(sqlwhere.ToString().TrimStart().Remove(0, 3));
                }
                totalCount = 0;
                if (query.IsPage)
                {
                    sb.Append(sqlcount.ToString() + sqlfrom.ToString() + ";");
                    System.Data.DataTable _dt = _access.getDataTable(sqlcount.ToString() + sqlfrom.ToString());

                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalcounts"]);
                    }

                    sqlfrom.AppendFormat(" ORDER BY packet_id DESC ");
                    sqlfrom.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                sb.Append(sql.ToString() + sqlfrom.ToString());
                return _access.getDataTableForObj<AreaPacket>(sql.ToString() + sqlfrom.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AreaPacketDao-->QueryAll-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 保存新增編輯區域包數據+int AreaPacketSave(AreaPacket ap)
        /// <summary>
        /// 保存新增區域數據
        /// </summary>
        /// <param name="ba"></param>
        /// <returns></returns>
        public int AreaPacketSave(AreaPacket ap)
        {
            ap.Replace4MySQL();
            StringBuilder strSql = new StringBuilder();
            try
            {
                if (ap.packet_id == 0)
                {
                    strSql.AppendFormat("insert into area_packet (packet_name,show_number,packet_sort,element_type,packet_status,packet_desc,packet_createdate,packet_updatedate,create_userid,update_userid)value('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", ap.packet_name, ap.show_number, ap.packet_sort, ap.element_type, ap.packet_status, ap.packet_desc, ap.packet_createdate.ToString("yyyy-MM-dd HH:mm:ss"), ap.packet_updatedate.ToString("yyyy-MM-dd HH:mm:ss"), ap.create_userid, ap.update_userid);
                    return _access.execCommand(strSql.ToString());
                }
                else
                {
                    strSql.AppendFormat(@"update area_packet  set 
              packet_name='{0}',
               packet_desc='{1}',packet_sort='{2}',
               packet_updatedate='{3}',
                update_userid='{4}',show_number='{5}',element_type='{6}' where packet_id={7}",
                  ap.packet_name,
                  ap.packet_desc, ap.packet_sort,
                   ap.packet_updatedate.ToString("yyyy-MM-dd HH:mm:ss"),
                  ap.update_userid, ap.show_number, ap.element_type, ap.packet_id);
                    return _access.execCommand(strSql.ToString());
                }

            }
            catch (Exception ex)
            {

                throw new Exception("AreaPacketDao-->AreaPacketSave-->" + ex.Message + strSql.ToString(), ex);
            }

        }
        #endregion

        #region 根據某個id獲取該id下的數據+AreaPacket GetModelById(AreaPacket model)
        public AreaPacket GetModelById(AreaPacket model)
        {
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"select packet_id,packet_name,show_number,packet_sort,element_type,packet_status,packet_desc,packet_createdate,packet_updatedate,create_userid,update_userid  from area_packet where packet_id ={0}", model.packet_id);
                return _access.getSinggleObj<AreaPacket>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AreaPacketDao-->GetModelById-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region  更新區域包的狀態+int UpAreaPacketStatus(AreaPacket model)
        /// <summary>
        /// 更新區域包的狀態
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpAreaPacketStatus(AreaPacket model)
        {
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("update area_packet set packet_status={0},packet_updatedate='{1}',update_userid={2} where packet_id={3} ", model.packet_status, CommonFunction.DateTimeToString(model.packet_updatedate), model.update_userid, model.packet_id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AreaPacketDao-->UpAreaPacketStatus-->" + ex.Message + sb.ToString(), ex);
            }

        }
        #endregion


        public List<AreaPacket> GetPacket(int element_type)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select packet_id,packet_name from area_packet where packet_status=1 ");
                if (element_type != 0)
                {
                    sql.AppendFormat(" and element_type={0}", element_type);
                }
                return _access.getDataTableForObj<AreaPacket>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AreaPacketDao-->GetPacket-->" + ex.Message + sql.ToString(), ex);
            }
        }


        public bool SelectCount(int packetId)
        {
            StringBuilder sqlElementD = new StringBuilder();
            StringBuilder sqlAreaPacket = new StringBuilder();
            DataTable _dtElementD = new DataTable();
            DataTable _dtAreaPacket = new DataTable();
            try
            {
                sqlElementD.AppendFormat("select count(packet_id) as totalCount from element_detail where packet_id={0} and element_status=1;", packetId);
                sqlAreaPacket.AppendFormat("select show_number from area_packet  where packet_id={0};", packetId);
                _dtElementD = _access.getDataTable(sqlElementD.ToString());
                _dtAreaPacket = _access.getDataTable(sqlAreaPacket.ToString());
                if (Convert.ToInt32(_dtElementD.Rows[0]["totalCount"]) >= Convert.ToInt32(_dtAreaPacket.Rows[0]["show_number"]))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                string sql = sqlElementD.ToString() + sqlAreaPacket.ToString();
                throw new Exception("AreaPacketDao-->SelectCount-->" + sql.ToString() + ex.Message, ex);
            }

        }
    }
}
