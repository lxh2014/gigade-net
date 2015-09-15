Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector',
    'Ext.selection.CheckboxModel'
]);
DeliverStore.load();
//臨時設定的出貨編號
var deliver_id = document.getElementById("deliver_id").value;
Ext.define('GIGADE.DeliverDetail', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "item_id", type: "string" }, //細項編號
        { name: "detail_id", type: "string" },
        { name: "deliver_id", type: "string" },
        { name: "product_name", type: "string" },
        { name: "product_spec_name", type: "string" },
        { name: "buy_num", type: "int" },
        { name: "parent_num", type: "int" },
        { name: "parent_name", type: "string" },
        { name: "detail_status", type: "string" },
        { name: "sdetail_status", type: "string" },
        { name: "product_mode", type: "string" },//order_detail表中的product_mode
        { name: "prod_mode", type: "string" },//product表中的product_mode
        { name: "combined_mode", type: "int" },
        { name: "item_mode", type: "int" },
        { name: "delivery_status", type: "string" }
    ]
});
//var url = '/SendProduct/DeliversView?deliver_id=' + document.getElementById("deliver_id").value;
//獲取grid中的數據
var NormalDeliverDetailStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.DeliverDetail',
    proxy: {
        type: 'ajax',
        //url: '/SendProduct/GetDeliverDetail?deliver_id=' + deliver_id+'&con=1',
        actionMethods: 'post',
        reader: {
            type: 'json',
            //totalProperty:10,
            idProperty: 'item_id',
            root: 'normaldata'
        }
    }
});
var CancelDeliverDetailStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.DeliverDetail',
    proxy: {
        type: 'ajax',
        // url: '/SendProduct/GetDeliverDetail?deliver_id=' + deliver_id+'&con=2',
        actionMethods: 'post',
        reader: {
            type: 'json',
            //totalProperty:10,
            idProperty: 'item_id',
            root: 'canceldata'
        }
    }
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    renderer: function (value, metaData, record, rowIndex, colIndex, store, view) {
        if (record.data.delivery_status == "0") {
            return '<div class="' + Ext.baseCSSPrefix + 'grid-row-checker">&#160;</div>';
        } else {
            return ' ';
        }
    },
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("nextdeliver").setDisabled(selections.length == 0);

        }
    }

});

