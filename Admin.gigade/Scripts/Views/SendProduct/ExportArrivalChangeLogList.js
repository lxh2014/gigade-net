Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;

//參數表model
Ext.define("gigade.paraModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'parameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
});


//創建類型store
var createTypeStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [       
        { "txt": '前臺', "value": "1" },
        { "txt": '後臺', "value": "2" }
    ]
});
////管理員store
//var ManageUserStore = Ext.create('Ext.data.Store', {
//    // fields: ['ParameterCode', 'parameterName'],
//    model: 'gigade.paraModel',
//    autoLoad: true,
//    proxy: {
//        type: 'ajax',
//        url: '/Parameter/QueryPara?paraType=product_mode',
//        noCache: false,
//        getMethod: function () { return 'get'; },
//        actionMethods: 'post',
//        reader: {
//            type: 'json',
//            root: 'items'
//        }
//    }
//});

//期望到貨日調整記錄查詢Model
Ext.define('GIGADE.deliveryChangeLog', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'dcl_id', type: 'int' },//出貨單行號（自增）
        { name: 'deliver_id', type: 'int' },//出貨單編號

        { name: 'dcl_create_username', type: 'string' },//創建人名稱（普通會員）
        { name: 'dcl_create_musername', type: 'string' },//創建人名稱（管理員）

        { name: 'dcl_create_user', type: 'string' },//創建人（普通會員）
        { name: 'dcl_create_datetime', type: 'string' },//調整記錄創建日期
        { name: 'dcl_create_muser', type: 'string' },//創建人（管理員）
        { name: 'dcl_create_type', type: 'int' },//創建類型1:前台創建 2:後台創建
        { name: 'dcl_note', type: 'string' },//備註        
        { name: 'dcl_ipfrom', type: 'string' },//來源ip
        { name: 'expect_arrive_date', type: 'string' },//期望到貨日
        { name: 'expect_arrive_period', type: 'int' },//期望到貨時段：0 => '不限時', 1 => '12:00以前',2 => '12:00-17:00', 3 => '17:00-20:00'
             
    ]
});
//獲取grid中的數據
var DeliveryChangeLogStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.deliveryChangeLog',
    proxy: {
        type: 'ajax',
        url: '/SendProduct/GetDeliveryChangeLogList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
            root: 'data'
        }
    }
});

//加載前
DeliveryChangeLogStore.on('beforeload', function () {
    Ext.apply(DeliveryChangeLogStore.proxy.extraParams, {
       
        deliver_id: Ext.htmlEncode(Ext.getCmp('deliverId').getValue().trim()),//出貨單編號
        dcl_create_type: Ext.getCmp('createType').getValue(),

        query_type: Ext.getCmp("query_type").getValue().Tax_Type,
        userName_ro_email: Ext.getCmp('userName_ro_email').getValue(),

        time_start: Ext.getCmp('time_start').getValue(),//預計到貨日（estimated_arrival_date）--開始時間
        time_end: Ext.getCmp('time_end').getValue(),//結束時間       
    })
});


