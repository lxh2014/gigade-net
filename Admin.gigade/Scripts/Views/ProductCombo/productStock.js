/*  
 * 
 * 文件名称：productStock.js 
 * 摘    要：組合商品修改和新增 庫存頁面
 * 
 */
var alarm;
var OLD_PRODUCT_ID;

var chekPanel = Ext.create("Ext.form.Panel", {
    height: 80,
    width: 800,
    border: false,
    items: [{
        xtype: 'numberfield',
        decimalPrecision: 0,
        id: 'stock_alarm',
        hidden: true,
        colName: 'stock_alarm',
        name: 'stock_alarm',
        minValue: 0,
        fieldLabel: STOCK_ALARM
    }, {
        xtype: 'fieldcontainer',
        defaultType: 'checkboxfield',
        items: [{
            boxLabel: IGNORE_STOCK,  //edit by Jiajun 2014.09.26
            id: 'ignore_stock',
            /*colName: 'ignore_stock',*/
            hidden: true,
            inputValue: '1',
            readOnly: true
        }, {
            boxLabel: SHORTAGE_MORE,
            id: 'shortage',
            colName: 'shortage',
            hidden: true,
            inputValue: '2'
        }]
    }],
    listeners: {
        beforerender: function () {
            chekPanel.getForm().load({
                type: 'ajax',
                url: '/ProductCombo/QueryProduct',
                actionMethods: 'post',
                params: {
                    "ProductId": Ext.htmlEncode(window.parent.GetProductId()),
                    "OldProductId": OLD_PRODUCT_ID
                },
                success: function (response, opts) {
                    var resText = eval("(" + opts.response.responseText + ")");
                    if (!resText.data) return;
                    if (resText.data.Ignore_Stock == 1) Ext.getCmp("ignore_stock").setValue(true); else Ext.getCmp("ignore_stock").setValue(false);
                    if (resText.data.Shortage == 1) Ext.getCmp("shortage").setValue(true); else Ext.getCmp("shortage").setValue(false);
                }
            });
        }
    }
});

var stockPanel = Ext.create("Ext.form.Panel", {
    border: false,
    autoScroll: true,
    height: window.parent.GetProductId() ? 300 : 500,
    width: 800,
    id: 'stockPanel'
});

Ext.onReady(function () {
    OLD_PRODUCT_ID = window.parent.GetCopyProductId();
    var columns = [{
        header: PRODUCT_NUM,
        sortable: false,
        menuDisabled: true,
        colName: 'product_id',
        hidden: true,
        dataIndex: 'product_id'
    }, {
        header: ITEM_ID,
        sortable: false,
        menuDisabled: true,
        hidden: true,
        colName: 'item_id',
        dataIndex: 'item_id'
    }, {
        header: PRODUCT_NAME,
        sortable: false,
        hidden: true,
        width: 300,
        menuDisabled: true,
        colName: 'product_name',
        dataIndex: 'product_name'
        // 去掉 renderer 保證頁面顯示後綴不重複  edit by zhuoqin0830w  2015/02/11
        //,
        //renderer: function (value, cellmeta, record) {
        //    return value + (record.data.prod_sz ? ' (' + record.data.prod_sz + ')' : '');
        //}
    }, {
        header: SPEC1,
        sortable: false,
        menuDisabled: true,
        hidden: true,
        colName: 'spec_name1',
        dataIndex: 'spec_name1'
    }, {
        header: SPEC2,
        sortable: false,
        menuDisabled: true,
        colName: 'spec_name2',
        hidden: true,
        dataIndex: 'spec_name2'
    }, {
        header: PRODUCT_STOCK,
        sortable: false,
        menuDisabled: true,
        colName: 'item_stock',
        hidden: true,
        dataIndex: 'item_stock',
        renderer: function (value) {
            if (value < alarm) {
                return "<span style='color:red'>" + value + "</span>";
            }
            else {
                return value;
            }
        }
    }];

    var product_id = window.parent.GetProductId();
    if (product_id != "") {
        //若存在product_id則顯示組合商品細項庫存信息
        //查詢組合類型
        Ext.Ajax.request({
            url: '/ProductCombo/QueryProduct',
            method: 'post',
            params: {
                "ProductId": Ext.htmlEncode(window.parent.GetProductId())
            },
            success: function (response) {
                var resText = eval("(" + response.responseText + ")");
                if (!resText.data) return;
                alarm = resText.data.stock_alarm;
                if (resText.data.Combination == 2 || resText.data.Combination == 3) {
                    var combinationStore = Ext.create("Ext.data.Store", {
                        fields: ['product_id', 'item_id', 'product_name', 'prod_sz', 'spec_name1', 'spec_name2', 'item_stock'],
                        proxy: {
                            type: 'ajax',
                            url: '/ProductCombo/QueryItemStock',
                            actionMethods: 'post',
                            reader: {
                                type: 'json',
                                root: 'items'
                            }
                        }
                    });
                    combinationStore.load({ params: { product_id: product_id, pile_id: 0 } });
                    var ficxed = Ext.create("Ext.grid.Panel", {
                        id: 'ComcoFixed',
                        store: combinationStore,
                        border: false,
                        columns: columns,
                        listeners: {
                            scrollershow: function (scroller) {
                                if (scroller && scroller.scrollEl) {
                                    scroller.clearManagedListeners();
                                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                                }
                            }
                        }
                    });

                    stockPanel.add(ficxed);
                    window.parent.updateAuth(stockPanel, 'colName');
                }
                if (resText.data.Combination == 4) {
                    //查詢群組
                    Ext.Ajax.request({
                        url: '/ProductCombo/groupNameQuery',
                        method: 'post',
                        params: {
                            "ProductId": Ext.htmlEncode(window.parent.GetProductId())
                        },
                        success: function (response) {
                            var resText = eval("(" + response.responseText + ")");
                            var pileCount = resText.length;
                            for (var i = 1; i <= pileCount; i++) {
                                /**********************Store*****************/
                                var storeName = "pileStore_" + i;
                                storeName = Ext.create("Ext.data.Store", {
                                    fields: ['product_id', 'item_id', 'product_name', 'spec_name1', 'spec_name2', 'item_stock'],
                                    proxy: {
                                        type: 'ajax',
                                        url: '/ProductCombo/QueryItemStock',
                                        actionMethods: 'post',
                                        reader: {
                                            type: 'json',
                                            root: 'items'
                                        }
                                    }
                                });
                                storeName.load({ params: { product_id: product_id, pile_id: resText[i - 1].Pile_Id } });
                                var title = GROUP + i
                                /*****************Grid***************************/
                                var pileGrid = Ext.create("Ext.grid.Panel", {
                                    title: title,
                                    height: 250,
                                    width: 820,
                                    id: '' + i + '_ComcoFixed',
                                    store: storeName,
                                    border: true,
                                    columns: columns,
                                    listeners: {
                                        scrollershow: function (scroller) {
                                            if (scroller && scroller.scrollEl) {
                                                scroller.clearManagedListeners();
                                                scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                                            }
                                        }
                                    }
                                });

                                stockPanel.add(pileGrid);

                                /*****************end******************************/
                            }
                            window.parent.updateAuth(stockPanel, 'colName');
                        }
                    });
                }
            }
        });
    }

    var viewPort = Ext.create('Ext.Viewport', {
        items: [chekPanel, stockPanel],

        renderTo: Ext.getBody(),
        style: {
            padding: '5 0 0 5'
        },
        autoScroll: true,
        listeners: {
            resize: function () {
                this.doLayout();
            }
        }
    });

    window.parent.updateAuth(viewPort, 'colName');
});

