using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
   public interface IIpoImplDao
    {
       List<IpoQuery> GetIpoList(IpoQuery query, out int totalcount);
       int UpdateIpo(IpoQuery query);
       int AddIpo(IpoQuery query);
       int SelectIpoCountByIpo(string ipo);
       int DeletIpo(IpoQuery query);
    }
}
