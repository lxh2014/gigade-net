/*
* Kendo UI Localization Project for v2012.3.1114 
* Copyright 2012 Telerik AD. All rights reserved.
* 
* Simplified Chinese (zh-CN) Language Pack
*
* Project home  : https://github.com/loudenvier/kendo-global
* Kendo UI home : http://kendoui.com
* Author        : IKKI Phoenix  
*                 
*
* This project is released to the public domain, although one must abide to the 
* licensing terms set forth by Telerik to use Kendo UI, as shown bellow.
*
* Telerik's original licensing terms:
* -----------------------------------
* Kendo UI Web commercial licenses may be obtained at
* https://www.kendoui.com/purchase/license-agreement/kendo-ui-web-commercial.aspx
* If you do not own a commercial license, this file shall be governed by the
* GNU General Public License (GPL) version 3.
* For GPL requirements, please review: http://www.gnu.org/copyleft/gpl.html
*/

kendo.culture("zh-TW"); // Add by IKKI
kendo.ui.Locale = "Traditional Chinese (zh-TW)";

kendo.ui.ColumnMenu.prototype.options.messages = 
	$.extend(kendo.ui.ColumnMenu.prototype.options.messages, {

/* COLUMN MENU MESSAGES 
 ****************************************************************************/   
	sortAscending: "升序排列",
	sortDescending: "降序排列",
	filter: "筛选",
	columns: "字段列"
 /***************************************************************************/   
});

kendo.ui.Groupable.prototype.options.messages = 
	$.extend(kendo.ui.Groupable.prototype.options.messages, {

/* GRID GROUP PANEL MESSAGES 
 ****************************************************************************/   
	empty: "将字段列名称拖拽到此处可进行该列的分组显示"
 /***************************************************************************/   
});

kendo.ui.FilterMenu.prototype.options.messages = 
	$.extend(kendo.ui.FilterMenu.prototype.options.messages, {
  
/* FILTER MENU MESSAGES 
 ***************************************************************************/   
	info: "筛选条件：",	// sets the text on top of the filter menu
	filter: "筛选",		// sets the text for the "Filter" button
	clear: "清空",		// sets the text for the "Clear" button
	// when filtering boolean numbers
	isTrue: "是",		// sets the text for "isTrue" radio button
	isFalse: "否",		// sets the text for "isFalse" radio button
	//changes the text of the "And" and "Or" of the filter menu
	and: "并且",
	or: "或者",
	selectValue: "-= 请选择 =-"
 /***************************************************************************/   
});
         
kendo.ui.FilterMenu.prototype.options.operators =           
	$.extend(kendo.ui.FilterMenu.prototype.options.operators, {

/* FILTER MENU OPERATORS (for each supported data type) 
 ****************************************************************************/   
	string: {
		eq: "等于",
		neq: "不等于",
		contains: "包含",
		doesnotcontain: "不包含",
		startswith: "开始于",
		endswith: "结束于"
	},
	number: {
		eq: "等于",
		neq: "不等于",
		gt: "大于",
		gte: "大于等于",
		lt: "小于",
		lte: "小于等于"
	},
	date: {
		eq: "等于",
		neq: "不等于",
		gt: "晚于",
		gte: "晚于等于",
		lt: "早于",
		lte: "早于等于"
	},
	enums: {
		eq: "等于",
		neq: "不等于"
	}
 /***************************************************************************/   
});

kendo.ui.Pager.prototype.options.messages = 
	$.extend(kendo.ui.Pager.prototype.options.messages, {
  
/* PAGER MESSAGES 
 ****************************************************************************/   
	display: "{0} - {1} 条　共 {2} 条数据",
	empty: "无数据",
	page: "转到第",
	of: "页　共 {0} 页",
	itemsPerPage: "条每页",
	first: "首页",
	previous: "上一页",
	next: "下一页",
	last: "尾页",
	refresh: "刷新"
 /***************************************************************************/   
});

kendo.ui.Validator.prototype.options.messages = 
	$.extend(kendo.ui.Validator.prototype.options.messages, {

/* VALIDATOR MESSAGES 
 ****************************************************************************/   
	required: "{0} 是必填项！",
	pattern: "{0} 的格式不正确！",
	min: "{0} 必须大于或等于 {1} ！",
	max: "{0} 必须小于或等于 {1} ！",
	step: "{0} 不是正确的步进值！",
	email: "{0} 不是正确的电子邮件！",
	url: "{0} 不是正确的网址！",
	date: "{0} 不是正确的日期！"
 /***************************************************************************/   
});

