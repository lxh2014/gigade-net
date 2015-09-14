Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);

var titleName = "區域包名稱：";
var leftW = 250; //左側樹狀結構的寬度
var conW = 300; //右側的寬度
var theight = 320; //窗口的高度
var topheight = 100; //窗口的高度
var pageSize = 25;
var leftW = 250; //左側樹狀結構的寬度
var rigW = 610; //右側的寬度
var theight = 620; //窗口的高度
var topheight = 170; //窗口的高度
var s_store;
var mainTainProduCateID = "0"; //活動類別ID
var siteid = 1;
var promationWin;
var boolClass = false;
var centerNorthPan;
var p_store;
var isSearchProduct = 0;//是否進行搜尋操作 >0的時候進行搜尋
var categoryId = "";    //商品類別編號
var categoryName = "";  //商品類別名稱
var categoryid = '';
var selectproductid = '';
var packetid = 0;
var treePanel;
var treeStore;
var centerPanel;
var searchGrid;
var productGrid;

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
     { name: 'product_freight_set', type: 'string' }
    ];
    //所有商品的store
    Ext.define('GIGADE.allPRODUCT', {
        extend: 'Ext.data.Model',
        fields: productPro
    });

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
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: '/Product/GetProductCatagory',
            actionMethods: 'post'
        },
        rootVisible: false,
        root: {
            text: PRODCATE,
            expanded: true,
            children: []
        }
    });
    treeStore.load();

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
                categoryId = nodeId;
                categoryName = nodeText;
                Ext.getCmp('categoryId').setValue(categoryId);
                Ext.getCmp('categoryName').setValue(categoryName);
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
                xtype: 'fieldcontainer',
                defaults: {
                    labelWidth: 40,
                    width: 120
                },
                id: 'ls',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'combobox', //class_id
                        allowBlank: true,
                        fieldLabel: CLASSNAME,
                        editable: false,
                        id: 'shopClass_id',
                        width: 200,
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
                        xtype: 'textfield',
                        fieldLabel: ALLPRODSEARCH,
                        labelWidth: 80,
                        id: 'key',
                        name: 'key',
                        width: 180
                    },
                    {
                        xtype: 'button',
                        fieldLabel: PRODVISIT,
                        name: 'visitdate',
                        text: SEARCH,
                        handler: Search
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                defaults: {
                    labelWidth: 40
                },
                id: '2s',
                combineErrors: true,
                layout: 'hbox',
                items: [
                {
                    xtype: 'combobox', //banner_id
                    allowBlank: true,
                    fieldLabel: BLANDNAME,
                    editable: true,
                    forceSelection: true,
                    id: 'clsssBrand_id',
                    Width: 200,
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
                },
                {
                    xtype: 'displayfield',
                    fieldLabel: CATEID,
                    id: 'categoryId',
                    name: 'categoryId',
                    width: 100,
                    labelWidth: 60
                },
                {
                    xtype: 'displayfield',
                    fieldLabel: CATENAME,
                    id: 'categoryName',
                    name: 'categoryName',
                    labelWidth: 60,
                    width: 200
                }
                ]
            },
            {
                xtype: 'fieldcontainer',
                defaults: {
                    labelWidth: 60,
                    width: 120
                },
                id: '3s',
                combineErrors: true,
                layout: 'hbox',
                items: [
                {
                    xtype: 'combobox', //status
                    allowBlank: true,
                    fieldLabel: PRODSTATUS,
                    editable: true,
                    forceSelection: true,
                    id: 's_status',
                    width: 200,
                    name: 's_status',
                    hiddenname: 's_status',
                    store: statusStore,
                    displayField: 'parameterName',
                    valueField: 'parameterCode',
                    typeAhead: true,
                    forceSelection: false,
                    emptyText: SELECT,
                    listeners: {
                        "select": function (combo, record) {
                            var z = Ext.getCmp("s_status");
                            if (z.getValue() == "0" || z.getValue() == "20") {
                                Ext.Msg.alert(INFORMATION, PRODSTATUSERRPE);
                                z.setValue("");
                            }
                        }
                    }
                },
                {
                    xtype: 'textfield',
                    fieldLabel: CATEPRODSEARCH,
                    id: 'keyactivy',
                    name: 'keyactivy',
                    labelWidth: 80,
                    width: 200
                },
                {
                    xtype: 'button',
                    fieldLabel: PRODVISIT,
                    name: 'visitdate',
                    text: SEARCH,
                    handler: SearchActivy
                }
                ]
            },
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
            }
        ]
    });

    p_store = Ext.create('Ext.data.Store', {
        pageSize: pageSize,
        autoLoad: false,
        model: 'GIGADE.allPRODUCT',
        proxy: {
            type: 'ajax',
            url: '/Product/GetAllProList',
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'item',
                totalProperty: 'totalCount'
            }
        }
    });
    s_store = Ext.create('Ext.data.Store', {
        pageSize: pageSize,
        autoLoad: false,
        model: 'GIGADE.searchProduct',
        proxy: {
            type: 'ajax',
            url: '/Element/GetProductByCategorySet',
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
            category_id: categoryId,
            status: Ext.getCmp('s_status').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('s_status').getValue()),
            keyCode: Ext.getCmp('keyactivy').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('keyactivy').getValue()),
            brand_id: Ext.getCmp('clsssBrand_id').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('clsssBrand_id').getValue()),
            class_id: Ext.getCmp('shopClass_id').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('shopClass_id').getValue()),
            comboFrontCage_hide: Ext.getCmp('comboFrontCage_hide').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('comboFrontCage_hide').getValue())

        });
    });
    p_store.on("beforeload", function () {
        Ext.apply(p_store.proxy.extraParams, {
            status: Ext.getCmp('s_status').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('s_status').getValue()),
            keyCode: Ext.getCmp('key').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('key').getValue()),
            brand_id: Ext.getCmp('clsssBrand_id').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('clsssBrand_id').getValue()),
            class_id: Ext.getCmp('shopClass_id').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('shopClass_id').getValue()),
            comboFrontCage_hide: Ext.getCmp('comboFrontCage_hide').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('comboFrontCage_hide').getValue())

        });
    });
    function Search() {
        if ((Ext.getCmp("s_status").getValue() != null && Ext.getCmp("s_status").getValue() != "") || Ext.getCmp("key").getValue() != ""
    || (Ext.getCmp('clsssBrand_id').getValue() != null && Ext.getCmp('clsssBrand_id').getValue() != "") ||
           (Ext.getCmp('shopClass_id').getValue() != null && Ext.getCmp('shopClass_id').getValue() != "")
            || Ext.getCmp('comboFrontCage_hide').getValue() != "") {
            p_store.removeAll();
            p_store.load();
        }
        else {
            Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
        }

    }
    function SearchActivy() {
        if (categoryId != 0 && categoryId != "") {
            s_store.removeAll();
            s_store.load();
        }
        else {
            Ext.Msg.alert(INFORMATION, "請選中查詢的類別！");
        }

    }

    //控制productGrid中的按鈕
    var t_product_cm = Ext.create('Ext.selection.CheckboxModel', {
        listeners: {
            selectionchange: function (sm, selections) {
                Ext.getCmp("productGrid").down('#t_save').setDisabled(selections.length == 0);
            }
        }
    });
    //控制類別中商品的控件  
    var t_search_cm = Ext.create('Ext.selection.CheckboxModel', {
        listeners: {
            selectionchange: function (sm, selections) {
                Ext.getCmp("searchGrid").down('#t_all_save').setDisabled(selections.length == 0);
            }
        }
    });

    //顯示所有商品
    productGrid = new Ext.grid.Panel({
        id: 'productGrid',
        store: p_store,
        region: 'center',
        selModel: t_product_cm,
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
                  { header: PROID, dataIndex: 'product_id', width: 60, align: 'center' },
                  { header: PRODNAME, dataIndex: 'product_name', width: 180, align: 'center' },
                  { header: BRANDNAME, dataIndex: 'brand_name', width: 120, align: 'center' },
                  { header: COMBINTYPE, dataIndex: 'combination', width: 120, align: 'center', hidden: true },
                  { header: PRODSTATUS, dataIndex: 'product_status', width: 80, align: 'center' },
                  { header: FRIGHTTYPE, dataIndex: 'product_freight_set', width: 60, align: 'center', hidden: true },
                  { header: PRODMODE, dataIndex: 'product_mode', width: 60, align: 'center', hidden: true }
        ],
        tbar: [
              { xtype: 'button', id: 't_save', text: SURE, iconCls: 'icon-add', disabled: true, handler: function () { onAddEleClick(); } }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: p_store,
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
    //顯示類別中商品的grid
    searchGrid = new Ext.grid.Panel({
        id: 'searchGrid',
        store: s_store,
        region: 'center',
        autoScroll: true,
        selModel: t_search_cm,
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
                 { header: CATENAME, dataIndex: 'category_name', width: 120, align: 'center' },
                 { header: PROID, dataIndex: 'product_id', width: 60, align: 'center' },
                 { header: PRODNAME, dataIndex: 'product_name', width: 180, align: 'center' },

                 { header: BRANDNAME, dataIndex: 'brand_name', width: 120, align: 'center' },
                 { header: COMBINTYPE, dataIndex: 'combination', width: 120, align: 'center', hidden: true },
                 { header: PRODSTATUS, dataIndex: 'product_status', width: 80, align: 'center' },
                 { header: FRIGHTTYPE, dataIndex: 'product_freight_set', width: 60, align: 'center', hidden: true },
                 { header: PRODMODE, dataIndex: 'product_mode', width: 60, align: 'center', hidden: true }
        ],
        tbar: [
            { xtype: 'button', id: 't_all_save', text: SURE, iconCls: 'icon-add', disabled: true, handler: function () { onAddEleCaClick(); } }
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
                title: '類別商品',
                items: [searchGrid]
            }, {
                title: '所有商品',
                items: [productGrid]
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
            //topPanel,
            treePanel,
            centerPanel
        ]
    });

});

