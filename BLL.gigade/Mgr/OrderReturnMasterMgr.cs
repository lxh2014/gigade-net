using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
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
    public class OrderReturnMasterMgr
    {
        private OrderReturnMasterDao _orderReturnMaster;
        private OrderDetailDao _orderDetailDao;
        private ISerialImplDao _serial;
        private MySqlDao _mySqlDao;
        public OrderReturnMasterMgr(string connectionStr)
        {
            _orderReturnMaster = new OrderReturnMasterDao(connectionStr);
            _serial = new SerialDao(connectionStr);
            _mySqlDao = new MySqlDao(connectionStr);
            _orderDetailDao = new OrderDetailDao(connectionStr);
        }

        public List<OrderReturnMasterQuery> GetReturnMaster(OrderReturnMasterQuery ormQuery, out int totalCount)
        {
            try
            {
                return _orderReturnMaster.GetReturnMaster(ormQuery, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterMgr.GetReturnMaster-->" + ex.Message, ex);
            }
        }
        public OrderReturnUserQuery GetReturnDetailById(uint order_id)
        {
            try
            {
                return _orderReturnMaster.GetReturnDetailById(order_id);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterMgr.GetReturnDetailById-->" + ex.Message, ex);
            }
        }
        public int Save(OrderReturnMasterQuery query)
        {
            try
            {
                return _orderReturnMaster.Save(query);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterMgr.Save-->" + ex.Message, ex);
            }
        }

        public string OrderReturn(OrderReturnMasterQuery query)
        { 
            string josn = "";
            List<OrderDetailQuery> orm = new List<OrderDetailQuery>();
            List<OrderDetail> od = new List<OrderDetail>();
            Serial s = new Serial();//流水號
            Serial s1 = new Serial();//流水號
            OrderReturnMaster m = new OrderReturnMaster();
            StringBuilder sb = new StringBuilder();
            ArrayList arrList = new ArrayList();
            string r_id="";
           uint slave=0;
            try
            {
                orm = _orderReturnMaster.GetOrderreturn(query);
                if (orm.Count == 0)
                {//1.判斷detail_id選中的個數,沒有則退出;  查詢該訂單詳細數據,取出需要的字段;
                    return josn = "{success:true,msg:2}";//
                }
                uint a = 1;
                foreach (var item in orm)
                {//同一出貨商的商品放在同一陣列中,成立獨立的退貨單                   
                    if (slave != item.Slave_Id)
                    { //    (1)區分出貨商order_slave.slave_id相同的添加到同一個退貨單中.
                        s =_serial.GetSerialById(45);//獲取退貨流水號
                        s.Serial_Value=s.Serial_Value+ a;
                        m.return_id = uint.Parse(s.Serial_Value.ToString());
                        r_id += m.return_id.ToString() + ",";
                        m.order_id = item.Order_Id;
                        m.vendor_id = item.Vendor_Id;
                        m.return_status = 0;
                        m.return_note = query.return_note;
                        m.return_ipfrom = query.return_ipfrom;
                        //新增退貨單信息
                        arrList.Add(_orderReturnMaster.InsertOrderReturnMaster(m));
                        arrList.Add(_serial.Update(45));//流水號+1
                        slave = item.Slave_Id;
                        //修改訂單的付款狀態
                        arrList.Add(_orderReturnMaster.UpdOrderMaster(item.Order_Id, "5"));
                        //新增訂單狀態變更記錄
                        s1 = _serial.GetSerialById(29);//獲取訂單主檔狀態流水號
                        s1.Serial_Value = s1.Serial_Value + a;
                        arrList.Add(_orderReturnMaster.InsertOrderMasterStatus(s1.Serial_Value, item.Order_Id, "91", "miaoshu"));
                        arrList.Add(_serial.Update(29));//流水號+1
                        a++;
                    }
                    //變更相關訂單細項商品狀態,并新增退貨單細項.判斷是否是組合商品并修改商品狀態.
                    arrList.Add(_orderReturnMaster.InsertOrderReturnDetail(s.Serial_Value.ToString(), item.Detail_Id.ToString()));

                    if (item.item_mode == 1)
                    {//變更組合商品內所有商品狀態,獲取時候是否獲取到整個組合商品
                        od = _orderReturnMaster.Getdetail(item.Slave_Id.ToString(), item.Parent_Id.ToString());
                        foreach (var item1 in od)
                        {
                            arrList.Add(_orderReturnMaster.UpdOrderDetail(item1.Detail_Id.ToString()));
                        }
                    }
                    else 
                    {//不是組合商品就只修改該商品的狀態
                        arrList.Add(_orderReturnMaster.UpdOrderDetail(item.Detail_Id.ToString()));
                    }
                }                
                //10000以上代表拋轉過資料，需重拋轉
                if (_orderReturnMaster.GetExportFlag(m.order_id.ToString())>0)
                {
                    arrList.Add(_orderReturnMaster.UpdExportFlag(m.order_id.ToString()));
                }
                r_id = r_id.Substring(0, r_id.Length - 1);
                //執行sql語句
                if ( _mySqlDao.ExcuteSqlsThrowException(arrList))
                {
                    josn = "{success:true,msg:0,return_id:'"+r_id+"'}";
                }
                else
                {
                    josn = "{success:true,msg:1}";
                }                
                return josn;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterMgr.OrderReturn-->" + ex.Message, ex);
            }
        }

        public string CancelReturnPurchaes(OrderReturnMasterQuery query)
        {
            string json = string.Empty;
            try
            {
                int ors_status=0;
                DataTable _dtStatus = _orderReturnMaster.OrsStatus(query.order_id);
                if (_dtStatus.Rows.Count == 0)
                {
                    ors_status = 0;
                }
                else
                {
                    if (_dtStatus.Rows[0][0].ToString() == "")
                    {
                        ors_status = 0;
                    }
                    else
                    {
                        ors_status = Convert.ToInt32(_dtStatus.Rows[0][0]);
                    }
                }
                if (ors_status == 0 && query.return_status == 0)
                {
                    OrderMasterStatusQuery statusquery = new OrderMasterStatusQuery();
                    OrderDetailQuery detailquery = new OrderDetailQuery();
                    ArrayList arrList = new ArrayList();
                    query.return_status = 2;
                    arrList.Add(_orderReturnMaster.UpOrderReturnMaster(query));
                    DataTable _dt = _orderReturnMaster.GetSerialID(29);
                    statusquery.serial_id = Convert.ToUInt64(_dt.Rows[0][0]);
                    statusquery.order_id = query.order_id;
                    statusquery.order_status = query.return_status;
                    statusquery.status_description = "Writer:(" + query.user_id + ")" + query.user_username + ",return_id:" + query.return_id + ",取消退貨請協助通知營管貨品確實出貨";
                    statusquery.status_ipfrom = query.return_ipfrom;
                    arrList.Add(_orderReturnMaster.InsertOrderMasterS(statusquery));

                    List<OrderDetailQuery> odli = _orderDetailDao.OrderDetail(query.return_id);
                    foreach (var item in odli)
                    {
                        detailquery.Slave_Id = item.Slave_Id;
                        detailquery.Parent_Id = item.Parent_Id;
                        detailquery.pack_id = item.pack_id;
                        detailquery.Detail_Id = item.Detail_Id;
                        if (item.item_mode == 1)
                        {
                            arrList.Add(_orderDetailDao.UpdateOrderDetailSome(detailquery));
                        }
                        else
                        {
                            arrList.Add(_orderDetailDao.UpdateOrderDetail(detailquery));
                        }
                    }
                    if (_mySqlDao.ExcuteSqlsThrowException(arrList))
                    {
                        json = "{success:true}";
                    }
                    else
                    {
                        json = "{success:false}";
                    }
                }
                else
                {
                    json = "{success:false,msg:'1'}";
                }
                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterMgr.CancelReturnPurchaes-->" + ex.Message, ex);
            }
        }
    }
}
//if (Request.Params["return_status"] == "1")// 若歸檔,判斷是否成立退款單
// {
//     _orderMasterMgr = new OrderMasterMgr(mySqlConnectionString);
//     OrderMaster om = _orderMasterMgr.GetPaymentById(query.order_id);
//     //order_id,order_payment,user_id,order_amount,order_date_pay
//     //查詢退貨內容
//     _ordertailMgr = new OrderDetailMgr(mySqlConnectionString);
//     OrderDetail od = _ordertailMgr.QueryReturnDetail(query.return_id);
//     if (od != null)
//     {
//         // 退款金額 = 商品購買金額 - 購物金
//         uint paymoney = od.Single_Money * od.Buy_Num;
//         uint returnmoney = paymoney - (od.Deduct_Bonus + od.Deduct_Welfare + od.Deduct_Happygo);//計算應退還的money
//         // 判斷是否有付款
//         if (om.Money_Collect_Date > 0 && om.Order_Amount > 0 && returnmoney > 0)
//         {
//             //退款方式
//             if (om.Order_Payment == 1 || om.Order_Payment == 2 || om.Order_Payment == 13 || om.Order_Payment == 16)
//             {
//                 uint moneytype = om.Order_Payment;
//                 //添加數據島order_money_return
//                 //                                'money_id'		=> (int) $nMoney_Id,
//                 //'order_id'		=> (int) $nOrder_Id,
//                 //'money_type'		=> (int) $nMoney_Type,
//                 //'money_total'		=> (int) $nReturn_Money,
//                 //'money_status'		=> (int) 0,
//                 //'money_note'		=> (string) '',
//                 //'bank_Note'			=> (string) $sBank_Note,
//                 //'bank_name' 		=> (string) $sBank_Name ,
//                 //'bank_branch' 	    => (string) $sBank_Branch ,
//                 //'bank_account' 	 	=> (string) $sBank_Account ,
//                 //'account_name' 	 	=> (string) $sAccount_Name , 
//                 //'money_source'		=> (string) ('return_id:' . $nReturn_Id),
//                 //'money_createdate'	=> (int) $nReturn_Updatedate,
//                 //'money_updatedate'	=> (int) $nReturn_Updatedate,
//                 //'money_ipfrom'		=> (string) $sUser_IP
//             }
//         }

//     }
// }