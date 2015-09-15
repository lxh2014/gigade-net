Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
//調度狀態 
var SchedulingStatuStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": '全部', "value": "-1" },
        { "txt": '無', "value": "0" },
        { "txt": '有', "value": "1" }
    ]
});
//出货日期
var datequeryStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": '所有日期', "value": "0" },
        { "txt": '出貨日期', "value": "1" }
    ]
});
//出貨篩選
var ShipmentScreeningStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": '全部', "value": "-1" },
        { "txt": '一般出貨', "value": "0" },
        { "txt": '重點出貨', "value": "1" }
    ]
});
//出貨查詢Model
Ext.define('GIGADE.deliver_master', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'overdue_day', type: 'int' },//逾期天數：出货日期-付款单成立日期。 出货日期当天计算
        { name: 'type', type: 'string' },//出貨類別 通過這個來判斷是訂單編號還是批次出貨編號
        { name: 'deliver_id', type: 'int' },//出貨編號
        { name: 'delivery_status', type: 'int' },//出貨狀態
        { name: 'freight_set', type: 'int' },//運送方式：freight_set 1 => '常溫',  2 => '冷凍', 5 => '冷藏'
        { name: 'ticket_id', type: 'string' },//
        { name: 'export_id', type: 'int' },//出貨商編號
        { name: 'delivery_store', type: 'int' },//物流業者
        { name: 'delivery_code', type: 'string' },//物流單號
        { name: 'delivery_freight_cost', type: 'int' },//物流費
        { name: 'delivery_date', type: 'string' },//出貨時間
        { name: 'arrival_date', type: 'string' },//到貨時間
        { name: 'estimated_delivery_date', type: 'string' },//預計出貨日期
        { name: 'estimated_arrival_date', type: 'string' },//預計到貨日期
        { name: 'estimated_arrival_period', type: 'string' },//預計到貨時段：0 => '不限時', 1 => '12:00以前',2 => '12:00-17:00', 3 => '17:00-20:00'
        { name: 'delivery_name', type: 'string' },//收貨人
        { name: 'order_id', type: 'int' },//訂單號
        { name: 'ShipmentName', type: 'string' },//物流商
        { name: 'vendor_name_simple', type: 'string' },//出貨商名稱
        { name: 'warehouse_status', type: 'int' },//調度：Ticket.warehouse_status 存在，输出调度，不存在 为空 
        { name: 'order_createtime', type: 'string' },//付款單成立日期
        { name: 'states', type: 'string' },//訂單狀態
        { name: 'order_status', type: 'int' },//訂單狀態
        { name: 'logisticsType', type: 'int' },
        { name: 'LogisticsStatus',type:'string' }//物流状态
    ]
});

