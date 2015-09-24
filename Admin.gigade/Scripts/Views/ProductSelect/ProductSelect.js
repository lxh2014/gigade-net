
var pageSize = 10;

//站台價格列
var site = new Array();
site = site.concat(m_pro_base);
site.push(m_pro_pricemasterid);
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
        product_status: Ext.getCmp('product_status') ? Ext.getCmp('product_status').getValue() : '',
        product_freight_set: Ext.getCmp('product_freight_set') ? Ext.getCmp('product_freight_set').getValue() : '',
        product_mode: Ext.getCmp('product_mode') ? Ext.getCmp('product_mode').getValue() : '',
        tax_type: Ext.getCmp('tax_type') ? Ext.getCmp('tax_type').getValue() : '',
        date_type: Ext.getCmp('date_type') ? Ext.getCmp('date_type').getValue() : '',
        time_start: Ext.getCmp('time_start') ? Ext.getCmp('time_start').getValue() : '',
        time_end: Ext.getCmp('time_end') ? Ext.getCmp('time_end').getValue() : '',
        key: Ext.getCmp('key') ? Ext.htmlEncode(Ext.getCmp('key').getValue()) : '',
        price_check: 'true', //Ext.getCmp('price_check') ? Ext.getCmp('price_check').getValue() : '',
        site_id: Ext.getCmp('site_id') ? Ext.getCmp('site_id').getValue() : '',
        user_level: Ext.getCmp('user_level') ? Ext.getCmp('user_level').getValue() : '',
        price_status: Ext.getCmp('price_status') ? Ext.getCmp('price_status').getValue() : '',
        //添加  選擇的值  排除 不需要的數據  add by zhuoqin0830w  2015/02/10
        priceCondition: Ext.getCmp("chock").getValue()["rb"],
        //add by zhuoqin0830w  2015/03/11  已買斷商品的篩選功能
        productPrepaid: Ext.getCmp('buyPro') ? Ext.getCmp('buyPro').getValue() : ''
    });
});

var sm = Ext.create('Ext.selection.CheckboxModel');


var site_sm = Ext.create('Ext.selection.CheckboxModel');

//添加選擇按鈕  add by zhuoqin0830w 2015/02/10
var radioCondition = {
    xtype: 'radiogroup',
    id: 'chock',
    name: 'chock',
    colName: 'chock',
    columns: 6,
    checkChange: function () {
        Ext.getCmp('tools').setDisabled(true);
    },
    items: [{ boxLabel: SINGLE_PRICE_AS_DISCOUNT_EDIT, name: 'rb', inputValue: '1', checked: true },//活動價依折數修改
            { boxLabel: SINGLE_PRICE_AS_DISCOUNT_INPUT, name: 'rb', inputValue: '2' }]//活動價各別輸入
}

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
            items: [brand, category, {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                anchor: '100%',
                items: [type, pro_status]
            }, freight_mode_tax, start_end, key_query,
            {//添加選擇按鈕  add by zhuoqin0830w 2015/02/10
                xtype: 'panel',
                border: 0,
                layout: 'anchor',
                items: [radioCondition]
            }]
        }, {
            xtype: 'panel',
            columnWidth: .30,
            layout: 'anchor',
            border: 0,
            items: [sitebox, user_level, price_status]
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
                    Ext.getCmp("site_id").setValue(siteStore.data.items[0].data.Site_Id);
                    Ext.getCmp("user_level").setValue(userlevelStore.data.items[0].data.parameterCode);
                    Ext.getCmp("combination").setValue(ComboStore.data.items[0].data.parameterCode);
                    Ext.getCmp("product_status").setValue(prodStatusStore.data.items[0].data.parameterCode);
                    Ext.getCmp("date_type").setValue(DateStore.data.items[0].data.code);
                    Ext.getCmp("price_status").setValue(priceStatusStore.getAt(0).data.parameterCode);
                    Ext.getCmp("product_freight_set").setValue(freightStore.getAt(0).data.parameterCode);
                    Ext.getCmp("product_mode").setValue(modeStore.getAt(0).data.parameterCode);
                    Ext.getCmp("tax_type").setValue(taxStore.getAt(0).data.value);
                }
            }
        }]
    });

    var tools = [
        //添加 ID 和 Name add by zhuoqin0830w  2015/02/10 //批量修改活動價
        { id: "tools", name: "tools", text: QUANTITY_EDIT_SINGLE_PRICE, /*colName: 'update', hidden: true, */ttype: 'tbar', iconCls: 'icon-edit', handler: onEditClick }//,
    ];

    //價格列表的column 
    var siteColumns = new Array();
    siteColumns = siteColumns.concat(c_pro_base);
    siteColumns.push(c_pro_type);
    siteColumns.push(c_pro_spec);
    siteColumns.push(c_pro_status);
    siteColumns.push(c_pro_site);
    siteColumns.push(c_pro_userlevel);
    siteColumns.push(c_pro_pricestatus);
    siteColumns.push(c_pro_itemoney);
    siteColumns.push(c_pro_itemeventmoney);
    siteColumns.push(c_pro_eventdate);
    var siteProGrid = Ext.create('Ext.grid.Panel', {
        hidden: true,
        id: 'siteProGrid',
        store: s_store,
        height: document.documentElement.clientHeight <= 700 ? document.documentElement.clientHeight - 242 : document.documentElement.clientHeight - 250,//edit by wwei0216w 將 document.documentElement.clientHeight - 222改為document.documentElement.clientHeight - 250
        //height: 556,
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
        items: [frm, siteProGrid],
        renderTo: Ext.getBody(),
        autoScroll: false,
        listeners: {
            resize: function () {
                siteProGrid.height = document.documentElement.clientHeight <= 700 ? document.documentElement.clientHeight - 242 : document.documentElement.clientHeight - 250;
                //siteProGrid.height = 556;
                this.doLayout();
            },
            afterrender: function () {
                ToolAuthorityQuery(function () {
                    //setShow(frm, 'colName');
                    //setTimeout(Search, 500);              //2015/05/25
                });
            }
        }
    });
});

function Search() {
    Ext.getCmp('key').setValue(Ext.getCmp('key').getValue().replace(/\s+/g, ','));
    Ext.getCmp('siteProGrid').show();
    Ext.getCmp('tools').setDisabled(false);//添加選擇按鈕  add by zhuoqin0830w 2015/02/10
    //edit by zhuoqin0830w 2015/01/21 判斷站臺是否被選中
    var form = Ext.getCmp('frm').getForm();
    if (form.isValid()) {
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

onEditClick = function () {
    var rows = Ext.getCmp('siteProGrid').getSelectionModel().getSelection();
    var priceMasterIds = '';
    Ext.each(rows, function (row) {
        priceMasterIds += row.data["price_master_id"] + ',';
    });
    priceMasterIds = priceMasterIds.substring(0, priceMasterIds.length - 1);

    // 添加 priceCondition 判斷選中的 按鈕  add by zhuoqin0830w 2015/02/10
    var url = "/ProductSelect/ModifyEventCost?priceMasterIds=" + priceMasterIds + "&?priceCondition=" + Ext.getCmp("chock").getValue()["rb"];
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var event = panel.down('#event');
    if (event) {
        event.close();
    }
    event = panel.add({
        id: 'event',
        title: REPEATEDLY_EDIT_SINGLE_PRICE,//批次修改活動價
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(event);
    panel.doLayout();
};

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

cellEditSort = Ext.create('Ext.grid.plugin.CellEditing', {
    clicksToEdit: 1
});