///Add by wangwei0216w 2014/8/15

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using DBAccess;
using BLL.gigade.Common;
using System.Collections;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class OrderImportPayEasy: IOrderImport
    {
        private OrderImportMgr orderImportMgr;
        private ChannelMgr channelMgr;
        private ChannelShippingMgr channelShippingMgr;

        public Channel CurChannel { get; set; }
        public string MySqlConnStr { get; set; }
        public string SqlServerConnStr { get; set; }
        public int PriceSum = 0;     //用於記錄總金額
        public int Excel_Price_Sum = 0;//Excel表上的總金額
        /// <summary>
        /// 構造函數
        /// </summary>
        public OrderImportPayEasy()
        {         
        }

        /// <summary>
        /// 讀取指定路徑的Excel文件
        /// </summary>
        /// <param name="filePath">文件路徑</param>
        /// <returns>記錄Exel文件內容的List集合</returns>
        public List<OrdersImport> ReadExcel2Page(string filePath, string template, string model_in) 
        {
            try
            {
                if (!System.IO.File.Exists(filePath))
                {
                    return null;
                }
                orderImportMgr = new OrderImportMgr(MySqlConnStr, CurChannel.channel_id);

                var tmp = orderImportMgr.ReadExcelWatch<OrdersImport>(filePath, template, model_in);
                if (tmp.Count() > 0)
                {
                    tmp.ForEach(m =>
                    {
                        //edit by wangwei0216w 2014/8/15 暫時設置訂購人與收購人是同一個人,默認宅配方式
                        m.ordpesnm = m.agpesnm;
                        m.shipco = "宅配";

                        ///移除訂單編號為空的記錄  edit by xinglu0624w 
                        if (string.IsNullOrEmpty(m.chlitpdno) || string.IsNullOrEmpty(m.dmtshxuid))
                        {
                            tmp.Remove(m);
                        }
                    });
                } 
     
                var orderGroup = from g in tmp group g by g.dmtshxuid; // 訂單分組
                #region 訂單分組
                foreach (var group in orderGroup)
                {
                    PriceSum = 0;//金額清0
                    Excel_Price_Sum = 0;//金額清0
                    var orders = group.ToList();
                    //組合商品的父商品chlitpdno = 0，只需要計算其父商品Price_sum
                    var o = orders.Find(m => m.chlitpdno == "0");
                    if (o != null)
                    {
                        Excel_Price_Sum += Convert.ToInt32(o.sumup);
                        orders.Remove(o);
                        tmp.Remove(o);
                    }
                    else {
                        //單一商品需要計算sumup   //Int32.Parse(p.qty) *   sumup 表示在excel文檔中的總價  購買數量 乘以 單價
                        orders.ForEach(p => Excel_Price_Sum += Int32.Parse(p.sumup)); // edit by zhuoqin0830w  2015/07/23
                    }

                    ValidateOrders(orders);
                    #region 數據分析
                    foreach (var item in orders)
                    {
                        if (item.OrderState == "不可配送")                //判斷是否可配送
                        {
                            item.Msg = Resource.CoreMessage.GetResource("UNDISPATCHING");
                            continue;
                        }

                        bool oResult = orderImportMgr.IsExistsOrder(item);
                        if (oResult)
                        {
                            orders.FindAll(m => m.dmtshxuid == item.dmtshxuid).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("ORDER_EXISTS"));
                            continue;
                        }

                        ProductItemMap map = orderImportMgr.QueryProductMapping(item);

                        if (map == null)
                        {
                            item.Msg = Resource.CoreMessage.GetResource("PRODUCT_MAP_NOT_EXISTS");
                            continue;
                        }

                        uint product_id = map.product_id;
                        if (map.item_id != 0)
                        {
                            ProductItem proItem = orderImportMgr.QueryProductItem(Convert.ToUInt32(map.item_id));
                            if (proItem == null)
                            {
                                item.Msg = Resource.CoreMessage.GetResource("ITEMID_ID_NOT_EXISTS");
                                continue;
                            }
                            product_id = proItem.Product_Id;
                        }

                        Product pro = product_id == 0 ? null : orderImportMgr.QueryProduct(product_id);
                        if (pro == null)
                        {
                            item.Msg = Resource.CoreMessage.GetResource("PRODUCT_NOT_EXISTS");
                            continue;
                        }
                        else
                        {
                            if (pro.Combination == 0)
                            {
                                item.Msg = Resource.CoreMessage.GetResource("PRODUCT_NOT_USE_IN_ORDER");
                                continue;
                            }
                            if (pro.Product_Status != 5 && item.chlitpdno != "6727_13742" && item.chlitpdno != "6727_13743")
                            {
                                item.Msg = Resource.CoreMessage.GetResource("PRODUCT_STATUS_WRONG");
                                continue;
                            }
                            if (pro.Product_Start != 0 && pro.Product_Start > CommonFunction.GetPHPTime())
                            {
                                item.Msg = Resource.CoreMessage.GetResource("PRODUCT_TIME_OUT");
                                continue;
                            }
                            if (pro.Product_End != 0 && pro.Product_End < CommonFunction.GetPHPTime())
                            {
                                item.Msg = Resource.CoreMessage.GetResource("PRODUCT_TIME_OUT");
                                continue;
                            }
                        }

                        item.sumup = (map.product_price * (Convert.ToInt32(item.qty))).ToString();

                        PriceSum += Convert.ToInt32(item.sumup);//累加總金額
                    }

                    if (PriceSum != Excel_Price_Sum)//則對比金額是否正確
                    {
                        orders.ForEach(m =>
                        {
                            if (m.Msg == "")
                            {
                                m.Msg = Resource.CoreMessage.GetResource("SUM_WRONG");
                            }
                        });
                    }
                    #endregion
                }
                #endregion


                return tmp;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderImportPayEasy-->ReadExcel2Page-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 驗證Excel導入的內容是否符合規則
        /// </summary>
        /// <param name="orders">記錄Exel文件內容的List集合</param>
        public void ValidateOrders(List<OrdersImport> orders)
        {
            try
            {
                //各種數據驗證
                orders.FindAll(m => !(string.IsNullOrEmpty(m.qty) && m.dsr.Contains("物流服務費")) && !DataCheck.IsNumeric(m.qty)).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("BUY_COUNT_WRONG"));
                orders.FindAll(m => !string.IsNullOrEmpty(m.prndldat) && !DataCheck.IsDate(m.prndldat)).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("LAST_DELIVERY_DATE"));
                orders.FindAll(m => !string.IsNullOrEmpty(m.orddat) && !DataCheck.IsDate(m.orddat)).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("BUY_DATE_WRONG"));
                orders.FindAll(m => !string.IsNullOrEmpty(m.agpesadrzip) && !DataCheck.IsNumeric(m.agpesadrzip)).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("DELIVERY_ZIP_WRONG"));
            }
            catch (Exception ex)
            {
                throw new Exception("OrderImportPayEasy-->ValidateOrders-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 將文件內容存儲到數據庫
        /// </summary>
        /// <param name="all"></param>
        /// <param name="pdfFile"></param>
        /// <param name="importType"></param>
        /// <param name="chkRecord"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public int Import2DB(List<OrdersImport> all, string pdfFile, string importType, string chkRecord,int siteId, ref int totalCount)
        {
            orderImportMgr = new OrderImportMgr(MySqlConnStr, CurChannel.channel_id);
            int successCount = 0;
            try
            {
                List<OrdersImport> chks = orderImportMgr.SplitChkData(chkRecord).FindAll(m => m.chlitpdno != "0");//移除廠商商品編號為0的
                if (chks != null && all != null && all.Count > 0)
                {
                    #region 篩選出選中數據資訊

                    //所有選中商品
                    var result = from h in chks
                                 join o in all on new { h.dmtshxuid, h.chlitpdno } equals new { o.dmtshxuid, o.chlitpdno } into cs
                                 from c in cs.DefaultIfEmpty()
                                 select c;
                    #endregion

                    //所有訂單編號
                    List<string> master = chks.GroupBy(m => m.dmtshxuid).Select(m => m.Key).ToList();
                    totalCount = master == null ? 0 : master.Count;

                    //外站物流業者
                    channelShippingMgr = new ChannelShippingMgr(MySqlConnStr);
                    List<ChannelShipping> shippings = channelShippingMgr.QueryByChannelId(new ChannelShipping { channel_id = CurChannel.channel_id });

                    foreach (var om in master)
                    {
                        //單個訂單下所有商品
                        var product = (from p in result
                                       where p.dmtshxuid == om
                                       select p).ToList();

                        all.FindAll(m => m.dmtshxuid == om && product.Exists(n => n.chlitpdno == m.chlitpdno)).ForEach(m => m.IsSel = true);

                        #region 訂單物流業者

                        ChannelShipping cShipping = null;
                        if (shippings != null)
                        {
                            //查詢該訂單物流業者 取貨方式
                            cShipping = shippings.Where(m => m.shipco.Trim().ToLower() == product.FirstOrDefault().shipco.Trim().ToLower()).FirstOrDefault();
                        }
                        if (shippings == null || cShipping == null)
                        {
                            all.FindAll(m => m.dmtshxuid == om && m.IsSel).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("DELIVERY_MAP_NOT_EXISTS"));
                            continue;
                        }
                        #endregion

                        #region 生成OrderSlave

                        List<OrderSlave> slaves = orderImportMgr.GetSlave(product, all);
                        //訂單內商品有問題（商品對照不存在，商品不存在，商品庫存不足）,跳過處理下筆訂單
                        if (slaves == null || slaves.Count == 0)
                        {
                            continue;
                        }
                        #endregion

                        #region OrderMaster

                        OrderMaster newMaster = new OrderMaster();

                        #region 其他信息

                        newMaster.Order_Status = 2;
                        //newMaster.Order_Id = uint.Parse(orderImportMgr.NextOrderId());
                        channelMgr = new ChannelMgr(MySqlConnStr);
                        newMaster.User_Id = Convert.ToUInt32(channelMgr.GetUserIdByChannelId(CurChannel.channel_id));

                        //備註
                        if (!string.IsNullOrEmpty(product.FirstOrDefault().xrem))
                        {
                            newMaster.Note_Order = "購買備註:" + product.FirstOrDefault().xrem;
                        }
                        if (!string.IsNullOrEmpty(product.FirstOrDefault().shipxrem))
                        {
                            newMaster.Note_Order = string.IsNullOrEmpty(newMaster.Note_Order) ? "配送備註:" + product.FirstOrDefault().shipxrem : newMaster.Note_Order + ",配送備註:" + product.FirstOrDefault().shipxrem;
                        }
                        string name = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_username;
                        newMaster.Note_Admin = name + " 於" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "建立";
                        if (!string.IsNullOrEmpty(product.FirstOrDefault().orddat))
                        {
                            newMaster.Order_Createdate = Convert.ToUInt32(CommonFunction.GetPHPTime(product.FirstOrDefault().orddat));
                        }
                        else
                        {
                            newMaster.Order_Createdate = Convert.ToUInt32(CommonFunction.GetPHPTime());
                        }
                        newMaster.Order_Date_Pay = Convert.ToUInt32(CommonFunction.GetPHPTime());
                        newMaster.Import_Time = DateTime.Now;
                        newMaster.Channel = Convert.ToUInt32(CurChannel.channel_id);
                        newMaster.Channel_Order_Id = product.FirstOrDefault().dmtshxuid;
                        if (!string.IsNullOrEmpty(product.FirstOrDefault().agpesadrzip))
                        {
                            newMaster.Order_Zip = Convert.ToUInt32(product.FirstOrDefault().agpesadrzip);
                        }
                        #endregion

                        #region 发票状态

                        if (CurChannel.receipt_to != 2)
                        {
                            newMaster.Invoice_Status = 2;
                        }
                        #endregion

                        #region 物流方式

                        newMaster.Delivery_Store = Convert.ToUInt32(cShipping.shipping_carrior);
                        newMaster.Retrieve_Mode = Convert.ToUInt32(cShipping.retrieve_mode);

                        #endregion

                        #region 運費

                        var freight = product.Where(m => m.dsr.Contains("物流服務費")).FirstOrDefault();
                        if (freight != null && !string.IsNullOrEmpty(freight.sumup))
                        {
                            if (freight.chlitpdno == "6727_13743")//若廠商商品編號為"6727_13743"是低溫運費
                            {
                                newMaster.Order_Freight_Low = uint.Parse(freight.sumup);
                            }
                            else//若廠商商品編號為"6727_13742"是常溫運費
                            {
                                newMaster.Order_Freight_Normal = uint.Parse(freight.sumup);
                            }
                        }
                        #endregion

                        #region 訂單價格

                        product.Where(m => !m.dsr.Contains("物流服務費")).ToList().ForEach(m => newMaster.Order_Product_Subtotal += uint.Parse(string.IsNullOrEmpty(m.sumup) ? "0" : m.sumup));
                        product.ForEach(m => newMaster.Order_Amount += uint.Parse(string.IsNullOrEmpty(m.sumup) ? "0" : m.sumup));
                        #endregion

                        #region 支付方式
                        //若是外站類型為”合作外站”的，匯入訂單時的 payment = 15
                        newMaster.Order_Payment = 15;
                        //switch (product.FirstOrDefault().Payment)
                        //{
                        //    case "信用卡一次":
                        //        newMaster.Order_Payment = 1;
                        //        break;
                        //    case "7-11貨到付款":
                        //        newMaster.Order_Payment = 15;
                        //        break;
                        //    default:
                        //        newMaster.Order_Payment = 1;
                        //        break;
                        //}
                        #endregion

                        #region 訂購人信息

                        newMaster.Order_Name = product.FirstOrDefault().ordpesnm;
                        newMaster.Order_Mobile = product.FirstOrDefault().ordpesnacttel.Replace("'", "");//去掉電話中的單引號
                        if (string.IsNullOrEmpty(newMaster.Order_Mobile))
                        {
                            newMaster.Order_Mobile = product.FirstOrDefault().agpesacttel.Replace("'", "");//去掉電話中的單引號
                        }
                        newMaster.Order_Address = product.FirstOrDefault().agpesadr;//order_address與delivery_address相同
                        #endregion

                        #region 發貨信息

                        newMaster.Delivery_Name = product.FirstOrDefault().agpesnm;
                        newMaster.Delivery_Mobile = product.FirstOrDefault().agpesacttel.Replace("'", "");//去掉電話中的單引號
                        newMaster.Delivery_Phone = product.FirstOrDefault().agpestel1.Replace("'", "");//去掉電話中的單引號
                        if (!string.IsNullOrEmpty(product.FirstOrDefault().agpesadrzip))
                        {
                            newMaster.Delivery_Zip = uint.Parse(product.FirstOrDefault().agpesadrzip);
                        }
                        newMaster.Delivery_Address = product.FirstOrDefault().agpesadr;
                        #endregion

                        #endregion

                        #region ChannelOrder

                        List<ChannelOrder> cOrder = new List<ChannelOrder>();
                        ChannelOrder channelOrder;
                        foreach (var slave in slaves)
                        {
                            foreach (var detail in slave.Details)
                            {
                                detail.Site_Id = siteId;//add xw 2014/02/03
                                //ChannelOrder
                                channelOrder = new ChannelOrder { Channel_Id = CurChannel.channel_id, Order_Id = newMaster.Channel_Order_Id.ToString() };
                                var p = product.Where(m => m.chlitpdno == detail.Channel_Detail_Id).FirstOrDefault();
                                if (p != null)
                                {
                                    //if (importType == "2")
                                    //{
                                    //    //channelOrder.Store_Dispatch_File = pdfFile;
                                    //}
                                    channelOrder.Dispatch_Seq = p.shipno;
                                    channelOrder.Channel_Detail_Id = detail.Channel_Detail_Id;
                                    if (!string.IsNullOrEmpty(p.orddat))
                                    {
                                        channelOrder.Createtime = Convert.ToDateTime(p.orddat);
                                    }
                                    channelOrder.Ordertime = channelOrder.Createtime;
                                    if (!string.IsNullOrEmpty(p.prndldat))
                                    {
                                        channelOrder.Latest_Deliver_Date = Convert.ToDateTime(p.prndldat);
                                    }
                                    cOrder.Add(channelOrder);
                                }
                            }
                        }
                        #endregion

                        #region 寫入數據庫

                        if (orderImportMgr.Save2DB(newMaster, slaves, cOrder, all))
                        {
                            all.FindAll(m => m.dmtshxuid == om && m.IsSel).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("IMPORT_SUCCESS"));
                            successCount++;
                        }

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("OrderImportPayEasy-->Import2DB-->" + ex.Message, ex);
            }
            return successCount;
        }

    }
}
