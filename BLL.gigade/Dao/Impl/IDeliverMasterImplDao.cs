using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
   public interface IDeliverMasterImplDao
    {
       List<DeliverMasterQuery> GetdeliverList(DeliverMasterQuery deliver, out int totalCount);
       List<DeliverMasterQuery> DeliverVerifyList(DeliverMaster dm, out int totalCount);
       List<DeliverMasterQuery> JudgeOrdid(DeliverMaster dm);
       DataTable GetMessageByDeliveryCode(DeliverMasterQuery dmQuery);
       int Updatedeliveryfreightcost(StringBuilder str);
       List<DeliverMasterQuery> GetdeliverListCSV(DeliverMasterQuery deliver);
       DataTable GetReportManagementList(DeliverMasterQuery deliver, out int totalCount);
       DataTable ReportManagementExcelList(DeliverMasterQuery deliver);
       List<DeliverMasterQuery> GetTicketDetailList(DeliverMasterQuery query, out int totalCount);

       DataTable Getlogistics(string type);
       DataTable GetDelayDeliverList(DeliverMasterQuery query, out int totalCount);

       DataTable GetDeliveryMsgList(DeliverMasterQuery deliver, out int totalCount);
       DataTable GetDeliveryMsgExcelList(DeliverMasterQuery deliver);
       DataTable GetVnndorId(string name);

       int GetDeliverMasterCount(DeliverMasterQuery deliver);
    }
}
