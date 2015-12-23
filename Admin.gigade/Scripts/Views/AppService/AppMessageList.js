/// <reference path="AppMessageList.js" />
/*
* 文件名稱 :AppMessageList.js
* 文件功能描述 :訊息公告列表JS
* 版權宣告 :
* 開發人員 : 白明威
* 版本資訊 : 1.0
* 日期 : 2015.8.27
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/

var CallidForm;
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//站臺管理Model
Ext.define('gigade.AppMessage', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "message_id", type: "int" },//序號
        { name: "type", type: "int" },//類別
        { name: "title", type: "string" },//抬頭
        { name: "content", type: "string" },//內容
        { name: "messagedate_time", type: "datetime" },//新增訊息日期
        { name: "group" },
        { name: "linkurl", type: "string" },//連接url
        { name: "display_type", type: "int" },//顯示類別
        { name: "msg_start_time", type: "datetime" },//訊息開始時間
        { name: "msg_end_time", type: "datetime" },//訊息結束時間
        { name: "fit_os", type: "string" },//適用平台
        { name: "appellation", type: "string" },//稱謂
        { name: "need_login" }
    ]
});

//頁面store
var AppMessageStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    autoLoad: false,
    model: 'gigade.AppMessage',
    proxy: {
        type: 'ajax',
        url: '/AppService/GetAppMessageList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

//適用平臺下拉框store
var FitOsStore = Ext.create('Ext.data.Store', {
    fields: ['ParameterCode', 'parameterName'],
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/AppService/GetAppMessagePara?paraType=fit_os',
        noCache: false,
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items',
        }
    }
});

//顯示類別store
var DisplayTypeStore = Ext.create('Ext.data.Store', {
    fields: ['parameterCode', 'parameterName'],
    data: [
         { parameterCode: '0', parameterName: NO },//否
         { parameterCode: '1', parameterName: YES },//是
    ]
});

//進行分頁查詢的時候附帶上查詢條件
AppMessageStore.on("beforeload", function () {
    var timestartvalue = Ext.getCmp('msg_start_first').getValue();
    var timestartendvalue = Ext.getCmp('msg_start_second').getValue();
    var timeendstartvalue = Ext.getCmp('msg_end_first').getValue();
    var timeendendvalue = Ext.getCmp('msg_end_second').getValue();
    AppMessageStore.removeAll();
    if (timestartvalue == null && timestartendvalue == null && timeendstartvalue == null && timeendendvalue == null) {
        Ext.Msg.alert(INFORMATION, SEARCHNULLTEXT);
        return false;
    }
    Ext.apply(AppMessageStore.proxy.extraParams, {
        msg_start_first: timestartvalue,
        msg_start_second: timestartendvalue,
        msg_end_first: timeendstartvalue,
        msg_end_second: timeendendvalue
    });
});

//查詢數據
function Query(x) {
    Ext.getCmp("AppMessageList").store.loadPage(1, {
        params: {

        }
    });
}

Ext.onReady(function () {
    //回車鍵查詢
    // edit by zhuoqin0830w  2015/09/22  以兼容火狐瀏覽器
    document.onkeydown = function (event) {
        e = event ? event : (window.event ? window.event : null);
        if (e.keyCode == 13) {
            $("#btnQuery").click();
        }
    };

    var AppMessageList = Ext.create('Ext.grid.Panel', {
        id: 'AppMessageList',
        store: AppMessageStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "", xtype: 'rownumberer', width: 38, align: 'center', sortable: false, menuDisabled: true },
            { header: MESSAGE_ID, dataIndex: 'message_id', width: 40, align: 'center', sortable: false, menuDisabled: true },//序號
            { header: TITLE, dataIndex: 'title', width: 120, align: 'center', sortable: false, menuDisabled: true },//標題
            { header: CONTENT, dataIndex: 'content', width: 200, align: 'center', sortable: false, menuDisabled: true },//內容
            { header: LINKURL, dataIndex: 'linkurl', width: 100, align: 'center', sortable: false, menuDisabled: true },//URL
            { header: MSG_START_TIME, dataIndex: 'msg_start_time', width: 140, align: 'center', sortable: false, menuDisabled: true },//開始時間
            { header: MSG_END_TIME, dataIndex: 'msg_end_time', width: 140, align: 'center', sortable: false, menuDisabled: true },//結束時間
            { header: APPELLATION, dataIndex: 'appellation', width: 100, align: 'center', sortable: false, menuDisabled: true },//稱謂
            { header: FIT_OS, dataIndex: 'fit_os', width: 100, align: 'center', sortable: false, menuDisabled: true },//適用平臺
            { header: MESSAGEDATE_TIME, dataIndex: 'messagedate_time', width: 140, align: 'center', sortable: false, menuDisabled: true },//新增日期
        ],
        dockedItems: [{
            dock: 'top',
            xtype: 'toolbar',
            items: [{    //modify by jiaohe0625j
                xtype: 'datetimefield',
                //format: 'Y-m-d',
                format: 'Y-m-d  H:i:s',
                time: { hour: 00, min: 00, sec: 00 },
                id: 'msg_start_first',
                fieldLabel: MSG_START_TIME,//開始時間
                labelWidth: 60,
                //width: 200,
                width: 220,
                editable: false,
                listeners: {
                    //change: function () {
                    //    Ext.getCmp("msg_start_second").setMinValue(this.getValue());
                    //    Ext.getCmp("msg_end_first").setMinValue(this.getValue());
                    //    Ext.getCmp("msg_end_second").setMinValue(this.getValue());
                    //}
                    select: function (a, b, c) {
                        var start = Ext.getCmp("msg_start_first");
                        var end = Ext.getCmp("msg_start_second");
                        if (start.getValue() > Ext.getCmp("msg_end_first").getValue() && Ext.getCmp("msg_end_first").getValue() != null) {
                            Ext.Msg.alert(INFORMATION, "結束時間不能小於開始時間");
                            start.setValue(setNextMonth(Ext.getCmp("msg_end_first").getValue(), -1));
                        }
                        if (end.getValue() == null) {
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }
                        else if (end.getValue() < start.getValue()) {
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }

                    }
                }
            }, {
                xtype: 'displayfield',
                value: '~ ',
                id: 'blp_start',
                disabled: true,
                margin: '0 5 0 5'
            }, {    //modify by jiaohe0625j
                xtype: 'datetimefield',
                //format: 'Y-m-d',
                format: 'Y-m-d  H:i:s',
                time: { hour: 23, min: 59, sec: 59 },
                id: 'msg_start_second',
                labelWidth: 60,
                editable: false,
                listeners: {
                    //change: function () {
                    //    Ext.getCmp("msg_start_first").setMaxValue(this.getValue());
                    //}
                    select: function (a, b, c) {
                        var start = Ext.getCmp("msg_start_first");
                        var end = Ext.getCmp("msg_start_second");
                        if (start.getValue() == null) {
                            start.setValue(setNextMonth(end.getValue(), -1));
                        }
                        if (end.getValue() < start.getValue()) {
                            Ext.Msg.alert(INFORMATION, "結束時間不能小於開始時間");
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }
                    }
                }
            }, {    //modify by jiaohe0625j
                xtype: 'datetimefield',
                //format: 'Y-m-d',
                format: 'Y-m-d  H:i:s',
                time: { hour: 00, min: 00, sec: 00 },
                id: 'msg_end_first',
                fieldLabel: MSG_END_TIME,
                labelWidth: 60,
                //width: 200,
                width: 220,
                editable: false,
                vtype: 'regxvalid_end',
                listeners: {
                    //change: function () {
                    //    Ext.getCmp("msg_end_second").setMinValue(this.getValue());
                    //}
                    select: function (a, b, c) {
                        var start = Ext.getCmp("msg_end_first");
                        var end = Ext.getCmp("msg_end_second");
                        if (start.getValue() < Ext.getCmp("msg_start_first").getValue()) {
                            Ext.Msg.alert(INFORMATION, "結束時間不能小於開始時間");
                            start.setValue(setNextMonth(Ext.getCmp("msg_start_first").getValue(), 1));
                        }
                        if (end.getValue() == null) {
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }
                        else if (end.getValue() < start.getValue()) {
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }

                    }
                }
            }, {
                xtype: 'displayfield',
                value: '~ ',
                id: 'blp_end',
                disabled: true,
                margin: '0 5 0 5'
            }, {    //modify by jiaohe0625j
                xtype: 'datetimefield',
                //format: 'Y-m-d',
                format: 'Y-m-d  H:i:s',
                time: { hour: 23, min: 59, sec: 59 },
                id: 'msg_end_second',
                labelWidth: 60,
                editable: false,
                listeners: {
                    //change: function () {
                    //    Ext.getCmp("msg_start_first").setMaxValue(this.getValue());
                    //    Ext.getCmp("msg_start_second").setMaxValue(this.getValue());
                    //    Ext.getCmp("msg_end_first").setMaxValue(this.getValue());
                    //}
                    select: function (a, b, c) {
                        var start = Ext.getCmp("msg_end_first");
                        var end = Ext.getCmp("msg_end_second");
                        if (start.getValue() == null) {
                            start.setValue(setNextMonth(end.getValue(), -1));
                        }
                        if (end.getValue() < start.getValue()) {
                            Ext.Msg.alert(INFORMATION, "結束時間不能小於開始時間");
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }
                    }
                }
            }, {
                xtype: 'button',
                text: QUERY,//查詢
                iconCls: 'icon-search',
                id: 'btnQuery',
                handler: Query
            }, {
                xtype: 'button',
                text: RESET,//重置
                iconCls: 'ui-icon ui-icon-reset',
                id: 'btn_reset',
                listeners: {
                    click: function () {
                        Ext.getCmp('msg_start_first').reset();
                        Ext.getCmp('msg_start_second').reset();
                        Ext.getCmp('msg_end_first').reset();
                        Ext.getCmp('msg_end_second').reset();
                    }
                }
            }
            ]
        }],
        tbar: [
            { xtype: 'button', id: 'add', text: INSERT, iconCls: 'ui-icon ui-icon-user-add', handler: onAddClick },//新增
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: AppMessageStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [AppMessageList],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                AppMessageList.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
});

function onAddClick() {
    pcFrm.getForm().reset();
    addPc.show();
    Ext.getCmp('new_display_type').setValue(1);
}
// add by jiaohe0625j
setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    } else if (n >= 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}