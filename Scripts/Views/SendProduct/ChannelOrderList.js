Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
CateStore.load();
ChannelStore.load();
DeliverStore.load();
GetGoodsWayStore.load();

var vendorStore;
var channelStore;
var deliverStore ;
var goodsWayStore;
var ConditionStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": '所有資料', "value": "0" },
        { "txt": '付款單號', "value": "1" },
        { "txt": '賣場訂單編號', "value": "2" },
        { "txt": '外站子單', "value": "3" }
    ]
});
//出货日期
var datequeryStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": '所有日期', "value": "0" },
        { "txt": '出貨日期', "value": "1" }
    ]
});

//出貨查詢Model
Ext.define('GIGADE.deliver_master', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'od_order_id', type: 'int' },
        { name: 'deliver_id', type: 'int' },//出貨編號
        { name: 'delivery_status', type: 'int' },//出貨狀態
        { name: 'freight_set', type: 'int' },//運送方式：freight_set 1 => '常溫',  2 => '冷凍', 5 => '冷藏'
        { name: 'export_id', type: 'int' },//出貨商編號
        { name: 'delivery_store', type: 'string' },//物流業者
        { name: 'delivery_code', type: 'string' },//物流單號
        { name: 'delivery_date', type: 'string' },//出貨時間
        { name: 'channel', type: 'string' },//
        { name: 'channel_order_id', type: 'string' },//
        { name: 'sub_order_id', type: 'string' },//
        { name: 'dd_status', type: 'int' },
        { name: 'retrieve_mode', type: 'int' },
        { name: 'sretrieve_mode', type: 'string' },
        { name: 'sdelivery_store', type: 'string' },//物流業者
        { name: 'schannel', type: 'string' },
        { name: 'sexport_id', type: 'string' }
    ]
});
//出貨查詢列表Store
var ChannelOrderStore = Ext.create("Ext.data.Store", {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'GIGADE.deliver_master',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/SendProduct/GetChannelOrderList',
        timeout:360000,
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
//加載前
ChannelOrderStore.on('beforeload', function () {
    Ext.apply(ChannelOrderStore.proxy.extraParams, {
        condition: Ext.getCmp('condition').getValue(),//出貨類別--type
        content: Ext.getCmp('content').getValue(),//出貨狀態--delivery_status
        channel: Ext.getCmp('channel').getValue(),//賣場
        delivery_store: Ext.getCmp('deliverystore').getValue(),//物流商-- delivery_store
        delivery_status: Ext.getCmp('status').getValue(),//出貨單狀態
        retrieve_mode: Ext.getCmp('goodsway').getValue(),//取貨模式
        datequery: Ext.getCmp('datequery').getValue(),//日期條件 --datequery
        time_start: Ext.getCmp('time_start').getValue(),//開始時間--time_start--delivery_date
        time_end: Ext.getCmp('time_end').getValue()//結束時間--time_end--delivery_date
});
});

Ext.onReady(function () {   
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        //flex: 1.8,
        //border: 0,
        layout: 'anchor',
        height: 150,
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
                        id: 'condition',
                        margin: '0 5px',
                        fieldLabel: '查詢條件',
                        queryMode: 'local',
                        editable: false,
                        store: ConditionStore,
                        displayField: 'txt',
                        valueField: 'value',
                        value: 0
                    },
                    {
                        xtype: 'textfield',
                        id: 'content',
                        name: 'content',
                        margin: '0 5px',
                        fieldLabel: '查詢內容'
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
                        id: 'channel',
                        name: 'channel',
                        margin: '0 5px',
                        fieldLabel: '賣場名稱',
                        queryMode: 'local',
                        editable: false,
                        store: ChannelStore,
                        displayField: 'channel_name_simple',
                        valueField: 'channel_id',
                        value: '0'
                    },
                    {
                        xtype: 'combobox',
                        id: 'deliverystore',
                        margin: '0 5px',
                        fieldLabel: '物流商',
                        queryMode: 'local',
                        editable: false,
                        store: DeliverStore,
                        displayField: 'parameterName',
                        valueField: 'parameterCode',
                        listeners: {
                            afterrender: function () {
                                DeliverStore.load({
                                    callback: function () {
                                        DeliverStore.insert(0, { parameterCode: '0', parameterName: '全部' });
                                        Ext.getCmp("deliverystore").setValue(0);
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
                          id: 'status',
                          margin: '0 5px',
                          fieldLabel: '狀態',
                          queryMode: 'local',
                          editable: false,
                          store: DeliveryStatusStore,
                          displayField: 'txt',
                          valueField: 'value',
                          value: -1
                      },
                      {
                          xtype: 'combobox',
                          id: 'goodsway',
                          margin: '0 5px',
                          fieldLabel: '取貨模式',
                          queryMode: 'local',
                          editable: false,
                          store: GetGoodsWayStore,
                          displayField: 'parameterName',
                          valueField: 'parameterCode',
                          listeners: {
                              afterrender: function () {
                                  GetGoodsWayStore.load({
                                      callback: function () {
                                          GetGoodsWayStore.insert(0, { parameterCode: '-1', parameterName: '全部' });
                                          Ext.getCmp("goodsway").setValue(-1);
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
                        id: 'datequery',
                        margin: '0 5px',
                        fieldLabel: '出貨時間',
                        queryMode: 'local',
                        editable: false,
                        store: datequeryStore,
                        displayField: 'txt',
                        valueField: 'value',
                        value: 0
                    },
                    {
                        xtype: 'datefield',
                        id: 'time_start',
                        name: 'time_start',
                        margin: '0 5px 0 5px',
                        editable: false,
                        value:new Date(new Date().getTime()-1000*60*60*24*5)//當前時間減去五天
                    },
                    {
                        xtype: 'displayfield',
                        value: '~'
                    },
                    {
                        xtype: 'datefield',
                        id: 'time_end',
                        name: 'time_end',
                        margin: '0 5px',
                        editable: false,
                        value: new Date()
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
                        margin: '0 10 0 10',
                        iconCls: 'icon-search',
                        text: "查詢",
                        handler: Query
                    },
                    {
                        xtype: 'button',
                        text: '重置',
                        id: 'btn_reset',
                        iconCls: 'ui-icon ui-icon-reset',
                        listeners: {
                            click: function () {
                                Ext.getCmp('condition').setValue(0);//出貨類別--type
                                Ext.getCmp('content').setValue("");//出貨狀態--delivery_status
                                Ext.getCmp('channel').setValue(0);//出貨廠商--export_id
                                Ext.getCmp('deliverystore').setValue(0);//物流商-- delivery_store
                                Ext.getCmp('status').setValue(-1);//調度狀態--warehouse_status
                                Ext.getCmp('goodsway').setValue(-1);//出貨篩選--OrderMaster`.`priority` = 1
                                Ext.getCmp('datequery').setValue(0);//日期條件 --datequery
                                Ext.getCmp('time_start').setValue(Tomorrow(1 - new Date().getDate()));//開始時間--time_start--delivery_date
                                Ext.getCmp('time_end').setValue(Tomorrow(0));   //結束時間--time_end--delivery_date
                            }
                        }
                    }
                ]
            }
        ]
    });
    //vendorStore = CateStore;
    //channelStore = Ext.getCmp('channel').getStore();
    //deliverStore = Ext.getCmp('deliverystore').getStore();
    //goodsWayStore = Ext.getCmp('goodsway').getStore();
    //頁面加載時創建grid
    var gdChannelOrder = Ext.create('Ext.grid.Panel', {
        id: 'gdChannelOrder',
        store: ChannelOrderStore,
        // height: 645,
        flex: 8.1,
        columnLines: true,
        frame: true,
        columns: [
            {
                header: '付款單號', dataIndex: 'od_order_id', width: 120, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.type == 3) {
                        return record.data.order_id;
                    } else {
                        return '<a href=javascript:TransToOrder(' + record.data.od_order_id + ') >' + record.data.od_order_id + '</a>';
                        //Ext.String.format("<a  href='www.baidu.com?oid={0}' target='_blank' style='text-decoration:none'>{1}</a>", record.data.order_id, record.data.order_id);
                }
                }
            },
            {
                header: '賣場', dataIndex: 'schannel', width: 120, align: 'center'
                //, renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                //    var channelrecord = channelStore.findRecord("channel_id", record.data.channel, 0, false, false, true);
                //    if (!Ext.isEmpty(channelrecord)) {
                //        return channelrecord.data.channel_name_simple;
                //    }

                //}
            },
            { header: '賣場訂單編號', dataIndex: 'channel_order_id', width: 100, align: 'center'},
            { header: '賣場訂單子單', dataIndex: 'sub_order_id', width: 100, align: 'center' },
            {
                header: '出貨商', dataIndex: 'sexport_id', width: 150, align: 'center'
                ,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return '<a href=javascript:TransToView(' + record.data.deliver_id + ') >' +value + '</a>';
                }
            },
            {
                header: '運送方式', dataIndex: 'freight_set', width: 100, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.freight_set == 1) {
                        return "常溫";
                    } else if (record.data.freight_set == 2) {
                        return "低溫";
                    }
                }
            },
        {
            header: '物流業者', dataIndex: 'sdelivery_store', width: 120, align: 'center'
            , renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                //var deliverystorerecord = deliverStore.findRecord("parameterCode", record.data.delivery_store, 0, false, false, true);
                //if (!Ext.isEmpty(deliverystorerecord)) {
                //    return deliverystorerecord.data.parameterName;
                //}
                //else {
                //    return '未配送';
                //}
                if (value == "") {
                    return '未配送';
                }
                else {
                    return value;
                }
            }
            
        },
            {
                header: '取貨模式', dataIndex: 'sretrieve_mode', width: 120, align: 'center'
                //,
                //renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                //    //var goodswaystorerecord = goodsWayStore.findRecord("parameterCode", record.data.retrieve_mode, 0, false, false, true);
                //    //if (!Ext.isEmpty(goodswaystorerecord)) {
                //    //    return goodswaystorerecord.data.parameterName;
                //    //}
                //    switch(value)
                //    {
                //        case 0:
                //            return '同貨運物流業者';
                //            break;
                //        case 1:
                //            return '7-11取貨';
                //            break;

                //    }
                    

                //}
            },
            { header: '物流單號', dataIndex: 'delivery_code', width: 150, align: 'center' },
            {
                header: '出貨時間', dataIndex: 'delivery_date', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "0001-01-01") {
                        return "N/A";
                    } else {
                        return value;
                    }
                }
            },
            {
                header: '出貨狀態', dataIndex: 'dd_status', width: 90, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (value) {
                        case 0:
                            return "待出貨";
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
            }
        ],
        tbar: [
          {
                xtype: 'button', text: '匯出YaHoo出貨檔', id: 'outchannel', hidden: false, handler: function () {
                    var condition = Ext.getCmp('condition').getValue();//出貨類別--type
                    var content = Ext.getCmp('content').getValue();
                    var channel = Ext.getCmp('channel').getValue();//賣場
                    var delivery_store = Ext.getCmp('deliverystore').getValue();//物流商-- delivery_store
                    var delivery_status = Ext.getCmp('status').getValue();//出貨單狀態
                    var retrieve_mode = Ext.getCmp('goodsway').getValue();//取貨模式
                    var datequery = Ext.getCmp('datequery').getValue();//日期條件 --datequery
                    var time_start = Ext.Date.format(Ext.getCmp('time_start').getValue(), 'Y-m-d');//開始時間--time_start--delivery_date
                    var time_end = Ext.Date.format(Ext.getCmp('time_end').getValue(), 'Y-m-d');//結束時間--time_end--delivery_date
                    var paras = "?condition=" + condition + "&content=" + content + "&channel=" + channel + "&delivery_store=" + delivery_store + "&datequery=" + datequery + "&time_start=" + time_start + "&time_end=" + time_end + "&delivery_status=" + delivery_status + "&retrieve_mode=" + retrieve_mode;
                    window.open('/SendProduct/OutChannelOrderCSV' + paras);
              }
          },
          {
                xtype: 'button', text: '出貨明細', id: 'deliverdetails', hidden: true, handler: function (){ }
            }
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
            store: ChannelOrderStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        })
    });

    Ext.create('Ext.Viewport', {
        layout: 'vbox',
        items: [frm, gdChannelOrder],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdChannelOrder.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });

    ToolAuthority();
    ChannelOrderStore.load({ params: { start: 0, limit: 25 } });
})
function Tomorrow(days) {
    var d;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                     // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += d.getDate() + days;                          // 获取日。
    return (new Date(s));                                 // 返回日期。
}

