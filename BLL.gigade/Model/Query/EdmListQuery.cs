using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class EdmListQuery:EdmList
    {
        public double personRate { get; set; }//人數比率
        public double clickRate { get; set; } //次數比例
        public uint image_width { get; set; } //最大開信次數
        public string date { get; set; } //日期
        public string week { get; set; } //星期
        public EdmListQuery()
        {
            personRate = 0;
            clickRate = 0;
            image_width = 0;
            date = string.Empty;
            week = string.Empty;
        }
    }
}
