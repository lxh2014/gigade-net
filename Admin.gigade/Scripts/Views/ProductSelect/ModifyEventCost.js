/// <reference path="../ProductList/ProductList.js" />
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

var m_pro_kuser = { name: 'user_name', type: 'string' };
var m_pro_applytype = { name: '', type: 'string' };
var m_pro_email = { name: 'user_email', type: 'string' };

var m_pro_cost = { name: 'cost', type: 'string' };
var m_pro_eventcost = { name: 'event_cost', type: 'string' };
var m_pro_event_price_discount = { name: 'event_price_discount', type: 'string' };
var m_pro_event_cost_discount = { name: 'event_cost_discount', type: 'string' };
var m_pro_pricetype = { name: 'price_type', type: 'string' };

/*******************GRID COLUMNS***********************/
var c_pro_del = {
    header: DELETE, width: 40, align: 'center', menuDisabled: true, xtype: 'actioncolumn', items: [{
        icon: '../../../Content/img/icons/cross.gif',
        handler: function (grid, rowIndex, colIndex) {
            var priceMasterId = s_store.getAt(rowIndex).get('price_master_id');
            Ext.Msg.confirm(NOTICE, CONFIRM_DEL, function (btn) {
                if (btn == "yes") {
                    s_store.removeAt(rowIndex);
                }
            });
        }

    }]

};
var c_pro_base = [
    { header: PRODUCT_IMAGE, /*colName: 'product_image', hidden: true,*/ xtype: 'templatecolumn', tpl: '<div style="width:50px;height:50px"><img width="50px" height="50px" src="{product_image}" /><div>', width: 60, align: 'center', sortable: false, menuDisabled: true },
    { header: PRODUCT_ID, dataIndex: 'product_id',/* colName: 'product_id', hidden: true, xtype: 'templatecolumn', tpl: '<a href=# onclick="javascript:showDetail({product_id})" >{product_id}</a>', */width: 60, align: 'center', sortable: false, menuDisabled: true },
    {
        header: PRODUCT_NAME,/* colName: 'product_name', hidden: true,*/ dataIndex: 'product_name', width: 180, align: 'left', sortable: false, menuDisabled: true
        /*,
        renderer: function (value, cellmeta, record) {
            return value + (record.data.prod_sz ? ' (' + record.data.prod_sz + ')' : '');
        }*/
    },
    { header: BRAND_NAME,/* colName: 'brand_id', hidden: true,*/ dataIndex: 'brand_name', width: 120, align: 'left', sortable: false, menuDisabled: true }
];
//商品類型
var c_pro_type = {
    header: PRODUCT_TYPE, /*colName: 'combination', hidden: true,*/ dataIndex: 'combination', width: 80, align: 'center', sortable: false, menuDisabled: true
};

//價格類型
var c_pro_pricetype = {
    header: PRICE_TYPE, /*colName: 'price_type',*/ dataIndex: 'price_type', width: 80, align: 'center', sortable: false, menuDisabled: true
};

var c_pro_spec = {
    header: PRODUCT_SPEC, /*colName: 'product_spec', hidden: true,*/ dataIndex: 'product_spec', width: 80, align: 'center', sortable: false, menuDisabled: true
};

var c_pro_status = {
    header: PRODUCT_STATUS, /*colName: 'product_status', hidden: true,*/ dataIndex: 'product_status', width: 90, align: 'center', sortable: false, menuDisabled: true
};

var c_pro_site = {
    header: SITE, /*colName: 'site_id', hidden: true,*/ dataIndex: 'site_name', width: 80, align: 'center', sortable: false, menuDisabled: true
};

var c_pro_userlevel = {
    header: USER_LEVEL, /*colName: 'user_level', hidden: true,*/ dataIndex: 'user_level', width: 100, align: 'center', sortable: false, menuDisabled: true
};
var c_pro_pricestatus = {
    header: PRICE_STATUS,/* colName: 'price_status', hidden: true,*/ dataIndex: 'price_status', width: 120, align: 'center', sortable: false, menuDisabled: true
};
var c_pro_itemoney = {
    header: ITEM_MONEY,/* colName: 'price', hidden: true,*/ dataIndex: 'price', width: 80, align: 'center', sortable: false, menuDisabled: true
};

var c_pro_cost = { header: COST, /*colName: 'cost', hidden: true,*/ dataIndex: 'cost', width: 80, align: 'center', sortable: false, menuDisabled: true };