Ext.onReady(function () {
    var titleForm = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        renderTo: Ext.getBody(),
        id: 'titleForm',
        border: false,
        width: 800,
        height: 80,
        plain: true,
        bodyPadding: 20,
        //defaults: { anchor: "95%", msgTarget: "side" },
        items: [
             {
                 html: '<div class="title">出貨明細</div>',
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
                         html: '出貨編號:' + deliver_id,
                         frame: false,
                         margin: '3 4 5 0',
                         border: false
                     },
                     {
                         xtype: "displayfield",
                         id: 'delivery_status',
                         name: 'delivery_status',
                         width: 200
                     }]
             }

        ]
    });
    //頁面加載時創建grid
    var gdCancel = Ext.create('Ext.grid.Panel', {
        id: 'gdCancel',
        store: CancelDeliverDetailStore,
        border: false,
        columnLines: true,
        hidden: true,
        margin: '0 0 0 20',
        width: document.documentElement.clientWidth * 0.93,
        // width:1500,
        columns: [
            { header: '產品細項編號', dataIndex: 'item_id', flex: 1, align: 'center' },
            {
                header: '產品名稱', dataIndex: '', flex: 4.5, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return record.data.product_name + record.data.product_spec_name;
                }
            },
            { header: '數量', dataIndex: 'buy_num', flex: 1, align: 'center' },
            { header: '商品狀態', dataIndex: 'sdetail_status', flex: 1, align: 'center' },
            {
                header: '備註', dataIndex: '', flex: 1, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.combined_mode >= 1 && record.data.item_mode == 2) {
                        return "組合商品 【" + record.data.parent_name + "】";
                    }
                }
            }
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        viewConfig: {
            forceFit: true,
            getRowClass: function (record, rowIndex, rowParams, store) {
                if (true) {
                    return 'cancelcolor';
                }
            }

        }
        //viewConfig: {
        //    forceFit: true
        //}
        //selModel: sm
    });
    var cancelForm = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        renderTo: Ext.getBody(),
        id: 'cancelForm',
        url: '',
        hidden: false,
        border: false,
        // width: document.documentElement.clientWidth,
        plain: true,
        //bodyPadding: 20,

        //defaults: { anchor: "95%", msgTarget: "side" },
        items: [gdCancel,
            {
                xtype: 'button', text: '訂單出貨明細', id: 'orderdelivers', margin: '5 0 0 20', hidden: true, handler: function () {
                    window.open('/SendProduct/GetOrderDetailsPDF?deliver_id=' + deliver_id);
                }
            },
           {
               xtype: 'button', text: '出貨明細', id: 'deliverdetails', margin: '5 0 0 20', hidden: true, handler: function () {
                   window.open('/SendProduct/GetDeliverDetailsPDF?deliver_id=' + deliver_id);
               }
           },
           {
               xtype: 'button', text: '貨運單', id: 'shopbills', margin: '5 0 0 5', hidden: true, handler: function () {
                   window.open('/SendProduct/GetShopbillsPDF?deliver_id=' + deliver_id);
               }
           }
        ]
    });

    //頁面加載時創建grid
    var gdOrderDetail = Ext.create('Ext.grid.Panel', {
        id: 'gdOrderDetail',
        store: NormalDeliverDetailStore,
        border: false,
        columnLines: true,
        margin: '0 0 0 20',
        width: document.documentElement.clientWidth * 0.93,
        columns: [
            { header: '產品細項編號', dataIndex: 'item_id', flex: 1, align: 'center' },
            {
                header: '產品名稱', dataIndex: '', flex: 4.5, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return record.data.product_name + record.data.product_spec_name;
                }
            },
            { header: '數量', dataIndex: 'buy_num', flex: 1, align: 'center' },
            { header: '商品狀態', dataIndex: 'sdetail_status', flex: 1, align: 'center' },
            {
                header: '備註', dataIndex: '', flex: 1, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.combined_mode >= 1 && record.data.item_mode == 2) {
                        return "組合商品 【" + record.data.parent_name + "】";
                    }
                    else {
                        return '';
                    }
                }
            },
            {
                header: '功能', dataIndex: '', flex: 1, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    var result = '';
                    //出貨單狀態為待取貨 和 訂單細項狀態為進倉中
                    if (record.data.delivery_status == '0' && record.data.detail_status == '6') {
                        result += "<a href='javascript:void(0);' onclick='NoDelivery(" + record.data.deliver_id + ',' + record.data.detail_id + ")'>" + '未到貨' + "</a>";
                        result += '&nbsp;&nbsp;';
                    }
                    //出貨單狀態為待取貨 和order_detail表中的combined_mode=0    0代表一般 1 組合商品的父商品 2 組合商品的子商品
                    if (record.data.delivery_status == '0' && record.data.buy_num > 1 && record.data.combined_mode < 1) {
                        result += "<a href='javascript:void(0);' onclick='SplitDetail(" + record.data.deliver_id + ',' + record.data.detail_id + ")'>" + '拆分細項' + "</a>";
                        result += '&nbsp;&nbsp;';
                    }

                    //product_mode  1 自出 2 寄倉 3 調度
                    /* 
                     1.當訂單中的商品貨運模式不是自出時，該商品可改自出； 
                     2.當訂單中商品貨運模式不是調度室，該商品可改調度； 
                     3.當訂單中商品貨運模式不是寄倉，但商品設定貨運模式是寄倉時，該商品才可改寄倉 
                   */
                    if (record.data.product_mode != '1') {
                        result += "<a href='javascript:void(0);' onclick='AlterProductMode(" + record.data.deliver_id + ',' + record.data.detail_id + ',' + 1 + ")'>" + '改自出' + "</a>";
                        result += '&nbsp;&nbsp;';
                    }
                    if (record.data.product_mode != '2' && record.data.prod_mode == '2') {
                        result += "<a href='javascript:void(0);' onclick='AlterProductMode(" + record.data.deliver_id + ',' + record.data.detail_id + ',' + 2 + ")'>" + '改寄倉' + "</a>";
                        result += '&nbsp;&nbsp;';
                    }
                    if (record.data.product_mode != '3') {
                        result += "<a href='javascript:void(0);' onclick='AlterProductMode(" + record.data.deliver_id + ',' + record.data.detail_id + ',' + 3 + ")'>" + '改調度' + "</a>";
                    }
                    return result;
                }
            }
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        viewConfig: {
            forceFit: true
        },
        selModel: sm

    });

    //回填物流單號
    var ChangeDeliverId = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        renderTo: Ext.getBody(),
        id: 'ChangeDeliverId',
        url: '/SendProduct/DeliverDetailEdit',
        border: false,
        plain: true,
        //bodyPadding: 20,
        margin: '1 0 0 20',
        defaults: { anchor: "93%", msgTarget: "side" },
        items: [
            {
                xtype: 'button',
                text: '下一次出貨',
                id: 'nextdeliver',
                anchor: "5%",
                margin: '0 0 5 0',
                hidden: true,
                disabled: true,
                handler: onNextClick
            },
            {
                xtype: "displayfield",
                id: 'note_order',
                name: 'note_order',
                hidden: true,
                width: 250
            },
            {
                xtype: 'fieldset',
                title: '回填物流單號',
                defaultType: 'textfield',
                bodyPadding: 20,
                margin: '8 0 0 0',
                layout: 'anchor',
                items: [
                    {
                        xtype: "displayfield",
                        id: 'holiday',
                        name: 'holiday',
                        hidden: true,
                        value: '<p style="font-size:x-large;color:white;background-color:red;">假日不出貨</p>',
                        width: 250
                    },
                    {
                        xtype: 'combobox',
                        allowBlank: false,
                        id: 'delivery_store',
                        name: 'delivery_store',
                        store: DeliverStore,
                        queryMode: 'local',
                        width: 250,
                        labelWidth: 100,
                        displayField: 'parameterName',
                        valueField: 'parameterCode',
                        typeAhead: true,
                        forceSelection: false,
                        editable: false,
                        fieldLabel: "物流業者"
                    },
                    {
                        xtype: "textfield",
                        fieldLabel: '物流單號',
                        id: 'delivery_code',
                        name: 'delivery_code',
                        labelWidth: 100,
                        allowBlank: false,
                        width: 250

                    },
                    {
                        xtype: "datetimefield",
                        fieldLabel: '出貨日期',
                        id: 'delivery_date',
                        name: 'delivery_date',
                        labelWidth: 100,
                        format: 'Y-m-d H:i:s',
                        allowBlank: true,
                        editable: false,
                        width: 250
                    },
                    {
                        xtype: 'button',
                        text: '保存',
                        id: 'savecode',
                        width: 50,
                        formBind: true,
                        disabled: true,
                        handler: function () {
                            var form = this.up('form').getForm();
                            if (form.isValid()) {
                                form.submit({
                                    params: {
                                        deliver_id: deliver_id,
                                        delivery_store: Ext.getCmp('delivery_store').getValue(),
                                        delivery_code: Ext.getCmp('delivery_code').getValue(),
                                        delivery_date: Ext.getCmp('delivery_date').getValue()
                                    },
                                    success: function (form, action) {
                                        var result = Ext.JSON.decode(action.response.responseText);
                                        if (result.success) {
                                            Ext.Msg.alert("提示", "回填貨運單號成功", function () {
                                                document.location.href = "/SendProduct/DeliverView?deliver_id=" + deliver_id;
                                            });
                                        }
                                        else {
                                            Ext.Msg.alert("提示", "系統錯誤", function () {
                                                document.location.href = "/SendProduct/DeliverView?deliver_id=" + deliver_id;
                                            });
                                        }
                                    },
                                    failure: function (form, action) {
                                        var result = Ext.decode(action.response.responseText);
                                        Ext.Msg.alert("提示", "系統錯誤", function () {
                                            document.location.href = "/SendProduct/DeliverView?deliver_id=" + deliver_id;
                                        });
                                    }
                                });
                            }
                        }
                    },
                    {
                        html: "<a href='javascript:void(0);'  onclick='TranToVerify()'>下一筆</a>",
                        frame: false,
                        width: 400,
                        border: false
                        //handler: function ()
                        //{ TranToVerify("/SendProduct/DeliverVerify") }
                    },
                    {
                        html: '<br/>&nbsp;&nbsp;&nbsp;&nbsp;.<font color=red>物流方式 7-11取貨(Yahoo), 公司自送, 其它 不發簡訊</font>',
                        frame: false,
                        width: 400,
                        border: false
                    },
                    {
                        html: '&nbsp;&nbsp;&nbsp;&nbsp;.<font color=red>出貨日期自選不發簡訊</font>',
                        frame: false,
                        width: 400,
                        border: false
                    }
                ]
            }
        ]
    });
    //變更出貨簡訊時間
    var ChangeSMSDate = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        renderTo: Ext.getBody(),
        url: '/SendProduct/DeliverDetailEdit',
        border: false,
        plain: true,
        bodyPadding: 20,
        margin: '5 0 0 0',
        id: 'ChangeSMSDate',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'fieldset',
                title: '變更出貨簡訊時間',
                items: [
                    {
                        xtype: "datefield",
                        fieldLabel: '簡訊日期',
                        id: 'sms_date',
                        name: 'sms_date',
                        labelWidth: 100,
                        allowBlank: false,
                        editable: false,
                        width: 250
                    },
                    {
                        xtype: 'button',
                        text: '保存',
                        id: 'ssmsdate',
                        hidden:false,
                        formBind: true,
                        disabled: true,
                        handler: function () {
                            var form = this.up('form').getForm();
                            if (form.isValid()) {
                                form.submit({
                                    params: {
                                        deliver_id: deliver_id,
                                        sms_date: Ext.getCmp('sms_date').getValue()
                                    },
                                    success: function (form, action) {
                                        var result = Ext.JSON.decode(action.response.responseText);
                                        if (result.success) {
                                            LoadData(deliver_id);
                                        }
                                        else {
                                            Ext.Msg.alert("提示", "已發送或無此出貨單簡訊");
                                        }
                                    },
                                    failure: function (form, action) {
                                        var result = Ext.decode(action.response.responseText);
                                        Ext.Msg.alert("提示", "已發送或無此出貨單簡訊");
                                    }
                                });
                            }
                        }
                    },
                    {
                        html: '&nbsp;&nbsp;&nbsp;&nbsp;.<font color=red>注意：簡訊若已發出，簡訊時間就不會再更新</font>',
                        frame: false,
                        width: 400,
                        border: false
                    }
                ]
            }
        ]
    });
    //變更到貨時間
    var ChangeArrivalDate = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        renderTo: Ext.getBody(),
        url: '/SendProduct/DeliverDetailEdit',
        border: false,
        plain: true,
        bodyPadding: 20,
        margin: '5 0 0 0',
        id: 'ChangeArrivalDate',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'fieldset',
                title: '變更到貨時間',
                defaultType: 'textfield',
                layout: 'anchor',
                items: [
                    {
                        xtype: "datefield",
                        fieldLabel: '預計出貨日期',
                        id: 'estimated_delivery_date',
                        name: 'estimated_delivery_date',
                        labelWidth: 100,
                        allowBlank: true,
                        editable: false,
                        width: 250

                    },
                    {
                        xtype: "datefield",
                        fieldLabel: '預計到貨日期',
                        id: 'estimated_arrival_date',
                        name: 'estimated_arrival_date',
                        labelWidth: 100,
                        allowBlank: true,
                        editable: false,
                        width: 250
                    },
                    {
                        xtype: 'combobox',
                        allowBlank: false,
                        id: 'estimated_arrival_period',
                        name: 'estimated_arrival_period',
                        store: ArrivalPeriodStore,
                        queryMode: 'local',
                        width: 250,
                        labelWidth: 100,
                        displayField: 'txt',
                        valueField: 'value',
                        typeAhead: true,
                        forceSelection: false,
                        editable: false,
                        fieldLabel: "預計到貨時段",
                        value: 0
                    },
                    {
                        xtype: 'button',
                        text: '保存',
                        id: 'savearrivaldate',
                        formBind: true,
                        disabled: true,
                        handler: function () {
                            var form = this.up('form').getForm();
                            if (form.isValid()) {
                                form.submit({
                                    params: {
                                        deliver_id: deliver_id,
                                        estimated_delivery_date: Ext.getCmp('estimated_delivery_date').getValue(),
                                        estimated_arrival_date: Ext.getCmp('estimated_arrival_date').getValue(),
                                        estimated_arrival_period: Ext.getCmp('estimated_arrival_period').getValue(),
                                        type: 1
                                    },
                                    success: function (form, action) {
                                        var result = Ext.JSON.decode(action.response.responseText);
                                        if (result.success) {
                                            Ext.Msg.alert("提示", "修改成功！");
                                            LoadData();
                                        }
                                    },
                                    failure: function (form, action) {
                                        var result = Ext.JSON.decode(action.response.responseText);
                                        Ext.Msg.alert(INFORMATION, UFAILURE);
                                    }
                                });
                            }
                        }
                    }
                ]
            }
        ]
    });
    //變更收件人資料
    var ChangeCustomerInfo = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        renderTo: Ext.getBody(),
        url: '/SendProduct/DeliverDetailEdit',
        border: false,
        plain: true,
        bodyPadding: 20,
        labelWidth: 45,
        margin: '5 0 0 0',
        id: 'ChangeCustomerInfo',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'fieldset',
                title: '變更收件人資料',
                defaultType: 'textfield',
                layout: 'anchor',
                items: [
                    {
                        xtype: "textfield",
                        fieldLabel: '寄件人',
                        id: 'delivery_name',
                        name: 'delivery_name',
                        labelWidth: 100,
                        allowBlank: true,
                        width: 250
                    },
                    {
                        xtype: "textfield",
                        fieldLabel: '行動電話',
                        id: 'delivery_mobile',
                        name: 'delivery_mobile',
                        labelWidth: 100,
                        allowBlank: true,
                        width: 250
                    },
                    {
                        xtype: "textfield",
                        fieldLabel: '聯絡電話',
                        id: 'delivery_phone',
                        name: 'delivery_phone',
                        labelWidth: 100,
                        allowBlank: true,
                        width: 250
                    },
                     {
                         xtype: 'combo',
                         fieldLabel: '郵遞區號',
                         id: 'delivery_zip',
                         name: 'delivery_zip',
                         queryMode: 'local',
                         displayField: 'user_zip_name',
                         valueField: 'user_zip',
                         labelWidth: 100,
                         width: 250,
                         store: user_zip_source,
                         editable: false
                     },
                    {
                        xtype: "textfield",
                        fieldLabel: '收貨地址',
                        id: 'delivery_address',
                        name: 'delivery_address',
                        labelWidth: 100,
                        allowBlank: true,
                        width: 250
                    },
                    {
                        xtype: 'button',
                        text: '保存',
                        id: 'saveinfo',
                        formBind: true,
                        disabled: true,
                        handler: function () {
                            var form = this.up('form').getForm();
                            if (form.isValid()) {
                                form.submit({
                                    params: {
                                        deliver_id: deliver_id,
                                        delivery_name: Ext.getCmp('delivery_name').getValue(),
                                        delivery_mobile: Ext.getCmp('delivery_mobile').getValue(),
                                        delivery_phone: Ext.getCmp('delivery_phone').getValue(),
                                        delivery_zip: Ext.getCmp('delivery_zip').getValue(),
                                        delivery_address: Ext.getCmp('delivery_address').getValue(),
                                        type: 2
                                    },
                                    success: function (form, action) {
                                        var result = Ext.JSON.decode(action.response.responseText);
                                        if (result.success) {
                                            Ext.Msg.alert("提示", "修改成功！");
                                            LoadData();
                                        }
                                    },
                                    failure: function (form, action) {
                                        var result = Ext.JSON.decode(action.response.responseText);
                                        Ext.Msg.alert("提示", "修改失敗！");
                                    }
                                });
                            }
                        }
                    }
                ]
            }
        ]
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'anchor',
        items: [titleForm, cancelForm, gdOrderDetail, ChangeDeliverId, ChangeSMSDate, ChangeArrivalDate, ChangeCustomerInfo],
        renderTo: Ext.getBody(),
        autoScroll: true,
        //overflowY: 'auto',
        margin: '0 0 0 0',
        // height: 1200,
        listeners: {
            resize: function () {
                ChangeDeliverId.width = document.documentElement.clientWidth;
                ChangeSMSDate.width = document.documentElement.clientWidth;
                ChangeArrivalDate.width = document.documentElement.clientWidth;
                ChangeCustomerInfo.width = document.documentElement.clientWidth;
                this.doLayout();
            },
            render: function () {
                LoadStore(deliver_id);
                LoadData(deliver_id);
            }
        }
    });
});
//加載倆個store的數據
function LoadStore(deliver_id) {
    Ext.Ajax.request({
        url: '/SendProduct/GetDeliverDetail',
        method: 'post',
        params: {
            deliver_id: deliver_id
        },
        success: function (form, action) {
            var responseJson = Ext.JSON.decode(form.responseText);
            var canceldata = responseJson.canceldata;
            var normaldata = responseJson.normaldata;
            if (canceldata.length > 0) {
                Ext.getCmp("gdCancel").show();
            }
            //alert(responseJson);
            CancelDeliverDetailStore.loadData(canceldata);
            NormalDeliverDetailStore.loadData(normaldata);
        },
        failure: function () {
            //Ext.Msg.alert("提示","您輸入的料位條碼不符,請查找對應的料位!");
        }
    });

}
//加載出貨單的信息
function LoadData(deliver_id) {
    Ext.Ajax.request({
        url: '/SendProduct/GetDeliverMaster',
        method: 'post',
        params: {
            deliver_id: deliver_id
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                //請求成功,控制下一步的操作
                if (result.data.length > 0) {
                    //顯示出貨狀態
                    Ext.getCmp("delivery_status").setValue("(" + DeliveryStatus(result.data[0].delivery_status) + ")");
                    //if (CancelDeliverDetailStore.getCount() > 0) {
                    //    Ext.getCmp("cancelForm").show();
                    //}
                    //顯示匯出按鈕
                    /*  deliver_master表的type對應內容
                     1 => '統倉出貨',
                     2 => '供應商自行出貨',
                     3 => '供應商調度出貨',
                     4 => '退貨',
                     5 => '退貨瑕疵',
                     6 => '瑕疵' */
                    if (result.data[0].type == 1) {
                        Ext.getCmp("orderdelivers").show();
                    }
                    else {
                        Ext.getCmp("deliverdetails").show();
                    }
                    if (result.data[0].delivery_store == 42) {
                        Ext.getCmp("shopbills").show();
                    }
                    var i = NormalDeliverDetailStore.getCount();
                    var j = CancelDeliverDetailStore.getCount();
                    //顯示下一次出貨按鈕
                    if (result.data[0].delivery_status == 0 && Ext.getCmp("gdOrderDetail").getStore().getCount() > 1) {
                        Ext.getCmp("nextdeliver").show();
                    }
                    //是否顯示備註
                    if (result.data[0].note_order != "") {
                        Ext.getCmp("note_order").setValue('<p><strong>備註</strong>:<font style="background-color:red;color:white" >' + result.data[0].note_order + '</font></p>');
                        Ext.getCmp("note_order").show();
                    }
                    if (result.data[0].delivery_status != 0) {
                        Ext.getCmp("savearrivaldate").hide();
                        Ext.getCmp("saveinfo").hide();
                    }
                    //var savecodestatus = [0, 1, 2];
                    ////待出貨 可出貨 出貨中
                    //if (true||!Ext.Array.contains(savecodestatus, result.data[0].delivery_status)) {
                    //    Ext.getCmp("savecode").hide();
                    //}
                    //顯示節假日出貨提示
                    if (result.data[0].holiday_deliver == 0) {
                        Ext.getCmp("holiday").show();
                    }
                    Ext.getCmp("delivery_store").setValue(result.data[0].delivery_store);
                    Ext.getCmp("delivery_code").setValue(result.data[0].delivery_code);
                    if (result.data[0].delivery_date != "0001-01-01 00:00:00") {
                        Ext.getCmp("delivery_date").setValue(result.data[0].delivery_date);
                    }
                    if (result.data[0].estimated_delivery_date != "0001-01-01") {
                        Ext.getCmp("estimated_delivery_date").setValue(result.data[0].estimated_delivery_date);
                    }
                    if (result.data[0].estimated_arrival_date != "0001-01-01") {
                        Ext.getCmp("estimated_arrival_date").setValue(result.data[0].estimated_arrival_date);
                    }
                    Ext.getCmp("estimated_arrival_period").setValue(result.data[0].estimated_arrival_period);
                    Ext.getCmp("delivery_name").setValue(result.data[0].delivery_name);
                    Ext.getCmp("delivery_mobile").setValue(result.data[0].delivery_mobile);
                    Ext.getCmp("delivery_phone").setValue(result.data[0].delivery_phone);
                    Ext.getCmp("delivery_zip").setValue(result.data[0].delivery_zip);
                    Ext.getCmp("delivery_address").setValue(result.data[0].delivery_address);
                    Ext.getCmp("sms_date").setValue(result.data[0].sms_date);
                  
                } else {
                    
                }
            } else {
                Ext.Msg.alert("提示", "暫無數據!");
            }
        },
        failure: function () {
            Ext.Msg.alert("提示", "系統異常!");
        }
    });
}
function DeliveryStatus(delivery_status) {
    switch (delivery_status) {
        case 0:
            return '待出貨';
        case 1:
            return '可出貨';
        case 2:
            return '出貨中';
        case 3:
            return '已出貨';
        case 4:
            return '已到貨';
        case 5:
            return '未到貨';
        case 6:
            return '取消出貨';
        case 7:
            return '待取貨';

    }


}
function AlterProductMode(deliver_id, detail_id, product_mode) {
    Ext.Ajax.request({
        url: '/SendProduct/ProductMode',
        method: 'post',
        params: {
            deliver_id: deliver_id,
            detail_id: detail_id,
            product_mode: product_mode
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                //Ext.Msg.alert(INFORMATION, "該訂單尙有商品未撿！", function () {
                //    document.location.href = "/WareHouse/MarkTallyTW?number=" + document.getElementById("ord_id").value;//繼續輸入產品條碼
                //});//跳轉輸入訂單號頁面 
                Ext.Msg.alert("提示", "變更成功!", function () {
                    document.location.href = "/SendProduct/DeliverView?deliver_id=" + result.msg;
                });

            }
        },
        failure: function () {
            Ext.Msg.alert("提示", "變更失敗!", function () {
                document.location.href = "/SendProduct/DeliverView?deliver_id=" + deliver_id;
            });
        }
    })
}
//未到貨
function NoDelivery(deliver_id, detail_id) {
    Ext.Ajax.request({
        url: '/SendProduct/LackDelivery',
        method: 'post',
        params: {
            deliver_id: deliver_id,
            detail_id: detail_id
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                Ext.Msg.alert("提示", "未到貨!", function () {
                    document.location.href = "/SendProduct/DeliverView?deliver_id=" + deliver_id;
                });
            }
        },
        failure: function () {
            Ext.Msg.alert("提示", "系統錯誤!", function () {
                document.location.href = "/SendProduct/DeliverView?deliver_id=" + deliver_id;
            });
        }
    })
}
//拆分細項
function SplitDetail(deliver_id, detail_id) {
    Ext.Ajax.request({
        url: '/SendProduct/SplitDetail',
        method: 'post',
        params: {
            deliver_id: deliver_id,
            detail_id: detail_id
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                Ext.Msg.alert("提示", "拆分成功!", function () {
                    document.location.href = "/SendProduct/DeliverView?deliver_id=" + deliver_id;
                });
            }
        },
        failure: function () {
            Ext.Msg.alert("提示", "系統錯誤!", function () {
                document.location.href = "/SendProduct/DeliverView?deliver_id=" + deliver_id;
            });
        }
    })
}
/***********************************下一次出貨方法**********************************************************/
function Split(deliver_id, detail_ids) {
    Ext.Ajax.request({
        url: '/SendProduct/Split',
        method: 'post',
        params: {
            deliver_id: deliver_id,
            detail_ids: detail_ids
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                Ext.Msg.alert("提示", "變更成功!", function () {
                    document.location.href = "/SendProduct/DeliverView?deliver_id=" + result.msg;
                });
            }
        },
        failure: function () {
            Ext.Msg.alert("提示", "變更失敗!", function () {
                document.location.href = "/SendProduct/DeliverView?deliver_id=" + deliver_id;
            });
        }
    })
}
/***********************************下一次出貨按鈕**********************************************************/
onNextClick = function () {
    var row = Ext.getCmp("gdOrderDetail").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length >= 1) {
        // Ext.Msg.alert(INFORMATION, ONE_SELECTION);
        var detail_ids = [];
        for (var i = 0; i < row.length; i++) {
            detail_ids.push(row[i].data.detail_id);
        }
        Split(deliver_id, detail_ids);
    }
}

//跳轉到出貨明細頁
function TranToVerify() {
    var urlTran = '/SendProduct/DeliverVerify';
    var panel1 = window.parent.parent.Ext.getCmp('ContentPanel');
    var c = panel1.down('#eleverify');
    if (c) {
        c.close();
    }
    c = panel1.add({
        id: 'eleverify',
        title: "出貨確認",
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel1.setActiveTab(c);
    panel1.doLayout();
}