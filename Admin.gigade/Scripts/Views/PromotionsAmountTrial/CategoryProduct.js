var titleName = CATEGORYPROD;
var leftW = 250; //左側樹狀結構的寬度
var conW = 300; //右側的寬度
var topheight = 165; //窗口的高度
var pageSize = 500;
var leftW = 250; //左側樹狀結構的寬度
var rigW = 540; //右側的寬度
var theight = 620; //窗口的高度
var s_store;
var promationWin;
var boolClass = false;
var centerNorthPan;
var isSearchProduct = 0;//是否進行搜尋操作 >0的時候進行搜尋
var MainCategoryId = "";    //商品類別編號
var MainCategoryName = "";  //商品類別名稱
var treePanel;
var treeStore;
var centerPanel;
var searchGrid;
var siteID = 1;
Ext.onReady(function () {
    var productPro = [
    { name: 'product_id', type: 'int' },
    { name: 'brand_id', type: 'string' },
    { name: 'category_id', type: 'string' },
    { name: 'category_name', type: 'string' },
    { name: 'brand_name', type: 'string' },
    { name: 'product_name', type: 'string' },
    { name: 'product_price_list', type: 'int' },
    { name: 'product_status', type: 'string' },
    { name: 'product_mode', type: 'string' },
    { name: 'combination', type: 'string' },
    { name: 'product_freight_set', type: 'string' },
    { name: 'product_image', type: 'string' }
    ];


    Ext.define('GIGADE.searchProduct', {
        extend: 'Ext.data.Model',
        fields: productPro
    });



    //館別Model
    Ext.define("gigade.shopClass", {
        extend: 'Ext.data.Model',
        fields: [
        { name: "class_id", type: "string" },
        { name: "class_name", type: "string" }]
    });
    //獲取館別
    var shopClassStore = Ext.create('Ext.data.Store', {
        model: 'gigade.shopClass',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/PromotionsMaintain/GetShopClass",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
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
    //獲取左邊的category樹結構(商品分類store)
    treeStore = Ext.create('Ext.data.TreeStore', {
        proxy: {
            type: 'ajax',
            url: '/Product/GetProductCatagory',
            actionMethods: 'post'
        },
        rootVisible: false,
        root: {
            text: '商品類別',
            id: '0',
            expanded: true,
            children: []
        }
    });
    treeStore.load();

    //treeStore.load({ params: { ProductId: 11111, OldProductId: 11111} });
    treePanel = new Ext.tree.TreePanel({
        id: 'treePanel',
        region: 'west',
        width: leftW,
        border: 0,
        height: theight,
        store: treeStore,
        listeners: {
            'itemclick': function (view, record, item, index, e) {
                nodeId = record.raw.id; //获取点击的节点id
                nodeText = record.raw.text; //获取点击的节点text
                MainCategoryId = nodeId;
                MainCategoryName = nodeText;
                Ext.getCmp('categoryId').setValue(MainCategoryId);
                Ext.getCmp('categoryName').setValue(MainCategoryName);
                SearchActivy();
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

    centerNorthPan = new Ext.form.FormPanel({
        region: 'north',
        id: 'frm',
        width: rigW,
        height: topheight,
        labelAlign: 'right',
        buttonAlign: 'center',
        padding: '5 5',
        defaults: { anchor: '95%', msgTarget: "side" },
        border: 0,
        layout: 'anchor',
        items: [
        {
            xtype: 'panel',
            border: 0,
            layout: 'column',
            items: [
            {
                xtype: 'fieldset',
                title: CLASSBRAND,
                items: [
                {
                    xtype: 'combobox', //class_id
                    allowBlank: true,
                    fieldLabel: SHOPCLASS,
                    editable: false,
                    id: 'shopClass_id',
                    width: 200,
                    labelWidth: 40,
                    name: 'class_name',
                    hiddenName: 'shopClass_id',
                    colName: 'class_name',
                    store: shopClassStore,
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
                },
                {
                    xtype: 'combobox', //banner_id
                    allowBlank: true,
                    fieldLabel: BLANDNAME,
                    editable: true,
                    forceSelection: true,
                    id: 'clsssBrand_id',
                    width: 200,
                    labelWidth: 40,
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
                            var o = classBrandStore.data.items;
                            if (document.getElementsByName('clsssbrand_name')[0].value != Ext.getCmp('clsssBrand_id').getValue()) {
                                document.getElementsByName('clsssbrand_name')[0].value = classBrandStore.getAt(0).get('brand_id');
                            }
                        }
                    }
                }
                ]
            },
            {
                xtype: 'panel',
                border: 0,
                columnWidth: .99,
                layout: 'anchor',
                padding: '5 0 5 15',
                items: [
                {
                    xtype: 'fieldcontainer',
                    defaults: {
                        labelWidth: 60,
                        width: 100
                    },
                    id: '3s',
                    combineErrors: true,
                    layout: 'hbox',
                    items: [
                    {
                        xtype: 'combotree',
                        id: 'comboFrontCage',
                        name: 'classname',
                        hiddenname: 'classname',
                        hidden: true,
                        editable: false,
                        submitValue: false,
                        colName: 'category_id',
                        store: frontCateStore,
                        fieldLabel: CLASSNAME,
                        height: 200,
                        labelWidth: 60

                    },
                    {
                        hidden: true,
                        xtype: 'textfield',
                        id: 'comboFrontCage_hide',
                        name: 'comboFrontCage_hide',
                        width: 10,
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
                    },
                    {
                        xtype: 'displayfield',
                        fieldLabel: CATEGORYID,
                        id: 'categoryId',
                        name: 'categoryId'
                    },
                    {
                        xtype: 'displayfield',
                        fieldLabel: CATEGORYNAME,
                        width: 160,
                        id: 'categoryName',
                        name: 'categoryName'
                    }
                    ]
                },
                {
                    xtype: 'combobox', //status
                    allowBlank: true,
                    fieldLabel: PRODSTATUS,
                    editable: true,
                    forceSelection: true,
                    id: 's_status',
                    width: 240,
                    labelWidth: 80,
                    name: 's_status',
                    hiddenname: 's_status',
                    lastQuery: '',
                    value: "5",
                    store: statusStore,
                    displayField: 'parameterName',
                    valueField: 'parameterCode',
                    typeAhead: true,
                    forceSelection: false,
                    emptyText: SELECT
                },
                {
                    xtype: 'textfield',
                    fieldLabel: KEY,
                    id: 'keyactivy',
                    name: 'keyactivy',
                    labelWidth: 80,
                    width: 240
                }

                ]
            }
            ]
        },
             {
                 xtype: 'fieldcontainer',
                 defaults: {
                     labelWidth: 60
                 },

                 id: 'prod',
                 combineErrors: true,
                 layout: 'hbox',
                 items: [
           {
               xtype: 'displayfield',
               fieldLabel: "活動商品",
               id: 'prodid',
               width: 250,
               height: 40,
               name: 'prodid'
           },
           {
               xtype: 'displayfield',
               fieldLabel: "販售商品",
               width: 250,
               height: 40,
               id: 'saleprodid',
               name: 'saleprodid'
           }
                 ]
             }

        ]
          ,
        buttons: [{
            text: SEARCH,
            id: 'btn_search',
            handler: SearchActivy,
            iconCls: 'ui-icon ui-icon-search-2'
        }, {
            text: RESET,
            id: 'btn_reset',
            iconCls: 'ui-icon ui-icon-reset',
            listeners: {
                click: function () {
                    Ext.getCmp("shopClass_id").setValue("");
                    Ext.getCmp("clsssBrand_id").setValue("");
                    Ext.getCmp("s_status").setValue("5");
                    Ext.getCmp("keyactivy").setValue("");
                }
            }
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
    s_store = Ext.create('Ext.data.Store', {
        pageSize: pageSize,
        autoLoad: false,
        model: 'GIGADE.searchProduct',
        proxy: {
            type: 'ajax',
            url: '/PromotionsAmountTrial/GetProductByCategorySet',
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'item',
                totalProperty: 'totalCount'
            }
        }
    });


    s_store.on("beforeload", function () {
        Ext.apply(s_store.proxy.extraParams, {
            category_id: MainCategoryId,
            site_id: Ext.htmlEncode(siteID),
            status: Ext.getCmp('s_status').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('s_status').getValue()),
            keyCode: Ext.getCmp('keyactivy').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('keyactivy').getValue()),
            brand_id: Ext.getCmp('clsssBrand_id').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('clsssBrand_id').getValue()),
            class_id: Ext.getCmp('shopClass_id').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('shopClass_id').getValue()),
            comboFrontCage_hide: Ext.getCmp('comboFrontCage_hide').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('comboFrontCage_hide').getValue())

        });
    });


    function SearchActivy() {
        s_store.removeAll();
        s_store.load({
            params: {
                category_id: MainCategoryId,
                site_id: Ext.htmlEncode(siteID),
                status: Ext.getCmp('s_status').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('s_status').getValue()),
                keyCode: Ext.getCmp('keyactivy').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('keyactivy').getValue()),
                brand_id: Ext.getCmp('clsssBrand_id').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('clsssBrand_id').getValue()),
                class_id: Ext.getCmp('shopClass_id').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('shopClass_id').getValue()),
                comboFrontCage_hide: Ext.getCmp('comboFrontCage_hide').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('comboFrontCage_hide').getValue())

            }
        });
    }


    //控制類別中商品的控件  
    var t_search_cm = Ext.create('Ext.selection.CheckboxModel', {
        listeners: {
            selectionchange: function (sm, selections) {
                Ext.getCmp("searchGrid").down('#t_all_save').setDisabled(selections.length == 0);
                Ext.getCmp("searchGrid").down('#t_sale_save').setDisabled(selections.length == 0);
            }
        }
    });
    //顯示類別中商品的grid
    searchGrid = new Ext.grid.Panel({
        id: 'searchGrid',
        store: s_store,
        region: 'center',
        autoScroll: true,
        selModel: t_search_cm,
        autoScroll: true,
        border: 0,
        height: 400,
        viewConfig: {
            enableTextSelection: true,
            stripeRows: false,
            getRowClass: function (record, rowIndex, rowParams, store) {
                return "x-selectable";
            }
        },
        columns: [
        { header: CATEGORYNAME, dataIndex: 'category_name', width: 120, align: 'center' },
        { header: PRODID, dataIndex: 'product_id', width: 60, align: 'center' },
        { header: PRODNAME, dataIndex: 'product_name', width: 180, align: 'center' },

        { header: BLANDNAME, dataIndex: 'brand_name', width: 120, align: 'center' },
        { header: COMBINATION, dataIndex: 'combination', width: 120, align: 'center', hidden: true },
        { header: PRODSTATUS, dataIndex: 'product_status', width: 80, align: 'center', hidden: true },
        { header: DELIVERYSTORE, dataIndex: 'product_freight_set', width: 60, align: 'center', hidden: true },
        { header: PRODMODE, dataIndex: 'product_mode', width: 60, align: 'center', hidden: true },
        { header: "商品圖", dataIndex: 'product_image', width: 60, align: 'center', hidden: true }
        ],
        tbar: [
        { xtype: 'button', id: 't_all_save', text: "確定活動商品", iconCls: 'icon-add', disabled: true, handler: function () { onAddEleCaClick(); } },
        { xtype: 'button', id: 't_sale_save', text: "確定販售商品", iconCls: 'icon-add', disabled: true, handler: function () { onAddSaleProdClick(); } }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: s_store,
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
    });


    centerSouthPan = new Ext.tab.Panel({
        region: 'center',
        autoScroll: true,
        items: [
        {
            title: CATEGORYPROD,
            items: [searchGrid]
        }
        ]
    });

    var centerPanel = new Ext.Panel({
        region: 'center',
        items: [centerNorthPan, centerSouthPan]
    });

    //頁面佈局
    promationWin = new Ext.Window({
        //title: titleName,
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

function CategoryProductShow(websiteID, cate_id, product_id, sale_product_id, sale_product_name) {
    siteID = websiteID == "" ? 1 : websiteID;
    if (cate_id.getValue() != 0) {
        MainCategoryId = cate_id.getValue();
    }
    s_store.removeAll();
    promationWin.show();
    centerNorthPan.form.reset();

    if (product_id.getValue() != 0 && product_id.getValue() != '' && product_id.getValue() != null) {
        Ext.getCmp("saleprodid").setValue(sale_product_id.getValue() + "|" + sale_product_name.getValue());
        if (MainCategoryId != '0' && MainCategoryId != '') {
            var record = treePanel.getStore().getNodeById(MainCategoryId);
            treePanel.getSelectionModel().select(record);

            Ext.getCmp('categoryId').setValue(record.raw.id);
            Ext.getCmp('categoryName').setValue(record.raw.text);
            s_store.load({
                params: {
                    category_id: MainCategoryId,
                    site_id: Ext.htmlEncode(siteID),
                    status: Ext.getCmp('s_status').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('s_status').getValue()),
                    keyCode: Ext.getCmp('keyactivy').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('keyactivy').getValue()),
                    brand_id: Ext.getCmp('clsssBrand_id').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('clsssBrand_id').getValue()),
                    class_id: Ext.getCmp('shopClass_id').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('shopClass_id').getValue()),
                    comboFrontCage_hide: Ext.getCmp('comboFrontCage_hide').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('comboFrontCage_hide').getValue())

                }, callback: function () {
                    var item = s_store.getAt(s_store.find("product_id", product_id.getValue()));//行對象獲取
                    Ext.getCmp('searchGrid').getSelectionModel().select(item);

                    Ext.getCmp("prodid").setValue(product_id.getValue() + item.data.product_name);
                }
            });
            centerSouthPan.setActiveTab(0);

        }
    } else {
        var record = treePanel.getStore().getNodeById(0);
        treePanel.getSelectionModel().select(record);
    }
}

