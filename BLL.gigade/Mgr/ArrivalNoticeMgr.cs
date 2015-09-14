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
using BLL.gigade.Model;

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
      public List<ArrivalNoticeQuery> GetArrNoticeList(ArrivalNoticeQuery query, out int totalCount)
      {
          try
          {
              List<ArrivalNoticeQuery> store = new List<ArrivalNoticeQuery>();
              store = _IArrivalNoticeDao.GetArrNoticeList(query, out totalCount);
              foreach (var item in store)
              {
                  item.product_spec = item.spec_title_1;
                  item.product_spec += string.IsNullOrEmpty(item.spec_title_1) ? item.spec_title_2 : (string.IsNullOrEmpty(item.spec_title_2) ? "" : " / " + item.spec_title_2);
                  //if ((!string.IsNullOrEmpty(item.spec_title_1)) || (!string.IsNullOrEmpty(item.spec_title_2)))
                  //{
                  //    item.product_spec = item.spec_title_1 + " / " + item.spec_title_2;
                  //}
              }
              return store;
          }
          catch (Exception ex)
          {
              throw new Exception("ArrivalNoticeMgr->GetArrNoticeList" + ex.Message);
          }
      }
      public List<ArrivalNoticeQuery> ShowArrByUserList(ArrivalNoticeQuery query, out int totalCount)
      {
          try
          {
              List<ArrivalNoticeQuery> store = new List<ArrivalNoticeQuery>();
              store = _IArrivalNoticeDao.ShowArrByUserList(query, out totalCount);
              foreach (var item in store)
              {
                  //  item.screate_time = CommonFunction.GetNetTime(item.create_time).ToString("yyyy-MM-dd HH:mm:ss");//一種顯示的格式
                  item.screate_time = CommonFunction.GetNetTime(item.create_time).ToString("yyyy-MM-dd");//把後台的int型的日期數據轉化為前台的string型的
                  item.ssend_notice_time = CommonFunction.GetNetTime(item.send_notice_time).ToString("yyyy-MM-dd");//把後台的int型的日期數據轉化為string型的 
                  // 用戶狀態  1: 已通知  0:未通知  2:取消通知
                  if (item.user_status == 1)
                  {
                      item.sstatus = "已通知";
                  }
                  else if (item.user_status == 0)
                  {
                      item.sstatus = "未通知";
                  }
                  else if (item.user_status == 2)
                  {
                      item.sstatus = "取消通知";
                  }
                  //商品規格  規格1 規格2 多種規格時進行合併
                  if ((!string.IsNullOrEmpty(item.spec_title_1)) || (!string.IsNullOrEmpty(item.spec_title_2)))
                  {
                      item.product_spec = item.spec_title_1 + item.spec_title_2;
                  }
                  // source_type 訊息來源 1:前台 顯示的是user_name  2: 後台操作 顯示的是 manage表中的user_username
                  if (item.source_type == 1)
                  {
                      item.muser_name = item.user_name;// add by yachao1120j 2015-8-31
                  }
                  else if (item.source_type == 2)
                  {
                      item.muser_name = item.user_username;
                  }
              }
              return store;
          }
          catch (Exception ex)
          {
              throw new Exception("ReplenishmentInformStatisticsMgr->ShowArrByUserList" + ex.Message);
          }
      }
      public int SaveArrivaleNotice(ArrivalNotice query)
      {
          try
          {
              return  _IArrivalNoticeDao.SaveArrivaleNotice(query);
          }
          catch (Exception ex)
          {
              throw new Exception("ArrivalNoticeMgr-->SaveArrivaleNotice-->" +ex.Message, ex);
          }
      }
      public int UpArrivaleNoticeStatus(ArrivalNotice query)
      {
          try
          {
              return _IArrivalNoticeDao.UpArrivaleNoticeStatus(query);
          }
          catch (Exception ex)
          {
              throw new Exception("ArrivalNoticeMgr-->UpArrivaleNoticeStatus-->" + ex.Message, ex);
          }
      }

      public List<ArrivalNoticeQuery> GetInventoryQueryList(ArrivalNoticeQuery query, out int totalCount)
      {
          try
          {
              List<ArrivalNoticeQuery> store = new List<ArrivalNoticeQuery>();
              store = _IArrivalNoticeDao.GetInventoryQueryList(query, out totalCount);
              foreach (var item in store)
              {
                  item.product_spec = item.spec_title_1;
                  item.product_spec += string.IsNullOrEmpty(item.spec_title_1) ? item.spec_title_2 : (string.IsNullOrEmpty(item.spec_title_2) ? "" : " / " + item.spec_title_2);
                  if (item.product_status == 0)
                  {
                      item.product_status_string = "新建立商品";
                  }
                  if (item.product_status == 1)
                  {
                      item.product_status_string = "申請審核";
                  }
                  if (item.product_status == 2)
                  {
                      item.product_status_string = "審核通過";
                  }
                  if (item.product_status == 5)
                  {
                      item.product_status_string = "上架";
                  }
                  if (item.product_status == 6)
                  {
                      item.product_status_string = "下架";
                  }
                  if (item.product_status == 20)
                  {
                      item.product_status_string = "供應商新建商品";
                  }

                  if (item.ignore_stock == 0)
                  {
                      item.ignore_stock_string = "否";
                  }
                  if (item.ignore_stock == 1)
                  {
                      item.ignore_stock_string = "是";
                  }
              }

              return store;

          }
          catch (Exception ex)
          {
              throw new Exception("ArrivalNoticeMgr->GetInventoryQueryList" + ex.Message);
          }
      }
    }
}
