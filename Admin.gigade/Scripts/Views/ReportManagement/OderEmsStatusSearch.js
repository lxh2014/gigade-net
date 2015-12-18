var CallidForm;
var pageSize = 20;
Ext.apply(Ext.form.field.VTypes, {
    daterange: function (val, field) {
        var date = field.parseDate(val);

        if (!date) {
            return false;
        }
        this.dateRangeMax = null;
        this.dateRangeMin = null;
        if (field.startDateField && (!this.dateRangeMax || (date.getTime() != this.dateRangeMax.getTime()))) {
            var start = field.up('form').down('#' + field.startDateField);
            start.setMaxValue(date);
            //start.validate();
            this.dateRangeMax = date;
        } else if (field.endDateField && (!this.dateRangeMin || (date.getTime() != this.dateRangeMin.getTime()))) {
            var end = field.up('form').down('#' + field.endDateField);
            end.setMinValue(date);
            //end.validate();
            this.dateRangeMin = date;
        }
        /*  
         * Always return true since we're only using this vtype to set the  
         * min/max allowed values (these are tested for after the vtype test)  
         */
        return true;
    },

    daterangeText: '開始時間必須小於結束時間'
});
//供应商出货单Model
Ext.define('gigade.OrderEmsStatusSearch', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "order_id", type: "string" },
        { name: "deliver_id", type: "string" },
        { name: "delivery_code", type: "string" },
        { name: "order_payment", type: "string" },
        { name: "delivery_store", type: "string" },
        { name: "order_status", type: "string" },
        { name: "logisticsTypes", type: "string" },
        { name: "order_date", type: "string" },
        { name: "delivery_status",type:"string" }
    ]
});
//function Tomorrow(days) {
//    var d;
//    d = new Date();                             // 创建 Date 对象。
//    d.setDate(d.getDate()+days);
//    return d;
//}
function TheMonthFirstDay()
{
    var times;
    times = new Date();
    return new Date(times.getFullYear(),times.getMonth(),1);
}
//列表
var OrderStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.OrderEmsStatusSearch',
    proxy: {
        type: 'ajax',
        url: '/ReportManagement/GetReportManagementList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

Ext.define("gigade.paraModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'parameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
});
//物流商【營管>出貨查詢】
var DeliverStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/OrderManage/QueryPara?paraType=Deliver_Store',
        noCache: false,
        getMethod: function () { return 'get'; },
        timeout: 180000,
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});
Ext.define("gigade.DeliveryModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'parameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
});
//物流商【營管>出貨查詢】
var DeliveryStatusStore = Ext.create("Ext.data.Store", {
    model: 'gigade.DeliveryModel',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/OrderManage/QueryPara?paraType=delivery_status',
        noCache: false,
        getMethod: function () { return 'get'; },
        timeout: 180000,
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});
Ext.define("gigade.orderStatusStores", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'parameterCode', type: 'string' },
        { name: 'remark', type: 'string' }
    ]
});
//訂單狀態
var OrderStatusStore = Ext.create("Ext.data.Store", {
    model: 'gigade.orderStatusStores',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/OrderManage/QueryPara?paraType=order_status',
        noCache: false,
        getMethod: function () { return 'get'; },
        timeout: 180000,
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});
Ext.define("gigade.paymentStore", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'parameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
});
//支付方式
var paymentStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paymentStore',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/OrderManage/QueryPara?paraType=payment',
        noCache: false,
        getMethod: function () { return 'get'; },
        timeout: 180000,
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});
//物流狀態
var LogisticsStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/OrderManage/QueryPara?paraType=logistics_type',
        noCache: false,
        getMethod: function () { return 'get'; },
        timeout: 180000,
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

OrderStore.on('beforeload', function () {
    Ext.apply(OrderStore.proxy.extraParams, {
        order_id: Ext.getCmp("order_id").getValue(),
        deliver_id: Ext.getCmp("deliver_id").getValue(),
        shipment_id: Ext.getCmp("shipment").getValue(),
        order_status_id: Ext.getCmp("order_status_id").getValue(),
        payment_id: Ext.getCmp("payment_id").getValue(),
        dateStart: Ext.getCmp("dateStart").getValue(),
        dateEnd: Ext.getCmp("dateEnd").getValue(),
        logistics_type: Ext.getCmp("logistics_type").getValue(),
        delivery_status: Ext.getCmp("delivery_status").getValue()
    });
});

