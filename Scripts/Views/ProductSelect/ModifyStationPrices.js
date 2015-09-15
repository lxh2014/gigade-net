//var isDiscountClick = false, isCostDiscountClick = false;
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

var m_pro_gigade_price = { name: 'gigade_price', type: 'string' };
var m_pro_gigade_cost = { name: 'gigade_cost', type: 'string' };

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
            var priceMasterId = n_store.getAt(rowIndex).get('price_master_id');
            Ext.Msg.confirm(NOTICE, CONFIRM_DEL, function (btn) {
                if (btn == "yes") {
                    n_store.removeAt(rowIndex);
                }
            });
        }

    }]

};
var c_pro_base = [
    { header: PRODUCT_IMAGE, /*colName: 'product_image', hidden: true,*/ xtype: 'templatecolumn', tpl: '<div style="width:50px;height:50px"><img width="50px" height="50px" src="{product_image}" /><div>', width: 60, align: 'center', sortable: false, menuDisabled: true },
    { header: PRODUCT_ID, dataIndex: 'product_id',/* colName: 'product_id', hidden: true, xtype: 'templatecolumn', tpl: '<a href=# onclick="javascript:showDetail({product_id})" >{product_id}</a>', */width: 60, align: 'center', sortable: false, menuDisabled: true },
        { header: BRAND_NAME,/* colName: 'brand_id', hidden: true,*/ dataIndex: 'brand_name', width: 120, align: 'left', sortable: false, menuDisabled: true },
    {
        header: PRODUCT_NAME,/* colName: 'product_name', hidden: true,*/ dataIndex: 'product_name', width: 180, align: 'left', sortable: false, menuDisabled: true,
        editor: {
            xtype: 'field'
        }
        ,
        renderer: function (value, cellmeta, record) {
            return value + (record.data.prod_sz ? ' (' + record.data.prod_sz + ')' : '');
        }
    }
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
    header: ITEM_MONEY,/* colName: 'price', hidden: true,*/ dataIndex: 'price', width: 80, align: 'center', sortable: false, menuDisabled: true,
    field: { xtype: 'numberfield', allowBlank: false, minValue: 0, regex: /^[0-9]*$/ }  //edit by zhuoqin0830w  2015/03/25
};

var c_pro_itemoney_discount = {
    header: ITEM_MONEY,/* colName: 'price', hidden: true,*/ dataIndex: 'price', width: 80, align: 'center', sortable: false, menuDisabled: true
};

var c_pro_cost = {
    header: COST, /*colName: 'cost', hidden: true,*/ dataIndex: 'cost', width: 80, align: 'center', sortable: false, menuDisabled: true,
    field: { xtype: 'numberfield', allowBlank: false, minValue: 0, regex: /^[0-9]*$/ }  //edit by zhuoqin0830w  2015/03/25
};

var c_pro_cost_discount = {
    header: COST, /*colName: 'cost', hidden: true,*/ dataIndex: 'cost', width: 80, align: 'center', sortable: false, menuDisabled: true
};

var c_pro_eventcost = {
    header: EVENT_COST, /*colName: 'cost', hidden: true,*/ dataIndex: 'event_cost', width: 80, align: 'center', sortable: false, menuDisabled: true,
    field: { xtype: 'numberfield', allowBlank: false, minValue: 0, regex: /^[0-9]*$/ }  //edit by zhuoqin0830w  2015/03/25
    
};

var c_pro_eventcost_discount = {
    header: EVENT_COST, /*colName: 'cost', hidden: true,*/ dataIndex: 'event_cost', width: 80, align: 'center', sortable: false, menuDisabled: true
};

var c_gigade_price = { header: GIGADE_PRICE, /*colName: 'cost', hidden: true,*/ dataIndex: 'gigade_price', width: 80, align: 'center', sortable: false, menuDisabled: true };

var c_gigade_cost = { header: GIGADE_COST, /*colName: 'cost', hidden: true,*/ dataIndex: 'gigade_cost', width: 80, align: 'center', sortable: false, menuDisabled: true };