var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            //Ext.getCmp("DeliveryChangeLogGrid").down('#edit').setDisabled(selections.length == 0);
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
                        xtype: 'textfield',
                        fieldLabel: '出貨單編號',
                        labelWidth: 70,
                        id: 'deliverId',
                        name: 'deliverId',
                        margin: '0 5 0 0',
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
                        xtype: 'combobox',
                        id: 'createType',
                        margin: '0 5 0 0',
                        fieldLabel: '創建類型',
                        labelWidth: 60,
                        //colName: 'freightType',
                        queryMode: 'local',
                        editable: false,
                        store: createTypeStore,
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
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [                  
                    {
                        xtype: 'radiogroup',
                        id: 'query_type',
                        name: 'query_type',
                        fieldLabel: "創建人條件",
                        labelWidth: 75,
                        margin: '0 5 0 0',
                        colName: 'query_type',
                        defaults: {
                            name: 'Tax_Type'
                        },
                        width: 224,
                        columns: 2,
                        items: [
                        { id: 'select1', boxLabel: "姓名", inputValue: '1', checked: true },
                        { id: 'select2', boxLabel: "郵箱", inputValue: '2' },
                        ]
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '查詢內容',
                        labelWidth: 60,
                        id: 'userName_ro_email',
                        name: 'userName_ro_email',
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
                        value: '創建時間:&nbsp&nbsp&nbsp',
                        margin: '0 5 0 0',
                    },                   
                    {
                        xtype: 'datetimefield',
                        format: 'Y-m-d H:i:s',
                        time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
                        id: 'time_start',
                        name: 'time_start',
                        margin: '0 5 0 0',
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
                        xtype: 'datetimefield',
                        format: 'Y-m-d H:i:s',
                        time: { hour: 23, min: 59, sec: 59 },//標記結束時間23:59:59
                        id: 'time_end',
                        name: 'time_end',
                        margin: '0 5 0 0',
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
                        margin: '4 10 0 50',
                        iconCls: 'icon-search',
                        text: "查詢",
                        handler: Query
                    },
                    {
                        xtype: 'button',
                        margin: '4 10 0 0',
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
        id: 'DeliveryChangeLogGrid',
        store: DeliveryChangeLogStore,
        // height: 645,
        flex: 8.5,
        columnLines: true,
        frame: true,
        columns: [
   
            { header: '出貨單編號', dataIndex: 'deliver_id', width: 90, align: 'center' },           
            {
                header: '創建類型', dataIndex: 'dcl_create_type', width: 90, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "前臺";
                    }
                    else if (value == 2) {
                        return "後臺";
                    } else {
                        return value;
                    }
                }
            },
            {
                header: '創建人', dataIndex: 'create_user', width: 90, align: 'center'
                ,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.dcl_create_type == 1) {
                        return record.data.dcl_create_username;
                    }
                    else if (record.data.dcl_create_type == 2) {
                        return record.data.dcl_create_musername;
                    } 
                }
            },
            { header: '創建時間', dataIndex: 'dcl_create_datetime', width: 140, align: 'center' },
            { header: '備註', dataIndex: 'dcl_note', width: 110, align: 'center' },
            { header: '來源IP', dataIndex: 'dcl_ipfrom', width: 150, align: 'center' },
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
              //xtype: 'button',
              //text: '編輯',
              //disabled: true,
              //iconCls: 'icon-user-edit',
              //id: 'edit',
              //handler: onEditClick
          }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: DeliveryChangeLogStore,
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
    var deliver_id = Ext.getCmp('deliverId').getValue().trim(); if (deliver_id != '') { falg++; }
    var dcl_create_type = Ext.getCmp('createType').getValue(); if (dcl_create_type != '-1') { falg++; } 
    var userName_ro_email = Ext.getCmp('userName_ro_email').getValue().trim(); if (userName_ro_email != '') { falg++; }
    var time_start = Ext.getCmp('time_start').getValue(); if (time_start != null) { falg++; }
    var time_end = Ext.getCmp('time_end').getValue(); if (time_end != null) { falg++; }

    DeliveryChangeLogStore.removeAll();
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
    
    //alert(Ext.getCmp("query_type").getValue().Tax_Type);
   
    
    Ext.getCmp("DeliveryChangeLogGrid").store.loadPage(1, {
        params: {
            deliver_id: Ext.htmlEncode(Ext.getCmp('deliverId').getValue().trim()),//出貨單編號
            dcl_create_type: Ext.getCmp('createType').getValue(),

            query_type: Ext.getCmp("query_type").getValue().Tax_Type,
            userName_ro_email: Ext.getCmp('userName_ro_email').getValue().trim(),

            time_start: Ext.getCmp('time_start').getValue(),//預計到貨日（estimated_arrival_date）--開始時間
            time_end: Ext.getCmp('time_end').getValue(),//結束時間             
        }
    });
}
//編輯
//onEditClick = function () {
//    var row = Ext.getCmp("deliverExpectArrivalGrid").getSelectionModel().getSelection();
//    if (row.length == 0) {
//        Ext.Msg.alert(INFORMATION, NO_SELECTION);
//    }
//    else if (row.length > 1) {
//        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
//    } else if (row.length == 1) {
//        editFunction(row[0], DeliverExpectArrivalStore);
//    }
//}







