Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
//聲明grid
Ext.define('GIGADE.OrderBrandProduces', {
    extend: 'Ext.data.Model',
    fields: [
        //定單信息
        { name: "Order_Id", type: "int" }, //付款單號
        { name: "Order_Name", type: "string" }, //訂購人名稱
        { name: "Delivery_Name", type: "string" }, //收貨人
        { name: "Order_Amount", type: "int" }, //訂單應收金額
        { name: "Order_Payment", type: "string" }, //付款方式
        { name: "Order_Status", type: "string" }, //訂單狀態
        { name: "ordercreatedate", type: "string" }, //建立日期
        { name: "Note_Admin", type: "string" }, //管理員備註
        { name: "redirect_name", type: "string" }, //來源ID
        { name: "redirect_url", type: "string" }, //來源ID連接
        { name: "order_pay_message", type: "string" },//點數使用明細
        //定購人信息
        { name: "user_id", type: "int" }, //用戶編號   下面的這些是編輯用戶表時要用到的。
        { name: "user_email", type: "string" }, //用戶郵箱
        { name: "user_name", type: "string" }, //用戶名
        { name: "user_password", type: "string" }, //密碼
        { name: "user_gender", type: "string" }, //性別
        { name: "user_birthday_year", type: "string" }, //年
        { name: "user_birthday_month", type: "string" }, //月
        { name: "user_birthday_day", type: "string" }, //日
        { name: "user_zip", type: "string" }, //用戶地址
        { name: "user_address", type: "string" }, //用戶地址
        { name: "birthday", type: "string" }, //生日 
        { name: "user_mobile", type: "string" },
        { name: "user_phone", type: "string" }, //聯絡電話 
        { name: "suser_reg_date", type: "string" }, //註冊日期 
        { name: "mytype", type: "string" },
        { name: "send_sms_ad", type: "string" }, //是否接收簡訊廣告 
        { name: "adm_note", type: "string" } //管理員備註 
    ]
});
//獲取grid中的數據
var OrderBrandProducesListStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.OrderBrandProduces',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetOrderList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
            root: 'data'
        }
    }
});
//運送方式Model
Ext.define("gigade.typeModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "type_id", type: "string" },
        { name: "type_name", type: "string" }
    ]
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("OrderBrandProducesListGrid").down('#Remove').setDisabled(selections.length == 0);
            Ext.getCmp("OrderBrandProducesListtGrid").down('#Edit').setDisabled(selections.length == 0)
        }
    }
});
var searchStatusrStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有訂單資料", "value": "0" },
        { "txt": "訂單編號", "value": "1" },
        { "txt": "訂購人姓名", "value": "2" },
        { "txt": "訂購人信箱", "value": "3" },
        { "txt": "收貨人姓名", "value": "4" },
        { "txt": "來源ID", "value": "5" }
    ]
});

var dateoneStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有日期", "value": "0" },
        { "txt": "訂單日期", "value": "1" }
    ]
});
//付款方式
var PayTypeStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有方式", "value": "0" },
        { "txt": "聯合信用卡", "value": "1" },
        { "txt": "永豐ATM", "value": "2" },
        { "txt": "藍新信用卡", "value": "3" },
        { "txt": "支付寶", "value": "4" },
        { "txt": "銀聯", "value": "5" },
        { "txt": "傳真刷卡", "value": "6" },
        { "txt": "延遲付款", "value": "7" },
        { "txt": "黑貓貨到付款", "value": "8" },
        { "txt": "現金", "value": "9" },
        { "txt": "中國信託信用卡", "value": "10" },
        { "txt": "中國信託信用卡紅利折抵", "value": "11" },
        { "txt": "中國信託信用卡紅利折抵(10%)", "value": "12" },
        { "txt": "網際威信  HiTRUST 信用卡", "value": "13" },
        { "txt": "中國信託信用卡紅利折抵(20%)", "value": "14" },
        { "txt": "7-11貨到付款", "value": "15" }
    ]
});

