/*  
 * 
 * 文件名称：PriceMgr.js 
 * 摘    要：組合商品修改和新增 價格頁面
 * 
 */
var mainPanel;
var editPanel;
var itemsNew
var showComboPanel;
var OLD_PRODUCT_ID;
var combination = 0;
var PRICETYPE = 0;
var isNew = true;
var SPLIT = "`LM`";
var FULL_NAME = "`LM``LM``LM`";
var pro_sz = "";
var product_id = window.parent.GetProductId();

//價格類型model
Ext.define("gigade.PriceType", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'parameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
});

//價格類型store
var priceTypeStore = Ext.create("Ext.data.Store", {
    model: 'gigade.PriceType',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryPara?paraType=price_type',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//會員等級store
var userlevelStore = Ext.create("Ext.data.Store", {
    model: 'gigade.PriceType',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryPara?paraType=userlevel',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

Ext.define('GIGADE.SITE', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'Site_Id', type: 'string' },
        { name: 'Site_Name', type: 'string' }
    ]
});

var siteStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.SITE',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/ProductCombo/GetSite',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
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
        { name: 'bonus_percent', type: 'string' },
        { name: 'event_cost', type: 'string' },
        { name: 'event_price', type: 'string' },
        { name: 'default_bonus_percent', type: 'string' },
        { name: 'event_start', type: 'string' },
        { name: 'event_end', type: 'string' },
        { name: 'status', type: 'string' },
        { name: 'accumulated_bonus', type: 'string' },
        { name: 'bonus_percent_start', type: 'string' },
        { name: 'bonus_percent_end', type: 'string' },
        { name: 'same_price', type: 'string' },
        { name: 'valid_start', type: 'string' },
        { name: 'valid_end', type: 'string' },
        { name: 'product_name_format', type: 'string' }
    ]
});

