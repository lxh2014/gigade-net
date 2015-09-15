var NEW_SITE = false;
var PRODUCT_ID = '', OLD_PRODUCT_ID = '';
var FULL_NAME = "`LM``LM``LM`";
var NAME_FORMAT = "";
var SPLIT = "`LM`";
var prod_sz = "";
Ext.define('GIGADE.SITE', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'Site_Id', type: 'string' },
        { name: 'Site_Name', type: 'string' }
    ]
});

var siteStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.SITE',
    proxy: {
        type: 'ajax',
        url: '/Product/GetSite',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
});

var userLevelStore = Ext.create('Ext.data.Store', {
    fields: ['parameterCode', 'parameterName'],
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryPara?paraType=USERLEVEL',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

var userLevelStore2 = Ext.create('Ext.data.Store', {
    fields: ['parameterCode', 'parameterName'],
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryPara?paraType=USERLEVEL',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});



Ext.define('GIGADE.PRODUCTITEM', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'Item_Id', type: 'string' },
        { name: 'Spec_Name_1', type: 'string' },
        { name: 'Spec_Name_2', type: 'string' },
        { name: 'Product_Id', type: 'string' },
        { name: 'Item_Money', type: 'string' },
        { name: 'Item_Cost', type: 'string' },
        { name: 'Event_Money', type: 'string' },
        { name: 'Event_Cost', type: 'string' },
        { name: 'Event_Item_Money', type: 'string' },
        { name: 'Event_Item_Cost', type: 'string' },
        { name: 'Event_Product_Start', type: 'string' },
        { name: 'Event_Product_End', type: 'string' },
        { name: 'Item_Code', type: 'string' }
    ]
});

var itemStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.PRODUCTITEM',
    proxy: {
        type: 'ajax',
        url: '/Product/GetProItems',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    },
    listeners: {
        load: function (store) {
            if (store.getCount() == 1) {
                Ext.getCmp('frm').down('#same_price').setReadOnly(true);
            }
        }
    }
});
var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
    clicksToEdit: 1,
    listeners: {
        beforeedit: function (e, eOpts) {
            if (Ext.getCmp('same_price').getValue()) {
                if (e.rowIdx != 0 && e.colIdx != 3) {
                    return false;
                }
            }
        }
    }
});
var cellEditing_wfrm = Ext.create('Ext.grid.plugin.CellEditing', {
    clicksToEdit: 1,
    listeners: {
        beforeedit: function (e, eOpts) {
            if (!Ext.getCmp('wfrm')) {
                return;
            }
            if (Ext.getCmp('wfrm').query('*[colName=same_price]')[0].getValue()) {
                if (e.rowIdx != 0) {
                    return false;
                }
            }
        }
    }
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

Ext.define('GIGADE.PRODUCTSITE', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'price_master_id', type: 'string' },
        { name: 'product_id', type: 'string' },
        { name: 'site_id', type: 'string' },
        { name: 'site_name', type: 'string' },
        { name: 'product_name', type: 'string' },
        { name: 'accumulated_bonus', type: 'string' },
        { name: 'bonus_percent', type: 'string' },
        { name: 'default_bonus_percent', type: 'string' },
        { name: 'bonus_percent_start', type: 'string' },
        { name: 'bonus_percent_end', type: 'string' },
        { name: 'user_level', type: 'string' },
        { name: 'user_level_name', type: 'string' },
        { name: 'user_email', type: 'string' },
        { name: 'user_id', type: 'string' },
        { name: 'same_price', type: 'string' },
        { name: 'event_start', type: 'string' },
        { name: 'event_end', type: 'string' },
        { name: 'item_cost', type: 'string' },
        { name: 'item_money', type: 'string' },
        { name: 'event_cost', type: 'string' },
        { name: 'event_money', type: 'string' },
        { name: 'price_status', type: 'string' },
        { name: 'status', type: 'string' },
        { name: 'valid_start', type: 'string' },
        { name: 'valid_end', type: 'string' },
        { name: 'product_name_format', type: 'string' }
    ]
});
var siteProductStore = Ext.create("Ext.data.Store", {
    model: 'GIGADE.PRODUCTSITE',
    proxy: {
        type: 'ajax',
        url: '/Product/GetPriceMaster',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
});
Ext.define('GIGADE.ITEMPRICE', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'item_price_id', type: 'string' },
        { name: 'item_id', type: 'string' },
        { name: 'price_master_id', type: 'string' },
        { name: 'item_money', type: 'string' },
        { name: 'item_cost', type: 'string' },
        { name: 'event_money', type: 'string' },
        { name: 'event_cost', type: 'string' },
        { name: 'spec_name_1', type: 'string' },
        { name: 'spec_name_2', type: 'string' }
    ]
});
var sitePriceStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.ITEMPRICE',
    proxy: {
        type: 'ajax',
        url: '/Product/GetItemPrice',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
})


