/**
 * chaojie_zz添加于2014/11/4 11:24 am
 * 料位管理
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class Iprdd:PageBase
    {
        public int row_id { set; get; }//
        public int dc_id { set; get; }//物流中心編號
        public string prod_id { set; get; }//商品編號
        public int prdd_id { set; get; }//序號,類似之一,之二的概念
        public decimal cse_wid { set; get; }//外箱的寬度,單位為公分
        public decimal cse_wgt { set; get; }//外箱的重量,單位為公斤
        public int cse_unit { set; get; }//名詞定義:OM ,外箱單位
        public decimal cse_len { set; get; }//外箱的長度,單位為公分
        public decimal cse_hgt { set; get; }//外箱的長度,單位為公分
        public decimal cse_cub { set; get; }//商品維護了長寬高之後,程序計算得到外箱材積,記錄此欄位
        public DateTime dim_chng_dtim { set; get; }//最近一次材積變更的日期時間
        public int stor_hi { set; get; }//疊板時,有幾層,例如6層,Hi=6
        public int stor_ti { set; get; }//疊板時,每一層有機箱,例如8箱,Ti=8
        public int unit_ship_cse { set; get; }//這個欄位記錄的是商品的op,撿貨次:一個外箱打開裡頭有幾個撿貨單位,等於OM除以OP,例如OP:OM為6:24的一箱可口可樂,24/6=4,所以撿貨次為4。一箱可以滿足4個撿貨次,如果客戶訂貨數量為4,就是出貨一整箱(6*4=24罐)
        public string vend_id { set; get; }//供應商編號
        public int vnds_id { set; get; }//
        public string upc { set; get; }//國際條碼
        public string prod_sz { set; get; }//規格欄位
        public int vend_hi { set; get; }//供應商自己的疊板層數
        public int vend_ti { set; get; }//供應商自己的一層箱數
        public decimal inner_pack_wid { set; get; }//內裝的寬度,單位為公分
        public decimal inner_pack_wgt { set; get; }//內裝的重量,單位為公斤
        public int inner_pack_uint { set; get; }//名詞定義:OP,內裝單位
        public decimal inner_pack_len { set; get; }//內裝的長度,單位為公分
        public decimal inner_pack_hgt { set; get; }//內裝的高度,單位為公分
        public decimal inner_pack_cub { set; get; }//商品維護了長寬高之後,程序計算得到內裝材積,記錄在此欄位
        public decimal eaches_wid { set; get; }//單件的寬度,單位為公分
        public decimal eaches_wgt { set; get; }//單件的重度,單位為公斤
        public decimal eaches_len { set; get; }//單件的長度,單位為公分
        public decimal eaches_hgt { set; get; }//單件的高度,單位為公分
        public decimal eaches_cub { set; get; }//商品維護了長寬高之後,程序計算得到單件的材積,記錄在此欄位
        public DateTime change_dtim { set; get; }//異動的時間戳記,年月日時分秒
        public string change_user { set; get; }//最近一次異動人員帳號
        public DateTime create_dtim { set; get; }//創建的時間戳記,年月日時分秒
        public string create_user { set; get; }//創建人的帳號
        public decimal nest_hgt { set; get; }//nest指的是類似垃圾桶的產品
        public DateTime lst_cnt_dt { set; get; }//本產品最近一次盤點的日期
        public decimal nest_wid { set; get; }//nest指的是類似垃圾桶的產品
        public decimal nest_len { set; get; }//nest指的是類似垃圾桶的產品

        public Iprdd()
        {
            row_id = 0;
            dc_id = 0;
            prod_id = string.Empty;
            prdd_id = 0;
            cse_wid = 0;
            cse_wgt = 0;
            cse_unit = 0;
            cse_len = 0;
            cse_hgt = 0;
            cse_cub = 0;
            dim_chng_dtim = DateTime.MinValue;
            stor_hi = 0;
            unit_ship_cse = 0;
            vend_id = string.Empty;
            vnds_id = 0;
            upc = string.Empty;
            prod_sz = string.Empty;
            vend_hi = 0;
            vend_ti = 0;
            inner_pack_wid = 0;
            inner_pack_wgt = 0;
            inner_pack_uint = 0;
            inner_pack_len = 0;
            inner_pack_hgt = 0;
            inner_pack_cub = 0;
            eaches_wid = 0;
            eaches_wgt = 0;
            eaches_len = 0;
            eaches_hgt = 0;
            eaches_cub = 0;
            change_dtim = DateTime.MinValue;
            change_user = string.Empty;
            create_dtim = DateTime.MinValue;
            create_user = string.Empty;
            nest_hgt = 0;
            lst_cnt_dt = DateTime.MinValue;
            nest_wid = 0;
            nest_len = 0;
        }
     






    }
}
