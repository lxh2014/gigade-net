using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IPromoTicketImplMgr
    {
        int Save(Model.PromoTicket model);
        int Update(Model.PromoTicket model);
        int Delete(int rid);
        PromoTicket Query(int rid);
    }
}
