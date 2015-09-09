//地址
Ext.define("gigade.user_zip", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'user_zip', type: "int" },
        { name: 'user_zip_name', type: "string" }]
});
//地址
var user_zip_source = Ext.create('Ext.data.Store', {
    model: 'gigade.user_zip',
    autoLoad: true,
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
	{ user_zip: 320, user_zip_name: "320 桃園縣 中壢市" },
	{ user_zip: 324, user_zip_name: "324 桃園縣 平鎮市" },
	{ user_zip: 325, user_zip_name: "325 桃園縣 龍潭鄉" },
	{ user_zip: 326, user_zip_name: "326 桃園縣 楊梅鎮" },
	{ user_zip: 327, user_zip_name: "327 桃園縣 新屋鄉" },
	{ user_zip: 328, user_zip_name: "328 桃園縣 觀音鄉" },
	{ user_zip: 330, user_zip_name: "330 桃園縣 桃園市" },
	{ user_zip: 333, user_zip_name: "333 桃園縣 龜山鄉" },
	{ user_zip: 334, user_zip_name: "334 桃園縣 八德市" },
	{ user_zip: 335, user_zip_name: "335 桃園縣 大溪鎮" },
	{ user_zip: 336, user_zip_name: "336 桃園縣 復興鄉" },
	{ user_zip: 337, user_zip_name: "337 桃園縣 大園鄉" },
	{ user_zip: 338, user_zip_name: "338 桃園縣 蘆竹鄉" },
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

//編輯客戶備註
editOrderNote = function () {
    var editNoteFrm = Ext.create('Ext.form.Panel', {
        constrain: true,
        url: '/OrderManage/SaveNoteOrder',
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: '',
                value: '<span  style="color:red">※注意：此備註，前台消費者看的到，請小心書寫。</span>'
            },
            {
                xtype: 'displayfield',
                fieldLabel: '付款單號',
                id: 'order_id',
                name: 'order_id',
                width: 300
            },  
            {
                xtype: 'textarea',
                fieldLabel: '消費者備註(最多100字)',
                id: 'note_orders',
                name: 'note_orders',
                maxLength: 100,
                maxLengthText:'最多輸入100個中文字',
                width: 300
            },
            {
                xtype: 'displayfield',
                fieldLabel: '異動人員',
                id: 'user_username',
                name: 'user_username',
                width: 300
            },
            {
                xtype: 'displayfield',
                fieldLabel: '異動時間',
                id: 'note_order_modify_time',
                name: 'note_order_modify_time',
                width: 300
            }
        ],
        buttons: [
            {
                text: '保存',
                formBind: true,
                disabled: true,
                handler: function () {
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        form.submit({
                            params: {
                                order_id: Ext.getCmp('order_id').getValue(),
                                note_order: Ext.getCmp('note_orders').getValue()
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    window.location.reload(true);
                                }
                                else {
                                    Ext.Msg.alert("提示信息", "保存失敗!");
                                }
                            }
                        });
                    }
                }
            }
        ]
    });
    var editNoteWin = Ext.create('Ext.window.Window', {
        title: '新增消費者備註',
        iconCls: 'icon-user-edit',
        width: 450,
        height: 300,
        layout: 'fit',
        items: [editNoteFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        listeners: {
            'beforerender': function () {
                //   editNoteFrm.getForm().loadRecord(store);
                //                alert(store.data.order_id);
                Ext.Ajax.request({
                    url: '/OrderManage/GetData',
                    params: {
                        order_id: document.getElementById('OrderId').value
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                       // alert(result);
                        if (result.success) {
                            Ext.getCmp('order_id').setValue(result.data.order_id);
                            Ext.getCmp('note_orders').setValue(result.data.note_order);
                            Ext.getCmp('user_username').setValue(result.data.manager_name);
                            if (result.data.NoteOrderModifyTime != '1970-01-01 08:00:00') {
                                Ext.getCmp('note_order_modify_time').setValue(result.data.NoteOrderModifyTime);
                            }
                        }
                    }
                });
            }
        },
        closable: false,
        tools: [
        {
            type: 'close',
            handler: function (event, toolEl, panel) {
                Ext.MessageBox.confirm('提示信息', '是否關閉窗口', function (btn) {
                    if (btn == "yes") {
                        editNoteWin.destroy();
                    }
                    else {
                        return false;
                    }
                });
            }
        }
        ]

    });
    editNoteWin.show();
}

