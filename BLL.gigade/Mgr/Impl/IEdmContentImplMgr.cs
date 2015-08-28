using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IEdmContentImplMgr
    {
        List<EdmContentQuery> GetEdmContentList(EdmContentQuery store, out int totalCount);
        int DeleteEdm(int contentId);
        List<EdmContentQuery> GetEdmGroup();
        int EdmContentSave(EdmContentQuery store);
        EdmContentQuery GetEdmContentById(EdmContentQuery query);
        string GetEdmContent();
        int CancelEdm(string mail, uint update_id, out uint vid);
        int EditStatus(EdmContentQuery query);
    }
}
