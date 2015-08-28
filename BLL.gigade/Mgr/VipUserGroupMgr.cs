using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
using System.Data;
using System.Collections;

namespace BLL.gigade.Mgr
{
    public class VipUserGroupMgr : IVipUserGroupImplMgr
    {
        private IVipUserGroupImplDao _vipUserGroup;
        private VipUserGroupDao _vipUserGroupDao;
        private ISerialImplDao _ISerialImpl;
        public VipUserGroupMgr(string connectionStr)
        {
            _vipUserGroup = new VipUserGroupDao(connectionStr);
            _vipUserGroupDao = new VipUserGroupDao(connectionStr);
            _ISerialImpl = new SerialDao(connectionStr);
        }
        public List<VipUserGroup> GetAllUserGroup()
        {
            try
            {
                return _vipUserGroup.QueryAll(new VipUserGroup());
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupMgr.GetAllUserGroup-->" + ex.Message, ex);
            }
        }
        public List<VipUserGroupQuery> QueryAll(VipUserGroupQuery query, out int totalCount)
        {
            try
            {
                return _vipUserGroup.QueryAll(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupMgr.QueryAll-->" + ex.Message, ex);
            }
        }
        public string GetVuserCount(VipUserGroup query)
        {
            try
            {
                return _vipUserGroup.GetVuserCount(query);
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupMgr.GetVuserCount-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 根據group_id獲取所有數據
        /// </summary>
        /// <param name="group_id"></param>
        /// <returns></returns>
        public VipUserGroup GetModelById(uint group_id)
        {
            try
            {
                return _vipUserGroup.GetModelById(group_id);
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupMgr.GetModelById-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// vip_user_group數據增加
        /// </summary>
        /// <param name="userGroup"></param>
        /// <returns></returns>
        public int Insert(VipUserGroup userGroup)
        {
            try
            {
                return _vipUserGroup.Insert(userGroup);
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupMgr.Insert-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// vip_user_group數據的編輯更新
        /// </summary>
        /// <param name="userGroup"></param>
        /// <returns></returns>
        public int Update(VipUserGroup userGroup)
        {
            try
            {
                return _vipUserGroup.Update(userGroup);
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupMgr.Update-->" + ex.Message, ex);
            }
        }
        public List<VipUserQuery> GetVipUserList(VipUserQuery vu, out int totalCount)
        {
            try
            {
                return _vipUserGroup.GetVipUserList(vu, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupMgr.GetVipUserList-->" + ex.Message, ex);
            }
        }
        public DataTable GetUser(string sqlwhere)
        {
            try
            {
                return _vipUserGroup.GetUser(sqlwhere);
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupMgr.GetUser-->" + ex.Message, ex);
            }
        }
        public DataTable GetVipUser(VipUser vu)
        {
            try
            {
                return _vipUserGroup.GetVipUser(vu);
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupMgr.GetVipUser-->" + ex.Message, ex);
            }
        }
        public int InsertVipUser(VipUser vu)
        {
            try
            {
                return _vipUserGroup.InsertVipUser(vu);
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupMgr.InsertVipUser-->" + ex.Message, ex);
            }

        }
        public int UpdateUserState(int state, int id)
        {
            try
            {
                return _vipUserGroup.UpdateUserState(state, id);
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupMgr.UpdateUserState-->" + ex.Message, ex);
            }
        }

        public DataTable BtobEmp(string group_id)
        {
            try
            {
                return _vipUserGroup.BtobEmp(group_id);
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupMgr.BtobEmp-->" + ex.Message, ex);
            }
        }
        public int UpdateEmp(string group_id, string erp_id, int k)
        {
            try
            {
                return _vipUserGroup.UpdateEmp(group_id, erp_id, k);
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupMgr.UpdateEmp-->" + ex.Message, ex);
            }
        }
        public int InsertEmp(string group_id, string emp_id)
        {
            try
            {
                return _vipUserGroup.InsertEmp(group_id, emp_id);
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupMgr.InsertEmp-->" + ex.Message, ex);
            }
        }

        #region 企業會員管理
        public List<ZipQuery> GetZipStore()
        {
            try
            {
                return _vipUserGroupDao.GetZipStore();
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao-->GetZipStore-->" + ex.Message , ex);
            }
        }

        public string InsertVipUserGroup(VipUserGroupQuery query)
        {
            string json = string.Empty;
            Serial serial = new Serial();
            ArrayList arrList = new ArrayList();
            try
            {
                query.k_date = DateTime.Now;
                query.m_date = query.k_date;
                query.k_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                query.m_user = query.k_user;
                serial.Serial_id =72;
                serial = _ISerialImpl.GetSerialById(serial.Serial_id);//根據ID得到serial_value;
                query.group_id = Convert.ToUInt32(serial.Serial_Value) + 1;//將serial_value值加1就是group_id;
                arrList.Add( _vipUserGroupDao.InsertVipUserGroup(query));
                arrList.Add(_ISerialImpl.Update(serial.Serial_id));//update serial
                if (_vipUserGroupDao.execSql(arrList))
                {
                    json = "{success:'true',msg:" + query.group_id + "}";
                }
                else
                {
                    json = "{success:'true',msg:" +0+ "}";
 
                }
                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao-->InsertVipUserGroup-->" + ex.Message, ex);
            }
        }

        public List<DeliveryAddress> GetComAddress(DeliveryAddress query)
        {
            try
            {
                return _vipUserGroupDao.GetComAddress(query);
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao-->GetComAddress-->" + ex.Message, ex);
            }
        }

        public string SaveVipUserGroup(VipUserGroupQuery query,DataTable _dt)
        {
            string json = "{success:'true'}";
            ArrayList arrList = new ArrayList();
            VipUser vQuery = new VipUser();
            BtobEmp bQuery=new BtobEmp ();
            int totalCount=0;
            VipUserGroupQuery oldQuery = new VipUserGroupQuery();
            oldQuery = _vipUserGroupDao.GetVipUserGList(query, out totalCount).FirstOrDefault();
            try
            {
                if (query.file_name == "" && oldQuery != null)
                {
                    query.file_name = oldQuery.file_name;
                }
                query.m_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                 query.group_committe_promotion = query.group_committe_promotion + query.group_committe_other;
                arrList.Add(_vipUserGroupDao.SaveVipUserGroup(query));
                bQuery.group_id = query.group_id.ToString();
                //excel檔匯入到btob_emp表
                #region
                if (_dt != null)
                {
                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        if (_dt.Rows[i][0].ToString() != "")
                        {
                            bQuery.emp_id = _dt.Rows[i][0].ToString();
                            bQuery.id = Convert.ToUInt32(_vipUserGroupDao.GetIdFromBtob(bQuery));
                            if (bQuery.id != 0)
                            {
                                arrList.Add(_vipUserGroupDao.GetUpdateBtobSql(bQuery));
                            }
                            else
                            {
                                //emp_id是唯一索引，員工編號在整張表中是唯一的
                                if (!_vipUserGroupDao.IsEmpIdExist(bQuery))
                                {
                                    arrList.Add(_vipUserGroupDao.GetInsertBtobSql(bQuery));
                                }
                            }
                        }
                    }
                }
                else
                {
 
                }


               #endregion

                if (arrList.Count > 0)
                {
                    if (_vipUserGroupDao.execSql(arrList))
                    {
                        json = "{success:'true'}";
                    }
                    else
                    {
                        json = "{success:'false'}";
                    }
                }
                return json;
            }
         
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao-->SaveVipUserGroup-->" + ex.Message, ex);
            }
        }


        public string  SaveComAddress(DeliveryAddress query)
        {
            string json = string.Empty;
            try
            {
                if (query.da_id== 0)
                {
                    if (_vipUserGroupDao.SaveComAddress(query) > 0)
                    {
                        json = "{success:'true'}";
                    }
                    else
                    {
                        json = "{success:'false'}";
                    }
                }
                else
                {
                    int n=100;
                    if (!int.TryParse(query.da_dist, out n))
                    {
                        query.da_dist = _vipUserGroupDao.GetZiPCode(query.da_dist);
                    }
                    if (_vipUserGroupDao.UPComAddress(query) > 0)
                    {
                        json = "{success:'true'}";
                    }
                    else
                    {
                        json = "{success:'false'}";
                    }
                }
             
                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao-->SaveComAddress-->" + ex.Message, ex);
            }
        }


        public List<VipUserGroupQuery> GetVipUserGList(VipUserGroupQuery query, out int totalCount)
        {
            try
            {
                List<VipUserGroupQuery> store = _vipUserGroupDao.GetVipUserGList(query, out totalCount);
                if (query.isSecret)
                {
                    //foreach (var item in store)
                    //{
                    //    item.group_committe_chairman = item.group_committe_chairman.Substring(0, 1) + "**";
                    //    if (item.group_committe_phone.ToString().Length > 3)
                    //    {
                    //        item.group_committe_phone = item.group_committe_phone.Substring(0, 3) + "***";
                    //    }
                    //    else
                    //    {
                    //        item.group_committe_phone = item.group_committe_phone + "***";
                    //    }
                    //    item.group_committe_mail = item.group_committe_mail.Split('@')[0] + "@***";
                    //}
                }


                return store;
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao-->GetVipUserGList-->" + ex.Message, ex);
            }
        }

        public string DeleteDeliveryAddress(DeliveryAddress query)
        {
            string json = string.Empty;
            try
            {
                if (_vipUserGroupDao.DeleteDeliveryAddress(query) > 0)
                {
                    json = "{success:'true'}";
                }
                else
                {
                    json = "{success:'false'}";
                }
                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao-->DeleteDeliveryAddress-->" + ex.Message, ex);
            }
        }

        public int UpVUGStatus(VipUserGroupQuery query)
        {
            int result = 0;
            try
            {
              
                if (query.group_status ==0)
                {
                    DataTable _GSDt = _vipUserGroupDao.IsGroupIdStatus(query);
                    if (_GSDt != null && _GSDt.Rows.Count > 0)//有數據不能變為禁用
                    {
                        result = 0;
                        return result;
                    }
                }
                return _vipUserGroupDao.UpVUGStatus(query);
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao-->UpVUGStatus-->" + ex.Message, ex);
            }
        }

        //public int ExistEmail(VipUserGroupQuery query)
        //{
        //    try
        //    {
        //        return _vipUserGroupDao.ExistEmail(query);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("VipUserGroupDao-->ExistEmail-->" + ex.Message, ex);
        //    }
        //}

        public int ExisGroupCode(VipUserGroupQuery query)
        {
            try
            {
                return _vipUserGroupDao.ExisGroupCode(query);
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserGroupDao-->ExisGroupCode-->" + ex.Message, ex);
            }
        }

        public int ExisTaxId(VipUserGroupQuery query)
          {
              try
              {
                  return _vipUserGroupDao.ExisTaxId(query);
              }
              catch (Exception ex)
              {
                  throw new Exception("VipUserGroupDao-->ExisTaxId-->" + ex.Message, ex);
              }
          }
        
        #endregion

    }
}
