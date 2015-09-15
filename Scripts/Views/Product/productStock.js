var OLD_PRODUCTID = window.parent.GetCopyProductId();
var defaultArriveDays = 0;
var isEditStock = false;  //add by Jiajun 2014.09.26 判斷庫存是否可以修改

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
                { name: "erp_id", type: "string" }, //add by xiangwang0413w 2014/06/18 (增加ERP廠商編號)
                { name: "remark", type: "string" }, //add by zhuoqin0830w 2015/02/05 增加備註
                { name: "arrive_days", type: "int" },// add by zhuoqin0830w 2014/03/20 增加運達天數
                { name: "default_arrive_days", type: "int" }
    ]
});
//庫存Store
var stockStore = Ext.create("Ext.data.Store", {
    model: 'stock',
    proxy: {
        type: 'ajax',
        url: '/Product/QueryStock',
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
            OldProductId: OLD_PRODUCTID
        });
});


var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
    clicksToEdit: 1
});


Ext.onReady(function () {
    var chekPanel = Ext.create("Ext.form.Panel", {
        width: 800,
        border: false,
        items: [{
            xtype: 'fieldcontainer',
            defaultType: 'checkboxfield',
            items: [{
                boxLabel: IGNORE_STOCK,
                id: 'ignore_stock',
                hidden: true,
                colName: 'ignore_stock',
                inputValue: '1'
            }, {
                boxLabel: SHORTAGE,
                id: 'shortage',
                hidden: true,
                colName: 'shortage',
                inputValue: '2'
            }, {
                xtype: 'panel',
                border: false,
                hidden: window.parent.GetProductId(),
                layout: 'hbox',
                items: [
                    //{
                    //xtype: 'button',
                    //text: '所有規格庫存相同',
                    //margin: '0 5 0 0',
                    //listeners: {
                    //    click: function () {
                    //        var stockGrid = Ext.getCmp("stockGrid").store.data.items;
                    //        for (var i = 0; i < stockGrid.length - 1; i++) {
                    //            stockStore.getAt(i + 1).set('item_stock', stockStore.getAt(0).get('item_stock'));
                    //        }
                    //    }
                    //}
                    //},
                {
                    xtype: 'button',
                    text: ALL_STANDARD_WARNING_VALUE_IDENTICAL,
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
                    text: ALL_STANDARD_SHAPE_CODE_VALUE_IDENTICAL,
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
        }]//,
        //listeners: {
           // beforerender: function () {
                //Ext.Ajax.request({
                //    url: '/Product/QueryProduct',
                //    method: 'post',
                //    params: {
                //        "ProductId": window.parent.GetProductId(),
                //        "OldProductId": OLD_PRODUCTID
                //    },
                //    success: function (response) {
                //        var reStr = eval("(" + response.responseText + ")");
                //        if (reStr && reStr.data) {
                //            if (reStr.data.Ignore_Stock == 1) Ext.getCmp("ignore_stock").setValue(true); else Ext.getCmp("ignore_stock").setValue(false);
                //            if (reStr.data.Shortage == 1) Ext.getCmp("shortage").setValue(true); else Ext.getCmp("shortage").setValue(false);

                //        }
                //        isEditStock = reStr.data.IsEdit; //edit by xiangwang0413w 2014/10/09 設置庫存是否可以修改
                //        if (!isEditStock&&reStr.data.Prepaid != 1 && reStr.data.Product_Mode != 2) {       //add by Jiajun 2014.09.26 判斷庫存是否可以修改
                //            isEditStock = true;
                //        }
                        
                //    }
                //});
           // }
       // }
    })

    var stockPanel = Ext.create("Ext.grid.Panel", {
        id: 'stockGrid',
        store: stockStore,
        width: 960,
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
            hidden: true,
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
            hidden: true,
            id: 'spec_title_2'
        }, {
            dataIndex: 'spec_id_2',
            hidden: true
        }, {
            header: PRODUCT_STOCK,//庫存
            sortable: false,
            colName: 'item_stock',
            menuDisabled: true,
            width: 60,
            dataIndex: 'item_stock',
            hidden: true,
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
            width: 60,
            dataIndex: 'item_alarm',
            hidden: true,
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
            hidden: true,
            editor: {
                xtype: 'textfield',
                allowBlank: false
            }
        }, {//add by xiangwang0413w 2014/06/18 (增加ERP廠商編號)
            header: ERP_ID,
            dataIndex: 'erp_id',
            sortable: false,
            width: 100,
            hidden: window.parent.GetProductId() == ""
        }, {// add by zhuoqin0830w 2014/02/05 增加備註
            header: REMARK,
            dataIndex: 'remark',
            colName: 'remark',
            sortable: false,
            menuDisabled: true,
            width: 150,
            editor: {
                xtype: 'textfield',
                id: 'remark',
                allowBlank: false
            }
        }, {// add by zhuoqin0830w 2014/03/20 增加運達天數
            header: TRANSPORT_ARRIVE_DAY,
            dataIndex: 'arrive_days',
            colName: 'arrive_days',
            //hidden:true,
            sortable: false,
            menuDisabled: true,
            width: 60,
            editor: {
                xtype: 'numberfield',
                id: 'arrive_days',
                decimalPrecision: 0,
                minValue: 0,
                allowBlank: false
            }
        },{
            header: BARCODE,
            sortable: false,
            menuDisabled: true,
            flex: 1,
            //width: 180,
            dataIndex: 'barcode',
            hidden: true,
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
            }//,
            //beforeedit: function (e) {    //add by Jiajun 2014.09.26 判斷庫存是否可以修改
            //    //if (!isEditStock && e.field == "item_stock") {
            //    //    return false;
            //    //}
            //},
            //render: function () {
            //    for (var i = 0, j = stockStore.getCount() ; i < j; i++) {
            //        var record = stockStore.getAt(i);
            //        if (window.parent.GetProductId() == "" && isEditStock) {
            //            record.set('item_stock', 99);
            //        }
            //        else {
            //            record.set('item_stock', 0);
            //        }
            //    }
            //},
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

    stockStore.load({
        callback: function () {
            defaultArriveDays = this.getAt(0).data.default_arrive_days;
            queryProduct(stockPanel);
        }
    });



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
    //臨時表批量更新到正式表
    Ext.Ajax.request({
        url: '/Product/Temp2Pro',
        method: 'post',
        params: {
            OldProductId: OLD_PRODUCTID
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
                        window.parent.parent.Ext.getCmp('ContentPanel').activeTab.update(window.top.rtnFrame('/Product/ProductSave'));
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
function save(functionid) {
    cellEditing.completeEdit();
    var retVal = true;
    //var InsertValue = "";
    var InsertValueArray = [];//edit by wwei0216w 將字符串格式傳遞改成json傳遞

    
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
        // add by zhuoqin0830w 2014/02/05 增加備註
        var remark = stockGrid[i].get("remark");
        //add by zhuoqin0830w 2014/03/20 增加運達天數
        var arrive_days = stockGrid[i].get("arrive_days") - defaultArriveDays;
        //InsertValue += spec_title_1 + "," + spec_title_2 + "," + item_stock + "," + item_alarm + "," + barcode + "," + item_id + "," + item_code + "," + erp_id + "," + remark + "," + arrive_days + ";";
        InsertValueArray.push({//edit by wwei0216w 將字符串格式傳遞改成json傳遞
            'Spec_Id_1': spec_title_1,
            'Spec_Id_2': spec_title_2,
            'Item_Stock': item_stock,
            'Item_Alarm': item_alarm,
            'Barcode': barcode,
            'Item_Id': item_id,
            'Item_Code': item_code,
            'Erp_Id': erp_id,
            'Remark': remark,
            'Arrive_Days': arrive_days
        });
    }
    var InsertValue = Ext.encode(InsertValueArray);//edit by wwei0216w 將字符串格式傳遞改成json傳遞
    var ignore = Ext.getCmp("ignore_stock").getValue() == true ? 1 : 0;
    var shortage = Ext.getCmp("shortage").getValue() == true ? 1 : 0;
    var ig_sh_InsertValue = ignore + ',' + shortage;



    if (!functionid) {
        functionid = '';
    }

    Ext.Ajax.request({
        url: '/Product/StockSave',
        method: 'POST',
        async: false,
        params: {
            "product_id": Ext.htmlEncode(window.parent.GetProductId()),
            OldProductId: window.parent.GetCopyProductId(),
            "InsertValue": InsertValue,//Ext.htmlEncode(InsertValue),
            "ig_sh_InsertValue": Ext.htmlEncode(ig_sh_InsertValue),
            "function": functionid,
            "batch": window.parent.GetBatchNo()
        },
        success: function (msg) {
            var resMsg = eval("(" + msg.responseText + ")");
            if (resMsg.success == true && resMsg.msg != null) {
                Ext.Msg.alert(PROMPT, resMsg.msg);
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

function queryProduct(stockPanel) {
    Ext.Ajax.request({
        url: '/Product/QueryProduct',
        method: 'post',
        params: {
            "ProductId": window.parent.GetProductId(),
            "OldProductId": OLD_PRODUCTID
        },
        success: function (response) {
            var reStr = eval("(" + response.responseText + ")");
            if (reStr && reStr.data) {
                if (reStr.data.Ignore_Stock == 1) Ext.getCmp("ignore_stock").setValue(true); else Ext.getCmp("ignore_stock").setValue(false);
                if (reStr.data.Shortage == 1) Ext.getCmp("shortage").setValue(true); else Ext.getCmp("shortage").setValue(false);
            }
            isEditStock = reStr.data.IsEdit; //edit by xiangwang0413w 2014/10/09 設置庫存是否可以修改
            if (!isEditStock && (reStr.data.Prepaid == 1 || reStr.data.Product_Mode == 2)) {       //add by Jiajun 2014.09.26 判斷庫存是否可以修改
                // isEditStock = true;
                stockPanel.columns[6].getEditor().disable();
            }
            if (defaultArriveDays == 0) {
                var productMode = "";
                switch (reStr.data.Product_Mode) {
                    case 1:
                        productMode = ONESELF_SELL;
                        break;
                    case 2:
                        productMode = WAREHOUSE;
                        break;
                    case 3:
                        productMode = DISPATCH;
                        break;
                }
                Ext.Msg.alert(INFORMATION, DELIVER + productMode + DATE_IS_ZERO);
            }
        }
    });
}
