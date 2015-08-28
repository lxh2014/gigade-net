var CallidForm;
var pageSize = 25;

//供应商出货单Model
Ext.define('gigade.OrderWaitDeliver', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "order_id", type: "string" },
        { name: "order_name", type: "string" },
        { name: "order_mobile", type: "string" },
        { name: "delivery_name", type: "string" },
        { name: "delivery_mobile", type: "string" },
        { name: "delivery_address", type: "string" },
        { name: "status", type: "string" },
        { name: "pay_time", type: "string" },
        { name: "holiday_deliver", type: "int" },
        {name: "delivery_zip", type: "string" },
        { name: "delivery_address", type: "string" },
        { name: "note_order", type: "string" },
        { name: "delivery", type: "string" },
        { name: "Slave_Id", type: "string" }
        //{ name: "key", type: "string" } //用於登錄到供應商後臺而被加密的key
    ]
});
//供应商出货单Store
var OrderWaitStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.OrderWaitDeliver',
    proxy: {
        type: 'ajax',
        url: '/SendProduct/VendorWaitDeliverList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
OrderWaitStore.load({
    callback: function () {
        var totalcount = OrderWaitStore.getCount();
        if (totalcount==0) {
            alert("~沒有待出貨訂單~");
        }
    }
});

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 50,
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        html: '<div class="capion">待出貨訂單</div>',
                        frame: false,
                        border: false
                    }
                ]
            }
        ]
    });

    var WaitDeliver = Ext.create('Ext.grid.Panel', {
        id: 'WaitDeliver',
        store: OrderWaitStore,
        flex: 8.8,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "付款單號", dataIndex: 'order_id', width: 80, align: 'center' },
            { header: "訂購姓名", dataIndex: 'order_name', width: 80, align: 'center' },
            { header: "訂購手機", dataIndex: 'order_mobile', width: 90, align: 'center' },
            { header: "收貨姓名", dataIndex: 'delivery_name', width: 80, align: 'center' },
            { header: "收貨手機", dataIndex: 'delivery_mobile', width: 90, align: 'center' },
            { header: "地址", dataIndex: 'delivery_address', width: 270, align: 'center' },
            { header: "狀態", dataIndex: 'status', width: 70, align: 'center' },
            { header: "轉單日期", dataIndex: 'pay_time', width: 150, align: 'center' },
            {
                header: "假日可出貨", dataIndex: 'holiday_deliver', width: 90, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "可";
                    } else {
                        return "不可";
                    }
                }
            },
            { header: "到貨時間", dataIndex: 'delivery', width: 100, align: 'center' },
            { header: "備註", dataIndex: 'note_order', width: 230, align: 'center' },
           
            {
                header: "出貨", dataIndex: '', width: 70, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return Ext.String.format("<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.Slave_Id + ")'>出貨</a>");
                }
            },
            {
                header: "列印", dataIndex: '', width: 70, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return Ext.String.format("<a  href='javascript:void(0);' onclick='PrintOrderDeliverDetail("+record.data.order_id+")'>列印</a>", record.data.order_id);
                }
            }
        ],
        tbar: [
            //{
            //    xtype: 'button',
            //    text: '出貨',
            //    id: 'btnShipment',
            //    handler: function () {

            //    }
            //},
            //{},
            {
                xtype: 'button',
                text: '批次列印',
                id: 'btnPrintList',
                handler: PrintOrderDeliverDetailList
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: OrderWaitStore,
            pageSize: pageSize,
            displayInfo: true
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
        layout: 'vbox',
        items: [frm,WaitDeliver],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                WaitDeliver.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    OrderWaitStore.load({ params: { start: 0, limit: 25 } });
});
function UpdateActive(id) {
    addFunction(id);
}

function PrintOrderDeliverDetail(order_id) {
    PintFunction(order_id);
}

function PrintOrderDeliverDetailList() {
    PintListFunction();
}