/* 
 * 武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ParticularsSrcMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作    者：zhuoqin0830w 
 * 完成日期：2015/05/19
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
        public ParticularsSrcMgr(string XMLPath)
        {
            _particularsSrcDao = new ParticularsSrcDao(XMLPath);
        }

        #region 獲取 ParticularsSrc.xml 文檔的信息  + GetParticularsSrc()
        /// <summary>
        /// 獲取 ParticularsSrc.xml 文檔的信息
        /// </summary>
        /// <returns></returns>
        public List<ParticularsSrc> GetParticularsSrc()
        {
            try
            {
                List<ParticularsSrc> lps = _particularsSrcDao.GetParticularsSrc();
                //遍歷查詢到的 XML 數據  並將 原始 particularsCome  particularsCollect 數據賦值 給 oldCome oldCollect  eidt by zhuoqin0830w  2015/06/01
                lps.ForEach(m =>
                {
                    m.oldCome = m.particularsCome;
                    m.oldCollect = m.particularsCollect;
                });
                return lps;
            }
            catch (Exception ex)
            {
                throw new Exception("ParticularsSrcMgr-->GetParticularsSrc-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 根據 保存期限 添加 或 修改 相關信息  + SaveNode(List<ParticularsSrc> particularsSrc, string connectionString, BLL.gigade.Model.Custom.ProductExtCustom.Condition condition, params int[] ids)
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
            try
            {
                //判斷XML文檔是否修改成功
                if (_particularsSrcDao.SaveNode(particularsSrc))
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

        #region 根據 保存期限 刪除 相關信息  + DeleteNode(string ParticularsName)
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