var c_pro_event_price_discount = { header: EVENT_PRICE_DISCOUNT, /*colName: 'event_price_discount', hidden: true,*/ dataIndex: 'event_price_discount', width: 80, align: 'center', sortable: false, menuDisabled: true, field: { xtype: 'numberfield', allowBlank: false, minValue: 1, maxValue: 99, regex: /^[0-9]*$/ } };//edit by hjiajun1211w 2014/08/07 確保輸入合法性
var c_pro_event_cost_discount = { header: EVENT_COST_DISCOUNT, /*colName: 'event_cost_discount', hidden: true,*/ dataIndex: 'event_cost_discount', width: 80, align: 'center', sortable: false, menuDisabled: true, field: { xtype: 'numberfield', allowBlank: false, minValue: 1, maxValue: 100, regex: /^[0-9]*$/ } };//edit by hjiajun1211w 2014/08/07 確保輸入合法性

var c_pro_itemeventmoney = {
    header: ITEM_EVENT_MONEY, /*colName: 'event_price', hidden: true,*/ dataIndex: 'event_price', width: 80, align: 'center', sortable: false, menuDisabled: true,
    field: { xtype: 'numberfield', allowBlank: false, minValue: 0, regex: /^[0-9]*$/ }  //edit by zhuoqin0830w  2015/03/25
};

var c_pro_itemeventmoney_discount = {
    header: ITEM_EVENT_MONEY, /*colName: 'event_price', hidden: true,*/ dataIndex: 'event_price', width: 80, align: 'center', sortable: false, menuDisabled: true
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
        time: { hour: 0, min: 0, sec: 0 },// add by zhuoqin0830w  自定義時間空間的值  2015/03/18
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
        time: { hour: 23, min: 59, sec: 59 },// add by zhuoqin0830w  自定義時間空間的值  2015/03/18
    }
};

var pageSize = 15;

//折扣頁面的column 
siteColumns_discount = new Array();
siteColumns_discount.push(c_pro_del);
siteColumns_discount = siteColumns_discount.concat(c_pro_base);
siteColumns_discount.push(c_pro_type);
siteColumns_discount.push(c_pro_pricetype);
siteColumns_discount.push(c_pro_status);
siteColumns_discount.push(c_gigade_price);
siteColumns_discount.push(c_gigade_cost);
siteColumns_discount.push(c_pro_itemoney_discount);
siteColumns_discount.push(c_pro_cost_discount);
siteColumns_discount.push(c_pro_itemeventmoney_discount);
siteColumns_discount.push(c_pro_eventcost_discount);
siteColumns_discount.push(c_pro_eventstart);
siteColumns_discount.push(c_pro_eventend);

//價格列表的column 
var siteColumns = new Array();
siteColumns.push(c_pro_del);
siteColumns = siteColumns.concat(c_pro_base);
siteColumns.push(c_pro_type);
siteColumns.push(c_pro_pricetype);
siteColumns.push(c_pro_status);
siteColumns.push(c_gigade_price);
siteColumns.push(c_gigade_cost);
siteColumns.push(c_pro_itemoney);
siteColumns.push(c_pro_cost);
siteColumns.push(c_pro_itemeventmoney);
siteColumns.push(c_pro_eventcost);
siteColumns.push(c_pro_eventstart);
siteColumns.push(c_pro_eventend);
//站台價格列
var site = new Array();
site = site.concat(m_pro_base);
site.push(m_pro_gigade_price);
site.push(m_pro_gigade_cost);
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
site.push({name:'prepaid',type:'int'});

Ext.define('GIGADE.SITEPRODUCT', {
    extend: 'Ext.data.Model',
    fields: site
});

var n_store = Ext.create('Ext.data.Store', {
    model: 'GIGADE.SITEPRODUCT',
    proxy: {
        type: 'ajax',
        url: '/ProductSelect/GetInfoByProductID',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'item'
        }
    }
});


