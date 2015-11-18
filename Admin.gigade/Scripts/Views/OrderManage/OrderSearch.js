Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 20;
Ext.define('GIGADE.OrderBrandProduces', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "Order_Id", type: "int" }, //付款單號
        { name: "Order_Name", type: "string" }, //訂購人名稱
        { name: "Delivery_Name", type: "string" }, //收貨人
        { name: "Order_Amount", type: "int" }, //訂單應收金額 
        { name: "Order_Payment", type: "string" }, //付款方式
        { name: "Order_Status", type: "string" }, //訂單狀態
        { name: "ordercreatedate", type: "string" }, //建立日期
        { name: "channel_name_full", type: "string" }, //賣場
        { name: "Note_Admin", type: "string" }, //管理員備註
        { name: "redirect_name", type: "string" }, //來源ID
        { name: "redirect_url", type: "string" }, //來源ID連接
        { name: "Source_Trace", type: "string" },
        { name: "order_pay_message", type: "string" },//點數使用明細
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
        { name: "adm_note", type: "string" }, //管理員備註 
        { name: "orderStatus", type: "string" }, //訂單狀態
        { name: "payment", type: "string" }, //付款方式 
        { name: "export_flag_str", type: "string" } //ERP拋售狀態
    ]
});
//查詢條件
var searchStatusrStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有訂單資料", "value": "0" },
        { "txt": "訂單編號", "value": "1" },
        { "txt": "訂購人姓名", "value": "2" },
        { "txt": "訂購人信箱", "value": "3" },
        { "txt": "收貨人姓名", "value": "4" },
        { "txt": "來源ID", "value": "5" },
        { "txt": "訂購人手機", "value": "6" },
        { "txt": "外站訂單編號", "value": "7" },
        { "txt": "收貨人手機", "value": "8" },
        { "txt": "訂購人/收貨人地址", "value": "9" },
        { "txt": "訂購人/收貨人市話", "value": "10" },
        { "txt": "物流單號", "value": "11" }
        //{ "txt": "外站訂單資料", "value": "12" }

    ]
});
//日期類型
var dateoneStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有日期", "value": "0" },
        { "txt": "訂單日期", "value": "1" },
        { "txt": "可出貨(帳款實收)日期", "value": "2" }
    ]
});
//會員群組列表
Ext.define('gigade.Channel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "group_id", type: "string" },
        { name: "group_name", type: "string" }
    ]
});
var UserGroupStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'gigade.Channel',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetVipUserGroup',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
//獲取訂單內容
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
OrderBrandProducesListStore.on('beforeload', function () {
    Ext.apply(OrderBrandProducesListStore.proxy.extraParams, {
        selecttype: Ext.getCmp('select_type').getValue(), //選擇查詢種類，訂單編號，訂購姓名
        searchcon: Ext.getCmp('search_con').getValue(), //查詢的內容
        timeone: Ext.getCmp('timeone').getValue(),
        dateOne: Ext.getCmp('dateOne').getValue(),
        dateTwo: Ext.getCmp('dateTwo').getValue(),
        page_status: Ext.getCmp('a').getValue(),
//      slave_status: Ext.getCmp('a').getVlaue(), //付款單狀態，付款失敗，已發貨
        channel: Ext.getCmp('channel').getValue(), //賣場
        Vip_User_Group: Ext.getCmp('Vip_User_Group').getValue(), //會員群組
        order_payment: Ext.getCmp("order_payment").getValue(), //付款方式，AT
        order_pay: Ext.htmlEncode(Ext.getCmp("order_pay").getValue().Order_Pay), //付款狀態
        invoice: Ext.htmlEncode(Ext.getCmp("invoice").getValue().Invoice)//過濾條件
    })
});
//多選框
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("OrderBrandProducesListGrid").down('#Remove').setDisabled(selections.length == 0);
            Ext.getCmp("OrderBrandProducesListtGrid").down('#Edit').setDisabled(selections.length == 0);
        }
    }
});

