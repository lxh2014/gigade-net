var pageSize = 25;
//出貨查詢Model
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
Ext.define('GIGADE.deliver_master', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'overdue_day', type: 'string' },//逾期天數：出货日期-付款单成立日期。 出货日期当天计算
        { name: 'deliver_id', type: 'string' },//出貨編號
        { name: 'delivery_status', type: 'string' },//出貨狀態
        { name: 'freight_set', type: 'string' },//運送方式：freight_set 1 => '常溫',  2 => '冷凍', 5 => '冷藏'
        { name: 'delivery_code', type: 'string' },//物流單號
        { name: 'delivery_freight_cost', type: 'string' },//物流費
        { name: 'delivery_date', type: 'string' },//出貨時間
        { name: 'arrival_date', type: 'string' },//到貨時間
        { name: 'estimated_delivery_date', type: 'string' },//預計出貨日期
        { name: 'estimated_arrival_date', type: 'string' },//預計到貨日期
        { name: 'estimated_arrival_period', type: 'string' },//預計到貨時段：0 => '不限時', 1 => '12:00以前',2 => '12:00-17:00', 3 => '17:00-20:00'
        { name: 'delivery_name', type: 'string' },//收貨人
        { name: 'order_id', type: 'string' },//訂單號
        { name: 'delivery_store', type: 'string' },//物流商
        { name: 'vendor_name_simple', type: 'string' },//出貨商名稱
        { name: 'order_status', type: 'string' },//訂單狀態
        { name: 'logisticsTypes', type: 'string' },//物流状态
        { name: 'buy_num', type: 'string' },
        { name: 'product_name', type: 'string' },
        { name: 'order_date', type: 'string' },
        { name: 'order_payment', type: 'string' },
        { name: 'item_id', type: 'string' },
        { name: 'product_mode', type: 'string' },
        { name: 'slave_status', type: 'string' },
        { name: 'detail_status', type: 'string' },
        { name: 'dvendor_name_simple', type: 'string' },
        { name: 'deliver_master_date', type: 'string' },
        { name: 'note_order', type: 'string' },
        { name: 'note_admin', type: 'string' }
    ]
});

//出貨查詢列表Store
var DeliversListStore = Ext.create("Ext.data.Store", {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'GIGADE.deliver_master',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        timeout: 1800000,
        url: '/ReportManagement/GetDeliversList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
//加載前
DeliversListStore.on('beforeload', function () {
    var order = Ext.getCmp("order_day");
    if (!order.isValid()) {
        Ext.Msg.alert("提示", "請輸入正確的距離預計到貨日");
        return;
    } else {
        Ext.apply(DeliversListStore.proxy.extraParams, {
            order_id: Ext.getCmp("order_id").getValue(),
            deliver_id: Ext.getCmp("deliver_id").getValue(),
            shipment_id: Ext.getCmp("shipment").getValue(),
            order_status_id: Ext.getCmp("order_id_status").getValue(),
            slave_status_id: Ext.getCmp("slave_id_status").getValue(),
            detail_status_id: Ext.getCmp("detail_id_status").getValue(),
            payment_id: Ext.getCmp("payment_id").getValue(),
            dateStart: Ext.getCmp("dateStart").getValue(),
            dateEnd: Ext.getCmp("dateEnd").getValue(),
            logistics_type: Ext.getCmp("logistics_type").getValue(),
            delivery_status: Ext.getCmp("delivery_status").getValue(),
            product_mode: Ext.getCmp("product_mode").getValue(),
            serch_msg: Ext.getCmp("serch_msg").getValue(),
            serch_where: Ext.getCmp("serch_where").getValue(),
            t_days: Ext.getCmp("t_days").getValue(),
            serch_time: Ext.getCmp("serch_time").getValue(),
            order_day: Ext.getCmp("order_day").getValue()

        })
    }

});

function Tomorrow() {
    var d = new Date();                             // 创建 Date 对象。
    d.setDate(d.getDate() + 1);
    return d;                                 // 返回日期。
}
function TheMonthFirstDay() {
    var times;
    times = new Date();
    return new Date(times.getFullYear(), times.getMonth(), 1);
}

Ext.define("gigade.serchmodel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'serch_id', type: 'string' },
        { name: 'serch_name', type: 'string' }
    ]
});

