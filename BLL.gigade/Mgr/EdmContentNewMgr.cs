using BLL.gigade.Dao;
using BLL.gigade.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
  public class EdmContentNewMgr
    {

      private EdmContentNewDao _edmContentNewDao;
      private MySqlDao _mySql;
      private EdmListConditionMainMgr _edmListConditionMgr;
      public EdmContentNewMgr(string  connectionString)
      {
          _edmContentNewDao = new EdmContentNewDao(connectionString);
          _mySql = new MySqlDao(connectionString);
          _edmListConditionMgr = new EdmListConditionMainMgr(connectionString);
      }
      /// <summary>
      /// 電子報列表
      /// </summary>
      /// <param name="query"></param>
      /// <param name="totalCount"></param>
      /// <returns></returns>
      public List<EdmContentNew> GetECNList(EdmContentNew query, out int totalCount)
      {
          try
          {
              return _edmContentNewDao.GetECNList(query, out totalCount);
          }
          catch (Exception ex)
          {
              throw new Exception("EdmContentNewMgr-->GetECNList-->"+ex.Message,ex);
          }
      }
      /// <summary>
      /// 電子報類型
      /// </summary>
      /// <returns></returns>
      public List<EdmGroupNew> GetEdmGroupNewStore()
      {
          try
          {
              return _edmContentNewDao.GetEdmGroupNewStore();
          }
          catch (Exception ex)
          {
              throw new Exception("EdmContentNewMgr-->GetEdmGroupNewStore-->" + ex.Message, ex);
          }
      }
      /// <summary>
      /// 發件者設定
      /// </summary>
      /// <returns></returns>
      public List<MailSender> GetMailSenderStore()
      {
          try
          {
              return _edmContentNewDao.GetMailSenderStore();
          }
          catch (Exception ex)
          {
              throw new Exception("EdmContentNewMgr-->GetMailSenderStore-->" + ex.Message, ex);
          }
      }
    
      /// <summary>
      /// 電子報模版
      /// </summary>
      /// <returns></returns>
      public List<EdmTemplate> GetEdmTemplateStore()
      {
          try
          {
              return _edmContentNewDao.GetEdmTemplateStore();
          }
          catch (Exception ex)
          {
              throw new Exception("EdmContentNewMgr-->GetEdmTemplateStore-->" + ex.Message, ex);
          }
      }
      /// <summary>
      /// 電子報新增/編輯
      /// </summary>
      /// <param name="query"></param>
      /// <returns></returns>
      public string SaveEdmContentNew(EdmContentNew query)
      {
          int result = 0;
          string json = string.Empty;
          try
          {
              if (query.content_id == 0)//新增
              {
                result=  _edmContentNewDao.InsertEdmContentNew(query);
              }
              else
              {
                  result = _edmContentNewDao.UpdateEdmContentNew(query);
              }
              if (result > 0)
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
              throw new Exception("EdmContentNewMgr-->SaveEdmContentNew-->" + ex.Message, ex);
          }
      }

      public string MailAndRequest(EdmSendLog eslQuery, MailRequest MRquery)
      {
          string json = string.Empty;
          ArrayList arrList = new ArrayList();
          try
          {
              if (eslQuery.test_send_end)//測試發送
              {
                  arrList.Add(_edmContentNewDao.InsertEdmSendLog(eslQuery));
                  arrList.Add(_edmContentNewDao.InsertEmailRequest(MRquery));
              }
              else//正式發送
              {
                  #region 發送名單條件

                  DataTable _dt = _edmListConditionMgr.GetUserEmail(eslQuery.elcm_id);
                  List<string> st = new List<string>();
                  
                  #region 額外發送列表

                  if (MRquery.extra_send != "")
                  {
                     
                      string[] extra_send = MRquery.extra_send.Split('\n');
                      for (int i = 0; i < extra_send.Length; i++)
                      {
                          if (extra_send[i] != "")
                          {
                              int norepeat = 0;
                              #region 額外發送的時候看看是不是已經存在這個email了，存在則不加入
                              for (int j = 0; j < _dt.Rows.Count; j++)
                              {
                                  if (_dt.Rows[j]["user_email"].ToString() != extra_send[i])
                                  {
                                      norepeat++;
                                  }
                              }
                              if (norepeat == _dt.Rows.Count)//證明不重複
                              {
                                  DataRow dr = _dt.NewRow();
                                  dr["user_email"] = extra_send[i];
                                  _dt.Rows.Add(dr);
                              }
                              #endregion
                          }
                      }
                  }
                  #endregion
                  #region 額外排除列表
                  if (MRquery.extra_no_send != "")
                  {
                      string[] extra_no_send = MRquery.extra_no_send.Split('\n');

                      for (int i = 0; i < extra_no_send.Length; i++)
                      {
                          if (extra_no_send[i] != "")
                          {
                              for (int j = 0; j <_dt.Rows.Count; j++)
                              {
                                  if (_dt.Rows[j]["user_email"].ToString() == extra_no_send[i])
                                  {
                                      _dt.Rows.Remove(_dt.Rows[j]);
                                      _dt.AcceptChanges();
                                  }
                              }
                          }
                      }
                  }
                  #endregion
                  #endregion
                  if (MRquery.is_outer)
                  {
                  #region 包含非訂閱
                  DataTable _outDt = _edmContentNewDao.GetOuterCustomer();
                  #region 額外發送列表

                  if (MRquery.extra_send != "")
                  {
                      string[] extra_send = MRquery.extra_send.Split('\n');
                      for (int i = 0; i < extra_send.Length; i++)
                      {
                          if (extra_send[i] != "")
                          {
                              #region 額外發送的時候看看是不是已經存在這個email了，存在則不加入
                              int norepeat = 0;
                              for (int j = 0; j < _outDt.Rows.Count; j++)
                              {
                                  if (_outDt.Rows[j]["customer_email"].ToString() != extra_send[i])
                                  {
                                      norepeat++;
                                  }
                              }
                              if (norepeat == _outDt.Rows.Count)//證明不重複
                              {
                                  DataRow dr = _outDt.NewRow();
                                  dr["customer_email"] = extra_send[i];
                                  _outDt.Rows.Add(dr);
                              }
                              #endregion
                          }
                      }
                  }
                  #endregion
                  #region 額外排除列表
                  if (MRquery.extra_no_send != "")
                  {
                      string[] extra_no_send = MRquery.extra_no_send.Split('\n');

                      for (int i = 0; i < extra_no_send.Length; i++)
                      {
                          if (extra_no_send[i] != "")
                          {
                              for (int j = 0; j <_outDt.Rows.Count; j++)
                              {
                                  if (_outDt.Rows[j]["customer_email"].ToString() == extra_no_send[i])
                                  {
                                      _outDt.Rows.Remove( _outDt.Rows[j]);
                                      _outDt.AcceptChanges();
                                  }
                              }
                          }
                      }
                  }
                  #endregion
                  #region  去重
                      for (int i = 0; i < _outDt.Rows.Count; i++)
                      {
                          for (int j = 0; j < _dt.Rows.Count; j++)
                          {
                              if (_dt.Rows[j]["user_email"] == _outDt.Rows[i]["customer_email"])
                              {
                                  _dt.Rows.RemoveAt(j);
                                  _dt.AcceptChanges();
                              }
                          }
                      }
                      #endregion
                  _dt.Merge(_outDt);
                  #endregion
                  }
                  if (_dt.Rows.Count > 0)
                  {
                      eslQuery.receiver_count = _dt.Rows.Count;
                      arrList.Add(_edmContentNewDao.InsertEdmSendLog(eslQuery));
                      for (int i = 0; i < _dt.Rows.Count; i++)
                      {
                          if (_dt.Rows[i]["user_email"].ToString() != "" && _dt.Rows[i]["user_email"].ToString() != null)
                          {
                              MRquery.receiver_address = _dt.Rows[i]["user_email"].ToString();
                              if (!string.IsNullOrEmpty(_dt.Rows[i]["user_name"].ToString()))
                              {
                                  MRquery.receiver_name = _dt.Rows[i]["user_name"].ToString();
                              }
                              else
                              {
                                  MRquery.receiver_name = "";
                              }
                              if (!string.IsNullOrEmpty(_dt.Rows[i]["user_id"].ToString()))
                              {
                                  MRquery.user_id = Convert.ToInt32(_dt.Rows[i]["user_id"].ToString());
                              }
                              else
                              {
                                  MRquery.user_id = 0;
                              }
                          }
                          else
                          {
                              MRquery.receiver_address = _dt.Rows[i]["customer_email"].ToString();
                              MRquery.receiver_name ="";
                              MRquery.user_id = 0;
                          }
                         arrList.Add(_edmContentNewDao.InsertEmailRequest(MRquery));
                      }
                  }
              }
              if (_mySql.ExcuteSqlsThrowException(arrList))
              {
                  json = "{success:'true'}";
              }
              else
              {
                  json = "{success:'false'}";
              }
              return json;
          }
          catch (Exception ex)
          {
              throw new Exception("EdmContentNewMgr-->MailAndRequest-->" + ex.Message, ex);
          }
      }
    }
}
