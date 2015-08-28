/* 
* Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司
* All rights reserved. 
*  
* 文件名称：VendorVerifyList.js 
* 摘   要：From Vendor Copy
*  供應商商品審核列表
* 当前版本：1.0 
* 作   者：shuangshuang0420j 
* 完成日期：2014/8/18 16:17:56 
*/
var winDetail;
var productId;
/*********參數表model***********/
Ext.define("gigade.paraModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'parameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
});
/******frm中的item*******/
Ext.define("gigade.Brand", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "brand_id", type: "string" },
        { name: "brand_name", type: "string"}]
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
var brand = {
    xtype: 'combobox',
    fieldLabel: BRAND,
    store: brandStore,
    id: 'brand_id',
    colName: 'brand_id',
    name: 'brand_id',
    displayField: 'brand_name',
    typeAhead: true,
    queryMode: 'local',
    valueField: 'brand_id'
};

//品類分類store
var proCateStore = Ext.create('Ext.data.TreeStore', {
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/ProductList/GetProCategory',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post'
    }
});
//前台分類store
var frontCateStore = Ext.create('Ext.data.TreeStore', {
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/ProductList/GetFrontCatagory',
        noCache: false,
        getMethod: function () { return 'get'; },
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
var category = {
    xtype: 'fieldcontainer',
    layout: 'hbox',
    anchor: '100%',
    items: [{
        xtype: 'combotree',
        id: 'comboProCate',
        fieldLabel: CATEGORY,
        colName: 'cate_id',
        editable: false,
        submitValue: false,
        store: proCateStore
    }, {
        xtype: 'textfield',
        hidden: true,
        id: 'comboProCate_hide',
        name: 'comboProCate_hide'
    }, {
        xtype: 'combotree',
        id: 'comboFrontCage',
        margin: '0 20px',
        editable: false,
        submitValue: false,
        colName: 'category_id',
        store: frontCateStore,
        fieldLabel: FRONT_CATEGORY
    }, {
        hidden: true,
        xtype: 'textfield',
        id: 'comboFrontCage_hide',
        name: 'comboFrontCage_hide'
    }]
};

//商品類型Store
var ComboStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryPara?paraType=combo_type',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});
////申請類型Store
//var ApplyTypeStore = Ext.create('Ext.data.Store', {
//    fields: ["name", "code"],
//    data: [{ name: '全部', code: '-1' },{ name: '供應商新建商品', code: '20' }, { name: '新建商品', code: '0' }, { name: '下架', code: '6'}]
//});
//商品類型
var type_apply = Ext.create('Ext.form.FieldContainer', {
    layout: 'hbox',
    anchor: '100%',
    items: [{
        xtype: 'combobox',
        fieldLabel: PRODUCT_TYPE,
        store: ComboStore,
        editable: false,
        id: 'combination',
        colName: 'combination',
        queryMode: 'local',
        displayField: 'parameterName',
        valueField: 'parameterCode',
        listeners: {
            beforerender: function () {
                ComboStore.load({
                    callback: function () {
                        ComboStore.insert(0, { parameterCode: '0', parameterName: '全部' });
                        Ext.getCmp("combination").setValue(ComboStore.data.items[0].data.parameterCode);
                    }
                });
            }
        }
    }
//    , {
//        xtype: 'combobox',
//        id: 'prev_status',
//        margin: '0 20px',
//        fieldLabel: APPLY_TYPE,
//        colName: 'prev_status',
//        queryMode: 'local',
//        editable: false,
//        store: ApplyTypeStore,
//        displayField: 'name',
//        valueField: 'code',
//        value: '-1'
//    }
    ]
});

//日期條件
var r_all = { name: DATE_ALL, code: '' },
    //r_apply = { name: ASK_FOR_DATE, code: 'apply_time' },
    r_create = { name: PRODUCT_CREATE, code: 'product_createdate' },
    r_start = { name: PRODUCT_START, code: 'product_start' },
    r_end = { name: PRODUCT_END, code: 'product_end' };
//日期條件store
var DateStore = Ext.create("Ext.data.Store", {
    fields: ["name", "code"],
    data: [r_all]
})
//添加日期類型
function addDateType() {
    if (DateStore.getCount() == 1) {
        //DateStore.add(r_apply, r_start, r_end);
        DateStore.add( r_start, r_end);
    }
}
Ext.apply(Ext.form.field.VTypes, {
    //日期筛选
    daterange: function (val, field) {
        var date = field.parseDate(val);
        if (!date) {
            return false;
        }
        if (field.startDateField && (!this.dateRangeMax || (date.getTime() != this.dateRangeMax.getTime()))) {
            var start = field.up('form').down('#' + field.startDateField);
            start.setMaxValue(date);
            start.validate();
            this.dateRangeMax = date;
        }
        else if (field.endDateField && (!this.dateRangeMin || (date.getTime() != this.dateRangeMin.getTime()))) {
            var end = field.up('form').down('#' + field.endDateField);
            end.setMinValue(date);
            end.validate();
            this.dateRangeMin = date;
        }
        return true;
    },
    daterangeText: START_BEFORE_END

});
var start_end = {
    xtype: 'fieldcontainer',
    layout: 'hbox',
    anchor: '100%',
    fieldLabel: DATE_TYPE,
    colName: 'date_type',
    items: [{
        xtype: 'combobox',
        store: DateStore,
        id: 'date_type',
        name: 'date_type',
        displayField: 'name',
        valueField: 'code',
        editable: false,
        queryMode: 'local',
        listeners: {
            afterrender: function () {
                this.setValue(DateStore.data.items[0].data.code);
            },
            beforerender: function () {
                addDateType();
            }
        }
    }, {
        xtype: 'datefield',
        id: 'time_start',
        name: 'time_start',
        margin: '0 5px',
        editable: false,
        vtype: 'daterange',
        endDateField: 'time_end'
    }, {
        xtype: 'displayfield',
        value: '~'
    }, {
        xtype: 'datefield',
        id: 'time_end',
        name: 'time_end',
        margin: '0 5px',
        editable: false,
        vtype: 'daterange',
        startDateField: 'time_start'
    }]
};

//名稱/編號搜尋
var key_query = {
    xtype: 'textfield',
    fieldLabel: NAME_NUM_QUERY,
    id: 'key',
    colName: 'key',
    submitValue: false,
    name: 'key'
};
/******end frm中的item*******/


/*******************GRID COLUMNS***********************/
//商品基本信息
var c_pro_base_byproduct = [
    { header: PRODUCT_IMAGE, colName: 'product_image', hidden: true, xtype: 'templatecolumn', tpl: '<div style="width:50px;height:50px"><img width="50px" height="50px" src="{product_image}" /><div>', width: 60, align: 'center', sortable: false, menuDisabled: true },
    { header: PRODUCT_ID, colName: 'product_id', hidden: true, xtype: 'templatecolumn', tpl: '<a href=# onclick=' + 'javascript:showDetail("{product_id}")' + ' >{product_id}</a>', width: 60, align: 'center', sortable: false, menuDisabled: true },
    { header: PRODUCT_NAME, colName: 'product_name', hidden: true, dataIndex: 'product_name', width: 180, align: 'left', sortable: false, menuDisabled: true },
    { header: BRAND_NAME, colName: 'brand_id', hidden: true, dataIndex: 'brand_name', width: 120, align: 'left', sortable: false, menuDisabled: true }
];
//商品類型
var c_pro_type = [{
    header: PRODUCT_TYPE, colName: 'combination', hidden: true, dataIndex: 'combination', width: 80, align: 'center', sortable: false, menuDisabled: true
}, { header: "combination_id", hidden: true, dataIndex: 'combination_id' }
];
/*建議售價*/
var c_pro_pricelist = {
    header: PRODUCT_PRICE_LIST, colName: 'product_price_list', hidden: true, dataIndex: 'product_price_list', width: 80, align: 'center', sortable: false, menuDisabled: true
};
//商品狀態
var c_pro_status = {
    header: PRODUCT_STATUS, colName: 'product_status', hidden: true, dataIndex: 'product_status', width: 90, align: 'center', sortable: false, menuDisabled: true
};
//運送方式
var c_pro_freight = {
    header: PRODUCT_FREIGHT_SET, colName: 'product_freight_set', hidden: true, dataIndex: 'product_freight_set', width: 60, align: 'center', sortable: false, menuDisabled: true
};
//出貨方式
var c_pro_mode = {
    header: PRODUCT_MODE, colName: 'product_mode', hidden: true, dataIndex: 'product_mode', width: 60, align: 'center', sortable: false, menuDisabled: true
};
//營業稅
var c_pro_tax = {
    header: TAX_TYPE, colName: 'tax_type', hidden: true, dataIndex: 'tax_type', width: 60, align: 'center',
    renderer: function (val) { switch (val) { case '1': return TAX_NEED; case '3': return TAX_NO } }, sortable: false, menuDisabled: true
};
//上架時間
var c_pro_start = {
    header: PRODUCT_START, colName: 'product_start', hidden: true, dataIndex: 'product_start', width: 120, align: 'center',
    renderer: function (val) { return val == '0' ? '' : Ext.Date.format(new Date(val * 1000), 'Y/m/d H:i:s'); },
    sortable: false, menuDisabled: true
};
//下架時間
var c_pro_end = {
    header: PRODUCT_END, colName: 'product_end', hidden: true, dataIndex: 'product_end', width: 120, align: 'center',
    renderer: function (val) { return val == '0' ? '' : Ext.Date.format(new Date(val * 1000), 'Y/m/d H:i:s'); },
    sortable: false, menuDisabled: true
};
///*申請時間*/
//var c_pro_askfordate = {
//    header: ASK_FOR_DATE, hidden: true, colName: 'apply_time', dataIndex: 'apply_time', width: 120, align: 'center', sortable: false, menuDisabled: true, renderer: function (value) {
//        value = value.substring(value.lastIndexOf('(') + 1, value.lastIndexOf(')'));
//        value = Ext.Date.format(new Date(eval(value)), 'Y-m-d H:i');
//        return value;
//    }
//}
/*審核后即刻上架*/
var c_pro_up_now = {
    header: UP_NOW, dataIndex: 'online_mode', colName: 'online_mode', width: 80, align: 'center', sortable: false, menuDisabled: true, hidden: true,
    renderer: function (val) {
        switch (val) {
            case '2':
                return YES;
            default:
                return NO;
        }
    }
}
/*建立人*/
var c_pro_kuser = {
    header: PRODUCT_KUSER, colName: 'kuser', hidden: true, dataIndex: 'user_name', width: 80, align: 'center', sortable: false, menuDisabled: true
}
/*申請類型*/
var c_pro_applytype = {
    header: APPLY_TYPE, colName: 'prev_status', hidden: true, dataIndex: 'prev_status', width: 120, align: 'center', sortable: false, menuDisabled: true
}
/*******************END GRID COLUMNS***********************/

function showDetail(product_id) {

    productId = product_id;
    if (winDetail == undefined) {
        winDetail = Ext.create('Ext.window.Window', {
            title: '商品詳細資料',
            constrain: true,
            modal: true,
            resizable: false,
            height: document.documentElement.clientHeight * 565 / 783,
            width: 1000,
            autoScroll: false,
            layout: 'fit',
            html: "<iframe scrolling='no' frameborder=0 width=100% height=100% src='/ProductVendorList/ProductDetails'></iframe>",
            listeners: {
                close: function (e) {
                    winDetail = undefined;
                    tabs = new Array();
                }
            }
        }).show();
    }

}

function GetProductId() {
    return productId;
}

/******************************窗口佈局**********************************/
var pageSize = 25;

//列表要顯示的列
var columns = c_pro_base_byproduct;
columns.push(c_pro_kuser);          //建立人
columns.push(c_pro_type);           //商品類型
columns.push(c_pro_pricelist);      //建議售價
columns.push(c_pro_applytype);      //申請類型
columns.push(c_pro_status);         //商品狀態
columns.push(c_pro_freight);        //運送方式
columns.push(c_pro_mode);           //出貨方式
columns.push(c_pro_tax);            //營業稅
//columns.push(c_pro_askfordate);     //申請時間 
columns.push(c_pro_start);          //售價
columns.push(c_pro_end);            //活動價
columns.push(c_pro_up_now);          //審核后即刻上架


//待審核商品MODEL
Ext.define('GIGADE.WAITVERIFY', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'product_image', type: 'string' },
        { name: 'product_id', type: 'string' },
        { name: 'brand_name', type: 'string' },
        { name: 'product_name', type: 'string' },
        { name: 'user_name', type: 'string' },
        { name: 'combination_id', type: 'int' },
        { name: 'combination', type: 'string' },
        { name: 'product_price_list', type: 'string' },
        { name: 'prev_status', type: 'string' },
        { name: 'product_status', type: 'string' },
        { name: 'product_freight_set', type: 'string' },
        { name: 'product_mode', type: 'string' },
        { name: 'tax_type', type: 'string' },
        { name: 'apply_time', type: 'string' },
        { name: 'online_mode', type: 'string' },
        { name: 'product_start', type: 'string' },
        { name: 'product_end', type: 'string' },
        { name: 'CanDo', type: 'string' }
    ]
});

