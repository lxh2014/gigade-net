var pageSize = 25;
/**********************************************************************群組管理主頁面**************************************************************************************/
//群組管理Model

Ext.define('gigade.hgStatus', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'status', type: 'int' },
    { name: 'statusStr', type: 'string' }
    ]
});
var hgStatusStore = Ext.create('Ext.data.Store', {
    model: 'gigade.hgStatus',
    autoLoad: true,
    data: [
    { status: '0', statusStr: '未上傳' },
    { status: '1', statusStr: '已上傳' },
    { status: '2', statusStr: '取消上傳' },
     { status: '3', statusStr: '上傳中' }
    ]
});



Ext.define('gigade.HgDeductModel', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'id', type: 'int' },
    { name: 'token', type: 'string' },
    { name: 'message', type: 'string' },
    { name: 'date', type: 'string' },
    { name: 'time', type: 'string' }
    ]
});
var HgDeductStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    autoLoad: true,
    model: 'gigade.HgDeductModel',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetHgDeductList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
HgDeductStore.on('beforeload', function () {
    Ext.apply(HgDeductStore.proxy.extraParams,
        {
            Order_Id: window.parent.GetOrderId()

        });
});

Ext.define('gigade.HgAccumulateModel', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'id', type: 'int' },
    { name: 'set_point', type: 'string' },
    { name: 'point_amount', type: 'string' },
    { name: 'status', type: 'string' },
    { name: 'note', type: 'string' }
    ]
});
var HgAccumulateStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    autoLoad: true,
    model: 'gigade.HgAccumulateModel',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetHgAccumulateList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
HgAccumulateStore.on('beforeload', function () {
    Ext.apply(HgAccumulateStore.proxy.extraParams,
        {
            Order_Id: window.parent.GetOrderId()

        });
});

Ext.define('gigade.HgAccumulateRefundModel', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'id', type: 'int' },
    { name: 'refund_point', type: 'string' },
    { name: 'status', type: 'string' },
    { name: 'note', type: 'string' }
    ]
});
var HgAccumulateRefundStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    autoLoad: true,
    model: 'gigade.HgAccumulateRefundModel',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetHgAccumulateRefundList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
HgAccumulateRefundStore.on('beforeload', function () {
    Ext.apply(HgAccumulateRefundStore.proxy.extraParams,
        {
            Order_Id: window.parent.GetOrderId()
        });
});

Ext.define('gigade.HgDeductRefundModel', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'id', type: 'int' },
    { name: 'refund_point', type: 'string' },
    { name: 'status', type: 'string' },
    { name: 'note', type: 'string' }
    ]
});

var HgDeductRefundStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    autoLoad: true,
    model: 'gigade.HgDeductRefundModel',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetHgDeductRefundList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
HgDeductRefundStore.on('beforeload', function () {
    Ext.apply(HgDeductRefundStore.proxy.extraParams,
        {
            Order_Id: window.parent.GetOrderId()
        });
});


Ext.define('gigade.HgDeductReverseModel', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'id', type: 'int' },
    { name: 'message', type: 'string' }
    ]
});
var HgDeductReverseStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    autoLoad: true,
    model: 'gigade.HgDeductReverseModel',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetHgDeductReverseList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

HgDeductReverseStore.on('beforeload', function () {
    Ext.apply(HgDeductReverseStore.proxy.extraParams,
        {
            Order_Id: window.parent.GetOrderId()

        });
});

