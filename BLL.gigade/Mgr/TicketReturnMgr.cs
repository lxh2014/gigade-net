using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class TicketReturnMgr
    {
        private TicketReturnDao _ticketReturn;
        public TicketReturnMgr(string connectionString)
        {
            _ticketReturn = new TicketReturnDao(connectionString);
        }
        public List<TicketReturnQuery> GetTicketReturnList(TicketReturnQuery query, out int totalCount)
        {
            try
            {
                return _ticketReturn.GetTicketReturnList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("TicketReturnMgr-->GetTicketReturnList-->"+ex.Message,ex);
            }
        }

        public string SaveTicketReturn(TicketReturnQuery query)
        {
            string json = string.Empty;
            try
            {
                 ArrayList arrList = new ArrayList();
                 arrList.Add( _ticketReturn.SaveTicketReturn(query));
                 arrList.Add(_ticketReturn.SaveTicketReturnLog(query));
                 if (_ticketReturn.ExecTicketReturnSql(arrList))
                 {
                     json = "{success:true}";
                 }
                 else
                 {
                     json = "{success:false}";
                 }
                 return json;
            }
            catch (Exception ex)
            {
                throw new Exception("TicketReturnMgr-->SaveTicketReturn-->" + ex.Message, ex);
            }
        }

        public List<TicketReturnQuery> GetReasonTypeStore()
        {
            try
            {
                return _ticketReturn.GetReasonTypeStore();
            }
            catch (Exception ex)
            {
                throw new Exception("TicketReturnMgr-->GetReasonTypeStore-->" + ex.Message, ex);
            }
        }

    } 
}
