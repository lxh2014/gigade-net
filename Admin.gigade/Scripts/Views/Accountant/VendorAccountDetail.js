Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
'Ext.form.Panel',
'Ext.ux.form.MultiSelect',
'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
//聲明grid
Ext.define('GIGADE.VendorList', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "slave_id", type: "int" }, //訂單編號
    { name: "account_amount", type: "int" }, //成本小計（扣刷卡費，逆物流）
    { name: "order_id", type: "int" },//付款單編號
    { name: "vendor_id", type: "int" }, //供應商ID
    { name: "creditcard_1_percent", type: "int" },
    { name: "creditcard_3_percent", type: "int" },
    { name: "sales_limit", type: "int" },
    { name: "bonus_percent", type: "int" }, //折扣
    { name: "freight_low_limit", type: "int" },
    { name: "freight_low_money", type: "int" },
    { name: "freight_normal_limit", type: "int" },
    { name: "freight_normal_money", type: "int" }, //常溫運費
    { name: "freight_return_low_money", type: "int" },//低溫運費
    { name: "freight_return_normal_money", type: "int" },
    { name: "product_money", type: "int" }, //商品售價
    { name: "product_cost", type: "int" },//商品成本
    { name: "money_creditcard_1", type: "int" }, //一期刷卡費 
    { name: "money_creditcard_3", type: "int" }, //三期刷卡費
    { name: "freight_delivery_low", type: "int" },
    { name: "freight_delivery_normal", type: "int" },
    { name: "freight_return_low", type: "int" },
    { name: "freight_return_normal", type: "int" },
    { name: "account_dates", type: "string" },
    { name: "gift", type: "int" },//贈品成本
    { name: "deduction", type: "int" },//商品凈售價
    { name: "bag_check_money", type: "int" },
    { name: "detail_id", type: "int" },
    { name: "item_id", type: "int" },
    { name: "product_freight_set", type: "int" },//配送方式
    { name: "product_mode", type: "int" },
    { name: "product_name", type: "string" },//商品名
    { name: "product_spec_name", type: "string" },//商品名稱
    { name: "single_cost", type: "int" },//成本
    { name: "event_cost", type: "int" },//活動成本
    { name: "single_price", type: "int" },//售價
    { name: "single_money", type: "int" },//購買價
    { name: "buy_num", type: "int" },//數量
    { name: "detail_status", type: "int" },//商品狀態
    { name: "item_mode", type: "int" },
    { name: "parent_id", type: "int" },
    { name: "pack_id", type: "int" },
    { name: "order_payment", type: "int" },//付款方式
    //              { name: "order_createdate", type: "int" },
    //              { name: "slave_date_delivery", type: "int" },
    { name: "slave_date_close", type: "int" },
    { name: "order_createdates", type: "string" },
    { name: "slave_date_deliverys", type: "string" },
    //                { name: "slave_date_closes", type: "string" },
    { name: "newname", type: "string" },
    { name: "upc_id", type: "string" }

    ]
});
//獲取grid中的數據
var VendorListStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.VendorList',
    proxy: {
        type: 'ajax',
        url: '/Accountant/GetVendorAccountMonthDetail',
        actionMethods: 'post',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
            root: 'data'
        }
    }
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("VendorListGrid").down('#Remove').setDisabled(selections.length == 0);
            Ext.getCmp("VendorListGrid").down('#Edit').setDisabled(selections.length == 0)
        }
    }
});
//加載前先獲取ddl的值
VendorListStore.on('beforeload', function () {
    Ext.apply(VendorListStore.proxy.extraParams, {
        dateone: document.getElementById("dateone").value,
        datetwo: document.getElementById("datetwo").value,
        dateone: Ext.getCmp('dateOne').getValue(),
        datetwo: Ext.getCmp('dateTwo').getValue(),
        vendorid: document.getElementById("vendorid").value,
        vendorcode: document.getElementById("vendorcode").value
        //vendorname: document.getElementById("vendorname").value
    });
});
function thMoth() {
    var moth = new Date().getMonth();
    if (moth == 0) {
        moth = 12;
        return moth;
    } else {
        return moth;
    }
}
function thYear() {
    var year = new Date().getFullYear();
    var moth = new Date().getMonth();
    if (moth == 0) {
        year = year - 1;
        return year;
    } else {
        return year;
    }
}

