var winDetail;
var productId;


/*******************Store Model***********************/


/*******************GRID COLUMNS**********************/
var c_pro_base = [
    { header: PRODUCT_IMAGE,/* colName: 'product_image', hidden: true,*/ xtype: 'templatecolumn', tpl: '<div style="width:50px;height:50px"><img width="50px" height="50px" src="{product_image}" /><div>', width: 60, align: 'center', sortable: false, menuDisabled: true },
    { header: PRODUCT_ID,/* colName: 'product_id', hidden: true,*/ xtype: 'templatecolumn', tpl: '<a href=# onclick="javascript:showDetail({product_id})" >{product_id}</a>', width: 60, align: 'center', sortable: false, menuDisabled: true },
    {
        header: PRODUCT_NAME,/* colName: 'product_name', hidden: true, */dataIndex: 'product_name', width: 180, align: 'left', sortable: false, menuDisabled: true
        ,
        renderer: function (value, cellmeta, record) {
            return value + (record.data.prod_sz ? ' (' + record.data.prod_sz + ')' : '');
        }
    },
    { header: BRAND_NAME,/* colName: 'brand_id', hidden: true,*/ dataIndex: 'brand_name', width: 120, align: 'left', sortable: false, menuDisabled: true }
];
//商品類型
var c_pro_type = {
    header: PRODUCT_TYPE, /*colName: 'combination', hidden: true,*/ dataIndex: 'combination', width: 80, align: 'center', sortable: false, menuDisabled: true
};
//價格類型 
var c_pro_pricetype = {
    header: PRICE_TYPE, /*colName: 'price_type',*/ dataIndex: 'price_type', width: 150, align: 'center', sortable: false, menuDisabled: true
};
var c_pro_spec = {
    header: PRODUCT_SPEC,/* colName: 'product_spec', hidden: true,*/ dataIndex: 'product_spec', width: 80, align: 'center', sortable: false, menuDisabled: true
};
/*建議售價*/
//var c_pro_pricelist = {
//    header: PRODUCT_PRICE_LIST, colName: 'product_price_list', hidden: true, dataIndex: 'product_price_list', width: 80, align: 'center', sortable: false, menuDisabled: true
//};
var c_pro_status = {
    header: PRODUCT_STATUS,/* colName: 'product_status', hidden: true,*/ dataIndex: 'product_status', width: 90, align: 'center', sortable: false, menuDisabled: true
};
var c_pro_freight = {
    header: PRODUCT_FREIGHT_SET,/* colName: 'product_freight_set', hidden: true,*/ dataIndex: 'product_freight_set', width: 60, align: 'center', sortable: false, menuDisabled: true
};
var c_pro_mode = {
    header: PRODUCT_MODE,/* colName: 'product_mode', hidden: true,*/ dataIndex: 'product_mode', width: 60, align: 'center', sortable: false, menuDisabled: true
};
var c_pro_tax = {
    header: TAX_TYPE,/* colName: 'tax_type', hidden: true,*/ dataIndex: 'tax_type', width: 60, align: 'center',
    renderer: function (val) { switch (val) { case '1': return TAX_NEED; case '3': return TAX_NO } }, sortable: false, menuDisabled: true
};
//var c_pro_sort = {
//    header: PRODUCT_SORT,/* colName: 'product_sort', hidden: true,*/ dataIndex: 'product_sort', width: 60, align: 'center', sortable: false, menuDisabled: true
//};
var c_pro_create = {
    header: PRODUCT_CREATE, /*colName: 'product_createdate', hidden: true,*/ dataIndex: 'product_createdate', width: 120, align: 'center',
    renderer: function (val) { return val == '0' ? '' : Ext.Date.format(new Date(val * 1000), 'Y/m/d H:i:s'); },
    sortable: false, menuDisabled: true
};
var c_pro_start = {
    header: PRODUCT_START, /*colName: 'product_start', hidden: true,*/ dataIndex: 'product_start', width: 120, align: 'center',
    renderer: function (val) { return val == '0' ? '' : Ext.Date.format(new Date(val * 1000), 'Y/m/d H:i:s'); },
    sortable: false, menuDisabled: true
};
var c_pro_end = {
    header: PRODUCT_END,/* colName: 'product_end', hidden: true, */dataIndex: 'product_end', width: 120, align: 'center',
    renderer: function (val) { return val == '0' ? '' : Ext.Date.format(new Date(val * 1000), 'Y/m/d H:i:s'); },
    sortable: false, menuDisabled: true
};
/************************************************************************************************************************************************************************/


/*********參數表model***********/
Ext.define("gigade.paraModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'parameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
});
/*********end*******************/
//數據源store
/**********model&store**************/
Ext.define('GIGADE.SITE', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'Site_Id', type: 'string' },
        { name: 'Site_Name', type: 'string' }
    ]
});



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



//商品狀態Store
var prodStatusStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    autoLoad: false,
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
})


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

