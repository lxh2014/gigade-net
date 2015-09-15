var leftW = 250; //左側樹狀結構的寬度
var rigW = 560; //右側的寬度
var theight = 620; //窗口的高度
var topheight = 170; //窗口的高度
var pageSize = 25;
var ac_store;
var mainTainProduCateID = "0"; //活動類別ID
var siteid = 1;
var promationWin;
var boolClass = false;
var centerNorthPan;
var p_store;

Ext.onReady(function () {
    var treeStore = Ext.create('Ext.data.TreeStore', {
        proxy: {
            type: 'ajax',
            url: '/PromotionsMaintain/GetCatagory',
            noCache: false,
            getMethod: function () { return 'get'; },
            actionMethods: 'post'
        },
        root: {
            text: PROMOITEMS,
            expanded: true,
            children: [
            ]
        } 
    });
    treeStore.load();

    var treePanel = new Ext.tree.TreePanel({
        id: 'treePanel',
        //title: '树',
        region: 'west',
        width: leftW,
        border: 0,
        height: theight,
        store: treeStore

    });

    Ext.define("gigade.Brand", {
        extend: 'Ext.data.Model',
        fields: [
        { name: "brand_id", type: "string" },
        { name: "brand_name", type: "string" }]
    });

    //品牌store
    var classBrandStore = Ext.create('Ext.data.Store', {
        model: 'gigade.Brand',
        autoLoad: false,
        autoDestroy: true, //自動銷毀
        remoteSort: false,
        proxy: {
            type: 'ajax',
            url: "/PromotionsMaintain/QueryClassBrand",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'item'
            }
        }
    });

    classBrandStore.on('beforeload', function () {
        Ext.apply(classBrandStore.proxy.extraParams, {
            topValue: Ext.htmlEncode(Ext.getCmp("shopClass_id").getValue())
        });
    });
    ShopClassStoreYuan.load();

    //前台分類store
    var frontCateStore = Ext.create('Ext.data.TreeStore', {
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: '/ProductList/GetFrontCatagory',
            actionMethods: 'post'
        },
        root: {
            expanded: true,
            checked: false,
            children: [

            ]
        }
    });
    frontCateStore.load();
    centerNorthPan = new Ext.form.FormPanel({
        //title: '搜索框',
        region: 'north',
        width: rigW,
        height: topheight,
        labelAlign: 'right',
        buttonAlign: 'right',
        padding: 10,
        border: 0,
        items: [
            {
                layout: 'column',
                border: false,
                labelSeparator: ':',
                items: [
                    {
                        xtype: 'combobox', //class_id
                        allowBlank: true,
                        fieldLabel: SHOPCLASS,
                        editable: false,
                        id: 'shopClass_id',
                        labelWidth: 100,
                        name: 'class_name',
                        hiddenName: 'shopClass_id',
                        colName: 'class_name',
                        store: ShopClassStoreYuan,
                        displayField: 'class_name',
                        valueField: 'class_id',
                        typeAhead: true,
                        forceSelection: false,
                        emptyText: SELECT,
                        listeners: {
                            "select": function (combo, record) {
                                var z = Ext.getCmp("clsssBrand_id");
                                var c = Ext.getCmp("comboFrontCage");
                                var ch = Ext.getCmp("comboFrontCage_hide");
                                z.clearValue();
                                z.setDisabled(false);
                                c.setValue("");
                                ch.setValue("");
                                classBrandStore.removeAll();
                                boolClass = true;
                                z.setDisabled(false);

                            }
                        }
                    }
                ]
            },
            {
                layout: 'column',
                border: false,
                items: [
                  {
                      xtype: 'combobox', //banner_id
                      allowBlank: true,
                      fieldLabel: BLANDNAME,
                      editable: true,
                      forceSelection: true,
                      queryMode: 'local',
                      hidden: false,
                      id: 'clsssBrand_id',
                      labelWidth: 100,
                      name: 'clsssbrand_name',
                      disabled: true,
                      hiddenname: 'brand_id',
                      store: classBrandStore,
                      displayField: 'brand_name',
                      valueField: 'brand_id',
                      typeAhead: true,
                      forceSelection: false,
                      emptyText: SELECT,
                      listeners: {
                          beforequery: function (qe) {
                              if (boolClass) {
                                  delete qe.combo.lastQuery;
                                  classBrandStore.load({
                                      params: {
                                          topValue: Ext.getCmp("shopClass_id").getValue()
                                      }
                                  });
                                  boolClass = false;
                              }
                          },
                          'blur': function () {
                              if (Ext.getCmp('clsssBrand_id').getValue() != null && Ext.getCmp('clsssBrand_id').getValue() != "") {
                                  var o = classBrandStore.data.items;
                                  if (document.getElementsByName('clsssbrand_name')[0].value != Ext.getCmp('clsssBrand_id').getValue()) {
                                      document.getElementsByName('clsssbrand_name')[0].value = classBrandStore.getAt(0).get('brand_id');
                                  }
                              }
                          }
                      }
                  }
                ]
            },
            {
                layout: 'column',
                border: false,
                items: [
                    {
                        xtype: 'combotree',
                        id: 'comboFrontCage',
                        name: 'classname',
                        hiddenname: 'classname',
                        editable: false,
                        submitValue: false,
                        colName: 'category_id',
                        store: frontCateStore,
                        fieldLabel: CLASSNAME,
                        labelWidth: 100,
                        columnWidth: .9

                    },
                    {
                        hidden: true,
                        xtype: 'textfield',
                        id: 'comboFrontCage_hide',
                        name: 'comboFrontCage_hide',
                        listeners: {
                            change: function (txt, newValue, oldValue) {
                                if (newValue) {
                                    var z = Ext.getCmp("clsssBrand_id");
                                    var sc = Ext.getCmp("shopClass_id");
                                    z.clearValue();
                                    sc.clearValue();
                                    z.setDisabled(true);
                                }
                            }
                        }
                    }
                ]
            },
            {
                layout: 'column',
                border: false,
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: '查詢所有商品',
                        id: 'key',
                        name: 'key',
                        regex: /^(\d+)([,，|]{1}\d+)*(\d+)*$/,
                        regexText: '輸入格式不符合要求',//add by sdy
                        labelWidth: 100,
                        columnWidth: .7
                    },
                    {
                        xtype: 'button',
                        fieldLabel: PRODVISIT,
                        name: 'visitdate',
                        text: SEARCH,
                        margin: '0 0 0 10',
                        columnWidth: .2,
                        handler: Search
                    }
                ]
            },
            {
                layout: 'column',
                border: false,
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: '查詢活動商品',
                        id: 'keyactivy',
                        name: 'keyactivy',
                        regex: /^(\d+)([,，|]{1}\d+)*(\d+)*$/,
                        regexText: '輸入格式不符合要求',//add by sdy 
                        labelWidth: 100,
                        columnWidth: .7
                    },
                    {
                        xtype: 'button',
                        fieldLabel: PRODVISIT,
                        name: 'visitdate',
                        text: SEARCH,
                        margin: '0 0 0 10',
                        columnWidth: .2,
                        handler: SearchActivy
                    }
                ]
            }
        ]
    });

    var pro = [
        { name: 'id', type: 'int' },     
        { name: 'product_id', type: 'string' },
        { name: 'brand_id', type: 'string' },
        { name: 'brand_name', type: 'string' },
        { name: 'product_name', type: 'string' },
        { name: 'product_price_list', type: 'int' },
        { name: 'price', type: 'int' },
        { name: 'cost', type: 'int' },
        { name: 'product_freight_set', type: 'string' }
    ];
    var pro2 = [
        { name: 'id', type: 'int' },
        { name: 'product_id', type: 'string' },
        { name: 'brand_id', type: 'string' },
        { name: 'brand_name', type: 'string' },
        { name: 'product_name', type: 'string' },
        { name: 'product_price_list', type: 'int' },
        { name: 'price', type: 'int' },
        { name: 'cost', type: 'int' },
        { name: 'product_freight_set', type: 'string' }
    ];

    Ext.define('GIGADE.PRODUCT', {
        extend: 'Ext.data.Model',
        fields: pro
    });
    Ext.define('GIGADE.PRODUCT2', {
        extend: 'Ext.data.Model',
        fields: pro2
    });
    p_store = Ext.create('Ext.data.Store', {
        pageSize: pageSize,
        model: 'GIGADE.PRODUCT',
        proxy: {
            type: 'ajax',
            url: '/PromotionsMaintain/QueryProList',
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'item',
                totalProperty: 'totalCount'
            }
        }
    });

    ac_store = Ext.create('Ext.data.Store', {
        pageSize: pageSize,
        model: 'GIGADE.PRODUCT2',
        autoload: true,
        proxy: {
            type: 'ajax',
            url: '/PromotionsMaintain/GetProductByCategorySet',
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'item',
                totalProperty: 'totalCount'
            }
        }
    });

    ac_store.on("beforeload", function () {
        Ext.apply(ac_store.proxy.extraParams, {
            key: Ext.getCmp('keyactivy').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('keyactivy').getValue()),
            producateid: Ext.htmlEncode(mainTainProduCateID),
            site_id: Ext.htmlEncode(siteid)
        });
    });
    p_store.on("beforeload", function () {
        Ext.apply(p_store.proxy.extraParams, {
            key: Ext.htmlEncode(Ext.getCmp('key').getValue()),
            site_id: Ext.htmlEncode(siteid),
            ProCatid: Ext.htmlEncode(mainTainProduCateID), //clsssBrand_id
            brand_id: document.getElementsByName('clsssbrand_name').length == 0 ? '' : Ext.htmlEncode(document.getElementsByName('clsssbrand_name')[0].value),
            class_id: Ext.getCmp('shopClass_id').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('shopClass_id').getValue()),
            comboFrontCage_hide: Ext.getCmp('comboFrontCage_hide').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('comboFrontCage_hide').getValue())
        });
    });


    function SearchActivy() {
        ac_store.removeAll();
        ac_store.load({
            params: {
                key: Ext.getCmp('keyactivy').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('keyactivy').getValue()),
                ProCatid: Ext.htmlEncode(mainTainProduCateID),
                site_id: Ext.htmlEncode(siteid)
            }
        });
    }
    function Search() {
        p_store.removeAll();
        p_store.load({
            callback: function () {
                if (p_store.data.items.length == 0) {
                    Ext.Msg.alert(INFORMATION, "沒有符合條件的數據");
                }
            }
        });
    }

    var t_search_cm = Ext.create('Ext.selection.CheckboxModel', {
        listeners: {
            selectionchange: function (sm, selections) {
                Ext.getCmp("searchGrid").down('#t_add').setDisabled(selections.length == 0);
            }
        }
    });
    var t_activity_cm = Ext.create('Ext.selection.CheckboxModel', {
        listeners: {
            selectionchange: function (sm, selections) {
                Ext.getCmp("activityGrid").down('#t_remove').setDisabled(selections.length == 0);
            }
        }
    });

    var searchGrid = new Ext.grid.Panel({
        id: 'searchGrid',
        store: p_store,
        region: 'center',
        autoScroll: true,
        border: 0,
        height: 390,
        viewConfig: {
            enableTextSelection: true,
            stripeRows: false,
            getRowClass: function (record, rowIndex, rowParams, store) {
                return "x-selectable";
            }
        },
        columns: [
            { header: PRODID, dataIndex: 'product_id', width: 60 },
            { header: BLANDNAME, dataIndex: 'brand_name' },
            { header: '品牌id', dataIndex: 'brand_id', hidden: true },
            { header: PRODNAME, dataIndex: 'product_name' },
            { header: DELIVERYSTORE, dataIndex: 'product_freight_set' },
            {
                header: '商品金額', dataIndex: 'price',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 0) {
                        return "<span style='color:Red;'>" + value + "</span>";
                    } else {
                        return value;
                    }
                }
            }, //商品金額為0時，紅色顯示
            { header: '成本', dataIndex: 'cost' }
        ],
        tbar: [
            { xtype: 'button', id: 't_add', text: ADDTOACTIVE, iconCls: 'icon-add', disabled: true, handler: function () { onToolAddClick(); } },
        ],
        selModel: t_search_cm,
        bbar: Ext.create('Ext.PagingToolbar', {
            store: p_store,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        })

    });

    var activityGrid = new Ext.grid.Panel({
        id: 'activityGrid',
        store: ac_store,
        region: 'center',
        autoScroll: true,
        autoScroll: true,
        border: 0,
        height: 390,
        viewConfig: {
            enableTextSelection: true,
            stripeRows: false,
            getRowClass: function (record, rowIndex, rowParams, store) {
                return "x-selectable";
            }
        },
        columns: [
            { header: PRODID, dataIndex: 'product_id', width: 60 },
            { header: BLANDNAME, dataIndex: 'brand_name' },
            { header: '品牌id', dataIndex: 'brand_id', hidden: true },
            { header: PRODNAME, dataIndex: 'product_name' },
            { header: DELIVERYSTORE, dataIndex: 'product_freight_set' },
            { header: '商品金額', dataIndex: 'price' },
            { header: '成本', dataIndex: 'cost' }
        ],
        tbar: [
            { xtype: 'button', id: 't_remove', text: REMOVE, iconCls: 'icon-remove', disabled: true, handler: function () { onToolRemoveClick(); } }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ac_store,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        selModel: t_activity_cm,
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
        
    });

    var centerSouthPan = new Ext.tab.Panel({
        region: 'center',
        autoScroll: true,
        items: [
            {
                title: SEARCHPRODS,
                items: [searchGrid]
            }, {
                title: ACTIVEPRODS,
                items: [activityGrid]
            }
        ]
    });

    var centerPanel = new Ext.Panel({
        region: 'center',
        items: [centerNorthPan, centerSouthPan]
    }); //頁面佈局

    promationWin = new Ext.Window({
        title: PROMOITEMMATION,
        height: theight,
        width: leftW + rigW,
        layout: 'border',
        modal: true,
        constrain: true,
        closeAction: 'hide', //hide
        items: [
            treePanel,
            centerPanel
        ]
    });
});