Ext.onReady(function () {
    PRODUCT_ID = window.parent.GetProductId();
    OLD_PRODUCT_ID = window.parent.GetCopyProductId();
    var frmItems = new Array();

    var siteName = [{
        xtype: 'fieldcontainer',
        layout: 'hbox',
        colName: 'product_name',
        hidden: true,
        border: false,
        width: 600,
        items: [{
            xtype: 'textfield',
            labelWidth: 120,
            width: 400,
            fieldLabel: PRODUCT_NAME + NOT_EMPTY,
            submitValue: false,
            allowBlank: false,
            id: 'product_name',
            name: 'product_name'
        }, {
            xtype: 'textfield',
            id: 'Prod_Sz',
            name: 'Prod_Sz',
            margin: '0 0 0 5',
            width: 80,
            readOnly: true
        }]
    }];
    var siteItem = [{
        id: 'siteInfo',
        border: false,
        defaults: { labelWidth: 120 },
        defaultType: 'textfield',
        items: [{
            xtype: 'combobox',
            store: siteStore,
            displayField: 'Site_Name',
            valueField: 'Site_Id',
            //queryMode: 'local',
            fieldLabel: SITE_NAME + NOT_EMPTY,
            editable: false,
            allowBlank: false,
            hidden: true,
            id: 'site_name',
            colName: 'site_name',
            name: 'site_name'
        }, {
            xtype: 'combobox',
            store: userLevelStore,
            queryMode: 'local',
            displayField: 'parameterName',
            valueField: 'parameterCode',
            editable: false,
            allowBlank: false,
            fieldLabel: USER_LEVEL + NOT_EMPTY,
            hidden: true,
            id: 'user_level',
            colName: 'user_level', //會員等級
            name: 'user_level',
            value: 1
        }, {
            fieldLabel: USER_EMAIL,
            hidden: true,
            id: 'user_id',
            vtype: 'email',
            colName: 'user_id',
            name: 'user_id'
        }]
    }];
    var defaultItem = [{
        border: false,
        defaults: { labelWidth: 120 },
        items: [{
            xtype: 'checkbox',
            hidden: true,
            id: 'accumulated_bonus',
            name: 'accumulated_bonus',
            colName: 'accumulated_bonus',
            padding: '0 125px',
            checked: true,
            boxLabel: ACCUMULATED_BONUS
        }, {
            xtype: 'numberfield',
            decimalPrecision: 1,
            hidden: true,
            id: 'default_bonus_percent',
            name: 'default_bonus_percent',
            colName: 'default_bonus_percent',
            fieldLabel: DEFAULT_BONUS_PERCENT,
            step: 0.1,
            minValue: 1,
            maxValue: 2,
            value: 1
        }, {
            xtype: 'numberfield',
            decimalPrecision: 1,
            hidden: true,
            name: 'bonus_percent',
            colName: 'bonus_percent',
            id: 'bonus_percent',
            fieldLabel: BONUS_PERCENT,
            step: 0.1,
            minValue: 1,
            maxValue: 2,
            value: 1
        }, {
            xtype: 'fieldcontainer',
            hidden: true,
            colName: 'bonus_percent_start',
            layout: 'hbox',
            width: 900,
            fieldLabel: BONUS_PERCENT_TIME,
            items: [{
                xtype: 'datetimefield',
                disabledMin: true,
                disabledSec: true,
                format: 'Y-m-d H:i:s',
                editable: false,
                name: 'bonus_percent_start',
                id: 'bonus_percent_start',
                listeners: {
                    select: function () {
                        var end = new Date(Ext.getCmp("bonus_percent_end").rawValue);
                        var start = new Date(this.rawValue);
                        if (end < start) {
                            Ext.Msg.alert(INFORMATION, TIME_ERROR);
                            this.setValue('');
                        }
                    }
                }
            }, {
                xtype: 'displayfield',
                value: '~'
            }, {
                xtype: 'datetimefield',
                disabledMin: true,
                disabledSec: true,
                format: 'Y-m-d H:i:s',
                editable: false,
                name: 'bonus_percent_end',
                id: 'bonus_percent_end',
                listeners: {
                    select: function () {
                        var start = new Date(Ext.getCmp("bonus_percent_start").rawValue);
                        var end = new Date(this.rawValue);
                        if (end < start) {
                            Ext.Msg.alert(INFORMATION, TIME_ERROR);
                            this.setValue('');
                        }
                    }
                }
            },
            { // add by Jiajun 2014/09/17
                xtype: 'button',
                text: ELIMINATE,
                id: 'btn_reset5',
                margin: '0 0 0 5',
                listeners: {
                    click: function () {
                        Ext.getCmp("bonus_percent_start").setValue("");
                        Ext.getCmp("bonus_percent_end").setValue("");
                    }
                }
            }
            ]
        }]
    }];
    var defaultPrice = [{
        border: false,
        defaults: { labelWidth: 120 },
        items: [{
            xtype: 'checkbox',
            boxLabel: FRONT_SHOW_PRICE,
            name: 'show_listprice',
            colName: 'show_listprice',
            id: 'show_listprice',
            hidden: true,
            padding: '0 125px'
        }, {
            xtype: 'numberfield',
            decimalPrecision: 0,
            hidden: true,
            fieldLabel: PRODUCT_PRICE_LIST + NOT_EMPTY, //建議售價
            minValue: 0, //2014/09/17
            allowBlank: false,
            name: 'product_price_list',
            colName: 'product_price_list',
            id: 'product_price_list'
        }, {
            xtype: 'numberfield',
            decimalPrecision: 0,
            hidden: true,
            fieldLabel: BAG_CHECK_MONEY + NOT_EMPTY,
            minValue: 0, //2014/09/17
            allowBlank: false,
            name: 'bag_check_money',
            colName: 'bag_check_money',
            id: 'bag_check_money'
        }]
    }]
    var defaultEvent = [{
        border: false,
        defaults: { labelWidth: 120 },
        items: [{
            xtype: 'fieldcontainer',
            hidden: true,
            colName: 'event_product_start',
            layout: 'hbox',
            width: 900,
            fieldLabel: EVENT_TIME,
            items: [{
                xtype: 'datetimefield',
                disabledMin: false,
                disabledSec: false,
                time: { hour: 0, min: 0, sec: 0 },// add by zhuoqin0830w  自定義時間空間的值  2015/03/16
                format: 'Y-m-d H:i:s',
                editable: false,
                name: 'event_product_start',
                id: 'event_product_start',
                listeners: {
                    select: function () {
                        var end = new Date(Ext.getCmp("event_product_end").rawValue);
                        var start = new Date(this.rawValue);
                        if (end < start) {
                            Ext.Msg.alert(INFORMATION, TIME_ERROR);
                            this.setValue('');
                        }
                    }
                }
            }, {
                xtype: 'displayfield',
                value: '~'
            }, {
                xtype: 'datetimefield',
                disabledMin: false,
                disabledSec: false,
                time: { hour: 23, min: 59, sec: 59 },// add by zhuoqin0830w  自定義時間空間的值  2015/03/16
                format: 'Y-m-d H:i:s',
                editable: false,
                name: 'event_product_end',
                id: 'event_product_end',
                listeners: {
                    select: function () {
                        var start = new Date(Ext.getCmp("event_product_start").rawValue);
                        var end = new Date(this.rawValue);
                        if (end < start) {
                            Ext.Msg.alert(INFORMATION, TIME_ERROR);
                            this.setValue('');
                        }
                    }
                }
            }, { // add by Jiajun 2014/09/17
                xtype: 'button',
                text: ELIMINATE,
                id: 'btn_reset6',
                margin: '0 0 0 5',
                listeners: {
                    click: function () {
                        Ext.getCmp("event_product_start").setValue("");
                        Ext.getCmp("event_product_end").setValue("");
                    }
                }
            }]
        }]
    }];
    var siteEvent = [{
        border: false,
        defaults: { labelWidth: 120 },
        width: 900,
        items: [{
            xtype: 'fieldcontainer',
            hidden: true,
            colName: 'event_start',
            layout: 'hbox',
            width: 900,
            fieldLabel: EVENT_TIME,
            items: [{
                xtype: 'datetimefield',
                disabledMin: false,
                disabledSec: false,
                time: { hour: 0, min: 0, sec: 0 },// add by zhuoqin0830w  自定義時間空間的值  2015/03/16
                format: 'Y-m-d H:i:s',
                editable: false,
                name: 'event_start',
                id: 'event_start',
                listeners: {
                    select: function () {
                        var end = new Date(Ext.getCmp("event_end").rawValue);
                        var start = new Date(this.rawValue);
                        if (end < start) {
                            Ext.Msg.alert(INFORMATION, TIME_ERROR);
                            this.setValue('');
                        }
                    }
                }
            }, {
                xtype: 'displayfield',
                value: '~'
            }, {
                xtype: 'datetimefield',
                disabledMin: false,
                disabledSec: false,
                time: { hour: 23, min: 59, sec: 59 },// add by zhuoqin0830w  自定義時間空間的值  2015/03/16
                format: 'Y-m-d H:i:s',
                editable: false,
                name: 'event_end',
                id: 'event_end',
                listeners: {
                    select: function () {
                        var start = new Date(Ext.getCmp("event_start").rawValue);
                        var end = new Date(this.rawValue);
                        if (end < start) {
                            Ext.Msg.alert(INFORMATION, TIME_ERROR);
                            this.setValue('');
                        }
                    }
                }
            },

            { // add by Jiajun 2014/09/29
                xtype: 'button',
                text: ELIMINATE,
                id: 'btn_reset7',
                margin: '0 0 0 5',
                listeners: {
                    click: function () {
                        Ext.getCmp("event_start").setValue("");
                        Ext.getCmp("event_end").setValue("");
                    }
                }
            }]
        }]
    }];

    var priceValidTime = [{
        border: false,
        defaults: { labelWidth: 120 },
        width: 900,
        items: [{
            xtype: 'fieldcontainer',
            colName: 'priceValidTime',
            layout: 'hbox',
            width: 900,
            fieldLabel: PRODUCT_UP_DOWN_DATE,
            items: [{
                xtype: 'datetimefield',
                disabledMin: false,
                disabledSec: false,
                time: { hour: 0, min: 0, sec: 0 },// add by zhuoqin0830w  自定義時間空間的值  2015/03/16
                format: 'Y-m-d H:i:s',
                editable: false,
                name: 'valid_start',
                id: 'valid_start'
            }, {
                xtype: 'displayfield',
                value: '~'
            }, {
                xtype: 'datetimefield',
                disabledMin: false,
                disabledSec: false,
                time: { hour: 23, min: 59, sec: 59 },// add by zhuoqin0830w  自定義時間空間的值  2015/03/16
                format: 'Y-m-d H:i:s',
                editable: false,
                name: 'valid_end',
                id: 'valid_end'
            }, { // add by Jiajun 2014/09/29
                xtype: 'button',
                text: ELIMINATE,
                id: 'btn_reset8',
                margin: '0 0 0 5',
                listeners: {
                    click: function () {
                        Ext.getCmp("valid_start").setValue("");
                        Ext.getCmp("valid_end").setValue("");
                    }
                }
            }]
        }]
    }];

    var same = [{
        xtype: 'checkbox',
        hidden: true,
        boxLabel: SAME_PRICE + SAME_PRICE_NOTICE_2,
        id: 'same_price',
        name: 'same_price',
        colName: 'same_price',
        listeners: {
            change: function (cmp, newValue, oldValue, eOpts) {
                if (newValue) {
                    var record = itemStore.getAt(0);
                    if (record) {
                        var money = record.get('Item_Money'),
                        cost = record.get('Item_Cost'),
                        ecost = record.get('Event_Cost'),
                        emoney = record.get('Event_Money'),
                        eicost = record.get('Event_Item_Cost'),
                        eimoney = record.get('Event_Item_Money');
                        for (var i = 1; i < itemStore.getCount() ; i++) {
                            itemStore.getAt(i).set('Item_Money', money);
                            itemStore.getAt(i).set('Item_Cost', cost);
                            itemStore.getAt(i).set('Event_Cost', ecost);
                            itemStore.getAt(i).set('Event_Money', emoney);
                            itemStore.getAt(i).set('Event_Item_Cost', eicost);
                            itemStore.getAt(i).set('Event_Item_Money', eimoney);
                        }
                    }
                }
            }
        }
    }];
    if (PRODUCT_ID == '') {
        //新增時 Form呈現內容
        frmItems.push(siteName);
        frmItems.push(defaultPrice);
        frmItems.push(defaultItem);
        frmItems.push(defaultEvent);
        frmItems.push(priceValidTime);
        frmItems.push(same);
    }
    else {
        //修改時 Form呈現內容
        frmItems.push(siteItem);
        frmItems.push(siteName);
        frmItems.push(defaultItem);
        frmItems.push(siteEvent);
        frmItems.push(priceValidTime);
        frmItems.push(same);
    }

    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        url: '/Product/SaveItemPrice',
        layout: 'anchor',
        width: 900,
        defaults: { anchor: "95%", msgTarget: "side" },
        border: false,
        plain: true,
        bodyStyle: 'padding:5px 5px 0px 5px',
        items: [frmItems]
    });

    var grid = Ext.create('Ext.grid.Panel', {
        id: 'grid',
        store: itemStore,
        plugins: [cellEditing],
        width: 960,
        height: PRODUCT_ID == '' ? 350 : 200,
        buttons: [
            '->',
            { text: SAVE, id: 'siteSave', hidden: PRODUCT_ID == '', handler: function () { save('siteSave'); } },
            {
                text: CANCEL, hidden: PRODUCT_ID == '',
                handler: function () {
                    tempPanel.hide(); showPanel.show(); frm.getForm().reset(); NEW_SITE = false; itemStore.load({ params: { ProductId: PRODUCT_ID, OldProductId: OLD_PRODUCT_ID } });
                }
            }
        ],
        columns: [
            { xtype: 'rownumberer', width: 30, align: 'center' },
            { colName: 'Item_Id', header: DETAIL_CODE, dataIndex: 'Item_Id', width: 85, align: 'center', sortable: false, menuDisabled: true, hidden: false },//edit by zhuoqin0830w 2015/01/27 增加細項編號
            { colName: 'spec_id_1', header: ITEM_SPEC1, dataIndex: 'Spec_Name_1', width: 130, align: 'center', sortable: false, menuDisabled: true, hidden: true },
            { colName: 'spec_id_2', header: ITEM_SPEC2, dataIndex: 'Spec_Name_2', width: 130, align: 'center', sortable: false, menuDisabled: true, hidden: true },
            {
                colName: 'item_code', header: ITEM_CODE, dataIndex: 'Item_Code', width: 103, align: 'center', sortable: false, menuDisabled: true, hidden: true,
                editor: {
                    xtype: 'textfield'
                }
            },
            {
                colName: 'item_money', header: ITEM_MONEY + NOT_EMPTY, dataIndex: 'Item_Money', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: true,
                editor: {
                    xtype: 'numberfield',
                    decimalPrecision: 0,
                    minValue: 0, //2014/09/17
                    allowBlank: false,
                    listeners: {
                        blur: function () {
                            if (Ext.getCmp('same_price').getValue()) {
                                for (var i = 0; i < itemStore.getCount() ; i++) {
                                    itemStore.getAt(i).set('Item_Money', this.getValue());
                                }
                            }
                        }
                    }
                }
            },
            {
                colName: 'item_cost', header: ITEM_COST + NOT_EMPTY, dataIndex: 'Item_Cost', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: true,
                editor: {
                    xtype: 'numberfield',
                    decimalPrecision: 0,
                    minValue: 0, //2014/09/17
                    allowBlank: false,
                    listeners: {
                        blur: function () {
                            if (Ext.getCmp('same_price').getValue()) {
                                for (var i = 0; i < itemStore.getCount() ; i++) {
                                    itemStore.getAt(i).set('Item_Cost', this.getValue());
                                }
                            }
                        }
                    }
                }
            },
            {
                colName: 'event_money', header: ITEM_EVENT_MONEY, dataIndex: 'Event_Money', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: true,
                editor: {
                    xtype: 'numberfield',
                    decimalPrecision: 0,
                    minValue: 0,
                    listeners: {
                        blur: function () {
                            if (Ext.getCmp('same_price').getValue()) {
                                for (var i = 0; i < itemStore.getCount() ; i++) {
                                    itemStore.getAt(i).set('Event_Money', this.getValue());
                                }
                            }
                        }
                    }
                }
            },
            {
                colName: 'event_cost', header: ITEM_EVENT_COST, dataIndex: 'Event_Cost', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: true,
                editor: {
                    xtype: 'numberfield',
                    decimalPrecision: 0,
                    minValue: 0,
                    listeners: {
                        blur: function () {
                            if (Ext.getCmp('same_price').getValue()) {
                                for (var i = 0; i < itemStore.getCount() ; i++) {
                                    itemStore.getAt(i).set('Event_Cost', this.getValue());
                                }
                            }
                        }
                    }
                }
            },
            {
                colName: 'event_item_money', header: ITEM_EVENT_MONEY, dataIndex: 'Event_Item_Money', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: true,
                editor: {
                    xtype: 'numberfield',
                    decimalPrecision: 0,
                    minValue: 0,
                    listeners: {
                        blur: function () {
                            if (Ext.getCmp('same_price').getValue()) {
                                for (var i = 0; i < itemStore.getCount() ; i++) {
                                    itemStore.getAt(i).set('Event_Item_Money', this.getValue());
                                }
                            }
                        }
                    }
                }
            },
            {
                colName: 'event_item_cost', header: ITEM_EVENT_COST, dataIndex: 'Event_Item_Cost', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: true,
                editor: {
                    xtype: 'numberfield',
                    decimalPrecision: 0,
                    minValue: 0,
                    listeners: {
                        blur: function () {
                            if (Ext.getCmp('same_price').getValue()) {
                                for (var i = 0; i < itemStore.getCount() ; i++) {
                                    itemStore.getAt(i).set('Event_Item_Cost', this.getValue());
                                }
                            }
                        }
                    }
                }
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

    var siteProdutctGrid = Ext.create('Ext.grid.Panel', {
        id: 'siteProdutctGrid',
        store: siteProductStore,
        width: 960,
        height: 200,
        selModel: sm,
        tbar: [{
            hidden: true,
            colName: 'new_site_price',
            text: NEW_SITE_PRICE,
            iconCls: 'ui-icon ui-icon-money-add',
            handler: function () {
                NEW_SITE = true;
                window.parent.updateAuth(frm, 'colName');
                grid.query('*[colName=event_item_money]')[0].hide();
                grid.query('*[colName=event_item_cost]')[0].hide();

                for (var i = 0; i < itemStore.getCount() ; i++) {
                    var record = itemStore.getAt(i);
                    if (record) {
                        record.set('Event_Cost', record.get('Event_Item_Cost'));
                        record.set('Event_Money', record.get('Event_Item_Money'));
                    }
                }
                if (NO_SPEC) {
                    Ext.getCmp('frm').down('#same_price').setValue(true);
                }
                FULL_NAME = SPLIT + SPLIT + SPLIT;
                showPanel.hide();
                tempPanel.show();
                window.parent.GetProduct(window);
            }
        }, {
            hidden: true,
            colName: 'update_site_price',
            text: UPDATE_SITE_PRICE,
            iconCls: 'ui-icon ui-icon-paper-write',
            id: 'update_site_price',
            disabled: true,
            handler: function () {
                var row = siteProdutctGrid.getSelectionModel().getSelection();
                if (row.length == 0) {
                    Ext.Msg.alert(INFORMATION, NO_SELECTION);
                }
                else if (row.length > 1) {
                    Ext.Msg.alert(INFORMATION, ONE_SELECTION);
                } else if (row.length == 1) {
                    Ext.getCmp('tempPanel').hide();
                    Ext.getCmp('showPanel').hide();
                    SitePrice(row[0]);
                }
            }
        }, {
            text: PRICE_UP,
            id: 'price_up',
            iconCls: 'ui-icon ui-icon-up',
            colName: 'price_up',
            hidden: true,
            /*disabled: true,*/
            handler: function () { updatePriceStatus("up") }
        }, {
            text: PRICE_DOWN,
            id: 'price_down',
            colName: 'price_down',
            iconCls: 'ui-icon ui-icon-down',
            hidden: true,
            /*disabled: true,*/
            handler: function () { updatePriceStatus("down") }
        }],
        columns: [
             { hidden: false, header: MAJOR_CODE, dataIndex: 'price_master_id', width: 100, align: 'center', sortable: false, menuDisabled: true },//edit by zhuoqin0830w 2015/01/27 增加主檔編碼
            { colName: 'site_name', header: SITE_NAME, dataIndex: 'site_name', width: 130, align: 'center', sortable: false, menuDisabled: true, hidden: true },
            { colName: 'product_name', header: PRODUCT_NAME, dataIndex: 'product_name', width: 180, align: 'left', sortable: false, menuDisabled: true, hidden: true },
            { colName: 'user_level', header: USER_LEVEL, dataIndex: 'user_level_name', width: 100, align: 'center', sortable: false, menuDisabled: true, hidden: true },
            { colName: 'user_id', header: USER_EMAIL, dataIndex: 'user_email', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: true },
            { colName: 'price_status', header: PRICE_STATUS, dataIndex: 'status', width: 100, align: 'center', sortable: false, menuDisabled: true, hidden: true },
            { colName: 'item_money', header: ITEM_MONEY, dataIndex: 'item_money', width: 100, align: 'center', sortable: false, menuDisabled: true, hidden: true },
            { colName: 'item_cost', header: ITEM_COST, dataIndex: 'item_cost', width: 100, align: 'center', sortable: false, menuDisabled: true, hidden: true },
            { colName: 'default_bonus_percent', header: DEFAULT_BONUS_PERCENT, dataIndex: 'default_bonus_percent', width: 100, align: 'center', sortable: false, menuDisabled: true, hidden: true },
            { colName: 'event_item_money', header: ITEM_EVENT_MONEY, dataIndex: 'event_money', width: 100, align: 'center', sortable: false, menuDisabled: true, hidden: true },
            { colName: 'event_item_cost', header: ITEM_EVENT_COST, dataIndex: 'event_cost', width: 100, align: 'center', sortable: false, menuDisabled: true, hidden: true },
            {
                colName: 'event_start', header: EVENT_TIME, xtype: 'templatecolumn',
                tpl: Ext.create('Ext.XTemplate',
                    '{[values.event_start==0?"":Ext.Date.format(new Date(values.event_start * 1000),"Y/m/d H:i:s")]}',
                    '~',
                    '{[values.event_end==0?"":Ext.Date.format(new Date(values.event_end * 1000),"Y/m/d H:i:s")]}'
                ),
                width: 220, align: 'left', sortable: false, menuDisabled: true, hidden: true
            },
            { colName: 'bonus_percent', header: BONUS_PERCENT, dataIndex: 'bonus_percent', width: 100, align: 'center', sortable: false, menuDisabled: true, hidden: true },
            {
                colName: 'bonus_percent_start', header: BONUS_PERCENT_TIME, xtype: 'templatecolumn',
                tpl: Ext.create('Ext.XTemplate',
                    '{[values.bonus_percent_start==0?"":Ext.Date.format(new Date(values.bonus_percent_start * 1000),"Y/m/d H:i:s")]}',
                    '~',
                    '{[values.bonus_percent_end==0?"":Ext.Date.format(new Date(values.bonus_percent_end * 1000),"Y/m/d H:i:s")]}'
                ),
                width: 220, align: 'left', sortable: false, menuDisabled: true, hidden: true
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

    var gigadeGrid = Ext.create('Ext.grid.Panel', {
        store: itemStore,
        width: 960,
        height: 270,
        columns: [
            { xtype: 'rownumberer', width: 50, align: 'center' },
            { colName: 'spec_id_1', header: ITEM_SPEC1, dataIndex: 'Spec_Name_1', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: true },
            { colName: 'spec_id_2', header: ITEM_SPEC2, dataIndex: 'Spec_Name_2', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: true },
            { colName: 'item_code', header: ITEM_CODE, dataIndex: 'Item_Code', width: 180, align: 'center', sortable: false, menuDisabled: true, hidden: true },
            { colName: 'item_money', header: ITEM_MONEY, dataIndex: 'Item_Money', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: true },
            { colName: 'item_cost', header: ITEM_COST, dataIndex: 'Item_Cost', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: true },
            { colName: 'event_item_money', header: ITEM_EVENT_MONEY, dataIndex: 'Event_Item_Money', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: true },
            { colName: 'event_item_cost', header: ITEM_EVENT_COST, dataIndex: 'Event_Item_Cost', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: true }
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
    //新增時呈現Panel
    var tempPanel = Ext.create('Ext.panel.Panel', {
        id: 'tempPanel',
        layout: 'anchor',
        border: false,
        hidden: PRODUCT_ID != '',
        items: [frm, grid],
        listeners: {
            afterrender: function () {
                window.parent.updateAuth(tempPanel, 'colName');
            },
            show: function () {
                tempPanel.setWidth(960);
                Ext.getCmp('same_price').setValue(Ext.getCmp('showPanel').query('*[colName=same_price]')[0].getValue());

            }
        }
    });
    //修改時呈現Panel
    var showPanel = Ext.create('Ext.panel.Panel', {
        id: 'showPanel',
        layout: 'anchor',
        border: false,
        hidden: PRODUCT_ID == '',
        items: [{
            xtype: 'form',
            id: 'profrm',
            padding: '5 5',
            border: 0,
            items: PRODUCT_ID == '' ? [] : defaultPrice,
            buttonAlign: 'center',
            buttons: [{ text: SAVE, id: 'savePrice', handler: function () { savePrice('savePrice'); } }]
        }, {
            xtype: 'displayfield',
            padding: '0 5',
            value: SAME_PRICE_NOTICE
        }, siteProdutctGrid, {
            xtype: 'displayfield',
            padding: '10 5 0',
            value: GIGADE_PRODUCT_ITEM
        }, {
            xtype: 'checkbox',
            padding: '0 5',
            disabled: true,
            colName: 'same_price',
            boxLabel: SAME_PRICE
        },
        gigadeGrid],
        listeners: {
            afterrender: function () {
                window.parent.updateAuth(showPanel, 'colName');
                showPanel.setWidth(960);
                if (PRODUCT_ID == '') {
                    var obj = Ext.getCmp('tempPanel').down('#same_price');
                    obj.setValue(true);
                    obj.fireEvent("change", obj, true)
                }
            },
            show: function () {
                showPanel.setWidth(960);

            }
        }
    });

    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        border: false,
        items: [tempPanel, showPanel],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                this.doLayout();
            }

        }
    });

    itemStore.load({
        params: {
            ProductId: PRODUCT_ID,
            OldProductId: OLD_PRODUCT_ID
        }
    });
    window.parent.GetProduct(this);
});


var NO_SPEC = false;
function setForm(result) {
    NO_SPEC = result.Product_Spec == 0;
    if (NO_SPEC) {
        Ext.getCmp('frm').down('#same_price').setReadOnly(NO_SPEC);
        Ext.getCmp('frm').down('#same_price').setValue(true);
    }


    Ext.getCmp('tempPanel').down('#Prod_Sz').setValue(result.Prod_Sz);
    //Ext.getCmp('tempPanel').down('#product_name').setValue(result.Product_Name);
    Ext.getCmp('tempPanel').down('#product_name').setValue(result.Prod_Name);
    prod_sz = result.Prod_Sz;
    if (PRODUCT_ID != '') {
        Ext.getCmp('showPanel').down('#product_price_list').setValue(result.Product_Price_List);
        Ext.getCmp('showPanel').down('#show_listprice').setValue(result.show_listprice);
        Ext.getCmp('showPanel').down('#bag_check_money').setValue(result.Bag_Check_Money);
        //product_mode:2(寄倉),不為寄倉則不能設置寄倉費
        Ext.getCmp('showPanel').down('#bag_check_money').setReadOnly(result.Product_Mode != 2);
        siteProductStore.load({
            params: {
                ProductId: PRODUCT_ID
            },
            callback: function (records, operation, success) {
                siteProductStore.findBy(function (record) {
                    if (record.get('site_id') == 1 && record.get('user_level') == 1 && record.get('user_id') == 0) {
                        Ext.getCmp('showPanel').query('*[colName=same_price]')[0].setValue(record.get('same_price'));
                        return true;
                    }
                });
            }
        });
    }
    else {
        if (result.Product_Price_List != 0) {  //add by Jiajun 2014/09/18
            Ext.getCmp('frm').down('#product_price_list').setValue(result.Product_Price_List);
        }
        Ext.getCmp('frm').down('#show_listprice').setValue(result.show_listprice);
        Ext.getCmp('frm').down('#bag_check_money').setValue(result.Bag_Check_Money);
        //product_mode:2(寄倉),不為寄倉則不能設置寄倉費
        Ext.getCmp('frm').down('#bag_check_money').setReadOnly(result.Product_Mode != 2);
        Ext.getCmp('grid').query('*[colName=event_money]')[0].hide();
        Ext.getCmp('grid').query('*[colName=event_cost]')[0].hide();
        Ext.Ajax.request({
            url: '/Product/GetPriceMaster',
            method: 'post',
            params: {
                OldProductId: OLD_PRODUCT_ID
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result) {
                    if (!NO_SPEC) Ext.getCmp('tempPanel').query('*[colName=same_price]')[0].setValue(result.same_price);
                    Ext.getCmp('tempPanel').query('*[colName=accumulated_bonus]')[0].setValue(result.accumulated_bonus);
                    Ext.getCmp('tempPanel').down('#default_bonus_percent').setValue(result.default_bonus_percent);
                    Ext.getCmp('tempPanel').down('#bonus_percent').setValue(result.bonus_percent);

                    FULL_NAME = result.product_name;
                    var productName = result.product_name.split(SPLIT);
                    Ext.getCmp('tempPanel').down('#product_name').setValue(productName[1]);
                    var time;
                    if (result.bonus_percent_start != 0) {
                        time = new Date(result.bonus_percent_start * 1000);
                        Ext.getCmp('tempPanel').down('#bonus_percent_start').setValue(Ext.Date.format(time, "Y-m-d H:i:s"));
                    }
                    if (result.bonus_percent_end != 0) {
                        time = new Date(result.bonus_percent_end * 1000);
                        Ext.getCmp('tempPanel').down('#bonus_percent_end').setValue(Ext.Date.format(time, "Y-m-d H:i:s"));
                    }
                    if (result.event_start != 0) {
                        time = new Date(result.event_start * 1000);
                        Ext.getCmp('tempPanel').down('#event_product_start').setValue(Ext.Date.format(time, "Y-m-d H:i:s"));
                    }
                    if (result.event_end != 0) {
                        time = new Date(result.event_end * 1000);
                        Ext.getCmp('tempPanel').down('#event_product_end').setValue(Ext.Date.format(time, "Y-m-d H:i:s"));
                    }
                    if (result.valid_start != 0) {
                        time = new Date(result.valid_start * 1000);
                        Ext.getCmp('tempPanel').down('#valid_start').setValue(Ext.Date.format(time, "Y-m-d H:i:s"));
                    }
                    if (result.valid_end != 0) {
                        time = new Date(result.valid_end * 1000);
                        Ext.getCmp('tempPanel').down('#valid_end').setValue(Ext.Date.format(time, "Y-m-d H:i:s"));
                    }
                }
            },
            failure: function () {
                Ext.Msg.alert(INFORMATION, DATA_LOAD_FAILURE);
            }
        })
    }
}
function savePrice(functionid) {
    var frm = Ext.getCmp('showPanel').down('#profrm');
    if (frm.getForm().isValid()) {
        var values = frm.getForm().getValues(true) + "&ProductId=" + PRODUCT_ID + "&function=" + functionid + "&batch=" + window.parent.GetBatchNo();
        Ext.Ajax.request({
            url: '/Product/UpdatePrice',
            method: 'POST',
            params: values,
            async: false,
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) { Ext.Msg.alert(INFORMATION, SUCCESS); }
            },
            failure: function (form, action) {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        });
    }
}

/// add by wwei0216w 2015/8/18
/// 將原先的save語句改成現行模式,用來增加對價格提示框的判斷
function save(functionid) {
    if (OLD_PRODUCT_ID != '') {///如果OLD_PRODUCT_ID不為空,則證明是複製
        if (confirm(AREYOUSURE)) {//此時添加消息框彈出
            return saveIds(functionid);
        }
    }else {
        return saveIds(functionid);///否則直接調用之前的save方法既現在的saveIds方法
    }
}
//保存數據至數據庫
function saveIds(functionid) {
    cellEditing.completeEdit();
    var success = false;
    var frm = Ext.getCmp('frm');
            if (frm.getForm().isValid()) {
                var dates = frm.query('datetimefield');
                var event_null = true;
                if (dates[2].getValue() && dates[3].getValue()) {
                    event_null = false;
                }
                var percent = frm.down('#bonus_percent').getValue();
                if (!event_null && percent != 0 && !percent) {
                    window.parent.setMoveEnable(true);
                    Ext.Msg.alert(INFORMATION, BONUS_PERCENT_EMPTY);
                    return false;
                }
                var itemPrices = "[";
                for (var i = 0; i < itemStore.getCount() ; i++) {
                    var record = itemStore.getAt(i);

                    //edit by Jiajun 2014/09/18
                    if (!record.get('Item_Money') || record.get('Item_Money') == 0) {
                        window.parent.setMoveEnable(true);
                        alert(ITEM_MONEY_EMPTY);
                    }
                    if (!record.get('Item_Cost') || record.get('Item_Cost') == 0) {
                        window.parent.setMoveEnable(true);
                        alert(ITEM_COST_EMPTY);
                    }

                    if (!event_null) {
                        if (NEW_SITE) {
                            if (!record.get('Event_Money') || !record.get('Event_Cost')) {
                                window.parent.setMoveEnable(true);
                                Ext.Msg.alert(INFORMATION, EVENT_MONEY_EMPTY);
                                return false;
                            }
                        }
                        else {
                            if (!record.get('Event_Item_Money') || !record.get('Event_Item_Cost')) {
                                window.parent.setMoveEnable(true);
                                Ext.Msg.alert(INFORMATION, EVENT_MONEY_EMPTY);
                                return false;
                            }
                        }
                    }
                    itemPrices += "{Item_Id:\"" + record.get('Item_Id') + "\",Item_Code:\"" + Ext.htmlEncode(record.get('Item_Code'))
                                + "\",Item_Money:\"" + record.get('Item_Money') + "\",Item_Cost:\"" + record.get('Item_Cost')
                                + "\",Event_Money:\"" + record.get('Event_Money') + "\",Event_Cost:\"" + record.get('Event_Cost') + "\",Event_Item_Money:\""
                                + record.get('Event_Item_Money') + "\",Event_Item_Cost:\"" + record.get("Event_Item_Cost") + "\"}";
                }
                itemPrices += "]";
                itemPrices = itemPrices.replace(/}{/g, '},{');

                if (!functionid) {
                    functionid = '';
                }

                PRODUCT_ID = window.parent.GetProductId();
                var product_name = frm.down('#product_name') ? Ext.htmlEncode(frm.down('#product_name').getValue()) : '';
                var tmpName = FULL_NAME.split(SPLIT);
                tmpName[1] = product_name;
                tmpName[2] = frm.down('#Prod_Sz').getValue();
                product_name = "";
                for (var a = 0; a < tmpName.length; a++) {
                    product_name += tmpName[a] + SPLIT;
                }
                product_name = product_name.substring(0, product_name.lastIndexOf(SPLIT));
                var values = Ext.Object.fromQueryString(frm.getForm().getValues(true) + "&ProductId=" + PRODUCT_ID + "&OldProductId=" + OLD_PRODUCT_ID + "&function=" + functionid + "&batch=" + window.parent.GetBatchNo());
                values.Items = itemPrices;
                values.product_name = product_name;

                // +"&product_name="+Ext.htmlEncode(frm.down('#product_name').getValue())

                Ext.Ajax.request({
                    url: '/Product/SaveItemPrice',
                    method: 'POST',
                    params: values,
                    async: false,
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            if (NEW_SITE) {
                                siteProductStore.load({
                                    params: {
                                        ProductId: PRODUCT_ID
                                    }
                                });
                                itemStore.load({ params: { ProductId: PRODUCT_ID, OldProductId: OLD_PRODUCT_ID } });
                                Ext.getCmp('tempPanel').hide();
                                Ext.getCmp('showPanel').show();
                                NEW_SITE = false;
                            }
                            success = true;
                            frm.getForm().reset(); //edit by wwei0216w 保存成功后,重置showPanel
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, result.msg ? result.msg : FAILURE);
                            window.parent.setMoveEnable(true);
                        }
                    },
                    failure: function (form, action) {
                        Ext.Msg.alert(INFORMATION, result.msg ? result.msg : FAILURE);
                        window.parent.setMoveEnable(true);
                    }
                });
            }
            else {
                return false;
            }
            window.parent.setMoveEnable(true);
            //frm.getForm().reset();
            return success;
        }

