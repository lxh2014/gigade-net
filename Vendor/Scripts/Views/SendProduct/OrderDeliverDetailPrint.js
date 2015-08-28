PintFunction = function (id) {
    //Ext.define('GIGADE.DeliverDetail', {
    //    extend: 'Ext.data.Model',
    //    fields: [
    //        { name: "Detail_Id", type: "string" }, //購物編號
    //        { name: "Product_Name", type: "string" },//子商品名稱
    //        { name: "Product_Freight_Set", type: "string" }, //托運單屬性
    //        { name: "parent_name", type: "string" },//母商品名稱
    //        { name: "Buy_Num", type: "int" },//購買數量
    //        { name: "Combined_Mode", type: "int" },//
    //        { name: "item_mode", type: "int" }
    //    ]
    //});
    //var NormalDeliverDetailStore = Ext.create('Ext.data.Store', {
    //    model: 'GIGADE.DeliverDetail',
    //    proxy: {
    //        type: 'ajax',
    //        //   url: '/SendProduct/DeliveryInformation?rowIDs=' + id,
    //        actionMethods: 'post',
    //        reader: {
    //            type: 'json',
    //            totalProperty: 'totalCount',
    //            root: 'data',
    //            idProperty: 'item_id'
    //        }
    //    }
    //});

    //var sm = Ext.create('Ext.selection.CheckboxModel', {
    //    listeners: {
    //        selectionchange: function (sm, selections) {
    //            Ext.getCmp("gdOrderDetail").down('#Export1').setDisabled(selections.length == 0)
    //            Ext.getCmp("gdOrderDetail").down('#Export2').setDisabled(selections.length == 0)
    //            var model = Ext.getCmp("gdOrderDetail").getSelectionModel();
    //            var row = Ext.getCmp("gdOrderDetail").getSelectionModel().getSelection();
    //            if (row.length > 1) {
    //                for (i = 0; i < row.length; i++) {
    //                    var start = row[0].data.delivery_store;
    //                }
    //            }
    //        }
    //    }
    //});

    Ext.onReady(function () {
        //頁面加載時創建grid
        //var gdOrderDetail = Ext.create('Ext.grid.Panel', {
        //    id: 'gdOrderDetail',
        //    store: NormalDeliverDetailStore,
        //    border: false,
        //    columnLines: true,
        //    height: 245,
        //    verticalScroller: {
        //        trailingBufferZone: 200,  // 保持200条记录到内存缓冲中，在向后滚动时
        //        leadingBufferZone: 5000   // 保持5000条记录到内存缓冲中，在向前滚动时
        //    },
        //    loadMask: true,
        //    margin: '0 0 0 0',
        //    width: document.documentElement.clientWidth,
        //    columns: [
        //        { header: '購物編號', dataIndex: 'Detail_Id', align: 'center', width: 80 },
        //        {
        //            header: '商品名稱', dataIndex: 'Product_Name', width: 200, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
        //                if (record.data.Combined_Mode >= 1 && record.data.item_mode == 2) {
        //                    return "組合商品 : 【" + record.data.parent_name + "】— " + record.data.Product_Name;
        //                } else {
        //                    return record.data.Product_Name;
        //                }
        //            }
        //        },
        //        { header: '托運單屬性', dataIndex: 'Product_Freight_Set', align: 'center', width: 90 },
        //        { header: '數量', dataIndex: 'Buy_Num', align: 'center', width: 50 }

        //    ],
        //    selModel: sm,
        //    viewConfig: {
        //        forceFit: true
        //    }
        //});

        var frmPrint = Ext.create('Ext.form.Panel', {
            id: 'frmPrint',
            margin: '5 0 0 0',
            layout: 'anchor',
            border: false,
            items: [
                //gdOrderDetail
                {
                    xtype: 'button',
                    id:'btnPrint',
                    text: '列印',
                    handler: Query
                    //handler: function () {

                    //}
                },
                {
                    xtype: 'panel',
                    id: 'pnlPrint',
                    width: 750,
                    height: 600,
                    autoScroll: true,
                    html: '載入中...'
                }
            ]
        });

        //變更收件人資料
        //var ChangeCustomerInfo = Ext.create('Ext.form.Panel', {
        //    layout: 'anchor',
        //    renderTo: Ext.getBody(),
        //    url: '/SendProduct/DeliverDetailEdit',
        //    border: false,
        //    plain: true,
        //    width: 444,
        //    HEIGHT: 250,
        //    margin: '10 5 10 0',
        //    id: 'ChangeCustomerInfo',
        //    items: [
        //        {
        //            xtype: 'fieldset',
        //            defaultType: 'textfield',
        //            width: 444,
        //            height: 240,
        //            layout: 'anchor',
        //            items: [
        //                {
        //                    xtype: 'displayfield',
        //                    id: 'order_code',
        //                    fieldLabel: '付款單號',
        //                    value: id
        //                },
        //                {
        //                    xtype: 'combobox',
        //                    fieldLabel: '物流业者',
        //                    store: DeliverStore,
        //                    id: 'DeliverStores',
        //                    displayField: 'parameterName',
        //                    valueField: 'parameterCode',
        //                    labelWidth: 100,
        //                    allowBlank: false,
        //                    editable: false,
        //                    emptyText: '請選擇...',
        //                    width: 250
        //                },
        //                {
        //                    xtype: "textfield",
        //                    fieldLabel: '物流单号',
        //                    id: 'delivery_Code',
        //                    name: 'delivery_Code',
        //                    labelWidth: 100,
        //                    allowBlank: false,
        //                    width: 250
        //                },
        //                {
        //                    xtype: 'combo',
        //                    fieldLabel: '出货时间',
        //                    store: sendProTStore,
        //                    id: 'sendProTime',
        //                    queryMode: 'local',
        //                    name: 'sendProTime',
        //                    displayField: 'day',
        //                    valueField: 'time',
        //                    labelWidth: 100,
        //                    width: 250,
        //                    value: Today(),
        //                    editable: false,
        //                    listeners: {
        //                        beforerender: function () {
        //                            sendProTStore.load({
        //                                callback: function () {
        //                                    sendProTStore.insert(0, { time: '0', day: '請選擇' });
        //                                }
        //                            });
        //                        }
        //                    }
        //                },
        //                {
        //                    xtype: "textarea",
        //                    fieldLabel: '出货单备注(※100個字以內)',
        //                    id: 'delivery_note',
        //                    name: 'delivery_note',
        //                    labelWidth: 100,
        //                    //allowBlank: false,
        //                    maxLength: 100,
        //                    maxLengthText: '最多輸入100個中文字',
        //                    width: 250
        //                },

        //                {
        //                    xtype: 'button',
        //                    text: '確認出貨',
        //                    id: 'saveinfo',

        //                    formBind: true,
        //                    disabled: true,
        //                    handler: ConfirmShipment
        //                }
        //            ]
        //        }
        //    ]
        //});

        var deliverDetailPrintWin = Ext.create('Ext.window.Window', {
            title: "列印定單明細",
            id: 'deliverDetailPrintWin',
            iconCls: 'icon-user-edit',
            width: 800,
            height:680,
            layout: 'anchor',
            items: [
                frmPrint
                //,
                //ChangeCustomerInfo
            ],
            closeAction: 'destroy',
            modal: true,
          // resizable: false,
            constrain: true,
            labelWidth: 60,
            //bodyStyle: 'padding:5px 5px 5px 5px',
            closable: false,
            tools: [
                {
                    type: 'close',
                    qtip: "關閉",
                    handler: function (event, toolEl, panel) {
                        Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                            if (btn == "yes") {
                                Ext.getCmp('deliverDetailPrintWin').destroy();
                            } else {
                                return false;
                            }
                        });
                    }
                }
            ],
            listeners: {
                'show': function () {
                    LoadData(id);
                }
            }
        });
        deliverDetailPrintWin.show();
    });
}

