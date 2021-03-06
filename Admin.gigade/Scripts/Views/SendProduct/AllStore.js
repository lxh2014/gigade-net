﻿//廠商批次出貨單查詢條件
var searchStore = Ext.create('Ext.data.Store', {
    fields: ['text', 'value'],
    data: [
    { "text": '所有資料', "value": "0" },
    { "text": '批次出貨單號', "value": "1" },
    { "text": '供應商名稱', "value": "2" }
    ]
});
var dateTypeStore = Ext.create('Ext.data.Store', {
    fields: ['text', 'value'],
    data: [
    { "text": '所有日期', "value": "0" },
    { "text": '建立日期', "value": "1" },
    { "text": '出貨日期', "value": "2" }
    ]
});
//預計到貨時段
var ArrivalPeriodStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { "txt": "不限時", "value": "0" },
    { "txt": "12:00以前", "value": "1" },
    { "txt": "12:00-17:00", "value": "2" },
    { "txt": "17:00-20:00", "value": "3" }

    ]
});
//訂單狀態
var OrderStatusStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { "txt": "等待付款", "value": "0" },
    { "txt": "付款失敗", "value": "1" },
    { "txt": "待出貨", "value": "2" },
    { "txt": "出貨中", "value": "3" },
    { "txt": "已出貨", "value": "4" },
    { "txt": "處理中", "value": "5" },
    { "txt": "進倉中", "value": "6" },
    { "txt": "已進倉", "value": "7" },
    { "txt": "扣點中", "value": "8" },
    { "txt": "待取貨", "value": "9" },
    { "txt": "等待取消", "value": "10" },
    { "txt": "訂單異常", "value": "20" },
    { "txt": "單一商品取消", "value": "89" },
    { "txt": "訂單取消", "value": "90" },
    { "txt": "訂單退貨", "value": "91" },
    { "txt": "訂單換貨", "value": "92" },
    { "txt": "訂單歸檔", "value": "99" }
    ]
});

//付款方式
Ext.define("gigade.paraModel", {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'parameterCode', type: 'string' },
    { name: 'parameterName', type: 'string' }
    ]
});
var paymentStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/OrderManage/QueryPara?paraType=payment',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});
//付款單狀態
Ext.define('gigade.PaymentTypes', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "ParameterCode", type: "string" },
    { name: "remark", type: "string" }
    ]
});
var paymentType = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'gigade.PaymentTypes',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetPayMentType',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
//供應商Model
Ext.define("gigade.Vendor", {
    extend: 'Ext.data.Model',
    fields: [
    { name: "vendor_id", type: "string" },
    { name: "vendor_name_simple", type: "string" }
    ]
});
//供應商Store
var CateStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Vendor',
    storeId: 'vendorstore',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: "/SendProduct/GetVendorName?type=1",
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

//查詢條件【營管>供應商出貨單】
var ShippedQueryStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { "txt": "所有訂單資料", "value": "0" },
    { "txt": "訂單編號", "value": "1" },
    { "txt": "訂購人姓名", "value": "2" },
    { "txt": "收貨人姓名", "value": "4" }
    ]
});
//供應商條件-出貨廠商【營管>供應商出貨單\出貨查詢】--from vendor where assist=1

//外站出貨檔匯出
//var VendorStore = Ext.create('Ext.data.Store', {
//    model: 'gigade.Vendor',
//    autoLoad: true,
//    proxy: {
//        type: 'ajax',
//        url: "/Vendor/GetVendor",
//        noCache: false,
//        getMethod: function () { return 'get'; },
//        timeout: 180000,
//        actionMethods: 'post',
//        reader: {
//            type: 'json',
//            root: 'data'
//        }
//    }
//});
var VendorConditionStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Vendor',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: "/SendProduct/GetVendorName",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
//出貨類別【營管>出貨查詢】
var ShipCategoryStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { "txt": "統倉出貨", "value": "1" },
    { "txt": "供應商自行出貨", "value": "2" },
    { "txt": "供應商調度出貨", "value": "3" },
    { "txt": "退貨", "value": "4" },
    { "txt": "退貨瑕疵", "value": "5" },
    { "txt": "瑕疵", "value": "6" }
    ]
});
//出貨單出貨狀態【營管>出貨查詢】
var DeliveryStatusStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { "txt": "全部", "value": "-1" },
    { "txt": "待出貨", "value": "0" },
    { "txt": "可出貨", "value": "1" },
    { "txt": "出貨中", "value": "2" },
    { "txt": "已出貨", "value": "3" },
    { "txt": "已到貨", "value": "4" },
    { "txt": "未到貨", "value": "5" },
    { "txt": "取消出貨", "value": "6" },
    { "txt": "待取貨", "value": "7" }
    ]
});

