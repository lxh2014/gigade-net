/* Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 * 文件名称：OrderExpectList.js 
 * 摘   要： 預購單
 * 当前版本：1.0 
 * 作   者：shuangshuang0420j 
 * 完成日期：2014/10/21 09:10:10
 */
Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
//聲明grid
Ext.define('GIGADE.OrderBrandProduces', {
    extend: 'Ext.data.Model',
    fields: [
              { name: "expect_id", type: "int" }, //預購單號
              { name: "order_id", type: "int" }, //付款單號
              { name: "slave_id", type: "int" }, //廠商出貨單號
              { name: "product_name", type: "string" }, //商品名稱
              { name: "detail_status", type: "string" }, //商品出貨
              { name: "d_status_name", type: "string" },
              { name: "status", type: "string" }, //預購單狀態
              { name: "date_one", type: "string" }, //建立日期
              { name: "date_two", type: "string" }, //建立日期
              { name: "store", type: "string" }, //
              { name: "code", type: "string" }, //
              { name: "stime", type: "string" }, //
              { name: "supdatedate", type: "string" }, //
              { name: "note", type: "string" }, //
              { name: "note_order", type: "string" } //客戶備註  
    ]
});
//獲取grid中的數據
var OrderExpectListStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.OrderBrandProduces',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetOrderExpectList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
            root: 'data'
        }
    }
});

var DTStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有日期", "value": "0" },
        { "txt": "建立日期", "value": "1" }
    ]
});
var statusStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有狀態", "value": "-1" },
        { "txt": "未出貨", "value": "0" },
        { "txt": "已出貨", "value": "1" },
        { "txt": "異常", "value": "2" }
    ]
});
OrderExpectListStore.on('beforeload', function () {
    Ext.apply(OrderExpectListStore.proxy.extraParams, {
        seledate:Ext.getCmp('seldate').getValue(),
        dateOne: Ext.getCmp('dateOne').getValue(),
        dateTwo: Ext.getCmp('dateTwo').getValue(),
        status: Ext.htmlEncode(Ext.getCmp("status").getValue())//商品出貨
    })
});