function LoadData(order_id) {
    Ext.Ajax.request({
        url: "/SendProduct/GetOrderDeliverDetail",
        method: 'post',
        params: {
            oid: order_id,
            p_mode: 1
        },
        success: function (response) {
            var result = Ext.JSON.decode(response.responseText);
            if (result.success) {
                Ext.getCmp("pnlPrint").update("<form>"+result.msg+"</form>")
            } else {
                alert('加載出錯');
            }
        },
        failure: function () {
            Ext.Msg.alert('系統錯誤！');
        }
    });
}

//function ConfirmShipment(Deliver_Id, Detail_Id) {
//    var form = this.up('form').getForm();
//    var rowID = document.getElementById("rowID").value;
//    if (judge()) {
//        if (form.isValid) {
//            Ext.getBody().mask("请稍等......", "x-mask-loading");
//            Ext.Ajax.request({
//                url: "/SendProduct/ConfirmShipment",
//                method: 'post',
//                params: {
//                    delivery_Code: Ext.getCmp('delivery_Code').getValue(),//物流單號
//                    DeliverStores: Ext.getCmp('DeliverStores').getValue(),//物流業者
//                    sendProTime: Ext.getCmp('sendProTime').getValue(),//出貨時間
//                    normal_Subtotal: Ext.getCmp('normal_Subtotal').getValue(),
//                    //    freight_Normal:Ext.getCmp('freight_Normal').getValue(),
//                    hypothermia_Subtotal: Ext.getCmp('hypothermia_Subtotal').getValue(),
//                    //   freight_Low:Ext.getCmp('freight_Low').getValue(),
//                    delivery_note: Ext.getCmp('delivery_note').getValue(),//出貨單備註
//                    order_Freight_Normal: Ext.getCmp('Order_Freight_Normal').getValue(),
//                    order_Freight_Low: Ext.getCmp('Order_Freight_Low').getValue(),
//                    code: Ext.getCmp('order_code').getValue(),
//                    rowIDs: rowID
//                },
//                success: function (form, action) {
//                    var result = Ext.decode(form.responseText);
//                    if (result.success == true) {
//                        Ext.getBody().unmask();
//                        //if (confirm("出貨成功!是否進行頁面跳轉？")) {
//                        //    window.location.href = "AllOrderWaitDeliver";
//                        //} else {
//                        //    window.location.reload();
//                        //}
//                        Ext.Msg.confirm('系统提示', '出貨成功!是否進行頁面跳轉？', function (btn) {
//                            if (btn == 'yes') {
//                                window.location.href = "AllOrderWaitDeliver";
//                            } else {
//                                window.location.reload();
//                            }
//                        });
//                    } else {
//                        Ext.getBody().unmask();
//                        Ext.Msg.alert("提示", "数据出现异常");
//                    }
//                },
//                error: function (sucess) {
//                    Ext.getBody().unmask();
//                    Ext.Msg.alert(INFORMATION, FAILURE);
//                    Ext.ux.Toast.msg("操作信息", "請求超時,请稍後在進行操作!");
//                }
//            });
//        }
//    }
//}

//function judge() {
//    if (Ext.getCmp('DeliverStores').getValue() == "0") {
//        Ext.Msg.alert("提示", "請選擇物流業者");
//        return false;
//    }
//    if (Ext.getCmp('sendProTime').getValue() == 0) {
//        Ext.Msg.alert("提示", "請選擇出貨時間");
//        return false;
//    }
//    var nums = NormalDeliverDetailStore.getCount();
//    //var asds = gdOrderDetail.getStore().getCount();

//    if (nums == 0) {
//        Ext.Msg.alert("提示", "此出货单下没有要出货的商品");
//        return false;
//    }
//    return true;
//}
function Query() {
    bdhtml =  window.document.body.innerHTML;// Ext.getCmp("pnlPrint").htmljosn;//htmljosn;////
    startprint = ("<form>");
    endprint = ("</form>");
    printhtml = bdhtml.substr(bdhtml.indexOf(startprint));//39
    printhtml = printhtml.substr(0, printhtml.indexOf(endprint));
    window.document.body.innerHTML = printhtml;
    window.print();
    // javascript: print();
    window.location.reload();
}