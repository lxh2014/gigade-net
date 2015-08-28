using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface IEdmContentImplDao
    {
        List<EdmContentQuery> GetEdmContentList(EdmContentQuery store, out int totalCount);
        int DeleteEdm(int contentId);
        int EdmContentSave(EdmContentQuery store);
        List<EdmContentQuery> GetEdmGroup();
        EdmContentQuery GetEdmContentById(EdmContentQuery query);
        List<EdmContentQuery> GetEdmContent();
        int CancelEdm(string mail);
        string EditStatus(EdmContentQuery query);
    }
}
