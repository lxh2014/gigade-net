using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class CategoryItem
    {
        public String Id { get; set; }

        public String Name { get; set; }

        public Int32 Depth { get; set; }

        public CategoryItem()
        {
            Id = string.Empty;
            Name = string.Empty;
            Depth = 0;
        }

    }
}
