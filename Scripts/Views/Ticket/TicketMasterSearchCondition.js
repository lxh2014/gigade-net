//实现验证开始时间必须小于结束时间
//Ext.apply(Ext.form.field.VTypes, {
//    //日期筛选
//    daterange: function (val, field) {
//        var date = field.parseDate(val);
//        if (!date) {
//            return false;
//        }
//        if (field.startDateField && (!this.dateRangeMax || (date.getTime() != this.dateRangeMax.getTime()))) {
//            var start = field.up('form').down('#' + field.startDateField);
//            start.setMaxValue(date);
//            start.validate();
//            this.dateRangeMax = date;
//        }
//        else if (field.endDateField && (!this.dateRangeMin || (date.getTime() != this.dateRangeMin.getTime()))) {
//            var end = field.up('form').down('#' + field.endDateField);
//            end.setMinValue(date);
//            end.validate();
//            this.dateRangeMin = date;
//        }
//        return true;
//    },
//    daterangeText: ''
//});

//查询
Query = function () {
    TicketMasterStore.removeAll();
    Ext.getCmp("gdTicketMaster").store.loadPage(1, {
        params: {
            master_status: Ext.getCmp('master_status').getValue(),//訂單狀態
            order_payment: Ext.getCmp('order_payment').getValue(),//付款方式
            ticket_master_id: Ext.getCmp('o_id').getValue(),//訂單編號
            order_name: Ext.getCmp('o_name').getValue(),//訂購人
            ticket_start: Ext.getCmp('ticket_start').getValue(),//訂單開始時間
            ticket_end: Ext.getCmp('ticket_end').getValue(),//訂單結束時間
            course_search: Ext.getCmp('course_search').getValue(),//課程編號或課程名稱
            course_start: Ext.getCmp('course_start').getValue(),//課程開始時間
            course_end: Ext.getCmp('course_end').getValue(),//課程結束時間
            bill_check: Ext.getCmp('bill_check').getValue(),//是否對賬
            relation_id: "",
             isSecret: true,
        }
    });
}
function TheMonthFirstDay() {
    var times;
    times = new Date();
    return new Date(times.getFullYear(), times.getMonth(), 1);
}
function Tomorrow(days) {
    var d;
    d = new Date();                             // 创建 Date 对象。
    d.setDate(d.getDate() + days);
    return d;
}

