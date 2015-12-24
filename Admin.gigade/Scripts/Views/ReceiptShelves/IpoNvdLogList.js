Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;


//創建類型store
//var createTypeStore = Ext.create('Ext.data.Store', {
//    fields: ['txt', 'value'],
//    data: [       
//        { "txt": '前臺', "value": "1" },
//        { "txt": '後臺', "value": "2" }
//    ]
//});
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
Ext.define('GIGADE.IpoNvdLog', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'row_id', type: 'string' },//流水號（自增）
        { name: 'work_id', type: 'string' },//工作編號

        { name: 'ipo_id', type: 'string' },//採購單單號
        { name: 'item_id', type: 'string' },//商品細項編號
        //{ name: 'upc_id', type: 'string' },//商品條碼
        { name: 'loc_id', type: 'string' },//商品主料位
        { name: 'add_qty', type: 'string' },//收貨上架數量
        { name: 'made_date', type: 'string' },//收貨上架的製造日期
        { name: 'pwy_dte_ctl', type: 'string' },//是否效期控管
        { name: 'cde_date', type: 'string' },//保存期限
        { name: 'create_user_string', type: 'string' },//創建人
        { name: 'create_datetime', type: 'string' },//創建時間 pwy_dte_ctl    
        
             
    ]
});
//獲取grid中的數據
var IpoNvdLogStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.IpoNvdLog',
    proxy: {
        type: 'ajax',
        url: '/ReceiptShelves/GetIpoNvdLogList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
            root: 'data'
        }
    }
});

