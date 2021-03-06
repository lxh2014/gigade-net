﻿using BLL.gigade.Dao;
using BLL.gigade.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using System.Net;
using System.IO;

namespace BLL.gigade.Mgr
{
    public class EdmContentNewMgr
    {

        private EdmContentNewDao _edmContentNewDao;
        private MySqlDao _mySql;
        private EdmListConditionMainMgr _edmListConditionMgr;
        private EmailGroupDao _emailGroup;
        private string subscribe = "SUBSCRIBE_2015";
        private string subscribe_url = "<p style='text-align:center;'><span style='font-size:small;'><span style='color:#666666;'><a href='https://www.gigade100.com/member/mb_newsletter.php' target='_blank'>訂閱/解訂電子報</a></span></span></p>";
        public EdmContentNewMgr(string connectionString)
        {
            _edmContentNewDao = new EdmContentNewDao(connectionString);
            _mySql = new MySqlDao(connectionString);
            _emailGroup = new EmailGroupDao(connectionString);
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
                throw new Exception("EdmContentNewMgr-->GetECNList-->" + ex.Message, ex);
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
                    result = _edmContentNewDao.InsertEdmContentNew(query);
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

        #region 測試發送與正式發送
        public string MailAndRequest(EdmSendLog eslQuery, MailRequest MRquery)
        {
            eslQuery.Replace4MySQL();
            MRquery.Replace4MySQL();
            string json = string.Empty;
           
            ArrayList arrList = new ArrayList();
            try
            {
                if (eslQuery.test_send_end)//測試發送
                {
                    _edmContentNewDao.InsertEdmSendLog(eslQuery);
                    arrList.Add(_edmContentNewDao.InsertEmailRequest(MRquery));
                }
                else//正式發送
                {
                    /*
                     1.發送名單條件和額外發送列表和額外不發送列表進行查重
                     2.固定信箱名單和額外發送列表和額外不發送列表進行查重
                     3.發送名單條件和固定信箱名單進行查重
                     4.包含非訂閱的與額外發送列表和額外不發送列表進行查重進行查重
                     5.4的結果和3的結果去重
                     */
                    string[] extra_send_temp = new string[] { };
                    string[] extra_send = new string[] { };
                    string[] extra_no_send_temp = new string[] { };
                    string[] extra_no_send = new string[] { };
                    #region 額外發送列表中數據去重
                    if (MRquery.extra_send != "")
                    {
                        extra_send_temp = MRquery.extra_send.Split('\n');
                        List<string> extraSendTList = new List<string>();
                        for (int i = 0; i < extra_send_temp.Length; i++)
                        {
                            if (!extraSendTList.Contains(extra_send_temp[i]))
                            {
                                extraSendTList.Add(extra_send_temp[i]);
                            }
                        }
                      extra_send=    extraSendTList.ToArray();
                    }
                    #endregion
                    #region 額外不發送列表中數據去重
                    if (MRquery.extra_no_send != "")
                    {
                        extra_no_send_temp = MRquery.extra_no_send.Split('\n');
                        List<string> extraNoSendTList = new List<string>();
                        for (int i = 0; i < extra_no_send_temp.Length; i++)
                        {
                            if (!extraNoSendTList.Contains(extra_no_send_temp[i]))
                            {
                                extraNoSendTList.Add(extra_no_send_temp[i]);
                            }
                        }
                        extra_no_send = extraNoSendTList.ToArray();
                    }
                    #endregion
                    #region 第一步： 【發送名單條件】和額外發送列表和額外不發送列表進行查重
                    #region 發送名單條件
                    DataTable _newDt = new DataTable();
                    _newDt.Columns.Add("user_email", typeof(string));
                    _newDt.Columns.Add("user_name", typeof(string));
                    DataTable _dt = new DataTable();
                    //如果選得無則沒有任何email
                    if (eslQuery.elcm_id > 0)
                    {
                        _dt = _edmListConditionMgr.GetUserEmail(eslQuery.elcm_id);
                    }
                    else
                    {
                        _dt.Columns.Add("user_email", typeof(string));
                        _dt.Columns.Add("user_name", typeof(string));
                        _dt.Columns.Add("user_id", typeof(string));
                    }
                    #region 額外發送列表
                    #region 發送名單為空，額外發送不空
                    if ((_dt == null || _dt.Rows.Count == 0) && MRquery.extra_send != "")
                    {
                        
 
                        for (int i = 0; i < extra_send.Length; i++)
                        {
                            if (extra_send[i] != "")
                            {
                                DataRow dr = _dt.NewRow();
                                dr["user_email"] = extra_send[i];
                                dr["user_name"] = "";
                                dr["user_id"] = "0";
                                _dt.Rows.Add(dr);
                            }
                        }
                    }

                    #endregion
                    #region 發送名單為空，額外發送列表為空

                    #endregion
                    #region 發送名單不空，額外發送為空

                    #endregion
                    #region 發送名單不空，額外發送不空
                    else if ((_dt != null && _dt.Rows.Count > 0) && MRquery.extra_send != "")
                    {
                       
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
                                    dr["user_name"] = "";
                                    dr["user_id"] = "0";
                                    _dt.Rows.Add(dr);
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion
                    #endregion
                    #region 額外排除列表
                    #region 發送名單為空額外排除名單不空

                    #endregion
                    #region 發送名單為空，額外排除列表為空

                    #endregion
                    #region 發送名單不空，額外排除為空

                    #endregion
                    #region 發送名單不空，額外排除不空
                    if ((_dt != null && _dt.Rows.Count > 0) && MRquery.extra_no_send != "")
                    {

                        for (int i = 0; i < extra_no_send.Length; i++)
                        {
                            if (extra_no_send[i] != "")
                            {
                                for (int j = 0; j < _dt.Rows.Count; j++)
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
                    #endregion
                    #endregion
                    #region 第二步： 【固定信箱名單】額外發送列表和額外發送列表進行查重
                    #region 固定信箱名單
                    DataTable _emailDt = _emailGroup.GetEmailList(eslQuery.email_group_id);

                    #region 額外發送列表
                    #region 固定信箱名單為空，額外發送不為空
                    if ((_emailDt == null || _emailDt.Rows.Count == 0) && MRquery.extra_send != "")
                    {
                        _emailDt.Columns.Add("email_address", typeof(string));
                        _emailDt.Columns.Add("name", typeof(string));
                         
                        for (int i = 0; i < extra_send.Length; i++)
                        {
                            if (extra_send[i] != "")
                            {
                                DataRow dr = _emailDt.NewRow();
                                dr["email_address"] = extra_send[i];
                                dr["name"] = "";
                                _emailDt.Rows.Add(dr);
                            }
                        }
                    }
                    #endregion
                    #region 固定信箱名單為空，額外發送為空

                    #endregion
                    #region 固定信箱名單不為空，額外發送為空

                    #endregion
                    #region 固定信箱名單不為空，額外發送不為空
                    else if ((_emailDt != null && _emailDt.Rows.Count > 0) && MRquery.extra_send != "")
                    {
                         
                        for (int i = 0; i < extra_send.Length; i++)
                        {
                            if (extra_send[i] != "")
                            {
                                int norepeat = 0;
                                #region 額外發送的時候看看是不是已經存在這個email了，存在則不加入
                                for (int j = 0; j < _emailDt.Rows.Count; j++)
                                {
                                    if (_emailDt.Rows[j]["email_address"].ToString() != extra_send[i])
                                    {
                                        norepeat++;
                                    }
                                }
                                if (norepeat == _emailDt.Rows.Count)//證明不重複
                                {
                                    DataRow dr = _emailDt.NewRow();
                                    dr["email_address"] = extra_send[i];
                                    dr["name"] = "";
                                    _emailDt.Rows.Add(dr);
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion
                    #endregion

                    #region 額外排除列表
                    #region 信箱名單為空，額外排除為空

                    #endregion
                    #region 信箱名單為空，額外排除不為空

                    #endregion
                    #region 信箱名單不為空，額外排除為空

                    #endregion
                    #region 信箱名單不為空，額外排除為空
                    if ((_emailDt != null && _emailDt.Rows.Count > 0) && MRquery.extra_no_send != "")
                    {

                        for (int i = 0; i < extra_no_send.Length; i++)
                        {
                            if (extra_no_send[i] != "")
                            {
                                for (int j = 0; j < _emailDt.Rows.Count; j++)
                                {
                                    if (_emailDt.Rows[j]["email_address"].ToString() == extra_no_send[i])
                                    {
                                        _emailDt.Rows.Remove(_emailDt.Rows[j]);
                                        _emailDt.AcceptChanges();
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #endregion
                    #endregion
                    #endregion
                    #region 第三步：  發送名單條件和固定信箱名單查重

                    #region 發送名單為空，固定信箱不為空
                    if ((_dt == null || _dt.Rows.Count == 0) && _emailDt != null)
                    {
                        for (int i = 0; i < _emailDt.Rows.Count; i++)
                        {
                            DataRow dr = _dt.NewRow();
                            dr["user_email"] = _emailDt.Rows[i]["email_address"];
                            dr["user_name"] = _emailDt.Rows[i]["name"];
                            dr["user_id"] = "0";
                            _dt.Rows.Add(dr);
                        }
                    }
                    #endregion
                    #region 發送名單為空，固定信箱為空

                    #endregion
                    #region 發送名單不為空，固定信箱為空

                    #endregion
                    #region 發送名單不為空，固定信箱不為空

                    else if ((_dt != null && _dt.Rows.Count > 0) && (_emailDt != null && _emailDt.Rows.Count > 0))
                    {
                        for (int i = 0; i < _emailDt.Rows.Count; i++)
                        {
                            int norepeat = 0;
                            string email_address = string.Empty;
                            for (int j = 0; j < _dt.Rows.Count; j++)
                            {
                                if (_dt.Rows[j]["user_email"].ToString() != _emailDt.Rows[i]["email_address"].ToString())
                                {
                                    norepeat++;
                                    email_address = _emailDt.Rows[i]["email_address"].ToString();
                                }
                            }
                            if (norepeat == _dt.Rows.Count)//證明不重複
                            {
                                DataRow dr = _dt.NewRow();
                                dr["user_name"] = "";
                                dr["user_email"] = email_address;
                                dr["user_id"] = "0";
                                _dt.Rows.Add(dr);
                            }
                        }
                    }


                    #endregion


                    #endregion
                    #region 【包含訂閱】與額外發送列表和額外排除列表
                    if (MRquery.is_outer)
                    {
                        #region 包含訂閱
                        DataTable _outDt = GetCheckedDataTable(MRquery.group_id);
                        #region 額外發送列表

                        if (MRquery.extra_send != "")
                        {
                            
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
                            for (int i = 0; i < extra_no_send.Length; i++)
                            {
                                if (extra_no_send[i] != "")
                                {
                                    for (int j = 0; j < _outDt.Rows.Count; j++)
                                    {
                                        if (_outDt.Rows[j]["customer_email"].ToString() == extra_no_send[i])
                                        {
                                            _outDt.Rows.Remove(_outDt.Rows[j]);
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
                                if (_dt.Rows[j]["user_email"].ToString() == _outDt.Rows[i]["customer_email"].ToString())
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
                    #endregion
                    #region 賦值，生成sql語句
                    string RecommendHtml = string.Empty;
                    if (_dt.Rows.Count > 0)
                    {
                        eslQuery.receiver_count = _dt.Rows.Count;
                        int log_id = Convert.ToInt32(_edmContentNewDao.InsertEdmSendLog(eslQuery).Rows[0][0]);
                        for (int i = 0; i < _dt.Rows.Count; i++)
                        {
                            if (_dt.Columns.Contains("user_email"))
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
                                    MRquery.receiver_name = "";
                                    MRquery.user_id = 0;
                                }
                            }
                            else
                            {
                                if (_dt.Columns.Contains("customer_email"))
                                {
                                    MRquery.receiver_address = _dt.Rows[i]["customer_email"].ToString();
                                    MRquery.receiver_name = "";
                                    MRquery.user_id = 0;
                                }
                            }
                            if (MRquery.static_template == 0)
                            {
                                RecommendHtml = GetRecommendHtml(Convert.ToUInt32(MRquery.user_id));//根據user_id做出精準推薦
                            }
                            else
                            {
                                RecommendHtml = string.Empty;
                            }
                            EdmTraceEmail ete = new EdmTraceEmail();
                            ete.email = MRquery.receiver_address;
                            ete.name = MRquery.receiver_name;
                            int email_id = Convert.ToInt32(_edmContentNewDao.InsertEdmTraceEmail(ete).Rows[0][0]);
                            EdmTrace et = new EdmTrace();
                            et.log_id = log_id;
                            et.content_id = eslQuery.content_id;
                            et.count = 0;
                            et.success = -1;
                            et.email_id = email_id;
                            arrList.Add(_edmContentNewDao.InsertEdmTrace(et));
                            MRquery.success_action = "update edm_trace set success=1,send_date=NOW()  where log_id=" + log_id + " and  content_id=" + eslQuery.content_id + " and email_id=" + email_id + ";";
                            MRquery.fail_action = "update edm_trace set success=0,send_date=NOW()  where log_id=" + log_id + " and  content_id=" + eslQuery.content_id + " and email_id=" + email_id + ";";
                            #region 用於統計開信人數次數的url
                            DataTable _dtUrl = _edmContentNewDao.GetPraraData(2);//用於統計開信人數次數的url
                            string url = string.Empty;
                            if (_dtUrl != null && _dtUrl.Rows.Count > 0)
                            {
                                url = "<img src='" + _dtUrl.Rows[0][0].ToString() + "?c=" + eslQuery.content_id + "&e=" + email_id + "&l=" + log_id + "'/>";
                            }
                            #endregion
                            #region 獲得電子報整體內容

                            #region 是範本還是活動頁面
                                string replaceStr = string.Empty;
                                string editStr = string.Empty;
                                string content_url = GetContentUrlByContentId(eslQuery.content_id);
                                if (!string.IsNullOrEmpty(content_url))
                                {
                                    #region 獲取網頁內容
                                    HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(content_url);
                                    httpRequest.Timeout = 9000;
                                    httpRequest.Method = "GET";
                                    HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                                    StreamReader sr = new StreamReader(httpResponse.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                                    //     contentStr = sr.ReadToEnd();
                                    string contentStr = sr.ReadToEnd();
                                    DataTable replaceStrDt = GetPraraData(1);
                                    if (replaceStrDt != null && replaceStrDt.Rows.Count > 0)
                                    {
                                        replaceStr = replaceStrDt.Rows[0][0].ToString();
                                    }
                                    else
                                    {
                                        replaceStr = "&nbsp;&nbsp;";
                                    }
                                    DataTable _dtEdit =GetPraraData(3);
                                    if (_dtEdit != null && _dtEdit.Rows.Count > 0)
                                    {
                                        editStr = _dtEdit.Rows[0][0].ToString();
                                    }
                                    else
                                    {
                                        editStr = "&nbsp;&nbsp;";
                                    }
                                    if (MRquery.body.IndexOf(subscribe) > 0)//找到了埋的那個code，證明是點擊了訂閱電子報
                                    {
                                        MRquery.bodyData = contentStr.Replace(replaceStr, MRquery.body.Replace(editStr, "").Replace(subscribe, "\n") + RecommendHtml) + subscribe_url + url;
                                    }
                                    else
                                    {
                                        MRquery.bodyData = contentStr.Replace(replaceStr, MRquery.body.Replace(editStr, "") + RecommendHtml + url);
                                    }
                                    #endregion
                                }
                            #endregion
                            #endregion
                            arrList.Add(_edmContentNewDao.InsertEmailRequest(MRquery));
                            MRquery.bodyData = string.Empty;

                        }
                    }
                    #endregion
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

        //根據user_id得到精準推薦商品
        public string GetRecommendHtml(uint user_id)
        {
            string html = string.Empty;
            //html = "This is mine";
            return html;
        }
        #endregion

        public int GetSendMailSCount(int content_id,int log_id)
        {

            try
            {
                return _edmContentNewDao.GetSendMailSCount(content_id, log_id);
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewMgr-->GetSendMailSCount-->" + ex.Message, ex);
            }
        }
        public int GetSendMailFCount(int content_id, int log_id)
        {

            try
            {
                return _edmContentNewDao.GetSendMailFCount(content_id,log_id);
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewMgr-->GetSendMailFCount-->" + ex.Message, ex);
            }
        }
        public int GetSendMailCount(int content_id,int  log_id)
        {

            try
            {
                return _edmContentNewDao.GetSendMailCount(content_id, log_id);
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewMgr-->GetSendMailCount-->" + ex.Message, ex);
            }
        }
        public int GetSendCount(int content_id,int log_id)
        {

            try
            {
                return _edmContentNewDao.GetSendCount(content_id,log_id);
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewMgr-->GetSendCount-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 開信名單下載
        /// </summary>
        /// <param name="content_id"></param>
        /// <returns></returns>
        public DataTable KXMD(int content_id,int log_id)
        {
            try
            {
                return _edmContentNewDao.KXMD(content_id,log_id);
            }
            catch (Exception ex)
            {

                throw new Exception("EdmContentNewMgr-->KXMD-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 未開信名單下載
        /// </summary>
        /// <param name="content_id"></param>
        /// <returns></returns>
        public DataTable WKXMD(int content_id,int log_id)
        {
            try
            {
                return _edmContentNewDao.WKXMD(content_id,log_id);
            }
            catch (Exception ex)
            {

                throw new Exception("EdmContentNewMgr-->WKXMD-->" + ex.Message, ex);
            }
        }

        public DataTable GetCheckedDataTable(int group_id)
        {
            try
            {
                DataTable _outerCustomer = _edmContentNewDao.GetOuterCustomer(group_id);
                DataTable _innerCustomer = _edmContentNewDao.GetInnerCustomer(group_id);
                for (int i = 0; i < _innerCustomer.Rows.Count; i++)
                {
                    DataRow dr = _outerCustomer.NewRow();
                    dr["customer_email"] = _innerCustomer.Rows[i]["user_email"];
                    dr["customer_id"] = _innerCustomer.Rows[i]["user_id"];
                    _outerCustomer.Rows.Add(dr);
                }
                return _outerCustomer;
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewMgr-->GetCheckedDataTable-->" + ex.Message, ex);
            }
        }

        public DataTable FXMD(EdmTrace query)
        {
            try
            {
                return _edmContentNewDao.FXMD(query);
            }
            catch (Exception ex)
            {

                throw new Exception("EdmContentNewMgr-->FXMD-->" + ex.Message, ex);
            }
        }

        public DataTable EdmContentNewReportList(EdmTrace query)
        {
            try
            {
                return _edmContentNewDao.EdmTrace(query);
            }
            catch (Exception ex)
            {

                throw new Exception("EdmContentNewMgr-->EdmContentNewReportList-->" + ex.Message, ex);
            }
        }

        public DataTable CreatedateAndLogId(int content_id)
        {
            try
            {
                return _edmContentNewDao.CreatedateAndLogId(content_id);
            }
            catch (Exception ex)
            {
             throw new Exception("EdmContentNewMgr-->FXMD-->" + ex.Message, ex);
            }
        }

        public DataTable GetScheduleDate(int content_id, int log_id)
        {
            try
            {
                return _edmContentNewDao.GetScheduleDate(content_id, log_id);
            }
            catch (Exception ex)
            {
           throw new Exception("EdmContentNewMgr-->FXMD-->" + ex.Message, ex);
            }
        }

        public DataTable GetPraraData(int paraCode)
        {
            try
            {
                return _edmContentNewDao.GetPraraData(paraCode);
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewMgr-->GetPraraData-->" + ex.Message, ex);
            }
        }

        public string GetEditUrl(int template_id)
        {
            string url = string.Empty;
            try
            {
                DataTable _dt = _edmContentNewDao.GetEditUrl(template_id);
                if (_dt.Rows.Count > 0 && _dt != null)
                {
                    url = _dt.Rows[0][0].ToString();
                }
                return url;
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewMgr-->GetEditUrl-->" + ex.Message, ex);
            }
        }


        public string GetContentUrl(int template_id)
        {
            string url = string.Empty;
            try
            {
                DataTable _dt = _edmContentNewDao.GetContentUrl(template_id);
                if (_dt.Rows.Count > 0 && _dt != null)
                {
                    url = _dt.Rows[0][0].ToString();
                }
                return url;
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewMgr-->GetContentUrl-->" + ex.Message, ex);
            }
        }

        public string GetHtml(EdmContentNew query)
        {
            string htmlStr = string.Empty;
            try
            {
                DataTable _dt = _edmContentNewDao.GetHtml(query);
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    htmlStr = _dt.Rows[0][0].ToString();
                }
                return htmlStr;
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewMgr-->GetHtml-->" + ex.Message, ex);
            }
        }

        #region 郵件排成使用
        //清除過期信件
        public bool SendEMail(MailHelper mail)
        {
            try
            {
                _edmContentNewDao.ValidUntilDate();
                _edmContentNewDao.MaxRetry();

                return _edmContentNewDao.SendEMail(mail);
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleServiceDao-->SendEMail-->" + ex.Message);
            }
        }
        #endregion

        public DataTable GetParaStore(string paraType)
        {
            try
            {
                return _edmContentNewDao.GetParaStore(paraType);
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewMgr-->GetParaStore-->" + ex.Message, ex);
            }
        }

        public string  GetContentUrlByContentId(int content_id)
        {
            string content_url = string.Empty;
            try
            {
                DataTable _dt = _edmContentNewDao.GetContentUrlByContentId(content_id);
                if (_dt.Rows.Count > 0)
                {
                    content_url = _dt.Rows[0][0].ToString();
                }
                return content_url;
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewMgr-->GetParaStore-->" + ex.Message, ex);
            }
        }

        public string GetContentIDAndUrl(int group_id,int user_id)
        {
            string replaceStr = string.Empty;
            string content_data = string.Empty;
            string splitStr = string.Empty;
            try
            {
                DataTable _dt = _edmContentNewDao.GetContentIDAndUrl(group_id);
                if (_dt.Rows.Count > 0 && _dt != null)
                {
                    string content_url = _dt.Rows[0][0].ToString();
                    int content_id = Convert.ToInt32(_dt.Rows[0][1]);
                    int template_id = Convert.ToInt32(_dt.Rows[0][2]);
                    string template_data = _dt.Rows[0][3].ToString();
                    string static_template=  _dt.Rows[0][4].ToString();
                    if (static_template == "0")//動態範本
                    {
                        
                        template_data = template_data + GetRecommendHtml(0);
                    }
                    #region 替換符
                    DataTable _dtReplace  = GetPraraData(1);
                    if (_dtReplace != null && _dtReplace.Rows.Count > 0)
                    {
                        replaceStr = _dtReplace.Rows[0][0].ToString();
                    }
                    else
                    {
                        replaceStr = "&nbsp;&nbsp;";
                    }
                    #endregion
                    #region template_data中的分隔符
                    DataTable _dtSplit = GetPraraData(3);
                    if (_dtSplit != null && _dtSplit.Rows.Count > 0)
                    {
                        splitStr = _dtSplit.Rows[0][0].ToString();
                    }
                    else
                    {
                        splitStr = "&nbsp;&nbsp;";
                    }
                    #endregion
                    #region 讀content_url
                    HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(content_url);
                    httpRequest.Timeout = 9000;
                    httpRequest.Method = "GET";
                    HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                    StreamReader sr = new StreamReader(httpResponse.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                    content_data = sr.ReadToEnd();
                    #endregion
                    if (template_data.IndexOf(subscribe) > 0)
                    {
                        content_data =  content_data.Replace(replaceStr, template_data.Replace(subscribe, "\n").Replace(splitStr,"")) + subscribe_url;
                    }
                    else
                    {
                        content_data = content_data.Replace(replaceStr, template_data.Replace(subscribe, "\n").Replace(splitStr, ""));
                    }
                    #region 拼組成html
                    #endregion
                }
                return content_data;
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewMgr-->GetContentIDAndUrl-->" + ex.Message, ex);
            }
        }

        public int AdvanceTemplate()
        {
            int template_id = 0;
            try
            {
                DataTable _dt = _edmContentNewDao.AdvanceTemplate();
                if (_dt.Rows.Count > 0 && _dt != null)
                {
                    template_id = Convert.ToInt32(_dt.Rows[0][0]);
                }
                return template_id;
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewMgr-->AdvanceTemplate-->" + ex.Message, ex);
            }
        }
    }
}
