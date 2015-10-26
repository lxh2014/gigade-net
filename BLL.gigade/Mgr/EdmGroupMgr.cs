using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using System.Data;

namespace BLL.gigade.Mgr
{
  public  class EdmGroupMgr
    {
      private EdmGroupDao _edmGroup;
      private ISerialImplDao _ISerialImpl;
      private IEdmGroupEmailImpIDao _IEdmGroupEmailMgr;
      private InspectionReportDao _inspectionReport;
      public EdmGroupMgr(string connectionString)
      {
          _edmGroup = new EdmGroupDao(connectionString);
          _ISerialImpl = new SerialDao(connectionString);
          _IEdmGroupEmailMgr = new EdmGroupEmailDao(connectionString);
          _inspectionReport = new InspectionReportDao(connectionString);
      }

      public List<EdmGroup> GetEdmGroupList(EdmGroup query, out int totalCount)
      {
          try
          {
              List<EdmGroup> store = new List<EdmGroup>();
              store=_edmGroup.GetEdmGroupList(query, out totalCount);
              foreach (var item in store)
              {
                  item.s_group_createdate = CommonFunction.GetNetTime(item.group_createdate).ToString("yyyy-MM-dd HH:mm:ss");
                  item.s_group_updatedate = CommonFunction.GetNetTime(item.group_updatedate).ToString("yyyy-MM-dd HH:mm:ss");
              }
              return store;
          }
          catch (Exception ex)
          {
              throw new Exception("EdmGroupMgr-->GetEdmGroupList-->" + ex.Message, ex);
          }
      }

