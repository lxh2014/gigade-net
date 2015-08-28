Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);


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

Ext.onReady(function () {
    var leftW = 250; //左側樹狀結構的寬度
    var conW = 300; //右側的寬度
    var theight = 320; //窗口的高度
    var topheight = 130; //窗口的高度
    var pageSize = 25;

    var s_store;
    var p_store;

    var categoryId = "";    //商品類別編號
    var categoryName = "";  //商品類別名稱
    var boolClass = false;

    //獲取左邊的category樹結構(商品分類store)
    var treeStore = Ext.create('Ext.data.TreeStore', {
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: '/Product/GetProductCatagory',
            actionMethods: 'post'
        },
        rootVisible: false,
        root: {
            text: PRODUCT_SORT_CATEGORY,
            expanded: true,
            children: []
        }
    });
    treeStore.load();


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
            children: []
        }
    });
    frontCateStore.load();


    var treePanel = new Ext.tree.TreePanel({
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
                if (treeStore.getNodeById(nodeId).hasChildNodes() === true) {
                    //alert(nodeId);
                    //Ext.Msg.alert("有子節點,當前節點Id:" + nodeId);
                    Ext.getCmp('lblCategory').text = "";
                } else {    //沒有子節點時右側才顯示與當前活動相關的商品信息
                    //Ext.Msg.alert("沒有子節點,當前節點Id:" + nodeId);
                    categoryId = nodeId;
                    categoryName = nodeText;
                    //Ext.getCmp('lblCategory').text = "當前商品類別編號:" + categoryId + "　　類別名稱:" + categoryName;
                    Ext.getCmp('categoryId').setValue(categoryId);
                    Ext.getCmp('categoryName').setValue(categoryName);
                    SearchActivy();
                }
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
    //品牌store
    var classBrandStore = Ext.create('Ext.data.Store', {
        model: 'gigade.Brand',
        autoLoad: false,
        autoLoad: true,//
        //  autoDestroy: true, //自動銷毀
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
    //右半部的上半部的搜索panel
    var centerNorthPan = new Ext.form.FormPanel({
        region: 'north',
        width: document.documentElement.clientWidth,
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
                width: 1050,
                items: [
                    {
                        xtype: 'combobox', //class_id
                        allowBlank: true,
                        fieldLabel: SHOPCLASS,
                        editable: false,
                        id: 'shopClass_id',
                        labelWidth: 100,
                        width: conW,
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
                             fieldLabel: PRODUCT_SELECT_ALL,
                             id: 'key',
                             name: 'key',
                             width: conW,
                             margin: '0 0 0 150',
                             labelWidth: 100,
                             columnWidth: .7,
                             listeners: {
                                 specialkey: function (field, e) {
                                     if (e.getKey() == Ext.EventObject.ENTER) {
                                         Search();
                                     }
                                 }
                             }
                         },
                    {
                        xtype: 'button',
                        fieldLabel: PRODVISIT,
                        name: 'visitdate',
                        text: SEARCH,
                        margin: '0 0 0 10',
                        columnWidth: .2,
                        handler: Search,

                    }

                ]
            },
            {
                layout: 'column',
                border: false,
                width: 1050,
                items: [
                  {
                      xtype: 'combobox', //banner_id
                      allowBlank: true,
                      msgTarget: "side",
                      fieldLabel: BLANDNAME,//品牌
                      editable: true,
                      // forceSelection: true,
                      id: 'clsssBrand_id',
                      labelWidth: 100,
                      width: conW,
                      name: 'clsssbrand_name',
                      queryMode: 'local',
                      //disabled: true,
                      hiddenname: 'brand_id',
                      store: classBrandStore,
                      displayField: 'brand_name',
                      valueField: 'brand_id',
                      //forceSelection: false,
                      typeAhead: true,

                      emptyText: SELECT
                      ,
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
                          }
                          ,//做的修改有store的自動銷毀，下拉框焦點離開事件，forceSelection注釋掉了
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
                    fieldLabel: PRODUCT_NOW_CATEGORY_CODE,
                    id: 'categoryId',
                    name: 'categoryId',
                    labelWidth: 100,
                    columnWidth: .4,
                    margin: '0 0 0 150'
                },
                    {
                        xtype: 'displayfield',
                        fieldLabel: PRODUCT_CATEGORY_NAME,
                        id: 'categoryName',
                        name: 'categoryName',
                        labelWidth: 100,
                        margin: '0 0 0 50',
                        columnWidth: .5
                    }
                ]
            },
             {
                 layout: 'column',
                 border: false,
                 width: 1050,
                 items: [
                     {
                         xtype: 'combobox', //status
                         allowBlank: true,
                         fieldLabel: PRODSTATUS,
                         editable: true,
                         forceSelection: true,
                         id: 's_status',
                         labelWidth: 100,
                         width: conW,
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
                                 if (z.getValue() == "0" || z.getValue() == "20") {//新建立商品和供應商新建商品
                                     Ext.Msg.alert(INFORMATION, NOPRODSTATUS);
                                     z.setValue("");
                                 }
                             }
                         }
                     }, {
                         xtype: 'textfield',
                         fieldLabel: PRODUCT_CATEGORY_SELECT,
                         id: 'keyactivy',
                         name: 'keyactivy',
                         width: conW,
                         margin: '0 0 0 150',
                         labelWidth: 100
                         ,
                         columnWidth: .7,
                         listeners: {
                             specialkey: function (field, e) {
                                 if (e.getKey() == Ext.EventObject.ENTER) {
                                     SearchActivy();
                                 }
                             }
                         }
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
             },
            {
                layout: 'column',
                border: false,
                width: 1050,
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
                        fieldLabel: CLASSNAME,//產品分類
                        width: conW,
                        labelWidth: 100

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
                                    //z.clearValue();
                                    sc.clearValue();
                                    //  z.setDisabled(true);
                                }
                            }
                        }
                    }

                ]
            }

        ]
    });

    var p_store = Ext.create('Ext.data.Store', {
        pageSize: pageSize,
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

    var s_store = Ext.create('Ext.data.Store', {
        pageSize: pageSize,
        model: 'GIGADE.searchProduct',
        proxy: {
            type: 'ajax',
            url: '/Product/GetProductByCategorySet',
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
    || (Ext.getCmp('clsssBrand_id').getValue() != null && Ext.getCmp('clsssBrand_id').getValue() != "")
            || (Ext.getCmp('shopClass_id').getValue() != null && Ext.getCmp('shopClass_id').getValue() != "") || Ext.getCmp('comboFrontCage_hide').getValue() != "") {
            p_store.removeAll();
            p_store.load();
        } else {
            Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
        }
    }
    function SearchActivy() {
        if (categoryId != 0 && categoryId != "") {
            s_store.removeAll();
            s_store.load();
        }
        else {
            Ext.Msg.alert(INFORMATION, PLEASE_CHECKDE_SELECT_CATEGORY_MESSAGE);
        }
    }

    //控制productGrid中的按鈕
    var t_product_cm = Ext.create('Ext.selection.CheckboxModel', {
        listeners: {
            selectionchange: function (sm, selections) {
                Ext.getCmp("productGrid").down('#t_add').setDisabled(selections.length == 0);
            }
        }
    });

    //顯示所有商品
    var productGrid = new Ext.grid.Panel({
        id: 'productGrid',
        store: p_store,
        region: 'center',
        selModel: t_product_cm,
        autoScroll: true,
        border: 0,
        height: document.documentElement.clientHeight - topheight - 22,
        viewConfig: {
            enableTextSelection: true,
            stripeRows: false,
            getRowClass: function (record, rowIndex, rowParams, store) {
                return "x-selectable";
            }
        },
        columns: [
                  { header: PRODID, dataIndex: 'product_id', width: 60, align: 'center' },
                  { header: PRODNAME, dataIndex: 'product_name', width: 180, align: 'center' },
                  { header: BLANDNAME, dataIndex: 'brand_name', width: 120, align: 'center' },
                  { header: COMBINATION, dataIndex: 'combination', width: 120, align: 'center' },
                  { header: PRODSTATUS, dataIndex: 'product_status', width: 80, align: 'center' },
                  { header: DELIVERYSTORE, dataIndex: 'product_freight_set', width: 60, align: 'center' },
                  { header: PRODMODE, dataIndex: 'product_mode', width: 60, align: 'center' }
        ],
        tbar: [
            { xtype: 'button', id: 't_add', text: INSERT_TO_CATEGORY_MESSAGE, iconCls: 'icon-add', disabled: true, handler: function () { onToolAddClick(); } },
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


    //控制類別中商品的控件  
    var t_search_cm = Ext.create('Ext.selection.CheckboxModel', {
        listeners: {
            selectionchange: function (sm, selections) {
                Ext.getCmp("searchGrid").down('#t_remove').setDisabled(selections.length == 0);
            }
        }
    });



    //顯示類別中商品的grid
    var searchGrid = new Ext.grid.Panel({
        id: 'searchGrid',
        store: s_store,
        region: 'center',
        autoScroll: true,
        selModel: t_search_cm,
        autoScroll: true,
        border: 0,
        height: document.documentElement.clientHeight - topheight - 22,
        viewConfig: {
            enableTextSelection: true,
            stripeRows: false,
            getRowClass: function (record, rowIndex, rowParams, store) {
                return "x-selectable";
            }
        },
        columns: [
                 { header: PRODUCT_CATEGORY, dataIndex: 'category_name', width: 120, align: 'center' },
                 { header: PRODID, dataIndex: 'product_id', width: 60, align: 'center' },
                 { header: PRODNAME, dataIndex: 'product_name', width: 180, align: 'center' },

                 { header: BLANDNAME, dataIndex: 'brand_name', width: 120, align: 'center' },
                 { header: COMBINATION, dataIndex: 'combination', width: 120, align: 'center' },
                 { header: PRODSTATUS, dataIndex: 'product_status', width: 80, align: 'center' },
                 { header: DELIVERYSTORE, dataIndex: 'product_freight_set', width: 60, align: 'center' },
                 { header: PRODMODE, dataIndex: 'product_mode', width: 60, align: 'center' }
        ],
        tbar: [
            { xtype: 'button', id: 't_remove', text: REMOVE, iconCls: 'icon-remove', disabled: true, handler: function () { onToolRemoveClick(); } }
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

    //右半部的下半部的grid
    var centerSouthPan = new Ext.tab.Panel({
        region: 'center',
        autoScroll: true,
        items: [
            {
                title: PRODUCT_IN_CATEGORY,
                items: [searchGrid]
            }, {
                title: PRODUCT_ALL,
                items: [productGrid]

            }
        ]
    });

    //整個右半部的panel
    var centerPanel = new Ext.Panel({
        region: 'center',
        autoScroll: true,
        items: [centerNorthPan, centerSouthPan]
    }); //頁面佈局


    Ext.create('Ext.container.Viewport', {
        layout: 'border',
        items: [treePanel, centerPanel]
    });
    ToolAuthority();
    //p_store.load({ params: { start: 0, limit: pageSize } });
    if (categoryId != "") {
        s_store.load({ params: { start: 0, limit: pageSize } });
    }



    //將某一商品添加至特定的類別
    function onToolAddClick() {
        if (categoryId != "") {
            var row = Ext.getCmp("productGrid").getSelectionModel().getSelection();
            var pids = '';
            var bids = '';
            for (var i = 0; i < row.length; i++) {
                var product_id = row[i].get('product_id');
                var brand_id = row[i].get('brand_id');
                pids += product_id;
                bids += brand_id
                if (i != row.length - 1) {
                    pids += '|';
                    bids += '|';
                }
            }
            Ext.Msg.confirm(CONFIRM, Ext.String.format(SURE_INSERT_TO_CATEGORY_MESSAGE, categoryName), function (btn) {
                if (btn == 'yes') {
                    Ext.Ajax.request({
                        url: '/Product/SaveProductCategorySet',
                        params: {
                            brandids: bids,
                            categoryid: categoryId,
                            productids: pids
                        },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            if (result.success) {
                                if (result.proIds == "") {
                                    Ext.Msg.alert(INFORMATION, SUCCESS);
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, THIS_CATEGORY_HAVE_PRODUCT + result.proIds);
                                }
                                SearchActivy();
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
            });
        }
        else {
            Ext.Msg.alert(INFORMATION, PLEASE_SELECT_CATEGORY_MESSAGE);
        }


    }
    //將某一商品從特定的類別移除
    function onToolRemoveClick() {
        var row = Ext.getCmp("searchGrid").getSelectionModel().getSelection();
        if (row.length < 0) {
            Ext.Msg.alert(INFORMATION, NO_SELECTION);
        } else {
            Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
                if (btn == 'yes') {
                    var rowIDs = '';
                    var pids = '';
                    var bids = '';
                    var cids = '';
                    for (var i = 0; i < row.length; i++) {
                        var product_id = row[i].get('product_id');
                        var brand_id = row[i].get('brand_id');
                        var cateID = row[i].get('category_id');
                        pids += product_id;
                        bids += brand_id
                        cids += cateID;
                        if (i != row.length - 1) {
                            pids += '|';
                            bids += '|';
                            cids += '|';
                        }
                    }
                    Ext.Ajax.request({
                        url: '/Product/DeleteProductFromCategorySet',
                        method: 'post',
                        params: {
                            brandids: bids,
                            categoryid: cids,
                            productids: pids
                        },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            SearchActivy();
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    });
                }
            });
        }

    }

});
