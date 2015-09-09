Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 50;
var winDetail;
var productId;
var statusname;
var info_type = "order_master";
var secret_info = "order_id;order_name;order_phone;order_mobile;order_address;delivery_name;delivery_phone;delivery_mobile;delivery_address";
//地址Mode
Ext.define("gigade.user_zip", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'zipcode', type: "int" },
        { name: 'zipname', type: "string" }]
});
var user_zip_source = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'GIGADE.user_zip',
    remoteSort: false,
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetZip',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
//自取狀態
Ext.define('gigade.PaymentZQ', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "parameterCode", type: "string" },
        { name: "ParameterName", type: "string" }
    ]
});
var ZQStore = Ext.create('Ext.data.Store', {
    autoLoad: true,
    model: 'gigade.PaymentZQ',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetZQ',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
//用作編輯時獲得數據包含機敏信息
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


Ext.define("t_zip", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'zipcode', type: "string" },
        { name: 'zipname', type: "string" }]
});
var ZipStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 't_zip',
    remoteSort: false,
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetZip',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
//訂單Mode
Ext.define('GIGADE.orderList', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'Slave_Id', type: 'uint' },//訂單編號
        { name: 'Slave_Status', type: 'string' },//訂單狀態
        { name: 'Slave_Status_Str', type: 'string' },//訂單狀態
        { name: 'Slave_Date_Close', type: 'string' },//訂單歸檔日
        { name: 'Clos_Date', type: 'string' },//訂單歸檔日
        { name: 'Detail_Id', type: 'string' },//購物編號
        { name: 'Detail_Status', type: 'string' },//商品狀態
        { name: 'Detail_Status_Str', type: 'string' },//商品狀態
        { name: 'Item_Id', type: 'int' },//商品編號
        { name: 'Product_Id', type: 'int' },//商品編號
        { name: 'Product_Name', type: 'string' },//商品名稱
        { name: 'Brand_Name', type: 'string' },//品牌名稱
        { name: 'Product_Spec_Name', type: 'string' },//規格名稱
        { name: 'item_mode', type: 'int' },
        { name: 'Product_Freight_Set', type: 'string' },//托運單屬性
        { name: 'Product_Freight_Set_Str', type: 'string' },//托運單屬性
        { name: 'Tax_Type', type: 'string' },//營業稅
        { name: 'Buy_Num', type: 'int' },//數量
        { name: 'Single_Money', type: 'int' },//折扣價
        { name: 'Deduct_Happygo_Money', type: 'int' },//折扣價
        { name: 'Deduct_Bonus', type: 'string' },//使用購物金
        { name: 'Deduct_Welfare', type: 'int' },//抵用卷
        { name: 'Deduct_Happygo', type: 'int' },//hg
        { name: 'subtotal', type: 'string' },//小計
        { name: 'Product_Mode', type: 'int' },//出貨方式
        { name: 'Product_Mode_Name', type: 'string' },
        { name: 'Account_Status', type: 'string' },//廠商對賬 0:未對帳 1：已對賬
        { name: 'Accumulated_Bonus', type: 'string' },//給予購物金
        { name: 'Vendor_Name_Simple', type: 'string' },//出貨商
        { name: 'Single_Cost', type: 'int' },
        { name: 'Event_Cost', type: 'int' },
        { name: 'Single_Price', type: 'int' },
        { name: 'parent_num', type: 'int' },
        { name: 'Bag_Check_Money', type: 'int' },
        { name: 'Detail_Note', type: 'string' },
        { name: 'Parent_Id', type: 'int' },
        { name: 'pack_id', type: 'int' },
        { name: 'Order_Id', type: 'int' },
        { name: 'Vendor_Id', type: 'int' },
        { name: 'VendorMd5', type: 'string' },
       // { name: 'singlemoney', type: 'string' },//折扣價(千分位)
        { name: 'channel_name_simple', type: 'string' }//賣場        
    ]
});
var orderListStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.orderList',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetOrderDetailList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
            root: 'data'
        }
    }
});
var olderChildStore = Ext.create('Ext.data.Store', {
   // pageSize: pageSize,
    model: 'GIGADE.orderList',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetOrderChildList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
            root: 'data'
        }
    }
});

//供應商Mode
Ext.define('GIGADE.VendorList', {
    extend: 'Ext.data.Model',
    fields: [
            { name: "vendor_id", type: "string" },
                { name: "vendor_code", type: "string" },
                { name: "vendor_name_simple", type: "string" },
                { name: "checkout_type", type: "string" },
                { name: "agr_date", type: "string" },
                { name: "freight_normal_limit", type: "decimal" },
                { name: "freight_normal_money", type: "decimal" },
                { name: "freight_low_limit", type: "decimal" },
                { name: "freight_low_money", type: "decimal" },
                { name: "freight_return_normal_money", type: "decimal" },
                { name: "freight_return_low_money", type: "decimal" },
                { name: "vendor_status", type: "int" },
                { name: "vendor_email", type: "string" },
                { name: "vendor_password", type: "string" },
                { name: "vendor_name_full", type: "string" },
                { name: "vendor_invoice", type: "string" },
                { name: "company_phone", type: "string" },
                { name: "vendor_confirm_code", type: "string" },
                { name: "vendor_note", type: "string" },
                { name: "vendor_login_attempts", type: "int" },
                { name: "assist", type: "int" },
                { name: "dispatch", type: "int" },
                { name: "product_mode", type: "int" },
                { name: "product_manage", type: "int" },
                { name: "gigade_bunus_percent", type: "int" },
                { name: "gigade_bunus_threshold", type: "int" },
                { name: "company_fax", type: "string" },
                { name: "company_person", type: "string" },
                { name: "company_zip", type: "int" },
                { name: "company_address", type: "string" },
                { name: "invoice_zip", type: "int" },
                { name: "invoice_address", type: "string" },
                { name: "contact_type_1", type: "int" },
                { name: "contact_name_1", type: "string" },
                { name: "contact_phone_1_1", type: "string" },
                { name: "contact_phone_2_1", type: "string" },
                { name: "contact_mobile_1", type: "string" },
                { name: "contact_email_1", type: "string" },
                { name: "contact_type_2", type: "int" },
                { name: "contact_name_2", type: "string" },
                { name: "contact_phone_1_2", type: "string" },
                { name: "contact_phone_2_2", type: "string" },
                { name: "contact_mobile_2", type: "string" },
                { name: "contact_email_2", type: "string" },
                { name: "contact_type_3", type: "int" },
                { name: "contact_name_3", type: "string" },
                { name: "contact_phone_1_3", type: "string" },
                { name: "contact_phone_2_3", type: "string" },
                { name: "contact_mobile_3", type: "string" },
                { name: "contact_email_3", type: "string" },
                { name: "contact_type_4", type: "int" },
                { name: "contact_name_4", type: "string" },
                { name: "contact_phone_1_4", type: "string" },
                { name: "contact_phone_2_4", type: "string" },
                { name: "contact_mobile_4", type: "string" },
                { name: "contact_email_4", type: "string" },
                { name: "contact_type_5", type: "int" },
                { name: "contact_name_5", type: "string" },
                { name: "contact_phone_1_5", type: "string" },
                { name: "contact_phone_2_5", type: "string" },
                { name: "contact_mobile_5", type: "string" },
                { name: "contact_email_5", type: "string" },
                { name: "cost_percent", type: "int" },
                { name: "creditcard_1_percent", type: "int" },
                { name: "creditcard_3_percent", type: "string" },
                { name: "sales_limit", type: "int" },
                { name: "bonus_percent", type: "int" },
                { name: "agreement_createdate", type: "int" },
                { name: "agr_start", type: "string" },
                { name: "agr_end", type: "string" },
                { name: "checkout_other", type: "string" },
                { name: "bank_code", type: "string" },
                { name: "bank_name", type: "string" },
                { name: "bank_number", type: "string" },
                { name: "bank_account", type: "string" },
                { name: "i_middle", type: "string" },
                { name: "c_middle", type: "string" },
                { name: "i_zip", type: "string" },
                { name: "c_zip", type: "string" },
                { name: "erp_id", type: "string" },
                { name: "manage_name", type: "string" },
                { name: "c_zip", type: "string" },
                { name: "erp_id", type: "string" },
                { name: "manage_email", type: "string" }
    ]
});
var VendorListStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.VendorList',
    proxy: {
        type: 'ajax',
        url: '/Vendor/GetVendorList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
            root: 'data'
        }
    }
});
//多選框
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            var row = Ext.getCmp("orderListGrid").getSelectionModel().getSelection();
            var model = Ext.getCmp("orderListGrid").getSelectionModel();
            if (row.length > 0 && (row[0].data.Detail_Status == "4" || row[0].data.Detail_Status == "6" || row[0].data.Detail_Status == "7")) {
                Ext.getCmp("orderListGrid").down('#return').setDisabled(false);
            }
            else {
                Ext.getCmp("orderListGrid").down('#return').setDisabled(true);
            }
            if (row.length > 1) {
                for (i = 0; i < row.length; i++) {                    
                    if (!(row[i].data.Detail_Status == "4" || row[i].data.Detail_Status== "6" || row[i].data.Detail_Status == "7")) {
                        model.deselectAll();
                        Ext.Msg.alert("提示信息", "該狀態的商品不可退!");
                    }
                }
            }
        }
    }
});