//群組管理Model 
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
    { name: "user_level", type: "int" }, //會員等級
    { name: "userLevel", type: "string" }, //會員等級
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
    { name: "paper_invoice", type: "bool" },
    { name: "ml_code", type: "string" }
    ]
});
//用作編輯時獲得數據包含機敏信息
var edit_UserStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.Users',
    proxy: {
        type: 'ajax',
        url: '/Member/UsersList',
        reader: {
            type: 'json',
            root: 'data',//在執行成功后。顯示數據。所以record.data.用戶字段可以直接讀取
            totalProperty: 'totalCount'
        }
    }
});
Ext.define('gigade.UserLife', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "user_id", type: "int" },
    { name: "user_marriage", type: "int" },
    { name: "child_num", type: "int" },
    { name: "vegetarian_type", type: "int" },
    { name: "like_fivespice", type: "int" },
    { name: "like_contact", type: "string" },
    { name: "like_time", type: "int" },
    { name: "work_type", type: "int" },
    { name: "cancel_edm_date", type: "string" },
    { name: "disable_date", type: "string" },
    { name: "cancel_info_date", type: "string" },
    { name: "user_educated", type: "int" },
    { name: "user_salary", type: "int" },
    { name: "user_religion", type: "int" },
    { name: "user_constellation", type: "int" }

    ]
});
var UserLifeStore = Ext.create('Ext.data.Store', {
    model: 'gigade.UserLife',
    proxy: {
        type: 'ajax',
        url: '/Member/GetUserLife',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
edit_UserStore.on('beforeload', function () {
    Ext.apply(edit_UserStore.proxy.extraParams, {
        relation_id: "",
        isSecret: false
    });
});


Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 190,
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
                    value:0
                },
                {
                    xtype: 'textfield',
                    fieldLabel: "查詢內容",
                    width: 200,
                    labelWidth: 60,
                    margin: '0 10 0 0',
                    id: 'search_con',
                    name: 'search_con',
                    listeners: {
                        specialkey: function (field, e) {
                            if (e.getKey() == Ext.EventObject.ENTER) {
                                Query();
                            }
                        }
                    }
                }]
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
                    editable: false,
                    valueField: 'value',
                    labelWidth: 80,
                    value: 1
                },
                {
                    xtype: "datefield",
                    labelWidth: 60,
                    margin: '0 0 0 0',
                    id: 'dateOne',
                    name: 'dateOne',
                    format: 'Y-m-d',
                    allowBlank: false,
                    editable: false,
                    submitValue: true,
                    //value: new Date(Tomorrow().setDate(Tomorrow().getDay() - 2)),
                    value:new Date(Tomorrow().setMonth(Tomorrow().getMonth() - 1)),
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("dateOne");
                            var end = Ext.getCmp("dateTwo");
                            var s_date = new Date(end.getValue());
                            if (end.getValue() < start.getValue()) {
                                Ext.Msg.alert("提示", "開始時間不能大於結束時間！");
                                start.setValue(Tomorrow().setMonth(Tomorrow().getMonth() - 1));
                            }
                        },
                        specialkey: function (field, e) {
                            if (e.getKey() == Ext.EventObject.ENTER) {
                                Query();
                            }
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
                    editable: false,
                    allowBlank: false,
                    submitValue: true,
                    value: Tomorrow(),
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("dateOne");
                            var end = Ext.getCmp("dateTwo");
                            var s_date = new Date(start.getValue());
                            if (end.getValue() < start.getValue()) {
                                Ext.Msg.alert("提示", "開始時間不能大於結束時間！");
                                end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                            }
                        },
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
                fieldLabel: '付款單狀態',
                margin: '5 0 0 5',
                layout: 'hbox',
                items: [
                //{
                //    xtype: 'combobox',
                //    id: 'a',
                //    name: 'a',
                //    store: paymentType,
                //    fieldLabel: "",
                //    margin: '0 10 0 5',
                //    editable: false,
                //    displayField: 'remark',
                //    valueField: 'ParameterCode',
                //    emptyText: '付款單狀態'
                //},
                {//付款單狀態                    
                    xtype: 'combobox',
                    id: 'a',
                    name: 'a',
                    store: paymentStore,
                    queryMode: 'local',
                    editable: false,
                    margin: '0 10 0 5',
                    displayField: 'remark',
                    valueField: 'ParameterCode',
                    listeners: {
                        beforerender: function () {
                            paymentStore.load({
                                callback: function () {
                                    paymentStore.insert(0, { ParameterCode: '-1', remark: '所有狀態' });
                                    Ext.getCmp("a").setValue(paymentStore.data.items[0].data.ParameterCode);
                                }
                            });
                        }
                    }
                },
                {
                    xtype: 'combobox',
                    allowBlank: true,
                    fieldLabel: '賣場',
                    hidden: false,
                    id: 'channel',
                    name: 'channel',
                    store: ChannelStore,
                    margin: '0 10 0 5',
                    displayField: 'channel_name_simple',
                    valueField: 'channel_id',
                    typeAhead: true,
                    forceSelection: false,
                    editable: false,
                    emptyText: '全部'
                }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                fieldLabel: "付款方式",
                layout: 'hbox',
                margin: '5 0 0 5',
                items: [
                    {//付款方式                    
                        xtype: 'combobox',
                        id: 'order_payment',
                        name: 'order_payment',
                        queryMode: 'local',
                        margin: '0 10 0 5',
                        editable: false,
                        typeAhead: true,
                        forceSelection: false,
                        store: paymentType,
                        displayField: 'parameterName',
                        valueField: 'ParameterCode',
                        emptyText: '不分',
                        listeners: {
                            beforerender: function () {
                                paymentType.load();
                            }
                        }
                    },
                    {
                        xtype: 'fieldcontainer',
                        combineErrors: true,
                        margin: '0 0 0 5',
                        fieldLabel: '付款狀態',
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
                                    { id: 'OP1', boxLabel: "所有狀態", inputValue: '0', checked: true, width: 80 },
                                    { id: 'OP2', boxLabel: "已付款", inputValue: '1', width: 80 },
                                    { id: 'OP3', boxLabel: "未付款", inputValue: '2', width: 80 }
                                ]
                            }]                                 
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                margin: '5 0 0 5',
                fieldLabel: "群組會員",
                items: [
                    {
                        margin: '0 0 0 5',
                        xtype: 'combobox',
                        id: 'Vip_User_Group',
                        name: 'Vip_User_Group',
                        width: 200,
                        displayField: 'group_name',
                        valueField: 'group_id',
                        emptyText: '請選擇',
                        editable: false,
                        store: UserGroupStore
                    },
                    {
                        xtype: 'fieldcontainer',
                        combineErrors: true,
                        margin: '0 0 0 15',
                        fieldLabel: '過濾發票',
                        width: 350,
                        layout: 'hbox',
                        items: [
                            {
                                xtype: 'radiogroup',
                                id: 'invoice',
                                name: 'invoice',
                                colName: 'invoice',
                                width: 380,
                                defaults: {
                                    name: 'Invoice'
                                },
                                columns: 5,
                                items: [
                                    { id: 'P1', boxLabel: "全部", inputValue: '0', checked: true, width: 70 },
                                    { id: 'P2', boxLabel: "過濾已開發票", margin: '0 0 0 5', inputValue: '1', width: 130 }
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
                        text: '查詢',
                        iconCls: 'icon-search',
                        margin: '5 0 0 5',
                        id: 'btnQuery',
                        handler: Query
                    },
                    {
                        xtype: 'button',
                        text: '重置',
                        margin: '5 0 0 5',
                        iconCls: 'ui-icon ui-icon-reset',
                        id: 'btnchongzhi',
                        handler: comeback
                    },
                    {
                        xtype: 'button',
                        text: '匯出',
                        margin: '5 0 0 5',
                        iconCls: 'icon-excel',
                        id: 'btnExcel',
                        hidden:true,
                        handler: Export
                    },
                                        {
                                            xtype: 'button',
                                            text: '會計報表',
                                            margin: '5 0 0 10',
                                            iconCls: 'icon-excel',
                                            id: 'btnExcelReport',
                                            hidden: true,
                                            handler: ExportReport
                                        }
                ]
            }
        ]
    });
    var OrderBrandProducesListGrid = Ext.create('Ext.grid.Panel', {
        id: 'OrderBrandProducesListGrid',
        store: OrderBrandProducesListStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 8.1,
        columns: [
            {
                header: '付款單號', colName: 'Order_Id', xtype: 'templatecolumn',align: 'center', sortable: false,
                tpl: Ext.create('Ext.XTemplate',
                    '<tpl >',
                        '<a href=javascript:TranToDetial("{Order_Id}") >{Order_Id}</a>',
                    '</tpl>'),menuDisabled: true                 
            },
            {
                header: "訂購人", dataIndex: 'Order_Name', width: 110, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return "<a href='javascript:void(0);' onclick='oneditUser(" + record.data.user_id + ")'>" + record.data.Order_Name + "</a>";
                }
            },
            { header: "收貨人", dataIndex: 'Delivery_Name', width: 110, align: 'center' },
            {
                header: "訂單應收金額", dataIndex: 'Order_Amount', width: 90, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return change(value);
                }
            },
            { header: "付款方式", dataIndex: 'payment', width: 200, align: 'center' },
            {
                header: "訂單狀態", dataIndex: 'orderStatus', width: 120, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "待出貨") {
                        return '<font color="green"> 待出貨 </font>';
                    }
                    else {
                        return value;
                    }                  
                }
            },
            { header: "訂單日期", dataIndex: 'ordercreatedate', width: 150, align: 'center' },
            { header: "賣場", dataIndex: 'channel_name_full', width: 100, align: 'center' },
            { header: "管理員備註", dataIndex: 'Note_Admin', width: 220, align: 'center' },
            {
                header: "來源ID", dataIndex: 'redirect_name', width: 220, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.Source_Trace != "0") {
                        return Ext.String.format('<a href="{0}" target="_blank">{1}</a>', record.data.redirect_url, record.data.redirect_name);
                    }
                    else {
                        return "";
                    }
                }
            },
            { header: "點數使用明細", dataIndex: 'order_pay_message', width: 100, align: 'center' },
            { header: "ERP拋轉狀態", dataIndex: 'export_flag_str', width: 100, align: 'center', }
        ],
        //tbar: [
        //    { xtype: 'button', id: 'Add', text: ADD, iconCls: 'icon-add', hidden: true, handler: onAddClick },
        //    { xtype: 'button', id: 'Edit', text: EDIT, iconCls: 'icon-edit', hidden: true, disabled: true, handler: onEditClick }
        //],
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
   // OrderBrandProducesListStore.load({ params: { start: 0, limit: 20 } });
})
//查詢條件重置方法
function comeback() {
    Ext.getCmp('select_type').setValue(0);  //選擇查詢種類，訂單編號，訂購姓名
    Ext.getCmp('search_con').setValue();  //查詢的內容
    Ext.getCmp('timeone').setValue(1);
    Ext.getCmp("a").setValue(-1);  //付款單狀態，付款失敗，已發貨
    Ext.getCmp('channel').setValue();  //賣場
    Ext.getCmp("dateOne").setValue(new Date(Tomorrow().setMonth(Tomorrow().getMonth() - 1)));
    Ext.getCmp("dateTwo").setValue(Tomorrow());    
    Ext.getCmp('Vip_User_Group').setValue();  //會員群組
    Ext.getCmp("order_payment").setValue();  //付款方式，AT
    Ext.getCmp("P1").setValue(true);  //付款狀態
    Ext.getCmp("OP1").setValue(true);//過濾條件
}
//查询
Query = function () {
    if (Ext.getCmp('select_type').getValue() > 0 && Ext.getCmp('search_con').getValue() == "") {
        Ext.Msg.alert(INFORMATION, "請輸入內容！");
    }
    else {
        OrderBrandProducesListStore.removeAll();
        Ext.getCmp("OrderBrandProducesListGrid").store.loadPage(1, {
            params: {
                selecttype: Ext.getCmp('select_type').getValue(), //選擇查詢種類，訂單編號，訂購姓名
                searchcon: Ext.getCmp('search_con').getValue(), //查詢的內容
                timeone: Ext.getCmp('timeone').getValue(),
                dateOne: Ext.getCmp('dateOne').getValue(),
                dateTwo: Ext.getCmp('dateTwo').getValue(),
                page_status: Ext.getCmp('a').getValue(),
                //            slave_status: Ext.getCmp('a').getVlaue(), //付款單狀態，付款失敗，已發貨
                channel: Ext.getCmp('channel').getValue(), //賣場
                Vip_User_Group: Ext.getCmp('Vip_User_Group').getValue(), //會員群組
                order_payment: Ext.getCmp("order_payment").getValue(), //付款方式，AT
                order_pay: Ext.htmlEncode(Ext.getCmp("order_pay").getValue().Order_Pay), //付款狀態
                invoice: Ext.htmlEncode(Ext.getCmp("invoice").getValue().Invoice)//過濾條件
            }
        });
    }
}
//編輯訂購人
//function UpdateUser() {
//    var row = Ext.getCmp("OrderBrandProducesListGrid").getSelectionModel().getSelection();
//    if (row.length == 0) {
//        Ext.Msg.alert(INFORMATION, NO_SELECTION);    }
//    else if (row.length > 1) {
//        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
//    } else if (row.length == 1) {
//        editFunction(row[0], OrderBrandProducesListStore);
//    }
//}
////新增訂單
//onAddClick = function () {
//    editFunction(null, OrderBrandProducesListStore);
//}
////編輯訂單
//onEditClick = function () {
//    var row = Ext.getCmp("OrderBrandProducesListGrid").getSelectionModel().getSelection();
//    if (row.length == 0) {
//        Ext.Msg.alert(INFORMATION, NO_SELECTION);
//    }
//    else if (row.length > 1) {
//        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
//    } else if (row.length == 1) {
//        editFunction(row[0], OrderBrandProducesListStore);
//    }
//}

