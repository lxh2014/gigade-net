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
/*********end*******************/

/**********model&store**************/
Ext.define('GIGADE.SITE', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'Site_Id', type: 'string' },
        { name: 'Site_Name', type: 'string' }
    ]
});

//站臺store
var siteStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.SITE',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/ProductList/GetSite',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
});

////會員等級store
var userlevelStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryPara?paraType=userlevel',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
})

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

//價格狀態store
var priceStatusStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryPara?paraType=price_status',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
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

var brand = {
    xtype: 'combobox',
    fieldLabel: BRAND,
    store: brandStore,
    id: 'brand_id',
    colName: 'brand_id',
    //hidden: true,
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

//add by wangwei0216w 2014/8/21
var discount = {
    xtype: 'numberfield',
    maxValue: 1000,
    fieldLabel: DISCOUNT,
    width: 200,
    minValue: 1,
    allowBlank: false,
    id: 'discount',
    enableKeyEvents: true,
    colName: 'key',
    //hidden: true,
    submitValue: false,
    name: 'discount',
    regex: /^[0-9]*$/,
    listeners: {
        change: function () {
            Ext.getCmp("btn_discount").setDisabled(false);
            Ext.getCmp('newSitePrice').setDisabled(true)
        }
    }
};

var price = {
    xtype: 'numberfield',
    fieldLabel: ITEM_MONEY,
    width: 200,
    id: 'price',
    name: 'price',
    allowBlank: true,
    submitValue: false,
    regex: /^[0-9]*$/
};

var cost = {
    xtype: 'numberfield',
    fieldLabel: COST,
    width: 200,
    id: 'cost',
    name: 'cost',
    margin: '0 5 0 30',
    allowBlank: true,
    submitValue: false,
    regex: /^[0-9]*$/
}

var active_discount = {
    xtype: 'numberfield',
    fieldLabel: EVENT_PRICE_DISCOUNT,
    width: 200,
    maxValue: 1000,
    minValue: 1,
    id: 'active_discount',
    name: 'active_discount',
    submitValue: false,
    regex: /^[0-9]*$/
}

var active_price = {
    xtype: 'numberfield',
    fieldLabel: ITEM_EVENT_MONEY,
    width: 200,
    id: 'active_price',
    name: 'active_price',
    submitValue: false,
    allowBlank: true,
    regex: /^[0-9]*$/
}

var cost_discount = {
    xtype: 'numberfield',
    fieldLabel: EVENT_COSTDISCOUNT,
    width: 200,
    maxValue: 1000,
    minValue: 1,
    allowBlank: false,
    margin: '0 5 0 30',
    id: 'cost_discount',
    submitValue: false,
    name: 'cost_discount',
    regex: /^[0-9]*$/,
    listeners: {
        change: function () {
            Ext.getCmp("btn_cost_discount").setDisabled(false);
            Ext.getCmp('newSitePrice').setDisabled(true)
        }
    }
}

var active_cost_discount = {
    xtype: 'numberfield',
    fieldLabel: EVENT_COST_DISCOUNT,
    width: 200,
    maxValue: 1000,
    minValue: 1,
    //allowBlank: false,
    margin: '0 5 0 30',
    id: 'active_cost_discount',
    //colName: 'key',
    //hidden: true,
    submitValue: false,
    name: 'active_cost_discount',
    regex: /^[0-9]*$/
}

var activ_cost = {
    xtype: 'numberfield',
    fieldLabel: EVENT_COST,
    width: 200,
    margin: '0 5 0 30',
    id: 'activ_cost',
    submitValue: false,
    name: 'activ_cost',
    allowBlank: true,
    regex: /^[0-9]*$/
}


var activ_cost_price = {
    xtype: 'textfield',
    fieldLabel: NAME_NUM_QUERY,
    width: 500,
    id: 'activ_cost_price',
    submitValue: false,
    name: 'activ_cost_price'
}



var price_check = {
    xtype: 'checkbox',
    name: 'price_check',
    id: 'price_check',
    boxLabel: PRICE_CHECK,
    colName: 'price_check'
};

var sitebox = {
    xtype: 'combobox',
    fieldLabel: SITE,
    store: siteStore,
    id: 'site_id',
    name: 'site_id',
    displayField: 'Site_Name',
    valueField: 'Site_Id',
    queryMode: 'local',
    colName: 'site_id',
    editable: false,
    allowBlank: false,
    listeners: {
        beforerender: function () {
            siteStore.load({
                callback: function () {
                    if (location.pathname != "/ProductSelect") {
                        for (var i = 0; i < siteStore.getCount() ; i++) {
                            if (siteStore.getAt(i).get("Site_Id") == 1) {
                                siteStore.remove(siteStore.getAt(i));
                            }
                        }
                    }
                }
            });
        }
    }
};

var user_level_group = {
    xtype: 'combobox',
    fieldLabel: USER_LEVEL,
    store: userlevelStore,
    id: 'user_level',
    margin: '0 5 0 44',
    name: 'user_level',
    displayField: 'parameterName',
    valueField: 'parameterCode',
    queryMode: 'local',
    colName: 'user_level',
    editable: false,
    listeners: {
        beforerender: function () {
            userlevelStore.load({
                callback: function () {
                    Ext.getCmp("user_level").setValue(userlevelStore.data.items[0].data.parameterCode);
                }
            });
        }
    }
};


var user_level = {
    xtype: 'combobox',
    fieldLabel: USER_LEVEL,
    store: userlevelStore,
    id: 'user_level',
    name: 'user_level',
    displayField: 'parameterName',
    valueField: 'parameterCode',
    queryMode: 'local',
    colName: 'user_level',
    editable: false,
    listeners: {
        beforerender: function () {
            userlevelStore.load({
                callback: function () {
                    userlevelStore.insert(0, { parameterCode: '0', parameterName: VALUE_ALL });
                    Ext.getCmp("user_level").setValue(userlevelStore.data.items[1].data.parameterCode);
                }
            });
        }
    }
};

var userLevel = {
    xtype: 'panel',
    margin: '0 0 0 20px',
    border: false,
    items: [user_level]
};

//站臺_會員等級
var site = {
    xtype: 'fieldcontainer',
    layout: 'hbox',
    anchor: '100%',
    items: [sitebox, userLevel]
};

var price_status = {
    xtype: 'combobox',
    name: 'price_status',
    id: 'price_status',
    store: priceStatusStore,
    fieldLabel: PRICE_STATUS,
    queryMode: 'local',
    displayField: 'parameterName',
    valueField: 'parameterCode',
    colName: 'price_status',
    //hidden: true,
    editable: false,
    listeners: {
        beforerender: function () {
            priceStatusStore.load({
                callback: function () {
                    priceStatusStore.insert(0, { parameterCode: '0', parameterName: VALUE_ALL });
                    Ext.getCmp("price_status").setValue(priceStatusStore.getAt(0).data.parameterCode);
                }
            });
        }
    }
};



/*******************Store Model************************/
var m_pro_pricemasterid = { name: 'price_master_id', type: 'string' };
var m_pro_base = [
    { name: 'product_image', type: 'string' },
    { name: 'product_id', type: 'string' },
    { name: 'brand_name', type: 'string' },
    { name: 'product_name', type: 'string' },
    { name: 'prod_sz', type: 'string' }
];
var m_pro_type = { name: 'combination', type: 'string' };
var m_pro_pricetype = { name: 'price_type', type: 'string' };
var m_pro_pricetypeid = { name: 'price_type_id', type: 'string' };
var m_pro_type_id = { name: 'combination_id', type: 'string' };
var m_pro_spec = { name: 'product_spec', type: 'string' };
var m_pro_pricelist = { name: 'product_price_list', type: 'string' };
var m_pro_status = { name: 'product_status', type: 'string' };
var m_pro_status_id = { name: 'product_status_id', type: 'string' };
var m_pro_freight = { name: 'product_freight_set', type: 'string' };
var m_pro_mode = { name: 'product_mode', type: 'string' };
var m_pro_tax = { name: 'tax_type', type: 'string' };
var m_pro_sort = { name: 'product_sort', type: 'string' };
var m_pro_create = { name: 'product_createdate', type: 'string' };
var m_pro_start = { name: 'product_start', type: 'string' };
var m_pro_end = { name: 'product_end', type: 'string' };
var m_pro_site = { name: 'site_name', type: 'string' };
var m_pro_site_id = { name: 'site_id', type: 'string' };
var m_pro_level = { name: 'level', type: 'string' };
var m_pro_master_user_id = { name: 'master_user_id', type: 'string' };
var m_pro_userlevel = { name: 'user_level', type: 'string' };
var m_pro_pricestatus = { name: 'price_status', type: 'string' };
var m_pro_itemmoney = { name: 'price', type: 'string' };
var m_pro_itemeventmoney = { name: 'event_price', type: 'string' };
var m_pro_eventstart = { name: 'event_start', type: 'string' };
var m_pro_eventend = { name: 'event_end', type: 'string' };
var m_pro_askfordate = { name: 'apply_time', type: 'string' };
var m_pro_askforperson = { name: 'apply_user', type: 'string' };
var m_pro_kuser = { name: 'user_name', type: 'string' };
var m_pro_applytype = { name: '', type: 'string' };
var m_pro_email = { name: 'user_email', type: 'string' };
var m_CanSel = { name: 'CanSel', type: 'string' };
var m_CanDo = { name: 'CanDo', type: 'string' };

var m_pro_cost = { name: 'cost', type: 'string' };
var m_pro_eventcost = { name: 'event_cost', type: 'string' };
var m_pro_event_price_discount = { name: 'event_price_discount', type: 'string' };
var m_pro_event_cost_discount = { name: 'event_cost_discount', type: 'string' };
var m_pro_pricetype = { name: 'price_type', type: 'string' };

/*******************GRID COLUMNS***********************/
var c_pro_base = [
    { header: PRODUCT_IMAGE, colName: 'product_image', hidden: true, xtype: 'templatecolumn', tpl: '<div style="width:50px;height:50px"><img width="50px" height="50px" src="{product_image}" /><div>', width: 60, align: 'center', sortable: false, menuDisabled: true },
    { header: PRODUCT_ID, colName: 'product_id', hidden: true, xtype: 'templatecolumn', tpl: '<a href=# onclick="javascript:showDetail({product_id})" >{product_id}</a>', width: 60, align: 'center', sortable: false, menuDisabled: true },
    {
        header: PRODUCT_NAME, colName: 'product_name', hidden: true, dataIndex: 'product_name', width: 180, align: 'left', sortable: false, menuDisabled: true, flex: 1
        /*,
        renderer: function (value, cellmeta, record) {
            return value + (record.data.prod_sz ? ' (' + record.data.prod_sz + ')' : '');
        }*/
    },
    { header: BRAND_NAME, colName: 'brand_id', hidden: true, dataIndex: 'brand_name', width: 120, align: 'left', sortable: false, menuDisabled: true }
];
//商品類型
var c_pro_type = {
    header: PRODUCT_TYPE, colName: 'combination', hidden: true, dataIndex: 'combination', width: 80, align: 'center', sortable: false, menuDisabled: true
};

//價格類型
var c_pro_pricetype = {
    header: PRICE_TYPE, colName: 'price_type', dataIndex: 'price_type', width: 150, align: 'center', sortable: false, menuDisabled: true
};

var c_pro_spec = {
    header: PRODUCT_SPEC, colName: 'product_spec', hidden: true, dataIndex: 'product_spec', width: 80, align: 'center', sortable: false, menuDisabled: true
};

var c_pro_status = {
    header: PRODUCT_STATUS, colName: 'product_status', hidden: true, dataIndex: 'product_status', width: 90, align: 'center', sortable: false, menuDisabled: true
};

var c_pro_site = {
    header: SITE, colName: 'site_id', hidden: true, dataIndex: 'site_name', width: 80, align: 'center', sortable: false, menuDisabled: true
};

var c_pro_userlevel = {
    header: USER_LEVEL, colName: 'user_level', hidden: true, dataIndex: 'user_level', width: 120, align: 'center', sortable: false, menuDisabled: true
};
var c_pro_pricestatus = {
    header: PRICE_STATUS, colName: 'price_status', hidden: true, dataIndex: 'price_status', width: 120, align: 'center', sortable: false, menuDisabled: true
};
var c_pro_itemoney = {
    header: ITEM_MONEY, colName: 'price', hidden: true, dataIndex: 'price', width: 80, align: 'center', sortable: false, menuDisabled: true
};


var c_pro_itemeventmoney = {
    header: ITEM_EVENT_MONEY, colName: 'event_price', hidden: true, dataIndex: 'event_price', width: 80, align: 'center', sortable: false, menuDisabled: true
};

//var c_pro_eventcost = { header: '活動成本', /*colName: 'cost', hidden: true,*/ dataIndex: 'event_cost', width: 80, align: 'center', sortable: false, menuDisabled: true };

var c_pro_eventdate = {
    header: EVENT_DATE, xtype: 'templatecolumn', colName: 'event_start', hidden: true,
    tpl: Ext.create('Ext.XTemplate',
        '{[values.event_start==0?"":Ext.Date.format(new Date(values.event_start * 1000),"Y/m/d H:i:s")]}',
        '~',
        '{[values.event_end==0?"":Ext.Date.format(new Date(values.event_end * 1000),"Y/m/d H:i:s")]}'
    ),
    width: 240, align: 'center', sortable: false, menuDisabled: true
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