/**
 * chaojie_zz添加于2014/11/4 10:46 am
 * 料位管理
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public  class Iplas:PageBase
    {
        public int plas_id { set; get; }//和商品檔的關聯id
        public int dc_id { set; get; }//物流中心編號
        public int whse_id { set; get; }//倉庫編號
        public string loc_id { set; get; }//所關聯的料位編號
        public DateTime change_dtim { set; get; }//最後異動時間戳記，年月時分秒
        public int change_user { set; get; }//最後異動者賬號
        public DateTime create_dtim { set; get; }//創建時間
        public int create_user { set; get; }//創建者
        public string lcus_id { set; get; }//關聯料位P為主料位，主料位裡頭其餘版號為Deep
        public string luis_id { set; get; }//OP=OM為C,OP<>OM為I
        public uint item_id { set; get; }//商品編號
        public int prdd_id { set; get; }//商品序號
        public int loc_rpln_lvl_uoi { set; get; }//觸動補貨量，例如設定為0，當主料位庫存為0時，自動生成補貨工作
        public int loc_stor_cse_cap { set; get; }//指定主料位可以存放多少庫存，人工維護，必須檢查商品的材積和料位可用空間，單位為箱。當iloc.lhnd_id=C,這個欄位才生效
        public string ptwy_anch { set; get; }//系統自動PutAway(收貨時尋找可用副料位)時，尋找可用料位的邏輯設定組
        public string flthru_anch { set; get; }//系統自動PutAway(收貨時尋找可用副料位)時，尋找可用料位的邏輯設定組
        public string pwy_loc_cntl { set; get; }//系統自動PutAway(收貨時尋找可用副料位)時，尋找可用料位的邏輯設定組

        public Iplas()
        {
            plas_id = 0;
            dc_id = 0;
            whse_id = 0;
            loc_id = string.Empty;
            change_dtim = DateTime.MinValue;
           
            create_dtim = DateTime.MinValue;
           
            lcus_id = "P";
            luis_id = string.Empty;
            item_id = 0;
            prdd_id = 1;
            loc_rpln_lvl_uoi =0;
            loc_stor_cse_cap = 0;
            ptwy_anch = string.Empty;
            flthru_anch = string.Empty;
            pwy_loc_cntl = string.Empty;
        }
    }
}
