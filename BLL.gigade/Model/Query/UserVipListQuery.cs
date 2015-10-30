using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class UserVipListQuery : Model.Custom.Users
    {
        public Int64 cou { get; set; }
        public decimal sum_amount { get; set; }
        public decimal sum_bonus { get; set; }
        public decimal freight_normal { get; set; }
        public decimal freight_low { get; set; }
        public uint order_createdate { get; set; }
        public double happygo { get; set; }
        public string vip { get; set; }
        public DateTime reg_date { get; set; }
        public DateTime create_date { get; set; }
        public int aver_amount { get; set; }//平均購買金額
        public uint om_user_id { get; set; }
        public decimal normal_product { get; set; }
        public decimal normal_deduct_bonus { get; set; }
        public decimal low_deduct_bonus { get; set; }
        public decimal low_product { get; set; }
        public decimal ct { get; set; }
        public decimal ht { get; set; }
        public uint create_dateOne { get; set; }
        public uint create_dateTwo { get; set; }
        public string birthday { get; set; }
        public string mytype { get; set; }
        public int age { get; set; }
        public string ml_code { get; set; }
        public int order_product_subtotal { get; set; }
        public UserVipListQuery()
        {

            cou = 0;
            sum_amount = 0;
            sum_bonus = 0;
            freight_normal = 0;
            freight_low = 0;
            order_createdate = 0;
            happygo = 0;
            vip = string.Empty;
            reg_date = DateTime.MinValue;
            create_date = DateTime.MinValue;
            aver_amount = 0;
            om_user_id = 0;
            normal_product = 0;
            normal_deduct_bonus = 0;
            low_deduct_bonus = 0;
            low_product = 0;
            ct = 0;
            ht = 0;
            create_dateOne = 0;
            create_dateTwo = 0;
            birthday = string.Empty;
            mytype = string.Empty;
            age = 0;
            ml_code = string.Empty;
            order_product_subtotal = 0;
        }

    }
}