var SerchModelStore = Ext.create('Ext.data.Store', {
    model: 'gigade.serchmodel',
    autoLoad: true,
    data: [
        { serch_id: '1', serch_name: '供應商編號' },
        { serch_id: '2', serch_name: '供應商簡稱' }
    ]
});

Ext.define("gigade.timetype", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'time_id', type: 'string' },
        { name: 'time_name', type: 'string' }
    ]
});

var TimeModelStore = Ext.create('Ext.data.Store', {
    model: 'gigade.timetype',
    autoLoad: true,
    data: [
        { time_id: '1', time_name: '所有日期' },
        { time_id: '2', time_name: '付款單日期' }
    ]
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

Ext.define("gigade.productmode", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'parameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
});
//出貨方式
var ProductModeStore = Ext.create("Ext.data.Store", {
    model: 'gigade.productmode',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/OrderManage/QueryPara?paraType=product_mode',
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
var SlaveStatusStore = Ext.create("Ext.data.Store", {
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
var DetailStatusStore = Ext.create("Ext.data.Store", {
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
Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 175,
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
                    labelWidth: 70,
                    listeners: {
                        specialkey: function (field, e) {
                            if (e.getKey() == e.ENTER) {
                                Query();
                            }
                        }
                    }
                },
                {
                    xtype: 'textfield',
                    id: 'deliver_id',
                    margin: '0 20px',
                    labelWidth: 70,
                    fieldLabel: "出貨單號",
                    name: 'deliver_id',
                    hidden:true,
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
                    value: "-1",
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
                        id: 'slave_id_status',//訂單狀態
                        margin: '0 20px',
                        labelWidth: 70,
                        fieldLabel: '訂單狀態',
                        editable: false,
                        queryMode: 'local',
                        store: SlaveStatusStore,
                        displayField: 'remark',
                        valueField: 'parameterCode',
                        value: "-1",
                        listeners: {
                            beforerender: function () {
                                SlaveStatusStore.load({
                                    callback: function () {
                                        SlaveStatusStore.insert(0, { parameterCode: '-1', remark: '全部' });
                                        Ext.getCmp("slave_id_status").setValue(SlaveStatusStore.data.items[0].data.parameterCode);
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
                          xtype: 'combobox',
                          id: 'order_id_status',
                          margin: '0 20px',
                          labelWidth: 70,
                          fieldLabel: '付款單狀態',//付款單狀態
                          editable: false,
                          queryMode: 'local',
                          store: OrderStatusStore,
                          displayField: 'remark',
                          valueField: 'parameterCode',
                          value: "-1",
                          listeners: {
                              beforerender: function () {
                                  OrderStatusStore.load({
                                      callback: function () {
                                          OrderStatusStore.insert(0, { parameterCode: '-1', remark: '全部' });
                                          Ext.getCmp("order_id_status").setValue(OrderStatusStore.data.items[0].data.parameterCode);
                                      }
                                  });
                              }
                          }
                      },
                  {
                      xtype: 'combobox',
                      id: 'detail_id_status',
                      margin: '0 20px',
                      labelWidth: 70,
                      fieldLabel: '商品狀態',//付款單狀態
                      editable: false,
                      queryMode: 'local',
                      store: DetailStatusStore,
                      displayField: 'remark',
                      valueField: 'parameterCode',
                      value: "-1",
                      listeners: {
                          beforerender: function () {
                              DetailStatusStore.load({
                                  callback: function () {
                                      DetailStatusStore.insert(0, { parameterCode: '-1', remark: '全部' });
                                      Ext.getCmp("detail_id_status").setValue(DetailStatusStore.data.items[0].data.parameterCode);
                                  }
                              });
                          }
                      }
                  }
                    ,
                       {
                           xtype: 'numberfield',
                           labelWidth: 135,
                           margin: '0 20px',
                           fieldLabel: '距離壓單日(大於等於)',
                           minValue: 0,
                           value: 0,
                           allowDecimals: false,
                           id: 't_days',
                           listeners: {
                               specialkey: function (field, e) {
                                   if (e.getKey() == e.ENTER) {
                                       Query();
                                   }
                               }
                           }
                       }//距離壓單日
                  ]
              },



                        {
                            xtype: 'fieldcontainer',
                            combineErrors: true,
                            layout: 'hbox',
                            items: [
                                {
                                    xtype: 'combobox',
                                    id: 'product_mode',
                                    margin: '0 20px',
                                    labelWidth: 70,
                                    fieldLabel: '出貨方式',
                                    editable: false,
                                    queryMode: 'local',
                                    store: ProductModeStore,
                                    displayField: 'parameterName',
                                    valueField: 'parameterCode',
                                    value: "0",
                                    listeners: {
                                        beforerender: function () {
                                            ProductModeStore.load({
                                                callback: function () {
                                                    ProductModeStore.insert(0, { parameterCode: '0', parameterName: '全部' });
                                                    Ext.getCmp("product_mode").setValue(ProductModeStore.data.items[0].data.parameterCode);
                                                }
                                            });
                                        }
                                    }
                                }
                                ,
                                {
                                    xtype: 'combobox',
                                    id: 'serch_msg',
                                    margin: '0 20px',
                                    labelWidth: 70,
                                    name: 'serch_msg',
                                    fieldLabel: '查詢條件',
                                    colName: 'serch_msg',
                                    queryMode: 'local',
                                    editable: false,
                                    store: SerchModelStore,
                                    emptyText: "請選擇",
                                    displayField: 'serch_name',
                                    valueField: 'serch_id',
                                    typeAhead: true,
                                    forceSelection: false
                                },
                                 {
                                     xtype: 'textfield',
                                     labelWidth: 60,
                                     margin: '0 20px',
                                     allowBlank: true,
                                     id: 'serch_where',
                                     listeners: {
                                         specialkey: function (field, e) {
                                             if (e.getKey() == e.ENTER) {
                                                 Query();
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
                         xtype: 'combobox',
                         id: 'serch_time',
                         margin: '0 20px',
                         labelWidth: 70,
                         name: 'serch_time',
                         fieldLabel: '日期條件',
                         colName: 'serch_time',
                         queryMode: 'local',
                         editable: false,
                         store: TimeModelStore,
                         displayField: 'time_name',
                         valueField: 'time_id',
                         value:2,
                         typeAhead: true,
                         forceSelection: false
                     },
                    {
                        xtype: 'datefield',
                        id: 'dateStart',
                        name: 'dateStart',
                        margin: '0 0 0 20px',
                        labelWidth: 70,
                        editable: false,
                      
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
                        xtype: 'datefield',
                        id: 'dateEnd',
                        name: 'dateEnd',
                        editable: false,
                        value: Tomorrow(),
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
                    },
                    {
                        xtype: 'numberfield',
                        id: 'order_day',
                        fieldLabel: "距離預計到貨日",
                        name: 'order_day',
                        allowBlank: true,
                        hideTrigger: true,
                        minValue:0,
                        allowDecimals:false,
                        margin: '0 0 0 10',
                        labelWidth: 90,
                        value: 0,
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Query();
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
                        iconCls: 'ui-icon ui-icon-reset',
                        id: 'btn_reset',
                        listeners: {
                            click: function () {
                                Ext.getCmp('order_id').setValue("");
                                Ext.getCmp('deliver_id').setValue("");
                                Ext.getCmp('shipment').setValue(0);
                                Ext.getCmp('order_id_status').setValue(-1);
                                Ext.getCmp('slave_id_status').setValue(-1);
                                Ext.getCmp('detail_id_status').setValue(-1);
                                Ext.getCmp('payment_id').setValue(0);
                                Ext.getCmp('logistics_type').setValue(0);
                                Ext.getCmp('dateStart').setValue(new Date(Tomorrow().setMonth(Tomorrow().getMonth() - 1)));
                                Ext.getCmp('dateEnd').setValue(Tomorrow());
                                Ext.getCmp("delivery_status").setValue(-1);
                                Ext.getCmp("product_mode").setValue(0);
                                Ext.getCmp("serch_msg").setValue(null);
                                Ext.getCmp("serch_where").setValue("");
                                Ext.getCmp("t_days").setValue(0);
                                Ext.getCmp("serch_time").setValue(2);
                                Ext.getCmp("order_day").setValue(0);
                            }
                        }
                    }
                ]
            }
        ]
    });

    //頁面加載時創建grid
    var VendorListGrid = Ext.create('Ext.grid.Panel', {
        id: 'VendorListGrid',
        store: DeliversListStore,
        flex: 8.0,
        columnLines: true,
        frame: true,
        columns: [
            {
                header: '距離壓單日', dataIndex: 'overdue_day', width: 65, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.order_status == 99) {
                        return "<div style='color:#AAAAEE;'> " + value + "</div>";
                    } else if (record.data.order_status == 4 && record.data.delivery_status == 3) {
                        return "<div style='color:#EEEEEE;'> " + value + "</div>";
                    } else if (record.data.order_status == 90 && record.data.delivery_status == 6) {
                        return "<div style='color:#EEAAAA;'> " + value + "</div>";
                    } else if (record.data.overdue_day > 0) {
                        return "<div style='background-color:#FFAAAA;'> " + value + "</div>";
                    } else if (record.data.overdue_day < 1 && record.data.overdue_day > -3) {
                        return "<div style='background-color:#FFFFAA;color:#FF0000;'> " + value + "</div>";
                    } else {
                        return "<div style='color:#888888;'> " + value + "</div>";
                    }
                }
            },
            { header: '出貨單號', dataIndex: 'deliver_id', hidden: true, width: 70, align: 'center' },
             { header: '付款單成立日期', dataIndex: 'order_date', width: 110, align: 'center' },
            
            {
                header: '訂單編號', dataIndex: 'order_id', id: 'o_id', width: 90, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//搞定
                    return Ext.String.format("<a id='link" + record.data.order_id + "' href='{0}{1}' target='_blank'>{1}</a>", "http://admin.gigade100.com/order/order_show.php?oid=", record.data.order_id);
                }
            },
            
        {
            header: '出貨商簡稱', dataIndex: 'dvendor_name_simple', width: 110, align: 'center'
        },
            {
                header: '供應商簡稱', dataIndex: 'vendor_name_simple', width: 110, align: 'center'
            },
            {
                header: '運送方式', dataIndex: 'freight_set', width: 60, align: 'center'
            },
            { header: '收貨人', dataIndex: 'delivery_name', width: 60, align: 'center' },
            { header: '細項編號', dataIndex: 'item_id', width: 80, align: 'center' },
            { header: '商品名稱', dataIndex: 'product_name', width: 80, align: 'center' },
            { header: '出貨方式', dataIndex: 'product_mode', width: 80, align: 'center' },
            { header: '數量', dataIndex: 'buy_num', width: 70, align: 'center' },
            { header: '物流費', dataIndex: 'delivery_freight_cost',hidden:true, width: 50, align: 'center' },
            { header: '付款方式', dataIndex: 'order_payment', width: 80, align: 'center' },
            { header: '付款單狀態', dataIndex: 'order_status', width: 80, id: 'order_s', align: 'center' },
            { header: '訂單狀態', dataIndex: 'slave_status', width: 80, id: 'slave_s', align: 'center' },
            { header: '商品狀態', dataIndex: 'detail_status', width: 80, id: 'detail_s', align: 'center' },
             { header: '訂單備註', dataIndex: 'note_order', width: 80, align: 'center' },
            { header: '管理員備註', dataIndex: 'note_admin', width: 100, align: 'center' },

            
            {
                header: '出貨單狀態', dataIndex: 'delivery_status', width: 80, align: 'center'
            },
            { header: '物流狀態', dataIndex: 'logisticsTypes', width: 70, align: 'center' },
            {
                header: '可出貨日期', dataIndex: 'deliver_master_date', width: 110, align: 'center'
            },
            {
                header: '預計到貨時段', dataIndex: 'estimated_arrival_period', width: 86, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.estimated_arrival_period == 0) {
                        return "";
                    } else if (record.data.estimated_arrival_period == 1) {
                        return "12:00以前";
                    } else if (record.data.estimated_arrival_period == 2) {
                        return "12:00-17:00";
                    } else if (record.data.estimated_arrival_period == 3) {
                        return "17:00-20:00";
                    } else {
                        return record.data.estimated_arrival_period;
                    }
                }
            },
            {
                header: '預計出貨日期', dataIndex: 'estimated_delivery_date', width: 110, align: 'center'//最近出貨時間
            },
            {
                header: '預計到貨日期', dataIndex: 'estimated_arrival_date', width: 110, align: 'center'//貨物運達時間
            },

            {
                header: '出貨時間', dataIndex: 'delivery_date', width: 110, align: 'center'
            },
            {
                header: '到貨時間', dataIndex: 'arrival_date', width: 110, align: 'center'
            },
            
            { header: '物流單號', dataIndex: 'delivery_code', width: 100, align: 'center' },
              { header: '物流業者', dataIndex: 'delivery_store', width: 90, align: 'center' }
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        tbar: [
         {
             xtype: 'button',
             text: '匯出EXCEL',
             iconCls: 'icon-excel',
             hidden:true,
             id: 'btnExcel',
             handler: function () {
                 var order = Ext.getCmp("order_day");
                 if (!order.isValid()) {
                     Ext.Msg.alert("提示", "請輸入正確的距離預計到貨日");
                     return;
                 } else {
                     window.open("/ReportManagement/GetDeliversExcelList?order_id=" +
                         Ext.getCmp('order_id').getValue() + "&deliver_id=" + Ext.getCmp('deliver_id').getValue() +
                         "&shipment_id=" + Ext.getCmp('shipment').getValue() + "&order_status_id=" + Ext.getCmp('order_id_status').getValue() +
                         "&payment_id=" + Ext.getCmp('payment_id').getValue() + "&dateStart=" + Ext.Date.format(Ext.getCmp('dateStart').getValue(), 'Y-m-d') +
                         "&dateEnd=" + Ext.Date.format(Ext.getCmp('dateEnd').getValue(), 'Y-m-d') + "&logistics_type=" + Ext.getCmp("logistics_type").getValue() +
                         "&delivery_status=" + Ext.getCmp("delivery_status").getValue() + "&t_days=" + Ext.getCmp("t_days").getValue() + "&serch_where=" + Ext.getCmp("serch_where").getValue()
                         + "&serch_msg=" + Ext.getCmp("serch_msg").getValue() + "&product_mode=" + Ext.getCmp("product_mode").getValue() + "&slave_status_id=" + Ext.getCmp("slave_id_status").getValue()
                          + "&detail_status_id=" + Ext.getCmp("detail_id_status").getValue() + "&serch_time=" + Ext.getCmp("serch_time").getValue() + "&order_day=" + Ext.getCmp("order_day").getValue()
                         );
                 }
             }
         }, '->'
         ,
        {
        xtype: 'displayfield',
        value: '.'
    }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: DeliversListStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        })
    });
    Ext.create('Ext.Viewport', {
        layout: 'vbox',
        items: [frm, VendorListGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                VendorListGrid.width = document.documentElement.clientWidth;
                this.doLayout();

            }
        }
    });

    ToolAuthority();
    // DeliversListStore.load({ params: { start: 0, limit: 25 } });
})

//function Tomorrow(days) {
//    var d;
//    var s = "";
//    d = new Date();                             // 创建 Date 对象。
//    s += d.getFullYear() + "/";                     // 获取年份。
//    s += (d.getMonth() + 1) + "/";              // 获取月份。
//    s += d.getDate() + days;                          // 获取日。
//    return (new Date(s));                                 // 返回日期。
//}
setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    }
    else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}
