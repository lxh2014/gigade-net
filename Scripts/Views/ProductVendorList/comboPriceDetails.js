var firPanel;
var showComboPanel;
var showPanel;
var siteProductStore;

Ext.define('GIGADE.CHILDPRODUCT', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'Child_Id', type: 'string' },
        { name: 'Product_Name', type: 'string' },
        { name: 'spec_1', type: 'string' },
        { name: 'spec_2', type: 'string' },
        { name: 'item_money', type: 'int' },
        { name: 'event_money', type: 'int' },
        { name: 'item_cost', type: 'int' },
        { name: 'event_cost', type: 'int' },
        { name: 'Pile_Id', type: 'string' },
        { name: 'S_Must_Buy', type: 'string' },
        { name: 'G_Must_Buy', type: 'string' }
    ]
});
var columns = [{
    header: PRODUCTID,
    //  hidden: true,
    sortable: false,
    menuDisabled: true,
    colName: 'productNum',
    dataIndex: 'product_id'
}, {
    header: PRODUCTNAME,
    sortable: false,
    width: 300,
    // hidden: true,
    colName: 'product_name',
    menuDisabled: true,
    dataIndex: 'product_name'
}, {
    header: ITEMPRICE,
    colName: 'item_price',
    sortable: false,
    menuDisabled: true,
    dataIndex: 'price'
}, {
    header: MUSTBUY,
    colName: 'mustBuyNum',
    sortable: false,
    //hidden: true,
    menuDisabled: true,
    dataIndex: 's_must_buy'
}];

var columns1 = [{
    header: PRODUCTID,
    width: 60,
    hidden: true,
    sortable: false,
    menuDisabled: true,
    colName: 'productNum',
    dataIndex: 'Child_Id',
    renderer: function (val, metaData, record, rowIndex, colIndex, store, view) {
        if (rowIndex > 0) {
            return store.getAt(rowIndex - 1).get('Child_Id') == val ? "" : val;
        }
        return val;
    }
}, {
    header: PRODUCTNAME,
    sortable: false,
    width: 250,
    hidden: true,
    colName: 'product_name',
    menuDisabled: true,
    dataIndex: 'Product_Name'
}, { header: ITEM_SPEC1, dataIndex: 'spec_1', colName: 'txtSpec1Name', hidden: false, sortable: false, menuDisabled: true, align: 'center', width: 70 },
   { header: ITEM_SPEC2, dataIndex: 'spec_2', colName: 'txtSpec2Name', hidden: false, sortable: false, menuDisabled: true, align: 'center', width: 70 },
   { header: ITEM_COST, dataIndex: 'item_cost', colName: 'item_cost', hidden: true, sortable: false, menuDisabled: true, width: 70,
       renderer: function (val) { return val == 0 ? "" : val }
   },
    { header: ITEM_MONEY, dataIndex: 'item_money', colName: 'item_price', hidden: true, sortable: false, menuDisabled: true, width: 70,
        renderer: function (val) { return val == 0 ? "" : val }
    },
    { header: ITEM_EVENT_COST, dataIndex: 'event_cost', colName: 'event_item_cost', hidden: true, sortable: false, menuDisabled: true, width: 80,
        renderer: function (val) { return val == 0 ? "" : val }
    },
    { header: ITEM_EVENT_MONEY, dataIndex: 'event_money', colName: 'event_price', hidden: true, sortable: false, menuDisabled: true, width: 80,
        renderer: function (val) { return val == 0 ? "" : val }

    }, {
        header: MUSTBUY,
        colName: 'mustBuyNum',
        sortable: false,
        hidden: true,
        menuDisabled: true,
        width: 70,
        dataIndex: 'S_Must_Buy',
        renderer: function (val, metaData, record, rowIndex, colIndex, store, view) {
            if (rowIndex > 0) {
                return store.getAt(rowIndex - 1).get('Child_Id') == record.get('Child_Id') ? "" : val;
            }
            return val;
        }
    }];

