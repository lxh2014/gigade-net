Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;

//出貨查詢Model
Ext.define('GIGADE.OrderMaster', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'OrderId', type: 'int' },          //訂單編號
        { name: 'OrderDatePay', type: 'string' }, //付款日期
        { name: 'MoneyCollectDate', type: 'string' },
        { name: 'Order_Status', type: 'int' },      //訂單狀態
        { name: 'note_order', type: 'string' },     //備註
        { name: 'deliver_id', type: 'int' },        //出貨編號
        { name: 'freight_set', type: 'int' },       //運送方式：freight_set 1 => '常溫',  2 => '冷凍', 5 => '冷藏'
        { name: 'type', type: 'int' },              //出貨類別 通過這個來判斷是訂單編號還是批次出貨編號
        { name: 'vendor_id', type: 'int' },         //供應商編號
        { name: 'vendor_name_simple', type: 'string' },//供應商名稱
        { name: 'product_name', type: 'string' },   //商品名稱
        { name: 'product_spec_name', type: 'string' },
        { name: 'buy_num', type: 'int' },           //購買數量
        { name: 'delivery_name', type: 'string' },  //收件人
        { name: 'order_name', type: 'string' },     //寄件人
        { name: 'delivery_status', type: 'int' },   //出貨單狀態
        { name: 'detail_status', type: 'int' },     //商品狀態
        { name: 'combined_mode', type: 'int' },
        { name: 'parent_name', type: 'string' },
        { name: 'item_mode', type: 'int' }
    ]
});