Ext.onReady(function () {
    var topHeight = 250;
    var PayID = GetOrderId();
    orderListStore.on('beforeload', function () {
        Ext.apply(orderListStore.proxy.extraParams, {
            OrderId: Ext.htmlEncode(PayID)//商品出貨
        })
    });
    //顯示付款單中的所有訂單
    var orderListGrid = new Ext.grid.Panel({
        id: 'orderListGrid',
        store: orderListStore,
        region: 'center',
        autoScroll: true,
        border: 0,
        height: topHeight,
        columns: [
            {
                header: '出貨商', dataIndex: 'Vendor_Name_Simple', width: 110, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    //出貨按鈕先隱藏
                    //var vendorServerPath = document.getElementById('VendorServer').value;
                    //var shipStr = vendorServerPath + "?vendor_id=" + record.data.Vendor_Id + "&key=" + record.data.VendorMd5; 
                    return '<a href="#"  onclick="VendorFunction(' + record.data.Vendor_Id + ')">' + value + '</a>&nbsp;&nbsp;';
                    
                    //return '<a href="#"  onclick="showVendorDetail(' + value + ',' + record.data.Vendor_Name_Simple + ',' + record.data.Vendor_Name_Simple + ',' + record.data.Vendor_Name_Simple + ')">' + value + '</a>&nbsp;&nbsp;';
                    //<a target="_blank" href="' + shipStr + '" >出貨</a>
                }
            },
            { header: '訂單編號', dataIndex: 'Slave_Id', width: 55, align: 'center' },
            { header: '訂單狀態', dataIndex: 'Slave_Status_Str', width: 55, align: 'center' },
            {
                header: '訂單歸檔日', dataIndex: 'Clos_Date', width: 115, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == '1970-01-01 08:00:00') {
                        return "";
                    }
                    else {
                        return value;
                    }
                }
            },
            {
                header: '購物編號', dataIndex: 'Detail_Id', width: 80, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return '<a href="#"  onclick="showDetail(' + value + ')">' + value + '</a>';
                }
            },
            { header: '商品狀態', dataIndex: 'Detail_Status_Str', width: 80, align: 'center' },
            {
                header: '商品編號', colName: 'Item_Id', width: 60, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.item_mode == 1) {//0:單一商品, 1:父商品, 2:子商品
                        return '<a href="#"  onclick="javascript:showProductDetail(' + record.data.Parent_Id + ')">' + record.data.Parent_Id + '</a>';
                    } else
                    {
                        return '<a href="#"  onclick="javascript:showProductDetail(' + record.data.Parent_Id + ')">' + record.data.Item_Id + '</a>';
                    }
                          
                }
            },
            {
                header: '商品名稱', dataIndex: 'Product_Name', width: 260, align: 'left',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    var value_str = record.data.Brand_Name + "---" + value + record.data.Product_Spec_Name;
                    if (record.data.item_mode == 1) {//0:單一商品, 1:父商品, 2:子商品
                        value_str += ' <a href="#"  onClick="showChildProduct(' + record.data.Detail_Id + ')">【組合明細】</a> ';
                    }
                    if (record.data.Detail_Note != null && record.data.Detail_Note != "" && record.data.Detail_Note != "0") {
                        value_str += '<br /><span style="color:#f0f;">&nbsp;&nbsp;' + record.data.Detail_Note + '</span>';
                    }
                    return value_str;
                }
            },
            { header: '托運單屬性', dataIndex: 'Product_Freight_Set_Str', width: 80, align: 'center' },
            {
                header: '營業稅', dataIndex: 'Tax_Type', width: 55, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (value) {
                        case "1": return "應稅"; break;
                        case "3": return "免稅"; break;
                    }
                }
            },
            { header: '數量', dataIndex: 'Buy_Num', width: 50, align: 'center' },
            {
                header: '折扣價', dataIndex: 'Single_Money', width: 60, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return change(value);
                }
            },
            {
                header: '使用購物金', dataIndex: 'Deduct_Bonus', width: 70, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return change(record.data.Deduct_Bonus);
                }
            },
            { header: '抵用券', dataIndex: 'Deduct_Welfare', width: 55, align: 'center' },
            {
                header: 'HG折抵(點數/元)', dataIndex: 'Deduct_Happygo', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return record.data.Deduct_Happygo + '/' + record.data.Deduct_Happygo_Money;
                }
            },
            {
                header: '小計', dataIndex: 'subtotal', width: 60, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return change(value);
                }
            },
            { header: '出貨方式', dataIndex: 'Product_Mode_Name', width: 60, align: 'center' },
            {
                header: '廠商對賬', dataIndex: 'Account_Status', width: 60, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (value) {
                        case "0": return "未對帳"; break;
                        case "1": return "已對賬"; break;
                    }
                }
            },
            { header: '給予購物金', dataIndex: 'Accumulated_Bonus', width: 70, align: 'center' }, 
            { header: '賣場', dataIndex: 'channel_name_simple', width: 70, align: 'center' }
        ],
        tbar: [
        {
            xtype: 'button', text: '訂單記錄', id: 'record', iconCls: 'icon-information', handler: function () {
                var order_id = GetOrderId();
                var record = "訂單詳細記錄";
                var urlTran = "/OrderManage/Index" + '?Order_Id=' + order_id;
                var panel = window.parent.parent.Ext.getCmp('ContentPanel');
                var copy = panel.down('#orderdetial');
                if (copy) {
                    copy.close();
                }
                copy = panel.add({
                    id: 'orderdetial',
                    title: record,
                    html: window.top.rtnFrame(urlTran),
                    closable: true
                });
                panel.setActiveTab(copy);
                panel.doLayout();
            }
        },
        {
            xtype: 'button', text: '新增訂單問題', id: 'addOrderQuestion', iconCls: 'icon-user-add', handler: function () {
                var order_id = GetOrderId();
                var record = "新增訂單問題";
                var urlTran = "/Service/OrderQuestionAdd" + '?Order_Id=' + order_id;
                var panel = window.parent.parent.Ext.getCmp('ContentPanel');
                var copy = panel.down('#addOrderQuestion');
                if (copy) {
                    copy.close();
                }
                copy = panel.add({
                    id: 'addOrderQuestion',
                    title: record,
                    html: window.top.rtnFrame(urlTran),
                    closable: true
                });
                panel.setActiveTab(copy);
                panel.doLayout();
            }
        },
        { xtype: 'button', text: "取消整筆訂單", id: 'return_All_Order', disabled: true, handler: onReturnALLOrderClick },
        { xtype: 'button', text: "退貨", id: 'return', disabled: true, handler: onReturnClick },
        { xtype: 'button', text: "等待付款", id: 'wait', disabled: true, handler: onWaitClick },
        { xtype: 'button', text: "轉自取", id: 'change', disabled: true, handler: onChangePayment_cash },
        { xtype: 'button', text: "轉黑貓貨到付款", id: 't_cat', disabled: true, handler: onChangePayment_cat }
        ],
        //bbar: Ext.create('Ext.PagingToolbar', {
        //    store: orderListStore,
        //    pageSize: pageSize,
        //    displayInfo: true,
        //    displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
        //    emptyMsg: NOTHING_DISPLAY
        //}),
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
    //基本資料
    var BaseInfo = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        id: 'BaseInfo',
        border: false,
        items: [
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'fieldset',
                        title: '基本資料',
                        layout: 'anchor',
                        width: 600,
                        items: [
                            {
                                xtype: 'displayfield',
                                fieldLabel: '付款單號',
                                id: 'order_id',
                                name: 'order_id',
                                value: document.getElementById('OrderId').value,
                                width: 300
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '結帳購物車',
                                id: 'site',
                                name: 'site',
                                value: document.getElementById('c_site').value,
                                width: 400
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '付款方式',
                                labelWidth: 100,
                                id: 'order_payment_string',
                                name: 'order_payment_string',
                                width: 400
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '可出貨日期',
                                id: 'OrderDatePay',
                                name: 'OrderDatePay',
                                width: 400
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '賬款實收日期',
                                id: 'MoneyCollectDate',
                                name: 'MoneyCollectDate',
                                width: 400
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '永豐匯款帳號',
                                id: 'sinopaId',
                                name: 'sinopaId',
                                hidden: true,
                                listeners: {
                                    'beforerender': function () {
                                        if (document.getElementById('c_sinopa').value != "") {
                                            Ext.getCmp('sinopaId').hidden = false;
                                            Ext.getCmp('sinopaId').setValue(document.getElementById('c_sinopa').value);
                                        }
                                    }
                                }
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '華南匯款帳號',
                                id: 'hncb_id',
                                name: 'hncb_id',
                                hidden: true,
                                listeners: {
                                    'beforerender': function () {
                                        if (document.getElementById('c_hncb').value != "") {
                                            Ext.getCmp('hncb_id').hidden = false;
                                            Ext.getCmp('hncb_id').setValue(document.getElementById('c_hncb').value);
                                        }
                                    }
                                }
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '付款單狀態',
                                id: 'order_status',
                                name: 'order_status',
                                width: 400
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '付款單成立日期',
                                id: 'order_createdate',
                                name: 'order_createdate',
                                width: 400
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '付款單歸檔日期',
                                id: 'order_date_close',
                                name: 'order_date_close',
                                width: 400
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '發票統編',
                                id: 'company_invoice',
                                name: 'company_invoice',
                                hidden: true,
                                listeners: {
                                    'beforerender': function () {
                                        if (document.getElementById('c_companyWrite').value == "1") {
                                            Ext.getCmp('company_invoice').hidden = false;
                                            Ext.getCmp('company_title').hidden = false;
                                            Ext.getCmp('company_tongbian').hidden = true;
                                        }
                                        else {
                                            Ext.getCmp('company_tongbian').hidden = false;
                                        }
                                    }
                                }
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '發票抬頭',
                                id: 'company_title',
                                name: 'company_title',
                                width:700,
                                hidden: true
                            },  
                            {
                                xtype: 'displayfield',
                                fieldLabel: '開立統編',
                                id: 'company_tongbian',
                                name: 'company_tongbian',
                                value: "<span style='color:red'>否</span>",
                                hidden: true
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '賣場',
                                id: 'channel',
                                name: 'channel',
                                value: "<span style='color:red'>" + document.getElementById('c_channel').value + "<span/>",
                                width: 400
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '外站匯入時間',
                                id: 'import_time',
                                name: 'import_time',
                                hidden: true,
                                listeners: {
                                    'beforerender': function () {
                                        if (document.getElementById('c_importTime').value != "1970-01-01 08:00:00") {
                                            // alert(document.getElementById('c_importTime'));
                                            Ext.getCmp('import_time').hidden = false;
                                            Ext.getCmp('channel_order_id').hidden = false;
                                            Ext.getCmp('retrieve_mode').hidden = false;
                                            Ext.getCmp('import_time').setValue(document.getElementById('c_importTime').value);
                                            Ext.getCmp('channel_order_id').setValue(document.getElementById('c_channelOderId').value);
                                            Ext.getCmp('retrieve_mode').setValue(document.getElementById('c_retrieve_mode').value);
                                        }
                                    }
                                }
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '外站訂單編號',
                                id: 'channel_order_id',
                                name: 'channel_order_id',
                                hidden: true
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '取貨方式',
                                id: 'retrieve_mode',
                                name: 'retrieve_mode',
                                hidden: true
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '物流模式',
                                id: 'delivery_store',
                                name: 'delivery_store',
                                width: 600
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '對賬',
                                id: 'billing_checked',
                                name: 'billing_checked'
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '假日可出貨',
                                id: 'holiday_deliver',
                                name: 'holiday_deliver'
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '到貨指定時段',
                                id: 'estimated_arrival_period',
                                name: 'estimated_arrival_period',
                                width: 450
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '備註', 
                                id: 'note_order',
                                name: 'note_order',
                                width: 520,
                                height: 100
                            },
                            {
                                xtype: 'fieldcontainer',
                                layout: 'hbox',
                                items: [
                                {
                                    xtype: 'button',
                                    text: '編輯客戶備註',
                                    handler: onEditOrderNote
                                },
                                {
                                    xtype: 'button',
                                    text: '新增狀態列表',
                                    handler: onAddStatus
                                }]
                            }
                        ]
                    },
                    {
                        xtype: 'fieldset',
                        title: '基本資料',
                        layout: 'anchor',
                        width: 450,
                        items: [
                            {
                                xtype: 'displayfield',
                                fieldLabel: '商品總金額',
                                id: 'order_product_subtotal',
                                name: 'order_product_subtotal',
                                width: 200
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '購物金抵用金額',
                                id: 'deduct_bonus',
                                name: 'deduct_bonus',
                                width: 200
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '購物金發放金額',
                                id: 'accumulated_bonus',
                                name: 'accumulated_bonus',
                                width: 200
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: 'HG抵用點數',
                                id: 'deduct_happygo',
                                name: 'deduct_happygo',
                                width: 200
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: 'HG發放點數',
                                id: 'accumulated_happygo',
                                name: 'accumulated_happygo',
                                width: 200
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '抵用券金額',
                                id: 'deduct_welfare',
                                name: 'deduct_welfare',
                                width: 200
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '紅利折抵點數',
                                id: 'deduct_card_bonus',
                                name: 'deduct_card_bonus',
                                width: 200
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '常溫運費',
                                id: 'order_freight_normal',
                                name: 'order_freight_normal',
                                width: 200
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '低溫運費',
                                id: 'order_freight_low',
                                name: 'order_freight_low',
                                width: 200
                            },
                            {
                                xtype: 'displayfield',
                                fieldLabel: '訂單應付金額',
                                id: 'order_amount',
                                name: 'order_amount',
                                width: 200
                            }
                        ]
                    }
                ]
            }
        ]
    });
    //基本資料
    var cancel = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        id: 'cancel',
        border: false,
        width: 900,
        items: [
        {
            xtype: 'fieldset',
            title: '取消/退貨',
            items: [
            {
                xtype: 'displayfield',
                fieldLabel: '取消金額',
                id: 'money_cancel',
                name: 'money_cancel',
                width: 200
            },
            {
                xtype: 'displayfield',
                fieldLabel: '退貨金額',
                id: 'money_return',
                name: 'money_return',
                width: 200
            }]           
        }]        
    });
    //購買人
    var order = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        id: 'order',
        border: false,
        width: 900,
        items: [
            {
                xtype: 'fieldset',
                title: '購買人',
                items: [
                {
                    xtype: 'displayfield',
                    fieldLabel: '用戶ID',
                    id: 'user_id',
                    name: 'user_id',
                    hidden: true
                },
                {
                    xtype: 'displayfield',
                    fieldLabel: '姓名',
                    id: 'order_name',
                    name: 'order_name',
                    width: 350
                },
                {
                    xtype: 'displayfield',
                    fieldLabel: '市內電話',
                    id: 'order_phone',
                    name: 'order_phone',
                    width: 350
                },
                {
                    xtype: 'displayfield',
                    fieldLabel: '手機',
                    id: 'order_mobile',
                    name: 'order_mobile',
                    width: 350
                },
                {
                    xtype: 'fieldcontainer',
                    layout: 'hbox',
                    items: [
                        {
                            xtype: 'displayfield',
                            value: '帳單地址:'
                        },
                        {
                            xtype: 'displayfield',
                            id: 'order_address',
                            margin:'0 0 0 26',
                            width: 400,
                            name: 'order_address'
                        }]
                }]
            }
        ]
    });
    //收貨人
    var delivery = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        id: 'delivery',
        border: false,
        width: 900,
        items: [
        {
            xtype: 'fieldset',
            title: '收貨人',
            items: [
            {
                xtype: 'displayfield',
                fieldLabel: '姓名',
                id: 'delivery_name',
                name: 'delivery_name',
                width: 350
                //listeners: {
                //    'beforerender': function () {
                //        //if (document.getElementById('c_delivery_same').value == "1") {
                //        //    Ext.getCmp('delivery_phone').hidden = true;
                //        //    Ext.getCmp('delivery_mobile').hidden = true;
                //        //    //Ext.getCmp('delivery_zip').hidden = true;
                //        //    Ext.getCmp('delivery_address').hidden = true;
                //        //    Ext.getCmp('address').hidden = true;
                //        //    Ext.getCmp('delivery_name').setValue("《同購買人》");
                //        //}
                //        //else {
                //            var name = document.getElementById('c_delivery_name').value;
                //            var gender = document.getElementById('c_delivery_gender').value == "0" ? "小姐" : "先生";
                //            //Ext.getCmp('delivery_name').setValue(name + "/ " + gender);
                //            Ext.getCmp('delivery_name').setValue("<a href='javascript:void(0);' onclick='SecretLogin(" + result.data.order_id + "," + 0 + ",\"" + info_type + "\")'  >" + name + "|" + gender + "</a>");
                //        }
                //    //}
                //}
            },
            {
                xtype: 'displayfield',
                fieldLabel: '市內電話',
                id: 'delivery_phone',
                name: 'delivery_phone',
                width: 350
            },
            {
                xtype: 'displayfield',
                fieldLabel: '手機',
                id: 'delivery_mobile',
                name: 'delivery_mobile',
                width: 350
            },
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                {
                    xtype: 'displayfield',
                    id: 'address',
                    value: '帳單地址:'
                },
                {
                    xtype: 'displayfield',
                    id: 'delivery_address',
                    margin: '0 0 0 26',
                    width: 400,
                    name: 'delivery_address'
                },
                ]
            },
               {
                   xtype: 'button',
                   id: 'change_info',
                   text: '更改收貨人資訊',
                   margin: '0 0 0 0',
                   hidden: true,
                   handler: onModifyDeliverData
               },
            ]
        }]        
    });
    //管理員欄位設定
    var admin = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        id: 'deliver',
        border: false,
        width: 900,
        height: 300,
        listeners: {
            'afterrender': function () {
                Ext.Ajax.request({
                    url: '/OrderManage/GetData',
                    method: 'post',
                    reader: {
                        type: 'json',
                        root: 'data'
                    },
                    params: {
                        order_id: document.getElementById('OrderId').value
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            if (result.data.order_payment != "") {
                                Ext.getCmp('order_payment_string').setValue(result.data.payment_string);
                            }
                            if (result.data.money_collect_date == 0) {
                                Ext.getCmp('MoneyCollectDate').setValue("");
                            }
                            else {
                                Ext.getCmp('MoneyCollectDate').setValue(result.data.MoneyCollectDate);
                            }
                            Ext.getCmp('order_status').setValue("<span style='color:blue'>" + result.data.order_status_str + "</span>");
                            if (result.data.order_createdate == 0) {
                                Ext.getCmp('order_createdate').setValue("");
                            }
                            else {
                                Ext.getCmp('order_createdate').setValue(result.data.OrderCreateDate);
                            }
                            Ext.getCmp('order_createdate').setValue(result.data.OrderCreateDate);
                            if (result.data.order_date_close == 0) {
                                Ext.getCmp('order_date_close').setValue("");
                            } else if (result.data.order_status != '99')
                            {
                                Ext.getCmp('order_date_close').setValue("");
                            }
                            else {
                                Ext.getCmp('order_date_close').setValue(result.data.OrderDateClose);
                            }

                            if (result.data.order_date_pay == 0) {
                                Ext.getCmp('OrderDatePay').setValue("");
                            }
                            else {
                                Ext.getCmp('OrderDatePay').setValue(result.data.OrderDatePay);
                            }
                            switch (result.data.order_status) {
                                case 0:
                                case 1:
                                case 2:
                                case 10:
                                case 20:
                                    Ext.getCmp("orderListGrid").down('#return_All_Order').setDisabled(false);
                                    break;
                                default:
                                    Ext.getCmp("orderListGrid").down('#return_All_Order').setDisabled(true);
                                    break;
                            }

                            Ext.getCmp('company_invoice').setValue(result.data.company_invoice);
                            Ext.getCmp('company_title').setValue(result.data.company_title);
                            //deliver_str
                            Ext.getCmp('delivery_store').setValue("<span style='color:red'>" + result.data.deliver_str + "</span>");
                            if (result.data.billing_checked == 0) {
                                Ext.getCmp('billing_checked').setValue("<span style='color:red'>無</span>");
                            }
                            else { Ext.getCmp('billing_checked').setValue("<span style='color:red'>有</span>"); }
                            if (result.data.holiday_deliver == 1) {
                                Ext.getCmp('holiday_deliver').setValue("<span style='color:red'>是</span>");
                            }
                            else {
                                Ext.getCmp('holiday_deliver').setValue("<span style='color:red'>否</span>");
                            }
                            switch (result.data.estimated_arrival_period) {
                                case 0:
                                    Ext.getCmp('estimated_arrival_period').setValue("<span style='color:red'>不限時</span>");
                                    break;
                                case 1:
                                    Ext.getCmp('estimated_arrival_period').setValue("<span style='color:red'>12:00以前</span>");
                                    break;
                                case 2:
                                    Ext.getCmp('estimated_arrival_period').setValue("<span style='color:red'>12:00-17:00</span>");
                                    break;
                                case 3:
                                    Ext.getCmp('estimated_arrival_period').setValue("<span style='color:red'>17:00-20:00</span>");
                                    break;
                            }
                            Ext.getCmp('order_product_subtotal').setValue(change(result.data.order_product_subtotal) + " 元");
                            if (result.data.deduct_bonus != 0) {
                                Ext.getCmp('deduct_bonus').setValue("-" + result.data.deduct_bonus + " 元");
                            }
                            else {
                                Ext.getCmp('deduct_bonus').setValue(result.data.deduct_bonus + " 元");
                            }
                            Ext.getCmp('accumulated_bonus').setValue(result.data.accumulated_bonus + " 元");
                            if (result.data.deduct_happygo != "0") {
                                Ext.getCmp('deduct_happygo').setValue("-" + result.data.deduct_happygo + " 點 " + result.data.Hg_Nt + " 元 ");
                            }
                            else {
                                Ext.getCmp('deduct_happygo').setValue(result.data.deduct_happygo + " 點 " + result.data.Hg_Nt + " 元 ");
                            }

                            Ext.getCmp('accumulated_happygo').setValue(result.data.accumulated_happygo + " 點");
                            Ext.getCmp('deduct_card_bonus').setValue(result.data.deduct_card_bonus + " 元");
                            if (result.data.deduct_welfare != "0") {
                                Ext.getCmp('deduct_welfare').setValue("-" + result.data.deduct_welfare + " 元");
                            }
                            else {
                                Ext.getCmp('deduct_welfare').setValue(result.data.deduct_welfare + " 元");
                            }
                            Ext.getCmp('order_freight_normal').setValue(result.data.order_freight_normal + " 元");
                            Ext.getCmp('order_freight_low').setValue(result.data.order_freight_low + " 元");
                            Ext.getCmp('order_amount').setValue("<span style='color:red'>" +change(result.data.order_amount) + " 元</span>");
                            if (result.data.money_cancel != "0") {
                                Ext.getCmp('money_cancel').setValue("-" +change(result.data.money_cancel) + " 元");
                            }
                            else {
                                Ext.getCmp('money_cancel').setValue(change(result.data.money_cancel) + " 元");
                            }
                            if (result.data.money_return != "0") {
                                Ext.getCmp('money_return').setValue("-" + result.data.money_return + " 元");
                            }
                            else {
                                Ext.getCmp('money_return').setValue(result.data.money_return + " 元");
                            }
                            Ext.getCmp('note_order').setValue("<span style='color:red'>" + result.data.note_order + "</span>");
                            Ext.getCmp('note_admin').setValue(result.data.note_admin);

                            var orderGender = result.data.order_gender == "0" ? "小姐" : "先生";
                            //購買人添加資安
                            Ext.getCmp('order_name').setValue("<a href='javascript:void(0);' onclick='oneditUser(" + result.data.user_id + ")'>" + result.data.order_name + "</a>" + " / " + orderGender);
                            //Ext.getCmp('order_phone').setValue(result.data.order_phone);
                            Ext.getCmp('order_phone').setValue("<a href='javascript:void(0);' onclick='SecretLogin(" + result.data.order_id + "," + 0 + ",\"" + info_type + "\")'  >" + result.data.order_phone + "</a>");
                            //Ext.getCmp('order_mobile').setValue(result.data.order_mobile);
                            Ext.getCmp('order_mobile').setValue("<a href='javascript:void(0);' onclick='SecretLogin(" + result.data.order_id + "," + 0 + ",\"" + info_type + "\")'  >" + result.data.order_mobile + "</a>");
                            //Ext.getCmp('order_zip').setValue(result.data.order_zip);
                            //Ext.getCmp('order_address').setValue(result.data.order_address);
                            Ext.getCmp('order_address').setValue("<a href='javascript:void(0);' onclick='SecretLogin(" + result.data.order_id + "," + 0 + ",\"" + info_type + "\")'  >" + result.data.order_address + "</a>");
                            //Ext.getCmp('delivery_phone').setValue(result.data.delivery_phone);
                            Ext.getCmp('delivery_phone').setValue("<a href='javascript:void(0);' onclick='SecretLogin(" + result.data.order_id + "," + 0 + ",\"" + info_type + "\")'  >" + result.data.delivery_phone + "</a>");
                            //Ext.getCmp('delivery_mobile').setValue(result.data.delivery_mobile);
                            Ext.getCmp('delivery_mobile').setValue("<a href='javascript:void(0);' onclick='SecretLogin(" + result.data.order_id + "," + 0 + ",\"" + info_type + "\")'  >" + result.data.delivery_mobile + "</a>");
                            //Ext.getCmp('delivery_zip').setValue(result.data.delivery_zip);
                            //Ext.getCmp('delivery_address').setValue(result.data.delivery_address);
                            Ext.getCmp('delivery_address').setValue("<a href='javascript:void(0);' onclick='SecretLogin(" + result.data.order_id + "," + 0 + ",\"" + info_type + "\")'  >" + result.data.delivery_address + "</a>");
                            Ext.getCmp('delivery_name').setValue("<a href='javascript:void(0);' onclick='SecretLogin(" + result.data.order_id + "," + 0 + ",\"" + info_type + "\")'  >" + result.data.delivery_name + "</a>" + "/" + orderGender);
                            //等待付款加驗證
                            if (result.data.order_status == 1 || result.data.order_status == 2 || result.data.order_status == 10 || result.data.order_status == 20) {
                                Ext.getCmp('wait').setDisabled(false);
                            }
                            else {
                                Ext.getCmp('wait').setDisabled(true);
                            }
                            statusname = result.data.order_status_str;
                            var is_vendor_deliver = result.data.is_vendor_deliver;
                            if (result.data.order_status == 0 || result.data.order_status == 2)
                            {
                                if (result.data.is_vendor_deliver == false) {
                                    Ext.getCmp('change').setDisabled(false);
                                }
                                if (result.data.order_status == 0)
                                {
                                    if (result.data.is_vendor_deliver == false) {
                                        Ext.getCmp('t_cat').setDisabled(false);
                                    }
                                }
                            }
                            else {
                                Ext.getCmp('change').setDisabled(true);
                                Ext.getCmp('t_cat').setDisabled(true);
                            }
                            if (result.data.cart_id != 16 && (result.data.order_status == 2 || result.data.order_status == 0)) {
                                Ext.getCmp('change_info').show();
                            }
                            //購買人
                        }
                    }
                });
            }
        },
        items: [
            {
                xtype: 'fieldset',
                title: '管理員設定欄位',
                items: [
                {
                    xtype: 'displayfield',
                    id: 'adminNote',
                    fieldLabel: '管理員備註',
                    id: 'note_admin',
                    name: 'note_admin',
                    width: 800
                },
                {
                    xtype: 'button',
                    text: '新增管理員備註',
                    handler: onEditNoteAdmin
                }]                
            }
        ]
    });
    var total = Ext.create('Ext.form.Panel', {
        autoScroll: true,
        items: [orderListGrid, BaseInfo, cancel, order, delivery, admin]
    });
    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [total],
        renderTo: Ext.getBody(),
        listeners: {
            resize: function () {
                orderListGrid.width = document.documentElement.clientWidth;
                total.height = document.documentElement.clientHeight;
                this.doLayout();
            }
        }
    });
    QueryToolAuthorityByUrl('/OrderManage/OrderDetialList');
    orderListStore.load({ params: { start: 0, limit: pageSize } });
});
///取消整筆訂單
onReturnALLOrderClick = function () {
    var order_id = Ext.getCmp('order_id').getValue();
   // alert(order_id + statusname);
    ReturnALLOrderFunction(order_id, statusname);
}

