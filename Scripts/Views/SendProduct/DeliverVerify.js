//出貨確認頁面
//出貨確認Model
Ext.define('gigade.Stock', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'item_id', type: 'int' },
        { name: 'product_name', type: 'string' },
        { name: 'buy_num', type: 'int' },
        { name: 'status', type: 'string' }//狀態
    ]
});
//出貨確認Store
var StockStore = Ext.create('Ext.data.Store', {
    pageSize: 25,
    model: 'gigade.Stock',
    proxy: {
        type: 'ajax',
        url: '/SendProduct/DeliverVerifyList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
//庫存信息載入之前添加查詢條件
StockStore.on('beforeload', function () {
    Ext.apply(StockStore.proxy.extraParams, {
        deliver_id: Ext.getCmp("deliver_id").getValue(),
        order_id: Ext.getCmp("ord_id").getValue()
    });
});
//庫存信息grid
var gdStock = Ext.create('Ext.grid.Panel', {
    id: 'gdStock',
    title: '出貨信息',
    store: StockStore,
    columnLines: true,
    width: 500,
    height: 300,
    columnLines: true,
    frame: true,
    hidden: true,
    columns: [
        { header: '產品細項編號', dataIndex: 'item_id', width: 100, align: 'center' },
        { header: '產品名稱', dataIndex: 'product_name', width: 200, align: 'center' },
        { header: '數量', dataIndex: 'buy_num', width: 80, align: 'center' },
        { header: '狀態', dataIndex: 'status', width: 100, align: 'center' }
    ],
    selType: 'cellmodel',
    plugins: [
        Ext.create('Ext.grid.plugin.CellEditing', {
            clicksToEdit: 1
        })
    ],
    viewConfig: {
        emptyText: '<span>暫無數據!</span>'
    },
    listeners: {
        scrollershow: function (scroller) {
            if (scroller && scroller.scrollEl) {
                scroller.clearManageListeners();
                scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
            }
        }
    }
});

Ext.onReady(function () {
    var addTab = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        width: 900,
        //url: '/SendProduct/GETMarkTallyWD',
        defaults: {
            labelWidth: 100,
            width: 250,
            margin: '5 0 0 10'
        },
        border: false,
        plain: true,
        id: 'AddUserPForm',
        items: [
            {
                xtype: 'textfield',
                fieldLabel: '出貨單號',
                id: 'deliver_id',
                name: 'deliver_id'
            },          
            {
                xtype: 'textfield',
                fieldLabel: '訂單編號',
                id: 'ord_id',
                name: 'ord_id',
                allowBlank: false,
                listeners: {
                    blur: function () {
                        var deliver = Ext.getCmp("deliver_id").getValue();
                        if (deliver.length == 9) {
                            Ext.Ajax.request({
                                url: "/SendProduct/JudgeOrdid",
                                method: 'post',
                                type: 'text',
                                async: false,
                                params: {
                                    deliver_id: Ext.getCmp("deliver_id").getValue(),
                                    order_id: Ext.getCmp("ord_id").getValue()
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    if (result.success) {
                                        if (result.msg == 0 )
                                        {
                                            Ext.getCmp("ord").show();
                                            Ext.getCmp("delivery").show();
                                            //Ext.getCmp("btn").show();dis_DeliverList
                                            Ext.getCmp("dis_DeliverList").show();
                                            Ext.getCmp("dis_DeliverList").setValue("<a href=javascript:TranToDeliverList('/SendProduct/DeliversSearchList','" + Ext.getCmp("ord_id").getValue() + "')>出貨查詢</a>");
                                            Ext.getCmp("ord").setValue(Ext.getCmp("ord_id").getValue());
                                            var delivery_store_name = result.data[0].deliver_id + result.data[0].delivery_store_name;
                                            Ext.getCmp("delivery").setValue(delivery_store_name);
                                            //載入庫存信息
                                            Ext.getCmp("gdStock").show();
                                            Ext.getCmp('gdStock').store.loadPage(1, {
                                                params: {
                                                    deliver_id: result.data[0].deliver_id,
                                                    order_id: Ext.getCmp("ord_id").getValue()
                                                }
                                            });
                                        } else if (result.msg == 1) {
                                            Ext.Msg.alert(INFORMATION, "訂單錯誤!");
                                            Ext.getCmp("ord").hide();
                                            Ext.getCmp("delivery").hide();
                                            Ext.getCmp("dis_DeliverList").hide();
                                            Ext.getCmp("gdStock").hide();
                                        } else if (result.msg == 2) {
                                            Ext.Msg.alert(INFORMATION, "該商品已出貨!");
                                            Ext.getCmp("ord").hide();
                                            Ext.getCmp("delivery").hide();
                                            Ext.getCmp("dis_DeliverList").hide();
                                            Ext.getCmp("gdStock").hide();
                                        } else if (result.msg == 3){                                        
                                            Ext.Msg.alert(INFORMATION, "出貨單號格式錯誤,例:D000XXXXX!");
                                            Ext.getCmp("deliver_id").setValue(null);
                                            Ext.getCmp("ord_id").setValue(null);
                                        } else if (result.msg == 4) {
                                            Ext.Msg.alert(INFORMATION, "錯誤物流商!");
                                            Ext.getCmp("deliver_id").setValue(null);
                                            Ext.getCmp("ord_id").setValue(null);
                                        } else if (result.msg == 5) {
                                            delivery_store_name = result.data[0].order_id +"("+ result.data[0].deliver_id+")";
                                            Ext.getCmp("delivery").setValue(delivery_store_name);
                                            Ext.getCmp("gdStock").hide();
                                            Ext.getCmp("dis_DeliverList").hide();
                                        }
                                        else
                                        {
                                            Ext.getCmp("deliver_id").setValue(null);
                                            Ext.getCmp("ord_id").setValue(null);

                                        }
                                    } else {
                                        Ext.getCmp("deliver_id").setValue(null);
                                        Ext.getCmp("ord_id").setValue(null);
                                        Ext.Msg.alert(INFORMATION, "代碼錯誤!");                                        
                                    }
                                },
                                failure: function (form, action) {
                                    Ext.getCmp("deliver_id").setValue(null);
                                    Ext.getCmp("ord_id").setValue(null);
                                    Ext.Msg.alert(INFORMATION, "系統錯誤!");
                                }
                            });                            
                        } else {
                            //if (result.delivery_store != 1 && result.delivery_store != 10) {
                            //} else {
                            //Ext.getCmp("deliver_id").setValue(null);
                            //Ext.getCmp("ord_id").setValue(null);
                            //Ext.Msg.alert(INFORMATION, "錯誤物流商!");
                            //}
                            Ext.getCmp("deliver_id").setValue(null);
                            Ext.getCmp("ord_id").setValue(null);
                            Ext.getCmp("ord").hide();
                            Ext.getCmp("delivery").hide();
                            Ext.getCmp("gdStock").hide();
                            //Ext.getCmp("btn").hide();
                            Ext.getCmp("dis_DeliverList").hide();
                        }
                    }
                }
            },
            {
                xtype: 'displayfield',
                fieldLabel: '訂單編號',
                id: 'ord',
                name: 'ord',
                hidden: true
            },
            {
                xtype: 'displayfield',
                fieldLabel: '出貨編號',
                id: 'delivery',
                name: 'delivery',
                hidden: true
            },
            //{
            //    xtype: 'button',
            //    id: 'btn',
            //    text: '出貨查詢',
            //    hidden: true,                
            //    listeners: {
            //        click: function () {
            //            document.location.href = "/SendProduct/DeliversSearchList?ord_id=" + Ext.getCmp("ord_id").getValue();
            //        }
            //    }
            //},
            {
                xtype: 'displayfield',
                id: 'dis_DeliverList',
                hidden: true//,
                //value: '<a href="/SendProduct/DeliverList">出貨查詢</a>'
            },
            gdStock
        ],
        buttonAlign: 'left'
    });
    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: addTab,
        autoScroll: true
    });  
});

//頁面跳轉到
function TranToDeliverList(url, ord_id) {
    var urlTran = url + '?delivery_type=1&delivery_status=-1&search=' + ord_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copylist = panel.down('#list');
    if (copylist) {
        copylist.close();
    }
    copylist = panel.add({
        id: 'list',
        title: "出貨查詢",
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copylist);
    panel.doLayout();
}