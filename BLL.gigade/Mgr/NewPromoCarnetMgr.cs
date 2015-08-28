using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Mgr
{
    public class NewPromoCarnetMgr : INewPromoCarnetImplMgr
    {
        private INewPromoCarnetImplDao _INewPromoCarnet;
        private IDBAccess _access;
        public NewPromoCarnetMgr(string connectionString)
        {
            _INewPromoCarnet = new NewPromoCarnetDao(connectionString);
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public DataTable NewPromoCarnetList(NewPromoCarnetQuery query, out int totalCount)
        {
            try
            {
                return _INewPromoCarnet.NewPromoCarnetList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoCarnetMgr-->NewPromoCarnetList-->" + ex.Message, ex);
            }
        }

        public int InsertNewPromoCarnet(NewPromoCarnetQuery query)
        {
            try
            {
                return _access.execCommand(_INewPromoCarnet.InsertNewPromoCarnet(query));
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoCarnetMgr-->InsertNewPromoCarnet-->" + ex.Message, ex);
            }
        }

        public bool UpdateNewPromoCarnet(NewPromoCarnetQuery query)
        {
            try
            {
                if (_access.execCommand(_INewPromoCarnet.UpdateNewPromoCarnet(query)) > 0)
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
                throw new Exception("NewPromoCarnetMgr-->UpdateNewPromoCarnet-->" + ex.Message, ex);
            }
        }

        public NewPromoCarnetQuery GetModel(NewPromoCarnetQuery query)
        {
            try
            {
                return _access.getSinggleObj<NewPromoCarnetQuery>(_INewPromoCarnet.GetModel(query));
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoCarnetMgr-->GetModel-->" + ex.Message, ex);
            }
        }

        public int DeleteNewPromoCarnet(string row_ids)
        {
            try
            {
                return _INewPromoCarnet.DeleteNewPromoCarnet(row_ids);
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoCarnetMgr-->DeleteNewPromoCarnet-->" + ex.Message, ex);
            }
        }

        public bool UpdateActive(NewPromoCarnetQuery query)
        {
            try
            {
                if (_access.execCommand(_INewPromoCarnet.UpdateActive(query)) > 0)
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
                throw new Exception("NewPromoCarnetMgr-->UpdateActive-->" + ex.Message, ex);
            }

        }


        public int GetNewPromoCarnetMaxId()
        {
            try
            {
                return _INewPromoCarnet.GetNewPromoCarnetMaxId();
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoCarnetMgr-->GetNewPromoCarnetMaxId-->" + ex.Message, ex);
            }
        }
    }
}
