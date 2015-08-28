PRODUCT_ID = window.parent.GetProductId();
OLD_PRODUCT_ID = window.parent.GetCopyProductId();
ISEDIT = window.parent.GetIsEdit() == "true" ? true : false; //確定當前面板是否是修改

//庫存Model
Ext.define("stock", {
    extend: 'Ext.data.Model',
    fields: [
                { name: "item_id", type: "string" },
                { name: "spec_title_1", type: "string" },
                { name: "spec_id_1", type: "string" },
                { name: "spec_title_2", type: "string" },
                { name: "spec_id_2", type: "string" },
                { name: "item_stock", type: "string" },
                { name: "item_alarm", type: "string" },
                { name: "barcode", type: "string" },
                { name: "item_code", type: "string" },
                { name: "erp_id", type: "string"} //add by xiangwang0413w 2014/06/18 (增加ERP廠商編號)
               , { name: 'prepaid', type: 'int' },
               {name:'product_mode',type:'string'}
    ]
});
//庫存Store
var stockStore = Ext.create("Ext.data.Store", {
    model: 'stock',
    proxy: {
        type: 'ajax',
        url: '/VendorProduct/QueryStock',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        },
        failure: function (response) {
            var result = Ext.decode(response.responseText);
            Ext.Msg.alert(PROMPT, result.Msg);
        }
    }
});

stockStore.on('beforeload', function () {
    Ext.apply(stockStore.proxy.extraParams,
        {
            product_id: Ext.htmlEncode(window.parent.GetProductId()),
            OldProductId: OLD_PRODUCT_ID
        });
});


    var product_mode;
    var prepaid;
var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
    clicksToEdit: 1,
    listeners: {
        beforeedit: function (e, eOpts) {
            if (product_mode == 2||prepaid==1) {
                if (e.colIdx == 6) {
                    return false;
                }
            }
        }
    }

});


