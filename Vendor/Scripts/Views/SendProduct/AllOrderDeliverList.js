Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector',
    'Ext.selection.CheckboxModel'
]);

Ext.define('GIGADE.DeliverDetail', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "Slave_Id", type: "string" }, //廠商出貨編號
        { name: "Detail_Id", type: "string" },//訂單編號
        { name: "item_id", type: "string" }, //細項編號
        { name: "Product_Name", type: "string" },//商品名稱
      //  { name: "Single_Price", type: "string" },//商品單價
        { name: "Buy_Num", type: "int" },//購買數量
        { name: "Single_Money", type: "string" },//商品單價
        { name: "parent_num", type: "int" },//
        { name: "parent_name", type: "string" },//組合商品名稱
        { name: "item_mode", type: "int" }
    ]
});
var NormalDeliverDetailStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.DeliverDetail',
    proxy: {
        type: 'ajax',
        //   url: '/SendProduct/AllOrderDeliver?rowIDs=' + document.getElementById("rowID").value,
        actionMethods: 'post',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
            root: 'data',
            idProperty: 'item_id'
        }
    }
});
var myMask;
Ext.onReady(function () {
    //頁面加載時創建grid
    var gdOrderDetail = Ext.create('Ext.grid.Panel', {
        id: 'gdOrderDetail',
        store: NormalDeliverDetailStore,
        border: false,
        columnLines: true,
        height: 245,
        verticalScroller: {
            trailingBufferZone: 200,  // 保持200条记录到内存缓冲中，在向后滚动时
            leadingBufferZone: 5000   // 保持5000条记录到内存缓冲中，在向前滚动时
        },
        loadMask: true,
        margin: '0 0 0 10',
        width: document.documentElement.clientWidth,
        columns: [
            { header: '廠商出貨編號', dataIndex: 'Slave_Id', align: 'center' },
            { header: '訂單編號', dataIndex: 'Detail_Id', align: 'center' },
            { header: '商品名稱', dataIndex: 'Product_Name', width: 200, align: 'center' },
           // { header: '單價', dataIndex: 'Single_Price', align: 'center' },
            { header: '數量', dataIndex: 'Buy_Num', align: 'center' },
            { header: '總價', dataIndex: 'Single_Price', align: 'center' },
            {
                header: '備註', dataIndex: 'item_mode', align: 'center', width: 320,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.item_mode == 2) {
                        return "組合名稱 : " + record.data.parent_name;
                    } else {
                        return "";
                    }
                }
            }
        ],
        viewConfig: {
            forceFit: true
        }
    });

    myMask = new Ext.LoadMask(Ext.getBody(), { msg: "loading...", store: NormalDeliverDetailStore });
    myMask.show();


    var orderCode = Ext.create('Ext.form.Panel', {
        id: 'orderCode',
        border: false,
        items: [
            {
                xtype: 'displayfield',
                id: 'order_code',
                margin: '20,0,0,0',
                fieldLabel: '批次出貨單號',
                value: document.getElementById("picitime").value
            }
        ]
    });
    // orderCode.render('centerPanel');
    //常溫
    var normal = Ext.create("Ext.form.Panel", {
        layout: 'anchor',
        border: false,
        id: 'normal',
        items: [
            {
                xtype: 'fieldset',
                width: 450,
                margin: '20 0 0 20',
                items: [
                    {
                        html: '<div class="title">常溫</div>',
                        frame: false,
                        margin: '0 0 3 0',
                        border: false
                    },
                    {
                        xtype: 'fieldcontainer',
                        layout: 'hbox',
                        width: 200,
                        items: [
                            {
                                xtype: "displayfield",
                                fieldLabel: '商品總額',
                                labelWidth: 100
                                //    value: 1
                            },
                            {
                                xtype: "displayfield",
                                id: 'normal_Subtotal',
                                name: 'normal_Subtotal',
                                width: 100
                            }
                        ]
                    },
                    {
                        xtype: 'fieldcontainer',
                        layout: 'hbox',
                        width: 200,
                        items: [
                            {
                                xtype: "displayfield",
                                fieldLabel: '補貼運費',
                                labelWidth: 100
                                //  value: 1
                            },
                            {
                                xtype: "displayfield",
                                id: 'freight_Normal',
                                name: 'freight_Normal',
                                width: 100
                            },
                            {
                                xtype: "displayfield",
                                id: 'Order_Freight_Normal',
                                name: 'Order_Freight_Normal',
                                hidden: true,
                                width: 100
                            }
                        ]
                    }
                ]
            }
        ]
    });
    //低溫
    var low = Ext.create("Ext.form.Panel", {
        layout: 'anchor',
        border: false,
        id: 'low',
        items: [
            {
                xtype: 'fieldset',
                width: 450,
                margin: '20 0 0 20',
                items: [
                    {
                        html: '<div class="title">低溫</div>',
                        frame: false,
                        margin: '0 0 3 0',
                        border: false
                    },
                    {
                        xtype: 'fieldcontainer',
                        layout: 'hbox',
                        width: 200,
                        items: [
                            {
                                xtype: "displayfield",
                                fieldLabel: '商品總額',
                                labelWidth: 100
                                //value: 1
                            },
                            {
                                xtype: "displayfield",
                                id: 'hypothermia_Subtotal',
                                name: 'hypothermia_Subtotal',
                                width: 100
                            }
                        ]
                    },
                    {
                        xtype: 'fieldcontainer',
                        layout: 'hbox',
                        width: 200,
                        items: [
                            {
                                xtype: "displayfield",
                                fieldLabel: '補貼運費',
                                labelWidth: 100
                                // value: 1
                            },
                            {
                                xtype: "displayfield",
                                id: 'freight_Low',
                                name: 'freight_Low',
                                width: 100
                            },
                            {
                                xtype: "displayfield",
                                id: 'Order_Freight_Low',
                                name: 'Order_Freight_Low',
                                hidden: true,
                                width: 100
                            }
                        ]
                    }
                ]
            }
        ],
        listeners: {
            'afterrender': function () {
            }
        }
    });

    //變更收件人資料
    var ChangeCustomerInfo = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        renderTo: Ext.getBody(),
        url: '/SendProduct/DeliverDetailEdit',
        border: false,
        plain: true,
        margin: '20 0 0 20',
        id: 'ChangeCustomerInfo',
        items: [
            {
                xtype: 'fieldset',
                defaultType: 'textfield',
                width: 450,
                layout: 'anchor',
                items: [
                    {
                        xtype: 'combobox',
                        fieldLabel: '物流业者',
                        store: DeliverStore,
                        id: 'DeliverStores',
                        displayField: 'parameterName',
                        valueField: 'parameterCode',
                        labelWidth: 100,
                        allowBlank: false,
                        editable: false,
                        emptyText: '請選擇...',
                        width: 250
                        //,
                        //listeners: {
                        //    beforerender: function () {
                        //        DeliverStore.load({
                        //            callback: function () {
                        //                DeliverStore.insert(0, { parameterCode: '0', parameterName: '請選擇' });
                        //                //Ext.getCmp("DeliverStores").setValue(DeliverStore.data.items[0].data.parameterCode);
                        //            }
                        //        });
                        //    }
                        //}      
                    },
                    {
                        xtype: "textfield",
                        fieldLabel: '物流单号',
                        id: 'delivery_Code',
                        name: 'delivery_Code',
                        labelWidth: 100,
                        allowBlank: false,
                        width: 250
                    },
                    {
                        xtype: 'combo',
                        fieldLabel: '出货时间',
                        store: sendProTStore,
                        id: 'sendProTime',
                        queryMode: 'local',
                        name: 'sendProTime',
                        displayField: 'day',
                        valueField: 'time',
                        labelWidth: 100,
                        width: 250,
                        value: Today(),
                        editable: false,
                        listeners: {
                            beforerender: function () {
                                sendProTStore.load({
                                    callback: function () {
                                        sendProTStore.insert(0, { time: '0', day: '請選擇' });
                                    }
                                });
                            }
                        }
                    },
                    {
                        xtype: "textarea",
                        fieldLabel: '出货单备注(※100個字以內)',
                        id: 'delivery_note',
                        name: 'delivery_note',
                        labelWidth: 100,
                        //allowBlank: false,
                        maxLength: 100,
                        maxLengthText: '最多輸入100個中文字',
                        width: 250
                    },
                    {
                        xtype: 'button',
                        text: '確認出貨',
                        id: 'saveinfo',
                        formBind: true,
                        disabled: true,
                        handler: ConfirmShipment
                    }
                ]
            }
        ]
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'anchor',
        items: [orderCode, gdOrderDetail, normal, low, ChangeCustomerInfo],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                this.doLayout();
            },
            afterrender: function () {
                LoadData();
                //myMask.hide();
            }
        }
    });
    ToolAuthority();
});

