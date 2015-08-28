using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Working;

namespace BLL.gigade.Mgr
{
    public class ScheduleMgr : IScheduleImplMgr
    {
        private Dao.Impl.IScheduleImplDao _scheduleDao;
        private string connectionString = "";
        public ScheduleMgr(string connectionString)
        {
            this.connectionString = connectionString;
            _scheduleDao = new Dao.ScheduleDao(connectionString);
        }

        //add by wwei0216w 2015/2/9
        /// <summary>
        /// 保存Freight排程的執行信息
        /// </summary>
        /// <param name="fst">FreightSetTime對象</param>
        /// <returns>執行后受影響的行數</returns>
        public bool Save(Schedule s)
        {
            try
            {
                IScheduleRelationImplMgr _srMgr = new ScheduleRelationMgr(connectionString);
                if (s.schedule_id == 0)
                    return _scheduleDao.Save(s) > 0;
                else
                {
                    return _scheduleDao.Update(s) > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleMgr-->Save" + ex.Message, ex);
            }
        }

        //add by wwei0216w 2015/2/10
        /// <summary>
        /// 查詢排成的執行信息
        /// </summary>
        /// <param name="fst">FreightSetTime對象</param>
        /// <returns>FreightSetTime的集合</returns>
        public List<Schedule> Query(Model.Query.ScheduleQuery s)
        {
            try
            {
                return _scheduleDao.Query(s);
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleMgr-->Query" + ex.Message, ex);
            }
        }

        ////add by wwei0216w 2015/2/25
        ///// <summary>
        ///// 為排程賦予product_id
        ///// </summary>
        ///// <returns>受影響的行數</returns>
        //public int UpdateProductId(uint schedule_id)
        //{
        //    try
        //    {
        //        return _scheduleDao.UpdateProductId(schedule_id);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("FreightSetTimeMgr-->UpdateProductId" + ex.Message,ex);
        //    }
        //}

        //add by wwei0216w 2015/2/25
        /// <summary>
        /// 根據商品Id刪除排程
        /// </summary>
        /// <param name="Schedule">Schedule</param>
        /// <returns>受影響的行數</returns>
        public bool Delete(params int[] ids)
        {
            try
            {
                return _scheduleDao.Delete(ids) >= 0;
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleMgr-->Delete" + ex.Message, ex);
            }
        }


        public List<Parametersrc> GetRelevantInfo(string path,string type)
        {
            ///獲得key的值,key值一般為 供應商,品牌,或者其他類型
            try
            {
                StringBuilder sb = new StringBuilder();
                IParametersrcImplMgr _parameteMgr = new ParameterMgr(path, ParaSourceType.XML);
                //return _parameteMgr.QueryUsed(new Parametersrc { ParameterType = "ScheduleType" });
                return _parameteMgr.QueryUsed(new Parametersrc { ParameterType = type });
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleMgr-->GetRelevantInfo" + ex.Message,ex);
            }

        }

        public List<Parametersrc> GetValue(int keyValue)
        {
            ///獲得value值,value值一般為根據key值所得到的信息集合,例如供應商,則查詢所有供應商的信息,
            ///品牌則查詢所有品牌信息
            List<Parametersrc> listParametersrc = new List<Parametersrc>();
            try
            {
                switch (keyValue)
                { 
                    case 1:
                        IVendorImplMgr _vendorMge = new VendorMgr(connectionString);
                        List<Vendor> list = _vendorMge.VendorQueryAll(new Vendor());
                        list = list.FindAll(m => m.vendor_status == 1);
                        foreach (var v in list)
                        {
                            listParametersrc.Add(new Parametersrc { ParameterCode = v.vendor_id.ToString(),parameterName = v.vendor_name_full});
                        }
                        return listParametersrc;
                    default:
                        return listParametersrc;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleMgr-->GetValue" + ex.Message, ex);
            }
        }
    }
}
