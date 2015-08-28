using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using System.Collections;
using MySql.Data.MySqlClient;

namespace BLL.gigade.Mgr
{
    public class EventPromoAmountFareMgr : IEventPromoAmountFareImplMgr
    {
        private IEventPromoAmountFareImplDao _iepaFareDao;
        private readonly string _conn;
        public EventPromoAmountFareMgr(string connectionStr)
        {
            _iepaFareDao = new EventPromoAmountFareDao(connectionStr);
            this._conn = connectionStr;
        }
        public List<Model.EventPromoAmountFare> GetList(Model.EventPromoAmountFare model, out int totalCount)
        {
            try
            {
                List<Model.EventPromoAmountFare> _list = _iepaFareDao.GetList(model, out totalCount);
                if (_list.Count > 0)
                {
                    SiteDao _sDao = new SiteDao(_conn);
                    List<Site> _slist = _sDao.Query(new Site { });
                    foreach (Model.EventPromoAmountFare item in _list)
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
            catch (MySqlException ex)
            {
                throw new Exception(ex.Number.ToString() + ":EventPromoAmountFareMgr-->GetList-->" + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoAmountFareMgr-->GetList-->" + ex.Message, ex);
            }
        }

        public bool SavePromoAmountFare(EventPromoAmountFare epaFare, string condiType)
        {
            ArrayList _list = new ArrayList();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(_conn);
            bool isSave = false;
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                if (epaFare.row_id == 0)//新增
                {
                    //處理主表event_promo_amount_fare
                    epaFare.event_type = "AC";
                    epaFare.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                    epaFare.create_time = DateTime.Now;
                    epaFare.modify_user = epaFare.create_user;
                    epaFare.modify_time = epaFare.create_time;

                    mySqlCmd.CommandText = _iepaFareDao.AddOrUpdate(epaFare);
                    epaFare.row_id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                    epaFare.event_id = Common.CommonFunction.GetEventId(epaFare.event_type, epaFare.row_id.ToString());

                    _list.Add(_iepaFareDao.UpdateEventId(epaFare.row_id, epaFare.event_id));

                    //處理輔表活動商品設定
                    _list.AddRange(InsertCondiType(epaFare, condiType));
                }
                else//編輯 
                {
                    //處理主表event_promo_amount_fare
                    epaFare.event_type = "AC";
                    epaFare.modify_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                    epaFare.modify_time = DateTime.Now;
                    epaFare.event_id = Common.CommonFunction.GetEventId(epaFare.event_type, epaFare.row_id.ToString());
                    _list.Add(_iepaFareDao.AddOrUpdate(epaFare));


                    //處理輔表活動商品設定
                    EventPromoBrandDao _epbDao = new EventPromoBrandDao(_conn);
                    _list.Add(_epbDao.Delete(epaFare.event_id));
                    EventPromoCategoryDao _epcateDao = new EventPromoCategoryDao(_conn);
                    _list.Add(_epcateDao.Delete(epaFare.event_id));
                    EventPromoClassDao _epclassDao = new EventPromoClassDao(_conn);
                    _list.Add(_epclassDao.Delete(epaFare.event_id));
                    EventPromoPaymentDao _eppayDao = new EventPromoPaymentDao(_conn);
                    _list.Add(_eppayDao.Delete(epaFare.event_id));
                    EventPromoShoppingcartDao _epcartDao = new EventPromoShoppingcartDao(_conn);
                    _list.Add(_epcartDao.Delete(epaFare.event_id));
                    EventPromoProductDao _epproDao = new EventPromoProductDao(_conn);
                    _list.Add(_epproDao.Delete(epaFare.event_id));

                    _list.AddRange(InsertCondiType(epaFare, condiType));

                }

                for (int i = 0; i < _list.Count; i++)
                {
                    mySqlCmd.CommandText = _list[i].ToString();
                    mySqlCmd.ExecuteNonQuery();
                }
                mySqlCmd.Transaction.Commit();
                isSave = true;

            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Number.ToString() + ":EventPromoAmountFareMgr-->SavePromoAmountFare" + ex.Message, ex);
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("EventPromoAmountFareMgr-->SavePromoAmountFare" + ex.Message, ex);
            }
            finally
            {

                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
            return isSave;
        }


        public ArrayList InsertCondiType(EventPromoAmountFare epaFare, string condiType)
        {
            ArrayList _list = new ArrayList();
            if (!string.IsNullOrEmpty(condiType))
            {
                var arryType = condiType.Split(',');
                if (epaFare.condition_type == 1)//按品牌
                {
                    foreach (string iType in arryType)
                    {
                        EventPromoBrand epb = new EventPromoBrand();
                        epb.event_type = epaFare.event_type;
                        epb.site_id = epaFare.site_id;
                        epb.brand_id = Convert.ToInt32(iType);
                        epb.event_id = epaFare.event_id;
                        epb.event_status = epaFare.event_status;
                        epb.create_user = epaFare.modify_user;
                        epb.create_time = epaFare.modify_time;
                        epb.modify_user = epb.create_user;
                        epb.modify_time = epb.create_time;
                        epb.event_start = epaFare.event_start;
                        epb.event_end = epaFare.event_end;
                        EventPromoBrandDao _epbDao = new EventPromoBrandDao(_conn);
                        _list.Add(_epbDao.AddOrUpdate(epb));

                    }
                }
                else if (epaFare.condition_type == 2)//類別
                {
                    foreach (string iType in arryType)
                    {
                        EventPromoCategory epcate = new EventPromoCategory();
                        epcate.event_type = epaFare.event_type;
                        epcate.site_id = epaFare.site_id;
                        epcate.category_id = Convert.ToInt32(iType);
                        epcate.event_id = epaFare.event_id;
                        epcate.event_status = epaFare.event_status;
                        epcate.create_user = epaFare.modify_user;
                        epcate.create_time = epaFare.modify_time;
                        epcate.modify_user = epcate.create_user;
                        epcate.modify_time = epcate.create_time;
                        epcate.event_start = epaFare.event_start;
                        epcate.event_end = epaFare.event_end;
                        EventPromoCategoryDao _epcateDao = new EventPromoCategoryDao(_conn);
                        _list.Add(_epcateDao.AddOrUpdate(epcate));

                    }
                }
                else if (epaFare.condition_type == 3)//館別
                {
                    foreach (string iType in arryType)
                    {
                        EventPromoClass epclass = new EventPromoClass();
                        epclass.event_type = epaFare.event_type;
                        epclass.site_id = epaFare.site_id;
                        epclass.class_id = Convert.ToInt32(iType);
                        epclass.event_id = epaFare.event_id;
                        epclass.event_status = epaFare.event_status;
                        epclass.create_user = epaFare.modify_user;
                        epclass.create_time = epaFare.modify_time;
                        epclass.modify_user = epclass.create_user;
                        epclass.modify_time = epclass.create_time;
                        epclass.event_start = epaFare.event_start;
                        epclass.event_end = epaFare.event_end;
                        EventPromoClassDao _epclassDao = new EventPromoClassDao(_conn);
                        _list.Add(_epclassDao.AddOrUpdate(epclass));

                    }
                }
                else if (epaFare.condition_type == 4)//商品
                {
                    foreach (string iType in arryType)
                    {
                        var arryPro = iType.Split('&');
                        EventPromoProduct eppro = new EventPromoProduct();
                        eppro.event_type = epaFare.event_type;
                        eppro.site_id = epaFare.site_id;
                        eppro.product_id = Convert.ToInt32(arryPro[0]);
                        eppro.product_num_limit = Convert.ToInt32(arryPro[1]);
                        eppro.event_id = epaFare.event_id;
                        eppro.event_status = epaFare.event_status;
                        eppro.create_user = epaFare.modify_user;
                        eppro.create_time = epaFare.modify_time;
                        eppro.modify_user = eppro.create_user;
                        eppro.modify_time = eppro.create_time;
                        eppro.event_start = epaFare.event_start;
                        eppro.event_end = epaFare.event_end;
                        EventPromoProductDao _epproDao = new EventPromoProductDao(_conn);
                        _list.Add(_epproDao.AddOrUpdate(eppro));

                    }
                }
                else if (epaFare.condition_type == 5)//購物車
                {

                    foreach (string iType in arryType)
                    {
                        EventPromoShoppingcart epcart = new EventPromoShoppingcart();
                        epcart.event_type = epaFare.event_type;
                        //epcart.site_id = epaFare.site_id;
                        epcart.cart_id = Convert.ToInt32(iType);
                        epcart.event_id = epaFare.event_id;
                        epcart.event_status = epaFare.event_status;
                        epcart.create_user = epaFare.modify_user;
                        epcart.create_time = epaFare.modify_time;
                        epcart.modify_user = epcart.create_user;
                        epcart.modify_time = epcart.create_time;
                        epcart.event_start = epaFare.event_start;
                        epcart.event_end = epaFare.event_end;
                        EventPromoShoppingcartDao _epcartDao = new EventPromoShoppingcartDao(_conn);
                        _list.Add(_epcartDao.AddOrUpdate(epcart));

                    }
                }
                else if (epaFare.condition_type == 6)//付款方式
                {

                    foreach (string iType in arryType)
                    {
                        EventPromoPayment eppay = new EventPromoPayment();
                        eppay.event_type = epaFare.event_type;
                        eppay.site_id = epaFare.site_id;
                        eppay.payment_id = Convert.ToInt32(iType);
                        eppay.event_id = epaFare.event_id;
                        eppay.event_status = epaFare.event_status;
                        eppay.create_user = epaFare.modify_user;
                        eppay.create_time = epaFare.modify_time;
                        eppay.modify_user = eppay.create_user;
                        eppay.modify_time = eppay.create_time;
                        eppay.event_start = epaFare.event_start;
                        eppay.event_end = epaFare.event_end;
                        EventPromoPaymentDao _eppayDao = new EventPromoPaymentDao(_conn);
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
                    EventPromoBrandDao _epbDao = new EventPromoBrandDao(_conn);
                    List<EventPromoBrand> model = _epbDao.GetList(event_id);
                    foreach (EventPromoBrand item in model)
                    {
                        content += item.brand_id + ",";
                    }
                    content = content.TrimEnd(',');
                }
                else if (condiType == 2)//類別
                {
                    EventPromoCategoryDao _epcateDao = new EventPromoCategoryDao(_conn);
                    List<EventPromoCategory> model = _epcateDao.GetList(event_id);
                    foreach (EventPromoCategory item in model)
                    {
                        content += item.category_id + "&" + item.category_name + ",";
                    }
                    content = content.TrimEnd(',');
                }
                else if (condiType == 3)//館別
                {
                    EventPromoClassDao _epclassDao = new EventPromoClassDao(_conn);
                    List<EventPromoClass> model = _epclassDao.GetList(event_id);
                    foreach (EventPromoClass item in model)
                    {
                        content += item.class_id + ",";
                    }
                    content = content.TrimEnd(',');
                }
                else if (condiType == 4)//商品
                {
                    EventPromoProductDao _epproDao = new EventPromoProductDao(_conn);

                    List<EventPromoProduct> model = _epproDao.GetList(event_id);
                    foreach (EventPromoProduct item in model)
                    {
                        content += item.product_id + "&" + item.product_num_limit + "&" + item.product_name + ",";
                    }
                    content = content.TrimEnd(',');
                }
                else if (condiType == 5)//購物車
                {
                    EventPromoShoppingcartDao _epcartDao = new EventPromoShoppingcartDao(_conn);

                    List<EventPromoShoppingcart> model = _epcartDao.GetList(event_id);
                    foreach (EventPromoShoppingcart item in model)
                    {
                        content += item.cart_id + ",";
                    }
                    content = content.TrimEnd(',');
                }
                else if (condiType == 6)//付款方式
                {
                    EventPromoPaymentDao _eppayDao = new EventPromoPaymentDao(_conn);

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

        public bool UpdateActive(EventPromoAmountFare model)
        {
            ArrayList _list = new ArrayList();
            try
            {
                _list.Add(_iepaFareDao.UpdateActive(model));
                //處理輔表活動商品設定
                if (model.condition_type == 1)
                {
                    EventPromoBrandDao _epbDao = new EventPromoBrandDao(_conn);
                    _list.Add(_epbDao.UpdateActive(new EventPromoBrand { event_status = model.event_status, modify_user = model.modify_user, modify_time = model.modify_time, event_id = model.event_id }));
                }
                else if (model.condition_type == 2)
                {
                    EventPromoCategoryDao _epcateDao = new EventPromoCategoryDao(_conn);
                    _list.Add(_epcateDao.UpdateActive(new EventPromoCategory { event_status = model.event_status, modify_user = model.modify_user, modify_time = model.modify_time, event_id = model.event_id }));
                }
                else if (model.condition_type == 3)
                {
                    EventPromoClassDao _epclassDao = new EventPromoClassDao(_conn);
                    _list.Add(_epclassDao.UpdateActive(new EventPromoClass { event_status = model.event_status, modify_user = model.modify_user, modify_time = model.modify_time, event_id = model.event_id }));
                }
                else if (model.condition_type == 4)
                {
                    EventPromoProductDao _epproDao = new EventPromoProductDao(_conn);
                    _list.Add(_epproDao.UpdateActive(new EventPromoProduct { event_status = model.event_status, modify_user = model.modify_user, modify_time = model.modify_time, event_id = model.event_id }));

                }
                else if (model.condition_type == 5)
                {
                    EventPromoShoppingcartDao _epcartDao = new EventPromoShoppingcartDao(_conn);
                    _list.Add(_epcartDao.UpdateActive(new EventPromoShoppingcart { event_status = model.event_status, modify_user = model.modify_user, modify_time = model.modify_time, event_id = model.event_id }));
                }
                else if (model.condition_type == 6)
                {
                    EventPromoPaymentDao _eppayDao = new EventPromoPaymentDao(_conn);
                    _list.Add(_eppayDao.UpdateActive(new EventPromoPayment { event_status = model.event_status, modify_user = model.modify_user, modify_time = model.modify_time, event_id = model.event_id }));

                }
                MySqlDao _mySqlDao = new MySqlDao(_conn);
                return _mySqlDao.ExcuteSqls(_list);

            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Number.ToString() + ":EventPromoAmountFareMgr-->UpdateActive-->" + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoAmountFareMgr-->UpdateActive-->" + ex.Message, ex);
            }
        }
    }
}
