using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class IpoNvdQuery:IpoNvd
    {
        public string create_username { get; set; }
        public string modify_username { get; set; }

        public IpoNvdQuery()
        {
            create_username = string.Empty;
            modify_username = string.Empty;
        }
    }
}