/************匯出Excel************/
function Export() {
   var selecttype= Ext.getCmp('select_type').getValue(); //選擇查詢種類，訂單編號，訂購姓名
   var searchcon= Ext.getCmp('search_con').getValue(); //查詢的內容
   var timeone= Ext.getCmp('timeone').getValue();
   var dateOne= Ext.getCmp('dateOne').getValue();
   var dateTwo= Ext.getCmp('dateTwo').getValue();
   var page_status= Ext.getCmp('a').getValue();
    //            slave_status: Ext.getCmp('a').getVlaue(), //付款單狀態，付款失敗，已發貨
   var channel= Ext.getCmp('channel').getValue(); //賣場
   var Vip_User_Group= Ext.getCmp('Vip_User_Group').getValue(); //會員群組
   var order_payment= Ext.getCmp("order_payment").getValue(); //付款方式，AT
   var order_pay= Ext.htmlEncode(Ext.getCmp("order_pay").getValue().Order_Pay); //付款狀態
   var invoice= Ext.htmlEncode(Ext.getCmp("invoice").getValue().Invoice);//過濾條件
    window.open("/OrderManage/OrderSerchExport?selecttype=" + selecttype + "&invoice=" + invoice + "&order_pay=" + order_pay + "&order_payment=" + order_payment + "&channel=" + channel + "&Vip_User_Group=" + Vip_User_Group + "&page_status=" + page_status + "&searchcon=" + searchcon + "&timeone=" + timeone + "&dateOne=" + Ext.Date.format(new Date(Ext.getCmp('dateOne').getValue()), 'Y-m-d') + "&dateTwo=" + Ext.Date.format(new Date(Ext.getCmp('dateTwo').getValue()), 'Y-m-d'));
}

