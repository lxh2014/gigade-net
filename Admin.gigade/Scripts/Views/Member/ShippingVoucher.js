Ext.apply(Ext.form.VTypes, {
    daterange: function (val, field) {
        var date = field.parseDate(val);
        if (!date) {
            return;
        }
        if (field.startDateField && (!this.dateRangeMax || (date.getTime() != this.dateRangeMax.getTime()))) {
            var start = Ext.getCmp(field.startDateField);
            start.setMaxValue(date);
            start.validate();
            this.dateRangeMax = date;
        } else if (field.endDateField && (!this.dateRangeMin || (date.getTime() != this.dateRangeMin.getTime()))) {
            var end = Ext.getCmp(field.endDateField);
            end.setMinValue(date);
            end.validate();
            this.dateRangeMin = date;
        }
        return true;
    }
});
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
var pageSize = 25;
Ext.define('gigade.ShippingVoucherModel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "sv_id", type: "int" },
        { name: "user_id", type: "int" },
        { name: "user_name", type: "string" },
        { name: "ml_name", type: "string" },
        { name: "sv_year", type: "int" },
        { name: "sv_month", type: "int" },
        { name: "sv_created", type: "string" },
        { name: "state_name", type: "string" },
        { name: "sv_modified", type: "string" },
        { name: "order_id", type: "int" }
    ]
});
var ShippingVoucherStore = Ext.create('Ext.data.Store', {
    model: 'gigade.ShippingVoucherModel',
    autoDestroy: true,
    autoLoad: false,
    pageSize: pageSize,
    proxy: {
        type: 'ajax',
        url: '/Member/GetShippingVoucher',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
var stateStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "全部", "value": "3" },
        { "txt": "未使用", "value": "0" },
        { "txt": "已使用", "value": "1" },
        { "txt": "未使用已到期", "value": "2" }
    ]
});


