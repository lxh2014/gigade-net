

var pageSize = 25;

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
                        ComboStore.insert(0, { parameterCode: '0', parameterName: VALUE_ALL });
                        Ext.getCmp("combination").setValue(ComboStore.data.items[0].data.parameterCode);
                    }
                });
            }
        }
    }, {
        xtype: 'combobox',
        id: 'prev_status',
        margin: '0 20px',
        fieldLabel: APPLY_TYPE,
        colName: 'prev_status',
        queryMode: 'local',
        editable: false,
        store: ApplyTypeStore,
        displayField: 'name',
        valueField: 'code',
        value: '-1'
    }]
});


var columns = c_pro_base_byproduct;
columns.push(c_pro_kuser);          //建立人
columns.push(c_pro_type);           //商品類型
columns.push(c_pro_pricelist);      //建議售價
columns.push(c_pro_applytype);      //申請類型
columns.push(c_pro_status);         //商品狀態
columns.push(c_pro_freight);        //運送方式
columns.push(c_pro_mode);           //出貨方式
columns.push(c_pro_tax);            //營業稅
columns.push(c_pro_askfordate);     //申請時間 
columns.push(c_pro_start);          //售價
columns.push(c_pro_end);            //活動價
columns.push(c_pro_up_now);          //審核后即刻上架


//待審核商品MODEL
Ext.define('GIGADE.WAITVERIFY', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'product_image', type: 'string' },
        { name: 'product_id', type: 'int' },
        { name: 'brand_name', type: 'string' },
        { name: 'product_name', type: 'string' },
        { name: 'prod_sz', type: 'string' },
        { name: 'user_name', type: 'string' },
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

// fields: ["product_image", "product_id", "brand_name",
//             "product_name", "user_name", "combination",
//             "product_price_list", "prev_status", "product_status",
//             "product_freight_set", "product_mode", "tax_type",
//             "apply_time", "product_start", "product_end"],

