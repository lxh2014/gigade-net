﻿
/*
 * Copyright (c)J01 
 * 作   者：yachao1120j
 * CreateTime :2015/10/19
 * 等待料位報表
 */
var pageSize = 25;


// 列表頁的model
Ext.define('gridlistWLW', {
    extend: 'Ext.data.Model',
    fields: [
        {name:"plas_id_string",type:"int"},//料位編號
        { name: "product_id", type: "int" },//商品編號
        { name: "product_name", type: "string" },//商品名稱
        { name: "item_id", type: "int" },//商品細項編號
        { name: "product_spec", type: "string" },//商品規格
        { name: "product_status", type: "int" },//商品狀態
        { name: "product_status_string", type: "string" },//商品狀態顯示
        { name: "product_createdate", type: "int" },//商品建立日期  
        { name: "product_createdate_string", type: "string" },//商品建立日期  
        { name: "product_createdate", type: "string" },
        { name: "combination", type: "int" },//商品類型   商品組合類型  1:單一商品 2:固定組合 3:任選組合 4:群組搭配
        { name: "combination_string", type: "string" },
        { name: "product_freight_set", type: "int" },//溫層 
         { name: "product_freight_set_string", type: "string" },//溫層顯示
        { name: "product_mode", type: "int" },//出貨方式
         { name: "product_mode_string", type: "string" },//出貨方式 商品出貨方式  1:自出   2:寄倉 3 調度
        { name: "product_start", type: "int" },//商品上架時間
        { name: "product_start_string", type: "string" },//商品上架時間
        { name: "product_fenlei_xiaolei", type: "string" },//商品上架時間
        { name: "product_fenlei_dalei", type: "string" },//商品上架時間
        { name: "po_id", type: "string" },//採購單單號

    ],
});

//列表頁的數據源
var WareHouseStore = Ext.create('Ext.data.Store', {//WareHouseStore
    pageSize: pageSize,
    autoDestroy: true,
    model: 'gridlistWLW',
    proxy: {
        type: 'ajax',
        url: '/WareHouse/GetWaitLiaoWeiList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

// 商品狀態
//var ProductStatusStore = Ext.create('Ext.data.Store', {
//    fields: ['txt', 'value'],
//    data: [
//        { "txt": "全部", "value": "100" },
//        { "txt": "新建立商品", "value": "0" },
//        { "txt": "申請審核", "value": "1" },
//        { "txt": "審核通過", "value": "2" },
//        { "txt": "上架", "value": "5" },

//    ]
//});

// 出貨方式
var OutProductStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "全部", "value": "100" },
        { "txt": "自出", "value": "1" },
        { "txt": "寄倉", "value": "2" },
        { "txt": "調度", "value": "3" }
    ]
});

//  溫層 
var freightStore = Ext.create('Ext.data.Store', {
    fields: ['parameterName', 'ParameterCode'],
    data: [
           { 'parameterName': '全部', 'ParameterCode': '100' },
           { 'parameterName': '常溫', 'ParameterCode': '1' },
           { 'parameterName': '冷凍', 'ParameterCode': '2' },
    ]
});

//列表頁加載時候得到的數據
WareHouseStore.on('beforeload', function () {
    Ext.apply(WareHouseStore.proxy.extraParams,
        {
            //product_status: Ext.getCmp('product_status').getValue(),//商品狀態
            product_mode: Ext.getCmp('product_mode').getValue(),
            freight: Ext.getCmp('freight').getValue(),
            start_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('start_time').getValue()), 'Y-m-d H:i:s')),
            end_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('end_time').getValue()), 'Y-m-d H:i:s')),
        });
});

////每行數據前段的矩形選擇框
//var sm = Ext.create('Ext.selection.CheckboxModel', {
//    listeners: {
//        selectionchange: function (sm, selections) {
//            Ext.getCmp("IQGrid").down('#edit').setDisabled(selections.length == 0);
//        }
//    }
//});