function ElementProductShow(packetid, packetname, categoryid, selectproductid) {
    titleName += packetname;
    promationWin.title = titleName;
    packetid = packetid;
    promationWin.show();
    centerNorthPan.form.reset();
    s_store.removeAll();
    if (selectproductid != 0 && selectproductid != '') {
        if (categoryid != '0' && categoryid != '') {
            var record = treePanel.getStore().getNodeById(categoryid);
            treePanel.getSelectionModel().select(record);
            Ext.getCmp('categoryId').setValue(record.raw.id);
            Ext.getCmp('categoryName').setValue(record.raw.text);
            Ext.getCmp('keyactivy').setValue(selectproductid);
            if (category_id != "") {
                s_store.load({
                    params: {
                        category_id: categoryid
                    }
                });
                centerSouthPan.setActiveTab(0);
            } else {
                Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
            }
        } else {
            centerSouthPan.setActiveTab(1);
            Ext.getCmp('key').setValue(selectproductid);
            p_store.load();
        }
    }
}

function onAddEleCaClick() {
    if (Ext.getCmp('productGrid').getSelectionModel().getSelection().length != 0) {
        Ext.Msg.alert(INFORMATION, SELECPROTTIP);
        return;
    }
    var row = Ext.getCmp('searchGrid').getSelectionModel().getSelection();

    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        promationWin.close();
        Ext.getCmp('product_id').setValue(row[0].data.product_id);
        Ext.getCmp('category_id').setValue(categoryId);
        Ext.getCmp('category_name').show();
        Ext.getCmp('category_name').setValue(categoryName);
    }
}

function onAddEleClick() {
    if (Ext.getCmp('searchGrid').getSelectionModel().getSelection().length != 0) {
        Ext.Msg.alert(INFORMATION, SELECPROTTIP);
        return;
    }

    var row = Ext.getCmp('productGrid').getSelectionModel().getSelection();

    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        Ext.getCmp('category_name').setValue("");
        Ext.getCmp('category_id').setValue(0);
        Ext.getCmp('product_id').setValue(row[0].data.product_id);
        Ext.getCmp('category_name').hide();
        promationWin.close();
    }
}