var waitVerifyStore = Ext.create("Ext.data.Store", {
    model: 'GIGADE.WAITVERIFY',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/ProductList/waitVerifyQuery',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

waitVerifyStore.on("beforeload", function () {
    Ext.apply(waitVerifyStore.proxy.extraParams,
             {
                 brand_id: Ext.getCmp("brand_id").getValue(),
                 cate_id: Ext.getCmp('comboProCate_hide').getValue(),
                 category_id: Ext.getCmp('comboFrontCage_hide').getValue(),
                 combination: Ext.getCmp("combination").getValue(),
                 prev_status: Ext.getCmp('prev_status').getValue(),    //申請類型
                 date_type: Ext.getCmp("date_type").getValue(),
                 time_start: Ext.getCmp("time_start").getValue(),
                 time_end: Ext.getCmp("time_end").getValue(),
                 key: Ext.htmlEncode(Ext.getCmp("key").getValue())
             });
});


var back = function (row, reason, functionid) {
    var result = '';
    for (var i = 0, j = row.length; i < j; i++) {
        if (i > 0) {
            result += ',';
        }
        result += row[i].data.product_id;
    }

    Ext.Ajax.request({
        url: '/ProductList/vaiteVerifyBack',
        params: {
            productIds: result,
            backReason: reason,
            "function": functionid
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

    var myMask = new Ext.LoadMask(Ext.getBody(), {
        msg: 'Loading...'
    });
    myMask.show();

    var rows = Ext.getCmp("waitVerifyGrid").getSelectionModel().getSelection();
    if (rows.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
        return false;
        myMask.hide();
    }
    var result = '';
    for (var i = 0, j = rows.length; i < j; i++) {
        if (i > 0) {
            result += ',';
        }
        result += rows[i].data.product_id;
    }

    Ext.Ajax.request({
        url: '/ProductList/vaiteVerifyPass',
        timeout: 1000 * 60 * 2,
        params: {
            prodcutIdStr: result,
            "function": 'btnPass'
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
            myMask.hide();
        },
        failure: function (response, opts) {
            myMask.hide();
            Ext.Msg.alert(NOTICE, OVERTIME_SAVE);
        }
    });
}

//駁回
var btnBackOnClick = function () {
    var row = Ext.getCmp("waitVerifyGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
        return false;
    }

    Ext.MessageBox.show({
        title: BACK_REASON,
        id: 'txtReason',
        msg: INPUT_BACK_REASON,
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

var query = function () {
    Ext.getCmp('key').setValue(Ext.getCmp('key').getValue().replace(/\s+/g, ','));
    waitVerifyStore.loadPage(1);
}


//添加日期類型
function addDateType() {
    if (DateStore.getCount() == 1) {
        DateStore.add(r_apply, r_start, r_end);
    }
}

Ext.onReady(function () {
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
            items: [brand, category, type_apply, start_end, key_query]
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
                    Ext.getCmp("prev_status").setValue(ApplyTypeStore.data.items[0].data.code);
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
                Ext.getCmp("waitVerifyGrid").down('#btnPass').setDisabled(selections.length == 0);
                Ext.getCmp("waitVerifyGrid").down('#btnBack').setDisabled(selections.length == 0);
                //Ext.getCmp("waitVerifyGrid").down('#last_modify').setDisabled(selections.length == 0);
            }
        }
    });

    function searchShow() {
        Ext.getCmp('waitVerifyGrid').show();
        query();
    }

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
            text: VERIFY_PASS,
            id: 'btnPass',
            colName: 'btnPass',
            hidden: true,
            disabled: true,
            iconCls: 'icon-accept',
            handler: btnPassOnClick

        }, {
            text: VERIFY_BACK,
            colName: 'btnBack',
            disabled: true,
            hidden: true,
            id: 'btnBack',
            icon: '../../../Content/img/icons/drop-no.gif',
            handler: btnBackOnClick
        }
        //, {
        //    text: LAST_MODIFY,
        //    colName: 'last_modify',
        //    id: 'last_modify',
        //    disabled: true,
        //    hidden: true,
        //    iconCls: 'icon-edit',
        //    handler: function () {
        //        var rows = Ext.getCmp('waitVerifyGrid').getSelectionModel().getSelection();
        //        if (rows.length == 0) {
        //            Ext.Msg.alert(INFORMATION, NO_SELECTION);
        //        } else if (rows.length > 1) {
        //            Ext.Msg.alert(INFORMATION, ONE_SELECTION);
        //        } else if (rows.length == 1) {
        //            Ext.create('Ext.window.Window', {
        //                title: '上次修改記錄',
        //                width: 1000,
        //                height: document.documentElement.clientHeight * 610 / 783,
        //                autoScroll: true,
        //                bodyStyle: 'background:#ffffff; padding:5px;',
        //                closeaction: 'destroy',
        //                resizable: false,
        //                draggable: false,
        //                modal: true,
        //                listeners: {
        //                    show: function (e, eOpts) {
        //                        Ext.Ajax.request({
        //                            url: '/ProductList/QueryLastModifyRecord',
        //                            method: 'post',
        //                            params: { Product_Id: rows[0].data.product_id, Type: 'product' },
        //                            success: function (form, action) {
        //                                var result = Ext.decode(form.responseText);
        //                                if (result.success) {
        //                                    if (result.html) {
        //                                        e.update(Ext.htmlDecode(result.html));
        //                                    } else {
        //                                        e.destroy();
        //                                        Ext.Msg.alert(INFORMATION, SEARCH_NO_DATA);
        //                                    }
        //                                }
        //                            },
        //                            failure: function () {
        //                                Ext.Msg.alert(INFORMATION, FAILURE);
        //                            }
        //                        });
        //                    }
        //                }
        //            }).show();
        //        }
        //    }
        //}eidt by wangwei0216w 2014/8/12 說明:將上面 <上次修改> 按鈕的代碼註釋,并在grid中重新添加<上次修改>選項
        , '->', { text: '' }],
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
                    //window.setTimeout(searchShow, 1000);
                });
            }
        }
    });
});