//物流商【營管>出貨查詢】
var DeliverStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/OrderManage/QueryPara?paraType=Deliver_Store',
        noCache: false,
        getMethod: function () { return 'get'; },
        timeout: 180000,
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});
//賣場
Ext.define("gigade.channel", {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'channel_id', type: "int" },
    { name: 'channel_name_simple', type: "string" }]
});
var ChannelStore = Ext.create("Ext.data.Store", {
    model: 'gigade.channel',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/SendProduct/GetChannel',
        noCache: false,
        getMethod: function () { return 'get'; },
        timeout: 180000,
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
//取貨模式
var GetGoodsWayStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/OrderManage/QueryPara?paraType=retrieve_mode',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});
Ext.define("gigade.user_zip", {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'user_zip', type: "int" },
    { name: 'user_zip_name', type: "string" }]
});
var user_zip_source = Ext.create('Ext.data.Store', {
    model: 'gigade.user_zip',
    autoLoad: false,
    data: [
    { user_zip: 0, user_zip_name: "請選擇郵遞區號" },
    { user_zip: 100, user_zip_name: "100 臺北市 中正區" },
    { user_zip: 103, user_zip_name: "103 臺北市 大同區" },
    { user_zip: 104, user_zip_name: "104 臺北市 中山區" },
    { user_zip: 105, user_zip_name: "105 臺北市 松山區" },
    { user_zip: 106, user_zip_name: "106 臺北市 大安區" },
    { user_zip: 108, user_zip_name: "108 臺北市 萬華區" },
    { user_zip: 110, user_zip_name: "110 臺北市 信義區" },
    { user_zip: 111, user_zip_name: "111 臺北市 士林區" },
    { user_zip: 112, user_zip_name: "112 臺北市 北投區" },
    { user_zip: 114, user_zip_name: "114 臺北市 內湖區" },
    { user_zip: 115, user_zip_name: "115 臺北市 南港區" },
    { user_zip: 116, user_zip_name: "116 臺北市 文山區" },
    { user_zip: 200, user_zip_name: "200 基隆市 仁愛區" },
    { user_zip: 201, user_zip_name: "201 基隆市 信義區" },
    { user_zip: 202, user_zip_name: "202 基隆市 中正區" },
    { user_zip: 203, user_zip_name: "203 基隆市 中山區" },
    { user_zip: 204, user_zip_name: "204 基隆市 安樂區" },
    { user_zip: 205, user_zip_name: "205 基隆市 暖暖區" },
    { user_zip: 206, user_zip_name: "206 基隆市 七堵區" },
    { user_zip: 207, user_zip_name: "207 新北市 萬里區" },
    { user_zip: 208, user_zip_name: "208 新北市 金山區" },
    { user_zip: 220, user_zip_name: "220 新北市 板橋區" },
    { user_zip: 221, user_zip_name: "221 新北市 汐止區" },
    { user_zip: 222, user_zip_name: "222 新北市 深坑區" },
    { user_zip: 223, user_zip_name: "223 新北市 石碇區" },
    { user_zip: 224, user_zip_name: "224 新北市 瑞芳區" },
    { user_zip: 226, user_zip_name: "226 新北市 平溪區" },
    { user_zip: 227, user_zip_name: "227 新北市 雙溪區" },
    { user_zip: 228, user_zip_name: "228 新北市 貢寮區" },
    { user_zip: 231, user_zip_name: "231 新北市 新店區" },
    { user_zip: 232, user_zip_name: "232 新北市 坪林區" },
    { user_zip: 233, user_zip_name: "233 新北市 烏來區" },
    { user_zip: 234, user_zip_name: "234 新北市 永和區" },
    { user_zip: 235, user_zip_name: "235 新北市 中和區" },
    { user_zip: 236, user_zip_name: "236 新北市 土城區" },
    { user_zip: 237, user_zip_name: "237 新北市 三峽區" },
    { user_zip: 238, user_zip_name: "238 新北市 樹林區" },
    { user_zip: 239, user_zip_name: "239 新北市 鶯歌區" },
    { user_zip: 241, user_zip_name: "241 新北市 三重區" },
    { user_zip: 242, user_zip_name: "242 新北市 新莊區" },
    { user_zip: 243, user_zip_name: "243 新北市 泰山區" },
    { user_zip: 244, user_zip_name: "244 新北市 林口區" },
    { user_zip: 247, user_zip_name: "247 新北市 蘆洲區" },
    { user_zip: 248, user_zip_name: "248 新北市 五股區" },
    { user_zip: 249, user_zip_name: "249 新北市 八里區" },
    { user_zip: 251, user_zip_name: "251 新北市 淡水區" },
    { user_zip: 252, user_zip_name: "252 新北市 三芝區" },
    { user_zip: 253, user_zip_name: "253 新北市 石門區" },
    { user_zip: 260, user_zip_name: "260 宜蘭縣 宜蘭市" },
    { user_zip: 261, user_zip_name: "261 宜蘭縣 頭城鎮" },
    { user_zip: 262, user_zip_name: "262 宜蘭縣 礁溪鄉" },
    { user_zip: 263, user_zip_name: "263 宜蘭縣 壯圍鄉" },
    { user_zip: 264, user_zip_name: "264 宜蘭縣 員山鄉" },
    { user_zip: 265, user_zip_name: "265 宜蘭縣 羅東鎮" },
    { user_zip: 266, user_zip_name: "266 宜蘭縣 三星鄉" },
    { user_zip: 267, user_zip_name: "267 宜蘭縣 大同鄉" },
    { user_zip: 268, user_zip_name: "268 宜蘭縣 五結鄉" },
    { user_zip: 269, user_zip_name: "269 宜蘭縣 冬山鄉" },
    { user_zip: 270, user_zip_name: "270 宜蘭縣 蘇澳鎮" },
    { user_zip: 272, user_zip_name: "272 宜蘭縣 南澳鄉" },
    { user_zip: 300, user_zip_name: "300 新竹市" },
    { user_zip: 302, user_zip_name: "302 新竹縣 竹北市" },
    { user_zip: 303, user_zip_name: "303 新竹縣 湖口鄉" },
    { user_zip: 304, user_zip_name: "304 新竹縣 新豐鄉" },
    { user_zip: 305, user_zip_name: "305 新竹縣 新埔鎮" },
    { user_zip: 306, user_zip_name: "306 新竹縣 關西鎮" },
    { user_zip: 307, user_zip_name: "307 新竹縣 芎林鄉" },
    { user_zip: 308, user_zip_name: "308 新竹縣 寶山鄉" },
    { user_zip: 310, user_zip_name: "310 新竹縣 竹東鎮" },
    { user_zip: 311, user_zip_name: "311 新竹縣 五峰鄉" },
    { user_zip: 312, user_zip_name: "312 新竹縣 橫山鄉" },
    { user_zip: 313, user_zip_name: "313 新竹縣 尖石鄉" },
    { user_zip: 314, user_zip_name: "314 新竹縣 北埔鄉" },
    { user_zip: 315, user_zip_name: "315 新竹縣 峨眉鄉" },
    { user_zip: 320, user_zip_name: "320 桃園市 中壢區" },
    { user_zip: 324, user_zip_name: "324 桃園市 平鎮區" },
    { user_zip: 325, user_zip_name: "325 桃園市 龍潭區" },
    { user_zip: 326, user_zip_name: "326 桃園市 楊梅區" },
    { user_zip: 327, user_zip_name: "327 桃園市 新屋區" },
    { user_zip: 328, user_zip_name: "328 桃園市 觀音區" },
    { user_zip: 330, user_zip_name: "330 桃園市 桃園區" },
    { user_zip: 333, user_zip_name: "333 桃園市 龜山區" },
    { user_zip: 334, user_zip_name: "334 桃園市 八德區" },
    { user_zip: 335, user_zip_name: "335 桃園市 大溪區" },
    { user_zip: 336, user_zip_name: "336 桃園市 復興區" },
    { user_zip: 337, user_zip_name: "337 桃園市 大園區" },
    { user_zip: 338, user_zip_name: "338 桃園市 蘆竹區" },
    { user_zip: 350, user_zip_name: "350 苗栗縣 竹南鎮" },
    { user_zip: 351, user_zip_name: "351 苗栗縣 頭份鎮" },
    { user_zip: 352, user_zip_name: "352 苗栗縣 三灣鄉" },
    { user_zip: 353, user_zip_name: "353 苗栗縣 南庄鄉" },
    { user_zip: 354, user_zip_name: "354 苗栗縣 獅潭鄉" },
    { user_zip: 356, user_zip_name: "356 苗栗縣 後龍鎮" },
    { user_zip: 357, user_zip_name: "357 苗栗縣 通霄鎮" },
    { user_zip: 358, user_zip_name: "358 苗栗縣 苑裡鎮" },
    { user_zip: 360, user_zip_name: "360 苗栗縣 苗栗市" },
    { user_zip: 361, user_zip_name: "361 苗栗縣 造橋鄉" },
    { user_zip: 362, user_zip_name: "362 苗栗縣 頭屋鄉" },
    { user_zip: 363, user_zip_name: "363 苗栗縣 公館鄉" },
    { user_zip: 364, user_zip_name: "364 苗栗縣 大湖鄉" },
    { user_zip: 365, user_zip_name: "365 苗栗縣 泰安鄉" },
    { user_zip: 366, user_zip_name: "366 苗栗縣 銅鑼鄉" },
    { user_zip: 367, user_zip_name: "367 苗栗縣 三義鄉" },
    { user_zip: 368, user_zip_name: "368 苗栗縣 西湖鄉" },
    { user_zip: 369, user_zip_name: "369 苗栗縣 卓蘭鎮" },
    { user_zip: 400, user_zip_name: "400 臺中市 中　區" },
    { user_zip: 401, user_zip_name: "401 臺中市 東　區" },
    { user_zip: 402, user_zip_name: "402 臺中市 南　區" },
    { user_zip: 403, user_zip_name: "403 臺中市 西　區" },
    { user_zip: 404, user_zip_name: "404 臺中市 北　區" },
    { user_zip: 406, user_zip_name: "406 臺中市 北屯區" },
    { user_zip: 407, user_zip_name: "407 臺中市 西屯區" },
    { user_zip: 408, user_zip_name: "408 臺中市 南屯區" },
    { user_zip: 411, user_zip_name: "411 臺中市 太平區" },
    { user_zip: 412, user_zip_name: "412 臺中市 大里區" },
    { user_zip: 413, user_zip_name: "413 臺中市 霧峰區" },
    { user_zip: 414, user_zip_name: "414 臺中市 烏日區" },
    { user_zip: 420, user_zip_name: "420 臺中市 豐原區" },
    { user_zip: 421, user_zip_name: "421 臺中市 后里區" },
    { user_zip: 422, user_zip_name: "422 臺中市 石岡區" },
    { user_zip: 423, user_zip_name: "423 臺中市 東勢區" },
    { user_zip: 424, user_zip_name: "424 臺中市 和平區" },
    { user_zip: 426, user_zip_name: "426 臺中市 新社區" },
    { user_zip: 427, user_zip_name: "427 臺中市 潭子區" },
    { user_zip: 428, user_zip_name: "428 臺中市 大雅區" },
    { user_zip: 429, user_zip_name: "429 臺中市 神岡區" },
    { user_zip: 432, user_zip_name: "432 臺中市 大肚區" },
    { user_zip: 433, user_zip_name: "433 臺中市 沙鹿區" },
    { user_zip: 434, user_zip_name: "434 臺中市 龍井區" },
    { user_zip: 435, user_zip_name: "435 臺中市 梧棲區" },
    { user_zip: 436, user_zip_name: "436 臺中市 清水區" },
    { user_zip: 437, user_zip_name: "437 臺中市 大甲區" },
    { user_zip: 438, user_zip_name: "438 臺中市 外埔區" },
    { user_zip: 439, user_zip_name: "439 臺中市 大安區" },
    { user_zip: 500, user_zip_name: "500 彰化縣 彰化市" },
    { user_zip: 502, user_zip_name: "502 彰化縣 芬園鄉" },
    { user_zip: 503, user_zip_name: "503 彰化縣 花壇鄉" },
    { user_zip: 504, user_zip_name: "504 彰化縣 秀水鄉" },
    { user_zip: 505, user_zip_name: "505 彰化縣 鹿港鎮" },
    { user_zip: 506, user_zip_name: "506 彰化縣 福興鄉" },
    { user_zip: 507, user_zip_name: "507 彰化縣 線西鄉" },
    { user_zip: 508, user_zip_name: "508 彰化縣 和美鎮" },
    { user_zip: 509, user_zip_name: "509 彰化縣 伸港鄉" },
    { user_zip: 510, user_zip_name: "510 彰化縣 員林鎮" },
    { user_zip: 511, user_zip_name: "511 彰化縣 社頭鄉" },
    { user_zip: 512, user_zip_name: "512 彰化縣 永靖鄉" },
    { user_zip: 513, user_zip_name: "513 彰化縣 埔心鄉" },
    { user_zip: 514, user_zip_name: "514 彰化縣 溪湖鎮" },
    { user_zip: 515, user_zip_name: "515 彰化縣 大村鄉" },
    { user_zip: 516, user_zip_name: "516 彰化縣 埔鹽鄉" },
    { user_zip: 520, user_zip_name: "520 彰化縣 田中鎮" },
    { user_zip: 521, user_zip_name: "521 彰化縣 北斗鎮" },
    { user_zip: 522, user_zip_name: "522 彰化縣 田尾鄉" },
    { user_zip: 523, user_zip_name: "523 彰化縣 埤頭鄉" },
    { user_zip: 524, user_zip_name: "524 彰化縣 溪州鄉" },
    { user_zip: 525, user_zip_name: "525 彰化縣 竹塘鄉" },
    { user_zip: 526, user_zip_name: "526 彰化縣 二林鎮" },
    { user_zip: 527, user_zip_name: "527 彰化縣 大城鄉" },
    { user_zip: 528, user_zip_name: "528 彰化縣 芳苑鄉" },
    { user_zip: 530, user_zip_name: "530 彰化縣 二水鄉" },
    { user_zip: 540, user_zip_name: "540 南投縣 南投市" },
    { user_zip: 541, user_zip_name: "541 南投縣 中寮鄉" },
    { user_zip: 542, user_zip_name: "542 南投縣 草屯鎮" },
    { user_zip: 544, user_zip_name: "544 南投縣 國姓鄉" },
    { user_zip: 545, user_zip_name: "545 南投縣 埔里鎮" },
    { user_zip: 546, user_zip_name: "546 南投縣 仁愛鄉" },
    { user_zip: 551, user_zip_name: "551 南投縣 名間鄉" },
    { user_zip: 552, user_zip_name: "552 南投縣 集集鎮" },
    { user_zip: 553, user_zip_name: "553 南投縣 水里鄉" },
    { user_zip: 555, user_zip_name: "555 南投縣 魚池鄉" },
    { user_zip: 556, user_zip_name: "556 南投縣 信義鄉" },
    { user_zip: 557, user_zip_name: "557 南投縣 竹山鎮" },
    { user_zip: 558, user_zip_name: "558 南投縣 鹿谷鄉" },
    { user_zip: 600, user_zip_name: "600 嘉義市" },
    { user_zip: 602, user_zip_name: "602 嘉義縣 番路鄉" },
    { user_zip: 603, user_zip_name: "603 嘉義縣 梅山鄉" },
    { user_zip: 604, user_zip_name: "604 嘉義縣 竹崎鄉" },
    { user_zip: 605, user_zip_name: "605 嘉義縣 阿里山" },
    { user_zip: 606, user_zip_name: "606 嘉義縣 中埔鄉" },
    { user_zip: 607, user_zip_name: "607 嘉義縣 大埔鄉" },
    { user_zip: 608, user_zip_name: "608 嘉義縣 水上鄉" },
    { user_zip: 611, user_zip_name: "611 嘉義縣 鹿草鄉" },
    { user_zip: 612, user_zip_name: "612 嘉義縣 太保市" },
    { user_zip: 613, user_zip_name: "613 嘉義縣 朴子市" },
    { user_zip: 614, user_zip_name: "614 嘉義縣 東石鄉" },
    { user_zip: 615, user_zip_name: "615 嘉義縣 六腳鄉" },
    { user_zip: 616, user_zip_name: "616 嘉義縣 新港鄉" },
    { user_zip: 621, user_zip_name: "621 嘉義縣 民雄鄉" },
    { user_zip: 622, user_zip_name: "622 嘉義縣 大林鎮" },
    { user_zip: 623, user_zip_name: "623 嘉義縣 溪口鄉" },
    { user_zip: 624, user_zip_name: "624 嘉義縣 義竹鄉" },
    { user_zip: 625, user_zip_name: "625 嘉義縣 布袋鎮" },
    { user_zip: 630, user_zip_name: "630 雲林縣 斗南鎮" },
    { user_zip: 631, user_zip_name: "631 雲林縣 大埤鄉" },
    { user_zip: 632, user_zip_name: "632 雲林縣 虎尾鎮" },
    { user_zip: 633, user_zip_name: "633 雲林縣 土庫鎮" },
    { user_zip: 634, user_zip_name: "634 雲林縣 褒忠鄉" },
    { user_zip: 635, user_zip_name: "635 雲林縣 東勢鄉" },
    { user_zip: 636, user_zip_name: "636 雲林縣 臺西鄉" },
    { user_zip: 637, user_zip_name: "637 雲林縣 崙背鄉" },
    { user_zip: 638, user_zip_name: "638 雲林縣 麥寮鄉" },
    { user_zip: 640, user_zip_name: "640 雲林縣 斗六市" },
    { user_zip: 643, user_zip_name: "643 雲林縣 林內鄉" },
    { user_zip: 646, user_zip_name: "646 雲林縣 古坑鄉" },
    { user_zip: 647, user_zip_name: "647 雲林縣 莿桐鄉" },
    { user_zip: 648, user_zip_name: "648 雲林縣 西螺鎮" },
    { user_zip: 649, user_zip_name: "649 雲林縣 二崙鄉" },
    { user_zip: 651, user_zip_name: "651 雲林縣 北港鎮" },
    { user_zip: 652, user_zip_name: "652 雲林縣 水林鄉" },
    { user_zip: 653, user_zip_name: "653 雲林縣 口湖鄉" },
    { user_zip: 654, user_zip_name: "654 雲林縣 四湖鄉" },
    { user_zip: 655, user_zip_name: "655 雲林縣 元長鄉" },
    { user_zip: 700, user_zip_name: "700 臺南市 中西區" },
    { user_zip: 701, user_zip_name: "701 臺南市 東　區" },
    { user_zip: 702, user_zip_name: "702 臺南市 南　區" },
    { user_zip: 704, user_zip_name: "704 臺南市 北　區" },
    { user_zip: 708, user_zip_name: "708 臺南市 安平區" },
    { user_zip: 709, user_zip_name: "709 臺南市 安南區" },
    { user_zip: 710, user_zip_name: "710 臺南市 永康區" },
    { user_zip: 711, user_zip_name: "711 臺南市 歸仁區" },
    { user_zip: 712, user_zip_name: "712 臺南市 新化區" },
    { user_zip: 713, user_zip_name: "713 臺南市 左鎮區" },
    { user_zip: 714, user_zip_name: "714 臺南市 玉井區" },
    { user_zip: 715, user_zip_name: "715 臺南市 楠西區" },
    { user_zip: 716, user_zip_name: "716 臺南市 南化區" },
    { user_zip: 717, user_zip_name: "717 臺南市 仁德區" },
    { user_zip: 718, user_zip_name: "718 臺南市 關廟區" },
    { user_zip: 719, user_zip_name: "719 臺南市 龍崎區" },
    { user_zip: 720, user_zip_name: "720 臺南市 官田區" },
    { user_zip: 721, user_zip_name: "721 臺南市 麻豆區" },
    { user_zip: 722, user_zip_name: "722 臺南市 佳里區" },
    { user_zip: 723, user_zip_name: "723 臺南市 西港區" },
    { user_zip: 724, user_zip_name: "724 臺南市 七股區" },
    { user_zip: 725, user_zip_name: "725 臺南市 將軍區" },
    { user_zip: 726, user_zip_name: "726 臺南市 學甲區" },
    { user_zip: 727, user_zip_name: "727 臺南市 北門區" },
    { user_zip: 730, user_zip_name: "730 臺南市 新營區" },
    { user_zip: 731, user_zip_name: "731 臺南市 後壁區" },
    { user_zip: 732, user_zip_name: "732 臺南市 白河區" },
    { user_zip: 733, user_zip_name: "733 臺南市 東山區" },
    { user_zip: 734, user_zip_name: "734 臺南市 六甲區" },
    { user_zip: 735, user_zip_name: "735 臺南市 下營區" },
    { user_zip: 736, user_zip_name: "736 臺南市 柳營區" },
    { user_zip: 737, user_zip_name: "737 臺南市 鹽水區" },
    { user_zip: 741, user_zip_name: "741 臺南市 善化區" },
    { user_zip: 742, user_zip_name: "742 臺南市 大內區" },
    { user_zip: 743, user_zip_name: "743 臺南市 山上區" },
    { user_zip: 744, user_zip_name: "744 臺南市 新市區" },
    { user_zip: 745, user_zip_name: "745 臺南市 安定區" },
    { user_zip: 800, user_zip_name: "800 高雄市 新興區" },
    { user_zip: 801, user_zip_name: "801 高雄市 前金區" },
    { user_zip: 802, user_zip_name: "802 高雄市 苓雅區" },
    { user_zip: 803, user_zip_name: "803 高雄市 鹽埕區" },
    { user_zip: 804, user_zip_name: "804 高雄市 鼓山區" },
    { user_zip: 805, user_zip_name: "805 高雄市 旗津區" },
    { user_zip: 806, user_zip_name: "806 高雄市 前鎮區" },
    { user_zip: 807, user_zip_name: "807 高雄市 三民區" },
    { user_zip: 811, user_zip_name: "811 高雄市 楠梓區" },
    { user_zip: 812, user_zip_name: "812 高雄市 小港區" },
    { user_zip: 813, user_zip_name: "813 高雄市 左營區" },
    { user_zip: 814, user_zip_name: "814 高雄市 仁武區" },
    { user_zip: 815, user_zip_name: "815 高雄市 大社區" },
    { user_zip: 820, user_zip_name: "820 高雄市 岡山區" },
    { user_zip: 821, user_zip_name: "821 高雄市 路竹區" },
    { user_zip: 822, user_zip_name: "822 高雄市 阿蓮區" },
    { user_zip: 823, user_zip_name: "823 高雄市 田寮區" },
    { user_zip: 824, user_zip_name: "824 高雄市 燕巢區" },
    { user_zip: 825, user_zip_name: "825 高雄市 橋頭區" },
    { user_zip: 826, user_zip_name: "826 高雄市 梓官區" },
    { user_zip: 827, user_zip_name: "827 高雄市 彌陀區" },
    { user_zip: 828, user_zip_name: "828 高雄市 永安區" },
    { user_zip: 829, user_zip_name: "829 高雄市 湖內區" },
    { user_zip: 830, user_zip_name: "830 高雄市 鳳山區" },
    { user_zip: 831, user_zip_name: "831 高雄市 大寮區" },
    { user_zip: 832, user_zip_name: "832 高雄市 林園區" },
    { user_zip: 833, user_zip_name: "833 高雄市 鳥松區" },
    { user_zip: 840, user_zip_name: "840 高雄市 大樹區" },
    { user_zip: 842, user_zip_name: "842 高雄市 旗山區" },
    { user_zip: 843, user_zip_name: "843 高雄市 美濃區" },
    { user_zip: 844, user_zip_name: "844 高雄市 六龜區" },
    { user_zip: 845, user_zip_name: "845 高雄市 內門區" },
    { user_zip: 846, user_zip_name: "846 高雄市 杉林區" },
    { user_zip: 847, user_zip_name: "847 高雄市 甲仙區" },
    { user_zip: 848, user_zip_name: "848 高雄市 桃源區" },
    { user_zip: 849, user_zip_name: "849 高雄市 那瑪夏區" },
    { user_zip: 851, user_zip_name: "851 高雄市 茂林區" },
    { user_zip: 852, user_zip_name: "852 高雄市 茄萣區" },
    { user_zip: 880, user_zip_name: "880 澎湖縣 馬公市" },
    { user_zip: 881, user_zip_name: "881 澎湖縣 西嶼鄉" },
    { user_zip: 882, user_zip_name: "882 澎湖縣 望安鄉" },
    { user_zip: 883, user_zip_name: "883 澎湖縣 七美鄉" },
    { user_zip: 884, user_zip_name: "884 澎湖縣 白沙鄉" },
    { user_zip: 885, user_zip_name: "885 澎湖縣 湖西鄉" },
    { user_zip: 900, user_zip_name: "900 屏東縣 屏東市" },
    { user_zip: 901, user_zip_name: "901 屏東縣 三地門" },
    { user_zip: 902, user_zip_name: "902 屏東縣 霧臺鄉" },
    { user_zip: 903, user_zip_name: "903 屏東縣 瑪家鄉" },
    { user_zip: 904, user_zip_name: "904 屏東縣 九如鄉" },
    { user_zip: 905, user_zip_name: "905 屏東縣 里港鄉" },
    { user_zip: 906, user_zip_name: "906 屏東縣 高樹鄉" },
    { user_zip: 907, user_zip_name: "907 屏東縣 鹽埔鄉" },
    { user_zip: 908, user_zip_name: "908 屏東縣 長治鄉" },
    { user_zip: 909, user_zip_name: "909 屏東縣 麟洛鄉" },
    { user_zip: 911, user_zip_name: "911 屏東縣 竹田鄉" },
    { user_zip: 912, user_zip_name: "912 屏東縣 內埔鄉" },
    { user_zip: 913, user_zip_name: "913 屏東縣 萬丹鄉" },
    { user_zip: 920, user_zip_name: "920 屏東縣 潮州鎮" },
    { user_zip: 921, user_zip_name: "921 屏東縣 泰武鄉" },
    { user_zip: 922, user_zip_name: "922 屏東縣 來義鄉" },
    { user_zip: 923, user_zip_name: "923 屏東縣 萬巒鄉" },
    { user_zip: 924, user_zip_name: "924 屏東縣 崁頂鄉" },
    { user_zip: 925, user_zip_name: "925 屏東縣 新埤鄉" },
    { user_zip: 926, user_zip_name: "926 屏東縣 南州鄉" },
    { user_zip: 927, user_zip_name: "927 屏東縣 林邊鄉" },
    { user_zip: 928, user_zip_name: "928 屏東縣 東港鎮" },
    { user_zip: 929, user_zip_name: "929 屏東縣 琉球鄉" },
    { user_zip: 931, user_zip_name: "931 屏東縣 佳冬鄉" },
    { user_zip: 932, user_zip_name: "932 屏東縣 新園鄉" },
    { user_zip: 940, user_zip_name: "940 屏東縣 枋寮鄉" },
    { user_zip: 941, user_zip_name: "941 屏東縣 枋山鄉" },
    { user_zip: 942, user_zip_name: "942 屏東縣 春日鄉" },
    { user_zip: 943, user_zip_name: "943 屏東縣 獅子鄉" },
    { user_zip: 944, user_zip_name: "944 屏東縣 車城鄉" },
    { user_zip: 945, user_zip_name: "945 屏東縣 牡丹鄉" },
    { user_zip: 946, user_zip_name: "946 屏東縣 恆春鎮" },
    { user_zip: 947, user_zip_name: "947 屏東縣 滿州鄉" },
    { user_zip: 950, user_zip_name: "950 臺東縣 臺東市" },
    { user_zip: 951, user_zip_name: "951 臺東縣 綠島鄉" },
    { user_zip: 952, user_zip_name: "952 臺東縣 蘭嶼鄉" },
    { user_zip: 953, user_zip_name: "953 臺東縣 延平鄉" },
    { user_zip: 954, user_zip_name: "954 臺東縣 卑南鄉" },
    { user_zip: 955, user_zip_name: "955 臺東縣 鹿野鄉" },
    { user_zip: 956, user_zip_name: "956 臺東縣 關山鎮" },
    { user_zip: 957, user_zip_name: "957 臺東縣 海端鄉" },
    { user_zip: 958, user_zip_name: "958 臺東縣 池上鄉" },
    { user_zip: 959, user_zip_name: "959 臺東縣 東河鄉" },
    { user_zip: 961, user_zip_name: "961 臺東縣 成功鎮" },
    { user_zip: 962, user_zip_name: "962 臺東縣 長濱鄉" },
    { user_zip: 963, user_zip_name: "963 臺東縣 太麻里" },
    { user_zip: 964, user_zip_name: "964 臺東縣 金峰鄉" },
    { user_zip: 965, user_zip_name: "965 臺東縣 大武鄉" },
    { user_zip: 966, user_zip_name: "966 臺東縣 達仁鄉" },
    { user_zip: 970, user_zip_name: "970 花蓮縣 花蓮市" },
    { user_zip: 971, user_zip_name: "971 花蓮縣 新城鄉" },
    { user_zip: 972, user_zip_name: "972 花蓮縣 秀林鄉" },
    { user_zip: 973, user_zip_name: "973 花蓮縣 吉安鄉" },
    { user_zip: 974, user_zip_name: "974 花蓮縣 壽豐鄉" },
    { user_zip: 975, user_zip_name: "975 花蓮縣 鳳林鎮" },
    { user_zip: 976, user_zip_name: "976 花蓮縣 光復鄉" },
    { user_zip: 977, user_zip_name: "977 花蓮縣 豐濱鄉" },
    { user_zip: 978, user_zip_name: "978 花蓮縣 瑞穗鄉" },
    { user_zip: 979, user_zip_name: "979 花蓮縣 萬榮鄉" },
    { user_zip: 981, user_zip_name: "981 花蓮縣 玉里鎮" },
    { user_zip: 982, user_zip_name: "982 花蓮縣 卓溪鄉" },
    { user_zip: 983, user_zip_name: "983 花蓮縣 富里鄉" },
    { user_zip: 890, user_zip_name: "890 金門縣 金沙鎮" },
    { user_zip: 891, user_zip_name: "891 金門縣 金湖鎮" },
    { user_zip: 892, user_zip_name: "892 金門縣 金寧鄉" },
    { user_zip: 893, user_zip_name: "893 金門縣 金城鎮" },
    { user_zip: 894, user_zip_name: "894 金門縣 烈嶼鄉" },
    { user_zip: 896, user_zip_name: "896 金門縣 烏坵鄉" },
    { user_zip: 209, user_zip_name: "209 連江縣 南竿鄉" },
    { user_zip: 210, user_zip_name: "210 連江縣 北竿鄉" },
    { user_zip: 211, user_zip_name: "211 連江縣 莒光鄉" },
    { user_zip: 212, user_zip_name: "212 連江縣 東引鄉" },
    { user_zip: 817, user_zip_name: "817 南海諸島 東沙" },
    { user_zip: 819, user_zip_name: "819 南海諸島 南沙" },
    { user_zip: 290, user_zip_name: "290 釣魚台列嶼" }
    ]
});