//出貨查詢列表Store
var ShipmentStore = Ext.create("Ext.data.Store", {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'GIGADE.OrderMaster',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/SendProduct/EveryDayShipmentList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
ShipmentStore.on('beforeload', function () {
    if (document.getElementById("hid_delay").value == 1) {
        var start;
        var end;
        var time;
        var time = new Date(document.getElementById("hid_time").value);
        start = new Date(time.setMinutes(00));
        start = new Date(time.setSeconds(00));
        start = new Date(time.setHours(00));
        end = new Date(time.setMinutes(59));
        end = new Date(time.setSeconds(59));
        end = new Date(time.setHours(23));
        Ext.getCmp("time_start").setValue(start);
        Ext.getCmp("time_end").setValue(end);
        Ext.getCmp("delay").setValue(1);
        document.getElementById("hid_delay").value = 0;
    } else { Ext.getCmp("delay").setValue(0); }
    Ext.apply(ShipmentStore.proxy.extraParams, {

        orderDatePayStartTime: Ext.getCmp("time_start").getValue(),
        orderDatePayEndTime: Ext.getCmp("time_end").getValue(),
        delays: Ext.getCmp("delay").getValue()
    })
   
});

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 80,
        border: 0,
        width: document.documentElement.clientWidth,
        items: [
            {
                html: '<div class="title">每日出貨總表</div>',
                frame: false,
                margin: '0 0 3 0',
                border: false
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'displayfield',
                        fieldLabel: '加載',
                        id: 'delay',
                        name: 'delay',
                        padding: '5 0 5 5',
                        hidden: true,
                        width: 20,
                        value:0
                    },
                    {
                        xtype: 'datetimefield',
                        id: 'time_start',
                        fieldLabel: '出貨開始日期',
                        name: 'time_start',
                        margin: '0 5px 0 5px',
                        editable: false,
                        format: 'Y-m-d H:i:s',
                        value: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 0, 0, 0)
                    },
                    {
                        xtype: 'displayfield',
                        value: '~'
                    },
                    {
                        xtype: 'datetimefield',
                        id: 'time_end',
                        fieldLabel: '出貨結束日期',
                        name: 'time_end',
                        margin: '0 5px',
                        format: 'Y-m-d H:i:s',
                        editable: false,
                        value: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 23, 59, 59)

                    },
                    {
                        xtype: 'button',
                        margin: '0 10 0 10',
                        iconCls: 'icon-search',
                        text: "查詢",
                        handler: Query
                    }
                ]
            }
        ]
    });

    //頁面加載時創建grid
    var ShipmentListGrid = Ext.create('Ext.grid.Panel', {
        id: 'ShipmentListGrid',
        store: ShipmentStore,
        //height: 720,
        flex: 1.8,
        columnLines: true,
        frame: true,
        columns: [
            {
                header: '訂單編號', dataIndex: 'OrderId', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 0)
                    {
                        return "";
                    } else {
                        return Ext.String.format("<a  href=javascript:TranToOrderDetial('/OrderManage/OrderDetialList','{0}') style='text-decoration:none'>{1}</a>", record.data.OrderId, record.data.OrderId);
                    }
                }
            },
            {
                header: '付款日期', dataIndex: 'MoneyCollectDate', width: 140, align: 'center', format: 'Y-m-d H:i:s',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (value) {
                        case "1970-01-01 08:00:00":
                            return "未付款";
                            break;
                        case "0001-01-01 00:00:00":
                            return "";
                            break;
                        default:
                            return value;
                            break;
                    }
                }
            },
            { header: '收件人', dataIndex: 'order_name', width: 100, align: 'center' },
            { header: '寄件人', dataIndex: 'delivery_name', width: 100, align: 'center' },
            {
                header: '出貨類型', dataIndex: 'type', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (value) {
                        case 0:
                            return "";
                            break;
                        case 1:
                            return "統倉出貨";
                            break;
                        case 2:
                            return "供應商自行出貨";
                            break;
                        default:
                            return value;
                            break;
                    }
                }
            },
            {
                header: '出貨編號', dataIndex: 'deliver_id', width: 90, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value==0) {
                        return "";//重復數據不顯示
                    } else {
                        //return value;
                        return Ext.String.format("<a  href=javascript:TranToDeliverDetial('/SendProduct/DeliverView','{0}') style='text-decoration:none'>{1}</a>", record.data.deliver_id, record.data.deliver_id);
                    }
                }
            },
            {
                header: '運送方式', dataIndex: 'freight_set', width: 90, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (record.data.freight_set) {
                        case 0:
                            return "";
                            break;
                        case 1:
                            return "常溫";
                            break;
                        case 2:
                            return "冷凍";
                            break;
                        case 3:
                            return "常溫免運";
                            break;
                        case 4:
                            return "冷凍免運";
                            break;
                        case 5:
                            return "冷藏";
                            break;
                        case 6:
                            return "冷藏免運";
                            break;
                        default:
                            return record.data.freight_set;
                            break;
                    }
                }
            },
            { header: '供應商', dataIndex: 'vendor_name_simple', width: 90, align: 'center' },
            { header: '商品名稱', dataIndex: 'product_name', width: 250, align: 'center' },
            { header: '數量', dataIndex: 'buy_num', width: 90, align: 'center' },
            {
                header: '訂單狀態', dataIndex: 'Order_Status', width: 90, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (value) {
                        case 999:
                            return "";
                            break;
                        case 0:
                            return "等待付款";
                            break;
                        case 2:
                            return "待出貨";
                            break;
                        case 3:
                            return "出貨中";
                            break;
                        default:
                            return value;
                            break;
                    }
                }
            },
            {
                header: '出貨單狀態', dataIndex: 'delivery_status', width: 90, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (record.data.delivery_status) {
                        case 999:
                            return "";
                            break;
                        case 0:
                            return "<font color='red'>待出貨</font>";
                            break;
                        case 1:
                            return "可出貨";
                            break;
                        case 2:
                            return "出貨中";
                            break;
                        case 3:
                            return "已出貨";
                            break;
                        case 4:
                            return "已到貨";
                            break;
                        case 5:
                            return "未到貨";
                            break;
                        case 6:
                            return "取消出貨";
                            break;
                        case 7:
                            return "待取貨";
                            break;
                        default:
                            return "意外數據錯誤";
                            break;
                    }
                }
            },
            {
                header: '商品狀態', dataIndex: 'detail_status', width: 90, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (value) {
                        case 0:
                            return "等待付款";
                            break;
                        case 1:
                            return "付款失敗";
                            break;
                        case 2:
                            return "待出貨";
                            break;
                        case 3:
                            return "出貨中";
                            break;
                        case 4:
                            return "已出貨";
                            break;
                        case 5:
                            return "處理中";
                            break;
                        case 6:
                            return "進倉中";
                            break;
                        case 7:
                            return "已進倉";
                            break;
                        case 8:
                            return "扣點中";
                            break;
                        case 9:
                            return "待取貨";
                            break;
                        case 10:
                            return "等待取消";
                            break;
                        case 20:
                            return "訂單異常";
                            break;
                        case 89:
                            return "單一商品取消";
                            break;
                        case 90:
                            return "訂單取消";
                            break;
                        case 91:
                            return "訂單退貨";
                            break;
                        case 92:
                            return "訂單換貨";
                            break;
                        case 99:
                            return "訂單歸檔";
                            break;
                        default:
                            return value;
                            break;
                    }
                }
            },
            { header: '備註', dataIndex: 'note_order', width: 150, align: 'center' }
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ShipmentStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        })
    });

    Ext.create('Ext.Viewport', {
        layout: 'vbox',
        items: [frm, ShipmentListGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                ShipmentListGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });

    ToolAuthority();
    ShipmentStore.load({ params: { start: 0, limit: 25 } });
});

//查询
Query = function () {
    ShipmentStore.removeAll();
    Ext.getCmp("ShipmentListGrid").store.loadPage(1, {
        params: {
            orderDatePayStartTime: Ext.getCmp("time_start").getValue(),
            orderDatePayEndTime: Ext.getCmp("time_end").getValue(),
            delays:0
        }
    });
}

//跳轉到訂單明細頁
function TranToOrderDetial(url, order_id) {
    var urlTran = url + '?Order_Id=' + order_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#pnlOrderDetail');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'pnlOrderDetail',
        title: "訂單內容",
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}
//跳轉到出貨明細頁
function TranToDeliverDetial(url, deliver_id) {
    var urlTran = url + '?deliver_id=' + deliver_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#pnlDeliverView');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'pnlDeliverView',
        title: "出貨明細",
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}