var frm;
Ext.onReady(function () {
    Enter();
    var priceCondition = location.search.substr(location.search.lastIndexOf('=') + 1);
    if (priceCondition == "1") {
        var GridInfo = Ext.create('Ext.grid.Panel', {
            id: 'nProductShow',
            store: n_store,
            height: document.documentElement.clientHeight <= 700 ? document.documentElement.clientHeight - 302 : document.documentElement.clientHeight - 282,
            columnLines: true,
            columns: siteColumns_discount,
            listeners: {
                viewready: function () {
                    setShow(nProductShow, 'colName');
                },
                scrollershow: function (scroller) {
                    if (scroller && scroller.scrollEl) {
                        scroller.clearManagedListeners();
                        scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                    }
                }
            },
            plugins: [Ext.create('Ext.grid.plugin.CellEditing', { clicksToEdit: 1 })]
        });

    } else if (priceCondition == "2") {

        var GridInfo = Ext.create('Ext.grid.Panel', {
            id: 'nProductShow',
            store: n_store,
            height: document.documentElement.clientHeight <= 700 ? document.documentElement.clientHeight - 302 : document.documentElement.clientHeight - 282,
            columnLines: true,
            columns: siteColumns,
            listeners: {
                viewready: function () {
                    setShow(nProductShow, 'colName');
                },
                scrollershow: function (scroller) {
                    if (scroller && scroller.scrollEl) {
                        scroller.clearManagedListeners();
                        scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                    }
                }
            },
            plugins: [Ext.create('Ext.grid.plugin.CellEditing', { clicksToEdit: 1 })]
        });

    }
    CreateForm(priceCondition);

    var save_button = Ext.create('Ext.form.Panel', {
        layout: 'column',
        id: 'save_button',
        width: 200,
        border: false,
        padding: '5 5',
        buttons: [
            {
                id: 'newSitePrice',
                xtype: 'button',
                text: BE_SURE,
                disabled: true,
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
        items: [frm, GridInfo, save_button],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                nProductShow.height = document.documentElement.clientHeight <= 700 ? document.documentElement.clientHeight - 242 : document.documentElement.clientHeight - 260;
                this.doLayout();
            },
            afterrender: function () {
               // Ext.getCmp("site_id").setValue(siteStore.data.items[1].data.Site_Id);
                if (priceCondition == 2) {
                    Ext.getCmp('newSitePrice').setDisabled(false); //edit by ww0216w 2014/12/1
                }
                ToolAuthorityQuery(function () {
                    setTimeout(Search, 500);
                });
            }
        }
    });

});