//出貨查詢列表Store
var DeliversListStore = Ext.create("Ext.data.Store", {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'GIGADE.deliver_master',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/SendProduct/DeliversList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
//加載前
DeliversListStore.on('beforeload', function () {
    Ext.apply(DeliversListStore.proxy.extraParams, {
        //search: document.getElementById("Search").value,
        //type: document.getElementById("Delivery_Type").value,
        //delivery_status: document.getElementById("Delivery_Status").value
        type: Ext.getCmp('shipmenttype').getValue(),//出貨類別--type
        delivery_status: Ext.getCmp('shipmentstatus').getValue(),//出貨狀態--delivery_status
        export_id: Ext.getCmp('vendorcondition').getValue(),//出貨廠商--export_id
        delivery_store: Ext.getCmp('shipment').getValue(),//物流商-- delivery_store
        warehouse_statu: Ext.getCmp('scheduling').getValue(),//調度狀態--warehouse_status
        priority: Ext.getCmp('screen').getValue(),//出貨篩選--OrderMaster`.`priority` = 1
        datequery: Ext.getCmp('datequery').getValue(),//日期條件 --datequery
        time_start: Ext.getCmp('time_start').getValue(),//開始時間--time_start--delivery_date
        time_end: Ext.getCmp('time_end').getValue(),//結束時間--time_end--delivery_date
        search: Ext.getCmp('search').getValue()//搜索內容--deliver_id or delivery_code or delivery_name,delivery_mobile,vendor_name_simple 
    })

});

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
//獲取grid中的數據
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


var i = 1;
Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 120,
        //flex:1.5,
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'combobox',
                        id: 'shipmenttype',
                        margin: '0 5px',
                        fieldLabel: '出貨類別',
                        colName: 'shipmenttype',
                        queryMode: 'local',
                        editable: false,
                        store: ShipCategoryStore,
                        displayField: 'txt',
                        valueField: 'value',
                        value: 1,
                        listeners: {
                            change: function (ths, newValue, oldValue, eOpts) {
                                if (i != 1) {
                                    //當選擇的是供應商調度出貨的話
                                    if (newValue == '3') {
                                        Ext.getCmp('o_id').hide();
                                        Ext.getCmp('po_id').show();
                                        Ext.getCmp('order_s').hide();
                                    }
                                    else {
                                        Ext.getCmp('po_id').hide();
                                        Ext.getCmp('o_id').show();
                                        Ext.getCmp('order_s').show();
                                    }
                                  
                                }
                                else {
                                    i++;
                                }
                              
                            }
                        }
                    },
                    {
                        xtype: 'combobox',
                        id: 'shipmentstatus',
                        margin: '0 5px',
                        fieldLabel: '出貨狀態',
                        colName: 'shipmentstatus',
                        queryMode: 'local',
                        editable: false,
                        store: DeliveryStatusStore,
                        displayField: 'txt',
                        valueField: 'value',
                        value: -1
                    },
                    {
                        xtype: 'combobox',
                        id: 'vendorcondition',
                        margin: '0 5px',
                        fieldLabel: '出貨廠商',
                        colName: 'vendorcondition',
                        queryMode: 'local',
                        editable: false,
                        store: VendorConditionStore,
                        displayField: 'vendor_name_simple',
                        valueField: 'vendor_id',
                        listeners: {
                            beforerender: function () {
                                VendorConditionStore.load({
                                    callback: function () {
                                        VendorConditionStore.insert(0, { vendor_id: '0', vendor_name_simple: '全部' });
                                        Ext.getCmp("vendorcondition").setValue(VendorConditionStore.data.items[0].data.vendor_id);
                                    }
                                });
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
                        xtype: 'combobox',
                        id: 'shipment',
                        margin: '0 5px',
                        fieldLabel: '物流商',
                        colName: 'shipment',
                        queryMode: 'local',
                        editable: false,
                        store: DeliverStore,
                        displayField: 'parameterName',
                        valueField: 'parameterCode',
                        listeners: {
                            beforerender: function () {
                                DeliverStore.load({
                                    callback: function () {
                                        DeliverStore.insert(0, { parameterCode: '0', parameterName: '全部' });
                                        Ext.getCmp("shipment").setValue(DeliverStore.data.items[0].data.parameterCode);
                                    }
                                });
                            }
                        }
                    },
                    {
                        xtype: 'combobox',
                        id: 'scheduling',
                        margin: '0 5px',
                        fieldLabel: '調度狀態',
                        colName: 'scheduling',
                        queryMode: 'local',
                        editable: false,
                        store: SchedulingStatuStore,
                        displayField: 'txt',
                        valueField: 'value',
                        value: -1
                    },
                    {
                        xtype: 'combobox',
                        id: 'screen',
                        margin: '0 5px',
                        fieldLabel: '出貨篩選',
                        colName: 'screen',
                        queryMode: 'local',
                        editable: false,
                        store: ShipmentScreeningStore,
                        displayField: 'txt',
                        valueField: 'value',
                        value: -1
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'combobox',
                        id: 'datequery',
                        margin: '0 5px',
                        fieldLabel: '日期條件',
                        colName: 'datequery',
                        queryMode: 'local',
                        editable: false,
                        store: datequeryStore,
                        displayField: 'txt',
                        valueField: 'value',
                        value: 0
                    },
                    {
                        xtype: 'datefield',
                        id: 'time_start',
                        name: 'time_start',
                        margin: '0 5px 0 5px',
                        editable: false,
                        value: Tomorrow(1 - new Date().getDate())
                    },
                    {
                        xtype: 'displayfield',
                        value: '~'
                    },
                    {
                        xtype: 'datefield',
                        id: 'time_end',
                        name: 'time_end',
                        margin: '0 5px',
                        editable: false,
                        value: Tomorrow(0)
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: '搜索',
                        id: 'search',
                        margin: '0 5px',
                        //hidden: true,
                        submitValue: false,
                        name: 'search',
                        value: document.getElementById("Search").value
                    },
                    {
                        xtype: 'button',
                        margin: '0 10 0 10',
                        iconCls: 'icon-search',
                        text: "查詢",
                        handler: Query
                    },
                    {
                        xtype: 'button',
                        text: '重置',
                        id: 'btn_reset',
                        iconCls: 'ui-icon ui-icon-reset',
                        listeners: {
                            click: function () {
                                Ext.getCmp('shipmenttype').setValue(1);//出貨類別--type
                                Ext.getCmp('shipmentstatus').setValue(-1);//出貨狀態--delivery_status
                                Ext.getCmp('vendorcondition').setValue(0);//出貨廠商--export_id
                                Ext.getCmp('shipment').setValue(0);//物流商-- delivery_store
                                Ext.getCmp('scheduling').setValue(-1);//調度狀態--warehouse_status
                                Ext.getCmp('screen').setValue(-1);//出貨篩選--OrderMaster`.`priority` = 1
                                Ext.getCmp('datequery').setValue(0);//日期條件 --datequery
                                Ext.getCmp('time_start').setValue(Tomorrow(1 - new Date().getDate()));//開始時間--time_start--delivery_date
                                Ext.getCmp('time_end').setValue(Tomorrow(0));//結束時間--time_end--delivery_date
                                Ext.getCmp('search').setValue("");//搜索內容--deliver_id or delivery_code or delivery_name,delivery_mobile,vendor_name_simple
                            }
                        }
                    }
                ]
            }
        ], listeners: {
            beforerender: function () {
                var delivery_statu = document.getElementById("Delivery_Status").value;
                var sear = document.getElementById("Search").value;
                var type = document.getElementById("Delivery_Type").value;
                if (delivery_statu != "") {
                    Ext.getCmp('shipmentstatus').setValue(delivery_statu);
                }
                if (sear != "") {
                    Ext.getCmp('search').setValue(sear);
                }
                if (type != "") {
                    Ext.getCmp('shipmenttype').setValue(type);
                }
                //DeliverStore.load({
                //    callback: function () {
                //        DeliverStore.insert(0, { parameterCode: '0', parameterName: '全部' });
                //        Ext.getCmp("shipment").setValue(DeliverStore.data.items[0].data.parameterCode);
                //    }
                //});
            }
        }
    });

   
    //頁面加載時創建grid
    var VendorListGrid = Ext.create('Ext.grid.Panel', {
        id: 'VendorListGrid',
        store: DeliversListStore,
        // height: 645,
        flex: 8.5,
        columnLines: true,
        frame: true,
        columns: [
            {
                header: '逾期天數', dataIndex: 'overdue_day', width: 60, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.order_status == 99) {
                        return "<div style='color:#AAAAEE;'> " + value + "</div>";
                    } else if (record.data.order_status == 4 && record.data.delivery_status == 3) {
                        return "<div style='color:#EEEEEE;'> " + value + "</div>";
                    } else if (record.data.order_status == 90 && record.data.delivery_status == 6) {
                        return "<div style='color:#EEAAAA;'> " + value + "</div>";
                    } else if (record.data.overdue_day > 0) {
                        return "<div style='background-color:#FFAAAA;'> " + value + "</div>";
                    } else if (record.data.overdue_day < 1 && record.data.overdue_day > -3) {
                        return "<div style='background-color:#FFFFAA;color:#FF0000;'> " + value + "</div>";
                        //return '<font color="purple" > ' + value + '</font>';
                    } else {
                        return "<div style='color:#888888;'> " + value + "</div>";
                    }
                }
            },
            { header: '出货编号', dataIndex: 'deliver_id', width: 70, align: 'center' },
            {
                header: '訂單編號', dataIndex: 'order_id', id: 'o_id', width: 90, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return '<a href=javascript:TransToOrderDetial(' + record.data.order_id + ') >' + record.data.order_id + '</a>';
                }
            },
            {
                 header: '批次出貨編號', dataIndex: 'order_id', id: 'po_id', width: 90, align: 'center', hidden: true
                 //,
                 //renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                 //    if (record.data.type == '3') {
                 //        return record.data.order_id;
                 //    } else {
                 //        return '<a href=javascript:TransToDetial(' + record.data.order_id + ') >' + record.data.order_id + '</a>';
                 //        //Ext.String.format("<a  href='www.baidu.com?oid={0}' target='_blank' style='text-decoration:none'>{1}</a>", record.data.order_id, record.data.order_id);
                 //    }
                 //}
            },
            {
                header: '出貨廠商', dataIndex: 'vendor_name_simple', width: 110, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                   // return Ext.String.format("<a href='javascript:void({0});'  onclick='TransToVendor()'>{1}</a>", record.data.export_id, value);
                    //  return '<a href="#"  onclick="showVendorDetail(' + record.data.export_id + ')">' + value+'</a>';
                    return '<a href="#"  onclick="showVendorDetail(' + record.data.export_id + ')">' + value + '</a>';
                }
            }, 
            {
                header: '運送方式', dataIndex: 'freight_set', width: 60, align: 'center'
            },
            { header: '收貨人', dataIndex: 'delivery_name', width: 80, align: 'center' },
            { header: '物流業者', dataIndex: 'ShipmentName', width: 90, align: 'center' },
            {
                header: '調度', dataIndex: 'warehouse_status', width: 60, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.warehouse_status != 0) {
                        return "調度"
                    }
                }
            },
            { header: '物流單號', dataIndex: 'delivery_code', width: 100, align: 'center' },
            { header: '物流費', dataIndex: 'delivery_freight_cost', width: 50, align: 'center' },
            {
                header: '付款單成立日期', dataIndex: 'order_createtime', width: 95, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value.substr(0, 10) == "0001-01-01") {
                        return "N/A";
                    }
                    else {
                        return value.substr(0, 10);
                    }
                    
                }
            },
            {
                header: '預計出貨日期', dataIndex: 'estimated_delivery_date', width: 85, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value.substr(0, 10) == "0001-01-01") {
                        return "N/A";
                    }
                    else {
                        return value.substr(0, 10);
                    }
                }
            },
            {
                header: '預計到貨日期', dataIndex: 'estimated_arrival_date', width: 85, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value.substr(0, 10) == "0001-01-01") {
                        return "N/A";
                    }
                    else {
                        return value.substr(0, 10);
                    }
                }
            },
            {
                header: '預計到貨時段', dataIndex: 'estimated_arrival_period', width: 86, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.estimated_arrival_period == 0) {
                        return "";
                    } else if (record.data.estimated_arrival_period == 1) {
                        return "12:00以前";
                    } else if (record.data.estimated_arrival_period == 2) {
                        return "12:00-17:00";
                    } else if (record.data.estimated_arrival_period == 3) {
                        return "17:00-20:00";
                    } else {
                        return record.data.estimated_arrival_period;
                    }
                }
            },
            {
                header: '出貨時間', dataIndex: 'delivery_date', width: 93, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "0001-01-01 00:00:00 ") {
                        return "N/A";
                    } else {
                        return value;
                    }
                }
            },
            {
                header: '到貨時間', dataIndex: 'arrival_date', width: 93, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "0001-01-01 00:00:00 ") {
                        return "N/A";
                    } else {
                        return value;
                    }
                }
            },
            { header: '訂單狀態', dataIndex: 'states', width: 92,id:'order_s', align: 'center' },
            {
                header: '出貨單狀態', dataIndex: 'delivery_status', width: 80, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (record.data.delivery_status) {
                        case 0:
                            return "待出貨";
                            break;
                        case 1:
                            return "可出貨";
                            break;
                        case 2:
                            return "出貨中";
                            break;
                        case 3:
                            return "已出貨";
                            break;
                        case 4:
                            return "已到貨";
                            break;
                        case 5:
                            return "未到貨";
                            break;
                        case 6:
                            return "取消出貨";
                            break;
                        case 7:
                            return "待取貨";
                            break;
                        default:
                            return "意外數據錯誤";
                            break;
                    }
                }
            },
            { header: '物流狀態', dataIndex: 'LogisticsStatus', width: 70, align: 'center' },
            {
                header: '功能', dataIndex: 'deliver_id', width: 60, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    //if (value != null) {
                    //    return Ext.String.format("<a href=javascript:TranToDetial('/SendProduct/DeliverView?deliver_id={0}' target='_blank' >檢視</a>", record.data.deliver_id);
                    //    //return Ext.String.format("<a href='www.baidu.com?deliver_id={0}' target='_blank' >檢視</a>", record.data.deliver_id);
                    //    //return Ext.String.format("<a href=javascript:TranToDetial('/SendProduct/DeliverView','{0}')>檢視</a>", record.data.deliver_id);
                    //}
                    return '<a href=javascript:TranToDetial("/SendProduct/DeliverView","' + value + '")>檢視</a>';
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
        tbar: [
          {
              xtype: 'button',
              text: '匯出CSV',
              iconCls: 'icon-excel',
              id: 'btnExcel',
              handler: function () {
                  var type = Ext.getCmp('shipmenttype').getValue();//出貨類別--type
                  var delivery_status = Ext.getCmp('shipmentstatus').getValue();//出貨狀態--delivery_status
                  var export_id = Ext.getCmp('vendorcondition').getValue();//出貨廠商--export_id
                  var delivery_store = Ext.getCmp('shipment').getValue();//物流商-- delivery_store
                  var warehouse_statu = Ext.getCmp('scheduling').getValue();//調度狀態--warehouse_status
                  var priority = Ext.getCmp('screen').getValue();//出貨篩選--OrderMaster`.`priority` = 1
                  var datequery = Ext.getCmp('datequery').getValue();//日期條件 --datequery
                 //var time_start=Ext.getCmp('time_start').getValue();//開始時間--time_start--delivery_date
                  //var time_end= Ext.getCmp('time_end').getValue();//結束時間--time_end--delivery_date
                 var time_start = Ext.Date.format(Ext.getCmp('time_start').getValue(), 'Y-m-d');//開始時間--time_start--delivery_date
                 var time_end = Ext.Date.format(Ext.getCmp('time_end').getValue(), 'Y-m-d');//結束時間--time_end--delivery_date
                  var search = Ext.getCmp('search').getValue();//搜索內容--deliver_id or delivery_code or delivery_name,delivery_mobile,vendor_name_simple
                 var paras = "?type=" + type + "&delivery_status=" + delivery_status + "&export_id=" + export_id + "&delivery_store=" + delivery_store + "&warehouse_statu=" + warehouse_statu + "&priority=" + priority + "&datequery=" + datequery + "&time_start=" + time_start + "&time_end=" + time_end + "&search=" + search;
                  window.open('/SendProduct/DeliversExport' + paras);
                  }
              // ,handler: Query 
          }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: DeliversListStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        })
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
    DeliversListStore.load({ params: { start: 0, limit: 25 } });
    VendorConditionStore.load({
        callback: function () {
            VendorConditionStore.insert(0, { vendor_id: '0', vendor_name_simple: '全部' });
            Ext.getCmp("vendorcondition").setValue(VendorConditionStore.data.items[0].data.vendor_id);
        }
    });
})
function Tomorrow(days) {
    var d;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                     // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += d.getDate() + days;                          // 获取日。
    return (new Date(s));                                 // 返回日期。
}


