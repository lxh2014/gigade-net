using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using System.Collections;

namespace BLL.gigade.Mgr
{
   public class EmsDepRelationMgr
    {
       private EmsDepRelationDao emsDepRe;

       public EmsDepRelationMgr(string connectionString)
       {
           emsDepRe = new EmsDepRelationDao(connectionString);
       }

       public List<EmsDepRelation> EmsDepRelationList(EmsDepRelation query, out int totalCount)
       {
           try
           {
               insertPreDate(query);
               return emsDepRe.EmsDepRelationList(query, out totalCount);
           }
           catch (Exception ex)
           {
               throw new Exception("EmsDepRelationMgr-->EmsDepRelationList-->"+ex.Message);
           }
       }

       public bool insertPreDate(EmsDepRelation query)
       {
           StringBuilder sqlInsert = new StringBuilder();
           try
           {
               List<EmsDepRelation> store = emsDepRe.GetDepStore();
               ArrayList arrList = new ArrayList();
               for (int i = 1; i <= query.predate.Day; i++)
               {
                   for (int j = 0; j < store.Count; j++)
                   {
                       query.insert_day = i;
                       query.dep_code_insert = store[j].dep_code;
                       query.relation_type_insert = 1;//公關單
                       if (emsDepRe.IsPRSingleExist(query) == 0)
                       {
                           arrList.Add(emsDepRe.insertSql(query));
                       }
                       query.relation_type_insert = 2;//報廢單
                       if (emsDepRe.IsPRSingleExist(query) == 0)
                       {
                           arrList.Add(emsDepRe.insertSql(query));
                       }
                   }
               }
               return emsDepRe.execSql(arrList);
           }
           catch (Exception ex)
           {
               throw new Exception("EmsDepRelationMgr-->insertPreDate-->" + ex.Message);
           }
     
       }

       public List<EmsDepRelation> GetDepStore()
       {
           try
           {
               return emsDepRe.GetDepStore();
           }
           catch (Exception ex)
           {
               throw new Exception("EmsDepRelationMgr-->GetDepStore-->"+ex.Message);
           }
       }
       public string EditEmsDepR(EmsDepRelation query)
       {
           string json = string.Empty;
           try
           {
               if (query.emsdep == "relation_order_count")//訂單筆數)
               {
                   if (emsDepRe.RelationOrderCount(query) > 0)
                   {
                       json = "{success:true}";
                   }
                   else
                   {
                       json = "{success:false}";
                   }

               }
               else if (query.emsdep == "relation_order_cost")//訂單成本
               {
                   if (emsDepRe.RelationOrderCost(query) > 0)
                   {
                       json = "{success:true}";
                   }
                   else
                   {
                       json = "{success:false}";
                   }
               }
               return json;
           }
           catch (Exception ex)
           {
               throw new Exception("EmsDepRelationMgr-->EditEmsDepR-->" + ex.Message, ex);
           }
       }
       public int VerifyData(EmsDepRelation query)
       {
           try
           {
               return emsDepRe.VerifyData(query);
           }
           catch (Exception ex)
           {
               throw new Exception("EmsDepRelationMgr-->VerifyData-->" + ex.Message, ex);
           }
       }
       public string SaveEmsDepRe(EmsDepRelation query)
       {
           string json = string.Empty;
           try
           {
               string time = query.relation_year + "-" + query.relation_month + "-" + query.relation_day;
               DateTime dtime = DateTime.Now;
               DateTime sTime = DateTime.Now;
               bool isTime = DateTime.TryParse(time, out dtime);//是否是正常時間 是 true 否  false
               if (!isTime)//不是正常時間
               {
                   json = "{success:true,msg:3}";//選擇時間有誤
               }
               else//如果是正常時間判斷是否大於當前時間
               {
                   if (emsDepRe.VerifyData(query) > 0)
                   {
                       json = "{success:true,msg:0}";//不可添加數據
                   }
                   else
                   {
                       if (emsDepRe.SaveEmsDepRe(query) > 0)
                       {
                           json = "{success:true,msg:1}";//新增成功
                       }
                       else
                       {
                           json = "{success:true,msg:2}";//新增失敗
                       }
                   }
               }
               return json;
           }
           catch (Exception ex)
           {
               throw new Exception("EmsDepRelationMgr-->SaveEmsDepRe-->" + ex.Message, ex);
           }
       }
    }
}
