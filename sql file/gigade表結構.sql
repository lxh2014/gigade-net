/*
Navicat MySQL Data Transfer

Source Server         : 192.168.71.1
Source Server Version : 50545
Source Host           : 192.168.71.1:3306
Source Database       : gigade

Target Server Type    : MYSQL
Target Server Version : 50545
File Encoding         : 65001

Date: 2016-01-08 10:55:27
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for activity_entance_rule
-- ----------------------------
DROP TABLE IF EXISTS `activity_entance_rule`;
CREATE TABLE `activity_entance_rule` (
  `entrance_rule_id` int(9) NOT NULL AUTO_INCREMENT,
  `activity_id` int(9) NOT NULL COMMENT '活動id',
  `entrance_type` int(4) NOT NULL COMMENT '入口類型.商品或知識或其他.1表示商品',
  `rule_filter` varchar(500) NOT NULL COMMENT '篩選欄位',
  `values` varchar(500) NOT NULL COMMENT 'filter篩選值,如果有多個值,用逗號分隔.',
  PRIMARY KEY (`entrance_rule_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for activity_gift
-- ----------------------------
DROP TABLE IF EXISTS `activity_gift`;
CREATE TABLE `activity_gift` (
  `gift_id` int(9) NOT NULL AUTO_INCREMENT COMMENT '贈品id',
  `activity_id` int(9) NOT NULL COMMENT '活動id',
  `expiry_date` int(10) NOT NULL COMMENT '過期時間',
  `type` tinyint(4) NOT NULL COMMENT '贈品類型.1:購物金,2:商品',
  `threshold` int(9) NOT NULL COMMENT '門檻,比如答對10題才能兌換',
  `limit_num` int(9) NOT NULL COMMENT '優惠可兌換數量',
  `point` int(9) NOT NULL COMMENT '兌換該優惠所需點數',
  `product_id` int(10) NOT NULL COMMENT '商品id',
  PRIMARY KEY (`gift_id`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for activity_user
-- ----------------------------
DROP TABLE IF EXISTS `activity_user`;
CREATE TABLE `activity_user` (
  `activity_id` int(9) NOT NULL,
  `user_id` int(10) NOT NULL,
  `total_point` float NOT NULL DEFAULT '0' COMMENT '總計得分',
  `current_point` float NOT NULL COMMENT '當前分數',
  `participate` int(9) NOT NULL COMMENT '參與數',
  `passed` int(9) NOT NULL COMMENT '通過數',
  PRIMARY KEY (`activity_id`,`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for activity_user_exchange
-- ----------------------------
DROP TABLE IF EXISTS `activity_user_exchange`;
CREATE TABLE `activity_user_exchange` (
  `exchange_id` int(9) NOT NULL AUTO_INCREMENT,
  `activity_id` int(9) NOT NULL,
  `user_id` int(10) NOT NULL,
  `gift_id` int(9) NOT NULL,
  `exchange_time` int(10) NOT NULL,
  `remarks` varchar(2000) DEFAULT NULL,
  `order_id` int(9) DEFAULT NULL COMMENT '關聯到的訂單id',
  `item_id` int(10) NOT NULL,
  `num` int(9) NOT NULL,
  PRIMARY KEY (`exchange_id`)
) ENGINE=InnoDB AUTO_INCREMENT=140 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for admin_user_token
-- ----------------------------
DROP TABLE IF EXISTS `admin_user_token`;
CREATE TABLE `admin_user_token` (
  `user_email` varchar(100) NOT NULL,
  `access_token` varchar(128) DEFAULT NULL,
  `expired` int(10) unsigned DEFAULT NULL,
  `modDate` int(10) unsigned DEFAULT NULL,
  PRIMARY KEY (`user_email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for ahon_test
-- ----------------------------
DROP TABLE IF EXISTS `ahon_test`;
CREATE TABLE `ahon_test` (
  `id` int(10) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for announce
-- ----------------------------
DROP TABLE IF EXISTS `announce`;
CREATE TABLE `announce` (
  `announce_id` int(9) unsigned NOT NULL DEFAULT '0',
  `title` varchar(50) NOT NULL DEFAULT '',
  `content` text NOT NULL,
  `sort` smallint(4) unsigned NOT NULL DEFAULT '0',
  `status` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `type` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `creator` int(9) unsigned NOT NULL DEFAULT '0',
  `create_time` int(10) unsigned NOT NULL DEFAULT '0',
  `modifier` int(9) unsigned NOT NULL DEFAULT '0',
  `modify_time` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`announce_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for api_access_role
-- ----------------------------
DROP TABLE IF EXISTS `api_access_role`;
CREATE TABLE `api_access_role` (
  `role_id` int(9) NOT NULL AUTO_INCREMENT,
  `role_name` varchar(100) NOT NULL,
  PRIMARY KEY (`role_id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for api_appkey_role
-- ----------------------------
DROP TABLE IF EXISTS `api_appkey_role`;
CREATE TABLE `api_appkey_role` (
  `role_id` int(9) NOT NULL,
  `appkey` varchar(128) NOT NULL,
  `log` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`appkey`,`role_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for apicalllog
-- ----------------------------
DROP TABLE IF EXISTS `apicalllog`;
CREATE TABLE `apicalllog` (
  `ipfrom` varchar(100) NOT NULL,
  `apicontroller` varchar(50) NOT NULL,
  `apiaction` varchar(50) NOT NULL,
  `calldate` int(10) NOT NULL,
  `user_email` varchar(50) DEFAULT NULL,
  `appkey` varchar(128) NOT NULL,
  `body` longtext,
  `device` varchar(200) DEFAULT NULL,
  `model` varchar(200) DEFAULT NULL,
  `os` varchar(100) DEFAULT NULL,
  `osversion` varchar(100) DEFAULT NULL,
  `context_id` varchar(128) NOT NULL,
  `result` varchar(100) DEFAULT NULL,
  `time_cast` int(10) DEFAULT NULL COMMENT 'ms',
  PRIMARY KEY (`context_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for apierror_log
-- ----------------------------
DROP TABLE IF EXISTS `apierror_log`;
CREATE TABLE `apierror_log` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `Date` datetime DEFAULT NULL,
  `Thread` varchar(1000) DEFAULT NULL,
  `Levels` varchar(1000) DEFAULT NULL,
  `Logger` varchar(200) DEFAULT NULL,
  `Message` longtext,
  `Exception` longtext,
  `context_id` varchar(128) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3051 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for apilog
-- ----------------------------
DROP TABLE IF EXISTS `apilog`;
CREATE TABLE `apilog` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `ipfrom` varchar(100) NOT NULL,
  `apicontroller` varchar(50) NOT NULL,
  `apiaction` varchar(50) NOT NULL,
  `lastcalldate` int(10) NOT NULL,
  `user_email` varchar(50) DEFAULT NULL,
  `context_id` varchar(128) NOT NULL,
  `appkey` varchar(128) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5970 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for app_activity
-- ----------------------------
DROP TABLE IF EXISTS `app_activity`;
CREATE TABLE `app_activity` (
  `site_id` int(4) NOT NULL DEFAULT '17' COMMENT '站台',
  `activity_id` int(9) NOT NULL AUTO_INCREMENT COMMENT '答題類活動編號',
  `start_time` int(10) NOT NULL COMMENT '起始時間',
  `end_time` int(10) NOT NULL COMMENT '結束時間',
  `name` varchar(500) NOT NULL COMMENT '活動名稱',
  `content` varchar(4000) NOT NULL COMMENT '活動詳情',
  `link_url` varchar(2000) NOT NULL COMMENT '鏈接地址',
  `exchange_limit` int(9) NOT NULL COMMENT '可兌換獎品次數',
  `support_version_code` int(10) NOT NULL COMMENT 'app支持活動需要的最低版本',
  `activity_type` int(9) NOT NULL DEFAULT '0',
  PRIMARY KEY (`activity_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for app_banner_linkinfo
-- ----------------------------
DROP TABLE IF EXISTS `app_banner_linkinfo`;
CREATE TABLE `app_banner_linkinfo` (
  `id` int(9) NOT NULL AUTO_INCREMENT,
  `element_id` int(9) NOT NULL,
  `type` tinyint(4) NOT NULL COMMENT '1表示商品,2表示品牌',
  `data` int(9) NOT NULL COMMENT '商品id或品牌id',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for app_localization
-- ----------------------------
DROP TABLE IF EXISTS `app_localization`;
CREATE TABLE `app_localization` (
  `culture` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `value` varchar(4000) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`name`,`culture`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for app_notify
-- ----------------------------
DROP TABLE IF EXISTS `app_notify`;
CREATE TABLE `app_notify` (
  `id` int(9) NOT NULL AUTO_INCREMENT,
  `title` varchar(500) NOT NULL,
  `badge` int(9) NOT NULL,
  `alert` varchar(1000) NOT NULL,
  `url` varchar(1000) NOT NULL,
  `to` longtext NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for app_notify_pool
-- ----------------------------
DROP TABLE IF EXISTS `app_notify_pool`;
CREATE TABLE `app_notify_pool` (
  `id` int(9) NOT NULL AUTO_INCREMENT COMMENT '自增id,key',
  `title` varchar(500) NOT NULL COMMENT '通知標題',
  `alert` varchar(1000) NOT NULL COMMENT '通知正文',
  `url` varchar(1000) NOT NULL COMMENT '通知導向的鏈接,比如某商品詳情頁',
  `to` longtext NOT NULL COMMENT '要推送到的user_id(以逗號分隔),空字串則推送所有',
  `valid_start` int(10) NOT NULL COMMENT '有效期-起始-utc時間戳',
  `valid_end` int(10) NOT NULL COMMENT '有效期-截止-utc時間戳',
  `notified` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否已推送(1:已推送,0:未推送),默認:0',
  `notify_time` int(10) NOT NULL COMMENT '推送時間-utc時間戳',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for app_notify_result
-- ----------------------------
DROP TABLE IF EXISTS `app_notify_result`;
CREATE TABLE `app_notify_result` (
  `id` int(9) NOT NULL AUTO_INCREMENT,
  `device_id` varchar(1000) NOT NULL,
  `user_id` int(9) NOT NULL,
  `notify_id` int(9) NOT NULL,
  `result` varchar(4000) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4022 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for app_search_record
-- ----------------------------
DROP TABLE IF EXISTS `app_search_record`;
CREATE TABLE `app_search_record` (
  `id` int(9) NOT NULL AUTO_INCREMENT,
  `keyword` varchar(500) NOT NULL,
  `kdate` int(10) NOT NULL DEFAULT '0',
  `user_id` int(10) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2953 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for app_signalr
-- ----------------------------
DROP TABLE IF EXISTS `app_signalr`;
CREATE TABLE `app_signalr` (
  `user_id` int(9) NOT NULL,
  `connection_id` varchar(128) DEFAULT NULL,
  `group` varchar(200) DEFAULT NULL,
  `online` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for appcategory
-- ----------------------------
DROP TABLE IF EXISTS `appcategory`;
CREATE TABLE `appcategory` (
  `category_id` int(10) NOT NULL AUTO_INCREMENT,
  `category` varchar(20) DEFAULT NULL COMMENT '分類（食品/用品）',
  `brand_id` int(10) DEFAULT NULL COMMENT '品牌id',
  `brand_name` varchar(50) DEFAULT NULL COMMENT '品牌名稱',
  `category1` varchar(20) DEFAULT NULL COMMENT '館別（一）',
  `category2` varchar(20) DEFAULT NULL COMMENT '分類（二）',
  `category3` varchar(20) DEFAULT NULL COMMENT '分類（三）',
  `product_id` int(10) DEFAULT NULL COMMENT '產品id',
  `Property` varchar(20) DEFAULT NULL COMMENT '屬性',
  PRIMARY KEY (`category_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2193 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for apperror_log
-- ----------------------------
DROP TABLE IF EXISTS `apperror_log`;
CREATE TABLE `apperror_log` (
  `id` int(8) NOT NULL AUTO_INCREMENT,
  `errordate` int(10) NOT NULL,
  `kdate` int(10) NOT NULL,
  `message` varchar(4000) DEFAULT NULL,
  `exception` longtext,
  `error_source` varchar(1000) NOT NULL,
  `user_email` varchar(500) DEFAULT NULL,
  `upload_appkey` varchar(128) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1176 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for applogin
-- ----------------------------
DROP TABLE IF EXISTS `applogin`;
CREATE TABLE `applogin` (
  `user_email` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `appkey` varchar(128) COLLATE utf8mb4_unicode_ci NOT NULL,
  `logindate` int(10) NOT NULL,
  `location` varchar(2000) COLLATE utf8mb4_unicode_ci NOT NULL,
  `identity_code` varchar(500) COLLATE utf8mb4_unicode_ci NOT NULL,
  `device` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL,
  `model` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL,
  `os` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `osversion` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `id` int(10) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5508 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for appmessage
-- ----------------------------
DROP TABLE IF EXISTS `appmessage`;
CREATE TABLE `appmessage` (
  `message_id` int(10) NOT NULL AUTO_INCREMENT,
  `type` int(4) DEFAULT NULL,
  `title` varchar(20) NOT NULL,
  `content` varchar(255) NOT NULL,
  `messagedate` int(10) NOT NULL,
  `group` varchar(10) DEFAULT NULL,
  `linkurl` varchar(255) DEFAULT NULL,
  `display_type` int(4) DEFAULT NULL,
  `msg_end` int(10) NOT NULL COMMENT '消息有效結束時間',
  `msg_start` int(10) NOT NULL COMMENT '消息有效起始時間',
  `fit_os` varchar(20) NOT NULL COMMENT '適用平台(Android or IOS )',
  `appellation` varchar(100) NOT NULL,
  `need_login` bit(1) DEFAULT NULL,
  PRIMARY KEY (`message_id`)
) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for appsecurity
-- ----------------------------
DROP TABLE IF EXISTS `appsecurity`;
CREATE TABLE `appsecurity` (
  `appkey` varchar(128) COLLATE utf8mb4_unicode_ci NOT NULL,
  `appsecret` varchar(128) COLLATE utf8mb4_unicode_ci NOT NULL,
  `createdate` int(10) NOT NULL,
  `developer` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`appkey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for appversions
-- ----------------------------
DROP TABLE IF EXISTS `appversions`;
CREATE TABLE `appversions` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `versions_id` int(10) NOT NULL DEFAULT '0' COMMENT '版本id',
  `versions_code` int(10) NOT NULL DEFAULT '0' COMMENT '版本code',
  `versions_name` varchar(200) NOT NULL COMMENT '版本名稱',
  `versions_desc` varchar(255) DEFAULT NULL COMMENT '版本描述',
  `drive` int(8) NOT NULL DEFAULT '0' COMMENT '裝置類型 0:IOS 1:Android',
  `release_type` tinyint(8) NOT NULL DEFAULT '0' COMMENT '是否發佈 0未發佈 1發佈',
  `release_date` int(10) NOT NULL DEFAULT '0' COMMENT '發佈時間',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for area_packet
-- ----------------------------
DROP TABLE IF EXISTS `area_packet`;
CREATE TABLE `area_packet` (
  `packet_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '信息包id',
  `packet_name` varchar(50) COLLATE utf8_unicode_ci NOT NULL COMMENT '信息包的名稱',
  `show_number` int(4) NOT NULL COMMENT '包內元素的數量：必須是4的倍數',
  `packet_sort` int(9) DEFAULT NULL COMMENT '信息包的排序',
  `element_type` int(9) DEFAULT NULL,
  `packet_status` int(4) DEFAULT NULL,
  `packet_desc` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL COMMENT '包描述',
  `packet_createdate` datetime NOT NULL COMMENT '創建時間',
  `packet_updatedate` datetime NOT NULL COMMENT '修改時間',
  `create_userid` int(9) NOT NULL COMMENT '創建人',
  `update_userid` int(9) DEFAULT NULL COMMENT '修改人',
  PRIMARY KEY (`packet_id`),
  KEY `packet_id` (`packet_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=142 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- ----------------------------
-- Table structure for arrival_notice
-- ----------------------------
DROP TABLE IF EXISTS `arrival_notice`;
CREATE TABLE `arrival_notice` (
  `id` int(9) unsigned NOT NULL AUTO_INCREMENT,
  `user_id` int(9) unsigned NOT NULL DEFAULT '0',
  `item_id` int(9) unsigned NOT NULL DEFAULT '0',
  `product_id` int(9) unsigned NOT NULL DEFAULT '0',
  `status` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `create_time` int(10) unsigned NOT NULL DEFAULT '0',
  `source_type` int(4) NOT NULL DEFAULT '1' COMMENT '訊息來源 1來自前臺 2後臺操作 默認1',
  `muser_id` int(9) NOT NULL DEFAULT '0' COMMENT '管理員編號 默認0',
  `send_notice_time` int(10) NOT NULL DEFAULT '0' COMMENT '發送補貨通知時間',
  `coming_time` int(10) DEFAULT NULL COMMENT '預計捕獲日期',
  PRIMARY KEY (`id`),
  KEY `item_ix` (`item_id`),
  KEY `all_ix` (`user_id`,`item_id`)
) ENGINE=InnoDB AUTO_INCREMENT=5453 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for aseld
-- ----------------------------
DROP TABLE IF EXISTS `aseld`;
CREATE TABLE `aseld` (
  `seld_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水号',
  `dc_id` smallint(6) NOT NULL DEFAULT '1' COMMENT '物流中心编号',
  `whse_id` smallint(6) NOT NULL DEFAULT '1' COMMENT '仓库编号',
  `ord_id` int(11) DEFAULT NULL COMMENT '客户订单说明inbount进来WMS后，WMS产生的订单号码',
  `sgmt_id` smallint(6) DEFAULT '1' COMMENT '段號，WMS產生作為批次選單，執行庫存遞減作業用    1',
  `ordd_id` int(6) DEFAULT NULL COMMENT '訂單的每個line的序號',
  `cust_id` varchar(8) NOT NULL COMMENT '客戶編號，就是我們的會員編號',
  `item_id` int(9) unsigned NOT NULL COMMENT '貨號，商品編號，來自IProd',
  `prdd_id` smallint(6) NOT NULL DEFAULT '1' COMMENT '商品序號，來自IPrdd',
  `assg_id` varchar(255) DEFAULT NULL COMMENT 'job的編號，理貨工作單，作為員工領用工作的記錄',
  `sety_id` varchar(2) DEFAULT NULL COMMENT '庫存遞減完畢產生的理貨工作，我們絕大部分的訂單，都是零星撿貨，是為HS;如果單一商品編號的訂單量大於一個整版的數量，則產生PS的撿貨工作，PS必須分派給駕駛堆高機的員工，不必人工分貨，才符合效率',
  `unit_ship_cse` smallint(6) DEFAULT NULL COMMENT 'OP商品內裝裡頭的PCS數量',
  `prod_cub` decimal(9,0) DEFAULT NULL COMMENT '該品號的數量X材積，這裡的數量是最終出貨的數量。我們的規範要求是百分百滿足出貨',
  `prod_wgt` decimal(9,0) DEFAULT NULL COMMENT '該品號的數量X，重量',
  `prod_qty` int(11) DEFAULT NULL COMMENT '預期撿貨量（在WMS實際上已經扣掉可用的庫存）《=客戶訂購量》',
  `sel_loc` varchar(8) DEFAULT NULL COMMENT '撿貨的料位',
  `ckpt_id` int(11) DEFAULT NULL COMMENT '庫存遞減號（每批次撿貨工作就產生一個，一個庫存遞減號可能產生一個以上的ASSG_id）',
  `curr_pal_no` bigint(11) DEFAULT NULL COMMENT 'WMS出貨板號，一張訂單可以有多個板號',
  `cse_lbl_lmt` smallint(6) DEFAULT NULL COMMENT '等同Iprod說明，不乾膠撿貨標籤，模塊使用的設定值',
  `wust_id` varchar(3) DEFAULT 'AVL' COMMENT '記錄料位是否已經撿貨了',
  `lic_plt_id` decimal(10,0) DEFAULT NULL COMMENT 'WMS指的是收貨時伴隨生成的棧板編號',
  `description` varchar(100) DEFAULT NULL COMMENT '商品名稱',
  `prod_sz` varchar(100) DEFAULT NULL COMMENT '商品規格',
  `hzd_ind` varchar(10) DEFAULT NULL COMMENT '來自Iprod，易損等級',
  `cust_name` varchar(30) DEFAULT NULL COMMENT '客戶姓名',
  `order_type_id` varchar(4) DEFAULT NULL COMMENT '訂單類型',
  `stg_dcpt_id` varchar(3) DEFAULT NULL COMMENT '控制本批出貨貨品在倉庫中的定位點',
  `stg_dcpd_id` varchar(3) DEFAULT NULL COMMENT '控制本批出貨貨品在倉庫中的定位點具體命名',
  `invc_id` bigint(11) DEFAULT NULL COMMENT 'ERP來源的客戶訂單單號',
  `route_id` varchar(4) DEFAULT NULL COMMENT '運輸路線，作為接單出車的控制批號，是根據客戶地址所有不同，一個ckpt_id底下有多個route_id',
  `stop_id` smallint(6) DEFAULT NULL COMMENT '運輸車趟路線下的停靠站順序',
  `batch_id` smallint(6) DEFAULT NULL COMMENT '批次編號',
  `batch_seq` smallint(6) DEFAULT NULL COMMENT '批次下的流水號',
  `start_dtim` datetime DEFAULT NULL COMMENT '料位的開始時間，用來計算員工的理貨效率',
  `complete_dtim` datetime DEFAULT NULL COMMENT '料位的結束時間',
  `change_dtim` datetime DEFAULT NULL COMMENT '料位的最後一次異動時間',
  `change_user` int(8) DEFAULT '0' COMMENT '最後異動者',
  `create_dtim` datetime DEFAULT NULL,
  `create_user` int(8) DEFAULT '0',
  `ord_msg_id` int(11) DEFAULT NULL COMMENT '備用文字，顯示客戶留言等的信息',
  `door_dcpd_id` varchar(3) DEFAULT NULL COMMENT '運送模塊，對應出貨碼頭名稱ID ',
  `door_dcpt_id` varchar(3) DEFAULT NULL COMMENT '運送模塊的對應出貨碼頭ID',
  `catch_wgt_cntl` varchar(1) DEFAULT NULL COMMENT '是否計算商品重量',
  `lot_no` varchar(14) DEFAULT NULL COMMENT '履歷管理批號',
  `commodity_type` varchar(3) DEFAULT NULL COMMENT '商品分類',
  `sect_id` varchar(3) DEFAULT NULL COMMENT '料位之外，WMS還有區域的概念',
  `ucn` varchar(6) DEFAULT NULL COMMENT '外箱條碼',
  `hzd_class` varchar(3) DEFAULT NULL,
  `pkde_id` varchar(1) DEFAULT NULL COMMENT 'RF如果功能如果起不來，要啟動標籤還是報表，這是WMS系統控制程序事先要設定好的，這裡只是繼承設定值',
  `ord_rqst_del_dt` datetime DEFAULT NULL COMMENT '客戶指定的到貨日',
  `ord_rqst_del_tim` datetime DEFAULT NULL COMMENT '客戶指定到貨時間',
  `spmd_id` varchar(1) DEFAULT NULL COMMENT '區分運輸條件',
  `flow_dcpt_id` varchar(3) DEFAULT NULL COMMENT '出貨碼頭設定點',
  `flow_dcpd_id` varchar(3) DEFAULT NULL COMMENT '出貨碼頭設定點',
  `flow_assg_flg` varchar(1) DEFAULT NULL COMMENT '區分寄倉買斷/調度的job',
  `sel_seq_loc` varchar(8) DEFAULT NULL COMMENT '撿貨路順序',
  `out_qty` int(11) DEFAULT NULL COMMENT '缺貨數量',
  `eqpt_class_id` varchar(5) DEFAULT NULL COMMENT '使用什麼設備完成撿貨工作',
  `sel_x_coord` int(11) DEFAULT NULL COMMENT '主料位X坐標',
  `sel_y_coord` int(11) DEFAULT NULL COMMENT '主料位y坐標',
  `sel_z_coord` int(11) DEFAULT NULL COMMENT '主料位z坐標',
  `upc_id` varchar(50) DEFAULT NULL COMMENT '條碼',
  `ft_id` int(11) DEFAULT NULL COMMENT 'FlowThru模塊的資料表的P_key',
  `ftd_id` int(11) DEFAULT NULL COMMENT 'FlowThru模塊的資料表的P_key',
  `act_pick_qty` int(11) DEFAULT NULL COMMENT '實際撿貨量PCS',
  `ord_qty` int(11) DEFAULT NULL COMMENT '訂單訂貨量PCS',
  `family_group` varchar(10) DEFAULT NULL COMMENT '應用在記錄商品的大小分類',
  `scaned` int(4) DEFAULT '0' COMMENT '是否已經撿過 0為沒有撿過 1為查看過',
  `deliver_id` int(11) DEFAULT '0' COMMENT '出貨單編號',
  `deliver_code` varchar(50) DEFAULT '' COMMENT '出貨單號',
  PRIMARY KEY (`seld_id`)
) ENGINE=InnoDB AUTO_INCREMENT=46400 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for aseld_master
-- ----------------------------
DROP TABLE IF EXISTS `aseld_master`;
CREATE TABLE `aseld_master` (
  `row_id` int(9) NOT NULL AUTO_INCREMENT,
  `assg_id` varchar(50) DEFAULT NULL,
  `complete_time` datetime DEFAULT NULL,
  `start_time` datetime DEFAULT NULL,
  `create_time` datetime DEFAULT NULL,
  `create_user` int(11) DEFAULT NULL,
  `update_time` datetime DEFAULT NULL,
  `update_user` int(11) DEFAULT NULL,
  `locked` int(4) DEFAULT NULL COMMENT '鎖工作項 0是開鎖  1是鎖住',
  PRIMARY KEY (`row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2668 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for banner_category
-- ----------------------------
DROP TABLE IF EXISTS `banner_category`;
CREATE TABLE `banner_category` (
  `category_id` int(11) NOT NULL AUTO_INCREMENT,
  `category_father_id` int(11) NOT NULL,
  `category_sort` int(11) NOT NULL,
  `category_name` varchar(255) NOT NULL,
  `content_type` varchar(50) NOT NULL,
  `content_id` int(11) NOT NULL,
  `description` text NOT NULL,
  `activity` int(11) NOT NULL,
  `created_on` datetime NOT NULL,
  `updated_on` datetime NOT NULL,
  PRIMARY KEY (`category_id`)
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for banner_content
-- ----------------------------
DROP TABLE IF EXISTS `banner_content`;
CREATE TABLE `banner_content` (
  `banner_content_id` int(9) unsigned NOT NULL DEFAULT '0',
  `banner_site_id` int(9) unsigned NOT NULL DEFAULT '0',
  `banner_title` varchar(255) NOT NULL DEFAULT '',
  `banner_link_url` varchar(255) NOT NULL DEFAULT '',
  `banner_link_mode` int(2) NOT NULL DEFAULT '1',
  `banner_sort` smallint(4) unsigned NOT NULL DEFAULT '0',
  `banner_status` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `banner_image` varchar(40) NOT NULL DEFAULT '',
  `banner_start` int(10) unsigned NOT NULL DEFAULT '0',
  `banner_end` int(10) unsigned NOT NULL DEFAULT '0',
  `banner_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `banner_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `banner_ipfrom` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`banner_content_id`),
  KEY `ix_banner_content_sid` (`banner_site_id`),
  KEY `ix_banner_content_status` (`banner_status`),
  KEY `ix_banner_content_image` (`banner_image`),
  KEY `ix_banner_content_start` (`banner_start`),
  KEY `ix_banner_content_end` (`banner_end`),
  CONSTRAINT `fk_banner_content_sid` FOREIGN KEY (`banner_site_id`) REFERENCES `banner_site` (`banner_site_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for banner_news_content
-- ----------------------------
DROP TABLE IF EXISTS `banner_news_content`;
CREATE TABLE `banner_news_content` (
  `news_id` int(9) unsigned NOT NULL DEFAULT '0',
  `news_site_id` int(9) unsigned NOT NULL DEFAULT '0',
  `news_title` varchar(255) NOT NULL DEFAULT '',
  `news_content` mediumtext NOT NULL,
  `news_link_url` varchar(255) NOT NULL DEFAULT '',
  `news_link_mode` int(2) NOT NULL DEFAULT '1',
  `news_sort` smallint(4) unsigned NOT NULL DEFAULT '0',
  `news_status` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `news_start` int(10) unsigned NOT NULL DEFAULT '0',
  `news_end` int(10) unsigned NOT NULL DEFAULT '0',
  `news_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `news_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `news_ipfrom` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`news_id`),
  KEY `ix_banner_news_content_sid` (`news_site_id`),
  KEY `ix_banner_news_content_ss` (`news_status`),
  KEY `ix_banner_news_content_st` (`news_start`),
  KEY `ix_banner_news_content_ed` (`news_end`),
  CONSTRAINT `fk_banner_news_content_sid` FOREIGN KEY (`news_site_id`) REFERENCES `banner_news_site` (`news_site_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for banner_news_site
-- ----------------------------
DROP TABLE IF EXISTS `banner_news_site`;
CREATE TABLE `banner_news_site` (
  `news_site_id` int(9) unsigned NOT NULL DEFAULT '0',
  `news_site_sort` smallint(4) unsigned NOT NULL DEFAULT '0',
  `news_site_status` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `news_site_mode` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `news_site_name` varchar(255) NOT NULL,
  `news_site_description` varchar(255) DEFAULT NULL,
  `news_site_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `news_site_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `news_site_ipfrom` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`news_site_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for banner_site
-- ----------------------------
DROP TABLE IF EXISTS `banner_site`;
CREATE TABLE `banner_site` (
  `banner_site_id` int(9) unsigned NOT NULL DEFAULT '0',
  `banner_site_sort` smallint(4) unsigned NOT NULL DEFAULT '0',
  `banner_site_status` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `banner_site_name` varchar(255) NOT NULL,
  `banner_site_description` varchar(255) DEFAULT NULL,
  `banner_site_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `banner_site_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `banner_site_ipfrom` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`banner_site_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for boiler_relation
-- ----------------------------
DROP TABLE IF EXISTS `boiler_relation`;
CREATE TABLE `boiler_relation` (
  `boiler_id` int(11) NOT NULL AUTO_INCREMENT,
  `boiler_type` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '對應安康內鍋型號',
  `boiler_describe` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '對應安康內鍋型號詳細信息',
  `inner_boiler_number` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '內鍋型號',
  `out_boiler_number` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '外鍋型號(依款式&字母順序排列)',
  `add_user` int(4) DEFAULT NULL COMMENT '添加人/修改人',
  `add_time` datetime DEFAULT NULL COMMENT '添加時間/修改時間',
  `boiler_remark` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '備註',
  PRIMARY KEY (`boiler_id`)
) ENGINE=InnoDB AUTO_INCREMENT=31640 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for bonus_master
-- ----------------------------
DROP TABLE IF EXISTS `bonus_master`;
CREATE TABLE `bonus_master` (
  `master_id` int(9) unsigned NOT NULL,
  `user_id` int(9) unsigned NOT NULL,
  `type_id` int(9) unsigned NOT NULL,
  `master_total` int(9) unsigned NOT NULL DEFAULT '0',
  `master_balance` int(9) NOT NULL DEFAULT '0',
  `master_note` varchar(255) NOT NULL DEFAULT '',
  `master_writer` varchar(255) NOT NULL DEFAULT '',
  `master_start` int(10) unsigned NOT NULL DEFAULT '0',
  `master_end` int(10) unsigned NOT NULL DEFAULT '0',
  `master_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `master_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `master_ipfrom` varchar(40) NOT NULL DEFAULT '',
  `apprise` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `bonus_type` tinyint(2) unsigned NOT NULL DEFAULT '1',
  PRIMARY KEY (`master_id`),
  KEY `ix_bonus_master_uid` (`user_id`),
  KEY `ix_bonus_master_tid` (`type_id`),
  KEY `ix_bonus_master_st` (`master_start`),
  KEY `ix_bonus_master_ed` (`master_end`),
  KEY `ix_bonus_master_ce` (`master_createdate`),
  CONSTRAINT `fk_bonus_master_tid` FOREIGN KEY (`type_id`) REFERENCES `bonus_type` (`type_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_bonus_master_uid` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for bonus_record
-- ----------------------------
DROP TABLE IF EXISTS `bonus_record`;
CREATE TABLE `bonus_record` (
  `record_id` int(9) unsigned NOT NULL,
  `master_id` int(9) unsigned NOT NULL,
  `type_id` int(9) unsigned NOT NULL,
  `order_id` int(9) unsigned NOT NULL,
  `record_use` int(9) unsigned NOT NULL DEFAULT '0',
  `record_note` varchar(255) NOT NULL DEFAULT '',
  `record_writer` varchar(255) NOT NULL DEFAULT '',
  `record_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `record_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `record_ipfrom` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`record_id`),
  KEY `ix_bonus_record_mid` (`master_id`),
  KEY `ix_bonus_record_tid` (`type_id`),
  KEY `ix_bonus_record_oid` (`order_id`),
  KEY `ix_bonus_record_cdate` (`record_createdate`),
  CONSTRAINT `fk_bonus_record_mid` FOREIGN KEY (`master_id`) REFERENCES `bonus_master` (`master_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_bonus_record_oid` FOREIGN KEY (`order_id`) REFERENCES `order_master` (`order_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_bonus_record_tid` FOREIGN KEY (`type_id`) REFERENCES `bonus_type` (`type_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for bonus_type
-- ----------------------------
DROP TABLE IF EXISTS `bonus_type`;
CREATE TABLE `bonus_type` (
  `type_id` int(9) unsigned NOT NULL,
  `type_description` varchar(255) NOT NULL DEFAULT '',
  `type_user_link` varchar(255) NOT NULL DEFAULT '',
  `type_admin_link` varchar(255) NOT NULL DEFAULT '',
  `type_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `type_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `type_ipfrom` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`type_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for brand_collect
-- ----------------------------
DROP TABLE IF EXISTS `brand_collect`;
CREATE TABLE `brand_collect` (
  `brand_collect_id` int(9) unsigned NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `brand_id` int(9) NOT NULL COMMENT '品牌ID',
  `brand_collect_time` datetime NOT NULL COMMENT '收藏時間（yyyy-mm-dd hh:mm:ss）',
  `user_id` int(9) NOT NULL COMMENT '會員ID',
  `brand_collect_status` tinyint(3) NOT NULL DEFAULT '1' COMMENT '狀態（1:顯示 2:刪除不顯示）',
  PRIMARY KEY (`brand_collect_id`)
) ENGINE=InnoDB AUTO_INCREMENT=478 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for brand_logo_sort
-- ----------------------------
DROP TABLE IF EXISTS `brand_logo_sort`;
CREATE TABLE `brand_logo_sort` (
  `blo_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `category_id` int(11) unsigned NOT NULL COMMENT '類別編號 (品牌館分類)',
  `brand_id` int(11) unsigned NOT NULL COMMENT '品牌編號',
  `blo_sort` int(11) NOT NULL DEFAULT '0' COMMENT '品牌圖排序 (同一類別下, 由小排到大)',
  `blo_kuser` smallint(4) unsigned NOT NULL COMMENT '建立人員',
  `blo_kdate` datetime NOT NULL COMMENT '建立時間',
  `blo_muser` smallint(4) unsigned NOT NULL COMMENT '異動人員',
  `blo_mdate` datetime NOT NULL COMMENT '異動時間',
  PRIMARY KEY (`blo_id`),
  KEY `category_id` (`category_id`),
  KEY `brand_id` (`brand_id`),
  CONSTRAINT `fk_brand_id` FOREIGN KEY (`brand_id`) REFERENCES `vendor_brand` (`brand_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_cate_id` FOREIGN KEY (`category_id`) REFERENCES `product_category` (`category_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=64 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for browse_data
-- ----------------------------
DROP TABLE IF EXISTS `browse_data`;
CREATE TABLE `browse_data` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `user_id` int(10) unsigned NOT NULL,
  `product_id` int(10) unsigned NOT NULL,
  `type` int(2) unsigned NOT NULL COMMENT '1:瀏覽 2:刪除',
  `count` int(5) unsigned NOT NULL DEFAULT '0' COMMENT '操作次數',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1719931 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for btob_emp
-- ----------------------------
DROP TABLE IF EXISTS `btob_emp`;
CREATE TABLE `btob_emp` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `emp_id` varchar(25) NOT NULL DEFAULT '''''' COMMENT '員工編號',
  `group_id` int(10) unsigned NOT NULL,
  `status` tinyint(1) unsigned DEFAULT '1' COMMENT '狀態',
  `create_date` int(10) unsigned NOT NULL DEFAULT '0',
  `update_date` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `un_id` (`emp_id`),
  KEY `inx_emp` (`emp_id`),
  KEY `inx_status` (`status`),
  KEY `inx_emp_status` (`emp_id`,`status`)
) ENGINE=InnoDB AUTO_INCREMENT=619 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for calendar
-- ----------------------------
DROP TABLE IF EXISTS `calendar`;
CREATE TABLE `calendar` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `date` date NOT NULL,
  `working_day` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `date` (`date`)
) ENGINE=InnoDB AUTO_INCREMENT=2954 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for category_banner
-- ----------------------------
DROP TABLE IF EXISTS `category_banner`;
CREATE TABLE `category_banner` (
  `id` int(9) unsigned NOT NULL AUTO_INCREMENT,
  `banner_content_id` int(9) NOT NULL DEFAULT '0',
  `category_id` int(9) NOT NULL DEFAULT '0',
  `size` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `banner_content_id_index` (`banner_content_id`),
  KEY `category_id_index` (`category_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2078 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for category_news
-- ----------------------------
DROP TABLE IF EXISTS `category_news`;
CREATE TABLE `category_news` (
  `id` int(9) unsigned NOT NULL AUTO_INCREMENT,
  `news_id` int(9) unsigned DEFAULT '0',
  `category_id` int(9) unsigned DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `category_index` (`category_id`),
  KEY `news_id_index` (`news_id`)
) ENGINE=InnoDB AUTO_INCREMENT=92 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for cbjob_detail
-- ----------------------------
DROP TABLE IF EXISTS `cbjob_detail`;
CREATE TABLE `cbjob_detail` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT,
  `cb_jobid` varchar(50) NOT NULL COMMENT 'job編號 cb_job_master的cb_jobid',
  `cb_newid` int(11) NOT NULL COMMENT '新生成的編號',
  `iinvd_id` int(11) NOT NULL COMMENT 'iinvd表裏的rowid',
  `create_datetime` datetime DEFAULT NULL,
  `create_user` int(10) DEFAULT NULL,
  `change_datetime` datetime DEFAULT NULL,
  `change_user` int(10) DEFAULT NULL,
  `status` int(10) DEFAULT '1' COMMENT ' 默認1 狀態0 刪除 1啟用',
  PRIMARY KEY (`row_id`),
  KEY `cb_jobid` (`cb_jobid`),
  CONSTRAINT `cbjob_detail_ibfk_1` FOREIGN KEY (`cb_jobid`) REFERENCES `cbjob_master` (`cbjob_id`)
) ENGINE=InnoDB AUTO_INCREMENT=542775 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for cbjob_master
-- ----------------------------
DROP TABLE IF EXISTS `cbjob_master`;
CREATE TABLE `cbjob_master` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT,
  `cbjob_id` varchar(50) NOT NULL COMMENT '盤點job編號 每次匯出動作生成一次',
  `create_datetime` datetime DEFAULT NULL,
  `create_user` int(10) DEFAULT NULL,
  `status` int(4) DEFAULT '1' COMMENT 'cbjob狀態 0為已經刪除 1為正常 默認為 1',
  `sta_id` varchar(4) DEFAULT 'CNT' COMMENT 'CNT  第一次 UPD更新時 COM 複盤完成 END 蓋賬完成',
  PRIMARY KEY (`row_id`),
  KEY `cbjob_id` (`cbjob_id`)
) ENGINE=InnoDB AUTO_INCREMENT=1526 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for certificate_category
-- ----------------------------
DROP TABLE IF EXISTS `certificate_category`;
CREATE TABLE `certificate_category` (
  `rowID` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `certificate_categoryname` varchar(100) DEFAULT NULL COMMENT '證書類別名稱',
  `certificate_categorycode` varchar(100) DEFAULT NULL COMMENT '證書類別編碼',
  `certificate_categoryfid` int(11) DEFAULT NULL COMMENT '所屬大類編號',
  `status` tinyint(2) DEFAULT '1' COMMENT '是否啟用，0-不啟用，1-啟用',
  `sort` int(9) DEFAULT NULL COMMENT '排序',
  `k_user` int(11) DEFAULT NULL COMMENT '創建人',
  `k_date` datetime DEFAULT NULL COMMENT '創建時間',
  PRIMARY KEY (`rowID`),
  KEY `cc_rowID_index` (`rowID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=216 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for challenge
-- ----------------------------
DROP TABLE IF EXISTS `challenge`;
CREATE TABLE `challenge` (
  `challenge_id` char(32) NOT NULL DEFAULT '',
  `challenge_key` varchar(40) NOT NULL DEFAULT '',
  `challenge_ip` varchar(40) NOT NULL DEFAULT '',
  `challenge_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`challenge_id`),
  KEY `ix_challenge_createdate` (`challenge_createdate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for channel
-- ----------------------------
DROP TABLE IF EXISTS `channel`;
CREATE TABLE `channel` (
  `channel_id` int(4) NOT NULL AUTO_INCREMENT,
  `channel_status` int(1) NOT NULL DEFAULT '1',
  `channel_name_full` varchar(200) NOT NULL,
  `channel_name_simple` varchar(200) NOT NULL,
  `channel_email` varchar(200) DEFAULT NULL,
  `company_phone` varchar(40) DEFAULT NULL,
  `company_fax` varchar(40) DEFAULT NULL,
  `company_zip` int(11) DEFAULT NULL,
  `company_address` varchar(200) DEFAULT NULL,
  `channel_invoice` varchar(30) NOT NULL,
  `invoice_title` varchar(200) NOT NULL,
  `invoice_zip` int(11) NOT NULL,
  `invoice_address` varchar(200) NOT NULL,
  `user_id` int(11) NOT NULL,
  `contract_createdate` date DEFAULT NULL,
  `contract_start` date DEFAULT NULL,
  `contract_end` date DEFAULT NULL,
  `annaul_fee` decimal(12,0) DEFAULT NULL,
  `renew_fee` decimal(12,0) DEFAULT NULL,
  `channel_note` varchar(510) DEFAULT NULL,
  `channel_manager` varchar(40) DEFAULT NULL,
  `deal_method` int(1) DEFAULT '1',
  `deal_percent` float DEFAULT NULL,
  `deal_fee` int(1) DEFAULT NULL,
  `creditcard_1_percent` float DEFAULT NULL,
  `creditcard_3_percent` float DEFAULT NULL,
  `shopping_car_percent` float DEFAULT NULL,
  `commission_percent` float DEFAULT NULL,
  `cost_by_percent` int(1) DEFAULT NULL,
  `cost_low_percent` float DEFAULT NULL,
  `cost_normal_percent` float DEFAULT NULL,
  `invoice_period` int(1) DEFAULT NULL,
  `invoice_checkout_day` int(1) DEFAULT NULL,
  `invoice_apply_start` int(11) DEFAULT NULL,
  `invoice_apply_end` int(11) DEFAULT NULL,
  `checkout_note` varchar(510) DEFAULT NULL,
  `receipt_to` int(1) DEFAULT '1',
  `channel_type` int(1) DEFAULT '1',
  `createdate` datetime DEFAULT NULL,
  `updatedate` datetime DEFAULT NULL,
  `model_in` varchar(45) NOT NULL DEFAULT '1',
  `notify_sms` int(1) DEFAULT '0',
  `erp_id` varchar(30) DEFAULT NULL,
  `dept_erp_id` varchar(30) DEFAULT NULL,
  `sales_erp_id` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`channel_id`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for channel_contact
-- ----------------------------
DROP TABLE IF EXISTS `channel_contact`;
CREATE TABLE `channel_contact` (
  `rid` int(9) NOT NULL AUTO_INCREMENT,
  `channel_id` int(4) DEFAULT NULL,
  `contact_type` varchar(40) DEFAULT NULL,
  `contact_name` varchar(40) DEFAULT NULL,
  `contact_phone1` varchar(40) DEFAULT NULL,
  `contact_phone2` varchar(40) DEFAULT NULL,
  `contact_mobile` varchar(40) DEFAULT NULL,
  `contact_email` varchar(200) DEFAULT NULL,
  `createdate` datetime DEFAULT NULL,
  `updatedate` datetime DEFAULT NULL,
  PRIMARY KEY (`rid`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for channel_order
-- ----------------------------
DROP TABLE IF EXISTS `channel_order`;
CREATE TABLE `channel_order` (
  `serial_number` int(4) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `channel_id` int(11) DEFAULT NULL COMMENT '外站ID',
  `order_id` varchar(20) DEFAULT NULL COMMENT '外站交易編號/寄售單號 對到order_master.channel_order_id ',
  `channel_detail_id` varchar(32) DEFAULT NULL COMMENT '外站商品編號, 可對到order_detail.channel_detail_id',
  `store_dispatch_file` varchar(200) DEFAULT NULL COMMENT '配送提貨單, 同批下載的訂單連結到同份提貨單file',
  `dispatch_seq` varchar(20) DEFAULT NULL COMMENT '超商出貨單號',
  `createtime` datetime DEFAULT NULL,
  `ordertime` datetime DEFAULT NULL,
  `latest_deliver_date` datetime DEFAULT NULL,
  PRIMARY KEY (`serial_number`),
  KEY `ix_channel_order` (`order_id`,`channel_detail_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=3197 DEFAULT CHARSET=utf8 COMMENT='外站訂單表';

-- ----------------------------
-- Table structure for channel_orders_record
-- ----------------------------
DROP TABLE IF EXISTS `channel_orders_record`;
CREATE TABLE `channel_orders_record` (
  `id` int(32) NOT NULL AUTO_INCREMENT,
  `record_master_id` int(32) NOT NULL DEFAULT '0',
  `ip` varchar(32) NOT NULL,
  `channel_id` int(9) NOT NULL,
  `channel_detail_id` varchar(32) NOT NULL,
  `cart_num` mediumint(7) NOT NULL,
  `product_price` int(9) NOT NULL DEFAULT '0',
  `product_cost` int(9) NOT NULL DEFAULT '0',
  `group_item_id` varchar(100) NOT NULL,
  `product_name` varchar(510) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3259 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for channel_orders_url
-- ----------------------------
DROP TABLE IF EXISTS `channel_orders_url`;
CREATE TABLE `channel_orders_url` (
  `id` int(32) unsigned NOT NULL AUTO_INCREMENT,
  `ip` varchar(32) NOT NULL,
  `channel_id` int(9) NOT NULL,
  `order_freight_normal` int(9) NOT NULL DEFAULT '0',
  `order_freight_low` int(9) NOT NULL DEFAULT '0',
  `url` varchar(255) NOT NULL,
  `created` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1942 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for channel_shipping
-- ----------------------------
DROP TABLE IF EXISTS `channel_shipping`;
CREATE TABLE `channel_shipping` (
  `channel_id` int(4) NOT NULL,
  `shipping_carrior` int(11) NOT NULL,
  `shipco` varchar(100) NOT NULL,
  `n_threshold` int(9) DEFAULT NULL,
  `n_fee` int(9) DEFAULT NULL,
  `n_return_fee` int(9) DEFAULT NULL,
  `l_threshold` int(9) DEFAULT NULL,
  `l_fee` int(9) DEFAULT NULL,
  `l_return_fee` int(9) DEFAULT NULL,
  `retrieve_mode` int(4) DEFAULT NULL,
  `createdate` datetime DEFAULT NULL,
  `updatedate` datetime DEFAULT NULL,
  PRIMARY KEY (`channel_id`,`shipping_carrior`,`shipco`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for channel_shopping_cart
-- ----------------------------
DROP TABLE IF EXISTS `channel_shopping_cart`;
CREATE TABLE `channel_shopping_cart` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `cart_id` varchar(32) NOT NULL,
  `channel_id` int(9) NOT NULL DEFAULT '1',
  `channel_detail_id` varchar(32) DEFAULT NULL,
  `cart_num` mediumint(7) NOT NULL DEFAULT '1',
  `created` date NOT NULL,
  `modified` date NOT NULL,
  PRIMARY KEY (`id`),
  KEY `channel_inx` (`channel_id`),
  KEY `channel_detail_inx` (`channel_detail_id`),
  KEY `channel,channel_detail_index` (`channel_id`,`channel_detail_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4705 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for china_mgm
-- ----------------------------
DROP TABLE IF EXISTS `china_mgm`;
CREATE TABLE `china_mgm` (
  `ch_id` int(9) unsigned NOT NULL DEFAULT '0',
  `user_id` int(9) unsigned NOT NULL,
  `delivery_name` varchar(64) NOT NULL DEFAULT '',
  `delivery_gender` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `delivery_phone` varchar(30) NOT NULL DEFAULT '',
  `question_email` varchar(255) NOT NULL DEFAULT '',
  `delivery_zip` mediumint(7) unsigned NOT NULL DEFAULT '0',
  `delivery_address` varchar(255) NOT NULL DEFAULT '',
  `status` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `createdate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`ch_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for click_notes
-- ----------------------------
DROP TABLE IF EXISTS `click_notes`;
CREATE TABLE `click_notes` (
  `site_id` int(11) DEFAULT NULL,
  `notes_type` int(11) DEFAULT NULL,
  `notes_id` int(11) DEFAULT NULL,
  `click_year` smallint(4) DEFAULT NULL,
  `click_month` tinyint(2) DEFAULT NULL,
  `click_day` tinyint(2) DEFAULT NULL,
  `click_hour` tinyint(2) DEFAULT NULL,
  `click_week` tinyint(1) DEFAULT NULL,
  `from_ip` varchar(20) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for click_notes_sort
-- ----------------------------
DROP TABLE IF EXISTS `click_notes_sort`;
CREATE TABLE `click_notes_sort` (
  `sort_type` int(11) DEFAULT NULL,
  `site_id` int(11) DEFAULT NULL,
  `notes_type` int(11) DEFAULT NULL,
  `notes_id` int(11) DEFAULT NULL,
  `click_total` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for comment_detail
-- ----------------------------
DROP TABLE IF EXISTS `comment_detail`;
CREATE TABLE `comment_detail` (
  `comment_detail_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '序號.自動增長',
  `comment_id` int(11) DEFAULT NULL COMMENT '評論編號',
  `comment_info` text COMMENT '評論內容',
  `comment_advice` varchar(50) DEFAULT NULL COMMENT '用戶建議',
  `status` tinyint(2) DEFAULT NULL COMMENT '0:不啟用，1：啟用',
  `user_ip` varchar(255) DEFAULT NULL,
  `create_time` int(10) DEFAULT NULL,
  `comment_answer` varchar(200) DEFAULT NULL COMMENT '評價回覆',
  `answer_is_show` tinyint(2) DEFAULT NULL COMMENT '評價回覆是否顯示 0:隱藏 1:顯示',
  `reply_time` int(10) DEFAULT NULL COMMENT '回覆時間',
  `reply_user` int(9) DEFAULT NULL COMMENT '回覆人',
  PRIMARY KEY (`comment_detail_id`),
  KEY `idx_01` (`comment_id`)
) ENGINE=InnoDB AUTO_INCREMENT=8371 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for comment_num
-- ----------------------------
DROP TABLE IF EXISTS `comment_num`;
CREATE TABLE `comment_num` (
  `comment_numid` int(11) NOT NULL AUTO_INCREMENT COMMENT '編號自增',
  `comment_id` int(11) unsigned DEFAULT NULL COMMENT '評論編號',
  `product_desc` tinyint(2) DEFAULT NULL COMMENT '商品與描述相符滿意度',
  `seller_server` tinyint(2) DEFAULT NULL COMMENT '客戶服務滿意度',
  `web_server` tinyint(2) DEFAULT NULL COMMENT '網站整體服務滿意度',
  `logistics_deliver` tinyint(2) DEFAULT NULL COMMENT '配送速度滿意度 ',
  `status` tinyint(2) DEFAULT NULL COMMENT '0:不啟用，1啟用',
  `user_ip` varchar(255) DEFAULT NULL,
  `create_time` int(10) DEFAULT NULL,
  PRIMARY KEY (`comment_numid`),
  KEY `idx_01` (`comment_id`)
) ENGINE=InnoDB AUTO_INCREMENT=8371 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for confapply
-- ----------------------------
DROP TABLE IF EXISTS `confapply`;
CREATE TABLE `confapply` (
  `confapply_id` int(9) unsigned NOT NULL AUTO_INCREMENT,
  `conf_id` int(9) DEFAULT NULL,
  `confapply_org` varchar(64) COLLATE utf8_unicode_ci DEFAULT NULL,
  `confapply_jobtitle` varchar(32) COLLATE utf8_unicode_ci DEFAULT NULL,
  `confapply_name` varchar(16) COLLATE utf8_unicode_ci DEFAULT NULL,
  `confapply_title` varchar(8) COLLATE utf8_unicode_ci DEFAULT NULL,
  `confapply_asis` varchar(32) COLLATE utf8_unicode_ci DEFAULT NULL,
  `confapply_mobile` varchar(50) COLLATE utf8_unicode_ci NOT NULL,
  `confapply_email` varchar(128) COLLATE utf8_unicode_ci DEFAULT NULL,
  `confapply_totalppl` int(4) NOT NULL DEFAULT '1',
  `confapply_partner` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `confapply_note` text COLLATE utf8_unicode_ci,
  `confapply_confirmed` tinyint(1) NOT NULL DEFAULT '0',
  `confapply_initdate` int(10) unsigned DEFAULT '0',
  `confapply_admnote` text COLLATE utf8_unicode_ci,
  PRIMARY KEY (`confapply_id`),
  KEY `cidemail` (`conf_id`,`confapply_email`)
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for config
-- ----------------------------
DROP TABLE IF EXISTS `config`;
CREATE TABLE `config` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `config_name` varchar(255) NOT NULL DEFAULT '',
  `config_value` varchar(255) NOT NULL DEFAULT '',
  `config_content` mediumtext NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `config_name` (`config_name`)
) ENGINE=InnoDB AUTO_INCREMENT=311 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for config_copy
-- ----------------------------
DROP TABLE IF EXISTS `config_copy`;
CREATE TABLE `config_copy` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `config_name` varchar(255) NOT NULL DEFAULT '',
  `config_value` varchar(255) NOT NULL DEFAULT '',
  `config_content` mediumtext NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `config_name` (`config_name`)
) ENGINE=InnoDB AUTO_INCREMENT=311 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for contact_us_question
-- ----------------------------
DROP TABLE IF EXISTS `contact_us_question`;
CREATE TABLE `contact_us_question` (
  `question_id` int(9) unsigned NOT NULL,
  `question_language` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `question_type` smallint(4) unsigned NOT NULL DEFAULT '0',
  `question_company` varchar(64) NOT NULL DEFAULT '',
  `question_username` varchar(64) NOT NULL DEFAULT '',
  `question_email` varchar(255) NOT NULL DEFAULT '',
  `question_phone` varchar(50) NOT NULL,
  `question_reply` varchar(10) DEFAULT '0|0|0' COMMENT '回覆方式-0：不使用，1：使用(信箱 | 簡訊 | 電話)',
  `question_reply_time` int(1) unsigned NOT NULL DEFAULT '0' COMMENT '回覆時間',
  `question_problem` int(1) unsigned NOT NULL DEFAULT '0' COMMENT '問題分類',
  `question_status` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `question_content` text NOT NULL,
  `question_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `question_ipfrom` varchar(40) NOT NULL DEFAULT '',
  `question_file` varchar(40) DEFAULT '',
  PRIMARY KEY (`question_id`),
  KEY `ix_contact_us_question_le` (`question_language`),
  KEY `ix_contact_us_question_ss` (`question_status`),
  KEY `ix_contact_us_question_ce` (`question_createdate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for contact_us_response
-- ----------------------------
DROP TABLE IF EXISTS `contact_us_response`;
CREATE TABLE `contact_us_response` (
  `response_id` int(9) unsigned NOT NULL,
  `question_id` int(9) unsigned NOT NULL,
  `user_id` smallint(4) unsigned NOT NULL,
  `response_content` text NOT NULL,
  `response_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `response_ipfrom` varchar(40) NOT NULL DEFAULT '',
  `response_type` int(4) DEFAULT NULL,
  PRIMARY KEY (`response_id`),
  KEY `ix_contact_us_response_qid` (`question_id`),
  KEY `ix_contact_us_response_uid` (`user_id`),
  CONSTRAINT `fk_contact_us_response_qid` FOREIGN KEY (`question_id`) REFERENCES `contact_us_question` (`question_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_contact_us_response_uid` FOREIGN KEY (`user_id`) REFERENCES `manage_user` (`user_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for counter_click
-- ----------------------------
DROP TABLE IF EXISTS `counter_click`;
CREATE TABLE `counter_click` (
  `site_id` int(9) unsigned NOT NULL,
  `click_id` int(10) unsigned NOT NULL,
  `click_year` smallint(4) unsigned NOT NULL,
  `click_month` tinyint(2) unsigned NOT NULL,
  `click_day` tinyint(2) unsigned NOT NULL,
  `click_hour` tinyint(2) unsigned NOT NULL,
  `click_week` tinyint(1) unsigned NOT NULL,
  `click_total` int(9) unsigned NOT NULL DEFAULT '1',
  PRIMARY KEY (`site_id`,`click_id`),
  CONSTRAINT `fk_counter_click_sid` FOREIGN KEY (`site_id`) REFERENCES `counter_site` (`site_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for counter_person_day
-- ----------------------------
DROP TABLE IF EXISTS `counter_person_day`;
CREATE TABLE `counter_person_day` (
  `site_id` int(9) unsigned NOT NULL,
  `person_id` int(10) unsigned NOT NULL,
  `person_year` smallint(4) unsigned NOT NULL,
  `person_month` tinyint(2) unsigned NOT NULL,
  `person_day` tinyint(2) unsigned NOT NULL,
  `person_week` tinyint(1) unsigned NOT NULL,
  `person_total` int(9) unsigned NOT NULL DEFAULT '1',
  PRIMARY KEY (`site_id`,`person_id`),
  CONSTRAINT `fk_counter_person_day_sid` FOREIGN KEY (`site_id`) REFERENCES `counter_site` (`site_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for counter_person_month
-- ----------------------------
DROP TABLE IF EXISTS `counter_person_month`;
CREATE TABLE `counter_person_month` (
  `site_id` int(9) unsigned NOT NULL,
  `person_id` mediumint(6) unsigned NOT NULL,
  `person_year` smallint(4) unsigned NOT NULL,
  `person_month` tinyint(2) unsigned NOT NULL,
  `person_total` int(9) unsigned NOT NULL DEFAULT '1',
  PRIMARY KEY (`site_id`,`person_id`),
  CONSTRAINT `fk_counter_person_month_sid` FOREIGN KEY (`site_id`) REFERENCES `counter_site` (`site_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for counter_person_year
-- ----------------------------
DROP TABLE IF EXISTS `counter_person_year`;
CREATE TABLE `counter_person_year` (
  `site_id` int(9) unsigned NOT NULL,
  `person_id` smallint(4) unsigned NOT NULL,
  `person_total` int(9) unsigned NOT NULL DEFAULT '1',
  PRIMARY KEY (`site_id`,`person_id`),
  CONSTRAINT `fk_counter_person_year_sid` FOREIGN KEY (`site_id`) REFERENCES `counter_site` (`site_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for counter_session_data
-- ----------------------------
DROP TABLE IF EXISTS `counter_session_data`;
CREATE TABLE `counter_session_data` (
  `session_id` char(32) NOT NULL DEFAULT '',
  `session_time` int(10) unsigned NOT NULL DEFAULT '0',
  `session_ip` varchar(40) NOT NULL DEFAULT '',
  `session_browser` varchar(150) NOT NULL DEFAULT '',
  PRIMARY KEY (`session_id`),
  KEY `session_time` (`session_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for counter_session_site
-- ----------------------------
DROP TABLE IF EXISTS `counter_session_site`;
CREATE TABLE `counter_session_site` (
  `site_id` int(9) unsigned NOT NULL,
  `session_id` char(32) NOT NULL DEFAULT '',
  `session_time` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`site_id`,`session_id`),
  KEY `session_time` (`session_time`),
  KEY `fk_counter_session_site_2` (`session_id`),
  CONSTRAINT `fk_counter_session_site_1` FOREIGN KEY (`site_id`) REFERENCES `counter_site` (`site_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_counter_session_site_2` FOREIGN KEY (`session_id`) REFERENCES `counter_session_data` (`session_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for counter_site
-- ----------------------------
DROP TABLE IF EXISTS `counter_site`;
CREATE TABLE `counter_site` (
  `site_id` int(9) unsigned NOT NULL,
  `site_name` varchar(64) NOT NULL,
  `site_verify` varchar(32) NOT NULL,
  `site_status` tinyint(1) NOT NULL DEFAULT '1',
  `click_id` int(10) unsigned NOT NULL DEFAULT '0',
  `click_total` bigint(19) unsigned NOT NULL DEFAULT '0',
  `person_id` int(8) unsigned NOT NULL DEFAULT '0',
  `person_total` bigint(19) unsigned NOT NULL DEFAULT '0',
  `site_catedate` int(10) unsigned NOT NULL DEFAULT '0',
  `site_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`site_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for course
-- ----------------------------
DROP TABLE IF EXISTS `course`;
CREATE TABLE `course` (
  `course_id` int(11) NOT NULL AUTO_INCREMENT,
  `course_name` varchar(250) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '課程名稱',
  `tel` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '聯繫電話',
  `send_msg` int(2) DEFAULT NULL COMMENT '是否發送簡訊(0:是 1:否)',
  `msg` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '簡訊內容',
  `send_mail` int(2) DEFAULT NULL COMMENT '是否發送mail',
  `mail_content` text COLLATE utf8mb4_unicode_ci COMMENT 'mail內容',
  `start_date` datetime DEFAULT NULL COMMENT '整個課程的開始時間',
  `end_date` datetime DEFAULT NULL COMMENT '整個課程的結束時間',
  `create_time` datetime DEFAULT NULL COMMENT '創建時間',
  `source` int(2) DEFAULT NULL COMMENT '課程來源(0:自辦 1:合作商)',
  `ticket_type` int(2) DEFAULT NULL COMMENT '票券類型(0:虛擬 1:實體)',
  PRIMARY KEY (`course_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for Course
-- ----------------------------
DROP TABLE IF EXISTS `Course`;
CREATE TABLE `Course` (
  `course_id` int(11) NOT NULL AUTO_INCREMENT,
  `course_name` varchar(250) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '課程名稱',
  `tel` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '聯繫電話',
  `send_msg` int(2) DEFAULT NULL COMMENT '是否發送簡訊(0:是 1:否)',
  `msg` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '簡訊內容',
  `send_mail` int(2) DEFAULT NULL COMMENT '是否發送Mail',
  `mail_content` text COLLATE utf8mb4_unicode_ci COMMENT 'Mail內容',
  `start_date` datetime DEFAULT NULL COMMENT '整個課程的開始時間',
  `end_date` datetime DEFAULT NULL COMMENT '整個課程的結束時間',
  `create_time` datetime DEFAULT NULL COMMENT '創建時間',
  `source` int(2) DEFAULT NULL COMMENT '課程來源(0:自辦 1:合作商)',
  `ticket_type` int(2) DEFAULT NULL COMMENT '票券類型(0:虛擬 1:實體)',
  PRIMARY KEY (`course_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for course_detail
-- ----------------------------
DROP TABLE IF EXISTS `course_detail`;
CREATE TABLE `course_detail` (
  `course_detail_id` int(11) NOT NULL AUTO_INCREMENT,
  `course_id` int(11) DEFAULT NULL COMMENT '與course',
  `course_detail_name` varchar(250) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '課程名稱',
  `address` varchar(250) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '上課地點',
  `start_date` datetime DEFAULT NULL COMMENT '單節課程開始時間',
  `end_date` datetime DEFAULT NULL COMMENT '單節課程結束時間',
  `p_number` int(11) DEFAULT NULL COMMENT '人數',
  PRIMARY KEY (`course_detail_id`),
  KEY `course_id` (`course_id`),
  CONSTRAINT `course_detail_ibfk_1` FOREIGN KEY (`course_id`) REFERENCES `course` (`course_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for course_detail_item
-- ----------------------------
DROP TABLE IF EXISTS `course_detail_item`;
CREATE TABLE `course_detail_item` (
  `course_detail_item_id` int(11) NOT NULL AUTO_INCREMENT,
  `course_detail_id` int(11) NOT NULL COMMENT '與course_detail關聯',
  `item_id` int(11) unsigned DEFAULT NULL COMMENT '與product_item關聯',
  `people_count` int(9) DEFAULT NULL COMMENT '人數',
  PRIMARY KEY (`course_detail_item_id`),
  KEY `course_detail_id` (`course_detail_id`),
  KEY `item_id` (`item_id`),
  CONSTRAINT `course_detail_item_ibfk_1` FOREIGN KEY (`course_detail_id`) REFERENCES `course_detail` (`course_detail_id`),
  CONSTRAINT `course_detail_item_ibfk_2` FOREIGN KEY (`item_id`) REFERENCES `product_item` (`item_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for course_detail_item_temp
-- ----------------------------
DROP TABLE IF EXISTS `course_detail_item_temp`;
CREATE TABLE `course_detail_item_temp` (
  `course_detail_item_id` int(11) NOT NULL AUTO_INCREMENT,
  `course_detail_id` int(11) NOT NULL COMMENT '與course_detail關聯',
  `item_id` int(11) unsigned DEFAULT NULL COMMENT '與product_item關聯',
  `writer_id` int(4) NOT NULL,
  `people_count` int(9) DEFAULT NULL COMMENT '人數',
  PRIMARY KEY (`course_detail_item_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for course_picture
-- ----------------------------
DROP TABLE IF EXISTS `course_picture`;
CREATE TABLE `course_picture` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `course_id` int(11) DEFAULT NULL COMMENT '課程id',
  `picture_name` varchar(90) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '圖片路徑',
  `picture_type` varchar(45) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '圖片類型',
  `picture_status` int(2) DEFAULT NULL COMMENT '圖片狀態',
  `picture_sort` int(9) DEFAULT NULL COMMENT '排序',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for course_product
-- ----------------------------
DROP TABLE IF EXISTS `course_product`;
CREATE TABLE `course_product` (
  `course_product_id` int(11) NOT NULL AUTO_INCREMENT,
  `course_id` int(11) NOT NULL COMMENT '與course關聯',
  `product_id` int(9) unsigned NOT NULL COMMENT '與product關聯',
  PRIMARY KEY (`course_product_id`),
  KEY `course_id` (`course_id`),
  KEY `product_id` (`product_id`),
  CONSTRAINT `course_product_ibfk_1` FOREIGN KEY (`course_id`) REFERENCES `course` (`course_id`),
  CONSTRAINT `course_product_ibfk_2` FOREIGN KEY (`product_id`) REFERENCES `product` (`product_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for course_product_temp
-- ----------------------------
DROP TABLE IF EXISTS `course_product_temp`;
CREATE TABLE `course_product_temp` (
  `course_product_id` int(11) NOT NULL AUTO_INCREMENT,
  `writer_id` int(4) NOT NULL,
  `course_id` int(11) NOT NULL COMMENT '與course關聯',
  `product_id` int(10) unsigned NOT NULL COMMENT '與product關聯',
  PRIMARY KEY (`course_product_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for course_ticket
-- ----------------------------
DROP TABLE IF EXISTS `course_ticket`;
CREATE TABLE `course_ticket` (
  `ticket_id` int(11) NOT NULL AUTO_INCREMENT,
  `course_detail_item_id` int(11) DEFAULT NULL COMMENT '與course_detail關聯',
  `ticket_code` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '虛擬:存虛擬編碼,實體:可以為空',
  `user_id` int(11) DEFAULT NULL COMMENT '購買人id可用來做驗證用',
  `create_date` datetime DEFAULT NULL COMMENT '創建時間',
  `create_user` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '創建人員',
  `flag` int(2) DEFAULT NULL COMMENT '票券狀態(0:未使用 1:已使用 2:已退訂)',
  PRIMARY KEY (`ticket_id`),
  KEY `course_detail_item_id` (`course_detail_item_id`),
  CONSTRAINT `course_ticket_ibfk_1` FOREIGN KEY (`course_detail_item_id`) REFERENCES `course_detail_item` (`course_detail_item_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for cvs_acceptance_f03
-- ----------------------------
DROP TABLE IF EXISTS `cvs_acceptance_f03`;
CREATE TABLE `cvs_acceptance_f03` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ecno` varchar(10) NOT NULL COMMENT '網站代號',
  `deliver_id` int(11) NOT NULL,
  `cnno` varchar(5) NOT NULL COMMENT '通路代號',
  `cuname` varchar(50) NOT NULL COMMENT '取貨人姓名',
  `prodtype` int(1) NOT NULL COMMENT '商品別代碼',
  `pincode` varchar(20) NOT NULL,
  `set_time` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=856 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for cvs_accounts_abnormal_f21
-- ----------------------------
DROP TABLE IF EXISTS `cvs_accounts_abnormal_f21`;
CREATE TABLE `cvs_accounts_abnormal_f21` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ecno` varchar(3) NOT NULL COMMENT '廠商代碼',
  `stno` varchar(7) NOT NULL COMMENT '取貨門市編號',
  `order_id` int(11) NOT NULL,
  `deliver_id` int(11) NOT NULL,
  `status` varchar(1) NOT NULL COMMENT '結帳狀態碼',
  `ret_r` varchar(10) NOT NULL COMMENT '結帳狀態原因',
  `tradetype` varchar(1) NOT NULL COMMENT '交易方式識別碼',
  `tkdt` varchar(8) NOT NULL COMMENT '結帳基準日',
  `amt` int(11) NOT NULL COMMENT '代收金額',
  `realamt` int(11) NOT NULL COMMENT '商品實際金額',
  `fee` varchar(20) NOT NULL COMMENT '手續費',
  `set_time` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for cvs_cancel_collection_f45
-- ----------------------------
DROP TABLE IF EXISTS `cvs_cancel_collection_f45`;
CREATE TABLE `cvs_cancel_collection_f45` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `bc1` varchar(9) NOT NULL COMMENT '第一段條碼',
  `bc2` varchar(16) NOT NULL COMMENT '第二段條碼',
  `stno` varchar(7) NOT NULL COMMENT '取貨門市編號',
  `rtdt` varchar(8) NOT NULL COMMENT '實際取貨代收日期',
  `tkdt` varchar(8) NOT NULL COMMENT '結帳基準日期',
  `set_time` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for cvs_cancel_deliver_f09
-- ----------------------------
DROP TABLE IF EXISTS `cvs_cancel_deliver_f09`;
CREATE TABLE `cvs_cancel_deliver_f09` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ret_m` varchar(2) NOT NULL COMMENT '取消類型',
  `ecno` varchar(3) NOT NULL COMMENT '網站代號',
  `stno` varchar(7) NOT NULL COMMENT '取貨門市編號',
  `deliver_id` int(11) NOT NULL,
  `ret_r` varchar(3) NOT NULL COMMENT '取消訂單原因',
  `set_time` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=62 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for cvs_convenience_store_f01
-- ----------------------------
DROP TABLE IF EXISTS `cvs_convenience_store_f01`;
CREATE TABLE `cvs_convenience_store_f01` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `stno` varchar(20) NOT NULL COMMENT '商店id',
  `stnm` varchar(255) NOT NULL COMMENT '商店名稱',
  `sttel` varchar(30) NOT NULL COMMENT '商店電話',
  `stcity` varchar(50) NOT NULL COMMENT '縣市',
  `stcntry` varchar(50) NOT NULL COMMENT '區域',
  `stadr` varchar(255) NOT NULL COMMENT '地址',
  `zipcd` int(5) NOT NULL COMMENT '郵遞區號',
  `dcrono` varchar(50) NOT NULL COMMENT '路順路線',
  `sdate` varchar(50) NOT NULL,
  `edate` varchar(50) NOT NULL,
  `set_time` datetime NOT NULL,
  `up_time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5189 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for cvs_examine_back_f07
-- ----------------------------
DROP TABLE IF EXISTS `cvs_examine_back_f07`;
CREATE TABLE `cvs_examine_back_f07` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ret_m` varchar(3) NOT NULL COMMENT '退貨原因',
  `ecno` varchar(3) NOT NULL COMMENT '網站代號',
  `stno` varchar(7) NOT NULL COMMENT '取貨門市編號',
  `deliver_id` int(11) NOT NULL,
  `rtdcdt` varchar(8) NOT NULL COMMENT '大物流實際驗退日',
  `frtdcdt` varchar(8) NOT NULL COMMENT '8 結帳基準日',
  `set_time` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for cvs_in_store_f44
-- ----------------------------
DROP TABLE IF EXISTS `cvs_in_store_f44`;
CREATE TABLE `cvs_in_store_f44` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ecno` varchar(5) NOT NULL COMMENT '網站代號',
  `stno` varchar(10) NOT NULL COMMENT '取貨門市編號',
  `deliver_id` int(11) NOT NULL,
  `dcstdt` varchar(8) NOT NULL COMMENT '實際進店日期',
  `set_time` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=943 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for cvs_order_return_f11
-- ----------------------------
DROP TABLE IF EXISTS `cvs_order_return_f11`;
CREATE TABLE `cvs_order_return_f11` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `errcode` varchar(1) NOT NULL COMMENT '錯誤代碼',
  `errdesc` varchar(255) NOT NULL COMMENT '錯誤說明',
  `ecno` varchar(10) NOT NULL COMMENT 'EC 網站代號',
  `order_id` int(11) NOT NULL,
  `deliver_id` int(11) NOT NULL,
  `stno` varchar(10) NOT NULL COMMENT '取貨門市編號',
  `atm` int(5) NOT NULL COMMENT '代收金額',
  `cutknm` varchar(20) NOT NULL COMMENT '取貨人姓名',
  `cutktl` varchar(20) NOT NULL COMMENT '取貨人電話',
  `prodnm` varchar(1) NOT NULL COMMENT '商品別代碼',
  `ecweb` varchar(200) NOT NULL COMMENT 'EC 網站名稱',
  `ecsertel` varchar(20) NOT NULL COMMENT 'EC網站客服電話',
  `realamt` int(5) NOT NULL COMMENT '商品實際金額',
  `tradetype` varchar(1) NOT NULL COMMENT '交易方式識別碼',
  `sercode` varchar(3) NOT NULL COMMENT '代收代號',
  `edcno` varchar(3) NOT NULL COMMENT '大物流代號',
  `set_time` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for cvs_pickup_f05
-- ----------------------------
DROP TABLE IF EXISTS `cvs_pickup_f05`;
CREATE TABLE `cvs_pickup_f05` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `bc1` varchar(9) NOT NULL COMMENT '第一段條碼',
  `bc2` varchar(16) NOT NULL COMMENT '第二段條碼',
  `stno` varchar(7) NOT NULL COMMENT '取貨門市編號',
  `rtdt` varchar(8) NOT NULL COMMENT '實際取貨代收日期',
  `tkdt` varchar(8) NOT NULL COMMENT '結帳基準日期',
  `set_time` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=855 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for cvs_pickup_immediate_f17
-- ----------------------------
DROP TABLE IF EXISTS `cvs_pickup_immediate_f17`;
CREATE TABLE `cvs_pickup_immediate_f17` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `bc1` varchar(9) NOT NULL COMMENT '第一段條碼',
  `bc2` varchar(16) NOT NULL COMMENT '第二段條碼',
  `stno` varchar(7) NOT NULL COMMENT '取貨門市編號',
  `rtdt` varchar(8) NOT NULL COMMENT '實際代收日期',
  `pincode` varchar(12) NOT NULL COMMENT '繳費代碼',
  `set_time` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for cvs_timeout_back_f61
-- ----------------------------
DROP TABLE IF EXISTS `cvs_timeout_back_f61`;
CREATE TABLE `cvs_timeout_back_f61` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ecno` varchar(3) NOT NULL COMMENT '網站代號',
  `deliver_id` int(11) NOT NULL,
  `stno` varchar(7) NOT NULL COMMENT '取貨門市編號',
  `rtdt` varchar(8) NOT NULL COMMENT '預計退貨日',
  `set_time` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for deliver_detail
-- ----------------------------
DROP TABLE IF EXISTS `deliver_detail`;
CREATE TABLE `deliver_detail` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `deliver_id` int(9) unsigned NOT NULL DEFAULT '0',
  `detail_id` int(9) unsigned NOT NULL DEFAULT '0',
  `delivery_status` int(11) NOT NULL,
  `predict_org_days` int(11) DEFAULT '0' COMMENT '預計到貨日',
  PRIMARY KEY (`id`),
  UNIQUE KEY `deliver_id` (`deliver_id`,`detail_id`),
  KEY `detail_id` (`detail_id`)
) ENGINE=InnoDB AUTO_INCREMENT=369411 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for deliver_master
-- ----------------------------
DROP TABLE IF EXISTS `deliver_master`;
CREATE TABLE `deliver_master` (
  `deliver_id` int(11) unsigned NOT NULL DEFAULT '0',
  `order_id` int(11) NOT NULL DEFAULT '0',
  `ticket_id` int(11) NOT NULL DEFAULT '0',
  `type` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `export_id` int(11) NOT NULL,
  `import_id` int(11) NOT NULL,
  `freight_set` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `delivery_status` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `delivery_name` varchar(10) NOT NULL,
  `delivery_mobile` varchar(50) NOT NULL,
  `delivery_phone` varchar(30) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `delivery_zip` mediumint(7) unsigned NOT NULL DEFAULT '0',
  `delivery_address` varchar(255) NOT NULL DEFAULT '',
  `delivery_store` smallint(4) unsigned NOT NULL DEFAULT '1',
  `delivery_code` varchar(50) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `delivery_freight_cost` int(4) NOT NULL DEFAULT '0' COMMENT '物流費',
  `delivery_date` datetime DEFAULT NULL,
  `pickup_date` datetime DEFAULT NULL,
  `sms_date` datetime DEFAULT NULL,
  `arrival_date` datetime DEFAULT NULL,
  `estimated_delivery_date` date DEFAULT NULL,
  `estimated_arrival_date` date DEFAULT NULL,
  `estimated_arrival_period` int(11) NOT NULL,
  `creator` int(11) NOT NULL,
  `verifier` int(11) NOT NULL,
  `created` datetime NOT NULL,
  `modified` datetime NOT NULL,
  `export_flag` smallint(5) NOT NULL DEFAULT '0',
  `data_chg` tinyint(2) NOT NULL DEFAULT '0',
  `work_status` int(4) NOT NULL DEFAULT '0' COMMENT '批次號下訂單是否已經指派工作項，默認0:沒有指派，變為1:已經指派',
  `deliver_org_days` int(4) DEFAULT '0' COMMENT '預計到貨日',
  `expect_arrive_date` date NOT NULL DEFAULT '0001-01-01' COMMENT '期望到貨日',
  `expect_arrive_period` int(11) NOT NULL DEFAULT '0' COMMENT '期望到貨時段',
  PRIMARY KEY (`deliver_id`),
  KEY `ticket_id` (`ticket_id`),
  KEY `order_id_index` (`order_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for deliver_status
-- ----------------------------
DROP TABLE IF EXISTS `deliver_status`;
CREATE TABLE `deliver_status` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `deliver_id` int(11) NOT NULL COMMENT '出貨單號',
  `state` int(2) NOT NULL COMMENT '商品送達狀態:0收到訂單、1揀貨、2理貨、3出貨、4.已送達',
  `settime` datetime NOT NULL COMMENT '建立時間',
  `endtime` datetime NOT NULL COMMENT '商品最後處理時間',
  `freight_type` smallint(11) NOT NULL COMMENT '物流配送模式',
  `Logistics_providers` smallint(11) NOT NULL COMMENT '物流商',
  PRIMARY KEY (`id`),
  KEY `index_deliverid` (`deliver_id`)
) ENGINE=InnoDB AUTO_INCREMENT=136094 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for Delivery
-- ----------------------------
DROP TABLE IF EXISTS `Delivery`;
CREATE TABLE `Delivery` (
  `De000` varchar(20) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL COMMENT '訂單編號',
  `De001` varchar(20) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL COMMENT '客戶代號',
  `De002` varchar(100) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL COMMENT '客戶地址',
  `De003` smallint(5) DEFAULT NULL COMMENT '配送順序',
  `De004` varchar(20) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL COMMENT '配送狀態',
  `De005` varchar(100) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL COMMENT '配送備註',
  `De006` varchar(10) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL COMMENT '簽收人',
  `De007` datetime DEFAULT NULL COMMENT 'UPT yyyy/mm/dd',
  `De008` varchar(20) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL COMMENT 'UPU',
  `Cust02` varchar(50) DEFAULT NULL COMMENT '客戶姓名',
  `Cust03` varchar(50) DEFAULT NULL COMMENT '客戶電話一',
  `Cust04` varchar(50) DEFAULT NULL COMMENT '客戶電話二',
  `Cust05` varchar(100) DEFAULT NULL COMMENT '客戶備註',
  PRIMARY KEY (`De000`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for delivery_address
-- ----------------------------
DROP TABLE IF EXISTS `delivery_address`;
CREATE TABLE `delivery_address` (
  `da_id` int(11) NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `user_id` int(11) NOT NULL COMMENT '用戶主ID',
  `da_title` varchar(50) NOT NULL COMMENT '配送名稱',
  `da_name` varchar(50) NOT NULL COMMENT '收件人姓名',
  `da_gender` int(11) NOT NULL COMMENT '收件人稱謂：1）先生 2）小姐',
  `da_mobile_no` varchar(50) NOT NULL,
  `da_tel_no` varchar(20) NOT NULL COMMENT '收件人市內電話',
  `da_dist` varchar(255) NOT NULL COMMENT '收件人聯絡地址－區',
  `da_address` varchar(255) NOT NULL COMMENT '收件人地址－街道',
  `da_default` int(11) NOT NULL COMMENT '為該會員預設配送地址：1）是 2）否（備註：這個欄位放在會員資料表可以節省資料庫更新的判斷，但先放這張表，再討論。）',
  `da_created` datetime NOT NULL COMMENT '建立時間－yyyymmdd hh:mm:ss',
  `da_modified` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '最後修改',
  PRIMARY KEY (`da_id`)
) ENGINE=InnoDB AUTO_INCREMENT=38 DEFAULT CHARSET=utf8 COMMENT='會員配送資訊資料表';

-- ----------------------------
-- Table structure for delivery_change_log
-- ----------------------------
DROP TABLE IF EXISTS `delivery_change_log`;
CREATE TABLE `delivery_change_log` (
  `dcl_id` int(11) NOT NULL AUTO_INCREMENT,
  `deliver_id` int(11) NOT NULL COMMENT '出貨單編號',
  `dcl_create_user` int(11) NOT NULL DEFAULT '0' COMMENT '創建人',
  `dcl_create_datetime` datetime NOT NULL,
  `dcl_create_muser` int(11) NOT NULL DEFAULT '0' COMMENT '創建人管理員',
  `dcl_create_type` int(11) NOT NULL DEFAULT '1' COMMENT '創建類型1:前台創建 2:後台創建',
  `dcl_note` varchar(255) NOT NULL COMMENT '備註',
  `dcl_ipfrom` varchar(40) NOT NULL COMMENT '來源ip',
  `expect_arrive_date` date NOT NULL COMMENT '期望到貨日',
  `expect_arrive_period` int(4) NOT NULL DEFAULT '0' COMMENT '預計到貨時段',
  PRIMARY KEY (`dcl_id`),
  KEY `deliver_id` (`deliver_id`)
) ENGINE=InnoDB AUTO_INCREMENT=400 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for delivery_freight_set_mapping
-- ----------------------------
DROP TABLE IF EXISTS `delivery_freight_set_mapping`;
CREATE TABLE `delivery_freight_set_mapping` (
  `product_freight_set` int(11) NOT NULL,
  `delivery_freight_set` int(11) NOT NULL,
  PRIMARY KEY (`product_freight_set`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for delivery_store
-- ----------------------------
DROP TABLE IF EXISTS `delivery_store`;
CREATE TABLE `delivery_store` (
  `rowid` int(11) NOT NULL AUTO_INCREMENT,
  `delivery_store_id` int(4) NOT NULL,
  `big` varchar(510) DEFAULT NULL,
  `bigcode` varchar(510) DEFAULT NULL,
  `middle` varchar(510) DEFAULT NULL,
  `middlecode` varchar(510) DEFAULT NULL,
  `small` varchar(510) DEFAULT NULL,
  `smallcode` varchar(510) DEFAULT NULL,
  `store_id` varchar(20) NOT NULL,
  `store_name` varchar(510) NOT NULL,
  `address` varchar(510) NOT NULL,
  `phone` varchar(20) NOT NULL,
  `status` tinyint(2) NOT NULL DEFAULT '1',
  PRIMARY KEY (`rowid`),
  KEY `ix_store` (`delivery_store_id`,`store_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COMMENT='超商店家';

-- ----------------------------
-- Table structure for design_request
-- ----------------------------
DROP TABLE IF EXISTS `design_request`;
CREATE TABLE `design_request` (
  `dr_id` int(11) unsigned zerofill NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `dr_requester_id` int(11) NOT NULL COMMENT '需求提出者User ID',
  `product_id` int(9) unsigned NOT NULL,
  `dr_type` int(11) NOT NULL COMMENT '需求類型：1）電子報 2）活動頁 3）品牌故事 4）內頁設計 5）商品圖280x280 6）Banner',
  `dr_assign_to` int(11) NOT NULL COMMENT '被指派需求執行者User ID',
  `dr_content_text` text NOT NULL COMMENT '文案內容',
  `dr_description` varchar(255) NOT NULL COMMENT '需求說明',
  `dr_resource_path` varchar(255) NOT NULL COMMENT '素材路徑',
  `dr_document_path` varchar(255) NOT NULL COMMENT '文件路徑',
  `dr_status` int(11) NOT NULL COMMENT '需求狀態：1）新建立 2）已指派 3）製作中 4）待校稿 5）已結案',
  `dr_created` datetime NOT NULL COMMENT '建立時間',
  `dr_modified` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '最後修改',
  `dr_expected` datetime DEFAULT NULL,
  PRIMARY KEY (`dr_id`) COMMENT 'ID'
) ENGINE=InnoDB AUTO_INCREMENT=64 DEFAULT CHARSET=utf8 COMMENT='設計需求派工資料表';

-- ----------------------------
-- Table structure for disable_keywords
-- ----------------------------
DROP TABLE IF EXISTS `disable_keywords`;
CREATE TABLE `disable_keywords` (
  `dk_id` int(11) NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `dk_string` varchar(255) NOT NULL COMMENT '禁用字串',
  `user_id` int(11) NOT NULL COMMENT '建立者User ID',
  `dk_created` datetime NOT NULL COMMENT '建立時間',
  `dk_modified` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '最後更新',
  `dk_active` int(2) NOT NULL DEFAULT '0' COMMENT '0啟用,1不啟用',
  PRIMARY KEY (`dk_id`),
  KEY `dk_string` (`dk_string`) COMMENT '禁用字串'
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8 COMMENT='禁用關鍵字資料表';

-- ----------------------------
-- Table structure for edm_content
-- ----------------------------
DROP TABLE IF EXISTS `edm_content`;
CREATE TABLE `edm_content` (
  `content_id` int(9) unsigned NOT NULL DEFAULT '0',
  `group_id` int(9) unsigned NOT NULL DEFAULT '0',
  `content_status` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `content_email_id` int(9) unsigned NOT NULL DEFAULT '0',
  `content_start` int(10) unsigned NOT NULL DEFAULT '0',
  `content_end` int(10) unsigned NOT NULL DEFAULT '0',
  `content_range` int(10) unsigned NOT NULL DEFAULT '0',
  `content_single_count` int(10) unsigned NOT NULL DEFAULT '0',
  `content_click` int(9) unsigned NOT NULL DEFAULT '0',
  `content_person` int(9) unsigned NOT NULL DEFAULT '0',
  `content_send_success` int(9) unsigned NOT NULL DEFAULT '0',
  `content_send_failed` int(9) unsigned NOT NULL DEFAULT '0',
  `content_from_name` varchar(255) NOT NULL,
  `content_from_email` varchar(255) NOT NULL,
  `content_reply_email` varchar(255) NOT NULL,
  `content_priority` tinyint(1) unsigned NOT NULL DEFAULT '3',
  `content_title` varchar(255) NOT NULL,
  `info_epaper_id` int(9) DEFAULT NULL,
  `content_body` mediumtext NOT NULL,
  `content_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `content_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`content_id`),
  KEY `ix_edm_content_cs` (`content_status`),
  KEY `fk_edm_content_gid` (`group_id`),
  CONSTRAINT `fk_edm_content_gid` FOREIGN KEY (`group_id`) REFERENCES `edm_group` (`group_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for edm_content_new
-- ----------------------------
DROP TABLE IF EXISTS `edm_content_new`;
CREATE TABLE `edm_content_new` (
  `content_id` int(9) NOT NULL AUTO_INCREMENT COMMENT 'EDM內容代碼，主索引鍵',
  `group_id` int(9) NOT NULL COMMENT 'EDM群組代碼，決定該EDM屬於哪一個EDM群組',
  `subject` varchar(255) NOT NULL COMMENT 'EDM郵件標題',
  `template_id` int(9) NOT NULL COMMENT '所選郵件範本代碼',
  `template_data` mediumtext NOT NULL COMMENT '範本程式產生的相關資料，未來產生郵件內容時，範本程式會需要讀回去的資料',
  `importance` int(9) NOT NULL COMMENT '郵件重要性',
  `sender_id` int(10) NOT NULL COMMENT 'EDM寄件人代碼',
  `content_createdate` datetime NOT NULL COMMENT 'EDM內容建立時間',
  `content_updatedate` datetime NOT NULL COMMENT 'EDM內容修改時間',
  `content_create_userid` int(9) NOT NULL COMMENT 'EDM內容建立者',
  `content_update_userid` int(9) NOT NULL COMMENT 'EDM內容修改者',
  `pm` int(9) NOT NULL COMMENT '需求提出者',
  PRIMARY KEY (`content_id`)
) ENGINE=InnoDB AUTO_INCREMENT=57 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for edm_daily_open
-- ----------------------------
DROP TABLE IF EXISTS `edm_daily_open`;
CREATE TABLE `edm_daily_open` (
  `open_id` int(9) unsigned NOT NULL,
  `content_id` int(9) unsigned NOT NULL DEFAULT '0',
  `email_id` int(9) unsigned NOT NULL DEFAULT '0',
  `open_total` int(9) unsigned NOT NULL DEFAULT '1',
  `open_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `open_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `open_ipfrom` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`open_id`,`content_id`,`email_id`),
  KEY `fk_edm_open_cid` (`content_id`),
  KEY `fk_edm_open_eid` (`email_id`),
  CONSTRAINT `fk_edm_open_cid` FOREIGN KEY (`content_id`) REFERENCES `edm_content` (`content_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_edm_open_eid` FOREIGN KEY (`email_id`) REFERENCES `edm_email` (`email_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for edm_daily_statistics
-- ----------------------------
DROP TABLE IF EXISTS `edm_daily_statistics`;
CREATE TABLE `edm_daily_statistics` (
  `statistics_id` int(9) unsigned NOT NULL,
  `content_id` int(9) unsigned NOT NULL DEFAULT '0',
  `total_click` int(9) unsigned NOT NULL DEFAULT '1',
  `total_person` int(9) unsigned NOT NULL DEFAULT '1',
  PRIMARY KEY (`statistics_id`,`content_id`),
  KEY `fk_edm_statistics_cid` (`content_id`),
  CONSTRAINT `fk_edm_statistics_cid` FOREIGN KEY (`content_id`) REFERENCES `edm_content` (`content_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for edm_email
-- ----------------------------
DROP TABLE IF EXISTS `edm_email`;
CREATE TABLE `edm_email` (
  `email_id` int(9) unsigned NOT NULL DEFAULT '0',
  `email_name` varchar(255) NOT NULL,
  `email_address` varchar(255) NOT NULL,
  `email_check` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `email_sent` smallint(4) unsigned NOT NULL DEFAULT '0',
  `email_user_unknown` smallint(4) unsigned NOT NULL DEFAULT '0',
  `email_click` int(10) unsigned NOT NULL DEFAULT '0',
  `email_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `email_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`email_id`),
  UNIQUE KEY `uk_edm_email_ea` (`email_address`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for edm_email_copy
-- ----------------------------
DROP TABLE IF EXISTS `edm_email_copy`;
CREATE TABLE `edm_email_copy` (
  `email_id` int(9) unsigned NOT NULL DEFAULT '0',
  `email_name` varchar(255) NOT NULL,
  `email_address` varchar(255) NOT NULL,
  `email_check` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `email_sent` smallint(4) unsigned NOT NULL DEFAULT '0',
  `email_user_unknown` smallint(4) unsigned NOT NULL DEFAULT '0',
  `email_click` int(10) unsigned NOT NULL DEFAULT '0',
  `email_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `email_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`email_id`),
  UNIQUE KEY `uk_edm_email_ea` (`email_address`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for edm_group
-- ----------------------------
DROP TABLE IF EXISTS `edm_group`;
CREATE TABLE `edm_group` (
  `group_id` int(9) unsigned NOT NULL DEFAULT '0',
  `group_name` varchar(255) NOT NULL,
  `group_total_email` int(9) unsigned NOT NULL DEFAULT '0',
  `status` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `group_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `group_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`group_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for edm_group_email
-- ----------------------------
DROP TABLE IF EXISTS `edm_group_email`;
CREATE TABLE `edm_group_email` (
  `group_id` int(9) unsigned NOT NULL DEFAULT '0',
  `email_id` int(9) unsigned NOT NULL DEFAULT '0',
  `email_name` varchar(255) NOT NULL,
  `email_status` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `email_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `email_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`group_id`,`email_id`),
  KEY `fk_edm_group_email_eid` (`email_id`),
  CONSTRAINT `fk_edm_group_email_eid` FOREIGN KEY (`email_id`) REFERENCES `edm_email` (`email_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_edm_group_email_gid` FOREIGN KEY (`group_id`) REFERENCES `edm_group` (`group_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for edm_group_new
-- ----------------------------
DROP TABLE IF EXISTS `edm_group_new`;
CREATE TABLE `edm_group_new` (
  `group_id` int(9) NOT NULL AUTO_INCREMENT COMMENT '電子報群組代碼',
  `group_name` varchar(255) DEFAULT NULL COMMENT '群組名稱',
  `description` varchar(255) DEFAULT NULL COMMENT '群組描述文字',
  `is_member_edm` tinyint(4) DEFAULT '0' COMMENT '指定該群組是不是會員訂閱電子報，設定為True時，該群組將會顯示在會員中心電子報管理畫面中，讓會員自行選擇是否要訂閱。',
  `enabled` tinyint(4) DEFAULT '1' COMMENT '設定群組是否啟用。如果不啟用，即使is_member_edm為True，也不會顯示在會員中心電子報管理畫面；同時也不會顯示在電子報管理後台。',
  `sort_order` int(9) DEFAULT NULL COMMENT '群組排序。當is_member_edm為True時，該群組會顯示在會員中心的電子報訂閱畫面，此時採用這個值來決定顯示的排序。',
  `group_createdate` timestamp NULL DEFAULT NULL,
  `group_updatedate` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `group_create_userid` int(9) DEFAULT NULL,
  `group_update_userid` int(9) DEFAULT NULL,
  `trial_url` varchar(255) DEFAULT NULL COMMENT '試閱',
  PRIMARY KEY (`group_id`)
) ENGINE=InnoDB AUTO_INCREMENT=58 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for edm_list_condition_main
-- ----------------------------
DROP TABLE IF EXISTS `edm_list_condition_main`;
CREATE TABLE `edm_list_condition_main` (
  `elcm_id` int(11) NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `elcm_creator_id` int(11) NOT NULL COMMENT '建立者ID',
  `elcm_name` varchar(255) NOT NULL COMMENT '條件名稱',
  `elcm_created` datetime NOT NULL COMMENT '建立時間',
  `elcm_modified` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '最後修改',
  PRIMARY KEY (`elcm_id`)
) ENGINE=InnoDB AUTO_INCREMENT=175 DEFAULT CHARSET=utf8 COMMENT='EDM名單條件主資料表';

-- ----------------------------
-- Table structure for edm_list_conditoin_sub
-- ----------------------------
DROP TABLE IF EXISTS `edm_list_conditoin_sub`;
CREATE TABLE `edm_list_conditoin_sub` (
  `elcs_id` int(11) NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `elcm_id` int(11) NOT NULL COMMENT 'EDM名單條件主資料ID',
  `elcs_key` varchar(255) NOT NULL COMMENT '鍵值（gender, age, register_time, last_order, last_login, buy_times, cancel_times, return_times, replenishment_info, total_consumption,black_list）',
  `elcs_value1` varchar(255) NOT NULL COMMENT '值一',
  `elcs_value2` varchar(255) DEFAULT '' COMMENT '值二（可空白）',
  `elcs_value3` varchar(255) DEFAULT '' COMMENT '值三（可空白）',
  `elcs_value4` varchar(255) DEFAULT '' COMMENT '值四（可空白）',
  PRIMARY KEY (`elcs_id`),
  KEY `elcm_id` (`elcm_id`)
) ENGINE=InnoDB AUTO_INCREMENT=1187 DEFAULT CHARSET=utf8 COMMENT='EDM名單條件子資料表';

-- ----------------------------
-- Table structure for edm_send
-- ----------------------------
DROP TABLE IF EXISTS `edm_send`;
CREATE TABLE `edm_send` (
  `content_id` int(9) unsigned NOT NULL DEFAULT '0',
  `email_id` int(9) unsigned NOT NULL DEFAULT '0',
  `send_status` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `send_datetime` int(10) unsigned NOT NULL DEFAULT '0',
  `open_first` int(10) unsigned NOT NULL DEFAULT '0',
  `open_last` int(10) unsigned NOT NULL DEFAULT '0',
  `open_total` int(9) unsigned NOT NULL DEFAULT '1',
  PRIMARY KEY (`content_id`,`email_id`),
  KEY `fk_edm_send_eid` (`email_id`),
  CONSTRAINT `fk_edm_send_cid` FOREIGN KEY (`content_id`) REFERENCES `edm_content` (`content_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_edm_send_eid` FOREIGN KEY (`email_id`) REFERENCES `edm_email` (`email_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for edm_send_log
-- ----------------------------
DROP TABLE IF EXISTS `edm_send_log`;
CREATE TABLE `edm_send_log` (
  `log_id` int(9) NOT NULL AUTO_INCREMENT COMMENT '發送記錄代碼',
  `content_id` int(9) NOT NULL COMMENT '電子報代碼',
  `test_send` tinyint(1) NOT NULL COMMENT '是否測試發送。測試發送=1，正式發送=0',
  `receiver_count` int(9) NOT NULL COMMENT '本次發送總共寫了多少筆資料到mail_queue',
  `schedule_date` datetime NOT NULL COMMENT '排程寄送時間',
  `expire_date` datetime DEFAULT NULL COMMENT '該EDM的到期時間',
  `createdate` datetime NOT NULL COMMENT '本次「測試發送」或「正式發送」按下的時間',
  `create_userid` int(9) unsigned zerofill NOT NULL COMMENT '本次「測試發送」或「正式發送」按下的人',
  PRIMARY KEY (`log_id`)
) ENGINE=InnoDB AUTO_INCREMENT=196 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for edm_subscription
-- ----------------------------
DROP TABLE IF EXISTS `edm_subscription`;
CREATE TABLE `edm_subscription` (
  `group_id` int(9) NOT NULL AUTO_INCREMENT COMMENT '電子報群組代碼',
  `user_id` int(9) unsigned NOT NULL COMMENT '會員代碼',
  PRIMARY KEY (`group_id`,`user_id`),
  KEY `f_users_user_id_subscription` (`user_id`),
  CONSTRAINT `f_edm_group_new_group_id` FOREIGN KEY (`group_id`) REFERENCES `edm_group_new` (`group_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `f_users_user_id_subscription` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=54 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for edm_template
-- ----------------------------
DROP TABLE IF EXISTS `edm_template`;
CREATE TABLE `edm_template` (
  `template_id` int(9) NOT NULL AUTO_INCREMENT COMMENT 'EDM範本代碼',
  `template_name` varchar(255) NOT NULL COMMENT 'EDM範本名稱',
  `edit_url` varchar(255) NOT NULL COMMENT 'EDM編輯者，選擇該範本後，用來給編輯者提供該範本相關資料的網頁',
  `content_url` varchar(255) NOT NULL COMMENT '最終用來產出EDM內容的網頁，會被程式呼叫，以便取得EDM郵件內容。產出的內容會用來寫入到mail_request的body欄位',
  `enabled` tinyint(4) NOT NULL DEFAULT '1' COMMENT '設定該範本是否啟用，不啟用的時候，EDM編輯者在EDM新增修改畫面就選不到該範本',
  `static_template` bit(1) NOT NULL DEFAULT b'1' COMMENT '是否為靜態範本。靜態範本就是指內容產生一次就適用所有的收件人，動態範本則需要依照每個收件人產生一次內容。',
  `template_createdate` datetime NOT NULL COMMENT '範本建立時間',
  `template_updatedate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '範本修改時間',
  `template_create_userid` int(9) NOT NULL,
  `template_update_userid` int(11) NOT NULL,
  PRIMARY KEY (`template_id`)
) ENGINE=InnoDB AUTO_INCREMENT=78 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for edm_test
-- ----------------------------
DROP TABLE IF EXISTS `edm_test`;
CREATE TABLE `edm_test` (
  `email_id` int(9) unsigned NOT NULL DEFAULT '0',
  `test_username` varchar(255) NOT NULL,
  `test_status` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `test_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `test_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`email_id`),
  CONSTRAINT `fk_edm_test_eid` FOREIGN KEY (`email_id`) REFERENCES `edm_email` (`email_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for edm_trace
-- ----------------------------
DROP TABLE IF EXISTS `edm_trace`;
CREATE TABLE `edm_trace` (
  `log_id` int(9) NOT NULL COMMENT '寄送記錄代碼',
  `content_id` int(9) NOT NULL COMMENT 'EDM代碼',
  `email_id` int(9) NOT NULL COMMENT '電子郵件信箱代碼',
  `send_date` datetime DEFAULT NULL COMMENT '寄信時間',
  `first_traceback` datetime DEFAULT NULL COMMENT '該電子郵件第一次開信時間',
  `last_traceback` datetime DEFAULT NULL COMMENT '該電子郵件最近一次開信時間',
  `count` int(9) DEFAULT '0' COMMENT '該電子郵件總開信次數',
  `success` int(1) NOT NULL DEFAULT '0' COMMENT '該郵件寄送是否成功,1成功 0失敗 -1 未寄送',
  PRIMARY KEY (`log_id`,`content_id`,`email_id`),
  KEY `f_esl_content_id` (`content_id`),
  KEY `f_ete_email_id` (`email_id`),
  KEY `log_id` (`log_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for edm_trace_email
-- ----------------------------
DROP TABLE IF EXISTS `edm_trace_email`;
CREATE TABLE `edm_trace_email` (
  `email_id` int(9) NOT NULL AUTO_INCREMENT COMMENT '電子郵件信箱代碼',
  `email` varchar(255) NOT NULL COMMENT '電子郵件信箱',
  `name` varchar(255) DEFAULT NULL COMMENT '顯示名稱',
  PRIMARY KEY (`email_id`)
) ENGINE=InnoDB AUTO_INCREMENT=331 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for edm_trace_log
-- ----------------------------
DROP TABLE IF EXISTS `edm_trace_log`;
CREATE TABLE `edm_trace_log` (
  `log_id` int(9) NOT NULL,
  `content_id` int(9) NOT NULL,
  `email_id` int(9) NOT NULL,
  `trace_day` datetime NOT NULL,
  `trace_count` int(10) NOT NULL,
  `trace_createdate` datetime NOT NULL,
  `trace_updatedate` datetime NOT NULL,
  PRIMARY KEY (`log_id`,`content_id`,`email_id`,`trace_day`),
  KEY `fk_content_id` (`content_id`),
  KEY `fk_email_id` (`email_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for element_detail
-- ----------------------------
DROP TABLE IF EXISTS `element_detail`;
CREATE TABLE `element_detail` (
  `element_id` int(9) NOT NULL AUTO_INCREMENT,
  `packet_id` int(9) NOT NULL,
  `category_id` int(9) unsigned DEFAULT NULL,
  `category_name` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `element_content` varchar(5000) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `element_img_big` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT '' COMMENT '元素圖(大) 只有元素類型為圖片或商品時，才需要出現這欄位',
  `element_name` varchar(100) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `element_link_url` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `element_link_mode` int(2) NOT NULL DEFAULT '1',
  `element_sort` smallint(4) NOT NULL DEFAULT '0',
  `element_status` tinyint(2) NOT NULL DEFAULT '1',
  `element_start` datetime NOT NULL,
  `element_end` datetime NOT NULL,
  `element_createdate` datetime NOT NULL,
  `element_updatedate` datetime DEFAULT NULL,
  `create_userid` int(10) NOT NULL DEFAULT '0',
  `update_userid` int(10) NOT NULL DEFAULT '0',
  `element_remark` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`element_id`),
  KEY `element_id` (`element_id`) USING BTREE,
  KEY `packet_id` (`packet_id`) USING BTREE,
  KEY `category_id` (`category_id`) USING BTREE,
  KEY `element_start` (`element_start`) USING BTREE,
  KEY `element_end` (`element_end`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=3015 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for element_map
-- ----------------------------
DROP TABLE IF EXISTS `element_map`;
CREATE TABLE `element_map` (
  `map_id` int(9) NOT NULL AUTO_INCREMENT,
  `site_id` int(9) NOT NULL DEFAULT '0',
  `page_id` int(9) NOT NULL DEFAULT '0',
  `area_id` int(9) NOT NULL DEFAULT '0',
  `packet_id` int(9) NOT NULL DEFAULT '0',
  `sort` smallint(4) NOT NULL,
  `create_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `create_user_id` int(9) NOT NULL DEFAULT '0',
  `update_date` datetime NOT NULL,
  `update_user_id` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`map_id`),
  KEY `map_id` (`map_id`),
  KEY `site_id` (`site_id`),
  KEY `page_id` (`page_id`),
  KEY `area_id` (`area_id`),
  KEY `packet_id` (`packet_id`)
) ENGINE=InnoDB AUTO_INCREMENT=106 DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for email_block_list
-- ----------------------------
DROP TABLE IF EXISTS `email_block_list`;
CREATE TABLE `email_block_list` (
  `email_address` varchar(255) NOT NULL COMMENT '拒絕收信的電子郵件地址',
  `block_reason` varchar(255) DEFAULT NULL COMMENT '拒絕原因',
  `block_createdate` datetime NOT NULL COMMENT '拒絕記錄建立的時間',
  `block_updatedate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '拒絕記錄的修改時間',
  `block_create_userid` int(9) NOT NULL COMMENT '拒絕記錄的建立者代碼',
  `block_update_userid` int(11) DEFAULT NULL COMMENT '拒絕記錄的修改者代碼',
  PRIMARY KEY (`email_address`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for email_block_log
-- ----------------------------
DROP TABLE IF EXISTS `email_block_log`;
CREATE TABLE `email_block_log` (
  `block_id` int(9) NOT NULL AUTO_INCREMENT COMMENT '記錄代碼',
  `email_address` varchar(255) NOT NULL COMMENT '拒絕收信的電子郵件',
  `block_start` datetime NOT NULL COMMENT '拒絕收信的開始時間，這個欄位的值從email_block_list的block_start',
  `block_end` datetime NOT NULL COMMENT '拒絕收件的結束時間。當email_block_list的資料要移除時，要將該筆記錄移到email_block_log，block_end就是記錄當時的時間。',
  `block_reason` varchar(255) NOT NULL COMMENT '從email_block_list的block_reason寫過來',
  `unblock_reason` varchar(255) NOT NULL COMMENT 'email_block_log會有記錄就是解除block，所以必須記下解除的原因',
  `block_create_userid` int(9) NOT NULL COMMENT '從email_block_list的block_create_user_id寫過來',
  `unblock_create_userid` int(9) NOT NULL COMMENT '解除封鎖的使用者代碼',
  PRIMARY KEY (`block_id`)
) ENGINE=InnoDB AUTO_INCREMENT=33 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for email_group
-- ----------------------------
DROP TABLE IF EXISTS `email_group`;
CREATE TABLE `email_group` (
  `group_id` int(9) NOT NULL AUTO_INCREMENT COMMENT '名單群組代碼',
  `group_name` varchar(255) NOT NULL COMMENT '群組名稱',
  `group_createdate` datetime NOT NULL COMMENT '群組建立時間',
  `group_updatedate` datetime NOT NULL COMMENT '群組更新時間',
  `group_create_userid` int(10) NOT NULL COMMENT '群組建立者代碼',
  `group_update_userid` int(10) NOT NULL COMMENT '群組更新者代碼',
  PRIMARY KEY (`group_id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for email_list
-- ----------------------------
DROP TABLE IF EXISTS `email_list`;
CREATE TABLE `email_list` (
  `group_id` int(9) NOT NULL COMMENT '群組代碼',
  `email_address` varchar(255) NOT NULL COMMENT '電子信箱地址',
  `name` varchar(255) DEFAULT NULL COMMENT '收件人名稱',
  PRIMARY KEY (`group_id`,`email_address`),
  CONSTRAINT `f_eg_group_id` FOREIGN KEY (`group_id`) REFERENCES `email_group` (`group_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for ems_actual
-- ----------------------------
DROP TABLE IF EXISTS `ems_actual`;
CREATE TABLE `ems_actual` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT,
  `department_code` varchar(50) NOT NULL COMMENT '部門編號',
  `year` int(5) NOT NULL DEFAULT '2000' COMMENT '年',
  `month` int(4) NOT NULL DEFAULT '1' COMMENT '月',
  `day` int(4) NOT NULL,
  `type` int(4) NOT NULL DEFAULT '1' COMMENT '1為默認排程生成 2為人工keyin',
  `cost_sum` int(11) NOT NULL DEFAULT '0' COMMENT '成本',
  `order_count` int(11) NOT NULL DEFAULT '0' COMMENT '訂單筆數',
  `amount_sum` int(11) NOT NULL DEFAULT '0' COMMENT '累計實績',
  `status` int(11) NOT NULL DEFAULT '1' COMMENT '狀態 1正常 0表示刪除',
  `iskeyin` int(11) NOT NULL DEFAULT '1' COMMENT '1表示已經keyin 2表示系統代替生成',
  `create_time` datetime NOT NULL COMMENT '創建時間',
  `create_user` int(11) NOT NULL COMMENT '創建人',
  PRIMARY KEY (`row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=29940 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for ems_dep_relation
-- ----------------------------
DROP TABLE IF EXISTS `ems_dep_relation`;
CREATE TABLE `ems_dep_relation` (
  `relation_id` int(9) NOT NULL AUTO_INCREMENT,
  `relation_type` int(4) NOT NULL DEFAULT '1' COMMENT '公關單類型 1為公關單 2為報廢單',
  `relation_order_count` int(9) NOT NULL DEFAULT '0' COMMENT '訂單筆數',
  `relation_order_cost` int(9) NOT NULL DEFAULT '0' COMMENT '訂單成本',
  `relation_dep` int(4) NOT NULL COMMENT '部門編號',
  `update_time` datetime DEFAULT NULL COMMENT '更新時間',
  `create_time` datetime DEFAULT NULL COMMENT '創建時間',
  `relation_create_type` int(4) DEFAULT '1' COMMENT '數據創建類型 1為系統自動生成 2為人工keyin',
  `create_user` int(9) DEFAULT NULL COMMENT '創建用戶id',
  `update_user` int(9) DEFAULT NULL COMMENT '更新用戶id',
  `relation_year` int(4) NOT NULL,
  `relation_month` int(4) NOT NULL,
  `relation_day` int(4) NOT NULL,
  PRIMARY KEY (`relation_id`)
) ENGINE=InnoDB AUTO_INCREMENT=89554 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for ems_email_record
-- ----------------------------
DROP TABLE IF EXISTS `ems_email_record`;
CREATE TABLE `ems_email_record` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT,
  `mail_title` varchar(100) NOT NULL,
  `mail_body` text NOT NULL,
  `create_user` int(11) NOT NULL,
  `create_time` datetime NOT NULL,
  `sendfrom` varchar(100) NOT NULL,
  `mailto` varchar(500) NOT NULL,
  PRIMARY KEY (`row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=248 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for ems_goal
-- ----------------------------
DROP TABLE IF EXISTS `ems_goal`;
CREATE TABLE `ems_goal` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `department_code` varchar(50) NOT NULL COMMENT '部門編號',
  `year` int(6) NOT NULL DEFAULT '2000' COMMENT '年',
  `month` int(6) NOT NULL DEFAULT '1' COMMENT '月',
  `goal_amount` int(11) NOT NULL DEFAULT '0' COMMENT '目標金額',
  `status` int(4) NOT NULL DEFAULT '1' COMMENT '狀態',
  `create_user` int(11) NOT NULL COMMENT '創建人',
  `create_time` datetime NOT NULL COMMENT '創建時間',
  PRIMARY KEY (`row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=65 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for epaper_content
-- ----------------------------
DROP TABLE IF EXISTS `epaper_content`;
CREATE TABLE `epaper_content` (
  `epaper_id` int(9) unsigned NOT NULL DEFAULT '0',
  `user_id` smallint(4) unsigned NOT NULL,
  `epaper_title` varchar(255) NOT NULL DEFAULT '' COMMENT '電子報標題',
  `epaper_short_title` varchar(50) NOT NULL COMMENT '簡短標題',
  `epaper_content` text NOT NULL COMMENT '活動頁內容',
  `epaper_sort` smallint(4) unsigned NOT NULL DEFAULT '0',
  `epaper_status` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `epaper_size` varchar(6) NOT NULL DEFAULT '725px' COMMENT '版面大小',
  `epaper_show_start` int(10) unsigned NOT NULL DEFAULT '0',
  `epaper_show_end` int(10) unsigned NOT NULL DEFAULT '0',
  `fb_description` varchar(50) NOT NULL DEFAULT '',
  `epaper_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `epaper_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `epaper_ipfrom` varchar(40) NOT NULL DEFAULT '',
  `type` tinyint(1) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`epaper_id`),
  KEY `ix_epaper_content_uid` (`user_id`),
  KEY `ix_epaper_content_status` (`epaper_status`),
  KEY `ix_epaper_content_start` (`epaper_show_start`),
  KEY `ix_epaper_content_end` (`epaper_show_end`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for epaper_email
-- ----------------------------
DROP TABLE IF EXISTS `epaper_email`;
CREATE TABLE `epaper_email` (
  `email_id` int(9) unsigned NOT NULL,
  `email_address` varchar(100) NOT NULL DEFAULT '',
  `email_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `email_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `email_ipfrom` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`email_id`),
  UNIQUE KEY `uk_epaper_email_as` (`email_address`),
  KEY `ix_epaper_email_ce` (`email_createdate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for epaper_email_edm
-- ----------------------------
DROP TABLE IF EXISTS `epaper_email_edm`;
CREATE TABLE `epaper_email_edm` (
  `email_id` int(9) unsigned NOT NULL,
  `edm_id` tinyint(2) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`email_id`,`edm_id`),
  KEY `ix_epaper_email_edm_eid` (`edm_id`),
  CONSTRAINT `fk_epaper_email_edm_eid` FOREIGN KEY (`email_id`) REFERENCES `epaper_email` (`email_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for epaper_log
-- ----------------------------
DROP TABLE IF EXISTS `epaper_log`;
CREATE TABLE `epaper_log` (
  `log_id` bigint(19) unsigned NOT NULL,
  `epaper_id` int(9) unsigned NOT NULL,
  `user_id` smallint(4) unsigned NOT NULL,
  `log_description` varchar(255) DEFAULT NULL,
  `log_ipfrom` varchar(40) NOT NULL,
  `log_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`log_id`),
  KEY `ix_epaper_log_nid` (`epaper_id`),
  KEY `ix_epaper_log_uid` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for erp_dd
-- ----------------------------
DROP TABLE IF EXISTS `erp_dd`;
CREATE TABLE `erp_dd` (
  `rid` int(11) NOT NULL AUTO_INCREMENT,
  `erp_dm_rid` int(11) NOT NULL,
  `erp_od_rid` int(11) NOT NULL,
  PRIMARY KEY (`rid`)
) ENGINE=InnoDB AUTO_INCREMENT=155041 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for erp_dm
-- ----------------------------
DROP TABLE IF EXISTS `erp_dm`;
CREATE TABLE `erp_dm` (
  `rid` int(11) NOT NULL AUTO_INCREMENT,
  `deliver_id` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `tax_type` tinyint(2) NOT NULL DEFAULT '0',
  `tax_ratio` float NOT NULL,
  `erp_om_rid` int(11) NOT NULL,
  `om_id` int(11) NOT NULL,
  `delivery_name` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL,
  `delivery_address` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `delivery_store` int(9) NOT NULL,
  `note` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `dispatch_type` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `dispatch_no` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `delivery_date` date DEFAULT NULL,
  `estimated_dispatch_date` date DEFAULT NULL,
  `arrival_period` tinyint(2) NOT NULL,
  `logtime` datetime NOT NULL,
  `erp_log_rid` int(11) NOT NULL,
  PRIMARY KEY (`rid`)
) ENGINE=InnoDB AUTO_INCREMENT=67249 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for erp_goods
-- ----------------------------
DROP TABLE IF EXISTS `erp_goods`;
CREATE TABLE `erp_goods` (
  `rid` int(11) NOT NULL AUTO_INCREMENT,
  `erp_id` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `logtime` datetime NOT NULL,
  `erp_log_rid` int(11) NOT NULL,
  PRIMARY KEY (`rid`)
) ENGINE=InnoDB AUTO_INCREMENT=11823 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for erp_log
-- ----------------------------
DROP TABLE IF EXISTS `erp_log`;
CREATE TABLE `erp_log` (
  `rid` int(11) NOT NULL AUTO_INCREMENT,
  `log_type` tinyint(2) NOT NULL,
  `filename` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `logtime` datetime NOT NULL,
  `status` tinyint(2) NOT NULL DEFAULT '0',
  PRIMARY KEY (`rid`)
) ENGINE=InnoDB AUTO_INCREMENT=20127 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for erp_od
-- ----------------------------
DROP TABLE IF EXISTS `erp_od`;
CREATE TABLE `erp_od` (
  `rid` int(11) NOT NULL AUTO_INCREMENT,
  `erp_om_rid` int(11) NOT NULL DEFAULT '0',
  `erp_id` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `om_id` int(11) NOT NULL,
  `detail_id` int(9) NOT NULL DEFAULT '0',
  `item_id` int(9) NOT NULL DEFAULT '0',
  `tax_type` tinyint(2) NOT NULL DEFAULT '0',
  `product_freight_set` tinyint(2) NOT NULL,
  `product_mode` tinyint(2) NOT NULL,
  `product_name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `product_spec_name` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `cost` float NOT NULL DEFAULT '0',
  `paid_price` float NOT NULL DEFAULT '0',
  `deduct_happygo_money` int(9) NOT NULL DEFAULT '0',
  `buy_num` int(9) NOT NULL DEFAULT '0',
  `bag_check_money` int(9) NOT NULL DEFAULT '0',
  `combined_mode` tinyint(2) NOT NULL DEFAULT '0',
  `item_mode` tinyint(2) NOT NULL,
  `pack_id` int(9) NOT NULL DEFAULT '0',
  `item_serial` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`rid`)
) ENGINE=InnoDB AUTO_INCREMENT=205741 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for erp_om
-- ----------------------------
DROP TABLE IF EXISTS `erp_om`;
CREATE TABLE `erp_om` (
  `rid` int(11) NOT NULL AUTO_INCREMENT,
  `order_id` int(9) NOT NULL,
  `tax_type` tinyint(2) NOT NULL DEFAULT '0',
  `om_id` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `channel_erp_id` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `deduct_happygo` int(9) NOT NULL DEFAULT '0',
  `deduct_card_bonus` int(9) NOT NULL DEFAULT '0',
  `order_freight_normal` int(9) NOT NULL DEFAULT '0',
  `order_freight_low` int(9) NOT NULL DEFAULT '0',
  `order_amount` int(9) NOT NULL DEFAULT '0',
  `delivery_name` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL,
  `delivery_mobile` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `delivery_phone` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `delivery_zip` mediumint(7) NOT NULL,
  `delivery_address` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `order_createdate` date NOT NULL,
  `order_date_pay` date NOT NULL,
  `logtime` datetime NOT NULL,
  `erp_log_rid` int(11) NOT NULL,
  `erp_payment` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `delivery_store` int(9) NOT NULL,
  `user_id` int(11) NOT NULL DEFAULT '0',
  `user_name` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL,
  `note_order` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`rid`)
) ENGINE=InnoDB AUTO_INCREMENT=71905 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for erp_op
-- ----------------------------
DROP TABLE IF EXISTS `erp_op`;
CREATE TABLE `erp_op` (
  `rid` int(11) NOT NULL AUTO_INCREMENT,
  `om_id` int(11) NOT NULL,
  `om_rid` int(11) NOT NULL,
  `op_payment` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `payment_amount` int(11) NOT NULL DEFAULT '0',
  `payment_serial` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`rid`)
) ENGINE=InnoDB AUTO_INCREMENT=79057 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for event
-- ----------------------------
DROP TABLE IF EXISTS `event`;
CREATE TABLE `event` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `calendarId` int(9) DEFAULT NULL COMMENT '控件id ',
  `title` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '事件標題',
  `notes` varchar(45) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '具體事件的內容',
  `startDate` varchar(45) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '事件開始時間',
  `endDate` varchar(45) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '事件結束時間',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for event_brand_history
-- ----------------------------
DROP TABLE IF EXISTS `event_brand_history`;
CREATE TABLE `event_brand_history` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `brand_id` int(9) unsigned NOT NULL DEFAULT '0',
  `event_cost` int(2) unsigned NOT NULL DEFAULT '0',
  `event_money` int(2) unsigned NOT NULL DEFAULT '0',
  `event_statr_time` int(10) unsigned NOT NULL DEFAULT '0',
  `event_end_time` int(10) unsigned NOT NULL DEFAULT '0',
  `creator` int(9) unsigned NOT NULL DEFAULT '0',
  `createdate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=46 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for event_category
-- ----------------------------
DROP TABLE IF EXISTS `event_category`;
CREATE TABLE `event_category` (
  `category_id` int(11) NOT NULL AUTO_INCREMENT,
  `category_name` varchar(255) NOT NULL,
  `description` text NOT NULL,
  `activity` int(11) NOT NULL,
  `created_on` datetime NOT NULL,
  `updated_on` datetime NOT NULL,
  PRIMARY KEY (`category_id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for event_chinatrust
-- ----------------------------
DROP TABLE IF EXISTS `event_chinatrust`;
CREATE TABLE `event_chinatrust` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '活動編號，自增',
  `event_type` varchar(10) NOT NULL COMMENT '活動類型，比如:PB P1等',
  `event_id` varchar(10) DEFAULT NULL COMMENT '8碼',
  `event_name` varchar(200) NOT NULL COMMENT '活動名稱',
  `event_desc` varchar(1000) DEFAULT NULL COMMENT '活動描述',
  `event_banner` varchar(100) NOT NULL COMMENT '活動banner',
  `event_start_time` datetime NOT NULL COMMENT '活動開始時間',
  `event_end_time` datetime NOT NULL COMMENT '活動結束時間',
  `event_active` int(11) NOT NULL DEFAULT '0' COMMENT '活動狀態1啟用0禁用，默認禁用',
  `event_create_user` int(11) NOT NULL COMMENT '創建人',
  `event_update_user` int(11) NOT NULL COMMENT '修改人',
  `event_create_time` datetime NOT NULL COMMENT '活動創建時間',
  `event_update_time` datetime NOT NULL COMMENT '活動修改時間',
  `user_register_time` datetime NOT NULL COMMENT '會員註冊時間',
  PRIMARY KEY (`row_id`),
  KEY `event_id` (`event_id`) USING BTREE,
  KEY `row_id` (`row_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=40 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for event_chinatrust_bag
-- ----------------------------
DROP TABLE IF EXISTS `event_chinatrust_bag`;
CREATE TABLE `event_chinatrust_bag` (
  `bag_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '包編號,自增',
  `bag_name` varchar(10) NOT NULL COMMENT '包-名稱',
  `bag_desc` varchar(200) DEFAULT NULL COMMENT '包-描述',
  `bag_banner` varchar(200) DEFAULT NULL COMMENT '包-banner',
  `bag_start_time` datetime NOT NULL COMMENT '包開始時間',
  `bag_end_time` datetime NOT NULL COMMENT '包結束時間',
  `bag_active` int(11) NOT NULL DEFAULT '0' COMMENT '包狀態1啟用0禁用',
  `bag_create_user` int(11) NOT NULL COMMENT '創建人',
  `bag_update_user` int(11) NOT NULL COMMENT '修改人',
  `bag_create_time` datetime NOT NULL COMMENT '創建時間',
  `bag_update_time` datetime NOT NULL COMMENT '修改時間',
  `bag_show_start_time` datetime DEFAULT NULL COMMENT '顯示時間開始',
  `bag_show_end_time` datetime DEFAULT NULL COMMENT '顯示時間結束',
  `event_id` varchar(10) DEFAULT NULL COMMENT '活動編號-event_chinatrust中的event_id',
  `product_number` int(10) DEFAULT NULL COMMENT '商品數量',
  PRIMARY KEY (`bag_id`),
  KEY `bag_id` (`bag_id`) USING BTREE,
  KEY `event_id` (`event_id`),
  CONSTRAINT `event_chinatrust_bag_ibfk_1` FOREIGN KEY (`event_id`) REFERENCES `event_chinatrust` (`event_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=118 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for event_chinatrust_bag_map
-- ----------------------------
DROP TABLE IF EXISTS `event_chinatrust_bag_map`;
CREATE TABLE `event_chinatrust_bag_map` (
  `map_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '區域包編號-自增',
  `bag_id` int(11) NOT NULL COMMENT '區域包編號',
  `product_id` int(11) unsigned NOT NULL COMMENT '商品編號',
  `linkurl` varchar(225) DEFAULT NULL COMMENT '商品鏈接',
  `product_forbid_banner` varchar(100) DEFAULT NULL COMMENT '商品失效',
  `product_active_banner` varchar(100) DEFAULT NULL COMMENT '商品正常圖片',
  `map_active` int(10) NOT NULL DEFAULT '1' COMMENT '商品關係狀態',
  `map_sort` int(10) NOT NULL DEFAULT '0' COMMENT '商品排序',
  `ad_product_id` varchar(200) DEFAULT NULL COMMENT '廣告商品圖片',
  `product_desc` varchar(400) NOT NULL DEFAULT '' COMMENT '商品描述',
  PRIMARY KEY (`map_id`),
  KEY `map_id` (`map_id`) USING BTREE,
  KEY `bag_id` (`bag_id`),
  CONSTRAINT `event_chinatrust_bag_map_ibfk_1` FOREIGN KEY (`bag_id`) REFERENCES `event_chinatrust_bag` (`bag_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=88 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for event_collect
-- ----------------------------
DROP TABLE IF EXISTS `event_collect`;
CREATE TABLE `event_collect` (
  `event_collect_id` int(9) unsigned NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `event_id` varchar(10) NOT NULL DEFAULT '0' COMMENT '活動ID',
  `event_collect_time` datetime NOT NULL COMMENT '收藏時間（yyyy-mm-dd hh:mm:ss）',
  `user_id` int(9) NOT NULL COMMENT '會員ID',
  `event_collect_status` tinyint(3) NOT NULL DEFAULT '1' COMMENT '狀態（1:顯示 2:刪除不顯示）',
  PRIMARY KEY (`event_collect_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for event_online
-- ----------------------------
DROP TABLE IF EXISTS `event_online`;
CREATE TABLE `event_online` (
  `event_id` int(11) NOT NULL AUTO_INCREMENT,
  `category_id` int(11) NOT NULL,
  `prize_id` int(11) NOT NULL,
  `article` varchar(50) DEFAULT NULL,
  `quantity` int(11) NOT NULL DEFAULT '1',
  `order_id` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  `user_ipfrom` varchar(15) DEFAULT NULL,
  `created_on` datetime NOT NULL,
  PRIMARY KEY (`event_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4350 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for event_type
-- ----------------------------
DROP TABLE IF EXISTS `event_type`;
CREATE TABLE `event_type` (
  `et_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水編號',
  `et_name` varchar(50) DEFAULT NULL COMMENT '活動類型DD->每天，WW->每週，MM->每月',
  `et_date_parameter` varchar(50) DEFAULT NULL COMMENT '時間參數',
  `et_starttime` varchar(50) DEFAULT NULL COMMENT '活動一天開始時間',
  `et_endtime` varchar(50) DEFAULT NULL COMMENT '活動一天結束時間',
  PRIMARY KEY (`et_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for fortune_list
-- ----------------------------
DROP TABLE IF EXISTS `fortune_list`;
CREATE TABLE `fortune_list` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `product_id` int(9) unsigned NOT NULL,
  `user_id` int(9) unsigned NOT NULL,
  `user_name` varchar(255) NOT NULL,
  `user_gender` tinyint(1) unsigned NOT NULL,
  `user_mobile` varchar(50) NOT NULL,
  `order_id` int(9) unsigned NOT NULL DEFAULT '0',
  `user_zip` tinyint(1) unsigned NOT NULL,
  `user_address` varchar(255) NOT NULL,
  `lot_createdate` int(10) unsigned NOT NULL COMMENT '報名時間',
  `comment` text COMMENT '備註',
  `lot_status` int(1) unsigned NOT NULL DEFAULT '0' COMMENT '狀態: 0.待審, 1.已中獎, 2.已出貨',
  `lot_num` varchar(20) DEFAULT NULL COMMENT '領獎序號',
  `atm_num` varchar(20) DEFAULT NULL COMMENT '運費ATM繳款序號',
  `mgr_comment` text COMMENT '管理員備註',
  PRIMARY KEY (`id`),
  UNIQUE KEY `product_user` (`product_id`,`user_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8 COMMENT='抽獎名單';

-- ----------------------------
-- Table structure for function_history
-- ----------------------------
DROP TABLE IF EXISTS `function_history`;
CREATE TABLE `function_history` (
  `row_id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `function_id` int(11) DEFAULT NULL COMMENT '功能Id',
  `user_id` int(11) DEFAULT NULL COMMENT '用戶id',
  `operate_time` datetime DEFAULT NULL COMMENT '操作時間',
  PRIMARY KEY (`row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=56209 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for gigade_keywords
-- ----------------------------
DROP TABLE IF EXISTS `gigade_keywords`;
CREATE TABLE `gigade_keywords` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT,
  `keywords` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=98 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for group_auth_map
-- ----------------------------
DROP TABLE IF EXISTS `group_auth_map`;
CREATE TABLE `group_auth_map` (
  `content_id` int(9) NOT NULL AUTO_INCREMENT,
  `group_id` int(9) DEFAULT NULL,
  `table_name` varchar(100) DEFAULT NULL,
  `column_name` varchar(100) DEFAULT NULL,
  `value` varchar(100) DEFAULT NULL,
  `status` int(4) DEFAULT '1',
  `create_user_id` varchar(50) DEFAULT NULL,
  `create_date` datetime DEFAULT NULL,
  `table_alias_name` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`content_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for group_committe_contact
-- ----------------------------
DROP TABLE IF EXISTS `group_committe_contact`;
CREATE TABLE `group_committe_contact` (
  `gcc_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `group_id` int(9) NOT NULL COMMENT '公司編號關聯vip_user_group',
  `gcc_chairman` varchar(50) NOT NULL COMMENT '福委姓名',
  `gcc_phone` varchar(50) NOT NULL COMMENT '福委電話',
  `gcc_mail` varchar(100) NOT NULL COMMENT '福委mail',
  `k_user` int(11) DEFAULT NULL COMMENT '創建人',
  `k_date` datetime DEFAULT NULL COMMENT '創建時間',
  `m_user` int(11) DEFAULT NULL COMMENT '修改人',
  `m_date` datetime DEFAULT NULL COMMENT '修改時間',
  PRIMARY KEY (`gcc_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for hashmaps
-- ----------------------------
DROP TABLE IF EXISTS `hashmaps`;
CREATE TABLE `hashmaps` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `event` int(11) NOT NULL,
  `key` varchar(255) NOT NULL,
  `value` varchar(255) NOT NULL,
  `created` datetime NOT NULL,
  `modified` datetime NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `key` (`event`,`key`),
  UNIQUE KEY `value` (`event`,`value`)
) ENGINE=InnoDB AUTO_INCREMENT=2385 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for health_members
-- ----------------------------
DROP TABLE IF EXISTS `health_members`;
CREATE TABLE `health_members` (
  `health_id` int(11) NOT NULL AUTO_INCREMENT,
  `member_id` int(11) NOT NULL,
  `user_id` int(11) DEFAULT NULL,
  `user_status` int(11) NOT NULL DEFAULT '0',
  `recorded_on` datetime DEFAULT NULL,
  `created_on` datetime DEFAULT NULL,
  `update_on` datetime DEFAULT NULL,
  PRIMARY KEY (`health_id`)
) ENGINE=InnoDB AUTO_INCREMENT=15158 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for hg_accumulate
-- ----------------------------
DROP TABLE IF EXISTS `hg_accumulate`;
CREATE TABLE `hg_accumulate` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `head` varchar(1) CHARACTER SET utf8 NOT NULL DEFAULT 'B',
  `card_num` varchar(32) CHARACTER SET utf8 NOT NULL DEFAULT '',
  `chk_card` varchar(4) CHARACTER SET utf8 NOT NULL DEFAULT '',
  `enc_idno` varchar(32) CHARACTER SET utf8 NOT NULL DEFAULT '',
  `chk_sum` varchar(4) CHARACTER SET utf8 NOT NULL DEFAULT '',
  `transaction_date` datetime NOT NULL,
  `merchant` varchar(20) CHARACTER SET utf8 NOT NULL DEFAULT '',
  `terminal` varchar(20) CHARACTER SET utf8 NOT NULL DEFAULT '',
  `order_amount` int(10) unsigned NOT NULL,
  `set_point` int(10) unsigned NOT NULL,
  `point_amount` int(10) unsigned NOT NULL,
  `category` varchar(10) CHARACTER SET utf8 NOT NULL DEFAULT '',
  `nbu` varchar(5) CHARACTER SET utf8 NOT NULL DEFAULT '',
  `order_id` int(10) unsigned NOT NULL DEFAULT '0',
  `rrn` varchar(15) CHARACTER SET utf8 NOT NULL DEFAULT '',
  `code` varchar(5) CHARACTER SET utf8 NOT NULL DEFAULT '',
  `note` varchar(50) CHARACTER SET utf8 NOT NULL DEFAULT '',
  `campaign` int(1) unsigned NOT NULL DEFAULT '0',
  `wallet` varchar(10) CHARACTER SET utf8 NOT NULL DEFAULT '',
  `import_time` datetime DEFAULT NULL,
  `error_type` varchar(5) NOT NULL DEFAULT '',
  `status` int(1) unsigned NOT NULL DEFAULT '0',
  `modified` datetime DEFAULT NULL,
  `billing_checked` tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '對帳狀態 0 無 :1 :有',
  PRIMARY KEY (`id`),
  KEY `order_id` (`order_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3626 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for hg_accumulate_refund
-- ----------------------------
DROP TABLE IF EXISTS `hg_accumulate_refund`;
CREATE TABLE `hg_accumulate_refund` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `head` varchar(1) NOT NULL DEFAULT 'B',
  `card_num` varchar(32) NOT NULL DEFAULT '',
  `chk_card` varchar(4) NOT NULL DEFAULT '',
  `enc_idno` varchar(32) NOT NULL DEFAULT '',
  `chk_sum` varchar(4) NOT NULL DEFAULT '',
  `transaction_date` datetime NOT NULL,
  `merchant` varchar(20) NOT NULL DEFAULT '',
  `terminal` varchar(20) NOT NULL DEFAULT '',
  `refund_point` int(10) unsigned NOT NULL,
  `category` varchar(10) NOT NULL DEFAULT '',
  `note` varchar(50) NOT NULL DEFAULT '',
  `wallet` varchar(10) NOT NULL DEFAULT '',
  `order_id` int(10) unsigned NOT NULL DEFAULT '0',
  `import_time` datetime DEFAULT NULL,
  `error_type` varchar(5) NOT NULL DEFAULT '',
  `status` int(1) unsigned NOT NULL DEFAULT '0',
  `modified` datetime DEFAULT NULL,
  `billing_checked` tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '對帳狀態 0 無 :1 :有',
  PRIMARY KEY (`id`),
  KEY `order_id` (`order_id`)
) ENGINE=InnoDB AUTO_INCREMENT=60 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for hg_batch_accumulate
-- ----------------------------
DROP TABLE IF EXISTS `hg_batch_accumulate`;
CREATE TABLE `hg_batch_accumulate` (
  `order_id` int(9) NOT NULL COMMENT '訂單編號',
  `head` char(1) NOT NULL DEFAULT 'B' COMMENT '交易檔頭代碼(B)，英文字母大寫，必填',
  `card_no` varchar(32) DEFAULT NULL COMMENT '加密後的卡號',
  `card_checksum` varchar(4) DEFAULT NULL COMMENT '卡號檢查碼，卡號的第13~16碼',
  `enc_idno` varchar(32) DEFAULT NULL COMMENT 'MPID',
  `checksum` varchar(4) DEFAULT NULL COMMENT 'MPID檢核碼',
  `transaction_time` timestamp NULL DEFAULT NULL COMMENT '交易時間',
  `merchant_pos` varchar(15) DEFAULT NULL COMMENT '特店代號',
  `terminal_pos` varchar(16) DEFAULT NULL COMMENT '端末機代碼',
  `order_amount` int(10) DEFAULT NULL COMMENT '交易金額(不含小數點)，實際支付之交易金額，不含已兌換金額',
  `award_point` int(10) DEFAULT NULL COMMENT '回饋點數',
  `total_award_point` int(10) DEFAULT NULL COMMENT '總加值點數(回饋點數+額外點數)',
  `category_id` varchar(8) DEFAULT NULL COMMENT '部類碼，由DDIM提供',
  `NBU` varchar(4) DEFAULT NULL COMMENT '交易大類(NBU)，請放空白即可',
  `RRN` varchar(12) DEFAULT NULL COMMENT 'RRN(交易序號)',
  `award_code` varchar(4) DEFAULT NULL COMMENT '回饋代號',
  `order_note` varchar(50) DEFAULT NULL COMMENT '中文交易說明(非一般交易時，需特別註明)',
  `is_campaign` int(1) DEFAULT NULL COMMENT '是否計算campaign(1=yes，0=no)',
  `wallet` varchar(6) DEFAULT NULL COMMENT '錢包代號，由DDIM提供',
  `batch_import_time` timestamp NULL DEFAULT NULL COMMENT '匯入時間',
  `batch_error_code` varchar(3) DEFAULT NULL COMMENT '匯入結果代碼',
  `batch_status` int(1) DEFAULT '0' COMMENT '批次處理狀態(0=未處理，1=已處理，2=有錯誤)',
  `create_time` timestamp NULL DEFAULT NULL COMMENT '資料建立時間',
  `modified_time` timestamp NULL DEFAULT NULL COMMENT '資料更新時間',
  `billing_checked` int(1) DEFAULT '0' COMMENT '會計對帳匯出狀態(0=未匯出，1=已匯出)',
  PRIMARY KEY (`order_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for hg_batch_accumulate_refund
-- ----------------------------
DROP TABLE IF EXISTS `hg_batch_accumulate_refund`;
CREATE TABLE `hg_batch_accumulate_refund` (
  `order_id` int(9) unsigned NOT NULL COMMENT '訂單編號',
  `head` char(1) NOT NULL DEFAULT 'B' COMMENT '交易檔頭代號(B)，英文字母大寫，必填',
  `card_no` varchar(32) DEFAULT NULL COMMENT '加密後的卡號',
  `card_checksum` varchar(4) DEFAULT NULL COMMENT '卡號檢查碼，卡號的第13~16碼',
  `enc_idno` varchar(32) NOT NULL COMMENT 'MPID',
  `checksum` varchar(4) NOT NULL COMMENT 'MPID檢核碼',
  `transaction_time` datetime NOT NULL COMMENT '交易時間',
  `merchant_pos` varchar(15) NOT NULL COMMENT '特店代號',
  `terminal_pos` varchar(16) NOT NULL COMMENT '端末機代號',
  `refund_point` int(10) NOT NULL COMMENT '退貨點數',
  `category_id` varchar(8) NOT NULL COMMENT '部類碼',
  `order_note` varchar(50) NOT NULL COMMENT '中文交易說明',
  `wallet` varchar(6) NOT NULL COMMENT '錢包代號',
  `batch_import_time` timestamp NULL DEFAULT NULL COMMENT '批次匯入時間',
  `batch_error_code` varchar(3) NOT NULL COMMENT '批次處理結果錯誤代碼',
  `batch_status` int(1) NOT NULL DEFAULT '0' COMMENT '批次處理狀態(0=未處理，1=已處理，2=有錯誤)',
  `created_time` datetime NOT NULL COMMENT '資料建立時間',
  `modified_time` datetime NOT NULL COMMENT '資料修改時間',
  `billing_checked` bit(1) NOT NULL DEFAULT b'0' COMMENT '會計對帳處理狀態(0=未處理，1=已處理)',
  PRIMARY KEY (`order_id`),
  KEY `hg_login.order_id` (`order_id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for hg_batch_deduct_refund
-- ----------------------------
DROP TABLE IF EXISTS `hg_batch_deduct_refund`;
CREATE TABLE `hg_batch_deduct_refund` (
  `order_id` int(9) unsigned NOT NULL COMMENT '訂單編號',
  `head` char(1) NOT NULL DEFAULT 'B' COMMENT '交易檔頭代號(B)，英文字母大寫，必填',
  `card_no` varchar(32) NOT NULL COMMENT '加密後的卡號',
  `card_checksum` varchar(4) NOT NULL COMMENT '卡號檢查碼，卡號的第13~16碼',
  `enc_idno` varchar(32) NOT NULL COMMENT 'MPID',
  `checksum` varchar(4) NOT NULL COMMENT 'MPID檢核碼',
  `transaction_time` datetime NOT NULL COMMENT '交易時間',
  `merchant_pos` varchar(15) NOT NULL COMMENT '特店代號',
  `terminal_pos` varchar(16) NOT NULL COMMENT '端末機代號',
  `category_id` varchar(8) NOT NULL COMMENT '部類碼',
  `refund_point` int(10) NOT NULL COMMENT '退貨點數',
  `order_note` varchar(50) NOT NULL COMMENT '中文交易說明',
  `wallet` varchar(6) NOT NULL COMMENT '錢包代號',
  `batch_import_time` timestamp NULL DEFAULT NULL COMMENT '批次匯入時間',
  `batch_error_code` varchar(3) NOT NULL COMMENT '批次處理結果錯誤代碼',
  `batch_status` int(1) NOT NULL DEFAULT '0' COMMENT '批次處理狀態(0=未處理，1=已處理，2=有錯誤)',
  `created_time` datetime NOT NULL COMMENT '資料建立時間',
  `modified_time` datetime NOT NULL COMMENT '資料修改時間',
  `billing_checked` bit(1) NOT NULL DEFAULT b'0' COMMENT '會計對帳處理狀態(0=未處理，1=已處理)',
  PRIMARY KEY (`order_id`),
  KEY `hg_login.order_id` (`order_id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for hg_deduct
-- ----------------------------
DROP TABLE IF EXISTS `hg_deduct`;
CREATE TABLE `hg_deduct` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `merchant_pos` char(10) NOT NULL,
  `terminal_pos` char(8) NOT NULL,
  `enc_idno` varchar(32) NOT NULL,
  `chk_sum` char(4) NOT NULL,
  `token` varchar(255) NOT NULL,
  `order_id` char(12) NOT NULL,
  `prn` char(12) NOT NULL,
  `date` char(4) NOT NULL,
  `time` char(6) NOT NULL,
  `code` char(4) NOT NULL,
  `message` varchar(255) NOT NULL,
  `created` datetime NOT NULL,
  `modified` datetime NOT NULL,
  `billing_checked` tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '對帳狀態 0 無 :1 :有',
  PRIMARY KEY (`id`),
  KEY `order_id` (`order_id`)
) ENGINE=InnoDB AUTO_INCREMENT=1516 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for hg_deduct_refund
-- ----------------------------
DROP TABLE IF EXISTS `hg_deduct_refund`;
CREATE TABLE `hg_deduct_refund` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `head` varchar(1) NOT NULL DEFAULT 'B',
  `card_num` varchar(32) NOT NULL DEFAULT '',
  `chk_card` varchar(4) NOT NULL DEFAULT '',
  `enc_idno` varchar(32) NOT NULL DEFAULT '',
  `chk_sum` varchar(4) NOT NULL DEFAULT '',
  `transaction_date` datetime NOT NULL,
  `merchant` varchar(20) NOT NULL DEFAULT '',
  `terminal` varchar(20) NOT NULL DEFAULT '',
  `refund_point` int(10) unsigned NOT NULL,
  `category` varchar(10) NOT NULL DEFAULT '',
  `note` varchar(50) NOT NULL DEFAULT '',
  `wallet` varchar(10) NOT NULL DEFAULT '',
  `order_id` int(10) unsigned NOT NULL DEFAULT '0',
  `import_time` datetime DEFAULT NULL,
  `error_type` varchar(5) NOT NULL DEFAULT '',
  `status` int(1) unsigned NOT NULL DEFAULT '0',
  `modified` datetime DEFAULT NULL,
  `billing_checked` tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '對帳狀態 0 無 :1 :有',
  PRIMARY KEY (`id`),
  KEY `order_id` (`order_id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for hg_deduct_reversal
-- ----------------------------
DROP TABLE IF EXISTS `hg_deduct_reversal`;
CREATE TABLE `hg_deduct_reversal` (
  `order_id` int(10) NOT NULL COMMENT '訂單編號',
  `response_code` varchar(255) DEFAULT NULL COMMENT '交易結果代碼',
  `response_message` varchar(255) DEFAULT NULL COMMENT '交易結果訊息',
  `transaction_time` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '交易時間',
  PRIMARY KEY (`order_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for hg_deduct_reverse
-- ----------------------------
DROP TABLE IF EXISTS `hg_deduct_reverse`;
CREATE TABLE `hg_deduct_reverse` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `merchant_pos` char(10) NOT NULL,
  `terminal_pos` char(8) NOT NULL,
  `enc_idno` varchar(32) NOT NULL,
  `chk_sum` char(4) NOT NULL,
  `token` varchar(255) NOT NULL,
  `order_id` char(12) NOT NULL,
  `date` char(4) NOT NULL,
  `time` char(6) NOT NULL,
  `code` char(4) NOT NULL,
  `message` varchar(255) NOT NULL,
  `created` datetime NOT NULL,
  `modified` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `order_id` (`order_id`)
) ENGINE=InnoDB AUTO_INCREMENT=263 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for hg_login
-- ----------------------------
DROP TABLE IF EXISTS `hg_login`;
CREATE TABLE `hg_login` (
  `order_id` int(9) unsigned NOT NULL COMMENT '訂單編號',
  `merchant_pos` varchar(10) NOT NULL COMMENT '特店代號，由DDIM提供',
  `terminal_pos` varchar(8) NOT NULL COMMENT '端末機代號，由DDIM提供',
  `response_code` int(10) NOT NULL COMMENT '交易結果代碼',
  `response_message` varchar(255) NOT NULL COMMENT '交易結果描述',
  `enc_idno` varchar(32) NOT NULL COMMENT 'MPID(BCD格式)',
  `chk_sum` varchar(4) NOT NULL COMMENT '檢核碼',
  `remain_point` int(10) NOT NULL COMMENT '即時點數',
  `token` varchar(255) NOT NULL COMMENT '交易序號',
  `mask_name` varchar(255) NOT NULL COMMENT '例：王Ⅹ大',
  `mask_id` varchar(255) NOT NULL COMMENT '例：A100XXXXXX',
  `transaction_time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '交易時間',
  `createdAt` timestamp NULL DEFAULT NULL COMMENT '創建時間',
  PRIMARY KEY (`order_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for idiff_count_book
-- ----------------------------
DROP TABLE IF EXISTS `idiff_count_book`;
CREATE TABLE `idiff_count_book` (
  `book_id` int(11) NOT NULL AUTO_INCREMENT,
  `cb_jobid` varchar(50) DEFAULT NULL,
  `loc_id` varchar(50) DEFAULT NULL,
  `pro_qty` int(11) DEFAULT NULL,
  `st_qty` int(11) DEFAULT NULL,
  `create_user` int(11) DEFAULT NULL,
  `create_time` datetime DEFAULT NULL,
  `item_id` int(10) unsigned DEFAULT NULL,
  `made_date` date DEFAULT NULL,
  `cde_dt` date DEFAULT NULL,
  PRIMARY KEY (`book_id`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8 COMMENT='即時差異報表';

-- ----------------------------
-- Table structure for iialg
-- ----------------------------
DROP TABLE IF EXISTS `iialg`;
CREATE TABLE `iialg` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `loc_id` varchar(50) NOT NULL COMMENT '料位編號',
  `item_id` int(11) unsigned NOT NULL COMMENT '商品細項編號',
  `iarc_id` varchar(11) NOT NULL COMMENT '庫調原因',
  `qty_o` int(11) NOT NULL COMMENT '原始庫存數量',
  `type` int(4) NOT NULL DEFAULT '1' COMMENT '1 代表正常庫調 2代表盤點庫調',
  `adj_qty` int(11) NOT NULL COMMENT '轉移數量',
  `create_dtim` datetime DEFAULT NULL,
  `create_user` int(9) DEFAULT NULL,
  `doc_no` varchar(50) DEFAULT NULL COMMENT '庫調單編號',
  `po_id` varchar(50) DEFAULT NULL COMMENT '前置單號',
  `made_dt` date DEFAULT NULL,
  `cde_dt` date DEFAULT NULL,
  `c_cde_dt` date DEFAULT NULL,
  `c_made_dt` date DEFAULT NULL,
  `remarks` varchar(255) DEFAULT NULL COMMENT '庫調備註',
  PRIMARY KEY (`row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2755 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for iinvd
-- ----------------------------
DROP TABLE IF EXISTS `iinvd`;
CREATE TABLE `iinvd` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `lic_plt_id` decimal(18,0) NOT NULL COMMENT '棧板號碼，料位中的庫存，都有一組數字編號，一直跟著庫存，直到庫存為零，才從系統中消失',
  `dc_id` int(9) NOT NULL DEFAULT '1' COMMENT '物流中心編號',
  `whse_id` int(9) NOT NULL DEFAULT '1' COMMENT '倉庫編號',
  `po_id` varchar(18) DEFAULT NULL COMMENT '採購單單號',
  `plas_id` int(11) NOT NULL DEFAULT '0' COMMENT '與iplas關聯欄位',
  `prod_qty` int(11) NOT NULL DEFAULT '0' COMMENT '庫存數量，單位為PCS',
  `rcpt_id` int(11) DEFAULT NULL COMMENT '收貨代號，依照請標籤的次數產生的一組編號',
  `lot_no` varchar(14) NOT NULL DEFAULT '1' COMMENT '批號管理時存放批號的欄位',
  `hgt_used` int(9) DEFAULT NULL COMMENT '用掉的料位高度',
  `create_user` int(8) DEFAULT '0' COMMENT '創建人',
  `create_dtim` datetime DEFAULT NULL COMMENT '創建時間',
  `change_user` int(8) DEFAULT '0' COMMENT '修改人',
  `change_dtim` datetime DEFAULT NULL COMMENT '修改時間',
  `cde_dt` date NOT NULL DEFAULT '1990-00-00' COMMENT '有效日期',
  `ista_id` varchar(1) DEFAULT NULL COMMENT '庫存的狀態 H:庫存鎖住    A:正常可以出貨 ',
  `receipt_dtim` datetime DEFAULT NULL COMMENT '收貨時間',
  `stor_ti` int(9) DEFAULT NULL COMMENT '收貨當時的Ti',
  `stor_hi` int(9) DEFAULT NULL COMMENT '收貨當時的Hi',
  `inv_pos_cat` varchar(1) DEFAULT NULL COMMENT 'P或R',
  `plas_loc_id` varchar(8) DEFAULT NULL COMMENT '庫存所在的料位',
  `item_id` int(9) unsigned NOT NULL DEFAULT '0' COMMENT '庫存商品的編號',
  `plas_prdd_id` int(6) DEFAULT NULL COMMENT '庫存商品編號帶動的序號',
  `free` int(4) DEFAULT '0' COMMENT '料位是否被佔用，如果庫存變為0，此欄位自動變為1 表示沒有被佔用，0表示當前料位被佔用',
  `made_date` date DEFAULT NULL COMMENT '製造日期',
  `qity_id` int(2) DEFAULT '0' COMMENT '上鎖錯誤原因',
  `st_qty` int(11) DEFAULT '0' COMMENT '盤點複盤數量',
  PRIMARY KEY (`row_id`),
  KEY `idx_01` (`plas_loc_id`)
) ENGINE=InnoDB AUTO_INCREMENT=11865 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for iinvd_log
-- ----------------------------
DROP TABLE IF EXISTS `iinvd_log`;
CREATE TABLE `iinvd_log` (
  `rowid` int(11) NOT NULL AUTO_INCREMENT,
  `nvd_id` int(11) NOT NULL DEFAULT '0' COMMENT 'iinvd的rowid',
  `from_num` int(11) NOT NULL DEFAULT '0' COMMENT '變價記錄日誌',
  `change_num` int(11) NOT NULL DEFAULT '0' COMMENT '庫存變化數量',
  `create_user` int(11) NOT NULL DEFAULT '0',
  `create_date` datetime NOT NULL,
  PRIMARY KEY (`rowid`)
) ENGINE=InnoDB AUTO_INCREMENT=3538 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for iloc
-- ----------------------------
DROP TABLE IF EXISTS `iloc`;
CREATE TABLE `iloc` (
  `row_id` int(9) NOT NULL AUTO_INCREMENT,
  `dc_id` int(9) NOT NULL COMMENT '物流中心編號',
  `whse_id` int(9) NOT NULL COMMENT '倉庫編號',
  `loc_id` varchar(8) DEFAULT NULL COMMENT '料為編號',
  `llts_id` varchar(1) DEFAULT NULL COMMENT '控制先進先出 F或者L',
  `bkfill_loc` varchar(8) DEFAULT NULL COMMENT '流利架料位',
  `ldes_id` varchar(2) DEFAULT NULL COMMENT 'PK標準鐵架整版科位',
  `ldim_id` varchar(1) DEFAULT NULL COMMENT 'H 以科位高度來計算可以存放的商品',
  `x_coord` int(9) DEFAULT NULL COMMENT '鐵架料位坐標x軸',
  `y_coord` int(9) DEFAULT NULL COMMENT '鐵架料位坐標y軸',
  `z_coord` int(9) DEFAULT NULL COMMENT '鐵架料位坐標z軸',
  `bkfill_x_coord` int(9) DEFAULT NULL COMMENT '流利架料位坐標x軸',
  `bkfill_y_coord` int(9) DEFAULT NULL COMMENT '流利架料位坐標y軸',
  `bkfill_z_coord` int(9) DEFAULT NULL COMMENT '流利架料位坐標z軸',
  `lsta_id` varchar(1) DEFAULT 'F' COMMENT 'F可用 H鎖住 A已經指派',
  `sel_stk_pos` int(9) DEFAULT NULL COMMENT '主料位疊貨空間計算 0代表系統自動計算',
  `sel_seq_loc` varchar(8) DEFAULT NULL,
  `sel_pos_hgt` int(9) DEFAULT NULL COMMENT '主料位空間高度',
  `rsv_stk_pos` int(9) DEFAULT '1' COMMENT '副料位疊貨空間計算',
  `rsv_pos_hgt` int(9) DEFAULT NULL COMMENT '副料位疊貨空間高度',
  `stk_pos_dep` int(9) DEFAULT NULL COMMENT '料位深度',
  `stk_lmt` int(9) DEFAULT NULL COMMENT '存放多少個棧板',
  `stk_pos_wid` int(9) DEFAULT NULL COMMENT '料位的寬度',
  `lev` int(9) DEFAULT NULL COMMENT '料位第幾層',
  `lhnd_id` varchar(50) DEFAULT NULL COMMENT '料位存放的商品的撿貨單位 E I C P',
  `ldsp_id` varchar(50) DEFAULT NULL,
  `create_user` int(8) DEFAULT NULL COMMENT '創建者賬號',
  `create_dtim` datetime DEFAULT NULL COMMENT '創建的時間戳',
  `comingle_allow` varchar(1) DEFAULT NULL COMMENT 'Y/N',
  `change_user` int(8) DEFAULT NULL COMMENT '修改人賬號',
  `change_dtim` datetime DEFAULT NULL COMMENT '修改的時間戳',
  `lcat_id` varchar(1) DEFAULT NULL COMMENT '料位用途 S/R',
  `space_remain` int(9) DEFAULT NULL COMMENT '剩餘空間用的欄位',
  `max_loc_wgt` int(9) DEFAULT NULL COMMENT '最大承載重量',
  `loc_status` int(9) DEFAULT '1' COMMENT 'l料位是否已刪除 1 正常 0刪除',
  `hash_loc_id` varchar(50) DEFAULT NULL COMMENT 'hash料位條碼',
  PRIMARY KEY (`row_id`),
  KEY `idx_01` (`lsta_id`,`loc_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=7742 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for iloc_change_detail
-- ----------------------------
DROP TABLE IF EXISTS `iloc_change_detail`;
CREATE TABLE `iloc_change_detail` (
  `icd_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '搬移job編號',
  `icd_work_id` varchar(50) DEFAULT NULL COMMENT '工作編號',
  `icd_item_id` int(10) unsigned NOT NULL COMMENT '商品細項編號',
  `icd_old_loc_id` varchar(11) NOT NULL COMMENT '原料位編號',
  `icd_new_loc_id` varchar(255) NOT NULL COMMENT '新料位編號',
  `icd_create_time` datetime NOT NULL COMMENT '創建時間',
  `icd_create_user` int(11) NOT NULL COMMENT '創建人',
  `icd_remark` varchar(255) DEFAULT NULL COMMENT '異常備註',
  `icd_msg` varchar(50) DEFAULT NULL COMMENT '異常原因',
  `icd_status` varchar(4) NOT NULL DEFAULT 'CRE' COMMENT 'CRE:初始狀態 BSY:正在搬移 COM:搬移完成 ABD:放棄 CAL:取消',
  `icd_modify_time` datetime NOT NULL COMMENT '修改時間',
  `icd_modify_user` int(9) NOT NULL COMMENT '修改人',
  PRIMARY KEY (`icd_id`)
) ENGINE=InnoDB AUTO_INCREMENT=1133 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for import_orders_log
-- ----------------------------
DROP TABLE IF EXISTS `import_orders_log`;
CREATE TABLE `import_orders_log` (
  `rid` int(4) NOT NULL AUTO_INCREMENT,
  `channel_id` int(4) DEFAULT NULL COMMENT '外站ID',
  `tcount` int(4) DEFAULT NULL COMMENT '匯入總筆數',
  `success_count` int(4) DEFAULT NULL COMMENT '成功匯入筆數',
  `file_name` varchar(200) DEFAULT NULL COMMENT '匯入文件名稱',
  `import_date` datetime DEFAULT NULL,
  `exec_name` varchar(45) DEFAULT NULL COMMENT '操作人員',
  PRIMARY KEY (`rid`)
) ENGINE=InnoDB AUTO_INCREMENT=821 DEFAULT CHARSET=utf8 COMMENT='訂單匯入記錄';

-- ----------------------------
-- Table structure for info_map
-- ----------------------------
DROP TABLE IF EXISTS `info_map`;
CREATE TABLE `info_map` (
  `map_id` int(9) NOT NULL AUTO_INCREMENT COMMENT '主鍵',
  `site_id` int(9) NOT NULL COMMENT '站台id  site表主鍵',
  `page_id` int(9) NOT NULL COMMENT '頁面id site_page主鍵',
  `area_id` int(9) NOT NULL COMMENT '區域id  page_area表主鍵',
  `type` int(2) NOT NULL COMMENT '類型 1：活動頁面 2：最新消息 3：訊息公告 4：電子報',
  `info_id` int(9) NOT NULL COMMENT '根據類型不同 取不同表的主鍵 1：epaper_content 2：news_content 3：announce 4：edm_content ',
  `sort` smallint(4) NOT NULL,
  `create_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00' COMMENT '創建時間',
  `create_user_id` int(9) NOT NULL DEFAULT '0' COMMENT '創建者',
  `update_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00' COMMENT '更新時間',
  `update_user_id` int(11) NOT NULL DEFAULT '0' COMMENT '更新人id',
  PRIMARY KEY (`map_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for Informction
-- ----------------------------
DROP TABLE IF EXISTS `Informction`;
CREATE TABLE `Informction` (
  `IN000` varchar(36) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL COMMENT '本身ID',
  `De000` varchar(20) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL COMMENT '訂單編號',
  `Pick001` varchar(20) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL COMMENT '出貨單號',
  `IN001` varchar(20) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL COMMENT '配送狀態1',
  `IN002` varchar(20) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL COMMENT '配送狀態2',
  `IN003` varchar(20) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL COMMENT '照片名稱',
  `IN004` datetime DEFAULT NULL COMMENT 'UPT yyyy/mm/dd',
  `IN005` varchar(20) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL COMMENT 'UPU',
  `IN006` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for inspection_report
-- ----------------------------
DROP TABLE IF EXISTS `inspection_report`;
CREATE TABLE `inspection_report` (
  `rowID` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水編號',
  `product_id` int(9) unsigned NOT NULL COMMENT '商品編號',
  `certificate_type1` varchar(50) NOT NULL COMMENT '證書大類-中文類別',
  `certificate_type2` varchar(50) NOT NULL COMMENT '證書小類-中文類別',
  `sort` int(9) DEFAULT NULL COMMENT '排序',
  `certificate_expdate` datetime NOT NULL COMMENT '證書有效期限',
  `certificate_desc` varchar(50) DEFAULT NULL COMMENT '說明',
  `certificate_filename` varchar(255) NOT NULL DEFAULT '' COMMENT '檔案名稱-連接',
  `k_user` int(11) NOT NULL COMMENT '建立人員',
  `k_date` datetime NOT NULL COMMENT '建立時間',
  `m_user` int(11) NOT NULL COMMENT '異動人員',
  `m_date` datetime NOT NULL COMMENT '異動時間',
  PRIMARY KEY (`rowID`),
  KEY `ix_report_pid` (`product_id`) USING BTREE,
  CONSTRAINT `fk_report_pid` FOREIGN KEY (`product_id`) REFERENCES `product` (`product_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2016 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for invoice_allowance_info
-- ----------------------------
DROP TABLE IF EXISTS `invoice_allowance_info`;
CREATE TABLE `invoice_allowance_info` (
  `invoice_allowance_id` bigint(19) unsigned NOT NULL,
  `allowance_id` int(10) unsigned NOT NULL,
  `order_id` int(10) unsigned NOT NULL,
  `item_id` int(9) unsigned NOT NULL,
  `invoice_status` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `product_name` varchar(255) NOT NULL DEFAULT '',
  `product_spec_name` varchar(255) NOT NULL DEFAULT '',
  `sort` int(9) unsigned NOT NULL,
  `buy_num` int(9) unsigned NOT NULL,
  `single_money` int(9) unsigned NOT NULL,
  `sub_deduct_bonus` int(9) unsigned NOT NULL,
  `subtotal` int(9) NOT NULL,
  `allowance_note` varchar(255) NOT NULL DEFAULT '',
  `allowance_createdate` int(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`invoice_allowance_id`),
  KEY `ix_invoice_allowance_info_aid` (`allowance_id`),
  KEY `ix_invoice_allowance_info_oid` (`order_id`),
  KEY `ix_invoice_allowance_info_ist` (`invoice_status`),
  CONSTRAINT `fk_invoice_allowance_info_aid` FOREIGN KEY (`allowance_id`) REFERENCES `invoice_allowance_record` (`allowance_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for invoice_allowance_record
-- ----------------------------
DROP TABLE IF EXISTS `invoice_allowance_record`;
CREATE TABLE `invoice_allowance_record` (
  `allowance_id` int(10) unsigned NOT NULL,
  `invoice_id` int(10) unsigned NOT NULL,
  `order_id` int(10) unsigned NOT NULL,
  `invoice_number` varchar(10) NOT NULL,
  `invoice_date` int(10) unsigned NOT NULL,
  `allowance_date` int(10) unsigned NOT NULL,
  `buyer_type` tinyint(1) unsigned NOT NULL,
  `buyer_name` varchar(255) NOT NULL DEFAULT '',
  `company_title` varchar(255) NOT NULL DEFAULT '',
  `company_invoice` varchar(30) NOT NULL DEFAULT '',
  `status_createdate` int(9) unsigned NOT NULL,
  `invoice_status` tinyint(1) unsigned NOT NULL,
  `allownace_total` int(9) unsigned NOT NULL,
  `allowance_amount` int(9) unsigned NOT NULL,
  `allowance_tax` int(9) unsigned NOT NULL,
  `allowance_return` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `allowance_return_date` int(10) unsigned NOT NULL,
  `master_writer` varchar(255) NOT NULL,
  `invoice_note` varchar(255) NOT NULL DEFAULT '',
  PRIMARY KEY (`allowance_id`),
  KEY `ix_invoice_allowance_record_iid` (`invoice_id`),
  KEY `ix_invoice_allowance_record_oid` (`order_id`),
  KEY `ix_invoice_allowance_record_inb` (`invoice_number`),
  CONSTRAINT `fk_invoice_allowance_record_iid` FOREIGN KEY (`invoice_id`) REFERENCES `invoice_master_record` (`invoice_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for invoice_cancel_record
-- ----------------------------
DROP TABLE IF EXISTS `invoice_cancel_record`;
CREATE TABLE `invoice_cancel_record` (
  `cancel_id` int(10) unsigned NOT NULL,
  `invoice_id` int(10) unsigned NOT NULL,
  `order_id` int(10) unsigned NOT NULL,
  `invoice_number` varchar(10) NOT NULL,
  `invoice_date` int(10) unsigned NOT NULL,
  `cancel_date` int(10) unsigned NOT NULL,
  `buyer_type` tinyint(1) unsigned NOT NULL,
  `buyer_name` varchar(255) NOT NULL DEFAULT '',
  `company_title` varchar(255) NOT NULL DEFAULT '',
  `company_invoice` varchar(30) NOT NULL DEFAULT '',
  `cancel_reason` varchar(255) NOT NULL,
  `return_tax_document_number` varchar(255) NOT NULL,
  `status_createdate` int(9) unsigned NOT NULL,
  `invoice_status` tinyint(1) unsigned NOT NULL,
  `master_writer` varchar(255) NOT NULL,
  `invoice_note` varchar(255) NOT NULL DEFAULT '',
  PRIMARY KEY (`cancel_id`),
  KEY `ix_invoice_cancel_record_iid` (`invoice_id`),
  KEY `ix_invoice_cancel_record_oid` (`order_id`),
  KEY `ix_invoice_cancel_record_inb` (`invoice_number`),
  CONSTRAINT `fk_invoice_cancel_record_iid` FOREIGN KEY (`invoice_id`) REFERENCES `invoice_master_record` (`invoice_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for invoice_data
-- ----------------------------
DROP TABLE IF EXISTS `invoice_data`;
CREATE TABLE `invoice_data` (
  `id_id` int(11) NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `user_id` int(11) NOT NULL COMMENT '用戶主ID',
  `id_title` varchar(255) NOT NULL COMMENT '發票名稱',
  `id_type` int(11) NOT NULL DEFAULT '1' COMMENT '發票類型：1）電子發票 2）三聯式發票',
  `id_invoice_no` varchar(20) NOT NULL COMMENT '統一編號',
  `id_invoice_title` varchar(255) NOT NULL COMMENT '發票抬頭',
  `id_send_type` int(11) NOT NULL DEFAULT '2' COMMENT '寄送給：1）寄訂貨人 2）隨貨附上',
  `id_default` int(11) NOT NULL COMMENT '為該會員預設發票資訊：1）是 2）否（備註：這個欄位放在會員資料表可以節省資料庫更新的判斷，但先放這張表，再討論。）',
  `id_created` datetime NOT NULL COMMENT '建立時間－yyyymmdd hh:mm:ss',
  `id_modified` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '最後修改',
  PRIMARY KEY (`id_id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8 COMMENT='會員發票資訊資料表';

-- ----------------------------
-- Table structure for invoice_master_record
-- ----------------------------
DROP TABLE IF EXISTS `invoice_master_record`;
CREATE TABLE `invoice_master_record` (
  `invoice_id` int(10) unsigned NOT NULL,
  `order_id` int(10) unsigned NOT NULL,
  `invoice_status` tinyint(2) unsigned NOT NULL,
  `invoice_attribute` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `invoice_modify_count` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `invoice_number` varchar(10) NOT NULL,
  `invoice_date` int(10) unsigned NOT NULL,
  `free_tax` varchar(255) NOT NULL,
  `sales_amount` varchar(255) NOT NULL DEFAULT '',
  `tax_amount` varchar(255) NOT NULL DEFAULT '',
  `total_amount` varchar(255) NOT NULL DEFAULT '',
  `deduct_bonus` int(9) unsigned NOT NULL,
  `deduct_welfare` int(9) unsigned NOT NULL DEFAULT '0',
  `order_freight_normal` int(9) unsigned NOT NULL,
  `order_freight_normal_notax` varchar(255) NOT NULL DEFAULT '',
  `order_freight_low` int(9) unsigned NOT NULL,
  `order_freight_low_notax` varchar(255) NOT NULL DEFAULT '',
  `buyer_type` tinyint(1) unsigned NOT NULL,
  `buyer_name` varchar(255) NOT NULL DEFAULT '',
  `company_invoice` varchar(30) NOT NULL DEFAULT '',
  `company_title` varchar(255) NOT NULL DEFAULT '',
  `order_zip` mediumint(7) unsigned NOT NULL DEFAULT '0',
  `order_address` varchar(255) NOT NULL,
  `invoice_note` varchar(255) NOT NULL DEFAULT '',
  `print_post_createdate` int(9) unsigned NOT NULL,
  `print_post_mailer` varchar(255) NOT NULL DEFAULT '',
  `print_flag` tinyint(1) unsigned NOT NULL,
  `status_createdate` int(9) unsigned NOT NULL,
  `user_update` varchar(255) NOT NULL DEFAULT '',
  `user_updatedate` int(9) unsigned NOT NULL,
  `invoice_win` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `invoice_mode` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `invoice_close` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `invoice_close_date` int(9) unsigned NOT NULL,
  `tax_type` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`invoice_id`),
  KEY `ix_invoice_order_master_oid` (`order_id`),
  KEY `ix_invoice_order_master_inb` (`invoice_number`),
  KEY `ix_invoice_order_master_iss` (`invoice_status`),
  KEY `ix_invoice_order_master_bt` (`buyer_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for invoice_setting
-- ----------------------------
DROP TABLE IF EXISTS `invoice_setting`;
CREATE TABLE `invoice_setting` (
  `invoice_id` int(9) unsigned NOT NULL,
  `invoice_status` tinyint(2) NOT NULL DEFAULT '2',
  `invoice_char` varchar(10) NOT NULL,
  `start_number` int(8) unsigned NOT NULL,
  `end_number` int(8) unsigned NOT NULL,
  `total_number` int(8) unsigned NOT NULL,
  `start_time` int(10) NOT NULL DEFAULT '0',
  `end_time` int(10) NOT NULL DEFAULT '0',
  `now_number` int(8) unsigned NOT NULL,
  `invoice_createdate` varchar(255) NOT NULL DEFAULT '',
  `create_time` int(10) NOT NULL DEFAULT '0',
  `invoice_updatedate` varchar(255) NOT NULL DEFAULT '',
  `modify_time` int(10) NOT NULL DEFAULT '0',
  `invoice_note` varchar(255) NOT NULL DEFAULT '',
  `last_ipfrom` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`invoice_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for invoice_slive_info
-- ----------------------------
DROP TABLE IF EXISTS `invoice_slive_info`;
CREATE TABLE `invoice_slive_info` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `invoice_slive_id` bigint(19) unsigned NOT NULL,
  `invoice_id` int(9) unsigned NOT NULL,
  `order_id` int(10) unsigned NOT NULL,
  `item_id` int(9) unsigned NOT NULL,
  `invoice_status` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `invoice_allowance` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `product_name` varchar(255) NOT NULL DEFAULT '',
  `product_spec_name` varchar(255) NOT NULL DEFAULT '',
  `sort` int(9) unsigned NOT NULL,
  `single_money` int(9) unsigned NOT NULL,
  `sub_deduct_bonus` int(9) unsigned NOT NULL,
  `buy_num` int(9) unsigned NOT NULL,
  `subtotal` int(9) NOT NULL,
  `slive_note` varchar(255) NOT NULL DEFAULT '',
  `slive_createdate` int(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `ix_invoice_slive_info_oid` (`order_id`),
  KEY `ix_invoice_slive_info_ist` (`invoice_status`)
) ENGINE=InnoDB AUTO_INCREMENT=331902 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for invoice_winning_number
-- ----------------------------
DROP TABLE IF EXISTS `invoice_winning_number`;
CREATE TABLE `invoice_winning_number` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '編號',
  `year` int(4) DEFAULT '0' COMMENT '年',
  `month` int(4) DEFAULT '0' COMMENT '月',
  `winning_type` varchar(50) DEFAULT NULL COMMENT '中獎類型',
  `winning_value` varchar(50) DEFAULT NULL COMMENT '中獎號碼',
  `winning_status` int(2) DEFAULT '1' COMMENT '狀態 默認1',
  PRIMARY KEY (`row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=270 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for iplas
-- ----------------------------
DROP TABLE IF EXISTS `iplas`;
CREATE TABLE `iplas` (
  `plas_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '和商品檔的關聯id',
  `dc_id` smallint(6) NOT NULL COMMENT '物流中心編號',
  `whse_id` smallint(6) NOT NULL COMMENT '倉庫編號',
  `loc_id` varchar(8) DEFAULT NULL COMMENT '所關聯的料位編號',
  `change_dtim` datetime DEFAULT NULL COMMENT '最後異動時間戳記，年月時分秒',
  `change_user` int(8) DEFAULT NULL COMMENT '最後異動者賬號',
  `create_dtim` datetime DEFAULT NULL COMMENT '創建時間',
  `create_user` int(8) DEFAULT NULL COMMENT '創建者',
  `lcus_id` varchar(1) DEFAULT 'P' COMMENT '關聯料位P為主料位，主料位裡頭其餘版號為Deep',
  `luis_id` varchar(1) DEFAULT NULL COMMENT 'OP=OM為C,OP<>OM為I',
  `item_id` int(9) unsigned DEFAULT '0' COMMENT '商品編號',
  `prdd_id` smallint(6) NOT NULL DEFAULT '1' COMMENT '商品序號',
  `loc_rpln_lvl_uoi` int(11) DEFAULT NULL COMMENT '觸動補貨量，例如設定為0，當主料位庫存為0時，自動生成補貨工作',
  `loc_stor_cse_cap` smallint(6) DEFAULT NULL COMMENT '指定主料位可以存放多少庫存，人工維護，必須檢查商品的材積和料位可用空間，單位為箱。當iloc.lhnd_id=C,這個欄位才生效',
  `ptwy_anch` varchar(8) DEFAULT NULL COMMENT '系統自動PutAway(收貨時尋找可用副料位)時，尋找可用料位的邏輯設定組',
  `flthru_anch` varchar(8) DEFAULT NULL COMMENT '系統自動PutAway(收貨時尋找可用副料位)時，尋找可用料位的邏輯設定組',
  `pwy_loc_cntl` varchar(1) DEFAULT NULL COMMENT '系統自動PutAway(收貨時尋找可用副料位)時，尋找可用料位的邏輯設定組',
  PRIMARY KEY (`plas_id`)
) ENGINE=InnoDB AUTO_INCREMENT=5596 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for ipo
-- ----------------------------
DROP TABLE IF EXISTS `ipo`;
CREATE TABLE `ipo` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT,
  `po_id` varchar(18) NOT NULL COMMENT '採購單單號',
  `vend_id` varchar(18) NOT NULL COMMENT '廠商編號   關聯供應商表中的erp_id',
  `buyer` varchar(18) NOT NULL COMMENT '採購員代號',
  `sched_rcpt_dt` date DEFAULT NULL COMMENT '預計送貨(吉甲地)日期',
  `po_type` varchar(4) NOT NULL COMMENT '分辨Po的屬性-採購單別',
  `po_type_desc` varchar(16) DEFAULT NULL COMMENT 'PO屬性中文說明-採購單別描述',
  `cancel_dt` date DEFAULT NULL COMMENT '未送即刪除記錄',
  `msg1` varchar(70) DEFAULT NULL COMMENT '備註欄',
  `msg2` varchar(70) DEFAULT NULL COMMENT '備註欄',
  `msg3` varchar(70) DEFAULT NULL COMMENT '備註欄',
  `create_user` int(9) NOT NULL COMMENT '創建人',
  `create_dtim` datetime NOT NULL COMMENT '創建時間',
  `change_user` int(9) NOT NULL COMMENT '更改人',
  `change_dtim` datetime NOT NULL,
  `status` int(9) NOT NULL DEFAULT '1' COMMENT '1表示存在  0 表示刪除',
  PRIMARY KEY (`row_id`),
  KEY `po_id` (`po_id`)
) ENGINE=InnoDB AUTO_INCREMENT=9201 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for ipo_nvd
-- ----------------------------
DROP TABLE IF EXISTS `ipo_nvd`;
CREATE TABLE `ipo_nvd` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `work_id` varchar(50) NOT NULL DEFAULT '工作單號' COMMENT '流水號',
  `ipo_id` varchar(50) NOT NULL DEFAULT '' COMMENT '採購單編號',
  `item_id` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '商品細項編號',
  `ipo_qty` int(11) NOT NULL DEFAULT '0' COMMENT '採購單驗收數量',
  `out_qty` int(11) NOT NULL DEFAULT '0' COMMENT '未收貨上架數量',
  `com_qty` int(11) NOT NULL DEFAULT '0' COMMENT '完成收穫上架數量',
  `cde_dt` date NOT NULL DEFAULT '0001-01-01' COMMENT '有效日期',
  `made_date` date NOT NULL DEFAULT '0001-01-01' COMMENT '製造日期',
  `work_status` varchar(4) NOT NULL DEFAULT 'AVL' COMMENT '收穫上架狀態 AVL:未處理 SKP:已處理但未完成 COM:已完成',
  `create_user` int(11) NOT NULL DEFAULT '2' COMMENT '創建人',
  `create_datetime` datetime NOT NULL DEFAULT '0001-01-01 00:00:00' COMMENT '創建時間',
  `modify_user` int(11) NOT NULL DEFAULT '2' COMMENT '修改人',
  `modify_datetime` datetime NOT NULL DEFAULT '0001-01-01 00:00:00' COMMENT '修改時間',
  PRIMARY KEY (`row_id`),
  KEY `work_id` (`work_id`),
  KEY `item_id` (`item_id`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for ipo_nvd_log
-- ----------------------------
DROP TABLE IF EXISTS `ipo_nvd_log`;
CREATE TABLE `ipo_nvd_log` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `work_id` varchar(50) NOT NULL DEFAULT '' COMMENT '工作編號',
  `ipo_id` varchar(50) NOT NULL DEFAULT '' COMMENT '採購單編號',
  `item_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '商品細項編號',
  `add_qty` int(11) NOT NULL DEFAULT '0' COMMENT '收貨上架數量',
  `made_date` date NOT NULL DEFAULT '0001-01-01' COMMENT '收穫上架的製造日期',
  `cde_date` date NOT NULL DEFAULT '0001-01-01' COMMENT '保存期限',
  `create_user` int(11) NOT NULL DEFAULT '2' COMMENT '創建人',
  `create_datetime` datetime NOT NULL DEFAULT '0001-01-01 00:00:00' COMMENT '創建時間',
  PRIMARY KEY (`row_id`),
  KEY `work_id` (`work_id`),
  KEY `item_id` (`item_id`),
  KEY `ipo_id` (`ipo_id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for ipod
-- ----------------------------
DROP TABLE IF EXISTS `ipod`;
CREATE TABLE `ipod` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT,
  `po_id` varchar(18) NOT NULL COMMENT '採購單(收貨單)單號',
  `pod_id` int(9) NOT NULL COMMENT '序號，每一個的序號',
  `plst_id` varchar(1) NOT NULL DEFAULT 'o' COMMENT '每個品項的收貨狀態，生成po預設為"o"',
  `bkord_allow` varchar(1) NOT NULL DEFAULT '' COMMENT '是否允許多次到貨(by品項) 默認不允許 Y/N',
  `cde_dt_incr` smallint(6) NOT NULL DEFAULT '0' COMMENT '有效期天數(保存期限)',
  `cde_dt_var` smallint(6) NOT NULL DEFAULT '0' COMMENT '允收天數',
  `cde_dt_shp` smallint(6) NOT NULL DEFAULT '0' COMMENT '允出天數',
  `pwy_dte_ctl` varchar(1) NOT NULL COMMENT '有效期控制的商品(Ｙ/Ｎ)',
  `qty_ord` int(9) NOT NULL COMMENT '下單採購量',
  `qty_damaged` int(9) NOT NULL DEFAULT '0' COMMENT '不允收的量',
  `qty_claimed` int(9) NOT NULL DEFAULT '0' COMMENT '(最後)實際收貨量(允收量)',
  `promo_invs_flg` varchar(1) NOT NULL COMMENT '標註品項庫存用途 “I”為crasy item印花商店',
  `prod_id` varchar(18) NOT NULL COMMENT '六碼品號',
  `create_user` int(9) NOT NULL,
  `create_dtim` datetime NOT NULL,
  `change_user` int(9) NOT NULL,
  `change_dtim` datetime NOT NULL COMMENT '本次訂貨價格',
  `req_cost` double NOT NULL DEFAULT '0' COMMENT '原訂單價格',
  `off_invoice` double NOT NULL DEFAULT '0' COMMENT '折扣%',
  `new_cost` double NOT NULL DEFAULT '0' COMMENT '本次訂貨價格',
  `freight_price` int(9) NOT NULL DEFAULT '0' COMMENT '運費',
  `cde_dt` date DEFAULT '0001-01-01',
  `made_date` date DEFAULT '0001-01-01' COMMENT '製造日期',
  PRIMARY KEY (`row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=23547 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for istock_change
-- ----------------------------
DROP TABLE IF EXISTS `istock_change`;
CREATE TABLE `istock_change` (
  `sc_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '帳卡編號',
  `sc_trans_id` varchar(50) NOT NULL COMMENT '交易編號',
  `sc_cd_id` varchar(50) DEFAULT NULL COMMENT '前置單號',
  `item_id` int(9) unsigned NOT NULL COMMENT '商品細項編號',
  `sc_istock_why` int(4) DEFAULT NULL COMMENT '1.庫鎖   2,庫調',
  `sc_trans_type` int(4) NOT NULL COMMENT '交易類型  1、收貨上架 2、庫存調整 3、RF理貨 4、 出盤復盤',
  `sc_num_old` int(11) NOT NULL COMMENT '原來數量',
  `sc_num_chg` int(11) NOT NULL COMMENT '變化數量',
  `sc_num_new` int(11) NOT NULL COMMENT '新數量',
  `sc_time` datetime NOT NULL COMMENT '創建時間',
  `sc_user` smallint(4) NOT NULL COMMENT '創建者',
  `sc_note` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`sc_id`),
  KEY `item_id_index` (`item_id`)
) ENGINE=InnoDB AUTO_INCREMENT=21456 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for item_collect
-- ----------------------------
DROP TABLE IF EXISTS `item_collect`;
CREATE TABLE `item_collect` (
  `item_collect_id` int(9) unsigned NOT NULL AUTO_INCREMENT COMMENT '收藏商品流水號',
  `site_id` int(9) NOT NULL COMMENT '站台ID',
  `product_id` int(9) NOT NULL COMMENT '商品ID',
  `item_id` int(9) NOT NULL COMMENT '規格ID',
  `item_collect_price` int(9) NOT NULL DEFAULT '0' COMMENT '當時價格',
  `item_collect_time` datetime NOT NULL COMMENT '收藏時間（yyyy-mm-dd hh:mm:ss）',
  `user_id` int(9) NOT NULL COMMENT '會員ID',
  `item_collect_status` tinyint(3) NOT NULL DEFAULT '1' COMMENT '狀態（1:顯示 2:刪除不顯示）',
  PRIMARY KEY (`item_collect_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3282 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for item_ipo_create_log
-- ----------------------------
DROP TABLE IF EXISTS `item_ipo_create_log`;
CREATE TABLE `item_ipo_create_log` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `item_id` int(11) NOT NULL COMMENT '商品細項編號',
  `create_time` datetime NOT NULL COMMENT '創建時間',
  `create_user` int(11) NOT NULL COMMENT '創建人',
  `log_status` int(11) NOT NULL DEFAULT '1' COMMENT '狀態',
  PRIMARY KEY (`row_id`),
  KEY `item_id` (`item_id`)
) ENGINE=InnoDB AUTO_INCREMENT=198 DEFAULT CHARSET=utf8 COMMENT='商品採購下單記錄';

-- ----------------------------
-- Table structure for item_price
-- ----------------------------
DROP TABLE IF EXISTS `item_price`;
CREATE TABLE `item_price` (
  `item_price_id` int(9) unsigned NOT NULL AUTO_INCREMENT,
  `item_id` int(9) unsigned NOT NULL COMMENT 'product_item.id',
  `price_master_id` int(9) unsigned NOT NULL,
  `item_money` int(9) unsigned NOT NULL COMMENT '商品細項售價',
  `item_cost` int(9) unsigned NOT NULL COMMENT '商品細項成本',
  `event_money` int(9) unsigned NOT NULL COMMENT '商品細項活動售價',
  `event_cost` int(9) unsigned NOT NULL COMMENT '商品細項活動成本',
  PRIMARY KEY (`item_price_id`),
  KEY `ix_item_price` (`item_id`,`price_master_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=33005 DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT COMMENT='商品細項價格';

-- ----------------------------
-- Table structure for item_price_temp
-- ----------------------------
DROP TABLE IF EXISTS `item_price_temp`;
CREATE TABLE `item_price_temp` (
  `item_price_id` int(9) unsigned NOT NULL AUTO_INCREMENT,
  `item_id` int(9) unsigned NOT NULL COMMENT 'product_item.id',
  `price_master_id` int(9) unsigned NOT NULL,
  `item_money` int(9) unsigned NOT NULL COMMENT '售價',
  `item_cost` int(9) unsigned NOT NULL COMMENT '若成本在ERP,則此欄位不填入值',
  `event_money` int(9) unsigned NOT NULL COMMENT '活動售價',
  `event_cost` int(9) unsigned NOT NULL COMMENT '活動成本,成本若在ERP,此欄位不填入',
  PRIMARY KEY (`item_price_id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT COMMENT='商品細項價格';

-- ----------------------------
-- Table structure for item_price_ts
-- ----------------------------
DROP TABLE IF EXISTS `item_price_ts`;
CREATE TABLE `item_price_ts` (
  `item_price_id` int(9) unsigned NOT NULL AUTO_INCREMENT,
  `item_id` int(9) unsigned NOT NULL COMMENT 'product_item.id',
  `price_master_id` int(9) unsigned NOT NULL,
  `item_money` int(9) unsigned NOT NULL COMMENT '商品細項售價',
  `item_cost` int(9) unsigned NOT NULL COMMENT '商品細項成本',
  `event_money` int(9) unsigned NOT NULL COMMENT '商品細項活動售價',
  `event_cost` int(9) unsigned NOT NULL COMMENT '商品細項活動成本',
  `apply_id` int(9) unsigned NOT NULL,
  PRIMARY KEY (`item_price_id`,`apply_id`),
  KEY `ix_item_price` (`item_id`,`price_master_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=32992 DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT COMMENT='商品細項價格';

-- ----------------------------
-- Table structure for item_set
-- ----------------------------
DROP TABLE IF EXISTS `item_set`;
CREATE TABLE `item_set` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `item_parent_id` int(11) NOT NULL,
  `item_child_id` int(11) NOT NULL,
  `num` int(11) unsigned NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=89 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for iupc
-- ----------------------------
DROP TABLE IF EXISTS `iupc`;
CREATE TABLE `iupc` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT,
  `upc_id` varchar(50) NOT NULL DEFAULT '' COMMENT '條碼',
  `item_id` int(9) unsigned NOT NULL DEFAULT '0',
  `suppr_upc` varchar(6) DEFAULT NULL,
  `lst_ship_dte` datetime DEFAULT NULL COMMENT '最近一次出貨日期',
  `lst_rct_dte` datetime DEFAULT NULL COMMENT '最近一次收貨日期',
  `create_dtim` datetime DEFAULT NULL COMMENT '創建時間',
  `create_user` int(8) DEFAULT '0' COMMENT '創建的賬號id',
  `upc_type_flg` varchar(1) DEFAULT NULL,
  PRIMARY KEY (`row_id`),
  KEY `idx_01` (`upc_id`,`item_id`)
) ENGINE=InnoDB AUTO_INCREMENT=8955 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for iwms_record
-- ----------------------------
DROP TABLE IF EXISTS `iwms_record`;
CREATE TABLE `iwms_record` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `order_id` int(11) NOT NULL COMMENT '訂單編號',
  `detail_id` int(11) NOT NULL COMMENT '訂單細項編號order_detail.detail_id',
  `act_pick_qty` int(11) NOT NULL COMMENT '撿貨數量 一個生產日期一個  例如：訂5個 2012 2  2013 2 2014 1',
  `made_dt` date DEFAULT '0000-00-00' COMMENT '製造日期',
  `cde_dt` date NOT NULL DEFAULT '0000-00-00' COMMENT '有效日期',
  `status` int(4) NOT NULL DEFAULT '1',
  `create_date` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `create_user_id` int(9) NOT NULL,
  `cde_dt_incr` int(6) DEFAULT '0' COMMENT '保存期限(天数)',
  `cde_dt_shp` int(6) DEFAULT '0' COMMENT '允出天數',
  PRIMARY KEY (`row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=43279 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for logistics_cat_detail
-- ----------------------------
DROP TABLE IF EXISTS `logistics_cat_detail`;
CREATE TABLE `logistics_cat_detail` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `deliver_id` int(11) NOT NULL,
  `delivery_store` int(3) NOT NULL,
  `logistics_type` int(2) NOT NULL,
  `set_time` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=43787 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for logistics_detail
-- ----------------------------
DROP TABLE IF EXISTS `logistics_detail`;
CREATE TABLE `logistics_detail` (
  `rid` int(11) NOT NULL AUTO_INCREMENT,
  `deliver_id` int(11) DEFAULT NULL COMMENT '出貨單號',
  `delivery_store_id` int(11) DEFAULT NULL COMMENT '物流公司',
  `logisticsType` int(4) DEFAULT NULL COMMENT '小物流狀態大(轉運中)',
  `set_time` datetime DEFAULT NULL COMMENT '作業時間',
  PRIMARY KEY (`rid`),
  KEY `order_master.deliver_id` (`deliver_id`)
) ENGINE=InnoDB AUTO_INCREMENT=51351 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for logistics_logic
-- ----------------------------
DROP TABLE IF EXISTS `logistics_logic`;
CREATE TABLE `logistics_logic` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `fun_id` int(11) NOT NULL COMMENT '功能id',
  `fun_name` varchar(255) NOT NULL COMMENT '功能名稱',
  `logistics` int(11) NOT NULL COMMENT '從此物流業者',
  `tun_logistics` int(11) NOT NULL COMMENT '轉此物流業者',
  `logistics_order` int(3) DEFAULT NULL COMMENT '執行順序',
  `status` smallint(1) NOT NULL DEFAULT '0' COMMENT '開關',
  `up_user` varchar(255) NOT NULL COMMENT '最後改人員',
  `createtime` datetime NOT NULL,
  `updatetime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `id` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for logistics_menu
-- ----------------------------
DROP TABLE IF EXISTS `logistics_menu`;
CREATE TABLE `logistics_menu` (
  `fun_id` int(11) NOT NULL AUTO_INCREMENT,
  `fun_name` varchar(255) NOT NULL COMMENT '功能名稱',
  `status` smallint(1) NOT NULL DEFAULT '1' COMMENT '顯示',
  `createtime` datetime NOT NULL,
  `updatetime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`fun_id`),
  KEY `fun_id` (`fun_id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for logistics_setup
-- ----------------------------
DROP TABLE IF EXISTS `logistics_setup`;
CREATE TABLE `logistics_setup` (
  `setup_id` int(11) NOT NULL AUTO_INCREMENT,
  `logic_id` int(11) NOT NULL COMMENT '邏輯id',
  `fun_id` int(11) NOT NULL COMMENT '功能id',
  `code_name` varchar(255) NOT NULL COMMENT 'code名稱',
  `code_value` varchar(255) NOT NULL COMMENT 'code值',
  `logic_status` smallint(1) NOT NULL DEFAULT '1' COMMENT '邏輯狀態',
  `memo` varchar(255) DEFAULT NULL COMMENT '說明',
  `up_user` int(11) NOT NULL COMMENT '修改者',
  `createtime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`setup_id`)
) ENGINE=InnoDB AUTO_INCREMENT=102 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for logistics_setup_exception
-- ----------------------------
DROP TABLE IF EXISTS `logistics_setup_exception`;
CREATE TABLE `logistics_setup_exception` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `setup_id` int(11) NOT NULL,
  `road` varchar(50) NOT NULL COMMENT '路/街/大道',
  `section` int(3) NOT NULL COMMENT '段',
  `number_start` int(5) NOT NULL COMMENT '號',
  `number_end` int(5) NOT NULL COMMENT '號',
  `all_status` smallint(1) NOT NULL COMMENT '全不送',
  `ex_status` smallint(1) NOT NULL COMMENT '設定狀態',
  `memo` varchar(200) DEFAULT NULL COMMENT '備註',
  `up_user` int(10) NOT NULL,
  `createtime` datetime NOT NULL,
  `updatetime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `setup_id` (`setup_id`),
  KEY `setup_road` (`setup_id`,`road`,`ex_status`)
) ENGINE=InnoDB AUTO_INCREMENT=3549 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for logistics_tcat_eod
-- ----------------------------
DROP TABLE IF EXISTS `logistics_tcat_eod`;
CREATE TABLE `logistics_tcat_eod` (
  `delivery_type` tinyint(1) NOT NULL COMMENT '1：客戶自行列印託運單。2：速達協助列印(由速達系統分配託運單號)。3：已有單號，由速達列印(A4二模)—逆物流收退貨',
  `delivery_number` varchar(12) COLLATE utf8_unicode_ci NOT NULL COMMENT '當delivery_type=2時，此欄空白',
  `order_id` int(9) NOT NULL COMMENT '吉甲地訂單編號',
  `freight_set` char(4) COLLATE utf8_unicode_ci NOT NULL COMMENT '溫層。0001：常溫，0002：冷藏，0003：冷凍',
  `delivery_distance` char(2) COLLATE utf8_unicode_ci NOT NULL COMMENT '距離。00：同縣市，01：外縣市，02：離島',
  `package_size` char(4) COLLATE utf8_unicode_ci NOT NULL COMMENT '規格。0001：60cm，0002：90cm，0003：120cm，0004：150cm',
  `cash_collect_service` tinyint(4) NOT NULL COMMENT '是否代收貨款。0：否，1：是',
  `cash_collect_amount` int(9) NOT NULL COMMENT '代收金額。若cach_collect_service=1，則此欄為代收金額。若cash_collect_service=0，則此欄為0',
  `receiver_zip` varchar(5) COLLATE utf8_unicode_ci NOT NULL COMMENT '收件人郵遞區號',
  `receiver_address` varchar(120) COLLATE utf8_unicode_ci NOT NULL COMMENT '收件人地址',
  `delivery_date` datetime NOT NULL COMMENT '出貨日期',
  `estimate_arrival` varchar(1) COLLATE utf8_unicode_ci NOT NULL COMMENT '預定配達時段。1：9~12，2：12~17，3：17~20，4：不限時，5：20~21(需限定區域)',
  `package_name` varchar(60) COLLATE utf8_unicode_ci NOT NULL COMMENT '物品名稱',
  `delivery_note` varchar(100) COLLATE utf8_unicode_ci NOT NULL COMMENT '備註',
  `create_user_id` int(10) NOT NULL COMMENT '建立資料的人員代碼',
  `create_date` datetime NOT NULL COMMENT '資料建立日期',
  `upload_time` datetime DEFAULT NULL COMMENT '資料上傳的時間',
  PRIMARY KEY (`delivery_number`,`order_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- ----------------------------
-- Table structure for logistics_tcat_sod
-- ----------------------------
DROP TABLE IF EXISTS `logistics_tcat_sod`;
CREATE TABLE `logistics_tcat_sod` (
  `delivery_number` varchar(12) COLLATE utf8_unicode_ci NOT NULL COMMENT '託運單號',
  `order_id` int(9) NOT NULL COMMENT '訂單編號',
  `delivery_status_time` datetime NOT NULL COMMENT '貨態輸入日期',
  `status_id` varchar(5) COLLATE utf8_unicode_ci NOT NULL COMMENT '狀態ID',
  `station_name` varchar(20) COLLATE utf8_unicode_ci NOT NULL COMMENT '營業所名稱',
  `customer_id` varchar(10) COLLATE utf8_unicode_ci NOT NULL COMMENT '客戶代號',
  `status_note` varchar(60) COLLATE utf8_unicode_ci NOT NULL COMMENT '狀態說明',
  `specification` varchar(3) COLLATE utf8_unicode_ci NOT NULL COMMENT '規格。視客戶需求而回',
  `create_date` datetime NOT NULL COMMENT '資料建立日期',
  PRIMARY KEY (`delivery_number`,`order_id`,`delivery_status_time`,`status_id`),
  CONSTRAINT `FK_logistics_tcat_sod_logistics_tcat_eod` FOREIGN KEY (`delivery_number`, `order_id`) REFERENCES `logistics_tcat_eod` (`delivery_number`, `order_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- ----------------------------
-- Table structure for mail_group
-- ----------------------------
DROP TABLE IF EXISTS `mail_group`;
CREATE TABLE `mail_group` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `group_name` varchar(100) NOT NULL COMMENT '羣組名稱',
  `remark` varchar(100) DEFAULT NULL COMMENT '備註',
  `status` int(11) NOT NULL COMMENT '狀態 0不啓用 1啓用',
  `create_time` datetime NOT NULL COMMENT '創建時間',
  `create_user` int(11) NOT NULL COMMENT '創建人',
  `update_time` datetime NOT NULL COMMENT '創建時間',
  `update_user` int(11) NOT NULL COMMENT '修改人',
  `group_code` varchar(100) NOT NULL COMMENT '群組代碼',
  PRIMARY KEY (`row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=83 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for mail_group_map
-- ----------------------------
DROP TABLE IF EXISTS `mail_group_map`;
CREATE TABLE `mail_group_map` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `group_id` int(11) NOT NULL COMMENT '羣組編號',
  `user_id` int(11) NOT NULL COMMENT '用戶編號',
  `status` int(11) NOT NULL DEFAULT '1' COMMENT '狀態 0不啓用 1啓用',
  `create_time` datetime NOT NULL COMMENT '創建時間',
  `create_user` int(11) NOT NULL COMMENT '創建人',
  `update_time` datetime NOT NULL COMMENT '修改時間',
  `update_user` int(11) NOT NULL COMMENT '修改人',
  PRIMARY KEY (`row_id`),
  KEY `mail_group_map_ibfk_1` (`group_id`),
  KEY `user_id` (`user_id`),
  CONSTRAINT `mail_group_map_ibfk_1` FOREIGN KEY (`group_id`) REFERENCES `mail_group` (`row_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `mail_group_map_ibfk_2` FOREIGN KEY (`user_id`) REFERENCES `mail_user` (`row_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=967 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for mail_log
-- ----------------------------
DROP TABLE IF EXISTS `mail_log`;
CREATE TABLE `mail_log` (
  `priority` int(9) DEFAULT NULL,
  `user_id` int(9) DEFAULT NULL,
  `send_address` varchar(255) DEFAULT NULL,
  `sender_name` varchar(255) DEFAULT NULL,
  `receiver_address` varchar(255) DEFAULT NULL,
  `receiver_name` varchar(255) DEFAULT NULL,
  `subject` varchar(255) DEFAULT NULL,
  `importance` int(9) DEFAULT NULL,
  `schedule_date` datetime DEFAULT NULL,
  `valid_until_date` datetime DEFAULT NULL,
  `retry_count` int(9) DEFAULT '0',
  `last_sent` datetime DEFAULT NULL,
  `sent_log` longtext,
  `send_result` int(1) DEFAULT '1' COMMENT '郵件最後的寄送結果。success=1;retry count exceeded=2;mail expired=3;blocked email=4;',
  `request_createdate` datetime DEFAULT NULL,
  `request_updatedate` datetime DEFAULT NULL,
  `log_createdate` timestamp NULL DEFAULT NULL COMMENT '該筆mail_log被建立的日期'
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for mail_request
-- ----------------------------
DROP TABLE IF EXISTS `mail_request`;
CREATE TABLE `mail_request` (
  `request_id` int(9) NOT NULL AUTO_INCREMENT COMMENT '郵件寄送的優先權，數字越小越優先',
  `priority` int(9) NOT NULL DEFAULT '99' COMMENT '郵件寄送的優先權，數字越小越優先',
  `user_id` int(9) DEFAULT NULL COMMENT '客戶的使用者代碼。如果該信件寄送對象不是已註冊客戶，則可以是空值。記錄這個欄位是為了快速回收特定使用者的信件。',
  `sender_address` varchar(255) NOT NULL COMMENT '寄件人電子郵件地址，如果是EDM，則是由edm_content的sender_id，查找mail_sender得來。',
  `sender_name` varchar(255) NOT NULL COMMENT '寄件人顯示名稱，可以空白。如果是EDM，則是由edm_content的sender_id，查找mail_sender得來。',
  `receiver_address` varchar(255) NOT NULL DEFAULT 'jialei0706h@hz-mail.eamc.com.tw' COMMENT '收件人電子郵件地址。如果是EDM，則是由edm_content的group_id，查找edm_group，再查找edm_subscription，再查找users得來。',
  `receiver_name` varchar(255) DEFAULT NULL COMMENT '收件人顯示名稱，可以空白。如果是EDM，則是由edm_content的group_id，查找edm_group，再查找edm_subscription，再查找users得來。',
  `subject` varchar(255) NOT NULL COMMENT '郵件標題。EDM的話，則是來自edm_content的subject。',
  `body` longtext NOT NULL COMMENT '郵件內容。EDM的話，是來自edm_content中的body。',
  `importance` int(9) NOT NULL COMMENT '郵件重要性',
  `schedule_date` datetime NOT NULL COMMENT '排程寄送的時間。EDM的話，是來自edm_content的schedule_date',
  `valid_until_date` datetime DEFAULT NULL COMMENT '信件的有效期，超過這個時間即不發送，應立刻將信件移至mail_log，並記錄mail_log.send_result = ''過期''',
  `retry_count` int(9) NOT NULL DEFAULT '0' COMMENT '郵件目前已經嘗試寄送的次數。',
  `last_sent` datetime DEFAULT NULL COMMENT '郵件最後一次的寄送時間',
  `next_send` datetime DEFAULT NULL COMMENT '郵件預計的下次寄送時間，一開始這個值會等於schedule_date，如果發生寄送失敗，但是還沒有達到重試上限，寄信程式會決定多久之後要重寄，並把時間記錄在這裡。',
  `max_retry` int(9) NOT NULL DEFAULT '0' COMMENT '郵件的嘗試寄送次數上限，搭配retry_count。如果retry_count >= max_retry，則該郵件將直接從佇列移除，並寫入到mail_log中。',
  `sent_log` longtext COMMENT '郵件寄送過程中發生的錯誤記錄在此，如果多次嘗試寄送，則會一直覆蓋這個欄位。只會記得最後的錯誤。',
  `success_action` varchar(255) DEFAULT NULL COMMENT '信件寄送成功後，應該執行的動作',
  `fail_action` varchar(255) DEFAULT NULL COMMENT '信件寄送失敗後，應該執行的動作',
  `request_createdate` datetime NOT NULL,
  `request_updatedate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`request_id`)
) ENGINE=InnoDB AUTO_INCREMENT=92085 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for mail_send
-- ----------------------------
DROP TABLE IF EXISTS `mail_send`;
CREATE TABLE `mail_send` (
  `rowid` int(11) NOT NULL AUTO_INCREMENT,
  `mailfrom` varchar(50) NOT NULL COMMENT '發件人',
  `mailto` varchar(2000) NOT NULL COMMENT '收件人',
  `subject` varchar(2000) NOT NULL COMMENT '發信主旨',
  `mailbody` varchar(8000) NOT NULL COMMENT '發信內容',
  `status` tinyint(1) NOT NULL DEFAULT '0' COMMENT '0:未發送 1:已發送',
  `kuser` varchar(50) NOT NULL COMMENT '輸入人員',
  `kdate` datetime NOT NULL COMMENT '輸入時間',
  `senddate` datetime NOT NULL COMMENT '發送時間',
  `source` varchar(50) NOT NULL COMMENT '發信來源',
  `weight` int(11) NOT NULL DEFAULT '1' COMMENT '發信權重',
  PRIMARY KEY (`rowid`)
) ENGINE=InnoDB AUTO_INCREMENT=4942 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for mail_sender
-- ----------------------------
DROP TABLE IF EXISTS `mail_sender`;
CREATE TABLE `mail_sender` (
  `sender_id` int(10) NOT NULL AUTO_INCREMENT COMMENT 'EDM寄件者代碼',
  `sender_email` varchar(255) DEFAULT NULL COMMENT 'EDM寄件者電子郵件地址',
  `sender_name` varchar(255) DEFAULT NULL COMMENT 'EDM寄件者顯示名稱',
  `sender_createdate` datetime DEFAULT NULL,
  `sender_updatedate` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sender_create_userid` int(9) DEFAULT NULL,
  `sender_update_userid` int(9) DEFAULT NULL,
  PRIMARY KEY (`sender_id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for mail_user
-- ----------------------------
DROP TABLE IF EXISTS `mail_user`;
CREATE TABLE `mail_user` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `user_mail` varchar(255) NOT NULL COMMENT '郵箱地址',
  `user_name` varchar(255) NOT NULL COMMENT '用戶姓名',
  `status` int(11) NOT NULL COMMENT '狀態 0不啓用 1啓用',
  `user_pwd` varchar(50) NOT NULL COMMENT '郵箱密碼',
  `create_time` datetime NOT NULL COMMENT '創建時間',
  `create_user` int(11) NOT NULL COMMENT '創建人',
  `update_time` datetime NOT NULL COMMENT '修改時間',
  `update_user` int(11) NOT NULL COMMENT '修改人',
  PRIMARY KEY (`row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=96 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for mail_user_copy
-- ----------------------------
DROP TABLE IF EXISTS `mail_user_copy`;
CREATE TABLE `mail_user_copy` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `user_mail` varchar(255) NOT NULL COMMENT '郵箱地址',
  `user_name` varchar(255) NOT NULL COMMENT '用戶姓名',
  `status` int(11) NOT NULL COMMENT '狀態 0不啓用 1啓用',
  `user_pwd` varchar(50) NOT NULL COMMENT '郵箱密碼',
  `create_time` datetime NOT NULL COMMENT '創建時間',
  `create_user` int(11) NOT NULL COMMENT '創建人',
  `update_time` datetime NOT NULL COMMENT '修改時間',
  `update_user` int(11) NOT NULL COMMENT '修改人',
  PRIMARY KEY (`row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=70 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for manage_function
-- ----------------------------
DROP TABLE IF EXISTS `manage_function`;
CREATE TABLE `manage_function` (
  `rowid` int(11) NOT NULL AUTO_INCREMENT,
  `functionType` int(11) DEFAULT NULL COMMENT '功能類型(1.頁面，2.控件)',
  `functionGroup` varchar(100) DEFAULT NULL COMMENT '功能模组(頁面存模塊名稱，控件存頁面名稱)',
  `functionName` varchar(100) DEFAULT NULL COMMENT '功能名稱',
  `functionCode` varchar(50) DEFAULT NULL COMMENT '功能代碼(頁面存路徑，控件存ID)',
  `functionPage` varchar(50) NOT NULL COMMENT '預設 index 頁面',
  `iconCls` varchar(50) DEFAULT NULL,
  `remark` varchar(600) DEFAULT NULL COMMENT '描述',
  `kuser` varchar(50) DEFAULT NULL,
  `kdate` datetime DEFAULT NULL,
  `topValue` int(11) DEFAULT NULL COMMENT '控件父級頁面id',
  `permission` int(11) NOT NULL,
  `category` varchar(30) NOT NULL,
  PRIMARY KEY (`rowid`)
) ENGINE=InnoDB AUTO_INCREMENT=46 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for manage_login
-- ----------------------------
DROP TABLE IF EXISTS `manage_login`;
CREATE TABLE `manage_login` (
  `login_id` int(10) unsigned NOT NULL,
  `user_id` smallint(4) unsigned NOT NULL,
  `login_ipfrom` varchar(40) NOT NULL DEFAULT '',
  `login_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`login_id`),
  KEY `ix_manage_login_user_id` (`user_id`),
  CONSTRAINT `fk_manage_login_uid` FOREIGN KEY (`user_id`) REFERENCES `manage_user` (`user_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for manage_permission
-- ----------------------------
DROP TABLE IF EXISTS `manage_permission`;
CREATE TABLE `manage_permission` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` smallint(4) unsigned NOT NULL,
  `permission_num` tinyint(2) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `user_id` (`user_id`,`permission_num`),
  CONSTRAINT `fk_manage_permission_uid` FOREIGN KEY (`user_id`) REFERENCES `manage_user` (`user_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=6315 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for manage_user
-- ----------------------------
DROP TABLE IF EXISTS `manage_user`;
CREATE TABLE `manage_user` (
  `user_id` smallint(4) unsigned NOT NULL,
  `user_username` varchar(255) NOT NULL,
  `user_email` varchar(255) NOT NULL,
  `user_delete_email` varchar(255) DEFAULT NULL,
  `user_password` varchar(64) DEFAULT NULL,
  `user_confirm_code` varchar(64) DEFAULT NULL,
  `user_status` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `user_login_attempts` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `user_lastvisit` int(10) unsigned NOT NULL DEFAULT '0',
  `user_last_login` int(10) unsigned NOT NULL DEFAULT '0',
  `manage` smallint(1) unsigned DEFAULT '0',
  `user_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `user_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `erp_id` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `uk_manage_user_email` (`user_email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for manage_user_copy
-- ----------------------------
DROP TABLE IF EXISTS `manage_user_copy`;
CREATE TABLE `manage_user_copy` (
  `user_id` smallint(4) unsigned NOT NULL,
  `user_username` varchar(255) NOT NULL,
  `user_email` varchar(255) NOT NULL,
  `user_delete_email` varchar(255) DEFAULT NULL,
  `user_password` varchar(64) DEFAULT NULL,
  `user_confirm_code` varchar(64) DEFAULT NULL,
  `user_status` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `user_login_attempts` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `user_lastvisit` int(10) unsigned NOT NULL DEFAULT '0',
  `user_last_login` int(10) unsigned NOT NULL DEFAULT '0',
  `manage` smallint(1) unsigned DEFAULT '0',
  `user_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `user_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `erp_id` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `uk_manage_user_email` (`user_email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for market_category
-- ----------------------------
DROP TABLE IF EXISTS `market_category`;
CREATE TABLE `market_category` (
  `market_category_id` int(9) unsigned NOT NULL AUTO_INCREMENT COMMENT '美安類別編號',
  `market_category_father_id` int(9) DEFAULT '0' COMMENT '父級類別編號',
  `market_category_code` varchar(10) DEFAULT '' COMMENT '美安類別數字編號',
  `market_category_name` varchar(50) DEFAULT '' COMMENT '美安類別名稱',
  `market_category_sort` int(4) DEFAULT '0' COMMENT '顯示順序',
  `market_category_status` int(2) DEFAULT '0' COMMENT '狀態0禁用，1啟用',
  `kuser` int(10) DEFAULT '0' COMMENT '建立人編號',
  `muser` int(10) DEFAULT '0' COMMENT '修改人',
  `created` datetime DEFAULT NULL COMMENT '建立時間',
  `modified` datetime DEFAULT NULL COMMENT '修改時間',
  `attribute` varchar(300) DEFAULT '' COMMENT '屬性，匯入類別時的原始字段',
  PRIMARY KEY (`market_category_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2235 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for market_product_map
-- ----------------------------
DROP TABLE IF EXISTS `market_product_map`;
CREATE TABLE `market_product_map` (
  `map_id` int(9) NOT NULL AUTO_INCREMENT COMMENT '關係編號',
  `product_category_id` int(9) NOT NULL DEFAULT '0' COMMENT '吉甲地類別編號',
  `market_category_id` int(9) NOT NULL DEFAULT '0' COMMENT '美安類別編號',
  `kuser` int(10) DEFAULT '0' COMMENT '建立人',
  `muser` int(10) DEFAULT NULL,
  `created` datetime DEFAULT NULL COMMENT '建立時間',
  `modified` datetime DEFAULT NULL COMMENT '修改時間',
  PRIMARY KEY (`map_id`)
) ENGINE=InnoDB AUTO_INCREMENT=84 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for member_event
-- ----------------------------
DROP TABLE IF EXISTS `member_event`;
CREATE TABLE `member_event` (
  `rowID` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水編號',
  `me_name` varchar(100) DEFAULT NULL COMMENT '活動名稱',
  `me_desc` varchar(100) DEFAULT NULL COMMENT '活動描述',
  `me_startdate` datetime DEFAULT NULL COMMENT '活動開始日期',
  `me_enddate` datetime DEFAULT NULL COMMENT '活動結束日期',
  `et_id` int(11) DEFAULT NULL COMMENT '活動類型',
  `me_birthday` tinyint(2) DEFAULT '0' COMMENT '壽星會員獨享。1->是，0->否',
  `event_id` varchar(50) DEFAULT NULL COMMENT '促銷編號',
  `me_big_banner` varchar(100) DEFAULT NULL COMMENT '活動圖',
  `me_banner_link` varchar(200) DEFAULT NULL COMMENT '圖片連接地址',
  `me_bonus_onetime` tinyint(2) DEFAULT '1' COMMENT '是否只限領1次。1->是，0->否',
  `ml_code` varchar(50) DEFAULT NULL COMMENT '群組編碼',
  `me_status` tinyint(2) DEFAULT '0' COMMENT '活動啟用狀態。1->啟用，0->不啟用',
  `k_date` datetime DEFAULT NULL COMMENT '建立日期',
  `k_user` int(11) DEFAULT NULL COMMENT '建立人員',
  `m_date` datetime DEFAULT NULL COMMENT '異動日期',
  `m_user` int(11) DEFAULT NULL COMMENT '異動人員',
  PRIMARY KEY (`rowID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for member_level
-- ----------------------------
DROP TABLE IF EXISTS `member_level`;
CREATE TABLE `member_level` (
  `rowID` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水編號',
  `ml_code` varchar(50) DEFAULT NULL COMMENT '級別代碼',
  `ml_name` varchar(50) DEFAULT NULL COMMENT '級別名稱',
  `ml_seq` int(11) DEFAULT NULL COMMENT '級別排序',
  `ml_minimal_amount` int(11) DEFAULT NULL COMMENT '最低累積消費金額',
  `ml_max_amount` int(11) DEFAULT '0' COMMENT '累積消費金額上限',
  `ml_month_seniority` int(11) DEFAULT NULL COMMENT '最低會員年資',
  `ml_last_purchase` int(11) DEFAULT NULL COMMENT '最近消費時間距離（月）',
  `ml_minpurchase_times` int(11) DEFAULT NULL COMMENT '最低消費次數',
  `ml_birthday_voucher` int(11) DEFAULT NULL COMMENT '生日禮金（元）',
  `ml_shipping_voucher` int(11) NOT NULL DEFAULT '0' COMMENT '免運劵 (張)',
  `ml_status` tinyint(2) DEFAULT NULL COMMENT '啟用狀態。1->啟用，0->未啟用',
  `k_date` datetime DEFAULT NULL COMMENT '建立日期',
  `k_user` int(11) DEFAULT NULL COMMENT '建立人員',
  `m_date` datetime DEFAULT NULL COMMENT '異動日期',
  `m_user` int(11) DEFAULT NULL COMMENT '異動人員',
  PRIMARY KEY (`rowID`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for nccc_bank_data
-- ----------------------------
DROP TABLE IF EXISTS `nccc_bank_data`;
CREATE TABLE `nccc_bank_data` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `bank_code` varchar(3) CHARACTER SET utf8 NOT NULL,
  `bank_name` varchar(100) CHARACTER SET utf8 NOT NULL,
  `card_type` varchar(2) CHARACTER SET utf8 NOT NULL COMMENT '卡別',
  `card` int(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=877 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for nccc_detail_data
-- ----------------------------
DROP TABLE IF EXISTS `nccc_detail_data`;
CREATE TABLE `nccc_detail_data` (
  `nccc_detail_id` int(9) unsigned NOT NULL DEFAULT '0',
  `order_id` int(9) unsigned NOT NULL DEFAULT '0',
  `deduct_amount` int(9) unsigned NOT NULL DEFAULT '0',
  `create_time` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `payment_amount_time` int(10) unsigned NOT NULL DEFAULT '0',
  `modify_time` int(10) unsigned NOT NULL DEFAULT '0',
  `transaction_time` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`nccc_detail_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for nccc_master_data
-- ----------------------------
DROP TABLE IF EXISTS `nccc_master_data`;
CREATE TABLE `nccc_master_data` (
  `nccc_master_id` int(9) unsigned NOT NULL DEFAULT '0',
  `order_id` int(9) unsigned NOT NULL DEFAULT '0',
  `payment_amount` int(9) unsigned NOT NULL DEFAULT '0',
  `create_time` int(10) unsigned NOT NULL DEFAULT '0',
  `payment_amount_time` int(10) unsigned NOT NULL DEFAULT '0',
  `modify_time` int(10) unsigned NOT NULL DEFAULT '0',
  `transaction_time` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`nccc_master_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for nccc_msg
-- ----------------------------
DROP TABLE IF EXISTS `nccc_msg`;
CREATE TABLE `nccc_msg` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `code` varchar(50) NOT NULL DEFAULT '',
  `msg` varchar(100) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `INDEX` (`code`)
) ENGINE=InnoDB AUTO_INCREMENT=154 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for new_present_record
-- ----------------------------
DROP TABLE IF EXISTS `new_present_record`;
CREATE TABLE `new_present_record` (
  `row_id` int(10) NOT NULL AUTO_INCREMENT COMMENT '主鍵，自動增長列',
  `event_id` varchar(10) NOT NULL DEFAULT '0' COMMENT '活動編號new_promo_present.event_id ',
  `user_order_id` int(9) unsigned NOT NULL COMMENT '會員ID或訂單ID',
  `choice_type` tinyint(1) NOT NULL COMMENT '標記贈禮時取的是user_id(1),還是order_id(2)',
  `createdate` int(10) DEFAULT NULL COMMENT '贈送時間',
  PRIMARY KEY (`row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=11664 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for new_promo_carnet
-- ----------------------------
DROP TABLE IF EXISTS `new_promo_carnet`;
CREATE TABLE `new_promo_carnet` (
  `row_id` int(4) NOT NULL AUTO_INCREMENT COMMENT '流水編號',
  `event_name` varchar(510) DEFAULT '' COMMENT '活動名稱',
  `event_desc` varchar(100) DEFAULT '' COMMENT '活動描述',
  `present_event_id` varchar(10) NOT NULL,
  `message_mode` int(4) DEFAULT '0' COMMENT '留言模式 0留言 1密語',
  `message_content` varchar(500) DEFAULT '' COMMENT '留言內容',
  `group_id` int(11) DEFAULT '0' COMMENT '會員群組',
  `link_url` varchar(255) DEFAULT '' COMMENT '連接地址',
  `promo_image` varchar(40) DEFAULT '' COMMENT '活動圖檔',
  `event_id` varchar(10) NOT NULL DEFAULT '' COMMENT '促銷編號',
  `device` varchar(10) DEFAULT '0' COMMENT '裝置 0 :不分 1:PC 2:手機/平板',
  `count_by` tinyint(2) DEFAULT '0' COMMENT '次數限制依  1:訂單  2:會員',
  `count` int(4) DEFAULT '0' COMMENT '限制次數',
  `active_now` tinyint(1) DEFAULT '0' COMMENT '當天就啟用 0:否 1:是',
  `new_user` tinyint(1) DEFAULT '0' COMMENT '是否為新會員0:否1：是',
  `new_user_date` datetime DEFAULT NULL COMMENT '何時之後註冊的會員',
  `start` datetime DEFAULT NULL COMMENT '活動開始時間',
  `end` datetime DEFAULT NULL COMMENT '活動結束時間',
  `active` smallint(2) DEFAULT '0' COMMENT '0:無效  1:有效',
  `kuser` int(10) DEFAULT '0' COMMENT '建立人',
  `muser` int(10) DEFAULT '0' COMMENT '修改人',
  `created` datetime DEFAULT NULL COMMENT '創建時間',
  `modified` datetime DEFAULT NULL COMMENT '修改時間',
  PRIMARY KEY (`row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for new_promo_present
-- ----------------------------
DROP TABLE IF EXISTS `new_promo_present`;
CREATE TABLE `new_promo_present` (
  `row_id` int(10) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `event_id` varchar(10) DEFAULT '0' COMMENT '活動編號',
  `event_type` varchar(50) NOT NULL DEFAULT '' COMMENT '促銷共用用途類型',
  `group_id` int(11) DEFAULT NULL,
  `gift_type` tinyint(2) DEFAULT '0' COMMENT '贈品類型 1:商品2:購物金3：抵用券',
  `valid_start` datetime DEFAULT NULL COMMENT '有效期限起日',
  `valid_end` datetime DEFAULT NULL COMMENT '有效期限迄日',
  `ticket_name` varchar(100) DEFAULT '' COMMENT '活動機會名稱',
  `ticket_serial` varchar(20) DEFAULT NULL COMMENT '活動序號',
  `gift_id` int(4) DEFAULT '0' COMMENT '贈品名稱',
  `deduct_welfare` decimal(12,0) DEFAULT '0' COMMENT 'Bonus金額',
  `gift_amount` int(10) DEFAULT '0' COMMENT '贈送數量',
  `gift_amount_over` int(10) DEFAULT '0' COMMENT '贈送剩餘',
  `freight_price` int(10) DEFAULT '0' COMMENT '運費價',
  `bonus_expire_day` int(10) NOT NULL DEFAULT '30' COMMENT '購物金抵用卷有效天數',
  `status` tinyint(1) DEFAULT '0' COMMENT '狀態  0:未啟用  1:啟用',
  `start` datetime DEFAULT NULL,
  `end` datetime DEFAULT NULL,
  `welfare_mulriple` double(10,2) DEFAULT '1.00' COMMENT '購物金倍數',
  `kuser` int(10) DEFAULT '0' COMMENT '創建人',
  `muser` int(10) DEFAULT '0' COMMENT '更新人',
  `created` datetime DEFAULT NULL COMMENT '創建時間',
  `modified` datetime DEFAULT NULL COMMENT '更新時間',
  `use_span_day` int(11) NOT NULL DEFAULT '0' COMMENT '使用間隔時間（天）',
  PRIMARY KEY (`row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for new_promo_questionnaire
-- ----------------------------
DROP TABLE IF EXISTS `new_promo_questionnaire`;
CREATE TABLE `new_promo_questionnaire` (
  `row_id` int(4) NOT NULL AUTO_INCREMENT COMMENT '流水編號',
  `event_name` varchar(510) DEFAULT NULL COMMENT '活動名稱',
  `event_desc` varchar(100) DEFAULT NULL COMMENT '活動描述',
  `present_event_id` varchar(10) NOT NULL,
  `event_id` varchar(10) NOT NULL COMMENT '活動編號',
  `group_id` int(11) DEFAULT '0' COMMENT '會員群組',
  `link_url` varchar(255) DEFAULT '' COMMENT '連接地址',
  `promo_image` varchar(40) DEFAULT '' COMMENT '活動圖檔',
  `device` varchar(10) DEFAULT NULL COMMENT '裝置',
  `count_by` tinyint(2) DEFAULT '0' COMMENT '次數限制依  1:訂單  2:會員',
  `count` int(4) DEFAULT '0' COMMENT '限制次數',
  `active_now` tinyint(1) DEFAULT '0' COMMENT '當天就啟用 0:否 1:是',
  `new_user` tinyint(1) DEFAULT '0' COMMENT '是否限制為新會員參加',
  `new_user_date` datetime DEFAULT NULL COMMENT '何時之後註冊的會員',
  `start` datetime DEFAULT NULL COMMENT '活動開始時間',
  `end` datetime DEFAULT NULL COMMENT '活動結束時間',
  `active` smallint(2) DEFAULT '0' COMMENT '是否啟用0:無效  1:有效',
  `kuser` int(10) DEFAULT '0' COMMENT '建立人',
  `muser` int(10) DEFAULT '0' COMMENT '修改人',
  `created` datetime DEFAULT NULL COMMENT '創建時間',
  `modified` datetime DEFAULT NULL COMMENT '修改時間',
  PRIMARY KEY (`row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for new_promo_record
-- ----------------------------
DROP TABLE IF EXISTS `new_promo_record`;
CREATE TABLE `new_promo_record` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `event_id` varchar(50) NOT NULL COMMENT '活動編號',
  `message` varchar(500) DEFAULT NULL COMMENT '用戶留言內容',
  `event_type` varchar(10) NOT NULL COMMENT '活動類型',
  `user_id` int(11) DEFAULT NULL COMMENT '會員編號',
  `user_name` varchar(50) DEFAULT NULL COMMENT '用戶名稱',
  `ip` varchar(50) DEFAULT NULL COMMENT '用戶ip',
  `create_time` datetime NOT NULL COMMENT '創建時間',
  `user_tel` varchar(20) DEFAULT NULL COMMENT '用戶電話',
  `user_address` varchar(100) DEFAULT NULL COMMENT '用戶地址',
  `user_mail` varchar(50) DEFAULT NULL COMMENT '用戶信箱',
  `user_reg_date` datetime DEFAULT NULL COMMENT '用戶註冊時間',
  PRIMARY KEY (`row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=201 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for neweb_feedback
-- ----------------------------
DROP TABLE IF EXISTS `neweb_feedback`;
CREATE TABLE `neweb_feedback` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `n_id` int(9) unsigned NOT NULL DEFAULT '0',
  `PRC` varchar(10) NOT NULL DEFAULT '',
  `SRC` varchar(10) NOT NULL DEFAULT '',
  `MerchantNumber` varchar(10) NOT NULL DEFAULT '',
  `OrderNumber` int(9) unsigned NOT NULL DEFAULT '0',
  `ApprovalCode` varchar(10) NOT NULL DEFAULT '',
  `BankResponseCode` varchar(10) NOT NULL DEFAULT '',
  `Amount` varchar(10) NOT NULL DEFAULT '',
  `cardnumber` varchar(50) NOT NULL DEFAULT '',
  `CheckSum` varchar(50) NOT NULL DEFAULT '',
  `engname` varchar(10) NOT NULL DEFAULT '',
  `issuebank` varchar(10) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for neweb_receive
-- ----------------------------
DROP TABLE IF EXISTS `neweb_receive`;
CREATE TABLE `neweb_receive` (
  `n_id` int(9) unsigned NOT NULL DEFAULT '0',
  `final_result` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `P_MerchantNumber` varchar(10) NOT NULL DEFAULT '',
  `P_OrderNumber` int(9) unsigned NOT NULL DEFAULT '0',
  `P_Amount` varchar(10) NOT NULL DEFAULT '',
  `P_CheckSum` varchar(50) NOT NULL DEFAULT '',
  `final_return_PRC` varchar(10) NOT NULL DEFAULT '',
  `final_return_SRC` varchar(10) NOT NULL DEFAULT '',
  `final_return_ApproveCode` varchar(10) NOT NULL DEFAULT '',
  `final_return_BankRC` varchar(10) NOT NULL DEFAULT '',
  `final_return_BatchNumber` varchar(10) NOT NULL DEFAULT '',
  PRIMARY KEY (`n_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for news_content
-- ----------------------------
DROP TABLE IF EXISTS `news_content`;
CREATE TABLE `news_content` (
  `news_id` int(9) unsigned NOT NULL DEFAULT '0',
  `user_id` smallint(4) unsigned NOT NULL,
  `news_title` varchar(255) NOT NULL DEFAULT '',
  `news_content` text NOT NULL,
  `news_sort` smallint(4) unsigned NOT NULL DEFAULT '0',
  `news_status` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `news_show_start` int(10) unsigned NOT NULL DEFAULT '0',
  `news_show_end` int(10) unsigned NOT NULL DEFAULT '0',
  `news_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `news_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `news_ipfrom` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`news_id`),
  KEY `ix_news_content_uid` (`user_id`),
  KEY `ix_news_content_status` (`news_status`),
  KEY `ix_news_content_start` (`news_show_start`),
  KEY `ix_news_content_end` (`news_show_end`),
  CONSTRAINT `fk_news_content_uid` FOREIGN KEY (`user_id`) REFERENCES `manage_user` (`user_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for news_log
-- ----------------------------
DROP TABLE IF EXISTS `news_log`;
CREATE TABLE `news_log` (
  `log_id` bigint(19) unsigned NOT NULL,
  `news_id` int(9) unsigned NOT NULL,
  `user_id` smallint(4) unsigned NOT NULL,
  `log_description` varchar(255) DEFAULT NULL,
  `log_ipfrom` varchar(40) NOT NULL,
  `log_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`log_id`),
  KEY `ix_news_log_nid` (`news_id`),
  KEY `ix_news_log_uid` (`user_id`),
  CONSTRAINT `fk_news_log_nid` FOREIGN KEY (`news_id`) REFERENCES `news_content` (`news_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_news_log_uid` FOREIGN KEY (`user_id`) REFERENCES `manage_user` (`user_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_account_collection
-- ----------------------------
DROP TABLE IF EXISTS `order_account_collection`;
CREATE TABLE `order_account_collection` (
  `row_id` int(9) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `order_id` int(9) unsigned NOT NULL COMMENT '訂單編號',
  `account_collection_time` datetime DEFAULT NULL COMMENT '入賬時間',
  `account_collection_money` int(9) DEFAULT '0' COMMENT '入賬金額',
  `poundage` int(9) DEFAULT '0' COMMENT '手續費',
  `return_collection_time` datetime DEFAULT NULL COMMENT '入賬退貨時間',
  `return_collection_money` int(9) DEFAULT '0' COMMENT '入賬退貨金額',
  `return_poundage` int(9) DEFAULT '0' COMMENT '入賬退貨手續費',
  `remark` varchar(100) DEFAULT NULL COMMENT '備註',
  `invoice_date_manual` datetime DEFAULT NULL COMMENT '手動開立發票日期',
  `invoice_sale_manual` int(9) DEFAULT NULL COMMENT '手動開立發票銷售額',
  `invoice_tax_manual` int(9) DEFAULT NULL COMMENT '手動開立發票稅額',
  PRIMARY KEY (`row_id`),
  UNIQUE KEY `order_id_uniqe` (`order_id`),
  KEY `order_id` (`order_id`),
  CONSTRAINT `order_account_collection_ibfk_1` FOREIGN KEY (`order_id`) REFERENCES `order_master` (`order_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=915 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_accum_amount
-- ----------------------------
DROP TABLE IF EXISTS `order_accum_amount`;
CREATE TABLE `order_accum_amount` (
  `event_id` int(9) NOT NULL AUTO_INCREMENT COMMENT '編號',
  `accum_amount` int(9) NOT NULL DEFAULT '0' COMMENT '累計金額',
  `event_start_time` datetime NOT NULL COMMENT '開始時間',
  `event_end_time` datetime NOT NULL COMMENT '結束時間',
  `event_desc` varchar(200) DEFAULT NULL COMMENT '活動描述',
  `event_name` varchar(100) DEFAULT NULL COMMENT '活動名稱',
  `event_desc_start` datetime DEFAULT NULL COMMENT '描述開始時間',
  `event_desc_end` datetime DEFAULT NULL COMMENT '描述結束時間',
  `event_status` int(4) NOT NULL DEFAULT '0' COMMENT '活動狀態(0啟用,1禁用)',
  `event_create_user` int(9) NOT NULL COMMENT '創建人',
  `event_create_time` datetime NOT NULL COMMENT '創建時間',
  `event_update_user` int(9) NOT NULL COMMENT '修改人',
  `event_update_time` datetime NOT NULL COMMENT '修改時間',
  PRIMARY KEY (`event_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_cancel_detail
-- ----------------------------
DROP TABLE IF EXISTS `order_cancel_detail`;
CREATE TABLE `order_cancel_detail` (
  `cancel_id` int(9) unsigned NOT NULL,
  `detail_id` int(9) unsigned NOT NULL,
  PRIMARY KEY (`cancel_id`,`detail_id`),
  KEY `fk_order_cancel_detail_did` (`detail_id`),
  CONSTRAINT `fk_order_cancel_detail_cid` FOREIGN KEY (`cancel_id`) REFERENCES `order_cancel_master` (`cancel_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_order_cancel_detail_did` FOREIGN KEY (`detail_id`) REFERENCES `order_detail` (`detail_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_cancel_master
-- ----------------------------
DROP TABLE IF EXISTS `order_cancel_master`;
CREATE TABLE `order_cancel_master` (
  `cancel_id` int(9) unsigned NOT NULL,
  `order_id` int(9) unsigned NOT NULL,
  `cancel_status` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `cancel_note` varchar(255) NOT NULL DEFAULT '',
  `bank_note` text COMMENT '銀行資訊',
  `cancel_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `cancel_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `cancel_ipfrom` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`cancel_id`),
  KEY `ix_order_cancel_master_oid` (`order_id`),
  KEY `ix_order_cancel_master_status` (`cancel_status`),
  CONSTRAINT `fk_order_cancel_master_oid` FOREIGN KEY (`order_id`) REFERENCES `order_master` (`order_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_cancel_msg
-- ----------------------------
DROP TABLE IF EXISTS `order_cancel_msg`;
CREATE TABLE `order_cancel_msg` (
  `cancel_id` int(9) unsigned NOT NULL,
  `order_id` int(9) unsigned NOT NULL,
  `cancel_type` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `cancel_status` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `cancel_content` text NOT NULL,
  `cancel_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `cancel_ipfrom` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`cancel_id`),
  KEY `ix_order_cancel_msg_oid` (`order_id`),
  KEY `ix_order_cancel_msg_ss` (`cancel_status`),
  CONSTRAINT `fk_order_cancel_msg_oid` FOREIGN KEY (`order_id`) REFERENCES `order_master` (`order_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_cancel_response
-- ----------------------------
DROP TABLE IF EXISTS `order_cancel_response`;
CREATE TABLE `order_cancel_response` (
  `response_id` int(9) unsigned NOT NULL,
  `cancel_id` int(9) unsigned NOT NULL,
  `user_id` smallint(4) unsigned NOT NULL,
  `response_content` text NOT NULL,
  `response_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `response_ipfrom` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`response_id`),
  KEY `ix_order_cancel_response_cid` (`cancel_id`),
  KEY `ix_order_cancel_response_uid` (`user_id`),
  CONSTRAINT `fk_order_cancel_response_cid` FOREIGN KEY (`cancel_id`) REFERENCES `order_cancel_msg` (`cancel_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_order_cancel_response_uid` FOREIGN KEY (`user_id`) REFERENCES `manage_user` (`user_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_deliver
-- ----------------------------
DROP TABLE IF EXISTS `order_deliver`;
CREATE TABLE `order_deliver` (
  `deliver_id` int(9) unsigned NOT NULL,
  `slave_id` int(9) unsigned NOT NULL,
  `deliver_status` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `deliver_store` smallint(4) unsigned NOT NULL DEFAULT '0',
  `deliver_code` varchar(50) NOT NULL DEFAULT '',
  `deliver_time` int(10) unsigned NOT NULL DEFAULT '0',
  `deliver_note` varchar(255) NOT NULL DEFAULT '',
  `deliver_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `deliver_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `deliver_ipfrom` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`deliver_id`),
  KEY `ix_order_deliver_sid` (`slave_id`),
  KEY `ix_order_deliver_ce` (`deliver_code`),
  CONSTRAINT `fk_order_deliver_sid` FOREIGN KEY (`slave_id`) REFERENCES `order_slave` (`slave_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_detail
-- ----------------------------
DROP TABLE IF EXISTS `order_detail`;
CREATE TABLE `order_detail` (
  `detail_id` int(9) unsigned NOT NULL,
  `slave_id` int(9) unsigned NOT NULL,
  `item_id` int(9) unsigned NOT NULL DEFAULT '0',
  `item_vendor_id` int(9) unsigned NOT NULL DEFAULT '0',
  `product_freight_set` smallint(5) unsigned NOT NULL DEFAULT '1',
  `product_mode` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `product_name` varchar(255) NOT NULL,
  `product_spec_name` varchar(255) NOT NULL,
  `single_cost` mediumint(7) unsigned NOT NULL DEFAULT '0',
  `event_cost` mediumint(7) unsigned NOT NULL DEFAULT '0',
  `single_price` mediumint(7) unsigned NOT NULL DEFAULT '0',
  `single_money` mediumint(7) unsigned NOT NULL DEFAULT '0',
  `deduct_bonus` int(9) unsigned NOT NULL,
  `deduct_welfare` int(9) unsigned NOT NULL,
  `deduct_happygo` int(11) NOT NULL,
  `deduct_happygo_money` int(11) NOT NULL DEFAULT '0',
  `deduct_account` int(9) unsigned NOT NULL,
  `deduct_account_note` varchar(255) NOT NULL,
  `accumulated_bonus` int(11) NOT NULL DEFAULT '0',
  `accumulated_happygo` int(11) NOT NULL DEFAULT '0',
  `buy_num` mediumint(7) unsigned NOT NULL DEFAULT '0',
  `detail_status` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `detail_note` varchar(255) NOT NULL,
  `item_code` varchar(30) NOT NULL,
  `arrival_status` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `delay_till` int(10) unsigned NOT NULL DEFAULT '0',
  `lastmile_deliver_serial` varchar(50) NOT NULL DEFAULT '''''',
  `lastmile_deliver_datetime` int(10) unsigned NOT NULL DEFAULT '0',
  `lastmile_deliver_agency` varchar(255) DEFAULT NULL,
  `bag_check_money` int(9) unsigned NOT NULL DEFAULT '0',
  `channel_detail_id` varchar(16) DEFAULT NULL,
  `combined_mode` int(11) NOT NULL COMMENT '組合商品 0:一般 1:組合 2:子商品',
  `item_mode` smallint(2) unsigned NOT NULL DEFAULT '0' COMMENT '0:單一商品, 1:父商品, 2:子商品',
  `parent_id` int(11) NOT NULL COMMENT '子商品 父親item_id',
  `parent_name` varchar(255) DEFAULT NULL COMMENT '組合商品名稱',
  `parent_num` mediumint(7) unsigned NOT NULL DEFAULT '0' COMMENT '此組合包購買之數量',
  `price_master_id` int(9) unsigned NOT NULL DEFAULT '0',
  `pack_id` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '組合同組代號',
  `sub_order_id` varchar(20) DEFAULT NULL,
  `site_id` int(9) NOT NULL DEFAULT '1',
  `event_id` varchar(11) DEFAULT NULL COMMENT '活動ID',
  `prepaid` tinyint(2) NOT NULL DEFAULT '0' COMMENT '已買斷的商品  0:否  1:是',
  `receiving_status` int(1) NOT NULL DEFAULT '0' COMMENT '確認收貨狀態',
  `receiving_time` datetime DEFAULT NULL COMMENT '確認收貨時間',
  PRIMARY KEY (`detail_id`),
  KEY `fk_product_item_iid` (`item_id`),
  KEY `ix_order_detail_ivid` (`item_vendor_id`),
  KEY `ix_order_detail_dss` (`detail_status`),
  KEY `uk_order_detail` (`slave_id`,`item_id`),
  KEY `parent_id` (`parent_id`),
  CONSTRAINT `fk_order_detail_sid` FOREIGN KEY (`slave_id`) REFERENCES `order_slave` (`slave_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_product_item_iid` FOREIGN KEY (`item_id`) REFERENCES `product_item` (`item_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `order_detail_ibfk_1` FOREIGN KEY (`item_vendor_id`) REFERENCES `vendor` (`vendor_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_detail_manager
-- ----------------------------
DROP TABLE IF EXISTS `order_detail_manager`;
CREATE TABLE `order_detail_manager` (
  `odm_id` int(11) NOT NULL AUTO_INCREMENT,
  `odm_user_id` smallint(4) unsigned DEFAULT NULL,
  `odm_user_name` varchar(255) DEFAULT NULL,
  `odm_status` int(11) DEFAULT NULL,
  `odm_createdate` datetime DEFAULT NULL,
  `odm_createuser` int(11) DEFAULT NULL,
  PRIMARY KEY (`odm_id`),
  KEY `fk_od_user_id` (`odm_user_id`),
  CONSTRAINT `fk_od_user_id` FOREIGN KEY (`odm_user_id`) REFERENCES `manage_user` (`user_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_detail_old
-- ----------------------------
DROP TABLE IF EXISTS `order_detail_old`;
CREATE TABLE `order_detail_old` (
  `detail_id` int(9) unsigned NOT NULL,
  `slave_id` int(9) unsigned NOT NULL,
  `item_id` int(9) unsigned NOT NULL DEFAULT '0',
  `item_vendor_id` int(9) unsigned NOT NULL DEFAULT '0',
  `product_freight_set` smallint(5) unsigned NOT NULL DEFAULT '1',
  `product_mode` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `product_name` varchar(255) NOT NULL,
  `product_spec_name` varchar(255) NOT NULL,
  `single_cost` mediumint(7) unsigned NOT NULL DEFAULT '0',
  `event_cost` mediumint(7) unsigned NOT NULL DEFAULT '0',
  `single_price` mediumint(7) unsigned NOT NULL DEFAULT '0',
  `single_money` mediumint(7) unsigned NOT NULL DEFAULT '0',
  `deduct_bonus` int(9) unsigned NOT NULL,
  `deduct_welfare` int(9) unsigned NOT NULL,
  `deduct_happygo` int(11) NOT NULL,
  `deduct_happygo_money` int(11) NOT NULL DEFAULT '0',
  `deduct_account` int(9) unsigned NOT NULL,
  `deduct_account_note` varchar(255) NOT NULL,
  `accumulated_bonus` int(11) NOT NULL DEFAULT '0',
  `accumulated_happygo` int(11) NOT NULL DEFAULT '0',
  `buy_num` mediumint(7) unsigned NOT NULL DEFAULT '0',
  `detail_status` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `detail_note` varchar(255) NOT NULL,
  `item_code` varchar(30) NOT NULL,
  `arrival_status` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `delay_till` int(10) unsigned NOT NULL DEFAULT '0',
  `lastmile_deliver_serial` varchar(50) NOT NULL DEFAULT '''''',
  `lastmile_deliver_datetime` int(10) unsigned NOT NULL DEFAULT '0',
  `lastmile_deliver_agency` varchar(255) DEFAULT NULL,
  `bag_check_money` int(9) unsigned NOT NULL DEFAULT '0',
  `channel_detail_id` varchar(16) DEFAULT NULL,
  `combined_mode` int(11) NOT NULL COMMENT '組合商品 0:一般 1:組合 2:子商品',
  `item_mode` smallint(2) unsigned NOT NULL DEFAULT '0' COMMENT '0:單一商品, 1:父商品, 2:子商品',
  `parent_id` int(11) NOT NULL COMMENT '子商品 父親item_id',
  `parent_name` varchar(255) DEFAULT NULL COMMENT '組合商品名稱',
  `parent_num` mediumint(7) unsigned NOT NULL DEFAULT '0' COMMENT '此組合包購買之數量',
  `price_master_id` int(9) unsigned NOT NULL DEFAULT '0',
  `pack_id` smallint(2) unsigned NOT NULL DEFAULT '0' COMMENT '組合同組代號',
  `sub_order_id` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`detail_id`),
  KEY `fk_product_item_iid` (`item_id`),
  KEY `ix_order_detail_ivid` (`item_vendor_id`),
  KEY `ix_order_detail_dss` (`detail_status`),
  KEY `uk_order_detail` (`slave_id`,`item_id`),
  KEY `parent_id` (`parent_id`),
  CONSTRAINT `order_detail_old_ibfk_1` FOREIGN KEY (`slave_id`) REFERENCES `order_slave` (`slave_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `order_detail_old_ibfk_2` FOREIGN KEY (`item_id`) REFERENCES `product_item` (`item_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `order_detail_old_ibfk_3` FOREIGN KEY (`item_vendor_id`) REFERENCES `vendor` (`vendor_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_expect_deliver
-- ----------------------------
DROP TABLE IF EXISTS `order_expect_deliver`;
CREATE TABLE `order_expect_deliver` (
  `expect_id` int(9) unsigned NOT NULL,
  `order_id` int(9) unsigned NOT NULL,
  `slave_id` int(9) unsigned NOT NULL,
  `detail_id` int(9) unsigned NOT NULL,
  `status` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `store` smallint(4) unsigned NOT NULL DEFAULT '0',
  `code` varchar(50) NOT NULL DEFAULT '',
  `time` int(10) unsigned NOT NULL DEFAULT '0',
  `note` varchar(255) NOT NULL DEFAULT '',
  `createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`expect_id`,`detail_id`),
  KEY `ix_order_deliver_sid` (`slave_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_hitrust_log
-- ----------------------------
DROP TABLE IF EXISTS `order_hitrust_log`;
CREATE TABLE `order_hitrust_log` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `order_id` int(9) NOT NULL,
  `retcode` varchar(225) NOT NULL,
  `orderstatus` varchar(225) NOT NULL,
  `createtime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `order_id` (`order_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2234 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_master
-- ----------------------------
DROP TABLE IF EXISTS `order_master`;
CREATE TABLE `order_master` (
  `order_id` int(9) unsigned NOT NULL,
  `user_id` int(9) unsigned NOT NULL,
  `bonus_receive` int(9) unsigned NOT NULL,
  `bonus_discount_percent` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `bonus_convert_nt` mediumint(7) unsigned NOT NULL DEFAULT '0',
  `deduct_happygo_convert` float NOT NULL,
  `deduct_bonus` int(9) unsigned NOT NULL,
  `deduct_welfare` int(9) unsigned NOT NULL,
  `deduct_happygo` int(11) NOT NULL DEFAULT '0',
  `deduct_card_bonus` int(11) NOT NULL DEFAULT '0',
  `deduct_account` int(9) unsigned NOT NULL,
  `accumulated_bonus` int(11) NOT NULL DEFAULT '0',
  `accumulated_happygo` int(11) NOT NULL DEFAULT '0',
  `order_freight_normal` int(9) unsigned NOT NULL,
  `order_freight_low` int(9) unsigned NOT NULL,
  `order_product_subtotal` int(9) unsigned NOT NULL,
  `order_amount` int(9) unsigned NOT NULL,
  `money_cancel` int(9) unsigned NOT NULL DEFAULT '0',
  `money_return` int(9) unsigned NOT NULL DEFAULT '0',
  `order_status` tinyint(2) unsigned NOT NULL COMMENT '付款單狀態',
  `order_payment` tinyint(2) unsigned NOT NULL,
  `order_deliver_success` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `order_name` varchar(64) NOT NULL DEFAULT '',
  `order_gender` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `order_mobile` varchar(50) NOT NULL,
  `order_phone` varchar(30) NOT NULL DEFAULT '',
  `order_zip` mediumint(7) unsigned NOT NULL DEFAULT '0',
  `order_address` varchar(255) NOT NULL DEFAULT '',
  `delivery_same` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `delivery_name` varchar(64) NOT NULL DEFAULT '',
  `delivery_gender` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `delivery_mobile` varchar(50) NOT NULL,
  `delivery_phone` varchar(30) NOT NULL DEFAULT '',
  `delivery_zip` mediumint(7) unsigned NOT NULL DEFAULT '0',
  `delivery_address` varchar(255) NOT NULL DEFAULT '',
  `estimated_arrival_period` int(11) NOT NULL,
  `company_write` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `company_invoice` varchar(30) NOT NULL DEFAULT '',
  `company_title` varchar(255) NOT NULL DEFAULT '',
  `invoice_id` int(9) unsigned NOT NULL,
  `order_invoice` varchar(15) NOT NULL DEFAULT '0',
  `invoice_status` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `note_order` varchar(255) NOT NULL DEFAULT '',
  `note_admin` varchar(255) NOT NULL DEFAULT '',
  `order_date_pay` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '可出貨時間',
  `money_collect_date` int(10) DEFAULT '0' COMMENT '付款時間',
  `order_date_close` int(10) unsigned NOT NULL DEFAULT '0',
  `order_date_cancel` int(10) unsigned NOT NULL DEFAULT '0',
  `order_compensate` int(9) NOT NULL DEFAULT '0',
  `cart_id` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `order_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `order_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `order_ipfrom` varchar(40) NOT NULL DEFAULT '',
  `source_trace` int(7) unsigned NOT NULL DEFAULT '0',
  `source_cookie_value` varchar(255) NOT NULL,
  `source_cookie_name` varchar(255) NOT NULL,
  `note_order_modifier` int(10) unsigned NOT NULL DEFAULT '0',
  `note_order_modify_time` int(10) unsigned NOT NULL DEFAULT '0',
  `error_check` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `fax_sn` varchar(10) NOT NULL DEFAULT '''''',
  `channel` int(11) NOT NULL DEFAULT '1',
  `bonus_type` int(11) NOT NULL DEFAULT '1',
  `multi_bonus` varchar(30) NOT NULL DEFAULT '',
  `channel_order_id` varchar(20) DEFAULT NULL,
  `delivery_store` int(11) NOT NULL DEFAULT '1',
  `billing_checked` tinyint(1) NOT NULL,
  `import_time` datetime DEFAULT NULL,
  `retrieve_mode` tinyint(2) NOT NULL DEFAULT '0',
  `bonus_expire_day` int(3) unsigned NOT NULL DEFAULT '90',
  `priority` tinyint(2) unsigned NOT NULL DEFAULT '0' COMMENT '訂單優先順序  0:一般 1:急件',
  `estimated_arrival_start` int(10) NOT NULL DEFAULT '0' COMMENT '預計到貨日-開始',
  `estimated_arrival_end` int(10) NOT NULL DEFAULT '0' COMMENT '預計到貨日-結束',
  `holiday_deliver` tinyint(2) unsigned NOT NULL DEFAULT '0' COMMENT '假日收貨 0:否 1:假日可收 2:只可週六 3:只可週日',
  `export_flag` smallint(5) NOT NULL DEFAULT '0',
  `data_chg` tinyint(2) NOT NULL DEFAULT '0',
  `paper_invoice` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否要發票',
  `deliver_stno` varchar(100) DEFAULT NULL,
  `dcrono` varchar(100) DEFAULT NULL,
  `stnm` varchar(100) DEFAULT NULL,
  `account_collection_time` int(10) DEFAULT '0' COMMENT '會計賬款實收時間',
  PRIMARY KEY (`order_id`),
  KEY `ix_order_master_uid` (`user_id`),
  KEY `ix_order_master_oss` (`order_status`),
  KEY `ix_order_master_opt` (`order_payment`),
  KEY `ix_order_master_ie` (`order_invoice`),
  KEY `ix_order_master_ocd` (`order_date_close`),
  KEY `ix_order_master_odl` (`order_date_cancel`),
  KEY `ix_order_master_ce` (`order_createdate`),
  KEY `ix_order_master_ue` (`order_updatedate`),
  KEY `ix_order_master_iss` (`invoice_status`),
  KEY `ix_estimated_arrival_start_or_end` (`estimated_arrival_start`,`estimated_arrival_end`),
  KEY `ix_estimated_arrival_end` (`estimated_arrival_end`),
  CONSTRAINT `fk_order_master_uid` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_master_pattern
-- ----------------------------
DROP TABLE IF EXISTS `order_master_pattern`;
CREATE TABLE `order_master_pattern` (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT 'pk',
  `order_id` int(11) unsigned DEFAULT NULL COMMENT '訂單編號fk',
  `pattern` int(4) DEFAULT NULL COMMENT '型別 10公關單 20報廢單',
  `dep` int(4) DEFAULT NULL COMMENT '部門別',
  PRIMARY KEY (`id`),
  KEY `order_id` (`order_id`),
  CONSTRAINT `order_master_pattern_ibfk_1` FOREIGN KEY (`order_id`) REFERENCES `order_master` (`order_id`)
) ENGINE=InnoDB AUTO_INCREMENT=119 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for order_master_shop_com
-- ----------------------------
DROP TABLE IF EXISTS `order_master_shop_com`;
CREATE TABLE `order_master_shop_com` (
  `row_id` int(9) NOT NULL AUTO_INCREMENT,
  `order_id` int(9) unsigned NOT NULL,
  `rid` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`row_id`,`order_id`),
  KEY `order_id` (`order_id`)
) ENGINE=InnoDB AUTO_INCREMENT=29 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_master_status
-- ----------------------------
DROP TABLE IF EXISTS `order_master_status`;
CREATE TABLE `order_master_status` (
  `serial_id` bigint(19) unsigned NOT NULL,
  `order_id` int(9) unsigned NOT NULL,
  `order_status` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `status_description` varchar(255) DEFAULT NULL,
  `status_ipfrom` varchar(40) NOT NULL,
  `status_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`serial_id`),
  KEY `ix_order_master_status_oid` (`order_id`),
  CONSTRAINT `fk_order_master_status_oid` FOREIGN KEY (`order_id`) REFERENCES `order_master` (`order_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_money_return
-- ----------------------------
DROP TABLE IF EXISTS `order_money_return`;
CREATE TABLE `order_money_return` (
  `money_id` int(9) unsigned NOT NULL,
  `order_id` int(9) unsigned NOT NULL,
  `money_type` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `money_total` int(9) unsigned NOT NULL,
  `money_status` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `money_note` varchar(255) NOT NULL DEFAULT '',
  `money_source` varchar(255) NOT NULL DEFAULT '',
  `bank_name` varchar(20) NOT NULL DEFAULT '''''',
  `bank_branch` varchar(20) NOT NULL DEFAULT '''''',
  `bank_account` varchar(20) NOT NULL DEFAULT '''''',
  `bank_note` text COMMENT '銀行資訊',
  `account_name` varchar(20) NOT NULL DEFAULT '''''',
  `cs_note` varchar(255) DEFAULT NULL COMMENT '客服備註',
  `money_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `money_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `money_ipfrom` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`money_id`),
  KEY `ix_order_money_return_oid` (`order_id`),
  KEY `ix_order_money_return_status` (`money_status`),
  CONSTRAINT `fk_order_money_return_oid` FOREIGN KEY (`order_id`) REFERENCES `order_master` (`order_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_payment_alipay
-- ----------------------------
DROP TABLE IF EXISTS `order_payment_alipay`;
CREATE TABLE `order_payment_alipay` (
  `alipay_id` int(9) unsigned NOT NULL DEFAULT '0',
  `merchantnumber` varchar(50) NOT NULL DEFAULT '',
  `ordernumber` varchar(50) NOT NULL DEFAULT '',
  `serialnumber` varchar(50) NOT NULL DEFAULT '',
  `writeoffnumber` varchar(50) NOT NULL DEFAULT '',
  `timepaid` varchar(50) NOT NULL DEFAULT '',
  `paymenttype` varchar(50) NOT NULL DEFAULT '',
  `amount` varchar(50) NOT NULL DEFAULT '',
  `tel` varchar(50) NOT NULL DEFAULT '',
  `hash` varchar(50) NOT NULL DEFAULT '',
  `hash2` varchar(50) NOT NULL DEFAULT '',
  `error` tinyint(1) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`alipay_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_payment_all
-- ----------------------------
DROP TABLE IF EXISTS `order_payment_all`;
CREATE TABLE `order_payment_all` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `order_id` int(11) NOT NULL,
  `retstatus` tinyint(1) NOT NULL COMMENT '交易結果',
  `order_payment` tinyint(2) NOT NULL,
  `retcode` varchar(10) NOT NULL,
  `retcodename` varchar(255) DEFAULT NULL,
  `amount` int(12) NOT NULL,
  `bankname` varchar(100) DEFAULT NULL,
  `redem_discount_point` int(10) DEFAULT '0' COMMENT '本次折抵點數',
  `redem_discount_amount` int(10) DEFAULT '0' COMMENT '本次折抵金額',
  `redem_purchase_amount` int(10) DEFAULT '0' COMMENT '本次實付金額',
  `cap_date` datetime DEFAULT NULL COMMENT '請款日期',
  `set_time` datetime NOT NULL,
  `up_time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_payment_ct
-- ----------------------------
DROP TABLE IF EXISTS `order_payment_ct`;
CREATE TABLE `order_payment_ct` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `status` int(2) NOT NULL,
  `errcode` int(2) NOT NULL,
  `authcode` char(6) NOT NULL,
  `authamt` int(7) NOT NULL,
  `merid` int(11) NOT NULL,
  `lidm` int(11) NOT NULL,
  `originalamt` int(7) NOT NULL,
  `offsetamt` int(7) DEFAULT NULL,
  `utilizedpoint` int(7) DEFAULT NULL,
  `last4digitpan` int(4) NOT NULL,
  `errdesc` varchar(255) NOT NULL,
  `xid` varchar(40) NOT NULL,
  `awardedpoint` int(11) NOT NULL,
  `pointbalance` int(11) NOT NULL,
  `numberofpay` int(2) NOT NULL,
  `prodcode` int(2) NOT NULL,
  `checked` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `created` datetime NOT NULL,
  `modified` datetime NOT NULL,
  `ipfrom` varchar(40) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `lidm` (`lidm`)
) ENGINE=InnoDB AUTO_INCREMENT=10841 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_payment_cvs
-- ----------------------------
DROP TABLE IF EXISTS `order_payment_cvs`;
CREATE TABLE `order_payment_cvs` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ecno` varchar(3) NOT NULL COMMENT '網站代號',
  `stno` varchar(7) NOT NULL COMMENT '取貨門市編號',
  `order_id` int(11) NOT NULL,
  `deliver_id` int(11) NOT NULL,
  `status` varchar(1) NOT NULL COMMENT '結帳狀態碼',
  `ret_r` varchar(10) NOT NULL COMMENT '結帳狀態原因',
  `tradetype` varchar(1) NOT NULL COMMENT '交易方式識別碼',
  `tkdt` varchar(8) NOT NULL COMMENT '結帳基準日',
  `amt` int(5) NOT NULL COMMENT '代收金額',
  `realamt` int(5) NOT NULL COMMENT '商品實際金額',
  `fee` varchar(20) NOT NULL COMMENT '手續費',
  `set_time` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=709 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_payment_hitrust
-- ----------------------------
DROP TABLE IF EXISTS `order_payment_hitrust`;
CREATE TABLE `order_payment_hitrust` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `order_id` varchar(30) NOT NULL COMMENT '訂單編號',
  `retstatus` int(1) NOT NULL DEFAULT '0' COMMENT '交易結果，0：失敗、1：成功',
  `retcode` varchar(10) NOT NULL COMMENT '回傳的訊息編碼',
  `retcodename` varchar(255) DEFAULT NULL COMMENT 'retcode轉中文',
  `rettype` varchar(10) NOT NULL COMMENT '交易類別',
  `depositamount` int(10) DEFAULT NULL COMMENT '請款金額',
  `approveamount` int(12) DEFAULT NULL COMMENT '核准金額',
  `orderstatus` int(2) DEFAULT NULL COMMENT '訂單狀態碼',
  `authCode` varchar(6) DEFAULT NULL COMMENT '銀行授權碼',
  `pan` varchar(16) DEFAULT NULL COMMENT '卡號',
  `bankname` varchar(100) DEFAULT NULL COMMENT '卡別名稱',
  `eci` varchar(6) DEFAULT NULL COMMENT '授權方式',
  `authRRN` int(12) DEFAULT NULL COMMENT '銀行調單編號',
  `paybatchnumber` int(9) DEFAULT NULL COMMENT '請款批次號碼',
  `capDate` varchar(26) DEFAULT NULL COMMENT '請款日期',
  `credamount` int(10) DEFAULT NULL COMMENT '退款金額',
  `credbatchnumber` int(9) DEFAULT NULL COMMENT '退款批次號碼',
  `credRRN` int(12) DEFAULT NULL COMMENT '退款調單編號',
  `credCode` int(6) DEFAULT NULL COMMENT '退款授權碼',
  `creddate` varchar(26) DEFAULT NULL COMMENT '退款日期',
  `E09` varchar(12) DEFAULT NULL COMMENT '手續費',
  `redem_discount_point` varchar(50) DEFAULT NULL COMMENT '本次折抵點數',
  `redem_discount_amount` varchar(50) DEFAULT NULL COMMENT '本次折抵金額',
  `redem_purchase_amount` varchar(50) DEFAULT NULL COMMENT '本次實付金額',
  `redem_balance_point` varchar(50) DEFAULT NULL COMMENT '剩餘點數',
  `memo` varchar(255) DEFAULT NULL COMMENT '備註',
  `createtime` varchar(26) NOT NULL COMMENT '建立時間',
  `updatetime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '檔案修改時間',
  PRIMARY KEY (`id`),
  KEY `orderid` (`order_id`)
) ENGINE=InnoDB AUTO_INCREMENT=32475 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_payment_hncb
-- ----------------------------
DROP TABLE IF EXISTS `order_payment_hncb`;
CREATE TABLE `order_payment_hncb` (
  `hncb_id` varchar(32) NOT NULL DEFAULT '',
  `order_id` int(9) unsigned NOT NULL DEFAULT '0' COMMENT '付款單號',
  `bank` varchar(32) NOT NULL DEFAULT '0' COMMENT '客戶華南帳號',
  `entday` int(8) unsigned NOT NULL DEFAULT '0' COMMENT '入帳日期',
  `txtday` int(10) NOT NULL DEFAULT '0' COMMENT '交易日期',
  `sn` varchar(8) NOT NULL DEFAULT '' COMMENT '序號',
  `specific_currency` varchar(10) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL DEFAULT '' COMMENT '幣別',
  `paid` int(8) unsigned NOT NULL DEFAULT '0' COMMENT '交易金額',
  `type` int(1) unsigned NOT NULL DEFAULT '1' COMMENT '借貸別',
  `hncb_sn` int(32) unsigned NOT NULL DEFAULT '0' COMMENT '虛擬帳號',
  `outputbank` int(8) unsigned NOT NULL DEFAULT '0' COMMENT '轉出銀行',
  `pay_type` varchar(16) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL DEFAULT '' COMMENT '作業別',
  `e_date` int(8) unsigned NOT NULL DEFAULT '0' COMMENT '票繳日期',
  `note` varchar(32) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL DEFAULT '' COMMENT '備註',
  `vat_number` int(8) unsigned NOT NULL DEFAULT '0',
  `error` tinyint(2) unsigned zerofill NOT NULL DEFAULT '00' COMMENT '錯誤狀態 0無 1有',
  `msg` varchar(50) DEFAULT '' COMMENT '錯誤訊息',
  `createdate` int(10) NOT NULL DEFAULT '0',
  `updatedate` int(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`hncb_id`),
  KEY `inx_order_id` (`order_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_payment_nccc
-- ----------------------------
DROP TABLE IF EXISTS `order_payment_nccc`;
CREATE TABLE `order_payment_nccc` (
  `nccc_id` int(10) unsigned NOT NULL,
  `order_id` int(9) unsigned NOT NULL,
  `merchantid` varchar(10) DEFAULT NULL,
  `terminalid` varchar(8) DEFAULT NULL,
  `orderid` varchar(40) DEFAULT NULL,
  `pan` varchar(19) DEFAULT NULL,
  `bankname` varchar(100) DEFAULT NULL,
  `transcode` varchar(2) DEFAULT NULL,
  `transmode` varchar(1) DEFAULT NULL,
  `transdate` varchar(8) DEFAULT NULL,
  `transtime` varchar(6) DEFAULT NULL,
  `transamt` varchar(8) DEFAULT NULL,
  `approvecode` varchar(8) DEFAULT NULL,
  `responsecode` varchar(3) DEFAULT NULL,
  `responsemsg` varchar(60) DEFAULT NULL,
  `installtype` varchar(1) DEFAULT NULL,
  `install` varchar(2) DEFAULT NULL,
  `firstamt` varchar(8) DEFAULT NULL,
  `eachamt` varchar(8) DEFAULT NULL,
  `fee` varchar(8) DEFAULT NULL,
  `redeemtype` varchar(1) DEFAULT NULL,
  `redeemused` varchar(8) DEFAULT NULL,
  `redeembalance` varchar(8) DEFAULT NULL,
  `creditamt` varchar(8) DEFAULT NULL,
  `riskmark` varchar(1) DEFAULT NULL,
  `foreign1` varchar(1) DEFAULT NULL,
  `secure_status` varchar(1) DEFAULT NULL,
  `nccc_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `nccc_ipfrom` varchar(40) NOT NULL DEFAULT '',
  `post_data` text,
  PRIMARY KEY (`nccc_id`),
  KEY `ix_order_payment_nccc_oid` (`order_id`),
  CONSTRAINT `fk_order_payment_nccc_oid` FOREIGN KEY (`order_id`) REFERENCES `order_master` (`order_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_payment_sinopac
-- ----------------------------
DROP TABLE IF EXISTS `order_payment_sinopac`;
CREATE TABLE `order_payment_sinopac` (
  `sinopac_id` varchar(20) NOT NULL,
  `order_id` int(9) unsigned NOT NULL,
  `sinopac_entday` int(10) unsigned NOT NULL DEFAULT '0',
  `sinopac_txday` int(10) unsigned NOT NULL DEFAULT '0',
  `sinopac_txtime` int(10) unsigned NOT NULL DEFAULT '0',
  `sinopac_txamt` int(10) unsigned NOT NULL DEFAULT '0',
  `product_status` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `sinopac_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `sinopac_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`sinopac_id`),
  KEY `ix_order_payment_sinopac_oid` (`order_id`),
  CONSTRAINT `fk_order_payment_sinopac_oid` FOREIGN KEY (`order_id`) REFERENCES `order_master` (`order_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_payment_union_pay
-- ----------------------------
DROP TABLE IF EXISTS `order_payment_union_pay`;
CREATE TABLE `order_payment_union_pay` (
  `union_id` int(9) unsigned NOT NULL DEFAULT '0',
  `transtype` varchar(10) NOT NULL DEFAULT '',
  `respcode` varchar(10) NOT NULL DEFAULT '',
  `order_id` int(9) unsigned NOT NULL DEFAULT '0',
  `respmsg` varchar(60) NOT NULL DEFAULT '',
  `merabbr` varchar(25) NOT NULL DEFAULT '',
  `merid` varchar(15) NOT NULL DEFAULT '',
  `orderamount` varchar(12) NOT NULL DEFAULT '',
  `ordercurrency` varchar(3) NOT NULL DEFAULT '',
  `resptime` varchar(14) NOT NULL DEFAULT '',
  `cupReserved` varchar(60) NOT NULL DEFAULT '',
  `union_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `union_ipfrom` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`union_id`,`transtype`,`respcode`,`order_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_question
-- ----------------------------
DROP TABLE IF EXISTS `order_question`;
CREATE TABLE `order_question` (
  `question_id` int(9) unsigned NOT NULL,
  `order_id` int(9) unsigned NOT NULL,
  `question_username` varchar(64) NOT NULL DEFAULT '',
  `question_phone` varchar(50) NOT NULL,
  `question_email` varchar(255) NOT NULL DEFAULT '',
  `question_type` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `question_reply` varchar(10) DEFAULT '0|0|0' COMMENT '回覆方式-0：不使用，1：使用(信箱 | 簡訊 | 電話)',
  `question_reply_time` int(1) NOT NULL DEFAULT '0' COMMENT '回覆時間',
  `question_status` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `question_content` text NOT NULL,
  `question_ipfrom` varchar(40) NOT NULL DEFAULT '',
  `question_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `question_file` varchar(50) DEFAULT '',
  PRIMARY KEY (`question_id`),
  KEY `ix_order_question_order_id` (`order_id`),
  KEY `ix_order_question_status` (`question_status`),
  CONSTRAINT `fk_order_question_oid` FOREIGN KEY (`order_id`) REFERENCES `order_master` (`order_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_response
-- ----------------------------
DROP TABLE IF EXISTS `order_response`;
CREATE TABLE `order_response` (
  `response_id` int(9) unsigned NOT NULL,
  `question_id` int(9) unsigned NOT NULL,
  `user_id` smallint(4) unsigned NOT NULL,
  `response_content` text NOT NULL,
  `response_ipfrom` varchar(40) NOT NULL DEFAULT '',
  `response_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `response_type` int(4) DEFAULT NULL,
  PRIMARY KEY (`response_id`),
  KEY `ix_order_response_qid` (`question_id`),
  KEY `ix_order_response_uid` (`user_id`),
  CONSTRAINT `fk_order_response_qid` FOREIGN KEY (`question_id`) REFERENCES `order_question` (`question_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_order_response_uid` FOREIGN KEY (`user_id`) REFERENCES `manage_user` (`user_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_return_content
-- ----------------------------
DROP TABLE IF EXISTS `order_return_content`;
CREATE TABLE `order_return_content` (
  `orc_id` int(9) NOT NULL AUTO_INCREMENT COMMENT '訂單退貨內容流水號',
  `return_id` int(9) unsigned NOT NULL COMMENT '關聯order_return_master',
  `orc_order_id` int(9) unsigned NOT NULL COMMENT '訂單編號',
  `orc_deliver_code` varchar(50) NOT NULL COMMENT '物流單號',
  `orc_deliver_date` datetime NOT NULL COMMENT '訂單物流收貨日期',
  `orc_deliver_time` varchar(10) NOT NULL COMMENT '收貨時間段(上午/下午/晚上/隨時)',
  `orc_name` varchar(10) NOT NULL COMMENT '收貨人姓名',
  `orc_phone` varchar(30) NOT NULL COMMENT '收貨電話',
  `orc_zipcode` varchar(510) NOT NULL COMMENT 't_zip_code中的zipcode',
  `orc_address` varchar(255) NOT NULL COMMENT '收貨地址',
  `orc_remark` varchar(255) DEFAULT NULL COMMENT '備註',
  `orc_type` int(11) DEFAULT NULL COMMENT '退貨類型',
  `orc_service_remark` varchar(255) DEFAULT NULL COMMENT '客服備註',
  `orc_send` int(2) NOT NULL DEFAULT '1' COMMENT '0 未發貨 1已發貨',
  PRIMARY KEY (`orc_id`)
) ENGINE=InnoDB AUTO_INCREMENT=55 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_return_detail
-- ----------------------------
DROP TABLE IF EXISTS `order_return_detail`;
CREATE TABLE `order_return_detail` (
  `return_id` int(9) unsigned NOT NULL,
  `detail_id` int(9) unsigned NOT NULL,
  PRIMARY KEY (`return_id`,`detail_id`),
  KEY `fk_order_return_detail_did` (`detail_id`),
  CONSTRAINT `fk_order_return_detail_did` FOREIGN KEY (`detail_id`) REFERENCES `order_detail` (`detail_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_order_return_detail_rid` FOREIGN KEY (`return_id`) REFERENCES `order_return_master` (`return_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_return_master
-- ----------------------------
DROP TABLE IF EXISTS `order_return_master`;
CREATE TABLE `order_return_master` (
  `return_id` int(9) unsigned NOT NULL,
  `order_id` int(9) unsigned NOT NULL,
  `vendor_id` int(9) unsigned NOT NULL,
  `return_status` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `return_note` varchar(255) NOT NULL DEFAULT '',
  `user_note` text COMMENT '給user備註',
  `bank_note` text COMMENT '銀行資訊',
  `invoice_deal` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `package` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `return_zip` mediumint(7) unsigned NOT NULL DEFAULT '0',
  `return_address` varchar(255) NOT NULL DEFAULT '''''',
  `deliver_code` varchar(50) DEFAULT '''''' COMMENT '退貨單物流單號',
  `return_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `return_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `return_ipfrom` varchar(40) NOT NULL DEFAULT '',
  `return_reason` tinyint(1) NOT NULL DEFAULT '1',
  `bank_name` varchar(20) NOT NULL DEFAULT '',
  `bank_branch` varchar(20) NOT NULL DEFAULT '',
  `bank_account` varchar(20) NOT NULL DEFAULT '',
  `account_name` varchar(20) NOT NULL DEFAULT '',
  PRIMARY KEY (`return_id`),
  KEY `ix_order_return_master_oid` (`order_id`),
  KEY `ix_order_return_master_vid` (`vendor_id`),
  KEY `ix_order_return_master_status` (`return_status`),
  CONSTRAINT `fk_order_return_master_oid` FOREIGN KEY (`order_id`) REFERENCES `order_master` (`order_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_order_return_master_vid` FOREIGN KEY (`vendor_id`) REFERENCES `vendor` (`vendor_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_return_status
-- ----------------------------
DROP TABLE IF EXISTS `order_return_status`;
CREATE TABLE `order_return_status` (
  `ors_id` int(9) NOT NULL AUTO_INCREMENT COMMENT '退貨單狀態記錄流水號',
  `return_id` int(9) unsigned NOT NULL COMMENT '關聯order_return_master',
  `ors_order_id` int(9) unsigned NOT NULL COMMENT '訂單標號',
  `ors_status` int(9) NOT NULL COMMENT '退貨狀態',
  `ors_remark` varchar(255) DEFAULT NULL COMMENT '退貨狀態備註',
  `ors_createdate` datetime NOT NULL COMMENT '退貨狀態時間',
  `ors_createuser` int(5) NOT NULL COMMENT '退貨狀態創建人',
  PRIMARY KEY (`ors_id`)
) ENGINE=InnoDB AUTO_INCREMENT=232 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_return_user
-- ----------------------------
DROP TABLE IF EXISTS `order_return_user`;
CREATE TABLE `order_return_user` (
  `user_return_id` int(9) unsigned NOT NULL,
  `detail_id` int(9) unsigned NOT NULL,
  `return_reason` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `gift` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `temp_status` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `user_note` varchar(255) NOT NULL DEFAULT '',
  `return_zip` mediumint(7) unsigned NOT NULL DEFAULT '0',
  `return_address` varchar(255) NOT NULL DEFAULT '',
  `bank_name` varchar(20) NOT NULL DEFAULT '',
  `bank_branch` varchar(20) NOT NULL DEFAULT '',
  `bank_account` varchar(20) NOT NULL DEFAULT '',
  `account_name` varchar(20) NOT NULL DEFAULT '',
  `user_return_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `user_return_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `user_return_ipfrom` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`user_return_id`,`detail_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_slave
-- ----------------------------
DROP TABLE IF EXISTS `order_slave`;
CREATE TABLE `order_slave` (
  `slave_id` int(9) unsigned NOT NULL,
  `order_id` int(9) unsigned NOT NULL,
  `vendor_id` int(9) unsigned NOT NULL COMMENT '供應商編號',
  `slave_freight_normal` int(9) unsigned NOT NULL DEFAULT '0',
  `slave_freight_low` int(9) unsigned NOT NULL DEFAULT '0',
  `slave_product_subtotal` int(9) unsigned NOT NULL DEFAULT '0',
  `slave_amount` int(9) unsigned NOT NULL DEFAULT '0',
  `slave_status` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `slave_note` varchar(255) NOT NULL DEFAULT '',
  `slave_date_delivery` int(10) unsigned NOT NULL DEFAULT '0',
  `slave_date_cancel` int(10) unsigned NOT NULL DEFAULT '0',
  `slave_date_return` int(10) unsigned NOT NULL DEFAULT '0',
  `slave_date_close` int(10) unsigned NOT NULL DEFAULT '0',
  `account_status` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `slave_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `slave_ipfrom` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`slave_id`),
  KEY `ix_order_slave_oid` (`order_id`),
  KEY `ix_order_slave_vid` (`vendor_id`),
  KEY `ix_order_slave_ss` (`slave_status`),
  KEY `ix_order_slave_dd` (`slave_date_delivery`),
  KEY `ix_order_slave_dcl` (`slave_date_cancel`),
  KEY `ix_order_slave_dr` (`slave_date_return`),
  KEY `ix_order_slave_dce` (`slave_date_close`),
  KEY `ix_order_slave_du` (`slave_updatedate`),
  KEY `ix_order_slave_ass` (`account_status`),
  CONSTRAINT `fk_order_slave_oid` FOREIGN KEY (`order_id`) REFERENCES `order_master` (`order_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_order_slave_vid` FOREIGN KEY (`vendor_id`) REFERENCES `vendor` (`vendor_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_slave_detail
-- ----------------------------
DROP TABLE IF EXISTS `order_slave_detail`;
CREATE TABLE `order_slave_detail` (
  `slave_master_id` int(9) unsigned NOT NULL DEFAULT '0',
  `slave_id` int(9) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`slave_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_slave_master
-- ----------------------------
DROP TABLE IF EXISTS `order_slave_master`;
CREATE TABLE `order_slave_master` (
  `slave_master_id` int(9) unsigned NOT NULL DEFAULT '0',
  `code_num` varchar(25) CHARACTER SET armscii8 NOT NULL DEFAULT '''''',
  `paper` int(9) unsigned NOT NULL DEFAULT '0',
  `order_freight_normal` int(9) unsigned NOT NULL DEFAULT '0',
  `order_freight_low` int(9) unsigned NOT NULL DEFAULT '0',
  `normal_subtotal` int(9) unsigned NOT NULL DEFAULT '0',
  `hypothermia_subtotal` int(9) unsigned NOT NULL DEFAULT '0',
  `deliver_store` smallint(4) unsigned NOT NULL DEFAULT '0',
  `deliver_code` varchar(50) NOT NULL DEFAULT '',
  `deliver_time` int(9) unsigned NOT NULL DEFAULT '0',
  `deliver_note` varchar(255) NOT NULL DEFAULT '',
  `createdate` int(9) unsigned NOT NULL DEFAULT '0',
  `creator` int(9) unsigned NOT NULL DEFAULT '0',
  `on_check` tinyint(1) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`slave_master_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for order_slave_status
-- ----------------------------
DROP TABLE IF EXISTS `order_slave_status`;
CREATE TABLE `order_slave_status` (
  `serial_id` bigint(19) unsigned NOT NULL,
  `slave_id` int(9) unsigned NOT NULL,
  `order_status` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `status_description` varchar(255) DEFAULT NULL,
  `status_ipfrom` varchar(40) NOT NULL,
  `status_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`serial_id`),
  KEY `ix_order_slave_status_sid` (`slave_id`),
  CONSTRAINT `fk_order_slave_status_oid` FOREIGN KEY (`slave_id`) REFERENCES `order_slave` (`slave_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for outer_customer
-- ----------------------------
DROP TABLE IF EXISTS `outer_customer`;
CREATE TABLE `outer_customer` (
  `customer_id` int(9) NOT NULL AUTO_INCREMENT,
  `customer_email` varchar(100) NOT NULL,
  PRIMARY KEY (`customer_id`)
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for outer_edm_subscription
-- ----------------------------
DROP TABLE IF EXISTS `outer_edm_subscription`;
CREATE TABLE `outer_edm_subscription` (
  `customer_id` int(9) NOT NULL,
  `group_id` int(9) NOT NULL,
  PRIMARY KEY (`customer_id`,`group_id`),
  KEY `f_group_id` (`group_id`),
  CONSTRAINT `f_customer_id` FOREIGN KEY (`customer_id`) REFERENCES `outer_customer` (`customer_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `f_group_id` FOREIGN KEY (`group_id`) REFERENCES `edm_group_new` (`group_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for page_area
-- ----------------------------
DROP TABLE IF EXISTS `page_area`;
CREATE TABLE `page_area` (
  `area_id` int(9) NOT NULL AUTO_INCREMENT,
  `area_name` varchar(50) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `area_status` int(4) NOT NULL DEFAULT '0',
  `area_desc` varchar(100) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `area_element_id` varchar(50) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `show_number` int(4) NOT NULL DEFAULT '0',
  `area_createdate` datetime NOT NULL,
  `area_updatedate` datetime DEFAULT NULL,
  `create_userid` int(9) NOT NULL,
  `update_userid` int(9) DEFAULT NULL,
  `element_type` int(9) DEFAULT '0',
  PRIMARY KEY (`area_id`),
  KEY `area_id` (`area_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=141 DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for page_error_log
-- ----------------------------
DROP TABLE IF EXISTS `page_error_log`;
CREATE TABLE `page_error_log` (
  `rowID` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水編號',
  `error_page_url` varchar(255) NOT NULL COMMENT '錯誤頁面地址',
  `error_type` int(9) NOT NULL COMMENT '錯誤類型',
  `create_date` datetime NOT NULL COMMENT '訪問時間',
  `create_ip` varchar(50) NOT NULL COMMENT '訪問IP',
  PRIMARY KEY (`rowID`)
) ENGINE=InnoDB AUTO_INCREMENT=87539 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for page_metadata
-- ----------------------------
DROP TABLE IF EXISTS `page_metadata`;
CREATE TABLE `page_metadata` (
  `pm_id` int(11) NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `pm_url_para` varchar(50) NOT NULL COMMENT '網頁參數',
  `pm_page_name` varchar(255) NOT NULL COMMENT '顯示名稱',
  `pm_title` varchar(255) NOT NULL COMMENT '頁面title',
  `pm_keywords` varchar(255) NOT NULL COMMENT '頁面keywords',
  `pm_description` varchar(255) NOT NULL COMMENT '頁面description',
  `pm_created` datetime NOT NULL COMMENT '建立時間',
  `pm_modified` datetime NOT NULL COMMENT '修改時間',
  `pm_modify_user` int(9) NOT NULL COMMENT '修改人',
  `pm_create_user` int(9) NOT NULL COMMENT '創建人',
  PRIMARY KEY (`pm_id`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8 COMMENT='頁面meta-data資料表';

-- ----------------------------
-- Table structure for paper
-- ----------------------------
DROP TABLE IF EXISTS `paper`;
CREATE TABLE `paper` (
  `paperID` int(11) NOT NULL AUTO_INCREMENT COMMENT '問卷編號',
  `paperName` varchar(100) DEFAULT NULL COMMENT '問卷名稱',
  `paperMemo` varchar(2000) DEFAULT NULL COMMENT '問卷備註',
  `paperBanner` varchar(100) DEFAULT NULL COMMENT '問卷banner',
  `bannerUrl` varchar(200) DEFAULT NULL COMMENT 'banner指向地址',
  `isRepeatWrite` int(1) DEFAULT '0' COMMENT '是否可重複填寫 0:否 1:是',
  `event_ID` varchar(10) DEFAULT NULL COMMENT '問卷關聯促銷地址',
  `isRepeatGift` int(1) DEFAULT '0' COMMENT '是否關聯促銷 0:否 1:是',
  `isNewMember` int(1) DEFAULT '0' COMMENT '是否為新會員填寫 0否 1是',
  `paperStart` datetime DEFAULT NULL COMMENT '問卷有效開始時間',
  `paperEnd` datetime DEFAULT NULL COMMENT '問卷有效結束時間',
  `status` int(1) DEFAULT '0' COMMENT '是否生效',
  `creator` int(10) DEFAULT NULL COMMENT '創建人',
  `created` datetime DEFAULT NULL COMMENT '創建時間',
  `modifier` int(10) DEFAULT NULL COMMENT '修改人',
  `modified` datetime DEFAULT NULL COMMENT '修改時間',
  `ipfrom` varchar(50) DEFAULT NULL COMMENT '來源ip',
  PRIMARY KEY (`paperID`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for paper_answer
-- ----------------------------
DROP TABLE IF EXISTS `paper_answer`;
CREATE TABLE `paper_answer` (
  `answerID` int(11) NOT NULL AUTO_INCREMENT,
  `paperID` int(11) DEFAULT NULL,
  `userid` int(9) DEFAULT NULL,
  `userMail` varchar(100) NOT NULL,
  `order_id` int(9) DEFAULT NULL COMMENT '訂單編號',
  `classID` int(10) DEFAULT NULL,
  `answerContent` varchar(255) DEFAULT NULL,
  `classType` varchar(255) DEFAULT NULL COMMENT '題目類型 SL:單行,ML:多行,SC:單選,MC:多選',
  `answerDate` datetime DEFAULT NULL,
  PRIMARY KEY (`answerID`)
) ENGINE=InnoDB AUTO_INCREMENT=52357 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for paper_class
-- ----------------------------
DROP TABLE IF EXISTS `paper_class`;
CREATE TABLE `paper_class` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `paperID` int(11) NOT NULL COMMENT '問卷編號',
  `classID` int(10) NOT NULL COMMENT '題目編號',
  `className` varchar(200) NOT NULL COMMENT '題目名稱',
  `classType` varchar(50) NOT NULL COMMENT '題目類型 SL:單行,ML:多行,SC:單選,MC:多選',
  `projectNum` int(4) DEFAULT NULL COMMENT '題目順序',
  `classContent` varchar(200) NOT NULL COMMENT '選項內容',
  `orderNum` int(4) DEFAULT NULL COMMENT '選項排序',
  `isMust` int(4) NOT NULL COMMENT '是否必填 0 否 1是',
  `status` int(1) NOT NULL COMMENT '是否啟用',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=83 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for payment_type
-- ----------------------------
DROP TABLE IF EXISTS `payment_type`;
CREATE TABLE `payment_type` (
  `rid` int(11) NOT NULL AUTO_INCREMENT,
  `payment_name` varchar(50) NOT NULL COMMENT '付款方式名稱',
  `payment_code` varchar(10) NOT NULL COMMENT '付款方式1.ATM 2.信用卡 3.happyGO 4.紅利',
  `bank_id` varchar(10) DEFAULT NULL COMMENT '銀行ID',
  `kdate` datetime DEFAULT NULL,
  `kuser` varchar(50) DEFAULT NULL,
  `mdate` datetime DEFAULT NULL,
  `muser` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`rid`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for Pickup
-- ----------------------------
DROP TABLE IF EXISTS `Pickup`;
CREATE TABLE `Pickup` (
  `Pick001` varchar(20) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL COMMENT '出貨單號',
  `Pick002` smallint(5) DEFAULT NULL COMMENT '箱數',
  `Pick003` varchar(5) DEFAULT NULL COMMENT '狀態',
  `Pick004` varchar(1) DEFAULT NULL COMMENT '溫層',
  `Pick005` datetime DEFAULT NULL COMMENT 'UPT',
  `Pick006` varchar(20) DEFAULT NULL COMMENT 'UPU',
  `De000` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`Pick001`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for price_master
-- ----------------------------
DROP TABLE IF EXISTS `price_master`;
CREATE TABLE `price_master` (
  `price_master_id` int(9) unsigned NOT NULL AUTO_INCREMENT,
  `product_id` int(9) unsigned NOT NULL,
  `site_id` int(4) unsigned NOT NULL DEFAULT '1' COMMENT '站台',
  `user_level` smallint(2) unsigned NOT NULL DEFAULT '1' COMMENT '會員等級 (1~5)',
  `user_id` int(9) unsigned NOT NULL DEFAULT '0' COMMENT '會員id',
  `product_name` varchar(255) NOT NULL COMMENT '賣場商品名稱',
  `accumulated_bonus` smallint(2) unsigned NOT NULL DEFAULT '1' COMMENT '是否發放購物金 0:No, 1:Yes',
  `bonus_percent` float unsigned NOT NULL DEFAULT '0' COMMENT '活動購物金回饋倍數',
  `default_bonus_percent` float unsigned NOT NULL DEFAULT '0' COMMENT '預設回饋倍數',
  `bonus_percent_start` int(10) unsigned DEFAULT '0',
  `bonus_percent_end` int(10) unsigned DEFAULT '0',
  `same_price` smallint(1) NOT NULL DEFAULT '1' COMMENT '所有product_item是否同價,此為方便商品新增/修改價錢頁面用 1:相同 0:不同',
  `event_start` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '活動開始時間',
  `event_end` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '活動結束時間',
  `price_status` smallint(1) unsigned NOT NULL DEFAULT '2' COMMENT '價格狀態  1:上架 2:申請審核 3:申請駁回 4:下架',
  `price` int(9) NOT NULL DEFAULT '0' COMMENT '售價',
  `event_price` int(9) NOT NULL DEFAULT '0' COMMENT '活動售價',
  `child_id` int(9) NOT NULL DEFAULT '0',
  `apply_id` int(9) unsigned NOT NULL DEFAULT '0' COMMENT '價格修改申請ID',
  `cost` int(9) NOT NULL DEFAULT '0' COMMENT '成本',
  `event_cost` int(9) NOT NULL DEFAULT '0' COMMENT '活動成本',
  `max_price` int(9) NOT NULL DEFAULT '0',
  `max_event_price` int(9) NOT NULL DEFAULT '0',
  `valid_start` int(10) NOT NULL DEFAULT '0' COMMENT '價格有效時間起',
  `valid_end` int(10) NOT NULL DEFAULT '0' COMMENT '價格有效時間迄',
  PRIMARY KEY (`price_master_id`),
  KEY `pmid` (`product_id`,`site_id`,`user_level`,`user_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=17713 DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT COMMENT='商品細項價格';

-- ----------------------------
-- Table structure for price_master_temp
-- ----------------------------
DROP TABLE IF EXISTS `price_master_temp`;
CREATE TABLE `price_master_temp` (
  `writer_id` int(9) unsigned NOT NULL,
  `price_master_id` int(9) unsigned NOT NULL AUTO_INCREMENT,
  `product_id` varchar(10) NOT NULL DEFAULT '0',
  `site_id` int(4) unsigned NOT NULL DEFAULT '1',
  `user_level` smallint(2) unsigned NOT NULL DEFAULT '1' COMMENT '會員等級 (1~5)',
  `user_id` int(9) unsigned NOT NULL DEFAULT '0' COMMENT '會員id',
  `product_name` varchar(255) NOT NULL COMMENT '賣場商品名稱',
  `accumulated_bonus` smallint(2) unsigned NOT NULL DEFAULT '1' COMMENT '是否發放購物金 0:No, 1:Yes',
  `bonus_percent` float unsigned NOT NULL DEFAULT '0' COMMENT '活動購物金回饋百分比',
  `default_bonus_percent` float unsigned NOT NULL DEFAULT '0' COMMENT '預設回饋百分比',
  `bonus_percent_start` int(10) unsigned DEFAULT '0',
  `bonus_percent_end` int(10) unsigned DEFAULT '0',
  `same_price` smallint(1) NOT NULL DEFAULT '1' COMMENT '所有product_item是否同價,此為方便商品新增/修改價錢頁面用 1:相同 0:不同',
  `event_start` int(10) unsigned NOT NULL DEFAULT '0',
  `event_end` int(10) unsigned NOT NULL DEFAULT '0',
  `price_status` smallint(1) unsigned NOT NULL DEFAULT '2',
  `price` int(9) NOT NULL DEFAULT '0',
  `event_price` int(9) NOT NULL DEFAULT '0',
  `child_id` varchar(10) NOT NULL DEFAULT '0',
  `combo_type` int(9) DEFAULT NULL,
  `cost` int(9) NOT NULL DEFAULT '0',
  `event_cost` int(9) NOT NULL DEFAULT '0',
  `max_price` int(9) NOT NULL DEFAULT '0',
  `max_event_price` int(9) NOT NULL DEFAULT '0',
  `valid_start` int(10) NOT NULL DEFAULT '0',
  `valid_end` int(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`price_master_id`)
) ENGINE=InnoDB AUTO_INCREMENT=5518 DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT COMMENT='商品細項價格';

-- ----------------------------
-- Table structure for price_master_ts
-- ----------------------------
DROP TABLE IF EXISTS `price_master_ts`;
CREATE TABLE `price_master_ts` (
  `price_master_id` int(9) unsigned NOT NULL,
  `product_id` int(9) unsigned NOT NULL,
  `site_id` int(4) unsigned NOT NULL DEFAULT '1' COMMENT '站台',
  `user_level` smallint(2) unsigned NOT NULL DEFAULT '1' COMMENT '會員等級 (1~5)',
  `user_id` int(9) unsigned NOT NULL DEFAULT '0' COMMENT '會員id',
  `product_name` varchar(255) NOT NULL COMMENT '賣場商品名稱',
  `accumulated_bonus` smallint(2) unsigned NOT NULL DEFAULT '1' COMMENT '是否發放購物金 0:No, 1:Yes',
  `bonus_percent` float unsigned NOT NULL DEFAULT '0' COMMENT '活動購物金回饋倍數',
  `default_bonus_percent` float unsigned NOT NULL DEFAULT '0' COMMENT '預設回饋倍數',
  `bonus_percent_start` int(10) unsigned DEFAULT '0',
  `bonus_percent_end` int(10) unsigned DEFAULT '0',
  `same_price` smallint(1) NOT NULL DEFAULT '1' COMMENT '所有product_item是否同價,此為方便商品新增/修改價錢頁面用 1:相同 0:不同',
  `event_start` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '活動開始時間',
  `event_end` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '活動結束時間',
  `price_status` smallint(1) unsigned NOT NULL DEFAULT '2' COMMENT '價格狀態',
  `price` int(9) NOT NULL DEFAULT '0' COMMENT '售價',
  `event_price` int(9) NOT NULL DEFAULT '0' COMMENT '活動售價',
  `child_id` int(9) NOT NULL DEFAULT '0',
  `apply_id` int(9) unsigned NOT NULL DEFAULT '0' COMMENT '價格修改申請ID',
  `cost` int(9) NOT NULL DEFAULT '0' COMMENT '成本',
  `event_cost` int(9) NOT NULL DEFAULT '0' COMMENT '活動成本',
  `max_price` int(9) NOT NULL DEFAULT '0',
  `max_event_price` int(9) NOT NULL DEFAULT '0',
  `valid_start` int(10) NOT NULL DEFAULT '0' COMMENT '價格有效時間起',
  `valid_end` int(10) NOT NULL DEFAULT '0' COMMENT '價格有效時間迄',
  PRIMARY KEY (`apply_id`,`price_master_id`),
  KEY `pmid` (`product_id`,`site_id`,`user_level`,`user_id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT COMMENT='商品細項價格';

-- ----------------------------
-- Table structure for price_update_apply
-- ----------------------------
DROP TABLE IF EXISTS `price_update_apply`;
CREATE TABLE `price_update_apply` (
  `apply_id` int(9) unsigned NOT NULL AUTO_INCREMENT,
  `price_master_id` int(9) unsigned NOT NULL,
  `apply_time` datetime NOT NULL COMMENT '申請時間',
  `apply_user` smallint(4) unsigned NOT NULL COMMENT '申請人',
  PRIMARY KEY (`apply_id`)
) ENGINE=InnoDB AUTO_INCREMENT=30428 DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT COMMENT='價格修改申請審核';

-- ----------------------------
-- Table structure for price_update_apply_history
-- ----------------------------
DROP TABLE IF EXISTS `price_update_apply_history`;
CREATE TABLE `price_update_apply_history` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `apply_id` int(9) unsigned NOT NULL COMMENT '申請ID',
  `user_id` smallint(4) unsigned NOT NULL COMMENT '操作人員',
  `create_time` datetime NOT NULL COMMENT '操作時間',
  `price_status` tinyint(2) unsigned DEFAULT NULL COMMENT '操作後價格狀態  1:上架 2:申請審核 3:申請駁回 4:下架. parameterType=price_status',
  `type` tinyint(2) unsigned DEFAULT NULL COMMENT '操作類型  1:申請審核 2:核可 3:駁回 4:下架 5:新建商品 6:上架 7:系統移轉建立 8:取消送審. parameterType=verify_operate_type',
  `remark` varchar(255) DEFAULT NULL COMMENT '備註',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=60044 DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT COMMENT='價格審核詳細記錄';

-- ----------------------------
-- Table structure for prod_promo
-- ----------------------------
DROP TABLE IF EXISTS `prod_promo`;
CREATE TABLE `prod_promo` (
  `rid` int(11) NOT NULL AUTO_INCREMENT,
  `product_id` int(9) NOT NULL COMMENT '商品編號',
  `event_id` varchar(11) CHARACTER SET utf8 NOT NULL COMMENT '活動id',
  `event_type` varchar(10) CHARACTER SET utf8 DEFAULT NULL COMMENT '活動類型',
  `event_desc` varchar(100) CHARACTER SET utf8 DEFAULT NULL COMMENT '活動描述',
  `start` datetime DEFAULT NULL COMMENT '活動開始時間',
  `end` datetime DEFAULT NULL COMMENT '活動結束時間',
  `page_url` varchar(255) CHARACTER SET utf8 DEFAULT NULL COMMENT '專區連接',
  `user_specified` tinyint(2) DEFAULT '0' COMMENT '是否綁定身份 0:否  1:是',
  `kuser` varchar(50) CHARACTER SET utf8 DEFAULT NULL,
  `kdate` datetime DEFAULT NULL,
  `muser` varchar(50) CHARACTER SET utf8 DEFAULT NULL,
  `mdate` datetime DEFAULT NULL,
  `status` int(9) DEFAULT '1',
  PRIMARY KEY (`rid`)
) ENGINE=InnoDB AUTO_INCREMENT=19675 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for prod_vd_req
-- ----------------------------
DROP TABLE IF EXISTS `prod_vd_req`;
CREATE TABLE `prod_vd_req` (
  `rid` int(11) NOT NULL AUTO_INCREMENT,
  `vendor_id` int(9) DEFAULT NULL COMMENT '申請供應商ID',
  `product_id` int(9) DEFAULT NULL COMMENT '申請之上品編號',
  `req_status` tinyint(2) DEFAULT NULL COMMENT '申請處理狀態 1:申請中 2:已完成 3:不更動',
  `req_datatime` datetime DEFAULT NULL COMMENT '申請時間',
  `explain` text COMMENT '申請說明',
  `req_type` tinyint(2) DEFAULT NULL COMMENT '申請類型 1:申請上架 2:申請下架',
  `user_id` int(9) DEFAULT NULL COMMENT '處理之管理人員ID',
  `reply_datetime` datetime DEFAULT NULL COMMENT '處理完成時間',
  `reply_note` varchar(200) DEFAULT NULL COMMENT '處理說明',
  PRIMARY KEY (`rid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for product
-- ----------------------------
DROP TABLE IF EXISTS `product`;
CREATE TABLE `product` (
  `product_id` int(9) unsigned NOT NULL,
  `brand_id` int(9) unsigned NOT NULL,
  `product_vendor_code` varchar(20) NOT NULL DEFAULT '',
  `product_name` varchar(255) NOT NULL DEFAULT '' COMMENT '商品名稱',
  `product_price_list` mediumint(7) unsigned NOT NULL DEFAULT '0' COMMENT '建議售價',
  `product_spec` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `spec_title_1` varchar(50) NOT NULL DEFAULT '',
  `spec_title_2` varchar(50) NOT NULL DEFAULT '',
  `product_freight_set` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `product_buy_limit` mediumint(6) unsigned NOT NULL DEFAULT '10',
  `product_status` tinyint(2) unsigned NOT NULL DEFAULT '0' COMMENT '商品狀態',
  `product_hide` tinyint(1) NOT NULL DEFAULT '0',
  `product_mode` tinyint(2) unsigned NOT NULL DEFAULT '1' COMMENT '貨運模式',
  `product_sort` smallint(4) unsigned NOT NULL DEFAULT '0',
  `product_start` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '上架時間',
  `product_end` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '下架時間',
  `page_content_1` text NOT NULL,
  `page_content_2` text NOT NULL COMMENT '商品規格',
  `page_content_3` text NOT NULL,
  `product_keywords` varchar(255) NOT NULL DEFAULT '',
  `fortune_quota` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '抽獎名額',
  `product_recommend` int(10) unsigned NOT NULL DEFAULT '0',
  `product_password` varchar(32) NOT NULL DEFAULT '',
  `product_total_click` int(10) unsigned NOT NULL DEFAULT '0',
  `expect_time` int(10) unsigned NOT NULL DEFAULT '0',
  `expect_msg` varchar(80) NOT NULL DEFAULT '',
  `product_image` varchar(40) NOT NULL DEFAULT '',
  `product_alt` varchar(255) DEFAULT NULL COMMENT '商品浮動說明/HTML ALT屬性',
  `product_detail_text` text COMMENT '商品詳情文字',
  `detail_created` int(9) DEFAULT NULL COMMENT '商品詳情文字建立人',
  `detail_createdate` datetime DEFAULT NULL COMMENT '商品詳情文字創建時間',
  `detail_update` int(9) DEFAULT NULL COMMENT '商品文字詳情變更人',
  `detail_updatedate` datetime DEFAULT NULL COMMENT '商品文字詳情更改時間',
  `mobile_image` varchar(40) DEFAULT NULL,
  `product_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `product_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `product_ipfrom` varchar(40) NOT NULL,
  `goods_area` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `goods_image1` varchar(40) NOT NULL,
  `goods_image2` varchar(40) NOT NULL,
  `city` varchar(20) CHARACTER SET utf8mb4 NOT NULL DEFAULT '''''',
  `bag_check_money` int(9) unsigned NOT NULL DEFAULT '0',
  `combination` tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '商品組合類型  1:單一商品 2:固定組合 3:任選組合 4:群組搭配',
  `bonus_percent` float unsigned NOT NULL DEFAULT '1' COMMENT '活動購物金回饋百分比',
  `default_bonus_percent` float unsigned NOT NULL DEFAULT '3' COMMENT '預設回饋百分比',
  `bonus_percent_start` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '購物金回饋時間',
  `bonus_percent_end` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '購物金回饋時間',
  `tax_type` int(11) NOT NULL DEFAULT '1',
  `cate_id` varchar(10) NOT NULL COMMENT '品類管理分類',
  `fortune_freight` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '中獎運費',
  `product_media` varchar(255) NOT NULL,
  `ignore_stock` smallint(1) NOT NULL DEFAULT '0' COMMENT '庫存為0時是否還能販售 1:是 0:否',
  `shortage` smallint(1) NOT NULL DEFAULT '0' COMMENT '補貨中停止販售 1:是 0:否',
  `stock_alarm` int(4) NOT NULL DEFAULT '0',
  `price_type` smallint(2) NOT NULL,
  `user_id` int(9) unsigned NOT NULL DEFAULT '0',
  `create_channel` tinyint(2) NOT NULL DEFAULT '0' COMMENT '商品建立來源  1:後台管理者(manage_user)  2:供應商(vendor)',
  `accumulated_bonus` smallint(2) NOT NULL DEFAULT '1',
  `show_listprice` smallint(2) unsigned NOT NULL DEFAULT '0',
  `show_in_deliver` tinyint(2) NOT NULL DEFAULT '1' COMMENT ' 顯示於出貨單中 1:是  0:否\n',
  `prepaid` tinyint(2) NOT NULL DEFAULT '0' COMMENT '已買斷的商品  0:否  1:是',
  `product_type` tinyint(2) NOT NULL DEFAULT '0' COMMENT '商品型態  0:實體商品,1:課程,2:旅遊,3:電子票券,91:購物金,92:抵用卷',
  `sale_status` tinyint(2) NOT NULL DEFAULT '-1' COMMENT '販售狀態(決定目前是否可顯示此商品及是否可賣)',
  `prod_name` varchar(50) NOT NULL DEFAULT '' COMMENT '商品原名稱',
  `prod_sz` char(10) NOT NULL DEFAULT '' COMMENT '規格欄位',
  `process_type` tinyint(2) NOT NULL DEFAULT '1' COMMENT '商品出貨方式  1:實體商品   2:電子商品 (自動押單,不寄簡訊)\n',
  `prod_classify` int(11) NOT NULL COMMENT '商品館別,例如食品館=10,用品館=20,等等',
  `deliver_days` int(2) DEFAULT '3' COMMENT '採購天數',
  `min_purchase_amount` int(2) DEFAULT '1' COMMENT '最小採購數量',
  `safe_stock_amount` double DEFAULT NULL COMMENT '安全存量細數',
  `extra_days` int(2) DEFAULT '0' COMMENT '寄倉天數/調度天數',
  `off_grade` int(2) DEFAULT NULL COMMENT '失格\n0:正常\n1:失格',
  `purchase_in_advance` tinyint(2) DEFAULT NULL COMMENT '是否預購商品 0:否 1:是',
  `purchase_in_advance_start` int(11) unsigned DEFAULT NULL COMMENT '預購商品開始時間',
  `purchase_in_advance_end` int(11) unsigned DEFAULT NULL COMMENT '預購商品結束時間',
  `outofstock_days_stopselling` int(4) DEFAULT '15' COMMENT '缺貨停售天數 ',
  PRIMARY KEY (`product_id`),
  KEY `ix_product_bid` (`brand_id`),
  KEY `ix_product_pvc` (`product_vendor_code`),
  KEY `ix_product_ss` (`product_status`),
  KEY `ix_product_st` (`product_sort`),
  KEY `ix_product_ie` (`product_image`),
  KEY `product_status` (`product_status`),
  CONSTRAINT `fk_product_bid` FOREIGN KEY (`brand_id`) REFERENCES `vendor_brand` (`brand_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for product_bak20150415_Ivan
-- ----------------------------
DROP TABLE IF EXISTS `product_bak20150415_Ivan`;
CREATE TABLE `product_bak20150415_Ivan` (
  `product_id` int(9) unsigned NOT NULL,
  `brand_id` int(9) unsigned NOT NULL,
  `product_vendor_code` varchar(20) NOT NULL DEFAULT '',
  `product_name` varchar(255) NOT NULL DEFAULT '' COMMENT '商品名稱',
  `product_price_list` mediumint(7) unsigned NOT NULL DEFAULT '0' COMMENT '建議售價',
  `product_spec` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `spec_title_1` varchar(50) NOT NULL DEFAULT '',
  `spec_title_2` varchar(50) NOT NULL DEFAULT '',
  `product_freight_set` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `product_buy_limit` mediumint(6) unsigned NOT NULL DEFAULT '10',
  `product_status` tinyint(2) unsigned NOT NULL DEFAULT '0' COMMENT '商品狀態',
  `product_hide` tinyint(1) NOT NULL DEFAULT '0',
  `product_mode` tinyint(2) unsigned NOT NULL DEFAULT '1' COMMENT '貨運模式',
  `product_sort` smallint(4) unsigned NOT NULL DEFAULT '0',
  `product_start` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '上架時間',
  `product_end` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '下架時間',
  `page_content_1` text NOT NULL,
  `page_content_2` text NOT NULL COMMENT '商品規格',
  `page_content_3` text NOT NULL,
  `product_keywords` varchar(255) NOT NULL DEFAULT '',
  `fortune_quota` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '抽獎名額',
  `product_recommend` int(10) unsigned NOT NULL DEFAULT '0',
  `product_password` varchar(32) NOT NULL DEFAULT '',
  `product_total_click` int(10) unsigned NOT NULL DEFAULT '0',
  `expect_time` int(10) unsigned NOT NULL DEFAULT '0',
  `expect_msg` varchar(80) NOT NULL DEFAULT '',
  `product_image` varchar(40) NOT NULL DEFAULT '',
  `product_alt` varchar(255) DEFAULT NULL COMMENT '商品浮動說明/HTML ALT屬性',
  `mobile_image` varchar(40) DEFAULT NULL,
  `product_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `product_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `product_ipfrom` varchar(40) NOT NULL,
  `goods_area` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `goods_image1` varchar(40) NOT NULL,
  `goods_image2` varchar(40) NOT NULL,
  `city` varchar(20) CHARACTER SET utf8mb4 NOT NULL DEFAULT '''''',
  `bag_check_money` int(9) unsigned NOT NULL DEFAULT '0',
  `combination` tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '商品組合類型  1:單一商品 2:固定組合 3:任選組合 4:群組搭配',
  `bonus_percent` float unsigned NOT NULL DEFAULT '1' COMMENT '活動購物金回饋百分比',
  `default_bonus_percent` float unsigned NOT NULL DEFAULT '3' COMMENT '預設回饋百分比',
  `bonus_percent_start` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '購物金回饋時間',
  `bonus_percent_end` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '購物金回饋時間',
  `tax_type` int(11) NOT NULL DEFAULT '1',
  `cate_id` varchar(10) NOT NULL COMMENT '品類管理分類',
  `fortune_freight` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '中獎運費',
  `product_media` varchar(255) NOT NULL,
  `ignore_stock` smallint(1) NOT NULL DEFAULT '0' COMMENT '庫存為0時是否還能販售 1:是 0:否',
  `shortage` smallint(1) NOT NULL DEFAULT '0' COMMENT '補貨中停止販售 1:是 0:否',
  `stock_alarm` int(4) NOT NULL DEFAULT '0',
  `price_type` smallint(2) NOT NULL,
  `user_id` int(9) unsigned NOT NULL DEFAULT '0',
  `create_channel` tinyint(2) NOT NULL DEFAULT '0' COMMENT '商品建立來源  1:後台管理者(manage_user)  2:供應商(vendor)',
  `accumulated_bonus` smallint(2) NOT NULL DEFAULT '1',
  `show_listprice` smallint(2) unsigned NOT NULL DEFAULT '0',
  `show_in_deliver` tinyint(2) NOT NULL DEFAULT '1' COMMENT ' 顯示於出貨單中 1:是  0:否\n',
  `prepaid` tinyint(2) NOT NULL DEFAULT '0' COMMENT '已買斷的商品  0:否  1:是',
  `product_type` tinyint(2) NOT NULL DEFAULT '0' COMMENT '商品型態  0:實體商品,1:課程,2:旅遊,3:電子票券,91:購物金,92:抵用卷',
  `sale_status` tinyint(2) NOT NULL DEFAULT '-1' COMMENT '販售狀態(決定目前是否可顯示此商品及是否可賣)',
  `prod_name` varchar(50) NOT NULL DEFAULT '' COMMENT '商品原名稱',
  `prod_sz` char(10) NOT NULL DEFAULT '' COMMENT '規格欄位',
  `process_type` tinyint(2) NOT NULL DEFAULT '1' COMMENT '商品出貨方式  1:實體商品   2:電子商品 (自動押單,不寄簡訊)\n',
  `prod_classify` int(11) NOT NULL COMMENT '商品館別,例如食品館=10,用品館=20,等等',
  `deliver_days` int(2) DEFAULT '3' COMMENT '採購天數',
  `min_purchase_amount` int(2) DEFAULT '1' COMMENT '最小採購數量',
  `safe_stock_amount` double DEFAULT NULL COMMENT '安全存量細數',
  `extra_days` int(2) DEFAULT '0' COMMENT '寄倉天數/調度天數',
  PRIMARY KEY (`product_id`),
  KEY `ix_product_bid` (`brand_id`),
  KEY `ix_product_pvc` (`product_vendor_code`),
  KEY `ix_product_ss` (`product_status`),
  KEY `ix_product_st` (`product_sort`),
  KEY `ix_product_ie` (`product_image`),
  KEY `product_status` (`product_status`),
  CONSTRAINT `product_bak20150415_Ivan_ibfk_1` FOREIGN KEY (`brand_id`) REFERENCES `vendor_brand` (`brand_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for product_category
-- ----------------------------
DROP TABLE IF EXISTS `product_category`;
CREATE TABLE `product_category` (
  `category_id` int(9) unsigned NOT NULL AUTO_INCREMENT,
  `category_father_id` int(9) unsigned NOT NULL DEFAULT '0',
  `category_name` varchar(255) NOT NULL COMMENT '分類名稱',
  `category_sort` smallint(4) unsigned NOT NULL DEFAULT '0',
  `category_display` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `category_show_mode` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `category_image_in` varchar(40) NOT NULL DEFAULT '',
  `category_image_out` varchar(40) NOT NULL DEFAULT '',
  `category_link_mode` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `category_link_url` varchar(255) NOT NULL DEFAULT '',
  `banner_image` varchar(40) NOT NULL DEFAULT '',
  `banner_status` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `banner_link_mode` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `banner_link_url` varchar(255) NOT NULL DEFAULT '',
  `banner_show_start` int(10) unsigned NOT NULL DEFAULT '0',
  `banner_show_end` int(10) unsigned NOT NULL DEFAULT '0',
  `category_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `category_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `category_ipfrom` varchar(40) NOT NULL DEFAULT '',
  `short_description` varchar(300) DEFAULT '' COMMENT '類別短文字說明（字數限制300字）',
  `status` int(9) DEFAULT '1',
  `category_image_app` varchar(40) DEFAULT '' COMMENT 'APP圖片',
  PRIMARY KEY (`category_id`),
  KEY `ix_product_category_ii` (`category_image_in`),
  KEY `ix_product_category_io` (`category_image_out`),
  KEY `ix_product_category_bi` (`banner_image`)
) ENGINE=InnoDB AUTO_INCREMENT=2128 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for product_category_banner
-- ----------------------------
DROP TABLE IF EXISTS `product_category_banner`;
CREATE TABLE `product_category_banner` (
  `row_id` int(9) NOT NULL AUTO_INCREMENT COMMENT 'row_id',
  `banner_cateid` int(9) unsigned DEFAULT NULL COMMENT '專區類別id',
  `category_id` int(9) unsigned NOT NULL COMMENT '類別id',
  `category_father_id` int(9) unsigned NOT NULL DEFAULT '0' COMMENT 'father類別id',
  `category_name` varchar(255) NOT NULL COMMENT '分類名稱',
  `category_sort` smallint(4) unsigned NOT NULL DEFAULT '0' COMMENT '排序',
  `category_display` tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '是否顯示 1：顯示 0：隱藏',
  `category_link_mode` tinyint(1) unsigned NOT NULL DEFAULT '1' COMMENT '連接方式 1：原視窗 2：新視窗',
  `createdate` int(10) unsigned DEFAULT '0' COMMENT '創建時間',
  `updatedate` int(10) unsigned DEFAULT '0' COMMENT '更新時間',
  `create_ipfrom` varchar(40) DEFAULT '' COMMENT '創建ip',
  `status` int(9) DEFAULT '1' COMMENT '狀態 1：啟用 0：禁用',
  PRIMARY KEY (`row_id`),
  KEY `row_id` (`row_id`) USING BTREE,
  KEY `banner_cateid` (`banner_cateid`) USING BTREE,
  KEY `category_id` (`category_id`) USING BTREE,
  KEY `category_father_id` (`category_father_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=434059 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for product_category_brand
-- ----------------------------
DROP TABLE IF EXISTS `product_category_brand`;
CREATE TABLE `product_category_brand` (
  `row_id` int(9) NOT NULL AUTO_INCREMENT,
  `banner_cate_id` int(9) DEFAULT NULL COMMENT '專區類別',
  `category_id` int(9) unsigned NOT NULL COMMENT '類別編號',
  `category_name` varchar(255) DEFAULT NULL COMMENT '分類名稱',
  `category_father_id` int(9) unsigned DEFAULT '0' COMMENT '父類別編號',
  `category_father_name` varchar(255) DEFAULT NULL COMMENT '父類別名稱',
  `depth` int(9) DEFAULT '0' COMMENT '深度 以新館為第1層',
  `brand_id` int(9) unsigned NOT NULL COMMENT '品牌編號',
  `createdate` datetime NOT NULL COMMENT '創建時間',
  PRIMARY KEY (`row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=421249 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for product_category_set
-- ----------------------------
DROP TABLE IF EXISTS `product_category_set`;
CREATE TABLE `product_category_set` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `product_id` int(9) unsigned NOT NULL DEFAULT '0',
  `category_id` int(9) unsigned NOT NULL DEFAULT '0',
  `brand_id` int(9) unsigned NOT NULL DEFAULT '0',
  `status` int(9) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `idx_prodid` (`product_id`,`category_id`) USING BTREE,
  KEY `fk_product_category_set_cid` (`category_id`),
  KEY `idx_brandid` (`brand_id`) USING BTREE,
  CONSTRAINT `fk_product_category_set_cid` FOREIGN KEY (`category_id`) REFERENCES `product_category` (`category_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_product_category_set_pid` FOREIGN KEY (`product_id`) REFERENCES `product` (`product_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=211447 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for product_category_set_temp
-- ----------------------------
DROP TABLE IF EXISTS `product_category_set_temp`;
CREATE TABLE `product_category_set_temp` (
  `writer_id` int(11) DEFAULT NULL,
  `product_id` varchar(10) NOT NULL DEFAULT '0',
  `category_id` int(9) unsigned DEFAULT NULL,
  `brand_id` int(9) unsigned DEFAULT NULL,
  `combo_type` int(9) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for product_check
-- ----------------------------
DROP TABLE IF EXISTS `product_check`;
CREATE TABLE `product_check` (
  `check_id` int(9) unsigned NOT NULL,
  `item_id` int(9) unsigned NOT NULL,
  `old_product_name` varchar(255) DEFAULT '',
  `old_item_cost` int(9) unsigned NOT NULL,
  `old_item_money` int(9) unsigned NOT NULL,
  `old_item_stock` int(9) unsigned NOT NULL,
  `on_check` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `modify_user_id` smallint(4) unsigned NOT NULL,
  `createdate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`check_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for product_click
-- ----------------------------
DROP TABLE IF EXISTS `product_click`;
CREATE TABLE `product_click` (
  `site_id` int(9) unsigned NOT NULL DEFAULT '1',
  `product_id` int(9) unsigned NOT NULL,
  `click_id` int(10) unsigned NOT NULL,
  `click_year` smallint(4) unsigned NOT NULL,
  `click_month` tinyint(2) unsigned NOT NULL,
  `click_day` tinyint(2) unsigned NOT NULL,
  `click_hour` tinyint(2) unsigned NOT NULL,
  `click_week` tinyint(1) unsigned NOT NULL,
  `click_total` int(9) unsigned NOT NULL,
  PRIMARY KEY (`site_id`,`product_id`,`click_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for product_combo
-- ----------------------------
DROP TABLE IF EXISTS `product_combo`;
CREATE TABLE `product_combo` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `parent_id` int(9) NOT NULL,
  `child_id` int(9) NOT NULL,
  `s_must_buy` smallint(2) DEFAULT NULL,
  `g_must_buy` smallint(2) DEFAULT NULL,
  `pile_id` smallint(2) DEFAULT NULL,
  `buy_limit` smallint(2) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `cm_pair` (`parent_id`,`child_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=4230 DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT COMMENT='商品組合';

-- ----------------------------
-- Table structure for product_combo_temp
-- ----------------------------
DROP TABLE IF EXISTS `product_combo_temp`;
CREATE TABLE `product_combo_temp` (
  `writer_id` int(4) DEFAULT NULL,
  `parent_id` varchar(10) DEFAULT NULL,
  `child_id` varchar(10) DEFAULT NULL,
  `s_must_buy` smallint(2) DEFAULT NULL,
  `g_must_buy` smallint(2) DEFAULT NULL,
  `pile_id` smallint(2) DEFAULT NULL,
  `buy_limit` smallint(2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT COMMENT='商品組合';

-- ----------------------------
-- Table structure for product_comment
-- ----------------------------
DROP TABLE IF EXISTS `product_comment`;
CREATE TABLE `product_comment` (
  `comment_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '序號.自動增長',
  `order_id` int(9) DEFAULT NULL COMMENT '訂單編號',
  `product_id` int(9) unsigned DEFAULT NULL COMMENT '商品編號',
  `user_id` int(9) DEFAULT NULL COMMENT '用戶編號',
  `is_show_name` tinyint(2) DEFAULT NULL COMMENT '0:匿名，1：公開',
  `create_time` int(10) DEFAULT NULL,
  `accumulated_bonus` int(10) DEFAULT NULL COMMENT '評價獲得購物金',
  PRIMARY KEY (`comment_id`)
) ENGINE=InnoDB AUTO_INCREMENT=8371 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for product_consult
-- ----------------------------
DROP TABLE IF EXISTS `product_consult`;
CREATE TABLE `product_consult` (
  `consult_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主鍵，自增',
  `product_id` int(9) unsigned DEFAULT NULL COMMENT '商品編號',
  `user_id` int(9) DEFAULT NULL COMMENT '用戶編號',
  `consult_info` varchar(100) DEFAULT NULL COMMENT '咨詢內容',
  `consult_type` tinyint(2) DEFAULT NULL COMMENT '咨詢類型1：商品咨詢，2：庫存及配送，3：支付問題，4：發票及保修，5：促銷及贈品',
  `consult_answer` varchar(300) DEFAULT NULL COMMENT '回復內容',
  `is_sendEmail` tinyint(2) DEFAULT NULL COMMENT '回復是否發送到郵箱',
  `create_date` datetime DEFAULT NULL COMMENT '咨詢時間',
  `answer_date` datetime DEFAULT NULL COMMENT '回復時間',
  `answer_user` int(9) DEFAULT NULL COMMENT '回復人',
  `status` int(2) DEFAULT NULL COMMENT '是否啟用',
  `item_id` int(9) DEFAULT NULL,
  `consult_url` varchar(100) DEFAULT NULL,
  `product_url` varchar(100) DEFAULT NULL,
  `answer_status` int(2) DEFAULT '1' COMMENT '1:待回覆，2：處理中，3：已回覆',
  `delay_reason` varchar(200) DEFAULT NULL COMMENT 'answwer_status為2時，需寫推遲處理的原因',
  PRIMARY KEY (`consult_id`)
) ENGINE=InnoDB AUTO_INCREMENT=260 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for product_delivery_set
-- ----------------------------
DROP TABLE IF EXISTS `product_delivery_set`;
CREATE TABLE `product_delivery_set` (
  `product_id` int(10) NOT NULL,
  `freight_big_area` tinyint(2) NOT NULL DEFAULT '0' COMMENT '配送區域',
  `freight_type` smallint(4) NOT NULL DEFAULT '0' COMMENT '配送模式'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for product_delivery_set_temp
-- ----------------------------
DROP TABLE IF EXISTS `product_delivery_set_temp`;
CREATE TABLE `product_delivery_set_temp` (
  `writer_id` int(4) DEFAULT NULL,
  `product_id` int(10) NOT NULL,
  `freight_big_area` tinyint(2) NOT NULL DEFAULT '0' COMMENT '配送區域',
  `freight_type` smallint(4) NOT NULL DEFAULT '0' COMMENT '配送模式',
  `combo_type` int(9) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for product_ext
-- ----------------------------
DROP TABLE IF EXISTS `product_ext`;
CREATE TABLE `product_ext` (
  `item_id` int(9) unsigned NOT NULL,
  `pend_del` char(1) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'N' COMMENT '等待删除中(Ｙ/Ｎ)',
  `cde_dt_shp` smallint(6) NOT NULL DEFAULT '0' COMMENT '允出天数',
  `pwy_dte_ctl` char(1) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '' COMMENT '有效期控制的商品(Ｙ/Ｎ)',
  `cde_dt_incr` smallint(6) NOT NULL DEFAULT '0' COMMENT '保存期限(天数)',
  `cde_dt_var` smallint(6) NOT NULL DEFAULT '0' COMMENT '允收天数',
  `hzd_ind` char(1) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '' COMMENT '易损坏的等级',
  `cse_wid` decimal(7,1) NOT NULL DEFAULT '0.0' COMMENT '外箱的宽度(cm)',
  `cse_wgt` decimal(9,2) NOT NULL DEFAULT '0.00' COMMENT '外箱的重量(kg)',
  `cse_unit` smallint(6) NOT NULL DEFAULT '0' COMMENT '外箱单位(ＯＭ)',
  `cse_len` decimal(7,1) NOT NULL DEFAULT '0.0' COMMENT '外箱的长度(cm)',
  `cse_hgt` decimal(7,1) NOT NULL DEFAULT '0.0' COMMENT '外箱的高度(cm)',
  `unit_ship_cse` smallint(6) NOT NULL DEFAULT '0' COMMENT '商品的ＯＰ',
  `inner_pack_wid` decimal(7,1) NOT NULL DEFAULT '0.0' COMMENT '内装的宽度(cm)',
  `inner_pack_wgt` decimal(9,2) NOT NULL DEFAULT '0.00' COMMENT '内装的重量(kg)',
  `inner_pack_unit` smallint(6) NOT NULL DEFAULT '0' COMMENT '内装单位(ＯＰ)',
  `inner_pack_len` decimal(7,1) NOT NULL DEFAULT '0.0' COMMENT '内装的长度(cm)',
  `inner_pack_hgt` decimal(7,1) NOT NULL DEFAULT '0.0' COMMENT '内装的高度(cm)',
  PRIMARY KEY (`item_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for product_ext_temp
-- ----------------------------
DROP TABLE IF EXISTS `product_ext_temp`;
CREATE TABLE `product_ext_temp` (
  `item_id` int(9) unsigned NOT NULL,
  `pend_del` char(1) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'N' COMMENT '等待删除中(Ｙ/Ｎ)',
  `cde_dt_shp` smallint(6) NOT NULL DEFAULT '0' COMMENT '允出天数',
  `pwy_dte_ctl` char(1) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '' COMMENT '有效期控制的商品(Ｙ/Ｎ)',
  `cde_dt_incr` smallint(6) NOT NULL DEFAULT '0' COMMENT '保存期限(天数)',
  `cde_dt_var` smallint(6) NOT NULL DEFAULT '0' COMMENT '允收天数',
  `hzd_ind` char(1) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '' COMMENT '易损坏的等级',
  `cse_wid` decimal(7,1) NOT NULL DEFAULT '0.0' COMMENT '外箱的宽度(cm)',
  `cse_wgt` decimal(9,2) NOT NULL DEFAULT '0.00' COMMENT '外箱的重量(kg)',
  `cse_unit` smallint(6) NOT NULL DEFAULT '0' COMMENT '外箱单位(ＯＭ)',
  `cse_len` decimal(7,1) NOT NULL DEFAULT '0.0' COMMENT '外箱的长度(cm)',
  `cse_hgt` decimal(7,1) NOT NULL DEFAULT '0.0' COMMENT '外箱的高度(cm)',
  `unit_ship_cse` smallint(6) NOT NULL DEFAULT '0' COMMENT '商品的ＯＰ',
  `inner_pack_wid` decimal(7,1) NOT NULL DEFAULT '0.0' COMMENT '内装的宽度(cm)',
  `inner_pack_wgt` decimal(9,2) NOT NULL DEFAULT '0.00' COMMENT '内装的重量(kg)',
  `inner_pack_unit` smallint(6) NOT NULL DEFAULT '0' COMMENT '内装单位(ＯＰ)',
  `inner_pack_len` decimal(7,1) NOT NULL DEFAULT '0.0' COMMENT '内装的长度(cm)',
  `inner_pack_hgt` decimal(7,1) NOT NULL DEFAULT '0.0' COMMENT '内装的高度(cm)',
  PRIMARY KEY (`item_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for product_extend
-- ----------------------------
DROP TABLE IF EXISTS `product_extend`;
CREATE TABLE `product_extend` (
  `rid` bigint(20) NOT NULL AUTO_INCREMENT,
  `price_master_id` int(11) unsigned NOT NULL COMMENT ' price_master.price_master_id',
  `product_prefix` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '商品前綴',
  `product_suffix` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '商品後綴',
  `event_start` int(10) unsigned DEFAULT NULL COMMENT '活動開始時間',
  `event_end` int(10) unsigned DEFAULT NULL COMMENT '活動結束時間',
  `flag` int(2) DEFAULT '0',
  `kuser` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '創建人',
  `kdate` timestamp NULL DEFAULT NULL COMMENT '創建時間',
  `apply_id` int(9) unsigned DEFAULT NULL COMMENT 'pirce_master_ts.apply_id',
  PRIMARY KEY (`rid`)
) ENGINE=InnoDB AUTO_INCREMENT=739 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for product_item
-- ----------------------------
DROP TABLE IF EXISTS `product_item`;
CREATE TABLE `product_item` (
  `item_id` int(9) unsigned NOT NULL DEFAULT '0',
  `product_id` int(9) unsigned NOT NULL DEFAULT '0',
  `spec_id_1` int(9) unsigned NOT NULL DEFAULT '0',
  `spec_id_2` int(9) unsigned NOT NULL DEFAULT '0',
  `item_cost` int(9) unsigned NOT NULL DEFAULT '0',
  `item_money` int(9) unsigned NOT NULL DEFAULT '0',
  `event_product_start` int(10) unsigned NOT NULL DEFAULT '0',
  `event_product_end` int(10) unsigned NOT NULL DEFAULT '0',
  `event_item_cost` int(9) unsigned NOT NULL DEFAULT '0',
  `event_item_money` int(9) unsigned NOT NULL DEFAULT '0',
  `item_stock` int(9) NOT NULL DEFAULT '0' COMMENT '庫存',
  `item_alarm` int(9) unsigned NOT NULL DEFAULT '0',
  `item_status` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `item_code` varchar(30) NOT NULL DEFAULT '',
  `barcode` varchar(20) NOT NULL DEFAULT '' COMMENT '條碼',
  `erp_id` varchar(30) DEFAULT NULL,
  `export_flag` smallint(5) NOT NULL DEFAULT '0',
  `data_chg` tinyint(2) NOT NULL DEFAULT '0',
  `remark` varchar(100) DEFAULT NULL COMMENT '庫存備注',
  `arrive_days` int(2) DEFAULT '0' COMMENT '送達天數',
  PRIMARY KEY (`item_id`),
  UNIQUE KEY `uk_product_item_pss` (`product_id`,`spec_id_1`,`spec_id_2`),
  KEY `ix_product_item_pid` (`product_id`),
  KEY `ix_item_erp_id` (`erp_id`) USING BTREE,
  CONSTRAINT `fk_product_item_pid` FOREIGN KEY (`product_id`) REFERENCES `product` (`product_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for product_item_map
-- ----------------------------
DROP TABLE IF EXISTS `product_item_map`;
CREATE TABLE `product_item_map` (
  `rid` int(9) unsigned NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `channel_id` int(4) unsigned DEFAULT NULL COMMENT '外站編號  FK:channel.channel_id',
  `channel_detail_id` varchar(32) DEFAULT NULL COMMENT '外站商品編號',
  `item_id` int(9) unsigned DEFAULT '0' COMMENT '吉甲地商品細項編號\nFK:product_item.item_id\n',
  `product_name` varchar(510) DEFAULT NULL COMMENT '外站商品名稱',
  `product_cost` int(9) DEFAULT NULL COMMENT '外站商品成本',
  `product_price` int(9) DEFAULT NULL COMMENT '外站商品售價',
  `product_id` int(9) unsigned DEFAULT NULL,
  `group_item_id` varchar(100) DEFAULT NULL,
  `price_master_id` int(9) unsigned DEFAULT NULL,
  PRIMARY KEY (`rid`)
) ENGINE=InnoDB AUTO_INCREMENT=3174 DEFAULT CHARSET=utf8 COMMENT='外站商品對照表';

-- ----------------------------
-- Table structure for product_item_temp
-- ----------------------------
DROP TABLE IF EXISTS `product_item_temp`;
CREATE TABLE `product_item_temp` (
  `writer_id` int(4) DEFAULT NULL,
  `item_id` int(9) unsigned NOT NULL AUTO_INCREMENT,
  `product_id` varchar(10) NOT NULL DEFAULT '0',
  `spec_id_1` int(9) unsigned DEFAULT NULL,
  `spec_id_2` int(9) unsigned DEFAULT NULL,
  `item_cost` int(9) unsigned DEFAULT NULL,
  `item_money` int(9) unsigned DEFAULT NULL,
  `event_product_start` int(10) unsigned DEFAULT NULL,
  `event_product_end` int(10) unsigned DEFAULT NULL,
  `event_item_cost` int(9) unsigned DEFAULT NULL,
  `event_item_money` int(9) unsigned DEFAULT NULL,
  `item_stock` int(9) DEFAULT NULL,
  `item_alarm` int(9) unsigned DEFAULT NULL,
  `item_status` tinyint(1) unsigned DEFAULT NULL,
  `item_code` varchar(30) DEFAULT NULL,
  `barcode` varchar(20) DEFAULT NULL COMMENT '條碼',
  `erp_id` varchar(30) DEFAULT NULL,
  `export_flag` tinyint(2) NOT NULL DEFAULT '0',
  `data_chg` tinyint(2) NOT NULL DEFAULT '0',
  `remark` varchar(100) DEFAULT NULL COMMENT '庫存備注',
  `arrive_days` int(2) DEFAULT '0' COMMENT '送達天數',
  PRIMARY KEY (`item_id`)
) ENGINE=InnoDB AUTO_INCREMENT=9278 DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for product_map_set
-- ----------------------------
DROP TABLE IF EXISTS `product_map_set`;
CREATE TABLE `product_map_set` (
  `rid` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `map_rid` int(9) unsigned DEFAULT NULL,
  `item_id` int(9) unsigned DEFAULT NULL,
  `set_num` smallint(4) unsigned DEFAULT NULL,
  PRIMARY KEY (`rid`)
) ENGINE=InnoDB AUTO_INCREMENT=801 DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for product_migration_map
-- ----------------------------
DROP TABLE IF EXISTS `product_migration_map`;
CREATE TABLE `product_migration_map` (
  `rowid` int(9) unsigned NOT NULL AUTO_INCREMENT,
  `product_id` int(9) unsigned NOT NULL COMMENT '商品編號',
  `temp_id` varchar(10) NOT NULL COMMENT '臨時編號',
  PRIMARY KEY (`rowid`)
) ENGINE=InnoDB AUTO_INCREMENT=3336 DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT COMMENT='商品轉移編號對照';

-- ----------------------------
-- Table structure for product_notice
-- ----------------------------
DROP TABLE IF EXISTS `product_notice`;
CREATE TABLE `product_notice` (
  `notice_id` int(9) unsigned NOT NULL,
  `notice_name` varchar(255) NOT NULL DEFAULT '',
  `notice_sort` smallint(4) unsigned NOT NULL DEFAULT '0',
  `notice_status` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `notice_filename` varchar(40) NOT NULL DEFAULT '',
  `notice_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `notice_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `notice_ipfrom` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`notice_id`),
  KEY `ix_product_notice_st` (`notice_sort`),
  KEY `ix_product_notice_ss` (`notice_status`),
  KEY `ix_product_notice_fe` (`notice_filename`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for product_notice_set
-- ----------------------------
DROP TABLE IF EXISTS `product_notice_set`;
CREATE TABLE `product_notice_set` (
  `product_id` int(9) unsigned NOT NULL DEFAULT '0',
  `notice_id` int(9) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`product_id`,`notice_id`),
  KEY `fk_product_notice_set_cid` (`notice_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for product_notice_set_temp
-- ----------------------------
DROP TABLE IF EXISTS `product_notice_set_temp`;
CREATE TABLE `product_notice_set_temp` (
  `writer_id` int(9) DEFAULT '0',
  `product_id` varchar(10) NOT NULL DEFAULT '0',
  `notice_id` int(9) unsigned DEFAULT '0',
  `combo_type` int(9) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for product_picture
-- ----------------------------
DROP TABLE IF EXISTS `product_picture`;
CREATE TABLE `product_picture` (
  `product_id` int(9) unsigned NOT NULL,
  `image_filename` varchar(40) NOT NULL DEFAULT '',
  `image_sort` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `image_state` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `image_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`product_id`,`image_filename`),
  CONSTRAINT `fk_product_picture_pid` FOREIGN KEY (`product_id`) REFERENCES `product` (`product_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for product_picture_app
-- ----------------------------
DROP TABLE IF EXISTS `product_picture_app`;
CREATE TABLE `product_picture_app` (
  `product_id` int(9) unsigned NOT NULL,
  `image_filename` varchar(40) NOT NULL DEFAULT '',
  `image_sort` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `image_state` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `image_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`product_id`,`image_filename`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for product_picture_app_temp
-- ----------------------------
DROP TABLE IF EXISTS `product_picture_app_temp`;
CREATE TABLE `product_picture_app_temp` (
  `writer_id` int(9) DEFAULT NULL,
  `product_id` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '0',
  `image_filename` varchar(40) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `image_sort` tinyint(2) unsigned DEFAULT NULL,
  `image_state` tinyint(2) unsigned DEFAULT NULL,
  `image_createdate` int(10) unsigned DEFAULT NULL,
  `combo_type` int(4) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for product_picture_temp
-- ----------------------------
DROP TABLE IF EXISTS `product_picture_temp`;
CREATE TABLE `product_picture_temp` (
  `writer_id` int(9) DEFAULT NULL,
  `product_id` varchar(10) NOT NULL DEFAULT '0',
  `image_filename` varchar(40) DEFAULT NULL,
  `image_sort` tinyint(2) unsigned DEFAULT NULL,
  `image_state` tinyint(2) unsigned DEFAULT NULL,
  `image_createdate` int(10) unsigned DEFAULT NULL,
  `combo_type` int(4) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for product_remove_reason
-- ----------------------------
DROP TABLE IF EXISTS `product_remove_reason`;
CREATE TABLE `product_remove_reason` (
  `row_id` int(9) NOT NULL AUTO_INCREMENT,
  `product_id` int(9) unsigned DEFAULT '0' COMMENT '商品編號',
  `product_num` int(9) DEFAULT '0' COMMENT '庫存',
  `create_name` varchar(100) DEFAULT '' COMMENT '創建人',
  `create_time` int(11) DEFAULT '0' COMMENT '創建時間',
  PRIMARY KEY (`row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=174 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for product_season
-- ----------------------------
DROP TABLE IF EXISTS `product_season`;
CREATE TABLE `product_season` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `product_id` int(9) unsigned NOT NULL DEFAULT '0',
  `season` tinyint(2) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `season` (`season`)
) ENGINE=InnoDB AUTO_INCREMENT=356 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for product_set
-- ----------------------------
DROP TABLE IF EXISTS `product_set`;
CREATE TABLE `product_set` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `parent_id` int(11) NOT NULL,
  `child_id` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for product_spec
-- ----------------------------
DROP TABLE IF EXISTS `product_spec`;
CREATE TABLE `product_spec` (
  `spec_id` int(9) unsigned NOT NULL DEFAULT '0',
  `product_id` int(9) unsigned NOT NULL DEFAULT '0',
  `spec_type` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `spec_name` varchar(255) NOT NULL DEFAULT '' COMMENT '規格名稱',
  `spec_sort` smallint(4) unsigned NOT NULL DEFAULT '0',
  `spec_status` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `spec_image` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`spec_id`),
  KEY `ix_product_spec_pid` (`product_id`),
  KEY `ix_product_spec_te` (`spec_type`),
  KEY `ix_product_spec_ie` (`spec_image`),
  CONSTRAINT `fk_product_spec_pid` FOREIGN KEY (`product_id`) REFERENCES `product` (`product_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for product_spec_temp
-- ----------------------------
DROP TABLE IF EXISTS `product_spec_temp`;
CREATE TABLE `product_spec_temp` (
  `writer_id` int(4) DEFAULT NULL,
  `spec_id` int(9) unsigned NOT NULL,
  `product_id` varchar(10) NOT NULL DEFAULT '0',
  `spec_type` tinyint(2) unsigned DEFAULT NULL,
  `spec_name` varchar(255) DEFAULT NULL,
  `spec_sort` smallint(4) unsigned DEFAULT NULL,
  `spec_status` tinyint(2) unsigned DEFAULT NULL,
  `spec_image` varchar(40) DEFAULT NULL,
  PRIMARY KEY (`spec_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for product_status_apply
-- ----------------------------
DROP TABLE IF EXISTS `product_status_apply`;
CREATE TABLE `product_status_apply` (
  `apply_id` int(9) unsigned NOT NULL AUTO_INCREMENT,
  `product_id` int(9) unsigned NOT NULL,
  `prev_status` tinyint(2) unsigned NOT NULL COMMENT '前一狀態',
  `apply_time` datetime NOT NULL COMMENT '申請審核時間',
  `online_mode` tinyint(2) unsigned NOT NULL DEFAULT '1' COMMENT '上架模式 1:依上架時間自動上架 2:審核通過即上架',
  PRIMARY KEY (`apply_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4870 DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for product_status_history
-- ----------------------------
DROP TABLE IF EXISTS `product_status_history`;
CREATE TABLE `product_status_history` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `product_id` int(9) unsigned NOT NULL,
  `user_id` smallint(4) unsigned NOT NULL COMMENT '操作人員',
  `create_time` datetime NOT NULL COMMENT '操作時間',
  `type` tinyint(2) unsigned NOT NULL COMMENT '操作類型 1:申請審核 2:核可 3:駁回 4:下架 5:新建商品 6:上架 7:系統移轉建立 8:取消送審. parameterType=verify_operate_type',
  `product_status` tinyint(2) NOT NULL COMMENT '操作後狀態  0:新建立商品 1:申請審核 2:審核通過 5:上架 6:下架 20:供應商新建商品. parameterType=product_status',
  `remark` varchar(255) DEFAULT NULL COMMENT '備註',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=30231 DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for product_statuslog
-- ----------------------------
DROP TABLE IF EXISTS `product_statuslog`;
CREATE TABLE `product_statuslog` (
  `rowid` int(15) NOT NULL AUTO_INCREMENT COMMENT '主鍵自增ID',
  `sale_status_no` varchar(32) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '插入的批次號',
  `product_id` int(11) DEFAULT NULL COMMENT '商品id',
  `old_sale_status` int(2) DEFAULT NULL COMMENT '上一次販售狀態',
  `new_sale_status` int(2) DEFAULT NULL COMMENT '當前販售狀態',
  `kdate` datetime DEFAULT NULL COMMENT '插入時間',
  PRIMARY KEY (`rowid`)
) ENGINE=InnoDB AUTO_INCREMENT=6496009 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for product_tag
-- ----------------------------
DROP TABLE IF EXISTS `product_tag`;
CREATE TABLE `product_tag` (
  `tag_id` int(9) unsigned NOT NULL,
  `tag_name` varchar(255) NOT NULL DEFAULT '',
  `tag_sort` smallint(4) unsigned NOT NULL DEFAULT '0',
  `tag_status` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `tag_filename` varchar(40) NOT NULL DEFAULT '',
  `tag_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `tag_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `tag_ipfrom` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`tag_id`),
  KEY `ix_product_tag_st` (`tag_sort`),
  KEY `ix_product_tag_ss` (`tag_status`),
  KEY `ix_product_tag_fe` (`tag_filename`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for product_tag_set
-- ----------------------------
DROP TABLE IF EXISTS `product_tag_set`;
CREATE TABLE `product_tag_set` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `product_id` int(9) unsigned NOT NULL DEFAULT '0',
  `tag_id` int(9) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `product_id` (`product_id`,`tag_id`),
  KEY `fk_product_tag_set_cid` (`tag_id`),
  CONSTRAINT `fk_product_tag_set_cid` FOREIGN KEY (`tag_id`) REFERENCES `product_tag` (`tag_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_product_tag_set_pid` FOREIGN KEY (`product_id`) REFERENCES `product` (`product_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=148155 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for product_tag_set_temp
-- ----------------------------
DROP TABLE IF EXISTS `product_tag_set_temp`;
CREATE TABLE `product_tag_set_temp` (
  `writer_id` int(9) DEFAULT '0',
  `product_id` varchar(10) NOT NULL DEFAULT '0',
  `tag_id` int(9) unsigned DEFAULT '0',
  `combo_type` int(9) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for product_temp
-- ----------------------------
DROP TABLE IF EXISTS `product_temp`;
CREATE TABLE `product_temp` (
  `rid` int(9) NOT NULL AUTO_INCREMENT,
  `writer_id` int(4) DEFAULT NULL,
  `product_id` varchar(10) NOT NULL DEFAULT '0',
  `brand_id` int(9) unsigned DEFAULT NULL,
  `product_vendor_code` varchar(20) DEFAULT NULL,
  `product_name` varchar(255) DEFAULT NULL,
  `product_price_list` mediumint(7) unsigned DEFAULT NULL,
  `product_spec` tinyint(2) unsigned DEFAULT NULL,
  `spec_title_1` varchar(50) DEFAULT NULL,
  `spec_title_2` varchar(50) DEFAULT NULL,
  `product_freight_set` tinyint(2) unsigned DEFAULT NULL,
  `product_buy_limit` mediumint(6) unsigned DEFAULT NULL,
  `product_status` tinyint(2) unsigned DEFAULT NULL,
  `product_hide` tinyint(1) DEFAULT NULL,
  `product_mode` tinyint(2) unsigned DEFAULT NULL,
  `product_sort` smallint(4) unsigned DEFAULT NULL,
  `product_start` int(10) unsigned DEFAULT NULL,
  `product_end` int(10) unsigned DEFAULT NULL,
  `page_content_1` text,
  `page_content_2` text,
  `page_content_3` text,
  `product_keywords` varchar(255) DEFAULT NULL,
  `product_recommend` int(10) unsigned DEFAULT NULL,
  `product_password` varchar(32) DEFAULT NULL,
  `product_total_click` int(10) unsigned DEFAULT NULL,
  `expect_time` int(10) unsigned DEFAULT NULL,
  `product_image` varchar(40) DEFAULT NULL,
  `mobile_image` varchar(40) DEFAULT NULL,
  `product_createdate` int(10) unsigned DEFAULT NULL,
  `product_updatedate` int(10) unsigned DEFAULT NULL,
  `product_ipfrom` varchar(40) DEFAULT NULL,
  `goods_area` tinyint(1) unsigned DEFAULT NULL,
  `goods_image1` varchar(40) DEFAULT NULL,
  `goods_image2` varchar(40) DEFAULT NULL,
  `city` varchar(20) DEFAULT NULL,
  `bag_check_money` int(9) unsigned DEFAULT NULL,
  `combination` tinyint(1) unsigned DEFAULT NULL COMMENT '組合商品 0:N 1:Y',
  `bonus_percent` float unsigned DEFAULT NULL COMMENT '活動購物金回饋百分比',
  `default_bonus_percent` float unsigned DEFAULT NULL COMMENT '預設回饋百分比',
  `bonus_percent_start` int(10) unsigned DEFAULT NULL COMMENT '購物金回饋時間',
  `bonus_percent_end` int(10) unsigned DEFAULT NULL COMMENT '購物金回饋時間',
  `tax_type` int(11) DEFAULT NULL,
  `cate_id` varchar(10) DEFAULT NULL COMMENT '品類管理之分類',
  `fortune_quota` int(11) unsigned DEFAULT NULL,
  `fortune_freight` int(11) unsigned DEFAULT NULL,
  `product_media` varchar(255) DEFAULT NULL,
  `ignore_stock` int(4) DEFAULT NULL,
  `shortage` int(4) DEFAULT NULL,
  `combo_type` int(4) DEFAULT NULL,
  `stock_alarm` int(11) DEFAULT NULL,
  `price_type` int(4) DEFAULT NULL,
  `show_listprice` smallint(2) unsigned DEFAULT NULL,
  `process_type` tinyint(2) NOT NULL DEFAULT '0' COMMENT '配送系統(商品處理方式)  0:物流配送, 1:電子郵件, 2:簡訊, 99:EC系統\n',
  `prod_classify` int(11) NOT NULL COMMENT '商品館別,例如食品館=10,用品館=20,等等',
  `deliver_days` int(2) DEFAULT '3' COMMENT '採購天數',
  `min_purchase_amount` int(2) DEFAULT '1' COMMENT '最小採購數量',
  `safe_stock_amount` double DEFAULT NULL COMMENT '安全存量細數',
  `extra_days` int(2) DEFAULT '0' COMMENT '寄倉天數/調度天數',
  `show_in_deliver` tinyint(2) NOT NULL DEFAULT '1' COMMENT ' 顯示於出貨單中 1:是  0:否\n',
  `prepaid` tinyint(2) NOT NULL DEFAULT '0' COMMENT '已買斷的商品  0:否  1:是',
  `product_type` tinyint(2) NOT NULL DEFAULT '0' COMMENT '商品型態  0:實體商品,1:課程,2:旅遊,3:電子票券,91:購物金,92:抵用卷',
  `sale_status` tinyint(2) NOT NULL DEFAULT '-1' COMMENT '販售狀態(決定目前是否可顯示此商品及是否可賣)',
  `prod_name` varchar(50) NOT NULL DEFAULT '' COMMENT '商品原名稱',
  `prod_sz` char(10) NOT NULL DEFAULT '' COMMENT '規格欄位',
  `create_channel` tinyint(2) NOT NULL DEFAULT '0' COMMENT '商品建立來源  1:後台管理者(manage_user)  2:供應商(vendor)',
  `expect_msg` varchar(255) DEFAULT NULL,
  `temp_status` tinyint(2) DEFAULT NULL,
  `product_alt` varchar(255) DEFAULT NULL COMMENT '商品浮動說明/HTML ALT屬性',
  `purchase_in_advance` tinyint(2) DEFAULT NULL COMMENT '是否預購商品 0:否 1:是',
  `purchase_in_advance_start` int(11) unsigned DEFAULT NULL COMMENT '預購商品開始時間',
  `purchase_in_advance_end` int(11) unsigned DEFAULT NULL COMMENT '預購商品結束時間',
  `outofstock_days_stopselling` int(4) DEFAULT '15' COMMENT '缺貨下架天數 ',
  PRIMARY KEY (`rid`),
  KEY `writer_id` (`writer_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=4627 DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for promo_additional_price
-- ----------------------------
DROP TABLE IF EXISTS `promo_additional_price`;
CREATE TABLE `promo_additional_price` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `event_name` varchar(255) CHARACTER SET utf8 NOT NULL COMMENT '活動名稱',
  `event_desc` varchar(100) CHARACTER SET utf8 NOT NULL COMMENT '活動描述',
  `event_type` varchar(10) CHARACTER SET utf8 NOT NULL COMMENT '活動類型',
  `condition_id` int(10) DEFAULT NULL COMMENT '會員條件id',
  `group_id` int(11) DEFAULT NULL COMMENT '會員群組',
  `class_id` int(11) DEFAULT NULL COMMENT '館別',
  `brand_id` int(11) DEFAULT NULL COMMENT '品牌',
  `product_id` int(11) DEFAULT NULL COMMENT '商品',
  `start` datetime DEFAULT NULL COMMENT '活動開始時間',
  `end` datetime DEFAULT NULL COMMENT '活動結束時間',
  `created` datetime NOT NULL COMMENT '建立時間',
  `modified` datetime DEFAULT NULL COMMENT '修改時間',
  `active` tinyint(1) NOT NULL COMMENT '0:無效  1:有效',
  `deliver_type` int(1) DEFAULT NULL COMMENT '運送類別 0:不分 1:常溫 2.低溫',
  `device` varchar(10) CHARACTER SET utf8 DEFAULT NULL COMMENT '裝置',
  `payment_code` varchar(10) CHARACTER SET utf8 DEFAULT NULL COMMENT '付款方式',
  `fixed_price` int(10) DEFAULT NULL COMMENT '固定加價購',
  `category_id` int(10) DEFAULT NULL COMMENT '促銷項目',
  `buy_limit` smallint(4) DEFAULT NULL COMMENT '限購件數',
  `kuser` varchar(50) CHARACTER SET utf8 NOT NULL COMMENT '建立人',
  `muser` varchar(50) CHARACTER SET utf8 DEFAULT NULL COMMENT '修改人',
  `status` int(2) DEFAULT NULL,
  `website` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'web設定',
  `url_by` int(2) DEFAULT NULL COMMENT '是否為專區',
  `discount` int(9) DEFAULT '0',
  `left_category_id` int(10) DEFAULT NULL,
  `right_category_id` int(10) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for promo_all
-- ----------------------------
DROP TABLE IF EXISTS `promo_all`;
CREATE TABLE `promo_all` (
  `rid` int(11) NOT NULL AUTO_INCREMENT,
  `event_id` varchar(11) CHARACTER SET utf8 NOT NULL COMMENT '活動id',
  `event_type` varchar(10) CHARACTER SET utf8 DEFAULT NULL COMMENT '活動類型',
  `brand_id` int(9) DEFAULT NULL COMMENT '品牌',
  `class_id` int(9) DEFAULT NULL COMMENT '館別',
  `category_id` int(9) DEFAULT NULL COMMENT '類別',
  `product_id` int(9) DEFAULT NULL COMMENT '商品id',
  `start` datetime DEFAULT NULL COMMENT '活動開始時間',
  `end` datetime DEFAULT NULL COMMENT '活動結束時間',
  `kuser` varchar(50) CHARACTER SET utf8 DEFAULT NULL,
  `kdate` datetime NOT NULL,
  `muser` varchar(50) CHARACTER SET utf8 DEFAULT NULL,
  `mdate` datetime DEFAULT NULL,
  `status` int(9) NOT NULL DEFAULT '1',
  PRIMARY KEY (`rid`)
) ENGINE=InnoDB AUTO_INCREMENT=381 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for promo_discount
-- ----------------------------
DROP TABLE IF EXISTS `promo_discount`;
CREATE TABLE `promo_discount` (
  `rid` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水碼',
  `event_id` varchar(11) CHARACTER SET utf8 NOT NULL COMMENT '活動id',
  `quantity` int(11) NOT NULL COMMENT '滿件',
  `discount` int(11) NOT NULL COMMENT '折扣',
  `special_price` int(9) NOT NULL COMMENT '促銷價格',
  `kuser` varchar(50) CHARACTER SET utf8 NOT NULL,
  `kdate` datetime NOT NULL,
  `muser` varchar(50) CHARACTER SET utf8 DEFAULT NULL,
  `mdate` datetime DEFAULT NULL,
  `status` int(9) NOT NULL DEFAULT '1',
  PRIMARY KEY (`rid`)
) ENGINE=InnoDB AUTO_INCREMENT=56 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for promo_pair
-- ----------------------------
DROP TABLE IF EXISTS `promo_pair`;
CREATE TABLE `promo_pair` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `event_name` varchar(255) CHARACTER SET utf8 NOT NULL COMMENT '活動名稱',
  `event_desc` varchar(100) CHARACTER SET utf8 NOT NULL COMMENT '活動描述',
  `event_type` varchar(10) CHARACTER SET utf8 NOT NULL COMMENT '活動類型',
  `condition_id` int(10) DEFAULT NULL COMMENT '會員條件id',
  `group_id` int(11) DEFAULT NULL COMMENT '會員群組',
  `start` datetime DEFAULT NULL COMMENT '活動開始時間',
  `end` datetime DEFAULT NULL COMMENT '活動結束時間',
  `created` datetime DEFAULT NULL COMMENT '建立時間',
  `modified` datetime DEFAULT NULL COMMENT '修改時間',
  `active` tinyint(1) DEFAULT NULL COMMENT '0:無效 1:啟用',
  `deliver_type` int(1) DEFAULT NULL COMMENT '運送類別 0.不分 1.常溫 2.低溫',
  `device` varchar(10) CHARACTER SET utf8 DEFAULT NULL COMMENT '裝置',
  `payment_code` varchar(10) CHARACTER SET utf8 DEFAULT NULL COMMENT '付款方式',
  `cate_red` int(9) DEFAULT NULL COMMENT '紅類別',
  `cate_green` int(9) DEFAULT NULL COMMENT '綠類別',
  `kuser` varchar(10) CHARACTER SET utf8 DEFAULT NULL COMMENT '添加人',
  `muser` varchar(10) CHARACTER SET utf8 DEFAULT NULL COMMENT '修改人',
  `category_id` int(11) NOT NULL COMMENT 'product_category表的ID',
  `website` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'website設定',
  `price` int(15) DEFAULT NULL COMMENT '合價',
  `discount` int(15) DEFAULT NULL COMMENT '打折',
  `status` int(2) DEFAULT NULL COMMENT '0:刪除 1:使用',
  `vendor_coverage` int(11) NOT NULL DEFAULT '0' COMMENT '供應商回扣萬分比',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=33 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for promo_ticket
-- ----------------------------
DROP TABLE IF EXISTS `promo_ticket`;
CREATE TABLE `promo_ticket` (
  `rid` int(10) NOT NULL AUTO_INCREMENT,
  `ticket_name` varchar(200) CHARACTER SET utf8 NOT NULL,
  `event_id` varchar(11) CHARACTER SET utf8 NOT NULL,
  `event_type` varchar(10) CHARACTER SET utf8 NOT NULL,
  `active_now` tinyint(2) NOT NULL,
  `valid_interval` smallint(4) NOT NULL,
  `use_start` datetime NOT NULL,
  `use_end` datetime NOT NULL,
  `kuser` varchar(50) CHARACTER SET utf8 NOT NULL,
  `kdate` datetime NOT NULL,
  `muser` varchar(50) CHARACTER SET utf8 DEFAULT NULL,
  `mdate` datetime DEFAULT NULL,
  `status` int(9) DEFAULT '1',
  PRIMARY KEY (`rid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for promotion_banner
-- ----------------------------
DROP TABLE IF EXISTS `promotion_banner`;
CREATE TABLE `promotion_banner` (
  `pb_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '促銷圖片編號',
  `pb_image` varchar(50) NOT NULL DEFAULT '' COMMENT '促銷圖片檔案',
  `pb_image_link` varchar(255) DEFAULT '' COMMENT '圖片連結地址',
  `pb_startdate` datetime NOT NULL COMMENT '顯示開始時間',
  `pb_enddate` datetime NOT NULL COMMENT '顯示結束時間',
  `pb_status` tinyint(4) NOT NULL DEFAULT '0' COMMENT '1=啟用, 0=未啟用',
  `pb_kdate` datetime NOT NULL COMMENT '建立時間',
  `pb_kuser` int(11) NOT NULL DEFAULT '0' COMMENT '建立人員',
  `pb_mdate` datetime NOT NULL COMMENT '異動時間',
  `pb_muser` int(11) NOT NULL DEFAULT '0' COMMENT '異動人員',
  PRIMARY KEY (`pb_id`),
  KEY `ix_pb_startdate` (`pb_startdate`) USING BTREE,
  KEY `ix_pb_enddate` (`pb_enddate`) USING BTREE,
  KEY `ix_pb_status` (`pb_status`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=40 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for promotion_banner_relation
-- ----------------------------
DROP TABLE IF EXISTS `promotion_banner_relation`;
CREATE TABLE `promotion_banner_relation` (
  `pb_id` int(11) NOT NULL DEFAULT '0' COMMENT '促銷圖片編號',
  `brand_id` int(9) unsigned NOT NULL DEFAULT '0' COMMENT '品牌編號',
  PRIMARY KEY (`pb_id`,`brand_id`),
  KEY `fx_brand_id` (`brand_id`),
  CONSTRAINT `fx_brand_id` FOREIGN KEY (`brand_id`) REFERENCES `vendor_brand` (`brand_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fx_pb_id` FOREIGN KEY (`pb_id`) REFERENCES `promotion_banner` (`pb_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for promotions_accumulate_bonus
-- ----------------------------
DROP TABLE IF EXISTS `promotions_accumulate_bonus`;
CREATE TABLE `promotions_accumulate_bonus` (
  `id` int(32) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `group_id` int(11) NOT NULL,
  `start` datetime NOT NULL,
  `end` datetime NOT NULL,
  `bonus_rate` int(3) unsigned NOT NULL,
  `extra_point` int(3) NOT NULL DEFAULT '0',
  `bonus_expire_day` int(3) NOT NULL,
  `new_user` tinyint(1) NOT NULL DEFAULT '0',
  `new_user_date` datetime NOT NULL,
  `repeat` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `present_time` int(3) NOT NULL DEFAULT '0',
  `created` datetime NOT NULL,
  `modified` datetime NOT NULL,
  `active` tinyint(1) NOT NULL DEFAULT '1',
  `muser` int(10) NOT NULL COMMENT '修改人',
  `event_desc` varchar(100) NOT NULL COMMENT '此為會出現於前台頁面之促銷活動文字',
  `event_type` varchar(10) NOT NULL COMMENT 'B1:購物金贈送',
  `condition_id` int(10) NOT NULL DEFAULT '0' COMMENT '會員條件',
  `device` tinyint(2) NOT NULL DEFAULT '1',
  `payment_code` varchar(50) NOT NULL DEFAULT '0' COMMENT '付款方式',
  `kuser` int(10) NOT NULL COMMENT '建立人',
  `status` tinyint(4) NOT NULL DEFAULT '1' COMMENT '是否已刪除;0:已刪除,1:未刪除',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for promotions_accumulate_bonus_history
-- ----------------------------
DROP TABLE IF EXISTS `promotions_accumulate_bonus_history`;
CREATE TABLE `promotions_accumulate_bonus_history` (
  `id` int(32) unsigned NOT NULL AUTO_INCREMENT,
  `user_id` int(10) unsigned NOT NULL,
  `order_id` int(10) unsigned NOT NULL,
  `promotion_id` int(10) unsigned NOT NULL,
  `create_time` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1210 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for promotions_accumulate_rate
-- ----------------------------
DROP TABLE IF EXISTS `promotions_accumulate_rate`;
CREATE TABLE `promotions_accumulate_rate` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `group_id` int(11) NOT NULL,
  `amount` int(11) NOT NULL,
  `bonus_type` int(11) NOT NULL,
  `point` int(11) NOT NULL,
  `dollar` int(11) NOT NULL,
  `start` datetime NOT NULL,
  `end` datetime NOT NULL,
  `created` datetime NOT NULL,
  `modified` datetime NOT NULL,
  `active` tinyint(1) NOT NULL DEFAULT '1',
  `condition_id` int(10) NOT NULL,
  `payment_type_rid` varchar(50) DEFAULT NULL COMMENT '付款方式',
  `status` int(10) NOT NULL DEFAULT '1',
  `kuser` int(10) DEFAULT '0' COMMENT '創建人',
  `muser` int(10) DEFAULT '0' COMMENT '更新人',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for promotions_amount_discount
-- ----------------------------
DROP TABLE IF EXISTS `promotions_amount_discount`;
CREATE TABLE `promotions_amount_discount` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `group_id` int(11) NOT NULL,
  `class_id` int(11) NOT NULL,
  `brand_id` int(11) NOT NULL,
  `category_id` int(11) unsigned NOT NULL,
  `product_id` int(11) NOT NULL,
  `type` int(11) NOT NULL,
  `amount` int(11) NOT NULL,
  `quantity` int(11) NOT NULL,
  `discount` int(11) NOT NULL,
  `vendor_coverage` int(11) NOT NULL,
  `start` datetime NOT NULL,
  `end` datetime NOT NULL,
  `created` datetime NOT NULL,
  `modified` datetime NOT NULL,
  `active` tinyint(1) NOT NULL DEFAULT '1',
  `event_desc` varchar(100) NOT NULL COMMENT '活動描述',
  `event_type` varchar(10) NOT NULL COMMENT '活動類型',
  `condition_id` int(10) NOT NULL COMMENT '會員條件id',
  `device` int(10) NOT NULL COMMENT '裝置 0.不分 1.pc 2.移動設備',
  `payment_code` int(10) NOT NULL,
  `kuser` varchar(50) DEFAULT NULL,
  `muser` varchar(50) DEFAULT NULL,
  `site` varchar(50) NOT NULL,
  `status` int(4) DEFAULT NULL,
  `url_by` tinyint(2) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `product_id` (`product_id`)
) ENGINE=InnoDB AUTO_INCREMENT=202 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for promotions_amount_fare
-- ----------------------------
DROP TABLE IF EXISTS `promotions_amount_fare`;
CREATE TABLE `promotions_amount_fare` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `display` varchar(255) NOT NULL,
  `delivery_store` int(11) NOT NULL DEFAULT '1',
  `group_id` int(11) NOT NULL,
  `class_id` int(11) NOT NULL,
  `brand_id` int(11) NOT NULL,
  `category_id` int(11) unsigned NOT NULL,
  `product_id` int(11) NOT NULL,
  `type` int(11) NOT NULL,
  `amount` int(11) NOT NULL,
  `quantity` int(11) NOT NULL,
  `start` datetime NOT NULL,
  `end` datetime NOT NULL,
  `created` datetime NOT NULL,
  `modified` datetime NOT NULL,
  `active` tinyint(1) NOT NULL DEFAULT '1',
  `muser` varchar(50) NOT NULL COMMENT '修改人',
  `event_desc` varchar(100) NOT NULL COMMENT '此為會出現於前台頁面之促銷活動文字',
  `event_type` varchar(10) NOT NULL COMMENT '活動類型 D1:滿額免運,D2:滿件免運',
  `condition_id` int(10) NOT NULL DEFAULT '0' COMMENT '會員條件',
  `device` tinyint(2) NOT NULL DEFAULT '1',
  `payment_code` varchar(50) NOT NULL DEFAULT '0' COMMENT '付款方式',
  `kuser` varchar(50) NOT NULL COMMENT '建立人',
  `fare_percent` int(10) NOT NULL COMMENT '收取原運費之百分比(若0則為免費)',
  `off_times` int(10) NOT NULL COMMENT '減免次數(0為不限)',
  `url_by` tinyint(2) NOT NULL,
  `status` int(9) NOT NULL DEFAULT '1',
  `site` varchar(50) NOT NULL,
  `vendor_coverage` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=100 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for promotions_amount_gift
-- ----------------------------
DROP TABLE IF EXISTS `promotions_amount_gift`;
CREATE TABLE `promotions_amount_gift` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `group_id` int(11) NOT NULL,
  `class_id` int(11) NOT NULL,
  `brand_id` int(11) NOT NULL,
  `category_id` int(11) unsigned NOT NULL,
  `product_id` int(11) NOT NULL,
  `type` int(11) NOT NULL,
  `amount` int(11) NOT NULL,
  `quantity` int(11) NOT NULL,
  `repeat` tinyint(1) NOT NULL,
  `gift_id` int(11) NOT NULL,
  `deduct_welfare` int(9) unsigned NOT NULL DEFAULT '0',
  `bonus_type` int(11) NOT NULL,
  `mailer_id` int(9) unsigned NOT NULL DEFAULT '0',
  `start` datetime NOT NULL,
  `end` datetime NOT NULL,
  `created` datetime NOT NULL,
  `modified` datetime NOT NULL,
  `active` tinyint(2) NOT NULL DEFAULT '1',
  `event_desc` varchar(100) NOT NULL COMMENT '此為會出現於前台頁面之促銷活動文字',
  `event_type` varchar(10) NOT NULL COMMENT '活動類型 G1:滿額送禮,G2:滿件送禮',
  `condition_id` int(10) NOT NULL DEFAULT '0' COMMENT '會員條件',
  `device` tinyint(2) NOT NULL DEFAULT '1',
  `payment_code` varchar(50) NOT NULL DEFAULT '0' COMMENT '付款方式',
  `gift_type` tinyint(2) NOT NULL DEFAULT '0' COMMENT '贈品類型 1:商品 2:機會 3:購物金 4:抵用券',
  `ticket_id` int(10) NOT NULL COMMENT '機會表id',
  `ticket_name` varchar(100) NOT NULL DEFAULT '''''',
  `count_by` tinyint(2) NOT NULL DEFAULT '1' COMMENT '次數限制依  1:無  2:訂單  3:會員',
  `number` int(10) NOT NULL,
  `num_limit` int(9) NOT NULL DEFAULT '0' COMMENT '活動次數(0不限)',
  `active_now` tinyint(2) NOT NULL COMMENT '當天就啟用 0:否 1:是',
  `valid_interval` smallint(4) NOT NULL DEFAULT '0' COMMENT '有效天數',
  `use_start` datetime NOT NULL COMMENT '啟用起日',
  `use_end` datetime NOT NULL COMMENT '啟用迄日',
  `kuser` varchar(50) NOT NULL COMMENT '建立人',
  `muser` varchar(50) NOT NULL COMMENT '修改人',
  `url_by` tinyint(10) NOT NULL COMMENT '是否專區:1:無專區 2:專區',
  `url` varchar(10) NOT NULL COMMENT '專區URL先綁預設或衍生新的專區頁面',
  `banner_file` varchar(10) NOT NULL COMMENT 'banner檔名',
  `status` int(2) NOT NULL DEFAULT '0' COMMENT '活動狀態 是否刪除',
  `site` varchar(50) NOT NULL,
  `vendor_coverage` int(11) NOT NULL DEFAULT '0',
  `gift_product_number` int(11) NOT NULL DEFAULT '0',
  `freight_price` int(9) NOT NULL DEFAULT '0' COMMENT '運費',
  `delivery_category` varchar(50) NOT NULL DEFAULT '',
  `event_id` varchar(10) DEFAULT NULL,
  `gift_mundane` int(10) DEFAULT '0',
  `bonus_state` int(9) DEFAULT NULL COMMENT '樣板區分0=一般、1=試吃、2紅利',
  `point` int(9) DEFAULT NULL COMMENT '非固定條件(紅利)',
  `dollar` int(9) DEFAULT NULL,
  `dividend` int(9) DEFAULT '0' COMMENT '紅利類型0.無1.點2:點+金3:金4比率固定5:非固定',
  PRIMARY KEY (`id`),
  KEY `gift_id` (`gift_id`)
) ENGINE=InnoDB AUTO_INCREMENT=758 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for promotions_amount_reduce
-- ----------------------------
DROP TABLE IF EXISTS `promotions_amount_reduce`;
CREATE TABLE `promotions_amount_reduce` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `delivery_store` int(11) NOT NULL COMMENT '物流廠商',
  `group_id` int(11) NOT NULL COMMENT '會員群組',
  `type` int(11) NOT NULL COMMENT '依運送類別',
  `amount` int(11) NOT NULL COMMENT '減免趴數',
  `quantity` int(11) NOT NULL COMMENT '減免次數',
  `start` datetime NOT NULL,
  `end` datetime NOT NULL,
  `created` datetime NOT NULL COMMENT '建立時間',
  `updatetime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '異動時間',
  `active` tinyint(1) NOT NULL DEFAULT '1' COMMENT '開關',
  `status` int(9) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=98 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for promotions_amount_reduce_member
-- ----------------------------
DROP TABLE IF EXISTS `promotions_amount_reduce_member`;
CREATE TABLE `promotions_amount_reduce_member` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(11) NOT NULL,
  `group_id` int(11) NOT NULL,
  `reduce_id` int(11) NOT NULL COMMENT '減免活動id',
  `order_id` int(11) NOT NULL,
  `order_type` tinyint(2) NOT NULL DEFAULT '0' COMMENT '運送方式使用狀況',
  `order_status` tinyint(2) NOT NULL DEFAULT '1',
  `created` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=122 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for promotions_amount_trial
-- ----------------------------
DROP TABLE IF EXISTS `promotions_amount_trial`;
CREATE TABLE `promotions_amount_trial` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL DEFAULT '' COMMENT '活動名稱',
  `event_type` varchar(10) DEFAULT NULL COMMENT '活動類型：T1：試吃 T2:試用',
  `event_id` varchar(10) DEFAULT NULL COMMENT '活動編號',
  `paper_id` int(11) DEFAULT NULL COMMENT '問卷調查id paper.paper_id',
  `url` varchar(255) DEFAULT NULL COMMENT 'url地址',
  `site` varchar(255) DEFAULT NULL COMMENT '活動隸屬的站台',
  `device` tinyint(2) DEFAULT NULL COMMENT '設備',
  `freight_type` int(11) DEFAULT NULL COMMENT '運送方式',
  `group_id` int(11) DEFAULT NULL COMMENT '會員群組',
  `condition_id` int(10) DEFAULT '0' COMMENT '會員條件',
  `count_by` tinyint(2) DEFAULT '1' COMMENT '次數限制依  1:無  2:訂單  3:會員',
  `num_limit` int(9) DEFAULT '0' COMMENT '活動次數(0不限)',
  `gift_mundane` int(10) DEFAULT NULL COMMENT '限量件數',
  `repeat` tinyint(1) DEFAULT NULL COMMENT '是否重複試用 0：否 1：是',
  `product_id` int(11) NOT NULL DEFAULT '0' COMMENT '參與活動的商品編號（已上架）',
  `product_name` varchar(255) NOT NULL DEFAULT '' COMMENT '商品在該活動中顯示的名稱【自定義】',
  `sale_productid` int(11) DEFAULT NULL COMMENT '可販售商品id',
  `brand_id` int(11) DEFAULT NULL COMMENT '品牌編號',
  `category_id` int(11) unsigned DEFAULT NULL COMMENT '類別號',
  `product_img` varchar(40) DEFAULT NULL COMMENT '商品圖片',
  `market_price` int(9) DEFAULT NULL COMMENT '市價【自定義】',
  `show_number` int(9) DEFAULT NULL COMMENT '開放數量',
  `apply_sum` int(9) DEFAULT '0' COMMENT '申請人數',
  `apply_limit` int(9) DEFAULT '0' COMMENT '申請上線',
  `event_img_small` varchar(40) NOT NULL DEFAULT '' COMMENT '活動小圖',
  `event_img` varchar(40) NOT NULL DEFAULT '' COMMENT '活動EDM大圖圖片',
  `event_desc` varchar(100) DEFAULT NULL COMMENT '活動描述',
  `active` tinyint(2) DEFAULT NULL COMMENT '活動狀態0：不啟用1：啟用',
  `start_date` datetime DEFAULT NULL COMMENT '活動開始時間',
  `end_date` datetime DEFAULT NULL COMMENT '活動結束時間',
  `created` datetime DEFAULT NULL COMMENT '創建時間',
  `modified` datetime DEFAULT NULL COMMENT '修改時間',
  `kuser` varchar(50) DEFAULT NULL COMMENT '建立人',
  `muser` varchar(50) DEFAULT NULL COMMENT '修改人',
  `status` int(2) NOT NULL DEFAULT '0' COMMENT '活動狀態0：無效數據 1：有效數據',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for promotions_bonus
-- ----------------------------
DROP TABLE IF EXISTS `promotions_bonus`;
CREATE TABLE `promotions_bonus` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) CHARACTER SET utf8 NOT NULL,
  `group_id` int(11) NOT NULL,
  `group_id_1` int(10) NOT NULL DEFAULT '0' COMMENT '綁定加入群組',
  `type` int(11) NOT NULL,
  `amount` int(11) NOT NULL,
  `days` int(11) NOT NULL,
  `new_user` tinyint(1) NOT NULL,
  `repeat` tinyint(1) NOT NULL,
  `multiple` tinyint(1) NOT NULL DEFAULT '0',
  `start` datetime NOT NULL,
  `end` datetime NOT NULL,
  `created` datetime NOT NULL,
  `modified` datetime NOT NULL,
  `active` tinyint(1) NOT NULL DEFAULT '1',
  `condition_id` int(10) NOT NULL COMMENT '是否設定會員條件;0:否(對應user_condition表中condition_id)',
  `status` int(10) NOT NULL DEFAULT '1',
  `kuser` int(10) DEFAULT '0' COMMENT '創建人',
  `muser` int(10) DEFAULT '0' COMMENT '更新人',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=101 DEFAULT CHARSET=utf32;

-- ----------------------------
-- Table structure for promotions_bonus_serial
-- ----------------------------
DROP TABLE IF EXISTS `promotions_bonus_serial`;
CREATE TABLE `promotions_bonus_serial` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `promotion_id` int(11) NOT NULL,
  `serial` varchar(32) NOT NULL,
  `active` tinyint(1) unsigned NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `serial` (`serial`)
) ENGINE=InnoDB AUTO_INCREMENT=15108 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for promotions_bonus_serial_history
-- ----------------------------
DROP TABLE IF EXISTS `promotions_bonus_serial_history`;
CREATE TABLE `promotions_bonus_serial_history` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `promotion_id` int(11) NOT NULL,
  `serial` varchar(32) NOT NULL,
  `user_id` int(11) NOT NULL,
  `created` datetime NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `acitve` (`promotion_id`,`serial`,`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=1973 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for promotions_deduct_rate
-- ----------------------------
DROP TABLE IF EXISTS `promotions_deduct_rate`;
CREATE TABLE `promotions_deduct_rate` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `group_id` int(11) NOT NULL,
  `amount` int(11) NOT NULL,
  `bonus_type` int(11) NOT NULL,
  `dollar` int(11) NOT NULL DEFAULT '1',
  `point` int(11) NOT NULL DEFAULT '1',
  `rate` int(11) NOT NULL,
  `start` datetime NOT NULL,
  `end` datetime NOT NULL,
  `created` datetime NOT NULL,
  `modified` datetime NOT NULL,
  `active` tinyint(1) NOT NULL DEFAULT '1',
  `condition_id` int(11) NOT NULL,
  `status` int(10) NOT NULL,
  `kuser` int(10) DEFAULT '0' COMMENT '創建人',
  `muser` int(10) DEFAULT '0' COMMENT '更新人',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for quiz_question_bank
-- ----------------------------
DROP TABLE IF EXISTS `quiz_question_bank`;
CREATE TABLE `quiz_question_bank` (
  `question_id` int(9) NOT NULL AUTO_INCREMENT COMMENT '題目id',
  `activity_id` int(9) NOT NULL COMMENT 'quiz_id 活動id',
  `content` varchar(2000) NOT NULL COMMENT '題目內容',
  `type` tinyint(4) NOT NULL COMMENT '類型:1選擇題,2填空題',
  `options` varchar(4000) DEFAULT NULL COMMENT '選項,非選擇題時為null,格式:A:選項a;B:選項b;',
  `answer` varchar(4000) NOT NULL COMMENT '答案',
  `question_desc` varchar(4000) NOT NULL COMMENT '問題解釋',
  `answer_desc` varchar(4000) NOT NULL COMMENT '答案說明',
  `point` float NOT NULL COMMENT '分數',
  `question_type` varchar(500) NOT NULL COMMENT '題型:誠心田園,健康休閒,ext.',
  PRIMARY KEY (`question_id`)
) ENGINE=InnoDB AUTO_INCREMENT=193 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for quiz_user_answer_detail
-- ----------------------------
DROP TABLE IF EXISTS `quiz_user_answer_detail`;
CREATE TABLE `quiz_user_answer_detail` (
  `question_id` int(9) NOT NULL,
  `user_id` int(10) NOT NULL,
  `answer` varchar(4000) NOT NULL,
  `answer_right` tinyint(1) NOT NULL,
  `answer_time` int(10) NOT NULL COMMENT '答題時間',
  PRIMARY KEY (`question_id`,`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for recommend_register_user
-- ----------------------------
DROP TABLE IF EXISTS `recommend_register_user`;
CREATE TABLE `recommend_register_user` (
  `rru_id` int(11) NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `rru_referral_id` int(11) NOT NULL COMMENT '被推薦者ID',
  `rru_recommended_by` int(11) NOT NULL COMMENT '推薦者ID',
  `rru_register_time` datetime NOT NULL COMMENT '被推薦者註冊時間',
  `rru_first_order_time` datetime DEFAULT NULL COMMENT '被推薦者首購時間',
  `rru_created` datetime NOT NULL COMMENT '建立時間',
  `rru_modified` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '最後修改',
  `state` tinyint(1) NOT NULL DEFAULT '0' COMMENT '狀態 0無使用 1 已使用',
  PRIMARY KEY (`rru_id`),
  KEY `ix_key` (`rru_referral_id`,`rru_recommended_by`,`state`)
) ENGINE=InnoDB AUTO_INCREMENT=230 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='推薦註冊使用者資料表';

-- ----------------------------
-- Table structure for recommended_product_attribute
-- ----------------------------
DROP TABLE IF EXISTS `recommended_product_attribute`;
CREATE TABLE `recommended_product_attribute` (
  `product_id` int(9) unsigned NOT NULL COMMENT '商品ID',
  `time_start` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '推薦販賣開始時間',
  `time_end` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '推薦販賣結束時間',
  `expend_day` int(9) unsigned NOT NULL DEFAULT '0' COMMENT '消耗天數',
  `months` varchar(100) DEFAULT NULL COMMENT '推薦商品月數',
  PRIMARY KEY (`product_id`),
  CONSTRAINT `recommended_product_attribute_ibfk_1` FOREIGN KEY (`product_id`) REFERENCES `product` (`product_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for recommended_product_attribute_temp
-- ----------------------------
DROP TABLE IF EXISTS `recommended_product_attribute_temp`;
CREATE TABLE `recommended_product_attribute_temp` (
  `product_id` int(9) unsigned NOT NULL COMMENT '產品Id',
  `write_id` int(4) NOT NULL COMMENT '用戶ID',
  `time_start` int(10) unsigned NOT NULL,
  `time_end` int(10) unsigned NOT NULL,
  `expend_day` int(9) unsigned NOT NULL COMMENT '推薦時間',
  `months` varchar(100) DEFAULT NULL COMMENT '推薦月數',
  `combo_type` int(4) DEFAULT NULL COMMENT '1表示單一商品  2表示組合商品'
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for redirect
-- ----------------------------
DROP TABLE IF EXISTS `redirect`;
CREATE TABLE `redirect` (
  `redirect_id` int(9) unsigned NOT NULL DEFAULT '0',
  `group_id` int(9) unsigned NOT NULL DEFAULT '0',
  `user_group_id` int(10) NOT NULL COMMENT '綁定加入群組',
  `redirect_name` varchar(255) NOT NULL DEFAULT '',
  `redirect_url` varchar(255) NOT NULL DEFAULT '',
  `redirect_status` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `redirect_total` int(10) unsigned NOT NULL DEFAULT '0',
  `redirect_note` varchar(255) DEFAULT NULL,
  `redirect_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `redirect_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `redirect_ipfrom` varchar(40) NOT NULL,
  PRIMARY KEY (`redirect_id`),
  KEY `ix_redirect_gid` (`group_id`),
  CONSTRAINT `fk_redirect_gid` FOREIGN KEY (`group_id`) REFERENCES `redirect_group` (`group_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for redirect_click
-- ----------------------------
DROP TABLE IF EXISTS `redirect_click`;
CREATE TABLE `redirect_click` (
  `redirect_id` int(9) unsigned NOT NULL,
  `click_id` int(10) unsigned NOT NULL,
  `click_year` smallint(4) unsigned NOT NULL,
  `click_month` tinyint(2) unsigned NOT NULL,
  `click_day` tinyint(2) unsigned NOT NULL,
  `click_hour` tinyint(2) unsigned NOT NULL,
  `click_week` tinyint(1) unsigned NOT NULL,
  `click_total` int(9) unsigned NOT NULL DEFAULT '1',
  PRIMARY KEY (`redirect_id`,`click_id`),
  KEY `ix_redirect_click_y` (`click_year`),
  KEY `ix_redirect_click_m` (`click_month`),
  CONSTRAINT `fk_redirect_click_rid` FOREIGN KEY (`redirect_id`) REFERENCES `redirect` (`redirect_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for redirect_group
-- ----------------------------
DROP TABLE IF EXISTS `redirect_group`;
CREATE TABLE `redirect_group` (
  `group_id` int(9) unsigned NOT NULL DEFAULT '0',
  `group_name` varchar(255) NOT NULL DEFAULT '',
  `group_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `group_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `group_ipfrom` varchar(40) NOT NULL,
  `group_type` varchar(20) DEFAULT NULL COMMENT '來源類型',
  PRIMARY KEY (`group_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for regional_table
-- ----------------------------
DROP TABLE IF EXISTS `regional_table`;
CREATE TABLE `regional_table` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `zip_3` int(3) NOT NULL COMMENT '郵遞3碼',
  `zip_5` int(5) DEFAULT NULL COMMENT '郵遞5碼',
  `counties` varchar(255) DEFAULT NULL COMMENT '縣市',
  `area` varchar(255) DEFAULT NULL COMMENT '區',
  `createtime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `id` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=62 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for sales_performance
-- ----------------------------
DROP TABLE IF EXISTS `sales_performance`;
CREATE TABLE `sales_performance` (
  `order_id` int(9) unsigned NOT NULL DEFAULT '0',
  `user_id` int(9) unsigned NOT NULL DEFAULT '0',
  `createdate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`order_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for sales_user
-- ----------------------------
DROP TABLE IF EXISTS `sales_user`;
CREATE TABLE `sales_user` (
  `s_id` float unsigned NOT NULL DEFAULT '0',
  `user_id` int(9) unsigned NOT NULL DEFAULT '0',
  `status` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `type` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `creator` int(9) unsigned NOT NULL DEFAULT '0',
  `createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `modifier` int(9) unsigned NOT NULL DEFAULT '0',
  `modify_time` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`s_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for schedule
-- ----------------------------
DROP TABLE IF EXISTS `schedule`;
CREATE TABLE `schedule` (
  `schedule_id` int(11) NOT NULL AUTO_INCREMENT,
  `schedule_name` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '排程名稱',
  `type` int(2) DEFAULT NULL COMMENT '1:重複執行,2單次執行',
  `execute_type` varchar(2) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '2M:按月執行 2W:按星期執行 2D:按天執行',
  `day_type` int(1) DEFAULT NULL COMMENT '1:單次執行 2:重複執行',
  `month_type` int(1) DEFAULT NULL COMMENT '1:單次執行 2:重複執行',
  `date_value` int(2) DEFAULT NULL COMMENT '2D:該值為null,2W:該值為0~4:2M:該值為1~31',
  `repeat_count` int(11) DEFAULT NULL COMMENT '重複的次數',
  `repeat_hours` int(11) DEFAULT NULL COMMENT '重複的小時數',
  `time_type` int(1) DEFAULT NULL COMMENT '時間單位(用於repeat_hours的後綴) 1:時2:分:3:秒',
  `week_day` varchar(13) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '星期',
  `start_time` time DEFAULT NULL COMMENT '執行時間精確到second ',
  `end_time` time DEFAULT NULL COMMENT '重複小時結束時間',
  `duration_start` date DEFAULT NULL COMMENT '該排程的開始時間',
  `duration_end` date DEFAULT NULL COMMENT '該排程的結束時間',
  `desc` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '描述',
  `create_user` int(11) DEFAULT NULL COMMENT '創建人',
  `create_date` datetime DEFAULT NULL COMMENT '創建時間',
  `execute_days` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '特殊排程的觸發時間',
  `trigger_time` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '特殊排程所對應的執行時間',
  PRIMARY KEY (`schedule_id`)
) ENGINE=InnoDB AUTO_INCREMENT=122 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for schedule_config
-- ----------------------------
DROP TABLE IF EXISTS `schedule_config`;
CREATE TABLE `schedule_config` (
  `rowid` int(9) NOT NULL AUTO_INCREMENT,
  `schedule_code` varchar(255) NOT NULL COMMENT 'schedule_master表schedule_code',
  `parameterCode` varchar(255) NOT NULL COMMENT '參數碼',
  `parameterName` varchar(255) NOT NULL COMMENT '參數作用',
  `value` varchar(255) NOT NULL COMMENT '參數值',
  `create_user` int(9) NOT NULL,
  `create_time` int(9) NOT NULL,
  `change_user` int(9) NOT NULL,
  `change_time` int(9) NOT NULL,
  PRIMARY KEY (`rowid`)
) ENGINE=InnoDB AUTO_INCREMENT=148 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for schedule_item
-- ----------------------------
DROP TABLE IF EXISTS `schedule_item`;
CREATE TABLE `schedule_item` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `item_name` varchar(90) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '細項名稱',
  `schedule_id` int(11) DEFAULT NULL COMMENT '關聯的排程id',
  `type` int(11) DEFAULT NULL COMMENT '排程屬於哪張table',
  `key1` int(11) DEFAULT NULL COMMENT '排程刪選條件1',
  `key2` int(11) DEFAULT NULL COMMENT '排程刪選條件2',
  `value1` varchar(45) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '條件1的值',
  `value2` varchar(45) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '條件2的值',
  `key3` int(11) DEFAULT NULL COMMENT '排程刪選條件3',
  `value3` varchar(45) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '條件3的值',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=145 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for schedule_log
-- ----------------------------
DROP TABLE IF EXISTS `schedule_log`;
CREATE TABLE `schedule_log` (
  `rowid` int(9) NOT NULL AUTO_INCREMENT,
  `schedule_code` varchar(255) NOT NULL COMMENT 'schedule表schedule_code',
  `create_user` int(9) NOT NULL,
  `create_time` int(9) NOT NULL,
  `ipfrom` varchar(255) NOT NULL,
  PRIMARY KEY (`rowid`)
) ENGINE=InnoDB AUTO_INCREMENT=639 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for schedule_master
-- ----------------------------
DROP TABLE IF EXISTS `schedule_master`;
CREATE TABLE `schedule_master` (
  `rowid` int(9) NOT NULL AUTO_INCREMENT,
  `schedule_code` varchar(255) NOT NULL COMMENT '排程Code',
  `schedule_name` varchar(255) NOT NULL COMMENT '排程名稱',
  `schedule_api` varchar(255) NOT NULL COMMENT 'contriller/action',
  `schedule_description` varchar(255) NOT NULL COMMENT '排程描述',
  `schedule_state` int(1) NOT NULL DEFAULT '0' COMMENT '排程狀態 0表示停用，1表示啟用',
  `previous_execute_time` int(9) NOT NULL DEFAULT '0' COMMENT '上次執行時間',
  `next_execute_time` int(9) NOT NULL DEFAULT '0' COMMENT '下次執行時間',
  `schedule_period_id` int(9) NOT NULL DEFAULT '0' COMMENT '下次執行的記錄  schedule_period表主鍵',
  `create_user` int(9) NOT NULL COMMENT '創建人',
  `create_time` int(9) NOT NULL COMMENT '創建日期',
  `change_user` int(9) NOT NULL COMMENT '修改人',
  `change_time` int(9) NOT NULL COMMENT '更改日期',
  PRIMARY KEY (`rowid`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for schedule_period
-- ----------------------------
DROP TABLE IF EXISTS `schedule_period`;
CREATE TABLE `schedule_period` (
  `rowid` int(9) NOT NULL AUTO_INCREMENT,
  `schedule_code` varchar(255) NOT NULL COMMENT 'schedule_master表schedule_code',
  `period_type` int(9) unsigned NOT NULL COMMENT '執行頻率方式（year、month、week.....）',
  `period_nums` int(9) unsigned NOT NULL COMMENT '執行頻率的倍數',
  `begin_datetime` int(9) NOT NULL COMMENT '啟用時間',
  `current_nums` int(9) unsigned NOT NULL DEFAULT '0' COMMENT '當前已執行次數',
  `limit_nums` int(9) unsigned NOT NULL DEFAULT '0' COMMENT '次數限制(0代表不限制)',
  `create_user` int(9) NOT NULL,
  `create_time` int(9) NOT NULL,
  `change_user` int(9) NOT NULL,
  `change_time` int(9) NOT NULL,
  PRIMARY KEY (`rowid`)
) ENGINE=InnoDB AUTO_INCREMENT=39 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for schedule_relation
-- ----------------------------
DROP TABLE IF EXISTS `schedule_relation`;
CREATE TABLE `schedule_relation` (
  `rowid` int(11) NOT NULL AUTO_INCREMENT,
  `schedule_id` int(11) DEFAULT NULL COMMENT 'schedule.schedule_id',
  `relation_table` varchar(25) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'schedule與之關聯的表',
  `relation_id` int(11) DEFAULT NULL COMMENT 'relation表的主鍵',
  PRIMARY KEY (`rowid`),
  KEY `schedule_id` (`schedule_id`),
  CONSTRAINT `schedule_relation_ibfk_1` FOREIGN KEY (`schedule_id`) REFERENCES `schedule` (`schedule_id`)
) ENGINE=InnoDB AUTO_INCREMENT=57 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for secret_account_set
-- ----------------------------
DROP TABLE IF EXISTS `secret_account_set`;
CREATE TABLE `secret_account_set` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` smallint(4) unsigned NOT NULL COMMENT '會員id  管理manager_user主鍵user_id',
  `secret_pwd` varchar(64) NOT NULL COMMENT '查詢機密資料的密碼 確保和會員登錄密碼不一致',
  `ipfrom` varchar(40) NOT NULL COMMENT '機敏資料查詢ip設定',
  `user_login_attempts` tinyint(2) NOT NULL DEFAULT '0' COMMENT '查詢機敏資料登入失敗次數控制 錯誤一次數字加1 數字為5，賬號停用 status為0；密碼正確，數字設定為0',
  `secret_limit` smallint(4) NOT NULL DEFAULT '0' COMMENT '輸入密碼5分鐘之後，最大可查詢的數據最大值',
  `secret_count` smallint(4) NOT NULL DEFAULT '0' COMMENT '5分鐘內該用戶查詢的機敏信息次數',
  `createdate` datetime NOT NULL,
  `updatedate` datetime NOT NULL,
  `status` tinyint(2) NOT NULL COMMENT '狀態 0：無效數據 1：有效數據 ',
  `pwd_status` tinyint(2) NOT NULL DEFAULT '0' COMMENT '密碼狀態  管理員修改重置密碼之後，狀態為0 ；用戶第一次登陸修改密碼之後，狀態為1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `user_ip` (`user_id`,`ipfrom`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=95 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for secret_info_log
-- ----------------------------
DROP TABLE IF EXISTS `secret_info_log`;
CREATE TABLE `secret_info_log` (
  `log_id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` smallint(4) unsigned NOT NULL COMMENT '會員編號  關聯 manager_user表',
  `createdate` datetime NOT NULL COMMENT '機密資料訪問時間',
  `ipfrom` varchar(40) NOT NULL COMMENT '訪問機密資料ip',
  `url` varchar(255) NOT NULL COMMENT '訪問機密資料url',
  `type` int(2) NOT NULL DEFAULT '0' COMMENT '機敏資料類型  關聯t_parameter_src表  secret_type',
  `related_id` int(11) NOT NULL DEFAULT '0' COMMENT '查詢的相對應的機敏資料數據的id ',
  `input_pwd_date` datetime DEFAULT NULL COMMENT '输入密码时间',
  PRIMARY KEY (`log_id`)
) ENGINE=InnoDB AUTO_INCREMENT=21417 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for send_vendor_mail
-- ----------------------------
DROP TABLE IF EXISTS `send_vendor_mail`;
CREATE TABLE `send_vendor_mail` (
  `vendor_id` int(9) unsigned NOT NULL DEFAULT '0',
  `vendor_name_simple` varchar(50) NOT NULL DEFAULT '',
  `vendor_email` varchar(50) NOT NULL DEFAULT '',
  PRIMARY KEY (`vendor_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for send_vendor_mail_detail
-- ----------------------------
DROP TABLE IF EXISTS `send_vendor_mail_detail`;
CREATE TABLE `send_vendor_mail_detail` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `vendor_id` int(9) NOT NULL DEFAULT '0',
  `order_id` int(9) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=38 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for sendmail
-- ----------------------------
DROP TABLE IF EXISTS `sendmail`;
CREATE TABLE `sendmail` (
  `rowid` int(4) NOT NULL AUTO_INCREMENT,
  `mailfrom` varchar(50) NOT NULL COMMENT 'mailfrom',
  `mailto` varchar(2000) NOT NULL COMMENT 'mailTo',
  `subject` varchar(2000) NOT NULL COMMENT '發信主旨',
  `mailbody` varchar(8000) NOT NULL COMMENT '發信內容',
  `status` tinyint(1) NOT NULL DEFAULT '0' COMMENT '0:未發送 1:已發送',
  `kuser` varchar(50) NOT NULL COMMENT '輸入人員',
  `kdate` datetime NOT NULL COMMENT '輸入時間',
  `senddate` datetime NOT NULL COMMENT '發送時間',
  `source` varchar(50) NOT NULL COMMENT '發信來源',
  `weight` int(11) NOT NULL DEFAULT '1' COMMENT '發信權重',
  PRIMARY KEY (`rowid`)
) ENGINE=InnoDB AUTO_INCREMENT=50 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for serial
-- ----------------------------
DROP TABLE IF EXISTS `serial`;
CREATE TABLE `serial` (
  `serial_id` smallint(4) unsigned NOT NULL,
  `serial_value` bigint(19) unsigned NOT NULL,
  `serial_description` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`serial_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for session
-- ----------------------------
DROP TABLE IF EXISTS `session`;
CREATE TABLE `session` (
  `session_id` char(32) NOT NULL DEFAULT '',
  `session_type` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `session_user_id` int(9) unsigned NOT NULL DEFAULT '0',
  `session_start` int(10) unsigned NOT NULL DEFAULT '0',
  `session_end` int(10) unsigned NOT NULL DEFAULT '0',
  `session_web` smallint(4) unsigned NOT NULL DEFAULT '0',
  `session_site` smallint(4) unsigned NOT NULL DEFAULT '0',
  `session_ip` varchar(40) NOT NULL DEFAULT '',
  `session_browser` varchar(255) NOT NULL DEFAULT '',
  PRIMARY KEY (`session_id`),
  KEY `ix_session_end` (`session_end`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for session_key
-- ----------------------------
DROP TABLE IF EXISTS `session_key`;
CREATE TABLE `session_key` (
  `key_id` char(32) NOT NULL DEFAULT '',
  `key_type` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `key_user_id` int(9) unsigned NOT NULL DEFAULT '0',
  `key_start` int(10) unsigned NOT NULL DEFAULT '0',
  `key_end` int(10) unsigned NOT NULL DEFAULT '0',
  `key_ip` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`key_id`),
  KEY `ix_session_key_end` (`key_end`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for shipping_carrior
-- ----------------------------
DROP TABLE IF EXISTS `shipping_carrior`;
CREATE TABLE `shipping_carrior` (
  `rid` int(11) NOT NULL AUTO_INCREMENT,
  `delivery_store_id` int(11) NOT NULL COMMENT '物流公司',
  `freight_big_area` tinyint(2) NOT NULL DEFAULT '0' COMMENT '配送區域',
  `freight_type` smallint(4) NOT NULL DEFAULT '0' COMMENT '物流配送模式',
  `delivery_freight_set` tinyint(2) NOT NULL DEFAULT '0' COMMENT '常低溫, 1:常溫  2:低溫',
  `active` tinyint(2) NOT NULL DEFAULT '0' COMMENT '啟用 ',
  `charge_type` tinyint(2) NOT NULL DEFAULT '0' COMMENT '收費  1.固定  2.累加',
  `shipping_fee` int(10) DEFAULT NULL COMMENT '運費',
  `return_fee` int(10) DEFAULT NULL COMMENT '逆運費',
  `size_limitation` tinyint(2) NOT NULL COMMENT '有size限制',
  `length` int(10) DEFAULT NULL COMMENT '限長',
  `width` int(10) DEFAULT NULL COMMENT '限寬',
  `height` int(10) DEFAULT NULL COMMENT '限高',
  `weight` int(10) DEFAULT NULL COMMENT '限重',
  `pod` tinyint(2) NOT NULL COMMENT '可貨到付款',
  `note` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`rid`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for shipping_voucher
-- ----------------------------
DROP TABLE IF EXISTS `shipping_voucher`;
CREATE TABLE `shipping_voucher` (
  `sv_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水編號',
  `user_id` int(9) NOT NULL COMMENT '會員編號',
  `order_id` int(11) DEFAULT NULL COMMENT '訂單編號',
  `sv_state` int(11) NOT NULL COMMENT '免運?狀態: 0)未使用 1)已使用 2)未使用已到期',
  `sv_year` int(11) NOT NULL COMMENT '免運?發放年份',
  `sv_month` int(11) NOT NULL COMMENT '免運?發放月份',
  `sv_created` datetime NOT NULL COMMENT '建立時間',
  `sv_modified` datetime NOT NULL COMMENT '最後修改時間',
  `sv_note` varchar(255) DEFAULT NULL COMMENT '備註',
  PRIMARY KEY (`sv_id`),
  KEY `user_id` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for shop_class
-- ----------------------------
DROP TABLE IF EXISTS `shop_class`;
CREATE TABLE `shop_class` (
  `class_id` int(9) unsigned NOT NULL DEFAULT '0',
  `class_name` varchar(255) NOT NULL DEFAULT '',
  `class_sort` smallint(4) unsigned NOT NULL DEFAULT '0',
  `class_status` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `class_content` text NOT NULL,
  `class_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `class_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `class_ipfrom` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`class_id`),
  KEY `ix_shop_class_st` (`class_sort`),
  KEY `ix_shop_class_ss` (`class_status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for shopping_cart
-- ----------------------------
DROP TABLE IF EXISTS `shopping_cart`;
CREATE TABLE `shopping_cart` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `cart_id` char(32) NOT NULL DEFAULT '',
  `item_id` int(9) unsigned NOT NULL DEFAULT '0',
  `cart_delivery` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `cart_num` mediumint(7) unsigned NOT NULL DEFAULT '0',
  `cart_start` int(10) unsigned NOT NULL DEFAULT '0',
  `cart_end` int(10) unsigned NOT NULL DEFAULT '0',
  `created` datetime NOT NULL,
  `modified` datetime NOT NULL,
  `combined_mode` int(11) NOT NULL,
  `parent_id` int(11) NOT NULL,
  `pack_id` int(10) unsigned NOT NULL DEFAULT '1',
  `item_mode` smallint(2) unsigned NOT NULL COMMENT '0:單一商品, 1:父商品, 2:子商品',
  `site_id` int(9) NOT NULL DEFAULT '1',
  `price_master_id` int(10) unsigned NOT NULL DEFAULT '1',
  `event_id` varchar(16) NOT NULL,
  `general_type` varchar(16) NOT NULL,
  `client_ip` varchar(40) DEFAULT NULL,
  `user_id` int(9) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `cart_id` (`cart_id`,`item_id`,`cart_delivery`,`parent_id`,`id`),
  KEY `ix_shopping_cart_ed` (`cart_end`),
  KEY `fk_shopping_cart_iid` (`item_id`),
  KEY `parent_id` (`parent_id`),
  KEY `user_id` (`user_id`),
  CONSTRAINT `fk_shopping_cart_iid` FOREIGN KEY (`item_id`) REFERENCES `product_item` (`item_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=309414 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for shopping_carts
-- ----------------------------
DROP TABLE IF EXISTS `shopping_carts`;
CREATE TABLE `shopping_carts` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `cart_id` char(32) NOT NULL DEFAULT '',
  `item_id` int(9) unsigned NOT NULL DEFAULT '0',
  `cart_delivery` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `cart_num` mediumint(7) unsigned NOT NULL DEFAULT '0',
  `cart_start` int(10) unsigned NOT NULL DEFAULT '0',
  `cart_end` int(10) unsigned NOT NULL DEFAULT '0',
  `created` datetime NOT NULL,
  `modified` datetime NOT NULL,
  `combined_mode` int(11) NOT NULL,
  `parent_id` int(11) NOT NULL,
  `pack_id` smallint(2) unsigned NOT NULL DEFAULT '0',
  `pile_id` smallint(2) unsigned NOT NULL DEFAULT '0',
  `item_mode` smallint(2) unsigned NOT NULL COMMENT '0:單一商品, 1:父商品, 2:子商品',
  `site_id` int(9) NOT NULL DEFAULT '0',
  `price_master_id` int(10) unsigned NOT NULL DEFAULT '1',
  `event_id` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4490 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for sigh
-- ----------------------------
DROP TABLE IF EXISTS `sigh`;
CREATE TABLE `sigh` (
  `sigh_id` int(9) unsigned NOT NULL,
  `question_username` varchar(64) NOT NULL DEFAULT '',
  `question_email` varchar(255) NOT NULL DEFAULT '',
  `question_phone` varchar(50) NOT NULL,
  `question_company` varchar(64) NOT NULL DEFAULT '''''',
  `question_status` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `question_content` text NOT NULL,
  `type` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `question_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `question_ipfrom` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`sigh_id`),
  KEY `ix_contact_us_question_ss` (`question_status`),
  KEY `ix_contact_us_question_ce` (`question_createdate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for sinopac_detail
-- ----------------------------
DROP TABLE IF EXISTS `sinopac_detail`;
CREATE TABLE `sinopac_detail` (
  `sinopac_detail_id` int(9) NOT NULL,
  `order_id` int(9) NOT NULL,
  `sinopac_id` varchar(20) NOT NULL,
  `pay_amount` int(9) NOT NULL,
  `entday` int(10) NOT NULL,
  `txday` int(10) NOT NULL,
  `trmseq` char(2) NOT NULL,
  `txtno` int(5) NOT NULL,
  `error` tinyint(1) NOT NULL DEFAULT '0',
  `sinopac_createdate` int(10) DEFAULT NULL,
  PRIMARY KEY (`sinopac_detail_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for site
-- ----------------------------
DROP TABLE IF EXISTS `site`;
CREATE TABLE `site` (
  `site_id` int(9) unsigned NOT NULL AUTO_INCREMENT COMMENT '流水號，預設1為吉甲地',
  `site_name` varchar(50) NOT NULL COMMENT '站台名稱',
  `domain` varchar(250) DEFAULT NULL,
  `cart_delivery` mediumint(7) unsigned NOT NULL,
  `online_user` int(9) NOT NULL DEFAULT '0' COMMENT '站台人數',
  `max_user` int(9) NOT NULL DEFAULT '0' COMMENT '最大人數',
  `page_location` varchar(255) NOT NULL DEFAULT '/' COMMENT '站台首頁',
  `checkout_location` varchar(32) NOT NULL DEFAULT '/shoppings/cart/' COMMENT '結帳頁面',
  `product_location` varchar(50) NOT NULL DEFAULT '/product.php?pid=' COMMENT '商品賣場位置',
  `site_status` int(4) NOT NULL DEFAULT '1',
  `site_createdate` datetime NOT NULL,
  `site_updatedate` datetime DEFAULT NULL,
  `create_userid` int(9) NOT NULL,
  `update_userid` int(9) DEFAULT NULL,
  PRIMARY KEY (`site_id`),
  KEY `site_id` (`site_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=105 DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT COMMENT='站台資訊';

-- ----------------------------
-- Table structure for site_analytics
-- ----------------------------
DROP TABLE IF EXISTS `site_analytics`;
CREATE TABLE `site_analytics` (
  `sa_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `sa_pageviews` int(11) NOT NULL COMMENT '瀏覽量',
  `sa_date` date NOT NULL COMMENT '日索引',
  `sa_user` int(11) NOT NULL DEFAULT '0' COMMENT '使用者',
  `sa_pages_session` float(11,2) NOT NULL COMMENT '單次造訪頁數',
  `sa_bounce_rate` float(11,2) NOT NULL COMMENT '跳出率',
  `sa_create_time` datetime NOT NULL COMMENT '創建時間',
  `sa_create_user` int(11) NOT NULL COMMENT '創建人',
  `sa_modify_time` datetime NOT NULL COMMENT '異動時間',
  `sa_modify_user` int(11) NOT NULL COMMENT '異動人員',
  `sa_session` int(11) NOT NULL COMMENT '造訪數',
  `sa_avg_session_duration` float(11,5) NOT NULL COMMENT '平均停留時間',
  PRIMARY KEY (`sa_id`)
) ENGINE=InnoDB AUTO_INCREMENT=112 DEFAULT CHARSET=utf8 COMMENT='Analytics目標對';

-- ----------------------------
-- Table structure for site_page
-- ----------------------------
DROP TABLE IF EXISTS `site_page`;
CREATE TABLE `site_page` (
  `page_id` int(9) NOT NULL AUTO_INCREMENT,
  `page_name` varchar(50) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `page_url` varchar(50) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `page_status` int(4) NOT NULL,
  `page_html` varchar(100) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `page_desc` varchar(100) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `page_createdate` datetime NOT NULL,
  `page_updatedate` datetime DEFAULT NULL,
  `create_userid` int(9) NOT NULL,
  `update_userid` int(9) DEFAULT NULL,
  PRIMARY KEY (`page_id`),
  KEY `page_id` (`page_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=97 DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for site_statistics
-- ----------------------------
DROP TABLE IF EXISTS `site_statistics`;
CREATE TABLE `site_statistics` (
  `ss_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `ss_show_num` int(11) NOT NULL DEFAULT '0' COMMENT '曝光數',
  `ss_click_num` int(11) NOT NULL DEFAULT '0' COMMENT '點擊',
  `ss_click_through` float(11,2) NOT NULL DEFAULT '0.00' COMMENT '點閱率',
  `ss_cost` float(11,2) NOT NULL COMMENT '費用',
  `ss_newuser_number` int(11) NOT NULL COMMENT '新會員數',
  `ss_converted_newuser` int(11) NOT NULL COMMENT '實際轉換會員',
  `ss_sum_order_amount` int(11) NOT NULL COMMENT '訂單金額',
  `ss_date` date NOT NULL COMMENT '時間',
  `ss_code` varchar(16) NOT NULL COMMENT '廠家代碼',
  `ss_create_time` datetime NOT NULL COMMENT '創建時間',
  `ss_create_user` int(11) NOT NULL COMMENT '創建人',
  `ss_modify_time` datetime NOT NULL COMMENT '更新時間',
  `ss_modify_user` int(11) NOT NULL COMMENT '修改人',
  PRIMARY KEY (`ss_id`)
) ENGINE=InnoDB AUTO_INCREMENT=41 DEFAULT CHARSET=utf8 COMMENT='吉甲地訪問統計';

-- ----------------------------
-- Table structure for sms
-- ----------------------------
DROP TABLE IF EXISTS `sms`;
CREATE TABLE `sms` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `type` int(11) NOT NULL COMMENT '7：ATM異常提醒簡訊第4天，8：ATM異常提醒簡訊第6天，9：電話會員開立發票歸檔，12：到店提醒，13：店配拆單提醒',
  `order_id` int(11) NOT NULL,
  `mobile` varchar(50) NOT NULL,
  `subject` varchar(255) NOT NULL,
  `content` text NOT NULL,
  `estimated_send_time` datetime NOT NULL,
  `send` tinyint(2) NOT NULL,
  `sms_number` varchar(20) DEFAULT NULL COMMENT '誠邦單號',
  `trust_send` varchar(7) NOT NULL DEFAULT '0' COMMENT '誠邦寄送狀態：0發送中、1成功、2失敗、3回補、4失敗歸檔',
  `memo` varchar(255) DEFAULT NULL COMMENT '備註',
  `created` datetime NOT NULL,
  `modified` datetime NOT NULL,
  `serial_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `send` (`send`),
  KEY `order_id` (`order_id`)
) ENGINE=InnoDB AUTO_INCREMENT=119666 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for sms_free_log
-- ----------------------------
DROP TABLE IF EXISTS `sms_free_log`;
CREATE TABLE `sms_free_log` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `sms_id` int(11) NOT NULL,
  `msgid` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '簡訊單號或錯誤訊息',
  `status` tinyint(1) NOT NULL,
  `createtime` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '電信成功時間',
  `updatetime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '此筆資料時間',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=502671 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for sms_history
-- ----------------------------
DROP TABLE IF EXISTS `sms_history`;
CREATE TABLE `sms_history` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `order_id` int(9) unsigned NOT NULL DEFAULT '0',
  `sn` varchar(32) CHARACTER SET utf8 NOT NULL DEFAULT '''''',
  `sms_balance` int(9) unsigned NOT NULL DEFAULT '0',
  `type` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `create_time` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8710 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for sms_log
-- ----------------------------
DROP TABLE IF EXISTS `sms_log`;
CREATE TABLE `sms_log` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `sms_id` int(11) NOT NULL,
  `provider` int(11) NOT NULL,
  `success` tinyint(1) NOT NULL,
  `code` varchar(8) NOT NULL,
  `free_sms_id` varchar(20) CHARACTER SET utf8 DEFAULT NULL,
  `created` datetime NOT NULL,
  `modified` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `sms_id` (`sms_id`),
  KEY `created` (`created`,`provider`,`success`)
) ENGINE=InnoDB AUTO_INCREMENT=222419 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for specifics
-- ----------------------------
DROP TABLE IF EXISTS `specifics`;
CREATE TABLE `specifics` (
  `specific_id` int(11) NOT NULL AUTO_INCREMENT,
  `event_title` varchar(100) NOT NULL,
  `event_start` int(11) NOT NULL,
  `event_end` int(11) NOT NULL,
  `activity` int(11) NOT NULL DEFAULT '0',
  `items` varchar(255) NOT NULL,
  `inspect` enum('single','amount') NOT NULL DEFAULT 'single',
  `quantity` int(11) NOT NULL DEFAULT '1',
  `message` varchar(255) NOT NULL,
  PRIMARY KEY (`specific_id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for split_single_remind
-- ----------------------------
DROP TABLE IF EXISTS `split_single_remind`;
CREATE TABLE `split_single_remind` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `order_id` int(11) NOT NULL,
  `status` int(1) NOT NULL,
  `set_time` datetime NOT NULL,
  `up_time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for staff_gifts
-- ----------------------------
DROP TABLE IF EXISTS `staff_gifts`;
CREATE TABLE `staff_gifts` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `staff_id` int(10) DEFAULT NULL,
  `name` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `mail` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=42 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for Status1
-- ----------------------------
DROP TABLE IF EXISTS `Status1`;
CREATE TABLE `Status1` (
  `ST101` int(11) NOT NULL AUTO_INCREMENT COMMENT '狀態ID',
  `ST102` varchar(20) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL COMMENT '狀態名稱',
  PRIMARY KEY (`ST101`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for Status2
-- ----------------------------
DROP TABLE IF EXISTS `Status2`;
CREATE TABLE `Status2` (
  `ST201` int(11) NOT NULL COMMENT 'Status1狀態ID',
  `ST202` int(11) NOT NULL COMMENT '狀態ID',
  `ST203` varchar(20) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL COMMENT '狀態名稱'
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for stock_test
-- ----------------------------
DROP TABLE IF EXISTS `stock_test`;
CREATE TABLE `stock_test` (
  `b_name` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id` int(11) DEFAULT NULL,
  `p_name` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `site_id` int(1) DEFAULT NULL,
  `price` int(10) DEFAULT NULL,
  `cost` int(10) DEFAULT NULL,
  `event_price` int(10) DEFAULT NULL,
  `event_cost` int(10) DEFAULT NULL,
  `y_cost` int(10) DEFAULT NULL,
  `bag` int(10) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for suppliers_user_token
-- ----------------------------
DROP TABLE IF EXISTS `suppliers_user_token`;
CREATE TABLE `suppliers_user_token` (
  `user_email` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `access_token` varchar(128) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `expired` int(10) unsigned DEFAULT NULL,
  `modDate` int(10) unsigned DEFAULT NULL,
  PRIMARY KEY (`user_email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for t_buyinfo_temp
-- ----------------------------
DROP TABLE IF EXISTS `t_buyinfo_temp`;
CREATE TABLE `t_buyinfo_temp` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `batch_no` varchar(50) DEFAULT NULL,
  `item_id` int(8) DEFAULT NULL,
  `buy_num` int(8) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=203 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for t_errorlog
-- ----------------------------
DROP TABLE IF EXISTS `t_errorlog`;
CREATE TABLE `t_errorlog` (
  `rowid` int(11) NOT NULL AUTO_INCREMENT,
  `log_date` datetime DEFAULT NULL COMMENT 'log时间',
  `Thread` varchar(255) DEFAULT NULL COMMENT '线程',
  `Level` varchar(100) DEFAULT NULL COMMENT '级别',
  `logger` varchar(510) DEFAULT NULL COMMENT '名称空间',
  `message` longtext COMMENT '出現錯誤讯息',
  `method` varchar(50) DEFAULT NULL COMMENT '出错的方法',
  PRIMARY KEY (`rowid`)
) ENGINE=InnoDB AUTO_INCREMENT=21943 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for t_fgroup
-- ----------------------------
DROP TABLE IF EXISTS `t_fgroup`;
CREATE TABLE `t_fgroup` (
  `rowid` int(11) NOT NULL AUTO_INCREMENT,
  `groupName` varchar(100) DEFAULT NULL COMMENT '群组名称',
  `groupCode` varchar(50) DEFAULT NULL COMMENT '群组编码',
  `remark` varchar(600) DEFAULT NULL COMMENT '备注',
  `kuser` varchar(50) DEFAULT NULL,
  `kdate` datetime DEFAULT NULL,
  PRIMARY KEY (`rowid`)
) ENGINE=InnoDB AUTO_INCREMENT=49 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for t_fgroup2
-- ----------------------------
DROP TABLE IF EXISTS `t_fgroup2`;
CREATE TABLE `t_fgroup2` (
  `rowid` int(11) NOT NULL AUTO_INCREMENT,
  `groupName` varchar(100) DEFAULT NULL COMMENT '群组名稱',
  `groupCode` varchar(50) DEFAULT NULL COMMENT '群組編碼',
  `remark` varchar(600) DEFAULT NULL COMMENT '備註',
  `kuser` varchar(50) DEFAULT NULL,
  `kdate` datetime DEFAULT NULL,
  PRIMARY KEY (`rowid`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for t_function
-- ----------------------------
DROP TABLE IF EXISTS `t_function`;
CREATE TABLE `t_function` (
  `rowid` int(11) NOT NULL AUTO_INCREMENT,
  `functionType` int(11) DEFAULT NULL COMMENT '功能类型(1.页面，2.控件，3.僅為其他頁面子頁面)',
  `functionGroup` varchar(100) DEFAULT NULL COMMENT '功能模组(页面存模块名称，控件存页面名称)',
  `functionName` varchar(100) DEFAULT NULL COMMENT '功能名称',
  `functionCode` varchar(50) DEFAULT NULL COMMENT '功能代码(页面存路径，控件存ID)',
  `iconCls` varchar(50) DEFAULT NULL,
  `isEdit` tinyint(4) DEFAULT '1' COMMENT '是否可設置編輯權限',
  `remark` varchar(600) DEFAULT NULL COMMENT '描述',
  `kuser` varchar(50) DEFAULT NULL,
  `kdate` datetime DEFAULT NULL,
  `topValue` int(11) DEFAULT NULL COMMENT '控件父級頁面id',
  PRIMARY KEY (`rowid`)
) ENGINE=InnoDB AUTO_INCREMENT=972 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for t_functiongroup
-- ----------------------------
DROP TABLE IF EXISTS `t_functiongroup`;
CREATE TABLE `t_functiongroup` (
  `rowid` int(11) NOT NULL AUTO_INCREMENT,
  `functionId` int(11) DEFAULT NULL COMMENT 't_function.rowid',
  `groupId` int(11) DEFAULT NULL COMMENT 't_fgroup.rowid',
  `isView` tinyint(2) DEFAULT '1' COMMENT '是否可查看',
  `isEdit` tinyint(2) DEFAULT '1' COMMENT '是否可編輯',
  `kdate` datetime DEFAULT NULL,
  `kuser` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`rowid`),
  KEY `FK_t_functionGroup_t_function` (`functionId`),
  KEY `FK_t_functionGroup_t_fgroup` (`groupId`)
) ENGINE=InnoDB AUTO_INCREMENT=220713 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for t_functiongroup2
-- ----------------------------
DROP TABLE IF EXISTS `t_functiongroup2`;
CREATE TABLE `t_functiongroup2` (
  `rowid` int(11) NOT NULL AUTO_INCREMENT,
  `functionId` int(11) DEFAULT NULL COMMENT 't_function.rowid',
  `groupId` int(11) DEFAULT NULL COMMENT 't_fgroup2.rowid',
  `kdate` datetime DEFAULT NULL,
  `kuser` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`rowid`),
  KEY `FK_t_functionGroup2_t_function` (`functionId`),
  KEY `FK_t_functionGroup2_t_fgroup2` (`groupId`),
  CONSTRAINT `FK_t_functionGroup2_t_fgroup2` FOREIGN KEY (`groupId`) REFERENCES `t_fgroup2` (`rowid`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_t_functionGroup2_t_function` FOREIGN KEY (`functionId`) REFERENCES `t_function` (`rowid`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for t_groupcaller
-- ----------------------------
DROP TABLE IF EXISTS `t_groupcaller`;
CREATE TABLE `t_groupcaller` (
  `rowid` int(11) NOT NULL AUTO_INCREMENT,
  `groupId` int(11) DEFAULT NULL COMMENT 't_fgroup.rowid',
  `callid` varchar(50) DEFAULT NULL COMMENT '吉甲地管理员编号',
  PRIMARY KEY (`rowid`),
  KEY `FK_t_groupCaller_t_fgroup` (`groupId`),
  CONSTRAINT `FK_t_groupCaller_t_fgroup` FOREIGN KEY (`groupId`) REFERENCES `t_fgroup` (`rowid`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=2538 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for t_groupcaller_copy
-- ----------------------------
DROP TABLE IF EXISTS `t_groupcaller_copy`;
CREATE TABLE `t_groupcaller_copy` (
  `rowid` int(11) NOT NULL AUTO_INCREMENT,
  `groupId` int(11) DEFAULT NULL COMMENT 't_fgroup.rowid',
  `callid` varchar(50) DEFAULT NULL COMMENT '吉甲地管理员编号',
  PRIMARY KEY (`rowid`),
  KEY `FK_t_groupCaller_t_fgroup` (`groupId`),
  CONSTRAINT `t_groupcaller_copy_ibfk_1` FOREIGN KEY (`groupId`) REFERENCES `t_fgroup` (`rowid`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=1555 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for t_groupcaller2
-- ----------------------------
DROP TABLE IF EXISTS `t_groupcaller2`;
CREATE TABLE `t_groupcaller2` (
  `rowid` int(11) NOT NULL AUTO_INCREMENT,
  `groupId` int(11) DEFAULT NULL COMMENT 't_fgroup2.rowid',
  `callid` varchar(50) DEFAULT NULL COMMENT '供應商人員編號',
  PRIMARY KEY (`rowid`),
  KEY `FK_t_groupCaller2_t_fgroup2` (`groupId`),
  CONSTRAINT `FK_t_groupCaller2_t_fgroup2` FOREIGN KEY (`groupId`) REFERENCES `t_fgroup2` (`rowid`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for t_history_batch
-- ----------------------------
DROP TABLE IF EXISTS `t_history_batch`;
CREATE TABLE `t_history_batch` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `batchno` varchar(45) NOT NULL COMMENT '批次編號',
  `kuser` varchar(45) NOT NULL,
  `kdate` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=86694 DEFAULT CHARSET=latin1 ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for t_parametersrc
-- ----------------------------
DROP TABLE IF EXISTS `t_parametersrc`;
CREATE TABLE `t_parametersrc` (
  `rowid` int(11) NOT NULL AUTO_INCREMENT,
  `parameterType` varchar(300) DEFAULT NULL,
  `parameterProperty` varchar(100) DEFAULT NULL COMMENT '屬性',
  `parameterCode` varchar(300) DEFAULT NULL,
  `parameterName` varchar(300) DEFAULT NULL,
  `remark` varchar(300) DEFAULT NULL,
  `kdate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `kuser` varchar(50) DEFAULT NULL,
  `used` int(11) DEFAULT NULL COMMENT '是否使用(0.否 1.是)',
  `sort` int(11) DEFAULT NULL,
  `topValue` varchar(300) DEFAULT '' COMMENT '父級類型id',
  PRIMARY KEY (`rowid`),
  KEY `idx_01` (`parameterType`(255)),
  KEY `idx_02` (`parameterType`(255),`parameterProperty`,`parameterCode`(255))
) ENGINE=InnoDB AUTO_INCREMENT=6817 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for t_productinfo_temp
-- ----------------------------
DROP TABLE IF EXISTS `t_productinfo_temp`;
CREATE TABLE `t_productinfo_temp` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `batch_no` varchar(50) DEFAULT NULL,
  `product_id` int(11) DEFAULT NULL,
  `item_id` int(11) DEFAULT NULL,
  `site_id` int(11) DEFAULT NULL,
  `price_master_id` int(11) DEFAULT NULL,
  `product_name` varchar(255) DEFAULT NULL,
  `item_vendor_id_detail` int(11) DEFAULT NULL,
  `item_vendor_id_slave` int(11) DEFAULT NULL,
  `product_freight_set` int(11) DEFAULT NULL,
  `product_mode` int(11) DEFAULT NULL,
  `product_spec_name` varchar(50) DEFAULT NULL,
  `price` int(11) DEFAULT NULL,
  `combined_mode` int(11) DEFAULT NULL,
  `item_money` int(11) DEFAULT NULL,
  `item_cost` int(11) DEFAULT NULL,
  `event_money` int(11) DEFAULT NULL,
  `event_cost` int(11) DEFAULT NULL,
  `buy_num` int(11) DEFAULT NULL,
  `prepaid` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=225 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for t_stock_update
-- ----------------------------
DROP TABLE IF EXISTS `t_stock_update`;
CREATE TABLE `t_stock_update` (
  `t_stock_update_id2` int(11) NOT NULL AUTO_INCREMENT,
  `brand_id` varchar(12) NOT NULL,
  `brand_name` varchar(120) NOT NULL,
  `temp_id` varchar(12) NOT NULL,
  `item_id` varchar(12) NOT NULL,
  `product_name` varchar(255) NOT NULL,
  `spec_name_1` varchar(25) NOT NULL,
  `spec_name_2` varchar(12) NOT NULL,
  `item_code` varchar(255) NOT NULL,
  `stock` varchar(5) NOT NULL,
  PRIMARY KEY (`t_stock_update_id2`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for t_table_history
-- ----------------------------
DROP TABLE IF EXISTS `t_table_history`;
CREATE TABLE `t_table_history` (
  `rowid` int(11) NOT NULL AUTO_INCREMENT,
  `table_name` varchar(45) DEFAULT NULL COMMENT '表名',
  `functionId` int(11) DEFAULT NULL COMMENT '功能名稱',
  `PK_name` varchar(45) DEFAULT NULL,
  `PK_value` varchar(45) DEFAULT NULL,
  `batchno` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`rowid`)
) ENGINE=InnoDB AUTO_INCREMENT=201717 DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT COMMENT='資料表修改歷史記錄';

-- ----------------------------
-- Table structure for t_table_historyitem
-- ----------------------------
DROP TABLE IF EXISTS `t_table_historyitem`;
CREATE TABLE `t_table_historyitem` (
  `rowid` int(11) NOT NULL AUTO_INCREMENT,
  `tableHistoryId` int(11) DEFAULT NULL,
  `col_name` varchar(45) DEFAULT NULL COMMENT '欄位',
  `col_chsName` varchar(45) DEFAULT NULL COMMENT '欄位中文名稱',
  `col_value` varchar(1000) DEFAULT NULL COMMENT '修改前的值',
  `old_value` varchar(1000) DEFAULT NULL,
  `type` int(11) DEFAULT NULL COMMENT '記錄類型 0歷史 OR 1最新',
  PRIMARY KEY (`rowid`)
) ENGINE=InnoDB AUTO_INCREMENT=618131 DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT COMMENT='資料表維護歷史記錄';

-- ----------------------------
-- Table structure for t_zip_code
-- ----------------------------
DROP TABLE IF EXISTS `t_zip_code`;
CREATE TABLE `t_zip_code` (
  `RowID` int(11) NOT NULL AUTO_INCREMENT,
  `big` varchar(510) DEFAULT NULL,
  `bigcode` varchar(510) DEFAULT NULL,
  `middle` varchar(510) DEFAULT NULL,
  `middlecode` varchar(510) DEFAULT NULL,
  `zipcode` varchar(510) DEFAULT NULL,
  `small` varchar(510) DEFAULT NULL,
  PRIMARY KEY (`RowID`)
) ENGINE=InnoDB AUTO_INCREMENT=369 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for table_change_log
-- ----------------------------
DROP TABLE IF EXISTS `table_change_log`;
CREATE TABLE `table_change_log` (
  `row_id` int(9) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `user_type` int(9) DEFAULT NULL COMMENT '變更者類型1.供應商2.管理員',
  `pk_id` int(9) DEFAULT NULL COMMENT '變更細項主鍵',
  `change_table` varchar(50) DEFAULT NULL COMMENT '變更表名',
  `change_field` varchar(50) DEFAULT NULL COMMENT '變更欄位名',
  `old_value` varchar(30) DEFAULT NULL COMMENT '原有數據',
  `new_value` varchar(30) DEFAULT NULL COMMENT '變更后數據',
  `create_user` int(10) DEFAULT NULL COMMENT '變更人',
  `create_time` datetime DEFAULT NULL COMMENT '變更日期',
  `field_ch_name` varchar(255) DEFAULT NULL COMMENT '欄位中文名稱',
  PRIMARY KEY (`row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=854 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tblFaeIssueList
-- ----------------------------
DROP TABLE IF EXISTS `tblFaeIssueList`;
CREATE TABLE `tblFaeIssueList` (
  `tID` int(2) unsigned NOT NULL AUTO_INCREMENT,
  `issueName` char(50) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '' COMMENT 'Question Name',
  `issueStatus` int(3) unsigned NOT NULL DEFAULT '0' COMMENT 'Question Category',
  `notes` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Remarks',
  PRIMARY KEY (`tID`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for tblFaeRecList
-- ----------------------------
DROP TABLE IF EXISTS `tblFaeRecList`;
CREATE TABLE `tblFaeRecList` (
  `tickedID` int(9) unsigned NOT NULL,
  `issueType` char(20) COLLATE utf8mb4_unicode_ci DEFAULT '0' COMMENT 'Question Category',
  `ticketStatus` int(2) unsigned DEFAULT '0' COMMENT 'Question Status',
  `notes` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT ' ' COMMENT 'Remarks',
  PRIMARY KEY (`tickedID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for temp
-- ----------------------------
DROP TABLE IF EXISTS `temp`;
CREATE TABLE `temp` (
  `rid` int(11) NOT NULL DEFAULT '0',
  `COMPANY` varchar(10) DEFAULT NULL,
  `CREATOR` varchar(10) DEFAULT NULL,
  `MB001` varchar(20) NOT NULL DEFAULT '',
  `MB002` varchar(30) DEFAULT NULL,
  `MB003` varchar(10) NOT NULL DEFAULT '0',
  `MB004` varchar(10) NOT NULL DEFAULT '0',
  `MB032` varchar(20) NOT NULL DEFAULT '0',
  `MB080` varchar(20) DEFAULT NULL,
  `MB111` varchar(2) DEFAULT NULL,
  `note` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`MB001`),
  UNIQUE KEY `ix_mb001` (`MB001`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for temp_detail
-- ----------------------------
DROP TABLE IF EXISTS `temp_detail`;
CREATE TABLE `temp_detail` (
  `id` int(9) unsigned NOT NULL AUTO_INCREMENT COMMENT '流水編號',
  `order_id` int(9) unsigned NOT NULL DEFAULT '0' COMMENT '付款單號',
  `product_name` varchar(255) DEFAULT '' COMMENT '商品名稱',
  `item_id` int(9) unsigned NOT NULL DEFAULT '0' COMMENT '細項編號',
  `item_cost` mediumint(7) unsigned NOT NULL DEFAULT '0' COMMENT '商品成本',
  `item_money` mediumint(7) unsigned NOT NULL DEFAULT '0' COMMENT '商品價格',
  `buy_num` mediumint(7) unsigned NOT NULL DEFAULT '0' COMMENT '購買數量',
  `deliver` tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '出貨 0:未出 1:已出',
  `status` tinyint(1) unsigned NOT NULL DEFAULT '1' COMMENT '購買狀態 1購買 2取消',
  PRIMARY KEY (`id`),
  KEY `inx_order_id` (`order_id`),
  KEY `inx_item_id` (`order_id`,`item_id`)
) ENGINE=InnoDB AUTO_INCREMENT=1838 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for temp_exchange
-- ----------------------------
DROP TABLE IF EXISTS `temp_exchange`;
CREATE TABLE `temp_exchange` (
  `rid` int(9) unsigned NOT NULL AUTO_INCREMENT,
  `uid` int(9) unsigned NOT NULL,
  PRIMARY KEY (`rid`)
) ENGINE=InnoDB AUTO_INCREMENT=77 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for temp_master
-- ----------------------------
DROP TABLE IF EXISTS `temp_master`;
CREATE TABLE `temp_master` (
  `order_id` int(9) unsigned NOT NULL DEFAULT '0' COMMENT '付款單號',
  `invoice_number` varchar(10) NOT NULL DEFAULT '' COMMENT '發票號碼',
  `free_tax` int(9) unsigned NOT NULL DEFAULT '0' COMMENT '未稅銷售額',
  `tax_amount` int(9) unsigned NOT NULL DEFAULT '0' COMMENT '營業稅 (free_tax+tax_amount=付款總額)',
  `order_freight_normal` int(9) unsigned NOT NULL DEFAULT '0' COMMENT '常溫運費',
  `order_freight_low` int(9) unsigned NOT NULL DEFAULT '0' COMMENT '低溫運費',
  `order_name` varchar(64) NOT NULL COMMENT '付款人姓名',
  `order_gender` tinyint(1) NOT NULL DEFAULT '0' COMMENT '付款人性別',
  `order_mobile` varchar(50) NOT NULL,
  `order_zip` mediumint(7) NOT NULL DEFAULT '0' COMMENT '付款人郵遞區號',
  `order_address` varchar(255) NOT NULL DEFAULT '' COMMENT '付款人住址',
  `order_createdate` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '建立時間',
  `company_title` varchar(255) NOT NULL DEFAULT '' COMMENT '發票抬頭',
  `company_invoice` varchar(10) NOT NULL DEFAULT '' COMMENT '統編',
  `print_invoice` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `pay_type` tinyint(1) unsigned NOT NULL DEFAULT '1' COMMENT '付款方式 1:現金 2:刷卡',
  `creator` int(9) unsigned NOT NULL DEFAULT '0' COMMENT '建立者',
  `note` varchar(255) DEFAULT '',
  PRIMARY KEY (`order_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for temp2
-- ----------------------------
DROP TABLE IF EXISTS `temp2`;
CREATE TABLE `temp2` (
  `rid` int(11) NOT NULL AUTO_INCREMENT,
  `pid` int(10) NOT NULL,
  `item_id` int(9) DEFAULT NULL,
  `note` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `prepaid` int(10) DEFAULT NULL,
  PRIMARY KEY (`rid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for ticket
-- ----------------------------
DROP TABLE IF EXISTS `ticket`;
CREATE TABLE `ticket` (
  `ticket_id` int(11) NOT NULL AUTO_INCREMENT,
  `type` int(11) NOT NULL,
  `freight_set` int(11) NOT NULL,
  `export_id` int(11) NOT NULL,
  `import_id` int(11) NOT NULL,
  `delivery_store` int(11) NOT NULL DEFAULT '1',
  `warehouse_status` int(11) NOT NULL COMMENT '倉別',
  `payment` int(2) DEFAULT NULL COMMENT '金流別',
  `bank` varchar(100) DEFAULT NULL COMMENT '銀行別',
  `ticket_status` int(11) NOT NULL,
  `seized_status` int(11) NOT NULL COMMENT '檢貨',
  `ship_status` int(11) NOT NULL COMMENT '出貨',
  `Freight_status` int(11) NOT NULL COMMENT '貨運',
  `verifier` int(11) NOT NULL,
  `created` datetime NOT NULL,
  `modified` datetime NOT NULL,
  PRIMARY KEY (`ticket_id`)
) ENGINE=InnoDB AUTO_INCREMENT=25615 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for track_detail
-- ----------------------------
DROP TABLE IF EXISTS `track_detail`;
CREATE TABLE `track_detail` (
  `detail_id` int(11) NOT NULL AUTO_INCREMENT,
  `utm_id` varchar(4) NOT NULL,
  `host` varchar(50) NOT NULL,
  `port` varchar(10) NOT NULL,
  `uri` varchar(255) NOT NULL,
  `from_http` varchar(255) NOT NULL,
  `from_ip` varchar(15) NOT NULL,
  `utm_source` varchar(30) NOT NULL,
  `product_id` int(11) NOT NULL,
  `category_id` int(11) NOT NULL,
  `brand_id` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  `created_on` datetime NOT NULL,
  PRIMARY KEY (`detail_id`)
) ENGINE=InnoDB AUTO_INCREMENT=10638 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for travel
-- ----------------------------
DROP TABLE IF EXISTS `travel`;
CREATE TABLE `travel` (
  `travel_id` int(9) unsigned NOT NULL,
  `travel_status` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `travel_product_id` int(9) unsigned DEFAULT NULL,
  `travel_product_item_id` int(9) unsigned DEFAULT NULL,
  `travel_deduct_product_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `travel_title` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `travel_desc` text COLLATE utf8_unicode_ci,
  `travel_memberlimit` mediumint(4) unsigned NOT NULL DEFAULT '0' COMMENT '0 no limit',
  `travel_location` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `travel_geocoord` varchar(64) COLLATE utf8_unicode_ci DEFAULT NULL,
  `travel_raisedate_since` int(10) unsigned NOT NULL DEFAULT '0',
  `travel_raisedate_till` int(10) unsigned NOT NULL DEFAULT '0',
  `travel_startdate` int(10) unsigned NOT NULL DEFAULT '0',
  `travel_enddate` int(10) unsigned NOT NULL DEFAULT '0',
  `travel_initdate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`travel_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- ----------------------------
-- Table structure for travelmember
-- ----------------------------
DROP TABLE IF EXISTS `travelmember`;
CREATE TABLE `travelmember` (
  `travelmember_id` int(9) unsigned NOT NULL AUTO_INCREMENT,
  `travel_id` int(9) unsigned NOT NULL,
  `user_id` int(9) unsigned NOT NULL,
  `traveluser_id` int(9) unsigned NOT NULL,
  `travelmember_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `travelmember_mobile` varchar(20) COLLATE utf8_unicode_ci DEFAULT NULL,
  `travelmember_email` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`travelmember_id`),
  KEY `travel_id` (`travel_id`),
  KEY `traveluser_id` (`traveluser_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci ROW_FORMAT=COMPACT;

-- ----------------------------
-- Table structure for traveluser
-- ----------------------------
DROP TABLE IF EXISTS `traveluser`;
CREATE TABLE `traveluser` (
  `traveluser_id` int(9) unsigned NOT NULL AUTO_INCREMENT,
  `travel_id` int(9) unsigned NOT NULL,
  `user_id` int(9) unsigned NOT NULL,
  `traveluser_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `traveluser_mobile` varchar(50) COLLATE utf8_unicode_ci NOT NULL,
  `traveluser_email` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `traveluser_order_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL COMMENT 'comma seperate',
  `travel_pay_order_id` int(9) unsigned NOT NULL DEFAULT '0',
  `traveluser_totalitem` int(4) NOT NULL DEFAULT '0',
  `traveluser_totalppl` int(4) NOT NULL DEFAULT '1',
  `traveluser_totalcost` int(9) unsigned NOT NULL DEFAULT '0',
  `traveluser_applydate` int(10) unsigned NOT NULL DEFAULT '0',
  `traveluser_order_pay_date` int(10) unsigned DEFAULT NULL,
  `traveluser_confirmed` tinyint(1) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`traveluser_id`),
  KEY `travel_id` (`travel_id`),
  KEY `user_id` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- ----------------------------
-- Table structure for trial_picture
-- ----------------------------
DROP TABLE IF EXISTS `trial_picture`;
CREATE TABLE `trial_picture` (
  `share_id` int(9) NOT NULL COMMENT 'trial_share表的主鍵 用主鍵關聯圖片',
  `image_filename` varchar(40) NOT NULL DEFAULT '' COMMENT '圖片路徑',
  `image_sort` tinyint(2) unsigned NOT NULL DEFAULT '0' COMMENT '排序',
  `image_state` tinyint(2) unsigned NOT NULL DEFAULT '1' COMMENT '圖片狀態',
  `image_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`share_id`,`image_filename`),
  CONSTRAINT `fk_trial_picture_rid` FOREIGN KEY (`share_id`) REFERENCES `trial_share` (`share_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for trial_prod_cate
-- ----------------------------
DROP TABLE IF EXISTS `trial_prod_cate`;
CREATE TABLE `trial_prod_cate` (
  `id` int(9) NOT NULL AUTO_INCREMENT,
  `event_id` varchar(10) NOT NULL COMMENT '活動id',
  `type` tinyint(2) DEFAULT NULL COMMENT '試用試吃類型 1：試吃  2：試用',
  `category_id` int(9) unsigned NOT NULL COMMENT '類別id',
  `product_id` int(9) unsigned NOT NULL COMMENT '商品id',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for trial_record
-- ----------------------------
DROP TABLE IF EXISTS `trial_record`;
CREATE TABLE `trial_record` (
  `record_id` int(9) NOT NULL AUTO_INCREMENT,
  `event_id` int(9) NOT NULL COMMENT '活動id',
  `user_id` int(9) NOT NULL COMMENT '確定會員 唯一會員 ',
  `apply_time` datetime DEFAULT NULL COMMENT '報名時間',
  `status` int(9) DEFAULT '1' COMMENT '會員申請試用狀態1：報名中2：已錄取，試用品寄送中3：未錄取',
  PRIMARY KEY (`record_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for trial_share
-- ----------------------------
DROP TABLE IF EXISTS `trial_share`;
CREATE TABLE `trial_share` (
  `share_id` int(9) NOT NULL AUTO_INCREMENT,
  `event_id` int(9) DEFAULT NULL COMMENT '活動id',
  `user_id` int(9) DEFAULT NULL COMMENT '會員id',
  `is_show_name` tinyint(2) DEFAULT NULL COMMENT '是否顯示名稱 不顯示的時候 會員名稱顯示先生小姐0:匿名，1：公開''',
  `user_name` varchar(255) DEFAULT NULL COMMENT '會員姓名',
  `user_gender` tinyint(1) DEFAULT NULL COMMENT '性別 0：女  1：男',
  `content` text COMMENT '分享內容',
  `share_time` datetime DEFAULT NULL COMMENT '分享時間',
  `status` int(2) DEFAULT NULL COMMENT '狀態 0：新建立 1顯示 2：隱藏 3：下檔',
  PRIMARY KEY (`share_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for use_search_info
-- ----------------------------
DROP TABLE IF EXISTS `use_search_info`;
CREATE TABLE `use_search_info` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `search_area` int(1) NOT NULL COMMENT '搜尋區域:0:food 1:stuff',
  `search_category_id` int(10) NOT NULL COMMENT '搜尋類別id',
  `keyword` varchar(255) NOT NULL COMMENT '搜尋內容',
  `session_id` char(32) NOT NULL DEFAULT '' COMMENT 'cookie_id',
  `user_id` int(9) NOT NULL,
  `create_time` int(10) NOT NULL DEFAULT '0' COMMENT '建立時間',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=41937 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for user_behavior_log
-- ----------------------------
DROP TABLE IF EXISTS `user_behavior_log`;
CREATE TABLE `user_behavior_log` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(11) NOT NULL,
  `user_name` varchar(50) CHARACTER SET utf8 NOT NULL,
  `domain` varchar(255) CHARACTER SET utf8 NOT NULL COMMENT '網域',
  `domain_site` varchar(255) CHARACTER SET utf8 NOT NULL COMMENT '網址',
  `action_properties` varchar(255) CHARACTER SET utf8 NOT NULL COMMENT '動作屬性',
  `code` text CHARACTER SET utf8 NOT NULL COMMENT '值',
  `memo` varchar(255) CHARACTER SET utf8 NOT NULL COMMENT '備註',
  `set_time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=22512 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for user_browse_product
-- ----------------------------
DROP TABLE IF EXISTS `user_browse_product`;
CREATE TABLE `user_browse_product` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `user_id` int(10) NOT NULL,
  `product_id` int(10) NOT NULL,
  `start_time` int(10) NOT NULL DEFAULT '0' COMMENT '記錄時間',
  `session_id` char(32) NOT NULL DEFAULT '' COMMENT 'cookie_id',
  `type` int(2) unsigned NOT NULL DEFAULT '1' COMMENT '1:瀏覽2:刪除',
  `site_id` int(2) NOT NULL DEFAULT '1',
  `browse_source` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`),
  KEY `unix_key` (`user_id`,`product_id`,`session_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3440750 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for user_collection_product
-- ----------------------------
DROP TABLE IF EXISTS `user_collection_product`;
CREATE TABLE `user_collection_product` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `user_id` int(10) NOT NULL,
  `product_id` int(10) NOT NULL,
  `start_time` int(10) NOT NULL DEFAULT '0' COMMENT '記錄時間',
  `session_id` char(32) NOT NULL DEFAULT '' COMMENT 'cookie_id',
  `type` int(2) unsigned NOT NULL DEFAULT '1' COMMENT '1:瀏覽2:刪除',
  `site_id` int(2) NOT NULL,
  `collection_source` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1456 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for user_condition
-- ----------------------------
DROP TABLE IF EXISTS `user_condition`;
CREATE TABLE `user_condition` (
  `condition_id` int(11) NOT NULL AUTO_INCREMENT,
  `condition_name` varchar(100) CHARACTER SET utf8 NOT NULL,
  `reg_start` int(10) DEFAULT NULL COMMENT '會員註冊時間起',
  `reg_end` int(10) DEFAULT NULL COMMENT '會員註冊時間迄',
  `reg_interval` smallint(4) DEFAULT NULL COMMENT '會員註冊時間距今月數',
  `buy_times_min` int(10) DEFAULT NULL COMMENT '最少消費次數',
  `buy_times_max` int(10) DEFAULT NULL COMMENT '最高消費次數',
  `buy_amount_min` int(10) DEFAULT NULL COMMENT '最低消費總金額',
  `buy_amount_max` int(10) DEFAULT NULL COMMENT '最高消費總金額',
  `last_time_start` int(10) DEFAULT NULL COMMENT '上次購買時間起',
  `last_time_end` int(10) DEFAULT NULL COMMENT '上次購買時間迄',
  `last_time_interval` smallint(4) DEFAULT NULL COMMENT '上次購買時間距今月數',
  `join_channel` tinyint(2) DEFAULT NULL COMMENT '會員加入來源',
  `status` tinyint(2) NOT NULL DEFAULT '0' COMMENT '狀態 0:無效  1:有效',
  PRIMARY KEY (`condition_id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ----------------------------
-- Table structure for user_device
-- ----------------------------
DROP TABLE IF EXISTS `user_device`;
CREATE TABLE `user_device` (
  `id` int(9) NOT NULL AUTO_INCREMENT,
  `user_id` int(9) NOT NULL,
  `device_id` varchar(1000) NOT NULL,
  `kdate` datetime NOT NULL,
  `mdate` datetime NOT NULL,
  `platform` varchar(100) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1067 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for user_forbid
-- ----------------------------
DROP TABLE IF EXISTS `user_forbid`;
CREATE TABLE `user_forbid` (
  `forbid_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `forbid_ip` varchar(50) DEFAULT NULL COMMENT '禁用IP',
  `forbid_createdate` int(10) DEFAULT NULL COMMENT '創建時間',
  `forbid_createuser` int(4) DEFAULT NULL COMMENT '創建人',
  PRIMARY KEY (`forbid_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for user_history
-- ----------------------------
DROP TABLE IF EXISTS `user_history`;
CREATE TABLE `user_history` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `user_id` int(9) unsigned NOT NULL DEFAULT '0',
  `user_name` varchar(255) NOT NULL DEFAULT '',
  `file_name` varchar(255) NOT NULL DEFAULT '',
  `content` mediumtext,
  `creat_time` int(9) unsigned NOT NULL DEFAULT '0',
  `ip` varchar(255) NOT NULL DEFAULT '''''',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2540950 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for user_level_log
-- ----------------------------
DROP TABLE IF EXISTS `user_level_log`;
CREATE TABLE `user_level_log` (
  `rowID` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水號',
  `user_id` int(9) unsigned NOT NULL COMMENT '會員ID',
  `user_order_amount` int(11) DEFAULT NULL COMMENT '會員購買金額 前12個月',
  `ml_code_old` varchar(255) DEFAULT '' COMMENT '舊會員等級',
  `ml_code_new` varchar(50) NOT NULL COMMENT '新會員等級 對應member_level表中ml_code;',
  `ml_code_change_type` varchar(255) NOT NULL DEFAULT 'up' COMMENT '等級變化類型 up:升\r\n級 down:降級',
  `create_date_time` datetime NOT NULL COMMENT '日期',
  `year` int(11) DEFAULT NULL COMMENT '年',
  `month` int(11) DEFAULT NULL COMMENT '月',
  PRIMARY KEY (`rowID`),
  KEY `user_id` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=41541 DEFAULT CHARSET=utf8 COMMENT='會員等級歷程';

-- ----------------------------
-- Table structure for user_life
-- ----------------------------
DROP TABLE IF EXISTS `user_life`;
CREATE TABLE `user_life` (
  `row_id` int(9) NOT NULL AUTO_INCREMENT COMMENT '序號',
  `user_id` int(12) unsigned NOT NULL,
  `info_type` varchar(50) NOT NULL COMMENT '屬性英文名',
  `info_code` varchar(50) DEFAULT NULL COMMENT '屬性值',
  `info_name` varchar(50) DEFAULT NULL COMMENT '屬性中文名稱',
  `remark` varchar(100) DEFAULT NULL COMMENT '備註',
  `kdate` int(9) unsigned DEFAULT NULL,
  `kuser` int(9) DEFAULT NULL,
  PRIMARY KEY (`row_id`),
  UNIQUE KEY `un_key_user_id_type` (`user_id`,`info_type`),
  KEY `in_key` (`user_id`,`info_type`),
  CONSTRAINT `fk_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=380 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for user_login_attempts
-- ----------------------------
DROP TABLE IF EXISTS `user_login_attempts`;
CREATE TABLE `user_login_attempts` (
  `login_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '登陸編號',
  `login_mail` varchar(50) DEFAULT NULL COMMENT '登陸郵件',
  `login_ipfrom` varchar(40) DEFAULT NULL COMMENT '登陸IP',
  `login_type` tinyint(2) DEFAULT NULL COMMENT '錯誤登入類型 1：會員登入錯誤 2：php後台登入錯誤3：net後台登入錯誤 4：查詢機敏資料登入錯誤',
  `login_createdate` int(10) DEFAULT NULL COMMENT '登陸時間',
  PRIMARY KEY (`login_id`)
) ENGINE=InnoDB AUTO_INCREMENT=11772 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for user_orders_subtotal
-- ----------------------------
DROP TABLE IF EXISTS `user_orders_subtotal`;
CREATE TABLE `user_orders_subtotal` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(9) NOT NULL COMMENT '會員編號',
  `order_product_subtotal` int(9) DEFAULT NULL COMMENT '當月累計消費金額',
  `year` int(9) DEFAULT '0' COMMENT '年',
  `month` int(11) DEFAULT '0' COMMENT '月',
  `year_month` int(11) DEFAULT NULL COMMENT '年月',
  `buy_count` int(11) DEFAULT NULL COMMENT '當月購買次數',
  `last_buy_time` datetime DEFAULT NULL COMMENT '最後購買時間',
  `buy_avg` int(11) DEFAULT '0' COMMENT ' 當月平均購買價',
  `normal_product_subtotal` int(11) DEFAULT '0' COMMENT '常溫商品總額',
  `low_product_subtotal` int(11) DEFAULT '0' COMMENT '低溫商品總額',
  `create_datetime` datetime DEFAULT NULL COMMENT '創建時間',
  `create_user` int(11) DEFAULT '0' COMMENT '創建人',
  `note` varchar(255) DEFAULT NULL COMMENT '備註',
  PRIMARY KEY (`row_id`),
  KEY `year` (`year`),
  KEY `month` (`month`),
  KEY `year_month` (`year_month`)
) ENGINE=InnoDB AUTO_INCREMENT=275452 DEFAULT CHARSET=utf8 COMMENT='會員累積金額';

-- ----------------------------
-- Table structure for user_orders_subtotal_temp
-- ----------------------------
DROP TABLE IF EXISTS `user_orders_subtotal_temp`;
CREATE TABLE `user_orders_subtotal_temp` (
  `row_id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(9) NOT NULL COMMENT '會員編號',
  `order_product_subtotal` int(9) DEFAULT NULL COMMENT '當月累計消費金額',
  `year` int(9) DEFAULT '0' COMMENT '年',
  `month` int(11) DEFAULT '0' COMMENT '月',
  `year_month` int(11) DEFAULT NULL COMMENT '年月',
  `buy_count` int(11) DEFAULT NULL COMMENT '當月購買次數',
  `last_buy_time` datetime DEFAULT NULL COMMENT '最後購買時間',
  `buy_avg` int(11) DEFAULT '0' COMMENT ' 當月平均購買價',
  `normal_product_subtotal` int(11) DEFAULT '0' COMMENT '常溫商品總額',
  `low_product_subtotal` int(11) DEFAULT '0' COMMENT '低溫商品總額',
  `create_datetime` datetime DEFAULT NULL COMMENT '創建時間',
  `create_user` int(11) DEFAULT '0' COMMENT '創建人',
  `note` varchar(255) DEFAULT NULL COMMENT '備註',
  PRIMARY KEY (`row_id`),
  KEY `year` (`year`),
  KEY `month` (`month`),
  KEY `year_month` (`year_month`)
) ENGINE=InnoDB AUTO_INCREMENT=25225 DEFAULT CHARSET=utf8 COMMENT='會員累積金額臨時表';

-- ----------------------------
-- Table structure for user_recommend
-- ----------------------------
DROP TABLE IF EXISTS `user_recommend`;
CREATE TABLE `user_recommend` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `event_id` int(2) NOT NULL DEFAULT '0' COMMENT '活動分類id',
  `user_id` int(10) DEFAULT '0',
  `name` varchar(15) NOT NULL,
  `mail` varchar(50) NOT NULL,
  `user_ip` varchar(25) DEFAULT NULL COMMENT '被推薦人ip',
  `recommend_user_id` int(10) NOT NULL COMMENT '推薦人id',
  `recommend_user_ip` varchar(25) DEFAULT NULL COMMENT '推薦人ip',
  `recommend_commodity_id` int(11) NOT NULL COMMENT '推薦商品id',
  `is_recommend` tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '是否到過推薦商品頁',
  `order_id` int(11) DEFAULT NULL,
  `memo` varchar(255) DEFAULT NULL,
  `createtime` datetime NOT NULL COMMENT '建立時間',
  `updatetime` datetime NOT NULL COMMENT '修改時間',
  PRIMARY KEY (`id`),
  KEY `inx_mail` (`mail`)
) ENGINE=InnoDB AUTO_INCREMENT=498 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for user_search_product
-- ----------------------------
DROP TABLE IF EXISTS `user_search_product`;
CREATE TABLE `user_search_product` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `user_id` int(10) NOT NULL,
  `product_id` int(10) NOT NULL,
  `start_time` int(10) NOT NULL DEFAULT '0' COMMENT '記錄時間',
  `session_id` char(32) NOT NULL DEFAULT '' COMMENT 'cookie_id',
  `type` int(2) unsigned NOT NULL DEFAULT '1' COMMENT '1:瀏覽2:刪除',
  `site_id` int(2) NOT NULL DEFAULT '1',
  `browse_source` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=15121 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for user_token
-- ----------------------------
DROP TABLE IF EXISTS `user_token`;
CREATE TABLE `user_token` (
  `user_email` varchar(100) NOT NULL,
  `access_token` varchar(128) DEFAULT NULL,
  `expired` int(10) unsigned DEFAULT NULL,
  `modDate` int(10) unsigned DEFAULT NULL,
  PRIMARY KEY (`user_email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for users
-- ----------------------------
DROP TABLE IF EXISTS `users`;
CREATE TABLE `users` (
  `user_id` int(9) unsigned NOT NULL,
  `user_email` varchar(100) NOT NULL DEFAULT '',
  `user_new_email` varchar(100) NOT NULL DEFAULT '',
  `user_status` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `user_password` varchar(64) NOT NULL DEFAULT '',
  `user_newpasswd` varchar(64) NOT NULL DEFAULT '',
  `user_name` varchar(255) NOT NULL DEFAULT '',
  `user_nickname` varchar(20) DEFAULT NULL COMMENT '會員昵稱',
  `user_photo` varchar(20) DEFAULT NULL COMMENT '用戶頭像',
  `user_gender` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `user_birthday_year` smallint(4) unsigned NOT NULL DEFAULT '0',
  `user_birthday_month` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `user_birthday_day` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `user_mobile` varchar(50) NOT NULL DEFAULT '',
  `user_mobile_bak` varchar(50) NOT NULL DEFAULT '',
  `user_phone` varchar(20) NOT NULL DEFAULT '',
  `user_zip` mediumint(5) unsigned NOT NULL DEFAULT '0',
  `user_address` varchar(255) NOT NULL DEFAULT '',
  `user_login_attempts` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `user_actkey` varchar(32) NOT NULL DEFAULT '',
  `user_reg_date` int(10) unsigned NOT NULL DEFAULT '0',
  `user_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `user_old_password` varchar(40) NOT NULL DEFAULT '',
  `user_company_id` varchar(255) NOT NULL DEFAULT '',
  `user_source` varchar(50) NOT NULL DEFAULT '',
  `user_fb_id` varchar(20) NOT NULL DEFAULT '',
  `user_country` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `user_ref_user_id` int(9) unsigned DEFAULT NULL,
  `user_province` varchar(32) DEFAULT NULL,
  `user_city` varchar(32) DEFAULT NULL,
  `source_trace` int(7) unsigned NOT NULL DEFAULT '0',
  `user_type` tinyint(2) NOT NULL DEFAULT '1' COMMENT '會員類型  1.網路會員  2.電話會員 3.賣場(通路)代表會員',
  `send_sms_ad` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否接收簡訊廣告  0:不收  1:收',
  `adm_note` text COMMENT '管理員備註',
  `user_level` smallint(2) unsigned NOT NULL DEFAULT '1' COMMENT '會員等級',
  `buy_times` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '購買次數',
  `buy_amount` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '購買總金額',
  `first_time` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '第一次訂單付款時間',
  `last_time` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '上一次訂單付款時間',
  `be4_last_time` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '上上一次訂單付款時間',
  `paper_invoice` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否要長期發票',
  `ml_code` varchar(50) NOT NULL DEFAULT 'N' COMMENT '對應member_level表中ml_code;會員編碼',
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `uk_users_el` (`user_email`),
  KEY `ix_users_birthday_year` (`user_birthday_year`),
  KEY `ix_users_birthday_month` (`user_birthday_month`),
  KEY `ix_users_birthday_day` (`user_birthday_day`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for users_copy
-- ----------------------------
DROP TABLE IF EXISTS `users_copy`;
CREATE TABLE `users_copy` (
  `user_id` int(9) unsigned NOT NULL,
  `user_email` varchar(100) NOT NULL DEFAULT '',
  `user_new_email` varchar(100) NOT NULL DEFAULT '',
  `user_status` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `user_password` varchar(64) NOT NULL DEFAULT '',
  `user_newpasswd` varchar(64) NOT NULL DEFAULT '',
  `user_name` varchar(255) NOT NULL DEFAULT '',
  `user_nickname` varchar(20) DEFAULT NULL COMMENT '會員昵稱',
  `user_photo` varchar(20) DEFAULT NULL COMMENT '用戶頭像',
  `user_gender` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `user_birthday_year` smallint(4) unsigned NOT NULL DEFAULT '0',
  `user_birthday_month` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `user_birthday_day` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `user_mobile` varchar(20) NOT NULL DEFAULT '',
  `user_phone` varchar(20) NOT NULL DEFAULT '',
  `user_zip` mediumint(5) unsigned NOT NULL DEFAULT '0',
  `user_address` varchar(255) NOT NULL DEFAULT '',
  `user_login_attempts` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `user_actkey` varchar(32) NOT NULL DEFAULT '',
  `user_reg_date` int(10) unsigned NOT NULL DEFAULT '0',
  `user_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `user_old_password` varchar(40) NOT NULL DEFAULT '',
  `user_company_id` varchar(255) NOT NULL DEFAULT '',
  `user_source` varchar(50) NOT NULL DEFAULT '',
  `user_fb_id` varchar(20) NOT NULL DEFAULT '',
  `user_country` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `user_ref_user_id` int(9) unsigned DEFAULT NULL,
  `user_province` varchar(32) DEFAULT NULL,
  `user_city` varchar(32) DEFAULT NULL,
  `source_trace` int(7) unsigned NOT NULL DEFAULT '0',
  `user_type` tinyint(2) NOT NULL DEFAULT '1' COMMENT '會員類型  1.網路會員  2.電話會員 3.賣場(通路)代表會員',
  `send_sms_ad` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否接收簡訊廣告  0:不收  1:收',
  `adm_note` text COMMENT '管理員備註',
  `user_level` smallint(2) unsigned NOT NULL DEFAULT '1' COMMENT '會員等級',
  `buy_times` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '購買次數',
  `buy_amount` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '購買總金額',
  `first_time` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '第一次訂單付款時間',
  `last_time` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '上一次訂單付款時間',
  `be4_last_time` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '上上一次訂單付款時間',
  `paper_invoice` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否要長期發票',
  `ml_code` varchar(50) DEFAULT 'N' COMMENT '對應member_level表中ml_code;會員編碼',
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `uk_users_el` (`user_email`),
  KEY `ix_users_birthday_year` (`user_birthday_year`),
  KEY `ix_users_birthday_month` (`user_birthday_month`),
  KEY `ix_users_birthday_day` (`user_birthday_day`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for users_deduct_bonus
-- ----------------------------
DROP TABLE IF EXISTS `users_deduct_bonus`;
CREATE TABLE `users_deduct_bonus` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `deduct_bonus` int(10) unsigned NOT NULL DEFAULT '0',
  `user_id` int(10) unsigned NOT NULL DEFAULT '0',
  `order_id` int(10) unsigned NOT NULL DEFAULT '0',
  `status` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `createdate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=180 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for users_edm
-- ----------------------------
DROP TABLE IF EXISTS `users_edm`;
CREATE TABLE `users_edm` (
  `user_id` int(9) unsigned NOT NULL,
  `edm_id` tinyint(2) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`user_id`,`edm_id`),
  KEY `ix_users_edm_eid` (`edm_id`),
  CONSTRAINT `fk_users_edm_uid` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for users_login
-- ----------------------------
DROP TABLE IF EXISTS `users_login`;
CREATE TABLE `users_login` (
  `login_id` int(10) unsigned NOT NULL,
  `user_id` int(9) unsigned NOT NULL,
  `login_ipfrom` varchar(40) NOT NULL DEFAULT '',
  `login_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `login_address` varchar(50) NOT NULL DEFAULT '' COMMENT '登錄地址',
  PRIMARY KEY (`login_id`),
  KEY `ix_users_login_uid` (`user_id`),
  CONSTRAINT `fk_users_login_uid` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for vendor
-- ----------------------------
DROP TABLE IF EXISTS `vendor`;
CREATE TABLE `vendor` (
  `vendor_id` int(9) unsigned NOT NULL,
  `vendor_code` varchar(30) NOT NULL DEFAULT '',
  `vendor_status` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `vendor_email` varchar(100) NOT NULL DEFAULT '',
  `vendor_password` varchar(64) NOT NULL DEFAULT '',
  `vendor_name_full` varchar(100) NOT NULL DEFAULT '',
  `vendor_name_simple` varchar(100) NOT NULL DEFAULT '',
  `vendor_invoice` varchar(15) NOT NULL DEFAULT '',
  `company_phone` varchar(20) NOT NULL DEFAULT '',
  `company_fax` varchar(20) NOT NULL DEFAULT '',
  `company_person` varchar(15) NOT NULL DEFAULT '',
  `company_zip` mediumint(5) unsigned NOT NULL DEFAULT '0',
  `company_address` varchar(100) NOT NULL DEFAULT '',
  `invoice_zip` mediumint(5) unsigned NOT NULL DEFAULT '0',
  `invoice_address` varchar(100) NOT NULL DEFAULT '',
  `contact_type_1` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `contact_name_1` varchar(20) NOT NULL DEFAULT '',
  `contact_phone_1_1` varchar(20) NOT NULL DEFAULT '',
  `contact_phone_2_1` varchar(20) NOT NULL DEFAULT '',
  `contact_mobile_1` varchar(20) NOT NULL DEFAULT '',
  `contact_email_1` varchar(100) NOT NULL DEFAULT '',
  `contact_type_2` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `contact_name_2` varchar(20) NOT NULL DEFAULT '',
  `contact_phone_1_2` varchar(20) NOT NULL DEFAULT '',
  `contact_phone_2_2` varchar(20) NOT NULL DEFAULT '',
  `contact_mobile_2` varchar(20) NOT NULL DEFAULT '',
  `contact_email_2` varchar(100) NOT NULL DEFAULT '',
  `contact_type_3` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `contact_name_3` varchar(20) NOT NULL DEFAULT '',
  `contact_phone_1_3` varchar(20) NOT NULL DEFAULT '',
  `contact_phone_2_3` varchar(20) NOT NULL DEFAULT '',
  `contact_mobile_3` varchar(20) NOT NULL DEFAULT '',
  `contact_email_3` varchar(100) NOT NULL DEFAULT '',
  `contact_type_4` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `contact_name_4` varchar(20) NOT NULL DEFAULT '',
  `contact_phone_1_4` varchar(20) NOT NULL DEFAULT '',
  `contact_phone_2_4` varchar(20) NOT NULL DEFAULT '',
  `contact_mobile_4` varchar(20) NOT NULL DEFAULT '',
  `contact_email_4` varchar(100) NOT NULL DEFAULT '',
  `contact_type_5` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `contact_name_5` varchar(20) NOT NULL DEFAULT '',
  `contact_phone_1_5` varchar(20) NOT NULL DEFAULT '',
  `contact_phone_2_5` varchar(20) NOT NULL DEFAULT '',
  `contact_mobile_5` varchar(20) NOT NULL DEFAULT '',
  `contact_email_5` varchar(100) NOT NULL DEFAULT '',
  `cost_percent` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `creditcard_1_percent` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `creditcard_3_percent` varchar(3) NOT NULL DEFAULT '0',
  `sales_limit` int(9) unsigned NOT NULL DEFAULT '0',
  `bonus_percent` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `agreement_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `agreement_start` int(10) unsigned NOT NULL DEFAULT '0',
  `agreement_end` int(10) unsigned NOT NULL DEFAULT '0',
  `checkout_type` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `checkout_other` varchar(255) NOT NULL DEFAULT '',
  `bank_code` varchar(10) NOT NULL DEFAULT '',
  `bank_name` varchar(50) NOT NULL DEFAULT '',
  `bank_number` varchar(50) NOT NULL DEFAULT '',
  `bank_account` varchar(50) NOT NULL DEFAULT '',
  `freight_low_limit` int(9) unsigned NOT NULL DEFAULT '0',
  `freight_low_money` int(9) unsigned NOT NULL DEFAULT '0',
  `freight_normal_limit` int(9) unsigned NOT NULL DEFAULT '0',
  `freight_normal_money` int(9) unsigned NOT NULL DEFAULT '0',
  `freight_return_low_money` int(9) unsigned NOT NULL DEFAULT '0',
  `freight_return_normal_money` int(9) unsigned NOT NULL DEFAULT '0',
  `vendor_note` text NOT NULL,
  `vendor_confirm_code` varchar(64) NOT NULL DEFAULT '',
  `vendor_login_attempts` int(4) unsigned NOT NULL DEFAULT '0',
  `assist` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `dispatch` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `product_mode` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `product_manage` int(4) unsigned NOT NULL DEFAULT '0',
  `gigade_bunus_percent` int(2) unsigned NOT NULL DEFAULT '0' COMMENT '業績獎勵百分比',
  `gigade_bunus_threshold` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '業績獎勵門檻',
  `erp_id` varchar(30) DEFAULT NULL,
  `export_flag` smallint(5) NOT NULL DEFAULT '0',
  `data_chg` tinyint(2) NOT NULL DEFAULT '0',
  `procurement_days` int(6) DEFAULT '0' COMMENT '採購天數',
  `self_send_days` int(6) DEFAULT '0' COMMENT '自出天數',
  `stuff_ware_days` int(6) DEFAULT '0' COMMENT '寄倉天數',
  `dispatch_days` int(6) DEFAULT '0' COMMENT '調度天數',
  `vendor_updatedate` int(10) NOT NULL DEFAULT '0',
  `vendor_type` varchar(50) DEFAULT NULL COMMENT '供應商類型',
  `kuser` int(10) DEFAULT '0' COMMENT '建立人',
  `kdate` datetime DEFAULT NULL COMMENT '建立日期',
  PRIMARY KEY (`vendor_id`),
  UNIQUE KEY `uk_vendor_el` (`vendor_email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for vendor_account_detail
-- ----------------------------
DROP TABLE IF EXISTS `vendor_account_detail`;
CREATE TABLE `vendor_account_detail` (
  `slave_id` int(9) unsigned NOT NULL,
  `vendor_id` int(9) unsigned NOT NULL,
  `order_id` int(9) unsigned NOT NULL,
  `creditcard_1_percent` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `creditcard_3_percent` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `sales_limit` int(9) unsigned NOT NULL DEFAULT '0',
  `bonus_percent` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `freight_low_limit` int(9) unsigned NOT NULL DEFAULT '0',
  `freight_low_money` int(9) unsigned NOT NULL DEFAULT '0',
  `freight_normal_limit` int(9) unsigned NOT NULL DEFAULT '0',
  `freight_normal_money` int(9) unsigned NOT NULL DEFAULT '0',
  `freight_return_low_money` int(9) unsigned NOT NULL DEFAULT '0',
  `freight_return_normal_money` int(9) unsigned NOT NULL DEFAULT '0',
  `product_money` int(9) unsigned NOT NULL DEFAULT '0',
  `product_cost` int(9) unsigned NOT NULL DEFAULT '0',
  `money_creditcard_1` int(9) unsigned NOT NULL DEFAULT '0',
  `money_creditcard_3` int(9) unsigned NOT NULL DEFAULT '0',
  `freight_delivery_low` int(9) unsigned NOT NULL DEFAULT '0',
  `freight_delivery_normal` int(9) unsigned NOT NULL DEFAULT '0',
  `freight_return_low` int(9) unsigned NOT NULL DEFAULT '0',
  `create_time` int(10) NOT NULL DEFAULT '0' COMMENT '建立時間',
  `freight_return_normal` int(9) unsigned NOT NULL DEFAULT '0',
  `account_amount` int(9) NOT NULL DEFAULT '0',
  `account_date` int(10) unsigned NOT NULL DEFAULT '0',
  `gift` int(9) unsigned NOT NULL DEFAULT '0',
  `deduction` int(9) unsigned NOT NULL DEFAULT '0',
  `bag_check_money` int(9) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`slave_id`,`vendor_id`),
  KEY `ix_vendor_account_detail_oid` (`order_id`),
  KEY `ix_vendor_account_detail_ade` (`account_date`),
  KEY `fk_vendor_account_detail_vid` (`vendor_id`),
  CONSTRAINT `fk_vendor_account_detail_oid` FOREIGN KEY (`order_id`) REFERENCES `order_master` (`order_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_vendor_account_detail_sid` FOREIGN KEY (`slave_id`) REFERENCES `order_slave` (`slave_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_vendor_account_detail_vid` FOREIGN KEY (`vendor_id`) REFERENCES `vendor` (`vendor_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for vendor_account_month
-- ----------------------------
DROP TABLE IF EXISTS `vendor_account_month`;
CREATE TABLE `vendor_account_month` (
  `vendor_id` int(9) unsigned NOT NULL,
  `account_year` smallint(4) unsigned NOT NULL DEFAULT '0',
  `account_month` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `m_product_money` int(9) unsigned NOT NULL DEFAULT '0',
  `m_product_cost` int(9) unsigned NOT NULL DEFAULT '0',
  `m_money_creditcard_1` int(9) unsigned NOT NULL DEFAULT '0',
  `m_money_creditcard_3` int(9) unsigned NOT NULL DEFAULT '0',
  `m_freight_delivery_low` int(9) unsigned NOT NULL DEFAULT '0',
  `m_freight_delivery_normal` int(9) unsigned NOT NULL DEFAULT '0',
  `m_dispatch_freight_delivery_normal` int(9) unsigned NOT NULL DEFAULT '0' COMMENT '調度倉運費(常溫)',
  `m_dispatch_freight_delivery_low` int(9) unsigned NOT NULL DEFAULT '0' COMMENT '調度倉運費(低溫)',
  `m_freight_return_low` int(9) unsigned NOT NULL DEFAULT '0',
  `m_freight_return_normal` int(9) unsigned NOT NULL DEFAULT '0',
  `m_account_amount` int(9) NOT NULL DEFAULT '0',
  `m_all_deduction` int(9) unsigned NOT NULL DEFAULT '0',
  `m_gift` int(9) unsigned NOT NULL DEFAULT '0',
  `dispatch` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `m_bag_check_money` int(9) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`vendor_id`,`account_year`,`account_month`),
  CONSTRAINT `fk_vendor_account_month_vid` FOREIGN KEY (`vendor_id`) REFERENCES `vendor` (`vendor_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for vendor_brand
-- ----------------------------
DROP TABLE IF EXISTS `vendor_brand`;
CREATE TABLE `vendor_brand` (
  `brand_id` int(9) unsigned NOT NULL,
  `vendor_id` int(9) unsigned NOT NULL,
  `brand_name` varchar(255) NOT NULL DEFAULT '',
  `brand_name_simple` varchar(20) NOT NULL DEFAULT '',
  `brand_sort` smallint(4) unsigned NOT NULL DEFAULT '0',
  `brand_status` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `image_name` varchar(40) NOT NULL DEFAULT '',
  `image_status` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `image_link_mode` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `image_link_url` varchar(255) NOT NULL DEFAULT '',
  `media_report_link_url` text NOT NULL,
  `brand_msg` varchar(20) NOT NULL DEFAULT '',
  `brand_msg_start_time` int(10) unsigned NOT NULL DEFAULT '0',
  `brand_msg_end_time` int(10) unsigned NOT NULL DEFAULT '0',
  `brand_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `brand_updatedate` int(10) unsigned NOT NULL DEFAULT '0',
  `brand_ipfrom` varchar(40) NOT NULL DEFAULT '',
  `cucumber_brand` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `event` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `promotion_banner_image` varchar(40) NOT NULL DEFAULT '',
  `resume_image` varchar(40) NOT NULL DEFAULT '',
  `promotion_banner_image_link` varchar(255) NOT NULL DEFAULT '',
  `resume_image_link` varchar(255) NOT NULL DEFAULT '',
  `brand_story_text` text COMMENT '品牌故事文字',
  `story_created` int(9) DEFAULT NULL COMMENT '品牌故事文字建立人',
  `story_createdate` datetime DEFAULT NULL COMMENT '品牌故事文字創建時間',
  `story_update` int(9) DEFAULT NULL COMMENT '品牌故事文字變更者',
  `story_updatedate` datetime DEFAULT NULL COMMENT '品牌故事文字更新時間',
  `short_description` varchar(300) DEFAULT '' COMMENT '品牌短文字說明（字數限制300字）',
  `brand_logo` varchar(40) DEFAULT '' COMMENT '品牌logo',
  PRIMARY KEY (`brand_id`),
  KEY `ix_vendor_brand_vid` (`vendor_id`),
  KEY `ix_vendor_brand_st` (`brand_sort`),
  KEY `ix_vendor_brand_ss` (`brand_status`),
  KEY `ix_vendor_brand_ne` (`image_name`),
  CONSTRAINT `fk_vendor_brand_vid` FOREIGN KEY (`vendor_id`) REFERENCES `vendor` (`vendor_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for vendor_brand_set
-- ----------------------------
DROP TABLE IF EXISTS `vendor_brand_set`;
CREATE TABLE `vendor_brand_set` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `brand_id` int(9) unsigned NOT NULL,
  `class_id` int(9) unsigned NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `brand_id` (`brand_id`,`class_id`),
  KEY `fk_vendor_brand_set_cid` (`class_id`),
  CONSTRAINT `fk_vendor_brand_set_bid` FOREIGN KEY (`brand_id`) REFERENCES `vendor_brand` (`brand_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_vendor_brand_set_cid` FOREIGN KEY (`class_id`) REFERENCES `shop_class` (`class_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=4771 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for vendor_brand_story
-- ----------------------------
DROP TABLE IF EXISTS `vendor_brand_story`;
CREATE TABLE `vendor_brand_story` (
  `brand_id` int(9) unsigned NOT NULL,
  `image_filename` varchar(40) NOT NULL DEFAULT '',
  `image_sort` tinyint(2) unsigned NOT NULL DEFAULT '0',
  `image_state` tinyint(2) unsigned NOT NULL DEFAULT '1',
  `image_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`brand_id`,`image_filename`),
  CONSTRAINT `fk_vendor_brand_story_bid` FOREIGN KEY (`brand_id`) REFERENCES `vendor_brand` (`brand_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for vendor_cate_set
-- ----------------------------
DROP TABLE IF EXISTS `vendor_cate_set`;
CREATE TABLE `vendor_cate_set` (
  `rid` int(11) NOT NULL AUTO_INCREMENT,
  `vendor_id` int(11) NOT NULL COMMENT 'vendor.vendor_id',
  `cate_code_big` varchar(10) NOT NULL COMMENT '廠商類別大分類1碼',
  `cate_code_middle` varchar(10) NOT NULL COMMENT '廠商類別中分類2碼',
  `cate_code_serial` varchar(10) NOT NULL COMMENT '廠商類別流水號5碼',
  `tax_type` varchar(10) NOT NULL COMMENT '稅別代碼 1:應稅 2:免稅',
  PRIMARY KEY (`rid`)
) ENGINE=InnoDB AUTO_INCREMENT=307 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for vendor_login
-- ----------------------------
DROP TABLE IF EXISTS `vendor_login`;
CREATE TABLE `vendor_login` (
  `login_id` int(10) unsigned NOT NULL,
  `vendor_id` int(9) unsigned NOT NULL,
  `login_ipfrom` varchar(40) NOT NULL DEFAULT '',
  `login_createdate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`login_id`),
  KEY `ix_vendor_login_vid` (`vendor_id`),
  CONSTRAINT `fk_vendor_login_vid` FOREIGN KEY (`vendor_id`) REFERENCES `vendor` (`vendor_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for vip_user
-- ----------------------------
DROP TABLE IF EXISTS `vip_user`;
CREATE TABLE `vip_user` (
  `v_id` int(9) unsigned NOT NULL AUTO_INCREMENT,
  `user_email` varchar(100) NOT NULL DEFAULT '',
  `user_id` int(9) unsigned NOT NULL DEFAULT '0',
  `status` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `group_id` int(9) unsigned NOT NULL DEFAULT '0',
  `emp_id` varchar(25) DEFAULT '',
  `createdate` int(10) unsigned NOT NULL DEFAULT '0',
  `source` int(2) DEFAULT '0' COMMENT '添加來源 1用戶 2客服',
  `create_id` int(11) DEFAULT NULL COMMENT '創建人',
  `update_id` int(11) DEFAULT NULL COMMENT '修改人',
  `updatedate` int(10) DEFAULT NULL COMMENT '修改時間',
  PRIMARY KEY (`v_id`),
  KEY `index_uid,index_gid` (`user_id`,`group_id`),
  KEY `group_id` (`group_id`)
) ENGINE=InnoDB AUTO_INCREMENT=72830 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for vip_user_copy
-- ----------------------------
DROP TABLE IF EXISTS `vip_user_copy`;
CREATE TABLE `vip_user_copy` (
  `v_id` int(9) unsigned NOT NULL AUTO_INCREMENT,
  `user_email` varchar(100) NOT NULL DEFAULT '',
  `user_id` int(9) unsigned NOT NULL DEFAULT '0',
  `status` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `group_id` int(9) unsigned NOT NULL DEFAULT '0',
  `emp_id` varchar(25) DEFAULT '',
  `createdate` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`v_id`),
  KEY `index_uid,index_gid` (`user_id`,`group_id`),
  KEY `group_id` (`group_id`)
) ENGINE=InnoDB AUTO_INCREMENT=71562 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for vip_user_group
-- ----------------------------
DROP TABLE IF EXISTS `vip_user_group`;
CREATE TABLE `vip_user_group` (
  `group_id` int(9) unsigned NOT NULL DEFAULT '0',
  `group_name` varchar(255) DEFAULT NULL,
  `domain` varchar(25) NOT NULL DEFAULT '''''',
  `tax_id` varchar(25) NOT NULL DEFAULT '''''',
  `group_code` varchar(50) DEFAULT NULL COMMENT '公司代號',
  `group_capital` int(11) DEFAULT NULL COMMENT '資本額',
  `group_emp_number` int(11) DEFAULT NULL COMMENT '員工人數',
  `group_emp_age` varchar(25) DEFAULT NULL COMMENT '年齡分佈，可複選 1=20歲(含)以下； 2=21~30歲；3=31~40歲；4=41~50歲；5=51~60歲；6=61歲以上',
  `group_emp_gender` int(2) DEFAULT NULL COMMENT '男女比率，單選1=多於；2=等於；3=少於',
  `group_benefit_type` varchar(25) NOT NULL COMMENT '福利類別，可複選1=福利金;2=年節禮劵;3=年節禮盒;4=現金;5=旅遊津貼;0=其他',
  `group_benefit_desc` varchar(200) DEFAULT NULL COMMENT '福利發放簡述',
  `group_subsidiary` int(2) DEFAULT '0' COMMENT '是否為子公司 1= 為子公司;0= 非子公司',
  `group_hq_name` varchar(50) NOT NULL COMMENT '總公司名稱',
  `group_hq_code` varchar(50) NOT NULL COMMENT '總公司編號',
  `group_committe_name` varchar(50) DEFAULT NULL COMMENT '福委會全名',
  `group_committe_code` varchar(50) DEFAULT NULL COMMENT '福委會統編',
  `group_committe_promotion` varchar(100) NOT NULL COMMENT '福利推廣，可复选 1=特約商店;2=特約購物平台;0=其他',
  `group_committe_desc` varchar(200) DEFAULT NULL COMMENT '推廣簡述',
  `image_name` varchar(40) NOT NULL DEFAULT '''''',
  `gift_bonus` int(4) unsigned NOT NULL DEFAULT '0' COMMENT '贈送購物金',
  `createdate` int(9) unsigned NOT NULL DEFAULT '0',
  `group_category` int(9) unsigned NOT NULL DEFAULT '0',
  `bonus_rate` int(3) NOT NULL DEFAULT '1' COMMENT '贈購物金倍率',
  `bonus_expire_day` int(3) NOT NULL DEFAULT '90' COMMENT '購物金有效期',
  `eng_name` varchar(50) NOT NULL DEFAULT '',
  `check_iden` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `site_id` int(9) unsigned NOT NULL DEFAULT '0',
  `group_status` int(2) DEFAULT NULL COMMENT '啟用狀態    1= 啟用 0= 停用',
  `k_user` int(11) DEFAULT NULL COMMENT '創建人員',
  `k_date` datetime DEFAULT NULL COMMENT '創建時間',
  `m_user` int(11) DEFAULT NULL COMMENT '異動人員',
  `m_date` datetime DEFAULT NULL COMMENT '異動時間',
  PRIMARY KEY (`group_id`),
  KEY `index_name,index_eng` (`group_name`,`eng_name`),
  KEY `index_category` (`group_category`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for vote_article
-- ----------------------------
DROP TABLE IF EXISTS `vote_article`;
CREATE TABLE `vote_article` (
  `article_id` int(9) NOT NULL AUTO_INCREMENT COMMENT '文章編號',
  `product_id` int(9) unsigned DEFAULT NULL COMMENT '商品編號',
  `event_id` int(9) DEFAULT NULL COMMENT '活動編號',
  `user_id` int(9) DEFAULT NULL COMMENT '會員編號',
  `article_content` varchar(2000) DEFAULT NULL,
  `article_status` int(1) DEFAULT '1' COMMENT '文章狀態',
  `article_title` varchar(200) DEFAULT NULL COMMENT '文章標題',
  `article_banner` varchar(100) DEFAULT NULL COMMENT '文章大圖',
  `create_user` int(9) NOT NULL COMMENT '創建人',
  `create_time` datetime NOT NULL COMMENT '創建時間',
  `update_time` datetime NOT NULL COMMENT '修改時間',
  `update_user` int(9) NOT NULL COMMENT '修改人',
  `vote_count` int(9) NOT NULL DEFAULT '0' COMMENT '投票次數',
  `article_sort` int(9) NOT NULL DEFAULT '0' COMMENT '文章排序 數值越大越靠前',
  `prod_link` varchar(100) NOT NULL COMMENT '商品連結',
  `article_start_time` datetime DEFAULT NULL COMMENT '文章開始時間',
  `article_end_time` datetime DEFAULT NULL COMMENT '文章結束時間',
  `article_show_start_time` datetime DEFAULT NULL COMMENT '顯示開始時間',
  `article_show_end_time` datetime DEFAULT NULL COMMENT '顯示結束時間',
  PRIMARY KEY (`article_id`),
  KEY `event_id` (`event_id`)
) ENGINE=InnoDB AUTO_INCREMENT=45 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for vote_detail
-- ----------------------------
DROP TABLE IF EXISTS `vote_detail`;
CREATE TABLE `vote_detail` (
  `vote_id` int(9) NOT NULL AUTO_INCREMENT,
  `article_id` int(9) DEFAULT NULL,
  `user_id` int(9) DEFAULT NULL COMMENT '會員編號',
  `ip` varchar(40) DEFAULT NULL COMMENT 'ip地址',
  `vote_status` int(9) DEFAULT '1' COMMENT '投票狀態',
  `create_user` int(9) DEFAULT NULL,
  `update_user` int(9) DEFAULT NULL,
  `create_time` datetime DEFAULT NULL COMMENT '創建時間',
  `update_time` datetime DEFAULT NULL,
  PRIMARY KEY (`vote_id`),
  KEY `article_id` (`article_id`),
  CONSTRAINT `vote_detail_ibfk_1` FOREIGN KEY (`article_id`) REFERENCES `vote_article` (`article_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=1511 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for vote_event
-- ----------------------------
DROP TABLE IF EXISTS `vote_event`;
CREATE TABLE `vote_event` (
  `event_id` int(9) NOT NULL AUTO_INCREMENT COMMENT '活動編號',
  `event_name` varchar(100) DEFAULT NULL COMMENT '活動名稱',
  `event_desc` varchar(200) DEFAULT NULL COMMENT '活動描述',
  `event_banner` varchar(100) DEFAULT NULL COMMENT '活動大圖',
  `event_start` datetime DEFAULT NULL COMMENT '活動起日',
  `event_end` datetime DEFAULT NULL COMMENT '活動迄日',
  `word_length` int(9) DEFAULT '0' COMMENT '字數長度 0不限制',
  `vote_everyone_limit` int(9) NOT NULL DEFAULT '0' COMMENT '每人投票次數限制',
  `vote_everyday_limit` int(9) NOT NULL DEFAULT '0' COMMENT '每天投票限制',
  `number_limit` int(4) NOT NULL DEFAULT '1' COMMENT '單一用戶贈送次數',
  `present_event_id` varchar(10) DEFAULT NULL COMMENT '共用促銷編號  new_promo_present.event_id',
  `create_user` int(9) NOT NULL COMMENT '創建人',
  `create_time` datetime NOT NULL COMMENT '創建時間',
  `update_user` int(9) NOT NULL COMMENT '修改人',
  `update_time` datetime NOT NULL COMMENT '修改時間',
  `event_status` int(4) DEFAULT '1' COMMENT ' 活動狀態 1啟用 2禁用',
  `is_repeat` int(1) NOT NULL DEFAULT '0' COMMENT '是否重覆投選 0為不可重複投遞 1為可重複投遞 默認為0 ',
  PRIMARY KEY (`event_id`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for vote_message
-- ----------------------------
DROP TABLE IF EXISTS `vote_message`;
CREATE TABLE `vote_message` (
  `message_id` int(9) NOT NULL AUTO_INCREMENT COMMENT '留言流水號',
  `article_id` int(9) NOT NULL COMMENT '文章編號',
  `ip` varchar(40) NOT NULL COMMENT 'ip地址',
  `message_status` int(9) NOT NULL DEFAULT '1' COMMENT '狀態 0禁用  1啟用',
  `message_content` varchar(1000) NOT NULL COMMENT '留言內容',
  `create_time` datetime NOT NULL COMMENT '留言時間',
  `create_user` int(9) NOT NULL COMMENT '會員編號',
  `update_time` datetime DEFAULT NULL,
  `update_user` int(9) DEFAULT NULL,
  PRIMARY KEY (`message_id`),
  KEY `article_id` (`article_id`),
  CONSTRAINT `vote_message_ibfk_1` FOREIGN KEY (`article_id`) REFERENCES `vote_article` (`article_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for votes
-- ----------------------------
DROP TABLE IF EXISTS `votes`;
CREATE TABLE `votes` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `vote_id` int(11) NOT NULL,
  `vote_item_id` int(11) NOT NULL,
  `facebook_id` bigint(20) NOT NULL,
  `created` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=750 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for web_content_type_setup
-- ----------------------------
DROP TABLE IF EXISTS `web_content_type_setup`;
CREATE TABLE `web_content_type_setup` (
  `content_id` int(11) NOT NULL AUTO_INCREMENT,
  `web_content_type` varchar(50) CHARACTER SET utf8 DEFAULT NULL,
  `site_id` int(11) DEFAULT NULL COMMENT '網站',
  `page_id` int(11) DEFAULT NULL COMMENT '網頁',
  `area_id` int(11) DEFAULT NULL COMMENT '區域',
  `type_id` int(11) DEFAULT NULL COMMENT '類別',
  `default_link_url` varchar(255) CHARACTER SET utf8 DEFAULT NULL COMMENT '預設聯結',
  `area_name` varchar(100) CHARACTER SET utf8 DEFAULT NULL COMMENT '說明',
  `content_default_num` int(11) DEFAULT NULL COMMENT '預設數量限制',
  `content_status_num` int(11) DEFAULT NULL COMMENT '啟用數量限制',
  `content_status` int(11) DEFAULT '1',
  PRIMARY KEY (`content_id`)
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=latin1 COMMENT='參數2';

-- ----------------------------
-- Table structure for web_content_type1
-- ----------------------------
DROP TABLE IF EXISTS `web_content_type1`;
CREATE TABLE `web_content_type1` (
  `content_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '序號',
  `site_id` int(11) DEFAULT NULL COMMENT '網站：site=7=health',
  `page_id` int(11) DEFAULT NULL COMMENT '網頁：page=07,04,05,06',
  `area_id` int(11) DEFAULT NULL COMMENT '區域：area=0703,0705,0402,0502,0602',
  `type_id` int(11) DEFAULT NULL COMMENT '類別：',
  `content_title` varchar(100) CHARACTER SET utf8 DEFAULT NULL COMMENT 'title:說明',
  `content_image` varchar(255) CHARACTER SET utf8 DEFAULT NULL COMMENT '圖:路徑或檔名',
  `content_default` int(11) DEFAULT NULL COMMENT '預設：0=預設(勾選啟用的才能設預設)，最多1筆，非預設=1',
  `content_status` int(11) DEFAULT '1' COMMENT '啟用:0=不啟用，1=啟用(預設)，前台隨機',
  `link_url` varchar(255) CHARACTER SET utf8 DEFAULT NULL COMMENT '聯結:自行輸入(連結回吉甲地影音專區的某個影片)',
  `link_page` varchar(255) CHARACTER SET utf8 DEFAULT NULL COMMENT '顯示頁面:在那個頁面顯示(如food.php,life.php)，空值代表不設限',
  `link_mode` int(11) DEFAULT NULL COMMENT '開新視窗:0=不開，1=開原視窗(預設)，2=開新視窗_blank',
  `update_on` datetime DEFAULT NULL COMMENT '修改日期',
  `created_on` datetime DEFAULT NULL COMMENT '建立日期',
  PRIMARY KEY (`content_id`)
) ENGINE=InnoDB AUTO_INCREMENT=146 DEFAULT CHARSET=latin1 COMMENT='07首頁(影音、廣告)';

-- ----------------------------
-- Table structure for web_content_type2
-- ----------------------------
DROP TABLE IF EXISTS `web_content_type2`;
CREATE TABLE `web_content_type2` (
  `content_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '序號',
  `site_id` int(11) DEFAULT NULL COMMENT '網站:7=預設，site_name=health',
  `page_id` int(11) DEFAULT NULL COMMENT '網頁：page=07,01,02',
  `area_id` int(11) DEFAULT NULL COMMENT '區域：area=0701,0102,0202',
  `type_id` int(11) DEFAULT NULL COMMENT '類別',
  `content_title` varchar(100) CHARACTER SET utf8 DEFAULT NULL COMMENT 'title：說明',
  `content_image` varchar(255) CHARACTER SET utf8 DEFAULT NULL COMMENT '圖：路徑或檔名',
  `home_title` varchar(50) CHARACTER SET utf8 DEFAULT NULL COMMENT '首頁標',
  `home_text` varchar(100) CHARACTER SET utf8 DEFAULT NULL COMMENT '首頁文',
  `product_id` int(11) DEFAULT NULL COMMENT '商品編號：商品編號(開查詢頁面後選擇，限健康20類別內品牌)',
  `content_default` int(11) DEFAULT NULL COMMENT '預設：0=預設(勾選啟用的才能設預設)，最多3筆，非預設=1',
  `content_status` int(11) DEFAULT '1' COMMENT '啟用：0=不啟用，1=啟用(預設)，前台隨機',
  `link_url` varchar(255) CHARACTER SET utf8 DEFAULT NULL COMMENT '聯結：回吉甲地本站商品頁http://www.gigade100.com/product.php，範例http://www.gigade100.com/product.php?pid=13729&cid=4',
  `link_page` varchar(50) CHARACTER SET utf8 DEFAULT NULL COMMENT '顯示頁面：在那個頁面顯示(如food.php,life.php)，空值代表不設限',
  `link_mode` int(11) DEFAULT '1' COMMENT '開新視窗：0=不開，1=開原視窗(預設)，2=開新視窗_blank',
  `start_time` datetime DEFAULT NULL COMMENT '上架時間',
  `end_time` datetime DEFAULT NULL COMMENT '下架時間',
  `update_on` datetime DEFAULT NULL COMMENT '修改日期',
  `created_on` datetime DEFAULT NULL COMMENT '建立日期',
  PRIMARY KEY (`content_id`)
) ENGINE=InnoDB AUTO_INCREMENT=103 DEFAULT CHARSET=latin1 COMMENT='07首頁(吉食分享，本月推薦)';

-- ----------------------------
-- Table structure for web_content_type3
-- ----------------------------
DROP TABLE IF EXISTS `web_content_type3`;
CREATE TABLE `web_content_type3` (
  `content_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '序號',
  `site_id` int(11) DEFAULT NULL COMMENT '網站:7=預設，site_name=health',
  `page_id` int(11) DEFAULT NULL COMMENT '網頁：page=07,04,05,06',
  `area_id` int(11) DEFAULT NULL COMMENT '區域：area=0704,0403,0503,0603',
  `type_id` int(11) DEFAULT NULL COMMENT '類別',
  `content_title` varchar(255) CHARACTER SET utf8 DEFAULT NULL COMMENT 'title:說明',
  `content_image` varchar(255) CHARACTER SET utf8 DEFAULT NULL COMMENT '圖：路徑或檔名',
  `brand_id` int(11) DEFAULT NULL COMMENT '品牌：品牌編號(開查詢頁面後選擇，限健康20類別內品牌)',
  `content_default` int(11) DEFAULT NULL COMMENT '預設：0=預設(勾選啟用的才能設預設)，最多4筆，非預設=1',
  `content_status` int(11) DEFAULT '1' COMMENT '啟用：0=不啟用，1=啟用(預設)，前台隨機',
  `link_url` varchar(255) CHARACTER SET utf8 DEFAULT NULL COMMENT '聯結：brand.php',
  `link_page` varchar(50) CHARACTER SET utf8 DEFAULT NULL COMMENT '顯示頁面：在那個頁面顯示(brand.php)，空值代表不設限',
  `link_mode` int(11) DEFAULT '1' COMMENT '開新視窗：0=不開，1=開原視窗(預設)，2=開新視窗_blank',
  `update_on` datetime DEFAULT NULL COMMENT '修改日期',
  `created_on` datetime DEFAULT NULL COMMENT '建立日期',
  PRIMARY KEY (`content_id`)
) ENGINE=InnoDB AUTO_INCREMENT=170 DEFAULT CHARSET=latin1 COMMENT='07首頁(品牌推薦)';

-- ----------------------------
-- Table structure for web_content_type4
-- ----------------------------
DROP TABLE IF EXISTS `web_content_type4`;
CREATE TABLE `web_content_type4` (
  `content_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '序號',
  `site_id` int(11) DEFAULT '7' COMMENT '網站,7=預設,site_name=health',
  `page_id` int(11) DEFAULT NULL COMMENT '網頁,page=03',
  `area_id` int(11) DEFAULT NULL COMMENT '區域,area=0301',
  `type_id` int(11) DEFAULT NULL COMMENT '類別',
  `brand_id` int(11) DEFAULT NULL COMMENT '品牌編號,秀出所有品牌清單后選擇編輯,健康2.0底下的品牌都要有品牌故事',
  `content_html` text CHARACTER SET utf8 COMMENT '文,格式txt,使用html編輯器',
  `content_default` int(11) DEFAULT NULL COMMENT '預設：0=預設(勾選啟用的才能設預設)，最多1筆，非預設=1',
  `content_status` int(11) DEFAULT '1' COMMENT '啟用：0=不啟用，1=啟用(預設)，不隨機',
  `link_url` varchar(255) CHARACTER SET utf8 DEFAULT NULL COMMENT '連接,http://www.gigade100.com/brand.php,范例:http://www.gigade100.com/brand.php?cid=4&bid=249#readstory',
  `link_mode` int(11) DEFAULT '1' COMMENT '開新視窗,0=不開,1=開原視窗(預設),2=開新視窗_blank',
  `update_on` datetime DEFAULT NULL COMMENT '修改日期',
  `created_on` datetime DEFAULT NULL COMMENT '建立日期',
  PRIMARY KEY (`content_id`)
) ENGINE=InnoDB AUTO_INCREMENT=133 DEFAULT CHARSET=latin1 COMMENT='03(品牌頁)';

-- ----------------------------
-- Table structure for web_content_type5
-- ----------------------------
DROP TABLE IF EXISTS `web_content_type5`;
CREATE TABLE `web_content_type5` (
  `content_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '序號,自動增長',
  `site_id` int(11) DEFAULT '7' COMMENT '網站,7=預設,site_name=health',
  `page_id` int(11) DEFAULT NULL COMMENT '網頁,page=01,02',
  `area_id` int(11) DEFAULT NULL COMMENT '區域,area=0101,0201',
  `type_id` int(11) DEFAULT NULL COMMENT '類別',
  `brand_id` int(11) DEFAULT NULL COMMENT '品牌：品牌編號(開查詢頁面後選擇，限健康20類別內品牌)',
  `content_title` varchar(50) CHARACTER SET utf8 DEFAULT NULL COMMENT 'title',
  `content_image` varchar(255) CHARACTER SET utf8 DEFAULT NULL COMMENT '圖,路徑或檔名',
  `content_default` int(11) DEFAULT '0' COMMENT '預設,0=預設(自動設為啟用),只能設一筆,非預設=1',
  `content_status` int(11) DEFAULT '1' COMMENT '啟用,0=不啟用,1=啟用(預設),最大5筆,新增時超過5筆時將最舊的設為不啟用',
  `link_url` varchar(255) CHARACTER SET utf8 DEFAULT NULL COMMENT '連結.01,02=brand.php',
  `link_mode` int(11) DEFAULT '1' COMMENT '開新視窗.0=不開,1=開原視窗(預設),2=開新視窗_blank',
  `update_on` datetime DEFAULT NULL COMMENT '修改日期',
  `created_on` datetime DEFAULT NULL COMMENT '建立日期',
  PRIMARY KEY (`content_id`)
) ENGINE=InnoDB AUTO_INCREMENT=98 DEFAULT CHARSET=latin1 COMMENT='01(食安)02(生活)';

-- ----------------------------
-- Table structure for web_content_type6
-- ----------------------------
DROP TABLE IF EXISTS `web_content_type6`;
CREATE TABLE `web_content_type6` (
  `content_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '序號.自動增長',
  `site_id` int(11) DEFAULT '7' COMMENT '網站.7=預設,site_name=health',
  `page_id` int(11) DEFAULT NULL COMMENT '網頁.pgae=4',
  `area_id` int(11) DEFAULT NULL COMMENT '區域.area=0401',
  `type_id` int(11) DEFAULT NULL COMMENT '類別',
  `home_title` varchar(50) CHARACTER SET utf8 DEFAULT NULL COMMENT '首頁標',
  `content_title` varchar(255) CHARACTER SET utf8 DEFAULT NULL COMMENT '內頁標',
  `content_html` text CHARACTER SET utf8 COMMENT '內頁文.格式txt,使用html編輯器',
  `home_image` varchar(255) CHARACTER SET utf8 DEFAULT NULL COMMENT '首頁大圖.路徑或檔名,預設(content_default欄)=1時此欄位一定要有值',
  `content_default` int(11) DEFAULT '0' COMMENT '預設.0=預設(自動設為啟用),只能設一筆,非預設=1',
  `content_status` int(11) DEFAULT '1' COMMENT '啟用.0=不啟用,1=啟用(預設)',
  `link_url` varchar(255) CHARACTER SET utf8 DEFAULT NULL COMMENT '連結.04=gigade.php',
  `link_mode` int(11) DEFAULT '1' COMMENT '開新視窗.0=不開,1=開原視窗(預設),2=開新視窗_blank',
  `update_on` datetime DEFAULT NULL COMMENT '修改日期',
  `created_on` datetime DEFAULT NULL COMMENT '建立日期',
  `keywords` varchar(255) CHARACTER SET utf8 NOT NULL,
  `content_total_click` int(10) DEFAULT '0',
  PRIMARY KEY (`content_id`)
) ENGINE=InnoDB AUTO_INCREMENT=79 DEFAULT CHARSET=latin1 COMMENT='04(吉甲地單元)';

-- ----------------------------
-- Table structure for web_content_type7
-- ----------------------------
DROP TABLE IF EXISTS `web_content_type7`;
CREATE TABLE `web_content_type7` (
  `content_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '序號',
  `site_id` int(11) DEFAULT NULL COMMENT '網站：7=預設，site_name=health',
  `page_id` int(11) DEFAULT NULL COMMENT '網頁：page=05,06',
  `area_id` int(11) DEFAULT NULL COMMENT '區域:area=0501,0601',
  `type_id` int(11) DEFAULT NULL COMMENT '類別',
  `home_title` varchar(50) CHARACTER SET utf8 DEFAULT NULL COMMENT '首頁標',
  `home_text` varchar(100) CHARACTER SET utf8 DEFAULT NULL COMMENT '首頁文',
  `content_title` varchar(255) CHARACTER SET utf8 DEFAULT NULL COMMENT '內頁標',
  `content_html` text CHARACTER SET utf8 COMMENT '內頁文：格式txt，使用html編輯器',
  `content_image` varchar(255) CHARACTER SET utf8 DEFAULT NULL COMMENT '首頁圖：路徑或檔名',
  `content_default` int(11) DEFAULT NULL COMMENT '預設：0=預設(最左邊顯示，自動設為啟用)，只能設一筆，非預設=1',
  `content_status` int(11) DEFAULT '1' COMMENT '啟用：0=不啟用，1=啟用(預設)，最多4筆，新增時超過4筆時將最舊的設為不啟用',
  `link_url` varchar(255) CHARACTER SET utf8 DEFAULT NULL COMMENT '聯結：05=recommend.php(明星)，06=pro.php(達人)',
  `link_mode` int(11) DEFAULT '1' COMMENT '開新視窗：0=不開，1=開原視窗(預設)，2=開新視窗_blank',
  `update_on` datetime DEFAULT NULL COMMENT '修改日期',
  `created_on` datetime DEFAULT NULL COMMENT '建立日期',
  `keywords` varchar(255) CHARACTER SET utf8 DEFAULT NULL,
  `sort` int(9) DEFAULT '0',
  `start_time` datetime DEFAULT NULL,
  `end_time` datetime DEFAULT NULL,
  `content_total_click` int(10) DEFAULT '0',
  PRIMARY KEY (`content_id`)
) ENGINE=InnoDB AUTO_INCREMENT=88 DEFAULT CHARSET=latin1 COMMENT='05(明星)、06(達人)';

-- ----------------------------
-- Table structure for web_content_type8
-- ----------------------------
DROP TABLE IF EXISTS `web_content_type8`;
CREATE TABLE `web_content_type8` (
  `content_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '序號.自動增長',
  `site_id` int(11) DEFAULT NULL COMMENT '網站',
  `page_id` int(11) DEFAULT NULL COMMENT '網頁:page=07',
  `area_id` int(11) DEFAULT NULL COMMENT '區域:area=0702',
  `type_id` int(11) DEFAULT NULL COMMENT '類別',
  `home_title` varchar(50) DEFAULT NULL COMMENT '首頁標',
  `home_image` varchar(255) DEFAULT NULL COMMENT '首頁大圖',
  `content_default` int(11) DEFAULT NULL COMMENT '預設:前臺取消預設限制（本欄位暫時不用）',
  `content_status` int(11) DEFAULT NULL COMMENT '啟用:0=不啟用，1=啟用（預設）',
  `link_url` varchar(255) DEFAULT NULL COMMENT '連接',
  `link_mode` int(11) DEFAULT NULL COMMENT '0=不開，1=開原視窗（預設），2=開新視窗_blank',
  `update_on` datetime DEFAULT NULL COMMENT '修改日期',
  `created_on` datetime DEFAULT NULL COMMENT '建立日期',
  `big_title` varchar(50) DEFAULT NULL,
  `small_title` varchar(50) DEFAULT NULL,
  `sort` int(9) DEFAULT '0',
  `start_time` datetime DEFAULT NULL,
  `end_time` datetime DEFAULT NULL,
  PRIMARY KEY (`content_id`)
) ENGINE=InnoDB AUTO_INCREMENT=63 DEFAULT CHARSET=utf8;

-- ----------------------------
-- View structure for get_productinfo
-- ----------------------------
DROP VIEW IF EXISTS `get_productinfo`;