OrderBrandProducesListStore.on('beforeload', function () {
    Ext.apply(OrderBrandProducesListStore.proxy.extraParams, {
        selecttype: Ext.getCmp('select_type').getValue(), //選擇查詢種類，訂單編號，訂購姓名
        searchcon: Ext.getCmp('search_con').getValue(), //查詢的內容
        timeone: Ext.getCmp('timeone').getValue(),
        dateOne: Ext.getCmp('dateOne').getValue(),
        dateTwo: Ext.getCmp('dateTwo').getValue(),
        //slave_status: Ext.htmlEncode(Ext.getCmp("slave_status").getValue().Slave_Status), //付款單狀態，付款失敗，已發貨
        order_payment: Ext.getCmp("order_payment").getValue(), //付款方式，AT
        order_pay: Ext.htmlEncode(Ext.getCmp("order_pay").getValue().Order_Pay) //付款狀態
    })
});

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 180,
        border: 0,
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                margin: '5 0 0 5',
                fieldLabel: "查詢條件",
                items: [
                    {
                        xtype: 'combobox',
                        allowBlank: true,
                        editable: false,
                        hidden: false,
                        id: 'select_type',
                        name: 'select_type',
                        store: searchStatusrStore,
                        displayField: 'txt',
                        valueField: 'value',
                        typeAhead: true,
                        forceSelection: false,
                        margin: '0 10 0 5',
                        emptyText: '所有訂單資料'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: "查詢內容",
                        width: 200,
                        labelWidth: 60,
                        margin: '0 10 0 0',
                        id: 'search_con',
                        name: 'search_con'
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                margin: '5 0 0 5',
                fieldLabel: '日期條件',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'combobox',
                        id: 'timeone',
                        name: 'timeone',
                        fieldLabel: "",
                        margin: '0 10 0 5',
                        store: dateoneStore,
                        displayField: 'txt',
                        valueField: 'value',
                        labelWidth: 80,
                        value: 0
                    },
                    {
                        xtype: "datetimefield",
                        labelWidth: 60,
                        margin: '0 0 0 0',
                        id: 'dateOne',
                        name: 'dateOne',
                        format: 'Y-m-d',
                        allowBlank: false,
                        submitValue: true,
                        value: '2010-01-01'
                    },
                    {
                        xtype: 'displayfield',
                        margin: '0 0 0 0',
                        value: "~"
                    },
                    {
                        xtype: "datetimefield",
                        format: 'Y-m-d',
                        id: 'dateTwo',
                        name: 'dateTwo',
                        margin: '0 0 0 0',
                        allowBlank: false,
                        submitValue: true,
                        value: Tomorrow()
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                fieldLabel: '付款單狀態',
                margin: '5 0 0 5',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'combobox',
                        id: 'a',
                        name: 'a',
                        store: paymentType,
                        fieldLabel: "",
                        width: 200,
                        labelWidth: 80,
                        margin: '0 10 0 5',
                        displayField: 'remark',
                        valueField: 'ParameterCode',
                        emptyText: '付款單狀態'
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                margin: '5 0 0 5',
                fieldLabel: "付款方式",
                items: [
                    {
                        xtype: 'combobox',
                        allowBlank: true,
                        hidden: false,
                        id: 'order_payment',
                        name: 'order_payment',
                        store: PayTypeStore,
                        queryMode: 'local',
                        width: 200,
                        labelWidth: 80,
                        margin: '0 10 0 5',
                        displayField: 'txt',
                        valueField: 'value',
                        typeAhead: true,
                        forceSelection: false,
                        editable: false,
                        emptyText: '所有方式'
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                margin: '5 0 0 5',
                fieldLabel: "付款狀態",
                items: [
                    {
                        xtype: 'fieldcontainer',
                        combineErrors: true,
                        margin: '0 0 0 5',
                        fieldLabel: '',
                        width: 350,
                        layout: 'hbox',
                        items: [
                            {
                                xtype: 'radiogroup',
                                id: 'order_pay',
                                name: 'order_pay',
                                colName: 'order_pay',
                                width: 320,
                                defaults: {
                                    name: 'Order_Pay'
                                },
                                columns: 4,
                                items: [
                                    { id: 'OP1', boxLabel: "所有狀態", inputValue: '-1', checked: true, width: 80 },
                                    { id: 'OP2', boxLabel: "已付款", inputValue: '1', width: 80 },
                                    { id: 'OP3', boxLabel: "未付款", inputValue: '2', width: 80 }
                                ]
                            }
                        ]
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                fieldLabel: '',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'button',
                        text: '送出',
                        iconCls: 'icon-search',
                        margin: '5 0 0 5',
                        id: 'btnQuery',
                        handler: Query
                    }
                ]
            }
        ]
    });

    //頁面加載時創建grid
    var OrderBrandProducesListGrid = Ext.create('Ext.grid.Panel', {
        id: 'OrderBrandProducesListGrid',
        store: OrderBrandProducesListStore,
        height: 600,
        columnLines: true,
        frame: true,
        columns: [
            {
                header: "付款單號", dataIndex: 'Order_Id', width: 120, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value != null) {
                        return Ext.String.format("<a id='fkdh" + record.data.Order_Id + "' href='{0}' target='_blank' style='text-decoration:none'>{1}</a>", 'order_show.php?oid='+record.data.Order_Id, record.data.Order_Id);
                    }
                }
            },
            {
                header: "訂購人",
                dataIndex: 'Order_Name',
                width: 110,
                align: 'center',
                renderer: function (value,cellmeta,record,rowIndex,columnIndex,store) {
                    return "<a href='javascript:void(0);' onclick='UpdateUser()'>" + value + "</a>";
                }
            },
            { header: "收貨人", dataIndex: 'Delivery_Name', width: 110, align: 'center' },
            { header: "訂單應收金額", dataIndex: 'Order_Amount', width: 90, align: 'center' },
            {
                header: "付款方式",
                dataIndex: 'Order_Payment',
                width: 205,
                align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (value) {
                        case "1":
                            return ' 聯合信用卡 ';
                            break;
                        case "2":
                            return ' 永豐ATM ';
                            break;
                        case "3":
                            return ' 藍新信用卡 ';
                            break;
                        case "4":
                            return ' 支付寶 ';
                            break;
                        case "5":
                            return ' 銀聯 ';
                            break;
                        case "6":
                            return ' 傳真刷卡 ';
                            break;
                        case "7":
                            return ' 延遲付款';
                            break;
                        case "8":
                            return ' 黑貓貨到付款 ';
                            break;
                        case "9":
                            return ' 現金 ';
                            break;
                        case "10":
                            return ' 中國信託信用卡 ';
                            break;
                        case "11":
                            return ' 中國信託信用卡紅利折抵 ';
                            break;
                        case "12":
                            return '中國信託信用卡紅利折抵(10%) ';
                            break;
                        case "13":
                            return ' 網際威信  HiTRUST 信用卡 ';
                            break;
                        case "14":
                            return ' 中國信託信用卡紅利折抵(20%) ';
                            break;
                        case "15":
                            return ' 7-11貨到付款 ';
                            break;
                        default:
                            return "數據異常";
                            break;
                    }
                }
            },
            {
                header: "訂單狀態", dataIndex: 'Order_Status', width: 120, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (value) {
                        case "0":
                            return ' 等待付款 ';
                            break;
                        case "1":
                            return '付款失敗 ';
                            break;
                        case "2":
                            return '<font color="green"> 待出貨 </font>';
                            break;
                        case "3":
                            return ' 出貨中 ';
                            break;
                        case "4":
                            return ' 已出貨 ';
                            break;
                        case "5":
                            return ' 處理中 ';
                            break;
                        case "6":
                            return ' 進倉中 ';
                            break;
                        case "7":
                            return ' 已進倉 ';
                            break;
                        case "8":
                            return ' 已分配 ';
                            break;
                        case "10":
                            return ' 等待取消 ';
                            break;
                        case "20":
                            return ' 訂單異常 ';
                            break;
                        case "89":
                            return ' 單一商品取消 ';
                            break;
                        case "90":
                            return ' 訂單取消 ';
                            break;
                        case "91":
                            return ' 訂單退貨 ';
                            break;
                        case "92":
                            return ' 訂單換貨 ';
                            break;
                        case "99":
                            return ' 訂單歸檔 ';
                            break;
                        default:
                            return "數據異常";
                            break;
                    }
                }
            },
            { header: "訂單日期", dataIndex: 'ordercreatedate', width: 150, align: 'center' },
            { header: "管理員備註", dataIndex: 'Note_Admin', width: 260, align: 'center' },
            {
                header: "來源ID", dataIndex: 'redirect_name', width: 280, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.redirect_name != "") {
                        return Ext.String.format('<a href="{0}" target="_blank">{1}</a>', record.data.redirect_url, record.data.redirect_name);
                    }
                }
            }
        ],
        tbar: [
            { xtype: 'button', id: 'Add', text: ADD, iconCls: 'icon-add', hidden: true, handler: onAddClick },
            { xtype: 'button', id: 'Edit', text: EDIT, iconCls: 'icon-edit', hidden: true, disabled: true, handler: onEditClick },
            { xtype: 'button', id: 'Remove', text: REMOVE, iconCls: 'icon-remove', hidden: true, disabled: true, handler: onRemoveClick }
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        bbar: Ext.create('Ext.PagingToolbar', {
            store: OrderBrandProducesListStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        })
    });

    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [frm, OrderBrandProducesListGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                OrderBrandProducesListGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });

    ToolAuthority();
    OrderBrandProducesListStore.load({ params: { start: 0, limit: 25 } });
})

