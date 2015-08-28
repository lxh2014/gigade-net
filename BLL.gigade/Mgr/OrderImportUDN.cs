///* 
// * Copyright (c) 2013，武漢聯綿信息技術有限公司
// * All rights reserved. 
// *  
// * 文件名称：OrderImportUDN 
// * 摘   要： 
// *  
// * 当前版本：1.0 
// * 作   者：lhInc 
// * 完成日期：2013/9/3 17:10:27 
// * 
// */

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using BLL.gigade.Mgr.Impl;
//using BLL.gigade.Common;
//using BLL.gigade.Model;
//using BLL.gigade.Model.Custom;

//namespace BLL.gigade.Mgr
//{
//    public class OrderImportUDN : IOrderImport
//    {
//        private OrderImportMgr orderImportMgr;
//        private IChannelImplMgr channelMgr;

//        public int ChannelID { get; set; }
//        public string MySqlConnStr { get; set; }
//        public string SqlServerConnStr { get; set; }
//        public OrderImportUDN()
//        { 
            
//        }

//        #region IOrderImport 成员


//        public List<OrdersImport> ReadExcel2Page(string filePath)
//        {
//            if (!System.IO.File.Exists(filePath))
//            {
//                return null;
//            }
//            orderImportMgr = new OrderImportMgr(MySqlConnStr, ChannelID);

//            var tmp = ValidateOrders(orderImportMgr.ReadExcel<OrdersImport>(filePath));

//            List<OrdersImport> rtnOrder = new List<OrdersImport>();

//            #region 數據分析

//            foreach (var item in tmp)
//            {
//                if (item.dsr == "物流服務費")
//                {
//                    rtnOrder.Add(item);
//                    continue;
//                }
//                bool oResult = orderImportMgr.IsExistsOrder(item);
//                if (oResult)
//                {
//                    item.Msg = Resource.CoreMessage.GetResource("ORDER_EXISTS");
//                    rtnOrder.Add(item);
//                    continue;
//                }

//                ProductItemMapCustom pCustom = orderImportMgr.QueryProductMapping(item);
//                if (pCustom == null)
//                {
//                    item.Msg = Resource.CoreMessage.GetResource("PRODUCT_MAP_NOT_EXISTS");
//                    rtnOrder.Add(item);
//                    continue;
//                }

//                ProductItem proItem = orderImportMgr.QueryProductItem(Convert.ToUInt32(pCustom.item_id));
//                if (proItem == null)
//                {
//                    item.Msg = Resource.CoreMessage.GetResource("ITEMID_ID_NOT_EXISTS");
//                }

//                if (orderImportMgr.QueryProduct(proItem.Product_Id) == null)
//                {
//                    item.Msg = Resource.CoreMessage.GetResource("PRODUCT_NOT_EXISTS");
//                    rtnOrder.Add(item);
//                    continue;
//                }
//                rtnOrder.Add(item);
//            }
//            #endregion

//            return rtnOrder;
//        }

//        public List<OrdersImport> ValidateOrders(List<OrdersImport> orders)
//        {
//            var result = orders.Where(m => DataCheck.IsNumeric(m.qty));
//            result = result.Where(m => DataCheck.IsDate(m.prndldat));
//            result = result.Where(m => DataCheck.IsDate(m.orddat));
//            result = result.Where(m => DataCheck.IsNumeric(m.agpesadrzip));
//            return result.ToList();
//        }

//        public int Import2DB(List<OrdersImport> all, string pdfFile, string importType, string chkRecord, ref int totalCount)
//        {
//            int successCount = 0;
//            orderImportMgr = new OrderImportMgr(MySqlConnStr, ChannelID);
//            orderImportMgr.SlaveNote = "udn子單:";
//            try
//            {
//                List<OrdersImport> chks = orderImportMgr.SplitChkData(chkRecord);
//                if (chks != null && all != null && all.Count > 0)
//                {
//                    #region 篩選出選中數據資訊

//                    //所有選中商品
//                    var result = from h in chks
//                                 join o in all on new { h.dmtshxuid, h.chlitpdno } equals new { o.dmtshxuid, o.chlitpdno } into cs
//                                 from c in cs.DefaultIfEmpty()
//                                 select c;
//                    #endregion

//                    //所有訂單編號
//                    List<string> master = chks.GroupBy(m => m.dmtshxuid).Select(m => m.Key).ToList();
//                    totalCount = master == null ? 0 : master.Count;
//                    foreach (var om in master)
//                    {
//                        //單個訂單下所有商品
//                        var product = (from p in result
//                                       where p.dmtshxuid == om
//                                       select p).ToList();

//                        all.FindAll(m => m.dmtshxuid == om && product.Exists(n => n.chlitpdno == m.chlitpdno)).ForEach(m => m.IsSel = true);

//                        #region 生成OrderSlave

//                        List<OrderSlave> slaves = orderImportMgr.FillSlave(product, all);
//                        //訂單內商品有問題（商品對照不存在，商品不存在，商品庫存不足）,跳過處理下筆訂單
//                        if (slaves == null || slaves.Count == 0)
//                        {
//                            continue;
//                        }
//                        #endregion

