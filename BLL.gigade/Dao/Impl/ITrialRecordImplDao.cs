using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
  public  interface ITrialRecordImplDao
    {
      DataTable GetShareList(TrialShareQuery query, out int totalCount); 
      int TrialRecordUpdate(TrialRecordQuery query);
      int TrialRecordSave(TrialShareQuery query);
      TrialRecordQuery GetTrialRecordById(TrialRecordQuery query);
      List<Model.Query.TrialRecordQuery> GetTrialRecordList(TrialRecordQuery store, out int totalCount);
      DataTable GetSumCount(PromotionsAmountTrialQuery query);
      bool VerifyMaxCount(TrialRecordQuery query);
      TrialShare GetTrialShare(TrialShare model);
    }
} 
