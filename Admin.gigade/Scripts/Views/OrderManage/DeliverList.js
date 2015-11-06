//物流單頁面
Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 23;
/*********************物流單主頁面*******************************/
//物流單Model
Ext.define('gigade.Fares', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "order_id", type: "int" }, //付款單號
        { name: "slave_id", type: "string" },
        { name: "vendor_name_simple", type: "string" }, //供應商名稱
        { name: "deliver_store", type: "string" }, //物流商
        { name: "deliver_code", type: "string" },   //物流單號
        { name: "delivertime", type: "string" },         //出貨時間
        { name: "delivercre", type: "string" },          //填單時間
        { name: "deliverup", type: "string" },          //填單時間
        { name: "deliver_note", type: "string" }    //備註
    ]
});
var OrderDeliverStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.Fares',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetOrderDeliverInfo',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
////使用者Model
//Ext.define('gigade.ManageUser', {
//    extend: 'Ext.data.Model',
//    fields: [
//        { name: "name", type: "string" },
//        { name: "callid", type: "string" }]
//});
//var ManageUserStore = Ext.create('Ext.data.Store', {
//    autoDestroy: true,
//    model: 'gigade.ManageUser',
//    proxy: {
//        type: 'ajax',
//        url: '/Fgroup/QueryCallid',
//        reader: {
//            type: 'json',
//            root: 'items'
//        }
//    }
//});
var DDLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有訂單資料", "value": "0" },
        { "txt": "付款單號", "value": "1" },
        { "txt": "廠商出貨單號", "value": "2" },
        { "txt": "物流單號", "value": "3" }
    ]
});
var DTStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有日期", "value": "0" },
        { "txt": "出貨日期", "value": "1" }
    ]
});
OrderDeliverStore.on('beforeload', function () {
    Ext.apply(OrderDeliverStore.proxy.extraParams,
        {
            serchs: Ext.getCmp('serchs').getValue(),
            orderid: Ext.getCmp('orderid').getValue(),
            seldate: Ext.getCmp('seldate').getValue(),
            selven: Ext.getCmp('selven').getValue(),
            deliverstart: Ext.getCmp('deliverstart').getValue(),
            deliverend: Ext.getCmp('deliverend').getValue()
        });
});
function Query(x) {
    if (Ext.getCmp("seldate").getValue() == 1) {
        var start = Ext.getCmp("deliverstart");
        var end = Ext.getCmp("deliverend");
        var s_date = new Date(start.getValue());
        var datespara = new Date(s_date.setMonth(s_date.getMonth() + 2));
        if (datespara < end.getValue()) {
            Ext.Msg.alert("提示", "請選擇日期在兩個月之內！");
            return;
        }
    }
    var conten = Ext.getCmp('orderid');
  
    if (!conten.isValid()) {
        Ext.Msg.alert("提示", "查詢內容格式錯誤!");
        return;
    }
    if (Ext.getCmp("serchs").getValue() != 0 && Ext.getCmp("orderid").getValue().trim() == "") {
        Ext.Msg.alert("提示", "請輸入查詢內容！");
        return;
    }
    OrderDeliverStore.removeAll();
    Ext.getCmp("gdFgroup").store.loadPage(1, {
        params: {
            serchs: Ext.getCmp('serchs').getValue(),
            orderid: Ext.getCmp('orderid').getValue(),
            seldate: Ext.getCmp('seldate').getValue(),
            selven: Ext.getCmp('selven').getValue(),
            deliverstart: Ext.getCmp('deliverstart').getValue(),
            deliverend: Ext.getCmp('deliverend').getValue()
        }
    });
}

