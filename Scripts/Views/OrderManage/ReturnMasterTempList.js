Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
//聲明grid
Ext.define('GIGADE.OrderTempReturn', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "order_id", type: "int" }, //付款單號
        { name: "detail_id", type: "int" }, //購物單號
        { name: "temp_status", type: "string" }, //狀態
        { name: "user_return_createdates", type: "string" }, //建立時間
        { name: "user_note", type: "string" }, //備註
        { name: "user_return_id", type: "int" }, //
        { name: "return_reason", type: "string" }, //
        { name: "user_return_updatedates", type: "string" },//
        { name: "slave_status", type: "string" },
        { name: "return_id", type: "int" },
        { name: "item_vendor_id", type: "int" },
        { name: "return_zip", type: "string" },
        { name: "return_address", type: "string" }
    ]
});
//獲取grid中的數據
var OrderTempReturnListStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.OrderTempReturn',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetReturnMasterList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
            root: 'data'
        }
    }
});
//多選框
var sm = Ext.create('Ext.selection.CheckboxModel', {
    showHeaderCheckbox: true,
    renderer: function (value, metaData, record, rowIndex, colIndex, store, view) {
        if (record.data.temp_status == "0") {
            metaData.tdCls = Ext.baseCSSPrefix + 'grid-cell-special';
            return '<div class="' + Ext.baseCSSPrefix + 'grid-row-checker">&#160;</div>';
        }
        else {
            return '';
        }
    },
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("ReturnMasterTempListGrid").down('#Edit').setDisabled(selections.length == 0);
        }
    }
});
var searchStatusrStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有資料", "value": "0" },
        { "txt": "購物單號", "value": "1" }
    ]
});
var dateoneStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有日期", "value": "0" },
        { "txt": "建立日期", "value": "1" }
    ]
});
OrderTempReturnListStore.on('beforeload', function () {
    Ext.apply(OrderTempReturnListStore.proxy.extraParams, {
        selecttype: Ext.getCmp('select_type').getValue(), //選擇查詢種類
        searchcon: Ext.getCmp('search_con').getValue(),
        seldate: Ext.getCmp('seldate').getValue(),
        timestart: Ext.getCmp('timestart').getValue(),
        timeend: Ext.getCmp('timeend').getValue(),
        temp_status: Ext.htmlEncode(Ext.getCmp("temp_status").getValue().Temp_Status)//商品出貨
    })

});
Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        height: 100,
        border: 0,
        defaults: { anchor: '95%', msgTarget: "side" },
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                margin: '5 0 10 5',
                items: [
                {
                    xtype: 'combobox',
                    allowBlank: true,
                    hidden: false,
                    id: 'select_type',
                    name: 'select_type',
                    fieldLabel: "查詢條件",
                    store: searchStatusrStore,
                    queryMode: 'local',
                    width: 200,
                    labelWidth: 80,
                    margin: '0 10 0 0',
                    displayField: 'txt',
                    valueField: 'value',
                    typeAhead: true,
                    forceSelection: false,
                    editable: false,
                    value: 0
                },
                {
                    xtype: 'textfield',
                    fieldLabel: "查詢內容",
                    width: 200,
                    labelWidth: 80,
                    margin: '0 10 0 0',
                    id: 'search_con',
                    name: 'search_con'
                },
                {
                    xtype: 'combobox',
                    id: 'seldate',
                    name: 'seldate',
                    fieldLabel: "日期條件",
                    width: 200,
                    store: dateoneStore,
                    displayField: 'txt',
                    valueField: 'value',
                    labelWidth: 80,
                    margin: '0 10 0 0',
                    value: 0

                },
                {
                    xtype: "datetimefield",
                    labelWidth: 60,
                    margin: '0 0 0 0',
                    id: 'timestart',
                    name: 'timestart',
                    format: 'Y-m-d',
                    allowBlank: false,
                    submitValue: true,
                    value: '2014-02-01'
                },
                {
                    xtype: 'displayfield',
                    margin: '0 0 0 0',
                    value: "~"
                },
                {
                    xtype: "datetimefield",
                    format: 'Y-m-d',
                    id: 'timeend',
                    name: 'timeend',
                    margin: '0 0 0 0',
                    allowBlank: false,
                    submitValue: true,
                    value: Tomorrow()
                }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                margin: '5 0 10 5',
                items: [
                {
                    fieldLabel: '付款單狀態',
                    xtype: 'radiogroup',
                    id: 'temp_status',
                    name: 'temp_status',
                    colName: 'temp_status',
                    width: 600,
                    defaults: {
                        name: 'Temp_Status'
                    },
                    columns: 4,
                    items: [
                    { id: 'id1', boxLabel: "所有狀態", inputValue: '3', checked: true },
                    { id: 'id2', boxLabel: "未歸檔", inputValue: '0' },
                    { id: 'id3', boxLabel: "歸檔", inputValue: '1' },
                    { id: 'id4', boxLabel: "作廢", inputValue: '2' }
                    ]                    
                }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                fieldLabel: '',
                layout: 'hbox',
                items: [
                {
                    xtype: 'button',
                    text: '送出',
                    iconCls: 'icon-search',
                    margin: '0 0 10 5',
                    id: 'btnQuery',
                    handler: Query
                }]                
            }
        ]
    });
    //頁面加載時創建grid
    var ReturnMasterTempListGrid = Ext.create('Ext.grid.Panel', {
        id: 'ReturnMasterTempListGrid',
        height: 669,
        store: OrderTempReturnListStore,
        columnLines: true,
        frame: true,
        columns: [
            {
                header: "付款單號", dataIndex: 'order_id', width: 120, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                {
                    if (value != null) {
                        return Ext.String.format("<a id='fkdh" + record.data.order_id + "' href='{0}' target='_blank' style='text-decoration:none'>{1}</a>", 'www.baidu.com', record.data.order_id);
                    }
                }
            },
            { header: "購物單號", dataIndex: 'detail_id', width: 120, align: 'center' },
            {
                header: "狀態", dataIndex: 'temp_status', width: 120, align: 'center', renderer: function (value) {
                    if (value == "0") {
                        return "<font color=red>待歸檔</font>";
                    }
                    else if (value == "1") {
                        return Ext.String.format('歸檔');
                    }
                    else if (value == "2") {
                        return Ext.String.format('作廢');
                    }
                }
            },
            { header: "建立時間", dataIndex: 'user_return_createdates', width: 120, align: 'center' },
            { header: "備註", dataIndex: 'user_note', width: 800, align: 'center' }
        ],
        tbar: [
            { xtype: 'button', id: 'Add', text: ADD, iconCls: 'icon-add', hidden: true, handler: onAddClick },
            { xtype: 'button', id: 'Edit', text: EDIT, iconCls: 'icon-edit', disabled: false, handler: onEditClick }
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        bbar: Ext.create('Ext.PagingToolbar', {
            store: OrderTempReturnListStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        selModel: sm
    });

    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [frm, ReturnMasterTempListGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                ReturnMasterTempListGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    OrderTempReturnListStore.load({ params: { start: 0, limit: 25 } });
})
//查询
Query = function () {
    OrderTempReturnListStore.removeAll();
    Ext.getCmp("ReturnMasterTempListGrid").store.loadPage(1, {
        params: {
            selecttype: Ext.getCmp('select_type').getValue(), //選擇查詢種類
            searchcon: Ext.getCmp('search_con').getValue(),
            seldate: Ext.getCmp('seldate').getValue(),
            timestart: Ext.getCmp('timestart').getValue(),
            timeend: Ext.getCmp('timeend').getValue(),
            temp_status: Ext.htmlEncode(Ext.getCmp("temp_status").getValue().Temp_Status)//商品出貨
        }
    });
}
onAddClick = function () {
    editFunction(null, OrderTempReturnListStore);
}
onEditClick = function () {
    var row = Ext.getCmp("ReturnMasterTempListGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        //Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        //Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], OrderTempReturnListStore);
    }
}
//onShowClick = function () {
//    var row = Ext.getCmp("ReturnMasterTempListGrid").getSelectionModel().getSelection();
//    if (row.length == 0) {
//        Ext.Msg.alert(INFORMATION, NO_SELECTION);
//    }
//    else if (row.length > 1) {
//        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
//    } else if (row.length == 1) {
//        showFunction(row[0], OrderTempReturnListStore);
//    }
//}
/************匯入到Excel**ATM************/
//function Export() {
//    Ext.Ajax.request({
//        url: "/OrderExpectList/Export",
//        success: function (response) {
//            if (response.responseText == "true") {
//                window.location = '../../ImportUserIOExcel/order_expect_delivery.csv';
//            }
//        }
//    });
//}

