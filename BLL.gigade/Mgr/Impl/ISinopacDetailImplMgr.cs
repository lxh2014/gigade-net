using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
   public interface ISinopacDetailImplMgr
    {
       DataTable GetSinopacDetai(SinopacDetail store,string sql);
    }
} 
