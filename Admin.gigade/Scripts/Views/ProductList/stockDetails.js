var stockPanel;
var chekPanel;


//組合商品庫存
function comboStockDetails(combination, product_id) {

    stockPanel = Ext.create("Ext.form.Panel", {
        border: false,
        //height: 400,
        width: 840,
        id: 'stockPanel'
    });

    chekPanel = Ext.create("Ext.form.Panel", {
        height: 90,
        width: 800,
        border: false,
        items: [{
            xtype: 'displayfield',
            id: 'stock_alarm',
            hidden: true,
            colName: 'stock_alarm',
            name: 'stock_alarm',
            fieldLabel: STOCK_ALARM
        }, {
            xtype: 'fieldcontainer',
            defaultType: 'checkboxfield',
            items: [{
                boxLabel: IGNORE_STOCK,
                id: 'ignore_stock',
                hidden: true,
                colName: 'ignore_stock',
                inputValue: '1',
                readOnly: true
            }, {
                boxLabel: SHORTAGE_MORE,
                id: 'shortage',
                hidden: true,
                inputValue: '2',
                colName: 'shortage',
                readOnly: true
            }]
        }],
        listeners: {
            afterrender: function () {
                chekPanel.getForm().load({
                    type: 'ajax',
                    url: '/ProductCombo/QueryProduct',
                    actionMethods: 'post',
                    params: {
                        "ProductId": product_id
                    },
                    success: function (response, opts) {
                        var resText = eval("(" + opts.response.responseText + ")");
                        if (!resText) return;
                        if (resText.data.Ignore_Stock == 1) Ext.getCmp("ignore_stock").setValue(true); else Ext.getCmp("ignore_stock").setValue(false);
                        if (resText.data.Shortage == 1) {
                            Ext.getCmp("shortage").setValue(true);
                            //Ext.getCmp("shortstatus").setValue(true);
                        }
                        else {
                            Ext.getCmp("shortage").setValue(false);
                            //Ext.getCmp("shortstatus").setValue(false);
                        }
                    }
                });
            }
        }
    })

    var stockColumns = [{
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
        width: 280,
        menuDisabled: true,
        colName: 'product_name',
        dataIndex: 'product_name'
    }, {
        header: SPEC1,
        sortable: false,
        menuDisabled: true,
        width: 80,
        hidden: true,
        colName: 'txtSpec1Name',
        dataIndex: 'spec_name1'
    }, {
        header: SPEC2,
        sortable: false,
        menuDisabled: true,
        colName: 'txtSpec2Name',
        width: 80,
        hidden: true,
        dataIndex: 'spec_name2'
    }, {
        header: PRODUCT_STOCK,
        sortable: false,
        menuDisabled: true,
        colName: 'item_stock',
        width: 80,
        hidden: true,
        dataIndex: 'item_stock'
    }, {
        header: NOT_MATTER_STOCK,
        sortable: false,
        menuDisabled: true,
        colName: 'ignore_stock',
        width: 60,
        hidden: true,
        dataIndex: 'ignore_stock',
        renderer: function (val) {
            return "<input type='checkbox' disabled='disabled' " + (val == 1 ? 'checked' : '') + "/>";
        }
    }, {
        header: IGNORE_SHORTAGE,
        sortable: false,
        menuDisabled: true,
        colName: 'shortstatus',
        width: 60,
        hidden: true,
        dataIndex: 'shortstatus',
        renderer: function (val) {
            return "<input type='checkbox' disabled='disabled' " + (val == 1 ? 'checked' : '') + "/>";//補貨中停止販賣
        }
    }];


    //加載庫存信息
    if (combination == 2 || combination == 3) {
        var stockStore = Ext.create("Ext.data.Store", {
            fields: ['product_id', 'item_id', 'product_name', 'spec_name1', 'spec_name2', 'item_stock', 'ignore_stock', 'shortstatus'],
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
        stockStore.load({ params: { product_id: product_id, pile_id: 0} });
        var stockficxed = Ext.create("Ext.grid.Panel", {
            id: 'stockComcoFixed',
            store: stockStore,
            border: false,
            columns: stockColumns
        });

        stockPanel.add(stockficxed);
        //window.parent.setShow(stockPanel, 'colName');
    }
    if (combination == 4) {
        //查詢群組
        Ext.Ajax.request({
            url: '/ProductCombo/groupNameQuery',
            method: 'post',
            params: {
                "ProductId": product_id
            },
            success: function (response) {
                var resText = eval("(" + response.responseText + ")");
                var pileCount = resText.length;

                for (var i = 1; i <= pileCount; i++) {
                    /**********************Store*****************/
                    var stockName = 'pileStore_' + i;
                    stockName = Ext.create("Ext.data.Store", {
                        fields: ['product_id', 'item_id', 'product_name', 'spec_name1', 'spec_name2', 'item_stock', 'ignore_stock', 'shortstatus'],
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
                    stockName.load({ params: { product_id: product_id, pile_id: resText[i - 1].Pile_Id} });
                    var title = GROUP + i
                    /*****************Grid***************************/
                    var stockPileGrid = Ext.create("Ext.grid.Panel", {
                        title: title,
                        //height: 250,
                        width: 840,
                        id: '' + i + '_stockComcoFixed',
                        store: stockName,
                        border: true,
                        columns: stockColumns
                    });

                    stockPanel.add(stockPileGrid);

                    /*****************end******************************/
                }
            }
        });
    }
}

//單一商品庫存
function singleStockDetails(combination, product_id) {
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
                { name: "erp_id", type: "string" },
                { name: "barcode", type: "string" },
                { name: "remark", type: "string" }, //add by zhuoqin0830w 2015/04/07 增加備註
                { name: "arrive_days", type: "int" },// add by zhuoqin0830w 2015/04/07 增加運達天數
            ]
    });

    var sinStockStore = Ext.create("Ext.data.Store", {
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
    sinStockStore.load({ params: { 'product_id': product_id} });

    chekPanel = Ext.create("Ext.form.Panel", {
        height: 90,
        width: 800,
        border: false,
        items: [{
            xtype: 'fieldcontainer',
            defaultType: 'checkboxfield',
            items: [{
                boxLabel: IGNORE_STOCK,
                id: 'ignore_stock',
                readOnly: true,
                hidden: true,
                colName: 'ignore_stock',
                inputValue: '1'
            },
            {
                xtype: 'displayfield',
                fieldLabel: STOCK_DAYS,
                name: 't_outofstock_days_stopselling',//爲了解決重複問題
                id: 't_outofstock_days_stopselling',
                readOnly: true,
                value:0
            },
            {
                boxLabel: SHORTAGE,
                id: 'shortage',
                readOnly: true,
                hidden: true,
                colName: 'shortage',
                inputValue: '2'
            }]
        }],
        listeners: {
            afterrender: function () {
                Ext.Ajax.request({
                    url: '/Product/QueryProduct',
                    method: 'post',
                    params: {
                        "ProductId": product_id
                    },
                    success: function (response) {
                        var reStr = eval("(" + response.responseText + ")");
                        if (reStr && reStr.data) {
                            if (reStr.data.Ignore_Stock == 1) Ext.getCmp("ignore_stock").setValue(true); else Ext.getCmp("ignore_stock").setValue(false); 
                            if (reStr.data.Shortage == 1) Ext.getCmp("shortage").setValue(true); else Ext.getCmp("shortage").setValue(false);
                            if (reStr.data.outofstock_days_stopselling>0)
                            {
                                Ext.getCmp("t_outofstock_days_stopselling").setValue(reStr.data.outofstock_days_stopselling);
                            }
                        }
                    }   
                });
            }
        }
    })

    stockPanel = Ext.create("Ext.grid.Panel", {
        //id: 'stockGrid',
        store: sinStockStore,
        border: false,
        height: 400,
        width: 960,
        columns: [{ xtype: 'rownumberer', width: 50, align: 'center' }, {
            header: ITEM_ID,
            dataIndex: 'item_id',
            sortable: false,
            menuDisabled: true,
            hidden: true,
            align: 'center',
            colName: 'item_id',
            width: 80
        }, {
            header: SPEC1,
            sortable: false,
            menuDisabled: true,
            id: 'spec_title_1',
            hidden: true,
            colName: 'txtSpec1Name',
            dataIndex: 'spec_title_1',
            width: 100
        }, {
            dataIndex: 'spec_id_1',
            hidden: true
        }, {
            header: SPEC2,
            dataIndex: 'spec_title_2',
            sortable: false,
            menuDisabled: true,
            width: 100,
            hidden: true,
            colName: 'txtSpec2Name',
            id: 'spec_title_2'
        }, {
            dataIndex: 'spec_id_2',
            hidden: true
        }, {
            header: PRODUCT_STOCK,
            sortable: false,
            menuDisabled: true,
            width: 100,
            dataIndex: 'item_stock',
            hidden: true,
            colName: 'item_stock',
            id: 'item_stock'
        }, {
            header: ITEM_ALARM,
            sortable: false,
            menuDisabled: true,
            width: 100,
            dataIndex: 'item_alarm',
            hidden: true,
            colName: 'item_alarm',
            id: 'item_alarm'
        },
        {//add by xiangwang0413w 2014/06/26 增加 ERP廠商編號（erp_id)
            header: ERP_ID,
            sortable: false,
            menuDisabled: true,
            width: 100,
            dataIndex: 'erp_id',
            hidden: false,
            colName: 'erp_id',
            id: 'erp_id'
        },{// add by zhuoqin0830w 2014/04/07 增加備註
            header: REMARK,
            dataIndex: 'remark',
            colName: 'remark',
            sortable: false,
            menuDisabled: true,
            width: 150,
            hidden: false,
            id: 'remark'
        },
        {// add by zhuoqin0830w 2014/04/07 增加運達天數
            header: TRANSPORT_ARRIVE_DAY,
            dataIndex: 'arrive_days',
            colName: 'arrive_days',
            sortable: false,
            menuDisabled: true,
            width: 60,
            hidden: false,
            id: 'arrive_days'
        },
        {
            header: BARCODE,
            sortable: false,
            menuDisabled: true,
            width: 200,
            dataIndex: 'barcode',
            hidden: true,
            colName: 'barcode',
            id: 'barcode'
        }]
    });
}