function CreateForm(priceCondition) {
    if (priceCondition == 1) {
        frm = Ext.create('Ext.form.Panel', {
            layout: 'column',
            id: 'frm',
            width: 1300,
            border: false,
            padding: '5 0',
            defaults: { anchor: '95%', msgTarget: "side", margin: '3 0' },
            items: [{
                xtype: 'panel',
                columnWidth: .65,
                border: 0,
                layout: 'anchor',
                items: [{
                    xtype: 'fieldcontainer',
                    anchor: '100%',
                    layout: {
                        type: 'hbox',
                        padding: '10',
                        pack: 'start',
                        align: 'top'
                    },
                    items: [sitebox, user_level_group]
                }, {
                    xtype: 'fieldcontainer',
                    anchor: '100%',
                    layout: {
                        type: 'hbox',
                        padding: '10',
                        pack: 'start',
                        align: 'top'
                    },
                    //售價折數
                    items: [discount, {
                        id: 'btn_discount',
                        xtype: 'button',
                        margin: '0 5 0 5',
                        text: MODIFY_LIST,
                        listeners: {
                            click: function () {
                                var val = Ext.getCmp('discount').getValue();
                                if (Ext.getCmp('discount').isValid()) {
                                    //for (var i = 0, j = n_store.getCount() ; i < j; i++) {
                                    //    var record = n_store.getAt(i);
                                    //    record.set('price', (record.get('gigade_price') * (val * 0.01)).toFixed(0));
                                    //}
                                    //改為後台計算折扣
                                    var jsonArr = new Array();
                                    for (var i = 0, j = n_store.getCount() ; i < j; i++) {
                                        var record = n_store.getAt(i);
                                        //edit by zhuoqin0830w  2015/02/27  更改商品活動成本驗算公式使活動價乘以折扣  增加type屬性判斷是否是折扣
                                        jsonArr.push({ "product_id": record.data.product_id, "price": record.get('gigade_price'), "price_discount": val, "type": "discount" });
                                    }
                                    //edit by zhuoqin0830w  2015/03/03  更改商品活動成本驗算公式使活動價乘以折扣  增加type屬性判斷是否是折扣
                                    ArithmeticalDiscount(n_store, jsonArr, "product_id", "price", "price_discount");//計算折扣

                                    this.setDisabled(true);
                                    if (Ext.getCmp("btn_cost_discount").isDisabled())
                                        Ext.getCmp('newSitePrice').setDisabled(false); //edit by ww0216w 2014/12/1
                                }

                            }
                        }
                        //成本折數
                    }, cost_discount, {
                        id: 'btn_cost_discount',
                        xtype: 'button',
                        margin: '0 5 0 5',
                        text: MODIFY_LIST,
                        listeners: {
                            click: function () {
                                var val = Ext.getCmp('cost_discount').getValue();
                                if (Ext.getCmp('cost_discount').isValid()) {
                                    //for (var i = 0, j = n_store.getCount() ; i < j; i++) {
                                    //    var record = n_store.getAt(i);
                                    //    record.set('cost', (record.get('gigade_cost') * (val * 0.01)).toFixed(0));
                                    //}

                                    //改為後台計算折扣
                                    var jsonArr = new Array();
                                    for (var i = 0, j = n_store.getCount() ; i < j; i++) {
                                        var record = n_store.getAt(i);
                                        //edit by zhuoqin0830w  2015/02/27  更改商品活動成本驗算公式使活動價乘以折扣  增加type屬性判斷是否是折扣
                                        jsonArr.push({ "product_id": record.data.product_id, "cost": record.get('gigade_cost'), "cost_discount": val,"type": "discount" });
                                    }
                                    //edit by zhuoqin0830w  2015/03/03  更改商品活動成本驗算公式使活動價乘以折扣  增加type屬性判斷是否是折扣
                                    ArithmeticalDiscount(n_store, jsonArr, "product_id", "cost", "cost_discount");//計算折扣
                                    this.setDisabled(true);
                                    if (Ext.getCmp("btn_discount").isDisabled())
                                        Ext.getCmp('newSitePrice').setDisabled(false); //edit by ww0216w 2014/12/1
                                }
                            }
                        }
                    }

                    ]
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
                    //活動售價折數
                    items: [active_discount, {
                        id: 'btn_active_discount',
                        xtype: 'button',
                        margin: '0 5 0 5',
                        text: MODIFY_LIST,
                        listeners: {
                            click: function () {
                                var val = Ext.getCmp('active_discount').getValue();
                                if (Ext.getCmp('active_discount').isValid()) {
                                    //for (var i = 0, j = n_store.getCount() ; i < j; i++) {
                                    //    var record = n_store.getAt(i);
                                    //    record.set('event_price', (record.get('gigade_price') * (val * 0.01)).toFixed(0));
                                    //}

                                    //改為後台計算折扣
                                    var jsonArr = new Array();
                                    for (var i = 0, j = n_store.getCount() ; i < j; i++) {
                                        var record = n_store.getAt(i);
                                        //add by zhuoqin0830w  2015/02/27  更改商品活動成本驗算公式使活動價乘以折扣
                                        var active_cost_discount = Ext.getCmp('active_cost_discount').getValue();
                                        var cost_discount
                                        if (active_cost_discount == null) { cost_discount = 0; }
                                        else { cost_discount = active_cost_discount; }
                                        jsonArr.push({ "product_id": record.data.product_id, "price": record.get('gigade_price'), "event_price_discount": val, "event_cost_discount": cost_discount, "event_cost": record.data.event_cost, "event_price": record.data.event_price});
                                    }
                                    ArithmeticalDiscount(n_store, jsonArr, "product_id", "event_price", "event_price_discount");//計算折扣
                                }
                            }
                        }
                        //活動成本折數
                    }, active_cost_discount,{
                        id: 'btn_active_cost_discount',
                        xtype: 'button',
                        margin: '0 5 0 5',
                        text: MODIFY_LIST,
                        listeners: {
                            click: function () {
                                var val = Ext.getCmp('active_cost_discount').getValue();
                                //if (Ext.getCmp('active_cost_discount').isValid()) {
                                //    for (var i = 0, j = n_store.getCount() ; i < j; i++) {
                                //        var record = n_store.getAt(i);
                                //        record.set('event_cost', (record.get('gigade_cost') * (val * 0.01)).toFixed(0));
                                //    }
                                //}
                                //改為後台計算折扣
                                var jsonArr = new Array();
                                for (var i = 0, j = n_store.getCount() ; i < j; i++) {
                                    var record = n_store.getAt(i);
                                    jsonArr.push({ "product_id": record.data.product_id, "event_price": record.data.event_price, "event_cost_discount": val });
                                }
                                ArithmeticalDiscount(n_store, jsonArr, "product_id", "event_cost", "event_cost_discount");//計算折扣
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
                    xtype: 'fieldcontainer',//edit by Jiajun 2014/09/10
                    layout: 'hbox',
                    anchor: '100%',
                    layout: 'column',
                    items: [{
                        xtype: 'fieldcontainer',
                        layout: 'hbox',
                        anchor: '100%',
                        layout: 'column',
                        items: [{
                            xtype: 'datetimefield',
                            disabledMin: false,
                            disabledSec: false,
                            format: 'Y/m/d H:i:s',
                            fieldLabel: EVENT_START,
                            time: { hour: 0, min: 0, sec: 0 },// add by zhuoqin0830w  自定義時間空間的值  2015/03/18
                            id: 'time_start',
                            name: 'time_start',
                            margin: '0 10px',
                            editable: false
                        }, {
                            id: 'btn_time_start',
                            xtype: 'button',
                            text: MODIFY_LIST,
                            listeners: {
                                click: function () {
                                    var val = Ext.getCmp('time_start').getValue();
                                    if (Ext.getCmp('time_start').getValue() != null) {
                                        if (Ext.getCmp('time_start').isValid()) {
                                            for (var i = 0, j = n_store.getCount() ; i < j; i++) {
                                                var record = n_store.getAt(i);
                                                record.set('event_start', val);
                                            }
                                        }
                                    }
                                }
                            }
                        }]
                    }, {
                        xtype: 'fieldcontainer',
                        layout: 'hbox',
                        anchor: '100%',
                        layout: 'column',
                        margin: '0 10px',
                        items: [{
                            xtype: 'datetimefield',
                            disabledMin: false,
                            disabledSec: false,
                            format: 'Y/m/d H:i:s',
                            fieldLabel: EVENT_END,
                            time: { hour: 23, min: 59, sec: 59 },// add by zhuoqin0830w  自定義時間空間的值  2015/03/18
                            id: 'time_end',
                            name: 'time_end',
                            margin: '0 5px',
                            editable: false
                        }, {
                            id: 'btn_time_end',
                            xtype: 'button',
                            text: MODIFY_LIST,
                            value: 0,
                            listeners: {
                                click: function () {
                                    var val = Ext.getCmp('time_end').getValue();
                                    if (Ext.getCmp('time_end').getValue() != null) {
                                        if (Ext.getCmp('time_end').isValid()) {
                                            for (var i = 0, j = n_store.getCount() ; i < j; i++) {
                                                var record = n_store.getAt(i);
                                                record.set('event_end', val);
                                            }
                                        }
                                    }
                                }
                            }
                        }]
                    }]
                }]
            }, { //edit by jiajun 2014/08/28
                xtype: 'panel',
                columnWidth: .35,
                layout: 'anchor',
                border: 0,
                items: [{
                    xtype: 'fieldset',
                    margin: '50 0',
                    padding: '5 5 5 5 ',
                    items: [{
                        xtype: 'displayfield',
                        value: PRICE_DISCOUNT_DESC
                    }, {
                        xtype: 'displayfield',
                        value: COST_DISCOUNT_DESC
                    }, {
                        xtype: 'displayfield',
                        value: EVENT_PRICE_DISCOUNT_DESC
                    }, {
                        xtype: 'displayfield',
                        value: EVENT_COST_DISCOUNT_DESC
                    }]
                }]
            }]
        });
    } else if (priceCondition == 2) {
        frm = Ext.create('Ext.form.Panel', {
            layout: 'anchor',
            id: 'frm',
            width: 1185,
            border: false,
            padding: '8 0',
            defaults: { anchor: '95%', msgTarget: "side", margin: '3 0' },
            items: [{
                xtype: 'fieldcontainer',
                anchor: '100%',
                layout: {
                    type: 'hbox',
                    padding: '10',
                    pack: 'start',
                    align: 'top'
                },
                items: [sitebox, user_level_group]
            }, {
                xtype: 'fieldcontainer',
                anchor: '100%',
                layout: {
                    type: 'hbox',
                    padding: '10',
                    pack: 'start',
                    align: 'top'
                },
                items: [price, {
                    xtype: 'button',
                    id: 'btn_price',
                    margin: '0 5 0 5',
                    text: MODIFY_LIST,
                    listeners: {
                        click: function () {
                            var val = Ext.getCmp('price').getValue();
                            if (Ext.getCmp('price').isValid()) {
                                for (var i = 0, j = n_store.getCount() ; i < j; i++) {
                                    var record = n_store.getAt(i);
                                    record.set('price', val);
                                }
                            }
                        }
                    }
                }, cost, {
                    id: 'btn_cost',
                    xtype: 'button',
                    margin: '0 5 0 5',
                    text: MODIFY_LIST,
                    listeners: {
                        click: function () {
                            var val = Ext.getCmp('cost').getValue();
                            if (Ext.getCmp('cost').isValid()) {
                                for (var i = 0, j = n_store.getCount() ; i < j; i++) {
                                    var record = n_store.getAt(i);
                                    record.set('cost', val);
                                }
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
                items: [active_price, {
                    id: 'btn_active_price',
                    xtype: 'button',
                    margin: '0 5 0 5',
                    text: MODIFY_LIST,
                    listeners: {
                        click: function () {
                            var val = Ext.getCmp('active_price').getValue();
                            if (Ext.getCmp('active_price').isValid()) {
                                for (var i = 0, j = n_store.getCount() ; i < j; i++) {
                                    var record = n_store.getAt(i);
                                    record.set('event_price', val);
                                }
                            }
                        }
                    }
                }, activ_cost, {
                    id: 'btn_activ_cost',
                    xtype: 'button',
                    margin: '0 5 0 5',
                    text: MODIFY_LIST,
                    listeners: {
                        click: function () {
                            var val = Ext.getCmp('activ_cost').getValue();
                            if (Ext.getCmp('activ_cost').isValid()) {
                                for (var i = 0, j = n_store.getCount() ; i < j; i++) {
                                    var record = n_store.getAt(i);
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
                xtype: 'fieldcontainer',//edit by Jiajun 2014/09/10
                layout: 'hbox',
                anchor: '100%',
                layout: 'column',
                items: [{
                    xtype: 'fieldcontainer',
                    layout: 'hbox',
                    anchor: '100%',
                    layout: 'column',
                    items: [{
                        xtype: 'datetimefield',
                        disabledMin: false,
                        disabledSec: false,
                        format: 'Y/m/d H:i:s',
                        fieldLabel: EVENT_START,
                        time: { hour: 0, min: 0, sec: 0 },// add by zhuoqin0830w  自定義時間空間的值  2015/03/18
                        id: 'time_start',
                        name: 'time_start',
                        margin: '0 10px',
                        editable: false
                    }, {
                        id: 'btn_time_start',
                        xtype: 'button',
                        text: MODIFY_LIST,
                        listeners: {
                            click: function () {
                                var val = Ext.getCmp('time_start').getValue();
                                if (Ext.getCmp('time_start').getValue() != null) {
                                    if (Ext.getCmp('time_start').isValid()) {
                                        for (var i = 0, j = n_store.getCount() ; i < j; i++) {
                                            var record = n_store.getAt(i);
                                            record.set('event_start', val);
                                        }
                                    }
                                }
                            }
                        }
                    }]
                }, {
                    xtype: 'fieldcontainer',
                    layout: 'hbox',
                    anchor: '100%',
                    layout: 'column',
                    margin: '0 10px',
                    items: [{
                        xtype: 'datetimefield',
                        disabledMin: false,
                        disabledSec: false,
                        format: 'Y/m/d H:i:s',
                        fieldLabel: EVENT_END,
                        time: { hour: 23, min: 59, sec: 59 },// add by zhuoqin0830w  自定義時間空間的值  2015/03/18
                        id: 'time_end',
                        name: 'time_end',
                        margin: '0 5px',
                        editable: false
                    }, {
                        id: 'btn_time_end',
                        xtype: 'button',
                        text: MODIFY_LIST,
                        value: 0,
                        listeners: {
                            click: function () {
                                var val = Ext.getCmp('time_end').getValue();
                                if (Ext.getCmp('time_end').getValue() != null) {
                                    if (Ext.getCmp('time_end').isValid()) {
                                        for (var i = 0, j = n_store.getCount() ; i < j; i++) {
                                            var record = n_store.getAt(i);
                                            record.set('event_end', val);
                                        }
                                    }
                                }
                            }
                        }
                    }]
                }]
            }]
        });
    }
}

function Search() {
    Ext.getCmp('nProductShow').show();
    n_store.load({ params: { productIds: location.search.substring(location.search.indexOf('=') + 1, location.search.lastIndexOf('&')) } });
}

function Save() {
    if (!frm.getForm().isValid()) {
        return;
    }
    //if (!Ext.comform()) {
    //    return;
    //    }
    Ext.Msg.confirm(TITLE_MSG, CONDITION_MSG, function (e) {
        if (e == "no") {
            return;
        }
        var priceCondition = location.search.substr(location.search.lastIndexOf('=') + 1);
        var myMask = new Ext.LoadMask(Ext.getBody(), {
            msg: 'Loading...'
        });
        myMask.show();
        if (n_store.getCount() < 1) {
            return;
        }
        var priceMasters = [];
        for (var i = 0, j = n_store.getCount() ; i < j; i++) {
            var record = n_store.getAt(i);
            if (priceCondition == "1") {
                priceMasters.push({
                    "price_discount": Ext.getCmp('discount').getValue() ? Ext.getCmp('discount').getValue() : 0,//獲得折數
                    "cost_discount": Ext.getCmp('cost_discount').getValue() ? Ext.getCmp('cost_discount').getValue() : 0,//獲得成本折數
                    "event_price_discount": Ext.getCmp('active_discount').getValue() ? Ext.getCmp('active_discount').getValue() : 0,//獲得活動售價折數
                    "event_cost_discount": Ext.getCmp('active_cost_discount').getValue() ? Ext.getCmp('active_cost_discount').getValue() : 0,//獲得活動成本折
                    "site_id": Ext.getCmp('site_id').getValue(), //站臺ID
                    "product_id": record.get("product_id"),//商品ID
                    "user_level": Ext.getCmp('user_level').getValue(),//獲取會員等級
                    "product_name": record.get("product_name"),  //商品名稱
                    "prod_sz": record.get("prod_sz"),
                    "price": record.get("gigade_price"),//原價
                    "cost": record.get("gigade_cost"),//原成本
                    "price_at": record.get("price"),//現售價
                    "cost_at": record.get("cost"),//現成本
                    "event_price": record.get("event_price"),//活動價
                    "event_cost": record.get("event_cost"),//活動成本
                    "combination": record.get("combination_id"),//商品類型
                    "price_type": record.get("price_type_id"),//價格類型
                    "event_start": new Date(record.get("event_start")).getTime() / 1000,//開始時間
                    "event_end": new Date(record.get("event_end")).getTime() / 1000//結束時間   
                });
            } else if (priceCondition == "2") {
                priceMasters.push({
                    "site_id": Ext.getCmp('site_id').getValue(), //站臺ID
                    "product_id": record.get("product_id"),//商品ID
                    "user_level": Ext.getCmp('user_level').getValue(),//獲取會員等級
                    "product_name": record.get("product_name"),  //商品名稱
                    "prod_sz":record.get("prod_sz"),
                    "event_price": record.get("event_price"), //? record.get("event_price"):0,//活動價
                    "event_cost": record.get("event_cost"), //? record.get("event_cost"):0  ,//活動成本
                    "price_at": record.get("price"),//現售價
                    "cost_at": record.get("cost"),//現成本
                    "combination": record.get("combination_id"),//商品類型
                    "price_type": record.get("price_type_id"),//價格類型
                    "event_start": new Date(record.get("event_start")).getTime() / 1000,//開始時間
                    "event_end": new Date(record.get("event_end")).getTime() / 1000//結束時間 
                });
            }
        }

        var priceMasters = JSON.stringify(priceMasters);
        Ext.Ajax.request({
            url: '/ProductSelect/AddSitePriceByGroup',
            params: {
                priceMasters: priceMasters,
                priceCondition: priceCondition,
                chkCost: Ext.getCmp("chkCost").getValue() == true ? 0 : 1
            },
           // timeout: 360000,
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                myMask.hide();
                if (result.success) {
                    Ext.Msg.alert(MESSAGEINFO, SAVESUCCESS);
                } else {
                    Ext.Msg.alert(MESSAGEINFO, result.msg ? (PRODUCTMSG + result.msg) : SAVE_FILED);//保存失敗！
                }
            },
            failure: function (response, opts) {
                if (response.timedout) {
                    Ext.Msg.alert(INFORMATION, TIME_OUT);
                }
                myMask.hide();
            }
        });
    });


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

//add by wwei0216w 2014/11/27 根據enter進行確認
function Enter() {
    document.body.onkeydown = function () {
        var name = event.target.name
        if (event.keyCode == 13) {
            switch (name) {
                case 'discount':
                    Ext.getCmp("btn_discount").fireEvent("click");
                    break;
                case 'cost_discount':
                    Ext.getCmp("btn_cost_discount").fireEvent("click");
                    break;
                case 'active_discount':
                    Ext.getCmp("btn_active_discount").fireEvent("click");
                    break;
                case 'active_cost_discount':
                    Ext.getCmp("btn_active_cost_discount").fireEvent("click");
                    break;
                case 'price':
                    Ext.getCmp("btn_price").fireEvent("click");
                    break;
                case 'cost':
                    Ext.getCmp("btn_cost").fireEvent("click");
                    break;
                case 'active_price':
                    Ext.getCmp("btn_cost").fireEvent("click");
                    break;
                case 'activ_cost':
                    Ext.getCmp("btn_activ_cost").fireEvent("click");
                    break;
                case 'time_start':
                    Ext.getCmp("btn_time_start").fireEvent("click");
                    break;
                case 'time_end':
                    Ext.getCmp("btn_time_end").fireEvent("click");
                    break;
                default:
                    break;
            }
        }
    };
}

//add by zhuoqin0830w  2015/04/02  添加 依原成本設定值
function getCost() {
    if (location.search.substr(location.search.lastIndexOf('=') + 1) == 1) {
        if (Ext.getCmp("chkCost").getValue()) {
            Ext.getCmp("btn_active_cost_discount").setDisabled(true);
            for (var i = 0, j = n_store.getCount() ; i < j; i++) {
                var record = n_store.getAt(i);
                record.set('event_cost', n_store.getAt(i).data.gigade_cost);
            }
        } else {
            Ext.getCmp("btn_active_cost_discount").setDisabled(false);
            for (var i = 0, j = n_store.getCount() ; i < j; i++) {
                var record = n_store.getAt(i);
                record.set('event_cost', '0');
            }
        }
    } else {
        if (Ext.getCmp("chkCost").getValue()) {
            Ext.getCmp("btn_activ_cost").setDisabled(true);
            for (var i = 0, j = n_store.getCount() ; i < j; i++) {
                var record = n_store.getAt(i);
                record.set('event_cost', n_store.getAt(i).data.gigade_cost);
            }
        } else {
            Ext.getCmp("btn_activ_cost").setDisabled(false);
            for (var i = 0, j = n_store.getCount() ; i < j; i++) {
                var record = n_store.getAt(i);
                record.set('event_cost', '0');
            }
        }
    }
}