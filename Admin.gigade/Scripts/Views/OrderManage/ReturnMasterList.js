var pageSize = 25;
/**********************************************************************退貨單主頁面**************************************************************************************/
//退貨單Model
Ext.define('gigade.ReturnMaster', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "return_id", type: "int" },
        { name: "order_id", type: "string" },
        { name: "vendor_id", type: "string" },
        { name: "vendor_name", type: "string" },
        { name: "return_status", type: "int" },
        { name: "return_createdate", type: "int" },
        { name: "createdate", type: "string" },
        { name: "invoice_deal", type: "string" },
        { name: "package", type: "string" },
        { name: "deliver_code", type: "string" },
        { name: "return_note", type: "string" },
        { name: "bank_note", type: "string" },
        { name: "return_updatedate", type: "int" },
        { name: "updatedate", type: "string" },
        { name: "return_ipfrom", type: "string" },
        { name: "return_zip", type: "string" },
        { name: "return_address", type: "string" },
        { name: "orc_remark", type: "string" },
        { name: "orc_service_remark", type: "string" },
    ]
});

var ReturnMasterStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.ReturnMaster',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetOrderReturnMasterList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdFgroup").down('#cancel').setDisabled(selections.length == 0);
            var row = Ext.getCmp("gdFgroup").getSelectionModel().getSelection();
            if (row != "") {
                for (var i = 0; i < row.length; i++) {
                    if (row[i].data.return_status != 0) {
                        Ext.getCmp("gdFgroup").down('#cancel').setDisabled(true);
                        break;
                    }
                }
            }
        }
    }
});

var DDLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有會員資料", "value": "0" },
        { "txt": "付款單號", "value": "1" },
        { "txt": "退貨單號", "value": "2" }
    ]
});

//日期條件store
var DateStore = Ext.create("Ext.data.Store", {
    fields: ["name", "value"],
    data: [
        { "name": "所有日期", "value": "0" },
        { "name": "建立日期", "value": "1" },
        { "name": "出貨日期", "value": "2" }
    ]
});