//加載前
IpoNvdLogStore.on('beforeload', function () {
    Ext.apply(IpoNvdLogStore.proxy.extraParams, {      
        work_id: Ext.htmlEncode(Ext.getCmp('work_id').getValue().trim()),//工作單號
        ipo_id: Ext.getCmp('ipo_id').getValue().trim(),//採購單單號            
        itemId_or_upcId: Ext.getCmp('itemId_or_upcId').getValue().trim(),//商品細項編號、條碼
        loc_id: Ext.getCmp('loc_id').getValue().trim(),//商品主料位

        time_start: Ext.getCmp('time_start').getValue(),//log創建日期，開始時間
        time_end: Ext.getCmp('time_end').getValue()//結束時間       
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
        height: 130,
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
                        fieldLabel: '工作編號',
                        labelWidth: 70,
                        id: 'work_id',
                        name: 'work_id',
                        margin: '0 5 0 0',
                        //regex: /^[0-9]*$/,
                        //regexText: '請輸入0-9數字類型的字符',
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
                        fieldLabel: '採購單單號',
                        labelWidth: 85,
                        id: 'ipo_id',
                        name: 'ipo_id',
                        margin: '0 5 0 0',
                        //regex: /^[0-9]*$/,
                        //regexText: '請輸入0-9數字類型的字符',
                        submitValue: true,
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == Ext.EventObject.ENTER) {
                                    Query();
                                }
                            }
                        }
                    }

                    //{
                    //    xtype: 'combobox',
                    //    id: 'createType',
                    //    margin: '0 5 0 0',
                    //    fieldLabel: 'ipo_id',
                    //    labelWidth: 60,
                    //    //colName: 'freightType',
                    //    queryMode: 'local',
                    //    editable: false,
                    //    store: createTypeStore,
                    //    displayField: 'txt',
                    //    valueField: 'value',
                    //    submitValue: true,
                    //    emptyText: '全部',
                    //    value: -1
                    //    ,
                    //    listeners: {
                    //        specialkey: function (field, e) {
                    //            if (e.getKey() == Ext.EventObject.ENTER) {
                    //                Query();
                    //            }
                    //        }
                    //    }
                    //}
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: '商品主料位',
                        labelWidth: 70,
                        id: 'loc_id',
                        name: 'loc_id',
                        margin: '0 5 0 0',
                        //regex: /^[0-9]*$/,
                        //regexText: '請輸入0-9數字類型的字符',
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
                        fieldLabel: '商品細項編號',
                        labelWidth: 85,
                        id: 'itemId_or_upcId',
                        name: 'item_id',
                        margin: '0 5 0 0',
                        //regex: /^[0-9]*$/,
                        //regexText: '請輸入0-9數字類型的字符',
                        emptyText:"  或者輸入商品條碼",
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
                        margin: '0 5 0 0'
                    },                   
                    {
                        xtype: 'datefield',
                        format: 'Y-m-d',
                        //time: { hour: 00, min: 00, sec: 00 },
                        id: 'time_start',
                        name: 'time_start',
                        margin: '0 5 0 0',
                        submitValue: true,
                        editable: false,
                        //value: Tomorrow(),
                        listeners: {
                            select: function (a, b, c) {
                                //var start = Ext.getCmp("time_start");
                                //var end = Ext.getCmp("time_end");
                          
                                //if (end.getValue() != null && start.getValue() > end.getValue()) {
                                //    Ext.Msg.alert("提示信息","開始時間不能大於結束時間");
                                //    start.setValue(setNextMonth(end.getValue(),-1));
                                //}
                                var start = Ext.getCmp("time_start");
                                var end = Ext.getCmp("time_end");
                                var s_date = new Date(start.getValue());
                                end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));

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
                        //time: { hour: 23, min: 59, sec: 59 },
                        id: 'time_end',
                        name: 'time_end',
                        margin: '0 5 0 0',
                        submitValue: true,
                        editable: false,
                        listeners: {
                            select: function (a, b, c) {
                                //var start = Ext.getCmp("time_start");
                                //var end = Ext.getCmp("time_end");

                                //if (start.getValue() != null && start.getValue() > end.getValue()) {
                                //    Ext.Msg.alert("提示信息", "結束時間不能小於開始時間");
                                //    end.setValue(setNextMonth(start.getValue(),+1));
                                //}
                                var start = Ext.getCmp("time_start");
                                var end = Ext.getCmp("time_end");
                                var s_date = new Date(start.getValue());
                                var end_date = new Date(end.getValue());
                                if (start.getValue() == null)
                                {
                                    start.setValue(new Date(end_date.setMonth(end_date.getMonth() - 1)));
                                }

                                if (end.getValue() < start.getValue())
                                {
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
                                //var datetime1 = new Date();
                                //datetime1.setFullYear(2000, 1, 1);
                                //datetime1.setHours(00,00,00);
                                //var datetime2 = new Date();
                                //datetime2.setFullYear(2100, 1, 1);
                                //datetime2.setHours(23, 59, 59);                               
                                //Ext.getCmp("time_start").setMinValue(datetime1);
                                //Ext.getCmp("time_start").setMaxValue(datetime2);
                                //Ext.getCmp("time_end").setMinValue(datetime1);
                                //Ext.getCmp("time_end").setMaxValue(datetime2);

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
        id: 'IpoNvdLogGrid',
        store: IpoNvdLogStore,
        // height: 645,
        flex: 8.5,
        columnLines: true,
        frame: true,     
        columns: [
   
            { header: '流水號', dataIndex: 'row_id', width: 50, align: 'center' },
            { header: '工作編號', dataIndex: 'work_id', width: 150, align: 'center'},
            {
                header: '採購單單號', dataIndex: 'ipo_id', width: 150, align: 'center'               
            },
            //{ header: '商品條碼', dataIndex: 'upc_id', width: 120, align: 'center' },
            { header: '商品細項編號', dataIndex: 'item_id', width: 100, align: 'center' },
            { header: '商品主料位', dataIndex: 'loc_id', width: 100, align: 'center' },
            { header: '收貨上架數量', dataIndex: 'add_qty', width: 100, align: 'center' },
            {
                header: '製造日期', dataIndex: 'made_date', width: 100, align: 'center',
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
                header: '有效日期', dataIndex: 'cde_date', width: 100, align: 'center',
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
                header: '是否效期控管', dataIndex: 'pwy_dte_ctl', width: 90, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "Y") {
                        return "<font color='green'>是<font/>";
                    }
                    else  {
                        return "<font color='red'>否<font/>";
                    }                  
                }
            },
            { header: '創建人', dataIndex: 'create_user_string', width: 100, align: 'center' },
            { header: '創建時間', dataIndex: 'create_datetime', width: 150, align: 'center' }
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
            store: IpoNvdLogStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        })
        //selModel: sm
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
        s.setHours(00, 00, 00);
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
    var work_id = Ext.getCmp('work_id').getValue().trim(); if (work_id != '') { falg++; }
    var ipo_id = Ext.getCmp('ipo_id').getValue().trim(); if (ipo_id != '') { falg++; }
    var itemId_or_upcId = Ext.getCmp('itemId_or_upcId').getValue().trim(); if (itemId_or_upcId != '') { falg++; }
    var loc_id = Ext.getCmp('loc_id').getValue().trim(); if (loc_id != '') { falg++; }

    var time_start = Ext.getCmp('time_start').getValue(); if (time_start != null) { falg++; }
    var time_end = Ext.getCmp('time_end').getValue(); if (time_end != null) { falg++; }

    IpoNvdLogStore.removeAll();
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
   
    
    Ext.getCmp("IpoNvdLogGrid").store.loadPage(1, {
        params: {
            work_id: Ext.htmlEncode(Ext.getCmp('work_id').getValue().trim()),//工作單號
            ipo_id: Ext.getCmp('ipo_id').getValue().trim(),//採購單單號            
            itemId_or_upcId: Ext.getCmp('itemId_or_upcId').getValue().trim(),//商品細項編號、條碼
            loc_id: Ext.getCmp('loc_id').getValue().trim(),//商品主料位

            time_start: Ext.getCmp('time_start').getValue(),//log創建日期，開始時間
            time_end: Ext.getCmp('time_end').getValue()//結束時間             
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