      //define('SERIAL_ID_EDM_GROUP',			50);	// 電子報流水號
      public string SaveEdmGroup(EdmGroup query)
      {
          string json = string.Empty;
          Serial serial = new Serial();
          ArrayList arrList = new ArrayList();
          try
          {
              if (query.group_id == 0)//新增
              {
                  serial.Serial_id=50;
                  serial=_ISerialImpl.GetSerialById(serial.Serial_id);//根據ID得到serial_value;
                  query.group_id=Convert.ToUInt32(serial.Serial_Value) + 1;//將serial_value值加1就是group_id;
                  arrList.Add(_edmGroup.InsertEdmGroup(query));//insert edm_group
                  arrList.Add(_ISerialImpl.Update(serial.Serial_id));//update serial
                  if (_edmGroup.execSql(arrList))
                  {
                      json = "{success:true}";
                  }
                  else
                  {
                      json = "{success:false}";
                  }
              }
              else//編輯
              {
                  if (_edmGroup.UpEdmGroup(query) > 0)
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
              throw new Exception("EdmGroupMgr-->UpEdmGroup-->" + ex.Message, ex);
          }
      }

      public string DeleteEdmGroup(List<EdmGroup> list)
      {
          ArrayList arrListGroup = new ArrayList();
          ArrayList arrListMail = new ArrayList();
          bool group = true;
          bool mail = true;
          string json = "{success:true}";
          try
          {
              for (int i = 0; i < list.Count; i++)
              {
                  arrListGroup.Add(_edmGroup.DeleteEdmGroup(list[i]));
                  arrListMail.Add(_edmGroup.DeleteEdmGroupMail(list[i]));
              }
              if (arrListMail.Count > 0)
              {
               group=   _edmGroup.execSql(arrListMail);
              }
              if (arrListGroup.Count > 0)
              {
                mail= _edmGroup.execSql(arrListGroup);
              }
              if (group && mail)
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
              throw new Exception("EdmGroupMgr-->DeleteEdmGroup-->" + ex.Message, ex);
          }
      }

      public DataTable Export(EdmGroup query)
      {
          try
          {
              return _edmGroup.Export(query);
          }
          catch (Exception ex)
          {
              throw new Exception("EdmGroupMgr-->Export-->" + ex.Message, ex);
          }
      }

      public string Import(DataTable _dt, EdmGroupEmailQuery groupMailQuery)
      {
          string json = "{success:'true'}";
          ArrayList arrList = new ArrayList();
          Serial serial = new Serial();
          EdmEmailQuery emailQuery = new EdmEmailQuery();
          EdmGroupQuery query = new EdmGroupQuery();
          List<EdmEmail> emailStore =new List<EdmEmail>();
          List<EdmGroupEmailQuery> groupMailStore =new List<EdmGroupEmailQuery>();
          try
          {
              query.group_id = groupMailQuery.group_id;
              for (int i = 0; i < _dt.Rows.Count; i++)
              {
                  if (_dt.Rows[i][0] != "")
                  {
                      emailQuery.email_address = _dt.Rows[i][0].ToString();
                      //若不為 0、1、未指定或有錯誤時，預設皆為訂閱。
                      int n = 1;
                      if (int.TryParse(_dt.Rows[i][1].ToString(), out n))
                      {
                          groupMailQuery.email_status = Convert.ToUInt32(_dt.Rows[i][1].ToString());
                          if (groupMailQuery.email_status != 1 && groupMailQuery.email_status != 0)
                          {
                              groupMailQuery.email_status = 1;
                          }
                      }
                      else
                      {
                          groupMailQuery.email_status = 1;
                      }
                      if (_dt.Rows[i][2] == "")//或無指定，預設以電子信箱帳號代替。
                      {
                          emailQuery.email_name = _dt.Rows[i][0].ToString().Substring(0, _dt.Rows[i][0].ToString().LastIndexOf("@"));
                      }
                      else
                      {
                          emailQuery.email_name = _dt.Rows[i][2].ToString();
                      }
                      groupMailQuery.email_name = emailQuery.email_name;
                      //查看edm_mail中郵箱是否存在，如果重複則更新
                      emailStore = _IEdmGroupEmailMgr.getList(emailQuery);
                      if (emailStore.Count > 0)//存在,更新
                      {
                          emailQuery.email_id = emailStore[0].email_id;
                          emailQuery.email_updatedate = Convert.ToInt32(CommonFunction.GetPHPTime());
                          arrList.Add(_IEdmGroupEmailMgr.UpdateEdmEmailStr(emailQuery));//更新edm_mail表
                       
                          groupMailQuery.email_id = emailQuery.email_id;
                          // 查看 edm_group_email表中數據是否存在
                          if (_IEdmGroupEmailMgr.Check(groupMailQuery).Count > 0)//存在
                          {
                              groupMailQuery.email_updatedate = Convert.ToUInt32(CommonFunction.GetPHPTime());
                              arrList.Add(_edmGroup.UpdateEGE(groupMailQuery));
                          }
                          else
                          {
                              groupMailQuery.email_updatedate = Convert.ToUInt32(CommonFunction.GetPHPTime());
                              groupMailQuery.email_createdate = Convert.ToUInt32(CommonFunction.GetPHPTime());
                              arrList.Add(_edmGroup.insertEGEInfo(groupMailQuery));
                          }
                      }
                      else//不存在新增
                      {
                          //1 新增edm_email表
                          //2 新增edm_group_email表
                          string sql = _ISerialImpl.Update(51);//51代表edm_email表
                          serial = _IEdmGroupEmailMgr.execSql(sql);
                          emailQuery.email_id = Convert.ToUInt32(serial.Serial_Value);

                          emailQuery.email_createdate = Convert.ToInt32(Common.CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                          emailQuery.email_updatedate = Convert.ToInt32(Common.CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                          emailQuery.email_check = 0;
                          arrList.Add(_edmGroup.insertEdmEmail(emailQuery));

                          groupMailQuery.email_createdate = Convert.ToUInt32(Common.CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                          groupMailQuery.email_updatedate = Convert.ToUInt32(Common.CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                          groupMailQuery.email_id = emailQuery.email_id;
                          arrList.Add(_edmGroup.insertEGEInfo(groupMailQuery));
                      }
                  }
              }
              if (arrList.Count > 0)
              {
                  if (_inspectionReport.ExecSql(arrList))
                  {
                
                      json = "{success:'true'}";
                  }
                  else
                  {
                      json = "{success:'false'}";
                  }
              }
            DataTable _dtCount =   _IEdmGroupEmailMgr.getCount(Convert.ToInt32(query.group_id));
            if (_dtCount.Rows.Count > 0)
            {
                query.group_total_email = Convert.ToUInt32(_dtCount.Rows[0][0]);
                _IEdmGroupEmailMgr.updateEdmGroupCount(query);
            }
             
              return json;
          }
          catch (Exception ex)
          {
              throw new Exception("EdmGroupMgr-->Import-->" + ex.Message, ex);
          }
      }
    }
}