//頁面加載
Ext.onReady(function () {
    var searchForm = Ext.create('Ext.form.Panel', {
        id: 'searchForm',
        layout: 'anchor',
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                           {
                               xtype: 'datetimefield',
                               margin: '5 0 0 5',
                               fieldLabel: '商品建立時間',
                               labelWidth: 80,
                               id: 'start_time',
                               format: 'Y-m-d H:i:s',
                               time: { hour: 00, min: 00, sec: 00 },
                               width: 230,
                               value: new Date(Tomorrow(0).setMonth(Tomorrow(0).getMonth() - 1)),
                               editable: false,
                               listeners: {
                                   select: function (a, b, c) {
                                       var start = Ext.getCmp("start_time");
                                       var end = Ext.getCmp("end_time");                                      
                                       if (end.getValue() == null) {
                                           end.setValue(setNextMonth(start.getValue(), 1));
                                       }
                                       else if (end.getValue() < start.getValue()) {
                                           Ext.Msg.alert(INFORMATION, DATA_TIP);
                                           end.setValue(setNextMonth(start.getValue(), 1));
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
                    margin: '5 0 0 5',
                    value: '~'
                },
                {
                    xtype: 'datetimefield',
                    id: 'end_time',
                    margin: '5 0 0 5',
                    format: 'Y-m-d  H:i:s',
                    time: { hour: 23, min: 59, sec: 59 },
                    width: 150,
                    value: setNextMonth(Tomorrow(0), 0),
                    editable: false,
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("start_time");
                            var end = Ext.getCmp("end_time");
                            if (start.getValue() == null) {
                                start.setValue(setNextMonth(end.getValue(), -1));
                            }
                            if (end.getValue() < start.getValue()) {
                                start.setValue(setNextMonth(end.getValue(), -1));
                            }
                        },
                        specialkey: function (field, e) {
                            if (e.getKey() == Ext.EventObject.ENTER) {
                                Query();
                            }
                        }
                    }
                }, {
                    xtype: 'combobox',
                    editable: false,
                    id: 'freight',
                    fieldLabel: "溫層",
                    name: 'freight',
                    width: 110,
                    labelWidth: 30,
                    margin: '5 0 0 5',
                    store: freightStore,
                    lastQuery: '',
                    displayField: 'parameterName',
                    valueField: 'ParameterCode',
                    value: 100,
                    listeners: {
                        specialkey: function (field, e) {
                            if (e.getKey() == e.ENTER) {
                                Query();
                            }
                        }
                    }
                },
                    {
                        xtype: 'combobox',
                        name: 'product_mode',
                        id: 'product_mode',
                        editable: false,
                        fieldLabel: "出貨方式",
                        labelWidth: 60,
                        margin: '5 0 0 5',
                        store: OutProductStore,
                        queryMode: 'local',
                        submitValue: true,
                        displayField: 'txt',
                        valueField: 'value',
                        typeAhead: true,
                        forceSelection: false,
                        value: 100,
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                ]
            },
           // {
           //     xtype: 'fieldcontainer',
           //     combineErrors: true,
           //     layout: 'hbox',
           //     items: [
           ////{
           ////    xtype: 'combobox',
           ////    name: 'product_status',
           ////    id: 'product_status',
           ////    editable: false,
           ////    fieldLabel: "商品狀態",
           ////    labelWidth: 60,
           ////    margin: '5 0 0 5',
           ////    store: ProductStatusStore,
           ////    queryMode: 'local',
           ////    submitValue: true,
           ////    displayField: 'txt',
           ////    valueField: 'value',
           ////    typeAhead: true,
           ////    forceSelection: false,
           ////    value: 100,
           ////    listeners: {
           ////        specialkey: function (field, e) {
           ////            if (e.getKey() == e.ENTER) {
           ////                Query();
           ////            }
           ////        }
           ////    }
           ////},
           ////{
           ////    xtype: 'combobox',
           ////    name: 'product_mode',
           ////    id: 'product_mode',
           ////    editable: false,
           ////    fieldLabel: "出貨方式",
           ////    labelWidth: 60,
           ////    margin: '5 0 0 5',
           ////    store: OutProductStore,
           ////    queryMode: 'local',
           ////    submitValue: true,
           ////    displayField: 'txt',
           ////    valueField: 'value',
           ////    typeAhead: true,
           ////    forceSelection: false,
           ////    value: 100,
           ////    listeners: {
           ////        specialkey: function (field, e) {
           ////            if (e.getKey() == e.ENTER) {
           ////                Query();
           ////            }
           ////        }
           ////    }
           ////},
           //     ]
           // }
        ],
        buttonAlign: 'left',
        buttons: [
                {
                    text: '查詢',
                    id: 'query',
                    //disabled: true,
                    margin: '0 10 0 10',
                    iconCls: 'icon-search',
                    handler: function () {
                        Query();
                    }
                },
                {
                    text: '重置',
                    iconCls: 'ui-icon ui-icon-reset',
                    listeners:
                            {
                                click: function () {
                                    searchForm.getForm().reset();
                                    var datetime1 = new Date();
                                    datetime1.setFullYear(2000, 1, 1);
                                    var datetime2 = new Date();
                                    datetime2.setFullYear(2100, 1, 1);
                                    Ext.getCmp("start_time").setMinValue(datetime1);
                                    Ext.getCmp("start_time").setMaxValue(datetime2);
                                    Ext.getCmp("end_time").setMinValue(datetime1);
                                    Ext.getCmp("end_time").setMaxValue(datetime2);
                                }
                            }
                }
        ]
    });
    //第二個panel
    var WLWGrid = Ext.create('Ext.grid.Panel', {
        id: 'WLWGrid',
        store: WareHouseStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 9.4,
        columns: [
                { header: '料位編號', dataIndex: 'plas_id_string', align: 'center',hidden:true },
                { header: '商品細項編號', dataIndex: 'item_id', align: 'center' },
                { header: '商品名稱', dataIndex: 'product_name', width: 180, align: 'center' },
                { header: '商品規格', dataIndex: 'product_spec', width: 120, align: 'center' },
                { header: '商品類型', dataIndex: 'combination_string', width: 120, align: 'center' },//商品類型
                { header: "分類--大類", dataIndex: "product_fenlei_dalei", align: 'center' },
                 { header: "分類--小類", dataIndex: "product_fenlei_xiaolei", align: 'center' },
                { header: "商品狀態", dataIndex: "product_status_string", align: 'center' },
                 { header: "出貨方式", dataIndex: "product_mode_string", align: 'center' },//出貨方式
                { header: "溫層", dataIndex: "product_freight_set_string", align: 'center' },
                 { header: "商品建立日期", dataIndex: 'product_createdate_string', align: 'center', width: 180 },
                { header: "商品上架時間", dataIndex: "product_start_string", align: 'center', width: 180 },
                { header: "採購單單號", dataIndex: "po_id", align: 'center', width: 180 },

        ],
        tbar: [
        { xtype: 'button', text: '匯出等待料位報表', margin: '0 0 0 5', iconCls: 'icon-excel', id: 'btnExcel', handler: Export }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: WareHouseStore,
            pageSize: pageSize,
            displayInfo: true,//是否顯示數據信息
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

        },
        //selModel: sm  // 矩形選擇框
    });

    Ext.create('Ext.Viewport', {
        layout: 'vbox',
        items: [searchForm, WLWGrid],// 包含两个控件 
        autoScroll: true,
        renderTo: Ext.getBody(),
        listeners: {
            resize: function () {
                WLWGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
})

/*************************************************************************************查询信息*************************************************************************************************/

function Query(x) {
    //var ignore_stockRdo = Ext.getCmp('ignore_stockRdo').getValue().ignore_stockVal;
    Ext.getCmp('WLWGrid').store.loadPage(1, {
        params: {
        }
    });
}

/******************************************************************************************************************************************************************************************/
function Tomorrow(n) {
        var d;
        var dt;
        var s = "";
        d = new Date();                             // 创建 Date 对象。
        s += d.getFullYear() + "/";                     // 获取年份。
        s += (d.getMonth() + 1) + "/";              // 获取月份。
        s += d.getDate();
        dt = new Date(s);
        dt.setDate(dt.getDate() + n);
        return dt;                  // 返回日期。
}
setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    } else if (n >= 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}
/************匯出到Exce************/
function Export() {
    var freight = Ext.getCmp('freight').getValue();
    //var product_status = Ext.getCmp('product_status').getValue();
    var product_mode = Ext.getCmp('product_mode').getValue();
    var start_time = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('start_time').getValue()), 'Y-m-d H:i:s'));
    var end_time = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('end_time').getValue()), 'Y-m-d H:i:s'));
    window.open("/WareHouse/ExportCSV?product_mode=" + product_mode + "&freight=" + freight + "&start_time=" + Ext.Date.format(new Date(Ext.getCmp('start_time').getValue()), 'Y-m-d H:i:s') + "&end_time=" + Ext.Date.format(new Date(Ext.getCmp('end_time').getValue()), 'Y-m-d H:i:s'));

}