//新增管理員備註
editNoteAdmin = function () {
    var eidtAdmin = Ext.create('Ext.form.Panel', {
        constrain: true,
        url: '/OrderManage/SaveNoteAdmin',
        items: [
    {
        xtype: 'displayfield',
        fieldLabel: '付款單號',
        id: 'order_id',
        name: 'order_id',
        width: 300
    },
    {
        xtype: 'textarea',
        fieldLabel: '管理員備註(最多100字)',
        id: 'note_admin',
        name: 'note_admin',
        maxLength: 100,
        maxLengthText:'最多輸入100個中文字',
        width: 300
    }
        ],
        buttons: [
    {
        text: '保存',
        formBind: true,
        disabled: true,
        handler: function () {
            //            alert(location.reload(true));
            var form = this.up('form').getForm();
            if (form.isValid()) {
                form.submit({
                    params: {
                        order_id: Ext.getCmp('order_id').getValue(),
                        note_admin: Ext.getCmp('note_admin').getValue()
                    },
                    success: function (form, action) {
                        var result = Ext.decode(action.response.responseText);
                        if (result.success) {
                            window.location.reload(true);
                        }
                        else {
                            Ext.Msg.alert("提示信息", "保存失敗!");
                        }
                    }
                });
            }
        }
    }
        ]
    });

    var editAdminWin = Ext.create('Ext.window.Window', {
        title: '新增管理員備註',
        iconCls: 'icon-user-edit',
        width: 450,
        height: 300,
        y: 100,
        layout: 'fit',
        items: [eidtAdmin],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        listeners: {
            'beforerender': function () {
                //   editNoteFrm.getForm().loadRecord(store);
                //                alert(store.data.order_id);
                Ext.Ajax.request({
                    url: '/OrderManage/GetData',
                    params: {
                        order_id: document.getElementById('OrderId').value

                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.getCmp('order_id').setValue(result.data.order_id);
                            Ext.getCmp('note_admin').setValue(result.data.note_admin);
                        }
                    }
                });
            }
        },
        closable: false,
        tools: [
            {
                type: 'close',
                handler: function (event, toolEl, panel) {
                    Ext.MessageBox.confirm('提示信息', '是否關閉窗口', function (btn) {
                        if (btn == "yes") {
                            editAdminWin.destroy();
                        }
                        else {
                            return false;
                        }
                    });
                }
            }
        ]

    });
    editAdminWin.show();
}

//新增狀態列表
addStatus = function () {
    var addStatus = Ext.create('Ext.form.Panel', {
        constrain: true,
        url: '/OrderManage/SaveStatus',
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: '付款單號',
                id: 'order_id',
                name: 'order_id',
                width: 300
            },
            {
                xtype: 'displayfield',
                fieldLabel: '姓名',
                id: 'user_username',
                name: 'user_username',
                hidden:true,
                width: 300
            },
            {
                xtype: 'displayfield',
                fieldLabel: '狀態',
                id: 'order_status',
                name: 'order_status',
                hidden:true,
                width: 300
            },
            {
                xtype: 'textarea',
                fieldLabel: '內容(最多100字)',
                id: 'status_description',
                name: 'status_description',
                allowBlank: false,
                maxLength: 100,
                maxLengthText: '最多輸入100個中文字',
                width: 300
            }
        ],
        buttons: [
            {
                text: '保存',
                formBind: true,
                disabled: true,
                handler: function () {
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        form.submit({
                            params: {
                                order_id: Ext.getCmp('order_id').getValue(),
                                user_username: Ext.getCmp('user_username').getValue(),
                                order_status: Ext.getCmp('order_status').getValue(),
                                status_description: Ext.getCmp('status_description').getValue()
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    Ext.Msg.alert("提示信息", "保存成功!");
                                    window.location.reload(true);
                                    editNoteFrm.close();
                                }
                                else {
                                    Ext.Msg.alert("提示信息", "保存失敗!");
                                }
                            }
                        });
                    }
                }
            }
        ]
    });
    var editStatusWin = Ext.create('Ext.window.Window', {
        title: '新增付款單狀態',
        iconCls: 'icon-user-edit',
        width: 450,
        height: 300,
        layout: 'fit',
        items: [addStatus],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        listeners: {
            'beforerender': function () {
                Ext.Ajax.request({
                    url: '/OrderManage/GetData',
                    params: {
                        order_id: document.getElementById('OrderId').value
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            //   alert(result.data.manager_name);
                            Ext.getCmp('order_id').setValue(result.data.order_id);
                            Ext.getCmp('order_status').setValue(result.data.order_status);
                            Ext.getCmp('user_username').setValue(result.data.manager_name);
                        }
                    }
                });
            }
        },
        closable: false,
        tools: [
        {
            type: 'close',
            handler: function (event, toolEl, panel) {
                Ext.MessageBox.confirm('提示信息', '是否關閉窗口', function (btn) {
                    if (btn == "yes") {
                        editStatusWin.destroy();
                    }
                    else {
                        return false;
                    }
                });
            }
        }
        ]

    });
    editStatusWin.show();
}