function PromationMationShow(categoryid, websiteid) {

    mainTainProduCateID = categoryid;
    siteid = websiteid == "" ? 1 : websiteid;
    p_store.removeAll();
    Ext.Ajax.request({
        url: '/PromotionsMaintain/DeleteAllClassProductByModel',
        params: {
            categoryid: mainTainProduCateID
        },
        success: function (response) {
            var text = response.responseText;
            ac_store.load({
                params: {
                    ProCatid: mainTainProduCateID,
                    site_id: siteid
                }
            });
        }
    });
    centerNorthPan.form.reset();
    promationWin.show();
}

function onToolRemoveClick() {
    var row = Ext.getCmp("activityGrid").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                var pids = '';
                var bids = '';
                for (var i = 0; i < row.length; i++) {
                    var product_id_fortemp = row[i].get('product_id');
                    var brand_id_fortemp = row[i].get('brand_id');
                    pids += product_id_fortemp + '|';
                    bids += brand_id_fortemp + '|';
                }
                Ext.Ajax.request({
                    url: '/PromotionsMaintain/DeleteProductFromCategorySet',
                    method: 'post',
                    params: {
                        brandids: bids,
                        categoryid: mainTainProduCateID,
                        productids: pids
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        ac_store.load({ params: { ProCatid: mainTainProduCateID, site_id: siteid } });
                        p_store.load({
                            params: {
                                key: Ext.htmlEncode(Ext.getCmp('key').getValue()),
                                site_id: Ext.htmlEncode(siteid),
                                ProCatid: Ext.htmlEncode(mainTainProduCateID),
                                brand_id: document.getElementsByName('clsssbrand_name').length == 0 ? '' : Ext.htmlEncode(document.getElementsByName('clsssbrand_name')[0].value),
                                class_id: Ext.getCmp('shopClass_id') == '' ? '' : Ext.htmlEncode(Ext.getCmp('shopClass_id').getValue()),
                                comboFrontCage_hide: Ext.getCmp('comboFrontCage_hide') == '' ? '' : Ext.htmlEncode(Ext.getCmp('comboFrontCage_hide').getValue())
                            }
                        });
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }
        });
    }
}
function onToolAddClick() {
    var row = Ext.getCmp("searchGrid").getSelectionModel().getSelection();
    var pids = '';
    var bids = '';
    for (var i = 0; i < row.length; i++) {
        var product_id_fortemp = row[i].get('product_id');
        var brand_id_fortemp = row[i].get('brand_id');
        pids += product_id_fortemp + '|';
        bids += brand_id_fortemp + '|';
        if (row[i].get('price') == 0) {
            Ext.Msg.alert(INFORMATION, SAVECATESETTIP);
            return;
        }
    }

    Ext.Ajax.request({
        url: '/PromotionsMaintain/SaveProductCategorySet',
        params: {
            brandids: bids,
            categoryid: mainTainProduCateID,
            productids: pids
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                Ext.Msg.alert(INFORMATION, SUCCESS);
                ac_store.load({ params: { ProCatid: mainTainProduCateID, site_id: siteid } });
                p_store.load({
                    params: {
                        key: Ext.htmlEncode(Ext.getCmp('key').getValue()),
                        site_id: Ext.htmlEncode(siteid),
                        ProCatid: Ext.htmlEncode(mainTainProduCateID),
                        brand_id: document.getElementsByName('clsssbrand_name').length == 0 ? '' : Ext.htmlEncode(document.getElementsByName('clsssbrand_name')[0].value),
                        class_id: Ext.getCmp('shopClass_id') == '' ? '' : Ext.htmlEncode(Ext.getCmp('shopClass_id').getValue()),
                        comboFrontCage_hide: Ext.getCmp('comboFrontCage_hide') == '' ? '' : Ext.htmlEncode(Ext.getCmp('comboFrontCage_hide').getValue())
                    }
                });

            }
            else {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        },
        failure: function () {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }

    });

}

