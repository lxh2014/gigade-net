var CallidForm;
var pageSize = 25;

//密碼Model
Ext.define('gigade.Product', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "product_id", type: "string" },
    { name: "product_name", type: "string" },
    { name: "product_detail_text", type: "string" },
    { name: "vendor_id", type: "string" },
    { name: "vendor_name_full", type: "string" },
    { name: "vendor_name_simple", type: "string" },
    { name: "brand_id", type: "string" },
    { name: "brand_name", type: "string" },
    { name: "prod_classify", type: "string" },
    { name: "product_status", type: "string" },
    { name: "create_username", type: "string" },
    { name: "detail_createdate", type: "datetime" }
    ]
});
/*********參數表model***********/
Ext.define("gigade.paraModel", {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'parameterCode', type: 'string' },
    { name: 'parameterName', type: 'string' }
    ]
});
var ProductStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Product',
    proxy: {
        type: 'ajax',
        url: '/Product/GetProductList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
//品牌Model
Ext.define("gigade.Brand", {
    extend: 'Ext.data.Model',
    fields: [
    { name: "Brand_Id", type: "string" },
    { name: "Brand_Name", type: "string" }]
});

//品牌store
var brandStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Brand',
    autoLoad: true,
    //filterOnLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Product/GetVendorBrand",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
//供應商Model
Ext.define("gigade.Vendor", {
    extend: 'Ext.data.Model',
    fields: [
    { name: "vendor_id", type: "string" },
    { name: "vendor_name_simple", type: "string" }]
});
//供應商Store
var VendorStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Vendor',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Product/GetVendor",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
//供應商Store
var StatusStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { "txt": VALUE_ALL, "value": "0" },
    { "txt": THIS_NOT_EDIT, "value": "1" },
    { "txt": THIS_IS_EDIT, "value": "2" }]
});

