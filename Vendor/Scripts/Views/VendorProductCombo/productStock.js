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
        //        hidden: true,
        colName: 'stock_alarm',
        name: 'stock_alarm',
        minValue: 0,
        fieldLabel: STOCK_ALARM
    }, {
        xtype: 'fieldcontainer',
        defaultType: 'checkboxfield',
        items: [{
            boxLabel: IGNORE_STOCK,
            id: 'ignore_stock',
            colName: 'ignore_stock',
            //            hidden: true,
            inputValue: '1'
            //            readOnly: true
        }, {
            boxLabel: SHORTAGE_MORE,
            id: 'shortage',
            colName: 'shortage',
            //            hidden: true,
            inputValue: '2'
        }]
    }],
    listeners: {
        beforerender: function () {
            chekPanel.getForm().load({
                type: 'ajax',
                url: '/VendorProductCombo/QueryProduct',
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
})

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
        //        hidden: true,
        dataIndex: 'vendor_product_id'
    }, {
        header: ITEM_ID,
        sortable: false,
        menuDisabled: true,
        //        hidden: true,
        colName: 'item_id',
        dataIndex: 'item_id'
    }, {
        header: PRODUCT_NAME,
        sortable: false,
        //        hidden: true,
        width: 300,
        menuDisabled: true,
        colName: 'product_name',
        dataIndex: 'product_name'
    }, {
        header: SPEC1,
        sortable: false,
        menuDisabled: true,
        //        hidden: true,
        colName: 'spec_name1',
        dataIndex: 'spec_name1'
    }, {
        header: SPEC2,
        sortable: false,
        menuDisabled: true,
        colName: 'spec_name2',
        //        hidden: true,
        dataIndex: 'spec_name2'
    }, {
        header: PRODUCT_STOCK,
        sortable: false,
        menuDisabled: true,
        colName: 'item_stock',
        //        hidden: true,
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

    var isEdit = window.parent.GetIsEdit();
    if (isEdit == "true") {
        //若存在product_id則顯示組合商品細項庫存信息
        //查詢組合類型
        Ext.Ajax.request({
            url: '/VendorProductCombo/QueryProduct',
            method: 'post',
            params: {
                "ProductId": Ext.htmlEncode(window.parent.GetProductId())
            },
            success: function (response) {
                var resText = eval("(" + response.responseText + ")");
                if (!resText) return;
                alarm = resText.data.stock_alarm;
                if (resText.data.Combination == 2 || resText.data.Combination == 3) {
                    var combinationStore = Ext.create("Ext.data.Store", {
                        fields: ['vendor_product_id', 'item_id', 'product_name', 'spec_name1', 'spec_name2', 'item_stock'],
                        proxy: {
                            type: 'ajax',
                            url: '/VendorProductCombo/VendorQueryItemStock',
                            actionMethods: 'post',
                            reader: {
                                type: 'json',
                                root: 'items'
                            }
                        }
                    });
                    combinationStore.load({ params: { product_id: Ext.htmlEncode(window.parent.GetProductId())} });
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
                        url: '/VendorProductCombo/groupNameQuery',
                        method: 'post',
                        params: {
                            ProductId: Ext.htmlEncode(window.parent.GetProductId())
                        },
                        success: function (response) {
                            var resText = eval("(" + response.responseText + ")");
                            var pileCount = resText.length;
                            for (var i = 1; i <= pileCount; i++) {
                                /**********************Store*****************/
                                var storeName = "pileStore_" + i;
                                storeName = Ext.create("Ext.data.Store", {
                                    fields: ['vendor_product_id', 'item_id', 'product_name', 'spec_name1', 'spec_name2', 'item_stock'],
                                    proxy: {
                                        type: 'ajax',
                                        url: '/VendorProductCombo/VendorQueryItemStock',
                                        actionMethods: 'post',
                                        reader: {
                                            type: 'json',
                                            root: 'items'
                                        }
                                    }
                                });

                                storeName.load({ params: { product_id: Ext.htmlEncode(window.parent.GetProductId()), pile_id: resText[i - 1].Pile_Id} });
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

})



function saveTemp() {
    var mask;
    var InsertValue = "";

    var ignore = Ext.getCmp("ignore_stock").getValue() == true ? 1 : 0;
    var shortage = Ext.getCmp("shortage").getValue() == true ? 1 : 0;
    var ig_sh_InsertValue = ignore + ',' + shortage + ',' + Ext.getCmp("stock_alarm").getValue();
    if (!mask) {
        mask = new Ext.LoadMask(Ext.getBody(), { msg: WAIT });
    }
    mask.show();
    //將數據寫入臨時表
    if (!save()) {
        mask.hide();
        return;
    }
    //點儲存是儲存到臨時表
    Ext.Ajax.request({
        url: '/VendorProductCombo/SaveTemp',
        method: 'post',
        params: {
            "ProductId": Ext.htmlEncode(window.parent.GetProductId()),
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
                        window.parent.parent.Ext.getCmp('ContentPanel').activeTab.update(window.top.rtnFrame('/VendorProductCombo/Index'));
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
    var retVal = true;
    var InsertValue = "";

    var ignore = Ext.getCmp("ignore_stock").getValue() == true ? 1 : 0;
    var shortage = Ext.getCmp("shortage").getValue() == true ? 1 : 0;
    var ig_sh_InsertValue = ignore + ',' + shortage + ',' + Ext.getCmp("stock_alarm").getValue();

    Ext.Ajax.request({
        url: '/VendorProductCombo/SaveBaseInfo',
        method: 'POST',
        async: false,
        params: {
            "product_id": Ext.htmlEncode(window.parent.GetProductId()),
            "OldProductId": OLD_PRODUCT_ID,
            "ig_sh_InsertValue": Ext.htmlEncode(ig_sh_InsertValue)
        },
        success: function (msg) {
            var resMsg = eval("(" + msg.responseText + ")");

            if (resMsg.success == true && resMsg.msg != null) {
                Ext.Msg.alert(PROMPT, resMsg.msg);
            } else if (resMsg.msg == null) {
                Ext.Msg.alert(INFORMATION, SUCCESS);
            }
            if (resMsg.success == false) {
                Ext.Msg.alert(PROMPT, resMsg.msg);
                retVal = false;
                window.parent.setMoveEnable(true);
            }
        }
    });
    return retVal;
}
