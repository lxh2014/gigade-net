/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：UsersListMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：dongya0410j 
 * 完成日期：2014/09/22 13:35:21 
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using System.Data;
using BLL.gigade.Model.Query;
using System.Collections;
using MySql.Data.MySqlClient;

namespace BLL.gigade.Mgr
{
    public class UsersListMgr : IUsersListImplMgr
    {
        private IUsersListImplDao _UserIplDao;
        private string conn;
        public UsersListMgr(string connectionString)
        {
            _UserIplDao = new UsersListDao(connectionString);
            conn = connectionString;
        }

        public List<Model.Query.UsersListQuery> Query(Model.Query.UsersListQuery store, out int totalCount)
        {
            return _UserIplDao.Query(store, out totalCount);
        }

        public bool SaveUserList(Model.Query.UsersListQuery usr, List<Model.UserLife> userLife)
        {
            try
            {
                ArrayList sqlList = new ArrayList();
                sqlList.Add(_UserIplDao.SaveUserList(usr));
                UserLifeDao _UserLifeDao = new UserLifeDao(conn);
                sqlList.Add(_UserLifeDao.DeleteUserLife(usr.user_id));//刪除原有數據
                foreach (Model.UserLife model in userLife)
                {
                    sqlList.Add(_UserLifeDao.SaveUserLife(model));
                }

                List<Model.UserLife> old_user_life = _UserLifeDao.GetUserLife(usr.user_id);
                var alist = old_user_life.Find(m => m.info_type == "cancel_info_time");
                if (alist == null)
                {
                    if (!usr.send_sms_ad)
                    {
                        sqlList.Add(_UserLifeDao.UpdateCancelTime(usr.user_id, (uint)Common.CommonFunction.GetPHPTime(), usr.update_user));
                    }
                }
                else
                {
                    if (usr.send_sms_ad)
                    {
                        sqlList.Add(_UserLifeDao.UpdateCancelTime(usr.user_id, 0, usr.update_user));
                    }
                }


                MySqlDao _MySqlDao = new MySqlDao(conn);
                return _MySqlDao.ExcuteSqls(sqlList);
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Number.ToString() + ":UsersListMgr-->SaveUserList-->" + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("UsersListMgr-->SaveUserList-->" + ex.Message, ex);
            }
        }


        public Model.Custom.Users getModel(int id)
        {
            return _UserIplDao.getModel(id);
        }


        public List<Model.Query.BonusMasterQuery> bQuery(Model.Query.BonusMasterQuery store, out int totalCount)
        {
            return _UserIplDao.bQuery(store, out totalCount);
        }

        public int UpdateUser(Model.Custom.Users usr)
        {
            return _UserIplDao.UpdateUser(usr);
        }


        public int updateuser_master(Model.Query.BonusMasterQuery store)
        {
            return _UserIplDao.updateuser_master(store);
        }

        public List<Model.Query.UsersListQuery> Export(Model.Query.UsersListQuery store)
        {
            return _UserIplDao.Export(store);
        }

        public int UserCancel(UsersListQuery u)
        {
            try
            {

                return _UserIplDao.UserCancel(u);
            }
            catch (Exception ex)
            {
                throw new Exception("UsersListMgr-->UserCancel-->" + ex.Message, ex);
            }
        }
        #region 汇出会员查询列表所需要的几个列
        ///chaojie_zz添加于2014/10/08 
        public DataTable GetBonusTotal(DateTime timestart, DateTime timeend, string user_id)/*购物金发放*/
        {
            return _UserIplDao.GetBonusTotal(timestart, timeend, user_id);
        }
        public DataTable GetRecordTotal(DateTime timestart, DateTime timeend, string user_id)/*购物金使用*/
        {
            return _UserIplDao.GetRecordTotal(timestart, timeend, user_id);
        }
        public DataTable GetZipCode()/*查询所有地址*/
        {
            return _UserIplDao.GetZipCode();
        }

        #endregion
    }
}