//獲取要顯示的付款單號
function GetOrderId() {
    return document.getElementById('OrderId').value;
}
//獲取要顯示的付款單號
function GetOrderstatus() {
    return statusname;
}
//由商品編號鏈接至商品詳細資料
function showProductDetail(product_id) {
    productId = product_id;
    if (winDetail == undefined) {
        winDetail = Ext.create('Ext.window.Window', {
            title: '商品詳細資料',
            constrain: true,
            modal: true,
            resizable: false,
            height: document.documentElement.clientHeight * 565 / 783,
            width: 1000,
            autoScroll: false,
            layout: 'fit',
            html: "<iframe scrolling='no' frameborder=0 width=100% height=100% src='/ProductList/ProductDetails'></iframe>",
            listeners: {
                close: function (e) {
                    winDetail = undefined;
                    tabs = new Array();
                }
            }
        }).show();
    }

}
//查看商品詳情資料時提供pid
function GetProductId() {
    return productId;
}
function showDetail(DetailId) {
    var record = orderListStore.getAt(orderListStore.find("Detail_Id", DetailId));
    if (record != undefined && record != null) {
        showDetailFun(record);
    }
}
function showChildProduct(DetailId) {
    var record = orderListStore.getAt(orderListStore.find("Detail_Id", DetailId));
    if (record != undefined && record != null) {
        var parentId = record.data.Parent_Id;
        var packId = record.data.pack_id;
        var orderId = record.data.Order_Id;
        var fatherName = record.data.Product_Name;
        olderChildStore.load({ params: { OrderId: orderId, ParentId: parentId, PackId: packId } });
        showChildFun(olderChildStore, fatherName);
    }
}

