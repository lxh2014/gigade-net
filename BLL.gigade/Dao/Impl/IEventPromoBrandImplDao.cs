﻿using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IEventPromoBrandImplDao
    {
        List<Model.EventPromoBrand> GetList(string event_id);
        string AddOrUpdate(Model.EventPromoBrand epQuery);
        string Delete(string event_id);
    }
}
