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

        { name: "product_id", type: "int" },//商品編號
        { name: "product_name", type: "string" },//商品名稱
        { name: "item_id", type: "int" },//商品細項編號
        { name: "product_spec", type: "string" },//商品規格
        { name: "product_status", type: "int" },//商品狀態
        { name: "product_status_string", type: "string" },//商品狀態顯示
        { name: "product_createdate", type: "int" },//商品建立日期
        //商品類型
        { name: "product_freight_set", type: "string" },//溫層
        { name: "process_type", type: "int" },//出貨方式
        { name: "product_start", type: "int" },//商品上架時間

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
var ProductStatusStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "全部", "value": "100" },
        { "txt": "新建立商品", "value": "0" },
        { "txt": "申請審核", "value": "1" },
        { "txt": "審核通過", "value": "2" },
        { "txt": "上架", "value": "5" },
        
    ]
});

// 出貨方式
var OutProductStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "實體商品", "value": "1" },
        { "txt": "電子商品", "value": "2" },
    ]
});

//  溫層 
var freightStore = Ext.create('Ext.data.Store', {
    fields: ['parameterName', 'ParameterCode'],
    data: [
           { 'parameterName': '全部', 'ParameterCode': '0' },
           { 'parameterName': '常溫', 'ParameterCode': '1' },
           { 'parameterName': '冷凍', 'ParameterCode': '2' },
    ]
});

//列表頁加載時候得到的數據
WareHouseStore.on('beforeload', function () {
    Ext.apply(WareHouseStore.proxy.extraParams,
        {
            //vendor_name_full_OR_vendor_id: Ext.getCmp('vendor_name_full_OR_vendor_id').getValue(),//供應商名稱或編號
            //product_id_OR_product_name: Ext.getCmp('product_id_OR_product_name').getValue(),//商品編號或名稱
            //brand_id_OR_brand_name: Ext.getCmp('brand_id_OR_brand_name').getValue(),//品牌編號或名稱
            //product_status: Ext.getCmp('product_status').getValue(),//商品狀態
            //sale_status: Ext.getCmp('sale_status').getValue(),//商品販售狀態
            //item_stock_start: Ext.getCmp('item_stock_start').getValue(),//庫存數量開始
            //item_stock_end: Ext.getCmp('item_stock_end').getValue(),//庫存數量結束
            //ignore_stockRdo: Ext.getCmp('ignore_stockRdo').getValue()//庫存為0時是否還能販售
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
                               xtype: 'datefield',
                               margin: '5 0 0 5',
                               fieldLabel: '時間區間',
                               labelWidth: 60,
                               id: 'start_time',
                               format: 'Y-m-d',
                               width: 170,
                               value: Tomorrow(1 - new Date().getDate()),
                               editable: false,
                               listeners: {
                                   select: function (a, b, c) {
                                       var start = Ext.getCmp("start_time");
                                       var end = Ext.getCmp("end_time");
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
                    margin: '5 0 0 5',
                    value: '~'
                },
                {
                    xtype: 'datefield',
                    id: 'end_time',
                    margin: '5 0 0 5',
                    format: 'Y-m-d',
                    width: 110,
                    value: Tomorrow(0),
                    editable: false,
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("start_time");
                            var end = Ext.getCmp("end_time");
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
                },  {
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
                             value: 0,
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
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
           {
               xtype: 'combobox',
               name: 'Search_type',
               id: 'Search_type',
               editable: false,
               fieldLabel: "商品狀態",
               labelWidth: 60,
               margin: '5 0 0 5',
               store: ProductStatusStore,
               queryMode: 'local',
               submitValue: true,
               displayField: 'txt',
               valueField: 'value',
               typeAhead: true,
               forceSelection: false,
               value: 100
           },
                ]
            }
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
                                    Ext.getCmp("time_start").setMinValue(datetime1);
                                    Ext.getCmp("time_start").setMaxValue(datetime2);
                                    Ext.getCmp("time_end").setMinValue(datetime1);
                                    Ext.getCmp("time_end").setMaxValue(datetime2);
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
                { header: '商品編號', dataIndex: 'product_id', width: 80, align: 'center' },
                { header: "商品建立日期", dataIndex: 'product_createdate', align: 'center' },
                { header: '商品名稱', dataIndex: 'product_name', width: 180, align: 'center' },
                { header: '商品細項編號', dataIndex: 'item_id', align: 'center' },
                { header: '商品規格', dataIndex: 'product_spec', width: 120, align: 'center' },
                //商品類型
                //出貨方式
                { header: "商品狀態", dataIndex: "product_status_string", align: 'center' },
                {
                    header: "溫層", dataIndex: 'product_freight_set', width: 40, align: 'center',
                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                        if (value == "1") {
                            return "常溫";
                        } if (value == "2") { return "冷凍"; }
                    }

                },
                { header: "商品上架時間", dataIndex: "product_start", align: 'center' }

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
function Tomorrow(s) {
    var d;
    d = new Date();                             // 创建 Date 对象。                               // 返回日期。
    d.setDate(d.getDate() + s);
    return d;
}

/************匯出到Exce************/
function Export() {
    //var vendor_name_full_OR_vendor_id = Ext.getCmp('vendor_name_full_OR_vendor_id').getValue();
    //var product_id_OR_product_name = Ext.getCmp('product_id_OR_product_name').getValue();
    //var brand_id_OR_brand_name = Ext.getCmp('brand_id_OR_brand_name').getValue();
    //var product_status = Ext.getCmp('product_status').getValue();
    //var sale_status = Ext.getCmp('sale_status').getValue();
    //var item_stock_start = Ext.getCmp('item_stock_start').getValue();
    //var item_stock_end = Ext.getCmp('item_stock_end').getValue();
    //var ignore_stockRdo = Ext.getCmp('ignore_stockRdo').getValue().ignore_stockVal;
    //window.open("/Product/ExportCSV?vendor_name_full_OR_vendor_id=" + vendor_name_full_OR_vendor_id + "&sale_status=" + sale_status + "&item_stock_end=" + item_stock_end + "&product_id_OR_product_name=" + product_id_OR_product_name + "&brand_id_OR_brand_name=" + brand_id_OR_brand_name + "&item_stock_start=" + item_stock_start + "&ignore_stockRdo=" + ignore_stockRdo + "&product_status=" + product_status);
}