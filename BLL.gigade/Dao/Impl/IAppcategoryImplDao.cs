using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IAppcategoryImplDao
    {
        List<Appcategory> GetAppcategoryList(Appcategory appgory, out int totalCount);
        List<Appcategory> GetParaList(string sql);
        int AppcategoryDelete(Appcategory appgory);
        int AppcategorySave(Appcategory appgory);
    }
}
