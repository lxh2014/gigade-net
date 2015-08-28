
Ext.onReady(function () {
    NOT_EMPTY = "<font color='red'>*</font>";
    if (Ext.form.Panel) {
        SITE_NAME = "站臺名稱";
        USER_LEVEL = "會員等級";
        USER_EMAIL = "會員信箱";
        PRODUCT_NAME = "商品顯示名稱";
        FRONT_SHOW_PRICE = "前台顯示建議售價";
        PRODUCT_PRICE_LIST = "建議售價";
       // DEFAULT_BONUS_PERCENT = "購物金回饋倍率";
       // BONUS_PERCENT = "購物金活動回饋倍率";
       // BONUS_PERCENT_TIME = "購物金回饋倍率時間";
        EVENT_TIME = "特價活動期間";
      //  ACCUMULATED_BONUS = "是否發放購物金";
        BAG_CHECK_MONEY = "寄倉費";
        SAME_PRICE = "所有規格同價";
        SAME_PRICE_NOTICE = "若所有規格同價以下表格之售價才會顯示價格";
        SAME_PRICE_NOTICE_2 = "「只需輸入第一列之售價、成本、特價活動售價、特價活動成本」";
        GIGADE_PRODUCT_ITEM = "吉甲地價格細項";
    }
    if (Ext.grid.Panel) {
        ITEM_SPEC1 = "規格1";
        ITEM_SPEC2 = "規格2";
        ITEM_CODE = "廠商編號";
        ITEM_MONEY = "售價";
        ITEM_COST = "成本";
        ITEM_EVENT_MONEY = "特價活動售價";
        ITEM_EVENT_COST = "特價活動成本";
        NEW_SITE_PRICE = "新增站臺價格";
        UPDATE_SITE_PRICE = "修改站臺價格";
        PRICE_UP = "價格上架";
        PRICE_DOWN = "價格下架";
        PRICE_STATUS = "價格狀態";
    }

    if (Ext.Msg.alert) {
        BONUS_PERCENT_EMPTY = "請填寫活動回饋倍率";
        ITEM_MONEY_EMPTY = "售價欄位為0";
     
        ITEM_MONEY_EMPTY = "警告：商品售價為0";
        ITEM_COST_EMPTY = "警告：商品成本為0";
        EVENT_MONEY_EMPTY = "特價活動售價、成本欄位為0";//edit by Jiajun 2014.09.30
        SITE_EXIST = "該站台下已存在此商品信息";
        TIME_ERROR = "活動結束時間必須大於開始時間，請重新選擇。";
    }
});