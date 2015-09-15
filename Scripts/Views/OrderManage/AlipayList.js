
var CallidForm;
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//站臺管理Model
Ext.define('gigade.Status', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "alipay_id", type: "int" },
        { name: "merchantnumber", type: "int" },
        { name: "timepaid", type: "int" },
        { name: "writeoffnumber", type: "string" },
        { name: "serialnumber", type: "string" },
        { name: "tel", type: "string" },
        { name: "amount", type: "string" },
        { name: "hash", type: "string" }
    ]
});

var AlipayStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Status',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetAlipayList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
AlipayStore.on('beforeload', function () {
    Ext.apply(AlipayStore.proxy.extraParams,
        {
            Order_Id: window.parent.GetOrderId()

        });
});
Ext.onReady(function () {
    var StatusGrid = Ext.create('Ext.grid.Panel', {
        id: 'StatusGrid',
        store: AlipayStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "流水號", dataIndex: 'alipay_id', width: 60, align: 'center' },
            { header: "商店代號", dataIndex: 'merchantnumber', width: 200, align: 'center' },
            { header: "付款日期", dataIndex: 'timepaid', width: 100, align: 'center' },
            { header: "銷帳編號", dataIndex: 'writeoffnumber', width: 100, align: 'center' },
            { header: "交易序號", dataIndex: 'serialnumber', width: 200, align: 'center' },
            { header: "繳款金額", dataIndex: 'amount', width: 100, align: 'center' },
            { header: "電話", dataIndex: 'tel', width: 100, align: 'center' },
            { header: "加密", dataIndex: 'hash', width: 100, align: 'center' }
        ],

        bbar: Ext.create('Ext.PagingToolbar', {
            store: AlipayStore,
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
    AlipayStore.load({ params: { start: 0, limit: pageSize } });
});
