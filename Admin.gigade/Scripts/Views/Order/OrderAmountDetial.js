﻿pageSize = 18;
Ext.define('gigade.OrderDetial', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'order_id', type: 'int' },
    { name: 'user_name', type: 'string' },
    { name: 'delivery_name', type: 'string' },
    { name: 'order_product_subtotal', type: 'int' },
    { name: 'order_amount', type: 'int' },
    { name: 'amount', type: 'int' },
    { name: 'order_payment', type: 'string' },
    { name: 'slave_status', type: 'string' },
    { name: 'order_createdate', type: 'string' },
    { name: 'site_name', type: 'string' }
    ]
});
var AmountDetial = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.OrderDetial',
    proxy: {
        type: 'ajax',
        url: '/Order/GetAmountDetial',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
AmountDetial.on('beforeload', function () {
    Ext.apply(AmountDetial.proxy.extraParams,
    {
        category_id: document.getElementById('category_id').value,
        category_status: document.getElementById('category_status').value,
        date_stauts: document.getElementById('date_stauts').value,
        date_start: document.getElementById('date_start').value,
        date_end: document.getElementById('date_end').value
    });
});

Ext.onReady(function () {
    var Frm = Ext.create('Ext.form.Panel', {
        id: 'Frm',
        bodyPadding: '15',
        border: 0,
        width: document.documentElement.clientWidth,
        items: [
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: 'displayfield',
                id: 'category_id',
                fieldLabel: '類別編號',
                value: document.getElementById('category_id').value
            }
            ]
        },
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: 'displayfield',
                id: 'category_name',
                fieldLabel: '類別名稱',
                width: 400,
                value: document.getElementById('category_name').value
            }
            ]
        }, {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: 'displayfield',
                id: 'amount',
                fieldLabel: '類別總金額',
                width: 300,
                value: change(document.getElementById('category_amount').value)
            }
            ]
        }
        ]
    });
    var gdList = Ext.create('Ext.grid.Panel', {
        id: 'gdList',
        store: AmountDetial,
        columnLines: true,
        flex: 8.5,
        frame: true,
        columns: [
        { header: "付款單號", dataIndex: 'order_id', flex: 1, align: 'center' },
        { header: "訂購人", dataIndex: 'user_name', flex: 1, align: 'center' },
        { header: "收貨人", dataIndex: 'delivery_name', flex: 1, align: 'center' },
        {
            header: "訂單應收金額", dataIndex: 'order_product_subtotal', flex: 1, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                return change(value);
            }
        },
        {
            header: "訂單實收金額", dataIndex: 'order_amount', flex: 1, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                return change(value);
            }
        },
        {
            header: "類別訂單金額", dataIndex: 'amount', flex: 1, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                return change(value);
            }
        },
        { header: "付款方式", dataIndex: 'order_payment', flex: 2, align: 'center' },
        { header: "訂單狀態", dataIndex: 'slave_status', flex: 1, align: 'center' },
        { header: "訂單日期", dataIndex: 'order_createdate', flex: 1, align: 'center' },
        { header: "賣場", dataIndex: 'site_name', flex: 1, align: 'center', }
        ],
        tbar: [
        { xtype: 'button', id: 'category_detialExport', text: '類別訂單明細匯出', icon: '../../../Content/img/icons/excel.gif', handler: OnCategoryDetialExport },
        { xtype: 'button', id: 'order_detialExport', text: '訂單明細匯出', icon: '../../../Content/img/icons/excel.gif', handler: OnOrderDetialExport }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: AmountDetial,
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
        layout: 'vbox',
        items: [Frm, gdList],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdList.width = document.documentElement.clientWidth;
                this.doLayout();
                AmountDetial.load();
            }
        }
    });
})

OnCategoryDetialExport = function () {
    Ext.MessageBox.show({
        msg: 'Loading....',
        wait: true
    });
    Ext.Ajax.request({
        url: '/Order/CategoryDetialExport',
        timeout: 900000,
        params: {
            category_id: document.getElementById('category_id').value,
            category_name: document.getElementById('category_name').value,
            category_status: document.getElementById('category_status').value,
            date_stauts: document.getElementById('date_stauts').value,
            date_start: document.getElementById('date_start').value,
            date_end: document.getElementById('date_end').value
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                window.location = '../../ImportUserIOExcel/' + result.ExcelName;
                Ext.MessageBox.hide();
            } else {
                Ext.MessageBox.hide();
                Ext.Msg.alert(INFORMATION, "匯出失敗或沒有數據");
            }
        }
    });
}
OnOrderDetialExport = function () {
    Ext.MessageBox.show({
        msg: 'Loading....',
        wait: true
    });
    Ext.Ajax.request({
        url: '/Order/OrderDetialExport',
        timeout: 900000,
        params: {
            category_id: document.getElementById('category_id').value,
            category_status: document.getElementById('category_status').value,
            date_stauts: document.getElementById('date_stauts').value,
            date_start: document.getElementById('date_start').value,
            date_end: document.getElementById('date_end').value
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                window.location = '../../ImportUserIOExcel/' + result.ExcelName;
                Ext.MessageBox.hide();
            } else {
                Ext.MessageBox.hide();
                Ext.Msg.alert(INFORMATION, "匯出失敗或沒有數據");
            }
        }
    });
}

function change(value) {
    value = value.toString();
    if (/^\d+$/.test(value)) {
        value = value.replace(/^(\d+)(\d{3})$/, "$1,$2");
    }
    return value;
}

