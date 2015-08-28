using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using System.Data;
namespace BLL.gigade.Mgr
{
    public class TicketMgr : ITicketImplMgr
    {
        private ITicketImplDao _ticketDao;
        public TicketMgr(string connectionString)
        {
            _ticketDao = new TicketDao(connectionString);
        }
        public List<Model.Query.TicketQuery> GetTicketList(Model.Query.TicketQuery tqQuery, out int totalCount, string condition)
        {
            try
            {
                return _ticketDao.GetTicketList(tqQuery, out totalCount, condition);
            }
            catch (Exception ex)
            {
                throw new Exception("TicketMgr-->GetTicketList-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 匯出揀貨單PDF
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<TicketQuery> GetPickingDetail(TicketQuery query)
        {
            try
            {
                return _ticketDao.GetPickingDetail(query);
            }
            catch (Exception ex)
            {

                throw new Exception("TicketMgr-->GetPickingDetail-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 匯出出貨明細PDF
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<TicketQuery> GetTicketDetail(TicketQuery query)
        {
            try
            {
                return _ticketDao.GetTicketDetail(query);
            }
            catch (Exception ex)
            {

                throw new Exception("TicketMgr-->GetTicketDetail-->" + ex.Message, ex);
            }
        }
        public DataTable GetOrderDelivers(TicketQuery query)
        {
            try
            {
                return _ticketDao.GetOrderDelivers(query);
            }
            catch (Exception ex)
            {

                throw new Exception("TicketMgr-->GetOrderDelivers-->" + ex.Message, ex);
            }
        }


        public int UpdateTicketStatus(TicketQuery query)
        {
            try
            {
                return _ticketDao.UpdateTicketStatus(query);
            }
            catch (Exception ex)
            {

                throw new Exception("TicketMgr-->UpdateTicketStatus-->" + ex.Message, ex);
            }
        }
    }
}
