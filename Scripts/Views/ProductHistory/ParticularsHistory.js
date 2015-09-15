//商品細項管理資料記錄 model
Ext.define('gigade.particularsHistory', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'Product_id', type: 'int' },
        { name: 'Product_name', type: 'string' },
        { name: 'Item_id', type: 'int' },//6碼ID
        { name: 'Pend_del_bool', type: 'bool' },
        { name: 'Spec_name', type: 'string' },
        { name: 'Brand_name', type: 'string' },//商品品牌
        { name: 'User_mail', type: 'string' },//創建人
        { name: 'Product_createdate', type: 'int' },//創建時間
        { name: 'Kuser', type: 'string' },//修改人
        { name: 'Kdate', type: 'int' },//修改時間
        { name: 'Col_name', type: 'string' },//修改的欄位名稱
        { name: 'Incr_old', type: 'string' },//(保存期限)修改前值
        { name: 'Shp_old', type: 'string' },//(允出天數)修改前值
        { name: 'Var_old', type: 'string' },//(允收天數)修改前值
        { name: 'Batchno', type: 'string' },//修改的批次號
        { name: 'Var_value', type: 'string' },//(允收天數)修改後值
        { name: 'Shp_value', type: 'string' },//(允出天數)修改后值
        { name: 'Incr_value', type: 'string' }]//(保存期限)修改后值
});


//價格異動記錄 model
Ext.define('gigade.priceHistory', {
    extend: 'Ext.data.Model',
    fields: [
        { name: '', type: '' },
        { name: '', type: '' },
        { name: '', type: '' },
        { name: '', type: '' },
        { name: '', type: '' },
        { name: '', type: '' },
        { name: '', type: '' },
        { name: '', type: '' },
        { name: '', type: '' },
        { name: '', type: '' },
        { name: '', type: '' },
        { name: '', type: '' },
        { name: '', type: '' },
        { name: '', type: '' },
        { name: '', type: '' },
        { name: '', type: '' },
        { name: '', type: '' },
        { name: '', type: '' },
        { name: '', type: '' },
        { name: '', type: '' }]
});


//商品細項管理資料記錄
var particularsHistoryStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    autoLoad: false,
    model: 'gigade.particularsHistory',
    proxy: {
        type: 'ajax',
        url: '/ProductHistory/GetParticularsHistory',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
});

//價格異動記錄
var priceHistoryStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    autoLoad: false,
    model: 'gigade.priceHistory',
    proxy: {
        type: 'ajax',
        url: '/ProductHistory/GetPricesHistory',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
});

//which
var searchComboxStore = Ext.create('Ext.data.Store', {
    fields: ['conditionId', 'conditions'],
    data: [
        { "conditionId": "1", "conditions": PRODUCT_DETAIL_DATA_RECORD },//商品細項資料記錄
        { "conditionId": "2", "conditions": PRODUCT_PRICE_DATA_RECORD }//商品價格資料記錄
    ]
});


//品牌Model
Ext.define("gigade.Brand", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "brand_id", type: "string" },
        { name: "brand_name", type: "string" }]
});



//品牌store
var brandStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Brand',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Brand/QueryBrand",
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'item'
        }
    }
});



