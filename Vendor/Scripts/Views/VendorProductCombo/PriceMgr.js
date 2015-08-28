var mainPanel;
var editPanel;
var itemsNew
var showComboPanel;
var OLD_PRODUCT_ID, product_id;

var combination = 0;
var PRICETYPE = 0;
var isNew = true;
var IsEdit;



///價格類型model
Ext.define("gigade.PriceType", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'parameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
});
//價格類型model 
var priceTypeStore = Ext.create('Ext.data.Store', {
    fields: ['parameterName', 'parameterCode'],
    data: [
        { "parameterName": '照單一商品售價比例拆分', "parameterCode": "1" }
    ]
});




Ext.define('GIGADE.PRODUCTSITE', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'price_master_id', type: 'string' },
        { name: 'product_id', type: 'string' },
        { name: 'site_id', type: 'string' },
        { name: 'site_name', type: 'string' },
        { name: 'product_name', type: 'string' },
        { name: 'price_status', type: 'string' },
        { name: 'user_level_name', type: 'string' },
        { name: 'user_level', type: 'string' },
        { name: 'user_id', type: 'string' },
        { name: 'user_email', type: 'string' },
        { name: 'cost', type: 'string' },
        { name: 'price', type: 'string' },
        { name: 'event_cost', type: 'string' },
        { name: 'event_price', type: 'string' },
        { name: 'event_start', type: 'string' },
        { name: 'event_end', type: 'string' },
        { name: 'status', type: 'string' },
        { name: 'same_price', type: 'string' },
        { name: 'valid_start', type: 'string' },
        { name: 'valid_end', type: 'string' }
    ]
});
var siteProductStore = Ext.create("Ext.data.Store", {
    model: 'GIGADE.PRODUCTSITE',
    proxy: {
        type: 'ajax',
        url: '/VendorProductCombo/GetPriceMaster',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
});

Ext.define('GIGADE.PRODUCT', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'Parent_Id', type: 'string' },
        { name: 'Child_Id', type: 'string' },
        { name: 'Product_Name', type: 'string' },
        { name: 'item_id', type: 'string' },
        { name: 'spec_1', type: 'string' },
        { name: 'spec_2', type: 'string' },
        { name: 'price_master_id', type: 'string' },
        { name: 'item_price_id', type: 'string' },
        { name: 'item_money', type: 'int' },
        { name: 'event_money', type: 'int' },
        { name: 'item_cost', type: 'int' },
        { name: 'event_cost', type: 'int' },
        { name: 'Pile_Id', type: 'string' },
        { name: 'S_Must_Buy', type: 'string' },
        { name: 'G_Must_Buy', type: 'string' },
        { name: 'total_price', type: 'int' }
    ]
});
var product_store = Ext.create('Ext.data.Store', {
    model: 'GIGADE.PRODUCT',
    groupField: 'Pile_Id'
});

var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
    clicksToEdit: 1
});



//列选择模式
var sm = Ext.create('Ext.selection.CheckboxModel', {
    mode: 'SINGLE',
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("siteProdutctGrid").down('#update_site_price').setDisabled(selections.length == 0);
        }
    }
});

var columns = [{
    header: PRODUCTID,
    //  hidden: true,
    sortable: false,
    muiltName: 'product_id',
    menuDisabled: true,
    dataIndex: 'product_id'
}, {
    header: PRODUCTNAME,
    sortable: false,
    width: 300,
    //  hidden: true,
    muiltName: 'product_name',
    menuDisabled: true,
    dataIndex: 'product_name'
}, {
    header: ITEMPRICE,
    muiltName: 'item_price',
    sortable: false,
    //  hidden: true,
    menuDisabled: true,
    dataIndex: 'price'
}
, {
    header: MUSTBUY,
    muiltName: 's_must_buy',
    sortable: false,
    //  hidden: true,
    menuDisabled: true,
    dataIndex: 's_must_buy'
}
];



