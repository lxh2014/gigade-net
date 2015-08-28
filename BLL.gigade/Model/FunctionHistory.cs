using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class FunctionHistory:PageBase
    {
        public uint Row_Id { get; set; }
        public int Function_Id { get; set; }
        public int User_Id { get; set; }
        public DateTime Operate_Time { get; set; }

        public FunctionHistory()
        {
            Row_Id = 0;
            Function_Id = 0;
            User_Id = 0;
            Operate_Time = DateTime.MinValue;
        }
    }
}
