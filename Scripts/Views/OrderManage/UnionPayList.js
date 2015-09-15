
var CallidForm;
var pageSize = 25;
/***************************站臺管理主頁面********************************************************/

Ext.define('gigade.Status', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "union_id", type: "int" },
        { name: "respcode", type: "string" },
        { name: "transtype", type: "string" },
        { name: "respmsg", type: "string" },
        { name: "merabbr", type: "string" },
        { name: "merid", type: "string" },
        { name: "orderamount", type: "string" },
        { name: "ordercurrency", type: "string" },
        { name: "resptime", type: "string" },
        { name: "cupReserved", type: "string" }
    ]
});
var UnionPayStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Status',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetUnionPayList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
UnionPayStore.on('beforeload', function () {
    Ext.apply(UnionPayStore.proxy.extraParams,
        {
            Order_Id: window.parent.GetOrderId()
        });
});

Ext.onReady(function () {
    var StatusGrid = Ext.create('Ext.grid.Panel', {
        id: 'StatusGrid',
        store: UnionPayStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "流水號", dataIndex: 'union_id', width: 60, align: 'center' },
            { header: "回應碼", dataIndex: 'respcode', width: 200, align: 'center' },
            { header: "交易類型", dataIndex: 'transtype', width: 100, align: 'center' },
            { header: "回應訊息", dataIndex: 'respmsg', width: 100, align: 'center' },
            { header: "特店名稱", dataIndex: 'merabbr', width: 200, align: 'center' },
            { header: "特店代碼", dataIndex: 'merid', width: 100, align: 'center' },
            { header: "缴款金额", dataIndex: 'orderamount', width: 100, align: 'center' },
            { header: "交易币种", dataIndex: 'ordercurrency', width: 100, align: 'center' },
            { header: "交易完成时间", dataIndex: 'resptime', width: 100, align: 'center' },
            { header: "备注", dataIndex: 'cupReserved', width: 100, align: 'center' }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: UnionPayStore,
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
    UnionPayStore.load({ params: { start: 0, limit: 25 } });
});