//                        #region OrderMaster

//                        OrderMaster newMaster = new OrderMaster();

//                        #region 其他信息

//                        newMaster.Order_Status = 2;
//                        newMaster.OrderId = uint.Parse(orderImportMgr.NextOrderId());
//                        channelMgr = new ChannelMgr(MySqlConnStr);
//                        newMaster.UserId = Convert.ToUInt32(channelMgr.GetUserIdByChannelId(ChannelID));

//                        newMaster.Order_Name = product.FirstOrDefault().ordpesnm;

//                        newMaster.Note_Order = "購買備註:" + product.FirstOrDefault().xrem + ",配送備註:" + product.FirstOrDefault().shipxrem;
//                        newMaster.Note_Admin = "於" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "建立";
//                        newMaster.Order_Date_Pay = Convert.ToUInt32(CommonFunction.GetPHPTime());
//                        newMaster.Channel = Convert.ToUInt32(ChannelID);
//                        newMaster.Channel_Order_Id = product.FirstOrDefault().dmtshxuid;
//                        newMaster.Order_Createdate = Convert.ToUInt32(CommonFunction.GetPHPTime());
//                        newMaster.Order_Zip = Convert.ToUInt32(product.FirstOrDefault().agpesadrzip);
//                        #endregion

//                        #region 物流方式

//                        switch (product.FirstOrDefault().shipco)
//                        {
//                            case "貨運 / 宅配":
//                            case "低溫配送":
//                                newMaster.Delivery_Store = 1;
//                                break;
//                            case "7-11":
//                                newMaster.Delivery_Store = 11;
//                                break;
//                            default:
//                                newMaster.Delivery_Store = 1;
//                                break;
//                        }
//                        #endregion

//                        #region 運費

//                        var freight = product.Where(m => m.dsr == "物流服務費").FirstOrDefault();
//                        if (freight != null)
//                        {
//                            if (freight.shipco == "低溫運送")
//                            {
//                                newMaster.Order_Freight_Low = uint.Parse(freight.sumup);
//                            }
//                            else
//                            {
//                                newMaster.Order_Freight_Normal = uint.Parse(freight.sumup);
//                            }
//                        }
//                        #endregion

//                        #region 訂單價格

//                        product.Where(m => m.dsr != "物流服務費").ToList().ForEach(m => newMaster.Order_Product_Subtotal += uint.Parse(m.sumup));
//                        product.ForEach(m => newMaster.Order_Amount += uint.Parse(m.sumup));
//                        #endregion

//                        #region 支付方式

//                        switch (product.FirstOrDefault().Payment)
//                        {
//                            case "信用卡一次":
//                                newMaster.Order_Payment = 1;
//                                break;
//                            case "7-11貨到付款":
//                                newMaster.Order_Payment = 14;
//                                break;
//                            default:
//                                newMaster.Order_Payment = 1;
//                                break;
//                        }
//                        #endregion

//                        #region 發貨信息

//                        newMaster.Delivery_Name = product.FirstOrDefault().agpesnm;
//                        newMaster.Delivery_Mobile = product.FirstOrDefault().agpesacttel;
//                        newMaster.Delivery_Phone = product.FirstOrDefault().agpestel1;
//                        newMaster.Delivery_Zip = uint.Parse(product.FirstOrDefault().agpesadrzip);
//                        newMaster.Delivery_Address = product.FirstOrDefault().agpesadr;
//                        #endregion

//                        #endregion

//                        #region ChannelOrder

//                        List<ChannelOrder> cOrder = new List<ChannelOrder>();
//                        ChannelOrder channelOrder;
//                        foreach (var slave in slaves)
//                        {
//                            foreach (var detail in slave.Details)
//                            {
//                                //ChannelOrder
//                                channelOrder = new ChannelOrder { Channel_Id = ChannelID, Order_Id = newMaster.Channel_Order_Id.ToString() };
//                                channelOrder.Ordertime = CommonFunction.GetPHPTime();
//                                var p = product.Where(m => m.chlitpdno == detail.Channel_Detail_Id).FirstOrDefault();
//                                if (p != null)
//                                {
//                                    channelOrder.Channel_Detail_Id = detail.Channel_Detail_Id;
//                                    channelOrder.Createtime = CommonFunction.GetPHPTime(p.orddat);
//                                    channelOrder.Latest_Deliver_Date = CommonFunction.GetPHPTime(p.prndldat);
//                                    cOrder.Add(channelOrder);
//                                }
//                            }
//                        }
//                        #endregion

//                        #region 寫入數據庫

//                        if (orderImportMgr.Save2DB(newMaster, slaves, cOrder, all))
//                        {
//                            all.FindAll(m => m.dmtshxuid == om && m.IsSel).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("IMPORT_SUCCESS"));
//                            successCount++;
//                        }

//                        #endregion
//                    }
//                }
//            }
//            catch (Exception)
//            {
//            }
//            return successCount;
//        }

//        #endregion
//    }
//}
