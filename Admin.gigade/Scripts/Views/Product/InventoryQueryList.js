/*
 * Copyright (c)J01 
 * 作   者：yachao1120j
 * CreateTime :2015/9/9
 * 商品庫存查詢
 */
var pageSize = 25;

// 列表頁的model
Ext.define('gridlistIQ', {
    extend: 'Ext.data.Model',
    fields: [
         { name: "id", type: "int" },//商品編號
        { name: "product_id", type: "int" },//商品編號
        { name: "product_name", type: "string" },//商品名稱
        { name: "item_id", type: "int" },//商品細項編號
        { name: "product_spec", type: "string" },//商品規格
        { name: "product_status", type: "int" },//商品狀態
        { name: "product_status_string", type: "string" },//商品狀態顯示
        { name: "product_id_OR_product_name", type: "string" },//商品編號或名稱
        { name: "vendor_name_full_OR_vendor_id", type: "string" },//供應商名稱或編號
        { name: "vendor_name_full", type: "string" },//供應商名稱
        { name: "vendor_id", type: "int" },//供應商編號
        { name: "brand_id", type: "int" },//品牌編號
        { name: "brand_name", type: "string" },//品牌名稱
        { name: "brand_id_OR_brand_name", type: "string" },//品牌編號或名稱
        { name: "item_stock", type: "int" },//庫存數量
        { name: "ignore_stock", type: "int" },//補貨中停止販售
        { name: "ignore_stock_string", type: "string" },//補貨中停止販售顯示
        { name: "item_stock_start", type: "int" },//庫存數量開始
        { name: "item_stock_end", type: "int" },//庫存數量結束
        { name: "spec_title_1", type: "string" },//規格1
        { name: "spec_title_2", type: "string" }//規格2
    ],
});

