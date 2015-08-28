using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using MySql.Data.MySqlClient;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class EventPromoAmountGiftMgr : IEventPromoAmountGiftImplMgr
    {
        private IEventPromoAmountGiftImplDao _iepaGiftDao;
        private readonly string conn;
        public EventPromoAmountGiftMgr(string connectionStr)
        {
            _iepaGiftDao = new EventPromoAmountGiftDao(connectionStr);
            this.conn = connectionStr;
        }
        public List<Model.Query.EventPromoAmountGiftQuery> GetList(Model.Query.EventPromoAmountGiftQuery epQuery, out int totalCount)
        {
            try
            {
                List<Model.Query.EventPromoAmountGiftQuery> _list = _iepaGiftDao.GetList(epQuery, out totalCount);
                if (_list.Count > 0)
                {
                    SiteDao _sDao = new SiteDao(conn);
                    List<Site> _slist = _sDao.Query(new Site { });
                    foreach (Model.Query.EventPromoAmountGiftQuery item in _list)
                    {
                        if (_slist.Count > 0 && !string.IsNullOrEmpty(item.site_id))
                        {
                            foreach (string sid in item.site_id.Split(','))
                            {
                                item.site_name = _slist.Find(m => m.Site_Id == Convert.ToUInt32(sid)).Site_Name;
                            }
                        }

                    }

                }
                return _list;
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoAmountGiftMgr-->GetList-->" + ex.Message, ex);
            }
        }

        public bool SavePromoAmountGift(EventPromoAmountGift epaGift, List<EventPromoGiftQuery> epGiftDetail, string condiType)
        {
            ArrayList _list = new ArrayList();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(conn);
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                if (epaGift.row_id == 0)//新增
                {
                    //處理主表event_promo_amount_gift
                    epaGift.event_type = "AA";
                    epaGift.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                    epaGift.create_time = DateTime.Now;
                    epaGift.modify_user = epaGift.create_user;
                    epaGift.modify_time = epaGift.create_time;
                    mySqlCmd.CommandText = _iepaGiftDao.AddOrUpdate(epaGift);
                    epaGift.row_id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                    epaGift.event_id = Common.CommonFunction.GetEventId(epaGift.event_type, epaGift.row_id.ToString());

                    _list.Add(_iepaGiftDao.UpdateEventId(epaGift.row_id, epaGift.event_id));

                    //處理輔表event_promo_gift
                    EventPromoGiftDao _epgDao = new EventPromoGiftDao(conn);
                    foreach (EventPromoGiftQuery item in epGiftDetail)
                    {
                        if (item.gift_type == 2)//購物金
                        {
                            item.bonus = item.gift_ware;
                            item.bonus_multiple = item.gift_num;
                        }
                        else if (item.gift_type == 3)//抵用券
                        {
                            item.welfare = item.gift_ware;
                            item.welfare_multiple = item.gift_num;
                        }
                        item.event_id = epaGift.event_id;
                        item.create_user = epaGift.create_user;
                        item.create_time = epaGift.create_time;
                        item.modify_user = item.create_user;
                        item.modify_time = item.create_time;
                        _list.Add(_epgDao.AddOrUpdate(item));
                    }

                    //處理輔表活動商品設定
                    _list.AddRange(InsertCondiType(epaGift, condiType));
                }
                else//編輯 
                {
                    //處理主表event_promo_amount_gift
                    epaGift.event_type = "AA";
                    epaGift.modify_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                    epaGift.modify_time = DateTime.Now;


                    epaGift.event_id = Common.CommonFunction.GetEventId(epaGift.event_type, epaGift.row_id.ToString());
                    _list.Add(_iepaGiftDao.AddOrUpdate(epaGift));

                    //處理輔表event_promo_gift
                    EventPromoGiftDao _epgDao = new EventPromoGiftDao(conn);
                    string gift_ids = "";
                    foreach (EventPromoGiftQuery item in epGiftDetail)
                    {
                        if (item.gift_type == 2)//購物金
                        {
                            item.bonus = item.gift_ware;
                            item.bonus_multiple = item.gift_num;
                        }
                        else if (item.gift_type == 3)//抵用券
                        {
                            item.welfare = item.gift_ware;
                            item.welfare_multiple = item.gift_num;
                        }
                        item.event_id = epaGift.event_id;
                        item.modify_user = epaGift.modify_user;
                        item.modify_time = epaGift.modify_time;

                        _list.Add(_epgDao.AddOrUpdate(item));
                        if (item.gift_id != 0)
                        {
                            gift_ids = gift_ids + item.gift_id + ",";
                        }
                    }

                    gift_ids = gift_ids.TrimEnd(',');
                    var arr_new_g = gift_ids.Split(',');
                    //獲取本次修改刪除的gift_id,先獲取現有的除新增外的gift，那麼原有的不在其中的gift則是刪除的

                    List<EventPromoGiftQuery> old_gift = _epgDao.GetList(epaGift.event_id);
                    string del_g = string.Empty;//存放本次刪除的gift
                    foreach (EventPromoGiftQuery item in old_gift)
                    {
                        if (!arr_new_g.Contains(item.gift_id.ToString()))
                        {
                            del_g = del_g + item.gift_id.ToString() + ",";
                        }
                    }
                    if (!string.IsNullOrEmpty(del_g))
                    {
                        del_g = del_g.TrimEnd(',');
                        _list.Add(_epgDao.Delete(epaGift.event_id, del_g));

                    }


                    //處理輔表活動商品設定
                    EventPromoBrandDao _epbDao = new EventPromoBrandDao(conn);
                    _list.Add(_epbDao.Delete(epaGift.event_id));
                    EventPromoCategoryDao _epcateDao = new EventPromoCategoryDao(conn);
                    _list.Add(_epcateDao.Delete(epaGift.event_id));
                    EventPromoClassDao _epclassDao = new EventPromoClassDao(conn);
                    _list.Add(_epclassDao.Delete(epaGift.event_id));
                    EventPromoPaymentDao _eppayDao = new EventPromoPaymentDao(conn);
                    _list.Add(_eppayDao.Delete(epaGift.event_id));
                    EventPromoShoppingcartDao _epcartDao = new EventPromoShoppingcartDao(conn);
                    _list.Add(_epcartDao.Delete(epaGift.event_id));
                    EventPromoProductDao _epproDao = new EventPromoProductDao(conn);
                    _list.Add(_epproDao.Delete(epaGift.event_id));

                    _list.AddRange(InsertCondiType(epaGift, condiType));

                }

                for (int i = 0; i < _list.Count; i++)
                {
                    mySqlCmd.CommandText = _list[i].ToString();
                    mySqlCmd.ExecuteNonQuery();
                }
                mySqlCmd.Transaction.Commit();
                return true;

            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                return false;
                throw new Exception("EventPromoAmountGiftMgr-->SavePromoAmountGift" + ex.Message, ex);

            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
        }


        public ArrayList InsertCondiType(EventPromoAmountGift epaGift, string condiType)
        {
            ArrayList _list = new ArrayList();
            if (!string.IsNullOrEmpty(condiType))
            {
                var arryType = condiType.Split(',');
                if (epaGift.condition_type == 1)//按品牌
                {
                    foreach (string iType in arryType)
                    {
                        EventPromoBrand epb = new EventPromoBrand();
                        epb.event_type = epaGift.event_type;
                        epb.site_id = epaGift.site_id;
                        epb.brand_id = Convert.ToInt32(iType);
                        epb.event_id = epaGift.event_id;
                        epb.event_status = epaGift.event_status;
                        epb.create_user = epaGift.modify_user;
                        epb.create_time = epaGift.modify_time;
                        epb.modify_user = epb.create_user;
                        epb.modify_time = epb.create_time;
                        epb.event_start = epaGift.event_start;
                        epb.event_end = epaGift.event_end;
                        EventPromoBrandDao _epbDao = new EventPromoBrandDao(conn);
                        _list.Add(_epbDao.AddOrUpdate(epb));

                    }
                }
                else if (epaGift.condition_type == 2)//類別
                {
                    foreach (string iType in arryType)
                    {
                        EventPromoCategory epcate = new EventPromoCategory();
                        epcate.event_type = epaGift.event_type;
                        epcate.site_id = epaGift.site_id;
                        epcate.category_id = Convert.ToInt32(iType);
                        epcate.event_id = epaGift.event_id;
                        epcate.event_status = epaGift.event_status;
                        epcate.create_user = epaGift.modify_user;
                        epcate.create_time = epaGift.modify_time;
                        epcate.modify_user = epcate.create_user;
                        epcate.modify_time = epcate.create_time;
                        epcate.event_start = epaGift.event_start;
                        epcate.event_end = epaGift.event_end;
                        EventPromoCategoryDao _epcateDao = new EventPromoCategoryDao(conn);
                        _list.Add(_epcateDao.AddOrUpdate(epcate));

                    }
                }
                else if (epaGift.condition_type == 3)//館別
                {
                    foreach (string iType in arryType)
                    {
                        EventPromoClass epclass = new EventPromoClass();
                        epclass.event_type = epaGift.event_type;
                        epclass.site_id = epaGift.site_id;
                        epclass.class_id = Convert.ToInt32(iType);
                        epclass.event_id = epaGift.event_id;
                        epclass.event_status = epaGift.event_status;
                        epclass.create_user = epaGift.modify_user;
                        epclass.create_time = epaGift.modify_time;
                        epclass.modify_user = epclass.create_user;
                        epclass.modify_time = epclass.create_time;
                        epclass.event_start = epaGift.event_start;
                        epclass.event_end = epaGift.event_end;
                        EventPromoClassDao _epclassDao = new EventPromoClassDao(conn);
                        _list.Add(_epclassDao.AddOrUpdate(epclass));

                    }
                }
                else if (epaGift.condition_type == 4)//商品
                {
                    foreach (string iType in arryType)
                    {
                        var arryPro = iType.Split('&');
                        EventPromoProduct eppro = new EventPromoProduct();
                        eppro.event_type = epaGift.event_type;
                        eppro.site_id = epaGift.site_id;
                        eppro.product_id = Convert.ToInt32(arryPro[0]);
                        eppro.product_num_limit = Convert.ToInt32(arryPro[1]);
                        eppro.event_id = epaGift.event_id;
                        eppro.event_status = epaGift.event_status;
                        eppro.create_user = epaGift.modify_user;
                        eppro.create_time = epaGift.modify_time;
                        eppro.modify_user = eppro.create_user;
                        eppro.modify_time = eppro.create_time;
                        eppro.event_start = epaGift.event_start;
                        eppro.event_end = epaGift.event_end;
                        EventPromoProductDao _epproDao = new EventPromoProductDao(conn);
                        _list.Add(_epproDao.AddOrUpdate(eppro));

                    }
                }
                else if (epaGift.condition_type == 5)//購物車
                {

                    foreach (string iType in arryType)
                    {
                        EventPromoShoppingcart epcart = new EventPromoShoppingcart();
                        epcart.event_type = epaGift.event_type;
                        //epcart.site_id = epaGift.site_id;
                        epcart.cart_id = Convert.ToInt32(iType);
                        epcart.event_id = epaGift.event_id;
                        epcart.event_status = epaGift.event_status;
                        epcart.create_user = epaGift.modify_user;
                        epcart.create_time = epaGift.modify_time;
                        epcart.modify_user = epcart.create_user;
                        epcart.modify_time = epcart.create_time;
                        epcart.event_start = epaGift.event_start;
                        epcart.event_end = epaGift.event_end;
                        EventPromoShoppingcartDao _epcartDao = new EventPromoShoppingcartDao(conn);
                        _list.Add(_epcartDao.AddOrUpdate(epcart));

                    }
                }
                else if (epaGift.condition_type == 6)//付款方式
                {

                    foreach (string iType in arryType)
                    {
                        EventPromoPayment eppay = new EventPromoPayment();
                        eppay.event_type = epaGift.event_type;
                        eppay.site_id = epaGift.site_id;
                        eppay.payment_id = Convert.ToInt32(iType);
                        eppay.event_id = epaGift.event_id;
                        eppay.event_status = epaGift.event_status;
                        eppay.create_user = epaGift.modify_user;
                        eppay.create_time = epaGift.modify_time;
                        eppay.modify_user = eppay.create_user;
                        eppay.modify_time = eppay.create_time;
                        eppay.event_start = epaGift.event_start;
                        eppay.event_end = epaGift.event_end;
                        EventPromoPaymentDao _eppayDao = new EventPromoPaymentDao(conn);
                        _list.Add(_eppayDao.AddOrUpdate(eppay));

                    }
                }

            }
            return _list;
        }



        public string GetCondiType(int condiType, string event_id)
        {
            string content = string.Empty;
            if (condiType != 0 && !string.IsNullOrEmpty(event_id))
            {

                if (condiType == 1)//按品牌
                {
                    EventPromoBrandDao _epbDao = new EventPromoBrandDao(conn);
                    List<EventPromoBrand> model = _epbDao.GetList(event_id);
                    foreach (EventPromoBrand item in model)
                    {
                        content += item.brand_id + ",";
                    }
                    content = content.TrimEnd(',');
                }
                else if (condiType == 2)//類別
                {
                    EventPromoCategoryDao _epcateDao = new EventPromoCategoryDao(conn);
                    List<EventPromoCategory> model = _epcateDao.GetList(event_id);
                    foreach (EventPromoCategory item in model)
                    {
                        content += item.category_id + "&" + item.category_name + ",";
                    }
                    content = content.TrimEnd(',');
                }
                else if (condiType == 3)//館別
                {
                    EventPromoClassDao _epclassDao = new EventPromoClassDao(conn);
                    List<EventPromoClass> model = _epclassDao.GetList(event_id);
                    foreach (EventPromoClass item in model)
                    {
                        content += item.class_id + ",";
                    }
                    content = content.TrimEnd(',');
                }
                else if (condiType == 4)//商品
                {
                    EventPromoProductDao _epproDao = new EventPromoProductDao(conn);

                    List<EventPromoProduct> model = _epproDao.GetList(event_id);
                    foreach (EventPromoProduct item in model)
                    {
                        content += item.product_id + "&" + item.product_num_limit + "&" + item.product_name + ",";
                    }
                    content = content.TrimEnd(',');
                }
                else if (condiType == 5)//購物車
                {
                    EventPromoShoppingcartDao _epcartDao = new EventPromoShoppingcartDao(conn);

                    List<EventPromoShoppingcart> model = _epcartDao.GetList(event_id);
                    foreach (EventPromoShoppingcart item in model)
                    {
                        content += item.cart_id + ",";
                    }
                    content = content.TrimEnd(',');
                }
                else if (condiType == 6)//付款方式
                {
                    EventPromoPaymentDao _eppayDao = new EventPromoPaymentDao(conn);

                    List<EventPromoPayment> model = _eppayDao.GetList(event_id);
                    foreach (EventPromoPayment item in model)
                    {
                        content += item.payment_id + ",";
                    }
                    content = content.TrimEnd(',');
                }



            }
            return content;
        }

        public bool UpdateActive(EventPromoAmountGift model)
        {
            ArrayList _list = new ArrayList();
            try
            {
                _list.Add(_iepaGiftDao.UpdateActive(model));
                //處理輔表活動商品設定
                if (model.condition_type == 1)
                {
                    EventPromoBrandDao _epbDao = new EventPromoBrandDao(conn);
                    _list.Add(_epbDao.UpdateActive(new EventPromoBrand { event_status = model.event_status, modify_user = model.modify_user, modify_time = model.modify_time, event_id = model.event_id }));
                }
                else if (model.condition_type == 2)
                {
                    EventPromoCategoryDao _epcateDao = new EventPromoCategoryDao(conn);
                    _list.Add(_epcateDao.UpdateActive(new EventPromoCategory { event_status = model.event_status, modify_user = model.modify_user, modify_time = model.modify_time, event_id = model.event_id }));
                }
                else if (model.condition_type == 3)
                {
                    EventPromoClassDao _epclassDao = new EventPromoClassDao(conn);
                    _list.Add(_epclassDao.UpdateActive(new EventPromoClass { event_status = model.event_status, modify_user = model.modify_user, modify_time = model.modify_time, event_id = model.event_id }));
                }
                else if (model.condition_type == 4)
                {
                    EventPromoProductDao _epproDao = new EventPromoProductDao(conn);
                    _list.Add(_epproDao.UpdateActive(new EventPromoProduct { event_status = model.event_status, modify_user = model.modify_user, modify_time = model.modify_time, event_id = model.event_id }));

                }
                else if (model.condition_type == 5)
                {
                    EventPromoShoppingcartDao _epcartDao = new EventPromoShoppingcartDao(conn);
                    _list.Add(_epcartDao.UpdateActive(new EventPromoShoppingcart { event_status = model.event_status, modify_user = model.modify_user, modify_time = model.modify_time, event_id = model.event_id }));
                }
                else if (model.condition_type == 6)
                {
                    EventPromoPaymentDao _eppayDao = new EventPromoPaymentDao(conn);
                    _list.Add(_eppayDao.UpdateActive(new EventPromoPayment { event_status = model.event_status, modify_user = model.modify_user, modify_time = model.modify_time, event_id = model.event_id }));

                }
                MySqlDao _mySqlDao = new MySqlDao(conn);
                return _mySqlDao.ExcuteSqls(_list);

            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoAmountGiftMgr-->UpdateActive-->" + ex.Message, ex);
            }
        }
    }
}
