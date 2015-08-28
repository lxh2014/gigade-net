using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface ICalendarImplDao
    {
        List<Calendar> Query();
        int Save(Calendar c);
        int Update(Calendar c);
        int Delete(Calendar c);
        List<Calendar> GetCalendarInfo(Calendar c);
    }
}
