
var CallidForm;
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//站臺管理Model
Ext.define('gigade.Status', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "deliver_id", type: "string" },
        { name: "logistics_type", type: "string" },
        { name: "delivery_store_name", type: "string" },
        { name: "set_time", type: "string" }
       
    ]
});

var LogisticsStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Status',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetLogistics',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
LogisticsStore.on('beforeload', function () {
    Ext.apply(LogisticsStore.proxy.extraParams,
        {
            Order_Id: window.parent.GetOrderId()

        });
});
Ext.onReady(function () {
    var LogisticsGrid = Ext.create('Ext.grid.Panel', {
        id: 'LogisticsGrid',
        store: LogisticsStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "NO", xtype: 'rownumberer', width: 60, align: 'center' },
            { header: "出貨單號", dataIndex: 'deliver_id', width: 200, align: 'center' },
            { header: "出貨單狀態", dataIndex: 'logistics_type', width: 100, align: 'center' },
            { header: "物流廠商", dataIndex: 'delivery_store_name', width: 100, align: 'center' },
            { header: "記錄日期", dataIndex: 'set_time', width: 200, align: 'center' }

        ],

        bbar: Ext.create('Ext.PagingToolbar', {
            store: LogisticsStore,
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
        items: [LogisticsGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                LogisticsGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    LogisticsStore.load({ params: { start: 0, limit: pageSize } });
});