//查询
Query = function () {
    DeliversListStore.removeAll();
    Ext.getCmp("VendorListGrid").store.loadPage(1, {
        params: {
            type: Ext.getCmp('shipmenttype').getValue(),//出貨類別--type
            delivery_status: Ext.getCmp('shipmentstatus').getValue(),//出貨狀態--delivery_status
            export_id: Ext.getCmp('vendorcondition').getValue(),//出貨廠商--export_id
            delivery_store: Ext.getCmp('shipment').getValue(),//物流商-- delivery_store
            warehouse_statu: Ext.getCmp('scheduling').getValue(),//調度狀態--warehouse_status
            priority: Ext.getCmp('screen').getValue(),//出貨篩選--OrderMaster`.`priority` = 1
            datequery: Ext.getCmp('datequery').getValue(),//日期條件 --datequery
            time_start: Ext.getCmp('time_start').getValue(),//開始時間--time_start--delivery_date
            time_end: Ext.getCmp('time_end').getValue(),//結束時間--time_end--delivery_date
            search: Ext.getCmp('search').getValue()//搜索內容--deliver_id or delivery_code or delivery_name,delivery_mobile,vendor_name_simple 
        }
    });
}

//跳轉到出貨明細頁
function TranToDetial(url, deliver_id) {
    var urlTran = url + '?deliver_id=' + deliver_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#eledetial');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'eledetial',
        title: "出貨明細",
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}
function TransToOrderDetial(orderId) {

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



function showVendorDetail(export_id) {
   // Ext.Msg.alert(export_id);
    VendorListStore.load({
        params: { VendorId: export_id },
        callback: function (records, options, success) {
            editFunction(records[0], VendorListStore, false);
        }

    });
}

//跳轉到供應商編輯頁面
//function TransToVendor()
//{


//}
