using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class FunctionCustom:Function
    {
        public string CallId { get; set; }//用戶email
        public string GroupName { get; set; }//t_fgroup表中的角色名
        public int GroupId { get; set; }//t_fgroup表中rowid
        public string User_UserName { get; set; }//用戶名
    }
}