//供應商彈框信息
VendorFunction = function (id) {
    var VendorFrm = Ext.create('Ext.form.Panel', {
        id: 'VendorFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 120 },
        items: [
            {
                xtype: 'displayfield',
                name: 'vendor_name_simple',
                id: 'vendor_name_simple',
                submitValue: true,
                fieldLabel: '供應商'
            },
            {
                xtype: 'displayfield',
                name: 'company_person',
                id: 'company_person',
                submitValue: true,
                fieldLabel: '業務負責人'
            },
            {
                xtype: 'displayfield',
                name: 'company_phone',
                id: 'company_phone',
                submitValue: true,
                fieldLabel: '聯繫電話'
            },
            {
                xtype: 'displayfield',
                name: 'vendor_email',
                id: 'vendor_email',
                submitValue: true,
                fieldLabel: 'email'
            }
        ],
    });
    var vendorWin = Ext.create('Ext.window.Window', {
        title: '供應商詳情',
        id: 'vendorWin',
        iconCls: 'icon-user-edit',
        width: 450,
        height: 160,
        layout: 'fit',
        items: [VendorFrm],
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        constrain: true,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        margin: '5px 0px 0px 0px',
        closable: false,
        tools: [
            {
                type: 'close',
                qtip: "關閉",
                handler: function (event, toolEl, panel) {
                    Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp('vendorWin').destroy();
                        }
                        else {
                            return false;
                        }
                    });
                }
            }
        ],
        listeners: {
            'show': function () {
                //獲取供應商信息
                Ext.Ajax.request({
                    url: "/OrderManage/GetVendorDetail",
                    method: 'post',
                    reader: {
                        type: 'json',
                        root: 'data'
                    },
                    params: {
                        vid: id
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.getCmp("vendor_name_simple").setValue(result.data[0].vendor_name_simple);
                            Ext.getCmp("company_person").setValue(result.data[0].company_person);
                            Ext.getCmp("company_phone").setValue(result.data[0].company_phone);
                            Ext.getCmp("vendor_email").setValue(result.data[0].vendor_email);
                        }
                        else {
                            Ext.Msg.alert("提示信息", "獲取供應商信息失敗！");
                        }
                    }
                });
            }
        }
    });
    vendorWin.show();
};

