using BLL.gigade.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class EdmEmailMgr
    {
        private EdmEmailDao _edmemailDao;
        public EdmEmailMgr(string connectionString)
        {
            try
            {
                _edmemailDao = new EdmEmailDao(connectionString);
            }
            catch (Exception ex)
            {

                throw new Exception("EdmTestMgr-->EdmTestMgr-->", ex);
            }

        }

        public void GetData(string email_address, int id, out uint largestId, out uint email_id, out string email_name)
        {
            try
            {
                _edmemailDao.GetData(email_address, out largestId, out email_id, out email_name);
            }
            catch (Exception ex)
            {

                throw new Exception("EdmTestMgr-->GetData-->", ex);
            }

        }

        public void GetEmailByID(uint id, out string email_name, out string email_address)
        {
            DataTable dt = new DataTable();
            email_address = string.Empty;
            email_name = string.Empty;
            try
            {
                dt = _edmemailDao.GetEmailByID(id);
                if (dt != null && dt.Rows.Count > 0)
                {
                    email_address = dt.Rows[0][0].ToString();
                    email_name = dt.Rows[0][1].ToString();
                }
            }
            catch (Exception ex)
            {

                throw new Exception("EdmTestMgr-->GetEmailByID-->", ex);
            }
        }

        public DataTable GetEdmPersonList(EdmEmailQuery query, out int totalCount)
        {
            try
            {
                return _edmemailDao.GetEdmPersonList(query, out totalCount);

            }
            catch (Exception ex)
            {

                throw new Exception("EdmTestMgr-->GetEdmPersonList-->", ex);
            }
        }

        public DataTable GetPersonList(EdmEmailQuery query, out int totalCount)
        {
            try
            {
                return _edmemailDao.GetPersonList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("EdmTestMgr-->GetPersonList-->", ex);
            }
        }
    }
}
