using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Dao.Impl
{
   public interface IUserEdmImplDao
    {
        void AddEdmGroup();
       List<Users> GetUserInfo();
       UserEdm GetUserEmail(string email_address);
       EdmGroupQuery GetEdmGroupById();
       uint AddEmail(string emailName, string userName);
       void AddGroupEmail(uint groupId, uint emailId, string emailName, uint emailStatus = 1);
       EdmGroupEmail getEdmGroupEmail(uint groupid, uint emailId);
       void modifyEmailName(uint emailId, string emailName);
       void ModifyGroupEmail(uint groupId, uint emailId, string emailName, uint emailStatus = 1);
       void ModifyEmailId(uint serialvalue);
    }
}
