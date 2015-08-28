
var pageSize = 15;


var site = new Array();
site = site.concat(m_pro_base);
site.push(m_pro_type);
site.push(m_pro_type_id);
site.push(m_pro_spec);
site.push(m_pro_status);
site.push(m_pro_status_id);
site.push(m_pro_freight);
site.push(m_pro_site);
site.push(m_pro_site_id); //add by hufeng0813w 2014/05/22 站臺ID
site.push(m_pro_level); //add by hufeng0813w 2014/05/22 會員等級ID
site.push(m_pro_master_user_id); //add by hufeng0813w 2014/05/22 會員ID
//site.push(m_pro_userlevel); //會員等級
site.push(m_pro_pricestatus);
site.push(m_pro_mode);  //出貨方式
site.push(m_pro_pricelist); //建議售價
site.push(m_pro_cost);  //成本
site.push(m_pro_event_cost);    //活動成本
site.push(m_pro_itemmoney);
site.push(m_pro_itemeventmoney);
site.push(m_pro_eventstart);
site.push(m_pro_eventend);
site.push(m_CanSel);
site.push(m_CanDo);
site.push(m_temp_status);
site.push(m_create_channel);

Ext.define('GIGADE.SITEPRODUCT', {
    extend: 'Ext.data.Model',
    fields: site
});
var s_store = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.SITEPRODUCT',
    proxy: {
        type: 'ajax',
        url: '/VendorProductList/QueryVendorProductList',
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
        brand_id: Ext.getCmp('brand_id') ? Ext.getCmp('brand_id').getValue() : '',  //品牌
        combination: Ext.getCmp('combination') ? Ext.getCmp('combination').getValue() : '', //商品類型(固定組合/任選組合/群組搭配/單一商品)
        product_status: Ext.getCmp('product_status') ? Ext.getCmp('product_status').getValue() : '',    //商品狀態
        product_freight_set: Ext.getCmp('product_freight_set') ? Ext.getCmp('product_freight_set').getValue() : '', //運送方式
        product_mode: Ext.getCmp('product_mode') ? Ext.getCmp('product_mode').getValue() : '',  //出貨方式
        date_type: Ext.getCmp('date_type') ? Ext.getCmp('date_type').getValue() : '',   //日期條件類型
        time_start: Ext.getCmp('time_start') ? Ext.getCmp('time_start').getValue() : '',
        time_end: Ext.getCmp('time_end') ? Ext.getCmp('time_end').getValue() : '',
        key: Ext.getCmp('key') ? Ext.htmlEncode(Ext.getCmp('key').getValue()) : ''  //名稱/編號搜尋
    });
});
var site_sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            var tls = Ext.getCmp("siteProGrid").query('*[ttype=tbar]');
            var rows = Ext.getCmp('siteProGrid').getSelectionModel().getSelection();
            if (selections.length > 0) {
                Ext.Array.each(tls, function () {
                    var IsDisabled = true;
                    switch (this.text) {
                        //0:新建立商品,1:申請審核,2:審核通過,5:上架,6:下架,20:供應商新建商品 
                        case PRODUCT_UP:
                            Ext.Array.each(selections, function (rows) {
                                if (rows.data.product_status_id != 5 && rows.data.temp_status == "0") {
                                    IsDisabled = false;
                                }
                            }); //審核通過 可以上架 || val.isModified("CanSel")
                            break;
                        case PRODUCT_DOWN:
                            Ext.Array.each(selections, function (rows) {
                                if (rows.data.product_status_id == 5) {
                                    IsDisabled = false;
                                }
                            }); //上架商品  可以下架 || val.isModified("CanSel")
                            break;
                        case PRODUCT_UPDATE:
                            Ext.Array.each(selections, function (rows) {
                                if (rows.data.temp_status == "12" && rows.data.product_status_id != 1) {
                                    IsDisabled = false;
                                }
                            }); //新建商品 商品修改
                            break;
                        case PRODUCT_APPLY:
                            Ext.Array.each(selections, function (rows) {
                                if (rows.data.temp_status == "12" && rows.data.product_status_id == 20) {
                                    IsDisabled = false;
                                }
                            }); //新建商品 下架商品  可以申請審核 || val.isModified("CanSel")
                            break;
                        case DELETE:
                            Ext.Array.each(selections, function (rows) {
                                if (rows.data.temp_status == "12") {
                                    IsDisabled = false;
                                }
                            }); //新建商品 可以刪除
                            break;
                        case CANCEL_APPLY:
                            Ext.Array.each(selections, function (rows) {
                                if (rows.data.temp_status == "12" && rows.data.product_status_id == 1) {
                                    IsDisabled = false;
                                }
                            }); //新建商品 下架商品  可以取消申請審核 || val.isModified("CanSel") 
                            break;
                        default:
                            IsDisabled = true;
                            break;
                    };
                    this.setDisabled(IsDisabled);
                });
            } else {
                Ext.Array.each(tls, function () { this.setDisabled(true); });
            }
        }
    }
    //storeColNameForHide: 'CanSel'
});