Ext.onReady(function () {

    product_id = window.parent.GetProductId();
    OLD_PRODUCT_ID = window.parent.GetCopyProductId();
    IsEdit = window.parent.GetIsEdit();
    var combination = 0;
    Ext.Ajax.request({
        url: '/VendorProductCombo/QueryProduct',
        method: 'POST',
        async: false,
        params: {
            ProductId: product_id,
            OldProductId: OLD_PRODUCT_ID
        },
        success: function (response, opts) {
            var resText = eval("(" + response.responseText + ")");
            if (resText != null && resText.data != null) {
                combination = resText.data.combination;
            }
        }
    });


    /****************新增介面*******************************/
    priceTypeStore.load();

    mainPanel = Ext.create("Ext.form.Panel", {
        id: 'mainpanel',
        hidden: IsEdit == "true",
        border: false,
        width: 800,
        bodyStyle: 'padding:5px 5px 0px 5px',
        defaults: {
            labelWidth: 120
        },
        buttons: [{
            text: SAVE,
            width: 100,
            id: 'newPriceSave',
            hidden: window.parent.GetIsEdit() == "false",
            listeners: {
                click: function () {
                    save();
                }
            }
        }, {
            text: '取消',
            width: 100,
            hidden: window.parent.GetIsEdit() == "false",
            listeners: {
                click: function () {
                    mainPanel.hide();
                    window.parent.setMoveEnable(true);
                    mainPanel.getForm().reset();
                    editPanel.show();
                    showComboPanel.show();
                    siteProductStore.load({ params: { ProductId: product_id} });
                }
            }
        }],
        items: [
            {
                xtype: 'displayfield',
                id: 'price_master_id',
                name: 'price_master_id',
                hidden: true
            },
            {
                xtype: 'panel',
                hidden: IsEdit == "true",
                defaults: {
                    labelWidth: 120
                },
                border: false,
                items: [{
                    xtype: 'combobox',
                    fieldLabel: PRICE_TYPE,
                    store: priceTypeStore,
                    id: 'price_type',
                    queryMode: 'local',
                    muiltName: 'price_type',
                    readyOnly: true,
                    //hidden: true,
                    name: 'Price_Type',
                    displayField: 'parameterName',
                    valueField: 'parameterCode',
                    editable: false,
                    listeners: {
                        afterrender: function () {
                            priceTypeStore.load({
                                callback: function () {
                                    Ext.getCmp("price_type").setValue(priceTypeStore.data.items[0].data.parameterCode);
                                }
                            });
                        },
                        change: function (a, newValue, oldValue, eOpts) {
                            if (IsEdit == "true") return;
                            if (newValue == 1) {
                                //Ext.getCmp("same_price").hide();
                                //  Ext.getCmp("GridPanel").hide();
                            }

                            change(newValue, combination);

                        }
                    }
                }]

            },
            {
                xtype: 'textfield',
                fieldLabel: PRODUCTSHOENAME + '<span style="color:red">*</span>',
                id: 'product_name',
                //  hidden: true,
                name: 'product_name',
                muiltName: 'product_name',
                width: 400,
                allowBlank: false
            },
            {
                xtype: 'panel',
                hidden: IsEdit == "true",
                border: false,
                defaults: {
                    labelWidth: 120
                },
                items: [{
                    xtype: 'numberfield',
                    decimalPrecision: 0,
                    minValue: 0,
                    value: 0,
                    //  hidden: true,
                    fieldLabel: PRICELIST + '<span style="color:red">*</span>',
                    id: 'product_price_list',
                    name: 'product_price_list',
                    muiltName: 'product_price_list',
                    allowBlank: false
                }, {
                    xtype: 'numberfield',
                    decimalPrecision: 0,
                    minValue: 0,
                    value: 0,
                    fieldLabel: BAG_CHECK_MONEY,
                    id: 'bag_check_money',
                    name: 'bag_check_money',
                    //  hidden: true,
                    muiltName: 'bag_check_money'
                }]
            },
            {
                xtype: 'panel',
                border: false,
                defaults: {
                    labelWidth: 120
                },
                muiltName: 'price',
                //  hidden: true,
                height: 27,
                items: [
            {
                xtype: 'numberfield',
                decimalPrecision: 0,
                minValue: 0,
                value: 0,
                fieldLabel: PRICE + '<span style="color:red">*</span>',
                id: 'price',
                name: 'price',
                allowBlank: false
            }, {
                xtype: 'displayfield',
                fieldLabel: PRICE + '<span style="color:red">*</span>',
                value: '依選擇規格',
                id: 'price1',
                //  hidden: true,
                name: 'price1'
            }
            ]
            },
            {
                xtype: 'panel',
                muiltName: 'item_cost',
                defaults: {
                    labelWidth: 120
                },
                //  hidden: true,
                border: false,
                height: 27,
                items: [{
                    xtype: 'numberfield',
                    decimalPrecision: 0,
                    minValue: 0,
                    value: 0,
                    fieldLabel: COST + '<span style="color:red">*</span>',
                    id: 'cost',
                    muiltName: 'cost',
                    //  hidden: true,
                    allowBlank: false
                }, {
                    xtype: 'displayfield',
                    fieldLabel: COST + '<span style="color:red">*</span>',
                    value: '依選擇規格',
                    id: 'cost1',
                    //  hidden: true,
                    name: 'cost1'
                }]
            },
            {
                xtype: 'panel',
                border: false,
                defaults: {
                    labelWidth: 120
                },
                //  hidden: true,
                muiltName: 'event_price',
                height: 27,
                items: [
            {
                xtype: 'numberfield',
                decimalPrecision: 0,
                minValue: 0,
                value: 0,
                fieldLabel: EVENTPRICE,
                id: 'event_price',
                name: 'event_price'
            }, {
                xtype: 'displayfield',
                fieldLabel: EVENTPRICE,
                value: '依選擇規格',
                id: 'event_price1',
                hidden: true
            }
            ]
            },
            {
                xtype: 'numberfield',
                decimalPrecision: 0,
                minValue: 0,
                value: 0,
                fieldLabel: EVENT_COST,
                id: 'event_cost',
                muiltName: 'event_cost',
                //  hidden: true,
                allowBlank: false
            },
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                id: 'event_time',
                muiltName: 'event_time',
                //  hidden: true,
                fieldLabel: EVENT_TIME,
                items: [{
                    xtype: 'datetimefield',
                    disabledMin: true,
                    disabledSec: true,
                    format: 'Y-m-d H:i:s',
                    name: 'event_start',
                    id: 'event_start',
                    listeners: {

                }
            }, {
                xtype: 'displayfield',
                value: '~'
            }, {
                xtype: 'datetimefield',
                disabledMin: true,
                disabledSec: true,
                format: 'Y-m-d H:i:s',
                name: 'event_end',
                id: 'event_end',
                listeners: {

            }
        },
        {
            xtype: 'button',
            text: '清除時間',
            handler: function () {
                Ext.getCmp('event_start').setValue('');
                Ext.getCmp('event_end').setValue('');
            }
        }
        ]
    },
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                id: 'priceValidTime',
                muiltName: 'priceValidTime',
                //  hidden: true,
                fieldLabel: "上下架時間",
                items: [{
                    xtype: 'datetimefield',
                    disabledMin: true,
                    disabledSec: true,
                    format: 'Y-m-d H:i:s',
                    name: 'valid_start',
                    id: 'valid_start'
                }, {
                    xtype: 'displayfield',
                    value: '~'
                }, {
                    xtype: 'datetimefield',
                    disabledMin: true,
                    disabledSec: true,
                    format: 'Y-m-d H:i:s',
                    name: 'valid_end',
                    id: 'valid_end'
                },
                {
                    xtype: 'button',
                    text: '清除時間',
                    handler: function () {
                        Ext.getCmp('valid_start').setValue('');
                        Ext.getCmp('valid_end').setValue('');
                    }
                }
                ]
            },

            {
                xtype: 'panel',
                border: false,
                defaults: {
                    labelWidth: 120
                },
                items: [{
                    xtype: 'fieldcontainer',
                    hidden: IsEdit == "true",
                    defaultType: 'checkboxfield',
                    items: [{
                        boxLabel: FRONT_SHOW_PRICE,
                        inputValue: '2',
                        muiltName: 'show_listprice',
                        id: 'show_listprice',
                        name: 'show_listprice'
                    }]
                }]

            }],
        listeners: {
            beforerender: function () {
                //再次編輯同一條數據 加載數據內容
                if (IsEdit == "false") {
                    mainPanel.getForm().load({
                        type: 'ajax',
                        url: '/VendorProductCombo/QueryPriceMasterProduct',
                        actionMethods: 'post',
                        params: {
                            "ProductId": Ext.htmlEncode(window.parent.GetProductId()),
                            "OldProductId": OLD_PRODUCT_ID
                        },
                        success: function (response, opts) {
                            var resText = eval("(" + opts.response.responseText + ")");
                            if (!resText.data) return;

                            Ext.getCmp("price").setValue(resText.data.price);
                            Ext.getCmp("event_price").setValue(resText.data.event_price);

                            var event_start = resText.data.event_start;
                            if (event_start != "0") {
                                Ext.getCmp("event_start").setValue(new Date(event_start * 1000));
                            }
                            else {
                                Ext.getCmp("event_start").setValue();
                            }
                            var event_end = resText.data.event_end;
                            if (event_end != "0") {
                                Ext.getCmp("event_end").setValue(new Date(event_end * 1000));
                            }
                            else {
                                Ext.getCmp("event_end").setValue();
                            }
                            var valid_start = resText.data.valid_start;
                            var valid_end = resText.data.valid_end;
                            if (valid_start != "0") {
                                Ext.getCmp("valid_start").setValue(new Date(valid_start * 1000));
                            }
                            else {
                                Ext.getCmp("valid_start").setValue();
                            }

                            if (valid_end != "0") {
                                Ext.getCmp("valid_end").setValue(new Date(valid_end * 1000));
                            }
                            else {
                                Ext.getCmp("valid_end").setValue();
                            }

                        }
                    })
                }
            }
        }
    });
    /****************end*******************************/




    /****************修改介面*******************************/
    siteProductStore.load({ params: { ProductId: product_id} });
    editPanel = Ext.create("Ext.form.Panel", {
        id: 'editPanel',
        border: false,
        width: 950,
        height: 345,
        hidden: IsEdit == "false",
        autoScroll: true,
        defaults: {
            labelWidth: 110,
            padding: '2 0 0 0'
        },
        items: [
        {
            xtype: 'checkbox',
            boxLabel: FRONT_SHOW_PRICE,
            inputValue: '1',
            //  hidden: true,
            muiltName: 'show_pricelist2',
            id: 'show_pricelist2',
            name: 'show_pricelist2',
            checked: true,
            margin: '0 0 0 115'
        },
        {
            xtype: 'numberfield',
            fieldLabel: PRICELIST,
            //  hidden: true,
            id: 'Product_Price_List',
            muiltName: 'price_list',
            name: 'Product_Price_List'
        },
         {
             xtype: 'numberfield',
             decimalPrecision: 0,
             minValue: 0,
             value: 0,
             //  hidden: true,
             fieldLabel: BAG_CHECK_MONEY,
             id: 'bag_check_money1',
             name: 'bag_check_money1',
             muiltName: 'bag_check_money1'
         },
        {
            xtype: 'button',
            width: 80,
            id: 'comboSavePrice',
            text: SAVE,
            listeners: {
                click: function () {
                    Ext.Ajax.request({
                        url: '/VendorProductCombo/UpdatePrice',
                        method: 'post',
                        async: false,
                        params: {
                            "ProductId": product_id,
                            "product_price_list": Ext.getCmp("Product_Price_List").getValue(),
                            "bag_check_money": Ext.getCmp("bag_check_money1").getValue(),
                            "show_listprice": Ext.getCmp("show_pricelist2").getValue() ? "on" : "off"
                        },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            if (result.success) { Ext.Msg.alert(PROMPT, SUCCESS); }
                        },
                        failure: function (form, action) {
                            Ext.Msg.alert(PROMPT, FAILURE);
                        }
                    });
                }
            },
            margin: '0 0 0 115'
        },
        {
            xtype: 'displayfield',
            width: 400,
            //  hidden: true,
            fieldLabel: PRICE_TYPE,
            muiltName: 'price_type',
            id: 'price_type_name',
            name: 'price_type_name'
        },
         {
             xtype: 'gridpanel',
             id: 'siteProdutctGrid',
             store: siteProductStore,
             autoScroll: true,
             height: 200,
             selModel: sm,
             tbar: [{
                 text: UPDATE_SITE_PRICE,
                 iconCls: 'icon-edit',
                 id: 'update_site_price',
                 disabled: true,
                 handler: function () {
                     //xxl
                     editPanel.hide();
                     showComboPanel.hide();
                     mainPanel.show();
                     isNew = false;

                     var row = Ext.getCmp('siteProdutctGrid').getSelectionModel().getSelection();
                     mainPanel.getForm().loadRecord(row[0]);

                     change(PRICETYPE, combination); //根據價格類型、同價來確定頁面展現

                     //Ext.getCmp("product_price_list").setValue(Ext.getCmp("Product_Price_List").getValue());
                     if (row[0].data.price != '') {
                         Ext.getCmp('price').setValue(row[0].data.price);
                     }
                     if (row[0].data.event_price != '') {
                         Ext.getCmp('event_price').setValue(row[0].data.event_price);
                     }
                     if (row[0].data.event_cost != '') {
                         Ext.getCmp('event_cost').setValue(row[0].data.event_cost);
                     }

                     if (row[0].data.cost != '') {
                         Ext.getCmp('cost').setValue(row[0].data.cost);
                     }

                     var event_start = row[0].data.event_start;
                     if (event_start != 0) {
                         event_start = new Date(event_start * 1000);
                         Ext.getCmp("event_start").setValue(event_start);
                     }
                     else {
                         Ext.getCmp("event_start").setValue();
                     }
                     var event_end = row[0].data.event_end;
                     if (event_end != 0) {
                         event_end = new Date(event_end * 1000);
                         Ext.getCmp("event_end").setValue(event_end);
                     }
                     else {
                         Ext.getCmp("event_start").setValue();
                     }


                     var valid_start = row[0].data.valid_start;
                     var valid_end = row[0].data.valid_end;
                     if (valid_start != 0) {
                         Ext.getCmp("valid_start").setValue(new Date(valid_start * 1000));
                     }
                     else {
                         Ext.getCmp("valid_start").setValue();
                     }

                     if (valid_end != 0) {
                         Ext.getCmp("valid_end").setValue(new Date(valid_end * 1000));
                     }
                     else {
                         Ext.getCmp("valid_end").setValue();
                     }


                     window.parent.setMoveEnable(false);
                     /****/
                 }
             }],
             columns: [
                    { muiltName: 'site_name', header: SITENAME, dataIndex: 'site_name', width: 100, align: 'center', sortable: false, menuDisabled: true },
                    { muiltName: 'product_name', header: PRODUCTNAME, dataIndex: 'product_name', width: 180, align: 'center', sortable: false, menuDisabled: true },
                    { muiltName: 'user_level', header: USERLEVEL, dataIndex: 'user_level_name', width: 100, align: 'center', sortable: false, menuDisabled: true },
                    { header: USERLEVEL, dataIndex: 'user_level', width: 100, align: 'center', sortable: false, menuDisabled: true, hidden: true },
                    { dataIndex: 'user_id', width: 100, align: 'center', sortable: false, menuDisabled: true, hidden: true },
                    { muiltName: 'user_email', header: USERMAIL, dataIndex: 'user_email', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: true },
                    { muiltName: 'price_status', header: PRICESTATUS, dataIndex: 'status', width: 100, align: 'center', sortable: false, menuDisabled: true },             //edit by hjj 2014/08/13 將固定組合商品與單一商品售價與成本的順序一致化
                    {muiltName: 'price', header: PRICE, dataIndex: 'price', width: 100, align: 'center', sortable: false, menuDisabled: true },
                    { muiltName: 'item_cost', header: COST, dataIndex: 'cost', width: 100, align: 'center', sortable: false, menuDisabled: true },

                    { muiltName: 'event_price', header: EVENTPRICE, dataIndex: 'event_price', width: 100, align: 'center', sortable: false, menuDisabled: true },
                    { muiltName: 'event_cost', header: EVENT_COST, dataIndex: 'event_cost', width: 100, align: 'center', sortable: false, menuDisabled: true },
                    { muiltName: 'event_time', header: EVENT_TIME, xtype: 'templatecolumn',
                        tpl: Ext.create('Ext.XTemplate',
                            '{[values.event_start==0?"":Ext.Date.format(new Date(values.event_start * 1000),"Y/m/d H:i:s")]}',
                            '~',
                            '{[values.event_end==0?"":Ext.Date.format(new Date(values.event_end * 1000),"Y/m/d H:i:s")]}'
                        ), width: 240, align: 'left', sortable: false, menuDisabled: true
                    }

                ]
         }
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }

        }
    });





    showComboPanel = Ext.create("Ext.form.Panel", {
        id: 'showComboPanel',
        border: false,
        // autoScroll: true,
        //height: 350,
        width: 950,
        hidden: IsEdit == "false",
        defaults: {
            labelWidth: 110,
            padding: '2 0 0 0'
        }
    })
    /***********************加載組合商品信息***************************/
    //查詢組合類型
    Ext.Ajax.request({
        url: '/VendorProductCombo/QueryProduct',
        method: 'post',
        async: false,
        params: {
            "ProductId": Ext.htmlEncode(window.parent.GetProductId()),
            "OldProductId": OLD_PRODUCT_ID
        },
        success: function (response) {
            var resText = eval("(" + response.responseText + ")");
            if (!resText.data) return;
            combination = resText.data.Combination;
            //product_mode:2(寄倉),不為寄倉則不能設置寄倉費,組成商品不為同一vendor也不能設置寄倉費
            setBagCheckMoney(Ext.getCmp('bag_check_money1'), resText.data.Product_Mode);
            setBagCheckMoney(Ext.getCmp('bag_check_money'), resText.data.Product_Mode);
            if (IsEdit == "false") return;

            if (!resText.data) return;
            Ext.getCmp("Product_Price_List").setValue(resText.data.Product_Price_List);
            Ext.getCmp("bag_check_money1").setValue(resText.data.Bag_Check_Money);


            Ext.getCmp("show_pricelist2").setValue(resText.data.show_listprice == 1 ? true : false);
            priceTypeStore.load({
                callback: function () {
                    if (priceTypeStore.find("parameterCode", resText.data.Price_type) != -1) {
                        Ext.getCmp("price_type_name").setValue(priceTypeStore.getAt(priceTypeStore.find("parameterCode", resText.data.Price_type)).data.parameterName);
                        PRICETYPE = resText.data.Price_type;
                    }

                }
            });

            //固定組合和任選組合
            if (resText.data.Combination == 2 || resText.data.Combination == 3) {
                var combinationStore = Ext.create("Ext.data.Store", {
                    fields: ['product_id', 'product_name', 'price', 's_must_buy', 'g_must_buy'],
                    proxy: {
                        type: 'ajax',
                        url: '/VendorProductCombo/QuerySingleProPrice',
                        actionMethods: 'post',
                        reader: {
                            type: 'json',
                            root: 'data'
                        }
                    }
                });
                combinationStore.load({ params: { product_id: product_id, pile_id: 0} });
                var ficxed = Ext.create("Ext.grid.Panel", {
                    id: 'ComcoFixed',
                    store: combinationStore,
                    width: 600,
                    height: 200,
                    border: false,
                    columns: columns,
                    listeners: {
                        viewready: function () {
                            var rec = this.getStore().getAt(0);
                            if (rec) {
                                if (rec.data.g_must_buy != 0) {
                                    this.setTitle('<span style="color:red">必選數量:' + rec.data.g_must_buy + "</span>");
                                }
                            }
                            window.parent.setShow(showComboPanel, 'muiltName');
                        },
                        scrollershow: function (scroller) {
                            if (scroller && scroller.scrollEl) {
                                scroller.clearManagedListeners();
                                scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                            }
                        }
                    }
                });

                showComboPanel.add(ficxed);
            }
            if (resText.data.Combination == 4) {
                //查詢群組
                Ext.Ajax.request({
                    url: '/VendorProductCombo/groupNameQuery',
                    method: 'post',
                    params: {
                        "ProductId": Ext.htmlEncode(window.parent.GetProductId())
                    },
                    success: function (response) {
                        var resText = eval("(" + response.responseText + ")");
                        var pileCount = resText.length;


                        for (var i = 1; i <= pileCount; i++) {
                            /**********************Store*****************/
                            var storeName = "pileStore_" + i;
                            storeName = Ext.create("Ext.data.Store", {
                                fields: ['product_id', 'product_name', 'price', 's_must_buy', 'g_must_buy'],
                                proxy: {
                                    type: 'ajax',
                                    url: '/VendorProductCombo/QuerySingleProPrice',
                                    actionMethods: 'post',
                                    reader: {
                                        type: 'json',
                                        root: 'data'
                                    }
                                }
                            });
                            storeName.load({ params: { product_id: product_id, pile_id: resText[i - 1].Pile_Id} });
                            /*****************Grid***************************/
                            var pileGrid = Ext.create("Ext.grid.Panel", {
                                title: GROUP + i,
                                height: 200,
                                width: 600,
                                id: '' + i + '_ComcoFixed',
                                store: storeName,
                                border: false,
                                columns: columns,
                                listeners: {
                                    beforerender: function () {
                                        var me = this;
                                        me.getStore().on("load", function (records) {
                                            var rec = records.getAt(0);
                                            if (rec) {
                                                me.setTitle(me.title + '<span style="color:red;margin-left:40px">必選數量:' + rec.data.g_must_buy + "</span>");
                                            }
                                        });
                                    },
                                    scrollershow: function (scroller) {
                                        if (scroller && scroller.scrollEl) {
                                            scroller.clearManagedListeners();
                                            scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                                        }
                                    }
                                }
                            });

                            showComboPanel.add(pileGrid);

                            /*****************end******************************/
                        }
                        window.parent.setShow(showComboPanel, 'muiltName');
                    }
                });
            }


        }
    });
    /***********************end***************************/

    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [mainPanel, editPanel, showComboPanel],
        border: false,
        style: {
            padding: '5 0 0 5'
        },
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                this.doLayout();
            }
        }
    });

    //權限
    window.parent.updateAuth(mainPanel, 'muiltName');
    if (IsEdit == "true") {
        window.parent.updateAuth(editPanel, 'muiltName');
    }
})