//商品類型Store
var ComboStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryPara?paraType=combo_type',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});
//供應商狀態 vendor表
var VendorBoxStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { "txt": VALUE_ALL, "value": "0" },
    { "txt": THIS_IS_START_USE, "value": "1" },
    { "txt": THIS_NOT_START_USE, "value": "2" },
    ]
});
//品牌狀態 vendor_brand表
var ProductBoxStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { "txt": VALUE_ALL, "value": "0" },
    { "txt": PIC_SHOW, "value": "1" },
    { "txt": PIC_HIDE, "value": "2" }
    ]
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdProduct").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdProduct").down('#link').setDisabled(selections.length == 0);
        }
    }
});
Ext.define("gigade.paraModel", {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'parameterCode', type: 'string' },
    { name: 'parameterName', type: 'string' }
    ]
});
var statusStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryPara?paraType=product_status',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});
ProductStore.on('beforeload', function () {
    ProductStore.removeAll();
    var creater_o = Ext.getCmp('creater').getValue();
    creater_o = creater_o.replace(/[，]/g, ",");
    var searchContent_o = Ext.getCmp('search_content').getValue();
    searchContent_o = searchContent_o.replace(/[，]/g, ",");
    Ext.apply(ProductStore.proxy.extraParams, {
        vendor_id: Ext.getCmp('vendor_id').getValue(),
        brand_id: Ext.getCmp('brand_id').getValue(),
        status: Ext.getCmp('status').getValue(),
        product_status: Ext.getCmp('product_status').getValue(),
        search_content: searchContent_o == null ? "" : searchContent_o,
        creater: creater_o == null ? "" : creater_o,
        timestart: Ext.getCmp('timestart').getValue(),
        timeend: Ext.getCmp('timeend').getValue(),
        combination: Ext.getCmp('combination').getValue(),
        vendorState: Ext.getCmp('vendorState').getValue(),
        brandState: Ext.getCmp('brandState').getValue()
    });
});
brandStore.on('beforeload', function () {
    Ext.apply(ProductStore.proxy.extraParams, {
        vendor_id: Ext.getCmp('vendor_id') == null ? 0 : Ext.getCmp('vendor_id').getValue()
    });
});
Ext.onReady(function () {
    Ext.tip.QuickTipManager.init();
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
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
                fieldLabel: '供應商',
                labelWidth: 50,
                defaultListConfig: {              //取消loading的Mask
                    loadMask: false,
                    loadingHeight: 70,
                    minWidth: 70,
                    maxHeight: 300,
                    shadow: "sides"
                },
                editable: false,
                margin: '0 5 3 0',
                id: 'vendor_id',
                name: 'vendor_id',
                store: VendorStore,
                displayField: 'vendor_name_simple',
                valueField: 'vendor_id',
                typeAhead: true,
                forceSelection: true,
                emptyText: SELECT,
                listeners: {
                    select: function (records) {
                        Ext.getCmp('brand_id').reset();
                        brandStore.load({ params: { vendor_id: Ext.getCmp('vendor_id').getValue() } });
                    }
                }
            },
            {//品牌
                xtype: 'combobox',
                fieldLabel: BRAND,
                labelWidth: 40,
                defaultListConfig: {              //取消loading的Mask
                    loadMask: false,
                    loadingHeight: 70,
                    minWidth: 70,
                    maxHeight: 300,
                    shadow: "sides"
                },
                id: 'brand_id',
                name: 'brand_id',
                colName: 'brand_id',
                editable: true,
                store: brandStore,
                margin: '0 5 3 10',
                queryMode: 'local',
                displayField: 'Brand_Name',
                valueField: 'Brand_Id',
                typeAhead: true,
                triggerAction: 'all',
                forceSelection: true,
                emptyText: SELECT
            },
            {
                xtype: 'combobox',
                fieldLabel: PRODUCT_TYPE,
                id: 'combination',
                name: 'combination',
                colName: 'combination',
                width: 195,
                //hidden: true,
                labelWidth: 60,
                margin: '0 5 3 0',
                store: ComboStore,
                queryMode: 'local',
                displayField: 'parameterName',
                valueField: 'parameterCode',
                editable: false,
                listeners: {
                    beforerender: function () {
                        ComboStore.load({
                            callback: function () {
                                ComboStore.insert(0, { parameterCode: '0', parameterName: VALUE_ALL });
                                Ext.getCmp("combination").setValue(ComboStore.data.items[0].data.parameterCode);
                            }
                        });
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
                    id: 'vendorState',
                    fieldLabel: DELIVER_STATUS,
                    labelWidth: 100,
                    width: 205,
                    margin: '0 5 3 0',
                    queryMode: 'local',
                    editable: false,
                    store: VendorBoxStore,
                    displayField: 'txt',
                    valueField: 'value',
                    value: 0
                },
              {
                  xtype: 'combobox',
                  id: 'brandState',
                  fieldLabel: BRAND_STATUS,
                  labelWidth: 60,
                  margin: '0 5 3 0',
                  queryMode: 'local',
                  editable: false,
                  store: ProductBoxStore,
                  displayField: 'txt',
                  valueField: 'value',
                  value: 0
              },
            {
                xtype: 'combobox', //status
                fieldLabel: PRODUCT_STATUS,
                labelWidth: 60,
                width: 195,
                editable: false,
                id: 'product_status',
                margin: '0 5 3 0',
                store: statusStore,
                displayField: 'parameterName',
                valueField: 'parameterCode',
                typeAhead: true,
                forceSelection: false,
                allowBlank: true,
                emptyText: SELECT
            }

            ]
        },
        {
            xtype: 'fieldcontainer',
            combineErrors: true,
            layout: 'hbox',
            items: [
            {
                xtype: 'textfield', id: 'creater', fieldLabel: PRODUCT_KUSER, labelWidth: 70, width: 205, margin: '0 5 3 0',
                listeners: {
                    specialkey: function (field, e) {
                        if (e.getKey() == e.ENTER) {
                            Query(1);
                        }
                    }
                }
            },
            {
                xtype: 'datetimefield',
                fieldLabel: PRODUCT_CREATE,
                id: 'timestart',
                name: 'timestart',
                labelWidth: 60,
                margin: '0 5 3 0',
                format: 'Y-m-d H:i:s',
                editable: false,
                time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
                listeners: {
                    select: function () {
                        var start = Ext.getCmp("timestart");
                        var end = Ext.getCmp("timeend");
                        if (end.getValue() == null) {
                            end.setValue(setNextMonth(start.getValue(), 1));
                        } else if (start.getValue() > end.getValue()) {
                            Ext.Msg.alert(INFORMATION, START_DATE_CANT_GT_END_DATE);
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }
                        else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                            // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }
                    },
                    specialkey: function (field, e) {
                        if (e.getKey() == e.ENTER) {
                            Query();
                        }
                    }
                }

            },
            {
                xtype: 'displayfield',
                value: '~',
                labelWidth: 20,
                margin: '0 10 3 7'
            },
            {
                xtype: 'datetimefield',
                id: 'timeend',
                name: 'timeend',
                format: 'Y-m-d H:i:s',
                editable: false,
                time: { hour: 23, min: 59, sec: 59 },//標記結束時間23:59:59
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("timestart");
                        var end = Ext.getCmp("timeend");
                        var s_date = new Date(start.getValue());
                        var now_date = new Date(end.getValue());
                        if (start.getValue() != "" && start.getValue() != null) {
                            if (end.getValue() < start.getValue()) {
                                Ext.Msg.alert(INFORMATION, START_DATE_CANT_LT_END_DATE);
                                end.setValue(setNextMonth(start.getValue(), 1));
                            } else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                //Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                start.setValue(setNextMonth(end.getValue(), -1));
                            }

                        } else {
                            start.setValue(setNextMonth(end.getValue(), -1));
                        }

                    },
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
            //align:'center',
            layout: 'hbox',
            items: [
                 {
                     xtype: 'combobox',
                     id: 'status',
                     fieldLabel: EDIT_STATUS,
                     labelWidth: 70,
                     width: 205,
                     margin: '0 5 3 0',
                     queryMode: 'local',
                     editable: false,
                     store: StatusStore,
                     displayField: 'txt',
                     valueField: 'value',
                     value: 0
                 },
            {
                xtype: 'textfield', fieldLabel: PRODUCT_CODE_AND_NAME, labelWidth: 90, width: 215, margin: '0 5 3 0', id: 'search_content', listeners: {
                    specialkey: function (field, e) {
                        if (e.getKey() == e.ENTER) {
                            Query(1);
                        }
                    }
                }
            },
            {
                xtype: 'button',
                margin: '0 10 3 30',
                iconCls: 'ui-icon ui-icon-search-2',
                text: BTN_SEARCH,
                handler: Query
            },
            {
                xtype: 'button',
                text: THIS_RESET,
                id: 'btn_reset',
                iconCls: 'ui-icon ui-icon-reset',
                listeners: {
                    click: function () {
                        frm.getForm().reset();
                        Ext.getCmp('combination').setValue(0),
                        brandStore.removeAll();
                        brandStore.load();
                    }
                }
            }
            ]
        }

        ]
    });
    var gdProduct = Ext.create('Ext.grid.Panel', {
        id: 'gdProduct',
        store: ProductStore,
        // width: document.documentElement.clientWidth,
        columnLines: true,
        // height: document.documentElement.clientHeight - 140,
        frame: true,
        columns: [
        { header: PRODUCTID, dataIndex: 'product_id', flex: 3, align: 'center' },
        {
            header: PRODUCT_NAME, dataIndex: 'product_name', flex: 7, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                cellmeta.style = 'overflow:visible;padding:3px 3px 3px 5px;white-space:normal';
                return value;
            }
        }
        ,
        { header: PRODUCT_KUSER, dataIndex: 'create_username', flex: 1, align: 'center' }
        ,
        { header: PRODUCT_CREATE, dataIndex: 'detail_createdate', flex: 2, align: 'center' }
        ],
        tbar: [
        { xtype: 'button', text: THIS_EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },

        { xtype: 'button', text: OPEN_FRONT_PRODUCT_PAGE, id: 'link', hidden: false, disabled: true, handler: ProductPreview },
        {
            text: THIS_OUT,
            iconCls: 'icon-excel',
            id: 'btnExcel',
            handler: onExport
        }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ProductStore,
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
        },
        selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm, gdProduct],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdProduct.width = document.documentElement.clientWidth;
                gdProduct.height = document.documentElement.clientHeight - 150;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //ProductStore.load({ params: { start: 0, limit: 25 } });
});
/*************************************************************************************查詢*************************************************************************************************/
function Query(x) {
    ProductStore.removeAll();
    var creater_r = Ext.getCmp('creater').getValue();
    creater_r = creater_r.replace(/[，]/g, ",");
    var searchContent = Ext.getCmp('search_content').getValue();
    searchContent = searchContent.replace(/[，]/g, ",");
    Ext.getCmp("gdProduct").store.loadPage(1);
}
/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdProduct").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editProductDetailFunction(row[0], ProductStore);
    }
}