// The upload part add by IKKI
kendo.ui.Upload.prototype.options.localization = 
	$.extend(kendo.ui.Upload.prototype.options.localization, {

/* UPLOAD LOCALIZATION
 ****************************************************************************/   
	select: "選擇文件",
	dropFilesHere: "將文件拖拽到此處上傳",
	cancel: "取消",
	remove: "移除",
	uploadSelectedFiles: "上傳文件",
	statusUploading: "上傳中……",
	statusUploaded: "上傳成功！",
	statusFailed: "上傳失敗！",
	retry: "重試"
 /***************************************************************************/   
});

kendo.ui.ImageBrowser.prototype.options.messages = 
	$.extend(kendo.ui.ImageBrowser.prototype.options.messages, {

/* IMAGE BROWSER MESSAGES 
 ****************************************************************************/   
	uploadFile: "上傳文件",
	orderBy: "排序方式",
	orderByName: "按名稱",
	orderBySize: "按大小",
	directoryNotFound: "目錄為找到",
	emptyFolder: "空文件夾",
	deleteFile: '你確定要刪除【{0}】這個文件嗎？',
	invalidFileType: "你上傳的文件格式 {0} 是無效的，支持的文件類型為：{1}",
	overwriteFile: "一個名稱為【{0}】的文件已經存在，是否覆蓋？",
	dropFilesHere: "將文件拖拽到此處上傳"
 /***************************************************************************/   
	});

kendo.ui.FileBrowser.prototype.options.messages =
	$.extend(kendo.ui.FileBrowser.prototype.options.messages, {

	    /* IMAGE BROWSER MESSAGES 
         ****************************************************************************/
	    uploadFile: "上傳文件",
	    orderBy: "排序方式",
	    orderByName: "按名稱",
	    orderBySize: "按大小",
	    directoryNotFound: "目錄未找到",
	    emptyFolder: "空文件夾",
	    deleteFile: '你確定要刪除【{0}】這個文件嗎？',
	    invalidFileType: "你上傳的文件格式 {0} 是無效的，支持的文件類型為：{1}",
	    overwriteFile: "一個名稱為【{0}】的文件已經存在，是否覆蓋？",
	    dropFilesHere: "將文件拖拽到此處上傳"
	    /***************************************************************************/
	});

kendo.ui.Editor.prototype.options.messages = 
	$.extend(kendo.ui.Editor.prototype.options.messages, {

/* EDITOR MESSAGES 
 ****************************************************************************/   
	bold: "粗體",
	italic: "斜體",
	underline: "下劃線",
	strikethrough: "刪除線",
	superscript: "上標",
	subscript: "下標",
	justifyCenter: "居中對齊",
	justifyLeft: "左對齊",
	justifyRight: "右對齊",
	justifyFull: "兩端對齊",
	insertUnorderedList: "插入無序列表",
	insertOrderedList: "插入有序列表",
	indent: "增加縮進",
	outdent: "減少縮進",
	createLink: "插入鏈接",
	unlink: "刪除鏈接",
	insertImage: "插入圖片",
	insertFile: "插入文件",
	insertHtml: "插入Html",
	viewHtml: "查看HTML",
	fontName: "請選擇字體",
	fontNameInherit: "（默認字體）",
	fontSize: "請選擇字號",
	fontSizeInherit: "（默認字號）",
	formatBlock: "格式",
	formatting: "格式選擇",
	foreColor: "文字顏色",
	backColor: "文字背景色",
	style: "樣式",
	emptyFolder: "空文件夾",
	uploadFile: "上傳文件",
	orderBy: "排序方式：",
	orderBySize: "按大小排序",
	orderByName: "按名稱排序",
	invalidFileType: "你上傳的文件格式 {0} 是無效的，支持的文件類型為：{1}",
	deleteFile: '你確定要刪除【{0}】這個文件嗎？',
	overwriteFile: '一個名稱為【{0}】的文件已經存在，是否覆蓋？',
	directoryNotFound: "目錄未找到",
	imageWebAddress: "圖片鏈接地址",
	imageAltText: "圖片佔位符",
	imageWidth: "顯示寬度 (px)",
	imageHeight: "顯示高度 (px)",
	fileWebAddress: "文件鏈接地址",
	fileTitle: "標題",
	linkWebAddress: "鏈接地址",
	linkText: "鏈接文字",
	linkToolTip: "文字提示",
	linkOpenInNewWindow: "是否在新窗口中打開",
	dialogInsert: "插入",
	dialogButtonSeparator: "或",
	dialogCancel: "取消",
	createTable: "新建表格",
	createTableHint: "新建一個 {0} x {1} 表格",
	addColumnLeft: "左側添加列",
	addColumnRight: "右側添加列",
	addRowAbove: "上側添加行",
	addRowBelow: "下側添加行",
	deleteRow: "刪除行",
	deleteColumn: "刪除列"
 /***************************************************************************/   
});
