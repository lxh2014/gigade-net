using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
  public  class OrderReturnStatusMgr
    {
      private OrderReturnStatusDao _orderReturnStatus;
      private MySqlDao _mysql;
      public OrderReturnStatusMgr(string connectionString)
      {
          _orderReturnStatus = new OrderReturnStatusDao(connectionString);
          _mysql = new MySqlDao(connectionString);
      }

      public string CheckOrderId(OrderReturnStatusQuery query)
      {
          string json = string.Empty;
          List<OrderReturnStatusQuery> store = new List<OrderReturnStatusQuery>();
          try
          {
            
            //DataTable _masterDt = _orderReturnStatus.OrderIdIsExist(Convert.ToUInt32(query.ors_order_id));
            query.orc_order_id = query.ors_order_id;
            if (query.ors_order_id!=0)//有此編號
            {
                //判斷訂單狀態
                DataTable _dt = _orderReturnStatus.CheckOrderId(query);
                DataTable _tranDt = _orderReturnStatus.CheckTransport(query);
                int totalCount = 0;
                query.IsPage = false;
                store = _orderReturnStatus.CouldGridList(query, out   totalCount);

                if (_dt != null && _dt.Rows.Count > 0)
                {
                    if (Convert.ToInt32(_dt.Rows[0][0]) == 2 || Convert.ToInt32(_dt.Rows[0][0]) == 3 || Convert.ToInt32(_dt.Rows[0][0]) == 4)
                    {
                        return json = "{success:true,status:'" + Convert.ToInt32(_dt.Rows[0]["ors_status"]) + "',order_payment:'" + store[0].order_payment + "',bank_name:'" + store[0].bank_name + "',bank_branch:'" + store[0].bank_branch + "',bank_account:'" + store[0].bank_account + "',account_name:'" + store[0].account_name + "',bank_note:'" + store[0].bank_note + "' }";
                    }

                    if (_tranDt==null ||  _tranDt.Rows[0][0].ToString() == "")
                    {
                        json = "{success:true,status:'0.5'}";
                    }
                    else
                    {
                         if (Convert.ToInt32(_dt.Rows[0]["ors_status"])==2)//此時需要點擊確認入庫
                        {
                         
                            json = "{success:true,status:'" + Convert.ToInt32(_dt.Rows[0]["ors_status"]) + "'}";
                        }
                        else
                        {
                            json = "{success:true,status:'" + Convert.ToInt32(_dt.Rows[0]["ors_status"]) + "'}";
                        }
                    }
                }
                else
                {
                    OrderMaster om = _orderReturnStatus.GetOrderInfo(Convert.ToUInt32(query.ors_order_id));
                    string delivery_name = "***";
                    string delivery_mobile = "***";
                    string delivery_address = "***";
                    if (!string.IsNullOrEmpty(om.Delivery_Name))
                    {
                        delivery_name = om.Delivery_Name.Substring(0, 1) + "**";
                    }
                    if (!string.IsNullOrEmpty(om.Delivery_Mobile))
                    {
                        if (om.Delivery_Mobile.Length > 3)
                        {
                            delivery_mobile = om.Delivery_Mobile.Substring(0, 3) + "**";
                        }
                        else
                        {
                            delivery_mobile = om.Delivery_Mobile + "***";
                        }
                    }
                    if (!string.IsNullOrEmpty(om.Delivery_Address))
                    {
                        if (om.Delivery_Address.Length > 3)
                        {
                            delivery_address = om.Delivery_Address.Substring(0, 3) + "**";
                        }
                        else
                        {
                            delivery_address = om.Delivery_Address + "***";
                        }
                    }
                    json = "{success:true,name:'" + delivery_name + "',mobile:'" + delivery_mobile + "',address:'" + delivery_address + "',zipcode:'" + om.Delivery_Zip + "',status:'0'}";
                }
            }
            else//無此訂單編號
            {
                json = "{success:true,status:'-1'}";
            }
            return json;
          }
          catch (Exception ex)
          {
              throw new Exception("OrderReturnStatusMgr-->CheckOrderId-->"+ex.Message,ex);
          }
      }

      public string SaveOrderReturn(OrderReturnStatusQuery query)
      {
          string json = string.Empty;
          try
          {
              ArrayList arrList = new ArrayList();
              bool zichu = false;
              query.ors_createdate = DateTime.Now;
              if (query.orc_name.IndexOf("*")>0)
              {
                  OrderMaster om = _orderReturnStatus.GetOrderInfo(Convert.ToUInt32(query.orc_order_id));
                  query.orc_name = om.Delivery_Name;
                  query.orc_phone = om.Delivery_Mobile;
                  query.orc_address = om.Delivery_Address;
              }
              arrList.Add(_orderReturnStatus.InsertOrderReturnContent(query));
              arrList.Add(_orderReturnStatus.InsertOrderReturnStatus(query));
              arrList.Add(_orderReturnStatus.UpdateORM(query));
              //產看要退的商品是不是都是自出
              List<OrderReturnStatusQuery> store = new List<OrderReturnStatusQuery>();
              int totalCount = 0;
              query.IsPage = false;
              store = _orderReturnStatus.CouldGridList(query, out   totalCount);
              int modeCount = 0;
              foreach (var item in store)
              {
                  if (item.product_mode == "自出")
                  {
                      modeCount++;
                  }
              }
              if (modeCount == store.Count)
              {
                  //全是自出
                  zichu = true;
                  query.ors_status = 3;
                  arrList.Add(_orderReturnStatus.CouldReturn(query));
              }
              else
              {
                  if (query.orc_send == 0)//未發貨，寄倉/調度則在客戶信息填完之後，就確認入庫
                  {
                      query.ors_status = 2;
                      arrList.Add(_orderReturnStatus.CouldReturn(query));
                  }
              }

              if (_orderReturnStatus.ExecSqls(arrList))
              {
                  if (zichu)
                  {
                      json = "{success:true,isZIChu:'1',}";
                  }
                  else
                  {
                      json = "{success:true}";
                  }
              }
              else
              {
                  json = "{success:false}";
              }
              return json;
          }
          catch (Exception ex)
          {
              throw new Exception("OrderReturnStatusMgr-->SaveOrderReturn-->" + ex.Message, ex);
          }
      }

      public List<OrderReturnStatusQuery> CouldGridList(OrderReturnStatusQuery query, out int totalCount)
      {
          try
          {
             return _orderReturnStatus.CouldGridList(query, out totalCount);
          }
          catch (Exception ex)
          {
              throw new Exception("OrderReturnStatusMgr-->CouldGridList-->" + ex.Message, ex);
          }

      }

      public string CouldReturn(OrderReturnStatusQuery query)
      {
          string json = string.Empty;
         // List<OrderReturnStatusQuery> store = new List<OrderReturnStatusQuery>();
          ArrayList arrList = new ArrayList();
          try
          {
              //int totalCount = 0;
              //query.IsPage = false;
              //store= _orderReturnStatus.CouldGridList(query, out   totalCount);
            
             //   int modeCount = 0;
              //foreach (var item in store)
              //{
              //    if (item.product_mode =="自出")
              //    {
              //        modeCount++;
              //    }
              //}
              //if (modeCount ==store.Count)
              //{
              //    //全是自出
              //    query.ors_status = 4;
              //    arrList.Add(_orderReturnStatus.CouldReturn(query));
              //}
                  //不全是自出
              arrList.Add(_orderReturnStatus.UpdateORM(query));
              arrList.Add(_orderReturnStatus.CouldReturn(query));
              if (_mysql.ExcuteSqlsThrowException(arrList))
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
              throw new Exception("OrderReturnStatusMgr-->CouldReturn-->" + ex.Message, ex);
          }
      }

      public string InsertTransport(OrderReturnStatusQuery query)
      {
          string json = string.Empty;
          try
          {
              ArrayList arrList = new ArrayList();
              arrList.Add(_orderReturnStatus.InsertTransport(query));
              arrList.Add(_orderReturnStatus.UpdateORS(query));//由於邏輯變更，此處為想order_return_status表裏插入數據
              if (_mysql.ExcuteSqlsThrowException(arrList))
              {
                  json = "{success:'true',msg:'1'}";
              }
              else
              {
                  json = "{success:'false'}";
              }
              return json;
          }
          catch (Exception ex)
          {
              throw new Exception("OrderReturnStatusMgr-->InsertTransport-->" + ex.Message, ex);
 
          }
      }

      public string CouldReturnMoney(OrderReturnStatusQuery query)
      {
          string json = string.Empty;
          try
          {
              if (_orderReturnStatus.CouldReturnMoney(query))
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
              throw new Exception("OrderReturnStatusMgr-->CouldReturnMoney-->" + ex.Message, ex);
          }
      }

      public DataTable GetOrcTypeStore()
      {
          try
          {
              return _orderReturnStatus.GetOrcTypeStore();
          }
          catch (Exception ex)
          {
              throw new Exception("OrderReturnStatusMgr-->GetOrcTypeStore-->" + ex.Message, ex);
          }
      }

      public DataTable GetInvoiceDealStore()
      {
          try
          {
              return _orderReturnStatus.GetInvoiceDealStore();
          }
          catch (Exception ex)
          {
              throw new Exception("OrderReturnStatusMgr-->GetInvoiceDealStore-->" + ex.Message, ex);
          }
      }

      public DataTable GetPackageStore()
      {
          try
          {
              return _orderReturnStatus.GetPackageStore();
          }
          catch (Exception ex)
          {
              throw new Exception("OrderReturnStatusMgr-->GetInvoiceDealStore-->" + ex.Message, ex);
          }
      }

      public int GetOrderIdByReturnId(uint return_id)
      {
          int order_id=0;
          try
          {
              DataTable _dt=_orderReturnStatus.GetOrderIdByReturnId(return_id);
              if(_dt.Rows.Count>0&&_dt!=null)
              {
                  order_id=Convert.ToInt32(_dt.Rows[0][0]);
              }
              return order_id;
          }
          catch (Exception ex)
          {
              throw new Exception("OrderReturnStatusDao-->GetOrderIdByReturnId-->" + ex.Message, ex);
          }
      }

      //確認入庫，更新庫存
      public string PlaceOnFile(OrderReturnStatusQuery query)
      {
          string json = string.Empty;
          try
          {
              if (_orderReturnStatus.PlaceOnFile(query))
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
              throw new Exception("OrderReturnStatusDao-->PlaceOnFile-->" + ex.Message, ex);
          }
      }

      public OrderMaster GetOrderInfo(uint return_id)
      {
          try
          {
              int order_id = GetOrderIdByReturnId(return_id);
              OrderMaster om = _orderReturnStatus.GetOrderInfo(Convert.ToUInt32(order_id));
              return om;
          }
          catch (Exception ex)
          {
              throw new Exception("OrderReturnStatusDao-->GetOrderInfo-->" + ex.Message, ex);
          }
      }
    }
}
