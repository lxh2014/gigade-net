//供應商訂單查詢
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
//聲明grid
Ext.define('GIGADE.OrderBrandProduces', {
    extend: 'Ext.data.Model',
    fields: [
              { name: "order_id", type: "int" }, //付款單號
              { name: "vendor_name_simple", type: "string" }, //供應商名稱
              { name: "pic_patch", type: "string" }, //圖示連接
              { name: "Product_Name", type: "string" }, //商品名稱
              { name: "Product_Spec_Name", type: "string" },
              { name: "item_mode", type: "int" },//商品類型
              { name: "parent_num", type: "int" },
              { name: "payment", type: "string" }, //付款方式
              { name: "order_payment", type: "int" }, //付款方式
              { name: "Single_Money", type: "int" }, //實際售價
              { name: "Single_Cost", type: "int" },
              { name: "Buy_Num", type: "int" }, //數量
              { name: "Event_Cost", type: "int" },
              //{ name: "accumulated_bonus", type: "int" }, //購物金
              { name: "order_name", type: "string" }, //訂購姓名
              { name: "slave_status", type: "int" }, //狀態
              { name: "slave", type: "string" }, //狀態
              { name: "Deduct_Bonus", type: "int" },//購物金
              { name: "order_createdate", type: "string" }, //建立日期
              { name: "money_collect_date", type: "string" }, //付款日期
              { name: "slave_date_delivery", type: "string" }, //出貨日期
              { name: "product_manage", type: "string" },//管理者
              { name: "note_order", type: "string" },//備註
               { name: "SingleMoney", type: "int" },//備註
                {name:"user_id",type:"int"}
    ]
});
//獲取grid中的數據
var OrderBrandProducesListStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.OrderBrandProduces',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetOrderVendorProduces',
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
secret_info = "user_id;user_email;user_name;user_mobile";
var edit_UserStore = Ext.create('Ext.data.Store', {
    //  autoDestroy: true,
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
        { "txt": ALLSERACH, "value": "0" },
        { "txt": PARDUCTNAME, "value": "1" },
        { "txt": USERID, "value": "2" },
        { "txt": VENDORNAME, "value": "3" },
        { "txt": PRODUCTID, "value": "4" },
        { "txt": '付款單號', "value": "5" }
    ]
});
var dateStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": ALLDATE, "value": "0" },
        { "txt": CREATEDATE, "value": "1" },
        { "txt": PAYDATE, "value": "2" },
        { "txt": ISDELIVERY, "value": "3" },
        { "txt": DELIVERY, "value": "4" },
        { "txt": FDATE, "value": "5" },
    ]
});
OrderBrandProducesListStore.on('beforeload', function () {  
    Ext.apply(OrderBrandProducesListStore.proxy.extraParams, {
        selecttype: Ext.getCmp('select_type').getValue(), //選擇查詢種類
        searchcon: Ext.getCmp('search_con').getValue(),
        datetype: Ext.getCmp('datetype').getValue(),
        Vendor_Id: Ext.getCmp('Vendor').getValue(),
        dateStart: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('dateStart').getValue()), 'Y-m-d H:i:s')),
        dateEnd: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('dateEnd').getValue()), 'Y-m-d H:i:s')),
        product_freight_set: Ext.getCmp('product_freight_set').getValue(),
        product_manage: Ext.getCmp('product_manage').getValue(), //供應商管理人員
        order_status: Ext.getCmp("order_status").getValue(), //訂單狀態
        order_payment: Ext.getCmp("order_payment").getValue()//付款方式
    })
});

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 140,
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
                    fieldLabel: "供應商",
                    editable: false,
                    hidden: false,
                    queryMode: 'local',
                    id: 'Vendor',
                    name: 'Vendor',
                    store: VendorStore,
                    displayField: 'vendor_name_simple',
                    valueField: 'vendor_id',
                    typeAhead: true,
                    forceSelection: false,
                    listeners: {
                        beforerender: function () {
                            VendorStore.load({
                                callback: function () {
                                    VendorStore.insert(0, { vendor_id: '0', vendor_name_simple: '所有供應商資料' });
                                    Ext.getCmp("Vendor").setValue(VendorStore.data.items[0].data.vendor_id);
                                }
                            });
                        }
                    }
                },
                {
                    xtype: 'combobox',
                    allowBlank: true,
                    fieldLabel: KEYWORD,
                    hidden: false,
                    id: 'select_type',
                    name: 'select_type',
                    store: searchStatusrStore,
                    queryMode: 'local',
                    width: 250,
                    labelWidth: 100,
                    margin: '0 10 0 5',
                    displayField: 'txt',
                    valueField: 'value',
                    typeAhead: true,
                    forceSelection: false,
                    editable: false,
                    value: 0
                },
                {
                    xtype: 'textfield',
                    labelWidth: 80,
                    margin: '0 10 0 0',
                    id: 'search_con',
                    name: 'search_con',
                    regex: /^((?!%).)*$/,
                    regexText: "禁止輸入百分號",
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
                layout: 'hbox',
                margin: '0 0 0 5',
                items: [
                {
                    xtype: 'combobox',
                    id: 'datetype',
                    name: 'datetype',
                    fieldLabel: DATETYPE,
                    store: dateStore,
                    displayField: 'txt',
                    valueField: 'value',
                    margin: '0 10 0 0',                   
                    editable: false,
                    value: 1
                },
                {
                    xtype: "datefield",
                    format: 'Y-m-d',
                    labelWidth: 60,
                    id: 'dateStart',
                    name: 'dateStart',
                    editable: false,
                    value: new Date(Today().setMonth(Today().getMonth() - 1)),
                    listeners: {
                        select: function (a, b, c) {
                            var Month = new Date(this.getValue()).getMonth() + 1;
                            Ext.getCmp("dateEnd").setValue(new Date(new Date(this.getValue()).setMonth(Month)));
                        }
                    }
                },
                {
                    xtype: 'displayfield',
                    margin: '0 25 0 25',
                    value: "~"
                },
                {
                    xtype: "datefield",
                    format: 'Y-m-d',
                    id: 'dateEnd',
                    name: 'dateEnd',
                    value: Today(),
                    editable: false,
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("dateStart");
                            var end = Ext.getCmp("dateEnd");
                            var s_date = new Date(start.getValue());
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
                margin: '0 0 0 5',
                fieldLabel: SLAVESTATUS,
                layout: 'hbox',
                items: [
                {//訂單狀態                    
                    xtype: 'combobox',
                    id: 'order_status',
                    name: 'order_status',
                    store: paymentStore,
                    queryMode: 'local',
                    labelWidth: 80,
                    editable: false,
                    displayField: 'remark',
                    valueField: 'ParameterCode',
                    emptyText: "所有狀態",
                    listeners: {
                        beforerender: function () {
                            paymentStore.load();
                        }
                    }
                },
                {//付款方式                    
                    xtype: 'combobox',
                    fieldLabel: PAYTYPE,
                    id: 'order_payment',
                    name: 'order_payment',
                    store: paymentType,
                    queryMode: 'local',
                    margin: '0 10 0 5',
                    editable: false,
                    displayField: 'parameterName',
                    valueField: 'ParameterCode',
                    emptyText: "不分",
                    listeners: {
                        beforerender: function () {
                            paymentType.load();
                        }
                    }
                }]                
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                margin: '0 0 0 5',
                fieldLabel: DELIVERYTYPE,
                items: [
                    {
                        xtype: 'combobox',
                        hidden: false,
                        id: 'product_freight_set',
                        name: 'product_freight_set',
                        store: FreightSetStore,
                        queryMode: 'local',
                        labelWidth: 80,
                        displayField: 'parameterName',
                        valueField: 'ParameterCode',
                        typeAhead: true,
                        multiSelect: true, //多選
                        forceSelection: false,
                        editable: false,
                        emptyText: "請選擇",
                        listeners: {
                            beforerender: function () {
                                FreightSetStore.load();
                            }
                        }
                    },
                    {
                        xtype: 'combobox',
                        allowBlank: true,
                        fieldLabel: VMANAGE,
                        hidden: false,
                        id: 'product_manage',
                        name: 'product_manage',
                        store: ProductManageStore,
                        labelWidth: 100,
                        margin: '0 10 0 5',
                        displayField: 'userName',
                        valueField: 'userId',
                        typeAhead: true,
                        forceSelection: false,
                        editable: false,
                        emptyText: ALLMANAGE
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                margin: '0 0 0 5',
                items: [
                    {
                        xtype: 'button',
                        text: "查詢",
                        iconCls: 'icon-search',
                        margin: '0 0 0 5',
                        id: 'btnQuery',
                        hidden: true,
                        handler: Query
                    },
                    {
                        xtype: 'button',
                        text: "重置",
                        iconCls: 'ui-icon ui-icon-reset',
                        margin: '0 0 0 5',
                        id: 'btnresert',
                        listeners: {
                            click: function () {
                                Ext.getCmp("datetype").setValue(1);
                                Ext.getCmp("Vendor").setValue(0);
                                Ext.getCmp("select_type").setValue(0);
                                Ext.getCmp("order_status").setValue(-1);
                                Ext.getCmp("order_payment").setValue(0);
                                Ext.getCmp("search_con").setValue("");
                                Ext.getCmp("product_freight_set").setValue("");
                                Ext.getCmp("product_manage").setValue("");
                                Ext.getCmp('dateStart').setValue(new Date(Today().setMonth(Today().getMonth() - 1)));
                                Ext.getCmp('dateEnd').setValue(Today());
                            }
                        }
                    },                    
                    {
                        xtype: 'button',
                        text: '匯出Excel',
                        margin: '0 0 0 5',
                        iconCls: 'icon-excel',
                        id: 'btnExcel',
                        hidden:true,
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
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 8.1,
        columns: [
            {
                header: ORDERID, dataIndex: 'order_id', width: 70, align: 'center',
                //renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                //    if (value != null) {
                //        return '<a href=javascript:TransToOrder(' + record.data.order_id + ') >' + record.data.order_id + '</a>';
                //    }
                //}
            },
            { header: VENDOR, dataIndex: 'vendor_name_simple', width: 60, align: 'center' },
            {
                header: PRODUCTNAMES, dataIndex: '', width: 80, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.pic_patch != '') {
                        return Ext.String.format("<a  href='{0}' target='_blank'><font color='#00F'>{1}</font></a>", record.data.pic_patch, record.data.Product_Name);
                    }
                    else {
                        return record.data.Product_Name;
                    }
                }
            },
            { header: PRODUCTSPECNAME, dataIndex: 'Product_Spec_Name', width: 60, align: 'center' },
            { header: PAYTYPE, dataIndex: 'payment', width: 80, align: 'center' },
            {
                header: ITEMMODE, dataIndex: 'item_mode', width: 60, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (value) {
                        case 1: return FPRODUCT;//父商品
                            break;
                        case 2: return SPRODUCT;//子商品
                            break;
                        default: return SIPRODUCT;
                            break;
                    }
                }
            },
            {//進貨價
                header: SINGLEPRICE, dataIndex: 'Event_Cost', width: 70, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value==0) {
                        return record.data.Single_Cost;
                    }
                    else {
                        return record.data.Event_Cost;
                    }
                }
            },
            { header: SINGLEMONEY, dataIndex: 'Single_Money', width: 60, align: 'center' },//實際售價
            { header: BUYNUM, dataIndex: 'Buy_Num', width: 60, align: 'center' },
      
            {//小計
                header: SUBTOTAL, dataIndex: '', width: 50, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {

                    if (record.data.item_mode == 2) {
                        return (record.data.SingleMoney * record.data.parent_num);
                    }
                    else {
                        return record.data.Single_Money * record.data.Buy_Num;
                    }
                }
            },
            { header: BONUS, dataIndex: 'Deduct_Bonus', width: 100, align: 'center' },
            {
                header: ORDERNAME, dataIndex: 'order_name', width: 100, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return "<a href='javascript:void(0);'  onclick='onUserEditClick()'>" + value.substr(0,1) + "**</a>";
                }
            },
            { header: STATES, dataIndex: 'slave', width: 120, align: 'center' },
            {
                header: EVENTS, dataIndex: 'event', width: 50, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.Event_Cost != 0) {
                        return YES;
                    }
                    else {
                        return "-";
                    }
                }
            },
            { header: CREATEDATE, dataIndex: 'order_createdate', width: 120, align: 'center' },
            {
                header: PAYDATE, dataIndex: 'money_collect_date', width: 120, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.money_collect_date == '1970-01-01 08:00:00' || record.data.money_collect_date == '') {
                        return ISPAY;
                    }
                    else {
                        return record.data.money_collect_date;
                    }
                }
            },
            {
                header: SLAVEDATEDELIVERYS, dataIndex: 'slave_date_delivery', width: 120, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.slave_date_delivery == '1970-01-01 08:00:00') {
                        return "-";
                    }
                    else {
                        return record.data.slave_date_delivery;
                    }
                }
            },
            { header: NOTEORDER, dataIndex: 'note_order', width: 120, align: 'center' },
            { header: MANAGER, dataIndex: 'product_manage', width: 120, align: 'center' }
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
        listeners: {
            resize: function () {
                OrderBrandProducesListGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    // OrderBrandProducesListStore.load({ params: { start: 0, limit: 22 } });
})
onEditClick = function () {
    var row = Ext.getCmp("OrderBrandProducesListGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], OrderBrandProducesListStore);
    }
}