function onAddEleCaClick() {

    var row = Ext.getCmp('searchGrid').getSelectionModel().getSelection();

    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {

        Ext.getCmp('product_id').setValue(row[0].data.product_id);
        Ext.getCmp("prodid").setValue(row[0].data.product_id + "|" + row[0].data.product_name);

        Ext.getCmp('category_id').setValue(row[0].data.category_id);
        Ext.getCmp('category_name').show();
        Ext.getCmp('category_name').setValue(row[0].data.category_name);

        Ext.getCmp('brand_id').setValue(row[0].data.brand_id);
        Ext.getCmp('brand_name').show();
        Ext.getCmp('brand_name').setValue(row[0].data.brand_name);
        Ext.getCmp('product_name').setValue(row[0].data.product_name);
        var filename = row[0].data.product_image.substring(row[0].data.product_image.lastIndexOf("/") + 1, row[0].data.product_image.length);
        //    alert(filename)
        if (filename != "nopic_150.jpg") {
            Ext.getCmp("product_img").setRawValue(filename);
        } else {
            Ext.getCmp("product_img").setRawValue("nopic_150.jpg");
        }
        Ext.getCmp("prod_file").setValue(row[0].data.product_image);
        //promationWin.close();
    }
}
function onAddSaleProdClick() {

    var row = Ext.getCmp('searchGrid').getSelectionModel().getSelection();

    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        Ext.getCmp('sale_product_id').setValue(row[0].data.product_id);
        Ext.getCmp('sale_product_name').setValue(row[0].data.product_name);
        Ext.getCmp("saleprodid").setValue(row[0].data.product_id + "|" + row[0].data.product_name);
        Ext.getCmp('sale_product_name').show();
    }
}