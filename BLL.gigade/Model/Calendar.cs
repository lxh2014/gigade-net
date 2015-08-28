using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class Calendar:PageBase
    {
        public uint id { get; set; }
        public int CalendarId { get; set; }
        public string Title { get; set; }
        public string StartDateStr { get; set; }
        public string EndDateStr { get; set; }
        public string Notes { get; set; }
        public bool IsAllDay { get; set; }

        public Calendar()
        {
            id = 0;
            CalendarId = 0;
            Title = string.Empty;
            StartDateStr = string.Empty;
            EndDateStr = string.Empty;
            Notes = string.Empty;
            IsAllDay = true;
        }
    }
}