function UpdateUser() {
    var row = Ext.getCmp("OrderBrandProducesListGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], OrderBrandProducesListStore);
    }
}

onAddClick = function () {
    editFunction(null, OrderBrandProducesListStore);
}

onEditClick = function () {
    var row = Ext.getCmp("OrderBrandProducesListGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], OrderBrandProducesListStore);
    }
}

onRemoveClick = function () {
    var row = Ext.getCmp("OrderBrandProducesListGrid").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.id + '|';
                }
                Ext.Ajax.request({
                    url: '/Promotions/DeletePromotionsAmountGift',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        if (result.success) {
                            SinopacedtailListStore.load(1);
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


/************匯入到Excel**ATM************/
function Export() {
    Ext.Ajax.request({
        url: "/SinopacList/Export",
        success: function (response) {
            if (response.responseText == "true") {
                window.location = '../../ImportUserIOExcel/sinpac_list.csv';
            }
        }
    });
}
//查询
Query = function () {
    OrderBrandProducesListStore.removeAll();
    Ext.getCmp("OrderBrandProducesListGrid").store.loadPage(1, {
        params: {
            selecttype: Ext.getCmp('select_type').getValue(), //選擇查詢種類，訂單編號，訂購姓名
            searchcon: Ext.getCmp('search_con').getValue(), //查詢的內容
            timeone: Ext.getCmp('timeone').getValue(),
            dateOne: Ext.getCmp('dateOne').getValue(),
            dateTwo: Ext.getCmp('dateTwo').getValue(),
            slave_status: Ext.htmlEncode(Ext.getCmp("slave_status").getValue().Slave_Status), //付款單狀態，付款失敗，已發貨
            order_payment: Ext.htmlEncode(Ext.getCmp("order_payment").getValue().Order_Payment), //付款方式，AT
            order_pay: Ext.htmlEncode(Ext.getCmp("order_pay").getValue().Order_Pay) //付款狀態
        }
    });
}