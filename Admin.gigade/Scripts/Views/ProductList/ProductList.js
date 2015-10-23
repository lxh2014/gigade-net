var pageSize = 15;

var pro = [
    { name: 'product_image', type: 'string' },
    { name: 'product_id', type: 'string' },
    { name: 'brand_name', type: 'string' },
    { name: 'product_name', type: 'string' },
    { name: 'prod_sz', type: 'string' }
];

pro.push({ name: 'combination', type: 'string' });
pro.push({ name: 'price_type', type: 'string' });//add by hufeng0813w 2014/06/18 Reason:商品列表中加上價格類型
pro.push({ name: 'combination_id', type: 'string' });
pro.push({ name: 'product_spec', type: 'string' });
pro.push({ name: 'product_price_list', type: 'string' });
pro.push({ name: 'product_status', type: 'string' });
pro.push({ name: 'sale_name', type: 'string' });//2015/02/05
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
//供應商
pro.push({ name: 'create_channel', type: 'string' });
//添加館別欄位 add  by zhuoqin0830w 2015/03/05
pro.push({ name: 'Prod_Classify', type: 'int' });
//添加是否失格  add  by zhuoqin0830w 2015/06/30
pro.push({ name: 'off_grade', type: 'int' });
//添加預購商品 guodong1130w 2015/9/16添加
pro.push({ name: 'purchase_in_advance', type: 'int' });
//2015/08/12
pro.push({ name: 'itemIds', type: 'string' });
//add by dongya   2015/10/16
pro.push({ name: 'outofstock_days_stopselling', type: 'int' });
//add by dongya  2015/10/22
pro.push({ name: 'outofstock_create_time', type: 'string' });
Ext.define('GIGADE.PRODUCT', {
    extend: 'Ext.data.Model',
    fields: pro
});

var p_store = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.PRODUCT',
    proxy: {
        type: 'ajax',
        url: '/ProductList/QueryProList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'item',
            totalProperty: 'totalCount'
        }
    }
});

p_store.on("beforeload", function () {
    Ext.apply(p_store.proxy.extraParams, {
        brand_id: Ext.getCmp('brand_id') ? Ext.getCmp('brand_id').getValue() : '',
        comboProCate_hide: Ext.getCmp('comboProCate_hide') ? Ext.getCmp('comboProCate_hide').getValue() : '',
        comboFrontCage_hide: Ext.getCmp('comboFrontCage_hide') ? Ext.getCmp('comboFrontCage_hide').getValue() : '',
        combination: Ext.getCmp('combination') ? Ext.getCmp('combination').getValue() : '',
        outofstock_time_days: Ext.getCmp('outofstock_time_days') ? Ext.getCmp('outofstock_time_days').getValue() : '',//add by dongya 2015/10/22
        product_status: Ext.getCmp('product_status') ? Ext.getCmp('product_status').getValue() : '',
        product_type: Ext.getCmp('product_type') ? Ext.getCmp('product_type').getValue() : '',//add 2015/06/01
        product_freight_set: Ext.getCmp('product_freight_set') ? Ext.getCmp('product_freight_set').getValue() : '',
        product_mode: Ext.getCmp('product_mode') ? Ext.getCmp('product_mode').getValue() : '',
        tax_type: Ext.getCmp('tax_type') ? Ext.getCmp('tax_type').getValue() : '',
        date_type: Ext.getCmp('date_type') ? Ext.getCmp('date_type').getValue() : '',
        time_start: Ext.getCmp('time_start') ? Ext.getCmp('time_start').getValue() : '',
        time_end: Ext.getCmp('time_end') ? Ext.getCmp('time_end').getValue() : '',
        // key: Ext.getCmp('key') ? Ext.htmlEncode(Ext.getCmp('key').getValue()) : '',//原本的
        key: Ext.getCmp('key').getValue() != "" ? Ext.getCmp('key').getValue() : document.getElementById('product_id').value,
        price_check: Ext.getCmp('price_check') ? Ext.getCmp('price_check').getValue() : '',
        site_id: Ext.getCmp('site_id') ? Ext.getCmp('site_id').getValue() : '',
        user_level: Ext.getCmp('user_level') ? Ext.getCmp('user_level').getValue() : '',
        price_status: Ext.getCmp('price_status') ? Ext.getCmp('price_status').getValue() : '',
        stockStatus: Ext.getCmp('stockStatus') ? Ext.getCmp('stockStatus').getValue() : '',//庫存類型
        //添加失格商品 查詢條件 
        off_grade: Ext.getCmp("off_grade").getValue() ? 1 : 0,
        //添加預購商品 查詢條件guodong1130w 2015/9/16添加
        purchase_in_advance: Ext.getCmp('purchase_in_advance').getValue() ? 1 : 0
    });
});

var site = new Array();
site = site.concat(m_pro_base);
site.push(m_pro_type);
site.push(m_pro_type_id);
site.push(m_pro_spec);
site.push(m_pro_status);
site.push(m_pro_status_id);
site.push(m_pro_site);
site.push(m_pro_site_id); //add by hufeng0813w 2014/05/22 站臺ID
site.push(m_pro_level); //add by hufeng0813w 2014/05/22 會員等級ID
site.push(m_pro_master_user_id); //add by hufeng0813w 2014/05/22 會員ID
site.push(m_pro_userlevel); //會員等級
site.push(m_pro_pricestatus);
site.push(m_pro_itemmoney);
site.push(m_pro_itemeventmoney);
site.push(m_pro_eventstart);
site.push(m_pro_eventend);
site.push(m_CanSel);
site.push(m_CanDo);
//添加館別欄位 add  by zhuoqin0830w 2015/03/05
site.push(m_prod_classify);
//添加是否失格  add  by zhuoqin0830w 2015/06/30
site.push(off_grade);
//添加預購商品 guodong1130w 2015/9/16添加
site.push(m_purchase_in_advance);

site.push(m_outofstock_days_stopselling);//add by dongya 2015/10/18

site.push(m_outofstock_create_time);//add by dongya 2015/10/22
Ext.define('GIGADE.SITEPRODUCT', {
    extend: 'Ext.data.Model',
    fields: site
});

