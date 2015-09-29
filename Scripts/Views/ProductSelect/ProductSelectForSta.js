var pageSize = 15;

var pro = [
    { name: 'product_image', type: 'string' },
    { name: 'product_id', type: 'string' },
    { name: 'brand_name', type: 'string' },
    { name: 'product_name', type: 'string' },
    { name: 'prod_sz', type: 'string' }
];

pro.push({ name: 'combination', type: 'string' });
pro.push({ name: 'price_type', type: 'string' });
pro.push({ name: 'combination_id', type: 'string' });
pro.push({ name: 'product_spec', type: 'string' });
pro.push({ name: 'product_price_list', type: 'string' });
pro.push({ name: 'product_status', type: 'string' });
pro.push({ name: 'product_status_id', type: 'string' });
pro.push({ name: 'product_freight_set', type: 'string' });
pro.push({ name: 'product_mode', type: 'string' });
pro.push({ name: 'tax_type', type: 'string' });
pro.push({ name: 'product_sort', type: 'string' });
pro.push({ name: 'product_createdate', type: 'string' });
pro.push({ name: 'product_start', type: 'string' });
pro.push({ name: 'product_end', type: 'string' });
pro.push({ name: 'CanSel', type: 'string' }); //0 為checkbox可以勾選
pro.push({ name: 'CanDo', type: 'string' }); //1 為自己建立的商品 

Ext.define('GIGADE.PRODUCT', {
    extend: 'Ext.data.Model',
    fields: pro
});

//創建數據源
var p_store = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.PRODUCT',
    proxy: {
        type: 'ajax',
        url: '/ProductSelect/QueryProListForAddSta',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'item',
            totalProperty: 'totalCount'
        }
    }
});
//（數據加載前）獲得所選的條件
p_store.on("beforeload", function () {
    Ext.apply(p_store.proxy.extraParams, {
        brand_id: Ext.getCmp('brand_id') ? Ext.getCmp('brand_id').getValue() : '',
        comboProCate_hide: Ext.getCmp('comboProCate_hide') ? Ext.getCmp('comboProCate_hide').getValue() : '',
        comboFrontCage_hide: Ext.getCmp('comboFrontCage_hide') ? Ext.getCmp('comboFrontCage_hide').getValue() : '',
        combination: Ext.getCmp('combination') ? Ext.getCmp('combination').getValue() : '',
        product_status: Ext.getCmp('product_status') ? Ext.getCmp('product_status').getValue() : '',
        product_freight_set: Ext.getCmp('product_freight_set') ? Ext.getCmp('product_freight_set').getValue() : '',
        product_mode: Ext.getCmp('product_mode') ? Ext.getCmp('product_mode').getValue() : '',
        tax_type: Ext.getCmp('tax_type') ? Ext.getCmp('tax_type').getValue() : '',
        date_type: Ext.getCmp('date_type') ? Ext.getCmp('date_type').getValue() : '',
        time_start: Ext.getCmp('time_start') ? Ext.getCmp('time_start').getValue() : '',
        time_end: Ext.getCmp('time_end') ? Ext.getCmp('time_end').getValue() : '',
        key: Ext.getCmp('key') ? Ext.htmlEncode(Ext.getCmp('key').getValue()) : '',
        //添加  選擇的值  排除 不需要的數據  add by zhuoqin0830w  2015/02/10
        priceCondition: Ext.getCmp("chock").getValue()["rb"],
        //add by zhuoqin0830w  2015/03/11  已買斷商品的篩選功能
        productPrepaid: Ext.getCmp('buyPro') ? Ext.getCmp('buyPro').getValue() : ''
    });
});
//panel里的選框
var sm = Ext.create('Ext.selection.CheckboxModel', {
    storeColNameForHide: 'CanDo'    //根據Cando來隱藏checkbox 2014/09/10
});