Ext.onReady(function () {

    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        flex: 1.2,
        layout: 'anchor',
        height: 70,
        border: 0,
        width: document.documentElement.clientWidth,
        items: [
        {
            xtype: 'label',
            id: "lblday",
            margin: '5 0 0 0',
            style: 'fontSize:20px ;color:red',
            text: "【" + document.getElementById("dateone").value + "年" + document.getElementById("datetwo").value + "月】"
        },
        {
            xtype: 'label',
            margin: '5 0 0 0',
            style: 'fontSize:25px ;color:orange',
            text: "業績明細表"
        },
        {
            xtype: 'label',
            margin: '5 0 0 0',
            style: 'fontSize:20px ;color:red',
            id: 'lbltt',
            listeners: {
                render: function () {
                    Ext.Ajax.request({
                        url: '/Accountant/VendorName',
                        method: 'post',
                        params: {
                            vendor_id: document.getElementById("vendorid").value
                        },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            if (result.success) {
                                Ext.getCmp('lbltt').setText("【" + document.getElementById("vendorcode").value + "-" + result.msg + "】");
                            } else {

                            }
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    });
                }
            }
        },
        {
            xtype: 'fieldcontainer',
            combineErrors: true,
            layout: 'hbox',
            items: [
            {
                xtype: "numberfield",
                fieldLabel: "報表月份",
                width: 200,
                labelWidth: 80,
                margin: '5 0 0 0',
                id: 'dateOne',
                name: 'dateOne',
                margin: '5 0 0 0',
                allowBlank: false,
                maxValue: new Date().getFullYear().toString(),
                value: document.getElementById("dateone").value,
                minValue: '2011'
            },
            {
                xtype: 'displayfield',
                margin: '5 0 0 0',
                value: "~"
            },
            {
                xtype: "numberfield",
                id: 'dateTwo',
                name: 'dateTwo',
                margin: '5 0 0 0',
                width: 120,
                allowBlank: false,
                value: document.getElementById("datetwo").value,
                minValue: '1',
                maxValue: '12'
            },
            {
                xtype: 'button',
                iconCls: 'icon-search',
                text: "查詢",
                margin: '5 0 6 6',
                handler: Query,
                listeners: {
                    render: function () {
                        var lblday = Ext.getCmp('lblday');
                        a = Ext.getCmp('dateOne').getRawValue();
                        b = Ext.getCmp('dateTwo').getRawValue();
                        lblday.setText("【" + a + "年" + b + "月】");
                    },
                    'click': function () {
                        var lblday = Ext.getCmp('lblday');
                        a = Ext.getCmp('dateOne').getRawValue();
                        b = Ext.getCmp('dateTwo').getRawValue();
                        lblday.setText("【" + a + "年" + b + "月】");
                    }
                }
            }
            //,
            //{
            //    xtype: "button",
            //    text: "返回",
            //    margin: '5 0 6 6',
            //    id: "goback",
            //    handler: function () {
            //        window.location.href = "/VendorAccountMonth/Index";
            //    }
            //}
            ]
        }
        ]
    });

    //頁面加載時創建grid
    var VendorListGrid = Ext.create('Ext.grid.Panel', {
        id: 'VendorListGrid',
        flex: 8.8,
        store: VendorListStore,
        columnLines: true,
        frame: true,
        columns: [
        { header: "歸檔日期", dataIndex: 'account_dates', width: 80, align: 'center' },
        { header: "訂單日期", dataIndex: 'order_createdates', width: 80, align: 'center' },
        { header: "出貨日期", dataIndex: 'slave_date_deliverys', width: 80, align: 'center' },
        {
            header: "付款單編號", dataIndex: 'order_id', width: 80, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                return Ext.String.format('<a href=" "# target="_self" style=" text-decoration:none">{0}</a>', value);
            }
        },
        { header: "訂單編號", dataIndex: 'slave_id', width: 80, align: 'center' },
        { header: "商品售價", dataIndex: 'product_money', width: 50, align: 'center' },
        { header: "商品凈售價", dataIndex: 'deduction', width: 50, align: 'center' },
        { header: "商品成本", dataIndex: 'product_cost', width: 50, align: 'center' },
        { header: "贈品成本", dataIndex: 'gift', width: 50, align: 'center' },
        {
            header: "付款方式", dataIndex: 'order_payment', width: 50, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (value == 1) {
                    return "聯合信用卡";
                } else if (value == 2) {
                    return "ATM";
                }
                else if (value == 3) { return "藍新信用卡"; }
                else if (value == 4) { return "支付寶"; } else if (value == 5) { return "聯銀"; }
                else if (value == 6) { return "傳真刷卡"; } else if (value == 7) { return "延遲付款"; }
                else if (value == 8) { return "黑貓貨到付款"; } else if (value == 9) { return "現金"; }
            }
        },
        { header: "一期刷卡費", dataIndex: 'money_creditcard_1', width: 80, align: 'center' },
        { header: "常溫運費", dataIndex: 'freight_delivery_normal', width: 50, align: 'center' },
        { header: "低溫運費", dataIndex: 'freight_delivery_low', width: 50, align: 'center' },
        { header: "常溫逆物流", dataIndex: 'freight_return_normal', align: 'center', width: 50 },
        { header: "低溫逆物流", dataIndex: 'freight_return_low', align: 'center', width: 50 },
        { header: "成本小計（扣刷卡費，逆物流）", dataIndex: 'account_amount', align: 'center', width: 50 },
        { header: "寄倉費用小計", dataIndex: 'bag_check_money', align: 'center', width: 50 },
        { header: "商品名稱", dataIndex: 'newname', align: 'center', width: 50 },
        { header: "數量", dataIndex: 'buy_num', align: 'center', width: 50 },
        { header: "成本", dataIndex: 'single_cost', align: 'center', width: 50 },
        { header: "成本小計", dataIndex: 'single_cost', align: 'center', width: 50 },
        { header: "售價", dataIndex: 'single_price', align: 'center', width: 50 },
        { header: "售價小計", dataIndex: 'single_price', align: 'center', width: 50 },
        { header: "購買價", dataIndex: 'single_money', align: 'center', width: 50 },
        { header: "活動成本", dataIndex: 'event_cost', align: 'center', width: 50 },
        {
            header: "配送方式", dataIndex: 'product_freight_set', align: 'center', width: 50,
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (value == 1) {
                    return "常溫";
                } else if (value == 2) {
                    return "冷凍";
                }
                else if (value == 3) { return "常溫免運"; }
                else if (value == 4) { return "冷凍免運"; }
                else if (value == 5) { return "冷藏"; }
                else if (value == 6) { return "冷藏免運"; }
            }
        },
        { header: "退貨日期", dataIndex: '', align: 'center', width: 50 },
        {
            header: "商品狀態", dataIndex: 'detail_status', align: 'center', width: 50,
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (value == 0) {
                    return "等待付款";
                } else if (value == 1) {
                    return "付款失敗";
                }
                else if (value == 2) { return "待出貨"; }
                else if (value == 3) { return "出貨中"; } else if (value == 4) { return "已出貨"; }
                else if (value == 5) { return "處理中"; } else if (value == 6) { return "進倉中"; }
                else if (value == 7) { return "已進倉"; } else if (value == 8) { return "已分配"; }
            }
        },
        {
            header: "組合方式", dataIndex: 'item_mode', align: 'center', width: 50,
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (value == 0) {
                    return "單一商品";
                }
                else if (value == 1) {
                    return "父";
                }
                else if (value == 2) {
                    return "子";
                }
            }

        },
        { header: "細項編號", dataIndex: 'item_id', align: 'center', width: 50 },
        { header: "國際編碼", dataIndex: 'upc_id', align: 'center', width: 50 },



        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        tbar: [
        '->',
        {
            xtype: 'button', id: 'btnExcel', text: '業績明細匯出', iconCls: 'icon-excel', handler: function () {
                window.open("/Accountant/ExportVendorAccountMonthDetail?dateone=" + Ext.getCmp("dateOne").getValue() + "&datetwo=" + Ext.getCmp("dateTwo").getValue() + "&vendorid=" + document.getElementById("vendorid").value);
            }
        },

        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: VendorListStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        selModel: sm
    });

    Ext.create('Ext.Viewport', {
        layout: 'vbox',
        items: [frm, VendorListGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                VendorListGrid.width = document.documentElement.clientWidth;
                this.doLayout();

            }
        }
    });

    ToolAuthority();
    VendorListStore.load({ params: { start: 0, limit: 25 } });

})
/************匯入到Excel**************/
function Export() {
    Ext.Ajax.request({
        url: "/VendorAccountMonth/GetExecleData",
        params: {
            dateone: Ext.getCmp('dateOne').getValue(),
            datetwo: Ext.getCmp('dateTwo').getValue(),
            vendorid: document.getElementById("vendorid").value


        },
        success: function (response) {
            //                    alert(response.responseText);
            if (response.responseText == "true") {
                Ext.MessageBox.alert("下載提示", "<a href='../../ImportUserIOExcel/vendor_account_detail.csv'>點擊下載</a>");
            }
        }
    });
}

//查询
Query = function () {

    VendorListStore.removeAll();
    Ext.getCmp("VendorListGrid").store.loadPage(1, {
        params: {
            dateone: Ext.getCmp('dateOne').getValue(),
            datetwo: Ext.getCmp('dateTwo').getValue()

        }
    });
}