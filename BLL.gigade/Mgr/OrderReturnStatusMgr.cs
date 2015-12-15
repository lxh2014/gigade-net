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
      private OrderMasterDao _orderMaster;
      private MySqlDao _mysql;
      public OrderReturnStatusMgr(string connectionString)
      {
          _orderReturnStatus = new OrderReturnStatusDao(connectionString);
          _orderMaster = new OrderMasterDao(connectionString);
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
              throw new Exception("OrderReturnStatusMgr-->GetOrderInfo-->" + ex.Message, ex);
          }
      }
      /// <summary>
      /// 普通賦值
      /// </summary>
      /// <param name="dtHZ"></param>
      /// <param name="_dt"></param>
      /// <param name="i"></param>
      /// <returns></returns>
      public DataRow GetOrdinaryData(DataTable dtHZ, DataTable _dt,int i)
      {
          string total_amount = string.Empty;
          string invoice_number = string.Empty;
          string invoice_date = string.Empty;
          DataRow row = dtHZ.NewRow();
          try
          {
              #region

              row["會員姓名"] = _dt.Rows[i]["order_name"];
              row["購買時間"] = Convert.ToDateTime(_dt.Rows[i]["order_createdate"]).ToString("yyyy-MM-dd HH:mm:ss");
              row["付款單號"] = _dt.Rows[i]["order_id"];
              DataTable invoiceDt = _orderMaster.GetInvoiceData(Convert.ToUInt32(_dt.Rows[i]["order_id"])); //根據order_id 進行查詢
              if (invoiceDt != null && invoiceDt.Rows.Count > 0)
              {
                  row["發票金額"] = invoiceDt.Rows[0]["total_amount"];
              }
              else
              {
                  row["發票金額"] = "";
              }
              row["發票開立日期"] = "1970-01-01 08:00:00";
              row["發票號碼"] = "0";
              row["付款方式"] = _dt.Rows[i]["order_payment"];
              row["購買金額"] = _dt.Rows[i]["order_amount"];
              row["付款狀態"] = _dt.Rows[i]["order_status"];
              row["商品細項編號"] = _dt.Rows[i]["item_id"];
              row["訂單狀態"] = _dt.Rows[i]["detail_status"];

              row["供應商"] = _dt.Rows[i]["vendor_name_simple"];
              row["供應商編碼"] = _dt.Rows[i]["vendor_code"];
              row["品名"] = _dt.Rows[i]["product_name"];
              row["數量"] = _dt.Rows[i]["buy_num"];
              row["購買單價"] = _dt.Rows[i]["single_money"];
              row["折抵購物金"] = _dt.Rows[i]["deduct_bonus"];
              row["抵用卷"] = _dt.Rows[i]["deduct_welfare"];
              row["總價"] = (Convert.ToInt32(_dt.Rows[i]["total_money"]) - (Convert.ToInt32(_dt.Rows[i]["deduct_bonus"]) + Convert.ToInt32(_dt.Rows[i]["deduct_welfare"]))).ToString();
              row["成本單價"] = _dt.Rows[i]["single_cost"];
              row["寄倉費"] = _dt.Rows[i]["bag_check_money"];
              row["成本總額"] = (Convert.ToInt32(_dt.Rows[i]["total_cost"]) - (Convert.ToInt32(_dt.Rows[i]["bag_check_money"])));
              if (Convert.ToDateTime(_dt.Rows[i]["slave_date_close"]).ToString("yyyy-MM-dd HH:mm:ss") == "1970-01-01 08:00:00")
              {
                  row["出貨單歸檔期"] = "未歸檔";
              }
              else
              {
                  row["出貨單歸檔期"] = _dt.Rows[i]["slave_date_close"];
              }
              row["負責PM"] = _dt.Rows[i]["pm"];
              row["來源ID"] = _dt.Rows[i]["ID"];
              row["來源名稱"] = _dt.Rows[i]["redirect_name"];
              row["出貨方式"] = _dt.Rows[i]["product_mode"];
              
              #endregion
          }
          catch (Exception ex)
          {
              throw new Exception("OrderReturnStatusMgr-->GetOrdinaryData-->" + ex.Message, ex);
          }
          
          return row;
      }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dtHZ"></param>
    /// <param name="_dt"></param>
    /// <param name="_dtFreight"></param>
    /// <param name="i"></param>
    /// <param name="type">1常溫 2低溫</param>
    /// <returns></returns>
      public DataRow GetFreightData(DataTable dtHZ, DataTable _dt, DataTable _dtFreight, int i,int type)
      {
          DataRow row = dtHZ.NewRow();
          try
          {
              row["會員姓名"] = _dt.Rows[i]["order_name"];
              row["購買時間"] = Convert.ToDateTime(_dt.Rows[i]["order_createdate"]).ToString("yyyy-MM-dd HH:mm:ss");
              row["付款單號"] = _dt.Rows[i]["order_id"];
              row["付款方式"] = _dt.Rows[i]["order_payment"];
              row["購買金額"] = _dt.Rows[i]["order_amount"];
              row["付款狀態"] = _dt.Rows[i]["order_status"];
              DataTable invoiceDt = _orderMaster.GetInvoiceData(Convert.ToUInt32(_dt.Rows[i]["order_id"])); //根據order_id 進行查詢
              if (invoiceDt != null && invoiceDt.Rows.Count > 0)
              {
                  row["發票金額"] = invoiceDt.Rows[0]["total_amount"];
              }
              else
              {
                  row["發票金額"] = "";
              }
              row["發票開立日期"] = "1970-01-01 08:00:00";
              row["發票號碼"] = "0";
              if (type == 1)
              {
                  row["商品細項編號"] = "G00001";
                  row["品名"] = "常溫運費";
                  row["購買單價"] = _dtFreight.Rows[0][0];
                  row["總價"] = _dtFreight.Rows[0][0];
              }
              else
              {
                  row["商品細項編號"] = "G00002";
                  row["品名"] = "低溫運費";
                  row["購買單價"] = _dtFreight.Rows[0][1];
                  row["總價"] = _dtFreight.Rows[0][1];
              }
              row["訂單狀態"] = "";
              row["供應商"] = "";
              row["供應商編碼"] = "";
            
              row["數量"] = "1";
             
              row["折抵購物金"] = "";
              row["抵用卷"] = "";
              row["成本單價"] = "";
              row["寄倉費"] = "";
              row["成本總額"] = "";
              if (Convert.ToDateTime(_dt.Rows[i]["slave_date_close"]).ToString("yyyy-MM-dd HH:mm:ss") == "1970-01-01 08:00:00")
              {
                  row["出貨單歸檔期"] = "未歸檔";
              }
              else
              {
                  row["出貨單歸檔期"] = _dt.Rows[i]["slave_date_close"];
              }
              row["負責PM"] = "";
              row["來源ID"] = "";
              row["來源名稱"] = "";
              row["出貨方式"] = "";
          }
          catch (Exception ex)
          {
              throw new Exception("OrderReturnStatusMgr-->GetFreightData-->" + ex.Message, ex);
          }
          return row;
      }
    }
}
