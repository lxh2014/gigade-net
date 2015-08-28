using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;

namespace BLL.gigade.Mgr
{
    public class ContactUsResponseMgr : IContactUsResponseImplMgr
    {
        private Dao.Impl.IContactUsResponseImplDao _contResponse;

        public ContactUsResponseMgr(string connectionString)
        {
            _contResponse = new Dao.ContactUsResponseDao(connectionString);
        }


        public System.Data.DataTable GetRecordList(Model.ContactUsResponse query, out int totalcount)
        {
            try
            {
                return _contResponse.GetRecordList(query, out totalcount);
            }
            catch (Exception ex)
            {
                throw new Exception("ContactUsQuestionMgr-->GetContactUsQuestionList-->" + ex.Message, ex);
            }
        }


        public System.Data.DataTable GetRecordList(Model.ContactUsResponse query, string startDate, string endDate, string reply_user, out int totalcount)
        {
            try
            {
                return _contResponse.GetRecordList(query, startDate, endDate, reply_user, out totalcount);
            }
            catch (Exception ex)
            {
                throw new Exception("ContactUsQuestionMgr-->GetContactUsQuestionList-->" + ex.Message, ex);
            }
        }
        public int Insert(string sql, Model.ContactUsResponse query)
        {
            try
            {
                return _contResponse.Insert(sql, query);
            }
            catch (Exception ex)
            {
                throw new Exception("ContactUsQuestionMgr-->Insert-->" + ex.Message, ex);
            }
        }


        public int GetMaxResponseId()
        {
            try
            {
                return _contResponse.GetMaxResponseId();
            }
            catch (Exception ex)
            {
                throw new Exception("ContactUsQuestionMgr-->GetMaxResponseId-->" + ex.Message, ex);
            }
        }


    }
}
