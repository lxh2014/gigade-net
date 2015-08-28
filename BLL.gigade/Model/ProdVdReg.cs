#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：ProdVdReq.cs
* 摘 要：
* 供應商上下架審核列表Model
* 当前版本：v1.1
* 作 者： mengjuan0826j
* 完成日期：
*/

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;

namespace BLL.gigade.Model
{
    [DBTableInfo("prod_vd_req")]
    public class ProdVdReq : PageBase
    {

        public int rid { get; set; }
        public int vendor_id { get; set; }
        public int product_id { get; set; }
        public int req_status { get; set; }
        public DateTime req_datatime { get; set; }
        public string explain { get; set; }
        public int req_type { get; set; }
        public int user_id { get; set; }
        public DateTime reply_datetime { get; set; }
        public string reply_note { get; set; }

        public ProdVdReq()
        {
            rid = 0;
            vendor_id = 0;
            product_id = 0;
            req_status = 0;
            req_datatime = DateTime.MinValue;
            explain = string.Empty;
            req_type = 0;
            user_id = 0;
            reply_datetime = DateTime.MinValue;
            reply_note = string.Empty;
        }
    }
}