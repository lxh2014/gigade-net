using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class TrialPictureMgr : ITrialPictureImplMgr
    { 
        private ITrialPictureImplDao _ITrialPictureDao;
        private string connStr;
           public TrialPictureMgr(string connectionString)
        {
            _ITrialPictureDao = new TrialPictureDao(connectionString);
            this.connStr = connectionString;
        }

           public List<Model.Query.TrialPictureQuery> QueryPic(Model.Query.TrialPictureQuery query)
        {
            try
            {
                return _ITrialPictureDao.QueryPic(query);
            }
            catch (Exception ex)
            {
                throw new Exception("TrialPictureMgr.QueryPic-->"+ex.Message,ex);
            }
        }


           public bool SavePic(List<Model.Query.TrialPictureQuery> PicList, Model.Query.TrialPictureQuery pic)
           {
               try
               {
                   ArrayList arrList = new ArrayList();
                   {
                       arrList.Add(_ITrialPictureDao.DeletePic(pic));
                   }
                   foreach (var item in PicList)
                   {
                       arrList.Add(_ITrialPictureDao.SavePic(item));
                   }
                   MySqlDao mySqlDao = new MySqlDao(connStr);
                   return mySqlDao.ExcuteSqls(arrList);
               }
               catch (Exception ex)
               {
                   throw new Exception("TrialPictureMgr.SavePic-->"+ex.Message,ex);
               }
           }


           public int QueryMaxId()
           {

               try
               {
                   return _ITrialPictureDao.QueryMaxId();
               }
               catch (Exception ex)
               {
                   throw new Exception("TrialPictureMgr.QueryMaxId-->" + ex.Message, ex);

               }
           }


           public bool insertPic(List<Model.Query.TrialPictureQuery> PicList)
           {
               ArrayList arrList = new ArrayList();
               try
               {
                   foreach (var item in PicList)
                   {
                       arrList.Add(_ITrialPictureDao.SavePic(item));
                   }
                   MySqlDao mySqlDao = new MySqlDao(connStr);
                   return mySqlDao.ExcuteSqls(arrList);
               }
               catch (Exception ex)
               {
                   throw new Exception("TrialPictureMgr.insertPic-->" + ex.Message, ex);
               }
           }


           public string VerifyEmail(string email)
           {
               try
               {
                   return _ITrialPictureDao.VerifyEmail(email);
               }
               catch (Exception ex)
               {
                   throw new Exception("TrialPictureMgr.VerifyEmail-->" + ex.Message, ex);
               }
           }


           public int DeleteAllPic(TrialPictureQuery query)
           {
               try
               {
                   return _ITrialPictureDao.DeleteAllPic(query);
               }
               catch (Exception ex)
               {
                   throw new Exception("TrialPictureMgr.DeleteAllPic-->" + ex.Message, ex);
               }

           }
    }
}
