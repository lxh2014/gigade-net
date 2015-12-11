Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
//出貨單狀態 
Ext.define("gigade.paraModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'parameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
});
var deliveryStatusStore = Ext.create('Ext.data.Store', {   
    model: 'gigade.paraModel',
    //fields: ['ParameterCode', 'parameterName'],
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryPara?paraType=delivery_status',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//運送方式
var freightTypeStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        //{ "txt": '全部', "value": "0" },
        { "txt": '常溫', "value": "1" },
        { "txt": '低溫', "value": "2" }
    ]
});
//出貨方式store
//var productModeStore = Ext.create('Ext.data.Store', {
//    fields: ['txt', 'value'],
//    data: [
//        { "txt": '自出', "value": "1" },
//        { "txt": '寄倉', "value": "2" },
//        { "txt": '調度', "value": "3" }
//    ]
//});
var productModeStore = Ext.create('Ext.data.Store', {
    // fields: ['ParameterCode', 'parameterName'],
    model: 'gigade.paraModel',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryPara?paraType=product_mode',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//出貨查詢Model
Ext.define('GIGADE.deliverExpectArrival', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'deliver_id', type: 'int' },//出貨單編號
        { name: 'order_id', type: 'string' },//訂單編號
        { name: 'user_id', type: 'string' },//客戶編號
        { name: 'type', type: 'int' },//出貨方式
        { name: 'freight_set', type: 'int' },//運送方式：freight_set 1 => '常溫',  2 => '冷凍', 5 => '冷藏'
        { name: 'vendor_name_full', type: 'string' },//供應商名稱
        { name: 'delivery_status_str', type: 'string' },//出貨單狀態        
        { name: 'estimated_delivery_date', type: 'string' },//預計出貨日期
        { name: 'deliver_org_days_str', type: 'string' },//預計到貨日期
        { name: 'estimated_arrival_period', type: 'int' },//預計到貨時段：0 => '不限時', 1 => '12:00以前',2 => '12:00-17:00', 3 => '17:00-20:00'
        { name: 'expect_arrive_date', type: 'string' },//期望到貨日
        { name: 'expect_arrive_period', type: 'int' },//期望到貨時段
        
    ]
});
//獲取grid中的數據
var DeliverExpectArrivalStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.deliverExpectArrival',
    proxy: {
        type: 'ajax',
        url: '/SendProduct/GetDeliverExpectArrivalList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
            root: 'data'
        }
    }
});

//加載前
DeliverExpectArrivalStore.on('beforeload', function () {
    Ext.apply(DeliverExpectArrivalStore.proxy.extraParams, {

        productMode: Ext.getCmp('productMode').getValue(),//出貨方式
        freightType: Ext.getCmp('freightType').getValue(),//運送方式
        deliveryStatus: Ext.getCmp('deliveryStatus').getValue(),//出貨單狀態
        deliverId: Ext.htmlEncode(Ext.getCmp('deliverId').getValue().trim()),//出貨單編號
        orderId: Ext.htmlEncode(Ext.getCmp('orderId').getValue().trim()),//訂單編號
        vendorId_ro_name: Ext.htmlEncode(Ext.getCmp('vendorId_ro_name').getValue().trim()),//供應商編號/名稱

        time_start: Ext.getCmp('time_start').getValue(),//預計到貨日（deliver_org_days）--開始時間
        time_end: Ext.getCmp('time_end').getValue(),//結束時間         
    })
});


