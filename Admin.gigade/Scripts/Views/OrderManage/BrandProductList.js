//品牌訂單查詢
Ext.Loader.setConfig({ enabled: true });
var info_type = "order_master1";
var secret_info = "order_id;order_name";
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 22;
Ext.define('GIGADE.OrderBrandProduces', {
    extend: 'Ext.data.Model',
    fields: [
              { name: "order_id", type: "int" },//付款單號
              { name: "slave_id", type: "int" },//訂單編號
              { name: "detail_id", type: "int" },//商品單號
              { name: "item_id", type: "int" },//商品細項編號
              { name: "vendor_name_simple", type: "string" }, //供應商名稱
              { name: "brand_name", type: "string" }, //品牌名稱
              { name: "product_name", type: "string" },//商品名稱
              { name: "product_spec_name", type: "string" },//規格
              { name: "item_mode", type: "int" },//商品類型
              { name: "payments", type: "string" },//付款方式
              { name: "order_payment", type: "int" },//付款方式
              { name: "event_cost", type: "int" },//成本
              { name: "single_cost", type: "int" },//成本
              { name: "single_money", type: "int" }, //實際售價
              { name: "buy_num", type: "int" }, //數量
              { name: "parent_num", type: "int" },//組合數量
              { name: "deduct_bonus", type: "int" },//購物金
              { name: "deduct_welfare", type: "int" },//購物金
              { name: "order_name", type: "string" },//訂購姓名
              { name: "states", type: "string" },//狀態
              { name: "slave_status", type: "int" }, //狀態
              { name: "order_createdates", type: "string" },//建立日期
              { name: "order_date_pays", type: "string" }, //付款日期
              { name: "slave_date_deliverys", type: "string" }, //出貨日期
              { name: "note_order", type: "string" },//備註
              { name: "SingleMoney", type: "int" },//備註
              { name: "user_id", type: "int" }//備註
    ]
});

//獲取grid中的數據
var OrderBrandProducesListStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.OrderBrandProduces',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetOrderBrandProduces',
        actionMethods: 'post',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
            root: 'data'
        }
    }
});

Ext.define('gigade.Users', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "user_id", type: "int" }, //用戶編號     上面的是編輯的時候關係到的
        { name: "user_email", type: "string" }, //用戶郵箱
        { name: "user_name", type: "string" }, //用戶名
        { name: "user_password", type: "string" }, //密碼
        { name: "user_gender", type: "string" }, //性別
        { name: "user_birthday_year", type: "string" }, //年
        { name: "user_birthday_month", type: "string" }, //月
        { name: "user_birthday_day", type: "string" }, //日
        { name: "birthday", type: "string" }, //生日 
        { name: "user_zip", type: "string" }, //用戶地址
        { name: "user_address", type: "string" }, //用戶地址
        { name: "user_actkey", type: "string" },
        { name: "user_mobile", type: "string" },
        { name: "user_phone", type: "string" }, //行動電話
        { name: "reg_date", type: "string" }, //註冊日期 
        { name: "mytype", type: "string" },//會員類別
        { name: "send_sms_ad", type: "bool" }, //是否接收簡訊廣告 
        { name: "adm_note", type: "string" }, //管理員備註   上面這些編輯時要帶入的值
        { name: "user_type", type: "string" }, //用戶類別   下面的這些結合上面的會顯示在列表頁
        { name: "user_status", type: "string" }, //用戶狀態
        { name: "sfirst_time", type: "string" }, //首次註冊時間
        { name: "slast_time", type: "string" }, //下次時間
        { name: "sbe4_last_time", type: "string" }, //下下次時間
        { name: "user_company_id", type: "string" },
        { name: "user_source", type: "string" },
        { name: "source_trace", type: "string" },
        { name: "s_id", type: "string" },
        { name: "source_trace_url", type: "string" },
        { name: "redirect_name", type: "string" },
        { name: "redirect_url", type: "string" },
        { name: "paper_invoice", type: "bool" }
    ]
});

var searchStatusrStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": ALLSERACH, "value": "0" },
        { "txt": PARDUCTNAME, "value": "1" },
        { "txt": USERID, "value": "2" },
        { "txt": BRANDNAME, "value": "3" }
    ]
});
var DateTypeStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": ALLDATE, "value": "0" },
        { "txt": CREATEDATE, "value": "1" },
        { "txt": PAYDATE, "value": "2" }
    ]
});
OrderBrandProducesListStore.on('beforeload', function () {
    Ext.apply(OrderBrandProducesListStore.proxy.extraParams, {
        selecttype: Ext.getCmp('select_type').getValue(), //選擇查詢種類
        searchcon: Ext.getCmp('search_con').getValue(),
        date_type: Ext.getCmp('date_type').getValue(),
        dateOne: Ext.getCmp('dateOne').getValue(),
        dateTwo: Ext.getCmp('dateTwo').getValue(),
        slave_status: Ext.getCmp("slave_status").getValue(), //訂單狀態
        order_payment: Ext.getCmp("order_payment").getValue()//付款方式
    })

});
OrderBrandProducesListStore.on('load', function (store, records, options) {
    var totalcount = records.length;
    if (totalcount == 0) {
        Ext.MessageBox.alert(INFORMATION, SEARCHNONE);
    }

});
Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 120,
        border: 0,
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                margin: '5 0 0 5',               
                items: [
                    {
                        xtype: 'combobox',
                        allowBlank: true,
                        hidden: false,
                        fieldLabel: SEARCHTYPE,
                        id: 'select_type',
                        name: 'select_type',
                        store: searchStatusrStore,
                        queryMode: 'local',
                        labelWidth: 60,
                        displayField: 'txt',
                        valueField: 'value',
                        typeAhead: true,
                        forceSelection: false,
                        editable: false,
                        value: 0
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: SEARCHCON,
                        id: 'search_con',
                        name: 'search_con',
                        labelWidth: 60,
                        regex: /^((?!%).)*$/,
                        regexText: "禁止輸入百分號",
                        margin: '0 0 0 10',
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == Ext.EventObject.ENTER) {
                                    Query();
                                }
                            }
                        }
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                margin: '5 0 0 5',
              
                layout: 'hbox',
                items: [
                    {
                        xtype: 'combobox',
                        id: 'slave_status',
                        name: 'slave_status',
                        fieldLabel: SLAVESTATUS,
                        store: paymentStore,
                        labelWidth: 60,
                        queryMode: 'local',
                        displayField: 'remark',
                        editable: false,
                        typeAhead: true,
                        forceSelection: false,
                        valueField: 'ParameterCode',
                        emptyText: "所有狀態",
                        listeners: {
                            beforerender: function () {
                                paymentStore.load();
                            }
                        }
                    },
                    {
                        xtype: 'combobox',
                        id: 'order_payment',
                        name: 'order_payment',
                        store: paymentType,
                        fieldLabel: PAYTYPE,
                        queryMode: 'local',
                        labelWidth: 60,
                        margin: '0 0 0 10',
                        editable: false,
                        typeAhead: true,
                        forceSelection: false,
                        displayField: 'parameterName',
                        valueField: 'ParameterCode',
                        emptyText: "不分",
                        listeners: {
                            beforerender: function () {
                                paymentType.load();
                            }
                        }
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                margin: '5 0 0 5',
                items: [
                    {
                        xtype: 'combobox',
                        id: 'date_type',
                        name: 'date_type',
                        fieldLabel: DATETYPE,
                        store: DateTypeStore,
                        displayField: 'txt',
                        labelWidth: 60,
                        valueField: 'value',
                        editable: false,
                        value: 1
                    },
                    {
                        xtype: "datefield",
                        labelWidth: 60,
                        margin: '0 0 0 10',
                        id: 'dateOne',
                        name: 'dateOne',
                        format: 'Y-m-d',
                        allowBlank: false,
                        editable: false,
                        submitValue: true,
                        value: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                        listeners: {
                            select: function (a, b, c) {
                                var Month = new Date(this.getValue()).getMonth() + 1;
                                Ext.getCmp("dateTwo").setValue(new Date(new Date(this.getValue()).setMonth(Month)));
                            }
                        }
                    },
                    { xtype: 'displayfield',margin: '0 0 0 0',value: "~" },
                    {
                        xtype: "datefield",
                        format: 'Y-m-d',
                        id: 'dateTwo',
                        name: 'dateTwo',
                        margin: '0 0 0 0',
                        allowBlank: false,
                        editable: false,
                        submitValue: true,
                        value: new Date(),
                        listeners: {
                            select: function (a, b, c) {
                                var start = Ext.getCmp("dateOne");
                                var end = Ext.getCmp("dateTwo");
                                var s_date = start.getValue();
                                if (end.getValue() < start.getValue()) {
                                    Ext.Msg.alert("提示信息", "開始時間不能大於結束時間！");
                                    end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
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
                items: [
                    {
                        xtype: 'button',
                        text: SEARCH,
                        iconCls: 'icon-search',
                        margin: '0 0 10 5',
                        id: 'btnQuery',
                        handler: Query
                    },               
                    {
                        xtype: 'button',
                        text: RESET,
                        margin: '0 0 0 5',
                        id: 'btn_reset',
                        iconCls: 'ui-icon ui-icon-reset',
                        listeners: {
                            click: function () {
                                Ext.getCmp("select_type").setValue(0);
                                Ext.getCmp("search_con").setValue("");
                                Ext.getCmp("date_type").setValue(1);
                                Ext.getCmp("slave_status").setValue(-1);
                                Ext.getCmp("order_payment").setValue(0);
                                Ext.getCmp("dateOne").setValue(new Date(Today().setMonth(Today().getMonth() - 1)));
                                Ext.getCmp("dateTwo").setValue(Today());
                            }
                        }
                    },
                    {
                        xtype: 'button',
                        text: '匯出',
                        margin: '0 0 0 5',
                        iconCls: 'icon-excel',
                        id: 'btnExcel',
                        hidden:true,//添加權限
                        handler: Export
                    }
                ]
            }
        ]
    });
    //頁面加載時創建grid
    var OrderBrandProducesListGrid = Ext.create('Ext.grid.Panel', {
        id: 'OrderBrandProducesListGrid',
        store: OrderBrandProducesListStore,
        flex: 8.1,
        columnLines: true,
        frame: true,
        columns: [
            {
                header: ORDERID, dataIndex: 'order_id', width: 100, align: 'center',
                //renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                //    if (value != null) {
                //        return '<a href=javascript:TransToOrder(' + record.data.order_id + ') >' + record.data.order_id + '</a>';
                //    }
                //}
            },
            { header: BRANDNAME, dataIndex: 'brand_name', width: 100, align: 'center' },
            { header: PRODUCTNAMES, dataIndex: 'product_name', width: 150, align: 'center' },
            { header: PRODUCTSPECNAME, dataIndex: 'product_spec_name', width: 50, align: 'center' },
            { header: PAYTYPE, dataIndex: 'payments', width: 100, align: 'center' },
            {//進貨價
                header: SINGLEPRICE, dataIndex: 'event_cost', width: 50, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 0) {
                        return record.data.single_cost;
                    }
                    return value;
                }
            },
            { header: SINGLEMONEY, dataIndex: 'single_money', width: 50, align: 'center' },//實際售價
            { header: BUYNUM, dataIndex: 'buy_num', width: 50, align: 'center' },
            {//小計
                header: SUBTOTAL, dataIndex: '', width: 50, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.item_mode == 2) {
                        return (record.data.SingleMoney * record.data.parent_num);
                    }
                    else {
                        return record.data.single_money * record.data.buy_num;
                    }
                }
            },
            { header: BONUS, dataIndex: 'deduct_bonus', width: 100, align: 'center' },
            {
                header: ORDERNAME, dataIndex: 'order_name', width: 100, align: 'center',
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return "<a href='javascript:void(0);'  onclick='onUserEditClick()'>" + value.substr(0,1) + "**</a>";
                }},
            {
                header: STATES, dataIndex: 'states', width: 100, align: 'center',             
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {                   
                    if (record.data.slave_status != 2) {
                        return value;
                    }
                    else {
                        return '<font color="green">待出貨</font>';
                    }                                
                }
            },
            {
                header: EVENTS, dataIndex: 'events', width: 50, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.event_cost != 0 ) {//&& record.data.single_cost != record.data.single_money
                        return "是";
                    }
                    else {
                        return "-";
                    }
                }
            },
            { header: CREATEDATE, dataIndex: 'order_createdates', width: 120, align: 'center' },
            {
                header: PAYDATE, dataIndex: 'order_date_pays', width: 120, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.order_date_pays == '1970-01-01 08:00:00') {
                        return '<font color="red"> 未付款 </font>';
                    }
                    else {
                        return record.data.order_date_pays;
                    }
                }
            },
            {
                header: SLAVEDATEDELIVERYS, dataIndex: 'slave_date_deliverys', width: 120, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.slave_date_deliverys == '1970-01-01 08:00:00') {
                        return "-";
                    }
                    else {
                        return record.data.slave_date_deliverys;
                    }
                }
            },
            { header: NOTEORDER, dataIndex: 'note_order', width: 120, align: 'center' }
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
    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
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
   // OrderBrandProducesListStore.load({ params: { start: 0, limit: 22} });
})

