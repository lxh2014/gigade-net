
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//站臺管理Model
Ext.define('gigade.Status', {
    extend: 'Ext.data.Model',
    fields: [
                { name: "bank", type: "string" },//銀行帳號
                { name: "entday", type: "string" },//會計日期
                { name: "txtdate", type: "string" },//交易日期
                { name: "sn", type: "string" },//交易序號
                { name: "specific_currency", type: "string" },//幣別
                { name: "paid", type: "int" },//交易金額
                { name: "type", type: "int" },//借貸別
                { name: "outputbank", type: "int" },//轉出銀行
                { name: "pay_type", type: "string" },//作業別
                { name: "e_date", type: "int" },//票繳日期
                { name: "note", type: "string" },//備註
                { name: "vat_number", type: "int" }//統一編號
    ]
});
var OrderHncbStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Status',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetOrderHncbList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
OrderHncbStore.on('beforeload', function () {
    Ext.apply(OrderHncbStore.proxy.extraParams,
        {
            Order_Id: window.parent.GetOrderId()

        });
});

Ext.onReady(function () {
    var StatusGrid = Ext.create('Ext.grid.Panel', {
        id: 'StatusGrid',
        store: OrderHncbStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "銀行帳號", dataIndex: 'bank', width: 150, align: 'center' },
            { header: "會計日期", dataIndex: 'entday', width: 100, align: 'center' },
            { header: "交易日期", dataIndex: 'txtdate', width: 150, align: 'center' },
            { header: "交易序號", dataIndex: 'sn', width: 200, align: 'center' },
            { header: "幣別", dataIndex: 'specific_currency', width: 60, align: 'center' },
            { header: "交易金額", dataIndex: 'paid', width: 60, align: 'center' },
            { header: "借貸別", dataIndex: 'type', width: 60, align: 'center' },
            { header: "轉出銀行", dataIndex: 'outputbank', width: 200, align: 'center' },
            { header: "作業別", dataIndex: 'pay_type', width: 200, align: 'center' },
            { header: "票繳日期", dataIndex: 'e_date', width: 100, align: 'center' },
            { header: "備註", dataIndex: 'note', width: 100, align: 'center' },
            { header: "統一編號", dataIndex: 'vat_number', width: 100, align: 'center' }

        ],

        bbar: Ext.create('Ext.PagingToolbar', {
            store: OrderHncbStore,
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
        items: [StatusGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                StatusGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    OrderHncbStore.load({ params: { start: 0, limit: 25 } });
});