var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("deliverExpectArrivalGrid").down('#edit').setDisabled(selections.length == 0);               
        }
    }
});

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 140,
        //flex:1.5,
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
                        id: 'productMode',
                        margin: '1 5 0 0',
                        fieldLabel: '出貨方式',
                        labelWidth: 80 ,
                        
                        store: productModeStore,
                        displayField: 'parameterName',
                        valueField: 'parameterCode',
                        editable: false,
                        allowBlank: true,
                        emptyText: '全部',
                        value: -1,
                        listeners: {
                            //change: function (ths, newValue, oldValue, eOpts) {
                            //    alert(Ext.getCmp('productMode').getValue());
      
                            //},
                            specialkey: function (field, e) {
                                if (e.getKey() == Ext.EventObject.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                    {
                        xtype: 'combobox',
                        id: 'freightType',
                        margin: '1 5 0 0',
                        fieldLabel: '運送方式',
                        labelWidth: 75,
                        //colName: 'freightType',
                        queryMode: 'local',
                        editable: false,
                        store: freightTypeStore,
                        displayField: 'txt',
                        valueField: 'value',
                        submitValue: true,
                        emptyText: '全部',
                        value: -1
                        ,
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
                        id: 'deliveryStatus',
                        margin: '1 5 0 0',
                        fieldLabel: '&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp出貨單狀態',
                        labelWidth: 105,
                        //colName: 'deliveryStatus',
                       // queryMode: 'local',
                       // editable: false,
                        store: deliveryStatusStore,
                        displayField: 'parameterName',
                        valueField: 'parameterCode',
                        editable: false,
                        allowBlank: true,
                        emptyText:'全部',
                        value: 10000
                        ,
                        listeners: {
                            //change: function (ths, newValue, oldValue, eOpts) {
                                
                            //},
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
                        xtype: 'textfield',
                        fieldLabel: '出貨單編號',
                        labelWidth: 80,
                        id: 'deliverId',
                        name: 'deliverId',
                        margin: '1 5 0 0',
                        regex: /^[0-9]*$/,
                        regexText:'請輸入0-9數字類型的字符',
                        submitValue: true,
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == Ext.EventObject.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                   {
                       xtype: 'textfield',
                       fieldLabel: '訂單編號',
                       labelWidth: 75,
                       id: 'orderId',
                       name: 'orderId',
                       margin: '1 5 0 0',
                       regex: /^[0-9]*$/,
                       regexText: '請輸入0-9數字類型的字符',
                       submitValue: true,
                       listeners: {
                           specialkey: function (field, e) {
                               if (e.getKey() == Ext.EventObject.ENTER) {
                                   Query();
                               }
                           }
                       }
                   },
                    {
                        xtype: 'textfield',
                        fieldLabel: '供應商編號/名稱',
                        labelWidth: 105,
                        id: 'vendorId_ro_name',
                        name: 'vendorId_ro_name',
                        margin: '1 5 0 0',
                        submitValue: true,
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == Ext.EventObject.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'displayfield',
                        value: '預計到貨日:&nbsp&nbsp&nbsp',
                        margin: '1 5 0 0',
                    },                   
                    {
                        xtype: 'datefield',
                        format: 'Y-m-d',
                        id: 'time_start',
                        name: 'time_start',
                        margin: '1 5 0 0',
                        submitValue: true,
                        editable: false,
                        //value: Tomorrow(),
                        listeners: {
                            select: function (a, b, c) {
                                var start = Ext.getCmp("time_start");
                                var end = Ext.getCmp("time_end");
                                
                                if (start.getValue() != null) {                                    
                                    end.setMinValue(start.getValue());
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
                        value: '~&nbsp&nbsp'
                    },
                    {
                        xtype: 'datefield',
                        format: 'Y-m-d',
                        id: 'time_end',
                        name: 'time_end',
                        margin: '1 5 0 0',
                        submitValue: true,
                        editable: false,
                        listeners: {
                            select: function (a, b, c) {
                                var start = Ext.getCmp("time_start");
                                var end = Ext.getCmp("time_end");                                
                                if (end.getValue() != null) {
                                    start.setMaxValue(end.getValue());
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
                        margin: '5 10 0 50',
                        iconCls: 'icon-search',
                        text: "查詢",
                        handler: Query
                    },
                    {
                        xtype: 'button',
                        margin: '5 10 0 0',
                        text: '重置',
                        id: 'btn_reset',
                        iconCls: 'ui-icon ui-icon-reset',
                        listeners: {
                            click: function () {
                                this.up('form').getForm().reset();
                                //frm.getForm().reset();
                                var datetime1 = new Date();
                                datetime1.setFullYear(2000, 1, 1);
                                var datetime2 = new Date();
                                datetime2.setFullYear(2100, 1, 1);
                                Ext.getCmp("time_start").setMinValue(datetime1);
                                Ext.getCmp("time_start").setMaxValue(datetime2);
                                Ext.getCmp("time_end").setMinValue(datetime1);
                                Ext.getCmp("time_end").setMaxValue(datetime2);
                                
                            }
                        }
                    }
                ]
            }
        ]
        , listeners: {
            //beforerender: function () {
            //    var delivery_statu = document.getElementById("Delivery_Status").value;
            //    var sear = document.getElementById("Search").value;
            //    var type = document.getElementById("Delivery_Type").value;
            //    if (delivery_statu != "") {
            //        Ext.getCmp('shipmentstatus').setValue(delivery_statu);
            //    }
            //    if (sear != "") {
            //        Ext.getCmp('search').setValue(sear);
            //    }
            //    if (type != "") {
            //        Ext.getCmp('shipmenttype').setValue(type);
            //    }
            //    //DeliverStore.load({
            //    //    callback: function () {
            //    //        DeliverStore.insert(0, { parameterCode: '0', parameterName: '全部' });
            //    //        Ext.getCmp("shipment").setValue(DeliverStore.data.items[0].data.parameterCode);
            //    //    }
            //    //});
            //}
        }
    });

   
    //頁面加載時創建grid
    var grid = Ext.create('Ext.grid.Panel', {
        id: 'deliverExpectArrivalGrid',
        store: DeliverExpectArrivalStore,
        // height: 645,
        flex: 8.5,
        columnLines: true,
        frame: true,
        columns: [

            { header: '出貨單編號', dataIndex: 'deliver_id', width: 70, align: 'center' },           
            { header: '訂單編號', dataIndex: 'order_id', width: 90, align: 'center' },
            { header: '客戶編號', dataIndex: 'user_id', width: 90, align: 'center' },
            {
                header: '出貨方式', dataIndex: 'type', width: 90, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (record.data.type) {
                        case 1:
                            return "寄倉";
                            break;
                        case 2:
                            return "自出";
                            break;
                        case 101:
                            return "調度";
                            break;
                        default:
                            return record.data.type;
                            break;
                    }
                }
            },
            {
                header: '運送方式', dataIndex: 'freight_set', width: 90, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (record.data.freight_set) {
                        case 1:
                            return "常溫";
                            break;
                        case 2:
                            return "低溫";
                            break;                      
                        default:
                            return "意外數據錯誤";
                            break;
                    }
                }
            },
            { header: '供應商名稱', dataIndex: 'vendor_name_full', width: 150, align: 'center' },
            {
                header: '出貨單狀態', dataIndex: 'delivery_status_str', width: 90, align: 'center'
                //,
                //renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                //    switch (record.data.delivery_status) {
                //        case 0:
                //            return "待出貨";
                //            break;
                //        case 1:
                //            return "可出貨";
                //            break;
                //        case 2:
                //            return "出貨中";
                //            break;
                //        case 3:
                //            return "已出貨";
                //            break;
                //        case 4:
                //            return "已到貨";
                //            break;
                //        case 5:
                //            return "未到貨";
                //            break;
                //        case 6:
                //            return "取消出貨";
                //            break;
                //        case 7:
                //            return "待取貨";
                //            break;
                //        default:
                //            return "意外數據錯誤";
                //            break;
                //    }
                //}
            },
            {
                header: '預計出貨日', dataIndex: 'estimated_delivery_date', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value.substr(0, 10) == "0001-01-01") {
                        return "";
                    }
                    else {
                        return value.substr(0, 10);
                    }
                }
            },
            {
                header: '預計到貨日', dataIndex: 'deliver_org_days_str', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value.substr(0, 10) == "0001-01-01") {
                        return "";
                    }
                    else {
                        return value.substr(0, 10);
                    }
                }
            },
            {
                header: '預計到貨時段', dataIndex: 'estimated_arrival_period', width: 120, align: 'center',
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
                header: '期望到貨日', dataIndex: 'expect_arrive_date',  width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value.substr(0, 10) == "0001-01-01") {
                        return ""
                    }
                    else {
                        return value.substr(0, 10);
                    }
                }
            },
            {
                header: '期望到貨時段', dataIndex: 'expect_arrive_period', width: 120, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.expect_arrive_period == 0) {
                        return "";
                    } else if (record.data.expect_arrive_period == 1) {
                        return "12:00以前";
                    } else if (record.data.expect_arrive_period == 2) {
                        return "12:00-17:00";
                    } else if (record.data.expect_arrive_period == 3) {
                        return "17:00-20:00";
                    } else {
                        return record.data.expect_arrive_period;
                    }
                }
            },
               
         
           
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
              text: '編輯',
              disabled: true,
              iconCls: 'icon-user-edit',
              id: 'edit',
              handler: onEditClick
          }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: DeliverExpectArrivalStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        selModel: sm
    });
    Ext.create('Ext.Viewport', {
        layout: 'vbox',
        items: [frm, grid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                grid.width = document.documentElement.clientWidth;
                this.doLayout();

            }
        }
    });
    
    //ToolAuthority();
    //DeliversListStore.load({ params: { start: 0, limit: 25 } });
    
})

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
function Tomorrow() {
    var d;
    d = new Date();                             // 创建 Date 对象。                               // 返回日期。
    d.setDate(d.getDate() + 1);
    return d;
}

