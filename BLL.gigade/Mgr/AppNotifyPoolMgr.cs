using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using Newtonsoft.Json;
using BLL.gigade.Model.Query;
using Newtonsoft.Json.Converters;

namespace BLL.gigade.Mgr
{
    public class AppNotifyPoolMgr : IAppNotifyPoolImplMgr
    {
        private IAppNotifyPoolImplDao _iapppoolImplDao;
        public AppNotifyPoolMgr(string connectionStr)
        {
            _iapppoolImplDao = new AppNotifyPoolDao(connectionStr);
        }
        #region 推播設定方法(肖國棟 2015.8.21)
        /// <summary>
        /// 通過開始時間和結束時間獲取推播設定
        /// </summary>
        /// <param name="valid_start"></param>
        /// <param name="valid_end"></param>
        /// <returns></returns>
        public string GetAppnotifypool(AppNotifyPool ap)
        {
            try
            {
                string Json = string.Empty;
                int totalCount;
                //調用查詢方法
                List<AppNotifyPoolQuery> liAppNotifyPool = _iapppoolImplDao.GetAppnotifypool(ap, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                Json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(liAppNotifyPool, Formatting.Indented, timeConverter) + "}";//返回json數據
                return Json;
            }
            catch (Exception ex)
            {
                throw new Exception("AppNotifyPoolMgr-->GetAppnotifypool-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 編輯方法
        /// </summary>
        /// <param name="anpq"></param>
        /// <returns></returns>
        public string EditAppNotifyPoolInfo(AppNotifyPoolQuery anpq)
        {
            try
            {
                string Json = string.Empty;
                string msgstr = "保存失敗";
                //判斷是插入還是修改,為空為插入
                string isAddOrEidt = anpq.isAddOrEidt;
                if (string.IsNullOrEmpty(isAddOrEidt))
                {
                    if (_iapppoolImplDao.AddAppnotifypool(anpq) > 0)
                    {
                        msgstr = "保存成功";
                    }
                }
                Json = "{success:true,msg:'" + msgstr + "'}";
                return Json;
            }
            catch (Exception ex)
            {
                throw new Exception("AppNotifyPoolMgr-->EditAppNotifyPoolInfo-->" + ex.Message, ex);
            }
        }
        #endregion
    }
}