onEditOrderNote = function () {
    editOrderNote()
}
onEditNoteAdmin = function () {
    editNoteAdmin();
}

//變更收貨人資訊
onModifyDeliverData = function () {
    modifyDeliverData();
}
onAddStatus = function () {
    addStatus();
}
//oneditUser = function () {
//    userEdit(document.getElementById('OrderId').value);
//}
//訂單退貨功能
onReturnClick = function () {
    var row = Ext.getCmp("orderListGrid").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行！");
    }
    else {
        //讀取id,確認退貨
        Ext.Msg.confirm("確認信息", Ext.String.format("確定要退這 {0} 個商品？", row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs = rowIDs + row[i].data.Detail_Id + ",";
                }
                //DesigneeFunction(rowIDs);
                Ext.Ajax.request({
                    url: '/OrderManage/OrderReturn',
                    method: 'post',
                    reader: {
                        type: 'json',
                        root: 'data'
                    },
                    params: {
                        order_id: document.getElementById('OrderId').value,
                        detail_id: rowIDs
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.msg == 2) {
                            Ext.Msg.alert("提示", "該訂單不可退！");
                        } else if (result.msg == 1) {
                            Ext.Msg.alert("錯誤提示", "新增退貨失敗！");
                        } else {
                            //Ext.Msg.alert("提示", "產生退貨單,退貨單編號為:" + result.return_id + "！請去退貨狀態頁面填寫退貨信息");
                            Ext.Msg.confirm("確認信息", Ext.String.format("產生退貨單,退貨單編號為:" + result.return_id + "！將跳轉 退貨狀態頁面 填寫客戶退貨信息.", row.length), function (btn) {
                                if (btn == 'yes') {
                                    TranToReturn();
                                }
                                else {
                                    orderListStore.load();
                                }
                            })
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert("錯誤提示", "退貨失敗,請聯繫管理員!");
                    }
                });
            }
        });
    }
}
function TranToReturn() {
    var url = '/OrderManage/OrderReturnList';
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#return');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'return',
        title: '退貨狀態查詢',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}