Ext.onReady(function () {
    var OrderExpectGrid = Ext.create('Ext.grid.Panel', {
        id: 'OrderExpectGrid',
        store: OrderExpectListStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "預購單號", dataIndex: 'expect_id', align: 'center', width: 60 },
            {
                header: '付款單號', colName: 'order_id', xtype: 'templatecolumn', width: 80,
                tpl: Ext.create('Ext.XTemplate',
                    '<tpl >',
                        '<a href=javascript:TranToDetial("{order_id}") >{order_id}</a>',
                    '</tpl>'),
                align: 'center', sortable: false, menuDisabled: true
            },
            { header: "廠商出貨單號", dataIndex: 'slave_id', align: 'center', width: 90 },
            { header: "商品名稱", dataIndex: 'product_name', width: 280, align: 'center' },
            {
                header: "商品出貨", dataIndex: 'd_status_name', align: 'center', width: 80,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (value) {
                        case "已出貨": return "包裝完成";
                            break;
                        case "出貨中": return "包裝中";
                            break;
                        default: return value;
                            break;
                    }
                }
            },
            {
                header: "預購單狀態", dataIndex: 'status', align: 'center', width: 80,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (value) {
                        case "0": return "未出貨";
                            break;
                        case "1": return "已出貨";
                            break;
                        case "2": return "異常";
                            break;
                    }
                }
            },
            { header: "建立時間", dataIndex: 'date_one', width: 120, align: 'center' },
            {
                header: "客戶備註", dataIndex: 'note', width: 200, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value.length==0) {
                        return record.data.note_order;
                    }
                }
            },
            {
                header: "編輯", dataIndex: '', width: 80, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.status == "0") {
                        return "<a href='javascript:void(0);'  onclick='onEditClick()'>出貨</a>";
                    }
                    else if (record.data.status == "1") {
                        return " ";
                    }
                    else if (record.data.status == "2") {
                        return " ";
                    }
                }
            },
            {
                header: "歷史資料", dataIndex: '', width: 80, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.status == "0") {
                        return " ";
                    }
                    else if (record.data.status == "1") {
                        return "<a href='javascript:void(0);'  onclick='onShowClick()'>瀏覽</a>";
                    }
                    else if (record.data.status == "2") {
                        return "<a href='javascript:void(0);'  onclick='onShowClick()'>瀏覽</a>";
                    }
                }
            }
        ],
        tbar: [
             {
                 xtype: 'button',
                 text: '匯出',
                 iconCls: 'icon-excel',
                 id: 'btnExcel',
                 handler: Export
             },
            '->',
            {
                xtype: 'combobox',
                id: 'seldate',
                editable: false,
                fieldLabel: "日期條件",
                labelWidth: 60,
                store: DTStore,
                displayField: 'txt',
                valueField: 'value',
                margin: '0 0 0 10',
                value: 0
            },
            {
                xtype: "datefield",
                labelWidth: 60,
                margin: '0 0 0 10',
                id: 'dateOne',
                editable: false,
                name: 'dateOne',
                format: 'Y-m-d',
                allowBlank: false,
                submitValue: true,
                value: new Date(Tomorrow().setMonth(Tomorrow().getMonth() - 1)),
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("dateOne");
                        var end = Ext.getCmp("dateTwo");
                        var s_date = new Date(end.getValue());
                        if (end.getValue() < start.getValue()) {
                            Ext.Msg.alert("提示", "開始時間不能大於結束時間！");
                            start.setValue(new Date(s_date.setMonth(s_date.getMonth() - 1)));
                        }
                    }
                }
            },
            {
                xtype: 'displayfield',
                value: "~"
            },
            {
                xtype: "datefield",
                format: 'Y-m-d',
                id: 'dateTwo',
                name: 'dateTwo',
                value: Tomorrow(),
                allowBlank: false,
                editable: false,
                submitValue: true,
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("dateOne");
                        var end = Ext.getCmp("dateTwo");
                        var s_date = new Date(start.getValue());
                        if (end.getValue() < start.getValue()) {
                            Ext.Msg.alert("提示", "開始時間不能大於結束時間！");
                            end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                        }
                    }
                }
            },
            { xtype: 'combobox', editable: false, fieldLabel: '付款單狀態', labelWidth: 70, id: 'status', store: statusStore, displayField: 'txt', valueField: 'value', value: '-1' },
            {
                text: SEARCH,
                iconCls: 'icon-search',
                id: 'btnQuery',
                handler: Query
            }
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
            store: OrderExpectListStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        })
    });
    Ext.create('Ext.Viewport', {
        layout: 'fit',
        items: [OrderExpectGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                OrderExpectGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //OrderExpectListStore.load({ params: { start: 0, limit: pageSize } });
})
onEditClick = function () {
    var row = Ext.getCmp("OrderExpectGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], OrderExpectListStore);
    }
}

onShowClick = function () {
    var row = Ext.getCmp("OrderExpectGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        showFunction(row[0], OrderExpectListStore);
    }
}


/************匯入到Excel**ATM************/
function Export() {
    window.open("/OrderManage/ExportToExcel?status=" + Ext.getCmp('status').getValue() + "&seledate="+Ext.getCmp('seldate').getValue() + "&dateOne=" + Ext.Date.format(new Date(Ext.getCmp('dateOne').getValue()), 'Y-m-d H:i:s') + "&dateTwo=" + Ext.Date.format(new Date(Ext.getCmp('dateTwo').getValue()), 'Y-m-d H:i:s'));
}
//查询
Query = function () {
    OrderExpectListStore.removeAll();
    Ext.getCmp("OrderExpectGrid").store.loadPage(1, {
        params: {
            dateOne: Ext.getCmp('dateOne').getValue(),
            dateTwo: Ext.getCmp('dateTwo').getValue(),
            status: Ext.htmlEncode(Ext.getCmp("status").getValue())//商品出貨
        }
    });
}


function TranToDetial(orderId) {
    var url = '/OrderManage/OrderDetialList?Order_Id=' + orderId;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#detial');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'detial',
        title: '訂單內容',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}