//運送方式
var freightStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryPara?paraType=product_freight',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//出貨方式
var modeStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryPara?paraType=product_mode',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//營業稅
var taxStore = Ext.create('Ext.data.Store', {
    fields: ['text', 'value'],
    data: [
        { "text": VALUE_ALL, "value": "0" },
        { "text": TAX_NEED, "value": "1" },
        { "text": TAX_NO, "value": "3" }
    ]
});

//申請類型Store
var ApplyTypeStore = Ext.create('Ext.data.Store', {
    fields: ["name", "code"],
    data: [{ name: VALUE_ALL, code: '-1' }, { name: PRODUCT_NEW, code: '0' }, { name: PRODUCT_DOWN, code: '6' }]//新建商品,下架
});

var r_all = { name: DATE_ALL, code: '' },
    r_apply = { name: ASK_FOR_DATE, code: 'apply_time' },
    r_create = { name: PRODUCT_CREATE, code: 'product_createdate' },
    r_start = { name: PRODUCT_START, code: 'product_start' },
    r_end = { name: PRODUCT_END, code: 'product_end' };
//日期條件store
var DateStore = Ext.create("Ext.data.Store", {
    fields: ["name", "code"],
    data: [r_all]
})

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

//add by zhuoqin0830w  2015/03/11  已買斷商品的篩選功能
var productPrepaidStore = Ext.create('Ext.data.Store', {
    id: 'productPrepaidStore',
    autoDestroy: true,
    fields: ['ParameterCode', 'parameterName'],
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryParaByXml?paraType=productPrepaid',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});


/*****************end*******************************/


//單選按鈕
var radioCondition = {
    xtype: 'radiogroup',
    id: 'chock',
    name: 'chock',
    colName: 'chock',
    checkChange: function () {
        Ext.getCmp('tools').setDisabled(true);//add 2014/08/27
    },
    items: [{ boxLabel: SINGLE_PRICE_AS_DISCOUNT_EDIT, name: 'rb', inputValue: '1', checked: true },//價格依折數修改
            { boxLabel: SINGLE_PRICE_AS_DISCOUNT_INPUT, name: 'rb', inputValue: '2' }]//價格各別輸入
}





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

var category = {
    xtype: 'fieldcontainer',
    layout: 'hbox',
    anchor: '100%',
    items: [{
        xtype: 'combotree',
        id: 'comboProCate',
        fieldLabel: CATEGORY,
        colName: 'cate_id',
        //hidden: true,
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
        //hidden: true,
        //width: 300,
        store: frontCateStore,
        fieldLabel: FRONT_CATEGORY
    }, {
        hidden: true,
        xtype: 'textfield',
        id: 'comboFrontCage_hide',
        name: 'comboFrontCage_hide'
    }]
};



var type = {
    xtype: 'combobox',
    fieldLabel: PRODUCT_TYPE,
    id: 'combination',
    name: 'combination',
    colName: 'combination',
    //hidden: true,
    store: ComboStore,
    queryMode: 'local',
    displayField: 'parameterName',
    valueField: 'parameterCode',
    editable: false,
    listeners: {
        beforerender: function () {
            ComboStore.load({
                callback: function () {
                    ComboStore.insert(0, { parameterCode: '0', parameterName: VALUE_ALL });
                    Ext.getCmp("combination").setValue(ComboStore.data.items[0].data.parameterCode);
                }
            });
        }
    }
};


var pro_status = {
    xtype: 'combobox',
    margin: '0 20px',
    fieldLabel: PRODUCT_STATUS,
    store: prodStatusStore,
    id: 'product_status',
    name: 'product_status',
    queryMode: 'local',
    displayField: 'parameterName',
    valueField: 'parameterCode',
    colName: 'product_status',
    //hidden: true,
    editable: false,
    listeners: {
        beforerender: function () {
            prodStatusStore.load({
                callback: function () {
                    prodStatusStore.insert(0, { parameterCode: '-1', parameterName: VALUE_ALL });
                    Ext.getCmp("product_status").setValue(prodStatusStore.data.items[0].data.parameterCode);
                }
            });
        }
    }
};

var type_status = {
    xtype: 'fieldcontainer',
    layout: 'hbox',
    anchor: '100%',
    items: [type, pro_status]
};


var freight_mode_tax = {
    xtype: 'fieldcontainer',
    layout: 'hbox',
    anchor: '100%',
    items: [{
        xtype: 'combobox',
        id: 'product_freight_set',
        name: 'product_freight_set',
        store: freightStore,
        queryMode: 'local',
        displayField: 'parameterName',
        valueField: 'parameterCode',
        fieldLabel: PRODUCT_FREIGHT_SET,
        colName: 'product_freight_set',
        //hidden: true,
        editable: false,
        listeners: {
            beforerender: function () {
                freightStore.load({
                    callback: function () {
                        freightStore.insert(0, { parameterCode: '0', parameterName: VALUE_ALL });
                        Ext.getCmp("product_freight_set").setValue(freightStore.getAt(0).data.parameterCode);
                    }
                });
            }
        }
    }, {
        xtype: 'combobox',
        margin: '0 20px',
        id: 'product_mode',
        name: 'product_mode',
        store: modeStore,
        queryMode: 'local',
        displayField: 'parameterName',
        valueField: 'parameterCode',
        fieldLabel: PRODUCT_MODE,
        colName: 'product_mode',
        //hidden: true,
        editable: false,
        listeners: {
            beforerender: function () {
                modeStore.load({
                    callback: function () {
                        modeStore.insert(0, { parameterCode: '0', parameterName: VALUE_ALL });
                        Ext.getCmp("product_mode").setValue(modeStore.getAt(0).data.parameterCode);
                    }
                });
            }
        }
    }, {
        xtype: 'combobox',
        margin: '0 20px',
        id: 'tax_type',
        name: 'tax_type',
        width: 180,
        labelWidth: 50,
        store: taxStore,
        queryMode: 'local',
        displayField: 'text',
        valueField: 'value',
        value: '0',
        editable: false,
        colName: 'tax_type',
        //hidden: true,
        fieldLabel: TAX_TYPE
    }]
};

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
    //hidden: true,
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
        xtype: 'datetimefield',
        format: 'Y/m/d H:i:s',
        time:{hour:00,min:00,sec:00},
        id: 'time_start',
        name: 'time_start',
        margin: '0 5px',
        editable: false,
        listeners: {//change by shiwei0620j 20151217,將時間控件改為可以選擇時分秒，開始時間時分秒默認為00:00:00,結束時間時分秒默認為23:59:59，當選擇的開始時間大於結束時間，結束時間在開始時間月份加1，當選擇的結束時間大於開始時間，開始時間在結束時間月份加1;
            select: function (a, b, c) {
                var start = Ext.getCmp("time_start");
                var end = Ext.getCmp("time_end");
                var start_date = start.getValue();
                if (end.getValue() == ""||end.getValue() ==null) {
                    Ext.getCmp('time_end').setValue(new Date(start_date.getFullYear(), start_date.getMonth() + 1, start_date.getDate(), 23, 59, 59));
                }
                else if (end.getValue() < start.getValue()) {
                    Ext.getCmp('time_end').setValue(new Date(start_date.getFullYear(), start_date.getMonth() + 1, start_date.getDate(), 23, 59, 59));
                }
            }
        }
      //  vtype: 'daterange',
       // endDateField: 'time_end'
    }, {
        xtype: 'displayfield',
        value: '~'
    }, {
        xtype: 'datetimefield',
        format: 'Y/m/d H:i:s',
        time: { hour: 23, min: 59, sec: 59 },
        id: 'time_end',
        name: 'time_end',
        margin: '0 5px',
        editable: false,
        listeners: {
            select: function (a, b, c) {
                var start = Ext.getCmp("time_start");
                var end = Ext.getCmp("time_end");
                var end_date = end.getValue();
                if (start.getValue() == ""||start.getValue() ==null) {
                    Ext.getCmp('time_start').setValue(new Date(end_date.getFullYear(), end_date.getMonth() - 1, end_date.getDate()));
                }
                if (end.getValue() < start.getValue()) {
                    Ext.getCmp('time_start').setValue(new Date(end_date.getFullYear(), end_date.getMonth() - 1, end_date.getDate()));
                }
            }
        }
        //vtype: 'daterange',
      //  startDateField: 'time_start'
    }, {//add by zhuoqin0830w  2015/03/11  已買斷商品的篩選功能
        xtype: 'combobox',
        store: productPrepaidStore,
        fieldLabel: '&nbsp;&nbsp;' + NON_STOCK_PRODCUT,//已買斷商品
        labelWidth: 80,
        width: 205,
        id: 'buyPro',
        name: 'buyPro',
        displayField: 'parameterName',
        valueField: 'ParameterCode',
        editable: false,
        queryMode: 'local',
        listeners: {
            beforerender: function () {
                productPrepaidStore.load({
                    callback: function () {
                        Ext.getCmp("buyPro").setValue(productPrepaidStore.data.items[0].data.ParameterCode);
                    }
                });
            }
        }
    }]
};


var key_query = {
    xtype: 'textfield',
    fieldLabel: NAME_NUM_QUERY,
    width: 500,
    id: 'key',
    colName: 'key',
    //hidden: true,
    submitValue: false,
    name: 'key'
};

var price_check = {
    xtype: 'checkbox',
    name: 'price_check',
    id: 'price_check',
    boxLabel: PRICE_CHECK,
    colName: 'price_check'//,
    //hidden: true
};




function showDetail(product_id) {
    productId = product_id;
    if (winDetail == undefined) {
        winDetail = Ext.create('Ext.window.Window', {
            title: PRODUCT_DETAILS_DATA,//商品詳細資料
            constrain: true,
            modal: true,
            resizable: false,
            height: document.documentElement.clientHeight * 565 / 783,
            width: 1000,
            autoScroll: false,
            layout: 'fit',
            html: "<iframe scrolling='no' frameborder=0 width=100% height=100% src='/ProductList/ProductDetails'></iframe>",
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