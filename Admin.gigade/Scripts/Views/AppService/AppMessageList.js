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
    //    autoLoad: true
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
         { parameterCode: '0', parameterName: '否' },
         { parameterCode: '1', parameterName: '是' },
    ]
});
//進行分頁查詢的時候附帶上查詢條件
AppMessageStore.on("beforeload", function () {
    Ext.apply(AppMessageStore.proxy.extraParams, {
        msg_start: Ext.getCmp('msg_start') ? Ext.getCmp('msg_start').getValue() : '',
        msg_end: Ext.getCmp('msg_end') ? Ext.getCmp('msg_end').getValue() : '',
    });
});

function Query(x) {
    AppMessageStore.removeAll();
    Ext.getCmp("AppMessageList").store.loadPage(1, {
        params: {

        }
    });
}

Ext.onReady(function () {
    var AppMessageList = Ext.create('Ext.grid.Panel', {
        id: 'AppMessageList',
        store: AppMessageStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "", xtype: 'rownumberer', width: 38, align: 'center', sortable: false, menuDisabled: true },
            { header: "序號", dataIndex: 'message_id', width: 40, align: 'center', sortable: false, menuDisabled: true },
            { header: "標題", dataIndex: 'title', width: 120, align: 'center', sortable: false, menuDisabled: true },
            { header: "內容", dataIndex: 'content', width: 200, align: 'center', sortable: false, menuDisabled: true },
            { header: "URL", dataIndex: 'linkurl', width: 100, align: 'center', sortable: false, menuDisabled: true },
            { header: "開始時間", dataIndex: 'msg_start_time', width: 140, align: 'center', sortable: false, menuDisabled: true },
            { header: "結束時間", dataIndex: 'msg_end_time', width: 140, align: 'center', sortable: false, menuDisabled: true },
            { header: "稱謂", dataIndex: 'appellation', width: 100, align: 'center', sortable: false, menuDisabled: true },
            { header: "適用平臺", dataIndex: 'fit_os', width: 100, align: 'center', sortable: false, menuDisabled: true },
            { header: "新增日期", dataIndex: 'messagedate_time', width: 140, align: 'center', sortable: false, menuDisabled: true },
        ],
        tbar: [
            { xtype: 'button', id: 'add', text: '新增', iconCls: 'ui-icon ui-icon-user-add', handler: onAddClick },
           '->', {
               xtype: 'datetimefield',
               format: 'Y-m-d H:i:s',
               id: 'msg_start',
               fieldLabel: '開始時間',
               labelWidth: 60,
               editable: false,
               listeners: {
                   change: function () {
                       var select_msg_end = Ext.getCmp('msg_end').getValue()
                       if (select_msg_end != null && this.getValue() > select_msg_end) {
                           Ext.Msg.alert(INFORMATION, '開始時間不能小於結束時間');
                           console.log(select_msg_end);
                           console.log(this.getValue());
                       }
                   }
               }
           }, {
               xtype: 'displayfield',
               value: '~ ',
               id: 'blp',
               disabled: true,
               margin: '0 5 0 5'
           }, {
               xtype: 'datetimefield',
               format: 'Y-m-d H:i:s',
               id: 'msg_end',
               fieldLabel: '結束時間',
               labelWidth: 60,
               editable: false,
               listeners: {
                   change: function () {
                       var select_msg_start = Ext.getCmp('msg_start').getValue()
                       if (select_msg_start != null && this.getValue() < select_msg_start) {
                           Ext.Msg.alert(INFORMATION, '結束時間不能大於開始時間');
                       }
                   }
               }
           }, {
               xtype: 'button',
               text: '查詢',
               iconCls: 'icon-search',
               id: 'btnQuery',
               handler: Query
           }, {
               xtype: 'button',
               text: '重置',
               id: 'btn_reset',
               listeners: {
                   click: function () {
                       Ext.getCmp('msg_start').reset();
                       Ext.getCmp('msg_end').reset();
                   }
               }
           }
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
    AppMessageStore.load({ params: { start: 0, limit: 25 } });
});
function onAddClick() {
    pcFrm.getForm().reset();
    addPc.show();
    Ext.getCmp('new_display_type').setValue(1);
}
var pcFrm = Ext.create('Ext.form.Panel', {
    id: 'pcFrm',
    layout: 'anchor',
    frame: true,
    plain: true,
    border: false,
    bodyStyle: "padding: 12px 12px 6px 12px;",
    url: '/AppService/AppMessageInsert',
    items: [{
        xtype: 'textfield',
        id: 'new_title',
        fieldLabel: '標題',
        width: 300,
        allowBlank: false
    }, {
        xtype: 'textfield',
        id: 'new_linkurl',
        fieldLabel: 'URL',
        width: 300,
        vtype: 'url'
    }, {
        xtype: 'textarea',
        id: 'new_content',
        fieldLabel: '內容',
        margin: '0 0 20 0',
        height: 60,
        width: 300,
        allowBlank: false
    }, {
        xtype: 'datetimefield',
        format: 'Y-m-d H:i:s',
        id: 'new_msg_start',
        fieldLabel: '開始時間',
        width: 300,
        allowBlank: false,
        editable: false
    }, {
        xtype: 'datetimefield',
        format: 'Y-m-d H:i:s',
        id: 'new_msg_end',
        fieldLabel: '結束時間',
        width: 300,
        allowBlank: false,
        editable: false
    }, {
        xtype: 'combobox',
        id: 'new_fit_os',
        store: FitOsStore,
        fieldLabel: '適用平台',
        displayField: 'parameterName',
        width: 300,
        queryMode: 'local',
        allowBlank: false
    }, {
        xtype: 'combobox',
        id: 'new_display_type',
        store: DisplayTypeStore,
        displayField: 'parameterName',
        valueField: 'parameterCode',
        fieldLabel: '顯示類別',
        width: 300,
        editable: false,
        //disabled: true,
        //hidden: true
    }, {
        xtype: 'textfield',
        id: 'new_appellation',
        width: 300,
        fieldLabel: '稱謂',
        allowBlank: false
    }],
    buttons: [{
        text: '保存',
        id: 'btnSave',
        //formBind: true,
        handler: function () {
            var form = this.up('form').getForm();
            if (Ext.getCmp('new_msg_start').getValue() > Ext.getCmp('new_msg_end').getValue()) {
                Ext.Msg.alert("提示信息", "開始時間不能小於結束時間！");
                return;
            }
            if (form.isValid()) {
                form.submit({
                    params: getParams(),
                    success: function (form, action) {
                        var result = Ext.decode(action.response.responseText);
                        if (result.success) {
                            addPc.hide();
                            pcFrm.getForm().reset();
                            AppMessageStore.load();
                            //FitOsStore.load();
                            Ext.Msg.alert(INFORMATION, '新增數據成功');
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, '新增數據失敗');
                    }
                });
            }
        }
    }]
})
var addPc = Ext.create('Ext.window.Window', {
    title: '添加記錄',
    id: 'addPc',
    width: 360,
    height: 400,
    iconCls: 'ui-icon ui-icon-add',
    plain: true,
    border: false,
    modal: true,
    resizable: false,
    draggable: true,
    hidden: true,
    bodyStyle: "padding: 10px 10px 7px 10px;",
    layout: 'fit',
    items: [pcFrm],
    closable: false,
    tools: [{
        type: 'close',
        handler: function (event, toolEl, panel) {
            addPc.hide();
        }
    }]
})
function getParams() {
    var params = new Object();
    params.title = Ext.getCmp('new_title').getValue();
    params.content = Ext.getCmp('new_content').getValue();
    params.linkurl = Ext.getCmp('new_linkurl').getValue();
    params.msg_start = Ext.getCmp('new_msg_start').getValue();
    params.msg_end = Ext.getCmp('new_msg_end').getValue();
    params.appellation = Ext.getCmp('new_appellation').getValue();
    params.fit_os = Ext.getCmp('new_fit_os').getValue();
    params.display_type = Ext.getCmp('new_display_type').getValue();
    return params;
}