/*
 * Copyright (c)J01 
 * 作   者：chaojie1124j
 * CreateTime :2015/10/27
 * 補貨通知統計
 */

Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;

// 列表頁的model
Ext.define('gigade.gridlistStatus', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "product_id", type: "string" },//料位編號
        { name: "product_name", type: "string" },//商品細項編號
        { name: "loc_id", type: "string" },//料位編號
        { name: "item_id", type: "string" },//商品細項編號
        { name: "combination", type: "int" },//組合Y/N
        { name: "item_stock", type: "int" },//前台庫存量
        { name: "iinvd_stock", type: "string" },//後台庫存量
        { name: "prepaid", type: "int" },//買斷
        { name: "product_freight", type: "int" },//溫層Shortage
        { name: "product_status_string", type: "string" },//商品狀態
        { name: "shortage", type: "int" }//庫存不為零不允許販售
        
    ],
});
//查詢Store
var  StatusStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.gridlistStatus',
    proxy: {
        type: 'ajax',
        url: '/ProductPurchase/GetStatusListLowerShelf',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
StatusStore.on('beforeload', function () {
    var checked = Ext.getCmp("Istock").items;
    var check = "";

    if (checked.get(0).checked) {

        check ="1";
    } else {
        check ="0";
    }
    Ext.apply(StatusStore.proxy.extraParams, {
        Short: check,
        searchcon: Ext.getCmp('searchcontent').getValue(),//查詢內容
        startIloc: Ext.getCmp('startIloc').getValue(),//走道範圍
        endIloc: Ext.getCmp('endIloc').getValue(),//走道範圍
        fright_set: Ext.getCmp('Fright_set').getValue(),//溫層
    });
});
var yunsongtype = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "全部", "value": "0" },
        { "txt": "常溫", "value": "1" },
        { "txt": "冷凍", "value": "2" }
    ]
});
function Query(x) {
 
    var form = this.up('form').getForm();

    if (form.isValid()) {
      
    
            StatusStore.removeAll();
            var checked = Ext.getCmp("Istock").items;
            var check = "";
   
            if (checked.get(0).checked) {
       
                check = "1";
            } else
            {
                check = "0";
            }
            Ext.getCmp("StatusView").store.loadPage(1, {
                params: {
                    Short:check,
                    searchcon: Ext.getCmp('searchcontent').getValue(),//查詢內容
                    startIloc: Ext.getCmp('startIloc').getValue(),//走道範圍
                    endIloc: Ext.getCmp('endIloc').getValue(),//走道範圍
                    fright_set: Ext.getCmp('Fright_set').getValue(),//溫層
         
                }
            });
    } else {
        Ext.Msg.alert("提示", "請對照正確的格式輸入!");
        return;
    }
}


Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 120,
        flex: 2,
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
              {
                  xtype: 'fieldcontainer',
                  combineErrors: true,
                  layout: 'hbox',
                  // margin: '10,0,0,0',
                  items: [
                        
                        {
                            xtype: 'textfield',
                            fieldLabel: "商品編號/名稱/細項編號",
                            labelWidth: 150,
                            regex: /^((?!%).)*$/,
                            regexText: "禁止輸入百分號",
                            allowBlank: true,
                            id: 'searchcontent',
                            name: 'searchcontent',
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
                 fieldLabel: "料位區間",
                 width: 350,
                 labelWidth: 60,
                // combineErrors: true,
                 layout: 'hbox',
                 items: [
                     {
                         xtype: "textfield",
                         id: 'startIloc',
                         name: 'startIloc',
                         regex: /^((?!%).)*$/,
                         regexText: "禁止輸入百分號",
                         maxLength: 20,
                         flex: 5,
                         listeners: {
                             specialkey: function (field, e) {
                                 if (e.getKey() == e.ENTER) {
                                     Query();
                                 }
                             }
                         }
                     },
                     {
                         xtype: 'displayfield',
                         value: "--",
                         flex: 1,
                         margin: '0 0 0 5'
                     },
                     {
                         xtype: "textfield",
                         id: 'endIloc',
                         regex: /^((?!%).)*$/,
                         regexText: "禁止輸入百分號",
                         name: 'endIloc',
                         maxLength: 20,
                         flex: 5,
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
                  // margin: '10,0,0,0',
                  items: [
                         {
                             xtype: 'combobox',
                             name: 'Fright_set',
                             id: 'Fright_set',
                             editable: false,
                             fieldLabel: "溫層",
                             labelWidth: 60,
                             store: yunsongtype,
                             queryMode: 'local',
                             submitValue: true,
                             displayField: 'txt',
                             valueField: 'value',
                             typeAhead: true,
                             forceSelection: false,
                             value: 0
                         },
                         {
                             xtype: 'checkboxgroup',
                             fieldLabel: '有庫存掛補',
                             labelWidth: 80,
                             width: 350,
                             margin: '0 10 0 5',
                             id: 'Istock',
                             name: 'Istock',
                             columns: 1,
                             items: [
                                 { id:'IstockCheck',name: 'vtype', inputValue: '1', checked: true },
                             ]
                         }
                  ]
              },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                margin: '10,0,0,0',
                items: [
                    {
                        xtype: 'button',
                        text: SEARCH,
                        margin: '0,0, 5,40',
                        //iconCls: 'icon-search',
                        margin: '0 10 0 20',
                        id: 'btnQuery',
                        iconCls: 'icon-search',
                        handler: Query
                    },
                     {
                         xtype: 'button',
                         text: "重置",
                         margin: '0,0, 5,40',
                         //iconCls: 'icon-search',
                         margin: '0 10 0 20',
                         id: 'btnresult',
                         iconCls: 'ui-icon ui-icon-reset',
                         handler: function () {
                             this.up('form').getForm().reset();
                           
                         }
                     }
                ]
            }
        ]
    });

    var StatusView = Ext.create('Ext.grid.Panel', {
        id: 'StatusView',
        store: StatusStore,
        flex: 8,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
             // new Ext.grid.RowNumberer(),//自動顯示行號
             { header: "商品編號", dataIndex: 'product_id', width: 100, align: 'center' },
            { header: "商品名稱", dataIndex: 'product_name', width: 80, align: 'center' },
            { header: "料位編號", dataIndex: 'loc_id', width: 100, align: 'center' },
            { header: "商品細項編號", dataIndex: 'item_id', width: 80, align: 'center' },
            {
                header: "組合Y/N", dataIndex: 'combination', width: 60, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (value) {
                        case 1:
                            return "N";
                        case 2:
                        case 3:
                        case 4:
                            return "Y";
                        default:
                            return value;
                    }
                }
            },
            { header: "前台庫存量", dataIndex: 'item_stock', width: 80, align: 'center' },
            { header: "後台庫存量", dataIndex: 'iinvd_stock', width: 80, align: 'center' },
            {
                header: "溫層", dataIndex: 'product_freight', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "常溫";
                    }
                    if (value == 2) {
                        return "冷凍";
                    }
                    else {
                        return value;
                    }
                }
            },
            {
                header: "是否買斷", dataIndex: 'prepaid', width: 80, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 0) {
                        return "否";
                    }
                    if (value == 1) {
                        return "是";
                    }
                    else {
                        return value;
                    }
                }
            },
             { header: "商品狀態", dataIndex: 'product_status_string', width: 80, align: 'center' },
             {
                 header: "有庫存掛補", dataIndex: 'shortage', width: 80, align: 'center',
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     if (value == 0) {
                         return "否";
                     }
                     if (value == 1) {
                         return "是";
                     }
                     else {
                         return value;
                     }
                 }
             }
        ],
        tbar: [
            { xtype: 'button', text: "匯出", id: 'exportExcel', icon: '../../../Content/img/icons/excel.gif', handler: ExportExcel }

        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: StatusStore,//StatusStore
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
        //, selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm, StatusView],//
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                StatusView.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
});



///匯出EXcel
ExportExcel = function () {
    var searchcon = Ext.getCmp('searchcontent');
    var start = Ext.getCmp('startIloc');
    var end= Ext.getCmp('endIloc');
    if (searchcon.isValid() && start.isValid() && end.isValid) {
    var checked = Ext.getCmp("Istock").items;
    var check = "";

    if (checked.get(0).checked) {

        check = "1";
    } else {
        check = "0";
    }
    var Short = check;
    var searchcon = Ext.getCmp('searchcontent').getValue();
    var startIloc = Ext.getCmp('startIloc').getValue();
    var endIloc = Ext.getCmp('endIloc').getValue();
    var fright_set = Ext.getCmp('Fright_set').getValue();
    window.open("/ProductPurchase/GetStatusListLowerShelfExcel?Short=" + Short + "&searchcon=" + searchcon + "&startIloc=" + startIloc + "&endIloc=" + endIloc + "&fright_set=" + fright_set);
    } else {
        Ext.Msg.alert("提示", "請對照正確的格式輸入!");
        return;
    }
}