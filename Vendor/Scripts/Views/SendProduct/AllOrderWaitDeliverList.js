Ext.Loader.setConfig({ enabled: true });

Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 20;
//聲明grid
Ext.define('GIGADE.VendorList', {
    extend: 'Ext.data.Model',
    fields: [
         { name: "Slave_Id", type: "int" },
         { name: "code", type: "string" },//批次單號
         { name: "order_date_pay", type: "string" },//批次出貨單編號
         { name: "order_payment", type: "string" },
         { name: "order_id", type: "string" },//付款單號
         { name: "order_name", type: "string" },//訂購姓名
         { name: "delivery_name", type: "string" },//收貨姓名
         { name: "status", type: "string" },//訂單狀態
         { name: "pay_time", type: "string" },//轉單日期
         {name:"order_createdate",type:"string"}
    ]
});
//獲取grid中的數據
var VendorListStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.VendorList',
    proxy: {
        type: 'ajax',
        url: '/SendProduct/GetDeliverList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
            root: 'data'
        }
    }
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("VendorListGrid").down('#Export1').setDisabled(selections.length == 0)
            Ext.getCmp("VendorListGrid").down('#Export2').setDisabled(selections.length == 0)
            //Ext.getCmp("VendorListGrid").down('#Export3').setDisabled(selections.length == 0)
            var model = Ext.getCmp("VendorListGrid").getSelectionModel();
            var row = Ext.getCmp("VendorListGrid").getSelectionModel().getSelection();
            if (row.length > 1) {
                for (i = 0; i < row.length; i++) {
                    var start = row[0].data.delivery_store;
                    //if (start != row[i + 1].data.delivery_store) {
                    //    model.deselectAll();
                    //}
                }
            }
        }
    }
});

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 70,
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        //  html: '<div class="capion">傳票明細&nbsp;&nbsp;' + document.getElementById("hid_ticket_id").value + '</div>',
                        html: '<div class="capion">待出貨訂單(貨品請寄吉甲地)</div>',
                        frame: false,
                        border: false
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'textfield',
                        id: 'search',
                        fieldLabel:"付款單號",
                        colName: 'search',
                        margin: '0 5px',
                        //hidden: true,
                        submitValue: false,
                        name: 'productid'
                    },
                    {
                        xtype: 'button',
                        iconCls: 'icon-search',
                        margin: '0 5px',
                        text: "查詢",
                        handler: Query
                    },
                    {
                        xtype: 'button',
                        text: '重置',
                        id: 'btn_reset',
                        iconCls: 'ui-icon ui-icon-reset',
                        listeners: {
                            click: function () {
                                Ext.getCmp("search").setValue("");
                            }
                        }
                    }
                ]
            }
        ]
    });

    //頁面加載時創建grid
    var VendorListGrid = Ext.create('Ext.grid.Panel', {
        id: 'VendorListGrid',
        store: VendorListStore,
        flex: 8.8,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "批次出貨單編號", dataIndex: 'order_createdate', width: 100, align: 'center', hidden: true },//
            {
                header: "批次單號", dataIndex: 'code', width: 150, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return value;
                }
            },
            { header: "付款單號", dataIndex: 'order_id', width: 150, align: 'center' },
            { header: "訂購姓名", dataIndex: 'order_name', width: 150, align: 'center' },
            { header: "收貨姓名", dataIndex: 'delivery_name', width: 150, align: 'center' },
            { header: "訂單狀態", dataIndex: 'status', width: 150, align: 'center' },
            { header: "轉單日期", dataIndex: 'pay_time', width: 150, align: 'center' }
            //{ header: "檢視", xtype: 'templatecolumn', width: 80, tpl: channelTpl2, align: 'center' },
        ],
        tbar: [
            { xtype: 'button', id: 'Export1', text: '出貨', disabled: true, handler: shipment },
            { xtype: 'button', id: 'Export2', text: '批次檢貨明細列印', disabled: true, handler: BatchPicking }
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
            store: VendorListStore,
            pageSize: pageSize,
            displayInfo: true
            //displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            // emptyMsg: NOTHING_DISPLAY
        }),
        selModel: sm
    });

    Ext.create('Ext.Viewport', {
        layout: 'vbox',
        items: [frm, VendorListGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                VendorListGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });

    ToolAuthority();
    VendorListStore.load({ params: { start: 0, limit: 20 } });
});
function Tomorrow() {
    var d;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                     // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += d.getDate() + 1;                          // 获取日。
    return (new Date(s));                                 // 返回日期。
}

onAddClick = function () {
    editFunction(null, VendorListStore);
}

onEditClick = function () {
    var row = Ext.getCmp("VendorListGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], VendorListStore);
    }
}

//查询
Query = function () {
    VendorListStore.removeAll();
    Ext.getCmp("VendorListGrid").store.loadPage(1, {
        params: {
            search: Ext.getCmp('search').getValue()//搜索內容--deliver_id or delivery_code or delivery_name,delivery_mobile,vendor_name_simple 
        }
    });
}
function BatchPicking() {
    var row = Ext.getCmp("VendorListGrid").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        var rowIDs = '';
        for (var i = 0; i < row.length; i++) {
            rowIDs += row[i].data.Slave_Id + ',';
        }
    }
    var urlTran = '/SendProduct/PickingPrintView?rowIDs=' + rowIDs;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#detial');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'detial',
        title: '檢貨明細',
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}

function shipment() {//出貨
    var row = Ext.getCmp("VendorListGrid").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        var rowIDs = '';
        var edata = [];
        for (var i = 0; i < row.length; i++) {
            var s = row[i].data.order_createdate;
            edata.push(s);
            rowIDs += row[i].data.Slave_Id + ',';
        }
        for (var i = 0; i < edata.length - 1; i++) {
            if (edata[i] != edata[i + 1]) {
                Ext.Msg.alert("提示", "请选择相同日期出货单");
                return;
            }
        }
        var times = edata[0];
        var urlTran = '/SendProduct/AllOrderDeliverView?rowIDs=' + rowIDs + '&times=' + times;
        var panel = window.parent.parent.Ext.getCmp('ContentPanel');
        var copy = panel.down('#detial');
        if (copy) {
            copy.close();
        }
        copy = panel.add({
            id: 'detial',
            title: '批次出貨',
            html: window.top.rtnFrame(urlTran),
            closable: true
        });
        panel.setActiveTab(copy);
        panel.doLayout();
    }
}