using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class Column
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string Column_Name { get; set; }
        /// <summary>
        /// 列註釋
        /// </summary>
        public string Column_Comment { get; set; }
        public Column()
        {
            Column_Name = string.Empty;
            Column_Comment = string.Empty ;
            //ColumnComment = "沒有描述";
        }
        
    }
}