function setNextMonth(source, n)
{
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    }
    return s;
}
var frm = Ext.create('Ext.form.Panel', {
    id: 'frm',
    layout: 'anchor',
    height: 180,
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
                    id: 'master_status',
                    name: 'master_status',
                    margin: '0 5px',
                    fieldLabel: '訂單狀態',
                    labelWidth: 120,
                    editable: false,
                    store: paymentType,
                    displayField: 'remark',
                    valueField: 'ParameterCode',
                    emptyText: '所有方式'
                },
                {
                    xtype: 'combobox',
                    id: 'order_payment',
                    name: 'order_payment',
                    margin: '0 5px',
                    fieldLabel: '付款方式',
                    queryMode: 'local',
                    editable: false,
                    store: paymentStore,
                    displayField: 'parameterName',
                    valueField: 'parameterCode',
                    emptyText: '所有方式'
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
                    id: 'o_id',
                    margin: '0 5px',
                    name: 'o_id',
                    fieldLabel: '訂單編號',
                    labelWidth: 120,
                    listeners: {
                        specialkey: function (field, e) {
                            if (e.getKey() == e.ENTER) {
                                Query();
                            }
                        }
                    }
                },
                {
                    xtype: 'textfield',
                    id: 'o_name',
                    name: 'o_name',
                    margin: '0 5px',
                    fieldLabel: '訂購人',
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
                    xtype: 'displayfield',
                    margin: '0 5px',
                    fieldLabel: '訂單日期'
                },
                {
                    xtype: 'datetimefield',
                    id: 'ticket_start',
                    name: 'ticket_start',
                    margin: '0 5px',
                    editable: false,
               
                    format: 'Y-m-d H:i:s',
                    //time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
                    //listeners: {
                    //    select: function (a, b, c) {
                    //        var start = Ext.getCmp("ticket_start");
                    //        var end = Ext.getCmp("ticket_end");
                    //        if (end.getValue() == null) {
                    //            end.setValue(setNextMonth(start.getValue(), 1));
                    //        } else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                    //            Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                    //            end.setValue(setNextMonth(start.getValue(), 1));
                    //        }
                    //    },
                    //    specialkey: function (field, e) {
                    //        if (e.getKey() == e.ENTER) {
                    //            Query();
                    //        }
                    //    }
                    //}

                },
                {
                    xtype: 'displayfield',
                    value: '~'
                },
                {
                    xtype: 'datetimefield',
                    id: 'ticket_end',
                    name: 'ticket_end',
                    margin: '0 5px',
                    format: 'Y-m-d H:i:s',
                    editable: false,
                    //time: { hour: 23, min: 59, sec: 59 },
                    //listeners: {
                    //    select: function (a, b, c) {
                    //        var start = Ext.getCmp("ticket_start");
                    //        var end = Ext.getCmp("ticket_end");
                    //        if (start.getValue() != "" && start.getValue() != null) {
                    //            if (end.getValue() < start.getValue()) {
                    //                Ext.Msg.alert(INFORMATION, DATA_TIP);
                    //                end.setValue(setNextMonth(start.getValue(), 1));
                    //            }
                    //            else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                    //                start.setValue(setNextMonth(end.getValue(), -1));
                    //            }
                    //        }
                    //        else {
                    //            start.setValue(setNextMonth(end.getValue(), -1));
                    //        }
                    //    },
                    //    specialkey: function (field, e) {
                    //        if (e.getKey() == e.ENTER) {
                    //            Query();
                    //        }
                    //    }
                    //}

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
                        id: 'course_search',
                        name: 'course_search',
                        margin: '0 5px',
                        fieldLabel: '課程編號/課程名稱',
                        labelWidth: 120,
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                    {
                        xtype: 'radiogroup',
                        hidden: false,
                        id: 'bill_check',
                        name: 'bill_check',
                        fieldLabel: '是否對賬',
                        colName: 'bill_check',
                        width: 400,
                        margin: '0 5px',
                        columns: 3,
                        vertical: true,
                        items: [
                            {
                                boxLabel: '是',
                                name: 'bill_check',
                                id: 'y',
                                checked: true,
                                inputValue: "1"
                            },
                            {
                                boxLabel: '否',
                                name: 'bill_check',
                                id: 'n',
                                inputValue: "0"
                            }]
                    }]
         },
                       {
                           xtype: 'fieldcontainer',
                           combineErrors: true,
                           layout: 'hbox',
                           items: [
                               {
                                   xtype: 'displayfield',
                                   margin: '0 5px',
                                   fieldLabel: '課程日期'
                               },
                               {
                                   xtype: 'datetimefield',
                                   id: 'course_start',
                                   name: 'course_start',
                                   margin: '0 5px',
                                   format: 'Y-m-d H:i:s',
                                   //time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
                                   //listeners: {
                                   //    select: function (a, b, c) {
                                   //        var start = Ext.getCmp("course_start");
                                   //        var end = Ext.getCmp("course_end");
                                   //        if (end.getValue() == null) {
                                   //            end.setValue(setNextMonth(start.getValue(), 1));
                                   //        } else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                   //            Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                   //            end.setValue(setNextMonth(start.getValue(), 1));
                                   //        }
                                   //    },
                                   //    specialkey: function (field, e) {
                                   //        if (e.getKey() == e.ENTER) {
                                   //            Query();
                                   //        }
                                   //    }
                                   //}
                               },
                               {
                                   xtype: 'displayfield',
                                   value: '~'
                               },
                               {
                                   xtype: 'datetimefield',
                                   id: 'course_end',
                                   name: 'course_end',
                                   margin: '0 5px',
                                   format: 'Y-m-d H:i:s',
                                   editable: false,
                                  // time: { hour: 23, min: 59, sec: 59 },
                                   //listeners: {
                                   //    select: function (a, b, c) {
                                   //        var start = Ext.getCmp("course_start");
                                   //        var end = Ext.getCmp("course_end");
                                   //        if (start.getValue() != "" && start.getValue() != null) {
                                   //            if (end.getValue() < start.getValue()) {
                                   //                Ext.Msg.alert(INFORMATION, DATA_TIP);
                                   //                end.setValue(setNextMonth(start.getValue(), 1));
                                   //            }
                                   //            else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                   //                start.setValue(setNextMonth(end.getValue(), -1));
                                   //            }
                                   //        }
                                   //        else {
                                   //            start.setValue(setNextMonth(end.getValue(), -1));
                                   //        }
                                   //    },
                                   //    specialkey: function (field, e) {
                                   //        if (e.getKey() == e.ENTER) {
                                   //            Query();
                                   //        }
                                   //    }
                                   //}

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
                                Ext.getCmp('master_status').setValue("");
                                Ext.getCmp('order_payment').setValue("");
                                Ext.getCmp('o_id').setValue("");
                                Ext.getCmp('o_name').setValue("");
                                Ext.getCmp('course_search').setValue("");
                                Ext.getCmp('n').setValue(true);
                                Ext.getCmp('ticket_start').setValue();//開始時間--time_start--delivery_date
                                Ext.getCmp('ticket_end').setValue();//結束時間--time_end--delivery_date
                                Ext.getCmp('course_start').setValue();//開始時間--time_start--delivery_date
                                Ext.getCmp('course_end').setValue();//結束時間--time_end--delivery_date
                            }
                        }
                    }
                ]
            }
    ], listeners: {
        beforerender: function () {
            var course_id = document.getElementById("course_id").value;
            if (course_id != "") {
                Ext.getCmp('course_search').setValue(course_id);
            }
        }
    }
});

