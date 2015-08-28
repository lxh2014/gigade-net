
var CallidForm;
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//站臺管理Model
Ext.define('gigade.Cancel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "cancel_id", type: "int" },
        { name: "cancle_status_string", type: "string" },
        { name: "cancel_createdate", type: "string" },
        { name: "cancel_note", type: "string" },
        { name: "item_id", type: "int" },
        { name: "product_name", type: "string" },
        { name: "product_freight_set_string", type: "string" },
        { name: "buy_num", type: "int" },
        { name: "single_money", type: "int" },
        { name: "subtotal", type: "int" },
        { name: "deduct_bonus", type: "int" }
    ]
});

var CancelStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Cancel',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetCancel',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
CancelStore.on('beforeload', function () {
    Ext.apply(CancelStore.proxy.extraParams,
        {
            Order_Id: window.parent.GetOrderId()

        });
});
Ext.onReady(function () {
    var CancelGrid = Ext.create('Ext.grid.Panel', {
        id: 'CancelGrid',
        store: CancelStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "取消單號", dataIndex: 'cancel_id', width: 200, align: 'center' },
            { header: "取消單狀態", dataIndex: 'cancle_status_string', width: 200, align: 'center' },
            { header: "取消單建立時間", dataIndex: 'cancel_createdate', width: 100, align: 'center' },
            { header: "備註", dataIndex: 'cancel_note', width: 100, align: 'center' },
            { header: "商品編號", dataIndex: 'item_id', width: 100, align: 'center' },
            { header: "商品名稱", dataIndex: 'product_name', width: 200, align: 'center' },
            { header: "託運單屬性", dataIndex: 'product_freight_set_string', width: 150, align: 'center' },
            { header: "數量", dataIndex: 'buy_num', width: 100, align: 'center' },
            {
                header: "折扣價", dataIndex: 'single_money', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return change(value);
                }
            },
            {
                header: "小計", dataIndex: 'subtotal', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return change(value);
                }
            },
            { header: "購物金", dataIndex: 'deduct_bonus', width: 100, align: 'center' }
        ],

        bbar: Ext.create('Ext.PagingToolbar', {
            store: CancelStore,
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
        items: [CancelGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                CancelGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    CancelStore.load({ params: { start: 0, limit: 25 } });
});