//列表頁的數據源
var ProductStore = Ext.create('Ext.data.Store', {//ProductStore
    pageSize: pageSize,
    autoDestroy: true,
    model: 'gridlistIQ',
    proxy: {
        type: 'ajax',
        url: '/Product/GetInventoryQueryList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

Ext.define("gigade.gridPara", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "parameterCode", type: "string" },//
        { name: "parameterName", type: "string" }//
    ]
});
//供應商Store
var prodStatusStore = Ext.create('Ext.data.Store', {
    model: 'gigade.gridPara',
    //  autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Parameter/QueryPara?paraType=product_status",
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//列表頁加載時候得到的數據
ProductStore.on('beforeload', function () {
    Ext.apply(ProductStore.proxy.extraParams,
        {
            vendor_name_full_OR_vendor_id: Ext.getCmp('vendor_name_full_OR_vendor_id').getValue(),//供應商名稱或編號
            product_id_OR_product_name: Ext.getCmp('product_id_OR_product_name').getValue(),//商品編號或名稱
            brand_id_OR_brand_name: Ext.getCmp('brand_id_OR_brand_name').getValue(),//品牌編號或名稱
            product_status: Ext.getCmp('product_status').getValue(),//商品狀態
            item_stock_start: Ext.getCmp('item_stock_start').getValue(),//庫存數量開始
            item_stock_end: Ext.getCmp('item_stock_end').getValue(),//庫存數量結束
            ignore_stockRdo: Ext.getCmp('ignore_stockRdo').getValue()//補貨中停止販售
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
                          xtype: 'textfield',
                          id: 'vendor_name_full_OR_vendor_id',
                          labelWidth: 100,
                          fieldLabel: '供應商編號/名稱',
                          margin: '0 0 0 10',
                          listeners: {
                              specialkey: function (field, e) {
                                  if (e.getKey() == Ext.EventObject.ENTER) {
                                      Query();
                                  }
                              }
                          }
                      }
                ]
            }, {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                      {
                          xtype: 'textfield',
                          id: 'product_id_OR_product_name',
                          labelWidth: 100,
                          fieldLabel: '商品編號/名稱',
                          margin: '0 0 0 10',
                          listeners: {
                              specialkey: function (field, e) {
                                  if (e.getKey() == Ext.EventObject.ENTER) {
                                      Query();
                                  }
                              }
                          }
                      }
                ]
            }, {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                      {
                          xtype: 'textfield',
                          id: 'brand_id_OR_brand_name',
                          labelWidth: 100,
                          fieldLabel: '品牌編號/名稱',
                          margin: '0 0 0 10',
                          listeners: {
                              specialkey: function (field, e) {
                                  if (e.getKey() == Ext.EventObject.ENTER) {
                                      Query();
                                  }
                              }
                          }
                      }
                ]
            }, {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                      {
                          xtype: 'combobox',
                          margin: '0 0 0 10',
                          fieldLabel: '商品狀態',
                          store: prodStatusStore,
                          id: 'product_status',
                          queryMode: 'local',
                          displayField: 'parameterName',
                          valueField: 'parameterCode',
                          editable: false,
                          listeners: {
                              beforerender: function () {
                                  prodStatusStore.load({
                                      callback: function () {

                                          prodStatusStore.insert(0, { parameterCode: '10', parameterName: '全部' });
                                          Ext.getCmp('product_status').setValue(prodStatusStore.data.items[0].data.parameterCode);
                                          //alert(prodStatusStore.data.items[0].data.parameterCode);
                                      }
                                  });
                              }
                          }
                      }
                ]
            }, {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [{
                    xtype: 'numberfield',
                    id: 'item_stock_start',
                    margin: '0 0 0 10',
                    fieldLabel: '庫存數量',
                    value: 0,
                    minValue: -1000000,
                    maxValue: 99999,
                    anchor: '100%',
                    listeners: {
                        change: function (field, value) {
                            if (e.getKey() == Ext.EventObject.ENTER) {
                                Query();
                            }
                        },
                    }
                }, {
                    xtype: 'displayfield',
                    margin: '2 0 0 8',
                    value: '~ ~'
                }, {
                    xtype: 'numberfield',
                    anchor: '100%',
                    id: 'item_stock_end',
                    labelWidth: 100,
                    margin: '0 0 0 10',
                    value: 0,
                    minValue: -1000000,
                    maxValue: 99999,
                    listeners: {
                        change: function (field, value) {
                            if (e.getKey() == Ext.EventObject.ENTER) {
                                Query();
                            }
                        },
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
                    fieldLabel: '補貨中停止販售',
                    xtype: 'radiogroup',
                    id: 'ignore_stockRdo',
                    labelWidth: 100,
                    margin: '0 0 0 10',
                    width: 200,
                    defaults: {
                        name: 'ignore_stockVal'
                    },
                    columns: 2,
                    items: [
                    { id: 'id1', boxLabel: "是", inputValue: '1', checked: true },
                    { id: 'id2', boxLabel: "否", inputValue: '0' }
                    ]
                }
                ]
            }
        ],
        buttonAlign: 'left',
        buttons: [
                {
                    text: '查詢',
                    margin: '0 10 0 10',
                    iconCls: 'icon-search',
                    handler: function () {
                        Query();
                    }
                },
                {
                    text: '重置',
                    iconCls: 'ui-icon ui-icon-reset',
                    handler: function () {
                        this.up('form').getForm().reset();
                        Ext.getCmp('vendor_name_full_OR_vendor_id').setValue('');
                        Ext.getCmp('product_id_OR_product_name').setValue('');
                        Ext.getCmp('brand_id_OR_brand_name').setValue('');
                        Ext.getCmp('product_status').setValue('10');
                        Ext.getCmp('item_stock_start').setValue('0');
                        Ext.getCmp('item_stock_end').setValue('0');
                    }
                }
        ]
    });
    //第二個panel
    var IQGrid = Ext.create('Ext.grid.Panel', {
        id: 'IQGrid',
        store: ProductStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 9.4,
        columns: [
                { header: '商品編號', dataIndex: 'product_id' },
                { header: '商品名稱', dataIndex: 'product_name', width: 200 },
                { header: '商品細項編號', dataIndex: 'item_id' },
                { header: '商品規格', dataIndex: 'product_spec' },
                { header: '供應商編號', dataIndex: 'vendor_id' },
                { header: "供應商名稱", dataIndex: "vendor_name_full", width: 180 },
                { header: "品牌編號", dataIndex: "brand_id" },
                { header: "品牌名稱", dataIndex: "brand_name", width: 180 },
                { header: "商品狀態", dataIndex: "product_status_string" },
                { header: "庫存數量", dataIndex: "item_stock" },
                { header: "補貨中停止販售 ", dataIndex: "ignore_stock_string" },
        ],
        tbar: [
        { xtype: 'button', text: '匯出Excel', margin: '0 0 0 5', iconCls: 'icon-excel', id: 'btnExcel', handler: Export }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ProductStore,
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
        items: [searchForm, IQGrid],// 包含两个控件 
        autoScroll: true,
        renderTo: Ext.getBody(),
        listeners: {
            resize: function () {
                IQGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
})

/*************************************************************************************查询信息*************************************************************************************************/

function Query(x) {
    //alert(Ext.getCmp('product_status').getValue());
    var ignore_stockRdo = Ext.getCmp('ignore_stockRdo').getValue().ignore_stockVal;
    // alert(a);
    Ext.getCmp('IQGrid').store.loadPage(1, {
        params: {

        }
    });
}
/************匯入到Exce************/
function Export() {
    var vendor_name_full_OR_vendor_id = Ext.getCmp('vendor_name_full_OR_vendor_id').getValue();
    var product_id_OR_product_name = Ext.getCmp('product_id_OR_product_name').getValue();
    var brand_id_OR_brand_name = Ext.getCmp('brand_id_OR_brand_name').getValue();
    var product_status = Ext.getCmp('product_status').getValue();
    var item_stock_start = Ext.getCmp('item_stock_start').getValue();
    var item_stock_end = Ext.getCmp('item_stock_end').getValue();
    var ignore_stockRdo = Ext.getCmp('ignore_stockRdo').getValue().ignore_stockVal;
    window.open("/Product/ExportCSV?vendor_name_full_OR_vendor_id=" + vendor_name_full_OR_vendor_id + "&item_stock_end=" + item_stock_end + "&product_id_OR_product_name=" + product_id_OR_product_name + "&brand_id_OR_brand_name=" + brand_id_OR_brand_name + "&item_stock_start=" + item_stock_start + "&ignore_stockRdo=" + ignore_stockRdo + "&product_status=" + product_status);
}