function saveTemp() {
    var mask;
    if (!mask) {
        mask = new Ext.LoadMask(Ext.getBody(), { msg: WAIT });
    }
    mask.show();

    //將數據寫入臨時表
    if (!save()) {
        mask.hide();
        return;
    }

    //臨時表批量更新到正式表
    Ext.Ajax.request({
        url: '/ProductCombo/Temp2Pro',
        method: 'post',
        params: {
            OldProductId: window.parent.GetCopyProductId()
        },
        success: function (response) {
            var data = eval("(" + response.responseText + ")");
            mask.hide();
            if (data.success) {
                Ext.Msg.alert(INFORMATION, data.msg, function () {
                    if (window.parent.GetCopyProductId() != '') {
                        //window.parent.history.go(-1);
                        window.parent.parent.Ext.getCmp('ContentPanel').activeTab.close();
                    } else {
                        window.parent.parent.Ext.getCmp('ContentPanel').activeTab.update(window.top.rtnFrame('/ProductCombo/Index'));
                    }
                });
            }
            else {
                Ext.Msg.alert(INFORMATION, data.msg);
            }
        }
    });
}


//保存數據至數據庫
function save() {
    //添加 遮罩層  避免用戶多次點擊  edit by zhuoqin0830w  2015/09/24
    var mask;
    if (!mask) {
        mask = new Ext.LoadMask(Ext.getBody(), { msg: WAIT });
    }
    mask.show();
    //添加disabled屬性  避免用戶多次點擊  edit by zhuoqin0830w  2015/09/24
    window.parent.setMoveEnable(false);

    var retVal = true;
    var InsertValue = "";

    var ignore = Ext.getCmp("ignore_stock").getValue() == true ? 1 : 0;
    var shortage = Ext.getCmp("shortage").getValue() == true ? 1 : 0;
    var ig_sh_InsertValue = ignore + ',' + shortage + ',' + Ext.getCmp("stock_alarm").getValue();

    Ext.Ajax.request({
        url: '/ProductCombo/SaveBaseInfo',
        method: 'POST',
        async: window.parent.GetProductId() == '' ? false : true,
        params: {
            "product_id": Ext.htmlEncode(window.parent.GetProductId()),
            "OldProductId": OLD_PRODUCT_ID,
            "ig_sh_InsertValue": Ext.htmlEncode(ig_sh_InsertValue)
        },
        success: function (msg) {
            var resMsg = eval("(" + msg.responseText + ")");
            mask.hide();
            if (resMsg.success == true && resMsg.msg != null) {
                Ext.Msg.alert(PROMPT, resMsg.msg);
            }
            if (resMsg.success == false) {
                Ext.Msg.alert(PROMPT, resMsg.msg);
                retVal = false;
            }
            window.parent.setMoveEnable(true);
        }
    });
    return retVal;
}