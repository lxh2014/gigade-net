
var CallidForm;
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//站臺管理Model
Ext.define('gigade.Status', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "serial_id", type: "int" },
        { name: "order_id", type: "int" },
        { name: "order_status", type: "int" },
        { name: "status_description", type: "string" },
        { name: "status_ipfrom", type: "string" },
        { name: "status_createdates", type: "string" },
        { name: "states", type: "string" }
    ]
});

var StatusStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Status',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetStatus',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
StatusStore.on('beforeload', function () {
    Ext.apply(StatusStore.proxy.extraParams, {
        Order_Id: window.parent.GetOrderId()
    });
});
Ext.onReady(function () {
    var StatusGrid = Ext.create('Ext.grid.Panel', {
        id: 'StatusGrid',
        store: StatusStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "NO", dataIndex: 'serial_id', width: 60, align: 'center' },
            { header: "付款單狀態", dataIndex: 'states', width: 200, align: 'center' },
            { header: "記錄日期", dataIndex: 'status_createdates', width: 150, align: 'center' },
            { header: "來源IP", dataIndex: 'status_ipfrom', width: 120, align: 'center' },
            { header: "備註", dataIndex: 'status_description', width:300, align: 'center' }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: StatusStore,
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
    StatusStore.load({ params: { start: 0, limit: 25 } });
});