Ext.onReady(function () {
    var ShippingVoucherPanel = Ext.create('Ext.grid.Panel', {
        id: 'ShippingVoucherPanel',
        flex: 1.8,
        store: ShippingVoucherStore,
        width: document.documentElement.clientWidth,
        columnLines: true,//顯示列線條
        frame: true,//Panel是圆角框显示
        columns: [
            { header: "流水號", dataIndex: 'sv_id', width: 100, align: 'center' },
            { header: "訂單編號", dataIndex: 'order_id', width: 100, align: 'center' },
            { header: "會員編號", dataIndex: 'user_id', width: 100, align: 'center' },
            { header: "會員名稱", dataIndex: 'user_name', width: 100, align: 'center' },
              { header: "會員等級", dataIndex: 'ml_name', width: 100, align: 'center' },
               { header: "發放年份", dataIndex: 'sv_year', width: 100, align: 'center' },
                { header: "發放月份", dataIndex: 'sv_month', width: 70, align: 'center' },
                    {
                        header: "發放時間", dataIndex: 'sv_created', width: 150, align: 'center'
                    },
                    {
                        header: "更新時間", dataIndex: 'sv_modified', width: 150, align: 'center'
                    },
                    { header: "狀態", dataIndex: 'state_name', width: 100, align: 'center' }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ShippingVoucherStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY,
            handler: Query
        })
    });
    ShippingVoucherStore.on('beforeload', function () {
        Ext.apply(ShippingVoucherStore.proxy.extraParams, {
            oid: Ext.getCmp('oid').getValue(),
            time_start: Ext.getCmp('start_time').getValue(),
            time_end: Ext.getCmp('end_time').getValue(),
            username: Ext.getCmp('username').getValue(),
            state: Ext.getCmp("stateleibie").getValue()
        });
    });
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 120,
        border: 0,
        bodyPadding: 13,
        width: document.documentElement.clientWidth,
        items: [
                {
                    xtype: 'fieldcontainer',
                    combineErrors: true,
                    layout: 'hbox',
                    margin: '0 0 18 0',
                    items: [
                        {
                            xtype: 'numberfield',
                            allowBlank: true,
                            fieldLabel: "會員/訂單編號",
                            margin: '0 0 0 0',
                            labelWidth: 100,
                            width: 254,
                            id: 'oid',
                            name: 'searchcontentid',
                            minValue: 0,
                            //regex: /^\d+$/,
                            //regexText: '请输入數字',
                            listeners: {
                                specialkey: function (field, e) {
                                    if (e.getKey() == Ext.EventObject.ENTER) {
                                        Query();
                                    }
                                }
                            }
                        },
                            {
                                xtype: 'textfield',
                                allowBlank: true,
                                fieldLabel: "會員名稱",
                                margin: '0 0 0 10',
                                labelWidth: 70,
                                width: 220,
                                id: 'username',
                                name: 'searchcontentname',
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
                    margin: '0 0 0 0',
                    items: [
                        {
                            xtype: 'displayfield',
                            value: '發放起止時間:'
                        },
                       {
                           xtype: "datetimefield",
                           editable: false,
                           margin: '0 0 0 14',
                           id: 'start_time',
                           name: 'start_time',
                           format: 'Y-m-d  H:i:s',
                           time: { hour: 00, min: 00, sec: 00 },//標記結束時間00:00:00
                           width: 150,
                           //vtype: 'daterange',
                           //endDateField: 'end_time',
                           listeners: {
                               specialkey: function (field, e) {
                                   if (e.getKey() == Ext.EventObject.ENTER) {
                                       Query();
                                   }
                               }
                               , select: function () {
                                   var start = Ext.getCmp("start_time");
                                   var end = Ext.getCmp("end_time");
                                   if (end.getValue() == null) {
                                       end.setValue(setNextMonth(start.getValue(), 1));
                                   }
                                   else if (end.getValue() < start.getValue()) {
                                       Ext.Msg.alert(INFORMATION, DATA_TIP);
                                       end.setValue(setNextMonth(start.getValue(), 1));
                                   }
                               }
                           }
                       },
                       { xtype: 'displayfield', value: '~ ', margin: '0 0 0 10', },
                       {
                           xtype: "datetimefield",
                           editable: false,
                           margin: '0 0 0 10',
                           id: 'end_time',
                           name: 'end_time',
                           format: 'Y-m-d  H:i:s',
                           time: { hour: 23, min: 59, sec: 59 },//標記結束時間00:00:00
                           width: 150,
                           //vtype: 'daterange',
                           //startDateField: 'start_time',
                           listeners: {
                               specialkey: function (field, e) {
                                   if (e.getKey() == Ext.EventObject.ENTER) {
                                       Query();
                                   }
                               }
                               , select: function () {
                                   var start = Ext.getCmp("start_time");
                                   var end = Ext.getCmp("end_time");                                 
                                   if (start.getValue() != "" && start.getValue() != null) {
                                       if (end.getValue() < start.getValue()) {
                                           Ext.Msg.alert(INFORMATION, DATA_TIP);
                                           start.setValue(setNextMonth(end.getValue(), -1));
                                       }                                      
                                   }
                                   else {
                                       start.setValue(setNextMonth(end.getValue(), -1));
                                   }
                               }
                           }
                       },
                       {
                           xtype: 'combobox',
                           store: stateStore,
                           queryMode: 'local',
                           fieldLabel: "狀態",
                           margin: '0 0 0 10',
                           allowBlank: true,//可以為空
                           editable: false,//阻止直接在表单项的文本框中输入字符
                           id: 'stateleibie',
                           name: 'stateleibie',
                           displayField: 'txt',
                           valueField: 'value',
                           emptyText: '請選擇',
                           labelWidth: 60,
                           width: 160,
                           value: 3,
                           lastQuery: ''
                       }
                    ]
                },

                {
                    xtype: 'fieldcontainer',
                    combineErrors: true,//如果设置为 true, 则 field 容器自动将其包含的所有属性域的校验错误组合为单个错误信息, 并显示到 配置的 msgTarget 上. 默认值 false.
                    layout: 'hbox',
                    margin: '10 0 0 0',
                    items:
                     [

                        {
                            xtype: 'button',
                            text: SEARCH,
                            iconCls: 'icon-search',
                            id: 'btnQuery',
                            handler: Query
                        },
                         {
                             xtype: 'button',
                             text: RESET,
                             margin: '0 0 0 10',
                             id: 'btn_reset1',
                             iconCls: 'ui-icon ui-icon-reset',
                             listeners: {
                                 click: function () {
                                     Ext.getCmp("oid").setValue("");
                                     Ext.getCmp("start_time").setValue('');
                                     Ext.getCmp("end_time").setValue("");
                                     Ext.getCmp('username').setValue("");
                                     Ext.getCmp('stateleibie').setValue(3);
                                 }
                             }
                         }
                     ]
                }
        ]
    });
    function Query() {
        var falg = true;
        var oid = Ext.getCmp('oid').getValue();
        var usernames = Ext.getCmp('username');
        var time_start = Ext.getCmp('start_time').getValue();
        var time_end = Ext.getCmp('end_time').getValue();
        if (time_start != null && time_end == null)
        {
            Ext.Msg.alert("提示", "請輸入結束時間");
            return false;
        }
        if (time_end != null && time_start == null)
        {
            Ext.Msg.alert("提示", "請輸入開始時間");
            return false;
        }
        //if (oid.getValue()!=null) { falg = false; }
        //if (usernames.getValue() != undefined) {
        //    var ab = usernames.getValue();
        //    if (ab.trim() != "") {
        //        falg = false;
        //    }
        //}
        //if (falg) {
        //    Ext.Msg.alert("提示", "請輸入查詢條件");
        //    return false;
        //}
        if (oid != null) {
            if (oid < 1) {
                Ext.Msg.alert("提示", "會員/訂單編號必須大於0");
                return false;
            }
        }
        ShippingVoucherStore.removeAll();
        var oid = Ext.getCmp('oid').getValue();
        ShippingVoucherStore.removeAll();
        ShippingVoucherStore.loadPage(1,
            {
                params: {
                    oid: oid,
                    time_start: time_start,
                    time_end: time_end,
                    username: Ext.getCmp('username').getValue(),
                    state: Ext.getCmp("stateleibie").getValue()
                }
            });
    };
    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm, ShippingVoucherPanel],
        renderTo: Ext.getBody(),
        autoScroll: true,//自動顯示滾動條
        listeners: {
            resize: function () {//在组件被调整大小之后触发,首次布局初始化大小时不触发
                ShippingVoucherPanel.clientWidth = document.documentElement.clientWidth;
                this.doLayout();//手动强制这个容器的布局进行重新计算。大多数情况下框架自动完成刷新布局。
            }
        }
    });
});