Ext.onReady(function () {

    var gdHgDeduct = Ext.create('Ext.grid.Panel', {
        id: 'gdHgDeduct',
        layout: 'vbox',
        title: '交易記錄',
        store: HgDeductStore,
        width: document.documentElement.clientWidth,
        height: 150,
        columnLines: true,
        frame: true,
        columns: [
        { header: '扣點編號', dataIndex: 'id', align: 'center',flex: 1 },
        { header: 'token', dataIndex: 'token', align: 'center', flex: 1 },
        { header: '訊息', dataIndex: 'message',align: 'center', flex: 1 },
        //{ header: 'date', dataIndex: 'date', align: 'center' },
        //{ header: 'time', dataIndex: 'time', align: 'center' },
        {
            header: '時間', align: 'center', dataIndex: 'date', width: 100, align: 'center',flex: 1.5,
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                var date = value;
                var time = record.data.time;
                var datetime = date.toString().substr(0, 2) + "-" + date.toString().substr(2, 2) + " " + time.toString().substr(0, 2) + ":" + time.toString().substr(2, 2) + ":" + time.toString().substr(4, 2);
                return datetime;
            }
        }
        //<td>{{$value.date|substr:0:2}}-{{$value.date|substr:2:2}} {{$value.time|substr:0:2}}:{{$value.time|substr:2:2}}:{{$value.time|substr:4:2}}</td>
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });

    var gdHgAccumulate = Ext.create('Ext.grid.Panel', {
        id: 'gdHgAccumulate',
        title: '累點記錄',
        store: HgAccumulateStore,
        width: document.documentElement.clientWidth,
        height: 150,
        columnLines: true,
        frame: true,
        columns: [
        { header: '累點編號', dataIndex: 'id', align: 'center', align: 'center' ,flex: 1},
        { header: '累點', dataIndex: 'set_point', align: 'center', flex: 1 },
        { header: '累點總計', dataIndex: 'point_amount', align: 'center', flex: 1 },
        {
            header: '狀態', dataIndex: 'status', align: 'center', flex: 1,
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                return hgStatusStore.getAt(hgStatusStore.find('status', value)).data.statusStr;
            }
        },
        { header: '備註', dataIndex: 'note', width: 300, align: 'center', flex: 1 }

        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });
    var gdHgAccumulateRefund = Ext.create('Ext.grid.Panel', {
        id: 'gdHgAccumulateRefund',
        title: '累點取回記錄',
        store: HgAccumulateRefundStore,
        width: document.documentElement.clientWidth,
        height: 150,
        columnLines: true,
        frame: true,
        columns: [
        { header: '取回點數編號', dataIndex: 'id', align: 'center', flex: 1 },
        { header: '取回點數', dataIndex: 'refund_point', align: 'center', flex: 1 },
        {
            header: '狀態', dataIndex: 'status', align: 'center', flex: 1,
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                return hgStatusStore.getAt(hgStatusStore.find('status', value)).data.statusStr;
            }
        },
        { header: '備註', dataIndex: 'note', width: 300, align: 'center', flex: 1 }
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });
    var gdHgDeductRefund = Ext.create('Ext.grid.Panel', {
        id: 'gdHgDeductRefund',
        title: '抵點歸還記錄',
        store: HgDeductRefundStore,
        width: document.documentElement.clientWidth,
        height: 150,
        columnLines: true,
        frame: true,
        columns: [
        { header: '歸還點數編號', dataIndex: 'id', align: 'center', flex: 1 },
        { header: '歸還點數', dataIndex: 'refund_point', align: 'center', flex: 1 },
        {
            header: '狀態', dataIndex: 'status', align: 'center', flex: 1,
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                return hgStatusStore.getAt(hgStatusStore.find('status', value)).data.statusStr;
            }
        },
        { header: '備註', dataIndex: 'note', width: 300, align: 'center', flex: 1 }

        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });
    var gdHgDeductReverse = Ext.create('Ext.grid.Panel', {
        id: 'gdHgDeductReverse',
        title: '抵點歸還記錄(即時)',
        store: HgDeductReverseStore,
        width: document.documentElement.clientWidth,
        height: 150,
        columnLines: true,
        frame: true,
        columns: [
        { header: '抵點歸還記錄(即時)編號', width:200,dataIndex: 'id', align: 'center', align: 'center' },
        { header: '訊息', dataIndex: 'message', width: 300, align: 'center' }
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });
    var HGPanel = Ext.create('Ext.form.Panel', {
        id: 'HGPanel',
        // title: '交易記錄',
        autoScroll: true,
        layout: 'auto',
        width: document.documentElement.clientWidth,
        frame: true,
        items: [gdHgDeduct, gdHgAccumulate, gdHgAccumulateRefund, gdHgDeductRefund, gdHgDeductReverse],
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
        items: [HGPanel],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            // beforeRender: function () {
            //var value = 1;
            //alert(hgStatusStore.getAt(hgStatusStore.find('status', value)).data.statusStr);
            // <td>{{$value.date|substr:0:2}}-{{$value.date|substr:2:2}} {{$value.time|substr:0:2}}:{{$value.time|substr:2:2}}:{{$value.time|substr:4:2}}</td>
            //var date = "0603";
            //var time = 154448;
            //alert(date.toString().substr(0, 2))
            //alert(date.toString().substr(0, 2) + "-" + date.toString().substr(2, 2) + " " + time.toString().substr(0, 2) + ":" + time.toString().substr(2, 2) + ":" + time.toString().substr(4, 2))
            //  },
            resize: function () {
                HGPanel.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });

});
