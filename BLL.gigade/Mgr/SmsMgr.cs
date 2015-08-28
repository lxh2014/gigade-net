using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class SmsMgr:ISmsImplMgr
    {
         private ISmsImplDao _SmsDao;

         public SmsMgr(string connectionString)
        {
            _SmsDao = new SmsDao(connectionString);
        }
         public List<SmsQuery> GetSmsList(SmsQuery query, out int totalcount)
        {
            try
            {
                return _SmsDao.GetSmsList(query, out totalcount);
            }
            catch (Exception ex)
            {

                throw new Exception("SmsMgr-->GetSmsList-->" + ex.Message, ex);
            }
        }
         public int updateSms(SmsQuery query)
         {
             try
{
                 return _SmsDao.updateSms(query);
             }
             catch (Exception ex)
    {

                 throw new Exception("SmsMgr-->updateSms-->" + ex.Message, ex);
             }
         }

         #region 客服管理=>聯絡客服列表
         /// <summary>
         /// 新增sms表數據
         /// </summary>
         /// <param name="query">新增信息</param>
         /// <returns></returns>
         public int InsertSms(SmsQuery query)
         {
             try
             {
                 return _SmsDao.InsertSms(query);
             }
             catch (Exception ex)
             {

                 throw new Exception("SmsMgr-->InsertSms-->" + ex.Message, ex);
             }
         }
         #endregion
    }
}
