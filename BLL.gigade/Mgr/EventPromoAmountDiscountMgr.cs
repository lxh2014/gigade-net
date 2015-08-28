using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using MySql.Data.MySqlClient;
using BLL.gigade.Model;
using System.Collections;
namespace BLL.gigade.Mgr
{
    public class EventPromoAmountDiscountMgr : IEventPromoAmountDiscountImplMgr
    {
        private IEventPromoAmountDiscountImplDao IDiscountDao;
        private readonly string conn;
        public EventPromoAmountDiscountMgr(string connectionStr)
        {
            IDiscountDao = new EventPromoAmountDiscountDao(connectionStr);
            this.conn = connectionStr;
        }
        public List<Model.Query.EventPromoAmountDiscountQuery> GetList(Model.Query.EventPromoAmountDiscountQuery epQuery, out int totalCount)
        {
            try
            {
                return IDiscountDao.GetList(epQuery, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("EventPromoAmountDiscountMgr-->GetList" + ex.Message, ex);
            }
        }

        public string GetCondiType(int condiType, string event_id)
        {
            throw new NotImplementedException();
        }

        public bool SavePromoAmountDiscount(Model.EventPromoAmountDiscount epadmodel, List<Model.EventPromoDiscount> epdmodels, string condiType)
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
                if (epadmodel.row_id == 0)//新增
                {
                    epadmodel.event_type = "AB";
                    epadmodel.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                    epadmodel.create_time = DateTime.Now;
                    epadmodel.modify_user = epadmodel.create_user;
                    epadmodel.modify_time = epadmodel.create_time;
                    mySqlCmd.CommandText = IDiscountDao.AddOrUpdate(epadmodel);
                    epadmodel.row_id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                    epadmodel.event_id = Common.CommonFunction.GetEventId(epadmodel.event_type, epadmodel.row_id.ToString());

                    _list.Add(IDiscountDao.UpdateEventId(epadmodel.row_id, epadmodel.event_id));

                    //處理輔表event_promo_gift
                    EventPromoDiscountDao _epgDao = new EventPromoDiscountDao(conn);
                    foreach (EventPromoDiscount item in epdmodels)
                    {
                        item.event_id = epadmodel.event_id;
                        item.create_user = epadmodel.create_user;
                        item.create_time = epadmodel.create_time;
                        item.modify_user = item.create_user;
                        item.modify_time = item.create_time;
                        _list.Add(_epgDao.AddOrUpdate(item));
                    }

                    //處理輔表活動商品設定
                    _list.AddRange(InsertCondiType(epadmodel, condiType));
                }
                else//編輯 
                {
                    //處理主表event_promo_amount_gift
                    epadmodel.event_type = "AB";
                    epadmodel.modify_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                    epadmodel.modify_time = DateTime.Now;
                    epadmodel.create_user = epadmodel.modify_user;
                    epadmodel.create_time = epadmodel.modify_time;

                    epadmodel.event_id = Common.CommonFunction.GetEventId(epadmodel.event_type, epadmodel.row_id.ToString());
                    _list.Add(IDiscountDao.AddOrUpdate(epadmodel));

                    //處理輔表event_promo_gift
                    EventPromoDiscountDao _epdDao = new EventPromoDiscountDao(conn);
                    string discount_ids = "";
                    foreach (EventPromoDiscount item in epdmodels)
                    {
                        item.event_id = epadmodel.event_id;
                        item.modify_user = epadmodel.modify_user;
                        item.modify_time = epadmodel.modify_time;

                        _list.Add(_epdDao.AddOrUpdate(item));
                        if (item.discount_id != 0)
                        {
                            discount_ids = discount_ids + item.discount_id + ",";
                        }
                    }

                    discount_ids = discount_ids.TrimEnd(',');
                    var arr_new_g = discount_ids.Split(',');
                    //獲取本次修改刪除的discount_id,先獲取現有的除新增外的discount，那麼原有的不在其中的discount則是刪除的

                    List<EventPromoDiscount> models = _epdDao.GetList(epadmodel.event_id);
                    string del_g = string.Empty;//存放本次刪除的discount
                    foreach (EventPromoDiscount item in models)
                    {
                        if (!arr_new_g.Contains(item.discount_id.ToString()))
                        {
                            del_g = del_g + item.discount_id.ToString() + ",";
                        }
                    }
                    if (!string.IsNullOrEmpty(del_g))
                    {
                        del_g = del_g.TrimEnd(',');
                        _list.Add(_epdDao.Delete(epadmodel.event_id, del_g));

                    }


                    //處理輔表活動商品設定
                    EventPromoBrandDao _epbDao = new EventPromoBrandDao(conn);
                    _list.Add(_epbDao.Delete(epadmodel.event_id));
                    EventPromoCategoryDao _epcateDao = new EventPromoCategoryDao(conn);
                    _list.Add(_epcateDao.Delete(epadmodel.event_id));
                    EventPromoClassDao _epclassDao = new EventPromoClassDao(conn);
                    _list.Add(_epclassDao.Delete(epadmodel.event_id));
                    EventPromoPaymentDao _eppayDao = new EventPromoPaymentDao(conn);
                    _list.Add(_eppayDao.Delete(epadmodel.event_id));
                    EventPromoShoppingcartDao _epcartDao = new EventPromoShoppingcartDao(conn);
                    _list.Add(_epcartDao.Delete(epadmodel.event_id));
                    EventPromoProductDao _epproDao = new EventPromoProductDao(conn);
                    _list.Add(_epproDao.Delete(epadmodel.event_id));

                    _list.AddRange(InsertCondiType(epadmodel, condiType));

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
                throw new Exception("EventPromoAmountDiscountMgr-->SavePromoAmountDiscount" + ex.Message, ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
        }
        public ArrayList InsertCondiType(EventPromoAmountDiscount model, string condiType)
        {
            ArrayList _list = new ArrayList();
            if (!string.IsNullOrEmpty(condiType))
            {
                var arryType = condiType.Split(',');
                if (model.condition_type == 1)//按品牌
                {
                    foreach (string iType in arryType)
                    {
                        EventPromoBrand epb = new EventPromoBrand();
                        epb.event_type = model.event_type;
                        epb.site_id = model.site_id;
                        epb.brand_id = Convert.ToInt32(iType);
                        epb.event_id = model.event_id;
                        epb.event_status = model.event_status;
                        epb.create_user = model.create_user;
                        epb.create_time = model.create_time;
                        epb.modify_user = epb.create_user;
                        epb.modify_time = epb.create_time;
                        epb.event_start = model.event_start;
                        epb.event_end = model.event_end;
                        EventPromoBrandDao _epbDao = new EventPromoBrandDao(conn);
                        _list.Add(_epbDao.AddOrUpdate(epb));

                    }
                }
                else if (model.condition_type == 2)//類別
                {
                    foreach (string iType in arryType)
                    {
                        EventPromoCategory epcate = new EventPromoCategory();
                        epcate.event_type = model.event_type;
                        epcate.site_id = model.site_id;
                        epcate.category_id = Convert.ToInt32(iType);
                        epcate.event_id = model.event_id;
                        epcate.event_status = model.event_status;
                        epcate.create_user = model.create_user;
                        epcate.create_time = model.create_time;
                        epcate.modify_user = epcate.create_user;
                        epcate.modify_time = epcate.create_time;
                        epcate.event_start = model.event_start;
                        epcate.event_end = model.event_end;
                        EventPromoCategoryDao _epcateDao = new EventPromoCategoryDao(conn);
                        _list.Add(_epcateDao.AddOrUpdate(epcate));

                    }
                }
                else if (model.condition_type == 3)//館別
                {
                    foreach (string iType in arryType)
                    {
                        EventPromoClass epclass = new EventPromoClass();
                        epclass.event_type = model.event_type;
                        epclass.site_id = model.site_id;
                        epclass.class_id = Convert.ToInt32(iType);
                        epclass.event_id = model.event_id;
                        epclass.event_status = model.event_status;
                        epclass.create_user = model.create_user;
                        epclass.create_time = model.create_time;
                        epclass.modify_user = epclass.create_user;
                        epclass.modify_time = epclass.create_time;
                        epclass.event_start = model.event_start;
                        epclass.event_end = model.event_end;
                        EventPromoClassDao _epclassDao = new EventPromoClassDao(conn);
                        _list.Add(_epclassDao.AddOrUpdate(epclass));

                    }
                }
                else if (model.condition_type == 4)//商品
                {
                    foreach (string iType in arryType)
                    {
                        var arryPro = iType.Split('&');
                        EventPromoProduct eppro = new EventPromoProduct();
                        eppro.event_type = model.event_type;
                        eppro.site_id = model.site_id;
                        eppro.product_id = Convert.ToInt32(arryPro[0]);
                        eppro.product_num_limit = Convert.ToInt32(arryPro[1]);
                        eppro.event_id = model.event_id;
                        eppro.event_status = model.event_status;
                        eppro.create_user = model.create_user;
                        eppro.create_time = model.create_time;
                        eppro.modify_user = eppro.create_user;
                        eppro.modify_time = eppro.create_time;
                        eppro.event_start = model.event_start;
                        eppro.event_end = model.event_end;
                        EventPromoProductDao _epproDao = new EventPromoProductDao(conn);
                        _list.Add(_epproDao.AddOrUpdate(eppro));

                    }
                }
                else if (model.condition_type == 5)//購物車
                {

                    foreach (string iType in arryType)
                    {
                        EventPromoShoppingcart epcart = new EventPromoShoppingcart();
                        epcart.event_type = model.event_type;
                        //epcart.site_id = epaGift.site_id;
                        epcart.cart_id = Convert.ToInt32(iType);
                        epcart.event_id = model.event_id;
                        epcart.event_status = model.event_status;
                        epcart.create_user = model.create_user;
                        epcart.create_time = model.create_time;
                        epcart.modify_user = epcart.create_user;
                        epcart.modify_time = epcart.create_time;
                        epcart.event_start = model.event_start;
                        epcart.event_end = model.event_end;
                        EventPromoShoppingcartDao _epcartDao = new EventPromoShoppingcartDao(conn);
                        _list.Add(_epcartDao.AddOrUpdate(epcart));

                    }
                }
                else if (model.condition_type == 6)//付款方式
                {

                    foreach (string iType in arryType)
                    {
                        EventPromoPayment eppay = new EventPromoPayment();
                        eppay.event_type = model.event_type;
                        eppay.site_id = model.site_id;
                        eppay.payment_id = Convert.ToInt32(iType);
                        eppay.event_id = model.event_id;
                        eppay.event_status = model.event_status;
                        eppay.create_user = model.create_user;
                        eppay.create_time = model.create_time;
                        eppay.modify_user = eppay.create_user;
                        eppay.modify_time = eppay.create_time;
                        eppay.event_start = model.event_start;
                        eppay.event_end = model.event_end;
                        EventPromoPaymentDao _eppayDao = new EventPromoPaymentDao(conn);
                        _list.Add(_eppayDao.AddOrUpdate(eppay));

                    }
                }

            }
            return _list;
        }
        public bool UpdateActive(Model.EventPromoAmountDiscount model)
        {
            ArrayList _list = new ArrayList();
            try
            {
                _list.Add(IDiscountDao.UpdateActive(model));
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

                throw new Exception("EventPromoAmountDiscountMgr-->UpdateActive" + ex.Message, ex);
            }
        }
    }
}
