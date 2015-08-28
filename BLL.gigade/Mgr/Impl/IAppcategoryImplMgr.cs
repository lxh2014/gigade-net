using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IAppcategoryImplMgr
    {
        List<Appcategory> GetAppcategoryList(Appcategory appgory, out int totalCount);
        string GetParaList(string para, Appcategory appgory);
        int AppcategoryDelete(Appcategory appgory);
        int AppcategorySave(Appcategory appgory);
    }
}
