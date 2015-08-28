using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface ICoursePictureImplDao
    {
        string Save(CoursePicture cp);
        List<CoursePicture> Query(CoursePicture cp);
        string Delete(CoursePicture cp);
    }
}