var c_pro_eventcost = {
    header: EVENT_COST, /*colName: 'cost', hidden: true,*/
    dataIndex: 'event_cost',
    width: 80,
    align: 'center',
    sortable: false,
    menuDisabled: true
};
var c_pro_eventcost_edit = {
    header: EVENT_COST, /*colName: 'cost', hidden: true,*/
    dataIndex: 'event_cost',
    width: 80,
    align: 'center',
    sortable: false,
    menuDisabled: true,
    //添加 可編輯 add by zhuoqin0830w 2015/02/10
    field: { xtype: 'numberfield', minValue: 0, allowBlank: false, regex: /^[0-9]*$/ }
};

var c_pro_event_price_discount = {
    header: EVENT_PRICE_DISCOUNT, /*colName: 'event_price_discount', hidden: true,*/
    dataIndex: 'event_price_discount',
    width: 80, align: 'center',
    sortable: false,
    hidden: location.search.substr(location.search.lastIndexOf('=') + 1) == 2 ? true : false,  // 判斷選擇按鈕是 原價選擇還是折扣價選擇  edit by zhuoqin0830w 2015/02/10
    menuDisabled: true,
    field: { xtype: 'numberfield', allowBlank: false, minValue: 1, maxValue: 99, regex: /^[0-9]*$/ }
};//edit by hjiajun1211w 2014/08/07 確保輸入合法性
var c_pro_event_cost_discount = {
    header: EVENT_COST_DISCOUNT, /*colName: 'event_cost_discount', hidden: true,*/
    dataIndex: 'event_cost_discount',
    width: 80, align: 'center',
    sortable: false,
    hidden: location.search.substr(location.search.lastIndexOf('=') + 1) == 2 ? true : false,  // 判斷選擇按鈕是 原價選擇還是折扣價選擇  edit by zhuoqin0830w 2015/02/10
    menuDisabled: true,
    field: { xtype: 'numberfield', allowBlank: false, minValue: 1, maxValue: 100, regex: /^[0-9]*$/ }
};//edit by hjiajun1211w 2014/08/07 確保輸入合法性

var c_pro_itemeventmoney_edit = {
    header: ITEM_EVENT_MONEY, /*colName: 'event_price', hidden: true,*/
    dataIndex: 'event_price',
    width: 80,
    align: 'center',
    sortable: false,
    menuDisabled: true,
    //添加 可編輯 add by zhuoqin0830w 2015/02/10
    field: { xtype: 'numberfield', minValue: 0, allowBlank: false, regex: /^[0-9]*$/ }
};
var c_pro_itemeventmoney = {
    header: ITEM_EVENT_MONEY, /*colName: 'event_price', hidden: true,*/
    dataIndex: 'event_price',
    width: 80,
    align: 'center',
    sortable: false,
    menuDisabled: true
};

var c_pro_eventstart = {
    header: EVENT_START, dataIndex: 'event_start', width: 140, align: 'center', sortable: false, menuDisabled: true,
    renderer: function (val) {
        if (Ext.Date.format(new Date(val), 'Y/m/d') == '1970/01/01')
            return '';
        return Ext.Date.format(new Date(val), 'Y/m/d H:i:s');
    }, field: {
        xtype: 'datetimefield',
        disabledMin: false,
        disabledSec: false,
        allowBlank: false, format: 'Y/m/d H:i:s',
        time: { hour: 0, min: 0, sec: 0 }// add by zhuoqin0830w  自定義時間空間的值  2015/03/18
    }
};

var c_pro_eventend = {
    header: EVENT_END, dataIndex: 'event_end', width: 140, align: 'center', sortable: false, menuDisabled: true,
    renderer: function (val) {
        if (Ext.Date.format(new Date(val), 'Y/m/d') == '1970/01/01')
            return '';
        return Ext.Date.format(new Date(val), 'Y/m/d H:i:s');
    }, field: {
        xtype: 'datetimefield',
        disabledMin: false,
        disabledSec: false,
        allowBlank: false, format: 'Y/m/d H:i:s',
        time: { hour: 23, min: 59, sec: 59 }// add by zhuoqin0830w  自定義時間空間的值  2015/03/18
    }
};

var pageSize = 15;