Ext.onReady(function () {
    document.body.onkeydown = function () {
        if (event.keyCode == 13) {
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
        items: [
            {
                xtype: 'panel',
                columnWidth: .70,
                border: 0,
                layout: 'anchor',
                items: [
                    brand,
                    {
                        //category,
                        xtype: 'fieldcontainer',
                        layout: 'hbox',
                        anchor: '100%',
                        items: [type, pro_status]
                    },
                    freight_mode_tax, //營業稅
                    start_end,
                    key_query
                ]
            }
        ],
        buttonAlign: 'center',
        buttons: [{
            text: SEARCH,
            id: 'btn_search',
            handler: Search
        }, {
            text: RESET,
            id: 'btn_reset',
            listeners: {
                click: function () {
                    frm.getForm().reset();
                    Ext.getCmp("combination").setValue(ComboStore.data.items[0].data.parameterCode);
                    Ext.getCmp("product_status").setValue(prodStatusStore.data.items[0].data.parameterCode);
                    Ext.getCmp("date_type").setValue(DateStore.data.items[0].data.code);
                    Ext.getCmp("product_freight_set").setValue(freightStore.getAt(0).data.parameterCode);
                    Ext.getCmp("product_mode").setValue(modeStore.getAt(0).data.parameterCode);
                }
            }
        }]
    });

    var tools = [
        { text: PRODUCT_UP, colName: 'up', hidden: false, ttype: 'tbar', disabled: true, handler: onUpClick },
        { text: PRODUCT_DOWN, colName: 'down', hidden: false, ttype: 'tbar', disabled: true, handler: onDownClick },
        { text: PRODUCT_UPDATE, colName: 'update', hidden: false, ttype: 'tbar', iconCls: 'icon-edit', disabled: true, handler: onEditClick },
        { text: PRODUCT_APPLY, colName: 'apply', hidden: false, ttype: 'tbar', disabled: true, handler: onApplyClick },
        { text: DELETE, colName: 'delete', hidden: false, ttype: 'tbar', iconCls: 'icon-remove', disabled: true, handler: onDeleteClick },
        { text: CANCEL_APPLY, colName: 'cancel', hidden: false, ttype: 'tbar', disabled: true, handler: onCanCelClick },
        //{ text: SORT_SET, colName: 'sort', hidden: true, ttype: '', handler: onSortClick },
        //{ text: OUT_PRODUCT, colName: 'export', hidden: true, handler: onExport }, //add by xiangwang0413w 2014/08/04 增加'匯出商品對照'
        //{
        //    text: SUPER_AUTH, colName: 'super_auth', iconCls: 'icon-user-auth', hidden: false,
        //            handler: function () {
        //    s_store.each(function () { this.set('CanSel', 1); });
        //}
        //},
        '->',
        { text: ' ' }
    ];
    //價格列表的column add by hufeng0813w 2014/06/18
    var siteColumns = new Array();
    siteColumns.push(c_pro_copy); //複製
    siteColumns.push(c_pro_preview); //預覽
    //siteColumns.push(c_pro_linkcoding); //連接
    siteColumns = siteColumns.concat(c_pro_base);
    siteColumns.push(c_pro_type);
    siteColumns.push(c_pro_spec);
    siteColumns.push(c_pro_status);
    //siteColumns.push(c_pro_site);
    //siteColumns.push(c_pro_userlevel);
    //siteColumns.push(c_pro_pricestatus);
    siteColumns.push(c_pro_freight); //運送方式
    siteColumns.push(c_pro_mode); //出貨方式
    siteColumns.push(c_pro_pricelist); //建議售價
    siteColumns.push(c_pro_itemoney); //售價
    siteColumns.push(c_pro_cost); //成本
    siteColumns.push(c_pro_itemeventmoney); //活動價
    siteColumns.push(c_pro_eventcost); //活動成本
    siteColumns.push(c_pro_eventdate); //活動期間
    siteColumns.push(c_temp_status);    //臨時表中狀態
    siteColumns.push(c_create_channel);
    var siteProGrid = Ext.create('Ext.grid.Panel', {
        hidden: false,
        id: 'siteProGrid',
        store: s_store,
        //height: document.documentElement.clientHeight <= 700 ? document.documentElement.clientHeight - 242 : document.documentElement.clientHeight - 222,
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
        items: [frm, siteProGrid], //[frm, proGrid, siteProGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                siteProGrid.height = document.documentElement.clientHeight <= 700 ? document.documentElement.clientHeight - 190 : document.documentElement.clientHeight - 190;
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
    Ext.getCmp('siteProGrid').show();
    s_store.loadPage(1);
}
function addDateType() {
    if (DateStore.getCount() == 1) {
        DateStore.add(r_create, r_start, r_end);
    }
}
function storeLoad() {
    s_store.load();
}
onEditClick = function () {
    var rows;
    rows = Ext.getCmp('siteProGrid').getSelectionModel().getSelection();
    if (rows.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (rows.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (rows.length == 1) {
        var url;
        switch (rows[0].data.combination_id) {
            case '0':
            case '1':
                url = '/VendorProduct/ProductSave/' + rows[0].data.product_id;
                break;
            default:
                url = '/VendorProductCombo/index/' + rows[0].data.product_id;
                break;
        }
        //window.location.href = 'http://' + window.location.host + url;
        Ext.create('Ext.window.Window', {
            width: 1000,
            height: document.documentElement.clientHeight * 610 / 783,
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

onApplyClick = function () {
    var rows;
    rows = Ext.getCmp('siteProGrid').getSelectionModel().getSelection();
    if (rows.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        veryfiyConfirm(s_store, rows, 'apply');
        s_store.load();
    }
}
onUpClick = function () {
    var rows;
    rows = Ext.getCmp('siteProGrid').getSelectionModel().getSelection();
    if (rows.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        var pro_id = '';
        Ext.Array.each(rows, function (val) {
            if (pro_id.indexOf(val.data.product_id + '|') == -1)
                pro_id += val.data.product_id + '|';
        });

        Ext.Ajax.request({
            url: '/VendorProductList/ProductUp',
            method: 'post',
            params: { Product_Id: pro_id, "function": 'up' },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    Ext.Msg.alert(INFORMATION, SUCCESS);
                    storeLoad();
                } else {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                }
            },
            failure: function () {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        });
    }
}
onDownClick = function () {
    var rows;
    rows = Ext.getCmp('siteProGrid').getSelectionModel().getSelection();
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
            title: PRODUCT_END,
            closeAction: 'destroy',
            id: 'downwin',
            layout: 'fit',
            modal: 'true',
            resizable: false,
            draggable: false,
            bodyStyle: 'padding:5px 5px 5px 5px',
            items: [{
                xtype: 'form',
                id: 'downfrm',
                frame: true,
                plain: true,
                border: false,
                defaultType: 'textfield',
                layout: 'anchor',
                url: '/VendorProductList/ProductDown',
                defaults: { anchor: "95%", msgTarget: "side" },
                items: [
                {
                    xtype: 'textfield',
                    hidden: true,
                    name: 'function',
                    value: 'down'
                }, 
                {
                    xtype: 'textfield',
                    hidden: true,
                    name: 'Product_Id',
                    value: pro_id
                }, 
                {
                    xtype: 'radiogroup',
                    columns: 1,
                    items: [
                        { boxLabel: DOWN_NOW, name: 'rb', inputValue: '1', checked: true },
                        { boxLabel: DOWN_FUTURE, name: 'rb', inputValue: '2' }
                    ],
                    listeners: {
                        change: function (e, newValue, oldValue, eOpts) {
                            this.next().setDisabled(newValue.rb != '2');
                            this.next().setValue(newValue.rb != '2' ? '' : new Date());
                        }
                    }
                }, 
                {
                    xtype: 'datefield',
                    margin: '0 30px',
                    disabled: true,
                    editable: false,
                    name: 'product_end'
                }]
            }],
            buttons: [
            {
                text: SURE,
                handler: function () {
                    Ext.getCmp('downfrm').getForm().submit({
                        params: {
                            explain: '下架時間為：',
                            productid:pro_id
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);

                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                Ext.getCmp('downwin').close();
                                storeLoad();
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    });
                }
            }]            
        }).show();
    }
}
onDeleteClick = function () {
    var rows;
    rows = Ext.getCmp('siteProGrid').getSelectionModel().getSelection();
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
                // alert(pro_ids);
                Ext.Ajax.request({
                    url: '/VendorProductList/VendorProductDelete',
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
//取消送審
onCanCelClick = function () {
    var rows;
    rows = Ext.getCmp('siteProGrid').getSelectionModel().getSelection();
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
                        } else {
                            pro_ids += '|' + val.data.product_id;
                        }
                });
                Ext.Ajax.request({
                    url: '/VendorProductList/Product_Cancel',
                    method: 'post',
                    params: { Product_Id: pro_ids, "function": 'cancel' },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            s_store.load();
                        } else {
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

////排序設定
//onSortClick = function () {
//    Ext.create('Ext.window.Window', {
//        title: SORT_SET,
//        id: 'windowSort',
//        height: 450,
//        width: 400,
//        constrain: true,
//        modal: true,
//        layout: 'fit',
//        items: {
//            xtype: 'grid',
//            id: 'gridSort',
//            border: false,
//            plugins: [cellEditSort],
//            columns: [{ header: PRODUCT_ID, dataIndex: 'product_id' },
//                      { header: PRODUCT_NAME, dataIndex: 'product_name' },
//                      { header: PRODUCT_SORT, dataIndex: 'product_sort', editor: { xtype: 'numberfield', minValue: 0}}],
//            store: s_store// p_store
//        },
//        listeners: {
//            destroy: function () {
//                s_store.load(); //  p_store.load();
//            }
//        },
//        buttons: [{
//            text: SAVE, handler: function () {
//                //保存前先結束編輯狀態
//                cellEditSort.completeEdit();

//                var reg = /}{/g;
//                var strResult = '[';
//                for (var i = 0, j = s_store.getCount(); i < j; i++) {// p_store.getCount(); i < j; i++) {
//                    var c_data = s_store.getAt(i).data; // p_store.getAt(i).data;
//                    strResult += '{';
//                    strResult += Ext.String.format('product_id:{0},product_sort:{1}', c_data.product_id, c_data.product_sort);
//                    strResult += '}';

//                }
//                strResult += ']';
//                strResult = strResult.replace(reg, '},{');


//                Ext.Ajax.request({
//                    url: '/VendorProductList/sortSet',
//                    method: 'post',
//                    params: { result: strResult },
//                    success: function (form, action) {
//                        var result = Ext.decode(form.responseText);
//                        Ext.Msg.alert(INFORMATION, SUCCESS, function () { Ext.getCmp('windowSort').destroy() });
//                    },
//                    failure: function () {
//                        Ext.Msg.alert(INFORMATION, FAILURE);
//                    }
//                });
//            }
//        }
//        ]
//    }).show();


//}

////商品導出
//onExport = function () {
//    var brand_id = Ext.getCmp('brand_id').getValue() ? Ext.getCmp('brand_id').getValue() : '';
//    var combination = Ext.getCmp('combination').getValue() ? Ext.getCmp('combination').getValue() : '';
//    var product_status = Ext.getCmp('product_status').getValue() ? Ext.getCmp('product_status').getValue() : '';
//    var product_freight_set = Ext.getCmp('product_freight_set').getValue() ? Ext.getCmp('product_freight_set').getValue() : '';
//    var product_mode = Ext.getCmp('product_mode').getValue() ? Ext.getCmp('product_mode').getValue() : '';
//    var date_type = Ext.getCmp('date_type').getValue() ? Ext.getCmp('date_type').getValue() : '';
//    var time_start = Ext.getCmp('time_start').getValue() ? Ext.getCmp('time_start').getValue() : '';
//    var time_end = Ext.getCmp('time_end').getValue() ? Ext.getCmp('time_end').getValue() : '';
//    var key = Ext.getCmp('key').getValue() ? Ext.htmlEncode(Ext.getCmp('key').getValue()) : '';

//    var queryString = "brand_id=" + brand_id
//        + "&combination=" + combination
//        + "&product_status=" + product_status
//        + "&product_freight_set=" + product_freight_set
//        + "&product_mode=" + product_mode
//        + "&date_type=" + date_type
//        + "&time_start=" + time_start
//        + "&time_end=" + time_end
//        + "&key=" + key;

//    window.open("VendorProductList/ExportProductItemMap?" + queryString);
//}
