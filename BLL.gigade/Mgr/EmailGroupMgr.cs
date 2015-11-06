using BLL.gigade.Dao;
using BLL.gigade.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BLL.gigade.Mgr
{
    public class EmailGroupMgr
    {
        private EmailGroupDao _emailGroupDao;
        private MySqlDao _mySqlDao;
        public EmailGroupMgr(string connectionString)
        {
            _emailGroupDao = new EmailGroupDao(connectionString);
            _mySqlDao = new MySqlDao(connectionString);
        }

        public List<EmailGroup> EmailGroupList(EmailGroup query, out int totalCount)
        {
            try
            {
                return _emailGroupDao.EmailGroupList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("EmailGroupMgr-->EmailGroupList-->" + ex.Message, ex);
            }
        }

        public DataTable ImportEmailList(DataTable _dt, int group_id, out int totalCount)
        {

            DataTable _newDt = new DataTable();
            totalCount = 0;
            if (_dt != null && _dt.Rows.Count > 0)
            {
                EmailGroup query = new EmailGroup();
                ArrayList arrList = new ArrayList();
             
                query.group_id = group_id;
                _newDt.Columns.Add("電子信箱地址", typeof(string));
                _newDt.Columns.Add("收件人名稱", typeof(string));
                _newDt.Columns.Add("匯入失敗原因", typeof(string));
                string regex = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
                try
                {
                     
                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        if (_dt.Rows[i][0] != "")
                        {
                            totalCount++;
                            if (Regex.IsMatch(_dt.Rows[i][0].ToString(), regex))
                            {
                                if (!_emailGroupDao.IsExistEmail(_dt.Rows[i][0].ToString(), group_id))
                                {
                                    query.email_address = _dt.Rows[i][0].ToString();
                                    query.name = _dt.Rows[i][1].ToString();

                                    try
                                    {
                                        if (!_emailGroupDao.ImportEmailList(query))
                                        {
                                            DataRow _dr = _newDt.NewRow();
                                            _dr["電子信箱地址"] = _dt.Rows[i][0].ToString();
                                            _dr["收件人名稱"] = _dt.Rows[i][1].ToString();
                                            _dr["匯入失敗原因"] = "執行數據庫出錯";
                                            _newDt.Rows.Add(_dr);
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        DataRow _dr = _newDt.NewRow();
                                        _dr["電子信箱地址"] = _dt.Rows[i][0].ToString();
                                        _dr["收件人名稱"] = _dt.Rows[i][1].ToString();
                                        _dr["匯入失敗原因"] = "執行數據庫出錯";
                                        _newDt.Rows.Add(_dr);
                                        continue;
                                    }

                                  
                                }
                                else
                                {
                                    //數據重複
                                    DataRow _dr = _newDt.NewRow();
                                    _dr["電子信箱地址"] = _dt.Rows[i][0].ToString();
                                    _dr["收件人名稱"] = _dt.Rows[i][1].ToString();
                                    _dr["匯入失敗原因"] = "數據重複";
                                    _newDt.Rows.Add(_dr);
                                }
                            }
                            else
                            {
                                //不符合郵件格式
                                DataRow _dr = _newDt.NewRow();
                                _dr["電子信箱地址"] = _dt.Rows[i][0].ToString();
                                _dr["收件人名稱"] = _dt.Rows[i][1].ToString();
                                _dr["匯入失敗原因"] = "不符合郵件格式";
                                _newDt.Rows.Add(_dr);
                            }
                        }
                      
                    }
                    return _newDt;
                }
                catch (Exception ex)
                {
                    throw new Exception("EmailGroupMgr-->ImportEmailList-->" + ex.Message, ex);
                }

            }
            return _newDt;

        }

        public DataTable Export(int group_id)
        {
            try
            {
                return _emailGroupDao.Export(group_id);
            }
            catch (Exception ex)
            {
                throw new Exception("EmailGroupDao-->Export-->" + ex.Message, ex);
            }
        }

        public bool SaveEmailGroup(EmailGroup query)
        {
            ArrayList arrList = new ArrayList();
            try
            {
                if (query.group_id == 0)
                {
                    arrList.Add(_emailGroupDao.InsertEmailGroup(query));
                }
                else
                {
                    arrList.Add(_emailGroupDao.UpdateEmailGroup(query));
                }
                return _mySqlDao.ExcuteSqlsThrowException(arrList);
            }
            catch (Exception ex)
            {
                throw new Exception("EmailGroupDao-->Export-->" + ex.Message, ex);
            }

        }

        public List<EmailGroup> EmailGroupStore()
        {
            try
            {
                return _emailGroupDao.EmailGroupStore();
            }
            catch (Exception ex)
            {
                throw new Exception("EmailGroupDao-->EmailGroupStore-->" + ex.Message, ex);
            }
        }


        public string DelEmailGroupList(List<EmailGroup> list)
        {
            ArrayList arrList = new ArrayList();
            string json = "{success:true}";
            try
            {
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        arrList.Add(_emailGroupDao.DeleteEmailList(item.group_id));
                        arrList.Add(_emailGroupDao.DeleteEmailGroup(item.group_id));
                    }
                }
                if (arrList.Count > 0)
                {
                    if (_mySqlDao.ExcuteSqlsThrowException(arrList))
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

                throw new Exception("EmailGroupDao-->DelEmailGroupList-->" + ex.Message, ex);
            }
        }




    }
}
