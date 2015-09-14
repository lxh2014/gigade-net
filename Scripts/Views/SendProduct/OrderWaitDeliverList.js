var CallidForm;
var pageSize = 25;

//供应商出货单Model
Ext.define('gigade.OrderWaitDeliver', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "order_id", type: "string" },
        { name: "vendor_name_simple", type: "string" },    
        { name: "order_name", type: "string" },
        { name: "order_mobile", type: "string" },
        { name: "delivery_name", type: "string" },
        { name: "delivery_mobile", type: "string" },
        { name: "status", type: "string" },
        { name: "pay_time", type: "string" },
        { name: "vendor_id", type: "int" },
        { name: "key", type: "string" } //用於登錄到供應商後臺而被加密的key
    ]
});
//供应商出货单Store
var OrderWaitStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.OrderWaitDeliver',
    proxy: {
        type: 'ajax',
        url: '/SendProduct/OrderWaitDeliverList',
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

OrderWaitStore.on('beforeload', function () {
    Ext.apply(OrderWaitStore.proxy.extraParams, {
        searchcontent: Ext.getCmp('searchcontent').getValue(),
        search_type: Ext.getCmp('search_type').getValue(),
        search_vendor: Ext.getCmp('search_vendor').getValue()
    });
});

function Query(x) {
    OrderWaitStore.removeAll();
    Ext.getCmp("WaitDeliver").store.loadPage(1, {
        params: {
            searchcontent: Ext.getCmp('searchcontent').getValue(),
            search_type: Ext.getCmp('search_type').getValue(),
            search_vendor: Ext.getCmp('search_vendor').getValue()
        }
    });
}

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 120,
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
                        xtype: 'combobox',
                        name: 'search_type',
                        id: 'search_type',
                        editable: false,
                        fieldLabel: "查詢條件",
                        labelWidth: 70,
                        margin: '0 5px',
                        store: ShippedQueryStore,
                        queryMode: 'local',
                        submitValue: true,
                        displayField: 'txt',
                        valueField: 'value',
                        typeAhead: true,
                        forceSelection: false,
                        emptyText: '所有訂單資料',
                        listeners: {
                            change: function () {
                                Ext.getCmp('searchcontent').setValue('');
                            }
                        }
                    },
                    {
                        id: 'searchcontent',
                        xtype: 'textfield',
                        width: 200,
                        name: 'searchcontent',
                        allowBlank: true
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'combobox',
                        id: 'search_vendor',
                        margin: '0 5px',
                        fieldLabel: '供應商條件',
                        labelWidth: 70,
                        queryMode: 'local',
                        editable: false,
                        store: VendorConditionStore,
                        displayField: 'vendor_name_simple',
                        valueField: 'vendor_id',
                        listeners: {
                            beforerender: function () {
                                VendorConditionStore.load({
                                    callback: function () {
                                        VendorConditionStore.insert(0, { vendor_id: '0', vendor_name_simple: '所有供應商資料' });
                                        Ext.getCmp("search_vendor").setValue(VendorConditionStore.data.items[0].data.vendor_id);
                                    }
                                });
                            }
                        }
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'button',
                        text: SEARCH,
                        iconCls: 'icon-search',
                        margin: '0 10 0 10',
                        id: 'btnQuery',
                        handler: Query
                    },
                    {
                        xtype: 'button',
                        text: RESET,
                        id: 'btn_reset',
                        listeners: {
                            click: function () {
                                Ext.getCmp("searchcontent").setValue('');
                                Ext.getCmp("search_vendor").setValue(0);
                                Ext.getCmp("search_type").setValue(0);
                            }
                        }
                    }
                ]
            }
        ],
        listeners: {
            //beforerender: function () {
            //    var delivery_statu = document.getElementById("Delivery_Status").value;
            //    var sear= document.getElementById("Search").value;
            //    var type = document.getElementById("Delivery_Type").value;
            //    if (delivery_statu != "")
            //    {
            //        Ext.getCmp('shipmentstatus').setValue(delivery_statu);
            //    }
            //    if (sear != "")
            //    {
            //        Ext.getCmp('search').setValue(sear);
            //    }
            //    if (type != "") {
            //        Ext.getCmp('shipmenttype').setValue(type);
            //    }
               
            //}
        }
    });

    var WaitDeliver = Ext.create('Ext.grid.Panel', {
        id: 'WaitDeliver',
        store: OrderWaitStore,
        flex: 8.8,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            {
                header: '付款單號', colName: 'order_id', xtype: 'templatecolumn',
                tpl: Ext.create('Ext.XTemplate', '<tpl >', '<a href=javascript:TranToDetial("{order_id}") >{order_id}</a>', '</tpl>'),
                align: 'center', sortable: false, menuDisabled: true
            },
            { header: "供應商名稱", dataIndex: 'vendor_name_simple', width: 150, align: 'center' },
            { header: "訂購姓名", dataIndex: 'order_name', width: 150, align: 'center' },
            { header: "訂購手機", dataIndex: 'order_mobile', width: 100, align: 'center' },
            { header: "收貨姓名", dataIndex: 'delivery_name', width: 150, align: 'center' },
            { header: "收貨手機", dataIndex: 'delivery_mobile', width: 150, align: 'center' },
            { header: "狀態", dataIndex: 'status', width: 150, align: 'center' },
            { header: "轉單日期", dataIndex: 'pay_time', width: 150, align: 'center' },
            {
                header: "供應商後臺", dataIndex: 'vendor_id', width: 150, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value != null) {
                        return Ext.String.format("<a  href='{0}' target='_blank' style='text-decoration:none'>進入</a>", record.data.key);
                    }
                }
            }
        ],
        tbar: [
           {
               xtype: 'button',
               text: '匯出CSV',
               iconCls: 'icon-excel',
               id: 'btnExcel',
               handler: function ()
               {
                   window.open("/SendProduct/OrderWaitDeliverExport?searchcontent=" + Ext.getCmp('searchcontent').getValue() + "&search_type=" + Ext.getCmp('search_type').getValue() + "&search_vendor=" + Ext.getCmp('search_vendor').getValue());
               }
              // ,handler: Query 
           } 
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: OrderWaitStore,
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


function TranToDetial(orderId) {
    var url = '/OrderManage/OrderDetialList?Order_Id=' + orderId;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#detial');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'detial',
        title: '訂單內容',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}