Ext.onReady(function () {

    document.body.onkeydown = function () {
        if (event.keyCode == 13) {
            $("#search").click();
        }
    };

    var frm = Ext.create('Ext.form.Panel', {
        layout: 'column',
        id: 'frm',
        frame: true,
        items: [{
            xtype: 'combobox',
            fieldLabel: DATA_SORT,//資料類別
            labelWidth: 60,
            value: '1',
            margin: '0 6px',
            store: searchComboxStore,
            id: 'contentType',
            displayField: 'conditions',
            valueField: 'conditionId',
            listeners: {
                change: function () {
                    if (Ext.getCmp('contentType').getValue() == 1) {
                        Ext.getCmp('brand_id').show();
                    } else {
                        Ext.getCmp('brand_id').hide();
                        Ext.getCmp('outdate').hide();
                    }
                }
            }
        }, {
            xtype: 'combobox',
            fieldLabel: BRAND,//品牌
            store: brandStore,
            id: 'brand_id',
            labelWidth: 35,
            margin: '0 6px',
            displayField: 'brand_name',
            //typeAhead: true,
            queryMode: 'local',
            valueField: 'brand_id'
        }, {
            xtype: 'textfield',
            id: 'productID_5',
            fieldLabel: PRODUCT_ID_5_CODE,//商品編號_5碼
            margin: '0 6px',
            labelWidth: 85
        }, {
            xtype: 'textfield',
            id: 'productID_6',
            fieldLabel: PRODUCT_ID_6_CODE,//商品編號_6碼
            margin: '0 6px',
            labelWidth: 85
        }, {
            xtype: 'datefield',
            fieldLabel: EDIT_DATE,//修改時間
            labelWidth: 60,
            id: 'time_start',
            name: 'time_start',
            margin: '0 6px',
            editable: false
        }, {
            xtype: 'displayfield',
            value: '~'
        }, {
            xtype: 'datefield',
            id: 'time_end',
            name: 'time_end',
            margin: '0 6px',
            editable: false
        }, {
            xtype: 'button',
            id: 'search',
            text: QUERY,//查 詢
            margin: '0 0 0 15',
            iconCls: 'ui-icon ui-icon-search-2 ',
            handler: search
        }, {
            xtype: 'button',
            text: RESET,//重置
            id: 'reset',
            margin: '0 0 0 10',
            iconCls: 'ui-icon ui-icon-reset',
            listeners: {
                click: function () {
                    this.up('form').getForm().reset();
                }
            }
        }, {
            xtype: 'button',
            text: EXPORT_DATA,//匯出資料
            id: 'outdate',
            margin: '0 0 0 20',
            iconCls: 'ui-icon ui-icon-paper-out',
            handler: outexcel
        }]
    })

    var particularsHistoryGrid = Ext.create('Ext.grid.Panel', {
        id: 'particularsHistoryGrid',
        store: particularsHistoryStore,
        hidden: true,
        columnLines: true,
        //width: 1650,
        //frame: true,
        columns: [
        { header: SEQUENCE_NUMBER, xtype: 'rownumberer', width: 40, align: 'center' },//序號
        { header: PRODUCT_BRAND, dataIndex: 'Brand_name', width: 180, align: 'center', menuDisabled: true, sortable: false },//商品品牌
        { header: PRODUCT_ID_5_CODE, dataIndex: 'Product_id', width: 90, align: 'center', menuDisabled: true, sortable: false },//商品編號_5碼
        { header: PRODUCT_DETAIL_ID_6_CODE, dataIndex: 'Item_id', width: 110, align: 'center', menuDisabled: true, sortable: false },//商品細項編號_6碼
        { header: PRODUCT_NAME, dataIndex: 'Product_name', width: 60, align: 'center', menuDisabled: true, sortable: false, flex: 1 },//商品名稱
        { header: PRODUCT_KUSER, dataIndex: 'User_mail', width: 80, align: 'center', menuDisabled: true, sortable: false },//建立人
        { header: PRODUCT_CREATE, dataIndex: 'Product_createdate', width: 150, align: 'center', menuDisabled: true, sortable: false },//建立時間
        { header: MODIFICATION_MAN, dataIndex: 'Kuser', width: 80, align: 'center', menuDisabled: true, sortable: false },//修改人
        { header: EDIT_DATE, dataIndex: 'Kdate', width: 150, align: 'center', menuDisabled: true, sortable: false },//修改時間
        {
            text: EDIT_BEFOR_VALUE, menuDisabled: true, sortable: false, columns: [//修改前的值
                { header: SAVE_TIME_LIMIT, dataIndex: 'Incr_old', width: 85, align: 'center', menuDisabled: true, sortable: false },//保存期限
                { header: ALLOW_SELL_DAY, dataIndex: 'Shp_old', width: 85, align: 'center', menuDisabled: true, sortable: false },//允出天數
                { header: ALLOW_PURCHASE_DAY, dataIndex: 'Var_old', width: 85, align: 'center', menuDisabled: true, sortable: false }]//允收天數
        },
        {
            text: EDIT_AFTER_VALUE, menuDisabled: true, sortable: false, columns: [//修改後的值
                { header: SAVE_TIME_LIMIT, dataIndex: 'Incr_value', width: 85, align: 'center', menuDisabled: true, sortable: false },//保存期限
                { header: ALLOW_SELL_DAY, dataIndex: 'Shp_value', width: 85, align: 'center', menuDisabled: true, sortable: false },//允出天數
                { header: ALLOW_PURCHASE_DAY, dataIndex: 'Var_value', width: 85, align: 'center', menuDisabled: true, sortable: false }]//允收天數
        }]
    })


    var priceHistoryGrid = Ext.create('Ext.grid.Panel', {
        id: 'priceHistoryGrid',
        store: priceHistoryStore,
        hidden: true,
        columnLines: true,
        //width: 1650,
        //frame: true,
        columns: [
        { header: SEQUENCE_NUMBER, xtype: 'rownumberer', width: 40, align: 'center' },//序號
        { header: PRODUCT_ID_5_CODE, dataIndex: '', width: 90, align: 'center', menuDisabled: true, sortable: false },//商品編號_5碼
        { header: PRODUCT_DETAIL_ID_6_CODE, dataIndex: '', width: 110, align: 'center', menuDisabled: true, sortable: false },//商品細項編號_6碼
        { header: PRODUCT_NAME, dataIndex: '', width: 60, align: 'center', menuDisabled: true, sortable: false, flex: 1 },//商品名稱
        { header: MODIFICATION_MAN, dataIndex: '', width: 70, align: 'center', menuDisabled: true, sortable: false },//修改人
        { header: EDIT_DATE, dataIndex: '', width: 150, align: 'center', menuDisabled: true, sortable: false },//修改時間
        {
            text: EDIT_BEFOR_VALUE, menuDisabled: true, sortable: false, columns: [//修改前的值
                { header: ORIGINAL_PRICE, dataIndex: '', width: 90, align: 'center', menuDisabled: true, sortable: false },//原售價
                { header: ORIGINAL_COST, dataIndex: '', width: 90, align: 'center', menuDisabled: true, sortable: false },//原成本
                { header: SINGLE_PRODUCT_EVENT_PRICE, dataIndex: '', width: 90, align: 'center', menuDisabled: true, sortable: false },//活動售價
                { header: SINGLE_PRODUCT_EVENT_COST, dataIndex: '', width: 90, align: 'center', menuDisabled: true, sortable: false }]//活動成本
        },
        {
            text: EDIT_AFTER_VALUE, menuDisabled: true, sortable: false, columns: [//修改後的值
                { header: ORIGINAL_PRICE, dataIndex: '', width: 90, align: 'center', menuDisabled: true, sortable: false },//原售價
                { header: ORIGINAL_COST, dataIndex: '', width: 90, align: 'center', menuDisabled: true, sortable: false },//原成本
                { header: SINGLE_PRODUCT_EVENT_PRICE, dataIndex: '', width: 90, align: 'center', menuDisabled: true, sortable: false },//活動售價
                { header: SINGLE_PRODUCT_EVENT_COST, dataIndex: '', width: 90, align: 'center', menuDisabled: true, sortable: false }]//活動成本
        }]
    })

    Ext.create('Ext.container.Viewport', {
        layout: 'anchor',
        border: false,
        items: [frm, particularsHistoryGrid, priceHistoryGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                //particularsHistoryGrid.height = document.documentElement.clientHeight <= 700 ? document.documentElement.clientHeight - 242 : document.documentElement.clientHeight - 222;
                //priceHistoryGrid.height = particularsHistoryGrid.height;
                this.doLayout();
            }
        }
    });

})