function DeliverShow(val) {//對應供應商寫死的
    switch (val) {
        case "1":
            return "黑貓宅急便";
            break;
        case "2":
            return "郵局";
            break;
        case "3":
            return "大榮貨運";
            break;
        case "4":
            return "宅配通";
            break;
        case "5":
            return "新竹貨運";
            break;
        case "6":
            return "通盈通運";
            break;
        case "10":
            return "黑貓貨到付款";
            break;
        case "11":
            return "7-11取貨(Yahoo)";
            break;
        case "12":
            return "自取(松山)";
            break;
        case "13":
            return "自取(板橋)";
            break;
        case "14":
            return "自取(永和)";
            break;
        case "15":
            return "自取(桃園)";
            break;
        case "29":
            return "台灣便利配（斗牛ezShip）";
            break;
        case "30":
            return "得比速國際快遞";
            break;
        case "31":
            return "豐業快遞";
            break;
        case "32":
            return "全球快遞";
            break;
        case "40":
            return "公司自送";
            break;
        case "41":
            return "便利袋";
            break;
        case "99":
            return "其它";
            break;
        case "0":
            return "請選擇";
            break;
        default:
            return "其它";
            break;
    }
}
Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        height: 100,
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
                              id: 'serchs',
                              editable: false,
                              fieldLabel: "查詢條件",
                              labelWidth: 59,
                              store: DDLStore,
                              displayField: 'txt',

                              valueField: 'value',
                              value: 0
                          },
                          {
                              xtype: 'textfield',
                              id: 'orderid',
                              fieldLabel: "查詢內容",
                              margin: '0 0 0 10',
                              labelWidth: 59,
                              regex: /^((?!%).)*$/,
                              regexText: "禁止輸入百分號",
                              listeners: {
                                  specialkey: function (field, e) {
                                      if (e.getKey() == Ext.EventObject.ENTER) {
                                          Query();
                                      }
                                  }
                              }
                          },
                          {
                              xtype: 'combobox',
                              id: 'selven',
                              name: 'selven',
                              fieldLabel: '供應商',
                              store: VendorStore,
                              margin: '0 0 0 10',
                              queryMode: 'local',
                              displayField: 'vendor_name_simple',
                              editable: false,
                              typeAhead: true,
                              forceSelection: false,
                              valueField: 'vendor_id',
                              listeners: {
                                  beforerender: function () {
                                      VendorStore.load({
                                          callback: function () {
                                              VendorStore.insert(0, { vendor_id: '0', vendor_name_simple: '所有供應商資料' });
                                              Ext.getCmp("selven").setValue(VendorStore.data.items[0].data.vendor_id);//
                                          }
                                      });
                                  }
                              }
                          }]
             },
              {
                  xtype: 'fieldcontainer',
                  combineErrors: true,
                  layout: 'hbox',
                  items: [
                       {
                           xtype: 'combobox',
                           id: 'seldate',
                           editable: false,
                           fieldLabel: "日期條件",
                           labelWidth: 59,
                           store: DTStore,
                           displayField: 'txt',
                           valueField: 'value',
                           editable: false,
                           value: 1
                       },
                       {
                           xtype: "datefield",
                           id: 'deliverstart',
                           fieldLabel: "",
                           labelWidth: 40,
                           name: 'deliverstart',
                           format: 'Y-m-d',
                           submitValue: true,
                           editable: false,
                           margin: '0 0 0 10',
                           value: new Date(Today().setMonth(Today().getMonth() - 1)),
                           listeners: {
                               select: function (a, b, c) {
                                   var start = Ext.getCmp("deliverstart");
                                   var end = Ext.getCmp("deliverend");
                                   var s_date = new Date(end.getValue());
                                   if (end.getValue() < start.getValue()) {
                                       Ext.Msg.alert("提示", "開始時間不能大於結束時間！");
                                       start.setValue(new Date(s_date.setMonth(s_date.getMonth() - 1)));
                                   }
                               },
                               specialkey: function (field, e) {
                                   if (e.getKey() == Ext.EventObject.ENTER) {
                                       Query();
                                   }
                               }
                           }
                       },
                       {
                           xtype: 'displayfield',
                           margin: '0 10 0 10',
                           value: "~"
                       },
                       {
                           xtype: "datefield",
                           format: 'Y-m-d',
                           id: 'deliverend',
                           editable: false,
                           name: 'deliverend',
                           value: Today(),
                           listeners: {
                               select: function (a, b, c) {
                                   var start = Ext.getCmp("deliverstart");
                                   var end = Ext.getCmp("deliverend");
                                   var s_date = new Date(start.getValue());
                                   if (end.getValue() < start.getValue()) {
                                       Ext.Msg.alert("提示", "開始時間不能大於結束時間！");
                                       end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                                   }
                               },
                               specialkey: function (field, e) {
                                   if (e.getKey() == Ext.EventObject.ENTER) {
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
                    margin: '0 0 0 10',
                    text: SEARCH,
                    iconCls: 'icon-search',
                    id: 'btnQuery',
                    handler: Query
                },
                        {
                            xtype: 'button',
                            text: RESET,
                            margin: '0 0 0 10',
                            id: 'btn_reset',
                            iconCls: 'ui-icon ui-icon-reset',
                            listeners: {
                                click: function () {
                                    Ext.getCmp("serchs").setValue(0);
                                    Ext.getCmp("orderid").setValue("");
                                    Ext.getCmp("selven").setValue(0);
                                    Ext.getCmp("seldate").setValue(1);
                                    Ext.getCmp('deliverstart').setValue(new Date(Today().setMonth(Today().getMonth() - 1)));
                                    Ext.getCmp('deliverend').setValue(Today());
                                }
                            }
                        }]
             }
        ]
    });
    var gdFgroup = Ext.create('Ext.grid.Panel', {
        id: 'gdFgroup',
        store: OrderDeliverStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        flex: 8.1,
        frame: true,
        columns: [
            {
                header: "付款單號", dataIndex: 'order_id', width: 120, align: 'center',
                //renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                //    return '<a href=javascript:TransToOrder(' + record.data.order_id + ') >' + record.data.order_id + '</a>';
                //}
            },
            { header: "廠商出貨單號", dataIndex: 'slave_id', width: 100, align: 'center' },
            { header: "供應商", dataIndex: 'vendor_name_simple', width: 120, align: 'center' },
            { header: "物流商", dataIndex: 'deliver_store', width: 80, align: 'center', renderer: DeliverShow },
            { header: "物流單號", dataIndex: 'deliver_code', width: 120, align: 'center' },
            { header: "出貨時間", dataIndex: 'delivertime', width: 120, align: 'center' },
            { header: "備註", dataIndex: 'deliver_note', width: 180, align: 'center' },
            {
                header: "填單時間", dataIndex: 'deliverup', width: 120, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == '1970-01-01 08:00:00') {
                        return record.data.delivercre;
                    }
                    else {
                        return record.data.deliverup;
                    }
                }
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: OrderDeliverStore,
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
        items: [frm, gdFgroup],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdFgroup.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //OrderDeliverStore.load(
    //    {
    //        params: { start: 0, limit: 25 }
    //    });
});
function TransToOrder(orderId) {
    var url = '/OrderManage/OrderDetialList?Order_Id=' + orderId;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#DeliverList');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'DeliverList',
        title: '訂單內容',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();

}