function setBagCheckMoney(field, mode) {
    if (mode != 2) {
        field.setReadOnly(true);
        field.setValue(0);
    }
}

//保存至臨時表
function save() {
    var retVal = true;
    if (!Ext.getCmp("mainpanel").getForm().isValid()) {
        return false;
    }
    var start = Ext.getCmp("event_start").rawValue;
    var end = Ext.getCmp("event_end").rawValue;

    if (Ext.getCmp('price').getValue() == 0) {
        alert(ITEM_MONEY_EMPTY);
        
    }
    if (Ext.getCmp('cost').getValue() == 0) {
        alert(ITEM_COST_EMPTY);
       
    }

    if (IsEdit == "false") {
        if (Ext.getCmp("price_type").rawValue == SELECT) {
            Ext.Msg.alert(PROMPT, SELECT_PRICETYPE);
            return false;
        }
    }
    var mask;
    if (!mask) {
        mask = new Ext.LoadMask(Ext.getBody());
    }
    mask.show();


    var event_start = Ext.getCmp("event_start").rawValue == 0 ? "0" : Ext.getCmp("event_start").rawValue;
    var event_end = Ext.getCmp("event_end").rawValue == 0 ? "0" : Ext.getCmp("event_end").rawValue;

    var valid_start = Ext.getCmp('valid_start').rawValue == 0 ? "0" : Ext.getCmp('valid_start').rawValue;
    var valid_end = Ext.getCmp('valid_end').rawValue == 0 ? "0" : Ext.getCmp('valid_end').rawValue;


    var PriceType = PRICETYPE == 0 ? Ext.htmlEncode(Ext.getCmp("price_type").getValue()) : PRICETYPE;

    var price = Ext.htmlEncode(Ext.getCmp("price").getValue());
    var event_price = Ext.htmlEncode(Ext.getCmp("event_price").getValue());

    var priceStr;
    var childId = "";
    var MustBuy = "";

    minPrice = 0, maxPrice = 0, maxEventPrice = 0;
    Ext.Ajax.request({
        url: '/VendorProductCombo/ComboPriceSave',
        method: 'POST',
        async: false,
        params: {
            "OldProductId": OLD_PRODUCT_ID,
            "product_id": Ext.htmlEncode(window.parent.GetProductId()),
            "product_name": Ext.htmlEncode(Ext.getCmp("product_name").getValue()),
            "price_type": PriceType,
            "product_price_list": Ext.htmlEncode(Ext.getCmp("product_price_list").getValue()),
            "bag_check_money": Ext.htmlEncode(Ext.getCmp("bag_check_money").getValue()),

            "event_price": PriceType == 1 ? event_price : minPrice,
            "price": PriceType == 1 ? price : minPrice,
            "cost": Ext.htmlEncode(Ext.getCmp('cost').getValue()),
            "event_cost": Ext.htmlEncode(Ext.getCmp('event_cost').getValue()),
            "max_price": maxPrice,
            "max_event_price": maxEventPrice,
            "event_start": event_start,
            "event_end": event_end,
            "valid_start": valid_start,
            "valid_end": valid_end,
            "price_master_id": Ext.getCmp("price_master_id").getValue(),
            "same_price": PriceType == 1 ? "1" : "0",
            "show_listprice": Ext.getCmp("show_listprice").getValue() ? 1 : 0,
            "priceStr": PriceType == 1 ? "" : priceStr
        },
        success: function (response) {
            mask.hide();
            var resText = eval("(" + response.responseText + ")");
            if (resText.success == true) {
                Ext.Msg.alert(PROMPT, resText.msg, function () {
                    mainPanel.hide();
                    mainPanel.getForm().reset();
                    editPanel.show();
                    showComboPanel.show();
                    siteProductStore.load({ params: { ProductId: product_id} });
                });
            }
            else {
                Ext.Msg.alert(PROMPT, resText.msg);
            }
            window.parent.setMoveEnable(true);
        }
    });
    return retVal;
}

function change(price_type, combination) {
    Ext.getCmp("price").setValue(0);
    Ext.getCmp("event_price").setValue(0);

    Ext.getCmp('priceValidTime').show();

    Ext.getCmp("price1").hide();
    Ext.getCmp("event_price1").hide();

    Ext.getCmp("price").show();
    Ext.getCmp("event_price").show();

    Ext.getCmp("price").setReadOnly(false);
    Ext.getCmp("event_price").setReadOnly(false);

    Ext.getCmp("cost").show();
    Ext.getCmp("cost1").hide();
}