/************匯入到Excel**************/
function Export() {
    window.open("/OrderManage/OrderBrandProducesExport?selecttype=" + Ext.getCmp('select_type').getValue() + "&searchcon=" + Ext.getCmp('search_con').getValue() + "&date_type=" + Ext.getCmp('date_type').getValue() + "&dateOne=" + Ext.Date.format(new Date(Ext.getCmp('dateOne').getValue()), 'Y-m-d') + "&dateTwo=" + Ext.Date.format(new Date(Ext.getCmp('dateTwo').getValue()), 'Y-m-d') + "&slave_status=" + Ext.getCmp('slave_status').getValue() + "&order_payment=" + Ext.getCmp('order_payment').getValue());
}
//查询
Query = function () {
    var conten = Ext.getCmp('search_con');
    if (!conten.isValid()) {
        Ext.Msg.alert("提示", "查詢內容格式錯誤!");
        return;
    }
    var type = Ext.getCmp('select_type');
    if (type.getValue() != 0 && conten.getValue().trim() == "")
    {
        Ext.Msg.alert("提示","請輸入查詢內容!");
        return;
    }
    OrderBrandProducesListStore.removeAll();
    Ext.getCmp("OrderBrandProducesListGrid").store.loadPage(1, {
        params: {
            selecttype: Ext.getCmp('select_type').getValue(), //選擇查詢種類
            searchcon: Ext.getCmp('search_con').getValue(),
            date_type: Ext.getCmp('date_type').getValue(),
            dateOne: Ext.getCmp('dateOne').getValue(),
            dateTwo: Ext.getCmp('dateTwo').getValue(),
            slave_status: Ext.getCmp("slave_status").getValue(),
            order_payment: Ext.getCmp("order_payment").getValue()
        }
    });
}
function TransToOrder(orderId) {
    var url = '/OrderManage/OrderDetialList?Order_Id=' + orderId;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#BrandProductList');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'BrandProductList',
        title: '訂單內容',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}
function onUserEditClick() {
    var row = Ext.getCmp("OrderBrandProducesListGrid").getSelectionModel().getSelection();
    var secret_type = "20";//參數表中的"訂單"
    var url = "/OrderManage/BrandProductIndex";
    var ralated_id = row[0].data.order_id;
    var info_id = row[0].data.order_id;
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {
        if (boolPassword) {//驗證
            SecretLoginFun(secret_type, ralated_id, true, true, false, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框
        } else {
            SecretLoginFun(secret_type, ralated_id, false, true, false, url, info_type, info_id, secret_info);//直接彈出顯示框
        }
    }

    //editFunction(row[0], OrderUserReduceListStore);//user_id
    // editFunction(row[0].data.user_id);
}