//站台價格列
var site = new Array();
site = site.concat(m_pro_base);
site.push(m_pro_type);
site.push(m_pro_type_id);
site.push(m_pro_status);
site.push(m_pro_status_id);
site.push(m_pro_site);
site.push(m_pro_site_id);
site.push(m_pro_level);
site.push(m_pro_master_user_id);
site.push(m_pro_userlevel);
site.push(m_pro_pricestatus);
site.push(m_pro_itemmoney);
site.push(m_pro_itemeventmoney);
site.push(m_pro_cost);
site.push(m_pro_eventcost);
site.push(m_pro_eventstart);
site.push(m_pro_eventend);
site.push(m_pro_event_price_discount);
site.push(m_pro_event_cost_discount);
site.push(m_pro_pricetype);
site.push(m_pro_pricetypeid);
site.push(m_pro_pricemasterid);
site.push({ name: 'prepaid', type: 'int' });

Ext.define('GIGADE.SITEPRODUCT', {
    extend: 'Ext.data.Model',
    fields: site
});
var s_store = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.SITEPRODUCT',
    proxy: {
        type: 'ajax',
        url: '/ProductSelect/QueryProList',
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
        brand_id: Ext.getCmp('') ? Ext.getCmp('').getValue() : ''

    });
});

var frm;
Ext.onReady(function () {
    var myDate = new Date();
    var year = myDate.getFullYear();
    var month = myDate.getMonth();
    var day = myDate.getDate();
    var hour = myDate.getHours();

    // 獲取 按鈕選擇的 值 判斷 打開那一個 tab  add by  zhuoqin0830w  2015/01/10
    var priceCondition = location.search.substr(location.search.lastIndexOf('=') + 1);
    if (priceCondition == "1") {
        frm = Ext.create('Ext.form.Panel', {
            layout: 'anchor',
            id: 'frm',
            width: 1185,
            border: false,
            padding: '5 0',
            defaults: { anchor: '95%', msgTarget: "side" },
            items: [{
                xtype: 'fieldcontainer',
                anchor: '100%',
                layout: {
                    type: 'hbox',
                    padding: '10',
                    pack: 'start',
                    align: 'top'
                },
                items: [{
                    xtype: 'numberfield',
                    fieldLabel: EVENT_PRICE_DISCOUNT,
                    id: 'epDis',
                    margin: '0 5px',
                    colName: 'epDis',
                    name: 'epDis',
                    //value: 88,
                    maxValue: 99,
                    minValue: 1,
                    regex: /^[0-9]*$/
                }, {
                    xtype: 'button',
                    margin: '0px 60px 0px 0px',
                    text: MODIFY_LIST,
                    listeners: {
                        click: function () {
                            var val = Ext.getCmp('epDis').getValue();   //獲取活動售價折數
                            if (Ext.getCmp('epDis').isValid()) {        //add by hjiajun1211w 2014/08/07  控制輸入的活動售價折數的正確性
                                //for (var i = 0, j = s_store.getCount() ; i < j; i++) {
                                //    var record = s_store.getAt(i);
                                //    record.set('event_price_discount', val);
                                //    record.set('event_price', (record.get('price') * (val * 0.01)).toFixed(0));
                                //}
                                var jsonArr = new Array();
                                for (var i = 0, j = s_store.getCount() ; i < j; i++) {
                                    var record = s_store.getAt(i);
                                    record.set('event_price_discount', val);
                                    var cost_discount;
                                    if (record.data.event_cost_discount == "") { cost_discount = 0; }
                                    else { cost_discount = record.data.event_cost_discount; }
                                    jsonArr.push({
                                        //eidt by zhuoqin0830w  更改商品活動成本驗算公式使活動價乘以折扣  傳值給後臺  2015/03/03
                                        price_master_id: record.data.price_master_id,
                                        price: record.data.price,
                                        event_price_discount: val,
                                        event_cost: record.data.event_cost,
                                        event_cost_discount: cost_discount
                                    });
                                }
                                ArithmeticalDiscount(s_store, jsonArr, "price_master_id", "event_price", "event_price_discount");//計算折扣
                            }
                        }
                    }
                }, {
                    xtype: 'displayfield',
                    value: EVENT_PRICE_DISCOUNT_DESC_TWO
                }]
            }, {
                xtype: 'fieldcontainer',
                anchor: '100%',
                layout: {
                    type: 'hbox',
                    padding: '10',
                    pack: 'start',
                    align: 'top'
                },
                items: [{
                    xtype: 'numberfield',
                    fieldLabel: EVENT_COST_DISCOUNT,
                    id: 'ecDis',
                    margin: '0 5px',
                    colName: 'ecDis',
                    name: 'ecDis',
                    //value: 88,
                    maxValue: 100,
                    minValue: 1,
                    regex: /^[0-9]*$/
                }, {
                    xtype: 'button',
                    id: 'btn_ecDis',
                    text: MODIFY_LIST,
                    listeners: {
                        click: function () {
                            var val = Ext.getCmp('ecDis').getValue();   //獲取活動成本折數
                            if (Ext.getCmp('ecDis').isValid()) {        //add by hjiajun1211w 2014/08/07  控制輸入的活動成本折數的正確性
                                //for (var i = 0, j = s_store.getCount() ; i < j; i++) {
                                //    var record = s_store.getAt(i);
                                //    record.set('event_cost_discount', val);
                                //    record.set('event_cost', (record.get('cost') * (val * 0.01)).toFixed(0));
                                //}
                                var jsonArr = new Array();
                                for (var i = 0, j = s_store.getCount() ; i < j; i++) {
                                    var record = s_store.getAt(i);
                                    var price_discount;
                                    if (record.data.event_price_discount == "") { price_discount = 0; }
                                    else { price_discount = record.data.event_price_discount; }
                                    record.set('event_cost_discount', val);
                                    jsonArr.push({
                                        //eidt by zhuoqin0830w  更改商品活動成本驗算公式使活動價乘以折扣  傳值給後臺  2015/03/03
                                        price_master_id: record.data.price_master_id,
                                        event_price: record.data.event_price,
                                        event_cost_discount: val
                                    });
                                }
                                ArithmeticalDiscount(s_store, jsonArr, "price_master_id", "event_cost", "event_cost_discount");//計算折扣
                            }
                        }
                    }
                }, {//add by zhuoqin0830w  2015/04/02  添加 依原成本設定值
                    xtype: 'checkbox',
                    margin: '0 5 0 5',
                    boxLabel: AS_OLD_COST_VALUE,//依原成本設定值
                    id: 'chkCost',
                    colName: 'chkCost',
                    handler: function () {
                        getCost();
                    }
                }, {
                    xtype: 'displayfield',
                    value: EVENT_COST_DISCOUNT_DESC
                }]
            }, {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                anchor: '100%',
                layout: {
                    type: 'hbox',
                    padding: '10',
                    pack: 'start',
                    align: 'top'
                },
                items: [{
                    xtype: 'datetimefield',
                    disabledMin: false,
                    disabledSec: false,
                    format: 'Y/m/d H:i:s',
                    fieldLabel: EVENT_START,
                    time: { hour: 0, min: 0, sec: 0 },// add by zhuoqin0830w  自定義時間空間的值  2015/03/18
                    id: 'time_start',
                    name: 'time_start',
                    margin: '0 5px',
                    editable: false
                    //value: new Date(year, month, day, hour, 0, 0, 0)
                }, {
                    xtype: 'button',
                    text: MODIFY_LIST,
                    listeners: {
                        click: function () {
                            var val = Ext.getCmp('time_start').getValue();//獲取開始時間
                            for (var i = 0, j = s_store.getCount() ; i < j; i++) {
                                var record = s_store.getAt(i);
                                record.set('event_start', val);
                            }
                        }
                    }
                }]
            }, {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                anchor: '100%',
                layout: {
                    type: 'hbox',
                    padding: '10',
                    pack: 'start',
                    align: 'top'
                },
                items: [{
                    xtype: 'datetimefield',
                    disabledMin: false,
                    disabledSec: false,
                    fieldLabel: EVENT_END,
                    time: { hour: 23, min: 59, sec: 59 },// add by zhuoqin0830w  自定義時間空間的值  2015/03/18
                    id: 'time_end',
                    name: 'time_end',
                    margin: '0 5px',
                    editable: false,
                    format: 'Y/m/d H:i:s'
                    //value: new Date(year, month, day, hour, 0, 0, 0)
                }, {
                    xtype: 'button',
                    text: MODIFY_LIST,
                    listeners: {
                        click: function () {
                            var val = Ext.getCmp('time_end').getValue();//獲取開始時間
                            for (var i = 0, j = s_store.getCount() ; i < j; i++) {
                                var record = s_store.getAt(i);
                                record.set('event_end', val);
                            }
                        }
                    }
                }]

            }]
        });
    } else if (priceCondition == "2") {
        frm = Ext.create('Ext.form.Panel', {
            layout: 'anchor',
            id: 'frm',
            width: 1185,
            border: false,
            padding: '5 0',
            defaults: { anchor: '95%', msgTarget: "side" },
            items: [{
                xtype: 'fieldcontainer',
                anchor: '100%',
                layout: {
                    type: 'hbox',
                    padding: '10',
                    pack: 'start',
                    align: 'top'
                },
                items: [{
                    xtype: 'numberfield',
                    fieldLabel: SINGLE_PRODUCT_EVENT_PRICE,//活動售價
                    id: 'epDis',
                    margin: '0 5px',
                    colName: 'epDis',
                    name: 'epDis',
                    //maxValue: 1000,
                    regex: /^[0-9]*$/
                }, {
                    xtype: 'button',
                    text: MODIFY_LIST,
                    listeners: {
                        click: function () {
                            var val = Ext.getCmp('epDis').getValue();
                            if (Ext.getCmp('epDis').isValid()) {
                                for (var i = 0, j = s_store.getCount() ; i < j; i++) {
                                    var record = s_store.getAt(i);
                                    record.set('event_price', val);
                                }
                            }
                        }
                    }
                }]
            }, {
                xtype: 'fieldcontainer',
                anchor: '100%',
                layout: {
                    type: 'hbox',
                    padding: '10',
                    pack: 'start',
                    align: 'top'
                },
                items: [{
                    xtype: 'numberfield',
                    fieldLabel: SINGLE_PRODUCT_EVENT_COST,//活動成本
                    id: 'ecDis',
                    margin: '0 5px',
                    colName: 'ecDis',
                    name: 'ecDis',
                    //maxValue: 1000,
                    regex: /^[0-9]*$/
                }, {
                    xtype: 'button',
                    id: 'btn_ecDis',
                    text: MODIFY_LIST,
                    listeners: {
                        click: function () {
                            var val = Ext.getCmp('ecDis').getValue();
                            if (Ext.getCmp('ecDis').isValid()) {
                                for (var i = 0, j = s_store.getCount() ; i < j; i++) {
                                    var record = s_store.getAt(i);
                                    record.set('event_cost', val);
                                }
                            }
                        }
                    }
                }, {//add by zhuoqin0830w  2015/04/02  添加 依原成本設定值
                    xtype: 'checkbox',
                    margin: '0 10 0 10',
                    boxLabel: AS_OLD_COST_VALUE,//依原成本設定值
                    id: 'chkCost',
                    colName: 'chkCost',
                    handler: function () {
                        getCost();
                    }
                }]
            }, {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                anchor: '100%',
                layout: {
                    type: 'hbox',
                    padding: '10',
                    pack: 'start',
                    align: 'top'
                },
                items: [{
                    xtype: 'datetimefield',
                    disabledMin: false,
                    disabledSec: false,
                    format: 'Y/m/d H:i:s',
                    fieldLabel: EVENT_START,
                    time: { hour: 0, min: 0, sec: 0 },// add by zhuoqin0830w  自定義時間空間的值  2015/03/18
                    id: 'time_start',
                    name: 'time_start',
                    margin: '0 5px',
                    editable: false
                    //value: new Date(year, month, day, hour, 0, 0, 0)
                }, {
                    xtype: 'button',
                    text: MODIFY_LIST,
                    listeners: {
                        click: function () {
                            var val = Ext.getCmp('time_start').getValue();//獲取開始時間
                            for (var i = 0, j = s_store.getCount() ; i < j; i++) {
                                var record = s_store.getAt(i);
                                record.set('event_start', val);
                            }
                        }
                    }
                }]
            }, {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                anchor: '100%',
                layout: {
                    type: 'hbox',
                    padding: '10',
                    pack: 'start',
                    align: 'top'
                },
                items: [{
                    xtype: 'datetimefield',
                    disabledMin: false,
                    disabledSec: false,
                    fieldLabel: EVENT_END,
                    time: { hour: 23, min: 59, sec: 59 },// add by zhuoqin0830w  自定義時間空間的值  2015/03/18
                    id: 'time_end',
                    name: 'time_end',
                    margin: '0 5px',
                    editable: false,
                    format: 'Y/m/d H:i:s'
                    //value: new Date(year, month, day, hour, 0, 0, 0)
                }, {
                    xtype: 'button',
                    text: MODIFY_LIST,
                    listeners: {
                        click: function () {
                            var val = Ext.getCmp('time_end').getValue();//獲取開始時間
                            for (var i = 0, j = s_store.getCount() ; i < j; i++) {
                                var record = s_store.getAt(i);
                                record.set('event_end', val);
                            }
                        }
                    }
                }]

            }]
        });
    }

    //價格列表的column 
    var siteColumns = new Array();
    siteColumns.push(c_pro_del);
    siteColumns = siteColumns.concat(c_pro_base);
    siteColumns.push(c_pro_type);
    siteColumns.push(c_pro_pricetype);
    //siteColumns.push(c_pro_spec);
    siteColumns.push(c_pro_status);
    siteColumns.push(c_pro_site);
    siteColumns.push(c_pro_userlevel);
    //siteColumns.push(c_pro_pricestatus);
    siteColumns.push(c_pro_itemoney);
    siteColumns.push(c_pro_cost);
    //獲取 按鈕選擇的 值 判斷 活動價和活動成本可編輯  add by  zhuoqin0830w  2015/01/10
    if (location.search.substr(location.search.lastIndexOf('=') + 1) == "1") {
        siteColumns.push(c_pro_itemeventmoney);
        siteColumns.push(c_pro_eventcost);
    } else if (location.search.substr(location.search.lastIndexOf('=') + 1) == "2") {
        siteColumns.push(c_pro_itemeventmoney_edit);
        siteColumns.push(c_pro_eventcost_edit);
    }
    siteColumns.push(c_pro_event_price_discount);
    siteColumns.push(c_pro_event_cost_discount);
    //siteColumns.push(c_pro_eventdate);
    siteColumns.push(c_pro_eventstart);
    siteColumns.push(c_pro_eventend);
    var siteProGrid = Ext.create('Ext.grid.Panel', {
        hidden: true,
        id: 'siteProGrid',
        store: s_store,
        height: document.documentElement.clientHeight <= 700 ? document.documentElement.clientHeight - 272 : document.documentElement.clientHeight - 252,
        columnLines: true,
        columns: siteColumns,
        listeners: {
            viewready: function () {
                setShow(siteProGrid, 'colName');
            },
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        plugins: [Ext.create('Ext.grid.plugin.CellEditing', {
            clicksToEdit: 1
        })
        ]
    });

    siteProGrid.on({
        edit: function (editor, e) {
            if (e.field == 'event_price_discount') {
                // e.record.set('event_price', (e.record.get('price') * (e.value * 0.01)).toFixed(0));
                //eidt by zhuoqin0830w  更改商品活動成本驗算公式使活動價乘以折扣  傳值給後臺  2015/03/03
                var cost_discount;
                if (e.record.data.event_cost_discount == "") { cost_discount = 0; }
                else { cost_discount = e.record.data.event_cost_discount; }
                var jsonArr = [{ price_master_id: e.record.data.price_master_id, price: e.record.data.price, event_price_discount: e.value, event_cost: e.record.data.event_cost, event_cost_discount: cost_discount }];
                ArithmeticalDiscount(s_store, jsonArr, "price_master_id", "event_price", "event_price_discount");
            }
            else if (e.field == 'event_cost_discount') {
                // e.record.set('event_cost', (e.record.get('cost') * (e.value * 0.01)).toFixed(0));
                //eidt by zhuoqin0830w  更改商品活動成本驗算公式使活動價乘以折扣  傳值給後臺  2015/03/03
                var jsonArr = [{ price_master_id: e.record.data.price_master_id, event_price: e.record.data.event_price, event_cost_discount: e.value }];
                ArithmeticalDiscount(s_store, jsonArr, "price_master_id", "event_cost", "event_cost_discount");
            }
        }
    });

    var save_button = Ext.create('Ext.form.Panel', {
        layout: 'column',
        id: 'save_button',
        width: 200,
        border: false,
        padding: '5 5',
        buttons: [
            {
                id: 'eventUpdate',
                xtype: 'button', text: BTN_SAVE,
                listeners: {
                    click: Save
                }
            },
            {
                xtype: 'button', text: CANCEL,
                listeners: {
                    click: function () { Search(); }
                }
            }]
    });

    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        border: false,
        items: [frm, siteProGrid, save_button],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                siteProGrid.height = document.documentElement.clientHeight <= 700 ? document.documentElement.clientHeight - 242 : document.documentElement.clientHeight - 260;
                this.doLayout();
            },
            afterrender: function () {
                ToolAuthorityQuery(function () {
                    setTimeout(Search, 500);
                });
            }
        }
    });
});