var s_store = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.SITEPRODUCT',
    proxy: {
        type: 'ajax',
        url: '/ProductList/QueryProList',
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
        brand_id: Ext.getCmp('brand_id') ? Ext.getCmp('brand_id').getValue() : '',
        comboProCate_hide: Ext.getCmp('comboProCate_hide') ? Ext.getCmp('comboProCate_hide').getValue() : '',
        comboFrontCage_hide: Ext.getCmp('comboFrontCage_hide') ? Ext.getCmp('comboFrontCage_hide').getValue() : '',
        combination: Ext.getCmp('combination') ? Ext.getCmp('combination').getValue() : '',
        outofstock_time_days: Ext.getCmp('outofstock_time_days') ? Ext.getCmp('outofstock_time_days').getValue() : '',//add by dongya 2015/10/22
        product_status: Ext.getCmp('product_status') ? Ext.getCmp('product_status').getValue() : '',
        product_type: Ext.getCmp('product_type') ? Ext.getCmp('product_type').getValue() : '',//add 2015/06/01
        product_freight_set: Ext.getCmp('product_freight_set') ? Ext.getCmp('product_freight_set').getValue() : '',
        product_mode: Ext.getCmp('product_mode') ? Ext.getCmp('product_mode').getValue() : '',
        tax_type: Ext.getCmp('tax_type') ? Ext.getCmp('tax_type').getValue() : '',
        date_type: Ext.getCmp('date_type') ? Ext.getCmp('date_type').getValue() : '',
        time_start: Ext.getCmp('time_start') ? Ext.getCmp('time_start').getValue() : '',
        time_end: Ext.getCmp('time_end') ? Ext.getCmp('time_end').getValue() : '',
        key: Ext.getCmp('key').getValue() != "" ? Ext.getCmp('key').getValue() : document.getElementById('product_id').value,
        //key: Ext.getCmp('key') ? Ext.htmlEncode(Ext.getCmp('key').getValue()) : '',//原本的
        price_check: Ext.getCmp('price_check') ? Ext.getCmp('price_check').getValue() : '',
        site_id: Ext.getCmp('site_id') ? Ext.getCmp('site_id').getValue() : '',
        user_level: Ext.getCmp('user_level') ? Ext.getCmp('user_level').getValue() : '',
        price_status: Ext.getCmp('price_status') ? Ext.getCmp('price_status').getValue() : '',
        stockStatus: Ext.getCmp('stockStatus') ? Ext.getCmp('stockStatus').getValue() : '',//庫存類型
        off_grade: Ext.getCmp("off_grade").getValue() ? 1 : 0,
        //添加預購商品 查詢條件guodong1130w 2015/9/16添加
        purchase_in_advance: Ext.getCmp('purchase_in_advance').getValue() ? 1 : 0
    });
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            var tls = Ext.getCmp("proGrid").query('*[ttype=tbar]');
            if (selections.length > 0) {
                Ext.Array.each(tls, function () {
                    var disabled = false;
                    switch (this.text) {
                        case DELETE://刪除 新建商品 可以刪除
                            Ext.Array.each(selections, function (val) {
                                if (val.data.product_status_id != 0) {
                                    disabled = true;
                                }
                            });
                            break;
                        case PRODUCT_UP://上架  審核通過可以上架  但是失格商品 不可上架  || val.isModified("CanSel")
                            Ext.Array.each(selections, function (val) {
                                //edit by zhuoqin0830w  2015/06/30   || val.data.off_grade == 1  失格商品 不可上架
                                if (val.data.product_status_id != 2 || val.data.off_grade == 1) {
                                    disabled = true;
                                }
                            });
                            break;
                        case PRODUCT_DOWN://下架   上架商品 可以下架  || val.isModified("CanSel")
                            Ext.Array.each(selections, function (val) {
                                if (val.data.product_status_id != 5) {
                                    disabled = true;
                                }
                            });
                            break;
                        case PRODUCT_APPLY:// 申請審核 新建商品 下架商品  但是   失格商品 不可申請審核  || val.isModified("CanSel")
                            Ext.Array.each(selections, function (val) {
                                //edit by zhuoqin0830w  2015/06/30   || val.data.off_grade == 1  失格商品 不可申請審核
                                if ((val.data.product_status_id != 0 && val.data.product_status_id != 6 && val.data.product_status_id != 7) || val.data.product_id.length < 5 || val.data.off_grade == 1) {//edit 2015/04/22
                                    disabled = true;
                                }
                            });
                            break;
                        case CANCEL_APPLY:// 取消送審  || val.isModified("CanSel") 
                            Ext.Array.each(selections, function (val) {
                                if (val.data.product_status_id != 1 || val.data.CanDo != 1) {
                                    disabled = true;
                                }
                            });
                            break;
                        default:
                            disabled = false;
                            break;
                    }
                    this.setDisabled(disabled);
                });
            }
            else { Ext.Array.each(tls, function () { this.setDisabled(true); }); }
        }
    },
    storeColNameForHide: 'CanSel'
});

var site_sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            var tls = Ext.getCmp("siteProGrid").query('*[ttype=tbar]');
            if (selections.length > 0) {
                Ext.Array.each(tls, function () {
                    var disabled = false;
                    switch (this.text) {
                        case DELETE://刪除 新建商品 可以刪除
                            Ext.Array.each(selections, function (val) {
                                if (val.data.product_status_id != 0) {
                                    disabled = true;
                                }
                            });
                            break;
                        case PRODUCT_UP://上架  審核通過可以上架  但是失格商品 不可上架  || val.isModified("CanSel")
                            Ext.Array.each(selections, function (val) {
                                //edit by zhuoqin0830w  2015/06/30   || val.data.off_grade == 1  失格商品 不可上架
                                if (val.data.product_status_id != 2 || val.data.off_grade == 1) {
                                    disabled = true;
                                }
                            });
                            break;
                        case PRODUCT_DOWN://下架   上架商品 可以下架  || val.isModified("CanSel")
                            Ext.Array.each(selections, function (val) {
                                if (val.data.product_status_id != 5 || val.data.off_grade == 1) {
                                    disabled = true;
                                }
                            });
                            break;
                        case PRODUCT_APPLY://edit 2015/04/22  // 申請審核 新建商品 下架商品  但是   失格商品 不可申請審核  || val.isModified("CanSel")
                            Ext.Array.each(selections, function (val) {
                                //edit by zhuoqin0830w  2015/06/30   || val.data.off_grade == 1  失格商品 不可申請審核
                                if ((val.data.product_status_id != 0 && val.data.product_status_id != 6 && val.data.product_status_id != 7) || val.data.product_id.length < 5 || val.data.off_grade == 1) {
                                    disabled = true;
                                }
                            });
                            break;
                        case CANCEL_APPLY:// 取消送審  || val.isModified("CanSel") 
                            Ext.Array.each(selections, function (val) {
                                if (val.data.product_status_id != 1 || val.data.CanDo != 1) {
                                    disabled = true;
                                }
                            });
                            break;
                        default:
                            disabled = false;
                            break;
                    }
                    this.setDisabled(disabled);
                });
            }
            else { Ext.Array.each(tls, function () { this.setDisabled(true); }); }
        }
    },
    storeColNameForHide: 'CanSel'
});

