
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//站臺管理Model
Ext.define('gigade.Status', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "id", type: "int" },
        { name: "deduct_bonus", type: "int" },
        { name: "status", type: "int" }  
    ]
});

var UserDeductBonusStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Status',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetOrderBonus',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
UserDeductBonusStore.on('beforeload', function () {
    Ext.apply(UserDeductBonusStore.proxy.extraParams,
        {
            Order_Id: window.parent.GetOrderId()

        });
});
Ext.onReady(function () {
    var StatusGrid = Ext.create('Ext.grid.Panel', {
        id: 'StatusGrid',
        store: UserDeductBonusStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "編號", dataIndex: 'id', width: 60, align: 'center' },
            { header: "扣除購物金", dataIndex: 'deduct_bonus', width: 200, align: 'center' },
            {
                header: "狀態", dataIndex: 'status', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.status == 0) {
                        return "未扣除";
                    }
                    else {
                        return "已扣除";
                    }
                }
            }
        ],

        bbar: Ext.create('Ext.PagingToolbar', {
            store: UserDeductBonusStore,
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
    UserDeductBonusStore.load({ params: { start: 0, limit: 25 } });
});