//查询
Query = function () {
    var order = Ext.getCmp("order_day");
    if (!order.isValid()) {
        Ext.Msg.alert("提示", "請輸入正確的距離預計到貨日");
        return;
    } else {
        DeliversListStore.removeAll();
        Ext.getCmp("VendorListGrid").store.loadPage(1, {
            params: {
                order_id: Ext.getCmp("order_id").getValue(),
                deliver_id: Ext.getCmp("deliver_id").getValue(),
                shipment_id: Ext.getCmp("shipment").getValue(),
                order_status_id: Ext.getCmp("order_id_status").getValue(),
                slave_status_id: Ext.getCmp("slave_id_status").getValue(),
                detail_status_id: Ext.getCmp("detail_id_status").getValue(),
                payment_id: Ext.getCmp("payment_id").getValue(),
                dateStart: Ext.getCmp("dateStart").getValue(),
                dateEnd: Ext.getCmp("dateEnd").getValue(),
                logistics_type: Ext.getCmp("logistics_type").getValue(),
                delivery_status: Ext.getCmp("delivery_status").getValue(),
                product_mode: Ext.getCmp("product_mode").getValue(),
                serch_msg: Ext.getCmp("serch_msg").getValue(),
                serch_where: Ext.getCmp("serch_where").getValue(),
                t_days: Ext.getCmp("t_days").getValue(),
                serch_time: Ext.getCmp("serch_time").getValue(),
                order_day: Ext.getCmp("order_day").getValue()
            }
        });
    }
}
