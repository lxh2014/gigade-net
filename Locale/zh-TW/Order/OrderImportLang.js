Ext.onReady(function () {
    if (Ext.form.Panel) {
        CHANNEL_NAME = "賣場名稱";
        IMPORT_TYPE = "匯入格式";
        IMPORT_FILE = "選擇欲匯入之訂單(excel)";
        CHOOSE_IMPORT_FILE = "選擇匯入檔案";
        STORE_DISPATCH_FILE = "上傳超商提貨單檔";
        CHOOSE_STORE_DISPATCH_FILE = "選擇提貨單檔";
        IMPORT_SUCCESS_INFO = "共選擇 {0} 筆訂單,成功匯入 {1} 筆訂單";
        WAIT_TITLE = "請稍等";
        FILE_UPLOADING = "文件上傳中...";
        TEMPLATE_DOWNLOAD = "範本下載";
    }
    if (Ext.grid.Panel) {
        SERIAL_NUM = "訂單編號";
        PRODUCT_ID = "商品編號";
        PRODUCT_NAME = "商品名稱";
        SUM_PRICE = "金額小計";
        BUY_COUNT = "數量";
        DELIVERY = "物流";
        ORDER_NAME = "訂購人";
        TRANS_DATE = "轉單日";
        ERROR_INFO = "錯誤信息";
        FILE_REFRESH = "上傳新文件";
    }
});
FILE_TYPE_WRONG = "上傳文件格式錯誤";
IMPORT_TYPE_HOME = "一般";
IMPORT_TYPE_STORE = "有提貨單";
ORDER_SERIAL_ID = "訂單流水號：";
// add by zhuoqin0830w  2015/07/09
SITE = "站臺";

// add by mingwei0727w  2015/07/24
INFORMATION = "提示信息";
PLEASE_DOWN_MODEL_ESSAY_COMPARE="請下載範本與上傳Excel比較后再上傳~";