//查询
Query = function () {
    var falg = 0;
    var productMode = Ext.getCmp('productMode').getValue(); if (productMode != -1) { falg++; }
    var freightType = Ext.getCmp('freightType').getValue(); if (freightType != -1) { falg++; }
    var deliveryStatus = Ext.getCmp('deliveryStatus').getValue(); if (deliveryStatus != 10000) { falg++; }

    var deliverId = Ext.getCmp('deliverId').getValue().trim(); if (deliverId != '') { falg++; }
    var orderId = Ext.getCmp('orderId').getValue().trim(); if (orderId != '') { falg++; }
    var vendorId_ro_name = Ext.getCmp('vendorId_ro_name').getValue().trim(); if (vendorId_ro_name != '') { falg++; }

    var time_start = Ext.getCmp('time_start').getValue(); if (time_start != null) { falg++; }
    var time_end = Ext.getCmp('time_end').getValue(); if (time_end != null) { falg++; }

    DeliverExpectArrivalStore.removeAll();
    if (falg == 0) {
        Ext.Msg.alert("提示", "請輸入查詢條件");
        return false;
    }
    if (time_start != null && time_end == null) {
        Ext.Msg.alert("提示", "請選擇結束時間");
        return false;
    }
    if (time_end != null && time_start == null) {
        Ext.Msg.alert("提示", "請選擇開始時間");
        return false;
    }
    
    
   
    
    Ext.getCmp("deliverExpectArrivalGrid").store.loadPage(1, {
        params: {
            productMode: Ext.getCmp('productMode').getValue(),//出貨方式
            freightType: Ext.getCmp('freightType').getValue(),//運送方式
            deliveryStatus: Ext.getCmp('deliveryStatus').getValue(),//出貨單狀態
            deliverId: Ext.htmlEncode(Ext.getCmp('deliverId').getValue().trim()),//出貨單編號
            orderId: Ext.htmlEncode(Ext.getCmp('orderId').getValue().trim()),//訂單編號
            vendorId_ro_name: Ext.htmlEncode(Ext.getCmp('vendorId_ro_name').getValue().trim()),//供應商編號/名稱

            time_start: Ext.getCmp('time_start').getValue(),//預計到貨日（estimated_arrival_date）--開始時間
            time_end:Ext.getCmp('time_end').getValue(),//結束時間 
        }
    });
}
//編輯
onEditClick = function () {
    var row = Ext.getCmp("deliverExpectArrivalGrid").getSelectionModel().getSelection();
   
    var deliver_id = row[0].data.deliver_id;
    
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {     
        Ext.Ajax.request({
            url: '/SendProduct/isCanModifyExpectArriveDate',
            method: "POST",
            params: { deliver_id: deliver_id },
            success: function (form, action) {
                var data = Ext.decode(form.responseText);
                if (data.msg == "0") {                                      
                    Ext.Msg.alert("提示信息", "該出貨單不容許修改期望到貨日");                   
                }
                else {
                    editFunction(row[0], DeliverExpectArrivalStore);
                }
            },
            failure: function (form, action) {
                result = false;
                var data = Ext.decode(form.responseText);
                Ext.Msg.alert(data.msg);
            }   
        })       
    }
}







