using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    [Serializable]
    public class VendorQuery : Vendor
    { 
        public uint create_dateOne { get; set; }
        public uint create_dateTwo { get; set; }
        public string searchEmail { get; set; }
        public string searchName { get; set; }
        public string searchInvoice { get; set; }
        public Int32 searchStatus { get; set; }
        public DateTime agr_date { get; set; }
        public string file_name { get; set; }
        public string content { get; set; }
        public string ip { get; set; }
        public string kuser_name { get; set; }
        public uint kuser_id { get; set; }
        public DateTime created { get; set; }
        public DateTime agr_start { get; set; }

        public string i_middle { get; set; }
        public string c_middle { get; set; }
        public string i_zip { get; set; }
        public string c_zip { get; set; }

        public string i_bigcode { get; set; }
        public string c_bigcode { get; set; }
        public string i_midcode { get; set; }
        public string c_midcode { get; set; }
        public string i_zipcode { get; set; }
        public string c_zipcode { get; set; }

        public string manage_name { get; set; }
        public DateTime agr_end { get; set; }
        public String manage_email { get; set; }

        public string prod_cate { get; set; }
        public string buy_cate { get; set; }
        public string tax_type { get; set; }
        public string serial { get; set; }
        public string vendor_mode { get; set; }
        /// <summary>
        /// 供應商公司地址
        /// </summary>
        public string vendor_company_address { get; set; }
        //供應商類型
        public string vendor_type_name { get; set; }
        public int user_type { get; set; }//變更者類型1.供應商2.管理員
        public VendorQuery()
        {
            create_dateOne = 0;
            create_dateTwo = 0;
            searchEmail = string.Empty;
            searchName = string.Empty;
            searchInvoice = string.Empty;
            searchStatus = -1;
            agr_date = DateTime.MinValue;
            file_name = "";
            content = "";
            ip = "";
            kuser_name = "";
            kuser_id = 0;
            created = DateTime.MinValue;
            agr_start = DateTime.MinValue;
            agr_end = DateTime.MinValue;
            i_middle = string.Empty;
            i_zip = string.Empty;
            c_middle = string.Empty;
            c_zip = string.Empty;
            i_bigcode = string.Empty;
            c_bigcode = string.Empty;
            i_midcode = string.Empty;
            i_zipcode = string.Empty;
            c_midcode = string.Empty;
            c_zipcode = string.Empty;
            manage_name = string.Empty;
            prod_cate = string.Empty;
            buy_cate = string.Empty;
            tax_type = string.Empty;
            serial = string.Empty;
            vendor_mode = string.Empty;
            vendor_company_address = string.Empty;
            vendor_type_name = string.Empty;
            user_type = 0;
        }
    }
}
