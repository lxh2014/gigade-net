﻿var pageSize = 25;
Ext.define('gigade.Ipod', {
    extend: 'Ext.data.Model',
    fields: [
     { name: "row_id", type: "int" },
     { name: "po_id", type: "string" },//採購單單號
     { name: "parameterName", type: "string" },//採購單別描述
     { name: "Erp_Id", type: "string" },//品號
     { name: "qty_ord", type: "int" },//下單採購量
     { name: "qty_damaged", type: "int" },//不允收量
     { name: "qty_claimed", type: "int" },//允收量
     { name: "product_name", type: "string" },//商品名稱
     { name: "vendor_name_full", type: "string" },//供應商名稱
     { name: "spec", type: "string" },//規格
     { name: "item_stock", type: "string" },//庫存
     { name: "vendor_id", type: "string" }
    ]
});
var ipodStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Ipod',
    autoDestroy: true,
    proxy: {
        type: 'ajax',
        url: '/WareHouse/GetIpodListNo',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
            
        }
    }
});

Ext.define("gigade.parameter", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "ParameterCode", type: "string" },
        { name: "parameterName", type: "string" }
    ]
});
var PoTypeStore = Ext.create('Ext.data.Store', {
    model: 'gigade.parameter',
    autoDestroy: true,
    autoLoad:true,
    proxy: {
        type: 'ajax',
        url: "/WareHouse/GetPromoInvsFlgList?Type=po_type",
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

function Query() {
    var falg = 0;
    var Poty = Ext.getCmp('Poty').getValue(); if (Poty != '') { falg++; }
    var erp_id = Ext.getCmp('erp_id').getValue().trim(); if (erp_id != "") { falg++; }
    var vendor_id = Ext.getCmp('vendor_id').getValue(); if (vendor_id !=null) { falg++; }
    var check = Ext.getCmp('checkInfoYesOrNo').getValue();
    var vendor_name_full = Ext.getCmp('vendor_name_full').getValue().trim(); if (vendor_name_full != '') { falg++; }
    //if (falg == 0) {
    //    Ext.Msg.alert("提示", "請輸入查詢條件");
    //    return false;
    //}
    var form=Ext.getCmp('frm').getForm();
    if (form.isValid())
    {
        ipodStore.removeAll();
        Ext.getCmp("gdAccum").store.loadPage(1,
            {
                params: {
                    Potype: Poty,
                    erp_id: erp_id,
                    vendor_id: vendor_id,
                    check: check,
                    vendor_name_full: vendor_name_full
                }
            });
    } 
}
Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',//anchor固定
        height: 99,
        border: 0,
        bodyPadding: 13,
        width: document.documentElement.clientWidth,
        items: [
                {
                    xtype: 'fieldcontainer',
                    combineErrors: true,
                    layout: 'hbox',
                    items: [
                           {
                               xtype: 'combobox', //類型
                               editable: false,
                               id: 'Poty',
                               fieldLabel: "採購單別",
                               name: 'Poty',
                               width: 210,
                               labelWidth: 60,
                               margin: '5 0 0 0',
                               store: PoTypeStore,
                               lastQuery: '',
                               displayField: 'parameterName',
                               valueField: 'ParameterCode',
                               emptyText: "请选择採購單別",
                               value: '',
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
                        fieldLabel: "品號",
                        labelWidth: 35,
                        name: 'erp_id',
                        id: 'erp_id',
                        margin: '5 0 0 5',
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                     {
                         xtype: 'numberfield',
                         fieldLabel: '供應商編號',
                         id: 'vendor_id',
                         name: 'vendor_id',
                         labelWidth: 70,
                         allowDecimals: false,
                         maxValue:4294967295,
                         minValue: 0,
                         margin: '5 0 0 5',
                         hideTrigger: true,
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
                        fieldLabel: '供應商名稱',
                        id: 'vendor_name_full',
                        name: 'vendor_name_full',
                        margin: '5 0 0 5',
                        width: 210,
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
                          xtype: 'checkbox',
                          boxLabel: "採購驗收不符",
                          id: 'checkInfoYesOrNo',
                          name: 'checkInfoYesOrNo',
                          width: 160,
                          labelWidth: 35,
                          margin: '5 0 0 5',
                          checked:true
                      }
                    ]
                },

                {
                    xtype: 'fieldcontainer',
                    combineErrors: true,//如果设置为 true, 则 field 容器自动将其包含的所有属性域的校验错误组合为单个错误信息, 并显示到 配置的 msgTarget 上. 默认值 false.
                    layout: 'hbox',
                    margin: '15 0 0 0',
                    items:
                     [
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
                            listeners:
                            {
                                click: function () {
                                    frm.getForm().reset();
                                }
                            }
                        }
                     ]
                }
        ]

    });
    ipodStore.on('beforeload', function () {
        Ext.apply(ipodStore.proxy.extraParams, {
            Potype : Ext.getCmp('Poty').getValue(),
         erp_id :Ext.getCmp('erp_id').getValue().trim(),
         vendor_id : Ext.getCmp('vendor_id').getValue(),
         check : Ext.getCmp('checkInfoYesOrNo').getValue(),
         vendor_name_full : Ext.getCmp('vendor_name_full').getValue().trim()
        });
    });
    var gdAccum = Ext.create('Ext.grid.Panel', {
        id: 'gdAccum',
        flex: 1.8,
        store: ipodStore,
        width: document.documentElement.clientWidth,
        columnLines: true,//顯示列線條
        frame: true,//Panel是圆角框显示
        columns: [{ header: "採購單別", dataIndex: 'parameterName', width: 70, align: 'center' },
            { header: "採購單號", dataIndex: 'po_id', width: 120, align: 'center' },
              { header: "品號", dataIndex: 'Erp_Id', width: 120, align: 'center' },
                { header: "商品名稱", dataIndex: 'product_name', width: 250, align: 'center' },
                { header: "供應商編號", dataIndex: 'vendor_id', width: 100, align: 'center' },
                { header: "供應商名稱", dataIndex: 'vendor_name_full', width: 170, align: 'center' },
                  { header: "規格", dataIndex: 'spec', width: 150, align: 'center' },
                    { header: "採購數量", dataIndex: 'qty_ord', width: 70, align: 'center' },
                    { header: "允收數量", dataIndex: 'qty_claimed', width: 70, align: 'center' },
                    { header: "不允收量", dataIndex: 'qty_damaged', width: 70, align: 'center' }
        ],
        tbar: [
           { xtype: 'button', text: "匯出採購單Excel", id: 'outExcel', icon: '../../../Content/img/icons/excel.gif', handler: outExcel }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ipodStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        })
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm, gdAccum],
        renderTo: Ext.getBody(),
        autoScroll: true,//自動顯示滾動條
        listeners: {
            resize: function () {//在组件被调整大小之后触发,首次布局初始化大小时不触发
                gdAccum.clientWidth = document.documentElement.clientWidth;
                this.doLayout();//手动强制这个容器的布局进行重新计算。大多数情况下框架自动完成刷新布局。
            }
        }
    });
});
outExcel = function () {
    var params = 'Potype=' + Ext.getCmp('Poty').getValue() + '&erp_id=' + Ext.getCmp('erp_id').getValue() + '&vendor_id=' + Ext.getCmp('vendor_id').getValue() + '&check=' + Ext.getCmp('checkInfoYesOrNo').getValue() + '&vendor_name_full=' + Ext.getCmp('vendor_name_full').getValue().trim()
    window.open('/WareHouse/WriteExcel?' + params);
}