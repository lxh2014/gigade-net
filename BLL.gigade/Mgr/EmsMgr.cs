using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using System.Collections;

namespace BLL.gigade.Mgr
{
   public class EmsMgr : IEmsImplMgr
    {
       private IEmsImplDao _IEmsDao;
       private EmsDao _emsDao;
       public EmsMgr(string connectionString)
       {
           _IEmsDao = new EmsDao(connectionString);
           _emsDao = new EmsDao(connectionString);
       }
        public List<Model.Query.EmsGoalQuery> GetEmsGoalList(Model.Query.EmsGoalQuery query, out int totalCount)
        {

            try
            {
                return _IEmsDao.GetEmsGoalList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("EmsMgr-->GetEmsGoalList-->"+ex.Message,ex);
            }
        }


        public List<Model.Query.EmsGoalQuery> GetDepartmentStore()
        {
            try
            {
                return _IEmsDao.GetDepartmentStore();
            }
            catch (Exception ex)
            {
                throw new Exception("EmsMgr-->GetDepartmentStore-->"+ex.Message,ex);
            }
        }


        public string SaveEmsGoal(Model.Query.EmsGoalQuery query)
        {
            string json = string.Empty;
            try
            {

                if (VerifyData(query) > 0)//重複數據
                {
                    json = "{success:true,msg:0}";//不可添加數據
                }
                else
                {
                    if (_IEmsDao.SaveEmsGoal(query) > 0)
                    {
                        json = "{success:true,msg:1}";//新增成功
                    }
                    else
                    {
                        json = "{success:true,msg:2}";//新增失敗
                    }
                }
                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("EmsMgr-->SaveEmsGoal-->" + ex.Message, ex);
            }
        }


        public List<EmsActualQuery> GetEmsActualList(EmsActualQuery query, out int totalCount)
        {
            try
            {
                //列表頁數據查詢出來之前判斷前一天數據是否存在，不存在則插入默認值
                insertPreDate(query);
                return _IEmsDao.GetEmsActualList(query,out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("EmsMgr-->GetEmsActualList-->" + ex.Message, ex);
            }
        }

        public bool insertPreDate(EmsActualQuery query)
        {
            StringBuilder sqlInsert = new StringBuilder();
            try
            {
                List<EmsGoalQuery> store = new List<EmsGoalQuery>();
                store = _IEmsDao.GetDepartmentStore();
                ArrayList arrList = new ArrayList();
                for (int i = 1; i <= query.predate.Day; i++)
                {
                    for (int j = 0; j < store.Count; j++)
                    {
                        query.day = i;
                        query.department_code_insert = store[j].department_code;
                        if (_IEmsDao.IsExist(query) == 0)
                        {
                            arrList.Add(_IEmsDao.insertSql(query));
                        }
                    }
                }
                return _emsDao.execInsertSql(arrList);
            }
            catch (Exception ex)
            {
                throw new Exception("EmsMgr-->insertPreDate-->" + ex.Message, ex);
            }
        }


        public int VerifyData(Model.Query.EmsGoalQuery query)
        {
            try
            {
                return _IEmsDao.VerifyData(query);
            }
            catch (Exception ex)
            {
                throw new Exception("EmsMgr-->VerifyData-->" + ex.Message, ex);
            }
        }


        public string EditEmsGoal(Model.Query.EmsGoalQuery query)
        {
            string json = string.Empty;

            try
            {
                if (_IEmsDao.EditEmsGoal(query) > 0)
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
                throw new Exception("EmsMgr-->EditEmsGoal-->" + ex.Message, ex);
            }
        }


        public string EditEmsActual(Model.Query.EmsActualQuery query)
        {
            string json = string.Empty;
            try
            {
                if (query.EmsActual == "cost_sum")//成本
                {
                    if (_emsDao.CostSumEmsActual(query) > 0)
                    {
                        json = "{success:true}";
                    }
                    else
                    {
                        json = "{success:false}";
                    }
                }
                else if (query.EmsActual == "order_count")//訂單總數
                {
                     if ( _emsDao.OrderCountEmsActual(query)> 0)
                     {
                         json = "{success:true}";
                     }
                     else
                     {
                         json = "{success:false}";
                     }
                }
                else//累計實績
                {
                     if (_emsDao.AmountSumEmsActual(query)> 0)
                     {
                         json = "{success:true}";
                     }
                     else
                     {
                         json = "{success:false}";
                     }
                }
                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("EmsMgr-->EditEmsActual-->" + ex.Message, ex);
            }
        
        }


        public string SaveEmsActual(Model.Query.EmsActualQuery query)
        {
            string json = string.Empty;
            try
            {
                if (query.department_code == "通路發展部")
                {
                    query.department_code = "cdp_dep";
                }
                if (query.department_code == "cdp_dep")
                {
                    query.type = 2;
                }
                string time = query.year + "-" + query.month + "-" + query.day;
                DateTime dtime = DateTime.Now;
                DateTime sTime = DateTime.Now;
                bool isTime = DateTime.TryParse(time, out dtime);//是否是正常時間 是 true 否  false
                if (!isTime)//不是正常時間
                {
                    json = "{success:true,msg:3}";//選擇時間有誤
                }
                else//如果是正常時間判斷是否大於當前時間
                {
                    if (sTime < Convert.ToDateTime(time))//如果大於當前時間
                    {
                        json = "{success:true,msg:4}";//選擇時間有誤
                    }
                    else
                    {
                        if (_IEmsDao.VerifyActualData(query) > 0)
                        {
                            json = "{success:true,msg:0}";//不可添加數據
                        }
                        else
                        {
                            if (_IEmsDao.SaveEmsActual(query) > 0)
                            {
                                json = "{success:true,msg:1}";//新增成功
                            }
                            else
                            {
                                json = "{success:true,msg:2}";//新增失敗
                            }
                        }
                    }
                }
                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("EmsMgr-->SaveEmsActual-->" + ex.Message, ex);
            }
        }


        public int VerifyActualData(Model.Query.EmsActualQuery query)
        {
            try
            {
                return _IEmsDao.VerifyActualData(query);
            }
            catch (Exception ex)
            {
                throw new Exception("EmsMgr-->VerifyActualData-->" + ex.Message, ex);
            }
        }

    }
}