//編輯用戶信息
userEdit = function (o_id) {
    var userEdit = Ext.create('Ext.form.Panel', {
        constrain: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/Member/SaveUsersList',
        items: [
        {
            xtype: 'displayfield',
            fieldLabel: '會員編號',
            id: 'user_id',
            name: 'user_id',
            width: 300
        },
        {
            xtype: 'displayfield',
            fieldLabel: '電子信箱',
            id: 'user_email',
            name: 'user_email',
            width: 300
        },
        {
            xtype: 'textfield',
            fieldLabel: "姓名",
            id: 'user_name',
            name: 'user_name',
            allowBlank: false,
            width: 300
        },
        {
            xtype: 'textfield',
            fieldLabel: "密碼",
            id: 'user_password', //user_password
            name: 'user_password',
            hidden: true
        },
        {
            xtype: 'textfield',
            fieldLabel: "密碼(※不修改密碼則留空白)",
            id: 'user_passwords', //
            name: 'user_passwords',
            allowBlank: true
        },
        {
            xtype: 'radiogroup',
            id: 'user_gender',
            name: 'user_gender',
            fieldLabel: "性別",
            colName: 'Sex_type',
            width: 350,
            defaults: {
                name: 'SexTax_Type'
            },
            columns: 2,
            vertical: true,
            items: [
                { id: 'sexid1', boxLabel: "先生", inputValue: '1', checked: true },
                { id: 'sexid2', boxLabel: "小姐", inputValue: '0' }]},
                {
                    xtype: "datefield",
                    id: 'my_birthday',
                    name: 'my_birthday',
                    fieldLabel: '生日',
                    allowBlank: false
                },
                {
                    xtype: 'fieldcontainer',
                    layout: 'hbox',
                    width: 420,
                    defaults: {
                        hideLabel: true
                    },
                    items: [
                        {
                            xtype: 'displayfield',
                            value: '地址:',
                            //    labelWidth: 50,
                            margin: '3 40 0  0'
                        },
                        {
                            xtype: 'combo',
                            id: 'user_zip',
                            name: 'user_zip',
                            queryMode: 'local',
                            displayField: 'user_zip_name',
                            valueField: 'user_zip',
                            width: 130,
                            //  readOnly:true,
                            store: user_zip_source,
                            editable: false
                        },
                        {
                            xtype: 'textfield',
                            id: 'user_address',
                            width: 150,
                            name: 'user_address'
                        }
                    ]
                },                  
                {
                    xtype: 'textfield',
                    fieldLabel: "行動電話",
                    id: 'user_mobile',
                    name: 'user_mobile',
                    allowBlank: true,
                    regex: /^[-+]?([0-9]\d*|0)$/,
                    regexText: '格式不正確',
                    maxLength: 20,
                    maxLengthText: '最大長度為20'
                },
                {
                    xtype: 'textfield',
                    fieldLabel: "聯絡電話",
                    id: 'user_phone',
                    name: 'user_phone',
                    allowBlank: true,
                    regex: /^[-+]?([0-9]\d*|0)$/,
                    regexText: '格式不正確',
                    maxLength: 20,
                    maxLengthText: '最大長度為20'
                },
                {
                    xtype: 'displayfield',
                    fieldLabel: "註冊日期",
                    id: 'suser_reg_date', //需要修改
                    name: 'suser_reg_date',
                    width: 300
                },
                {
                    xtype: 'displayfield',
                    fieldLabel: "會員類別",
                    id: 'mytype', //需要修改
                    name: 'mytype',
                    width: 300

                },
                {
                    xtype: 'checkboxfield',
                    id: 'send_sms_ad',
                    name: 'send_sms_ad',
                    fieldLabel: '是否接收簡訊廣告',
                    width: 120
                },
                {
                    xtype: 'checkboxfield',
                    id: 'paper_invoice',
                    name: 'paper_invoice',
                    fieldLabel: '是否長期開立紙本發票',
                    width: 120
                },
                {
                    xtype: 'textfield',
                    fieldLabel: "管理員備註",
                    id: 'adm_note', //需要修改
                    name: 'adm_note',
                    allowBlank: true
                }
        ],
        buttons: [
            {
                text: '保存',
                formBind: true,
                disabled: true,
                handler: function () {
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        form.submit({
                            params: {
                                user_id: Ext.htmlEncode(Ext.getCmp('user_id').getValue()),
                                user_name: Ext.htmlEncode(Ext.getCmp('user_name').getValue()),
                                user_password: Ext.htmlEncode(Ext.getCmp('user_password').getValue()),
                                user_gender: Ext.htmlEncode(Ext.getCmp("user_gender").getValue().SexTax_Type),
                                user_zip: Ext.htmlEncode(Ext.getCmp('user_zip').getValue()),
                                my_birthday: Ext.htmlEncode(Ext.getCmp('my_birthday').getValue()),
                                user_address: Ext.htmlEncode(Ext.getCmp('user_address').getValue()),
                                user_phone: Ext.htmlEncode(Ext.getCmp('user_phone').getValue()),
                                user_mobile: Ext.htmlEncode(Ext.getCmp('user_mobile').getValue()),
                                send_sms_ad: Ext.htmlEncode(Ext.getCmp('send_sms_ad').getValue()),
                                user_password1: Ext.htmlEncode(Ext.getCmp('user_passwords').getValue()),
                                admNote: Ext.htmlEncode(Ext.getCmp('adm_note').getValue()),
                                paper_invoice: Ext.htmlEncode(Ext.getCmp('send_sms_ad').getValue()),
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    window.location.reload(true);
                                }
                                else {
                                    alert("提示信息", "保存失敗!");
                                }
                            }
                        });
                    }
                }
            }
        ]
    });
    var userEditWin = Ext.create('Ext.window.Window', {
        title: '編輯會員信息',
        iconCls: 'icon-user-edit',
        width: 450,
        //  height: 300,
        layout: 'fit',
        items: [userEdit],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        listeners: {
            'beforerender': function () {
                Ext.Ajax.request({
                    url: '/OrderManage/GetData',
                    async: false,
                    params: {
                        order_id: o_id
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {                            
                            Ext.getCmp('user_id').setValue(result.data.user_id);
                        }
                    }
                });
                Ext.Ajax.request({
                    url: '/OrderManage/GetUserInfo',
                    params: {
                        user_id: Ext.getCmp('user_id').getValue()
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {

                            Ext.getCmp('user_email').setValue("<a href='mailto:" + result.data.user_email + "'>" + result.data.user_email + "</a>");
                            Ext.getCmp('user_mobile').setValue(result.data.user_mobile);
                            Ext.getCmp('user_name').setValue(result.data.user_name);
                            Ext.getCmp('user_password').setValue(result.data.user_password);
                            if (result.data.user_gender == 0) {
                                Ext.getCmp('sexid2').setValue(true);
                            }
                            else {
                                Ext.getCmp('sexid1').setValue(true);
                            }
                            Ext.getCmp('user_phone').setValue(result.data.user_phone);
                            Ext.getCmp('suser_reg_date').setValue(result.data.suser_reg_date);
                            Ext.getCmp('mytype').setValue(result.data.mytype);
                            if (result.data.send_sms_ad) {
                                Ext.getCmp('send_sms_ad').setValue(true);
                            }
                            if (result.data.paper_invoice) {
                                Ext.getCmp('paper_invoice').setValue(true);
                            }
                            Ext.getCmp('adm_note').setValue(result.data.adm_note);
                            Ext.getCmp('user_zip').setValue(result.data.user_zip);
                            Ext.getCmp('user_address').setValue(result.data.user_address);
                            Ext.getCmp('my_birthday').setValue(new Date(result.data.birthday));

                        }
                    }
                });
            }
        },
        closable: false,
        tools: [
        {
            type: 'close',
            handler: function (event, toolEl, panel) {
                Ext.MessageBox.confirm('提示信息', '是否關閉窗口', function (btn) {
                    if (btn == "yes") {
                        userEditWin.destroy();
                    }
                    else {
                        return false;
                    }
                });
            }
        }
        ]

    });
    userEditWin.show();
}