function search() {

    if (!Ext.getCmp('brand_id').getValue() && !Ext.getCmp('productID_5').getValue() && !Ext.getCmp('productID_6').getValue() && !Ext.getCmp('time_start').getValue() && !Ext.getCmp('time_end').getValue()) {
        Ext.Msg.alert(INFORMATION, PLEASE_INPUT_QUERY_CONDITION);//請輸入查詢條件
        return;
    }

    if (Ext.getCmp('time_start').getValue() && Ext.getCmp('time_end').getValue()) {
        if (Ext.getCmp('time_start').getValue().getTime() >= Ext.getCmp('time_end').getValue().getTime()) {
            Ext.Msg.alert(INFORMATION, PLEASE_INPUT_TRUE_DATE_WAIT_AREA);//請輸入正確的時間區域
            return;
        }
    }

    Ext.getCmp('productID_5').setValue(Ext.getCmp('productID_5').getValue().replace(/\s+/g, ','));
    Ext.getCmp('productID_6').setValue(Ext.getCmp('productID_6').getValue().replace(/\s+/g, ','));

    var contentType = Ext.getCmp('contentType').getValue();
    var brand_id = Ext.getCmp('brand_id').getValue();
    var productID_5 = Ext.getCmp('productID_5').getValue();
    var productID_6 = Ext.getCmp('productID_6').getValue();
    var time_start = Ext.getCmp('time_start').getValue();
    var time_end = Ext.getCmp('time_end').getValue();

    if (contentType == 1) {
        Ext.getCmp('particularsHistoryGrid').show();
        Ext.getCmp('priceHistoryGrid').hide();
        particularsHistoryStore.load({
            params: {
                brand_id: brand_id,
                productID_5: productID_5,
                productID_6: productID_6,
                time_start: time_start,
                time_end: time_end
            }
        });
    } else {
        Ext.getCmp('priceHistoryGrid').show();
        Ext.getCmp('particularsHistoryGrid').hide();
        priceHistoryStore.load({
            params: {
                productID_5: productID_5,
                productID_6: productID_6,
                time_start: time_start,
                time_end: time_end
            }
        });
    }
}

