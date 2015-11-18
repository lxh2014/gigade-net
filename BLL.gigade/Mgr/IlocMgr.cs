/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：IlocMgr.cs
* 摘 要：
* 點數抵用與資料庫交互方法  
* 当前版本：v1.1
* 作 者：dongya0410j    
* 完成日期：2014/11/04 
* 修改歷史:
*         v1.1修改日期：2014/11/04
*         v1.1修改人員：dongya0410j 
*         v1.1修改内容：
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class IlocMgr:IIlocImplMgr
    {
        private IIlocImplDao _IocDao;
        public IlocMgr(string connectionString)
        {
            _IocDao = new IlocDao(connectionString);
        }
        #region 插入料位+int IlocInsert(Model.Iloc loc)
        public int IlocInsert(Model.Iloc loc)
        {
            try
            {
                return _IocDao.IlocInsert(loc);
            }
            catch (Exception ex)
            {
                throw new Exception("IlocMgr-->IlocInsert-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 編輯料位+int IlocEdit(Model.Iloc loc)
        public int IlocEdit(Model.Iloc loc)
        {
            try
            {
                return _IocDao.IlocEdit(loc);
            }
            catch (Exception ex)
            {
                throw new Exception("IlocMgr-->IlocEdit-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 查詢料位或者返回料位列表+ List<Model.Iloc>GetIocList(Model.Iloc loc,out int totalCount)
        public List<IlocQuery> GetIocList(IlocQuery loc, out int totalCount)
        {
            try
            {
                return _IocDao.GetIocList(loc, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("IlocMgr-->GetIocList-->" + ex.Message, ex);
            }
        }
        #endregion


        public int GetLoc_id(Model.Iloc loc)
        {
            try
            {
                return _IocDao.GetLoc_id(loc);
            }
            catch (Exception ex)
            {
                throw new Exception("IlocMgr-->GetLoc_id-->" + ex.Message, ex);
            }
        }


        public string GetLoc_idByRow_id(int row_id)
        {
            try
            {
                return _IocDao.GetLoc_idByRow_id(row_id);
            }
            catch (Exception ex)
            {
                throw new Exception("IlocMgr-->GetLoc_idByRow_id-->" + ex.Message, ex);
            }
        }


        public int DeleteLocidByIloc(Model.Iloc loc)
        {
            try
            {
                return _IocDao.DeleteLocidByIloc(loc);
            }
            catch (Exception ex)
            {
                throw new Exception("IlocMgr-->DeleteLocidByIloc-->" + ex.Message, ex);
            }
        }


        public int UpdateIlocLock(Model.Iloc loc)
        {
            try
            {
                return _IocDao.UpdateIlocLock(loc);
            }
            catch (Exception ex)
            {
                throw new Exception("IlocMgr-->UpdateIlocLock-->" + ex.Message, ex);
            }
        }


        public int SetIlocUsed(Model.Iloc loc)
        {
            try
            {
                return _IocDao.SetIlocUsed(loc);
            }
            catch (Exception ex)
            {
                throw new Exception("IlocMgr-->SetIlocUsed-->" + ex.Message, ex);
            }
        }


        public List<IlocQuery> Export(IlocQuery loc)
        {
            try
            {
                return _IocDao.Export(loc);
            }
            catch (Exception ex)
            {
                throw new Exception("IlocMgr-->Export-->" + ex.Message, ex);
            }
        }


        public List<IlocQuery> GetIlocExportList(IlocQuery loc)
        {
            try
            {
                return _IocDao.GetIlocExportList(loc);
            }
            catch (Exception ex)
            {
                throw new Exception("IlocMgr-->GetIlocExportList-->" + ex.Message, ex);
            }
        }


        public int SaveBySql(string str)
        {
            try
            {
                return _IocDao.SaveBySql(str);
            }
            catch (Exception ex)
            {
                throw new Exception("IlocMgr-->SaveBySql-->" + ex.Message, ex);
            }
        }


        public int HashAll()
        {
            try
            {
                return _IocDao.HashAll();
            }
            catch (Exception ex)
            {
                throw new Exception("IlocMgr-->HashAll-->" + ex.Message, ex);
            }
        }


        public string GetLocidByHash(string loc_id)
        {
            try
            {
                return _IocDao.GetLocidByHash(loc_id);
            }
            catch (Exception ex)
            {
                throw new Exception("IlocMgr-->GetLocidByHash-->" + ex.Message, ex);
            }
        }

        public bool GetIlocCount(IlocQuery loc)
        {
            try
            {
                return _IocDao.GetIlocCount(loc);
            }
            catch (Exception ex)
            {
                throw new Exception("IlocMgr-->GetIlocCount-->" + ex.Message, ex);
            }
        }
    }
}