function SitePrice(row) {
    var wfrm = Ext.create('Ext.form.Panel', {
        id: 'wfrm',
        layout: 'anchor',
        height: 230,
        border: false,
        layout: 'anchor',
        defaults: { anchor: '95%', msgTarget: "side", labelWidth: 120 },
        items: [{
            xtype: 'textfield',
            name: 'price_master_id',
            hidden: true
        }, {
            width: 300,
            xtype: 'displayfield',
            fieldLabel: SITE_NAME,
            hidden: true,
            colName: 'site_name',
            name: 'site_name'
        }, {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            colName: 'product_name',
            hidden: true,
            border: false,
            items: [{
                xtype: 'textfield',
                fieldLabel: PRODUCT_NAME + NOT_EMPTY,
                labelWidth: 120,
                width: 400,
                allowBlank: false,
                name: 'product_name',
                id: 'product_name1'
            }, {
                xtype: 'textfield',
                name: 'Prod_Sz',
                id: 'Prod_Sz1',
                margin: '0 0 0 5',
                width: 80,
                readOnly: true
            }]
        }, {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            anchor: '100%',
            frame: true,
            defaults: { msgTarget: "side", labelWidth: 120 },
            items: [{
                xtype: 'combobox',
                store: userLevelStore2,
                displayField: 'parameterName',
                valueField: 'parameterCode',
                queryMode: 'local',
                editable: false,
                width: 300,
                fieldLabel: USER_LEVEL + NOT_EMPTY,
                hidden: true,
                allowBlank: false,
                colName: 'user_level',
                name: 'user_level'
            }, {
                width: 300,
                xtype: 'textfield',
                margin: '0 30px',
                fieldLabel: USER_EMAIL,
                hidden: true,
                vtype: 'email',
                colName: 'user_id',
                id: 'user_id_modify',
                name: 'user_id'
            }]
        }, {
            xtype: 'checkbox',
            hidden: true,
            name: 'accumulated_bonus',
            colName: 'accumulated_bonus',
            padding: '0 125px',
            boxLabel: ACCUMULATED_BONUS
        }, {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            anchor: '100%',
            frame: true,
            defaults: { msgTarget: "side", labelWidth: 120 },
            items: [{
                width: 220,
                xtype: 'numberfield',
                decimalPrecision: 1,
                hidden: true,
                name: 'default_bonus_percent',
                colName: 'default_bonus_percent',
                fieldLabel: DEFAULT_BONUS_PERCENT,
                step: 0.1,
                minValue: 1,
                maxValue: 2,
                value: 1
            }, {
                width: 220,
                xtype: 'numberfield',
                decimalPrecision: 1,
                margin: '0 70px',
                hidden: true,
                name: 'bonus_percent',
                colName: 'bonus_percent',
                fieldLabel: BONUS_PERCENT,
                step: 0.1,
                minValue: 1,
                maxValue: 2,
                value: 1
            }]
        }, {
            xtype: 'fieldcontainer',
            hidden: true,
            colName: 'bonus_percent_start',
            layout: 'hbox',
            fieldLabel: BONUS_PERCENT_TIME,
            items: [{
                xtype: 'datetimefield',
                disabledMin: true,
                disabledSec: true,
                format: 'Y-m-d H:i:s',
                editable: false,
                name: 'bonus_percent_start',
                id: 'w_bonus_start',
                listeners: {
                    select: function () {
                        var end = new Date(Ext.getCmp("w_bonus_end").rawValue);
                        var start = new Date(this.rawValue);
                        if (end < start) {
                            Ext.Msg.alert(INFORMATION, TIME_ERROR);
                            this.setValue('');
                        }
                    }
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
                editable: false,
                id: 'w_bonus_end',
                listeners: {
                    select: function () {
                        var start = new Date(Ext.getCmp("w_bonus_start").rawValue);
                        var end = new Date(this.rawValue);
                        if (end < start) {
                            Ext.Msg.alert(INFORMATION, TIME_ERROR);
                            this.setValue('');
                        }
                    }
                }
            },
            { // add by Jiajun 2014/09/29
                xtype: 'button',
                text: ELIMINATE,
                id: 'btn_reset1',
                margin: '0 0 0 5',
                listeners: {
                    click: function () {
                        Ext.getCmp("w_bonus_start").setValue("");
                        Ext.getCmp("w_bonus_end").setValue("");
                    }
                }
            }
            ]
        }, {
            xtype: 'fieldcontainer',
            colName: 'event_start',
            hidden: true,
            layout: 'hbox',
            fieldLabel: EVENT_TIME,
            items: [{
                xtype: 'datetimefield',
                disabledMin: false,
                disabledSec: false,
                time: { hour: 0, min: 0, sec: 0 },// add by zhuoqin0830w  自定義時間空間的值  2015/03/16
                format: 'Y-m-d H:i:s',
                editable: false,
                name: 'event_start',
                id: 'w_event_start',
                listeners: {
                    select: function () {
                        var end = new Date(Ext.getCmp("w_event_end").rawValue);
                        var start = new Date(this.rawValue);
                        if (end < start) {
                            Ext.Msg.alert(INFORMATION, TIME_ERROR);
                            this.setValue('');
                        }
                    }
                }
            }, {
                xtype: 'displayfield',
                value: '~'
            }, {
                xtype: 'datetimefield',
                disabledMin: false,
                disabledSec: false,
                time: { hour: 23, min: 59, sec: 59 },// add by zhuoqin0830w  自定義時間空間的值  2015/03/16
                format: 'Y-m-d H:i:s',
                name: 'event_end',
                editable: false,
                id: 'w_event_end',
                listeners: {
                    select: function () {
                        var start = new Date(Ext.getCmp("w_event_start").rawValue);
                        var end = new Date(this.rawValue);
                        if (end < start) {
                            Ext.Msg.alert(INFORMATION, TIME_ERROR);
                            this.setValue('');
                        }
                    }
                }
            },
            { // add by Jiajun 2014/09/29
                xtype: 'button',
                text: ELIMINATE,
                id: 'btn_reset2',
                margin: '0 0 0 5',
                listeners: {
                    click: function () {
                        Ext.getCmp("w_event_start").setValue("");
                        Ext.getCmp("w_event_end").setValue("");
                    }
                }
            }]
        },
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            //id: 'priceValidTime',
            colName: 'priceValidTime',
            fieldLabel: PRODUCT_UP_DOWN_DATE,
            items: [{
                xtype: 'datetimefield',
                disabledMin: false,
                disabledSec: false,
                time: { hour: 0, min: 0, sec: 0 },// add by zhuoqin0830w  自定義時間空間的值  2015/03/16
                format: 'Y-m-d H:i:s',
                name: 'valid_start',
                id: 'w_valid_start'//重新命名ID保證後面的相關控件可以使用  zhuoqin0830w 2015/01/28
            }, {
                xtype: 'displayfield',
                value: '~'
            }, {
                xtype: 'datetimefield',
                disabledMin: false,
                disabledSec: false,
                time: { hour: 23, min: 59, sec: 59 },// add by zhuoqin0830w  自定義時間空間的值  2015/03/16
                format: 'Y-m-d H:i:s',
                name: 'valid_end',
                id: 'w_valid_end'//重新命名ID保證後面的相關控件可以使用  zhuoqin0830w 2015/01/28
            },
        { // add by Jiajun 2014/09/29
            xtype: 'button',
            text: ELIMINATE,
            id: 'btn_reset4',
            margin: '0 0 0 5',
            listeners: {
                click: function () {
                    Ext.getCmp("w_valid_start").setValue("");
                    Ext.getCmp("w_valid_end").setValue("");
                }
            }
        }]
        }, {
            xtype: 'checkbox',
            hidden: true,
            boxLabel: SAME_PRICE + SAME_PRICE_NOTICE_2,
            readOnly: NO_SPEC,
            name: 'same_price',
            colName: 'same_price',
            listeners: {
                change: function (cmp, newValue, oldValue, eOpts) {
                    if (newValue) {
                        var record = sitePriceStore.getAt(0);
                        if (record) {
                            var money = record.get('item_money'),
                                    cost = record.get('item_cost'),
                                    ecost = record.get('event_cost'),
                                    emoney = record.get('event_money');
                            for (var i = 1; i < sitePriceStore.getCount() ; i++) {
                                sitePriceStore.getAt(i).set('item_money', money);
                                sitePriceStore.getAt(i).set('item_cost', cost);
                                sitePriceStore.getAt(i).set('event_cost', ecost);
                                sitePriceStore.getAt(i).set('event_money', emoney);
                            }
                        }
                    }
                }
            }
        }]
    });
    var wgrid = Ext.create('Ext.grid.Panel', {
        id: 'wgrid',
        store: sitePriceStore,
        plugins: [cellEditing_wfrm],
        width: 860,
        height: 270,
        buttons: ['->', { text: SAVE, id: 'siteUpdate', handler: function () { update('siteUpdate'); } }, { text: CANCEL, handler: function () { Ext.getCmp('showPanel').show(); Ext.getCmp('win').destroy(); } }],
        columns: [
            { xtype: 'rownumberer', width: 50, align: 'center' },
            { colName: 'item_id', header: DETAIL_CODE, dataIndex: 'item_id', width: 80, align: 'center', sortable: false, menuDisabled: true, hidden: false },//edit by zhuoqin0830w 2015/01/27 增加細項編號
            { colName: 'spec_id_1', header: ITEM_SPEC1, dataIndex: 'spec_name_1', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: true },
            { colName: 'spec_id_2', header: ITEM_SPEC2, dataIndex: 'spec_name_2', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: true },
            {
                colName: 'item_money', header: ITEM_MONEY + NOT_EMPTY, dataIndex: 'item_money', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: true,
                editor: {
                    xtype: 'numberfield',
                    decimalPrecision: 0,
                    minValue: 0,
                    allowBlank: false,
                    listeners: {
                        blur: function () {
                            if (wfrm.query('*[colName=same_price]')[0].getValue()) {
                                for (var i = 0; i < sitePriceStore.getCount() ; i++) {
                                    sitePriceStore.getAt(i).set('item_money', this.getValue());
                                }
                            }
                        }
                    }
                }
            },
            {
                colName: 'item_cost', header: ITEM_COST + NOT_EMPTY, dataIndex: 'item_cost', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: true,
                editor: {
                    xtype: 'numberfield',
                    decimalPrecision: 0,
                    minValue: 0,
                    allowBlank: false,
                    listeners: {
                        blur: function () {
                            if (wfrm.query('*[colName=same_price]')[0].getValue()) {
                                for (var i = 0; i < sitePriceStore.getCount() ; i++) {
                                    sitePriceStore.getAt(i).set('item_cost', this.getValue());
                                }
                            }
                        }
                    }
                }
            },
            {
                colName: 'event_money', header: ITEM_EVENT_MONEY, dataIndex: 'event_money', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: true,
                editor: {
                    xtype: 'numberfield',
                    decimalPrecision: 0,
                    minValue: 0,
                    listeners: {
                        blur: function () {
                            if (wfrm.query('*[colName=same_price]')[0].getValue()) {
                                for (var i = 0; i < sitePriceStore.getCount() ; i++) {
                                    sitePriceStore.getAt(i).set('event_money', this.getValue());
                                }
                            }
                        }
                    }
                }
            },
            {
                colName: 'event_cost', header: ITEM_EVENT_COST, dataIndex: 'event_cost', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: true,
                editor: {
                    xtype: 'numberfield',
                    decimalPrecision: 0,
                    minValue: 0,
                    listeners: {
                        blur: function () {
                            if (wfrm.query('*[colName=same_price]')[0].getValue()) {
                                for (var i = 0; i < sitePriceStore.getCount() ; i++) {
                                    sitePriceStore.getAt(i).set('event_cost', this.getValue());
                                }
                            }
                        }
                    }
                }
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
    Ext.create('Ext.panel.Panel', {
        id: 'win',
        items: [wfrm, wgrid],
        width: 870,
        height: 520,
        layout: 'anchor',
        labelWidth: 120,
        closeAction: 'destroy',
        border: false,
        renderTo: Ext.getBody(),
        bodyStyle: 'padding:5px 5px 5px 5px',
        listeners: {
            "afterrender": function () {
                //userLevelStore.load();
                window.parent.updateAuth(wfrm, 'colName');
                window.parent.updateAuth(wgrid, 'colName');
                if (row) {
                    wfrm.getForm().loadRecord(row);
                    Ext.getCmp("Prod_Sz1").setValue(prod_sz);
                    NAME_FORMAT = row.data.product_name_format;
                    var productName = row.data.product_name_format.split(SPLIT);
                    Ext.getCmp("product_name1").setValue(productName[1]);

                    var time;
                    if (row.data.event_start != 0) {
                        time = new Date(row.data.event_start * 1000);
                        wfrm.query('*[name=event_start]')[0].setValue(Ext.Date.format(time, "Y-m-d H:i:s"));
                    }
                    if (row.data.event_end != 0) {
                        time = new Date(row.data.event_end * 1000);
                        wfrm.query('*[name=event_end]')[0].setValue(Ext.Date.format(time, "Y-m-d H:i:s"));
                    }
                    if (row.data.bonus_percent_start != 0) {
                        time = new Date(row.data.bonus_percent_start * 1000);
                        wfrm.query('*[name=bonus_percent_start]')[0].setValue(Ext.Date.format(time, "Y-m-d H:i:s"));
                    }
                    if (row.data.bonus_percent_end != 0) {
                        time = new Date(row.data.bonus_percent_end * 1000);
                        wfrm.query('*[name=bonus_percent_end]')[0].setValue(Ext.Date.format(time, "Y-m-d H:i:s"));
                    }
                    if (row.data.valid_start != 0) {
                        time = new Date(row.data.valid_start * 1000);
                        wfrm.query('*[name=valid_start]')[0].setValue(Ext.Date.format(time, "Y-m-d H:i:s"));
                    }
                    if (row.data.valid_end != 0) {
                        time = new Date(row.data.valid_end * 1000);
                        wfrm.query('*[name=valid_end]')[0].setValue(Ext.Date.format(time, "Y-m-d H:i:s"));
                    }

                    sitePriceStore.removeAll();
                    sitePriceStore.load({
                        params: {
                            PriceMasterId: row.data.price_master_id
                        }
                    });
                    //                    if (NO_SPEC && !wfrm.query('*[name=same_price]')[0].getValue()) {
                    //                        wfrm.query('*[name=same_price]')[0].setValue(NO_SPEC);
                    //                    }
                }
            }
        }
    });         //.show();

    Ext.getCmp('user_id_modify').setValue(row.data.user_email);
}

function update(functionid) {

    Ext.Msg.confirm(CONFIRM, AREYOUSURE, function (btn) {
        if (btn == 'yes') {
            var frm = Ext.getCmp('wfrm');
            if (frm.getForm().isValid()) {
                var dates = frm.query('datetimefield');
                var event_null = true;
                if (dates[2].getValue() && dates[3].getValue()) {
                    event_null = false;
                }
                var percent = frm.query('*[colName=bonus_percent]')[0].getValue();
                if (!event_null && percent != 0 && !percent) {
                    Ext.Msg.alert(INFORMATION, BONUS_PERCENT_EMPTY);
                    return false;
                }
                var itemPrices = "[";
                for (var i = 0; i < sitePriceStore.getCount() ; i++) {
                    var record = sitePriceStore.getAt(i);
                    if (!record.get('item_money') || record.get('item_money') == "0") {
                        Ext.Msg.alert(INFORMATION, ITEM_MONEY_EMPTY);
                    }
                    if (!event_null) {
                        if (!record.get('event_money') || !record.get('event_cost') || record.get('event_cost') == "0") {
                            Ext.Msg.alert(INFORMATION, EVENT_MONEY_EMPTY);
                        }
                    }
                    itemPrices += "{item_price_id:\"" + record.get('item_price_id') + "\",item_id:\"" + record.get('item_id')
                                + "\",price_master_id:\"" + record.get('price_master_id') + "\",item_money:\"" + record.get('item_money')
                                + "\",item_cost:\"" + record.get('item_cost') + "\",event_money:\"" + record.get('event_money') + "\",event_cost:\""
                                + record.get('event_cost') + "\"}";
                }
                itemPrices += "]";
                itemPrices = itemPrices.replace(/}{/g, '},{');

                if (!functionid) {
                    functionid = '';
                }
                var product_name = Ext.htmlEncode(Ext.getCmp("product_name1").getValue());
                var tmpName = NAME_FORMAT.split(SPLIT);
                tmpName[1] = product_name;
                tmpName[2] = Ext.getCmp("Prod_Sz1").getValue();
                product_name = "";
                for (var a = 0; a < tmpName.length; a++) {
                    product_name += tmpName[a] + SPLIT;
                }
                product_name = product_name.substring(0, product_name.lastIndexOf(SPLIT));
                PRODUCT_ID = window.parent.GetProductId();
                var values = Ext.Object.fromQueryString(frm.getForm().getValues(true) + "&Items=" + itemPrices + "&function=" + functionid + "&batch=" + window.parent.GetBatchNo());
                values.productFormat = product_name;
                //+ "&product_name=" + Ext.htmlEncode(frm.query('*[colName=product_name]')[0].getValue())
                Ext.Ajax.request({
                    url: '/Product/UpdateItemPrice',
                    method: 'POST',
                    params: values,
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            // edit by  zhuoqin0830w 2015/01/14  增加提示功能
                            Ext.MessageBox.alert(ALERT_MESSAGE, FIX_COMPLETE,
                                                    function () {
                                                        Ext.getCmp('showPanel').show(); Ext.getCmp('win').destroy();
                                                    });
                            siteProductStore.load({
                                params: {
                                    ProductId: PRODUCT_ID
                                },
                                callback: function (records, operation, success) {
                                    if (PRODUCT_ID != '') {
                                        var record = siteProductStore.findRecord('site_id', 1);
                                        if (record) {
                                            Ext.getCmp('showPanel').query('*[colName=same_price]')[0].setValue(record.data.same_price);
                                        }
                                    }
                                }
                            });
                            itemStore.load({ params: { ProductId: PRODUCT_ID } });
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, result.msg);
                            return false;
                        }
                    },
                    failure: function (form, action) {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                        return false;
                    }
                });
            }
            else {
                return false;
            }
        } else {
            return false;
        }
    })


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
        var values = Ext.Object.fromQueryString("function=" + "siteUpdate" + "&batch=" + window.parent.GetBatchNo());
        values.priceMasterId = row[0].get('price_master_id');
        values.updateStatus = updateStatus;

        Ext.Ajax.request({
            url: '/Product/UpdatePriceStatus',
            method: 'POST',
            params: values,
            async: false,
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    Ext.Msg.alert(INFORMATION, SUCCESS);
                    siteProductStore.load({
                        params: {
                            ProductId: PRODUCT_ID
                        }
                    });
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