function Search() {
    //進行截取，并讀取 商品ID  edit by zhuoqin0830w  2015/02/10
    s_store.load({
        params: { priceMasterIds: location.search.substring(location.search.indexOf('=') + 1, location.search.lastIndexOf('&')) },
        callback: function () {

            Ext.each(s_store.data.items, function () {
                if (this.data.prepaid == 1) {
                    this.data.event_cost_discount = 100;
                }
            });

        }
    });
    Ext.getCmp('siteProGrid').show();
}

//計算折扣
function ArithmeticalDiscount(store, jsonArr, key, colVal, type) {
    Ext.Ajax.request({
        url: '/ProductSelect/ArithmeticalDiscount',
        method: "POST",
        params: { priceMasterCustoms: Ext.encode(jsonArr), type: type },
        success: function (form, action) {
            var results = Ext.decode(form.responseText);
            Ext.each(results, function () {
                store.findRecord(key, this[key]).set(colVal, this[colVal]);
                //add by zhuoqin0830w  2015/02/27  更改商品活動成本驗算公式使活動價乘以折扣
                if (colVal == "event_price")
                    store.findRecord(key, this[key]).set('event_cost', this["event_cost"]);
            });
        }
    });
}

Save = function () {

    Ext.Msg.confirm(CONFIRM, AREYOUSURE, function (btn) {
        if (btn == 'yes') {

            var myMask = new Ext.LoadMask(Ext.getBody(), {
                msg: 'Loading...'
            }); //edit by wangwei0216w 2014/08/6 (為儲存修改按鈕添加遮蓋層,解決多次點擊彈出多個保存成功問題)
            myMask.show();
            if (s_store.getCount() < 1) {
                return;
            }
            var priceMasters = [];
            //add by zhuoqin0830w  2015/04/02  獲取 依原成本設定值 是否選中
            var chkCost = Ext.getCmp("chkCost").getValue();




            //依折數修改

            //獲取 按鈕選擇的 值 判斷 是否為折扣價  add by  zhuoqin0830w  2015/01/10
            if (location.search.substr(location.search.lastIndexOf('=') + 1) == "1") {


                for (var i = 0, j = s_store.getCount() ; i < j; i++) {
                    var record = s_store.getAt(i);



                    var event_price_discount = record.get("event_price_discount");
                    var event_cost_discount = chkCost == true ? 0 : record.get("event_cost_discount");

                    var event_starts = record.get("event_start");
                    var event_ends = record.get("event_end");



                    if (!event_price_discount) {
                        myMask.hide();
                        Ext.Msg.alert(INFORMATION, EVENT_PRICE_DISCOUNT_ISNULL);
                        return;
                    }



                    if (event_starts == "1970/01/01 08:00:00" || event_ends == "1970/01/01 08:00:00" || event_starts == "" || event_ends == "") {
                        myMask.hide();
                        Ext.Msg.alert(INFORMATION, '請輸入活動時間');
                        return;
                    }

                    if (new Date(record.get("event_start")).getTime() / 1000 >= new Date(record.get("event_end")).getTime() / 1000) {
                        myMask.hide();
                        Ext.Msg.alert(INFORMATION, '請輸入正確的活動時間區間！');
                        return;
                    }


                    //eidt by zhuoqin0830w  2015/04/02  判斷 依原成本設定值 是否選中
                    if (!chkCost) {
                        if (!event_cost_discount) {
                            myMask.hide();
                            Ext.Msg.alert(INFORMATION, EVENT_COST_DISCOUNT_ISNULL);
                            return;
                        }
                    }
                    ///edit by wwei0216w 價格為0時不允許保存 2015/12/28
                    if (record.get("event_price") == 0 || record.get("event_cost") == 0){
                        Ext.Msg.alert('消息', '活動售價,活動成本不能為0!');
                        myMask.hide();
                        return
                    }
                    priceMasters.push({
                        "product_id": record.get("product_id"),
                        "price_master_id": record.get("price_master_id"),
                        "combination": record.get("combination_id"),
                        "price_type": record.get("price_type_id"),
                        "event_price_discount": event_price_discount,
                        "event_cost_discount": event_cost_discount,
                        "event_price": record.get("event_price"),
                        "event_cost": record.get("event_cost"),
                        "event_start": new Date(record.get("event_start")).getTime() / 1000,
                        "event_end": new Date(record.get("event_end")).getTime() / 1000
                    });

                }




                Ext.Ajax.request({
                    url: '/ProductSelect/SavePriceMaster',
                    params: {
                        priceMasters: JSON.stringify(priceMasters),
                        priceCondition: location.search.substr(location.search.lastIndexOf('=') + 1),
                        chkCost: Ext.getCmp("chkCost").getValue() == true ? 0 : 1
                    },
                    timeout: 360000,
                    success: function (response) {
                        var res = Ext.decode(response.responseText);
                        if (res.success) {
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                        myMask.hide();
                    },
                    failure: function (response, opts) {
                        if (response.timedout) {
                            Ext.Msg.alert(INFORMATION, TIME_OUT);
                        }
                        myMask.hide();
                    }
                });

            }




            //個別輸入
            if (location.search.substr(location.search.lastIndexOf('=') + 1) == "2") {

                var event_cp = 0;

                for (var i = 0, j = s_store.getCount() ; i < j; i++) {
                    var record = s_store.getAt(i);

                    var event_cost = record.get("event_cost");
                    var event_price = record.get("event_price");


                    var event_starts = record.get("event_start");
                    var event_ends = record.get("event_end");

                    if (event_starts == "1970/01/01 08:00:00" || event_ends == "1970/01/01 08:00:00" || event_starts == "" || event_ends == "") {
                        myMask.hide();
                        Ext.Msg.alert(INFORMATION, '請輸入活動時間');
                        return;
                    }

                    if (new Date(record.get("event_start")).getTime() / 1000 >= new Date(record.get("event_end")).getTime() / 1000) {
                        myMask.hide();
                        Ext.Msg.alert(INFORMATION, '請輸入正確的活動時間區間！');
                        return;
                    }

                    if (event_cost == '0' || event_price == '0') {
                        //event_cp = i + 1;
                        myMask.hide();
                        Ext.Msg.alert("消息", "活動價或活動成本為0,不能保存");
                        return;
                    }

                    priceMasters.push({
                        "product_id": record.get("product_id"),
                        "price_master_id": record.get("price_master_id"),
                        "combination": record.get("combination_id"),
                        "price_type": record.get("price_type_id"),
                        "event_price": record.get("event_price"),
                        "event_cost": record.get("event_cost"),
                        "event_start": new Date(record.get("event_start")).getTime() / 1000,
                        "event_end": new Date(record.get("event_end")).getTime() / 1000
                    });
                }

                if (event_cp > 0) {

                    //Ext.Msg.confirm(CONFIRM, '活動價或活動成本為0,確定保存嗎？', function (btntwo) {
                    //    if (btntwo == 'yes') {

                    //        Ext.Ajax.request({
                    //            url: '/ProductSelect/SavePriceMaster',
                    //            params: {
                    //                priceMasters: JSON.stringify(priceMasters),
                    //                priceCondition: location.search.substr(location.search.lastIndexOf('=') + 1),
                    //                chkCost: Ext.getCmp("chkCost").getValue() == true ? 0 : 1
                    //            },
                    //            timeout: 360000,
                    //            success: function (response) {
                    //                var res = Ext.decode(response.responseText);
                    //                if (res.success) {
                    //                    Ext.Msg.alert(INFORMATION, SUCCESS);
                    //                }
                    //                else {
                    //                    Ext.Msg.alert(INFORMATION, FAILURE);
                    //                }
                    //                myMask.hide();
                    //            },
                    //            failure: function (response, opts) {
                    //                if (response.timedout) {
                    //                    Ext.Msg.alert(INFORMATION, TIME_OUT);
                    //                }
                    //                myMask.hide();
                    //            }
                    //        });
                    //    } else {
                    //        myMask.hide();
                    //        return;
                    //    }

                    //})
                } else {

                    Ext.Ajax.request({
                        url: '/ProductSelect/SavePriceMaster',
                        params: {
                            priceMasters: JSON.stringify(priceMasters),
                            priceCondition: location.search.substr(location.search.lastIndexOf('=') + 1),
                            chkCost: Ext.getCmp("chkCost").getValue() == true ? 0 : 1
                        },
                        timeout: 360000,
                        success: function (response) {
                            var res = Ext.decode(response.responseText);
                            if (res.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                            myMask.hide();
                        },
                        failure: function (response, opts) {
                            if (response.timedout) {
                                Ext.Msg.alert(INFORMATION, TIME_OUT);
                            }
                            myMask.hide();
                        }
                    });
                }

            }

        }
    })
}



//add by zhuoqin0830w  2015/04/02  添加 依原成本設定值
function getCost() {
    if (Ext.getCmp("chkCost").getValue()) {
        Ext.getCmp("btn_ecDis").setDisabled(true);
        for (var i = 0, j = s_store.getCount() ; i < j; i++) {
            var record = s_store.getAt(i);
            record.set('event_cost', s_store.getAt(i).data.cost);
        }
    } else {
        Ext.getCmp("btn_ecDis").setDisabled(false);
        for (var i = 0, j = s_store.getCount() ; i < j; i++) {
            var record = s_store.getAt(i);
            record.set('event_cost', '0');
        }
    }
}
