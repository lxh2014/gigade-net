using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using System.Data;

namespace BLL.gigade.Dao.Impl
{
    interface IPaperAnswerImplDao
    {
        List<PaperAnswer> GetPaperAnswerList(PaperAnswer pa, out int totalCount);
        DataTable Export(PaperAnswer pa);
        DataTable GetPaperClassID(PaperClass pc);
        DataTable GetPaperAnswerUser(PaperAnswer pa);
        DataTable ExportSinglePaperAnswer(PaperAnswer pa);
    }
}