function outexcel() {

    if (!Ext.getCmp('brand_id').getValue() && !Ext.getCmp('productID_5').getValue() && !Ext.getCmp('productID_6').getValue() && !Ext.getCmp('time_start').getValue() && !Ext.getCmp('time_end').getValue()) {
        Ext.Msg.alert(INFORMATION, PLEASE_INPUT_QUERY_CONDITION);//請輸入查詢條件
        return;
    }

    if (Ext.getCmp('time_start').getValue() && Ext.getCmp('time_end').getValue()) {
        if (Ext.getCmp('time_start').getValue().getTime() >= Ext.getCmp('time_end').getValue().getTime()) {
            Ext.Msg.alert(INFORMATION, PLEASE_INPUT_TRUE_DATE_WAIT_AREA);//請輸入正確的時間區域
            return;
        }
    }

    Ext.getCmp('productID_5').setValue(Ext.getCmp('productID_5').getValue().replace(/\s+/g, ','));
    Ext.getCmp('productID_6').setValue(Ext.getCmp('productID_6').getValue().replace(/\s+/g, ','));


    var contentType = Ext.getCmp('contentType').getValue();
    var brand_id = Ext.getCmp('brand_id').getValue();
    var productID_5 = Ext.getCmp('productID_5').getValue();
    var productID_6 = Ext.getCmp('productID_6').getValue();
    var time_start = Ext.getCmp('time_start').getValue().getTime();
    var time_end = Ext.getCmp('time_end').getValue().getTime();

    window.open("/ProductHistory/HistoryExcel?brand_id=" + brand_id + "&productID_5=" + productID_5 + "&productID_6=" + productID_6 + "&time_start=" + time_start + "&time_end=" + time_end);
}