function Query(x) {
    OrderStore.removeAll();
    Ext.getCmp("WaitDeliver").store.loadPage(1, {
        params: {
            order_id: Ext.getCmp("order_id").getValue(),
            deliver_id: Ext.getCmp("deliver_id").getValue(),
            shipment_id: Ext.getCmp("shipment").getValue(),
            order_status_id: Ext.getCmp("order_status_id").getValue(),
            payment_id: Ext.getCmp("payment_id").getValue(),
            dateStart: Ext.getCmp("dateStart").getValue(),
            dateEnd: Ext.getCmp("dateEnd").getValue(),
            logistics_type: Ext.getCmp("logistics_type").getValue(),
            delivery_status: Ext.getCmp("delivery_status").getValue()
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
                    xtype: 'textfield',
                    id: 'order_id',
                    fieldLabel: "訂單編號",
                    name: 'order_id',
                    allowBlank: true,
                    margin: '0 20px',
                    labelWidth: 70
                },
                {
                    xtype: 'textfield',
                    id: 'deliver_id',
                    margin: '0 20px',
                    labelWidth: 70,
                    fieldLabel: "出貨單號",
                    name: 'order_id',
                    allowBlank: true
                },
                {
                    xtype: 'combobox',
                    id: 'shipment',
                    fieldLabel: '物流業者',
                    colName: 'shipment',
                    name: 'shipment',
                    queryMode: 'local',
                    margin: '0 20px',
                    labelWidth: 70,
                    editable: false,
                    store: DeliverStore,
                    displayField: 'parameterName',
                    valueField: 'parameterCode',
                    value: "0",
                    listeners: {
                        beforerender: function () {
                            DeliverStore.load({
                                callback: function () {
                                    DeliverStore.insert(0, { parameterCode: '0', parameterName: '全部' });
                                    Ext.getCmp("shipment").setValue(DeliverStore.data.items[0].data.parameterCode);
                                }
                            });
                        }
                    }
                },
                {
                    xtype: 'combobox',
                    id: 'delivery_status',
                    fieldLabel: '出貨單狀態',
                    colName: 'delivery_status',
                    name: 'delivery_status',
                    queryMode: 'local',
                    margin: '0 20px',
                    labelWidth: 70,
                    editable: false,
                    store: DeliveryStatusStore,
                    displayField: 'parameterName',
                    valueField: 'parameterCode',
                    value: "0",
                    listeners: {
                        beforerender: function () {
                            DeliveryStatusStore.load({
                                callback: function () {
                                    DeliveryStatusStore.insert(0, { parameterCode: '-1', parameterName: '全部' });
                                    Ext.getCmp("delivery_status").setValue(DeliveryStatusStore.data.items[0].data.parameterCode);
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
                        xtype: 'combobox',
                        id: 'order_status_id',
                        margin: '0 20px',
                        labelWidth: 70,
                        fieldLabel: '訂單狀態',
                        editable: false,
                        queryMode: 'local',
                        store: OrderStatusStore,
                        displayField: 'remark',
                        valueField: 'parameterCode',
                        value:"-1",
                        listeners: {
                            beforerender: function () {
                                OrderStatusStore.load({
                                    callback: function () {
                                        OrderStatusStore.insert(0, { parameterCode: '-1', remark: '全部' });
                                        Ext.getCmp("order_status_id").setValue(OrderStatusStore.data.items[0].data.parameterCode);
                                    }
                                });
                            }
                        }
                    },
                    {
                        xtype: 'combobox',
                        id: 'payment_id',
                        margin: '0 20px',
                        labelWidth: 70,
                        name: 'payment_id',
                        fieldLabel: '付款方式',
                        colName: 'payment_id',
                        queryMode: 'local',
                        editable: false,
                        store: paymentStore,
                        displayField: 'parameterName',
                        valueField: 'parameterCode',
                        typeAhead: true,
                        forceSelection: false, //默認是false
                        value: "0"
                    },
                    {
                        xtype: 'combobox',
                        id: 'logistics_type',
                        margin: '0 20px',
                        labelWidth: 70,
                        fieldLabel: '物流狀態',
                        editable: false,
                        queryMode: 'local',
                        store: LogisticsStore,
                        displayField: 'parameterName',
                        valueField: 'parameterCode',
                        value: "0",
                        listeners: {
                            beforerender: function () {
                                LogisticsStore.load({
                                    callback: function () {
                                        LogisticsStore.insert(0, { parameterCode: '0', parameterName: '全部' });
                                        Ext.getCmp("logistics_type").setValue(LogisticsStore.data.items[0].data.parameterCode);
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
                          xtype: 'datetimefield',
                          id: 'dateStart',
                          name: 'dateStart',
                          margin: '0 0 0 20px',
                          labelWidth: 70,
                          editable: false,
                          fieldLabel: '訂單日期',
                          format: 'Y-m-d H:i:s',
                          time: { hour: 00, min: 00, sec: 00 },
                          value: new Date(Tomorrow().setMonth(Tomorrow().getMonth() - 1)),
                          listeners: {
                              select: function (a, b, c) {
                                  var start = Ext.getCmp("dateStart");
                                  var end = Ext.getCmp("dateEnd");
                                  if (end.getValue() == null) {
                                      end.setValue(setNextMonth(start.getValue(), 1));
                                  }
                                  else if (end.getValue() < start.getValue()) {
                                      Ext.Msg.alert(INFORMATION, DATA_TIP);
                                      start.setValue(setNextMonth(end.getValue(), -1));
                                  }
                                  else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                      // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                      end.setValue(setNextMonth(start.getValue(), 1));
                                  }
                              }
                          }
                      },
                      {
                          xtype: 'displayfield',
                          value: '~',
                          margin: '0 2px',
                          labelWidth: 10
                      },
                      {
                          xtype: 'datetimefield',
                          id: 'dateEnd',
                          name: 'dateEnd',                          
                          editable: false,
                          format: 'Y-m-d H:i:s',
                          time: { hour: 23, min: 59, sec: 59 },
                          value: setNextMonth(Tomorrow(), 0),
                          listeners: {
                              select: function (a, b, c) {
                                  var start = Ext.getCmp("dateStart");
                                  var end = Ext.getCmp("dateEnd");
                                  if (start.getValue() != "" && start.getValue() != null) {
                                      if (end.getValue() < start.getValue()) {
                                          Ext.Msg.alert(INFORMATION, DATA_TIP);
                                          end.setValue(setNextMonth(start.getValue(), 1));
                                      }
                                      else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                          // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                          start.setValue(setNextMonth(end.getValue(), -1));
                                      }
                                  }
                                  else {
                                      start.setValue(setNextMonth(end.getValue(), -1));
                                  }
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
                        text: "查詢",
                        iconCls: 'icon-search',
                        margin: '0 20px',
                        id: 'btnQuery',
                        handler: Query
                    },
                    {
                        xtype: 'button',
                        text: "重置",
                        margin: '0 20px',
                        id: 'btn_reset',
                        listeners: {
                            click: function () {
                                Ext.getCmp('order_id').setValue("");
                                Ext.getCmp('deliver_id').setValue("");
                                Ext.getCmp('shipment').setValue(0);
                                Ext.getCmp('order_status_id').setValue(-1);
                                Ext.getCmp('payment_id').setValue(0);
                                Ext.getCmp('logistics_type').setValue(0);
                                Ext.getCmp('dateStart').reset();
                                Ext.getCmp('dateEnd').reset();
                                Ext.getCmp("delivery_status").setValue(-1)
                            }
                        }
                    }
                ]
            }
        ]
    });

    var WaitDeliver = Ext.create('Ext.grid.Panel', {
        id: 'WaitDeliver',
        store: OrderStore,
        flex: 8.8,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "出貨單號", dataIndex: 'deliver_id', width: 100, align: 'center' },
            {
                header: "訂單編號", dataIndex: 'order_id', width: 120, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//搞定
                  
                    return Ext.String.format("<a id='link" + record.data.order_id + "' href='{0}{1}' target='_blank'>{1}</a>", "http://admin.gigade100.com/order//order_show.php?oid=", record.data.order_id);
         
                }
            },
            { header: "物流單號", dataIndex: 'delivery_code', width: 120, align: 'center' },
            { header: "付款方式", dataIndex: 'order_payment', width: 100, align: 'center' },
            { header: "物流業者", dataIndex: 'delivery_store', width: 110, align: 'center' },
            { header: "訂單狀態", dataIndex: 'order_status', width: 120, align: 'center' },
            { header: "出貨單狀態", dataIndex: 'delivery_status', width: 120, align: 'center' },
            { header: "物流狀態", dataIndex: 'logisticsTypes', width: 120, align: 'center' },
            { header: "訂單日期", dataIndex: 'order_date', width: 120, align: 'center' }
        ],
        tbar: [
             {
                 xtype: 'button',
                 text: '匯出EXCEL',
                 iconCls: 'icon-excel',
                 hidden:true,
                 id: 'btnExcel',
                 handler: function () {
                     window.open("/ReportManagement/ReportManagementExcelList?order_id=" + Ext.getCmp('order_id').getValue() + "&deliver_id=" + Ext.getCmp('deliver_id').getValue() + "&shipment_id=" + Ext.getCmp('shipment').getValue() + "&order_status_id=" + Ext.getCmp('order_status_id').getValue() + "&payment_id=" + Ext.getCmp('payment_id').getValue() + "&dateStart=" + Ext.Date.format(Ext.getCmp('dateStart').getValue(), 'Y-m-d') + "&dateEnd=" + Ext.Date.format(Ext.getCmp('dateEnd').getValue(), 'Y-m-d') + "&logistics_type=" + Ext.getCmp("logistics_type").getValue() + "&delivery_status=" + Ext.getCmp("delivery_status").getValue());
                 }
             }, '->'
         ,
        {
            xtype: 'displayfield',
            value: '.'
        }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: OrderStore,
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
        items: [frm, WaitDeliver],
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
    //OrderStore.load({ params: { start: 0, limit: 25 } });
});
setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    }
    else if (n >= 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}

function Tomorrow() {
    var d;
    var dt;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                     // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += d.getDate();
    dt = new Date(s);
    dt.setDate(dt.getDate() + 1);
    return dt;                  // 返回日期。
}
