/* 
 * 武漢聯綿信息技術有限公司
 *  
 * 文件名称：ParticularsSrcMgr 
 * 摘    要： 
 *  
 * 当前版本：1.0 
 * 作    者：zhuoqin0830w 
 * 完成日期：2015/05/19
 * 
 * 修改日期：2015/09/29
 * 修改原因：將使用 xml 資料庫 改為使用 db 數據庫
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class ParticularsSrcMgr : IParticularsSrcImplMgr
    {
        private IParticularsSrcImplDao _particularsSrcDao;

        private int particularsSrcType;

        public ParticularsSrcMgr(string connectionString, int particularsSrcType = 1)
        {
            this.particularsSrcType = particularsSrcType;
            _particularsSrcDao = new ParticularsSrcDao(connectionString, particularsSrcType);
        }

        #region 獲取 ParticularsSrc  的信息  + GetParticularsSrc()
        /// <summary>
        /// 獲取 ParticularsSrc  的信息
        /// </summary>
        /// <returns></returns>
        public List<ParticularsSrc> GetParticularsSrc()
        {
            try
            {
                List<ParticularsSrc> lps = new List<ParticularsSrc>();
                lps = _particularsSrcDao.GetParticularsSrc();

                #region 判斷 是 使用 數據庫 還是 資料庫  2015/09/29
                switch (particularsSrcType)
                {
                    case 1://使用 db數據庫
                        lps.ForEach(item =>
                        {
                            if (!string.IsNullOrEmpty(item.ParameterCode))
                            {
                                ParticularsSrc ps = new ParticularsSrc();
                                ps.Rowid = item.Rowid;
                                ps.particularsName = item.ParameterProperty;
                                ps.particularsValid = Convert.ToInt32(item.ParameterCode.Split(',')[0]);
                                ps.particularsCollect = Convert.ToInt32(item.ParameterCode.Split(',')[1]);
                                ps.particularsCome = Convert.ToInt32(item.ParameterCode.Split(',')[2]);
                                ps.oldCollect = Convert.ToInt32(item.ParameterCode.Split(',')[1]);
                                ps.oldCome = Convert.ToInt32(item.ParameterCode.Split(',')[2]);
                                lps.Add(ps);
                            }
                        });
                        //傳到前台的數據 應該為  particularsName 有效天數 不為空的集合
                        lps = lps.FindAll(m => m.particularsName != "" && m.particularsValid != 0);
                        break;
                    case 2://使用 xml 資料庫
                        //遍歷查詢到的 XML 數據  並將 原始 particularsCome  particularsCollect 數據賦值 給 oldCome oldCollect  eidt by zhuoqin0830w  2015/06/01
                        lps.ForEach(item =>
                        {
                            item.oldCome = item.particularsCome;
                            item.oldCollect = item.particularsCollect;
                        });
                        break;
                }
                #endregion

                return lps;
            }
            catch (Exception ex)
            {
                throw new Exception("ParticularsSrcMgr-->GetParticularsSrc-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 根據 保存期限 添加 或 修改 相關信息  + SaveNode(List<ParticularsSrc> particularsSrc, string connectionString)
        /// <summary>
        /// 根據 保存期限 添加 或 修改 相關信息
        /// </summary>
        /// <param name="particularsSrc"></param>
        /// <param name="connectionString"></param>
        /// <param name="condition"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool SaveNode(List<ParticularsSrc> particularsSrc, string connectionString)
        {
            bool result = false;
            try
            {
                #region 判斷 是 使用 數據庫 還是 資料庫  2015/09/29
                switch (particularsSrcType)
                {
                    case 1://使用 db數據庫
                        particularsSrc.ForEach(item =>
                         {
                             item.ParameterType = "ParticularsSrc";
                             item.ParameterProperty = item.particularsName;
                             //判斷 參數表 中 Rowid 是否為 0  如果為 0 則表示新增 反之 則為 修改
                             if (item.Rowid != 0)
                             {
                                 item.ParameterCode = item.particularsValid + "," + item.particularsCollect + "," + item.particularsCome + "," + item.oldCollect + "," + item.oldCome;
                             }
                             else
                             {
                                 item.ParameterCode = item.particularsValid + "," + item.particularsCollect + "," + item.particularsCome + ",0,0";
                             }
                             item.ParameterName = "particularsValid,particularsCollect,particularsCome,oldCollect,oldCome";
                             item.Remark = "保存期限對照表";
                         });
                        result = _particularsSrcDao.SaveNode(particularsSrc);
                        break;
                    case 2://使用 xml 資料庫
                        result = _particularsSrcDao.SaveNode(particularsSrc);
                        break;
                }
                #endregion

                //判斷XML文檔 或 數據庫 信息 是否修改成功
                if (result)
                {
                    IProductExtImplDao ped = new ProductExtDao(connectionString);
                    //判斷數據庫數據是否修改成功
                    return ped.UpdateExtByCdedtincr(particularsSrc);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ParticularsSrcDao-->SaveNode-->" + ex.Message, ex);
            }
        }
        #endregion

        #region  根據 保存期限 刪除 相關信息  + DeleteNode(string ParticularsName)
        /// <summary>
        /// 根據 保存期限 刪除 相關信息
        /// </summary>
        /// <param name="incr"></param>
        /// <returns></returns>
        public bool DeleteNode(string ParticularsName)
        {
            try
            {
                return _particularsSrcDao.DeleteNode(ParticularsName);
            }
            catch (Exception ex)
            {
                throw new Exception("ParticularsSrcMgr-->DeleteNode-->" + ex.Message, ex);
            }
        }
        #endregion
    }
}