//待審核商品的store
var waitVerifyStore = Ext.create("Ext.data.Store", {
    model: 'GIGADE.WAITVERIFY',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/ProductVendorList/waitVerifyQuery',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

//待審核商品store加載前須獲得的條件
waitVerifyStore.on("beforeload", function () {
    Ext.apply(waitVerifyStore.proxy.extraParams,
             {
                 brand_id: Ext.getCmp("brand_id").getValue(),
                 cate_id: Ext.getCmp('comboProCate_hide').getValue(),
                 category_id: Ext.getCmp('comboFrontCage_hide').getValue(),
                 combination: Ext.getCmp("combination").getValue(),
                 //prev_status: Ext.getCmp('prev_status').getValue(),    //申請類型
                 prev_status: 20,    //申請類型
                 date_type: Ext.getCmp("date_type").getValue(),
                 time_start: Ext.getCmp("time_start").getValue(),
                 time_end: Ext.getCmp("time_end").getValue(),
                 key: Ext.htmlEncode(Ext.getCmp("key").getValue())
             });
});

//執行駁回的方法
var back = function (row, reason, functionid) {
    var result = '';
    for (var i = 0, j = row.length; i < j; i++) {
        if (i > 0) {
            result += ',';
        }
        result += row[i].data.product_id;
    }

    Ext.Ajax.request({
        url: '/ProductVendorList/vaiteVerifyBack',
        params: {
            productIds: result//,
            //backReason: reason,
            //"function": functionid
        },
        success: function (response) {
            var result = Ext.decode(response.responseText);
            if (result.success) {
                Ext.Msg.alert(INFORMATION, SUCCESS, function () {
                    waitVerifyStore.loadPage(1);
                });
            }
            else {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        }
    });
}

//核可
var btnPassOnClick = function () {
    var rows = Ext.getCmp("waitVerifyGrid").getSelectionModel().getSelection();
    if (rows.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
        return false;
    }
    var result = '';
    for (var i = 0, j = rows.length; i < j; i++) {
        if (i > 0) {
            result += ',';
        }
        result += rows[i].data.product_id;
    }
    Ext.Ajax.request({
        url: '/ProductVendorList/vaiteVerifyPass',
        params: {
            prodcutIdStr: result
        },
        success: function (response) {
            var result = Ext.decode(response.responseText);
            if (result.success) {
                Ext.Msg.alert(INFORMATION, SUCCESS)
                waitVerifyStore.loadPage(1);
            } else {
                Ext.Msg.alert(INFORMATION, result.msg);
            }
        }
    });
}
//駁回按鈕才觸發事件
var btnBackOnClick = function () {
    var row = Ext.getCmp("waitVerifyGrid").getSelectionModel().getSelection();
    if (row.length == 0) {//未選中任何行
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
        return false;
    }

    Ext.MessageBox.show({
        title: BACK_REASON, //駁回原因
        id: 'txtReason',
        msg: INPUT_BACK_REASON, //請填寫駁回原因
        width: 300,
        buttons: Ext.MessageBox.OKCANCEL,
        multiline: true,
        fn: function (btn, text) {
            if (btn == "cancel") {
                return false;
            }
            back(row, text, 'btnBack');
        },
        animateTarget: 'btnBack'
    });
}

//執行查詢操作
var query = function () {
    waitVerifyStore.loadPage(1);
}
//頁面執行1秒后執行的操作
function searchShow() {
    Ext.getCmp('waitVerifyGrid').show();
    query();
}



Ext.onReady(function () {
    //按enter鍵執行查詢操作
    document.body.onkeydown = function () {
        if (event.keyCode == 13) {
            $("#btnSearch").click();
        }
    };
    var frm = Ext.create('Ext.form.Panel', {
        layout: 'column',
        border: false,
        width: 1185,
        margin: '0 0 10 0',
        defaults: { anchor: '95%', msgTarget: "side", padding: '5 5' },
        items: [{
            xtype: 'panel',
            columnWidth: 1,
            border: 0,
            layout: 'anchor',
            items: [brand, category, type_apply, start_end, key_query]//查詢條件
        }],
        buttonAlign: 'center',
        buttons: [{
            text: SEARCH,
            id: 'btnSearch',
            colName: 'btnSearch',
            iconCls: 'ui-icon ui-icon-search-2',
            handler: function () {
                Ext.getCmp('waitVerifyGrid').show();
                query();
            }
        }, {
            text: RESET,
            id: 'btn_reset',
            iconCls: 'ui-icon ui-icon-reset',
            listeners: {
                click: function () {
                    frm.getForm().reset();
                    Ext.getCmp("combination").setValue(ComboStore.data.items[0].data.parameterCode);
                  //  Ext.getCmp("prev_status").setValue(ApplyTypeStore.data.items[0].data.code);
                    Ext.getCmp("date_type").setValue(DateStore.data.items[0].data.code);
                }
            }
        }]
    });


    //列选择模式
    var sm = Ext.create('Ext.selection.CheckboxModel', {
        //mode: 'SINGLE',
        storeColNameForHide: 'CanDo',
        listeners: {
            selectionchange: function (sm, selections) {
                Ext.getCmp("waitVerifyGrid").down('#btnPass').setDisabled(selections.length == 0); //核可
                Ext.getCmp("waitVerifyGrid").down('#btnBack').setDisabled(selections.length == 0); //駁回
                //  Ext.getCmp("waitVerifyGrid").down('#last_modify').setDisabled(selections.length == 0);//上次修改
            }
        }
    });


    var grid = Ext.create("Ext.grid.Panel", {
        id: 'waitVerifyGrid',
        selModel: sm,
        height: document.documentElement.clientWidth <= 1185 ? document.documentElement.clientHeight - 200 - 20 : document.documentElement.clientHeight - 200,
        columnLines: true,
        store: waitVerifyStore,
        columns: columns,
        hidden: true,
        autoScroll: true,
        listeners: {
            viewready: function (grid) {
                setShow(grid, 'colName');
            }
        },
        tbar: [{
            text: VERIFY_PASS, //核可
            id: 'btnPass',
            colName: 'btnPass',
            hidden: true,
            disabled: true,
            iconCls: 'icon-accept',
            handler: btnPassOnClick

        }, {
            text: VERIFY_BACK, //駁回
            colName: 'btnBack',
            disabled: true,
            hidden: true,
            id: 'btnBack',
            icon: '../../../Content/img/icons/drop-no.gif',
            handler: btnBackOnClick
        }
        , '->', { text: ''}],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: waitVerifyStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        })
    });


    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [frm, grid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                if (document.documentElement.clientWidth <= 1185) {
                    Ext.getCmp('waitVerifyGrid').setHeight(document.documentElement.clientHeight - 200 - 20);
                }
                else {
                    Ext.getCmp('waitVerifyGrid').setHeight(document.documentElement.clientHeight - 200);
                }

                this.doLayout();
            },
            afterrender: function (view) {
                ToolAuthorityQuery(function () {
                    setShow(frm, 'colName');
                    window.setTimeout(searchShow, 1000); //頁面加載1秒后執行查詢
                });
            }
        }
    });
});