//查詢按鈕
Ext.onReady(function () {
    //批量新增站臺價
    var tools = [{ id: "tools", name: "tools", text: QUANTITY_INSERT_SITE_PRICE, type: 'tbar', iconCls: 'icon-edit', disabled: false, handler: onAddClick }];
    //回車鍵查詢
    // edit by zhuoqin0830w  2015/09/22  以兼容火狐瀏覽器
    document.onkeydown = function (event) {
        e = event ? event : (window.event ? window.event : null);
        if (e.keyCode == 13) {
            $("#btn_search").click();
        }
    };
    //表單panel
    //查詢，重置按鈕
    var frm = Ext.create('Ext.form.Panel', {
        layout: 'column',
        id: 'frm',
        width: 1500,
        border: false,
        padding: '5 5',
        defaults: { anchor: '95%', msgTarget: "side" },
        items: [{
            xtype: 'panel',
            columnWidth: .70,
            border: 0,
            layout: 'anchor',
            items: [brand, category, {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                anchor: '100%',
                items: [type, pro_status]
            }, freight_mode_tax, start_end, key_query, {
                xtype: 'panel',
                border: 0,
                layout: 'anchor',
                items: [radioCondition]
            }],
            buttonAlign: 'center',
            buttons: [{
                text: SEARCH,
                id: 'btn_search',
                iconCls: 'ui-icon ui-icon-search-2',
                handler: Search
            }, {
                text: RESET,
                id: 'btn_reset',
                iconCls: 'ui-icon ui-icon-reset',
                listeners: {
                    click: function () {
                        frm.getForm().reset();
                        Ext.getCmp("combination").setValue(ComboStore.data.items[0].data.parameterCode);
                        Ext.getCmp("date_type").setValue(DateStore.data.items[0].data.code);
                        Ext.getCmp("product_freight_set").setValue(freightStore.getAt(0).data.parameterCode);
                        Ext.getCmp("product_mode").setValue(modeStore.getAt(0).data.parameterCode);
                        Ext.getCmp("tax_type").setValue(taxStore.getAt(0).data.value);
                    }
                }
            }]
        }]
    });

    var proColumns = new Array();
    proColumns = proColumns.concat(c_pro_base);
    proColumns.push(c_pro_type);
    proColumns.push(c_pro_pricetype); //價格類型 再商品列表中添加
    proColumns.push(c_pro_spec);
    proColumns.push(c_pro_status);
    proColumns.push(c_pro_freight);
    proColumns.push(c_pro_mode);
    proColumns.push(c_pro_tax);
    proColumns.push(c_pro_create);
    proColumns.push(c_pro_start);
    proColumns.push(c_pro_end);

    var proGrid = Ext.create('Ext.grid.Panel', {
        hidden: true,
        id: 'proGrid',
        height: document.documentElement.clientHeight <= 700 ? document.documentElement.clientHeight - 242 : document.documentElement.clientHeight - 222,
        columnLines: true,
        selModel: sm,
        tbar: tools,
        store: p_store,
        columns: proColumns,
        bbar: Ext.create('Ext.PagingToolbar', {
            store: p_store,
            pageSize: pageSize,
            displayInfo: true
        }),

        listeners: {

            viewready: function () {
                setShow(proGrid, 'colName');
            },
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });

    //整個容器
    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        border: false,
        items: [frm, proGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                proGrid.height = document.documentElement.clientHeight <= 700 ? document.documentElement.clientHeight - 242 : document.documentElement.clientHeight - 222;
                this.doLayout();
            },
            afterrender: function () {
                ToolAuthorityQuery(function () {
                    //setShow(frm, 'colName');
                    //setTimeout(Search, 500);                  //2015/05/25
                });
            }
        }
    });
});

function Search() {
    Ext.getCmp('key').setValue(Ext.getCmp('key').getValue().replace(/\s+/g, ','));
    Ext.getCmp('tools').setDisabled(false);//add 2014/08/27
    Ext.getCmp('proGrid').show();
    p_store.loadPage(1);
}

function addDateType() {
    if (DateStore.getCount() == 1) {
        DateStore.add(r_create, r_start, r_end);
    }
}

function storeLoad() {
    p_store.load();
}

onAddClick = function () {
    var rows = Ext.getCmp('proGrid').getSelectionModel().getSelection();
    var productIds = '';
    Ext.each(rows, function (row) {
        productIds += row.data["product_id"] + ',';
    });
    productIds = productIds.substring(0, productIds.length - 1);

    var url = "/ProductSelect/ModifyStationPrices?productIds=" + productIds + "&?priceCondition=" + Ext.getCmp("chock").getValue()["rb"];
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var event = panel.down('#event');
    if (event) {
        event.close();
    }
    event = panel.add({
        id: 'event',
        title: QUANTITY_ADD_SITE_PRICE,//批量增加站臺價
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(event);
    panel.doLayout();
};

cellEditSort = Ext.create('Ext.grid.plugin.CellEditing', {
    clicksToEdit: 1
});