var siteProductStore = Ext.create("Ext.data.Store", {
    model: 'GIGADE.PRODUCTSITE',
    proxy: {
        type: 'ajax',
        url: '/ProductCombo/GetPriceMaster',
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

var child_columns = [
    {
        header: PRODUCTID, dataIndex: 'Child_Id', muiltName: 'productNum', hidden: true, sortable: false, menuDisabled: true, width: 60, align: 'center',
        renderer: function (val, metaData, record, rowIndex, colIndex, store, view) {
            if (rowIndex > 0) {
                return store.getAt(rowIndex - 1).get('Child_Id') == val ? "" : val;
            }
            return val;
        }
    },
    { header: PRODUCTNAME, dataIndex: 'Product_Name', muiltName: 'Product_name', hidden: true, sortable: false, menuDisabled: true, width: 150 },
    { header: SPEC_1, dataIndex: 'spec_1', muiltName: 'spec_name1', hidden: true, sortable: false, menuDisabled: true, width: 70, align: 'center' },
    { header: SPEC_2, dataIndex: 'spec_2', muiltName: 'spec_name2', hidden: true, sortable: false, menuDisabled: true, width: 70, align: 'center' },
    {
        header: SINGLE_PRODUCT_COST, dataIndex: 'item_cost', muiltName: 'item_cost', hidden: true, sortable: false, menuDisabled: true, width: 70, align: 'center',
        editor: { xtype: 'numberfield', decimalPrecision: 0, minValue: 0 }, renderer: function (val) { return val == 0 ? "" : val }
    },
    {
        header: SINGLE_PRODUCT_PRICE, dataIndex: 'item_money', muiltName: 'price', hidden: true, sortable: false, menuDisabled: true, width: 60, align: 'center',
        editor: { xtype: 'numberfield', decimalPrecision: 0, minValue: 0 }, renderer: function (val) { return val == 0 ? "" : val }
    },
    {
        header: SINGLE_PRODUCT_EVENT_COST, dataIndex: 'event_cost', muiltName: 'event_cost', hidden: true, sortable: false, menuDisabled: true, width: 70, align: 'center',
        editor: { xtype: 'numberfield', decimalPrecision: 0, minValue: 0 }, renderer: function (val) { return val == 0 ? "" : val }
    },
    {
        header: SINGLE_PRODUCT_EVENT_PRICE, dataIndex: 'event_money', muiltName: 'event_price', hidden: true, sortable: false, menuDisabled: true, width: 60, align: 'center',
        editor: { xtype: 'numberfield', decimalPrecision: 0, minValue: 0 }, renderer: function (val) { return val == 0 ? "" : val }
    },
    {
        header: MUSTBUY, dataIndex: 'S_Must_Buy', muiltName: 's_must_buy', hidden: true, sortable: false, menuDisabled: true, width: 60, align: 'center',
        renderer: function (val, metaData, record, rowIndex, colIndex, store, view) {
            if (rowIndex > 0) {
                return store.getAt(rowIndex - 1).get('Child_Id') == record.get('Child_Id') ? "" : val;
            }
            return val;
        }
    }
];

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
    hidden: true,
    sortable: false,
    muiltName: 'product_id',
    menuDisabled: true,
    dataIndex: 'product_id'
}, {
    header: PRODUCTNAME,
    sortable: false,
    width: 300,
    hidden: true,
    muiltName: 'product_name',
    menuDisabled: true,
    dataIndex: 'product_name'
}, {
    header: ITEMPRICE,
    muiltName: 'item_price',
    sortable: false,
    hidden: true,
    menuDisabled: true,
    dataIndex: 'price'
}, {
    header: MUSTBUY,
    muiltName: 's_must_buy',
    sortable: false,
    hidden: true,
    menuDisabled: true,
    dataIndex: 's_must_buy'
}];

Ext.onReady(function () {
    OLD_PRODUCT_ID = window.parent.GetCopyProductId();
    var combination = 0;
    Ext.Ajax.request({
        url: '/ProductCombo/QueryProduct',
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
        hidden: product_id != "",
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
            hidden: window.parent.GetProductId() == "",
            listeners: {
                click: function () {
                    save('newPriceSave');
                }
            }
        }, {
            text: CANCEL,
            width: 100,
            hidden: window.parent.GetProductId() == "",
            listeners: {
                click: function () {
                    mainPanel.hide();
                    window.parent.setMoveEnable(true);
                    mainPanel.getForm().reset();
                    editPanel.show();
                    showComboPanel.show();
                    siteProductStore.load({ params: { ProductId: product_id } });
                }
            }
        }],
        items: [{
            xtype: 'displayfield',
            id: 'price_master_id',
            name: 'price_master_id',
            hidden: true
        }, {
            xtype: 'panel',
            hidden: product_id != "",
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
                hidden: true,
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
                        if (product_id != "") return;
                        if (newValue == 2) {
                            //Ext.getCmp("same_price").show();
                            Ext.getCmp("GridPanel").show();
                            SameChkChange(Ext.getCmp("sameChk").getValue());
                        }
                        else {
                            //Ext.getCmp("same_price").hide();
                            Ext.getCmp("GridPanel").hide();
                        }
                        change(newValue, Ext.getCmp('sameChk').getValue(), combination);
                    }
                }
            }]
        }, {
            xtype: 'fieldcontainer',
            muiltName: 'same_price',
            height: 27,
            hidden: true,
            id: 'same_price',
            defaultType: 'checkboxfield',
            items: [{
                boxLabel: SAMECHK,
                id: 'sameChk',
                checked: true,
                inputValue: '1',
                listeners: {
                    change: function (e, newValue, oldValue, opts) {
                        change(PRICETYPE == 0 ? Ext.getCmp("price_type").getValue() : PRICETYPE, newValue, combination);
                        SameChkChange(newValue);
                    }
                }
            }]
        }, {
            xtype: 'panel',
            hidden: product_id == "",
            border: false,
            defaults: {
                labelWidth: 120
            },
            id: 'sitePart',
            items: [{
                xtype: 'combobox',
                fieldLabel: SITENAME,
                hidden: true,
                editable: false,
                store: siteStore,
                //queryMode:'local',
                displayField: 'Site_Name',
                valueField: 'Site_Id',
                id: 'site_id',
                muiltName: 'site_name'
            }, {
                xtype: 'combobox',
                fieldLabel: USERLEVEL,
                hidden: true,
                editable: false,
                store: userlevelStore,
                queryMode: 'local',
                muiltName: 'user_level', //會員等級
                id: 'user_level',
                name: 'user_level',
                displayField: 'parameterName',
                valueField: 'parameterCode',
                value: 1
            }, {
                xtype: 'textfield',
                fieldLabel: USERMAIL,
                hidden: true,
                muiltName: 'user_email',
                id: 'user_email',
                name: 'user_email',
                width: 400
            }]
        }, {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            border: false,
            fieldLabel: PRODUCTSHOENAME + '<span style="color:red">*</span>',
            items: [{
                xtype: 'textfield',
                id: 'product_name',
                name: 'product_name',
                muiltName: 'product_name',
                allowBlank: false
            }, {
                xtype: 'textfield',
                id: 'Prod_Sz',
                name: 'Prod_Sz',
                margin: '0 0 0 5',
                width: 80,
                readOnly: true
            }]
        }, {
            xtype: 'panel',
            hidden: product_id != "",
            border: false,
            defaults: {
                labelWidth: 120
            },
            items: [{
                xtype: 'numberfield',
                decimalPrecision: 0,
                minValue: 0,
                value: 0,
                hidden: true,
                fieldLabel: PRICELIST + '<span style="color:red">*</span>', //建議售價*
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
                hidden: true,
                muiltName: 'bag_check_money'
            }]
        }, {
            xtype: 'numberfield',
            decimalPrecision: 1,
            allowDecimals: true,
            hidden: true,
            minValue: 1,
            maxValue: 2,
            value: 1,
            step: 0.1,
            muiltName: 'default_bonus_percent',
            fieldLabel: DEFAULTBONUSPERCENT + '<span style="color:red">*</span>',
            id: 'default_bonus_percent',
            name: 'default_bonus_percent',
            allowBlank: false
        }, {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            id: 'bonus_percent_time',
            muiltName: 'bonus_percent_time',
            hidden: true,
            fieldLabel: BONUS_PERCENT_TIME,
            items: [{
                xtype: 'datetimefield',
                disabledMin: true,
                disabledSec: true,
                format: 'Y-m-d H:i:s',
                name: 'bonus_percent_start',
                id: 'bonus_percent_start',
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
                name: 'bonus_percent_end',
                id: 'bonus_percent_end',
                listeners: {
                }
            }]
        }, {
            xtype: 'panel',
            border: false,
            defaults: {
                labelWidth: 120
            },
            muiltName: 'price',
            hidden: true,
            height: 27,
            items: [{
                xtype: 'numberfield',
                decimalPrecision: 0,
                minValue: 0,
                value: 0,
                fieldLabel: PRICE + '<span style="color:red">*</span>', //售價*
                id: 'price',
                name: 'price',
                allowBlank: false
            }, {
                xtype: 'displayfield',
                fieldLabel: PRICE + '<span style="color:red">*</span>',
                value: LIKE_SELECT_PRICE,//依選擇規格
                id: 'price1',
                hidden: true,
                name: 'price1'
            }]
        }, {
            xtype: 'panel',
            muiltName: 'item_cost',
            defaults: {
                labelWidth: 120
            },
            hidden: true,
            border: false,
            height: 27,
            items: [{
                xtype: 'numberfield',
                decimalPrecision: 0,
                minValue: 0,
                value: 0,
                fieldLabel: COST + '<span style="color:red">*</span>', //成本*
                id: 'cost',
                muiltName: 'cost',
                hidden: true,
                allowBlank: false
            }, {
                xtype: 'displayfield',
                fieldLabel: COST + '<span style="color:red">*</span>',
                value: LIKE_SELECT_PRICE,//依選擇規格
                id: 'cost1',
                hidden: true,
                name: 'cost1'
            }]
        }, {
            xtype: 'numberfield',
            decimalPrecision: 1,
            allowDecimals: true,
            hidden: true,
            minValue: 1,
            maxValue: 2,
            value: 1,
            step: 0.1,
            muiltName: 'bonus_percent',
            fieldLabel: BONUSPERCENT,
            id: 'bonus_percent',
            name: 'bonus_percent',
            muiltName: 'bonus_percent'
        }, {
            xtype: 'panel',
            border: false,
            defaults: {
                labelWidth: 120
            },
            hidden: true,
            muiltName: 'event_price',
            height: 27,
            items: [{
                xtype: 'numberfield',
                decimalPrecision: 0,
                minValue: 0,
                value: 0,
                fieldLabel: EVENTPRICE,
                id: 'event_price',
                name: 'event_price',
                //add by mingwei0727w 禁止用戶不輸入數據 2015/08/04
                allowBlank: false
            }, {
                xtype: 'displayfield',
                fieldLabel: EVENTPRICE,
                value: LIKE_SELECT_PRICE,//依選擇規格
                id: 'event_price1',
                hidden: true
            }]
        }, {
            xtype: 'numberfield',
            decimalPrecision: 0,
            minValue: 0,
            value: 0,
            fieldLabel: EVENT_COST,
            id: 'event_cost',
            muiltName: 'event_cost',
            hidden: true,
            allowBlank: false
        }, {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            id: 'event_time',
            muiltName: 'event_time',
            hidden: true,
            fieldLabel: EVENT_TIME,
            items: [{
                xtype: 'datetimefield',
                disabledMin: false,
                disabledSec: false,
                time: { hour: 0, min: 0, sec: 0 },// add by zhuoqin0830w  自定義時間空間的值  2015/03/16
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
                disabledMin: false,
                disabledSec: false,
                time: { hour: 23, min: 59, sec: 59 }, // add by zhuoqin0830w  自定義時間空間的值  2015/03/16
                format: 'Y-m-d H:i:s',
                name: 'event_end',
                id: 'event_end',
                listeners: {
                }
            }, { //add by wangwei0216w 2014/11/6 設置清除時間控件
                xtype: 'button',
                text: ELIMINATE,
                id: 'btn_reset5',
                margin: '0 0 0 5',
                listeners: {
                    click: function () {
                        Ext.getCmp("event_start").setValue("");
                        Ext.getCmp("event_end").setValue("");
                    }
                }
            }]
        }, {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            id: 'priceValidTime',
            muiltName: 'priceValidTime',
            hidden: true,
            fieldLabel: UP_AND_DOWN_DATE,//上下架時間
            items: [{
                xtype: 'datetimefield',
                disabledMin: false,
                disabledSec: false,
                time: { hour: 0, min: 0, sec: 0 },// add by zhuoqin0830w  自定義時間空間的值  2015/03/16
                format: 'Y-m-d H:i:s',
                name: 'valid_start',
                id: 'valid_start'
            }, {
                xtype: 'displayfield',
                value: '~'
            }, {
                xtype: 'datetimefield',
                disabledMin: false,
                disabledSec: false,
                time: { hour: 23, min: 59, sec: 59 },// add by zhuoqin0830w  自定義時間空間的值   2015/03/16
                format: 'Y-m-d H:i:s',
                name: 'valid_end',
                id: 'valid_end'
            },
            { //add by wangwei0216w 2014/11/6 設置清除時間控件
                xtype: 'button',
                text: ELIMINATE,
                id: 'btn_reset6',
                margin: '0 0 0 5',
                listeners: {
                    click: function () {
                        Ext.getCmp("valid_start").setValue("");
                        Ext.getCmp("valid_end").setValue("");
                    }
                }
            }]
        }, {
            xtype: 'checkbox',
            boxLabel: ACCUMULATED_BONUS,
            inputValue: '1',
            hidden: true,
            muiltName: 'accumulated_bonus',
            id: 'accumulated_bonus',
            name: 'accumulated_bonus',
            checked: true
        }, {
            xtype: 'panel',
            border: false,
            defaults: {
                labelWidth: 120
            },
            items: [{
                xtype: 'fieldcontainer',
                hidden: product_id,
                defaultType: 'checkboxfield',
                items: [{
                    boxLabel: FRONT_SHOW_PRICE,
                    inputValue: '2',
                    muiltName: 'show_listprice',
                    hidden: true,
                    id: 'show_listprice',
                    name: 'show_listprice'
                }]
            }]
        }, {
            xtype: 'panel',
            id: 'GridPanel',
            layout: 'anchor',
            border: false
        }],
        listeners: {
            beforerender: function () {
                //加載各自定價grid
                CreateGrid(combination);
                if (product_id == "") {
                    mainPanel.getForm().load({
                        type: 'ajax',
                        url: '/ProductCombo/QueryPriceMasterProduct',
                        actionMethods: 'post',
                        params: {
                            "ProductId": Ext.htmlEncode(window.parent.GetProductId()),
                            "OldProductId": OLD_PRODUCT_ID
                        },
                        success: function (response, opts) {
                            var resText = eval("(" + opts.response.responseText + ")");
                            if (!resText.data) return;
                            FULL_NAME = resText.data.product_name;
                            var productName = resText.data.product_name.split(SPLIT);
                            Ext.getCmp('product_name').setValue(productName[1]);
                            Ext.getCmp("sameChk").setValue(resText.data.same_price == 1 ? true : false);
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
                            var bonus_percent_start = resText.data.bonus_percent_start;
                            var bonus_percent_end = resText.data.bonus_percent_end;
                            if (bonus_percent_start != "0") {
                                Ext.getCmp("bonus_percent_start").setValue(new Date(bonus_percent_start * 1000));
                            }
                            else {
                                Ext.getCmp("bonus_percent_start").setValue();
                            }
                            if (bonus_percent_end != "0") {
                                Ext.getCmp("bonus_percent_end").setValue(new Date(bonus_percent_end * 1000));
                            }
                            else {
                                Ext.getCmp("bonus_percent_end").setValue();
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
    siteProductStore.load({ params: { ProductId: product_id } });
    editPanel = Ext.create("Ext.form.Panel", {
        id: 'editPanel',
        border: false,
        width: 950,
        height: 345,
        hidden: product_id == "",
        autoScroll: true,
        defaults: {
            labelWidth: 110,
            padding: '2 0 0 0'
        },
        items: [{
            xtype: 'checkbox',
            boxLabel: FRONT_SHOW_PRICE,
            inputValue: '1',
            hidden: true,
            muiltName: 'show_pricelist2',
            id: 'show_pricelist2',
            name: 'show_pricelist2',
            checked: true,
            margin: '0 0 0 115'
        }, {
            xtype: 'numberfield',
            fieldLabel: PRICELIST,
            hidden: true,
            id: 'Product_Price_List',
            muiltName: 'price_list',
            name: 'Product_Price_List'
        }, {
            xtype: 'numberfield',
            decimalPrecision: 0,
            minValue: 0,
            value: 0,
            hidden: true,
            fieldLabel: BAG_CHECK_MONEY,
            id: 'bag_check_money1',
            name: 'bag_check_money1',
            muiltName: 'bag_check_money1'
        }, {
            xtype: 'button',
            width: 80,
            id: 'comboSavePrice',
            text: SAVE,
            listeners: {
                click: function () {
                    //添加 遮罩層  避免用戶多次點擊  edit by zhuoqin0830w  2015/09/24
                    var mask;
                    if (!mask) {
                        mask = new Ext.LoadMask(Ext.getBody(), { msg: '請稍等...' });
                    }
                    mask.show();
                    Ext.Ajax.request({
                        url: '/Product/UpdatePrice',
                        method: 'post',
                        async: window.parent.GetProductId() == '' ? false : true,
                        params: {
                            "ProductId": product_id,
                            "function": 'comboSavePrice',
                            "batch": window.parent.GetBatchNo(),
                            "product_price_list": Ext.getCmp("Product_Price_List").getValue(),
                            "bag_check_money": Ext.getCmp("bag_check_money1").getValue(),
                            "show_listprice": Ext.getCmp("show_pricelist2").getValue() ? "on" : "off"
                        },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            mask.hide();
                            if (result.success) { Ext.Msg.alert(PROMPT, SUCCESS); }
                        },
                        failure: function (form, action) {
                            mask.hide();
                            Ext.Msg.alert(PROMPT, FAILURE);
                        }
                    });
                }
            },
            margin: '0 0 0 115'
        }, {
            xtype: 'displayfield',
            width: 400,
            hidden: true,
            fieldLabel: PRICE_TYPE,
            muiltName: 'price_type',
            id: 'price_type_name',
            name: 'price_type_name'
        }, {
            xtype: 'gridpanel',
            id: 'siteProdutctGrid',
            store: siteProductStore,
            autoScroll: true,
            height: 200,
            selModel: sm,
            tbar: [{
                hidden: true,
                muiltName: 'new_site_price',
                text: NEW_SITE_PRICE,
                iconCls: 'ui-icon ui-icon-money-add',
                handler: function () {
                    editPanel.hide();
                    FULL_NAME = SPLIT + SPLIT + SPLIT;
                    isNew = true;
                    showComboPanel.hide();
                    mainPanel.show();
                    loadComboInfo();
                    change(PRICETYPE, true, combination);
                    SameChkChange(true); //確定顯示新增站臺價格grid
                    //window.parent.setMoveEnable(false);
                }
            }, {
                hidden: true,
                muiltName: 'update_site_price',
                text: UPDATE_SITE_PRICE,
                iconCls: 'ui-icon ui-icon-paper-write',
                id: 'update_site_price',
                disabled: true,
                handler: function () {
                    //xxl
                    editPanel.hide();
                    showComboPanel.hide();
                    mainPanel.show();
                    isNew = false;

                    var row = Ext.getCmp('siteProdutctGrid').getSelectionModel().getSelection();
                    if (siteStore.getCount() == 0)
                        siteStore.load();
                    mainPanel.getForm().loadRecord(row[0]);
                    FULL_NAME = row[0].data.product_name_format;
                    var productName = row[0].data.product_name_format.split(SPLIT);
                    Ext.getCmp('product_name').setValue(productName[1]);
                    Ext.getCmp("Prod_Sz").setValue(pro_sz);
                    var user_id = Number(row[0].data.user_id);
                    var user_level = Number(row[0].data.user_level);
                    var site_id = Number(row[0].data.site_id);
                    var is_same = row[0].data.same_price == 1 ? true : false;
                    Ext.getCmp("sameChk").setValue(is_same);
                    if (PRICETYPE == 2)
                        GetUpdateData(user_id, user_level, site_id, is_same); //請求修改數據
                    change(PRICETYPE, is_same, combination); //根據價格類型、同價來確定頁面展現
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
                    var bonus_percent_start = row[0].data.bonus_percent_start;
                    var bonus_percent_end = row[0].data.bonus_percent_end;
                    if (bonus_percent_start != "0") {
                        Ext.getCmp("bonus_percent_start").setValue(new Date(bonus_percent_start * 1000));
                    }
                    else {
                        Ext.getCmp("bonus_percent_start").setValue();
                    }
                    if (bonus_percent_end != "0") {
                        Ext.getCmp("bonus_percent_end").setValue(new Date(bonus_percent_end * 1000));
                    }
                    else {
                        Ext.getCmp("bonus_percent_end").setValue();
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
                    //window.parent.setMoveEnable(false);
                    /****/
                }
            }, {
                text: PRICE_UP,
                id: 'price_up',
                iconCls: 'ui-icon ui-icon-up',
                hidden: true,
                muiltName: 'price_up',
                /*disabled: true,*/
                handler: function () { updatePriceStatus("up") }
            }, {
                text: PRICE_DOWN,
                id: 'price_down',
                iconCls: 'ui-icon ui-icon-down',
                muiltName: 'price_down',
                hidden: true,
                /*disabled: true,*/
                handler: function () { updatePriceStatus("down") }
            }],
            columns: [
                { hidden: false, header: MAJOR_CODE, dataIndex: 'price_master_id', width: 100, align: 'center', sortable: false, menuDisabled: true }, //edit by zhuoqin0830w 2015/01/27 增加主檔編碼
                { hidden: true, muiltName: 'site_name', header: SITENAME, dataIndex: 'site_name', width: 100, align: 'center', sortable: false, menuDisabled: true },
                { hidden: true, muiltName: 'product_name', header: PRODUCTNAME, dataIndex: 'product_name', width: 180, align: 'center', sortable: false, menuDisabled: true },
                { hidden: true, muiltName: 'user_level', header: USERLEVEL, dataIndex: 'user_level_name', width: 100, align: 'center', sortable: false, menuDisabled: true },
                { hidden: true, header: USERLEVEL, dataIndex: 'user_level', width: 100, align: 'center', sortable: false, menuDisabled: true },
                { hidden: true, dataIndex: 'user_id', width: 100, align: 'center', sortable: false, menuDisabled: true },
                { hidden: true, muiltName: 'user_email', header: USERMAIL, dataIndex: 'user_email', width: 120, align: 'center', sortable: false, menuDisabled: true },
                { muiltName: 'price_status', header: PRICESTATUS, dataIndex: 'status', width: 100, align: 'center', sortable: false, menuDisabled: true, hidden: true },             //edit by hjj 2014/08/13 將固定組合商品與單一商品售價與成本的順序一致化
                { hidden: true, muiltName: 'price', header: PRICE, dataIndex: 'price', width: 100, align: 'center', sortable: false, menuDisabled: true },
                { hidden: true, muiltName: 'item_cost', header: COST, dataIndex: 'cost', width: 100, align: 'center', sortable: false, menuDisabled: true },
                { hidden: true, muiltName: 'default_bonus_percent', header: DEFAULTBONUSPERCENT, dataIndex: 'default_bonus_percent', width: 120, align: 'center', sortable: false, menuDisabled: true },
                { hidden: true, muiltName: 'event_price', header: EVENTPRICE, dataIndex: 'event_price', width: 100, align: 'center', sortable: false, menuDisabled: true },
                { hidden: true, muiltName: 'event_cost', header: EVENT_COST, dataIndex: 'event_cost', width: 100, align: 'center', sortable: false, menuDisabled: true },
                {
                    hidden: true, muiltName: 'event_time', header: EVENT_TIME, xtype: 'templatecolumn',
                    tpl: Ext.create('Ext.XTemplate',
                        '{[values.event_start==0?"":Ext.Date.format(new Date(values.event_start * 1000),"Y/m/d H:i:s")]}',
                        '~',
                        '{[values.event_end==0?"":Ext.Date.format(new Date(values.event_end * 1000),"Y/m/d H:i:s")]}'
                        ), width: 240, align: 'left', sortable: false, menuDisabled: true
                },
                { hidden: true, muiltName: 'bonus_percent', header: BONUSPERCENT, dataIndex: 'bonus_percent', width: 100, align: 'center', sortable: false, menuDisabled: true },
                {
                    muiltName: 'bonus_percent_time', header: BONUS_PERCENT_TIME, xtype: 'templatecolumn',
                    tpl: Ext.create('Ext.XTemplate',
                        '{[values.bonus_percent_start==0?"":Ext.Date.format(new Date(values.bonus_percent_start * 1000),"Y/m/d H:i:s")]}',
                        '~',
                        '{[values.bonus_percent_end==0?"":Ext.Date.format(new Date(values.bonus_percent_end * 1000),"Y/m/d H:i:s")]}'
                        ),
                    width: 220, align: 'left', sortable: false, menuDisabled: true, hidden: true
                }]
        }],
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
        hidden: product_id == "",
        defaults: {
            labelWidth: 110,
            padding: '2 0 0 0'
        }
    })
    loadComboInfo();

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
    if (product_id != "") {
        window.parent.updateAuth(editPanel, 'muiltName');
    }
});

function setBagCheckMoney(field, mode) {
    if (mode != 2) {
        field.setReadOnly(true);
        field.setValue(0);
    }
    else {
        Ext.Ajax.request({
            url: '/ProductCombo/IsSameChildVendor',
            params: {
                ProductId: window.parent.GetProductId(),
                OldProductId: OLD_PRODUCT_ID
            },
            method: 'post',
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    field.setReadOnly(!result.same);
                    if (!result.same) {
                        field.setValue(0);
                    }
                }
            },
            failure: function () {
                field.setReadOnly(false);
            }
        });
    }
}

/// add by wwei0216w 2015/8/18
/// 將原先的save語句改成現行模式,用來增加對價格提示框的判斷
function save(functionid) {
    if (OLD_PRODUCT_ID != '' || product_id != "") {///如果OLD_PRODUCT_ID不為空,則證明是複製
        if (confirm(AREYOUSURE)) {//此時添加消息框彈出
            return saveIds(functionid);
        }
    } else {
        return saveIds(functionid);///否則直接調用之前的save方法既現在的saveIds方法
    }
}

//保存至臨時表
function saveIds(functionid) {
    var retVal = true;
    //判斷站臺名稱是否填寫 避免用戶直接點擊
    if (window.parent.GetProductId()) {
        if (Ext.getCmp("site_id").getValue() == null) {
            Ext.Msg.alert(PROMPT, WRITE_SITEID);
            return false;
        }
    }

    if (!Ext.getCmp("mainpanel").getForm().isValid()) {
        return false;
    }
    var start = Ext.getCmp("event_start").rawValue;
    var end = Ext.getCmp("event_end").rawValue;
    if (start != "" || end == "") {
        if (Ext.getCmp("bonus_percent").getValue() == null) {
            Ext.Msg.alert(PROMPT, PLEASE_SELECT_ACTIVITY_PRICE_RETURN_PERCENT);//請選擇活動購物金回饋百分比
            return false;
        }
    }

    //edit 2014/09/23
    if (!window.parent.GetProductId()) {
        var pro_price = Ext.getCmp("product_price_list").getValue();
        if (pro_price === 0) {
            Ext.Msg.alert(PROMPT, PRODUCT_PRICE_LIST_EMPTY);
            return false;
        }
    }

    ///edit by wwei0216w 價格為0時不允許保存 2015/12/28
    if (Ext.getCmp('price').getValue() == 0) {
        alert(ITEM_MONEY_EMPTY);
        return;
    }
    if (Ext.getCmp('cost').getValue() == 0) {
        alert(ITEM_COST_EMPTY);
        return;
    }

    if (product_id == "") {
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
    var level;
    if (window.parent.GetProductId() == "")
    { level = ""; } else {
        if (Ext.getCmp("user_level").rawValue == "") {
            level = "";
        }
        else {
            level = userlevelStore.data.items[userlevelStore.find("parameterName", Ext.getCmp("user_level").rawValue)].data.parameterCode
        }
    };
    var event_start = Ext.getCmp("event_start").rawValue == 0 ? "0" : Ext.getCmp("event_start").rawValue;
    var event_end = Ext.getCmp("event_end").rawValue == 0 ? "0" : Ext.getCmp("event_end").rawValue;
    var bonus_percent_start = Ext.getCmp("bonus_percent_start").rawValue == 0 ? "0" : Ext.getCmp("bonus_percent_start").rawValue;
    var bonus_percent_end = Ext.getCmp("bonus_percent_end").rawValue == 0 ? "0" : Ext.getCmp("bonus_percent_end").rawValue;
    var valid_start = Ext.getCmp('valid_start').rawValue == 0 ? "0" : Ext.getCmp('valid_start').rawValue;
    var valid_end = Ext.getCmp('valid_end').rawValue == 0 ? "0" : Ext.getCmp('valid_end').rawValue;
    if (!functionid) {
        functionid = '';
    }
    var PriceType = PRICETYPE == 0 ? Ext.htmlEncode(Ext.getCmp("price_type").getValue()) : PRICETYPE;
    var SameCHK = Ext.getCmp("sameChk").getValue();
    var price = Ext.htmlEncode(Ext.getCmp("price").getValue());
    var event_price = Ext.htmlEncode(Ext.getCmp("event_price").getValue());
    var priceStr;
    var childId = "";
    var MustBuy = "";
    minPrice = 0, maxPrice = 0, maxEventPrice = 0;
    if (PriceType != 2) {///各自定價不進行該判斷
        if ((Ext.getCmp('cost').getValue() > Ext.getCmp('price').getValue()) || (Ext.getCmp('event_cost').getValue() > Ext.getCmp('event_price').getValue())) {
            mask.hide();
            Ext.Msg.alert('消息', '售價不能小於成本');
            return;
        }
    }
    //獲取各自定價grid數據
    if (PriceType == 2) {
        priceStr = submitData(Ext.getCmp('GridPanel'));
        if (priceStr == undefined) {
            mask.hide();
            Ext.Msg.alert("消息","售價不能小於成本")
        }
        if (!SameCHK) {
            sunMinMaxPrice(Ext.getCmp('GridPanel'));
        }
        else {
            totalPrice(Ext.getCmp('GridPanel')); //edit by xinglu0624w reason:规格同价且商品类型为固定组合时也需要给max_price值
        }
    }
    var product_name = Ext.htmlEncode(Ext.getCmp("product_name").getValue());
    var tmpName = FULL_NAME.split(SPLIT);
    tmpName[1] = product_name;
    tmpName[2] = Ext.getCmp("Prod_Sz").getValue();
    product_name = "";
    for (var a = 0; a < tmpName.length; a++) {
        product_name += tmpName[a] + SPLIT;
    }
    product_name = product_name.substring(0, product_name.lastIndexOf(SPLIT));
    Ext.Ajax.request({
        url: '/ProductCombo/ComboPriceSave',
        method: 'POST',
        async: window.parent.GetProductId() == '' ? false : true,
        params: {
            "OldProductId": OLD_PRODUCT_ID,
            "product_id": Ext.htmlEncode(window.parent.GetProductId()),
            "product_name": product_name,
            "price_type": PriceType,
            "product_price_list": Ext.htmlEncode(Ext.getCmp("product_price_list").getValue()),
            "bag_check_money": Ext.htmlEncode(Ext.getCmp("bag_check_money").getValue()),
            "accumulated_bonus": Ext.getCmp("accumulated_bonus").getValue() ? 1 : 0,
            "default_bonus_percent": Ext.htmlEncode(Ext.getCmp("default_bonus_percent").getValue()),
            "bonus_percent": Ext.htmlEncode(Ext.getCmp("bonus_percent").getValue()),
            "event_price": PriceType == 1 ? event_price : SameCHK ? event_price : Ext.htmlEncode(Ext.getCmp('event_price').getValue()), //edit by hufeng0813w 2014/06/16
            "price": PriceType == 1 ? price : SameCHK ? price : minPrice,
            "cost": Ext.htmlEncode(Ext.getCmp('cost').getValue()),
            "event_cost": Ext.htmlEncode(Ext.getCmp('event_cost').getValue()),
            "max_price": maxPrice,
            "max_event_price": maxEventPrice,
            "event_start": event_start,
            "event_end": event_end,
            "bonus_percent_start": bonus_percent_start,
            "bonus_percent_end": bonus_percent_end,
            "valid_start": valid_start,
            "valid_end": valid_end,
            "site_id": Ext.getCmp("site_id").getValue(),
            "user_level": level,
            "user_mail": Ext.getCmp("user_email").getValue(),
            "price_master_id": Ext.getCmp("price_master_id").getValue(),
            "same_price": PriceType == 1 ? "1" : SameCHK ? "1" : "0",
            "show_listprice": Ext.getCmp("show_listprice").getValue() ? 1 : 0,
            "priceStr": PriceType == 1 ? "" : priceStr,
            "function": functionid,
            "batch": window.parent.GetBatchNo()
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
                    siteProductStore.load({ params: { ProductId: product_id } });
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

//得到grid數據
function submitData(panel) {
    var data = '[';
    var grids = panel.query('grid');
    if (panel) {
        for (var i = 0; i < grids.length; i++) {
            var store = grids[i].getStore();
            for (var j = 0; j < store.getCount() ; j++) {
                var record = store.getAt(j);
                if (record.get('item_money') < record.get("item_cost")){
                    return;
                }
                data += "{Parent_Id:" + record.get('Parent_Id') + ",Child_Id:" + record.get('Child_Id') + ",Product_Name:\"" + record.get('Product_Name') + "\""
                + ",item_id:" + record.get('item_id') + ",price_master_id:" + record.get('price_master_id') + ",item_price_id:" + record.get('item_price_id')
                + ",item_money:" + record.get('item_money') + ",item_cost:" + record.get('item_cost') + ",event_money:" + record.get('event_money')
                + ",event_cost:" + record.get('event_cost') + ",Pile_Id:" + record.get('Pile_Id') + ",S_Must_Buy:" + record.get('S_Must_Buy')
                + ",G_Must_Buy:" + record.get('G_Must_Buy') + "}";
            }
        }
    }
    data += "]";
    data = data.replace(/}{/g, '},{');
    return data;
}

//更新價格狀態(上架或下架)
function updatePriceStatus(updateStatus) {
    var row = Ext.getCmp('siteProdutctGrid').getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        var values = Ext.Object.fromQueryString("function=" + "newPriceSave" + "&batch=" + window.parent.GetBatchNo());
        values.priceMasterId = row[0].get('price_master_id');
        values.updateStatus = updateStatus;

        Ext.Ajax.request({
            url: '/ProductCombo/UpdatePriceStatus',
            method: 'POST',
            params: values,
            async: false,
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    Ext.Msg.alert(INFORMATION, SUCCESS);
                    siteProductStore.load({ params: { ProductId: product_id } });
                } else {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                }
            },
            failure: function () {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        });
    }
}

function loadComboInfo() {
    /***********************加載組合商品信息***************************/
    //查詢組合類型
    Ext.Ajax.request({
        url: '/ProductCombo/QueryProduct',
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
            Ext.getCmp("Prod_Sz").setValue(resText.data.Prod_Sz);
            //Ext.getCmp("product_name").setValue(resText.data.Product_Name);
            Ext.getCmp("product_name").setValue(resText.data.Prod_Name);
            pro_sz = resText.data.Prod_Sz;
            if (product_id == "") return;
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
            if (resText.data.Combination == 2 || resText.data.Combination == 3) {
                var combinationStore = Ext.create("Ext.data.Store", {
                    fields: ['product_id', 'product_name', 'price', 's_must_buy', 'g_must_buy'],
                    proxy: {
                        type: 'ajax',
                        url: '/ProductCombo/QuerySingleProPrice',
                        actionMethods: 'post',
                        reader: {
                            type: 'json',
                            root: 'data'
                        }
                    }
                });
                //判斷組合商品中的價格類型是否是各自定價 Price_type: resText.data.Price_type 如果是  則 顯示 各自定價中的 價格  如果不是 則顯示 單一商品原本價格  edit by zhuoqin0830w  2015/07/09
                combinationStore.load({ params: { product_id: product_id, pile_id: 0, Price_type: resText.data.Price_type } });
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
                                    this.setTitle('<span style="color:red">' + MUSTCHOOSE + ':' + rec.data.g_must_buy + "</span>");
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
                    url: '/ProductCombo/groupNameQuery',
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
                                    url: '/ProductCombo/QuerySingleProPrice',
                                    actionMethods: 'post',
                                    reader: {
                                        type: 'json',
                                        root: 'data'
                                    }
                                }
                            });
                            //判斷組合商品中的價格類型是否是各自定價 Price_type: resText.data.Price_type  如果是  則 顯示 各自定價中的 價格  如果不是 則顯示 單一商品原本價格  edit by zhuoqin0830w  2015/07/09
                            storeName.load({ params: { product_id: product_id, pile_id: resText[i - 1].Pile_Id, Price_type: resText.data.Price_type } });
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
                                                me.setTitle(me.title + '<span style="color:red;margin-left:40px">' + MUSTCHOOSE + ':' + rec.data.g_must_buy + "</span>");
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
}