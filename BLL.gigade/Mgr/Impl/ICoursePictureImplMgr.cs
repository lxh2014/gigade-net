using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface ICoursePictureImplMgr
    {
        string Save(CoursePicture c);
        List<CoursePicture> Query(CoursePicture cp);
        string Delete(CoursePicture cp);
    }
}
