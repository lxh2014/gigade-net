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
        url: '/VendorProductList/GetSite',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
});

//會員等級store
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
        url: '/VendorProductList/GetProCategory',
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
        url: '/VendorProductList/GetFrontCatagory',
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
    data: [{ name: '全部', code: '-1' }, { name: '新建商品', code: '0' }, { name: '下架', code: '6'}]
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
        { name: "brand_name", type: "string"}]
});



//品牌store
var brandStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Brand',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/VendorBrand/QueryBrand",
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
    items: [
        {
            //運送方式
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
        },
        {
            //出貨方式
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
        }//,
    //    {
    //        //營業稅
    //        xtype: 'combobox',
    //        margin: '0 20px',
    //        id: 'tax_type',
    //        name: 'tax_type',
    //        width: 180,
    //        labelWidth: 50,
    //        store: taxStore,
    //        queryMode: 'local',
    //        displayField: 'text',
    //        valueField: 'value',
    //        value: '0',
    //        editable: false,
    //        colName: 'tax_type',
    //        //hidden: true,
    //        fieldLabel: TAX_TYPE
    //    }
    ]
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
    }]
};


var key_query = {
    xtype: 'textfield',
    fieldLabel: NAME_NUM_QUERY,
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
    //hidden: true,
    editable: false,
    listeners: {
        beforerender: function () {
            siteStore.load({
                callback: function () {
                    siteStore.insert(0, { Site_Id: '0', Site_Name: VALUE_ALL });
                    Ext.getCmp("site_id").setValue(siteStore.data.items[0].data.Site_Id);
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
    //hidden: true,
    editable: false,
    listeners: {
        beforerender: function () {
            userlevelStore.load({
                callback: function () {
                    userlevelStore.insert(0, { parameterCode: '0', parameterName: VALUE_ALL });
                    Ext.getCmp("user_level").setValue(userlevelStore.data.items[0].data.parameterCode);
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
var m_pro_base = [
    { name: 'product_image', type: 'string' },
    { name: 'product_id', type: 'string' },
    { name: 'brand_name', type: 'string' },
    { name: 'product_name', type: 'string' }
];
var m_pro_type = { name: 'combination', type: 'string' };
var m_pro_pricetype = { name: 'price_type', type: 'string'}//add by hufeng0813w Reason:商品列表加上價格類型 2014/06/17
var m_pro_type_id = { name: 'combination_id', type: 'string' };
var m_pro_spec = { name: 'product_spec', type: 'string' };
var m_pro_pricelist = { name: 'product_price_list', type: 'string' };   //建議售價
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
var m_pro_site_id = { name: 'site_id', type: 'string' }; //add by hufeng0813w 2014/05/22 站臺ID
var m_pro_level = { name: 'level', type: 'string' }; //add by hufeng0813w 2014/05/22 會員等級ID
var m_pro_master_user_id = { name: 'master_user_id', type: 'string' }; //add by hufeng0813w 2014/05/22 會員ID
var m_pro_userlevel = { name: 'user_level', type: 'string' };
var m_pro_pricestatus = { name: 'price_status', type: 'string' };
var m_pro_itemeventmoney = { name: 'event_price', type: 'string' };
var m_pro_itemmoney = { name: 'price', type: 'string' };
var m_pro_cost = { name: 'cost', type: 'string' };  //成本
var m_pro_event_cost = { name: 'event_cost', type: 'string' };  //活動成本
var m_pro_eventstart = { name: 'event_start', type: 'string' };
var m_pro_eventend = { name: 'event_end', type: 'string' };
var m_pro_askfordate = { name: 'apply_time', type: 'string' };
var m_pro_askforperson = { name: 'apply_user', type: 'string' };
var m_pro_kuser = { name: 'user_name', type: 'string' };
var m_pro_applytype = { name: '', type: 'string' };
var m_pro_email = { name: 'user_email', type: 'string' };
var m_CanSel = { name: 'CanSel', type: 'string' };
var m_CanDo = { name: 'CanDo', type: 'string' };
var m_temp_status = { name: 'temp_status', type: 'string' }; //add jialei 20140902
var m_create_channel = { name: 'create_channel', type: 'string' }; // add zhejiang 20140905
/*******************GRID COLUMNS***********************/
/*我把hidden修改成false*/
var c_pro_base = [
    { header: PRODUCT_IMAGE, colName: 'product_image', hidden: false, xtype: 'templatecolumn', tpl: '<div style="width:50px;height:50px"><img width="50px" height="50px" src="{product_image}" /><div>', width: 60, align: 'center', sortable: false, menuDisabled: true },
    { header: PRODUCT_ID, dataIndex: 'product_id', css: 'color:blue;', colName: 'product_id', hidden: false, width: 60, align: 'center', sortable: false, menuDisabled: true,
        renderer: function (value, metaData, record, rowIndex, columnIndex, store) {
            if (record.data.create_channel == 2) {
                valueid = '"' + value.toString() + '"';
                return "<div style='background-color:#B7FF4A' ><a href='#' onclick='javascript:showDetail(" + valueid + ")' >" + value + "</a></div>";
            } else {
                valueid = '"' + value.toString() + '"';
                return "<div><a href='#' onclick='javascript:showDetail(" + valueid + ")' >" + value + "</a></div>";
            }

        }
    },
    { header: LAST_MODIFY, colName: 'history_updata', hidden: true, xtype: 'templatecolumn', tpl: '<a href=# onclick="javascript:historyUpdata({product_id},{site_id},{level},{user})" >上次修改</a>', width: 60, align: 'center', sortable: false, menuDisabled: true }, //editor by wangwei0216w 在ext.grid.panel中添加上次修改
    {header: PRODUCT_NAME, colName: 'product_name', hidden: false, dataIndex: 'product_name', width: 180, align: 'left', sortable: false, menuDisabled: true },
    { header: BRAND_NAME, colName: 'brand_id', hidden: false, dataIndex: 'brand_name', width: 120, align: 'left', sortable: false, menuDisabled: true }
];

var c_pro_base_byproduct = [
    { header: PRODUCT_IMAGE, colName: 'product_image', hidden: false, xtype: 'templatecolumn', tpl: '<div style="width:50px;height:50px"><img width="50px" height="50px" src="{product_image}" /><div>', width: 60, align: 'center', sortable: false, menuDisabled: true },
    { header: PRODUCT_ID, colName: 'product_id', hidden: false, xtype: 'templatecolumn', tpl: '<a href=# onclick="javascript:showDetail({product_id})" >{product_id}</a>', width: 60, align: 'center', sortable: false, menuDisabled: true },
    { header: LAST_MODIFY, colName: 'history_updata', hidden: false, xtype: 'templatecolumn', tpl: '<a href=# onclick="javascript:historyUpdataByProduct({product_id})" >上次修改</a>', width: 60, align: 'center', sortable: false, menuDisabled: true }, //editor by wangwei0216w 在ext.grid.panel中添加上次修改
    {header: PRODUCT_NAME, colName: 'product_name', hidden: false, dataIndex: 'product_name', width: 180, align: 'left', sortable: false, menuDisabled: true },
    { header: BRAND_NAME, colName: 'brand_id', hidden: false, dataIndex: 'brand_name', width: 120, align: 'left', sortable: false, menuDisabled: true }
];
//商品類型
var c_pro_type = {
    header: PRODUCT_TYPE, colName: 'combination', hidden: false, dataIndex: 'combination', width: 80, align: 'center', sortable: false, menuDisabled: true
};
//價格類型 add by hufeng0813w 2014/06/18 Reason:為了再商品列表上加上價格類型
var c_pro_pricetype = {
    header: PRICE_TYPE, colName: 'price_type', dataIndex: 'price_type', width: 150, align: 'center', sortable: false, menuDisabled: true
};
var c_pro_spec = {
    header: PRODUCT_SPEC, colName: 'product_spec', hidden: false, dataIndex: 'product_spec', width: 80, align: 'center', sortable: false, menuDisabled: true
};
/*建議售價*/
var c_pro_pricelist = {
    header: PRODUCT_PRICE_LIST, colName: 'product_price_list', hidden: false, dataIndex: 'product_price_list', width: 70, align: 'center', sortable: false, menuDisabled: true
};
var c_pro_status = {
    header: PRODUCT_STATUS, colName: 'product_status', hidden: false, dataIndex: 'product_status', width: 90, align: 'center', sortable: false, menuDisabled: true
};
var c_pro_freight = {
    header: PRODUCT_FREIGHT_SET, colName: 'product_freight_set', hidden: false, dataIndex: 'product_freight_set', width: 60, align: 'center', sortable: false, menuDisabled: true
};
var c_pro_mode = {
    header: PRODUCT_MODE, colName: 'product_mode', hidden: false, dataIndex: 'product_mode', width: 60, align: 'center', sortable: false, menuDisabled: true
};
var c_pro_tax = {
    header: TAX_TYPE, colName: 'tax_type', hidden: false, dataIndex: 'tax_type', width: 60, align: 'center',
    renderer: function (val) { switch (val) { case '1': return TAX_NEED; case '3': return TAX_NO } }, sortable: false, menuDisabled: true
};
var c_pro_sort = {
    header: PRODUCT_SORT, colName: 'product_sort', hidden: false, dataIndex: 'product_sort', width: 60, align: 'center', sortable: false, menuDisabled: true
};
var c_pro_create = {
    header: PRODUCT_CREATE, colName: 'product_createdate', hidden: false, dataIndex: 'product_createdate', width: 120, align: 'center',
    renderer: function (val) { return val == '0' ? '' : Ext.Date.format(new Date(val * 1000), 'Y/m/d H:i:s'); },
    sortable: false, menuDisabled: true
};
var c_pro_start = {
    header: PRODUCT_START, colName: 'product_start', hidden: false, dataIndex: 'product_start', width: 120, align: 'center',
    renderer: function (val) { return val == '0' ? '' : Ext.Date.format(new Date(val * 1000), 'Y/m/d H:i:s'); },
    sortable: false, menuDisabled: true
};
var c_pro_end = {
    header: PRODUCT_END, colName: 'product_end', hidden: false, dataIndex: 'product_end', width: 120, align: 'center',
    renderer: function (val) { return val == '0' ? '' : Ext.Date.format(new Date(val * 1000), 'Y/m/d H:i:s'); },
    sortable: false, menuDisabled: true
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
    header: PRICE, colName: 'price', hidden: false, dataIndex: 'price', width: 80, align: 'center', sortable: false, menuDisabled: true
};

//edit by wangwei0216w 功能:2014/8/8在下列添加 c_pro_cost,c_pro_eventcost
//成本 edit by jiajun 2014/08/14 調整欄位 
var c_pro_cost = {
    header: COST, colName: 'cost', hidden: false, dataIndex: 'cost', width: 80, align: 'center', sortable: false, menuDisabled: true
};

var c_pro_itemeventmoney = {
    header: ITEM_EVENT_MONEY, colName: 'event_price', hidden: false, dataIndex: 'event_price', width: 80, align: 'center', sortable: false, menuDisabled: true
};


//活動成本
var c_pro_eventcost = {
    header: EVENT_COST, colName: 'event_cost', hidden: false, dataIndex: 'event_cost', width: 80, align: 'center', sortable: false, menuDisabled: true
};
//活動期間
var c_pro_eventdate = {
    header: EVENT_DATE, xtype: 'templatecolumn', colName: 'event_start', hidden: false,
    tpl: Ext.create('Ext.XTemplate',
        '{[values.event_start==0?"":Ext.Date.format(new Date(values.event_start * 1000),"Y/m/d H:i:s")]}',
        '~',
        '{[values.event_end==0?"":Ext.Date.format(new Date(values.event_end * 1000),"Y/m/d H:i:s")]}'
    ),
    width: 240, align: 'center', sortable: false, menuDisabled: true
};
//複製按鈕
var c_pro_copy = {
    header: '', colName: 'copy_product', hidden: false, xtype: 'templatecolumn',
    tpl: Ext.create('Ext.XTemplate',
        '<tpl if="this.canCopy(product_id)">',
            '<a href=javascript:ProductCopy("{product_id}",{combination_id},"{[Ext.String.htmlEncode(values.product_name.split(" ").join("&nbsp;"))]}") >' + COPY_PRODUCT + '</a>',
        '</tpl>', {
            canCopy: function (product_id) {
                var reg = new RegExp("^[0-9]*$");
                if (reg.test(product_id)) {
                    return product_id >= 10000;
                }
                else {
                    return true;
                }
            }
        }),
    width: 50, align: 'center', sortable: false, menuDisabled: true
};
//預覽 add by hufeng0813w 2014/05/19 需求需要提供一個預覽的按鈕
var c_pro_preview = {
    header: '', colName: 'preview_product', hidden: false, xtype: 'templatecolumn',
    tpl: '<a href=javascript:ProductPreview(0,"{product_id}")>' + PREVIEW_PRODUCT + '</a>', //0為商品預覽 调用自己写的方法 ProductPreview()
    width: 50, align: 'center', sortable: false, menuDisabled: true
}
//連接編碼 add by hufeng0813w 2014/05/19 需求需要提供一個連接編碼的按鈕
var c_pro_linkcoding = {
    header: '', colName: 'preview_product', hidden: false, xtype: 'templatecolumn',
    tpl: '<a href=javascript:LinkCoding(1,"{product_id}",{site_id},{level},{master_user_id})>' + PREVIEW_PRODUCT + '</a>', //1為商品+價格列表 调用自己写好的方法LinkCoding()
    width: 90, align: 'center', sortable: false, menuDisabled: true
}
/*申請時間*/
var c_pro_askfordate = {
    header: ASK_FOR_DATE, hidden: true, colName: 'apply_time', dataIndex: 'apply_time', width: 120, align: 'center', sortable: false, menuDisabled: true, renderer: function (value) {
        value = value.substring(value.lastIndexOf('(') + 1, value.lastIndexOf(')'));
        value = Ext.Date.format(new Date(eval(value)), 'Y-m-d H:i');
        return value;
    }
}

/*申請人*/
var c_pro_askforperson = {
    header: ASK_FOR_PERSON, dataIndex: 'apply_user', colName: 'apply_user', width: 80, align: 'center', sortable: false, menuDisabled: true, hidden: true
}
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
    header: APPLY_TYPE, colName: 'prev_status', hidden: true, dataIndex: 'prev_status', width: 80, align: 'center', sortable: false, menuDisabled: true
}

/*會員email*/
var c_pro_email = {
    header: USER_EMAIL, dataIndex: 'user_email', colName: 'user_email', width: 80, align: 'center', sortable: false, menuDisabled: true, hidden: true
}

/*臨時表中狀態*/
var c_temp_status = {
    header: 'temp_status', colName: 'temp_status', hidden: true, dataIndex: 'temp_status', width: 80, align: 'center', sotable: false, menuDisabled: true
}
var c_create_channel = {
    header: PRODUCT_KUSER, colName: 'create_channel', dataIndex: 'create_channel', width: 80, align: 'center', sotable: false, menuDisabled: true,
    renderer: function (val, cellmeta, record, rowIndex, columnIndex, store) {
        switch (val) {
            case "1":
                return UserManger;
            case "2":
                return VendorManger;
            default:
                return "";
        }
    }
}

function changeValue(value) {
    value = value.substring(value.lastIndexOf('(') + 1, value.lastIndexOf(')'));
    value = Ext.Date.format(new Date(eval(value)), 'Y-m-d H:i');
    return value;
}



var veryfiyConfirm = function (store, rows, functionid) {
    if (!functionid) {
        functionid = '';
    }
    var result = '';
    for (var i = 0, j = rows.length; i < j; i++) {
        if (i > 0) {
            result += ',';
        }
        result += rows[i].data.product_id;
    }
    Ext.Ajax.request({
        url: '/VendorProductList/VerifyApply',
        params: {
            prodcutIdStr: result
        },
        success: function (response) {
            var result = Ext.decode(response.responseText);
            if (result.success) {
                Ext.Msg.alert(INFORMATION, SUCCESS, function () {
                    store.loadPage(1);
                });
            }
            else {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
            btn.setDisabled(false);
        }
    });

}

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
            html: "<iframe scrolling='no' frameborder=0 width=100% height=100% src='/VendorProductList/ProductDetails'></iframe>",
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
//{product_id},{combination_id},"{product_name}
//複製商品
function ProductCopy(product_id, combination, product_name) {
    Ext.Msg.confirm(CONFIRM, COPY_PRODUCT_CONFIRM, function (btn) {
        if (btn == 'yes') {
            Ext.Ajax.request({
                url: '/VendorProductList/ProductCopy',
                params: { Product_Id: product_id, Combination: combination },
                success: function (form, action) {
                    var result = Ext.decode(form.responseText);
                    if (result.success) {
                        var url = '';
                        switch (combination) {
                            case 0:
                            case 1:
                                url = '/VendorProduct/ProductSave?product_id=' + product_id + '&id=' + result.id;
                                break;
                            default:
                                url = '/VendorProductCombo/index?product_id=' + product_id + '&id=' + result.id;
                                break;
                        }
                        var panel = window.parent.parent.Ext.getCmp('ContentPanel');
                        var copy = panel.down('#copy');
                        if (copy) {
                            copy.close();
                        }
                        copy = panel.add({
                            id: 'copy',
                            title: COPY_PRODUCT + ' - ' + (product_name.length > 10 ? (product_name.substr(0, 10) + '...') : product_name),
                            html: window.top.rtnFrame(url),
                            closable: true
                        });
                        panel.setActiveTab(copy);
                        panel.doLayout();
                        //window.location.href = 'http://' + window.location.host + url;
                    }
                    else {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                }
                //                ,
                //                failure: function () {
                //                    Ext.Msg.alert(INFORMATION, FAILURE);
                //                }
            });
        }
    });
}
//預覽信息(商品)
//没有什么逻辑的地方
//基本都为Ext的创建方法 add by hufeng0813w 2014/06/09
function ProductPreview(type, product_id) {
    Ext.Ajax.request({
        url: '/VendorProductList/ProductPreview',
        params: { Product_Id: product_id, Type: type },
        success: function (form, action) {
            var result = form.responseText;
            var htmlval = "<a href=" + result + " target='new'>" + result + "</a>";
            Ext.create('Ext.window.Window', {
                title: PREVIEW_PRODUCT,
                modal: true,
                height: 110,
                width: 600,
                constrain: true,
                layout: 'fit',
                items: [
                {
                    xtype: 'panel',
                    border: false,
                    layout: 'hbox',
                    autoScroll: true,
                    bodyStyle: {
                        padding: '20px 10px'
                    },
                    items: [{
                        xtype: 'displayfield',
                        labelWidth: 55,
                        labelAlign: 'top',
                        fieldLabel: PREVIEW_PRODUCT,
                        value: htmlval
                    }]
                }]
            }).show();
        }
    })
}
//預覽信息(商品+價格)
//没有什么逻辑的地方
//基本都为Ext的创建方法 add by hufeng0813w 2014/06/09
function LinkCoding(type, product_id, site_id, user_level, user_id) {
    Ext.Ajax.request({
        url: '/VendorProductList/ProductPreview',
        params: { Product_Id: product_id, Type: type, Site_Id: site_id, Level: user_level, Master_User_Id: user_id },
        success: function (form, action) {
            var result = form.responseText;
            var htmlval1 = "<a href=" + result.split('|')[0] + " target='new'>" + result.split('|')[0] + "</a>"; //預覽
            var htmlval2 = "<a href=" + result.split('|')[1] + " target='new'>" + result.split('|')[1] + "</a>"; //商品隱賣連結
            Ext.create('Ext.window.Window', {
                title: PREVIEW_PRODUCT,
                modal: true,
                height: 160,
                width: 780,
                constrain: true,
                layout: 'fit',
                items: {
                    xtype: 'panel',
                    border: false,
                    autoScroll: true,
                    bodyStyle: {
                        padding: '20px 10px'
                    },
                    items: [{
                        xtype: 'displayfield',
                        labelWidth: 55,
                        labelAlign: 'top',
                        fieldLabel: PREVIEW_PRODUCT,
                        value: htmlval1
                    }, {
                        xtype: 'displayfield',
                        labelWidth: 85,
                        labelAlign: 'top',
                        fieldLabel: '商品隱賣連結',
                        value: htmlval2
                    }]
                }
            }).show();
        }
    })
}

///Summary
///將'上次修改'放置於Ext.gird.panel中 edit by wangwei0216w 2014/8/12
///Summary
function historyUpdata(id, side_id, level, user) {
    Ext.create('Ext.window.Window', {
        title: '上次修改記錄',
        width: 1000,
        height: document.documentElement.clientHeight * 610 / 783,
        autoScroll: true,
        bodyStyle: 'background:#ffffff; padding:5px;',
        closeaction: 'destroy',
        resizable: false,
        draggable: false,
        modal: true,
        listeners: {
            show: function (e, eOpts) {
                Ext.Ajax.request({
                    url: '/VendorProductList/QueryLastModifyRecord',
                    method: 'post',
                    params: {
                        Product_Id: id,
                        Type: 'price',
                        Site_id: side_id,
                        User_Level: level,
                        User_id: user
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            if (result.html) {
                                e.update(Ext.htmlDecode(result.html));
                            } else {
                                e.destroy();
                                Ext.Msg.alert(INFORMATION, SEARCH_NO_DATA);
                            }
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }
        }
    }).show();
}

///Summary
///將'上次修改'放置於Ext.gird.panel中 edit by wangwei0216w 2014/8/12
///Summary
function historyUpdataByProduct(id) {
    Ext.create('Ext.window.Window', {
        title: '上次修改記錄',
        width: 1000,
        height: document.documentElement.clientHeight * 610 / 783,
        autoScroll: true,
        bodyStyle: 'background:#ffffff; padding:5px;',
        closeaction: 'destroy',
        resizable: false,
        draggable: false,
        modal: true,
        listeners: {
            show: function (e, eOpts) {
                Ext.Ajax.request({
                    url: '/VendorProductList/QueryLastModifyRecord',
                    method: 'post',
                    params: { Product_Id: id, Type: 'product' },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            if (result.html) {
                                e.update(Ext.htmlDecode(result.html));
                            } else {
                                e.destroy();
                                Ext.Msg.alert(INFORMATION, SEARCH_NO_DATA);
                            }
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }
        }
    }).show();
}
