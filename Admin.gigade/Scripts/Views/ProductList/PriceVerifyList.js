var pageSize = 10;

var priceListStore = Ext.create("Ext.data.Store", {
    fields: ["product_image", "product_id", "brand_name",
             "product_name", 'prod_sz', "combination", "product_status",
             "site_name", "user_level", "user_email",
             "price_status", "price", "event_price", "cost", "event_cost",
             "event_start", "event_end", "apply_id", "apply_time", "apply_user", "price_master_id", "CanDo", "site_id", "level", "user"],
    autoLoad: false,
    pageSize: pageSize,
    proxy: {
        type: 'ajax',
        url: '/ProductList/QueryPriceVerifyList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'total'
        }
    }
});

priceListStore.on('beforeload', function () {
    var brandId = Ext.getCmp("brand_id").getValue();
    if (brandId == null) {
        brandId = 0;
    }
    var siteId = Ext.getCmp("site_id").getValue() == null ? 0 : Ext.getCmp("site_id").getValue();
    var userLevel = Ext.getCmp("user_level").getValue() == null ? 0 : Ext.getCmp("user_level").getValue();
    var productType = Ext.getCmp("combination").getValue() == null ? 0 : Ext.getCmp("combination").getValue();
    var productStatus = Ext.getCmp("product_status").getValue() == null ? -1 : Ext.getCmp("product_status").getValue();
    var dataType = Ext.getCmp("date_type").getValue();
    var timeStart = Ext.getCmp("time_start").rawValue;
    var timeEnd = Ext.getCmp("time_end").rawValue;
    var key = Ext.getCmp("key").getValue();

    var queryCondition = "{brand_id:'" + brandId + "',site_id:'" + siteId + "',";
    queryCondition += "user_level:'" + userLevel + "',combination:'" + productType + "',";
    queryCondition += "product_status:'" + productStatus + "',date_type:'" + dataType + "',";
    queryCondition += "name_number:'" + key + "',time_start:'',time_end:''}";
    Ext.apply(priceListStore.proxy.extraParams,
        {
            queryCondition: Ext.htmlEncode(queryCondition),
            time_start: Ext.htmlEncode(timeStart),
            time_end: Ext.htmlEncode(timeEnd)
        });
});

//列选择模式
var sm = Ext.create('Ext.selection.CheckboxModel', {
    storeColNameForHide: 'CanDo',//根據Cando來隱藏checkbox
    listeners: {
        selectionchange: function (sm, selections) {
            if (selections.length == 0) {
                Ext.getCmp("pass").setDisabled(true);
                Ext.getCmp("reject").setDisabled(true);
                //Ext.getCmp("last_modify").setDisabled(true);
            }
            else {
                Ext.getCmp("pass").setDisabled(false);
                Ext.getCmp("reject").setDisabled(false);
                //Ext.getCmp("last_modify").setDisabled(false);
            }
        }
    }
});

var price_master = { dataIndex: 'price_master_id', hidden: true };

var proColumns = c_pro_base;
proColumns.push(c_pro_type);            //商品類型
proColumns.push(c_pro_status);          //商品狀態
proColumns.push(c_pro_site);            //站臺
proColumns.push(c_pro_userlevel);       //會員等級
proColumns.push(c_pro_email);           //會員email
proColumns.push(c_pro_pricestatus);     //價格狀態
proColumns.push(c_pro_itemoney);        //售價
proColumns.push(c_pro_cost);            //成本 edit by jiajun 2014/08/14 調整欄位 
proColumns.push(c_pro_itemeventmoney);  //活動價
///edit by wangwei0216w  功能: 添加成本與活動成本
proColumns.push(c_pro_eventcost);       //活動成本
///時間2014/8/8
proColumns.push(c_pro_eventdate);       //活動期間
proColumns.push(c_pro_askfordate);      //申請時間
proColumns.push(c_pro_askforperson);    //申請人
proColumns.push(price_master);          //價格Id

//grid
var clientHeight = document.documentElement.clientHeight;
var clientWidth = document.documentElement.clientWidth;
var frmHeight;