function LoadData() {
    Ext.Ajax.request({
        url: "/SendProduct/AllOrderDeliver",
        method: 'post',
        params: {
            rowIDs: document.getElementById("rowID").value
        },
        success: function (response) {
            var result = Ext.JSON.decode(response.responseText);
            //var re = Ext.JSON.decode(result);
            var data = result.data;
            if (result.success) {
                NormalDeliverDetailStore.loadData(data);

                Ext.getCmp('normal_Subtotal').setValue(result.Normal_Subtotal);
                Ext.getCmp('freight_Normal').setValue(result.All_Order_Freight_Normal);
                Ext.getCmp('hypothermia_Subtotal').setValue(result.Hypothermia_Subtotal);
                Ext.getCmp('freight_Low').setValue(result.All_Order_Freight_Low);
                Ext.getCmp('Order_Freight_Low').setValue(result.Order_Freight_Low);
                Ext.getCmp('Order_Freight_Normal').setValue(result.Order_Freight_Normal);

                myMask.hide();
            } else {
                alert('加載出錯');
            }
        },
        failure: function () {
            Ext.Msg.alert('系統錯誤！');
        }
    });
}

function ConfirmShipment(Deliver_Id, Detail_Id) {
    var form = this.up('form').getForm();
    var rowID = document.getElementById("rowID").value;
    if (judge()) {
        if (form.isValid) {
            Ext.getBody().mask("请稍等......", "x-mask-loading");
            Ext.Ajax.request({
                url: "/SendProduct/DispatchConfirmShipment",
                method: 'post',
              
                params: {
                    delivery_Code: Ext.getCmp('delivery_Code').getValue(),//物流單號
                    DeliverStores: Ext.getCmp('DeliverStores').getValue(),//物流業者
                    sendProTime: Ext.getCmp('sendProTime').getValue(),//出貨時間
                    normal_Subtotal: Ext.getCmp('normal_Subtotal').getValue(),
                    //    freight_Normal:Ext.getCmp('freight_Normal').getValue(),
                    hypothermia_Subtotal: Ext.getCmp('hypothermia_Subtotal').getValue(),
                    //   freight_Low:Ext.getCmp('freight_Low').getValue(),
                    delivery_note: Ext.getCmp('delivery_note').getValue(),//出貨單備註
                    order_Freight_Normal: Ext.getCmp('Order_Freight_Normal').getValue(),
                    order_Freight_Low: Ext.getCmp('Order_Freight_Low').getValue(),
                    code: Ext.getCmp('order_code').getValue(),
                    rowIDs: rowID
                },
                success: function (form, action) {
                    var result = Ext.decode(form.responseText);
                    if (result.success == true) {
                        Ext.getBody().unmask();
                        //if (confirm("出貨成功!是否進行頁面跳轉？")) {
                        //    window.location.href = "AllOrderWaitDeliver";
                        //} else {
                        //    window.location.reload();
                        //}
                        Ext.Msg.confirm('系统提示', '出貨成功!是否進行頁面跳轉？', function (btn) {
                            if (btn == 'yes') {
                                window.location.href = "AllOrderWaitDeliver";
                            } else {
                                window.location.reload();
                            }
                        });
                    } else {
                        Ext.getBody().unmask();
                        Ext.Msg.alert("提示", "数据出现异常");
                    }
                },
                error: function (sucess) {
                    Ext.getBody().unmask();
                    Ext.Msg.alert(INFORMATION, FAILURE);
                    Ext.ux.Toast.msg("操作信息", "請求超時,请稍後在進行操作!");
                }

            });
        }
    }
}

function judge() {
    if (Ext.getCmp('DeliverStores').getValue() == "0") {
        Ext.Msg.alert("提示", "請選擇物流業者");
        return false;
    }
    if (Ext.getCmp('sendProTime').getValue() == 0) {
        Ext.Msg.alert("提示", "請選擇出貨時間");
        return false;
    }
    var nums = NormalDeliverDetailStore.getCount();
    //var asds = gdOrderDetail.getStore().getCount();

    if (nums == 0) {
        Ext.Msg.alert("提示", "此出货单下没有要出货的商品");
        return false;
    }
    return true;
}