/************匯入到Exce************/
function Export() {
    window.open("/OrderManage/ExportCSV?selecttype=" + Ext.getCmp('select_type').getValue() + "&searchcon=" + Ext.getCmp('search_con').getValue() + "&datetype=" + Ext.getCmp('datetype').getValue() + "&dateStart=" + Ext.Date.format(new Date(Ext.getCmp('dateStart').getValue()), 'Y-m-d') + "&dateEnd=" + Ext.Date.format(new Date(Ext.getCmp('dateEnd').getValue()), 'Y-m-d') + "&Vendor_Id=" + Ext.getCmp('Vendor').getValue() + "&product_freight_set=" + Ext.getCmp('product_freight_set').getValue() + "&product_manage=" + Ext.getCmp('product_manage').getValue() + "&order_status=" + Ext.getCmp('order_status').getValue() + "&order_payment=" + Ext.getCmp('order_payment').getValue());
}
//查询
Query = function () {
    var conten = Ext.getCmp('search_con');
    if (!conten.isValid()) {
        Ext.Msg.alert("提示", "查詢內容格式錯誤!");
        return;
    }
    var type = Ext.getCmp('select_type');
    if (type.getValue() != 0 && conten.getValue().trim() == "") {
        Ext.Msg.alert("提示", "請輸入查詢內容!");
        return;
    }
    OrderBrandProducesListStore.removeAll();
    Ext.getCmp("OrderBrandProducesListGrid").store.loadPage(1, {
        params: {
            selecttype: Ext.getCmp('select_type').getValue(), //選擇查詢種類
            searchcon: Ext.getCmp('search_con').getValue(),
            datetype: Ext.getCmp('datetype').getValue(),
            dateStart: Ext.Date.format(new Date(Ext.getCmp('dateStart').getValue()), 'Y-m-d'),
            dateEnd: Ext.Date.format(new Date(Ext.getCmp('dateEnd').getValue()), 'Y-m-d'),
            Vendor_Id: Ext.getCmp('Vendor').getValue(),
            product_freight_set: Ext.getCmp('product_freight_set').getValue(),
            product_manage: Ext.getCmp('product_manage').getValue(),
            order_status: Ext.getCmp("order_status").getValue(),
            order_payment: Ext.getCmp("order_payment").getValue()
        }
    });
}
function TransToOrder(orderId) {
    var url = '/OrderManage/OrderDetialList?Order_Id=' + orderId;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#VendorProductList');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'VendorProductList',
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
}