function ExportReport() {
    var dateOne = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('dateOne').getValue()), 'Y-m-d 00:00:00'));
    var dateTwo = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('dateTwo').getValue()), 'Y-m-d 23:59:59'));
    window.open("/OrderManage/ExportReport?dateOne=" + dateOne + "&dateTwo=" + dateTwo);
}
function Tomorrow() {
    var d;
    d = new Date();
    d.setDate(d.getDate() + 1);
    return d;
}
//跳轉到訂單內容頁面
function TranToDetial(orderId) {
    var url = '/OrderManage/OrderDetialList?Order_Id=' + orderId;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#detial');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'detial',
        title: '訂單內容',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}
oneditUser = function (user_id) {
    var row = Ext.getCmp('OrderBrandProducesListGrid').getSelectionModel().getSelection();
   // alert(row[0].data.user_id);
    //editFunction(user_id);
    var secret_type = '20';
    var url = "/Member/UsersListIndex/Edit ";
    var ralated_id = row[0].data.user_id;
    var info_id = row[0].data.user_id;
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {
        if (boolPassword) {
            SecretLoginFun(secret_type, ralated_id, true, false, true, url, "", info_id, "");//先彈出驗證框，關閉時在彈出顯示框
        }
        else {
            editFunction(ralated_id);
        }
    }
}