using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class ScheduleItemMgr : IScheduleItemImplMgr
    {
        private string conStr = "";
        IScheduleItemImplDao _scheduleItemDao;
        public ScheduleItemMgr(string connectionStr)
        {
            _scheduleItemDao = new ScheduleItemDao(connectionStr);
            conStr = connectionStr;
        }

        public string Save(ScheduleItem si)
        {
            try
            {
                return _scheduleItemDao.Save(si);
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleItemMgr-->Save"+ ex.Message,ex);
            }
        }

        public List<ScheduleItemCustom> Query(ScheduleItem si)
        {
            try
            {
                return _scheduleItemDao.Query(si);
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleItemMgr-->Query"+ ex.Message,ex);;
            }
        }

        public string Update(ScheduleItem si)
        {
            try
            {
                return _scheduleItemDao.Update(si);
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleItemMgr-->Update"+ ex.Message,ex);;
            }
        }

        public bool Delete(int schedule, string ids, string item_type, string item_value)
        {
            try
            {
                ArrayList arrayList = new ArrayList();
                //獲得供應商對應旗下的商品id集合
                IProductImplDao _productDao = new ProductDao(conStr);
                IScheduleRelationImplDao _scheduleDao = new ScheduleRelationDao(conStr);
                List<Product> listProduct = _productDao.GetProductByVendor(Convert.ToInt32(item_value));
                string id = "";
                foreach (var item in listProduct) //連接id
                {
                    id += item.Product_Id + ",";
                }
                id = id.Remove(id.Length - 1, 1);

                arrayList.Add(_scheduleDao.Delete(item_type, id, schedule));
                arrayList.Add(_scheduleItemDao.Delete(schedule, ids));
                MySqlDao sqlDao = new MySqlDao(conStr);
                return sqlDao.ExcuteSqls(arrayList);
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleItemMgr-->Delete" + ex.Message, ex); ;
            }
        }

        public List<ScheduleItemCustom> QueryByCondition(ScheduleItem si)
        {
            try
            {
                return _scheduleItemDao.QueryByCondition(si);
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleItemMgr-->QueryByCondition" + ex.Message, ex); ;
            }
        }

        /// <summary>
        /// 批量對數據進行更改和添加操作
        /// </summary>
        public bool UpdateByBacth(List<ScheduleItem> lists)
        {
            ArrayList listStr = new ArrayList();
            try
            {
                foreach(ScheduleItem s in lists)
                {
                    switch(s.id)
                    {
                        case 0:
                            listStr.Add(Save(s));
                            break;
                        default:
                            listStr.Add(Update(s));
                            break;
                    }
                }

                MySqlDao mysql = new MySqlDao(conStr);
                return mysql.ExcuteSqls(listStr);
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleItemMgr-->UpdateByBacth" + ex.Message,ex);
            }
        }
    }
}
