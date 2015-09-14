var pageSize = 25;
/**********************************************************************群組管理主頁面**************************************************************************************/
//群組管理Model

Ext.define('gigade.ProductDetailModel', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'return_id', type: 'int' },
    { name: 'orc_order_id', type: 'string' },
    { name: 'vendor_name_simple', type: 'string' },
    { name: 'company_phone', type: 'string' },
    { name: 'orc_address', type: 'string' },
    { name: 'order_status_str', type: 'string' },
    { name: 'orc_remark', type: 'string' },
    { name: 'orc_deliver_date', type: 'string' },
    { name: 'orc_service_remark', type: 'string' }
    ]
});

var OrderReturnUpStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    autoLoad: true,
    model: 'gigade.ProductDetailModel',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetOrderReturnContentQueryUp',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
OrderReturnUpStore.on('beforeload', function () {
    Ext.apply(OrderReturnUpStore.proxy.extraParams,
        {
            Order_Id: window.parent.GetOrderId()
        });
});

Ext.define('gigade.Status', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'return_id', type: 'int' },
        { name: "item_id", type: "string" },
        { name: "product_name", type: "string" },
        { name: "product_freight_set_string", type: "string" },
        { name: "buy_num", type: "string" },
        { name: "single_money", type: "string" },
        { name: "subtotal", type: "string" },
        { name: "deduct_bonus", type: "string" }
    ]
});

var OrderReturnDownStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    autoLoad: true,
    model: 'gigade.Status',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetOrderReturnDown',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
OrderReturnDownStore.on('beforeload', function () {
    Ext.apply(OrderReturnDownStore.proxy.extraParams,
        {
            Order_Id: window.parent.GetOrderId()

        });
});




Ext.onReady(function () {
    var OrderReturnUp = Ext.create('Ext.grid.Panel', {
        id: 'OrderReturnUp',
       // title: '退貨商品詳情',
        store: OrderReturnUpStore,
        width: document.documentElement.clientWidth,
        height: 350,
        columnLines: true,
        frame: true,
        columns: [
        { header: '退貨單號', dataIndex: 'return_id', align: 'center', flex: 1 },
        { header: '退貨單狀態', dataIndex: 'order_status_str', align: 'center',flex: 1 },
        { header: '出貨商', dataIndex: 'vendor_name_simple', align: 'center', flex: 1 },
        { header: '公司電話', dataIndex: 'company_phone', align: 'center', flex: 1},
        { header: '地址', dataIndex: 'orc_address', align: 'center', flex: 2 },
        { header: '退貨狀態創建時間', dataIndex: 'orc_deliver_date', align: 'center', flex: 2 },
        { header: '使用者備註', dataIndex: 'orc_remark', align: 'center', flex: 1 },
        { header: '管理者備註', dataIndex: 'orc_service_remark', align: 'center', flex: 1 }
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

    var OrderReturnDown = Ext.create('Ext.grid.Panel', {
        id: 'OrderReturnDown',
        layout: 'vbox',       
        store: OrderReturnDownStore,
        width: document.documentElement.clientWidth,
        height: 350,
        columnLines: true,
        frame: true,
        columns: [
        { header: '退貨單號', dataIndex: 'return_id', align: 'center' },
        { header: '商品編號', dataIndex: 'item_id', align: 'center' },
        { header: '商品名稱', dataIndex: 'product_name', align: 'center' },
        { header: '托運單屬性', dataIndex: 'product_freight_set_string', align: 'center' },
        { header: '數量', dataIndex: 'buy_num',align: 'center',  width: 100, align: 'center' },
        { header: '折扣價', dataIndex: 'single_money', align: 'center' },
        { header: '小計', dataIndex: 'subtotal', align: 'center' },
        { header: '購物金', dataIndex: 'deduct_bonus', align: 'center' }
      
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

   
    var OrderReturnPanel = Ext.create('Ext.form.Panel', {
        id: 'OrderReturnPanel',
        // title: '交易記錄',
        autoScroll: true,
        layout: 'auto',
        width: document.documentElement.clientWidth,
        frame: true,
        items: [ OrderReturnUp,OrderReturnDown],
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
        items: [OrderReturnPanel],
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
                OrderReturnPanel.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });

});
