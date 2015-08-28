using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class ArrivalNoticeMgr : IArrivalNoticeImplMgr
    {
      private IArrivalNoticeImplDao _IArrivalNoticeDao;
      private MySqlDao _mysqlDao;
      public ArrivalNoticeMgr(string connectionString)
      {
          _IArrivalNoticeDao = new ArrivalNoticeDao(connectionString);
          _mysqlDao = new MySqlDao(connectionString);
      }


      public List<ArrivalNoticeQuery> ArrivalNoticeList(ArrivalNoticeQuery query, out int totalCount)
      {
          try
{
              return _IArrivalNoticeDao.ArrivalNoticeList(query,out totalCount);
          }
          catch (Exception ex)
          {
              throw new Exception("ArrivalNoticeMgr-->ArrivalNoticeList-->"+ex.Message,ex);
          }
      }


      public bool IgnoreNotice(List<ArrivalNoticeQuery> list)
      {
          ArrayList arrList = new ArrayList();
          string mailBody = string.Empty;
          MailHelper mailHelper = new MailHelper();
          try
          {
              for (int i = 0; i < list.Count; i++)
              {
                  arrList.Add(_IArrivalNoticeDao.IgnoreNotice(list[i]));
              }
              //_mysqlDao.ExcuteSqls(arrList)
              if (_mysqlDao.ExcuteSqls(arrList))
              {
                  return true;
              }
              else
              {
                  return false;
              }
           
          }
          catch (Exception ex)
          {
              throw new Exception("ArrivalNoticeMgr-->IgnoreNotice-->" + arrList.ToString() + ex.Message, ex);
          }
      }
    }
}