//等待付款
onWaitClick = function () {
    var WaitFrm = Ext.create('Ext.form.Panel', {
        id: 'WaitFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/OrderManage/OrderWaitClick',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 120 },
        items: [
            {
                xtype: 'displayfield',
                name: 'order_id',
                id: 'order_id',
                submitValue: true,
                value:GetOrderId(),
                fieldLabel: '訂單編號'
            },               
            {
                xtype: 'displayfield',
                name: 'return_note',
                id: 'return_note',
                submitValue: true,
                value: GetOrderstatus(),
                fieldLabel: '目前付款單狀態'
            },
            {
                xtype: 'textfield',
                name: 'remark',
                id: 'remark',
                allowBlank: false,
                submitValue: true,
                MaxValue:100,
                fieldLabel: '備註'
            }
        ],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            order_id: document.getElementById('OrderId').value,
                            remark:   Ext.getCmp('remark').getValue()
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.msg == 1) {
                                Ext.Msg.alert("提示", "控制器報錯！");
                            } else if (result.msg == 2) {
                                Ext.Msg.alert("錯誤提示", "sql執行錯！");
                            } else if (result.msg == 3) {
                                Ext.Msg.alert("錯誤提示", "slave表沒數據！");
                            } else {//轉等待付款成功刷新頁面
                                TranToDetial(document.getElementById('OrderId').value);
                            }
                            WaitWin.close();
                            orderListStore.load();
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    });
                }
            }
        }]
    });
    var WaitWin = Ext.create('Ext.window.Window', {
        title: '付款單待付款',
        id: 'WaitWin',
        iconCls: 'icon-user-edit',
        width: 550,
        height: 280,
        layout: 'fit',
        items: [WaitFrm],
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        constrain: true,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        margin: '5px 0px 0px 0px',
        closable: false,
        tools: [
            {
                type: 'close',
                qtip: "關閉",
                handler: function (event, toolEl, panel) {
                    Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp('WaitWin').destroy();
                        }
                        else {
                            return false;
                        }
                    });
                }
            }
        ],
        listeners: {
            'show': function () {
            }
        }
    });   
    WaitWin.show();
}
onChangePayment_cash = function () {
        var cashFrm = Ext.create('Ext.form.Panel', {
            id: 'cashFrm',
            frame: true,
            plain: true,
            defaultType: 'textfield',
            layout: 'anchor',
            autoScroll: true,
            labelWidth: 45,
            url: '/OrderManage/ChangePayment',
            defaults: { anchor: "95%", msgTarget: "side", labelWidth: 120 },
            items: [
                {
                    xtype: 'combobox',
                    allowBlank: true,
                    fieldLabel: '轉自取地址',
                    hidden: false,
                    id: 'deliver_store',
                    name: 'deliver_store',
                    store: ZQStore,
                    margin: '0 10 0 5',
                    displayField: 'ParameterName',
                    valueField: 'parameterCode',
                    typeAhead: true,
                    allowBlank:false,
                    forceSelection: false,
                    editable: false
                }
            ],
            buttons: [{
                text: SAVE,
                formBind: true,
                disabled: true,
                handler: function () {
                    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                    myMask.show();
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        form.submit({
                            params: {
                                payment: "PAYMENT_CASH",
                                delivery: '12',
                                order_id: document.getElementById('OrderId').value,
                                delivery_store: Ext.getCmp("deliver_store").getValue()
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                myMask.hide();
                                if (result.msg == 1) {
                                    Ext.Msg.alert("提示", "con報錯！");
                                } else if (result.msg == 2) {
                                    Ext.Msg.alert("錯誤提示", "sql執行錯！");
                                } else if (result.msg == 3) {
                                    Ext.Msg.alert("錯誤提示", "slave沒數據！");
                                } else {
                                    Ext.Msg.alert("提示", "轉自取成功!");
                                    TranToDetial(document.getElementById('OrderId').value);
                                }
                            },
                            failure: function () {
                                Ext.Msg.alert("提示", "出現異常!");
                                myMask.hide();
                            }
                        });
                    }
                }
            }]
        });
        var cashWin = Ext.create('Ext.window.Window', {
            title: '轉自取詳情',
            id: 'cashWin',
            iconCls: 'icon-user-edit',
            width: 400,
            height: 150,
            layout: 'fit',
            items: [cashFrm],
            closeAction: 'destroy',
            modal: true,
            resizable: false,
            constrain: true,
            labelWidth: 60,
            bodyStyle: 'padding:5px 5px 5px 5px',
            margin: '5px 0px 0px 0px',
            closable: false,
            tools: [
                {
                    type: 'close',
                    qtip: "關閉",
                    handler: function (event, toolEl, panel) {
                        Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                            if (btn == "yes") {
                                Ext.getCmp('cashWin').destroy();
                            }
                            else {
                                return false;
                            }
                        });
                    }
                }
            ]
        });
        cashWin.show();
}
    
