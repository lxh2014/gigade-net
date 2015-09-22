using BLL.gigade.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
  public class EdmContentNewMgr
    {

      private EdmContentNewDao _edmContentNewDao;

      public EdmContentNewMgr(string  connectionString)
      {
          _edmContentNewDao = new EdmContentNewDao(connectionString);
      }


    }
}