function showResultText(btn, text) {
    if (btn == "ok") {
        var row = Ext.getCmp('showGrid').getSelectionModel().getSelection();
        //var prices = "";
        //for (var i = 0; i < row.length; i++) {
        //    prices += row[i].data.price_master_id + ',';
        //}
        var priceMasters = new Array();
        for (var i = 0, j = row.length; i < j ; i++) {
            priceMasters.push({ product_id: row[i].data.product_id, price_master_id: row[i].data.price_master_id, apply_id: row[i].data.apply_id });
        }

        Ext.Ajax.request({
            url: '/ProductList/PriceVerify',
            method: 'post',
            params: {
                "type": 2,
                "reason": text,
                //"prices": Ext.htmlEncode(prices.substring(0, prices.length - 1)),
                "priceMasters": Ext.encode(priceMasters),
                "function": 'reject'
            },
            success: function (response, opts) {
                var resText = Ext.decode(response.responseText);
                if (resText.success = true) {
                    Ext.Msg.alert(INFORMATION, VERIFY_SUCCESS);
                    priceListStore.loadPage(1);
                }
                else {
                    Ext.Msg.alert(INFORMATION, VERIFY_FALIURE);
                }
            }
        });
    }
    else {
        return;
    }
}

function searchShow() {
    Ext.getCmp('key').setValue(Ext.getCmp('key').getValue().replace(/\s+/g, ','));
    Ext.getCmp("showGrid").show();
    priceListStore.loadPage(1);
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

    var frm = Ext.create("Ext.form.Panel", {
        layout: 'column',
        id: 'frm',
        height: 180,
        border: false,
        width: 700,
        style: {
            padding: '5 0 0 5'
        },
        defaults: { anchor: '95%', msgTarget: "side", padding: '5 0 3 5' },
        items: [{
            xtype: 'panel',
            columnWidth: .90,
            border: 0,
            layout: 'anchor',
            items: [brand, site, type_status, start_end, key_query]
        }],
        buttonAlign: 'right',
        buttons: [{
            text: BTN_SEARCH,
            id: 'btn_search',
            iconCls: 'ui-icon ui-icon-search-2',
            width: 100,
            listeners: {
                afterrender: function () {
                },
                click: function () {
                    searchShow();
                }
            }
        }, {
            text: RESET,
            id: 'btn_reset',
            iconCls: 'ui-icon ui-icon-reset',
            width: 100,
            listeners: {
                click: function () {
                    frm.getForm().reset();
                    Ext.getCmp("site_id").setValue(siteStore.data.items[0].data.Site_Id);
                    Ext.getCmp("user_level").setValue(userlevelStore.data.items[0].data.parameterCode);
                    Ext.getCmp("combination").setValue(ComboStore.data.items[0].data.parameterCode);
                    Ext.getCmp("product_status").setValue(prodStatusStore.data.items[0].data.parameterCode);
                    Ext.getCmp("date_type").setValue(DateStore.data.items[1].data.code);

                }
            }
        }]
    });

    var showGrid = Ext.create("Ext.grid.Panel", {
        selModel: sm,
        height: document.documentElement.clientWidth <= 700 ? clientHeight - 180 - 20 : clientHeight - 180,
        id: 'showGrid',
        hidden: true,
        store: priceListStore,
        columns: proColumns,
        listeners: {
            viewready: function () {
                setShow(showGrid, 'colName');
            },
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        tbar: [{
            text: VERIFY_PASS,
            id: 'pass',
            hidden: true,
            colName: 'pass',
            disabled: true,
            iconCls: 'icon-accept',
            listeners: {
                click: function () {
                    var myMask = new Ext.LoadMask(Ext.getBody(), {
                        msg: 'Loading...'
                    });
                    myMask.show();

                    var row = Ext.getCmp('showGrid').getSelectionModel().getSelection();

                    var priceMasters = new Array();
                    for (var i = 0, j = row.length; i < j ; i++) {
                        priceMasters.push({ product_id: row[i].data.product_id, price_master_id: row[i].data.price_master_id, apply_id: row[i].data.apply_id });
                    }

                    Ext.Ajax.request({
                        url: '/ProductList/PriceVerify',
                        method: 'post',
                        timeout: 1000 * 60 * 2,
                        params: {
                            "type": 1,
                            "priceMasters": Ext.encode(priceMasters),
                            "function": 'pass'
                        },
                        success: function (response, opts) {
                            var resText = Ext.decode(response.responseText);
                            if (resText.success = true) {
                                Ext.Msg.alert(INFORMATION, PASS_SUCCESS);
                                priceListStore.loadPage(1);
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, PASS_FAILURE);
                            }
                            myMask.hide();
                        },
                        failure: function (response, opts) {
                            myMask.hide();
                            Ext.Msg.alert(NOTICE, OVERTIME_SAVE);
                        }
                    });
                }
            }
        }, {
            text: VERIFY_BACK,
            disabled: true,
            hidden: true,
            colName: 'reject',
            id: 'reject',
            icon: '../../../Content/img/icons/drop-no.gif',
            listeners: {
                click: function () {
                    Ext.MessageBox.show({
                        title: INFORMATION,
                        msg: BACK_REASON,
                        width: 300,
                        buttons: Ext.MessageBox.OKCANCEL,
                        multiline: true,
                        fn: showResultText
                    });
                }
            }
        }
        //{
        //    text: LAST_MODIFY,
        //    colName: 'last_modify',
        //    id: 'last_modify',
        //    disabled: true,
        //    hidden: true,
        //    iconCls: 'icon-edit',
        //    //handler: function () {
        //    //    var rows = Ext.getCmp('showGrid').getSelectionModel().getSelection();
        //    //    if (rows.length == 0) {
        //    //        Ext.Msg.alert(INFORMATION, NO_SELECTION);
        //    //    } else if (rows.length > 1) {
        //    //        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
        //    //    } else if (rows.length == 1) {
        //    //        Ext.create('Ext.window.Window', {
        //    //            title: '上次修改記錄',
        //    //            width: 1000,
        //    //            height: document.documentElement.clientHeight * 610 / 783,
        //    //            autoScroll: true,
        //    //            bodyStyle: 'background:#ffffff; padding:5px;',
        //    //            closeaction: 'destroy',
        //    //            resizable: false,
        //    //            draggable: false,
        //    //            modal: true,
        //    //            listeners: {
        //    //                show: function (e, eOpts) {
        //    //                    Ext.Ajax.request({
        //    //                        url: '/ProductList/QueryLastModifyRecord',
        //    //                        method: 'post',
        //    //                        params: {
        //    //                            Product_Id: rows[0].data.product_id,
        //    //                            Type: 'price',
        //    //                            Site_id: rows[0].data.site_id,
        //    //                            User_Level: rows[0].data.level,
        //    //                            User_id: rows[0].data.user
        //    //                        },
        //    //                        success: function (form, action) {
        //    //                            var result = Ext.decode(form.responseText);
        //    //                            if (result.success) {
        //    //                                if (result.html) {
        //    //                                    e.update(Ext.htmlDecode(result.html));
        //    //                                } else {
        //    //                                    e.destroy();
        //    //                                    Ext.Msg.alert(INFORMATION, SEARCH_NO_DATA);
        //    //                                }
        //    //                            }
        //    //                        },
        //    //                        failure: function () {
        //    //                            Ext.Msg.alert(INFORMATION, FAILURE);
        //    //                        }
        //    //                    });
        //    //                }
        //    //            }
        //    //        }).show();
        //    //    }
        //    //}
        //}  eidt by wangwei0216w 2014/8/12 說明:將上面 <上次修改> 按鈕的代碼註釋,并在grid中重新添加<上次修改>選項
        , '->', { text: ' ' }],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: priceListStore,
            pageSize: pageSize,
            displayInfo: true
        })
    });

    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        border: false,
        items: [frm, showGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                if (document.documentElement.clientWidth <= 700) {
                    Ext.getCmp("showGrid").height = document.documentElement.clientHeight - 180 - 20;
                }
                else {
                    Ext.getCmp("showGrid").height = document.documentElement.clientHeight - 180;
                }
                this.doLayout();
            },
            beforerender: function () {
                ToolAuthorityQuery(function () {
                    setShow(frm, 'colName');
                    //window.setTimeout(searchShow, 1000);
                });
            }
        }
    });
    Ext.getCmp('date_type').setValue(DateStore.data.items[1].data.code);
});

function addDateType() {
    if (DateStore.getCount() == 1) {
        DateStore.add(r_apply, r_start, r_end);
    }
}