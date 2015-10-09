using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class EdmListConditionMainMgr
    {
        private EdmListConditionMainDao _edmlistmainDao;
        private EdmListConditoinSubDao _edmlistsubDao;
        public EdmListConditionMainMgr(string connectionString)
        {
            _edmlistmainDao = new EdmListConditionMainDao(connectionString);
            _edmlistsubDao = new EdmListConditoinSubDao(connectionString);
        }
        public List<EdmListConditionMain> GetConditionList()
        {
            try
            {
                return _edmlistmainDao.GetConditionList();
            }
            catch (Exception ex)
            {
                throw new Exception(" EdmListConditionMainMgr-->GetConditionList " + ex.Message, ex);
            }
        }
        public int DeleteListInfo(EdmListConditionMain query)
        {
            EdmListConditionMain model = new EdmListConditionMain();
            try
            {
                model = _edmlistmainDao.SelectElcmIDByConditionName(query.elcm_name);
                query.elcm_id = model.elcm_id;
                return _edmlistmainDao.DeleteListInfo(query);
            }
            catch (Exception ex)
            {
                throw new Exception(" EdmListConditionMainMgr-->DeleteListInfo " + ex.Message, ex);
            }
        }
        public int SaveListInfoName(EdmListConditionMain query, out int id,out int msg)
        {
            try
            {
                EdmListConditionMain idModel = new EdmListConditionMain();       
                EdmListConditionMain model = new EdmListConditionMain();
                idModel = _edmlistmainDao.SelectElcmIDByConditionName(query.elcm_name);
                msg = 0;
                id = 0;
                if (idModel == null)
                {
                    query.elcm_created = DateTime.Now;
                    int i = _edmlistmainDao.SaveListInfoName(query);
                    if (i > 0)
                    {
                        model = _edmlistmainDao.SelectElcmIDByConditionName(query.elcm_name);
                        id = model.elcm_id;
                    }
                    return i;
                }
                else
                {
                    msg = 1; //篩選條件名已存在
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(" EdmListConditionMainMgr-->SaveListInfoName " + ex.Message, ex);
            }
        }
        public int UpdateCondition(EdmListConditoinSubQuery query)
        {
            int i = 0;
            try
            {                
                EdmListConditionMain model = new EdmListConditionMain();
                model = _edmlistmainDao.SelectElcmIDByConditionName(query.elcm_name);
                if (model != null)
                {
                    query.elcm_id = model.elcm_id;
                    _edmlistsubDao.DeleteInfo(query);
                     i = _edmlistsubDao.SaveListInfoCondition(query);
                }
                return i;
            }
            catch (Exception ex)
            {
                throw new Exception(" EdmListConditionMainMgr-->UpdateCondition " + ex.Message, ex);
            }
        }
        public DataTable GetUserNum(EdmListConditoinSubQuery q)
        {
            try
            {
                return _edmlistmainDao.GetUserNum(q);
            }
            catch (Exception ex)
            {
                throw new Exception(" EdmListConditionMainMgr-->GetUserNum " + ex.Message, ex);
            }
        }

        /// <summary>
        /// EDM電子報發送mail
        /// </summary>
        /// <param name="q">elcm_id</param>
        /// <returns>email</returns>
        public DataTable GetUserEmail(int elcm_id)
        {
            try
            {
                EdmListConditoinSubQuery query = new EdmListConditoinSubQuery();
                query.elcm_id=elcm_id;
                DateTime dt;
                List<EdmListConditoinSub> edm = _edmlistsubDao.LoadCondition(query);
                foreach (var item in edm)
                {
                    if (item.elcs_key == "gender")
                    {
                        query.chkGender = true;
                        query.genderCondition =  int.Parse(item.elcs_value1.ToString());
                    }
                    if (item.elcs_key == "buy_times")
                    {
                        query.ChkBuy = true;
                        query.buyCondition = int.Parse(item.elcs_value1.ToString());
                        query.buyTimes = int.Parse(item.elcs_value2.ToString());
                        if (DateTime.TryParse(item.elcs_value3.ToString(), out dt))
                        {
                            query.buyTimeMin = dt;
                        }
                        if (DateTime.TryParse(item.elcs_value4.ToString(), out dt))
                        {
                            query.buyTimeMax = dt;
                        }
                    }
                    if (item.elcs_key == "age")
                    {
                        query.ChkAge = true;
                        query.ageMin = Convert.ToInt32(item.elcs_value1.ToString());
                        query.ageMax = Convert.ToInt32(item.elcs_value2.ToString());
                    }
                    if (item.elcs_key == "cancel_times")
                    {
                        query.ChkCancel = true;
                        query.cancelCondition = Convert.ToInt32(item.elcs_value1.ToString());
                        query.cancelTimes = Convert.ToInt32(item.elcs_value2.ToString());
                        if (DateTime.TryParse(item.elcs_value3.ToString(), out dt))
                        {
                            query.cancelTimeMin = dt;
                        }
                        if (DateTime.TryParse(item.elcs_value4.ToString(), out dt))
                        {
                            query.cancelTimeMax = dt;
                        }

                    }
                    if (item.elcs_key == "register_time")
                    {
                        query.ChkRegisterTime = true;
                        if (DateTime.TryParse(item.elcs_value1.ToString(), out dt))
                        {
                            query.registerTimeMin = dt;
                        }
                        if (DateTime.TryParse(item.elcs_value2.ToString(), out dt))
                        {
                            query.registerTimeMax = dt;
                        }

                    }
                    if (item.elcs_key == "return_times")
                    {
                        query.ChkReturn = true;
                        query.returnCondition = Convert.ToInt32(item.elcs_value1.ToString());
                        query.returnTimes = Convert.ToInt32(item.elcs_value2.ToString());
                        if (DateTime.TryParse(item.elcs_value3.ToString(), out dt))
                        {
                            query.returnTimeMin = dt;
                        }
                        if (DateTime.TryParse(item.elcs_value4.ToString(), out dt))
                        {
                            query.returnTimeMax = dt;
                        }
                    }
                    if (item.elcs_key == "last_order")
                    {
                        query.ChkLastOrder = true;
                        if (DateTime.TryParse(item.elcs_value3.ToString(), out dt))
                        {
                            query.lastOrderMin = dt;
                        }
                        if (DateTime.TryParse(item.elcs_value4.ToString(), out dt))
                        {
                            query.lastOrderMax = dt;
                        }
                    }
                    if (item.elcs_key == "replenishment_info")
                    {
                        query.ChkNotice = true;
                        query.noticeCondition = Convert.ToInt32(item.elcs_value1.ToString());
                        query.noticeTimes = Convert.ToInt32(item.elcs_value2.ToString());
                    }
                    if (item.elcs_key == "last_login")
                    {
                        query.ChkLastLogin = true;
                        if (DateTime.TryParse(item.elcs_value3.ToString(), out dt))
                        {
                            query.lastLoginMin = dt;
                        }
                        if (DateTime.TryParse(item.elcs_value4.ToString(), out dt))
                        {
                            query.lastLoginMax = dt;
                        }
                    }
                    if (item.elcs_key == "total_consumption")
                    {
                        query.ChkTotalConsumption = true;
                        query.totalConsumptionMin = Convert.ToInt32(item.elcs_value1.ToString());
                        query.totalConsumptionMax = Convert.ToInt32(item.elcs_value2.ToString());
                    }
                    if (item.elcs_key == "black_list")
                    {
                        query.ChkBlackList = true;
                    }
                }
                return _edmlistmainDao.GetUserNum(query);
            }
            catch (Exception ex)
            {
                throw new Exception(" EdmListConditionMainMgr-->GetUserNum " + ex.Message, ex);
            }
        }
    }
}