//查询
Query = function () {
    ChannelOrderStore.removeAll();
    Ext.getCmp("gdChannelOrder").store.loadPage(1, {
        params: {
            condition: Ext.getCmp('condition').getValue(),//出貨類別--type
            content: Ext.getCmp('content').getValue(),//出貨狀態--delivery_status
            channel: Ext.getCmp('channel').getValue(),//賣場
            delivery_store: Ext.getCmp('deliverystore').getValue(),//物流商-- delivery_store
            delivery_status: Ext.getCmp('status').getValue(),//出貨單狀態
            retrieve_mode: Ext.getCmp('goodsway').getValue(),//取貨模式
            datequery: Ext.getCmp('datequery').getValue(),//日期條件 --datequery
            time_start: Ext.getCmp('time_start').getValue(),//開始時間--time_start--delivery_date
            time_end: Ext.getCmp('time_end').getValue()//結束時間--time_end--delivery_date

        }
    });
}
function TransToOrder(orderId) {

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
function TransToView(deliverId) {

    var url = '/SendProduct/DeliverView?deliver_id=' + deliverId;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copychannel = panel.down('#channeldetial');
    if (copychannel) {
        copychannel.close();
    }
    copychannel = panel.add({
        id: 'channeldetial',
        title: '出貨明細',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copychannel);
    panel.doLayout();

}
