var NEW_SITE = false;
var PRODUCT_ID = '', OLD_PRODUCT_ID = '';
var ISEDIT = '';


Ext.define('GIGADE.PRODUCTITEM', {//productitem新增時每個item的價格
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
//獲取商品細項價格
var itemStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.PRODUCTITEM',
    proxy: {
        type: 'ajax',
        url: '/VendorProduct/GetProItems',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
});
//控制grid列的修改
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

Ext.define('GIGADE.PRODUCTSITE', {//站台價格grid
    extend: 'Ext.data.Model',
    fields: [
        { name: 'price_master_id', type: 'string' },
        { name: 'product_id', type: 'string' },
        { name: 'site_id', type: 'string' },
        { name: 'site_name', type: 'string' },
        { name: 'product_name', type: 'string' },
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
        { name: 'valid_end', type: 'string' }
    ]
});
var siteProductStore = Ext.create("Ext.data.Store", {
    model: 'GIGADE.PRODUCTSITE',
    proxy: {
        type: 'ajax',
        url: '/VendorProduct/GetPriceMaster',
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
        url: '/VendorProduct/GetItemPrice',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
})


Ext.onReady(function () {
    PRODUCT_ID = window.parent.GetProductId();
    OLD_PRODUCT_ID = window.parent.GetCopyProductId();
    ISEDIT = window.parent.GetIsEdit() == "true" ? true : false; //確定當前面板是否是修改
    var frmItems = new Array();

    var siteName = [{
        border: false,
        width: 500,
        items: [{
            xtype: 'textfield',
            labelWidth: 120,
            width: 400,
            fieldLabel: PRODUCT_NAME + NOT_EMPTY, //商品顯示名稱
            // hidden: false,
            submitValue: false,
            allowBlank: false,
            id: 'product_name',
            colName: 'product_name',
            name: 'product_name'
        }]
    }];
    var defaultPrice = [{
        border: false,
        defaults: { labelWidth: 120 },
        items: [{
            xtype: 'checkbox',
            boxLabel: FRONT_SHOW_PRICE, //前臺顯示建議售價
            name: 'show_listprice',
            colName: 'show_listprice',
            id: 'show_listprice',
            hidden: false,
            padding: '0 125px'
        }, {
            xtype: 'numberfield',
            decimalPrecision: 0,
            hidden: false,
            fieldLabel: PRODUCT_PRICE_LIST + NOT_EMPTY, //建議售價
            minValue: 0,
            allowBlank: false,
            name: 'product_price_list',
            colName: 'product_price_list',
            id: 'product_price_list'
        }, {
            xtype: 'numberfield',
            decimalPrecision: 0,
            hidden: false,
            fieldLabel: BAG_CHECK_MONEY + NOT_EMPTY, //寄倉費
            minValue: 0,
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
            hidden: false,
            colName: 'event_product_start',
            layout: 'hbox',
            width: 900,
            fieldLabel: EVENT_TIME, //特價活動期間
            items: [
            {
                xtype: 'datetimefield',
                disabledMin: true,
                disabledSec: true,
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
            },

            {
                xtype: 'displayfield',
                value: '~'
            },

            {
                xtype: 'datetimefield',
                disabledMin: true,
                disabledSec: true,
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
            },
               {
                   xtype: 'button',
                   text: '清除時間',
                   handler: function () {
                       Ext.getCmp('event_product_start').setValue('');
                       Ext.getCmp('event_product_end').setValue('');
                   }
               }
            ]
        }]
    }];
    var siteEvent = [{
        border: false,
        defaults: { labelWidth: 120 },
        width: 900,
        items: [{
            xtype: 'fieldcontainer',
            hidden: false,
            colName: 'event_start',
            layout: 'hbox',
            width: 900,
            fieldLabel: EVENT_TIME, //特價活動期間
            items: [{
                xtype: 'datetimefield',
                disabledMin: true,
                disabledSec: true,
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
                disabledMin: true,
                disabledSec: true,
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
            }

            ]
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
            fieldLabel: '上下架時間',
            items: [{
                xtype: 'datetimefield',
                disabledMin: true,
                disabledSec: true,
                format: 'Y-m-d H:i:s',
                editable: false,
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
                editable: false,
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
        }]
    }];
    var same = [{
        xtype: 'checkbox',
        hidden: false,
        boxLabel: SAME_PRICE + SAME_PRICE_NOTICE_2, //所有規格同價
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
                        for (var i = 1; i < itemStore.getCount(); i++) {
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

    if (!ISEDIT) {  //新增時 Form呈現內容
        frmItems.push(siteName); //商品顯示名稱
        frmItems.push(defaultPrice);
        frmItems.push(defaultEvent);
        frmItems.push(priceValidTime);
        frmItems.push(same);
    }
    else {
        //修改時 Form呈現內容
        frmItems.push(siteName);
        frmItems.push(siteEvent);
        frmItems.push(priceValidTime);
        frmItems.push(same);
    }

    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        url: '/VendorProduct/SaveItemPrice',
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
        height: !ISEDIT ? 350 : 200,
        buttons: [
            '->',
            { text: SAVE, id: 'siteSave', hidden: !ISEDIT, handler: function () { save(); } },
            { text: CANCEL, hidden: !ISEDIT,
                handler: function () { tempPanel.hide(); showPanel.show(); frm.getForm().reset(); NEW_SITE = false; itemStore.load({ params: { ProductId: PRODUCT_ID, OldProductId: OLD_PRODUCT_ID} }); }
            }
        ],
        columns: [
            { xtype: 'rownumberer', width: 50, align: 'center' },
            { colName: 'spec_id_1', header: ITEM_SPEC1, dataIndex: 'Spec_Name_1', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'spec_id_2', header: ITEM_SPEC2, dataIndex: 'Spec_Name_2', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'item_code', header: ITEM_CODE, dataIndex: 'Item_Code', width: 180, align: 'center', sortable: false, menuDisabled: true, hidden: false,
                editor: {
                    xtype: 'textfield'
                }
            },
            { colName: 'item_money', header: ITEM_MONEY + NOT_EMPTY, dataIndex: 'Item_Money', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: false,
                editor: {
                    xtype: 'numberfield',
                    decimalPrecision: 0,
                    minValue: 0,
                    allowBlank: false,
                    listeners: {
                        blur: function () {
                            if (Ext.getCmp('same_price').getValue()) {
                                for (var i = 0; i < itemStore.getCount(); i++) {
                                    itemStore.getAt(i).set('Item_Money', this.getValue());
                                }
                            }
                        }
                    }
                }
            },
            { colName: 'item_cost', header: ITEM_COST + NOT_EMPTY, dataIndex: 'Item_Cost', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: false,
                editor: {
                    xtype: 'numberfield',
                    decimalPrecision: 0,
                    minValue: 0,
                    allowBlank: false,
                    listeners: {
                        blur: function () {
                            if (Ext.getCmp('same_price').getValue()) {
                                for (var i = 0; i < itemStore.getCount(); i++) {
                                    itemStore.getAt(i).set('Item_Cost', this.getValue());
                                }
                            }
                        }
                    }
                }
            },
            { colName: 'event_money', header: ITEM_EVENT_MONEY, dataIndex: 'Event_Money', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: false,
                editor: {
                    xtype: 'numberfield',
                    decimalPrecision: 0,
                    minValue: 0,
                    listeners: {
                        blur: function () {
                            if (Ext.getCmp('same_price').getValue()) {
                                for (var i = 0; i < itemStore.getCount(); i++) {
                                    itemStore.getAt(i).set('Event_Money', this.getValue());
                                }
                            }
                        }
                    }
                }
            },
            { colName: 'event_cost', header: ITEM_EVENT_COST, dataIndex: 'Event_Cost', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: false,
                editor: {
                    xtype: 'numberfield',
                    decimalPrecision: 0,
                    minValue: 0,
                    listeners: {
                        blur: function () {
                            if (Ext.getCmp('same_price').getValue()) {
                                for (var i = 0; i < itemStore.getCount(); i++) {
                                    itemStore.getAt(i).set('Event_Cost', this.getValue());
                                }
                            }
                        }
                    }
                }
            },
            { colName: 'event_item_money', header: ITEM_EVENT_MONEY, dataIndex: 'Event_Item_Money', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: false,
                editor: {
                    xtype: 'numberfield',
                    decimalPrecision: 0,
                    minValue: 0,
                    listeners: {
                        blur: function () {
                            if (Ext.getCmp('same_price').getValue()) {
                                for (var i = 0; i < itemStore.getCount(); i++) {
                                    itemStore.getAt(i).set('Event_Item_Money', this.getValue());
                                }
                            }
                        }
                    }
                }
            },
            { colName: 'event_item_cost', header: ITEM_EVENT_COST, dataIndex: 'Event_Item_Cost', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: false,
                editor: {
                    xtype: 'numberfield',
                    decimalPrecision: 0,
                    minValue: 0,
                    listeners: {
                        blur: function () {
                            if (Ext.getCmp('same_price').getValue()) {
                                for (var i = 0; i < itemStore.getCount(); i++) {
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
        tbar: [
         {
             text: UPDATE_SITE_PRICE,
             iconCls: 'icon-edit',
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
         }
        ],
        columns: [
            { colName: 'site_name', header: SITE_NAME, dataIndex: 'site_name', width: 130, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'product_name', header: PRODUCT_NAME, dataIndex: 'product_name', width: 180, align: 'left', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'user_level', header: USER_LEVEL, dataIndex: 'user_level_name', width: 100, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'user_id', header: USER_EMAIL, dataIndex: 'user_email', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'price_status', header: PRICE_STATUS, dataIndex: 'status', width: 100, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'item_money', header: ITEM_MONEY, dataIndex: 'item_money', width: 100, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'item_cost', header: ITEM_COST, dataIndex: 'item_cost', width: 100, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'event_item_money', header: ITEM_EVENT_MONEY, dataIndex: 'event_money', width: 100, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'event_item_cost', header: ITEM_EVENT_COST, dataIndex: 'event_cost', width: 100, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'event_start', header: EVENT_TIME, xtype: 'templatecolumn',
                tpl: Ext.create('Ext.XTemplate',
                    '{[values.event_start==0?"":Ext.Date.format(new Date(values.event_start * 1000),"Y/m/d H:i:s")]}',
                    '~',
                    '{[values.event_end==0?"":Ext.Date.format(new Date(values.event_end * 1000),"Y/m/d H:i:s")]}'
                ),
                width: 220, align: 'left', sortable: false, menuDisabled: true, hidden: false
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
            { colName: 'spec_id_1', header: ITEM_SPEC1, dataIndex: 'Spec_Name_1', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'spec_id_2', header: ITEM_SPEC2, dataIndex: 'Spec_Name_2', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'item_code', header: ITEM_CODE, dataIndex: 'Item_Code', width: 180, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'item_money', header: ITEM_MONEY, dataIndex: 'Item_Money', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'item_cost', header: ITEM_COST, dataIndex: 'Item_Cost', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'event_item_money', header: ITEM_EVENT_MONEY, dataIndex: 'Event_Item_Money', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'event_item_cost', header: ITEM_EVENT_COST, dataIndex: 'Event_Item_Cost', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: false }
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
        hidden: ISEDIT,
        items: [frm, grid],
        listeners: {
            afterrender: function () {
                window.parent.updateAuth(tempPanel, 'colName');
            },
            show: function () {
                tempPanel.setWidth(960);
            }
        }
    });
    //修改時呈現Panel
    var showPanel = Ext.create('Ext.panel.Panel', {
        id: 'showPanel',
        layout: 'anchor',
        border: false,
        hidden: !ISEDIT,
        items: [{
            xtype: 'form',
            id: 'profrm',
            padding: '5 5',
            border: 0,
            items: !ISEDIT ? [] : defaultPrice,
            buttonAlign: 'center',
            buttons: [{ text: SAVE, id: 'savePrice', handler: function () { savePrice(); } }]
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
    if (ISEDIT) {
        Ext.getCmp('showPanel').down('#product_price_list').setValue(result.Product_Price_List);
        Ext.getCmp('showPanel').down('#show_listprice').setValue(result.show_listprice);
        Ext.getCmp('showPanel').down('#bag_check_money').setValue(result.Bag_Check_Money);
        //product_mode:2(寄倉),不為寄倉則不能設置寄倉費
        Ext.getCmp('showPanel').down('#bag_check_money').setReadOnly(result.Product_Mode != 2);
        siteProductStore.load({
            params: {
                ProductId: PRODUCT_ID,
                IsEdit: ISEDIT
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
        Ext.getCmp('frm').down('#product_price_list').setValue(result.Product_Price_List);
        Ext.getCmp('frm').down('#show_listprice').setValue(result.show_listprice);
        Ext.getCmp('frm').down('#bag_check_money').setValue(result.Bag_Check_Money);
        //product_mode:2(寄倉),不為寄倉則不能設置寄倉費
        Ext.getCmp('frm').down('#bag_check_money').setReadOnly(result.Product_Mode != 2);
        Ext.getCmp('grid').query('*[colName=event_money]')[0].hide();
        Ext.getCmp('grid').query('*[colName=event_cost]')[0].hide();
        Ext.Ajax.request({
            url: '/VendorProduct/GetPriceMaster',
            method: 'post',
            params: {
                ProductId: PRODUCT_ID,
                OldProductId: OLD_PRODUCT_ID,
                IsEdit: ISEDIT
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result) {
                    if (!NO_SPEC) Ext.getCmp('tempPanel').query('*[colName=same_price]')[0].setValue(result.same_price);
                    Ext.getCmp('tempPanel').down('#product_name').setValue(result.product_name);
                    var time;
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
function savePrice() {
    var frm = Ext.getCmp('showPanel').down('#profrm');
    if (frm.getForm().isValid()) {
        var values = frm.getForm().getValues(true) + "&ProductId=" + PRODUCT_ID;
        Ext.Ajax.request({
            url: '/VendorProduct/UpdatePrice',
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
//保存數據至數據庫
function save() {
    cellEditing.completeEdit();
    var success = false;
    var frm = Ext.getCmp('frm');
    if (frm.getForm().isValid()) {
        var dates = frm.query('datetimefield');
        var event_null = true;
        if (dates[2].getValue() && dates[3].getValue()) {
            event_null = false;
        }
        var itemPrices = "[";
        for (var i = 0; i < itemStore.getCount(); i++) {
            var record = itemStore.getAt(i);
            if (!record.get('Item_Money')) {
                window.parent.setMoveEnable(true);
                Ext.Msg.alert(INFORMATION, ITEM_MONEY_EMPTY);
                return false;
            }
            //edit by shuangshuang0420j 20141008
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



        PRODUCT_ID = window.parent.GetProductId();
        var product_name = frm.down('#product_name') ? Ext.htmlEncode(frm.down('#product_name').getValue()) : '';
        var values = Ext.Object.fromQueryString(frm.getForm().getValues(true) + "&ProductId=" + PRODUCT_ID + "&OldProductId=" + OLD_PRODUCT_ID);
        values.Items = itemPrices;
        values.product_name = product_name;

        Ext.Ajax.request({
            url: '/VendorProduct/SaveItemPrice',
            method: 'POST',
            params: values,
            async: false,
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    if (NEW_SITE) {
                        siteProductStore.load({
                            params: {
                                ProductId: PRODUCT_ID,
                                IsEdit: ISEDIT
                            }
                        });
                        itemStore.load({ params: { ProductId: PRODUCT_ID, OldProductId: OLD_PRODUCT_ID} });
                        Ext.getCmp('tempPanel').hide();
                        Ext.getCmp('showPanel').show();
                        NEW_SITE = false;
                    }
                    success = true;
                }
                else {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                    window.parent.setMoveEnable(true);
                }
            },
            failure: function (form, action) {
                Ext.Msg.alert(INFORMATION, FAILURE);
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
        items: [
                {
                    xtype: 'textfield',
                    name: 'price_master_id',
                    hidden: true
                },

                {
                    anchor: '60%',
                    xtype: 'textfield',
                    fieldLabel: PRODUCT_NAME + NOT_EMPTY,
                    hidden: false,
                    allowBlank: false,
                    //submitValue: false,
                    colName: 'product_name',
                    name: 'product_name'
                },
                {
                    xtype: 'fieldcontainer',
                    colName: 'event_start',
                    hidden: false,
                    layout: 'hbox',
                    fieldLabel: EVENT_TIME,
                    items: [
                            {
                                xtype: 'datetimefield',
                                disabledMin: true,
                                disabledSec: true,
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
                            },
            {
                xtype: 'displayfield',
                value: '~'
            },
             {
                 xtype: 'datetimefield',
                 disabledMin: true,
                 disabledSec: true,
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
             }]
                },
                {
                    xtype: 'fieldcontainer',
                    layout: 'hbox',
                    id: 'priceValidTime',
                    colName: 'priceValidTime',
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
                    }]
                },
                {
                    xtype: 'checkbox',
                    hidden: false,
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
                                    for (var i = 1; i < sitePriceStore.getCount(); i++) {
                                        sitePriceStore.getAt(i).set('item_money', money);
                                        sitePriceStore.getAt(i).set('item_cost', cost);
                                        sitePriceStore.getAt(i).set('event_cost', ecost);
                                        sitePriceStore.getAt(i).set('event_money', emoney);
                                    }
                                }
                            }
                        }
                    }
                }
        ]
    });
    var wgrid = Ext.create('Ext.grid.Panel', {
        id: 'wgrid',
        store: sitePriceStore,
        plugins: [cellEditing_wfrm],
        width: 780,
        height: 270,
        buttons: ['->', { text: SAVE, id: 'siteUpdate', handler: function () { update(); } }, { text: CANCEL, handler: function () { Ext.getCmp('showPanel').show(); Ext.getCmp('win').destroy(); } }],
        columns: [
            { xtype: 'rownumberer', width: 50, align: 'center' },
            { colName: 'spec_id_1', header: ITEM_SPEC1, dataIndex: 'spec_name_1', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'spec_id_2', header: ITEM_SPEC2, dataIndex: 'spec_name_2', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'item_money', header: ITEM_MONEY + NOT_EMPTY, dataIndex: 'item_money', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: false,
                editor: {
                    xtype: 'numberfield',
                    decimalPrecision: 0,
                    minValue: 0,
                    allowBlank: false,
                    listeners: {
                        blur: function () {
                            if (wfrm.query('*[colName=same_price]')[0].getValue()) {
                                for (var i = 0; i < sitePriceStore.getCount(); i++) {
                                    sitePriceStore.getAt(i).set('item_money', this.getValue());
                                }
                            }
                        }
                    }
                }
            },
            { colName: 'item_cost', header: ITEM_COST + NOT_EMPTY, dataIndex: 'item_cost', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: false,
                editor: {
                    xtype: 'numberfield',
                    decimalPrecision: 0,
                    minValue: 0,
                    allowBlank: false,
                    listeners: {
                        blur: function () {
                            if (wfrm.query('*[colName=same_price]')[0].getValue()) {
                                for (var i = 0; i < sitePriceStore.getCount(); i++) {
                                    sitePriceStore.getAt(i).set('item_cost', this.getValue());
                                }
                            }
                        }
                    }
                }
            },
            { colName: 'event_money', header: ITEM_EVENT_MONEY, dataIndex: 'event_money', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: false,
                editor: {
                    xtype: 'numberfield',
                    decimalPrecision: 0,
                    minValue: 0,
                    listeners: {
                        blur: function () {
                            if (wfrm.query('*[colName=same_price]')[0].getValue()) {
                                for (var i = 0; i < sitePriceStore.getCount(); i++) {
                                    sitePriceStore.getAt(i).set('event_money', this.getValue());
                                }
                            }
                        }
                    }
                }
            },
            { colName: 'event_cost', header: ITEM_EVENT_COST, dataIndex: 'event_cost', width: 120, align: 'center', sortable: false, menuDisabled: true, hidden: false,
                editor: {
                    xtype: 'numberfield',
                    decimalPrecision: 0,
                    minValue: 0,
                    listeners: {
                        blur: function () {
                            if (wfrm.query('*[colName=same_price]')[0].getValue()) {
                                for (var i = 0; i < sitePriceStore.getCount(); i++) {
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
        width: 800,
        height: 520,
        layout: 'anchor',
        labelWidth: 120,
        closeAction: 'destroy',
        border: false,
        renderTo: Ext.getBody(),
        bodyStyle: 'padding:5px 5px 5px 5px',
        listeners: {
            "afterrender": function () {

                window.parent.updateAuth(wfrm, 'colName');
                window.parent.updateAuth(wgrid, 'colName');
                if (row) {
                    wfrm.getForm().loadRecord(row);
                    var time;
                    if (row.data.event_start != 0) {
                        time = new Date(row.data.event_start * 1000);
                        wfrm.query('*[name=event_start]')[0].setValue(Ext.Date.format(time, "Y-m-d H:i:s"));
                    }
                    if (row.data.event_end != 0) {
                        time = new Date(row.data.event_end * 1000);
                        wfrm.query('*[name=event_end]')[0].setValue(Ext.Date.format(time, "Y-m-d H:i:s"));
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
                }
            }
        }
    });       //.show();

    //  Ext.getCmp('user_id_modify').setValue(row.data.user_email);
}

function update() {

    var frm = Ext.getCmp('wfrm');
    if (frm.getForm().isValid()) {
        var dates = frm.query('datetimefield');
        var event_null = true;
        if (dates[2].getValue() && dates[3].getValue()) {
            event_null = false;
        }

        var itemPrices = "[";
        for (var i = 0; i < sitePriceStore.getCount(); i++) {
            var record = sitePriceStore.getAt(i);
            if (!record.get('item_money')) {
                Ext.Msg.alert(INFORMATION, ITEM_MONEY_EMPTY);
                return false;
            }
            if (!event_null) {
                if (!record.get('event_money') || !record.get('event_cost')) {
                    Ext.Msg.alert(INFORMATION, EVENT_MONEY_EMPTY);
                    return false;
                }
            }
            itemPrices += "{item_price_id:\"" + record.get('item_price_id') + "\",item_id:\"" + record.get('item_id')
                        + "\",price_master_id:\"" + record.get('price_master_id') + "\",item_money:\"" + record.get('item_money')
                        + "\",item_cost:\"" + record.get('item_cost') + "\",event_money:\"" + record.get('event_money') + "\",event_cost:\""
                        + record.get('event_cost') + "\"}";
        }
        itemPrices += "]";
        itemPrices = itemPrices.replace(/}{/g, '},{');


        PRODUCT_ID = window.parent.GetProductId();
        var values = Ext.Object.fromQueryString(frm.getForm().getValues(true) + "&Items=" + itemPrices);
        Ext.Ajax.request({
            url: '/VendorProduct/UpdateItemPrice',
            method: 'POST',
            params: values,
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    Ext.getCmp('showPanel').show(); Ext.getCmp('win').destroy();
                    siteProductStore.load({
                        params: {
                            ProductId: PRODUCT_ID,
                            IsEdit: ISEDIT
                        },
                        callback: function (records, operation, success) {
                            if (ISEDIT) {
                                var record = siteProductStore.findRecord('site_id', 1);
                                if (record) {
                                    Ext.getCmp('showPanel').query('*[colName=same_price]')[0].setValue(record.data.same_price);
                                }
                            }
                        }
                    });
                    itemStore.load({ params: { ProductId: PRODUCT_ID} });
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
}