Ext.onReady(function () {

    var chekPanel = Ext.create("Ext.form.Panel", {
        width: 800,
        border: false,
        items: [{
            xtype: 'fieldcontainer',
            defaultType: 'checkboxfield',
            items: [{
                boxLabel: IGNORE_STOCK, //庫存為0時,還能販售(不管庫存)
                id: 'ignore_stock',
                hidden: false,
                colName: 'ignore_stock',
                inputValue: '1'
            }, {
                boxLabel: SHORTAGE, //補貨中停止販售
                id: 'shortage',
                hidden: false,
                colName: 'shortage',
                inputValue: '2'
            }, {
                xtype: 'panel',
                border: false,
                hidden: ISEDIT,
                layout: 'hbox',
                items: [{
                    xtype: 'button',
                    text: '所有規格庫存相同',
                    margin: '0 5 0 0',
                    listeners: {
                        click: function () {
                            var stockGrid = Ext.getCmp("stockGrid").store.data.items;
                            for (var i = 0; i < stockGrid.length - 1; i++) {
                                stockStore.getAt(i + 1).set('item_stock', stockStore.getAt(0).get('item_stock'));
                            }
                        }
                    }
                }, {
                    xtype: 'button',
                    text: '所有規格警告值相同',
                    margin: '0 5 0 0',
                    listeners: {
                        click: function () {
                            var stockGrid = Ext.getCmp("stockGrid").store.data.items;
                            for (var i = 0; i < stockGrid.length - 1; i++) {
                                stockStore.getAt(i + 1).set('item_alarm', stockStore.getAt(0).get('item_alarm'));
                            }
                        }
                    }
                }, {
                    xtype: 'button',
                    text: '所有規格條形碼相同',
                    margin: '0 5 0 0',
                    listeners: {
                        click: function () {
                            var stockGrid = Ext.getCmp("stockGrid").store.data.items;
                            for (var i = 0; i < stockGrid.length - 1; i++) {
                                stockStore.getAt(i + 1).set('barcode', stockStore.getAt(0).get('barcode'));
                            }
                        }
                    }
                }]
            }]
        }],
        listeners: {
            beforerender: function () {
                Ext.Ajax.request({
                    url: '/VendorProduct/QueryProduct',
                    method: 'post',
                    params: {
                        "ProductId": window.parent.GetProductId(),
                        "OldProductId": OLD_PRODUCT_ID
                    },
                    success: function (response) {
                        var reStr = eval("(" + response.responseText + ")");
                        if (reStr && reStr.data) {
                            prepaid = reStr.data.Prepaid;
                            product_mode = reStr.data.Product_Mode;
                            if (reStr.data.Ignore_Stock == 1) Ext.getCmp("ignore_stock").setValue(true); else Ext.getCmp("ignore_stock").setValue(false);
                            if (reStr.data.Shortage == 1) Ext.getCmp("shortage").setValue(true); else Ext.getCmp("shortage").setValue(false);
                        }
                    }
                });
            }
        }
    })


    var stockPanel = Ext.create("Ext.grid.Panel", {
        id: 'stockGrid',
        store: stockStore,
        width: 800,
        //height:window.parent.GetProductId()?300:500,
        plugins: [cellEditing],
        border: false,
        columns: [{ xtype: 'rownumberer', width: 50, align: 'center' }, {
            dataIndex: 'item_id',
            hidden: true
        }, {
            header: SPEC1,
            sortable: false,
            menuDisabled: true,
            id: 'spec_title_1',
            colName: 'spec_title_1',
            hidden: false,
            dataIndex: 'spec_title_1',
            width: 100
        }, {
            dataIndex: 'spec_id_1',
            hidden: true
        }, {
            header: SPEC2,
            dataIndex: 'spec_title_2',
            sortable: false,
            colName: 'spec_title_2',
            menuDisabled: true,
            width: 100,
            hidden: false,
            id: 'spec_title_2'
        }, {
            dataIndex: 'spec_id_2',
            hidden: true
        }, {
            header: PRODUCT_STOCK + NOT_EMPTY,
            sortable: false,
            colName: 'item_stock',
            menuDisabled: true,
            width: 100,
            dataIndex: 'item_stock',
            hidden: false,
            id: 'item_stock',
            editor: {
                xtype: 'numberfield',
                decimalPrecision: 0,
                minValue: 0,
                id: 'stock',
                allowBlank: false
            }
        }, {
            header: ITEM_ALARM + NOT_EMPTY,
            sortable: false,
            menuDisabled: true,
            colName: 'item_alarm',
            width: 100,
            dataIndex: 'item_alarm',
            hidden: false,
            id: 'item_alarm',
            editor: {
                xtype: 'numberfield',
                decimalPrecision: 0,
                minValue: 0,
                id: 'alarm',
                allowBlank: false
            }
        }, {
            header: ITEM_CODE,
            dataIndex: 'item_code',
            colName: 'item_code',
            sortable: false,
            menuDisabled: true,
            width: 100,
            hidden: true, //與吉甲地商品後台保持一致並未存入數據庫並且此欄位已被價格面板的廠商商品編號一欄佔用
            editor: {
                xtype: 'textfield',
                allowBlank: false
            }

        }, {//add by xiangwang0413w 2014/06/18 (增加ERP廠商編號)
            header: ERP_ID,
            dataIndex: 'erp_id',
            sortable: false,
            width: 100,
            hidden: !ISEDIT
        }, {
            header: BARCODE,
            sortable: false,
            menuDisabled: true,
            width: 200,
            dataIndex: 'barcode',
            hidden: false,
            colName: 'barcode',
            id: 'barcode',
            editor: {
                xtype: 'textfield',
                id: 'barcode',
                allowBlank: false
            }
        }],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }

        }
    });


    Ext.create('Ext.Viewport', {
        items: [chekPanel, stockPanel],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                this.doLayout();
            }
        }
    });

    stockStore.load();
    window.parent.updateAuth(stockPanel, 'colName');
    window.parent.updateAuth(chekPanel, 'colName');

})



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
    //將商品狀態改為12：新建
    Ext.Ajax.request({
        url: '/VendorProduct/SaveTemp',
        method: 'post',
        params: {
            ProductId: PRODUCT_ID,
            OldProductId: OLD_PRODUCT_ID
        },
        success: function (response) {
            var data = eval("(" + response.responseText + ")");
            mask.hide();
            if (data.success) {
                Ext.Msg.alert(INFORMATION, data.msg, function () {
                    if (OLD_PRODUCT_ID != '') {
                        window.parent.parent.Ext.getCmp('ContentPanel').activeTab.close();
                    } else {
                        window.parent.parent.Ext.getCmp('ContentPanel').activeTab.update(window.top.rtnFrame('/VendorProduct/ProductSave'));
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
    cellEditing.completeEdit();
    var retVal = true;
    var InsertValue = "";
    var stockGrid = Ext.getCmp("stockGrid").store.data.items;
    for (var i = 0; i < stockGrid.length; i++) {
        var spec_title_1 = stockGrid[i].get("spec_id_1");
        var spec_title_2 = stockGrid[i].get("spec_id_2");
        var item_stock = stockGrid[i].get("item_stock");
        var item_alarm = stockGrid[i].get("item_alarm");
        var item_code = stockGrid[i].get("item_code");
        var barcode = stockGrid[i].get("barcode");
        var item_id = stockGrid[i].get("item_id");
        var erp_id = stockGrid[i].get("erp_id");
        InsertValue += spec_title_1 + "," + spec_title_2 + "," + item_stock + "," + item_alarm + "," + barcode + "," + item_id + "," + item_code + "," + erp_id + ";";
    }
    var ignore = Ext.getCmp("ignore_stock").getValue() == true ? 1 : 0;
    var shortage = Ext.getCmp("shortage").getValue() == true ? 1 : 0;
    var ig_sh_InsertValue = ignore + ',' + shortage;


    Ext.Ajax.request({
        url: '/VendorProduct/StockSave',
        method: 'POST',
        async: false,
        params: {
            "product_id": Ext.htmlEncode(window.parent.GetProductId()),
            OldProductId: window.parent.GetCopyProductId(),
            "InsertValue": Ext.htmlEncode(InsertValue),
            "ig_sh_InsertValue": Ext.htmlEncode(ig_sh_InsertValue)
        },
        success: function (msg) {
            var resMsg = eval("(" + msg.responseText + ")");
            if (resMsg.success == true && resMsg.msg != null) {
                if (ISEDIT) {
                    Ext.Msg.alert(PROMPT, resMsg.msg);
                }
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
