using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using BLL.gigade.Common;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Mgr;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Dao
{
    public class BonusMasterDao : IBonusMasterImplDao
    {
        private IDBAccess _access;
        public string connectionStr;
        public BonusMasterDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connectionStr = connectionString;
        }

        public List<BonusMasterQuery> BonusTypeStore()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select type_id,type_description from bonus_type ORDER BY type_id DESC;");
                return _access.getDataTableForObj<BonusMasterQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BonusMasterDao-->BonusTypeStore-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public List<BonusMasterQuery> GetBonusMasterList(BonusMasterQuery query, ref int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sql.Append(@"select bm.master_id,bm.user_id,bm.type_id,bm.master_total,bm.master_balance");
                sql.Append(@",bm.master_note,bm.master_writer, FROM_UNIXTIME(bm.master_start) as smaster_start,FROM_UNIXTIME(bm.master_end) as smaster_end,bm.bonus_type");
                sql.Append(@",bt.type_admin_link,bt.type_description,FROM_UNIXTIME(bm.master_createdate) as smaster_createtime  ");
                sqlCondi.Append(@" from bonus_master bm ");
                sqlCondi.Append(@" left join bonus_type bt on bm.type_id=bt.type_id ");
                sqlCondi.Append(@" where 1=1");
                if (query.user_id != 0)
                {
                    sqlCondi.AppendFormat(@" and bm.user_id='{0}'", query.user_id);
                }
                sqlCondi.Append(@" order by bm.master_end desc,bm.master_start desc");
                totalCount = 0;
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable("select count(1) as totalCount " + sqlCondi.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"].ToString());
                    }
                    sqlCondi.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                sql.Append(sqlCondi.ToString());
                return _access.getDataTableForObj<BonusMasterQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BonusMasterDao-->GetBonusMasterList-->" + sql.ToString() + ex.Message, ex);
            }
        }
        /// <summary>
        /// 計算購物金每條詳情的狀態
        /// // 購物金
        ///$aLang['Bonus_Status'][1]			= '尚未開通';
        ///$aLang['Bonus_Status'][2]			= '已過期';
        ///$aLang['Bonus_Status'][3]			= '未使用';
        ///$aLang['Bonus_Status'][4]			= '尚餘點數';
        ///$aLang['Bonus_Status'][5]			= '已用完';
        ///$aLang['Bonus_Status'][6]			= '已取消';
        /// </summary>   
        /// <param name="query"></param>
        /// <param name="now"></param>
        /// <param name="master_id"></param>
        /// <returns></returns>
        public int CheckBonusMasterStatus(BonusMasterQuery query, DateTime now, int master_id = 1)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sql.Append(@"select count(1) as search_total");
                sqlCondi.Append(@" from bonus_record  ");
                sqlCondi.AppendFormat(@" where master_id={0}", master_id);
                sqlCondi.Append(@" and type_id = 63");
                sql.Append(sqlCondi.ToString());
                DataTable _dt = _access.getDataTable(sql.ToString());
                if (_dt.Rows.Count > 0)
                {
                    int search_total = Convert.ToInt32(_dt.Rows[0]["search_total"].ToString());
                    if (search_total > 0)
                    {
                        return 6;
                    }

                }
                if (now < query.smaster_start)
                {
                    return 1;
                }
                else if (now > query.smaster_end)
                {
                    return 2;
                }
                else if (query.master_total <= query.master_balance)
                {
                    return 3;
                }
                else if (query.master_balance > 0)
                {
                    return 4;
                }
                else
                {
                    return 5;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("BonusMasterDao-->GetBonusMasterList-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public string InsertBonusMaster(BonusMasterQuery store)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("insert into bonus_master(`master_id`,`user_id`,`type_id`,`master_total`,`master_balance`,`master_note` ");
            sql.Append(" ,`master_writer`,`master_start`,`master_end`,`master_createdate`,`master_updatedate`,`master_ipfrom`,`bonus_type`)");
            sql.AppendFormat("values({0},{1},{2},{3},{4},'{5}'", store.master_id, store.user_id, store.type_id, store.master_total, store.master_balance, store.master_note);
            sql.AppendFormat(",'{0}',{1},{2},{3},{4},'{5}',{6});", store.writer, CommonFunction.GetPHPTime(store.smaster_start.ToString()), CommonFunction.GetPHPTime(store.smaster_end.ToString()), CommonFunction.GetPHPTime(store.smaster_createtime.ToString()), CommonFunction.GetPHPTime(store.smaster_updatedate.ToString()), store.master_ipfrom, store.bonus_type);

            return sql.ToString();
        }

        public bool UpdateBonusMaster(BonusMasterQuery store)
        {
            store.Replace4MySQL();
            StringBuilder sql = new StringBuilder();

            try
            {
                sql.AppendFormat("update  bonus_master set `user_id`='{0}',`type_id`='{1}',`master_total`='{2}',`master_balance`='{3}',`master_note`='{4}' ", store.user_id, store.type_id, store.master_total, store.master_balance, store.master_note);
                sql.AppendFormat(" ,`master_writer`='{0}',`master_start`='{1}',`master_end`='{2}',`master_updatedate`='{3}',`master_ipfrom`='{4}',`bonus_type`='{5}')", store.master_writer, store.master_start, store.master_end, store.master_updatedate, store.master_ipfrom, store.bonus_type);
                if (_access.execCommand(sql.ToString()) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("BonusMasterDao-->UpdateBonusMaster-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public int GetUserBonus(int user_id)
        {
            StringBuilder sbSql = new StringBuilder();
            try
            {

                sbSql.AppendFormat(@"select	sum(master_balance) as master_balance from bonus_master 
 where user_id={0},master_start<={1},master_end>={1}", user_id, Common.CommonFunction.GetPHPTime());
                return Convert.ToInt32(_access.getDataTable(sbSql.ToString()).Rows[0][0]);
            }
            catch (Exception ex)
            {

                throw new Exception("BonusMasterDao-->GetUserBonus-->" + sbSql.ToString() + ex.Message, ex);
            }
        }

        #region 根據type_id獲取類型描述 +BonusType GetBonusTypeFromId(int type_id)
        /// <summary>
        /// 根據type_id獲取類型描述
        /// </summary>
        /// <param name="type_id"></param>
        /// <returns></returns>
        public BonusType GetBonusTypeFromId(int type_id)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat("select type_id,type_description from bonus_type where type_id={0}", type_id);
            try
            {
                return _access.getSinggleObj<BonusType>(sbSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("BonusMasterDao-->GetBonusTypeFromId-->" + sbSql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// 找出目前可用購物金
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<BonusMasterQuery> GetBonusMasterListByUser(int user_id)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sql.Append(@"select bm.master_id,bm.user_id,bm.type_id,bm.master_total,bm.master_balance");
                sql.Append(@",bm.master_note,bm.master_writer, FROM_UNIXTIME(bm.master_start) as smaster_start,FROM_UNIXTIME(bm.master_end) as smaster_end,bm.bonus_type");
                sql.Append(@",bt.type_admin_link,bt.type_description,FROM_UNIXTIME(bm.master_createdate) as smaster_createtime  ");
                sqlCondi.Append(@" from bonus_master bm ");
                sqlCondi.Append(@" left join bonus_type bt on bm.type_id=bt.type_id ");
                sqlCondi.Append(@" where 1=1");
                if (user_id != 0)
                {
                    sqlCondi.AppendFormat(@" and bm.user_id='{0}'", user_id);
                }
                sqlCondi.Append(@" order by bm.master_end asc");

                sql.Append(sqlCondi.ToString());
                return _access.getDataTableForObj<BonusMasterQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BonusMasterDao-->GetBonusMasterList-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public bool InsertIntoBonusRecord(BonusRecord bonusRecord)
        {
            StringBuilder sbSql = new StringBuilder();
            try
            {
                sbSql.AppendFormat(@"insert into bonus_record (record_id,master_id,type_id,");
                sbSql.AppendFormat(@"order_id,record_use,record_note,record_writer");
                sbSql.AppendFormat(@"record_createdate,record_updatedate,record_ipfrom)");
                sbSql.AppendFormat(@" values({0},{1},{2},{3},", bonusRecord.record_id, bonusRecord.master_id, bonusRecord.type_id, bonusRecord.order_id);
                sbSql.AppendFormat(@"{0},{1},{2},", bonusRecord.record_use, bonusRecord.record_note, bonusRecord.record_writer);
                sbSql.AppendFormat(@"{0},{1},{2})", CommonFunction.GetPHPTime(DateTime.Now.ToString()), CommonFunction.GetPHPTime(DateTime.Now.ToString()), bonusRecord.record_ipfrom);

                return _access.execCommand(sbSql.ToString()) > 0;
            }
            catch (Exception ex)
            {

                throw new Exception("BonusMasterDao-->InsertIntoBonusRecord-->" + sbSql.ToString() + ex.Message, ex);
            }
        }

        public bool UpdateBonusMasterMasterBalance(BonusMasterQuery query)
        {
            string sql = String.Empty;
            try
            {
                sql = string.Format("update bonus_master SET	master_balance = {0} where master_id={1} ", query.master_balance, query.master_id);
                return _access.execCommand(sql) > 0;
            }
            catch (Exception ex)
            {

                throw new Exception("BonusMasterDao-->UpdateBonusMasterMasterBalance-->" + sql.ToString() + ex.Message, ex);
            }
        }

        #region 向 bonus_master 裱中 添加 一筆數據  add by zhuoqin0830w 2015/08/24
        /// <summary>
        /// 向 bonus_master 裱中 添加 一筆數據
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public string AddBonusMaster(BonusMaster bm)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                StringBuilder master = new StringBuilder();
                //獲取 參數裱中 最大的 master_id 并 +1
                master.Append(@"SELECT MAX(master_id) FROM bonus_master;");
                uint master_id = Convert.ToUInt32(_access.getDataTable(master.ToString()).Rows[0][0].ToString());
                master_id++;
                //獲取使用者電腦IP
                System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
                string ip = string.Empty;
                if (addlist.Length > 0)
                {
                    ip = addlist[0].ToString();
                }
                strSql.Append(@"INSERT INTO bonus_master(master_id,user_id,type_id,master_total,master_balance,master_writer,master_start,master_end,master_createdate,master_updatedate,master_ipfrom,bonus_type) VALUES(");
                strSql.AppendFormat(@"{0},{1},{2},{3},{4},'{5}',", master_id, bm.user_id, bm.type_id, bm.master_total, bm.master_balance, bm.master_writer);
                strSql.AppendFormat(@"{0},{1},{2},{3},'{4}',{5});", bm.master_start, bm.master_end, bm.master_createdate, bm.master_updatedate, ip, bm.bonus_type);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("BonusMasterDao-->AddBonusMaster(BonusMaster bm)-->" + strSql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 得到 bonus_master 裱中 購物金的 總和  add by zhuoqin0830w 2015/08/24
        /// <summary>
        /// 得到 bonus_master 裱中 購物金的 總和
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public int GetSumBouns(BonusRecord br)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat(@"SELECT SUM(master_balance) AS master_balance FROM bonus_master WHERE user_id = {0};", br.user_id);
                return Convert.ToInt32(_access.getDataTable(strSql.ToString()).Rows[0][0]);
            }
            catch (Exception ex)
            {
                throw new Exception("BonusMasterDao-->GetSumBouns(BonusRecord br)-->" + strSql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 根據 User_id 和 時間 的倒序查詢出相應的列表   add by zhuoqin0830w 2015/08/24
        /// <summary>
        /// 根據 User_id 和 時間 的倒序查詢出相應的列表
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public List<BonusMaster> GetBonusMasterByEndTime(BonusRecord br)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat(@"SELECT master_id,user_id,type_id,master_total,master_balance,master_note,master_writer,master_start,master_end FROM bonus_master WHERE user_id = {0} ORDER BY master_end ASC;", br.user_id);
                return _access.getDataTableForObj<BonusMaster>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BonusMasterDao-->GetBonusMasterByEndTime(BonusRecord br)-->" + strSql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 根據 master_id 修改數據   add by zhuoqin0830w 2015/08/24
        /// <summary>
        /// 根據 master_id 修改數據 
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public string UpdateBonusMasterBalance(BonusMaster bm)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat(@"UPDATE bonus_master SET master_balance = {0},master_updatedate = {1} where master_id = {2};", bm.master_balance, bm.master_updatedate, bm.master_id);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("BonusMasterDao-->UpdateBonusMasterBalance(BonusMaster bm)-->" + strSql.ToString() + ex.Message, ex);
            }
        }
        #endregion
    }
}