onExport = function () {
    Ext.MessageBox.show({
        msg: 'Loading...',
        wait: true
    });
    var creater_e = Ext.getCmp('creater').getValue();
    creater_e = creater_e.replace(/[，]/g, ",");
    var searchContent_e = Ext.getCmp('search_content').getValue();
    searchContent_e = searchContent_e.replace(/[，]/g, ",");
    Ext.Ajax.request({
        url: "/Product/ProductDetailExport",
        timeout: 900000,
        params: {
            vendor_id: Ext.getCmp('vendor_id').getValue(),
            brand_id: Ext.getCmp('brand_id').getValue(),
            status: Ext.getCmp('status').getValue(),
            product_status: Ext.getCmp('product_status').getValue(),
            search_content: searchContent_e == null ? "" : searchContent_e,
            creater: creater_e == null ? "" : creater_e,
            timestart: Ext.getCmp('timestart').getValue(),
            timeend: Ext.getCmp('timeend').getValue(),
            combination: Ext.getCmp('combination').getValue(),
            vendorState: Ext.getCmp('vendorState').getValue(),
            brandState: Ext.getCmp('brandState').getValue()
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                window.location = '../../ImportUserIOExcel/' + result.ExcelName;
                Ext.MessageBox.hide();
            } else {
                Ext.MessageBox.hide();
                Ext.Msg.alert(INFORMATION, OUT_DATA_FAILED_OR_NO_DATA);
            }
        }
    });
}
function ProductPreview() {
    var row = Ext.getCmp("gdProduct").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        var product_id = row[0].data.product_id;
        var type = 0;
        var prod_Classify = row[0].data.prod_classify;
        Ext.Ajax.request({
            url: '/ProductList/ProductPreview',
            params: { Product_Id: product_id, Type: type, Prod_Classify: prod_Classify },
            success: function (form, action) {
                var result = form.responseText;
                //var wl = "<a href=" + result + " target='new'>" + result + "</a>";
                if (result != NO_PREVIEW_PRODUCT_MESSAGE) {
                    window.open(result, '_blank');
                }
                else {
                    Ext.Msg.alert(INFORMATION, PRODUCT_PARALLELISM_NOT_IN_FRONT);
                }
            }
        })
    }

}



setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    } else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}





