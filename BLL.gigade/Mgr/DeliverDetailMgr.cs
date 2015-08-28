using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Mgr.Impl;

namespace BLL.gigade.Mgr
{
    public class DeliverDetailMgr : IDeliverDetailImplMgr
    {
        private IDeliverDetailImplDao _IDeliverDetailDao;
        public DeliverDetailMgr(string connectionString)
        {
            _IDeliverDetailDao = new DeliverDetailDao(connectionString);
        }
        public List<DeliverDetailQuery> GetDeliverDetail(DeliverDetailQuery dd)
        {
            try
            {
                return _IDeliverDetailDao.GetDeliverDetail(dd);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverDetailMgr-->GetDeliverDetail-->" + ex.Message, ex);
            }
        }
        public List<DeliverMasterQuery> GetDeliverMaster(DeliverMasterQuery dm)
        {
            try
            {
                return _IDeliverDetailDao.GetDeliverMaster(dm);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverDetailMgr-->GetDeliverMaster-->" + ex.Message, ex);
            }
        }
        public string ProductMode(string deliver_id, string detail_id, string product_mode)
        {
            try
            {
                return _IDeliverDetailDao.ProductMode(deliver_id, detail_id, product_mode);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverDetailMgr-->ProductMode-->" + ex.Message, ex);
            }
        }
        public bool DeliveryCode(string deliver_id, string delivery_store, string delivery_code, string delivery_date, string vendor_id)
        {
            try
            {
                return _IDeliverDetailDao.DeliveryCode(deliver_id, delivery_store, delivery_code, delivery_date, vendor_id);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverDetailMgr-->DeliveryCode-->" + ex.Message, ex);
            }
        }
        public bool NoDelivery(string deliver_id, string detail_id)
        {
            try
            {
                return _IDeliverDetailDao.NoDelivery(deliver_id, detail_id);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverDetailMgr-->NoDelivery-->" + ex.Message, ex);
            }
        }
        public bool SplitDetail(string deliver_id, string detail_id)
        {
            try
            {
                return _IDeliverDetailDao.SplitDetail(deliver_id, detail_id);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverDetailMgr-->SplitDetail-->" + ex.Message, ex);
            }
        }
        public string Split(string deliver_id, string[] detail_ids)
        {
            try
            {
                return _IDeliverDetailDao.Split(deliver_id, detail_ids);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverDetailMgr-->Split-->" + ex.Message, ex);
            }

        }
        public string GetSmsId(Sms sms)
        {
            try
            {
                return _IDeliverDetailDao.GetSmsId(sms);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverDetailMgr-->GetSmsId-->" + ex.Message, ex);
            }

        }
        public int UpSmsTime(string deliver_id, string sms_date, string sms_id)
        {
            try
            {
                return _IDeliverDetailDao.UpSmsTime(deliver_id, sms_date, sms_id);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverDetailMgr-->UpSmsTime-->" + ex.Message, ex);
            }
        }
        public int DeliverMasterEdit(DeliverMaster dm, int type)
        {
            try
            {
                return _IDeliverDetailDao.DeliverMasterEdit(dm, type);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverDetailMgr-->DeliverMasterEdit-->" + ex.Message, ex);
            }
        }
        public object GetChannelOrderList(DeliverMasterQuery dmq, out int totalCount, int type = 0)
        {
            try
            {
                return _IDeliverDetailDao.GetChannelOrderList(dmq, out totalCount, type);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverDetailMgr-->GetChannelOrderList-->" + ex.Message, ex);
            }

        }
        public DataTable GetOrderDelivers(string deliver_id, int type = 0)
        {
            try
            {
                return _IDeliverDetailDao.GetOrderDelivers(deliver_id, type);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverDetailMgr-->GetOrderDelivers-->" + ex.Message, ex);
            }
        }
        public DataTable GetWayBills(string deliver_id,string ticketids)
        {
            try
            {
                return _IDeliverDetailDao.GetWayBills(deliver_id,ticketids);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverDetailMgr-->GetWayBills-->" + ex.Message, ex);
            }
        }
    }
}
