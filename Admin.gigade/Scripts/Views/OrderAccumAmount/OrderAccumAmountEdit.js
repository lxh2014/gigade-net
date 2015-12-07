editFunction = function (row, store) {
 

    var editUserFrm = Ext.create('Ext.form.Panel', {
        id: 'editUserFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        labelWidth: 40,
        url: '/OrderAccumAmount/SaveOrderAccumAmount',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            
            {
                fieldLabel: '編號',
                xtype: 'textfield',
                id: 'event_id',
                name: 'event_id',
                hidden: true
            },
            {
                fieldLabel: "活動名稱",
                xtype: 'textfield',
                padding: '0 0 0 0',
                id: 'event_name',
                name: 'event_name',
                allowBlank: false
            },
            {
                fieldLabel: "活動描述",
                xtype: 'textfield',
                padding: '0 0 0 0',
                id: 'event_desc',
                name: 'event_desc',
                allowBlank: false
            },
            {
                fieldLabel: "描述開始時間",
                xtype: 'datetimefield',
                id: 'event_desc_start',
                name: 'event_desc_start',
                allowBlank: false,
                editable: false,
                format: 'Y-m-d H:i:s',
                value: Tomorrow(),
                time: { hour: 00, min: 00, sec: 00 },
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("event_desc_start");
                        var end = Ext.getCmp("event_desc_end");
                        if (end.getValue() < start.getValue()) {
                            var start_date = start.getValue();
                            Ext.getCmp('event_desc_end').setValue(new Date(start_date.getFullYear(), start_date.getMonth() + 1, start_date.getDate(), 23, 59, 59));
                        }
                       
                        
                    }
                }
            },
            {
                fieldLabel: "描述結束時間",
                xtype: 'datetimefield',
                id: 'event_desc_end',
                name: 'event_desc_end',
                time: { hour: 23, min: 59, sec: 59 },
                format: 'Y-m-d H:i:s',
                allowBlank: false,
                editable: false,
                value: setNextMonth(Tomorrow(), 1),
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("event_desc_start");
                        var end = Ext.getCmp("event_desc_end");
                        if (end.getValue() < start.getValue()) {
                            var end_date = end.getValue();
                            Ext.getCmp('event_desc_start').setValue(new Date(end_date.getFullYear(), end_date.getMonth() - 1, end_date.getDate()));
                        }
                    }
                }

            },
             {
                 fieldLabel: "開始時間",
                 xtype: 'datetimefield',
                 id: 'event_start_time',
                 name: 'event_start_time',
                 allowBlank: false,
                 editable: false,
                 format: 'Y-m-d H:i:s',
                 value: Tomorrow(),
                 time: { hour: 00, min: 00, sec: 00 },
                 listeners: {
                     select: function (a, b, c) {
                         var start = Ext.getCmp("event_start_time");
                         var end = Ext.getCmp("event_end_time");
                         if (end.getValue() < start.getValue()) {
                             var start_date = start.getValue();
                             Ext.getCmp('event_end_time').setValue(new Date(start_date.getFullYear(), start_date.getMonth() + 1, start_date.getDate(), 23, 59, 59));
                         }


                     }
                 }
             },
            {
                fieldLabel: "結束時間",
                xtype: 'datetimefield',
                id: 'event_end_time',
                name: 'event_end_time',
                time: { hour: 23, min: 59, sec: 59 },
                format: 'Y-m-d H:i:s',
                allowBlank: false,
                editable: false,
                value: setNextMonth(Tomorrow(), 1),
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("event_start_time");
                        var end = Ext.getCmp("event_end_time");
                        if (end.getValue() < start.getValue()) {
                            var end_date = end.getValue();
                            Ext.getCmp('event_start_time').setValue(new Date(end_date.getFullYear(), end_date.getMonth() - 1, end_date.getDate()));
                        }
                    }
                }

            },
            {
                fieldLabel: "累積金額",
                xtype: 'numberfield',
                id: 'accum_amount',
                allowDecimals: false,
                allowNegative:false,
                padding: '0 0 0 0',
                maxLength: 9,
                minValue:0,
                maskRe: /^\d$/,
                name: 'accum_amount',
                value: 0,
               
                allowBlank: false
            },
             {
                 xtype: 'radiogroup',
                 fieldLabel: "活動狀態",
                 id: 'event_status',
                 name: 'event_status',
                 columns: 2,
                 defaults: {
                     flex: 1,
                     name: 'status'
                 },
                 items: [
                     {
                         boxLabel: "啟用", id: 'yes', inputValue: '1', checked: true
                     },
                     {
                         boxLabel: "禁用", id: 'no', inputValue: '0'
                     }
                 ],
             }
        ],
        buttons: [{
            formBind: true,
            disabled: true,
            text: SAVE,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: 'Loading...' });
                    form.submit({
                        params: {
                            event_id: Ext.getCmp('event_id').getValue(),
                            event_name: Ext.getCmp('event_name').getValue(),
                            event_desc: Ext.getCmp('event_desc').getValue(),
                            event_start_time: Ext.getCmp('event_start_time').getValue(),
                            event_end_time: Ext.getCmp('event_end_time').getValue(),
                            event_desc_start: Ext.getCmp('event_desc_start').getValue(),
                            event_desc_end: Ext.getCmp('event_desc_end').getValue(),
                            accum_amount: Ext.getCmp('accum_amount').getValue(),
                            event_desc_start: Ext.getCmp('event_desc_start').getValue(),
                            event_status: Ext.htmlEncode(Ext.getCmp('event_status').getValue().status)
                            
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            myMask.hide();
                            if (result.success == "true") {
                                Ext.Msg.alert("提示信息", result.msg);
                                editUserWin.close();
                                OrderAccumAmountStore.load();
                        
                            }
                            else {
                                Ext.Msg.alert("提示信息", result.msg);
                            }
                        },
                        failure: function (form, action) {
                            myMask.hide();
                            Ext.Msg.alert("提示信息", result.msg);
                        }
                    });
                }
            }
        }]
    });


    var editUserWin = Ext.create('Ext.window.Window', {
        id: 'editUserWin',
        width: 400,
        title: "會員累積金額",
        iconCls: 'icon-user-edit',
        iconCls: row ? "icon-user-edit" : "icon-user-add",
        layout: 'fit',
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        closable: false,
        items: [
                 editUserFrm
        ],
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        tools: [
         {
             type: 'close',
             qtip: "關閉",
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm("提示", "確定關閉?", function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editUserWin').destroy();
                         OrderAccumAmountStore.destroy();
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
                if (row) {
                    editUserFrm.getForm().loadRecord(row); //如果是編輯的話

                }
                else {
                    editUserFrm.getForm().reset(); //如果是新增的話
                }
            }
        }
    });
    if (row != null) {
        if (!row.data.event_status) {
            Ext.getCmp('no').setValue(true);
        }
        else {
            Ext.getCmp('yes').setValue(true);
        }
    }
    editUserWin.show();
}

function Tomorrow() {
    var d;
    var dt;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                     // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += d.getDate();
    dt = new Date(s);
    dt.setDate(dt.getDate() + 1);
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
