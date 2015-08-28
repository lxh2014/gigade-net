using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    public interface IContactUsResponseImplDao
    {
        DataTable GetRecordList(ContactUsResponse query, out int totalcount);
        DataTable GetRecordList(Model.ContactUsResponse query, string startDate, string endDate, string reply_user, out int totalcount);
        int Insert(string sql, ContactUsResponse query);
        int GetMaxResponseId();
    }
}