function Query(x) {
    if (Ext.getCmp('ven_type').getValue() > 0 && Ext.getCmp('search_content').getValue() == "") {
        Ext.Msg.alert(INFORMATION, "請輸入內容！");
    }
    else {
        ReturnMasterStore.removeAll();
        Ext.getCmp("gdFgroup").store.loadPage(1, {
            params: {
                ven_type: Ext.getCmp('ven_type').getValue(),
                search_content: Ext.getCmp('search_content').getValue(),
                date_type: Ext.getCmp('date_type').getValue(),
                time_start: Ext.getCmp('time_start').getValue(),
                time_end: Ext.getCmp('time_end').getValue(),
                return_status: Ext.getCmp('return_status').getValue().s,
            }
        });
    }
}
ReturnMasterStore.on('beforeload', function () {
    Ext.apply(ReturnMasterStore.proxy.extraParams, {
        ven_type: Ext.getCmp('ven_type').getValue(),
        search_content: Ext.getCmp('search_content').getValue(),
        date_type: Ext.getCmp('date_type').getValue(),
        time_start: Ext.getCmp('time_start').getValue(),
        time_end: Ext.getCmp('time_end').getValue(),
        return_status: Ext.getCmp('return_status').getValue().s,
    });
});
Ext.onReady(function () {

Ext.apply(Ext.form.field.VTypes, {
    //日期筛选
    daterange: function (val, field) {
        var date = field.parseDate(val);
        if (!date) {
            return false;
        }
        if (field.startDateField && (!this.dateRangeMax || (date.getTime() != this.dateRangeMax.getTime()))) {
            var start = field.up('form').down('#' + field.startDateField);
            start.setMaxValue(date);
            start.validate();
            this.dateRangeMax = date;
        }
        else if (field.endDateField && (!this.dateRangeMin || (date.getTime() != this.dateRangeMin.getTime()))) {
            var end = field.up('form').down('#' + field.endDateField);
            end.setMinValue(date);
            end.validate();
            this.dateRangeMin = date;
        }
        return true;
    },
    daterangeText: ''
});
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
            items: [
                {
                    xtype: 'combobox',
                    allowBlank: true,
                    hidden: false,
                    id: 'ven_type',
                    name: 'ven_type',
                    store: DDLStore,
                    queryMode: 'local',
                    width: 200,
                    labelWidth: 80,
                    margin: '5 10 0 5',
                    displayField: 'txt',
                    valueField: 'value',
                    typeAhead: true,
                    forceSelection: false,
                    editable: false,
                    fieldLabel: "查詢條件",
                    value: 0
                },
                {
                    xtype: 'textfield',
                    fieldLabel: "查詢內容",
                    width: 200,
                    labelWidth: 80,
                    margin: '5 10 0 0',
                    id: 'search_content',
                    name: 'search_content',
                    listeners: {
                        specialkey: function (field, e) {
                            if (e.getKey() == e.ENTER) {
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
            items: [
                {
                    xtype: 'combobox',
                    store: DateStore,
                    id: 'date_type',
                    name: 'date_type',
                    fieldLabel: "日期條件",
                    labelWidth: 80,
                    margin: '5 10 0 5',
                    displayField: 'name',
                    valueField: 'value',
                    typeAhead: true,
                    forceSelection: false,
                    editable: false,
                    queryMode: 'local',
                    value: 0
                },
                {
                    xtype: 'datetimefield',
                    id: 'time_start',
                    name: 'time_start',
                    margin: '5 10 0 0',
                    editable: false,
                    format: 'Y-m-d H:i:s',
                    time: { hour: 00, min: 00, sec: 00 },
                    value: setNextMonth(Today(), -1),
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("time_start");
                            var end = Ext.getCmp("time_end");
                            if (end.getValue() < start.getValue()) {
                                var start_date = start.getValue();
                                Ext.getCmp('time_end').setValue(new Date(start_date.getFullYear(), start_date.getMonth() + 1, start_date.getDate(), 23, 59, 59));
                            }
                        },
                        specialkey: function (field, e) {
                            if (e.getKey() == Ext.EventObject.ENTER) {
                                Query();
                            }
                        }
                    }
                },
                { xtype: 'displayfield', margin: '5 10 0 0', value: '~'},
                {
                    xtype: "datetimefield",
                    time: { hour: 23, min: 59, sec: 59 },
                    id: 'time_end',
                    name: 'time_end',
                    margin: '5 10 0 0',
                    editable: false,
                    format: 'Y-m-d H:i:s',
                    value: Today(),
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("time_start");
                            var end = Ext.getCmp("time_end");
                            if (end.getValue() < start.getValue()) {
                                var end_date = end.getValue();
                                Ext.getCmp('time_start').setValue(new Date(end_date.getFullYear(), end_date.getMonth() - 1, end_date.getDate()));
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
            layout: 'hbox',
            items: [
            {
                xtype: 'radiogroup',
                id: 'return_status',
                labelWidth: 80,
                colName: 'return_status',
                fieldLabel: "歸檔狀態",
                width: 400,
                margin: '5 10 0 5',
                columns: 4,
                vertical:true,
                items: [
                    { boxLabel: "所有狀態", name: 's', inputValue: '3', checked: true },
                    { boxLabel: "未歸檔 ", name: 's', inputValue: '0' },
                    { boxLabel: "歸檔 ", name: 's', inputValue: '1' },
                    { boxLabel: "取消退貨 ", name: 's', inputValue: '2' }
                ]
            },
            {
                xtype: 'button',
                text: "查詢",
                width: 70,
                margin: '5 10 0 5',
                handler: Query
            },
            {
                xtype: 'button',
                text: "重置",
                width: 70,
                margin: '5 20 0 5',
                handler: function () {
                    this.up('form').getForm().reset();
                }
            }
        ]}
    ]
});
    var gdFgroup = Ext.create('Ext.grid.Panel', {
        id: 'gdFgroup',
        store: ReturnMasterStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 8.1,
        columns: [
            { header: "退貨單號", dataIndex: 'return_id', width: 80, align: 'center'},
            {
                header: "付款單號", dataIndex: 'order_id', width: 150, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return '<a href=javascript:TransToOrder(' + record.data.order_id + ') >' + record.data.order_id + '</a>';
                }
            },
            { header: "出貨商", dataIndex: 'vendor_name', width: 120, align: 'center' },
            {
                header: "狀態", dataIndex: 'return_status', width: 100, align: 'center', renderer: function (value) {
                    if (value == 0) {
                        return Ext.String.format('<font color="#FF0000">{0}</font>', '待歸檔');
                    }
                    else if (value == 1) {
                        return Ext.String.format('{0}', '歸檔');
                    }
                    else if (value == 2) {
                        return Ext.String.format('<font color="#FF0000">{0}</font>', '取消退貨');
                    }
                    else {
                        return Ext.String.format('{0}', value);
                    }
                }
            },
            { header: "建立時間", dataIndex: 'createdate', width: 180, align: 'center' },
            { header: "使用者備註", dataIndex: 'orc_remark', width: 200 },
                { header: "管理者備註", dataIndex: 'orc_service_remark', width: 200 }

        ],
        tbar: [
     //       { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
                 {
                     xtype: 'button', text: "取消退貨", id: 'cancel', hidden: false, iconCls: 'icon-user-edit', disabled: true,
                     handler: onCancelClick
                 },
        ],

        bbar: Ext.create('Ext.PagingToolbar', {
            store: ReturnMasterStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
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

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm, gdFgroup],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdFgroup.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
   // ReturnMasterStore.load({ params: { start: 0, limit: 25 } });
});
/******取消退貨*********/
onCancelClick = function () {
    var row = Ext.getCmp("gdFgroup").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        Ext.MessageBox.confirm("提示信息", "確認取消退貨？", function (btn) {
            if (btn == "yes") {
                Ext.Ajax.request({
                    url: '/OrderManage/CancelReturnPurchaes',
                    params: {
                        return_id: row[0].data.return_id,
                        order_id: row[0].data.order_id,
                        return_status: row[0].data.return_status,
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert("提示信息", "操作成功！");
                            ReturnMasterStore.load();
                        }
                        else {
                            if (result.msg == '1') {
                                Ext.Msg.alert("提示信息", "已通知物流取貨，不可退");
                            }
                            else {
                                Ext.Msg.alert("提示信息", "操作失敗！");
                            }
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert("提示信息", "出現異常！");
                    }
                });
            }
            else {
                return false;
            }
        });
    
    }
}
function TransToOrder(orderId) {
    var url = '/OrderManage/OrderDetialList?Order_Id=' + orderId;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#ReturnMasterList');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'ReturnMasterList',
        title: '訂單內容',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();

}
function Tomorrow() {
    var d;
    d = new Date();
    d.setDate(d.getDate() + 1);
    return d;
}


function Today() {
    var d;
    var dt;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                     // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += d.getDate();
    dt = new Date(s);
    dt.setDate(dt.getDate());
    dt.setHours(23, 59, 59);
    return dt;                                 // 返回日期。
}

function setNextMonth(source, n) {
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