onChangePayment_cat = function () {
    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
    myMask.show();
    Ext.Ajax.request({
        url: '/OrderManage/ChangePayment',
        method: 'post',
        reader: {
            type: 'json',
            root: 'data'
        },
        params: {
            payment: "T_CAT",
            delivery: '10',
            order_id: document.getElementById('OrderId').value,
        },
        success: function (form, action) {
            myMask.hide();
            var result = Ext.decode(form.responseText);
            myMask.hide();
            if (result.msg == 1) {
                Ext.Msg.alert("提示", "con報錯！");
            } else if (result.msg == 2) {
                Ext.Msg.alert("錯誤提示", "sql執行錯！");
            } else if (result.msg == 3) {
                Ext.Msg.alert("錯誤提示", "slave沒數據！");
            } else {
                TranToDetial(document.getElementById('OrderId').value);
                Ext.Msg.alert("提示", "轉黑貓貨到付款成功!");
            }
        },
        failure: function () {
            myMask.hide();
            Ext.Msg.alert("提示", "出現異常!");
}
    });
}
//訂單內容頁面刷新方法
function TranToDetial(orderId) {
    var url = '/OrderManage/OrderDetialList?Order_Id=' + orderId;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var old = panel.down('#detial');
    var copy = panel.down('#detiallist');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'detiallist',
        title: '訂單內容',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
    old.close();
}

