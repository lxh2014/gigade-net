using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface IDeliverDetailImplDao
    {
        List<DeliverDetailQuery> GetDeliverDetail(DeliverDetailQuery dd);
        List<DeliverMasterQuery> GetDeliverMaster(DeliverMasterQuery dm);
        string ProductMode(string deliver_id, string detail_id, string product_mode);
        bool DeliveryCode(string deliver_id, string delivery_store, string delivery_code, string delivery_date, string vendor_id);
        bool NoDelivery(string deliver_id, string detail_id);
        bool SplitDetail(string deliver_id, string detail_id);
        string Split(string deliver_id, string[] detail_ids);
        int UpSmsTime(string deliver_id, string sms_date, string sms_id);
        string GetSmsId(Sms sms);
        int DeliverMasterEdit(DeliverMaster dm, int type);
        object GetChannelOrderList(DeliverMasterQuery dmq, out int totalCount, int type=0);
        DataTable GetOrderDelivers(string deliver_id, int type = 0);
        DataTable GetWayBills(string deliver_id, string ticketids);
    }
}
