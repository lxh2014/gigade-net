using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
  public  interface ITrialPictureImplMgr
    { 
      List<TrialPictureQuery> QueryPic(TrialPictureQuery query);
      bool SavePic(List<TrialPictureQuery> PicList, TrialPictureQuery pic);
      bool insertPic(List<TrialPictureQuery> PicList);
      int QueryMaxId();
      string VerifyEmail(string email);
      int DeleteAllPic(TrialPictureQuery query);//執行刪除圖片
    }
}
