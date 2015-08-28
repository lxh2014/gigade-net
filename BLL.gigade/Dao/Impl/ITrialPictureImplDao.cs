using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
   public interface ITrialPictureImplDao
    {
       List<TrialPictureQuery> QueryPic(TrialPictureQuery query);
       string DeletePic(TrialPictureQuery query);//返回sql語句
       string SavePic(TrialPictureQuery query);//返回sql語句
       int QueryMaxId();
       string VerifyEmail(string email);
       int DeleteAllPic(TrialPictureQuery query);//執行刪除圖片
    }
} 