/***************組合商品價格介面************************/
function createPricePanel(combination, product_id, price_type) {

    Ext.define('GIGADE.PRODUCTSITE', {
        extend: 'Ext.data.Model',
        fields: [
        { name: 'price_master_id', type: 'string' },
        { name: 'product_id', type: 'string' },
        { name: 'site_id', type: 'string' },
        { name: 'site_name', type: 'string' },
        { name: 'product_name', type: 'string' },
        { name: 'price_status', type: 'string' },
        { name: 'user_level', type: 'string' },
        { name: 'user_email', type: 'string' },
        { name: 'user_id', type: 'string' },
        { name: 'same_price', type: 'string' },
        { name: 'cost', type: 'string' },
        { name: 'price', type: 'string' },
        { name: 'event_cost', type: 'string' },
        { name: 'event_price', type: 'string' },
        { name: 'event_start', type: 'string' },
        { name: 'event_end', type: 'string' },
        { name: 'status', type: 'string' }
        ]
    });

    siteProductStore = Ext.create("Ext.data.Store", {
        model: 'GIGADE.PRODUCTSITE',
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: '/ProductVendorList/ProcmoGetPriceMaster',
            actionMethods: 'post',
            reader: {
                type: 'json'
            }
        }
    });

    firPanel = Ext.create("Ext.form.Panel", {
        id: 'firPanel',
        border: false,
        defaults: {
            labelWidth: 110,
            padding: '2 0 0 0'
        },
        items: [{
            xtype: 'checkbox',
            boxLabel: FRONT_SHOW_PRICE,
            inputValue: '1',
            hidden: false,
            readOnly: true,
            colName: 'show_pricelist2',
            id: 'show_pricelist2',
            name: 'show_pricelist2',
            checked: true,
            margin: '0 0 0 115'
        }, {
            xtype: 'displayfield',
            fieldLabel: PRICELIST,
            hidden: false,
            id: 'Product_Price_List',
            colName: 'price_list',
            name: 'Product_Price_List'
        }, {
            xtype: 'displayfield',
            // hidden: true,
            fieldLabel: BAG_CHECK_MONEY,
            id: 'bag_check_money1',
            name: 'bag_check_money1',
            colName: 'bag_check_money1'
        }, {
            xtype: 'panel',
            border: false,
            autoScroll: true,
            width: 600,
            layout: 'hbox',
            items: [
            {
                xtype: 'displayfield',
                width: 300,
                hidden: false,
                fieldLabel: PRICETYPE,
                colName: 'price_type',
                id: 'price_type_name',
                name: 'price_type_name'
            }, {
                id: 'self_price',
                hidden: true,
                xtype: 'displayfield',
                value: '<span style="color:red">各自定價之成本售價顯示為最小成本及售價</span>'
            }]
        }, {
            xtype: 'gridpanel',
            id: 'siteProdutctGrid',
            store: siteProductStore,
            height: 150,
            columns: [
                { colName: 'site_name', header: SITENAME, dataIndex: 'site_name', width: 100, align: 'center', sortable: false, menuDisabled: true },
                { colName: 'product_name', header: PRODUCTNAME, dataIndex: 'product_name', width: 180, align: 'center', sortable: false, menuDisabled: true },
                { colName: 'user_level', header: USERLEVEL, dataIndex: 'user_level', width: 100, align: 'center', sortable: false, menuDisabled: true },
                { colName: 'user_email', header: USERMAIL, dataIndex: 'user_email', width: 120, align: 'center', sortable: false, menuDisabled: true },
                { colName: 'price_status', header: PRICESTATUS, dataIndex: 'status', width: 100, align: 'center', sortable: false, menuDisabled: true },
            //edit by hjj 2014/08/13 將固定組合商品與單一商品售價與成本的順序一致化
                {colName: 'price', header: PRICE, dataIndex: 'price', width: 100, align: 'center', sortable: false, menuDisabled: true },
                { colName: 'item_cost', hidden: true, header: COST, dataIndex: 'cost', width: 100, align: 'center', sortable: false, menuDisabled: true },

                { colName: 'event_price', header: EVENTPRICE, dataIndex: 'event_price', width: 100, align: 'center', sortable: false, menuDisabled: true },
                { colName: 'event_item_cost', header: EVENT_COST, dataIndex: 'event_cost', width: 100, align: 'center', sortable: false, menuDisabled: true },
                { colName: 'event_time', header: EVENT_TIME, xtype: 'templatecolumn',
                    tpl: Ext.create('Ext.XTemplate',
                        '{[values.event_start==0?"":Ext.Date.format(new Date(values.event_start * 1000),"Y/m/d H:i:s")]}',
                        '~',
                        '{[values.event_end==0?"":Ext.Date.format(new Date(values.event_end * 1000),"Y/m/d H:i:s")]}'
                    ), width: 240, align: 'left', sortable: false, menuDisabled: true
                }
            ],
            selModel: price_type == 1 ? [] : Ext.create('Ext.selection.CheckboxModel', {
                mode: 'SINGLE',
                listeners: {
                    selectionchange: function (sm, selections) {
                        Ext.getCmp("showComboPanel").show();
                        var rec = selections[0];
                        if (rec) {
                            GetSiteChildData(rec.data.user_id, rec.data.user_level, rec.data.site_id, rec.data.same_price);
                        }
                    }
                }
            })
        }]
    });
    showComboPanel = Ext.create("Ext.form.Panel", {
        id: 'showComboPanel',
        border: false,
        hidden: price_type == 1 ? false : true,
        defaults: {
            labelWidth: 110,
            padding: '2 0 0 0'
        }
    });

    if (combination == 2 || combination == 3) {
        var combinationStore;
        if (price_type == 1) {
            combinationStore = Ext.create("Ext.data.Store", {
                fields: ['product_id', 'product_name', 'price', 's_must_buy', 'g_must_buy'],
                proxy: {
                    type: 'ajax',
                    url: '/ProductVendorList/QuerySingleProPrice',
                    actionMethods: 'post',
                    reader: {
                        type: 'json',
                        root: 'data'
                    }
                }
            });
            combinationStore.load({ params: { product_id: product_id, pile_id: 0} });
        }
        else {
            combinationStore = Ext.create("Ext.data.Store", {
                model: 'GIGADE.CHILDPRODUCT'
            });
        }

        var ficxed = Ext.create("Ext.grid.Panel", {
            id: 'ComcoFixed',
            store: combinationStore,
            border: false,
            columns: price_type == 1 ? columns : columns1,
            listeners: {
                viewready: function () {
                    var rec = this.getStore().getAt(0);
                    if (rec) {
                        if (rec.data.g_must_buy != 0) {
                            this.setTitle('<span style="color:red">' + MUSTCHOOSE + ':' + rec.data.g_must_buy + "</span>");
                        }
                    }
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
    if (combination == 4) {
        //查詢群組
        Ext.Ajax.request({
            url: '/ProductVendorList/groupNameQuery',
            method: 'post',
            params: {
                "ProductId": product_id
            },
            success: function (response) {
                var resText = eval("(" + response.responseText + ")");
                var pileCount = resText.length;

                if (pileCount == 0) return;

                for (var i = 1; i <= pileCount; i++) {
                    /**********************Store*****************/
                    var priceStore = "priceStore_1" + i;
                    if (price_type == 1) {
                        priceStore = Ext.create("Ext.data.Store", {
                            fields: ['product_id', 'product_name', 'price', 's_must_buy', 'g_must_buy'],
                            proxy: {
                                type: 'ajax',
                                url: '/ProductVendorList/QuerySingleProPrice',
                                actionMethods: 'post',
                                reader: {
                                    type: 'json',
                                    root: 'data'
                                }
                            }
                        });
                        priceStore.load({ params: { product_id: product_id, pile_id: resText[i - 1].Pile_Id} });
                    }
                    else {
                        priceStore = Ext.create("Ext.data.Store", {
                            model: 'GIGADE.CHILDPRODUCT'
                        });
                    }

                    /*****************Grid***************************/
                    var pileGrid = Ext.create("Ext.grid.Panel", {
                        title: GROUP + i,
                        height: 200,
                        width: 800,
                        id: '' + i + '_ComcoFixed',
                        store: priceStore,
                        border: false,
                        columns: price_type == 1 ? columns : columns1,
                        listeners: {
                            viewready: function () {
                                var rec = this.getStore().getAt(0);
                                if (rec) {
                                    this.setTitle(this.title + '<span style="color:red;margin-left:40px">' + MUSTCHOOSE + ':' + rec.data.g_must_buy + "</span>");
                                }
                            }
                        }
                    });

                    showComboPanel.add(pileGrid);
                    /*****************end******************************/
                }
            }
        });
    }
}

var tmp_store = Ext.create("Ext.data.Store", {
    model: 'GIGADE.CHILDPRODUCT',
    groupField: 'Pile_Id'
});
function GetSiteChildData(user_id, user_level, site_id, is_same) {
    Ext.Ajax.request({
        method: 'post',
        url: '/ProductCombo/GetUpdatePrice',
        params: {
            "ProductId": product_id,
            "site_id": site_id,
            "user_id": user_id,
            "user_level": user_level,
            "is_same": is_same
        },
        success: function (response) {
            var resText = eval("(" + response.responseText + ")");
            if (resText.success) {
                tmp_store.loadData(resText.data);
                tmp_store.sort('Pile_Id', 'ASC');
                var group = tmp_store.getGroups();
                for (var i = 0; i < group.length; i++) {
                    var grid = Ext.getCmp('showComboPanel').query('grid')[i];
                    if (grid) {
                        var spec_1 = grid.down('*[colName=txtSpec1Name]');
                        is_same == "1" ? spec_1.hide() : spec_1.show();
                        var spec_2 = grid.down('*[colName=txtSpec2Name]');
                        is_same == "1" ? spec_2.hide() : spec_2.show();

                        grid.getStore().loadRecords(group[i].children);
                        var rec = grid.getStore().getAt(0);
                        if (rec && grid.title) {
                            var title = '';
                            if (group.length == 1) {
                                if (rec.data.G_Must_Buy != 0)
                                    title = '<span style="color:red">' + MUSTCHOOSE + ':' + rec.data.G_Must_Buy + "</span>";
                            }
                            else {
                                title = (GROUP + group[i].name) + '<span style="color:red;margin-left:40px">' + MUSTCHOOSE + ':' + rec.data.G_Must_Buy + "</span>";
                            }
                            grid.setTitle(title);
                        }
                    }
                }
            }
        },
        failure: function () {
            Ext.Msg.alert(INFORMATION, DATA_LOAD_FAILURE);
        }
    });
};

function PriceLoadTogether(combination, product_id) {
    siteProductStore.load({ params: { ProductId: product_id} });


    //價格類型store
    var priceTypeStore = Ext.create("Ext.data.Store", {
        fields: ['parameterCode', 'parameterName'],
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: '/Parameter/QueryPara?paraType=price_type',
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'items'
            }
        }
    });

    Ext.Ajax.request({
        url: '/ProductVendorList/QueryProduct',
        method: 'post',
        params: {
            "ProductId": product_id
        },
        success: function (response) {
            var resText = eval("(" + response.responseText + ")");
            if (!resText) return;
            Ext.getCmp("Product_Price_List").setValue(resText.data.Product_Price_List);
            Ext.getCmp("bag_check_money1").setValue(resText.data.Bag_Check_Money);
            Ext.getCmp("show_pricelist2").setValue(resText.data.show_listprice == 1 ? true : false);
            priceTypeStore.load({
                callback: function () {
                    if (priceTypeStore.find("parameterCode", resText.data.Price_type) != -1) {
                        Ext.getCmp("price_type_name").setValue(priceTypeStore.getAt(priceTypeStore.find("parameterCode", resText.data.Price_type)).data.parameterName);
                        //adit by hufeng0813w 2014/06/17 Reason:各自定價顯示文字
                        if (resText.data.Price_type == "2") {
                            Ext.getCmp("self_price").show();
                            Ext.getCmp("self_price").setVisible(true);
                        }
                    }
                }
            });

        }
    });


}

//單一商品價格介面
function singlePricePanel(product_id) {
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
            url: '/ProductVendorList/GetProItems',
            actionMethods: 'post',
            reader: {
                type: 'json'
            }
        }
    });

    itemStore.load({ params: { ProductId: product_id} });
    Ext.define('GIGADE.PRODUCTSITE', {
        extend: 'Ext.data.Model',
        fields: [
        { name: 'price_master_id', type: 'string' },
        { name: 'product_id', type: 'string' },
        { name: 'site_id', type: 'string' },
        { name: 'site_name', type: 'string' },
        { name: 'product_name', type: 'string' },
        { name: 'user_level_name', type: 'string' },
        { name: 'user_level', type: 'string' },
        { name: 'user_email', type: 'string' },
        { name: 'same_price', type: 'string' },
        { name: 'event_start', type: 'string' },
        { name: 'event_end', type: 'string' },
        { name: 'item_cost', type: 'string' },
        { name: 'item_money', type: 'string' },
        { name: 'event_cost', type: 'string' },
        { name: 'event_money', type: 'string' },
        { name: 'status', type: 'string' }
    ]
    });
    siteProductStore = Ext.create("Ext.data.Store", {
        model: 'GIGADE.PRODUCTSITE',
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: '/ProductVendorList/GetPriceMaster',
            actionMethods: 'post',
            reader: {
                type: 'json'
            }
        }
    });

    var siteProdutctGrid = Ext.create('Ext.grid.Panel', {
        id: 'siteProdutctGrid',
        store: siteProductStore,
        height: 150,
        columns: [
            { colName: 'site_name', header: SITENAME, dataIndex: 'site_name', width: 130, align: 'center', sortable: false, menuDisabled: true },
            { colName: 'product_name', header: PRODUCTSHOENAME, dataIndex: 'product_name', width: 180, align: 'center', sortable: false, menuDisabled: true },
            { colName: 'user_level', header: USERLEVEL, dataIndex: 'user_level_name', width: 100, align: 'center', sortable: false, menuDisabled: true },
            { colName: 'user_email', header: USERMAIL, dataIndex: 'user_email', width: 120, align: 'center', sortable: false, menuDisabled: true },
            { colName: 'price_status', header: PRICESTATUS, dataIndex: 'status', width: 100, align: 'center', sortable: false, menuDisabled: true },
            { colName: 'price', header: ITEM_MONEY, dataIndex: 'item_money', width: 100, align: 'center', sortable: false, menuDisabled: true },
            { colName: 'item_cost', header: ITEM_COST, dataIndex: 'item_cost', width: 100, align: 'center', sortable: false, menuDisabled: true },

            { colName: 'event_price', header: ITEM_EVENT_MONEY, dataIndex: 'event_money', width: 100, align: 'center', sortable: false, menuDisabled: true },
            { colName: 'event_item_cost', header: ITEM_EVENT_COST, dataIndex: 'event_cost', width: 100, align: 'center', sortable: false, menuDisabled: true },
            { colName: 'event_time', header: EVENT_TIME, xtype: 'templatecolumn',
                tpl: Ext.create('Ext.XTemplate',
                    '{[values.event_start==0?"":Ext.Date.format(new Date(values.event_start * 1000),"Y/m/d H:i:s")]}',
                    '~',
                    '{[values.event_end==0?"":Ext.Date.format(new Date(values.event_end * 1000),"Y/m/d H:i:s")]}'
                ), width: 220, align: 'left', sortable: false, menuDisabled: true
            }


        ]
    });

    var gigadeGrid = Ext.create('Ext.grid.Panel', {
        store: itemStore,
        height: 280,
        columns: [
            { xtype: 'rownumberer', width: 50, align: 'center' },
            { colName: 'item_id', header: ITEM_ID, dataIndex: 'Item_Id', width: 80, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'txtSpec1Name', header: ITEM_SPEC1, dataIndex: 'Spec_Name_1', width: 110, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'txtSpec2Name', header: ITEM_SPEC2, dataIndex: 'Spec_Name_2', width: 110, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'product_vendor_code', header: ITEM_CODE, dataIndex: 'Item_Code', width: 160, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'price', header: ITEM_MONEY, dataIndex: 'Item_Money', width: 110, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'item_cost', header: ITEM_COST, dataIndex: 'Item_Cost', width: 110, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'event_price', header: ITEM_EVENT_MONEY, dataIndex: 'Event_Item_Money', width: 110, align: 'center', sortable: false, menuDisabled: true, hidden: false },
            { colName: 'event_item_cost', header: ITEM_EVENT_COST, dataIndex: 'Event_Item_Cost', width: 110, align: 'center', sortable: false, menuDisabled: true, hidden: false }
        ]
    });

    showPanel = Ext.create('Ext.panel.Panel', {
        id: 'showPanel',
        layout: 'anchor',
        border: false,
        items: [{
            xtype: 'checkbox',
            boxLabel: FRONT_SHOW_PRICE,
            inputValue: '1',
            hidden: false,
            readOnly: true,
            colName: 'show_pricelist2',
            id: 'show_pricelist2',
            name: 'show_pricelist2',
            margin: '0 0 0 115'
        }, {
            xtype: 'displayfield',
            width: 150,
            colName: 'price_list',
            hidden: false,
            labelAlign: 'left',
            margin: '0 5',
            fieldLabel: PRICELIST,
            value: '&nbsp;',
            id: 'product_price_list'
        }, {
            xtype: 'displayfield',
            hidden: false,
            fieldLabel: BAG_CHECK_MONEY,
            margin: '5 5',
            id: 'bag_check_money1',
            name: 'bag_check_money1',
            colName: 'bag_check_money1'
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
            disabled: false,
            id: 'same_price',
            colName: 'same_price',
            boxLabel: SAME_PRICE
        },
        gigadeGrid]
    });
}

function LinkCoding(type, product_id) {
    Ext.Ajax.request({
        url: '/VendorProductList/ProductPreview',
        params: { Product_Id: product_id, Type: type },
        success: function (form, action) {
            var result = form.responseText;
            var htmlval = "<a href=" + result + " target='new'>" + result + "</a>";
            Ext.create('Ext.window.Window', {
                title: '連接編碼',
                modal: true,
                height: 100,
                width: 600,
                constrain: true,
                layout: 'fit',
                items: {
                    xtype: 'panel',
                    border: false,
                    bodyStyle: {
                        padding: '20px 10px'
                    },
                    html: htmlval
                }
            }).show();
        }
    })
}