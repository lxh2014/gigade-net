using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class EdmListConditoinSubMgr
    {
        private EdmListConditoinSubDao _edmlistsubDao;
        private EdmListConditionMainDao _edmlistmainDao;
        public EdmListConditoinSubMgr(string connectionString) 
        {
            _edmlistsubDao = new EdmListConditoinSubDao(connectionString);
            _edmlistmainDao = new EdmListConditionMainDao(connectionString);
        }
        public int SaveListInfoCondition(EdmListConditoinSubQuery query)
        {
            try
            {
                int i = _edmlistsubDao.SaveListInfoCondition(query);               
                return i;
            }
            catch (Exception ex)
            {
                throw new Exception(" EdmListConditoinSubMgr-->SaveListInfoCondition " + ex.Message, ex);
            }
        }
        public List<EdmListConditoinSubQuery> LoadCondition(EdmListConditoinSubQuery query)
        {
            EdmListConditionMain model = new EdmListConditionMain();
            List<EdmListConditoinSub> result = new List<EdmListConditoinSub>();
            List<EdmListConditoinSubQuery> store = new List<EdmListConditoinSubQuery>();
            try
            {
                model = _edmlistmainDao.SelectElcmIDByConditionName(query.elcm_name);
                if (model != null)
                {
                    query.elcm_id = model.elcm_id;
                    result = _edmlistsubDao.LoadCondition(query);
                    if (result != null)
                    {
                        EdmListConditoinSubQuery q = new EdmListConditoinSubQuery();
                        store.Add(q);
                        #region  保存條件狀態
                        for (int i = 0; i < result.Count; i++)
                        {
                            string key = result[i].elcs_key;                        
                            switch (key)
                            {
                                case "gender":
                                    store[0].chkGender = true;
                                    if (!string.IsNullOrEmpty(result[i].elcs_value1))
                                    {
                                        store[0].genderCondition = Convert.ToInt32(result[i].elcs_value1);
                                    }
                                    break;
                                case "buy_times":
                                    store[0].ChkBuy = true;
                                    if (Convert.ToInt32(result[i].elcs_value1) != 0)
                                    {
                                        store[0].buyCondition = Convert.ToInt32(result[i].elcs_value1);
                                    }
                                    if (Convert.ToInt32(result[i].elcs_value2) != 0)
                                    {
                                        store[0].buyTimes = Convert.ToInt32(result[i].elcs_value2);
                                    }
                                    if (!string.IsNullOrEmpty(result[i].elcs_value3))
                                    {
                                        if (Convert.ToDateTime(result[i].elcs_value3) != DateTime.MinValue)
                                        {
                                            store[0].buyTimeMin = Convert.ToDateTime(result[i].elcs_value3);
                                        }
                                        if (Convert.ToDateTime(result[i].elcs_value4) != DateTime.MinValue)
                                        {
                                            store[0].buyTimeMax = Convert.ToDateTime(result[i].elcs_value4);
                                        }
                                    }
                                    break;
                                case "age":
                                    store[0].ChkAge = true;
                                    if (Convert.ToInt32(result[i].elcs_value1) != 0)
                                    {
                                        store[0].ageMin = Convert.ToInt32(result[i].elcs_value1);
                                    }
                                    if (Convert.ToInt32(result[i].elcs_value2) != 0)
                                    {
                                        store[0].ageMax = Convert.ToInt32(result[i].elcs_value2);
                                    }
                                    break;
                                case "cancel_times":
                                    store[0].ChkCancel = true;
                                    if (Convert.ToInt32(result[i].elcs_value1) != 0)
                                    {
                                        store[0].cancelCondition = Convert.ToInt32(result[i].elcs_value1);
                                    }
                                    if (Convert.ToInt32(result[i].elcs_value2) != 0)
                                    {
                                        store[0].cancelTimes = Convert.ToInt32(result[i].elcs_value2);
                                    }
                                    if (!string.IsNullOrEmpty(result[i].elcs_value3))
                                    {
                                        if (Convert.ToDateTime(result[i].elcs_value3) != DateTime.MinValue)
                                        {
                                            store[0].cancelTimeMin = Convert.ToDateTime(result[i].elcs_value3);
                                        }
                                        if (Convert.ToDateTime(result[i].elcs_value4) != DateTime.MinValue)
                                        {
                                            store[0].cancelTimeMax = Convert.ToDateTime(result[i].elcs_value4);
                                        }
                                    }
                                    break;
                                case "register_time":
                                    store[0].ChkRegisterTime = true;
                                    if (!string.IsNullOrEmpty(result[i].elcs_value1))
                                    {
                                        if (Convert.ToDateTime(result[i].elcs_value1) != DateTime.MinValue)
                                        {
                                            store[0].registerTimeMin = Convert.ToDateTime(result[i].elcs_value1);
                                        }
                                        if (Convert.ToDateTime(result[i].elcs_value2) != DateTime.MinValue)
                                        {
                                            store[0].registerTimeMax = Convert.ToDateTime(result[i].elcs_value2);
                                        }
                                    }
                                    break;
                                case "return_times":
                                    store[0].ChkReturn = true;
                                    if (Convert.ToInt32(result[i].elcs_value1) != 0)
                                    {
                                        store[0].returnCondition = Convert.ToInt32(result[i].elcs_value1);
                                    }
                                    if (Convert.ToInt32(result[i].elcs_value2) != 0)
                                    {
                                        store[0].returnTimes = Convert.ToInt32(result[i].elcs_value2);
                                    }
                                    if (!string.IsNullOrEmpty(result[i].elcs_value3))
                                    {
                                        if (Convert.ToDateTime(result[i].elcs_value3) != DateTime.MinValue)
                                        {
                                            store[0].returnTimeMin = Convert.ToDateTime(result[i].elcs_value3);
                                        }
                                        if (Convert.ToDateTime(result[i].elcs_value4) != DateTime.MinValue)
                                        {
                                            store[0].returnTimeMax = Convert.ToDateTime(result[i].elcs_value4);
                                        }
                                    }
                                    break;
                                case "last_order":
                                    store[0].ChkLastOrder = true;
                                    if (!string.IsNullOrEmpty(result[i].elcs_value1))
                                    {
                                        if (Convert.ToDateTime(result[i].elcs_value1) != DateTime.MinValue)
                                        {
                                            store[0].lastOrderMin = Convert.ToDateTime(result[i].elcs_value1);
                                        }
                                        if (Convert.ToDateTime(result[i].elcs_value2) != DateTime.MinValue)
                                        {
                                            store[0].lastOrderMax = Convert.ToDateTime(result[i].elcs_value2);
                                        }
                                    }
                                    break;
                                case "replenishment_info":
                                    store[0].ChkNotice = true;
                                    if (!string.IsNullOrEmpty(result[i].elcs_value1))
                                    {
                                        if (Convert.ToInt32(result[i].elcs_value1) != 0)
                                        {
                                            store[0].noticeCondition = Convert.ToInt32(result[i].elcs_value1);
                                        }
                                        if (Convert.ToInt32(result[i].elcs_value2) != 0)
                                        {
                                            store[0].noticeTimes = Convert.ToInt32(result[i].elcs_value2);
                                        }
                                    }
                                    break;
                                case "last_login":
                                    store[0].ChkLastLogin = true;
                                    if (!string.IsNullOrEmpty(result[i].elcs_value1))
                                    {
                                        if (Convert.ToDateTime(result[i].elcs_value1) != DateTime.MinValue)
                                        {
                                            store[0].lastLoginMin = Convert.ToDateTime(result[i].elcs_value1);
                                        }
                                        if (Convert.ToDateTime(result[i].elcs_value2) != DateTime.MinValue)
                                        {
                                            store[0].lastLoginMax = Convert.ToDateTime(result[i].elcs_value2);
                                        }
                                    }
                                    break;
                                case "total_consumption":
                                    store[0].ChkTotalConsumption = true;
                                    if (!string.IsNullOrEmpty(result[i].elcs_value1))
                                    {
                                        if (Convert.ToInt32(result[i].elcs_value1) != 0)
                                        {
                                            store[0].totalConsumptionMin = Convert.ToInt32(result[i].elcs_value1);
                                        }
                                        if (Convert.ToInt32(result[i].elcs_value2) != 0)
                                        {
                                            store[0].totalConsumptionMax = Convert.ToInt32(result[i].elcs_value2);
                                        }
                                    }
                                    break;
                                case "black_list":
                                    store[0].ChkBlackList = true;
                                    break;
                            }
                        } 
                        #endregion
                    }
                }
                return store;
            }
            catch (Exception ex)
            {
                throw new Exception(" EdmListConditoinSubMgr-->LoadCondition " + ex.Message, ex);
            }
        }
    }
}
