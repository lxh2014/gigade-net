using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IDeliverMasterImplMgr 
    {
        List<DeliverMasterQuery> GetdeliverList(DeliverMasterQuery deliver, out int totalCount);
        List<DeliverMasterQuery> DeliverVerifyList(DeliverMaster deliver, out int totalCount);
        List<DeliverMasterQuery> JudgeOrdid(DeliverMaster dm);
        DataTable GetMessageByDeliveryCode(DeliverMasterQuery dmQuery);
        int Updatedeliveryfreightcost(StringBuilder str);
        List<DeliverMasterQuery> GetdeliverListCSV(DeliverMasterQuery deliver);
        DataTable GetReportManagementList(DeliverMasterQuery deliver,out int totalCount);
        DataTable ReportManagementExcelList(DeliverMasterQuery deliver);
        List<DeliverMasterQuery> GetTicketDetailList(DeliverMasterQuery query, out int totalCount);
        DataTable GetDelayDeliverList(DeliverMasterQuery query, out int totalCount);
        DataTable GetDeliveryMsgList(DeliverMasterQuery deliver, out int totalCount);
        DataTable GetDeliveryMsgExcelList(DeliverMasterQuery deliver);
        int GetDeliverMasterCount(DeliverMasterQuery query);
        DataTable GetDeliverMaster(string hourNum);
        int UpdateExpectArrive(DeliverMasterQuery query);
        List<DeliverMasterQuery> GetDeliverExpectArriveList(DeliverMasterQuery query, out int totalCount);
    }
}
