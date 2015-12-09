using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.APIModels
{
    public class ModifyExpertArriveDateViewModel
    {
        /// <summary>
        /// 出貨單id
        /// </summary>
        public long deliver_id { get; set; }
        /// <summary>
        /// 期望到貨日
        /// </summary>
        public DateTime newDate { get; set; }
        /// <summary>
        /// 期望到貨時段
        /// </summary>
        public ExpectArrivePeriod period { get; set; }

        public string note { get; set; }

    }

    
}
