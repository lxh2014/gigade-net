var pageSize = 20;
Ext.define('gigade.OrderMasterExportList', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "row_id", type: "int" },
    { name: "order_id", type: "int" },
    { name: "order_amount", type: "int" },
    { name: "order_name", type: "string" },
    { name: "deduct_card_bonus", type: "string" },
    { name: "parameterName", type: "string" },
    { name: "ordercreatedate", type: "string" },
    { name: "account_collection_time", type: "string" },
    { name: "poundage", type: "string" },
    { name: "account_collection_money", type: "string" },
    { name: "return_collection_time", type: "string" },
    { name: "return_poundage", type: "string" },
    { name: "return_collection_money", type: "string" },
    { name: "invoicedate", type: "string" },
    { name: "sales_amount", type: "int" },
    { name: "free_tax", type: "int" },
    { name: "remark", type: "string" },
    { name: "imramount", type: "int" },
    { name: "oacamount", type: "int" },
    { name: "tax_amount", type: "int" }
    ]
});
var OrderMasterExportStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    autoLoad: false,
    model: 'gigade.OrderMasterExportList',
    proxy: {
        type: 'ajax',
        url: '/Order/OrderMasterExportList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
//日期條件
var DateAllQueryStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { "txt": '請選擇日期條件', "value": "0" },
    { "txt": '銀行入賬日期', "value": "1" },
    { "txt": '退貨入賬日期', "value": "4" },
    { "txt": '開立發票日期', "value": "2" },
    { "txt": '訂單日期', "value": "3" }

    ]
});
//加載前先獲取ddl的值
OrderMasterExportStore.on('beforeload', function () {
    var orderTimeStart = dateFormat(Ext.getCmp('orderTimeStart').getValue());
    var orderTimeEnd = dateFormat(Ext.getCmp('orderTimeEnd').getValue());
    var t_order_id = Ext.getCmp('t_order_id').getValue();
    var show_type = Ext.htmlEncode(Ext.getCmp("show_type").getValue().show_type);
    var invoice_type = Ext.htmlEncode(Ext.getCmp("invoice_type").getValue().invoice_type);
    Ext.apply(OrderMasterExportStore.proxy.extraParams, {
        orderTimeStart: orderTimeStart,
        orderTimeEnd: orderTimeEnd,
        order_id: t_order_id,
        dateType: Ext.getCmp('date_type').getValue(),
        show_type: show_type,
        invoice_type: invoice_type
    });
    Ext.Ajax.request({
        url: '/Order/OrderMasterHuiZong',
        method: 'post',
        params: {
            orderTimeStart: orderTimeStart,
            orderTimeEnd: orderTimeEnd,
            order_id: t_order_id,
            dateType: Ext.getCmp('date_type').getValue(),
            show_type: show_type,
            invoice_type: invoice_type
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                Ext.getCmp("lblpoundage").setText(result.msg);
                Ext.getCmp("lblAccount").setText(result.AccountMoney);
                Ext.getCmp("ZMoney").setText(result.ZMoney);
                Ext.getCmp("SalesAmount").setText(result.SalesAmount);
                Ext.getCmp("FreeTax").setText(result.FreeTax);
                Ext.getCmp("ZTax").setText(result.ZTax);
            } else {

            }
        },
        failure: function () {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            if (selections.length == 0 || (selections.length != 0 && selections[0].data.account_collection_time == "" && selections[0].data.return_collection_time == "")) {
              //  Ext.getCmp("pcGift").down('#edit').setDisabled(true);
                Ext.getCmp("pcGift").down('#delete').setDisabled(true);
            } else {
               // Ext.getCmp("pcGift").down('#edit').setDisabled(false);
                Ext.getCmp("pcGift").down('#delete').setDisabled(false);
            }

            Ext.getCmp("pcGift").down('#edit').setDisabled(selections.length == 0);
            //Ext.getCmp("pcGift").down('#delete').setDisabled(selections.length == 0);
        
        }
    }
});
function Query(x) {
    Ext.getCmp("pcGift").show();
    if (Ext.getCmp('date_type').getValue() == 0 && Ext.getCmp('t_order_id').getValue() == "") {
        Ext.Msg.alert(INFORMATION, "請輸入付款單號或選擇日期條件查詢！");
    } else {
        OrderMasterExportStore.removeAll();
        Ext.getCmp("pcGift").store.loadPage(1)
    }
}
Ext.onReady(function () {
    var exportTab = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        title: '匯入',
        width: 600,
        url: '/Order/OrderMasterImport',
        margin: '0 10 0 0',
        defaults: {
            labelWidth: 150,
            width: 400,
            margin: '10 10 0 0'
        },
        border: false,
        plain: true,
        id: 'OrderMasterImport',
        items: [
        {
            xtype: 'panel',
            bodyStyle: "padding:5px;background:#87CEEB",
            border: false,
            html: "注意事項：<br/>1.檔案為.xls<br/>2.欄位：付款單號、銀行入帳日期、入賬金額、手續費、退貨入帳日期、退貨入賬金額、退貨入帳手續費、備註。<br/>3.當檔案中存在異常時,將不會處理異常數據,且其它數據會繼續匯入.<br/>4.<a href='javascript:void(0);' onclick='ShowMuBan()'>點擊下載匯入模板</a>"
        },
        {
            xtype: 'filefield',
            name: 'ImportFileMsg',
            id: 'ImportFileMsg',
            fieldLabel: '檔案',
            msgTarget: 'side',
            buttonText: '瀏覽..',
            submitValue: true,
            allowBlank: false,
            fileUpload: true,
            validator: function (value) {
                var type = value.split('.');
                if (type[type.length - 1] == 'xls' || type[type.length - 1] == 'xlsx') {
                    return true;
                } else {
                    return '上傳文件類型不正確！';
                }
            }
        }
        ],
        buttonAlign: 'right',
        buttons: [{
            text: '確定匯入',
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            ImportFileMsg: Ext.htmlEncode(Ext.getCmp('ImportFileMsg').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert("提示信息", result.msg);
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert("提示信息", result.msg);
                        }
                    });
                }
            }
        }
        ]
    });
    var hpanel = Ext.create('Ext.form.Panel', {
        //layout: 'hbox',
        title: '匯出查詢',
        width: 800,
        border: false,
        plain: true,
        id: 'OrderMasterExport',
        items: [
        {
            xtype: 'panel',
            bodyStyle: "padding:5px;background:#87CEEB",
            border: false,
            html: "注意事項：<br/>查詢條件：銀行入賬日期、退貨入賬日期、開立發票日期，訂單日期，只能選其一查詢；若不選擇日期條件，請輸入訂單編號查詢。"
        },
        {
            xtype: 'fieldcontainer',
            combineErrors: true,
            id: 'date',
            layout: 'hbox',
            items: [
            {
                xtype: 'combobox',
                id: 'date_type',
                name: 'date_type',
                fieldLabel: '日期條件',
                queryMode: 'local',
                store: DateAllQueryStore,
                displayField: 'txt',
                valueField: 'value',
                margin: "5 5 0 0",
                value: 0,
                typeAhead: true,
                editable: false,
                hiddenName: 'value'
            },
            {
                xtype: 'datetimefield',
                allowBlank: true,
                id: 'orderTimeStart',
                margin: "5 5 0 0",
                //format: 'Y-m-d H:i:s',
                name: 'orderTime',
                width: 200,
                fieldLabel: '日期',
                value: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate() - 14),
                labelWidth: 60,
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("orderTimeStart");
                        var end = Ext.getCmp("orderTimeEnd");
                        if (end.getValue() == null) {
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }
                        else if (start.getValue() > end.getValue()) {
                            Ext.Msg.alert(INFORMATION, DATA_TIP);
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }
                        else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }
                    },
                    listeners: {
                        specialkey: function (field, e) {
                            if (e.getKey() == e.ENTER) {
                                Query();
                            }
                        }

                    }
                }
            },
            {
                xtype: 'datetimefield',
                allowBlank: true,
                id: 'orderTimeEnd',
                margin: "5 5 0 0",
                name: 'orderTime',
                fieldLabel: '到',
                width: 200,
                value: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate()),
                labelWidth: 15,
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("orderTimeStart");
                        var end = Ext.getCmp("orderTimeEnd");
                        if (end.getValue() == null) {
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }
                        else if (start.getValue() > end.getValue()) {
                            Ext.Msg.alert(INFORMATION, DATA_TIP);
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }
                        else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                            start.setValue(setNextMonth(end.getValue(), -1));
                        }
                    },
                    listeners: {
                        specialkey: function (field, e) {
                            if (e.getKey() == e.ENTER) {
                                Query();
                            }
                        }
                    }
                }
            }
            ]
        },
        {
            xtype: 'fieldcontainer',
            combineErrors: true,
            layout: 'hbox',
            id: 'type',
            items: [
            {
                xtype: 'textfield',
                allowBlank: true,
                id: 't_order_id',
                //padding: "0 0 5 0",
                margin: "0 5 0 0",
                name: 't_order_id',
                fieldLabel: '付款單號',
                labelWidth: 100,
                listeners: {
                    specialkey: function (field, e) {
                        if (e.getKey() == e.ENTER) {
                            Query();
                        }
                    }
                }
            },
            {
                xtype: 'radiogroup',
                id: 'show_type',
                name: 'show_type',
                margin: "0 5 0 0",
                width: 380,
                fieldLabel: "入賬狀態",
                colName: 'show_type',
                defaults: {
                    name: 'show_type'
                },
                columns: 3,
                items: [
                { id: 'stateid1', boxLabel: "全部", inputValue: '0', checked: true },
                { id: 'stateid2', boxLabel: "已入賬", inputValue: '1' },
                { id: 'stateid3', boxLabel: "未入賬", inputValue: '2' }
                ]
            }
            ]
        },
        {
            xtype: 'radiogroup',
            id: 'invoice_type',
            name: 'invoice_type',
            margin: "0 5 0 0",
            width: 380,
            fieldLabel: "發票狀態",
            colName: 'invoice_type',
            defaults: {
                name: 'invoice_type'
            },
            columns: 3,
            items: [
            { id: 'invo_1', boxLabel: "全部", inputValue: '0', checked: true },
            { id: 'invo_2', boxLabel: "已開", inputValue: '1' },
            { id: 'invo_3', boxLabel: "未開", inputValue: '2' }
            ]
        }
        ],
        buttonAlign: 'right',
        buttons: [
        {
            xtype: 'button',
            text: SEARCH,
            iconCls: 'icon-search',
            id: 'btnQuery',
            width: 100,
            height: 25,
            margin: "0 5 0 0",
            handler: Query

        },
        {

            xtype: 'button',
            text: RESET,
            width: 100,
            margin: "8 5 0 0",
            id: 'btn_reset',
            listeners: {
                click: function () {
                    Ext.getCmp('orderTimeStart').setValue(null);
                    Ext.getCmp('orderTimeEnd').setValue(null);
                    Ext.getCmp('orderTimeStart').setValue(new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate() - 14));
                    Ext.getCmp('orderTimeEnd').setValue(new Date());
                    Ext.getCmp('t_order_id').setValue("");
                    Ext.getCmp('date_type').setValue(0);
                    Ext.getCmp('show_type').reset();
                    Ext.getCmp('invoice_type').reset();
                }
            }
        }
        ]
    });
    var searchForm = Ext.create('Ext.form.Panel', {
        id: 'searchForm',
        layout: 'hbox',
        //height: 450,
        flex: 3.1,
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
        exportTab,
        hpanel

        ]
    });
    var pcGift = Ext.create('Ext.grid.Panel', {
        id: 'pcGift',
        store: OrderMasterExportStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        //hidden: true,
        flex: 6.9,
        viewConfig: {
            enableTextSelection: true,
            stripeRows: false,
            getRowClass: function (record, rowIndex, rowParams, store) {
                return "x-selectable";
            }
        },
        columns: [
        { header: '編號', dataIndex: 'row_id', width: 100, align: 'center' },
        { header: '付款單號', dataIndex: 'order_id', width: 100, align: 'center' },
        { header: '訂購人', dataIndex: 'order_name', width: 100, align: 'center' },
        { header: '訂單應收金額', dataIndex: 'order_amount', width: 100, align: 'center' },
        { header: '紅利折抵金額', dataIndex: 'deduct_card_bonus', width: 100, align: 'center' },
        { header: '付款方式', dataIndex: 'parameterName', width: 100, align: 'center' },
        { header: '付款單成立日期', dataIndex: 'ordercreatedate', width: 100, align: 'center' },
        { header: '銀行入帳日期', dataIndex: 'account_collection_time', width: 100, align: 'center' },
        {
            header: '手續費', dataIndex: 'poundage', width: 100, align: 'center'
        },
        { header: '入帳金額', dataIndex: 'account_collection_money', width: 100, align: 'center' },
        { header: '退貨入帳日期', dataIndex: 'return_collection_time', width: 100, align: 'center' },
        {
            header: '退貨入賬手續費', dataIndex: 'return_poundage', width: 100, align: 'center'
        },
        { header: '退貨入帳金額', dataIndex: 'return_collection_money', width: 100, align: 'center' },
        { header: '入帳總額', dataIndex: 'oacamount', width: 100, align: 'center' },
        { header: '開立發票日期', dataIndex: 'invoicedate', width: 100, align: 'center' },
        { header: '發票銷售額', dataIndex: 'free_tax', width: 100, align: 'center' },
        { header: '發票稅額', dataIndex: 'tax_amount', width: 100, align: 'center' },
        { header: '發票總額', dataIndex: 'imramount', width: 100, align: 'center' },
        { header: '備註', dataIndex: 'remark', width: 100, align: 'center' }
        ],
        tbar: [

        { xtype: 'button', text: "新增單筆記錄", id: 'add', hidden: true, iconCls: 'icon-user-add', handler: onAddClick },
        { xtype: 'button', text: EDIT, id: 'edit', iconCls: 'icon-user-edit', disabled: true, hidden: true, handler: onEditClick },
        { xtype: 'button', text: "刪除", id: 'delete', iconCls: 'icon-user-edit', disabled: true, hidden: true, handler: onDeleteClick },
        '->',
        {
            xtype: 'button',
            text: '匯出EXCEL',
            iconCls: 'icon-excel',
            id: 'btnExcel',
            handler: function () {
                //var timestart = dateFormat(Ext.getCmp('timestart').getValue());
                //var timeend = dateFormat(Ext.getCmp('timeend').getValue());
                var orderTimeStart = dateFormat(Ext.getCmp('orderTimeStart').getValue());
                var orderTimeEnd = dateFormat(Ext.getCmp('orderTimeEnd').getValue());
                if (Ext.getCmp('date_type').getValue() == 0 && Ext.getCmp('t_order_id').getValue() == "") {
                    Ext.Msg.alert(INFORMATION, "請輸入查詢條件匯出！");
                } else {
                    window.open("/Order/OrderMasterExport?orderTimeStart=" + orderTimeStart + "&orderTimeEnd=" + orderTimeEnd + "&dateType=" + Ext.getCmp('date_type').getValue() + "&order_id=" + Ext.getCmp('t_order_id').getValue() + "&show_type=" + Ext.htmlEncode(Ext.getCmp("show_type").getValue().show_type) + "&invoice_type=" + Ext.htmlEncode(Ext.getCmp("invoice_type").getValue().invoice_type));
                }
            }
        }

        ],
        bbar: [Ext.create('Ext.PagingToolbar', {
            store: OrderMasterExportStore,
            pageSize: pageSize,
            width: 720,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        {
            xtype: 'label',
            id: "lblpoundage",
            margin: '0 5 0 5',
            text: '',
            flex: 1.2,
            style: 'fontSize:15px ;color:red'
        },
        {
            xtype: 'label',
            id: "lblAccount",
            margin: '0 10 0 5',
            text: '',
            flex: 1.2,
            style: 'fontSize:15px ;color:red'
        },
        {
            xtype: 'label',
            id: "ZMoney",
            margin: '0 10 0 5',
            text: '',
            flex: 1.2,
            style: 'fontSize:15px ;color:red'
        },
        {
            xtype: 'label',
            id: "SalesAmount",
            margin: '0 10 0 5',
            text: '',
            flex: 1.5,
            style: 'fontSize:15px ;color:red'
        },
        {
            xtype: 'label',
            id: "FreeTax",
            margin: '0 10 0 5',
            text: '',
            flex: 1.2,
            style: 'fontSize:15px ;color:red'
        },
        {
            xtype: 'label',
            id: "ZTax",
            margin: '0 10 0 5',
            text: '',
            flex: 1.2,
            style: 'fontSize:15px ;color:red'
        },
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        selModel: sm
    });
    Ext.create('Ext.Viewport', {
        layout: 'vbox',
        items: [searchForm, pcGift],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                pcGift.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //  OrderMasterExportStore.load({ params: { start: 0, limit: 20 } });
});
function dateFormat(value) {
    if (null != value) {
        return Ext.Date.format(new Date(value), 'Y-m-d H:i:s');
    } else {
        return "";
    }
}
/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    //addWin.show();
    editFunction(null, OrderMasterExportStore);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("pcGift").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], OrderMasterExportStore);
    }
}

onDeleteClick = function () {
    var row = Ext.getCmp("pcGift").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format("共" + row.length + "條數據," + "是否確定要刪除嗎?", row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.row_id + ',';
                }
                Ext.Ajax.request({
                    url: '/OrderAccountCollection/Delete',//執行方法
                    method: 'post',
                    params: { rid: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert(INFORMATION, "刪除成功!");
                            OrderMasterExportStore.load();
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, "刪除失敗!");
                            OrderMasterExportStore.load();
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }
        });
    }
}
function ShowMuBan() {
    window.open("/Order/OrderMasterImportMuBan");
}

setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    }
    else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}
