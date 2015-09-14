
Ext.onReady(function () {

    TITLE_BASE_INFO = "商品基本資料";
    TITLE_DESCRIPTION = "描述";
    TITLE_SPEC = "規格";
    TITLE_PRICE = "價格";
    TITLE_CATEGORY = "類別";
    TITLE_STOCK = "庫存";
    TITLE_PICTURE = "圖檔";
    TITLE_PRIZE = "抽獎";

    NEXT_MOVE = "下一步";
    PERV_MOVE = "上一步";

    BTN_SAVE = "保存";
    BTN_TEMP_SAVE = "儲存";


    if (Ext.Msg.confirm) {
        PROMPT = "提示";
        CONFIRM = "確認";
        DELETE_INFO = "删除选中 {0} 条数据？";
        IS_CONTNUES = '上次操作數據未保存，是否繼續？<br/> "Yes" : 繼續上次操作<br/> "No" : 刪除臨時數據';
    }

});

//add by mingwei0727w 2015/07/17
PHYSICAL_DISTRIBUTION_DISPATCH_MODE = "物流配送模式";
CURRICULUM = "課程";
NEW_PRODUCT_CATEGORY = "新類別";