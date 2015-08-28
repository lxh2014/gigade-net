using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    
     public  class EventChinaTrustBagMgr
    {
         private EventChinaTrustBagDao _eventChinaTrustBag;

         public EventChinaTrustBagMgr(string connectionString)
         {
             _eventChinaTrustBag = new EventChinaTrustBagDao(connectionString);
         }

         public List<EventChinaTrustBagQuery> EventChinaTrustBagList(EventChinaTrustBagQuery query, out int totalCount)
         {
             try
             {
                 return _eventChinaTrustBag.EventChinaTrustBagList(query, out totalCount);
             }
             catch (Exception ex)
             {
                 throw new Exception("EventChinaTrustBagMgr-->EventChinaTrustBagList-->"+ex.Message);
             }

         }

         public int EventChinaTrustBagSave(EventChinaTrustBagQuery query)
         {
             try
             {
                 return _eventChinaTrustBag.EventChinaTrustBagSave(query);
             }
             catch (Exception ex)
             {
                 throw new Exception("EventChinaTrustBagMgr-->EventChinaTrustBagSave-->" + ex.Message);
             }
         }

         public EventChinaTrustBagQuery GetSinggleData(EventChinaTrustBagQuery query)
         {
             try
             {
                 return _eventChinaTrustBag.GetSinggleData(query);
             }
             catch (Exception ex)
             {
                 throw new Exception("EventChinaTrustBagMgr-->GetSinggleData-->" + ex.Message);
             }
         }
         public int EventChinaTrustBagStatus(EventChinaTrustBagQuery query)
         {
             try
             {
                 return _eventChinaTrustBag.EventChinaTrustBagStatus(query);
             }
             catch (Exception ex)
             {
                 throw new Exception("EventChinaTrustBagMgr-->EventChinaTrustBagStatus-->" + ex.Message);
             }
         }

         public List<EventChinaTrustBag> GetBag(EventChinaTrustBagQuery q)
         {
             try
             {
                 return _eventChinaTrustBag.GetBag(q);
             }
             catch (Exception ex)
             {
                 throw new Exception("EventChinaTrustBagMgr-->GetBag-->" + ex.Message);
             }
         }
    }
}
