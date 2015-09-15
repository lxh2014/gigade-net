/*
* 文件名稱 :AppversionsList.js
* 文件功能描述 :上架版本列表JS
* 版權宣告 :
* 開發人員 : 肖國棟
* 版本資訊 : 1.0
* 日期 : 2015.8.24
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/


var pageSize = 25;
/*************站臺管理主頁面開始*************/
//上架版本Model
Ext.define('gigade.AppVersions', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "id", type: "int" },//序號
        { name: "versions_id", type: "int" },//版本ID
        { name: "versions_code", type: "int" },//版本code
        { name: "versions_name", type: "string" },//版本名稱
        { name: "versions_desc", type: "string" },//版本描述
        { name: "drive", type: "int" },//平台
        { name: "release_type", type: "int" },//此字段暫無使用
        { name: "releasedateQuery", type: "string" }//上架日
    ]
});
//上架版本數據源
var AppVersionsStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.AppVersions',
    proxy: {
        type: 'ajax',
        url: '/AppService/GetAppversionsInfo',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
//數據源加載前事件
AppVersionsStore.on('beforeload', function () {
    AppVersionsStore.removeAll();
    //驗證查詢條件
    var cmbdrivervalue = Ext.getCmp('cmbdriver').getValue();
    if (cmbdrivervalue == null) {
        Ext.Msg.alert(INFORMATION, SEARCHNULLTEXT);
        return false;
    }
    Ext.apply(AppVersionsStore.proxy.extraParams,
        {
            cmbdriver: cmbdrivervalue
        });
});
//創建多選
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("AppVersionsList").down('#DeleteBtn').setDisabled(selections.length == 0);
        }
    }
});
//查詢事件
function Query(x) {
    AppVersionsStore.removeAll();
    //驗證查詢條件
    var cmbdrivervalue = Ext.getCmp('cmbdriver').getValue();
    if (cmbdrivervalue == null) {
        Ext.Msg.alert(INFORMATION, SEARCHNULLTEXT);
        return false;
    }
    Ext.getCmp("AppVersionsList").store.loadPage(1, {
        params: {
            cmbdriver: cmbdrivervalue
        }
    });
}
//平台數據模型
var driverModel = Ext.create('Ext.data.Store', {
    fields: [{ type: 'int', name: 'drivervalue' },
                { type: 'string', name: 'drivername' }
    ],
    data: [
        { "drivervalue": "0", "drivername": "ios" },
        { "drivervalue": "1", "drivername": "android" }
    ]
});
//平臺數據下拉列表框
var SelectDriver = Ext.create('Ext.form.ComboBox', {
    fieldLabel: DRIVERTEXT,
    store: driverModel,
    id: "cmbdriver",
    queryMode: 'local',
    margin: "0 5 0 0",
    displayField: 'drivername',
    valueField: 'drivervalue',
    triggerAction: 'all',
    selectOnFocus: true,
    forceSelection: true,
    editable: true,
    emptyText: SHULDCHECK + DRIVERTEXT,
    blankText: SHULDCHECK + DRIVERTEXT,
    labelWidth: 35,
    width: 150
});


Ext.onReady(function () {
    //回撤鍵查詢
    document.body.onkeydown = function () {
        if (event.keyCode == 13) {
            $("#btnQuery").click();
        }
    };
    //建立列表
    var GShow = Ext.create('Ext.grid.Panel', {
        id: 'AppVersionsList',
        store: AppVersionsStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        selModel: sm,
        columns: [
            { header: RID, dataIndex: 'id', width: 100, align: 'center', hidden: true, menuDisabled: true, sortable: false },
            { text: XID, xtype: 'rownumberer', width: 40, align: 'center', menuDisabled: true, sortable: false },
            { header: VERSIONS_ID, dataIndex: 'versions_id', width: 100, align: 'center', menuDisabled: true, sortable: false },
            { header: VERSIONS_CODE, dataIndex: 'versions_code', width: 100, align: 'center', menuDisabled: true, sortable: false },
            { header: VERSIONS_NAME, dataIndex: 'versions_name', width: 200, align: 'center', menuDisabled: true, sortable: false },
            { header: VERSIONS_DESC, dataIndex: 'versions_desc', width: 200, align: 'center', menuDisabled: true, sortable: false },
            {
                header: DRIVERTEXT, dataIndex: 'drive', width: 100, align: 'center', menuDisabled: true, sortable: false,
                renderer: function (value) {
                    if (value == 0) {
                        return "ios";
                    }
                    else if (value == 1) {
                        return "android";
                    }
                }
            },
            {
                header: RELEASE_DATE, dataIndex: 'releasedateQuery', width: 100, align: 'center', menuDisabled: true, sortable: false,
                renderer: function (val) { return val == '1970-01-01' ? "" : val; }
            }
        ],
        dockedItems: [{
            dock: 'top',
            xtype: 'toolbar',
            items: [
                SelectDriver,
                 {
                     xtype: 'button',
                     text: SEARCHBTN,
                     iconCls: 'ui-icon ui-icon-search-2',
                     id: 'btnQuery',
                     handler: Query
                 },
                 {
                     xtype: 'button',
                     text: REPEATBTN,
                     id: 'btn_reset',
                     iconCls: 'ui-icon ui-icon-reset',
                     listeners: {
                         click: function () {
                             Ext.getCmp("cmbdriver").reset();
                         }
                     }
                 }
            ]
        }],
        tbar: [
            {
                xtype: 'button',
                labelWidth: 50,
                text: ADDBTN,
                iconCls: 'ui-icon ui-icon-user-add',
                xtype: 'button',
                handler: btnAdd
            }, {
                xtype: 'button',
                labelWidth: 50,
                id: 'DeleteBtn',
                text: DELETEBTN,
                disabled: true,
                iconCls: 'ui-icon ui-icon-user-delete',
                xtype: 'button',
                handler: btnDelete
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: AppVersionsStore,
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
        }
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [GShow],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                GShow.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
});
/*************站臺管理主頁面結束*************/

/****************刪除方法開始****************/
function btnDelete() {
    var row = Ext.getCmp("AppVersionsList").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.id + ',';
                }
                Ext.Ajax.request({
                    url: '/AppService/AppversionsDelete',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            AppVersionsStore.load();
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }
        });
    }
}
/****************刪除方法結束****************/
//添加信息
function btnAdd() {
    SaveReport(null);
}


