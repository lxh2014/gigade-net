using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace BLL.gigade.Mgr
{
    public class AppversionsMgr : IAppversionsImplMgr
    {
        private IAppversionsImplDao _iappversionsImplDao;
        public AppversionsMgr(string connectionStr)
        {
            _iappversionsImplDao = new AppversionsDao(connectionStr);
        }
        #region 上架版本方法(肖國棟 2015.8.25)
        //查詢分頁列表
        public string GetAppversionsList(Model.AppversionsQuery appsions)
        {
            try
            {
                string Json = string.Empty;
                int totalCount;
                //調用查詢方法
                List<Model.AppversionsQuery> liAppversionsQuery = _iappversionsImplDao.GetAppversionsList(appsions, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                Json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(liAppversionsQuery, Formatting.Indented, timeConverter) + "}";//返回json數據
                return Json;
            }
            catch (Exception ex)
            {
                throw new Exception("AppversionsMgr-->GetAppversionsList-->" + ex.Message, ex);
            }
        }
        //通過ID刪除
        public string DeleteAppversionsById(string id)
        {
            try
            {
                string Json = string.Empty;
                //調用刪除方法
                int backRows = _iappversionsImplDao.DeleteAppversionsById(id);
                //返回json數據
                if (backRows > 0)
                {
                    Json = "{success:true}";
                }
                else
                {
                    Json = "{success:false}";
                }
                return Json;
            }
            catch (Exception ex)
            {
                throw new Exception("AppversionsMgr-->DeleteAppversionsById-->" + ex.Message, ex);
            }
        }
        //編輯事件
        public string EditAppversionsInfo(Model.AppversionsQuery anpq) 
        {
            try
            {
                string Json = string.Empty;
                string msgstr = "保存失敗";
                //判斷是插入還是修改,為空為插入
                string isAddOrEidt = anpq.isAddOrEidt;
                if (string.IsNullOrEmpty(isAddOrEidt))
                {
                    if (_iappversionsImplDao.AddAppversions(anpq) > 0)
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
