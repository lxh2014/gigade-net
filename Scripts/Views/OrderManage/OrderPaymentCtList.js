
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//站臺管理Model
Ext.define('gigade.Status', {
    extend: 'Ext.data.Model',
    fields: [
            { name: "id", type: "int" },//編號
            { name: "status", type: "int" },//狀態
            { name: "errcode", type: "int" },//錯誤代碼
            { name: "authcode", type: "string" },//授權 代碼
            { name: "originalamt", type: "int" },//訂單金額
            { name: "offsetamt", type: "int" },//紅利折抵金額
            { name: "utilizedpoint", type: "int" },//紅利折抵點數
            { name: "errdesc", type: "string" },//交易錯誤訊息
            { name: "xid", type: "string" },//交易序號
            { name: "awardedpoint", type: "int" },//獲取紅利點數
            { name: "pointbalance", type: "int" }//紅利餘額
    ]
});
var OrderPaymentCtStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Status',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetOrderPaymentCtList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
OrderPaymentCtStore.on('beforeload', function () {
    Ext.apply(OrderPaymentCtStore.proxy.extraParams,
        {
            Order_Id: window.parent.GetOrderId()

        });
});

Ext.onReady(function () {
    var StatusGrid = Ext.create('Ext.grid.Panel', {
        id: 'StatusGrid',
        store: OrderPaymentCtStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "編號", dataIndex: 'id', width: 60, align: 'center' },
            { header: "狀態", dataIndex: 'status', width: 60, align: 'center' },
            { header: "錯誤代碼", dataIndex: 'errcode', width: 100, align: 'center' },
            { header: "授權代碼", dataIndex: 'authcode', width: 200, align: 'center' },
            { header: "訂單金額", dataIndex: 'originalamt', width: 60, align: 'center' },
            { header: "紅利折抵金額", dataIndex: 'offsetamt', width: 60, align: 'center' },
            { header: "紅利折抵點數", dataIndex: 'utilizedpoint', width: 60, align: 'center' },
            { header: "交易錯誤訊息", dataIndex: 'errdesc', width: 200, align: 'center' },
            { header: "交易序號", dataIndex: 'xid', width: 200, align: 'center' },
            { header: "獲取紅利點數", dataIndex: 'awardedpoint', width: 100, align: 'center' },
            { header: "紅利餘額", dataIndex: 'pointbalance', width: 100, align: 'center' }
        
        ],

        bbar: Ext.create('Ext.PagingToolbar', {
            store: OrderPaymentCtStore,
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
    OrderPaymentCtStore.load({ params: { start: 0, limit: 25 } });
});