//更改收貨人資訊
modifyDeliverData = function () {
    var deliverData = Ext.create('Ext.form.Panel', {
        constrain: true,
        url: '/OrderManage/ModifyDeliveryData',
        items: [
            {
                xtype: 'displayfield',
                name: 'd_order_id',
                id: 'd_order_id',
                //hidden:true,
            },
               {
                   xtype: 'textfield',
                   fieldLabel: '收貨人姓名',
                   id: 'd_user_name',
                   name: 'd_user_name',
                   width: 300
               },
            {
                xtype: 'radiogroup',
                fieldLabel: '性別',
                id: 'd_user_gender',
                width: 300,
                items: [
                    { boxLabel: '先生', id: 'Mr', name: 'dn_user_gender', inputValue: '1', checked: true },
                       { boxLabel: '小姐', id: 'Mrs', name: 'dn_user_gender', inputValue: '0' },
                ]

            },
            {
                xtype: 'textfield',
                fieldLabel: '行動電話',
                id: 'd_user_mobile',
                name: 'd_user_mobile',
                width: 300
            },
               {
                   xtype: 'textfield',
                   fieldLabel: '市內電話',
                   id: 'd_user_phone',
                   name: 'd_user_phone',
                   width: 300
               },
               {
                   xtype: 'fieldcontainer',
                   layout: 'hbox',
                   items: [
                      {
                          xtype: 'combobox',
                          fieldLabel: '地址',
                          queryModel: 'local',
                          id: 'd_zip',
                          store: zipStore,
                          valueField: 'zipcode',
                          editable: false,
                          displayField: 'zipname',
                          queryModel: 'local',
                          allowBlank: false,
                      },
                      {
                          xtype: 'textfield',
                          id: 'd_user_address',
                          name: 'd_user_address',
                          // width: 300
                      },
                   ]
               },

        ],
        buttons: [{
            text: '確定',
            formBind: true,
            disabled: true,
            handler: function () {
                var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                myMask.show();
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            order_id: Ext.getCmp('d_order_id').getValue(),
                            user_name: Ext.getCmp('d_user_name').getValue(),
                            user_gender: Ext.getCmp('d_user_gender').getValue().dn_user_gender,
                            user_mobile: Ext.getCmp('d_user_mobile').getValue(),
                            user_phone: Ext.getCmp('d_user_phone').getValue(),
                            user_zip: Ext.getCmp('d_zip').getValue(),
                            user_address: Ext.getCmp('d_user_address').getValue(),
                        },
                        success: function (form, action) {
                            myMask.hide();
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                if (result.msg == 1) {
                                    Ext.Msg.alert("提示信息", "已出貨，不可修改！");
                                    deliverDataWin.destroy();
                                }
                                else {
                                    Ext.Msg.alert("提示信息", "保存成功！");
                                    deliverDataWin.destroy();
                                    TranToDetial(document.getElementById('OrderId').value);
                                }

                            }
                            else {
                                Ext.Msg.alert("提示信息", "保存失敗！");
                                deliverDataWin.destroy();
                            }

                        },
                        failure: function () {
                            myMask.hide();
                            Ext.Msg.alert("提示信息", "出現異常！");
                            deliverDataWin.destroy();
                        }
                    });
                }
            }
        }]
    });
    var deliverDataWin = Ext.create('Ext.window.Window', {
        title: '變更收件人資訊',
        iconCls: 'icon-user-edit',
        width: 450,
        height: 300,
        layout: 'fit',
        items: [deliverData],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        listeners: {
            'beforerender': function () {
                Ext.Ajax.request({
                    url: '/OrderManage/GetData',
                    params: {
                        order_id: document.getElementById('OrderId').value,
                        isSecret: 'false',
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.getCmp('d_order_id').setValue(result.data.order_id);
                            Ext.getCmp('d_user_name').setValue(result.data.delivery_name);
                            Ext.getCmp('d_user_mobile').setValue(result.data.delivery_mobile);
                            Ext.getCmp('d_user_phone').setValue(result.data.delivery_phone);
                            Ext.getCmp('d_zip').setValue(result.data.delivery_zip);
                            Ext.getCmp('d_user_address').setValue(result.data.delivery_address_str);
                            if (result.data.delivery_gender == 1) {
                                Ext.getCmp('Mr').setValue(true);
                            }
                            else {
                                Ext.getCmp('Mrs').setValue(true);
                            }
                        }
                    }
                });
            }
        },
        tools: [
        {
            type: 'close',
            handler: function (event, toolEl, panel) {
                Ext.MessageBox.confirm('提示信息', '是否關閉窗口', function (btn) {
                    if (btn == "yes") {
                        deliverDataWin.destroy();
                    }
                    else {
                        return false;
                    }
                });
            }
        }
        ]
    });
    deliverDataWin.show();
}

//訂單內容頁面刷新方法
function TranToDetial(orderId) {
    var url = '/OrderManage/OrderDetialList?Order_Id=' + orderId;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var old = panel.down('#detial');
    var copy = panel.down('#detiallist');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'detiallist',
        title: '訂單內容',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
    old.close();
}

