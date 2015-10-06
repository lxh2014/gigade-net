using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class BonusMasterMgr : IBonusMasterImplMgr
    {

        private IBonusMasterImplDao _IBonusMasterDao;
        private ISerialImplDao _ISerialDao;
        private MySqlDao _mysqlDao;
        public BonusMasterMgr(string connectionString)
        {
            _IBonusMasterDao = new BonusMasterDao(connectionString);
            _ISerialDao = new SerialDao(connectionString);
            _mysqlDao = new MySqlDao(connectionString);
        }

        public List<BonusMasterQuery> BonusTypeStore()
        {
            try
            {
                return _IBonusMasterDao.BonusTypeStore();
            }
            catch (Exception ex)
            {
                throw new Exception("BonusMasterMgr-->BonusTypeStore-->" + ex.Message, ex);
            }
        }
        public List<BonusMasterQuery> GetBonusMasterList(BonusMasterQuery query, ref int totalCount)
        {
            try
            {
                List<BonusMasterQuery> store = _IBonusMasterDao.GetBonusMasterList(query, ref totalCount);
                DateTime now = DateTime.Now;
                foreach (BonusMasterQuery item in store)
                {

                    item.master_status = _IBonusMasterDao.CheckBonusMasterStatus(item, now);
                }
                return store;
            }
            catch (Exception ex)
            {
                throw new Exception("BonusMasterMgr-->GetBonusMasterList-->" + ex.Message, ex);
            }
        }

        public DataTable GetBonusMasterList(BonusMasterQuery query)
        {
            try
            {
                return _IBonusMasterDao.GetBonusMasterList(query); 
            }
            catch (Exception ex)
            {
                throw new Exception("BonusMasterMgr-->GetBonusMasterList-->" + ex.Message, ex);
            }
        }
        public bool BonusMasterAdd(List<BonusMasterQuery> list)
        {
            Serial serial = new Serial();
            ArrayList arrayList = new ArrayList();
            try
            {
                //define('SERIAL_ID_BONUS_MASTER',		27);	// 購物金流水號
                serial = _ISerialDao.GetSerialById(27);
                for (int i = 0; i < list.Count; i++)
                {
                    //得到+1後的serialValue,是bonus_master的master_id
                    serial.Serial_Value++;
                    list[i].master_id = Convert.ToUInt32(serial.Serial_Value);
                    //更新serial表
                    arrayList.Add(_ISerialDao.UpdateAutoIncreament(serial));
                    //插入BonusMaster數據
                    arrayList.Add(_IBonusMasterDao.InsertBonusMaster(list[i]));
                }
                return _mysqlDao.ExcuteSqls(arrayList);
            }
            catch (Exception ex)
            {
                throw new Exception("BonusMasterMgr-->BonusMasterAdd-->" + arrayList.ToString() + ex.Message, ex);
            }
        }

        public bool BonusMasterUpdate(BonusMasterQuery store)
        {
            try
            {

                return _IBonusMasterDao.UpdateBonusMaster(store);
            }
            catch (Exception ex)
            {
                throw new Exception("BonusMasterMgr-->BonusMasterAdd-->" + ex.Message, ex);
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
            try
            {
                return _IBonusMasterDao.AddBonusMaster(bm);
            }
            catch (Exception ex)
            {
                throw new Exception("BonusMasterMgr-->AddBonusMaster(BonusMaster bm)-->" + ex.Message, ex);
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
            try
            {
                return _IBonusMasterDao.GetSumBouns(br);
            }
            catch (Exception ex)
            {
                throw new Exception("BonusMasterMgr-->GetSumBouns(BonusRecord br)-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 根據 User_id 和 時間 的倒序查詢出相應的列表   add by zhuoqin0830w 2015/08/24
        /// <summary>
        /// 根據 User_id 和 時間 的倒序查詢出相應的列表
        /// </summary>
        /// <param name="bm"></param>
        /// <returns></returns>
        public List<BonusMaster> GetBonusByEndTime(BonusRecord br)
        {
            try
            {
                return _IBonusMasterDao.GetBonusByEndTime(br);
            }
            catch (Exception ex)
            {
                throw new Exception("BonusMasterMgr-->GetBonusMasterByEndTime(BonusRecord br)-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 得到 bonus_master 裱中 抵用卷 總和  add by zhuoqin0830w 2015/08/24
        /// <summary>
        /// 得到 bonus_master 裱中 抵用卷 總和
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public int GetSumWelfare(BonusRecord br)
        {
            try
            {
                return _IBonusMasterDao.GetSumWelfare(br);
            }
            catch (Exception ex)
            {
                throw new Exception("BonusMasterMgr-->GetSumWelfare(BonusRecord br)-->"  + ex.Message, ex);
            }
        }
        #endregion

        #region 根據 User_id 和 時間 的倒序查詢出相應的列表  抵用券  add by zhuoqin0830w 2015/08/24
        /// <summary>
        /// 根據 User_id 和 時間 的倒序查詢出相應的列表 抵用券
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public List<BonusMaster> GetWelfareByEndTime(BonusRecord br)
        {
            try
            {
                return _IBonusMasterDao.GetWelfareByEndTime(br);
            }
            catch (Exception ex)
            {
                throw new Exception("BonusMasterDao-->GetWelfareByEndTime(BonusRecord br)-->"+ex.Message, ex);
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
            try
            {
                return _IBonusMasterDao.UpdateBonusMasterBalance(bm);
            }
            catch (Exception ex)
            {
                throw new Exception("BonusMasterMgr-->UpdateBonusMasterBalance(BonusMaster bm)-->" + ex.Message, ex);
            }
        }
        #endregion

        //是否有發放購物金  write for cj
        public List<BonusMasterQuery>  IsExtendBonus(BonusMasterQuery query)
        {
            try
            {
                return _IBonusMasterDao.IsExtendBonus(query);
            }
            catch (Exception ex)
            {
                throw new Exception("BonusMasterMgr-->UpdateBonusMasterBalance(BonusMaster bm)-->" + ex.Message, ex);
            }
        }

        public ArrayList regainBonus(BonusMasterQuery bm, BonusRecord br)
        {
            ArrayList arrList = new ArrayList();
            try
            {
                arrList.Add(_IBonusMasterDao.UpdateBonusMaster(bm));
                arrList.Add(_IBonusMasterDao.InsertBonusRecord(br));
                return arrList;
            }
            catch (Exception ex)
            {
                throw new Exception("BonusMasterMgr-->regainBonus-->" + ex.Message, ex);
            }
        }
    }
}
