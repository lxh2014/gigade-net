
var pageSize = 10;
var clientHeight = document.documentElement.clientHeight;
var clientWidth = document.documentElement.clientWidth;
//列选择模式
var sm = Ext.create('Ext.selection.CheckboxModel', {
    storeColNameForHide: 'CanReplace',
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("replaceVerifyGrid").down('#btnApply').setDisabled(selections.length == 0);
        }
    }
});

//代審核商品MODEL
Ext.define('GIGADE.REPLACEVERIFY', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'product_image', type: 'string' },
        { name: 'product_id', type: 'int' },
        { name: 'brand_name', type: 'string' },
        { name: 'product_name', type: 'string' },
        { name: 'user_name', type: 'string' },
        { name: 'combination', type: 'string' },
        { name: 'product_price_list', type: 'string' },
        { name: 'product_status', type: 'string' },
        { name: 'product_freight_set', type: 'string' },
        { name: 'product_mode', type: 'string' },
        { name: 'tax_type', type: 'string' },
        { name: 'product_createdate', type: 'string' },
        { name: 'product_start', type: 'string' },
        { name: 'product_end', type: 'string' },
        { name: 'CanReplace', type: 'string' }
    ]
});


var replaceListStore = Ext.create("Ext.data.Store", {
    model: 'GIGADE.REPLACEVERIFY',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/ProductList/replaceVerifyQuery',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});


replaceListStore.on('beforeload', function () {

    var brandId = Ext.getCmp("brand_id").getValue() == null ? 0 : Ext.getCmp("brand_id").getValue();
    var cateType = Ext.getCmp('comboProCate_hide').getValue();
    var frontCateType = !Number(Ext.getCmp('comboFrontCage_hide').getValue()) ? 0 : Ext.getCmp('comboFrontCage_hide').getValue();
    var productType = Ext.getCmp("combination").getValue() == null ? 0 : Ext.getCmp("combination").getValue();
    var productStatus = Ext.getCmp("product_status").getValue() == null ? -1 : Ext.getCmp("product_status").getValue();
    var dataType = Ext.getCmp("date_type").getValue();
    var key = Ext.htmlEncode(Ext.getCmp("key").getValue());

    var queryCondition = "{brand_id:'" + brandId + "',cate_id:'" + cateType + "',";
    queryCondition += "category_id:'" + frontCateType + "',combination:'" + productType + "',";
    queryCondition += "product_status:'" + productStatus + "',date_type:'" + dataType + "',";
    queryCondition += "name_number:'" + key + "'}";
    Ext.apply(replaceListStore.proxy.extraParams,
        {
            queryCondition: Ext.htmlEncode(queryCondition),
            time_start: Ext.getCmp("time_start").getValue(),
            time_end: Ext.getCmp("time_end").getValue()
        });
});




var proColumns = c_pro_base;
proColumns.push(c_pro_kuser);            //建立人
proColumns.push(c_pro_type);            //商品類型
proColumns.push(c_pro_pricelist);            //建議售價
proColumns.push(c_pro_status);       //商品狀態
proColumns.push(c_pro_freight);           //運送方式
proColumns.push(c_pro_mode);     //出貨方式
proColumns.push(c_pro_tax);        //營業稅
proColumns.push(c_pro_create);  //建立時間
proColumns.push(c_pro_start);        //上架時間
proColumns.push(c_pro_end);        //下架時間

//添加日期類型
function addDateType() {
    if (DateStore.getCount() == 1) {
        DateStore.add(r_create, r_start, r_end);
    }
}

function searchShow() {
    Ext.getCmp('replaceVerifyGrid').show();
    replaceListStore.loadPage(1);
}

Ext.onReady(function () {
    document.body.onkeydown = function () {
        if (event.keyCode == 13) {
            $("#btnSearch").click();
        }
    };

    //panel
    var frm = Ext.create("Ext.form.Panel", {
        layout: 'column',
        id: 'frm',
        width: 1185,
        border: false,
        margin: '0 0 10 0',
        defaults: { anchor: '95%', msgTarget: "side", padding: '5 5' },
        items: [{
            xtype: 'panel',
            columnWidth: .60,
            border: 0,
            layout: 'anchor',
            items: [brand, category, type_status, start_end, key_query]
        }],
        buttonAlign: 'center',
        buttons: [{
            text: BTN_SEARCH,
            id: 'btnSearch',
            handler: function () {
                searchShow();
            }
        }, {
            text: RESET,
            id: 'btn_reset',
            listeners: {
                click: function () {
                    frm.getForm().reset();
                    Ext.getCmp("combination").setValue(ComboStore.data.items[0].data.parameterCode);
                    Ext.getCmp("product_status").setValue(prodStatusStore.data.items[0].data.parameterCode);
                    Ext.getCmp("date_type").setValue(DateStore.data.items[0].data.code);
                }
            }
        }]
    });

    //grid
    var replaceVerifyGrid = Ext.create("Ext.grid.Panel", {
        selModel: sm,
        height: clientWidth <= 1185 ? clientHeight - 200 - 20 : clientHeight - 200,
        columnLines: true,
        id: 'replaceVerifyGrid',
        hidden: true,
        autoScroll: true,
        store: replaceListStore,
        columns: proColumns,
        listeners: {
            viewready: function (grid) {
                setShow(replaceVerifyGrid, 'colName');
            },
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        tbar: [{
            text: PRODUCT_APPLY,
            id: 'btnApply',
            colName: 'btnApply',
            hidden: true,
            disabled: true,
            iconCls: 'icon-upload-local',
            handler: function () {
                var rows = Ext.getCmp("replaceVerifyGrid").getSelectionModel().getSelection();
                if (rows.length == 0) {
                    Ext.Msg.alert(INFORMATION, NO_SELECTION);
                    return false;
                }
                veryfiyConfirm(replaceListStore, rows, 'btnApply');
            }
        }, '->', { text: ''}],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: replaceListStore,
            pageSize: pageSize,
            displayInfo: true
        })
    });


    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        border: false,
        items: [frm, replaceVerifyGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                clientHeight = document.documentElement.clientHeight;
                clientWidth = document.documentElement.clientWidth;
                if (clientWidth <= 1185) {
                    Ext.getCmp('replaceVerifyGrid').setHeight(clientHeight - 200 - 20);
                }
                else {
                    Ext.getCmp('replaceVerifyGrid').setHeight(clientHeight - 200);
                }

                this.doLayout();
            },
            afterrender: function (view) {
                ToolAuthorityQuery(function () {
                    window.setTimeout(searchShow, 1000);
                });


            }
        }
    });

});
