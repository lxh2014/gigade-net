
var CallidForm;
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//站臺管理Model
Ext.define('gigade.CancelMsg', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "scancel_type", type: "string" },
        { name: "cancel_createdate", type: "string" },
        { name: "response_createdate", type: "string" },
        { name: "cancel_content", type: "string" },
        { name: "response_content", type: "string" }
       
    ]
});

var CancelMsgStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.CancelMsg',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetCancelMsg',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
}); CancelMsgStore.on('beforeload', function () {
    Ext.apply(CancelMsgStore.proxy.extraParams,
        {
            Order_Id: window.parent.GetOrderId()

        });
});
Ext.onReady(function () {
    var CancelMsgGrid = Ext.create('Ext.grid.Panel', {
        id: 'CancelMsgGrid',
        store: CancelMsgStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "問題分類", dataIndex: 'scancel_type', width: 100, align: 'center' },
            {
                header: "留言日期", dataIndex: 'cancel_createdate', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value.substr(0, 10) == '0001-01-01') {
                        return "-"
                    }
                    else {
                        return value;
                    }
                }
            },
            {
                header: "回覆日期", dataIndex: 'response_createdate', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value.substr(0,10) == '0001-01-01') {
                        return "-"
                    }
                    else {
                        return value;
                    }
                }
            },
            { header: "留言內容", dataIndex: 'cancel_content', width: 100, align: 'center' },
            { header: "回覆內容", dataIndex: 'response_content', width: 100, align: 'center' }
           

        ],

        bbar: Ext.create('Ext.PagingToolbar', {
            store: CancelMsgStore,
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
        items: [CancelMsgGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                CancelMsgGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    CancelMsgStore.load({ params: { start: 0, limit: 25 } });
});





