using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class BonusType
    {
        public uint type_id;
        public string type_description;
        public BonusType()
        {
            type_id = 0;
            type_description = string.Empty;
        }
    }
}
