using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface ISmsImplMgr
    {
        List<SmsQuery> GetSmsList(SmsQuery query, out int totalcount);
        int updateSms(SmsQuery query);
        int InsertSms(SmsQuery query);
    }
}