//下架備註欄位Store  add by zhuoqin0830w  2015/06/25
var UnShelveStore = Ext.create('Ext.data.Store', {
    id: 'UnShelveStore',
    fields: ['ParameterCode', 'parameterName'],
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryParaByXmlTop?paraType=Off_Grade&topValue=1',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//下架不販售備註欄位Store  add by zhuoqin0830w  2015/06/25
var UnShelveSellStore = Ext.create('Ext.data.Store', {
    id: 'UnShelveSellStore',
    fields: ['ParameterCode', 'parameterName'],
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryParaByXmlTop?paraType=Off_Grade&topValue=2',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//上架備註欄位Store  add by zhuoqin0830w  2015/06/25
var PutAwayStore = Ext.create('Ext.data.Store', {
    id: 'PutAwayStore',
    fields: ['ParameterCode', 'parameterName'],
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryParaByXmlTop?paraType=Off_Grade&topValue=3',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

Ext.onReady(function () {
    //回車鍵查詢
    // edit by zhuoqin0830w  2015/09/22  以兼容火狐瀏覽器
    document.onkeydown = function (event) {
        e = event ? event : (window.event ? window.event : null);
        if (e.keyCode == 13) {
            $("#btn_search").click();
        }
    };

    var frm = Ext.create('Ext.form.Panel', {
        layout: 'column',
        id: 'frm',
        width: 1185,
        border: false,
        padding: '5 5',
        defaults: { anchor: '95%', msgTarget: "side" },
        items: [{
            xtype: 'panel',
            columnWidth: .70,
            border: 0,
            layout: 'anchor',
            items: [{
                xtype: 'fieldcontainer',
                layout: 'hbox',
                anchor: '100%',
                margin: '0 0',
                items: [brand, stockStatus,
                {//edit by guodong1130w  2015/09/16 添加預購商品查詢欄位
                    xtype: 'checkbox',
                    margin: '0 0 0 10',
                    boxLabel: PURCHASE_IN_ADVANCE_LABLE,
                    id: 'purchase_in_advance'
                }]
            }
           , category, {
               xtype: 'fieldcontainer',
               layout: 'hbox',
               anchor: '100%',
               items: [type, pro_status, product_type]
           }, freight_mode_tax, start_end, {
               xtype: 'container',
               layout: 'hbox',
               items: [key_query, {//edit by zhuoqin0830w  2015/06/30 添加失格查詢欄位
                   xtype: 'checkbox',
                   margin: '0 0 0 10',
                   boxLabel: QUALIFICATION_LOSE,
                   id: 'off_grade'
               },
              //add dongya 2015/10/22 缺貨天數查詢條件
              {
               xtype: 'numberfield',
               labelWidth: 100,
               margin: '0 20 0 50',
               hidden:true,
               fieldLabel: '缺貨天數(>=)',
               emptyText: '請輸入缺貨下架天數',
               minValue: 1,
               maxValue:9999,
               allowDecimals: false,
               id: 'outofstock_time_days',
               name: 'outofstock_time_days',
               value:1
               }
               ]
           }]
        }, {
            xtype: 'panel',
            columnWidth: .30,
            layout: 'anchor',
            border: 0,
            items: [price_check, {
                xtype: 'fieldset',
                padding: '5 5',
                items: [sitebox, user_level, price_status]
            }]
        }],
        buttonAlign: 'center',
        buttons: [{
            text: SEARCH,
            id: 'btn_search',
            handler: Search,
            iconCls: 'ui-icon ui-icon-search-2'
        }, {
            text: RESET,
            id: 'btn_reset',
            iconCls: 'ui-icon ui-icon-reset',
            listeners: {
                click: function () {
                    frm.getForm().reset();
                    Ext.getCmp("site_id").setValue(siteStore.data.items[0].data.Site_Id);
                    Ext.getCmp("user_level").setValue(userlevelStore.data.items[0].data.parameterCode);
                    Ext.getCmp("combination").setValue(ComboStore.data.items[0].data.parameterCode);
                    Ext.getCmp("product_status").setValue(prodStatusStore.data.items[0].data.parameterCode);
                    Ext.getCmp("date_type").setValue(DateStore.data.items[0].data.code);
                    Ext.getCmp("price_status").setValue(priceStatusStore.getAt(0).data.parameterCode);
                    Ext.getCmp("product_freight_set").setValue(freightStore.getAt(0).data.parameterCode);
                    Ext.getCmp("product_mode").setValue(modeStore.getAt(0).data.parameterCode);
                    Ext.getCmp("tax_type").setValue(taxStore.getAt(0).data.value);
                    //移除重置后時間的緩存  add by zhuoqin0830w  2015/09/15
                    Ext.getCmp("time_start").setMaxValue("");
                    Ext.getCmp("time_end").setMinValue("");
                    Ext.getCmp("time_end").setMaxValue("");
                    //add by dongya 2015/10/22
                    Ext.getCmp("outofstock_time_days").setMinValue(1);
                    Ext.getCmp("outofstock_time_days").hide();
                }
            }
        }]
    });

    //add by Jiajun 2014/10/20 
    var dataDown = {
        text: PRODUCT_DATA_OUT, colName: 'dataDown', hidden: true,
        iconCls: 'ui-icon ui-icon-paper-out',
        menu: {
            items: [{
                text: STOCK_DATA_OUT,
                scale: 'small',
                id: "kcdata",
                width: 130,
                handler: function () { exportProduct(1); }
            }, {
                text: PRODUCT_DATA_OUT,
                id: "spdata",
                scale: 'small',
                width: 130,
                handler: function () { exportProduct(2); }
            }, {
                text: PURCHASE_IN_ADVANCE_DATA_OUT,
                id: "ygdata",
                scale: 'small',
                width: 130,
                handler: function () { exportProduct(5); }
            }, '-', {
                text: PRODUCT_PRICE_OUT,
                id: "jgdata",
                scale: 'small',
                width: 130,
                handler: function () { exportProduct(3); }
            }, {
                text: PRODUCT_DETAILS_PRICE_OUT,
                id: "spxxdata",
                scale: 'small',
                width: 130,
                handler: function () { exportProduct(4); }
            }],
            listeners: {
                activate: function (e) {
                    var purchase_in_advance = Ext.htmlEncode(Ext.getCmp('purchase_in_advance').getValue() ? 1 : 0);//添加預購商品 guodong1130w 2015/9/16添加
                    if (Ext.getCmp('proGrid').isVisible()) {
                        e.items.get("kcdata").enable();
                        e.items.get("spdata").enable();
                        if (purchase_in_advance == 1) {
                            e.items.get("ygdata").enable();
                        }
                        else {
                            e.items.get("ygdata").disable();
                        }
                        e.items.get("jgdata").disable();
                        e.items.get("spxxdata").disable();
                    } else {
                        e.items.get("kcdata").disable();
                        e.items.get("spdata").disable();
                        e.items.get("ygdata").disable();
                        e.items.get("jgdata").enable();
                        e.items.get("spxxdata").enable();
                    }
                }
            }
        }
    }

    //下架不販售 2015/03/20
    var productDown = {
        text: PRODUCT_DOWN, colName: 'down', hidden: true, disabled: true, ttype: 'tbar',
        iconCls: 'ui-icon ui-icon-down',
        menu: {
            items: [{
                text: PRODUCT_DOWN,
                scale: 'small',
                id: "xj",
                width: 80,
                handler: function () { onDownClick(); }
            }, {
                text: PRODUCT_DOWN_NON_TRAFFIC,
                id: "xjbfs",
                scale: 'small',
                width: 80,
                handler: function () { onDonoCilck(); }
            }]
        }
    }

    var tools = [
        { text: PRODUCT_UPDATE, colName: 'update', hidden: true, ttype: 'tbar', iconCls: 'ui-icon ui-icon-table-confi', disabled: true, handler: onEditClick },
        { text: PRODUCT_APPLY, colName: 'apply', hidden: true, ttype: 'tbar', iconCls: 'ui-icon ui-icon-dis', disabled: true, handler: onApplyClick },
        { text: PRODUCT_UP, colName: 'up', hidden: true, ttype: 'tbar', iconCls: 'ui-icon ui-icon-up', disabled: true, handler: onUpClick }, productDown,
        //{ text: PRODUCT_DOWN, colName: 'down', hidden: true, ttype: 'tbar', iconCls: 'ui-icon ui-icon-down', disabled: true, handler: onDownClick },
        { text: DELETE, colName: 'delete', hidden: true, ttype: 'tbar', iconCls: 'icon-remove', disabled: true, handler: onDeleteClick },
        { text: CANCEL_APPLY, colName: 'cancel', hidden: true, ttype: 'tbar', iconCls: 'ui-icon ui-icon-noin ', disabled: true, handler: onCanCelClick },
        { text: SORT_SET, colName: 'sort', hidden: true, ttype: '', iconCls: 'ui-icon ui-icon-conf-edit', handler: onSortClick },
        { text: OUT_PRODUCT, colName: 'export', hidden: true, iconCls: 'ui-icon ui-icon-paper-out', handler: onExport },//add by xiangwang0413w 2014/08/04 增加'匯出商品對照'
        dataDown,
        {
            text: SUPER_AUTH, colName: 'super_auth', iconCls: 'ui-icon ui-icon-repair', hidden: true,
            handler: function () {
                if (!Ext.getCmp('price_check').getValue())
                { p_store.each(function () { this.set('CanSel', 0); }); }
                else
                { s_store.each(function () { this.set('CanSel', 0); }); }
            }
        },
        '->',
        { text: ' ' }
    ];

    //把你要的column PUSH進去
    var proColumns = new Array();
    proColumns.push(c_pro_copy); //複製
    proColumns.push(c_pro_preview); //預覽
    proColumns = proColumns.concat(c_pro_base);
    proColumns.push(c_pro_type);
    proColumns.push(c_pro_pricetype); //價格類型 再商品列表中添加
    proColumns.push(c_pro_spec);
    proColumns.push(c_pro_pricelist);
    proColumns.push(c_pro_status);
    proColumns.push(c_pro_salestatus);//2015.02.05 販售狀態
    proColumns.push(c_pro_freight);
    proColumns.push(c_pro_mode);
    proColumns.push(c_pro_tax);
    proColumns.push(c_pro_sort);
    proColumns.push(c_pro_days);//add by dongya 2015/10/16
    proColumns.push(c_pro_date);//add by dongya 2015/10/22
    proColumns.push(c_pro_create);
    proColumns.push(c_pro_start);
    proColumns.push(c_pro_end);
    proColumns.push(c_pro_create_channel);
    //添加館別欄位  add  by zhuoqin0830w 2015/03/05
    proColumns.push(c_prod_classify);
    //添加是否失格  add  by zhuoqin0830w 2015/06/30
    proColumns.push(c_off_grade);

    var proGrid = Ext.create('Ext.grid.Panel', {
        hidden: true,
        id: 'proGrid',
        height: document.documentElement.clientHeight <= 700 ? document.documentElement.clientHeight - 240 : document.documentElement.clientHeight - 221,//edit by wwei0216w 2015/8/14 調整商品列表高度
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

    //價格列表的column add by hufeng0813w 2014/06/18
    var siteColumns = new Array();
    siteColumns.push(c_pro_copy);
    siteColumns.push(c_pro_linkcoding); //預覽
    siteColumns = siteColumns.concat(c_pro_base);
    siteColumns.push(c_pro_type);
    siteColumns.push(s_pro_spec);//規格 edit 2015/08/14
    siteColumns.push(c_pro_status);
    siteColumns.push(c_pro_site);
    siteColumns.push(c_pro_userlevel);
    siteColumns.push(c_pro_pricestatus);
    siteColumns.push(c_pro_itemoney);
    siteColumns.push(c_pro_itemeventmoney);
    siteColumns.push(c_pro_eventdate);
    //添加館別欄位  add  by zhuoqin0830w 2015/03/05
    siteColumns.push(c_prod_classify);
    //添加是否失格  add  by zhuoqin0830w 2015/06/30
    siteColumns.push(c_off_grade);
    siteColumns.push(c_pro_days);
    siteColumns.push(c_pro_date);//add by dongya 2015/10/22
    var siteProGrid = Ext.create('Ext.grid.Panel', {
        hidden: true,
        id: 'siteProGrid',
        store: s_store,
        height: document.documentElement.clientHeight <= 700 ? document.documentElement.clientHeight - 240 : document.documentElement.clientHeight - 221,//edit by wwei0216w 2015/8/14 調整商品列表高度
        columnLines: true,
        tbar: tools,
        selModel: site_sm,
        columns: siteColumns,
        bbar: Ext.create('Ext.PagingToolbar', {
            store: s_store,
            pageSize: pageSize,
            displayInfo: true
        }),
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
        }
    });

    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        border: false,
        items: [frm, proGrid, siteProGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                proGrid.height = document.documentElement.clientHeight <= 700 ? document.documentElement.clientHeight - 240 : document.documentElement.clientHeight - 221;//edit by wwei0216w 2015/8/14 調整商品列表高度
                siteProGrid.height = proGrid.height;
                this.doLayout();
            },
            afterrender: function () {
                ToolAuthorityQuery(function () {
                    //setShow(frm, 'colName');
                    //setTimeout(Search, 500);
                });
            }
        }
    });
});

function Search() {
    Ext.getCmp('key').setValue(Ext.getCmp('key').getValue().replace(/\s+/g, ','));
    if (!Ext.getCmp('price_check').getValue()) {
        Ext.getCmp('proGrid').show();
        Ext.getCmp('siteProGrid').hide();
        s_store.removeAll();
        p_store.loadPage(1);

    }
    else {
        Ext.getCmp('proGrid').hide();
        Ext.getCmp('siteProGrid').show();
        p_store.removeAll();
        s_store.loadPage(1);
    }
}

function addDateType() {
    if (DateStore.getCount() == 1) {
        DateStore.add(r_create, r_start, r_end);
    }
}

function storeLoad() {
    if (!Ext.getCmp('price_check').getValue()) {
        p_store.load();
    } else {
        s_store.load();
    }
}
//修改
onEditClick = function () {
    var rows;
    if (!Ext.getCmp('price_check').getValue()) {
        rows = Ext.getCmp('proGrid').getSelectionModel().getSelection();
    }
    else {
        rows = Ext.getCmp('siteProGrid').getSelectionModel().getSelection();
    }
    if (rows.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (rows.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (rows.length == 1) {
        var url;
        switch (rows[0].data.combination_id) {
            case '0':
            case '1':
                url = '/Product/ProductSave/' + rows[0].data.product_id;
                break;
            default:
                url = '/ProductCombo/index/' + rows[0].data.product_id;
                break;
        }
        //window.location.href = 'http://' + window.location.host + url;
        Ext.create('Ext.window.Window', {
            width: document.documentElement.clientWidth * 610 / 750,
            height: document.documentElement.clientHeight * 610 / 670,
            autoScroll: false,
            title: PRODUCT_UPDATE,
            iconCls: 'icon-edit',
            closeaction: 'destroy',
            resizable: true,
            draggable: true,
            modal: true,
            html: window.top.rtnFrame(url)
        }).show();
    }
}
//申請審核
onApplyClick = function () {
    var rows;
    if (!Ext.getCmp('price_check').getValue()) {
        rows = Ext.getCmp('proGrid').getSelectionModel().getSelection();
    }
    else {
        rows = Ext.getCmp('siteProGrid').getSelectionModel().getSelection();
    }
    if (rows.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        veryfiyConfirm(!Ext.getCmp('price_check').getValue() ? p_store : s_store, rows, 'apply');
    }
}
//上架
onUpClick = function () {
    var rows;
    if (!Ext.getCmp('price_check').getValue()) {
        rows = Ext.getCmp('proGrid').getSelectionModel().getSelection();
    }
    else {
        rows = Ext.getCmp('siteProGrid').getSelectionModel().getSelection();
    }
    if (rows.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        var pro_id = '';
        Ext.Array.each(rows, function (val) {
            if (pro_id.indexOf(val.data.product_id + '|') == -1)
                pro_id += val.data.product_id + '|';
        });
        // edit by zhuoqin0830w  2015/06/25  添加上架的備註欄位
        Ext.create('Ext.window.Window', {
            title: PRODUCT_UP_ANNOTATION,
            closeAction: 'destroy',
            id: 'onUpWin',
            layout: 'fit',
            modal: 'true',
            closable: true,
            resizable: false,
            draggable: false,
            bodyStyle: 'padding:5px 5px 5px 5px',
            items: [{
                xtype: 'form',
                id: 'onUpfrm',
                frame: true,
                plain: true,
                border: false,
                layout: 'anchor',
                defaults: { anchor: "95%", msgTarget: "side" },
                items: [{
                    xtype: "combo",
                    listConfig: { loadMask: false },
                    name: 'PutAway',
                    id: 'PutAway',
                    labelWidth: 40,
                    margin: '15 15 15 15',
                    store: PutAwayStore,
                    width: 200,
                    fieldLabel: REMARK,
                    displayField: 'parameterName',
                    valueField: 'ParameterCode',
                    triggerAction: 'all',
                    queryMode: 'local',
                    editable: true,
                    listeners: {
                        beforerender: function () {
                            PutAwayStore.load({
                                callback: function (records, operation, success) {
                                    Ext.getCmp("PutAway").setValue(records[0].data.parameterName);
                                }
                            });
                        }
                    }
                }]
            }],
            buttons: [{
                text: SURE,
                handler: function () {
                    var myMask = new Ext.LoadMask(Ext.getBody(), {
                        msg: 'Loading...'
                    });
                    myMask.show();
                    var remark = Ext.getCmp("PutAway").rawValue;
                    Ext.Ajax.request({
                        url: '/ProductList/ProductUp',
                        method: 'post',
                        params: { Product_Id: pro_id, "function": 'up', Remark: remark },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                Ext.getCmp("onUpWin").close();
                                storeLoad();
                            }
                            else {
                                if (result.msg == "") {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                } else {
                                    Ext.Msg.alert(INFORMATION, result.msg);
                                    Ext.getCmp("onUpWin").close();
                                    storeLoad();
                                }
                            }
                            myMask.hide();
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                            myMask.hide();
                        }
                    });
                }
            }]
        }).show();
    }
}

//下架不販售
onDonoCilck = function () {
    var rows;
    if (!Ext.getCmp('price_check').getValue()) {
        rows = Ext.getCmp('proGrid').getSelectionModel().getSelection();
    }
    else {
        rows = Ext.getCmp('siteProGrid').getSelectionModel().getSelection();
    }
    if (rows.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        var pro_id = '';
        Ext.Array.each(rows, function (val) {
            if (pro_id.indexOf(val.data.product_id + '|') == -1)
                pro_id += val.data.product_id + '|';
        });
        // edit by zhuoqin0830w  2015/06/25  添加下架不販售的備註欄位
        Ext.create('Ext.window.Window', {
            title: PRODUCT_DOWN_NON_TRAFFIC_ANNOTATION,
            closeAction: 'destroy',
            id: 'donoWin',
            layout: 'fit',
            modal: 'true',
            resizable: false,
            draggable: false,
            bodyStyle: 'padding:5px 5px 5px 5px',
            items: [{
                xtype: 'form',
                id: 'donofrm',
                frame: true,
                plain: true,
                border: false,
                layout: 'anchor',
                defaults: { anchor: "95%", msgTarget: "side" },
                items: [{
                    xtype: "combo",
                    listConfig: { loadMask: false },
                    name: 'UnShelveSell',
                    id: 'UnShelveSell',
                    margin: '15 15 15 15',
                    labelWidth: 40,
                    store: UnShelveSellStore,
                    width: 200,
                    fieldLabel: REMARK,
                    displayField: 'parameterName',
                    valueField: 'ParameterCode',
                    triggerAction: 'all',
                    queryMode: 'local',
                    editable: true,
                    listeners: {
                        beforerender: function () {
                            UnShelveSellStore.load({
                                callback: function (records, operation, success) {
                                    Ext.getCmp("UnShelveSell").setValue(records[0].data.parameterName);
                                }
                            });
                        }
                    }
                }]
            }],
            buttons: [{
                text: SURE,
                handler: function () {
                    var myMask = new Ext.LoadMask(Ext.getBody(), {
                        msg: 'Loading...'
                    });
                    myMask.show();
                    var remark = Ext.getCmp("UnShelveSell").rawValue;
                    Ext.Ajax.request({
                        url: '/ProductList/ProductDown?type=2',
                        method: 'post',
                        params: { Product_Id: pro_id, Remark: remark },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            Ext.Msg.alert(INFORMATION, MANIPULATE_SUCCEED);
                            if (result.success) {
                                Ext.getCmp('donoWin').close();
                                storeLoad();
                            }
                            myMask.hide();
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                            myMask.hide();
                        }
                    });
                }
            }]
        }).show();
    }
}

//下架
onDownClick = function () {
    var rows;
    if (!Ext.getCmp('price_check').getValue()) {
        rows = Ext.getCmp('proGrid').getSelectionModel().getSelection();
    }
    else {
        rows = Ext.getCmp('siteProGrid').getSelectionModel().getSelection();
    }
    if (rows.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        var pro_id = '';
        Ext.Array.each(rows, function (val) {
            if (pro_id.indexOf(val.data.product_id + '|') == -1)
                pro_id += val.data.product_id + '|';
        });
        Ext.create('Ext.window.Window', {
            title: PRODUCT_END + AND_ANNOTATION,
            closeAction: 'destroy',
            id: 'downwin',
            layout: 'fit',
            modal: 'true',
            resizable: false,
            draggable: false,
            width: 322,
            bodyStyle: 'padding:5px 5px 5px 5px',
            items: [{
                xtype: 'form',
                id: 'downfrm',
                frame: true,
                plain: true,
                border: false,
                defaultType: 'textfield',
                layout: 'anchor',
                url: '/ProductList/ProductDown',
                defaults: { anchor: "95%", msgTarget: "side" },
                items: [{
                    xtype: 'fieldcontainer',
                    layout: 'hbox',
                    id: 'downCombox',
                    items: [{
                        xtype: 'textfield',
                        hidden: true,
                        name: 'function',
                        value: 'down'
                    }, {
                        xtype: 'textfield',
                        hidden: true,
                        name: 'Product_Id',
                        value: pro_id
                    }, {
                        xtype: 'radiogroup',
                        columns: 1,
                        width: 80,
                        margin: '0 0 0 15',
                        items: [
                            { boxLabel: DOWN_NOW, name: 'rb', inputValue: '1', checked: true },
                            { boxLabel: DOWN_FUTURE, name: 'rb', inputValue: '2' }],
                        listeners: {
                            change: function (e, newValue, oldValue, eOpts) {
                                this.next().setDisabled(newValue.rb != '2');
                                this.next().setValue(newValue.rb != '2' ? '' : new Date());
                            }
                        }
                    }, {
                        xtype: 'datefield',
                        margin: '11px 5px 0 0',
                        disabled: true,
                        editable: false,
                        name: 'product_end'
                    }]
                }, { // add by zhuoqin0830w  2015/06/25  添加下架的備註
                    xtype: 'fieldcontainer',
                    layout: 'hbox',
                    id: 'downRadio',
                    items: [{
                        xtype: "combo",
                        margin: '-200px 0 0 16px',
                        listConfig: { loadMask: false },
                        name: 'UnShelve',
                        id: 'UnShelve',
                        labelWidth: 40,
                        store: UnShelveStore,
                        width: 228,
                        fieldLabel: REMARK,
                        displayField: 'parameterName',
                        valueField: 'parameterName',
                        triggerAction: 'all',
                        queryMode: 'local',
                        editable: true,
                        listeners: {
                            beforerender: function () {
                                UnShelveStore.load({
                                    callback: function (records, operation, success) {
                                        Ext.getCmp("UnShelve").setValue(records[0].data.parameterName);
                                    }
                                });
                            }
                        }
                    }]
                }]
            }],
            buttons: [{
                text: SURE,
                handler: function () {
                    var myMask = new Ext.LoadMask(Ext.getBody(), {
                        msg: 'Loading...'
                    });
                    myMask.show();
                    Ext.getCmp('downfrm').getForm().submit({
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            if (result.success) {
                                Ext.getCmp('downwin').close();
                                storeLoad();
                            }
                            myMask.hide();
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                            myMask.hide();
                        }
                    });
                }
            }]
        }).show();
    }
}

//刪除
onDeleteClick = function () {
    var rows;
    if (!Ext.getCmp('price_check').getValue()) {
        rows = Ext.getCmp('proGrid').getSelectionModel().getSelection();
    }
    else {
        rows = Ext.getCmp('siteProGrid').getSelectionModel().getSelection();
    }
    if (rows.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, rows.length), function (btn) {
            if (btn == 'yes') {
                var pro_ids = '';
                Ext.Array.each(rows, function (val) {
                    if (pro_ids.indexOf(val.data.product_id + '|') == -1)
                        pro_ids += val.data.product_id + '|';
                });
                Ext.Ajax.request({
                    url: '/ProductList/ProductDelete',
                    method: 'post',
                    params: { Product_Id: pro_ids },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            storeLoad();
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }
        });
    }
}

//取消申請
onCanCelClick = function () {
    var rows;
    if (!Ext.getCmp('price_check').getValue()) {
        rows = Ext.getCmp('proGrid').getSelectionModel().getSelection();
    }
    else {
        rows = Ext.getCmp('siteProGrid').getSelectionModel().getSelection();
    }
    if (rows.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(CANCEL_INFO, rows.length), function (btn) {
            if (btn == 'yes') {
                var pro_ids = '';
                //遍歷勾選了的product_id
                Ext.Array.each(rows, function (val) {
                    if (pro_ids.indexOf(val.data.product_id) == -1)
                        if (pro_ids == '') {
                            pro_ids += val.data.product_id;
                        }
                        else {
                            pro_ids += '|' + val.data.product_id;
                        }
                });
                Ext.Ajax.request({
                    url: '/ProductList/Product_Cancel',
                    method: 'post',
                    params: { Product_Id: pro_ids, "function": 'cancel' },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            storeLoad();
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }
        })
    }
}

cellEditSort = Ext.create('Ext.grid.plugin.CellEditing', {
    clicksToEdit: 1
});

//排序設定
onSortClick = function () {
    Ext.create('Ext.window.Window', {
        title: SORT_SET,
        id: 'windowSort',
        height: 450,
        width: 400,
        constrain: true,
        modal: true,
        layout: 'fit',
        items: {
            xtype: 'grid',
            id: 'gridSort',
            border: false,
            plugins: [cellEditSort],
            columns: [{ header: PRODUCT_ID, dataIndex: 'product_id' },
                      { header: PRODUCT_NAME, dataIndex: 'product_name' },
                      { header: PRODUCT_SORT, dataIndex: 'product_sort', editor: { xtype: 'numberfield', minValue: 0 } }],
            store: p_store
        },
        listeners: {
            destroy: function () {
                p_store.load();
            }
        },
        buttons: [{
            text: SAVE, handler: function () {
                //保存前先結束編輯狀態
                cellEditSort.completeEdit();

                var reg = /}{/g;
                var strResult = '[';
                for (var i = 0, j = p_store.getCount() ; i < j; i++) {
                    var c_data = p_store.getAt(i).data;
                    strResult += '{';
                    strResult += Ext.String.format('product_id:{0},product_sort:{1}', c_data.product_id, c_data.product_sort);
                    strResult += '}';

                }
                strResult += ']';
                strResult = strResult.replace(reg, '},{');


                Ext.Ajax.request({
                    url: '/ProductList/sortSet',
                    method: 'post',
                    params: { result: strResult },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, SUCCESS, function () { Ext.getCmp('windowSort').destroy() });
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }
        }
        ]
    }).show();
}

onExport = function () {
    var brand_id = Ext.getCmp('brand_id').getValue() ? Ext.getCmp('brand_id').getValue() : '';//品牌
    var comboProCate_hide = Ext.getCmp('comboProCate_hide').getValue() ? Ext.getCmp('comboProCate_hide').getValue() : '';//品類分類
    var comboFrontCage_hide = Ext.getCmp('comboFrontCage_hide').getValue() ? Ext.getCmp('comboFrontCage_hide').getValue() : '';//前臺分類
    var combination = Ext.getCmp('combination').getValue() ? Ext.getCmp('combination').getValue() : '';
    var product_status = Ext.getCmp('product_status').getValue() ? Ext.getCmp('product_status').getValue() : '';//商品狀態
    var product_freight_set = Ext.getCmp('product_freight_set').getValue() ? Ext.getCmp('product_freight_set').getValue() : '';//運送方式
    var product_mode = Ext.getCmp('product_mode').getValue() ? Ext.getCmp('product_mode').getValue() : '';//出貨方式
    var tax_type = Ext.getCmp('tax_type').getValue() ? Ext.getCmp('tax_type').getValue() : '';//營業稅
    var date_type = Ext.getCmp('date_type').getValue() ? Ext.getCmp('date_type').getValue() : '';
    var time_start = Ext.getCmp('time_start').getValue() ? Ext.getCmp('time_start').getValue() : '';
    var time_end = Ext.getCmp('time_end').getValue() ? Ext.getCmp('time_end').getValue() : '';
    var key = Ext.getCmp('key').getValue() ? Ext.htmlEncode(Ext.getCmp('key').getValue()) : '';
    //add by zhuoqin0830w 2015/07/22
    var off_grade = Ext.htmlEncode(Ext.getCmp("off_grade").getValue() ? 1 : 0); //失格商品查詢
    var purchase_in_advance = Ext.htmlEncode(Ext.getCmp('purchase_in_advance').getValue() ? 1 : 0);//添加預購商品 guodong1130w 2015/9/16添加

    if (brand_id == "" && comboProCate_hide == "" && comboFrontCage_hide == "" && combination == 0 && product_status == -1
        && product_freight_set == 0 && product_mode == 0 && tax_type == 0 & date_type == "" && time_start == "" && time_end == "" && key == "" && off_grade == 0 && purchase_in_advance == 0) {
        Ext.Msg.alert(INFORMATION, SEARCH_CONDITION);
        return;
    }

    var queryString = "brand_id=" + brand_id
        + "&comboProCate_hide=" + comboProCate_hide
        + "&comboFrontCage_hide=" + comboFrontCage_hide
        + "&combination=" + combination
        + "&product_status=" + product_status
        + "&product_freight_set=" + product_freight_set
        + "&product_mode=" + product_mode
        + "&tax_type=" + tax_type
        + "&date_type=" + date_type
        + "&time_start=" + time_start
        + "&time_end=" + time_end
        + "&key=" + key
        + "&off_grade=" + off_grade
        + "&purchase_in_advance=" + purchase_in_advance;

    window.open("ProductList/ExportProductItemMap?" + queryString);
}
//add 2014/10/21
function exportProduct(exprotFlag) {
    var brand_id = Ext.getCmp('brand_id').getValue() ? Ext.getCmp('brand_id').getValue() : '';//品牌
    var comboProCate_hide = Ext.getCmp('comboProCate_hide').getValue() ? Ext.getCmp('comboProCate_hide').getValue() : '';//品類分類
    var comboFrontCage_hide = Ext.getCmp('comboFrontCage_hide').getValue() ? Ext.getCmp('comboFrontCage_hide').getValue() : '';//前臺分類
    var combination = Ext.getCmp('combination').getValue() ? Ext.getCmp('combination').getValue() : '';
    var product_status = Ext.getCmp('product_status').getValue() ? Ext.getCmp('product_status').getValue() : '';//商品狀態
    var product_freight_set = Ext.getCmp('product_freight_set').getValue() ? Ext.getCmp('product_freight_set').getValue() : '';//運送方式
    var product_mode = Ext.getCmp('product_mode').getValue() ? Ext.getCmp('product_mode').getValue() : '';//出貨方式
    var tax_type = Ext.getCmp('tax_type').getValue() ? Ext.getCmp('tax_type').getValue() : '';//營業稅
    var date_type = Ext.getCmp('date_type').getValue() ? Ext.getCmp('date_type').getValue() : '';
    var time_start = Ext.getCmp('time_start').getValue() ? Ext.getCmp('time_start').rawValue : '';
    var time_end = Ext.getCmp('time_end').getValue() ? Ext.getCmp('time_end').rawValue : '';
    var key = Ext.getCmp('key').getValue() ? Ext.htmlEncode(Ext.getCmp('key').getValue()) : '';
    var stockstatus = Ext.getCmp('stockStatus') ? Ext.getCmp('stockStatus').getValue() : '';  //edti by wwei0216w 2014/12/27 添加庫存分類
    var purchase_in_advance = Ext.getCmp('purchase_in_advance').getValue() ? 1 : 0;//添加預購商品 guodong1130w 2015/9/16添加
    var price_check = false;
    switch (exprotFlag) {
        case 1:
        case 2:
            price_check = false;
            break;
        case 5:
            price_check = false;
            break;
        case 3:
            price_check = true;
        case 4:
            price_check = true;
    }

    //SITE = "站臺";
    var site_id = Ext.getCmp('site_id').getValue() ? Ext.getCmp('site_id').getValue() : '';

    //USER_LEVEL = "會員等級";
    var user_level = Ext.getCmp('user_level').getValue() ? Ext.getCmp('user_level').getValue() : '';

    //PRICE_STATUS = "價格狀態";
    var price_status = Ext.getCmp('price_status').getValue() ? Ext.getCmp('price_status').getValue() : '';

    //add by zhuoqin0830w 2015/07/22
    var off_grade = Ext.getCmp("off_grade").getValue() ? 1 : 0;  //失格商品查詢

    if (brand_id == "" && comboProCate_hide == "" && comboFrontCage_hide == "" && combination == 0 && product_status == -1
        && product_freight_set == 0 && product_mode == 0 && tax_type == 0 & date_type == "" && time_start == "" && time_end == "" && key == "" && off_grade == 0 && purchase_in_advance == 0) {
        Ext.Msg.alert(INFORMATION, SEARCH_CONDITION);
        return;
    }
    var cols = "";
    if (exprotFlag == 3) {
        var columns = Ext.getCmp("siteProGrid").columns;
        for (var i = 1, j = columns.length ; i < j; i++) {
            if (columns[i].hidden && columns[i].colName) {
                cols += columns[i].colName + ",";
            }
        }
        cols = cols.substr(0, cols.length - 1);
    }

    var queryString = Ext.Object.toQueryString({ //edit by wwei0216w 是用ext類型的傳遞字符串方式
        brand_id: brand_id,
        cate_id: comboProCate_hide,
        category_id: comboFrontCage_hide,
        combination: combination,
        product_status: product_status,
        freight: product_freight_set,
        mode: product_mode,
        tax_type: tax_type,
        date_type: date_type,
        time_start: time_start,
        time_end: time_end,
        name_number: key,
        price_check: price_check,
        site_id: site_id,
        user_level: user_level,
        price_status: price_status,
        exportFlag: exprotFlag,
        StockStatus: stockstatus,
        cols: cols,
        off_grade: off_grade,   //add by zhuoqin0830w 2015/07/22
        purchase_in_advance: purchase_in_advance //add by guodong1130w 2015/9/16
    });


    //var queryString = "brand_id=" + brand_id
    //    + "&cate_id=" + comboProCate_hide
    //    + "&category_id=" + comboFrontCage_hide
    //    + "&combination=" + combination
    //    + "&product_status=" + product_status
    //    + "&freight=" + product_freight_set
    //    + "&mode=" + product_mode
    //    + "&tax_type=" + tax_type
    //    + "&date_type=" + date_type
    //    + "&time_start=" + time_start
    //    + "&time_end=" + time_end
    //    + "&name_number=" + key
    //    + "&price_check=" + price_check
    //    + "&site_id=" + site_id
    //    + "&user_level=" + user_level
    //    + "&site_id=" + site_id
    //    + "&exportFlag=" + exprotFlag
    //    + "&StockStatus=" + stockstatus
    //    + "&cols=" + cols;
    window.open("ProductList/ExportProduct?" + queryString);
}