//資安
function SecretLogin(rid, info_id, info_type) {//secretcopy
    var secret_type = "20";//參數表中的"訊息管理"可根據需要修改
    var url = "/OrderManage/GetData";//這個可能在後面的SecretController中用到
    var ralated_id = rid;
    //點擊機敏信息先保存記錄在驗證密碼是否需要輸入
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {//不准查看
        if (boolPassword) {//超過5分鐘沒有輸入密碼      
            SecretLoginFun(secret_type, ralated_id, true, true, false, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框

        } else {
            SecretLoginFun(secret_type, ralated_id, false, true, false, url, info_type, info_id, secret_info);//直接彈出顯示框
        }
    }
}
oneditUser = function (user_id) {
  //  alert(user_id);
   // var row = Ext.getCmp('OrderBrandProducesListGrid').getSelectionModel().getSelection();
    // alert(row[0].data.user_id);
    //editFunction(user_id);
    var secret_type = '20';
    var url = "/Member/UsersListIndex/Edit ";
    var ralated_id = user_id;
    var info_id = user_id;
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
//訂單退貨詳情
//DesigneeFunction = function (rowIDs) {
//    var editFrm1 = Ext.create('Ext.form.Panel', {
//        id: 'editFrm1',
//        frame: true,
//        plain: true,
//        defaultType: 'textfield',
//        layout: 'anchor',
//        autoScroll: true,
//        labelWidth: 45,
//        url: '/OrderManage/OrderReturn',
//        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 120 },
//        items: [
//            {
//                xtype: 'textfield',
//                name: 'name',
//                id: 'name',
//                allowBlank: false,
//                submitValue: true,
//                fieldLabel: '退貨人姓名'
//            },
//            {
//                xtype: 'textfield',
//                name: 'phone',
//                id: 'phone',
//                allowBlank: false,
//                submitValue: true,
//                fieldLabel: '退貨人手機'
//            },               
//            {
//                xtype: 'fieldcontainer',
//                layout: 'hbox',
//                fieldLabel: "取貨地點",
//                items: [   
//                    {
//                        xtype: 'combobox',
//                        id: 'adr',
//                        name: 'adr',
//                        store: ZipStore,
//                        valueField: 'zipcode',
//                        displayField: 'zipname',
//                        forceSelection: true,
//                        typeAhead: true,
//                        triggerAction: 'all'
//                    },
//                    {
//                        xtype: 'textfield',
//                        name: 'adderss',
//                        id: 'adderss',
//                        allowBlank: false,
//                        submitValue: true
//                    }
//                ]
//            },  
//            {
//                xtype: 'textfield',
//                name: 'return_note',
//                id: 'return_note',
//                allowBlank: false,
//                submitValue: true,
//                fieldLabel: '退貨原因'
//            }
//            //,
//            //{
//            //    xtype: 'textfield',
//            //    name: 'note',
//            //    id: 'note',
//            //    allowBlank: false,
//            //    submitValue: true,
//            //    fieldLabel: '備註'
//            //},
//            //{
//            //    xtype: 'textfield',
//            //    name: 'remarks',
//            //    id: 'remarks',
//            //    allowBlank: false,
//            //    submitValue: true,
//            //    fieldLabel: '管理者備註'
//            //}
//        ],
//        buttons: [{
//            text: SAVE,
//            formBind: true,
//            disabled: true,
//            handler: function () {
//                var form = this.up('form').getForm();
//                if (form.isValid()) {
//                    form.submit({
//                        params: {
//                            detail: rowIDs,
//                            name: Ext.htmlEncode(Ext.getCmp("name").getValue()),
//                            phone: Ext.htmlEncode(Ext.getCmp("phone").getValue()),
//                            zip: Ext.htmlEncode(Ext.getCmp("adr").getValue()),
//                            adderss: Ext.htmlEncode(Ext.getCmp("adderss").getValue()),
//                            return_note: Ext.htmlEncode(Ext.getCmp("return_note").getValue())
//                            //note: Ext.htmlEncode(Ext.getCmp("note").getValue()),
//                            //remarks: Ext.htmlEncode(Ext.getCmp("remarks").getValue())
//                        },
//                        success: function (form, action) {
//                            var result = Ext.decode(action.response.responseText);
//                            if (result.success) {
//                                Ext.Msg.alert(INFORMATION, "退貨申請成功!");
//                            }
//                            else {
//                                Ext.Msg.alert(INFORMATION, FAILURE);
//                            }
//                        },
//                        failure: function () {
//                            Ext.Msg.alert(INFORMATION, FAILURE);
//                        }
//                    });
//                }
//            }
//        }]
//    });
//    var editWin1 = Ext.create('Ext.window.Window', {
//        title: '退貨詳情',
//        id: 'editWin1',
//        iconCls: 'icon-user-edit',
//        width: 550,
//        height: 280,
//        layout: 'fit',
//        items: [editFrm1],
//        closeAction: 'destroy',
//        modal: true,
//        resizable: false,
//        constrain: true,
//        labelWidth: 60,
//        bodyStyle: 'padding:5px 5px 5px 5px',
//        margin: '5px 0px 0px 0px',
//        closable: false,
//        tools: [
//            {
//                type: 'close',
//                qtip: "關閉",
//                handler: function (event, toolEl, panel) {
//                    Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
//                        if (btn == "yes") {
//                            Ext.getCmp('editWin1').destroy();
//                        }
//                        else {
//                            return false;
//                        }
//                    });
//                }
//            }
//        ],
//        listeners: {
//            'show': function () {
//            }
//        }
//    });
//    editWin1.show();
//};



