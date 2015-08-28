using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Common;

namespace BLL.gigade.Mgr
{
    public class DesignRequestMgr
    {
        private DesignRequestDao _DesignRequestDao;

         public DesignRequestMgr(string connectionString)
        {
            _DesignRequestDao = new DesignRequestDao(connectionString);
        }
         #region 獲取design_request表的list
         public List<DesignRequestQuery> GetList(DesignRequestQuery query, out int totalCount)
         {
             try
             {
                return _DesignRequestDao.GetList(query, out totalCount);
             }
             catch (Exception ex)
             {
                throw new Exception("DesignRequestMgr.GetList-->" + ex.Message, ex);
             }
         }
         #endregion
         #region 新增/編輯保存
         public string DesignRequestEdit(DesignRequestQuery query)
         {
             string json = string.Empty;
             query.Replace4MySQL();
             query.dr_resource_path = query.dr_resource_path.Replace("\\", "\\\\");
             query.dr_document_path = query.dr_document_path.Replace("\\", "\\\\");
             List<DisableKeywords> list = new List<DisableKeywords>();
             try
             {
                 int j = 0;
                 list = _DesignRequestDao.GetKeyWordsList();
                 for (int i = 0; i < list.Count; i++)
                 {
                     if (query.dr_content_text.Contains(list[i].dk_string))
                     {
                         j = 1;
                     }
                 }
                 if (query.dr_id == 0)
                 {//新增
                     if (query.product_id != 0 && query.dr_type == 4 && j != 1)
                     {
                         _DesignRequestDao.UpdateProductDetailText(query);
                     }
                     if (j == 1)
                     {
                         query.dr_status = 1;
                         MailHelper mail = new MailHelper();
                         string sbHtml = "你好,派工系統申請的文案包含有禁用的關鍵字,還請前去查看新建立的項目   ";
                         mail.SendToGroup("job", "派工系統", sbHtml.ToString(), false, true);
                     }
                     else
                     {
                         query.dr_status = 2;//已審核
                         if (GetExpected(query) > 0 && j != 1)
                         {//已審核的文件直接算出期望完成時間
                             int day = Getday(query);
                             //query.dr_expected = DateTime.Now.AddDays(day);
                             query.day = day;
                         }
                     }
                     int res = _DesignRequestDao.InsertDesignRequest(query);
                     if (res > 0)
                     {
                         json = "{success:true,type:1,msg:1}";//type=1表示新增,msg=1表示新增成功
                     }
                    else
                    {
                         json = "{success:true,type:1,msg:0}";//type=1表示新增,msg=0表示新增失敗
                     }
                 }
                else
                { //編輯
                    query.dr_status = 1;
                    DesignRequestQuery OldModel = new DesignRequestQuery();
                    OldModel = _DesignRequestDao.GetSingleDesignRequest(query);
                    if (!string.IsNullOrEmpty(query.dr_type_tostring))
                    {
                        query.dr_type = OldModel.dr_type;
                    }
                    if (query.product_id != 0 && query.dr_type == 4 && j != 1)
                    {
                        _DesignRequestDao.UpdateProductDetailText(query);
                    }
                    if (j == 1)
                    {
                        query.dr_status = 1;
                        MailHelper mail = new MailHelper();
                        string sbHtml = "你好,派工系統申請的文案包含有禁用的關鍵字,還請前去查看新建立的項目     ";
                        mail.SendToGroup("job", "派工系統", sbHtml.ToString(), false, true);
                    }
                    else
                    {
                        query.dr_status = 2;
                        if (GetExpected(query) > 0 && j != 1)
                        {//已審核的文件直接算出期望完成時間
                            int day = Getday(query);
                            //query.dr_expected = DateTime.Now.AddDays(day);
                            query.day = day;
                        }
                    }
                     int res = _DesignRequestDao.UpdateDesignRequest(query);
                    
                     if (res > 0)
                     {
                         json = "{success:true,type:2,msg:1}";//type=2表示編輯,msg=1表示編輯成功
                     }
                    else
                    {
                         json = "{success:true,type:2,msg:0}";//type=2表示編輯,msg=0表示編輯失敗
                     }
                 }
                 return json;
             }
             catch (Exception ex)
             {
                 throw new Exception("DesignRequestMgr.DesignRequestEdit-->" + ex.Message, ex);
             }
         }
        #endregion
         #region 刪除
         public string DelDesignRequest(DesignRequestQuery query)
         {
             string json = "";
             try
             {
                 if (IsSelf(query))
                 {
                     if (query.dr_status>2)
                     {
                        MailHelper mail = new MailHelper();
                        string sbHtml = "你好,派工系統分配給您的工作被需求者刪除,還請查看   ";
                        if (!mail.SendToUser(GetmailId(query.dr_assign_to), "派工系統", sbHtml.ToString()))//發送email給指派的人員
                        {
                            return json = "{success:true,msg:3}";//郵件發送失敗
                        }
                     }
                     if (_DesignRequestDao.DelDesignRequest(query) > 0)
                     {
                         return json = "{success:true,msg:0}";
                     }
                     else
                     {
                         return json = "{success:true,msg:1}";//msg=1刪除失敗
                     }
                 }
                 else
                 {
                     return json = "{success:true,msg:2}";//msg=2不是需求者不能刪除
                 }
             }
             catch (Exception ex)
             {
                 throw new Exception("DesignRequestMgr.DelDesignRequest-->" + ex.Message, ex);
             }
         }
        #endregion
        public DataTable GetDesign(ManageUserQuery query)
        {
            try
            {
                return _DesignRequestDao.GetDesign(query);
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestMgr.GetDesign-->" + ex.Message, ex);
            }
        }
        #region 根據product_id獲取name
        public DataTable GetPorductNameByProductId(int product_id)
        {
            try
            {
                return _DesignRequestDao.GetPorductNameByProductId(product_id);
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestMgr.GetPorductNameByProductId-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 狀態變更
        public string UpdStatus(DesignRequestQuery drt)
        {
            string json = string.Empty;
            try
            {
                if (drt.dr_status == 2 )
                {//狀態為新建立,類型為內頁文,
                    if (drt.product_id != 0&& drt.dr_type == 4)
                    {//審核文字通過新增到product表,變更狀態 
                        if (_DesignRequestDao.UpdateProductDetailText(drt) ==0)
                        {
                            return json = "{success:true,msg:3}";//更新內頁文失敗
                        }
                    }
                    if (GetExpected(drt) > 0)
                    {//已審核的文件算出期望完成時間
                        int day = Getday(drt);
                        drt.day = day;
                    }
                    if (_DesignRequestDao.UpdStatus(drt) > 0)
                    {
                        return json = "{success:true,msg:0}";//審核成功
                    }
                    else
                    {
                        return json = "{success:false}";
                    }
                }
                else if (drt.dr_assign_to > 0)
                {//指派工作    變更指派人員 
                    //認領工作只能設計部人員認領
                    //指派工作只能主管指派
                    if (_DesignRequestDao.IsManager(drt) ||(drt.Istake==1 && IsManagerNumber(drt)))
                    {
                        drt.dr_status = 3;
                        MailHelper mail = new MailHelper();
                        string sbHtml = "你好,派工系統申請的文案通過審核分配給您,請前去查看工作內容   ";
                        if (mail.SendToUser(GetmailId(drt.dr_assign_to), "派工系統", sbHtml.ToString()))//發送email給指派的人員
                        {
                            if (_DesignRequestDao.UpdStatus(drt) > 0)
                            {
                                return json = "{success:true,msg:0}";//msg=2表示有敏感詞
                            }
                            else
                            {
                                return json = "{success:false}";//msg=2表示有敏感詞
                            }
                        }
                        else
                        {
                            return json = "{success:true,msg:4}";//email發送失敗
                        }
                    }
                    else
                    {
                        return json = "{success:true,msg:2}";//msg=2不是設計部人員不能指派
                    }                
                }
                else
                {//如果是設計人員就可以變更已指派後面的所有狀態
                    if (IsDesSelf(drt))
                    {
                        if (_DesignRequestDao.UpdStatus(drt) > 0)
                        {
                            return json = "{success:true,msg:0}";//msg=2表示有敏感詞
                        }
                        else
                        {
                            return json = "{success:false}";//msg=2表示有敏感詞
                        }
                    }
                    else 
                    {
                        return json = "{success:true,msg:2}";//msg=2不是設計部人員不能指派
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestMgr.UpdStatus-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 預期天數
        public int GetExpected(DesignRequestQuery drt)
        {
            string json = string.Empty;
            int day =0;
            try
            {
                if (_DesignRequestDao.GetExpected(drt).Rows.Count>0)
                {
                    if (int.TryParse(_DesignRequestDao.GetExpected(drt).Rows[0][0].ToString(),out day))
                    {
                        return day;
                    }
                }
                return day;                
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestMgr.GetExpected-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 是否是設計部人員
        public bool IsManagerNumber(DesignRequestQuery drt)
        {
            try
            {
               return  _DesignRequestDao.IsManagerNumber(drt);
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestMgr.IsManagerNumber-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 是否是自己的申請需求--刪除用
        public bool IsSelf(DesignRequestQuery drt)
        {//是否是自己提出的需求
            try
            {
                return _DesignRequestDao.IsSelf(drt);
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestMgr.IsSelf-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 是否是自己的工作--變更狀態用
        public bool IsDesSelf(DesignRequestQuery drt)
        {
            try
            {
                return _DesignRequestDao.IsDesSelf(drt);
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestMgr.IsDesSelf-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 獲取期望天數
        public int Getday(DesignRequestQuery query)
        { //獲取完成天數
            string week =DateTime.Now.DayOfWeek.ToString();
            int day = GetExpected(query);
            switch (day)
            {
                case 3:
                    if (week == "Wednesday" || week == "Thursday" || week == "Friday")
                        day = day + 2;
                    break;
                case 2:
                    if (week == "Thursday" || week == "Friday")
                        day = day + 2;
                    break;
                case 1:
                    if (week == "Friday")
                        day = day + 2;
                    break;
                default:                                 
                    break;
            }
            return day;        
        }
        #endregion
        #region 獲取該mail對應的郵件群裡面的id
        public int GetmailId(int id)
        {
            try
            {
                return _DesignRequestDao.GetmailId(id);
            }
            catch (Exception ex)
            {
                throw new Exception("DesignRequestMgr.GetmailId-->" + ex.Message, ex);
            }
        }
        #endregion
    }
}
