CNFULLNAME = "中文全名";
ACTION_PHONENO = "行動電話";
CONTACT_PHONENO = "聯絡電話";
POSTNO = "郵遞區號";
CONTACT_ADDRESS = "聯絡地址";
//外站類型
CHANNEL_TYPE_GIGADE = 2;
CHANNEL_TYPE_COOPERATOR = 1;
//物流模式定義
STORE_1 = "黑貓宅急便";
STORE_10 = "黑貓貨到付款";
STORE_12 = "自取";
// 訂單狀態定義
ORDER_STATUS_WAIT_PAYMENT = "等待付款";
ORDER_STATUS_PAYMENT_ERROR = "付款失敗";
ORDER_STATUS_WAIT_DELIVER = "待出貨";
ORDER_STATUS_DELIVER_NOW = "出貨中";
ORDER_STATUS_DELIVER_SUCCESS = "已出貨";
ORDER_STATUS_PROCESS = "處理中";
ORDER_STATUS_DISPATCH = "進倉中";
ORDER_STATUS_DISPATCH_IN = "已進倉";
ORDER_STATUS_DISPATCHED = "已分配";
ORDER_STATUS_WAIT_CANCEL = "等待取消";
ORDER_STATUS_MAN_CHECK = "訂單異常";
ORDER_STATUS_ORDER_SINGLE_CANCEL = "單一商品取消";
ORDER_STATUS_ORDER_CANCEL = " 訂單取消";
ORDER_STATUS_ORDER_RETURN = "訂單退貨";
ORDER_STATUS_ORDER_CHANGE = "訂單換貨";
ORDER_STATUS_ORDER_SUCCESS = "訂單歸檔";
//標題
BUYER_INFO = "訂購人資料";
SAME_AS_BUYER = "同訂購人";
SAME_AS_RECEIVER = "同收件人";
RECEIVER_INFO = "收件人資料";
CHANNEL_NAME = "賣場名稱";
PAYMENT_WAY = "付款方式";
STORE_MODE = "物流模式";
ORDER_STATUS = "訂單狀態";
ORDER_DATE = "訂單日期";
ORDER_ID = "訂單子編號";
SUPER_STORE = "超商店家";
TRADE_NUMBER = "付款單號";
DELIVERY_NUMBER = "賣場出貨單號";
LATEST_DELIVERY_DATE = "最晚出貨日";
ADMIN_NOTE = "管理員備註";
CART_NOTE = "訂單備註";
TIME_YMD = "年/月/日";
TIME_HOUR = "時";
TIME_MINUTE = "分";
BTN_ADD = "新增";
BTN_SURE = "確定";
//提示用語
OUTSITE_SELECT = "請選擇賣場";
FOURNUMBER_INPUT = "請輸入正確編號";
FORMAT_ERROR = "格式有誤";
PHONENO_ERROR = "移動號碼必須以 09 開頭的十位數字";
VALUE_EMPTY = "此欄位為必填";
COMPLETE_ORDER = "請完善訂單信息";
//商品信息表頭定義
PRODUCT_ID = "賣場商品編號";
PRODUCT_NAME = "商品名稱";
SPEC1 = "規格一";
SPEC2 = "規格二";
COST = "成本";
PRICE = "定價";
EVENT_COST = "活動成本";
//EVENT_PRICE = "活動價";
STOCK = "庫存";
BUY_NUM = "數量";
SUM_PRICE = "金額小計";
TOTAL_PRICE = "總價";
NORMAL_FREIGHT = "常溫運費";
LOW_FREIGHT = "低溫運費";
FREIGHT_FORMAT_WRONG = "運費格式不正確";
SET_FREIGHT = "請設置運費";
SET_CHANNEL = "請設置賣場";
PRODUCT_ID_WRONG = "賣場商品編號錯誤";
INFORMATION = "提示消息";
ADDORDER_SUCCESS = "成功生成訂單";
ADDORDER_FAILURE = "訂單生成失敗";
NOTHING_ORDER = "無下單商品";
PRODUCT = "商品";
PRODUCT_BUYNUM_ISNULL = "購買數量為0，無法生成訂單!";
PLEASE_WAIT = "請稍等...";
Ext.onReady(function () {
    if (Ext.panel.Panel) {
        DELIVERY = "物流";
        ORDER_NAME = "訂購人";
        TRANS_DATE = "轉單日";
    }
});
SELECT = "請選擇";
CHILD_PRODUCT_NAME = "子商品名稱";
PROMPT = "提示";
CHILD_PROD_HAS_SELECTED = "此單一商品已經選擇，請重新選擇";
ORDER_ISREST = '假日可否收件';
YES = '是';
NO = '否';
IDSRESTRICT = '不限時';
PRE12 = '12:00 以前';
AFTERNOON = '12:00~17:00';
NIGHT = '17:00~20:00';
//add by zhuoqin0830w  2015/02/25  公關單與報廢單功能
BILLTYPE = "單據類型";
DEPTTYPE = "部門類別";
DEDUCT_BONUS = "購物金金額";
DEDUCT_WELFARE = "抵用券金額";
EVENT_COST = "活動成本";
//add by zhuoqin0830w  2015/07/09
SITE = "站臺";
HOPE_TIME = "希望收貨時段";
GENDER = "性別";
BOY = "男";
GIRL = "女";
CHILD_PRODUCT_BUYNUM_ISNULL = "子商品的購買數量不能為 0 ";
GROUP_PRODUCT_BUYNUM = "當前組合商品需選擇數量為：";
CHILD_PRODUCT_BUYNUM = "子商品的購買數量為：";
PLEASE_CHOOSE_AGAIN = ",請重新選擇~";
PRODUCT_STATUS = "商品狀態";

//add by mingwei0727w 2015/07/24
GIGADE = "吉甲地";

//add by zhuoqin0830w  2015/07/31
ACCUMULATED_BONUS = "返還購物金";

//add by zhuoqin0830w 2015/08/26
TOTAL_PRICE_ISNULL = "總價必須為正數！";